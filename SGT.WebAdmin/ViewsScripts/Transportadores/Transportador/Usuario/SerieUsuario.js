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
/// <reference path="../../Enumeradores/EnumTipoSerie.js" />
/// <reference path="Transportador.js" />
/// <reference path="../../../Consultas/SerieTransportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _statusSerieUsuario = [{ text: Localization.Resources.Gerais.Geral.Ativo, value: "A" },
    { text: Localization.Resources.Gerais.Geral.Inativo, value: "I" }];

var _tipoSerieUsuario = [{ text: "CT-e", value: EnumTipoSerie.CTe },
                  { text: "MDF-e", value: EnumTipoSerie.MDFe }]

var _gridSerieUsuario;
var _serieUsuario;
var _buscaSeries;

var SerieUsuario = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Serie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Transportadores.Transportador.Serie.getRequiredFieldDescription(), idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarSerieUsuarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    //this.Atualizar = PropertyEntity({ eventClick: atualizarSerieUsuarioClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    //this.Excluir = PropertyEntity({ eventClick: excluirSerieUsuarioClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    //this.Cancelar = PropertyEntity({ eventClick: cancelarSerieUsuarioClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var SerieMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.Numero = PropertyEntity({ val: 0, def: 0 });
    this.Tipo = PropertyEntity({ val: 0, def: 0 });
};

//*******EVENTOS*******

function loadSerieUsuario() {
    _serieUsuario = new SerieUsuario();
    KoBindings(_serieUsuario, "knockoutSerieUsuario");

    _buscaSeries = new BuscarSeriesTransportador(_transportador.Codigo.val(), _serieUsuario.Serie);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Exckuir, id: guid(), metodo: excluirSerieUsuarioClick }] };

    var header = [{ data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "Numero", title: Localization.Resources.Transportadores.Transportador.Numero, width: "20%" },
        { data: "DescricaoTipo", title: Localization.Resources.Transportadores.Transportador.Tipo, width: "65%" }];

    _gridSerieUsuario = new BasicDataTable(_serieUsuario.Grid.id, header, menuOpcoes, { column: 3, dir: orderDir.asc });

    recarregarGridSerieUsuario();

}

function DescricaoTipoSerieUsuario(tipo) {
    for (var i = 0; i < _tipoSerieUsuario.length; i++) {
        if (_tipoSerieUsuario[i].value == tipo)
            return _tipoSerieUsuario[i].text;
    }
}

function recarregarGridSerieUsuario() {
    var data = new Array();

    $.each(_usuario.Series.list, function (i, serie) {
        var serieGrid = new Object();
        serieGrid.Codigo = serie.Codigo.val;
        serieGrid.Numero = serie.Numero.val;
        serieGrid.DescricaoTipo = DescricaoTipoSerieUsuario(serie.Tipo.val);
        serieGrid.Tipo = serie.Tipo.val;
        data.push(serieGrid);
    });

    _gridSerieUsuario.CarregarGrid(data);
}

function adicionarSerieUsuarioClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_serieUsuario);

    if (tudoCerto) {
        var existe = false;

        $.each(_usuario.Series.list, function (i, serie) {
            if (serie.Codigo.val == _serieUsuario.Serie.codEntity()) {
                existe = true;
                return false;
            }
        });

        if (!existe) {
            executarReST("Transportador/BuscarSeriePorCodigo", { Codigo: _serieUsuario.Serie.codEntity() }, function (r) {
                if (r.Success) {
                    var novaSerie = new SerieMap();

                    novaSerie.Codigo.val = r.Data.Codigo;
                    novaSerie.Numero.val = r.Data.Numero;
                    novaSerie.Tipo.val = r.Data.Tipo;

                    _usuario.Series.list.push(novaSerie);

                    recarregarGridSerieUsuario();

                    $("#" + _serieUsuario.Serie.id).focus();
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao, r.Error);
                }
            });
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.SerieExistente, Localization.Resources.Transportadores.Transportador.SerieEstaCadastrada.format(_serieUsuario.Serie.val()));
        }

        limparCamposSerieUsuario();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorio, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }

}

function excluirSerieUsuarioClick(data) {
    $.each(_usuario.Series.list, function (i, serie) {
        if (serie.Codigo.val == data.Codigo) {
            _usuario.Series.list.splice(i, 1);
            recarregarGridSerieUsuario();
            return false;
        }
    });
}

function limparCamposSerieUsuario() {

    _serieUsuario.Adicionar.visible(true);

    LimparCampos(_serieUsuario);
}