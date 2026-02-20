namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAplicacaoColetaEntrega
    {
        Todos = 0,
        Entrega = 1,
        Coleta = 2
    }

    public static class TipoEventoColetaEntregaHelper
    {
        public static string ObterDescricao(this TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega)
        {
            switch (tipoAplicacaoColetaEntrega)
            {
                case TipoAplicacaoColetaEntrega.Todos: return "Todos";
                case TipoAplicacaoColetaEntrega.Entrega: return "Entrega";
                case TipoAplicacaoColetaEntrega.Coleta: return "Coleta";
                default: return "";
            }
        }
    }
}
