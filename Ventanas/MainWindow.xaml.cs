using System.Text;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PracticaFinalV2.Logica;
using PracticaFinalV2.Modelos;
using PracticaFinalV2.Ventanas;

namespace PracticaFinalV2.Ventanas
{
    public partial class MainWindow : Window
    {
        private LogicaRestaurante Logica;
        private Mesa mesaSeleccionada = null;
        private Grid gridSeleccionado = null;

        public MainWindow()
        {
            InitializeComponent();
            Logica = new LogicaRestaurante();

            // --- Eventos ---
            Logica.MesaAnadida += Logica_MesaAnadida;
            Logica.MesaEliminada += Logica_MesaEliminada;
            Logica.SeleccionCambiada += Logica_SeleccionCambiada;

            Logica.CargarDatosIniciales();
        }

        private void DibujarMesa(Mesa mesa)
        {
            // --- Contenedor principal ---
            Grid contenedor = new Grid();
            contenedor.Width = mesa.ancho;
            contenedor.Height = mesa.alto;

            Canvas.SetLeft(contenedor, mesa.X);
            Canvas.SetTop(contenedor, mesa.Y);

            // --- Figura de la mesa ---
            Shape figura;
            if (mesa.Forma == TipoMesa.Circular) figura = new Ellipse();
            else figura = new Rectangle();
            ActualizarColorFigura(figura, mesa);
            contenedor.Children.Add(figura);

            // --- Texto de la mesa ---
            TextBlock texto = new TextBlock();
            texto.Text = GenerarTextoMesa(mesa);
            texto.FontSize = 11;
            texto.FontWeight = FontWeights.Bold;
            texto.TextAlignment = TextAlignment.Center;
            texto.VerticalAlignment = VerticalAlignment.Center;
            texto.HorizontalAlignment = HorizontalAlignment.Center;
            texto.IsHitTestVisible = false;
            contenedor.Children.Add(texto);

            contenedor.Tag = mesa;

            contenedor.MouseLeftButtonDown += ContenedorMesa_Click;
            mesa.MesaActualizada += Logica_MesaActualizada;

            lienzoSala.Children.Add(contenedor);
        }

        private static void ActualizarColorFigura(Shape figura, Mesa mesa)
        {
            switch (mesa.Estado)
            {
                case EstadoMesa.Libre: figura.Fill = Brushes.LimeGreen; break;
                case EstadoMesa.Reservada: figura.Fill = Brushes.Yellow; break;
                case EstadoMesa.Ocupada: figura.Fill = Brushes.Orange; break;
                case EstadoMesa.OcupadaComanda: figura.Fill = Brushes.Red; break;
            }
        }

        private void AbrirVentanaDetalles()
        {
            DetallesMesas ventanaDetalles = new DetallesMesas(Logica);

            if (ventanaDetalles == null || !ventanaDetalles.IsLoaded) ventanaDetalles.Show();
            else ventanaDetalles.Activate();
        }

