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
/// <reference path="Chamado.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _chamadoData;
var codigoMotivoChamadoCarregouData = 0;

/*
 * Declaração das Classes
 */

var ChamadoData = function () {
    this.PossuiDatas = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TotalHoras = PropertyEntity({ type: types.map, text: "Total de Horas" });
    this.Datas = ko.observableArray();
};

var DataMotivoChamado = function (situacao, status) {
    this.Codigo = PropertyEntity({ val: ko.observable(situacao.Codigo), getType: typesKnockout.int, def: situacao.Codigo });
    this.CodigoMotivoChamadoData = PropertyEntity({ val: ko.observable(situacao.CodigoMotivoChamadoData), getType: typesKnockout.int, def: situacao.CodigoMotivoChamadoData });
    this.DataInicio = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.dateTime, enable: ko.observable(status), required: ko.observable(situacao.Obrigatorio), def: "", text: (situacao.Obrigatorio ? "*" : "") + "Data Chegada " + situacao.Descricao + ":" });
    this.DataFim = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.dateTime, enable: ko.observable(status), required: ko.observable(situacao.Obrigatorio), def: "", text: (situacao.Obrigatorio ? "*" : "") + "Data Saída " + situacao.Descricao + ":" });

    //this.DataInicio.dateRangeLimit = this.DataFim;
    //this.DataFim.dateRangeInit = this.DataInicio;

    if (situacao.DataInicio)
        this.DataInicio.val(situacao.DataInicio);
    if (situacao.DataFim)
        this.DataFim.val(situacao.DataFim);
};

/*
 * Declaração das Funções de Inicialização
 */

function loadChamadoData() {
    _chamadoData = new ChamadoData();
    KoBindings(_chamadoData, "knockoutChamadoData");
}

/*
 * Declaração das Funções
 */

function buscarDatasMotivoChamado() {
    if (_CONFIGURACAO_TMS.UtilizaListaDinamicaDatasChamado && _abertura.MotivoChamado.codEntity() != codigoMotivoChamadoCarregouData) {
        executarReST("ChamadoOcorrencia/BuscarDatasMotivoChamado", { Codigo: _abertura.MotivoChamado.codEntity() }, function (retorno) {
            if (retorno.Success) {
                carregarDatas(retorno.Data, true);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        });
    }
}

function carregarDatas(listaData, status) {
    limparCamposChamadoData();
    if (_CONFIGURACAO_TMS.UtilizaListaDinamicaDatasChamado && listaData.length > 0) {
        _chamadoData.PossuiDatas.val(true);
        $("#liChamadoData").show();
        codigoMotivoChamadoCarregouData = _abertura.MotivoChamado.codEntity();

        for (var i = 0; i < listaData.length; i++) {
            var data = new DataMotivoChamado(listaData[i], status);

            _chamadoData.Datas.push(data);

            //data.DataInicio.dateRangeLimit = data.DataFim;
            //data.DataFim.dateRangeInit = data.DataInicio;
            ConfigurarCampoDate(data.DataInicio);
            ConfigurarCampoDate(data.DataFim);
        }
    }
}

function obterDatas() {
    var datas = new Array();

    for (var i = 0; i < _chamadoData.Datas().length; i++) {
        var data = _chamadoData.Datas()[i];

        datas.push({
            Codigo: data.Codigo.val(),
            CodigoMotivoChamadoData: data.CodigoMotivoChamadoData.val(),
            DataInicio: data.DataInicio.val(),
            DataFim: data.DataFim.val()
        });
    }

    return JSON.stringify(datas);
}

function validarDatasInformadas() {
    if (!_CONFIGURACAO_TMS.UtilizaListaDinamicaDatasChamado)
        return true;

    var valido = true;
    for (var i = 0; i < _chamadoData.Datas().length; i++) {
        var data = _chamadoData.Datas()[i];

        if (!ValidarCamposObrigatorios(data))
            valido = false;
    }

    return valido;
}

function preencherEnvioDataMotivoChamado(dadosEnvioData, status) {
    carregarDatas(dadosEnvioData, status);
    calcularHorasDatas();
}

function calcularHorasDatas() {
    var hours = 0;
    var minute = 0;
    for (var i = 0; i < _chamadoData.Datas().length; i++) {
        var data = _chamadoData.Datas()[i];

        var dataInicio = data.DataInicio.val();
        var dataTermino = data.DataFim.val();

        if (!string.IsNullOrWhiteSpace(dataInicio) && !string.IsNullOrWhiteSpace(dataTermino)) {
            dataInicio = Global.criarData(dataInicio).getTime();
            dataTermino = Global.criarData(dataTermino).getTime();

            var msec = dataTermino - dataInicio;
            if (!isNaN(msec)) {
                var mins = Math.floor(msec / 60000);
                var hrs = Math.floor(mins / 60);

                mins = mins % 60;
                if ((minute + mins) >= 60) {
                    var minsAplicado = 60 - minute;
                    minute = mins - minsAplicado;
                    hours += 1;
                } else
                    minute += mins;
                hours += hrs;
            }
        }
    }

    _chamadoData.TotalHoras.val(addZero(hours) + ":" + addZero(minute));
}

function addZero(i) {
    if (i < 10) {
        i = "0" + i;
    }
    return i;
}

function limparCamposChamadoData() {
    _chamadoData.Datas.removeAll();
    _chamadoData.PossuiDatas.val(false);
    $("#liChamadoData").hide();
    codigoMotivoChamadoCarregouData = 0;
}