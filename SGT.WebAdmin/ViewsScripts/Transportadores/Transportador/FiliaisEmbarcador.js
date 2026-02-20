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
/// <reference path="../../Consultas/FilialEmbarcador.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFilialEmbarcador;
var _filialEmbarcador;

var FilialEmbarcador = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.FilialHabilitadaEmitir, idBtnSearch: guid(), required: true });
    this.Localidade = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Localidade.getFieldDescription() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarFilialEmbarcadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadFilialEmbarcador() {

    _filialEmbarcador = new FilialEmbarcador();
    KoBindings(_filialEmbarcador, "knockoutFilialEmbarcador");

    new BuscarFilial(_filialEmbarcador.Filial, callbackFilial);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirFilialEmbarcadorClick }] };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Transportadores.Transportador.RazaoSocial, width: "50%" },
        { data: "Localidade", title: Localization.Resources.Transportadores.Transportador.Localidade, width: "30%" }
    ];

    _gridFilialEmbarcador = new BasicDataTable(_filialEmbarcador.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridFilialEmbarcador();
}

function callbackFilial(dataRow) {
    _filialEmbarcador.Filial.val(dataRow.Descricao);
    _filialEmbarcador.Filial.codEntity(dataRow.Codigo);
    _filialEmbarcador.Localidade.val(dataRow.Localidade);
}

function recarregarGridFilialEmbarcador() {

    var data = new Array();

    $.each(_transportador.FiliaisEmbarcador.list, function (i, filialEmbarcador) {
        var filialEmbarcadorGrid = new Object();

        filialEmbarcadorGrid.Codigo = filialEmbarcador.Filial.codEntity;
        filialEmbarcadorGrid.Descricao = filialEmbarcador.Filial.val;
        filialEmbarcadorGrid.Localidade = filialEmbarcador.Localidade.val;

        data.push(filialEmbarcadorGrid);
    });

    _gridFilialEmbarcador.CarregarGrid(data);
}


function excluirFilialEmbarcadorClick(data) {
    for (var i = 0; i < _transportador.FiliaisEmbarcador.list.length; i++) {
        filialEmbarcadorExcluir = _transportador.FiliaisEmbarcador.list[i];
        if (data.Codigo == filialEmbarcadorExcluir.Filial.codEntity)
            _transportador.FiliaisEmbarcador.list.splice(i, 1);
    }
    recarregarGridFilialEmbarcador();
}

function adicionarFilialEmbarcadorClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_filialEmbarcador);

    if (valido) {
        var existe = false;
        $.each(_transportador.FiliaisEmbarcador.list, function (i, filialEmbarcador) {
            if (filialEmbarcador.Filial.codEntity == _filialEmbarcador.Filial.codEntity()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.FilialExistente, Localization.Resources.Transportadores.Transportador.FilialCadastradaComoFilialHabilitada.format(_filialEmbarcador.Filial.val()));
            return;
        }

        _transportador.FiliaisEmbarcador.list.push(SalvarListEntity(_filialEmbarcador));

        recarregarGridFilialEmbarcador();

        $("#" + _filialEmbarcador.Filial.id).focus();

        limparCamposFilialEmbarcador();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
    }
}

function limparCamposFilialEmbarcador() {
    LimparCampos(_filialEmbarcador);
}