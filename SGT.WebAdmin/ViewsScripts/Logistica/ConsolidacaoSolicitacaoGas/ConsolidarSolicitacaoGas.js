/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Empresa.js" />

var _consolidarSolicitacaoGas;
var _CRUDConsolidarSolicitacaoGas;
var _capacidadeVeiculo = 1;

var ConsolidarSolicitacaoGas = function () {
    this.Solicitacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Produto = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });

    this.FilialSolicitante = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), enable: false, text: "Filial Solicitante:" });
    this.ProdutoSolicitacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), enable: false, text: "Produto:" });
    this.BaseSupridora = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "*Base Supridora:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "*Tipo de Carga:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "*Modelo Veicular:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Transportadora = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Transportadora:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "*Tipo de Operação:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.DataCarregamento = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), required: true, text: "Data Carregamento:" });
    this.DataEntrega = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), required: true, text: "Data Entrega:" });
    this.DataCarregamento.dateRangeLimit = this.DataEntrega;
    this.DataEntrega.dateRangeInit = this.DataCarregamento;

    this.CargaTrechoAnterior = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Carga trecho Anterior:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.QuantidadeDisponivel = PropertyEntity({ text: "Disponível:", val: ko.observable("") });
    this.QuantidadeFaltante = PropertyEntity({ text: "Falta Suprir:", val: ko.observable("") });

    this.Quantidade = PropertyEntity({ text: "Quantidade:", val: ko.observable(0), getType: typesKnockout.decimal, enable: false });
    this.QuantidadeCarga = PropertyEntity({ text: "*Quantidade de Carga:", required: true, val: ko.observable("0"), getType: typesKnockout.int });

    this.QuantidadeCarga.val.subscribe(function (novoValor) {
        var quantidade = Number(novoValor);
        var resultado = quantidade * _capacidadeVeiculo;

        _consolidarSolicitacaoGas.Quantidade.val(resultado + " ton");
    });

    this.Origem = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, idBtnSearch: guid(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(false) });//usado para ter a origem da carga gerada.

    this.ModeloVeicular.codEntity.subscribe(function (codigo) {
        if (codigo == 0)
            _capacidadeVeiculo = 1;
    });
}

var CRUDConsolidarSolicitacaoGas = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarConsolidacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), id: guid() });
}

function loadConsolidarSolicitacaoGas() {
    _consolidarSolicitacaoGas = new ConsolidarSolicitacaoGas();
    _CRUDConsolidarSolicitacaoGas = new CRUDConsolidarSolicitacaoGas();

    KoBindings(_consolidarSolicitacaoGas, "knockoutConsolidarSolicitacao");
    KoBindings(_CRUDConsolidarSolicitacaoGas, "knockoutCRUDConsolidarSolicitacao");

    new BuscarClientes(_consolidarSolicitacaoGas.BaseSupridora, ajustarDisponibilidade, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    new BuscarEmpresa(_consolidarSolicitacaoGas.Transportadora);
    new BuscarCargas(_consolidarSolicitacaoGas.CargaTrechoAnterior);

    new BuscarModelosVeicularesCarga(_consolidarSolicitacaoGas.ModeloVeicular, ajustarQuantidade);
    new BuscarTiposOperacao(_consolidarSolicitacaoGas.TipoOperacao);
    new BuscarTiposdeCarga(_consolidarSolicitacaoGas.TipoDeCarga);
}

function limparCamposConsolidarSolicitacao() {
    LimparCampos(_consolidarSolicitacaoGas);
}

function buscarInformacoesConsolidacao() {
    executarReST("ConsolidacaoSolicitacaoGas/BuscarInformacoesConsolidacao", { CodigoSolicitacao: _consolidarSolicitacaoGas.Solicitacao.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_consolidarSolicitacaoGas, { Data: retorno.Data.InformacoesBaseSatelite });
                _gridConsolidacoesGeradas.CarregarGrid(retorno.Data.ConsolidacoesGeradas);
                _capacidadeVeiculo = retorno.Data.CapacidadeVeiculo / 1000;
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function adicionarConsolidacaoClick() {
    if (!ValidarCamposObrigatorios(_consolidarSolicitacaoGas)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    if (obterQuantidadeDisponivel(_consolidarSolicitacaoGas.BaseSupridora.codEntity(), _consolidarSolicitacaoGas.Produto.val()) < (Number(obterValorNumerico(Number(_consolidarSolicitacaoGas.Quantidade.val().replace("ton", "")))))) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "A unidade supridora não possuí quantidade suficiente disponível.");
        return;
    }

    if (obterQuantidadeFaltante(_consolidarSolicitacaoGas.BaseSupridora.codEntity()) < (Number(_consolidarSolicitacaoGas.Quantidade.val().replace(".", "").replace(",", ".").replace("ton", "")))) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "A quantidade faltante é menor do que " + _consolidarSolicitacaoGas.Quantidade.val() + ".");
        return;
    }

    executarReST("ConsolidacaoSolicitacaoGas/AdicionarConsolidacao", RetornarObjetoPesquisa(_consolidarSolicitacaoGas), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                Global.fecharModal("divModalConsolidarSolicitacao");

                _gridConsolidacaoSolicitacaoGas.CarregarGrid();
                _gridQuantidades.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function ajustarQuantidade(data) {
    _capacidadeVeiculo = Number(data.CapacidadePesoTransporte.replace(".", "").replace(",", ".")) / 1000;
    var quantidade = _capacidadeVeiculo * Number(_consolidarSolicitacaoGas.QuantidadeCarga.val().replace(".", "").replace(",", "."));

    _consolidarSolicitacaoGas.ModeloVeicular.val(data.Descricao);
    _consolidarSolicitacaoGas.ModeloVeicular.codEntity(data.Codigo);

    _consolidarSolicitacaoGas.Quantidade.val(quantidade + " ton");
}

function ajustarDisponibilidade(data) {
    var quantidadeDisponivel = obterQuantidadeDisponivel(data.Codigo, _consolidarSolicitacaoGas.Produto.val());

    _consolidarSolicitacaoGas.QuantidadeDisponivel.val(quantidadeDisponivel + " ton");
    _consolidarSolicitacaoGas.BaseSupridora.val(data.Descricao);
    _consolidarSolicitacaoGas.BaseSupridora.codEntity(data.Codigo);
}

function obterQuantidadeDisponivel(codigoBase, codigoProduto) {
    var listaQuantidade = _gridQuantidades.GridViewTableData().filter(function (obj) { return (obj.CodigoBase == codigoBase && obj.CodigoProduto == codigoProduto) });
    return listaQuantidade.reduce((valorAtual, b) => valorAtual + Number(obterValorNumerico(b.SaldoRestante)), 0);
}

function obterQuantidadeFaltante() {
    return Number(_consolidarSolicitacaoGas.QuantidadeFaltante.val().replace(".", "").replace(",", ".").replace("ton", ""));
}

function obterValorNumerico(valor) {
    valor = valor.toString();

    if (valor.includes(","))
        return valor.replace(".", "").replace(",", ".");

    return valor.replace(",", ".");
}