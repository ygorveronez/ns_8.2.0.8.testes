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
/// <reference path="../../Enumeradores/EnumParametroRateioFormula.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRateioFormula;
var _rateioFormula;
var _pesquisaRateioFormula;

var PesquisaRateioFormula = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRateioFormula.CarregarGrid();
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

var RateioFormula = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ",issue: 586, required: true });
    this.ParametroRateioFormula = PropertyEntity({ val: ko.observable(EnumParametroRateioFormula.peso), options: EnumParametroRateioFormula.obterOpcoes(), text: "*Parâmetro base para o rateio: ",issue: 256, def: EnumParametroRateioFormula.peso });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 556 });
    this.RatearPrimeiroIgualmenteEntrePedidos = PropertyEntity({ text: "Primeiramente deseja ratear igualmente entre os pedidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ArredondarParaNumeroParMaisProximo = PropertyEntity({ text: "Arredondar para o número par mais próximo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RatearEmBlocoDeEmissao = PropertyEntity({ text: "Ratear em bloco de emissão?", getType: typesKnockout.bool, val: ko.observable(false) });    
    this.PercentualAcrescentarPesoTotalCarga = PropertyEntity({ text: ko.observable("Percentual acrescentar peso total da carga:"), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, val: ko.observable(""), def: "", maxlength: 6, required: ko.observable(false) });
    this.ExigirConferenciaManual = PropertyEntity({ text: "Exigir Conferência Manual", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });

    this.ParametroRateioFormula.val.subscribe(function (novoValor) {
        if (novoValor == EnumParametroRateioFormula.FatorPonderacaoDistanciaPeso) {
            _rateioFormula.PercentualAcrescentarPesoTotalCarga.required(true);
            _rateioFormula.PercentualAcrescentarPesoTotalCarga.text("*Percentual acrescentar peso total da carga:");
        }
        else {
            _rateioFormula.PercentualAcrescentarPesoTotalCarga.required(false);
            _rateioFormula.PercentualAcrescentarPesoTotalCarga.text("Percentual acrescentar peso total da carga:");
        }

        if (novoValor == EnumParametroRateioFormula.PorCTe) {
            _rateioFormula.ExigirConferenciaManual.visible(true);
        }
        else {
            _rateioFormula.ExigirConferenciaManual.visible(false);
        }
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadRateioFormula() {

    _rateioFormula = new RateioFormula();
    KoBindings(_rateioFormula, "knockoutCadastroRateioFormula");

    _pesquisaRateioFormula = new PesquisaRateioFormula();
    KoBindings(_pesquisaRateioFormula, "knockoutPesquisaRateioFormula", false, _pesquisaRateioFormula.Pesquisar.id);

    HeaderAuditoria("RateioFormula", _rateioFormula);

    buscarRateioFormulas();

}

function adicionarClick(e, sender) {
    Salvar(e, "RateioFormula/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridRateioFormula.CarregarGrid();
                limparCamposRateioFormula();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "RateioFormula/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridRateioFormula.CarregarGrid();
                limparCamposRateioFormula();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a rateioFormula " + _rateioFormula.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_rateioFormula, "RateioFormula/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridRateioFormula.CarregarGrid();
                limparCamposRateioFormula();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposRateioFormula();
}

//*******MÉTODOS*******


function buscarRateioFormulas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRateioFormula, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRateioFormula = new GridView(_pesquisaRateioFormula.Pesquisar.idGrid, "RateioFormula/Pesquisa", _pesquisaRateioFormula, menuOpcoes, null);
    _gridRateioFormula.CarregarGrid();
}

function editarRateioFormula(rateioFormulaGrid) {
    limparCamposRateioFormula();
    _rateioFormula.Codigo.val(rateioFormulaGrid.Codigo);
    BuscarPorCodigo(_rateioFormula, "RateioFormula/BuscarPorCodigo", function (arg) {
        _pesquisaRateioFormula.ExibirFiltros.visibleFade(false);
        _rateioFormula.Atualizar.visible(true);
        _rateioFormula.Cancelar.visible(true);
        _rateioFormula.Excluir.visible(true);
        _rateioFormula.Adicionar.visible(false);
    }, null);
}

function limparCamposRateioFormula() {
    _rateioFormula.Atualizar.visible(false);
    _rateioFormula.Cancelar.visible(false);
    _rateioFormula.Excluir.visible(false);
    _rateioFormula.Adicionar.visible(true);
    LimparCampos(_rateioFormula);
}
