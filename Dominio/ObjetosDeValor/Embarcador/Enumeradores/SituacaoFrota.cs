namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFrota
    {
        Todos = 0,
        Disponivel = 1,
        EmViagem = 2,
        EmManutencao = 3,
        EmCarregamento = 4,
        EmDescarregamento = 5
    }

    public static class SituacaoFrotaHelper
    {
        public static string ObterDescricao(this SituacaoFrota situacaoFrota)
        {
            switch (situacaoFrota)
            {
                case SituacaoFrota.Disponivel: return "Disponível";
                case SituacaoFrota.EmViagem: return "Em Viagem";
                case SituacaoFrota.EmManutencao: return "Em Manutenção";
                case SituacaoFrota.EmCarregamento: return "Alocado";
                default: return string.Empty;
            }
        }
    }
}
