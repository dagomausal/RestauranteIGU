using System.ComponentModel;
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
using PracticaFinalV2.Servicios;

namespace PracticaFinalV2.Vistas
{
    public partial class MainWindow : Window
    {
        private LogicaRestaurante Logica;
        private Grid gridSeleccionado;
        private bool isDragging = false;
        private Point clickEnGrid;
        private Grid? gridArrastrado;

        public MainWindow()
        {
            InitializeComponent();
            Logica = new LogicaRestaurante();

            // --- Eventos ---
            Logica.MesaAnadida += Logica_MesaAnadida;
            Logica.MesaEliminada += Logica_MesaEliminada;
            Logica.SeleccionCambiada += Logica_SeleccionCambiada;
            
            Logica.CargarDatosIniciales();

            lienzoSala.MouseLeftButtonDown += LienzoSala_MouseLeftButtonDown;
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

            contenedor.MouseLeftButtonDown += Mesa_MouseLeftButtonDown;
            contenedor.MouseLeftButtonUp += Mesa_MouseLeftButtonUp;
            contenedor.MouseMove += Mesa_MouseMove;

            mesa.PropertyChanged += Logica_MesaActualizada;
            mesa.ComandaActualizada += Logica_ComandaActualizada;

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

        private void ControlVentanaComanda()
        {
            if (Logica.MesaSeleccionada.Estado != EstadoMesa.Libre && Logica.MesaSeleccionada.Estado != EstadoMesa.Reservada)
            {
                VentanaComanda ventanaComanda = new VentanaComanda(Logica.MesaSeleccionada, Logica.MenuDelDia);
                if (ventanaComanda.ShowDialog() == true)
                {
                    Logica.MesaSeleccionada.ConfirmarComanda(ventanaComanda.ComandaTemporal);
                    DibujanteGraficos.DibujarGraficoMesa(lienzoMesa, Logica.MesaSeleccionada);
                    DibujanteGraficos.DibujarGraficoGlobal(lienzoGlobal, Logica);
                }
            } else
            {
                MessageBox.Show("Primero debes Ocupar la mesa para gestionar su comanda.", "Acción no permitida", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void ControlVentanaGestionMesas()
        {
            GestionMesas ventanaGestion = new GestionMesas(Logica);
            if (ventanaGestion.ShowDialog() == true) {
                Logica.ProcesarMesas(ventanaGestion.ListaMesasTemporal, ventanaGestion.ListaMesasABorrar);
            }
        }

        private void ControlVentanaGestionPlatos()
        {
            GestionPlatos ventanaGestion = new GestionPlatos(Logica);
            if (ventanaGestion.ShowDialog() == true)
            {
                Logica.ProcesarPlatos(ventanaGestion.menuTemporal, ventanaGestion.platosABorrar);
            }
        }

        private void ActualizarPanelDerecho(Mesa mesa)
        {
            if (mesa != null)
            {
                txtInfoMesa.Text = $"Mesa {Logica.MesaSeleccionada.Id}";
                txtComensales.Text = $"{Logica.MesaSeleccionada.ComensalesActuales}";

                // --- Habilitar botones segun estado ---
                btnReservar.IsEnabled = Logica.MesaSeleccionada.Estado == EstadoMesa.Libre;
                btnOcupar.IsEnabled = Logica.MesaSeleccionada.Estado == EstadoMesa.Libre || Logica.MesaSeleccionada.Estado == EstadoMesa.Reservada;
                btnLiberar.IsEnabled = Logica.MesaSeleccionada.Estado == EstadoMesa.Ocupada || Logica.MesaSeleccionada.Estado == EstadoMesa.OcupadaComanda || Logica.MesaSeleccionada.Estado == EstadoMesa.Reservada;
                btnGestionarComanda.IsEnabled = Logica.MesaSeleccionada.Estado == EstadoMesa.Ocupada || Logica.MesaSeleccionada.Estado == EstadoMesa.OcupadaComanda;

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
        private void Logica_MesaEliminada(object sender, MesaEventArgs e)
        {
            if (e.MesaNueva == Logica.MesaSeleccionada)
            {
                gridSeleccionado = null;
                ActualizarPanelDerecho(null);
            }

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
            if (gridSeleccionado != null)
            {
                Shape mesaAntigua = (Shape)gridSeleccionado.Children[0];
                mesaAntigua.Stroke = null;
            }

            foreach (var hijo in lienzoSala.Children)
            {
                if (hijo is Grid g && g.Tag == Logica.MesaSeleccionada)
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

            ActualizarPanelDerecho(Logica.MesaSeleccionada);
            //DibujanteGraficos.DibujarGraficoMesa(lienzoMesa, Logica.MesaSeleccionada);
        }
        private void Logica_MesaActualizada(object sender, PropertyChangedEventArgs e)
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
        private void Logica_ComandaActualizada(object? sender, EventArgs e)
        {
            DibujanteGraficos.DibujarGraficoMesa(lienzoMesa, Logica.MesaSeleccionada);
            DibujanteGraficos.DibujarGraficoGlobal(lienzoGlobal, Logica);
        }

        // --- EVENTOS INTERFAZ ---
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
            ControlVentanaComanda();
        }
        private void Mesa_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gridArrastrado = (Grid)sender;
            Mesa mesaClicada = (Mesa)gridArrastrado.Tag;

            Logica.SeleccionarMesa(mesaClicada);

            if (e.ClickCount == 2)
            {
                ControlVentanaComanda();
                return;
            }

            isDragging = true;
            clickEnGrid = e.GetPosition(gridArrastrado);
            gridArrastrado.CaptureMouse();

            e.Handled = true;
        }
        private void Mesa_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging || gridArrastrado == null) return;

            Point posicionRaton = e.GetPosition(lienzoSala);

            double nuevaX = posicionRaton.X - clickEnGrid.X;
            double nuevaY = posicionRaton.Y - clickEnGrid.Y;

            if (nuevaX <0) nuevaX = 0;
            if (nuevaY <0) nuevaY = 0;
            if (nuevaX + gridArrastrado.ActualWidth > lienzoSala.ActualWidth) nuevaX = lienzoSala.ActualWidth - gridArrastrado.ActualWidth;
            if (nuevaY + gridArrastrado.ActualHeight > lienzoSala.ActualHeight) nuevaY = lienzoSala.ActualHeight - gridArrastrado.ActualHeight;

            Canvas.SetLeft(gridArrastrado, nuevaX);
            Canvas.SetTop(gridArrastrado, nuevaY);
        }
        private void Mesa_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging && gridArrastrado != null)
            {
                isDragging = false;
                gridArrastrado.ReleaseMouseCapture();

                Mesa mesa = (Mesa)gridArrastrado.Tag;

                mesa.X = Canvas.GetLeft(gridArrastrado);
                mesa.Y = Canvas.GetTop(gridArrastrado);

                gridArrastrado = null;
            }
        }
        private void LienzoSala_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (gridSeleccionado != null)
            {
                Shape mesaAntigua = (Shape)gridSeleccionado.Children[0];
                mesaAntigua.Stroke = null;
            }

            gridSeleccionado = null;
            Logica.SeleccionarMesa(null);
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tab = (TabControl)sender;
            TabItem item = (TabItem)tab.SelectedItem;

            if (item == tabGlobal) DibujanteGraficos.DibujarGraficoGlobal(lienzoGlobal, Logica);
            else if (item == tabMesa) DibujanteGraficos.DibujarGraficoMesa(lienzoMesa, Logica.MesaSeleccionada);
        }
        private void DetallesSala_Click(object sender, RoutedEventArgs e)
        {
            AbrirVentanaDetalles();
        }
        private void LienzoGlobal_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DibujanteGraficos.DibujarGraficoGlobal(lienzoGlobal, Logica);
        }
        private void LienzoMesa_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DibujanteGraficos.DibujarGraficoMesa(lienzoMesa, Logica.MesaSeleccionada);
        }
        private void GestionarMesas_Click(object sender, RoutedEventArgs e)
        {
            ControlVentanaGestionMesas();
        }
        private void GestionarPlatos_Click(object sender, RoutedEventArgs e)
        {
            ControlVentanaGestionPlatos();
        }
    }
}