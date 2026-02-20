namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoToleranciaPesagem
    {
        Todos = 0,        
        Ativo = 1,
        Inativo = 2
    }

    public static class SituacaoToleranciaPesagemHelper
    {
        public static string ObterDescricao(this SituacaoToleranciaPesagem situacao)
        {
            switch (situacao)
            {
                case SituacaoToleranciaPesagem.Todos: return "Todos";
                case SituacaoToleranciaPesagem.Ativo: return "Ativo";
                case SituacaoToleranciaPesagem.Inativo: return "Inativo";
                default: return string.Empty;
            }
        }
    }
}
