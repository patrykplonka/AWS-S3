using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;

class Program
{
    static void Główna(string[] args)
    {
        string kluczDostępu = "TwójKluczDostępu";
        string tajnyKlucz = "TwójTajnyKlucz";
        string nazwaKosza = "NazwaKosza";
        string region = "Region";

        var poświadczenia = new Amazon.Runtime.BasicAWSCredentials(kluczDostępu, tajnyKlucz);
        var konfiguracja = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(region)
        };

        using var klientS3 = new AmazonS3Client(poświadczenia, konfiguracja);

        string kluczObiektu = "KluczObiektu";
        PobierzObiektZS3(klientS3, nazwaKosza, kluczObiektu);

        string ścieżkaPliku = "ŚcieżkaDoTwojegoPliku";
        string nowyKluczObiektu = "NowyKluczObiektu";
        DodajObiektDoS3(klientS3, nazwaKosza, nowyKluczObiektu, ścieżkaPliku);

        Console.WriteLine("Operacje zakończone!");
    }

    static void PobierzObiektZS3(IAmazonS3 klient, string nazwaKosza, string kluczObiektu)
    {
        try
        {
            GetObjectRequest żądanie = new GetObjectRequest
            {
                BucketName = nazwaKosza,
                Key = kluczObiektu
            };

            using GetObjectResponse odpowiedź = klient.GetObject(żądanie);
            using Stream strumieńOdpowiedzi = odpowiedź.ResponseStream;
            using StreamReader czytelnik = new StreamReader(strumieńOdpowiedzi);
            string zawartość = czytelnik.ReadToEnd();
            Console.WriteLine("Zawartość obiektu:");
            Console.WriteLine(zawartość);
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine("Wystąpił błąd podczas pobierania obiektu. Komunikat: " + e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Wystąpił błąd. Komunikat: " + e.Message);
        }
    }

    static void DodajObiektDoS3(IAmazonS3 klient, string nazwaKosza, string kluczObiektu, string ścieżkaPliku)
    {
        try
        {
            PutObjectRequest żądanie = new PutObjectRequest
            {
                BucketName = nazwaKosza,
                Key = kluczObiektu,
                FilePath = ścieżkaPliku
            };

            PutObjectResponse odpowiedź = klient.PutObject(żądanie);
            Console.WriteLine("Obiekt przesłany. ETag: " + odpowiedź.ETag);
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine("Wystąpił błąd podczas przesyłania obiektu. Komunikat: " + e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Wystąpił błąd. Komunikat: " + e.Message);
        }
    }
}
