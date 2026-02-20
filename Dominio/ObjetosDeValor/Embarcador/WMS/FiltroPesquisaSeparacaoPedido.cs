using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.WMS
{
    public class FiltroPesquisaSeparacaoPedido
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public SituacaoSeparacaoPedido? Situacao { get; set; }
        public int CodigoPedido { get; set; }
    }
}
