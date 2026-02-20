/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../wwwroot/js/Global/Buscas.js" />
/// <reference path="../../../wwwroot/js/Global/Buscas.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />

var _detalhesPedidos;
var _historicoObservacaoPedido;
var _observacaoPedido;
var _gridHistoricoPedidoObservacao;
var _alteracaoDataPrevisaoEntrega;
var _modalAnexosPedidoEmbarcadorDetalhesPedido;
var _historicoPedidoObservacao;

var DetalhesPedido = function (detalhesPedido) {
    this.Codigo = PropertyEntity({ val: ko.observable() });
    this.CodigoCarga = PropertyEntity({ val: ko.observable() });
    this.CodigoCargaEntrega = PropertyEntity({ text: "Código Carga Entrega", val: ko.observable("") });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Pedido", val: ko.observable("") });
    this.ProtocoloIntegracao = PropertyEntity({ text: "Protocolo Pedido", val: ko.observable("") });
    this.DataEntrega = PropertyEntity({ text: "Data de Entrega", val: ko.observable("") });
    this.Filial = PropertyEntity({ text: "Filial", type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), entityDescription: ko.observable() });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação", type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), entityDescription: ko.observable() });
    this.Carga = PropertyEntity({ text: "Carga", type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), entityDescription: ko.observable() });
    this.CargaCritica = PropertyEntity({ text: "Carga Critica", val: ko.observable("") });
    this.ModeloVeicular = PropertyEntity({ text: "Modelo Veicular", type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), entityDescription: ko.observable() });
    this.Destinatario = PropertyEntity({ text: "Destinatário", type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), entityDescription: ko.observable() });
    this.EnderecoDestino = PropertyEntity({ text: "Destino", val: ko.observable("") });
    this.NotasFiscais = PropertyEntity({ text: "Notas Fiscais", val: ko.observable("") });
    this.CanalVenda = PropertyEntity({ text: "Canal de Venda", val: ko.observable("") });
    this.EquipeVendas = PropertyEntity({ text: "Equipe de Venda", val: ko.observable("") });
    this.TipoMercadoria = PropertyEntity({ text: "Tipo de Mercado", val: ko.observable("") });
    this.EscritorioVenda = PropertyEntity({ text: "Escritorio de Venda", val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Última Observação", val: ko.observable("") });
    this.DataPrevisaoEntrega = PropertyEntity({ text: "Previsão Entrega", val: ko.observable("") });
    this.DataEntrega = PropertyEntity({ text: "Data Entrega", val: ko.observable("") });
    this.DataFaturamento = PropertyEntity({ text: "Data de Faturamento", val: ko.observable("") });
    this.Peso = PropertyEntity({ text: "Peso Total", val: ko.observable("") });
    this.FarolStatus = PropertyEntity({ text: "Farol", val: ko.observable("") });
    this.CanalEntrega = PropertyEntity({ text: "Canal de Entrega", val: ko.observable("") });
    this.DataReprogramada = PropertyEntity({ text: "Data de Entrega Atualizada (ETA)", val: ko.observable("") });
    this.DataPrevisaoEntregaAjustada = PropertyEntity({ text: "Data de Entrega Ajustada", val: ko.observable("") });
    this.Item = PropertyEntity({ text: "Item", val: ko.observable("") });
    this.PedidoCritico = PropertyEntity({ text: "Pedido Crítico", val: ko.observable(false) });


    this.Produtos = PropertyEntity({ idGrid: guid(), val: ko.observableArray([]) });
    this.Ocorrencias = PropertyEntity({ idGrid: guid(), val: ko.observableArray([]) });
    this.OcorrenciasComerciais = PropertyEntity({ idGrid: guid(), val: ko.observableArray([]) });


    this.CodigoProximaEntrega = PropertyEntity({ val: ko.observable() });

    this.Anexos = PropertyEntity({ eventClick: visualizarAnexosPedidoClick, type: types.event, text: Localization.Resources.Cargas.Carga.Anexos, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe), enable: ko.observable(true) });
    this.AdicionarOcorrencia = PropertyEntity({ eventClick: abrirModalAdicionarOcorrenciaEntrega, type: types.event, text: "Adicionar Ocorrência", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe), enable: ko.observable(true) });
    this.VisualizarHistoricoObservacaoPedido = PropertyEntity({ text: "Observações", eventClick: abrirModalHistoricoObservacaoPedido, type: types.event, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe), enable: ko.observable(true) });
    this.AlterarDataPrevisaoDeEntrega = PropertyEntity({ text: "Alterar data previsão entrega", id: guid(), eventClick: abrirModalAlteracaoDataPrevisaoEntrega, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) });
    this.SetarPedidoCritico = PropertyEntity({ text: "Pedido crítico", id: guid(), eventClick: abrirModalPedidoCritico, visible: ko.observable(true) });

    PreencherObjetoKnout(this, { Data: detalhesPedido });
};

