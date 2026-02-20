using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class FiltroPesquisaBloqueioEmissaoPorNaoConformidade
    {
        public string Descricao { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public int CodigoTipoNaoConformidade { get; set; }
        public bool? Situacao { get; set; }
    }
}

