namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ParametroRateioFormula
    {
        todos = 0,
        peso = 1,
        distancia = 2,
        porNotaFiscal = 3,
        ValorMercadoria = 4,
        porCTe = 5,
        porValorCTe = 6, //formula exclusiva para subcontratação, não pode ser configurada.
        MetroCubico = 7,
        porFracionadaTonelada = 8,
        porFracionadaMetroCubico = 9,
        PesoLiquido = 10,
        Volume = 11,
        FatorPonderacaoDistanciaPeso = 12,
        PorPesoUtilizandoFatorCubagem = 13,
    }

    public static class ParametroRateioFormulaHelper
    {
        public static string ObterDescricao(this ParametroRateioFormula formula)
        {
            switch (formula)
            {
                case ParametroRateioFormula.todos: return "Todos";
                case ParametroRateioFormula.peso: return "Por peso";
                case ParametroRateioFormula.distancia: return "Por distância";
                case ParametroRateioFormula.porNotaFiscal: return "Por nota fiscal";
                case ParametroRateioFormula.ValorMercadoria: return "Por valor da mercadoria";
                case ParametroRateioFormula.porCTe: return "Por CT-e";
                case ParametroRateioFormula.porValorCTe: return "Por valor do CT-e";
                case ParametroRateioFormula.MetroCubico: return "Por metros cúbicos";
                case ParametroRateioFormula.porFracionadaTonelada: return "Por fracionada em toneladas";
                case ParametroRateioFormula.porFracionadaMetroCubico: return "Por fracionada em metros cúbicos";
                case ParametroRateioFormula.PesoLiquido: return "Por peso líquido";
                case ParametroRateioFormula.Volume: return "Por volume";
                case ParametroRateioFormula.FatorPonderacaoDistanciaPeso: return "Por fator de ponderação entre distância e peso";
                case ParametroRateioFormula.PorPesoUtilizandoFatorCubagem: return "Por peso utilizando fator de cubagem";
                default: return string.Empty;
            }
        }
    }
}
