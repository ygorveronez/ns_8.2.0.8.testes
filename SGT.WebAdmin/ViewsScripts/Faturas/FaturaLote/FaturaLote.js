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
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _faturaLote;
var _CRUDFaturaLote;
var _gridSelecaoCarga;
var _PermissoesPersonalizadas;

var FaturaLote = function () {
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.GrupoPessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.GrupoPessoa, text: "*Tipo de Pessoa: ", issue: 306, enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Pessoa:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Grupo de Pessoa:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        _faturaLote.Pessoa.visible(false);
        _faturaLote.GrupoPessoas.visible(false);
        LimparCampoEntity(_faturaLote.Pessoa);
        LimparCampoEntity(_faturaLote.GrupoPessoas);

        if (novoValor === EnumTipoPessoaGrupo.Pessoa)
            _faturaLote.Pessoa.visible(true);
        else
            _faturaLote.GrupoPessoas.visible(true);
    });

    this.DataFatura = PropertyEntity({ text: "*Data Fatura: ", val: ko.observable(""), getType: typesKnockout.date, issue: 331, required: true, def: ko.observable(Global.DataAtual()), enable: ko.observable(false) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, enable: ko.observable(true), issue: 331, required: false });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date, enable: ko.observable(true), required: false, issue: 331 });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Tipo de Operação:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroBooking = PropertyEntity({ text: "Nº Booking:", maxlength: 300, enable: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Origem:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Destino:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoPropostaMultimodal = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, url: "CancelamentoCargaLote/ObterTodosTipoPropostaMultimodal", text: ko.observable("*Tipo da Proposta: "), options: ko.observable(new Array()), visible: ko.observable(true) });
    this.PedidoViagemDirecao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Navio/Viagem/Direção:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Terminal Origem:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Terminal Destino:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), maxlength: 255, required: false, enable: ko.observable(true), type: types.local, visible: ko.observable(true) });

    this.ApenasFaturaExclusiva = PropertyEntity({ type: types.bool, text: "Apenas CTes que possuem permissão exclusiva", def: false, val: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });

    this.Pesquisa = PropertyEntity({ eventClick: PesquisaConhecimentosClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.Conhecimentos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.ListaConhecimentos = PropertyEntity({ text: "", val: ko.observable(""), required: false, enable: ko.observable(true), getType: typesKnockout.string, visible: ko.observable(false) });
    this.ConsultarTodos = PropertyEntity({ val: ko.observable(false), def: false, required: false, enable: ko.observable(true), getType: typesKnockout.bool, visible: ko.observable(false) });

    this.TipoCTe = PropertyEntity({ val: ko.observable(EnumTipoCTe.Normal), options: EnumTipoCTe.ObterOpcoesFaturamento(), def: EnumTipoCTe.Normal, text: "Tipo CT-e: ", enable: ko.observable(true) });
};

var CRUDFaturaLote = function () {
    this.GerarFatura = PropertyEntity({ eventClick: GerarFaturaClick, type: types.event, text: ko.observable("Gerar Faturas"), visible: ko.observable(true) });
    this.GerarFaturaExclusiva = PropertyEntity({ eventClick: GerarFaturaExclusivaClick, type: types.event, text: ko.observable("Gerar Faturas Exclusivas"), visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadFaturaLote() {
    _faturaLote = new FaturaLote();
    KoBindings(_faturaLote, "knockoutFaturaLote");

    _faturaLote.DataFatura.val(Global.DataAtual());

    _CRUDFaturaLote = new CRUDFaturaLote();
    KoBindings(_CRUDFaturaLote, "knockoutCRUDFaturaLote");

    new BuscarPedidoViagemNavio(_faturaLote.PedidoViagemDirecao);
    new BuscarTipoTerminalImportacao(_faturaLote.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_faturaLote.TerminalDestino);
    new BuscarLocalidades(_faturaLote.Origem);
    new BuscarLocalidades(_faturaLote.Destino);
    new BuscarTiposOperacao(_faturaLote.TipoOperacao);
    new BuscarClientes(_faturaLote.Pessoa);
    new BuscarGruposPessoas(_faturaLote.GrupoPessoas);

    if (_CONFIGURACAO_TMS.PermiteFaturamentoPermissaoExclusiva) {
        _CRUDFaturaLote.GerarFaturaExclusiva.visible(false);
        _faturaLote.ApenasFaturaExclusiva.visible(true);
    }

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FaturamentoLote_RetirarCamposObrigatorios, _PermissoesPersonalizadas)) {
        _faturaLote.TerminalOrigem.required = false;
        _faturaLote.PedidoViagemDirecao.required = false;
        _faturaLote.TipoPropostaMultimodal.required = false;

        _faturaLote.TerminalOrigem.text("Terminal Origem:");
        _faturaLote.PedidoViagemDirecao.text("Navio/Viagem/Direção:");
        _faturaLote.TipoPropostaMultimodal.text("Tipo da Proposta: ");
    }

    CriarGridConsultaConhecimento();
    //buscarConhecimentos();
}

function GerarFaturaExclusivaClick(e, sender) {
    if (ValidarCamposObrigatorios(_faturaLote)) {

        if (_faturaLote.SelecionarTodos.val())
            _faturaLote.ConsultarTodos.val(true);
        else {
            _faturaLote.ListaConhecimentos.val(PreencherListaCodigos());
            _faturaLote.ConsultarTodos.val(false);
        }

        var data = RetornarObjetoPesquisa(_faturaLote);
        exibirConfirmacao("Atenção!", "Realmente deseja gerar as faturas para os clientes exclusivos com os filtros selecionados?", function () {
            executarReST("FaturaLote/GerarFaturaLoteClientesExclusivos", data, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de solicitação de faturamento em lote realizado com sucesso, favor acompanhe na tela de Fatura.");
                    limparCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            });
        });
    }
}

function GerarFaturaClick(e, sender) {
    if (ValidarCamposObrigatorios(_faturaLote)) {

        if (_faturaLote.SelecionarTodos.val())
            _faturaLote.ConsultarTodos.val(true);
        else {
            _faturaLote.ListaConhecimentos.val(PreencherListaCodigos());
            _faturaLote.ConsultarTodos.val(false);
        }

        var data = RetornarObjetoPesquisa(_faturaLote);
        exibirConfirmacao("Atenção!", "Realmente deseja gerar as faturas com os filtros selecionados?", function () {
            executarReST("FaturaLote/GerarFaturaLote", data, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de solicitação de faturamento em lote realizado com sucesso, favor acompanhe na tela de Fatura.");
                    limparCampos();
                    CriarGridConsultaConhecimento();
                    _gridSelecaoCarga.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            });
        });
    }
}

