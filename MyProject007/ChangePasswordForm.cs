using System;
using System.Drawing;
using System.Windows.Forms;
using MyProject007.BLL;
using MyProject007.Helpers;

namespace MyProject007
{
    public partial class ChangePasswordForm : Form
    {
        private readonly AuthService _authService = new AuthService();
        private readonly int _userId;
        private readonly bool _isForced;

        private const string HINT_OLD = "Mevcut parolanızı giriniz";
        private const string HINT_NEW = "Yeni parolanızı giriniz";
        private const string HINT_CONFIRM = "Yeni parolanızı tekrar giriniz";

        private TextBox txtOldPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkShowPassword; 
        private Button btnSave;
        private Button btnCancel;

        public ChangePasswordForm(int userId, bool isForced = false)
        {
            _userId = userId;
            _isForced = isForced;
            InitializeDesign();
        }

        private void InitializeDesign()
        {
            this.Text = _isForced ? "ARK BANK - Zorunlu Parola Güncelleme" : "ARK BANK - Parola Değiştir";
            this.Size = new Size(460, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = UiTheme.BgDark;

            Panel card = new Panel { Size = new Size(400, 410), Location = new Point(22, 15), BackColor = UiTheme.CardBg };

            Label lblTitle = new Label
            {
                Text = _isForced ? "İLK GİRİŞ PAROLA BELİRLEME" : "PAROLA GÜNCELLEME",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = UiTheme.VibrantGold,
                Location = new Point(25, 20),
                AutoSize = true
            };

            
            Label lblOld = new Label { Text = "Mevcut Parola", Location = new Point(25, 65), ForeColor = UiTheme.PureWhite, AutoSize = true };
            txtOldPassword = new TextBox { Location = new Point(25, 88), Size = new Size(350, 28) };
            UiTheme.StyleTextBox(txtOldPassword);
            UiTheme.SetPlaceholder(txtOldPassword, HINT_OLD, isPassword: true);

            
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
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;

        
            btnSave = new Button { Text = "PAROLAYI GÜNCELLE", Location = new Point(25, 290), Size = new Size(350, 40) };
            UiTheme.StyleButton(btnSave, UiTheme.VibrantGold, Color.FromArgb(10, 18, 36));
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button { Text = "İPTAL", Location = new Point(25, 340), Size = new Size(350, 34) };
            UiTheme.StyleButton(btnCancel, UiTheme.CardLightBg, UiTheme.PureWhite);
            btnCancel.Visible = !_isForced;
            btnCancel.Click += (s, e) => this.Close();

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblOld);
            card.Controls.Add(txtOldPassword);
            card.Controls.Add(lblNew);
            card.Controls.Add(txtNewPassword);
            card.Controls.Add(lblConfirm);
            card.Controls.Add(txtConfirmPassword);
            card.Controls.Add(chkShowPassword);
            card.Controls.Add(btnSave);
            card.Controls.Add(btnCancel);

            this.Controls.Add(card);
            this.ActiveControl = lblTitle;
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool show = chkShowPassword.Checked;

            
            if (txtOldPassword.Text != HINT_OLD) txtOldPassword.UseSystemPasswordChar = !show;
            if (txtNewPassword.Text != HINT_NEW) txtNewPassword.UseSystemPasswordChar = !show;
            if (txtConfirmPassword.Text != HINT_CONFIRM) txtConfirmPassword.UseSystemPasswordChar = !show;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string oldPass = (txtOldPassword.Text == HINT_OLD) ? "" : txtOldPassword.Text;
            string newPass = (txtNewPassword.Text == HINT_NEW) ? "" : txtNewPassword.Text;
            string confirmPass = (txtConfirmPassword.Text == HINT_CONFIRM) ? "" : txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(oldPass) || string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirmPass))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Yeni parolalar birbiriyle eşleşmiyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _authService.ChangePassword(_userId, oldPass, newPass);
                MessageBox.Show("Parolanız başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Parola Değiştirilemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}