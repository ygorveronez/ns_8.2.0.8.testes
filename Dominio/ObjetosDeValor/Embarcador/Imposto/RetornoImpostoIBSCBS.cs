namespace Dominio.ObjetosDeValor.Embarcador.Imposto
{
    public class RetornoImpostoIBSCBS
    {
        public int CodigoOutraAliquota { get; set; }

        public string CST { get; set; }
        public string NBS { get; set; }
        public string CodigoIndicadorOperacao { get; set; } 

        public string ClassificacaoTributaria { get; set; }

        public decimal BaseCalculo { get; set; }

        public decimal AliquotaIBSEstadual { get; set; }

        public decimal PercentualReducaoIBSEstadual { get; set; }

        public decimal ValorIBSEstadual { get; set; }

        public decimal AliquotaIBSMunicipal { get; set; }

        public decimal PercentualReducaoIBSMunicipal { get; set; }

        public decimal ValorIBSMunicipal { get; set; }

        public decimal AliquotaCBS { get; set; }

        public decimal PercentualReducaoCBS { get; set; }

        public decimal ValorCBS { get; set; }
    }
}
