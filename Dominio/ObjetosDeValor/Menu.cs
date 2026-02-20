using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class Menu
    {
        public string Descricao { get; set; }

        public string Formulario { get; set; }

        public string Icone { get; set; }

        public bool SubMenu { get; set; }

        public int CodigoMenu { get; set; }

        public List<Menu> Itens { get; set; }
    }
}
