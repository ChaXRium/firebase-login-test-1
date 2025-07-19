using System;
using System.Windows.Forms;

namespace firebase_login_test_1
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm() { InitializeComponent(); }

        private void MainMenuForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            new LoginForm().Show(); // Show the login form again when the main menu is closed
        }
    }

}