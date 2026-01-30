using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaFinalV2.Modelos
{
    public class PlatoComanda : INotifyPropertyChanged
    {
        private int cantidadRespaldo;
        public Plato PlatoPedido { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public int Cantidad
        {
            get { return cantidadRespaldo; }
            set
            {
                if (cantidadRespaldo != value)
                {
                    cantidadRespaldo = value;
                    OnPropertyChanged("Cantidad");
                }
            }
        }

        public PlatoComanda(Plato plato)
        {
            PlatoPedido = plato;
            Cantidad = 1;
        }

        // Constructor de copia
        public PlatoComanda(PlatoComanda original)
        {
            this.PlatoPedido = original.PlatoPedido;
            this.Cantidad = original.Cantidad;
        }


        protected void OnPropertyChanged(string nombrePropiedad)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
        }
    }
}
