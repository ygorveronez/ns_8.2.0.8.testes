namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoVisualizacaoCapacidadeJanela
    {
        Volume = 1,
        Cubagem = 2
    }

    public static class TipoVisualizacaoCapacidadeJanelaHelper
    {
        public static string ObterDescricao(this TipoVisualizacaoCapacidadeJanela origem)
        {
            switch (origem)
            {
                case TipoVisualizacaoCapacidadeJanela.Volume: return "Volume";
                case TipoVisualizacaoCapacidadeJanela.Cubagem: return "Cubagem";
                default: return string.Empty;
            }
        }
    }
}

