using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MyProject007.BLL;
using MyProject007.Helpers;

namespace MyProject007
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService = new AuthService();

        private const string HINT_EMAIL = "Örn: ad@banka.com";
        private const string HINT_PASS = "Parolanızı giriniz";

        private Panel pnlCard;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblEmailWarn;
        private Label lblPassword;
        private TextBox txtPassword;
        private CheckBox chkShowPassword;
        private CheckBox chkRememberMe;
        private Button btnLogin;
        private LinkLabel lnkForgotPassword;

        public LoginForm()
        {
            InitializeCustomDesign();
            LoadRememberedCredentials();
        }

        private void InitializeCustomDesign()
        {
            this.Text = "ARK BANK - Kurumsal & Özel Bankacılık";
            this.Size = new Size(480, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = UiTheme.BgDark;

            pnlCard = new Panel
            {
                Size = new Size(400, 480),
                Location = new Point(32, 25),
                BackColor = UiTheme.CardBg
            };

            lblTitle = new Label
            {
                Text = "ARK BANK",
                ForeColor = UiTheme.VibrantGold,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(35, 25),
                AutoSize = true
            };

            lblSubtitle = new Label
            {
                Text = "Geleceğin Güvenli Dijital Bankacılığı",
                ForeColor = UiTheme.LightIceBlue,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(37, 65),
                AutoSize = true
            };


            lblEmail = new Label { Text = "E-Posta Adresi", ForeColor = UiTheme.PureWhite, Font = new Font("Segoe UI", 9.5F), Location = new Point(35, 115), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(35, 140), Size = new Size(330, 28) };
            UiTheme.StyleTextBox(txtEmail);
            UiTheme.SetPlaceholder(txtEmail, HINT_EMAIL);
            txtEmail.TextChanged += TxtEmail_TextChanged;

            lblEmailWarn = new Label { Text = "", Font = new Font("Segoe UI", 8.5F, FontStyle.Italic), Location = new Point(35, 170), AutoSize = true };

            
            lblPassword = new Label { Text = "Parola", ForeColor = UiTheme.PureWhite, Font = new Font("Segoe UI", 9.5F), Location = new Point(35, 195), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(35, 220), Size = new Size(330, 28) };
            UiTheme.StyleTextBox(txtPassword);
            UiTheme.SetPlaceholder(txtPassword, HINT_PASS, isPassword: true);

           
            chkShowPassword = new CheckBox
            {
                Text = "Şifreyi Göster",
                ForeColor = UiTheme.LightIceBlue,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(35, 255),
                AutoSize = true
            };
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                if (txtPassword.Text != HINT_PASS)
                    txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            };

            
            chkRememberMe = new CheckBox
            {
                Text = "Beni Hatırla",
                ForeColor = UiTheme.LightIceBlue,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(245, 255),
                AutoSize = true
            };

            
            btnLogin = new Button { Text = "GİRİŞ YAP", Location = new Point(35, 305), Size = new Size(330, 42) };
            UiTheme.StyleButton(btnLogin, UiTheme.VibrantGold, Color.FromArgb(10, 18, 36));
            btnLogin.Click += BtnLogin_Click;

          
            lnkForgotPassword = new LinkLabel
            {
                Text = "Şifrenizi mi unuttunuz?",
                LinkColor = UiTheme.LightIceBlue,
                ActiveLinkColor = UiTheme.VibrantGold,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(135, 370),
                AutoSize = true
            };
            lnkForgotPassword.LinkClicked += (s, e) =>
            {
                this.Hide();
                using (var forgotForm = new ForgotPasswordForm())
                {
                    forgotForm.ShowDialog();
                }
                this.Show();
            };

            pnlCard.Controls.Add(lblTitle);
            pnlCard.Controls.Add(lblSubtitle);
            pnlCard.Controls.Add(lblEmail);
            pnlCard.Controls.Add(txtEmail);
            pnlCard.Controls.Add(lblEmailWarn);
            pnlCard.Controls.Add(lblPassword);
            pnlCard.Controls.Add(txtPassword);
            pnlCard.Controls.Add(chkShowPassword);
            pnlCard.Controls.Add(chkRememberMe);
            pnlCard.Controls.Add(btnLogin);
            pnlCard.Controls.Add(lnkForgotPassword);

            this.Controls.Add(pnlCard);
            this.AcceptButton = btnLogin;
        }

        private void TxtEmail_TextChanged(object sender, EventArgs e)
        {
            string email = (txtEmail.Text == HINT_EMAIL) ? "" : txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                lblEmailWarn.Text = "";
                return;
            }

            bool isValid = Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (isValid)
            {
                lblEmailWarn.Text = "✓ Geçerli e-posta biçimi";
                lblEmailWarn.ForeColor = Color.FromArgb(74, 222, 128);
            }
            else
            {
                lblEmailWarn.Text = "⚠ Geçersiz e-posta formatı";
                lblEmailWarn.ForeColor = UiTheme.CrimsonRed;
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = (txtEmail.Text == HINT_EMAIL) ? "" : txtEmail.Text.Trim();
            string pass = (txtPassword.Text == HINT_PASS) ? "" : txtPassword.Text;

            try
            {
                var user = _authService.Login(email, pass);

                if (chkRememberMe.Checked)
                {
                    Properties.Settings.Default.RememberedEmail = email;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.RememberedEmail = "";
                    Properties.Settings.Default.Save();
                }

                if (user.MustChangePassword)
                {
                    MessageBox.Show("İlk girişiniz olduğundan güvenlik gereği parolanızı değiştirmeniz gerekmektedir.",
                        "Zorunlu Parola Değişimi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    using (var changeForm = new ChangePasswordForm(user.UserId, isForced: true))
                    {
                        if (changeForm.ShowDialog() != DialogResult.OK)
                        {
                            Session.SessionTemizle();
                            this.Show();
                            return;
                        }
                    }
                }

                MainForm mainForm = new MainForm();
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRememberedCredentials()
        {
            try
            {
                string savedEmail = Properties.Settings.Default.RememberedEmail;
                if (!string.IsNullOrEmpty(savedEmail))
                {
                    txtEmail.Text = savedEmail;
                    txtEmail.ForeColor = UiTheme.PureWhite;
                    chkRememberMe.Checked = true;
                    this.ActiveControl = txtPassword;
                }
            }
            catch { }
        }
    }
}