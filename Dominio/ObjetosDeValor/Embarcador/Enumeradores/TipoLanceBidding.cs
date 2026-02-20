namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoLanceBidding
    {
        LancePorEquipamento = 1,
        LanceFrotaFixaKmRodado = 2,
        LancePorcentagemNota = 3,
        LanceViagemAdicional = 4,
        LancePorPeso = 5,
        LancePorCapacidade = 6,
        LancePorFreteViagem = 7,
        LanceFrotaFixaFranquia = 8,
        LancePorViagemEntregaAjudante = 9
    }

    public static class TipoLanceBiddingHelper
    {
        public static string ObterDescricao(this TipoLanceBidding tipo)
        {
            switch (tipo)
            {
                case TipoLanceBidding.LancePorEquipamento: return "Lance por Equipamento";
                case TipoLanceBidding.LanceFrotaFixaKmRodado: return "Lance por Frota Fixa + KM Rodado";
                case TipoLanceBidding.LancePorcentagemNota: return "Lance por % sobre a Nota";
                case TipoLanceBidding.LanceViagemAdicional: return "Lance por Carga + Adicional por Entrega";
                case TipoLanceBidding.LancePorPeso: return "Lance por Peso";
                case TipoLanceBidding.LancePorCapacidade: return "Lance por Capacidade";
                case TipoLanceBidding.LancePorFreteViagem: return "Lance por Carga";
                case TipoLanceBidding.LanceFrotaFixaFranquia: return "Lance por Frota Fixa com Franquia de KM";
                case TipoLanceBidding.LancePorViagemEntregaAjudante: return "Lance por Carga + Adic. Entrega + Adic. Ajudante";
                default: return string.Empty;
            }
        }
    }
}