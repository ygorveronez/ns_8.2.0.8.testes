/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="MotoristasUtilizados.js" />
/// <reference path="VeiculosUtilizados.js" />

// #region Objetos Globais do Arquivo

var _sumarizadoViagensRealizadas;
var _sumarizadoFranquia;
var _sumarizadoFranquiaPorFaixaKm;
var _sumarizadoFechamentoMensal;
var _sumarizadoFechamentoPeriodo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var SumarizadoViagensRealizadas = function () {
    var self = this;

    this.TotalViagens = PropertyEntity({ text: "Total de Viagens", val: ko.observable(0), def: 0 });
    this.ValorTotalPagoTabela = PropertyEntity({ text: "Valor total pago por Tabela", val: ko.observable(0), def: 0 });
    this.AdicionalKM = PropertyEntity({ text: "Adicional por KM", val: ko.observable(0), def: 0, visible: ko.observable(_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorKm) });

    this.ValorMedioPagoTabela = PropertyEntity({
        type: types.local,
        text: "Valor médio pago por Tabela", val: ko.computed(function () {
            if (self.TotalViagens.val() > 0 && self.ValorTotalPagoTabela.val() > 0)
                return self.ValorTotalPagoTabela.val() / self.TotalViagens.val();
            else
                return 0;
        }), def: 0
    });
    this.ValorMedioPagoTabela.formatted = ko.computed(formatarCampoValor(this.ValorMedioPagoTabela));

    this.ValorTotalPagoTabela.formatted = ko.computed(formatarCampoValor(this.ValorTotalPagoTabela));
    this.AdicionalKM.formatted = ko.computed(formatarCampoValor(this.AdicionalKM));

    this.VerContrato = PropertyEntity({ text: "Ver Contrato", val: ko.observable(""), visible: ko.observable(false), eventClick: verContratoClick, type: types.event });
    this.VerHistorico = PropertyEntity({ text: "Ver Histórico", val: ko.observable(""), visible: ko.observable(false), eventClick: verHistoricoClick, type: types.event });
}

var SumarizadoFranquia = function () {
    var self = this;

    this.TotalKMExcedido = PropertyEntity({ text: "Total KM Excedido", val: ko.observable(0), def: 0 });
    this.TotalKMFranquia = PropertyEntity({ text: "Total KM Franquia", val: ko.observable(0), def: 0 });
    this.TotalKMRealizado = PropertyEntity({ text: "Total KM Realizado", val: ko.observable(0), def: 0 });
    this.ValorKMExcedido = PropertyEntity({ text: "Total Excedido", val: ko.observable(0), def: 0 });
    this.ValorKMFranquia = PropertyEntity({ text: "Valor Franquia", val: ko.observable(0), def: 0 });
    this.ValorTotalKMFranquia = PropertyEntity({ text: "Valor Total KM Franquia", val: ko.observable(0), def: 0 });

    this.ValorTotalKMExcedido = PropertyEntity({
        text: "Valor Total KM Excedido", type: types.local, val: ko.computed(function () {
            return self.TotalKMExcedido.val() * self.ValorKMExcedido.val();
        })
    });

    this.Total = PropertyEntity({
        type: types.local,
        val: ko.computed(function () {
            return self.ValorTotalKMFranquia.val() + self.ValorTotalKMExcedido.val();
        })
    });

    this.Total.formatted = ko.computed(formatarCampoValor(this.Total));
    this.ValorKMExcedido.formatted = ko.computed(formatarCampoValor(this.ValorKMExcedido));
    this.ValorKMFranquia.formatted = ko.computed(formatarCampoValor(this.ValorKMFranquia, "n6"));
    this.ValorTotalKMExcedido.formatted = ko.computed(formatarCampoValor(this.ValorTotalKMExcedido));
    this.ValorTotalKMFranquia.formatted = ko.computed(formatarCampoValor(this.ValorTotalKMFranquia));
}

