namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum OrigemAuditado
    {
        Sistema = 0,
        SistemaImportacao = 1,
        WebServiceCargas = 2,
        WebServiceCTe = 3,
        WebServiceEmpresa = 4,
        WebServiceFilial = 5,
        WebServiceImpressao = 6,
        WebServiceIntegracaoCTe = 7,
        WebServiceIntegracaoMDFe = 8,
        WebServiceIntegracaoNFSe = 9,
        WebServiceJanelaCarregamento = 10,
        WebServiceMDFe = 11,
        WebServiceNFe = 12,
        WebServiceNFS = 13,
        WebServiceOcorrencias = 14,
        WebServicePallet = 15,
        WebServicePessoas = 16,
        WebServiceRota = 17,
        WebServicePedidos = 18,
        WebServiceModeloVeicular = 19,
        WebServiceRastreadora = 20,
        LeituraEmail = 21,
        WebServiceCanhotos = 22,
        WebServiceMonitoriamento = 23,
        WebServiceFaturamento = 24,
        WebServiceControleEntrega = 25,
        WebServiceUsuarios = 26,
        WebServiceOrdemEmbarque = 27,
        GerenciadorApp = 28,
        WebServiceMobile = 29,
        WebServiceValePedagio = 30,
        WebServiceFretes = 31,
        WebServiceFinanceiro = 32,
        WebServiceOrdemServico = 33,
        LeituraDeRIC_WebServiceMobile = 34,
        LeituraDeRIC_Sistema = 35,
        WebServiceFrota = 36,
        WebSContratoFreteTerceiro = 37,
        IntegracaoCargaMultiEmbarcador = 38,
        WebServiceProduto = 39,
        WebServiceEntrega = 40,
        WebServiceAtendimento = 41,
        WebServicePrePlanejamento = 42,
        WebServiceSuperApp = 43,
        WebServiceOrdemCompra = 44,
        WebServiceDevolucao = 45,
        WebServiceModeloDados = 46,
        WebServiceMiddleware = 47,
        WebServiceWebhook = 48,
        WebServiceAbastecimento = 49,
        WebServiceCRT = 50,
    }

    public static class OrigemAuditadoHelper
    {
        public static string ObterDescricao(this OrigemAuditado origem)
        {
            switch (origem)
            {
                case OrigemAuditado.Sistema: return "Sistema";
                case OrigemAuditado.SistemaImportacao: return "Importação";
                case OrigemAuditado.WebServiceCargas: return "Cargas";
                case OrigemAuditado.WebServiceCTe: return "CT-e";
                case OrigemAuditado.WebServiceEmpresa: return "Empresa";
                case OrigemAuditado.WebServiceFilial: return "Filial";
                case OrigemAuditado.WebServiceImpressao: return "Impressão";
                case OrigemAuditado.WebServiceIntegracaoCTe: return "Integração de CT-e";
                case OrigemAuditado.WebServiceIntegracaoMDFe: return "Integração de MDF-e";
                case OrigemAuditado.WebServiceIntegracaoNFSe: return "Integração de NFS-e";
                case OrigemAuditado.WebServiceJanelaCarregamento: return "Janela de Carregamento";
                case OrigemAuditado.WebServiceMDFe: return "MDF-e";
                case OrigemAuditado.WebServiceMonitoriamento: return "Monitoramento";
                case OrigemAuditado.WebServiceNFe: return "NF-e";
                case OrigemAuditado.WebServiceNFS: return "NFS";
                case OrigemAuditado.WebServiceOcorrencias: return "Ocorrências";
                case OrigemAuditado.WebServicePallet: return "Pallet";
                case OrigemAuditado.WebServicePessoas: return "Pessoas";
                case OrigemAuditado.WebServiceRota: return "Rota";
                case OrigemAuditado.WebServicePedidos: return "Pedidos";
                case OrigemAuditado.WebServiceRastreadora: return "Rastreadora";
                case OrigemAuditado.LeituraEmail: return "Leitura de e-mail";
                case OrigemAuditado.WebServiceCanhotos: return "Canhotos";
                case OrigemAuditado.WebServiceFaturamento: return "Faturamento";
                case OrigemAuditado.WebServiceControleEntrega: return "Controle de Entrega";
                case OrigemAuditado.WebServiceUsuarios: return "Usuarios";
                case OrigemAuditado.WebServiceOrdemEmbarque: return "Ordem de Embarque";
                case OrigemAuditado.GerenciadorApp: return "GerenciadorApp";
                case OrigemAuditado.WebServiceValePedagio: return "Vale Pedágio";
                case OrigemAuditado.WebServiceFretes: return "Fretes";
                case OrigemAuditado.WebServiceFinanceiro: return "Financeiro";
                case OrigemAuditado.WebServiceOrdemServico: return "Ordem de Serviço";
                case OrigemAuditado.WebServiceFrota: return "Frota";
                case OrigemAuditado.WebSContratoFreteTerceiro: return "Contrato Frete Terceiro";
                case OrigemAuditado.WebServiceProduto: return "Produto";
                case OrigemAuditado.IntegracaoCargaMultiEmbarcador: return "Integração de cargas do MultiEmbarcador";
                case OrigemAuditado.WebServiceEntrega: return "WebService Entrega";
                case OrigemAuditado.WebServiceAtendimento: return "WebService Atendimento";
                case OrigemAuditado.WebServicePrePlanejamento: return "WebService Pré Planejamento";
                case OrigemAuditado.WebServiceSuperApp: return "WebService Super App";
                case OrigemAuditado.WebServiceModeloDados: return "WebService Modelo de Dados";
                case OrigemAuditado.WebServiceMiddleware: return "WebService Middleware";
                case OrigemAuditado.WebServiceAbastecimento: return "WebService Abastecimento";
                default: return string.Empty;
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega ObterOrigemSituacaoEntrega(this OrigemAuditado origem)
        {
            switch (origem)
            {
                case OrigemAuditado.Sistema: return Embarcador.Enumeradores.OrigemSituacaoEntrega.UsuarioMultiEmbarcador;
                case OrigemAuditado.WebServiceCargas:
                case OrigemAuditado.WebServiceCTe:
                case OrigemAuditado.WebServiceEmpresa:
                case OrigemAuditado.WebServiceFilial:
                case OrigemAuditado.WebServiceImpressao:
                case OrigemAuditado.WebServiceIntegracaoCTe:
                case OrigemAuditado.WebServiceIntegracaoMDFe:
                case OrigemAuditado.WebServiceIntegracaoNFSe:
                case OrigemAuditado.WebServiceJanelaCarregamento:
                case OrigemAuditado.WebServiceMDFe:
                case OrigemAuditado.WebServiceMonitoriamento:
                case OrigemAuditado.WebServiceNFe:
                case OrigemAuditado.WebServiceNFS:
                case OrigemAuditado.WebServiceOcorrencias:
                case OrigemAuditado.WebServicePallet:
                case OrigemAuditado.WebServicePessoas:
                case OrigemAuditado.WebServiceRota:
                case OrigemAuditado.WebServicePedidos:
                case OrigemAuditado.WebServiceRastreadora:
                case OrigemAuditado.WebServiceCanhotos:
                case OrigemAuditado.WebServiceFaturamento:
                case OrigemAuditado.WebServiceUsuarios:
                case OrigemAuditado.WebServiceOrdemEmbarque:
                case OrigemAuditado.WebServiceValePedagio:
                case OrigemAuditado.WebServiceFretes:
                case OrigemAuditado.WebServiceFinanceiro:
                case OrigemAuditado.WebServiceProduto:
                case OrigemAuditado.WebServiceOrdemServico:
                case OrigemAuditado.IntegracaoCargaMultiEmbarcador:
                case OrigemAuditado.WebServiceEntrega:
                case OrigemAuditado.WebServiceAtendimento:
                case OrigemAuditado.WebServicePrePlanejamento:
                case OrigemAuditado.WebServiceSuperApp:
                case OrigemAuditado.WebServiceModeloDados:
                case OrigemAuditado.WebServiceFrota: return Embarcador.Enumeradores.OrigemSituacaoEntrega.WebService;
                case OrigemAuditado.GerenciadorApp: return Embarcador.Enumeradores.OrigemSituacaoEntrega.MonitoramentoAutomaticamente;
                case OrigemAuditado.WebServiceControleEntrega: return Embarcador.Enumeradores.OrigemSituacaoEntrega.App;
                case OrigemAuditado.WebServiceMobile: return Embarcador.Enumeradores.OrigemSituacaoEntrega.App;
                default: return Embarcador.Enumeradores.OrigemSituacaoEntrega.UsuarioMultiEmbarcador;
            }
        }
    }
}
