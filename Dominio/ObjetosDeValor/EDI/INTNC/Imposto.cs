namespace Dominio.ObjetosDeValor.EDI.INTNC
{
    public class Imposto
    {
        public int IdNotaCobranca { get; set; }
        public int NumeroNotaCobranca { get; set; }
        public long SequencialUnico { get; set; }
        public string CodImposto { get; set; }
        public decimal ValorBaseImposto { get; set; }
        public decimal PercentualAliquitaImposto { get; set; }
        public decimal ValorImposto { get; set; }

        public decimal ValorDoTipoImposto { get; set; }
        public decimal PercentualAliquotaTipoImposto { get; set; }
        public decimal ValorBaseTipoImposto { get; set; }
        public string TipoTributacaoICMS { get; set; }
        public string TipoTributacao { get; set; }
        public decimal ValorBasePIS { get; set; }
        public decimal AliquotaPIS { get; set; }
        public decimal ValorPIS { get; set; }
        public decimal ValorBaseCOFINS { get; set; }
        public decimal AliquotaCOFINS { get; set; }
        public decimal ValorCOFINS { get; set; }
        public decimal ValorBaseParaCreditoICMS { get; set; }
        public decimal ValorBaseSemCreditoICMS { get; set; }
        public decimal ValorImpostoComCredito { get; set; }
        public decimal ValorImpostoSemCredito { get; set; }
        public decimal ValorApagarComCredito { get; set; }
        public decimal ValorApagarSemCredito { get; set; }

        // ===== IBS / CBS – Registro 720 =====

        // Base de cálculo IBS/CBS
        public decimal ValorBaseCalculoIBSCBS { get; set; }

        // CBS
        public decimal PercentualAliquotaCBS { get; set; }
        public decimal ValorCBS { get; set; }
        public string ImpostoEstatisticoCBS { get; set; }

        // IBS Estadual
        public decimal PercentualAliquotaIBSEstadual { get; set; }
        public decimal ValorIBSEstadual { get; set; }
        public string ImpostoEstatisticoIBSEstadual { get; set; }

        // IBS Municipal
        public decimal PercentualAliquotaIBSMunicipal { get; set; }
        public decimal ValorIBSMunicipal { get; set; }
        public string ImpostoEstatisticoIBSMunicipal { get; set; }

        // Classificação tributária
        public string CSTIBSCBS { get; set; }
        public string ClassificacaoTributariaIBSCBS { get; set; }
    }
}
