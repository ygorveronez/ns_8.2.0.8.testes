/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumTipoArredondamento.js" />
/// <reference path="SinistroEtapaIndicacaoPagador.js" />

var _etapaIndicadorPagadorParcelamento;
var _gridIndicacaoPagadorParcelas;
var _detalheIndicacaoPagadorParcelamento;

var IndicacaoPagadorSinistroParcelamento = function () {
    this.QuantidadeParcelas = PropertyEntity({ text: "*Qtd. Parcelas:", getType: typesKnockout.int, val: ko.observable(""), def: "", required: ko.observable(false), enable: ko.observable(true) });
    this.IntervaloDias = PropertyEntity({ text: "*Intervalo de Dias (Ex.: 20.30.40 para intervalos diferentes): ", maxlength: 100, required: ko.observable(false), enable: ko.observable(true) });
    this.DataPrimeiroVencimento = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), text: "*Data Primeiro Vencimento:", required: ko.observable(false), enable: ko.observable(true) });
    this.TipoArredondamento = PropertyEntity({ text: "Arredondar na:", options: EnumTipoArredondamento.ObterOpcoes(), val: ko.observable(EnumTipoArredondamento.PrimeiroItem), def: EnumTipoArredondamento.PrimeiroItem, required: false, enable: ko.observable(true) });

    this.GerarParcelas = PropertyEntity({ text: "Gerar Parcelas", eventClick: gerarParcelasClick, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });

    this.GridParcelas = PropertyEntity({ type: types.local, val: ko.observableArray([]), idGrid: guid() });
};

var IndicacaoPagadorSinistroParcelaMap = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Parcela = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Valor = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date });
};

var DetalheIndicacaoPagadorSinistroParcelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Parcela = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Sequência:", maxlength: 10, enable: ko.observable(false) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10, enable: ko.observable(true), configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data Vencimento:", enable: ko.observable(true) });

    this.SalvarParcela = PropertyEntity({ eventClick: SalvarDetalheParcelaClick, type: types.event, text: "Salvar Parcela", enable: ko.observable(true), visible: ko.observable(true) });
};

function loadEtapaIndicacaoPagadorParcelamento() {
    _etapaIndicadorPagadorParcelamento = new IndicacaoPagadorSinistroParcelamento();
    KoBindings(_etapaIndicadorPagadorParcelamento, "knockoutFluxoSinistroIndicacaoPagadorParcelamento");

    _detalheIndicacaoPagadorParcelamento = new DetalheIndicacaoPagadorSinistroParcelamento();
    KoBindings(_detalheIndicacaoPagadorParcelamento, "knockoutDetalheIndicacaoPagadorSinistroParcelamento");

    loadGridIndicadorPagadorParcelas();
}

