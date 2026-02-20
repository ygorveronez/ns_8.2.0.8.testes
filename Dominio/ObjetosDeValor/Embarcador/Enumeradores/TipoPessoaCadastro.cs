namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPessoaCadastro
    {
        Todas = 99,
        Fisica = 0,
        Juridica = 1,
        Exterior = 2
    }

    public static class TipoPessoaCadastroHelper
    {
        public static string ObterDescricao(this TipoPessoaCadastro tipoPessoaCadastro)
        {
            switch (tipoPessoaCadastro)
            {
                case TipoPessoaCadastro.Fisica: return Localization.Resources.Enumeradores.TipoPessoa.Fisica;
                case TipoPessoaCadastro.Juridica: return Localization.Resources.Enumeradores.TipoPessoa.Juridica;
                case TipoPessoaCadastro.Exterior: return Localization.Resources.Enumeradores.TipoPessoa.Exterior;
                default: return string.Empty;
            }
        }
    }
}
