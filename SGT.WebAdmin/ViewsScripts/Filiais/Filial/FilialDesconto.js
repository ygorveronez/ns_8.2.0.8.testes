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

//*******MAPEAMENTO KNOUCKOUT*******

var _listaFilialDesconto;
var _gridFilialDesconto = null;
var _FilialDesconto;

var ListaFilialDesconto = function () {
    this.AdicionarDesconto = PropertyEntity({ eventClick: exibirDescontoModalClick, type: types.event, text: ko.observable(Localization.Resources.Filiais.Filial.AdicionarDesconto), visible: true });
    this.Grid = PropertyEntity({ type: types.local });
}

var FilialDesconto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorDesconto = PropertyEntity({ text: Localization.Resources.Filiais.Filial.ValorDesconto.getRequiredFieldDescription(), type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true), required: true, configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Filiais.Filial.ModeloVeicularCarga.getRequiredFieldDescription()), issue: 44, required: true, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Filiais.Filial.TipoOperacao.getRequiredFieldDescription()), required: true, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarDescontoApoliceSeguroClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
}

//*******EVENTOS*******
function criarGridFilialDesconto() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirFilialDescontoClick(_FilialDesconto.Desconto, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoModeloVeicular", visible: false },
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Filiais.Filial.ModeloVeicularCarga, width: "16%" },
        { data: "CodigoTipoOperacao", visible: false },
        { data: "DescricaoTipoOperacao", title: Localization.Resources.Filiais.Filial.TipoOperacao, width: "16%" },
        { data: "ValorDesconto", title: Localization.Resources.Filiais.Filial.ValorDesconto, width: "26%" }

    ];

    _gridFilialDesconto = new BasicDataTable(_listaFilialDesconto.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}

function loadFilialDesconto() {
    _listaFilialDesconto = new ListaFilialDesconto();
    KoBindings(_listaFilialDesconto, "knockoutFilialDesconto");

    _FilialDesconto = new FilialDesconto();
    KoBindings(_FilialDesconto, "knockoutCadastroFilialDesconto");

    new BuscarModelosVeicularesCarga(_FilialDesconto.ModeloVeicularCarga);
    new BuscarTiposOperacao(_FilialDesconto.TipoOperacao);

    criarGridFilialDesconto();
}

function ExcluirFilialDescontoClick(knoutFilialDesconto, data) {

    var descontosGrid = _gridFilialDesconto.BuscarRegistros();

    for (var i = 0; i < descontosGrid.length; i++) {
        if (data.Codigo == descontosGrid[i].Codigo) {
            descontosGrid.splice(i, 1);
            break;
        }
    }

    _gridFilialDesconto.CarregarGrid(descontosGrid);
}


function fecharDescontoModal() {
    Global.fecharModal('divModalDesconto');
}

function exibirDescontoModal() {
    Global.abrirModal('divModalDesconto');
    $("#divModalDesconto").one('hidden.bs.modal', function () {
        LimparCampos(_FilialDesconto);
    });
}


function exibirDescontoModalClick(e, sender) {
    exibirDescontoModal()
}


function obterFilialDescontoSalvar() {
    return {
        Codigo: _FilialDesconto.Codigo.val(),
        CodigoModeloVeicular: _FilialDesconto.ModeloVeicularCarga.codEntity(),
        ValorDesconto: _FilialDesconto.ValorDesconto.val(),
        DescricaoModeloVeicular: _FilialDesconto.ModeloVeicularCarga.val(),
        CodigoTipoOperacao: _FilialDesconto.TipoOperacao.codEntity(),
        DescricaoTipoOperacao: _FilialDesconto.TipoOperacao.val()
    };
}

function adicionarDescontoApoliceSeguroClick() {
    if (ValidarCamposObrigatorios(_FilialDesconto)) {

        var apoliceSeguro = _gridFilialDesconto.BuscarRegistros();

        apoliceSeguro.push(obterFilialDescontoSalvar());

        _gridFilialDesconto.CarregarGrid(apoliceSeguro);

        fecharDescontoModal();
    }
    else
        exibirMensagemCamposObrigatorio();
}
