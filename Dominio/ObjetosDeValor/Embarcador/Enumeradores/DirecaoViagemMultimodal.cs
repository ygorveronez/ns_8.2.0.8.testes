using Dominio.Excecoes.Embarcador;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DirecaoViagemMultimodal
    {
        Todos = 0,
        Norte = 1,
        Sul = 2,
        Leste = 3,
        Oeste = 4
    }

    public static class DirecaoViagemMultimodalHelper
    {
        public static string ObterDescricao(this DirecaoViagemMultimodal direcaoViagemMultimodal)
        {
            switch (direcaoViagemMultimodal)
            {
                case DirecaoViagemMultimodal.Norte: return "N - Norte";
                case DirecaoViagemMultimodal.Sul: return "S - Sul";
                case DirecaoViagemMultimodal.Leste: return "E - Leste";
                case DirecaoViagemMultimodal.Oeste: return "W - Oeste";
                default: return string.Empty;
            }
        }
        public static string ObterAbreviacao(this DirecaoViagemMultimodal direcaoViagemMultimodal)
        {
            switch (direcaoViagemMultimodal)
            {
                case DirecaoViagemMultimodal.Norte: return "N";
                case DirecaoViagemMultimodal.Sul: return "S";
                case DirecaoViagemMultimodal.Leste: return "E";
                case DirecaoViagemMultimodal.Oeste: return "W";
                default: return string.Empty;
            }
        }

        public static DirecaoViagemMultimodal ConverterDoIngles(string direction) => direction?.ToUpper() switch
        {
            "NORTH" => DirecaoViagemMultimodal.Norte,
            "SOUTH" => DirecaoViagemMultimodal.Sul,
            "EAST" => DirecaoViagemMultimodal.Leste,
            "WEST" => DirecaoViagemMultimodal.Oeste,
            _ => throw new ServicoException($"Direção de viagem inválida: {direction}")
        };

        public static DirecaoViagemMultimodal ConverterDeOutroIdioma(string direction) => direction?.ToUpper() switch
        {
            var dir when dir == Localization.Resources.Enumeradores.Direcao.Norte => DirecaoViagemMultimodal.Norte,
            var dir when dir == Localization.Resources.Enumeradores.Direcao.Sul => DirecaoViagemMultimodal.Sul,
            var dir when dir == Localization.Resources.Enumeradores.Direcao.Leste => DirecaoViagemMultimodal.Leste,
            var dir when dir == Localization.Resources.Enumeradores.Direcao.Oeste => DirecaoViagemMultimodal.Oeste,
            _ => throw new ServicoException($"Direção de viagem inválida: {direction}")
        };

        public static string ConverterParaOutroIdioma(this DirecaoViagemMultimodal direcaoViagemMultimodal)
        {
            switch (direcaoViagemMultimodal)
            {
                case DirecaoViagemMultimodal.Norte: return Localization.Resources.Enumeradores.Direcao.Norte;
                case DirecaoViagemMultimodal.Sul: return Localization.Resources.Enumeradores.Direcao.Sul;
                case DirecaoViagemMultimodal.Leste: return Localization.Resources.Enumeradores.Direcao.Leste;
                case DirecaoViagemMultimodal.Oeste: return Localization.Resources.Enumeradores.Direcao.Oeste;
                default: return null;
            }
        }
    }
}