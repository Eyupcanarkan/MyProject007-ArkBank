using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyProject007
{
    public partial class OtpVerificationForm : Form
    {
        public bool IsVerified { get; private set; } = false;
        private readonly string _generatedCode;

        private const string HINT_OTP = "6 Haneli Kodu Girin";
        private TextBox txtCode;
        private Button btnVerify;

        public OtpVerificationForm(decimal amount)
        {
            _generatedCode = new Random().Next(100000, 999999).ToString();
            InitializeDesign(amount);
        }

        private void InitializeDesign(decimal amount)
        {
            this.Text = "ARK-Shield: Yüksek Riskli Transfer Doğrulaması";
            this.Size = new Size(440, 310);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = UiTheme.BgDark;

            Panel card = new Panel { Size = new Size(380, 230), Location = new Point(22, 18), BackColor = UiTheme.CardBg };

            Label lblWarn = new Label
            {
                Text = "⚠ YÜKSEK RİSKLİ TRANSFER TESPİT EDİLDİ",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = UiTheme.CrimsonRed,
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label lblDesc = new Label
            {
                Text = $"{amount:C2} tutarındaki transfer işlemi için aşağıdaki güvenlik onay kodunu kutuya giriniz:",
                Font = new Font("Segoe UI", 9F),
                ForeColor = UiTheme.PureWhite,
                Location = new Point(20, 45),
                Size = new Size(340, 35)
            };

            Label lblCodeDisplay = new Label
            {
                Text = $"ONAY KODU: {_generatedCode}",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = UiTheme.VibrantGold,
                Location = new Point(90, 85),
                AutoSize = true
            };

            txtCode = new TextBox
            {
                Location = new Point(70, 125),
                Size = new Size(240, 30),
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            UiTheme.StyleTextBox(txtCode);
            UiTheme.SetPlaceholder(txtCode, HINT_OTP);

            btnVerify = new Button
            {
                Text = "TRANSFERİ ONAYLA",
                Location = new Point(70, 170),
                Size = new Size(240, 38)
            };
            UiTheme.StyleButton(btnVerify, UiTheme.VibrantGold, Color.FromArgb(10, 18, 36));
            btnVerify.Click += (s, e) =>
            {
                if (txtCode.Text.Trim() == _generatedCode)
                {
                    IsVerified = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Hatalı onay kodu girdiniz!", "Güvenlik Engeli", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            card.Controls.Add(lblWarn);
            card.Controls.Add(lblDesc);
            card.Controls.Add(lblCodeDisplay);
            card.Controls.Add(txtCode);
            card.Controls.Add(btnVerify);

            this.Controls.Add(card);
            this.ActiveControl = lblWarn;
        }
    }
}