/// <reference path="../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/ClassificacaoRiscoONU.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/LinhaSeparacao.js" />
/// <reference path="../../Consultas/EnderecosProduto.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="Pedido.js" />
/// <reference path="Adicional.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _produtoEmbarcador;
var _novoProdutoEmbarcador;
var _gridProduto;
var _gridONU;
var editandoProduto = false;



var Produto = function () {
    this.ExibirValorUnitarioDoProduto = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiIdDemandaProdutos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.ProdutosEmbarcador = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.ONUsProdutosEmbarcador = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.AdicionarProdutosEmbarcador = PropertyEntity({ eventClick: adicionarProdutoEmbarcadorClick, type: types.event, text: ko.observable(Localization.Resources.Pedidos.Pedido.AdicionarProduto) });
};

var AdicionarProdutoEmbarcador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "" });
    this.NumeroONU = PropertyEntity({ val: ko.observable(""), def: "" });
    this.ClasseRiscoONU = PropertyEntity({ val: ko.observable(""), def: "" });
    this.RiscoSubsidiarioONU = PropertyEntity({ val: ko.observable(""), def: "" });
    this.NumeroRiscoONU = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoLinhaSeparacao = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoEnderecoProduto = PropertyEntity({ val: ko.observable(""), def: "" });

    this.CodigoIntegracaoArmazem = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoFilialEmbarcador = PropertyEntity({ val: ko.observable(""), def: "" });

    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.ProdutoEmbarcador.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.SiglaUnidade = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.SiglaUnidade.getFieldDescription(), required: false, visible: true });
    this.CodigoProdutoEmbarcadorIntegracao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CodigoIntegracao.getFieldDescription(), enable: false, required: false, visible: true, val: ko.observable("") });
    this.Quantidade = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Quantidade.getFieldDescription(), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true }, val: ko.observable("0,000"), maxlength: 10, def: "0,000" });
    this.QuantidadePlanejada = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.QuantidadePlanejada.getFieldDescription(), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true }, val: ko.observable("0,000"), maxlength: 10, def: "0,000" });

    this.AlturaCM = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.AlturaUnitario.getFieldDescription(), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 6, allowZero: true }, val: ko.observable(0.00), maxlength: 10 });
    this.LarguraCM = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.LarguraUnitario.getFieldDescription(), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 6, allowZero: true }, val: ko.observable(0.00), maxlength: 10 });
    this.ComprimentoCM = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ComprimentoUnitario.getFieldDescription(), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 6, allowZero: true }, val: ko.observable(0.00), maxlength: 10 });
    this.MetrosCubico = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.MetrosCubicoUnitario.getFieldDescription(), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 6, allowZero: true }, val: ko.observable(0.00), maxlength: 10 });

    this.Peso = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PesoUnitario.getFieldDescription(), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true }, val: ko.observable(0.00), maxlength: 10 });
    this.ValorUnitario = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true }, val: ko.observable(0.00), maxlength: 10 });
    this.QuantidadePalet = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true }, text: Localization.Resources.Pedidos.Pedido.NumeroPalletsUnitario.getFieldDescription(), required: false });
    this.PalletFechado = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PalletFechado.getFieldDescription(), val: ko.observable(true), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false });
    this.ObservacaoProdutoEmbarcador = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Observacao.getFieldDescription(), maxlength: 200, visible: true, required: false });
    this.Setor = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Setor.getFieldDescription(), maxlength: 200, visible: true, required: false });
    this.Organizacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Organizacao.getFieldDescription(), maxlength: 200, visible: true, required: false });
    this.Canal = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Canal.getFieldDescription(), maxlength: 200, visible: true, required: false });

    this.LinhaSeparacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.LinhaSeparacao.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.EnderecoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.EnderecoProduto.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Armazem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Armazem.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarCadastroArmazem) });
    this.QuantidadeUnidadePorCaixa = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.QuantidadeUnidadePorCaixa.getFieldDescription(), required: ko.observable(false), visible: ko.observable(true) });
    this.QuantidadeCaixaPorPallet = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.QuantidadeCaixaPorPallet.getFieldDescription(), required: ko.observable(false), visible: ko.observable(true) });
    this.UnidadeMedidaSecundaria = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.UnidadeMedidaSecundaria.getFieldDescription(), required: false, visible: true });
    this.QuantidadeSecundaria = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.QuantidadeSecundaria.getFieldDescription(), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true } });

    this.PrecoUnitario = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,000"), def: "0,000", text: Localization.Resources.Pedidos.Pedido.PrecoUnitario.getFieldDescription(), required: ko.observable(false), visible: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });

    this.ClassificacaoRiscoONU = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.ClassificacaoONU.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.ObservacaoONU = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Observacao.getFieldDescription(), maxlength: 200, visible: true, required: false, val: ko.observable("") });
    this.ONUs = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.AdicionarONU = PropertyEntity({ eventClick: adicionarONUClick, type: types.event, text: ko.observable(Localization.Resources.Pedidos.Pedido.AdicionarONU.getFieldDescription()), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarProdutoEmbarcadorClick, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
    this.CamposPersonalizados = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CamposPersonalizados.getFieldDescription(), visible: true, required: false });
};