function gerarParcelasClick() {
    _etapaIndicadorPagadorSinistro.ValorOriginal.required(true);
    _etapaIndicadorPagadorParcelamento.QuantidadeParcelas.required(true);
    _etapaIndicadorPagadorParcelamento.IntervaloDias.required(true);
    _etapaIndicadorPagadorParcelamento.DataPrimeiroVencimento.required(true);

    var validoGeral = ValidarCamposObrigatorios(_etapaIndicadorPagadorSinistro);
    var valido = ValidarCamposObrigatorios(_etapaIndicadorPagadorParcelamento);
    if (!valido || !validoGeral)
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");

    _etapaIndicadorPagadorParcelamento.QuantidadeParcelas.required(false);
    _etapaIndicadorPagadorParcelamento.IntervaloDias.required(false);
    _etapaIndicadorPagadorParcelamento.DataPrimeiroVencimento.required(false);

    if (!valido || !validoGeral)
        return;

    var valorTotal = Globalize.parseFloat(_etapaIndicadorPagadorSinistro.ValorOriginal.val());
    var quantidadeDuplicatas = Globalize.parseInt(_etapaIndicadorPagadorParcelamento.QuantidadeParcelas.val());

    var x = _etapaIndicadorPagadorParcelamento.IntervaloDias.val();
    if (x.indexOf(".") >= 0) {
        var arrayDias = x.split(".");
        if (arrayDias.length != quantidadeDuplicatas) {
            exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "As quantidades das parcelas não estão de acordo com o intervalo de dias informado!");
            return;
        }
        for (var i = 0; i < arrayDias.length; i++) {
            if (parseInt(arrayDias[i]) == NaN) {
                exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "O intervalo de dias está fora do formato desejado!");
                return;
            }
        }
    } else {
        var arrayDias = new Array;
        arrayDias[0] = x;
        if (!parseInt(arrayDias[0]) > 0 || parseInt(arrayDias[0]) == NaN) {
            exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "O intervalo de dias está fora do formato desejado!");
            return;
        }
    }

    _etapaIndicadorPagadorSinistro.Parcelas.list = [];

    var dataUltimaParcela = _etapaIndicadorPagadorParcelamento.DataPrimeiroVencimento.val();
    var valorParcela = Globalize.parseFloat(Globalize.format(valorTotal / quantidadeDuplicatas, "n2"));
    var valorDiferenca = Globalize.parseFloat(Globalize.format(valorTotal - (valorParcela * quantidadeDuplicatas), "n2"));
    var tipoArredondamento = _etapaIndicadorPagadorParcelamento.TipoArredondamento.val();

    if (isNaN(quantidadeDuplicatas) || quantidadeDuplicatas <= 0)
        quantidadeDuplicatas = 1;

    for (var i = 0; i < quantidadeDuplicatas; i++) {
        dataUltimaParcela = dataUltimaParcela.substr(6, 4) + "/" + dataUltimaParcela.substr(3, 2) + "/" + dataUltimaParcela.substr(0, 2);
        var dataVencimento = new Date(dataUltimaParcela);
        if (i > 0) {
            if (arrayDias.length > 1)
                dataVencimento.setDate(dataVencimento.getDate() + parseInt(arrayDias[i]));
            else
                dataVencimento.setDate(dataVencimento.getDate() + parseInt(arrayDias[0]));
        }

        var parcelaGrid = new IndicacaoPagadorSinistroParcelaMap();
        parcelaGrid.Codigo.val(guid());
        parcelaGrid.DataVencimento.val(moment(dataVencimento).format("DD/MM/YYYY"));
        parcelaGrid.Parcela.val(i + 1);

        var valor = 0;
        if (i == 0 && tipoArredondamento === EnumTipoArredondamento.PrimeiroItem)
            valor = (valorParcela + valorDiferenca);
        else if ((i + 1) == quantidadeDuplicatas && tipoArredondamento === EnumTipoArredondamento.UltimoItem)
            valor = (valorParcela + valorDiferenca);
        else
            valor = valorParcela;
        parcelaGrid.Valor.val(Globalize.format(valor, "n2"));

        var yyyy = dataVencimento.getFullYear().toString();
        var mm = (dataVencimento.getMonth() + 1).toString();
        var dd = dataVencimento.getDate().toString();

        dataUltimaParcela = (dd[1] ? dd : "0" + dd[0]) + "/" + (mm[1] ? mm : "0" + mm[0]) + "/" + yyyy;

        _etapaIndicadorPagadorSinistro.Parcelas.list.push(SalvarListEntity(parcelaGrid));
    }

    limparCamposIndicadorPagadorParcela();
    recarregarGridIndicacaoPagadorParcela();
}

function loadGridIndicadorPagadorParcelas() {
    var linhasPorPagina = 5;

    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarParcelaClick, icone: "" };
    var opcaoExcluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirParcelaClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoEditar, opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Parcela", title: "Parcela", width: "20%", className: "text-align-left" },
        { data: "DataVencimento", title: "Data Vencimento", width: "20%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "20%", className: "text-align-right" }
    ];

    _gridIndicacaoPagadorParcelas = new BasicDataTable(_etapaIndicadorPagadorParcelamento.GridParcelas.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, linhasPorPagina);
    _gridIndicacaoPagadorParcelas.CarregarGrid([]);
}

