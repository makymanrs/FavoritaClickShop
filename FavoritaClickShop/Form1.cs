using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FavoritaClickShop
{
    public partial class Form1 : MaterialForm
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        public Form1()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue700,
                Primary.Grey900,
                Primary.BlueGrey500,
                Accent.LightBlue400,
                TextShade.WHITE);
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            this.Resize += new EventHandler(Form1_Resize);
            //ColorBotones.SetButtonColors(button1, Color.FromArgb(120, 103, 229), Color.White);
            CustomMaterialTabControl customTabControl = materialTabControl1 as CustomMaterialTabControl;
            if (customTabControl != null)
            {
                customTabControl.TabBarColor = Color.DimGray;
            }
            ShowTabPage1();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                Region = null; // Restablece la región a null al maximizar
            }
            else if (WindowState == FormWindowState.Normal)
            {
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20)); // Aplica la región redondeada al restaurar
            }
        }

        private void ShowTabPage1()
        {
            materialTabControl1.SelectedTab = tabPage1;
            Forms.FormHome formulario1 = new Forms.FormHome(this);
            formulario1.TopLevel = false;
            formulario1.FormBorderStyle = FormBorderStyle.None;
            formulario1.Dock = DockStyle.Fill;
            tabPage1.Controls.Clear(); // Limpiar cualquier control existente
            tabPage1.Controls.Add(formulario1);
            formulario1.Show();
        }

        public void ChangeTab(int tabIndex)
        {
            if (tabIndex >= 0 && tabIndex < materialTabControl1.TabPages.Count)
            {
                materialTabControl1.SelectedIndex = tabIndex;
            }
        }

        private void materialTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialTabControl1.SelectedTab == tabPage1)
            {
                // Crear instancia del formulario que deseas mostrar
                Forms.FormHome formulario1 = new Forms.FormHome(this);
                formulario1.TopLevel = false;
                formulario1.FormBorderStyle = FormBorderStyle.None;
                formulario1.Dock = DockStyle.Fill;
                tabPage1.Controls.Clear(); // Limpiar cualquier control existente
                tabPage1.Controls.Add(formulario1);
                formulario1.Show();
            }
            if (materialTabControl1.SelectedTab == tabPage2)
            {
                // Crear instancia del formulario que deseas mostrar
                Forms.FormProducto formulario2 = new Forms.FormProducto();
                formulario2.TopLevel = false;
                formulario2.FormBorderStyle = FormBorderStyle.None;
                formulario2.Dock = DockStyle.Fill;
                tabPage2.Controls.Clear(); // Limpiar cualquier control existente
                tabPage2.Controls.Add(formulario2);
                formulario2.Show();
            }
            if (materialTabControl1.SelectedTab == tabPage3)
            {
                // Crear instancia del formulario que deseas mostrar
                Forms.FormCliente formulario3 = new Forms.FormCliente();
                formulario3.TopLevel = false;
                formulario3.FormBorderStyle = FormBorderStyle.None;
                formulario3.Dock = DockStyle.Fill;
                tabPage3.Controls.Clear(); // Limpiar cualquier control existente
                tabPage3.Controls.Add(formulario3);
                formulario3.Show();
            }
            if (materialTabControl1.SelectedTab == tabPage4)
            {
                Forms.FormFactura formulario4 = new Forms.FormFactura();
                formulario4.TopLevel = false;
                formulario4.FormBorderStyle = FormBorderStyle.None;
                formulario4.Dock = DockStyle.Fill;
                tabPage4.Controls.Clear(); // Limpiar cualquier control existente
                tabPage4.Controls.Add(formulario4);
                formulario4.Show();
            }
            if (materialTabControl1.SelectedTab == tabPage1)
            {
                // Crear instancia del formulario que deseas mostrar
                Forms.FormHome formulario1 = new Forms.FormHome(this);
                formulario1.TopLevel = false;
                formulario1.FormBorderStyle = FormBorderStyle.None;
                formulario1.Dock = DockStyle.Fill;
                tabPage1.Controls.Clear(); // Limpiar cualquier control existente
                tabPage1.Controls.Add(formulario1);
                formulario1.Show();
            }
            
            if (materialTabControl1.SelectedTab == tabPage5)
            {
                Forms.FormBodega formulario5 = new Forms.FormBodega();
                formulario5.TopLevel = false;
                formulario5.FormBorderStyle = FormBorderStyle.None;
                formulario5.Dock = DockStyle.Fill;
                tabPage5.Controls.Clear(); // Limpiar cualquier control existente
                tabPage5.Controls.Add(formulario5);
                formulario5.Show();
            }
            if (materialTabControl1.SelectedTab == tabPage6)
            {
                Forms.FormCredito formulario6 = new Forms.FormCredito();
                formulario6.TopLevel = false;
                formulario6.FormBorderStyle = FormBorderStyle.None;
                formulario6.Dock = DockStyle.Fill;
                tabPage6.Controls.Clear(); // Limpiar cualquier control existente
                tabPage6.Controls.Add(formulario6);
                formulario6.Show();
            }
            if (materialTabControl1.SelectedTab == tabPage7)
            {
                Forms.FormHistorial formulario7 = new Forms.FormHistorial();
                formulario7.TopLevel = false;
                formulario7.FormBorderStyle = FormBorderStyle.None;
                formulario7.Dock = DockStyle.Fill;
                tabPage7.Controls.Clear(); // Limpiar cualquier control existente
                tabPage7.Controls.Add(formulario7);
                formulario7.Show();
            }
            if (materialTabControl1.SelectedTab == tabPage8)
            {
                Forms.FormProveedor formulario8 = new Forms.FormProveedor();
                formulario8.TopLevel = false;
                formulario8.FormBorderStyle = FormBorderStyle.None;
                formulario8.Dock = DockStyle.Fill;
                tabPage8.Controls.Clear(); // Limpiar cualquier control existente
                tabPage8.Controls.Add(formulario8);
                formulario8.Show();
            }
        }
    }
}
