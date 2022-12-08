using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ImageTag.Code;
using ImageTag.Data;
using ImageTag.Model;
using Microsoft.Extensions.Logging;
using Image = ImageTag.Data.Image;
using Path = System.Windows.Shapes.Path;

namespace ImageTag.Controls.Forms
{
    /// <summary>
    /// Interaction logic for ImageTaggerForm.xaml
    /// </summary>
    public partial class ImageTaggerForm : UserControl
    {
        private readonly ILogger<ImageTaggerForm> logger;
        private readonly ImageTagContext context;
        private readonly ImageTagSettings settings;
        private readonly Code.ImageTag imageTag;
        protected List<Image> IndexImages = new List<Image>();

        protected bool MultipleSelected = false;
        protected bool IsUpdating = false;

        protected List<TagModel> LastSelectedTags = new List<TagModel>();

        public ImageTaggerForm(
            ILogger<ImageTaggerForm> logger,
            ImageTagContext context,
            ImageTagSettings settings,
            Code.ImageTag imageTag)
        {
            this.logger = logger;
            this.settings = settings;
            this.context = context;
            this.imageTag = imageTag;
            InitializeComponent();
        }

        public void Initialize()
        {
            var defaultDirectory = settings.DefaultDirectory;
            if (String.IsNullOrEmpty(defaultDirectory) || !Directory.Exists(defaultDirectory))
            {
                defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
            }

            FileTree.SetRootDirectory(defaultDirectory);

            TagSelectControl.Initialize();
        }


        public void ClearThumbItems()
        {
            FileExplorer.ClearThumbs();
        }

        private void FileTree_OnOnPickDirectory(string directory)
        {
            //FileTree.IsEnabled = false;
            Task.Run(() =>
            {
                FileExplorer.UpdateExplorerWindowPath(directory);

                App.Current.Dispatcher.Invoke(() =>
                {
                    //FileTree.IsEnabled = true;
                });
            });
        }



        private void TagSelectControl_OnOnTagAdd(TagModel selecteditem)
        {
            
            // Get list of images
            var imageList = GetSelectedImages();

            Util.UpdateImageTags(context, imageList, new List<TagModel>() {selecteditem}, new CancellationToken(), addOnly:true);

            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.LogError("Couldn't tag images: " + ex.Message);
                throw ex;
            }

            if (!LastSelectedTags.Contains(selecteditem))
                LastSelectedTags.Add(selecteditem);
            

            // Update recent tag box?
            TagSelectControl.UpdateRecentTags();
        }


        protected List<ImageFileThumbData> GetSelectedImages()
        {
            var imageList = new List<ImageFileThumbData>();

            var selected = FileExplorer.FileViewer.SelectedItems;
            foreach (var selectedItem in selected)
            {
                var tempItem = selectedItem as ImageFileThumbData;
                if (tempItem != null)
                {
                    imageList.Add(tempItem);
                }
            }
            return imageList;
        }

        protected void LoadImageRecords(List<ImageFileThumbData> images)
        {
            foreach (var image in images)
            {
                if (image.Image == null)
                {
                    // Try and load it
                    image.Image = context.Images.FirstOrDefault(x => x.Path == image.FullPath);
                }
            }
        }

        protected List<TagModel> GetSelectedTags()
        {
            var tagList = new List<TagModel>();
            
            var selected = TagSelectControl.TagViewer.SelectedItems;
            foreach (var selectedItem in selected)
            {
                var tempItem = selectedItem as TagModel;
                if (tempItem != null)
                    tagList.Add(tempItem);
            }
            return tagList;
        }

        private void TagSelectControl_OnOnRatingChanged(int rating)
        {
            if (!IsUpdating)
            {
                var imageList = GetSelectedImages();

                var dispatchItem = new ImageTagDispatchItem()
                {
                    Action = () => UpdateRatingForSelectedImages(imageList, rating),
                    Description = "Updating rating to: " + rating,
                };

                imageTag.Enqueue(dispatchItem);
            }

        }

        public void UpdateRatingForSelectedImages(List<ImageFileThumbData> imageList, int rating)
        {
            LoadImageRecords(imageList);

            Util.UpdateImageRating(context, imageList, rating);

            //if (App.CancellationTokenSource.IsCancellationRequested)
            //{
            //    App.Log.Info("Image rating change was canceled.");
            //    return;
            //}

            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.LogError("Couldn't update rating of images: " + ex.Message);
                throw ex;
            }
        }

        private void TagSelectControl_OnOnSelectionChanged(List<TagModel> tags)
        {
            // Get list of images
            var imageList = GetSelectedImages();
            
            var dispatchItem = new ImageTagDispatchItem()
            {
                Action = () => UpdateTagsForSelectedImages(imageList, tags),
                Description = "Updating tags to: " + String.Join(", ", tags.Select(x => x.Tag.Name)),
            };
            imageTag.Enqueue(dispatchItem);
        }

        protected void UpdateTagsForSelectedImages(List<ImageFileThumbData> imageList, List<TagModel> tags)
        {
            var token = new CancellationToken();
            if (!IsUpdating)
            {
                LoadImageRecords(imageList);

                if (MultipleSelected)
                {

                    var tagIDs = tags.Select(x => (int)x.Tag.ID).ToArray();

                    // Tags removed
                    var removedTags = LastSelectedTags.Where(x => !tagIDs.Contains((int)x.Tag.ID)).ToList();


                    Util.RemoveImageTags(imageList, removedTags);
                    Util.UpdateImageTags(context, imageList, tags, token, addOnly: true);

                    LastSelectedTags = tags;
                }
                else
                    Util.UpdateImageTags(context, imageList, tags, token);


                var now = DateTime.Now;

                //var token = App.CancellationTokenSource.Token;

                if (token.IsCancellationRequested)
                {
                    logger.LogInformation("Image tag was canceled.");
                    return;
                }

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.LogError("Couldn't tag images: " + ex.Message);
                    throw ex;
                }

                var span = DateTime.Now - now;
                //App.Log.Info("Saving entities: " + span);

                
                // Update recent tag box?
                //TagSelectControl.UpdateRecentTags();
            }
        }

        private void FileExplorer_OnOnFileSelected(ImageFileThumbData data)
        {
            TagSelectControl.SetImageData(data);

            TagSelectControl.DisableTagViewerSelectionChange();

            if (FileExplorer.FileViewer.SelectedItems.Count > 1)
            {
                IsUpdating = true;

                MultipleSelected = true;

                TagSelectControl.TagViewer.UnselectAll();
                LastSelectedTags.Clear();


                var thumbItems = FileExplorer.FileViewer.SelectedItems;
                var list = new List<ImageFileThumbData>();
                foreach (var thumbItem in thumbItems)
                {
                    list.Add(thumbItem as ImageFileThumbData);
                }

                LastSelectedTags = TagSelectControl.SetCommonTags(list);

                IsUpdating = false;
            }
            else
                MultipleSelected = false;


            TagSelectControl.EnableTagViewerSelectionChange();
        }

    }
}