//*******EVENTOS*******

function loadProduto() {
    _produtoEmbarcador = new Produto();
    KoBindings(_produtoEmbarcador, "knockoutProdutos");

    _novoProdutoEmbarcador = new AdicionarProdutoEmbarcador();
    KoBindings(_novoProdutoEmbarcador, "knoutAdicionarProdutoEmbarcador");

    new BuscarProdutos(_novoProdutoEmbarcador.ProdutoEmbarcador, RetornoProdutoEmbarcadorProduto, _pedido.GrupoPessoa, _pedido.Remetente);
    new BuscarClassificacaoRiscoONU(_novoProdutoEmbarcador.ClassificacaoRiscoONU, RetornoClassificacaoRiscoONU);
    new BuscarLinhasSeparacao(_novoProdutoEmbarcador.LinhaSeparacao);
    new BuscarEnderecosProduto(_novoProdutoEmbarcador.EnderecoProduto);
    new BuscarArmazem(_novoProdutoEmbarcador.Armazem, RetornoArmazem, null, _pedido.Filial);

    $("#tabProdutos").hide();

    _novoProdutoEmbarcador.Codigo.val(guid());
    loadGridProdutos();
    loadGridONUs();
}

function calcularMetroCubico(e, sender) {
    var fatorConversao = _CONFIGURACAO_TMS.FatorMetroCubicoProdutoEmbarcadorIntegracao;
    if (fatorConversao == 0)
        fatorConversao = 1;

    if (Globalize.parseFloat(_novoProdutoEmbarcador.AlturaCM.val()) > 0 && Globalize.parseFloat(_novoProdutoEmbarcador.LarguraCM.val()) > 0 && Globalize.parseFloat(_novoProdutoEmbarcador.ComprimentoCM.val()) > 0) {
        var volume = Globalize.parseFloat((_novoProdutoEmbarcador.AlturaCM.val()) * Globalize.parseFloat(_novoProdutoEmbarcador.LarguraCM.val()) * Globalize.parseFloat(_novoProdutoEmbarcador.ComprimentoCM.val()) / fatorConversao);
        _novoProdutoEmbarcador.MetrosCubico.val((volume.toFixed(3).replace(".", ",")));
    } else {
        _novoProdutoEmbarcador.MetrosCubico.val("0.000");
    }
}

function RetornoClassificacaoRiscoONU(data) {
    if (data != null) {
        _novoProdutoEmbarcador.ClassificacaoRiscoONU.codEntity(data.Codigo);
        _novoProdutoEmbarcador.ClassificacaoRiscoONU.val(data.Descricao);

        _novoProdutoEmbarcador.NumeroONU.val(data.NumeroONU);
        _novoProdutoEmbarcador.ClasseRiscoONU.val(data.ClasseRisco);
        _novoProdutoEmbarcador.RiscoSubsidiarioONU.val(data.RiscoSubsidiario);
        _novoProdutoEmbarcador.NumeroRiscoONU.val(data.NumeroRisco);
    }
}

function RetornoArmazem(data) {
    _novoProdutoEmbarcador.Armazem.codEntity(data.Codigo);
    _novoProdutoEmbarcador.Armazem.val(data.Descricao);
    _novoProdutoEmbarcador.CodigoIntegracaoArmazem.val(data.CodigoIntegracao);
    _novoProdutoEmbarcador.CodigoFilialEmbarcador.val(data.CodigoFilialEmbarcador);
}


