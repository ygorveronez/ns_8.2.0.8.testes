namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPreCarga
    {
        AguardandoGeracaoCarga = 0,
        CargaGerada = 1,
        Cancelada = 2,
        Nova = 3,
        AguardandoAceite = 4,
        AguardandoDadosTransporte = 5
    }

    public static class SituacaoPreCargaHelper
    {
        public static string ObterCorFonte(this SituacaoPreCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoPreCarga.Nova: return string.Empty;
                case SituacaoPreCarga.Cancelada: return "#ffffff";
                default: return "#212529";
            }
        }

        public static string ObterCorLinha(this SituacaoPreCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoPreCarga.AguardandoAceite: return "#c2ff66";
                case SituacaoPreCarga.AguardandoDadosTransporte: return "#ffcc99";
                case SituacaoPreCarga.AguardandoGeracaoCarga: return "#ffd966";
                case SituacaoPreCarga.Cancelada: return "#ff6666";
                case SituacaoPreCarga.CargaGerada: return "#80ff80";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoPreCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoPreCarga.AguardandoAceite: return "Ag. Aceite Transportador";
                case SituacaoPreCarga.AguardandoDadosTransporte: return "Ag. Dados Transportador";
                case SituacaoPreCarga.AguardandoGeracaoCarga: return "Ag. Pré Carga";
                case SituacaoPreCarga.Cancelada: return "Cancelado";
                case SituacaoPreCarga.CargaGerada: return "Pre Carga Vinculada";
                case SituacaoPreCarga.Nova: return "Nova";
                default: return string.Empty;
            }
        }
    }
}
