namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPessoa
    {
        Pessoa = 1,
        GrupoPessoa = 2
    }

    public static class TipoPessoaHelper
    {
        public static string ObterDescricao(this TipoPessoa tipo)
        {
            switch (tipo)
            {
                case TipoPessoa.Pessoa: return "Pessoa";
                case TipoPessoa.GrupoPessoa: return "Grupo de Pessoa";
                default: return string.Empty;
            }
        }
    }
}
