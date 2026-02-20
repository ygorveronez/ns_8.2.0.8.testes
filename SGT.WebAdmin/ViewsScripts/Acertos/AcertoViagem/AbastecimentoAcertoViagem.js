/// <reference path="OcorrenciaAcertoViagem.js" />
/// <reference path="../../Consultas/Abastecimento.js" />
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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CabecalhoAcertoViagem.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="CargaAcertoViagem.js" />
/// <reference path="AcertoViagem.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Pedagio.js" />
/// <reference path="DespesaAcertoViagem.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="PedagioAcertoViagem.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="FechamentoAcertoViagem.js" />
/// <reference path="EtapaAcertoViagem.js" />
/// <reference path="../../../js/plugin/chartjs/chart.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _abastecimentoAcertoViagem;
var _gridAbastecimento;
var _gridArla;
var _HTMLAbastecimentoAcertoViagem;
var _HTMLAbastecimentoDoVeiculo;
var _HTMLArlaDoVeiculo;
var _abastecimentoDoVeiculo;
var _arlaDoVeiculo;
var _novoAbastecimento;
var _novoArla;
var _gridAbastecimentoSelecao;
var _gridArlaSelecao;

var AbastecimentoAcertoViagem = function () {
    this.Veiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.VeiculosArla = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.RetornarCarga = PropertyEntity({ eventClick: RetornarCargaClick, type: types.event, text: "Retornar Cargas", visible: ko.observable(false), enable: ko.observable(true) });
    this.InformarPedagio = PropertyEntity({ eventClick: LancarPedagioClick, type: types.event, text: "Salvar Abastecimento", visible: ko.observable(true), enable: ko.observable(true) });
};

