using System;

namespace Dominio.ObjetosDeValor.WebService.Devolucao
{
    public class FinalizarDevolucao
    {
        public string CodFilial { get; set; }

        public string ChaveNFD { get; set; }

        public DateTime? DataEntradaNF { get; set; }

        public string EscritorioVendas { get; set; }

        public string CodCliente { get; set; }

        public string CpfCnpjCliente { get; set; }

        public int QtdConv { get; set; }

        public int PesoBruto { get; set; }

        public string DtDevolucao { get; set; }

        public DateTime? DataDt { get; set; }

        public string TipoOrdem { get; set; }

        public string Ordem { get; set; }

        public DateTime? DataOrdem { get; set; }

        public string MotivoOrdem { get; set; }

        public string EquipeVendas { get; set; }

        public string GrupoPessoa { get; set; }

        public string Remessa { get; set; }

        public DateTime? DataPrevisaoEntrega { get; set; }

        public string DocumentoContabil { get; set; }

        public string DataDocumentoContabil { get; set; }

        public string Deposito { get; set; }

        public string ChaveNFRefencia { get; set; }
    }
}
