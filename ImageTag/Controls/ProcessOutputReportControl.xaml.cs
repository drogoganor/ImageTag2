using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ImageTag.Code;
using ImageTag.Controls.Windows;
using Microsoft.Extensions.Logging;
using Path = System.IO.Path;

namespace ImageTag.Controls
{
    /// <summary>
    /// Interaction logic for ProcessOutputReportControl.xaml
    /// </summary>
    public partial class ProcessOutputReportControl : UserControl
    {
        protected ProcessOperation Process;
        protected ProcessOutputReport Report;

        private readonly ImagetagContext context;
        private readonly ILogger<ProcessOutputReportControl> logger;

        // TODO: Figure out how to inject dependencies
        public ProcessOutputReportControl()
        {

        }

        public ProcessOutputReportControl(
            ILogger<ProcessOutputReportControl> logger,
            ImagetagContext context)
        {
            this.logger = logger;
            this.context = context;
            InitializeComponent();
        }

        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var obj = ListBox.SelectedItem as ProcessOperation;
            if (obj != null)
            {
                Process = obj;

            }
        }

        public void SetProcessReport(ProcessOutputReport report)
        {
            Report = report;
            if (Report != null)
            {
                ListBox.ItemsSource = Report.Operations.Where(x => x.Severity == FileProcessSeverity.Warn);
            }
            else
            {
                ListBox.ItemsSource = null;
            }
        }


        private void AttemptResolutionButton_Click(object sender, RoutedEventArgs e)
        {
            //if (App.CheckForProcessingItems())
            //    return;

            if (Process != null)
            {
                if (Process.Output == ProcessRecommendedOutput.CompareFiles)
                {
                    string result = string.Empty;
                    bool bothImages = false;
                    bool renameOne = false;
                    var ok = ImageCompareWindow.ShowDialog(Process.SourceFilename, Process.DestinationFilename,
                        out result, out bothImages, out renameOne);

                    if (ok && !String.IsNullOrEmpty(result))
                    {
                        result = result.Trim().ToLowerInvariant();
                        if (bothImages)
                        {
                            // Keep both
                            if (renameOne)
                            {
                                var targetImage = context.Images.FirstOrDefault(x => x.Path.Trim().ToLower() == result);

                                if (targetImage != null && File.Exists(targetImage.Path))
                                {
                                    var suffix = Path.GetExtension(targetImage.Path);
                                    var newPath = targetImage.Path.Substring(0, targetImage.Path.Length - suffix.Length)
                                                  + " " + Path.GetRandomFileName().Substring(0, 6)
                                                  + suffix;
                                    
                                    if (!Util.RetryMove(logger, targetImage.Path, newPath))
                                    {
                                        logger.LogError("Couldn't move file: " + targetImage.Path + " to " + newPath);
                                        return;
                                    }
                                    
                                    targetImage.Path = newPath;
                                
                                    context.SaveChanges();

                                    logger.LogInformation("Saved renamed file as: " + newPath);
                                }
                            }

                        }
                        else
                        {
                            // Discard one

                            var discardedImageName = result == Process.SourceFilename
                               ? Process.DestinationFilename
                               : Process.SourceFilename;
                            discardedImageName = discardedImageName.Trim().ToLowerInvariant();

                            var targetImage = context.Images.FirstOrDefault(x => x.Path.Trim().ToLower() == result);
                            var discardedImage = context.Images.FirstOrDefault(x => x.Path.Trim().ToLower() == discardedImageName);

                            if (targetImage != null && discardedImage != null)
                            {
                                // Consolidate tags
                                foreach (var discardedImageTag in discardedImage.Tags)
                                {
                                    targetImage.Tags.Add(discardedImageTag);
                                }

                                // Rating, too
                                if (!targetImage.Rating.HasValue && discardedImage.Rating.HasValue)
                                {
                                    targetImage.Rating = discardedImage.Rating;
                                }



                                context.Images.Remove(discardedImage);

                                context.SaveChanges();

                                logger.LogInformation("Removed image from DB: " + discardedImage.Path);
                                
                                try
                                {
                                    File.Delete(discardedImage.Path);
                                    logger.LogInformation("Deleted image file: " + discardedImage.Path);
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError("Error deleting image file: " + discardedImage.Path + ": " + ex.Message);
                                    throw ex;
                                }


                                // Update process report items - remove any with the same target path as our discarded
                                if (Report != null)
                                {
                                    var removeList = new List<ProcessOperation>();
                                    foreach (var processOperation in Report.Operations)
                                    {
                                        if (processOperation.Output == Process.Output)
                                        {
                                            // If the source is the same as the discarded image, remove
                                            if (processOperation.SourceFilename.ToLowerInvariant().Trim() == discardedImageName
                                                || processOperation.DestinationFilename.ToLowerInvariant().Trim() == discardedImageName)
                                            {
                                                removeList.Add(processOperation);
                                            }
                                        }
                                    }

                                    foreach (var processOperation in removeList)
                                    {
                                        Report.Operations.Remove(processOperation);
                                    }

                                }



                            }
                            else
                            {
                                logger.LogError("Error: Either target image or discarded image couldn't be found: " + result +
                                              ", " + discardedImageName);
                                return;
                            }


                            // Remove ourselves too
                            if (Report != null)
                            {
                                Report.Operations.Remove(Process);
                                Process = null;

                                SetProcessReport(Report);
                            }
                        }

                        
                    }
                }
            }
        }
    }
}
