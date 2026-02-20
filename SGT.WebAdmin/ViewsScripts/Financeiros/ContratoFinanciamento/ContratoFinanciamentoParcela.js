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
/// <reference path="../../Enumeradores/EnumPeriodicidade.js" />
/// <reference path="ContratoFinanciamento.js" />
/// <reference path="ContratoFinanciamentoParcelaValor.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contratoFinanciamentoParcela, _gridContratoFinanciamentoParcelas;

var _periodoContratoFinanciamentoParcela = [
    { text: "Mensal", value: EnumPeriodicidade.Mensal },
    { text: "Semanal", value: EnumPeriodicidade.Semanal },
    { text: "Bimestral", value: EnumPeriodicidade.Bimestral },
    { text: "Trimestral", value: EnumPeriodicidade.Trimestral },
    { text: "Semestral", value: EnumPeriodicidade.Semestral },
    { text: "Anual", value: EnumPeriodicidade.Anual },
];

var ContratoFinanciamentoParcela = function () {
    this.Provisao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Estes títulos serão uma Provisão?", def: false, enable: ko.observable(true) });
    
    this.VencimentoPrimeiraParcela = PropertyEntity({ text: "Venc. Primeira Parcela: ", getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(false) });

    this.DiaOcorrencia = PropertyEntity({ getType: typesKnockout.int, text: "*Dia da Ocorrência:", maxlength: 2, visible: ko.observable(false), enable: ko.observable(true) });
    this.NumeroOcorrencia = PropertyEntity({ getType: typesKnockout.int, text: "*Numero Ocorrência:", visible: ko.observable(false), enable: ko.observable(true) });
    this.Repetir = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deseja REPETIR o lançamento? Isso gerará mais parcelas do mesmo valor informado.", def: false, enable: ko.observable(true), eventChange: HabilitarCamposRepetir });
    this.Dividir = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deseja DIVIDIR o lançamento? Isso gerará mais parcelas com o valor informado dividido entre eles.", def: false, enable: ko.observable(true), eventChange: HabilitarCamposDividir });
    this.Periodicidade = PropertyEntity({ val: ko.observable(EnumPeriodicidade.Mensal), def: EnumPeriodicidade.Mensal, options: _periodoContratoFinanciamentoParcela, text: "*Repetição:", required: false, visible: ko.observable(false), enable: ko.observable(true) });

    this.GerarParcela = PropertyEntity({ eventClick: gerarParcelaClick, type: types.event, text: "Gerar Parcelas", enable: ko.observable(true), visible: ko.observable(false) });
    this.LimparParcela = PropertyEntity({ eventClick: limparParcelaClick, type: types.event, text: "Limpar Parcelas", enable: ko.observable(false), visible: ko.observable(false) });
    this.Parcelas = PropertyEntity({ type: types.local, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadContratoFinanciamentoParcela() {
    _contratoFinanciamentoParcela = new ContratoFinanciamentoParcela();
    KoBindings(_contratoFinanciamentoParcela, "knockoutParcelamentoContratoFinanciamento");

    $("#" + _contratoFinanciamentoParcela.Repetir.id).click(HabilitarCamposRepetir);
    $("#" + _contratoFinanciamentoParcela.Dividir.id).click(HabilitarCamposDividir);

    var editarColuna = { permite: true, callback: callbackEditarParcelasContratoFinanciamentoColuna, atualizarRow: false };

    _gridContratoFinanciamentoParcelas = new GridView(_contratoFinanciamentoParcela.Parcelas.id, "ContratoFinanciamento/PesquisaParcelas", _contratoFinanciamento, null, { column: 0, dir: orderDir.asc }, 50, null, null, null, null, null, editarColuna);
    _gridContratoFinanciamentoParcelas.CarregarGrid();

    loadContratoFinanciamentoParcelaValor();
}

//*******MÉTODOS*******

function editarDemaisValoresContratoFinanciamentoParcela(parcelaGrid) {
    LimparCamposContratoFinanciamentoParcelaValor();
    _contratoFinanciamentoParcelaValor.CodigoContratoFinanciamentoParcela.val(parcelaGrid.Codigo);
    _gridContratoFinanciamentoParcelaValor.CarregarGrid();
        
    Global.abrirModal("divContratoFinanciamentoParcelaValor");
}

function callbackEditarParcelasContratoFinanciamentoColuna(dataRow, row, head, callbackTabPress) {
    executarReST("ContratoFinanciamento/AlterarDadosContratoFinanciamentoParcela", dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data.dynParcelaContratoFinanciamento);
                _gridContratoFinanciamentoParcelas.AtualizarDataRow(row, dataRow, callbackTabPress);
                _gridContratoFinanciamentoParcelas.CarregarGrid(null, true);
            } else {
                _gridContratoFinanciamentoParcelas.DesfazerAlteracaoDataRow(row);
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            _gridContratoFinanciamentoParcelas.DesfazerAlteracaoDataRow(row);
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function desativarEditarGridContratoFinanciamentoParcelas() {
    var editarColuna = { permite: false, callback: null, atualizarRow: false };
    _gridContratoFinanciamentoParcelas.SetarEditarColunas(editarColuna);
}

function habilitarEditarGridContratoFinanciamentoParcelas() {
    var editarColuna = { permite: true, callback: callbackEditarParcelasContratoFinanciamentoColuna, atualizarRow: false };
    _gridContratoFinanciamentoParcelas.SetarEditarColunas(editarColuna);
}

function gerarParcelaClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja gerar as parcelas com essas informações lançadas? As quantidades não poderão ser alteradas", function () {
        adicionarClick();
    });
}

function limparParcelaClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente limpar as parcelas lançadas?", function () {
        executarReST("ContratoFinanciamento/LimparParcelas", { Codigo: _contratoFinanciamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    editarContratoFinanciamento({ Codigo: _contratoFinanciamento.Codigo.val() }, true);                    
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
    
}

function HabilitarCamposDividir(e, sender) {
    _contratoFinanciamentoParcela.Repetir.val(false);

    if (_contratoFinanciamentoParcela.Dividir.val() === false) {
        _contratoFinanciamentoParcela.Periodicidade.val(EnumPeriodicidade.Mensal);
        _contratoFinanciamentoParcela.NumeroOcorrencia.val("");
        _contratoFinanciamentoParcela.DiaOcorrencia.val("");

        _contratoFinanciamentoParcela.Periodicidade.visible(false);
        _contratoFinanciamentoParcela.VencimentoPrimeiraParcela.visible(false);
        _contratoFinanciamentoParcela.NumeroOcorrencia.visible(false);
        _contratoFinanciamentoParcela.DiaOcorrencia.visible(false);
        _contratoFinanciamentoParcela.Periodicidade.required = false;
        _contratoFinanciamentoParcela.NumeroOcorrencia.required = false;
        _contratoFinanciamentoParcela.DiaOcorrencia.required = false;

        _contratoFinanciamentoParcela.GerarParcela.visible(false);
        _contratoFinanciamentoParcela.LimparParcela.visible(false);
    } else {
        _contratoFinanciamentoParcela.Periodicidade.visible(true);
        _contratoFinanciamentoParcela.NumeroOcorrencia.visible(true);
        _contratoFinanciamentoParcela.VencimentoPrimeiraParcela.visible(true);
        _contratoFinanciamentoParcela.DiaOcorrencia.visible(true);
        _contratoFinanciamentoParcela.Periodicidade.required = true;
        _contratoFinanciamentoParcela.NumeroOcorrencia.required = true;
        _contratoFinanciamentoParcela.DiaOcorrencia.required = true;

        _contratoFinanciamentoParcela.GerarParcela.visible(true);        
    }
}

function HabilitarCamposRepetir(e, sender) {
    _contratoFinanciamentoParcela.Dividir.val(false);

    if (_contratoFinanciamentoParcela.Repetir.val() === false) {
        _contratoFinanciamentoParcela.Periodicidade.val(EnumPeriodicidade.Mensal);
        _contratoFinanciamentoParcela.NumeroOcorrencia.val("");
        _contratoFinanciamentoParcela.DiaOcorrencia.val("");
        _contratoFinanciamentoParcela.VencimentoPrimeiraParcela.val("");

        _contratoFinanciamentoParcela.Periodicidade.visible(false);
        _contratoFinanciamentoParcela.NumeroOcorrencia.visible(false);
        _contratoFinanciamentoParcela.VencimentoPrimeiraParcela.visible(false);
        _contratoFinanciamentoParcela.DiaOcorrencia.visible(false);
        _contratoFinanciamentoParcela.Periodicidade.required = false;
        _contratoFinanciamentoParcela.NumeroOcorrencia.required = false;
        _contratoFinanciamentoParcela.DiaOcorrencia.required = false;

        _contratoFinanciamentoParcela.GerarParcela.visible(false);
        _contratoFinanciamentoParcela.LimparParcela.visible(false);
    } else {
        _contratoFinanciamentoParcela.Periodicidade.visible(true);
        _contratoFinanciamentoParcela.NumeroOcorrencia.visible(true);
        _contratoFinanciamentoParcela.VencimentoPrimeiraParcela.visible(true);
        _contratoFinanciamentoParcela.DiaOcorrencia.visible(true);
        _contratoFinanciamentoParcela.Periodicidade.required = true;
        _contratoFinanciamentoParcela.NumeroOcorrencia.required = true;
        _contratoFinanciamentoParcela.DiaOcorrencia.required = true;

        _contratoFinanciamentoParcela.GerarParcela.visible(true);        
    }
}

function limparCamposContratoFinanciamentoParcela() {
    LimparCampos(_contratoFinanciamentoParcela);

    _contratoFinanciamentoParcela.Periodicidade.visible(false);
    _contratoFinanciamentoParcela.NumeroOcorrencia.visible(false);
    _contratoFinanciamentoParcela.VencimentoPrimeiraParcela.visible(false);
    _contratoFinanciamentoParcela.DiaOcorrencia.visible(false);
    _contratoFinanciamentoParcela.GerarParcela.visible(false);
    _contratoFinanciamentoParcela.LimparParcela.visible(false);
    _contratoFinanciamentoParcela.Periodicidade.required = false;
    _contratoFinanciamentoParcela.NumeroOcorrencia.required = false;
    _contratoFinanciamentoParcela.DiaOcorrencia.required = false;
    _contratoFinanciamentoParcela.Periodicidade.enable(true);
    _contratoFinanciamentoParcela.NumeroOcorrencia.enable(true);
    _contratoFinanciamentoParcela.VencimentoPrimeiraParcela.enable(true);
    _contratoFinanciamentoParcela.DiaOcorrencia.enable(true);
    _contratoFinanciamentoParcela.Repetir.enable(true);
    _contratoFinanciamentoParcela.Dividir.enable(true);
}

function validaCamposObrigatoriosContratoFinanciamentoParcela() {
    var tudoCerto = ValidarCamposObrigatorios(_contratoFinanciamentoParcela);
    if (tudoCerto) {
        if (_contratoFinanciamentoParcela.Dividir.val() === false && _contratoFinanciamentoParcela.Repetir.val() === false)
            tudoCerto = false;
    }

    return tudoCerto;
}