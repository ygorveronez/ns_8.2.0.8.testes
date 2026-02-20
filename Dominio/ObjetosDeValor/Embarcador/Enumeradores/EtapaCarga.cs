namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaCarga
    {
        InicioEmbarcador = 1,
        InicioTMS = 2,
        FreteEmbarcador = 3,
        FreteTMS = 4,
        DadosTransportador = 5,
        NotaFiscal = 6,
        CTeNFs = 7,
        MDFe = 8,
        CTeFilialEmissora = 9,
        MDFeFilialEmissora = 10,
        Integracao = 11,
        Impressao = 12,
        IntegracaoFilialEmissora = 13,
        SubContratacao = 14,
        Transbordo = 15,
        SalvarDadosTransporte = 16
    }

    public static class EtapaCargaHelper
    {
        public static string ObterDescricao(this EtapaCarga situacaoPedido)
        {
            switch (situacaoPedido)
            {
                case EtapaCarga.InicioEmbarcador:
                    return "Início";
                case EtapaCarga.InicioTMS:
                    return "Início";
                case EtapaCarga.FreteEmbarcador:
                    return "Frete";
                case EtapaCarga.FreteTMS:
                    return "Frete";
                case EtapaCarga.DadosTransportador:
                    return "Dados Transportador";
                case EtapaCarga.NotaFiscal:
                    return "Nota Fiscal";
                case EtapaCarga.CTeNFs:
                    return "CT-e NF-s";
                case EtapaCarga.MDFe:
                    return "MDF-e";
                case EtapaCarga.CTeFilialEmissora:
                    return "CT-e Filial Emissora";
                case EtapaCarga.MDFeFilialEmissora:
                    return "MDF-e Filial Emissora";
                case EtapaCarga.Integracao:
                    return "Integração";
                case EtapaCarga.Impressao:
                    return "Impressão";
                case EtapaCarga.IntegracaoFilialEmissora:
                    return "Integração Filial Emissora";
                case EtapaCarga.SubContratacao:
                    return "Sub Contratação";
                case EtapaCarga.Transbordo:
                    return "Transbordo";
                case EtapaCarga.SalvarDadosTransporte:
                    return "Salvar Dados Transporte";
                default:
                    return string.Empty;
            }
        }
    }
}
