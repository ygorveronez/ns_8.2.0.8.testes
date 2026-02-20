namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumTipoConsolidacao
    {
        NaoConsolida = 0,
        AutorizacaoEmissao = 1,
        PreCheckIn = 2,
    }

    public static class EnumTipoConsolidacaoHelper
    {
        public static string ObterDescricao(this EnumTipoConsolidacao tipoConsolidacao)
        {
            switch (tipoConsolidacao)
            {
                case EnumTipoConsolidacao.NaoConsolida: return "Não Consolida";
                case EnumTipoConsolidacao.AutorizacaoEmissao: return "Autorizaçã Emissão";
                case EnumTipoConsolidacao.PreCheckIn: return "Pré CheckIn";
                default: return "Não Consolida";
            }
        }

    }
}
