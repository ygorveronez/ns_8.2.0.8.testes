/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Enumeradores/EnumMoedaCotacaoBancoCentral.js" />

// #region Objetos Globais do Arquivo

var _interesseCargaAtual;
var _interesseCargaComponenteFrete;

// #endregion Objetos Globais do Arquivo

// #region Classes

var InteresseCargaComponenteFrete = function () {
    var self = this;

    this.AbreviacaoDocumentoFiscal = PropertyEntity({});
    this.TipoComponenteFrete = PropertyEntity({});
    this.TipoValor = PropertyEntity({});
    this.DescontarValorTotalAReceber = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Codigo = PropertyEntity({});
    this.ValorComponente = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable("*Valor do Componente:"), required: true, getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorSugerido = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable("Valor Sugerido:"), getType: typesKnockout.decimal });
    this.Percentual = PropertyEntity({ val: ko.observable(""), def: "", text: "*Percentual:", required: false, getType: typesKnockout.decimal, visible: ko.observable(false), configDecimal: { precision: 3, allowZero: false, allowNegative: false } });

    this.Moeda = PropertyEntity({ enable: ko.observable(false), text: "Moeda:", visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, issue: 0 });
    this.ValorTotalMoeda = PropertyEntity({ enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira), text: "Valor em Moeda: ", def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorCotacaoMoeda = PropertyEntity({ enable: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira), text: "Cotação da Moeda: ", def: "1,0000000000", val: ko.observable("1,0000000000"), getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 10, allowZero: false, allowNegative: false } });

    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Componente de Frete:", required: true, idBtnSearch: guid(), enable: ko.observable(true) });

    this.CobrarOutroDocumento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cobrar valor em outro documento Fiscal?", enable: ko.observable(true), required: false, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });

    this.Adicionar = PropertyEntity({ eventClick: adicionarInteresseCargaComponenteFreteClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarInteresseCargaComponenteFreteClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirInteresseCargaComponenteFreteClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarInteresseCargaComponenteFreteClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });

    this.ValorTotalMoeda.val.subscribe(function () { converterValorMoedaInteresseCargaComponenteFrete(self); });
};

// #endregion Classes

// #region Funções de Inicialização

