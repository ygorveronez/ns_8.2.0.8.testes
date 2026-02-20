namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTipoDetalhe
    {
        Todos = 0,
        ProcessamentoEspecial = 1,
        HorarioEntrega = 2,
        ZonaTransporte = 3,
        PeriodoEntrega = 4,
        DetalheEntrega = 5,
        TipoPallet = 6
    }

    public static class TipoTipoDetalheHelper
    {
        public static string ObterDescricao(this TipoTipoDetalhe tipo)
        {
            switch (tipo)
            {
                case TipoTipoDetalhe.ProcessamentoEspecial:
                    return "Processamento Especial";

                case TipoTipoDetalhe.HorarioEntrega:
                    return "Horário Entrega";

                case TipoTipoDetalhe.ZonaTransporte:
                    return "Zona de Transporte";

                case TipoTipoDetalhe.PeriodoEntrega:
                    return "Período de Entrega";

                case TipoTipoDetalhe.DetalheEntrega:
                    return "Detalhe de Entrega";

                case TipoTipoDetalhe.TipoPallet:
                    return "Tipo de Palete";

                default:
                    return string.Empty;
            }
        }
    }
}
