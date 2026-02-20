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
/// <reference path="../../Enumeradores/EnumSituacaoEnvioDocumentacao.js" />
/// <reference path="../../Enumeradores/EnumModalEnvioDocumentacao.js" />
/// <reference path="../../Enumeradores/EnumFormaEnvioDocumentacao.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoMultimodal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _selecaoCarga;
var _CRUDImpressaoLoteCarga;
var _gridSelecaoCarga;
var _PermissoesPersonalizadas;

var _situacaoEnvioDocumentacao = [
    { text: "Todos", value: EnumSituacaoEnvioDocumentacao.Todos },
    { text: "Enviado", value: EnumSituacaoEnvioDocumentacao.Enviado },
    { text: "Não Enviado", value: EnumSituacaoEnvioDocumentacao.NaoEnviado }
];

var SelecaoCarga = function () {
    this.NumeroBooking = PropertyEntity({ text: "Nº Booking: ", getType: typesKnockout.string, enable: ko.observable(true), required: false });
    this.NumeroOS = PropertyEntity({ text: "Nº O.S.: ", getType: typesKnockout.string, enable: ko.observable(true), required: false });
    this.NumeroFiscal = PropertyEntity({ text: "Nº Fiscal: ", getType: typesKnockout.int, enable: ko.observable(true), required: false });
    this.NumeroControle = PropertyEntity({ text: "Nº Controle: ", getType: typesKnockout.string, enable: ko.observable(true), required: false });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Container:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.PedidoViagemDirecao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Navio/Viagem/Direção:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Terminal Origem:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable("Terminal Destino:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoPropostaMultimodal = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, url: "CancelamentoCargaLote/ObterTodosTipoPropostaMultimodal", params: { Tipo: 0, Ativo: _statusPesquisa.Todos }, text: "*Tipo da Proposta: ", options: ko.observable(new Array()), visible: ko.observable(true) });

    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Grupo de Pessoa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.ProvedorOS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Provedor da O.S.:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.SituacaoEnvioDocumentacao = PropertyEntity({ val: ko.observable(EnumSituacaoEnvioDocumentacao.Todos), options: _situacaoEnvioDocumentacao, def: EnumSituacaoEnvioDocumentacao.Todos, text: "Situação do Envio: " });

    this.TipoServico = PropertyEntity({ val: ko.observable(EnumTipoServicoMultimodal.Todos), def: EnumTipoServicoMultimodal.Todos, text: "Tipo Serviço:", options: EnumTipoServicoMultimodal.obterOpcoesImpressaoLote(), getType: typesKnockout.selectMultiple });
    this.FoiAnulado = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, text: "Foi Anulado?", options: EnumSimNaoPesquisa.obterOpcoesPesquisa() });
    this.FoiSubstituido = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, text: "Foi Substituído?", options: EnumSimNaoPesquisa.obterOpcoesPesquisa() });
    this.ModalEnvio = PropertyEntity({ val: ko.observable(EnumModalEnvioDocumentacao.PortoDestino), def: EnumModalEnvioDocumentacao.PortoDestino, text: "Modal do Envio", options: EnumModalEnvioDocumentacao.obterOpcoes() });

    this.FormaEnvioDocumentacao = PropertyEntity({ val: ko.observable(EnumFormaEnvioDocumentacao.Padrao), def: EnumFormaEnvioDocumentacao.Padrao, text: "Forma de envio do E-mail", options: EnumFormaEnvioDocumentacao.obterOpcoesImpressaoLote() });
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