function loadInteresseCargaComponenteFrete() {
    _interesseCargaComponenteFrete = new InteresseCargaComponenteFrete();
    KoBindings(_interesseCargaComponenteFrete, "knockoutInteresseCargaComponentesFrete");

    new BuscarModeloDocumentoFiscal(_interesseCargaComponenteFrete.ModeloDocumentoFiscal, retornoConsultaInteresseCargaModeloDocumentoFiscal, null, true);
    new BuscarComponentesDeFrete(_interesseCargaComponenteFrete.ComponenteFrete, retornoConsultaInteresseCargaComponenteFrete);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarInteresseCargaComponenteFreteClick() {
    _interesseCargaComponenteFrete.ModeloDocumentoFiscal.required = _interesseCargaComponenteFrete.CobrarOutroDocumento.val();

    if (!ValidarCamposObrigatorios(_interesseCargaComponenteFrete)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    var componentesFrete = _interesseCargaAtual.ComponentesFrete.val();

    componentesFrete.push(obterInteresseCargaComponenteFreteSalvar());

    _interesseCargaAtual.ComponentesFrete.val(componentesFrete);
    _interesseCargaAtual.Grid.CarregarGrid(_interesseCargaAtual.ComponentesFrete.val());

    fecharModalInteresseCargaComponentesFrete();
}

function atualizarInteresseCargaComponenteFreteClick() {
    _interesseCargaComponenteFrete.ModeloDocumentoFiscal.required = _interesseCargaComponenteFrete.CobrarOutroDocumento.val();

    if (!ValidarCamposObrigatorios(_interesseCargaComponenteFrete)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    var componentesFrete = _interesseCargaAtual.ComponentesFrete.val();

    for (var i = 0; i < componentesFrete.length; i++) {
        if (_interesseCargaComponenteFrete.Codigo.val() == componentesFrete[i].Codigo) {
            componentesFrete.splice(i, 1, obterInteresseCargaComponenteFreteSalvar());
            break;
        }
    }

    _interesseCargaAtual.ComponentesFrete.val(componentesFrete);
    _interesseCargaAtual.Grid.CarregarGrid(_interesseCargaAtual.ComponentesFrete.val());

    fecharModalInteresseCargaComponentesFrete();
}

function cancelarInteresseCargaComponenteFreteClick() {
    fecharModalInteresseCargaComponentesFrete();
}

function excluirInteresseCargaComponenteFreteClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o componente " + registroSelecionado.ComponenteFrete.val() + "?", function () {
        var componentesFrete = _interesseCargaAtual.ComponentesFrete.val();

        for (var i = 0; i < componentesFrete.length; i++) {
            if (registroSelecionado.Codigo.val() == componentesFrete[i].Codigo) {
                componentesFrete.splice(i, 1);
                break;
            }
        }

        _interesseCargaAtual.ComponentesFrete.val(componentesFrete);
        _interesseCargaAtual.Grid.CarregarGrid(_interesseCargaAtual.ComponentesFrete.val());

        fecharModalInteresseCargaComponentesFrete();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function adicionarInteresseCargaComponenteFrete(interesseCarga) {
    _interesseCargaAtual = interesseCarga;

    limparCamposinteresseCargaComponenteFrete();

    _interesseCargaComponenteFrete.Codigo.val(guid());

    exibirModalInteresseCargaComponentesFrete();
}

function editarInteresseCargaComponenteFrete(interesseCarga, registroSelecionado) {
    _interesseCargaAtual = interesseCarga;

    limparCamposinteresseCargaComponenteFrete();

    if (registroSelecionado.TipoValor === EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal)
        definirPadraointeresseCargaComponenteFretePercentual();
    else
        definirPadraointeresseCargaComponenteFreteValores();

    var moeda = _interesseCargaAtual.Moeda.val();
    var cotacaoMoeda = _interesseCargaAtual.ValorCotacaoMoeda.val();

    if (moeda !== null && moeda !== EnumMoedaCotacaoBancoCentral.Real && registroSelecionado.TipoValor !== EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal) {
        _interesseCargaComponenteFrete.Moeda.val(moeda);
        _interesseCargaComponenteFrete.ValorCotacaoMoeda.val(cotacaoMoeda);
        _interesseCargaComponenteFrete.ValorTotalMoeda.visible(true);
        _interesseCargaComponenteFrete.Moeda.visible(true);
        _interesseCargaComponenteFrete.ValorCotacaoMoeda.visible(true);
        _interesseCargaComponenteFrete.ValorComponente.enable(false);
        _interesseCargaComponenteFrete.ModeloDocumentoFiscal.visible(false);
    }
    else {
        _interesseCargaComponenteFrete.ValorTotalMoeda.visible(false);
        _interesseCargaComponenteFrete.Moeda.visible(false);
        _interesseCargaComponenteFrete.ValorCotacaoMoeda.visible(false);
    }

    if (!registroSelecionado.DescontarValorTotalAReceber) {
        _interesseCargaComponenteFrete.ValorComponente.val(registroSelecionado.Valor);
        _interesseCargaComponenteFrete.ValorComponente.text("*Valor do Componente:");
    }
    else {
        _interesseCargaComponenteFrete.ValorComponente.text("*Valor de Desconto:");
        _interesseCargaComponenteFrete.ValorComponente.val(registroSelecionado.Valor.replace("-", ""));
    }

    _interesseCargaComponenteFrete.Codigo.val(registroSelecionado.Codigo);
    _interesseCargaComponenteFrete.ValorTotalMoeda.val(registroSelecionado.ValorTotalMoeda);
    _interesseCargaComponenteFrete.ValorSugerido.val(registroSelecionado.ValorSugerido);
    _interesseCargaComponenteFrete.ComponenteFrete.val(registroSelecionado.DescricaoComponente);
    _interesseCargaComponenteFrete.ComponenteFrete.codEntity(registroSelecionado.ComponenteFrete);
    _interesseCargaComponenteFrete.Percentual.val(registroSelecionado.Percentual);

    _interesseCargaComponenteFrete.ModeloDocumentoFiscal.val(registroSelecionado.DescricaoModeloDocumentoFiscal);
    _interesseCargaComponenteFrete.ModeloDocumentoFiscal.codEntity(registroSelecionado.CodigoModeloDocumentoFiscal);
    _interesseCargaComponenteFrete.CobrarOutroDocumento.val(registroSelecionado.CobrarOutroDocumento);

    if (_interesseCargaAtual.CobrarOutroDocumento.val()) {
        _interesseCargaComponenteFrete.CobrarOutroDocumento.val(true);
        _interesseCargaComponenteFrete.CobrarOutroDocumento.enable(false);
    }
    else
        _interesseCargaComponenteFrete.CobrarOutroDocumento.enable(true);

    _interesseCargaComponenteFrete.Atualizar.visible(true);
    _interesseCargaComponenteFrete.Cancelar.visible(true);
    _interesseCargaComponenteFrete.Excluir.visible(true);
    _interesseCargaComponenteFrete.Adicionar.visible(false);

    if (_interesseCargaAtual.TipoFreteEscolhido.val() != EnumTipoFreteEscolhido.Embarcador && (registroSelecionado.TipoComponenteFrete == EnumTipoComponenteFrete.ICMS || registroSelecionado.TipoComponenteFrete == EnumTipoComponenteFrete.ISS)) {
        _interesseCargaComponenteFrete.Atualizar.visible(false);
        _interesseCargaComponenteFrete.Excluir.visible(false);
        _interesseCargaComponenteFrete.ModeloDocumentoFiscal.visible(false);
        _interesseCargaComponenteFrete.Percentual.visible(false);
        _interesseCargaComponenteFrete.ValorComponente.enable(false);
        _interesseCargaComponenteFrete.ComponenteFrete.enable(false);
    }

    if (_interesseCargaAtual.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Embarcador) {
        _interesseCargaComponenteFrete.Excluir.visible(false);
        _interesseCargaComponenteFrete.ModeloDocumentoFiscal.visible(false);
        _interesseCargaComponenteFrete.Percentual.visible(false);
        _interesseCargaComponenteFrete.ValorComponente.enable(false);
    }

    exibirModalInteresseCargaComponentesFrete();
}

// #endregion Funções Públicas

// #region Funções Privadas

function converterValorMoedaInteresseCargaComponenteFrete(e) {
    var valorCotacaoMoeda = 0;
    if (e.ValorCotacaoMoeda != null && e.ValorCotacaoMoeda != undefined && e.ValorCotacaoMoeda.val() != null && e.ValorCotacaoMoeda.val() != undefined)
        valorCotacaoMoeda = Globalize.parseFloat(e.ValorCotacaoMoeda.val());
    var valorTotalMoeda = 0;
    if (e.ValorTotalMoeda != null && e.ValorTotalMoeda != undefined && e.ValorTotalMoeda.val() != null && e.ValorTotalMoeda.val() != undefined)
        valorTotalMoeda = Globalize.parseFloat(e.ValorTotalMoeda.val());

    if (isNaN(valorCotacaoMoeda))
        valorCotacaoMoeda = 0;
    if (isNaN(valorTotalMoeda))
        valorTotalMoeda = 0;

    var valorTotalConvertido = valorCotacaoMoeda * valorTotalMoeda;

    if (valorTotalConvertido > 0)
        e.ValorComponente.val(Globalize.format(valorTotalConvertido, "n2"));
    else
        e.ValorComponente.val("");
}

function definirPadraointeresseCargaComponenteFretePercentual() {
    _interesseCargaComponenteFrete.ValorComponente.visible(false);
    _interesseCargaComponenteFrete.Percentual.visible(true);
    _interesseCargaComponenteFrete.Percentual.required = true;
    _interesseCargaComponenteFrete.ValorComponente.required = false;
    _interesseCargaComponenteFrete.Percentual.val("");
    _interesseCargaComponenteFrete.ValorComponente.val("0,00");
}

function definirPadraointeresseCargaComponenteFreteValores() {
    _interesseCargaComponenteFrete.ValorComponente.visible(true);
    _interesseCargaComponenteFrete.Percentual.visible(false);
    _interesseCargaComponenteFrete.Percentual.required = false;
    _interesseCargaComponenteFrete.ValorComponente.required = true;
    _interesseCargaComponenteFrete.Percentual.val("0,000");
    _interesseCargaComponenteFrete.ValorComponente.val("");
}

function exibirModalInteresseCargaComponentesFrete() {    
    Global.abrirModal('divModalInteresseCargaComponentesFrete');
}

function fecharModalInteresseCargaComponentesFrete() {
    Global.fecharModal('divModalInteresseCargaComponentesFrete');
}

function limparCamposinteresseCargaComponenteFrete(e) {
    _interesseCargaComponenteFrete.Atualizar.visible(false);
    _interesseCargaComponenteFrete.Cancelar.visible(false);
    _interesseCargaComponenteFrete.Excluir.visible(false);
    _interesseCargaComponenteFrete.Adicionar.visible(true);

    definirPadraointeresseCargaComponenteFreteValores();

    _interesseCargaComponenteFrete.ModeloDocumentoFiscal.required = false;

    LimparCampos(_interesseCargaComponenteFrete);

    var moeda = _interesseCargaAtual.Moeda.val();
    var cotacaoMoeda = _interesseCargaAtual.ValorCotacaoMoeda.val();

    if (moeda !== null && moeda !== EnumMoedaCotacaoBancoCentral.Real) {
        _interesseCargaComponenteFrete.Moeda.val(moeda);
        _interesseCargaComponenteFrete.ValorCotacaoMoeda.val(cotacaoMoeda);
        _interesseCargaComponenteFrete.ValorTotalMoeda.visible(true);
        _interesseCargaComponenteFrete.Moeda.visible(true);
        _interesseCargaComponenteFrete.ValorCotacaoMoeda.visible(true);
        _interesseCargaComponenteFrete.ValorComponente.enable(false);
        _interesseCargaComponenteFrete.ModeloDocumentoFiscal.visible(false);
    }
    else {
        _interesseCargaComponenteFrete.ModeloDocumentoFiscal.visible(true);
        _interesseCargaComponenteFrete.ModeloDocumentoFiscal.enable(true);
        _interesseCargaComponenteFrete.ValorComponente.enable(true);
        _interesseCargaComponenteFrete.ValorTotalMoeda.visible(false);
        _interesseCargaComponenteFrete.Moeda.visible(false);
        _interesseCargaComponenteFrete.ValorCotacaoMoeda.visible(false);
    }

    _interesseCargaComponenteFrete.ComponenteFrete.enable(true);

    if (_interesseCargaAtual.CobrarOutroDocumento.val()) {
        _interesseCargaComponenteFrete.CobrarOutroDocumento.val(true);
        _interesseCargaComponenteFrete.ModeloDocumentoFiscal.val(_interesseCargaAtual.ModeloDocumentoFiscal.val());
        _interesseCargaComponenteFrete.ModeloDocumentoFiscal.codEntity(_interesseCargaAtual.ModeloDocumentoFiscal.codEntity());
        _interesseCargaComponenteFrete.CobrarOutroDocumento.enable(false);
    }
    else {
        _interesseCargaComponenteFrete.CobrarOutroDocumento.enable(true);
        _interesseCargaComponenteFrete.CobrarOutroDocumento.val(false);
        _interesseCargaComponenteFrete.ModeloDocumentoFiscal.val("");
        _interesseCargaComponenteFrete.ModeloDocumentoFiscal.codEntity(0);
    }
}

function obterInteresseCargaComponenteFreteSalvar() {
    return {
        AbreviacaoDocumentoFiscal: _interesseCargaComponenteFrete.AbreviacaoDocumentoFiscal.val() || "CT-e",
        CobrarOutroDocumento: _interesseCargaComponenteFrete.CobrarOutroDocumento.val(),
        Codigo: _interesseCargaComponenteFrete.Codigo.val(),
        CodigoModeloDocumentoFiscal: _interesseCargaComponenteFrete.ModeloDocumentoFiscal.codEntity(),
        ComponenteFrete: _interesseCargaComponenteFrete.ComponenteFrete.codEntity(),
        DescontarValorTotalAReceber: _interesseCargaComponenteFrete.DescontarValorTotalAReceber.val(),
        Descricao: _interesseCargaComponenteFrete.ComponenteFrete.val(),
        DescricaoComponente: _interesseCargaComponenteFrete.ComponenteFrete.val(),
        DescricaoModeloDocumentoFiscal: _interesseCargaComponenteFrete.ModeloDocumentoFiscal.val(),
        Percentual: _interesseCargaComponenteFrete.Percentual.val(),
        TipoComponenteFrete: _interesseCargaComponenteFrete.TipoComponenteFrete.val(),
        TipoValor: _interesseCargaComponenteFrete.TipoValor.val(),
        Valor: _interesseCargaComponenteFrete.ValorComponente.val(),
        ValorSugerido: _interesseCargaComponenteFrete.ValorSugerido.val(),
        ValorTotalMoeda: _interesseCargaComponenteFrete.ValorTotalMoeda.val()
    };
}

function obterSugestaoValorInteresseCargaComponenteFreteCalculado(codigoComponente, codigoCarga) {
    executarReST("CargaComponenteFrete/ObterValorComponenteFreteCalculado", { ComponenteFrete: codigoComponente, Carga: codigoCarga }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _interesseCargaComponenteFrete.ValorComponente.val(retorno.Data.Valor);
                _interesseCargaComponenteFrete.ValorSugerido.val(retorno.Data.Valor);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function retornoConsultaInteresseCargaComponenteFrete(registroSelecionado) {
    _interesseCargaComponenteFrete.ComponenteFrete.val(registroSelecionado.Descricao);
    _interesseCargaComponenteFrete.ComponenteFrete.codEntity(registroSelecionado.Codigo);
    _interesseCargaComponenteFrete.TipoComponenteFrete.val(registroSelecionado.TipoComponenteFrete);
    _interesseCargaComponenteFrete.TipoValor.val(registroSelecionado.TipoValor);
    _interesseCargaComponenteFrete.DescontarValorTotalAReceber.val(registroSelecionado.DescontarValorTotalAReceber);

    if (_interesseCargaAtual.TipoFreteEscolhido.val() !== EnumTipoFreteEscolhido.Embarcador) {
        var moeda = _interesseCargaAtual.Moeda.val();

        if (registroSelecionado.TipoValor === EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal) {

            if (moeda !== null && moeda !== EnumMoedaCotacaoBancoCentral.Real) {
                _interesseCargaComponenteFrete.ValorTotalMoeda.visible(false);
                _interesseCargaComponenteFrete.Moeda.visible(false);
                _interesseCargaComponenteFrete.ValorCotacaoMoeda.visible(false);
            }

            definirPadraointeresseCargaComponenteFretePercentual();
        }
        else {
            if (moeda !== null && moeda !== EnumMoedaCotacaoBancoCentral.Real) {
                _interesseCargaComponenteFrete.ValorTotalMoeda.visible(true);
                _interesseCargaComponenteFrete.Moeda.visible(true);
                _interesseCargaComponenteFrete.ValorCotacaoMoeda.visible(true);
            }

            definirPadraointeresseCargaComponenteFreteValores();

            obterSugestaoValorInteresseCargaComponenteFreteCalculado(registroSelecionado.Codigo, _interesseCargaAtual.CodigoCarga.val());
        }

        if (!registroSelecionado.DescontarValorTotalAReceber)
            _interesseCargaComponenteFrete.ValorComponente.text("*Valor do Componente:");
        else
            _interesseCargaComponenteFrete.ValorComponente.text("*Valor de Desconto:");
    }
}

function retornoConsultaInteresseCargaModeloDocumentoFiscal(registroSelecionado) {
    _interesseCargaComponenteFrete.ModeloDocumentoFiscal.val(registroSelecionado.Descricao);
    _interesseCargaComponenteFrete.ModeloDocumentoFiscal.codEntity(registroSelecionado.Codigo);
    _interesseCargaComponenteFrete.AbreviacaoDocumentoFiscal.val(registroSelecionado.Abreviacao);
}

// #endregion Funções Privadas
