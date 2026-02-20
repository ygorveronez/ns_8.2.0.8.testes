/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="CentroCarregamento.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />

var _importacaoDisponibilidade;
var _diaParaImportarDisponibilidade = 0;

//*******MAPEAMENTO KNOUCKOUT*******

var ImportacaoDisponibilidadeModel = function () {
    this.DiaSemana = PropertyEntity({ type: types.map, val: ko.observable(EnumDiaSemana.Segunda), options: ko.observable(EnumDiaSemana.obterOpcoes()), text: Localization.Resources.Logistica.CentroCarregamento.Dia.getRequiredFieldDescription(), def: EnumDiaSemana.Segunda, required: true });

    this.Importar = PropertyEntity({ eventClick: ImportarDisponibilidadeDoDia, type: types.event, text: Localization.Resources.Gerais.Geral.Importar, visible: ko.observable(true), enable: ko.observable(true) });
}

function LoadImportacaoDisponibilidade() {
    _importacaoDisponibilidade = new ImportacaoDisponibilidadeModel();
    KoBindings(_importacaoDisponibilidade, "divModalImportacaoDisponibilidade");
}

//**********EVENTOS**********

function ImportarDisponibilidadeDoDia() {
    var diaImportarDe = _importacaoDisponibilidade.DiaSemana.val();

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.CentroCarregamento.DesejaRealmenteImportaOsDadosDeParaOsDadosExistentesDeSeraoApagados.format(EnumDiaSemana.obterDescricaoResumida(diaImportarDe), EnumDiaSemana.obterDescricaoResumida(_diaParaImportarDisponibilidade), EnumDiaSemana.obterDescricaoResumida(_diaParaImportarDisponibilidade)), function () {

        for (var i = _centroCarregamento.DisponibilidadesFrota.list.length - 1; i >= 0 ; i--)
            if (_diaParaImportarDisponibilidade == _centroCarregamento.DisponibilidadesFrota.list[i].DiaSemana.val)
                _centroCarregamento.DisponibilidadesCarregamento.list.splice(i, 1);

        var diasParaImportar = new Array();

        for (var i = 0; i < _centroCarregamento.DisponibilidadesFrota.list.length; i++) {
            if (diaImportarDe == _centroCarregamento.DisponibilidadesFrota.list[i].DiaSemana.val) {
                var diaImportar = $.extend(true, {}, _centroCarregamento.DisponibilidadesFrota.list[i]);
                diaImportar.DiaSemana.val = _diaParaImportarDisponibilidade;
                diaImportar.Codigo.val = guid();
                diasParaImportar.push(diaImportar);
            }
        }

        _centroCarregamento.DisponibilidadesFrota.list = _centroCarregamento.DisponibilidadesFrota.list.concat(diasParaImportar);

        _listaKnockoutDisponibilidadeFrota[_diaParaImportarDisponibilidade].RecarregarGrid();

        Global.fecharModal("divModalImportacaoDisponibilidade");

        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
    });
}