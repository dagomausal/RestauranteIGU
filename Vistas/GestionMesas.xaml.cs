using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PracticaFinalV2.Logica;
using PracticaFinalV2.Modelos;

namespace PracticaFinalV2.Vistas
{
    public partial class GestionMesas : Window
    {
        private LogicaRestaurante Logica;
        public ObservableCollection<Mesa> ListaMesasTemporal;
        public List<Mesa> ListaMesasABorrar;
        public GestionMesas(LogicaRestaurante logicaConstructor)
        {
            InitializeComponent();

            Logica = logicaConstructor;
            ListaMesasTemporal = new ObservableCollection<Mesa>(Logica.ListaMesas);
            ListaMesasABorrar = new List<Mesa>();
            lvMesas.ItemsSource = ListaMesasTemporal;
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtCapacidad.Text, out int comensales))
            {
                MessageBox.Show("No se ha introducido un numero de comensales.", "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TipoMesa forma;
            if (rbElip.IsChecked == true) forma = TipoMesa.Circular;
            else if (rbRect.IsChecked == true) forma = TipoMesa.Rectangular;
            else
            {
                MessageBox.Show("No se ha seleccionado una forma para la mesa.", "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int idObtenido = Logica.ObtenerSiguienteIdDisponible(ListaMesasTemporal);
            ListaMesasTemporal.Add(Logica.CrearMesa(idObtenido, comensales, forma));
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (lvMesas.SelectedItem != null)
            {
                Mesa mesaSeleccionada = (Mesa)lvMesas.SelectedItem;

                if (mesaSeleccionada.Estado != EstadoMesa.Libre)
                {
                    MessageBox.Show("No se puede eliminar una mesa que no esté libre.", "Error de Estado", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ListaMesasTemporal.Remove(mesaSeleccionada);

                if (Logica.ListaMesas.Contains(mesaSeleccionada))
                {
                    ListaMesasABorrar.Add(mesaSeleccionada);
                }

            } else
            {
                MessageBox.Show("Selecciona una mesa de la lista para eliminar.", "Error de Selección", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
