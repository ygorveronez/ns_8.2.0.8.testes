namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoQuantidadeCarga
    {
        AgConfirmacaoTransportador = 1,
        AguardandoCarregamento = 2,
        AgLiberacaoParaTransportadores = 3,
        JaCarregada = 4,
        EmAtraso = 5,
        SemTransportador = 7,
        SemValorFrete = 8,
        TotalCargaJanelaCarregamento = 9,
        ProntaParaCarregar = 10,
        Faturada = 11
    }

    public static class TipoQuantidadeCargaHelper
    {
        public static string ObterCor(this TipoQuantidadeCarga tipo)
        {
            switch (tipo)
            {
                case TipoQuantidadeCarga.AgConfirmacaoTransportador: return "#e29e23";
                case TipoQuantidadeCarga.AgLiberacaoParaTransportadores: return "#1667c6";
                case TipoQuantidadeCarga.EmAtraso: return "#b94747";
                case TipoQuantidadeCarga.JaCarregada: return "#739e73";
                case TipoQuantidadeCarga.AguardandoCarregamento: return "#85de7b";
                case TipoQuantidadeCarga.SemTransportador: return "#c8e8ff";
                case TipoQuantidadeCarga.SemValorFrete: return "#ddd855";
                case TipoQuantidadeCarga.TotalCargaJanelaCarregamento: return "#292929";
                case TipoQuantidadeCarga.ProntaParaCarregar: return "#3d613c";
                case TipoQuantidadeCarga.Faturada: return "#66CDAA";
                default: return string.Empty;
            }
        } 

        public static string ObterDescricao(this TipoQuantidadeCarga tipo)
        {
            switch (tipo)
            {
                case TipoQuantidadeCarga.AgConfirmacaoTransportador: return "Ag. Confirmação do Transportador";
                case TipoQuantidadeCarga.AgLiberacaoParaTransportadores: return "Ag. Liberação para Transportadores";
                case TipoQuantidadeCarga.EmAtraso: return "Em Atraso";
                case TipoQuantidadeCarga.JaCarregada: return "Já Carregada";
                case TipoQuantidadeCarga.AguardandoCarregamento: return "Aguardando Carregamento";
                case TipoQuantidadeCarga.SemTransportador: return "Sem Transportador";
                case TipoQuantidadeCarga.SemValorFrete: return "Sem Valor de Frete";
                case TipoQuantidadeCarga.TotalCargaJanelaCarregamento: return "Total de Cargas";
                case TipoQuantidadeCarga.ProntaParaCarregar: return "Pronta para Carregar";
                case TipoQuantidadeCarga.Faturada: return "Faturada";
                default: return string.Empty;
            }
        }

        public static int ObterOrdem(this TipoQuantidadeCarga tipo)
        {
            switch (tipo)
            {
                case TipoQuantidadeCarga.SemTransportador: return 1;
                case TipoQuantidadeCarga.SemValorFrete: return 2;
                case TipoQuantidadeCarga.AgConfirmacaoTransportador: return 3;
                case TipoQuantidadeCarga.AgLiberacaoParaTransportadores: return 4;
                case TipoQuantidadeCarga.EmAtraso: return 5;
                case TipoQuantidadeCarga.AguardandoCarregamento: return 6;
                case TipoQuantidadeCarga.JaCarregada: return 7;
                case TipoQuantidadeCarga.ProntaParaCarregar: return 8;
                case TipoQuantidadeCarga.Faturada: return 9;
                case TipoQuantidadeCarga.TotalCargaJanelaCarregamento: return 10;
                default: return 11;
            }
        }
    }
}
