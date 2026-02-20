namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TituloRenegociado
    {
        Todos = 0,
        Sim = 1,
        Nao = 2,
    }

    public static class TituloRenegociadoHelper
    {
        public static string ObterDescricao(this TituloRenegociado status)
        {
            switch (status)
            {
                case TituloRenegociado.Todos: return "Todos";
                case TituloRenegociado.Sim: return "Sim";
                case TituloRenegociado.Nao: return "Nao";
                default: return string.Empty;
            }
        }
    }
}
