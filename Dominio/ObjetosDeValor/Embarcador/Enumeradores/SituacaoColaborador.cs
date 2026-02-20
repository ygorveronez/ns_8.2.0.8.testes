namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoColaborador
    {
        Todos = 0,
        Afastado = 1,
        Atestado = 2,
        Ferias = 3,
        Folga = 4,
        Suspenso = 5,
        Trabalhando = 6,
        DSR = 7,
        EmTreinamento = 8
    }

    public static class SituacaoColaboradorHelper
    {
        public static string ObterDescricao(this SituacaoColaborador situacao)
        {
            switch (situacao)
            {
                case SituacaoColaborador.Afastado: return "Afastado";
                case SituacaoColaborador.Atestado: return "Atestado";
                case SituacaoColaborador.Ferias: return "Férias";
                case SituacaoColaborador.Folga: return "Folga";
                case SituacaoColaborador.Suspenso: return "Suspenso";
                case SituacaoColaborador.Trabalhando: return "Trabalhando";
                case SituacaoColaborador.DSR: return "DSR";
                case SituacaoColaborador.EmTreinamento: return "Em Treinamento";
                default: return string.Empty;
            }
        }
    }
}
