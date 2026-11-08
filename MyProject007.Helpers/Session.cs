using System;

namespace MyProject007.Helpers
{
    public static class Session
    {
        public static int UserID { get; set; }
        public static string Email { get; set; }
        public static string AdSoyad { get; set; }
        public static string Rol { get; set; }
        public static string AccountNumber { get; set; }
        public static decimal Balance { get; set; }

        // YENİ ALANLAR:
        public static decimal VaultBalance { get; set; }
        public static DateTime? VaultUnlockDate { get; set; }
        public static bool IsDuressSession { get; set; } 

        public static DateTime GirisZamani { get; set; }
        public static DateTime SonIslemZamani { get; set; }
        public static int TimeoutDakika { get; set; } = 10;

        public static bool OturumAcikMi => UserID > 0;

        public static void SessionBaslat(
            int userId, string email, string adSoyad, string rol,
            string accNumber, decimal balance, decimal vaultBalance,
            DateTime? vaultUnlockDate, bool isDuress)
        {
            UserID = userId;
            Email = email;
            AdSoyad = adSoyad;
            Rol = rol;
            AccountNumber = accNumber;
            GirisZamani = DateTime.Now;
            SonIslemZamani = DateTime.Now;

            IsDuressSession = isDuress;

            if (isDuress)
            {
                Balance = 250.00m;
                VaultBalance = 0.00m;
                VaultUnlockDate = null;
            }
            else
            {
                Balance = balance;
                VaultBalance = vaultBalance;
                VaultUnlockDate = vaultUnlockDate;
            }
        }

        public static void AktiviteGuncelle()
        {
            SonIslemZamani = DateTime.Now;
        }

        public static bool SessionSuresiDolduMu()
        {
            TimeSpan fark = DateTime.Now - SonIslemZamani;
            return fark.TotalMinutes >= TimeoutDakika;
        }

        public static TimeSpan KalanSure()
        {
            TimeSpan gecen = DateTime.Now - SonIslemZamani;
            TimeSpan timeout = TimeSpan.FromMinutes(TimeoutDakika);
            TimeSpan kalan = timeout - gecen;
            return kalan.TotalSeconds > 0 ? kalan : TimeSpan.Zero;
        }

        public static void SessionTemizle()
        {
            UserID = 0;
            Email = null;
            AdSoyad = null;
            Rol = null;
            AccountNumber = null;
            Balance = 0;
            VaultBalance = 0;
            VaultUnlockDate = null;
            IsDuressSession = false;
        }
    }
}