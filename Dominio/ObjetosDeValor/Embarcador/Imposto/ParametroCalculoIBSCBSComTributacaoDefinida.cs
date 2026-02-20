namespace Dominio.ObjetosDeValor.Embarcador.Imposto
{
    public class ParametroCalculoIBSCBSComTributacaoDefinida
    {
        public string CST { get; set; }
        public string CodigoIndicadorOperacao { get; set; }
        public string NBS { get; set; }

        public string ClassificacaoTributaria { get; set; }

        public decimal BaseCalculo { get; set; }

        public decimal AliquotaIBSEstadual { get; set; }

        public decimal PercentualReducaoIBSEstadual { get; set; }

        public decimal AliquotaIBSMunicipal { get; set; }

        public decimal PercentualReducaoIBSMunicipal { get; set; }

        public decimal AliquotaCBS { get; set; }

        public decimal PercentualReducaoCBS { get; set; }
        
        public bool ZerarBaseCalculo { get; set; }
    }
}
