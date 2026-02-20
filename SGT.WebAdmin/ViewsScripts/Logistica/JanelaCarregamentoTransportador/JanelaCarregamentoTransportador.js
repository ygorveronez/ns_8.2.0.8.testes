/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaCarregamentoTransportador.js" />
/// <reference path="../../Enumeradores/EnumCamposOpcionaisJanelaCarregamentoTransportador.js" />
/// <reference path="../../Cargas/CargaVeiculoContainer/CargaVeiculoContainerAnexo.js" />
/// <reference path="CargaLacre.js" />
/// <reference path="Interesse.js" />
/// <reference path="MensagemPadraoInformarDadosTransporte.js" />
/// <reference path="SelecaoPeriodoCarregamento.js" />
/// <reference path="VeiculosDisponiveis.js" />
/// <reference path="Checklist.js" />

// #region Objetos Globais do Arquivo

var _cargasExibidas = {};
var _dadosPesquisaCargas;
var _detalhesCarga;
var _gridDatasCarregamentoCargaJanelaTransportador
var _informacoesJanelaCarregamentoTransportador;
var _mapaRotaMontagemCargaTransportador;
var _pesquisaJanelaCarregamentoTransportador;
var _informacoesTransportado;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaJanelaCarregamentoTransportador = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroDaCarga.getFieldDescription(), def: "", val: ko.observable(""), maxlength: 50 });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroDoPedido.getFieldDescription(), def: "", val: ko.observable(""), maxlength: 50 });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataInicial.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataFinal.getFieldDescription(), getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamentoTransportador.Todas), options: EnumSituacaoCargaJanelaCarregamentoTransportador.obterOpcoesPesquisa(), def: EnumSituacaoCargaJanelaCarregamentoTransportador.Todas, text: Localization.Resources.Gerais.Geral.Situacao });
    this.TipoLiberacao = PropertyEntity({ val: ko.observable(EnumTipoLiberacaoCargaJanelaCarregamento.Todos), options: EnumTipoLiberacaoCargaJanelaCarregamento.obterOpcoesPesquisa(), def: EnumTipoLiberacaoCargaJanelaCarregamento.Todos, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.TipoDeLiberacao });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription(), idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription(), idBtnSearch: guid() });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Rota.getFieldDescription(), idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ModeloDeVeiculo.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroExp = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NroEXP.getFieldDescription(), maxlength: 150 });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.TipoDeCarga.getFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.Filiais = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), idBtnSearch: guid() });


    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(false);
            recarregarPesquisaCargas();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(false)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, eventChange: cargasPesquisaScroll, visible: ko.observable(false), cssClass: ko.observable("col-xs-7 col-sm-8 col-md-8 col-lg-9") });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, type: types.local });
    this.Requisicao = PropertyEntity({ val: ko.observable(false), def: false, type: types.local, idTab: guid(), visible: ko.observable(true) });
    this.Cargas = ko.observableArray();
}

