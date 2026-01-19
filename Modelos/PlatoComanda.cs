using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaFinalV2.Modelos
{
    public class PlatoComanda
    {

        public Plato PlatoPedido { get; set; }
        public int Cantidad { get; set; }

        public PlatoComanda(Plato plato)
        {
            PlatoPedido = plato;
            Cantidad = 1;
        }

    }
}
