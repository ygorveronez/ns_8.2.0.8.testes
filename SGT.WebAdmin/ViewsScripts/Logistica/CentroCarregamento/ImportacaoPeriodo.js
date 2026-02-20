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
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="CentroCarregamento.js" />
/// <reference path="PeriodoCarregamento.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />

var _importacaoPeriodo;
var _diaParaImportarPeriodo = 0;

//*******MAPEAMENTO KNOUCKOUT*******

var ImportacaoPeriodoModel = function () {
    this.DiaSemana = PropertyEntity({ type: types.map, val: ko.observable(EnumDiaSemana.Segunda), options: ko.observable(EnumDiaSemana.obterOpcoes()), text: Localization.Resources.Logistica.CentroCarregamento.Dia.getRequiredFieldDescription(), def: EnumDiaSemana.Segunda, required: true });

    this.Importar = PropertyEntity({ eventClick: ImportarDoDia, type: types.event, text: Localization.Resources.Gerais.Geral.Importar, visible: ko.observable(true), enable: ko.observable(true) });
}

function LoadImportacaoPeriodo() {
    _importacaoPeriodo = new ImportacaoPeriodoModel();
    KoBindings(_importacaoPeriodo, "divModalImportacaoPeriodo");
}

//**********EVENTOS**********

function ImportarDoDia() {
    var diaImportarDe = _importacaoPeriodo.DiaSemana.val();

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.CentroCarregamento.DesejaRealmenteImportarOsDadosDeParaOsDadosExistentesDeSeraoApagados.format(EnumDiaSemana.obterDescricaoResumida(diaImportarDe), EnumDiaSemana.obterDescricaoResumida(_diaParaImportarPeriodo), EnumDiaSemana.obterDescricaoResumida(_diaParaImportarPeriodo)), function () {
        for (var i = _centroCarregamento.PeriodosCarregamento.list.length - 1; i >= 0 ; i--)
            if (_diaParaImportarPeriodo == _centroCarregamento.PeriodosCarregamento.list[i].DiaSemana.val)
                _centroCarregamento.PeriodosCarregamento.list.splice(i, 1);

        var diasParaImportar = new Array();

        for (var i = 0; i < _centroCarregamento.PeriodosCarregamento.list.length; i++) {
            if (diaImportarDe == _centroCarregamento.PeriodosCarregamento.list[i].DiaSemana.val) {
                var diaImportar = $.extend(true, {}, _centroCarregamento.PeriodosCarregamento.list[i]);
                diaImportar.DiaSemana.val = _diaParaImportarPeriodo;
                diaImportar.Codigo.val = guid();
                diasParaImportar.push(diaImportar);
            }
        }

        _centroCarregamento.PeriodosCarregamento.list = _centroCarregamento.PeriodosCarregamento.list.concat(diasParaImportar);

        _listaKnockoutPeriodosCarregamento[_diaParaImportarPeriodo].RecarregarGrid();

        Global.fecharModal("divModalImportacaoPeriodo");

        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
    });
}