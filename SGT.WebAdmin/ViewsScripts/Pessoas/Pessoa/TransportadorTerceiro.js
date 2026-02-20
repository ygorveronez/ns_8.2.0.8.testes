/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="Pessoa.js" />
/// <reference path="AcrescimoDescontoAutomatico.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoProprietarioVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoQuitacaoCIOT.js" />
/// <reference path="../../Enumeradores/EnumTipoGeracaoCIOT.js" />
/// <reference path="../../Enumeradores/EnumFormaAberturaCIOTPeriodo.js" />
/// <reference path="../../Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../Consultas/TipoTerceiro.js" />
/// <reference path="../../Consultas/ConfiguracaoCIOT.js" />

var _transportadorTerceiro;
var _gridTiposPagamentoCIOT;
var _gridDiasFechamentoCIOTPeriodo;
//*******MAPEAMENTO KNOUCKOUT*******
var TipoPagamentoCIOTMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.OperadoraCIOT = PropertyEntity({ type: types.map, val: "" });
    this.TipoPagamentoCIOT = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoTipoPagamentoCIOT = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoOperadoraCIOT = PropertyEntity({ type: types.map, val: "" });
};

var DiaFechamentoCIOTMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.DiaFechamentoCIOT = PropertyEntity({ type: types.map, val: "" });
};

