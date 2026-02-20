namespace EmissaoCTe.Integracao
{
    public class RetornoNFSe
    {
        public int CodigoNFSe { get; set; }

        public string CNPJEmitente { get; set; }
        
        public int NumeroNFSe { get; set; }

        public int SerieNFSe { get; set; }

        public string DataEmissao { get; set; }

        public string NumeroProtocolo { get; set; }

        public string CodigoVerificacao { get; set; }

        public string XML { get; set; }

        public string PDF { get; set; }

        public Dominio.Enumeradores.TipoIntegracaoNFSe Tipo { get; set; }

        public string NomeArquivo { get; set; }

        public string Arquivo { get; set; }

        public Dominio.Enumeradores.StatusNFSe StatusNFSe { get; set; }

        public string MensagemRetorno { get; set; }

        public string JustificativaCancelamento { get; set; }

        public Dominio.ObjetosDeValor.NFSe.ValoresNFSe ValoresNFSe { get; set; }

        public int NumeroRPS { get; set; }

        public string SerieRPS { get; set; }

        public Dominio.ObjetosDeValor.NFSe.Tomador Tomador { get; set; }

        public string OutrasInformacoes { get; set; }

        public string NumeroNFSePrefeitura { get; set; }
    }
}