function RetornoProdutoEmbarcadorProduto(data) {
    if (data != null) {
        _novoProdutoEmbarcador.ProdutoEmbarcador.val(data.Descricao);
        _novoProdutoEmbarcador.ProdutoEmbarcador.codEntity(data.Codigo);
        _novoProdutoEmbarcador.CodigoProdutoEmbarcadorIntegracao.val(data.CodigoProdutoEmbarcador);
        _novoProdutoEmbarcador.SiglaUnidade.val(data.SiglaUnidade);

        if (_novoProdutoEmbarcador.Peso.val() == "" || parseFloat(_novoProdutoEmbarcador.Peso.val()) == 0)
            _novoProdutoEmbarcador.Peso.val(data.PesoUnitario);

        _novoProdutoEmbarcador.AlturaCM.val(data.AlturaCM);
        _novoProdutoEmbarcador.LarguraCM.val(data.LarguraCM);
        _novoProdutoEmbarcador.ComprimentoCM.val(data.ComprimentoCM);

        if (_novoProdutoEmbarcador.MetrosCubico.val() == "" || parseFloat(_novoProdutoEmbarcador.MetrosCubico.val()) == 0)
            _novoProdutoEmbarcador.MetrosCubico.val(data.MetroCubito);

        _novoProdutoEmbarcador.QuantidadeUnidadePorCaixa.val(data.QuantidadeUnidadePorCaixa);
        _novoProdutoEmbarcador.UnidadeMedidaSecundaria.val(data.UnidadeMedidaSecundaria);
        _novoProdutoEmbarcador.QuantidadeSecundaria.val(data.QuantidadeSecundaria);
        _novoProdutoEmbarcador.CodigoLinhaSeparacao.val(data.CodigoLinhaSeparacao);
        _novoProdutoEmbarcador.CodigoEnderecoProduto.val(data.CodigoEnderecoProduto);
        _novoProdutoEmbarcador.QuantidadeCaixaPorPallet.val(data.QuantidadeCaixaPorPallet);
        var qtdPalets = Globalize.parseFloat(data.QtdPalet);
        qtdPalets = qtdPalets.toFixed(0).replace(".", ",");

        if (_novoProdutoEmbarcador.QuantidadePalet.val() == "" || parseFloat(_novoProdutoEmbarcador.QuantidadePalet.val()) == 0)
            _novoProdutoEmbarcador.QuantidadePalet.val(qtdPalets);

        _novoProdutoEmbarcador.PalletFechado.val(data.PalletFechado);
        _novoProdutoEmbarcador.LinhaSeparacao.val(data.LinhaSeparacao.Descricao);
        _novoProdutoEmbarcador.LinhaSeparacao.codEntity(data.LinhaSeparacao.Codigo);

        if (_novoProdutoEmbarcador.MetrosCubico.val() == "")
            calcularMetroCubico();

        if (data.CodigoClassificacaoRiscoONU > 0) {

            var map = new Object();

            map.Codigo = guid();
            map.CodigoClassificacaoRiscoONU = data.CodigoClassificacaoRiscoONU;
            map.CodigoProdutoEmbarcador = _novoProdutoEmbarcador.Codigo.val();
            map.Descricao = data.DescricaoClassificacaoRiscoONU;
            map.Numero = data.NumeroONU;
            map.ClasseRisco = data.ClasseRisco;
            map.RiscoSubsidiario = data.RiscoSubsidiario;
            map.NumeroRisco = data.NumeroRisco;
            map.Observacao = "";

            _novoProdutoEmbarcador.ONUs.list.push(map);

            recarregarGridONUs();
        }
    }
}

function adicionarProdutoEmbarcadorClick(e, sender) {
    LimparCampos(_novoProdutoEmbarcador);
    resetarTabProduto();
    editandoProduto = false;
    recarregarGridONUs();

    _novoProdutoEmbarcador.Codigo.val(guid());
    _novoProdutoEmbarcador.Adicionar.text(Localization.Resources.Gerais.Geral.Adicionar);
    Global.abrirModal('divAdicionarProdutoEmbarcador');
}

