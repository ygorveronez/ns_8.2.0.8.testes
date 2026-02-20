/// <reference path="EtapasAgendamentoColeta.js" />
/// <reference path="DadosTransporte.js" />
/// <reference path="Agendamento.js" />
/// <reference path="NFe.js" />
/// <reference path="Emissao.js" />
/// <reference path="Lacre.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/PeriodoDescarregamentoSugerido.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaCarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaDescarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAgendamentoColeta.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../../js/Global/Globais.js" />

//#region Variaveis Globais

var _etapaCarga;
var _editarQuantidadePedido;
var _agendamentoColeta;
var _controlePedido;
var _pesquisaAgendamentoColeta;
var _gridAgendamentoColeta;
var _gridPedidosPendentes;
var _gridPedidos;
var _gridProdutos;
var _listaPedidos = [];
var _listaProdutos = [];
var dataAtual;
var _consultarTipoDeCarga;
var _minutosAdicionaisDaRota = 0;
//#endregion

//#region Pesquisa

var PesquisaAgendamentoColeta = function () {
    this.DataColeta = PropertyEntity({ val: ko.observable(""), required: true, getType: typesKnockout.date, text: "Data Coleta:", visible: ko.observable(true) });
    this.DataCriacao = PropertyEntity({ val: ko.observable(""), required: true, getType: typesKnockout.date, text: "Data Criação:", visible: ko.observable(false) });
    this.DataEntrega = PropertyEntity({ val: ko.observable(""), required: true, getType: typesKnockout.date, text: "Data Entrega:", visible: ko.observable(true) });
    this.DataAgendamento = PropertyEntity({ val: ko.observable(""), required: true, getType: typesKnockout.dateTime, text: "*Data Solicitada:", visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: ko.observable("Destinatário:"), idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "Remetente:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "Tipo de Carga:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ val: ko.observable(""), def: "", text: "Carga:" });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamento.Todas), options: EnumSituacaoCargaJanelaCarregamento.obterOpcoesPesquisa(), def: EnumSituacaoCargaJanelaCarregamento.Todas, text: "Situação Janela:", visible: ko.observable(true) });
    this.SituacaoJanelaDescarregamento = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaDescarregamento.Todas), options: EnumSituacaoCargaJanelaDescarregamento.obterOpcoesPesquisa(false), def: EnumSituacaoCargaJanelaDescarregamento.Todas, text: "Situação Janela Descarregamento:", visible: ko.observable(_CONFIGURACAO_TMS.ControlarAgendamentoSKU) });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumSituacoesCarga.Todas), options: EnumSituacoesCarga.obterOpcoesPesquisaSupervisor(), def: EnumSituacoesCarga.Todas, text: "Etapa Carga:", visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Recebedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Senha = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string, text: "Senha:", visible: _CONFIGURACAO_TMS.ControlarAgendamentoSKU, maxlength: 20 });
    this.Pedido = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string, text: "Pedido:", visible: _CONFIGURACAO_TMS.ControlarAgendamentoSKU, maxlength: 20 });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.PedidoEmbarcador = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string, text: "Nº Pedido Embarcador:", visible: !_CONFIGURACAO_TMS.ControlarAgendamentoSKU, maxlength: 20 });
    this.OcultarDescargaCancelada = PropertyEntity({ val: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true), def: false, text: "Ocultar agendamento com descarga cancelada", getType: typesKnockout.bool });

    this.Pesquisar = PropertyEntity({ eventClick: PesquisarAgendamentoColeta, type: types.event, text: "Pesquisar", visible: ko.observable(true), idGrid: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
};

//#endregion

//#region Mapeamento Knockout

var AgendamentoColeta = function () {
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0) });
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0) });
    this.CodigoAgendamento = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0) });
    this.Etapa = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0) });
    this.ApenasGerarPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, type: types.map });
    this.ExibirAlerta = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, type: types.map });
    this.ForcarEtapaNFe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, type: types.map });
};

