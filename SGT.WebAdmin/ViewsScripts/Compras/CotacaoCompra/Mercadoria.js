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

var _grudCotacaoCompra;
var _cotacaoCompra;
var _gridMercadorias;

var MercadoriaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.string });
    this.CodigoCotacao = PropertyEntity({ type: types.map, val: "" });
    this.CodigoProduto = PropertyEntity({ type: types.map, val: "" });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.Quantidade = PropertyEntity({ type: types.map, val: "" });

    this.CustoUnitario = PropertyEntity({ type: types.map, val: "" });
    this.CustoTotal = PropertyEntity({ type: types.map, val: "" });

    this.ValorUnitario = PropertyEntity({ type: types.map, val: "" });
    this.ValorTotal = PropertyEntity({ type: types.map, val: "" });
}

var _situacaoCotacaoCompra = [
    { text: "Aberto", value: EnumSituacaoCotacao.Aberto },
    { text: "Aguardando Retorno", value: EnumSituacaoCotacao.AguardandoRetorno },
    { text: "Finalizado", value: EnumSituacaoCotacao.Finalizado },
    { text: "Cancelado", value: EnumSituacaoCotacao.Cancelado }
]

var CotacaoCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoMercadoria = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Numero = PropertyEntity({ text: "Número:", required: false, maxlength: 18, getType: typesKnockout.int, enable: false, visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "*Descrição:", maxlength: 2000, enable: ko.observable(true), required: true });
    this.DataEmissao = PropertyEntity({ text: "*Data Emissão: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.DataPrevisao = PropertyEntity({ text: "*Prev. Retorno: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.Situacao = PropertyEntity({ text: "*Situação: ", val: ko.observable(EnumSituacaoCotacao.Aberto), options: _situacaoCotacaoCompra, def: EnumSituacaoCotacao.Aberto, enable: ko.observable(true) });

    this.ItensCotacao = PropertyEntity({ type: types.local });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Produto:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.decimal, required: ko.observable(false), text: ko.observable("*Quantidade:"), maxlength: 10, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ getType: typesKnockout.decimal, required: ko.observable(false), text: ko.observable("*Val. Unit.:"), maxlength: 18, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: false, allowNegative: false } });
    this.ValorTotalItem = PropertyEntity({ getType: typesKnockout.decimal, required: ko.observable(false), text: ko.observable("*Total:"), maxlength: 10, visible: ko.observable(true), enable: false });
    this.SalvarItemCotacao = PropertyEntity({ type: types.event, eventClick: SalvarItemCotacaoClick, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", required: ko.observable(false), text: ko.observable("Valor Total das Mercadorias:"), maxlength: 18, visible: ko.observable(true), enable: ko.observable(false) });

    this.ListaMercadoria = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaFornecedor = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaRetornoProdutoFornecedor = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.CodigoRequisicaoCompra = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });
}

var GRUDCotacaoCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Salvar = PropertyEntity({ type: types.event, eventClick: SalvarClick, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: CancelarClick, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
}


//*******EVENTOS*******
function loadMercadoriaCotacaoCompra() {
    _cotacaoCompra = new CotacaoCompra();
    KoBindings(_cotacaoCompra, "knockoutCotacao_Mercadoria");

    _grudCotacaoCompra = new GRUDCotacaoCompra();
    KoBindings(_grudCotacaoCompra, "knockoutCRUDCotacao");

    new BuscarProdutoTMS(_cotacaoCompra.Produto, RetornoProduto);

    _cotacaoCompra.ListaMercadoria.list = new Array();
    _cotacaoCompra.ListaFornecedor.list = new Array();
    _cotacaoCompra.ListaRetornoProdutoFornecedor.list = new Array();

    CarregarItensCotacao();
}

function RetornoProduto(data) {
    _cotacaoCompra.Produto.val(data.Descricao);
    _cotacaoCompra.Produto.codEntity(data.Codigo);
    _cotacaoCompra.ValorUnitario.val(data.UltimoCusto);
    CalcularTotalItemNovo();
}

function SalvarClick(e, sender) {
    if (_cotacaoCompra.Situacao.val() == EnumSituacaoCotacao.Finalizado) {
        exibirConfirmacao("Finalização", "Após finalizar a Cotação, o sistema irá gerar as Ordens de Compras para os ganhadores. Deseja continuar?", function () {
            if (_cotacaoCompra.ListaFornecedor.list.length > 0)
                SalvarCotacaoCompra();
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", "Para finalizar a cotação de compra é necessário informar um ou mais fornecedores.");
        });
    } else {
        SalvarCotacaoCompra();
    }
}

function SalvarCotacaoCompra() {
    Salvar(_cotacaoCompra, "CotacaoCompra/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridCotacao.CarregarGrid();
                LimparCamposCotacaoCompra();
                _modalCotacaoCompra.hide();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null, function () {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
        finalizarRequisicao();
    });
}

function CancelarClick(e, sender) {
    LimparCamposCotacaoCompra();
    _modalCotacaoCompra.hide();
}

function SalvarItemCotacaoClick(e, sender) {
    var tudoCerto = true;
    _cotacaoCompra.Produto.requiredClass("form-control");
    _cotacaoCompra.Quantidade.requiredClass("form-control");
    _cotacaoCompra.Quantidade.requiredClass("form-control");
    _cotacaoCompra.ValorTotalItem.requiredClass("form-control");

    if (!(_cotacaoCompra.Produto.codEntity() > 0)) {
        tudoCerto = false;
        _cotacaoCompra.Produto.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_cotacaoCompra.Quantidade.val()) <= 0) {
        tudoCerto = false;
        _cotacaoCompra.Quantidade.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_cotacaoCompra.ValorUnitario.val()) <= 0) {
        tudoCerto = false;
        _cotacaoCompra.ValorUnitario.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_cotacaoCompra.ValorTotalItem.val()) <= 0) {
        tudoCerto = false;
        _cotacaoCompra.ValorTotalItem.requiredClass("form-control is-invalid");
    }

    if (tudoCerto) {
        if (_cotacaoCompra.CodigoMercadoria.val() == "") {
            //novo
            var mercadoria = new MercadoriaMap()
            _cotacaoCompra.CodigoMercadoria.val(guid());

            mercadoria.Codigo.val = _cotacaoCompra.CodigoMercadoria.val();
            mercadoria.CodigoCotacao.val = _cotacaoCompra.Codigo.val();
            mercadoria.CodigoProduto.val = _cotacaoCompra.Produto.codEntity();
            mercadoria.Descricao.val = _cotacaoCompra.Produto.val();
            mercadoria.Quantidade.val = _cotacaoCompra.Quantidade.val();
            mercadoria.ValorUnitario.val = _cotacaoCompra.ValorUnitario.val();
            mercadoria.ValorTotal.val = _cotacaoCompra.ValorTotalItem.val();

            _cotacaoCompra.ListaMercadoria.list.push(mercadoria);
            LancarFornecedoresAnterior(_cotacaoCompra.Produto.codEntity());
        } else {
            //editando
            $.each(_cotacaoCompra.ListaMercadoria.list, function (i, mercadoria) {
                if (mercadoria.Codigo.val == _cotacaoCompra.CodigoMercadoria.val()) {

                    mercadoria.CodigoCotacao.val = _cotacaoCompra.Codigo.val();
                    mercadoria.CodigoProduto.val = _cotacaoCompra.Produto.codEntity();
                    mercadoria.Descricao.val = _cotacaoCompra.Produto.val();
                    mercadoria.Quantidade.val = _cotacaoCompra.Quantidade.val();
                    mercadoria.ValorUnitario.val = _cotacaoCompra.ValorUnitario.val();
                    mercadoria.ValorTotal.val = _cotacaoCompra.ValorTotalItem.val();

                    return false;
                }
            });
        }

        RegarregarGridMercadorias();
        RegarregarGridRetornos();
        SomarTotalizadoresCotacao();

        LimparCampoEntity(_cotacaoCompra.Produto);
        _cotacaoCompra.CodigoMercadoria.val("");
        _cotacaoCompra.Quantidade.val("0,00");
        _cotacaoCompra.ValorUnitario.val("0,0000");
        _cotacaoCompra.ValorTotalItem.val("0,00");
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LancarFornecedoresAnterior(codigoProduto) {
    var data = { CodigoProduto: codigoProduto };
    executarReST("CotacaoCompra/BuscarUltimosFornecedores", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != null && arg.Data != false) {
                var contemFornecedor = false;

                $.each(arg.Data.ListaFornecedor, function (i, listaFornecedor) {
                    var fornecedor = new FornecedorMap()

                    fornecedor.Codigo.val = guid();
                    fornecedor.CodigoCotacao.val = _fornecedorCotacaoCompra.Codigo.val();
                    fornecedor.CodigoFornecedor.val = listaFornecedor.CodigoFornecedor;
                    fornecedor.Fornecedor.val = listaFornecedor.Fornecedor;

                    $.each(_cotacaoCompra.ListaFornecedor.list, function (j, fornec) {
                        if (fornec.CodigoFornecedor.val == fornecedor.CodigoFornecedor.val && contemFornecedor == false)
                            contemFornecedor = true;
                    });

                    if (!contemFornecedor)
                        _cotacaoCompra.ListaFornecedor.list.push(fornecedor);
                    contemFornecedor = false;
                });
                RegarregarGridFornecedores();
                RegarregarGridRetornos();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ExcluirItemCotacaoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a mercadoria selecionada?", function () {
        var listaAtualizada = new Array();
        $.each(_cotacaoCompra.ListaMercadoria.list, function (i, mercadoria) {
            if (mercadoria.Codigo.val != data.Codigo) {
                listaAtualizada.push(mercadoria);
            }
        });
        _cotacaoCompra.ListaMercadoria.list = listaAtualizada;

        RegarregarGridMercadorias();
        RegarregarGridRetornos();

        var listaAtualizada = new Array();
        $.each(_cotacaoCompra.ListaRetornoProdutoFornecedor.list, function (i, retorno) {
            if (retorno.Codigo.val != data.CodigoMercadoria) {
                listaAtualizada.push(retorno);
            }
        });
        _cotacaoCompra.ListaRetornoProdutoFornecedor.list = listaAtualizada;

        RegarregarGridRetornoProdutoFornecedor();

        LimparCampoEntity(_cotacaoCompra.Produto);
        _cotacaoCompra.CodigoMercadoria.val("");
        _cotacaoCompra.Quantidade.val("0,00");
        _cotacaoCompra.ValorUnitario.val("0,0000");
        _cotacaoCompra.ValorTotalItem.val("0,00");

        DiminuirTotalizadoresCotacao(data);
    });
}

