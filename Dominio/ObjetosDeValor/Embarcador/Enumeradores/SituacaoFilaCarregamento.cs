namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFilaCarregamento
    {
        NaFila = 1,
        EmTransicao = 2,
        Removido = 3,
        EmViagem = 4,
        Disponivel = 5,
        EmRemocao = 6
    }

    public static class SituacaoFilaCarregamentoHelper
    {
        public static string ObterDescricao(this SituacaoFilaCarregamento situacaoFilaCarregamento)
        {
            switch (situacaoFilaCarregamento)
            {
                case SituacaoFilaCarregamento.NaFila: return "Na Fila";
                case SituacaoFilaCarregamento.EmTransicao: return "Em Transição";
                case SituacaoFilaCarregamento.Removido: return "Removido";
                case SituacaoFilaCarregamento.EmRemocao: return "Em Remoção";
                case SituacaoFilaCarregamento.EmViagem: return "Em Viagem";
                case SituacaoFilaCarregamento.Disponivel: return "Disponível";
                default: return string.Empty;
            }
        }
    }
}
