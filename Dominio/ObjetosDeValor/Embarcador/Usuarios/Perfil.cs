using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Usuarios
{
    public class Perfil
    {
        public int Codigo { get; set; }

        public string CodigoIntegracao { get; set; }

        public string Descricao { get; set; }

        public List<PerfilPagina> Paginas { get; set; }

        public string Sistema { get; set; }
    }
}
