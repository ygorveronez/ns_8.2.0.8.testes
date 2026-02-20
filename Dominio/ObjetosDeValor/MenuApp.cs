using System;

namespace Dominio.ObjetosDeValor
{
    public class MenuApp : IEquatable<MenuApp>
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public MenuApp MenuPai { get; set; }

        public bool Equals(MenuApp other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Codigo;
        }
    }
}
