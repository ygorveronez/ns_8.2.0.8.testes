/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _tempoBalsa;
var _gridTempoBalsa;

// Funções públicas

var TempoBalsa = function () {
    this.TempoGeral = PropertyEntity({ text: "*Tempo geral em Dias: ", getType: typesKnockout.int, visible: ko.observable(true), def: ko.observable(0), required: true, configInt: { precision: 0, allowZero: true } });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable() });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, val: ko.observable() });

    this.InserirTempoBalsa = PropertyEntity({ eventClick: OnClickInserirFaixa, type: types.event, text: "Adicionar Tempo do Trecho Balsa", visible: ko.observable(true), idGrid: guid() });
}

function LoadTempoBalsa() {
    _tempoBalsa = new TempoBalsa();
    KoBindings(_tempoBalsa, "knockoutTempoBalsa");

    LoadGridTempoBalsa();
}

function LoadGridTempoBalsa() {

    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            OnClickRemoverFaixa(_tempoBalsa.InserirTempoBalsa, data);
        }, tamanho: "15", icone: ""
    };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "DataInicio", title: "De", width: "30%", className: "text-align-left" },
        { data: "DataFinal", title: "Até", width: "30%", className: "text-align-left" },
        { data: "TempoGeral", title: "Tempo de", width: "30%", className: "text-align-left" },
    ];

    _gridTempoBalsa = new BasicDataTable(_tempoBalsa.InserirTempoBalsa.idGrid, header, menuOpcoes);
    _tempoBalsa.InserirTempoBalsa.basicTable = _gridTempoBalsa;

    RecarregarListaTempoBalsa();
}

function preencherListaTempoBalsaParaBackEnd() {
    _trechoBalsa.ListaTempoBalsa.list = new Array();
    let registros = _gridTempoBalsa.BuscarRegistros();
    _trechoBalsa.ListaTempoBalsa.val(JSON.stringify(registros));
}

// Funções privadas

function OnClickInserirFaixa(data) {
    if (data != null) {
        var novaFaixa = {
            Codigo: 0,
            DataInicio: _tempoBalsa.DataInicio.val(),
            DataFinal: _tempoBalsa.DataFinal.val(),
            TempoGeral: parseInt(_tempoBalsa.TempoGeral.val().replace(".", "")),
        }

        if (!ValidarFaixa(novaFaixa)) {
            return;
        }

        var dataGrid = _gridTempoBalsa.BuscarRegistros();
        dataGrid.push(novaFaixa);

        _gridTempoBalsa.CarregarGrid(dataGrid);

        ClearCamposFaixaTempoBalsa();
    }
}

// Valida se a faixa pode ser adicionada
function ValidarFaixa(faixa) {
    let faixaDataInicio = faixa.DataInicio.split("/");
    let faixaDataFim = faixa.DataFinal.split("/");
    let dataInicio = new Date(faixaDataInicio[2], faixaDataInicio[1], faixaDataInicio[0]);
    let dataFinal = new Date(faixaDataFim[2], faixaDataFim[1], faixaDataFim[0]);

    if (faixa.TempoGeral === null || faixa.TempoGeral == 0) {
        exibirMensagem(tipoMensagem.atencao, "Erro", "O tempo geral deve ser informado");
        return false;
    }

    if (dataInicio > dataFinal) {
        exibirMensagem(tipoMensagem.atencao, "Erro", "A data inicial não pode ser maior que a data final");
        return false;
    }

    var registros = _gridTempoBalsa.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        let registro = registros[i];
        let regDataInicio = registro.DataInicio.split("/");
        let regDataFim = registro.DataFinal.split("/");
        let registroDataInicio = new Date(regDataInicio[2], regDataInicio[1], regDataInicio[0]);
        let registroDataFinal = new Date(regDataFim[2], regDataFim[1], regDataFim[0]);

        if (dataInicio >= registroDataInicio && dataInicio <= registroDataFinal) {
            exibirMensagem(tipoMensagem.atencao, "Erro", "A data de inicio entra em conflito com outra faixa já cadastrada");
            return false;
        }

        if (dataFinal >= registroDataInicio && dataFinal <= registroDataFinal) {
            exibirMensagem(tipoMensagem.atencao, "Erro", "A data final entra em conflito com outra faixa já cadastrada");
            return false;
        }

        if (dataInicio < registroDataInicio && dataFinal > registroDataFinal) {
            exibirMensagem(tipoMensagem.atencao, "Erro", "As datas entram em conflito com outra faixa já cadastrada");
            return false;
        }
    }

    return true;
}

function OnClickRemoverFaixa(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja excluir essa faixa?", function () {
        var registros = e.basicTable.BuscarRegistros();
        
        for (var i = 0; i < registros.length; i++) {
            if (sender.DataInicio == registros[i].DataInicio && sender.DataFinal == registros[i].DataFinal) {
                registros.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(registros);
    });
}

function RecarregarListaTempoBalsa() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_trechoBalsa.ListaTempoBalsa.val().slice())) {

        $.each(_trechoBalsa.ListaTempoBalsa.val().slice(), function (i, faixa) {
            var obj = new Object();

            obj.Codigo = faixa.Codigo;
            obj.DataInicio = faixa.DataInicio;
            obj.DataFinal = faixa.DataFinal;
            obj.TempoGeral = faixa.TempoGeral;

            data.push(obj);
        });
    }

    _gridTempoBalsa.CarregarGrid(data);
}

function ClearCamposFaixaTempoBalsa() {
    LimparCampos(_tempoBalsa);
}