var DetalhesPedidos = function () {
    this.DetalhesPedidos = ko.observableArray([]);
    this.CodigoCarga = PropertyEntity({ val: ko.observable() });
    this.ExportarDetalhesPedido = PropertyEntity({ eventClick: ExportarDetalhesPedido, type: types.event, text: "Exportar", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe), enable: ko.observable(true) });
}

var HistoricoObservacaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable() });
    this.GridHistorico = PropertyEntity({ idGrid: guid() });
    this.AdicionarObservacaoPedido = PropertyEntity({ eventClick: abrirModalAdicionarObservacaoPedido, type: types.event, text: Localization.Resources.Gerais.Geral.AdicionarObservacao, visible: ko.observable(true), enable: ko.observable(true) });
}

var ObservacaoPedido = function () {
    this.Observacao = PropertyEntity({ val: ko.observable(""), maxlength: 999999, text: Localization.Resources.Gerais.Geral.Observacao, enable: ko.observable(true) });
    this.Salvar = PropertyEntity({ eventClick: alterarObservacaoPedido, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
}

var AlteracaoDataPrevisaoEntrega = function () {
    this.CodigoCarga = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription(), val: ko.observable() });
    this.CodigoEntrega = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription(), val: ko.observable() });
    this.DataPrevisaoReprogramada = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataPrevisaoReprogramada.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(0), enable: ko.observable(true), visible: ko.observable(false) });
    this.DataPrevisaoPlanejada = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataPrevisaoPlanejada.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), enable: ko.observable(true), visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Confirmar, type: types.event, val: ko.observable(false), eventClick: ConfirmarAlteracaoDataPrevisaoEntregaClick, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, type: types.event, val: ko.observable(false), eventClick: CancelaAlteracaoDataPrevisaoEntregaClick, visible: ko.observable(true) });
};

function loadDetalhesPedidosTorreControle(codigoCarga) {
    $.get("Content/TorreControle/DetalhesPedido/ModalDetalhesPedido.html?dyn=" + guid(), function (htmlModalDetalhesPedido) {
        $("#ModalDetalhesPedido").html(htmlModalDetalhesPedido);

        _detalhesPedidos = new DetalhesPedidos();
        _detalhesPedidos.CodigoCarga.val(codigoCarga);
        KoBindings(_detalhesPedidos, "knockoutDetalhesPedidos");

        _historicoObservacaoPedido = new HistoricoObservacaoPedido();
        KoBindings(_historicoObservacaoPedido, "knoutHistoricoObservacaoPedido");

        _alteracaoDataPrevisaoEntrega = new AlteracaoDataPrevisaoEntrega();
        KoBindings(_alteracaoDataPrevisaoEntrega, "knockoutAlteracaoDataPrevisaoEntrega");

        _observacaoPedido = new ObservacaoPedido();
        KoBindings(_observacaoPedido, "knockoutObservacaoPedido");

        loadDetalhePedidoAnexoEmbarcador();
        LoadOcorrenciaEntrega();

        _modalAnexosPedidoEmbarcadorDetalhesPedido = new bootstrap.Modal(document.getElementById("divModalAnexosPedidoEmbarcador"), { backdrop: 'static', keyboard: true });

        BuscarDetalhesPedido(codigoCarga);
    });
}

