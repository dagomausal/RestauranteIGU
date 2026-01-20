using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PracticaFinalV2.Modelos
{
    public class Mesa : INotifyPropertyChanged
    {
        private int comensalesRespaldo;
        private EstadoMesa estadoRespaldo;

        public event EventHandler MesaActualizada;
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public int CapacidadMaxima { get; set; }
        public int ComensalesActuales 
        { 
            get { return comensalesRespaldo; }
            set
            {
                if (ComensalesActuales != value)
                {
                    comensalesRespaldo = value;
                    OnPropertyChanged("ComensalesActuales");
                    MesaActualizada?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public EstadoMesa Estado 
        { 
            get { return estadoRespaldo; }
            set
            {
                if(estadoRespaldo != value)
                {
                    estadoRespaldo = value;
                    OnPropertyChanged("Estado");
                    MesaActualizada?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public TipoMesa Forma { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double ancho
        {
            get
            {
                return Forma == TipoMesa.Circular ? 120 : 80;
            }
        }
        public double alto
        {
            get
            {
                return Forma == TipoMesa.Circular ? 80 : 110;
            }
        }
        public ObservableCollection<PlatoComanda> Comanda { get; set; }

        public Mesa(int id, int capacidadMaxima, TipoMesa forma, double x, double y)
        {
            Id = id;
            CapacidadMaxima = capacidadMaxima;
            ComensalesActuales = 0;
            Estado = EstadoMesa.Libre;
            Forma = forma;
            X = x;
            Y = y;

            Comanda = new ObservableCollection<PlatoComanda>();
        }

        public void Reservar(int comensales)
        {
            if (comensales > CapacidadMaxima) throw new Exception("El número de comensales excede la capacidad máxima de la mesa.");
            if (comensales <= 0) throw new Exception("El número de comensales debe ser mayor que cero.");
            ComensalesActuales = comensales;
            Estado = EstadoMesa.Reservada;
        }

        public void Ocupar(int comensales)
        {
            if (comensales > CapacidadMaxima) throw new Exception("El número de comensales excede la capacidad máxima de la mesa.");
            if (comensales <= 0) throw new Exception("El número de comensales debe ser mayor que cero.");
            ComensalesActuales = comensales;
            Estado = EstadoMesa.Ocupada;
        }

        public void Liberar()
        {
            Comanda.Clear();
            ComensalesActuales = 0;
            Estado = EstadoMesa.Libre;
        }

        private void ActualizarEstadoComanda()
        {
            if (this.Comanda.Count > 0)
            {
                if (Estado == EstadoMesa.Ocupada)
                {
                    Estado = EstadoMesa.OcupadaComanda;
                }
            }
            else
            {
                if (Estado == EstadoMesa.OcupadaComanda)
                {
                    Estado = EstadoMesa.Ocupada;
                }
            }
        }

        public void AgregarPlatoComanda(Plato plato)
        {
            
            PlatoComanda platoComanda = null;
            foreach(PlatoComanda pc in Comanda)
            {
                if (pc.PlatoPedido.Nombre == plato.Nombre)
                {
                    platoComanda = pc;
                    platoComanda.Cantidad++;
                    break;
                }
            }

            if (platoComanda == null)
            {
                platoComanda = new PlatoComanda(plato);
                Comanda.Add(platoComanda);
            }

            ActualizarEstadoComanda();
        }

        public void ConfirmarComanda(ObservableCollection<PlatoComanda> comandaConfirmada)
        {
            this.Comanda.Clear();

            foreach (PlatoComanda pc in comandaConfirmada)
            {
                this.Comanda.Add(pc);
            }

            ActualizarEstadoComanda();
        }

        protected void OnPropertyChanged(string nombrePropiedad)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
        }
    }
}