function EditarItemCotacaoClick(data) {
    _cotacaoCompra.Produto.val(data.Descricao);
    _cotacaoCompra.Produto.codEntity(data.CodigoProduto);
    _cotacaoCompra.CodigoMercadoria.val(data.Codigo);
    _cotacaoCompra.Quantidade.val(data.Quantidade);
    _cotacaoCompra.ValorUnitario.val(data.ValorUnitario);
    _cotacaoCompra.ValorTotalItem.val(data.ValorTotal);

    DiminuirTotalizadoresCotacao(data);
}

function HistoricoItemCotacaoClick(data) {
    _modalHistoricoCompraCotacaoCompra.show();

    _historico.Produto.val(data.CodigoProduto);
    _gridHistorico = new GridView(_historico.Historico.id, "Produto/HistoricoProduto", _historico);
    _gridHistorico.CarregarGrid();
}

function CalcularTotalItemNovo(e, sender) {
    var quantidade = Globalize.parseFloat(_cotacaoCompra.Quantidade.val());
    var valorUnitario = Globalize.parseFloat(_cotacaoCompra.ValorUnitario.val());

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;
        _cotacaoCompra.ValorTotalItem.val(Globalize.format(valorTotal, "n2"));
    }
}

function CalcularValorUnitarioItem(e, sender) {

}

function SomarTotalizadoresCotacao() {
    var valorTotal = Globalize.parseFloat(_cotacaoCompra.ValorTotal.val());
    var valorTotalItem = Globalize.parseFloat(_cotacaoCompra.ValorTotalItem.val());

    _cotacaoCompra.ValorTotal.val(Globalize.format(valorTotal + valorTotalItem, "n2"));
}

function DiminuirTotalizadoresCotacao(data) {
    var valorTotal = Globalize.parseFloat(_cotacaoCompra.ValorTotal.val());
    var valorTotalItem = Globalize.parseFloat(data.ValorTotal);

    _cotacaoCompra.ValorTotal.val(Globalize.format(valorTotal - valorTotalItem, "n2"));
}

//*******MÉTODOS*******

