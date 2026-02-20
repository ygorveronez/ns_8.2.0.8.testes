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
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoRotaFrete.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _rotaFreteValePedagio;
var _gridRotaFreteValePedagio;

var RotaFreteValePedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.RotaFrete.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoRotaFrete = PropertyEntity({ val: ko.observable(EnumTipoRotaFrete.Ida), options: EnumTipoRotaFrete.ObterOpcoes(), def: EnumTipoRotaFrete.Ida, text: Localization.Resources.Transportadores.Transportador.TipoRota.getRequiredFieldDescription(), required: true, visible: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
    this.Adicionar = PropertyEntity({ eventClick: adicionarRotaFreteValePedagioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadRotaFreteValePedagio() {
    _rotaFreteValePedagio = new RotaFreteValePedagio();
    KoBindings(_rotaFreteValePedagio, "tabRotasValePedagio");

    new BuscarRotasFrete(_rotaFreteValePedagio.RotaFrete);

    loadGridRotaFreteValePedagio();
}

function loadGridRotaFreteValePedagio() {

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirRotaFreteValePedagioClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "RotaFrete", title: Localization.Resources.Transportadores.Transportador.RotaFrete, width: "60%" },
        { data: "TipoRotaFrete", title: Localization.Resources.Transportadores.Transportador.TipoRota, width: "30%" }
    ];

    _gridRotaFreteValePedagio = new BasicDataTable(_rotaFreteValePedagio.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridRotaFreteValePedagio();
}


function recarregarGridRotaFreteValePedagio() {
    var data = new Array();

    $.each(_transportador.RotasFreteValePedagio.list, function (i, rota) {
        var rotaGrid = new Object();

        rotaGrid.Codigo = rota.Codigo;
        rotaGrid.RotaFrete = rota.RotaFrete.val;
        rotaGrid.TipoRotaFrete = EnumTipoRotaFrete.ObterDescricao(rota.TipoRotaFrete.val);

        data.push(rotaGrid);
    });

    _gridRotaFreteValePedagio.CarregarGrid(data);
}


function excluirRotaFreteValePedagioClick(data) {
    for (var i = 0; i < _transportador.RotasFreteValePedagio.list.length; i++) {
        rotaExcluir = _transportador.RotasFreteValePedagio.list[i];
        if (data.Codigo == rotaExcluir.Codigo)
            _transportador.RotasFreteValePedagio.list.splice(i, 1);
    }

    recarregarGridRotaFreteValePedagio();
}

function adicionarRotaFreteValePedagioClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_rotaFreteValePedagio);
    if (!valido)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorio, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);

    var existe = false;
    $.each(_transportador.RotasFreteValePedagio.list, function (i, rota) {
        if (rota.RotaFrete.codEntity == _rotaFreteValePedagio.RotaFrete.codEntity()) {
            existe = true;
            return;
        }
    });

    if (existe)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.RotaFreteexistente, Localization.Resources.Transportadores.Transportador.RotaFreteAdicionadaLista.format(_rotaFreteValePedagio.RotaFrete.val()));

    _rotaFreteValePedagio.Codigo.val(guid());
    _transportador.RotasFreteValePedagio.list.push(SalvarListEntity(_rotaFreteValePedagio));

    $("#" + _rotaFreteValePedagio.RotaFrete.id).focus();

    limparCamposRotaFreteValePedagio();
}

function limparCamposRotaFreteValePedagio() {
    LimparCampos(_rotaFreteValePedagio);
    recarregarGridRotaFreteValePedagio();
}