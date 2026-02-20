namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class ConsultaNotaSefaz
    {
        public string Chave { get; set; }
        public string CNPJ { get; set; }
        public MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TCodUfIBGE CodigoIBGE { get; set; }
        public string NomeCertificado { get; set; }
        public string SenhaCertificado { get; set; }
        public string NomeCertificadoKeyVault { get; set; }
        public int TipoAmbiente { get; set; }
    }
}
