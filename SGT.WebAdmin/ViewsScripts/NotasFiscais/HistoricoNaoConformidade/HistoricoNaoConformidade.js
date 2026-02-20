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

var _historicoNaoConformidade;
var _gridNaoConformidade;

var HistoricoNaoConformidade = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NotaFiscal = PropertyEntity({ text: "*Nota Fiscal:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Pesquisar = PropertyEntity({ eventClick: buscarHistoricoNaoConformidade, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.ExibirGridPesquisa = PropertyEntity({ idFade: guid(), visibleFade: ko.observable(false) });
}

function loadHistoricoNaoConformidade() {
    _historicoNaoConformidade = new HistoricoNaoConformidade();
    KoBindings(_historicoNaoConformidade, "knockoutHistoricoNaoConformidade", false, _historicoNaoConformidade.Pesquisar.id);
}

function buscarHistoricoNaoConformidade() {
    if (!ValidarCamposObrigatorios(_historicoNaoConformidade)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar uma nota fiscal.");
        return;
    }

    if (_historicoNaoConformidade.Codigo.val() > 0)
        _gridNaoConformidade.CarregarGrid();

    _gridNaoConformidade = new GridView(_historicoNaoConformidade.Pesquisar.idGrid, "HistoricoNaoConformidade/Pesquisa", _historicoNaoConformidade, null, null, 28);
    _gridNaoConformidade.CarregarGrid(function (retornoGrid) {
        if (retornoGrid != null && retornoGrid.data.length > 0) {
            _historicoNaoConformidade.ExibirGridPesquisa.visibleFade(true);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", "Nota fiscal não foi encontrada.");
            _historicoNaoConformidade.ExibirGridPesquisa.visibleFade(false);
        }
    });
}

function limparCamposHistoricoNaoConformidade() {
    LimparCampos(_historicoNaoConformidade);
}