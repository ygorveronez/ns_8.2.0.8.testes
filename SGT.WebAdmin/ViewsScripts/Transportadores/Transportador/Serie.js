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
/// <reference path="../../Enumeradores/EnumTipoSerie.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _statusSerie = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var _gridSerie;
var _serie;

var Serie = function () {

    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.Serie.getRequiredFieldDescription(), def: "", getType: typesKnockout.int, maxlength: 3, required: true });
    this.ProximoNumeroDocumento = PropertyEntity({ val: ko.observable("1"), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.ProximoNumeroDocumento.getRequiredFieldDescription(), def: "1", getType: typesKnockout.int, maxlength: 10, required: true, visible: ko.observable(false) });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoSerie.CTe), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.Tipo.getRequiredFieldDescription(), options: EnumTipoSerie.obterOpcoes(), def: EnumTipoSerie.CTe });
    this.Status = PropertyEntity({ val: ko.observable("A"), text: Localization.Resources.Transportadores.Transportador.Status.getRequiredFieldDescription(), options: _statusSerie, def: "A" });
    this.NaoGerarCargaAutomaticamente = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Transportadores.Transportador.NaoGerarCargaAutomaticamente, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarSerieClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarSerieClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarSerieClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.Tipo.val.subscribe(function (valor) {
        if (valor == EnumTipoSerie.MDFe && _CONFIGURACAO_TMS.GerarCargaDeMDFesNaoVinculadosACargas === true)
            _serie.NaoGerarCargaAutomaticamente.visible(true);
        else
            _serie.NaoGerarCargaAutomaticamente.visible(false);
    });
};

//*******EVENTOS*******

function loadSerie() {

    _serie = new Serie();
    _serie.Codigo.val(guid());

    KoBindings(_serie, "knockoutCadastroSerie");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarSerieClick }] };
    var header = null;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin) {
        _serie.Tipo.def = EnumTipoSerie.NFe;
        _serie.Tipo.val(EnumTipoSerie.NFe);
        _serie.ProximoNumeroDocumento.visible(true);

        header = [
            { data: "Codigo", visible: false },
            { data: "Status", visible: false },
            { data: "Tipo", visible: false },
            { data: "Numero", title: Localization.Resources.Transportadores.Transportador.Numero, width: "10%" },
            { data: "DescricaoTipo", title: Localization.Resources.Transportadores.Transportador.Tipo, width: "20%" },
            { data: "DescricaoStatus", title: Localization.Resources.Transportadores.Transportador.Status, width: "35%" },
            { data: "ProximoNumeroDocumento", title: Localization.Resources.Transportadores.Transportador.ProximoNumeroDocumento, width: "15%" }
        ];

    } else {

        header = [
            { data: "Codigo", visible: false },
            { data: "Status", visible: false },
            { data: "Tipo", visible: false },
            { data: "Numero", title: Localization.Resources.Transportadores.Transportador.Numero, width: "10%" },
            { data: "DescricaoTipo", title: Localization.Resources.Transportadores.Transportador.Tipo, width: "20%" },
            { data: "DescricaoStatus", title: Localization.Resources.Transportadores.Transportador.Status, width: "35%" },
            { data: "ProximoNumeroDocumento", visible: false }
        ];
    }

    _gridSerie = new BasicDataTable(_serie.Grid.id, header, menuOpcoes, { column: 3, dir: orderDir.asc });

    recarregarGridSerie();
}

function DescricaoStatus(status) {
    for (var i = 0; i < _statusSerie.length; i++) {
        if (_statusSerie[i].value == status)
            return _status[i].text;
    }
}

function recarregarGridSerie() {
    var data = new Array();

    $.each(_transportador.Series.list, function (i, serie) {
        var serieGrid = new Object();
        serieGrid.Codigo = serie.Codigo.val;
        serieGrid.Status = serie.Status.val;
        serieGrid.Numero = serie.Numero.val;
        serieGrid.ProximoNumeroDocumento = serie.ProximoNumeroDocumento.val;
        serieGrid.DescricaoTipo = EnumTipoSerie.obterDescricao(serie.Tipo.val);
        serieGrid.DescricaoStatus = DescricaoStatus(serie.Status.val);
        serieGrid.Tipo = serie.Tipo.val;
        serieGrid.NaoGerarCargaAutomaticamente = serie.NaoGerarCargaAutomaticamente.val;

        data.push(serieGrid);
    });

    _gridSerie.CarregarGrid(data);
}


function editarSerieClick(data) {
    _serie.Atualizar.visible(true);
    _serie.Cancelar.visible(true);
    _serie.Adicionar.visible(false);

    EditarListEntity(_serie, data);

    _serie.Numero.enable(false);
    _serie.Tipo.enable(false);
}


function adicionarSerieClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_serie);
    if (tudoCerto) {
        var existe = false;
        $.each(_transportador.Series.list, function (i, serie) {
            if (serie.Numero.val == _serie.Numero.val() && serie.Tipo.val == _serie.Tipo.val()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _transportador.Series.list.push(SalvarListEntity(_serie));
            recarregarGridSerie();
            $("#" + _serie.Numero.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.SerieExistente, Localization.Resources.Transportadores.Transportador.SerieEstaCadastrada.format(_serie.Numero.val()));
        }
        limparCamposSerie();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorio, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }

}

function atualizarSerieClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_serie);
    if (tudoCerto) {
        $.each(_transportador.Series.list, function (i, serie) {
            if (serie.Codigo.val == _serie.Codigo.val()) {
                AtualizarListEntity(_serie, serie);
                return false;
            }
        });
        recarregarGridSerie();
        limparCamposSerie();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorio, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}


function cancelarSerieClick(e) {
    limparCamposSerie();
}

function limparCamposSerie() {
    _serie.Codigo.val(guid());
    _serie.Atualizar.visible(false);
    _serie.Cancelar.visible(false);
    _serie.Adicionar.visible(true);
    LimparCampos(_serie);
    _serie.Numero.enable(true);
    _serie.Tipo.enable(true);
}