using System.Text;
using System.Windows;
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

namespace PracticaFinalV2
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
            mesa.MesaActualizada += OnMesaActualizada;

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

        // --- EVENTOS COMUNICACION ---
        private void Logica_MesaAnadida(object sender, MesaEventArgs e)
        {
            Mesa mesaParaDibujar = e.MesaNueva;
            DibujarMesa(mesaParaDibujar);
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

        private void OnMesaActualizada(object sender, EventArgs e)
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

            if (item.Header.ToString() == "Estadística Global") /*DibujarGraficoGlobal()*/;
            else if (item.Header.ToString() == "Estadística Mesa") /*DibujarGraficoMesa()*/;
        }

        private void DetallesSala_Click(object sender, RoutedEventArgs e)
        {
            AbrirVentanaDetalles();
        }
    }
}