namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoValePedagio
    {
        Pendete = 0,
        Comprada = 1,
        Confirmada = 2,
        RotaGerada = 3,
        RotaSemCusto = 4,
        AguardandoCadastroRota = 5,
        Reembolso = 6,
        Recusada = 7,
        Cancelada = 8,
        Encerrada = 9,
        EmCancelamento = 10
    }

    public static class SituacaoValePedagioHelper
    {
        public static string ObterDescricao(this SituacaoValePedagio situacaoValePedagio)
        {
            switch (situacaoValePedagio)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Pendete:
                    return "Pendente";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada:
                    return "Comprada";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada:
                    return "Confirmada";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaGerada:
                    return "Rota gerada";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaSemCusto:
                    return "Rota sem custo/valor";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.AguardandoCadastroRota:
                    return "Aguardando cadastro rota";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Recusada:
                    return "Recusada";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada:
                    return "Cancelada";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Encerrada:
                    return "Encerrada";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.EmCancelamento:
                    return "Em Cancelamento";
                default:
                    return string.Empty;
            }
        }
    }

}
