/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _configuracaoWidget;
var camposConfiguracaoWidget = [
    'ExibirNomeMotorista',
    'ExibirVersaoAplicativo',
    'ExibirNivelBateria',
    'ExibirSinal',
    'ExibirNumeroCarga',
    'ExibirValorTotalProdutos',
    'ExibirPrevisaoProximaParada',
    'ExibirDistanciaRota',
    'ExibirTempoRota',
    'ExibirEntregaColetasRealizadas',
    'ExibirPesoRestanteEntrega',
    'ExibirNumeroPedidoCliente',
    'ExibirPrimeiroSegundoTrecho',
    'ExibirFilial',
    'ExibirAnalistaResponsavelMonitoramento',
    'ExibirTelefoneCelular',
    'ExibirPrevisaoRecalculada',
    'ExibirExpedidor',
    'ExibirNumeroOrdemPedido',
    'ExibirNumeroPedido',
    'ExibirTransportador',
    'ExibirTipoOperacao',
    'ExibirPesoBruto',
    'ExibirPesoLiquido',
    'ExibirTendenciaEntrega',
    'ExibirTendenciaColeta',
    'ExibirModalTransporte',
    'ExibirCanalVenda',
    'ExibirMesorregiao',
    'ExibirRegiao',
];
var estadoInicialConfiguracaoWidget = {};
var estadoInicialConfiguracaoWidgetDetalhesEntrega = {};
var dadosConfiguracaoWidget = null
/*
 * Declaração das Classes
 */

var ConfiguracaoWidget = function () {
    this.Habilitar = PropertyEntity({ val: ko.observable(true) });

    // Motorista
    this.ExibirNomeMotorista = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NomeDoMotorista, val: ko.observable(false) });
    this.ExibirVersaoAplicativo = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.VersaoDoAplicativo, val: ko.observable(false) });
    this.ExibirNivelBateria = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NivelDaBateria, val: ko.observable(false) });
    this.ExibirSinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Sinal, val: ko.observable(false) });
    this.ExibirTelefoneCelular = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TelefoneCelular, val: ko.observable(false) });

    // Carga
    this.ExibirNumeroCarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroDaCarga, val: ko.observable(false) });
    this.ExibirProximoCliente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ProximoCliente, val: ko.observable(false) });
    this.ExibirValorTotalProdutos = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ValorTotalProdutos, val: ko.observable(false) });
    this.ExibirNumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroPedidoCliente, val: ko.observable(false) });
    this.ExibirPrimeiroSegundoTrecho = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrimeiroOuSegundoTrecho, val: ko.observable(false) });
    this.ExibirFilial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Filial, val: ko.observable(false) });
    this.ExibirExpedidor = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Expedidor, val: ko.observable(false) });
    this.ExibirNumeroPedido = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroPedidoEmbarcador, val: ko.observable(false) });
    this.ExibirNumeroOrdemPedido = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroOrdem, val: ko.observable(false) });
    this.ExibirTransportador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Transportador, val: ko.observable(false) });
    this.ExibirTipoOperacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TipoDeOperacao, val: ko.observable(false) });
    this.ExibirPesoBruto = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirPesoBruto, val: ko.observable(false) });
    this.ExibirPesoLiquido = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirPesoLiquido, val: ko.observable(false) });
    this.ExibirTendenciaEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirTendenciaAtraso, val: ko.observable(false) });
    this.ExibirTendenciaColeta = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirTendenciaAtrasoColeta, val: ko.observable(false) });
    this.ExibirCanalVenda = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirCanalVenda, val: ko.observable(false) });
    this.ExibirModalTransporte = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirModalTransporte, val: ko.observable(false) });
    this.ExibirMesorregiao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirMesorregiao, val: ko.observable(false) });
    this.ExibirRegiao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirRegiao, val: ko.observable(false) });

    // Monitoramento
    this.ExibirPrevisaoProximaParada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrevisaoProximaParada, val: ko.observable(false) });
    this.ExibirDistanciaRota = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DistanciaRota, val: ko.observable(false) });
    this.ExibirTempoRota = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TempoDeRota, val: ko.observable(false) });
    this.ExibirEntregaColetasRealizadas = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.EntregaColetasRealizadas, val: ko.observable(false) });
    this.ExibirPesoRestanteEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PesoRestanteDaEntrega, val: ko.observable(false) });
    this.ExibirAnalistaResponsavelMonitoramento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.AnalistaResponsavelPeloMonitoramento, val: ko.observable(false) });
    this.ExibirPrevisaoRecalculada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrevisaoRecalculada, val: ko.observable(false) });

    this.LimiteWidgetAtivos = PropertyEntity({ val: ko.observable(0) });
    this.ConfiguracaoExibicaoDetalhesEntrega = PropertyEntity({ val: ko.observable("") });
};

