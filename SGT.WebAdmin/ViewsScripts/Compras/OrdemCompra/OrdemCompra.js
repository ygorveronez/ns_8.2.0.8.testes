/// <reference path="../../Consultas/Cliente.js" />
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
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/PedidoVenda.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/MotivoCompra.js" />
/// <reference path="Mercadorias.js" />
/// <reference path="Qualificao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ordemCompra;
var _licencaVencida;
var usuarioLogado = null;

var OrdemCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Requisicoes = PropertyEntity({ val: ko.observable(""), def: "" });

    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, enable: false });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor: ", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.ExigeInformarVeiculoObrigatoriamente = PropertyEntity({ getType: typesKnockout.bool });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo: "), idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Motorista: "), idBtnSearch: guid(), required: false, enable: ko.observable(true) });

    this.Data = PropertyEntity({ text: "*Data: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.DataPrevistaRetorno = PropertyEntity({ text: "*Data Prev. Retorno: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });

    this.Operador = PropertyEntity({ type: types.map, text: "*Operador: ", idBtnSearch: guid(), enable: false });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador: ", idBtnSearch: guid(), enable: ko.observable(true) });

    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", getType: typesKnockout.map, enable: ko.observable(true), maxlength: 2000 });
    this.CondicaoPagamento = PropertyEntity({ text: "Condição de Pagamento: ", getType: typesKnockout.map, enable: ko.observable(true), maxlength: 2000 });

    this.Produtos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.Qualificacao = PropertyEntity({ eventClick: QualificacaoClick, type: types.event, text: "Qualificação" });

    this.ValorTotal = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", required: ko.observable(false), text: ko.observable("Valor Total das Mercadorias:"), maxlength: 18, visible: ko.observable(true), enable: ko.observable(false) });

    this.Veiculo.codEntity.subscribe(VerificarObrigatoriedadeVeiculo);
};

var LicencaVencida = function () {
    this.LicencasVencidasGrid = PropertyEntity({ type: types.map, idGrid: guid(), visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Selecionar Todos", visible: ko.observable(true) });
    this.ApenasSemOrdemAberta = PropertyEntity({ text: "Mostrar apenas motorista sem requisição em aberto?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.ApenasSemOrdemAberta.val.subscribe(function (novoValor) {
        ApenasSemOrdemAberta(novoValor);
    });

    this.Codigo = PropertyEntity({ text: "Codigo", getType: typesKnockout.int, visible: ko.observable(false) });
    this.Nome = PropertyEntity({ text: "Motorista", getType: typesKnockout.string, visible: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "Data de Emissão", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ text: "Data de Vencimento", getType: typesKnockout.date, visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "Descrição", getType: typesKnockout.string, visible: ko.observable(true) });
    this.LicencaDescricao = PropertyEntity({ text: "Licença", getType: typesKnockout.string, visible: ko.observable(true) });
    
};

//*******EVENTOS*******
function ApenasSemOrdemAberta(novoValor) {
    console.log(novoValor);
    //here
    BuscarLicencasVencidas(novoValor);
}

function LoadOrdemCompra() {
    carregarLancamentoOrdemCompra("conteudoOrdemCompra");
}

function LoadLicencaVencida() {
    carregarLancamentoLicencaVencida("conteudoLicencaVencida");
}

function carregarLancamentoOrdemCompra(idDivConteudo, callback) {
    $.get("Content/Static/Compras/OrdemCompraDados.html?dyn=" + guid(), function (dataConteudo) {
        $("#" + idDivConteudo).html(dataConteudo);

        $.get("Content/Static/Compras/OrdemCompraModais.html?dyn=" + guid(), function (dataConteudo) {
            $("#OrdemCompraModais").html(dataConteudo);

            _ordemCompra = new OrdemCompra();
            KoBindings(_ordemCompra, "knockoutOrdemCompra");

            new BuscarClientes(_ordemCompra.Fornecedor);
            new BuscarFuncionario(_ordemCompra.Operador);
            new BuscarClientes(_ordemCompra.Transportador);
            new BuscarMotivoCompra(_ordemCompra.Motivo, ValidarVeiculo);
            new BuscarVeiculos(_ordemCompra.Veiculo, RetornoVeiculo);
            new BuscarMotoristas(_ordemCompra.Motorista);
            PreencheUsuarioLogado();

            LoadMercadorias();
            LoadQualificacao();

            if (callback !== undefined && callback !== null)
                callback();
        });
    });
}

function carregarLancamentoLicencaVencida(idDivConteudo, callback) {
    $.get("Content/Static/Compras/LicencaVencidaDados.html?dyn=" + guid(), function (dataConteudo) {
        $("#" + idDivConteudo).html(dataConteudo);

        $.get("Content/Static/Compras/OrdemCompraModais.html?dyn=" + guid(), function (dataConteudo) {
            $("#LicencaVencidaModais").html(dataConteudo);

            _licencaVencida = new LicencaVencida();
            KoBindings(_licencaVencida, "knockoutLicencaVencida");

            BuscarLicencasVencidas();
        });
    });
}

function QualificacaoClick() {
    ExibirQualificacaoFornecedor();
}

//*******MÉTODOS*******

function exibirMultiplasOpcoesOC(e) {
    let possuiSelecionado = _gridLicencasVencidas.ObterMultiplosSelecionados().length > 0;
    let selecionadoTodos = _licencaVencida.SelecionarTodos.val();

    // Esconde todas opções
    _crudLicencaVencida.Gerar.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        _crudLicencaVencida.Gerar.visible(true);
    }
}

function PreencheUsuarioLogado() {
    var _fillName = function () {
        _ordemCompra.Operador.val(usuarioLogado.Nome);
    }

    if (usuarioLogado != null) return _fillName();

    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                usuarioLogado = {
                    Nome: arg.Data.Nome
                };
                _fillName();
            }
        }
    });
}

