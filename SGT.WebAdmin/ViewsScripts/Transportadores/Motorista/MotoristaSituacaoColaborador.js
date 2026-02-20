/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Motorista.js" />

var _gridSituacaoColaborador;

//*******MAPEAMENTO KNOUCKOUT*******

//*******EVENTOS*******

function loadMotoristaSituacaoColaborador() {
    
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "DataInicial", title: Localization.Resources.Transportadores.Motorista.DataInicial, width: "15%", className: "text-align-left" },
        { data: "DataFinal", title: Localization.Resources.Transportadores.Motorista.DataFinal, width: "15%", className: "text-align-center" },
        { data: "Situacao", title: Localization.Resources.Gerais.Geral.Situacao, width: "15%", className: "text-align-center" },
    ];

    _gridSituacaoColaborador = new BasicDataTable(_motorista.GridSituacoesColaborador.idGrid, header, null);
    recarregarGridSituacaoColaborador();
}

//*******MÉTODOS*******

function recarregarGridSituacaoColaborador() {
    var data = new Array();
    $.each(_motorista.GridSituacoesColaborador.list, function (i, motorista) {
        var motoristaSituacaoColaborador = new Object();

        motoristaSituacaoColaborador.Codigo = motorista.Codigo.val;
        motoristaSituacaoColaborador.Descricao = motorista.Descricao.val;
        motoristaSituacaoColaborador.DataInicial = motorista.DataInicial.val;
        motoristaSituacaoColaborador.DataFinal = motorista.DataFinal.val;
        motoristaSituacaoColaborador.Situacao = motorista.Situacao.val;

        data.push(motoristaSituacaoColaborador);
    });
    _gridSituacaoColaborador.CarregarGrid(data);
}