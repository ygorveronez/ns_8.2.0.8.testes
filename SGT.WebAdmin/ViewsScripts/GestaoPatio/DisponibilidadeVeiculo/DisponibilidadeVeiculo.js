/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumPesquisaDisponibilidadeVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _pesquisaDisponibilidadeVeiculo;
var _gridDisponibilidadeVeiculo;

var situacaoPesquisaDisponibilidadeVeiculo = [
    { text: "Todas", value: EnumPesquisaDisponibilidadeVeiculo.Todos },
    { text: "Disponível", value: EnumPesquisaDisponibilidadeVeiculo.Disponivel },
    { text: "Em Viagem", value: EnumPesquisaDisponibilidadeVeiculo.EmViagem },
];

var PesquisaDisponibilidadeVeiculo = function () {
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.ContratoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Contrato de Frete:", idBtnSearch: guid() });

    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumPesquisaDisponibilidadeVeiculo.Todos), options: situacaoPesquisaDisponibilidadeVeiculo });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarDisponibilidadeVeiculo();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadDisponibilidadeVeiculo() {
    _pesquisaDisponibilidadeVeiculo = new PesquisaDisponibilidadeVeiculo();
    KoBindings(_pesquisaDisponibilidadeVeiculo, "knockoutPesquisaDisponibilidadeVeiculo");

    // Busca componentes pesquisa
    new BuscarTransportadores(_pesquisaDisponibilidadeVeiculo.Transportador);
    new BuscarContratoFreteTransportador(_pesquisaDisponibilidadeVeiculo.ContratoFrete);
    
    // Busca 
    BuscarDisponibilidadeVeiculo();
}




//*******MÉTODOS*******
function BuscarDisponibilidadeVeiculo() {
    var menuOpcoes = null;
    
    var configExportacao = {
        url: "DisponibilidadeVeiculo/ExportarPesquisa",
        titulo: "Disponibilidade de Veículos"
    };
    
    _gridDisponibilidadeVeiculo = new GridViewExportacao(_pesquisaDisponibilidadeVeiculo.Pesquisar.idGrid, "DisponibilidadeVeiculo/Pesquisa", _pesquisaDisponibilidadeVeiculo, menuOpcoes, configExportacao, { column: 4, dir: orderDir.asc }, 25);
    _gridDisponibilidadeVeiculo.CarregarGrid();
}