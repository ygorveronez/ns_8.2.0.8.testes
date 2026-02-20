namespace Dominio.ObjetosDeValor.Embarcador.Imposto
{
    public class ParametroCalculoIBSCBS
    {
        public decimal BaseCalculo { get; set; }

        /// <summary>
        /// Valor utilizado para abater da base de cálculo do IBS/CBS após o desconto de PIS/COFINS.
        /// Usado para cénarios de NFSe com valor de ISS.
        /// </summary>
        public decimal ValoAbaterBaseCalculo { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoLocalidade { get; set; }

        public string SiglaUF { get; set; }

        public Dominio.Entidades.Empresa Empresa { get; set; }

        public int CodigoOutrasAliquotas { get; set; }

        public string OutrasAliquotasCST { get; set; }

        public bool NaoReduzirPisCofins { get; set; }
    }
}
