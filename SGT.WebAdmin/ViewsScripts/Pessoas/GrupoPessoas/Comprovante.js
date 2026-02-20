/// <reference path="../../Consultas/TipoOperacao.js" />
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

var _gridComprovante;
var _comprovante;

var Comprovante = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Comprovante = PropertyEntity({ type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.AdicionarComprovante, idBtnSearch: guid() });
    this.ExigirComprovantesLiberacaoPagamentoContratoFrete = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.Pessoas.GrupoPessoas.ExigirComprovantesLiberacaoPagamentoContratoFrete, visible: ko.observable(true), enable: ko.observable(true) });
}


//*******EVENTOS*******

function loadComprovante() {
    _comprovante = new Comprovante();
    KoBindings(_comprovante, "knockoutComprovante");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Pessoas.GrupoPessoas.Excluir, id: guid(), metodo: function (data) {
                excluirComprovanteClick(_comprovante.Comprovante, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Pessoas.GrupoPessoas.Descricao, width: "80%" }];

    _gridComprovante = new BasicDataTable(_comprovante.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTipoComprovante(_comprovante.Comprovante, null, _gridComprovante);
    _comprovante.Comprovante.basicTable = _gridComprovante;

    recarregarGridComprovante();

    _grupoPessoas.ExigirComprovantesLiberacaoPagamentoContratoFrete.val(_comprovante.ExigirComprovantesLiberacaoPagamentoContratoFrete.val());
}

function recarregarGridComprovante() {
    var data = new Array();
    
    if (!string.IsNullOrWhiteSpace(_grupoPessoas.Comprovantes.val())) {

        $.each(_grupoPessoas.Comprovantes.val(), function (i, comprovante) {
            var comprovanteGrid = new Object();

            comprovanteGrid.Codigo = comprovante.Codigo;
            comprovanteGrid.Descricao = comprovante.Descricao;

            data.push(comprovanteGrid);
        });
    }

    _gridComprovante.CarregarGrid(data);
}


function excluirComprovanteClick(knoutComprovante, data) {
    var comprovanteGrid = knoutComprovante.basicTable.BuscarRegistros();

    for (var i = 0; i < comprovanteGrid.length; i++) {
        if (data.Codigo == comprovanteGrid[i].Codigo) {
            comprovanteGrid.splice(i, 1);
            break;
        }
    }

    knoutComprovante.basicTable.CarregarGrid(comprovanteGrid);
}

function limparCamposComprovante() {
    LimparCampos(_comprovante);
    _gridComprovante.CarregarGrid(new Array);
    _comprovante.ExigirComprovantesLiberacaoPagamentoContratoFrete.val(false);
}

function obterListaTipoComprovanteSalvar() {

    let tiposComprovantes = new Array();

    $.each(_comprovante.Comprovante.basicTable.BuscarRegistros(), function (i, tipoComprovante) {
        tiposComprovantes.push({ Tipo: tipoComprovante });
    });

    return JSON.stringify(tiposComprovantes);
}

function atualizaCamposComprovante() {
    _grupoPessoas.ExigirComprovantesLiberacaoPagamentoContratoFrete.val(_comprovante.ExigirComprovantesLiberacaoPagamentoContratoFrete.val());
}
function preencherCamposComprovante() {
    _comprovante.ExigirComprovantesLiberacaoPagamentoContratoFrete.val(_grupoPessoas.ExigirComprovantesLiberacaoPagamentoContratoFrete.val());
}