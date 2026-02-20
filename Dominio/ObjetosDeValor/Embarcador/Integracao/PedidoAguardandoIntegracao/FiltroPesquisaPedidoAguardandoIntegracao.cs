using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.PedidoAguardandoIntegracao
{
    public sealed class FiltroPesquisaPedidoAguardandoIntegracao
    {
        public string Pesquisa { get; set; }
        public SituacaoPedidoAguardandoIntegracao? SituacaoIntegracao { get; set; }
        public TipoIntegracao? TipoIntegracao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public DateTime DataEmbarqueInicio { get; set; }
        public DateTime DataEmbarqueFim { get; set; }
    }
}
