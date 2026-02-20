namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRoteirizacaoColetaEntrega
    {
        Entrega = 0,
        ColetaEntrega = 1,
        Coleta = 2
    }

    public static class TipoRoteirizacaoColetaEntregaHelper
    {
        public static string ObterDescricao(this TipoRoteirizacaoColetaEntrega tipo)
        {
            switch (tipo)
            {
                case TipoRoteirizacaoColetaEntrega.Entrega: return "Entrega";
                case TipoRoteirizacaoColetaEntrega.ColetaEntrega: return "Coleta e Entrega";
                case TipoRoteirizacaoColetaEntrega.Coleta: return "Coleta";
                default: return string.Empty;
            }
        }
    }
}
