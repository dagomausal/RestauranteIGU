using System.Collections.ObjectModel;
using System.Windows;
using PracticaFinalV2.Modelos;

namespace PracticaFinalV2.Vistas
{
    public partial class VentanaComanda : Window
    {
        private Mesa mesaActual;
        public ObservableCollection<PlatoComanda> ComandaTemporal;
        public VentanaComanda(Mesa mesaParaEditar, ObservableCollection<Plato> Menu)
        {
            InitializeComponent();
            this.mesaActual = mesaParaEditar;

            txtTituloMesa.Text = $"Gestionando Comanda - Mesa {mesaActual.Id}";
            cbMenu.ItemsSource = Menu;

            ComandaTemporal = new ObservableCollection<PlatoComanda>();

            foreach (PlatoComanda pc in mesaActual.Comanda)
            {
                ComandaTemporal.Add(new PlatoComanda(pc));
            }

            lvComandaActual.ItemsSource = ComandaTemporal;
        }
        private void btnAnadir_Click(object sender, RoutedEventArgs e)
        {
            if (cbMenu.SelectedItem != null)
            {
                Plato platoTemporal = (Plato)cbMenu.SelectedItem;
                foreach (PlatoComanda pc in ComandaTemporal)
                {
                    if (pc.PlatoPedido.Nombre == platoTemporal.Nombre)
                    {
                        pc.Cantidad++;
                        return;
                    }
                }
                ComandaTemporal.Add(new PlatoComanda(platoTemporal));
            } else
            {
                MessageBox.Show("Selecciona un plato del menú para añadir.", "Error de Selección", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnQuitar_Click(object sender, RoutedEventArgs e)
        {
            if (lvComandaActual.SelectedItem != null)
            {
                PlatoComanda platoSeleccionado = (PlatoComanda)lvComandaActual.SelectedItem;
                if (platoSeleccionado.Cantidad > 1)
                {
                    platoSeleccionado.Cantidad--;
                    return;
                }
                ComandaTemporal.Remove(platoSeleccionado);
            }

            else
            {
                MessageBox.Show("Selecciona un plato de la comanda para quitar.", "Error de Selección", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            mesaActual.ConfirmarComanda(ComandaTemporal);

            this.Close();
        }
    }
}
