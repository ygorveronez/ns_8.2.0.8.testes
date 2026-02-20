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
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridIntelipostTipoOcorrencia;
var _IntelipostTipoOcorrencia;

var IntelipostTipoOcorrencia = function () {

    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Transportadores.Transportador.TipoOcorrencia.getRequiredFieldDescription(), issue: 121, visible: ko.observable(true), idBtnSearch: guid() });
    this.Grid = PropertyEntity({ type: types.local });
    this.CodigoIntegracao = PropertyEntity({ visible: ko.observable(true), required: false, text: Localization.Resources.Transportadores.Transportador.CodigoIntegracao.getRequiredFieldDescription(), val: ko.observable(""), required: true });
    this.MacroStatus = PropertyEntity({ visible: ko.observable(true), required: false, text: Localization.Resources.Transportadores.Transportador.MacroStatus.getRequiredFieldDescription(), val: ko.observable(""), required: true });
    this.MicroStatus = PropertyEntity({ visible: ko.observable(true), required: false, text: Localization.Resources.Transportadores.Transportador.MicroStatus.getRequiredFieldDescription(), val: ko.observable(""), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarIntelipostTipoOcorrenciaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadIntelipostTipoOcorrencia() {

    _IntelipostTipoOcorrencia = new IntelipostTipoOcorrencia();
    KoBindings(_IntelipostTipoOcorrencia, "knockoutIntelipostTipoOcorrencia");

    new BuscarTipoOcorrencia(_IntelipostTipoOcorrencia.TipoOcorrencia, null, null, null, null, null, null, null, true);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirIntelipostTipoOcorrenciaClick }] };

    var header = [
        { data: "CodigoTipoOcorrencia", visible: false },
        { data: "TipoOcorrencia", title: Localization.Resources.Transportadores.Transportador.TipoOperacao, width: "20%" },
        { data: "CodigoIntegracao", title: Localization.Resources.Transportadores.Transportador.CodigoIntegracao, width: "20%" },
        { data: "MicroStatus", title: Localization.Resources.Transportadores.Transportador.MicroStatus, width: "20%" },
        { data: "MacroStatus", title: Localization.Resources.Transportadores.Transportador.MacroStatus, width: "20%" },
    ];

    var configExportacao = {
        url: "Transportador/ExportarIntelipostTipoOcorrencia",
        btnText: "Exportar excel (Dados Salvos)",
        funcaoObterParametros: function () {
            return { Codigo: _transportador.Codigo.val() };
        }
    }

    _gridIntelipostTipoOcorrencia = new BasicDataTable(_IntelipostTipoOcorrencia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, null, null, null, null, null, null, null, configExportacao);

    recarregarGridIntelipostTipoOcorrencia();
}

function recarregarGridIntelipostTipoOcorrencia() {
    var data = new Array();

    $.each(_transportador.IntelipostTipoOcorrencia.list, function (i, IntelipostTipoOcorrencia) {
        var IntelipostTipoOcorrenciaGrid = new Object();

        IntelipostTipoOcorrenciaGrid.CodigoTipoOcorrencia = IntelipostTipoOcorrencia.TipoOcorrencia.codEntity;
        IntelipostTipoOcorrenciaGrid.TipoOcorrencia = IntelipostTipoOcorrencia.TipoOcorrencia.val;
        IntelipostTipoOcorrenciaGrid.CodigoIntegracao = IntelipostTipoOcorrencia.CodigoIntegracao.val;
        IntelipostTipoOcorrenciaGrid.MicroStatus = IntelipostTipoOcorrencia.MicroStatus.val;
        IntelipostTipoOcorrenciaGrid.MacroStatus = IntelipostTipoOcorrencia.MacroStatus.val;

        data.push(IntelipostTipoOcorrenciaGrid);
    });

    _gridIntelipostTipoOcorrencia.CarregarGrid(data);
}

function excluirIntelipostTipoOcorrenciaClick(data) {

    for (var i = 0; i < _transportador.IntelipostTipoOcorrencia.list.length; i++) {
        IntelipostTipoOcorrenciaExcluir = _transportador.IntelipostTipoOcorrencia.list[i];
        if (data.CodigoTipoOcorrencia == IntelipostTipoOcorrenciaExcluir.TipoOcorrencia.codEntity)
            _transportador.IntelipostTipoOcorrencia.list.splice(i, 1);
    }
    recarregarGridIntelipostTipoOcorrencia();
}

function adicionarIntelipostTipoOcorrenciaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_IntelipostTipoOcorrencia);

    if (valido) {
        /*var existe = false;
        $.each(_transportador.IntelipostTipoOcorrencia.list, function (i, IntelipostTipoOcorrencia) {

            if (IntelipostTipoOcorrencia.TipoOcorrencia.codEntity == _IntelipostTipoOcorrencia.TipoOcorrencia.codEntity()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, "O token para este tipo de operação já existe");
            return;
        }*/

        _transportador.IntelipostTipoOcorrencia.list.push(SalvarListEntity(_IntelipostTipoOcorrencia));

        recarregarGridIntelipostTipoOcorrencia();

        $("#" + _IntelipostTipoOcorrencia.TipoOcorrencia.id).focus();

        limparCamposIntelipostTipoOcorrencia();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
    }
}

function limparCamposIntelipostTipoOcorrencia() {
    LimparCampos(_IntelipostTipoOcorrencia);
}