function ValidarVeiculo(e) {
    _ordemCompra.Motivo.val(e.Descricao);
    _ordemCompra.Motivo.codEntity(e.Codigo);
    _ordemCompra.ExigeInformarVeiculoObrigatoriamente.val(e.ExigeInformarVeiculoObrigatoriamente);

    if (e.ExigeInformarVeiculoObrigatoriamente) {
        _ordemCompra.Veiculo.text("*Veículo: ");
        _ordemCompra.Veiculo.required = true;
    }
    else {
        _ordemCompra.Veiculo.text("Veículo: ");
        _ordemCompra.Veiculo.required = false;
    }
}

function RetornoVeiculo(e) {
    _ordemCompra.Veiculo.val(e.Descricao);
    _ordemCompra.Veiculo.codEntity(e.Codigo);
    _ordemCompra.Motorista.val(e.Motorista);
    _ordemCompra.Motorista.codEntity(e.CodigoMotorista);
}

function VerificarObrigatoriedadeVeiculo() {
    if (_ordemCompra.ExigeInformarVeiculoObrigatoriamente.val()) {
        _ordemCompra.Veiculo.text("*Veículo: ");
        _ordemCompra.Veiculo.required = true;
    }
    else {
        _ordemCompra.Veiculo.text("Veículo: ");
        _ordemCompra.Veiculo.required = false;
    }
}

function ControleCamposOrdemCompra(status) {
    _ordemCompra.Data.enable(status);
    _ordemCompra.Veiculo.enable(status);
    _ordemCompra.Motorista.enable(status);
    _ordemCompra.DataPrevistaRetorno.enable(status);
    _ordemCompra.Fornecedor.enable(status);
    _ordemCompra.Transportador.enable(status);
    _ordemCompra.Motivo.enable(status);
    _ordemCompra.Observacao.enable(status);
    _ordemCompra.CondicaoPagamento.enable(status);

    _mercadorias.Produto.visible(status);
}

function LimparCamposOrdemCompra() {
    LimparCampos(_ordemCompra);
    LimparCamposProduto();
    RecarregarGridProdutos();
    ControleCamposOrdemCompra(true);
    _ordemCompra.Situacao.val(EnumSituacaoOrdemCompra.Aberta);
}