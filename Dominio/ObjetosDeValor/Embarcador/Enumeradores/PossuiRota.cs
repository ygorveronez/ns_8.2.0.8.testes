namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PossuiRota
    {
        Todos = 0,
        Sim = 1,
        Nao = 2
    }

    public static class PossuiRotaHelper
    {
        public static string ObterDescricao(this PossuiRota situacao)
        {
            switch (situacao)
            {
                case PossuiRota.Sim: return "Possui Rota";
                case PossuiRota.Nao: return "Não Possui Rota";
                default: return string.Empty;
            }
        }
    }
}
