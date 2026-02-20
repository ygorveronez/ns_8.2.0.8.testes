/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaCarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoOrdenacaoJanelaCarregamento.js" />
/// <reference path="AreaVeiculo.js" />
/// <reference path="CapacidadeCarregamentoDados.js" />
/// <reference path="ControleCorCarga.js" />
/// <reference path="CotacaoHistorico.js" />
/// <reference path="DetalheCarga.js" />
/// <reference path="HorarioCarregamento.js" />
/// <reference path="InteressadosCarga.js" />
/// <reference path="JanelaCarregamento.js" />
/// <reference path="LocalCarregamento.js" />
/// <reference path="Observacoes.js" />
/// <reference path="PossiveisVeiculos.js" />
/// <reference path="TipoTransportadorCarga.js" />
/// <reference path="TransportadoresOfertados.js" />
/// <reference path="../JanelaCarregamentoTransportador/JanelaCarregamentoTransportador.js" />

var PesquisaTabelaJanelaCarregamento = function () {
    this.CentroCarregamento = PropertyEntity({});
    this.DataCarregamento = PropertyEntity({ getType: typesKnockout.date, def: Global.DataAtual() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamento.Todas), def: EnumSituacaoCargaJanelaCarregamento.Todas });
    this.UFDestino = PropertyEntity({});

    //Filtros exceto os essenciais
    this.CodigoCargaEmbarcador = PropertyEntity({});
    this.CodigoPedidoEmbarcador = PropertyEntity({});
    this.Destinatario = PropertyEntity({});
    this.ExibirCargaQueNaoEstaoEmInicioViagem = PropertyEntity({ getType: typesKnockout.bool });
    this.ModeloVeicularCarga = PropertyEntity({});
    this.NumeroExp = PropertyEntity({});
    this.TipoOperacao = PropertyEntity({});
    this.Transportador = PropertyEntity({});
    this.Veiculo = PropertyEntity({});
    this.Destino = PropertyEntity({});
};

var TabelaJanelaCarregamento = function () {
    this.AreasVeiculos = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.AreasDeVeiculos, visible: ko.observable(false) });
    this.LiberarParaTransportadores = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.LiberarParaTransportadores, visible: ko.observable(false) });
    this.ObservacaoTransportador = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.ObservacaoAoTransportador, visible: ko.observable(false) });
};

var TabelaCarregamento = function () {
    this._idContainerTabela = "divListaCarregamento";
    this._dataAtual;
    this._gridTabelaCarregamento;
    this._pesquisaTabelaJanelaCarregamento;
    this._tabelaJanelaCarregamento;

    this._init();
};

