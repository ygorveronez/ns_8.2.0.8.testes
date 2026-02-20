///// <reference path="../../../js/libs/jquery-2.1.1.js" />
///// <reference path="../../../js/Global/CRUD.js" />
///// <reference path="../../../js/Global/knockout-3.1.0.js" />
///// <reference path="../../../js/Global/Rest.js" />
///// <reference path="../../../js/Global/Mensagem.js" />
///// <reference path="../../../js/Global/Grid.js" />
///// <reference path="../../../js/bootstrap/bootstrap.js" />
///// <reference path="../../../js/libs/jquery.blockui.js" />
///// <reference path="../../../js/Global/knoutViewsSlides.js" />
///// <reference path="../../../js/libs/jquery.maskMoney.js" />
///// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
///// <reference path="../../Consultas/TipoOperacao.js" />
///// <reference path="OperadorFilial.js" />
///// <reference path="ConfigOperador.js" />
///// <reference path="OperadorTipoCarga.js" />
///// <reference path="OperadorTipoOperacao.js" />
///// <reference path="../../Consultas/TabelaFrete.js" />


////*******MAPEAMENTO KNOUCKOUT*******

//var _gridTabelaFrete;
//var _tabelaFrete;

//var TabelaFrete = function () {
//    this.Grid = PropertyEntity({ type: types.local });
//    this.Tabela = PropertyEntity({ type: types.event, text: "Adicionar Tabela", idBtnSearch: guid(), issue: 78 });
//}


////*******EVENTOS*******

//function loadTabelaFrete() {

//    _tabelaFrete = new TabelaFrete();
//    KoBindings(_tabelaFrete, "knockoutTabelaFrete");

//    var menuOpcoes = {
//        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
//            descricao: "Excluir", id: guid(), metodo: function (data) {
//                excluirTabelaFreteClick(_tabelaFrete.Tabela, data)
//            }
//        }]
//    };

//    var header = [{ data: "Codigo", visible: false },
//                  { data: "Descricao", title: "Descrição", width: "80%" }];

//    _gridTabelaFrete = new BasicDataTable(_tabelaFrete.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

//    new BuscarTabelasDeFrete(_tabelaFrete.Tabela, null, null, _gridTabelaFrete);
//    _tabelaFrete.Tabela.basicTable = _gridTabelaFrete;

//    recarregarGridTabelaFrete();
//}

//function recarregarGridTabelaFrete() {

//    var data = new Array();

//    $.each(_operador.TabelasFrete.val(), function (i, tabelaFrete) {
//        var tabelaFreteGrid = new Object();

//        tabelaFreteGrid.Codigo = tabelaFrete.Tabela.Codigo;
//        tabelaFreteGrid.Descricao = tabelaFrete.Tabela.Descricao;

//        data.push(tabelaFreteGrid);
//    });

//    _gridTabelaFrete.CarregarGrid(data);
//}


//function excluirTabelaFreteClick(knoutTabela, data) {
//    var tabelaGrid = knoutTabela.basicTable.BuscarRegistros();

//    for (var i = 0; i < tabelaGrid.length; i++) {
//        if (data.Codigo == tabelaGrid[i].Codigo) {
//            tabelaGrid.splice(i, 1);
//            break;
//        }
//    }
//    knoutTabela.basicTable.CarregarGrid(tabelaGrid);
//}

//function limparCamposTabelaFrete() {
//    LimparCampos(_tabelaFrete);
//}