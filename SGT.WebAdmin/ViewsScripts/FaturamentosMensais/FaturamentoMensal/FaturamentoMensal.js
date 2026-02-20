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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Remessa.js" />
/// <reference path="FaturamentoMensalEtapa.js" />
/// <reference path="FaturamentoMensalBoleto.js" />
/// <reference path="FaturamentoMensalDocumento.js" />
/// <reference path="FaturamentoMensalEnvioEmail.js" />
/// <reference path="../../Enumeradores/EnumStatusFaturamentoMensal.js" />
/// <reference path="../../Enumeradores/EnumEtapaFaturamentoMensal.js" />
/// <reference path="../../Enumeradores/EnumTipoObservacaoFaturamentoMensal.js" />
/// <reference path="../../Enumeradores/EnumTipoNota.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _selecaoFaturamento;
var _gridSelecaoFaturamento;
var _gridFaturamentoMensal;
var _pesquisaFaturamentoMensal;
var _detalheFaturamentoMensal;
var _gridServicoExtraDetalheFaturamentoMensal;

var _statusFaturamento = [
    { text: "Todos", value: -1 },
    { text: "Pendentes de Finalização", value: 0 },
    { text: "Documentos Autorizados", value: EnumStatusFaturamentoMensal.DocumentosAutorizados },
    { text: "Finalizado", value: EnumStatusFaturamentoMensal.Finalizado },
    { text: "Gerado Boletos", value: EnumStatusFaturamentoMensal.GeradoBoletos },
    { text: "Gerado Documentos", value: EnumStatusFaturamentoMensal.GeradoDocumentos },
    { text: "Iniciada", value: EnumStatusFaturamentoMensal.Iniciada },
    { text: "Cancelado", value: EnumStatusFaturamentoMensal.Cancelado }];

var _tipoObservacao = [
    { text: "Usar em BOLETO", value: EnumTipoObservacaoFaturamentoMensal.Boleto },
    { text: "Nenhum", value: EnumTipoObservacaoFaturamentoMensal.Nenhum },
    { text: "Usar em NF", value: EnumTipoObservacaoFaturamentoMensal.NotaFiscal },
    { text: "Usar em NF e BOLETO", value: EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto }];

var _tipoNotaFaturamentoMensagemClinete = [
    { text: "NF-e", value: EnumTipoNota.NFe },
    { text: "NFS-e", value: EnumTipoNota.NFSe }];

var PesquisaFaturamentoMensal = function () {
    this.Pessoa = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: "Pessoa:", idBtnSearch: guid(), val: ko.observable("") });
    this.GrupoFaturamento = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: "Grupo de Faturamento:", idBtnSearch: guid(), val: ko.observable("") });
    this.Servico = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: "Serviço Principal:", idBtnSearch: guid(), val: ko.observable("") });

    this.DataVencimento = PropertyEntity({ text: "Data Vencimento: ", getType: typesKnockout.date });
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusFaturamento, def: 0, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFaturamentoMensal.CarregarGrid();
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


var SelecaoFaturamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumEtapaFaturamentoMensal.Etapa1), def: EnumEtapaFaturamentoMensal.Etapa1, getType: typesKnockout.int });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusFaturamentoMensal.Iniciada), def: EnumStatusFaturamentoMensal.Iniciada, getType: typesKnockout.int });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Pessoa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), issue: 52 });
    this.GrupoFaturamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Grupo de Faturamento:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Serviço:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.ConfiguracaoBanco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Configuração de Boleto:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.FaturamentoParaGeracao = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaFaturamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaNaoSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TotalizadorValorSelecionado = PropertyEntity({ text: "Total selecionado: ", getType: typesKnockout.string, def: "0,00", val: ko.observable("0,00"), enable: ko.observable(true), visible: true });

    this.PesquisaFaturamento = PropertyEntity({ eventClick: PesquisaFaturamentosClick, type: types.event, text: "Pesquisar faturamentos", visible: ko.observable(true), enable: ko.observable(true) });

    this.ListaFaturamentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Proximo = PropertyEntity({ eventClick: ProximoSelecaoFaturamentoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
}

var DetalheFaturamentoMensal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", enable: ko.observable(false) });
    this.TipoNota = PropertyEntity({ val: ko.observable(true), options: _tipoNotaFaturamentoMensagemClinete, def: EnumTipoNota.NFe, text: "*Tipo Nota: ", enable: ko.observable(false) });
    this.DiaFatura = PropertyEntity({ text: "*Dia da Fatura: ", required: true, getType: typesKnockout.int, enable: ko.observable(false) });
    this.ValorTotal = PropertyEntity({ text: "*Valor da Fatura: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(false) });
    this.DataUltimaFatura = PropertyEntity({ text: "Data Última Fatura: ", required: false, getType: typesKnockout.date, enable: ko.observable(false) });
    this.DataProximaFatura = PropertyEntity({ text: "Data Próxima Fatura: ", required: false, getType: typesKnockout.date, enable: ko.observable(false) });

    this.GrupoFaturamento = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: true, text: "*Grupo do Faturamento:", idBtnSearch: guid(), val: ko.observable(""), enable: ko.observable(false) });
    this.Pessoa = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pessoa:", idBtnSearch: guid(), val: ko.observable(""), enable: ko.observable(false) });
    this.ServicoPrincipal = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: true, text: "*Serviço Principal:", idBtnSearch: guid(), val: ko.observable(""), enable: ko.observable(false) });
    this.ValorServicoPrincipal = PropertyEntity({ text: "*Valor do Serviço: ", required: true, getType: typesKnockout.decimal, enable: ko.observable(false) });
    this.ValorAdesao = PropertyEntity({ text: "Valor de Adesão: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(false) });
    this.NaturezaOperacao = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: true, text: "*Natureza da Operação:", idBtnSearch: guid(), val: ko.observable(""), enable: ko.observable(false) });
    this.TipoMovimento = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tipo de Movimento:", idBtnSearch: guid(), val: ko.observable(""), enable: ko.observable(false) });
    this.BoletoConfiguracao = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: "Configuração do Boleto:", idBtnSearch: guid(), val: ko.observable(""), enable: ko.observable(false) });

    this.TipoObservacao = PropertyEntity({ val: ko.observable(EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto), options: _tipoObservacao, text: "Tipo Observacao: ", def: EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto, required: false, enable: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, maxlength: 500, enable: ko.observable(false) });

    this.DataContrato = PropertyEntity({ text: "Data Contrato: ", required: false, getType: typesKnockout.date, enable: ko.observable(false) });
    this.DataLancamento = PropertyEntity({ text: "Período da execução do serviço: ", required: false, getType: typesKnockout.date, enable: ko.observable(false) });
    this.DataLancamentoAte = PropertyEntity({ text: "Até ", required: false, getType: typesKnockout.date, enable: ko.observable(false) });
    this.NumeroPedidoCompra = PropertyEntity({ text: "Nº Pedido Compra:", required: false, maxlength: 50, enable: ko.observable(false) });
    this.NumeroPedidoItemCompra = PropertyEntity({ text: "Nº Pedido Item Compra:", required: false, maxlength: 50, enable: ko.observable(false) });

    this.ServicosExtras = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), });

    this.NumeroPedidoCompraExtra = PropertyEntity({ text: "Nº Pedido Compra:", required: false, maxlength: 50, enable: ko.observable(false) });
    this.NumeroPedidoItemCompraExtra = PropertyEntity({ text: "Nº Pedido Item Compra:", required: false, maxlength: 50, enable: ko.observable(false) });
    this.Historico = PropertyEntity({ text: "Histórico:", required: false, maxlength: 500, enable: ko.observable(false) });

    this.Fechar = PropertyEntity({ type: types.event, eventClick: FecharDetalheFaturamentoMensalClick, text: "Fechar", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadFaturamentoMensal() {
    _selecaoFaturamento = new SelecaoFaturamento();
    KoBindings(_selecaoFaturamento, "knockoutSelecaoFaturamento");

    HeaderAuditoria("FaturamentoMensal", _selecaoFaturamento);

    _pesquisaFaturamentoMensal = new PesquisaFaturamentoMensal();
    KoBindings(_pesquisaFaturamentoMensal, "knockoutPesquisaFaturamentoMensal", false, _pesquisaFaturamentoMensal.Pesquisar.id);

    _detalheFaturamentoMensal = new DetalheFaturamentoMensal();
    KoBindings(_detalheFaturamentoMensal, "knoutDetalheFaturamentoMensal");

    new BuscarClientes(_pesquisaFaturamentoMensal.Pessoa);
    new BuscarGrupoFaturamento(_pesquisaFaturamentoMensal.GrupoFaturamento);
    new BuscarServicoTMS(_pesquisaFaturamentoMensal.Servico);

    new BuscarClientes(_selecaoFaturamento.Pessoa);
    new BuscarGrupoFaturamento(_selecaoFaturamento.GrupoFaturamento);
    new BuscarServicoTMS(_selecaoFaturamento.Servico);
    new BuscarBoletoConfiguracao(_selecaoFaturamento.ConfiguracaoBanco, RetornoConfiguracaoBanco);

    loadEtapaFaturamentoMensal();
    loadGeracaoDocumentos();
    loadGeracaoBoletos();
    loadEnvioEmail();

    var editar = { descricao: "Detalhe", id: guid(), metodo: DetalheFaturamentoMensalClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [editar] };

    var somenteLeitura = false;
    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () {
            AtualizarValorSelecionado();
        },
        callbackNaoSelecionado: function () {
            AtualizarValorSelecionado();
        },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoFaturamento.SelecionarTodos,
        somenteLeitura: somenteLeitura
    }
    var editarColuna = { permite: true, callback: null, atualizarRow: true };

    _gridSelecaoFaturamento = new GridView(_selecaoFaturamento.FaturamentoParaGeracao.idGrid, "FaturamentoMensal/PesquisaFaturamentoMensal", _selecaoFaturamento, menuOpcoes, null, 50, null, null, null, multiplaescolha, null, editarColuna);

    buscarFaturamentoMensal();
    buscarFaturamentoMensalParaGeracao();

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "50%" },
        { data: "Quantidade", title: "Qtd.", width: "15%" },
        { data: "ValorUnitario", title: "Val. Unitário", width: "15%" },
        { data: "ValorTotalServicoExtra", title: "Val. Total", width: "15%" }
    ];

    _gridServicoExtraDetalheFaturamentoMensal = new BasicDataTable(_detalheFaturamentoMensal.ServicosExtras.idGrid, header);
}

