/// <reference path="MovimentacaoPneu.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ServicoVeiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _isMovimentacaoPneuParaReformaSalva;
var _movimentacaoPneuParaReforma;

/*
 * Declaração das Classes
 */

var MovimentacaoPneuParaReforma = function () {
    this.CodigoEixoPneuOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoEstepeOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPneu = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.OrigemPneuDoEstoque = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.CustoEstimado = PropertyEntity({ text: "*Custo Estimado:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true });
    this.DataHora = PropertyEntity({ text: "*Data/Hora: ", getType: typesKnockout.dateTime, required: true });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.Hodometro = PropertyEntity({ text: "*Hodometro (Km):", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, required: true, enable: ko.observable(true) });
    this.NumeroFogo = PropertyEntity({ text: "*Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500, required: true, enable: false });
    this.TipoOrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de O.S.:", idBtnSearch: guid() });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.ServicoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço do Veículo:", idBtnSearch: guid(), required: true });
    this.SulcoAnterior = PropertyEntity({ text: "*Sulco Anterior:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true, enable: false });
    this.SulcoAtual = PropertyEntity({ text: "*Sulco Atual:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMovimentacaoPneuParaReformaClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadMovimentacaoPneuParaReforma() {
    _movimentacaoPneuParaReforma = new MovimentacaoPneuParaReforma();
    KoBindings(_movimentacaoPneuParaReforma, "knockoutMovimentacaoPneuParaReforma");

    new BuscarClientes(_movimentacaoPneuParaReforma.Fornecedor);
    new BuscarServicoVeiculo(_movimentacaoPneuParaReforma.ServicoVeiculo);
    new BuscarTipoOrdemServico(_movimentacaoPneuParaReforma.TipoOrdemServico);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMovimentacaoPneuParaReformaClick() {
    _movimentacaoPneuParaReforma.CodigoVeiculo.val(_veiculo.Codigo.val());
    Salvar(_movimentacaoPneuParaReforma, "MovimentacaoPneu/EnviarParaReforma", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _isMovimentacaoPneuParaReformaSalva = true;

                fecharModalMovimentacaoPneuParaReforma();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirModalMovimentacaoPneuParaReforma() {
    preencherMovimentacaoPneuParaReforma();

    _isMovimentacaoPneuParaReformaSalva = false;

    Global.abrirModal('divModalMovimentacaoPneuParaReforma');
    $("#divModalMovimentacaoPneuParaReforma").one('hidden.bs.modal', function () {
        if (_isMovimentacaoPneuParaReformaSalva) {
            adicionarPneuReforma(_pneuAdicionar);
            efetivarMovimentacaoPneu();
        }
        else
            reverterMovimentacaoPneu();

        LimparCampos(_movimentacaoPneuParaReforma);
        limparCamposMovimentacaoPneu();
    });
}

/*
 * Declaração das Funções Privadas
 */

function fecharModalMovimentacaoPneuParaReforma() {
    Global.fecharModal('divModalMovimentacaoPneuParaReforma');
}

function preencherMovimentacaoPneuParaReforma() {
    _movimentacaoPneuParaReforma.CodigoPneu.val(_pneuAdicionar.CodigoPneu.val());
    _movimentacaoPneuParaReforma.DataHora.val(Global.DataHoraAtual());
    _movimentacaoPneuParaReforma.NumeroFogo.val(_pneuAdicionar.NumeroFogo.val());
    _movimentacaoPneuParaReforma.SulcoAnterior.val(_pneuAdicionar.Sulco.val());

    if (_tipoContainerComPneu != EnumTipoContainerPneu.Estoque)
        _movimentacaoPneuParaReforma.Hodometro.val(_dadosVeiculo.Hodometro.val());
    else
        _movimentacaoPneuParaReforma.Hodometro.val(0);

    _movimentacaoPneuParaReforma.OrigemPneuDoEstoque.val(_tipoContainerComPneu == EnumTipoContainerPneu.Estoque);
    _movimentacaoPneuParaReforma.Hodometro.enable(false);

    if (_tipoContainerComPneu == EnumTipoContainerPneu.Estepe)
        _movimentacaoPneuParaReforma.CodigoEstepeOrigem.val(_pneuRemover.Codigo.val());
    else if (_tipoContainerComPneu == EnumTipoContainerPneu.Veiculo)
        _movimentacaoPneuParaReforma.CodigoEixoPneuOrigem.val(_pneuRemover.Codigo.val());
}
