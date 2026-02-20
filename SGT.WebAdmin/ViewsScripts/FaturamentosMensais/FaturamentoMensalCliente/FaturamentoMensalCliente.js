/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/NaturezaOperacao.js" />
/// <reference path="../../Consultas/Servico.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="../../Consultas/GrupoFaturamento.js" />
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
/// <reference path="../../Enumeradores/EnumTipoObservacaoFaturamentoMensal.js" />
/// <reference path="../../Enumeradores/EnumTipoNota.js" />
/// <reference path="ServicoExtra.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridFaturamentoMensalCliente;
var _faturamentoMensalCliente;
var _pesquisaFaturamentoMensalCliente;

var _tipoObservacao = [
    { text: "Usar em BOLETO", value: EnumTipoObservacaoFaturamentoMensal.Boleto },
    { text: "Nenhum", value: EnumTipoObservacaoFaturamentoMensal.Nenhum },
    { text: "Usar em NF", value: EnumTipoObservacaoFaturamentoMensal.NotaFiscal },
    { text: "Usar em NF e BOLETO", value: EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto }];

var _tipoNotaFaturamentoMensagemClinete = [
    { text: "NF-e", value: EnumTipoNota.NFe },
    { text: "NFS-e", value: EnumTipoNota.NFSe }];

