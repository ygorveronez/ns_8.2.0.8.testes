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
/// <reference path="../../../ViewsScripts/Consultas/Estado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _coeficientePautaFiscal;
var _crudCoeficientePautaFiscal;
var _pesquisaCoeficientePautaFiscal;
var _gridCoeficientePautaFiscal;

var CoeficientePautaFiscal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Mes = PropertyEntity({ text: "*Mês: ", required: true, maxlength: 2 });
    this.Ano = PropertyEntity({ text: "*Ano: ", required: true, maxlength: 4 });
    this.Valor = PropertyEntity({ text: "*Valor: ", required: true, getType: typesKnockout.decimal });
    this.PercentualCoeficiente = PropertyEntity({ text: "*Percentual Coeficiente: ", required: true, getType: typesKnockout.decimal });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "*Estado:", idBtnSearch: guid(), required: true });
    this.Observacao = PropertyEntity({ text: "*Observação:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDCoeficientePautaFiscal = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaCoeficientePautaFiscal = function () {
    this.Mes = PropertyEntity({ text: "Mês: ", required: true, maxlength: 2 });
    this.Ano = PropertyEntity({ text: "Ano: ", required: true, maxlength: 4 });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Estado:", idBtnSearch: guid(), required: true });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCoeficientePautaFiscal.CarregarGrid();
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

function loadCoeficientePautaFiscal() {

    _pesquisaCoeficientePautaFiscal = new PesquisaCoeficientePautaFiscal();
    KoBindings(_pesquisaCoeficientePautaFiscal, "knockoutPesquisaCoeficientePautaFiscal", false, _pesquisaCoeficientePautaFiscal.Pesquisar.id);

    _coeficientePautaFiscal = new CoeficientePautaFiscal();
    KoBindings(_coeficientePautaFiscal, "knockoutCoeficientePautaFiscal");

    HeaderAuditoria("CoeficientePautaFiscal", _coeficientePautaFiscal);

    _crudCoeficientePautaFiscal = new CRUDCoeficientePautaFiscal();
    KoBindings(_crudCoeficientePautaFiscal, "knockoutCRUDCoeficientePautaFiscal");

    new BuscarEstados(_coeficientePautaFiscal.Estado);
    new BuscarEstados(_pesquisaCoeficientePautaFiscal.Estado);

    buscarCoeficientePautaFiscal();
}

function adicionarClick(e, sender) {
    Salvar(_coeficientePautaFiscal, "CoeficientePautaFiscal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridCoeficientePautaFiscal.CarregarGrid();
                limparCamposCoeficientePautaFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_coeficientePautaFiscal, "CoeficientePautaFiscal/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCoeficientePautaFiscal.CarregarGrid();
                limparCamposCoeficientePautaFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_coeficientePautaFiscal, "CoeficientePautaFiscal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCoeficientePautaFiscal.CarregarGrid();
                    limparCamposCoeficientePautaFiscal();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposCoeficientePautaFiscal();
}

function editarCoeficientePautaFiscalClick(itemGrid) {
    limparCamposCoeficientePautaFiscal();

    _coeficientePautaFiscal.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_coeficientePautaFiscal, "CoeficientePautaFiscal/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaCoeficientePautaFiscal.ExibirFiltros.visibleFade(false);

                _crudCoeficientePautaFiscal.Atualizar.visible(true);
                _crudCoeficientePautaFiscal.Excluir.visible(false);
                _crudCoeficientePautaFiscal.Cancelar.visible(true);
                _crudCoeficientePautaFiscal.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function buscarCoeficientePautaFiscal() {

    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCoeficientePautaFiscalClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridCoeficientePautaFiscal = new GridView(_pesquisaCoeficientePautaFiscal.Pesquisar.idGrid, "CoeficientePautaFiscal/Pesquisa", _pesquisaCoeficientePautaFiscal, menuOpcoes, null);
    _gridCoeficientePautaFiscal.CarregarGrid();
}

function limparCamposCoeficientePautaFiscal() {
    _crudCoeficientePautaFiscal.Atualizar.visible(false);
    _crudCoeficientePautaFiscal.Cancelar.visible(false);
    _crudCoeficientePautaFiscal.Excluir.visible(false);
    _crudCoeficientePautaFiscal.Adicionar.visible(true);
    LimparCampos(_coeficientePautaFiscal);
}
