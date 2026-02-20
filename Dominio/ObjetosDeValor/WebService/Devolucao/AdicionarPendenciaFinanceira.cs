using System;

namespace Dominio.ObjetosDeValor.WebService.Devolucao
{
    public class AdicionarPendenciaFinanceira
    {
        public string CodCliente { get; set; }

        public bool PossuiBloqueioFinanceiro { get; set; }

        public DateTime DataDocumento { get; set; }

        public string TipoDocumento { get; set; }

        public string NumeroDocumento { get; set; }

        public string Item { get; set; }

        public string Referencia { get; set; }

        public bool Perda { get; set; }

        public DateTime? DataVencimento { get; set; }

        public int DiasAtraso { get; set; }

        public DateTime? DataEntrega { get; set; }

        public decimal Valor { get; set; }

        public string CondicaoPagamento { get; set; }

        public string IdentificadorCaso { get; set; }

        public string Motivo { get; set; }

        public string ReferenciaFatura { get; set; }

        public string ObservacoesGerais { get; set; }

        public string ObservacoesDocumento { get; set; }

        public string ObservacoesItem { get; set; }

        public string EquipeVendas { get; set; }

        public string EscritorioVendas { get; set; }

        public string ChaveNFReferencia { get; set; }
    }
}