var ConfiguracaoExibicaoDetalhesEntregaCliente = function () {
    this.NomeRecebedor = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Cliente, val: ko.observable(true) });
    this.EtapaStage = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Etapa, val: ko.observable(false), visible: ko.observable(false) });
    this.TelefoneCliente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Telefone, val: ko.observable(true) });
    this.DocumentoRecebedor = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CPFCNPJ, val: ko.observable(true) });
    this.EnderecoCliente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Endereco, val: ko.observable(true) });
    this.LocalidadeCliente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.LocalidadeCliente, val: ko.observable(true) });
    this.Mesoregiao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Mesoregiao, val: ko.observable(false) });
    this.Regiao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Regiao, val: ko.observable(false) });
    this.Localizacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Localizacao, val: ko.observable(true) });
    this.Email = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Email, val: ko.observable(true) });

    this.JanelaDescarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.JanelaDeDescarga, val: ko.observable(false) });
    this.DataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataPrevisaoDaEntrega, val: ko.observable(true) });

    this.CodigoSap = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CodigoSAP, val: ko.observable(false) });
    this.InicioViagemPrevista = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrevisaoInicioViagemInicial, val: ko.observable(true) });
    this.InicioViagemRealizada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.InicioViagemRealizada, val: ko.observable(true) });
    //this.DataEntregaReprogramada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataEntregaReprogramada, val: ko.observable(true) }); esse cara só tem na entrega. nao pode ter no cliente
    this.CodigoIntegracaoCliente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CodigoIntegracaoCliente, val: ko.observable(true) });
    this.CodigoIntegracaoFilial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CodigoIntegracaoFilial, val: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ text: "Selecionar todos", val: ko.observable(false) });

    this.SelecionarTodos.val.subscribe(function (val) {
        selecionarTodosExibicoesDetalhesEntrega(_configuracaoExibicaoDetalhesEntregaCliente, val);
    });

};