function editarProduto(e) {
    $.each(_produtoEmbarcador.ProdutosEmbarcador.list, function (i, di) {
        if (di != null && di.Codigo != null && e != null && e.Codigo != null && e.Codigo == di.Codigo) {
            var quantidade = Globalize.parseFloat(di.Quantidade);
            if (isNaN(quantidade) || quantidade == undefined)
                quantidade = 0;

            var qtdPalets = Globalize.parseFloat(_pedido.PalletsFracionado.val());
            if (isNaN(qtdPalets) || qtdPalets == undefined)
                qtdPalets = 0;
            if (qtdPalets > 0 && Globalize.parseFloat(di.QuantidadePalets) > 0)
                qtdPalets -= (Globalize.parseFloat(Globalize.parseFloat(di.QuantidadePalets).toFixed(3).replace(".", ",")));

            var peso = Globalize.parseFloat(_adicional.PesoTotalCargaTMS.val());
            if (isNaN(peso) || peso == undefined)
                peso = 0;
            if (peso > 0 && Globalize.parseFloat(di.Peso) > 0 && quantidade > 0)
                peso -= (Globalize.parseFloat(Globalize.parseFloat(di.Peso).toFixed(2).replace(".", ",")) * quantidade);

            var cubagem = Globalize.parseFloat(_adicional.CubagemTotalTMS.val());
            if (isNaN(cubagem) || cubagem == undefined)
                cubagem = 0;
            if (cubagem > 0 && Globalize.parseFloat(di.MetrosCubico) > 0 && quantidade > 0)
                cubagem -= (Globalize.parseFloat(Globalize.parseFloat(di.MetrosCubico).toFixed(2).replace(".", ",")) * quantidade);

            if (cubagem < 0)
                cubagem = 0;
            if (peso < 0)
                peso = 0;
            if (qtdPalets < 0)
                qtdPalets = 0;

            _pedido.PalletsFracionado.val(((qtdPalets).toFixed(3).replace(".", ",")));
            _adicional.PesoTotalCargaTMS.val(((peso).toFixed(2).replace(".", ",")));
            _adicional.CubagemTotalTMS.val(((cubagem).toFixed(2).replace(".", ",")));

            LimparCampos(_novoProdutoEmbarcador);
            resetarTabProduto();
            _novoProdutoEmbarcador.Codigo.val(di.Codigo);
            _novoProdutoEmbarcador.Quantidade.val(di.Quantidade);
            _novoProdutoEmbarcador.QuantidadePlanejada.val(di.QuantidadePlanejada);
            _novoProdutoEmbarcador.CodigoProdutoEmbarcadorIntegracao.val(di.CodigoProdutoEmbarcadorIntegracao);
            _novoProdutoEmbarcador.ProdutoEmbarcador.val(di.Descricao);
            _novoProdutoEmbarcador.SiglaUnidade.val(di.SiglaUnidade);
            _novoProdutoEmbarcador.QuantidadeSecundaria.val(di.QuantidadeSecundaria);
            _novoProdutoEmbarcador.UnidadeMedidaSecundaria.val(di.UnidadeMedidaSecundaria);
            _novoProdutoEmbarcador.ProdutoEmbarcador.codEntity(di.CodigoProdutoEmbarcador);
            _novoProdutoEmbarcador.Peso.val(di.Peso);
            _novoProdutoEmbarcador.ValorUnitario.val(di.ValorUnitario);
            _novoProdutoEmbarcador.AlturaCM.val(di.AlturaCM);
            _novoProdutoEmbarcador.QuantidadeCaixaPorPallet.val(di.QuantidadeCaixaPorPallet);
            _novoProdutoEmbarcador.PrecoUnitario.val(di.PrecoUnitario);
            _novoProdutoEmbarcador.QuantidadeUnidadePorCaixa.val(di.QuantidadeUnidadePorCaixa);
            _novoProdutoEmbarcador.LarguraCM.val(di.LarguraCM);
            _novoProdutoEmbarcador.Canal.val(di.Canal);
            _novoProdutoEmbarcador.Setor.val(di.Setor);
            _novoProdutoEmbarcador.Organizacao.val(di.Organizacao);
            _novoProdutoEmbarcador.ComprimentoCM.val(di.ComprimentoCM);
            _novoProdutoEmbarcador.MetrosCubico.val(di.MetrosCubico);
            _novoProdutoEmbarcador.QuantidadePalet.val(di.QuantidadePalets);
            _novoProdutoEmbarcador.PalletFechado.val(di.PalletFechado);
            _novoProdutoEmbarcador.ObservacaoProdutoEmbarcador.val(di.Observacao);
            _novoProdutoEmbarcador.LinhaSeparacao.val(di.LinhaSeparacao.Descricao);
            _novoProdutoEmbarcador.LinhaSeparacao.codEntity(di.LinhaSeparacao.Codigo);
            _novoProdutoEmbarcador.Armazem.val(di.Armazem.Descricao);
            _novoProdutoEmbarcador.Armazem.codEntity(di.Armazem.Codigo);
            _novoProdutoEmbarcador.EnderecoProduto.val(di.EnderecoProduto.Descricao);
            _novoProdutoEmbarcador.EnderecoProduto.codEntity(di.EnderecoProduto.Codigo);
            _novoProdutoEmbarcador.Adicionar.text("Salvar");
            _novoProdutoEmbarcador.CamposPersonalizados.val(di.CamposPersonalizados);

            $.each(_produtoEmbarcador.ONUsProdutosEmbarcador.list, function (i, onu) {
                if (onu.CodigoProdutoEmbarcador == di.Codigo) {
                    var map = new Object();

                    map.Codigo = onu.Codigo;
                    map.CodigoClassificacaoRiscoONU = onu.CodigoClassificacaoRiscoONU;
                    map.CodigoProdutoEmbarcador = onu.CodigoProdutoEmbarcador;
                    map.Descricao = onu.Descricao;
                    map.Numero = onu.Numero;
                    map.ClasseRisco = onu.ClasseRisco;
                    map.RiscoSubsidiario = onu.RiscoSubsidiario;
                    map.NumeroRisco = onu.NumeroRisco;
                    map.Observacao = onu.Observacao;
                    _novoProdutoEmbarcador.ONUs.list.push(map);
                }
            });
            recarregarGridONUs();

            editandoProduto = true;
            Global.abrirModal('divAdicionarProdutoEmbarcador');
        }
    });
}

function excluirProduto(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaRemoverProdutoSelecionado, function () {
        $.each(_produtoEmbarcador.ProdutosEmbarcador.list, function (i, di) {
            if (di != null && di.Codigo != null && e != null && e.Codigo != null && e.Codigo == di.Codigo) {

                var quantidade = Globalize.parseFloat(di.Quantidade);
                if (isNaN(quantidade) || quantidade == undefined)
                    quantidade = 0;

                var qtdPalets = Globalize.parseFloat(_pedido.PalletsFracionado.val());
                if (isNaN(qtdPalets) || qtdPalets == undefined)
                    qtdPalets = 0;
                if (qtdPalets > 0 && Globalize.parseFloat(di.QuantidadePalets) > 0)
                    qtdPalets -= (Globalize.parseFloat(Globalize.parseFloat(di.QuantidadePalets).toFixed(3).replace(".", ",")));

                var peso = Globalize.parseFloat(_adicional.PesoTotalCargaTMS.val());
                if (isNaN(peso) || peso == undefined)
                    peso = 0;
                if (peso > 0 && Globalize.parseFloat(di.Peso) > 0 && quantidade > 0)
                    peso -= (Globalize.parseFloat(Globalize.parseFloat(di.Peso).toFixed(2).replace(".", ",")) * quantidade);

                var cubagem = Globalize.parseFloat(_adicional.CubagemTotalTMS.val());
                if (isNaN(cubagem) || cubagem == undefined)
                    cubagem = 0;
                if (cubagem > 0 && Globalize.parseFloat(di.MetrosCubico) > 0 && quantidade > 0)
                    cubagem -= (Globalize.parseFloat(Globalize.parseFloat(di.MetrosCubico).toFixed(2).replace(".", ",")) * quantidade);

                if (cubagem < 0)
                    cubagem = 0;
                if (peso < 0)
                    peso = 0;
                if (qtdPalets < 0)
                    qtdPalets = 0;

                _pedido.PalletsFracionado.val(((qtdPalets).toFixed(3).replace(".", ",")));
                _adicional.PesoTotalCargaTMS.val(((peso).toFixed(2).replace(".", ",")));
                _adicional.CubagemTotalTMS.val(((cubagem).toFixed(2).replace(".", ",")));

                _produtoEmbarcador.ProdutosEmbarcador.list.splice(i, 1);
            }
        });

        recarregarGridProduto();
    });
}

