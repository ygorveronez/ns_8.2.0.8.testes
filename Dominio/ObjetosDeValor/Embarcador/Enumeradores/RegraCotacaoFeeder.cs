namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RegraCotacaoFeeder
    {
        Todos = 0,
        Nenhuma = 1,
        TaxaDoDia = 2,
        TaxaDoDiaUtilDoETS = 3,
        TaxaDoDiaUtilAnterior = 4
    }

    public static class RegraCotacaoFeederHelper
    {
        public static string ObterDescricao(this RegraCotacaoFeeder diaSemana)
        {
            switch (diaSemana)
            {
                case RegraCotacaoFeeder.Nenhuma: return "Não utilizar regra";
                case RegraCotacaoFeeder.TaxaDoDia: return "Utilizar a taxa do dia";
                case RegraCotacaoFeeder.TaxaDoDiaUtilDoETS: return "Utilizar a taxa do dia (últil) do ETS do porto de origem";
                case RegraCotacaoFeeder.TaxaDoDiaUtilAnterior: return "Utilizar a taxa do dia (útil) anterior ao da emissão do CT-e";
                default: return string.Empty;
            }
        }

    }
}
