/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoToken.js" />
/// <reference path="../../../js/Global/" />

//#region Variaveis Globais
var _pesquisaRegraAutorizacaoToken;
var _regraAutorizacaoToken;
var _regraAutorizacaoUsario;
var _gridRegrarAutorizacaoToken;
var _gridAprovadoreRegrarAutorizacaoToken;
//#endregion

//#region Constructores
var PesquisaRegraOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data Inicio", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Limite", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situacao" });
    this.Descricao = PropertyEntity({ text: "Descrição", val: ko.observable(""), def: "" });
    this.EtapaAutorizacao = PropertyEntity({ text: "Etapa da Autorização", val: ko.observable(EnumEtapaAutorizacaoToken.Todas), options: EnumEtapaAutorizacaoToken.obterOpcoesPesquisa(), def: EnumEtapaAutorizacaoToken.Todas });
    this.Aprovador = PropertyEntity({ text: "Aprovador", issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    //this.TipoOcorrencia = PropertyEntity({ text: "Tipo Ocorrencia", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegrarAutorizacaoToken.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}
function RegraAutorizacaoToken() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "Descrição", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigencia", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: "*Numero Aprovadores", issue: 873, getType: typesKnockout.int, maxlength: 3 });
    this.NumeroReprovadores = PropertyEntity({ text: "Numero Reprovadores", getType: typesKnockout.int, required: false, maxlength: 3 });
    this.DiasPrazoAprovacao = PropertyEntity({ text: "Prazo Aprovação Dias", getType: typesKnockout.int });
    this.AprovacaoAutomaticaAposDias = PropertyEntity({ text: "Prazo Para Aprovação Automatica", getType: typesKnockout.int, visible: ko.observable(true) });
    this.Observacoes = PropertyEntity({ text: "Observação", issue: 593, maxlength: 2000 });
    this.EtapaAutorizacao = PropertyEntity({ text: "Etapa Autorização", issue: 908, val: ko.observable(EnumEtapaAutorizacaoToken.AprovacaoToken), options: EnumEtapaAutorizacaoToken.obterOpcoesPesquisa(), def: EnumEtapaAutorizacaoToken.AprovacaoToken });
    this.TipoDiasAprovacao = PropertyEntity({ text: "Tipo de Dias", issue: 908, val: ko.observable(EnumTipoDiasAprovacao.DiasCorridos), options: EnumTipoDiasAprovacao.obterOpcoes(), def: EnumTipoDiasAprovacao.DiasCorridos, visible: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Situação", issue: 557, visible: ko.observable(true) });
    this.PrioridadeAprovacao = PropertyEntity({ val: ko.observable(EnumPrioridadeAutorizacao.Zero), options: EnumPrioridadeAutorizacao.obterOpcoes(), def: EnumPrioridadeAutorizacao.Zero, text: "Prioridade" });
    this.EnviarLinkParaAprovacaoPorEmail = PropertyEntity({ val: ko.observable(false), def: false, text: "Enviar Link para aprovação por e-mail" });

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridAprovadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });
    this.GridAprovadores.val.subscribe(function () {
        _regraAutorizacaoToken.Aprovadores.val(JSON.stringify(_regraAutorizacaoToken.GridAprovadores.val()))
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraAutorizacaoTokenClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraAutorizacaoTokenClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraAutorizacaoTokenClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraAutorizacaoTokenClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}
//#endregion

//#region Funções Carregadoras
function loadRegraAutorizacaoToken() {
    _pesquisaRegraAutorizacaoToken = new PesquisaRegraOcorrencia();
    KoBindings(_pesquisaRegraAutorizacaoToken, "knockoutPesquisaRegrasAutorizacaoToken")

    _regraAutorizacaoToken = new RegraAutorizacaoToken();
    KoBindings(_regraAutorizacaoToken, "knockoutCadastroRegrasAutorizacaoToken")

    loadGridAprovadores()

    new BuscarFuncionario(_regraAutorizacaoToken.GridAprovadores, null, _gridAprovadoreRegrarAutorizacaoToken);
    new BuscarFuncionario(_pesquisaRegraAutorizacaoToken.Aprovador);
    carregarGridAprovadores();
    loadRegrasAutorizacaoToken()
}

function loadRegrasAutorizacaoToken() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraAutoriacaoClick, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegrarAutorizacaoToken = new GridView(_pesquisaRegraAutorizacaoToken.Pesquisar.idGrid, "RegrasAutorizacaoToken/Pesquisa", _pesquisaRegraAutorizacaoToken, menuOpcoes);
    _gridRegrarAutorizacaoToken.CarregarGrid();
}
function loadGridAprovadores() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "15",
                icone: "",
                metodo: function (data) {
                    RemoverAprovadorRegraAutoriacaoTokenClick(_regraAutorizacaoToken.GridAprovadores, data);
                }
            }
        ]
    };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: "Usuario", width: "100%", className: "text-align-left" }
    ];
    _gridAprovadoreRegrarAutorizacaoToken = new BasicDataTable(_regraAutorizacaoToken.GridAprovadores.idGrid, header, menuOpcoes, null, null, 5);
    _gridAprovadoreRegrarAutorizacaoToken.CarregarGrid([]);
}
//#endregion

