namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPedido
    {
        Todos = 0,
        Aberto = 1,
        Cancelado = 2,
        Finalizado = 3,
        AgAprovacao = 4,
        Rejeitado = 5,
        AutorizacaoPendente = 7,
        DesistenciaCarga = 8,
        DesistenciaCarregamento = 9,
        ColetaRealizada = 10,
        ColetaNaoRealizada = 11,
        EmCotacao = 12,
    }

    public static class SituacaoPedidoHelper
    {
        public static string ObterDescricao(this SituacaoPedido situacaoPedido)
        {
            switch (situacaoPedido)
            {
                case SituacaoPedido.Todos:
                    return "Todos";
                case SituacaoPedido.Aberto:
                    return "Aberto";
                case SituacaoPedido.Cancelado:
                    return "Cancelado";
                case SituacaoPedido.Finalizado:
                    return "Finalizado";
                case SituacaoPedido.AgAprovacao:
                    return "Ag. Aprovação";
                case SituacaoPedido.Rejeitado:
                    return "Rejeitado";
                case SituacaoPedido.AutorizacaoPendente:
                    return "Autorização Pendente";
                case SituacaoPedido.DesistenciaCarga:
                    return "Desistência da Carga";
                case SituacaoPedido.DesistenciaCarregamento:
                    return "Desitência de Carregamento";
                case SituacaoPedido.ColetaRealizada:
                    return "Coleta realizada";
                case SituacaoPedido.ColetaNaoRealizada:
                    return "Coleta não realizada";
                case SituacaoPedido.EmCotacao:
                    return "Em Cotação";
                default:
                    return string.Empty;
            }
        }
    }
}
