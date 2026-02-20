/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../../js/plugin/dropzone/dropzone.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="CargaPedidoDocumentoCTe.js" />
/// <reference path="ConsultaReceita.js" />
/// <reference path="CTe.js" />
/// <reference path="EtapaDocumentos.js" />
/// <reference path="DropZone.js" />
/// <reference path="EtapaDocumentos.js" />
/// <reference path="NotaFiscal.js" />
/// <reference path="CargaColetaContainer.js" />
/// <reference path="ColetaContainerAnexo.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoConsultaPortalFazenda.js" />
/// <reference path="../../../Enumeradores/EnumPermissoesEdicaoCTe.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoMinutaAvon.js" />
/// <reference path="../../../Enumeradores/EnumTipoDocumento.js" />
/// <reference path="../../../Enumeradores/EnumClassificacaoNFe.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridDocumentoDestinadoCargaPedido;
var _produtoDivergente;
var _gridChaves;
var _gridNotasParaEmissao;
var _gridCTesParaEmissao;
var _gridNotaParcial;
var _gridAnexoEtapaNFe;
var _gridNotasCompativeis;
var _gridGridEspelhoIntercement;
var _gridCTesVinculadosOSMae;
var _documentoEmissao;
var _anexo;
var _HTMLDocumentosParaEmissao = "";
var _pedidoConsultaDocumento;
var _TipoContratacaoCargaAtual;
var _indiceGlobalDocumentos = 0;
var _cargaContainer;

/*
 * Declaração das Classes
 */

var PedidoConsultaDocumentos = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CTeEmitidoNoEmbarcador = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PedidoTransbordo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PossuiIntegracaoMichelin = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.NumeroCarregamento = PropertyEntity({ getType: typesKnockout.string });

    this.DataEmissaoInicial = PropertyEntity({ getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ getType: typesKnockout.date });
    this.NumeroNotaInicial = PropertyEntity({ getType: typesKnockout.int });
    this.NumeroNotaFinal = PropertyEntity({ getType: typesKnockout.int });

    this.EtapaNfMercosul = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
};

var DocumentoEmissao = function () {

    var self = this;

    this.AnexosEtapaNF = PropertyEntity({ eventClick: MostrarModalAnexoEtapaNFe, type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, visible: ko.observable(true) });
    this.NOTFISMichelin = PropertyEntity({ eventClick: NOTFISMichelinClick, type: types.event, text: Localization.Resources.Cargas.Carga.BuscarNOTIFS, visible: ko.observable(false) });
    this.ExcluirIntercement = PropertyEntity({ eventClick: ExcluirIntercement, type: types.event, text: Localization.Resources.Cargas.Carga.ExcluirTodosEspelhos, visible: ko.observable(false) });
    this.ToleranciaParaAvanco = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.ToleranciaParaAvanco.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador), val: ko.observable(0) });
    this.Pacotes = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.Pacotes.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador), val: ko.observable(0) });
    this.CteAnteriores = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.CTesAnteriores.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador), val: ko.observable(0) });
    this.VerificarCorVerdeOuVermelho = PropertyEntity({ type: types.event, val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.BuscarDocOSMae = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "Buscar doc OS Mãe / OS:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.GridNotas = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true), visibleLegenda: ko.observable(false) });
    this.GridCTe = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.GridNotaParcial = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.GridNotasCompativeis = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.GridCargaPedidoDocumentoCTe = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.GridEspelhoIntercement = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.GridCTesVinculadosOSMae = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.CargaContainer = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });

    this.Dropzone = PropertyEntity({ type: types.local, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    //this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), text: ko.observable("NF-e(s) para emissão"), enable: ko.observable(false), eventClick: verificarReceitaClick });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), text: ko.observable(Localization.Resources.Cargas.Carga.NFesParaEmissao), enable: ko.observable(false), eventClick: validarDocumentosParaAdicionarCargaClick });
    this.Chave = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.ChaveDeAcessoNumero.getRequiredFieldDescription(), def: "", maxlength: 54, required: true, visible: ko.observable(true), idBtnSearch: guid(), enable: ko.observable(false) });

    var opcoesClassificacaoNF = _cargaAtual.ClassificacaoNFeRemessaVenda.val() ? EnumClassificacaoNFe.obterOpcoesTipoOperacaoRemessaVenda() : EnumClassificacaoNFe.obterOpcoes();
    this.ClassificacaoNFe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ClassificacaoNFe.getFieldDescription(), val: ko.observable(EnumClassificacaoNFe.SemClassificacao), options: opcoesClassificacaoNF, def: EnumClassificacaoNFe.SemClassificacao, visible: ko.observable(_CONFIGURACAO_TMS.ExibirClassificacaoNFe || _cargaAtual.ClassificacaoNFeRemessaVenda.val()), enable: ko.observable(true) });

    this.CargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Rotas = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.IdentificacaoDasNotas.getFieldDescription(), def: "", issue: 253 });
    this.RequerPermissaoParaAvancar = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    //this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //Aba Múltiplas Chaves
    this.PedidoMultiplaChave = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), text: ko.observable(Localization.Resources.Cargas.Carga.NFesParaEmissao), enable: ko.observable(false), eventClick: validarDocumentosParaAdicionarMultiplaChaveClick });
    this.MultiplaChave = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.ChaveDeAcessoNumero.getRequiredFieldDescription(), def: "", maxlength: 5000, required: false, visible: ko.observable(true), idBtnSearch: guid(), enable: ko.observable(false) });
    this.ConfirmarChaves = PropertyEntity({ eventClick: ConfirmarChavesClick, type: types.event, text: Localization.Resources.Cargas.Carga.ConfirmarEnvioDeChaves, visible: ko.observable(true), enable: ko.observable(false) });
    this.GridChaves = PropertyEntity({ type: types.local });
    this.Chaves = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumento.NFe), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.TipoDoDocumento.getRequiredFieldDescription(), options: VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais, _PermissoesPersonalizadasCarga) ? EnumTipoDocumento.obterOpcoes() : ((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe && _cargaAtual.PermitirTransportadorEnviarNotasFiscais.val()) ? EnumTipoDocumento.obterOpcoes() : EnumTipoDocumento.obterOpcoesSemOutros()), def: EnumTipoDocumento.NFe });

    this.ExcluirNotasFiscais = PropertyEntity({ eventClick: excluirNotasFiscaisClick, type: types.event, text: Localization.Resources.Cargas.Carga.ExcluirTodasAsNotasFiscais, visible: ko.observable(false), enable: ko.observable(true) });
    this.ExcluirCTesSubContratacao = PropertyEntity({ eventClick: excluirCTesSubContratacaoClick, type: types.event, text: Localization.Resources.Cargas.Carga.ExcluirTodosOsCTes, visible: ko.observable(false), enable: ko.observable(true) });
    this.LiberarEmissaoSemIntegracaoEtapaTransportador = PropertyEntity({ eventClick: LiberarEmissaoSemIntegracaoEtapaTransportadorClick, visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.LiberarSemIntegracaoEtapaTransportador });

    this.ExibirMais = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirMais.visibleFade()) {
                e.ExibirMais.visibleFade(false);
                $("#" + e.ExibirMais.id + " i").attr("class", "fal fa-plus");
            } else {
                e.ExibirMais.visibleFade(true);
                $("#" + e.ExibirMais.id + " i").attr("class", "fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Cargas.Carga.MaisDadosNFe, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ImportarCTeSubcontratacao = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Cargas.Carga.ImportarCTeSubcontratacao,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default waves-effect waves-themed",
        UrlImportacao: "DocumentoCTe/Importar?CargaPedido=" + self.CargaPedido.val(),
        UrlConfiguracao: "DocumentoCTe/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O044_CargaPedidoCTeSubcontratacao,
        CallbackImportacao: function () {
            executarReST("Carga/BuscarCargaPorCodigo", { Carga: _cargaAtual.Codigo.val() }, function (arg) {
                if (arg.Success) {
                    atualizarDadosCarga(_cargaAtual, arg.Data);
                    carregarGridDocumentosParaEmissao();
                }
            });
        }
    });

    this.ImportarPacotes = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Cargas.Carga.ImportarPacotes,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "CargaNotasFiscais/ImportarPacotes?CodigoCargaPedido=" + self.CargaPedido.val(),
        UrlConfiguracao: "CargaNotasFiscais/ConfiguracaoImportacaoPacotes",
        CodigoControleImportacao: EnumCodigoControleImportacao.O064_Pacotes,
        CallbackImportacao: function () {
            executarReST("Carga/BuscarCargaPorCodigo", { Carga: _cargaAtual.Codigo.val() }, function (arg) {
                if (arg.Success) {
                    atualizarDadosCarga(_cargaAtual, arg.Data);
                    carregarDadosPedido();
                }
            });
        }
    });

    this.NotasTransbordo = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.TransbordarNotasDeOutraCarga, visible: ko.observable(false), enable: ko.observable(true), idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ eventClick: abrirAdicionarDocumentosManualmenteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
    this.ConfirmarEnvioDocumentos = PropertyEntity({ eventClick: confirmarEnvioDosDocumentosClick, type: types.event, text: Localization.Resources.Cargas.Carga.ConfirmarEnvioDosDocumentos, visible: ko.observable(false), enable: ko.observable(true) });
    this.LiberarCargaComApoliceDivergente = PropertyEntity({ eventClick: (e, sender) => confirmarEnvioDosDocumentosClick(e, sender, true), type: types.event, text: Localization.Resources.Cargas.Carga.LiberarCargaComApoliceDivergente, visible: ko.observable(false), enable: ko.observable(true) });
    this.DownloadTodosXmlDocumentoEmissao = PropertyEntity({ eventClick: downloadTodosXmlDocumentoEmissaoClick, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.PermitirDownloadXmlEtapaNfe), type: types.event, text: Localization.Resources.Cargas.Carga.DownloadXmlDasNotas });
    this.LiberarEmissaoSemNF = PropertyEntity({ eventClick: liberarDocumentoEmissaoSemNFClick, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.LiberarEmissaoSemNFe });
    this.VincularCargaOrganizacao = PropertyEntity({ eventClick: informarCargaOrganizacaoClick, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: "Vincular Pré Carga" });

    //Grupo notas compatíveis
    this.DataEmissaoInicial = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataEmissaoInicial, getType: typesKnockout.date, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataEmissaoFinal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataEmissaoFinal, getType: typesKnockout.date, visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroNotaInicial = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroNotaInicial, getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroNotaFinal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroNotaFinal, getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroTrip, getType: typesKnockout.string, visible: ko.observable(false) });
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.GridDocumentoDestinado = PropertyEntity({ type: types.local });
    this.DocumentoDestinado = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.DocumentoDestinado, idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.Cargas.Carga.MarcarDesmarcarTodos, visible: ko.observable(false), enable: ko.observable(true) });
    this.VincularDocumentosCompativeis = PropertyEntity({ eventClick: VincularDocumentoNotasCompativeis, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
    this.AtualizarDocumentosCompativeis = PropertyEntity({ eventClick: AtualizarDocumentosCompativeisClick, type: types.event, text: Localization.Resources.Cargas.Carga.BuscarDocumentos, visible: ko.observable(true), enable: ko.observable(true) });

    this.ProcessandoDocumentosFiscais = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false, text: Localization.Resources.Cargas.Carga.OsDocumentosEstaoSendoProcessadosAguarde });

    this.VincularNotasFiscaisParciaisNovamente = PropertyEntity({ eventClick: VincularNotasFiscaisParciaisNovamenteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Reprocessar, visible: ko.observable(false), enable: ko.observable(true) });

    this.TempoRestante = PropertyEntity({ text: "Tempo restante para recebimento dos pacotes: ", visible: ko.observable(false) });

    //Variaveis Tradução DocumentosParaEmissao.html
    this.ExcluirTodasNotasFiscais = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ExcluirTodasAsNotasFiscais });
    this.ExcluirTodosCTes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ExcluirTodosOsCTes });
    this.NFeEmitidaEmContigencia = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NotaFiscalEmitidaEmContingenciaFSDA });
    this.DocumentoVinculadoEmOutraCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DocumentoVinculadoEmOutraCarga });
    this.EspelhoIntercement = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EspelhoIntercement });
    this.Validar = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Validar });
    this.AdicionarDocumentosManualmente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AdicionarDocumentosManualmente });
    this.NotasNaoRecebidas = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NotaNaoRecebidas });
    this.NotasCompativeisComCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NotasCompativeisComCarga });
    this.DropZone = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Dropzone });
    this.MultiplasChaves = PropertyEntity({ text: Localization.Resources.Cargas.Carga.MultiplasChaves });
    this.EnvioDeArquivos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EnvioDeArquivos });
    this.LancamentoDeChaves = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LancamentoDeChaves });
    this.EditarDadosDaNotaFiscal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EditarDadosDaNotaFiscal });
    this.Detalhes = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Detalhes });
    this.EditarDadosDoCTeDeSubcontratacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EditarDadosDoCTeDeSubcontratacao });
    this.Dimensoes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Dimensoes });
    this.CTesVinculadosOSMae = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CTesVinculadosOSMae });

    this.RetornarEtapa = PropertyEntity({ eventClick: RetornarEtapaClickManual, enable: ko.observable(!_FormularioSomenteLeitura), visible: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.Carga.RetornarEtapa });
};