var CargaModelTransportador = function (params) {
    var self = this;
    var dados = params.Dados;
    var camposVisiveis = params?.Dados?.CamposVisiveis?.split(";");

    //this.HabilitarClick = PropertyEntity({ val: params.HabilitarClick.val, click: params.HabilitarClick.eventClick });
    this.HabilitarClick = PropertyEntity({});

    this.Codigo = PropertyEntity({ val: ko.observable(dados.Codigo), def: dados.Codigo });
    this.CodigoJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(dados.CodigoJanelaCarregamentoTransportador), def: dados.CodigoJanelaCarregamentoTransportador });
    this.Carga = PropertyEntity({ val: ko.observable(dados.Carga.Codigo), def: dados.Carga.Codigo });
    this.CargaDeComplemento = PropertyEntity({ val: ko.observable(dados.Carga.CargaDeComplemento), def: dados.Carga.CargaDeComplemento, getType: typesKnockout.bool });
    this.Numero = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Cargas.getFieldDescription(), val: ko.observable(dados.Carga.Numero), def: dados.Carga.Numero });
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataDeCarregamento.getFieldDescription(), val: ko.observable(dados.InicioCarregamento), def: dados.InicioCarregamento, cssClass: dados.HorarioFixoCarregamento ? "data-carregamento-fixa" : "", visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.DataCarregamento.toString()) });
    this.DataCarregamentoReal = PropertyEntity({ val: ko.observable(dados.DataCarregamento), def: dados.DataCarregamento });
    this.HorarioFixoCarregamento = PropertyEntity({ val: ko.observable(dados.HorarioFixoCarregamento), def: dados.HorarioFixoCarregamento, getType: typesKnockout.bool });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(dados.Situacao), def: dados.Situacao, cssClass: ko.observable(EnumSituacaoCargaJanelaCarregamentoTransportador.obterClasseCor(dados.Situacao)) });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.TipoDeCarga.getFieldDescription(), val: ko.observable(dados.TipoCarga.Descricao), def: dados.TipoCarga.Descricao, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.TipoCarga.toString()) });
    this.PrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.PrevisaoDeEntrega.getFieldDescription(), val: ko.observable(dados.PrevisaoEntrega), def: dados.PrevisaoEntrega, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.PrevisaoEntrega.toString()) });
    this.ModeloVeiculo = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Veiculo.getFieldDescription(), val: ko.observable(dados.ModeloVeiculo.Descricao), def: dados.ModeloVeiculo.Descricao, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.ModeloVeiculo.toString()) });
    this.DataProximaSituacao = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Prazo.getFieldDescription(), val: ko.observable(dados.DataProximaSituacao), def: dados.DataProximaSituacao, visible: ko.observable(false) });
    this.Ordem = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Ordem.getFieldDescription(), val: ko.observable(dados.Ordem), def: dados.Ordem, visible: ko.observable(dados.ExibirDetalhesCargaJanelaCarregamentoTransportador && camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Ordem.toString())) });
    this.Origem = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription(), val: ko.observable(dados.Origem), def: dados.Origem, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Origem.toString()) });
    this.Destino = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription(), val: ko.observable(dados.Destino), def: dados.Destino, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Destino.toString()) });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), val: ko.observable(dados.Destinatario), def: dados.Destinatario, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Destinatario.toString()) });
    this.NumeroEntregas = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroDeEntregas.getFieldDescription(), val: ko.observable(dados.NumeroEntregas), def: dados.NumeroEntregas, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.NumeroEntregas.toString()) });
    this.NumeroEntregasFinais = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroDeEntregasFinais.getFieldDescription(), val: ko.observable(dados.NumeroEntregasFinais), def: dados.NumeroEntregasFinais, visible: ko.observable(dados.NumeroEntregasFinais > 0) });
    this.Volumes = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Volumes.getFieldDescription(), val: ko.observable(dados.Volumes), def: dados.Volumes, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Volumes.toString()) });
    this.CargaPerigosa = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaPerigosa.getFieldDescription(), val: ko.observable(dados.Carga.CargaPerigosa), def: dados.Carga.CargaPerigosa });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), val: ko.observable(dados.Remetente), def: dados.Remetente, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Remetente.toString()) });
    this.DataAgendamento = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataAgendamento.getFieldDescription(), val: ko.observable(dados.DataAgendamento), def: dados.DataAgendamento });
    this.DataColeta = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataColeta.getFieldDescription(), val: ko.observable(dados.DataColeta), def: dados.DataColeta });
    this.CargasVinculadas = PropertyEntity({ val: ko.observableArray(dados.CargasVinculadas), def: dados.CargasVinculadas });
    this.Pedido = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Pedido.getFieldDescription(), val: ko.observable(dados.Pedido), def: dados.Pedido, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Pedido.toString()) });
    this.NumeroEXP = PropertyEntity({ text: "EXP:", val: ko.observable(dados.NumeroEXP), def: dados.NumeroEXP, visible: ko.observable(false) });
    this.PesoCubado = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.PesoCubado.getFieldDescription(), val: ko.observable(dados.PesoCubado), def: dados.PesoCubado, visible: ko.observable(false) });
    this.Transportador = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Transportador.getFieldDescription(), val: ko.observable(dados.Transportador.Descricao), def: dados.Transportador.Descricao, visible: ko.observable(false) });
    this.Filial = PropertyEntity({ text: "Filial:", val: ko.observable(dados.Filial.Descricao), def: dados.Filial.Descricao, visible: ko.observable(false) });
    this.Enderecos = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Endereco.getFieldDescription(), val: ko.observable(dados.Enderecos), def: dados.Endereco, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Endereco.toString()) });
    this.CDDestino = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.CDDestino.getFieldDescription(), val: ko.observable(dados.CDDestino), def: dados.CDDestino, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.CDDestino.toString()) });
    this.DivisoriaIntegracaoLeilao = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Divisoria.getFieldDescription(), val: ko.observable(dados.DivisoriaIntegracaoLeilao), def: dados.DivisoriaIntegracaoLeilao, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.DivisoriaIntegracaoLeilao.toString()) });
    this.CargaPerigosaIntegracaoLeilao = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ContemCargaPerigosa.getFieldDescription(), val: ko.observable(dados.CargaPerigosaIntegracaoLeilao), def: dados.CargaPerigosaIntegracaoLeilao, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.CargaPerigosaIntegracaoLeilao.toString()) });
    this.CanalEntrega = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.CanalEntrega.getFieldDescription(), val: ko.observable(dados.CanalEntrega), def: dados.CanalEntrega, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.CanalEntrega.toString()) });
    this.CanalVenda = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.CanalVenda.getFieldDescription(), val: ko.observable(dados.CanalVenda), def: dados.CanalVenda, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.CanalVenda.toString()) });
    this.NotasEnviadas = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NotasEnviadas.getFieldDescription(), val: ko.observable(dados.NotasEnviadas), def: dados.NotasEnviadas, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.NotasEnviadas.toString()) });
    this.FreteSimulado = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.FreteSimulado.getFieldDescription(), val: ko.observable(dados.FreteSimulado), def: dados.FreteSimulado, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.FreteSimulado.toString()) });
    this.DataPrevisaoTerminoCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataPrevisaoTerminoCarregamento.getFieldDescription(), val: ko.observable(dados.DataPrevisaoTerminoCarregamento), def: dados.DataPrevisaoTerminoCarregamento, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.DataPrevisaoTerminoCarregamento.toString()) });

    if (_informacoesTransportado.Empresa.Codigo != dados.Transportador.Codigo && _informacoesTransportado.Empresa.Matriz)
        this.Transportador.visible(true);

    this.PermitirSelecionarDataCarregamento = PropertyEntity({ val: ko.observable(dados.PermitirSelecionarDataCarregamento), def: dados.PermitirSelecionarDataCarregamento, getType: typesKnockout.bool });
    this.PermitirSelecionarDatasDescarregamento = PropertyEntity({ val: ko.observable(dados.PermitirSelecionarDatasDescarregamento), def: dados.PermitirSelecionarDatasDescarregamento, getType: typesKnockout.bool });
    this.PermitirExibicaoDatasDescarregamento = PropertyEntity({ val: ko.observable(dados.PermitirExibicaoDatasDescarregamento), def: dados.PermitirExibicaoDatasDescarregamento, getType: typesKnockout.bool });

    var htmlDetalhesFrete = "<span class='fw-bold'>Valor do Frete: " + dados.ValorFreteSemICMS + "</span>";

    for (var i = 0; i < dados.Componentes.length; i++)
        htmlDetalhesFrete += "<br/><span class='fw-bold'>" + (dados.Componentes[i].Descricao.length > 20 ? dados.Componentes[i].Descricao.substring(0, 18) + "..." : dados.Componentes[i].Descricao) + ": " + dados.Componentes[i].Valor + "</span>";

    var htmlDetalhesFreteTransportador =
        "<span>Valor do Frete: " + Globalize.format(dados.ValorFreteTabelaTransportador, "n2") + "</span>"
        + dados.ComponentesFreteTransportador.map(function (componente) {
            return "<br/><span>" + (componente.Descricao.length > 20 ? componente.Descricao.substring(0, 18) + "..." : componente.Descricao) + ": " + componente.Valor + "</span>";
        }).join("");;

    var htmlDetalhesFreteTransportadorComProblema = "<span class='popover-valor-frete-transportador-com-problema'>Sem valor de frete, verifique com o Embarcador</span> <span class='popover-valor-frete-transportador-com-problema popover-valor-frete-transportador-com-problema-cor-laranja'>(clique para recalcular)</span>";

    this.ExibirValorDetalhadoJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.ExibirValorDetalhadoJanelaCarregamentoTransportador) });
    this.ValorFreteLiquido = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorDoFreteLiquido.getFieldDescription(), val: ko.observable(dados.ValorFreteLiquido), def: dados.ValorFreteLiquido, formatted: Globalize.format(dados.ValorFreteLiquido, "n2"), visible: ko.observable(!dados.NaoExibirValorFretePortalTransportador) });
    this.ValorFrete = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorDoFrete.getFieldDescription(), val: ko.observable(dados.ValorFrete), def: dados.ValorFrete, visible: ko.observable(!dados.NaoExibirValorFretePortalTransportador), popover: htmlDetalhesFrete });
    this.ValorFreteTabelaTransportador = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorDoFrete.getFieldDescription(), val: ko.observable(dados.ValorFreteTabelaTransportador), visible: ko.observable((dados.PossuiFreteCalculado && !dados.FreteCalculadoComProblemas && !dados.NaoExibirValorFretePortalTransportador)), def: dados.ValorFreteTabelaTransportador, formatted: Globalize.format(dados.ValorFreteTabelaTransportador, "n2"), popover: htmlDetalhesFreteTransportador });
    this.ValorFreteTabelaTransportadorComProblema = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorDoFrete.getFieldDescription(), visible: ko.observable((dados.PendenteCalcularFreteTransportador || (dados.PossuiFreteCalculado && dados.FreteCalculadoComProblemas)) && !dados.NaoExibirValorFretePortalTransportador), val: ko.observable("Sem Frete"), def: "Sem Frete", popover: htmlDetalhesFreteTransportadorComProblema, eventClick: recalcularValorFreteTabelaTransportadorClick });
    this.Peso = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Peso.getFieldDescription(), val: ko.observable(dados.Peso), def: dados.Peso, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Peso.toString()) });
    this.Placas = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Placas.getFieldDescription(), val: ko.observable(dados.Placas), def: dados.Placas, visible: ko.observable(false), idTab: guid() });
    this.Motorista = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription(), val: ko.observable(dados.Motorista), def: dados.Motorista, visible: ko.observable(false) });
    this.TotalPallets = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.TotalDePallets.getFieldDescription(), val: ko.observable(dados.TotalPallets), def: dados.TotalPallets, visible: ko.observable(dados.ExibirDetalhesCargaJanelaCarregamentoTransportador) });
    this.ValorMercadorias = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorDasMercadorias.getFieldDescription(), val: ko.observable(dados.ValorMercadorias), def: dados.ValorMercadorias, visible: ko.observable(dados.ExibirDetalhesCargaJanelaCarregamentoTransportador) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), val: ko.observable(dados.Observacao), def: dados.Observacao, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Observacao.toString()) });
    this.ObservacaoCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ObservacaoDoCarregamento.getFieldDescription(), val: ko.observable(dados.ObservacaoCarregamento), def: dados.ObservacaoCarregamento, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.ObservacaoCarregamento.toString()) });
    this.ObservacaoCliente = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ObservacoesClientes.getFieldDescription(), val: ko.observable(dados.ObservacaoCliente), def: dados.ObservacaoCliente, visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.ObservacaoCliente.toString()) });
    this.Mensagem = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Mensagem.getFieldDescription(), val: ko.observable(dados.Mensagem), def: dados.Mensagem, visible: ko.observable(false) });
    this.NotasFiscais = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NotasFiscais.getFieldDescription(), val: ko.observable(dados.NotasFiscais), def: dados.NotasFiscais, visible: ko.observable(false) });
    this.Rota = PropertyEntity({ visible: _CONFIGURACAO_TMS.PermiteSelecionarRotaMontagemCarga && camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Rota.toString()), eventClick: visualizarRotaFreteMapaClick, type: types.entity, codEntity: ko.observable(dados.Rota.Codigo), text: "Rota:", val: ko.observable(dados.Rota.Descricao), def: "" });
    this.HorarioLimiteConfirmarCarga = PropertyEntity({ val: ko.observable(dados.HorarioLimiteConfirmarCarga), def: dados.HorarioLimiteConfirmarCarga, visible: ko.observable(false) });
    this.BloquearComponentesDeFreteJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(dados.BloquearComponentesDeFreteJanelaCarregamentoTransportador), def: dados.BloquearComponentesDeFreteJanelaCarregamentoTransportador, getType: typesKnockout.bool });
    this.BloquearTrocaDataListaHorarios = PropertyEntity({ val: ko.observable(dados.BloquearTrocaDataListaHorarios), def: dados.BloquearTrocaDataListaHorarios, getType: typesKnockout.bool });
    this.ValidacaoConjunto = PropertyEntity({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.ValidacaoConjunto.getFieldDescription(), val: ko.observable(dados.ValidacaoConjunto), def: "", visible: camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.ValidacaoConjunto.toString()) });
    this.UnidadeDeMedida = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.UnidadeDeMedida.getFieldDescription(), val: ko.observable(dados.UnidadeDeMedida), def: dados.UnidadeDeMedida, visible: ko.observable(true) });

    this.NumeroDoca = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Doca.getFieldDescription(), val: ko.observable(dados.Carga.NumeroDoca), visible: ko.observable(true) });

    this.ExigeVeiculoRastreado = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Rastreado.getFieldDescription(), val: ko.observable(dados.TipoCarga.ExigeVeiculoRastreado), def: dados.TipoCarga.ExigeVeiculoRastreado, visible: ko.observable(dados.TipoCarga.ExigeVeiculoRastreado) });
    this.CargaLiberadaCotacao = PropertyEntity({ val: ko.observable(dados.CargaLiberadaCotacao), def: dados.CargaLiberadaCotacao, getType: typesKnockout.bool, visible: ko.observable(dados.CargaLiberadaCotacao && (dados.Situacao === EnumSituacaoCargaJanelaCarregamentoTransportador.Disponivel) && (dados.SituacaoJanelaCarregamento === EnumSituacaoCargaJanelaCarregamento.SemTransportador)) });
    this.ExigeTermoAceiteParaTransporte = PropertyEntity({ val: ko.observable(dados.ExigeTermoAceiteParaTransporte), def: dados.ExigeTermoAceiteParaTransporte, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.TermoAceite = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(dados.TermoAceite), def: dados.TermoAceite });

    this.MenorLance = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.MenorLance.getFieldDescription(), val: ko.observable(dados.Leilao.MenorLance), visible: ko.observable(dados.Leilao.MenorLance != ""), corFonte: ko.observable(dados.Leilao.CorFonte) });
    this.PosicaoLeilao = PropertyEntity({ text: "Posição Leilão:", val: ko.observable(dados.Leilao.PosicaoLeilao), visible: ko.observable(dados.Leilao.PosicaoLeilao != ""), corFonte: ko.observable("#23A723") });
    this.ValorTransportadorInteressadoSinalizado = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorOfertado.getFieldDescription(), val: ko.observable(dados.Leilao.ValorTransportadorInteressadoSinalizado), visible: ko.observable(dados.Leilao.ValorTransportadorInteressadoSinalizado != ""), corFonte: ko.observable("#23A723") });
    this.TempoRestante = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.TempoRestante.getFieldDescription(), val: ko.observable(""), visible: ko.observable(dados.Leilao.DataFimLeilao != ""), id: dados.Leilao.IdTempoRestante });
    this.AguardandoAutorizacaoFrete = PropertyEntity({ val: ko.observable(dados.AguardandoAutorizacaoFrete), visible: ko.observable(dados.AguardandoAutorizacaoFrete), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaAguardandoAutorizacaoDoFrete, });

    this.InformarValorFrete = PropertyEntity({ eventClick: function (e, sender) { sender.stopPropagation(); informarValorFreteClick(e); }, type: types.event, visible: (dados.Situacao == EnumSituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao && dados.PermitirInformarValorFreteCargasAtribuidasAoTransportadorNaJanelaCarregamentoTransportador) });
    this.SelecionarDataCarregamento = PropertyEntity({ eventClick: function (e, sender) { sender.stopPropagation(); exibirSelecaoDatasDisponiveisCarregamento(e); }, enable: ko.observable(true), type: types.event });
    this.SelecionarDatasDescarregamento = PropertyEntity({ eventClick: function (e, sender) { sender.stopPropagation(); exibirSelecaoDatasDisponiveisDescarregamentoClick(e); }, enable: ko.observable(true), type: types.event });
    this.ExibirDatasDescarregamento = PropertyEntity({ eventClick: function (e, sender) { sender.stopPropagation(); exibirDatasDescarregamentoJanelaTransportadorClick(e); }, enable: ko.observable(true), type: types.event });
    this.ExibirDatasCarregamento = PropertyEntity({ eventClick: function (e, sender) { sender.stopPropagation(); exibirDatasCarregamentoCargaJanelaTransportadorClick(e.Carga.val()); }, enable: ko.observable(true), type: types.event });
    this.TenhoInteresse = PropertyEntity({ eventClick: function (e, sender) { tenhoInteresseClick(e, sender); }, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.TenhoInteresse, icon: "fa fa-thumbs-up", idGrid: guid(), visible: ko.observable(false) });
    this.NaoTenhoInteresse = PropertyEntity({ eventClick: function (e, sender) { naoTenhoInteresseClick(e, sender); }, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoTenhoInteresse, icon: "fa fa-thumbs-down", idGrid: guid(), visible: ko.observable(false) });

    this.GerarControleVisualizacaoTransportadorasTerceiros = PropertyEntity({ val: ko.observable(dados.GerarControleVisualizacaoTransportadorasTerceiros), def: dados.GerarControleVisualizacaoTransportadorasTerceiros, getType: typesKnockout.bool });

    this.JaVisualizouCarga = PropertyEntity({ val: ko.observable(dados.JaVisualizouCarga), def: dados.JaVisualizouCarga, getType: typesKnockout.bool });
    this.VisualizarCarga = PropertyEntity({ eventClick: function (e, sender) { visualizarCargaClick(e, sender); }, type: types.event, icon: ko.observable(iconVisualizarCargaClick(this) ? "fa fa-eye-slash" : "fa fa-eye"), idGrid: guid(), visible: ko.observable(visibilidadeVisualizarCargaClick(this) ? true : false), val: ko.observable(iconVisualizarCargaClick(this) ? true : false) });


    this.DemarcarInteresse = PropertyEntity({ eventClick: function (e, sender) { removerInteresseClick(e, sender); }, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DesmarcarInteresse, icon: "fa fa-thumbs-down", idGrid: guid(), visible: ko.observable(false) });
    this.NovoLance = PropertyEntity({ eventClick: function (e, sender) { tenhoInteresseClick(e, sender); }, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NovoLance, idGrid: guid(), visible: ko.observable(false), icon: "fa fa-gavel" });
    this.InformarDadosTransporte = PropertyEntity({ eventClick: function () { exibirDetalhesCarga(self); }, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.InformarDadosDeTransporte, icon: "fa fa-truck", visible: ko.observable(false) });
    this.AutorizarEmissaoDocumentos = PropertyEntity({ eventClick: function () { autorizarEmissaoDocumentosCargaClick(self); }, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.AutorizarEmissaoDosDocumentos, icon: "fa fa-angle-right", visible: ko.observable(false) });

    this.HabilitarClick = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, eventClick: function () { exibirDetalhesCarga(self); }, cssClass: ko.observable(retornarCssClassHabilitarClick(this)) });
    this.PossuiVinculos = PropertyEntity({ val: ko.observable(dados.CargasVinculadas.length > 0), def: dados.CargasVinculadas.length > 0, getType: typesKnockout.bool });

    this.Dados = dados;

    switch (dados.Situacao) {
        case EnumSituacaoCargaJanelaCarregamentoTransportador.ComInteresse:
            this.Mensagem.visible(true);
            this.DemarcarInteresse.visible(true);
            break;
        case EnumSituacaoCargaJanelaCarregamentoTransportador.AgAceite:
        case EnumSituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao:
            this.InformarDadosTransporte.visible(true);
            this.DataProximaSituacao.visible(true);
            break;
        case EnumSituacaoCargaJanelaCarregamentoTransportador.Confirmada:
            this.Placas.visible(camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Placas.toString()));
            this.Motorista.visible(camposVisiveis.includes(EnumCamposOpcionaisJanelaCarregamentoTransportador.Motorista.toString()));
            this.HabilitarClick.val(true);
            this.AutorizarEmissaoDocumentos.visible(dados.Carga.PermiteAvancarEtapaEmissao);
            break;
        case EnumSituacaoCargaJanelaCarregamentoTransportador.Rejeitada:
            this.Mensagem.visible(true);
            this.ValorFrete.visible(false);
            this.ValorFreteTabelaTransportador.visible(false);
            this.ValorFreteTabelaTransportadorComProblema.visible(false);
            break;
        default:
            if (!this.JaVisualizouCarga.val() && this.GerarControleVisualizacaoTransportadorasTerceiros.val()) {
                this.TenhoInteresse.visible(false);
                this.NaoTenhoInteresse.visible(false);
            }
            else {
                this.TenhoInteresse.visible(!this.ValorFreteTabelaTransportadorComProblema.visible() || dados.NaoExibirValorFretePortalTransportador);
                this.NaoTenhoInteresse.visible(!this.ValorFreteTabelaTransportadorComProblema.visible() || dados.NaoExibirValorFretePortalTransportador);
            }

    }

    if (dados.Leilao.DataFimLeilao != "") {
        setTimeout(function () {
            $("#" + self.TempoRestante.id)
                .countdown(moment(dados.Leilao.DataFimLeilao, "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                .on('update.countdown', function (event) {
                    if (event.elapsed) {
                        $(this).text("[esgotado]");
                        self.NovoLance.visible(false);
                    }
                    else {
                        self.NovoLance.visible(dados.Situacao == EnumSituacaoCargaJanelaCarregamentoTransportador.ComInteresse);

                        if (event.offset.totalDays > 0)
                            $(this).text(event.strftime('%-Dd %H:%M:%S'));
                        else
                            $(this).text(event.strftime('%H:%M:%S'));
                    }
                })
        }, 300);
    }

    if (self.HorarioLimiteConfirmarCarga.val()) {
        setTimeout(function () {
            $("#" + self.HorarioLimiteConfirmarCarga.id)
                .countdown(moment(self.HorarioLimiteConfirmarCarga.val(), "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                .on('update.countdown', function (event) {
                    if (event.elapsed)
                        $(this).text(" [esgotado]")
                    else {
                        if (event.offset.totalDays > 0)
                            $(this).text(" [" + event.strftime('%-Dd %H:%M:%S') + "]");
                        else
                            $(this).text(" [" + event.strftime('%H:%M:%S') + "]");
                    }
                });
        }, 300);
    }
}