var TransportadorTerceiro = function () {
    this.DescontoPadrao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.PorcentagemDescontoQuandoSubcontrataComoTerceiroEsseTransportador.getFieldDescription(), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: true }, maxlength: 7, visible: ko.observable(true), val: ko.observable("0,0000") });
    this.PercentualCobradoPadrao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.PorcentagemCobradoSobreValorDoCTeQuandoSubcontratadoPorEsseTransportador.getFieldDescription(), required: false, getType: typesKnockout.decimal, maxlength: 6, visible: ko.observable(true) });
    this.RNTRC = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.RNTRC.getRequiredFieldDescription()), required: true, maxlength: 8 });
    this.PercentualAdiantamentoFretesTerceiro = PropertyEntity({
        text: ko.observable(Localization.Resources.Pessoas.Pessoa.PorcentagemDeAdiantamentoNoValorDoFreteQuandoSubcontrataComoTerceiroEsseTransportador.getFieldDescription()),
        visible: ko.observable(true), required: ko.observable(false),
        getType: typesKnockout.decimal,
        maxlength: 6, configDecimal: { precision: 2, allowZero: true }, val: ko.observable("0,00")
    });
    this.GerarPagamentoTerceiro = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.GerarPagamento, def: false, visible: ko.observable(true) });
    this.PagamentoMotoristaTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.TipoPagamentoMotorista, idBtnSearch: guid(), visible: ko.observable(true) });

    this.PercentualAbastecimentoFretesTerceiro = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.PorcentagemDeAbastecimentoNoValorDoFreteQuandoSubcontrataComoTerceiroEsseTransportador.getFieldDescription(), visible: ko.observable(true), required: false, getType: typesKnockout.decimal, maxlength: 6 });
    this.GerarCIOT = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.GerarCIOT, def: false, visible: ko.observable(true) });

    this.ExigeCanhotoFechamentoContratoFrete = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.ExigirCanhotosParaFinalizacaoDoContratoDeFrete, def: true, visible: ko.observable(true) });
    this.HabilitarDataFixaVencimento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.HabilitarDataFixaVencimento, def: false, visible: ko.observable(true) });
    this.ObservacaoCTe = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ObservacaoParaCTe.getFieldDescription(), required: false, maxlength: 400, visible: ko.observable(true), issue: 149 });
    this.TipoTransportadorTerceiro = PropertyEntity({ val: ko.observable(EnumTipoProprietarioVeiculo.TACAgregado), options: EnumTipoProprietarioVeiculo.obterOpcoes(), def: EnumTipoProprietarioVeiculo.TACAgregado, text: Localization.Resources.Pessoas.Pessoa.TipoProprietario.getRequiredFieldDescription(), required: true });
    this.DataEmissaoRNTRC = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.date, text: Localization.Resources.Pessoas.Pessoa.DataEmissaoRNTRC.getFieldDescription() });
    this.DataVencimentoRNTRC = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.date, text: Localization.Resources.Pessoas.Pessoa.DataVencimentoRNTRC.getFieldDescription() });
    this.CodigoINSS = PropertyEntity({ text: ko.observable(_CONFIGURACAO_TMS.TornarCampoINSSeReterImpostoTrazerComoSim ? Localization.Resources.Pessoas.Pessoa.CodigoINSS.getRequiredFieldDescription() : Localization.Resources.Pessoas.Pessoa.CodigoINSS.getFieldDescription()), required: ko.observable(_CONFIGURACAO_TMS.TornarCampoINSSeReterImpostoTrazerComoSim ? true : false), maxlength: 20 });
    this.NumeroCartao = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.NumeroDeCartao.getFieldDescription()), maxlength: 50 });
    this.TipoFavorecidoCIOT = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoDoFavorecido.getFieldDescription(), options: EnumTipoFavorecidoCIOT.ObterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.TipoPagamentoCIOT = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoDoPagamento.getFieldDescription(), options: EnumTipoPagamentoCIOT.ObterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: false });
    this.TipoGeracaoCIOT = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoDeCIOT.getFieldDescription(), options: EnumTipoGeracaoCIOT.ObterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.FormaAberturaCIOTPeriodo = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.FormaAberturaCIOTPeriodo.getFieldDescription(), options: EnumFormaAberturaCIOTPeriodo.ObterOpcoes(), val: ko.observable(EnumFormaAberturaCIOTPeriodo.Manual), def: EnumFormaAberturaCIOTPeriodo.Manual, issue: 0, visible: ko.observable(false) });

    this.TipoQuitacaoCIOT = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoDeQuitacaoCIOT.getFieldDescription(), options: EnumTipoQuitacaoCIOT.ObterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.TipoAdiantamentoCIOT = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoDeAdiantamentoCIOT.getFieldDescription(), options: EnumTipoQuitacaoCIOT.ObterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });


    this.TiposPagamentoCIOTOperadora = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });
    this.TipoPagamentoCIOTOperadora = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoDoPagamento.getFieldDescription(), options: ko.observable(EnumTipoPagamentoCIOT.ObterOpcoes()), val: ko.observable(EnumTipoPagamentoCIOT.SemPgto), def: "", issue: 0, visible: ko.observable(true), codEntity: ko.observable("") });
    this.OperadoraTipoPagamentoCIOTOperadora = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.OperadoraDeCIOT, options: EnumOperadoraCIOT.ObterOpcoes(), val: ko.observable(EnumOperadoraCIOT.eFrete), def: EnumOperadoraCIOT.eFrete, visible: ko.observable(true), codEntity: ko.observable("") });
    this.AdicionarTipoPagamentoCIOT = PropertyEntity({ eventClick: adicionarTipoPagamentoCIOTClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.AdicionarTipoPagamentoCIOT, visible: ko.observable(true) });

    this.OperadoraTipoPagamentoCIOTOperadora.val.subscribe(function (operadora) {
        _transportadorTerceiro.TipoPagamentoCIOTOperadora.options(EnumTipoPagamentoCIOT.obterOpcoesPorOperadora(operadora));

    });

    this.DiasFechamentoCIOTPeriodo = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });
    this.DiaFechamentoCIOT = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DiaFechamento.getFieldDescription(), enable: ko.observable(true), getType: typesKnockout.int, maxlength: 2, visible: ko.observable(true) });
    this.AdicionarDiaFechamentoCIOTPeriodo = PropertyEntity({ eventClick: adicionarDiaFechamentoCIOTPeriodoClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.AdicionarDiaFechamento, visible: ko.observable(true) });

    this.DiaFechamentoCIOT.val.subscribe(function (novoValor) {
        const numero = parseInt(novoValor, 10);

        if (!isNaN(numero) && numero > 30) {
            _transportadorTerceiro.DiaFechamentoCIOT.val("30");
        }
    });

    this.TagPlaca = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoCTe.id, "#Placa"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.Placa });
    this.TagRENAVAN = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoCTe.id, "#RENAVAM"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.RENAVAM });
    this.TagRNTRC = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoCTe.id, "#RNTRC"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.RNTRC });
    this.TagCNPJProprietario = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoCTe.id, "#CNPJProprietario"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.CNPJDoProprietario });
    this.TagNomeProprietario = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoCTe.id, "#NomeProprietario"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.NomeDoProprietario });
    this.TagNumeroCTe = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoCTe.id, "#NumeroCTe"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.NumeroDoCTe });
    this.TagSerieCTe = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoCTe.id, "#SerieCTe"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.SerieDoCTe });
    this.TagValorTotalPrestacao = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoCTe.id, "#ValorTotalPrestacao"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.ValorTotalDaPrestacao });

    this.AliquotaPIS = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AliquotaPIS.getFieldDescription(), visible: ko.observable(true), required: false, getType: typesKnockout.decimal, maxlength: 6, configDecimal: { precision: 4, allowZero: true } });
    this.AliquotaCOFINS = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AliquotaCOFINS.getFieldDescription(), visible: ko.observable(true), required: false, getType: typesKnockout.decimal, maxlength: 6, configDecimal: { precision: 4, allowZero: true } });

    this.CodigoIntegracaoTributaria = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoIntegracaoTributaria.getFieldDescription(), required: false, maxlength: 400, visible: ko.observable(true) });

    this.ObservacaoContratoFrete = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ObservacaoParaContratoDeFrete.getFieldDescription(), required: false, maxlength: 400, visible: ko.observable(true), issue: 0 });
    this.TagContratoFreteValorTotal = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoContratoFrete.id, "#ValorTotal"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.ValorTotal });
    this.TagContratoFretePercentualAdiantamento = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoContratoFrete.id, "#PercentualAdiantamento"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.PorcentagemAdiantamento });
    this.TagContratoFreteValorAdiantamento = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoContratoFrete.id, "#ValorAdiantamento"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.ValorAdiantamento });
    this.TagContratoFretePercentualAbastecimento = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoContratoFrete.id, "#PercentualAbastecimento"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.PorcentagemAbastecimento });
    this.TagContratoFreteValorAbastecimento = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoContratoFrete.id, "#ValorAbastecimento"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.ValorAbastecimento });
    this.TagContratoFreteSaldoReceber = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoContratoFrete.id, "#SaldoReceber"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.SaldoReceber });
    this.TagContratoFreteVencimentoAdiantamento = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoContratoFrete.id, "#VencimentoAdiantamento"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.VencimentoAdiantamento });
    this.TagContratoFreteVencimentoSaldo = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoContratoFrete.id, "#VencimentoSaldo"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.VencimentoSaldo });
    this.TagContratoFreteOperadoraValePedagio = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoContratoFrete.id, "#OperadoraValePedagio"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.OperadoraValePedagio });
    this.TagContratoFreteCartaoAbastecimento = PropertyEntity({ eventClick: function (e) { InserirTag(_transportadorTerceiro.ObservacaoContratoFrete.id, "#CartaoAbastecimento"); }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.CartaoAbastecimento });

    this.TextoAdicionalContratoFrete = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TextoAdicionalParaContratoDeFrete.getFieldDescription(), required: false, maxlength: 100000, visible: ko.observable(true), issue: 0 });
    this.ReterImpostosContratoFrete = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.TornarCampoINSSeReterImpostoTrazerComoSim ? true : ""), options: Global.ObterOpcoesNaoSelecionadoBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: ko.observable(_CONFIGURACAO_TMS.TornarCampoINSSeReterImpostoTrazerComoSim ? true : ""), text: Localization.Resources.Pessoas.Pessoa.ReterImpostos.getRequiredFieldDescription(), enable: ko.observable(true) });
    this.NaoSomarValorPedagioContratoFrete = PropertyEntity({ val: ko.observable(""), options: Global.ObterOpcoesNaoSelecionadoBooleano(Localization.Resources.Enumeradores.SimNao.Nao, Localization.Resources.Enumeradores.SimNao.Sim), def: "", text: Localization.Resources.Pessoas.Pessoa.SomarValorDoPedagioNoContratoDeFrete.getRequiredFieldDescription() });

    this.DiasVencimentoAdiantamentoContratoFrete = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DiasVenctoAdiantamento.getFieldDescription(), getType: typesKnockout.int, maxlength: 3, visible: ko.observable(true) });
    this.DiasVencimentoSaldoContratoFrete = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DiasVenctoSaldo.getFieldDescription(), enable: ko.observable(true), getType: typesKnockout.int, maxlength: 3, visible: ko.observable(true) });
    this.QuantidadeDependentes = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.QuantidadeDependentes.getFieldDescription(), getType: typesKnockout.int, maxlength: 3, visible: ko.observable(true) });
    this.CodigoProvedor = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoProvedor.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(""), def: "", maxlength: 9, visible: ko.observable(true), configInt: { thousands: "" } });
    this.TipoTerceiro = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoTerceiro.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.TipoPagamentoContratoFreteTerceiro = PropertyEntity({ val: ko.observable(""), options: EnumTipoPagamentoContratoFreteTerceiro.obterOpcoes("", Localization.Resources.Pessoas.Pessoa.UtilizarPadrao), def: "", text: Localization.Resources.Pessoas.Pessoa.TipoDeCalculo.getFieldDescription() });

    this.ConfiguracaoCIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.ConfiguracaoCIOT.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(true) });

    if (_CONFIGURACAO_TMS.TornarCampoINSSeReterImpostoTrazerComoSim) {
        this.CodigoINSS.required(true);
    }
    this.TipoTerceiro.val.subscribe(function (novoValor) {
        if (novoValor != null) {
            _transportadorTerceiro.ConfiguracaoCIOT.enable(false);
            _transportadorTerceiro.ReterImpostosContratoFrete.enable(false);
        }
        else {
            _transportadorTerceiro.ConfiguracaoCIOT.enable(true);
            _transportadorTerceiro.ReterImpostosContratoFrete.enable(true);
        }
    });

};

