using System;
using System.Drawing;
using System.Windows.Forms;
using MyProject007.BLL;

namespace MyProject007
{
    public partial class ResetPasswordForm : Form
    {
        private readonly AuthService _authService = new AuthService();
        private readonly string _initialToken;

        private const string HINT_TOKEN = "Güvenlik Kodunu (Token) Yapıştırın";
        private const string HINT_NEW = "Yeni parolanızı giriniz";
        private const string HINT_CONFIRM = "Yeni parolanızı tekrar giriniz";

        private TextBox txtToken;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkShowPassword; 
        private Button btnReset;
        private Button btnCancel;

        public ResetPasswordForm(string token = "")
        {
            _initialToken = token;
            InitializeDesign();
        }

        private void InitializeDesign()
        {
            this.Text = "ARK BANK - Parola Sıfırlama ve Yenileme";
            this.Size = new Size(460, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = UiTheme.BgDark;

            Panel card = new Panel { Size = new Size(400, 410), Location = new Point(22, 15), BackColor = UiTheme.CardBg };

            Label lblTitle = new Label
            {
                Text = "YENİ PAROLA OLUŞTURMA",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = UiTheme.VibrantGold,
                Location = new Point(25, 20),
                AutoSize = true
            };

        
            Label lblToken = new Label { Text = "Sıfırlama Güvenlik Kodu (Token)", Location = new Point(25, 65), ForeColor = UiTheme.PureWhite, AutoSize = true };
            txtToken = new TextBox { Location = new Point(25, 88), Size = new Size(350, 28) };
            UiTheme.StyleTextBox(txtToken);
            if (!string.IsNullOrEmpty(_initialToken))
            {
                txtToken.Text = _initialToken;
                txtToken.ForeColor = UiTheme.PureWhite;
            }
            else
            {
                UiTheme.SetPlaceholder(txtToken, HINT_TOKEN);
            }

           
            Label lblNew = new Label { Text = "Yeni Parola", Location = new Point(25, 128), ForeColor = UiTheme.PureWhite, AutoSize = true };
            txtNewPassword = new TextBox { Location = new Point(25, 150), Size = new Size(350, 28) };
            UiTheme.StyleTextBox(txtNewPassword);
            UiTheme.SetPlaceholder(txtNewPassword, HINT_NEW, isPassword: true);

           
            Label lblConfirm = new Label { Text = "Yeni Parola (Tekrar)", Location = new Point(25, 190), ForeColor = UiTheme.PureWhite, AutoSize = true };
            txtConfirmPassword = new TextBox { Location = new Point(25, 212), Size = new Size(350, 28) };
            UiTheme.StyleTextBox(txtConfirmPassword);
            UiTheme.SetPlaceholder(txtConfirmPassword, HINT_CONFIRM, isPassword: true);

            
            chkShowPassword = new CheckBox
            {
                Text = "Şifreleri Göster",
                ForeColor = UiTheme.LightIceBlue,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(25, 250),
                AutoSize = true
            };
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                bool show = chkShowPassword.Checked;
                if (txtNewPassword.Text != HINT_NEW) txtNewPassword.UseSystemPasswordChar = !show;
                if (txtConfirmPassword.Text != HINT_CONFIRM) txtConfirmPassword.UseSystemPasswordChar = !show;
            };

        
            btnReset = new Button { Text = "PAROLAYI SIFIRLA VE KAYDET", Location = new Point(25, 290), Size = new Size(350, 40) };
            UiTheme.StyleButton(btnReset, UiTheme.VibrantGold, Color.FromArgb(10, 18, 36));
            btnReset.Click += BtnReset_Click;

            btnCancel = new Button { Text = "İPTAL", Location = new Point(25, 340), Size = new Size(350, 34) };
            UiTheme.StyleButton(btnCancel, UiTheme.CardLightBg, UiTheme.PureWhite);
            btnCancel.Click += (s, e) => this.Close();

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblToken);
            card.Controls.Add(txtToken);
            card.Controls.Add(lblNew);
            card.Controls.Add(txtNewPassword);
            card.Controls.Add(lblConfirm);
            card.Controls.Add(txtConfirmPassword);
            card.Controls.Add(chkShowPassword);
            card.Controls.Add(btnReset);
            card.Controls.Add(btnCancel);

            this.Controls.Add(card);
            this.ActiveControl = lblTitle;
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            string token = (txtToken.Text == HINT_TOKEN) ? "" : txtToken.Text.Trim();
            string newPass = (txtNewPassword.Text == HINT_NEW) ? "" : txtNewPassword.Text;
            string confirmPass = (txtConfirmPassword.Text == HINT_CONFIRM) ? "" : txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirmPass))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Parolalar birbiriyle eşleşmiyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _authService.ResetPasswordWithToken(token, newPass);
                MessageBox.Show("Parolanız başarıyla sıfırlandı. Yeni parolanızla giriş yapabilirsiniz.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sıfırlama Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}