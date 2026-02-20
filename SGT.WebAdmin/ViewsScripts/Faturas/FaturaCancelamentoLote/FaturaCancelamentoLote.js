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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
///// <reference path="../../Consultas/Container.js" /> Tipo de Pessoa
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Container.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _faturaCancelamentoLote;
var _gridSelecaoFatura;
var _PermissoesPersonalizadas;

var FaturaCancelamentoLote = function () {
    this.PedidoViagemDirecao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: ko.observable("*Navio/Viagem/Direção:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: ko.observable("*Terminal Origem:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoPropostaMultimodal = PropertyEntity({ val: ko.observable(new Array()), required: ko.observable(true), def: new Array(), getType: typesKnockout.selectMultiple, url: "CancelamentoCargaLote/ObterTodosTipoPropostaMultimodal", params: { Tipo: 0, Ativo: _statusPesquisa.Todos }, text: ko.observable("*Tipo da Proposta: "), options: ko.observable(new Array()), visible: ko.observable(true) });
    this.FaturadoAR = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Nao), options: EnumSimNaoPesquisa.obterOpcoes(), def: EnumSimNaoPesquisa.Nao, text: "Faturado pelo AR? ", required: false });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo de Pessoa: " });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Booking = PropertyEntity({ text: "Booking:" });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Container:", idBtnSearch: guid() });
    this.DataFaturaInicial = PropertyEntity({ text: "Data Fatura Inicial:", getType: typesKnockout.date, enable: ko.observable(true), val: ko.observable(""), def: ko.observable("") });
    this.DataFaturaFinal = PropertyEntity({ text: "Data Fatura Final:", getType: typesKnockout.date, enable: ko.observable(true), val: ko.observable(""), def: ko.observable("") });

    this.Faturas = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaFaturas = PropertyEntity({ text: "", val: ko.observable(""), required: false, enable: ko.observable(true), getType: typesKnockout.string, visible: ko.observable(false) });
    this.ConsultarTodos = PropertyEntity({ val: ko.observable(false), def: false, required: false, enable: ko.observable(true), getType: typesKnockout.bool, visible: ko.observable(false) });

    this.Pesquisa = PropertyEntity({ eventClick: PesquisaFaturasClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });

    this.Motivo = PropertyEntity({ text: "*Motivo do Cancelamento:", val: ko.observable(""), issue: 632, maxlength: 255, required: false, enable: ko.observable(true) });
    this.GerarCancelamento = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: ko.observable("Gerar Cancelamentos"), visible: ko.observable(true) });

    this.DataFaturaFinal.dateRangeInit = this.DataFaturaInicial;
    this.DataFaturaInicial.dateRangeLimit = this.DataFaturaFinal;

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoPessoaGrupo.Pessoa) {
            _faturaCancelamentoLote.Pessoa.visible(true);
            _faturaCancelamentoLote.GrupoPessoa.visible(false);
            LimparCampoEntity(_faturaCancelamentoLote.GrupoPessoa);
        } else {
            _faturaCancelamentoLote.GrupoPessoa.visible(true);
            _faturaCancelamentoLote.Pessoa.visible(false);
            LimparCampoEntity(_faturaCancelamentoLote.Pessoa);
        }
    });
};

//*******EVENTOS*******

function loadFaturaCancelamentoLote() {
    _faturaCancelamentoLote = new FaturaCancelamentoLote();
    KoBindings(_faturaCancelamentoLote, "knockoutFaturaCancelamentoLote");

    new BuscarPedidoViagemNavio(_faturaCancelamentoLote.PedidoViagemDirecao);
    new BuscarTipoTerminalImportacao(_faturaCancelamentoLote.TerminalOrigem);
    new BuscarClientes(_faturaCancelamentoLote.Pessoa);
    new BuscarGruposPessoas(_faturaCancelamentoLote.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarContainers(_faturaCancelamentoLote.Container);

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FaturamentoLote_RetirarCamposObrigatorios, _PermissoesPersonalizadas)) {
        _faturaCancelamentoLote.TerminalOrigem.required(false);
        _faturaCancelamentoLote.PedidoViagemDirecao.required(false);
        _faturaCancelamentoLote.TipoPropostaMultimodal.required(false);

        _faturaCancelamentoLote.TerminalOrigem.text("Terminal Origem:");
        _faturaCancelamentoLote.PedidoViagemDirecao.text("Navio/Viagem/Direção:");
        _faturaCancelamentoLote.TipoPropostaMultimodal.text("Tipo da Proposta: ");
    }

    buscarFaturasParaCancelamento();
}

