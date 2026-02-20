using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaAuditoriaEMP
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string Topic { get; set; }
        public string Schedule { get; set; }
        public string Booking { get; set; }
        public string Customer { get; set; }
        public SimNao Justificativa { get; set; }
        public SituacaoIntegracaoEMP? Status { get; set; }
        public TipoIntegracaoEMP? TipoIntegracao { get; set; }
        public string Fatura { get; set; }
        public string Boleto { get; set; }
    }
}
