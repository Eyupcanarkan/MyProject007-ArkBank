using System;
using System.Drawing;
using System.Windows.Forms;
using MyProject007.BLL;
using MyProject007.Helpers;

namespace MyProject007
{
    public partial class MainForm : Form
    {
        private readonly UserService _userService = new UserService();

        private Timer timerSession;
        private bool sessionUyarisiGosterildi = false;

        private const string HINT_TARGET_ACC = "Örn: TR1000000002";
        private const string HINT_AMOUNT = "Gönderilecek tutar (TL)";
        private const string HINT_DESC = "İşlem açıklaması (örn: Kira, Borç)";

        private Panel pnlHeader;
        private Label lblBankLogo;
        private Label lblUserTitle;
        private Label lblRole;
        private Label lblKalanSure;
        private Button btnUserManagement;
        private Button btnLogout;

        private Panel pnlBalanceCard;
        private Label lblCardHead;
        private Label lblAccNo;
        private Label lblBalanceValue;
        private Button btnChangePassword;
        private Button btnChangeDuressPassword;

        private Panel pnlTransfer;
        private TextBox txtTargetAcc;
        private TextBox txtTransferAmount;
        private TextBox txtTransferDesc;
        private Button btnSendMoney;

    
        private Panel pnlHistory;
        private DataGridView dgvHistory;

        private Panel pnlAdminDashboard;

        public MainForm()
        {
            InitializeDesign();
            SetupSessionTimer();
        }

        private void InitializeDesign()
        {
            this.Text = "ARK BANK - Kurumsal Bankacılık Portalı";
            this.Size = new Size(1150, 730);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UiTheme.BgDark;

         
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 95, BackColor = UiTheme.CardBg };

            lblBankLogo = new Label
            {
                Text = "ARK BANK",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = UiTheme.VibrantGold,
                Location = new Point(25, 18),
                AutoSize = true
            };

            lblUserTitle = new Label
            {
                Text = "Hoş Geldiniz, ...",
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
                ForeColor = UiTheme.PureWhite,
                Location = new Point(28, 54),
                AutoSize = true
            };

            lblRole = new Label
            {
                Text = "Rol: ...",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = UiTheme.LightIceBlue,
                Location = new Point(320, 56),
                AutoSize = true
            };

            lblKalanSure = new Label
            {
                Text = "Kalan Süre: 10:00",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = UiTheme.SoftGold,
                Location = new Point(560, 32),
                AutoSize = true
            };

            btnUserManagement = new Button { Text = "KULLANICI YÖNETİMİ", Size = new Size(185, 42), Location = new Point(775, 26) };
            UiTheme.StyleButton(btnUserManagement, UiTheme.CardLightBg, UiTheme.PureWhite);
            btnUserManagement.Click += (s, e) =>
            {
                this.Hide();
                using (var form = new UserManagementForm())
                {
                    form.ShowDialog();
                }
                this.Show();
                BakiyeVeGecmisYukle();
            };

            btnLogout = new Button { Text = "ÇIKIŞ", Size = new Size(100, 42), Location = new Point(980, 26) };
            UiTheme.StyleButton(btnLogout, UiTheme.CrimsonRed, Color.White);
            btnLogout.Click += BtnLogout_Click;

            pnlHeader.Controls.Add(lblBankLogo);
            pnlHeader.Controls.Add(lblUserTitle);
            pnlHeader.Controls.Add(lblRole);
            pnlHeader.Controls.Add(lblKalanSure);
            pnlHeader.Controls.Add(btnUserManagement);
            pnlHeader.Controls.Add(btnLogout);

      
            pnlBalanceCard = new Panel { Size = new Size(520, 205), Location = new Point(30, 115), BackColor = UiTheme.CardBg };
            lblCardHead = new Label { Text = "VADESİZ MEVDUAT HESABI", Location = new Point(22, 18), ForeColor = UiTheme.SoftGold, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), AutoSize = true };
            lblAccNo = new Label { Text = "Hesap No: ...", Location = new Point(22, 48), ForeColor = UiTheme.PureWhite, Font = new Font("Segoe UI", 10.5F), AutoSize = true };
            lblBalanceValue = new Label { Text = "0.00 TL", Location = new Point(20, 85), ForeColor = UiTheme.VibrantGold, Font = new Font("Segoe UI", 26F, FontStyle.Bold), AutoSize = true };