//*******EVENTOS*******

function loadTransportadorTerceiro() {
    _transportadorTerceiro = new TransportadorTerceiro();
    KoBindings(_transportadorTerceiro, "knockoutCadastroTransportadorTerceiro");

    loadAcrescimoDescontoAutomatico();

    new BuscarPagamentoMotoristaTipo(_transportadorTerceiro.PagamentoMotoristaTipo);
    new BuscarTipoTerceiro(_transportadorTerceiro.TipoTerceiro, retornoTipoTerceiro);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _transportadorTerceiro.GerarCIOT.visible(false);
        _transportadorTerceiro.ExigeCanhotoFechamentoContratoFrete.visible(false);
    }    

    BuscarConfiguracaoCIOT(_transportadorTerceiro.ConfiguracaoCIOT,
        Localization.Resources.Pessoas.Pessoa.ConsultaDeOperadorasDeCIOT,
        Localization.Resources.Pessoas.Pessoa.OperadorasDeCIOT,
        RetornoConsultaConfiguracaoCIOT);

    $("#" + _transportadorTerceiro.HabilitarDataFixaVencimento.id).click(verificarDataFixaVencimento);

    verificarDataFixaVencimento();
    loadGridTiposPagamentoCIOT();
    loadGridDiasFechamentoCIOTPeriodo();
}

