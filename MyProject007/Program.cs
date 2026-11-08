using System;
using System.Windows.Forms;
using MyProject007.BLL;
using MyProject007.Helpers;

namespace MyProject007.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            
            Application.AddMessageFilter(new GlobalUserActivityFilter());

            
            try
            {
                var authService = new AuthService();
                authService.SeedInitialUsersIfNecessary();
            }
            catch { }

            Application.Run(new LoginForm());
        }
    }

    public class GlobalUserActivityFilter : IMessageFilter
    {
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_KEYDOWN = 0x0100;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEMOVE || m.Msg == WM_LBUTTONDOWN ||
                m.Msg == WM_RBUTTONDOWN || m.Msg == WM_KEYDOWN)
            {
                if (Session.OturumAcikMi)
                {
                    Session.AktiviteGuncelle();
                }
            }
            return false;
        }
    }
}