            btnChangePassword = new Button { Text = "Parola Değiştir", Location = new Point(220, 150), Size = new Size(130, 34) };
            UiTheme.StyleButton(btnChangePassword, UiTheme.CardLightBg, UiTheme.PureWhite);
            btnChangePassword.Click += (s, e) =>
            {
                this.Hide();
                using (var form = new ChangePasswordForm(Session.UserID))
                {
                    form.ShowDialog();
                }
                this.Show();
            };

            btnChangeDuressPassword = new Button { Text = "Panik Parolası Belirle", Location = new Point(360, 150), Size = new Size(145, 34) };
            UiTheme.StyleButton(btnChangeDuressPassword, UiTheme.VibrantGold, Color.FromArgb(10, 18, 36));
            btnChangeDuressPassword.Click += (s, e) =>
            {
                using (var form = new ChangeDuressPasswordForm())
                {
                    form.ShowDialog();
                }
            };

            pnlBalanceCard.Controls.Add(lblCardHead);
            pnlBalanceCard.Controls.Add(lblAccNo);
            pnlBalanceCard.Controls.Add(lblBalanceValue);
            pnlBalanceCard.Controls.Add(btnChangePassword);
            pnlBalanceCard.Controls.Add(btnChangeDuressPassword);

      
            pnlTransfer = new Panel { Size = new Size(530, 205), Location = new Point(570, 115), BackColor = UiTheme.CardBg };
            Label lblTransHead = new Label { Text = "HIZLI HAVALE / EFT", Location = new Point(20, 15), ForeColor = UiTheme.VibrantGold, Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoSize = true };

            Label lblTarget = new Label { Text = "Alıcı Hesap Numarası", Location = new Point(20, 42), ForeColor = UiTheme.LightIceBlue, Font = new Font("Segoe UI", 9F), AutoSize = true };
            txtTargetAcc = new TextBox { Location = new Point(20, 64), Size = new Size(230, 28) };
            UiTheme.StyleTextBox(txtTargetAcc);
            UiTheme.SetPlaceholder(txtTargetAcc, HINT_TARGET_ACC);

            Label lblAmount = new Label { Text = "Tutar (TL)", Location = new Point(270, 42), ForeColor = UiTheme.LightIceBlue, Font = new Font("Segoe UI", 9F), AutoSize = true };
            txtTransferAmount = new TextBox { Location = new Point(270, 64), Size = new Size(230, 28) };
            UiTheme.StyleTextBox(txtTransferAmount);
            UiTheme.SetPlaceholder(txtTransferAmount, HINT_AMOUNT);

            Label lblDesc = new Label { Text = "Açıklama", Location = new Point(20, 98), ForeColor = UiTheme.LightIceBlue, Font = new Font("Segoe UI", 9F), AutoSize = true };
            txtTransferDesc = new TextBox { Location = new Point(20, 120), Size = new Size(480, 28) };
            UiTheme.StyleTextBox(txtTransferDesc);
            UiTheme.SetPlaceholder(txtTransferDesc, HINT_DESC);

            btnSendMoney = new Button { Text = "ONAYLA VE GÖNDER", Location = new Point(20, 158), Size = new Size(480, 36) };
            UiTheme.StyleButton(btnSendMoney, UiTheme.CrimsonRed, Color.White);
            btnSendMoney.Click += BtnSendMoney_Click;

            pnlTransfer.Controls.Add(lblTransHead);
            pnlTransfer.Controls.Add(lblTarget);
            pnlTransfer.Controls.Add(txtTargetAcc);
            pnlTransfer.Controls.Add(lblAmount);
            pnlTransfer.Controls.Add(txtTransferAmount);
            pnlTransfer.Controls.Add(lblDesc);
            pnlTransfer.Controls.Add(txtTransferDesc);
            pnlTransfer.Controls.Add(btnSendMoney);

            
            pnlHistory = new Panel { Size = new Size(1070, 340), Location = new Point(30, 335), BackColor = UiTheme.CardBg };
            Label lblHistoryTitle = new Label { Text = "HESAP HAREKETLERİ GEÇMİŞİ", Location = new Point(20, 15), ForeColor = UiTheme.PureWhite, Font = new Font("Segoe UI", 11F, FontStyle.Bold), AutoSize = true };