var AbastecimentoDoVeiculo = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Veiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAcerto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Abastecimentos = PropertyEntity({ type: types.map, required: false, text: "Buscar abastecimento do veículo", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.AdicionarAbastecimento = PropertyEntity({ eventClick: InformarAbastecimentoClick, type: types.event, text: "Adicionar Abastecimento", visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverAbastecimento = PropertyEntity({ eventClick: RemoverAbastecimentoSelecionadosClick, type: types.event, text: "Remover Abastecimento Selecionados", visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoAbastecimento = PropertyEntity({ getType: typesKnockout.int, required: false, maxlength: 1, text: "", visible: false, val: ko.observable(1) });

    this.Placa = PropertyEntity({ text: "Placa: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.KMInicial = PropertyEntity({ text: "KM Inicial: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.KMFinal = PropertyEntity({ text: "KM Final: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.KMTotal = PropertyEntity({ text: "KM Total: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Litros = PropertyEntity({ text: "Litros: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorTotal = PropertyEntity({ text: "Valor Total: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.MediaFinal = PropertyEntity({ text: "Media Final: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Ideal = PropertyEntity({ text: "Media Ideal ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), visible: true, maxlength: 5, enable: ko.observable(true) });
    this.Aceitavel = PropertyEntity({ text: ko.observable("Aceitável (0 até 0) "), getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Fora = PropertyEntity({ text: ko.observable("Fora (abaixo de 0) "), getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.KMTotalAjustado = PropertyEntity({ text: "KM Total Ajustado", getType: typesKnockout.int, val: ko.observable(0), visible: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.PercentalAjusteKM = PropertyEntity({ text: "% de Ajuste ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: true }, val: ko.observable(0.00), visible: ko.observable(true), maxlength: 6, enable: ko.observable(true) });

    this.Autorizar = PropertyEntity({ eventClick: AutorizarAbastecimentoClick, type: types.event, text: "Autorizar", visible: ko.observable(true), enable: ko.observable(true) });

    this.HorimetroInicial = PropertyEntity({ text: "Hr Inicial: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.HorimetroFinal = PropertyEntity({ text: "Hr Final: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.HorimetroTotal = PropertyEntity({ text: "Hr Total: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.LitrosEquipamento = PropertyEntity({ text: "Litros: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorTotalEquipamento = PropertyEntity({ text: "Valor Total: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.MediaFinalHorimetro = PropertyEntity({ text: "Media Hr Final: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.IdealHorimetro = PropertyEntity({ text: "Media Hr Ideal ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), visible: true, maxlength: 5, enable: ko.observable(true) });
    this.HorimetroTotalAjustado = PropertyEntity({ text: "Hr Total Ajustado", getType: typesKnockout.int, val: ko.observable(0), visible: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.PercentalAjusteHorimetro = PropertyEntity({ text: "% de Ajuste ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: true }, val: ko.observable(0.00), visible: ko.observable(true), maxlength: 6, enable: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
};

var ArlaDoVeiculo = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Veiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });

    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAcerto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Arlas = PropertyEntity({ type: types.map, required: false, text: "Buscar arla do veículo", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.AdicionarArla = PropertyEntity({ eventClick: InformarArlaClick, type: types.event, text: "Adicionar Arla 32", visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverArla = PropertyEntity({ eventClick: RemoverArlasClick, type: types.event, text: "Remover Arla Selecionadas", visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoAbastecimento = PropertyEntity({ getType: typesKnockout.int, required: false, maxlength: 1, text: "", visible: false, val: ko.observable(2) });

    this.Placa = PropertyEntity({ text: "Placa: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Litros = PropertyEntity({ text: "Litros: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorTotal = PropertyEntity({ text: "Valor Total: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.MediaFinal = PropertyEntity({ text: "Media Final: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Ideal = PropertyEntity({ text: "% Gasto ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), visible: true, maxlength: 6, enable: ko.observable(true) });
    this.Aceitavel = PropertyEntity({ text: ko.observable("Aceitável (0 até 0) "), getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Fora = PropertyEntity({ text: ko.observable("Fora (abaixo de 0) "), getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.Autorizar = PropertyEntity({ eventClick: AutorizarArlaClick, type: types.event, text: "Autorizar", visible: ko.observable(true), enable: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
};

var AdicionarAbastecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.VeiculoAbastecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(false) });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), required: true, issue: 171 });
    this.DataAbastecimento = PropertyEntity({ getType: typesKnockout.dateTime, required: true, dataLimit: _acertoViagem.DataFinal, text: "*Data Hora do Abastecimento:", dateRangeInit: _acertoViagem.DataInicial, dateRangeLimit: _acertoViagem.DataFinal, issue: 2 });

    this.Combustivel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Combustivel:", idBtnSearch: guid(), required: true, issue: 140 });
    this.NumeroDocumento = PropertyEntity({ getType: typesKnockout.string, required: false, maxlength: 20, text: "Documento:" });
    this.Horimetro = PropertyEntity({ text: "*Horímetro:", required: false, getType: typesKnockout.int, maxlength: 15, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), visible: ko.observable(true) });
    this.Kilometragem = PropertyEntity({ getType: typesKnockout.string, required: false, maxlength: 20, text: "*Kilometragem:" });

    this.Litros = PropertyEntity({ text: "*Litros:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable("0,00"), maxlength: 8 });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable("0,0000"), maxlength: 8 });
    //this.ValorTotal = PropertyEntity({ text: "*Valor Total:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10 });
    this.ValorTotal = PropertyEntity({ text: "*Valor Total: ", required: true, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(true), configDecimal: { precision: 2, allowZero: false, allowNegative: false } });

    this.TipoAbastecimento = PropertyEntity({ getType: typesKnockout.int, required: false, maxlength: 1, text: "", visible: false, val: ko.observable(1), def: 1 });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarAbastecimentoClick, text: "Adicionar", visible: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.MoedaCotacaoBancoCentral.val.subscribe(function (novoValor) {
        //CalcularMoedaEstrangeiraAbastecimento();
    });

    this.DataBaseCRT.val.subscribe(function (novoValor) {
        //CalcularMoedaEstrangeiraAbastecimento();
    });

    this.ValorMoedaCotacao.val.subscribe(function (novoValor) {
        //ConverterValorAbastecimento();
    });

    this.ValorOriginalMoedaEstrangeira.val.subscribe(function (novoValor) {
        //ConverterValorAbastecimento();
    });

    //this.ValorTotal.val.subscribe(function (novoValor) {
    //    ConverterValorOriginalAbastecimento();
    //});
};

var AdicionarArla = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.VeiculoAbastecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(false)  });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), required: true, issue: 171 });
    this.DataAbastecimento = PropertyEntity({ getType: typesKnockout.dateTime, required: true, dataLimit: _acertoViagem.DataFinal, text: "*Data Hora do Abastecimento:", dateRangeInit: _acertoViagem.DataInicial, dateRangeLimit: _acertoViagem.DataFinal, issue: 2 });

    this.Combustivel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Combustivel:", idBtnSearch: guid(), required: true, issue: 140 });
    this.NumeroDocumento = PropertyEntity({ getType: typesKnockout.string, required: false, maxlength: 20, text: "Documento:" });
    this.Horimetro = PropertyEntity({ text: "*Horímetro:", required: false, getType: typesKnockout.int, maxlength: 15, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), visible: ko.observable(true) });
    this.Kilometragem = PropertyEntity({ getType: typesKnockout.string, required: false, maxlength: 20, text: "*Kilometragem:" });

    this.Litros = PropertyEntity({ text: "*Litros:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable("0,00"), maxlength: 8 });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable("0,0000"), maxlength: 8 });
    //this.ValorTotal = PropertyEntity({ text: "*Valor Total:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10 });
    this.ValorTotal = PropertyEntity({ text: "*Valor Total: ", required: true, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(true), configDecimal: { precision: 2, allowZero: false, allowNegative: false } });

    this.TipoAbastecimento = PropertyEntity({ getType: typesKnockout.int, required: false, maxlength: 1, text: "", visible: false, val: ko.observable(2), def: 2 });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarArlaClick, text: "Adicionar", visible: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.MoedaCotacaoBancoCentral.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeiraArla();
    });

    this.DataBaseCRT.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeiraArla();
    });

    this.ValorMoedaCotacao.val.subscribe(function (novoValor) {
        ConverterValorArla();
    });

    this.ValorOriginalMoedaEstrangeira.val.subscribe(function (novoValor) {
        ConverterValorArla();
    });

    //this.ValorTotal.val.subscribe(function (novoValor) {
    //    ConverterValorOriginalArla();
    //});
};


//*******EVENTOS*******

function loadAbastecimentoAcertoViagem() {
    $("#contentAbastecimentoAcertoViagem").html("");
    var idDiv = guid();
    $("#contentAbastecimentoAcertoViagem").append(_HTMLAbastecimentoAcertoViagem.replace(/#abastecimentoAcertoViagem/g, idDiv));
    _abastecimentoAcertoViagem = new AbastecimentoAcertoViagem();
    KoBindings(_abastecimentoAcertoViagem, idDiv);

    _novoAbastecimento = new AdicionarAbastecimento();
    KoBindings(_novoAbastecimento, "knoutAdicionarAbastecimento");

    new BuscarProdutoTMS(_novoAbastecimento.Combustivel, RetornoAbastecimento, _novoAbastecimento.TipoAbastecimento);
    new BuscarClientes(_novoAbastecimento.Fornecedor, RetornoFornecedorAbastecimento, false, [EnumModalidadePessoa.Fornecedor]);
    new BuscarVeiculos(_novoAbastecimento.VeiculoAbastecimento, null, null, null, null, null, _acertoViagem.Codigo);
    new BuscarEquipamentos(_novoAbastecimento.Equipamento);

    _novoArla = new AdicionarArla();
    KoBindings(_novoArla, "knoutAdicionarArla");

    new BuscarProdutoTMS(_novoArla.Combustivel, RetornoArla, _novoArla.TipoAbastecimento);
    new BuscarClientes(_novoArla.Fornecedor, RetornoFornecedorArla, false, [EnumModalidadePessoa.Fornecedor]);
    new BuscarVeiculos(_novoArla.VeiculoAbastecimento, null, null, null, null, null, _acertoViagem.Codigo);
    new BuscarEquipamentos(_novoArla.Equipamento);

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _novoArla.MoedaCotacaoBancoCentral.visible(true);
        _novoArla.DataBaseCRT.visible(true);
        _novoArla.ValorMoedaCotacao.visible(true);
        _novoArla.ValorOriginalMoedaEstrangeira.visible(true);
    }

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _novoAbastecimento.MoedaCotacaoBancoCentral.visible(true);
        _novoAbastecimento.DataBaseCRT.visible(true);
        _novoAbastecimento.ValorMoedaCotacao.visible(true);
        _novoAbastecimento.ValorOriginalMoedaEstrangeira.visible(true);
    }

    CarregarVeiculosAbastecimento();
    CarregarAbastecimentoVeiculo(0);
    CarregarArlaVeiculo(0);

    carregarConteudosPedagioHTML(loadPedagioAcertoViagem);
}

function AutorizarAbastecimentoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarAbastecimento, _PermissoesPersonalizadas)) {
        if (_abastecimentoDoVeiculo.CodigoVeiculo.val() != null && _abastecimentoDoVeiculo.CodigoVeiculo.val() > 0) {
            var data = {
                Codigo: _acertoViagem.Codigo.val(),
                CodigoVeiculo: _abastecimentoDoVeiculo.CodigoVeiculo.val(),
                TipoAbastecimento: 1
            };
            executarReST("AcertoAbastecimento/AutorizarAbastecimento", data, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                    _abastecimentoDoVeiculo.Autorizar.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        }
    } else
        exibirMensagem(tipoMensagem.atencao, "Sem Permissão", "Seu usuário não possui permissão para autorizar o abastecimento.");
}

function AutorizarArlaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarArla, _PermissoesPersonalizadas)) {
        if (_arlaDoVeiculo.CodigoVeiculo.val() != null && _arlaDoVeiculo.CodigoVeiculo.val() > 0) {
            var data = {
                Codigo: _acertoViagem.Codigo.val(),
                CodigoVeiculo: _arlaDoVeiculo.CodigoVeiculo.val(),
                TipoAbastecimento: 2
            };
            executarReST("AcertoAbastecimento/AutorizarAbastecimento", data, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                    _arlaDoVeiculo.Autorizar.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        }
    } else
        exibirMensagem(tipoMensagem.atencao, "Sem Permissão", "Seu usuário não possui permissão para autorizar a arla.");
}

function CarregarVeiculosAbastecimento() {
    _abastecimentoAcertoViagem.Veiculos.val(_acertoViagem.ListaVeiculos.val());
    _abastecimentoAcertoViagem.VeiculosArla.val(_acertoViagem.ListaVeiculosArla.val());
}

function CarregarAbastecimentoVeiculo(indice) {
    var idDiv = guid();
    $("#contentAbastecimentoDoVeiculo").html(_HTMLAbastecimentoDoVeiculo.replace("#knoutAbastecimentosDoVeiculo", idDiv + "_knoutAbastecimentosDoVeiculo"));

    _abastecimentoDoVeiculo = new AbastecimentoDoVeiculo();
    KoBindings(_abastecimentoDoVeiculo, idDiv + "_knoutAbastecimentosDoVeiculo");

    var classActive = '"';
    if (_abastecimentoAcertoViagem.Veiculos.val().length > 1) {
        var html = "";
        $.each(_abastecimentoAcertoViagem.Veiculos.val(), function (i, veiculo) {
            if (i == indice)
                html += '<li class="active" class="nav-item">';
            else
                html += '<li>';

            if (i == indice)
                html += '<a href="javascript:void(0);" class="nav-link active" data-bs-toggle="tab" onclick="CarregarAbastecimentoVeiculo(' + i + ')"><span class="hidden-mobile hidden-tablet">' + (veiculo.Placa + (veiculo.Equipamentos != null && veiculo.Equipamentos != "" ? (" " + veiculo.Equipamentos) : "")) + '</span></a>';
            else
                html += '<a href="javascript:void(0);" class="nav-link" data-bs-toggle="tab" onclick="CarregarAbastecimentoVeiculo(' + i + ')"><span class="hidden-mobile hidden-tablet">' + (veiculo.Placa + (veiculo.Equipamentos != null && veiculo.Equipamentos != "" ? (" " + veiculo.Equipamentos) : "")) + '</span></a>';
            html += '</li>';
        });
        $("#" + _abastecimentoDoVeiculo.Veiculo.idTab).html(html);
        $("#" + _abastecimentoDoVeiculo.Veiculo.idTab).show();
    }

    if (_abastecimentoAcertoViagem.Veiculos.val().length > 0) {
        _abastecimentoDoVeiculo.CodigoVeiculo.val(_abastecimentoAcertoViagem.Veiculos.val()[indice].Codigo);
        _abastecimentoDoVeiculo.Placa.val(_abastecimentoAcertoViagem.Veiculos.val()[indice].Placa + (_abastecimentoAcertoViagem.Veiculos.val()[indice].Equipamentos != null && _abastecimentoAcertoViagem.Veiculos.val()[indice].Equipamentos != "" ? (" " + _abastecimentoAcertoViagem.Veiculos.val()[indice].Equipamentos) : ""));
    }
    else
        _abastecimentoDoVeiculo.CodigoVeiculo.val(0);
    _abastecimentoDoVeiculo.CodigoAcerto.val(_acertoViagem.Codigo.val());
    _abastecimentoDoVeiculo.Autorizar.visible(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarAbastecimento, _PermissoesPersonalizadas));

    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteAjustarKMTotal, _PermissoesPersonalizadas)) {
        _abastecimentoDoVeiculo.KMTotalAjustado.enable(true);
        _abastecimentoDoVeiculo.PercentalAjusteKM.enable(true);
        _abastecimentoDoVeiculo.HorimetroTotalAjustado.enable(true);
        _abastecimentoDoVeiculo.PercentalAjusteHorimetro.enable(true);
    } else {
        _abastecimentoDoVeiculo.KMTotalAjustado.enable(false);
        _abastecimentoDoVeiculo.PercentalAjusteKM.enable(false);
        _abastecimentoDoVeiculo.HorimetroTotalAjustado.enable(false);
        _abastecimentoDoVeiculo.PercentalAjusteHorimetro.enable(false);
    }

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_BloquearEdicaoResumoAbastecimento, _PermissoesPersonalizadas)) {
        _abastecimentoDoVeiculo.KMTotalAjustado.visible(false);
        _abastecimentoDoVeiculo.PercentalAjusteKM.visible(false);
        _abastecimentoDoVeiculo.HorimetroTotalAjustado.visible(false);
        _abastecimentoDoVeiculo.PercentalAjusteHorimetro.visible(false);
    }
    else {
        _abastecimentoDoVeiculo.KMTotalAjustado.visible(true);
        _abastecimentoDoVeiculo.PercentalAjusteKM.visible(true);
        _abastecimentoDoVeiculo.HorimetroTotalAjustado.visible(true);
        _abastecimentoDoVeiculo.PercentalAjusteHorimetro.visible(true);
    }

    var excluirAbastecimento = { descricao: "Excluir", id: guid(), metodo: RemoverAbastecimentoClick, icone: "" };
    var editarAbastecimento = { descricao: "Editar", id: guid(), metodo: EditarAbastecimentoClick, icone: "" };
    var auditarAbastecimento = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("AcertoAbastecimento", null, _novoAbastecimento), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [excluirAbastecimento, editarAbastecimento, auditarAbastecimento] };

    _abastecimentoDoVeiculo.SelecionarTodos.visible(true);
    _abastecimentoDoVeiculo.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: null,
        callbackNaoSelecionado: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _abastecimentoDoVeiculo.SelecionarTodos,
        somenteLeitura: false
    };

    var headerAbastecimentoSelecao = [
        { data: "CodigoAcertoAbastecimento", visible: false },
        { data: "Codigo", visible: false },
        { data: "CodigoAcertoViagem", visible: false },
        { data: "CodigoVeiculo", visible: false },
        { data: "Combustivel", visible: false },
        { data: "Fornecedor", visible: false },
        { data: "DataAbastecimento", visible: false },
        { data: "Litros", visible: false },
        { data: "ValorUnitario", visible: false },
        { data: "ValorTotal", visible: false },
        { data: "TipoAbastecimento", visible: false },
        { data: "Kilometragem", visible: false },
        { data: "NumeroDocumento", visible: false },
        { data: "Equipamento", visible: false },
        { data: "Horimetro", visible: false },
        { data: "MoedaCotacaoBancoCentral", visible: false },
        { data: "DataBaseCRT", visible: false },
        { data: "ValorMoedaCotacao", visible: false },
        { data: "ValorOriginalMoedaEstrangeira", visible: false },
        { data: "CodigoFechamentoAbastecimento", visible: false },
    ];

    _gridAbastecimentoSelecao = new BasicDataTable(_abastecimentoDoVeiculo.Grid.id, headerAbastecimentoSelecao, null, { column: 0, dir: orderDir.asc });

    _gridAbastecimento = new GridView(_abastecimentoDoVeiculo.Abastecimentos.idGrid, "AcertoAbastecimento/PesquisarAbastecimento", _abastecimentoDoVeiculo, menuOpcoes, null, null, null, null, null, multiplaescolha);

    new BuscarAbastecimentosSemAcertoDeViagem(_abastecimentoDoVeiculo.Abastecimentos, RetornoInserirAbastecimento, _gridAbastecimentoSelecao, _acertoViagem.Codigo, _abastecimentoDoVeiculo.CodigoVeiculo, 1);
    _abastecimentoDoVeiculo.Abastecimentos.basicTable = _gridAbastecimentoSelecao;
    _gridAbastecimentoSelecao.CarregarGrid([]);

    _gridAbastecimento.CarregarGrid(AtualizarResumoAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val()));
    VerificarBotoes();
    //Global.ResetarAbas();
}

function RetornoInserirAbastecimento(data) {
    if (data != null) {
        var abastecimentos = new Array();

        for (var i = 0; i < data.length; i++) {
            abastecimentos.push({
                CodigoAcertoAbastecimento: data[i].CodigoAcertoAbastecimento,
                Codigo: data[i].Codigo,
                CodigoAcertoViagem: data[i].CodigoAcertoViagem,
                CodigoVeiculo: data[i].CodigoVeiculo,
                Combustivel: data[i].CodigoCombustivel,
                Fornecedor: data[i].CodigoFornecedor,
                DataAbastecimento: data[i].DataAbastecimento,
                Litros: data[i].Litros,
                ValorUnitario: data[i].ValorUnitario,
                ValorTotal: data[i].ValorTotal,
                TipoAbastecimento: data[i].TipoAbastecimento,
                Kilometragem: parseFloat(data[i].Kilometragem.replace(".", "").replace(".", "").replace(".", "").replace(",", ".")),
                NumeroDocumento: data[i].NumeroDocumento,
                Equipamento: data[i].CodigoEquipamento,
                Horimetro: data[i].Horimetro,
                MoedaCotacaoBancoCentral: data[i].MoedaCotacaoBancoCentral,
                DataBaseCRT: data[i].DataBaseCRT,
                ValorMoedaCotacao: data[i].ValorMoedaCotacao,
                ValorOriginalMoedaEstrangeira: data[i].ValorOriginalMoedaEstrangeira,
                CodigoFechamentoAbastecimento: data[i].CodigoFechamentoAbastecimento
            });
        }

        var dataEnvio = {
            Abastecimentos: JSON.stringify(abastecimentos)
        };

        executarReST("AcertoAbastecimento/InserirAbastecimento", dataEnvio, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (data[0].TipoAbastecimento == 1)
                        _gridAbastecimento.CarregarGrid(AtualizarResumoAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val()));
                    else
                        _gridArla.CarregarGrid(AtualizarResumoArlaVeiculo(_arlaDoVeiculo.CodigoVeiculo.val()));
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        });
    }
}

function AtualizarResumoAbastecimentoVeiculo(codigoVeiculo) {
    if (codigoVeiculo != null && codigoVeiculo > 0) {
        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoVeiculo: codigoVeiculo,
            TipoAbastecimento: 1,
            MediaIdeal: _abastecimentoDoVeiculo.Ideal.val()
        };
        executarReST("AcertoAbastecimento/DadosAbastecimentoVeiculo", data, function (arg) {
            if (arg.Success) {
                _abastecimentoDoVeiculo.KMInicial.val(arg.Data.KMInicial);
                _abastecimentoDoVeiculo.KMFinal.val(arg.Data.KMFinal);
                _abastecimentoDoVeiculo.KMTotal.val(arg.Data.KMTotal);
                _abastecimentoDoVeiculo.Litros.val(arg.Data.Litros);
                _abastecimentoDoVeiculo.ValorTotal.val(arg.Data.ValorTotal);
                _abastecimentoDoVeiculo.MediaFinal.val(arg.Data.MediaFinal);
                _abastecimentoDoVeiculo.Ideal.val(arg.Data.MediaIdeal);

                _abastecimentoDoVeiculo.KMTotalAjustado.val(arg.Data.KMTotalAjustado);
                _abastecimentoDoVeiculo.PercentalAjusteKM.val(arg.Data.PercentualAjuste);

                _abastecimentoDoVeiculo.HorimetroInicial.val(arg.Data.HorimetroInicial);
                _abastecimentoDoVeiculo.HorimetroFinal.val(arg.Data.HorimetroFinal);
                _abastecimentoDoVeiculo.LitrosEquipamento.val(arg.Data.LitrosEquipamento);
                _abastecimentoDoVeiculo.ValorTotalEquipamento.val(arg.Data.ValorTotalEquipamento);
                _abastecimentoDoVeiculo.HorimetroTotal.val(arg.Data.HorimetroTotal);
                _abastecimentoDoVeiculo.MediaFinalHorimetro.val(arg.Data.MediaFinalHorimetro);
                _abastecimentoDoVeiculo.IdealHorimetro.val(arg.Data.MediaIdealHorimetro);
                _abastecimentoDoVeiculo.HorimetroTotalAjustado.val(arg.Data.HorimetroTotalAjustado);
                _abastecimentoDoVeiculo.PercentalAjusteHorimetro.val(arg.Data.PercentalAjusteHorimetro);

                var valorIdeal = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.Ideal.val()), "n2"))
                var mediaFinal = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.MediaFinal.val()), "n2"))

                var valorAte = valorIdeal - 0.5;
                if (valorAte < 0)
                    valorAte = 0;

                if (isNaN(valorIdeal))
                    valorIdeal = 0;
                if (isNaN(mediaFinal))
                    mediaFinal = 0;
                if (isNaN(valorAte))
                    valorAte = 0;

                _abastecimentoDoVeiculo.Aceitavel.text("Aceitável (" + Globalize.format(valorIdeal, "n2") + " até " + Globalize.format(valorAte, "n2") + ") ");
                _abastecimentoDoVeiculo.Fora.text("Fora (abaixo de " + Globalize.format(valorAte, "n2") + ") ");

                if (arg.Data.HorimetroInicial > 0 || arg.Data.HorimetroFinal > 0)
                    $("#liTabHorimetro").show();
                else
                    $("#liTabHorimetro").hide();

                if (mediaFinal == 0)
                    $("#divMediaFinalAbastecimento").css("background-color", "#FFFFFF");
                else if (mediaFinal >= valorIdeal)
                    $("#divMediaFinalAbastecimento").css("background-color", "#90EE90");
                else if (mediaFinal >= (valorIdeal - 0.5) && mediaFinal < valorIdeal)
                    $("#divMediaFinalAbastecimento").css("background-color", "#FFFACD");
                else
                    $("#divMediaFinalAbastecimento").css("background-color", "#FF8C69");
                VerificaBotoesAutorizacaoAbastecimento(1, codigoVeiculo);
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    }
}

function AlteraHorimetroTotalAbastecimento(e) {
    var horimetroTotalAjustado = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.HorimetroTotalAjustado.val()), "n0"));
    var horimetroTotal = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.HorimetroTotal.val()), "n0"));
    var percentualAjuste = 0;

    if (horimetroTotalAjustado > 0 && horimetroTotal > 0) {
        percentualAjuste = ((horimetroTotalAjustado * 100) / horimetroTotal) - 100;
    }
    _abastecimentoDoVeiculo.PercentalAjusteHorimetro.val(Globalize.format(percentualAjuste, "n2"));
    AtualizarMediaIdealHorimetroAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val(), 1);
}

function AlteraKMTotalAbastecimento(e) {
    var kmTotalAjustado = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.KMTotalAjustado.val()), "n0"));
    var kmTotal = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.KMTotal.val()), "n0"));
    var percentualAjuste = 0;

    if (kmTotalAjustado > 0 && kmTotal > 0) {
        percentualAjuste = ((kmTotalAjustado * 100) / kmTotal) - 100;
    }
    _abastecimentoDoVeiculo.PercentalAjusteKM.val(Globalize.format(percentualAjuste, "n2"));
    AtualizarMediaIdealAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val(), 1);
}

