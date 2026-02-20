using System;

namespace Dominio.ObjetosDeValor.Embarcador.Cotacoes
{
    public class FiltroPesquisaCotacaoFrete
    {
        public DateTime DataCotacaoInicial { get; set; }
        public DateTime DataCotacaoFinal { get; set; }
        public string NumeroPedido { get; set; }
        public int CodigoDestino { get; set; }
        public int CodigoTransportador { get; set; }
        public double CpfCnpjExpedidor { get; set; }
        public int NumeroCotacao { get; set; }
    }
}