var SumarizadoFranquiaPorFaixaKm = function () {
    var self = this;

    this.TotalKMRealizado = PropertyEntity({ text: "Total KM Realizado", val: ko.observable(0), def: 0 });
    this.ValorDiferencaTotalCargas = PropertyEntity({ text: "Valor Diferença", val: ko.observable(0), def: 0 });
    this.ValorTotalCargas = PropertyEntity({ text: "Valor Cargas", val: ko.observable(0), def: 0 });
    this.ValorFranquia = PropertyEntity({ text: "Valor Franquia", val: ko.observable(0), def: 0 });
    this.ValorFranquiaPorKm = PropertyEntity({ text: "Valor KM Franquia", val: ko.observable(0), def: 0 });
    this.ValorTotalPorFaixaKm = PropertyEntity({ text: "Total por Faixa KM", val: ko.observable(0), def: 0 });

    this.Total = PropertyEntity({
        type: types.local,
        val: ko.computed(function () {
            return self.ValorDiferencaTotalCargas.val() + self.ValorTotalPorFaixaKm.val();
        })
    });

    this.Total.formatted = ko.computed(formatarCampoValor(this.Total));
    this.ValorFranquia.formatted = ko.computed(formatarCampoValor(this.ValorFranquia));
    this.ValorFranquiaPorKm.formatted = ko.computed(formatarCampoValor(this.ValorFranquiaPorKm, "n6"));
    this.ValorDiferencaTotalCargas.formatted = ko.computed(formatarCampoValor(this.ValorDiferencaTotalCargas));
    this.ValorTotalCargas.formatted = ko.computed(formatarCampoValor(this.ValorTotalCargas));
    this.ValorTotalPorFaixaKm.formatted = ko.computed(formatarCampoValor(this.ValorTotalPorFaixaKm));
    this.TotalKMRealizado.formatted = ko.computed(formatarCampoKm(this.TotalKMRealizado));
}

