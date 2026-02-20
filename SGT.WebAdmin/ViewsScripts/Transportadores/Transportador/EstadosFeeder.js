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
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEstadoFeeder;
var _estadoFeeder;

var EstadoFeeder = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.Estado.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    
    this.Adicionar = PropertyEntity({ eventClick: adicionarEstadoFeederClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}


//*******EVENTOS*******

function loadEstadoFeeder() {

    _estadoFeeder = new EstadoFeeder();
    KoBindings(_estadoFeeder, "knockoutEstadosFeeder");

    new BuscarEstados(_estadoFeeder.Estado);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirEstadoFeederTransportadorClick }] };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Transportadores.Transportador.Estado, width: "80%" }
    ];

    _gridEstadoFeeder = new BasicDataTable(_estadoFeeder.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridEstadoFeeder();
}

function recarregarGridEstadoFeeder() {

    var data = new Array();

    $.each(_transportador.EstadosFeeder.list, function (i, estadoFeeder) {
        var estadoFeederGrid = new Object();

        estadoFeederGrid.Codigo = estadoFeeder.Estado.codEntity;
        estadoFeederGrid.Descricao = estadoFeeder.Estado.val;

        data.push(estadoFeederGrid);
    });

    _gridEstadoFeeder.CarregarGrid(data);
}


function excluirEstadoFeederTransportadorClick(data) {
    for (var i = 0; i < _transportador.EstadosFeeder.list.length; i++) {
        estadoFeederExcluir = _transportador.EstadosFeeder.list[i];
        if (data.Codigo == estadoFeederExcluir.Estado.codEntity)
            _transportador.EstadosFeeder.list.splice(i, 1);
    }
    recarregarGridEstadoFeeder();
}

function adicionarEstadoFeederClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_estadoFeeder);

    if (valido) {
        var existe = false;
        $.each(_transportador.EstadosFeeder.list, function (i, estadoFeeder) {
            if (estadoFeeder.Estado.codEntity == _estadoFeeder.Estado.codEntity()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.EstadoExistente, Localization.Resources.Transportadores.Transportador.EstadoJaCadastrado.format(_estadoFeeder.Estado.val()));
            return;
        }

        _transportador.EstadosFeeder.list.push(SalvarListEntity(_estadoFeeder));

        recarregarGridEstadoFeeder();

        $("#" + _estadoFeeder.Estado.id).focus();

        limparCamposEstadoFeeder();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
    }
}

function limparCamposEstadoFeeder() {

    LimparCampos(_estadoFeeder);
}