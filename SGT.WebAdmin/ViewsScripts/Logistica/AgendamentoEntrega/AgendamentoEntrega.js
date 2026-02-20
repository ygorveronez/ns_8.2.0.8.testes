/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/NotaFiscalSituacao.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAgendamentoEntregaPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoContatoAgendamentoEntregaPedido.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="AgendamentoEntregaContato.js" />
/// <reference path="AgendamentoEntregaHorarioDescarga.js" />
/// <reference path="AgendamentoEntregaLegendas.js" />
/// <reference path="AgendamentoEntregaSugestaoDataEntrega.js" />
/// <reference path="../../Cargas/Pedido/DetalhesPedido.js" />
/// <reference path="../../Pedidos/RegraPlanejamentoFrota/Destino.js" />

var _pesquisaAgendamentoEntregaPedido;
var _gridAgendamentoEntregaPedido;
var _agendamentoEntregaPedido;
var _envioEmailAgendamento;
var _gravarSenhaAgendamentoEntrega;
var _destinatarioSelecionado;
var _reagendamento;
var _registroEnvioEmail;
var _modalInformarDadosAgendamento;
var _opcoesMultiplaSelecao;
var _gridEstadoDestino;

var _opcoesDataTerminoCarregamento = [
    { text: "Todos", value: "" },
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var _opcoesDataSugestaoEntrega = [
    { text: "Todos", value: "" },
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var _opcoesPossuiNotaFiscal = [
    { text: "Todos", value: "" },
    { text: "Sim", value: 1 },
    { text: "Não", value: 2 }
];

var _opcoesStatusCarga = [
    { text: "Não Finalizadas", value: "" },
    { text: "Finalizadas", value: 1 }
];

var _opcoesModeloEmailPadrao = [
    { text: "Modelo Padrão", value: 0 },
];

var _opcoesSenhaDeAgendamento = [
    { text: "Todos", value: "" },
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var PesquisaAgendamentoEntregaPedido = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.NFe = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), configInt: { precision: 0, allowZero: false, thousands: "" }, text: "NF-e:" });
    this.Carga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Carga:" });
    this.TipoOperacao = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Tipo de Operação:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Cliente = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.multiplesEntities, text: "Cliente:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Transportador = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.multiplesEntities, text: "Transportador:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.UFDestino = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.multiplesEntities, text: "UF Destino:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.SituacaoNotaFiscal = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.multiplesEntities, text: "Situação Nota Fiscal:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.StatusCarga = PropertyEntity({ text: "Status da Carga:", options: _opcoesStatusCarga, def: "", val: ko.observable(""), enable: ko.observable(true) });

    this.SenhaEntregaAgendamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Senha do Agendamento:" });
    this.EntegasComSenhaDeAgendamento = PropertyEntity({ text: "Apenas Agendamentos com Senha:", options: _opcoesSenhaDeAgendamento, def: "", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(false) });

    this.CanalEntrega = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.multiplesEntities, text: "Canal de Entrega:", idBtnSearch: guid(), enable: ko.observable(true), visible: true });

    this.DataCarregamentoInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Carregamento Inicial:", getType: typesKnockout.date });
    this.DataCarregamentoFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Carregamento Final:", getType: typesKnockout.date });

    this.DataCarregamentoInicial.dateRangeLimit = this.DataCarregamentoFinal;
    this.DataCarregamentoFinal.dateRangeInit = this.DataCarregamentoInicial;

    this.DataAgendamentoInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Agendamento Inicial:", getType: typesKnockout.date });
    this.DataAgendamentoFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Agendamento Final:", getType: typesKnockout.date });

    this.DataAgendamentoInicial.dateRangeLimit = this.DataAgendamentoFinal;
    this.DataAgendamentoFinal.dateRangeInit = this.DataAgendamentoInicial;

    this.DataPrevisaoEntregaInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Previsão Entrega Inicial:", getType: typesKnockout.date });
    this.DataPrevisaoEntregaFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Previsão Entrega Final:", getType: typesKnockout.date });

    this.DataPrevisaoEntregaInicial.dateRangeLimit = this.DataPrevisaoEntregaFinal;
    this.DataPrevisaoEntregaFinal.dateRangeInit = this.DataPrevisaoEntregaInicial;

    this.DataCriacaoPedidoInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Criação Pedido Inicial:", getType: typesKnockout.date });
    this.DataCriacaoPedidoFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Criação Pedido Final:", getType: typesKnockout.date });

    this.DataCriacaoPedidoInicial.dateRangeLimit = this.DataCriacaoPedidoFinal;
    this.DataCriacaoPedidoFinal.dateRangeInit = this.DataCriacaoPedidoInicial;

    this.DataInicialSugestaoEntrega = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Suges. Ent. Inicial:", getType: typesKnockout.date });
    this.DataFinalSugestaoEntrega = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Suges. Ent. Final:", getType: typesKnockout.date });

    this.DataInicialSugestaoEntrega.dateRangeLimit = this.DataFinalSugestaoEntrega;
    this.DataFinalSugestaoEntrega.dateRangeInit = this.DataInicialSugestaoEntrega;

    this.SituacaoAgendamento = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), def: new Array(), options: EnumSituacaoAgendamentoEntregaPedido.obterOpcoes(), text: "Situação do Agendamento:" });

    this.PossuiDataTermioCarregamento = PropertyEntity({ text: "Data Término de Carregamento:", options: _opcoesDataTerminoCarregamento, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.PossuiDataSugestaoEntrega = PropertyEntity({ text: "Data Sugestão de Entrega:", options: _opcoesDataSugestaoEntrega, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.PossuiNotaFiscalVinculada = PropertyEntity({ text: "Possui nota fiscal vinculada:", options: _opcoesPossuiNotaFiscal, val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.NumeroOrdem = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Número Ordem:" });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarAgendamentoEntregaPedido, type: types.event, text: "Pesquisar", visible: ko.observable(true) });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaAgendamentoEntregaPedido.Visible.visibleFade()) {
                _pesquisaAgendamentoEntregaPedido.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaAgendamentoEntregaPedido.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(false)
    });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });

    this.DataInicialCriacaoDaCarga = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Criação da Carga Inicial:", getType: typesKnockout.date });
    this.DataFinalCriacaoDaCarga = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Criação da Carga Final:", getType: typesKnockout.date });
    this.DataInicialCriacaoDaCarga.dateRangeLimit = this.DataFinalCriacaoDaCarga;
    this.DataFinalCriacaoDaCarga.dateRangeInit = this.DataInicialCriacaoDaCarga;

}

var AgendamentoEntregaPedido = function () {
    this.Grid = PropertyEntity({ idGrid: "grid-agendamento-entrega" });
}

var InformarMotivoContatos = function () {
    this.ContatoDescricao = PropertyEntity({ text: "Contato: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.AdicionarContatos = PropertyEntity({ eventClick: aprovarContatosClick, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(true) });
}

var EnvioEmailAgendamento = function () {
    this.TituloModal = PropertyEntity({ text: "Envio de Email para Agendamento" });
    this.ModeloEmail = PropertyEntity({ text: "Modelo de Email:", getType: typesKnockout.select, val: ko.observable(_opcoesModeloEmailPadrao[0]), options: ko.observable(_opcoesModeloEmailPadrao), def: _opcoesModeloEmailPadrao[0] });
    this.EnviarEmailParaTodosOsPedidosDoMesmoDestinoECarga = PropertyEntity({ text: "Enviar email para todos os pedidos do mesmo Destino e Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarEmail = PropertyEntity({ eventClick: enviarEmailAgendamento, type: types.event, text: "Enviar Email", idGrid: guid(), visible: ko.observable(true) });
    this.ContatoTransportador = PropertyEntity({ val: ko.observable(false) });
    this.MultiplasCargas = PropertyEntity({ val: ko.observable(false) });
}

var GravarSenhaAgendamentoEntrega = function () {
    this.TituloModal = PropertyEntity({ text: "Gravar Senha do Agendamento" });
    this.SenhaEntregaAgendamento = PropertyEntity({ getType: typesKnockout.string, maxlength: 100, val: ko.observable(""), required: true, text: "*Senha do Agendamento" });
    this.CodigoPedido = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0, required: false});
    this.CodigoCargaEntrega = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0, required: false});
    this.Salvar = PropertyEntity({ type: types.event, eventClick: gravarSenhaAgendamentoEntrega, text: "Salvar", id: guid() });
}

var InformarDadosAgendamento = function () {
    this.Pedido = PropertyEntity({ text: "Código Pedido:", val: ko.observable(0) });
    this.NomeResponsavel = PropertyEntity({ text: "*Nome do Responsável:", val: ko.observable(""), required: true, maxlength: 100 });
    this.SenhaAgendamento = PropertyEntity({ text: "*Senha de Agendamento:", val: ko.observable(""), required: true, maxlength: 150 });
    this.DataAgendamento = PropertyEntity({ text: "*Data do Agendamento:", enable: true, getType: typesKnockout.dateTime, val: ko.observable(""), required: true });
    this.ProtocoloAgendamento = PropertyEntity({ text: "*Protocolo:", val: ko.observable(""), required: true, maxlength: 100 });
    this.CodigoCargaEntrega = PropertyEntity({ val: ko.observable(0) });

    this.Enviar = PropertyEntity({
        eventClick: function (e, sender) {
            if (ValidarCamposObrigatorios(_informarDadosAgendamento))
                if (enviarDadosAgendamento(_informarDadosAgendamento, sender))
                    _modalInformarDadosAgendamento.hide();
        }, type: types.event, text: "Enviar", idGrid: guid(), visible: ko.observable(true)
    });
};

var OpcoesMultiplaSelecao = function () {
    this.Opcoes = ko.observableArray();
}

function loadAgendamentoEntregaPedido() {
    _pesquisaAgendamentoEntregaPedido = new PesquisaAgendamentoEntregaPedido();
    KoBindings(_pesquisaAgendamentoEntregaPedido, "knockoutPesquisaAgendamentoEntregaPedido");

    _agendamentoEntregaPedido = new AgendamentoEntregaPedido();
    KoBindings(_agendamentoEntregaPedido, "knockoutAgendamentoEntregaPedido");

    _informarMotivoContatos = new InformarMotivoContatos();
    KoBindings(_informarMotivoContatos, "knockoutInformacaoContatoCliente");

    _envioEmailAgendamento = new EnvioEmailAgendamento();
    KoBindings(_envioEmailAgendamento, "knockoutEnvioEmailAgendamento");

    _gravarSenhaAgendamentoEntrega = new GravarSenhaAgendamentoEntrega();
    KoBindings(_gravarSenhaAgendamentoEntrega, "knockoutGravarSenhaAgendamentoEntrega");

    new BuscarTiposOperacao(_pesquisaAgendamentoEntregaPedido.TipoOperacao);
    new BuscarClientes(_pesquisaAgendamentoEntregaPedido.Cliente);
    new BuscarTransportadores(_pesquisaAgendamentoEntregaPedido.Transportador);
    new BuscarNotaFiscalSituacao(_pesquisaAgendamentoEntregaPedido.SituacaoNotaFiscal);
    new BuscarCanaisEntrega(_pesquisaAgendamentoEntregaPedido.CanalEntrega);
    new BuscarEstados(_pesquisaAgendamentoEntregaPedido.UFDestino);

    _modalInformarDadosAgendamento = new bootstrap.Modal(document.getElementById("divInformarDadosAgendamento"), { backdrop: true, keyboard: true });

    loadGridAgendamentoEntregaPedido();
    loadOpcoesMultiplaSelecao();
    loadAlterarHorarioAgendamentoDescarga();
    loadAgendamentoEntregaContato();
    loadAgendamentoEntregaLegendas();
    loadAgendamentoEntregaSugestaoDataEntrega();
    loadAgendamentoEntregaNotaFiscalSituacao();
    loadAgendamentoConsultaTransportador();
    loadAgendamentoAgendamentoEntregaHistorico();

    executarReST("AgendamentoEntregaPedido/BuscarOpcoesModeloEmail", null, function (retorno) {
        if (retorno.Success && retorno.Data)
            _opcoesModeloEmailPadrao = retorno.Data;
    }, null, false);

}

function loadOpcoesMultiplaSelecao() {
    _opcoesMultiplaSelecao = new OpcoesMultiplaSelecao();
    KoBindings(_opcoesMultiplaSelecao, "menu-drop-down");
    _opcoesMultiplaSelecao.Opcoes.removeAll();

    var opcaoAgendarDescarga = {
        text: "Agendar Descarga",
        id: "agendar-descarga",
        eventClick: function () { agendarDescargaMultiplosClick(false); },
        visible: true
    };
    var opcaoReagendarDescarga = {
        text: "Reagendar",
        id: "reagendar-descarga",
        eventClick: function () { agendarDescargaMultiplosClick(true); },
        visible: true
    };
    var opcaoAdicionarContato = {
        text: "Adicionar Contato Cliente",
        id: "adicionar-contato",
        eventClick: exibirModalInformarContatoClick,
        visible: true
    };
    var opcaoSolicitarAgendamento = {
        text: "Solicitar Agendamento",
        id: "solicitar-agendamento",
        eventClick: solicitarAgendamentoClick,
        visible: true
    };
    var opcaoEnviarEmailAgendamento = {
        text: "Enviar Email Agendamento",
        id: "enviar-email-agendamento",
        eventClick: enviarEmailAgendamentoMultiplasCargasClick,
        visible: true
    };

    _opcoesMultiplaSelecao.Opcoes.push(opcaoAgendarDescarga, opcaoReagendarDescarga, opcaoAdicionarContato, opcaoSolicitarAgendamento, opcaoEnviarEmailAgendamento);
}

function loadGridAgendamentoEntregaPedido() {

    var opcaoSugerirDataEntrega = {
        descricao: "Sugerir Data de Entrega",
        id: guid(),
        evento: "onclick",
        metodo: exibirModalAgendamentoSugestaoDataEntrega,
        visibilidade: visibilidadeSugerirDataEntrega
    };

    var opcaoAgendarDescarga = {
        descricao: "Agendar Descarga",
        id: guid(),
        evento: "onclick",
        metodo: agendarDescargaClick,
        visibilidade: visibilidadeAgendarDescarga
    };

    var opcaoSugestaoReagendamento = {
        descricao: "Sugestão de Reagendamento",
        id: guid(),
        evento: "onclick",
        metodo: sugestaoReagendamentoClick,
        visibilidade: visibilidadeReagendar
    };

    var opcaoAlterarAgendamento = {
        descricao: "Reagendar",
        id: guid(),
        evento: "onclick",
        metodo: reagendarClick,
        visibilidade: visibilidadeReagendar
    };

    var opcaoContatoTransportador = {
        descricao: "Contato Transportador",
        id: guid(),
        evento: "onclick",
        metodo: contatoTransportadorClick
    };

    var opcaoContatoCliente = {
        descricao: "Contato Cliente",
        id: guid(),
        evento: "onclick",
        metodo: contatoClienteClick
    };

    var opcaoBuscarRotaFrete = {
        descricao: "Buscar Rota",
        id: guid(),
        evento: "onclick",
        metodo: buscarRotaClick,
        visibilidade: function () { return !isPesquisarSomenteCargasFinalizadas() }
    };

    var opcaoAlterarSituacaoNotaFiscal = {
        descricao: "Alterar Situação",
        id: guid(),
        evento: "onclick",
        metodo: exibirModalAgendamentoNotaFiscalSituacao,
        visibilidade: function () { return !isPesquisarSomenteCargasFinalizadas() }
    };

    var opcaoAguardandoRetornoCliente = {
        descricao: "Ag. Retorno Cliente",
        id: guid(),
        evento: "onclick",
        metodo: aguardandoRetornoClienteClick,
        visibilidade: visibilidadeAguardandoRetornoCliente
    };

    var opcaoAuditoria = {
        descricao: "Auditoria",
        id: guid(),
        evento: "onclick",
        metodo: OpcaoAuditoria("Pedido", "CodigoPedido")
    };

    var opcaoAuditoriaConsulta = {
        descricao: "Consulta Transportador",
        id: guid(),
        evento: "onclick",
        metodo: consultaTransportadorClick
    };

    var opcaoHistoricoAgendamentoEntrega = {
        descricao: "Histórico Datas Agendamento",
        id: guid(),
        evento: "onclick",
        metodo: exibirModalAgendamentoAgendamentoEntregaHistorico
    };

    var opcaoAguardandoReagendamento = {
        descricao: "Ag. Reagendamento",
        id: guid(),
        evento: "onclick",
        metodo: aguardandoReagendamentoClick,
        visibilidade: visibilidadeReagendamento
    };

    var opcaoSolicitarAgendamento = {
        descricao: "Solicitar Agendamento",
        id: guid(),
        evento: "onclick",
        metodo: solicitarAgendamentoClick,
        visibilidade: visibilidadeAgendamento
    };

    var opcaoAssumirAgendamento = {
        descricao: "Assumir Agendamento",
        id: guid(),
        evento: "onclick",
        metodo: assumirAgendamentoClick,
        visibilidade: visibilidadeAssumirAgendamento
    };

    var opcaoInformarDadosAgendamento = {
        descricao: "Informar Dados Agendamento",
        id: guid(),
        evento: "onclick",
        metodo: informarDadosAgendamentoClick,
        visibilidade: visibilidadeInformarDadosAgendamento
    };

    var opcaoDetalhesCargaAgendamento = {
        descricao: "Detalhes da Carga",
        id: guid(),
        evento: "onclick",
        metodo: detalhesCargaAgendamentoClick
    };

    var opcaoEnviarEmailAgendamento = {
        descricao: "Enviar E-mail de Agendamento",
        id: guid(),
        evento: "onclick",
        metodo: enviarEmailAgendamentoClick,
        visibilidade: true
    };

    var opcaoGravarSenhaAgendamentoEntrega = {
        descricao: "Gravar Senha de Agendamento",
        id: guid(),
        evento: "onclick",
        metodo: gravarSenhaDeAgendamentoClick,
        visibilidade: true
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [opcaoSugerirDataEntrega,
            opcaoAgendarDescarga,
            opcaoSugestaoReagendamento,
            opcaoAlterarAgendamento,
            opcaoSolicitarAgendamento,
            opcaoEnviarEmailAgendamento,
            opcaoAguardandoReagendamento,
            opcaoAguardandoRetornoCliente,
            opcaoContatoTransportador,
            opcaoContatoCliente,
            opcaoAssumirAgendamento,
            opcaoInformarDadosAgendamento,
            opcaoDetalhesCargaAgendamento,
            opcaoBuscarRotaFrete,
            opcaoAlterarSituacaoNotaFiscal,
            opcaoAuditoria,
            opcaoAuditoriaConsulta,
            opcaoHistoricoAgendamentoEntrega,
            opcaoGravarSenhaAgendamentoEntrega]
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        callbackNaoSelecionado: callbackNaoSelecionado,
        callbackSelecionado: callbackSelecionado,
        somenteLeitura: false,
        classeSelecao: "item-selecionado"
    };

    var configExportacao = {
        url: "AgendamentoEntregaPedido/ExportarPesquisa",
        titulo: "Agendamento Entrega Pedido"
    };

    _gridAgendamentoEntregaPedido = new GridViewExportacao(_agendamentoEntregaPedido.Grid.idGrid, "AgendamentoEntregaPedido/Pesquisa", _pesquisaAgendamentoEntregaPedido, menuOpcoes, configExportacao, null, 10, multiplaEscolha, null, null, callbackColumnAgendamentoEntrega);
    _gridAgendamentoEntregaPedido.SetPermitirEdicaoColunas(true);
    _gridAgendamentoEntregaPedido.SetSalvarPreferenciasGrid(true);

    _gridAgendamentoEntregaPedido.CarregarGrid();
}

function callbackSelecionado(argumentoNulo, registro) {
    var registrosSelecionados = _gridAgendamentoEntregaPedido.ObterMultiplosSelecionados();

    if (registrosSelecionados.length == 1) {
        _destinatarioSelecionado = registro.CPFCNPJCliente;
        _reagendamento = registro.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.Agendado ||
            registro.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.Reagendado ||
            registro.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.AguardandoReagendamento;

        $("#botao-opcoes-multipla-selecao").css("visibility", "visible");

        if (!_reagendamento) {
            $("#agendar-descarga").parent().show();
            $("#reagendar-descarga").parent().hide();
        }
        else {
            $("#reagendar-descarga").parent().show();
            $("#agendar-descarga").parent().hide();
        }
    }

    if (registro.CPFCNPJCliente != _destinatarioSelecionado) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Os destinatários precisam ser iguais.");
        document.getElementById(registro.Codigo.toString()).click();
        return;
    }

    if (!verificarPermissaoSugestaoDataEntrega(registro)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "O registro selecionado não permite agendar ou reagendar, pois o mesmo já possui data de entrega sugerida.");
        document.getElementById(registro.Codigo.toString()).click();
        return;
    }

    var permiteReagendar = visibilidadeReagendar(registro);
    var permiteAgendar = visibilidadeAgendarDescarga(registro);

    var permiteSolicitarAgendamento = registrosSelecionados.every(function (r) {
        return visibilidadeAgendamento(r);
    });

    if (permiteSolicitarAgendamento)
        $("#solicitar-agendamento").parent().show();
    else
        $("#solicitar-agendamento").parent().hide();

    if (_reagendamento && !permiteReagendar) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é permitido reagendar o registro selecionado.");
        document.getElementById(registro.Codigo.toString()).click();
        return;
    }

    if (!_reagendamento && !permiteAgendar) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é permitido agendar o registro selecionado.")
        document.getElementById(registro.Codigo.toString()).click();
        return;
    }
}