var AnexoEtapaNFe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150, required: true, visible: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Arquivo.getRequiredFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), required: true });
    this.OcultarParaTransportador = PropertyEntity({ val: ko.observable(false), text: "Ocultar para o transportador", def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });

    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string, enable: ko.observable(true) });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoEtapaNFe.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoEtapaNFeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(!_FormularioSomenteLeitura) });
};

var PrdutoDivergente = function () {

    this.GridProdutosDivergentes = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true), visibleLegenda: ko.observable(false) });
    this.ConfirmarEmissao = PropertyEntity({ eventClick: confirmarEmissaoDocumentosProdutoDivergenteClick, type: types.event, text: Localization.Resources.Cargas.Carga.ConfirmarEmissao, visible: ko.observable(true), enable: ko.observable(true) });
};


/*
 * Declaração das Funções Associadas a Eventos
 */

function CarregarConsultaDocumentoDestinadoCargaPedido() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "80%" }
    ];

    _gridDocumentoDestinadoCargaPedido = new BasicDataTable(_documentoEmissao.GridDocumentoDestinado.id, header, null, { column: 0, dir: orderDir.asc });

    new BuscarDocumentosDestinados(_documentoEmissao.DocumentoDestinado, AdicionarDocumentoDestinadoCargaPedido, _gridDocumentoDestinadoCargaPedido);

    _documentoEmissao.DocumentoDestinado.basicTable = _gridDocumentoDestinadoCargaPedido;
    _gridDocumentoDestinadoCargaPedido.CarregarGrid([]);
}

function AdicionarDocumentoDestinadoCargaPedido(documentos) {
    var codigosDocumentos = new Array();

    for (var i = 0; i < documentos.length; i++)
        codigosDocumentos.push(documentos[i].Codigo);

    executarReST("CargaDocumentosFiscais/AdicionarDocumentosDestinados", { CargaPedido: _documentoEmissao.CargaPedido.val(), Documentos: JSON.stringify(codigosDocumentos) }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DocumentosAdicionadosComSucesso);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        if (_gridNotasParaEmissao != null)
            _gridNotasParaEmissao.CarregarGrid();
    });
}

function abrirAdicionarDocumentosManualmenteClick(e) {
    if (e.TipoDocumento.val() == EnumTipoDocumento.CTe) {
        iniciarEnvioCTeManual();
    } else {
        iniciarEnvioNotaFiscalManual();
    }
}

function AtualizarDocumentosCompativeisClick() {
    carregarGridDocumentosParaEmissao();
}

function ExcluirChaveNotaClick(data) {
    var dadosGrid = _gridChaves.BuscarRegistros();

    for (var i = 0; i < dadosGrid.length; i++) {
        if (data.Codigo === dadosGrid[i].Codigo) {
            dadosGrid.splice(i, 1);
            break;
        }
    }
    _gridChaves.CarregarGrid(dadosGrid);
}

function informarCaptchaNotaParcialClick(data) {
    _documentoEmissao.Chave.val(data.Chave);
    verificarReceitaClick();
}

function removerNotaParcialClick(data) {
    exibirConfirmacao(Localization.Resources.Cargas.Carga.ExcluirNotaParcial, Localization.Resources.Cargas.Carga.TemCertezaQueDesjeaExcluirNotaParcial.format(data.Numero), function () {
        executarReST("CargaPedidoXMLNotaFiscalParcial/ExcluirPorCodigo", { Codigo: data.Codigo }, function (retorno) {
            if (!retorno.Success)
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

            if (!retorno.Data)
                return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.NotaParcialExcluidaComSucesso);
            CarregarGridNotasParciais();
        });
    });
}

function vincularCTeOSMaeClick(data) {
    executarReST("DocumentoNF/VincularCTeOSMae", { Codigo: data.Codigo, CodigoCargaPedido: _pedidoConsultaDocumento.CodigoCargaPedido.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CTeVinculadoSucesso);
                _gridCTesVinculadosOSMae.CarregarGrid();

                if (retorno.Data.DetalhesCarga != null) {
                    veririficarSeCargaMudouTipoContratacao(retorno.Data.DetalhesCarga);
                    carregarGridDocumentosParaEmissao();
                }

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
    });
}