var SumarizadoFechamentoMensal = function () {
    var self = this;
    
    this.FranquiaExcedente = PropertyEntity({ text: "Franquia Excedente", val: ko.observable(0), def: 0 });
    this.TotalFranquia = PropertyEntity({ text: "Total Acordado/Franquia", val: ko.observable(0), def: 0 });
    this.TotalJaPagoTabela = PropertyEntity({ text: "Total Pago Mensal", val: ko.observable(0), def: 0 });
    this.TotalJaPagoTabelaSemana1 = PropertyEntity({ text: "Total já Pago por Tabela 1ª Semana", val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.TotalJaPagoTabelaSemana2 = PropertyEntity({ text: "Total já Pago por Tabela 2ª Semana", val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.TotalJaPagoTabelaSemana3 = PropertyEntity({ text: "Total já Pago por Tabela 3ª Semana", val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.TotalJaPagoTabelaSemana4 = PropertyEntity({ text: "Total já Pago por Tabela 4ª Semana", val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.TotalJaPagoTabelaDezena1 = PropertyEntity({ text: "Total já Pago por Tabela 1ª Dezena", val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.TotalJaPagoTabelaDezena2 = PropertyEntity({ text: "Total já Pago por Tabela 2ª Dezena", val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.TotalJaPagoTabelaDezena3 = PropertyEntity({ text: "Total já Pago por Tabela 3ª Dezena", val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.TotalJaPagoTabelaQuinzena1 = PropertyEntity({ text: "Total já Pago por Tabela 1ª Quinzena", val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.TotalJaPagoTabelaQuinzena2 = PropertyEntity({ text: "Total já Pago por Tabela 2ª Quinzena", val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.FixoDiaria = PropertyEntity({ text: "Valor Diária/Veículo", val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.TotalVeiculos = PropertyEntity({ text: "Quantidade de Veículos", val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.TotalDias = PropertyEntity({ text: "Total Dias Rodados", val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.TotalFixoDiaria = PropertyEntity({ text: "Total Valor Fixo/Diária", val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.Diferenca = PropertyEntity({
        text: "Diferença", type: types.local, val: ko.computed(function () {
            var valorDiferenca = self.TotalFranquia.val() - self.TotalJaPagoTabela.val();

            return valorDiferenca > 0 ? valorDiferenca: 0;
        })
    });
  
    this.FranquiaExcedente.formatted = ko.computed(formatarCampoValor(this.FranquiaExcedente));
    this.TotalFranquia.formatted = ko.computed(formatarCampoValor(this.TotalFranquia));
    this.TotalJaPagoTabela.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabela));
    this.TotalJaPagoTabelaSemana1.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabelaSemana1));
    this.TotalJaPagoTabelaSemana2.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabelaSemana2));
    this.TotalJaPagoTabelaSemana3.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabelaSemana3));
    this.TotalJaPagoTabelaSemana4.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabelaSemana4));
    this.TotalJaPagoTabelaDezena1.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabelaDezena1));
    this.TotalJaPagoTabelaDezena2.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabelaDezena2));
    this.TotalJaPagoTabelaDezena3.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabelaDezena3));
    this.TotalJaPagoTabelaQuinzena1.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabelaQuinzena1));
    this.TotalJaPagoTabelaQuinzena2.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabelaQuinzena2));
    this.FixoDiaria.formatted = ko.computed(formatarCampoValor(this.FixoDiaria));
    this.TotalFixoDiaria.formatted = ko.computed(formatarCampoValor(this.TotalFixoDiaria));
    this.Diferenca.formatted = ko.computed(formatarCampoValor(this.Diferenca));
}

var SumarizadoFechamentoPeriodo = function () {
    var self = this;

    this.TotalJaPagoTabela = PropertyEntity({ text: "Total já pago por Tabela", val: ko.observable(0), def: 0 });
    this.TotalFranquia = PropertyEntity({ text: "Total Acordado/Franquia", val: ko.observable(0), def: 0 });
    this.FixoDiaria = PropertyEntity({ text: "Valor Diária/Veículo", val: ko.observable(0), def: 0, visible: ko.observable(true) });

    this.TotalJaPagoTabela.formatted = ko.computed(formatarCampoValor(this.TotalJaPagoTabela));
    this.TotalFranquia.formatted = ko.computed(formatarCampoValor(this.TotalFranquia));
    this.FixoDiaria.formatted = ko.computed(formatarCampoValor(this.FixoDiaria));

    this.Diferenca = PropertyEntity({
        text: "Diferença", type: types.local, val: ko.computed(function () {
            var valorDiferenca = self.TotalFranquia.val() - self.TotalJaPagoTabela.val();

            return valorDiferenca > 0 ? valorDiferenca : 0;
        })
    });
    this.Diferenca.formatted = ko.computed(formatarCampoValor(this.Diferenca));
}

// #endregion Classes

// #region Funções de Inicialização

function loadSumarizados() {
    _sumarizadoViagensRealizadas = new SumarizadoViagensRealizadas();
    KoBindings(_sumarizadoViagensRealizadas, "knockoutSumarizadoViagensRealizadas");

    _sumarizadoFranquia = new SumarizadoFranquia();
    KoBindings(_sumarizadoFranquia, "knockoutSumarizadoFranquia");

    _sumarizadoFranquiaPorFaixaKm = new SumarizadoFranquiaPorFaixaKm();
    KoBindings(_sumarizadoFranquiaPorFaixaKm, "knockoutSumarizadoFranquiaPorFaixaKm");

    _sumarizadoFechamentoMensal = new SumarizadoFechamentoMensal();
    KoBindings(_sumarizadoFechamentoMensal, "knockoutSumarizadoFechamentoMensal");

    _sumarizadoFechamentoPeriodo = new SumarizadoFechamentoPeriodo();
    KoBindings(_sumarizadoFechamentoPeriodo, "knockoutSumarizadoFechamentoPeriodo");

    loadVeiculosUtilizados();
    loadMotoristasUtilizados();
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function limparSumarizados() {
    LimparCampos(_sumarizadoViagensRealizadas);
    LimparCampos(_sumarizadoFranquia);
    LimparCampos(_sumarizadoFranquiaPorFaixaKm);
    LimparCampos(_sumarizadoFechamentoMensal);
    LimparCampos(_sumarizadoFechamentoPeriodo);
    limparCamposVeiculosUtilizados();
    limparCamposMotoristasUtilizados();

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

function preencherDadosSumarizados(dadosFechamento) {
    PreencherObjetoKnout(_sumarizadoViagensRealizadas, { Data: dadosFechamento.SumarizadoViagensRealizadas });
    PreencherObjetoKnout(_sumarizadoFranquia, { Data: dadosFechamento.SumarizadoFranquia });
    PreencherObjetoKnout(_sumarizadoFranquiaPorFaixaKm, { Data: dadosFechamento.SumarizadoFranquiaPorFaixaKm });
    PreencherObjetoKnout(_sumarizadoFechamentoMensal, { Data: dadosFechamento.SumarizadoFechamentoMensal });
    PreencherObjetoKnout(_sumarizadoFechamentoPeriodo, { Data: dadosFechamento.SumarizadoFechamentoPeriodo });
    preencherVeiculosUtilizados(dadosFechamento.VeiculosUtilizados);
    preencherMotoristasUtilizados(dadosFechamento.MotoristasUtilizados);
}

// #endregion Funções Públicas

// #region Funções Privadas

function formatarCampoQuantidade(ko) {
    return function () {
        return Globalize.format(ko.val(), "n0");
    }
}

function formatarCampoKm(ko) {
    return function () {
        return Globalize.format(ko.val(), "n2") + " KM";
    }
}

function formatarCampoValor(ko, format) {
    return function () {
        return "R$ " + Globalize.format(ko.val(), format || "n2");
    }
}

// #endregion Funções Privadas
