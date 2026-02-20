namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GeracaoGestaoDevolucao
    {
        Manual = 1,
        Automatica = 2,
        Atendimento = 3,
    }

    public static class GeracaoGestaoDevolucaoHelper
    {
        public static string ObterDescricao(this GeracaoGestaoDevolucao GeracaoGestaoDevolucao)
        {
            switch (GeracaoGestaoDevolucao)
            {
                case GeracaoGestaoDevolucao.Manual: return "Manual";
                case GeracaoGestaoDevolucao.Automatica: return "Automática";
                case GeracaoGestaoDevolucao.Atendimento: return "Atendimento";
                default: return "-";
            }
        }
    }
}