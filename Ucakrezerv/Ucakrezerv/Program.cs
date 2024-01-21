using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Ucakrezerv.Models;

namespace Ucakrezerv
{
    class Program
    {
        static void Main()
        {
            Program program = new Program();
            program.Calistir();
        }

        private List<Ucak> ucaklar = new List<Ucak>();
        private List<Lokasyon> lokasyonlar = new List<Lokasyon>();
        private List<Ucus> ucuslar = new List<Ucus>();
        private List<Rezervasyon> rezervasyonlar = new List<Rezervasyon>();

        public void Calistir()
        {
            bool devam = true;

            while (devam)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("1. Uçak Ekle");
                Console.WriteLine("2. Lokasyon Ekle");
                Console.WriteLine("3. Uçuş Ekle");
                Console.WriteLine("4. Rezervasyon Yap");
                Console.WriteLine("5. Çıkış");

                Console.Write("Lütfen bir seçenek girin (1-5): ");
                string secim = Console.ReadLine();

                switch (secim)
                {
                    case "1":
                        UcakEkle();
                        break;
                    case "2":
                        LokasyonEkle();
                        break;
                    case "3":
                        UcusEkle();
                        break;
                    case "4":
                        RezervasyonYap();
                        break;
                    case "5":
                        devam = false;
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçenek, lütfen tekrar deneyin.");
                        break;
                }
            }
        }

        private void UcakEkle()
        {
            Console.ForegroundColor = ConsoleColor.Gray; 
            Console.WriteLine("Uçak Ekleme İşlemi");
            Console.Write("Model: ");
            string model = Console.ReadLine();
            Console.Write("Marka: ");
            string marka = Console.ReadLine();
            Console.Write("Seri No: ");
            string seriNo = Console.ReadLine();
            Console.Write("Koltuk Kapasitesi: ");
            int koltukKapasitesi;
            if (int.TryParse(Console.ReadLine(), out koltukKapasitesi))
            {
                Ucak ucak = new Ucak { Model = model, Marka = marka, SeriNo = seriNo, KoltukKapasitesi = koltukKapasitesi };
                ucaklar.Add(ucak);
                Console.WriteLine("Uçak başarıyla eklendi.");
            }
            else
            {
                Console.WriteLine("Geçersiz koltuk kapasitesi. İşlem iptal edildi.");
            }
            Console.ResetColor(); 
            FileService.SaveDataToJson(ucaklar, "ucaklar");
        }

        private void LokasyonEkle()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta; 
            Console.WriteLine("Lokasyon Ekleme İşlemi");
            Console.Write("Ülke: ");
            string ulke = Console.ReadLine();
            Console.Write("Şehir: ");
            string sehir = Console.ReadLine();
            Console.Write("Havaalanı: ");
            string havaalani = Console.ReadLine();
            Console.Write("Aktif (true/false): ");
            bool aktif;
            if (bool.TryParse(Console.ReadLine(), out aktif))
            {
                Lokasyon lokasyon = new Lokasyon { Ulke = ulke, Sehir = sehir, Havaalani = havaalani, Aktif = aktif };
                lokasyonlar.Add(lokasyon);
                Console.WriteLine("Lokasyon başarıyla eklendi.");
            }
            else
            {
                Console.WriteLine("Geçersiz aktif değeri. İşlem iptal edildi.");
            }
            Console.ResetColor();
            FileService.SaveDataToJson(lokasyonlar, "lokasyonlar");
        }

        private void UcusEkle()
        {
            Console.ForegroundColor = ConsoleColor.White; 
            Console.WriteLine("Uçuş Ekleme İşlemi");
            Console.WriteLine("Lokasyonları aşağıdaki formatta girin (Ülke,Şehir,Havaalanı,Aktif):");
            Console.Write("Kalkış Lokasyonu: ");
            Lokasyon kalkisLokasyon = LokasyonGir();
            Console.Write("Varış Lokasyonu: ");
            Lokasyon varisLokasyon = LokasyonGir();
            Console.Write("Saat: ");
            string saat = Console.ReadLine();
            Console.Write("Uçak Modeli: ");
            string ucakModel = Console.ReadLine();

            Ucak ucak = ucaklar.Find(u => u.Model == ucakModel);
            if (ucak != null)
            {
                Ucus ucus = new Ucus { Lokasyon = kalkisLokasyon, Saat = saat, UcakBilgisi = ucak };
                ucuslar.Add(ucus);
                Console.WriteLine("Uçuş başarıyla eklendi.");
            }
            else
            {
                Console.WriteLine($"Uçak bulunamadı. '{ucakModel}' model uçağı ekleyiniz.");
            }
            Console.ResetColor(); 
            FileService.SaveDataToJson(ucuslar, "ucuslar");
        }

        private void RezervasyonYap()
        {
            Console.ForegroundColor = ConsoleColor.Yellow; 
            Console.WriteLine("Rezervasyon Yapma İşlemi");
            Console.Write("Ad: ");
            string ad = Console.ReadLine();
            Console.Write("Soyad: ");
            string soyad = Console.ReadLine();
            Console.Write("Yaş: ");
            int yas;
            if (int.TryParse(Console.ReadLine(), out yas))
            {
                Console.Write("Uçuş Saati: ");
                string ucusSaat = Console.ReadLine();

                Ucus ucus = ucuslar.Find(u => u.Saat == ucusSaat);
                if (ucus != null)
                {
                    Musteri musteri = new Musteri { Ad = ad, Soyad = soyad, Yas = yas };
                    Rezervasyon rezervasyon = new Rezervasyon { Ucus = ucus, Musteri = musteri };

                    if (ucus.UcakBilgisi.KoltukKapasitesi > 0)
                    {
                        rezervasyonlar.Add(rezervasyon);
                        ucus.UcakBilgisi.KoltukKapasitesi -= 1;
                        Console.WriteLine("Rezervasyon başarıyla tamamlanmıştır.");
                    }
                    else
                    {
                        Console.WriteLine("Üzgünüz, uçak dolu. Rezervasyon yapılamadı.");
                    }
                }
                else
                {
                    Console.WriteLine($"Uçuş bulunamadı. '{ucusSaat}' saatli uçuşu ekleyiniz.");
                }
            }
            else
            {
                Console.WriteLine("Geçersiz yaş değeri. İşlem iptal edildi.");
            }
            Console.ResetColor(); 
            FileService.SaveDataToJson(rezervasyonlar, "rezervasyonlar");
        }

        private Lokasyon LokasyonGir()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            string[] lokasyonBilgisi = Console.ReadLine().Split(',');
            if (lokasyonBilgisi.Length == 4)
            {
                string ulke = lokasyonBilgisi[0];
                string sehir = lokasyonBilgisi[1];
                string havaalani = lokasyonBilgisi[2];
                bool aktif;
                if (bool.TryParse(lokasyonBilgisi[3], out aktif))
                {
                    return new Lokasyon { Ulke = ulke, Sehir = sehir, Havaalani = havaalani, Aktif = aktif };
                }
            }

            Console.WriteLine("Geçersiz lokasyon formatı. İşlem iptal edildi.");
            return null;
        }
    }
}


