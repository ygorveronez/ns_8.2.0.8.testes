namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum IdentificadorControlePosicaoThread
    {
        VerificarCargasLiberarSemNFe = 1,
        VerificarCargasEmFinalizacaoCancelamento = 2,
        GerarCargasAguardandoGeracaoPreCarga = 3,
        SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe = 4,
        SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFrete = 5,
        SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissora = 6,
        SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteLeve = 7,
        SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteMedia = 8,
        SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFretePesada = 9,
        SolicitarEmissaoDocumentosAutorizadosNaEtapaNFeIntegracao = 10,
        SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracao = 11,
        SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissoraIntegracao = 12,
        SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoLeve = 13,
        SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoMedia = 14,
        SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoPesada = 15,
        ProcessarXMLCTeImportado = 16,
        VerificarLotesDeDesbloqueioPendentes = 17,
        GerarCargaEntregaPendentes = 18,
        FecharCargasEmFechamento = 19,
        AprovacaoMassivaChamado = 20,
        ConsultarValoresPedagioPendente = 21,
        IntegrarAverbacoesPendentesAutorizacao = 22,
        VerificarCargasPendentesEmissao = 23,
        VerificarCargasPendentesEmissaoIntegracao = 24,
        CargaOferta = 25,
        GerarCTesIntegracao = 26,
        GrupoMotoristas = 27,
        VerificarCargasOcorrenciaAutorizacaoPendentes = 28,
        RoteirizadorIntegracao = 29,
        SolicitacaoConfirmacaoDocumentosFiscais = 30,
        VerificarCargasGerarCTeAnteriorEmitirDocumentoSempreOrigemDestinoPedido = 31,
        FecharCargasEmFechamentoWorker = 32,
        VerificarEntregasPendentesNotificacao = 33,
        SolicitarEmissaoCargasEmEmissao = 34,
        SolicitarEmissaoCargasEmEmissaoWorker = 35,
        VerificarCargasFilialEmissoraAgGerarCTeAnterior = 36,
        GerarIntegracoesValePedagio = 37,
        VerificarRetornosValePedagio = 38,
        VerificarCargaIntegracaoPendentes = 39,
        VerificarIntegracoesCargaDadosTransportePendentes = 40,
        VerificarDocumentoComplementarPendenteEmissao = 41,
        VerificarMDFeManualPendentes = 42,
        GeracaoCargaEspelho = 43,
        IntegrarEventoSuperApp = 44,
        EfetuarDownloadXMLCTe = 46,
        GerarCargaPorMDFe = 49,
        BuscaCargasSemRoteirizacao = 50,
    }

    public static class IdentificadorControlePosicaoThreadHelper
    {
        public static string ObterDescricao(this IdentificadorControlePosicaoThread identificador)
        {
            return identificador.ToString();
        }

        public static string ObterDescricaoAdicional(this IdentificadorControlePosicaoThread identificador)
        {
            switch (identificador)
            {
                case IdentificadorControlePosicaoThread.VerificarCargasLiberarSemNFe: return "Etapa onde é feito a liberação de cargas sem NF-e de forma automática. Entidade principais manipuladas: Carga e XMLNotaFiscal";
                case IdentificadorControlePosicaoThread.VerificarCargasEmFinalizacaoCancelamento: return "Etapa onde é feita a finalização do cancelamento das cargas. Entidade principal manipulada: CancelamentoCarga";
                case IdentificadorControlePosicaoThread.GerarCargasAguardandoGeracaoPreCarga: return "Etapa onde é feita a finalização de montagem das pré cargas. Entidades principais manipuladas: Carga e PreCarga";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que estão na etapa de NF-e. Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFrete: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que estão na etapa de frete. Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissora: return "Etapa onde é feito a solicitação de emissão de documentos das cargas que são do tipo subContratação de filial emissora. Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteLeve: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que estão na etapa de frete (quantidade baixa de notas fiscais). Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteMedia: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que estão na etapa de frete (quantidade média de notas fiscais). Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFretePesada: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que estão na etapa de frete (quantidade alta de notas fiscais). Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaNFeIntegracao: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que estão na etapa de NF-e. Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracao: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que estão na etapa de frete. Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissoraIntegracao: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que são do tipo subContratação de filial emissora. Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoLeve: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que estão na etapa de frete (quantidade baixa de pedidos). Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoMedia: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que estão na etapa de frete (quantidade média de pedidos). Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoPesada: return "Etapa onde é feita a solicitação de emissão de documentos das cargas que estão na etapa de frete (quantidade alta de pedidos). Entidades principais manipuladas: Carga e CTe";
                case IdentificadorControlePosicaoThread.ProcessarXMLCTeImportado: return "Etapa onde é feito o processamento dos xml dos ct-es recebidos via integração pelo rest";
                case IdentificadorControlePosicaoThread.VerificarLotesDeDesbloqueioPendentes: return "Etapa onde é feito o processamento dos lotes de desbloqueio pendentes de envio de integração";
                case IdentificadorControlePosicaoThread.GerarCargaEntregaPendentes: return "Etapa onde é feita a geração dos controle de entregas pendentes. Entidades principais manipuladas: Carga e CargaEntrega";
                case IdentificadorControlePosicaoThread.FecharCargasEmFechamento: return "Etapa onde é feito o processamento dos fechamentos das cargas de forma assíncrona. Entidade principal manipulada: Carga";
                case IdentificadorControlePosicaoThread.AprovacaoMassivaChamado: return "Etapa onde é feita a aprovação massiva de chamados de forma assíncrona. Entidade principal manipulada: Chamados";
                case IdentificadorControlePosicaoThread.ConsultarValoresPedagioPendente: return "Etapa onde é feita a integração com vale pedágio pendente. Entidades principais manipuladas: Carga e CargaConsultaValorPedagioIntegracao";
                case IdentificadorControlePosicaoThread.IntegrarAverbacoesPendentesAutorizacao: return "Etapa onde é feita a integração para averbação de documentos pendentes de autorização. Entidade principal manipulada: AverbacaoCTe";
                case IdentificadorControlePosicaoThread.VerificarCargasPendentesEmissao: return "Etapa onde é feita a solicitação de emissão de documentos das cargas após emissão dos CT-es (MDF-e, vale pedágio, etc...). Entidade principal manipulada: Carga";
                case IdentificadorControlePosicaoThread.VerificarCargasPendentesEmissaoIntegracao: return "Etapa onde é feita a solicitação de emissão de documentos das cargas após emissão dos CT-es (MDF-e, vale pedágio, etc...). Entidade principal manipulada: Carga";
                case IdentificadorControlePosicaoThread.CargaOferta: return "Processamentos assíncronos da CargaOferta";
                case IdentificadorControlePosicaoThread.GerarCTesIntegracao: return "Processamentos de CTes recebidos pelo WebServiceCTe. Entidade principal manipulada: CTe";
                case IdentificadorControlePosicaoThread.GrupoMotoristas: return "Processamentos assíncronos do Grupo de Motoristas";
                case IdentificadorControlePosicaoThread.VerificarEntregasPendentesNotificacao: return "Processamentos assíncronos da Verificação das notificações de entregas pendentes";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoCargasEmEmissao: return "Processamentos assíncronos da solicitação de emissão de cargas";
                case IdentificadorControlePosicaoThread.SolicitarEmissaoCargasEmEmissaoWorker: return "Processamentos assíncronos da solicitação de emissão de cargas worker";
                case IdentificadorControlePosicaoThread.FecharCargasEmFechamentoWorker: return "Processamentos assíncronos do Fechamento de Cargas via Worker";
                case IdentificadorControlePosicaoThread.VerificarCargasOcorrenciaAutorizacaoPendentes: return "Etapa onde é feita a verificação de cargas com ocorrências de autorização pendentes. Entidade principal manipulada: CargaOcorrenciaAutorizacao";
                case IdentificadorControlePosicaoThread.RoteirizadorIntegracao: return "Processamentos de integração com roteirizadores.";
                case IdentificadorControlePosicaoThread.VerificarCargasFilialEmissoraAgGerarCTeAnterior: return "Etapa onde é verificado se há cargas de filial emissora que precisam gerar CT-e anterior";
                case IdentificadorControlePosicaoThread.VerificarCargasGerarCTeAnteriorEmitirDocumentoSempreOrigemDestinoPedido: return "Etapa onde é verificado se há cargas que precisam gerar CT-e anterior por emitir documento sempre origem/destino/pedido";
                case IdentificadorControlePosicaoThread.SolicitacaoConfirmacaoDocumentosFiscais: return "Processamentos da solicitação de confirmação de documentos fiscais. Entidade principal manipulada: Carga";
                case IdentificadorControlePosicaoThread.GerarIntegracoesValePedagio: return "Gera Integracoes do Vale Pedagio";
                case IdentificadorControlePosicaoThread.VerificarRetornosValePedagio: return "Verifica Integracoes do Vale Pedagio";
                case IdentificadorControlePosicaoThread.VerificarCargaIntegracaoPendentes: return "Verifica Integracoes da Carga Pendentes";
                case IdentificadorControlePosicaoThread.VerificarIntegracoesCargaDadosTransportePendentes: return "Verifica Integracoes da Carga Dados Transporte Pendentes";
                case IdentificadorControlePosicaoThread.VerificarDocumentoComplementarPendenteEmissao: return "Verifica Documentos Complementares Pendentes de Emissão";
                case IdentificadorControlePosicaoThread.VerificarMDFeManualPendentes: return "Verifica MDFe Manual pendentes de emissão";
                case IdentificadorControlePosicaoThread.GeracaoCargaEspelho: return "Responsável pela geração de cargas espelho, utilizada para replicar informações de cargas originais em Cargas Espelho. Entidades principais manipuladas: Carga e GeracaoCargaEspelho";
                case IdentificadorControlePosicaoThread.IntegrarEventoSuperApp: return "Processamentos assíncronos de eventos do Super App Trizy";
                case IdentificadorControlePosicaoThread.EfetuarDownloadXMLCTe: return "Efetuar download do xml do cte";
                case IdentificadorControlePosicaoThread.GerarCargaPorMDFe: return "Processamento Gerar Carga por MDFe";
                case IdentificadorControlePosicaoThread.BuscaCargasSemRoteirizacao: return "Busca Cargas para roterizar";
                default: return string.Empty;
            }
        }

        public static int ObterQuantidadeRegistrosRetornoPadrao(this IdentificadorControlePosicaoThread identificador)
        {
            switch (identificador)
            {
                case IdentificadorControlePosicaoThread.ConsultarValoresPedagioPendente:
                case IdentificadorControlePosicaoThread.GerarCTesIntegracao:
                case IdentificadorControlePosicaoThread.SolicitarEmissaoCargasEmEmissao:
                case IdentificadorControlePosicaoThread.SolicitarEmissaoCargasEmEmissaoWorker:
                case IdentificadorControlePosicaoThread.EfetuarDownloadXMLCTe:
                    return 10;

                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaNFeIntegracao:
                case IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracao:
                case IdentificadorControlePosicaoThread.VerificarCargasPendentesEmissaoIntegracao:
                    return 20;

                case IdentificadorControlePosicaoThread.GerarIntegracoesValePedagio:
                case IdentificadorControlePosicaoThread.VerificarRetornosValePedagio:
                case IdentificadorControlePosicaoThread.VerificarCargaIntegracaoPendentes:
                case IdentificadorControlePosicaoThread.VerificarIntegracoesCargaDadosTransportePendentes:
                    return 15;

                case IdentificadorControlePosicaoThread.IntegrarAverbacoesPendentesAutorizacao:
                case IdentificadorControlePosicaoThread.VerificarCargasPendentesEmissao:
                    return 100;

                default: return 5;
            }
        }
    }
}
