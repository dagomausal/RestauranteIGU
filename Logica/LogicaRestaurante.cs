using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticaFinalV2.Modelos;

namespace PracticaFinalV2.Logica
{
    public class LogicaRestaurante
    {
        public Mesa MesaSeleccionada { get; private set; }
        public ObservableCollection<Mesa> ListaMesas { get; set; }
        public ObservableCollection<Plato> MenuDelDia { get; set; }
        public event EventHandler<MesaEventArgs> MesaAnadida;
        public event EventHandler<MesaEventArgs> SeleccionCambiada;

        public LogicaRestaurante()
        {
            ListaMesas = new ObservableCollection<Mesa>();
            MenuDelDia = new ObservableCollection<Plato>();
        }

        public void CargarDatosIniciales()
        {
            // --- Lista de Mesas ---
            Mesa m1 = new Mesa(1, 4, TipoMesa.Cuadrada, 50, 50);
            Mesa m2 = new Mesa(2, 4, TipoMesa.Cuadrada, 150, 50);
            Mesa m3 = new Mesa(3, 4, TipoMesa.Cuadrada, 250, 50);
            Mesa m4 = new Mesa(4, 6, TipoMesa.Circular, 50, 250);
            Mesa m5 = new Mesa(5, 6, TipoMesa.Circular, 250, 250);

            // --- Lista de Platos ---
            MenuDelDia.Add(new Plato("Ensalada Mixta", CategoriaPlato.Primero, "Lechuga, tomate, cebolla y atún"));
            MenuDelDia.Add(new Plato("Fabada Asturiana", CategoriaPlato.Primero, "Fabes, chorizo, morcilla y panceta"));
            MenuDelDia.Add(new Plato("Macarrones a la Boloñesa", CategoriaPlato.Primero, "Macarrones con salsa de carne y tomate"));

            MenuDelDia.Add(new Plato("Merluza a la Romana", CategoriaPlato.Segundo, "Filete de merluza rebozado con ensalada"));
            MenuDelDia.Add(new Plato("Entrecot de Ternera", CategoriaPlato.Segundo, "Entrecot a la plancha con patatas fritas"));
            MenuDelDia.Add(new Plato("Pollo al Ajillo", CategoriaPlato.Segundo, "Pollo cocinado con ajo y vino blanco"));

            MenuDelDia.Add(new Plato("Flan Casero", CategoriaPlato.Postre, "Flan de huevo con caramelo"));
            MenuDelDia.Add(new Plato("Tarta de Queso", CategoriaPlato.Postre, "Tarta de queso con base de galleta"));
            MenuDelDia.Add(new Plato("Fruta del Tiempo", CategoriaPlato.Postre, "Manzana, naranja, melón o kiwi"));

            // --- Simulación de estados ---
            m1.Reservar(2);
            AgregarMesa(m1);

            m2.Ocupar(3);
            m2.AgregarPlatoComanda(MenuDelDia[1]);
            m2.AgregarPlatoComanda(MenuDelDia[0]);
            m2.AgregarPlatoComanda(MenuDelDia[3]);
            m2.AgregarPlatoComanda(MenuDelDia[5]);
            m2.AgregarPlatoComanda(MenuDelDia[7]);
            AgregarMesa(m2);

            AgregarMesa(m3);

            m4.Ocupar(4);
            m4.AgregarPlatoComanda(MenuDelDia[2]);
            m4.AgregarPlatoComanda(MenuDelDia[1]);
            m4.AgregarPlatoComanda(MenuDelDia[5]);
            m4.AgregarPlatoComanda(MenuDelDia[8]);
            m4.Liberar();
            m4.Ocupar(5);
            m4.AgregarPlatoComanda(MenuDelDia[3]);
            m4.AgregarPlatoComanda(MenuDelDia[4]);
            m4.AgregarPlatoComanda(MenuDelDia[0]);
            m4.AgregarPlatoComanda(MenuDelDia[8]);
            m4.AgregarPlatoComanda(MenuDelDia[7]);
            AgregarMesa(m4);


            m5.Ocupar(4);
            m5.AgregarPlatoComanda(MenuDelDia[2]);
            m5.AgregarPlatoComanda(MenuDelDia[4]);
            m5.AgregarPlatoComanda(MenuDelDia[8]);
            m5.AgregarPlatoComanda(MenuDelDia[6]);
            AgregarMesa(m5);
        }

        public void AgregarMesa(Mesa nuevaMesa)
        {
            ListaMesas.Add(nuevaMesa);
            OnMesaAnadida(nuevaMesa);
        }

        private void OnMesaAnadida(Mesa mesa)
        {
            MesaAnadida?.Invoke(this, new MesaEventArgs(mesa));
        }

        public void SeleccionarMesa(Mesa mesa)
        {
            if (MesaSeleccionada != mesa)
            {
                MesaSeleccionada = mesa;
                OnSeleccionCambiada(mesa);
            }
        }
        private void OnSeleccionCambiada(Mesa mesa)
        {
            SeleccionCambiada?.Invoke(this, new MesaEventArgs(mesa));
        }
    }

    // --- Para pasar los datos como segundo Argumento ---
    public class MesaEventArgs : EventArgs
    {
        public Mesa MesaNueva { get; set; }
        public MesaEventArgs(Mesa mesa)
        {
            MesaNueva = mesa;
        }
    }
}
