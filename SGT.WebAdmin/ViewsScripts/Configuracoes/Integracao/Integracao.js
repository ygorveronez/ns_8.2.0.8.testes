/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaMercante.js" />
/// <reference path="../../Enumeradores/EnumAngelLiraRegraCodigoIdentificacaoViagem.js" />
/// <reference path="../../Enumeradores/EnumTipoLayoutEDI.js" />
/// <reference path="../../Enumeradores/EnumDocumentosFiscaisTrizy.js" />
/// <reference path="../../Enumeradores/EnumVersaoA52.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoObtencaoCNPJTransportadora.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoAutenticacaoYMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _configuracaoIntegracao, _CRUDConfiguracaoIntegracao;

var _geoServiceGeocoding = [
    { text: "Google", value: 0 },
    { text: "Nominatim", value: 1 }
];

var ConfiguracaoIntegracao = function () {
    this.CodigoMatrizNatura = PropertyEntity({ text: "*Código da Matriz:", maxlength: 20, visible: ko.observable(true), required: false });
    this.UsuarioNatura = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaNatura = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.EnviarOcorrenciaNaturaAutomaticamente = PropertyEntity({ text: "Enviar Ocorrências Automaticamente para a Natura?", getType: typesKnockout.bool, val: ko.observable(true), def: true, visible: ko.observable(true), required: false });
    this.UtilizarValorFreteTMSNatura = PropertyEntity({ text: "Utilizar o valor do frete calculado pelo TMS?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });

    this.UsuarioOpenTech = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaOpenTech = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.DominioOpenTech = PropertyEntity({ text: "*Domínio:", maxlength: 50, visible: ko.observable(true), required: false });
    this.CodigoClienteOpenTech = PropertyEntity({ text: "*Código do cliente:", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.CodigoPASOpenTech = PropertyEntity({ text: "*Código PAS:", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.CodigoProdutoColetaOpentech = PropertyEntity({ text: "*Código produto para Coleta:", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.CodigoProdutoColetaEmbarcadorOpentech = PropertyEntity({ text: "Código produto para Coleta (Apólice seguro Embarcador):", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.CodigoProdutoColetaTransportadorOpentech = PropertyEntity({ text: "Código produto para Coleta (Apólice seguro Transportador)", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.CodigoProdutoPadraoOpentech = PropertyEntity({ text: "Código produto padrão (Quando não tiver grupo de produto, na etapa de integração)", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.URLOpenTech = PropertyEntity({ text: "*URL Web Service:", maxlength: 250, visible: ko.observable(true), required: false });
    this.ValorBaseOpenTech = PropertyEntity({ text: "*Valor Base:", getType: typesKnockout.decimal, visible: ko.observable(true), required: false });
    this.IntegrarVeiculoMotorista = PropertyEntity({ text: "Integrar veiculos e motoristas?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.IntegrarColetaOpentech = PropertyEntity({ text: "Integrar Coleta?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarCodigoEmbarcadorProdutoOpentech = PropertyEntity({ text: "Enviar código embarcador, do cadastro de produtos Opentech?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.AtualizarVeiculoMotoristaOpentech = PropertyEntity({ text: "Atualizar Veiculos e Motoristas?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.IntegrarRotaCargaOpentech = PropertyEntity({ text: "Integrar a rota da carga?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.BuscarCidadesOpenTech = PropertyEntity({ eventClick: BuscarCidadesOpenTechClick, type: types.event, text: "Buscar Códigos Localidades OpenTech", visible: ko.observable(true) });
    this.CodigoProdutoVeiculoComLocalizadorOpenTech = PropertyEntity({ text: "Código produto para veículos com localizador:", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.EmailsNotificacaoFalhaIntegracaoOpentech = PropertyEntity({ text: "Emails para notificar rejeição integração:", maxlength: 500, visible: ko.observable(true), required: false });
    this.CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador = PropertyEntity({ text: "Código de produto para validar somente veículo e motorista (sem uso de rastreador):", maxlength: 500, visible: ko.observable(true), required: false });
    this.NotificarFalhaIntegracaoOpentech = PropertyEntity({ text: "Notificar por e-mail em caso de rejeição na integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.PermitirTransportadorReenviarIntegracoesComProblemasOpenTech = PropertyEntity({ text: "Permitir que o Transportador Reenvie Integrações com problemas", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarDataNFeNaDataPrevistaOpentech = PropertyEntity({ text: "Enviar a data da NF-e na data prevista", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarDataPrevisaoEntregaDataCarregamentoOpentech = PropertyEntity({ text: "Enviar a data de previsão entrega na Data Prevista Fim e data carregamento na Data Prevista Início", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarDataAtualNaDataPrevisaoOpentech = PropertyEntity({ text: "Enviar a data atual na Data Prevista de Início e Fim", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.CalcularPrevisaoEntregaComBaseDistanciaOpentech = PropertyEntity({ text: "Enviar previsão de entrega de acordo com data de integração", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.IntegrarCargaOpenTechV10 = PropertyEntity({ text: "Integrar carga OpenTech versao V10", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });

    this.EnviarDataPrevisaoSaidaPedidoOpentech = PropertyEntity({ text: "Enviar \"Data Previsão Saída\" do pedido no campo dtprevini", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarInformacoesRastreadorCavaloOpentech = PropertyEntity({ text: "Enviar informações de rastreador do cavalo (rastreadorcavalo e cdemprastcavalo)", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarCodigoIntegracaoRotaCargaOpenTech = PropertyEntity({ text: "Enviar código de integração da rota da carga no campo cdrota", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarNrfonecelBrancoOpenTech = PropertyEntity({ text: "Enviar nrfonecel em branco", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarPlacaVeiculoSeNaoExistirNumeroFrotaOpenTech = PropertyEntity({ text: "Enviar placa do veículo no campo nrfrota se não existir número de frota", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarValorNotasValorDocOpenTech = PropertyEntity({ text: "Enviar valor das notas no campo valorDoc", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarCodigoIntegracaoCentroCustoCargaOpenTech = PropertyEntity({ text: "Enviar cód. integração do centro de custo da carga nos campos cdtransp e dsnomerespviag", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.EnviarValorDasNotasNoCampoValorDoc = PropertyEntity({ text: "Enviar Valor Das Notas No Campo ValorDoc", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.CadastrarRotaCargaOpentech = PropertyEntity({ text: "Cadastrar a rota da carga na Opentech", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.ConsiderarLocalidadeProdutoIntegracaoEntrega = PropertyEntity({ text: "Considerar localidade do produto na integração de entrega", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoGNRE = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioIntegracaoGNRE = PropertyEntity({ text: "*Usuário:", maxlength: 150, visible: ko.observable(true), required: false });
    this.SenhaIntegracaoGNRE = PropertyEntity({ text: "*Senha:", maxlength: 150, visible: ko.observable(true), required: false });
    this.URLIntegracaoGNRE = PropertyEntity({ text: "*URL Web Service:", maxlength: 150, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoLogRisk = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioLogRisk = PropertyEntity({ text: "*Usuário:", maxlength: 100, visible: ko.observable(true), required: false });
    this.SenhaLogRisk = PropertyEntity({ text: "*Senha:", maxlength: 100, visible: ko.observable(true), required: false });
    this.DominioLogRisk = PropertyEntity({ text: "*Domínio:", maxlength: 100, visible: ko.observable(true), required: false });
    this.CNPJClienteLogRisk = PropertyEntity({ text: "*CNPJ cadastrado na LogRisk:", maxlength: 100, visible: ko.observable(true), required: false });

    this.CodigoIntegradorEFrete = PropertyEntity({ text: "*Código do Integrador:", maxlength: 200, visible: ko.observable(true), required: false });
    this.UsuarioEFrete = PropertyEntity({ text: "*Usuário:", maxlength: 200, visible: ko.observable(true), required: false });
    this.SenhaEFrete = PropertyEntity({ text: "*Senha:", maxlength: 200, visible: ko.observable(true), required: false });
    this.MatrizEFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Matriz:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.EncerrarTodosCIOTAutomaticamente = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Encerrar Todos CIOTs Automaticamente", def: false, visible: ko.observable(true) });
    this.EnviarImpostosNaIntegracaoDoCIOT = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Enviar Impostos na Integração do CIOT", def: false, visible: ko.observable(true) });
    this.PossuiIntegracaoRecebivelEFrete = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração Recebível?", def: false, visible: ko.observable(true), required: ko.observable(false) })
    this.DeduzirImpostosValorTotalFrete = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Deduzir impostos do valor total do frete", def: false, visible: ko.observable(true), required: ko.observable(false) })
    this.VersaoEFrete = PropertyEntity({ text: "Versão do Vale Pedágio: ", val: ko.observable(EnumVersaoEFrete.Versao1), options: EnumVersaoEFrete.obterOpcoes(), def: EnumVersaoEFrete.Versao1, visible: ko.observable(true) });
    this.EnviarDadosRegulatorioANTT = PropertyEntity({ text: "Enviar dados Regulatório ANTT (Versão 2)", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), required: ko.observable(false) });
    this.ConsultarTagAoIncluirVeiculoNaCarga = PropertyEntity({ text: "Consultar TAG ao incluir veículo na carga", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLRecebivelEFrete = PropertyEntity({ text: "URL Recebível:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoEFrete = PropertyEntity({ text: "URL Autenticação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLCancelamentoRecebivel = PropertyEntity({ text: "URL Exclusão Recebível:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLPagamentoRecebivel = PropertyEntity({ text: "URL Pagamento Recebível:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.APIKeyEFrete = PropertyEntity({ text: "API Key:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.CodigoIntegracaoRecebivelEFrete = PropertyEntity({ text: "Codigo integração recebível e-frete:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioRecebivelEFrete = PropertyEntity({ text: "Usuário:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaRecebivelEFrete = PropertyEntity({ text: "Senha:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoBrasilRisk = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioBrasilRisk = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaBrasilRisk = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLHomologacaoBrasilRisk = PropertyEntity({ text: "*URL Homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLProducaoBrasilRisk = PropertyEntity({ text: "*URL Produção:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLBrasilRiskGestao = PropertyEntity({ text: "*URL Consulta Motoristas e Veiculos:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLBrasilRiskVeiculoMotorista = PropertyEntity({ text: "*URL Veículo e Motorista:", maxlength: 250, visible: ko.observable(true), required: false });
    this.CNPJEmbarcadorBrasilRisk = PropertyEntity({ getType: typesKnockout.cnpj, text: "*CNPJ Embarcador:", maxlength: 150, visible: ko.observable(true), required: false });
    this.ValorBaseBrasilRisk = PropertyEntity({ text: "Valor Base para gerar SM:", getType: typesKnockout.decimal, visible: ko.observable(true), required: false });
    this.EnviarTodosDestinosBrasilRisk = PropertyEntity({ text: "Enviar todos os destinos", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.InicioViagemFixoHoraAtualMaisMinutos = PropertyEntity({ text: "Inicio viagem fixo hora atual mais (minutos)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarDadosTransportadoraSubContratadaNasObservacoes = PropertyEntity({ text: "Enviar dados da transportadora sub-contratada nas observações", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MinutosAMaisInicioViagem = PropertyEntity({ text: "Minutos a mais:", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.BrasilRiskGerarParaCargasDeTransbordo = PropertyEntity({ text: "Gerar para Cargas de Transbordo", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.IntegrarRotaBrasilRisk = PropertyEntity({ text: "Integrar Rota", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.PossuiIntegracaoMundialRisk = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioMundialRisk = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaMundialRisk = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLHomologacaoMundialRisk = PropertyEntity({ text: "*URL Homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLProducaoMundialRisk = PropertyEntity({ text: "*URL Produção:", maxlength: 250, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoLogiun = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioLogiun = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaLogiun = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLHomologacaoLogiun = PropertyEntity({ text: "*URL Homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLProducaoLogiun = PropertyEntity({ text: "*URL Produção:", maxlength: 250, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoTrizy = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TokenTrizy = PropertyEntity({ text: "*Token APP:", maxlength: 5000, visible: ko.observable(true), required: false });
    this.URLTrizy = PropertyEntity({ text: "*URL:", maxlength: 5000, visible: ko.observable(true), required: false });
    this.AgenciaTrizy = PropertyEntity({ text: "*Agência:", maxlength: 500, visible: ko.observable(true), required: false });
    this.NaoRealizarIntegracaoPedido = PropertyEntity({ text: "Não realizar a integração no pedido?", visible: ko.observable(true), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.QuantidadeEixosPadrao = PropertyEntity({ text: "Quantidade Eixos Padrão:", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.CNPJCompanyTrizy = PropertyEntity({ text: "CNPJ Company:", maxlength: 20, visible: ko.observable(true), required: false });
    this.URLEnvioCarga = PropertyEntity({ text: "URL Envio de Carga:", maxlength: 500, visible: ko.observable(true), required: false });
    this.URLEnvioCancelamentoCarga = PropertyEntity({ text: "URL Envio de Cancelamento de Carga:", maxlength: 500, visible: ko.observable(true), required: false });
    this.URLEnvioEventosPatio = PropertyEntity({ text: "URL Envio Eventos Pátio:", maxlength: 500, visible: ko.observable(true), required: false });
    this.ValidarIntegracaoTrizyPorOperacao = PropertyEntity({ text: "Validar integração por tipo Operação?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TrizyIntegrarApenasCargasComControleDeEntrega = PropertyEntity({ text: "Validar apenas carga com Controle de Entrega para bloqueio de integração", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TrizyPermitirIntegrarMultiplasCargasParaOMesmoMotorista = PropertyEntity({ text: "Permitir integrar múltiplas cargas para o mesmo motorista", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TokenEnvioMS = PropertyEntity({ text: "Token Envio MS:", maxlength: 1000, visible: ko.observable(true), required: false });
    this.TrizyTipoDocumentoPais = PropertyEntity({ text: "Pais de Integração (Padrão do Tipo de Documento de Identificação)", val: ko.observable(EnumTipoDocumentoPaisTrizy.Brasil), options: EnumTipoDocumentoPaisTrizy.obterOpcoes(), def: EnumTipoDocumentoPaisTrizy.Brasil, visible: ko.observable(true) });
    this.TrizyEnviarPDFDocumentosFiscais = PropertyEntity({ text: "Enviar o PDF dos Documentos Fiscais:", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TrizyDocumentosFiscaisEnvioPDF = PropertyEntity({ text: "Documentos Fiscais:", getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumDocumentosFiscaisTrizy.obterOpcoes(), def: [] });
    this.EnviarPatchAtualizacoesEntrega = PropertyEntity({ text: "Enviar Patches de atualizações datas entregas", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.EnviarNomeFantasiaQuandoPossuir = PropertyEntity({ text: "Enviar Nome Fantasia no lugar da razão Social, quando possuir", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.VersaoIntegracaoTrizy = PropertyEntity({ text: "Versão da Integração", val: ko.observable(EnumVersaoIntegracaoTrizy.Versao1), options: EnumVersaoIntegracaoTrizy.obterOpcoes(), def: EnumVersaoIntegracaoTrizy.Versao1, visible: ko.observable(true) });

    this.IntegrarOfertasCargas = PropertyEntity({ text: "Integrar ofertas de cargas", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.URLIntegracaoOfertas = PropertyEntity({ text: "URL Integração de Ofertas:", maxlength: 2000, visible: ko.observable(false), required: false });
    this.URLIntegracaoGrupoMotoristas = PropertyEntity({ text: "URL Integração Grupo Motoristas:", maxlength: 2000, visible: ko.observable(false), required: false });

    this.DiasIntervaloTracking = PropertyEntity({ text: "Dias intervalo Tracking (inicio e fim)", getType: typesKnockout.int, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoAX = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLAX = PropertyEntity({ text: "*URL Transportadora:", maxlength: 5000, visible: ko.observable(true), required: false });
    this.URLAXContratoFrete = PropertyEntity({ text: "*URL Contrato Frete:", maxlength: 5000, visible: ko.observable(true), required: false });
    this.URLAXOrdemVenda = PropertyEntity({ text: "*URL Ordem Venda:", maxlength: 5000, visible: ko.observable(true), required: false });
    this.URLAXCompansacao = PropertyEntity({ text: "*URL Compensação:", maxlength: 5000, visible: ko.observable(true), required: false });
    this.URLAXPedido = PropertyEntity({ text: "*URL Pedido:", maxlength: 5000, visible: ko.observable(true), required: false });
    this.URLAXComplemento = PropertyEntity({ text: "*URL Complemento:", maxlength: 5000, visible: ko.observable(true), required: false });
    this.URLAXCancelamento = PropertyEntity({ text: "*URL Cancelamento:", maxlength: 5000, visible: ko.observable(true), required: false });
    this.UsuarioAX = PropertyEntity({ text: "*Usuário:", maxlength: 500, visible: ko.observable(true), required: false });
    this.SenhaAX = PropertyEntity({ text: "*Senha:", maxlength: 500, visible: ko.observable(true), required: false });
    this.CNPJAX = PropertyEntity({ text: "*CNPJ Empresa AX:", maxlength: 500, visible: ko.observable(true), required: false });

    this.CNPJClienteBuonny = PropertyEntity({ text: "CNPJ Cliente:", maxlength: 14, visible: ko.observable(true), required: false });
    this.TokenBuonny = PropertyEntity({ text: "Token:", maxlength: 50, visible: ko.observable(true), required: false });
    this.CNPJGerenciadoraDeRiscoBuonny = PropertyEntity({ text: "CNPJ Gerenciadora de risco:", maxlength: 14, visible: ko.observable(true), required: false });
    this.URLHomologacaoBuonny = PropertyEntity({ text: "*URL Homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLProducaoBuonny = PropertyEntity({ text: "*URL Produção:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLRestHomologacaoBuonny = PropertyEntity({ text: "*URL Rest Homologação:", maxlength: 500, visible: ko.observable(true), required: false });
    this.URLRestProducaoBuonny = PropertyEntity({ text: "*URL Rest Produção:", maxlength: 500, visible: ko.observable(true), required: false });
    this.TempoHorasConsultasBuonny = PropertyEntity({ text: "Tempo consultas automáticas:", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.CadastrarMotoristaAntesConsultarBuonny = PropertyEntity({ text: "Cadastrar Motorista antes de Consultar", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.URLAvior = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UsuarioAvior = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaAvior = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.CNPJAvior = PropertyEntity({ text: "*CNPJ:", maxlength: 14, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoNOX = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioNOX = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaNOX = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.TokenNOX = PropertyEntity({ text: "*Token:", maxlength: 50, visible: ko.observable(true), required: false });
    this.CNPJMatrizNOX = PropertyEntity({ text: "CNPJ da Matriz:", maxlength: 14, visible: ko.observable(true), required: false });
    this.URLHomologacaoNOX = PropertyEntity({ text: "*URL Homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLProducaoNOX = PropertyEntity({ text: "*URL Produção:", maxlength: 250, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoGoldenService = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CodigoGoldenService = PropertyEntity({ text: "*Código:", maxlength: 50, visible: ko.observable(true), required: false });
    this.IdGoldenService = PropertyEntity({ text: "*ID:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaGoldenService = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLHomologacaoGoldenService = PropertyEntity({ text: "*URL Homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLProducaoGoldenService = PropertyEntity({ text: "*URL Produção:", maxlength: 250, visible: ko.observable(true), required: false });

    this.URLCarrefourCancelamentoCarga = PropertyEntity({ text: "*URL Integração de Cancelamento de Carga:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLCarrefourCarga = PropertyEntity({ text: "*URL Integração de CT-es da Carga:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLCarrefourIndicadorIntegracaoCTe = PropertyEntity({ text: "*URL CT-e Indicador de Integração:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLCarrefourProvisao = PropertyEntity({ text: "*URL Provisão:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLCarrefourOcorrencia = PropertyEntity({ text: "*URL CT-e Ocorrência:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLCarrefourValidarCancelamentoCarga = PropertyEntity({ text: "*URL Validação de Cancelamento de Carga:", maxlength: 250, visible: ko.observable(true), required: false });
    this.TokenCarrefour = PropertyEntity({ text: "*Token:", maxlength: 250, visible: ko.observable(true), required: false });
    this.TokenCarrefourIndicadorIntegracaoCTe = PropertyEntity({ text: "*Token Indicador de Integração:", maxlength: 250, visible: ko.observable(true), required: false });
    this.TokenCarrefourProvisao = PropertyEntity({ text: "*Token Provisão:", maxlength: 250, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoGPA = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLHomologacaoGPA = PropertyEntity({ text: "*URL Homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLProducaoGPA = PropertyEntity({ text: "*URL Produção:", maxlength: 250, visible: ko.observable(true), required: false });
    this.APIKeyGPA = PropertyEntity({ text: "API Key:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });

    this.HomologacaoAngelLira = PropertyEntity({ text: "Homologação", getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.UsuarioAngelLira = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaAngelLira = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLAngelLira = PropertyEntity({ text: "*URL:", maxlength: 50, visible: ko.observable(true), required: false });
    this.ObterRotasAutomaticamenteAngelLira = PropertyEntity({ text: "Obter rotas automaticamente", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegracaoTemperaturaAngelLira = PropertyEntity({ text: "Integrar a temperatura", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarDataAgendamentoPedidoAngelLira = PropertyEntity({ text: "Utilizar a data de agendamento do pedido para envio", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarDadosFormatadosAngelLira = PropertyEntity({ text: "Enviar dados formatados (CPF/CNPJ e Placa)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoEnviarRotaViagemAngelLira = PropertyEntity({ text: "Não integrar a rota na viagem", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarViagensPorPedidoAngelLira = PropertyEntity({ text: "Gerar viagens por pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AplicarRegraLocalPalletizacaoAngelLira = PropertyEntity({ text: "Aplicar regra para local de palletização", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsultarPosicaoAbastecimentoAngelLira = PropertyEntity({ text: "Ativar a consulta de abastecimentos", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLAcessoPedido = PropertyEntity({ text: "URL Pedido:", maxlength: 200, visible: ko.observable(true), required: false });
    this.UsuarioAcessoPedido = PropertyEntity({ text: "Usuario Pedido:", maxlength: 100, visible: ko.observable(true), required: false });
    this.SenhaAcessoPedido = PropertyEntity({ text: "Senha Pedido:", maxlength: 100, visible: ko.observable(true), required: false });
    this.UtilizarDataAtualETempoRotaParaInicioEFimViagemAngelLira = PropertyEntity({ text: "Utilizar a data atual e o tempo da rota para o início e fim da viagem", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegraCodigoIdentificacaoViagem = PropertyEntity({ val: ko.observable(EnumAngelLiraRegraCodigoIdentificacaoViagem.NumeroPedidoEmbarcadorMaisPlaca), options: EnumAngelLiraRegraCodigoIdentificacaoViagem.obterOpcoes(), def: EnumAngelLiraRegraCodigoIdentificacaoViagem.NumeroPedidoEmbarcadorMaisPlaca, text: "Regra código identificação viagem" });
    this.IgnorarValidacaoCargaAgrupadaRegraCodigoViagemAngelLira = PropertyEntity({ text: "Ignorar a validação de carga agrupada na regra código identificação viagem", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.URLOrtec = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UsuarioOrtec = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaOrtec = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.IntegrarEntregaOrtec = PropertyEntity({ text: "Gerar Integrações de Entrega", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.APIKeyGoogle = PropertyEntity({ text: "Key API:" });
    this.GeoServiceGeocoding = PropertyEntity({ text: "Serviço de Geocoding: ", val: ko.observable(0), options: _geoServiceGeocoding, def: 0, visible: _CONFIGURACAO_TMS.UsuarioInternoAdministrador });
    this.ServidorRouteOSM = PropertyEntity({ text: "Servidor Route OSRM:", maxlength: 50, visible: _CONFIGURACAO_TMS.UsuarioInternoAdministrador });
    this.ServidorRouteGoogleOrTools = PropertyEntity({ text: "Servidor Route OrTools:", maxlength: 100, visible: _CONFIGURACAO_TMS.UsuarioInternoAdministrador });
    this.ServidorNominatim = PropertyEntity({ text: "Serviço Nominatim:", maxlength: 100, visible: _CONFIGURACAO_TMS.UsuarioInternoAdministrador });

    this.URLIntegracaoCanhotoPiracanjuba = PropertyEntity({ text: "Url Integração Canhoto:" });
    this.URLIntegracaoCanhotoPiracanjubaContingencia = PropertyEntity({ text: "Url Integração Canhoto em contingencia:" });
    this.DataFaturamentoNota = PropertyEntity({ text: ko.observable("Data Faturamento Nota:"), getType: typesKnockout.date, required: false, enable: ko.observable(false), visible: ko.observable(true) });
    this.URLIntegracaoCargaPiracanjuba = PropertyEntity({ text: "Url Integração Carga:" });
    this.StringAmbientePiracanjuba = PropertyEntity({ text: "Tipo Ambiente:", getType: typesKnockout.string, maxlength: 3, visible: ko.observable(true) });
    this.AmbienteProducaoPiracanjuba = PropertyEntity({ text: "Ambiente de Produção", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLAutenticacaoPiracanjuba = PropertyEntity({ text: "Url Autenticação:" });
    this.ClientIDPiracanjuba = PropertyEntity({ text: "Client ID:" });
    this.ClientSecretPiracanjuba = PropertyEntity({ text: "Client Secret:" });

    this.EmpresaFixaPamCard = PropertyEntity({ text: "Empresa fixa para integração de pagamentos:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.URLPamcardCorporativo = PropertyEntity({ text: "URL Pamcard Corporativo:", maxlength: 500, visible: ko.observable(true), required: false });
    this.URLPamcardCorporativoAutenticacao = PropertyEntity({ text: "URL Autenticação Pamcard Corporativo::", maxlength: 500, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoRaster = PropertyEntity({ text: "Habilitar integração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioRaster = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaRaster = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLRaster = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: false });
    this.NotificarFalhaIntegracaoRaster = PropertyEntity({ text: "Notificar operador da carga em caso de falha na integração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarIntegracaoPreSmEtapaCargaDadosTransporteRaster = PropertyEntity({ text: "Gerar integração PreSM na etapa carga dados transporte", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarIntegracaoEfetivaPreSmEtapaCargaFreteRaster = PropertyEntity({ text: "Gerar integração EfetivaPreSM na etapa carga frete", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarIntegracaoSetRevisaoPreSMnaEtapaIntegracao = PropertyEntity({ text: "Gerar integração setRevisaoPreSM na etapa de integração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ReenviarIntegracaoDadosTransporteAoAlterarDadosTransporteRaster = PropertyEntity({ text: "Reenviar integracao dados transporte ao alterar dados transporte", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.PossuiIntegracaoSAD = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConfiguracoesIntegracaoSAD = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaConfiguracoesIntegracaoSAD = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.UsuarioUnileverFourKites = PropertyEntity({ text: "*Usuário:", maxlength: 150, visible: ko.observable(true), required: false });
    this.SenhaUnileverFourKites = PropertyEntity({ text: "*Senha:", maxlength: 150, visible: ko.observable(true), required: false });
    this.URLHomologacaoUnileverFourKites = PropertyEntity({ text: "*URL Homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLProducaoUnileverFourKites = PropertyEntity({ text: "*URL Produção:", maxlength: 250, visible: ko.observable(true), required: false });

    //Mercado Livre
    this.URLMercadoLivre = PropertyEntity({ text: "*URL:", maxlength: 150, visible: ko.observable(true), required: false });
    this.IDMercadoLivre = PropertyEntity({ text: "*ID:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SecretKeyMercadoLivre = PropertyEntity({ text: "*Secret Key:", maxlength: 50, visible: ko.observable(true), required: false });
    this.LimparComposicaoCargaRetiradaRotaFacilityMercadoLivre = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Limpar a composição da carga quando retirada o rota e facility", def: false, visible: ko.observable(true), required: ko.observable(false) })
    this.NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes = PropertyEntity({ text: "Não  atualizar os dados desta pessoa em importações de notas fiscais ou  integrações", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.URLAutenticacaoDigibee = PropertyEntity({ text: "Endpoint Autenticação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UsuarioAutenticacaoDigibee = PropertyEntity({ text: "Usuário Autenticação:", maxlength: 150, visible: ko.observable(true), required: false });
    this.SenhaAutenticacaoDigibee = PropertyEntity({ text: "Senha Autenticação:", maxlength: 1000, visible: ko.observable(true), required: false });

    this.URLIntegracaoDigibee = PropertyEntity({ text: "Endpoint Integração Carregamento:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLIntegracaoCancelamentoDigibee = PropertyEntity({ text: "Endpoint Cancelamento:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLIntegracaoDadosCargaDigibee = PropertyEntity({ text: "Endpoint Dados Carga:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLIntegracaoDadosContabeisCTeDigibee = PropertyEntity({ text: "Endpoint Dados Contabeis dos Documentos:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLIntegracaoEscrituracaoCTeDigibee = PropertyEntity({ text: "Endpoint Escrituração dos Documentos:", maxlength: 250, visible: ko.observable(true), required: false });
    this.APIKeyDigibee = PropertyEntity({ text: "API Key Autenticação:", maxlength: 150, visible: ko.observable(true), required: false });
    this.APIKeyDigibeeGeral = PropertyEntity({ text: "API Key Geral:", maxlength: 150, visible: ko.observable(true), required: false });
    this.IntegracaoDigibeePadraoConsinco = PropertyEntity({ text: "Integração padrão Consinco?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AjustarDataParaCorresponderQuinzenaDigibee = PropertyEntity({ text: "Ajustar data para corresponder ao período da quinzena", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.PossuiIntegracaoPH = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioPH = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaPH = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLHomologacaoPH = PropertyEntity({ text: "*URL Homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLProducaoPH = PropertyEntity({ text: "*URL Produção:", maxlength: 250, visible: ko.observable(true), required: false });
    this.CNPJContadorPH = PropertyEntity({ text: "*CNPJ Contador:", maxlength: 250, visible: ko.observable(true), required: false });
    this.SoftwarePH = PropertyEntity({ text: "*Software:", maxlength: 250, visible: ko.observable(true), required: false });
    this.PortaPH = PropertyEntity({ text: "*Porta:", maxlength: 250, visible: ko.observable(true), required: false });
    this.IPSocketPH = PropertyEntity({ text: "*IP Socket:", maxlength: 250, visible: ko.observable(true), required: false });
    this.PortaSocketPH = PropertyEntity({ text: "*Porta Socket:", maxlength: 250, visible: ko.observable(true), required: false });

    this.URLIntegracaoTelerisco = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: false });
    this.CaminhoCertificadoTelerisco = PropertyEntity({ text: "Caminho certificado homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.SenhaCertificadoTelerisco = PropertyEntity({ text: "Senha certificado homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.CodigosAceitosRetornoTelerisco = PropertyEntity({ text: "Códigos de sucesso ex (280,350,400,...):", maxlength: 2000, visible: ko.observable(true), required: false });
    this.IntegracaoViaPOSTTelerisco = PropertyEntity({ text: "Ativar integração via POST?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    //this.CNPJEmbarcadorTelerisco = PropertyEntity({ text: "CNPJ Embarcador:", maxlength: 150, visible: ko.observable(true), required: false });
    this.EmpresaFixaTelerisco = PropertyEntity({ text: "Empresa fixa para integração:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NaoEnviarDataEmbarqueGrMotoristaTelerisco = PropertyEntity({ text: "Não enviar data de embarque na integração de GR?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarEmpresaFixaComoCNPJEmbarcadorNaIntegracaoTelerisco = PropertyEntity({ text: "Enviar Empresa fixa como CNPJ Embarcador na integração", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.URLIntegracaoCargoX = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: false });
    this.TokenCargoX = PropertyEntity({ text: "*Token:", maxlength: 2000, visible: ko.observable(true), required: false });

    this.URLIntegracaoRiachuelo = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: false });
    this.URLIntegracaoEntregaNFeRiachuelo = PropertyEntity({ text: "*URL Entrega NFe:", maxlength: 500, visible: ko.observable(true), required: false });
    this.HabilitarDataSaidaCDLoja = PropertyEntity({ text: "Habilitar data de saída da loja e CD", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.PossuiIntegracaoKrona = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoKrona = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoBoticario = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoBoticario = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: false });
    this.IntegracaoBoticarioClientId = PropertyEntity({ text: "*Client ID:", maxlength: 250, visible: ko.observable(true), required: false });
    this.IntegracaoBoticarioClientSecret = PropertyEntity({ text: "*Client Secret:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLGerarTokenBoticario = PropertyEntity({ text: "*URL Gerar Token:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLEnvioSequenciaBoticario = PropertyEntity({ text: "*URL Envio Sequência:", maxlength: 250, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoBoticarioFreeFlow = PropertyEntity({ text: "Habilitar integração Free Flow?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoBoticarioFreeFlow = PropertyEntity({ text: "URL Sênior:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLAutenticacaoBoticarioFreeFlow = PropertyEntity({ text: "URL Autenticação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.ClientSecretBoticarioFreeFlow = PropertyEntity({ text: "Client Secret:", maxlength: 250, visible: ko.observable(true), required: false });
    this.ClientIdBoticarioFreeFlow = PropertyEntity({ text: "Client ID:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLConsultaAVIPED = PropertyEntity({ text: "URL Consulta AVIPED:", maxlength: 250, visible: ko.observable(true), required: false });

    this.URLIntegracaoDPA = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLAutenticacaoDPA = PropertyEntity({ text: "*URL autenticação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UsuarioAutenticacaoDPA = PropertyEntity({ text: "*Usuário autenticação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.SenhaAutenticacaoDPA = PropertyEntity({ text: "*Senha autenticação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLIntegracaoDPACiot = PropertyEntity({ text: "*URL CIOT:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLAutenticacaoDPACiot = PropertyEntity({ text: "*URL autenticação CIOT:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UsuarioAutenticacaoDPACiot = PropertyEntity({ text: "*Usuário autenticação CIOT:", maxlength: 250, visible: ko.observable(true), required: false });
    this.SenhaAutenticacaoDPACiot = PropertyEntity({ text: "*Senha autenticação CIOT:", maxlength: 250, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoInfolog = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoInfolog = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UsuarioInfolog = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaInfolog = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.CodigoOperacaoInfolog = PropertyEntity({ text: "*Codigo Operação:", maxlength: 6, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoSaintGobain = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoSaintGobain = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UserNameSaintGobain = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.PasswordSaintGobain = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLIntegracaoPedidoSaintGobain = PropertyEntity({ text: "*URL Consulta Pedido:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLIntegracaoUsuarioSaintGobain = PropertyEntity({ text: "*URL Consulta Usuário:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLValidaTokenSaintGobain = PropertyEntity({ text: "*URL Valida Token:", maxlength: 250, visible: ko.observable(true), required: false });
    this.ApikeySaintGobain = PropertyEntity({ text: "*API Key:", maxlength: 250, visible: ko.observable(true), required: false });
    this.ClientIDSaintGobain = PropertyEntity({ text: "*Client ID:", maxlength: 250, visible: ko.observable(true), required: false });
    this.ClientSecretSaintGobain = PropertyEntity({ text: "*Client Secret:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UrlIntegracaoCargaSnowFlake = PropertyEntity({ text: "*Url Integração Carga:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UrlIntegracaoAgendamentoSnowFlake = PropertyEntity({ text: "*Url Integração Agendamento:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UrlIntegracaoFreteSnowFlake = PropertyEntity({ text: "*Url Integração Frete:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UsuariosSnowFlake = PropertyEntity({ text: "*Usuario:", maxlength: 250, visible: ko.observable(true), required: false });
    this.SenhaSnowFlake = PropertyEntity({ text: "*Senha:", maxlength: 250, visible: ko.observable(true), required: false });
    this.ApiKeySnowFlake = PropertyEntity({ text: "*Api Key:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UtilizarEndPointPIPO = PropertyEntity({ text: "Utilizar EndPoint PI/PO", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.PossuiIntegracaoUltragaz = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoUltragaz = PropertyEntity({ text: "*URL Integração:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLAutenticacaoUltragaz = PropertyEntity({ text: "*URL Autenticação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.ClientSecretUltragaz = PropertyEntity({ text: "*Client Secret:", maxlength: 50, visible: ko.observable(true), required: false });
    this.ClientIdUltragaz = PropertyEntity({ text: "*Client Id:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLContabilizacaoUltragaz = PropertyEntity({ text: "URL Contabilização:", maxlength: 300, visible: ko.observable(true), required: false });
    this.URLIntegracaoVeiculoUltragaz = PropertyEntity({ text: "URL Integração Veículo:", maxlength: 300, visible: ko.observable(true), required: false });
    this.NaoPermitirReenviarIntegracaoPagamentoAgRetorno = PropertyEntity({ text: "Não permitir reenviar integração de pagamento aguardando retorno", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.URLToledo = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: false });

    this.URLQbit = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: false });

    this.URLA52 = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLNovaA52 = PropertyEntity({ text: "URL Nova:", maxlength: 250, visible: ko.observable(true), required: false });
    this.CPFCNPJA52 = PropertyEntity({ text: "*CPF/CNPJ:", maxlength: 100, visible: ko.observable(true), required: false });
    this.SenhaA52 = PropertyEntity({ text: "*Senha:", maxlength: 100, visible: ko.observable(true), required: false });
    this.UtilizarDataAgendamentoPedidoA52 = PropertyEntity({ text: "Utilizar a data de agendamento do pedido para envio", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegrarMacrosDadosTransporteCargaA52 = PropertyEntity({ text: "Integrar Macros nos dados de transporte da carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegrarSituacaoMotoristaA52 = PropertyEntity({ text: "Integrar situações do motorista", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AplicarRegraLocalPalletizacaoA52 = PropertyEntity({ text: "Aplicar regra para local de palletização", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VersaoIntegracaoA52 = PropertyEntity({ text: "Versão integração: ", val: ko.observable(EnumVersaoA52.Versao10), options: EnumVersaoA52.obterOpcoes(), def: EnumVersaoA52.Versao10, visible: ko.observable(true) });

    this.ConfiguracoesIntegracaoAvon = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaConfiguracoesIntegracaoAvon = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.URLAdagio = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: false });
    this.EmailAdagio = PropertyEntity({ text: "*E-mail:", maxlength: 100, visible: ko.observable(true), required: false });
    this.SenhaAdagio = PropertyEntity({ text: "*Senha:", maxlength: 100, visible: ko.observable(true), required: false });

    this.URLCorreios = PropertyEntity({ text: "URL Buscar valor Postagem:", maxlength: 500, visible: ko.observable(true), required: false });
    this.UsuarioCorreios = PropertyEntity({ text: "Usuário Rastro:", maxlength: 100, visible: ko.observable(true), required: false });
    this.SenhaCorreios = PropertyEntity({ text: "Senha Rastro:", maxlength: 100, visible: ko.observable(true), required: false });
    this.URLTokenCorreios = PropertyEntity({ text: "URL Token:", maxlength: 500, visible: ko.observable(true), required: false });
    this.URLEventosCorreios = PropertyEntity({ text: "URL Eventos:", maxlength: 500, visible: ko.observable(true), required: false });
    this.CartaoPostagemCorreios = PropertyEntity({ text: "Cartão Postagem:", maxlength: 500, visible: ko.observable(true), required: false });
    this.URLPLPCorreios = PropertyEntity({ text: "URL PLP:", maxlength: 500, visible: ko.observable(true), required: false });
    this.UsuarioSIGEP = PropertyEntity({ text: "Usuário SIGEP:", maxlength: 500, visible: ko.observable(true), required: false });
    this.SenhaSIGEP = PropertyEntity({ text: "Senha SIGEP:", maxlength: 500, visible: ko.observable(true), required: false });
    this.NumeroContratoCorreios = PropertyEntity({ text: "Número Contrato:", maxlength: 100, visible: ko.observable(true), required: false });
    this.NumeroDiretoriaCorreios = PropertyEntity({ text: "Número Diretoria:", maxlength: 100, visible: ko.observable(true), required: false });
    this.CodigoAdministrativoCorreios = PropertyEntity({ text: "Código Administrativo:", maxlength: 100, visible: ko.observable(true), required: false });
    this.CodigoServicoAdicionalCorreios = PropertyEntity({ text: "Código Serviço Adicional:", maxlength: 100, visible: ko.observable(true), required: false });

    this.URLEmillenium = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: false });
    this.URLEmilleniumConfirmarEntrega = PropertyEntity({ text: "URL Confirmar Entrega:", maxlength: 500, visible: ko.observable(true), required: false });
    this.SenhaFrontDoor = PropertyEntity({ text: "Senha Front Door:", maxlength: 500, visible: ko.observable(true), required: false });
    this.UsuarioEmillenium = PropertyEntity({ text: "*Usuário:", maxlength: 100, visible: ko.observable(true), required: false });
    this.SenhaEmillenium = PropertyEntity({ text: "*Senha:", maxlength: 100, visible: ko.observable(true), required: false });
    this.TransIdAtualEmillenium = PropertyEntity({ text: "*Trans_ID consulta Notas:", getType: typesKnockout.int, visible: ko.observable(true), def: 0, val: ko.observable(0) });
    //this.QuantidadeNotificacaoEmillenium = PropertyEntity({ text: "Quantidade registros pendentes para notificar: ", required: false, visible: true, getType: typesKnockout.int });
    //this.EmailsNotificacaoEmillenium = PropertyEntity({ text: "E-mails para notificação: ", required: false, visible: true });

    this.TransIdInicioBuscaMassiva = PropertyEntity({ text: "Trans_ID Inicial Consulta Notas Massiva:", getType: typesKnockout.int, visible: ko.observable(true), def: 0, enable: false, val: ko.observable(0) });
    this.TransIdFimBuscaMassiva = PropertyEntity({ text: "Trans_ID Final Consulta Notas Massiva:", getType: typesKnockout.int, visible: ko.observable(true), def: 0, enable: false, val: ko.observable(0) });
    this.DataFinalizacaoBuscaTransIdMassivo = PropertyEntity({ text: "Data Finalização consulta Massiva:", getType: typesKnockout.text, visible: ko.observable(true), def: "", enable: false, val: ko.observable("") });


    this.PossuiIntegracaoMichelin = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioMichelin = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaMichelin = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLHomologacaoMichelin = PropertyEntity({ text: "*URL Homologação:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLProducaoMichelin = PropertyEntity({ text: "*URL Produção:", maxlength: 250, visible: ko.observable(true), required: false });
    this.CodigoTransportadoraMichelin = PropertyEntity({ text: "*Código Transportadora:", maxlength: 250, visible: ko.observable(true), required: false });
    this.CnpjTransportadoraMichelin = PropertyEntity({ text: "CNPJ Transportadora:", maxlength: 250, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoDeCadastrosMulti = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoCadastrosMulti = PropertyEntity({ text: "*URL WS Multi:", maxlength: 250, visible: ko.observable(true), required: false });
    this.TokenIntegracaoCadastrosMulti = PropertyEntity({ text: "*Token WS Multi:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLIntegracaoCadastrosMultiSecundario = PropertyEntity({ text: "*URL WS Multi Secundário:", maxlength: 250, visible: ko.observable(true), required: false });
    this.TokenIntegracaoCadastrosMultiSecundario = PropertyEntity({ text: "*Token WS Multi Secundário:", maxlength: 250, visible: ko.observable(true), required: false });
    this.RealizarIntegracaoDePessoaParaPessoa = PropertyEntity({ text: "Habilitar integração de Pessoa x Pessoa", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.RealizarIntegracaoDeTransportadorParaEmpresa = PropertyEntity({ text: "Habilitar integração de Transportador x Empresa", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.RealizarIntegracaoDeContainer = PropertyEntity({ text: "Habilitar integração de Container", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.RealizarIntegracaoDeNavio = PropertyEntity({ text: "Habilitar integração de Navio", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.RealizarIntegracaoDeViagem = PropertyEntity({ text: "Habilitar integração de Viagem", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.RealizarIntegracaoDeCTeAnterior = PropertyEntity({ text: "Habilitar integração de CT-e Anterior", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.RealizarIntegracaoDeCTeParaComplementoOSMae = PropertyEntity({ text: "Habilitar integração de CT-e para Complemento de O.S. Mãe", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.RealizarIntegracaoDePorto = PropertyEntity({ text: "Habilitar integração de Porto", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.RealizarIntegracaoDeTipoDeContainer = PropertyEntity({ text: "Habilitar integração de Tipo de Container", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.RealizarIntegracaoDeTerminalPortuario = PropertyEntity({ text: "Habilitar integração de Terminal Portuario", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.RealizarIntegracaoDeProdutoEmbarcador = PropertyEntity({ text: "Habilitar integração de Produto Embarcador", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.EnviarDocumentacaoCTeAverbacaoInstancia = PropertyEntity({ text: "Habilitar envio de documentação CT-e/Averbação para a instância", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });

    this.PossuiIntegracaoDeTotvs = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoTotvs = PropertyEntity({ text: "*URL TOTVS:", maxlength: 250, visible: ko.observable(true), required: false });
    this.URLIntegracaoTotvsProcess = PropertyEntity({ text: "*URL TOTVS PROCESS:", maxlength: 250, visible: ko.observable(true), required: false });
    this.UsuarioTotvs = PropertyEntity({ text: "*Usuário:", maxlength: 250, visible: ko.observable(true), required: false });
    this.SenhaTotvs = PropertyEntity({ text: "*Senha:", maxlength: 250, visible: ko.observable(true), required: false });
    this.ContextoTotvs = PropertyEntity({ text: "*Contexto:", maxlength: 250, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoKuehneNagel = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnderecoFTPKuehneNagel = PropertyEntity({ text: "*Endereço:", maxlength: 150, required: false });
    this.UsuarioKuehneNagel = PropertyEntity({ text: "*Usuário:", maxlength: 50, required: false });
    this.SenhaKuehneNagel = PropertyEntity({ text: "*Senha:", maxlength: 50, required: false });
    this.DiretorioKuehneNagel = PropertyEntity({ text: "*Diretório:", maxlength: 150, required: false });
    this.PortaKuehneNagel = PropertyEntity({ text: "*Porta:", maxlength: 10, required: false });
    this.PassivoKuehneNagel = PropertyEntity({ text: "FTP Passivo", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.UtilizarSFTPKuehneNagel = PropertyEntity({ text: "SFTP", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.SSLKuehneNagel = PropertyEntity({ text: "SSL", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });

    this.PossuiIntegracaoDansales = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioDansales = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaDansales = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLIntegracaoDansales = PropertyEntity({ text: "*URL Ocorrência:", maxlength: 350, visible: ko.observable(true), required: false });
    this.UsuarioDansalesToken = PropertyEntity({ text: "Usuário Token:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaDansalesToken = PropertyEntity({ text: "Senha Token:", maxlength: 50, visible: ko.observable(true), required: false });
    this.URLIntegracaoDansalesToken = PropertyEntity({ text: "URL Token:", maxlength: 350, visible: ko.observable(true), required: false });
    this.URLIntegracaoDansalesChat = PropertyEntity({ text: "URL Chat:", maxlength: 350, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoTargetEmpresa = PropertyEntity({ text: "Habilitar integração de pagamentos?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioTargetEmpresa = PropertyEntity({ text: "*Usuário:", maxlength: 50, required: ko.observable(false) });
    this.SenhaTargetEmpresa = PropertyEntity({ text: "*Senha:", maxlength: 50, required: ko.observable(false) });
    this.URLTargetEmpresa = PropertyEntity({ text: "*URL:", maxlength: 150, required: ko.observable(false) });

    this.PossuiIntegracaoExtratta = PropertyEntity({ text: "Habilitar integração com a Extratta?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLExtratta = PropertyEntity({ text: "*URL:", maxlength: 1000, required: ko.observable(false) });
    this.TokenExtratta = PropertyEntity({ text: "*Token:", maxlength: 1000, required: ko.observable(false) });
    this.CNPJAplicacaoExtratta = PropertyEntity({ text: "*CNPJ Aplicação:", maxlength: 150, required: ko.observable(false) });
    this.CNPJEmpresaExtratta = PropertyEntity({ text: "*CNPJ Empresa:", maxlength: 150, required: ko.observable(false) });
    this.DocumentoUsuarioExtratta = PropertyEntity({ text: "*Documento Usuário:", maxlength: 500, required: ko.observable(false) });
    this.UsuarioExtratta = PropertyEntity({ text: "*CPF Usuário:", maxlength: 500, required: ko.observable(false) });


    this.IntegrarAbastecimentoComTicketLog = PropertyEntity({ text: "Integrar abastecimento com TicketLog?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CodigoClienteTicketLog = PropertyEntity({ text: "*Codigo cliente TicketLog:", maxlength: 100, required: ko.observable(false) });
    this.CodigoProdutoTicketLog = PropertyEntity({ text: "*Codigo produto TicketLog:", maxlength: 100, required: ko.observable(false) });

    this.URLRecepcaoDTe = PropertyEntity({ text: "URL Recepção DT-e:", maxlength: 200, visible: ko.observable(true), required: false });

    this.PossuiIntegracaoRavex = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLRavex = PropertyEntity({ text: "*URL:", maxlength: 150, required: ko.observable(false) });
    this.UsuarioRavex = PropertyEntity({ text: "*Usuário:", maxlength: 150, required: ko.observable(false) });
    this.SenhaRavex = PropertyEntity({ text: "*Senha:", maxlength: 150, required: ko.observable(false) });

    this.PossuiIntegracaoComprovei = PropertyEntity({ text: "Habilitar integração Carregamento e Ocorrência", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLComprovei = PropertyEntity({ text: "URL:", maxlength: 150, required: ko.observable(false) });
    this.URLBaseRestComprovei = PropertyEntity({ text: "URL Base Rest:", maxlength: 150, required: ko.observable(false) });
    this.UsuarioComprovei = PropertyEntity({ text: "Usuário:", maxlength: 150, required: ko.observable(false) });
    this.SenhaComprovei = PropertyEntity({ text: "Senha:", maxlength: 150, required: ko.observable(false) });
    this.URLIntegracaoRetornoGerarCarregamentoComprovei = PropertyEntity({ text: "URL Integracao Retorno Gerar Carregamento:", maxlength: 150, required: ko.observable(false) });
    this.URLIntegracaoRetornoConfirmacaoPedidosComprovei = PropertyEntity({ text: "URL Integracao retorno confirmação pedidos:", maxlength: 150, required: ko.observable(false) });
    this.URLIntegracaoRetornoEnviarDigitalizacaoCanhotosComprovei = PropertyEntity({ text: "URL Integracao retorno enviar digitalização canhoto:", maxlength: 150, required: ko.observable(false) });

    this.PossuiIntegracaoComproveiRota = PropertyEntity({ text: "Habilitar integração Comprovei Rota?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLComproveiRota = PropertyEntity({ text: "URL Comprovei Rota:", maxlength: 150, required: ko.observable(false) });
    this.URLBaseRestComproveiRota = PropertyEntity({ text: "URL Base Rest Comprovei Rota:", maxlength: 150, required: ko.observable(false) });
    this.UsuarioComproveiRota = PropertyEntity({ text: "Usuário Comprovei Rota:", maxlength: 150, required: ko.observable(false) });
    this.SenhaComproveiRota = PropertyEntity({ text: "Senha Comprovei Rota:", maxlength: 150, required: ko.observable(false) });

    this.URLComproveiIACanhoto = PropertyEntity({ text: "URL IA Canhoto:", maxlength: 150, required: ko.observable(false) });
    this.UsuarioComproveiIACanhoto = PropertyEntity({ text: "Usuário IA Canhoto:", maxlength: 150, required: ko.observable(false) });
    this.SenhaComproveiIACanhoto = PropertyEntity({ text: "Senha IA Canhoto:", maxlength: 150, required: ko.observable(false) });
    this.PossuiIntegracaoIACanhoto = PropertyEntity({ text: "Habilitar integração IA Canhoto?", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.PossuiIntegracaoFlora = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLFlora = PropertyEntity({ text: "*URL:", maxlength: 150, required: ko.observable(false) });
    this.EnvioCargaFlora = PropertyEntity({ text: "*Envio de Carga:", maxlength: 150, required: ko.observable(false) });
    this.UsuarioFlora = PropertyEntity({ text: "*Usuário:", maxlength: 150, required: ko.observable(false) });
    this.SenhaFlora = PropertyEntity({ text: "*Senha:", maxlength: 150, required: ko.observable(false) });
    this.CodigoFretePrevistoFlora = PropertyEntity({ text: "*Código frete previsto:", maxlength: 150, required: ko.observable(false) });
    this.CodigoFreteConfirmadoFlora = PropertyEntity({ text: "*Código frete confirmado:", maxlength: 150, required: ko.observable(false) });

    this.PossuiIntegracaoMicDta = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLMicDta = PropertyEntity({ text: "*URL:", maxlength: 150, required: ko.observable(false) });
    this.MetodoManifestacaoEmbarcaMicDta = PropertyEntity({ text: "*Método Manifestação Embarca:", maxlength: 150, required: ko.observable(false) });
    this.LicencaTNTI = PropertyEntity({ text: "*Licença TNTI:", maxlength: 150, required: ko.observable(false) });
    this.VencimentoLicencaTNTI = PropertyEntity({ text: ko.observable("Vencimento Licença TNTI:"), getType: typesKnockout.date, required: false, enable: ko.observable(false), visible: ko.observable(true) });
    this.GerarIntegracaNaEtapaDoFrete = PropertyEntity({ text: "Gerar MIC/DTA na etapa do frete?", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });

    this.PossuiIntegracaoGadle = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoGadle = PropertyEntity({ text: "*URL Gadle:", maxlength: 250, visible: ko.observable(true), required: false });
    this.TokenIntegracaoGadle = PropertyEntity({ text: "*Token:", maxlength: 250, visible: ko.observable(true), required: false });

    this.URLCobasi = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: false });
    this.APIKeyCobasi = PropertyEntity({ text: "*API Key:", maxlength: 100, visible: ko.observable(true), required: false });

    this.URLOnetrust = PropertyEntity({ text: "URL:", maxlength: 500, visible: ko.observable(true), required: false });
    this.URLObterTokenOnetrust = PropertyEntity({ text: "URL Obter Token:", maxlength: 500, visible: ko.observable(true), required: false });
    this.UrlRegularizacaoOneTrust = PropertyEntity({ text: "URL Regularização:", maxlength: 500, visible: ko.observable(true), required: false });
    this.PurposeIdOneTrust = PropertyEntity({ text: "Purpose Id:", maxlength: 2000, visible: ko.observable(true), required: false });
    this.ClientIdOneTrust = PropertyEntity({ text: "Client Id:", maxlength: 2000, visible: ko.observable(true), required: false });
    this.ClientSecretOneTrust = PropertyEntity({ text: "Client Secret:", maxlength: 2000, visible: ko.observable(true), required: false });

    this.URLSintegra = PropertyEntity({ text: "URL:", maxlength: 500, visible: ko.observable(true), required: false });
    this.TokenSintegra = PropertyEntity({ text: "Token:", maxlength: 100, visible: ko.observable(true), required: false });
    this.IntervaloConsultaSintegra = PropertyEntity({ getType: typesKnockout.int, text: "Intervalo para consulta (em meses):", visible: ko.observable(true), required: false, def: 0, val: ko.observable(0) });

    this.URLInforDoc = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.APIKeyInforDoc = PropertyEntity({ text: "*API Key:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoFTPIsis = PropertyEntity({ text: "Habilitar integração com FTP?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnderecoFTPIsis = PropertyEntity({ text: "*Endereço:", maxlength: 150, required: ko.observable(false) });
    this.UsuarioIsis = PropertyEntity({ text: "*Usuário:", maxlength: 50, required: ko.observable(false) });
    this.SenhaIsis = PropertyEntity({ text: "*Senha:", maxlength: 50, required: ko.observable(false) });
    this.DiretorioIsis = PropertyEntity({ text: "*Diretório Exportação:", maxlength: 400, required: ko.observable(false) });
    this.PortaIsis = PropertyEntity({ text: "*Porta:", maxlength: 10, required: ko.observable(false) });
    this.PassivoIsis = PropertyEntity({ text: "FTP Passivo", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.UtilizarSFTPIsis = PropertyEntity({ text: "SFTP", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.SSLIsis = PropertyEntity({ text: "SSL", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });

    this.PossuiIntegracaoLactalis = PropertyEntity({ text: "Habilitar integração com Lactalis?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoLactalis = PropertyEntity({ text: "*URL:", maxlength: 250, required: ko.observable(false) });
    this.URLAutenticacaoLactalis = PropertyEntity({ text: "*URL Autenticação", maxlength: 250, required: ko.observable(false) });
    this.UsuarioLactalis = PropertyEntity({ text: "*Usuário:", maxlength: 250, required: ko.observable(false) });
    this.SenhaLactalis = PropertyEntity({ text: "*Senha:", maxlength: 250, required: ko.observable(false) });

    this.NomenclaturaArquivoIsis = PropertyEntity({ text: "*Nomenclatura arquivo exportação:", maxlength: 100, required: ko.observable(false) });
    this.TagDiaIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoIsis.id, "#Dia"); }, type: types.event, text: "Dia" });
    this.TagMesIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoIsis.id, "#Mes"); }, type: types.event, text: "Mês" });
    this.TagAnoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoIsis.id, "#Ano"); }, type: types.event, text: "Ano" });
    this.TagHoraIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoIsis.id, "#Hora"); }, type: types.event, text: "Hora" });
    this.TagMinutoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoIsis.id, "#Minuto"); }, type: types.event, text: "Minuto" });
    this.TagSegundoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoIsis.id, "#Segundo"); }, type: types.event, text: "Segundo" });
    this.TagNumeroAtendimentoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoIsis.id, "#NumeroAtendimento"); }, type: types.event, text: "Número Atendimento" });
    this.TestarConexaoFTPIsis = PropertyEntity({ eventClick: TestarConexaoFTPIsis, type: types.event, text: "Testar Conexão" });

    this.DiretorioCarregamentoIsis = PropertyEntity({ text: "*Diretório Exportação Carregamento:", maxlength: 100, required: ko.observable(false) });
    this.NomenclaturaArquivoCarregamentoIsis = PropertyEntity({ text: "*Nomenclatura arquivo exportação carregamento:", maxlength: 100, required: ko.observable(false) });
    this.TagDiaCarregamentoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoCarregamentoIsis.id, "#Dia"); }, type: types.event, text: "Dia" });
    this.TagMesCarregamentoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoCarregamentoIsis.id, "#Mes"); }, type: types.event, text: "Mês" });
    this.TagAnoCarregamentoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoCarregamentoIsis.id, "#Ano"); }, type: types.event, text: "Ano" });
    this.TagHoraCarregamentoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoCarregamentoIsis.id, "#Hora"); }, type: types.event, text: "Hora" });
    this.TagMinutoCarregamentoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoCarregamentoIsis.id, "#Minuto"); }, type: types.event, text: "Minuto" });
    this.TagSegundoCarregamentoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoCarregamentoIsis.id, "#Segundo"); }, type: types.event, text: "Segundo" });
    this.TagNumeroCarregamentoIsis = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.NomenclaturaArquivoCarregamentoIsis.id, "#NumeroCarregamento"); }, type: types.event, text: "Número Carregamento" });

    this.PossuiIntegracaoDiageo = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnderecoDiageo = PropertyEntity({ text: "*Endereço:", maxlength: 150, required: ko.observable(false) });
    this.UsuarioDiageo = PropertyEntity({ text: "*Usuário:", maxlength: 50, required: ko.observable(false) });
    this.SenhaDiageo = PropertyEntity({ text: "*Senha:", maxlength: 50, required: ko.observable(false) });
    this.OutboundDiageo = PropertyEntity({ text: "Outbound:", maxlength: 400, required: ko.observable(false) });
    this.DiretorioInboundDiageo = PropertyEntity({ text: "Diretório Inbound:", maxlength: 400, required: ko.observable(false) });
    this.PortaDiageo = PropertyEntity({ text: "*Porta:", maxlength: 10, required: ko.observable(false) });
    this.PassivoDiageo = PropertyEntity({ text: "FTP Passivo", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.UtilizarSFTPDiageo = PropertyEntity({ text: "SFTP", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.SSLDiageo = PropertyEntity({ text: "SSL", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });

    this.PossuiIntegracaoP44 = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioP44 = PropertyEntity({ text: "*Usuário:", maxlength: 50, required: ko.observable(false) });
    this.SenhaP44 = PropertyEntity({ text: "*Senha:", maxlength: 50, required: ko.observable(false) });
    this.ClientIdP44 = PropertyEntity({ text: "*Client ID:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretP44 = PropertyEntity({ text: "*Client Secret:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAplicacaoP44 = PropertyEntity({ text: "*URL Aplicação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoP44 = PropertyEntity({ text: "*URL Autenticação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoPatioP44 = PropertyEntity({ text: "*URL Integração Pátio:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });


    this.URLMagalu = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenMagalu = PropertyEntity({ text: "*Token:", maxlength: 2000, visible: ko.observable(true), required: ko.observable(false) });

    this.URLGSW = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioGSW = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaGSW = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.CodigoInicialConsultaXMLCTeGSW = PropertyEntity({ text: "*Código inicial da consulta de XML de CT-e:", getType: typesKnockout.int, visible: ko.observable(true), required: ko.observable(false) });

    this.URLArquivei = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.KeyArquivei = PropertyEntity({ text: "*Key:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.IDArquivei = PropertyEntity({ text: "*ID:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.CodigoInicialConsultaXMLCTeArquivei = PropertyEntity({ text: "*Código inicial da consulta de XML de CT-e:", getType: typesKnockout.int, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoItalac = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLItalac = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioItalac = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaItalac = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoItalacFatura = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLItalacFatura = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioItalacFatura = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaItalacFatura = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoPager = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoPager = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioPager = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaPager = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });

    this.ConfiguracoesIntegracaoCTASmart = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListConfiguracoesIntegracaoCTASmart = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });


    this.URLAutenticacaoHavan = PropertyEntity({ text: "*URL Autenticação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLEnvioOcorrenciaHavan = PropertyEntity({ text: "*URL Envio Ocorrência:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioHavan = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaHavan = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });

    this.TipoIntegracaoOAuth = PropertyEntity({ text: "Tipo Integração:", visible: ko.observable(true), required: ko.observable(false), getType: typesKnockout.select, val: ko.observable(""), options: EnumTipoIntegracaoOAuth.obterOpcoes(), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: Global.ObterOpcoesBooleano("Ativo", "Inativo"), def: true, text: "Situação:", visible: ko.observable(false) });
    this.URLContabilizacaoFrimesa = PropertyEntity({ text: "*URL Autenticação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioFrimesa = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaFrimesa = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientID = PropertyEntity({ text: "ClientID:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecret = PropertyEntity({ text: "ClientSecret:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.AccessToken = PropertyEntity({ text: "AccessToken:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.Scope = PropertyEntity({ text: "Scope:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoFrota162 = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioFrota162 = PropertyEntity({ text: "*Usuario:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaFrota162 = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.URLFrota162 = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenFrota162 = PropertyEntity({ text: "*Token:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });
    this.SecretKeyFrota162 = PropertyEntity({ text: "*Secret Key:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.CompanyIdFrota162 = PropertyEntity({ text: "*Company ID:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });

    this.AccessKeyDexco = PropertyEntity({ text: "*Acceskey:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) })
    this.FoType = PropertyEntity({ text: "*FoType:", maxlength: 4, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlDexco = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioDexco = PropertyEntity({ text: "*Usuario:", maxlength: 250, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaDexco = PropertyEntity({ text: "*Senha:", maxlength: 250, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoIntercab = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarIntegracaoFatura = PropertyEntity({ text: "Ativar integração da Fatura", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarControleDashRegiaoOperador = PropertyEntity({ text: "Ativar controle de Dash por Região do Operador", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarIntegracaoCargaAtualParaNovo = PropertyEntity({ text: "Ativar integração das cargas (atual para novo)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarIntegracaoCargas = PropertyEntity({ text: "Ativar Integração das Cargas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarNovoHomeDash = PropertyEntity({ text: "Ativar novo Home com Dash", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegracaoDocumentacaoCarga = PropertyEntity({ text: "Integrar documentação da Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarIntegracaoCartaCorrecao = PropertyEntity({ text: "Ativar Integração de Carta de Correção", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CodigoTipoOperacao = PropertyEntity({ text: "Cód. Tipo da Operação ", maxlength: 50, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.URLIntercab = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenIntercab = PropertyEntity({ text: "*Token:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });
    this.AtivarIntegracaoMercante = PropertyEntity({ text: "Ativar integração do mercante", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarIntegracaoCteManual = PropertyEntity({ text: "Ativar Integração do CT-e Manual", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarIntegracaoCancelamentoCarga = PropertyEntity({ text: "Ativar a integração do cancelamento de carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarIntegracaoOcorrencias = PropertyEntity({ text: "Ativar a integração das ocorrências", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarIntegracaoMDFeAquaviario = PropertyEntity({ text: "Ativar Integração do MDF-e Aquaviário", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.BuscarTipoServicoModeloDocumentoVinculadoCarga = PropertyEntity({ text: "Buscar Tipo de Serviço pelo modelo do documento vinculado na carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DefinirModalPeloTipoCarga = PropertyEntity({ text: "Definir o modal pelo Tipo de Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ModificarTimelineDeAcordoComTipoServicoDocumento = PropertyEntity({ text: "Modificar timeline de acordo com o Tipo de Serviço do documento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarTimelineIntegracaoFaturaCarga = PropertyEntity({ text: "Habilitar timeline da integração da fatura na carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarTimelineFaturamentoCarga = PropertyEntity({ text: "Habilitar timeline de faturamento na carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarTimelineMercanteCarga = PropertyEntity({ text: "Habilitar timeline do mercante na carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarTimelineMDFeAquaviario = PropertyEntity({ text: "Habiltar Timeline do MDF-e Aquaviario", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarNovosFiltrosConsultaCarga = PropertyEntity({ text: "Ativar Novos Filtros para Consulta de Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AjustarLayoutFiltrosTelaCarga = PropertyEntity({ text: "Ajustar Layout dos Filtros na Tela de Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ModificarTimelineIntegracaoCarga = PropertyEntity({ text: "Modificar Timeline de Integração da Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarTimelineCargaPortoPorto = PropertyEntity({ text: "Habilitar Timeline para carga Porto x Porto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarPreFiltrosTelaCargaIntercab = PropertyEntity({ text: "Ativar pré-filtros para a tela da Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarTimelineCargaPorta = PropertyEntity({ text: "Habilitar Timeline para carga Porta", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarTimelineCargaSVMProprio = PropertyEntity({ text: "Habilitar Timeline para carga SVM Próprio", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.QuantidadeDiasParaDataInicialIntercab = PropertyEntity({ text: "Quantidade Dias para Data Inicial:", getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.SituacoesCargaIntercab = PropertyEntity({ text: "Situações da Carga:", getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoCargaMercante.obterOpcoes(), def: [] });
    this.HabilitarTimelineCargaFeeder = PropertyEntity({ text: "Habilitar Timeline para carga Feeder", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RemoverObrigacaoCodigoEmbarcacaoCadastroNavioIntercab = PropertyEntity({ text: "Remover Obrigação do Código da Embarcação do Cadastro de Navio", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarGeracaoCCePelaRolagemWS = PropertyEntity({ text: "Ativar geração de CCe pela rolagem via WS", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.SelecionarTipoOperacaoIntercab = PropertyEntity({ text: "Selecionar Tipo de Operação Padrão", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TipoOperacaoIntercab = PropertyEntity({ text: "*Tipo de Operação Padrão:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });

    this.TipoCargaPadraoIntercab = PropertyEntity({ text: "*Tipo de Carga Padrão:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoProtheus = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLAutenticacaoProtheus = PropertyEntity({ text: "*URL Autenticação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioProtheus = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaProtheus = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoUnilever = PropertyEntity({ text: "Habilitar Integração?", getType: typesKnockout.bool, val: ko.observable(true), def: false });
    this.ClientIDIntegracaoUnilever = PropertyEntity({ text: "*Client ID:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretIntegracaoUnilever = PropertyEntity({ text: "*Client Secret:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.IntegrarDadosValePedagio = PropertyEntity({ text: "Integrar dados de Vale Pedágio", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegrarAvancoParaEmissao = PropertyEntity({ text: "Integrar Avanço para Emissão", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegrarValorPreCalculo = PropertyEntity({ text: "Integrar Valor de Pré Cálculo", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegrarLotePagamento = PropertyEntity({ text: "Integrar Lote Pagamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegrarCanhoto = PropertyEntity({ text: "Integrar Canhoto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegrarCancelamentoPagamento = PropertyEntity({ text: "Integrar Cancelamento de Pagamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoAvancoParaEmissao = PropertyEntity({ text: "URL Integração de Avanço para Emissão:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoProvisaoUnilever = PropertyEntity({ text: "URL Integração de Provisão:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoRetornoUnilever = PropertyEntity({ text: "URL Integração de Retorno:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoTravamentoDTUnilever = PropertyEntity({ text: "URL Integração de Travamento DT:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoValorPreCalculoUnilever = PropertyEntity({ text: "URL Integração Valor de Pré Cálculo:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCancelamento = PropertyEntity({ text: "URL Integração cancelamento:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.IntegrarLeilaoManual = PropertyEntity({ text: "Integrar Leilão Manual", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoLeilaoManual = PropertyEntity({ text: "URL Integração Leilão Manual:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoEscrituracaoRetorno = PropertyEntity({ text: "URL Integração Escrituração Retorno:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoLotePagamento = PropertyEntity({ text: "URL Integração Lote Pagamento:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCancelamentoProvisao = PropertyEntity({ text: "URL Integração Cancelamento Provisão:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCanhoto = PropertyEntity({ text: "URL Integração canhoto:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCancelamentoPagamento = PropertyEntity({ text: "URL Integração Cancelamento Pagamento:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoOcorrencia = PropertyEntity({ text: "URL Integração Ocorrências:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoSimonetti = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLEnviaOcorrenciaSimonetti = PropertyEntity({ text: "*URL envio Ocorrencia:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoMarisa = PropertyEntity({ text: "Habilitar Integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsuarioMarisa = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaMarisa = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlMarisa = PropertyEntity({ text: "*Url:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.EnderecoIntegracaoTabelaMarisa = PropertyEntity({ text: "*Endereço Integração Tabela:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioIntegracaoTabelaMarisa = PropertyEntity({ text: "*Usuário Integração Tabela:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaIntegracaoTabelaMarisa = PropertyEntity({ text: "*Senha Integração Tabela:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    this.UrlAutenticacaoNstech = PropertyEntity({ text: "URL Autenticação:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaAutenticacaoNstech = PropertyEntity({ text: "Senha autenticação:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.IDAutenticacaoNstech = PropertyEntity({ text: "ID Autenticação:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlIntegracaoSMNstech = PropertyEntity({ text: "URL Integração SM:", maxlength: 650, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlIntegracaoVerificacaoCadastral = PropertyEntity({ text: "URL Verificação Cadastral (veiculo/Motorista):", maxlength: 850, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlIntegracaoSolicitacaoCadastral = PropertyEntity({ text: "URL Solicitação Cadastral (veiculo/Motorista):", maxlength: 850, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoDeca = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLAutenticacaoDeca = PropertyEntity({ text: "URL Autenticação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioDeca = PropertyEntity({ text: "Usuário:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaDeca = PropertyEntity({ text: "Senha:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.PossuiIntegracaoBalancaDeca = PropertyEntity({ text: "Habilitar integração balança?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLBalancaDeca = PropertyEntity({ text: "*URL Balança:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenBalancaDeca = PropertyEntity({ text: "*Token Balança:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.URLInicioViagemDeca = PropertyEntity({ text: "URL Início de Viagem:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoVLI = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IDAutenticacaoVLI = PropertyEntity({ text: "ID Autenticação:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaAutenticacaoVLI = PropertyEntity({ text: "Senha autenticação:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlAutenticacaoVLI = PropertyEntity({ text: "URL Autenticacao:", maxlength: 650, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlIntegracaoRastreamentoVLI = PropertyEntity({ text: "*URL Integração Rastreamento:", maxlength: 850, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlIntegracaoCarregamento = PropertyEntity({ text: "*URL Integração Carregamento (utiliza data atual):", maxlength: 850, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlIntegracaoDescarregamentoPortosValeVLI = PropertyEntity({ text: "URL Integração Descarregamento Portos Vale:", maxlength: 850, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlIntegracaoDescarregamentoVLI = PropertyEntity({ text: "URL Integração Descarregamento Ferroviario (utiliza data atual):", maxlength: 850, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoMarilan = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLMarilan = PropertyEntity({ text: "URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLMarilanChamadoOcorrencia = PropertyEntity({ text: "URL Chamado Ocorrência:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioMarilan = PropertyEntity({ text: "Usuário:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaMarilan = PropertyEntity({ text: "Senha:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoArcelorMittal = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLOcorrenciaArcelorMittal = PropertyEntity({ text: "*URL Ocorrências:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLConfirmarAvancoTransporteArcelorMittal = PropertyEntity({ text: "*URL Confirmar Avanço Transporte:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAtualizarNFeAprovada = PropertyEntity({ text: "*URL Atualizar NFe Aprovada:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioArcelorMittal = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaArcelorMittal = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.URLDadosTransporteSAP = PropertyEntity({ text: "URL Dados Transporte SAP:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLRetornoAdicionarPedidoEmLote = PropertyEntity({ text: "URL Retorno Adicionar Pedido Em Lote:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoEMP = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.BoostrapServersEMP = PropertyEntity({ text: "*Boostrap Servers:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.GroupIdEMP = PropertyEntity({ text: "Group ID:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioEMP = PropertyEntity({ text: "*Usuário:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaEMP = PropertyEntity({ text: "*Senha:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.AtivarEnvioCTesAnterioresEMP = PropertyEntity({ text: "Ativar envio dos CT-es anteriores", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicCTesAnterioresEMP = PropertyEntity({ text: "*Topic CT-es Anteriores:", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicCTesAnterioresEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.TopicBuscarCTesEMP = PropertyEntity({ text: "*Topic Buscar CT-es:", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicBuscarCTesEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.TopicBuscarFaturaCTeEMP = PropertyEntity({ text: "*Topic Buscar Fatura CT-e:", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicBuscarFaturaCTeEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.TopicBuscarCargaEMP = PropertyEntity({ text: "*Topic Buscar Carga:", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicBuscarCargaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarIntegracaoCargaEMP = PropertyEntity({ text: "Ativar Integração da Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoCargaEMP = PropertyEntity({ text: "*Topic Envio da Integração dos CT-es da Carga", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoCargaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.TopicEnvioIntegracaoDadosCargaEMP = PropertyEntity({ text: "*Topic Envio da Integração dos dados da Carga", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoDadosCargaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarIntegracaoCancelamentoCargaEMP = PropertyEntity({ text: "Ativar Integração Cancelamento da Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoCancelamentoCargaEMP = PropertyEntity({ text: "*Topic Envio da Integração do Cancelamento da Carga", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoCancelamentoCargaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarIntegracaoOcorrenciaEMP = PropertyEntity({ text: "Ativar Integração da Ocorrência", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoOcorrenciaEMP = PropertyEntity({ text: "*Topic Envio da Integração da Ocorrência", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoOcorrenciaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarIntegracaoCancelamentoOcorrenciaEMP = PropertyEntity({ text: "Ativar Integração do Cancelamento da Ocorrência", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoCancelamentoOcorrenciaEMP = PropertyEntity({ text: "*Topic Envio da Integração do Cancelamento da Ocorrência", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoCancelamentoOcorrenciaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarIntegracaoCTeManualEMP = PropertyEntity({ text: "Ativar Integração do CT-e Manual", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoCTeManualEMP = PropertyEntity({ text: "*Topic Envio da Integração da Carga", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoCTeManualEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.TopicEnvioIntegracaoCancelamentoCTeManualEMP = PropertyEntity({ text: "*Topic Envio da Integração Cancelamento Manual", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoCancelamentoCTeManualEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarIntegracaoFaturaEMP = PropertyEntity({ text: "Ativar Integração da Fatura", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoFaturaEMP = PropertyEntity({ text: "*Topic Envio da Integração da Fatura", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoFaturaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarIntegracaoCancelamentoFaturaEMP = PropertyEntity({ text: "Ativar Integração do Cancelamento da Fatura", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoCancelamentoFaturaEMP = PropertyEntity({ text: "*Topic Envio da Integração do Cancelamento da Fatura", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoCancelamentoFaturaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarIntegracaoCartaCorrecaoEMP = PropertyEntity({ text: "Ativar Integração da Carta de Correção", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoCartaCorrecaoEMP = PropertyEntity({ text: "*Topic Envio da Integração da Carta de Correção", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.TipoAVRO = PropertyEntity({ text: "*Tipo AVRO:", visible: ko.observable(false), required: ko.observable(false), getType: typesKnockout.select, val: ko.observable(""), options: EnumTipoAVRO.obterOpcoes(), def: "" });
    this.EnviarTopicEnvioIntegracaoCartaCorrecaoEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarIntegracaoCancelamentoBoletoEMP = PropertyEntity({ text: "Ativar Integração do Cancelamento do Boleto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoCancelamentoBoletoEMP = PropertyEntity({ text: "*Topic Envio da Integração do Cancelamento do Boleto", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioDoCancelamentoBoletoEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });

    this.AtivarIntegracaoBooking = PropertyEntity({ text: "Ativar Integração Booking", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicBooking = PropertyEntity({ text: "*Topic Booking", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicRecebimentoIntegracaoBooking = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarLeituraHeaderBookingEMP = PropertyEntity({ text: "Ativar a leitura do header no consumo do booking?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsumerGroupBookingEMP = PropertyEntity({ text: "*ConsumerGroupBooking", maxlength: 200, visible: ko.observable(false), required: ko.observable(false) });

    this.AtivarIntegracaoContainerEMP = PropertyEntity({ text: "Ativar Integração do Container", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoContainerEMP = PropertyEntity({ text: "*Topic Envio da Integração do Container", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoContainerEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });

    this.AtivarIntegracaoNFTPEMP = PropertyEntity({ text: "Ativar Integração do NFTP", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoNFTPEMP = PropertyEntity({ text: "*Topic Envio da Integração do NFTP", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoNFTPEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });

    this.ComponenteFreteValorNFTPEMP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Componente Frete Valor do Cte", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ComponenteImpostosNFTPEMP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Componente Impostos do Cte", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ComponenteValorTotalPrestacaoNFTPEMP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Componente Valor Total da Prestação do Cte", idBtnSearch: guid(), visible: ko.observable(true) });

    this.AtivarIntegracaoRecebimentoNavioEMP = PropertyEntity({ text: "Ativar Integração de recebimento de Navio (Vessel)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicRecebimentoIntegracaoVesselEMP = PropertyEntity({ text: "*Topic para recebimento da Integração de Vessel", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicRecebimentoIntegracaoVesselEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarLeituraHeaderVesselEMP = PropertyEntity({ text: "Ativar a leitura do header no consumo do vessel?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsumerGroupVesselEMP = PropertyEntity({ text: "*ConsumerGroupVessel", maxlength: 200, visible: ko.observable(false), required: ko.observable(false) });

    this.AtivarIntegracaoRecebimentoPessoaEMP = PropertyEntity({ text: "Ativar Integração de recebimento de Pessoa (Customer)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicRecebimentoIntegracaoCustomerEMP = PropertyEntity({ text: "*Topic para recebimento da Integração de Customer", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicRecebimentoIntegracaoCustomerEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarLeituraHeaderCustomerEMP = PropertyEntity({ text: "Ativar leitura do header no consumo do Customer", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsumerGroupCustomerEMP = PropertyEntity({ text: "*ConsumerGroupCustomer", maxlength: 200, visible: ko.observable(false), required: ko.observable(false) });

    this.AtivarIntegracaoRecebimentoScheduleEMP = PropertyEntity({ text: "Ativar Integração de recebimento de Schedule", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicRecebimentoIntegracaoScheduleEMP = PropertyEntity({ text: "*Topic para recebimento da Integração de Schedule", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicRecebimentoIntegracaoScheduleEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarLeituraHeaderScheduleEMP = PropertyEntity({ text: "Ativar a leitura do header no consumo do Schedule?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsumerGroupScheduleEMP = PropertyEntity({ text: "*ConsumerGroupSchedule", maxlength: 200, visible: ko.observable(false), required: ko.observable(false) });

    this.AtivarIntegracaoRecebimentoRolagemEMP = PropertyEntity({ text: "Ativar Integração de recebimento de Rolagem", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicRecebimentoIntegracaoRolagemEMP = PropertyEntity({ text: "*Topic para recebimento da Integração de Rolagem", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicRecebimentoIntegracaoRolagemEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarLeituraHeaderRolagemEMP = PropertyEntity({ text: "Ativar a leitura do header no consumo na Rolagem?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsumerGroupRolagemEMP = PropertyEntity({ text: "*ConsumerGroupRolagem", maxlength: 200, visible: ko.observable(false), required: ko.observable(false) });

    this.AtivarIntegracaoParaSILEMP = PropertyEntity({ text: "Ativar Integração para SIL", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoParaSILEMP = PropertyEntity({ text: "*Topic Envio da Integração para SIL", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoParaSILEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });

    this.AtivarIntegracaoCEMercanteEMP = PropertyEntity({ text: "Ativar Integração CE Mercante com Portal", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoCEMercanteEMP = PropertyEntity({ text: "*Topic Envio da Integração do CE Mercante:", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoCEMercanteEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });

    this.AtivarIntegracaoBoletoEMP = PropertyEntity({ text: "Ativar Integração de Boleto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicEnvioIntegracaoBoletoEMP = PropertyEntity({ text: "*Topic Envio da Integração de Boleto", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicEnvioIntegracaoBoletoEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });

    this.AtivarEnvioIntegracaoCTEDaCargaEMP = PropertyEntity({ text: "Ativar Envio da Integração dos CT-es da Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoCTEDaCargaEMP = PropertyEntity({ text: "*Topic Envio da Integração dos CT-es da Carga", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoCTEDaCargaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarIntegracaoCancelamentoDaCargaEMP = PropertyEntity({ text: "Ativar Integração do Cancelamento da Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoCancelamentoDaCargaEMP = PropertyEntity({ text: "*Topic Envio da Integração do Cancelamento da Carga", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoCancelamentoDaCargaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarEnvioIntegracaoCTEManualEMP = PropertyEntity({ text: "Ativar Envio da Integração da CT-e Manual", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoCTEManualEMP = PropertyEntity({ text: "*Topic Envio da Integração da CT-e Manual", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoCTEManualEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarEnvioIntegracaoCancelamentoCTEManualEMP = PropertyEntity({ text: "Ativar Envio da Integração Cancelamento CT-e Manual", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoCancelamentoCTEManualEMP = PropertyEntity({ text: "*Topic Envio da Integração Cancelamento CT-e Manual", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoCancelamentoCTEManualEMP = this.EnviarTopicIntegracaoCancelamentoCarga = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarEnvioIntegracaoOcorrenciaEMP = PropertyEntity({ text: "Ativar Envio da Integração da Ocorrência", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoOcorrenciaEMP = PropertyEntity({ text: "*Topic Envio da Integração da Ocorrência", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoOcorrenciaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarEnvioIntegracaoCancelamentoOcorrenciaEMP = PropertyEntity({ text: "Ativar Envio da Integração do Cancelamento da Ocorrência", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoCancelamentoOcorrenciaEMP = PropertyEntity({ text: "*Topic Envio da Integração do Cancelamento da Ocorrência", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoCancelamentoOcorrenciaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarEnvioIntegracaoFaturaEMP = PropertyEntity({ text: "Ativar Envio da Integração da Fatura", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoFaturaEMP = PropertyEntity({ text: "*Topic Envio da Integração da Fatura", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoFaturaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarEnvioIntegracaoCancelamentoFaturaEMP = PropertyEntity({ text: "Ativar Envio da Integração do Cancelamento da Fatura", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoCancelamentoFaturaEMP = PropertyEntity({ text: "*Topic Envio da Integração do Cancelamento da Fatura", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoCancelamentoFaturaEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarEnvioIntegracaoCartaCorrecaoEMP = PropertyEntity({ text: "Ativar Envio da Integração da Carta de Correção", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoCartaCorrecaoEMP = PropertyEntity({ text: "*Topic Envio da Integração da Carta de Correção", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoCartaCorrecaoEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarEnvioIntegracaoBoletoEMP = PropertyEntity({ text: "Ativar Envio da Integração do Boleto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoBoletoEMP = PropertyEntity({ text: "*Topic Envio da Integração do Boleto", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoBoletoEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.AtivarEnvioIntegracaoCancelamentoBoletoEMP = PropertyEntity({ text: "Ativar Integração do Cancelamento do Boleto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TopicIntegracaoCancelamentoBoletoEMP = PropertyEntity({ text: "*Topic Envio da Integração do Cancelamento do Boleto", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarTopicIntegracaoCancelamentoBoletoEMP = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Retina" });
    this.EnviarNoLayoutAvroDoPortalEMP = PropertyEntity({ text: "Enviar no layout do AVRO do Portal", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.ModificarConexaoParaRetina = PropertyEntity({ text: "Modificar conexão para Recebimento pelo Retina", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ModificarConexaoParaEnvioRetina = PropertyEntity({ text: "Modificar conexão para Envio ao Retina", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.TipoCargaPadraoEMP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de carga padrão:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.GroupIDRetina = PropertyEntity({ text: "*Group ID Retina", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.BootstrapServerRetina = PropertyEntity({ text: "*Bootstrap Server Retina", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLSchemaRegistryRetina = PropertyEntity({ text: "*URL Schema Registry Retina", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioServerRetina = PropertyEntity({ text: "*Usuário Server Retina", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioSchemaRegistryRetina = PropertyEntity({ text: "*Usuário Schema Registry Retina", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaServerRetina = PropertyEntity({ text: "*Senha Server Retina", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaSchemaRegistryRetina = PropertyEntity({ text: "*Senha Schema Registry Retina", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.CertificadoCRTServerRetina = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Certificado CRT Server Retina", val: ko.observable(""), visible: ko.observable(true), required: ko.observable(false) });
    this.CertificadoP12SchemaRegistryRetina = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Certificado P12 Schema Registry Retina", val: ko.observable(""), visible: ko.observable(true), required: ko.observable(false) });

    this.UrlSchemaRegistry = PropertyEntity({ text: "*Url SchemaRegistry", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.UsuarioSchemaRegistry = PropertyEntity({ text: "*Usuário SchemaRegistry", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.SenhaSchemaRegistry = PropertyEntity({ text: "*Senha SchemaRegistry", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.AtivarEnvioSerializaçãoEMP = PropertyEntity({ text: "Ativar envio em serialização", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarIntegracaoComObjetoUnitoParaTodosTopics = PropertyEntity({ text: "Ativar envio com o mesmo objeto em todos os topics", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.AtivarLeituraHeadersConsumoKeyEMP = PropertyEntity({ text: "Key Header para consumo:", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });
    this.AtivarLeituraHeadersConsumoValueEMP = PropertyEntity({ text: "Value Header para consumo:", maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });

    this.PossuiIntegracaoTicketLog = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLTicketLog = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioTicketLog = PropertyEntity({ text: "Usuário:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaTicketLog = PropertyEntity({ text: "Senha:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.CodigoClienteTicketLog = PropertyEntity({ text: "Código Cliente:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.ChaveAutorizacaoTicketLog = PropertyEntity({ text: "Chave Autorização:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.HorasConsultaTicketLog = PropertyEntity({ text: "*Horas Consulta:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ConfiguracaoAbastecimentoTicketLog = PropertyEntity({ text: "*Config. Imp. Abastecimento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });

    this.ValidacaoTAGDigitalCom = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Validação de TAG", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.EnviarDataCriacaoVendaPedidoAbaAdicionaisIntegracao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Enviar data de criação da venda do pedido(aba adicionais) na integração", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.EndpointDigitalCom = PropertyEntity({ text: "Endpoint:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenDigitalCom = PropertyEntity({ text: "Token:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.CNPJLogin = PropertyEntity({ text: "CNPJ Login: ", val: ko.observable(""), def: "", getType: typesKnockout.cnpj, enable: ko.observable(true), required: false });
    this.UsuarioDigitalCom = PropertyEntity({ text: "Client Id:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaDigitalCom = PropertyEntity({ text: "Client Secret:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlAutenticacaoDigitalCom = PropertyEntity({ text: "URL Autenticação", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoObtencaoCNPJTransportadora = PropertyEntity({ text: "Tipo de obtenção do CNPJ da Transportadora:", val: ko.observable(EnumTipoObtencaoCNPJTransportadora.Carga), options: EnumTipoObtencaoCNPJTransportadora.obterOpcoes(), def: EnumTipoObtencaoCNPJTransportadora.Carga, visible: ko.observable(true) });

    this.PossuiIntegracaoLBC = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoLBC = PropertyEntity({ text: "* URL:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoLBCAnexo = PropertyEntity({ text: "* URL Anexo:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoLBCCustoFixo = PropertyEntity({ text: "* URL Custo Fixo:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoLBCTabelaFreteCliente = PropertyEntity({ text: "* URL Valores da Tabela de Frete:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioLBC = PropertyEntity({ text: "Usuário:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaLBC = PropertyEntity({ text: "Senha:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoTecnorisk = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoTecnorisk = PropertyEntity({ text: "*URL:", maxlength: 300, visible: ko.observable(true), required: ko.observable(false) });
    this.IDPGR = PropertyEntity({ text: "*ID do PGR:", getType: typesKnockout.int, visible: ko.observable(true), required: ko.observable(false) });
    this.IDPropriedadeMonitoramento = PropertyEntity({ text: "*ID da Propriedade do Monitoramento:", getType: typesKnockout.int, visible: ko.observable(true), required: ko.observable(false) });
    this.CargaMercadoria = PropertyEntity({ text: "*Carga Mercadoria:", getType: typesKnockout.int, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioTecnorisk = PropertyEntity({ text: "*Usuário:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaTecnorisk = PropertyEntity({ text: "*Senha:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoDestinadosSAP = PropertyEntity({ text: "Habilitar Integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ClientIDIntegracaoDestinadosSAP = PropertyEntity({ text: "*Client ID:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretIntegracaoDestinadosSAP = PropertyEntity({ text: "*Client Secret:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoXMLDestinadosSAP = PropertyEntity({ text: "URL Integração XML:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoStatusDestinadosSAP = PropertyEntity({ text: "URL Integração status:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoNeokohm = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoNeokohm = PropertyEntity({ text: "*URL:", maxlength: 400, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenNeokohm = PropertyEntity({ text: "*Token:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoMoniloc = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioFTPMoniloc = PropertyEntity({ text: "Usuário FTP:", maxlength: 400, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaFTPMoniloc = PropertyEntity({ text: "Senha FTP:", maxlength: 400, visible: ko.observable(true), required: ko.observable(false) });
    this.PortaFTPMoniloc = PropertyEntity({ text: "Porta FTP:", maxlength: 400, visible: ko.observable(true), required: ko.observable(false) });
    this.DiretorioConsumoCargasDiariasMoniloc = PropertyEntity({ text: "Diretório Consumo Cargas Diárias:", maxlength: 400, visible: ko.observable(true), required: ko.observable(false) });
    this.DiretorioConsumoMoniloc = PropertyEntity({ text: "Diretório Consumo:", maxlength: 400, visible: ko.observable(true), required: ko.observable(false) });
    this.DiretorioEnvioCVAMoniloc = PropertyEntity({ text: "Diretório Envio CVA:", maxlength: 400, visible: ko.observable(true), required: ko.observable(false) });
    this.DiretorioRetornoCVAMoniloc = PropertyEntity({ text: "Diretório Retorno CVA:", maxlength: 400, visible: ko.observable(true), required: ko.observable(false) });
    this.FTPPassivoMoniloc = PropertyEntity({ text: "FTP Passivo", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SFPTMoniloc = PropertyEntity({ text: "SFTP", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SSLMoniloc = PropertyEntity({ text: "SSL", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HostFTPMoniloc = PropertyEntity({ text: "Host FTP:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    this.URLBBC = PropertyEntity({ text: "*URL:", maxlength: 400, visible: ko.observable(true), required: ko.observable(false) });
    this.PossuiIntegracaoViagemBBC = PropertyEntity({ text: "Habilitar integração viagem?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLViagemBBC = PropertyEntity({ text: "*URL:", maxlength: 400, visible: ko.observable(true), required: ko.observable(false) });
    this.CnpjEmpresaViagemBBC = PropertyEntity({ text: "*Cnpj Empresa:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaViagemBBC = PropertyEntity({ text: "*Senha:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretBBC = PropertyEntity({ text: "*Client Secret:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoApisulLog = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoApisulLog = PropertyEntity({ text: "Url Integração:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenApisulLog = PropertyEntity({ text: "Token:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoApisulLogEventosApisulLog = PropertyEntity({ text: "Url Integração Log Eventos:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.CNPJEmbarcadorApisulLog = PropertyEntity({ text: "CNPJ Embarcador:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.IdentificadorUnicoViagemApisulLog = PropertyEntity({ text: "Identificador único de viagem:", val: ko.observable(EnumIdentificadorUnicoViagemApiSulLog.CodIntegracaoCidadeUF), options: EnumIdentificadorUnicoViagemApiSulLog.obterOpcoes(), def: EnumIdentificadorUnicoViagemApiSulLog.CodIntegracaoCidadeUF, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoCargaApisulLog = PropertyEntity({ text: "Tipo Carga:", val: ko.observable("DIVERSOS"), def: "DIVERSOS", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.ValorCargaOrigemApisulLog = PropertyEntity({ text: "Valor da carga origem:*", getType: typesKnockout.decimal, visible: ko.observable(true), required: ko.observable(false) });
    this.NaoUtilizarRastreadoresApisulLog = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Não Utilizar Rastreadores", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.EtapaCarga = PropertyEntity({ options: EnumSituacoesCarga.obterOpcoesIntegracaoApisul(), def: EnumSituacoesCarga.Todas, text: "Etapa", required: ko.observable(false), visible: ko.observable(false) });
    this.EnviarCodigoIntegracaoAbaCodigosIntegracaoTipoDeCargaApisulLog = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Enviar Cód. Int. da Aba Códigos de Integração do Campo Projeto", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.ConcatenarCodigoIntegracaoDoClienteCidadeEstadoApisulLog = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Concatenar Código de Integração do Cliente - Cidade - UF", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.ConcatenarCodigoIntegracaoTransporteOridemEDestinoApisulLog = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Concatenar Cód. Int. Transp. - Cód Int. Origem Cliente - Cód. Int. Destino Cliente", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.OrigemDataInicioViagem = PropertyEntity({ text: "Origem da Data de Início da Viagem:", val: ko.observable(EnumOrigemDataInicioViagem.DataEnvioIntegracao), options: EnumOrigemDataInicioViagem.obterOpcoes(), def: EnumOrigemDataInicioViagem.DataEnvioIntegracao, visible: ko.observable(true) });

    this.PossuiIntegracaoFroggr = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoFroggr = PropertyEntity({ text: "*URL Integração:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioIntegracaoFroggr = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaIntegracaoFroggr = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoSAP = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLSAP = PropertyEntity({ text: "*URL Armazena CTe:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLEnviaVendaFrete = PropertyEntity({ text: "*URL Envia Venda Frete:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLDescontoAvaria = PropertyEntity({ text: "*URL Desconto Avaria:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLSolicitacaoCancelamento = PropertyEntity({ text: "*URL Solicitação Cancelamento CIOT:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLSolicitacaoCancelamentoCTe = PropertyEntity({ text: "*URL Solicitação Cancelamento CT-e:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioSAP = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaSAP = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.RealizarIntegracaoComDadosFatura = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Realizar a integração com os dados de fatura?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoFatura = PropertyEntity({ text: "*URL Integracao Fatura:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLCriarSaldoFrete = PropertyEntity({ text: "*URL Criar Saldo Frete:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLConsultaDocumentos = PropertyEntity({ text: "*URL Consulta Documentos:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLConsultaFatura = PropertyEntity({ text: "*URL Consulta Fatura:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoEstornoFatura = PropertyEntity({ text: "*URL Estorno Fatura:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLConsultaEstornoFatura = PropertyEntity({ text: "*URL Consulta Estorno Fatura:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLEnviaVendaServicoNFSe = PropertyEntity({ text: "*URL Integracao NFSe:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlAutenticacaoTokenHUB = PropertyEntity({ text: "*URL Autenticação Token:", maxlength: 2000, visible: ko.observable(true), required: ko.observable(false) });
    this.ConexaoServiceBUSHUB = PropertyEntity({ text: "*Conexão ServiceBUS:", maxlength: 2000, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlIntegracaoHUB = PropertyEntity({ text: "*URL Integracao:", maxlength: 2000, visible: ko.observable(true), required: ko.observable(false) });
    this.IdOrganizacaoHUB = PropertyEntity({ text: "*Id Organização:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });
    this.ChaveSecretaHUB = PropertyEntity({ text: "*Chave Secreta:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoSAP_API4 = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLSAP_API4 = PropertyEntity({ text: "*URL:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioSAP_API4 = PropertyEntity({ text: "*Usuário:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaSAP_API4 = PropertyEntity({ text: "*Senha:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoYPE = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlintegracaoYpe = PropertyEntity({ text: "*URL integracao:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioYPE = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaYPE = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoOcorrencia = PropertyEntity({ text: "*URL Integracao Ocorrência:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLintegracaoRecebeDadosLaudo = PropertyEntity({ text: "*URL integracao RecebeDadosLaudo (Gestão Devolução):", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoOTM = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIDOTM = PropertyEntity({ text: "*Client ID:", val: ko.observable(""), def: "", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretOTM = PropertyEntity({ text: "*Client Secret:", val: ko.observable(""), def: "", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoLeilaoOTM = PropertyEntity({ text: "*URL integração de leilão:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoSIC = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoSIC = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.LoginSIC = PropertyEntity({ text: "*Login:", val: ko.observable(""), def: "", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaSIC = PropertyEntity({ text: "*Senha:", val: ko.observable(""), def: "", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoCadastroVeiculoSIC = PropertyEntity({ text: "Tipo Cadastro para Veículo:", val: ko.observable(""), def: "", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoCadastroMotoristaSIC = PropertyEntity({ text: "Tipo Cadastro para Motorista:", val: ko.observable(""), def: "", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoCadastroClientesSIC = PropertyEntity({ text: "Tipo Cadastro para Clientes:", val: ko.observable(""), def: "", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoCadastroTransportadoresTerceirosSIC = PropertyEntity({ text: "Tipo Cadastro para Transportadores Terceiros:", val: ko.observable(""), def: "", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.EmpresaSIC = PropertyEntity({ text: "Empresa:", val: ko.observable(""), def: "", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.RealizarIntegracaoNovosCadastrosPessoaSIC = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Realizar a integração de novos cadastros de pessoa?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoCadastroClientesTerceirosSIC = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Integrar cliente terceiros?", def: false, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoLoggi = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoLoggi = PropertyEntity({ text: "*URL Autenticação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlIntegracaoCTeLoggi = PropertyEntity({ text: "*URL Integração CTe:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIDLoggi = PropertyEntity({ text: "*Client ID:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenLoggi = PropertyEntity({ text: "Token:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLConsultaPacotes = PropertyEntity({ text: "URL Consulta Pacotes:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretLoggi = PropertyEntity({ text: "*Client Secret:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ScopeLoggi = PropertyEntity({ text: "*Scope:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoEventoEntrega = PropertyEntity({ text: "Url Integração Evento Entrega:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoEventoEntrega = PropertyEntity({ text: "Url Autenticação Evento Entrega:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoCTePagamentoLoggi = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoCTePagamentoLoggi = PropertyEntity({ text: "*URL Autenticação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIDCTePagamentoLoggi = PropertyEntity({ text: "*Client ID:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenCTePagamentoLoggi = PropertyEntity({ text: "Token:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretCTePagamentoLoggi = PropertyEntity({ text: "*Client Secret:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ScopeCTePagamentoLoggi = PropertyEntity({ text: "*Scope:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLEnvioDocumentosCTePagamentoLoggi = PropertyEntity({ text: "URL Envio Documentos:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.IntegrarCTeSubstitutoCTePagamentoLoggi = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Integrar CTe Substituto", def: false, visible: ko.observable(true), required: ko.observable(false) });


    this.PossuiIntegracaoValoresCTeLoggi = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoValoresCTeLoggi = PropertyEntity({ text: "*URL Autenticação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIDValoresCTeLoggi = PropertyEntity({ text: "*Client ID:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenValoresCTeLoggi = PropertyEntity({ text: "Token:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretValoresCTeLoggi = PropertyEntity({ text: "*Client Secret:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ScopeValoresCTeLoggi = PropertyEntity({ text: "*Scope:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLEnvioDocumentosValoresCTeLoggi = PropertyEntity({ text: "URL Envio Documentos:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });


    this.PossuiIntegracaoJJ = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoAtendimentoJJ = PropertyEntity({ text: "*URL Integração Atendimento:", val: ko.observable(""), def: "", maxlength: 250, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioJJ = PropertyEntity({ text: "*Usuario:", val: ko.observable(""), def: "", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaJJ = PropertyEntity({ text: "*Senha:", val: ko.observable(""), def: "", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoKlios = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoKlios = PropertyEntity({ text: "*URL Autenticação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioKlios = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaKlios = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.URLConsultaAnaliseConjuntoKlios = PropertyEntity({ text: " URL Consulta Analise Conjunto:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoSAPV9 = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLReciboFrete = PropertyEntity({ text: "*URL Recibo Frete:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.Usuario = PropertyEntity({ text: "*Usuário:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.Senha = PropertyEntity({ text: "*Senha:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLCancelamento = PropertyEntity({ text: "*URL Cancelamento:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });


    this.PossuiIntegracaoSAPST = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLCriarAtendimento = PropertyEntity({ text: "*URL Criar Atendimento:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.Usuario = PropertyEntity({ text: "*Usuário:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.Senha = PropertyEntity({ text: "*Senha:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLCancelamentoST = PropertyEntity({ text: "*URL Cancelamento:", maxlength: 1000, visible: ko.observable(true), required: ko.observable(false) });


    this.PossuiIntegracaoBrado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) })
    this.URLAutenticacaoBrado = PropertyEntity({ text: "*URL Autenticação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioBrado = PropertyEntity({ text: "*Usuário:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaBrado = PropertyEntity({ text: "*Senha:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.CodigoGestaoBrado = PropertyEntity({ text: "*Código de Gestão:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLEnvioDadosTransporteBrado = PropertyEntity({ text: "*URL Envio Dados Transporte:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLEnvioDocumentosEmitidosBrado = PropertyEntity({ text: "*URL Envio de Documentos Emitidos:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLCancelamentoBrado = PropertyEntity({ text: "*URL Envio Cancelamento:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoEship = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) })
    this.URLComunicacaoEship = PropertyEntity({ text: "*URL Comunicação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ApiKeyEship = PropertyEntity({ text: "*Apikey e-Ship:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoYandeh = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) })
    this.URLComunicacaoYandeh = PropertyEntity({ text: "*URL Comunicação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioYandeh = PropertyEntity({ text: "*Usuário:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaYandeh = PropertyEntity({ text: "*Senha:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoBalancaKIKI = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLBalancaKIKI = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoKMM = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLKMM = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioKMM = PropertyEntity({ text: "*Usuário:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaKMM = PropertyEntity({ text: "*Senha:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.CodGestaoKMM = PropertyEntity({ text: "*Cód. Gestão:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenTimeHoursKMM = PropertyEntity({ text: "*Token Time Hours:", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoLogvett = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioLogvett = PropertyEntity({ text: "Usuário:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaLogvett = PropertyEntity({ text: "Senha:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.URLTituloPagarLogvett = PropertyEntity({ text: "URL Título a Pagar:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLBaixarTituloLogvett = PropertyEntity({ text: "URL Baixar Título:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.AtivaAtlas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioAtlas = PropertyEntity({ text: "Usuário:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaAtlas = PropertyEntity({ text: "Senha:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAcessoAtlas = PropertyEntity({ text: "URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.CodigoClienteAtlas = PropertyEntity({ text: "Codigo Cliente:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoCalisto = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioCalisto = PropertyEntity({ text: "*Usuário:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaCalisto = PropertyEntity({ text: "*Senha:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.URLCalisto = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLContabilizacao = PropertyEntity({ text: "*URL Contabilização:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoObramax = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.EndpointObramax = PropertyEntity({ text: "*Endpoint:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenObramax = PropertyEntity({ text: "*Token:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.EndpointPedidoOcorrenciaObramax = PropertyEntity({ text: "*Endpoint Pedido Ocorrência:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.CodigoEventoCanhotoObramax = PropertyEntity({ text: "*Código Evento Canhoto:", visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoObramaxCTE = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.EndpointObramaxCTE = PropertyEntity({ text: "*Endpoint:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenObramaxCTE = PropertyEntity({ text: "*Token:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoObramaxNFE = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.EndpointObramaxNFE = PropertyEntity({ text: "*Endpoint:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenObramaxNFE = PropertyEntity({ text: "*Token:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoObramaxProvisao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.EndpointObramaxProvisao = PropertyEntity({ text: "*Endpoint:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenObramaxProvisao = PropertyEntity({ text: "*Token:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoPacoteShopee = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração de Pacotes?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.EndpointPacoteShopee = PropertyEntity({ text: "*Endpoint:", visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioShopee = PropertyEntity({ text: "*Usuario:", visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaShopee = PropertyEntity({ text: "*Senha:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoElectrolux = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioElectrolux = PropertyEntity({ text: "*Usuário:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaElectrolux = PropertyEntity({ text: "*Senha:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.URLConembElectrolux = PropertyEntity({ text: "*URL CONEMB:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.LayoutEDIConembElectrolux = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Layout EDI CONEMB:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });
    this.URLOcorrenElectrolux = PropertyEntity({ text: "*URL OCORREN:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.LayoutEDIOcorrenElectrolux = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Layout EDI OCORREN:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });
    this.URLNotfisListaElectrolux = PropertyEntity({ text: "*URL Notfis pendentes:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLNotfisDetalhadaElectrolux = PropertyEntity({ text: "*URL Notfis detalhado:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoWhatsApp = PropertyEntity({ text: "Habilitar integração com WhatsApp?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TokenWhatsApp = PropertyEntity({ text: "*Token de acesso:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.IdContaWhatsApp = PropertyEntity({ text: "*Identificação da conta do WhatsApp Business:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.IdNumeroTelefoneWhatsApp = PropertyEntity({ text: "*Identificação do número de telefone:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.IdAplicativoWhatsApp = PropertyEntity({ text: "*ID do aplicativo:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoLoggiFaturas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLLoggiFaturas = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioLoggiFaturas = PropertyEntity({ text: "*Usuário:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaLoggiFaturas = PropertyEntity({ text: "*Senha:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.NumeroMaterialLoggiFaturas = PropertyEntity({ text: "*Material de produção:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoRuntec = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLRuntec = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioRuntec = PropertyEntity({ text: "*Usuário:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaRuntec = PropertyEntity({ text: "*Senha:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoCTeAnterioresLoggi = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoCTeAnterioresLoggi = PropertyEntity({ text: "*URL Autenticação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIDCTeAnterioresLoggi = PropertyEntity({ text: "*Client ID:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretCTeAnterioresLoggi = PropertyEntity({ text: "*Client Secret:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ScopeCTeAnterioresLoggi = PropertyEntity({ text: "*Scope:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLEnvioDocumentosCTeAnterioresLoggi = PropertyEntity({ text: "*URL Envio Documentos:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoATSLog = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoATSLog = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioIntegracaoATSLog = PropertyEntity({ text: "*Usuário:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaIntegracaoATSLog = PropertyEntity({ text: "*Senha:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SecretKeyIntegracaoATSLog = PropertyEntity({ text: "*Secret Key:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.CNPJCompanyIntegracaoATSLog = PropertyEntity({ text: "*CNPJ Integração:", val: ko.observable(""), def: "", maxlength: 20, visible: ko.observable(true), required: ko.observable(false) });
    this.NomeCompanyIntegracaoATSLog = PropertyEntity({ text: "*Nome Integração:", val: ko.observable(""), def: "", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });
    this.LocalidadeIntegracaoATSLog = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Gerais.Geral.Cidade.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });

    this.PossuiIntegracaoCamil = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCamil = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ApiKeyCamil = PropertyEntity({ text: "*API Key:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoBuntech = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoBuntech = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLProvisao = PropertyEntity({ text: "*URL Provisão:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoRouteasy = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoRouteasy = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.APIKeyIntegracaoRouteasy = PropertyEntity({ text: "*API Key:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ConfiguracaoLoads = PropertyEntity({ text: "Configuração Loads:", val: ko.observable(""), def: "", maxlength: 100, visible: ko.observable(true), required: ko.observable(false), enabled: ko.observable(false) });
    this.quantidadeTagsInseridas = ko.observable(0);
    this.TagPesoLoads = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.ConfiguracaoLoads.id, "#Peso"); }, type: types.event, text: "Peso", enabled: ko.observable(true) });
    this.TagVolumeLoads = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.ConfiguracaoLoads.id, "#Volume"); }, type: types.event, text: "Volume", enabled: ko.observable(true) });
    this.TagValorLoads = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.ConfiguracaoLoads.id, "#Valor"); }, type: types.event, text: "Valor", enabled: ko.observable(true) });
    this.TagLitroLoads = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.ConfiguracaoLoads.id, "#Litro"); }, type: types.event, text: "Litro", enabled: ko.observable(true) });
    this.TagUnidadeLoads = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.ConfiguracaoLoads.id, "#Unidade"); }, type: types.event, text: "Unidade", enabled: ko.observable(true) });
    this.TagBagLoads = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.ConfiguracaoLoads.id, "#Bag"); }, type: types.event, text: "Bag", enabled: ko.observable(true) });
    this.TagSacLoads = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.ConfiguracaoLoads.id, "#Sac"); }, type: types.event, text: "Sac", enabled: ko.observable(true) });
    this.TagPallets = PropertyEntity({ eventClick: function () { InserirTag(_configuracaoIntegracao.ConfiguracaoLoads.id, "#Pallet"); }, type: types.event, text: "Pallet", enabled: ko.observable(true) });
    this.LimparTags = {
        text: "Limpar",
        click: function () {
            _configuracaoIntegracao.ConfiguracaoLoads.required(false);
            _configuracaoIntegracao.ConfiguracaoLoads.val("");
            _configuracaoIntegracao.TagPesoLoads.enabled(true);
            _configuracaoIntegracao.TagVolumeLoads.enabled(true);
            _configuracaoIntegracao.TagValorLoads.enabled(true);
            _configuracaoIntegracao.TagLitroLoads.enabled(true);
            _configuracaoIntegracao.TagUnidadeLoads.enabled(true);
            _configuracaoIntegracao.TagBagLoads.enabled(true);
            _configuracaoIntegracao.TagSacLoads.enabled(true);
            _configuracaoIntegracao.TagPallets.enabled(true);
        }
    };

    this.PossuiIntegracaoConfirmaFacil = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLConfirmaFacil = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.EmailConfirmaFacil = PropertyEntity({ text: "*Email:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaConfirmaFacil = PropertyEntity({ text: "*Senha:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.IDClienteConfirmaFacil = PropertyEntity({ text: "ID Cliente (Opcional):", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.IDProdutoConfirmaFacil = PropertyEntity({ text: "ID Produto (Opcional):", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoCebrace = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCebrace = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.APIKeyIntegracaoCebrace = PropertyEntity({ text: "*API Key:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoCebrace = PropertyEntity({ text: "*URL Autenticação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoBind = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoBind = PropertyEntity({ text: "*URL", maxlength: 250, visible: ko.observable(true), required: ko.observable(false) });
    this.APIKeyIntegracaoBind = PropertyEntity({ text: "*API Key:", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoMondelez = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLDrivinMondelez = PropertyEntity({ text: "*URL Drivin:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ApiKeyDrivinMondelez = PropertyEntity({ text: "*API Key:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.ApiKeyDrivinLegadoMondelez = PropertyEntity({ text: "Api Key (Legado):", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoGrupoSC = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoGrupoSC = PropertyEntity({ text: "*URL Integração:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ApiKeyGrupoSC = PropertyEntity({ text: "*API Key:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoFusion = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoPedidoFusion = PropertyEntity({ text: "*URL Integração Pedido:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCargaFusion = PropertyEntity({ text: "*URL Integração Carga:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenFusion = PropertyEntity({ text: "*Token:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoTrafegus = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCargaTrafegus = PropertyEntity({ text: "*URL Integração Carga:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.PGRTrafegus = PropertyEntity({ text: "PGR:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioTrafegus = PropertyEntity({ text: "*Usuário:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaTrafegus = PropertyEntity({ text: "*Senha:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoMars = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCargaCTeMars = PropertyEntity({ text: "*URL Integração carga CTe:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoMars = PropertyEntity({ text: "*URL Autenticação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCanhotoMars = PropertyEntity({ text: "*URL Integração Canhotos:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIDMars = PropertyEntity({ text: "*Client id:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretMars = PropertyEntity({ text: "*Client secret:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCancelamentosCargas = PropertyEntity({ text: "*URL de Integração para Cancelamentos de Cargas:", maxlength: 2000, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoCancelamentosCargas = PropertyEntity({ text: "*URL Autenticação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIDCancelamentosCargas = PropertyEntity({ text: "*Client id:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretCancelamentosCargas = PropertyEntity({ text: "*Client secret:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    this.GerarUrlAcessoPortalMultiCliforAcessoViaToken = PropertyEntity({ text: "Gerar Url Acesso PortalMultiClifor", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.AudienciaAcessoViaToken = PropertyEntity({ text: "*Audiencia:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ChaveSecretaAcessoViaToken = PropertyEntity({ text: "*Chave Secreta:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.EmissorAcessoViaToken = PropertyEntity({ text: "*Emissor:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoTrizyEventos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoTrizyEventos = PropertyEntity({ text: "*URL", maxlength: 250, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenIntegracaoTrizyEventos = PropertyEntity({ text: "*Token:", maxlength: 1000, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoVector = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoVector = PropertyEntity({ text: "*URL", maxlength: 250, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIdIntegracaoVector = PropertyEntity({ text: "*Client ID:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretIntegracaoVector = PropertyEntity({ text: "*Client Secret:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.PossuiIntegracaoConecttec = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLConecttec = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ProviderIDConecttec = PropertyEntity({ text: "Provider ID:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.StationIDConecttec = PropertyEntity({ text: "Station ID:", val: ko.observable(""), def: "", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.PortaBrokerConecttec = PropertyEntity({ text: "Porta:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SecretKEYConecttec = PropertyEntity({ text: "Secret KEY:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    this.URLRecebimentoCallbackConecttec = PropertyEntity({ text: "*URL de recebimento de callback:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.AtualizarCallbackConecttec = PropertyEntity({ eventClick: atualizarCallbackClick, type: types.event, text: "Atualizar Callback", visible: ko.observable(true) });

    this.URLMigrate = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    //Globus
    this.PossuiIntegracaoGlobus = PropertyEntity({ text: "Habilitar integração com a Globus?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLWebServiceEscrituracaoISSGlobus = PropertyEntity({ text: "URL:", maxlength: 1000, required: ko.observable(false) });
    this.TokenEscrituracaoISSGlobus = PropertyEntity({ text: "Token:", maxlength: 1000, required: ko.observable(false) });
    this.ShortCodeEscrituracaoISSGlobus = PropertyEntity({ text: "Short Code:", maxlength: 200, required: ko.observable(false) });
    this.URLWebServiceNFSeGlobus = PropertyEntity({ text: "URL:", maxlength: 1000, required: ko.observable(false) });
    this.TokenNFSeGlobus = PropertyEntity({ text: "Token:", maxlength: 1000, required: ko.observable(false) });
    this.ShortCodeNFSeGlobus = PropertyEntity({ text: "Short Code:", maxlength: 200, required: ko.observable(false) });
    this.URLWebServiceFinanceiroGlobus = PropertyEntity({ text: "URL:", maxlength: 1000, required: ko.observable(false) });
    this.TokenFinanceiroGlobus = PropertyEntity({ text: "Token:", maxlength: 1000, required: ko.observable(false) });
    this.ShortCodeFinanceiroGlobus = PropertyEntity({ text: "Short Code:", maxlength: 200, required: ko.observable(false) });
    this.URLWebServiceXMLGlobus = PropertyEntity({ text: "URL:", maxlength: 1000, required: ko.observable(false) });
    this.TokenXMLGlobus = PropertyEntity({ text: "Token:", maxlength: 1000, required: ko.observable(false) });
    this.ShortCodeXMLGlobus = PropertyEntity({ text: "Short Code:", maxlength: 200, required: ko.observable(false) });
    this.URLWebServiceParticipanteGlobus = PropertyEntity({ text: "URL:", maxlength: 1000, required: ko.observable(false) });
    this.TokenParticipanteGlobus = PropertyEntity({ text: "Token:", maxlength: 1000, required: ko.observable(false) });
    this.ShortCodeParticipanteGlobus = PropertyEntity({ text: "Short Code:", maxlength: 200, required: ko.observable(false) });
    this.IntegrarComContabilidadeGlobus = PropertyEntity({ text: "Integrar com Contabilidade:", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegrarComEscritaFiscalGlobus = PropertyEntity({ text: "Integrar com Escrita Fiscal:", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegrarComContasPagarGlobus = PropertyEntity({ text: "Integrar com Contas a Pagar:", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IntegrarComContasReceberGlobus = PropertyEntity({ text: "Integrar com Contas a Receber:", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CodigoIntegrarComContabilidadeGlobus = PropertyEntity({ text: "Código Contabilidade:", maxlength: 5, required: ko.observable(false) });
    this.CodigoIntegrarComEscritaFiscalGlobus = PropertyEntity({ text: "Código Escrita Fiscal:", maxlength: 5, required: ko.observable(false) });
    this.CodigoIntegrarComContasPagarGlobus = PropertyEntity({ text: "Código Contas Pagar:", maxlength: 5, required: ko.observable(false) });
    this.CodigoIntegrarComContasReceberGlobus = PropertyEntity({ text: "Código Contas Receber:", maxlength: 5, required: ko.observable(false) });
    this.SistemaIntegrarComContabilidadeGlobus = PropertyEntity({ text: "Sistema Contabilidade:", maxlength: 5, required: ko.observable(false) });
    this.SistemaIntegrarComEscritaFiscalGlobus = PropertyEntity({ text: "Sistema Escrita Fiscal:", maxlength: 5, required: ko.observable(false) });
    this.SistemaIntegrarComContasPagarGlobus = PropertyEntity({ text: "Sistema Contas Pagar:", maxlength: 5, required: ko.observable(false) });
    this.SistemaIntegrarComContasReceberGlobus = PropertyEntity({ text: "Sistema Contas Receber:", maxlength: 5, required: ko.observable(false) });
    this.UsuarioGlobus = PropertyEntity({ text: "Usuario:", maxlength: 200, required: ko.observable(false) });
    this.GrupoGlobus = PropertyEntity({ text: "Grupo:", maxlength: 200, required: ko.observable(false) });

    //TransSat
    this.PossuiIntegracaoTransSat = PropertyEntity({ text: "Habilitar integração com a TransSat?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLWebServiceTransSat = PropertyEntity({ text: "*URL:", maxlength: 500, required: ko.observable(false) });
    this.TokenTransSat = PropertyEntity({ text: "*Token:", maxlength: 1000, required: ko.observable(false) });
    this.EmailParaReceberRetornoDaGRTransSat = PropertyEntity({ text: "E-mail para retorno da TransSat:", maxlength: 100, visible: ko.observable(true), required: false });

    //FS
    this.PossuiIntegracaoFS = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLAutenticacaoFS = PropertyEntity({ text: "*URL Autenticação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoFS = PropertyEntity({ text: "*URL Integração:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIDFS = PropertyEntity({ text: "*Client ID:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretFS = PropertyEntity({ text: "*Client Secret:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    // Vedacit
    this.PossuiIntegracaoVedacit = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoVedacit = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioIntegracaoVedacit = PropertyEntity({ text: "*Usuário:", val: ko.observable(""), def: "", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaIntegracaoVedacit = PropertyEntity({ text: "*Senha:", val: ko.observable(""), def: "", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCargaVedacit = PropertyEntity({ text: "*URL Integração Carga:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioIntegracaoCargaVedacit = PropertyEntity({ text: "*Usuário:", val: ko.observable(""), def: "", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaIntegracaoCargaVedacit = PropertyEntity({ text: "*Senha:", val: ko.observable(""), def: "", maxlength: 150, visible: ko.observable(true), required: ko.observable(false) });

    // JDE Faturas
    this.PossuiIntegracaoJDEFaturas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoJDEFaturas = PropertyEntity({ text: "*URL:", maxlength: 250, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioIntegracaoJDEFaturas = PropertyEntity({ text: "*Usuário:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaIntegracaoJDEFaturas = PropertyEntity({ text: "*Senha:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    // Olfar

    this.PossuiIntegracaoOlfar = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Habilitar integração?", def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoOlfar = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    // Efesus
    this.PossuiIntegracaoEfesus = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLAutenticacaoEfesus = PropertyEntity({ text: "*URL Autenticação:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioEfesus = PropertyEntity({ text: "*Usuário:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaEfesus = PropertyEntity({ text: "*Senha:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });

    //SalesForce
    this.PossuiIntegracaoSalesforce = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLBaseSalesforce = PropertyEntity({ text: "URL Base", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URITokenSalesforce = PropertyEntity({ text: "Endpoint Token", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIDSalesforce = PropertyEntity({ text: "Client ID", getType: typesKnockout.string, val: ko.observable(""), def: "", visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretSalesforce = PropertyEntity({ text: "Client Secret", getType: typesKnockout.string, val: ko.observable(""), def: "", visible: ko.observable(true), required: ko.observable(false) });
    this.URICasoDevolucaoSalesforce = PropertyEntity({ text: "Endpoint Caso Devolução", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });

    // Cassol
    this.PossuiIntegracaoCassol = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TokenCassol = PropertyEntity({ text: "*Token:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.URLCassol = PropertyEntity({ text: "*URL:", maxlength: 2000, visible: ko.observable(true), required: ko.observable(false) });

    // WeberChile
    this.PossuiIntegracaoWeberChile = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoWeberChile = PropertyEntity({ text: "*URL Integração:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoWeberChile = PropertyEntity({ text: "*URL Autenticação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.APIKeyWeberChile = PropertyEntity({ text: "*API Key:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientIDWeberChile = PropertyEntity({ text: "*Client ID:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.ClientSecretWeberChile = PropertyEntity({ text: "*Client Secret:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    // PortalCabotagem
    this.AtivarIntegracaoPortalAzureStorage = PropertyEntity({ text: "Ativar Integração com Portal Azure Storage", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Container = PropertyEntity({ text: "Container:", maxlength: 200, visible: ko.observable(true), required: false });
    this.StorageAccount = PropertyEntity({ text: "Storage account:", maxlength: 200, visible: ko.observable(true), required: false });
    this.URL = PropertyEntity({ text: "*URL:", maxlength: 200, visible: ko.observable(true), required: false });
    this.ClienteID = PropertyEntity({ text: "Cliente ID:", maxlength: 200, visible: ko.observable(true), required: false });
    this.Secret = PropertyEntity({ text: "Secret:", maxlength: 200, visible: ko.observable(true), required: false });
    this.AtivarEnvioPDFFatura = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Ativar envio do PDF da Fatura", def: false, visible: ko.observable(true) });
    this.AtivarEnvioPDFCTE = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Ativar envio do PDF do CT-e", def: false, visible: ko.observable(true) });
    this.AtivarEnvioPDFBOLETO = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Ativar envio do PDF do Boleto", def: false, visible: ko.observable(true), required: ko.observable(false) })
    this.AtivarEnvioXMLCTE = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Ativar envio do xml do CT-e", def: false, visible: ko.observable(true), required: ko.observable(false) })

    //Sistema Transben
    this.PossuiIntegracaoSistemaTransben = PropertyEntity({ text: "Habilitar integração com o Sistema Transben?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoSistemaTransben = PropertyEntity({ text: "*URL:", maxlength: 1000, required: ko.observable(false) });
    this.UsuarioIntegracaoSistemaTransben = PropertyEntity({ text: "*Usuário:", maxlength: 50, required: ko.observable(false) });
    this.SenhaIntegracaoSistemaTransben = PropertyEntity({ text: "*Senha:", maxlength: 50, required: ko.observable(false) });
    this.EnviarDadosCargaParaSistemaTransben = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Enviar dados da carga para sistema da Transben", def: false, visible: ko.observable(true) });

    //ATS Smart Web
    this.PossuiIntegracaoATSSmartWeb = PropertyEntity({ text: "Habilitar integração com o ATS Smart Web?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.URLIntegracaoATSSmartWeb = PropertyEntity({ text: "*URL:", maxlength: 1000, required: ko.observable(false) });
    this.UsuarioIntegracaoATSSmartWeb = PropertyEntity({ text: "*Usuário:", maxlength: 150, required: ko.observable(false) });
    this.SenhaIntegracaoATSSmartWeb = PropertyEntity({ text: "*Senha:", maxlength: 150, required: ko.observable(false) });
    this.SecretKeyATSSmartWeb = PropertyEntity({ text: "*Secret Key:", maxlength: 500, required: ko.observable(false) });
    this.CNPJCompanyIntegracaoATSSmartWeb = PropertyEntity({ text: "*CNPJ Integração:", val: ko.observable(""), def: "", maxlength: 20, visible: ko.observable(true), required: ko.observable(false) });
    this.NomeCompanyIntegracaoATSSmartWeb = PropertyEntity({ text: "*Nome Integração:", val: ko.observable(""), def: "", maxlength: 100, visible: ko.observable(true), required: ko.observable(false) });
    this.LocalidadeIntegracaoATSSmartWeb = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Gerais.Geral.Cidade.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });

    //VS Track
    this.IntegracaoEtapa1CargaVSTrack = PropertyEntity({ text: "Integração Etapa 1 da carga - GR (Proprietários/Veículos/Motorista)", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.IntegracaoEtapa6CargaVSTrack = PropertyEntity({ text: "Integração Etapa 6 da carga - SM", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.URLProducaoVSTrack = PropertyEntity({ text: "*URL Produção:", maxlength: 1000, required: ko.observable(false), visible: ko.observable(true) });
    this.URLHomologacaoVSTrack = PropertyEntity({ text: "*URL Homologação:", maxlength: 1000, required: ko.observable(false), visible: ko.observable(true) });
    this.PasswordVSTrack = PropertyEntity({ text: "*Password:", maxlength: 150, required: ko.observable(false), visible: ko.observable(true) });
    this.UsernameVSTrack = PropertyEntity({ text: "*Username:", maxlength: 150, required: ko.observable(false), visible: ko.observable(true) });
    this.GrantTypeVSTrack = PropertyEntity({ text: "*Grant_Type:", maxlength: 1000, required: ko.observable(false), visible: ko.observable(true) });

    //Senior
    this.PossuiIntegracaoSenior = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLAutenticacaoSenior = PropertyEntity({ text: "URL Autenticação", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoSenior = PropertyEntity({ text: "URL Integração", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioSenior = PropertyEntity({ text: "*Usuário:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaSenior = PropertyEntity({ text: "*Senha:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });

    //YMS
    this.PossuiIntegracaoYMS = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoAutenticacaoYMS = PropertyEntity({ text: "URL Integração Autenticação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoCriacaoYMS = PropertyEntity({ text: "URL Integração Criação:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.URLCancelamentoYMS = PropertyEntity({ text: "URL Integração Cancelamento:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioYMS = PropertyEntity({ text: "Usuário:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.SenhaYMS = PropertyEntity({ text: "Senha:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.ParametrosAdicionaisYMS = PropertyEntity({ text: "Parametros Adicionais:", maxlength: 2000, visible: ko.observable(true), required: ko.observable(false) });
    this.URLIntegracaoAtualizacaoYMS = PropertyEntity({ text: "URL Integracao Atualização:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoAutenticacaoYMS = PropertyEntity({ text: "Tipo Autenticacao: ", val: ko.observable(EnumTipoAutenticacaoYMS.Todos), options: EnumTipoAutenticacaoYMS.obterOpcoes(), def: EnumTipoAutenticacaoYMS.Basic, visible: ko.observable(true) });

    //Skymark
    this.HabilitarIntegracaoSkymark = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), required: ko.observable(false) });
    this.CampoIntegracaoSkymark = PropertyEntity({ text: "*Integração:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.UrlSkymark = PropertyEntity({ text: "*URL:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ContratoSkymark = PropertyEntity({ text: "*Contrato:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ChaveUmSkymark = PropertyEntity({ text: "*Chave 1:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.ChaveDoisSkymark = PropertyEntity({ text: "*Chave 2:", val: ko.observable(""), def: "", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });

    //Listas
    this.ConfiguracoesIntegracaoTelhaNorte = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Container = PropertyEntity({ text: "Container:", maxlength: 200, visible: ko.observable(true), required: false });
    this.StorageAccount = PropertyEntity({ text: "Storage account:", maxlength: 200, visible: ko.observable(true), required: false });

    //#region Subscribe
    this.PossuiIntegracaoRaster.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioRaster.required = novoValor;
        _configuracaoIntegracao.SenhaRaster.required = novoValor;
        _configuracaoIntegracao.URLRaster.required = novoValor;
    });

    this.PossuiIntegracaoBrasilRisk.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioBrasilRisk.required = novoValor;
        _configuracaoIntegracao.SenhaBrasilRisk.required = novoValor;
        _configuracaoIntegracao.URLHomologacaoBrasilRisk.required = false;
        _configuracaoIntegracao.URLProducaoBrasilRisk.required = false;
        _configuracaoIntegracao.URLBrasilRiskGestao.required = false;
        _configuracaoIntegracao.URLBrasilRiskVeiculoMotorista.required = false;
        _configuracaoIntegracao.CNPJEmbarcadorBrasilRisk.required = false;
    });

    this.PossuiIntegracaoGNRE.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioIntegracaoGNRE.required = novoValor;
        _configuracaoIntegracao.SenhaIntegracaoGNRE.required = novoValor;
        _configuracaoIntegracao.URLIntegracaoGNRE.required = novoValor;
    });

    this.PossuiIntegracaoLogRisk.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioLogRisk.required = novoValor;
        _configuracaoIntegracao.SenhaLogRisk.required = novoValor;
        _configuracaoIntegracao.DominioLogRisk.required = novoValor;
        _configuracaoIntegracao.CNPJClienteLogRisk.required = novoValor;
    });

    this.PossuiIntegracaoMundialRisk.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioMundialRisk.required = novoValor;
        _configuracaoIntegracao.SenhaMundialRisk.required = novoValor;
        _configuracaoIntegracao.URLHomologacaoMundialRisk.required = novoValor;
        _configuracaoIntegracao.URLProducaoMundialRisk.required = novoValor;
    });

    this.PossuiIntegracaoLogiun.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioLogiun.required = novoValor;
        _configuracaoIntegracao.SenhaLogiun.required = novoValor;
        _configuracaoIntegracao.URLHomologacaoLogiun.required = novoValor;
        _configuracaoIntegracao.URLProducaoLogiun.required = novoValor;
    });

    this.PossuiIntegracaoTrizy.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TokenTrizy.required = novoValor;
        _configuracaoIntegracao.URLTrizy.required = novoValor;

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
            _configuracaoIntegracao.AgenciaTrizy.required = novoValor;
        }
        else {
            _configuracaoIntegracao.AgenciaTrizy.visible(false);
            _configuracaoIntegracao.QuantidadeEixosPadrao.visible(false);
            _configuracaoIntegracao.NaoRealizarIntegracaoPedido.visible(false);
        }
    });

    this.PossuiIntegracaoAX.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLAX.required = novoValor;
        _configuracaoIntegracao.URLAXContratoFrete.required = novoValor;
        _configuracaoIntegracao.URLAXOrdemVenda.required = novoValor;
        _configuracaoIntegracao.URLAXCompansacao.required = novoValor;
        _configuracaoIntegracao.URLAXPedido.required = novoValor;
        _configuracaoIntegracao.URLAXComplemento.required = novoValor;
        _configuracaoIntegracao.URLAXCancelamento.required = novoValor;
        _configuracaoIntegracao.UsuarioAX.required = novoValor;
        _configuracaoIntegracao.SenhaAX.required = novoValor;
        _configuracaoIntegracao.CNPJAX.required = novoValor;
    });

    this.PossuiIntegracaoPH.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioPH.required = novoValor;
        _configuracaoIntegracao.SenhaPH.required = novoValor;
        _configuracaoIntegracao.URLHomologacaoPH.required = novoValor;
        _configuracaoIntegracao.URLProducaoPH.required = novoValor;
        _configuracaoIntegracao.CNPJContadorPH.required = novoValor;
        _configuracaoIntegracao.SoftwarePH.required = novoValor;
        _configuracaoIntegracao.PortaPH.required = novoValor;
        _configuracaoIntegracao.IPSocketPH.required = novoValor;
        _configuracaoIntegracao.PortaSocketPH.required = novoValor;
    });

    this.PossuiIntegracaoNOX.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioNOX.required = novoValor;
        _configuracaoIntegracao.SenhaNOX.required = novoValor;
        _configuracaoIntegracao.TokenNOX.required = novoValor;
        _configuracaoIntegracao.URLHomologacaoNOX.required = novoValor;
        _configuracaoIntegracao.URLProducaoNOX.required = novoValor;
    });

    this.PossuiIntegracaoGoldenService.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.CodigoGoldenService.required = novoValor;
        _configuracaoIntegracao.IdGoldenService.required = novoValor;
        _configuracaoIntegracao.SenhaGoldenService.required = novoValor;
        _configuracaoIntegracao.URLHomologacaoGoldenService.required = novoValor;
        _configuracaoIntegracao.URLProducaoGoldenService.required = novoValor;
    });

    this.PossuiIntegracaoGPA.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLHomologacaoGPA.required = novoValor;
        _configuracaoIntegracao.URLProducaoGPA.required = novoValor;
    });

    this.PossuiIntegracaoKrona.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoKrona.required = novoValor;
    });

    this.PossuiIntegracaoBoticario.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoBoticario.required = novoValor;
        _configuracaoIntegracao.IntegracaoBoticarioClientId.required = novoValor;
        _configuracaoIntegracao.IntegracaoBoticarioClientSecret.required = novoValor;
    });

    this.PossuiIntegracaoInfolog.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoInfolog.required = novoValor;
    });

    this.PossuiIntegracaoSaintGobain.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoSaintGobain.required = novoValor;
    });

    this.PossuiIntegracaoUltragaz.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLAutenticacaoUltragaz.required = novoValor;
        _configuracaoIntegracao.URLIntegracaoUltragaz.required = novoValor;
        _configuracaoIntegracao.ClientSecretUltragaz.required = novoValor;
        _configuracaoIntegracao.ClientIdUltragaz.required = novoValor;
    });

    this.PossuiIntegracaoMichelin.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioMichelin.required = novoValor;
        _configuracaoIntegracao.SenhaMichelin.required = novoValor;
        _configuracaoIntegracao.URLHomologacaoMichelin.required = novoValor;
        _configuracaoIntegracao.URLProducaoMichelin.required = novoValor;
        _configuracaoIntegracao.CodigoTransportadoraMichelin.required = novoValor;
    });

    this.PossuiIntegracaoDeCadastrosMulti.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoCadastrosMulti.required = novoValor;
        _configuracaoIntegracao.TokenIntegracaoCadastrosMulti.required = novoValor;
    });

    this.PossuiIntegracaoDeTotvs.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoTotvs.required = novoValor;
        _configuracaoIntegracao.URLIntegracaoTotvsProcess.required = novoValor;
        _configuracaoIntegracao.UsuarioTotvs.required = novoValor;
        _configuracaoIntegracao.SenhaTotvs.required = novoValor;
        _configuracaoIntegracao.ContextoTotvs.required = novoValor;
    });

    this.PossuiIntegracaoGadle.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoGadle.required = novoValor;
        _configuracaoIntegracao.TokenIntegracaoGadle.required = novoValor;
    });

    this.PossuiIntegracaoKuehneNagel.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.DiretorioKuehneNagel.required = novoValor;
        _configuracaoIntegracao.EnderecoFTPKuehneNagel.required = novoValor;
        _configuracaoIntegracao.PortaKuehneNagel.required = novoValor;
        _configuracaoIntegracao.SenhaKuehneNagel.required = novoValor;
        _configuracaoIntegracao.UsuarioKuehneNagel.required = novoValor;
    });

    this.PossuiIntegracaoDansales.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoDansales.required = novoValor;
        _configuracaoIntegracao.SenhaDansales.required = novoValor;
        _configuracaoIntegracao.UsuarioDansales.required = novoValor;
    });

    this.PossuiIntegracaoTargetEmpresa.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioTargetEmpresa.required(novoValor);
        _configuracaoIntegracao.SenhaTargetEmpresa.required(novoValor);
        _configuracaoIntegracao.URLTargetEmpresa.required(novoValor);
    });

    this.PossuiIntegracaoExtratta.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLExtratta.required(novoValor);
        _configuracaoIntegracao.TokenExtratta.required(novoValor);
        _configuracaoIntegracao.CNPJAplicacaoExtratta.required(novoValor);
        _configuracaoIntegracao.CNPJEmpresaExtratta.required(novoValor);
        _configuracaoIntegracao.DocumentoUsuarioExtratta.required(novoValor);
        _configuracaoIntegracao.UsuarioExtratta.required(novoValor);
    });

    this.PossuiIntegracaoMicDta.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLMicDta.required(novoValor);
        _configuracaoIntegracao.MetodoManifestacaoEmbarcaMicDta.required(novoValor);
        _configuracaoIntegracao.LicencaTNTI.required(novoValor);
    });

    this.PossuiIntegracaoFlora.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLFlora.required(novoValor);
        _configuracaoIntegracao.SenhaFlora.required(novoValor);
        _configuracaoIntegracao.UsuarioFlora.required(novoValor);
        _configuracaoIntegracao.EnvioCargaFlora.required(novoValor);
        _configuracaoIntegracao.CodigoFretePrevistoFlora.required(novoValor);
        _configuracaoIntegracao.CodigoFreteConfirmadoFlora.required(novoValor);
    });

    this.PossuiIntegracaoRavex.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLRavex.required(novoValor);
        _configuracaoIntegracao.UsuarioRavex.required(novoValor);
        _configuracaoIntegracao.SenhaRavex.required(novoValor);
    });

    this.PossuiIntegracaoComprovei.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLComprovei.required(novoValor);
        _configuracaoIntegracao.UsuarioComprovei.required(novoValor);
        _configuracaoIntegracao.SenhaComprovei.required(novoValor);
    });

    this.PossuiIntegracaoIACanhoto.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLComproveiIACanhoto.required(novoValor);
        _configuracaoIntegracao.UsuarioComproveiIACanhoto.required(novoValor);
        _configuracaoIntegracao.SenhaComproveiIACanhoto.required(novoValor);
    });

    this.PossuiIntegracaoFTPIsis.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.DiretorioIsis.required(novoValor);
        _configuracaoIntegracao.EnderecoFTPIsis.required(novoValor);
        _configuracaoIntegracao.PortaIsis.required(novoValor);
        _configuracaoIntegracao.SenhaIsis.required(novoValor);
        _configuracaoIntegracao.UsuarioIsis.required(novoValor);
        _configuracaoIntegracao.NomenclaturaArquivoIsis.required(novoValor);
    });

    this.PossuiIntegracaoDiageo.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.EnderecoDiageo.required(novoValor);
        _configuracaoIntegracao.PortaDiageo.required(novoValor);
        _configuracaoIntegracao.SenhaDiageo.required(novoValor);
        _configuracaoIntegracao.UsuarioDiageo.required(novoValor);
    });

    this.PossuiIntegracaoP44.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioP44.required(novoValor);
        _configuracaoIntegracao.SenhaP44.required(novoValor);
        _configuracaoIntegracao.ClientIdP44.required(novoValor);
        _configuracaoIntegracao.ClientSecretP44.required(novoValor);
        _configuracaoIntegracao.URLAplicacaoP44.required(novoValor);
        _configuracaoIntegracao.URLAutenticacaoP44.required(novoValor);
        _configuracaoIntegracao.URLIntegracaoPatioP44.required(novoValor);
    });

    this.PossuiIntegracaoFrota162.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioFrota162.required(novoValor);
        _configuracaoIntegracao.SenhaFrota162.required(novoValor);
        _configuracaoIntegracao.URLFrota162.required(novoValor);
        _configuracaoIntegracao.TokenFrota162.required(novoValor);
        _configuracaoIntegracao.SecretKeyFrota162.required(novoValor);
        _configuracaoIntegracao.CompanyIdFrota162.required(novoValor);
    });
    this.PossuiIntegracaoIntercab.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntercab.required(novoValor);
        _configuracaoIntegracao.TokenIntercab.required(novoValor);
    });

    this.PossuiIntegracaoItalac.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLItalac.required(novoValor);
        _configuracaoIntegracao.UsuarioItalac.required(novoValor);
        _configuracaoIntegracao.SenhaItalac.required(novoValor);
    });

    this.PossuiIntegracaoItalacFatura.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLItalacFatura.required(novoValor);
        _configuracaoIntegracao.UsuarioItalacFatura.required(novoValor);
        _configuracaoIntegracao.SenhaItalacFatura.required(novoValor);
    });

    this.PossuiIntegracaoPager.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoPager.required(novoValor);
        _configuracaoIntegracao.UsuarioPager.required(novoValor);
        _configuracaoIntegracao.SenhaPager.required(novoValor);
    });

    this.SelecionarTipoOperacaoIntercab.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TipoOperacaoIntercab.required(novoValor);
        _configuracaoIntegracao.TipoOperacaoIntercab.visible(novoValor);
    });

    this.AtivarIntegracaoCargas.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.CodigoTipoOperacao.required(novoValor);
        _configuracaoIntegracao.CodigoTipoOperacao.visible(novoValor);
    });
    this.PossuiIntegracaoProtheus.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLAutenticacaoProtheus.required(novoValor);
        _configuracaoIntegracao.UsuarioProtheus.required(novoValor);
        _configuracaoIntegracao.SenhaProtheus.required(novoValor);
    });
    this.PossuiIntegracaoMarisa.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UsuarioMarisa.required(novoValor);
        _configuracaoIntegracao.SenhaMarisa.required(novoValor);
        _configuracaoIntegracao.UrlMarisa.required(novoValor);
        _configuracaoIntegracao.EnderecoIntegracaoTabelaMarisa.required(novoValor);
        _configuracaoIntegracao.UsuarioIntegracaoTabelaMarisa.required(novoValor);
        _configuracaoIntegracao.SenhaIntegracaoTabelaMarisa.required(novoValor);
    });

    this.PossuiIntegracaoDeca.val.subscribe(function (novoValor) {
        if (novoValor)
            _configuracaoIntegracao.URLAutenticacaoDeca.required(novoValor);
        else {
            _configuracaoIntegracao.URLAutenticacaoDeca.required(novoValor);
            _configuracaoIntegracao.UsuarioDeca.required(novoValor);
            _configuracaoIntegracao.SenhaDeca.required(novoValor);
            _configuracaoIntegracao.URLAutenticacaoDeca.val("");
        }
    });
    this.PossuiIntegracaoBalancaDeca.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLBalancaDeca.required(novoValor);
        _configuracaoIntegracao.TokenBalancaDeca.required(novoValor);
    });

    this.PossuiIntegracaoMarilan.val.subscribe(function (novoValor) {
        if (novoValor)
            _configuracaoIntegracao.URLMarilan.required(novoValor);
        else {
            _configuracaoIntegracao.URLMarilan.required(novoValor);
            _configuracaoIntegracao.UsuarioMarilan.required(novoValor);
            _configuracaoIntegracao.SenhaMarilan.required(novoValor);
            _configuracaoIntegracao.URLMarilan.val("");
        }
    });

    this.PossuiIntegracaoEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.BoostrapServersEMP.required(novoValor);
        _configuracaoIntegracao.UsuarioEMP.required(novoValor);
        _configuracaoIntegracao.SenhaEMP.required(novoValor);
        _configuracaoIntegracao.UrlSchemaRegistry.required(novoValor);
        _configuracaoIntegracao.UrlSchemaRegistry.visible(novoValor);
        _configuracaoIntegracao.UsuarioSchemaRegistry.required(novoValor);
        _configuracaoIntegracao.UsuarioSchemaRegistry.visible(novoValor);
        _configuracaoIntegracao.SenhaSchemaRegistry.required(novoValor);
        _configuracaoIntegracao.SenhaSchemaRegistry.visible(novoValor);
    });

    this.PossuiIntegracaoArcelorMittal.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLOcorrenciaArcelorMittal.required(novoValor);
        _configuracaoIntegracao.URLConfirmarAvancoTransporteArcelorMittal.required(novoValor);
        _configuracaoIntegracao.URLAtualizarNFeAprovada.required(novoValor);
        _configuracaoIntegracao.UsuarioArcelorMittal.required(novoValor);
        _configuracaoIntegracao.SenhaArcelorMittal.required(novoValor);
        _configuracaoIntegracao.URLRetornoAdicionarPedidoEmLote.required(novoValor);
    });

    this.AtivarEnvioCTesAnterioresEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicCTesAnterioresEMP.required(novoValor);
        _configuracaoIntegracao.TopicCTesAnterioresEMP.visible(novoValor);

        _configuracaoIntegracao.TopicBuscarCTesEMP.required(novoValor);
        _configuracaoIntegracao.TopicBuscarCTesEMP.visible(novoValor);

        _configuracaoIntegracao.TopicBuscarFaturaCTeEMP.required(novoValor);
        _configuracaoIntegracao.TopicBuscarFaturaCTeEMP.visible(novoValor);

        _configuracaoIntegracao.TopicBuscarCargaEMP.required(novoValor);
        _configuracaoIntegracao.TopicBuscarCargaEMP.visible(novoValor);
    });

    this.AtivarIntegracaoCargaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoCargaEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoCargaEMP.visible(novoValor);
    });

    this.AtivarIntegracaoCargaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoDadosCargaEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoDadosCargaEMP.visible(novoValor);
    });

    this.AtivarIntegracaoCancelamentoCargaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoCancelamentoCargaEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoCancelamentoCargaEMP.visible(novoValor);
    });
    this.AtivarIntegracaoOcorrenciaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoOcorrenciaEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoOcorrenciaEMP.visible(novoValor);
    });
    this.AtivarIntegracaoCancelamentoOcorrenciaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoCancelamentoOcorrenciaEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoCancelamentoOcorrenciaEMP.visible(novoValor);
    });
    this.AtivarIntegracaoCTeManualEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoCTeManualEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoCTeManualEMP.visible(novoValor);
    });
    this.AtivarIntegracaoCTeManualEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoCancelamentoCTeManualEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoCancelamentoCTeManualEMP.visible(novoValor);
    });
    this.AtivarIntegracaoFaturaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoFaturaEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoFaturaEMP.visible(novoValor);
    });
    this.AtivarIntegracaoCancelamentoFaturaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoCancelamentoFaturaEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoCancelamentoFaturaEMP.visible(novoValor);
    });
    this.AtivarIntegracaoCartaCorrecaoEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoCartaCorrecaoEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoCartaCorrecaoEMP.visible(novoValor);
        _configuracaoIntegracao.TipoAVRO.required(novoValor);
        _configuracaoIntegracao.TipoAVRO.visible(novoValor);
    });
    this.AtivarIntegracaoContainerEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoContainerEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoContainerEMP.visible(novoValor);
    });

    this.AtivarIntegracaoNFTPEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoNFTPEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoNFTPEMP.visible(novoValor);
    });

    this.AtivarIntegracaoRecebimentoNavioEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicRecebimentoIntegracaoVesselEMP.required(novoValor);
        _configuracaoIntegracao.TopicRecebimentoIntegracaoVesselEMP.visible(novoValor);
        if (_configuracaoIntegracao.AtivarLeituraHeaderVesselEMP.val() && !novoValor) {
            _configuracaoIntegracao.AtivarLeituraHeaderVesselEMP.val(false);
        }
    });

    this.AtivarIntegracaoRecebimentoPessoaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicRecebimentoIntegracaoCustomerEMP.required(novoValor);
        _configuracaoIntegracao.TopicRecebimentoIntegracaoCustomerEMP.visible(novoValor);
        if (_configuracaoIntegracao.AtivarLeituraHeaderCustomerEMP.val() && !novoValor) {
            _configuracaoIntegracao.AtivarLeituraHeaderCustomerEMP.val(false);
        }
    });

    this.AtivarIntegracaoRecebimentoScheduleEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicRecebimentoIntegracaoScheduleEMP.required(novoValor);
        _configuracaoIntegracao.TopicRecebimentoIntegracaoScheduleEMP.visible(novoValor);
        if (_configuracaoIntegracao.AtivarLeituraHeaderScheduleEMP.val() && !novoValor) {
            _configuracaoIntegracao.AtivarLeituraHeaderScheduleEMP.val(false);
        }
    });

    this.AtivarIntegracaoRecebimentoRolagemEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicRecebimentoIntegracaoRolagemEMP.required(novoValor);
        _configuracaoIntegracao.TopicRecebimentoIntegracaoRolagemEMP.visible(novoValor);
        if (_configuracaoIntegracao.AtivarLeituraHeaderRolagemEMP.val() && !novoValor) {
            _configuracaoIntegracao.AtivarLeituraHeaderRolagemEMP.val(false);
        }
    });

    this.AtivarLeituraHeaderVesselEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.ConsumerGroupVesselEMP.required(novoValor);
        _configuracaoIntegracao.ConsumerGroupVesselEMP.visible(novoValor);
    });

    this.AtivarLeituraHeaderCustomerEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.ConsumerGroupCustomerEMP.required(novoValor);
        _configuracaoIntegracao.ConsumerGroupCustomerEMP.visible(novoValor);
    });

    this.AtivarLeituraHeaderBookingEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.ConsumerGroupBookingEMP.required(novoValor);
        _configuracaoIntegracao.ConsumerGroupBookingEMP.visible(novoValor);
    });

    this.AtivarLeituraHeaderScheduleEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.ConsumerGroupScheduleEMP.required(novoValor);
        _configuracaoIntegracao.ConsumerGroupScheduleEMP.visible(novoValor);
    });


    this.AtivarLeituraHeaderRolagemEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.ConsumerGroupRolagemEMP.required(novoValor);
        _configuracaoIntegracao.ConsumerGroupRolagemEMP.visible(novoValor);
    });
    this.AtivarIntegracaoParaSILEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoParaSILEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoParaSILEMP.visible(novoValor);
    });

    this.AtivarIntegracaoCEMercanteEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoCEMercanteEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoCEMercanteEMP.visible(novoValor);
    });

    this.AtivarIntegracaoBoletoEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoBoletoEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoBoletoEMP.visible(novoValor);
    });

    this.AtivarIntegracaoCancelamentoBoletoEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicEnvioIntegracaoCancelamentoBoletoEMP.required(novoValor);
        _configuracaoIntegracao.TopicEnvioIntegracaoCancelamentoBoletoEMP.visible(novoValor);
    });

    this.PossuiIntegracaoSenior.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLAutenticacaoSenior.required(novoValor);
        _configuracaoIntegracao.URLIntegracaoSenior.required(novoValor);
        _configuracaoIntegracao.UsuarioSenior.required(novoValor);
        _configuracaoIntegracao.SenhaSenior.required(novoValor);
    });

    //this.ModificarConexaoParaRetina.val.subscribe(function (novoValor) {
    //    _configuracaoIntegracao.GroupIDRetina.required(novoValor);
    //    _configuracaoIntegracao.GroupIDRetina.visible(novoValor);
    //    _configuracaoIntegracao.BootstrapServerRetina.required(novoValor);
    //    _configuracaoIntegracao.BootstrapServerRetina.visible(novoValor);
    //    _configuracaoIntegracao.URLSchemaRegistryRetina.required(novoValor);
    //    _configuracaoIntegracao.URLSchemaRegistryRetina.visible(novoValor);
    //    _configuracaoIntegracao.UsuarioServerRetina.required(novoValor);
    //    _configuracaoIntegracao.UsuarioServerRetina.visible(novoValor);
    //    _configuracaoIntegracao.UsuarioSchemaRegistryRetina.required(novoValor);
    //    _configuracaoIntegracao.UsuarioSchemaRegistryRetina.visible(novoValor);
    //    _configuracaoIntegracao.SenhaServerRetina.required(novoValor);
    //    _configuracaoIntegracao.SenhaServerRetina.visible(novoValor);
    //    _configuracaoIntegracao.SenhaSchemaRegistryRetina.required(novoValor);
    //    _configuracaoIntegracao.SenhaSchemaRegistryRetina.visible(novoValor);
    //    _configuracaoIntegracao.CertificadoCRTServerRetina.required(novoValor);
    //    _configuracaoIntegracao.CertificadoCRTServerRetina.visible(novoValor);
    //    _configuracaoIntegracao.CertificadoP12SchemaRegistryRetina.required(novoValor);
    //    _configuracaoIntegracao.CertificadoP12SchemaRegistryRetina.visible(novoValor);
    //});

    //this.ModificarConexaoParaEnvioRetina.val.subscribe(function (novoValor) {
    //    _configuracaoIntegracao.GroupIDRetina.required(novoValor);
    //    _configuracaoIntegracao.GroupIDRetina.visible(novoValor);
    //    _configuracaoIntegracao.BootstrapServerRetina.required(novoValor);
    //    _configuracaoIntegracao.BootstrapServerRetina.visible(novoValor);
    //    _configuracaoIntegracao.URLSchemaRegistryRetina.required(novoValor);
    //    _configuracaoIntegracao.URLSchemaRegistryRetina.visible(novoValor);
    //    _configuracaoIntegracao.UsuarioServerRetina.required(novoValor);
    //    _configuracaoIntegracao.UsuarioServerRetina.visible(novoValor);
    //    _configuracaoIntegracao.UsuarioSchemaRegistryRetina.required(novoValor);
    //    _configuracaoIntegracao.UsuarioSchemaRegistryRetina.visible(novoValor);
    //    _configuracaoIntegracao.SenhaServerRetina.required(novoValor);
    //    _configuracaoIntegracao.SenhaServerRetina.visible(novoValor);
    //    _configuracaoIntegracao.SenhaSchemaRegistryRetina.required(novoValor);
    //    _configuracaoIntegracao.SenhaSchemaRegistryRetina.visible(novoValor);
    //    _configuracaoIntegracao.CertificadoCRTServerRetina.required(novoValor);
    //    _configuracaoIntegracao.CertificadoCRTServerRetina.visible(novoValor);
    //    _configuracaoIntegracao.CertificadoP12SchemaRegistryRetina.required(novoValor);
    //    _configuracaoIntegracao.CertificadoP12SchemaRegistryRetina.visible(novoValor);
    //});

    this.PossuiIntegracaoTicketLog.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLTicketLog.required(novoValor);
        _configuracaoIntegracao.HorasConsultaTicketLog.required(novoValor);
        _configuracaoIntegracao.ConfiguracaoAbastecimentoTicketLog.required(novoValor);
    });

    this.PossuiIntegracaoTecnorisk.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoTecnorisk.required(novoValor);
        _configuracaoIntegracao.IDPGR.required(novoValor);
        _configuracaoIntegracao.IDPropriedadeMonitoramento.required(novoValor);
        _configuracaoIntegracao.CargaMercadoria.required(novoValor);
        _configuracaoIntegracao.UsuarioTecnorisk.required(novoValor);
        _configuracaoIntegracao.SenhaTecnorisk.required(novoValor);
    });

    this.PossuiIntegracaoDestinadosSAP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.ClientIDIntegracaoDestinadosSAP.required(novoValor);
        _configuracaoIntegracao.ClientSecretIntegracaoDestinadosSAP.required(novoValor);
    });

    this.PossuiIntegracaoNeokohm.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoNeokohm.required(novoValor);
        _configuracaoIntegracao.TokenNeokohm.required(novoValor);
    });

    this.PossuiIntegracaoViagemBBC.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLViagemBBC.required(novoValor);
        _configuracaoIntegracao.CnpjEmpresaViagemBBC.required(novoValor);
        _configuracaoIntegracao.SenhaViagemBBC.required(novoValor);
        _configuracaoIntegracao.ClientSecretBBC.required(novoValor);
    });

    this.PossuiIntegracaoSAP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLSAP.required(novoValor);
        _configuracaoIntegracao.URLEnviaVendaFrete.required(novoValor);
        _configuracaoIntegracao.URLDescontoAvaria.required(novoValor);
        _configuracaoIntegracao.URLCriarSaldoFrete.required(novoValor);
        _configuracaoIntegracao.URLConsultaDocumentos.required(novoValor);
        _configuracaoIntegracao.URLConsultaFatura.required(novoValor);
        _configuracaoIntegracao.URLIntegracaoEstornoFatura.required(novoValor);
        _configuracaoIntegracao.URLConsultaEstornoFatura.required(novoValor);
        _configuracaoIntegracao.URLEnviaVendaServicoNFSe.required(novoValor);

        _configuracaoIntegracao.URLSolicitacaoCancelamento.required(novoValor);
        _configuracaoIntegracao.URLSolicitacaoCancelamentoCTe.required(novoValor);
        _configuracaoIntegracao.UsuarioSAP.required(novoValor);
        _configuracaoIntegracao.SenhaSAP.required(novoValor);
    });

    this.PossuiIntegracaoYPE.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.UrlintegracaoYpe.required(novoValor);
        _configuracaoIntegracao.UsuarioYPE.required(novoValor);
        _configuracaoIntegracao.SenhaYPE.required(novoValor);
        _configuracaoIntegracao.URLIntegracaoOcorrencia.required(novoValor);
    });

    this.PossuiIntegracaoOTM.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.ClientIDOTM.required(novoValor);
        _configuracaoIntegracao.ClientSecretOTM.required(novoValor);
        _configuracaoIntegracao.URLIntegracaoLeilaoOTM.required(novoValor);
    });

    this.PossuiIntegracaoSIC.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoSIC.required(novoValor);
        _configuracaoIntegracao.LoginSIC.required(novoValor);
        _configuracaoIntegracao.SenhaSIC.required(novoValor);
    });

    this.PossuiIntegracaoLoggi.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoLoggi.required(novoValor);
        _configuracaoIntegracao.ClientIDLoggi.required(novoValor);
        _configuracaoIntegracao.ClientSecretLoggi.required(novoValor);
        if (novoValor)
            $("#liTabEventosEntregaLoggi").show();
        else
            $("#liTabEventosEntregaLoggi").hide();
    });

    this.PossuiIntegracaoCTePagamentoLoggi.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLAutenticacaoCTePagamentoLoggi.required(novoValor);
        _configuracaoIntegracao.ClientIDCTePagamentoLoggi.required(novoValor);
        _configuracaoIntegracao.ClientSecretCTePagamentoLoggi.required(novoValor);
        _configuracaoIntegracao.ScopeCTePagamentoLoggi.required(novoValor);
    });

    this.PossuiIntegracaoValoresCTeLoggi.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLAutenticacaoValoresCTeLoggi.required(novoValor);
        _configuracaoIntegracao.ClientIDValoresCTeLoggi.required(novoValor);
        _configuracaoIntegracao.ClientSecretValoresCTeLoggi.required(novoValor);
        _configuracaoIntegracao.ScopeValoresCTeLoggi.required(novoValor);
    });


    this.PossuiIntegracaoJJ.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoAtendimentoJJ.required(novoValor);
        _configuracaoIntegracao.UsuarioJJ.required(novoValor);
        _configuracaoIntegracao.SenhaJJ.required(novoValor);
    });
    this.PossuiIntegracaoKlios.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLAutenticacaoKlios.required(novoValor);
        _configuracaoIntegracao.UsuarioKlios.required(novoValor);
        _configuracaoIntegracao.SenhaKlios.required(novoValor);
    });
    this.PossuiIntegracaoSAPV9.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLReciboFrete.required(novoValor);
        _configuracaoIntegracao.Usuario.required(novoValor);
        _configuracaoIntegracao.Senha.required(novoValor);
        _configuracaoIntegracao.URLCancelamento.required(novoValor);
    });
    this.PossuiIntegracaoSAPST.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLCriarAtendimento.required(novoValor);
        _configuracaoIntegracao.Usuario.required(novoValor);
        _configuracaoIntegracao.Senha.required(novoValor);
        _configuracaoIntegracao.URLCancelamentoST.required(novoValor);
    });
    this.PossuiIntegracaoSAP_API4.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLSAP_API4.required(novoValor);
        _configuracaoIntegracao.UsuarioSAP_API4.required(novoValor);
        _configuracaoIntegracao.SenhaSAP_API4.required(novoValor);
    });
    this.RealizarIntegracaoComDadosFatura.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoFatura.required(novoValor);
    });
    this.AtivarIntegracaoBooking.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicBooking.required(novoValor);
        _configuracaoIntegracao.TopicBooking.visible(novoValor);
        if (_configuracaoIntegracao.AtivarLeituraHeaderBookingEMP.val() && !novoValor) {
            _configuracaoIntegracao.AtivarLeituraHeaderBookingEMP.val(false);
        }
        if (!novoValor) {
            _configuracaoIntegracao.TopicBooking.val('')
        }
    });
    this.PossuiIntegracaoLoggiFaturas.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLLoggiFaturas.required(novoValor);
        _configuracaoIntegracao.UsuarioLoggiFaturas.required(novoValor);
        _configuracaoIntegracao.SenhaLoggiFaturas.required(novoValor);
        _configuracaoIntegracao.NumeroMaterialLoggiFaturas.required(novoValor);
    });
    this.PossuiIntegracaoRuntec.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLRuntec.required(novoValor);
        _configuracaoIntegracao.UsuarioRuntec.required(novoValor);
        _configuracaoIntegracao.SenhaRuntec.required(novoValor);
    });

    this.PossuiIntegracaoCTeAnterioresLoggi.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLAutenticacaoCTeAnterioresLoggi.required(novoValor);
        _configuracaoIntegracao.ClientIDCTeAnterioresLoggi.required(novoValor);
        _configuracaoIntegracao.ClientSecretCTeAnterioresLoggi.required(novoValor);
        _configuracaoIntegracao.ScopeCTeAnterioresLoggi.required(novoValor);
        _configuracaoIntegracao.URLEnvioDocumentosCTeAnterioresLoggi.required(novoValor);
    });

    this.PossuiIntegracaoATSLog.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoATSLog.required(true);
        _configuracaoIntegracao.UsuarioIntegracaoATSLog.required(true);
        _configuracaoIntegracao.SenhaIntegracaoATSLog.required(true);
        _configuracaoIntegracao.SecretKeyIntegracaoATSLog.required(true);
    })

    this.PossuiIntegracaoConfirmaFacil.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLConfirmaFacil.required(novoValor);
        _configuracaoIntegracao.EmailConfirmaFacil.required(novoValor);
        _configuracaoIntegracao.SenhaConfirmaFacil.required(novoValor);
    });

    this.PossuiIntegracaoCebrace.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoCebrace.required(novoValor);
        _configuracaoIntegracao.APIKeyIntegracaoCebrace.required(novoValor);
        _configuracaoIntegracao.URLAutenticacaoCebrace.required(novoValor);
    });

    this.PossuiIntegracaoBind.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoBind.required(novoValor);
        _configuracaoIntegracao.APIKeyIntegracaoBind.required(novoValor);
    });

    this.PossuiIntegracaoTrizyEventos.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoTrizyEventos.required(novoValor);
        _configuracaoIntegracao.TokenIntegracaoTrizyEventos.required(novoValor);
    });

    this.PossuiIntegracaoVector.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoVector.required(novoValor);
        _configuracaoIntegracao.ClientIdIntegracaoVector.required(novoValor);
        _configuracaoIntegracao.ClientSecretIntegracaoVector.required(novoValor);
    });

    this.TipoIntegracaoOAuth.val.subscribe(function (novoValor) {
        let visivel = novoValor == EnumTipoIntegracaoOAuth.OAuth1_0;

        _configuracaoIntegracao.Situacao.visible(!visivel);
        _configuracaoIntegracao.ClientID.visible(!visivel);
        _configuracaoIntegracao.ClientSecret.visible(!visivel);
        _configuracaoIntegracao.AccessToken.visible(!visivel);
        _configuracaoIntegracao.Scope.visible(!visivel);

        _configuracaoIntegracao.UsuarioFrimesa.visible(visivel);
        _configuracaoIntegracao.SenhaFrimesa.visible(visivel);

        if (visivel)
            _configuracaoIntegracao.Situacao.val(false);
    });

    this.PossuiIntegracaoCamil.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoCamil.required(novoValor);
        _configuracaoIntegracao.ApiKeyCamil.required(novoValor);
    });

    this.PossuiIntegracaoRouteasy.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoRouteasy.required(novoValor);
        _configuracaoIntegracao.APIKeyIntegracaoRouteasy.required(novoValor);
    });

    this.ConfiguracaoLoads.val.subscribe(function (novoValor) {
        const tags = (novoValor || "").split("#").filter(t => t.trim() !== "");

        if (tags.length > 6) {
            const seisPrimeiras = tags.slice(0, 6).map(t => "#" + t).join("");
            _configuracaoIntegracao.ConfiguracaoLoads.val(seisPrimeiras);

            exibirMensagem(tipoMensagem.falha, "Limite de Tags", "Você só pode inserir no máximo 6 tags.");
            _configuracaoIntegracao.quantidadeTagsInseridas(6);
            return;
        }

        _configuracaoIntegracao.quantidadeTagsInseridas(tags.length);
    });

    this.PossuiIntegracaoMondelez.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLDrivinMondelez.required = novoValor;
        _configuracaoIntegracao.ApiKeyDrivinMondelez.required = novoValor;
    });

    this.PossuiIntegracaoGrupoSC.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoGrupoSC.required = novoValor;
        _configuracaoIntegracao.ApiKeyGrupoSC.required = novoValor;
    });

    this.PossuiIntegracaoFusion.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoCargaFusion.required = novoValor;
        _configuracaoIntegracao.TokenFusion.required = novoValor;
        _configuracaoIntegracao.URLIntegracaoPedidoFusion.required = novoValor;
    });

    this.PossuiIntegracaoTrafegus.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoCargaTrafegus.required = novoValor;
        _configuracaoIntegracao.UsuarioTrafegus.required = novoValor;
        _configuracaoIntegracao.SenhaTrafegus.required = novoValor;
    });

    this.PossuiIntegracaoApisulLog.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoApisulLog.required = novoValor;
        _configuracaoIntegracao.TokenApisulLog.required = novoValor;
        _configuracaoIntegracao.ValorCargaOrigemApisulLog.required = novoValor;
        _configuracaoIntegracao.TipoCargaApisulLog.required = novoValor;
        _configuracaoIntegracao.IdentificadorUnicoViagemApisulLog.required = novoValor;
    });

    this.PossuiIntegracaoConecttec.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLConecttec.required(novoValor);
        _configuracaoIntegracao.ProviderIDConecttec.required(novoValor);
        _configuracaoIntegracao.StationIDConecttec.required(novoValor);
        _configuracaoIntegracao.PortaBrokerConecttec.required(novoValor);

    });

    this.PossuiIntegracaoMars.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoCargaCTeMars.required = novoValor;
        _configuracaoIntegracao.URLAutenticacaoMars.required = novoValor;
        _configuracaoIntegracao.URLIntegracaoCanhotoMars.required = novoValor;
        _configuracaoIntegracao.ClientIDMars.required = novoValor;
        _configuracaoIntegracao.ClientSecretMars.required = novoValor;
        _configuracaoIntegracao.URLIntegracaoCancelamentosCargas.required = novoValor;
        _configuracaoIntegracao.ClientIDCancelamentosCargas.required = novoValor;
        _configuracaoIntegracao.ClientSecretCancelamentosCargas.required = novoValor;
        _configuracaoIntegracao.URLAutenticacaoCancelamentosCargas.required = novoValor;
    });

    this.PossuiIntegracaoFS.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoFS.required = novoValor;
        _configuracaoIntegracao.URLAutenticacaoFS.required = novoValor;
        _configuracaoIntegracao.ClientIDFS.required = novoValor;
        _configuracaoIntegracao.ClientSecretFS.required = novoValor;
    });

    this.PossuiIntegracaoVedacit.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoVedacit.required(novoValor);
        _configuracaoIntegracao.UsuarioIntegracaoVedacit.required(novoValor);
        _configuracaoIntegracao.SenhaIntegracaoVedacit.required(novoValor);

        _configuracaoIntegracao.URLIntegracaoCargaVedacit.required(novoValor);
        _configuracaoIntegracao.UsuarioIntegracaoCargaVedacit.required(novoValor);
        _configuracaoIntegracao.SenhaIntegracaoCargaVedacit.required(novoValor);
    });

    this.EndpointDigitalCom.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TokenDigitalCom.required(novoValor && novoValor.trim() !== "");
    });

    this.PossuiIntegracaoCassol.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TokenCassol.required(novoValor);
        _configuracaoIntegracao.URLCassol.required(novoValor);
    });

    this.PossuiIntegracaoWeberChile.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLIntegracaoWeberChile.required(novoValor);
        _configuracaoIntegracao.URLAutenticacaoWeberChile.required(novoValor);
        _configuracaoIntegracao.ClientIDWeberChile.required(novoValor);
        _configuracaoIntegracao.ClientSecretWeberChile.required(novoValor);
        _configuracaoIntegracao.APIKeyWeberChile.required(novoValor)
    });

    this.PossuiIntegracaoLactalis.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.URLAutenticacaoLactalis.required(novoValor);
        _configuracaoIntegracao.URLIntegracaoLactalis.required(novoValor);
        _configuracaoIntegracao.UsuarioLactalis.required(novoValor);
        _configuracaoIntegracao.SenhaLactalis.required(novoValor);
    });

    this.AtivarEnvioIntegracaoCTEDaCargaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoCTEDaCargaEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoCTEDaCargaEMP.visible(novoValor);
    });

    this.AtivarIntegracaoCancelamentoDaCargaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoCancelamentoDaCargaEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoCancelamentoDaCargaEMP.visible(novoValor);
    });

    this.AtivarEnvioIntegracaoCTEManualEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoCTEManualEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoCTEManualEMP.visible(novoValor);
    });

    this.AtivarEnvioIntegracaoCancelamentoCTEManualEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoCancelamentoCTEManualEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoCancelamentoCTEManualEMP.visible(novoValor);
    });

    this.AtivarEnvioIntegracaoOcorrenciaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoOcorrenciaEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoOcorrenciaEMP.visible(novoValor);
    });

    this.AtivarEnvioIntegracaoCancelamentoOcorrenciaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoCancelamentoOcorrenciaEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoCancelamentoOcorrenciaEMP.visible(novoValor);
    });


    this.AtivarEnvioIntegracaoFaturaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoFaturaEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoFaturaEMP.visible(novoValor);
    });

    this.AtivarEnvioIntegracaoCancelamentoFaturaEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoCancelamentoFaturaEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoCancelamentoFaturaEMP.visible(novoValor);
    });

    this.AtivarEnvioIntegracaoCartaCorrecaoEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoCartaCorrecaoEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoCartaCorrecaoEMP.visible(novoValor);
    });

    this.AtivarEnvioIntegracaoBoletoEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoBoletoEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoBoletoEMP.visible(novoValor);
    });

    this.AtivarEnvioIntegracaoCancelamentoBoletoEMP.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.TopicIntegracaoCancelamentoBoletoEMP.required(novoValor);
        _configuracaoIntegracao.TopicIntegracaoCancelamentoBoletoEMP.visible(novoValor);
    });

    this.VersaoEFrete.val.subscribe(function (novoValor) {
        _configuracaoIntegracao.EnviarDadosRegulatorioANTT.visible(novoValor == EnumVersaoEFrete.Versao2);
        if (novoValor == EnumVersaoEFrete.Versao1)
            _configuracaoIntegracao.EnviarDadosRegulatorioANTT.val(false);
    });
    this.IntegrarOfertasCargas.val.subscribe(novoValor => {
        this.URLIntegracaoOfertas.visible(novoValor);
        this.URLIntegracaoOfertas.required = novoValor;
        this.URLIntegracaoGrupoMotoristas.visible(novoValor);
        this.URLIntegracaoGrupoMotoristas.required = novoValor;
        if (!novoValor) {
            this.URLIntegracaoGrupoMotoristas.val("");
            this.URLIntegracaoOfertas.val("");
        }
    });
    this.NaoUtilizarRastreadoresApisulLog.val.subscribe(novoValor => {
        this.EtapaCarga.visible(novoValor);
        this.EtapaCarga.required(novoValor);
    });

    //#endregion Subscribe
};

var CRUDConfiguracaoIntegracao = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
};

function loadConfiguracaoIntegracao() {

    _configuracaoIntegracao = new ConfiguracaoIntegracao();
    KoBindings(_configuracaoIntegracao, "knockoutConfiguracaoIntegracao");

    _CRUDConfiguracaoIntegracao = new CRUDConfiguracaoIntegracao();
    KoBindings(_CRUDConfiguracaoIntegracao, "knockoutCRUDConfiguracaoIntegracao");

    HeaderAuditoria("Integracao", _configuracaoIntegracao);

    BuscarTransportadores(_configuracaoIntegracao.MatrizEFrete);
    BuscarTransportadores(_configuracaoIntegracao.EmpresaFixaPamCard);
    BuscarTransportadores(_configuracaoIntegracao.EmpresaFixaTelerisco);
    BuscarConfiguracaoAbastecimento(_configuracaoIntegracao.ConfiguracaoAbastecimentoTicketLog);
    BuscarTiposOperacao(_configuracaoIntegracao.TipoOperacaoIntercab, null, null, null, null, null, null, null, null, null, null, true);
    BuscarLayoutsEDI(_configuracaoIntegracao.LayoutEDIConembElectrolux, null, null, null, null, [EnumTipoLayoutEDI.CONEMB]);
    BuscarLayoutsEDI(_configuracaoIntegracao.LayoutEDIOcorrenElectrolux, null, null, null, null, [EnumTipoLayoutEDI.OCOREN]);
    BuscarLocalidades(_configuracaoIntegracao.LocalidadeIntegracaoATSLog);
    BuscarLocalidades(_configuracaoIntegracao.LocalidadeIntegracaoATSSmartWeb);
    BuscarTiposdeCarga(_configuracaoIntegracao.TipoCargaPadraoEMP);
    BuscarTiposdeCarga(_configuracaoIntegracao.TipoCargaPadraoIntercab);

    new BuscarComponentesDeFrete(_configuracaoIntegracao.ComponenteFreteValorNFTPEMP);
    new BuscarComponentesDeFrete(_configuracaoIntegracao.ComponenteImpostosNFTPEMP);
    new BuscarComponentesDeFrete(_configuracaoIntegracao.ComponenteValorTotalPrestacaoNFTPEMP);


    LoadConfiguracaoIntegracaoAvon();
    LoadConfiguracaoIntegracaoSAD();
    loadIntegracaoTelhaNorte();
    LoadConfiguracaoIntegracaoCTASmart();

    ObterDados();

    $('.nav-tabs a').not(function () {
        return $(this).parents('#tabEMPSub').length;
    }).click(function (e) {
        e.preventDefault();
        let elementHRef = $(this).attr('href').substring(1);
        $('#tabsIntegracoes .tab-content').each(function (i, tabContent) {
            $(tabContent).children().not(function () {
                return $(this).attr('id') === elementHRef || $(this).attr('data-el-type') === 'tab-pane-sub';
            }).each(function (z, el) {
                $(el).removeClass('active');
            });
        });

        $(this).tab('show');
    });
}

function ObterDados() {
    executarReST("Integracao/ObterDados", {}, function (r) {
        if (r.Success) {
            if (r.Data.TiposExistentes != null && r.Data.TiposExistentes.length > 0) {
                for (let i = 0; i < r.Data.TiposExistentes.length; i++) {
                    switch (r.Data.TiposExistentes[i]) {
                        case EnumTipoIntegracao.Avon:
                            $("#liTabAvon").show();
                            break;
                        case EnumTipoIntegracao.GNRE:
                            $("#liTabGNRE").show();
                            break;
                        case EnumTipoIntegracao.LogRisk:
                            $("#liTabLogRisk").show();
                            break;
                        case EnumTipoIntegracao.Natura:
                            _configuracaoIntegracao.CodigoMatrizNatura.required = true;
                            _configuracaoIntegracao.UsuarioNatura.required = true;
                            _configuracaoIntegracao.SenhaNatura.required = true;
                            $("#liTabNatura").show();
                            break;
                        case EnumTipoIntegracao.OpenTech:
                            _configuracaoIntegracao.UsuarioOpenTech.required = true;
                            _configuracaoIntegracao.SenhaOpenTech.required = true;
                            _configuracaoIntegracao.DominioOpenTech.required = true;
                            _configuracaoIntegracao.CodigoClienteOpenTech.required = true;
                            _configuracaoIntegracao.CodigoPASOpenTech.required = true;
                            _configuracaoIntegracao.URLOpenTech.required = true;
                            _configuracaoIntegracao.ValorBaseOpenTech.required = true;
                            $("#liTabOpenTech").show();
                            break;
                        //case EnumTipoIntegracao.OpenTechV10:
                        //    _configuracaoIntegracao.UsuarioOpenTech.required = true;
                        //    _configuracaoIntegracao.SenhaOpenTech.required = true;
                        //    _configuracaoIntegracao.DominioOpenTech.required = true;
                        //    _configuracaoIntegracao.CodigoClienteOpenTech.required = true;
                        //    _configuracaoIntegracao.CodigoPASOpenTech.required = true;
                        //    _configuracaoIntegracao.URLOpenTech.required = true;
                        //    _configuracaoIntegracao.ValorBaseOpenTech.required = true;
                        //    $("#liTabOpenTech").show();
                        //    break;
                        case EnumTipoIntegracao.BrasilRisk:
                            $("#liTabBrasilRisk").show();
                            break;
                        case EnumTipoIntegracao.Extratta:
                            $("#liTabExtratta").show();
                            break;
                        case EnumTipoIntegracao.BrasilRiskGestao:
                            $("#liTabBrasilRisk").show();
                            break;
                        case EnumTipoIntegracao.MundialRisk:
                            $("#liTabMundialRisk").show();
                            break;
                        case EnumTipoIntegracao.Logiun:
                            $("#liTabLogiun").show();
                            break;
                        case EnumTipoIntegracao.Trizy:
                            $("#liTabTrizy").show();
                            break;
                        case EnumTipoIntegracao.AX:
                            $("#liTabAX").show();
                            break;
                        case EnumTipoIntegracao.PH:
                            $("#liTabPH").show();
                            break;
                        case EnumTipoIntegracao.Buonny:
                            _configuracaoIntegracao.CNPJClienteBuonny.required = true;
                            _configuracaoIntegracao.TokenBuonny.required = true;
                            _configuracaoIntegracao.URLHomologacaoBuonny.required = true;
                            _configuracaoIntegracao.URLProducaoBuonny.required = true;
                            $("#liTabBuonny").show();
                            break;
                        case EnumTipoIntegracao.Avior:
                            _configuracaoIntegracao.URLAvior.required = true;
                            _configuracaoIntegracao.UsuarioAvior.required = true;
                            _configuracaoIntegracao.SenhaAvior.required = true;
                            $("#liTabAvior").show();
                            break;
                        case EnumTipoIntegracao.NOX:
                            $("#liTabNOX").show();
                            break;
                        case EnumTipoIntegracao.Carrefour:
                            $("#liTabCarrefour").show();
                            break;
                        case EnumTipoIntegracao.GoldenService:
                            $("#liTabGoldenService").show();
                            break;
                        case EnumTipoIntegracao.GPA:
                            $("#liTabGPA").show();
                            break;
                        case EnumTipoIntegracao.GPAMDFe:
                            $("#liTabGPA").show();
                            break;
                        case EnumTipoIntegracao.GPAEscrituracaoCTe:
                            $("#liTabGPA").show();
                            break;
                        case EnumTipoIntegracao.AngelLira:
                            _configuracaoIntegracao.UsuarioAngelLira.required = true;
                            _configuracaoIntegracao.SenhaAngelLira.required = true;
                            _configuracaoIntegracao.URLAngelLira.required = true;
                            $("#liTabAngelLira").show();
                            break;
                        case EnumTipoIntegracao.Ortec:
                            _configuracaoIntegracao.URLOrtec.required = true;
                            _configuracaoIntegracao.UsuarioOrtec.required = true;
                            _configuracaoIntegracao.SenhaOrtec.required = true;
                            $("#liTabOrtec").show();
                            break;
                        case EnumTipoIntegracao.Piracanjuba:
                            _configuracaoIntegracao.URLIntegracaoCanhotoPiracanjuba.required = true;
                            $("#liTabPiracanjuba").show();
                            break;
                        case EnumTipoIntegracao.Raster:
                            $("#liTabRaster").show();
                            break;
                        case EnumTipoIntegracao.UnileverFourKites:
                            _configuracaoIntegracao.URLHomologacaoUnileverFourKites.required = true;
                            _configuracaoIntegracao.URLProducaoUnileverFourKites.required = true;
                            _configuracaoIntegracao.UsuarioUnileverFourKites.required = true;
                            _configuracaoIntegracao.SenhaUnileverFourKites.required = true;
                            $("#liTabUnileverFourKites").show();
                            break;
                        case EnumTipoIntegracao.Digibee:
                            _configuracaoIntegracao.APIKeyDigibee.required = true;
                            $("#liTabDigibee").show();
                            break;
                        case EnumTipoIntegracao.SAD:
                            $("#litabSAD").show();
                            break;
                        case EnumTipoIntegracao.Telerisco:
                            $("#liTabTelerisco").show();
                            break;
                        case EnumTipoIntegracao.CargoX:
                            $("#liTabCargoX").show();
                            break;
                        case EnumTipoIntegracao.Riachuelo:
                            $("#liTabRiachuelo").show();
                            break;
                        case EnumTipoIntegracao.Krona:
                            $("#liTabKrona").show();
                            break;
                        case EnumTipoIntegracao.Boticario:
                            $("#liTabBoticario").show();
                            break;
                        case EnumTipoIntegracao.DPA:
                            $("#liTabDPA").show();
                            break;
                        case EnumTipoIntegracao.Infolog:
                            $("#liTabInfolog").show();
                            break;
                        case EnumTipoIntegracao.Toledo:
                            $("#liTabToledo").show();
                            break;
                        case EnumTipoIntegracao.Qbit:
                            $("#liTabQbit").show();
                            break;
                        case EnumTipoIntegracao.MercadoLivre:
                            _configuracaoIntegracao.URLMercadoLivre.required = true;
                            _configuracaoIntegracao.IDMercadoLivre.required = true;
                            _configuracaoIntegracao.SecretKeyMercadoLivre.required = true
                            $("#liTabMercadoLivre").show();
                            break;
                        case EnumTipoIntegracao.Ultragaz:
                            $("#liTabUltragaz").show();
                            break;
                        case EnumTipoIntegracao.SaintGobain:
                            $("#liTabSaintGobain").show();
                            break;
                        case EnumTipoIntegracao.A52:
                            _configuracaoIntegracao.URLA52.required = true;
                            _configuracaoIntegracao.CPFCNPJA52.required = true;
                            _configuracaoIntegracao.SenhaA52.required = true
                            $("#liTabA52").show();
                            break;
                        case EnumTipoIntegracao.Adagio:
                            _configuracaoIntegracao.URLAdagio.required = true;
                            _configuracaoIntegracao.EmailAdagio.required = true;
                            _configuracaoIntegracao.SenhaAdagio.required = true;
                            $("#liTabAdagio").show();
                            break;
                        case EnumTipoIntegracao.Correios:
                            $("#liTabCorreios").show();
                            break;
                        case EnumTipoIntegracao.Michelin:
                            $("#liTabMichelin").show();
                            break;
                        case EnumTipoIntegracao.CadastrosMulti:
                            $("#liTabCadastrosMulti").show();
                            break;
                        case EnumTipoIntegracao.Totvs:
                            $("#liTabTotvs").show();
                            break;
                        case EnumTipoIntegracao.KuehneNagel:
                            $("#liTabKuehneNagel").show();
                            break;
                        case EnumTipoIntegracao.Dansales:
                            $("#liTabDansales").show();
                            break;
                        case EnumTipoIntegracao.Emillenium:
                            $("#liTabEmillenium").show();
                            break;
                        case EnumTipoIntegracao.DTe:
                            $("#liTabDTe").show();
                            break;
                        case EnumTipoIntegracao.MicDta:
                            $("#liTabMicDta").show();
                            break;
                        case EnumTipoIntegracao.Ravex:
                            $("#liTabRavex").show();
                            break;
                        case EnumTipoIntegracao.TelhaNorte:
                            preencherIntegracaoTelhaNorte(r.Data.IntegracaoTelhaNorte);
                            $("#liTabTelhaNorte").show();
                            break;
                        case EnumTipoIntegracao.Gadle:
                            $("#liTabGadle").show();
                            break;
                        case EnumTipoIntegracao.Cobasi:
                            $("#liTabCobasi").show();
                            break;
                        case EnumTipoIntegracao.Onetrust:
                            $("#liTabOnetrust").show();
                            break;
                        case EnumTipoIntegracao.InforDoc:
                            _configuracaoIntegracao.URLInforDoc.required(true);
                            _configuracaoIntegracao.APIKeyInforDoc.required(true);
                            $("#liTabInforDoc").show();
                            break;
                        case EnumTipoIntegracao.Sintegra:
                            $("#liTabSintegra").show();
                            break;
                        case EnumTipoIntegracao.Isis:
                            $("#liTabIsis").show();
                            break;
                        case EnumTipoIntegracao.Magalu:
                            _configuracaoIntegracao.URLMagalu.required(true);
                            _configuracaoIntegracao.TokenMagalu.required(true);
                            $("#liTabMagalu").show();
                            break;
                        case EnumTipoIntegracao.GSW:
                            _configuracaoIntegracao.URLGSW.required(true);
                            _configuracaoIntegracao.UsuarioGSW.required(true);
                            _configuracaoIntegracao.SenhaGSW.required(true);
                            _configuracaoIntegracao.CodigoInicialConsultaXMLCTeGSW.required(true);
                            $("#liTabGSW").show();
                            break;
                        case EnumTipoIntegracao.CTASmart:
                            $("#liTabCTASmart").show();
                            break;
                        case EnumTipoIntegracao.Havan:
                            _configuracaoIntegracao.URLAutenticacaoHavan.required(true);
                            _configuracaoIntegracao.URLEnvioOcorrenciaHavan.required(true);
                            _configuracaoIntegracao.UsuarioHavan.required(true);
                            _configuracaoIntegracao.SenhaHavan.required(true);
                            $("#liTabHavan").show();
                            break;
                        case EnumTipoIntegracao.Frota162:
                            $("#liTabFrota162").show();
                            break;
                        case EnumTipoIntegracao.Dexco:
                            $("#liTabDexco").show();
                            break;
                        case EnumTipoIntegracao.Intercab:
                            $("#liTabIntercab").show();
                            break;
                        case EnumTipoIntegracao.Protheus:
                            $("#liTabProtheus").show();
                            break;
                        case EnumTipoIntegracao.Unilever:
                            $("#liTabUnilever").show();
                            break;
                        case EnumTipoIntegracao.Nstech:
                            $("#liTabNstech").show();
                            _configuracaoIntegracao.EnviarTodosDestinosBrasilRisk.visible(true);
                            break
                        case EnumTipoIntegracao.Simonetti:
                            $("#liTabSimonetti").show();
                            break;
                        case EnumTipoIntegracao.Marisa:
                            $("#liTabMarisa").show();
                        case EnumTipoIntegracao.Deca:
                            $("#liTabDeca").show();
                            break;
                        case EnumTipoIntegracao.Marilan:
                            $("#liTabMarilan").show();
                            break;
                        case EnumTipoIntegracao.VLI:
                            _configuracaoIntegracao.UrlAutenticacaoVLI.required(true);
                            _configuracaoIntegracao.UrlIntegracaoRastreamentoVLI.required(true);
                            _configuracaoIntegracao.IDAutenticacaoVLI.required(true);
                            _configuracaoIntegracao.SenhaAutenticacaoVLI.required(true);
                            $("#liTabVLI").show();
                            break;
                        case EnumTipoIntegracao.ArcelorMittal:
                            _configuracaoIntegracao.URLOcorrenciaArcelorMittal.required(true);
                            _configuracaoIntegracao.URLConfirmarAvancoTransporteArcelorMittal.required(true);
                            _configuracaoIntegracao.UsuarioArcelorMittal.required(true);
                            _configuracaoIntegracao.SenhaArcelorMittal.required(true);
                            $("#liTabArcelorMittal").show();
                            break;
                        case EnumTipoIntegracao.EMP:
                            _configuracaoIntegracao.BoostrapServersEMP.required(true);
                            _configuracaoIntegracao.UsuarioEMP.required(true);
                            _configuracaoIntegracao.SenhaEMP.required(true);
                            $("#liTabEMP").show();
                            break;
                        case EnumTipoIntegracao.TicketLog:
                            //_configuracaoIntegracao.URLTicketLog.required(true);
                            //_configuracaoIntegracao.HorasConsultaTicketLog.required(true);
                            //_configuracaoIntegracao.ConfiguracaoAbastecimentoTicketLog.required(true);
                            $("#liTabTicketLog").show();
                            break;
                        case EnumTipoIntegracao.DigitalCom:
                            _configuracaoIntegracao.ValidacaoTAGDigitalCom.required(true);
                            $("#liDigitalCom").show();
                            break;
                        case EnumTipoIntegracao.LBC:
                            _configuracaoIntegracao.URLIntegracaoLBC.required(true);
                            _configuracaoIntegracao.UsuarioLBC.required(true);
                            _configuracaoIntegracao.SenhaLBC.required(true);
                            $("#liLBC").show();
                            break;
                        case EnumTipoIntegracao.Tecnorisk:
                            $("#liTabTecnorisk").show();
                            break;
                        case EnumTipoIntegracao.DestinadosSAP:
                            $("#liTabDestinadosSAP").show();
                            break;
                        case EnumTipoIntegracao.Neokohm:
                            _configuracaoIntegracao.URLIntegracaoNeokohm.required(true);
                            _configuracaoIntegracao.TokenNeokohm.required(true);
                            $("#liTabNeokohm").show();
                            break;
                        case EnumTipoIntegracao.Moniloc:
                            $("#liTabMoniloc").show();
                            break;
                        case EnumTipoIntegracao.ApisulLog:
                            $("#liTabApisulLog").show();
                            break;
                        case EnumTipoIntegracao.BBC:
                            $("#liTabBBC").show();
                            break;
                        case EnumTipoIntegracao.SAP:
                            $("#liTabSAP").show();
                            break;
                        case EnumTipoIntegracao.SAP_API4:
                            $("#liTabSAP_API4").show();
                            break;
                        case EnumTipoIntegracao.YPE:
                            $("#liTabYPE").show();
                            break;
                        case EnumTipoIntegracao.OTM:
                            $("#liTabOTM").show();
                            break;
                        case EnumTipoIntegracao.SIC:
                            $("#liTabSIC").show();
                            break;
                        case EnumTipoIntegracao.FrimesaFrete:
                        case EnumTipoIntegracao.FrimesaValePedagio:
                            $("#liTabFrimesa").show();
                            break;
                        case EnumTipoIntegracao.HUB:
                            $("#liTabHubOfertas").show();
                            break;
                        case EnumTipoIntegracao.Loggi:
                            $("#liTabLoggi").show();
                            $("#liTabEventosEntregaLoggi").show();
                            break;
                        case EnumTipoIntegracao.CTePagamentoLoggi:
                            $("#liTabCTePagamentoLoggi").show();
                            break;
                        case EnumTipoIntegracao.JJ:
                            $("#liTabJJ").show();
                            break;
                        case EnumTipoIntegracao.Klios:
                            $("#liTabKlios").show();
                            break;
                        case EnumTipoIntegracao.SAP_V9:
                            $("#liTabSAPV9").show();
                            break;
                        case EnumTipoIntegracao.SAP_ST:
                            $("#liTabSAPST").show();
                            break;
                        case EnumTipoIntegracao.Brado:
                            $("#liTabBrado").show();
                            break;
                        case EnumTipoIntegracao.Eship:
                            $("#liTabEship").show();
                            break;
                        case EnumTipoIntegracao.Diageo:
                            $("#liTabDiageo").show();
                            break;
                        case EnumTipoIntegracao.P44:
                            $("#liTabP44").show();
                            break;
                        case EnumTipoIntegracao.Yandeh:
                            $("#liTabYandeh").show();
                            break;
                        case EnumTipoIntegracao.BalancaKIKI:
                            $("#liTabBalancaKIKI").show();
                            break;
                        case EnumTipoIntegracao.Comprovei:
                        case EnumTipoIntegracao.ComproveiRota:
                            $("#liTabComprovei").show();
                            break;
                        case EnumTipoIntegracao.KMM:
                            $("#liTabKMM").show();
                            break;
                        case EnumTipoIntegracao.Logvett:
                            $("#liTabLogvett").show();
                            break;
                        case EnumTipoIntegracao.Atlas:
                            $("#liTabAtlas").show();
                            break;
                        case EnumTipoIntegracao.Flora:
                            $("#liTabFlora").show();
                            break;
                        case EnumTipoIntegracao.Calisto:
                            $("#liTabCalisto").show();
                            break;
                        case EnumTipoIntegracao.Obramax:
                            $("#liTabObramax").show();
                            break;
                        case EnumTipoIntegracao.ObramaxCTE:
                            $("#liTabObramaxCTE").show();
                            break;
                        case EnumTipoIntegracao.ObramaxNFE:
                            $("#liTabObramaxNFE").show();
                            break;
                        case EnumTipoIntegracao.ObramaxProvisao:
                            $("#liTabObramaxProvisao").show();
                            break;
                        case EnumTipoIntegracao.Shopee:
                            $("#liTabShopee").show();
                            break;
                        case EnumTipoIntegracao.Arquivei:
                            $("#liTabArquivei").show();
                            break;
                        case EnumTipoIntegracao.Italac:
                            $("#liTabItalac").show();
                            break;
                        case EnumTipoIntegracao.ItalacFaturas:
                            $("#liTabItalacFatura").show();
                            break;
                        case EnumTipoIntegracao.Electrolux:
                            $("#liTabEletrolux").show();
                            break;
                        case EnumTipoIntegracao.WhatsApp:
                            _configuracaoIntegracao.TokenWhatsApp.required(true)
                            $("#liTabWhatsApp").show();
                            break;
                        case EnumTipoIntegracao.Froggr:
                            $("#liTabFroggr").show();
                            break;
                        case EnumTipoIntegracao.LoggiFaturas:
                            $("#liTabLoggiFaturas").show();
                            break;
                        case EnumTipoIntegracao.ValoresCTeLoggi:
                            $("#liTabValoresCTeLoggi").show();
                            break;
                        case EnumTipoIntegracao.Runtec:
                            $("#liTabRuntec").show();
                            break;
                        case EnumTipoIntegracao.CTeAnterioresLoggi:
                            $("#liTabCTeAterioresLoggi").show();
                            break;
                        case EnumTipoIntegracao.ATSLog:
                            $("#liTabATSLog").show();
                            break;
                        case EnumTipoIntegracao.Camil:
                            $("#liTabCamil").show();
                            break;
                        case EnumTipoIntegracao.Buntech:
                            $("#liTabBuntech").show();
                            break;
                        case EnumTipoIntegracao.Routeasy:
                            $("#liTabRouteasy").show();
                            break;
                        case EnumTipoIntegracao.ConfirmaFacil:
                            $("#liTabConfirmaFacil").show();
                            break;
                        case EnumTipoIntegracao.Cebrace:
                            $("#liTabCebrace").show();
                            break;
                        case EnumTipoIntegracao.Bind:
                            $("#liTabBind").show();
                            break;
                        case EnumTipoIntegracao.Mondelez:
                            $("#liTabMondelez").show();
                            break;
                        case EnumTipoIntegracao.GrupoSC:
                            $("#liTabGrupoSC").show();
                            break;
                        case EnumTipoIntegracao.Mars:
                            $("#liTabMars").show();
                            break;
                        case EnumTipoIntegracao.TrizyEventos:
                            $("#liTabTrizyEventos").show();
                            break;
                        case EnumTipoIntegracao.Vector:
                            $("#liTabVector").show();
                            break;
                        case EnumTipoIntegracao.Salesforce:
                            $("#liTabSalesforce").show();
                            break;
                        case EnumTipoIntegracao.Conecttec:
                            $("#liTabConecttec").show();
                            break;
                        case EnumTipoIntegracao.Globus:
                            $("#liTabGlobus").show();
                            break;
                        case EnumTipoIntegracao.TransSat:
                            $("#liTabTransSat").show();
                            break;
                        case EnumTipoIntegracao.FS:
                            $("#liTabFS").show();
                            break;
                        case EnumTipoIntegracao.Vedacit:
                            $("#liTabVedacit").show();
                            break;
                        case EnumTipoIntegracao.JDEFaturas:
                            $("#liTabJDEFaturas").show();
                            break;
                        case EnumTipoIntegracao.Olfar:
                            $("#liTabOlfar").show();
                            break;
                        case EnumTipoIntegracao.Migrate:
                            $("#liTabMigrate").show();
                            break;
                        case EnumTipoIntegracao.Efesus:
                            $("#litabEfesus").show();
                            break;
                        case EnumTipoIntegracao.CassolEventosEntrega:
                        case EnumTipoIntegracao.Cassol:
                            $("#liTabCassol").show();
                            break;
                        case EnumTipoIntegracao.WeberChile:
                            $("#liTabWeberChile").show();
                            break;
                        case EnumTipoIntegracao.Lactalis:
                            $("#liTabLactalis").show();
                            break;
                        case EnumTipoIntegracao.SistemaTransben:
                            $("#liTabSistemaTransben").show();
                            break;
                        case EnumTipoIntegracao.ATSSmartWeb:
                            $("#liTabATSSmartWeb").show();
                            break;
                        case EnumTipoIntegracao.Fusion:
                            $("#liTabFusion").show();
                            break;
                        case EnumTipoIntegracao.VSTrack:
                            $("#liTabVSTrack").show();
                            break;
                        case EnumTipoIntegracao.Trafegus:
                            $("#liTabTrafegus").show();
                            break;
                        case EnumTipoIntegracao.YMS:
                            $("#liTabYMS").show();
                            break;
                        case EnumTipoIntegracao.Skymark:
                            $("#liTabSkymark").show();
                            _configuracaoIntegracao.UrlSkymark.required = true;
                            _configuracaoIntegracao.CampoIntegracaoSkymark.required = true;
                            _configuracaoIntegracao.ContratoSkymark.required = true;
                            _configuracaoIntegracao.ChaveUmSkymark.required = true;
                            _configuracaoIntegracao.ChaveDoisSkymark.required = true;
                            break;
                        case EnumTipoIntegracao.Senior:
                            $("#liTabSenior").show();
                            break;
                        default:
                            break;
                    }
                }

                var objetos = [
                    null,
                    'IntegracaoNatura',
                    'IntegracaoOpentech',
                    'IntegracaoDTe',
                    'IntegracaoMundialRisk',
                    'IntegracaoLogiun',
                    'IntegracaoBuonny',
                    'IntegracaoAvior',
                    'IntegracaoNOX',
                    'IntegracaoCarrefour',
                    'IntegracaoGoldenService',
                    'IntegracaoGPA',
                    'IntegracaoOrtec',
                    'IntegracaoAPIGoogle',
                    'IntegracaoPamCard',
                    'IntegracaoPiracanjuba',
                    'IntegracaoRaster',
                    'IntegracaoUnileverFourKites',
                    'IntegracaoDigibee',
                    'IntegracaoTelerisco',
                    'IntegracaoCargoX',
                    'IntegracaoRiachuelo',
                    'IntegracaoKrona',
                    'IntegracaoInfolog',
                    'IntegracaoPH',
                    'IntegracaoBoticario',
                    'IntegracaoToledo',
                    'IntegracaoQbit',
                    'IntegracaoAdagio',
                    'IntegracaoTrizzy',
                    'IntegracaoAX',
                    'IntegracaoCobasi',
                    'IntegracaoCadastrosMulti',
                    'IntegracaoTotvs',
                    'IntegracaoAngelLira',
                    'IntegracaoA52',
                    'IntegracaoMercadoLivre',
                    'IntegracaoKuehneNagel',
                    'IntegracaoDansales',
                    'IntegracaoTarget',
                    'IntegracaoExtratta',
                    'IntegracaoMicDta',
                    'IntegracaoRavex',
                    'IntegracaoGadle',
                    'IntegracaoOnetrust',
                    'IntegracaoMichelin',
                    'IntegracaoInforDoc',
                    'IntegracaoSintegra',
                    'IntegracaoUltragaz',
                    'IntegracaoIsis',
                    'IntegracaoMagalu',
                    'IntegracaoGSW',
                    'IntegracaoDPA',
                    'IntegracaoCTASmart',
                    'IntegracaoSaintGobain',
                    'IntegracaoHavan',
                    'IntegracaoFrota162',
                    'IntegracaoDexco',
                    'IntegracaoIntercab',
                    'IntegracaoProtheus',
                    'IntegracaoUnilever',
                    'IntegracaoBrasilRisk',
                    'IntegracaoSimonetti',
                    'IntegracaoNstech',
                    'IntegracaoMarisa',
                    'IntegracaoDeca',
                    'IntegracaoMarilan',
                    'IntegracaoVLI',
                    'IntegracaoCorreios',
                    'IntegracaoArcelorMittal',
                    'IntegracaoEMP',
                    'IntegracaoTicketLog',
                    'IntegracaoEmillenium',
                    'IntegracaoGNRE',
                    'IntegracaoLogRisk',
                    'IntegracaoDigitalCom',
                    'IntegracaoLBC',
                    'IntegracaoTecnorisk',
                    'IntegracaoDestinadosSAP',
                    'IntegracaoNeokohm',
                    'IntegracaoMoniloc',
                    'IntegracaoBBC',
                    'IntegracaoApisulLog',
                    'IntegracaoSAP',
                    'IntegracaoSAP_API4',
                    'IntegracaoYPE',
                    'IntegracaoOTM',
                    'IntegracaoSIC',
                    'IntegracaoFrimesa',
                    'IntegracaoLoggi',
                    'IntegracaoCTePagamentoLoggi',
                    'IntegracaoJJ',
                    'IntegracaoKlios',
                    'IntegracaoSAPV9',
                    'IntegracaoSAPST',
                    'IntegracaoBrado',
                    'IntegracaoEShip',
                    'IntegracaoDiageo',
                    'IntegracaoP44',
                    'IntegracaoYandeh',
                    'IntegracaoEFrete',
                    'IntegracaoBalancaKIKI',
                    'IntegracaoComprovei',
                    'IntegracaoComproveiRota',
                    'IntegracaoKMM',
                    'IntegracaoLogvett',
                    'IntegracaoAtlas',
                    'IntegracaoFlora',
                    'IntegracaoCalisto',
                    'IntegracaoTrizy',
                    'IntegracaoObramax',
                    'IntegracaoObramaxCTE',
                    'IntegracaoObramaxNFE',
                    'IntegracaoObramaxProvisao',
                    'IntegracaoShopee',
                    'IntegracaoArquivei',
                    'IntegracaoItalac',
                    'IntegracaoItalacFatura',
                    'IntegracaoElectrolux',
                    'IntegracaoWhatsApp',
                    'IntegracaoFroggr',
                    'IntegracaoLoggiFaturas',
                    'IntegracaoValoresCTeLoggi',
                    'IntegracaoRuntec',
                    'IntegracaoCTeAnterioresLoggi',
                    'IntegracaoATSLog',
                    'IntegracaoCamil',
                    'IntegracaoBuntech',
                    'IntegracaoRouteasy',
                    'IntegracaoConfirmaFacil',
                    'IntegracaoPager',
                    'IntegracaoBind',
                    'IntegracaoCebrace',
                    'IntegracaoMondelez',
                    'IntegracaoTrizyEventos',
                    'IntegracaoVector',
                    'IntegracaoGrupoSC',
                    'IntegracaoSalesforce',
                    'IntegracaoConecttec',
                    'IntegracaoMars',
                    'IntegracaoGlobus',
                    'IntegracaoTransSat',
                    'IntegracaoFS',
                    "IntegracaoVedacit",
                    "IntegracaoJDEFaturas",
                    "IntegracaoOlfar",
                    "IntegracaoEfesus",
                    'IntegracaoMigrate',
                    'IntegracaoCassol',
                    'IntegracaoWeberChile',
                    'IntegracaoLactalis',
                    'IntegracaoSistemaTransben',
                    'IntegracaoATSSmartWeb',
                    'ConfiguracaoAcessoViaToken',
                    'IntegracaoFusion',
                    'IntegracaoVSTrack',
                    'IntegracaoTrafegus',
                    'IntegracaoSenior',
                    'IntegracaoYMS',
                    'IntegracaoHUB',
                    'IntegracaoSkymark',
                ];

                var dados = r.Data;

                for (var objeto of objetos) {
                    var dadosObjeto = objeto == null ? dados : dados[objeto];

                    PreencherObjetoKnout(_configuracaoIntegracao, { Data: dadosObjeto });
                }

                PreencherObjetoKnout(_configuracaoIntegracao, r);

                RecarregarGridConfiguracaoIntegracaoAvon();
                RecarregarGridConfiguracaoIntegracaoSAD();
                RecarregarGridCTASmart();

            } else {
                $("#divNaoExistemIntegracoes").show();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

//*******EVENTOS*******

function atualizarClick(e, sender) {
    if (_configuracaoIntegracao.ModificarConexaoParaEnvioRetina.val() == true) {
        var certificadoCRT = document.getElementById(_configuracaoIntegracao.CertificadoCRTServerRetina.id);
        var certificadoP12 = document.getElementById(_configuracaoIntegracao.CertificadoP12SchemaRegistryRetina.id);

        var formData = new FormData();
        formData.append("Certificado CRT Server Retina", certificadoCRT.files[0]);
        formData.append("Certificado P12 Schema Registry Retina", certificadoP12.files[0]);

        enviarArquivo("Integracao/EnviarCertificado?callback=?", {}, formData, function (arg) {
            if (arg.Success) {

            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }

    _configuracaoIntegracao.ConfiguracoesIntegracaoAvon.val(JSON.stringify(_configuracaoIntegracao.ListaConfiguracoesIntegracaoAvon.val()));
    _configuracaoIntegracao.ConfiguracoesIntegracaoCTASmart.val(JSON.stringify(_configuracaoIntegracao.ListConfiguracoesIntegracaoCTASmart.val()));
    _configuracaoIntegracao.ConfiguracoesIntegracaoSAD.val(JSON.stringify(_configuracaoIntegracao.ListaConfiguracoesIntegracaoSAD.val()));
    _configuracaoIntegracao.ConfiguracoesIntegracaoTelhaNorte.val(JSON.stringify(obterDadosIntegracaoTelhaNorte()));

    Salvar(_configuracaoIntegracao, "Integracao/Salvar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
            ObterDados();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function BuscarCidadesOpenTechClick() {
    executarReST("Integracao/BuscarCidadesOpenTech", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Obtido com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function TestarConexaoFTPIsis() {
    executarReST("FTP/TestarConexao", {
        Host: _configuracaoIntegracao.EnderecoFTPIsis.val(),
        Porta: _configuracaoIntegracao.PortaIsis.val(),
        Diretorio: _configuracaoIntegracao.DiretorioIsis.val(),
        Usuario: _configuracaoIntegracao.UsuarioIsis.val(),
        Senha: _configuracaoIntegracao.SenhaIsis.val(),
        Passivo: _configuracaoIntegracao.PassivoIsis.val(),
        UtilizarSFTP: _configuracaoIntegracao.UtilizarSFTPIsis.val(),
        SSL: _configuracaoIntegracao.SSLIsis.val()
    }, function (r) {
        if (r.Success) {
            $("#" + _configuracaoIntegracao.TestarConexaoFTPIsis.id + "_sucesso").removeClass("hidden");
            $("#" + _configuracaoIntegracao.TestarConexaoFTPIsis.id + "_erro").addClass("hidden");
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Conexão obtida com sucesso.");
        } else {
            $("#" + _configuracaoIntegracao.TestarConexaoFTPIsis.id + "_sucesso").addClass("hidden");
            $("#" + _configuracaoIntegracao.TestarConexaoFTPIsis.id + "_erro").removeClass("hidden");
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function atualizarCallbackClick() {

    if (_configuracaoIntegracao.URLRecebimentoCallbackConecttec.val() == "") {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "URL de recebimento não informado");
        return;
    }
    executarReST("Integracao/AtualizarURLRecebimentoConecttec", { ProviderID: _configuracaoIntegracao.ProviderIDConecttec.val(), URL: _configuracaoIntegracao.URLRecebimentoCallbackConecttec.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "URL de recebimento de callback atualizado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}
