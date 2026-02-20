namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AprovacaoRejeicao
    {
        Rejeicao = 0,
        Aprovacao = 1
    }

    public static class AprovacaoRejeicaoHelper
    {
        public static string ObterDescricao(this AprovacaoRejeicao situacao)
        {
            switch (situacao)
            {
                case AprovacaoRejeicao.Aprovacao: return "Aprovação";
                case AprovacaoRejeicao.Rejeicao: return "Rejeição";
                default: return string.Empty;
            }
        }
    }
}
