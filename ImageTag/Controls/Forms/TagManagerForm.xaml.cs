using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageTag.Model;

namespace ImageTag.Controls.Forms
{
    /// <summary>
    /// Interaction logic for TagManagerControl.xaml
    /// </summary>
    public partial class TagManagerForm : UserControl
    {
        public delegate void TagsUpdatedHandler();

        public event TagsUpdatedHandler OnTagsUpdated;

        protected List<TagModel> Tags;
        protected Tag SelectedTag;
        private readonly ImagetagContext context;
        //private readonly ViewModel.ImageTagViewModel imageTag;

        public TagManagerForm()
        {
            context = App.Current.ViewModel.Context;
            InitializeComponent();
        }

        public void Initialize()
        {
            TypeCombo.ItemsSource = Enum.GetNames(typeof(TagType));
            TypeCombo.SelectedIndex = 0;

            UpdateTagList();
        }


        private void UpdateTagList()
        {
            // Get tags
            Tags = context.Tags
                .Select(x => new TagModel()
                {
                    Tag = x
                })
                .OrderBy(x => x.Tag.TagType)
                .ThenBy(x => x.Tag.Name)
                .ToList();

            foreach (var tagModel in Tags)
            {
                var type = (TagType) tagModel.Tag.TagType;

                //tagModel.HexColor = imageTag.GetColorForTagType(type); //.ToHexString();
            }

            TagList.ItemsSource = Tags;
            ChildTagList.ItemsSource = Tags;

            if (Tags.Count > 0)
            {
                TagList.SelectedIndex = 0;
            }
        }
        


        private void TagList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = TagList.SelectedItem;
            var tag = selectedItem as TagModel;

            if (tag != null)
            {
                SelectedTag = tag.Tag;

                NameTextBox.Text = SelectedTag.Name;
                DescriptionTextBox.Text = SelectedTag.Description;
                TypeCombo.SelectedIndex = (int)SelectedTag.TagType;

                SelectChildTagsForTag(tag);

                UpdateParentsForTag(tag);
            }
        }


        public void UpdateParentsForTag(TagModel tag)
        {
            var parents = tag.Tag.Parents.Select(x => x.Name).ToArray();
            var fullString = String.Join(", ", parents);
            ParentsLabel.Content = fullString;
        }


        private void SelectChildTagsForTag(TagModel tag)
        {
            var selectedIDs = tag.Tag.Children.Select(x => (int) x.Id).ToList();

            var childTags = ChildTagList.ItemsSource as IEnumerable<TagModel>;
            if (childTags != null)
            {
                ChildTagList.UnselectAll();

                var selectedTags = childTags.Where(x => selectedIDs.Contains((int) x.Tag.Id));

                foreach (var selectedTag in selectedTags)
                {
                    ChildTagList.SelectedItems.Add(selectedTag);
                }
            }
        }



        private void ChildTagList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            var selectedItem = TagList.SelectedItem;
            var tag = selectedItem as TagModel;

            if (tag != null)
            {
                SelectedTag = tag.Tag;

                NameTextBox.Text = SelectedTag.Name;
                DescriptionTextBox.Text = SelectedTag.Description;
                TypeCombo.SelectedIndex = (int)SelectedTag.TagType;
            }*/


            // TODO




        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Rename current tag
            var proposedname = NameTextBox.Text;

            if (SelectedTag != null && !String.IsNullOrEmpty(proposedname))
            {
                // First check there's not another one by this name
                if (Tags.Any(x => x.Tag.Name == proposedname && x.Tag.Id != SelectedTag.Id))
                {
                    // Already exists
                    MessageBox.Show("There's already a tag by this name.");
                    return;
                }


                SelectedTag.Name = NameTextBox.Text;
                SelectedTag.Description = DescriptionTextBox.Text;
                SelectedTag.TagType = TypeCombo.SelectedIndex;


                // Get the selected child tags
                foreach (var selectedItem in ChildTagList.SelectedItems)
                {
                    var tagItem = selectedItem as TagModel;
                    SelectedTag.Children.Add(tagItem.Tag);
                }




                context.SaveChanges();

                int selectedIndex = TagList.SelectedIndex;
                UpdateTagList();
                TagList.SelectedIndex = selectedIndex;
                


                OnTagsUpdated?.Invoke();
            }
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTag != null)
            {
                var dlgResult = MessageBox.Show("Are you sure you want to delete the tag " + SelectedTag.Name + "?",
                    "Delete Tag", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dlgResult == MessageBoxResult.Yes)
                {
                    context.Tags.Remove(SelectedTag);

                    context.SaveChanges();

                    UpdateTagList();

                    OnTagsUpdated?.Invoke();
                }
            }

        }


        private void NewButton_OnClick(object sender, RoutedEventArgs e)
        {
            var proposedname = NameTextBox.Text;

            if (!String.IsNullOrEmpty(proposedname))
            {
                // First check there's not another one by this name
                if (Tags.Any(x => x.Tag.Name == proposedname))
                {
                    // Already exists
                    MessageBox.Show("There's already a tag by this name.");
                    return;
                }

                SelectedTag = new Tag();

                SelectedTag.Name = NameTextBox.Text;
                SelectedTag.Description = DescriptionTextBox.Text;
                SelectedTag.TagType = TypeCombo.SelectedIndex;
                
                context.Tags.Add(SelectedTag);

                context.SaveChanges();

                UpdateTagList();
                TagList.SelectedIndex = 0;

                OnTagsUpdated?.Invoke();
            }
        }
    }
}