function BuscarDetalhesPedido(codigoCarga) {
    const data = { CodigoCarga: codigoCarga };

    _detalhesPedidos.DetalhesPedidos.removeAll();

    executarReST("MonitoramentoNovo/ObterDetalhesPedidos", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                for (let i = 0; i < arg.Data.length; i++) {

                    var knoutDetalhesPedido = new DetalhesPedido(arg.Data[i]);

                    knoutDetalhesPedido.Carga.val(arg.Data[i].Carga.CodigoCargaEmbarcador);
                    knoutDetalhesPedido.CargaCritica.val(arg.Data[i].Carga.CargaCriticaDescricao);
                    knoutDetalhesPedido.Destinatario.val(arg.Data[i].Destinatario.Nome);
                    knoutDetalhesPedido.EnderecoDestino.val(arg.Data[i].Destinatario.Endereco.EnderecoCompleto);
                    knoutDetalhesPedido.DataEntrega.val(arg.Data[i].CargaEntrega?.DataEntregaFormatada);
                    knoutDetalhesPedido.DataFaturamento.val(arg.Data[i].DataFaturamento);
                    knoutDetalhesPedido.Produtos.val(arg.Data[i].Produtos);
                    knoutDetalhesPedido.Ocorrencias.val(arg.Data[i].Ocorrencias);
                    knoutDetalhesPedido.CanalVenda.val(arg.Data[i].CanalVenda);
                    knoutDetalhesPedido.EquipeVendas.val(arg.Data[i].EquipeVendas);
                    knoutDetalhesPedido.TipoMercadoria.val(arg.Data[i].TipoMercadoria);
                    knoutDetalhesPedido.EscritorioVenda.val(arg.Data[i].EscritorioVenda);
                    knoutDetalhesPedido.Peso.val(arg.Data[i].PesoTotal);
                    knoutDetalhesPedido.CodigoCarga.val(arg.Data[i].CodigoCarga);
                    knoutDetalhesPedido.CanalEntrega.val(arg.Data[i].CanalEntrega);
                    knoutDetalhesPedido.Item.val(arg.Data[i].Adicional7);
                    knoutDetalhesPedido.PedidoCritico.val(arg.Data[i].PedidoCritico);

                    knoutDetalhesPedido.DataPrevisaoEntrega.val(arg.Data[i].CargaEntrega?.DataPrevisaoEntregaFormatada);
                    knoutDetalhesPedido.DataPrevisaoEntregaAjustada.val(arg.Data[i].CargaEntrega?.DataPrevisaoEntregaAjustadaFormatada);
                    knoutDetalhesPedido.DataReprogramada.val(arg.Data[i].CargaEntrega?.DataReprogramadaFormatada);
                    knoutDetalhesPedido.CodigoCargaEntrega.val(arg.Data[i].CargaEntrega?.Codigo);
                    knoutDetalhesPedido.FarolStatus.val(arg.Data[i].CargaEntrega?.FarolStatus ?? '#a19d9c');

                    _detalhesPedidos.DetalhesPedidos.push(knoutDetalhesPedido);

                    carregarGridProdutos(knoutDetalhesPedido, arg.Data[i].Produtos);

                    carregarGridOcorrencias(knoutDetalhesPedido, arg.Data[i].Ocorrencias);
                    carregarGridOcorrenciasComerciais(knoutDetalhesPedido, arg.Data[i].OcorrenciasComerciais);

                    if (arg.Data[i].NotasFiscais.length > 0) {
                        const notas = arg.Data[i].NotasFiscais.map(x => x.NumeroNota).join(',');
                        knoutDetalhesPedido.NotasFiscais.val(notas);
                    }
                    //CarregarGrid(knockoutDetalhesPedidos, arg.Data[i].HistoricoPedidoObservacao)

                    Global.abrirModal("divModalDetalhesPedidos");
                }
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function carregarGridProdutos(knoutDetalhesPedido, produtos) {
    const header = [
        { data: "Codigo", visible: false },
        { data: "CodigoProdutoEmbarcador", title: "Código Integração", width: "100%", className: "text-align-left" },
        { data: "Descricao", title: "Descrição", width: "100%", className: "text-align-left" }
    ];

    const gridProdutos = new BasicDataTable(knoutDetalhesPedido.Produtos.idGrid, header, null, null, null, 10);
    gridProdutos.CarregarGrid(produtos);
}

function carregarGridOcorrencias(knoutDetalhesPedido, ocorrencias) {
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Ocorrência", width: "100%", className: "text-align-left" },
        { data: "Latitude", title: "Latitude", width: "100%", className: "text-align-left" },
        { data: "Longitude", title: "Longitude", width: "100%", className: "text-align-left" },
        { data: "DataOcorrencia", title: "Data da Ocorrência", width: "100%", className: "text-align-left" },
        { data: "DataPosicao", title: "Data Posição", width: "100%", className: "text-align-left" },
        { data: "DataReprogramada", title: "Data Previsão Reprogramada", width: "100%", className: "text-align-left" },
        { data: "TempoPercurso", title: "Tempo Percurso", width: "100%", className: "text-align-left" },
        { data: "Distancia", title: "Distancia Até o Destino", width: "100%", className: "text-align-left" },
        { data: "Origem", title: "Origem", width: "100%", className: "text-align-left" },
    ];

    const gridOcorrencias = new BasicDataTable(knoutDetalhesPedido.Ocorrencias.idGrid, header, null, null, null, 10);
    gridOcorrencias.CarregarGrid(ocorrencias);
}

function carregarGridOcorrenciasComerciais(knoutDetalhesPedido, ocorrencias) {
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Ocorrência", width: "100%", className: "text-align-left" },
        { data: "Latitude", title: "Latitude", width: "100%", className: "text-align-left" },
        { data: "Longitude", title: "Longitude", width: "100%", className: "text-align-left" },
        { data: "DataOcorrencia", title: "Data da Ocorrência", width: "100%", className: "text-align-left" },
        { data: "DataPosicao", title: "Data Posição", width: "100%", className: "text-align-left" },
        { data: "DataReprogramada", title: "Data Previsão Reprogramada", width: "100%", className: "text-align-left" },
        { data: "TempoPercurso", title: "Tempo Percurso", width: "100%", className: "text-align-left" },
        { data: "Distancia", title: "Distancia Até o Destino", width: "100%", className: "text-align-left" },
        { data: "Origem", title: "Origem", width: "100%", className: "text-align-left" },
        { data: "Natureza", title: "Natureza", width: "100%", className: "text-align-left" },
        { data: "GrupoOcorrencia", title: "Grupo de Ocorrência", width: "100%", className: "text-align-left" },
        { data: "Razao", title: "Razão", width: "100%", className: "text-align-left" },
        { data: "NotaFiscalDevolucao", title: "NF de Devolução", width: "100%", className: "text-align-left" },
        { data: "SolicitacaoCliente", title: "Solicitação do Cliente", width: "100%", className: "text-align-left" },
    ];

    let gridOcorrenciasComerciais = new BasicDataTable(knoutDetalhesPedido.OcorrenciasComerciais.idGrid, header, null, null, null, 10);
    gridOcorrenciasComerciais.CarregarGrid(ocorrencias);
}

