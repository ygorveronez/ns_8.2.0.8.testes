namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoWebServiceIntegracao
    {
        Oracle_uCTeServiceTS = 1,
        Oracle_uMDFeServiceTS = 2,
        Oracle_uNFSeServiceTS = 3,
        ATM_ATMWebSvrPortType = 4,
        A2_InterfaceExternaServicePortType = 5,
        Correios_AtendeCliente = 6,
        Ravex_WebServiceCte = 7,
        SemParar_ValePedagio = 8,
        Avon_Request = 9,
        AX_GSWMultiCTECancelService = 10,
        AX_PurchaseInvoiceService = 11,
        BrasilRisk_GestaoAnalisePerfil = 12,
        BrasilRisk_Monitoramento = 14,
        Buonny_IncluirSm = 15,
        SGTWebService_ConsultaNFe = 16,
        CTF_WsCopia = 17,
        EFrete_FaturamentoTransportadora = 18,
        EFrete_Logon = 19,
        EFrete_Motoristas = 20,
        EFrete_Pef = 21,
        EFrete_Proprietarios = 22,
        EFrete_Veiculos = 23,
        ELT_Averba = 24,
        Minerva_IntegraCargas = 25,
        Natura_SI_ProcessaConsultaNF = 26,
        Natura_SI_RecebeNotaisFiscais = 27,
        Natura_SI_ProcessaOcorrencias = 28,
        Natura_SI_ProcessaCteNfse = 29,
        Natura_SI_ProcessaPreFatura = 30,
        Natura_SI_ProcessaFatura = 31,
        Natura_Novo_SI_ProcessaCteNfse = 32,
        Natura_Novo_SI_ProcessaFatura = 33,
        NOX_IntegraGR = 34,
        Opentech_SgrOpentech = 35,
        Pamcard_WSTransacional = 36,
        PHContabil_MetodosPH = 37,
        Repom_Conciliacao = 38,
        Repom_Expedicao = 39,
        Repom_Integracao = 40,
        Senig_WebAppAverba = 41,
        SGTWebService_Empresas = 42,
        SigaFacil_WSFreteUnik = 43,
        Target_FreteTMS = 44,
        BancoCentral_FachadaWSSGS = 45,
        SGTWebService_Disponibilidade = 46,
        Commerce_Validacao = 47,
        Link_wsUltimas = 48,
        Omnilink_IASWSSoap = 49,
        Sascar_SasIntegraWS = 50,
        SystemSat_PosicoesSoap = 51,
        Sefaz_CTeRecepcaoEventoV4 = 52,
        Omnilink_IASWSSoapInterno = 53,
        Target_FreteTMSExtended = 54,
        NFeDistribuicaoDFe = 55,
		CTeDistribuicaoDFe = 56,
		Buonny_Posicoes = 57
}

    public static class TipoWebServiceIntegracaoHelper
    {
        public static string ObterDescricao(this TipoWebServiceIntegracao situacao)
        {
            switch (situacao)
            {
                case TipoWebServiceIntegracao.Oracle_uCTeServiceTS: return "Oracle - uCTeServiceTS - WS Emissão CT-e";
                case TipoWebServiceIntegracao.Oracle_uMDFeServiceTS: return "Oracle - uMDFeServiceTS - WS Emissão MDF-e";
                case TipoWebServiceIntegracao.Oracle_uNFSeServiceTS: return "Oracle - uNFSeServiceTS - WS Emissão NFS-e";
                case TipoWebServiceIntegracao.ATM_ATMWebSvrPortType: return "ATM - ATMWebSvr";
                case TipoWebServiceIntegracao.A2_InterfaceExternaServicePortType: return "A2 - InterfaceExterna";
                case TipoWebServiceIntegracao.Correios_AtendeCliente: return "Correios - AtendeCliente";
                case TipoWebServiceIntegracao.Ravex_WebServiceCte: return "Ravex - WebServiceCte";
                case TipoWebServiceIntegracao.SemParar_ValePedagio: return "Sem Parar - ValePedagio";
                case TipoWebServiceIntegracao.Avon_Request: return "Avon - Request";
                case TipoWebServiceIntegracao.AX_GSWMultiCTECancelService: return "AX - GSWMultiCTECancel";
                case TipoWebServiceIntegracao.AX_PurchaseInvoiceService: return "AX - PurchaseInvoice";
                case TipoWebServiceIntegracao.BrasilRisk_GestaoAnalisePerfil: return "BrasilRisk - GestaoAnalisePerfil";
                case TipoWebServiceIntegracao.BrasilRisk_Monitoramento: return "BrasilRisk - Monitoramento";
                case TipoWebServiceIntegracao.Buonny_IncluirSm: return "Buonny - IncluirSm";
                case TipoWebServiceIntegracao.SGTWebService_ConsultaNFe: return "SGT.WebService - ConsultaNFe";
                case TipoWebServiceIntegracao.CTF_WsCopia: return "CTF - WsCopia";
                case TipoWebServiceIntegracao.EFrete_FaturamentoTransportadora: return "e-frete - FaturamentoTransportadora";
                case TipoWebServiceIntegracao.EFrete_Logon: return "e-frete - Logon";
                case TipoWebServiceIntegracao.EFrete_Motoristas: return "e-frete - Motoristas";
                case TipoWebServiceIntegracao.EFrete_Pef: return "e-frete - Pef";
                case TipoWebServiceIntegracao.EFrete_Proprietarios: return "e-frete - Proprietarios";
                case TipoWebServiceIntegracao.EFrete_Veiculos: return "e-frete - Veiculos";
                case TipoWebServiceIntegracao.ELT_Averba: return "ELT - Averba";
                case TipoWebServiceIntegracao.Minerva_IntegraCargas: return "Minerva - IntegraCargas";
                case TipoWebServiceIntegracao.Natura_SI_ProcessaConsultaNF: return "Natura - SI_ProcessaConsultaNF";
                case TipoWebServiceIntegracao.Natura_SI_RecebeNotaisFiscais: return "Natura - SI_RecebeNotaisFiscais";
                case TipoWebServiceIntegracao.Natura_SI_ProcessaOcorrencias: return "Natura - SI_ProcessaOcorrencias";
                case TipoWebServiceIntegracao.Natura_SI_ProcessaCteNfse: return "Natura - SI_ProcessaCteNfse";
                case TipoWebServiceIntegracao.Natura_SI_ProcessaPreFatura: return "Natura - SI_ProcessaPreFatura";
                case TipoWebServiceIntegracao.Natura_SI_ProcessaFatura: return "Natura - SI_ProcessaFatura";
                case TipoWebServiceIntegracao.Natura_Novo_SI_ProcessaCteNfse: return "Natura - Novo - SI_ProcessaCteNfse";
                case TipoWebServiceIntegracao.Natura_Novo_SI_ProcessaFatura: return "Natura - Novo - SI_ProcessaFatura";
                case TipoWebServiceIntegracao.NOX_IntegraGR: return "NOX - IntegraGR";
                case TipoWebServiceIntegracao.Opentech_SgrOpentech: return "Opentech - SgrOpentech";
                case TipoWebServiceIntegracao.Pamcard_WSTransacional: return "Pamcard - WSTransacional";
                case TipoWebServiceIntegracao.PHContabil_MetodosPH: return "PHContabil - MetodosPH";
                case TipoWebServiceIntegracao.Repom_Conciliacao: return "Repom - Conciliacao";
                case TipoWebServiceIntegracao.Repom_Expedicao: return "Repom - Expedicao";
                case TipoWebServiceIntegracao.Repom_Integracao: return "Repom - Integracao";
                case TipoWebServiceIntegracao.Senig_WebAppAverba: return "Senig - WebAppAverba";
                case TipoWebServiceIntegracao.SGTWebService_Empresas: return "SGT.WebService - Empresas";
                case TipoWebServiceIntegracao.SigaFacil_WSFreteUnik: return "SigaFacil - WSFreteUnik";
                case TipoWebServiceIntegracao.Target_FreteTMS: return "Target - FreteTMS";
                case TipoWebServiceIntegracao.BancoCentral_FachadaWSSGS: return "BancoCentral - FachadaWSSGS";
                case TipoWebServiceIntegracao.SGTWebService_Disponibilidade: return "SGT.WebService - Disponibilidade";
                case TipoWebServiceIntegracao.Commerce_Validacao: return "Commerce - Validacao";
                case TipoWebServiceIntegracao.Omnilink_IASWSSoap: return "Omnilink - IASWSSoap";
                case TipoWebServiceIntegracao.Link_wsUltimas: return "Link - wsUltimas";
                case TipoWebServiceIntegracao.Sascar_SasIntegraWS: return "Sascar - SasIntegraWS";
                case TipoWebServiceIntegracao.SystemSat_PosicoesSoap: return "SystemSat - PosicoesSoap";
                case TipoWebServiceIntegracao.Sefaz_CTeRecepcaoEventoV4: return "Sefaz - CTeRecepcaoEventoV4";
                case TipoWebServiceIntegracao.Omnilink_IASWSSoapInterno: return "Omnilink - IASWSSoapInterno";
                case TipoWebServiceIntegracao.Target_FreteTMSExtended: return "Target - FreteTMSExtended";
                case TipoWebServiceIntegracao.NFeDistribuicaoDFe: return "NFeDistribuicaoDFe";
                case TipoWebServiceIntegracao.CTeDistribuicaoDFe: return "CTeDistribuicaoDFe";
                case TipoWebServiceIntegracao.Buonny_Posicoes: return "Buonny - Posicoes";
                default: return string.Empty;
            }
        }
    }
}
