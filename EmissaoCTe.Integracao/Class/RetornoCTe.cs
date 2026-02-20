namespace EmissaoCTe.Integracao
{
    public class RetornoCTe
    {
        public int CodigoCTe { get; set; }

        public int NumeroCTe { get; set; }

        public int SerieCTe { get; set; }

        public string DataEmissao { get; set; }

        public string ChaveCTe { get; set; }

        public string NumeroProtocolo { get; set; }

        public string NumeroRecibo { get; set; }

        public string CNPJEmpresa { get; set; }

        public string XML { get; set; }

        public string PDF { get; set; }

        public string PDFBIN { get; set; }

        public string CONEMB { get; set; }

        public string TXT { get; set; }

        public string NomeArquivo { get; set; }

        public Dominio.Enumeradores.TipoIntegracao Tipo { get; set; }

        public string Arquivo { get; set; }

        public string StatusCTe { get; set; }

        public string MensagemRetorno { get; set; }

        public Dominio.Enumeradores.TipoAmbiente Ambiente { get; set; }

        public Dominio.ObjetosDeValor.CTe.ImpostoICMS ICMS { get; set; }

        public string JustificativaCancelamento { get; set; }

        public int CodigoRetorno { get; set; }
    }
}