var ConfiguracaoExibicaoDetalhesEntregaEntregaColeta = function () {
    this.EntregaNoPrazo = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.EntregaNoPrazo, def: true, val: ko.observable(true) });
    this.OrdemEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ordem, def: true, val: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao, def: true, val: ko.observable(true) });
    this.DistanciaOrigemXEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DistanciaOrigemXEntrega, def: true, val: ko.observable(true) });
    this.LocalidadeEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.LocalidadeEntrega, def: true, val: ko.observable(true) });
    this.Pedidos = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Pedidos, def: true, val: ko.observable(true) });
    this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroPedidoCliente, def: true, val: ko.observable(true) });
    this.NotasFiscais = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NotasFiscais, def: true, val: ko.observable(true) });
    this.QuantidadePacotesColetados = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PacotesColetados, val: ko.observable(false) });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Peso, def: true, def: true, val: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Observacoes, def: true, val: ko.observable(true) });
    this.NumeroCTe = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroCTe, val: ko.observable(false) });
    this.NumeroChamado = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Atendimentos, val: ko.observable(false) });
    this.TempoRestanteChamado = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TempoRestanteChamado, val: ko.observable(false) });
    this.ObservacoesAgendamento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ObservacoesAgendamento, val: ko.observable(false) });
    this.DataProgramadaColeta = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataProgramadaDaColeta, val: ko.observable(false) });
    this.TempoProgramadaColeta = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TempoProgramadaDaEntrega, val: ko.observable(false) });
    this.DataProgramadaDescarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataProgramadaDaDescarga, val: ko.observable(false) });
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioEntrega, def: true, val: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataFimEntrega, def: true, val: ko.observable(true) });
    this.DataConfirmacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataConfirmacaoEntrega, def: true, val: ko.observable(true) });
    this.DataConfirmacaoApp = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataDeConfirmacaoApp, val: ko.observable(false) });
    this.DataRejeitado = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataDeRejeicao, val: ko.observable(false) });
    this.ResponsavelFinalizacaoManual = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.OperadorBaixaManual, val: ko.observable(false) });
    this.QuantidadePlanejada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.QuantidadePlanejada, val: ko.observable(false) });
    this.QuantidadeTotal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.QuantidadeColetada, val: ko.observable(false) });
    this.InfoMotivoRejeicao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.MotivoRejeicao, val: ko.observable(false) });
    this.InfoMotivoRetificacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.MotivoDaRetificacao, val: ko.observable(false) });
    this.DataReentregaMesmaCarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataReentregaMesmaCarga, val: ko.observable(false) });
    this.DataEntregaNota = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataLimiteParaEntrega, val: ko.observable(false) });
    this.StatusEntregaNota = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.StatusEntregaNota, def: true, val: ko.observable(true) });
    this.LeadTimeTransportador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.LeadTimeTransportador, val: ko.observable(false) });
    this.ObservacoesPedidos = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ObservacoesPedidos, val: ko.observable(false) });
    this.Assinatura = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Assinatura, val: ko.observable(false) });
    this.AlterarDataAgendamentoEntregaTransportador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.AlterarDataAgendamentoEntregaTransportador, val: ko.observable(true) });
    this.DataAgendamentoDeEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataAgendamentoDeEntrega, def: true, val: ko.observable(true) });
    this.DataAgendamentoEntregaTransportador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataAgendamentoEntregaTransportador, val: ko.observable(true) });
    this.DataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntrega, def: true, val: ko.observable(true) });
    this.FilialVenda = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.FilialVenda, val: ko.observable(false) });
    this.DataEntregaReprogramada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataEntregaReprogramada, def: true, val: ko.observable(true) });
    this.DataPrevisaoEntregaTransportador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntregaTransportador, val: ko.observable(false) });
    this.OrigemSituacaoDataAgendamentoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.OrigemDataDeAgendamentoDaEntrega, val: ko.observable(false) });
    this.JustificativaOnTime = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Observacao, val: ko.observable(false) });
    this.OnTime = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.OnTime, val: ko.observable(false) });
    this.DataInicioCarregamentoOuDescarregamento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioCarregamentoOuDescarregamento, val: ko.observable(false) });
    this.DataTerminoCarregamentoOuDescarregamento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataTerminoCarregamentoOuDescarregamento, val: ko.observable(false) });
    this.TempoCarregamentoOuDescarregamento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TempoCarregamentoOuDescarregamento, val: ko.observable(false) });
    this.Parqueada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Parqueada, val: ko.observable(false) });
    this.DataRejeicaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataRejeicaoEntrega, val: ko.observable(false) });
    this.StatusTendenciaEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TendenciaEntregaColeta, val: ko.observable(false) });
    this.DataPrevisaoEntregaAjustada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntregaAjustada, def: true, val: ko.observable(true) });
    this.DataEmissaoNota = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataEmissaoNota, val: ko.observable(true) });
    this.DataEntradaRaio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataChegada, val: ko.observable(true) });
    this.DataSaidaRaio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataSaida, val: ko.observable(false) });
    this.DataPrevisaoSaida = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataPrevisaoSaida, val: ko.observable(false) });
    this.DataConfirmacaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataConfirmacaoEntregaUsuario, val: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ text: "Selecionar todos", val: ko.observable(false) });
    this.SelecionarTodos.val.subscribe(function (val) {
        selecionarTodosExibicoesDetalhesEntrega(_configuracaoExibicaoDetalhesEntregaEntregaColeta, val);
    });
};

