using MaterialSkin.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace FavoritaClickShop
{
    public class CustomMaterialTabControl : MaterialTabControl
    {
        public Color TabBarColor { get; set; } = Color.DimGray;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (SolidBrush brush = new SolidBrush(TabBarColor))
            {
                e.Graphics.FillRectangle(brush, new Rectangle(0, 0, this.Width, this.GetTabRect(0).Height));
            }
        }
    }
}