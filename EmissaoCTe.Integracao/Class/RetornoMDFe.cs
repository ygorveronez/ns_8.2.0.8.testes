namespace EmissaoCTe.Integracao
{
    public class RetornoMDFe
    {
        public int CodigoMDFe { get; set; }

        public int NumeroMDFe { get; set; }

        public int SerieMDFe { get; set; }

        public bool CargaPropria { get; set; }

        public string DataEmissao { get; set; }

        public string ChaveMDFe { get; set; }

        public string NumeroProtocolo { get; set; }

        public string NumeroRecibo { get; set; }

        public string CNPJEmpresa { get; set; }

        public string XML { get; set; }

        public string PDF { get; set; }

        public string TXT { get; set; }

        public string NomeArquivo { get; set; }

        public Dominio.Enumeradores.TipoIntegracaoMDFe Tipo { get; set; }

        public string Arquivo { get; set; }

        public Dominio.Enumeradores.StatusMDFe StatusMDFe { get; set; }

        public string MensagemRetorno { get; set; }

        public int? CodigoRetorno { get; set; }

        public string DataHoraRecibo { get; set; }
    }
}