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
/// <reference path="../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../ViewsScripts/Consultas/Justificativa.js" />
/// <reference path="../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoTaxaTerceiro.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTaxaTerceiro;
var _taxaTerceiro;
var _pesquisaTaxaTerceiro;

var PesquisaTaxaTerceiro = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid() });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTaxaTerceiro.CarregarGrid();
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

var TaxaTerceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.TipoTaxaTerceiro = PropertyEntity({ text: "*Tipo Taxa: ", val: ko.observable(EnumTipoTaxaTerceiro.PorKM), options: EnumTipoTaxaTerceiro.obterOpcoes(), def: EnumTipoTaxaTerceiro.PorKM });
    this.Valor = PropertyEntity({ text: "*Valor: ", getType: typesKnockout.decimal, maxlength: 18, required: ko.observable(true) });

    this.VigenciaInicial = PropertyEntity({ text: "Vigência Inicial: ", getType: typesKnockout.date });
    this.VigenciaFinal = PropertyEntity({ text: "Vigência Final: ", getType: typesKnockout.date });
    this.VigenciaInicial.dateRangeLimit = this.VigenciaFinal;
    this.VigenciaFinal.dateRangeInit = this.VigenciaInicial;

    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Terceiro:", idBtnSearch: guid(), required: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
};

var CRUDTaxaTerceiro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTaxaTerceiro() {
    _taxaTerceiro = new TaxaTerceiro();
    KoBindings(_taxaTerceiro, "knockoutCadastroTaxaTerceiro");

    HeaderAuditoria("TaxaTerceiro", _taxaTerceiro);

    _crudTaxaTerceiro = new CRUDTaxaTerceiro();
    KoBindings(_crudTaxaTerceiro, "knockoutCRUDTaxaTerceiro");

    _pesquisaTaxaTerceiro = new PesquisaTaxaTerceiro();
    KoBindings(_pesquisaTaxaTerceiro, "knockoutPesquisaTaxaTerceiro", false, _pesquisaTaxaTerceiro.Pesquisar.id);

    new BuscarVeiculos(_pesquisaTaxaTerceiro.Veiculo);
    new BuscarClientes(_pesquisaTaxaTerceiro.Terceiro);
    new BuscarJustificativas(_pesquisaTaxaTerceiro.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete]);
    new BuscarVeiculos(_taxaTerceiro.Veiculo);
    new BuscarClientes(_taxaTerceiro.Terceiro);
    new BuscarJustificativas(_taxaTerceiro.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete]);

    buscarTaxaTerceiro();
}

function adicionarClick(e, sender) {
    Salvar(_taxaTerceiro, "TaxaTerceiro/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTaxaTerceiro.CarregarGrid();
                limparCamposTaxaTerceiro();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_taxaTerceiro, "TaxaTerceiro/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTaxaTerceiro.CarregarGrid();
                limparCamposTaxaTerceiro();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a taxa " + _taxaTerceiro.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_taxaTerceiro, "TaxaTerceiro/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTaxaTerceiro.CarregarGrid();
                    limparCamposTaxaTerceiro();
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
    limparCamposTaxaTerceiro();
}

//*******MÉTODOS*******


function buscarTaxaTerceiro() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTaxaTerceiro, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTaxaTerceiro = new GridView(_pesquisaTaxaTerceiro.Pesquisar.idGrid, "TaxaTerceiro/Pesquisa", _pesquisaTaxaTerceiro, menuOpcoes);
    _gridTaxaTerceiro.CarregarGrid();
}

function editarTaxaTerceiro(taxaTerceiroGrid) {
    limparCamposTaxaTerceiro();
    _taxaTerceiro.Codigo.val(taxaTerceiroGrid.Codigo);
    BuscarPorCodigo(_taxaTerceiro, "TaxaTerceiro/BuscarPorCodigo", function (arg) {
        _pesquisaTaxaTerceiro.ExibirFiltros.visibleFade(false);
        _crudTaxaTerceiro.Atualizar.visible(true);
        _crudTaxaTerceiro.Cancelar.visible(true);
        _crudTaxaTerceiro.Excluir.visible(true);
        _crudTaxaTerceiro.Adicionar.visible(false);
    }, null);
}

function limparCamposTaxaTerceiro() {
    _crudTaxaTerceiro.Atualizar.visible(false);
    _crudTaxaTerceiro.Cancelar.visible(false);
    _crudTaxaTerceiro.Excluir.visible(false);
    _crudTaxaTerceiro.Adicionar.visible(true);
    LimparCampos(_taxaTerceiro);
}