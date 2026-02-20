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
/// <reference path="PeriodoCarregamento.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />

var _importacaoPrevisao;
var _diaParaImportarPrevisao = 0;

//*******MAPEAMENTO KNOUCKOUT*******

var ImportacaoPrevisaoModel = function () {
    this.DiaSemana = PropertyEntity({ type: types.map, val: ko.observable(EnumDiaSemana.Segunda), options: ko.observable(EnumDiaSemana.obterOpcoes()), text: Localization.Resources.Logistica.CentroCarregamento.Dia.getRequiredFieldDescription(), def: EnumDiaSemana.Segunda, required: true });

    this.Importar = PropertyEntity({ eventClick: ImportarPrevisaoDoDia, type: types.event, text: Localization.Resources.Gerais.Geral.Importar, visible: ko.observable(true), enable: ko.observable(true) });
}

function LoadImportacaoPrevisao() {
    _importacaoPrevisao = new ImportacaoPrevisaoModel();
    KoBindings(_importacaoPrevisao, "divModalImportacaoPrevisao");
}

//**********EVENTOS**********

function ImportarPrevisaoDoDia() {
    var diaImportarDe = _importacaoPrevisao.DiaSemana.val();

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.CentroCarregamento.DesejaRealmenteImportarOsDadosDeParaOsDadosExistentesDeSeraoApagados.format(EnumDiaSemana.obterDescricaoResumida(diaImportarDe), EnumDiaSemana.obterDescricaoResumida(_diaParaImportarPrevisao), EnumDiaSemana.obterDescricaoResumida(_diaParaImportarPrevisao)), function () {
        for (var i = _centroCarregamento.PrevisoesCarregamento.list.length - 1; i >= 0 ; i--)
            if (_diaParaImportarPrevisao == _centroCarregamento.PrevisoesCarregamento.list[i].DiaSemana.val)
                _centroCarregamento.PrevisoesCarregamento.list.splice(i, 1);

        var diasParaImportar = new Array();

        for (var i = 0; i < _centroCarregamento.PrevisoesCarregamento.list.length; i++) {
            if (diaImportarDe == _centroCarregamento.PrevisoesCarregamento.list[i].DiaSemana.val) {
                var diaImportar = $.extend(true, {}, _centroCarregamento.PrevisoesCarregamento.list[i]);
                diaImportar.DiaSemana.val = _diaParaImportarPrevisao;
                diaImportar.Codigo.val = guid();
                diasParaImportar.push(diaImportar);
            }
        }

        _centroCarregamento.PrevisoesCarregamento.list = _centroCarregamento.PrevisoesCarregamento.list.concat(diasParaImportar);

        _listaKnockoutPrevisoesCarregamento[_diaParaImportarPrevisao].RecarregarGrid();

        Global.fecharModal("divModalImportacaoPrevisao");

        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
    });
}