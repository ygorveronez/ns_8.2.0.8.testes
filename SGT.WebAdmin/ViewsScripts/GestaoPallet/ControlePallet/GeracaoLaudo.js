/// <reference path="../../../js/Global/Auditoria.js" />


// #region Referencias
// #endregion Referencias

// #region Obejos Globais do Arquivo
var _controlePalletGeracaoLaudo;
var _geracaoLaudoProdutos = [];
//#endregion Obejos Globais do Arquivo

// #region Classes
var DadosLaudo = function () {
    this.Codigo = PropertyEntity({ text: "Número Laudo", id: guid(), val: ko.observable(0) });
    this.CodigoMovimentacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataCriacao = PropertyEntity({ text: "Data", getType: typesKnockout.dateTime, val: ko.observable(null), required: ko.observable(true), enable: ko.observable(false) });
    this.Responsavel = PropertyEntity({ text: "Operador Responsável", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(false) });
    this.Veiculo = PropertyEntity({ text: "Veiculo", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(false) });
    this.AdicionarProduto = PropertyEntity({ text: "Adicionar Produto", eventClick: undefined, type: types.event, visible: ko.observable(false), enable: ko.observable(false) });

    this.GerarLaudo = PropertyEntity({ text: "Gerar Laudo", eventClick: gerarLaudoMovimentacaoPalletClick, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", eventClick: limparCamposGeracaoLaudoMovimentacaoPallet, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });

    this.TituloSubArea = PropertyEntity({ text: ("Dados do Produto") });
    this.ValorTotal = PropertyEntity({ text: (""), visible: ko.observable(false) });
    this.ValorTotalDescricao = PropertyEntity({ text: (""), visible: ko.observable(false) });
    this.auditarLaudoClick = auditarLaudoClick;
}

var GeracaoLaudoProduto = function () {
    this.Codigo = PropertyEntity({ id: guid(), val: ko.observable(0) });
    this.Produto = PropertyEntity({ text: "Produto:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(true) });
    this.ProdutoDescricao = PropertyEntity({ text: ko.observable("-"), type: types.string });
    this.QuantidadeOrigem = PropertyEntity({ text: ("Qt. Origem"), val: ko.observable(0), getType: typesKnockout.decimal, maxlength: 13, enable: ko.observable(true), required: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.QuantidadeDevolvida = PropertyEntity({ text: ("Qt. Entregue"), val: ko.observable(0), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.QuantidadeAvariada = PropertyEntity({ text: ("Qt. Avariada"), val: ko.observable(0), enable: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorAvariado = PropertyEntity({ text: ("Valor Avariado"), val: ko.observable(0), enable: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.QuantidadeSobras = PropertyEntity({ text: ("Qt. Sobras"), val: ko.observable(0), enable: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorSobras = PropertyEntity({ text: ("Valor Sobras"), val: ko.observable(0), enable: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.QuantidadeSemCondicao = PropertyEntity({ text: ("Qt. sem Condição"), val: ko.observable(0), enable: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorSemCondicao = PropertyEntity({ text: ("Valor sem Condição"), val: ko.observable(0), enable: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.QuantidadeFalta = PropertyEntity({ text: ("Qt. Falta"), val: ko.observable(0), enable: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorFalta = PropertyEntity({ text: ("Valor Falta"), val: ko.observable(0), enable: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.QuantidadeDescarte = PropertyEntity({ text: ("Qt. Descarte"), val: ko.observable("0,00"), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.QuantidadeManutencao = PropertyEntity({ text: ("Qt. Manutenção"), val: ko.observable("0,00"), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorTotal = PropertyEntity({ text: (""), visible: ko.observable(false) });
    this.ValorTotalDescricao = PropertyEntity({ text: (""), visible: ko.observable(false) });
    this.Remover = PropertyEntity({ text: "Remover", eventClick: undefined, type: types.event, visible: ko.observable(false), enable: ko.observable(false) });

    this.auditarLaudoClick = auditarLaudoClick;
}
// #endregion Classes

function auditarLaudoClick() {
    __AbrirModalAuditoria();
}

// #region Funções de Inicialização
function loadGeracaoLaudoControlePallet(movimentacao) {
    let data = { Codigo: movimentacao.Codigo };
    executarReST("ControlePallet/BuscarDadosLaudo", data, function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/GeracaoLaudo.html?dyn=" + guid(), function (html) {
                $("#modal-content").html(html);
                let laudo = r.Data;
                _controlePalletGeracaoLaudo = new DadosLaudo();
                KoBindings(_controlePalletGeracaoLaudo, "knockoutGeracaoLaudo");

                _controlePalletGeracaoLaudo.CodigoMovimentacao.val(movimentacao.Codigo);

                HeaderAuditoria("GestaoDevolucaoLaudo", _controlePalletGeracaoLaudo);

                BuscarVeiculos(_controlePalletGeracaoLaudo.Veiculo);
                BuscarTransportadores(_controlePalletGeracaoLaudo.Transportador);
                BuscarOperador(_controlePalletGeracaoLaudo.Responsavel);

                PreencherObjetoKnout(_controlePalletGeracaoLaudo, { Data: laudo });

                let permiteAlterar = _controlePalletGeracaoLaudo.Codigo.val() <= 0;

                if (laudo.Produtos) {
                    for (let produto of laudo.Produtos) {
                        adicionarProdutoPallet(produto, true, permiteAlterar);
                    }
                }

                if (!permiteAlterar)
                    SetarEnableCamposKnockout(_controlePalletGeracaoLaudo, permiteAlterar);
            });
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
    });
}
// #endregion Funções de Inicialização

// #region Ações
function exibirModalLaudo(movimentacao) {
    limparCamposGeracaoLaudoMovimentacaoPallet();
    loadGeracaoLaudoControlePallet(movimentacao);

    Global.abrirModal('divModalLaudo');
}

function fecharModalLaudo() {
    Global.fecharModal('divModalLaudo');
}

function adicionarProdutoPallet(produto, show, enableChanges) {
    $.get("Content/Static/Carga/GestaoDevolucao/GeracaoLaudoProduto.html?dyn=" + guid(), function (html) {
        let _produto = new GeracaoLaudoProduto();

        if (produto) {
            PreencherObjetoKnout(_produto, { Data: produto });
            _produto.QuantidadeOrigem.enable(false);
            _produto.ProdutoDescricao.text(produto.Produto.CodigoProdutoEmbarcador + ' - ' + produto.Produto.Descricao);
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

        setarSubscribesLaudoProdutoPallet(_produto);

        if (!enableChanges)
            SetarEnableCamposKnockout(_produto, enableChanges);

        _geracaoLaudoProdutos.push(_produto);
    });
}

function gerarLaudoMovimentacaoPalletClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja confirmar o recebimento?", function () {
        let produtos = [];
        if (!ValidarCamposObrigatorios(_controlePalletGeracaoLaudo)) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "Preencha os campos obrigatórios do Laudo!");
            return;
        }

        for (let produto of _geracaoLaudoProdutos) {
            if (!ValidarCamposObrigatorios(produto) || produto.QuantidadeOrigem.val() == '0,00') {
                if (produto.QuantidadeOrigem.val() == '0,00')
                    produto.QuantidadeOrigem.requiredClass("form-control is-invalid");

                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "Preencha os campos obrigatórios do Produto!");
                return;
            }

            if (produto.QuantidadeFalta.val() != '0,00') {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "A quantidade de falta precisa ser igual a zero!");
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
            })
        }

        let dados = {
            CodigoMovimentacao: _controlePalletGeracaoLaudo.CodigoMovimentacao.val(),
            Codigo: _controlePalletGeracaoLaudo.Codigo.val(),
            DataCriacao: _controlePalletGeracaoLaudo.DataCriacao.val(),
            Responsavel: _controlePalletGeracaoLaudo.Responsavel.codEntity(),
            Transportador: _controlePalletGeracaoLaudo.Transportador.codEntity(),
            Veiculo: _controlePalletGeracaoLaudo.Veiculo.codEntity(),
            Produtos: JSON.stringify(produtos)
        };

        executarReST("ControlePallet/GerarLaudoPallet", dados, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.Success, Localization.Resources.Gerais.Geral.Sucesso, "Laudo gerado com sucesso;");
                fecharModalLaudo();
                _gridControlePallet.CarregarGrid();
            } else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        });
    });
}

function limparCamposGeracaoLaudoMovimentacaoPallet() {
    LimparCampos(_controlePalletGeracaoLaudo);
    LimparCampos(_geracaoLaudoProdutos);
    _controlePalletGeracaoLaudo = null;
    _geracaoLaudoProdutos = [];
}

function setarSubscribesLaudoProdutoPallet(knoutProduto) {
    knoutProduto.QuantidadeDevolvida.val.subscribe(function () {
        qtdFaltaChange(knoutProduto)
    });
    knoutProduto.QuantidadeDescarte.val.subscribe(function () {
        qtdFaltaChange(knoutProduto)
    });
    knoutProduto.QuantidadeManutencao.val.subscribe(function () {
        qtdFaltaChange(knoutProduto)
    });
}


function qtdFaltaChange(sender) {
    let quantidadeFalta = Globalize.parseFloat(sender.QuantidadeOrigem.val()) - Globalize.parseFloat(sender.QuantidadeDescarte.val()) - Globalize.parseFloat(sender.QuantidadeDevolvida.val()) - Globalize.parseFloat(sender.QuantidadeManutencao.val());

    sender.QuantidadeFalta.val(Globalize.format(quantidadeFalta, "n2"));
}

// #endregion Ações