using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone;
using Microsoft.Phone.Tasks;

namespace panes
{
    public partial class MainPage : PhoneApplicationPage
    {

        CameraCaptureTask ctask;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"/Page1.xaml", UriKind.Relative));
        }

        private void PlayCreate_Click(object sender, RoutedEventArgs e)
        {
            //Create new instance of CameraCaptureClass
           ctask = new CameraCaptureTask();

            //Create new event handler for capturing a photo
            ctask.Completed += new EventHandler<PhotoResult>(ctask_Completed);
            ctask.Show();
        }

        void ctask_Completed(object sender, PhotoResult e)
        {

            if (e.TaskResult == TaskResult.OK && e.ChosenPhoto != null)
            {
                // TODO: Send photo to Page1
                System.Diagnostics.Debug.WriteLine(e.ChosenPhoto);
                NavigationService.Navigate(new Uri(@"/Page1.xaml", UriKind.Relative));

            }
        }

        private void PlayInstructions_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"/InstructionsPage.xaml", UriKind.Relative));
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}