var ConfiguracaoExibicaoDetalhesEntregaCadeiaAjuda = function () {
    this.GerenteNacional = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.GerenteNacional, val: ko.observable(false) });
    this.GerenteRegional = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.GerenteRegional, val: ko.observable(false) });
    this.Gerente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Gerente, val: ko.observable(false) });
    this.Supervisor = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Supervisor, val: ko.observable(false) });
    this.Vendedor = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Vendedor, val: ko.observable(false) });
    this.EscritorioVenda = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.EscritorioVendas, def: true, val: ko.observable(true) });
    this.EquipeVendas = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EquipeVendas, def: true, val: ko.observable(true) });
    this.TipoMercadoria = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TipoMercadoria, val: ko.observable(false) });
    this.CanalVenda = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CanalVenda, def: true, val: ko.observable(true) });
    this.MostrarNomeCadeiaAjuda = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Nome, val: ko.observable(false) });
    this.MostrarEmailCadeiaAjuda = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Email, val: ko.observable(false) });
    this.MostrarTelefoneCadeiaAjuda = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Telefone, val: ko.observable(false) });
    this.MostrarWhatsAppCadeiaAjuda = PropertyEntity({ text: "WhatsApp", val: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ text: "Selecionar todos", val: ko.observable(false) });
    this.SelecionarTodosInformacoes = PropertyEntity({ text: "Selecionar todas as informações", val: ko.observable(false) });
    this.Matriz = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Matriz, def: false, val: ko.observable(false) });
    this.SelecionarTodos.val.subscribe(function (val) {
        selecionarTodosExibicoesDetalhesEntrega(_configuracaoExibicaoDetalhesEntregaCadeiaAjuda, val);
    });
    this.SelecionarTodosInformacoes.val.subscribe(function (val) {
        _configuracaoExibicaoDetalhesEntregaCadeiaAjuda.MostrarNomeCadeiaAjuda.val(val);
        _configuracaoExibicaoDetalhesEntregaCadeiaAjuda.MostrarEmailCadeiaAjuda.val(val);
        _configuracaoExibicaoDetalhesEntregaCadeiaAjuda.MostrarTelefoneCadeiaAjuda.val(val);
        _configuracaoExibicaoDetalhesEntregaCadeiaAjuda.MostrarWhatsAppCadeiaAjuda.val(val);
    });
};

var SalvarConfiguracoesExibicoesDetalhesEntrega = function () {
    this.SalvarDetalhes = PropertyEntity({ eventClick: function () { salvarConfiguracoesExibicaoDetalhesEntrega(); fecharAccordions('#tabDetalhesEntregas'); }, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, enable: ko.observable(false) });
    this.SalvarWidget = PropertyEntity({ eventClick: salvarConfiguracaoWidgetClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, enable: ko.observable(false) });
    this.VoltarAoPadrao = PropertyEntity({ eventClick: restaurarPadraoConfiguracoesExibicaoDetalhesEntrega, type: types.event, text: "Voltar ao padrão" });
};

/*
 * Declaração das Funções de Inicialização
 */
function loadConfiguracaoWidget(cb) {
    _configuracaoWidget = new ConfiguracaoWidget();
    KoBindings(_configuracaoWidget, "knoutConfiguracaoWidget");

    _configuracaoExibicaoDetalhesEntregaCliente = new ConfiguracaoExibicaoDetalhesEntregaCliente();
    KoBindings(_configuracaoExibicaoDetalhesEntregaCliente, "knockoutConfiguracaoExibicaoDetalhesEntregaCliente");

    _configuracaoExibicaoDetalhesEntregaEntregaColeta = new ConfiguracaoExibicaoDetalhesEntregaEntregaColeta();
    KoBindings(_configuracaoExibicaoDetalhesEntregaEntregaColeta, "knockoutConfiguracaoExibicaoDetalhesEntregaEntregaColeta");

    _configuracaoExibicaoDetalhesEntregaCadeiaAjuda = new ConfiguracaoExibicaoDetalhesEntregaCadeiaAjuda();
    KoBindings(_configuracaoExibicaoDetalhesEntregaCadeiaAjuda, "knockoutConfiguracaoExibicaoDetalhesEntregaCadeiaAjuda");

    _salvarConfiguracoesExibicoesDetalhesEntrega = new SalvarConfiguracoesExibicoesDetalhesEntrega();
    KoBindings(_salvarConfiguracoesExibicoesDetalhesEntrega, "knockoutSalvarConfiguracoesExibicoesDetalhesEntrega");

    _configuracaoWidget.LimiteWidgetAtivos.val(_CONFIGURACAO_TMS.QuantidadeWidgetControleColetaEntregaUsuario);

    AdicionarObservablesWidget();
    VerificarAbaAtiva();
    BuscarConfiguracaoWidget(cb);

}

