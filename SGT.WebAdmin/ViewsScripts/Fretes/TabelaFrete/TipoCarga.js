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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoCarga;
var _tipoCarga;
var _tipoCargaGeral;

var TipoCarga = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarTipoDeCarga, idBtnSearch: guid(), issue: 53 });
};

var tipoCargaGeral = function () {
    this.NaoPermitirLancarValorPorTipoDeCarga = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NaoPermitirLancarValorPorTipoDeCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
};


//*******EVENTOS*******

function loadTipoCarga() {

    _tipoCarga = new TipoCarga();
    KoBindings(_tipoCarga, "knockoutTipoCarga");

    _tipoCargaGeral = new tipoCargaGeral();
    KoBindings(_tipoCargaGeral, "knockoutTipoCargaGeral");


    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                excluirTipoCargaClick(_tipoCarga.Tipo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }];

    _gridTipoCarga = new BasicDataTable(_tipoCarga.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_tipoCarga.Tipo, null, _tabelaFrete.GrupoPessoas, _gridTipoCarga);
    _tipoCarga.Tipo.basicTable = _gridTipoCarga;

    recarregarGridTipoCarga();
}

function recarregarGridTipoCarga() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.TiposCargas.val())) {

        $.each(_tabelaFrete.TiposCargas.val(), function (i, tipoCarga) {
            var tipoCargaGrid = new Object();

            tipoCargaGrid.Codigo = tipoCarga.Tipo.Codigo;
            tipoCargaGrid.Descricao = tipoCarga.Tipo.Descricao;

            data.push(tipoCargaGrid);
        });
    }

    _gridTipoCarga.CarregarGrid(data);
}


function excluirTipoCargaClick(knoutTipoCarga, data) {
    var tipoCargaGrid = knoutTipoCarga.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoCargaGrid.length; i++) {
        if (data.Codigo == tipoCargaGrid[i].Codigo) {
            tipoCargaGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoCarga.basicTable.CarregarGrid(tipoCargaGrid);
}

function limparCamposTipoCarga() {
    LimparCampos(_tipoCarga);
    LimparCampos(_tipoCargaGeral);
}