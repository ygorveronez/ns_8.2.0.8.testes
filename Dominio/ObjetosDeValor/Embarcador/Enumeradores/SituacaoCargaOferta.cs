namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaOferta
    {
        PendenteDeOferta = 1,
        EmOferta = 2,
        EmConfirmacao = 3,
        PrazoExpirado = 4,
        Confirmada = 5,
        Cancelada = 6,
    }

    public static class SituacaoCargaOfertaHelper
    {
        public static string ObterDescricao(this SituacaoCargaOferta situacaoCargaOferta)
        {
            switch (situacaoCargaOferta)
            {
                case SituacaoCargaOferta.PendenteDeOferta:
                    return "Pendente de oferta";
                case SituacaoCargaOferta.EmOferta:
                    return "Em oferta";
                case SituacaoCargaOferta.EmConfirmacao:
                    return "Em confirmação";
                case SituacaoCargaOferta.PrazoExpirado:
                    return "Prazo expirado";
                case SituacaoCargaOferta.Confirmada:
                    return "Confirmada";
                case SituacaoCargaOferta.Cancelada:
                    return "Cancelada";
                default:
                    return string.Empty;
            }
        }

        public static SituacaoCargaOferta ObterSituacaoCargaOfertaPorTipoIntegracaoCargaOferta(TipoIntegracaoOfertaCarga tipoIntegracaoOfertaCarga)
        {
            switch (tipoIntegracaoOfertaCarga)
            {
                case TipoIntegracaoOfertaCarga.Ofertar:
                case TipoIntegracaoOfertaCarga.Ativar:
                    return SituacaoCargaOferta.EmOferta;
                case TipoIntegracaoOfertaCarga.Cancelar:
                    return SituacaoCargaOferta.Cancelada;
                case TipoIntegracaoOfertaCarga.Completar:
                    return SituacaoCargaOferta.Confirmada;
                case TipoIntegracaoOfertaCarga.Inativar:
                    return SituacaoCargaOferta.EmConfirmacao;
                default:
                    return SituacaoCargaOferta.PendenteDeOferta;
            }
        }

    }
}
