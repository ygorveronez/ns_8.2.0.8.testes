/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="ValorParametroOcorrencia.js" />
/// <reference path="DadosTipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _dados;

var _tipoPessoa = [
    { text: "Pessoa (CNPJ/CPF)", value: 1 },
    { text: "Grupo de Pessoas", value: 2 }
];

var Dados = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: _tipoPessoa, def: 1, text: "Tipo de Pessoa:", issue: 306, enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    this.VigenciaInicial = PropertyEntity({ text: "*Vigência Inicial: ", getType: typesKnockout.date, required: true });
    this.VigenciaFinal = PropertyEntity({ text: "*Vigência Final: ", getType: typesKnockout.date, required: true });
    this.VigenciaInicial.dateRangeLimit = this.VigenciaFinal;
    this.VigenciaFinal.dateRangeInit = this.VigenciaInicial;
    this.Observacao = PropertyEntity({ text: "Observação: " });

    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.HoraExtraVeiculos = PropertyEntity({ type: types.listEntity, getType: typesKnockout.listEntity, def: [], val: ko.observableArray([]), codEntity: ko.observable(0) });
    this.HoraExtraAjudantes = PropertyEntity({ type: types.listEntity, getType: typesKnockout.listEntity, def: [], val: ko.observableArray([]), codEntity: ko.observable(0) });

    this.EstadiaVeiculos = PropertyEntity({ type: types.listEntity, getType: typesKnockout.listEntity, def: [], val: ko.observableArray([]), codEntity: ko.observable(0) });
    this.EstadiaAjudantes = PropertyEntity({ type: types.listEntity, getType: typesKnockout.listEntity, def: [], val: ko.observableArray([]), codEntity: ko.observable(0) });

    this.PernoiteValores = PropertyEntity({ type: types.listEntity, getType: typesKnockout.listEntity, def: [], val: ko.observableArray([]), codEntity: ko.observable(0) });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor === 1) {
            _dados.Pessoa.visible(true);
            _dados.GrupoPessoa.visible(false);
            LimparCampoEntity(_dados.GrupoPessoa);
        } else {
            _dados.GrupoPessoa.visible(true);
            _dados.Pessoa.visible(false);
            LimparCampoEntity(_dados.Pessoa);
        }
    });
};

//*******EVENTOS*******
function LoadDados() {
    _dados = new Dados();
    KoBindings(_dados, "knockoutDados");

    new BuscarClientes(_dados.Pessoa);
    new BuscarGruposPessoas(_dados.GrupoPessoa);
}


//*******METODOS*******
function LimparCamposCRUDAba(ko) {
    ko.Adicionar.visible(true);
    ko.Atualizar.visible(false);
    ko.Excluir.visible(false);
    ko.Cancelar.visible(true);
}

function LimparCamposDados() {
    LimparCampos(_dados);
    limparCamposDadosTipoOperacao();
}

function ValidarDuplicidadeModeloVeicular(ko, data) {
    //var dados = ko.Data.Get();

    //for (var i = 0; i < dados.length; i++) {
    //    var item = dados[i];
    //    if (item.ModeloVeicular.codEntity == data.ModeloVeicular.codEntity && item.Codigo.val != data.Codigo.val)
    //        return false;
    //}

    return true;
}

function HandleRequiredTipoOcorrencia() {
    for (var i in arguments) {
        if (arguments[i].BuscarRegistros().length > 0)
            return true;
    }

    return false;
}

function HandleRequiredComponente(ko) {
    return function () {
        return ko.Data.Grid().BuscarRegistros().length > 0;
    };
}