function AlteraPercentalAjusteHorimetroAbastecimento(e) {
    var horimetroTotalAjustado = 0;
    var horimetroTotal = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.HorimetroTotal.val()), "n0"));
    var percentualAjusteHorimetro = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.PercentalAjusteHorimetro.val()), "n2"));

    if (percentualAjusteHorimetro != 0 && horimetroTotal > 0) {
        horimetroTotalAjustado = ((horimetroTotal * percentualAjusteHorimetro) / 100) + horimetroTotal;
    }
    if (horimetroTotalAjustado.toFixed(0) > 0)
        _abastecimentoDoVeiculo.HorimetroTotalAjustado.val(Globalize.format(horimetroTotalAjustado, "n0"));
    else
        _abastecimentoDoVeiculo.HorimetroTotalAjustado.val(Globalize.format(horimetroTotal, "n0"));

    AtualizarMediaIdealHorimetroAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val(), 1)
}

function AlteraPercentalAjusteKMAbastecimento(e) {
    var kmTotalAjustado = 0;
    var kmTotal = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.KMTotal.val()), "n0"));
    var percentualAjuste = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.PercentalAjusteKM.val()), "n2"));

    if (percentualAjuste != 0 && kmTotal > 0) {
        kmTotalAjustado = ((kmTotal * percentualAjuste) / 100) + kmTotal;
    }
    if (kmTotalAjustado.toFixed(0) > 0)
        _abastecimentoDoVeiculo.KMTotalAjustado.val(Globalize.format(kmTotalAjustado, "n0"));
    else
        _abastecimentoDoVeiculo.KMTotalAjustado.val(Globalize.format(kmTotal, "n0"));
    AtualizarMediaIdealAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val(), 1);
}

