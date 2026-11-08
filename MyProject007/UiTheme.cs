using System.Drawing;
using System.Windows.Forms;

namespace MyProject007
{
    public static class UiTheme
    {
        public static readonly Color BgDark = Color.FromArgb(10, 18, 36);             
        public static readonly Color CardBg = Color.FromArgb(18, 30, 58);            
        public static readonly Color CardLightBg = Color.FromArgb(30, 48, 88);       
        public static readonly Color InputBg = Color.FromArgb(14, 24, 46);           
        public static readonly Color CrimsonRed = Color.FromArgb(225, 29, 72);        
        public static readonly Color SoftRed = Color.FromArgb(251, 113, 133);       
        public static readonly Color VibrantGold = Color.FromArgb(250, 204, 21);      
        public static readonly Color SoftGold = Color.FromArgb(254, 240, 138);       
        public static readonly Color PureWhite = Color.FromArgb(255, 255, 255);      
        public static readonly Color LightIceBlue = Color.FromArgb(203, 213, 225);   
        public static readonly Color PlaceholderColor = Color.FromArgb(148, 163, 184);

        
        public static readonly Color PrimaryNavy = BgDark;
        public static readonly Color TextWhite = PureWhite;
        public static readonly Color TextMuted = LightIceBlue;
        public static readonly Color AccentGreen = VibrantGold;
        public static readonly Color AccentBlue = Color.FromArgb(59, 130, 246);
        public static readonly Color MintAccent = VibrantGold;
        public static readonly Color CyanAccent = SoftGold;
        public static readonly Color DangerRed = CrimsonRed;

        public static void StyleButton(Button btn, Color bg, Color text)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = bg;
            btn.ForeColor = text;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        public static void StyleTextBox(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = InputBg;
            txt.ForeColor = PureWhite;
            txt.Font = new Font("Segoe UI", 10.5F);
        }

    
        public static void SetPlaceholder(TextBox txt, string placeholderText, bool isPassword = false)
        {
            txt.Text = placeholderText;
            txt.ForeColor = PlaceholderColor;
            if (isPassword)
            {
                txt.UseSystemPasswordChar = false; 
            }

            txt.Enter += (s, e) =>
            {
                if (txt.Text == placeholderText)
                {
                    txt.Text = "";
                    txt.ForeColor = PureWhite;
                    if (isPassword) txt.UseSystemPasswordChar = true;
                }
            };

            txt.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholderText;
                    txt.ForeColor = PlaceholderColor;
                    if (isPassword) txt.UseSystemPasswordChar = false;
                }
            };
        }
    }
}