var EtapaCarga = function () {
    var self = this;
    var tipoCampoData = _CONFIGURACAO_TMS.ControlarAgendamentoSKU ? typesKnockout.date : typesKnockout.dateTime;
    var exibirDataEntregaSugerida = _configuracaoAgendamentoColeta.SugerirDataEntregaAgendamentoColeta && !_CONFIGURACAO_TMS.ControlarAgendamentoSKU;

    dataAtual = moment().format((_CONFIGURACAO_TMS.ControlarAgendamentoSKU ? "DD/MM/YYYY" : "DD/MM/YYYY HH:mm"));

    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), text: "Número Carga:", visible: ko.observable(false) });
    this.DataCriacao = PropertyEntity({ val: ko.observable(""), text: "Data Criação:", visible: ko.observable(false) });
    this.PedidoEmbarcador = PropertyEntity({ val: ko.observable(""), text: "Pedido Embarcador:", visible: ko.observable(false) });

    this.DataColeta = PropertyEntity({ val: ko.observable(dataAtual), required: ko.observable(true), getType: tipoCampoData, text: "*Data Coleta:", enable: ko.observable(true), visible: ko.observable(true) });
    this.DataColeta.val.subscribe(DataColetaModificado);
    this.DataEntrega = PropertyEntity({ text: "*Data Entrega:", required: ko.observable(true), getType: tipoCampoData, enable: ko.observable(true), visible: ko.observable(!exibirDataEntregaSugerida) });
    this.DataEntregaSugerida = PropertyEntity({ text: "*Data Entrega:", val: self.DataEntrega.val, def: self.DataEntrega.def, required: ko.observable(true), getType: tipoCampoData, enable: ko.observable(true), visible: ko.observable(exibirDataEntregaSugerida), idBtnSearch: guid(), codEntity: ko.observable(0) });

    this.HorarioInicioFaixa = PropertyEntity({ val: ko.observable(""), required: ko.observable(false), getType: typesKnockout.time, text: "Hora Inicio Faixa:", enable: ko.observable(true), visible: ko.observable(false) });
    this.HorarioLimiteFaixa = PropertyEntity({ val: ko.observable(""), required: ko.observable(false), getType: typesKnockout.time, text: "Hora Limite Faixa:", enable: ko.observable(true), visible: ko.observable(false) });

    this.DataJanela = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string, text: "Data Janela: " });

    this.DataAgendamento = PropertyEntity({ val: ko.observable(""), required: ko.observable(false), getType: typesKnockout.dateTime, text: "*Data Solicitada:", enable: ko.observable(true), visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ val: ko.observable(""), required: ko.observable(false), getType: typesKnockout.text, text: "Observação:", enable: ko.observable(true), visible: ko.observable(true), maxlength: 300 });

    if (!_configuracaoAgendamentoColeta.SugerirDataEntregaAgendamentoColeta) {
        this.DataEntrega.dateRangeInit = this.DataColeta;
        this.DataColeta.dateRangeLimit = this.DataEntrega;
    }

    this.Recebedor = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Recebedor:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Filial = PropertyEntity({ codEntity: ko.observable(0), required: ko.observable(true), type: types.entity, text: "*Filial:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) });

    this.Remetente = PropertyEntity({ codEntity: ko.observable(0), required: ko.observable(false), type: types.entity, text: "*Remetente:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.Remetente.codEntity.subscribe(RemetenteModificado);

    this.Destinatario = PropertyEntity({ codEntity: ko.observable(0), required: ko.observable(true), type: types.entity, text: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor ? "*Destinatário/Tomador:" : "*Destinatário:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Destinatario.codEntity.subscribe(DestinatarioModificado);

    this.ModeloVeicular = PropertyEntity({
        codEntity: ko.observable(0), required: ko.observable(true), type: types.entity, text: ko.observable("*Modelo Veicular:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4")
    });
    this.ModeloVeicular.codEntity.subscribe(ModeloVeicularModificado);

    this.Volumes = PropertyEntity({ val: ko.observable(""), required: ko.observable(true), getType: typesKnockout.int, text: "*Volumes:", enable: ko.observable(true), visible: ko.observable(true) });
    this.Peso = PropertyEntity({ val: ko.observable(""), required: ko.observable(true), getType: typesKnockout.decimal, text: "*Peso:", enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ codEntity: ko.observable(0), required: ko.observable(true), type: types.entity, text: ko.observable("*Tipo de Carga:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.UnidadeMedida = PropertyEntity({ val: ko.observable(""), required: ko.observable(false), getType: typesKnockout.string, text: "*Unidade de Medida:", enable: ko.observable(true), visible: ko.observable(false), maxlength: 200, });
    this.TipoCarga.codEntity.subscribe(function (novoValor) {
        if (novoValor == 0) {
            self.Recebedor.visible(false);
            self.TransportadorManual.visible(_configuracaoAgendamentoColeta.ExibirOpcaoMultiModalAgendamentoColeta && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor);
            self.Transportador.visible(!_CONFIGURACAO_TMS.ControlarAgendamentoSKU && !(_configuracaoAgendamentoColeta.ExibirOpcaoMultiModalAgendamentoColeta && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe);
            self.Transportador.required(!(_configuracaoAgendamentoColeta.ExibirOpcaoMultiModalAgendamentoColeta && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe);
            self.Transportador.text(((_configuracaoAgendamentoColeta.ExibirOpcaoMultiModalAgendamentoColeta && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) ? "" : "*") + "Transportador:");
            self.ModeloVeicular.required(true);
            self.ModeloVeicular.text("*Modelo Veicular:");
        } else {
            TipoCargaModificado();
        }
    });

    this.Origem = PropertyEntity({ codEntity: ko.observable(0), type: types.entity }); // Campo apenas para filtrar tranportador a partir da localidade
    this.Transportador = PropertyEntity({ codEntity: ko.observable(0), required: ko.observable(true), type: types.entity, text: ko.observable("*Transportador:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TransportadorManual = PropertyEntity({ required: ko.observable(false), text: "Transportador:", visible: ko.observable(false), enable: ko.observable(true), maxlength: 200, getType: typesKnockout.string });
    this.EmailSolicitante = PropertyEntity({ required: ko.observable(!_CONFIGURACAO_TMS.ControlarAgendamentoSKU), text: "*E-mail Solicitante:", visible: ko.observable(!_CONFIGURACAO_TMS.ControlarAgendamentoSKU), enable: ko.observable(true), maxlength: 100, getType: typesKnockout.email });
    this.CargaPerigosa = PropertyEntity({ val: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true), def: false, text: "Carga Perigosa", getType: typesKnockout.bool, type: types.map });
    this.CDDestino = PropertyEntity({ codEntity: ko.observable(0), required: ko.observable(false), type: types.entity, text: ko.observable("CD de Destino:"), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true), });

    this.Pedidos = PropertyEntity({ type: types.listEntity, list: new Array(), visibleFade: ko.observable(false), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.PedidosPendentes = PropertyEntity({ type: types.listEntity, list: new Array(), visibleFade: ko.observable(false), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.InformarOutroTipoDeCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Informar outro tipo de carga para o agendamento", visible: ko.observable(false) });

    this.Reboque = PropertyEntity({ required: ko.observable(false), text: ko.observable("Reboque:"), visible: ko.observable(false), enable: ko.observable(true), maxlength: 20, getType: typesKnockout.string });
    this.Placa = PropertyEntity({ required: ko.observable(false), text: ko.observable("Placa:"), visible: ko.observable(false), enable: ko.observable(true), maxlength: 20, getType: typesKnockout.string });
    this.Motorista = PropertyEntity({ required: ko.observable(false), text: ko.observable("Motorista:"), visible: ko.observable(false), enable: ko.observable(true), maxlength: 100, getType: typesKnockout.string });
    this.TipoOperacao = PropertyEntity({ codEntity: ko.observable(0), required: ko.observable(false), type: types.entity, text: ko.observable("*Tipo de Operação:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), InformarDadosNotaCte: ko.observable(false) });
    this.PortoOrigem = PropertyEntity({ codEntity: ko.observable(0), required: ko.observable(false), type: types.entity, text: ko.observable("Porto de Origem:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.PortoDestino = PropertyEntity({ codEntity: ko.observable(0), required: ko.observable(false), type: types.entity, text: ko.observable("Porto de Destino:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Categoria = PropertyEntity({ codEntity: ko.observable(0), required: ko.observable(false), type: types.entity, text: ko.observable("Categoria:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });


    this.InformarOutroTipoDeCarga.val.subscribe(function (novoValor) {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor)
            return;

        if (!_CONFIGURACAO_TMS.ControlarAgendamentoSKU)
            return;

        self.TipoCarga.val("");
        self.TipoCarga.codEntity(0);

        if (novoValor) {
            self.TipoCarga.visible(true);
            self.TipoCarga.required(true);
            self.TipoCarga.text("*Tipo De Carga:");
        }
        else {
            self.TipoCarga.visible(false);
            self.TipoCarga.required(false);
            self.TipoCarga.text("Tipo De Carga:");
        }
    });

    this.AtualizarTransportador = PropertyEntity({ eventClick: AtualizarTransportadorClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarCargaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.AtualizarDadosTransporteEtapaCarga = PropertyEntity({ eventClick: atualizarInformacoesTransporteClick, type: types.event, text: "Atualizar Informações Transporte", visible: ko.observable(false) });
    this.Anexos = PropertyEntity({ eventClick: AnexosClick, type: types.event, text: "Anexos", visible: ko.observable(VisibilidadeAnexos()) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(false) });

    this.NumeroPedidoFiltro = PropertyEntity({ val: ko.observable(""), getType: types.string, text: "Número Pedido:", enable: ko.observable(true), visible: ko.observable(true), maxlength: 400 });
    this.DestinatarioFiltro = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Destinatário:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PesquisarPedidosPendentes = PropertyEntity({ eventClick: PesquisarPedidosPendentesClick, type: types.event, text: "Pesquisar", visible: ko.observable(true) });
    this.LimparPedidosPendentes = PropertyEntity({ eventClick: LimparPedidosPendentesClick, type: types.event, text: "Excluir todos os pedidos" });
    this.ImportarPedidoPendente = PropertyEntity({
        type: types.local,
        text: "Importar pedidos pendentes...",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: "AgendamentoColeta/ImportarPedidosPendentes",
        UrlConfiguracao: "AgendamentoColeta/ConfiguracaoImportarPedidosPendentes",
        CodigoControleImportacao: EnumCodigoControleImportacao.O033_AgendamentoPedidosPendentes,
        FecharModalSeSucesso: true,
        OcultarMensagemSeSucesso: true,
        CallbackImportacao: function (arg) {
            AdicioanarPedidosPendentesImportados(arg)
        }
    });
};

var EditarQuantidadePedido = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0) });
    this.QuantidadeEnviar = PropertyEntity({ val: ko.observable(""), required: true, getType: typesKnockout.int, text: "*Qtd. de Caixas para enviar:", enable: ko.observable(true), visible: ko.observable(true) });
    this.SKU = PropertyEntity({ val: ko.observable(""), required: true, getType: typesKnockout.int, text: "*Qtd. de Itens:", enable: ko.observable(true), visible: ko.observable(true) });
    this.QtProdutos = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.int, text: "*Qtd. de Produtos:", enable: ko.observable(false), visible: ko.observable(false) });
    this.SaldoRestante = PropertyEntity({ val: ko.observable(""), required: true, getType: typesKnockout.int, text: "", enable: ko.observable(true), visible: ko.observable(true) });
    this.Salvar = PropertyEntity({ eventClick: SalvarQtdPedidoClick, type: types.event, text: "Salvar", visible: ko.observable(true) });

    //this.QuantidadeEnviar.val.subscribe(ValidarQuantidadeEnviar);
    //this.SKU.val.subscribe(ValidarQuantidadeEnviar);
}

var AlterarProdutosPedido = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0) });
    this.Produtos = PropertyEntity({ type: types.listEntity, list: new Array(), visibleFade: ko.observable(false), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.Salvar = PropertyEntity({ eventClick: SalvarProdutos, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

var ControlePedido = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0) });
    this.Filial = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0) });
    this.DataInicio = PropertyEntity({ getType: typesKnockout.dateTime, visible: false, val: ko.observable("") });
    this.DataFim = PropertyEntity({ getType: typesKnockout.dateTime, visible: false, val: ko.observable("") });
}

function LoadAgendamentoColeta() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            dataAtual = moment().format((_CONFIGURACAO_TMS.ControlarAgendamentoSKU ? "DD/MM/YYYY" : "DD/MM/YYYY HH:mm"));

            _agendamentoColeta = new AgendamentoColeta();

            _controlePedido = new ControlePedido();

            HeaderAuditoria("AgendamentoColeta", _agendamentoColeta);

            _etapaCarga = new EtapaCarga();
            KoBindings(_etapaCarga, "knockoutCargaAgendamentoColeta");
            PreencherRemetentePadrao();

            _pesquisaAgendamentoColeta = new PesquisaAgendamentoColeta();
            KoBindings(_pesquisaAgendamentoColeta, "knockoutPesquisaAgendamentoColeta");

            _editarQuantidadePedido = new EditarQuantidadePedido();
            KoBindings(_editarQuantidadePedido, "knockoutEditarQuantidadePedido");

            _alterarProdutosPedido = new AlterarProdutosPedido();
            KoBindings(_alterarProdutosPedido, "knoutAlterarProdutosPedido");

            _consultarTipoDeCarga = new BuscarTiposDeCargaPorDestinatario(_etapaCarga.TipoCarga, retornoTipoDeCarga, _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor ? _controlePedido.Codigo : null);
            new BuscarModelosVeicularesCarga(_etapaCarga.ModeloVeicular, null, null, null, null, null, null, null, null, null, null, _controlePedido.Codigo);
            new BuscarTransportadores(_etapaCarga.Transportador, null, null, null, null, null, _etapaCarga.Origem);
            new BuscarCentroDistribuicao(_etapaCarga.CDDestino);

            new BuscarCategoria(_etapaCarga.Categoria, null, null, null, null);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) {
                new BuscarClientes(_etapaCarga.Destinatario, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
                new BuscarClientes(_etapaCarga.DestinatarioFiltro, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
            }
            else {
                new BuscarClientes(_etapaCarga.Destinatario);
                new BuscarClientes(_etapaCarga.DestinatarioFiltro);
            }

            new BuscarPorto(_etapaCarga.PortoDestino);
            new BuscarPorto(_etapaCarga.PortoOrigem);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
                new BuscarTiposOperacao(_etapaCarga.TipoOperacao, retornoTipoOperacao);
            } else {
                new BuscarTiposOperacao(_etapaCarga.TipoOperacao);
            }

            new BuscarFilial(_etapaCarga.Filial);
            BuscarClientesFactory(_etapaCarga.Remetente, CallBackRetornoCliente, { filtrarGrupoFornecedor: true });
            new BuscarClientes(_etapaCarga.Recebedor);
            new BuscarPeriodoDescarregamentoSugerido(_etapaCarga.DataEntregaSugerida, retornoConsultaPeriodoDescarregamentoSugerido, _etapaCarga.Destinatario);

            new BuscarTiposdeCarga(_pesquisaAgendamentoColeta.TipoCarga);
            new BuscarClientes(_pesquisaAgendamentoColeta.Destinatario);
            BuscarClientesFactory(_pesquisaAgendamentoColeta.Remetente, null, { filtrarGrupoFornecedor: true });
            new BuscarClientes(_pesquisaAgendamentoColeta.Recebedor);
            new BuscarTiposOperacao(_pesquisaAgendamentoColeta.TipoOperacao);

            LoadGridAgendamentoColeta();
            LoadDadosTransporte();
            LoadDadosAgendamento();
            LoadEtapaLacre();
            LoadEmissao();
            LoadEtapaNFe();
            LoadEtapasAgendamentoColeta();
            LoadCamposAgendamentoColeta();
            LoadGridPedidosPendentes();
            LoadGridPedidos();
            LoadGridProdutos();
            LoadDocumentoParaTransporte();

            VerificarSePossuiEtapaInformarNotaCte(_etapaCarga.TipoOperacao.InformarDadosNotaCte());

            ControlarVisibilidadeCamposPorTipoServico();

            $("#" + _etapaCarga.Pedidos.idGrid).droppable({
                drop: function (event, ui) {
                    var id = parseInt(ui.draggable[0].id);
                    DroppablePedido(id);
                },
                hoverClass: "ui-state-active"
            });

            _agendamentoColeta.ApenasGerarPedido.val.subscribe(function (v) {
                _dadosTransporte.ApenasGerarPedido.val(v);
                _aceiteTransporte.ApenasGerarPedido.val(v);
                _mensagemEtapaAgendamento.ApenasGerarPedido.val(v);
                _emissao.ApenasGerarPedido.val(v);
                _NFeAgendamento.ApenasGerarPedido.val(v);
                _lacreAgendamento.ApenasGerarPedido.val(v);
            });

            obterConfiguracoesTelaAgendamentoColeta();
        });
    });
}

function LoadGridAgendamentoColeta() {
    var opcaoInformacoes = {
        descricao: "Carregar",
        id: guid(),
        evento: "onclick",
        metodo: CarregarClick,
        tamanho: "5",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoInformacoes]
    };

    var configExportacao = {
        url: "AgendamentoColeta/ExportarPesquisa",
        titulo: "Agendamento Entrega"
    };

    _gridAgendamentoColeta = new GridViewExportacao(_pesquisaAgendamentoColeta.Pesquisar.idGrid, "AgendamentoColeta/Pesquisar", _pesquisaAgendamentoColeta, menuOpcoes, configExportacao);
    _gridAgendamentoColeta.CarregarGrid();
}

function LoadGridPedidos() {
    _listaPedidos = new Array();
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: ExcluirPedidoAgendamentoColetaClick, icone: "", visibilidade: true };
    var opcaoEditar = { descricao: "Alterar quantidade", id: guid(), metodo: EditarPedidoAgendamentoColetaClick, icone: "", visibilidade: alteraQuantidadePorCentrodeDescarregamento };
    var opcaoAlterarProdutos = { descricao: "Alterar Produtos", id: guid(), metodo: AlterarProdutosPedidoAgentamentoColetaClick, icone: "", visibilidade: alteraProdutosPorCentrodeDescarregamento };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 7, opcoes: [opcaoExcluir, opcaoEditar, opcaoAlterarProdutos] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Saldo", visible: false },
        { data: "TipoCargaCodigo", visible: false },
        { data: "NumeroPedidoEmbarcador", title: "Pedido", width: "15%" },
        { data: "DescricaoFilial", title: "Descrição Filial", width: "15%" },
        { data: "TipoCarga", title: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) ? "Modalidade" : "Tipo de Carga", width: "15%" },
        { data: "VolumesEnviar", title: "Qtd. de Caixas", width: "15%" },
        { data: "SKU", title: "Qtd. de Itens", width: "10%" },
        { data: "QtProdutos", title: "Qtd. Produtos", width: "20%" },
        { data: "DataInicioJanelaDescarga", title: "Data Início Janela", width: "15%" },
        { data: "DataFimJanelaDescarga", title: "Data Fim Janela", width: "15%" },
        { data: "CNPJRemetente", title: "CNPJ do remetente", width: "15%" },
        { data: "CodigoIntegracaoProduto", title: "Códigos dos Produtos", width: "20%", visible: false },
        { data: "DescricaoProduto", title: "Descrições dos Produtos", width: "20%", visible: false }
    ];
    _gridPedidos = new BasicDataTable(_etapaCarga.Pedidos.idGrid, header, menuOpcoes, null, null, 10);

    _gridPedidos.SetPermitirEdicaoColunas(true);
    //_gridPedidos.SetSalvarPreferenciasGrid(true);
    //_gridPedidos.SetHabilitarModelosGrid(true);
    //_gridPedidos.SetHabilitarScrollHorizontal(true, 200);

    RecarregarGridPedidos();
}


var editable = {
    editable: true,
    type: EnumTipoColunaEditavelGrid.int,
    numberMask: ConfigInt()
};

var editarColuna = {
    permite: true,
    atualizarRow: true,
    callback: linhaAlterada,
};
function linhaAlterada(registro) {

    if (registro.Quantidade == registro.QuantidadeOriginal)
        registro.DT_RowColor = "";
    else
        registro.DT_RowColor = "#fffce8";

    RecarregarGridProdutos();
}
function linhaAlteradaVermelho(registro) {
    registro.DT_RowColor = "#ebc3c3";
    for (var i = 0; i < _listaProdutos.length; i++) {
        if (_listaProdutos[i].Codigo == registro.Codigo) {
            _listaProdutos[i].Removido = true;
            break;
        }
    }
    RecarregarGridProdutos();
}

function linhaAlteradaBranco(registro) {
    registro.DT_RowColor = "";
    for (var i = 0; i < _listaProdutos.length; i++) {
        if (_listaProdutos[i].Codigo == registro.Codigo) {
            _listaProdutos[i].Removido = false;
            break;
        }
    }
    RecarregarGridProdutos();
}

function LoadGridProdutos() {
    _listaProdutos = new Array();
    var opcaoExcluir = { descricao: "Remover", id: guid(), metodo: ExcluirProdutoAgendamentoColetaClick, icone: "", visibilidade: isNotRemovido };
    var opcaoAdicionar = { descricao: "Adicionar", id: guid(), metodo: AdicionarProdutoAgendamentoColetaClick, icone: "", visibilidade: isRemovido };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 5, opcoes: [opcaoExcluir, opcaoAdicionar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoProduto", visible: false },
        { data: "Removido", visible: false },
        { data: "Descricao", title: "Produto", width: "30%" },
        { data: "CodigoEmbarcador", title: "Código Produto", width: "15%" },
        { data: "Setor", title: "Setor (Grupo)", width: "20%" },
        { data: "Quantidade", title: "Quantidade", width: "10%", editableCell: editable },
        { data: "QuantidadeOriginal", title: "Quantidade Original", width: "10%" }
    ];
    _gridProdutos = new BasicDataTable(_alterarProdutosPedido.Produtos.idGrid, header, menuOpcoes, null, null, 10, null, null, editarColuna);
    RecarregarGridProdutos();
}

function isRemovido(registro) {
    return registro.Removido == true;
}

function isNotRemovido(registro) {
    return (!(registro.Removido == true));
}

function RecarregarGridPedidos() {
    var data = new Array();
    $.each(_listaPedidos, function (i, pedido) {
        pedido.DT_RowColor = "#e6f5ea";
        data.push(pedido);
    });
    _gridPedidos.CarregarGrid(data);
}

function RecarregarGridProdutos() {
    var data = new Array();
    $.each(_listaProdutos, function (i, produto) {
        data.push(produto);
    });
    _gridProdutos.CarregarGrid(data);
}

function LoadGridPedidosPendentes() {

    var configExportacao = {
        url: "Pedido/ExportarPesquisaPendentes",
        titulo: "Pedidos Pendentes"
    };

    _gridPedidosPendentes = new GridView(_etapaCarga.PedidosPendentes.idGrid, "Pedido/PesquisaGridPendentes", _etapaCarga, null, null, 10, null, null, true, null, null, null, configExportacao, null, null, callbackRowGridPedidosPendentes);
    _gridPedidosPendentes.CarregarGrid();
}

function RemetenteModificado(codigo) {
    if (codigo == 0) {
        _etapaCarga.Origem.codEntity(0);
        _etapaCarga.Origem.val("");
    }
    BuscarDataDeEntregaPorTempoDeDescargaDaRota();
}

function DestinatarioModificado() {
    BuscarDataDeEntregaPorTempoDeDescargaDaRota();
}

function TipoCargaModificado() {
    BuscarDataDeEntregaPorTempoDeDescargaDaRota();
}

function ModeloVeicularModificado() {
    BuscarDataDeEntregaPorTempoDeDescargaDaRota();
}

function DataColetaModificado(dataColeta) {
    _etapaCarga.DataEntrega.minDate(dataColeta);

    if (dataColeta != null && dataColeta.length > 15)
        BuscarDataDeEntregaPorTempoDeDescargaDaRota();
}

function PreencherRemetentePadrao() {
    if (_configuracaoAgendamentoColeta.OrigemPadrao > 0) {
        _etapaCarga.Origem.codEntity(_configuracaoAgendamentoColeta.OrigemPadrao);
        _etapaCarga.Origem.val(_configuracaoAgendamentoColeta.OrigemPadrao);
    }
}

function CallBackRetornoCliente(data) {
    _etapaCarga.Remetente.codEntity(data.Codigo);
    _etapaCarga.Remetente.val(data.Descricao);
    _etapaCarga.Origem.codEntity(data.CodigoLocalidade);
    _etapaCarga.Origem.val(data.Localidade);
    _configuracaoAgendamentoColeta.GerarAgendamentoPedidosExistentes = data.GerarAgendamentoPedidosExistentes;
}

function LoadCamposAgendamentoColeta() {
    let dataAtual = moment().format((_CONFIGURACAO_TMS.ControlarAgendamentoSKU ? "DD/MM/YYYY" : "DD/MM/YYYY HH:mm"));

    if (_configuracaoAgendamentoColeta.CompartilharAcessoEntreGrupoPessoas || _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor) {
        _pesquisaAgendamentoColeta.Remetente.visible(true);
        _etapaCarga.Remetente.visible(true);
        _etapaCarga.Remetente.required(true);
    }

    if (_configuracaoAgendamentoColeta.UtilizarParametrizacaoDeHorarios) {
        _etapaCarga.HorarioInicioFaixa.visible(true);
        _etapaCarga.HorarioLimiteFaixa.visible(true);
    }

    if (_CONFIGURACAO_TMS.ControlarAgendamentoSKU) {
        _pesquisaAgendamentoColeta.DataAgendamento.visible(true);
        _pesquisaAgendamentoColeta.Situacao.visible(false);
        _pesquisaAgendamentoColeta.DataColeta.visible(false);
        _pesquisaAgendamentoColeta.DataEntrega.visible(false);
        _pesquisaAgendamentoColeta.Recebedor.visible(false);
        _etapaCarga.DataColeta.visible(false);
        _etapaCarga.DataAgendamento.visible(true);
        _etapaCarga.DataAgendamento.enable(false);
        _etapaCarga.DataAgendamento.required = true;
        _etapaCarga.DataColeta.required = false;
        _etapaCarga.DataEntrega.visible(false);
        _etapaCarga.DataEntrega.required = false;
        _etapaCarga.DataEntregaSugerida.visible(false);
        _etapaCarga.DataEntregaSugerida.required = false;
        _etapaCarga.TipoCarga.visible(!(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor));
        _etapaCarga.Destinatario.visible(false);
        _etapaCarga.Destinatario.required = false;
        _etapaCarga.Volumes.visible(false);
        _etapaCarga.Recebedor.visible(false);
        _etapaCarga.Volumes.required = false;
        _etapaCarga.Peso.visible(false);
        _etapaCarga.Peso.required = false;
        _etapaCarga.Transportador.visible(false);
        _etapaCarga.Transportador.required(false);
        _etapaCarga.Filial.visible(false);
        _etapaCarga.Filial.required = false;
        _etapaCarga.CargaPerigosa.visible(false);
        if (_configuracaoAgendamentoColeta.CompartilharAcessoEntreGrupoPessoas || _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor)
            _etapaCarga.ModeloVeicular.cssClass("col col-xs-12 col-sm-12 col-md-5 col-lg-5");
        else
            _etapaCarga.ModeloVeicular.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
        _etapaCarga.TipoCarga.cssClass("col col-xs-12 col-sm-12 col-md-5 col-lg-5");
        _etapaCarga.TipoCarga.required(false);
        _etapaCarga.TipoCarga.text("Tipo De Carga:");
        _CRUDDadosTransporte.InformarNotasFiscais.visible(false);
        $("#Etapa2Padrao").hide();
        $("#Etapa2SKU").show();
    }
    else {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor) {
            _etapaCarga.Transportador.text("*Transportador:");
            _etapaCarga.Transportador.required(true);
        }

        $("#Etapa2Padrao").show();
        $("#Etapa2SKU").hide();
        $("#divPedidosPendentes").hide();
        $("#divPedidos").hide();
    }

    if (_configuracaoAgendamentoColeta.ExibirOpcaoMultiModalAgendamentoColeta && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) {
        _etapaCarga.DataCriacao.val(dataAtual);
        _etapaCarga.DataCriacao.visible(true);
    }

    if (_configuracaoAgendamentoColeta.SugerirDataEntregaAgendamentoColeta && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) {
        _pesquisaAgendamentoColeta.DataColeta.visible(false);
        _pesquisaAgendamentoColeta.DataCriacao.visible(true);
    }
}

//#endregion

//#region Funções Click

function SalvarQtdPedidoClick(recarregarGrid = true) {
    if (!ValidarCamposObrigatorios(_editarQuantidadePedido)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Preencha os campos obrigatórios.");
        return;
    }
    if (!VerificarSaldoDisponivel()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "O Saldo informado é maior do que o disponível.");
        return;
    }

    for (var i = 0; i < _listaPedidos.length; i++) {
        if (_listaPedidos[i].Codigo == _editarQuantidadePedido.Codigo.val()) {
            _listaPedidos[i].VolumesEnviar = _editarQuantidadePedido.QuantidadeEnviar.val();
            _listaPedidos[i].SKU = _editarQuantidadePedido.SKU.val();
            _listaPedidos[i].QtProdutos = _editarQuantidadePedido.QtProdutos.val();
            break;
        }
    }

    if (recarregarGrid) RecarregarGridPedidos();

    Global.fecharModal("modalEditarPedido");
}

function SalvarProdutos() {
    for (var i = 0; i < _listaProdutos.length; i++) {
        _listaProdutos[i].DT_RowColor = "";
    }
    var dados = {
        Codigo: _alterarProdutosPedido.Codigo.val(),
        Produtos: JSON.stringify(_listaProdutos)
    };
    executarReST("AgendamentoColeta/AlterarProdutos", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false && arg.Data != null) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);

                for (var i = 0; i < _listaPedidos.length; i++) {
                    if (_listaPedidos[i].Codigo == _alterarProdutosPedido.Codigo.val()) {
                        _listaPedidos[i].VolumesEnviar = arg.Data.QuantidadeEnviar;
                        _listaPedidos[i].SKU = arg.Data.SKU;
                        _listaPedidos[i].QtProdutos = arg.Data.QuantidadeProdutos;
                        break;
                    }
                }

                _listaProdutos = arg.Data.ListaProdutos;
                RecarregarGridProdutos();
                RecarregarGridPedidos();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
    RecarregarGridPedidos();
    RecarregarGridProdutos();

}

function ExcluirPedidoAgendamentoColetaClick(registroSelecionado) {
    for (var i = 0; i < _listaPedidos.length; i++) {
        if (_listaPedidos[i].Codigo == registroSelecionado.Codigo) {
            _listaPedidos[i].VolumesEnviar = _listaPedidos[i].Saldo;
            _listaPedidos.splice(i, 1);
        }
    }

    if (_listaPedidos.length == 0) {
        LimparCampos(_controlePedido);
        _etapaCarga.DataAgendamento.val("");
        _etapaCarga.DataAgendamento.enable(false);
        _etapaCarga.InformarOutroTipoDeCarga.visible(false);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) {
            _etapaCarga.TipoCarga.val("");
            _etapaCarga.TipoCarga.codEntity(0);
        }
    }
    else {
        configurarDatasPermitidas();
    }

    RecarregarGridPedidos();
    _gridPedidosPendentes.setarCorGridPorID(registroSelecionado.Codigo, "#FFFFFF");
}

