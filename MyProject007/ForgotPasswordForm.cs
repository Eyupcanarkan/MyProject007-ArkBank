using System;
using System.Drawing;
using System.Windows.Forms;
using MyProject007.BLL;

namespace MyProject007
{
    public partial class ForgotPasswordForm : Form
    {
        private readonly AuthService _authService = new AuthService();
        private TextBox txtEmail;
        private Button btnSend;


        public ForgotPasswordForm()
        {
            InitializeDesign();
        }

        private void InitializeDesign()
        {
            this.Text = "Şifremi Unuttum - Token Üretimi";
            this.Size = new Size(420, 260);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = UiTheme.PrimaryNavy;

            Panel card = new Panel { Size = new Size(360, 180), Location = new Point(22, 20), BackColor = UiTheme.CardBg };

            Label lblEmail = new Label { Text = "Kayıtlı E-Posta Adresiniz", Location = new Point(25, 25), ForeColor = UiTheme.TextWhite, AutoSize = true };
            txtEmail = new TextBox { Location = new Point(25, 50), Size = new Size(300, 26) };
            UiTheme.StyleTextBox(txtEmail);

            btnSend = new Button { Text = "SIFIRLAMA BAĞLANTISI AL", Location = new Point(25, 100), Size = new Size(300, 40) };
            UiTheme.StyleButton(btnSend, UiTheme.AccentBlue, Color.White);
            btnSend.Click += BtnSend_Click;

            card.Controls.Add(lblEmail);
            card.Controls.Add(txtEmail);
            card.Controls.Add(btnSend);

            this.Controls.Add(card);
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string token = _authService.RequestPasswordReset(txtEmail.Text);

                if (!string.IsNullOrEmpty(token))
                {
                    Clipboard.SetText(token);
                    MessageBox.Show($"15 dakika geçerli tek kullanımlık güvenlik tokenınız üretildi ve panoya kopyalandı:\n\n{token}\n\nŞimdi sıfırlama ekranına yönlendiriliyorsunuz.",
                        "Token Üretildi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    var resetForm = new ResetPasswordForm(token);
                    this.Hide();
                    resetForm.ShowDialog();
                    this.Close();
                }
                else
                {
                   
                    MessageBox.Show("Eğer girilen e-posta adresi sistemde kayıtlı ve aktifse sıfırlama bağlantısı oluşturulmuştur.",
                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}