function loadGridHistoricoPedidoObservacao() {
    let visualizarPedidoObservacao = { descricao: Localization.Resources.Gerais.Geral.Visualizar, id: guid(), evento: "onclick", metodo: abrirModalVisualizarPedidoObservacao, tamanho: "10", icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Visualizar, tamanho: 10, opcoes: [visualizarPedidoObservacao] };

    _gridHistoricoPedidoObservacao = new GridView(_historicoObservacaoPedido.GridHistorico.idGrid, "Pedido/BuscarHistoricoPedidoObservacao", _historicoObservacaoPedido, menuOpcoes, null, 10);
    _gridHistoricoPedidoObservacao.CarregarGrid();
}

function fecharModalDetalhesPedidos() {
    LimparCampos(_detalhesPedidos.DetalhesPedidos);
    Global.fecharModal("divModalDetalhesPedidos");
}

function visualizarAnexosPedidoClick(registroSelecionado) {
    const codigo = registroSelecionado.Codigo.val();

    carregarAnexosPedidoEmbarcador(codigo);

    _modalAnexosPedidoEmbarcadorDetalhesPedido.show();
}

function alterarDataAgendamentoEntregaClick() {
    AbrirModalAlterarDataAgendamentoEntrega();
}

function abrirModalHistoricoObservacaoPedido(detalhePedido) {

    _historicoObservacaoPedido.Codigo.val(detalhePedido.Codigo.val());
    loadGridHistoricoPedidoObservacao();

    Global.abrirModal("divModalHistoricoObservacaoPedido");
}

function abrirModalAdicionarObservacaoPedido(registroSelecionado) {
    _observacaoPedido.Observacao.val("");
    _observacaoPedido.Observacao.enable(true);
    _observacaoPedido.Salvar.visible(true);
    Global.abrirModal("divModalAlterarObservacaoPedido");
}