function CriarGridConsultaConhecimento() {
    var somenteLeitura = false;

    _faturaLote.SelecionarTodos.visible(true);
    _faturaLote.SelecionarTodos.val(false);

    var baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5, opcoes: [baixarDACTE] };

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _faturaLote.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    _gridSelecaoCarga = new GridView(_faturaLote.Conhecimentos.idGrid, "FaturaLote/PesquisaConhecimentos", _faturaLote, menuOpcoes, null, null, null, null, null, multiplaescolha);
}

function PreencherListaCodigos() {
    //ListaConhecimentos
    var codigos = new Array();
    var titulosSelecionados = _gridSelecaoCarga.ObterMultiplosSelecionados();
    $.each(titulosSelecionados, function (i, carga) {
        codigos.push({ Codigo: carga.DT_RowId });
    });
    return JSON.stringify(codigos);
}

function buscarConhecimentos() {
    _gridSelecaoCarga.CarregarGrid();
}

function PesquisaConhecimentosClick(e, sender) {
    if (ValidarCamposObrigatorios(_faturaLote)) {
        buscarConhecimentos();
    }
    else
        exibirCamposObrigatorio();
}

//*******MÉTODOS*******

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function limparCampos() {
    LimparCampos(_faturaLote);
    _faturaLote.Observacao.val("");
    _faturaLote.DataFatura.val(Global.DataAtual());
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}