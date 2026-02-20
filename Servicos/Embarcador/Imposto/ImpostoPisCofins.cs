namespace Servicos.Embarcador.Imposto
{
    public class ImpostoPisCofins : ServicoBase
    {
        #region Construtores
        public ImpostoPisCofins() : base() { }
        #endregion

        #region Métodos Públicos

        public decimal ObterValoresPisCofins(Dominio.Entidades.Empresa empresa, decimal valorBase)
        {
            if (!(empresa?.Configuracao?.ReduzirPISCOFINSBaseCalculoIBSCBS ?? false))
                return 0;

            if (empresa.Configuracao.AliquotaPIS > 0 && empresa.Configuracao.AliquotaCOFINS > 0)
                return CalcularValorPis(empresa.Configuracao.AliquotaPIS.Value, valorBase) + CalcularValorCofins(empresa.Configuracao.AliquotaCOFINS.Value, valorBase);
            else if (empresa.EmpresaPai?.Configuracao != null && empresa.EmpresaPai.Configuracao.AliquotaPIS > 0 && empresa.EmpresaPai.Configuracao.AliquotaCOFINS > 0)
                return CalcularValorPis(empresa.EmpresaPai.Configuracao.AliquotaPIS.Value, valorBase) + CalcularValorCofins(empresa.EmpresaPai.Configuracao.AliquotaCOFINS.Value, valorBase);

            return 0;
        }

        public decimal ObterAliquotaPisConfis(Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS retornoRegraICMS, Dominio.Entidades.Empresa empresa)
        {
            if (retornoRegraICMS.IncluirPisCofinsBC && !retornoRegraICMS.NaoIncluirPisCofinsBCEmComplementos)
            {
                if (empresa?.Configuracao != null && empresa.Configuracao.AliquotaPIS > 0 && empresa.Configuracao.AliquotaCOFINS > 0)
                {
                    retornoRegraICMS.AliquotaPis = empresa.Configuracao.AliquotaPIS.Value;
                    retornoRegraICMS.AliquotaCofins = empresa.Configuracao.AliquotaCOFINS.Value;
                }
                else if (empresa?.EmpresaPai?.Configuracao != null && empresa.EmpresaPai.Configuracao.AliquotaPIS > 0 && empresa.EmpresaPai.Configuracao.AliquotaCOFINS > 0)
                {
                    retornoRegraICMS.AliquotaPis = empresa.EmpresaPai.Configuracao.AliquotaPIS.Value;
                    retornoRegraICMS.AliquotaCofins = empresa.EmpresaPai.Configuracao.AliquotaCOFINS.Value;
                }
            }
            return retornoRegraICMS.AliquotaPis + retornoRegraICMS.AliquotaCofins;
        }

        public decimal CalcularValorPis(decimal aliquotaPis, decimal valorBaseCalculoICMS)
            => aliquotaPis > 0 ? (valorBaseCalculoICMS * (aliquotaPis / 100)) : 0;

        public decimal CalcularValorCofins(decimal aliquotaCofins, decimal valorBaseCalculoICMS)
            => aliquotaCofins > 0 ? (valorBaseCalculoICMS * (aliquotaCofins / 100)) : 0;

        #endregion
    }
}
