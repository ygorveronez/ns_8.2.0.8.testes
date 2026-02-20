namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOrdemServicoVenda
    {
        OrdemServicoInterna = 1,
        CentroDeCusto = 2
    }

    public static class TipoOrdemServicoVendaHelper
    {
        public static string ObterDescricao(this TipoOrdemServicoVenda status)
        {
            switch (status)
            {
                case TipoOrdemServicoVenda.OrdemServicoInterna: return "OI - Ordem Serviço Interna";
                case TipoOrdemServicoVenda.CentroDeCusto: return "CC - Centro de Custo";
                default: return string.Empty;
            }
        }
    }
}