var ContainerCargaTransportadorModel = function (dados) {
    this.Codigo = PropertyEntity({ val: ko.observable(dados.Codigo), def: 0 });
    this.Cargas = PropertyEntity({ val: ko.observable(dados.Cargas), def: 0, tabClick: tabChangeCarga });
    this.PossuiVinculos = PropertyEntity({ val: ko.observable(dados.PossuiVinculos), def: false });
    this.CargaPrincipal = dados.CargaPrincipal;

    this.Dados = dados;
}

// #endregion Classes

// #region Funções de Inicialização

function LoadJanelaCarregamentoTransportador() {
    if (_CONFIGURACAO_TMS.PossuiSituacaoAguardandoAceite)
        $('#legenda-aguardando-aceite').show();

    registrarComponente();
    _pesquisaJanelaCarregamentoTransportador = new PesquisaJanelaCarregamentoTransportador();
    KoBindings(_pesquisaJanelaCarregamentoTransportador, "knockoutPesquisaCargas", false);

    new BuscarLocalidadesBrasil(_pesquisaJanelaCarregamentoTransportador.Destino);
    new BuscarLocalidadesBrasil(_pesquisaJanelaCarregamentoTransportador.Origem);
    new BuscarRotasFrete(_pesquisaJanelaCarregamentoTransportador.Rota);
    new BuscarModelosVeicularesCarga(_pesquisaJanelaCarregamentoTransportador.ModeloVeicularCarga);
    new BuscarTiposdeCarga(_pesquisaJanelaCarregamentoTransportador.TipoCarga);
    new BuscarTiposOperacao(_pesquisaJanelaCarregamentoTransportador.TipoOperacao);
    BuscarFilial(_pesquisaJanelaCarregamentoTransportador.Filiais);

    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                _informacoesTransportado = arg.Data;
                _pesquisaJanelaCarregamentoTransportador.Pesquisar.visible(true);
                recarregarPesquisaCargas();
            }
        }
    });

    //previne a página de dar scroll quando o usuário está usando o scroll de uma das divs
    $('.scrollable').bind('mousewheel DOMMouseScroll', function (e) {
        if ($(this)[0].scrollHeight !== $(this).outerHeight()) {
            var e0 = e.originalEvent,
                delta = e0.wheelDelta || -e0.detail;

            this.scrollTop += (delta < 0 ? 1 : -1) * 30;
            e.preventDefault();
        }
    });

    $('.detalhes-pedido-popover').each(function () {
        bootstrap.Popover.getOrCreateInstance(this, { html: true });
    });

    loadMensagemPadraoInformarDadosTransporte();
    loadInteresseHorarioDiferente();

    if (!_CONFIGURACAO_TMS.NaoExigeInformarDisponibilidadeDeVeiculo)
        loadPlacasDisponiveis();
    else {
        _pesquisaJanelaCarregamentoTransportador.Requisicao.visible(false);
        _pesquisaJanelaCarregamentoTransportador.Total.cssClass("col-xs-12");
    }

    loadDetalhePedido();
    loadCargaVeiculoContainerAnexo();
    loadSelecaoPeriodoCarregamento();
    loadDatasCarregamentoCargaJanelaTransportador();
    loadJanelaCarregamentoTransportadorDataDescarregamento();
    loadAlterarDataCarregamentoJanelaCarregamentoTransportador();
    loadCadastroChecklistVeiculo();
}

