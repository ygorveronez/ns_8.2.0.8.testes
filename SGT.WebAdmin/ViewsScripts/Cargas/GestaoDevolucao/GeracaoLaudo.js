/// <reference path="../../../js/Global/Auditoria.js" />


// #region Referencias
// #endregion Referencias

// #region Obejos Globais do Arquivo
var _gestaoDevolucaoGeracaoLaudo;
var _etapa;
//#endregion Obejos Globais do Arquivo

// #region Classes
var DadosLaudo = function () {
    this.Codigo = PropertyEntity({ text: "Número Laudo", id: guid(), val: ko.observable(0) });

    this.DataCriacao = PropertyEntity({ text: "Data", getType: typesKnockout.dateTime, val: ko.observable(null), required: ko.observable(true), enable: ko.observable(true) });
    this.Responsavel = PropertyEntity({ text: "Responsável", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veiculo", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true) });
    this.AdicionarProduto = PropertyEntity({ text: "Adicionar Produto", eventClick: adicionarProdutoClick, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
    this.Produtos = PropertyEntity({ val: ko.observableArray([]) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

    this.ValorTotal = PropertyEntity({
        text: ("Valor Total").getFieldDescription(),
        val: ko.computed(() => {
            let totalProdutos = 0;
            this.Produtos.val().forEach(produto => {
                totalProdutos += produto.ValorTotal.val();
            });
            return Math.round((totalProdutos.toFixed(2) * 100)) / 100;
        }), visible: ko.observable(true), enable: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }
    });
    this.ValorTotalDescricao = PropertyEntity({
        text: ("Valor Total").getFieldDescription(),
        val: ko.computed(() => {
            return Globalize.format(this.ValorTotal.val(), "n2");
        }), visible: ko.observable(true), enable: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }
    });

    this.GerarLaudo = PropertyEntity({ text: "Gerar Laudo", eventClick: gerarLaudoClick, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", eventClick: cancelarGeracaoLaudo, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });

    this.TituloSubArea = PropertyEntity({ text: ("Dados da Devolução") });

    this.auditarLaudoClick = auditarLaudoClick;
    this.DevolucaoExclusivaPallet = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });

    this.DevolucaoExclusivaPallet.val.subscribe((value) => {
        _gestaoDevolucaoGeracaoLaudo.ValorTotalDescricao.visible(!value);
    })
}

var GeracaoLaudoProduto = function () {
    this.Codigo = PropertyEntity({ id: guid(), val: ko.observable(0) });
    this.Produto = PropertyEntity({ text: "Produto:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(true) });
    this.ProdutoDescricao = PropertyEntity({ text: ko.observable("-"), type: types.string });
    this.QuantidadeOrigem = PropertyEntity({ text: ("Qt. Origem"), val: ko.observable(0), visible: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, enable: ko.observable(true), required: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: true });
    this.QuantidadeDevolvida = PropertyEntity({ text: ("Qt. Entregue"), val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: true });
    this.QuantidadeAvariada = PropertyEntity({ text: ("Qt. Avariada"), val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: false });
    this.ValorAvariado = PropertyEntity({ text: ("Valor Avariado"), val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: false });
    this.QuantidadeSobras = PropertyEntity({ text: ("Qt. Sobras"), val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: false });
    this.ValorSobras = PropertyEntity({ text: ("Valor Sobras"), val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: false });
    this.QuantidadeSemCondicao = PropertyEntity({ text: ("Qt. sem Condição"), visible: ko.observable(true), val: ko.observable(0), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: false });
    this.ValorSemCondicao = PropertyEntity({ text: ("Valor sem Condição"), val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: false });
    this.QuantidadeFalta = PropertyEntity({ text: ("Qt. Falta"), val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: true });
    this.ValorFalta = PropertyEntity({ text: ("Valor Falta"), val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: false });
    this.QuantidadeDescarte = PropertyEntity({ text: ("Qt. Descarte"), val: ko.observable("0,00"), visible: ko.observable(false), enable: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: true });
    this.QuantidadeManutencao = PropertyEntity({ text: ("Qt. Manutenção"), val: ko.observable("0,00"), visible: ko.observable(false), enable: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: true });
    this.QuantidadeDescarte = PropertyEntity({ text: ("Qt. Descarte"), val: ko.observable("0,00"), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: true });
    this.QuantidadeManutencao = PropertyEntity({ text: ("Qt. Manutenção"), val: ko.observable("0,00"), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: true });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

    this.ValorTotal = PropertyEntity({
        text: ("Valor Total"),
        val: ko.computed(() => {
            let totalCampos = (string.ParseFloat(this.ValorAvariado.val()) +
                string.ParseFloat(this.ValorSemCondicao.val()) +
                string.ParseFloat(this.ValorFalta.val())) - 
                (string.ParseFloat(this.ValorSobras.val()));
            return Math.round((totalCampos.toFixed(2) * 100)) / 100;
        }), visible: ko.observable(true), enable: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: false
    });
    this.ValorTotalDescricao = PropertyEntity({
        text: ("Valor Total").getFieldDescription(),
        val: ko.computed(() => {
            return Globalize.format(this.ValorTotal.val(), "n2");
        }), visible: ko.observable(true), enable: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, exibirDevolucaoPallet: false
    });

    this.Remover = PropertyEntity({ text: "Remover", eventClick: removerGeracaoLaudoProduto, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });

    this.auditarLaudoClick = auditarLaudoClick;

}
// #endregion Classes

function auditarLaudoClick() {
    __AbrirModalAuditoria();
}

// #region Funções de Inicialização
function loadGeracaoLaudo(etapa) {

    //TODO: Mudar como obter o código da GestaoDevolução
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/GeracaoLaudo.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                let laudo = r.Data;
                _etapa = etapa;
                _gestaoDevolucaoGeracaoLaudo = new DadosLaudo();
                KoBindings(_gestaoDevolucaoGeracaoLaudo, "knockoutGeracaoLaudo");

                HeaderAuditoria("GestaoDevolucaoLaudo", _gestaoDevolucaoGeracaoLaudo);

                BuscarVeiculos(_gestaoDevolucaoGeracaoLaudo.Veiculo);
                BuscarTransportadores(_gestaoDevolucaoGeracaoLaudo.Transportador);
                BuscarOperador(_gestaoDevolucaoGeracaoLaudo.Responsavel);

                PreencherObjetoKnout(_gestaoDevolucaoGeracaoLaudo, { Data: laudo });
                controlarAcoesContainerPrincipal(_etapa, _gestaoDevolucaoGeracaoLaudo);

                if (laudo.DadosDevolucao) {
                    for (let produto of laudo.DadosDevolucao)
                        adicionarProduto(produto);
                }

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
            });
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
    });
}
// #endregion Funções de Inicialização

// #region Ações
function adicionarProdutoClick() {
    adicionarProduto(null, true);
}

function adicionarProduto(produto, show) {
    $.get("Content/Static/Carga/GestaoDevolucao/GeracaoLaudoProduto.html?dyn=" + guid(), function (html) {
        let _produto = new GeracaoLaudoProduto();

        if (produto) {
            PreencherObjetoKnout(_produto, { Data: produto });
            _produto.QuantidadeOrigem.enable(false);
            if (produto.Produto.Descricao)
                _produto.ProdutoDescricao.text(produto.Produto.CodigoProdutoEmbarcador + ' - ' + produto.Produto.Descricao);
            else
                _produto.ProdutoDescricao.text(produto.Produto.CodigoProdutoEmbarcador);

            _produto.Remover.visible(false);
            if (produto.Produto.Codigo > 0)
                _produto.Produto.enable(false);
        }

        let guid = (_produto.Codigo.val() > 0 ? _produto.Codigo.val() : _produto.Codigo.id);
        let knockoutGeracaoLaudoProduto = "knockoutGeracaoLaudoProduto";
        let knockoutGeracaoLaudoProdutoDinamico = knockoutGeracaoLaudoProduto + guid;

        html = html.replaceAll(knockoutGeracaoLaudoProduto, knockoutGeracaoLaudoProdutoDinamico);

        let idAccordionHeader = 'idAccordionHeader_' + guid;
        let idAccordionCollapse = 'idAccordionCollapse_' + guid;
        html = html.replaceAll('idAccordionHeader', idAccordionHeader);
        html = html.replaceAll('idAccordionCollapse', idAccordionCollapse);

        $("#divGeracaoLaudoProdutos").append(html);
        if (show)
            $("#" + idAccordionCollapse).addClass("show");

        KoBindings(_produto, knockoutGeracaoLaudoProdutoDinamico);
        BuscarProdutos(_produto.Produto, preencheDadosProdutoSelecionado);
        controlarAcoesContainerPrincipal(_etapa, _produto);

        if (_gestaoDevolucaoGeracaoLaudo.DevolucaoExclusivaPallet.val()) {

            for (var prop in _produto) {
                var property = _produto[prop];

                if (property.exibirDevolucaoPallet != undefined && property.exibirDevolucaoPallet === false) {
                    property.visible(false);
                }
            }
        }

        _gestaoDevolucaoGeracaoLaudo.Produtos.val.push(_produto);
    });
}

function gerarLaudoClick() {
    let produtos = [];
    if (!ValidarCamposObrigatorios(_gestaoDevolucaoGeracaoLaudo)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "Preencha os campos obrigatórios do Laudo!");
        return;
    }

    for (let produto of _gestaoDevolucaoGeracaoLaudo.Produtos.val()) {
        if (!ValidarCamposObrigatorios(produto) || produto.QuantidadeOrigem.val() == 0) {
            if (produto.QuantidadeOrigem.val() == 0)
                produto.QuantidadeOrigem.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "Preencha os campos obrigatórios da Devolução!");
            return;
        }
        produtos.push({
            Codigo: produto.Codigo.val(),
            Produto: produto.Produto.codEntity(),
            QuantidadeOrigem: produto.QuantidadeOrigem.val(),
            QuantidadeDevolvida: produto.QuantidadeDevolvida.val(),
            QuantidadeAvariada: produto.QuantidadeAvariada.val(),
            ValorAvariado: produto.ValorAvariado.val(),
            QuantidadeSobras: produto.QuantidadeSobras.val(),
            ValorSobras: produto.ValorSobras.val(),
            QuantidadeSemCondicao: produto.QuantidadeSemCondicao.val(),
            ValorSemCondicao: produto.ValorSemCondicao.val(),
            QuantidadeFalta: produto.QuantidadeFalta.val(),
            ValorFalta: produto.ValorFalta.val(),
            QuantidadeDescarte: produto.QuantidadeDescarte.val(),
            QuantidadeManutencao: produto.QuantidadeManutencao.val(),
            ValorTotal: produto.ValorTotal.val(),
        })
    }

    if (produtos.length === 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "Informe ao menos um produto para gerar o Laudo.");
        return;
    }

    let dados = {
        CodigoGestaoDevolucao: _pesquisaGestaoDevolucao.Codigo.val(),
        Codigo: _gestaoDevolucaoGeracaoLaudo.Codigo.val(),
        DataCriacao: _gestaoDevolucaoGeracaoLaudo.DataCriacao.val(),
        Responsavel: _gestaoDevolucaoGeracaoLaudo.Responsavel.codEntity(),
        Transportador: _gestaoDevolucaoGeracaoLaudo.Transportador.codEntity(),
        Veiculo: _gestaoDevolucaoGeracaoLaudo.Veiculo.codEntity(),
        Produtos: JSON.stringify(produtos)
    };

    executarReST("GestaoDevolucao/AtualizarDadosLaudo", dados, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.Success, Localization.Resources.Gerais.Geral.Sucesso, "Laudo gerado com sucesso;");
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
    });
}