function VerificarAbaAtiva() {
    $('.nav-link').on('click', function (event) {
        var abaId = $(this).attr('href'); 

        $('#footerTabWidget, #footerTabDetalhes').addClass('d-none');

        if (abaId === '#tabConfiguracaoWidget') {
            $('#footerTabWidget').removeClass('d-none');
        } else if (abaId === '#tabDetalhesEntregas') {
            $('#footerTabDetalhes').removeClass('d-none');
        }
    });
}

function salvarConfiguracaoWidgetClick() {
    var data = RetornarObjetoPesquisa(_configuracaoWidget);

    executarReST("ConfiguracaoWidgetUsuario/SalvarConfiguracaoUsuario", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                setarVisibleCamposDetalhesEntrega();
                Global.fecharModal("divModalConfiguracao");
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function BuscarConfiguracaoWidget(cb) {
    executarReST("ConfiguracaoWidgetUsuario/ObterConfiguracaoUsuario", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_configuracaoWidget, arg);
                dadosConfiguracaoWidget = arg.Data;

                if (arg.Data.ConfiguracaoExibicaoDetalhesEntrega)
                    preencherConfiguracoesDetalhesEntrega(arg.Data.ConfiguracaoExibicaoDetalhesEntrega);

                estadoInicialConfiguracaoWidget = {};
                for (var campo of camposConfiguracaoWidget) {
                    if (_configuracaoWidget[campo]) {
                        estadoInicialConfiguracaoWidget[campo] = _configuracaoWidget[campo].val();
                    }
                }
                setarVisibleCamposDetalhesEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        _salvarConfiguracoesExibicoesDetalhesEntrega.SalvarWidget.enable(false);
        _salvarConfiguracoesExibicoesDetalhesEntrega.SalvarDetalhes.enable(false);

        habilitarDepoisAlterado(_configuracaoExibicaoDetalhesEntregaCliente);
        habilitarDepoisAlterado(_configuracaoExibicaoDetalhesEntregaEntregaColeta);
        habilitarDepoisAlterado(_configuracaoExibicaoDetalhesEntregaCadeiaAjuda);
        habilitarDepoisAlterado(_configuracaoWidget);

        if (cb) cb();
    }, null);
}

function VerificarLimiteOpcoes() {
    var quantidadeLimite = _configuracaoWidget.LimiteWidgetAtivos.val();
    var quantidadeMenorQueLimite = true;
    var quantidadeOpcoesAtivas = 0;

    for (var campo of camposConfiguracaoWidget) {
        if (_configuracaoWidget[campo]) {
            var ativo = _configuracaoWidget[campo].val();
            quantidadeOpcoesAtivas = ativo ? quantidadeOpcoesAtivas + 1 : quantidadeOpcoesAtivas;

            if (quantidadeOpcoesAtivas >= quantidadeLimite) {
                quantidadeMenorQueLimite = false;
                break;
            }
        }
    }

    _configuracaoWidget.Habilitar.val(quantidadeMenorQueLimite);
}

function AdicionarObservablesWidget() {
    for (var campo of camposConfiguracaoWidget) {
        if (_configuracaoWidget[campo]) {
            _configuracaoWidget[campo].val.subscribe(VerificarLimiteOpcoes);
        }
    }
}

function selecionarTodosExibicoesDetalhesEntrega(knockout, val) {
    for (const prop in knockout) {
        if (knockout[prop]?.val && typeof knockout[prop].val === "function") {
            knockout[prop].val(val);
        }
    }
}

