using System;
using System.Drawing;
using System.Windows.Forms;
using MyProject007.BLL;
using MyProject007.Helpers;

namespace MyProject007
{
    public partial class ChangeDuressPasswordForm : Form
    {
        private readonly UserService _userService = new UserService();
        private const string HINT_PASS = "Yeni panik parolasını giriniz";

        private TextBox txtNewPanic;
        private TextBox txtConfirm;
        private Button btnSave;
        private Button btnCancel;

        public ChangeDuressPasswordForm()
        {
            InitializeDesign();
        }

        private void InitializeDesign()
        {
            this.Text = "ARK BANK - Acil Durum (Panik) Parolası Güncelleme";
            this.Size = new Size(450, 340);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = UiTheme.BgDark;

            Panel card = new Panel { Size = new Size(390, 260), Location = new Point(22, 18), BackColor = UiTheme.CardBg };

            Label lblTitle = new Label
            {
                Text = "ACİL DURUM (DURESS) PAROLASI",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = UiTheme.CrimsonRed,
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label lblDesc = new Label
            {
                Text = "Zorla veya tehdit altında oturum açmanız istendiğinde bu parolayı kullanın. Sistem sahte bakiye gösterecek ve transferleri kilitleyecektir.",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = UiTheme.LightIceBlue,
                Location = new Point(22, 45),
                Size = new Size(350, 42)
            };

            Label lblNew = new Label { Text = "Yeni Panik Parolası", Location = new Point(22, 95), ForeColor = UiTheme.PureWhite, AutoSize = true };
            txtNewPanic = new TextBox { Location = new Point(22, 118), Size = new Size(345, 26) };
            UiTheme.StyleTextBox(txtNewPanic);
            UiTheme.SetPlaceholder(txtNewPanic, HINT_PASS, isPassword: true);

            Label lblConf = new Label { Text = "Yeni Panik Parolası (Tekrar)", Location = new Point(22, 150), ForeColor = UiTheme.PureWhite, AutoSize = true };
            txtConfirm = new TextBox { Location = new Point(22, 172), Size = new Size(345, 26) };
            UiTheme.StyleTextBox(txtConfirm);
            UiTheme.SetPlaceholder(txtConfirm, HINT_PASS, isPassword: true);

            btnSave = new Button { Text = "GÜNCELLE", Location = new Point(22, 212), Size = new Size(165, 36) };
            UiTheme.StyleButton(btnSave, UiTheme.VibrantGold, Color.FromArgb(10, 18, 36));
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button { Text = "İPTAL", Location = new Point(202, 212), Size = new Size(165, 36) };
            UiTheme.StyleButton(btnCancel, UiTheme.CardLightBg, UiTheme.PureWhite);
            btnCancel.Click += (s, e) => this.Close();

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblDesc);
            card.Controls.Add(lblNew);
            card.Controls.Add(txtNewPanic);
            card.Controls.Add(lblConf);
            card.Controls.Add(txtConfirm);
            card.Controls.Add(btnSave);
            card.Controls.Add(btnCancel);

            this.Controls.Add(card);
            this.ActiveControl = lblTitle;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string p1 = (txtNewPanic.Text == HINT_PASS) ? "" : txtNewPanic.Text;
            string p2 = (txtConfirm.Text == HINT_PASS) ? "" : txtConfirm.Text;

            if (string.IsNullOrWhiteSpace(p1) || p1.Length < 6)
            {
                MessageBox.Show("Panik parolası en az 6 karakter olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (p1 != p2)
            {
                MessageBox.Show("Parolalar birbiriyle uyuşmuyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _userService.UpdateUserDuressPassword(Session.UserID, p1);
                MessageBox.Show("Acil durum panik parolanız başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}