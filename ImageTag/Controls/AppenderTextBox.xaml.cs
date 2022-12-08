using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ImageTag.Code;

namespace ImageTag.Controls
{
    /// <summary>
    /// Interaction logic for AppenderTextBox.xaml
    /// </summary>
    public partial class AppenderTextBox : UserControl
    {
        //public Level FilterLevel = Level.Debug;

        public AppenderTextBox()
        {
            InitializeComponent();

            //var root = ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root;
            //var attachable = root as IAppenderAttachable;
            
            //attachable.AddAppender(this);
        }

        public void Close()
        {
            
        }

        //public void DoAppend(LoggingEvent loggingEvent)
        //{
        //    this.TextBlock.Dispatcher.Invoke(
        //        DispatcherPriority.Normal,
        //        new Action(
        //            delegate
        //            {
        //                if (loggingEvent.Level <= FilterLevel)
        //                    return;

        //                var color = Colors.Black;

        //                if (loggingEvent.Level == Level.Warn)
        //                    color = Colors.Orange;
        //                if (loggingEvent.Level == Level.Error)
        //                    color = Colors.Red;
        //                if (loggingEvent.Level == Level.Debug)
        //                    color = Colors.DarkBlue;
                        
        //                this.TextBlock.AppendText(loggingEvent.RenderedMessage + "\r", color);
        //            }));
        //}

        public void ScrollToEnd()
        {
            TextBlock.ScrollToEnd();
        }

        public void Clear()
        {
            TextBlock.Document.Blocks.Clear();
        }
    }
}