function removerEspelhoClick(data) {
    exibirConfirmacao(Localization.Resources.Cargas.Carga.ExcluirEspelhoIntercement, Localization.Resources.Cargas.Carga.TemCertezaQueDesejaExcluirEspelho.format(data.Codigo), function () {
        executarReST("DocumentoNF/ExcluirEspelho", { Codigo: data.Codigo, CodigoCargaPedido: _pedidoConsultaDocumento.CodigoCargaPedido.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.EspelhoExcluidoComSucesso);
                    _gridGridEspelhoIntercement.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}

/*
 * Declaração das Funções Públicas
 */

function CarregarGridNotasParciais() {
    if (_gridNotaParcial != null) {
        _gridNotaParcial.CarregarGrid(function () {
            if (_gridNotaParcial.NumeroRegistros() > 0) {
                _documentoEmissao.GridNotaParcial.visible(true);
            } else {
                _documentoEmissao.GridNotaParcial.visible(false);
            }
        });
    }
}

function carregarGridDocumentosParaEmissao(exibirOpcoes) {
    if (_CONFIGURACAO_TMS.PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada)
        $("#divNotaFiscalEmOutraCarga").removeClass("d-none");

    _pedidoConsultaDocumento.DataEmissaoInicial.val(_documentoEmissao.DataEmissaoInicial.val());
    _pedidoConsultaDocumento.DataEmissaoFinal.val(_documentoEmissao.DataEmissaoFinal.val());
    _pedidoConsultaDocumento.NumeroNotaInicial.val(_documentoEmissao.NumeroNotaInicial.val());
    _pedidoConsultaDocumento.NumeroNotaFinal.val(_documentoEmissao.NumeroNotaFinal.val());
    _pedidoConsultaDocumento.NumeroCarregamento.val(_documentoEmissao.NumeroCarregamento.val());
    _pedidoConsultaDocumento.PossuiIntegracaoMichelin.val(_cargaAtual.PossuiIntegracaoMichelin.val());
    criarGridDocumentos(exibirOpcoes);

    if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe) {
        _gridNotasCompativeis.CarregarGrid(function () {
            if (_gridNotasCompativeis.NumeroRegistros() > 0) {
                _documentoEmissao.GridNotasCompativeis.visible(true);
            } else {
                if (_cargaAtual.PermitirSelecionarNotasCompativeis.val() && _cargaAtual.PermiteImportarDocumentosManualmente.val())
                    _documentoEmissao.GridNotasCompativeis.visible(true);
                else
                    _documentoEmissao.GridNotasCompativeis.visible(false);
            }
        });
    }

    if (_cargaAtual.PossuiIntegracaoMichelin.val()) {
        _documentoEmissao.NOTFISMichelin.visible(true);
        _documentoEmissao.NumeroCarregamento.visible(true);
    }

    if (_cargaAtual.PossuiIntegracaoIntercement.val())//operacao intercement
    {
        _gridGridEspelhoIntercement.CarregarGrid(function () {
            if (_gridGridEspelhoIntercement.NumeroRegistros() > 0) {
                _gridGridEspelhoIntercement.CarregarGrid();
                _documentoEmissao.GridEspelhoIntercement.visible(true);
                _documentoEmissao.ExcluirIntercement.visible(true);
            } else {
                _documentoEmissao.GridEspelhoIntercement.visible(false);
                _documentoEmissao.ExcluirIntercement.visible(false);
            }
        });
    }

    if (_cargaAtual.CargaDestinadaCTeComplementar.val()) {
        _gridCTesVinculadosOSMae.CarregarGrid(function () {
            if (_gridCTesVinculadosOSMae.NumeroRegistros() > 0) {
                _gridCTesVinculadosOSMae.CarregarGrid();
                _documentoEmissao.GridCTesVinculadosOSMae.visible(true);
            } else {
                _documentoEmissao.GridCTesVinculadosOSMae.visible(false);
            }
        });
    }

    CarregarGridNotasParciais();

    if (_TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.NormalESubContratada) {
        _gridCTesParaEmissao.CarregarGrid();
        return _gridNotasParaEmissao.CarregarGrid().then(function () {
            posCarregarGridDocumentosParaEmissao();
        });
    } else {
        if (_TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.SVMTerceiro || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.SubContratada || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.Redespacho || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.RedespachoIntermediario
            || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.SVMProprio) {
            _gridCTesParaEmissao.CarregarGrid().then(function () {
                posCarregarGridDocumentosParaEmissao();
            });
        } else if (_gridNotasParaEmissao != null) {
            return _gridNotasParaEmissao.CarregarGrid().then(function () {
                posCarregarGridDocumentosParaEmissao();
            });
        }
    }
}

function carregarDocumentosParaEmissaoPedido(indice, carregarGrid, mercosul) {
    if (carregarGrid == null)
        carregarGrid = true;

    _gridNotasParaEmissao = null;
    _gridCTesParaEmissao = null;
    _gridNotaParcial = null;
    _gridNotasCompativeis = null;
    _gridGridEspelhoIntercement = null;
    _gridCTesVinculadosOSMae = null;

    let idGrid = _cargaAtual.EtapaNotaFiscal.idGrid;
    let idElemento = _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao";

    if (mercosul === true) {
        idGrid = _cargaAtual.EtapaDocumentosMercosul.idGrid;
        idElemento = _cargaAtual.EtapaDocumentosMercosul.idGrid + "_knoutDocumentosParaEmissao";
    }

    $("#" + idGrid).html(_HTMLDocumentosParaEmissao.replace(/#knoutDocumentosParaEmissao/g, idElemento));

    _documentoEmissao = new DocumentoEmissao();
    KoBindings(_documentoEmissao, idElemento);

    LocalizeCurrentPage();

    _produtoDivergente = new PrdutoDivergente();
    KoBindings(_produtoDivergente, "knoutProdutosDivergentes");

    preencherTabsPedidos(_documentoEmissao.Pedido.idTab, "carregarDocumentosParaEmissaoPedido", indice);

    _indiceGlobalDocumentos = indice;

    _pedidoConsultaDocumento = new PedidoConsultaDocumentos();
    //_pedidoConsultaDocumento.Codigo.val(_cargaAtual.Pedidos.val[indice].Codigo);
    _pedidoConsultaDocumento.CodigoCargaPedido.val(_cargaAtual.Pedidos.val[indice].CodigoCargaPedido);
    _pedidoConsultaDocumento.Carga.val(_cargaAtual.Codigo.val());
    _pedidoConsultaDocumento.CTeEmitidoNoEmbarcador.val(_cargaAtual.Pedidos.val[indice].CTeEmitidoNoEmbarcador);

    if (_cargaAtual.DataAvancouSegundaEtapa.val() != "") {
        _documentoEmissao.TempoRestante.visible(true);
        $("#" + _documentoEmissao.TempoRestante.id)
            .countdown(moment(_cargaAtual.DataAvancouSegundaEtapa.val(), "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
            .on('update.countdown', function (event) {
                if (event.elapsed) {
                    $(this).text(" [esgotado]");
                }
                else {
                    if (event.offset.totalDays > 0)
                        $(this).text(event.strftime('%-Dd %H:%M:%S'));
                    else
                        $(this).text(event.strftime('%H:%M:%S'));
                }
            })
    }

    if (mercosul)
        _pedidoConsultaDocumento.EtapaNfMercosul.val(true);

    var pedidotransbordo = _cargaAtual.Pedidos.val[indice].PedidoTransbordo || _cargaAtual.CargaSVM.val();

    _pedidoConsultaDocumento.PedidoTransbordo.val(pedidotransbordo);

    _documentoEmissao.CargaPedido.val(_pedidoConsultaDocumento.CodigoCargaPedido.val());
    _documentoEmissao.ImportarCTeSubcontratacao.UrlImportacao = "DocumentoCTe/Importar?CargaPedido=" + _documentoEmissao.CargaPedido.val();
    _documentoEmissao.ImportarPacotes.UrlImportacao = "CargaNotasFiscais/ImportarPacotes?CodigoCargaPedido=" + _documentoEmissao.CargaPedido.val();

    if ((_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe) || _cargaAtual.CargaEmitidaParcialmente.val()) {
        _documentoEmissao.GridNotasCompativeis.visible(true);
        _documentoEmissao.Pedido.enable(true);
        _documentoEmissao.PedidoMultiplaChave.enable(true);
    }
    else {
        _documentoEmissao.Pedido.enable(false);
        _documentoEmissao.PedidoMultiplaChave.enable(false);
    }

    _documentoEmissao.Dropzone.visible(_documentoEmissao.Pedido.enable());
    _documentoEmissao.Dropzone.enable(_documentoEmissao.Pedido.enable());
    _documentoEmissao.Adicionar.enable(_documentoEmissao.Pedido.enable());
    _documentoEmissao.NotasTransbordo.enable(_documentoEmissao.Pedido.enable());
    _documentoEmissao.ConfirmarEnvioDocumentos.enable(_documentoEmissao.Pedido.enable());
    _documentoEmissao.VincularNotasFiscaisParciaisNovamente.enable(_documentoEmissao.Pedido.enable());
    _documentoEmissao.Chave.enable(_documentoEmissao.Pedido.enable());
    _documentoEmissao.MultiplaChave.enable(_documentoEmissao.Pedido.enable())
    _documentoEmissao.PedidoMultiplaChave.enable(_documentoEmissao.Pedido.enable())
    _documentoEmissao.ConfirmarChaves.enable(_documentoEmissao.Pedido.enable())
    _documentoEmissao.ExcluirNotasFiscais.enable(_documentoEmissao.Pedido.enable() && !_cargaAtual.CargaEmitidaParcialmente.val());
    _documentoEmissao.ExcluirCTesSubContratacao.enable(_documentoEmissao.Pedido.enable());
    _documentoEmissao.GridNotas.visibleLegenda(_documentoEmissao.Pedido.enable());
    _documentoEmissao.LiberarCargaComApoliceDivergente.visible(_documentoEmissao.Pedido.enable() && _cargaAtual.PendenciaValorLimiteApolice.val());
    _documentoEmissao.LiberarCargaComApoliceDivergente.enable(_documentoEmissao.Pedido.enable() && _cargaAtual.PendenciaValorLimiteApolice.val());

    if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe &&
        _CONFIGURACAO_TMS.ConsultarDocumentosDestinadosCarga === true) {
        CarregarConsultaDocumentoDestinadoCargaPedido();

        _documentoEmissao.DocumentoDestinado.visible(true);
        _documentoEmissao.DocumentoDestinado.enable(true);
    }

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_LiberarEmissaoSemNF, _PermissoesPersonalizadasCarga)
        && !_cargaAtual.CargaDePreCarga.val() && ((_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe)
            && !Boolean(_cargaAtual.DataInicioEmissao.val()) || _cargaAtual.CargaEmitidaParcialmente) && _CONFIGURACAO_TMS.PermitirLiberarCargaSemNFe)
        _documentoEmissao.LiberarEmissaoSemNF.visible(true);
    else
        _documentoEmissao.LiberarEmissaoSemNF.visible(false);

    if (_cargaAtual.TipoOperacao.PermitirSelecionarPreCargaNaCarga)
        _documentoEmissao.VincularCargaOrganizacao.visible(true);

    new BuscarCargas(_documentoEmissao.NotasTransbordo, cargaParaTransbordoRetorno);
    new BuscarCargasFinalizadasPelaOSMaeOuOS(_documentoEmissao.BuscarDocOSMae, _cargaAtual.Codigo.val(), salvarCargaDocOSMaeOS);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.Normal || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.NormalESubContratada || _cargaAtual.FreteDeTerceiro.val())
        _documentoEmissao.GridNotas.visibleLegenda(false);

    if (_cargaAtual.PermitirTransbordarNotasDeOutrasCargas.val())
        _documentoEmissao.NotasTransbordo.visible(true);

    if ((!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais, _PermissoesPersonalizadasCarga) || _FormularioSomenteLeitura) && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor) {
        _documentoEmissao.Adicionar.enable(false);
        _documentoEmissao.NotasTransbordo.enable(false);
        _documentoEmissao.ConfirmarEnvioDocumentos.enable(false);
        _documentoEmissao.VincularNotasFiscaisParciaisNovamente.enable(false);
        _documentoEmissao.MultiplaChave.enable(false);
        _documentoEmissao.PedidoMultiplaChave.enable(false);
        _documentoEmissao.ConfirmarChaves.enable(false);
        _documentoEmissao.Dropzone.enable(false);
        _documentoEmissao.ExcluirNotasFiscais.enable(false);
        _documentoEmissao.ExcluirCTesSubContratacao.enable(false);
        _documentoEmissao.DocumentoDestinado.visible(false);
        _documentoEmissao.DocumentoDestinado.enable(false);

        if (!(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && _cargaAtual.TipoOperacao.PermitirTransportadorInformeNotasCompativeis))
            _documentoEmissao.Chave.enable(false);
    }

    if (_cargaAtual.PermitirTransportadorEnviarNotasFiscais.val() && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _documentoEmissao.Adicionar.enable(true);
        _documentoEmissao.ExcluirNotasFiscais.enable(true);
        _documentoEmissao.ConfirmarEnvioDocumentos.enable(true);
    }

    if (_pedidoConsultaDocumento.CTeEmitidoNoEmbarcador.val() || (_pedidoConsultaDocumento.PedidoTransbordo.val() && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)) {
        loadCargaPedidoDocumentoCTe(indice);
        LoadCargaPedidoDocumentoMDFe(indice);
        $("#divDocumentoManual_" + idGrid + "_knoutDocumentosParaEmissao").hide();
        $("#divDocumentosCTe_" + idGrid + "_knoutDocumentosParaEmissao").show();
    }
    else {
        $("#divDocumentoManual_" + idGrid + "_knoutDocumentosParaEmissao").show();
        $("#divDocumentosCTe_" + idGrid + "_knoutDocumentosParaEmissao").hide();
    }

    _documentoEmissao.LiberarEmissaoSemIntegracaoEtapaTransportador.visible(false);
    if (_CONFIGURACAO_TMS.LiberarIntegracaoTransportadorDeCargaImportarDocumentoManual && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        _documentoEmissao.LiberarEmissaoSemIntegracaoEtapaTransportador.visible(_cargaAtual.AguardarIntegracaoEtapaTransportador.val());
    }

    loadConsultaReceita();
    loadCargaDocumentoNF(idGrid);
    loadCargaDocumentoCTe();
    loadPedidoVinculado();
    loadDropZone();

    loadMultiplasChaves();
    RecarregarGridChaves();
    loadPacotesAvulsos();
    preecherDocumentosEmissaoAvancoCarga();

    loadAgrupamentoNotasPrechekin(idGrid, "#liTabEtapaNFeStage_" + idElemento, "#tabEtapaNFeStage_" + idElemento);
    carregarPacotes(idGrid, "#liTabPacotes_" + idElemento, "#tabPacotes_" + idElemento);

    if (carregarGrid)
        carregarGridDocumentosParaEmissao();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS || _cargaAtual.PermiteImportarDocumentosManualmente.val()) {
        executarReST("CargaNotasFiscais/ObterInformacoesGerais", { Carga: _cargaAtual.Codigo.val(), CargaPedido: _documentoEmissao.CargaPedido.val() }, function (r) {
            if (r.Success) {
                LoadIntegracaoMinutaAvon(_cargaAtual, r.Data.IntegracaoAvon, r.Data.Integracoes);
                LoadIntegracaoDocumentoTransporteNatura(_cargaAtual, r.Data.IntegracaoNatura, r.Data.Integracoes);
                LoadIntegracaoMercadoLivre(_cargaAtual, r.Data.Integracoes);
                _documentoEmissao.BuscarDocOSMae.codEntity(r.Data.DocOSMae.Codigo);
                _documentoEmissao.BuscarDocOSMae.val(r.Data.DocOSMae.Descricao);
                if (r.Data.RequerPermissaoParaAvancar) {
                    _documentoEmissao.RequerPermissaoParaAvancar.val(true);
                } else {
                    _documentoEmissao.RequerPermissaoParaAvancar.val(false);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });

        loadDadosIntegracaoCargaFreteIntegracao(true);
    }

    var processandoDocumentosFiscais = _cargaAtual.ProcessandoDocumentosFiscais.val();

    _documentoEmissao.ConfirmarEnvioDocumentos.visible(!processandoDocumentosFiscais);

    if (_CONFIGURACAO_TMS.VincularNotasParciaisPedidoPorProcesso === true)
        _documentoEmissao.VincularNotasFiscaisParciaisNovamente.visible(!processandoDocumentosFiscais);

    _documentoEmissao.ProcessandoDocumentosFiscais.val(processandoDocumentosFiscais);

    if (_CONFIGURACAO_TMS.ExibirFiltrosNotasCompativeisCarga) {
        $("#liFiltrosGridNotasCompativeis_" + idGrid + "_knoutDocumentosParaEmissao").show();
        _documentoEmissao.SelecionarTodos.visible(true);

        if (_cargaAtual.PossuiIntegracaoMichelin.val()) {
            _documentoEmissao.DataEmissaoInicial.visible(false);
            _documentoEmissao.DataEmissaoFinal.visible(false);
            _documentoEmissao.NumeroNotaInicial.visible(true);
            _documentoEmissao.NumeroNotaFinal.visible(true);
            _documentoEmissao.NumeroCarregamento.visible(true);
        }
    }

    if (_cargaAtual.ExigirInformarContainer.val() && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
        loadCargaContainer(_cargaAtual);
}

function exibirDetalhesNaoConformidadesDocumentoEmissaoClick(registroSelecionado) {
    exibirDetalhesNaoConformidadePorNotaFiscal(registroSelecionado.Codigo);
}

function loadGridAnexoEtapaNFe() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: DownloadAnexoEtapaNFeClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: RemoverAnexoEtapaNFeClick, icone: "", visibilidade: VerificarOpcaoRemoverAnexoNFe };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "30%", className: "text-align-left" }
    ];

    _gridAnexoEtapaNFe = new BasicDataTable(_anexoEtapaNFe.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoEtapaNFe.CarregarGrid([]);
}

function DownloadAnexoEtapaNFeClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("CargaNFeAnexos/DownloadAnexo", dados);
}

function RemoverAnexoEtapaNFeClick(registroSelecionado) {
    executarReST("CargaNFeAnexos/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AnexoExcluidoComSucesso);
                removerAnexoLocalNFe(registroSelecionado);
                _gridAnexoEtapaNFe.CarregarGrid(_anexoEtapaNFe.Anexos.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function removerAnexoLocalNFe(registroSelecionado) {
    var listaAnexos = _gridAnexoEtapaNFe.BuscarRegistros();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexoEtapaNFe.Anexos.val(listaAnexos);
}

function ConfirmarChavesClick(e, sender) {
    preencherListasSelecaoChaves();

    executarReST("ConsultaDocumentos/ConsultarMultiplasChaves", { Chaves: _documentoEmissao.Chaves.val(), CargaPedido: _documentoEmissao.CargaPedido.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var data = retorno.Data;

                carregarGridDocumentosParaEmissao();
                limparCamposDocumentosParaEmissao();

                _documentoEmissao.Chaves.val(data.Chaves);
                RecarregarGridChaves();

                if (data.QuantidadeImportada == 0)
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.NenhumaDasNotasInformadasForamImportadas.format(data.QuantidadeTotal), 20000);
                else
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ImportadoDeNotas.format(data.QuantidadeImportada, data.QuantidadeTotal), 15000);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function confirmarEmissaoDocumentosProdutoDivergenteClick(e) {
    Global.fecharModal("divModalProdutosDivergentes");
    executarConfirmacaoDocumentos(executarEnvioDocumentosRest, true);
}

function confirmarEnvioDosDocumentosClick(e, sender, naoValidarApolice) {
    if (!naoValidarApolice) naoValidarApolice = false;
    var executarEnvioDocumentos = executarEnvioDocumentosRestCargaDocumentosFiscais;
    executarReST("CargaDocumentosFiscais/VerificarDivergenciaProdutosEsperadoVSRecebidos", { Carga: _cargaAtual.Codigo.val() }, function (arg) {
        if (arg.Success) {
            var produtosDivergentes = arg.Data;
            if (produtosDivergentes.length == 0)
                executarConfirmacaoDocumentos(executarEnvioDocumentos, false, naoValidarApolice);
            else {
                var header = [
                    { data: "Produto", title: Localization.Resources.Cargas.Carga.Produto, width: "40%" },
                    { data: "Pedido", title: Localization.Resources.Cargas.Carga.Pedido, width: "20%" },
                    { data: "QuantidadeEsperada", title: Localization.Resources.Cargas.Carga.Esperado, width: "10%" },
                    { data: "QuantidadeRecebida", title: Localization.Resources.Cargas.Carga.Recebido, width: "10%" },
                    { data: "NumeroLotePedidoProdutoLoteEsperado", title: "Lote Esperado", width: "10%" },
                    { data: "NumeroLotePedidoProdutoLoteRecebido", title: "Lote Recebido", width: "10%" }
                ];
                var gridDivergente = new BasicDataTable(_produtoDivergente.GridProdutosDivergentes.idGrid, header, null, null, null, 10);
                gridDivergente.CarregarGrid(produtosDivergentes);
                Global.abrirModal("divModalProdutosDivergentes");
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function liberarDocumentoEmissaoSemNFClick(e) {
    liberarEmissaoSemNF(e, _cargaAtual.Codigo.val());
}

function informarCargaOrganizacaoClick(e) {
    exibirCargaOrganizacao();
}

function limparCamposDocumentosParaEmissao() {
    LimparCampos(_documentoEmissao);
    _documentoEmissao.CargaPedido.val(_pedidoConsultaDocumento.CodigoCargaPedido.val());
}

function loadInfoDocumentosParaEmissao(knoutCarga, carregarGrid, mercosul) {
    _cargaAtual = knoutCarga;
    carregarDocumentosParaEmissaoPedido(0, carregarGrid, mercosul);
}

function validarDocumentosParaAdicionarCargaClick() {
    if (ValidarCampoObrigatorioMap(_documentoEmissao.Chave)) {
        if ((_CONFIGURACAO_TMS.ExibirClassificacaoNFe || _cargaAtual.ClassificacaoNFeRemessaVenda.val()) && _documentoEmissao.ClassificacaoNFe.val() === EnumClassificacaoNFe.SemClassificacao) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Cargas.Carga.NecessarioInformarClassificacaoDeNFe);
            return;
        }

        var dados = {
            ChaveOuNumero: _documentoEmissao.Chave.val(),
            CargaPedido: _documentoEmissao.CargaPedido.val(),
            ClassificacaoNFe: _documentoEmissao.ClassificacaoNFe.val()
        };
        executarReST("ConsultaDocumentos/Consultar", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Msg != null && retorno.Msg != undefined && retorno.Msg != "")
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
                    else
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                    veririficarSeCargaMudouTipoContratacao(retorno.Data);
                    carregarGridDocumentosParaEmissao();
                    limparCamposDocumentosParaEmissao();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Cargas.Carga.ChaveDeAcessoOuNumeroObrigatorio);
}

function validarDocumentosParaAdicionarMultiplaChaveClick() {
    if (_documentoEmissao.MultiplaChave.val() != "") {
        if ((_CONFIGURACAO_TMS.ExibirClassificacaoNFe || _cargaAtual.ClassificacaoNFeRemessaVenda.val()) && _documentoEmissao.ClassificacaoNFe.val() === EnumClassificacaoNFe.SemClassificacao) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Cargas.Carga.NecessarioInformarClassificacaoDeNFe);
            return;
        }

        var chaves = _gridChaves.BuscarRegistros();

        var chaveConsulta = _documentoEmissao.MultiplaChave.val().split(';');

        for (var i = 0; i < chaveConsulta.length; i++) {
            var chaveNumero = chaveConsulta[i];

            if (Boolean(chaveNumero)) {
                var isSomenteNumero = chaveNumero.length < 10;

                chaves.push({
                    Codigo: guid(),
                    Chave: isSomenteNumero ? "" : chaveNumero,
                    Numero: isSomenteNumero ? chaveNumero : "",
                    MensagemRetorno: Localization.Resources.Cargas.Carga.AguardandoConfirmacao,
                    ClassificacaoNFe: _documentoEmissao.ClassificacaoNFe.val()
                });
            }
        }
        _gridChaves.CarregarGrid(chaves);

        _documentoEmissao.MultiplaChave.val("");
        $("#" + _documentoEmissao.MultiplaChave.id).focus();
    } else
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Cargas.Carga.ChaveDeAcessoOuNumeroObrigatorio);
}

function veririficarSeCargaMudouTipoContratacao(carga) {
    if (typeof (carga) === "object") {
        IniciarBindKnoutCarga(_cargaAtual, carga);
        InformarEstadosDasEtapas(_cargaAtual, carga, _cargaAtual.DivCarga.id);
        preencherTabsPedidos(_documentoEmissao.Pedido.idTab, "carregarDocumentosParaEmissaoPedido", 0);
    }
}

/*
 * Declaração das Funções Privadas
 */

function VincularNotasFiscaisParciaisNovamenteClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteRealizarNovaTentativaDeVincularAsNotasFiscaisEstaCarga, function () {
        var dados = {
            Carga: _cargaAtual.Codigo.val()
        };

        executarReST("CargaPedidoXMLNotaFiscalParcial/VincularNotasFiscaisParciaisNovamente", dados, function (r) {
            if (r.Success) {
                if (r.Data)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DocumentosParaReprocessamento);
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function cargaParaTransbordoRetorno(carga) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaTransbordarTodasAsNotasDaCarga.format(carga.CodigoCargaEmbarcador), function () {
        var data = { CargaTransbordo: carga.Codigo, CargaPedido: _pedidoConsultaDocumento.CodigoCargaPedido.val() };
        executarReST("DocumentoNF/VicularNotasDaCargaParaTransbordo", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    carregarGridDocumentosParaEmissao();
                    limparNotasFiscaisEmissao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 200000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function salvarCargaDocOSMaeOS(e) {
    var data = { Carga: _cargaAtual.Codigo.val(), DocOSMae: e.Codigo };
    executarReST("CargaDocumentosFiscais/SalvarCargaDocOSMaeOS", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _documentoEmissao.BuscarDocOSMae.codEntity(e.Codigo);
                _documentoEmissao.BuscarDocOSMae.val(e.Descricao);

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Carga salva com sucesso");

                executarReST("Carga/BuscarCargaPorCodigo", { Carga: _cargaAtual.Codigo.val() }, function (arg) {
                    if (arg.Success) {
                        atualizarDadosCarga(_cargaAtual, arg.Data);
                        carregarGridDocumentosParaEmissao();
                    }
                });

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 200000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function criarGridDocumentos(exibirOpcoes) {
    if (exibirOpcoes == null)
        exibirOpcoes = true;

    var informarCaptcha = { descricao: Localization.Resources.Cargas.Carga.BuscarDados, id: guid(), evento: "onclick", metodo: informarCaptchaNotaParcialClick, tamanho: "10", icone: "", visibilidade: VisibilidadeInformarCaptcha };
    var removerNotaParcial = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: removerNotaParcialClick, tamanho: "10", icone: "" };
    var menuOpcoesParcial = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Excluir, opcoes: [informarCaptcha, removerNotaParcial] };

    _gridNotaParcial = new GridView(_documentoEmissao.GridNotaParcial.idGrid, "CargaPedidoXMLNotaFiscalParcial/Pesquisa", _pedidoConsultaDocumento, menuOpcoesParcial, null);

    var excluirEspelho = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: removerEspelhoClick, tamanho: "1", icone: "" };
    var menuOpcoesEspelho = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Excluir, opcoes: [excluirEspelho] };
    _gridGridEspelhoIntercement = new GridView(_documentoEmissao.GridEspelhoIntercement.idGrid, "DocumentoNF/PesquisaEspelhoIntercement", _pedidoConsultaDocumento, menuOpcoesEspelho);
    _pedidoConsultaDocumento.PossuiIntegracaoMichelin.val(_cargaAtual.PossuiIntegracaoMichelin.val());

    var vincularCTeOSMae = { descricao: Localization.Resources.Cargas.Carga.VincularCTeOSMae, id: guid(), evento: "onclick", metodo: vincularCTeOSMaeClick, tamanho: "1", icone: "" };
    var menuOpcoesCTesVinculadosOSMae = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Cargas.Carga.VincularCTeOSMae, opcoes: [vincularCTeOSMae] };
    _gridCTesVinculadosOSMae = new GridView(_documentoEmissao.GridCTesVinculadosOSMae.idGrid, "DocumentoNF/CTesVinculadosOSMae", _pedidoConsultaDocumento, menuOpcoesCTesVinculadosOSMae);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        somenteLeitura: false,
        SelecionarTodosKnout: _documentoEmissao.SelecionarTodos
    };

    _gridNotasCompativeis = new GridView(_documentoEmissao.GridNotasCompativeis.idGrid, "DocumentoNF/PesquisaNotasCompativeis", _pedidoConsultaDocumento, null, null, null, null, null, null, multiplaescolha);
    var tipoContratacao = _cargaAtual.Pedidos.val[_indiceGlobalDocumentos].TipoContratacaoCarga;
    if ((_gridNotasParaEmissao == null || _gridCTesParaEmissao == null) || _TipoContratacaoCargaAtual != tipoContratacao) {
        _TipoContratacaoCargaAtual = tipoContratacao;

        if (_TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.SVMProprio || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.SVMTerceiro || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.SubContratada || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.NormalESubContratada || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.Redespacho || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.RedespachoIntermediario) {

            let aplicarMenuOpcoesCTe = true;
            if (_cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe || !exibirOpcoes)
                aplicarMenuOpcoesCTe = false

            criarGridCTesParaEmissao(aplicarMenuOpcoesCTe);

            if (_CONFIGURACAO_TMS.ModificarTimelineDeAcordoComTipoServicoDocumento)
                _documentoEmissao.Pedido.text(Localization.Resources.Cargas.Carga.CTeAnterior);

            if (_cargaAtual.TipoServicoCarga.val() == EnumTipoServicoCarga.Feeder && _CONFIGURACAO_TMS.ModificarTimelineDeAcordoComTipoServicoDocumento)
                _documentoEmissao.Pedido.text(Localization.Resources.Cargas.Carga.DeclaracaoFeeder);

            _documentoEmissao.GridCTe.visible(true);
        } else {
            _documentoEmissao.GridCTe.visible(false);
        }

        if (_TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.Normal || _TipoContratacaoCargaAtual == EnumTipoContratacaoCarga.NormalESubContratada || _cargaAtual.FreteDeTerceiro.val()) {
            var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: "clasEditar", evento: "onclick", metodo: excluirNFeClick, tamanho: 7, icone: "", visibilidade: VisibilidadeOpcaoExcluirNFe };
            var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: abrirTelaEdicaoNFe, tamanho: 7, icone: "" };
            var detalhesNaoConformidade = { descricao: "Detalhes das Não Conformidades", id: guid(), evento: "onclick", metodo: exibirDetalhesNaoConformidadesDocumentoEmissaoClick, tamanho: "10", icone: "", visibilidade: visibilidadeOpcaoDetalhesNaoConformidadesDocumentoEmissao };
            var downloadDANFE = { descricao: Localization.Resources.Cargas.Carga.DownloadDANFE, id: guid(), evento: "onclick", metodo: DownloadDANFEDocumentoEmissaoClick, tamanho: 7, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFE };
            var downloadXml = { descricao: Localization.Resources.Cargas.Carga.DownloadXML, id: guid(), evento: "onclick", metodo: downloadXmlDocumentoEmissaoClick, tamanho: 7, icone: "", visibilidade: visibilidadeOpcaoDownloadXml };
            var gridCCe = { descricao: Localization.Resources.Cargas.Carga.CartaDeCorrecao, id: guid(), evento: "onclick", metodo: exibirGridCartaCorrecao, tamanho: 7, icone: "", visibilidade: visibilidadeGridCCe };
            var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), evento: "onclick", metodo: OpcaoAuditoria("XMLNotaFiscal", "Codigo", _notaFiscal), tamanho: 7, icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
            var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [detalhesNaoConformidade, editar, excluir, downloadDANFE, downloadXml, gridCCe] };

            if (_cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe || !exibirOpcoes) {
                if (_cargaAtual.CargaEmitidaParcialmente.val() && exibirOpcoes)
                    menuOpcoes.opcoes = [detalhesNaoConformidade, editar, excluir, downloadDANFE, downloadXml, gridCCe];
                else
                    menuOpcoes.opcoes = [detalhesNaoConformidade, editar, downloadDANFE, downloadXml, gridCCe];
            }

            if (_FormularioSomenteLeitura)
                menuOpcoes.opcoes = [detalhesNaoConformidade, downloadDANFE, downloadXml, gridCCe];

            if (PermiteAuditar())
                menuOpcoes.opcoes.push(auditar);

            _gridNotasParaEmissao = new GridView(_documentoEmissao.GridNotas.idGrid, "DocumentoNF/Pesquisa", _pedidoConsultaDocumento, menuOpcoes, null);

            if (_cargaAtual.ExclusivaDeSubcontratacaoOuRedespacho.val())
                _documentoEmissao.Pedido.text(Localization.Resources.Cargas.Carga.CTesParaEmissao);
            else
                _documentoEmissao.Pedido.text(Localization.Resources.Cargas.Carga.NFesParaEmissao);

            _documentoEmissao.GridNotas.visible(true);
            _documentoEmissao.GridNotas.visibleLegenda(true);

            if (_cargaAtual.Mercosul.val() && !_pedidoConsultaDocumento.EtapaNfMercosul.val()) {
                _documentoEmissao.Pedido.text("Factura(s)");
                _documentoEmissao.Dropzone.visible(false);
            }

        } else {
            _documentoEmissao.GridNotas.visible(false);
            _documentoEmissao.GridNotas.visibleLegenda(false);
        }
    }
}

function executarConfirmacaoDocumentos(executarEnvioDocumentos, liberouProdutosDivergentes, naoValidarApolice) {

    if (naoValidarApolice && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_LiberarCargaComValorLimiteApoliceDivergente, _PermissoesPersonalizadasCarga))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Você não possui permissões para executar esta ação.", 10000);

    if (_documentoEmissao.RequerPermissaoParaAvancar.val()) {
        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AutorizarSemArquivoOrtec, _PermissoesPersonalizadasCarga)) {
            exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.EssaViagemNaoPossuiUmaIntegraaoDeAgrupamentoComOrtecDesejaRealmenteConfirmarEnvioDosDocumentos, function () {
                executarEnvioDocumentos(liberouProdutosDivergentes, naoValidarApolice);
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.EssaViagemNaoPossuiUmaIntegracaoDeAgrupamentoComOrtecSomenteUsuarioComPermissaoEspecialPodemAvancarEssaViagem, 10000);
        }
    } else {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteConfirmarEnvioDosDocumentos, function () {
            executarEnvioDocumentos(liberouProdutosDivergentes, naoValidarApolice);
        });
    }
}

