namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoGestaoDadosColetaRetornoConfirmacao
    {
        Todos = -1,
        SemErro = 0,
        ComErro = 1
    }

    public static class SituacaoGestaoDadosColetaRetornoConfirmacaoHelper
    {
        public static string ObterDescricao(this SituacaoGestaoDadosColetaRetornoConfirmacao situacao)
        {
            switch (situacao)
            {
                case SituacaoGestaoDadosColetaRetornoConfirmacao.SemErro: return "Sem erro";
                case SituacaoGestaoDadosColetaRetornoConfirmacao.ComErro: return "Com erro";
                default: return string.Empty;
            }
        }
    }
}
