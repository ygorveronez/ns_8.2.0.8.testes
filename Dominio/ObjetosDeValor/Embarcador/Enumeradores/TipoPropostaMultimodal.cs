namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPropostaMultimodal
    {
        Nenhum = 0,
        CargaFechada = 1,
        CargaFracionada = 2,
        Feeder = 3,
        VAS = 4,
        TakePayFeeder = 5,
        TAkePayCabotagem = 6,
        NoShowCabotagem = 7,
        FaturamentoContabilidade = 8,
        DemurrageCabotagem = 9,
        DetentionCabotagem = 10,
        NotaDebito = 11
    }

    public static class TipoPropostaMultimodalHelper
    {
        public static string ObterDescricao(this TipoPropostaMultimodal tipo)
        {
            switch (tipo)
            {
                case TipoPropostaMultimodal.Nenhum: return "Nenhum";
                case TipoPropostaMultimodal.CargaFechada: return "93 - Carga Fechada";
                case TipoPropostaMultimodal.CargaFracionada: return "94 - Carga Fracionada";
                case TipoPropostaMultimodal.Feeder: return "95 - Feeder";
                case TipoPropostaMultimodal.VAS: return "96 - VAS";
                case TipoPropostaMultimodal.TakePayFeeder: return "Embarque Certo - Feeder";
                case TipoPropostaMultimodal.TAkePayCabotagem: return "Embarque Certo - Cabotagem";
                case TipoPropostaMultimodal.NoShowCabotagem: return "No Show - Cabotagem";
                case TipoPropostaMultimodal.FaturamentoContabilidade: return "Faturamento - Contabilidade";
                case TipoPropostaMultimodal.DemurrageCabotagem: return "Demurrage - Cabotagem";
                case TipoPropostaMultimodal.DetentionCabotagem: return "Detention - Cabotagem";
                case TipoPropostaMultimodal.NotaDebito: return "Nota de Débito";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoNoCode(this TipoPropostaMultimodal tipo)
        {
            switch (tipo)
            {
                case TipoPropostaMultimodal.Nenhum: return "Nenhum";
                case TipoPropostaMultimodal.CargaFechada: return "Carga Fechada";
                case TipoPropostaMultimodal.CargaFracionada: return "Carga Fracionada";
                case TipoPropostaMultimodal.Feeder: return "Feeder";
                case TipoPropostaMultimodal.VAS: return "VAS";
                case TipoPropostaMultimodal.TakePayFeeder: return "Embarque Certo - Feeder";
                case TipoPropostaMultimodal.TAkePayCabotagem: return "Embarque Certo - Cabotagem";
                case TipoPropostaMultimodal.NoShowCabotagem: return "No Show - Cabotagem";
                case TipoPropostaMultimodal.FaturamentoContabilidade: return "Faturamento - Contabilidade";
                case TipoPropostaMultimodal.DemurrageCabotagem: return "Demurrage - Cabotagem";
                case TipoPropostaMultimodal.DetentionCabotagem: return "Detention - Cabotagem";
                case TipoPropostaMultimodal.NotaDebito: return "Nota de Débito";
                default: return string.Empty;
            }
        }
    }
}
