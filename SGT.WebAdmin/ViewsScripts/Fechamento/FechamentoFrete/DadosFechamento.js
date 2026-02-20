/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Consultas/ContratoFreteTransportador.js" />
/// <reference path="../../Enumeradores/EnumDezena.js" />
/// <reference path="../../Enumeradores/EnumQuinzena.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoComplementoContratoFreteTransportador.js" />

// #region Objetos Globais do Arquivo

var _dadosFechamento;

var _periodoMes = [
    { text: "Janeiro", value: 1 },
    { text: "Fevereiro", value: 2 },
    { text: "Março", value: 3 },
    { text: "Abril", value: 4 },
    { text: "Maio", value: 5 },
    { text: "Junho", value: 6 },
    { text: "Julho", value: 7 },
    { text: "Agosto", value: 8 },
    { text: "Setembro", value: 9 },
    { text: "Outubro", value: 10 },
    { text: "Novembro", value: 11 },
    { text: "Dezembro", value: 12 }
];

// #endregion Objetos Globais do Arquivo

// #region Classes

var DadosFechamento = function () {
    var _anoAtual = (new Date()).getFullYear();
    var _mesAtual = (new Date()).getMonth() + 1;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Contrato = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Contrato:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), eventChange: contratoFreteBlur, tipoEmissaoComplemento: EnumTipoEmissaoComplementoContratoFreteTransportador.PorTomador });
    this.TipoFechamento = PropertyEntity({ text: "Tipo do Fechamento: ", getType: typesKnockout.map, visible: ko.observable(true) });
    this.TipoFranquia = PropertyEntity({ text: "Tipo da Franquia: ", getType: typesKnockout.map, visible: ko.observable(true) });
    this.EnumPeriodoAcordo = PropertyEntity({ val: ko.observable(EnumPeriodoAcordoContratoFreteTransportador.Decendial), def: EnumPeriodoAcordoContratoFreteTransportador.Decendial });
    this.PeriodoSemana = PropertyEntity({ val: ko.observable(EnumSemana.Primeira), options: EnumSemana.obterOpcoes(), def: EnumSemana.Primeira, text: "Semana: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.PeriodoDezena = PropertyEntity({ val: ko.observable(EnumDezena.Primeira), options: EnumDezena.obterOpcoes(), def: EnumDezena.Primeira, text: "Dezena: ", visible: ko.observable(false), enable: ko.observable(true) });//, eventChange: periodoCargaBlur });
    this.PeriodoQuinzena = PropertyEntity({ val: ko.observable(EnumQuinzena.Primeira), options: EnumQuinzena.obterOpcoes(), def: EnumQuinzena.Primeira, text: "Quinzena: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.PeriodoMes = PropertyEntity({ val: ko.observable(_mesAtual), options: _periodoMes, def: _mesAtual, text: "Mês: ", visible: ko.observable(true), enable: ko.observable(true), eventChange: periodoCargaBlur });
    this.PeriodoAno = PropertyEntity({ text: "Ano: ", getType: typesKnockout.int, val: ko.observable(_anoAtual), def: _anoAtual, visible: ko.observable(true), maxlength: 4, enable: ko.observable(true), configInt: { precision: 0, allowZero: false, thousands: '' }, eventChange: periodoCargaBlur });
    this.ValorPorMotorista = PropertyEntity({ getType: typesKnockout.decimal, visible: false });
    this.DataInicio = PropertyEntity({ getType: typesKnockout.date, visible: false });
    this.DataFim = PropertyEntity({ getType: typesKnockout.date, visible: false });
    this.CargasRemovidas = PropertyEntity({ type: types.map, val: GetSetCargasRemovidas, def: [], list: [] });
    this.Acordos = [];

    this.PeriodoSemana.val.subscribe(parametrosDadosFechamentoAlterados);
    this.PeriodoDezena.val.subscribe(parametrosDadosFechamentoAlterados);
    this.PeriodoQuinzena.val.subscribe(parametrosDadosFechamentoAlterados);
    this.PeriodoMes.val.subscribe(parametrosDadosFechamentoAlterados);
    this.PeriodoAno.val.subscribe(function (novoValor) {
        if (novoValor.length == 4)
            parametrosDadosFechamentoAlterados();
    });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDadosFechamento() {
    _dadosFechamento = new DadosFechamento();
    KoBindings(_dadosFechamento, "knockoutDadosFechamento");

    new BuscarContratoFreteTransportador(_dadosFechamento.Contrato, retornoBuscaContrato);

    _fechamentoFrete.Codigo.val.subscribe(function (val) {
        _dadosFechamento.Codigo.val(val);
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function contratoFreteBlur() {
    if (_dadosFechamento.Contrato.val() == "") {
        _dadosFechamento.Contrato.tipoEmissaoComplemento = EnumTipoEmissaoComplementoContratoFreteTransportador.PorTomador;
        controlarInformacoesVisiveisPorContrato();
    }
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposDadosFechamento() {
    LimparCampos(_dadosFechamento);

    _dadosFechamento.Acordos = [];
    _dadosFechamento.PeriodoSemana.visible(false);
    _dadosFechamento.PeriodoDezena.visible(false);
    _dadosFechamento.PeriodoQuinzena.visible(false);
    _dadosFechamento.Contrato.tipoEmissaoComplemento = EnumTipoEmissaoComplementoContratoFreteTransportador.PorTomador;

    controlarCamposDadosFechamentoHabilitados(true);
    controlarInformacoesVisiveisPorContrato();
}

function preencherDadosFechamento(dados) {
    PreencherObjetoKnout(_dadosFechamento, { Data: dados });

    _dadosFechamento.Contrato.tipoEmissaoComplemento = dados.TipoEmissaoComplemento;

    controlarCamposDadosFechamentoHabilitados(false);
    parametrosDadosFechamentoAlterados();
    buscarDetalhesFranquia();
    controlarInformacoesVisiveisPorContrato();
    carregarAcordosContrato();
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarInformacoesVisiveisPorContrato() {
    if (_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorFaixaKm)
        return;

    if (_dadosFechamento.Contrato.tipoEmissaoComplemento == EnumTipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista) {
        $("#knockoutSumarizadoFranquia").hide();
        $("#dados-fechamento-periodo").hide();
        $("#knockoutResumo").hide();
        $("#knockoutValoresOutrosRecursosFechamento").show();
        $("#knockoutVeiculosUtilizados").show();
        $("#knockoutMotoristasUtilizados").show();
    }
    else {
        $("#knockoutSumarizadoFranquia").show();
        $("#dados-fechamento-periodo").show();
        $("#knockoutResumo").show();
        $("#knockoutValoresOutrosRecursosFechamento").hide();
        $("#knockoutVeiculosUtilizados").hide();
        $("#knockoutMotoristasUtilizados").hide();
    }
}

function buscarDadosParaFechamento(dados) {
    executarReST("FechamentoFrete/BuscarDadosParaFechamento", dados, function (arg) {
        _sumarizadoViagensRealizadas.VerHistorico.visible(false);
        if (arg.Success) {
            if (arg.Data) {
                GetSetCargasRemovidas([]);
                _sumarizadoViagensRealizadas.VerHistorico.visible(true);
                _gridHistoricoCargas.CarregarGrid();
                _CRUDFechamentoFrete.GerarFechamento.visible(true);
                preencherDadosSumarizados(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function carregarAcordosContrato() {
    executarReST("ContratoFreteTransportador/BuscarAcordosPorContrato", { Contrato: _dadosFechamento.Contrato.codEntity() }, function (arg) {
        if (arg.Success)
            _dadosFechamento.Acordos = arg.Data;
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            _dadosFechamento.Acordos = [];
        }
    });
}

function buscarDetalhesFranquia() {
    var dados = {
        Contrato: _dadosFechamento.Contrato.codEntity()
    };

    executarReST("FechamentoFrete/DetalhesContrato", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridDetalheContratoAcordos.CarregarGrid();
                PreencherObjetoKnout(_detalheContrato, arg);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function controlarCamposDadosFechamentoHabilitados(habilitar) {
    _dadosFechamento.Contrato.enable(habilitar);
    _dadosFechamento.PeriodoSemana.enable(habilitar);
    _dadosFechamento.PeriodoDezena.enable(habilitar);
    _dadosFechamento.PeriodoQuinzena.enable(habilitar);
    _dadosFechamento.PeriodoMes.enable(habilitar);
    _dadosFechamento.PeriodoAno.enable(habilitar);
}

function parametrosDadosFechamentoAlterados() {
    var dados = {
        Contrato: _dadosFechamento.Contrato.codEntity(),
        PeriodoSemana: _dadosFechamento.PeriodoSemana.val(),
        PeriodoDezena: _dadosFechamento.PeriodoDezena.val(),
        PeriodoQuinzena: _dadosFechamento.PeriodoQuinzena.val(),
        PeriodoMes: _dadosFechamento.PeriodoMes.val(),
        PeriodoAno: _dadosFechamento.PeriodoAno.val()
    }

    if (_fechamentoFrete.Codigo.val() == 0) {
        if (dados.Contrato != 0)
            buscarDadosParaFechamento(dados);
        else {
            _sumarizadoViagensRealizadas.VerHistorico.visible(false);
            _CRUDFechamentoFrete.GerarFechamento.visible(false);
        }
    }

    if (_dadosFechamento.EnumPeriodoAcordo.val() == EnumPeriodoAcordoContratoFreteTransportador.Semanal) {
        _dadosFechamento.PeriodoSemana.visible(true);
        _dadosFechamento.PeriodoDezena.visible(false);
        _dadosFechamento.PeriodoQuinzena.visible(false);

        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana1.visible(true);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana2.visible( _dadosFechamento.PeriodoSemana.val() == EnumSemana.Segunda || _dadosFechamento.PeriodoSemana.val() == EnumSemana.Terceira || _dadosFechamento.PeriodoSemana.val() == EnumSemana.Quarta);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana3.visible(_dadosFechamento.PeriodoSemana.val() == EnumSemana.Terceira || _dadosFechamento.PeriodoSemana.val() == EnumSemana.Quarta);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana4.visible(_dadosFechamento.PeriodoSemana.val() == EnumSemana.Quarta);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena1.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena2.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena3.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaQuinzena1.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaQuinzena2.visible(false);
    }
    else if (_dadosFechamento.EnumPeriodoAcordo.val() == EnumPeriodoAcordoContratoFreteTransportador.Decendial) {
        _dadosFechamento.PeriodoSemana.visible(false);
        _dadosFechamento.PeriodoDezena.visible(true);
        _dadosFechamento.PeriodoQuinzena.visible(false);

        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana1.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana2.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana3.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana4.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena1.visible(true);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena2.visible((_dadosFechamento.PeriodoDezena.val() == EnumDezena.Segunda) || (_dadosFechamento.PeriodoDezena.val() == EnumDezena.Terceira));
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena3.visible(_dadosFechamento.PeriodoDezena.val() == EnumDezena.Terceira);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaQuinzena1.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaQuinzena2.visible(false);
    }
    else if (_dadosFechamento.EnumPeriodoAcordo.val() == EnumPeriodoAcordoContratoFreteTransportador.Quinzenal) {
        _dadosFechamento.PeriodoSemana.visible(false);
        _dadosFechamento.PeriodoDezena.visible(false);
        _dadosFechamento.PeriodoQuinzena.visible(true);

        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana1.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana2.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana3.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana4.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena1.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena2.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena3.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaQuinzena1.visible(true);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaQuinzena2.visible(_dadosFechamento.PeriodoQuinzena.val() == EnumQuinzena.Segunda);
    }
    else {
        _dadosFechamento.PeriodoSemana.visible(false);
        _dadosFechamento.PeriodoDezena.visible(false);
        _dadosFechamento.PeriodoQuinzena.visible(false);

        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana1.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana2.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana3.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaSemana4.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena1.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena2.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaDezena3.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaQuinzena1.visible(false);
        _sumarizadoFechamentoMensal.TotalJaPagoTabelaQuinzena2.visible(false);
    }
}

function retornoBuscaContrato(data) {
    _dadosFechamento.Contrato.codEntity(data.Codigo);
    _dadosFechamento.Contrato.val(data.Descricao);
    _dadosFechamento.Contrato.tipoEmissaoComplemento = data.TipoEmissaoComplemento;
    _dadosFechamento.TipoFechamento.val(data.PeriodoAcordo);
    _dadosFechamento.TipoFranquia.val(data.TipoFranquia);
    _dadosFechamento.EnumPeriodoAcordo.val(data.EnumPeriodoAcordo);
    _dadosFechamento.ValorPorMotorista.val(data.ValorPorMotorista);

    if (data.Codigo != 0) {
        _sumarizadoViagensRealizadas.VerContrato.visible(_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorKm);
        buscarDetalhesFranquia();
    }
    else {
        _sumarizadoViagensRealizadas.VerContrato.visible(false);
        LimparCampos(_detalheContrato);
    }

    parametrosDadosFechamentoAlterados();
    controlarInformacoesVisiveisPorContrato();
    carregarAcordosContrato();
}

// #endregion Funções Privadas
