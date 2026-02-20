using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoGestaoDevolucao
    {
        Nenhum = 0,
        Salesforce = 10,
        SalesforceNFeNaoCompativel = 11,
        SalesforcePosEntrega = 12,
        SAP = 20,
        SAPLaudo = 21,
    }

    public static class TipoIntegracaoGestaoDevolucaoHelper
    {
        public static string ObterDescricao(this TipoIntegracaoGestaoDevolucao TipoIntegracaoGestaoDevolucao)
        {
            switch (TipoIntegracaoGestaoDevolucao)
            {
                case TipoIntegracaoGestaoDevolucao.Salesforce: return "Salesforce";
                case TipoIntegracaoGestaoDevolucao.SalesforceNFeNaoCompativel: return "Salesforce - NFe Não Compatível";
                case TipoIntegracaoGestaoDevolucao.SalesforcePosEntrega: return "Salesforce - Pós Entrega";
                case TipoIntegracaoGestaoDevolucao.SAP: return "SAP";
                case TipoIntegracaoGestaoDevolucao.SAPLaudo: return "SAP - Laudo";
                case TipoIntegracaoGestaoDevolucao.Nenhum:
                default: return "Nenhum";
            }
        }

        public static int ObterMomentoRecusa(this TipoIntegracaoGestaoDevolucao TipoIntegracaoGestaoDevolucao)
        {
            switch (TipoIntegracaoGestaoDevolucao)
            {
                case TipoIntegracaoGestaoDevolucao.SalesforceNFeNaoCompativel: return 2; //Nota Divergente
                case TipoIntegracaoGestaoDevolucao.SalesforcePosEntrega: return 1; //Fora do Ato da Entrega
                case TipoIntegracaoGestaoDevolucao.Nenhum:
                default: return 3; //No Ato da Entrega
            }
        }

        public static List<TipoIntegracaoGestaoDevolucao> ObterTipoPorEtapa(this EtapaGestaoDevolucao etapaGestaoDevolucao)
        {
            switch (etapaGestaoDevolucao)
            {
                case EtapaGestaoDevolucao.IntegracaoLaudo:
                    return new List<TipoIntegracaoGestaoDevolucao>() { TipoIntegracaoGestaoDevolucao.SAPLaudo };
                case EtapaGestaoDevolucao.GestaoDeDevolucao:
                    return new List<TipoIntegracaoGestaoDevolucao>() { TipoIntegracaoGestaoDevolucao.SalesforceNFeNaoCompativel };
                case EtapaGestaoDevolucao.CenarioPosEntrega:
                    return new List<TipoIntegracaoGestaoDevolucao>() { TipoIntegracaoGestaoDevolucao.SalesforcePosEntrega };
                case EtapaGestaoDevolucao.OrdemeRemessa:
                    return new List<TipoIntegracaoGestaoDevolucao>() { TipoIntegracaoGestaoDevolucao.SalesforcePosEntrega };
                case EtapaGestaoDevolucao.DefinicaoTipoDevolucao:
                case EtapaGestaoDevolucao.AprovacaoTipoDevolucao:
                case EtapaGestaoDevolucao.GeracaoOcorrenciaDebito:
                case EtapaGestaoDevolucao.DefinicaoLocalColeta:
                case EtapaGestaoDevolucao.GeracaoCargaDevolucao:
                case EtapaGestaoDevolucao.AgendamentoParaDescarga:
                case EtapaGestaoDevolucao.GestaoCustoContabil:
                case EtapaGestaoDevolucao.Agendamento:
                case EtapaGestaoDevolucao.AprovacaoDataDescarga:
                case EtapaGestaoDevolucao.Monitoramento:
                case EtapaGestaoDevolucao.GeracaoLaudo:
                case EtapaGestaoDevolucao.AprovacaoLaudo:
                default: return null;
            }
        }
    }
}