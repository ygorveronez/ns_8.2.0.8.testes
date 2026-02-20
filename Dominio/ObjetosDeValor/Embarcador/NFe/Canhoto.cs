namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class Canhoto
    {
        public int Protocolo { get; set; }
        public string ChaveAcesso { get; set; }
        public string DataEnvioCanhoto { get; set; }
        public string DataEntregaNota { get; set; }
        public string Observacao { get; set; }
        public string ImagemCanhotoBase64 { get; set; }
        public string NomeImagemCanhoto { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto TipoCanhoto { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto SituacaoCanhoto { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto SituacaoDigitalizacaoCanhoto { get; set; }
        public int NumeroCanhoto { get; set; }
        public string Digitalizacao { get; set; }
        public string DataDigitalizacao { get; set; }
        public string DataEmissaoNotaFiscal { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string SerieNotaFiscal { get; set; }
        public string CnpjEmitenteNotaFiscal { get; set; }
        public string CodigoIntegracaoFilial { get; set; }
        public string CodigoIntegracaoDestinatario { get; set; }
        public string SituacaoNotaFiscal { get; set; }
        public string ChaveAcessoCte { get; set; }
        public int NumeroCanhotoAvulso { get; set; }
        public string Transportador { get; set; }
    }
}