var PesquisaFaturamentoMensalCliente = function () {
    this.Pessoa = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: "Pessoa:", idBtnSearch: guid(), val: ko.observable("") });
    this.GrupoFaturamento = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: "Grupo de Faturamento:", idBtnSearch: guid(), val: ko.observable("") });
    this.Servico = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: "Serviço Principal:", idBtnSearch: guid(), val: ko.observable("") });
    this.DiaFatura = PropertyEntity({ text: "Dia da Fatura: ", getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFaturamentoMensalCliente.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var FaturamentoMensalCliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.TipoNota = PropertyEntity({ val: ko.observable(true), options: _tipoNotaFaturamentoMensagemClinete, def: EnumTipoNota.NFe, text: "*Tipo Nota: " });
    this.DiaFatura = PropertyEntity({ text: "*Dia da Fatura: ", required: true, getType: typesKnockout.int });
    this.ValorTotal = PropertyEntity({ text: "*Valor da Fatura: ", required: false, getType: typesKnockout.decimal, enable: false });
    this.DataUltimaFatura = PropertyEntity({ text: "Data Última Fatura: ", required: false, getType: typesKnockout.date, enable: false });
    this.DataProximaFatura = PropertyEntity({ text: "Data Próxima Fatura: ", required: false, getType: typesKnockout.date, enable: false });

    this.GrupoFaturamento = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: true, text: "*Grupo do Faturamento:", idBtnSearch: guid(), val: ko.observable("") });
    this.Pessoa = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pessoa:", idBtnSearch: guid(), val: ko.observable("") });
    this.ServicoPrincipal = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: true, text: "*Serviço Principal:", idBtnSearch: guid(), val: ko.observable("") });
    this.ValorServicoPrincipal = PropertyEntity({ text: "*Valor do Serviço: ", required: true, getType: typesKnockout.decimal });
    this.ValorAdesao = PropertyEntity({ text: "Valor de Adesão: ", required: false, getType: typesKnockout.decimal });
    this.NaturezaOperacao = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: true, text: "*Natureza da Operação:", idBtnSearch: guid(), val: ko.observable("") });
    this.TipoMovimento = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tipo de Movimento:", idBtnSearch: guid(), val: ko.observable("") });
    this.BoletoConfiguracao = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: "Configuração do Boleto:", idBtnSearch: guid(), val: ko.observable("") });

    this.TipoObservacao = PropertyEntity({ val: ko.observable(EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto), options: _tipoObservacao, text: "Tipo Observacao: ", def: EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto, required: false, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, maxlength: 500 });

    this.DataContrato = PropertyEntity({ text: "Data Contrato: ", required: false, getType: typesKnockout.date, enable: false });
    this.DataLancamento = PropertyEntity({ text: "Período da execução do serviço: ", required: false, getType: typesKnockout.date, enable: false });
    this.DataLancamentoAte = PropertyEntity({ text: "Até ", required: false, getType: typesKnockout.date, enable: false });
    this.NumeroPedidoCompra = PropertyEntity({ text: "Nº Pedido Compra:", required: false, maxlength: 50 });
    this.NumeroPedidoItemCompra = PropertyEntity({ text: "Nº Pedido Item Compra:", required: false, maxlength: 50 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.ServicosExtras = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), });

    this.CodigoServicoExtra = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ServicoExtra = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: "*Serviço Extra:", idBtnSearch: guid(), val: ko.observable("") });
    this.Quantidade = PropertyEntity({ text: "*Quantidade: ", required: false, getType: typesKnockout.decimal });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário: ", required: false, getType: typesKnockout.decimal });
    this.ValorTotalServicoExtra = PropertyEntity({ text: "*Valor Total: ", required: false, getType: typesKnockout.decimal, enable: false });
    this.DataLancamentoServico = PropertyEntity({ text: "Período da execução do serviço: ", required: false, getType: typesKnockout.date });
    this.DataLancamentoServicoAte = PropertyEntity({ text: "Até: ", required: false, getType: typesKnockout.date });
    this.TipoObservacaoServicoExtra = PropertyEntity({ val: ko.observable(EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto), options: _tipoObservacao, text: "Tipo Observacao: ", def: EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto, required: false, enable: ko.observable(true) });
    this.ObservacaoServicoExtra = PropertyEntity({ text: "Observação:", required: false, maxlength: 500 });

    this.NumeroPedidoCompraExtra = PropertyEntity({ text: "Nº Pedido Compra:", required: false, maxlength: 50 });
    this.NumeroPedidoItemCompraExtra = PropertyEntity({ text: "Nº Pedido Item Compra:", required: false, maxlength: 50 });
    this.Historico = PropertyEntity({ text: "Histórico:", required: false, maxlength: 500 });

    this.AdicionarServico = PropertyEntity({ eventClick: AdicionarServicoExtraClick, type: types.event, text: ko.observable("Adicionar Serviço Extra"), visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadFaturamentoMensalCliente() {

    _faturamentoMensalCliente = new FaturamentoMensalCliente();
    KoBindings(_faturamentoMensalCliente, "knockoutCadastroFaturamentoMensalCliente");

    _pesquisaFaturamentoMensalCliente = new PesquisaFaturamentoMensalCliente();
    KoBindings(_pesquisaFaturamentoMensalCliente, "knockoutPesquisaFaturamentoMensalCliente", false, _pesquisaFaturamentoMensalCliente.Pesquisar.id);

    HeaderAuditoria("FaturamentoMensalCliente", _faturamentoMensalCliente);

    new BuscarClientes(_pesquisaFaturamentoMensalCliente.Pessoa);
    new BuscarGrupoFaturamento(_pesquisaFaturamentoMensalCliente.GrupoFaturamento);
    new BuscarServicoTMS(_pesquisaFaturamentoMensalCliente.Servico);

    new BuscarGrupoFaturamento(_faturamentoMensalCliente.GrupoFaturamento);
    new BuscarClientes(_faturamentoMensalCliente.Pessoa);
    new BuscarServicoTMS(_faturamentoMensalCliente.ServicoPrincipal, function (data) {
        _faturamentoMensalCliente.ServicoPrincipal.codEntity(data.Codigo);
        _faturamentoMensalCliente.ServicoPrincipal.val(data.Descricao);
        if (data.ValorVenda != null && data.ValorVenda != "") {
            var valorUnitario = parseFloat(data.ValorVenda.toString().replace(".", "").replace(",", ".")).toFixed(2);
            valorUnitario = parseFloat(valorUnitario);
            _faturamentoMensalCliente.ValorServicoPrincipal.val(Globalize.format(valorUnitario, "n2"));
        }
    });
    new BuscarNaturezasOperacoesNotaFiscal(_faturamentoMensalCliente.NaturezaOperacao);
    new BuscarTipoMovimento(_faturamentoMensalCliente.TipoMovimento, null, null, null, null, EnumFinalidadeTipoMovimento.FaturamentoMensal);
    new BuscarBoletoConfiguracao(_faturamentoMensalCliente.BoletoConfiguracao, RetornoBoletoConfiguracao);

    buscarFaturamentoMensalClientes();
    loadServicoExtra();
}

function RetornoBoletoConfiguracao(data) {
    _faturamentoMensalCliente.BoletoConfiguracao.codEntity(data.Codigo);
    _faturamentoMensalCliente.BoletoConfiguracao.val(data.DescricaoBanco);
}

function adicionarClick(e, sender) {
    resetarTabs();
    Salvar(e, "FaturamentoMensalCliente/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridFaturamentoMensalCliente.CarregarGrid();
                limparCamposFaturamentoMensalCliente();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    resetarTabs();
    Salvar(e, "FaturamentoMensalCliente/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridFaturamentoMensalCliente.CarregarGrid();
                limparCamposFaturamentoMensalCliente();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o faturamento do cliente " + _faturamentoMensalCliente.Pessoa.val() + "?", function () {
        ExcluirPorCodigo(_faturamentoMensalCliente, "FaturamentoMensalCliente/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridFaturamentoMensalCliente.CarregarGrid();
                limparCamposFaturamentoMensalCliente();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposFaturamentoMensalCliente();
}

//*******MÉTODOS*******


function buscarFaturamentoMensalClientes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFaturamentoMensalCliente, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFaturamentoMensalCliente = new GridView(_pesquisaFaturamentoMensalCliente.Pesquisar.idGrid, "FaturamentoMensalCliente/Pesquisa", _pesquisaFaturamentoMensalCliente, menuOpcoes, null);
    _gridFaturamentoMensalCliente.CarregarGrid();
}

function editarFaturamentoMensalCliente(marcaGrid) {
    limparCamposFaturamentoMensalCliente();
    _faturamentoMensalCliente.Codigo.val(marcaGrid.Codigo);
    BuscarPorCodigo(_faturamentoMensalCliente, "FaturamentoMensalCliente/BuscarPorCodigo", function (arg) {
        resetarTabs();
        _pesquisaFaturamentoMensalCliente.ExibirFiltros.visibleFade(false);
        _faturamentoMensalCliente.Atualizar.visible(true);
        _faturamentoMensalCliente.Cancelar.visible(true);
        _faturamentoMensalCliente.Excluir.visible(true);
        _faturamentoMensalCliente.Adicionar.visible(false);
        preencherServicoExtra();
    }, null);
}

function limparCamposFaturamentoMensalCliente() {
    _faturamentoMensalCliente.Atualizar.visible(false);
    _faturamentoMensalCliente.Cancelar.visible(false);
    _faturamentoMensalCliente.Excluir.visible(false);
    _faturamentoMensalCliente.Adicionar.visible(true);
    LimparCampos(_faturamentoMensalCliente);
    preencherServicoExtra();
    resetarTabs();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}
