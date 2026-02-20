namespace EmissaoCTe.Integracao
{
    public class RetornoCTeNFSe
    {
        public string Documento {get; set;}

        public int Codigo { get; set; }

        public string Status { get; set; }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public string DataEmissao { get; set; }

        public string ChaveCTe { get; set; }

        public string NumeroProtocolo { get; set; }

        public string CNPJEmpresa { get; set; }

        public string XML { get; set; }

        public string XMLCancelamento { get; set; }

        public string PDF { get; set; }

        public string MensagemRetorno { get; set; }

        public Dominio.Enumeradores.TipoAmbiente Ambiente { get; set; }

        public string JustificativaCancelamento { get; set; }

        public decimal AliquotaISS { get; set; }

        public decimal ValorISS { get; set; }

        public string ServicoCodigo { get; set; }

        public string NumeroRPS { get; set; }

        public string SerieRPS { get; set; }

        public string DataRPS { get; set; }

        public string NumeroNFSePrefeitura { get; set; }

        public Dominio.ObjetosDeValor.CTe.ImpostoICMS ICMS { get; set; }
    }
}