function AlteraMediaIdealHorimetroAbastecimento(e) {
    AtualizarMediaIdealHorimetroAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val(), 1);
}

function AlteraMediaIdealAbastecimento(e) {
    var valorIdeal = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.Ideal.val()), "n2"))
    var mediaFinal = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.MediaFinal.val()), "n2"))

    var valorAte = valorIdeal - 0.5;
    if (valorAte < 0)
        valorAte = 0;

    if (isNaN(valorIdeal))
        valorIdeal = 0;
    if (isNaN(mediaFinal))
        mediaFinal = 0;
    if (isNaN(valorAte))
        valorAte = 0;

    _abastecimentoDoVeiculo.Aceitavel.text("Aceitável (" + Globalize.format(valorIdeal, "n2") + " até " + Globalize.format(valorAte, "n2") + ") ");
    _abastecimentoDoVeiculo.Fora.text("Fora (abaixo de " + Globalize.format(valorAte, "n2") + ") ");

    if (mediaFinal == 0)
        $("#divMediaFinalAbastecimento").css("background-color", "#FFFFFF");
    else if (mediaFinal >= valorIdeal)
        $("#divMediaFinalAbastecimento").css("background-color", "#90EE90");
    else if (mediaFinal >= (valorIdeal - 0.5) && mediaFinal < valorIdeal)
        $("#divMediaFinalAbastecimento").css("background-color", "#FFFACD");
    else
        $("#divMediaFinalAbastecimento").css("background-color", "#FF8C69");

    AtualizarMediaIdealAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val(), 1);
}

function AtualizarMediaIdealHorimetroAbastecimentoVeiculo(codigoVeiculo, tipoAbastecimento) {
    if (codigoVeiculo != null && codigoVeiculo > 0) {
        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoVeiculo: codigoVeiculo,
            TipoAbastecimento: tipoAbastecimento,
            MediaIdealHorimetro: _abastecimentoDoVeiculo.IdealHorimetro.val(),
            HorimetroTotalAjustado: _abastecimentoDoVeiculo.HorimetroTotalAjustado.val(),
            PercentalAjusteHorimetro: _abastecimentoDoVeiculo.PercentalAjusteHorimetro.val()
        };
        executarReST("AcertoAbastecimento/SalvarMediaIdealHorimetroAbastecimento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    _abastecimentoDoVeiculo.IdealHorimetro.val(arg.Data.MediaIdealHorimetro);
                    _abastecimentoDoVeiculo.MediaFinalHorimetro.val(arg.Data.MediaFinalHorimetro);

                    VerificaBotoesAutorizacaoAbastecimento(tipoAbastecimento, codigoVeiculo);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    }
}

