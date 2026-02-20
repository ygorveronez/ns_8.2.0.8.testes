namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoEMP
    {
        CTe = 0,
        Carga = 1,
        CTeFatura = 2,
        CartaCorrecao = 3,
        Ocorrencia = 4,
        CTeManual = 5,
        NaoInformado = 6,
        Vessel = 7,
        Customer = 8,
        Booking = 9,
        Container = 10,
        DadosCarga = 11,
        Schedule = 12,
        CEMercante = 13,
        Fatura = 14,
        Boleto = 15,
        FaturaNFTP = 16,
        CTeNFTP = 17,
        NdNFTP = 18,
        CTeComplementarNFTP = 19,
        OcorrenciaNFTP = 20
    }

    public static class TipoIntegracaoEMPHelper
    {
        public static string ObterDescricao(this TipoIntegracaoEMP tipoIntegracao)
        {
            switch (tipoIntegracao)
            {
                case TipoIntegracaoEMP.CTe: return "CT-e";
                case TipoIntegracaoEMP.Carga: return "Carga";
                case TipoIntegracaoEMP.CTeFatura: return "CT-e Fatura";
                case TipoIntegracaoEMP.CartaCorrecao: return "Carta de Correção";
                case TipoIntegracaoEMP.Ocorrencia: return "Ocorrência";
                case TipoIntegracaoEMP.CTeManual: return "CT-e Manual";
                case TipoIntegracaoEMP.NaoInformado: return "Não Informado";
                case TipoIntegracaoEMP.Vessel: return "Vessel";
                case TipoIntegracaoEMP.Customer: return "Customer";
                case TipoIntegracaoEMP.Booking: return "Booking";
                case TipoIntegracaoEMP.Container: return "Container";
                case TipoIntegracaoEMP.DadosCarga: return "Dados da Carga";
                case TipoIntegracaoEMP.Schedule: return "Schedule";
                case TipoIntegracaoEMP.CEMercante: return "CEMercante";
                case TipoIntegracaoEMP.Fatura: return "Fatura";
                case TipoIntegracaoEMP.Boleto: return "Boleto";
                case TipoIntegracaoEMP.FaturaNFTP: return "Fatura NFTP";
                case TipoIntegracaoEMP.CTeNFTP: return "CTe NFTP";
                case TipoIntegracaoEMP.NdNFTP: return "ND NFTP";
                case TipoIntegracaoEMP.CTeComplementarNFTP: return "CTe Complementar NFTP";
                case TipoIntegracaoEMP.OcorrenciaNFTP: return "Ocorrência NFTP";
                default: return string.Empty;
            }
        }
    }
}
