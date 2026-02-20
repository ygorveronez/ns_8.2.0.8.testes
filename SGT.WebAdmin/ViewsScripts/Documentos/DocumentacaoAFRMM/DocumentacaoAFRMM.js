/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Container.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEnvioDocumentacao.js" />
/// <reference path="../../Enumeradores/EnumModalEnvioDocumentacao.js" />
/// <reference path="../../Enumeradores/EnumFormaEnvioDocumentacao.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoMultimodal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _selecaoCarga;
var _CRUDDocumentacaoAFRMM;
var _gridSelecaoCarga;

var _situacaoEnvioDocumentacao = [
    { text: "Todos", value: EnumSituacaoEnvioDocumentacao.Todos },
    { text: "Enviado", value: EnumSituacaoEnvioDocumentacao.Enviado },
    { text: "Não Enviado", value: EnumSituacaoEnvioDocumentacao.NaoEnviado },
    { text: "Falha no Envio", value: EnumSituacaoEnvioDocumentacao.Falha }
];

var SelecaoCarga = function () {
    this.NumeroBooking = PropertyEntity({ text: "Nº Booking: ", getType: typesKnockout.string, enable: ko.observable(true), required: false });
    this.NumeroControle = PropertyEntity({ text: "Nº Controle: ", getType: typesKnockout.string, enable: ko.observable(true), required: false });
    this.PedidoViagemDirecao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Navio/Viagem/Direção:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.DescargaPODInicial = PropertyEntity({ text: "Descarga POD Inicial:", required: false, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DescargaPODFinal = PropertyEntity({ text: "Descarga POD Final:", required: false, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.EnvioDocInicial = PropertyEntity({ text: "Envio DOC. Inicial:", required: false, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.EnvioDocFinal = PropertyEntity({ text: "Envio DOC. Final:", required: false, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });

    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Porto Origem:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable("Porto Destino:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.SituacaoEnvioDocumentacao = PropertyEntity({ val: ko.observable(EnumSituacaoEnvioDocumentacao.Todos), options: _situacaoEnvioDocumentacao, def: EnumSituacaoEnvioDocumentacao.Todos, text: "Situação do Envio: " });

    this.TipoServico = PropertyEntity({ val: ko.observable(EnumTipoServicoMultimodal.Todos), def: EnumTipoServicoMultimodal.Todos, text: "Tipo Serviço:", options: EnumTipoServicoMultimodal.obterOpcoesSemNumero(), getType: typesKnockout.selectMultiple });

    this.NumeroManifesto = PropertyEntity({ text: "Nº Manifesto: ", getType: typesKnockout.string, enable: ko.observable(true), required: false });
    this.NumeroManifestoTransbordo = PropertyEntity({ text: "Nº Manifesto do Transbordo: ", getType: typesKnockout.string, enable: ko.observable(true), required: false });
    this.NumeroCEMercante = PropertyEntity({ text: "Nº CE Mercante: ", getType: typesKnockout.string, enable: ko.observable(true), required: false });

    this.InformarEmailEnvio = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.EmailEnvio = PropertyEntity({ text: "Informar o e-mail manualmente", required: false, enable: ko.observable(false), visible: ko.observable(true), maxlength: 2000 });

    this.InformarEmailEnvio.val.subscribe(function (novoValor) {
        _selecaoCarga.EmailEnvio.enable(novoValor);
    });

    this.Conhecimentos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.Pesquisa = PropertyEntity({ eventClick: PesquisaConhecimentosClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });
    this.ListaConhecimentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};


var CRUDDocumentacaoAFRMM = function () {
    this.EnviarPorEmail = PropertyEntity({ eventClick: EnviarPorEmailClick, type: types.event, text: ko.observable("Enviar por E-mail"), visible: ko.observable(true) });
    this.EnviarParaFTP = PropertyEntity({ eventClick: EnviarParaFTPClick, type: types.event, text: ko.observable("Enviar para FTP"), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadDocumentacaoAFRMM() {
    _selecaoCarga = new SelecaoCarga();
    KoBindings(_selecaoCarga, "knockoutSelecaoCarga");

    _CRUDDocumentacaoAFRMM = new CRUDDocumentacaoAFRMM();
    KoBindings(_CRUDDocumentacaoAFRMM, "knockoutCRUDDocumentacaoAFRMM");
        
    new BuscarPedidoViagemNavio(_selecaoCarga.PedidoViagemDirecao);
    new BuscarPorto(_selecaoCarga.PortoOrigem);
    new BuscarPorto(_selecaoCarga.PortoDestino);

    CriarGridConsultaConhecimento();
    buscarConhecimentosParaImpressao();
}

function PesquisaConhecimentosClick(e, sender) {
    if (ValidarCamposObrigatorios(_selecaoCarga)) {
        buscarConhecimentosParaImpressao();
    }
    else
        exibirCamposObrigatorio();
}

function EnviarPorEmailClick(e, sender) {
    if (ValidarCamposObrigatorios(_selecaoCarga)) {
        if (!_selecaoCarga.SelecionarTodos.val()) {
            EnviarPorEmail(false);
        } else {
            EnviarPorEmail(true);
        }
    }
    else
        exibirCamposObrigatorio();
}

function EnviarParaFTPClick(e, sender) {
    if (ValidarCamposObrigatorios(_selecaoCarga)) {
        if (!_selecaoCarga.SelecionarTodos.val()) {
            GerarDocumentacaoAFRMM(false);
        } else {
            GerarDocumentacaoAFRMM(true);
        }
    }
    else
        exibirCamposObrigatorio();
}

//*******MÉTODOS*******

function GerarDocumentacaoAFRMM(todosSelecionado) {
    var data = null;

    _selecaoCarga.ListaConhecimentos.val("");
    if (!todosSelecionado)
        _selecaoCarga.ListaConhecimentos.val(PreencherListaCodigos());
    data = RetornarObjetoPesquisa(_selecaoCarga);

    exibirConfirmacao("Atenção!", "Realmente deseja enviar para o FTP todos os conhecimentos selecionadas?", function () {
        executarReST("DocumentacaoAFRMM/GerarDocumentacaoAFRMM", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de inicio do envio ao FTP com sucesso, favor aguarde a notificação da conclusão.");                
                buscarConhecimentosParaImpressao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}

function EnviarPorEmail(todosSelecionado) {
    var data = null;

    _selecaoCarga.ListaConhecimentos.val("");
    if (!todosSelecionado)
        _selecaoCarga.ListaConhecimentos.val(PreencherListaCodigos());
    data = RetornarObjetoPesquisa(_selecaoCarga);
   
    exibirConfirmacao("Atenção!", "Realmente deseja enviar por e-mail em lote para os conhecimentos selecionados?", function () {
        executarReST("DocumentacaoAFRMM/EnviarPorEmail", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mail iniciado com sucesso.");                
                buscarConhecimentosParaImpressao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}

function PreencherListaCodigos() {
    var codigos = new Array();
    var titulosSelecionados = _gridSelecaoCarga.ObterMultiplosSelecionados();
    $.each(titulosSelecionados, function (i, carga) {
        codigos.push({ Codigo: carga.Codigo });
    });
    return JSON.stringify(codigos);
}

function CriarGridConsultaConhecimento() {
    var somenteLeitura = false;

    _selecaoCarga.SelecionarTodos.visible(true);
    _selecaoCarga.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoCarga.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    var auditar = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ConhecimentoDeTransporteEletronico", "Codigo"), tamanho: "5", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5, opcoes: [auditar] };

    //_gridSelecaoCarga = new GridView(_selecaoCarga.Conhecimentos.idGrid, "DocumentacaoAFRMM/PesquisaConhecimentosParaDocumentacao", _selecaoCarga, menuOpcoes, null, null, null, null, null, multiplaescolha);

    var configExportacao = {
        url: "DocumentacaoAFRMM/ExportarPesquisaConhecimentosParaDocumentacao",
        titulo: "Exportar"
    };

    _gridSelecaoCarga = new GridViewExportacao(_selecaoCarga.Conhecimentos.idGrid, "DocumentacaoAFRMM/PesquisaConhecimentosParaDocumentacao", _selecaoCarga, menuOpcoes, configExportacao, null, null, multiplaescolha);
}

function buscarConhecimentosParaImpressao() {
    _selecaoCarga.SelecionarTodos.visible(true);
    _selecaoCarga.SelecionarTodos.val(false);

    _gridSelecaoCarga.AtualizarRegistrosSelecionados([]);
    _gridSelecaoCarga.CarregarGrid();
}

function limparCamposSelecaoCarga() {
    LimparCampos(_selecaoCarga);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}