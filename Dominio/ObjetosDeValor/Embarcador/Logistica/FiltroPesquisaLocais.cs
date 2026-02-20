using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaLocais
    {
        public TipoLocal? TipoLocal { get; set; }
        public string Descricao { get; set; }
        public List<int> CodigosFiliais{ get; set; }
    }
}
