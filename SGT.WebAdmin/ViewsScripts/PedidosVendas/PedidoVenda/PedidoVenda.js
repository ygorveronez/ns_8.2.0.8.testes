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
/// <reference path="PedidoVendaEtapa.js" />
/// <reference path="PedidoVendaItens.js" />
/// <reference path="PedidoVendaParcela.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumTipoPedidoVenda.js" />
/// <reference path="../../Enumeradores/EnumStatusPedidoVenda.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoPesquisa = [
    { text: "Todos", value: -1 },
    { text: "Cotação", value: EnumTipoPedidoVenda.Cotacao },
    { text: "Pedido", value: EnumTipoPedidoVenda.Pedido }
];

var _tipoPedidoVenda = [
    { text: "Cotação", value: EnumTipoPedidoVenda.Cotacao },
    { text: "Pedido", value: EnumTipoPedidoVenda.Pedido }
];

var _pedidoVenda;
var _pesquisaPedidoVenda;
var _gridPedidoVenda;
var _casasQuantidadeProdutoNFe;
var _casasValorProdutoNFe;
var _bloquearDataEntregaDiferenteAtual;
var _observacaoInterna;

var ObservacaoInterna = function () {
    this.ObservacaoInterna = PropertyEntity({ text: "Observação Interna:", maxlength: 5000, enable: ko.observable(false), val: ko.observable("") });
};

