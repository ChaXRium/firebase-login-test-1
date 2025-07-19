using System;
using System.Windows.Forms;

namespace firebase_login_test_1 { static class Program { [STAThread] static void Main() { Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false); Application.Run(new LoginForm()); } } }