            dgvHistory = new DataGridView
            {
                Location = new Point(20, 50),
                Size = new Size(1030, 270),
                BackgroundColor = UiTheme.BgDark,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };
            dgvHistory.DefaultCellStyle.BackColor = UiTheme.CardBg;
            dgvHistory.DefaultCellStyle.ForeColor = UiTheme.PureWhite;
            dgvHistory.DefaultCellStyle.SelectionBackColor = UiTheme.VibrantGold;
            dgvHistory.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 18, 36);
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 48, 88);
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.SoftGold;
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvHistory.EnableHeadersVisualStyles = false;

            pnlHistory.Controls.Add(lblHistoryTitle);
            pnlHistory.Controls.Add(dgvHistory);

            
            pnlAdminDashboard = new Panel { Size = new Size(1070, 560), Location = new Point(30, 115), BackColor = UiTheme.CardBg, Visible = false };
            Label lblAdminHead = new Label { Text = "ARK BANK - SİSTEM VE GÜVENLİK YÖNETİM MERKEZİ", ForeColor = UiTheme.VibrantGold, Font = new Font("Segoe UI", 13F, FontStyle.Bold), Location = new Point(25, 25), AutoSize = true };
            Label lblAdminInfo = new Label
            {
                Text = "Yönetici Hesabı Aktif.\n\n" +
                       "• Bireysel bankacılık ve transfer işlemleri sistem denetimi kuralları gereğince bu hesapta devre dışıdır.\n" +
                       "• Müşterileri listelemek, yeni kullanıcı açmak ve durum değiştirmek için 'KULLANICI YÖNETİMİ' butonunu kullanınız.\n" +
                       "• Güvenlik alarm ve işlem denetim kayıtları SQL AuditLogs tablosunda anlık takip edilmektedir.",
                ForeColor = UiTheme.LightIceBlue,
                Font = new Font("Segoe UI", 11F),
                Location = new Point(25, 75),
                Size = new Size(1000, 200)
            };

            Button btnGoUsers = new Button { Text = "MÜŞTERİ YÖNETİMİNE GİT", Location = new Point(25, 230), Size = new Size(250, 45) };
            UiTheme.StyleButton(btnGoUsers, UiTheme.VibrantGold, Color.FromArgb(10, 18, 36));
            btnGoUsers.Click += (s, e) => btnUserManagement.PerformClick();

            pnlAdminDashboard.Controls.Add(lblAdminHead);
            pnlAdminDashboard.Controls.Add(lblAdminInfo);
            pnlAdminDashboard.Controls.Add(btnGoUsers);

            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlBalanceCard);
            this.Controls.Add(pnlTransfer);
            this.Controls.Add(pnlHistory);
            this.Controls.Add(pnlAdminDashboard);

            this.Load += MainForm_Load;
            this.FormClosing += (s, e) => Application.Exit();
        }

        private void SetupSessionTimer()
        {
            timerSession = new Timer { Interval = 1000 };
            timerSession.Tick += TimerSession_Tick;
            timerSession.Start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblUserTitle.Text = $"Hoş Geldiniz, {Session.AdSoyad}";
            lblRole.Text = $"Rol: {Session.Rol} | Giriş: {Session.GirisZamani:dd.MM.yyyy HH:mm}";
            lblAccNo.Text = $"Hesap No: {Session.AccountNumber}";

            bool isAdmin = (Session.Rol == "Admin");
            btnUserManagement.Visible = isAdmin;

           
            if (isAdmin)
            {
                pnlBalanceCard.Visible = false;
                pnlTransfer.Visible = false;
                pnlHistory.Visible = false;
                pnlAdminDashboard.Visible = true;
            }
            else
            {
                pnlBalanceCard.Visible = true;
                pnlTransfer.Visible = true;
                pnlHistory.Visible = true;
                pnlAdminDashboard.Visible = false;
                BakiyeVeGecmisYukle();
            }

            this.ActiveControl = lblBankLogo;
        }

        private void BakiyeVeGecmisYukle()
        {
            lblBalanceValue.Text = $"{Session.Balance:C2}";

            if (Session.IsDuressSession)
            {
                lblCardHead.Text = "VADESİZ MEVDUAT HESABI [KORUMALI]";
            }
            else if (Session.VaultBalance > 0 && Session.VaultUnlockDate.HasValue)
            {
                lblCardHead.Text = $"VADESİZ MEVDUAT (Kilitli Kasa: {Session.VaultBalance:C2} - Kilit: {Session.VaultUnlockDate.Value:dd.MM HH:mm})";
            }
            else
            {
                lblCardHead.Text = "VADESİZ MEVDUAT HESABI";
            }

            try
            {
                dgvHistory.DataSource = _userService.GetUserTransactions(Session.UserID, Session.AccountNumber);
            }
            catch { }
        }

        private void TimerSession_Tick(object sender, EventArgs e)
        {
            TimeSpan kalan = Session.KalanSure();

            if (kalan.TotalSeconds > 0)
            {
                lblKalanSure.Text = $"Kalan Süre: {kalan:mm\\:ss}";
            }

            if (kalan.TotalSeconds <= 60 && kalan.TotalSeconds > 0 && !sessionUyarisiGosterildi)
            {
                sessionUyarisiGosterildi = true;
                MessageBox.Show("Oturumunuz 1 dakika içerisinde sona erecektir.",
                    "Oturum Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (Session.SessionSuresiDolduMu())
            {
                timerSession.Stop();
                MessageBox.Show("Uzun süre işlem yapılmadığı için oturumunuz sonlandırılmıştır.",
                    "Oturum Zaman Aşımı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Session.SessionTemizle();
                LoginForm login = new LoginForm();
                login.Show();
                this.Dispose();
            }
        }

        private void BtnSendMoney_Click(object sender, EventArgs e)
        {
            string targetAcc = (txtTargetAcc.Text == HINT_TARGET_ACC) ? "" : txtTargetAcc.Text.Trim();
            string amountStr = (txtTransferAmount.Text == HINT_AMOUNT) ? "" : txtTransferAmount.Text.Trim();
            string desc = (txtTransferDesc.Text == HINT_DESC) ? "" : txtTransferDesc.Text.Trim();

            if (string.IsNullOrEmpty(targetAcc) || !decimal.TryParse(amountStr, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir alıcı hesap numarası ve tutar giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

         
            if (_userService.IsHighRiskTransfer(Session.UserID, amount))
            {
                var otpForm = new OtpVerificationForm(amount);
                otpForm.ShowDialog();
                if (!otpForm.IsVerified)
                {
                    MessageBox.Show("Yüksek riskli transfer güvenlik onayı verilmediği için iptal edildi.", "İşlem Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                var dialog = MessageBox.Show(
                    $"{targetAcc} numaralı hesaba {amount:C2} tutarında para göndermek istediğinize emin misiniz?",
                    "Transfer Onayı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dialog != DialogResult.Yes) return;
            }

            try
            {
                _userService.TransferMoney(Session.UserID, targetAcc, amount, desc);
                MessageBox.Show("Para transferi başarıyla gerçekleşti.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UiTheme.SetPlaceholder(txtTargetAcc, HINT_TARGET_ACC);
                UiTheme.SetPlaceholder(txtTransferAmount, HINT_AMOUNT);
                UiTheme.SetPlaceholder(txtTransferDesc, HINT_DESC);
                this.ActiveControl = lblBankLogo;

                BakiyeVeGecmisYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Güvenlik Engeli", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Oturumu kapatmak istediğinize emin misiniz?", "Oturumu Kapat", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                timerSession.Stop();
                Session.SessionTemizle();
                LoginForm login = new LoginForm();
                login.Show();
                this.Dispose();
            }
        }
    }
}