function excluirParcelaClick(parcela) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover a parcela selecionada?", function () {
        $.each(_etapaIndicadorPagadorSinistro.Parcelas.list, function (i, listaParcelas) {
            if (parcela.Codigo == listaParcelas.Codigo.val) {
                _etapaIndicadorPagadorSinistro.Parcelas.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridIndicacaoPagadorParcela();
    });
}

function editarParcelaClick(parcela) {
    LimparCampos(_detalheIndicacaoPagadorParcelamento);
    _detalheIndicacaoPagadorParcelamento.Parcela.enable(false);

    if (Boolean(parcela.Codigo)) {
        var data =
        {
            Codigo: parcela.Codigo,
            Parcela: parcela.Parcela,
            Valor: parcela.Valor,
            DataVencimento: parcela.DataVencimento
        };

        var dataParcela = { Data: data };
        PreencherObjetoKnout(_detalheIndicacaoPagadorParcelamento, dataParcela);        
        Global.abrirModal('divDetalheIndicacaoPagadorSinistroParcelamento');
    }
}

function SalvarDetalheParcelaClick() {
    if (!ValidarCamposObrigatorios(_detalheIndicacaoPagadorParcelamento))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");

    for (var i = 0; i < _etapaIndicadorPagadorSinistro.Parcelas.list.length; i++) {
        if (_detalheIndicacaoPagadorParcelamento.Codigo.val() == _etapaIndicadorPagadorSinistro.Parcelas.list[i].Codigo.val) {
            _etapaIndicadorPagadorSinistro.Parcelas.list.splice(i, 1);
            break;
        }
    }

    var parcelaGrid = new IndicacaoPagadorSinistroParcelaMap();
    parcelaGrid.Codigo.val(_detalheIndicacaoPagadorParcelamento.Codigo.val());
    parcelaGrid.Parcela.val(_detalheIndicacaoPagadorParcelamento.Parcela.val());
    parcelaGrid.DataVencimento.val(_detalheIndicacaoPagadorParcelamento.DataVencimento.val());
    parcelaGrid.Valor.val(_detalheIndicacaoPagadorParcelamento.Valor.val());

    _etapaIndicadorPagadorSinistro.Parcelas.list.push(SalvarListEntity(parcelaGrid));

    recarregarGridIndicacaoPagadorParcela();

    Global.fecharModal('divDetalheIndicacaoPagadorSinistroParcelamento');
}

function recarregarGridIndicacaoPagadorParcela() {
    var data = new Array();

    $.each(_etapaIndicadorPagadorSinistro.Parcelas.list, function (i, parcela) {
        var parcelaGrid = new Object();

        parcelaGrid.Codigo = parcela.Codigo.val;
        parcelaGrid.Parcela = parcela.Parcela.val;
        parcelaGrid.Valor = parcela.Valor.val;
        parcelaGrid.DataVencimento = parcela.DataVencimento.val;

        data.push(parcelaGrid);
    });

    _gridIndicacaoPagadorParcelas.CarregarGrid(data);
}

function bloquearCamposIndicadorPagadorParcela() {
    SetarEnableCamposKnockout(_etapaIndicadorPagadorParcelamento, false);
    _etapaIndicadorPagadorParcelamento.GerarParcelas.visible(false);

    _gridIndicacaoPagadorParcelas.DesabilitarOpcoes();
}

function limparCamposIndicadorPagadorParcela() {
    LimparCampos(_etapaIndicadorPagadorParcelamento);
}

function limparCamposIndicacaoPagadorParcelamento() {
    LimparCampos(_etapaIndicadorPagadorParcelamento);
    _etapaIndicadorPagadorSinistro.Parcelas.list = [];

    SetarEnableCamposKnockout(_etapaIndicadorPagadorParcelamento, true);
    _etapaIndicadorPagadorParcelamento.GerarParcelas.visible(true);

    _gridIndicacaoPagadorParcelas.HabilitarOpcoes();
}