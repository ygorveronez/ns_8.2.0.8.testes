using System;

namespace Dominio.ObjetosDeValor.WebService.NFS.PreNFSe
{
    public class NFSe
    {
        public string CodigoIdentificacao { get; set; }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public DateTime DataEmissao { get; set; }

        public decimal AliquotaIss { get; set; }

        public decimal BaseCalculoIss { get; set; }

        public decimal PercentualRetencaoIss { get; set; }

        public decimal ValorIss { get; set; }

        public decimal ValorIssRetido { get; set; }

        public decimal ValorPis { get; set; }

        public decimal ValorCofins { get; set; }

        public decimal ValorIr { get; set; }

        public decimal ValorCsll { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorPrestacaoServico { get; set; }

        public decimal ValorReceber { get; set; }

        public string NumeroRps { get; set; }

        public string Observacao { get; set; }

        public string Xml { get; set; }

        public string Pdf { get; set; }
    }
}