function executarEnvioDocumentosRestCargaDocumentosFiscais(liberouProdutosDivergentes, naoValidarApolice) {
    VerificarIncosistenciaRegraPlanejamentoFrota().then(function () {
        VerificarPendenciasEnvioDocumentos().then(function () {
            executarReST("CargaDocumentosFiscais/ConfirmarEnvioDosDocumentosFiscais", { Carga: _cargaAtual.Codigo.val(), LiberouProdutosDivergentes: liberouProdutosDivergentes, NaoValidarApolice: naoValidarApolice }, function (arg) {
                if (!arg.Success)
                    return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);

                if (arg.Data !== false) {
                    if (arg.Data.ErroApolice) {
                        _documentoEmissao.LiberarCargaComApoliceDivergente.visible(_documentoEmissao.Pedido.enable());
                        _documentoEmissao.LiberarCargaComApoliceDivergente.enable(_documentoEmissao.Pedido.enable());
                        if (arg.Data.MensagemAlerta)
                            _cargaAtual.MensagensAlerta.val([arg.Data.MensagemAlerta, ..._cargaAtual.MensagensAlerta.val()]);
                        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                    }

                    _cargaAtual.ProcessandoDocumentosFiscais.val(true);
                    _documentoEmissao.ProcessandoDocumentosFiscais.val(true);
                    _documentoEmissao.ConfirmarEnvioDocumentos.visible(false);
                    _documentoEmissao.VincularNotasFiscaisParciaisNovamente.visible(false);

                    if (arg.Data.EncontrouTipoOperacao)
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.MensagemTipoOperacao, 25000);

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            });
        });
    });
}