function AtualizarMediaIdealAbastecimentoVeiculo(codigoVeiculo, tipoAbastecimento) {
    if (codigoVeiculo != null && codigoVeiculo > 0) {
        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoVeiculo: codigoVeiculo,
            TipoAbastecimento: tipoAbastecimento,
            MediaIdeal: tipoAbastecimento == 1 ? _abastecimentoDoVeiculo.Ideal.val() : _arlaDoVeiculo.Ideal.val(),
            KMTotalAjustado: tipoAbastecimento == 1 ? _abastecimentoDoVeiculo.KMTotalAjustado.val() : "0",
            PercentalAjusteKM: tipoAbastecimento == 1 ? _abastecimentoDoVeiculo.PercentalAjusteKM.val() : "0"
        };
        executarReST("AcertoAbastecimento/SalvarMediaIdealAbastecimento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    if (tipoAbastecimento == 1) {
                        _abastecimentoDoVeiculo.Ideal.val(arg.Data.MediaIdeal);
                        _abastecimentoDoVeiculo.MediaFinal.val(arg.Data.MediaFinal);

                        var valorIdeal = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.Ideal.val()), "n2"))
                        var mediaFinal = parseFloat(formatarStrFloat(Globalize.format(_abastecimentoDoVeiculo.MediaFinal.val()), "n2"))

                        var valorAte = valorIdeal - 0.5;
                        if (valorAte < 0)
                            valorAte = 0;

                        if (isNaN(valorIdeal))
                            valorIdeal = 0;
                        if (isNaN(mediaFinal))
                            mediaFinal = 0;
                        if (isNaN(valorAte))
                            valorAte = 0;

                        _abastecimentoDoVeiculo.Aceitavel.text("Aceitável (" + Globalize.format(valorIdeal, "n2") + " até " + Globalize.format(valorAte, "n2") + ") ");
                        _abastecimentoDoVeiculo.Fora.text("Fora (abaixo de " + Globalize.format(valorAte, "n2") + ") ");

                        if (mediaFinal == 0)
                            $("#divMediaFinalAbastecimento").css("background-color", "#FFFFFF");
                        else if (mediaFinal >= valorIdeal)
                            $("#divMediaFinalAbastecimento").css("background-color", "#90EE90");
                        else if (mediaFinal >= (valorIdeal - 0.5) && mediaFinal < valorIdeal)
                            $("#divMediaFinalAbastecimento").css("background-color", "#FFFACD");
                        else
                            $("#divMediaFinalAbastecimento").css("background-color", "#FF8C69");
                    }
                    else
                        _arlaDoVeiculo.Ideal.val(arg.Data.MediaIdeal);

                    VerificaBotoesAutorizacaoAbastecimento(tipoAbastecimento, codigoVeiculo);
                }

            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    }
}

function CarregarArlaVeiculo(indice) {
    var idDiv = guid();
    $("#contentArlaDoVeiculo").html(_HTMLArlaDoVeiculo.replace("#knoutArlasDoVeiculo", idDiv + "_knoutArlasDoVeiculo"));

    _arlaDoVeiculo = new ArlaDoVeiculo();
    KoBindings(_arlaDoVeiculo, idDiv + "_knoutArlasDoVeiculo");

    var classActive = '"';
    if (_abastecimentoAcertoViagem.VeiculosArla.val().length > 1) {
        var html = "";
        $.each(_abastecimentoAcertoViagem.VeiculosArla.val(), function (i, veiculo) {
            if (i == indice)
                html += '<li class="active" class="nav-item">';
            else
                html += '<li>';
            if (i == indice)
                html += '<a href="javascript:void(0);" class="nav-link active" data-bs-toggle="tab" onclick="CarregarArlaVeiculo(' + i + ')"><span class="hidden-mobile hidden-tablet">' + veiculo.Placa + '</span></a>';
            else
                html += '<a href="javascript:void(0);" class="nav-link" data-bs-toggle="tab" onclick="CarregarArlaVeiculo(' + i + ')"><span class="hidden-mobile hidden-tablet">' + veiculo.Placa + '</span></a>';
            html += '</li>';
        });
        $("#" + _arlaDoVeiculo.Veiculo.idTab).html(html);
        $("#" + _arlaDoVeiculo.Veiculo.idTab).show();
    }

    if (_abastecimentoAcertoViagem.VeiculosArla.val().length > 0) {
        _arlaDoVeiculo.CodigoVeiculo.val(_abastecimentoAcertoViagem.VeiculosArla.val()[indice].Codigo);
        _arlaDoVeiculo.Placa.val(_abastecimentoAcertoViagem.VeiculosArla.val()[indice].Placa);
    }
    else
        _arlaDoVeiculo.CodigoVeiculo.val(0);
    _arlaDoVeiculo.CodigoAcerto.val(_acertoViagem.Codigo.val());
    _arlaDoVeiculo.Autorizar.visible(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarArla, _PermissoesPersonalizadas));

    var excluirArla = { descricao: "Excluir", id: guid(), metodo: RemoverArlaClick, icone: "" };
    var editarArla = { descricao: "Editar", id: guid(), metodo: EditarArlaClick, icone: "" };
    var auditarArla = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("AcertoAbastecimento", null, _novoAbastecimento), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [excluirArla, editarArla, auditarArla] };

    _arlaDoVeiculo.SelecionarTodos.visible(true);
    _arlaDoVeiculo.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: null,
        callbackNaoSelecionado: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _arlaDoVeiculo.SelecionarTodos,
        somenteLeitura: false
    };

    var headerAbastecimentoSelecao = [
        { data: "CodigoAcertoAbastecimento", visible: false },
        { data: "Codigo", visible: false },
        { data: "CodigoAcertoViagem", visible: false },
        { data: "CodigoVeiculo", visible: false },
        { data: "Combustivel", visible: false },
        { data: "Fornecedor", visible: false },
        { data: "DataAbastecimento", visible: false },
        { data: "Litros", visible: false },
        { data: "ValorUnitario", visible: false },
        { data: "ValorTotal", visible: false },
        { data: "TipoAbastecimento", visible: false },
        { data: "Kilometragem", visible: false },
        { data: "NumeroDocumento", visible: false },
        { data: "Equipamento", visible: false },
        { data: "Horimetro", visible: false },
        { data: "MoedaCotacaoBancoCentral", visible: false },
        { data: "DataBaseCRT", visible: false },
        { data: "ValorMoedaCotacao", visible: false },
        { data: "ValorOriginalMoedaEstrangeira", visible: false },
        { data: "CodigoFechamentoAbastecimento", visible: false }        
    ];

    _gridArlaSelecao = new BasicDataTable(_arlaDoVeiculo.Grid.id, headerAbastecimentoSelecao, null, { column: 0, dir: orderDir.asc });

    _gridArla = new GridView(_arlaDoVeiculo.Arlas.idGrid, "AcertoAbastecimento/PesquisarAbastecimento", _arlaDoVeiculo, menuOpcoes, null, null, null, null, null, multiplaescolha);

    new BuscarAbastecimentosSemAcertoDeViagem(_arlaDoVeiculo.Arlas, RetornoInserirAbastecimento, _gridArlaSelecao, _acertoViagem.Codigo, _arlaDoVeiculo.CodigoVeiculo, 2);
    _arlaDoVeiculo.Arlas.basicTable = _gridArlaSelecao;
    _gridArlaSelecao.CarregarGrid([]);

    _gridArla.CarregarGrid(AtualizarResumoArlaVeiculo(_arlaDoVeiculo.CodigoVeiculo.val()));
    VerificarBotoes();

    if (_abastecimentoAcertoViagem.VeiculosArla.val().length <= 0)
        $("#divArlaAbastecimento").hide();
    else
        $("#divArlaAbastecimento").show();
    //Global.ResetarAbas();
}