function PesquisaFaturasClick(e, sender) {
    if (ValidarCamposObrigatorios(_faturaCancelamentoLote)) {
        buscarFaturasParaCancelamento();
    }
}

function AdicionarClick(e, sender) {
    if (ValidarCamposObrigatorios(_faturaCancelamentoLote)) {
        if (!_faturaCancelamentoLote.SelecionarTodos.val()) {
            GerarCancelamentoLote(false);
        } else {
            GerarCancelamentoLote(true);
        }
    }
    else
        exibirCamposObrigatorio();
}

function TipoPessoaChange(e, sender) {
    if (_faturaCancelamentoLote.TipoPessoa.val() == 1) {
        _faturaCancelamentoLote.Pessoa.required = true;
        _faturaCancelamentoLote.Pessoa.visible(true);
        _faturaCancelamentoLote.GrupoPessoa.required = false;
        _faturaCancelamentoLote.GrupoPessoa.visible(false);
        LimparCampoEntity(_faturaCancelamentoLote.GrupoPessoa);
    } else if (_fatura.TipoPessoa.val() == 2) {
        _faturaCancelamentoLote.Pessoa.required = false;
        _faturaCancelamentoLote.Pessoa.visible(false);
        _faturaCancelamentoLote.GrupoPessoa.required = true;
        _faturaCancelamentoLote.GrupoPessoa.visible(true);
        LimparCampoEntity(_faturaCancelamentoLote.Pessoa);
    }
}

//*******MÉTODOS*******

function GerarCancelamentoLote(todosSelecionado) {
    var data = null;

    if (todosSelecionado) {
        _faturaCancelamentoLote.ConsultarTodos.val(true);
        data = RetornarObjetoPesquisa(_faturaCancelamentoLote);
    }
    else {
        _faturaCancelamentoLote.ConsultarTodos.val(false);
        _faturaCancelamentoLote.ListaFaturas.val(PreencherListaCodigos());
        data = RetornarObjetoPesquisa(_faturaCancelamentoLote);
    }

    exibirConfirmacao("Atenção!", "Realmente deseja cancelar as cargas selecionadas?", function () {
        executarReST("FaturaCancelamentoLote/GerarCancelamentoLote", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de solicitação de cancelamento realizado com sucesso, favor acompanhe na tela de Fatura.");
                limparCamposFaturaCancelamentoLote();
                buscarFaturasParaCancelamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}

function PreencherListaCodigos() {
    var codigos = new Array();
    var titulosSelecionados = _gridSelecaoFatura.ObterMultiplosSelecionados();
    $.each(titulosSelecionados, function (i, carga) {
        codigos.push({ Codigo: carga.DT_RowId });
    });
    return JSON.stringify(codigos);
}

function buscarFaturasParaCancelamento() {
    var somenteLeitura = false;

    _faturaCancelamentoLote.SelecionarTodos.visible(true);
    _faturaCancelamentoLote.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _faturaCancelamentoLote.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    _gridSelecaoFatura = new GridView(_faturaCancelamentoLote.Faturas.idGrid, "FaturaCancelamentoLote/PesquisaFaturasParaCancelamento", _faturaCancelamentoLote, null, null, null, null, null, null, multiplaescolha);
    _gridSelecaoFatura.CarregarGrid();
}

function limparCamposFaturaCancelamentoLote() {
    LimparCampos(_faturaCancelamentoLote);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}