function ExcluirProdutoAgendamentoColetaClick(registroSelecionado) {
    registroSelecionado.QuantidadeAgendada = 0;
    linhaAlteradaVermelho(registroSelecionado)
}

function AdicionarProdutoAgendamentoColetaClick(registroSelecionado) {
    registroSelecionado.Quantidade = registroSelecionado.QuantidadeOriginal;
    linhaAlteradaBranco(registroSelecionado)
}

function EditarPedidoAgendamentoColetaClick(registroSelecionado) {
    _editarQuantidadePedido.SaldoRestante.val(registroSelecionado.Saldo);
    _editarQuantidadePedido.Codigo.val(registroSelecionado.Codigo);
    _editarQuantidadePedido.QuantidadeEnviar.val(registroSelecionado.VolumesEnviar);
    _editarQuantidadePedido.SKU.val(registroSelecionado.SKU);

    Global.abrirModal('modalEditarPedido');

    $("#modalEditarPedido").one('hidden.bs.modal', function () {
        LimparCampos(_editarQuantidadePedido);
    });
}


function AlterarProdutosPedidoAgentamentoColetaClick(registroSelecionado) {
    _alterarProdutosPedido.Codigo.val(registroSelecionado.Codigo);
    var dados = {
        Pedido: registroSelecionado.Codigo,
    };
    executarReST("PedidoProduto/BuscarPorPedidoProdutoAlterado", dados, function (arg) {
        if (arg.Data != null && arg.Data != false) {
            _listaProdutos = arg.Data;
            RecarregarGridProdutos();
        }
    }, null);
    Global.abrirModal('modalAlterarProdutosPedido');

    $("#modalAlterarProdutosPedido").one('hidden.bs.modal', function () {
        LimparCampos(_alterarProdutosPedido);
    });
}

