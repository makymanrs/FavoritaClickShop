using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FavoritaClickShop
{
    internal static class ColorBotones
    {
        // Método para cambiar el color de fondo y el color del texto de un botón
        public static void SetButtonColors(Button button, Color backgroundColor, Color textColor)
        {
            button.BackColor = backgroundColor;
            button.ForeColor = textColor;
        }
        public static void SetPanelColors(Panel panel, Color backgroundColor, Color borderColor, int borderWidth = 1)
        {
            panel.BackColor = backgroundColor;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid);
            };
        }
        public static void SetTabPageColors(TabControl tabControl, Color tabPageColor, Color textColor)
        {
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                tabPage.BackColor = tabPageColor;
                tabPage.ForeColor = textColor;
                foreach (Control control in tabPage.Controls)
                {
                    control.ForeColor = textColor;
                }
            }
        }

    }
}
