/// <reference path="../../Consultas/TipoOcorrencia.js" />
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

var _gridTipoOcorrencia;
var _tipoOcorrencia;

var TipoOcorrencia = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarTipoOcorrencia, issue: 121, idBtnSearch: guid() });
}


//*******EVENTOS*******

function loadTipoOcorrencia() {

    _tipoOcorrencia = new TipoOcorrencia();
    KoBindings(_tipoOcorrencia, "knockoutTipoOcorrencia");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                excluirTipoOcorrenciaClick(_tipoOcorrencia.Tipo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }];

    _gridTipoOcorrencia = new BasicDataTable(_tipoOcorrencia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTipoOcorrencia(_tipoOcorrencia.Tipo, null, null, null, null, null, _gridTipoOcorrencia);
    _tipoOcorrencia.Tipo.basicTable = _gridTipoOcorrencia;

    recarregarGridTipoOcorrencia();
}

function recarregarGridTipoOcorrencia() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.TiposDeOcorrencia.val())) {

        $.each(_tabelaFrete.TiposDeOcorrencia.val(), function (i, tipoOcorrencia) {
            var tipoOcorrenciaGrid = new Object();

            tipoOcorrenciaGrid.Codigo = tipoOcorrencia.Tipo.Codigo;
            tipoOcorrenciaGrid.Descricao = tipoOcorrencia.Tipo.Descricao;

            data.push(tipoOcorrenciaGrid);
        });
    }

    _gridTipoOcorrencia.CarregarGrid(data);
}


function excluirTipoOcorrenciaClick(knoutTipoOcorrencia, data) {
    var tipoOcorrenciaGrid = knoutTipoOcorrencia.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoOcorrenciaGrid.length; i++) {
        if (data.Codigo == tipoOcorrenciaGrid[i].Codigo) {
            tipoOcorrenciaGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoOcorrencia.basicTable.CarregarGrid(tipoOcorrenciaGrid);
}

function limparCamposTipoOcorrencia() {
    LimparCampos(_tipoOcorrencia);
}