function loadDatasCarregamentoCargaJanelaTransportador() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 1, dir: orderDir.asc };

    var header = [
        { data: "Carga", visible: false },
        { data: "CentroCarregamento", title: "Centro de Carregamento", width: "40%", className: "text-align-left", orderable: false },
        { data: "InicioCarregamento", title: "Data do Carregamento", width: "25%", className: "text-align-center", orderable: false },
        { data: "TerminoCarregamento", title: "Data do Descarregamento", width: "25%", className: "text-align-center", orderable: false }
    ];

    _gridDatasCarregamentoCargaJanelaTransportador = new BasicDataTable("grid-datas-carregamento-carga-janela-transportador", header, null, ordenacao, null, linhasPorPaginas);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function cargasPesquisaScroll(e, sender) {
    var elem = sender.target;

    if (_pesquisaJanelaCarregamentoTransportador.Inicio.val() < _pesquisaJanelaCarregamentoTransportador.Total.val() && elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight))
        carregarCargas();
}

function exibirAnexosReboqueClick(cargaSelecionada) {
    exibirCargaVeiculoContainerAnexo(cargaSelecionada.ContainerReboqueAnexo, cargaSelecionada.CodigoContainerReboque.val());
}

function exibirAnexosSegundoReboqueClick(cargaSelecionada) {
    exibirCargaVeiculoContainerAnexo(cargaSelecionada.ContainerSegundoReboqueAnexo, cargaSelecionada.CodigoContainerSegundoReboque.val());
}