function preencherConfiguracoesDetalhesEntrega(data) {
    PreencherObjetoKnout(_configuracaoExibicaoDetalhesEntregaCliente, { Data: data.ConfiguracaoExibicaoDetalhesEntregaCliente });
    PreencherObjetoKnout(_configuracaoExibicaoDetalhesEntregaEntregaColeta, { Data: data.ConfiguracaoExibicaoDetalhesEntregaEntregaColeta });
    PreencherObjetoKnout(_configuracaoExibicaoDetalhesEntregaCadeiaAjuda, { Data: data.ConfiguracaoExibicaoDetalhesEntregaCadeiaAjuda });
}

function setarVisibleCamposDetalhesEntrega() {

    for (const propriedade in _configuracaoExibicaoDetalhesEntregaCliente) {
        if (!_entrega.hasOwnProperty(propriedade)) {
            continue;
        }
        let valorDetalhesEntregaCliente = _configuracaoExibicaoDetalhesEntregaCliente[propriedade].val();

        if (typeof _entrega[propriedade]?.visible === 'function') {
            _entrega[propriedade].visible(valorDetalhesEntregaCliente);
        } else {
            _entrega[propriedade].visible = valorDetalhesEntregaCliente;
        }
    }
    for (let propriedade in _configuracaoExibicaoDetalhesEntregaEntregaColeta) {
        if (!_entrega.hasOwnProperty(propriedade)) {
            continue;
        }
        let valorExibicaoDetalhesEntregaEntregaColeta = _configuracaoExibicaoDetalhesEntregaEntregaColeta[propriedade].val();

        if (typeof _entrega[propriedade]?.visible === 'function') {
            _entrega[propriedade].visible(valorExibicaoDetalhesEntregaEntregaColeta);
        } else {
            _entrega[propriedade].visible = valorExibicaoDetalhesEntregaEntregaColeta;
        }
    }
    for (let propriedade in _configuracaoExibicaoDetalhesEntregaCadeiaAjuda) {
        if (!_entrega.hasOwnProperty(propriedade)) {
            continue;
        }
        let valorExibicaoDetalhesEntregaCadeiaAjuda = _configuracaoExibicaoDetalhesEntregaCadeiaAjuda[propriedade].val();

        if (typeof _entrega[propriedade]?.visible === 'function') {
            _entrega[propriedade].visible(valorExibicaoDetalhesEntregaCadeiaAjuda);
        } else {
            _entrega[propriedade].visible = valorExibicaoDetalhesEntregaCadeiaAjuda;
        }
    }

}

function restaurarPadraoConfiguracoesExibicaoDetalhesEntrega() {
    exibirConfirmacao("Confirmação", "Realmente deseja voltar ao padrão incial das configurações?", function () {
        _configuracaoWidget.ConfiguracaoExibicaoDetalhesEntrega.val("");
        salvarConfiguracaoWidgetClick();
        LimparCampos(_configuracaoExibicaoDetalhesEntregaCliente);
        LimparCampos(_configuracaoExibicaoDetalhesEntregaEntregaColeta);
        LimparCampos(_configuracaoExibicaoDetalhesEntregaCadeiaAjuda);
        selecionarPropriedadesPadrao();
    });
}

function botoesConfiguracaoWidget() {
    this.Salvar = PropertyEntity({ eventClick: salvarConfiguracaoWidgetClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, enable: ko.observable(false) });
}

function salvarConfiguracoesExibicaoDetalhesEntrega() {
    var configuracoesCliente = RetornarObjetoPesquisa(_configuracaoExibicaoDetalhesEntregaCliente);
    var configuracoesEntregaColeta = RetornarObjetoPesquisa(_configuracaoExibicaoDetalhesEntregaEntregaColeta);
    var configuracoesCadeiaAjuda = RetornarObjetoPesquisa(_configuracaoExibicaoDetalhesEntregaCadeiaAjuda);

    var configuracoesDetalhesEntrega = {
        ConfiguracaoExibicaoDetalhesEntregaCliente: configuracoesCliente,
        ConfiguracaoExibicaoDetalhesEntregaEntregaColeta: configuracoesEntregaColeta,
        ConfiguracaoExibicaoDetalhesEntregaCadeiaAjuda: configuracoesCadeiaAjuda,
    };

    _configuracaoWidget.ConfiguracaoExibicaoDetalhesEntrega.val(JSON.stringify(configuracoesDetalhesEntrega));

    salvarConfiguracaoWidgetClick();
}

