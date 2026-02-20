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
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoOperacao;
var _tipoOperacao;

var TipoOperacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarTipoOperacao, issue: 121, idBtnSearch: guid() });
}


//*******EVENTOS*******

function loadTipoOperacao() {

    _tipoOperacao = new TipoOperacao();
    KoBindings(_tipoOperacao, "knockoutTipoOperacao");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                excluirTipoOperacaoClick(_tipoOperacao.Tipo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }];

    _gridTipoOperacao = new BasicDataTable(_tipoOperacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_tipoOperacao.Tipo, null, _tabelaFrete.GrupoPessoas, null, _gridTipoOperacao, null, _tabelaFrete.ContratoFreteTransportador);
    _tipoOperacao.Tipo.basicTable = _gridTipoOperacao;

    recarregarGridTipoOperacao();
}

function recarregarGridTipoOperacao() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.TiposOperacoes.val())) {

        $.each(_tabelaFrete.TiposOperacoes.val(), function (i, tipoOperacao) {
            var tipoOperacaoGrid = new Object();

            tipoOperacaoGrid.Codigo = tipoOperacao.Tipo.Codigo;
            tipoOperacaoGrid.Descricao = tipoOperacao.Tipo.Descricao;

            data.push(tipoOperacaoGrid);
        });
    }

    _gridTipoOperacao.CarregarGrid(data);
}


function excluirTipoOperacaoClick(knoutTipoOperacao, data) {
    var tipoOperacaoGrid = knoutTipoOperacao.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoOperacaoGrid.length; i++) {
        if (data.Codigo == tipoOperacaoGrid[i].Codigo) {
            tipoOperacaoGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoOperacao.basicTable.CarregarGrid(tipoOperacaoGrid);
}

function limparCamposTipoOperacao() {
    LimparCampos(_tipoOperacao);
}