function AdicionarProdutoEmbarcadorClick(e, sender) {
    var tudoCerto = true;
    if (_novoProdutoEmbarcador.ProdutoEmbarcador.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        if (editandoProduto) {
            $.each(_produtoEmbarcador.ProdutosEmbarcador.list, function (i, di) {
                if (di != null && di.Codigo != null && _novoProdutoEmbarcador.Codigo.val() == di.Codigo) {
                    _produtoEmbarcador.ProdutosEmbarcador.list.splice(i, 1);
                }
            });
        }
        var map = new Object();
        map.Codigo = _novoProdutoEmbarcador.Codigo.val();
        map.CodigoProdutoEmbarcador = _novoProdutoEmbarcador.ProdutoEmbarcador.codEntity();
        map.CodigoProdutoEmbarcadorIntegracao = _novoProdutoEmbarcador.CodigoProdutoEmbarcadorIntegracao.val();
        map.IdDemanda = "";
        map.Descricao = _novoProdutoEmbarcador.ProdutoEmbarcador.val();
        map.SiglaUnidade = _novoProdutoEmbarcador.SiglaUnidade.val();
        map.Quantidade = _novoProdutoEmbarcador.Quantidade.val();
        map.QuantidadePlanejada = _novoProdutoEmbarcador.QuantidadePlanejada.val();
        map.SaldoQuantidade = _novoProdutoEmbarcador.Quantidade.val();
        map.Peso = _novoProdutoEmbarcador.Peso.val();
        map.QuantidadePalets = _novoProdutoEmbarcador.QuantidadePalet.val();
        map.UnidadeMedidaSecundaria = _novoProdutoEmbarcador.UnidadeMedidaSecundaria.val();
        map.QuantidadeSecundaria = _novoProdutoEmbarcador.QuantidadeSecundaria.val();
        map.PesoTotal = parseFloat(_novoProdutoEmbarcador.Quantidade.val()) * parseFloat(_novoProdutoEmbarcador.Peso.val());
        map.PalletFechado = _novoProdutoEmbarcador.PalletFechado.val();
        map.MetrosCubico = _novoProdutoEmbarcador.MetrosCubico.val();
        map.Observacao = _novoProdutoEmbarcador.ObservacaoProdutoEmbarcador.val();
        map.AlturaCM = _novoProdutoEmbarcador.AlturaCM.val();
        map.ComprimentoCM = _novoProdutoEmbarcador.ComprimentoCM.val();
        map.QuantidadeCaixaPorPallet = _novoProdutoEmbarcador.QuantidadeCaixaPorPallet.val();
        map.PrecoUnitario = _novoProdutoEmbarcador.PrecoUnitario.val();
        map.ValorUnitario = _novoProdutoEmbarcador.ValorUnitario.val();
        map.QuantidadeUnidadePorCaixa = _novoProdutoEmbarcador.QuantidadeUnidadePorCaixa.val();
        map.CodigoLinhaSeparacao = _novoProdutoEmbarcador.CodigoLinhaSeparacao.val();
        map.QuantidadeUnidadePorCaixa = 0;
        map.Setor = _novoProdutoEmbarcador.Setor.val();
        map.Organizacao = _novoProdutoEmbarcador.Organizacao.val();
        map.Canal = _novoProdutoEmbarcador.Canal.val();
        //map.CodigoEnderecoProduto = _novoProdutoEmbarcador.CodigoEnderecoProduto.val();
        map.LarguraCM = _novoProdutoEmbarcador.LarguraCM.val();
        map.LinhaSeparacao = {
            Codigo: _novoProdutoEmbarcador.LinhaSeparacao.codEntity() || 0,
            Descricao: _novoProdutoEmbarcador.LinhaSeparacao.val() || ""
        };
        map.EnderecoProduto = {
            Codigo: _novoProdutoEmbarcador.EnderecoProduto.codEntity() || 0,
            Descricao: _novoProdutoEmbarcador.EnderecoProduto.val() || ""
        };
        map.CodigoEnderecoProduto = map.EnderecoProduto.Codigo;
        map.Armazem = {
            Codigo: _novoProdutoEmbarcador.Armazem.codEntity() || 0,
            Descricao: _novoProdutoEmbarcador.Armazem.val() || "",
            CodigoIntegracao: _novoProdutoEmbarcador.CodigoIntegracaoArmazem.val() || "",
        };
        map.CodigoFilialEmbarcador = _novoProdutoEmbarcador.CodigoFilialEmbarcador.val();
        _produtoEmbarcador.ProdutosEmbarcador.list.push(map);
        map.CamposPersonalizados = _novoProdutoEmbarcador.CamposPersonalizados.val();

        var quantidade = Globalize.parseFloat(_novoProdutoEmbarcador.Quantidade.val());
        if (isNaN(quantidade) || quantidade == undefined)
            quantidade = 0;

        var qtdPalets = Globalize.parseFloat(_pedido.PalletsFracionado.val());
        if (isNaN(qtdPalets) || qtdPalets == undefined)
            qtdPalets = 0;
        qtdPalets += (Globalize.parseFloat(_novoProdutoEmbarcador.QuantidadePalet.val()));

        var peso = Globalize.parseFloat(_adicional.PesoTotalCargaTMS.val());
        if (isNaN(peso) || peso == undefined)
            peso = 0;
        peso += (Globalize.parseFloat(_novoProdutoEmbarcador.Peso.val()) * quantidade);

        var cubagem = Globalize.parseFloat(_adicional.CubagemTotalTMS.val());
        if (isNaN(cubagem) || cubagem == undefined)
            cubagem = 0;
        if (_novoProdutoEmbarcador.MetrosCubico.val() >= 0)
            cubagem += ((_novoProdutoEmbarcador.MetrosCubico.val()) * quantidade);
        else
            cubagem += (Globalize.parseFloat(_novoProdutoEmbarcador.MetrosCubico.val()) * quantidade);

        if (quantidade > 0) {
            _adicional.PesoTotalCargaTMS.val(((peso).toFixed(2).replace(".", ",")));
            _adicional.CubagemTotalTMS.val(((cubagem).toFixed(2).replace(".", ",")));
        }
        if (qtdPalets > 0)
            _pedido.PalletsFracionado.val(((qtdPalets).toFixed(3).replace(".", ",")));

        CarregarListaONUs();

        LimparCampos(_novoProdutoEmbarcador);
        resetarTabProduto();

        recarregarGridProduto();
        recarregarGridONUs();
        editandoProduto = false;
        _novoProdutoEmbarcador.Adicionar.text(Localization.Resources.Gerais.Geral.Adicionar);
        _novoProdutoEmbarcador.Codigo.val(guid());
        $("#" + _novoProdutoEmbarcador.ProdutoEmbarcador.id).focus();
    } else {

        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.Pedido.InformeCamposObrigatoriosLancamentoONU);
    }
}

