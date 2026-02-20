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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Enumeradores/EnumStatusPedidoVenda.js" />
/// <reference path="OrdemServicoVendaEtapa.js" />
/// <reference path="OrdemServicoVendaMaoObra.js" />
/// <reference path="OrdemServicoVendaPecas.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ordemServicoVenda;
var _pesquisaOrdemServicoVenda;
var _gridOrdemServicoVenda;
var _configuracoesEmpresa;
var editando = false;

var PesquisaOrdemServicoVenda = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, maxlength: 11 });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int, maxlength: 11 });
    this.NumeroInternoInicial = PropertyEntity({ text: "Número Interno Inicial: ", getType: typesKnockout.int, maxlength: 11, visible: ko.observable(_configuracoesEmpresa.HabilitarNumeroInternoOrdemServicoVenda) });
    this.NumeroInternoFinal = PropertyEntity({ text: "Número Interno Final: ", getType: typesKnockout.int, maxlength: 11, visible: ko.observable(_configuracoesEmpresa.HabilitarNumeroInternoOrdemServicoVenda) });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Emissão Inicial: ", getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Emissão Final: ", getType: typesKnockout.date });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusPedidoVenda.Todos), options: EnumStatusPedidoVenda.obterOpcoesPesquisa(), def: EnumStatusPedidoVenda.Todos, text: "Status: " });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOrdemServicoVenda.CarregarGrid();
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
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(_configuracoesEmpresa.HabilitarNumeroInternoOrdemServicoVenda)
    });
};

