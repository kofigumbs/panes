using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;

namespace panes
{
    public partial class CreatePage : PhoneApplicationPage
    {

        //The camera chooser used to capture a picture.
        CameraCaptureTask ctask;

        // Constructor
        public CreatePage()
        {
            InitializeComponent();

            SupportedOrientations = SupportedPageOrientation.Portrait | SupportedPageOrientation.Landscape;

            //Create new instance of CameraCaptureClass
            ctask = new CameraCaptureTask();

            //Create new event handler for capturing a photo
            ctask.Completed += new EventHandler<PhotoResult>(ctask_Completed);

        }

        /// <summary>
        /// Event handler for retrieving the JPEG photo stream.
        /// Also to for decoding JPEG stream into a writeable bitmap and displaying.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ctask_Completed(object sender, PhotoResult e)
        {

            if (e.TaskResult == TaskResult.OK && e.ChosenPhoto != null)
            {

                NavigationService.Navigate(new Uri(@"/Page1.xaml", UriKind.Relative));

            }
        }

        private void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Show the camera.
            ctask.Show();
        }
    }
}