function adicionarONUClick(e, sender) {
    var tudoCerto = true;
    tudoCerto = ValidarCamposObrigatorios(_novoProdutoEmbarcador);

    if (tudoCerto) {
        var map = new Object();

        map.Codigo = guid();
        map.CodigoClassificacaoRiscoONU = _novoProdutoEmbarcador.ClassificacaoRiscoONU.codEntity();
        map.CodigoProdutoEmbarcador = _novoProdutoEmbarcador.Codigo.val();
        map.Descricao = _novoProdutoEmbarcador.ClassificacaoRiscoONU.val();
        map.Numero = _novoProdutoEmbarcador.NumeroONU.val();
        map.ClasseRisco = _novoProdutoEmbarcador.ClasseRiscoONU.val();
        map.RiscoSubsidiario = _novoProdutoEmbarcador.RiscoSubsidiarioONU.val();
        map.NumeroRisco = _novoProdutoEmbarcador.NumeroRiscoONU.val();
        map.Observacao = _novoProdutoEmbarcador.ObservacaoONU.val();

        _novoProdutoEmbarcador.ONUs.list.push(map);

        recarregarGridONUs();

        limparDadosONU();
        $("#" + _novoProdutoEmbarcador.ClassificacaoRiscoONU.id).focus();

    } else {

        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.Pedido.InformeCamposObrigatoriosLancamentoProduto);
    }
}

function excluirONU(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaRemoverONUSelecionada, function () {
        $.each(_novoProdutoEmbarcador.ONUs.list, function (i, di) {
            if (di != null && di.Codigo != null && e != null && e.Codigo != null && e.Codigo == di.Codigo)
                _novoProdutoEmbarcador.ONUs.list.splice(i, 1);
        });
        $.each(_produtoEmbarcador.ONUsProdutosEmbarcador.list, function (i, di) {
            if (di != null && di.Codigo != null && e != null && e.Codigo != null && e.Codigo == di.Codigo)
                _produtoEmbarcador.ONUsProdutosEmbarcador.list.splice(i, 1);
        });
        recarregarGridONUs();
    });
}