function retornoTipoTerceiro(valor) {
    _transportadorTerceiro.TipoTerceiro.val(valor.Descricao);
    _transportadorTerceiro.TipoTerceiro.codEntity(valor.Codigo);

    if (valor != null) {
        _transportadorTerceiro.ReterImpostosContratoFrete.val(valor.ReterImpostosContratoFrete);
        _transportadorTerceiro.ReterImpostosContratoFrete.enable(false);
    }
    else {
        _transportadorTerceiro.ReterImpostosContratoFrete.enable(true);

    }

    if (valor.CodigoConfiguracaoCIOT != null) {
        _transportadorTerceiro.ConfiguracaoCIOT.val(valor.DescricaoConfiguracaoCIOT);
        _transportadorTerceiro.ConfiguracaoCIOT.codEntity(valor.CodigoConfiguracaoCIOT);
        _transportadorTerceiro.ConfiguracaoCIOT.enable(false);
    }
    else {
        _transportadorTerceiro.ConfiguracaoCIOT.enable(true);
    }
}
function verificarDataFixaVencimento(){
    if (_transportadorTerceiro.HabilitarDataFixaVencimento.val()) {
        $("#liDataFixaVencimentoTransportadorTerceiro").show();
        _transportadorTerceiro.DiasVencimentoSaldoContratoFrete.enable(false);
        _transportadorTerceiro.DiasVencimentoSaldoContratoFrete.val("");
    } else {
        $("#liDataFixaVencimentoTransportadorTerceiro").hide();
        _transportadorTerceiro.DiasVencimentoSaldoContratoFrete.enable(true);
    }

}

//*******MÉTODOS*******

function desabilitarOpcoesPercentuais() {
    _transportadorTerceiro.PercentualAbastecimentoFretesTerceiro.visible(false);
    _transportadorTerceiro.PercentualAdiantamentoFretesTerceiro.visible(false);
    _transportadorTerceiro.AliquotaPIS.visible(false);
    _transportadorTerceiro.AliquotaCOFINS.visible(false);
    _transportadorTerceiro.PercentualCobradoPadrao.visible(false);
    _transportadorTerceiro.DescontoPadrao.visible(false);
    _transportadorTerceiro.ObservacaoCTe.visible(false);
    _transportadorTerceiro.CodigoIntegracaoTributaria.visible(false);
}

