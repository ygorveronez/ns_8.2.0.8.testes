namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFluxoGestaoDevolucao
    {
        Normal = 1,
        Simples = 2,
    }

    public static class TipoFluxoGestaoDevolucaoHelper
    {
        public static string ObterDescricao(this TipoFluxoGestaoDevolucao TipoFluxoGestaoDevolucao)
        {
            switch (TipoFluxoGestaoDevolucao)
            {
                case TipoFluxoGestaoDevolucao.Normal: return "Normal";
                case TipoFluxoGestaoDevolucao.Simples: return "Simples";
                default: return string.Empty;
            }
        }
    }
}