//*******MÉTODOS*******

function loadGridProdutos() {
    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            excluirProduto(data)
        }, tamanho: "10", icone: ""
    };
    var editar = {
        descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: function (data) {
            editarProduto(data)
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [excluir, editar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "AlturaCM", visible: false },
        { data: "ComprimentoCM", visible: false },
        { data: "LarguraCM", visible: false },
        { data: "CodigoLinhaSeparacao", visible: false },
        { data: "CodigoEnderecoProduto", visible: false },
        { data: "CodigoProdutoEmbarcador", visible: false },
        { data: "CodigoProdutoEmbarcadorIntegracao", title: Localization.Resources.Pedidos.Pedido.CodigoIntegracao, width: "12%" },
        { data: "IdDemanda", title: Localization.Resources.Pedidos.Pedido.IDDemanda, width: "8%", visible: visibilidadeIdDemanda() },
        { data: "QuantidadeCaixaPorPallet", visible: false },
        { data: "PrecoUnitario", visible: false },
        { data: "QuantidadeUnidadePorCaixa", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
        { data: "ValorUnitario", title: Localization.Resources.Pedidos.Pedido.ValorUnitario, width: "10%", visible: visibilidadeValorUnitario() },
        { data: "Quantidade", title: Localization.Resources.Pedidos.Pedido.Quantidade, width: "8%" },
        { data: "SiglaUnidade", title: Localization.Resources.Pedidos.Pedido.SiglaUnidade, width: "10%" },
        { data: "SaldoQuantidade", title: Localization.Resources.Pedidos.Pedido.SaldoQ, width: "8%" },
        { data: "Peso", title: Localization.Resources.Pedidos.Pedido.Peso, width: "8%" },
        { data: "PesoTotal", title: Localization.Resources.Pedidos.Pedido.PesoTotal, width: "8%" },
        { data: "Organizacao", title: Localization.Resources.Pedidos.Pedido.Organizacao, width: "8%" },
        { data: "Canal", title: Localization.Resources.Pedidos.Pedido.Canal, width: "8%" },
        { data: "Setor", title: Localization.Resources.Pedidos.Pedido.Setor, width: "8%" },
        { data: "QuantidadePalets", title: Localization.Resources.Pedidos.Pedido.QtdPalet, width: "7%" },
        { data: "PalletFechado", title: Localization.Resources.Pedidos.Pedido.PalletFechado, width: "6%" },
        { data: "MetrosCubico", title: "M³", width: "8%" },
        { data: "CodigoFilialEmbarcador", title: Localization.Resources.Pedidos.Pedido.CodigoFilial, width: "8%", visible: _CONFIGURACAO_TMS.HabilitarCadastroArmazem },
        { data: "CodigoIntegracaoArmazem", title: Localization.Resources.Pedidos.Pedido.CodigoArmazem, width: "8%", visible: _CONFIGURACAO_TMS.HabilitarCadastroArmazem },
        { data: "LinhaSeparacao", title: Localization.Resources.Pedidos.Pedido.LinhaSeparacao, width: "10%" },
        { data: "EnderecoProduto", title: Localization.Resources.Pedidos.Pedido.EndProduto, width: "10%" },
        { data: "Observacao", title: Localization.Resources.Gerais.Geral.Observacao, width: "14%" },
        { data: "PrecoUnitario", title: Localization.Resources.Pedidos.Pedido.PrecoUnitario, width: "10%" }
    ];
    //se adicionar coluna na grid, deve-se adicionar tbm no back ObterDadosProdutosPedidoPorTipoOperacao

    _gridProduto = new BasicDataTable(_produtoEmbarcador.ProdutosEmbarcador.idGrid, header, menuOpcoes);
    recarregarGridProduto();
}

function loadGridONUs() {
    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            excluirONU(data)
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoProdutoEmbarcador", visible: false },
        { data: "CodigoClassificacaoRiscoONU", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "20%" },
        { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "10%" },
        { data: "ClasseRisco", title: Localization.Resources.Pedidos.Pedido.ClasseRisco, width: "10%" },
        { data: "RiscoSubsidiario", title: Localization.Resources.Pedidos.Pedido.Subsidiário, width: "10%" },
        { data: "NumeroRisco", title: Localization.Resources.Pedidos.Pedido.NumeroRisco, width: "10%" },
        { data: "Observacao", title: Localization.Resources.Gerais.Geral.Observacao, width: "20%" },
    ];

    _gridONU = new BasicDataTable(_novoProdutoEmbarcador.ONUs.idGrid, header, menuOpcoes);
    recarregarGridONUs();
}

function recarregarGridONUs() {
    var data = new Array();
    $.each(_novoProdutoEmbarcador.ONUs.list, function (i, prod) {
        var obj = new Object();

        obj.Codigo = prod.Codigo;
        obj.CodigoClassificacaoRiscoONU = prod.CodigoClassificacaoRiscoONU;
        obj.CodigoProdutoEmbarcador = prod.CodigoProdutoEmbarcador;
        obj.Descricao = prod.Descricao;
        obj.Numero = prod.Numero;
        obj.ClasseRisco = prod.ClasseRisco;
        obj.RiscoSubsidiario = prod.RiscoSubsidiario;
        obj.NumeroRisco = prod.NumeroRisco;
        obj.Observacao = prod.Observacao;

        data.push(obj);
    });
    _gridONU.CarregarGrid(data);
}