function AtualizarResumoArlaVeiculo(codigoVeiculo) {
    if (codigoVeiculo != null && codigoVeiculo > 0) {
        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoVeiculo: codigoVeiculo,
            TipoAbastecimento: 2,
            MediaIdeal: _arlaDoVeiculo.Ideal.val()
        };
        executarReST("AcertoAbastecimento/DadosAbastecimentoVeiculo", data, function (arg) {
            if (arg.Success) {
                _arlaDoVeiculo.Litros.val(arg.Data.Litros);
                _arlaDoVeiculo.ValorTotal.val(arg.Data.ValorTotal);
                _arlaDoVeiculo.MediaFinal.val(arg.Data.MediaFinal);
                _arlaDoVeiculo.Ideal.val(arg.Data.MediaIdeal);

                var valorIdeal = parseFloat(formatarStrFloat(Globalize.format(_arlaDoVeiculo.Ideal.val()), "n2"))
                var mediaFinal = parseFloat(formatarStrFloat(Globalize.format(_arlaDoVeiculo.MediaFinal.val()), "n2"))

                var valorAte = valorIdeal - 5;
                if (valorAte < 0)
                    valorAte = 0;

                if (isNaN(valorIdeal))
                    valorIdeal = 0;
                if (isNaN(mediaFinal))
                    mediaFinal = 0;
                if (isNaN(valorAte))
                    valorAte = 0;

                _arlaDoVeiculo.Aceitavel.text("Aceitável (" + Globalize.format(valorIdeal, "n2") + " até " + Globalize.format(valorAte, "n2") + ") ");
                _arlaDoVeiculo.Fora.text("Fora (abaixo de " + Globalize.format(valorAte, "n2") + ") ");

                if (mediaFinal == 0)
                    $("#divMediaFinalArla").css("background-color", "#FFFFFF");
                else if (mediaFinal >= valorIdeal)
                    $("#divMediaFinalArla").css("background-color", "#90EE90");
                else if (mediaFinal >= (valorIdeal - 5) && mediaFinal < valorIdeal)
                    $("#divMediaFinalArla").css("background-color", "#FFFACD");
                else
                    $("#divMediaFinalArla").css("background-color", "#FF8C69");
                VerificaBotoesAutorizacaoAbastecimento(2, codigoVeiculo);
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    }
}

function AlteraMediaIdealArla(e) {
    var valorIdeal = parseFloat(formatarStrFloat(Globalize.format(_arlaDoVeiculo.Ideal.val()), "n2"))
    var mediaFinal = parseFloat(formatarStrFloat(Globalize.format(_arlaDoVeiculo.MediaFinal.val()), "n2"))

    var valorAte = valorIdeal - 5;
    if (valorAte < 0)
        valorAte = 0;

    if (isNaN(valorIdeal))
        valorIdeal = 0;
    if (isNaN(mediaFinal))
        mediaFinal = 0;
    if (isNaN(valorAte))
        valorAte = 0;

    _arlaDoVeiculo.Aceitavel.text("Aceitável (" + Globalize.format(valorIdeal, "n2") + " até " + Globalize.format(valorAte, "n2") + ") ");
    _arlaDoVeiculo.Fora.text("Fora (abaixo de " + Globalize.format(valorAte, "n2") + ") ");

    if (mediaFinal == 0)
        $("#divMediaFinalArla").css("background-color", "#FFFFFF");
    else if (mediaFinal >= valorIdeal)
        $("#divMediaFinalArla").css("background-color", "#90EE90");
    else if (mediaFinal >= (valorIdeal - 0.5) && mediaFinal < valorIdeal)
        $("#divMediaFinalArla").css("background-color", "#FFFACD");
    else
        $("#divMediaFinalArla").css("background-color", "#FF8C69");

    AtualizarMediaIdealAbastecimentoVeiculo(_arlaDoVeiculo.CodigoVeiculo.val(), 2);
}

function RemoverArlasClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as arlas selecionadas?", function () {

        var abastecimentosSelecionados = _gridArla.ObterMultiplosSelecionados();

        var codigosAbastecimentos = new Array();
        for (var i = 0; i < abastecimentosSelecionados.length; i++)
            codigosAbastecimentos.push(abastecimentosSelecionados[i].Codigo);

        if (codigosAbastecimentos && codigosAbastecimentos.length > 0) {

            var data = {
                Codigos: JSON.stringify(codigosAbastecimentos)
            };
            executarReST("AcertoAbastecimento/RemoverArlaSelecionados", data, function (arg) {
                if (arg.Success) {
                    _gridArla.AtualizarRegistrosSelecionados([]);
                    _arlaDoVeiculo.Autorizar.enable(true);
                    _gridArla.CarregarGrid(AtualizarResumoArlaVeiculo(_arlaDoVeiculo.CodigoVeiculo.val()));
                    VerificaBotoesAutorizacaoAbastecimento(2, _arlaDoVeiculo.CodigoVeiculo.val());
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arlas removidas com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
                }
            });
        }
        else
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Nenhum abastecimento selecionado.");
    });
}

function RemoverAbastecimentoSelecionadosClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir os abastecimentos selecionados?", function () {

        var abastecimentosSelecionados = _gridAbastecimento.ObterMultiplosSelecionados();

        var codigosAbastecimentos = new Array();
        for (var i = 0; i < abastecimentosSelecionados.length; i++)
            codigosAbastecimentos.push(abastecimentosSelecionados[i].Codigo);

        if (codigosAbastecimentos && codigosAbastecimentos.length > 0) {

            var data = {
                Codigos: JSON.stringify(codigosAbastecimentos)
            };
            executarReST("AcertoAbastecimento/RemoverAbastecimentoSelecionados", data, function (arg) {
                if (arg.Success) {
                    _gridAbastecimento.AtualizarRegistrosSelecionados([]);
                    _abastecimentoDoVeiculo.Autorizar.enable(true);
                    _gridAbastecimento.CarregarGrid(AtualizarResumoAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val()));
                    VerificaBotoesAutorizacaoAbastecimento(1, _abastecimentoDoVeiculo.CodigoVeiculo.val());
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Abastecimentos removidos com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
                }
            });
        }
        else
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Nenhum abastecimento selecionado.");
    });
}

function InformarAbastecimentoClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    LimparCampos(_novoAbastecimento);
    _novoAbastecimento.TipoAbastecimento.val(1);
        
    Global.abrirModal('divAdicionarAbastecimento');
    _novoAbastecimento.VeiculoAbastecimento.enable(false);

    var data = { Codigo: _abastecimentoDoVeiculo.CodigoVeiculo.val(), TipoAbastecimento: 1 };
    executarReST("Veiculo/BuscaCombustivelPadrao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _novoAbastecimento.Combustivel.val(arg.Data.Descricao);
                _novoAbastecimento.Combustivel.codEntity(arg.Data.Codigo);
            }            
            if (arg.Data.TipoVeiculo != null && arg.Data.TipoVeiculo != undefined && arg.Data.TipoVeiculo == "Reboque") {
                _novoAbastecimento.Horimetro.visible(true);
                _novoAbastecimento.Equipamento.visible(true);
                _novoAbastecimento.Equipamento.codEntity(arg.Data.CodigoEquipamento);
                _novoAbastecimento.Equipamento.val(arg.Data.DescricaoEquipamento);
            }
            else {
                _novoAbastecimento.Horimetro.visible(true);
                _novoAbastecimento.Equipamento.visible(true);
                _novoAbastecimento.Equipamento.codEntity(arg.Data.CodigoEquipamento);
                _novoAbastecimento.Equipamento.val(arg.Data.DescricaoEquipamento);
            }

            _novoAbastecimento.Horimetro.val(0);
            _novoAbastecimento.Litros.val(0);
            _novoAbastecimento.ValorTotal.val(0);
            _novoAbastecimento.ValorUnitario.val(0);

            _novoAbastecimento.VeiculoAbastecimento.enable(false);
            _novoAbastecimento.VeiculoAbastecimento.codEntity(_abastecimentoDoVeiculo.CodigoVeiculo.val());
            _novoAbastecimento.VeiculoAbastecimento.val(_abastecimentoDoVeiculo.Placa.val());

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function RetornoAbastecimento(data) {
    _novoAbastecimento.Combustivel.codEntity(data.Codigo);
    _novoAbastecimento.Combustivel.val(data.Descricao);
    ConsultarValorAbastecimento();
}

function RetornoFornecedorAbastecimento(data) {
    _novoAbastecimento.Fornecedor.codEntity(data.Codigo);
    _novoAbastecimento.Fornecedor.val(data.Nome);
    ConsultarValorAbastecimento();
}

