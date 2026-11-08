using System;
using System.Drawing;
using System.Windows.Forms;
using MyProject007.BLL;
using MyProject007.Helpers;

namespace MyProject007
{
    public partial class UserManagementForm : Form
    {
        private readonly UserService _userService = new UserService();

        private DataGridView dgvUsers;
        private Button btnToggleStatus;
        private Button btnNewUser;
        private Button btnBack;
        private Panel pnlTop;
        private Label lblHeader;

        public UserManagementForm()
        {
            InitializeDesign();
        }

        private void InitializeDesign()
        {
            this.Text = "ARK BANK - Yönetim ve Müşteri Kontrol Paneli";
            this.Size = new Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UiTheme.BgDark;

            pnlTop = new Panel { Dock = DockStyle.Top, Height = 75, BackColor = UiTheme.CardBg };

            lblHeader = new Label
            {
                Text = "SİSTEM KULLANICILARI VE MÜŞTERİ YÖNETİMİ",
                ForeColor = UiTheme.VibrantGold,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(20, 25),
                AutoSize = true
            };

            btnNewUser = new Button { Text = "+ YENİ KULLANICI EKLE", Size = new Size(190, 38), Location = new Point(480, 18) };
            UiTheme.StyleButton(btnNewUser, UiTheme.CardLightBg, UiTheme.PureWhite);
            btnNewUser.Click += BtnNewUser_Click;

            btnToggleStatus = new Button { Text = "DURUM DEĞİŞTİR (AKTİF/PASİF)", Size = new Size(230, 38), Location = new Point(680, 18) };
            UiTheme.StyleButton(btnToggleStatus, UiTheme.VibrantGold, Color.FromArgb(10, 18, 36));
            btnToggleStatus.Click += BtnToggleStatus_Click;

            btnBack = new Button { Text = "← PANELE DÖN", Size = new Size(130, 38), Location = new Point(925, 18) };
            UiTheme.StyleButton(btnBack, UiTheme.CrimsonRed, Color.White);
            btnBack.Click += (s, e) => this.Close();

            pnlTop.Controls.Add(lblHeader);
            pnlTop.Controls.Add(btnNewUser);
            pnlTop.Controls.Add(btnToggleStatus);
            pnlTop.Controls.Add(btnBack);

            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = UiTheme.BgDark,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };

            dgvUsers.DefaultCellStyle.BackColor = UiTheme.CardBg;
            dgvUsers.DefaultCellStyle.ForeColor = UiTheme.PureWhite;
            dgvUsers.DefaultCellStyle.SelectionBackColor = UiTheme.VibrantGold;
            dgvUsers.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 18, 36);
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 48, 88);
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.SoftGold;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvUsers.EnableHeadersVisualStyles = false;

            this.Controls.Add(dgvUsers);
            this.Controls.Add(pnlTop);

            this.Load += (s, e) => KullanicilariYukle();
        }

        private void KullanicilariYukle()
        {
            try
            {
                dgvUsers.DataSource = null;
                var list = _userService.GetAllUsers();
                dgvUsers.DataSource = list;

             
                string[] hiddenCols = { "PasswordHash", "PasswordSalt", "PasswordIterations",
                                        "DuressPasswordHash", "DuressPasswordSalt", "RoleId",
                                        "VaultBalance", "VaultUnlockDate", "IsDuressLogin", "CreatedBy" };

                foreach (var col in hiddenCols)
                {
                    if (dgvUsers.Columns[col] != null) dgvUsers.Columns[col].Visible = false;
                }

               
                if (dgvUsers.Columns["UserId"] != null) dgvUsers.Columns["UserId"].HeaderText = "Kullanıcı No";
                if (dgvUsers.Columns["Email"] != null) dgvUsers.Columns["Email"].HeaderText = "E-Posta Adresi";
                if (dgvUsers.Columns["FirstName"] != null) dgvUsers.Columns["FirstName"].HeaderText = "Ad";
                if (dgvUsers.Columns["LastName"] != null) dgvUsers.Columns["LastName"].HeaderText = "Soyad";
                if (dgvUsers.Columns["BirthDate"] != null) dgvUsers.Columns["BirthDate"].HeaderText = "Doğum Tarihi";
                if (dgvUsers.Columns["RoleName"] != null) dgvUsers.Columns["RoleName"].HeaderText = "Yetki Rolü";
                if (dgvUsers.Columns["Balance"] != null)
                {
                    dgvUsers.Columns["Balance"].HeaderText = "Hesap Bakiyesi";
                    dgvUsers.Columns["Balance"].DefaultCellStyle.Format = "C2";
                }
                if (dgvUsers.Columns["AccountNumber"] != null) dgvUsers.Columns["AccountNumber"].HeaderText = "Hesap No";
                if (dgvUsers.Columns["IsActive"] != null) dgvUsers.Columns["IsActive"].HeaderText = "Hesap Durumu (Aktif)";
                if (dgvUsers.Columns["MustChangePassword"] != null) dgvUsers.Columns["MustChangePassword"].HeaderText = "İlk Giriş Kuralı";
                if (dgvUsers.Columns["CreatedAt"] != null) dgvUsers.Columns["CreatedAt"].HeaderText = "Kayıt Tarihi";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnToggleStatus_Click(object sender, EventArgs e)
        {
            Session.AktiviteGuncelle();

            if (dgvUsers.CurrentRow == null)
            {
                MessageBox.Show("Lütfen işlem yapılacak bir kullanıcı seçin.", "Seçim Yapılmadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.CurrentRow.Cells["UserId"].Value);
            bool currentStatus = Convert.ToBoolean(dgvUsers.CurrentRow.Cells["IsActive"].Value);

            if (userId == Session.UserID)
            {
                MessageBox.Show("Kendi oturum açtığınız yönetici hesabını pasife alamazsınız.", "İşlem Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _userService.SetUserActiveStatus(userId, !currentStatus);
                MessageBox.Show("Kullanıcı aktiflik durumu güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                KullanicilariYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNewUser_Click(object sender, EventArgs e)
        {
            Session.AktiviteGuncelle();
            using (var form = new CreateUserForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    KullanicilariYukle();
                }
            }
        }
    }
}