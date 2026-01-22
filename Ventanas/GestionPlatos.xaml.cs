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

namespace PracticaFinalV2.Ventanas
{
    public partial class GestionPlatos : Window
    {
        private LogicaRestaurante Logica;
        private ObservableCollection<Plato> menuTemporal;
        private List<Plato> platosABorrar;
        public GestionPlatos(LogicaRestaurante logicaConstructor)
        {
            InitializeComponent();

            Logica = logicaConstructor;
            menuTemporal = new ObservableCollection<Plato>(Logica.MenuDelDia);
            platosABorrar = new List<Plato>();

            cbCategoria.ItemsSource = Enum.GetValues(typeof(CategoriaPlato));
            lvCarta.ItemsSource = menuTemporal;
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text;
            CategoriaPlato categoria = (CategoriaPlato)cbCategoria.SelectedItem;
            string descripcion = txtDescripcion.Text;
            
            foreach (Plato p in menuTemporal)
            {
                if (p.Nombre.Equals(nombre))
                {
                    MessageBox.Show("Ya existe un plato con ese nombre.", "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            menuTemporal.Add(Logica.CrearPlato(nombre, categoria, descripcion));
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (lvCarta.SelectedItem != null)
            {
                Plato platoSeleccionado = (Plato)lvCarta.SelectedItem;
                menuTemporal.Remove(platoSeleccionado);

                if (Logica.MenuDelDia.Contains(platoSeleccionado))
                {
                    platosABorrar.Add(platoSeleccionado);
                }

            } else
            {
                MessageBox.Show("Seleccione un plato de la lista para eliminarlo.", "Error de Selección", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            foreach (Plato p in platosABorrar) if (Logica.MenuDelDia.Contains(p)) Logica.EliminarPlato(p);

            foreach (Plato p in menuTemporal) if (!Logica.MenuDelDia.Contains(p)) Logica.AnadirPlato(p);

            this.Close();
        }
    }
}
