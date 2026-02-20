namespace Dominio.ObjetosDeValor.WebService.EmissaoNFe
{
    public class EmissaoNFe
    {
        public int CodigoNFe { get; set; }
        public string CNPJEmpresa { get; set; }
        public string XMLNaoAssinado { get; set; }
        public string ReciboAnterior { get; set; }
        public string CodigoStatusAnterior { get; set; }
        public string XMLDistribuicao { get; set; }
        public int TipoAmbiente { get; set; }
        public int UFEmpresa { get; set; }
        public string CIdToken { get; set; }
        public string Csc { get; set; }
        public string Modelo { get; set; }
    }
}