function ConsultarValorAbastecimento() {
    if (_novoAbastecimento.Combustivel.codEntity() != 0 && _novoAbastecimento.Combustivel.codEntity() != "") {
        if (_novoAbastecimento.Fornecedor.codEntity() != 0 && _novoAbastecimento.Fornecedor.codEntity() != "") {
            var data = { CodigoCombustivel: _novoAbastecimento.Combustivel.codEntity(), CodigoFornecedor: _novoAbastecimento.Fornecedor.codEntity() };
            executarReST("Pessoa/BuscarValorCombustivelTabela", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data != null) {
                        _novoAbastecimento.ValorUnitario.val(arg.Data.ValorUnitario);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        }
    }
}

function InformarArlaClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    LimparCampos(_novoArla);
    _novoArla.TipoAbastecimento.val(2);
    
    Global.abrirModal('divAdicionarArla');

    _novoArla.VeiculoAbastecimento.enable(false);

    var data = { Codigo: _arlaDoVeiculo.CodigoVeiculo.val(), TipoAbastecimento: 2 };
    executarReST("Veiculo/BuscaCombustivelAbastecimento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _novoArla.Combustivel.val(arg.Data.Descricao);
                _novoArla.Combustivel.codEntity(arg.Data.Codigo);
            }
            if (arg.Data.TipoVeiculo == "Reboque") {
                _novoArla.Horimetro.visible(true);
                _novoArla.Equipamento.visible(true);
            }
            else {
                _novoArla.Horimetro.visible(true);
                _novoArla.Equipamento.visible(true);
            }
            _novoArla.Horimetro.val(0);
            _novoArla.Litros.val(0);
            _novoArla.ValorTotal.val(0);
            _novoArla.ValorUnitario.val(0);

            _novoArla.VeiculoAbastecimento.enable(false);
            _novoArla.VeiculoAbastecimento.codEntity(_arlaDoVeiculo.CodigoVeiculo.val());
            _novoArla.VeiculoAbastecimento.val(_arlaDoVeiculo.Placa.val());

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function RetornoArla(data) {
    _novoArla.Combustivel.codEntity(data.Codigo);
    _novoArla.Combustivel.val(data.Descricao);
    ConsultarValorArla();
}

function RetornoFornecedorArla(data) {
    _novoArla.Fornecedor.codEntity(data.Codigo);
    _novoArla.Fornecedor.val(data.Nome);
    ConsultarValorArla();
}

function ConsultarValorArla() {
    if (_novoArla.Combustivel.codEntity() != 0 && _novoArla.Combustivel.codEntity() != "") {
        if (_novoArla.Fornecedor.codEntity() != 0 && _novoArla.Fornecedor.codEntity() != "") {
            var data = { CodigoCombustivel: _novoArla.Combustivel.codEntity(), CodigoFornecedor: _novoArla.Fornecedor.codEntity() };
            executarReST("Pessoa/BuscarValorCombustivelTabela", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data != null) {
                        _novoArla.ValorUnitario.val(arg.Data.ValorUnitario);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        }
    }
}

function AdicionarAbastecimentoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (ValidarCamposObrigatorios(e)) {
        var editando = e.Codigo.val() > 0;
        var codigoVeiculo = _abastecimentoDoVeiculo.CodigoVeiculo.val();
        if (editando)
            codigoVeiculo = _novoAbastecimento.VeiculoAbastecimento.codEntity();

        var abastecimentos = new Array();

        abastecimentos.push({
            CodigoAcertoAbastecimento: e.Codigo.val(),
            CodigoAcertoViagem: _acertoViagem.Codigo.val(),
            CodigoVeiculo: codigoVeiculo,
            Combustivel: e.Combustivel.codEntity(),
            Fornecedor: e.Fornecedor.codEntity(),
            DataAbastecimento: e.DataAbastecimento.val(),
            Litros: e.Litros.val(),
            ValorUnitario: e.ValorUnitario.val(),
            ValorTotal: e.ValorTotal.val(),
            TipoAbastecimento: 1,
            Kilometragem: e.Kilometragem.val(),
            NumeroDocumento: e.NumeroDocumento.val(),
            Equipamento: e.Equipamento.codEntity(),
            Horimetro: e.Horimetro.val(),
            MoedaCotacaoBancoCentral: e.MoedaCotacaoBancoCentral.val(),
            DataBaseCRT: e.DataBaseCRT.val(),
            ValorMoedaCotacao: e.ValorMoedaCotacao.val(),
            ValorOriginalMoedaEstrangeira: e.ValorOriginalMoedaEstrangeira.val(),
            CodigoFechamentoAbastecimento: 0,
            Codigo: 0
        });

        var dataEnvio = {
            Abastecimentos: JSON.stringify(abastecimentos)
        };

        executarReST("AcertoAbastecimento/InserirAbastecimento", dataEnvio, function (arg) {
            if (arg.Success) {
                LimparCampos(e);
                _novoAbastecimento.VeiculoAbastecimento.visible(true);
                _novoAbastecimento.VeiculoAbastecimento.enable(false);
                _novoAbastecimento.VeiculoAbastecimento.required(false);

                if (editando)
                    Global.fecharModal('divAdicionarAbastecimento');
                else {
                    LimparCampos(e);

                    e.Horimetro.val(0);
                    e.Litros.val(0);
                    e.ValorTotal.val(0);
                    e.ValorUnitario.val(0);
                    e.TipoAbastecimento.val(1);

                    $("#" + e.Fornecedor.id).focus();
                    _abastecimentoDoVeiculo.Autorizar.enable(true);

                    VerificaBotoesAutorizacaoAbastecimento(1, _abastecimentoDoVeiculo.CodigoVeiculo.val());

                    _novoAbastecimento.VeiculoAbastecimento.codEntity(_abastecimentoDoVeiculo.CodigoVeiculo.val());
                    _novoAbastecimento.VeiculoAbastecimento.val(_abastecimentoDoVeiculo.Placa.val());
                }

                _gridAbastecimento.CarregarGrid(AtualizarResumoAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val()));
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function AdicionarArlaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (ValidarCamposObrigatorios(e)) {
        var editando = e.Codigo.val() > 0;
        var codigoVeiculo = _arlaDoVeiculo.CodigoVeiculo.val();
        if (editando)
            codigoVeiculo = _novoArla.VeiculoAbastecimento.codEntity();

        var abastecimentos = new Array();

        abastecimentos.push({
            CodigoAcertoAbastecimento: e.Codigo.val(),
            CodigoAcertoViagem: _acertoViagem.Codigo.val(),
            CodigoVeiculo: codigoVeiculo,
            Combustivel: e.Combustivel.codEntity(),
            Fornecedor: e.Fornecedor.codEntity(),
            DataAbastecimento: e.DataAbastecimento.val(),
            Litros: e.Litros.val(),
            ValorUnitario: e.ValorUnitario.val(),
            ValorTotal: e.ValorTotal.val(),
            TipoAbastecimento: 2,
            Kilometragem: e.Kilometragem.val(),
            NumeroDocumento: e.NumeroDocumento.val(),
            Equipamento: e.Equipamento.codEntity(),
            Horimetro: e.Horimetro.val(),
            MoedaCotacaoBancoCentral: e.MoedaCotacaoBancoCentral.val(),
            DataBaseCRT: e.DataBaseCRT.val(),
            ValorMoedaCotacao: e.ValorMoedaCotacao.val(),
            ValorOriginalMoedaEstrangeira: e.ValorOriginalMoedaEstrangeira.val(),
            CodigoFechamentoAbastecimento: 0,
            Codigo: 0
        });

        var dataEnvio = {
            Abastecimentos: JSON.stringify(abastecimentos)
        };

        executarReST("AcertoAbastecimento/InserirAbastecimento", dataEnvio, function (arg) {
            if (arg.Success) {
                _novoArla.VeiculoAbastecimento.visible(true);
                _novoArla.VeiculoAbastecimento.enable(true);
                _novoArla.VeiculoAbastecimento.required(false);

                if (editando)
                    Global.fecharModal('divAdicionarArla');
                else {
                    LimparCampos(e);

                    e.Horimetro.val(0);
                    e.Litros.val(0);
                    e.ValorTotal.val(0);
                    e.ValorUnitario.val(0);
                    e.TipoAbastecimento.val(2);

                    $("#" + e.Fornecedor.id).focus();
                    _arlaDoVeiculo.Autorizar.enable(true);
                    VerificaBotoesAutorizacaoAbastecimento(2, _arlaDoVeiculo.CodigoVeiculo.val());

                    _novoArla.VeiculoAbastecimento.codEntity(_arlaDoVeiculo.CodigoVeiculo.val());
                    _novoArla.VeiculoAbastecimento.val(_arlaDoVeiculo.Placa.val());
                }

                _gridArla.CarregarGrid(AtualizarResumoArlaVeiculo(_arlaDoVeiculo.CodigoVeiculo.val()));
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function RemoverAbastecimentoClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o abastecimento " + e.Kilometragem + "?", function () {

        var data = {
            Codigo: e.Codigo
        };
        executarReST("AcertoAbastecimento/RemoverAbastecimento", data, function (arg) {
            if (arg.Success) {
                _abastecimentoDoVeiculo.Autorizar.enable(true);
                _gridAbastecimento.CarregarGrid(AtualizarResumoAbastecimentoVeiculo(_abastecimentoDoVeiculo.CodigoVeiculo.val()));
                VerificaBotoesAutorizacaoAbastecimento(1, _abastecimentoDoVeiculo.CodigoVeiculo.val());
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    });
}

function EditarAbastecimentoClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    LimparCampos(_novoAbastecimento);
    _novoAbastecimento.TipoAbastecimento.val(1);

    _novoAbastecimento.VeiculoAbastecimento.enable(true);
    _novoAbastecimento.VeiculoAbastecimento.visible(true);
    _novoAbastecimento.VeiculoAbastecimento.required(true);
    
    Global.abrirModal('divAdicionarAbastecimento');

    var data = {
        Codigo: e.Codigo,
        TipoAbastecimento: 1
    };

    executarReST("AcertoAbastecimento/PesquisarCombustivel", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                var dataAbastecimento = { Data: arg.Data };
                PreencherObjetoKnout(_novoAbastecimento, dataAbastecimento);
                VerificaBotoesAutorizacaoAbastecimento(1, _abastecimentoDoVeiculo.CodigoVeiculo.val());
                if (arg.Data.TipoVeiculo == "Reboque") {
                    _novoAbastecimento.Horimetro.visible(true);
                    _novoAbastecimento.Equipamento.visible(true);
                }
                else {
                    _novoAbastecimento.Horimetro.visible(true);
                    _novoAbastecimento.Equipamento.visible(true);
                }
                _novoAbastecimento.ValorMoedaCotacao.val(dataAbastecimento.ValorMoedaCotacao);

            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function RemoverArlaClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a arla " + e.Kilometragem + "?", function () {

        var data = {
            Codigo: e.Codigo
        };
        executarReST("AcertoAbastecimento/RemoverAbastecimento", data, function (arg) {
            if (arg.Success) {
                _arlaDoVeiculo.Autorizar.enable(true);
                _gridArla.CarregarGrid(AtualizarResumoArlaVeiculo(_arlaDoVeiculo.CodigoVeiculo.val()));
                VerificaBotoesAutorizacaoAbastecimento(2, _arlaDoVeiculo.CodigoVeiculo.val());
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    });
}

function EditarArlaClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    LimparCampos(_novoArla);
    _novoArla.TipoAbastecimento.val(2);

    _novoArla.VeiculoAbastecimento.enable(true);
    _novoArla.VeiculoAbastecimento.visible(true);
    _novoArla.VeiculoAbastecimento.required(true);

    Global.abrirModal('divAdicionarArla');

    var data = {
        Codigo: e.Codigo,
        TipoAbastecimento: 2
    };

    executarReST("AcertoAbastecimento/PesquisarCombustivel", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                var dataAbastecimento = { Data: arg.Data };
                PreencherObjetoKnout(_novoArla, dataAbastecimento);
                VerificaBotoesAutorizacaoAbastecimento(2, _arlaDoVeiculo.CodigoVeiculo.val());

                if (arg.Data.TipoVeiculo == "Reboque") {
                    _novoArla.Horimetro.visible(true);
                    _novoArla.Equipamento.visible(true);
                }
                else {
                    _novoArla.Horimetro.visible(true);
                    _novoArla.Equipamento.visible(true);
                }

                _novoArla.ValorMoedaCotacao.val(dataAbastecimento.ValorMoedaCotacao);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function LancarPedagioClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();

    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Abastecimentos);
    Salvar(_acertoViagem, "AcertoAbastecimento/AtualizarAbastecimento", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg, 100000);

                CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Abastecimentos);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function RetornarCargaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();

    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Acerto);
    Salvar(_acertoViagem, "AcertoAbastecimento/AtualizarAbastecimento", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg, 100000);

                $("#" + _etapaAcertoViagem.Etapa6.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa6.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa5.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa3.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa2.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa2.idTab + " .step").attr("class", "step lightgreen");
                $("#" + _etapaAcertoViagem.Etapa2.idTab).click();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);

}

function calculaLitrosAbascimento(e) {
    var litros = 0;
    var valorTotal = 0;
    var valorUnitario = 0;

    if (e.Litros.val() != null & e.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(e.Litros.val()), "n2")).toFixed(2);

    if (e.ValorUnitario.val() != null & e.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(e.ValorUnitario.val()), "n4")).toFixed(4);

    if (e.ValorTotal.val() != null & e.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(e.ValorTotal.val(), "n2"))).toFixed(2);

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        if (valorTotal > 0 && litros > 0) {
            valorUnitario = valorTotal / litros;
            e.ValorUnitario.val(Globalize.format(valorUnitario, "n4"));
        }
    }

    if (litros > 0) {
        if (valorUnitario > 0) {
            e.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        } else if (valorTotal > 0) {
            e.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    }
}

function calculaValorUnitarioAbascimento(e) {
    var litros = 0;
    var valorTotal = 0;
    var valorUnitario = 0;

    if (e.Litros.val() != null & e.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(e.Litros.val()), "n2")).toFixed(2);

    if (e.ValorUnitario.val() != null & e.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(e.ValorUnitario.val()), "n4")).toFixed(4);

    if (e.ValorTotal.val() != null & e.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(e.ValorTotal.val(), "n2"))).toFixed(2);

    //if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
    //    if (valorTotal > 0 && litros > 0) {
    //        valorUnitario = valorTotal / litros;
    //        e.ValorUnitario.val(Globalize.format(valorUnitario, "n4"));
    //    }
    //}

    if (valorUnitario > 0) {
        if (litros > 0) {
            e.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        }
    } else if (valorTotal > 0) {
        if (litros > 0) {
            e.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    }
}

function calculaValorTotalAbascimento(e) {
    var litros = 0;
    var valorTotal = 0;
    var valorUnitario = 0;   

    if (e.Litros.val() != null & e.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(e.Litros.val()), "n2")).toFixed(2);

    if (e.ValorUnitario.val() != null & e.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(e.ValorUnitario.val()), "n4")).toFixed(4);

    if (e.ValorTotal.val() != null & e.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(e.ValorTotal.val(), "n2"))).toFixed(2);

    //if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
    //    if (valorTotal > 0 && litros > 0) {
    //        valorUnitario = valorTotal / litros;
    //        e.ValorUnitario.val(Globalize.format(valorUnitario, "n4"));
    //    }
    //}

    if (valorTotal > 0) {
        if (litros > 0) {
            e.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    } else if (valorUnitario > 0) {
        if (litros > 0) {
            e.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        }
    }
}

