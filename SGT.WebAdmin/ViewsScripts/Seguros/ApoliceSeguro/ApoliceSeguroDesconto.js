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

var _listaApoliceSeguroDesconto;
var _gridApoliceSeguroDesconto = null;
var _apoliceSeguroDesconto;
var _codigoApoliceSeguroEditada;

var ListaApoliceSeguroDesconto = function () {
    this.AdicionarDesconto = PropertyEntity({ eventClick: exibirDescontoModalClick, type: types.event, text: ko.observable("Adicionar Desconto"), visible: true });
    this.ImportarDescontos = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "ApoliceSeguro/ImportarDescontos",
        UrlConfiguracao: "ApoliceSeguro/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O051_ImportacaoDescontos,
        ParametrosRequisicao: function () { return { Codigo: _codigoApoliceSeguroEditada} },
        CallbackImportacao: function (arg) {
            //RecarregarGrid
            BuscarPorCodigo(_apoliceSeguro, "ApoliceSeguro/BuscarPorCodigo", function (arg) {
                _codigoApoliceSeguroEditada = _apoliceSeguro.Codigo.val();
                recarregarGridApoliceSeguroDesconto();
            }, null);
        }
    });

    this.Grid = PropertyEntity({ type: types.local });
}

var ApoliceSeguroDesconto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorDesconto = PropertyEntity({ text: "Valor Desconto:", type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true), required: false, configDecimal: { precision: 2, allowZero: false, allowNegative: false} });
    this.PercentualDesconto = PropertyEntity({ text: "Percentual Desconto:", type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true), required: false, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, maxlength: 5 });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Modelo Veicular de Carga:"), issue: 44, required: true, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Filial:"), required: true, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Tipo Operação:"),  required: true, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarDescontoApoliceSeguroClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });
}

//*******EVENTOS*******
function criarGridApoliceSeguroDesconto() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirApoliceSeguroDescontoClick(_apoliceSeguroDesconto.Desconto, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoModeloVeicular", visible: false },
        { data: "DescricaoModeloVeicular", title: "Modelo Veicular", width: "16%" },
        { data: "CodigoFilial", visible: false },
        { data: "DescricaoFilial", title: "Filial", width: "16%" },
        { data: "CodigoTipoOperacao", visible: false },
        { data: "DescricaoTipoOperacao", title: "Tipo Operação", width: "16%" },
        { data: "ValorDesconto", title: "Valor Desconto", width: "26%" },
        { data: "PercentualDesconto", title: "% Desconto", width: "26%" }        
    ];
    var configExportacao = {
        url: "ApoliceSeguro/ExportarDescontos",
        btnText: "Exportar excel (Dados Salvos)",
        funcaoObterParametros: function () {
            return { Codigo: _codigoApoliceSeguroEditada };
        }
    }

    _gridApoliceSeguroDesconto = new BasicDataTable(_listaApoliceSeguroDesconto.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, null, null, null, null, null, null, null, configExportacao);
}

function loadApoliceSeguroDesconto() {
    _listaApoliceSeguroDesconto = new ListaApoliceSeguroDesconto();
    KoBindings(_listaApoliceSeguroDesconto, "knockoutApoliceSeguroDesconto");

    _apoliceSeguroDesconto = new ApoliceSeguroDesconto();
    KoBindings(_apoliceSeguroDesconto, "knockoutCadastroApoliceSeguroDesconto");

    BuscarModelosVeicularesCarga(_apoliceSeguroDesconto.ModeloVeicularCarga);
    BuscarFilial(_apoliceSeguroDesconto.Filial);
    BuscarTiposOperacao(_apoliceSeguroDesconto.TipoOperacao);

    criarGridApoliceSeguroDesconto();
}

function ExcluirApoliceSeguroDescontoClick(knoutApoliceSeguroDesconto, data) {

    var descontosGrid = _gridApoliceSeguroDesconto.BuscarRegistros();

    for (var i = 0; i < descontosGrid.length; i++) {
        if (data.Codigo == descontosGrid[i].Codigo) {
            descontosGrid.splice(i, 1);
            break;
        }
    }

    _gridApoliceSeguroDesconto.CarregarGrid(descontosGrid);
}


function fecharDescontoModal() {
    Global.fecharModal('divModalDesconto');
}

function exibirDescontoModal() {
    Global.abrirModal('divModalDesconto');
    $("#divModalDesconto").one('hidden.bs.modal', function () {
        LimparCampos(_apoliceSeguroDesconto);
    });
}


function exibirDescontoModalClick(e, sender) {
    exibirDescontoModal() 
}


function obterApoliceSeguroDescontoSalvar() {
    return {
        Codigo: _apoliceSeguroDesconto.Codigo.val(),
        CodigoModeloVeicular: _apoliceSeguroDesconto.ModeloVeicularCarga.codEntity(),
        ValorDesconto: _apoliceSeguroDesconto.ValorDesconto.val(),
        PercentualDesconto: _apoliceSeguroDesconto.PercentualDesconto.val(),
        DescricaoModeloVeicular: _apoliceSeguroDesconto.ModeloVeicularCarga.val(),
        CodigoFilial: _apoliceSeguroDesconto.Filial.codEntity(),
        DescricaoFilial: _apoliceSeguroDesconto.Filial.val(),
        CodigoTipoOperacao: _apoliceSeguroDesconto.TipoOperacao.codEntity(),
        DescricaoTipoOperacao: _apoliceSeguroDesconto.TipoOperacao.val()
    };
}

function adicionarDescontoApoliceSeguroClick() {
    if (ValidarCamposObrigatorios(_apoliceSeguroDesconto)) {

        var apoliceSeguro = _gridApoliceSeguroDesconto.BuscarRegistros();
        var novaApolice = obterApoliceSeguroDescontoSalvar();

        if (existeDescontoDuplicado(apoliceSeguro, novaApolice)) {
            exibirMensagem("atencao", "Registro Duplicado", "Já existe um registro com os mesmo valores.");
            return;
        }

        apoliceSeguro.push(novaApolice);
        _gridApoliceSeguroDesconto.CarregarGrid(apoliceSeguro);
        fecharDescontoModal();
    } else {
        exibirMensagemCamposObrigatorio();
    }
}

function existeDescontoDuplicado(apolices, novaApolice) {
    return apolices.some(a =>        
        a.CodigoModeloVeicular == novaApolice.CodigoModeloVeicular &&
        a.CodigoFilial == novaApolice.CodigoFilial &&
        a.CodigoTipoOperacao == novaApolice.CodigoTipoOperacao
    );
}