function PesquisaFaturamentosClick(e, sender) {
    if (_selecaoFaturamento.Pessoa.codEntity() == "0" && _selecaoFaturamento.GrupoFaturamento.codEntity() == "0" && _selecaoFaturamento.Servico.codEntity() == "0" && _selecaoFaturamento.ConfiguracaoBanco.codEntity() == "0")
        exibirMensagem(tipoMensagem.aviso, "Selecione os Faturamentos", "Por favor selecione ao menos um filtro para realizar a pesquisa.");
    else
        buscarFaturamentoMensalParaGeracao();
}

function ProximoSelecaoFaturamentoClick(e, sender) {
    if (_gridSelecaoFaturamento == undefined) {
        exibirMensagem(tipoMensagem.aviso, "Selecione os Faturamentos", "Por favor selecione ao menos um faturamento para avançar à próxima etapa.");
        return;
    }
    _selecaoFaturamento.ListaNaoSelecionados.val("");

    var faturamentosNaoSelecionados = _gridSelecaoFaturamento.ObterMultiplosNaoSelecionados();
    if (faturamentosNaoSelecionados.length > 0) {
        var dataGridNaoSelecionados = new Array();

        $.each(faturamentosNaoSelecionados, function (i, faturamento) {

            var obj = new Object();
            obj.Codigo = faturamento.Codigo;
            obj.CodigoFaturamentoCliente = faturamento.CodigoFaturamentoCliente;
            obj.CodigoFaturamentoClienteServico = faturamento.CodigoFaturamentoClienteServico;
            obj.CodigoConfiguracaoBanco = faturamento.CodigoConfiguracaoBanco;
            obj.CodigoTitulo = faturamento.CodigoTitulo;
            obj.CodigoNotaFiscal = faturamento.CodigoNotaFiscal;
            obj.CodigoNotaFiscalServico = faturamento.CodigoNotaFiscalServico;
            obj.Status = faturamento.Status;
            obj.Pessoa = faturamento.Pessoa;
            obj.GrupoFaturamento = faturamento.GrupoFaturamento;
            obj.Banco = faturamento.Banco;
            obj.DiaFaturamento = faturamento.DiaFaturamento;
            obj.DataVencimento = faturamento.DataVencimento;
            obj.NumeroTitulo = faturamento.NumeroTitulo;
            obj.NumeroBoleto = faturamento.NumeroBoleto;
            obj.NumeroNota = faturamento.NumeroNota;
            obj.NumeroNotaServico = faturamento.NumeroNotaServico;
            obj.StatusNotaFiscal = faturamento.StatusNotaFiscal;
            obj.RetornoNotaFiscal = faturamento.RetornoNotaFiscal;
            obj.Valor = faturamento.Valor;
            obj.Observacao = faturamento.Observacao;

            dataGridNaoSelecionados.push(obj);
        });

        _selecaoFaturamento.ListaNaoSelecionados.val(JSON.stringify(dataGridNaoSelecionados));
    }

    var faturamentosSelecionados = _gridSelecaoFaturamento.ObterMultiplosSelecionados();
    if (faturamentosSelecionados.length > 0) {
        var dataGrid = new Array();

        $.each(faturamentosSelecionados, function (i, faturamento) {

            var obj = new Object();
            obj.Codigo = faturamento.Codigo;
            obj.CodigoFaturamentoCliente = faturamento.CodigoFaturamentoCliente;
            obj.CodigoFaturamentoClienteServico = faturamento.CodigoFaturamentoClienteServico;
            obj.CodigoConfiguracaoBanco = faturamento.CodigoConfiguracaoBanco;
            obj.CodigoTitulo = faturamento.CodigoTitulo;
            obj.CodigoNotaFiscal = faturamento.CodigoNotaFiscal;
            obj.CodigoNotaFiscalServico = faturamento.CodigoNotaFiscalServico;
            obj.Status = faturamento.Status;
            obj.Pessoa = faturamento.Pessoa;
            obj.GrupoFaturamento = faturamento.GrupoFaturamento;
            obj.Banco = faturamento.Banco;
            obj.DiaFaturamento = faturamento.DiaFaturamento;
            obj.DataVencimento = faturamento.DataVencimento;
            obj.NumeroTitulo = faturamento.NumeroTitulo;
            obj.NumeroBoleto = faturamento.NumeroBoleto;
            obj.NumeroNota = faturamento.NumeroNota;
            obj.NumeroNotaServico = faturamento.NumeroNotaServico;
            obj.StatusNotaFiscal = faturamento.StatusNotaFiscal;
            obj.RetornoNotaFiscal = faturamento.RetornoNotaFiscal;
            obj.Valor = faturamento.Valor;
            obj.Observacao = faturamento.Observacao;

            dataGrid.push(obj);
        });

        _selecaoFaturamento.ListaFaturamento.val(JSON.stringify(dataGrid));
        var data = {
            ListaFaturamento: _selecaoFaturamento.ListaFaturamento.val(),
            ListaNaoSelecionados: _selecaoFaturamento.ListaNaoSelecionados.val(),
            Codigo: _selecaoFaturamento.Codigo.val(),
            Pessoa: _selecaoFaturamento.Pessoa.codEntity(),
            GrupoFaturamento: _selecaoFaturamento.GrupoFaturamento.codEntity(),
            Servico: _selecaoFaturamento.Servico.codEntity(),
            ConfiguracaoBanco: _selecaoFaturamento.ConfiguracaoBanco.codEntity(),
            SelecionarTodos: _selecaoFaturamento.SelecionarTodos.val()
        };
        executarReST("FaturamentoMensal/IniciarFaturamentoMensal", data, function (arg) {
            if (arg.Success) {
                _selecaoFaturamento.Status.val(EnumStatusFaturamentoMensal.Iniciada);
                _selecaoFaturamento.Codigo.val(arg.Data.Codigo);
                _geracaoDocumentos.Codigo.val(arg.Data.Codigo);
                _envioEmail.Codigo.val(arg.Data.Codigo);
                _geracaoBoletos.Codigo.val(arg.Data.Codigo);

                $("#knockoutGeracaoDocumentos").show();
                _etapaAtual = 2;
                $("#" + _etapaFaturamentoMensal.Etapa2.idTab).click();
                $("#" + _etapaFaturamentoMensal.Etapa2.idTab).tab("show");
                buscarFaturamentoMensalDocumento();

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa inicial concluída, siga a etapa 2.");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    }
    //else {
    //    exibirMensagem(tipoMensagem.aviso, "Selecione os Faturamentos", "Por favor selecione ao menos um faturamento para avançar à próxima etapa.");
    //    $("#knockoutGeracaoDocumentos").hide();
    //    $("#knockoutGeracaoBoleto").hide();
    //    $("#knockoutEnvioEmail").hide();
    //}
}

function DetalheFaturamentoMensalClick(e, sender) {
    LimparCampos(_detalheFaturamentoMensal);

    if (e.Codigo > 0 && e.Codigo != "") {
        var data =
        {
            Codigo: e.Codigo
        }
        executarReST("FaturamentoMensal/CarregarDadosFaturamentoMensalCliente", data, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataParcela = { Data: e.Data };
                    PreencherObjetoKnout(_detalheFaturamentoMensal, dataParcela);
                    recarregarGridServicoExtraDetalheFaturamentoMensal();
                    var modalDetalhes = new bootstrap.Modal(document.getElementById("divDetalheFaturamentoMensal"), { backdrop: 'static', keyboard: true });
                    modalDetalhes.show();
                }
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", e.Msg);
            }
        });
    }
}

function recarregarGridServicoExtraDetalheFaturamentoMensal() {
    var data = new Array();
    $.each(_detalheFaturamentoMensal.ServicosExtras.list, function (i, Servico) {
        var obj = new Object();

        obj.Codigo = Servico.Codigo.val;
        obj.CodigoServico = Servico.CodigoServico.val;
        obj.Descricao = Servico.Descricao.val;
        obj.Quantidade = Servico.Quantidade.val;
        obj.ValorUnitario = Servico.ValorUnitario.val;
        obj.ValorTotalServicoExtra = Servico.ValorTotalServicoExtra.val;

        data.push(obj);
    });
    _gridServicoExtraDetalheFaturamentoMensal.CarregarGrid(data);
}

function FecharDetalheFaturamentoMensalClick() {
    LimparCampos(_detalheFaturamentoMensal);
    Global.fecharModal('divDetalheFaturamentoMensal');
}

//*******MÉTODOS*******

function buscarFaturamentoMensal() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFaturamentoMensal, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFaturamentoMensal = new GridView(_pesquisaFaturamentoMensal.Pesquisar.idGrid, "FaturamentoMensal/Pesquisa", _pesquisaFaturamentoMensal, menuOpcoes, null);
    _gridFaturamentoMensal.CarregarGrid();
}

