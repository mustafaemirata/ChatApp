using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ChatApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=EMIR\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;Encrypt=False");

        public string Numara;

        // Form Yüklendiğinde
        private void Form2_Load(object sender, EventArgs e)
        {
            lblNumara.Text = Numara;
            gelenkutusu();
            gidenkutusu();

            baglanti.Open();

            SqlCommand komut = new SqlCommand("SELECT AD, SOYAD FROM Tbl_Kisiler WHERE NUMARA=" + Numara, baglanti);
            SqlDataReader dr = komut.ExecuteReader();

            while (dr.Read())
            {
                lblAdSoyad.Text = dr[0] + " " + dr[1];
            }

            baglanti.Close();
        }

        // Gelen Kutusu (Şifre çözerek gösterim)
        void gelenkutusu()
        {
            SqlDataAdapter da1 = new SqlDataAdapter("SELECT MESAJID, (AD + ' ' + SOYAD) AS GONDEREN, BASLIK, ICERIK " +
                                                    "FROM Tbl_Mesajlar " +
                                                    "INNER JOIN Tbl_Kisiler ON Tbl_Mesajlar.GONDEREN = Tbl_Kisiler.NUMARA " +
                                                    "WHERE ALICI = @Numara", baglanti);
            da1.SelectCommand.Parameters.AddWithValue("@Numara", Numara);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);

            // Mesajları çözme
            foreach (DataRow row in dt1.Rows)
            {
                row["ICERIK"] = SifreCoz(row["ICERIK"].ToString()); // Şifre çözülmüş içerik
            }

            dataGridView1.DataSource = dt1;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Giden Kutusu
        void gidenkutusu()
        {
            SqlDataAdapter da2 = new SqlDataAdapter("SELECT MESAJID, (AD + ' ' + SOYAD) AS ALICI, BASLIK, ICERIK " +
                                                    "FROM Tbl_Mesajlar " +
                                                    "INNER JOIN Tbl_Kisiler ON Tbl_Mesajlar.ALICI = Tbl_Kisiler.NUMARA " +
                                                    "WHERE GONDEREN = @Numara", baglanti);
            da2.SelectCommand.Parameters.AddWithValue("@Numara", Numara);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);

            // Mesajları çözme
            foreach (DataRow row in dt2.Rows)
            {
                row["ICERIK"] = SifreCoz(row["ICERIK"].ToString()); // Şifre çözülmüş içerik
            }

            dataGridView4.DataSource = dt2;
            dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Mesaj Gönderme (Şifreleyerek kaydetme)
        private void button2_Click(object sender, EventArgs e)
        {
            // Mesajı şifrele
            string sifreliMesaj = Sifrele(mesajTextBox.Text);

            baglanti.Open();

            SqlCommand komut = new SqlCommand("INSERT INTO Tbl_Mesajlar (GONDEREN, ALICI, BASLIK, ICERIK) " +
                                              "VALUES (@P1, @P2, @P3, @P4)", baglanti);
            komut.Parameters.AddWithValue("@P1", Numara);
            komut.Parameters.AddWithValue("@P2", aliciTextBox.Text);
            komut.Parameters.AddWithValue("@P3", baslilkTextBox.Text);
            komut.Parameters.AddWithValue("@P4", sifreliMesaj); // Şifrelenmiş içerik

            komut.ExecuteNonQuery();
            baglanti.Close();

            MessageBox.Show("Mesaj iletildi.");
            gidenkutusu();
        }

        // Base64 Şifreleme
        private string Sifrele(string plainText)
        {
            byte[] plainTextBytes = Encoding.ASCII.GetBytes(plainText); // Metni ASCII byte[]'e çevir
            return Convert.ToBase64String(plainTextBytes); // Byte[]'i Base64 string'e çevir
        }

        // Base64 Şifre Çözme
        private string SifreCoz(string encryptedText)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText); // Base64 string'i byte[]'e çevir
                return Encoding.ASCII.GetString(encryptedBytes); // Byte[]'i string'e çevir
            }
            catch
            {
                return "Şifre Çözme Hatası"; // Geçersiz Base64 verisi için
            }
        }
    }
}