        private void AbrirVentanaComanda()
        {
            if (Logica.MesaSeleccionada.Estado != EstadoMesa.Libre && Logica.MesaSeleccionada.Estado != EstadoMesa.Reservada)
            {
                VentanaComanda ventanaComanda = new VentanaComanda(Logica.MesaSeleccionada, Logica.MenuDelDia);
                ventanaComanda.ShowDialog();
            } else
            {
                MessageBox.Show("Primero debes Ocupar la mesa para gestionar su comanda.", "Acción no permitida", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void AbrirVentanaGestionMesas()
        {
            GestionMesas ventanaGestion = new GestionMesas(Logica);
            ventanaGestion.ShowDialog();
        }

        private void ActualizarPanelDerecho(Mesa mesa)
        {
            if (mesa != null)
            {
                txtInfoMesa.Text = $"Mesa {mesaSeleccionada.Id}";
                txtComensales.Text = $"{mesaSeleccionada.ComensalesActuales}";

                // --- Habilitar botones segun estado ---
                btnReservar.IsEnabled = mesaSeleccionada.Estado == EstadoMesa.Libre;
                btnOcupar.IsEnabled = mesaSeleccionada.Estado == EstadoMesa.Libre || mesaSeleccionada.Estado == EstadoMesa.Reservada;
                btnLiberar.IsEnabled = mesaSeleccionada.Estado == EstadoMesa.Ocupada || mesaSeleccionada.Estado == EstadoMesa.OcupadaComanda || mesaSeleccionada.Estado == EstadoMesa.Reservada;
                btnGestionarComanda.IsEnabled = mesaSeleccionada.Estado == EstadoMesa.Ocupada || mesaSeleccionada.Estado == EstadoMesa.OcupadaComanda;

            }
            else
            {
                txtInfoMesa.Text = "-";
                txtComensales.Text = "0";
                btnReservar.IsEnabled = false;
                btnOcupar.IsEnabled = false;
                btnLiberar.IsEnabled = false;
                btnGestionarComanda.IsEnabled = false;
            }
        }

        private static String GenerarTextoMesa(Mesa mesa)
        {
            return $"Mesa {mesa.Id}\n" +
                   $"Capacidad: {mesa.CapacidadMaxima}\n" +
                   $"Comensales: {mesa.ComensalesActuales}";
        }

        private void DibujarGraficoGlobal()
        {
            if (lienzoGlobal.ActualWidth == 0 || lienzoGlobal.ActualHeight == 0) return;
            lienzoGlobal.Children.Clear();
            
            if (Logica.ListaMesas.Count == 0)
            {
                MessageBox.Show("No hay mesas para mostrar estadísticas.", "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                tabsPrincipal.SelectedIndex = 0;
                return;
            }

            double anchoTotal = lienzoGlobal.ActualWidth;
            double altoTotal = lienzoGlobal.ActualHeight;

            double margen = 40;

            double altoUtil = altoTotal - 2 * margen;
            double anchoUtil = anchoTotal - 2 * margen;
            double yPos = altoTotal - margen;

            // FALTA INCLUIR EL HISTORICO DE COMANDAS
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

        private void DibujarGraficoMesa()
        {
            if (lienzoGlobal.ActualWidth == 0 || lienzoGlobal.ActualHeight == 0) return;
            lienzoMesa.Children.Clear();

            if (mesaSeleccionada == null)
            {
                MessageBox.Show("No hay ninguna mesa seleccionada.", "Error de Selección", MessageBoxButton.OK, MessageBoxImage.Error);
                tabsPrincipal.SelectedIndex = 0;
                return;
            }

            double anchoTotal = lienzoGlobal.ActualWidth;
            double altoTotal = lienzoGlobal.ActualHeight;

            double anchoLeyenda = 150;

            double margen = 40;

            double altoUtil = altoTotal - 2 * margen;
            double anchoUtil = anchoTotal - 2 * margen - anchoLeyenda;
            double yPos = altoTotal - margen;

            int maxPlatos = 1;
            foreach (Mesa mesa in Logica.ListaMesas)
            {               
                foreach(CategoriaPlato cat in Enum.GetValues(typeof(CategoriaPlato)))
                {
                    int cantidad = Logica.CalcularTotalPlatosCategoria(mesa, cat);
                    if (cantidad > maxPlatos) maxPlatos = cantidad;                    
                }
            }

            double escalaPlatos = altoUtil / maxPlatos;
            int numColumnas = Enum.GetValues(typeof(CategoriaPlato)).Length;
            double anchoColumna = anchoUtil / numColumnas;
            double anchoBarra = anchoColumna * 0.6;
            double margenBarra = (anchoColumna - anchoBarra) / 2;

            for (int i = 0; i < numColumnas; i++)
            {
                double xPos = margen + (i * anchoColumna) + margenBarra;
                double yActual = yPos;

                

                
            }
        }

        // --- EVENTOS COMUNICACION ---
        private void Logica_MesaAnadida(object sender, MesaEventArgs e)
        {
            Mesa mesaParaDibujar = e.MesaNueva;
            DibujarMesa(mesaParaDibujar);
        }
        private void Logica_MesaEliminada(object sender, MesaEventArgs e)
        {
            Mesa mesaEliminada = e.MesaNueva;

            foreach (var hijo in lienzoSala.Children)
            {
                if (hijo is Grid g && g.Tag == mesaEliminada)
                {
                    lienzoSala.Children.Remove(g);
                    break;
                }
            }
        }
        private void Logica_SeleccionCambiada(object sender, MesaEventArgs e)
        {
            mesaSeleccionada = e.MesaNueva;

            if (gridSeleccionado != null)
            {
                Shape mesaAntigua = (Shape)gridSeleccionado.Children[0];
                mesaAntigua.Stroke = null;
            }

            foreach (var hijo in lienzoSala.Children)
            {
                if (hijo is Grid g && g.Tag == mesaSeleccionada)
                {
                    gridSeleccionado = g;
                    break;
                }
            }

            if (gridSeleccionado != null)
            {
                Shape mesaNueva = (Shape)gridSeleccionado.Children[0];
                mesaNueva.Stroke = Brushes.DarkRed;
                mesaNueva.StrokeThickness = 3;
            }

            ActualizarPanelDerecho(mesaSeleccionada);
        }

        private void Logica_MesaActualizada(object sender, EventArgs e)
        {
            Mesa mesaCambio = (Mesa)sender;

            Grid? grid = null;
            foreach (var hijo in lienzoSala.Children)
            {
                if (hijo is Grid g && g.Tag == mesaCambio)
                {
                    grid = g;
                    break;
                }
            }

            if (grid != null)
            {
                if (grid.Children[0] is Shape figura) ActualizarColorFigura(figura, mesaCambio);
                if (grid.Children[1] is TextBlock texto) texto.Text = GenerarTextoMesa(mesaCambio);
                if (Logica.MesaSeleccionada == mesaCambio) ActualizarPanelDerecho(mesaCambio);
            }

            DibujarGraficoGlobal();
        }

        // --- EVENTOS INTERFAZ ---
        private void ContenedorMesa_Click(object sender, MouseButtonEventArgs e)
        {
            Grid contenedorClicado = (Grid)sender;
            Mesa mesaClicada = (Mesa)contenedorClicado.Tag;

            Logica.SeleccionarMesa(mesaClicada);

            if (e.ClickCount == 2)
            {
                AbrirVentanaComanda();
            }
        }

        private void btnReservar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtComensales.Text, out int comensales))
                {
                    MessageBox.Show("No se ha introducido un numero de comensales.", "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Logica.MesaSeleccionada.Reservar(comensales);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOcupar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtComensales.Text, out int comensales))
                {
                    MessageBox.Show("No se ha introducido un numero de comensales.", "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Logica.MesaSeleccionada.Ocupar(comensales);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnLiberar_Click(object sender, RoutedEventArgs e)
        {
            Logica.MesaSeleccionada.Liberar();
        }

        private void btnGestionarComanda_Click(object sender, RoutedEventArgs e)
        {
            AbrirVentanaComanda();
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tab = (TabControl)sender;
            TabItem item = (TabItem)tab.SelectedItem;

            if (item.Header.ToString() == "Estadística Global") DibujarGraficoGlobal();
            else if (item.Header.ToString() == "Estadística Mesa") DibujarGraficoMesa();
        }

        private void DetallesSala_Click(object sender, RoutedEventArgs e)
        {
            AbrirVentanaDetalles();
        }

        private void LienzoGlobal_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DibujarGraficoGlobal();
        }

        private void GestionarMesas_Click(object sender, RoutedEventArgs e)
        {
            AbrirVentanaGestionMesas();
        }
    }
}