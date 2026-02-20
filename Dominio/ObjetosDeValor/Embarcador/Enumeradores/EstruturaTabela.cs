namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EstruturaTabela
    {
        CustoFixo = 0,
        TarifaFlat = 1,
        TarifasCustoUnidade = 2,

    }

    public static class EstruturaTabelaHelper
    {
        public static string ObterDescricao(this EstruturaTabela estruturaTabela)
        {
            switch (estruturaTabela)
            {
                case EstruturaTabela.CustoFixo: return "Custo fixo";
                case EstruturaTabela.TarifaFlat: return "Tarifa flat";
                case EstruturaTabela.TarifasCustoUnidade: return "Tarifas - Custo por unidade";
                default: return string.Empty;
            }
        }
    }
}