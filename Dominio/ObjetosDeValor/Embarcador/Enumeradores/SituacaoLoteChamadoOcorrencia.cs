namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLoteChamadoOcorrencia
    {
        EmEdicao = 1,
        AgAprovacao = 2,
        Aprovado = 3,
        Rejeitado = 4
    }

    public static class SituacaoLoteChamadoOcorrenciaHelper
    {
        public static string ObterDescricao(this SituacaoLoteChamadoOcorrencia o)
        {
            switch (o)
            {
                case SituacaoLoteChamadoOcorrencia.EmEdicao: return "Em Edição";
                case SituacaoLoteChamadoOcorrencia.AgAprovacao: return "Ag. Aprovação";
                case SituacaoLoteChamadoOcorrencia.Aprovado: return "Aprovado";
                case SituacaoLoteChamadoOcorrencia.Rejeitado: return "Rejeitado";
                default: return string.Empty;
            }
        }
    }
}
