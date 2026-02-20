/// <reference path="MovimentacaoPneu.js" />
/// <reference path="MovimentacaoPneuReformaParaEstoqueProduto.js" />
/// <reference path="../../Consultas/Almoxarifado.js" />
/// <reference path="../../Consultas/BandaRodagemPneu.js" />
/// <reference path="../../Enumeradores/EnumServicoRealizadoPneu.js" />
/// <reference path="../../Enumeradores/EnumVidaPneu.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _isMovimentacaoPneuReformaParaEstoqueSalva;
var _movimentacaoPneuReformaParaEstoque;
var _servicoRealizadoAnterior = EnumServicoRealizadoPneu.Conserto;

/*
 * Declaração das Classes
 */

var MovimentacaoPneuReformaParaEstoque = function () {
    this.CodigoPneu = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });    
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Almoxarifado:", idBtnSearch: guid(), required: true });
    this.BandaRodagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Banda Rodagem:"), idBtnSearch: guid(), required: false });
    this.DataHora = PropertyEntity({ text: "*Data/Hora: ", getType: typesKnockout.dateTime, required: true });
    this.NumeroFogo = PropertyEntity({ text: "*Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500, required: true, enable: false });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.ResponsavelOrcamento = PropertyEntity({ text: "Orçado Por:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.ServicoRealizado = PropertyEntity({ val: ko.observable(EnumServicoRealizadoPneu.Conserto), def: EnumServicoRealizadoPneu.Conserto, options: EnumServicoRealizadoPneu.obterOpcoes(), text: "*Serviço Realizado:", enable: ko.observable(true) });
    this.SulcoAnterior = PropertyEntity({ text: "*Sulco Anterior", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, required: true, enable: false });
    this.SulcoAtual = PropertyEntity({ text: "*Sulco Atual", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, required: true });
    this.ValorMaoObra = PropertyEntity({ text: "*Valor em Mão de Obra:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true });
    this.ValorProdutos = PropertyEntity({ text: ko.observable("Valor em Produtos:"), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true }, maxlength: 10, required: false });
    this.ValorResidualAtualPneu = PropertyEntity({ text: "Valor Residual Atual do Pneu:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: false, enable: false });
    this.Vida = PropertyEntity({ val: ko.observable(EnumVidaPneu.PneuNovo), def: EnumVidaPneu.PneuNovo, options: EnumVidaPneu.obterOpcoes(), text: "*Vida:", enable: false });

    this.ServicoRealizado.val.subscribe(atualizarVidaPneu);
    this.ValorMaoObra.val.subscribe(atualizarValorResidualAtualPneu);
    this.ValorProdutos.val.subscribe(atualizarValorResidualAtualPneu);
    
    this.ServicoRealizado.val.subscribe(function (novoValor) {
        if (novoValor == EnumServicoRealizadoPneu.Conserto) {            
            _movimentacaoPneuReformaParaEstoque.BandaRodagem.required = false;
            _movimentacaoPneuReformaParaEstoque.BandaRodagem.text("Banda Rodagem:");
            _movimentacaoPneuReformaParaEstoque.ValorProdutos.required = false;
            _movimentacaoPneuReformaParaEstoque.ValorProdutos.text("Valor em Produtos:");
        } else {
            _movimentacaoPneuReformaParaEstoque.BandaRodagem.required = true;
            _movimentacaoPneuReformaParaEstoque.BandaRodagem.text("*Banda Rodagem:");
            _movimentacaoPneuReformaParaEstoque.ValorProdutos.required = true;
            _movimentacaoPneuReformaParaEstoque.ValorProdutos.text("*Valor em Produtos:");
        }
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMovimentacaoPneuReformaParaEstoqueClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
    this.AdicionarProdutos = PropertyEntity({ eventClick: adicionarProdutoClick, type: types.event, text: ko.observable("Adicionar Produtos"), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadMovimentacaoPneuReformaParaEstoque() {
    _movimentacaoPneuReformaParaEstoque = new MovimentacaoPneuReformaParaEstoque();
    KoBindings(_movimentacaoPneuReformaParaEstoque, "knockoutMovimentacaoPneuReformaParaEstoque");

    new BuscarAlmoxarifado(_movimentacaoPneuReformaParaEstoque.Almoxarifado);
    new BuscarBandaRodagemPneu(_movimentacaoPneuReformaParaEstoque.BandaRodagem);

    loadMovimentacaoPneuReformaParaEstoqueProduto();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMovimentacaoPneuReformaParaEstoqueClick() {
    if (ValidarCamposObrigatorios(_movimentacaoPneuReformaParaEstoque)) {
        var reformaParaEstoque = RetornarObjetoPesquisa(_movimentacaoPneuReformaParaEstoque);

        reformaParaEstoque.Produtos = obterMovimentacaoPneuReformaParaEstoqueProdutos();

        executarReST("MovimentacaoPneu/EnviarReformaParaEstoque", reformaParaEstoque, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _isMovimentacaoPneuReformaParaEstoqueSalva = true;

                    fecharModalMovimentacaoPneuReformaParaEstoque();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function adicionarProdutoClick() {
    adicionarMovimentacaoPneuReformaParaEstoqueProduto();
}

/*
 * Declaração das Funções Públicas
 */

function exibirModalMovimentacaoPneuReformaParaEstoque() {
    preencherMovimentacaoPneuReformaParaEstoque();

    _isMovimentacaoPneuReformaParaEstoqueSalva = false;

    Global.abrirModal('divModalMovimentacaoPneuReformaParaEstoque');
    $("#divModalMovimentacaoPneuReformaParaEstoque").one('hidden.bs.modal', function () {
        if (_isMovimentacaoPneuReformaParaEstoqueSalva) {
            atualizarDadosPneuReformaParaEstoque();
            adicionarPneuEstoque(_pneuAdicionar);
            efetivarMovimentacaoPneu();
        }
        else
            reverterMovimentacaoPneu();

        LimparCampos(_movimentacaoPneuReformaParaEstoque);
        limparProdutos();
        limparCamposMovimentacaoPneu();
    });
}

/*
 * Declaração das Funções Privadas
 */

function atualizarDadosPneuReformaParaEstoque() {
    _pneuAdicionar.Almoxarifado.codEntity(_movimentacaoPneuReformaParaEstoque.Almoxarifado.codEntity());
    _pneuAdicionar.Almoxarifado.val(_movimentacaoPneuReformaParaEstoque.Almoxarifado.val());
    _pneuAdicionar.BandaRodagem.codEntity(_movimentacaoPneuReformaParaEstoque.BandaRodagem.codEntity());
    _pneuAdicionar.BandaRodagem.val(_movimentacaoPneuReformaParaEstoque.BandaRodagem.val());
    _pneuAdicionar.Sulco.val(_movimentacaoPneuReformaParaEstoque.SulcoAtual.val());
    _pneuAdicionar.Vida.val(EnumVidaPneu.obterDescricao(_movimentacaoPneuReformaParaEstoque.Vida.val()));
    _pneuAdicionar.ValorMaoObra.val(_movimentacaoPneuReformaParaEstoque.ValorMaoObra.val());
    _pneuAdicionar.ValorProdutos.val(_movimentacaoPneuReformaParaEstoque.ValorProdutos.val());

}

function atualizarValorResidualAtualPneu() {
    _movimentacaoPneuReformaParaEstoque.ValorResidualAtualPneu.val(obterValorResidualAtualPneu());
}

function atualizarVidaPneu(servicoRealizado) {
    if (servicoRealizado == EnumServicoRealizadoPneu.Conserto)
        _movimentacaoPneuReformaParaEstoque.Vida.val(_movimentacaoPneuReformaParaEstoque.Vida.val() - 1);
    else if (_servicoRealizadoAnterior == EnumServicoRealizadoPneu.Conserto)
        _movimentacaoPneuReformaParaEstoque.Vida.val(_movimentacaoPneuReformaParaEstoque.Vida.val() + 1);

    _servicoRealizadoAnterior = servicoRealizado;
}

function fecharModalMovimentacaoPneuReformaParaEstoque() {
    Global.fecharModal('divModalMovimentacaoPneuReformaParaEstoque');
}

function obterValorResidualAtualPneu() {
    var valorMaoObra = parseFloat(_movimentacaoPneuReformaParaEstoque.ValorMaoObra.val().replace(/\./g, "").replace(",", "."));

    if (isNaN(valorMaoObra))
        return "";

    var valorProdutos = parseFloat(_movimentacaoPneuReformaParaEstoque.ValorProdutos.val().replace(/\./g, "").replace(",", "."));

    if (isNaN(valorProdutos))
        return "";

    return Globalize.format(valorMaoObra + valorProdutos, "n2");
}

function preencherMovimentacaoPneuReformaParaEstoque() {
    _movimentacaoPneuReformaParaEstoque.CodigoPneu.val(_pneuAdicionar.CodigoPneu.val());
    _movimentacaoPneuReformaParaEstoque.Almoxarifado.codEntity(_pneuAdicionar.Almoxarifado.codEntity());
    _movimentacaoPneuReformaParaEstoque.Almoxarifado.val(_pneuAdicionar.Almoxarifado.val());
    _movimentacaoPneuReformaParaEstoque.BandaRodagem.codEntity(_pneuAdicionar.BandaRodagem.codEntity());
    _movimentacaoPneuReformaParaEstoque.BandaRodagem.val(_pneuAdicionar.BandaRodagem.val());
    _movimentacaoPneuReformaParaEstoque.DataHora.val(Global.DataHoraAtual());
    _movimentacaoPneuReformaParaEstoque.NumeroFogo.val(_pneuAdicionar.NumeroFogo.val());
    _movimentacaoPneuReformaParaEstoque.SulcoAnterior.val(_pneuAdicionar.Sulco.val());
    _movimentacaoPneuReformaParaEstoque.Vida.val(EnumVidaPneu.obterValorPorDescricao(_pneuAdicionar.Vida.val()));
    _movimentacaoPneuReformaParaEstoque.ValorMaoObra.val(_pneuAdicionar.ValorMaoObra.val());
    _movimentacaoPneuReformaParaEstoque.ValorProdutos.val(_pneuAdicionar.ValorProdutos.val());
}
