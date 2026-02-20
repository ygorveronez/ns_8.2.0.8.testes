namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SimNao
    {
        Nao = 0,
        Sim = 1,
        Todos = 2
    }
    public static class SimNaoHelper
    {
        public static string ObterDescricao(this SimNao tipo)
        {
            switch (tipo)
            {
                case SimNao.Nao: return Localization.Resources.Enumeradores.SimNao.Nao;
                case SimNao.Sim: return Localization.Resources.Enumeradores.SimNao.Sim;
                default: return string.Empty;
            }
        }
    }
}