function callbackNaoSelecionado(argumentoNulo, registro) {
    var registrosSelecionados = _gridAgendamentoEntregaPedido.ObterMultiplosSelecionados();

    if (registrosSelecionados.length == 0) {
        _destinatarioSelecionado = 0;
        _reagendamento = false;
        $("#botao-opcoes-multipla-selecao").css("visibility", "hidden");
    }
}

function pesquisarAgendamentoEntregaPedido() {
    _gridAgendamentoEntregaPedido.CarregarGrid();
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

//Métodos Privados

function obterCodigosSelecionados() {
    return _gridAgendamentoEntregaPedido.ObterMultiplosSelecionados().map(function (registro) {
        return registro.CodigoPedido;
    });
}
function obterCodigosCargaEntregaSelecionados() {
    return _gridAgendamentoEntregaPedido.ObterMultiplosSelecionados().map(function (registro) {
        return registro.CodigoCargaEntrega;
    });
}

function obterCodigosCargasSelecionadas() {
    return _gridAgendamentoEntregaPedido.ObterMultiplosSelecionados().map(function (registro) {
        return registro.CodigoCarga;
    });
}

function obterDatasPrevisaoEntregaRegistrosSelecionados() {
    return _gridAgendamentoEntregaPedido.ObterMultiplosSelecionados().filter((registro) => !string.IsNullOrWhiteSpace(registro.DataPrevisaoEntregaFormatada)).map(function (registro) {
        return registro.DataPrevisaoEntregaFormatada;
    });
}

//Métodos Click

function agendarDescargaMultiplosClick(reagendamento) {
    _alterarHorarioAgendamentoDescarga.Reagendamento.val(reagendamento);

    agendarDescargaClick();
}

function agendarDescargaClick(registroSelecionado) {
    let codigos = [];
    let codigosCargaEntrega = [];
    let exigeSenhaAgendamento = false;
    let senhaEntregaAgendamento = '';

    if (registroSelecionado != undefined) {
        codigos.push(registroSelecionado.CodigoPedido);
        codigosCargaEntrega.push(registroSelecionado.CodigoCargaEntrega);
        exigeSenhaAgendamento = registroSelecionado.ExigeSenhaAgendamento;
        senhaEntregaAgendamento = registroSelecionado.SenhaEntregaAgendamento;
    }
    else {
        codigos = obterCodigosSelecionados();
        codigosCargaEntrega = obterCodigosCargaEntregaSelecionados();
        exigeSenhaAgendamento = _gridAgendamentoEntregaPedido.ObterMultiplosSelecionados()[0].ExigeSenhaAgendamento;
        senhaEntregaAgendamento = _gridAgendamentoEntregaPedido.ObterMultiplosSelecionados()[0].SenhaEntregaAgendamento;
    }

    var datasPrevisaoEntrega = [];

    if (registroSelecionado != undefined && !string.IsNullOrWhiteSpace(registroSelecionado.DataPrevisaoEntregaFormatada))
        datasPrevisaoEntrega = [registroSelecionado.DataPrevisaoEntregaFormatada];
    else
        datasPrevisaoEntrega = obterDatasPrevisaoEntregaRegistrosSelecionados();

    var cpfCnpjDestinatario = registroSelecionado != undefined ? registroSelecionado.CPFCNPJCliente : _gridAgendamentoEntregaPedido.ObterMultiplosSelecionados()[0].CPFCNPJCliente;

    _alterarHorarioAgendamentoDescarga.Titulo.text("Agendar Descarga");
    _alterarHorarioAgendamentoDescarga.Reagendamento.val(false);
    _alterarHorarioAgendamentoDescarga.MotivoReagendamento.visible(false);
    _alterarHorarioAgendamentoDescarga.ResponsavelMotivoReagendamento.visible(false);
    _alterarHorarioAgendamentoDescarga.ObservacaoReagendamento.visible(false);

    exibirModalAgendamentoDescarga(codigos, cpfCnpjDestinatario, datasPrevisaoEntrega, codigosCargaEntrega, exigeSenhaAgendamento, senhaEntregaAgendamento);
}

function reagendarClick(registroSelecionado) {
    _alterarHorarioAgendamentoDescarga.Titulo.text("Reagendar");
    _alterarHorarioAgendamentoDescarga.Reagendamento.val(true);
    _alterarHorarioAgendamentoDescarga.MotivoReagendamento.visible(true);
    _alterarHorarioAgendamentoDescarga.ResponsavelMotivoReagendamento.visible(true);
    _alterarHorarioAgendamentoDescarga.ObservacaoReagendamento.visible(true);
    var datasPrevisaoEntrega = [];

    if (!string.IsNullOrWhiteSpace(registroSelecionado.DataPrevisaoEntregaFormatada))
        datasPrevisaoEntrega = [registroSelecionado.DataPrevisaoEntregaFormatada];

    exibirModalAgendamentoDescarga([registroSelecionado.CodigoPedido], registroSelecionado.CPFCNPJCliente, datasPrevisaoEntrega, [registroSelecionado.CodigoCargaEntrega], registroSelecionado.ExigeSenhaAgendamento, registroSelecionado.SenhaEntregaAgendamento);
}

function sugestaoReagendamentoClick(registroSelecionado) {
    _alterarHorarioAgendamentoDescarga.Reagendamento.val(true);
    _alterarHorarioAgendamentoDescarga.MotivoReagendamento.visible(true);
    _alterarHorarioAgendamentoDescarga.SugestaoReagendamento.val(true);
    var datasPrevisaoEntrega = [];

    if (!string.IsNullOrWhiteSpace(registroSelecionado.DataPrevisaoEntregaFormatada))
        datasPrevisaoEntrega = [registroSelecionado.DataPrevisaoEntregaFormatada];

    exibirModalAgendamentoDescarga([registroSelecionado.CodigoPedido], registroSelecionado.CPFCNPJCliente, datasPrevisaoEntrega, [registroSelecionado.CodigoCargaEntrega], false, '');
}

function buscarRotaClick(registroSelecionado) {
    executarReST("AgendamentoEntregaPedido/BuscarRota", { Codigo: registroSelecionado.CodigoPedido, CodigoCargaEntrega: registroSelecionado.CodigoCargaEntrega }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A rota do pedido foi alterada e a previsão de entrega foi recalculada.");
                _gridAgendamentoEntregaPedido.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function contatoClienteClick(registroSelecionado) {
    exibirModalAgendamentoEntregaContato(registroSelecionado.CodigoPedido, EnumTipoContatoAgendamentoEntregaPedido.Cliente, registroSelecionado.CPFCNPJCliente, null, registroSelecionado.CodigoCargaEntrega);
}

function contatoTransportadorClick(registroSelecionado) {
    _registroEnvioEmail = registroSelecionado;
    exibirModalAgendamentoEntregaContato(registroSelecionado.CodigoPedido, EnumTipoContatoAgendamentoEntregaPedido.Transportador, registroSelecionado.CodigoTransportador, registroSelecionado.DataAgendamentoFormatada, registroSelecionado.CodigoCargaEntrega);
}

function aguardandoRetornoClienteClick(registroSelecionado) {
    executarReST("AgendamentoEntregaPedido/AguardandoRetornoCliente", { Codigo: registroSelecionado.CodigoPedido, CodigoCargaEntrega: registroSelecionado.CodigoCargaEntrega }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A situação do agendamento foi alterada.");
                _gridAgendamentoEntregaPedido.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function consultaTransportadorClick(registroSelecionado) {
    exibirModalAgendamentoConsultaTransportador(registroSelecionado);
}

function aguardandoReagendamentoClick(registroSelecionado) {
    executarReST("AgendamentoEntregaPedido/AguardandoReagendamento", { Codigo: registroSelecionado.CodigoPedido, CodigoCargaEntrega: registroSelecionado.CodigoCargaEntrega }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "O agendamento foi alterado para aguardando reagendamento.");
                _gridAgendamentoEntregaPedido.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function solicitarAgendamentoClick(registroSelecionado) {
    let codigos = [];
    let codigosCargaEntrega = [];

    if (registroSelecionado != undefined) {
        codigos.push(registroSelecionado.CodigoPedido);
        codigosCargaEntrega.push(registroSelecionado.CodigoCargaEntrega);
    }
    else {
        codigos = obterCodigosSelecionados();
        codigosCargaEntrega = obterCodigosCargaEntregaSelecionados();
    }

    exibirConfirmacao("Confirmação", "Realmente deseja enviar e-mail de solicitação de agendamento ao(s) cliente(s)?", function () {
        executarReST("AgendamentoEntregaPedido/SolicitarAgendamento", { Pedidos: JSON.stringify(codigos), CodigosCargaEntrega: JSON.stringify(codigosCargaEntrega) }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "O agendamento foi solicitado com sucesso.");
                    _gridAgendamentoEntregaPedido.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function assumirAgendamentoClick(registroSelecionado) {
    executarReST("AgendamentoEntregaPedido/AssumirAgendamento", { Pedido: registroSelecionado.CodigoPedido, CodigoCargaEntrega: registroSelecionado.CodigoCargaEntrega }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Agendamento assumido com sucesso.");
                _gridAgendamentoEntregaPedido.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function informarDadosAgendamentoClick(registroSelecionado) {
    _informarDadosAgendamento = new InformarDadosAgendamento()
    KoBindings(_informarDadosAgendamento, "knockoutInformarDadosAgendamento", false, _informarDadosAgendamento.Enviar.id);

    _informarDadosAgendamento.Pedido.val(registroSelecionado.CodigoPedido);
    _informarDadosAgendamento.CodigoCargaEntrega.val(registroSelecionado.CodigoCargaEntrega);
    _modalInformarDadosAgendamento.show();
}

function detalhesCargaAgendamentoClick(registroSelecionado) {
    loadDetalhePedido();
    exibirDetalhesPedidos(registroSelecionado.CodigoCarga);
}

function exibirModalInformarContatoClick() {
    Global.abrirModal('divModalInformacaoContatoCliente');
}

function fecharInformarContatosCliente() {
    LimparCampos(_informarMotivoContatos);

    Global.fecharModal('divModalInformacaoContatoCliente');
}

function gravarSenhaDeAgendamentoClick(registro) {
    _gravarSenhaAgendamentoEntrega.CodigoPedido.val(registro.CodigoPedido);
    _gravarSenhaAgendamentoEntrega.CodigoCargaEntrega.val(registro.CodigoCargaEntrega);
    _gravarSenhaAgendamentoEntrega.SenhaEntregaAgendamento.val(registro.SenhaEntregaAgendamento);
    Global.abrirModal('divModalGravarSenhaAgendamentoEntrega');
}

function enviarEmailAgendamentoClick(registro) {
    _registroEnvioEmail = registro;
    _envioEmailAgendamento.EnviarEmailParaTodosOsPedidosDoMesmoDestinoECarga.visible(true);
    _envioEmailAgendamento.MultiplasCargas.val(false);
    _envioEmailAgendamento.CodigosCargas = [];
    abrirModalEnvioEmailAgendamentoNotificacao();
}

function enviarEmailAgendamentoMultiplasCargasClick() {
    _registroEnvioEmail = _gridAgendamentoEntregaPedido.ObterMultiplosSelecionados()[0];
    _envioEmailAgendamento.EnviarEmailParaTodosOsPedidosDoMesmoDestinoECarga.visible(false);
    _envioEmailAgendamento.MultiplasCargas.val(true);
    _envioEmailAgendamento.CodigosCargas = obterCodigosCargasSelecionadas();
    abrirModalEnvioEmailAgendamentoNotificacao();
}

function abrirModalEnvioEmailAgendamentoNotificacao() {
    _envioEmailAgendamento.ModeloEmail.options(_opcoesModeloEmailPadrao);
    _envioEmailAgendamento.ModeloEmail.val(_opcoesModeloEmailPadrao[0].value);
    Global.abrirModal('divModalEnvioEmailAgendamento');
}

function enviarEmailAgendamento(evento) {
    if (evento.ContatoTransportador.val()) {
        exibirConfirmacao("Confirmação", "Realmente deseja enviar e-mail de notificação para o Transportador?", function () {
            _registroEnvioEmail.ModeloEmail = evento.ModeloEmail.val();
            executarReST("AgendamentoEntregaPedido/EnviarEmailNotificacaoTransportador", _registroEnvioEmail, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "E-mail enviado com sucesso.");
                        _gridAgendamentoEntregaPedido.CarregarGrid();
                        Global.fecharModal('divModalEnvioEmailAgendamento');
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    } else {
        exibirConfirmacao("Confirmação", "Realmente deseja enviar e-mail do agendamento para o cliente?", function () {
            _registroEnvioEmail.EnviarEmailParaTodosOsPedidosDoMesmoDestinoECarga = evento.EnviarEmailParaTodosOsPedidosDoMesmoDestinoECarga.val();
            _registroEnvioEmail.ModeloEmail = evento.ModeloEmail.val();
            _registroEnvioEmail.MultiplasCargas = evento.MultiplasCargas.val()
            _registroEnvioEmail.CodigosCargas = JSON.stringify(evento.CodigosCargas);
            executarReST("AgendamentoEntregaPedido/EnviarEmailAgendamento", _registroEnvioEmail, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "E-mail enviado com sucesso.");
                        _gridAgendamentoEntregaPedido.CarregarGrid();
                        _gridAgendamentoEntregaPedido.AtualizarRegistrosSelecionados([]);
                        Global.fecharModal('divModalEnvioEmailAgendamento');
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    }
}

function aprovarContatosClick() {
    var listaCodigos = [];

    listaCodigos = obterCodigosSelecionados();
    let listaCodigosEntregas = obterCodigosCargaEntregaSelecionados();

    var informarContato = RetornarObjetoPesquisa(_informarMotivoContatos);

    var informacaoContato = informarContato.ContatoDescricao;

    executarReST("AgendamentoEntregaPedido/SalvarInformacaoContatos", { Codigos: JSON.stringify(listaCodigos), InformacaoContato: informacaoContato, CodigosCargaEntrega: JSON.stringify(listaCodigosEntregas) }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação enviada com sucesso.");
                fecharInformarContatosCliente();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function enviarDadosAgendamento(knockout, sender) {
    Salvar(knockout, "AgendamentoEntregaPedido/InformarDadosAgendamento", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
    return true;
}

function verificarPermissaoSugestaoDataEntrega(registro) {
    return ((registro.PermitirAgendarDescargaAposDataEntregaSugerida && registro.DataSugestaoEntregaFormatada != "")
        || !registro.PermitirAgendarDescargaAposDataEntregaSugerida);
}

//Funções Visibilidade

function visibilidadeAgendarDescarga(registroSelecionado) {
    return !isPesquisarSomenteCargasFinalizadas() && (registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.AguardandoAgendamento || registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.AguardandoRetornoCliente) &&
        registroSelecionado.PermiteAgendarEntrega &&
        !EnumSituacoesCarga.obterSituacoesCargaCanceladaEncerradaAnulada().includes(registroSelecionado.SituacaoViagem)
        && verificarPermissaoSugestaoDataEntrega(registroSelecionado) && string.IsNullOrWhiteSpace(registroSelecionado.DataAgendamentoFormatada);
}

function visibilidadeSugerirDataEntrega(registroSelecionado) {
    return !isPesquisarSomenteCargasFinalizadas();
}

function visibilidadeReagendamento(registroSelecionado) {
    return (registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.Agendado || registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.Reagendado) && !isPesquisarSomenteCargasFinalizadas() && registroSelecionado.PermiteAgendarEntrega;
}

function visibilidadeAgendamento(registroSelecionado) {
    return registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.AguardandoAgendamento;
}

function visibilidadeAssumirAgendamento() {
    return _CONFIGURACAO_TMS.PermitirAgendamentoPedidosSemCarga;
}

function visibilidadeInformarDadosAgendamento() {
    return _CONFIGURACAO_TMS.PermitirAgendamentoPedidosSemCarga;
}

function visibilidadeEnviarEmailAgendamento(registroSelecionado) {
    return registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.Finalizado;
}

function visibilidadeReagendar(registroSelecionado) {
    return !isPesquisarSomenteCargasFinalizadas() && ((registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.Agendado ||
        registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.AguardandoRetornoCliente ||
        registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.ReagendamentoSolicitado ||
        registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.AguardandoReagendamento ||
        registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.Reagendado) &&
        (!string.IsNullOrWhiteSpace(registroSelecionado.DataAgendamentoFormatada) || (registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.AguardandoReagendamento))
        && verificarPermissaoSugestaoDataEntrega(registroSelecionado));
}

function visibilidadeAguardandoRetornoCliente(registroSelecionado) {
    return !isPesquisarSomenteCargasFinalizadas() && (registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.AguardandoAgendamento || registroSelecionado.SituacaoAgendamento == EnumSituacaoAgendamentoEntregaPedido.ReagendamentoSolicitado) && !EnumSituacoesCarga.obterSituacoesCargaCanceladaEncerradaAnulada().includes(registroSelecionado.SituacaoViagem);
}

function isPesquisarSomenteCargasFinalizadas() {
    return _pesquisaAgendamentoEntregaPedido.StatusCarga.val() === 1;
}

function callbackColumnAgendamentoEntrega(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name == "IconeEmailEnviado")
        return obterHtmlColunaIconeEmailEnviado(dadosLinha);
}

function obterHtmlColunaIconeEmailEnviado(dadosLinha) {
    let cor = '#ff0000';
    let titulo = "Email não enviado";
    if (dadosLinha.DataeHoraEnvioEmailAgendamento) {
        cor = '#00cc00';
        titulo = "Email enviado " + dadosLinha.DataeHoraEnvioEmailAgendamento;
    }
    return '<div title="' + titulo + '"> <svg fill="' + cor + '" width="20px" height="20px" viewBox="0 0 1920 1920" xmlns="http://www.w3.org/2000/svg" stroke="#00cc00"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path d="M0 1694.235h1920V226H0v1468.235ZM112.941 376.664V338.94H1807.06v37.723L960 1111.233l-847.059-734.57ZM1807.06 526.198v950.513l-351.134-438.89-88.32 70.475 378.353 472.998H174.042l378.353-472.998-88.32-70.475-351.134 438.89V526.198L960 1260.768l847.059-734.57Z" fill-rule="evenodd"></path> </g></svg> </div>';
}