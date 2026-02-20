using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Filial
{
    public class FiltroPesquisaFilialArmazem
    {
        public string CodigoIntegracao { get; set; }
        public int CodigoFilial { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public string Descricao { get; set; }
    }
}
