/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Consultas/NaturezaOperacao.js" />
/// <reference path="../../Enumeradores/EnumTipoCampoCCe.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _statusCampoCCePesquisa = [{ text: "Ativo", value: "A" },
{ text: "Inativo", value: "I" },
{ text: "Todos", value: "" }];

var _statusCampoCCe = [{ text: "Ativo", value: "A" },
{ text: "Inativo", value: "I" }];

var _indicadorRepeticaoCCe = [{ text: "Não", value: false },
{ text: "Sim", value: true }];

var _tipoCampoCCe = [{ text: "Texto", value: EnumTipoCampoCCe.Texto },
{ text: "Inteiro", value: EnumTipoCampoCCe.Inteiro },
{ text: "Decimal", value: EnumTipoCampoCCe.Decimal },
{ text: "Data", value: EnumTipoCampoCCe.Data },
{ text: "Seleção", value: EnumTipoCampoCCe.Selecao }];

var _gridCampoCCe;
var _campoCCe;
var _pesquisaCampoCCe;

var PesquisaCampoCCe = function () {

    this.Descricao = PropertyEntity({ text: "Descrição: ", getType: typesKnockout.string, maxlength: 200 });
    this.NomeCampo = PropertyEntity({ text: "Nome do Campo: ", getType: typesKnockout.string, maxlength: 20 });
    this.GrupoCampo = PropertyEntity({ text: "Grupo do Campo: ", getType: typesKnockout.string, maxlength: 20 });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusCampoCCePesquisa, def: "A" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCampoCCe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

}

var CampoCCe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", getType: typesKnockout.string, maxlength: 200, required: true });
    this.NomeCampo = PropertyEntity({ text: "*Nome do Campo: ", getType: typesKnockout.string, maxlength: 20, required: true });
    this.GrupoCampo = PropertyEntity({ text: "*Grupo do Campo: ", getType: typesKnockout.string, maxlength: 20, required: true });
    this.IndicadorRepeticao = PropertyEntity({ text: "*Campo se Repete: ", val: ko.observable(0), options: _indicadorRepeticaoCCe, def: false });
    this.TipoCampo = PropertyEntity({ text: "*Tipo do Campo: ", val: ko.observable(0), options: _tipoCampoCCe, def: false });
    this.QuantidadeCaracteres = PropertyEntity({ text: "*Qtd. Caracteres: ", getType: typesKnockout.int, maxlength: 3, visible: ko.observable(false) });
    this.QuantidadeInteiros = PropertyEntity({ text: "*Qtd. Inteiros: ", getType: typesKnockout.int, maxlength: 2, visible: ko.observable(false) });
    this.QuantidadeDecimais = PropertyEntity({ text: "*Qtd. Decimais: ", getType: typesKnockout.int, maxlength: 2, visible: ko.observable(false) });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(0), options: _statusCampoCCe, def: "A" });
    this.TipoCampoCCeAutomatica = PropertyEntity({ text: "Tipo do Campo Para CCe Automática: ", val: ko.observable(0), options: EnumTipoCampoCCeAutomatico.ObterOpcoes(), def: EnumTipoCampoCCeAutomatico.Nenhum, visible: ko.observable(_ativarGeracaoCCePelaRolagemWS) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadCampoCCe() {

    _campoCCe = new CampoCCe();
    KoBindings(_campoCCe, "knockoutCadastroCampoCCe");

    _pesquisaCampoCCe = new PesquisaCampoCCe();
    KoBindings(_pesquisaCampoCCe, "knockoutPesquisaCampoCCe", false, _pesquisaCampoCCe.Pesquisar.id);

    HeaderAuditoria("CampoCCe", _campoCCe);

    buscarCampoCCes();

    alterarTipoCampo();
    $("#" + _campoCCe.TipoCampo.id).change(function () {
        alterarTipoCampo();
    })
}

function alterarTipoCampo() {
    switch (_campoCCe.TipoCampo.val()) {
        case EnumTipoCampoCCe.Decimal:
            _campoCCe.QuantidadeCaracteres.visible(false);
            _campoCCe.QuantidadeDecimais.visible(true);
            _campoCCe.QuantidadeInteiros.visible(true);
            break;
        case EnumTipoCampoCCe.Inteiro:
            _campoCCe.QuantidadeCaracteres.visible(false);
            _campoCCe.QuantidadeDecimais.visible(false);
            _campoCCe.QuantidadeInteiros.visible(true);
            break;
        case EnumTipoCampoCCe.Texto:
            _campoCCe.QuantidadeCaracteres.visible(true);
            _campoCCe.QuantidadeDecimais.visible(false);
            _campoCCe.QuantidadeInteiros.visible(false);
            break;
        default:
            _campoCCe.QuantidadeCaracteres.visible(false);
            _campoCCe.QuantidadeDecimais.visible(false);
            _campoCCe.QuantidadeInteiros.visible(false);
            break;

    }
}

function adicionarClick(e, sender) {
    Salvar(e, "CampoCCe/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados salvos com sucesso!");
                _gridCampoCCe.CarregarGrid();
                limparCamposCampoCCe();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "CampoCCe/Salvar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados salvos com sucesso!");
            _gridCampoCCe.CarregarGrid();
            limparCamposCampoCCe();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposCampoCCe();
}

//*******MÉTODOS*******

function buscarCampoCCes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCampoCCe, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridCampoCCe = new GridView(_pesquisaCampoCCe.Pesquisar.idGrid, "CampoCCe/Consultar", _pesquisaCampoCCe, menuOpcoes, null);
    _gridCampoCCe.CarregarGrid();
}

function editarCampoCCe(campoCCeGrid) {
    limparCamposCampoCCe();
    _campoCCe.Codigo.val(campoCCeGrid.Codigo);
    BuscarPorCodigo(_campoCCe, "CampoCCe/BuscarPorCodigo", function (arg) {
        alterarTipoCampo();
        _pesquisaCampoCCe.ExibirFiltros.visibleFade(false);
        _campoCCe.Atualizar.visible(true);
        _campoCCe.Cancelar.visible(true);
        _campoCCe.Adicionar.visible(false);
    }, null);
}

function limparCamposCampoCCe() {
    _campoCCe.Atualizar.visible(false);
    _campoCCe.Cancelar.visible(false);
    _campoCCe.Adicionar.visible(true);
    LimparCampos(_campoCCe);
    alterarTipoCampo();
}

