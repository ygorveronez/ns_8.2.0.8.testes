namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEscalaVeiculo
    {
        EmEscala = 1,
        Suspenso = 2
    }

    public static class SituacaoEscalaVeiculoHelper
    {
        public static string ObterDescricao(this SituacaoEscalaVeiculo situacao)
        {
            switch (situacao)
            {
                case SituacaoEscalaVeiculo.EmEscala: return "Em Escala";
                case SituacaoEscalaVeiculo.Suspenso: return "Suspenso";
                default: return string.Empty;
            }
        }
    }
}
