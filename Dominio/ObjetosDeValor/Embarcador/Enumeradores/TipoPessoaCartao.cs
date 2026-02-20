namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPessoaCartao
    {
        Fisica = 1,
        Juridica = 2,
    }

    public static class TipoPessoaCartaoHelper
    {
        public static string ObterDescricao(this TipoPessoaCartao tipoPessoaCartao)
        {
            switch (tipoPessoaCartao)
            {
                case TipoPessoaCartao.Fisica: return Localization.Resources.Enumeradores.TipoPessoa.Fisica;
                case TipoPessoaCartao.Juridica: return Localization.Resources.Enumeradores.TipoPessoa.Juridica;
                default: return string.Empty;
            }
        }
    }
}
