namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CheckListOpcaoRelacaoCampo
    {
        LoteInternoUm = 1,
        LoteInternoDois = 2,
        PesoLiquidoPosPerdas = 3,
        ResultadoRendimentoLaranja = 4
    }

    public static class CheckListOpcaoRelacaoCampoHelper
    {
        public static string ObterDescricao(this CheckListOpcaoRelacaoCampo relacao)
        {
            switch (relacao)
            {
                case CheckListOpcaoRelacaoCampo.LoteInternoUm: return "Lote interno um";
                case CheckListOpcaoRelacaoCampo.LoteInternoDois: return "Lote interno dois";
                case CheckListOpcaoRelacaoCampo.PesoLiquidoPosPerdas: return "Peso líquido pós perdas KGs";
                case CheckListOpcaoRelacaoCampo.ResultadoRendimentoLaranja: return "Resultado Rendimento Laranja";
                default: return string.Empty;
            }
        }
    }
}
