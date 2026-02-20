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

var _importacaoPeriodo;
var _diaParaImportarPeriodo = 0;
var _diaImportar = 0;
var _mesImportar = 0;

//*******MAPEAMENTO KNOUCKOUT*******

var _meses = [
    { text: Localization.Resources.Gerais.Geral.Janeiro, value: 1 },
    { text: Localization.Resources.Gerais.Geral.Fevereiro, value: 2 },
    { text: Localization.Resources.Gerais.Geral.Marco, value: 3 },
    { text: Localization.Resources.Gerais.Geral.Abril, value: 4 },
    { text: Localization.Resources.Gerais.Geral.Maio, value: 5 },
    { text: Localization.Resources.Gerais.Geral.Junho, value: 6 },
    { text: Localization.Resources.Gerais.Geral.Julho, value: 7 },
    { text: Localization.Resources.Gerais.Geral.Agosto, value: 8 },
    { text: Localization.Resources.Gerais.Geral.Setembro, value: 9 },
    { text: Localization.Resources.Gerais.Geral.Outubro, value: 10 },
    { text: Localization.Resources.Gerais.Geral.Novembro, value: 11 },
    { text: Localization.Resources.Gerais.Geral.Dezembro, value: 12 }
];

var ImportacaoPeriodoModel = function () {
    this.DiaSemana = PropertyEntity({ type: types.map, val: ko.observable(EnumDiaSemana.Segunda), options: ko.observable(EnumDiaSemana.obterOpcoes()), text: Localization.Resources.Gerais.Geral.Dia.getRequiredFieldDescription(), def: EnumDiaSemana.Segunda, required: true });

    this.Importar = PropertyEntity({ eventClick: ImportarDoDia, type: types.event, text: Localization.Resources.Gerais.Geral.Importar, visible: ko.observable(true), enable: ko.observable(true) });
}

var ImportacaoPeriodoModelDiaMes = function () {
    this.Mes = PropertyEntity({ type: types.map, val: ko.observable(new Date().getMonth() + 1), options: ko.observable(_meses), text: Localization.Resources.Gerais.Geral.Mes.getRequiredFieldDescription(), def: new Date().getMonth() + 1, required: true });
    this.Dia = PropertyEntity({ type: types.map, val: ko.observable(new Date().getDate()), options: ko.observableArray(initDiasNoMesObj()), text: Localization.Resources.Gerais.Geral.Dia.getRequiredFieldDescription(), def: new Date().getDate(), required: true });

    this.Mes.val.subscribe(function (novoValor) {
        _importacaoPeriodoDiaMes.Dia.val(1);
        var data = new Date(new Date().getFullYear(), novoValor, 0);
        _importacaoPeriodoDiaMes.Dia.options(ObterDiasNoMesObj(data));
    });

    this.Importar = PropertyEntity({ eventClick: ImportarDoDiaMes, type: types.event, text: Localization.Resources.Gerais.Geral.Importar, visible: ko.observable(true), enable: ko.observable(true) });
}

function LoadImportacaoPeriodo() {
    _importacaoPeriodo = new ImportacaoPeriodoModel();
    KoBindings(_importacaoPeriodo, "divModalImportacaoPeriodo");
    _importacaoPeriodoDiaMes = new ImportacaoPeriodoModelDiaMes();
    KoBindings(_importacaoPeriodoDiaMes, "divModalImportacaoPeriodoDiaMes");
}

//**********EVENTOS**********

function ImportarDoDia() {
    var diaImportarDe = _importacaoPeriodo.DiaSemana.val();

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.CentroDescarregamento.DesejaRealmenteImportarDadosDe + EnumDiaSemana.obterDescricaoResumida(diaImportarDe) + Localization.Resources.Logistica.CentroDescarregamento.Para + EnumDiaSemana.obterDescricaoResumida(_diaParaImportarPeriodo) + Localization.Resources.Logistica.CentroDescarregamento.OsDadosExistentesDe + EnumDiaSemana.obterDescricaoResumida(_diaParaImportarPeriodo) + Localization.Resources.Logistica.CentroDescarregamento.SeraoApagados, function () {
        for (var i = _centroDescarregamento.PeriodosDescarregamento.list.length - 1; i >= 0; i--)
            if (_diaParaImportarPeriodo == _centroDescarregamento.PeriodosDescarregamento.list[i].DiaSemana.val)
                _centroDescarregamento.PeriodosDescarregamento.list.splice(i, 1);

        var diasParaImportar = new Array();

        for (var i = 0; i < _centroDescarregamento.PeriodosDescarregamento.list.length; i++) {
            if (diaImportarDe == _centroDescarregamento.PeriodosDescarregamento.list[i].DiaSemana.val) {
                var diaImportar = $.extend(true, {}, _centroDescarregamento.PeriodosDescarregamento.list[i]);
                diaImportar.DiaSemana.val = _diaParaImportarPeriodo;
                diaImportar.Codigo.val = guid();
                diasParaImportar.push(diaImportar);
            }
        }

        _centroDescarregamento.PeriodosDescarregamento.list = _centroDescarregamento.PeriodosDescarregamento.list.concat(diasParaImportar);

        _listaKnockoutPeriodosDescarregamento[_diaParaImportarPeriodo].RecarregarGrid();

        Global.fecharModal("divModalImportacaoPeriodo");

        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
    });
}

function ImportarDoDiaMes() {
    var diaAtual = _centroDescarregamentoDiasNoMes.Dia.val();
    var mesAtual = _centroDescarregamentoDiasNoMes.Mes.val();
    var dataBusca = { Codigo: _centroDescarregamento.Codigo.val(), Dia: _importacaoPeriodoDiaMes.Dia.val(), Mes: _importacaoPeriodoDiaMes.Mes.val() }

    executarReST("CentroDescarregamento/BuscarCapacidadePorDiaMes", dataBusca, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_centroDescarregamento, arg);

                var dataSalvar = RetornarObjetoPesquisa(_centroDescarregamento);
                dataSalvar.CodigoCentroDescarregamento = _centroDescarregamento.Codigo.val();
                dataSalvar.Mes = mesAtual;
                dataSalvar.Dia = diaAtual;
                dataSalvar.CopiarPeriodo = true;

                executarReST("CentroDescarregamento/SalvarCapacidadeCarregamento", dataSalvar, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            recarregarGridsCapacidadeDescarregamento();
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                            Global.fecharModal("divModalImportacaoPeriodoDiaMes");
                        }
                        else
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                });
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}