function cancelarGeracaoLaudo() {
    LimparCampos(_gestaoDevolucaoGeracaoLaudo);
    _gestaoDevolucaoGeracaoLaudo.Produtos.val().forEach(p => LimparCampos(p));
    limparEtapasDevolucao();
    _gestaoDevolucaoGeracaoLaudo = null;
    _gestaoDevolucaoGeracaoLaudo.Produtos.val([]);
}

function removerGeracaoLaudoProduto(e) {
    console.log('removerGeracaoLaudoProduto');
    let codigo = (e.Codigo.val() > 0 ? e.Codigo.val() : e.Codigo.id)
    let index = _gestaoDevolucaoGeracaoLaudo.Produtos.val().findIndex(function (produto) {
        return produto.Codigo.val() === codigo || produto.Codigo.id === codigo;
    });

    if (index > -1) {
        let produtos = _gestaoDevolucaoGeracaoLaudo.Produtos.val();
        produtos.splice(index, 1)
        _gestaoDevolucaoGeracaoLaudo.Produtos.val(produtos);
    }

    $('#knockoutGeracaoLaudoProduto' + codigo).remove();
}

function preencheDadosProdutoSelecionado(produtoSelecionado, knout) {
    if (_gestaoDevolucaoGeracaoLaudo.Produtos.val().find(f => f.Produto.codEntity() == produtoSelecionado.Codigo)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "Produto já faz parte dos Dados da Devolução. Selecione outro produto!");
        return;
    }

    knout.codEntity(produtoSelecionado.Codigo);
    knout.val(produtoSelecionado.Descricao);

    let knoutProdutoSelecionado = _gestaoDevolucaoGeracaoLaudo.Produtos.val().find(f => f.Produto.id == knout.id);
    if (knoutProdutoSelecionado)
        knoutProdutoSelecionado.ProdutoDescricao.text(produtoSelecionado.CodigoProdutoEmbarcador + ' - ' + produtoSelecionado.Descricao);
}
// #endregion Ações