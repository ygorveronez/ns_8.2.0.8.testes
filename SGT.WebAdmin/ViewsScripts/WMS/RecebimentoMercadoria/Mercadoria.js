/// <reference path="RecebimentoMercadoria.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Deposito.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _mercadoria;
var _adicionarMercadoria;
var _gridMercadoria;
var editandoMercadoria = false;
var _modalAdicionarMercadoria;

var Mercadoria = function () {
    this.Grid = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarMercadoriaClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
}

var AdicionarMercadoria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CNPJCliente = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoDepositoPosicao = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoRecebimento = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoProdutoEmbarcador = PropertyEntity({ val: ko.observable(""), def: "" });
    this.NCM = PropertyEntity({ val: ko.observable(""), def: "" });
    this.ChaveNFe = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Identificacao = PropertyEntity({ val: ko.observable(""), def: "" });

    this.NumeroLote = PropertyEntity({ text: "Número Lote: ", maxlength: 150, visible: true, required: false, val: ko.observable(""), enable: ko.observable(true) });
    this.CodigoBarras = PropertyEntity({ text: "Cód. Barras: ", maxlength: 150, visible: true, required: false, val: ko.observable(""), enable: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ text: "Data de Vencimento: ", getType: typesKnockout.date, visible: true, required: false, enable: ko.observable(true) });
    this.QuantidadeLote = PropertyEntity({ text: "*Quantidade:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, val: ko.observable("0,000"), maxlength: 11, def: "0,000", enable: ko.observable(true) });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto do Embarcador:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", maxlength: 150, visible: true, required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.QuantidadePalet = PropertyEntity({ text: "*Qtd. Total Palet:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true }, val: ko.observable("0,000"), maxlength: 11, def: "0,000", enable: ko.observable(true) });
    this.MetroCubico = PropertyEntity({ text: "*M³ Total:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true }, val: ko.observable("0,000"), maxlength: 11, def: "0,000", enable: ko.observable(true) });
    this.Peso = PropertyEntity({ text: "*Peso Unitário:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true }, val: ko.observable("0,000"), maxlength: 11, def: "0,000", enable: ko.observable(true) });
    this.DepositoPosicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Local de Armanzenamento:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable("0,0000"), maxlength: 12, def: "0,0000", enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Produto:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(true) });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarMercadoriaNovaClick, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function LoadMercadoria() {
    _mercadoria = new Mercadoria();
    KoBindings(_mercadoria, "knockoutMercadoria");

    _adicionarMercadoria = new AdicionarMercadoria();
    KoBindings(_adicionarMercadoria, "knoutAdicionarMercadoria");

    new BuscarProdutos(_adicionarMercadoria.ProdutoEmbarcador, RetornoMercadoriaProdutoEmbarcador);
    new BuscarProdutoTMS(_adicionarMercadoria.Produto);
    new BuscarDepositoPosicao(_adicionarMercadoria.DepositoPosicao);

    CarregarGridMercadoria();
    _modalAdicionarMercadoria = new bootstrap.Modal(document.getElementById("divAdicionarMercadoria"), { backdrop: true, keyboard: true });

}

function CarregarGridMercadoria() {
    var excluir = { descricao: "Remover", id: "clasRemoverMercadoria", evento: "onclick", metodo: ExcluirMercadoriaClick, tamanho: "15", icone: "" };
    var editar = { descricao: "Editar", id: "clasEditarMercadoria", evento: "onclick", metodo: EditarMercadoriaClick, tamanho: "15", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [excluir, editar] };

    _gridMercadoria = new GridView(_mercadoria.Grid.idGrid, "RecebimentoMercadoria/PesquisaMercadoria", _recebimentoMercadoria, menuOpcoes, null);
    _gridMercadoria.CarregarGrid();
}

function RetornoMercadoriaProdutoEmbarcador(data) {
    _adicionarMercadoria.ProdutoEmbarcador.val(data.Descricao);
    _adicionarMercadoria.ProdutoEmbarcador.codEntity(data.Codigo);
    _adicionarMercadoria.Peso.val(data.PesoUnitario);
    if (_adicionarMercadoria.Descricao.val() == "")
        _adicionarMercadoria.Descricao.val(data.Descricao);

    var quantidade = Globalize.parseFloat(_adicionarMercadoria.QuantidadeLote.val());
    if (isNaN(quantidade) || quantidade == undefined)
        quantidade = 0;

    var qtdPalets = Globalize.parseFloat(_adicionarMercadoria.QuantidadePalet.val());
    if (isNaN(qtdPalets) || qtdPalets == undefined)
        qtdPalets = 0;

    var peso = Globalize.parseFloat(_adicionarMercadoria.Peso.val());
    if (isNaN(peso) || peso == undefined)
        peso = 0;

    var cubagem = Globalize.parseFloat(_adicionarMercadoria.MetroCubico.val());
    if (isNaN(cubagem) || cubagem == undefined)
        cubagem = 0;

    var data = { CodigoProduto: data.Codigo, QuantidadePalet: qtdPalets, Peso: peso, MetroCubico: cubagem, Quantidade: quantidade };
    executarReST("RecebimentoMercadoria/BuscarArmazenamento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data.Codigo > 0) {
                _adicionarMercadoria.DepositoPosicao.codEntity(arg.Data.Codigo);
                _adicionarMercadoria.DepositoPosicao.val(arg.Data.Descricao);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function AdicionarMercadoriaClick(e, sender) {
    LimparCampos(_adicionarMercadoria);
    editandoMercadoria = false;

    _adicionarMercadoria.Codigo.val(guid());
    _adicionarMercadoria.Adicionar.text("Adicionar");
    _adicionarMercadoria.ProdutoEmbarcador.val(_recebimentoMercadoria.ProdutoEmbarcador.val());
    _adicionarMercadoria.ProdutoEmbarcador.codEntity(_recebimentoMercadoria.ProdutoEmbarcador.codEntity());
    if (_adicionarMercadoria.Descricao.val() == "")
        _adicionarMercadoria.Descricao.val(_recebimentoMercadoria.ProdutoEmbarcador.val());
    _adicionarMercadoria.Peso.val(pesoUnitarioProduto);

    var qtdPalets = Globalize.parseFloat(_adicionarMercadoria.QuantidadePalet.val());
    if (isNaN(qtdPalets) || qtdPalets == undefined)
        qtdPalets = 0;

    var peso = Globalize.parseFloat(_adicionarMercadoria.Peso.val());
    if (isNaN(peso) || peso == undefined)
        peso = 0;

    var cubagem = Globalize.parseFloat(_adicionarMercadoria.MetroCubico.val());
    if (isNaN(cubagem) || cubagem == undefined)
        cubagem = 0;

    var quantidade = Globalize.parseFloat(_adicionarMercadoria.QuantidadeLote.val());
    if (isNaN(quantidade) || quantidade == undefined)
        quantidade = 0;

    BuscarArmazenamento(_adicionarMercadoria.ProdutoEmbarcador.codEntity(), qtdPalets, peso, cubagem, quantidade);

    _modalAdicionarMercadoria.show();
}

function AdicionarMercadoriaNovaClick(e, sender) {
    if (ValidarCamposObrigatorios(_adicionarMercadoria)) {

        var mercadoriaGrid = new Object();

        mercadoriaGrid.Codigo = _adicionarMercadoria.Codigo.val();
        mercadoriaGrid.CNPJCliente = _adicionarMercadoria.CNPJCliente.val();
        mercadoriaGrid.Descricao = _adicionarMercadoria.Descricao.val();
        mercadoriaGrid.NumeroLote = _adicionarMercadoria.NumeroLote.val();
        mercadoriaGrid.NCM = _adicionarMercadoria.NCM.val();
        mercadoriaGrid.ChaveNFe = _adicionarMercadoria.ChaveNFe.val();
        mercadoriaGrid.Identificacao = _adicionarMercadoria.Identificacao.val();
        mercadoriaGrid.CodigoBarras = _adicionarMercadoria.CodigoBarras.val();
        mercadoriaGrid.DataVencimento = _adicionarMercadoria.DataVencimento.val();
        mercadoriaGrid.QuantidadeLote = _adicionarMercadoria.QuantidadeLote.val();
        mercadoriaGrid.MetroCubico = _adicionarMercadoria.MetroCubico.val();
        mercadoriaGrid.ValorUnitario = _adicionarMercadoria.ValorUnitario.val();
        mercadoriaGrid.Peso = _adicionarMercadoria.Peso.val();
        mercadoriaGrid.QuantidadePalet = _adicionarMercadoria.QuantidadePalet.val();
        mercadoriaGrid.CodigoDepositoPosicao = _adicionarMercadoria.DepositoPosicao.codEntity();
        mercadoriaGrid.CodigoRecebimento = _recebimentoMercadoria.Codigo.val();
        mercadoriaGrid.CodigoProdutoEmbarcador = _adicionarMercadoria.ProdutoEmbarcador.codEntity();
        mercadoriaGrid.CodigoProduto = _adicionarMercadoria.Produto.codEntity();
        mercadoriaGrid.DescricaoDepositoPosicao = _adicionarMercadoria.DepositoPosicao.val();
        mercadoriaGrid.DescricaoRecebimento = _adicionarMercadoria.Codigo.val();
        mercadoriaGrid.DescricaoProdutoEmbarcador = _adicionarMercadoria.ProdutoEmbarcador.val();
        mercadoriaGrid.DescricaoProduto = _adicionarMercadoria.Produto.val();

        executarReST("RecebimentoMercadoria/SalvarMercadoriaVolume", mercadoriaGrid, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _recebimentoMercadoria.Codigo.val(arg.Data.CodigoRecebimento);

                    _gridMercadoria.CarregarGrid();

                    LimparCampos(_adicionarMercadoria);
                    editandoMercadoria = false;

                    _adicionarMercadoria.Codigo.val(guid());
                    _adicionarMercadoria.Adicionar.text("Adicionar");

                    _adicionarMercadoria.ProdutoEmbarcador.val(_recebimentoMercadoria.ProdutoEmbarcador.val());
                    _adicionarMercadoria.ProdutoEmbarcador.codEntity(_recebimentoMercadoria.ProdutoEmbarcador.codEntity());
                    if (_adicionarMercadoria.Descricao.val() == "")
                        _adicionarMercadoria.Descricao.val(_recebimentoMercadoria.ProdutoEmbarcador.val());
                    _adicionarMercadoria.Peso.val(pesoUnitarioProduto);

                    var qtdPalets = Globalize.parseFloat(_adicionarMercadoria.QuantidadePalet.val());
                    if (isNaN(qtdPalets) || qtdPalets == undefined)
                        qtdPalets = 0;

                    var peso = Globalize.parseFloat(_adicionarMercadoria.Peso.val());
                    if (isNaN(peso) || peso == undefined)
                        peso = 0;

                    var cubagem = Globalize.parseFloat(_adicionarMercadoria.MetroCubico.val());
                    if (isNaN(cubagem) || cubagem == undefined)
                        cubagem = 0;

                    var quantidade = Globalize.parseFloat(_adicionarMercadoria.QuantidadeLote.val());
                    if (isNaN(quantidade) || quantidade == undefined)
                        quantidade = 0;

                    BuscarArmazenamento(_adicionarMercadoria.ProdutoEmbarcador.codEntity(), qtdPalets, peso, cubagem, quantidade);

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 16000);
                }

            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);

    }
}

function EditarMercadoriaClick(data) {
    _adicionarMercadoria.Codigo.val(data.Codigo);
    _adicionarMercadoria.CNPJCliente.val(data.CNPJCliente);
    _adicionarMercadoria.CodigoDepositoPosicao.val(data.CodigoDepositoPosicao);
    _adicionarMercadoria.CodigoRecebimento.val(data.CodigoRecebimento);
    _adicionarMercadoria.CodigoProdutoEmbarcador.val(data.CodigoProdutoEmbarcador);
    _adicionarMercadoria.NCM.val(data.NCM);
    _adicionarMercadoria.ChaveNFe.val(data.ChaveNFe);
    _adicionarMercadoria.Identificacao.val(data.Identificacao);

    _adicionarMercadoria.NumeroLote.val(data.NumeroLote);
    _adicionarMercadoria.CodigoBarras.val(data.CodigoBarras);
    _adicionarMercadoria.DataVencimento.val(data.DataVencimento);
    _adicionarMercadoria.QuantidadeLote.val(data.QuantidadeLote);
    _adicionarMercadoria.ValorUnitario.val(data.ValorUnitario);
    _adicionarMercadoria.Produto.val(data.DescricaoProduto);
    _adicionarMercadoria.Produto.codEntity(data.CodigoProduto);
    _adicionarMercadoria.ProdutoEmbarcador.val(data.DescricaoProdutoEmbarcador);
    _adicionarMercadoria.ProdutoEmbarcador.codEntity(data.CodigoProdutoEmbarcador);
    _adicionarMercadoria.Descricao.val(data.Descricao);
    _adicionarMercadoria.QuantidadePalet.val(data.QuantidadePalet);
    _adicionarMercadoria.MetroCubico.val(data.MetroCubico);
    _adicionarMercadoria.Peso.val(data.Peso);
    _adicionarMercadoria.DepositoPosicao.val(data.DescricaoDepositoPosicao);
    _adicionarMercadoria.DepositoPosicao.codEntity(data.CodigoDepositoPosicao);
    editandoMercadoria = true;
    _adicionarMercadoria.Adicionar.text("Atualizar");
    _modalAdicionarMercadoria.show();
}

function ExcluirMercadoriaClick(e) {
    if (_recebimentoMercadoria.Situacao.val() != EnumSituacaoRecebimentoMercadoria.Iniciado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é possivel remover a mercadoria.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja remover a mercadoria selecionada?", function () {
        var data = { Codigo: e.Codigo };
        executarReST("RecebimentoMercadoria/ExcluirMercadoriaVolume", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridMercadoria.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function LimparCamposMercadoria() {
    LimparCampos(_mercadoria);
    _gridMercadoria.CarregarGrid();
}

function BuscarArmazenamento(codigoProduto, quantidadePalet, peso, cubagem, quantidade) {
    if (codigoProduto != undefined && codigoProduto != null && codigoProduto > 0) {
        var data = { CodigoProduto: codigoProduto, QuantidadePalet: quantidadePalet, Peso: peso, MetroCubico: cubagem, Quantidade: quantidade };
        executarReST("RecebimentoMercadoria/BuscarArmazenamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data.Codigo > 0) {
                    _adicionarMercadoria.DepositoPosicao.codEntity(arg.Data.Codigo);
                    _adicionarMercadoria.DepositoPosicao.val(arg.Data.Descricao);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    }
}