function editarFaturamentoMensal(faturamentoMensalGrupoGrid) {
    limparCamposFaturamentoMensal()
    _selecaoFaturamento.Codigo.val(faturamentoMensalGrupoGrid.Codigo);
    BuscarPorCodigo(_selecaoFaturamento, "FaturamentoMensal/BuscarPorCodigo", function (arg) {
        _pesquisaFaturamentoMensal.ExibirFiltros.visibleFade(false);

        _selecaoFaturamento.Codigo.val(arg.Data.Codigo);
        _selecaoFaturamento.Status.val(arg.Data.StatusFaturamentoMensal);
        _geracaoDocumentos.Codigo.val(arg.Data.Codigo);
        _envioEmail.Codigo.val(arg.Data.Codigo);
        _geracaoBoletos.Codigo.val(arg.Data.Codigo);

        $("#knockoutSelecaoFaturamento").show();
        $("#knockoutGeracaoDocumentos").hide();
        $("#knockoutGeracaoBoleto").hide();
        $("#knockoutEnvioEmail").hide();


        if (arg.Data.StatusFaturamentoMensal == EnumStatusFaturamentoMensal.DocumentosAutorizados) {
            _etapaAtual = 3;
            $("#knockoutGeracaoDocumentos").show();
            $("#knockoutGeracaoBoleto").show();
            $("#knockoutEnvioEmail").hide();
            $("#" + _etapaFaturamentoMensal.Etapa3.idTab).click();
            $("#" + _etapaFaturamentoMensal.Etapa3.idTab).tab("show");
        }
        else if (arg.Data.StatusFaturamentoMensal == EnumStatusFaturamentoMensal.Finalizado || arg.Data.StatusFaturamentoMensal == EnumStatusFaturamentoMensal.AguardandoEnvioEmail
            || arg.Data.StatusFaturamentoMensal == EnumStatusFaturamentoMensal.EmGeracaoEnvioEmail) {
            _etapaAtual = 4;
            $("#knockoutGeracaoDocumentos").show();
            $("#knockoutGeracaoBoleto").show();
            $("#knockoutEnvioEmail").show();
            $("#" + _etapaFaturamentoMensal.Etapa4.idTab).click();
            $("#" + _etapaFaturamentoMensal.Etapa4.idTab).tab("show");
        }
        else if (arg.Data.StatusFaturamentoMensal == EnumStatusFaturamentoMensal.GeradoBoletos) {
            _etapaAtual = 4;
            $("#knockoutGeracaoDocumentos").show();
            $("#knockoutGeracaoBoleto").show();
            $("#knockoutEnvioEmail").show();

            $("#" + _etapaFaturamentoMensal.Etapa4.idTab).click();
            $("#" + _etapaFaturamentoMensal.Etapa4.idTab).tab("show");
        }
        else if (arg.Data.StatusFaturamentoMensal == EnumStatusFaturamentoMensal.GeradoDocumentos || arg.Data.StatusFaturamentoMensal == EnumStatusFaturamentoMensal.AguardandoAutorizacaoDocumento
            || arg.Data.StatusFaturamentoMensal == EnumStatusFaturamentoMensal.EmGeracaoAutorizacaoDocumento) {
            _etapaAtual = 2;
            $("#knockoutGeracaoDocumentos").show();
            $("#knockoutGeracaoBoleto").hide();
            $("#knockoutEnvioEmail").hide();

            $("#" + _etapaFaturamentoMensal.Etapa2.idTab).click();
            $("#" + _etapaFaturamentoMensal.Etapa2.idTab).tab("show");
        }
        else if (arg.Data.StatusFaturamentoMensal == EnumStatusFaturamentoMensal.Iniciada) {
            _etapaAtual = 2;
            $("#knockoutGeracaoDocumentos").show();
            $("#knockoutGeracaoBoleto").hide();
            $("#knockoutEnvioEmail").hide();
            $("#" + _etapaFaturamentoMensal.Etapa2.idTab).click();
            $("#" + _etapaFaturamentoMensal.Etapa2.idTab).tab("show");
        } else {
            _etapaAtual = 2;
            $("#knockoutGeracaoDocumentos").show();
            $("#knockoutGeracaoBoleto").hide();
            $("#knockoutEnvioEmail").hide();

            $("#" + _etapaFaturamentoMensal.Etapa2.idTab).click();
            $("#" + _etapaFaturamentoMensal.Etapa2.idTab).tab("show");
        }

        PosicionarEtapa();
        buscarFaturamentoMensalParaGeracao();
        buscarFaturamentoMensalDocumento();
        buscarFaturamentoMensalBoleto();
        buscarTitulosParaEnvioEmail();

    }, null);
}

