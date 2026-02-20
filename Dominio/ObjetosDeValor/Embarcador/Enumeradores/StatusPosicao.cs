namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusPosicao
    {
        Todos = -1,
        EmViagem = 1,
        SemViagem = 2
    }

    public static class StatusPosicaoHelper
    {
        public static string ObterDescricao(this StatusPosicao status)
        {
            switch (status)
            {
                case StatusPosicao.Todos: return "Todos";
                case StatusPosicao.SemViagem: return "Sem Viagem";
                case StatusPosicao.EmViagem: return "Em Viagem";
                

                default: return string.Empty;
            }
        }
    }
}
