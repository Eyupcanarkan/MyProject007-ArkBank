 ARK BANK - Çok Katmanlı Güvenli Bankacılık Platformu



Manisa Celal Bayar Üniversitesi Salihli MYO - Bilgisayar Programcılığı  

 BLG2109 Görsel Programlama II (GP07)  

 Eyüp Can Arkan  






Proje Hakkında

ARK BANK; C# Windows Forms ve MS SQL Server kullanılarak N-Tier (Çok Katmanlı) mimari prensipleriyle geliştirilmiş, rol tabanlı yetkilendirme (RBAC), kriptografik güvenlik standartları ve akıllı tehdit algılama mekanizmalarına sahip kurumsal bir masaüstü bankacılık uygulamasıdır.



 Öne Çıkan Güvenlik ve FinTech İnovasyonları

\- \*\*PBKDF2-HMAC-SHA256:\*\* 100.000 iterasyon ve kullanıcı başına 16 bayt benzersiz kriptografik Salt ile şifrelenen parola altyapısı.

\- \*\*Panik Parolası (Duress Code):\*\* Zorlama ve tehdit altında açılan oturumlarda sahte 250 TL bakiye gösterimi, gerçek işlemlerin gizlenmesi ve arka planda `CRITICAL\_DURESS\_ALARM` kaydı.

\- \*\*Akıllı Risk Motoru ve Dinamik 2FA (OTP):\*\* 5.000 TL ve üzeri transferlerde otomatik tetiklenen 6 haneli güvenlik onay ekranı.

\- \*\*Zaman Kilitli Kasa (Vault):\*\* Belirlenen süre dolana kadar çekilemeyen bloke mevduat güvencesi.

\- \*\*IMessageFilter Oturum Yönetimi:\*\* Fare ve klavye hareketlerini dinleyen, 10 dakika eylemsizlikte otomatik kapanan ve son 1 dakika kala uyaran canlı sayaç sistemi.

 Mimari Yapı

\- \*\*UI (Presentation Layer):\*\* Windows Forms, modern kurumsal koyu tema (`UiTheme`), dinamik placeholder (hint) desteği.

\- \*\*BLL (Business Logic Layer):\*\* İş kuralları, transfer validasyonları, parola politikaları ve risk motoru.

\- \*\*DAL (Data Access Layer):\*\* Parametreli SQL komutları, SQL Transaction ve veri tabanı bağlantı fabrikası.

\- \*\*Entities:\*\* Tablo nesne modelleri (`User`, `Role`, `AccountTransaction`, `AuditLog` vb.).

\- \*\*Helpers:\*\* Global oturum (`Session`), `PasswordHasher`, aktivite dinleyicisi.


 Test Hesapları

\- \*\*Admin:\*\* `admin@banka.com` | Parola: `Admin2026!`

\- \*\*User (Normal):\*\* `user@banka.com` | Parola: `User2026!`

\- \*\*User (Panik / Duress):\*\* `user@banka.com` | Parola: `Panic2026!`

