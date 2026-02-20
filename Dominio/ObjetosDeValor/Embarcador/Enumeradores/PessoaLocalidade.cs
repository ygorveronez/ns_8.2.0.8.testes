namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PessoaLocalidade
    {
        Pessoa = 1,
        Localidade = 2,
    }

    public static class PessoaLocalidadeHelper
    {
        public static string ObterDescricao(this PessoaLocalidade pessoaLocalidade)
        {
            switch (pessoaLocalidade)
            {
                case PessoaLocalidade.Pessoa: return "Pessoa";
                case PessoaLocalidade.Localidade: return "Localidade";
                default: return string.Empty;
            }
        }
    }
}
