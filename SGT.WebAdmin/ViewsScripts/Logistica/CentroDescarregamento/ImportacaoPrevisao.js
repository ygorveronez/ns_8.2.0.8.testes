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
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="CentroDescarregamento.js" />
/// <reference path="PeriodoDescarregamento.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />

var _importacaoPrevisao;
var _diaParaImportarPrevisao = 0;

//*******MAPEAMENTO KNOUCKOUT*******

var ImportacaoPrevisaoModel = function () {
    this.DiaSemana = PropertyEntity({ type: types.map, val: ko.observable(EnumDiaSemana.Segunda), options: ko.observable(EnumDiaSemana.obterOpcoes()), text: Localization.Resources.Gerais.Geral.Dia.getRequiredFieldDescription(), def: EnumDiaSemana.Segunda, required: true });

    this.Importar = PropertyEntity({ eventClick: ImportarPrevisaoDoDia, type: types.event, text: Localization.Resources.Gerais.Geral.Importar, visible: ko.observable(true), enable: ko.observable(true) });
}

function LoadImportacaoPrevisao() {
    _importacaoPrevisao = new ImportacaoPrevisaoModel();
    KoBindings(_importacaoPrevisao, "divModalImportacaoPrevisao");
}

//**********EVENTOS**********

function ImportarPrevisaoDoDia() {
    var diaImportarDe = _importacaoPrevisao.DiaSemana.val();

    exibirConfirmacao(Localization.Resources.Gerais.Geral.AtencaoAcentuado, Localization.Resources.Logistica.CentroDescarregamento.DesejaRealmenteImportarDadosDe + EnumDiaSemana.obterDescricaoResumida(diaImportarDe) + Localization.Resources.Logistica.CentroDescarregamento.Para + EnumDiaSemana.obterDescricaoResumida(_diaParaImportarPrevisao) + Localization.Resources.Logistica.CentroDescarregamento.OsDadosExistentesDe + EnumDiaSemana.obterDescricaoResumida(_diaParaImportarPrevisao) + Localization.Resources.Logistica.CentroDescarregamento.SeraoApagados, function () {
        for (var i = _centroDescarregamento.PrevisoesDescarregamento.list.length - 1; i >= 0; i--)
            if (_diaParaImportarPrevisao == _centroDescarregamento.PrevisoesDescarregamento.list[i].DiaSemana.val)
                _centroDescarregamento.PrevisoesDescarregamento.list.splice(i, 1);

        var diasParaImportar = new Array();

        for (var i = 0; i < _centroDescarregamento.PrevisoesDescarregamento.list.length; i++) {
            if (diaImportarDe == _centroDescarregamento.PrevisoesDescarregamento.list[i].DiaSemana.val) {
                var diaImportar = $.extend(true, {}, _centroDescarregamento.PrevisoesDescarregamento.list[i]);
                diaImportar.DiaSemana.val = _diaParaImportarPrevisao;
                diaImportar.Codigo.val = guid();
                diasParaImportar.push(diaImportar);
            }
        }

        _centroDescarregamento.PrevisoesDescarregamento.list = _centroDescarregamento.PrevisoesDescarregamento.list.concat(diasParaImportar);

        _listaKnockoutPrevisoesDescarregamento[_diaParaImportarPrevisao].RecarregarGrid();

        Global.fecharModal("divModalImportacaoPrevisao");

        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
    });
}