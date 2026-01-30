using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using PracticaFinalV2.Logica;
using PracticaFinalV2.Modelos;

namespace PracticaFinalV2.Servicios
{
    public static class DibujanteGraficos
    {
        public static void DibujarGraficoGlobal(Canvas lienzoGlobal, LogicaRestaurante Logica)
        {
            if (lienzoGlobal.ActualWidth == 0 || lienzoGlobal.ActualHeight == 0) return;
            lienzoGlobal.Children.Clear();

            if (Logica.ListaMesas.Count == 0)
            {
                TextBlock txtAviso = new TextBlock();
                txtAviso.Text = "No hay mesas para ver sus estadísticas";
                txtAviso.FontSize = 16;
                txtAviso.FontWeight = FontWeights.Bold;
                txtAviso.TextAlignment = TextAlignment.Center;
                txtAviso.Width = lienzoGlobal.ActualWidth;
                Canvas.SetLeft(txtAviso, 0);
                Canvas.SetTop(txtAviso, lienzoGlobal.ActualHeight / 2 - 20);
                lienzoGlobal.Children.Add(txtAviso);

                return;
            }

            double anchoTotal = lienzoGlobal.ActualWidth;
            double altoTotal = lienzoGlobal.ActualHeight;

            double margen = 40;

            double altoUtil = altoTotal - 2 * margen;
            double anchoUtil = anchoTotal - 2 * margen;
            double yPos = altoTotal - margen;

            int maxPlatos = 1;
            foreach (Mesa mesa in Logica.ListaMesas)
            {
                int cantidad = Logica.CalcularTotalPlatos(mesa);
                if (cantidad > maxPlatos) maxPlatos = cantidad;
            }

            double escalaPlatos = altoUtil / maxPlatos;
            int numMesas = Logica.ListaMesas.Count;
            double anchoColumna = anchoUtil / numMesas;
            double anchoBarra = anchoColumna * 0.6;
            double margenBarra = (anchoColumna - anchoBarra) / 2;

            for (int i = 0; i < numMesas; i++)
            {
                Mesa m = Logica.ListaMesas[i];
                int totalPlatos = Logica.CalcularTotalPlatos(m);
                double alturaBarra = totalPlatos * escalaPlatos;

                double xPos = margen + (i * anchoColumna) + margenBarra;

                Rectangle barra = new Rectangle
                {
                    Width = anchoBarra,
                    Height = alturaBarra,
                    Fill = Brushes.SteelBlue,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                Canvas.SetLeft(barra, xPos);
                Canvas.SetTop(barra, yPos - alturaBarra);

                // --- Crear texto cantidad de platos ---
                TextBlock txtCant = new TextBlock();
                txtCant.Text = totalPlatos.ToString();
                txtCant.FontWeight = FontWeights.Bold;
                txtCant.FontSize = 14;
                txtCant.TextAlignment = TextAlignment.Center;
                txtCant.Width = anchoBarra;

                Canvas.SetLeft(txtCant, xPos);
                Canvas.SetTop(txtCant, yPos - alturaBarra - 20);

                // --- Crear texto ID mesa ---
                TextBlock txtMesa = new TextBlock();
                txtMesa.Text = "Mesa " + m.Id;
                txtMesa.FontWeight = FontWeights.Bold;
                txtMesa.FontSize = 12;
                txtMesa.TextAlignment = TextAlignment.Center;
                txtMesa.Width = anchoBarra;

                Canvas.SetLeft(txtMesa, xPos);
                Canvas.SetTop(txtMesa, yPos + 5);

                // --- Añadir al lienzo ---
                lienzoGlobal.Children.Add(barra);
                lienzoGlobal.Children.Add(txtCant);
                lienzoGlobal.Children.Add(txtMesa);
            }

            Line ejeX = new Line();
            ejeX.X1 = margen;
            ejeX.Y1 = yPos;
            ejeX.X2 = anchoTotal - margen;
            ejeX.Y2 = yPos;
            ejeX.Stroke = Brushes.Black;
            ejeX.StrokeThickness = 2;
            lienzoGlobal.Children.Add(ejeX);
        }

        public static void DibujarGraficoMesa(Canvas lienzoMesa, Mesa MesaSeleccionada)
        {
            if (lienzoMesa.ActualWidth == 0 || lienzoMesa.ActualHeight == 0) return;

            lienzoMesa.Children.Clear();

            if (MesaSeleccionada == null)
            {
                TextBlock txtAviso = new TextBlock();
                txtAviso.Text = "Selecciona una mesa para ver sus estadísticas";
                txtAviso.FontSize = 16;
                txtAviso.FontWeight = FontWeights.Bold;
                txtAviso.TextAlignment = TextAlignment.Center;
                txtAviso.Width = lienzoMesa.ActualWidth;
                Canvas.SetLeft(txtAviso, 0);
                Canvas.SetTop(txtAviso, lienzoMesa.ActualHeight / 2 - 20);
                lienzoMesa.Children.Add(txtAviso);

                return;
            }

            double anchoTotal = lienzoMesa.ActualWidth;
            double altoTotal = lienzoMesa.ActualHeight;

            double margen = 40;

            double altoUtil = altoTotal - 2 * margen;
            double anchoUtil = anchoTotal - 2 * margen;
            double yPos = altoTotal - margen;

            int maxPlatos = 1;
        }
    }
}