function exibirSelecaoDatasDisponiveisCarregamento(e) {
    alterarDataCarregamentoClick(e);
}

function exibirSelecaoDatasDisponiveisDescarregamentoClick(e) {
    alterarJanelaCarregamentoTransportadorDataDescarregamento(e);
}

function exibirDatasDescarregamentoJanelaTransportadorClick(e) {
    exibirJanelaCarregamentoTransportadorDataDescarregamento(e);
}

function exibirDatasCarregamentoCargaJanelaTransportadorClick(codigoCarga) {
    executarReST("Carga/ObterDatasCarregamento", { Carga: codigoCarga }, function (retorno) {
        if (retorno.Success) {
            _gridDatasCarregamentoCargaJanelaTransportador.CarregarGrid(retorno.Data);

            Global.abrirModal('divModalDatasCarregamentoCargaJanelaTransportador');
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function autorizarEmissaoDocumentosCargaClick(e) {
    executarReST("CargaFrete/ValidarInicioEmissaoDocumentos", { Carga: e.Carga.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RoteirizacaoPendente)
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Data.Mensagem);
                else if (retorno.Data.PossuiPendencia)
                    exibirConfirmacao("Confirmação", retorno.Data.Mensagem + " Deseja iniciar a emissão dos documentos mesmo assim?", function () { ConfirmarFreteIniciarEmissao(e); });
                else if (!retorno.Data.PermiteGerarGNRE)
                    exibirConfirmacao("Confirmação", retorno.Data.Mensagem + " Deseja prosseguir?", function () { ConfirmarFreteIniciarEmissao(e); });
                else
                    exibirConfirmacao("Confirmação", "Deseja realmente iniciar a emissão dos documentos?", function () { ConfirmarFreteIniciarEmissao(e); });
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, retorno.Msg);
    });
}

function ConfirmarFreteIniciarEmissao(e) {
    var data = { Carga: e.Carga.val() };
    executarReST("CargaFrete/ConfirmarFreteIniciarEmissao", data, function (arg) {
        if (!arg.Success) {
            if (arg.Data == null) {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, arg.Msg);
                return;
            }

            if (arg.Data.RecarregarDadosEmissao) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 10000);
                return;
            }

            if (!arg.Data.PercursoMDFeValido) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Antes de iniciar a emissão é necessário configurar um percurso válido para gerar o(s) MDF-e(s)");
                return;
            }

            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, arg.Msg);
            return;
        }

        if (arg.Data != false) {
            e.AutorizarEmissaoDocumentos.visible(false);
            IniciarEmissaoCTes(data.Carga);
            return;
        }

        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 30000);
    });
}

