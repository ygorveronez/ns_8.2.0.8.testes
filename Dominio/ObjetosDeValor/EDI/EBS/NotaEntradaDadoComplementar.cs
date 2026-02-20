namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class NotaEntradaDadoComplementar
    {
        public string TipoRegistro { get; set; }
        public decimal ValorMercadoria { get; set; }
        public decimal Desconto { get; set; }
        public decimal Frete { get; set; }
        public decimal Despesa { get; set; }
        public decimal Seguro { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public string CNPJCPFTransportadora { get; set; }
        public string MeioTransporte { get; set; }
        public string Placa { get; set; }
        public decimal Volumes { get; set; }
        public string Especie { get; set; }
        public string ChaveNFe { get; set; }
        public string ICMSSTRetido { get; set; }
        public string InscricaoEstadualRemetente { get; set; }
        public decimal ValorICMSAntecipacaoParcial { get; set; }
        public decimal ValorDiferencialAliquota { get; set; }
        public decimal ValorDiferencialAliquotaCredito { get; set; }
        public decimal ValorDiferencialAliquotaDebito { get; set; }
        public decimal BaseICMSAntecipacaoParcial { get; set; }
        public decimal BaseICMSDiferencial { get; set; }
        public decimal ValorAntecipacaoParcialDebito { get; set; }
        public decimal ValorAntecipacaoParcialCredito { get; set; }
        public decimal ValorBaseICMSSTAntecipacao { get; set; }
        public decimal ValorICMSSTAntecipacao { get; set; }
        public decimal ValorICMSSTInternoAntecipacao { get; set; }
        public decimal ValorSTAntecipacaoICMSST { get; set; }
        public string ChaveCTeReferencia { get; set; }
        public string TipoImportacao { get; set; }
        public string TipoDocumentoImportacao { get; set; }
        public string NumeroDISiscomex { get; set; }
        public string NumeroATODrawback { get; set; }
        public decimal BasePISImportacao { get; set; }
        public decimal ValorPISImportacao { get; set; }
        public string DataPagamentoPISPASEP { get; set; }
        public decimal BaseCOFINSImportacao { get; set; }
        public decimal ValorCOFINSImportacao { get; set; }
        public string DataPagamentoCOFINS { get; set; }
        public decimal ValePedagio { get; set; }
        public string Brancos { get; set; }
        public string UsoEBS { get; set; }
        public string Sequencia { get; set; }
        public string ChaveCTeOrigem { get; set; }

    }
}