var OrdemServicoVenda = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "*Número:", required: false, maxlength: 10, getType: typesKnockout.int, enable: ko.observable(false), val: ko.observable("") });
    this.NumeroInterno = PropertyEntity({ text: "Número Interno:", maxlength: 11, getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(_configuracoesEmpresa.HabilitarNumeroInternoOrdemServicoVenda) });
    this.KM = PropertyEntity({ text: "KM:", required: false, maxlength: 10, getType: typesKnockout.int, enable: ko.observable(true), val: ko.observable("") });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusPedidoVenda.Aberta), options: EnumStatusPedidoVenda.obterOpcoes(), def: EnumStatusPedidoVenda.Aberta, text: "*Status: ", enable: ko.observable(_configuracoesEmpresa.HabilitarNumeroInternoOrdemServicoVenda), visible: ko.observable(_configuracoesEmpresa.HabilitarNumeroInternoOrdemServicoVenda) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pessoa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), issue: 52 });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Funcionário:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.FuncionarioSolicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa Solicitante:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.PessoaSolicitante = PropertyEntity({ text: "Pessoa Solicitante:", maxlength: 500, enable: ko.observable(true), visible: ko.observable(_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.DataEmissao = PropertyEntity({ text: "*Data Emissão: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: true, val: ko.observable(Global.DataHoraAtual()), def: ko.observable(Global.DataHoraAtual()) });
    this.DataEntrega = PropertyEntity({ text: "*Data Entrega: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: true, val: ko.observable(Global.DataHoraAtual()), def: ko.observable(Global.DataHoraAtual()) });
    this.Referencia = PropertyEntity({ text: "Referência:", maxlength: 5000, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-12 col-lg-12") });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, enable: ko.observable(true) });
    this.ValorProdutos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Produtos:", maxlength: 10, enable: ko.observable(false) });
    this.ValorServicos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Serviços:", maxlength: 10, enable: ko.observable(false) });
    this.ValorTotal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Total:", maxlength: 10, enable: ko.observable(false) });
    this.ValorDesconto = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Desconto:", maxlength: 10, enable: ko.observable(true) });
    this.PercentualDesconto = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "% Desconto:", maxlength: 10, enable: ko.observable(true) });

    //CRUD
    this.Finalizar = PropertyEntity({ eventClick: FinalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
    this.EnviarEmail = PropertyEntity({ eventClick: EnviarEmailClick, type: types.event, text: "Enviar E-mail", visible: ko.observable(false), enable: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(false), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Gravar = PropertyEntity({ eventClick: GravarOrdemServicoVendaClick, type: types.event, text: "Gravar OS", visible: ko.observable(true), enable: ko.observable(true) });

    this.ListaItens = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaMaoObras = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

//*******EVENTOS*******

function loadOrdemServicoVenda() {
    executarReST("OrdemServicoVenda/BuscarConfiguracoesEmpresa", null, function (arg) {
        if (arg.Success) {
            _configuracoesEmpresa = arg.Data;

            _ordemServicoVenda = new OrdemServicoVenda();
            KoBindings(_ordemServicoVenda, "knockoutOrdemServicoVenda");

            _pesquisaOrdemServicoVenda = new PesquisaOrdemServicoVenda();
            KoBindings(_pesquisaOrdemServicoVenda, "knockoutPesquisaOrdemServicoVenda", false, _pesquisaOrdemServicoVenda.Pesquisar.id);

            HeaderAuditoria("PedidoVenda", _ordemServicoVenda);

            new BuscarClientes(_pesquisaOrdemServicoVenda.Pessoa);
            new BuscarFuncionario(_pesquisaOrdemServicoVenda.Funcionario);
            new BuscarClientes(_ordemServicoVenda.Pessoa, null, true);
            new BuscarFuncionario(_ordemServicoVenda.Funcionario);
            new BuscarFuncionario(_ordemServicoVenda.FuncionarioSolicitante);
            new BuscarVeiculos(_pesquisaOrdemServicoVenda.Veiculo);
            new BuscarVeiculos(_ordemServicoVenda.Veiculo, RetornoVeiculo);
            new BuscarEmpresa(_ordemServicoVenda.Empresa, null, false, true);

            loadEtapaOrdemServicoVenda();
            loadOrdemServicoVendaPecas();
            loadOrdemServicoVendaMaoObra();

            buscarFuncionarioLogado();
            buscarOrdensServicosVendas();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function RetornoVeiculo(data) {
    _ordemServicoVenda.Veiculo.codEntity(data.Codigo);
    _ordemServicoVenda.Veiculo.val(data.DescricaoComMarcaModelo);
}

function GravarOrdemServicoVendaClick(e, sender) {
    if (_ordemServicoVendaPecas.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return;
    }
    if (_ordemServicoVendaPecas.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return;
    }
    if (_ordemServicoVendaMaoObras.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return;
    }

    Salvar(_ordemServicoVenda, "OrdemServicoVenda/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso");

                if (arg.Data.RecarregarOrdem)
                    editarOrdemServicoVenda(arg.Data);
                else
                    LimparClick();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null, function () {
        $("#" + _etapaOrdemServicoVenda.Etapa1.idTab).click();
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    });
}

function LimparClick(e, sender) {
    limparCamposOrdemServicoVenda();
    limparCamposOrdemServicoVendaMaoObra();
    limparCamposOrdemServicoVendaPecas();

    _ordemServicoVendaPecas.ValorProdutos.val("0,00");
    _ordemServicoVendaMaoObras.ValorServicos.val("0,00");
    _ordemServicoVendaPecas.ValorTotal.val("0,00");
    _ordemServicoVendaMaoObras.ValorTotal.val("0,00");
    recarregarGridListaItens();
    recarregarGridListaMaoObra();
    _ordemServicoVenda.Finalizar.visible(false);
    _ordemServicoVenda.Cancelar.visible(false);
    _ordemServicoVenda.EnviarEmail.visible(false);
    _ordemServicoVenda.Imprimir.visible(false);

    SetarEnableCamposKnockout(_ordemServicoVenda, true);
    SetarEnableCamposKnockout(_ordemServicoVendaPecas, true);
    SetarEnableCamposKnockout(_ordemServicoVendaMaoObras, true);

    _ordemServicoVenda.Numero.enable(false);
    if (!_configuracoesEmpresa.HabilitarNumeroInternoOrdemServicoVenda)
        _ordemServicoVenda.Status.enable(false);
    _ordemServicoVenda.ValorProdutos.enable(false);
    _ordemServicoVenda.ValorServicos.enable(false);
    _ordemServicoVenda.ValorTotal.enable(false);

    _ordemServicoVendaPecas.ValorProdutos.enable(false);
    _ordemServicoVendaPecas.ValorTotal.enable(false);
    _ordemServicoVendaPecas.ValorTotalItem.enable(false);

    _ordemServicoVendaMaoObras.ValorServicos.enable(false);
    _ordemServicoVendaMaoObras.ValorTotal.enable(false);
    _ordemServicoVendaMaoObras.ValorTotalItem.enable(false);
    _ordemServicoVendaMaoObras.KMTotal.enable(false);
    _ordemServicoVendaMaoObras.HoraTotal.enable(false);
    _ordemServicoVendaMaoObras.ValorTotalKM.enable(false);
    _ordemServicoVendaMaoObras.ValorTotalHora.enable(false);
    _ordemServicoVendaMaoObras.HoraTotal2.enable(false);
    _ordemServicoVendaMaoObras.KMTotal2.enable(false);

    if (!editando) {
        _gridOrdemServicoVenda.CarregarGrid();
        buscarFuncionarioLogado();
    }
}

function FinalizarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja Finalizar a Ordem de Serviço de número " + _ordemServicoVenda.Numero.val() + "? Com isso não será mais possível editar!", function () {
        _ordemServicoVenda.Status.val(EnumStatusPedidoVenda.Finalizada);
        GravarOrdemServicoVendaClick();
    });
}

function CancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja Cancelar a Ordem de Serviço de número " + _ordemServicoVenda.Numero.val() + "?", function () {
        _ordemServicoVenda.Status.val(EnumStatusPedidoVenda.Cancelada);
        GravarOrdemServicoVendaClick();
    });
}

function EnviarEmailClick(e, sender) {
    var cliente = { Pessoa: _ordemServicoVenda.Pessoa.codEntity() };
    executarReST("OrdemServicoVenda/VerificaClienteTemEmail", cliente, function (arg) {
        if (arg.Success) {

            var data = { Codigo: _ordemServicoVenda.Codigo.val() };
            executarReST("OrdemServicoVenda/EnviarPorEmail", data, function (arg) {
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
            });

        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
    });
}

function ImprimirClick(e, sender) {
    var data = { Codigo: _ordemServicoVenda.Codigo.val() };
    executarReST("OrdemServicoVenda/BaixarRelatorio", data, function (arg) {
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
    });
}

//*******MÉTODOS*******

function buscarFuncionarioLogado() {
    executarReST("OrdemServicoVenda/BuscarFuncionarioLogado", null, function (r) {
        if (r.Success) {
            var dados = r.Data;
            _ordemServicoVenda.Funcionario.codEntity(dados.Codigo);
            _ordemServicoVenda.Funcionario.val(dados.Nome);
        } else
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
    });
}

function editarOrdemServicoVenda(pedidoVendaGrid) {
    editando = true;
    LimparClick();
    _ordemServicoVenda.Codigo.val(pedidoVendaGrid.Codigo);
    BuscarPorCodigo(_ordemServicoVenda, "OrdemServicoVenda/BuscarPorCodigo", function (arg) {
        _pesquisaOrdemServicoVenda.ExibirFiltros.visibleFade(false);
        _ordemServicoVendaPecas.ValorProdutos.val(_ordemServicoVenda.ValorProdutos.val());
        _ordemServicoVendaMaoObras.ValorServicos.val(_ordemServicoVenda.ValorServicos.val());
        _ordemServicoVendaPecas.ValorTotal.val(_ordemServicoVenda.ValorTotal.val());
        _ordemServicoVendaMaoObras.ValorTotal.val(_ordemServicoVenda.ValorTotal.val());
        recarregarGridListaItens();
        recarregarGridListaMaoObra();

        $("#" + _etapaOrdemServicoVenda.Etapa1.idTab).click();
        if (arg.Data.Status !== EnumStatusPedidoVenda.Aberta && arg.Data.Status !== EnumStatusPedidoVenda.PendenteOperacional && arg.Data.Status !== EnumStatusPedidoVenda.EmAprovacao) {
            SetarEnableCamposKnockout(_ordemServicoVenda, false);
            SetarEnableCamposKnockout(_ordemServicoVendaPecas, false);
            SetarEnableCamposKnockout(_ordemServicoVendaMaoObras, false);
        }
        else {
            _ordemServicoVenda.Finalizar.visible(true);

            if (_configuracoesEmpresa.PermiteAlterarEmpresaOrdemServicoVenda) {
                _ordemServicoVenda.Empresa.visible(true);
                _ordemServicoVenda.Referencia.cssClass("col col-xs-12 col-sm-12 col-md-8 col-lg-8");
            }
        }

        if (arg.Data.Status !== EnumStatusPedidoVenda.Cancelada)
            _ordemServicoVenda.Cancelar.visible(true);

        _ordemServicoVenda.EnviarEmail.visible(true);
        _ordemServicoVenda.Imprimir.visible(true);

        editando = false;
    }, null);
}

function buscarOrdensServicosVendas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarOrdemServicoVenda, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridOrdemServicoVenda = new GridView(_pesquisaOrdemServicoVenda.Pesquisar.idGrid, "OrdemServicoVenda/Pesquisa", _pesquisaOrdemServicoVenda, menuOpcoes, null, null, null);
    _gridOrdemServicoVenda.CarregarGrid();
}

function RatearValorDescontoAosItens() {
    RatearDescontoAosItens(false);
}

function RatearPercentualDescontoAosItens() {
    RatearDescontoAosItens(true);
}

function RatearDescontoAosItens(porPercentual) {
    var valorTotal = Globalize.parseFloat(_ordemServicoVenda.ValorTotal.val());
    var valorDesconto = Globalize.parseFloat(_ordemServicoVenda.ValorDesconto.val()) || 0;
    var percentualDesconto = Globalize.parseFloat(_ordemServicoVenda.PercentualDesconto.val()) || 0;

    if (valorDesconto > valorTotal || percentualDesconto > 99.99) {
        _ordemServicoVenda.ValorDesconto.val("0,00");
        _ordemServicoVenda.PercentualDesconto.val("0,00");

        RemoverDescontoItens();
        recarregarGridListaItens();
        recarregarGridListaMaoObra();

        return exibirMensagem(tipoMensagem.atencao, "Atenção", "Desconto informado maior que o Valor Total");
    }

    if ((porPercentual && percentualDesconto == 0) || (!porPercentual && valorDesconto == 0)) {
        _ordemServicoVenda.ValorDesconto.val("0,00");
        _ordemServicoVenda.PercentualDesconto.val("0,00");
        valorDesconto = 0;
        percentualDesconto = 0;
    }

    RemoverDescontoItens();
    if (valorDesconto > 0 || percentualDesconto > 0)
        AplicarDescontoItens(porPercentual);

    recarregarGridListaItens();
    recarregarGridListaMaoObra();
}

function AplicarDescontoItens(porPercentual) {
    var valorTotal = Globalize.parseFloat(_ordemServicoVenda.ValorTotal.val());
    var valorDesconto = Globalize.parseFloat(_ordemServicoVenda.ValorDesconto.val()) || 0;
    var percentualDesconto = Globalize.parseFloat(_ordemServicoVenda.PercentualDesconto.val()) || 0;

    if (percentualDesconto > 0 && porPercentual)
        valorDesconto = (valorTotal * percentualDesconto) / 100;

    if (valorDesconto > 0 && !porPercentual)
        percentualDesconto = (valorDesconto / valorTotal) * 100;

    var somaNovoValorTotal = 0;
    var somaNovoValorProdutos = 0;
    for (var i = 0; i < _ordemServicoVenda.ListaItens.list.length; i++) {
        var quantidade = Globalize.parseFloat(_ordemServicoVenda.ListaItens.list[i].Quantidade.val);
        var valorUnitarioAnterior = Globalize.parseFloat(_ordemServicoVenda.ListaItens.list[i].ValorUnitario.val);
        var valorUnitario = arredondar(valorUnitarioAnterior * ((100 - percentualDesconto) / 100));
        var valorTotalItem = quantidade * valorUnitario;
        somaNovoValorTotal = somaNovoValorTotal + valorTotalItem;
        somaNovoValorProdutos = somaNovoValorProdutos + valorTotalItem;

        _ordemServicoVenda.ListaItens.list[i].ValorDesconto.val = Globalize.format(valorUnitarioAnterior - valorUnitario, "n2");
        _ordemServicoVenda.ListaItens.list[i].ValorUnitario.val = Globalize.format(valorUnitario, "n2");
        _ordemServicoVenda.ListaItens.list[i].ValorTotalItem.val = Globalize.format(valorTotalItem, "n2");
    }

    var somaNovoValorServicos = 0;
    for (var i = 0; i < _ordemServicoVenda.ListaMaoObras.list.length; i++) {
        var quantidade = Globalize.parseFloat(_ordemServicoVenda.ListaMaoObras.list[i].Quantidade.val);
        var valorUnitarioAnterior = Globalize.parseFloat(_ordemServicoVenda.ListaMaoObras.list[i].ValorUnitario.val);
        var valorUnitario = arredondar(valorUnitarioAnterior * ((100 - percentualDesconto) / 100));
        var valorTotalItem = quantidade * valorUnitario;
        somaNovoValorTotal = somaNovoValorTotal + valorTotalItem;
        somaNovoValorServicos = somaNovoValorServicos + valorTotalItem;

        _ordemServicoVenda.ListaMaoObras.list[i].ValorDesconto.val = Globalize.format(valorUnitarioAnterior - valorUnitario, "n2");
        _ordemServicoVenda.ListaMaoObras.list[i].ValorUnitario.val = Globalize.format(valorUnitario, "n2");
        _ordemServicoVenda.ListaMaoObras.list[i].ValorTotalItem.val = Globalize.format(valorTotalItem, "n2");
    }

    var valorTotalReal = valorTotal - valorDesconto;
    var valorDiferenca = valorTotalReal - somaNovoValorTotal;
    if (valorDiferenca != 0) {
        var valorDiferencaValidar = valorDiferenca;
        if (valorDiferenca < 0)
            valorDiferencaValidar = valorDiferenca * -1;

        if (_ordemServicoVenda.ListaItens.list.length > 0) {
            for (var i = 0; i < _ordemServicoVenda.ListaItens.list.length; i++) {
                var quantidade = Globalize.parseFloat(_ordemServicoVenda.ListaItens.list[i].Quantidade.val);
                var valorUnitarioAnterior = Globalize.parseFloat(_ordemServicoVenda.ListaItens.list[i].ValorUnitario.val);
                var valorTotalItemAnterior = Globalize.parseFloat(_ordemServicoVenda.ListaItens.list[i].ValorTotalItem.val);
                if (valorUnitarioAnterior > valorDiferencaValidar) {
                    var valorUnitario = arredondar(valorUnitarioAnterior + (valorDiferenca / quantidade));
                    var valorTotalItem = quantidade * valorUnitario;
                    somaNovoValorTotal = somaNovoValorTotal - valorTotalItemAnterior + valorTotalItem;
                    somaNovoValorProdutos = somaNovoValorProdutos - valorTotalItemAnterior + valorTotalItem;

                    _ordemServicoVenda.ListaItens.list[i].ValorUnitario.val = Globalize.format(valorUnitario, "n2");
                    _ordemServicoVenda.ListaItens.list[i].ValorTotalItem.val = Globalize.format(valorTotalItem, "n2");
                    break;
                }
            }
        } else {
            for (var i = 0; i < _ordemServicoVenda.ListaMaoObras.list.length; i++) {
                var quantidade = Globalize.parseFloat(_ordemServicoVenda.ListaMaoObras.list[i].Quantidade.val);
                var valorUnitarioAnterior = Globalize.parseFloat(_ordemServicoVenda.ListaMaoObras.list[i].ValorUnitario.val);
                var valorTotalItemAnterior = Globalize.parseFloat(_ordemServicoVenda.ListaMaoObras.list[i].ValorTotalItem.val);
                if (valorUnitarioAnterior > valorDiferencaValidar) {
                    var valorUnitario = arredondar(valorUnitarioAnterior + (valorDiferenca / quantidade));
                    var valorTotalItem = quantidade * valorUnitario;
                    somaNovoValorTotal = somaNovoValorTotal - valorTotalItemAnterior + valorTotalItem;
                    somaNovoValorServicos = somaNovoValorServicos - valorTotalItemAnterior + valorTotalItem;

                    _ordemServicoVenda.ListaMaoObras.list[i].ValorUnitario.val = Globalize.format(valorUnitario, "n2");
                    _ordemServicoVenda.ListaMaoObras.list[i].ValorTotalItem.val = Globalize.format(valorTotalItem, "n2");
                    break;
                }
            }
        }
    }

    _ordemServicoVenda.ValorDesconto.val(Globalize.format(valorDesconto, "n2"));
    _ordemServicoVenda.PercentualDesconto.val(Globalize.format(percentualDesconto, "n2"));

    _ordemServicoVenda.ValorTotal.val(Globalize.format(somaNovoValorTotal, "n2"));
    _ordemServicoVenda.ValorProdutos.val(Globalize.format(somaNovoValorProdutos, "n2"));
    _ordemServicoVenda.ValorServicos.val(Globalize.format(somaNovoValorServicos, "n2"));

    _ordemServicoVendaPecas.ValorTotal.val(Globalize.format(somaNovoValorTotal, "n2"));
    _ordemServicoVendaPecas.ValorProdutos.val(Globalize.format(somaNovoValorProdutos, "n2"));

    _ordemServicoVendaMaoObras.ValorTotal.val(Globalize.format(somaNovoValorTotal, "n2"));
    _ordemServicoVendaMaoObras.ValorServicos.val(Globalize.format(somaNovoValorServicos, "n2"));
}

function RemoverDescontoItens() {
    var valorTotal = Globalize.parseFloat(_ordemServicoVenda.ValorTotal.val());
    var valorProdutos = Globalize.parseFloat(_ordemServicoVenda.ValorProdutos.val());
    var valorServicos = Globalize.parseFloat(_ordemServicoVenda.ValorServicos.val());

    for (var i = 0; i < _ordemServicoVenda.ListaItens.list.length; i++) {
        var valorDesconto = Globalize.parseFloat(_ordemServicoVenda.ListaItens.list[i].ValorDesconto.val);

        if (valorDesconto > 0) {
            var quantidade = Globalize.parseFloat(_ordemServicoVenda.ListaItens.list[i].Quantidade.val);
            var valorUnitario = Globalize.parseFloat(_ordemServicoVenda.ListaItens.list[i].ValorUnitario.val);

            valorUnitario = valorUnitario + valorDesconto;
            valorTotal = valorTotal + (valorDesconto * quantidade);
            valorProdutos = valorProdutos + (valorDesconto * quantidade);

            _ordemServicoVenda.ListaItens.list[i].ValorDesconto.val = Globalize.format(0, "n2");
            _ordemServicoVenda.ListaItens.list[i].ValorUnitario.val = Globalize.format(valorUnitario, "n2");
            _ordemServicoVenda.ListaItens.list[i].ValorTotalItem.val = Globalize.format(quantidade * valorUnitario, "n2");
        }
    }

    for (var i = 0; i < _ordemServicoVenda.ListaMaoObras.list.length; i++) {
        var valorDesconto = Globalize.parseFloat(_ordemServicoVenda.ListaMaoObras.list[i].ValorDesconto.val);

        if (valorDesconto > 0) {
            var quantidade = Globalize.parseFloat(_ordemServicoVenda.ListaMaoObras.list[i].Quantidade.val);
            var valorUnitario = Globalize.parseFloat(_ordemServicoVenda.ListaMaoObras.list[i].ValorUnitario.val);

            valorUnitario = valorUnitario + valorDesconto;
            valorTotal = valorTotal + (valorDesconto * quantidade);
            valorServicos = valorServicos + (valorDesconto * quantidade);

            _ordemServicoVenda.ListaMaoObras.list[i].ValorDesconto.val = Globalize.format(0, "n2");
            _ordemServicoVenda.ListaMaoObras.list[i].ValorUnitario.val = Globalize.format(valorUnitario, "n2");
            _ordemServicoVenda.ListaMaoObras.list[i].ValorTotalItem.val = Globalize.format(quantidade * valorUnitario, "n2");
        }
    }

    _ordemServicoVenda.ValorTotal.val(Globalize.format(valorTotal, "n2"));
    _ordemServicoVenda.ValorProdutos.val(Globalize.format(valorProdutos, "n2"));
    _ordemServicoVenda.ValorServicos.val(Globalize.format(valorServicos, "n2"));

    _ordemServicoVendaPecas.ValorTotal.val(Globalize.format(valorTotal, "n2"));
    _ordemServicoVendaPecas.ValorProdutos.val(Globalize.format(valorProdutos, "n2"));

    _ordemServicoVendaMaoObras.ValorTotal.val(Globalize.format(valorTotal, "n2"));
    _ordemServicoVendaMaoObras.ValorServicos.val(Globalize.format(valorServicos, "n2"));
}

function arredondar(valor) {
    return Math.round(valor * 100) / 100;//manterá 2 casas
}

function limparCamposOrdemServicoVenda() {
    LimparCampos(_ordemServicoVenda);
    _ordemServicoVenda.DataEmissao.val(Global.DataHoraAtual());
    _ordemServicoVenda.DataEntrega.val(Global.DataHoraAtual());

    if (_configuracoesEmpresa.PermiteAlterarEmpresaOrdemServicoVenda) {
        _ordemServicoVenda.Empresa.visible(false);
        _ordemServicoVenda.Referencia.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-12");
    }
}