function RetornoConsultaConfiguracaoCIOT(data) {
    _transportadorTerceiro.ConfiguracaoCIOT.val(data.Descricao);
    _transportadorTerceiro.ConfiguracaoCIOT.codEntity(data.Codigo);
}

function adicionarTipoPagamentoCIOTClick() {

    if (_transportadorTerceiro.TipoPagamentoCIOTOperadora.val() === "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var tipo = new TipoPagamentoCIOTMap();
    tipo.Codigo.val = guid();
    tipo.OperadoraCIOT.val = _transportadorTerceiro.OperadoraTipoPagamentoCIOTOperadora.val();
    tipo.TipoPagamentoCIOT.val = _transportadorTerceiro.TipoPagamentoCIOTOperadora.val();
    tipo.DescricaoTipoPagamentoCIOT.val = EnumTipoPagamentoCIOT.obterDescricao(tipo.TipoPagamentoCIOT.val);

    tipo.DescricaoOperadoraCIOT.val = EnumOperadoraCIOT.obterDescricao(tipo.OperadoraCIOT.val); 

    _transportadorTerceiro.TiposPagamentoCIOTOperadora.list.push(tipo);
    recarregarGridTiposPagamentoCIOT();
    
}

function adicionarDiaFechamentoCIOTPeriodoClick() {

    if (_transportadorTerceiro.DiaFechamentoCIOT.val() === "0" || _transportadorTerceiro.DiaFechamentoCIOT.val() === "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    let duplicado = false;
    $.each(_transportadorTerceiro.DiasFechamentoCIOTPeriodo.list, function (i, tipo) {
        if (tipo.DiaFechamentoCIOT.val == _transportadorTerceiro.DiaFechamentoCIOT.val()) {
            duplicado = true;
            return;
        }
    });

    if (duplicado) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.RegistroDuplicadoMensagem);
        return;
    }

    var diaFechamento = new DiaFechamentoCIOTMap();
    diaFechamento.Codigo.val = guid();
    diaFechamento.DiaFechamentoCIOT.val = _transportadorTerceiro.DiaFechamentoCIOT.val();

    _transportadorTerceiro.DiasFechamentoCIOTPeriodo.list.push(diaFechamento);
    _transportadorTerceiro.DiaFechamentoCIOT.val("0");
    recarregarGridDiasFechamentoCIOTPeriodo();

}

//*******EVENTOS*******

function loadGridTiposPagamentoCIOT() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: excluirTipoPagamentoCIOTClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "OperadoraCIOT", visible: false },
        { data: "TipoPagamentoCIOT", visible: false },
        { data: "DescricaoOperadoraCIOT", title: Localization.Resources.Pessoas.Pessoa.OperadoraDeCIOT, width: "50%" },
        { data: "DescricaoTipoPagamentoCIOT", title: Localization.Resources.Pessoas.Pessoa.TipoDoPagamento, width: "50%" }
    ];
    _gridTiposPagamentoCIOT = new BasicDataTable(_transportadorTerceiro.TiposPagamentoCIOTOperadora.idGrid, header, menuOpcoes);
    recarregarGridTiposPagamentoCIOT();
}

function recarregarGridTiposPagamentoCIOT() {
    var data = new Array();
    
    $.each(_transportadorTerceiro.TiposPagamentoCIOTOperadora.list, function (i, tipo) {
        var obj = new Object(); 
        obj.Codigo = tipo.Codigo.val;
        obj.TipoPagamentoCIOT = tipo.TipoPagamentoCIOT.val;
        obj.OperadoraCIOT = tipo.OperadoraCIOT.val;
        obj.DescricaoTipoPagamentoCIOT = tipo.DescricaoTipoPagamentoCIOT.val;
        obj.DescricaoOperadoraCIOT = tipo.DescricaoOperadoraCIOT.val;

        data.push(obj);
    });
    _gridTiposPagamentoCIOT.CarregarGrid(data);
}

