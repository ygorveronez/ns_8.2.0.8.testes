/// <reference path="../../Enumeradores/EnumSituacaoLoteCliente.js" />
/// <reference path="Etapa.js" />
/// <reference path="SelecaoClientes.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLoteCliente;
var _loteCliente;
var _CRUDLoteCliente;
var _pesquisaLoteCliente;

var LoteCliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLoteCliente.EmCriacao), def: EnumSituacaoLoteCliente.EmCriacao, text: "Situação: " });
};

var CRUDLoteCliente = function () {
    this.Limpar = PropertyEntity({ eventClick: LimparLoteClienteClick, type: types.event, text: "Limpar (Gerar Novo Lote)", idGrid: guid(), visible: ko.observable(false) });
};

var PesquisaLoteCliente = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Numero = PropertyEntity({ text: "Nº do Lote:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoLoteCliente.ObterOpcoesPesquisa(), def: "", text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLoteCliente.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};


//*******EVENTOS*******

function LoadLoteCliente() {
    _loteCliente = new LoteCliente();
    HeaderAuditoria("LoteCliente", _loteCliente);

    _CRUDLoteCliente = new CRUDLoteCliente();
    KoBindings(_CRUDLoteCliente, "knockoutCRUD");

    _pesquisaLoteCliente = new PesquisaLoteCliente();
    KoBindings(_pesquisaLoteCliente, "knockoutPesquisaLoteCliente", false, _pesquisaLoteCliente.Pesquisar.id);

    LoadEtapasLoteCliente();
    LoadSelecaoClientes();
    loadIntegracoes();

    BuscarLoteCliente();
}

function LimparLoteClienteClick(e, sender) {
    LimparCamposLoteCliente();
    GridSelecaoClientes();
}


//*******MÉTODOS*******

function BuscarLoteCliente() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarLoteClienteClick, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        tamanho: 7,
        opcoes: [editar]
    };

    _gridLoteCliente = new GridView(_pesquisaLoteCliente.Pesquisar.idGrid, "LoteCliente/Pesquisa", _pesquisaLoteCliente, menuOpcoes);
    _gridLoteCliente.CarregarGrid();
}

function EditarLoteClienteClick(itemGrid) {
    LimparCamposLoteCliente();
    _pesquisaLoteCliente.ExibirFiltros.visibleFade(false);
    BuscarLoteClientePorCodigo(itemGrid.Codigo);
}

function BuscarLoteClientePorCodigo(codigo, cb) {
    executarReST("LoteCliente/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            EditarLoteCliente(arg.Data);
            EditarSelecaoClientes(arg.Data);
            SetarEtapasLoteCliente();
            recarregarIntegracoes();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarLoteCliente(data) {
    _loteCliente.Codigo.val(data.Codigo);
    _loteCliente.Situacao.val(data.Situacao);
    _CRUDLoteCliente.Limpar.visible(true);
}

function LimparCamposLoteCliente() {
    LimparCampos(_loteCliente);
    _CRUDLoteCliente.Limpar.visible(false);
    _loteCliente.Situacao.val(EnumSituacaoLoteCliente.EmCriacao);
    SetarEtapasLoteCliente();
    LimparCamposSelecaoClientes();
    $("#" + _etapaLoteCliente.Etapa1.idTab).click();
}