function CarregarItensCotacao() {
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: ExcluirItemCotacaoClick, tamanho: "10", icone: "" };
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarItemCotacaoClick, tamanho: "10", icone: "" };
    var historico = { descricao: "Histórico", id: guid(), evento: "onclick", metodo: HistoricoItemCotacaoClick, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [excluir, editar, historico], descricao: "Opções", tamanho: 10 };

    var header = [{ data: "Codigo", visible: false },
    { data: "CodigoCotacao", visible: false },
    { data: "CodigoProduto", visible: false },
    { data: "Descricao", title: "Descrição", width: "40%" },
    { data: "Quantidade", title: "Qtd.", width: "10%" },
    { data: "ValorUnitario", title: "Val. Unit.", width: "10%" },
    { data: "ValorTotal", title: "Val. Total", width: "10%" }];

    _gridMercadorias = new BasicDataTable(_cotacaoCompra.ItensCotacao.id, header, menuOpcoes, { column: 3, dir: orderDir.asc });
    RegarregarGridMercadorias();
}

function RegarregarGridMercadorias() {
    var data = new Array();

    $.each(_cotacaoCompra.ListaMercadoria.list, function (i, listaMercadoria) {
        var listaMercadoriaGrid = new Object();

        listaMercadoriaGrid.Codigo = listaMercadoria.Codigo.val;
        listaMercadoriaGrid.CodigoCotacao = listaMercadoria.CodigoCotacao.val;
        listaMercadoriaGrid.CodigoProduto = listaMercadoria.CodigoProduto.val;
        listaMercadoriaGrid.Descricao = listaMercadoria.Descricao.val;
        listaMercadoriaGrid.Quantidade = listaMercadoria.Quantidade.val;
        listaMercadoriaGrid.ValorUnitario = listaMercadoria.ValorUnitario.val;
        listaMercadoriaGrid.ValorTotal = listaMercadoria.ValorTotal.val;

        data.push(listaMercadoriaGrid);
    });

    _gridMercadorias.CarregarGrid(data);
}

function EnableCamposCotacaoCompra(status) {
    SetarEnableCamposKnockout(_cotacaoCompra, status);
    SetarEnableCamposKnockout(_grudCotacaoCompra, status);
    SetarEnableCamposKnockout(_fornecedorCotacaoCompra, status);
    SetarEnableCamposKnockout(_retornoProdutoFornecedor, status);

    if (!status) {
        _gridMercadorias.DesabilitarOpcoes();
        _gridFornecedores.DesabilitarOpcoes();
        _gridRetornoProdutoFornecedor.DesabilitarOpcoes();
    } else {
        _gridMercadorias.HabilitarOpcoes();
        _gridFornecedores.HabilitarOpcoes();
        _gridRetornoProdutoFornecedor.HabilitarOpcoes();
    }
}

function LimparCamposCotacaoCompra() {
    let triggerEl = document.querySelector('a[href="#divMercadoria"]');
    let firstTab = new bootstrap.Tab(triggerEl);
    firstTab.show();

    LimparCampos(_cotacaoCompra);
    LimparCampos(_fornecedorCotacaoCompra);
    LimparCampos(_retornoCotacaoCompra);
    LimparCampos(_retornoProdutoFornecedor);

    _cotacaoCompra.Produto.requiredClass("form-control");
    _cotacaoCompra.Quantidade.requiredClass("form-control");
    _cotacaoCompra.Quantidade.requiredClass("form-control");
    _cotacaoCompra.ValorTotalItem.requiredClass("form-control");

    _fornecedorCotacaoCompra.Fornecedor.requiredClass("form-control");

    _cotacaoCompra.ListaMercadoria.list = new Array();
    _cotacaoCompra.ListaFornecedor.list = new Array();
    _cotacaoCompra.ListaRetornoProdutoFornecedor.list = new Array();
    _cotacaoCompra.CodigoRequisicaoCompra.val("");

    RegarregarGridMercadorias();
    RegarregarGridRetornos();
    RegarregarGridFornecedores();
    RegarregarGridRetornoProdutoFornecedor();

    EnableCamposCotacaoCompra(true);
    _cotacaoCompra.Numero.enable = false;
    _cotacaoCompra.ValorTotal.enable(false);
    _grudCotacaoCompra.Salvar.visible(true);
}