function recarregarGridProduto() {
    var data = new Array();
    $.each(_produtoEmbarcador.ProdutosEmbarcador.list, function (i, prod) {
        var obj = new Object();

        obj.Codigo = prod.Codigo;
        obj.CodigoProdutoEmbarcador = prod.CodigoProdutoEmbarcador;
        obj.CodigoProdutoEmbarcadorIntegracao = prod.CodigoProdutoEmbarcadorIntegracao;
        obj.CodigoLinhaSeparacao = prod.CodigoLinhaSeparacao;
        obj.Setor = prod.Setor;
        obj.Organizacao = prod.Organizacao,
            obj.Canal = prod.Canal,
            obj.CodigoEnderecoProduto = prod.CodigoEnderecoProduto;
        obj.IdDemanda = prod.IdDemanda;
        obj.Descricao = prod.Descricao;
        obj.SiglaUnidade =  prod.SiglaUnidade ?? "";
        obj.QuantidadePlanejada = prod.QuantidadePlanejada;
        obj.Quantidade = prod.Quantidade;
        obj.SaldoQuantidade = prod.SaldoQuantidade;
        obj.PesoTotal = prod.PesoTotal;
        obj.Peso = prod.Peso;
        obj.QuantidadePalets = prod.QuantidadePalets;
        obj.PalletFechado = (prod.PalletFechado == true ? "SIM" : "NÃO");
        obj.AlturaCM = prod.AlturaCM;
        obj.ComprimentoCM = prod.ComprimentoCM;
        obj.LarguraCM = prod.LarguraCM;
        obj.MetrosCubico = prod.MetrosCubico;
        obj.Observacao = prod.Observacao;
        obj.LinhaSeparacao = prod.LinhaSeparacao.Descricao;
        obj.EnderecoProduto = prod.EnderecoProduto.Descricao;
        obj.QuantidadeCaixaPorPallet = prod.QuantidadeCaixaPorPallet;
        obj.PrecoUnitario = prod.PrecoUnitario;
        obj.ValorUnitario = prod.ValorUnitario;
        obj.QuantidadeUnidadePorCaixa = prod.QuantidadeUnidadePorCaixa;
        obj.CodigoIntegracaoArmazem = prod.Armazem.CodigoIntegracao;
        obj.CodigoFilialEmbarcador = prod.CodigoFilialEmbarcador;
        data.push(obj);
    });
    _gridProduto.CarregarGrid(data);

    controlarExibicaoSaldoProdutoPedido();
}

function controlarExibicaoSaldoProdutoPedido() {
    var exibir = _pedido.TipoOperacao.apresentarSaldoProduto && (_pedido.Codigo.val() > 0);
    _gridProduto.ControlarExibicaoColuna("SaldoQuantidade", exibir);
}

function CarregarListaONUs() {
    $.each(_novoProdutoEmbarcador.ONUs.list, function (i, prod) {
        var obj = new Object();

        if (editandoProduto) {
            $.each(_produtoEmbarcador.ONUsProdutosEmbarcador.list, function (i, di) {
                if (di != null && di.Codigo != null && prod != null && prod.Codigo != null && prod.Codigo == di.Codigo)
                    _produtoEmbarcador.ONUsProdutosEmbarcador.list.splice(prod, 1);
            });
        }

        obj.Codigo = prod.Codigo;
        obj.CodigoClassificacaoRiscoONU = prod.CodigoClassificacaoRiscoONU;
        obj.CodigoProdutoEmbarcador = prod.CodigoProdutoEmbarcador;
        obj.Descricao = prod.Descricao;
        obj.Numero = prod.Numero;
        obj.ClasseRisco = prod.ClasseRisco;
        obj.RiscoSubsidiario = prod.RiscoSubsidiario;
        obj.NumeroRisco = prod.NumeroRisco;
        obj.Observacao = prod.Observacao;

        _produtoEmbarcador.ONUsProdutosEmbarcador.list.push(obj);
    });
}

function resetarTabProduto() {
    $("#tabsProduto a:first").tab("show");
}

function limparDadosONU() {
    LimparCampoEntity(_novoProdutoEmbarcador.ClassificacaoRiscoONU);
    _novoProdutoEmbarcador.ObservacaoONU.val("");
}

function limparProduto() {
    LimparCampos(_produtoEmbarcador);
    loadGridProdutos();
    resetarTabProduto();
}

function preencherProduto(dadosProduto) {
    PreencherObjetoKnout(_produtoEmbarcador, { Data: dadosProduto });
}

function visibilidadeValorUnitario() {
    return _produtoEmbarcador.ExibirValorUnitarioDoProduto.val();
}

function visibilidadeIdDemanda() {
    return _produtoEmbarcador.PossuiIdDemandaProdutos.val();
}