var PesquisaPedidoVenda = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, maxlength: 16 });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int, maxlength: 16 });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Emissão Inicial: ", getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Emissão Final: ", getType: typesKnockout.date });
    this.Tipo = PropertyEntity({ val: ko.observable(0), options: _tipoPesquisa, def: 0, text: "Tipo: " });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusPedidoVenda.Todos), options: EnumStatusPedidoVenda.obterOpcoesPesquisa(), def: EnumStatusPedidoVenda.Todos, text: "Status: " });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPedidoVenda.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var PedidoVenda = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "*Número:", required: false, maxlength: 10, getType: typesKnockout.int, enable: ko.observable(false), val: ko.observable("") });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoPedidoVenda.Pedido), options: _tipoPedidoVenda, def: EnumTipoPedidoVenda.Pedido, text: "*Tipo: ", enable: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusPedidoVenda.Aberta), options: EnumStatusPedidoVenda.obterOpcoes(), def: EnumStatusPedidoVenda.Aberta, text: "*Status: ", enable: ko.observable(false) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pessoa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), issue: 52 });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Funcionário:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Referencia = PropertyEntity({ text: "Referência:", maxlength: 5000, enable: ko.observable(true), val: ko.observable("") });

    this.DataEmissao = PropertyEntity({ text: "*Data Emissão: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: true, val: ko.observable(Global.DataHoraAtual()), def: ko.observable(Global.DataHoraAtual()) });
    this.DataEntrega = PropertyEntity({ text: "*Data Entrega: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: true, val: ko.observable(Global.DataHoraAtual()), def: ko.observable(Global.DataHoraAtual()) });

    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, enable: ko.observable(true), val: ko.observable("") });    
    
    this.ValorProdutos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Produtos:", maxlength: 10, enable: ko.observable(false) });
    this.ValorServicos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Serviços:", maxlength: 10, enable: ko.observable(false) });
    this.ValorTotal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Total:", maxlength: 10, enable: ko.observable(false) });

    this.Finalizar = PropertyEntity({ eventClick: FinalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
    this.EnviarEmail = PropertyEntity({ eventClick: EnviarEmailClick, type: types.event, text: "Enviar E-mail", visible: ko.observable(false), enable: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(false), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Proximo = PropertyEntity({ eventClick: ProximoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });

    this.ListaItens = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaParcelas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.FormaPagamento = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CondicaoPagamentoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", visible: ko.observable(false) });

    //this.DetalheParcelaPedido = PropertyEntity({ type: types.map });//getType: typesKnockout.string
    //this.ParcelamentoPedido = PropertyEntity({ type: types.map });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "PedidoVenda/Importar",
        UrlConfiguracao: "PedidoVenda/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O058_PedidoVenda,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: false
            };
        },
        CallbackImportacao: function () {
            _gridPedidoVenda.CarregarGrid();
        }
    });

};

//*******EVENTOS*******

function loadPedidoVenda() {
    executarReST("NotaFiscalEletronica/BuscarDadosEmpresa", null, function (r) {
        if (r.Success) {
            _casasQuantidadeProdutoNFe = r.Data.CasasQuantidadeProdutoNFe;
            _casasValorProdutoNFe = r.Data.CasasValorProdutoNFe;
            _bloquearDataEntregaDiferenteAtual = r.Data.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual;

            _observacaoInterna = new ObservacaoInterna();
            KoBindings(_observacaoInterna, "knoutObservacaoInterna");

            _pedidoVenda = new PedidoVenda();
            KoBindings(_pedidoVenda, "knockoutPedidoVenda");

            _pesquisaPedidoVenda = new PesquisaPedidoVenda();
            KoBindings(_pesquisaPedidoVenda, "knockoutPesquisaPedidoVenda");

            HeaderAuditoria("PedidoVenda", _pedidoVenda);

            new BuscarClientes(_pesquisaPedidoVenda.Pessoa);
            new BuscarFuncionario(_pesquisaPedidoVenda.Funcionario);
            new BuscarClientes(_pedidoVenda.Pessoa, RetornoPessoa, true);
            new BuscarFuncionario(_pedidoVenda.Funcionario);

            buscarPedidosVendas();
            loadEtapaPedidoVenda();
            buscarFuncionarioLogado();
            loadPedidoVendaItens();

            _pedidoVenda.DetalheParcelaPedido = new DetalheParcelaPedido(_pedidoVenda);
            _pedidoVenda.ParcelamentoPedido = new ParcelamentoPedido(_pedidoVenda, _pedidoVenda.DetalheParcelaPedido);
            _pedidoVenda.DetalheParcelaPedido.Load();
            _pedidoVenda.ParcelamentoPedido.Load();

            new BuscarCondicaoPagamento(_pedidoVenda.ParcelamentoPedido.CondicaoPagamentoPadrao, RetornoCondicaoPagamento);

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function RetornoPessoa(data) {
    _pedidoVenda.Pessoa.val(data.Descricao);
    _pedidoVenda.Pessoa.codEntity(data.Codigo);
    _observacaoInterna.ObservacaoInterna.val("");
    var observacaoInterna = data.ObservacaoInterna;
    if (observacaoInterna !== "" && observacaoInterna !== undefined && observacaoInterna !== null) {
        _observacaoInterna.ObservacaoInterna.val(observacaoInterna);
        Global.abrirModal("divObservacaoInterna");
        //divObservacaoInterna
    }

    var dados = {
        Codigo: data.CondicaoPagamentoCodigo,
        Descricao: data.CondicaoPagamentoDescricao,
        QuantidadeParcelas: data.CondicaoPagamentoQuantidadeParcelas,
        IntervaloDias: data.CondicaoPagamentoIntervaloDias,
    }

    RetornoCondicaoPagamento(dados)
}

function RetornoCondicaoPagamento(data) {
    if (data == null)
        return;

    _pedidoVenda.ParcelamentoPedido.CondicaoPagamentoPadrao.codEntity(data.Codigo);
    _pedidoVenda.ParcelamentoPedido.CondicaoPagamentoPadrao.val(data.Descricao);
    _pedidoVenda.ParcelamentoPedido.QuantidadeParcelas.val(data.QuantidadeParcelas);
    _pedidoVenda.ParcelamentoPedido.IntervaloDeDias.val(data.IntervaloDias);
}

function ProximoClick(e, sender) {
    $("#" + _etapaPedidoVenda.Etapa1.idTab + " .step").attr("class", "step green");
    $("#" + _etapaPedidoVenda.Etapa2.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaPedidoVenda.Etapa2.idTab).tab('show');

}

function LimparClick(e, sender) {
    _gridPedidoVenda.CarregarGrid();
    limparCamposPedidoVenda();
    limparCamposPedidoVendaItens();
    _pedidoVendaItens.ValorProdutos.val("0,00");
    _pedidoVendaItens.ValorServicos.val("0,00");
    _pedidoVendaItens.ValorTotal.val("0,00");
    _pedidoVenda.ListaItens.list = new Array();
    recarregarGridListaItens();
    _pedidoVenda.Finalizar.visible(false);
    _pedidoVenda.Cancelar.visible(false);
    _pedidoVenda.EnviarEmail.visible(false);
    _pedidoVenda.Imprimir.visible(false);

    SetarEnableCamposKnockout(_pedidoVenda, true);
    SetarEnableCamposKnockout(_pedidoVendaItens, true);
    _pedidoVenda.Numero.enable(false);
    _pedidoVenda.Status.enable(false);
    _pedidoVenda.ValorProdutos.enable(false);
    _pedidoVenda.ValorServicos.enable(false);
    _pedidoVenda.ValorTotal.enable(false);
    _pedidoVendaItens.ValorProdutos.enable(false);
    _pedidoVendaItens.ValorServicos.enable(false);
    _pedidoVendaItens.ValorTotal.enable(false);
    _pedidoVendaItens.ValorTotalItem.enable(false);

    buscarFuncionarioLogado();
}

function FinalizarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja Finalizar o Pedido de número " + _pedidoVenda.Numero.val() + "? Com isso não será mais possível editar!", function () {
        _pedidoVenda.Status.val(EnumStatusPedidoVenda.Finalizada);
        GravarPedidoVendaItensClick();
    });
}

function CancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja Cancelar o Pedido de número " + _pedidoVenda.Numero.val() + "?", function () {
        _pedidoVenda.Status.val(EnumStatusPedidoVenda.Cancelada);
        GravarPedidoVendaItensClick();
    });
}

