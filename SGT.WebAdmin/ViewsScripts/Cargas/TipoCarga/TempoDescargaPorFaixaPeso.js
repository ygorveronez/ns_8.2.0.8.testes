/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _tempoDescargaPorFaixaPeso;
var _gridFaixasPeso = null;

// Funções públicas

var TempoDescargaPorFaixaPeso = function () {
    this.InicioFaixaTempoDescargaPorPeso = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.InicioDaFaixaKg, visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, maxlength: 5, val: ko.observable("0"), def: "0" });
    this.FimFaixaTempoDescargaPorPeso = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.FimDaFaixaKg, visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, maxlength: 5, val: ko.observable("0"), def: "0" });
    this.TempoFaixaTempoDescarregamentoPorPeso = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.TempoDeDescargaMinutos, visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, maxlength: 5, val: ko.observable("0"), def: "0" });

    this.InserirFaixaTempoDescargaPorPeso = PropertyEntity({ eventClick: OnClickInserirFaixa, type: types.event, text: Localization.Resources.Cargas.TipoCarga.AdicionarFaixaDePeso, visible: ko.observable(true), idGrid: guid() });
};

function LoadTempoDescargaPorFaixaPeso() {
    _tempoDescargaPorFaixaPeso = new TempoDescargaPorFaixaPeso();
    KoBindings(_tempoDescargaPorFaixaPeso, "knockoutTempoDescargaPorFaixaPeso");

    LoadGridFaixasPeso();
}

function LoadGridFaixasPeso() {

    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            OnClickRemoverFaixa(_tempoDescargaPorFaixaPeso.InserirFaixaTempoDescargaPorPeso, data);
        }, tamanho: "15", icone: ""
    };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Inicio", title: Localization.Resources.Cargas.TipoCarga.InicioKg, width: "30%", className: "text-align-left" },
        { data: "Fim", title: Localization.Resources.Cargas.TipoCarga.FimKg, width: "30%", className: "text-align-left" },
        { data: "Tempo", title: Localization.Resources.Cargas.TipoCarga.TempoMinutos, width: "30%", className: "text-align-left" },
    ];

    _gridFaixasPeso = new BasicDataTable(_tempoDescargaPorFaixaPeso.InserirFaixaTempoDescargaPorPeso.idGrid, header, menuOpcoes);
    _tempoDescargaPorFaixaPeso.InserirFaixaTempoDescargaPorPeso.basicTable = _gridFaixasPeso;

    RecarregarListaFaixasPeso();
}

function preencherListaFaixasTempoDescargaPorPesoParaBackEnd() {
    _tipoCarga.ListaFaixasTempoDescargaPorPeso.list = new Array();
    let registros = _gridFaixasPeso.BuscarRegistros();
    _tipoCarga.ListaFaixasTempoDescargaPorPeso.val(JSON.stringify(registros));
}

// Funções privadas

function OnClickInserirFaixa(data) {
    if (data != null) {
        var novaFaixa = {
            Codigo: 0,
            Inicio: parseInt(_tempoDescargaPorFaixaPeso.InicioFaixaTempoDescargaPorPeso.val().replace(".", "")),
            Fim: parseInt(_tempoDescargaPorFaixaPeso.FimFaixaTempoDescargaPorPeso.val().replace(".", "")),
            Tempo: parseInt(_tempoDescargaPorFaixaPeso.TempoFaixaTempoDescarregamentoPorPeso.val().replace(".", "")),
        }

        if (!ValidarFaixa(novaFaixa)) {
            return;
        }

        var dataGrid = _gridFaixasPeso.BuscarRegistros();
        dataGrid.push(novaFaixa);

        dataGrid.sort((a, b) => a.Inicio < b.Inicio ? -1 : 1);

        _gridFaixasPeso.CarregarGrid(dataGrid);

        ClearCamposFaixaTempoDescarga();
    }
}

// Valida se a faixa pode ser adicionada
function ValidarFaixa(faixa) {

    if (faixa.Inicio === null || faixa.Fim === null || faixa.Tempo === null) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.TipoCarga.Erro, Localization.Resources.Cargas.TipoCarga.TodosOsValoresDevemSerPreenchidos);
        return false;
    }

    if (faixa.Tempo == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.TipoCarga.Erro, Localization.Resources.Cargas.TipoCarga.TempoDaFaixaDeveSerMaiorQueZero);
        return false;
    }

    if (faixa.Inicio < 0 || faixa.Fim < 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.TipoCarga.Erro, Localization.Resources.Cargas.TipoCarga.OsValoresDaFaixaNaoPodemSerNegativos);
        return false;
    }

    if (faixa.Inicio >= faixa.Fim) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.TipoCarga.Erro, Localization.Resources.Cargas.TipoCarga.InicioDaFaixaNaoPodeSerMmaiorOuIgualAoFim);
        return false;
    }

    var registros = _gridFaixasPeso.BuscarRegistros();

    for (var i = 0; i < registros.length; i += 1) {
        let registro = registros[i];

        if (faixa.Inicio >= registro.Inicio && faixa.Inicio <= registro.Fim) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.TipoCarga.Erro, Localization.Resources.Cargas.TipoCarga.FaixaEntraEmConflitoComOutraFaixaJaCadastrada);
            return false;
        }

        if (faixa.Fim >= registro.Inicio && faixa.Fim <= registro.Fim) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.TipoCarga.Erro, Localization.Resources.Cargas.TipoCarga.FaixaEntraEmConflitoComOutraFaixaJaCadastrada);
            return false;
        }

        if (faixa.Inicio < registro.Inicio && faixa.Fim > registro.Fim) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.TipoCarga.Erro, Localization.Resources.Cargas.TipoCarga.FaixaEntraEmConflitoComOutraFaixaJaCadastrada);
            return false;
        }
    }

    return true;
}

function OnClickRemoverFaixa(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.TipoCarga.RealmenteDesejaExcluirFaixaDePeso, function () {
        var registros = e.basicTable.BuscarRegistros();

        for (var i = 0; i < registros.length; i++) {
            if (sender.Inicio == registros[i].Inicio && sender.Fim == registros[i].Fim) {
                registros.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(registros);
    });
}

function RecarregarListaFaixasPeso() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tipoCarga.ListaFaixasTempoDescargaPorPeso.val().slice())) {

        $.each(_tipoCarga.ListaFaixasTempoDescargaPorPeso.val().slice(), function (i, faixa) {
            var obj = new Object();

            obj.Codigo = faixa.Codigo;
            obj.Inicio = faixa.Inicio;
            obj.Fim = faixa.Fim;
            obj.Tempo = faixa.Tempo;

            data.push(obj);
        });
    }

    _gridFaixasPeso.CarregarGrid(data);
}

function ClearCamposFaixaTempoDescarga() {
    _tempoDescargaPorFaixaPeso.InicioFaixaTempoDescargaPorPeso.val("0");
    _tempoDescargaPorFaixaPeso.FimFaixaTempoDescargaPorPeso.val("0");
    _tempoDescargaPorFaixaPeso.TempoFaixaTempoDescarregamentoPorPeso.val("0");
}