//#region Funções auxiliares

function carregarGridAprovadores() {
    var aprovadores = _regraAutorizacaoToken.GridAprovadores.val();
    _gridAprovadoreRegrarAutorizacaoToken.CarregarGrid(aprovadores);
}

function editarRegraAutoriacaoClick(e) {
    _regraAutorizacaoToken.Codigo.val(e.Codigo);
    BuscarPorCodigo(_regraAutorizacaoToken, "RegrasAutorizacaoToken/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraAutorizacaoToken.ExibirFiltros.visibleFade(false);
        _gridAprovadoreRegrarAutorizacaoToken.CarregarGrid(_regraAutorizacaoToken.Aprovadores.val());
        ToogleOpcoesCrud(false);


    });
}

function RemoverAprovadorRegraAutoriacaoTokenClick(e, sender) {
    var aprovadores = _gridAprovadoreRegrarAutorizacaoToken.BuscarRegistros();
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    _gridAprovadoreRegrarAutorizacaoToken.CarregarGrid(aprovadores);
}
//#endregion

//#region Funções CRUD
function adicionarRegraAutorizacaoTokenClick(e, sender) {
    SalvarRegrasAutorizacaoToken("RegrasAutorizacaoToken/Adicionar", "Regra Cadastrada Com Sucesso", e, sender);
}
function excluirRegraAutorizacaoTokenClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esta regra?", function () {
        ExcluirPorCodigo(_regraAutorizacaoToken, "RegrasAutorizacaoToken/ExcluirPorCodigo", function (arg) {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

            if (!arg.Data)
                return exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);

            exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro Excluido com sucesso!");
            _gridRegrarAutorizacaoToken.CarregarGrid();
            cancelarRegraAutorizacaoTokenClick();

        }, null);
    });
}
function atualizarRegraAutorizacaoTokenClick(e, sender) {
    SalvarRegrasAutorizacaoToken("RegrasAutorizacaoToken/Atualizar", "Atualizado com sucesso", e, sender);
}
function cancelarRegraAutorizacaoTokenClick() {
    LimparCampos(_regraAutorizacaoToken);
    _regraAutorizacaoToken.GridAprovadores.val([]);
    _gridAprovadoreRegrarAutorizacaoToken.CarregarGrid([]);
    ToogleOpcoesCrud(true);
}

function ToogleOpcoesCrud(visibel) {
    _regraAutorizacaoToken.Adicionar.visible(visibel);
    _regraAutorizacaoToken.Cancelar.visible(!visibel);
    _regraAutorizacaoToken.Atualizar.visible(!visibel);
    _regraAutorizacaoToken.Excluir.visible(!visibel);
}

function SalvarRegrasAutorizacaoToken(url, msg, e, sender) {
    let numeroAprovadores = _regraAutorizacaoToken.NumeroAprovadores.val();
    let aprovadoresSelecionado = _gridAprovadoreRegrarAutorizacaoToken.BuscarRegistros();

    if (aprovadoresSelecionado.length < numeroAprovadores)
        return exibirMensagem(tipoMensagem.atencao, "Aviso", `Precisa informar no minimo ${numeroAprovadores} aprovadores para cadastrar esta regra!`);

    _regraAutorizacaoToken.Aprovadores.val(JSON.stringify(aprovadoresSelecionado))
    Salvar(_regraAutorizacaoToken, url, function (arg) {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        if (!arg.Data)
            return exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", msg);
        _gridRegrarAutorizacaoToken.CarregarGrid();
        cancelarRegraAutorizacaoTokenClick();

    }, sender, ExibirCamposObrigatorio)
}

function ExibirCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatorios", "Por favor precisa informar os campos obrigatorios");
}
//#endregion