namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTerceiroConfiguracaoContratoFrete
    {
        PorPessoa = 1,
        PorTipoDeOperacao = 2,
        PorTipoDeTerceiro = 3
    }

    public static class TipoTerceiroConfiguracaoContratoFreteHelper
    {
        public static string ObterDescricao(this TipoTerceiroConfiguracaoContratoFrete situacao)
        {
            switch (situacao)
            {
                case TipoTerceiroConfiguracaoContratoFrete.PorPessoa: return "Por Pessoa (Transp. Terceiro)";
                case TipoTerceiroConfiguracaoContratoFrete.PorTipoDeOperacao: return "Por Tipo de Operação";
                case TipoTerceiroConfiguracaoContratoFrete.PorTipoDeTerceiro: return "Por Tipo de Terceiro";
                default: return string.Empty;
            }
        }
    }
}
