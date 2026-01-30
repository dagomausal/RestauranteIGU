using System;
using System.Collections.Generic;
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
using PracticaFinalV2.Servicios;

namespace PracticaFinalV2.Vistas
{

    public partial class DetallesMesas : Window
    {
        private LogicaRestaurante Logica;
        public DetallesMesas(LogicaRestaurante logicaConstructor)
        {
            InitializeComponent();

            this.Logica = logicaConstructor;
            lvMesas.ItemsSource = Logica.ListaMesas;
            Logica.SeleccionCambiada += Logica_SeleccionCambiada;
            this.Closed += OnClosed;

            // --- Seleccionar la primera mesa al abrir la ventana ---
            if (Logica.MesaSeleccionada != null)
            {
                lvMesas.SelectedItem = Logica.MesaSeleccionada;
                CargarComanda(Logica.MesaSeleccionada);
            }
        }

        private void Logica_SeleccionCambiada(object sender, MesaEventArgs e)
        {
            lvMesas.SelectedItem = e.MesaNueva;
            CargarComanda(e.MesaNueva);
        }

        private void lvMesas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvMesas.SelectedItem != null)
            {
                Mesa mesaSeleccionada = (Mesa)lvMesas.SelectedItem;
                Logica.SeleccionarMesa(mesaSeleccionada);
                CargarComanda(mesaSeleccionada);
            }
        }

        private void CargarComanda(Mesa mesa)
        {
            if (mesa != null) lvComanda.ItemsSource = mesa.Comanda;
            else lvComanda.ItemsSource = null;
        }

        private void lvMesas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvMesas.SelectedItem!= null)
            {
                Mesa mesaSeleccionada = (Mesa)lvMesas.SelectedItem;

                if (mesaSeleccionada.Estado == EstadoMesa.Libre || mesaSeleccionada.Estado == EstadoMesa.Reservada)
                {
                    MessageBox.Show("Primero debes Ocupar la mesa para gestionar su comanda.", "Acción no permitida", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                VentanaComanda ventanaComanda = new VentanaComanda(mesaSeleccionada, Logica.MenuDelDia);

                if (ventanaComanda.ShowDialog() == true)
                {
                    mesaSeleccionada.ConfirmarComanda(ventanaComanda.ComandaTemporal);
                }
            }
        }
        private void OnClosed(object sender, EventArgs e)
        {
            Logica.SeleccionCambiada -= Logica_SeleccionCambiada;
        }
    }
}
