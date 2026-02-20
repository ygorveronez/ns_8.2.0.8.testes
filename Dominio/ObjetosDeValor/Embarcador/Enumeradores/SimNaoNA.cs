namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SimNaoNA
    {
        Nao = 0,
        Sim = 1,
        NaoAplicavel = 2
    }
    public static class SimNaoNAHelper
    {
        public static string ObterDescricao(this SimNaoNA tipo)
        {
            switch (tipo)
            {
                case SimNaoNA.Nao: return Localization.Resources.Enumeradores.SimNao.Nao;
                case SimNaoNA.Sim: return Localization.Resources.Enumeradores.SimNao.Sim;
                case SimNaoNA.NaoAplicavel: return Localization.Resources.Enumeradores.SimNao.NaoAplicavel;
                default: return string.Empty;
            }
        }
    }
}