function recarregarGridTiposPagamentoCIOTConsulta() {
    var data = new Array();

    $.each(_transportadorTerceiro.TiposPagamentoCIOTOperadora.list, function (i, tipo) {
        var obj = new Object();
        obj.Codigo = tipo.Codigo;
        obj.TipoPagamentoCIOT = tipo.TipoPagamentoCIOT;
        obj.OperadoraCIOT = tipo.OperadoraCIOT;
        obj.DescricaoTipoPagamentoCIOT = tipo.DescricaoTipoPagamentoCIOT;
        obj.DescricaoOperadoraCIOT = tipo.DescricaoOperadoraCIOT;

        data.push(obj);
    });
    _gridTiposPagamentoCIOT.CarregarGrid(data);
}
function excluirTipoPagamentoCIOTClick(data) {
    var listaAtualizada = new Array();
    $.each(_transportadorTerceiro.TiposPagamentoCIOTOperadora.list, function (i, tipo) {
        if (tipo.Codigo.val != data.Codigo) {
            listaAtualizada.push(tipo);
        }
    });
    _transportadorTerceiro.TiposPagamentoCIOTOperadora.list = listaAtualizada;
    recarregarGridTiposPagamentoCIOT();
}

function loadGridDiasFechamentoCIOTPeriodo() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: excluirDiaFechamentoCIOTPeriodoClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "DiaFechamentoCIOT", title: Localization.Resources.Pessoas.Pessoa.DiaFechamento, width: "50%" }
    ];
    _gridDiasFechamentoCIOTPeriodo = new BasicDataTable(_transportadorTerceiro.DiasFechamentoCIOTPeriodo.idGrid, header, menuOpcoes);
    recarregarGridDiasFechamentoCIOTPeriodo();
}

function recarregarGridDiasFechamentoCIOTPeriodo() {
    var data = new Array();

    $.each(_transportadorTerceiro.DiasFechamentoCIOTPeriodo.list, function (i, tipo) {
        var obj = new Object();
        obj.Codigo = tipo.Codigo.val;
        obj.DiaFechamentoCIOT = tipo.DiaFechamentoCIOT.val;

        data.push(obj);
    });
    _gridDiasFechamentoCIOTPeriodo.CarregarGrid(data);
}

function recarregarGridDiasFechamentoCIOTPeriodoConsulta() {
    var data = new Array();

    $.each(_transportadorTerceiro.DiasFechamentoCIOTPeriodo.list, function (i, tipo) {
        var obj = new Object();
        obj.Codigo = tipo.Codigo;
        obj.DiaFechamentoCIOT = tipo.DiaFechamentoCIOT;

        data.push(obj);
    });
    _gridDiasFechamentoCIOTPeriodo.CarregarGrid(data);
}

function excluirDiaFechamentoCIOTPeriodoClick(data) {
    var listaAtualizada = new Array();
    $.each(_transportadorTerceiro.DiasFechamentoCIOTPeriodo.list, function (i, tipo) {
        if (tipo.Codigo.val != data.Codigo) {
            listaAtualizada.push(tipo);
        }
    });
    _transportadorTerceiro.DiasFechamentoCIOTPeriodo.list = listaAtualizada;
    recarregarGridDiasFechamentoCIOTPeriodo();
}

function preencherDadosTransportadorTerceiro() {
    _transportadorTerceiro.GerarCIOT.val(true);

    _transportadorTerceiro.TipoGeracaoCIOT.val(_CONFIGURACAO_TMS.configuracaoGeralCIOT.TipoGeracaoCIOT);
    _transportadorTerceiro.TipoFavorecidoCIOT.val(_CONFIGURACAO_TMS.configuracaoGeralCIOT.TipoFavorecidoCIOT);
    _transportadorTerceiro.TipoQuitacaoCIOT.val(_CONFIGURACAO_TMS.configuracaoGeralCIOT.TipoQuitacaoCIOT);
    _transportadorTerceiro.TipoAdiantamentoCIOT.val(_CONFIGURACAO_TMS.configuracaoGeralCIOT.TipoAdiantamentoCIOT);

    var tipo = new TipoPagamentoCIOTMap();
    for (var objeto of _CONFIGURACAO_TMS.configuracaoGeralTipoPagamentoCIOTs.configuracaoGeralTipoPagamentoCIOTs) {
        tipo.Codigo.val = guid();
        tipo.OperadoraCIOT.val = objeto.OperadoraCIOT;
        tipo.TipoPagamentoCIOT.val = objeto.TipoPagamentoCIOT;
        tipo.DescricaoTipoPagamentoCIOT.val = objeto.DescricaoTipoPagamentoCIOT;
        tipo.DescricaoOperadoraCIOT.val = objeto.DescricaoOperadoraCIOT;

        _transportadorTerceiro.TiposPagamentoCIOTOperadora.list.push(tipo);
        tipo = new TipoPagamentoCIOTMap();

        recarregarGridTiposPagamentoCIOT();
    }
}