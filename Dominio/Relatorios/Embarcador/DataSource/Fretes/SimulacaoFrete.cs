using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class SimulacaoFrete
    {
        public int CodigoItemSimulacao { get; set; }
        public int CodigoCarga { get; set; }

        public string NumeroCarga { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public DateTime DataCriacaoCarga { get; set; }
        public string Transportador { get; set; }
        public string Veiculos { get; set; }
        public string Motoristas { get; set; }

        public decimal ValorFreteOriginal { get; set; }
        public decimal ValorFreteTotalOriginal { get; set; }
        public decimal ValorICMSOriginal { get; set; }
        public decimal ValorFreteAjuste { get; set; }
        public decimal ValorFreteTotalAjuste { get; set; }
        public decimal ValorICMSAjuste { get; set; }
    }
}
