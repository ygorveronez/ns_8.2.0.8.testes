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
/// <reference path="../../Enumeradores/EnumDirecaoViagemMultimodal.js" />
/// <reference path="../../Consultas/Navio.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridViagemNavio;
var _viagemNavio;
var _pesquisaViagemNavio;

var PesquisaViagemNavio = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridViagemNavio.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ViagemNavio = function () {
    
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição: ", required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(false) });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", required: ko.observable(false), maxlength: 50 });
    this.NumeroViagem = PropertyEntity({ text: "*Número Viagem: ", getType: typesKnockout.int, maxlength: 11, required: ko.observable(true) });

    this.Schedules = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.Direcao = PropertyEntity({ val: ko.observable(""), options: EnumDirecaoViagemMultimodal.obterOpcoes(), def: "", text: "*Direção: ", required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Navio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Navio:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.ConsumoPlugs = PropertyEntity({ getType: typesKnockout.decimal, text: "Consumo Plugs:", configDecimal: { precision: 4, allowZero: true }, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio) });
    this.ConsumoTeus = PropertyEntity({ text: " Consumo Teus: ", getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: true }, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio) });
    this.ConsumoTons = PropertyEntity({ text: " Consumo Tons: ", getType: typesKnockout.decimal, maxlength: 11, configDecimal: { precision: 4, allowZero: true },  visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio)});
    this.CapacidadePlug = PropertyEntity({ getType: typesKnockout.decimal, text: "Capacidade Plugs:", enable: ko.observable(false), configDecimal: { precision: 4, allowZero: true }, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio) });
    this.CapacidadeTeus = PropertyEntity({ text: " Capacidade Teus: ", enable: ko.observable(false), getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: true }, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio) });
    this.CapacidadeTons = PropertyEntity({ text: " Capacidade Tons: ", enable: ko.observable(false), getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: true }, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio) });
};

var CRUDViagemNavio = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadViagemNavio() {
    _viagemNavio = new ViagemNavio();
    KoBindings(_viagemNavio, "knockoutCadastroViagemNavio");

    HeaderAuditoria("PedidoViagemNavio", _viagemNavio);

    _crudViagemNavio = new CRUDViagemNavio();
    KoBindings(_crudViagemNavio, "knockoutCRUDViagemNavio");

    _pesquisaViagemNavio = new PesquisaViagemNavio();
    KoBindings(_pesquisaViagemNavio, "knockoutPesquisaViagemNavio", false, _pesquisaViagemNavio.Pesquisar.id);

    new BuscarNavios(_viagemNavio.Navio, RetornoBuscarNavios);

    buscarViagemNavio();    
    LoadSchedules();
}

function RetornoBuscarNavios(data) {
    _viagemNavio.Navio.codEntity(data.Codigo);
    _viagemNavio.Navio.val(data.Descricao);

    _viagemNavio.CapacidadePlug.val(data.CapacidadePlug);
    _viagemNavio.CapacidadeTeus.val(data.CapacidadeTeus);
    _viagemNavio.CapacidadeTons.val(data.CapacidadeTons);
}

function adicionarClick(e, sender) {
    Salvar(_viagemNavio, "PedidoViagemNavio/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridViagemNavio.CarregarGrid();
                limparCamposViagemNavio();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_viagemNavio, "PedidoViagemNavio/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridViagemNavio.CarregarGrid();
                limparCamposViagemNavio();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Viagem de Navio" + _viagemNavio.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_viagemNavio, "PedidoViagemNavio/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridViagemNavio.CarregarGrid();

                GridSchedules();

                limparCamposViagemNavio();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposViagemNavio();
}

//*******MÉTODOS*******


function buscarViagemNavio() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarViagemNavio, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridViagemNavio = new GridView(_pesquisaViagemNavio.Pesquisar.idGrid, "PedidoViagemNavio/Pesquisa", _pesquisaViagemNavio, menuOpcoes, null);
    _gridViagemNavio.CarregarGrid();
}

function editarViagemNavio(viagemNavioGrid) {
    limparCamposViagemNavio();
    _viagemNavio.Codigo.val(viagemNavioGrid.Codigo);
    BuscarPorCodigo(_viagemNavio, "PedidoViagemNavio/BuscarPorCodigo", function (arg) {
        _pesquisaViagemNavio.ExibirFiltros.visibleFade(false);
        _crudViagemNavio.Atualizar.visible(true);
        _crudViagemNavio.Cancelar.visible(true);
        _crudViagemNavio.Excluir.visible(true);
        _crudViagemNavio.Adicionar.visible(false);
        _viagemNavio.Descricao.visible(true);

        GridSchedules();
    }, null);
}

function limparCamposViagemNavio() {
    _crudViagemNavio.Atualizar.visible(false);
    _crudViagemNavio.Cancelar.visible(false);
    _crudViagemNavio.Excluir.visible(false);
    _crudViagemNavio.Adicionar.visible(true);
    _viagemNavio.Descricao.visible(false);    
    LimparCampos(_viagemNavio);
    LimparCampos(_schedule);

    GridSchedules();
}