function selecionarPropriedadesPadrao() {

    _configuracaoExibicaoDetalhesEntregaCliente.NomeRecebedor.val(true);
    _configuracaoExibicaoDetalhesEntregaCliente.TelefoneCliente.val(true);
    _configuracaoExibicaoDetalhesEntregaCliente.DocumentoRecebedor.val(true);
    _configuracaoExibicaoDetalhesEntregaCliente.EnderecoCliente.val(true);
    _configuracaoExibicaoDetalhesEntregaCliente.LocalidadeCliente.val(true);
    _configuracaoExibicaoDetalhesEntregaCliente.Localizacao.val(true);
    _configuracaoExibicaoDetalhesEntregaCliente.Email.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataEntradaRaio.val(true);
    _configuracaoExibicaoDetalhesEntregaCliente.DataPrevisaoEntrega.val(true);
    _configuracaoExibicaoDetalhesEntregaCliente.InicioViagemPrevista.val(true);
    _configuracaoExibicaoDetalhesEntregaCliente.InicioViagemRealizada.val(true);
    _configuracaoExibicaoDetalhesEntregaCliente.EtapaStage.val(false);
    _configuracaoExibicaoDetalhesEntregaCliente.Mesoregiao.val(false);
    _configuracaoExibicaoDetalhesEntregaCliente.Regiao.val(false);
    _configuracaoExibicaoDetalhesEntregaCliente.CodigoSap.val(false);
    _configuracaoExibicaoDetalhesEntregaCliente.CodigoIntegracaoCliente.val(false);
    _configuracaoExibicaoDetalhesEntregaCliente.CodigoIntegracaoFilial.val(false);
    _configuracaoExibicaoDetalhesEntregaCliente.DataPrevisaoSaida.val(false);
    _configuracaoExibicaoDetalhesEntregaCliente.DataSaidaRaio.val(false);
    _configuracaoExibicaoDetalhesEntregaCliente.JanelaDescarga.val(false);

    _configuracaoExibicaoDetalhesEntregaEntregaColeta.EntregaNoPrazo.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.OrdemEntrega.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.Situacao.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.DistanciaOrigemXEntrega.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.LocalidadeEntrega.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.Pedidos.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.NumeroPedidoCliente.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.NotasFiscais.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.Peso.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.Observacao.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataInicio.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataFim.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataConfirmacao.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.StatusEntregaNota.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.AlterarDataAgendamentoEntregaTransportador.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataAgendamentoDeEntrega.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataAgendamentoEntregaTransportador.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataPrevisaoEntrega.val(true);
    _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataEntregaReprogramada.val(true);

    _configuracaoExibicaoDetalhesEntregaCadeiaAjuda.EscritorioVenda.val(true);
    _configuracaoExibicaoDetalhesEntregaCadeiaAjuda.EquipeVendas.val(true);
    _configuracaoExibicaoDetalhesEntregaCadeiaAjuda.CanalVenda.val(true);

}

function configuracaoWidgetFoiAlterada() {
    for (var campo of camposConfiguracaoWidget) {
        if (_configuracaoWidget[campo]) {
            if (_configuracaoWidget[campo].val() !== estadoInicialConfiguracaoWidget[campo]) {
                return true;
            }
        }
    }
    return false;
}

function capturarEstadoInicialConfiguracaoWidgetDetalhesEntrega() {
    estadoInicialConfiguracaoWidgetDetalhesEntrega = {
        Cliente: {},
        EntregaColeta: {},
        CadeiaAjuda: {}
    };

    for (var prop in _configuracaoExibicaoDetalhesEntregaCliente) {
        if (_configuracaoExibicaoDetalhesEntregaCliente[prop]?.val) {
            estadoInicialConfiguracaoWidgetDetalhesEntrega.Cliente[prop] = _configuracaoExibicaoDetalhesEntregaCliente[prop].val();
        }
    }

    for (var prop in _configuracaoExibicaoDetalhesEntregaEntregaColeta) {
        if (_configuracaoExibicaoDetalhesEntregaEntregaColeta[prop]?.val) {
            estadoInicialConfiguracaoWidgetDetalhesEntrega.EntregaColeta[prop] = _configuracaoExibicaoDetalhesEntregaEntregaColeta[prop].val();
        }
    }

    for (var prop in _configuracaoExibicaoDetalhesEntregaCadeiaAjuda) {
        if (_configuracaoExibicaoDetalhesEntregaCadeiaAjuda[prop]?.val) {
            estadoInicialConfiguracaoWidgetDetalhesEntrega.CadeiaAjuda[prop] = _configuracaoExibicaoDetalhesEntregaCadeiaAjuda[prop].val();
        }
    }
}