function IniciarEmissaoCTes(codigoCarga) {
    executarReST("CargaCTe/AutorizarEmissaoCTes", { Carga: codigoCarga, Status: EnumStatusCTe.EMDIGITACAO }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Emissão dos Documentos", "Emissão dos Documentos iniciado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, arg.Msg);
        }
    });
}

function exibirDetalhesReboqueClick(cargaSelecionada) {
    if (cargaSelecionada.ExigirDefinicaoReboquePedido.val() && cargaSelecionada.PermitirInformarAnexoContainerCarga.val())
        controlarExibicaoOpcoesDetalhesPorVeiculo("#detalhes-reboque-opcoes-" + (cargaSelecionada.Reboque.enable() ? "" : "desabilitado-") + cargaSelecionada.Codigo.val());
    else if (cargaSelecionada.ExigirDefinicaoReboquePedido.val())
        exibirPedidosReboqueClick(cargaSelecionada);
    else
        exibirAnexosReboqueClick(cargaSelecionada);
}

function exibirDetalhesSegundoReboqueClick(cargaSelecionada) {
    if (cargaSelecionada.ExigirDefinicaoReboquePedido.val() && cargaSelecionada.PermitirInformarAnexoContainerCarga.val())
        controlarExibicaoOpcoesDetalhesPorVeiculo("#detalhes-segundo-reboque-opcoes-" + (cargaSelecionada.SegundoReboque.enable() ? "" : "desabilitado-") + cargaSelecionada.Codigo.val());
    else if (cargaSelecionada.ExigirDefinicaoReboquePedido.val())
        exibirPedidosSegundoReboqueClick(cargaSelecionada);
    else
        exibirAnexosSegundoReboqueClick(cargaSelecionada);
}

function exibirDetalhesVeiculoClick(cargaSelecionada) {
    exibirCargaVeiculoContainerAnexo(cargaSelecionada.ContainerVeiculoAnexo, cargaSelecionada.CodigoContainerVeiculo.val());
}

function exibirPedidosReboqueClick(cargaSelecionada) {
    exibirDetalhesPedidos(cargaSelecionada.Codigo.val(), function (knoutDetalhePedido, pedido) {
        if (pedido.NumeroReboque != EnumNumeroReboque.ReboqueUm)
            knoutDetalhePedido.OcultarPedido.val(true);
    });
}