function VerificarIncosistenciaRegraPlanejamentoFrota() {
    let p = new promise.Promise();

    executarReST("CargaDocumentosFiscais/VerificarIncosistenciaRegraPlanejamentoFrota", { Carga: _cargaAtual.Codigo.val() }, function (arg) {
        if (arg.Success) {
            let retorno = arg.Data;
            if (!retorno.Sucesso) {
                if (retorno.UsuarioComPermissao) {
                    setTimeout(function () {
                        exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteConfirmarRegraPlanejamentoFrota + " </br>" + retorno.Mensagem, function () {
                            p.done();
                        });
                    }, 500);
                }
                else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Mensagem, 20000);
                }
            }
            else
                p.done();

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

    return p;
}

function VerificarPendenciasEnvioDocumentos() {
    var p = new promise.Promise();

    executarReST("CargaDocumentosFiscais/ValidarEnvioDocumentosFiscais", { Carga: _cargaAtual.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_CONFIGURACAO_TMS.PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada && _gridNotasParaEmissao != null)
                    _gridNotasParaEmissao.CarregarGrid();
                if (arg.Data.PossuiPendencia) {
                    setTimeout(function () {
                        exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, arg.Data.Mensagem + " " + Localization.Resources.Cargas.Carga.DesejaProsseguirMesmoAssim, function () {

                            if (arg.Data.LimiteDeTaraAtingido || arg.Data.LimiteDePesoAtingido || arg.Data.LimiteDeValorAtingido || arg.Data.PossuiPedidoDevolucaoPacoteSemPacote) {
                                const dados = {
                                    Carga: _cargaAtual.Codigo.val(),
                                    LimiteDeTaraAtingido: arg.Data.LimiteDeTaraAtingido,
                                    LimiteDePesoAtingido: arg.Data.LimiteDePesoAtingido,
                                    LimiteDeValorAtingido: arg.Data.LimiteDeValorAtingido,
                                    PossuiPedidoDevolucaoPacoteSemPacote: arg.Data.PossuiPedidoDevolucaoPacoteSemPacote
                                };
                                executarReST("CargaDocumentosFiscais/SalvarAuditoriaCarga", dados, function (r) {
                                    if (!r.Success) {
                                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                                    }
                                    else
                                        p.done();
                                });
                            }
                            else
                                p.done();
                        });
                    }, 500);
                } else {
                    p.done();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });


    return p;
}

