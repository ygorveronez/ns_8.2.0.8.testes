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
/// <reference path="BonificacaoTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoOcorrencia;
var _tipoOcorrenciaBonificacaoTransporte;

var TipoOcorrencia = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: "Adicionar Tipos de Ocorrência", idBtnSearch: guid() });
}

//*******EVENTOS*******

function loadTipoOcorrencia() {

    _tipoOcorrenciaBonificacaoTransporte = new TipoOcorrencia();
    KoBindings(_tipoOcorrenciaBonificacaoTransporte, "knockoutTipoOcorrenciaBonificacaoTransportador");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirTipoOcorrenciaClick(_tipoOcorrenciaBonificacaoTransporte.Tipo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridTipoOcorrencia = new BasicDataTable(_tipoOcorrenciaBonificacaoTransporte.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTipoOcorrencia(_tipoOcorrenciaBonificacaoTransporte.Tipo, null, null, null, null, null, _gridTipoOcorrencia);
    _tipoOcorrenciaBonificacaoTransporte.Tipo.basicTable = _gridTipoOcorrencia;

    recarregarGridTipoOcorrencia();
}

function recarregarGridTipoOcorrencia() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_bonificacaoTransportador.TiposDeOcorrencia.val())) {
        $.each(_bonificacaoTransportador.TiposDeOcorrencia.val(), function (i, tipoOcorrencia) {
            var tipoOcorrenciaGrid = new Object();

            tipoOcorrenciaGrid.Codigo = tipoOcorrencia.Codigo;
            tipoOcorrenciaGrid.Descricao = tipoOcorrencia.Descricao;

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
    LimparCampos(_tipoOcorrenciaBonificacaoTransporte);
}