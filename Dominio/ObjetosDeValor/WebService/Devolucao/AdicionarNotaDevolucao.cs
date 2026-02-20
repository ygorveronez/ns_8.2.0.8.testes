using System;

namespace Dominio.ObjetosDeValor.WebService.Devolucao
{
    public class AdicionarNotaDevolucao
    {
        public string Status { get; set; }

        public string DocVendas { get; set; }

        public string Fornecimento { get; set; }

        public string DocFaturamento { get; set; }

        public DateTime? DataDocumento { get; set; }

        public string ChaveNFeReferencia { get; set; }

        public string ChaveNFD { get; set; }

        public string UsuarioModificouOrdem { get; set; }

        public string CpfCnpjCliente { get; set; }

        public string CodCliente { get; set; }

        public string XmlNFD { get; set; }

        public string CFOP { get; set; }
    }
}