function configuracaoWidgetDetalhesFoiAlterada() {
    for (var prop in _configuracaoExibicaoDetalhesEntregaCliente) {
        if (_configuracaoExibicaoDetalhesEntregaCliente[prop]?.val) {
            if (_configuracaoExibicaoDetalhesEntregaCliente[prop].val() !== estadoInicialConfiguracaoWidgetDetalhesEntrega.Cliente[prop]) {
                return true;
            }
        }
    }

    for (var prop in _configuracaoExibicaoDetalhesEntregaEntregaColeta) {
        if (_configuracaoExibicaoDetalhesEntregaEntregaColeta[prop]?.val) {
            if (_configuracaoExibicaoDetalhesEntregaEntregaColeta[prop].val() !== estadoInicialConfiguracaoWidgetDetalhesEntrega.EntregaColeta[prop]) {
                return true;
            }
        }
    }

    for (var prop in _configuracaoExibicaoDetalhesEntregaCadeiaAjuda) {
        if (_configuracaoExibicaoDetalhesEntregaCadeiaAjuda[prop]?.val) {
            if (_configuracaoExibicaoDetalhesEntregaCadeiaAjuda[prop].val() !== estadoInicialConfiguracaoWidgetDetalhesEntrega.CadeiaAjuda[prop]) {
                return true;
            }
        }
    }
    return false;
}

function inicializarEventosModalConfiguracaoWidget() {
    var fecharPorXConfiguracaoWidget = false;
    var btnClose = document.getElementById('btnCloseConfiguracaoWidget');
    var modal = document.getElementById('divModalConfiguracao');

    if (btnClose) {
        btnClose.addEventListener('mousedown', function () {
            fecharPorXConfiguracaoWidget = true;
        });
    }

    if (modal) {
        modal.addEventListener('hide.bs.modal', function (event) {
            if (fecharPorXConfiguracaoWidget && (configuracaoWidgetFoiAlterada() || configuracaoWidgetDetalhesFoiAlterada())) {
                fecharPorXConfiguracaoWidget = false;
                exibirConfirmacao(
                    Localization.Resources.Gerais.Geral.Atencao,
                    "Você possui dados não atualizados. Deseja sair mesmo assim?",
                    function () {
                        Global.fecharModal("divModalConfiguracao");
                        if (dadosConfiguracaoWidget.ConfiguracaoExibicaoDetalhesEntrega == null)
                            selecionarPropriedadesPadrao();
                    }
                );
                event.preventDefault();
            }
        });
    }
}

function fecharAccordions(containerSelector = '#tabDetalhesEntregas') {
    const container = document.querySelector(containerSelector);
    if (!container) return;

    const collapses = container.querySelectorAll('.accordion-collapse');
    collapses.forEach(el => {
        const inst = bootstrap.Collapse.getOrCreateInstance(el, { toggle: false });
        inst.hide();
        el.classList.remove('show');
    });

    const buttons = container.querySelectorAll('.accordion-button');
    buttons.forEach(btn => {
        btn.classList.add('collapsed');
        btn.setAttribute('aria-expanded', 'false');
    });
}

function habilitarDepoisAlterado(vm) {
    for (const key in vm) {
        const prop = vm[key];
        if (prop && ko.isObservable?.(prop.val)) {
            prop.val.subscribe(() => {
                _salvarConfiguracoesExibicoesDetalhesEntrega.SalvarDetalhes.enable(true);
                _salvarConfiguracoesExibicoesDetalhesEntrega.SalvarWidget.enable(true);
            });
        }
    }
}