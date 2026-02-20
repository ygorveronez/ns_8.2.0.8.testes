namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class NotaEntradaEmitenteDestinatario
    {
        public string TipoRegistro { get; set; }
        public string CNPJCPF { get; set; }
        public string Razao { get; set; }
        public string Fantasia { get; set; }
        public string Estado { get; set; }
        public string Inscricao { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string CEP { get; set; }
        public string Municipio { get; set; }
        public string DDD { get; set; }
        public string Fone { get; set; }
        public string ContaCliente { get; set; }
        public string HistoricoCliente { get; set; }
        public string ContaFornecedor { get; set; }
        public string HistoricoFornecedor { get; set; }
        public string Produtor { get; set; }
        public string IdentificacaoExterior { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string SUFRAMA { get; set; }
        public string CodigoPais { get; set; }
        public string NaturezaJuridica { get; set; }
        public string MunicipioIBGE { get; set; }
        public string Brancos { get; set; }
        public string UsoEBS { get; set; }
        public string Sequencial { get; set; }
        public NotaEntradaNotaFiscal NotaFiscal { get; set; }

    }
}