function abrirModalVisualizarPedidoObservacao(registroSelecionado) {
    _observacaoPedido.Observacao.val(registroSelecionado.Observacao);
    _observacaoPedido.Observacao.enable(false);
    _observacaoPedido.Salvar.visible(false);
    Global.abrirModal('divModalAlterarObservacaoPedido');
}

function alterarObservacaoPedido(pedido) {
    const data = {
        Codigo: _historicoObservacaoPedido.Codigo.val(),
        Observacao: _observacaoPedido.Observacao.val()
    }

    executarReST("Pedido/AlterarObservacaoPedido", data, function (retorno) {
        if (retorno.Success) {
            Global.fecharModal("divModalAlterarObservacaoPedido");
            Global.fecharModal("divModalHistoricoObservacaoPedido");
            fecharModalDetalhesPedidos();
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function abrirModalAdicionarOcorrenciaEntrega(registroSelecionado) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pedidos.Pedido.AcaoNaoEPermitida);
        return;
    }

    const pedido = registroSelecionado;
    _ocorrenciaEntrega.CodigoCargaEntrega.val(pedido.CodigoCargaEntrega.val());
    _ocorrenciaEntrega.CodigoCarga.val(pedido.CodigoCarga.val());
    Global.abrirModal("divModalAdicionarOcorrenciaEntrega");
}

function abrirModalAlteracaoDataPrevisaoEntrega(registroSelecionado) {
    _alteracaoDataPrevisaoEntrega.CodigoEntrega.val(registroSelecionado.CodigoCargaEntrega.val());
    _alteracaoDataPrevisaoEntrega.CodigoCarga.val(registroSelecionado.CodigoCarga.val());
    Global.abrirModal("divModalAlteracaoDataPrevisaoEntrega");
}

function ConfirmarAlteracaoDataPrevisaoEntregaClick() {

    const data = {
        CodigoProximaEntrega: 0,
        CodigoEntrega: _alteracaoDataPrevisaoEntrega.CodigoEntrega.val(),
        DataPrevisaoReprogramada: _alteracaoDataPrevisaoEntrega.DataPrevisaoReprogramada.val(),
        DataPrevisaoPlanejada: _alteracaoDataPrevisaoEntrega.DataPrevisaoPlanejada.val()
    };
    const codigoCarga = _alteracaoDataPrevisaoEntrega.CodigoCarga.val()
    executarReST("Monitoramento/AlterarDatasPrevisoes", data, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
            CancelaAlteracaoDataPrevisaoEntregaClick();
            fecharModalDetalhesPedidos()
            loadDetalhesPedidosTorreControle(codigoCarga);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function CancelaAlteracaoDataPrevisaoEntregaClick() {
    LimparCampos(_alteracaoDataPrevisaoEntrega);
    Global.fecharModal("divModalAlteracaoDataPrevisaoEntrega");
}

function abrirModalPedidoCritico(detalhesPedido) {
    let mensagemConfirmacaoPedidoCritico = '';

    if (detalhesPedido.PedidoCritico.val() === true) {
        mensagemConfirmacaoPedidoCritico = 'Deseja atualizar o pedido como <span style="color:CornflowerBlue;font-weight:700;"> não crítico </span>?';

    } else {
        mensagemConfirmacaoPedidoCritico = 'Deseja atualizar o pedido como <span style="color:red;font-weight:700;"> crítico </span>?';
    }

    exibirConfirmacao('Pedido crítico', mensagemConfirmacaoPedidoCritico, function () {
        let data = {
            Codigo: detalhesPedido.Codigo.val(),
        };

        executarReST("Pedido/SetarPedidoCritico", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    let retorno = arg.Data;
                    let mensagemRetorno = '';

                    retorno.CargaCritica === true ? detalhesPedido.CargaCritica.val(Localization.Resources.Gerais.Geral.Sim) : detalhesPedido.CargaCritica.val("-");

                    if (retorno.PedidoCritico === true) {
                        detalhesPedido.PedidoCritico.val(true);
                        mensagemRetorno = 'Pedido marcado como crítico';
                    } else {
                        detalhesPedido.PedidoCritico.val(false);
                        mensagemRetorno = 'Pedido marcado como não crítico';
                    }

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, mensagemRetorno );
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function ExportarDetalhesPedido(e) {
    executarDownload("MonitoramentoNovo/ExportarDetalhesPedidos", { CodigoCarga: e.CodigoCarga.val() }, null, null, true, true);
}