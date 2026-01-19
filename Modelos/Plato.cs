using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaFinalV2.Modelos
{
    public class Plato
    {
        public string Nombre { get; set; }
        public CategoriaPlato Categoria { get; set; }
        public string Descripcion { get; set; }

        public Plato(string nombre, CategoriaPlato categoria, string descripcion)
        {
            Nombre = nombre;
            Categoria = categoria;
            Descripcion = descripcion;
        }
    }
}