TabelaCarregamento.prototype = {
    AdicionarOuAtualizarCarga: function (dadosJanelaCarregamento) {
        _RequisicaoIniciada = true;

        buscarCapacidadeCarregamentoDados();
        this._carregarCargas();
    },
    RemoverCarga: function (dadosJanelaCarregamento) {
        _RequisicaoIniciada = true;

        this._carregarCargas();
    },
    Destroy: function () {
        this._removerDroppable();
        this._removerEventos();

        $("#" + this._idContainerTabela).html("");
    },
    Load: function () {
        this._dataAtual = _dadosPesquisaCarregamento.DataCarregamento ? moment(_dadosPesquisaCarregamento.DataCarregamento, "DD/MM/YYYY") : moment();

        this._loadGridTabelaCarregamento();
        this._carregarCargas();
    },
    ObterData: function () {
        return this._dataAtual.format("DD/MM/YYYY");
    },
    _adicionarCarregamento: function (event, ui) {
        var idContainerDestino = event.target.id;
        var idContainerOrigem = "container-" + $(ui.draggable[0]).parent().parent()[0].id;

        if (idContainerOrigem !== idContainerDestino) {
            var codigo = $(ui.draggable[0]).data("codigo");

            this._alterarHorarioCarregamentoPorCodigo(codigo);
        }
    },
    _adicionarEventos: function () {
        var self = this;

        $("#" + this._idContainerTabela + "-prev").on("click", function () { self._carregarCargasDataCarregamentoAnterior(); });
        $("#" + this._idContainerTabela + "-next").on("click", function () { self._carregarCargasProximaDataCarregamento(); });
    },
    _alterarHorarioCarregamento: function (janelaCarregamentoSelecionada, confirmarNaoComparecimento) {
        if (_CONFIGURACAO_TMS.AlterarDataCarregamentoEDescarregamentoPorPeriodo)
            exibirModalAlteracaoHorarioCarregamentoPeriodoPorJanelaCarregamento(janelaCarregamentoSelecionada, this.ObterData());
        else
            exibirModalAlteracaoHorarioCarregamento(janelaCarregamentoSelecionada.Codigo, this.ObterData(), confirmarNaoComparecimento);
    },
    _alterarHorarioCarregamentoPorCodigo: function (codigoJanelaCarregamento) {
        if (_CONFIGURACAO_TMS.AlterarDataCarregamentoEDescarregamentoPorPeriodo)
            exibirModalAlteracaoHorarioCarregamentoPeriodoPorCentroCarregamento(codigoJanelaCarregamento, this.ObterData());
        else
            exibirModalAlteracaoHorarioCarregamento(codigoJanelaCarregamento, this.ObterData(), false);
    },
    _carregarCargas: function () {
        var self = this;

        _dadosPesquisaCarregamento.DataCarregamento = self._dataAtual.format('DD/MM/YYYY');

        self._pesquisaTabelaJanelaCarregamento.DataCarregamento.val(self._dataAtual.format('DD/MM/YYYY'));
        self._pesquisaTabelaJanelaCarregamento.CentroCarregamento.val(_dadosPesquisaCarregamento.CentroCarregamento);
        self._pesquisaTabelaJanelaCarregamento.Situacao.val(_dadosPesquisaCarregamento.Situacao);
        self._pesquisaTabelaJanelaCarregamento.UFDestino.val(_dadosPesquisaCarregamento.UFDestino);

        //Filtros exceto os essenciais
        self._pesquisaTabelaJanelaCarregamento.CodigoCargaEmbarcador.val(_dadosPesquisaCarregamento.CodigoCargaEmbarcador);
        self._pesquisaTabelaJanelaCarregamento.CodigoPedidoEmbarcador.val(_dadosPesquisaCarregamento.CodigoPedidoEmbarcador);
        self._pesquisaTabelaJanelaCarregamento.Destinatario.val(_dadosPesquisaCarregamento.Destinatario);
        self._pesquisaTabelaJanelaCarregamento.ExibirCargaQueNaoEstaoEmInicioViagem.val(_dadosPesquisaCarregamento.ExibirCargaQueNaoEstaoEmInicioViagem);
        self._pesquisaTabelaJanelaCarregamento.ModeloVeicularCarga.val(_dadosPesquisaCarregamento.ModeloVeicularCarga);
        self._pesquisaTabelaJanelaCarregamento.NumeroExp.val(_dadosPesquisaCarregamento.NumeroExp);
        self._pesquisaTabelaJanelaCarregamento.TipoOperacao.val(_dadosPesquisaCarregamento.TipoOperacao);
        self._pesquisaTabelaJanelaCarregamento.Transportador.val(_dadosPesquisaCarregamento.Transportador);
        self._pesquisaTabelaJanelaCarregamento.Veiculo.val(_dadosPesquisaCarregamento.Veiculo);
        self._pesquisaTabelaJanelaCarregamento.Destino.val(_dadosPesquisaCarregamento.Destino);

        $("#" + self._idContainerTabela + "-data-description").text(self._dataAtual.locale('pt-br').format('LL'));
        $("#" + self._idContainerTabela + "-day-of-week").text(self._dataAtual.locale('pt-br').format('dddd'));

        self._recarregarGridTabelaCarregamento();
    },
    _carregarCargasDataCarregamentoAnterior: function () {
        this._dataAtual.add(-1, 'days');

        buscarCapacidadeCarregamentoDados();
        this._carregarCargas();
    },
    _carregarCargasProximaDataCarregamento: function () {
        this._dataAtual.add(1, 'days');

        buscarCapacidadeCarregamentoDados();
        this._carregarCargas();
    },
    _controlarExibicaoMultiplasOpcoes: function () {
        var existemRegistrosSelecionados = this._gridTabelaCarregamento.ObterMultiplosSelecionados().length > 0;

        this._tabelaJanelaCarregamento.AreasVeiculos.visible(existemRegistrosSelecionados && _centroCarregamentoAtual.PermitirInformarAreaVeiculo);
        this._tabelaJanelaCarregamento.LiberarParaTransportadores.visible(existemRegistrosSelecionados);
        this._tabelaJanelaCarregamento.ObservacaoTransportador.visible(existemRegistrosSelecionados);
    },
    _exibirDetalhes: function (janelaCarregamentoSelecionada) {
        if (!_CONFIGURACAO_TMS.ExibirOpcaoLiberarParaTransportador && this._isPermitirLiberarParaTransportador(janelaCarregamentoSelecionada))
            this._liberarParaTransportador(janelaCarregamentoSelecionada);
        else {
            var carga = { Carga: { Codigo: janelaCarregamentoSelecionada.CodigoCarga } };

            ExibirDetalhesCarga(carga);
        }
    },
    _exibirDetalhesCarga: function (janelaCarregamentoSelecionada) {
        exibirDetalhesPedidos(janelaCarregamentoSelecionada.CodigoCarga);
    },
    _exibirInteressados: function (janelaCarregamentoSelecionada) {
        AbrirTelaInteressadosCarga(janelaCarregamentoSelecionada.CodigoCarga);
    },
    _exibirVisualizacoes: function (janelaCarregamentoSelecionada) {
        AbrirTelaVisualizacoesCarga(janelaCarregamentoSelecionada.CodigoCarga);
    },
    _exibirTransportadoresOfertados: function (janelaCarregamentoSelecionada) {
        visualizarTransportadoresOfertados(janelaCarregamentoSelecionada.Codigo);
    },
    _exibirVeiculos: function (janelaCarregamentoSelecionada) {
        var carga = {
            CentroCarregamento: { Codigo: _dadosPesquisaCarregamento.CentroCarregamento },
            ModeloVeiculo: { Codigo: janelaCarregamentoSelecionada.CodigoModeloVeicular },
            TipoCarga: { Codigo: janelaCarregamentoSelecionada.CodigoTipoCarga }
        };

        visualizarVeiculosDisponviveisClick(carga);
    },
    _imprimirCotacoes: function (janelaCarregamentoSelecionada) {
        executarDownload("JanelaCarregamento/ImprimirValoresCotacao", { Carga: janelaCarregamentoSelecionada.CodigoCarga, Confirmado: true })
    },
    _informarLocalCarregamento: function (codigoJanelaCarregamento) {
        exibirModalLocalCarregamento(codigoJanelaCarregamento);
    },
    _init: function () {
        var self = this;

        $("#" + self._idContainerTabela).css("min-height", "0");
        $("#" + self._idContainerTabela).addClass("fc fc-ltr fc-unthemed");
        $("#" + self._idContainerTabela).html(self._obterHtml());

        self._pesquisaTabelaJanelaCarregamento = new PesquisaTabelaJanelaCarregamento();

        self._tabelaJanelaCarregamento = new TabelaJanelaCarregamento();
        self._tabelaJanelaCarregamento.AreasVeiculos.eventClick = function () { exibirAreasVeiculos(self._gridTabelaCarregamento.ObterCodigosMultiplosSelecionados()); };
        self._tabelaJanelaCarregamento.LiberarParaTransportadores.eventClick = function () { exibirTipoTransportadorCargas(self._gridTabelaCarregamento.ObterCodigosMultiplosSelecionados()); };
        self._tabelaJanelaCarregamento.ObservacaoTransportador.eventClick = function () { exibirObservacaoTransportadores(self._gridTabelaCarregamento.ObterCodigosMultiplosSelecionados()); };

        KoBindings(self._tabelaJanelaCarregamento, "knockoutTabelaJanelaCarregamento");

        self._loadDroppable();
        self._adicionarEventos();
    },
    _liberarParaTransportador: function (janelaCarregamentoSelecionada) {
        var dados = {
            Codigo: janelaCarregamentoSelecionada.Codigo,
            ObservacaoTransportador: janelaCarregamentoSelecionada.ObservacaoTransportador,
            Transportador: { Codigo: janelaCarregamentoSelecionada.CodigoTransportador, Descricao: janelaCarregamentoSelecionada.Transportador }
        };

        exibirTipoTransportadorCarga(dados);
    },
    _liberarParaTransportadorPorData: function (janelaCarregamentoSelecionada) {
        var self = this;

        executarReST("JanelaCarregamento/EncaminharTransportadorRota", { Codigo: janelaCarregamentoSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaFoiLiberadaParaShare);
                    buscarCapacidadeCarregamentoDados();

                    _RequisicaoIniciada = true;

                    self._carregarCargas();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    },
    _salvarDataPrevisaoChegada: function (janelaCarregamentoSelecionada) {
        exibirModalDisponibilidadeVeiculo(janelaCarregamentoSelecionada.Codigo);
    },
    _alterarMotivoAtrasoCarregamento: function (janelaCarregamentoSelecionada) {
        exibirModalMotivoAtraso(janelaCarregamentoSelecionada.Codigo);
    },
    _visualizarOrdemColeta: function (janelaCarregamentoSelecionada) {
        obterDetalhesDaCarga(janelaCarregamentoSelecionada);
    },
    _isItemSoltadoAbaCargasExcedentes: function (ui, item) {
        var $abaCargasExcedentes = $('#tabCargasExcedentes');

        if ($abaCargasExcedentes.css("display") !== "none") {
            var offsetCargasExcedentes = $abaCargasExcedentes.offset();
            var offsetCargaSelecionada = ui.offset;

            offsetCargasExcedentes.right = $abaCargasExcedentes.width() + offsetCargasExcedentes.left;
            offsetCargasExcedentes.bottom = $abaCargasExcedentes.height() + offsetCargasExcedentes.top - item.height();
            offsetCargaSelecionada.left += (item.width() / 2);

            return (offsetCargaSelecionada.left >= offsetCargasExcedentes.left) && (offsetCargaSelecionada.left <= offsetCargasExcedentes.right) && (offsetCargaSelecionada.top >= offsetCargasExcedentes.top) && (offsetCargaSelecionada.top <= offsetCargasExcedentes.bottom);
        }

        return false;
    },
    _isPermitirBloquearCargaCotacao: function (janelaCarregamentoSelecionada) {
        return false; // this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.CargaLiberadaCotacao && !janelaCarregamentoSelecionada.CargaLiberadaCotacaoAutomaticamente && (janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.SemTransportador);
    },
    _isPermitirDescartarCargaCotacao: function (janelaCarregamentoSelecionada) {
        return false; // this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.CargaLiberadaCotacao && !janelaCarregamentoSelecionada.CargaLiberadaCotacaoAutomaticamente && ((janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador) || (janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.ProntaParaCarregamento));
    },
    _isPermitirBloquearCargaFilaCarregamento: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.PermitirBloquearFilaCarregamentoManualmente && (janelaCarregamentoSelecionada.Situacao != EnumSituacaoCargaJanelaCarregamento.ProntaParaCarregamento);
    },
    _isPermitirDesagendarCarga: function (janelaCarregamentoSelecionada) {
        if (!this._isPermitirEditar(janelaCarregamentoSelecionada) || janelaCarregamentoSelecionada.PreCarga)
            return false;

        return _CONFIGURACAO_TMS.PermitirDesagendarCargaJanelaCarregamento;
    },
    _isPermitirEditar: function (janelaCarregamentoSelecionada) {
        return janelaCarregamentoSelecionada.Editavel && (janelaCarregamentoSelecionada.Tipo == EnumTipoCargaJanelaCarregamento.Carregamento);
    },
    _isPermiteEncaminharTransportadorRota: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.PermiteLiberarParaShare;
    },
    _isPermitirExibirDetalhes: function (janelaCarregamentoSelecionada) {
        return janelaCarregamentoSelecionada.PreCarga == false;
    },
    _isPermitirExibirHistoricoCotacao: function (janelaCarregamentoSelecionada) {
        return janelaCarregamentoSelecionada.PossuiHistoricoCotacao;
    },
    _isPermitirExibirInteressados: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && (janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.SemTransportador) && (janelaCarregamentoSelecionada.Interessados > 0);
    },
    _isPermitirExibirVisualizacoes: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && (janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.SemTransportador) && (janelaCarregamentoSelecionada.Visualizacoes > 0);
    },
    _isPermitirExibirTransportadoresOfertados: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && (_centroCarregamentoAtual.ExibirTransportadoresOfertadosComMenorValorFreteTabela || _centroCarregamentoAtual.ExibirTransportadoresOfertadosPorPrioridadeDeRota);
    },
    _isPermitirExibirVeiculos: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && !_CONFIGURACAO_TMS.UtilizarFilaCarregamento && (janelaCarregamentoSelecionada.Situacao == EnumSituacaoCargaJanelaCarregamento.SemTransportador);
    },
    _isPermitirImprimirCotacao: function (janelaCarregamentoSelecionada) {
        return true;
    },
    _isPermitirInformarAreaVeiculo: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && _centroCarregamentoAtual.PermitirInformarAreaVeiculo;
    },
    _isPermitirInformarLocalCarregamento: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.PermitirInformarLocalCarregamento;
    },
    _isPermitirInformarObservacaoFluxoPatio: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.PermitirInformarObservacaoFluxoPatio;
    },
    _isPermitirInformarObservacaoGuarita: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.PermitirInformarObservacaoGuarita;
    },
    _isPermitirInformarObservacaoTransportador: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada);
    },
    _isPermitirLiberarCargaCotacao: function (janelaCarregamentoSelecionada) {
        return false;// this._isPermitirEditar(janelaCarregamentoSelecionada) && !janelaCarregamentoSelecionada.CargaLiberadaCotacao && !_CONFIGURACAO_TMS.LiberarCargaParaCotacaoAoLiberarParaTransportadores && (janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.SemTransportador);
    },
    _isPermitirLiberarCargaFilaCarregamento: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.PermitirLiberarFilaCarregamentoManualmente && (janelaCarregamentoSelecionada.Situacao != EnumSituacaoCargaJanelaCarregamento.ProntaParaCarregamento);
    },
    _isPermitirLiberarParaTransportador: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && (janelaCarregamentoSelecionada.Situacao == EnumSituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores) && ((janelaCarregamentoSelecionada.SituacaoCarga != EnumSituacoesCarga.Nova) || janelaCarregamentoSelecionada.ExigeNotaFiscalParaCalcularFrete);
    },
    _isPermitirSetarDataPrevisaoChegada: function (janelaCarregamentoSelecionada) {
        return true;
    },
    _isPermitirSetarMotivoAtrasoCarregamento: function (janelaCarregamentoSelecionada) {
        return true;
    },
    _loadDroppable: function () {
        var self = this;

        $("#container-grid-tabela-carregamento").droppable({
            drop: function (event, ui) { self._adicionarCarregamento(event, ui); },
            hoverClass: "ui-state-active backgroundDropHover"
        });
    },
    _loadGridTabelaCarregamento: function () {
        const self = this;
        const callbackItemSoltado = (_centroCarregamentoAtual.TipoOrdenacaoJanelaCarregamento == EnumTipoOrdenacaoJanelaCarregamento.Prioridade) ? function (ui) { self._removerCarregamentoParaCargasExcedentesComOrdenacao(ui); } : function (event, ui) { self._removerCarregamentoParaCargasExcedentesSemOrdenacao(event, ui); };
        const callbackOrdenacao = (_centroCarregamentoAtual.TipoOrdenacaoJanelaCarregamento == EnumTipoOrdenacaoJanelaCarregamento.Prioridade) ? function (retorno, reverterOrdenacao) { self._reordenar(retorno, reverterOrdenacao); } : undefined;
        const exibirPaginacao = false;
        const totalRegistrosPorPagina = 9999999;
        const opcaoDetalhes = { descricao: Localization.Resources.Cargas.Carga.Detalhes, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._exibirDetalhes(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirExibirDetalhes(janelaCarregamentoSelecionada); } };
        const opcaoDetalhesCarga = { descricao: Localization.Resources.Cargas.Carga.DetalhesDaCarga, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._exibirDetalhesCarga(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirExibirDetalhes(janelaCarregamentoSelecionada); } };
        const opcaoAlterarHora = { descricao: (_CONFIGURACAO_TMS.AlterarDataCarregamentoEDescarregamentoPorPeriodo ? Localization.Resources.Cargas.Carga.AlterarDataDoAgendamento : Localization.Resources.Cargas.Carga.AlterarHora), id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._alterarHorarioCarregamento(janelaCarregamentoSelecionada, _centroCarregamentoAtual && _centroCarregamentoAtual.Configuracao.PermiteMarcarCargaComoNaoComparecimento); }, tamanho: "10", icone: "" };
        const opcaoAreaVeiculo = { descricao: Localization.Resources.Cargas.Carga.AreasDeVeiculos, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { exibirAreaVeiculo(janelaCarregamentoSelecionada.Codigo)(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirInformarAreaVeiculo(janelaCarregamentoSelecionada); } };
        const opcaoBloquearCargaCotacao = { descricao: Localization.Resources.Cargas.Carga.BloquearParaCotacao, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { bloquearCargaCotacao(janelaCarregamentoSelecionada.Codigo); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirBloquearCargaCotacao(janelaCarregamentoSelecionada); } };
        const opcaoBloquearCargaFilaCarregamento = { descricao: Localization.Resources.Cargas.Carga.BloquearParaFilaDeCarregamento, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { bloquearCargaFilaCarregamento(janelaCarregamentoSelecionada.Codigo); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirBloquearCargaFilaCarregamento(janelaCarregamentoSelecionada); } };
        const opcaoDesagendarCarga = { descricao: Localization.Resources.Cargas.Carga.DesagendarCarga, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { desagendarCarga(janelaCarregamentoSelecionada.CodigoCarga); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirDesagendarCarga(janelaCarregamentoSelecionada); } };
        const opcaoDescartarCargaCotacao = { descricao: Localization.Resources.Cargas.Carga.DescartarCotacao, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { descartarCargaCotacao(janelaCarregamentoSelecionada.Codigo); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirDescartarCargaCotacao(janelaCarregamentoSelecionada); } };
        const opcaoHistoricoCotacao = { descricao: Localization.Resources.Cargas.Carga.HistoricoDaCotacao, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { visualizarHistoricoCotacao(janelaCarregamentoSelecionada.Codigo); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirExibirHistoricoCotacao(janelaCarregamentoSelecionada); } };
        const opcaoInformarLocalCarregamento = { descricao: Localization.Resources.Cargas.Carga.InformarLocalDeCarregamento, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._informarLocalCarregamento(janelaCarregamentoSelecionada.Codigo); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirInformarLocalCarregamento(janelaCarregamentoSelecionada); } };
        const opcaoInteressados = { descricao: Localization.Resources.Cargas.Carga.Interessados, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._exibirInteressados(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirExibirInteressados(janelaCarregamentoSelecionada); } };
        const opcaoVisualizacoes = { descricao: Localization.Resources.Cargas.Carga.Visualizacoes, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._exibirVisualizacoes(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirExibirVisualizacoes(janelaCarregamentoSelecionada); } };
        const opcaoLiberarCargaCotacao = { descricao: Localization.Resources.Cargas.Carga.LiberarParaCotacao, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { liberarCargaCotacao(janelaCarregamentoSelecionada.Codigo); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirLiberarCargaCotacao(janelaCarregamentoSelecionada); } };

        const opcaoLiberarCargaFilaCarregamento = { descricao: Localization.Resources.Cargas.Carga.LiberarParaFilaDeCarregamento, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { liberarCargaFilaCarregamento(janelaCarregamentoSelecionada.Codigo); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirLiberarCargaFilaCarregamento(janelaCarregamentoSelecionada); } };
        const opcaoLiberarParaTransportador = { descricao: Localization.Resources.Cargas.Carga.LiberarParaTransportadores, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._liberarParaTransportador(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return _CONFIGURACAO_TMS.ExibirOpcaoLiberarParaTransportador && self._isPermitirLiberarParaTransportador(janelaCarregamentoSelecionada) } };
        const opcaoObservacaoFluxoPatio = { descricao: Localization.Resources.Cargas.Carga.ObservacaoDoFluxoDePatio, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { exibirObservacaoFluxoPatio(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirInformarObservacaoFluxoPatio(janelaCarregamentoSelecionada); } };
        const opcaoObservacaoGuarita = { descricao: Localization.Resources.Cargas.Carga.ObservacaoDaGuarita, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { exibirObservacaoGuarita(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirInformarObservacaoGuarita(janelaCarregamentoSelecionada); } };
        const opcaoObservacaoTransportador = { descricao: Localization.Resources.Cargas.Carga.ObservacaoAoTransportador, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { exibirObservacaoTransportador(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirInformarObservacaoTransportador(janelaCarregamentoSelecionada); } };
        const opcaoTransportadoresOfertados = { descricao: Localization.Resources.Cargas.Carga.TransportadoresOfertados, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._exibirTransportadoresOfertados(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirExibirTransportadoresOfertados(janelaCarregamentoSelecionada); } };
        const opcaoVeiculos = { descricao: Localization.Resources.Cargas.Carga.Veiculos, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._exibirVeiculos(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirExibirVeiculos(janelaCarregamentoSelecionada); } };
        const opcaoImprimirCotacoes = { descricao: Localization.Resources.Cargas.Carga.ImprimirCotacoes, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._imprimirCotacoes(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirImprimirCotacao(janelaCarregamentoSelecionada); } };
        const opcaoEncaminharTransportadorRota = { descricao: Localization.Resources.Cargas.Carga.EncaminharTransportadorRota, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._liberarParaTransportadorPorData(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermiteEncaminharTransportadorRota(janelaCarregamentoSelecionada); } };
        const opcaoDisponibilidadeVeiculo = { descricao: Localization.Resources.Cargas.Carga.DisponibilidadeDoVeiculo, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._salvarDataPrevisaoChegada(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirSetarDataPrevisaoChegada(janelaCarregamentoSelecionada); } };
        const opcaoInserirMotivoAtraso = { descricao: Localization.Resources.Cargas.Carga.AlterarMotivoAtrasoCarregamento, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._alterarMotivoAtrasoCarregamento(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "", visibilidade: function (janelaCarregamentoSelecionada) { return self._isPermitirSetarMotivoAtrasoCarregamento(janelaCarregamentoSelecionada); } };
        const opcaoVisualizarOrdemColeta = { descricao: Localization.Resources.Cargas.Carga.VisualizarOrdemColeta, id: guid(), evento: "onclick", metodo: function (janelaCarregamentoSelecionada) { self._visualizarOrdemColeta(janelaCarregamentoSelecionada); }, tamanho: "10", icone: "" };

        const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [opcaoLiberarParaTransportador, opcaoDetalhes, opcaoDetalhesCarga, opcaoAlterarHora, opcaoAreaVeiculo, opcaoBloquearCargaCotacao, opcaoBloquearCargaFilaCarregamento, opcaoDesagendarCarga, opcaoDescartarCargaCotacao, opcaoHistoricoCotacao, opcaoInformarLocalCarregamento, opcaoInteressados, opcaoLiberarCargaCotacao, opcaoLiberarCargaFilaCarregamento, opcaoObservacaoTransportador, opcaoObservacaoFluxoPatio, opcaoTransportadoresOfertados, opcaoVeiculos, opcaoImprimirCotacoes, opcaoEncaminharTransportadorRota, opcaoObservacaoGuarita, opcaoDisponibilidadeVeiculo, opcaoInserirMotivoAtraso, opcaoVisualizacoes, opcaoVisualizarOrdemColeta], tamanho: 7, };

        const multiplaEscolha = {
            basicGrid: null,
            eventos: {},
            selecionados: new Array(),
            naoSelecionados: new Array(),
            callbackNaoSelecionado: function () { self._controlarExibicaoMultiplasOpcoes(); },
            callbackSelecionado: function () { self._controlarExibicaoMultiplasOpcoes(); },
            callbackSelecionarTodos: function () { self._controlarExibicaoMultiplasOpcoes(); },
            classeSelecao: "item-selecionado",
            somenteLeitura: false
        };

        const configExportExcel = {
            url: "JanelaCarregamento/ExportarPesquisaInformacoesCargas",
            btnText: "Exportar Excel",
            titulo: "Janela Carregamento"
        }

        self._gridTabelaCarregamento = new GridView("grid-tabela-carregamento", "JanelaCarregamento/PesquisaInformacoesCargas", self._pesquisaTabelaJanelaCarregamento, menuOpcoes, null, totalRegistrosPorPagina, null, false, callbackItemSoltado, multiplaEscolha, totalRegistrosPorPagina, null, configExportExcel, callbackOrdenacao, exibirPaginacao);

        self._gridTabelaCarregamento.SetPermitirEdicaoColunas(true);
        self._gridTabelaCarregamento.SetSalvarPreferenciasGrid(true);
    },
    _obterHtml: function () {
        var html = '';

        html += '<div>';
        html += '    <div class="fc-toolbar">';
        html += '        <div class="fc-left">';
        html += '            <h2 id="' + this._idContainerTabela + '-data-description"></h2>';
        html += '        </div>';
        html += '        <div class="fc-right">';
        html += '            <div class="fc-button-group">';
        html += '                <button type="button" class="fc-prev-button fc-button fc-state-default fc-corner-left" id="' + this._idContainerTabela + '-prev">';
        html += '                   <span class="fc-icon fc-icon-left-single-arrow"></span>';
        html += '                </button>';
        html += '                <button type="button" class="fc-next-button fc-button fc-state-default fc-corner-right" id="' + this._idContainerTabela + '-next">';
        html += '                    <span class="fc-icon fc-icon-right-single-arrow"></span>';
        html += '                </button>';
        html += '            </div>';
        html += '        </div>';
        html += '        <div class="fc-center">';
        html += '            </div><div class="fc-clear">';
        html += '        </div>';
        html += '    </div>';
        html += '    <div class="fc-view-container">';
        html += '        <div class="fc-view fc-agendaDay-view fc-agenda-view">';
        html += '            <table>';
        html += '                <thead class="fc-head">';
        html += '                    <tr>';
        html += '                        <td class="fc-head-container fc-widget-header">';
        html += '                            <div class="fc-row fc-widget-header">';
        html += '                                <table>';
        html += '                                    <thead>';
        html += '                                        <tr><th class="fc-day-header fc-widget-header fc-tue" id="' + this._idContainerTabela + '-day-of-week"></th></tr>';
        html += '                                    </thead>';
        html += '                                </table>';
        html += '                            </div>';
        html += '                        </td>';
        html += '                    </tr>';
        html += '                </thead>';
        html += '                <tbody class="fc-body">';
        html += '                    <tr>';
        html += '                        <td class="fc-widget-content fc-widget-content-table" id="knockoutTabelaJanelaCarregamento">';
        html += '                            <div class="widget-body no-padding-bottom" id="container-grid-tabela-carregamento">';
        html += '                                <table width="100%" class="table table-bordered table-hover table-condensed tableCursorMove" id="grid-tabela-carregamento" cellspacing="0"></table>';
        html += '                            </div>';
        html += '                            <div class="d-flex justify-content-end">';
        html += '                                <div class="btn-group dropup" data-bind="visible: LiberarParaTransportadores.visible() || ObservacaoTransportador.visible() || AreasVeiculos.visible()">';
        html += '                                    <button type="button" class="btn btn-warning waves-effect waves-themed rounded" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
        html += '                                        <i class="fal fa-list"></i> <span>Opções</span>';
        html += '                                    </button>';
        html += '                                    <div class="dropdown-menu">';
        html += '                                        <a class="dropdown-item" data-bind="text: AreasVeiculos.text, click: AreasVeiculos.eventClick, attr: { id: AreasVeiculos.id }, visible: AreasVeiculos.visible"></a>';
        html += '                                        <a class="dropdown-item" data-bind="text: LiberarParaTransportadores.text, click: LiberarParaTransportadores.eventClick, attr: { id: LiberarParaTransportadores.id }, visible: LiberarParaTransportadores.visible"></a>';
        html += '                                        <a class="dropdown-item" data-bind="text: ObservacaoTransportador.text, click: ObservacaoTransportador.eventClick, attr: { id: ObservacaoTransportador.id }, visible: ObservacaoTransportador.visible"></a>';
        html += '                                    </div>';
        html += '                                </div>';
        html += '                            </div>';
        html += '                        </td>';
        html += '                    </tr>';
        html += '                </tbody>';
        html += '            </table>';
        html += '        </div>';
        html += '    </div>';
        html += '</div>';

        return html;
    },
    _recarregarGridTabelaCarregamento: function () {
        var self = this;

        self._gridTabelaCarregamento.AtualizarRegistrosSelecionados([]);
        self._gridTabelaCarregamento.CarregarGrid(function () {
            _RequisicaoIniciada = false;
            self._controlarExibicaoMultiplasOpcoes();
        });
    },
    _removerCarregamentoParaCargasExcedentes: function (codigoJanelaCarregamento) {
        var self = this;

        var _confirmarNaoComparecimento = function (setarComoNaoComparecimento) {
            executarReST("JanelaCarregamento/MandarCargaExcedentesCarregamento", { JanelaCarregamento: codigoJanelaCarregamento, NaoComparecimento: setarComoNaoComparecimento }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaRetornouAsExcedentes);
                        RecarregarCargasExcedentes();
                        buscarCapacidadeCarregamentoDados();

                        _RequisicaoIniciada = true;

                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            });
        };

        executarReST("JanelaCarregamento/VerificarPossibilidadeModificacaoJanela", { JanelaCarregamento: codigoJanelaCarregamento, Data: Global.DataHoraAtual() }, function (retornoVerificacao) {
            if (!retornoVerificacao.Success)
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retornoVerificacao.Msg);

            if (retornoVerificacao.Data.PossibilidadeNoShow)
                exibirConfirmacao(Localization.Resources.Cargas.Carga.NoShow, Localization.Resources.Cargas.Carga.DesejaMarcarCargaComoNoShow, function () { _confirmarNaoComparecimento(true) }, function () { _confirmarNaoComparecimento(false) });
            else
                _confirmarNaoComparecimento(false);
        });
    },
    _removerCarregamentoParaCargasExcedentesComOrdenacao: function (ui) {
        if (this._isItemSoltadoAbaCargasExcedentes(ui, ui.item))
            this._removerCarregamentoParaCargasExcedentes(ui.item[0].id);
    },
    _removerCarregamentoParaCargasExcedentesSemOrdenacao: function (event, ui) {
        if (this._isItemSoltadoAbaCargasExcedentes(ui, ui.helper))
            this._removerCarregamentoParaCargasExcedentes(event.target.id);
    },
    _removerDroppable: function () {
        $("#container-grid-tabela-carregamento").droppable("destroy");
    },
    _removerEventos: function () {
        $("#" + this._idContainerTabela + "-prev").off("click", "**");
        $("#" + this._idContainerTabela + "-next").off("click", "**");
    },
    _reordenar: function (retornoOrdenacao, reverterOrdenacao) {
        var self = this;
        var dados = {
            codigo: retornoOrdenacao.itemReordenado.idLinha,
            NovaPrioridade: retornoOrdenacao.itemReordenado.posicaoAtual
        };

        executarReST("JanelaCarregamento/AlterarPrioridade", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AlteradaPrioridadeDaCargaComSucesso);

                    _RequisicaoIniciada = true;

                    self._carregarCargas();
                }
                else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                    reverterOrdenacao();
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                reverterOrdenacao();
            }
        });
    }
}