//*******MÉTODOS*******

function CarregarAbastecimentos(data) {
    recarregarGridAbastecimentos();
}

function recarregarGridAbastecimentos() {
    CarregarVeiculosAbastecimento();
    CarregarAbastecimentoVeiculo(0);
    CarregarArlaVeiculo(0);
}

function carregarConteudosAbastecimentoHTML(calback) {
    $.get("Content/Static/Acerto/AbastecimentoAcertoViagem.html?dyn=" + guid(), function (data) {
        _HTMLAbastecimentoAcertoViagem = data;
        $.get("Content/Static/Acerto/AbastecimentoDoVeiculo.html?dyn=" + guid(), function (data) {
            _HTMLAbastecimentoDoVeiculo = data;
            $.get("Content/Static/Acerto/ArlaDoVeiculo.html?dyn=" + guid(), function (data) {
                _HTMLArlaDoVeiculo = data;
                $.get("Content/Static/Acerto/OcorrenciaAcertoViagem.html?dyn=" + guid(), function (data) {
                    _HTMLOcorrenciaAcertoViagem = data;
                    loadOcorrenciaAcertoViagem();
                    calback();
                });
            });
        });
    });
}

function formatarStrFloat(valor) {
    valor = valor.replace(".", "");
    return valor.replace(",", ".");
}

function VerificaBotoesAutorizacaoAbastecimento(tipoAbastecimento, codigoVeiculo) {
    var data = {
        Codigo: _acertoViagem.Codigo.val(),
        CodigoVeiculo: codigoVeiculo,
        TipoAbastecimento: tipoAbastecimento
    };
    executarReST("AcertoAbastecimento/ContemAbastecimentoPendenteAutorizacao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                if (arg.Data.ContemResumoPendente) {
                    if (tipoAbastecimento == 2)
                        _arlaDoVeiculo.Autorizar.visible(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarArla, _PermissoesPersonalizadas));
                    else
                        _abastecimentoDoVeiculo.Autorizar.visible(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarAbastecimento, _PermissoesPersonalizadas));
                } else {
                    if (tipoAbastecimento == 2)
                        _arlaDoVeiculo.Autorizar.visible(false);
                    else
                        _abastecimentoDoVeiculo.Autorizar.visible(false);
                }
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}


function CalcularMoedaEstrangeiraAbastecimento() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _novoAbastecimento.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _novoAbastecimento.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _novoAbastecimento.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValorAbastecimento();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorAbastecimento() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira && _novoAbastecimento.ValorMoedaCotacao.val() != undefined && _novoAbastecimento.ValorOriginalMoedaEstrangeira.val() != undefined) {
        var valorMoedaCotacao = Globalize.parseFloat(_novoAbastecimento.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novoAbastecimento.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novoAbastecimento.ValorTotal.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorOriginalAbastecimento() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novoAbastecimento.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novoAbastecimento.ValorTotal.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novoAbastecimento.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}

function CalcularMoedaEstrangeiraArla() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _novoArla.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _novoArla.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _novoArla.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValorArla();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorArla() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira && _novoArla.ValorMoedaCotacao.val() != undefined && _novoArla.ValorOriginalMoedaEstrangeira.val() != undefined) {
        var valorMoedaCotacao = Globalize.parseFloat(_novoArla.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novoArla.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novoArla.ValorTotal.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorOriginalArla() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novoArla.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novoArla.ValorTotal.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novoArla.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}