function loadMultiplasChaves() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 5, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) { ExcluirChaveNotaClick(data); } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Chave", title: Localization.Resources.Cargas.Carga.Chave, width: "20%" },
        { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "10%" },
        { data: "MensagemRetorno", title: Localization.Resources.Cargas.Carga.MensagemRetorno, width: "20%" },
        { data: "ClassificacaoNFe", visible: false }
    ];
    _gridChaves = new BasicDataTable(_documentoEmissao.GridChaves.id, header, menuOpcoes, null, null, 5);
}

function posCarregarGridDocumentosParaEmissao() {
    if (_gridNotasParaEmissao != null && _gridNotasParaEmissao.NumeroRegistros() > 0) {
        if (_integracaoMinutaAvon != null && _integracaoMinutaAvon.Situacao.val() != EnumSituacaoMinutaAvon.Sucesso && _integracaoMinutaAvon.Situacao.val() != EnumSituacaoMinutaAvon.SalvandoNotasFiscais)
            HideIntegracaoMinutaAvon();

        _documentoEmissao.ExcluirNotasFiscais.visible(true);
    } else {
        if (_integracaoMinutaAvon != null)
            ShowIntegracaoMinutaAvon();

        _documentoEmissao.ExcluirNotasFiscais.visible(false);
    }

    if (_gridCTesParaEmissao != null && _gridCTesParaEmissao.NumeroRegistros() > 0)
        _documentoEmissao.ExcluirCTesSubContratacao.visible(true);
    else
        _documentoEmissao.ExcluirCTesSubContratacao.visible(false);

}

function preencherListasSelecaoChaves() {
    var chaves = new Array();

    $.each(_gridChaves.BuscarRegistros(), function (i, chave) {
        chaves.push({ Chave: chave });
    });

    _documentoEmissao.Chaves.val(JSON.stringify(chaves));
}

function RecarregarGridChaves() {
    var data = new Array();

    if (_documentoEmissao.Chaves.val() != '') {
        $.each(_documentoEmissao.Chaves.val(), function (i, chave) {
            var chaveGrid = new Object();

            chaveGrid.Codigo = chave.Codigo;
            chaveGrid.Chave = chave.Chave;
            chaveGrid.Numero = chave.Numero;
            chaveGrid.MensagemRetorno = chave.MensagemRetorno;
            chaveGrid.ClassificacaoNFe = chave.ClassificacaoNFe;

            data.push(chaveGrid);
        });
    }

    _gridChaves.CarregarGrid(data);
}

function VincularDocumentoNotasCompativeis() {
    var documentosSelecionados = _gridNotasCompativeis.ObterMultiplosSelecionados();

    if (documentosSelecionados.length <= 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.SelecioneAoMenosUmDocumentoParaRealizarVinculoCarga);
        return;
    }
    if ((_CONFIGURACAO_TMS.ExibirClassificacaoNFe || _cargaAtual.ClassificacaoNFeRemessaVenda.val()) && _documentoEmissao.ClassificacaoNFe.val() === EnumClassificacaoNFe.SemClassificacao) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Cargas.Carga.NecessarioInformarClassificacaoDeNFe);
        return;
    }

    if (_documentoEmissao.SelecionarTodos.val())
        documentosSelecionados = _gridNotasCompativeis.ObterMultiplosNaoSelecionados();
    else
        documentosSelecionados = _gridNotasCompativeis.ObterMultiplosSelecionados();

    var codigosDocumentosSelecionados = new Array();

    for (var i = 0; i < documentosSelecionados.length; i++)
        codigosDocumentosSelecionados.push(documentosSelecionados[i].Codigo);

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteVincularOsDocumentosSelecionadosEstaCarga, function () {
        var dados = {
            CargaPedido: _pedidoConsultaDocumento.CodigoCargaPedido.val(),
            Documentos: JSON.stringify(codigosDocumentosSelecionados),
            ClassificacaoNFe: _documentoEmissao.ClassificacaoNFe.val(),
            DataEmissaoInicial: _documentoEmissao.DataEmissaoInicial.val(),
            DataEmissaoFinal: _documentoEmissao.DataEmissaoFinal.val(),
            SelecionarTodos: _documentoEmissao.SelecionarTodos.val(),
            NumeroCarregamento: _documentoEmissao.NumeroCarregamento.val(),
            NumeroNotaInicial: _documentoEmissao.NumeroNotaInicial.val(),
            NumeroNotaFinal: _documentoEmissao.NumeroNotaFinal.val()
        };

        executarReST("DocumentoNF/VincularNotas", dados, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DocumentosVinculadosComSucesso);
                    carregarGridDocumentosParaEmissao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function VisibilidadeInformarCaptcha(dados) {
    if (dados.Chave == "" || dados.Chave == null)
        return false;
    else
        return true;
}

function VisibilidadeOpcaoDownloadDANFE(dados) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && !_cargaAtual.Mercosul.val())
        return true;
    else if (_CONFIGURACAO_TMS.PermitirDownloadDANFE && !_cargaAtual.Mercosul.val())
        return true;
    else
        return false;

}

