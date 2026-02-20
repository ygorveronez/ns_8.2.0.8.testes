using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoNotaFiscalIntegrada
    {
        Todos = 0,
        Faturamento = 1,
        RemessaPallet = 2,
        OrdemVenda = 3,
        RemessaVenda = 4
    }
}

public static class TipoNotaFiscalIntegradaHelper
{
    public static string ObterDescricao(this TipoNotaFiscalIntegrada tipoNotaFiscalIntegrada)
    {
        string retorno = "";
        switch (tipoNotaFiscalIntegrada)
        {
            case TipoNotaFiscalIntegrada.Faturamento:
                retorno = "Faturamento";
                break;
            case TipoNotaFiscalIntegrada.RemessaPallet:
                retorno = "Remessa Pallet";
                break;
            case TipoNotaFiscalIntegrada.OrdemVenda:
                retorno = "Ordem Venda";
                break;
            case TipoNotaFiscalIntegrada.RemessaVenda:
                retorno = "Remessa Venda";
                break;
        }
        return retorno;
    }
}