var CRUDImpressaoLoteCarga = function () {
    this.EnviarPorEmail = PropertyEntity({ eventClick: EnviarPorEmailClick, type: types.event, text: ko.observable("Enviar por E-mail"), visible: ko.observable(true) });
    this.GerarPDF = PropertyEntity({ eventClick: GerarPDFClick, type: types.event, text: ko.observable("Gerar PDF dos conhecimentos"), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadImpressaoLoteCarga() {
    _selecaoCarga = new SelecaoCarga();
    KoBindings(_selecaoCarga, "knockoutSelecaoCarga");

    _CRUDImpressaoLoteCarga = new CRUDImpressaoLoteCarga();
    KoBindings(_CRUDImpressaoLoteCarga, "knockoutCRUDImpressaoLoteCarga");

    new BuscarClientes(_selecaoCarga.ProvedorOS);
    new BuscarGruposPessoas(_selecaoCarga.GrupoPessoa);
    new BuscarPedidoViagemNavio(_selecaoCarga.PedidoViagemDirecao);
    new BuscarTipoTerminalImportacao(_selecaoCarga.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_selecaoCarga.TerminalDestino);
    new BuscarContainers(_selecaoCarga.Container);

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ImpressaoLoteCarga_ObrigarSelecionarTerminalDestino, _PermissoesPersonalizadas) || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ImpressaoCargaLote_ObrigarInformarTerminalDestino, _PermissoesPersonalizadas))) {
        _selecaoCarga.TerminalDestino.required(true);
        _selecaoCarga.TerminalDestino.text("*Terminal Destino:");
    }
    else {
        _selecaoCarga.TerminalDestino.required(false);
        _selecaoCarga.TerminalDestino.text("Terminal Destino:");
    }

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

function GerarPDFClick(e, sender) {
    if (ValidarCamposObrigatorios(_selecaoCarga)) {
        if (!_selecaoCarga.SelecionarTodos.val()) {
            GerarImpressaoLoteCarga(false);
        } else {
            GerarImpressaoLoteCarga(true);
        }
    }
    else
        exibirCamposObrigatorio();
}

//*******MÉTODOS*******

function GerarImpressaoLoteCarga(todosSelecionado) {
    var data = null;

    _selecaoCarga.ListaConhecimentos.val("");
    if (!todosSelecionado)
        _selecaoCarga.ListaConhecimentos.val(PreencherListaCodigos());
    data = RetornarObjetoPesquisa(_selecaoCarga);

    //if (todosSelecionado) {
    //    data = RetornarObjetoPesquisa(_selecaoCarga);
    //}
    //else
    //    data = {
    //        ListaConhecimentos: PreencherListaCodigos(), InformarEmailEnvio: _selecaoCarga.InformarEmailEnvio.val(), EmailEnvio: _selecaoCarga.EmailEnvio.val(), FormaEnvioDocumentacao: _selecaoCarga.FormaEnvioDocumentacao.val(), ModalEnvio: _selecaoCarga.ModalEnvio.val()
    //    };

    exibirConfirmacao("Atenção!", "Realmente deseja gerar o PDF dos conhecimentos selecionadas?", function () {
        executarReST("ImpressaoLoteCarga/GerarImpressaoLoteCarga", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de geração do PDF iniciado com sucesso, favor aguarde a notificação da conclusão.");
                //limparCamposSelecaoCarga();
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

    //if (todosSelecionado) {
    //    data = RetornarObjetoPesquisa(_selecaoCarga);
    //}
    //else
    //    data = { ListaConhecimentos: PreencherListaCodigos(), InformarEmailEnvio: _selecaoCarga.InformarEmailEnvio.val(), EmailEnvio: _selecaoCarga.EmailEnvio.val(), FormaEnvioDocumentacao: _selecaoCarga.FormaEnvioDocumentacao.val(), ModalEnvio: _selecaoCarga.ModalEnvio.val() };

    exibirConfirmacao("Atenção!", "Realmente deseja enviar por e-mail em lote para os conhecimentos selecionados?", function () {
        executarReST("ImpressaoLoteCarga/EnviarPorEmail", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mail iniciado com sucesso.");
                //limparCamposSelecaoCarga();
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

    _gridSelecaoCarga = new GridView(_selecaoCarga.Conhecimentos.idGrid, "ImpressaoLoteCarga/PesquisaConhecimentosParaImpressao", _selecaoCarga, menuOpcoes, null, null, null, null, null, multiplaescolha);
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