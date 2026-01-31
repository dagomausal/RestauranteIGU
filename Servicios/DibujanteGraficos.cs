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
    public class DibujanteGraficos
    {
        public void DibujarGraficoGlobal(Canvas lienzoGlobal, LogicaRestaurante Logica)
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

            int maxPlatos = 0;
            foreach (Mesa mesa in Logica.ListaMesas)
            {
                int cantidad = Logica.CalcularTotalPlatos(mesa);
                if (cantidad > maxPlatos) maxPlatos = cantidad;
            }

            if (maxPlatos == 0)
            {
                TextBlock txtAviso = new TextBlock();
                txtAviso.Text = "No hay platos para mostrar en la sesión";
                txtAviso.FontSize = 16;
                txtAviso.FontWeight = FontWeights.Bold;
                txtAviso.TextAlignment = TextAlignment.Center;
                txtAviso.Width = lienzoGlobal.ActualWidth;
                Canvas.SetLeft(txtAviso, 0);
                Canvas.SetTop(txtAviso, lienzoGlobal.ActualHeight / 2 - 20);
                lienzoGlobal.Children.Add(txtAviso);

                return;
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
        public void DibujarGraficoMesa(Canvas lienzoMesa, LogicaRestaurante Logica, StackPanel panelLeyenda)
        {
            if (lienzoMesa.ActualWidth == 0 || lienzoMesa.ActualHeight == 0) return;

            lienzoMesa.Children.Clear();
            panelLeyenda.Children.Clear();

            TextBlock tituloLeyenda = new TextBlock
            {
                Text = "LEYENDA DE PLATOS",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5)
            };
            panelLeyenda.Children.Add(tituloLeyenda);

            if (Logica.MesaSeleccionada == null)
            {
                TextBlock txtAviso = new TextBlock()
                {
                    Text = "Selecciona una mesa para ver sus estadísticas",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Width = lienzoMesa.ActualWidth
                };

                Canvas.SetLeft(txtAviso, 0);
                Canvas.SetTop(txtAviso, lienzoMesa.ActualHeight / 2 - 20);
                lienzoMesa.Children.Add(txtAviso);

                return;
            }

            if (Logica.MesaSeleccionada.Comanda.Count <= 0 && Logica.MesaSeleccionada.HistoricoComandas.Count <= 0)
            {
                TextBlock txtAviso = new TextBlock()
                {
                    Text = "La mesa no tiene platos para mostrar",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Width = lienzoMesa.ActualWidth
                };

                Canvas.SetLeft(txtAviso, 0);
                Canvas.SetTop(txtAviso, lienzoMesa.ActualHeight / 2 - 20);
                lienzoMesa.Children.Add(txtAviso);

                return;
            }

            List<string> platosEnLeyenda = new List<string>();

            double anchoTotal = lienzoMesa.ActualWidth;
            double altoTotal = lienzoMesa.ActualHeight;

            double margen = 40;

            double altoUtil = altoTotal - 2 * margen;
            double anchoUtil = anchoTotal - 2 * margen;
            double yPos = altoTotal - margen;

            int maxPlatos = 1;

            foreach(CategoriaPlato cat in Enum.GetValues(typeof(CategoriaPlato)))
            {
                List<PlatoComanda> platosDeCategoria = Logica.ObtenerPlatoCategoria(Logica.MesaSeleccionada, cat);

                int sumaCat = 0;
                foreach(PlatoComanda pc in platosDeCategoria)
                {
                    sumaCat += pc.Cantidad;
                }

                if (sumaCat > maxPlatos) maxPlatos = sumaCat;
            }

            double escalaPlatos = altoUtil / maxPlatos;
            int numCat = Enum.GetValues(typeof(CategoriaPlato)).Length;
            double anchoColumna = anchoUtil / numCat;
            double anchoBarra = anchoColumna * 0.6;
            double margenBarra = (anchoColumna - anchoBarra) / 2;

            int indice = 0;
            foreach(CategoriaPlato cat in Enum.GetValues(typeof(CategoriaPlato)))
            {
                double xPos = margen + (indice * anchoColumna) + margenBarra;
                List<PlatoComanda> platosADibujar = Logica.ObtenerPlatoCategoria(Logica.MesaSeleccionada, cat);

                double alturaApilada = 0;

                foreach (PlatoComanda pc in platosADibujar)
                {
                    if (pc.Cantidad == 0) continue;

                    double alturaBarra = pc.Cantidad * escalaPlatos;

                    Rectangle barra = new Rectangle
                    {
                        Width = anchoBarra,
                        Height = alturaBarra,
                        Fill = GenerarColor(pc.PlatoPedido.Nombre),
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        ToolTip = $"{pc.PlatoPedido.Nombre} : {pc.Cantidad}"
                    };

                    Canvas.SetLeft(barra, xPos);
                    Canvas.SetTop(barra, yPos - alturaApilada - alturaBarra);

                    lienzoMesa.Children.Add(barra);
                    alturaApilada += alturaBarra;

                    if (!platosEnLeyenda.Contains(pc.PlatoPedido.Nombre))
                    {
                        platosEnLeyenda.Add(pc.PlatoPedido.Nombre);

                        StackPanel itemLeyenda = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 5, 0, 5)
                        };

                        Rectangle colorPlato = new Rectangle
                        {
                            Width = 15,
                            Height = 15,
                            Fill = GenerarColor(pc.PlatoPedido.Nombre),
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                        };

                        TextBlock txtNombre = new TextBlock
                        {
                            Text = pc.PlatoPedido.Nombre,
                            FontSize = 11,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(5, 0, 5, 0)
                        };

                        itemLeyenda.Children.Add(txtNombre);
                        itemLeyenda.Children.Add(colorPlato);

                        panelLeyenda.Children.Add(itemLeyenda);
                    }
                }

                TextBlock txtCat = new TextBlock();
                txtCat.Text = cat.ToString();
                txtCat.FontWeight = FontWeights.Bold;
                txtCat.FontSize = 12;
                txtCat.TextAlignment = TextAlignment.Center;
                txtCat.Width = anchoBarra;

                Canvas.SetLeft(txtCat, xPos);
                Canvas.SetTop(txtCat, yPos + 5);
                lienzoMesa.Children.Add(txtCat);

                indice++;
            }

            Line ejeX = new Line();
            ejeX.X1 = margen;
            ejeX.Y1 = yPos;
            ejeX.X2 = anchoTotal - margen;
            ejeX.Y2 = yPos;
            ejeX.Stroke = Brushes.Black;
            ejeX.StrokeThickness = 2;
            lienzoMesa.Children.Add(ejeX);
        }
        private Brush GenerarColor(string nombrePlato)
        {
            Random rand = new Random(nombrePlato.GetHashCode());

            byte r = (byte)rand.Next(50, 200);
            byte g = (byte)rand.Next(50, 200);
            byte b = (byte)rand.Next(50, 200);

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }
    }
}
