namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class DocumentoTransporte
    {
        public int Codigo { get; set; }
        public string CodigoFornecedor { get; set; }
        public string NumeroNFE { get; set; }
        public string NumeroCTE { get; set; }
        public string ChaveAcessoCTE { get; set; }
        public string ChaveAcessoNFE { get; set; }
        public string Peso { get; set; }
        public string Volumen { get; set; }
        public string Fornecedor { get; set; }
        public string Status { get; set; }
        public string Observacao { get; set; }
        public string DT_RowClass { get; set; }
    }
}
