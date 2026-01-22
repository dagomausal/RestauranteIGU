using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaFinalV2.Modelos
{
    public enum EstadoMesa
    {
        Libre, 
        Reservada,
        Ocupada,
        OcupadaComanda
    }

    public enum CategoriaPlato
    {
        Primero,
        Segundo,
        Postre
    }

    public enum TipoMesa
    {
        Rectangular,
        Circular
    }
}