function EnviarEmailClick(e, sender) {
    var cliente = { Pessoa: _pedidoVenda.Pessoa.codEntity() };
    executarReST("PedidoVenda/VerificaClienteTemEmail", cliente, function (arg) {
        if (arg.Success) {

            var data = { Codigo: _pedidoVenda.Codigo.val() };
            executarReST("PedidoVenda/EnviarPorEmail", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        BuscarProcessamentosPendentes();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            })

        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
    })
}

function ImprimirClick(e, sender) {
    var data = { Codigo: _pedidoVenda.Codigo.val() };
    executarReST("PedidoVenda/BaixarRelatorio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    })
}

//*******MÉTODOS*******

function buscarFuncionarioLogado() {
    executarReST("PedidoVenda/BuscarFuncionarioLogado", null, function (r) {
        if (r.Success) {
            _pedidoVenda.Funcionario.codEntity(r.Data.Codigo);
            _pedidoVenda.Funcionario.val(r.Data.Nome);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function editarPedidoVenda(pedidoVendaGrid) {
    LimparClick();
    _pedidoVenda.Codigo.val(pedidoVendaGrid.Codigo);
    BuscarPorCodigo(_pedidoVenda, "PedidoVenda/BuscarPorCodigo", function (arg) {
        _pesquisaPedidoVenda.ExibirFiltros.visibleFade(false);
        _pedidoVendaItens.ValorProdutos.val(_pedidoVenda.ValorProdutos.val());
        _pedidoVendaItens.ValorServicos.val(_pedidoVenda.ValorServicos.val());
        _pedidoVendaItens.ValorTotal.val(_pedidoVenda.ValorTotal.val());
        recarregarGridListaItens();
        _pedidoVenda.ParcelamentoPedido.RecarregarGrid();
        _pedidoVenda.ParcelamentoPedido.FormaPagamento.val(arg.Data.FormaPagamento);
        _pedidoVenda.ParcelamentoPedido.CondicaoPagamentoPadrao.codEntity(arg.Data.CondicaoPagamentoCodigo);
        _pedidoVenda.ParcelamentoPedido.CondicaoPagamentoPadrao.val(arg.Data.CondicaoPagamentoDescricao);
        _pedidoVenda.ParcelamentoPedido.QuantidadeParcelas.val(arg.Data.QuantidadeParcelas);

        $("#" + _etapaPedidoVenda.Etapa3.idTab + " .step").attr("class", "step ")
        $("#" + _etapaPedidoVenda.Etapa2.idTab + " .step").attr("class", "step ");
        $("#" + _etapaPedidoVenda.Etapa1.idTab).tab('show');

        _pedidoVenda.Finalizar.visible(false);
        _pedidoVenda.Cancelar.visible(false);
        _pedidoVenda.EnviarEmail.visible(true);
        _pedidoVenda.Imprimir.visible(true);
        SetarEnableCamposKnockout(_pedidoVenda, true);
        SetarEnableCamposKnockout(_pedidoVendaItens, true);
        SetarEnableCamposKnockout(_pedidoVenda.DetalheParcelaPedido, true);
        SetarEnableCamposKnockout(_pedidoVenda.ParcelamentoPedido, true);
        if (arg.Data.Status !== EnumStatusPedidoVenda.Aberta) {
            SetarEnableCamposKnockout(_pedidoVenda, false);
            SetarEnableCamposKnockout(_pedidoVendaItens, false);
            SetarEnableCamposKnockout(_pedidoVenda.DetalheParcelaPedido, false);
            SetarEnableCamposKnockout(_pedidoVenda.ParcelamentoPedido, false);
        }
        else
            _pedidoVenda.Finalizar.visible(true);
        if (arg.Data.Status !== EnumStatusPedidoVenda.Cancelada)
            _pedidoVenda.Cancelar.visible(true);

        _pedidoVenda.Numero.enable(false);
        _pedidoVenda.Status.enable(false);
        _pedidoVenda.ValorProdutos.enable(false);
        _pedidoVenda.ValorServicos.enable(false);
        _pedidoVenda.ValorTotal.enable(false);
        _pedidoVendaItens.ValorProdutos.enable(false);
        _pedidoVendaItens.ValorServicos.enable(false);
        _pedidoVendaItens.ValorTotal.enable(false);
        _pedidoVendaItens.ValorTotalItem.enable(false);
        if (_bloquearDataEntregaDiferenteAtual)
            _pedidoVenda.DataEmissao.enable(false);
    }, null);
}

function buscarPedidosVendas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPedidoVenda, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPedidoVenda = new GridView(_pesquisaPedidoVenda.Pesquisar.idGrid, "PedidoVenda/Pesquisa", _pesquisaPedidoVenda, menuOpcoes, null, null, null);
    _gridPedidoVenda.CarregarGrid();
}

function limparCamposPedidoVenda() {
    _pedidoVenda.ParcelamentoPedido.LimparCamposParcelamentoPedido();

    _pedidoVenda.Codigo.val(0);

    _pedidoVenda.Numero.val("");
    _pedidoVenda.Tipo.val(EnumTipoPedidoVenda.Pedido);
    _pedidoVenda.Status.val(EnumStatusPedidoVenda.Aberta);
    LimparCampoEntity(_pedidoVenda.Pessoa);
    LimparCampoEntity(_pedidoVenda.Funcionario);

    _pedidoVenda.Observacao.val("");
    _pedidoVenda.Referencia.val("");

    _pedidoVenda.ValorProdutos.val("0,00");
    _pedidoVenda.ValorServicos.val("0,00");
    _pedidoVenda.ValorTotal.val("0,00");

    _pedidoVenda.DataEmissao.val(Global.DataHoraAtual());
    _pedidoVenda.DataEntrega.val(Global.DataHoraAtual());
}