using System;
using System.Drawing;
using System.Windows.Forms;
using MyProject007.BLL;

namespace MyProject007
{
    public partial class CreateUserForm : Form
    {
        private readonly UserService _userService = new UserService();

        private const string HINT_EMAIL = "Örn: yeni.musteri@banka.com";
        private const string HINT_NAME = "Adını giriniz";
        private const string HINT_SURNAME = "Soyadını giriniz";
        private const string HINT_BALANCE = "Başlangıç bakiyesi (TL)";

        private TextBox txtEmail;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private DateTimePicker dtpBirthDate;
        private ComboBox cmbRole;
        private TextBox txtBalance;
        private Button btnCreate;

        public CreateUserForm()
        {
            InitializeDesign();
        }

        private void InitializeDesign()
        {
            this.Text = "ARK BANK - Yeni Müşteri / Yetkili Tanımlama";
            this.Size = new Size(440, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = UiTheme.BgDark;

            Panel card = new Panel { Size = new Size(380, 450), Location = new Point(22, 15), BackColor = UiTheme.CardBg };

            Label lblEmail = new Label { Text = "E-Posta Adresi", Location = new Point(25, 15), ForeColor = UiTheme.PureWhite, AutoSize = true };
            txtEmail = new TextBox { Location = new Point(25, 38), Size = new Size(325, 28) };
            UiTheme.StyleTextBox(txtEmail);
            UiTheme.SetPlaceholder(txtEmail, HINT_EMAIL);

            Label lblName = new Label { Text = "Ad", Location = new Point(25, 75), ForeColor = UiTheme.PureWhite, AutoSize = true };
            txtFirstName = new TextBox { Location = new Point(25, 98), Size = new Size(325, 28) };
            UiTheme.StyleTextBox(txtFirstName);
            UiTheme.SetPlaceholder(txtFirstName, HINT_NAME);

            Label lblSurname = new Label { Text = "Soyad", Location = new Point(25, 135), ForeColor = UiTheme.PureWhite, AutoSize = true };
            txtLastName = new TextBox { Location = new Point(25, 158), Size = new Size(325, 28) };
            UiTheme.StyleTextBox(txtLastName);
            UiTheme.SetPlaceholder(txtLastName, HINT_SURNAME);

            Label lblBirth = new Label { Text = "Doğum Tarihi", Location = new Point(25, 195), ForeColor = UiTheme.PureWhite, AutoSize = true };
            dtpBirthDate = new DateTimePicker { Location = new Point(25, 218), Size = new Size(325, 28), Format = DateTimePickerFormat.Short };

            Label lblRole = new Label { Text = "Rol", Location = new Point(25, 255), ForeColor = UiTheme.PureWhite, AutoSize = true };
            cmbRole = new ComboBox { Location = new Point(25, 278), Size = new Size(150, 28), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new object[] { "User", "Admin" });
            cmbRole.SelectedIndex = 0;

            Label lblBalance = new Label { Text = "Bakiye (TL)", Location = new Point(190, 255), ForeColor = UiTheme.PureWhite, AutoSize = true };
            txtBalance = new TextBox { Location = new Point(190, 278), Size = new Size(160, 28) };
            UiTheme.StyleTextBox(txtBalance);
            UiTheme.SetPlaceholder(txtBalance, HINT_BALANCE);

            btnCreate = new Button { Text = "GÜVENLİ KULLANICIYI OLUŞTUR", Location = new Point(25, 345), Size = new Size(325, 42) };
            UiTheme.StyleButton(btnCreate, UiTheme.VibrantGold, Color.FromArgb(10, 18, 36));
            btnCreate.Click += BtnCreate_Click;

            card.Controls.Add(lblEmail); card.Controls.Add(txtEmail);
            card.Controls.Add(lblName); card.Controls.Add(txtFirstName);
            card.Controls.Add(lblSurname); card.Controls.Add(txtLastName);
            card.Controls.Add(lblBirth); card.Controls.Add(dtpBirthDate);
            card.Controls.Add(lblRole); card.Controls.Add(cmbRole);
            card.Controls.Add(lblBalance); card.Controls.Add(txtBalance);
            card.Controls.Add(btnCreate);

            this.Controls.Add(card);
            this.ActiveControl = lblEmail;
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            string email = (txtEmail.Text == HINT_EMAIL) ? "" : txtEmail.Text.Trim();
            string name = (txtFirstName.Text == HINT_NAME) ? "" : txtFirstName.Text.Trim();
            string surname = (txtLastName.Text == HINT_SURNAME) ? "" : txtLastName.Text.Trim();
            string balanceStr = (txtBalance.Text == HINT_BALANCE) ? "0" : txtBalance.Text.Trim();

            try
            {
                int roleId = cmbRole.SelectedItem.ToString() == "Admin" ? 1 : 2;
                decimal balance = decimal.TryParse(balanceStr, out decimal b) ? b : 0;

                var passwords = _userService.CreateUserWithSecurity(email, name, surname, dtpBirthDate.Value, roleId, balance);

                string bilgi = $"Kullanıcı Başarıyla Oluşturuldu!\n\n" +
                               $"1. Standart Geçici Parola: {passwords.TempPassword}\n" +
                               $"2. Acil Durum (Panik) Parolası: {passwords.PanicPassword}\n\n" +
                               $"(Parolalar panoya kopyalandı. Kullanıcı ilk girişte standart parolasını değiştirmek zorundadır.)";

                Clipboard.SetText($"Kullanıcı: {email} | Şifre: {passwords.TempPassword} | Panik Şifresi: {passwords.PanicPassword}");
                MessageBox.Show(bilgi, "Güvenlik Kimliği Tanımlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Kayıt Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}