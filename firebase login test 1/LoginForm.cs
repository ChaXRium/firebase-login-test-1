using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace firebase_login_test_1
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient _httpClient; private const string FirebaseApiKey = "AIzaSyDJgF3OjwiHIJ2P_p09MbnsE1uSZ9JPCHo"; // Replace if regenerated

        public LoginForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30) // Set timeout for HTTP requests
            };
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnLogin.Enabled = false; // Disable button during API call
            try
            {
                // Create the request payload
                var loginData = new
                {
                    email,
                    password,
                    returnSecureToken = true
                };
                string json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send POST request to Firebase Authentication API
                string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseApiKey}";
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Read the response
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<FirebaseAuthResponse>(responseBody);

                    // Store the ID token for future API calls (optional)
                    string idToken = result.IdToken;

                    // Open the main menu form
                    MainMenuForm mainMenu = new MainMenuForm();
                    mainMenu.Show();
                    this.Hide(); // Hide the login form
                }
                else
                {
                    // Handle error response
                    string errorBody = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonConvert.DeserializeObject<FirebaseErrorResponse>(errorBody);
                    string errorMessage = errorResult.Error.Message switch
                    {
                        "EMAIL_NOT_FOUND" => "No account found with this email.",
                        "INVALID_PASSWORD" => "Incorrect password.",
                        "USER_DISABLED" => "This account has been disabled.",
                        "TOO_MANY_ATTEMPTS_TRY_LATER" => "Too many attempts. Please try again later.",
                        _ => $"Login failed: {errorResult.Error.Message}"
                    };
                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Network error: {ex.Message}. Please check your internet connection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true; // Re-enable button
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _httpClient.Dispose(); // Clean up HttpClient
            Application.Exit(); // Exit the application when the login form is closed
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(100,0,0,0); // Set panel background color to white
        }
    }

    // Classes to deserialize Firebase responses
    public class FirebaseAuthResponse
    {
        public string IdToken { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
        public string LocalId { get; set; }
    }

    public class FirebaseErrorResponse
    {
        public FirebaseError Error { get; set; }
    }

    public class FirebaseError
    {
        public string Message { get; set; }
    }

}