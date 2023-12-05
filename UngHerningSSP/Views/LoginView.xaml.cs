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
using UngHerningSSP.ViewModels;

namespace UngHerningSSP.Views;
/// <summary>
/// Interaction logic for LoginView.xaml
/// </summary>
public partial class LoginView : Page
{
	public LoginView()
	{
		InitializeComponent();
	}

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string username = UsernameTextBox.Text;
        string password = PasswordBox.Password;
        string path = "User.txt";


        if (username == "test" && password == "test")
        {

            // TODO: Add code to navigate to the main application window or perform other actions.
            NavigationService.Navigate(new UserMapView());
        }


        //if (username.Length <= 1 || password.Length <= 1 || username == null || password == null)
        //{
        //    MessageBox.Show("Login failed. Please try again.");

        //}

        //else if (File.Exists(path))
        //{
        //    string[] lines = File.ReadAllLines(path);
        //    bool matchFound = false;

        //    foreach (string line in lines)
        //    {
        //        if (line.Contains(UsernameTextBox.Text) && line.Contains(PasswordBox.Password) && line.Contains("True"))
        //        {
        //            NavigationService.Navigate(new Files());
        //            matchFound = true;
        //            break;
        //        }
        //        else if (line.Contains(UsernameTextBox.Text) && line.Contains(PasswordBox.Password) && line.Contains("False"))
        //        {
        //            NavigationService.Navigate(new FilesNoAdmin());
        //            matchFound = true;
        //            break;
        //        }
        //    }

        //    if (!matchFound)
        //    {
        //        MessageBox.Show("Login failed. Please try again.");
        //    }
        //}
    }
}
