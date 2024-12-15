using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ChatApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=EMIR\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;Encrypt=False");

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void girisBtn_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("SELECT *FROM Tbl_Kisiler WHERE NUMARA=@P1 AND SIFRE=@P2", baglanti);
            komut.Parameters.AddWithValue("@P1", numara.Text);
            komut.Parameters.AddWithValue("@P2", sifre.Text);
            SqlDataReader dr = komut.ExecuteReader();

            if (dr.Read())
            {
                Form2 frm = new Form2();
                frm.Numara = numara.Text;   
                frm.ShowDialog();   
            }
            else
            {
                MessageBox.Show("Bilgiler hatalı!");
            }

                baglanti.Close();
        }
    }
}
