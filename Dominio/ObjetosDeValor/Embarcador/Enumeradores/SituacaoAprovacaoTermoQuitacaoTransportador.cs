namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAprovacaoTermoQuitacaoTransportador
    {
        Pendente = 1,
        Aprovado = 2,
        Reprovado = 3
    }

    public static class SituacaoAprovacaoTermoQuitacaoTransportadorHelper
    {
        public static string ObterDescricao(this SituacaoAprovacaoTermoQuitacaoTransportador situacao)
        {
            switch (situacao)
            {
                case SituacaoAprovacaoTermoQuitacaoTransportador.Pendente:
                    return "Pendente";
                case SituacaoAprovacaoTermoQuitacaoTransportador.Aprovado:
                    return "Aprovado";
                case SituacaoAprovacaoTermoQuitacaoTransportador.Reprovado:
                    return "Reprovado";
                default:
                    return "";
            }
        }
    }
}