function RetornoConfiguracaoBanco(data) {
    _selecaoFaturamento.ConfiguracaoBanco.codEntity(data.Codigo);
    _selecaoFaturamento.ConfiguracaoBanco.val(data.DescricaoBanco);
}

function buscarFaturamentoMensalParaGeracao() {
    var somenteLeitura = false;

    _selecaoFaturamento.SelecionarTodos.visible(false);
    if (_selecaoFaturamento.Codigo.val() > 0) {
        _selecaoFaturamento.SelecionarTodos.val(true);
        //var multiplaescolha = {
        //    basicGrid: null,
        //    eventos: function () { },
        //    selecionados: new Array(),
        //    naoSelecionados: new Array(),
        //    SelecionarTodosKnout: _selecaoFaturamento.SelecionarTodos,
        //    somenteLeitura: somenteLeitura
        //}
        //var editarColuna = { permite: true, callback: null, atualizarRow: true };

        //_gridSelecaoFaturamento = new GridView(_selecaoFaturamento.FaturamentoParaGeracao.idGrid, "FaturamentoMensal/PesquisaFaturamentoMensal", _selecaoFaturamento, null, null, 50, null, null, null, multiplaescolha, null, editarColuna);
        _gridSelecaoFaturamento.CarregarGrid();
        _selecaoFaturamento.TotalizadorValorSelecionado.val(_geracaoDocumentos.TotalizadorValorDocumento.val());
    }
    else {
        var data = { Pessoa: _selecaoFaturamento.Pessoa.codEntity(), GrupoFaturamento: _selecaoFaturamento.GrupoFaturamento.codEntity(), Servico: _selecaoFaturamento.Servico.codEntity(), ConfiguracaoBanco: _selecaoFaturamento.ConfiguracaoBanco.codEntity() };
        executarReST("FaturamentoMensal/BuscarTodasEmpresas", data, function (arg) {
            if (arg.Success) {

                var naoselecionados = new Array();

                $.each(arg.Data, function (i, obj) {
                    var objCTe = {
                        DT_RowId: obj.Codigo, Codigo: obj.Codigo,
                        CodigoFaturamentoCliente: obj.CodigoFaturamentoCliente,
                        CodigoFaturamentoClienteServico: obj.CodigoFaturamentoClienteServico,
                        CodigoConfiguracaoBanco: obj.CodigoConfiguracaoBanco,
                        CodigoTitulo: obj.CodigoTitulo,
                        CodigoNotaFiscal: obj.CodigoNotaFiscal,
                        CodigoNotaFiscalServico: obj.CodigoNotaFiscalServico,
                        Status: obj.Status,
                        Pessoa: obj.Pessoa,
                        GrupoFaturamento: obj.GrupoFaturamento,
                        Banco: obj.Banco,
                        DiaFaturamento: obj.DiaFaturamento,
                        DataVencimento: obj.DataVencimento,
                        NumeroTitulo: obj.NumeroTitulo,
                        NumeroBoleto: obj.NumeroBoleto,
                        NumeroNota: obj.NumeroNota,
                        NumeroNotaServico: obj.NumeroNotaServico,
                        StatusNotaFiscal: obj.StatusNotaFiscal,
                        RetornoNotaFiscal: obj.RetornoNotaFiscal,
                        Valor: obj.Valor,
                        Observacao: obj.Observacao
                    };

                    naoselecionados.push(objCTe);
                });

                _selecaoFaturamento.SelecionarTodos.val(false);
                _selecaoFaturamento.SelecionarTodos.visible(true);

                //var multiplaescolha = {
                //    basicGrid: null,
                //    eventos: function () { },
                //    selecionados: naoselecionados,
                //    naoSelecionados: new Array(),
                //    SelecionarTodosKnout: _selecaoFaturamento.SelecionarTodos,
                //    somenteLeitura: somenteLeitura
                //}
                //var editarColuna = { permite: true, callback: null, atualizarRow: true };
                //_gridSelecaoFaturamento = undefined;
                //_gridSelecaoFaturamento = new GridView(_selecaoFaturamento.FaturamentoParaGeracao.idGrid, "FaturamentoMensal/PesquisaFaturamentoMensal", _selecaoFaturamento, null, null, 50, null, null, null, multiplaescolha, null, editarColuna);
                _gridSelecaoFaturamento.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    }
}

function limparCamposFaturamentoMensal() {
    limparCamposSelecaoFaturamento();
    limparCamposFaturamentoMensalDocumento();
    limparCamposFaturamentoMensalBoleto();
    limparCamposEnvioEmail();

    $("#knockoutSelecaoFaturamento").show();
    $("#knockoutGeracaoDocumentos").hide();
    $("#knockoutGeracaoBoleto").hide();
    $("#knockoutEnvioEmail").hide();

    buscarFaturamentoMensalParaGeracao();
    buscarFaturamentoMensalDocumento();
    buscarFaturamentoMensalBoleto();
    buscarTitulosParaEnvioEmail();

    _etapaAtual = 1;
    PosicionarEtapa();
    $("#" + _etapaFaturamentoMensal.Etapa1.idTab).click();
    $("#" + _etapaFaturamentoMensal.Etapa1.idTab).tab("show");
}

function limparCamposSelecaoFaturamento() {
    LimparCampos(_selecaoFaturamento);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function AtualizarValorSelecionado() {

    var faturamentosSelecionados = null;

    if (_selecaoFaturamento.SelecionarTodos.val()) {
        faturamentosSelecionados = _gridSelecaoFaturamento.ObterMultiplosNaoSelecionados();
    } else {
        faturamentosSelecionados = _gridSelecaoFaturamento.ObterMultiplosSelecionados();
    }

    var codigosFaturamento = new Array();

    for (var i = 0; i < faturamentosSelecionados.length; i++)
        codigosFaturamento.push(faturamentosSelecionados[i].DT_RowId);

    if (codigosFaturamento && (codigosFaturamento.length > 0 || _selecaoFaturamento.SelecionarTodos.val())) {
        _selecaoFaturamento.ListaFaturamentos.val(JSON.stringify(codigosFaturamento));
        executarReST("FaturamentoMensal/ObterDetalhesFaturamentosSelecionados", RetornarObjetoPesquisa(_selecaoFaturamento), function (r) {
            if (r.Success) {
                if (r.Data) {
                    var valorTotalSelecionado = 0;

                    for (var i = 0; i < r.Data.length; i++) {
                        var detalhesFaturamentos = r.Data[i];
                        //valorTotalSelecionado += Globalize.parseFloat(detalhesFaturamentos.ValorTotal);
                        valorTotalSelecionado += detalhesFaturamentos.ValorTotal;
                    }

                    _selecaoFaturamento.TotalizadorValorSelecionado.val(Globalize.format(valorTotalSelecionado, "n2"));
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    } else
        _selecaoFaturamento.TotalizadorValorSelecionado.val("0,00");

    //var valorTotalSelecionado = 0.0;

    //if (faturamentosSelecionados.length > 0) {
    //    $.each(faturamentosSelecionados, function (i, faturamento) {
    //        valorTotalSelecionado = valorTotalSelecionado + parseFloat(faturamento.Valor.toString().replace(".", "").replace(",", "."));
    //        _selecaoFaturamento.TotalizadorValorSelecionado.val(Globalize.format(valorTotalSelecionado, "n2"));
    //    });

    //} else
    //    _selecaoFaturamento.TotalizadorValorSelecionado.val("0,00");
}