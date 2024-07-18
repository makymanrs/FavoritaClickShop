using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FavoritaClickShop.Forms
{
    public partial class FormHome : Form
    {
        private Form1 _mainForm;

        public FormHome(Form1 mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            
        }

        //productos
        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(1);
        }

        private void label1_Click_1(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(1);
        }

        //clientes
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(2);

        }

        private void label3_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(2);

        }
        //pagos
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(3);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(3);

        }
        //inventario
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(4);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(4);
        }
        //credito
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(5);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(5);
        }
        //Historial
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(6);

        }

        private void label6_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(6);
        }
        //Proveedor
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(7);

        }
        private void label7_Click(object sender, EventArgs e)
        {
            _mainForm.ChangeTab(7);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelhr.Text = DateTime.Now.ToString("h:mm:ss");
            labelfch.Text = DateTime.Now.ToLongDateString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Hide();
            login.Show();
        }
        // Agrega más manejadores de eventos para otros paneles si es necesario
    }
}