function visibilidadeOpcaoDownloadXml() {
    return _CONFIGURACAO_TMS.PermitirDownloadXmlEtapaNfe;
}

function visibilidadeGridCCe(dados) {
    return _CONFIGURACAO_TMS.PermitirDownloadXmlEtapaNfe && dados.PossuiCartaCorrecao == true;
}

function VisibilidadeOpcaoExcluirNFe(dados) {
    if (dados.CTesEmitidos)
        return false;

    if (_cargaAtual.PermitirTransportadorEnviarNotasFiscais.val() && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        return true;

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais, _PermissoesPersonalizadasCarga) && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor)
        return false;

    return true;
}

function visibilidadeOpcaoDetalhesNaoConformidadesDocumentoEmissao(registroSelecionado) {
    return Boolean(registroSelecionado.SituacaoNaoConformidade);
}

function adicionarAnexoEtapaNFeClick() {
    if (!ValidarCamposObrigatorios(_anexoEtapaNFe)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    var arquivo = document.getElementById(_anexoEtapaNFe.Arquivo.id);

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoEtapaNFe.Descricao.val(),
        NomeArquivo: _anexoEtapaNFe.NomeArquivo.val(),
        Arquivo: arquivo.files[0],
        OcultarParaTransportador: _anexoEtapaNFe.OcultarParaTransportador.val()
    };

    var formData = new FormData();
    formData.append("Arquivo", anexo.Arquivo);
    formData.append("Descricao", anexo.Descricao);

    enviarArquivo("CargaNFeAnexos/AnexarArquivos", { Codigo: _cargaAtual.Codigo.val(), OcultarParaTransportador: anexo.OcultarParaTransportador }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _anexoEtapaNFe.Anexos.val(retorno.Data.Anexos);
                _gridAnexoEtapaNFe.CarregarGrid(_anexoEtapaNFe.Anexos.val());
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);

                LimparCampos(_anexoEtapaNFe);
                LimparCampoArquivoAnexoEtapaNFe();
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function MostrarModalAnexoEtapaNFe() {
    executarReST("Carga/BuscarAnexosDaCargaNFe", { CodigoCarga: _cargaAtual.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _anexoEtapaNFe = new AnexoEtapaNFe();
                KoBindings(_anexoEtapaNFe, "knockoutAnexoEtapaNFe");

                loadGridAnexoEtapaNFe();

                Global.abrirModal("knockoutAnexoEtapaNFe");

                _anexoEtapaNFe.Anexos.val(r.Data.Anexos);
                _gridAnexoEtapaNFe.CarregarGrid(r.Data.Anexos);
                if (_cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
                    _anexoEtapaNFe.Descricao.visible(false);
                    _anexoEtapaNFe.Arquivo.visible(false);
                    _anexoEtapaNFe.Adicionar.visible(false);
                    _anexoEtapaNFe.OcultarParaTransportador.visible(false);
                }
                else {
                    _anexoEtapaNFe.Descricao.visible(true);
                    _anexoEtapaNFe.Arquivo.visible(true);
                    _anexoEtapaNFe.Adicionar.visible(true);
                    _anexoEtapaNFe.OcultarParaTransportador.visible(true);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });

    $("#knockoutAnexoEtapaNFe").one('hidden.bs.modal', function () {
        LimparCampos(_anexoEtapaNFe);
        LimparCampoArquivoAnexoEtapaNFe();
        _anexoEtapaNFe.Anexos.val([]);
        _gridAnexoEtapaNFe.CarregarGrid([]);
    });
}

function LimparCampoArquivoAnexoEtapaNFe() {
    _anexoEtapaNFe.Arquivo.val("");
    var arquivo = document.getElementById(_anexoEtapaNFe.Arquivo.id);
    arquivo.value = null;
}

function NOTFISMichelinClick() {
    exibirConfirmacao(Localization.Resources.Cargas.Carga.NOTFISCMichelin, Localization.Resources.Cargas.Carga.TemCertezaQueConsultarTodosOsNOTFISAssociadosEstePedido, function () {
        executarReST("DocumentoNF/BuscarNOTFISMichelin", { CodigoCargaPedido: _pedidoConsultaDocumento.CodigoCargaPedido.val() }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    if (r.Data.DetalhesCarga != null)
                        veririficarSeCargaMudouTipoContratacao(r.Data.DetalhesCarga);

                    if (r.Data.Mensagem != null)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, r.Data.Mensagem);
                    else
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, r.Msg);

                    carregarGridDocumentosParaEmissao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        });
    });
}

function ExcluirIntercement() {
    exibirConfirmacao(Localization.Resources.Cargas.Carga.ExcluirEspelhoIntercement, Localization.Resources.Cargas.Carga.TemCertezaQueDesejaExcluirTodosOsEspelhos, function () {
        executarReST("DocumentoNF/ExcluirTodosEspelhos", { CodigoCargaPedido: _pedidoConsultaDocumento.CodigoCargaPedido.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.EspelhosExcluidosComSucesso);
                    _gridGridEspelhoIntercement.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function VerificarOpcaoRemoverAnexoNFe() {
    if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe)
        return true

    return false;
}

function RetornarEtapaClickManual(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaRetornarEstaCargaParaEtapaAnterior, function () {
        retornarEtapaManual(e);
    });
};

function retornarEtapaManual(e) {
    var data = { Carga: _pedidoConsultaDocumento.Carga.val(), PermiteRetornarEtapaComNfeVinculadas: true };

    executarReST("CargaNotasFiscais/RetornarEtapa", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                var carga = arg.Data;
                var idTab;
                $.each(_listaKnoutsCarga, function (i, knoutCarga) {
                    if (carga.Codigo == knoutCarga.Codigo.val()) {
                        idTab = knoutCarga.EtapaDadosTransportador.idTab;
                        e = knoutCarga;
                        return false;
                    }
                });
                EtapaDadosTransportadorLiberada(e);
                EtapaNotaFiscalDesabilitada(e);
                $("#" + idTab).trigger("click");
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.EtapaRetornadaComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function preecherDocumentosEmissaoAvancoCarga() {
    executarReST("CargaDocumentosFiscais/ObterToleranciaParaAvancoEtapaCarga", { CodigoCarga: _cargaAtual.Codigo.val(), CodigoCargaPedido: _pedidoConsultaDocumento.CodigoCargaPedido.val() }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _documentoEmissao.ToleranciaParaAvanco.val(retorno.Data.PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes);
            _documentoEmissao.Pacotes.val(retorno.Data.Pacotes);
            _documentoEmissao.CteAnteriores.val(retorno.Data.CTesAnteriores);
            _documentoEmissao.VerificarCorVerdeOuVermelho.val(retorno.Data.VerificarCorVerdeOuVermelho);

            if (retorno.Data.VerificarCorVerdeOuVermelho == true)
                $("#VerificarCores").css("color", "#00ff21");
            else
                $("#VerificarCores").css("color", "#dc3545");
        }
    });
}

function criarGridCTesParaEmissao(aplicarMenuOpcoesCTe) {
    let menuOpcoesCTe = null;
    if (aplicarMenuOpcoesCTe === true) {
        let excluirCTe = { descricao: Localization.Resources.Cargas.Carga.ExcluirCTe, id: "clasEditar", evento: "onclick", metodo: excluirCTeSubcontratacaoClick, tamanho: 7, icone: "" };
        let editarCTe = { descricao: Localization.Resources.Cargas.Carga.EditarCTe, id: guid(), evento: "onclick", metodo: abrirTelaEdicaoCTeSubcontratacao, tamanho: 7, icone: "" };
        let dimensaoCTe = { descricao: Localization.Resources.Cargas.Carga.Dimensoes, id: guid(), evento: "onclick", metodo: abrirTelaAtualizarDimensaoCTeSubcontratacao, tamanho: 7, icone: "" };

        if (_cargaAtual.TipoContratacaoCarga.val() == 2 && _cargaAtual.TipoOperacao.PossuiIntegracaoLogiun) {
            let indicarPaletes = { descricao: Localization.Resources.Cargas.Carga.IndicarPaletes, id: guid(), evento: "onclick", metodo: abrirTelaIndicarPaletes, tamanho: 7, icone: "" };
            menuOpcoesCTe = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [editarCTe, excluirCTe, dimensaoCTe, indicarPaletes] };

        }
        else {
            menuOpcoesCTe = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [editarCTe, excluirCTe, dimensaoCTe] };

        }

    }
    _gridCTesParaEmissao = new GridView(_documentoEmissao.GridCTe.idGrid, "DocumentoCTe/Pesquisa", _pedidoConsultaDocumento, menuOpcoesCTe, null);
    _documentoEmissao.Pedido.text(Localization.Resources.Cargas.Carga.CTesParaSubcontratacao);
}