function PesquisarPedidosPendentesClick() {
    _gridPedidosPendentes.CarregarGrid();
}

function LimparPedidosPendentesClick() {
    for (var i = 0; i < _listaPedidos.length; i++) {
        _listaPedidos[i].VolumesEnviar = _listaPedidos[i].Saldo;
        _gridPedidosPendentes.setarCorGridPorID(_listaPedidos[i].Codigo, "#FFFFFF");
    }
    _listaPedidos = [];
    _controlePedido.DataFim.val("")
    _controlePedido.DataInicio.val("")
    RecarregarGridPedidos();
}

function AtualizarTransportadorClick() {
    var dados = {
        Codigo: _agendamentoColeta.CodigoAgendamento.val(),
        Transportador: _etapaCarga.Transportador.codEntity(),
    };

    if (dados.Transportador == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe um transportador.");
        return;
    }

    executarReST("AgendamentoColeta/AtualizarTransportador", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Transportador atualizado.");
                LimparTodosCampos();
                BuscarAgendamento(arg.Data, function () {
                    setTimeout(FocarEtapaCargaAguardando, 500);
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function atualizarInformacoesTransporteClick() {
    var dados = {
        Codigo: _agendamentoColeta.CodigoAgendamento.val(),
        TransportadorManual: _etapaCarga.TransportadorManual.val(),
        Reboque: _etapaCarga.Reboque.val(),
        Motorista: _etapaCarga.Motorista.val(),
        Placa: _etapaCarga.Placa.val()
    };

    executarReST("AgendamentoColeta/AtualizarInformacoesTransporte", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function AdicionarCargaClick(e, sender) {
    if (!ValidarCamposObrigatorios(_etapaCarga)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    if (!_CONFIGURACAO_TMS.ControlarAgendamentoSKU && !ValidarEmail(_etapaCarga.EmailSolicitante.val())) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "O e-mail informado está em formato incorreto.");
        return;
    }

    var infoEtapaCarga = RetornarObjetoPesquisa(_etapaCarga);
    infoEtapaCarga["Pedidos"] = JSON.stringify(_listaPedidos);
    infoEtapaCarga["Anexo"] = JSON.stringify(obterAnexos());

    AdicionarAgendamentoColeta(infoEtapaCarga);
}

function AnexosClick() {
    _agendamentoColetaListaAnexos.Adicionar.visible(!(_agendamentoColeta.CodigoAgendamento.val() > 0));

    Global.abrirModal('divModalAnexoAgendamentoColeta');
    $("#divModalAnexoAgendamentoColeta").one("hidden.bs.modal", function () {
        Global.fecharModal("divModalAdicionarAnexoAgendamentoColeta");
    });
}

function CarregarClick(registroSelecionado) {
    ExibirFiltrosClick(_pesquisaAgendamentoColeta);
    ControlarVisibilidadeCamposPorTipoServico();
    BuscarAgendamento(registroSelecionado);
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function LimparClick() {
    SetarEtapasRequisicao();
    LimparTodosCampos();
    ControlarVisibilidadeCamposPorTipoServico();
}

function CancelarClick() {
    exibirConfirmacao("Confirmação", "Deseja cancelar o agendamento de coleta selecionado?",
        function () {
            ConfirmarCancelamento();
        });
}

function ImprimirClick() {
    executarDownload("AgendamentoColeta/Imprimir", { Codigo: _agendamentoColeta.CodigoAgendamento.val() });
}

//#endregion

//#region Funções Globais

function BuscarAgendamento(agendamento, callback) {
    LimparTodosCampos(false);
    executarReST("AgendamentoColeta/BuscarPorCodigo", {
        Codigo: agendamento.Codigo
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_agendamentoColeta, { Data: arg.Data.ControleAgendamento });
                PreencherObjetoKnout(_etapaCarga, { Data: arg.Data.Carga });
                PreencherObjetoKnout(_aceiteTransporte, { Data: arg.Data.AceiteTransporte });
                PreencherObjetoKnout(_dadosTransporte, { Data: arg.Data.DadosTransporte });
                PreencherObjetoKnout(_retornoAgendamento, { Data: arg.Data.RetornoAgendamento });
                PreencherObjetoKnout(_dadosAgendamento, { Data: arg.Data.DadosAgendamento });
                preencherAnexo(arg.Data.ListaAnexos);
                _gridNFe.CarregarGrid();
                _gridMDFe.CarregarGrid();
                SetarEtapasRequisicao(_agendamentoColeta.Etapa.val(), arg.Data.ControleAgendamento.RemoverEtapaAgendamentoDoAgendamentoColeta);
                _dadosTransporte.Codigo.val(_agendamentoColeta.CodigoCarga.val());
                _listaPedidos = arg.Data.ListaPedidos;
                RecarregarGridPedidos();
                VerificarSituacao(arg.Data.RetornoAgendamento.SituacaoCodigo);
                _gridPedidos.DesabilitarOpcoes();
                _etapaCarga.TipoOperacao.InformarDadosNotaCte(arg.Data.Carga.InformarDadosNotaCte);

                if (arg.Data.Carga.Recebedor)
                    _etapaCarga.Recebedor.visible(true);

                _emissao.Dropzone.visible(arg.Data.Carga.ObrigatorioInformarCTes);

                if (_gridNFeAgendamento)
                    _gridNFeAgendamento.CarregarGrid();

                if (!_agendamentoColeta.ApenasGerarPedido.val()) {
                    LoadGridLacreAgendamento();
                }

                if (_etapaCarga.TipoCarga.codEntity()) {
                    if (arg.Data.Carga.ExigirQueCDDestinoSejaInformadoAgendamento) {
                        _etapaCarga.CDDestino.visible(true);
                        _etapaCarga.CDDestino.required(true);
                    }
                }

                controlarBotoes(arg.Data);
                VerificarVisibilidadeEtapaNFe();
                ControlarVisibilidadeCamposPorTipoServico();

                VerificarSePossuiEtapaInformarNotaCte(arg.Data.Carga.InformarDadosNotaCte, arg.Data.RetornoAgendamento.Situacao);

                RecarregarGridDocumentoTransporte(arg.Data.DocumentosParaTrasporte);
                if (callback) callback();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function PesquisarAgendamentoColeta() {
    _gridAgendamentoColeta.CarregarGrid();
}

//#endregion

//#region Funções Privadas

function ConfirmarCancelamento() {
    executarReST("AgendamentoColeta/Cancelar", { Codigo: _agendamentoColeta.CodigoAgendamento.val(), ExibirAlerta: _agendamentoColeta.ExibirAlerta.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.ExibirAlerta) {
                    _agendamentoColeta.ExibirAlerta.val(true);
                    exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                        ConfirmarCancelamento();
                    }, function () {
                        _agendamentoColeta.ExibirAlerta.val(false);
                    });
                }
                else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "O cancelamento do agendamento foi solicitado.");
                    RecarregarGridAgendamentoColeta();
                    LimparTodosCampos();
                    SetarEtapasRequisicao();
                    ControlarVisibilidadeCamposPorTipoServico();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null)
}

function AdicionarAgendamentoColeta(infoEtapaCarga) {
    executarReST("AgendamentoColeta/Adicionar", infoEtapaCarga, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (!arg.Data.ViolacaoUniqueKey) {
                    enviarArquivosAnexados(arg.Data.Codigo);
                    exibirMensagem(tipoMensagem.ok, "Sucesso", _CONFIGURACAO_TMS.ControlarAgendamentoSKU ? "Carga adicionada." : "Agendamento adicionado.");
                    RecarregarGridAgendamentoColeta();
                    LimparTodosCampos();
                    ControlarVisibilidadeCamposPorTipoServico();
                    BuscarAgendamento(arg.Data, function () {
                        setTimeout(FocarEtapaCargaAguardando, 500);
                    });
                }
                else {
                    exibirConfirmacao("Atenção", "Ocorreu um problema na geração da carga. Deseja tentar novamente?",
                        function () {
                            var infoEtapaCarga = RetornarObjetoPesquisa(_etapaCarga);
                            infoEtapaCarga["Pedidos"] = JSON.stringify(_listaPedidos);
                            infoEtapaCarga["Anexo"] = JSON.stringify(obterAnexos());
                            AdicionarAgendamentoColeta(infoEtapaCarga);
                        });
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function retornoTipoDeCarga(data) {
    _etapaCarga.TipoCarga.codEntity(data.Codigo);
    _etapaCarga.TipoCarga.val(data.Descricao);

    if (!_CONFIGURACAO_TMS.ControlarAgendamentoSKU) {
        _etapaCarga.Transportador.visible(!data.FretePorContaDoCliente && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe);
        _etapaCarga.Transportador.required(!data.FretePorContaDoCliente && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe);
        _etapaCarga.Transportador.text((data.FretePorContaDoCliente ? "" : "*") + "Transportador:");
        _etapaCarga.TransportadorManual.visible(data.FretePorContaDoCliente);
    }

    _etapaCarga.Recebedor.visible(data.PermiteInformarRecebedor && !_CONFIGURACAO_TMS.ControlarAgendamentoSKU);

    if (_etapaCarga.TipoCarga.codEntity()) {
        if (data.ExigirQueCDDestinoSejaInformadoAgendamento) {
            _etapaCarga.CDDestino.visible(true);
            _etapaCarga.CDDestino.required(true);
            _etapaCarga.CDDestino.text("*CD de Destino:");
        } else {
            _etapaCarga.CDDestino.visible(false);
            _etapaCarga.CDDestino.required(false);
        }
    }

    executarReST("AgendamentoColeta/ObterDetalhesTipoCarga", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var dados = r.Data;
                _etapaCarga.ModeloVeicular.required(!dados.NaoObrigarInformarModeloVeicularAgendamento);
                _etapaCarga.ModeloVeicular.text((dados.NaoObrigarInformarModeloVeicularAgendamento ? "" : "*") + "Modelo Veicular:");
                _etapaCarga.Transportador.required(!dados.NaoObrigarInformarTransportadorAgendamento && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe);
                _etapaCarga.Transportador.text(((dados.NaoObrigarInformarTransportadorAgendamento && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) ? "" : "*") + "Transportador:");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function LiberarAtualizacaoTransportador() {
    _etapaCarga.Transportador.enable(true);
    _etapaCarga.AtualizarTransportador.visible(true);
}

function VerificarSaldoDisponivel() {
    if (parseInt(_editarQuantidadePedido.QuantidadeEnviar.val().toString().replace(/\D/g, '')) > parseInt(_editarQuantidadePedido.SaldoRestante.val().toString().replace(/\D/g, '')))
        return false;

    return true;
}

function VisibilidadeAnexos() {
    return (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor && !_CONFIGURACAO_TMS.ControlarAgendamentoSKU);
}

function DroppablePedido(idPedido) {
    setTimeout(function () {
        var dataRow = _gridPedidosPendentes.obterDataRow(idPedido);

        if (dataRow)
            AdicionarPedido(dataRow.data, true);
    }, 50);
}

function AdicionarPedido(pedido, recarregarGrid, importado = false) {
    if (!VerificarPedidoExistencia(pedido)) {
        if (!importado)
            exibirMensagem(tipoMensagem.atencao, "Atenção", "O Pedido selecionado já está na lista.");
        return;
    }

    if (!VerificarDestinatarioPedido(pedido)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Todos os pedidos precisam ter os mesmos destinatários.");
        return;
    }

    if (!VerificarFilialPedido(pedido)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Todos os pedidos precisam ser da mesma filial.");
        return;
    }

    if (!ValidarDatasPedidoDataMenor(pedido)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "A data de fim de janela do pedido selecionado é menor que a menor data de inicio de janela dos outros pedidos.");
        return;
    }

    if (!ValidarDatasPedidoDataMaior(pedido)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "A data de inicio da janela do pedido selecionado é maior que a menor data de fim de janela dos pedidos já adicionados.");
        return;
    }

    _gridPedidosPendentes.setarCorGridPorID(pedido.Codigo, "#AED6F1");
    _listaPedidos.push(pedido);

    _etapaCarga.InformarOutroTipoDeCarga.visible(_CONFIGURACAO_TMS.ControlarAgendamentoSKU && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor);

    _controlePedido.Codigo.val(pedido.CodigoDestinatario);
    _controlePedido.Filial.val(pedido.CodigoFilial);

    configurarDatasPermitidas();

    if (recarregarGrid) RecarregarGridPedidos();
}

function VerificarPedidoExistencia(pedido) {
    for (var i = 0; i < _listaPedidos.length; i++) {
        if (_listaPedidos[i].Codigo == pedido.Codigo)
            return false;
    }

    return true;
}

function VerificarDestinatarioPedido(pedido) {
    if (_controlePedido.Codigo.val() != pedido.CodigoDestinatario && _listaPedidos.length > 0)
        return false;

    return true;
}

function VerificarFilialPedido(pedido) {
    if (_controlePedido.Filial.val() != pedido.CodigoFilial && _listaPedidos.length > 0)
        return false;

    return true;
}


function ValidarDatasPedidoDataMenor(pedido) {
    var dataPedido = moment(pedido.DataFimJanelaDescarga, "D/M/YYYY HH:mm");
    var dataControleInicio = moment(_controlePedido.DataInicio.val(), "D/M/YYYY HH:mm");
    var diferencaEntreDatas = dataPedido.diff(dataControleInicio);
    var duracao = moment.duration(diferencaEntreDatas);
    var duracaoEmSegundos = duracao.asSeconds();

    if (_controlePedido.DataInicio.val() != "" && duracaoEmSegundos < 0)
        return false;

    return true;
}

function ValidarDatasPedidoDataMaior(pedido) {
    var dataPedido = moment(pedido.DataInicioJanelaDescarga, "D/M/YYYY HH:mm");
    var dataControleFim = moment(_controlePedido.DataFim.val(), "D/M/YYYY HH:mm");
    var diferencaEntreDatas = dataPedido.diff(dataControleFim);
    var duracao = moment.duration(diferencaEntreDatas);
    var duracaoEmSegundos = duracao.asSeconds();

    if (_controlePedido.DataFim.val() != "" && duracaoEmSegundos > 0)
        return false;

    return true;
}

function ValidarQuantidadeEnviar() {
    _editarQuantidadePedido.QuantidadeEnviar.val(_editarQuantidadePedido.QuantidadeEnviar.val().replace(/\D/g, ""));
    _editarQuantidadePedido.SKU.val(_editarQuantidadePedido.SKU.val().replace(/\D/g, ""));
}

function LimparTodosCampos(recarregarGrid = true) {
    dataAtual = moment().format((_CONFIGURACAO_TMS.ControlarAgendamentoSKU ? "DD/MM/YYYY" : "DD/MM/YYYY HH:mm"));

    LimparCampos(_etapaCarga);
    LimparCampos(_dadosTransporte);
    LimparCampos(_aceiteTransporte);
    LimparCampos(_agendamentoColeta);
    LimparCampos(_controlePedido);
    LimparCampos(_dadosAgendamento);
    LimparCampos(_lacreAgendamento);
    LimparCampos(_retornoAgendamento);
    LimparCampos(_documentoParaTransporte);
    limparAnexo();
    LimparUploadNFe();
    _gridPedidos.HabilitarOpcoes();

    PreencherRemetentePadrao();

    _etapaCarga.NumeroCarga.visible(false);
    _etapaCarga.DataCriacao.visible(false);
    _etapaCarga.AtualizarDadosTransporteEtapaCarga.visible(false);
    _etapaCarga.PedidoEmbarcador.visible(false);
    _etapaCarga.DataAgendamento.enable(false);
    _etapaCarga.Recebedor.visible(false);
    _etapaCarga.CDDestino.visible(false);
    _etapaCarga.Transportador.visible(!_CONFIGURACAO_TMS.ControlarAgendamentoSKU && !(_configuracaoAgendamentoColeta.ExibirOpcaoMultiModalAgendamentoColeta && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor));
    _etapaCarga.TransportadorManual.visible(_configuracaoAgendamentoColeta.ExibirOpcaoMultiModalAgendamentoColeta && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor);

    _listaPedidos = [];
    _listaProdutos = [];

    if (recarregarGrid) {
        RecarregarGridPedidos();
        _gridPedidosPendentes.CarregarGrid();
    }
    _etapaCarga.AtualizarTransportador.visible(false);
    VisibilidadeBotaoImprimir(false);

    if (_configuracaoAgendamentoColeta.ExibirOpcaoMultiModalAgendamentoColeta && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) {
        _etapaCarga.DataCriacao.val(dataAtual);
        _etapaCarga.DataCriacao.visible(true);

        if (_operadorLogistica.ClienteFornecedor > 0) {
            _etapaCarga.Remetente.codEntity(_operadorLogistica.ClienteFornecedor);
            _etapaCarga.Remetente.val(_operadorLogistica.NomeClienteFornecedor);
        }
    }
}

function RecarregarGridAgendamentoColeta() {
    _gridAgendamentoColeta.CarregarGrid();
}

function VisibilidadeBotaoImprimir(visible) {
    _etapaCarga.Imprimir.visible(visible);
    _CRUDDadosTransporte.Imprimir.visible(visible);
    _CRUDEtapaNFe.Imprimir.visible(visible);
    _emissao.Imprimir.visible(visible);
}

function controlarBotoes(dados) {
    if (_configuracaoAgendamentoColeta.ExibirOpcaoMultiModalAgendamentoColeta) {
        _etapaCarga.TransportadorManual.visible(true);
        _etapaCarga.Transportador.visible(false);

        _etapaCarga.TransportadorManual.enable(dados.Carga.PermiteInformarDadosTransporteEtapaCarga);
        _etapaCarga.Motorista.enable(dados.Carga.PermiteInformarDadosTransporteEtapaCarga);
        _etapaCarga.Reboque.enable(dados.Carga.PermiteInformarDadosTransporteEtapaCarga);
        _etapaCarga.Placa.enable(dados.Carga.PermiteInformarDadosTransporteEtapaCarga);
        _etapaCarga.AtualizarDadosTransporteEtapaCarga.visible(dados.Carga.PermiteInformarDadosTransporteEtapaCarga);

        if ((dados.Carga.Transportador == null || dados.Carga.Transportador.Codigo == 0) && !_agendamentoColeta.ApenasGerarPedido.val())
            LiberarAtualizacaoTransportador();
    }
    else if (!_CONFIGURACAO_TMS.ControlarAgendamentoSKU) {
        _etapaCarga.Transportador.visible(string.IsNullOrWhiteSpace(dados.Carga.TransportadorManual));
        _etapaCarga.TransportadorManual.visible(!string.IsNullOrWhiteSpace(dados.Carga.TransportadorManual));
    }

    if (!_CONFIGURACAO_TMS.ControlarAgendamentoSKU) {
        _etapaCarga.NumeroCarga.visible(true);
        _etapaCarga.DataCriacao.visible(true);
        _etapaCarga.PedidoEmbarcador.visible(true);
    }
    else {
        _CRUDDadosTransporte.InformarNotasFiscais.visible(false);
        VisibilidadeBotaoImprimir(true);
    }
}

function VerificarSituacao(codigoSituacao) {
    if (EnumSituacaoAgendamentoColeta.isCancelado(codigoSituacao)) {
        SetarEnableCamposKnockout(_etapaCarga, false);
        SetarEnableCamposKnockout(_dadosTransporte, false);
        SetarEnableCamposKnockout(_dadosAgendamento, false);

        _etapaCarga.Cancelar.visible(false);
        _CRUDDadosTransporte.Cancelar.visible(false);
        _CRUDEtapaNFe.Cancelar.visible(false);
        _etapaCarga.Adicionar.visible(false);
        _CRUDDadosTransporte.InformarNotasFiscais.visible(false);
        _dadosTransporte.Atualizar.visible(false);
        _CRUDEtapaNFe.Encaminhar.visible(false);
    }
}

function ObterEmPedidos(codigo) {
    for (var i = 0; i < _listaPedidos.length; i++) {

        if (_listaPedidos[i].Codigo == codigo)
            return true;
    }

    return false;
}

function callbackRowGridPedidosPendentes(row, data, c) {
    if (ObterEmPedidos(data.Codigo)) {
        data.DT_RowColor = "#AED6F1";
        setarCorDataRow(row, data);
    }
}

function configurarDatasPermitidas() {
    _etapaCarga.DataAgendamento.val("");
    _etapaCarga.DataAgendamento.minDate("");
    _etapaCarga.DataAgendamento.maxDate("");

    executarReST("AgendamentoColeta/BuscarDatasPermitidasAgendamento", { Pedidos: JSON.stringify(_listaPedidos) }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _controlePedido.DataInicio.val(retorno.Data.DataInicial);
                _controlePedido.DataFim.val(retorno.Data.DataFinal);

                _etapaCarga.DataAgendamento.minDate(_controlePedido.DataInicio.val());
                _etapaCarga.DataAgendamento.maxDate(_controlePedido.DataFim.val());

                if (Global.ObterDiasEntreDatas(Global.DataAtual(), _controlePedido.DataInicio.val()) < 0)
                    _etapaCarga.DataAgendamento.minDate(Global.DataAtual());

                _etapaCarga.DataAgendamento.enable(true);
            } else {
                _etapaCarga.DataAgendamento.enable(false);
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else {
            _etapaCarga.DataAgendamento.enable(false);
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function AdicioanarPedidosPendentesImportados(arg) {
    if (arg.Success && arg.Data != null) {
        var total = arg.Data.Retornolinhas.length;
        for (var i = 0; i < total; i++) {
            if (arg.Data.Retornolinhas[i].processou && arg.Data.Retornolinhas[i].codigo > 0 && arg.Data.Retornolinhas[i].pedido != null) {
                AdicionarPedido(arg.Data.Retornolinhas[i].pedido, false, true);
                _editarQuantidadePedido.SaldoRestante.val(arg.Data.Retornolinhas[i].pedido.Saldo);
                _editarQuantidadePedido.Codigo.val(arg.Data.Retornolinhas[i].pedido.Codigo);
                _editarQuantidadePedido.QuantidadeEnviar.val(arg.Data.Retornolinhas[i].VolumeEnviar);
                _editarQuantidadePedido.SKU.val(arg.Data.Retornolinhas[i].SKU);
                _editarQuantidadePedido.QtProdutos.val(arg.Data.Retornolinhas[i].QuantidadeProdutos);
                SalvarQtdPedidoClick(false);
            }
        }
        RecarregarGridPedidos();
    }
}

function retornoConsultaPeriodoDescarregamentoSugerido(registroSelecionado) {
    _etapaCarga.DataEntregaSugerida.val(registroSelecionado.DataDescarregamento);
}

function ControlarVisibilidadeCamposPorTipoServico() {
    if (_configuracaoAgendamentoColeta.ExibirOpcaoMultiModalAgendamentoColeta && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) {
        _pesquisaAgendamentoColeta.TipoCarga.visible(false);
        _pesquisaAgendamentoColeta.Recebedor.visible(false);
        _pesquisaAgendamentoColeta.Destinatario.text("Terminal de Entrega:");

        _etapaCarga.TipoCarga.visible(false);
        _etapaCarga.TipoCarga.required(false);
        _etapaCarga.EmailSolicitante.visible(false);
        _etapaCarga.EmailSolicitante.required(false);
        _etapaCarga.DataColeta.required(false);
        _etapaCarga.DataColeta.visible(false);
        _etapaCarga.Remetente.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
        _etapaCarga.Transportador.visible(false);

        _etapaCarga.Reboque.visible(true);
        _etapaCarga.Placa.visible(true);
        _etapaCarga.Motorista.visible(true);
        _etapaCarga.TipoOperacao.visible(true);
        _etapaCarga.TipoOperacao.required(true);
        _etapaCarga.PortoOrigem.visible(true);
        _etapaCarga.TransportadorManual.visible(true);
        _etapaCarga.PortoDestino.visible(true);

        _etapaCarga.DataCriacao.val(dataAtual);
        _etapaCarga.DataCriacao.visible(true);

        $("#" + _etapaCarga.Reboque.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });
        $("#" + _etapaCarga.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

        if (_operadorLogistica.ClienteFornecedor > 0) {
            _etapaCarga.Remetente.codEntity(_operadorLogistica.ClienteFornecedor);
            _etapaCarga.Remetente.val(_operadorLogistica.NomeClienteFornecedor);
        }
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        //Etapa da Carga
        if (!_configuracaoAgendamentoColeta.PermitirTransportadorCadastrarAgendamentoColeta) {
            SetarEnableCamposKnockout(_etapaCarga, false);
            _etapaCarga.Adicionar.visible(false);
            _etapaCarga.AtualizarTransportador.visible(false);
            _etapaCarga.AtualizarDadosTransporteEtapaCarga.visible(false);
            _etapaCarga.Cancelar.visible(false);
            _etapaCarga.Imprimir.visible(false);
            _etapaCarga.Limpar.visible(true);
        } else {
            _etapaCarga.TipoOperacao.visible(true);
            _etapaCarga.TipoOperacao.required(true);
            _etapaCarga.Transportador.visible(false);
            _etapaCarga.Transportador.required(false);
            _etapaCarga.Adicionar.visible(true);
            _etapaCarga.AtualizarTransportador.visible(false);
            _etapaCarga.AtualizarDadosTransporteEtapaCarga.visible(false);
            _etapaCarga.Cancelar.visible(false);
            _etapaCarga.Imprimir.visible(false);
            _etapaCarga.Limpar.visible(true);
        }

        //Etapa de Transporte 
        _CRUDDadosTransporte.Cancelar.visible(false);
        _CRUDDadosTransporte.Imprimir.visible(false);
        _CRUDDadosTransporte.InformarNotasFiscais.visible(_configuracaoAgendamentoColeta.PermitirTransportadorCadastrarAgendamentoColeta);
        _CRUDDadosTransporte.Limpar.visible(true);

        //Lacre
        SetarEnableCamposKnockout(_lacreAgendamento, false);

        //Etapa NFe
        _CRUDEtapaNFe.Encaminhar.visible(false);
        _CRUDEtapaNFe.Cancelar.visible(false);
        _CRUDEtapaNFe.Imprimir.visible(false);

        //Etapa Emissao
        _emissao.Download.visible(false);
        _emissao.Imprimir.visible(false);
        _emissao.Dropzone.visible(false);
    }
}

function retornoTipoOperacao(data) {
    _etapaCarga.TipoOperacao.codEntity(data.Codigo);
    _etapaCarga.TipoOperacao.val(data.Descricao);
    _etapaCarga.TipoOperacao.InformarDadosNotaCte(data.InformarDadosNotaCte);

    if (data.TipoDeCargaPadraoOperacao != undefined && data.TipoDeCargaPadraoOperacao != null && data.TipoDeCargaPadraoOperacao > 0) {
        _etapaCarga.TipoCarga.codEntity(data.TipoDeCargaPadraoOperacao);

        _consultarTipoDeCarga.CarregarTipoDeCargaPorCodigo(data.TipoDeCargaPadraoOperacao);
    }
    VerificarSePossuiEtapaInformarNotaCte(data.InformarDadosNotaCte);
}

function obterConfiguracoesTelaAgendamentoColeta() {
    executarReST("AgendamentoColeta/ObterConfiguracoesTelaAgendamentoColeta", null, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
                    _etapaCarga.TipoOperacao.required(arg.Data.MostrarTipoDeOperacaoNoPortalMultiEmbarcadorAgendamentoColeta);
                    _etapaCarga.TipoOperacao.visible(arg.Data.MostrarTipoDeOperacaoNoPortalMultiEmbarcadorAgendamentoColeta);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}
function alteraQuantidadePorCentrodeDescarregamento(registroSelecionado) {
    return registroSelecionado.UsarLayoutAgendamentoPorCaixaItem;
}

function alteraProdutosPorCentrodeDescarregamento(registroSelecionado) {
    return !registroSelecionado.UsarLayoutAgendamentoPorCaixaItem;
}

function BuscarDataDeEntregaPorTempoDeDescargaDaRota() {
    var fornecedor = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor;
    _minutosAdicionaisDaRota = _configuracaoAgendamentoColeta.TempoPadraoDeDescargaMinutos;

    _etapaCarga.DataEntrega.enable(true);

    if (_configuracaoAgendamentoColeta.CalcularDataDeEntregaPorTempoDeDescargaDaRota &&
        (_etapaCarga.Remetente.codEntity() || fornecedor) &&
        _etapaCarga.Destinatario.codEntity()) {

        var codRemetente = _etapaCarga.Remetente.codEntity();
        if (fornecedor && _operadorLogistica.ClienteFornecedor > 0)
            codRemetente = _operadorLogistica.ClienteFornecedor;

        var dados = {
            DataCarregamento: _etapaCarga.DataColeta.val(),
            Remetente: codRemetente,
            Destinatario: _etapaCarga.Destinatario.codEntity(),
            ModeloVeicularCarga: _etapaCarga.ModeloVeicular.codEntity(),
            TipoDeCarga: _etapaCarga.TipoCarga.codEntity()
        };

        executarReST("AgendamentoColeta/BuscarTempoDeDescargaDaRota", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _minutosAdicionaisDaRota = arg.Data.MinutosAdicionaisDaRota;
                    if (arg.Data.MinutosAdicionaisDaRota > 0)
                        _etapaCarga.DataEntrega.enable(false);

                    if (!arg.Data.Sucesso)
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.Mensagem);
                    else
                        ajustaDataEntrega(arg.Data.DataEntrega, false)
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    }
}

function ajustaDataEntrega(novaData, somaMinutos) {
    if (novaData) {
        let novaDataEntrega = Global.criarData(novaData);
        if (somaMinutos)
            novaDataEntrega.setMinutes(novaDataEntrega.getMinutes() + _minutosAdicionaisDaRota);
        _etapaCarga.DataEntrega.val(novaDataEntrega);
        _etapaCarga.DataEntrega.minDate(novaDataEntrega);
    }
}

//#endregion