namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEscala
    {
        EmCriacao = 1,
        AgVeiculos = 2,
        Finalizada = 3,
        Cancelada = 4
    }

    public static class SituacaoEscalaHelper
    {
        public static string ObterDescricao(this SituacaoEscala situacao)
        {
            switch (situacao)
            {
                case SituacaoEscala.AgVeiculos: return "Ag. Veículos";
                case SituacaoEscala.Cancelada: return "Cancelada";
                case SituacaoEscala.EmCriacao: return "Em Criação";
                case SituacaoEscala.Finalizada: return "Finalizada";
                default: return string.Empty;
            }
        }
    }
}
