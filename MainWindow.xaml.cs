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
            texto.Text = $"Mesa {mesa.Id}\n" +
                         $"Capacidad: {mesa.CapacidadMaxima}\n" +
                         $"Comensales: {mesa.ComensalesActuales}";
            texto.FontSize = 11;
            texto.FontWeight = FontWeights.Bold;
            texto.TextAlignment = TextAlignment.Center;
            texto.VerticalAlignment = VerticalAlignment.Center;
            texto.HorizontalAlignment = HorizontalAlignment.Center;
            texto.IsHitTestVisible = false;
            contenedor.Children.Add(texto);

            contenedor.Tag = mesa;

            contenedor.MouseLeftButtonDown += ContenedorMesa_Click;

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


        // --- EVENTOS COMUNICACION ---
        private void Logica_MesaAnadida(object sender, MesaEventArgs e)
        {
            Mesa mesaParaDibujar = e.MesaNueva;
            DibujarMesa(mesaParaDibujar);
        }
        private void Logica_SeleccionCambiada(object sender, MesaEventArgs e)
        {
            // Aquí se puede actualizar la interfaz según la mesa seleccionada
            Mesa mesaSeleccionada = e.MesaNueva;

            if (gridSeleccionado != null)
            {
                Shape mesaAntigua = (Shape)gridSeleccionado.Children[0];
                mesaAntigua.Stroke = null;
            }

            foreach(var hijo in lienzoSala.Children)
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
        }

        // --- EVENTOS INTERFAZ ---
        private void ContenedorMesa_Click(object sender, MouseButtonEventArgs e)
        {
            Grid contenedorClicado = (Grid)sender;
            Mesa mesaClicada = (Mesa)contenedorClicado.Tag;

            Logica.SeleccionarMesa(mesaClicada);

            if (e.ClickCount == 2)
            {
                // AbrirVentanaComanda(mesaClicada);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tab = (TabControl)sender;
            TabItem item = (TabItem)tab.SelectedItem;

            if (item.Header.ToString() == "Estadística Global") /*DibujarGraficoGlobal()*/;
            else if (item.Header.ToString() == "Estadística Mesa") /*DibujarGraficoMesa()*/;
        }

        private void DetallesSala_Click(object sender, RoutedEventArgs e )
        {
            AbrirVentanaDetalles();
        }
       
    }
}