function exibirPedidosSegundoReboqueClick(cargaSelecionada) {
    exibirDetalhesPedidos(cargaSelecionada.Codigo.val(), function (knoutDetalhePedido, pedido) {
        if (pedido.NumeroReboque != EnumNumeroReboque.ReboqueDois)
            knoutDetalhePedido.OcultarPedido.val(true);
    });
}

function recalcularValorFreteTabelaTransportadorClick(carga) {
    executarReST("JanelaCarregamentoTransportador/RecalcularValorFreteTabelaTransportador", { Carga: carga.Carga.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Frete recalculado com sucesso!");

                var cargas = _pesquisaJanelaCarregamentoTransportador.Cargas();

                for (var i = 0; i < cargas.length; i++) {
                    if (cargas[i].Codigo.val() == retorno.Data.Codigo) {
                        AdicionarCarga(retorno.Data, i);
                        break;
                    }
                }

                finalizarRequisicao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function anexoAgendamentoClick() {
    _agendamentoColetaListaAnexos.Adicionar.visible(false);

    Global.abrirModal('divModalAnexoAgendamentoColeta');
    $("#divModalAnexoAgendamentoColeta").one("hidden.bs.modal", function () {
        Global.fecharModal("divModalAdicionarAnexoAgendamentoColeta");
    });
}

function tabChangeCarga(dados, index) {
    var cargas = dados.Cargas.val();
    var carga = cargas[index];

    var LoadDataCarga = function (data) {
        carga.Dados(data);
        carga.Load(true);

        ///Comentado porque esta dando erro na janela de carregamento ao mudar as abas. Se não foi necessario remover
        //setTimeout(function () {
        //    $('[data-bs-toggle="popover"]').each(function () {
        //        tabs = bootstrap.Tab.getOrCreateInstance(this, { placement: "top", html: true, trigger: "hover", container: "body", delay: { "show": 500, "hide": 100 } });
        //    });

        //    tabs.show();
        //}, 100);
    }
    var ObterDoCache = function (key) {
        if (codigoCarga in _cargasExibidas)
            return _cargasExibidas[codigoCarga];

        return null;
    }

    if (!carga.Load() && !carga.Requisitando()) {
        var codigoCarga = carga.Codigo;
        var dadosParaCarregar = ObterDoCache(codigoCarga);

        if (dadosParaCarregar != null) {
            LoadDataCarga(dadosParaCarregar());
        } else {
            var dataRequisicao = { DadosCargaVinculada: true, Carga: dados.CargaPrincipal.Carga.val() };
            carga.Requisitando(true);
            executarReST("JanelaCarregamentoTransportador/ConsultarCargas", dataRequisicao, function (r) {
                carga.Requisitando(false);
                if (r.Success) {

                    $.each(r.Data.Cargas, function (i, carga) {
                        _cargasExibidas[carga.Carga.Codigo] = ko.observable(carga);
                    });

                    dadosParaCarregar = ObterDoCache(codigoCarga);
                    LoadDataCarga(dadosParaCarregar);
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                }
            }, null, false);
        }
    }
}

function visualizarRotaFreteMapaClick(dadosCarga, e) {
    e.stopPropagation();

    executarReST("JanelaCarregamentoTransportador/ObterDadosRota", { carga: dadosCarga.Carga.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirModalMapaRotaFreteTransportador();

                if (_mapaRotaMontagemCargaTransportador == null)
                    _mapaRotaMontagemCargaTransportador = new Mapa("mapaRotaMotagemCargaTransportador", false);

                _mapaRotaMontagemCargaTransportador.limparMapa();

                setTimeout(function () {
                    _mapaRotaMontagemCargaTransportador.desenharPolilinha(retorno.Data.PolilinhaRota, true);
                    _mapaRotaMontagemCargaTransportador.adicionarMarcadorComPontosDaRota(retorno.Data.PontosDaRota);
                }, 500);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function AdicionarCarga(dados, indice) {
    var dadosCarga = processarRetorno(dados);
    var ko_containerCarga = new ContainerCargaTransportadorModel(dadosCarga);
    var cargaPrincipal = dadosCarga.Cargas[0];

    // Carrega dados de preenchimento em variável global
    _cargasExibidas[cargaPrincipal.Codigo] = cargaPrincipal.Dados;

    if (indice == null)
        _pesquisaJanelaCarregamentoTransportador.Cargas.push(ko_containerCarga);
    else
        _pesquisaJanelaCarregamentoTransportador.Cargas.replace(_pesquisaJanelaCarregamentoTransportador.Cargas()[indice], ko_containerCarga);
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirTermoConfirmacaoChegadaHorario(carga) {
    if (carga.HabilitarTermoChegadaHorario)
        exibirAlerta("", carga.TermoChegadaHorario, "De acordo", function () {
            auditarCargaTermoChegadaHorario(carga);
        });
}

function auditarCargaTermoChegadaHorario(carga) {
    executarReST("JanelaCarregamentoTransportador/AuditarCargaTermoChegadaHorario", { Carga: carga.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Acordado horário de carregamento sem atraso.");
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, retorno.Msg);
    });
}

function carregarCargas() {
    if (!_pesquisaJanelaCarregamentoTransportador.Requisicao.val()) {
        _pesquisaJanelaCarregamentoTransportador.Requisicao.val(true);

        var quantidadePorVez = 12;

        _dadosPesquisaCargas.Inicio = _pesquisaJanelaCarregamentoTransportador.Inicio.val();
        _dadosPesquisaCargas.Limite = quantidadePorVez;

        executarReST("JanelaCarregamentoTransportador/ConsultarCargas", _dadosPesquisaCargas, function (r) {
            if (r.Success) {
                _pesquisaJanelaCarregamentoTransportador.Total.val(r.Data.Total);
                _pesquisaJanelaCarregamentoTransportador.Inicio.val(_pesquisaJanelaCarregamentoTransportador.Inicio.val() + quantidadePorVez);

                $.each(r.Data.Cargas, function (i, carga) {
                    AdicionarCarga(carga);
                });

                setTimeout(function () {
                    $('[data-bs-toggle="popover"]').each(function () {
                        bootstrap.Popover.getOrCreateInstance(this, { placement: "top", html: true, trigger: "hover", container: "body", delay: { "show": 500, "hide": 100 } });
                    });
                }, 100);

            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, r.Msg);
            }

            _pesquisaJanelaCarregamentoTransportador.Requisicao.val(false);
        }, null, false);
    }
}

function solicitarNotasFiscais(dadosTransporteCarga) {
    executarReST("JanelaCarregamentoTransportador/SolicitarNotasFiscais", { Carga: dadosTransporteCarga.Carga }, function (retorno) {
        if (!retorno.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, retorno.Msg);

        if (!retorno.Data)
            return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);

        if (!retorno.Data.PercursoMDFeValido)
            return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Antes de solicitar as Notas Fiscais é necessário configurar o percurso válido para gerar o(s) MDF-e(s)");

        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Solicitação Realizada");
    });
}


function controlarExibicaoOpcoesDetalhesPorVeiculo(idOpcoes) {
    var $opcoesDetalhes = $(idOpcoes);

    if ($opcoesDetalhes.hasClass("auditoria-opcoes-exibir")) {
        $opcoesDetalhes.removeClass("auditoria-opcoes-exibir");
    }
    else {
        $opcoesDetalhes.addClass("auditoria-opcoes-exibir");

        $(window).one("mouseup", function (e) {
            e.stopPropagation();
            $opcoesDetalhes.removeClass("auditoria-opcoes-exibir");
            $opcoesDetalhes.show();
        });
    }
}

function exibirModalMapaRotaFreteTransportador() {
    Global.abrirModal('divModalMapaMotagemCargaTransportador');
    $("#divModalMapaMotagemCargaTransportador").one('hidden.bs.modal', function () { });
}

function processarRetorno(dados) {
    // Cada box mostra uma lista de cargas
    // A primeira é sempre a carga em questão
    // As demais são as cargas vinculadas (se tiver)
    // As cargas vinculadas são carregadas de forma assíncrona

    var cargaPrincipal = {
        Codigo: dados.Carga.Codigo,
        CodigoCargaEmbarcador: dados.Carga.Numero,
        Ref: dados.Carga.Codigo,
        Dados: ko.observable(dados),
        Load: ko.observable(true),
        Requisitando: ko.observable(false),
        CamposVisiveis: dados.CamposVisiveis
    };
    var cargas = [cargaPrincipal];

    var quantidadeVinculos = dados.CargasVinculadas.length;

    for (var i = 0; i < quantidadeVinculos; i++) {
        cargas.push({
            Codigo: dados.CargasVinculadas[i].Codigo,
            CodigoCargaEmbarcador: dados.CargasVinculadas[i].CodigoCargaEmbarcador,
            Ref: cargaPrincipal.Codigo,
            Dados: ko.observable({}),
            Load: ko.observable(false),
            Requisitando: ko.observable(false)
        })
    }

    return {
        Codigo: dados.Codigo,
        CargaPrincipal: new CargaModelTransportador({ Dados: cargaPrincipal.Dados(), HabilitarClick: cargaPrincipal.HabilitarClick }),
        Cargas: cargas,
        PossuiVinculos: quantidadeVinculos > 0
    }
}

function recarregarPesquisaCargas() {
    _cargasExibidas = {};
    _pesquisaJanelaCarregamentoTransportador.Total.val(0);
    _pesquisaJanelaCarregamentoTransportador.Inicio.val(0);
    _pesquisaJanelaCarregamentoTransportador.Cargas.removeAll();
    _dadosPesquisaCargas = RetornarObjetoPesquisa(_pesquisaJanelaCarregamentoTransportador);
    carregarCargas();
}

function registrarComponente() {
    if (ko.components.isRegistered('carga-detalhe-carregamento'))
        return;

    ko.components.register('carga-detalhe-carregamento', {
        viewModel: CargaModelTransportador,
        template: {
            element: 'carga-detalhe-template'
        }
    });
}

function visualizarCargaClick(e, sender) {

    e.VisualizarCarga.val(!e.VisualizarCarga.val());

    if (!e.VisualizarCarga.val()) {
        e.VisualizarCarga.icon("fa fa-eye");
        e.HabilitarClick.cssClass("clickable-area blurred");
        e.NaoTenhoInteresse.visible(false);
        e.TenhoInteresse.visible(false);
    }
    else {

        var data = { CodigoJanelaCarregamentoTransportador: e.CodigoJanelaCarregamentoTransportador.val() };

        if (e.JaVisualizouCarga.val()) {
            e.VisualizarCarga.icon("fa fa-eye-slash");
            e.HabilitarClick.cssClass("clickable-area unblurred");
            e.NaoTenhoInteresse.visible(true);
            e.TenhoInteresse.visible(true);
        }
        else {
            executarReST("JanelaCarregamentoTransportador/GerarRegistroVisualizacaoCarga", data, function (r) {
                if (r.Success) {
                    if (r.Data) {
                        e.VisualizarCarga.icon("fa fa-eye-slash");
                        e.HabilitarClick.cssClass("clickable-area unblurred");
                        e.NaoTenhoInteresse.visible(true);
                        e.TenhoInteresse.visible(true);

                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                }
            });
        }

    }

}
function retornarCssClassHabilitarClick(e) {
    if (e.GerarControleVisualizacaoTransportadorasTerceiros.val()) {

        if (e.Situacao.val() !== EnumSituacaoCargaJanelaCarregamentoTransportador.Disponivel)
            return "clickable-area";

        if (e.JaVisualizouCarga.val())
            return "clickable-area unblurred";
        else
            return "clickable-area blurred";

    }
    return "clickable-area";
}

function visibilidadeVisualizarCargaClick(e) {

    if (e.GerarControleVisualizacaoTransportadorasTerceiros.val()) {
        if (e.Situacao.val() === EnumSituacaoCargaJanelaCarregamentoTransportador.Disponivel) {
            return true;
        }

    }
    return false;
}
function iconVisualizarCargaClick(e) {
    if (e.GerarControleVisualizacaoTransportadorasTerceiros.val()) {
        if (e.Situacao.val() !== EnumSituacaoCargaJanelaCarregamentoTransportador.Disponivel)
            return false;

        if (e.JaVisualizouCarga.val())
            return true;
        else
            return false;

    }
    return false;
}
// #endregion Funções Privadas