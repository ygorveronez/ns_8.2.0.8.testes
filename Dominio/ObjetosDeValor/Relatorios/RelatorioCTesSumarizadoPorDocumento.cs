using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCTesSumarizadoPorDocumento
    {
        public int CodigoCTe { get; set; }

        public DateTime? DataEmissaoCTe { get; set; }

        public int NumeroCTe { get; set; }

        public int SerieCTe { get; set; }

        public string NumeroDocumento { get; set; }

        public string CEPDestinatario { get; set; }

        public string CidadeDestinatario { get; set; }

        public string UFDestinatario { get; set; }

        public decimal PesoDocumento { get; set; }

        public decimal ValorDocumento { get; set; }

        public decimal PesoCTe { get; set; }

        public decimal PesoCTeTotal { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorAReceber { get; set; }

        public string Remetente { get; set; }

        public string Destinatario { get; set; }

        public string UFInicio { get; set; }

        public string UFFim { get; set; }

        public string Placa { get; set; }

        public decimal BaseCalculoICMS { get; set; }

        public decimal ValorMercadoria { get; set; }

        public string ObservacaoCTe { get; set; }

        public string Usuario { get; set; }

        public decimal ValorICMS { get; set; }

        public string Pagamento { get; set; }

        public decimal PesoUnidade { get; set; }
        public string UnidadeMedida { get; set; }

    }
}

