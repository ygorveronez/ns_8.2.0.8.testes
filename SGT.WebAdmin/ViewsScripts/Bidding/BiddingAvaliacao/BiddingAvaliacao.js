/// <reference path="Etapas.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../Enumeradores/EnumStatusBiddingTipoOferta.js" />
/// <reference path="../../Enumeradores/EnumTipoLanceBidding.js" />
/// <reference path="../../Enumeradores/EnumBiddingOfertaSituacao.js" />
/// <reference path="../../Enumeradores/EnumStatusBidding.js" />
/// <reference path="../../Enumeradores/EnumMes.js" />
/// <reference path="../../Enumeradores/EnumTipoTransportadorBidding.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoBiddingConvite.js" />

//#region Variáveis Globais

var _pesquisaBiddingConvite;
var _duvidas;
var _CRUDduvidas;
var _bidding;
var _biddingOfertas;
var _biddingOfertasComponente;
var _biddingResumo;
var _biddingConvites;
var _biddingChecklist;
var _biddingChecklistRespostas;
var _biddingOfertaDetalhes;
var _biddingFechamento;
var _biddingRankingOfertas;
var _biddingRankingCherryPickingOfertas;
var _titularidadeOferta;
var _selecionarVencedores;
var _selecionarVencedorUnicoParaTodasRotas;
var _modalOfertasComponente;
var _respostaAnexos;
var _gridBiddingConvite;
var _gridConvites;
var _gridChecklist;
var _gridAnexos;
var _gridOfertas;
var _gridOfertasComponente;
var _gridRankingOfertas;
var _gridRankingCherryPickingOfertas;
var _gridModalOfertasComponente;
var _gridParaSelecionarVencedores;
var _gridParaSelecionarVencedorUnicoParaTodasRotas;
var _gridEquipamento;
var _gridPorcentagem;
var _gridFrotaFixaKm;
var _gridViagemAdicional;
var _gridFrotaFixaFranquia;
var _gridViagemEntregaAjudante;
var _gridFechamento;
var _gridFretePorPeso;
var _gridFretePorViagem;
var _gridFretePorCapacidade;
var _todosResultadosOfertas = [];
var _gridRankingCherryPickingOfertasDados = [];
var _gridRankingOfertasDados = [];
var _timerBiddingConvite = null;
var _timerBiddingCheckList = null;
var _timerBiddingOferta = null;
var _gridOfertasFixa;
var $container;
var $resumo;
var _tableOfertas = null;
var _tableOfertasFixa = null;
var _tokenAcessoAvaliacao;
var _linhaTransportadorSimulado;
var _buscarPorCodigo = true;
var _visibilidadeCriarTabelaFreteCliente = true;

//#endregion

//#region Pesquisa
var PesquisaBiddingConvite = function () {
    this.Descricao = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: "Descrição:" });
    this.DataInicio = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.date, text: "Data Início:" });
    this.DataLimite = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.date, text: "Data Limite:" });
    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitante: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Comprador = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "Comprador: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: PesquisarConvite, type: types.event, text: "Pesquisar", visible: ko.observable(true), idGrid: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.NumeroBidding = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.int, text: "Número:" });
    this.Situacao = PropertyEntity({
        text: "Etapa:",
        val: ko.observable([EnumStatusBidding.Aguardando, EnumStatusBidding.Checklist, EnumStatusBidding.Fechamento, EnumStatusBidding.Ofertas]),
        def: [EnumSituacaoBiddingConvite.Aguardando, EnumSituacaoBiddingConvite.Checklist, EnumSituacaoBiddingConvite.Fechamento, EnumSituacaoBiddingConvite.Ofertas],
        options: EnumStatusBidding.obterOpcoes(), visible: ko.observable(true), getType: typesKnockout.selectMultiple
    });
    this.TipoBidding = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "Transportador: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.FiliaisParticipante = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "Filiais Participantes:", idBtnSearch: guid(), enable: ko.observable(true), required: false });
};
//#endregion

//#region Quadro Resumo
var Bidding = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
};

var BiddingResumo = function () {
    this.DataLimite = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.date });
    this.Codigo = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string });
    this.ExibirResumo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.CodigoRota = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.CodigoModeloVeicular = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.CodigoFilialParticipante = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.CodigoOrigem = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.CodigoDestino = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.CodigoMesorregiaoOrigem = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.CodigoMesorregiaoDestino = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.RotaDestino = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.RotaOrigem = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.ClienteDestino = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.ClienteOrigem = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.EstadoDestino = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.EstadoOrigem = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.PaisDestino = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.PaisOrigem = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.CEPOrigem = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.string });
    this.CEPDestino = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.string });
    this.QuantidadeEntregas = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.QuantidadeAjudantes = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.QuantidadeViagensAno = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.CodigoRegiaoDestino = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.CodigoRegiaoOrigem = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.CarregarGrid = PropertyEntity({ val: ko.observable(false), required: false, getType: typesKnockout.bool });
    this.GridRankingCherryPickingOferas = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string });
    this.GridRankingOferas = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string });
    this.GridOfertas = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string });
    this.Filtros = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string });
    this.OfertasComponente = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string });
    this.CodigoBiddingOferta = PropertyEntity({ codEntity: ko.observable(0), getType: typesKnockout.int });
};
//#endregion

//#region Aba Convites
var BiddingConvite = function () {
    this.Convites = PropertyEntity({ type: types.local });

    this.TempoRestante = PropertyEntity({ getType: typesKnockout.map, val: ko.observable("") });
};

//#endregion

//#region Aba Checklist
var BiddingChecklist = function () {
    this.Checklist = PropertyEntity({ type: types.local });

    this.TempoRestante = PropertyEntity({ getType: typesKnockout.map, val: ko.observable("") });
};

var BiddingChecklistResposta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Respostas = ko.observableArray([]);
    this.Observacao = PropertyEntity({ val: ko.observable(""), required: true, getType: typesKnockout.string, text: "*Observação:", maxlength: 300 });
    this.Aprovar = PropertyEntity({ eventClick: AprovarChecklistClick, type: types.event, text: "Aprovar Participação", visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: RejeitarChecklistClick, type: types.event, text: "Rejeitar Participação", visible: ko.observable(true) });
};

var Resposta = function (resposta) {
    this.Pergunta = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.Resposta = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.Observacao = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ getType: typesKnockout.map, val: ko.observableArray([]) });
    this.VerAnexos = PropertyEntity({ eventClick: VerAnexosChecklistClick, type: types.event, text: "Ver Anexos", visible: ko.observable(true) });

    PreencherObjetoKnout(this, { Data: resposta });
};

var RespostaAnexos = function () {
    this.Anexos = PropertyEntity({ type: types.local });
};
//#endregion

//#region Aba Ofertas
var BiddingOfertas = function () {
    this.Ofertas = PropertyEntity({ type: types.local });
    this.Rotas = PropertyEntity({ val: ko.observable(""), text: "Rota:", options: ko.observableArray([]) });
    this.Rotas.val.subscribe(FiltroOfertas, this);
    this.ModeloVeicular = PropertyEntity({ val: ko.observable(""), text: "Modelo Veicular:", options: ko.observableArray([]) });
    this.ModeloVeicular.val.subscribe(FiltroOfertas, this);
    this.Filial = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Filial:", options: ko.observableArray([]) });
    this.Filial.val.subscribe(FiltroOfertas, this);
    this.Origem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Cidade Origem:", options: ko.observableArray([]) });
    this.Origem.val.subscribe(FiltroOfertas, this);
    this.Destino = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Cidade Destino:", options: ko.observableArray([]) });
    this.Destino.val.subscribe(FiltroOfertas, this);
    this.MesorregiaoDestino = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Mesorregião Destino:", options: ko.observableArray([]) });
    this.MesorregiaoDestino.val.subscribe(FiltroOfertas, this);
    this.MesorregiaoOrigem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Mesorregião Origem:", options: ko.observableArray([]) });
    this.MesorregiaoOrigem.val.subscribe(FiltroOfertas, this);
    this.QuantidadeEntregas = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Quantidade de Entregas:", options: ko.observableArray([]) });
    this.QuantidadeEntregas.val.subscribe(FiltroOfertas, this);
    this.QuantidadeAjudantes = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Quantidade de Ajudantes por Veículo:", options: ko.observableArray([]) });
    this.QuantidadeAjudantes.val.subscribe(FiltroOfertas, this);
    this.QuantidadeViagensAno = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Quantidade de Viagens por Ano:", options: ko.observableArray([]) });
    this.QuantidadeViagensAno.val.subscribe(FiltroOfertas, this);
    this.RegiaoDestino = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Região Destino:", options: ko.observableArray([]) });
    this.RegiaoDestino.val.subscribe(FiltroOfertas, this);
    this.RegiaoOrigem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Região Origem:", options: ko.observableArray([]) });
    this.RegiaoOrigem.val.subscribe(FiltroOfertas, this);
    this.RotaDestino = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Rota Destino:", options: ko.observableArray([]) });
    this.RotaDestino.val.subscribe(FiltroOfertas, this);
    this.RotaOrigem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Rota Origem:", options: ko.observableArray([]) });
    this.RotaOrigem.val.subscribe(FiltroOfertas, this);
    this.ClienteDestino = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Cliente Destino:", options: ko.observableArray([]) });
    this.ClienteDestino.val.subscribe(FiltroOfertas, this);
    this.ClienteOrigem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Cliente Origem:", options: ko.observableArray([]) });
    this.ClienteOrigem.val.subscribe(FiltroOfertas, this);
    this.EstadoDestino = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Estado Destino:", options: ko.observableArray([]) });
    this.EstadoDestino.val.subscribe(FiltroOfertas, this);
    this.EstadoOrigem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Estado Origem:", options: ko.observableArray([]) });
    this.EstadoOrigem.val.subscribe(FiltroOfertas, this);
    this.PaisDestino = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Pais Destino:", options: ko.observableArray([]) });
    this.PaisDestino.val.subscribe(FiltroOfertas, this);
    this.PaisOrigem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Pais Origem:", options: ko.observableArray([]) });
    this.PaisOrigem.val.subscribe(FiltroOfertas, this);
    this.CEPOrigem = PropertyEntity({ text: "CEP Orgiem:", val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.CEPOrigem.val.subscribe(FiltroOfertas, this);
    this.CEPDestino = PropertyEntity({ text: "CEP Destino:", val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.CEPDestino.val.subscribe(FiltroOfertas, this);

    this.TempoRestante = PropertyEntity({ getType: typesKnockout.map, val: ko.observable("") });

    this.SelecionarVencedoresSituacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), def: false, text: "Selecionar Todos", visible: ko.observable(true) });
    this.ProporRodadaSelecionados = PropertyEntity({ eventClick: ProporMultiplosSelecionadosClick, type: types.event, text: "Propor Novas Rodadas", idGrid: guid(), visible: ko.observable(false) });
    this.SelecionarVencedores = PropertyEntity({ eventClick: SelecionarVencedoresClick, type: types.event, text: ko.observable("Selecionar Vencedores"), idGrid: guid(), visible: ko.observable(true) });
    this.SelecionarVencendorUnico = PropertyEntity({ eventClick: SelecionarVencendorUnicoClick, type: types.event, text: ko.observable("Selecionar Vencedor Único"), idGrid: guid(), visible: ko.observable(false) });
    this.ExportarGridOfertas = PropertyEntity({ type: types.event, text: "Exportar Excel (Dados Salvos)", visible: ko.observable(true), eventClick: ExportarGridOfertasClick });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({ eventClick: limparFiltrosConsultaBidding, type: types.event, text: "Limpar Filtros", idGrid: guid(), visible: ko.observable(true) });
};

var BiddingOfertasComponente = function () {
    this.OfertasComponente = PropertyEntity({ type: types.local });
    this.OfertaComponente = PropertyEntity({ visible: ko.observable(false) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var BiddingRankingCherryPickingOfertas = function () {

    this.OfertaRanking = PropertyEntity({ visible: ko.observable(false), idGrid: guid(), });
    this.Transportador = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Transportador: ", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.CodigosTransportadores = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable([]), visible: false })
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var BiddingRankingOfertas = function () {

    this.OfertaRanking = PropertyEntity({ visible: ko.observable(false), idGrid: guid(), });
    this.Transportador = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Transportador: ", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.CodigosTransportadores = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable([]), visible: false })
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};
var BiddingOfertaDetalhes = function () {
    this.Transportador = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.NumeroViagens = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.PesoTotal = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.Volume = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.ValorTotal = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.ValorMedioPorViagem = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.QuantidadeCargaNecessaria = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.ValorTotalProjetado = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.ValorPorViagemProjetado = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.Variacao = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });

    this.Rodada = PropertyEntity({ val: ko.observable("") });
    this.Codigo = PropertyEntity({ val: ko.observable("") });
    this.Equipamento = PropertyEntity({ type: types.map });
    this.FrotaFixaKm = PropertyEntity({ type: types.map });
    this.FretePorPeso = PropertyEntity({ type: types.map });
    this.FretePorCapacidade = PropertyEntity({ type: types.map });
    this.FretePorViagem = PropertyEntity({ type: types.map });
    this.ViagemEntregaAjudante = PropertyEntity({ type: types.map });
    this.PorcentagemSobreNota = PropertyEntity({ type: types.map });
    this.ViagemAdicional = PropertyEntity({ type: types.map });
    this.FrotaFixaFranquia = PropertyEntity({ type: types.map });
    this.Mes = PropertyEntity({ val: ko.observable(""), text: "Mês:", options: EnumMes.obterOpcoes() });
    this.Oferta = PropertyEntity({ val: ko.observable(""), text: "Oferta:", options: ko.observableArray([]) });
    this.Ano = PropertyEntity({ val: ko.observable(""), text: "Ano:", options: ko.observable(""), maxlength: 4, getType: typesKnockout.string, required: true });
    this.Aceitar = PropertyEntity({ eventClick: AceitarOfertaClick, type: types.event, text: "Aceitar Oferta", visible: ko.observable(true) });
    this.Comparar = PropertyEntity({ eventClick: CompararOfertaClick, type: types.event, text: "Comparar", visible: ko.observable(true) });
    this.Target = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Valor Alvo", required: true });
    this.ProporRodada = PropertyEntity({ eventClick: ProporRodadaClick, type: types.event, text: "Propor Nova Rodada", visible: ko.observable(true) });
};
//#endregion

//#region Aba Fechamento
var BiddingFechamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Vencedores = PropertyEntity({ type: types.local });
    this.Rotas = PropertyEntity({ val: ko.observable(""), text: "Rota:", options: ko.observableArray([]) });
    this.Rotas.val.subscribe(PesquisarResultados, this);
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.CriarTabelaFrete = PropertyEntity({ eventClick: criarTabelaFreteClick, type: types.event, text: "Criar Tabela Frete", visible: ko.observable(false) });
};
//#endregion

//#region Dúvidas
var Duvidas = function () {
    this.Duvidas = PropertyEntity({ val: ko.observableArray([]) });
};

var Duvida = function (duvida) {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.Data = PropertyEntity({ getType: typesKnockout.string });
    this.Pergunta = PropertyEntity({ getType: typesKnockout.string });
    this.Resposta = PropertyEntity({ getType: typesKnockout.string });
    this.Responder = PropertyEntity({ val: ko.observable(""), required: true, getType: typesKnockout.text, text: "*Resposta:" });
    this.EnviarResposta = PropertyEntity({ eventClick: EnviarRespostaClick, type: types.event, text: "Enviar Resposta", visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });

    PreencherObjetoKnout(this, { Data: duvida });
};

var CRUDDuvidas = function () {
    this.Duvidas = PropertyEntity({ eventClick: MostrarModalDuvidas, type: types.event, text: "Dúvidas", visible: ko.observable(true) });
    this.FecharBidding = PropertyEntity({ eventClick: FecharBidding, type: types.event, text: "Fechar Bidding", visible: ko.observable(true) });
    this.FinalizarEtapa = PropertyEntity({ eventClick: FinalizarEtapaClick, type: types.event, text: "Finalizar Etapa", visible: ko.observable(false) });
    this.NotificarInteressados = PropertyEntity({ eventClick: NotificarInteressadosClick, type: types.event, text: "Notificar Interessados", visible: ko.observable(false) });
    this.RecarregarBidding = PropertyEntity({ eventClick: RecarregarBiddingClick, type: types.event, text: "Recarregar Bidding", visible: ko.observable(false) });
};
//#endregion

var TitularidadeOferta = function () {
    this.CodigoOferta = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigosOfertas = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable([]), visible: false })
    this.TipoTransportador = PropertyEntity({ val: ko.observable(EnumTipoTransportadorBidding.Titular), def: EnumTipoTransportadorBidding.Titular, text: "Tipo Transportador:", options: EnumTipoTransportadorBidding.obterOpcoes() });

    this.Salvar = PropertyEntity({ eventClick: salvarTitularidadeClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
};

var SelecionarVencedores = function () {
    this.CodigoRota = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.GridVencedores = PropertyEntity({ type: types.local });
    this.TransportadoresDasRotas = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
    this.VencedoresDefinidos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
    this.MultiplasRotasTransportadores = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
    this.Confirmar = PropertyEntity({ eventClick: ConfirmarVencedoresRota, type: types.event, text: "Confirmar", visible: ko.observable(true) });
};

var SelecionarVencedorUnicoParaTodasRotas = function () {
    this.GridSelecaoVencedorUnicoParaTodasRotas = PropertyEntity({ type: types.local });
    this.CodigoTransportador = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Confirmar = PropertyEntity({ eventClick: ConfirmarVencedorUnicoParaTodasRota, type: types.event, text: "Confirmar", visible: ko.observable(true) });
};

var ModalOfertasComponente = function () {
    this.GridModalOfertasComponente = PropertyEntity({ type: types.local });
    this.ModalOfertasComponentes = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

//#region Funções Load
function LoadBiddingAvaliacao() {
    _bidding = new Bidding();

    _pesquisaBiddingConvite = new PesquisaBiddingConvite();
    KoBindings(_pesquisaBiddingConvite, "knockoutPesquisaBiddingConvite", false, _pesquisaBiddingConvite.Pesquisar.id);

    _biddingResumo = new BiddingResumo();
    KoBindings(_biddingResumo, "knockoutBiddingInfo");

    HeaderAuditoria("BiddingConvite", _biddingResumo);

    _biddingConvites = new BiddingConvite();
    KoBindings(_biddingConvites, "knockoutConvite");

    _biddingChecklist = new BiddingChecklist();
    KoBindings(_biddingChecklist, "knockoutChecklist");

    _biddingChecklistRespostas = new BiddingChecklistResposta();
    KoBindings(_biddingChecklistRespostas, "knockoutDetalhesRespostas");

    _respostaAnexos = new RespostaAnexos();
    KoBindings(_respostaAnexos, "knockoutRespostaAnexos");

    _biddingOfertas = new BiddingOfertas();
    KoBindings(_biddingOfertas, "knockoutOfertas");

    _biddingOfertasComponente = new BiddingOfertasComponente();
    KoBindings(_biddingOfertasComponente, "knockoutOfertasComponente");

    _biddingOfertaDetalhes = new BiddingOfertaDetalhes();
    KoBindings(_biddingOfertaDetalhes, "knockoutDetalhesOferta");

    _biddingFechamento = new BiddingFechamento();
    KoBindings(_biddingFechamento, "knockoutFechamento");

    _CRUDduvidas = new CRUDDuvidas();
    KoBindings(_CRUDduvidas, "knockoutCRUDBidding");

    _duvidas = new Duvidas();
    KoBindings(_duvidas, "knockoutDuvidas");

    _titularidadeOferta = new TitularidadeOferta();
    KoBindings(_titularidadeOferta, "knockoutTitularidadeOferta");

    _selecionarVencedores = new SelecionarVencedores();

    _selecionarVencedorUnicoParaTodasRotas = new SelecionarVencedorUnicoParaTodasRotas();
    KoBindings(_selecionarVencedorUnicoParaTodasRotas, "knockoutSelecionarVencedorUnicoParaTodasRotas");

    _modalOfertasComponente = new ModalOfertasComponente();
    KoBindings(_modalOfertasComponente, "knockoutModalOfertasComponente");

    _biddingRankingOfertas = new BiddingRankingOfertas();
    KoBindings(_biddingRankingOfertas, "knockoutBiddingRankingOfertas");

    _biddingRankingCherryPickingOfertas = new BiddingRankingCherryPickingOfertas();
    KoBindings(_biddingRankingCherryPickingOfertas, "knockoutBiddingRankingCherryPickingOfertas");

    LoadEtapasBidding();
    LoadGridBidding();
    LoadGridConvites();
    LoadGridChecklist();
    LoadGridAnexos();
    LoadGridOfertas();
    LoadGridOfertasComponente();
    LoadGridRankingCherryPickingOferas();
    LoadGridRankingOferas();
    LoadGridsDetalhesOferta();
    LoadGridFechamento();
    LoadBiddingAvaliacaoNovasRodadas();

    BuscarTipoDeBidding(_pesquisaBiddingConvite.TipoBidding);
    BuscarFuncionario(_pesquisaBiddingConvite.Solicitante);
    BuscarFuncionario(_pesquisaBiddingConvite.Comprador);
    BuscarTransportadores(_pesquisaBiddingConvite.Transportador);
    BuscarTransportadores(_biddingRankingOfertas.Transportador, CallbackFiltroTransportadorRanking, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, _biddingRankingOfertas.CodigosTransportadores);
    BuscarTransportadores(_biddingRankingCherryPickingOfertas.Transportador, CallbackFiltroTransportadorRankingCherryPicking, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, _biddingRankingOfertas.CodigosTransportadores);
    BuscarFilial(_pesquisaBiddingConvite.FiliaisParticipante);
    EditarBiddingComTokenAcesso();
}

function LoadGridBidding() {
    const opcaoInformacoes = { descricao: "Carregar", id: guid(), evento: "onclick", metodo: CarregarClick, tamanho: "10", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoInformacoes] };
    _gridBiddingConvite = new GridViewExportacao(_pesquisaBiddingConvite.Pesquisar.idGrid, "BiddingConvite/Pesquisar", _pesquisaBiddingConvite, menuOpcoes);
    _gridBiddingConvite.CarregarGrid();
}

function LoadGridConvites() {
    const header = [
        { data: "Codigo", visible: false },
        { data: "Transportador", title: "Transportador", width: "50%", className: "text-align-left" },
        { data: "Situacao", title: "Situação", width: "30%", className: "text-align-center" },
        { data: "DataRetorno", title: "Data Retorno", width: "20%", className: "text-align-center" }
    ];
    _gridConvites = new BasicDataTable(_biddingConvites.Convites.id, header, null, null, null, 10);
    _gridConvites.CarregarGrid([]);
}

function LoadGridChecklist() {
    const opcaoInformacoes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: DetalhesChecklistClick, tamanho: "10", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoInformacoes] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Transportador", title: "Transportador", width: "40%", className: "text-align-left" },
        { data: "Situacao", title: "Situação", width: "30%", className: "text-align-center" },
        { data: "DataRetorno", title: "Data Retorno", width: "20%", className: "text-align-center" },
        { data: "Aceitacao", title: "Aceitação Ind.", width: "10%", className: "text-align-center" },
        { data: "AceitacaoDesejavel", title: "Aceitação Des.", width: "10%", className: "text-align-center" }
    ];

    const configExportacao = {
        url: "BiddingAvaliacao/ExportarChecklist",
        btnText: "Exportar Excel (Dados Salvos)",
        funcaoObterParametros: function () {
            return { Codigo: _biddingResumo.Codigo.val() };
        }
    }

    _gridChecklist = new BasicDataTable(_biddingChecklist.Checklist.id, header, menuOpcoes, null, null, 10, null, null, null, null, null, null, configExportacao);
    _gridChecklist.CarregarGrid([]);
}

function LoadGridAnexos() {
    const opcaoInformacoes = { descricao: "Download", id: guid(), evento: "onclick", metodo: DownloadAnexoClick, tamanho: "10", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoInformacoes] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "30%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];
    _gridAnexos = new BasicDataTable(_respostaAnexos.Anexos.id, header, menuOpcoes, null, null, 10);
    _gridAnexos.CarregarGrid([]);
}

function LoadGridOfertas() {
    const multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _biddingOfertas.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoesOferta,
        callbackSelecionado: exibirMultiplasOpcoesOferta,
        callbackSelecionarTodos: exibirMultiplasOpcoesOferta,
        somenteLeitura: false,
    }

    let editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false, functionPermite: (data) => VisibilidadeOpcaoInformacoes(data) };
    _gridOfertas = new GridView(_biddingOfertas.Ofertas.id, "BiddingAvaliacao/PesquisarAvaliacaoOfertasTransportadores", _biddingResumo, null, null, 15, null, null, null, multiplaEscolha, 45, editarColuna, null, null, true, false, callbackColumnDefaultGridOfertas);
    _gridOfertas.SetHabilitarScrollHorizontal(true, 200);
    _gridOfertas.SetCallbackDrawGridView(callbackDrawGridViewTeste);

    LoadGridParaSelecionarVencedorUnicoParaTodasRotas();
}

function callbackDrawGridViewTeste(settings, api, scrollHorizontal = true) {

    let columnNames = ["Rota", "Origem", "Destino", "Quantidade Viagem Ano", "Quantidade de Entregas", "Quantidade de Ajudantes por Veículo", "Baseline", "Volume (Ton) Ano"];
    let columnIndexes = [];

    // Encontrar os índices das colunas desejadas
    api.columns().every(function (index) {
        let headerText = api.column(index).header().textContent.trim();
        if (columnNames.includes(headerText)) {
            columnIndexes.push(index);
        }
    });

    PreencherGridLinhasTotais(api);

    function updateFixedColumns() {
        let totalWidth = 0;

        if (_gridOfertas.GridViewTable() == null) return;

        columnIndexes.forEach((columnIndex, idx) => {
            let columnWidth = _gridOfertas.GridViewTable().column(columnIndex).header().offsetWidth;
            totalWidth += columnWidth;

            _gridOfertas.GridViewTable().rows().every(function (rowIdx) {
                let cell = $(_gridOfertas.GridViewTable().cell(rowIdx, columnIndex).node());
                cell.toggleClass('grid-view-fixed-left-column-scroll', scrollHorizontal);

                if (scrollHorizontal) {
                    $(cell).css('left', `${totalWidth - columnWidth}px`);
                } else {
                    $(cell).css('left', '');
                }

                // Ajusta o left para a linha TOTAL
                $(`tr[id='-1'] td[data-column-index='${columnIndex}']`).css('left', `${totalWidth - columnWidth}px`);

                // Ajusta o left para a linha TOTAL SPEND
                $(`tr[id='-2'] td[data-column-index='${columnIndex}']`).css('left', `${totalWidth - columnWidth}px`);

            });

            let cellHeader = $(_gridOfertas.GridViewTable().column(columnIndex).header());
            if (scrollHorizontal) {
                cellHeader.addClass('grid-view-fixed-left-column-scroll');
                cellHeader.css('left', `${totalWidth - columnWidth}px`);
            } else {
                cellHeader.removeClass('grid-view-fixed-left-column-scroll');
                cellHeader.css('left', '');
            }
        });

    }

    let table = $(api.table().node());
    let observer = new MutationObserver(() => {
        updateFixedColumns();
    });

    observer.observe(table[0], { attributes: true, childList: true, subtree: true });


    // 🔹 Depois, ajustamos as colunas fixas para incluir as novas linhas TOTAL e TOTAL SPEND
    setTimeout(() => {
        updateFixedColumns();
    }, 50); // Peque
}

function LoadGridOfertasComponente() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Detalhamento de Ofertas", id: guid(), metodo: exibirModalDetalhesOfertasPorRota, tamanho: "20", icone: "" });

    _gridOfertasComponente = new GridView("grid-ofertas-componente", "BiddingAvaliacao/PesquisarAvaliacaoOfertasComponente", _biddingResumo, menuOpcoes, null, 15, null, null, null, null, null, null, null, null, true);

    LoadModalGridOfertasComponentes();
}

function LoadGridRankingCherryPickingOferas() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Selecionar Transportador para simulação", id: guid(), metodo: exibirModalSelecionarTransportadorSimuladoCherryPicking, tamanho: "20", icone: "" });

    _biddingRankingCherryPickingOfertas.SelecionarTodos.visible(true);
    _biddingRankingCherryPickingOfertas.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _biddingRankingCherryPickingOfertas.SelecionarTodos,
        somenteLeitura: false
    };

    _gridRankingCherryPickingOfertas = new GridView(_biddingRankingCherryPickingOfertas.OfertaRanking.idGrid, "BiddingAvaliacao/PesquisarRankingCherryPickingOfertas", _biddingResumo, menuOpcoes, null, 15, null, null, null, multiplaescolha, 45, null, null, null, true);
}

function LoadGridRankingOferas() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Selecionar Transportador para simulação", id: guid(), metodo: exibirModalSelecionarTransportadorSimulado, tamanho: "20", icone: "" });

    _biddingRankingOfertas.SelecionarTodos.visible(true);
    _biddingRankingOfertas.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _biddingRankingOfertas.SelecionarTodos,
        somenteLeitura: false
    };

    _gridRankingOfertas = new GridView(_biddingRankingOfertas.OfertaRanking.idGrid, "BiddingAvaliacao/PesquisarRankingOfertas", _biddingResumo, menuOpcoes, null, 15, null, null, null, multiplaescolha, 45, null, null, null, true);
}

function LoadModalGridOfertasComponentes() {
    var header = [
        { data: "CodigoRota", visible: false },
        { data: "CodigoTransportador", visible: false },
        { data: "RotaDescricao", title: "Rota", width: "10%" },
        { data: "Origem", title: "Origem", width: "10%" },
        { data: "Destino", title: "Destino", width: "10%" },
        { data: "Transportador", title: "Transportador", width: "10%" },
        { data: "ValorFrete", title: "Valor Frete", width: "10%" },
        { data: "AdicionalPorEntrega", title: "Adicional Por Entrega", width: "10%" },
        { data: "Ajudante", title: "Ajudante", width: "10%" },
        { data: "Pedagio", title: "Pedagio", width: "10%" },
        { data: "Total", title: "Total", width: "10%" },
    ];

    _gridModalOfertasComponente = new BasicDataTable(_modalOfertasComponente.GridModalOfertasComponente.id, header, null, { column: 9, dir: orderDir.asc }, null);
    RecarregarGridOfertasComponente();
}
function exibirModalDetalhesOfertasPorRota(dataRow) {
    RecarregarGridOfertasComponente(dataRow);

    Global.abrirModal("divModalDetalhesOfertasPorRota");
}

function exibirModalSelecionarTransportadorSimulado(dataRow) {

    $('#' + _biddingRankingOfertas.Transportador.idBtnSearch).click();
    _linhaTransportadorSimulado = dataRow.Codigo;
}
function exibirModalSelecionarTransportadorSimuladoCherryPicking(dataRow) {

    $('#' + _biddingRankingCherryPickingOfertas.Transportador.idBtnSearch).click();
    _linhaTransportadorSimulado = dataRow.Codigo;
}

function RecarregarGridOfertasComponente(linhaSelecionada) {
    const data = [];

    $.each(_modalOfertasComponente.ModalOfertasComponentes.list, function (i, item) {
        var dadosRota = item;
        var itemGrid = new Object();
        var codigoRota = linhaSelecionada.Codigo;

        if (codigoRota != dadosRota.CodigoRota)
            return;

        itemGrid.CodigoRota = dadosRota.CodigoRota;
        itemGrid.CodigoTransportador = dadosRota.CodigoTransportador;
        itemGrid.RotaDescricao = dadosRota.RotaDescricao;
        itemGrid.Origem = dadosRota.Origem;
        itemGrid.Destino = dadosRota.Destino;
        itemGrid.ValorFrete = Globalize.format(dadosRota.ValorFrete, "n2");
        itemGrid.AdicionalPorEntrega = Globalize.format(dadosRota.AdicionalPorEntrega, "n2");
        itemGrid.Ajudante = Globalize.format(dadosRota.Ajudante, "n2");
        itemGrid.Pedagio = Globalize.format(dadosRota.Pedagio, "n2");
        itemGrid.Transportador = dadosRota.Transportador;
        itemGrid.Total = Globalize.format(dadosRota.Total, "n2");

        data.push(itemGrid);
    });

    _gridModalOfertasComponente.CarregarGrid(data);
}

function habilitarEditarGridOfertas() {
    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false, functionPermite: (data) => VisibilidadeOpcaoInformacoes(data) };
    _gridOfertas.SetarEditarColunas(editarColuna);
}

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    $('[data-toggle="tooltip"]').tooltip('hide');
    _gridOfertas.DesfazerAlteracaoDataRow(row);

    if (VisibilidadeOpcaoInformacoes(dataRow) && head.dynamicCode > 0)
        DetalhesOfertaClick(dataRow, head)
}

function callbackColumnDefaultGridOfertas(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name.includes("ColunaTransportador")) {
        cabecalho.editableCell.type = EnumTipoColunaEditavelGrid.bool;

        var jsonCelulasPersonalizadasString = dadosLinha.CelulasPersonalizadasString;
        var jsonColunasTransportadorString = dadosLinha.ColunasTrasportadorString;
        var celulasPersonalizadasArray = JSON.parse(jsonCelulasPersonalizadasString);
        var colunasTransportadorArray = JSON.parse(jsonColunasTransportadorString);
        var percentualDiferencaBaseline = null;
        var percentualDiferencaValorAlvo = null;

        const codigoTransportador = cabecalho.dynamicCode;
        const codigoRota = dadosLinha.Codigo;

        if (celulasPersonalizadasArray !== null && celulasPersonalizadasArray.length > 0) {

            const celulaPersonalizada = celulasPersonalizadasArray.filter(celula => celula.CodigoTrasportador === codigoTransportador)[0];

            if (celulaPersonalizada) {
                percentualDiferencaBaseline = celulaPersonalizada.ValorBaseline;
                percentualDiferencaValorAlvo = celulaPersonalizada.ValorAlvo;
            }
        }

        if (colunasTransportadorArray !== null && colunasTransportadorArray.length > 0) {

            const colunaTrasportador = colunasTransportadorArray.filter(coluna => coluna.CodigoTrasportador === codigoTransportador)[0];

            if (colunaTrasportador)
                valorColuna = colunaTrasportador.Valor;
            else
                valorColuna = '0,00';
        }

        const menor = dadosLinha.MenorValor == valorColuna ? 'class="menor-valor"' : "";

        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });


        const transportadorSelecionado = _selecionarVencedores.VencedoresDefinidos.list.some(vencedor => vencedor.TransportadorCodigo == codigoTransportador && vencedor.RotaCodigo == codigoRota);

        if (percentualDiferencaBaseline != null && percentualDiferencaValorAlvo != null) {
            const classeSelecionado = transportadorSelecionado ? 'class="selecionado"' : "";

            if (classeSelecionado) {
                return `<input type="checkbox" data-bind="checked" hidden/><span ${classeSelecionado} has-tooltip" data-toggle="tooltip" data-placement="top" title="${percentualDiferencaBaseline}
                    \n${percentualDiferencaValorAlvo}"><span>${valorColuna}</span></span>`
            } else {
                return `<input type="checkbox" data-bind="checked" hidden/><span ${menor} has-tooltip" data-toggle="tooltip" data-placement="top" title="${percentualDiferencaBaseline}
                    \n${percentualDiferencaValorAlvo}"><span>${valorColuna}</span></span>`
            }
        } else {
            return `<input type="checkbox" data-bind="checked" hidden/><span ${menor} title="${valorColuna}">${valorColuna}</span>`;
        }
    }

    if (cabecalho.name.includes("ImpactoReais")) {
        if (dadosLinha.ImpactoReais == '') return;

        const impactoReais = parseFloat(dadosLinha.ImpactoReais.replace(/\./g, "").replace(",", "."));

        let valorFormatado = impactoReais.toLocaleString("pt-BR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var estiloImpactoReais = "";

        if (impactoReais < 0) {
            estiloImpactoReais = 'class="destaque-positivo"';
        } else if (impactoReais > 0) {
            estiloImpactoReais = 'class="destaque-negativo"';
        }

        return `<span ${estiloImpactoReais} title="${valorFormatado}">${valorFormatado}</span>`;
    }

    if (cabecalho.name.includes("ImpactoPercentual")) {
        const impactoPercentual = parseFloat(dadosLinha.ImpactoPercentual.replace(/\./g, "").replace(",", "."));

        var estiloImpactoPercentual = "";

        if (impactoPercentual < 0) {
            estiloImpactoPercentual = 'class="destaque-positivo"';
        } else if (impactoPercentual > 0) {
            estiloImpactoPercentual = 'class="destaque-negativo"';
        }

        return `<span ${estiloImpactoPercentual} title="${valorColuna}">${valorColuna}</span>`;
    }
}


function SelecionarVencendorUnicoClick() {
    RecarregarGridSelecionarVencedorUnicoParaTodasRotas();

    Global.abrirModal("divModalSelecionarVencedorUnicoParaTodasRotas");
}



function LoadGridParaSelecionarVencedorUnicoParaTodasRotas() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "Transportador", title: "Transportador", width: "90%" },
    ];

    const configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: false }
    _gridParaSelecionarVencedorUnicoParaTodasRotas = new BasicDataTable(_selecionarVencedorUnicoParaTodasRotas.GridSelecaoVencedorUnicoParaTodasRotas.id, header, null, { column: 1, dir: orderDir.asc }, configRowsSelect, 25, null, null, null, null, null, null, null, null, null, true, tableSelecionarVencedorUnicoParaTodasRotasChange);

    RecarregarGridSelecionarVencedorUnicoParaTodasRotas();
}

function RecarregarGridSelecionarVencedorUnicoParaTodasRotas() {
    const data = [];

    $.each(_selecionarVencedores.TransportadoresDasRotas.list, function (i, item) {
        var dadosRota = item;
        var itemGrid = new Object();

        if (data.some(existingItem => existingItem.Codigo == dadosRota.TransportadorCodigo))
            return;

        itemGrid.Transportador = dadosRota.Transportador;
        itemGrid.Codigo = dadosRota.TransportadorCodigo;
        data.push(itemGrid);
    });

    _gridParaSelecionarVencedorUnicoParaTodasRotas.CarregarGrid(data);
};

function tableSelecionarVencedorUnicoParaTodasRotasChange(registro, selecionado) {
    let lista = [registro];
    _selecionarVencedorUnicoParaTodasRotas.CodigoTransportador = registro.Codigo;
    _gridParaSelecionarVencedorUnicoParaTodasRotas.SetarSelecionados(lista);
};

function ConfirmarVencedorUnicoParaTodasRota(e) {
    var rotasVisiveis = _gridOfertas.GridViewTableData();

    if (!rotasVisiveis || rotasVisiveis.length === 0) {
        return;
    }

    rotasVisiveis = rotasVisiveis.slice(0, -2);

    rotasVisiveis.forEach(rota => {
        try {
            const colunasTransportador = JSON.parse(rota.ColunasTrasportadorString || "[]");
            const colunaTransportador = colunasTransportador.find(col => col.CodigoTrasportador === e.CodigoTransportador);

            if (!colunaTransportador || colunaTransportador.NaoOfertou) {
                return;
            }

            _selecionarVencedores.VencedoresDefinidos.list = _selecionarVencedores.VencedoresDefinidos.list.filter(
                vencedor => vencedor.RotaCodigo !== rota.DT_RowId
            );

            _selecionarVencedores.VencedoresDefinidos.list.push({
                TransportadorCodigo: e.CodigoTransportador,
                RotaCodigo: rota.DT_RowId
            });

        } catch (error) {
            exibirMensagem(tipoMensagem.aviso, "Erro", "Não foi possível selecionar vencedor.", 10);
        }
    });

    Global.fecharModal("divModalSelecionarVencedorUnicoParaTodasRotas");
    _gridOfertas.CarregarGrid();
}


function ConfirmarVencedoresRota(e) {
    const multiplosSelecionados = _gridParaSelecionarVencedores.ListaSelecionados();

    for (var i = 0; i < _selecionarVencedores.VencedoresDefinidos.list.length; i++) {
        if (_selecionarVencedores.CodigoRota.val() === _selecionarVencedores.VencedoresDefinidos.list[i].RotaCodigo.toString()) {
            _selecionarVencedores.VencedoresDefinidos.list.splice(i, 1);
            i--;
        }
    }

    for (var i = 0; i < multiplosSelecionados.length; i++) {
        var itemGrid = new Object();

        itemGrid.TransportadorCodigo = multiplosSelecionados[i].TransportadorCodigo;
        itemGrid.RotaCodigo = multiplosSelecionados[i].RotaCodigo;

        _selecionarVencedores.VencedoresDefinidos.list.push(itemGrid);
    }

    Global.fecharModal("divModalSelecionarVencedores");

    _gridOfertas.CarregarGrid();
}

function ExportarGridOfertasClick() {
    executarDownload("BiddingAvaliacao/ExportarOfertas", RetornarObjetoPesquisa(_biddingResumo), null, null, null, true)
}

function LoadGridFechamento() {

    const configExportacao = {
        url: "BiddingAvaliacao/ExportarPesquisaResultados",
        titulo: "Resultados Aceitos Bidding",
        id: "btnExportarDocumento"
    };

    _biddingRankingCherryPickingOfertas.SelecionarTodos.visible(true);
    _biddingRankingCherryPickingOfertas.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _biddingFechamento.SelecionarTodos,
        somenteLeitura: false,
        callbackNaoSelecionado: rotasVencedorasSelecionadas,
        callbackSelecionado: rotasVencedorasSelecionadas,
        callbackSelecionarTodos: rotasVencedorasSelecionadas,
    };

    const opcaoTipoTransportador = { descricao: "Alterar titularidade", id: guid(), evento: "onclick", metodo: alterarTipoTransportadorClick, tamanho: "10", icone: "" };
    const opcaoCriarTabelaFrete = { descricao: "Criar Tabela de Frete Cliente", id: guid(), evento: "onclick", metodo: criarTabelaFreteClienteClick, tamanho: "10", icone: "", visibilidade: VisibilidadeCriarTabelaFreteCliente };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoTipoTransportador, opcaoCriarTabelaFrete] };

    _gridFechamento = new GridView(_biddingFechamento.Vencedores.id, "BiddingAvaliacao/PesquisarResultados", _biddingFechamento, menuOpcoes, null, 10, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridFechamento.CarregarGrid();
}

function VisibilidadeCriarTabelaFreteCliente() {
    return _visibilidadeCriarTabelaFreteCliente;
}

function alterarTipoTransportadorClick(registroSelecionado) {

    _titularidadeOferta.CodigoOferta.val(registroSelecionado.Codigo);

    var registosSelecionados = _gridFechamento.ObterMultiplosSelecionados().map(function (registro) {
        return registro.Codigo;
    });

    _titularidadeOferta.CodigosOfertas.val(registosSelecionados);

    $("#divModalAlterarTitularidade")
        .modal("show")
        .on("bs.hidden.modal", function () {
            LimparCampos(_titularidadeOferta);
            _gridFechamento.AtualizarRegistrosNaoSelecionados(new Array());
            _gridFechamento.AtualizarRegistrosSelecionados(new Array());
            SelecionarTodos.val(false);
        });
}

function LoadGridsDetalhesOferta() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "80%", className: "text-align-left" },
        { data: "ValorMes", title: "Valor Mês", width: "20%", className: "text-align-center" }
    ];
    _gridEquipamento = new BasicDataTable(_biddingOfertaDetalhes.Equipamento.id, header, null, null, null, 10);
    _gridEquipamento.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "60%", className: "text-align-left" },
        { data: "ValorFixo", title: "Valor Fixo", width: "20%", className: "text-align-center" },
        { data: "ValorKm", title: "Valor por Km", width: "20%", className: "text-align-center" },
        { data: "CustoEstimado", title: "Custo Estimado", width: "20%", className: "text-align-center" }
    ];
    _gridFrotaFixaKm = new BasicDataTable(_biddingOfertaDetalhes.FrotaFixaKm.id, header, null, null, null, 10);
    _gridFrotaFixaKm.CarregarGrid([]);


    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "40%", className: "text-align-left" },
        { data: "ICMS", title: "ICMS (%)", width: "20%", className: "text-align-center" },
        { data: "FreteTonelada", title: "Frete (R$)", width: "20%", className: "text-align-center" },
        { data: "PedagioEixo", title: "Pedágio p/ eixo (R$)", width: "20%", className: "text-align-center" },
        { data: "FreteComICMS", title: "Frete com ICMS", width: "20%", className: "text-align-center" },
        { data: "PedagioComICMS", title: "Pedágio com ICMS", width: "20%", className: "text-align-center" },
        { data: "NumeroEixos", title: "Número de Eixos do veículo", width: "20%", className: "text-align-center" },
        { data: "Capacidade", title: "Capacidade do veículo", width: "20%", className: "text-align-center" },
        { data: "TotalBruto", title: "Total bruto", width: "20%", className: "text-align-center" },
        { data: "TotalLiquido", title: "Total líquido", width: "20%", className: "text-align-center" },
    ];
    _gridFretePorPeso = new BasicDataTable(_biddingOfertaDetalhes.FretePorPeso.id, header, null, null, null, 10);
    _gridFretePorPeso.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "40%", className: "text-align-left" },
        { data: "ICMS", title: "ICMS (%)", width: "20%", className: "text-align-center" },
        { data: "FreteTonelada", title: "Frete (R$)", width: "20%", className: "text-align-center" },
        { data: "PedagioEixo", title: "Pedágio p/ eixo (R$)", width: "20%", className: "text-align-center" },
        { data: "FreteComICMS", title: "Frete com ICMS", width: "20%", className: "text-align-center" },
        { data: "PedagioComICMS", title: "Pedágio com ICMS", width: "20%", className: "text-align-center" },
        { data: "NumeroEixos", title: "Número de Eixos do veículo", width: "20%", className: "text-align-center" },
        { data: "Capacidade", title: "Capacidade do veículo", width: "20%", className: "text-align-center" },
        { data: "TotalBruto", title: "Total bruto", width: "20%", className: "text-align-center" },
        { data: "TotalLiquido", title: "Total líquido", width: "20%", className: "text-align-center" },
    ];
    _gridFretePorCapacidade = new BasicDataTable(_biddingOfertaDetalhes.FretePorCapacidade.id, header, null, null, null, 10);
    _gridFretePorCapacidade.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "40%", className: "text-align-left" },
        { data: "ICMS", title: "ICMS (%)", width: "20%", className: "text-align-center" },
        { data: "FreteTonelada", title: "Frete - sem ICMS", width: "20%", className: "text-align-center" },
        { data: "PedagioEixo", title: "Pedágio", width: "20%", className: "text-align-center" },
        { data: "NumeroEixos", title: "Número de Eixos do veículo", width: "20%", className: "text-align-center" },
        { data: "Capacidade", title: "Capacidade do veículo", width: "20%", className: "text-align-center" },
        { data: "TotalBruto", title: "Total bruto", width: "20%", className: "text-align-center" },
        { data: "TotalLiquido", title: "Total líquido", width: "20%", className: "text-align-center" },
    ];
    _gridFretePorViagem = new BasicDataTable(_biddingOfertaDetalhes.FretePorViagem.id, header, null, null, null, 10);
    _gridFretePorViagem.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "80%", className: "text-align-left" },
        { data: "Porcentagem", title: "Porcentagem Sobre a Nota", width: "20%", className: "text-align-center" },
        { data: "CustoEstimado", title: "Custo Estimado", width: "20%", className: "text-align-center" }
    ];
    _gridPorcentagem = new BasicDataTable(_biddingOfertaDetalhes.PorcentagemSobreNota.id, header, null, null, null, 10);
    _gridPorcentagem.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "60%", className: "text-align-left" },
        { data: "ValorViagem", title: "Valor Viagem", width: "20%", className: "text-align-center" },
        { data: "Adicional", title: "Adicional por Entrega", width: "20%", className: "text-align-center" },
        { data: "CustoEstimado", title: "Custo Estimado", width: "20%", className: "text-align-center" }
    ];
    _gridViagemAdicional = new BasicDataTable(_biddingOfertaDetalhes.ViagemAdicional.id, header, null, null, null, 10);
    _gridViagemAdicional.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "40%", className: "text-align-left" },
        { data: "ValorFixo", title: "Valor Fixo", width: "20%", className: "text-align-center" },
        { data: "ValorFranquia", title: "Valor Franquia", width: "20%", className: "text-align-center" },
        { data: "Quilometragem", title: "Quilometragem", width: "20%", className: "text-align-center" },
        { data: "CustoEstimado", title: "Custo Estimado", width: "20%", className: "text-align-center" }
    ];
    _gridFrotaFixaFranquia = new BasicDataTable(_biddingOfertaDetalhes.FrotaFixaFranquia.id, header, null, null, null, 10);
    _gridFrotaFixaFranquia.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "40%", className: "text-align-left" },
        { data: "ICMS", title: "ICMS (%)", width: "20%", className: "text-align-center" },
        { data: "FreteTonelada", title: "Frete viagem sem ICMS", width: "20%", className: "text-align-center" },
        { data: "PedagioEixo", title: "Pedágio (R$)", width: "20%", className: "text-align-center" },
        { data: "Ajudante", title: "Ajudante (R$)", width: "20%", className: "text-align-center" },
        { data: "AdicionalPorEntrega", title: "Adicional por entrega (R$)", width: "20%", className: "text-align-center" },
        { data: "ViagemComPedagio", title: "Frete fechado + pedágio (com ICMS)", width: "20%", className: "text-align-center" },
        { data: "AdicionalAjudanteComICMS", title: "Frete fechado + pedágio com ajudante (com ICMS)", width: "20%", className: "text-align-center" },
        { data: "AdicionalFracionadoComICMS", title: "Frete fracionado + pedágio + adicional entrega (com ICMS)", width: "20%", className: "text-align-center" },
        { data: "AdicionalFracionadoAjudanteComICMS", title: "Frete fracionado + pedágio + adicional entrega + ajudante (com ICMS)", width: "20%", className: "text-align-center" },
    ];
    _gridViagemEntregaAjudante = new BasicDataTable(_biddingOfertaDetalhes.ViagemEntregaAjudante.id, header, null, null, null, 10);
    _gridViagemEntregaAjudante.CarregarGrid([]);
}
//#endregion

function PesquisarConvite() {
    _gridBiddingConvite.CarregarGrid();
}

function PesquisarResultados() {
    if (_gridFechamento != undefined)
        _gridFechamento.CarregarGrid();
}

//#region Funções Click
function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function salvarTitularidadeClick() {
    const data = {
        CodigoOferta: _titularidadeOferta.CodigoOferta.val(),
        Titularidade: _titularidadeOferta.TipoTransportador.val(),
        CodigosOfertas: JSON.stringify(_titularidadeOferta.CodigosOfertas.val())
    };

    executarReST("BiddingAvaliacao/SalvarTitularidadeTransportador", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A titularidade da oferta foi salva com sucesso!");
                _gridFechamento.CarregarGrid();
                Global.fecharModal("divModalAlterarTitularidade");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function CarregarClick(registroSelecionado) {
    _buscarPorCodigo = true;
    LimparCamposBiddingAvaliacao();
    _biddingResumo.ExibirResumo.val(true);
    executarReST("BiddingAvaliacao/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _todosResultadosOfertas = retorno.Data.Ofertas;
                PreencherObjetoKnout(_biddingResumo, { Data: retorno.Data.Resumo });
                PreencherObjetoKnout(_bidding, { Data: retorno.Data.Dados });
                PreencherObjetoKnout(_biddingFechamento, { Data: retorno.Data.Dados });
                PreencherGridConvites(retorno.Data.Convites);
                PreencherGridChecklist(retorno.Data.Checklist);
                PreencherSelecionarVencedores(retorno.Data.TransportadoresDasRotas);
                PreencherTransportadoresFiltroRanking(retorno.Data.TransportadoresDasRotas);
                PreencherModalOfertasComponentes(retorno.Data.OfertasComponente);
                _biddingResumo.OfertasComponente.val(JSON.stringify(retorno.Data.OfertasComponente));
                PreencherGridOfertas();

                //// PREENCHIMENTO DOS FILTROS DE PESQUISA
                _biddingResumo.Filtros.val(retorno.Data.Filtros.FiltrosString);
                PreencherOpcoesOfertasRota(retorno.Data.Filtros.ListaRotas);
                PreencherOpcoesOfertasModeloVeicular(retorno.Data.Filtros.ListaModelosVeiculares);
                PreencherOpcoesOfertasFilial(retorno.Data.Filtros.ListaFilial);
                PreencherOpcoesQuantidadeEntregas(retorno.Data.Filtros.ListaQuantidadeEntregas);
                PreencherOpcoesQuantidadeAjudantes(retorno.Data.Filtros.ListaQuantidadeAjudantes);
                PreencherOpcoesQuantidadeViagensAno(retorno.Data.Filtros.ListaQuantidadeViagensAno);
                PreencherOpcoesOfertasOrigem(retorno.Data.Filtros.ListaOrigem); // Cidades Origem
                PreencherOpcoesOfertasDestino(retorno.Data.Filtros.ListaDestino); // Cidades Destino
                PreencherOpcoesOfertasMesorregiaoDestino(retorno.Data.Filtros.ListaMesorregiaoDestino);
                PreencherOpcoesOfertasMesorregiaoOrigem(retorno.Data.Filtros.ListaMesorregiaoOrigem);
                PreencherOpcoesOfertasRegiaoDestino(retorno.Data.Filtros.ListaRegiaoDestino); //Regioes Brasil Destino
                PreencherOpcoesOfertasRegiaoOrigem(retorno.Data.Filtros.ListaRegiaoOrigem); //Regioes Brasil Origem
                PreencherOpcoesOfertasRotaDestino(retorno.Data.Filtros.ListaRotaDestino); // Rotas Destino
                PreencherOpcoesOfertasRotaOrigem(retorno.Data.Filtros.ListaRotaOrigem); // Rotas Origem
                PreencherOpcoesOfertasClienteDestino(retorno.Data.Filtros.ListaClienteDestino); // Clientes Destino
                PreencherOpcoesOfertasClienteOrigem(retorno.Data.Filtros.ListaClienteOrigem); // Clientes Origem
                PreencherOpcoesOfertasEstadoDestino(retorno.Data.Filtros.ListaEstadoDestino); // Estados Destino
                PreencherOpcoesOfertasEstadoOrigem(retorno.Data.Filtros.ListaEstadoOrigem); // Estados Origem
                PreencherOpcoesOfertasPaisDestino(retorno.Data.Filtros.ListaPaisDestino); // Paises Destino
                PreencherOpcoesOfertasPaisOrigem(retorno.Data.Filtros.ListaPaisOrigem); // Paises Origem
                PreencherOpcoesOfertasCEPDestino(retorno.Data.Filtros.PossuiCEPDestino); // CEPs Destino
                PreencherOpcoesOfertasCEPOrigem(retorno.Data.Filtros.PossuiCEPOrigem); // CEPs Origem
                PreencherGridFechamento();
                PreencherOpcoesFechamentoRota(retorno.Data.Filtros.ListaRotas);
                SetarEtapasRequisicao(retorno.Data.Dados.Situacao, retorno.Data.TipoPreenchimentoChecklist);
                preencherPrazos(retorno.Data.Prazos);
                SetarVisibilidadeFiltrosPorTipoLance(retorno.Data.TipoOferta);

                _biddingRankingOfertas.OfertaRanking.visible(false);
                _biddingRankingCherryPickingOfertas.OfertaRanking.visible(false);
                if (retorno.Data.Dados.ExibirRankOfertas) {
                    _biddingRankingOfertas.OfertaRanking.visible(true);
                    _biddingRankingCherryPickingOfertas.OfertaRanking.visible(true);
                }

                if (retorno.Data.TipoLance === EnumTipoLanceBidding.LancePorViagemEntregaAjudante)
                    _biddingOfertasComponente.OfertaComponente.visible(true);
                else
                    _biddingOfertasComponente.OfertaComponente.visible(false);


                _pesquisaBiddingConvite.ExibirFiltros.visibleFade(false);
                _biddingOfertas.ProporRodadaSelecionados.visible(false);

                if (_bidding.Situacao.val() == EnumStatusBidding.Fechamento) {
                    _CRUDduvidas.RecarregarBidding.visible(false);
                } else {
                    _CRUDduvidas.RecarregarBidding.visible(true);
                }
                if (_bidding.Situacao.val() == EnumStatusBidding.Fechamento) {
                    _CRUDduvidas.FecharBidding.visible(false);
                } else {
                    _CRUDduvidas.FecharBidding.visible(true);
                }

                _buscarPorCodigo = false;
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function RecarregarGridConviteChecklist() {
    executarReST("BiddingAvaliacao/RecarregarGridConviteChecklist", { Codigo: _bidding.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherGridConvites(retorno.Data.Convites);
                PreencherGridChecklist(retorno.Data.Checklist);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function EditarBiddingComTokenAcesso() {
    if (!string.IsNullOrWhiteSpace(_tokenAcessoAvaliacao) && _tokenAcessoAvaliacao != "NaoInformado") {
        var dados = {
            Codigo: _tokenAcessoAvaliacao
        }
        CarregarClick(dados);
        window.history.replaceState(null, null, '#/Bidding/BiddingAvaliacao');
    }
}

function DetalhesChecklistClick(registroSelecionado) {
    Global.abrirModal('divModalDetalhesResposta');

    $("#divModalDetalhesResposta").one('hidden.bs.modal', function () {
        LimparCampos(_biddingChecklistRespostas);
    });

    executarReST("BiddingAvaliacao/BuscarRespostas", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _biddingChecklistRespostas.Codigo.val(registroSelecionado.Codigo);
                _biddingChecklistRespostas.Respostas.removeAll();

                for (var i = 0; i < retorno.Data.Respostas.length; i++) {
                    const resposta = new Resposta(retorno.Data.Respostas[i]);
                    _biddingChecklistRespostas.Respostas.push(resposta);
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function AprovarChecklistClick(e) {
    if (!ValidarCamposObrigatorios(_biddingChecklistRespostas)) {
        exibirMensagem("atencao", "Preencha os campos obrigatórios.");
        return;
    }

    executarReST("BiddingAvaliacao/AprovarChecklist", { Codigo: e.Codigo.val(), Observacao: e.Observacao.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Checklist aprovada.");
                Global.fecharModal('divModalDetalhesResposta');
                PreencherGridChecklist(retorno.Data.Checklist);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function RejeitarChecklistClick(e) {
    if (!ValidarCamposObrigatorios(_biddingChecklistRespostas)) {
        exibirMensagem("atencao", "Preencha os campos obrigatórios.");
        return;
    }

    executarReST("BiddingAvaliacao/ReprovarChecklist", { Codigo: e.Codigo.val(), Observacao: e.Observacao.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Checklist reprovada.");
                Global.fecharModal('divModalDetalhesResposta');
                PreencherGridChecklist(retorno.Data.Checklist);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function VerAnexosChecklistClick(e) {
    Global.abrirModal('divModalRespostaAnexos');
    _gridAnexos.SetarRegistros(e.Anexos.val());
    _gridAnexos.CarregarGrid(e.Anexos.val());
}

function DownloadAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    executarDownload("BiddingAceitamentoAnexo/DownloadAnexo", dados);
}

function DetalhesOfertaClick(linhaSelecionada, colunaSelecionada) {
    if (linhaSelecionada.Codigo <= 0 || colunaSelecionada.dynamicCode <= 0)
        return;

    if (!_biddingOfertas.SelecionarVencedoresSituacao.val()) {
        executarReST("BiddingAvaliacao/BuscarOfertas", { CodigoRota: linhaSelecionada.Codigo, CodigoTransportador: colunaSelecionada.dynamicCode }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    SetarTabsDetalheOferta(retorno.Data.Tabs);
                    PreencherGridsDetalheOferta(retorno.Data);
                    PreencherObjetoKnout(_biddingOfertaDetalhes, retorno);
                    PreencherOfertaOptions(retorno.Data.Ofertas);
                    ControlarVisibilidadeFooterModalOfertaDetalhes(retorno.Data);
                    MostrarModalOfertaDetalhes();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    } else {
        MarcarVencedor(linhaSelecionada, colunaSelecionada);
    }
}

function ControlarVisibilidadeFooterModalOfertaDetalhes(retorno) {
    if (retorno.SituacaoEnum != EnumBiddingOfertaSituacao.EmAnalise || _bidding.Situacao.val() == EnumStatusBidding.Fechamento)
        $("#divModalOfertaDetalhes #footerPrincipal").hide();
    else
        $("#divModalOfertaDetalhes #footerPrincipal").show();
}

function AceitarOfertaClick() {
    const identificador = parseInt(0);
    var ofertaAceita;

    switch (identificador) {
        case EnumStatusBiddingTipoOferta.Equipamento:
            ofertaAceita = _biddingOfertaDetalhes.Equipamento.val();
            break;

        case EnumStatusBiddingTipoOferta.FrotaFixaFranquia:
            ofertaAceita = _biddingOfertaDetalhes.FrotaFixaFranquia.val();
            break;

        case EnumStatusBiddingTipoOferta.FretePorPeso:
            ofertaAceita = _biddingOfertaDetalhes.FretePorPeso.val();
            break;

        case EnumStatusBiddingTipoOferta.FretePorCapacidade:
            ofertaAceita = _biddingOfertaDetalhes.FretePorCapacidade.val();
            break;

        case EnumStatusBiddingTipoOferta.FretePorViagem:
            ofertaAceita = _biddingOfertaDetalhes.FretePorViagem.val();
            break;

        case EnumStatusBiddingTipoOferta.FrotaFixaKm:
            ofertaAceita = _biddingOfertaDetalhes.FrotaFixaKm.val();
            break;

        case EnumStatusBiddingTipoOferta.Porcentagem:
            ofertaAceita = _biddingOfertaDetalhes.PorcentagemSobreNota.val();
            break;

        case EnumStatusBiddingTipoOferta.ViagemAdicional:
            ofertaAceita = _biddingOfertaDetalhes.ViagemAdicional.val();
            break;

        case EnumStatusBiddingTipoOferta.ViagemEntregaAjudante:
            ofertaAceita = _biddingOfertaDetalhes.ViagemEntregaAjudante.val();
            break;
    }

    if (ofertaAceita == undefined) {
        exibirMensagem(tipoMensagem.falha, "Não foi possível enviar a solicitação.");
        return;
    }

    executarReST("BiddingAvaliacao/AceitarOferta", { Ofertas: JSON.stringify(ofertaAceita), Codigo: _biddingOfertaDetalhes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Você aceitou a oferta.");
                PreencherGridOfertas();
                PreencherOpcoesOfertasRota(retorno.Data.Ofertas);
                PreencherOpcoesOfertasModeloVeicular(retorno.Data.ListaModelosVeiculares);
                Global.fecharModal('divModalOfertaDetalhes');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ProporRodadaClick() {
    var dados = {
        Codigo: _biddingOfertaDetalhes.Codigo.val(),
        Target: _propostaNovasRodadas.Target.val(),
    }

    executarReST("BiddingAvaliacao/ProporNovaRodada", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Você propôs uma nova rodada.");
                PreencherGridOfertas();
                PreencherOpcoesOfertasRota(retorno.Data.ListaRotas);
                PreencherOpcoesOfertasModeloVeicular(retorno.Data.ListaModelosVeiculares);
                Global.fecharModal('divModalOfertaDetalhes');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function criarTabelaFreteClick() {

    const rotasSelecionadas = _gridFechamento.ObterMultiplosSelecionados();
    const codigosRotas = new Array();
    for (let i = 0; i < rotasSelecionadas.length; i++) {
        codigosRotas.push(rotasSelecionadas[i].Codigo);
    }

    executarReST("BiddingAvaliacao/ObterDadosParaTabelaFrete", { Codigos: JSON.stringify(codigosRotas), CodigoBidding: _biddingResumo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                //let url = "?transportador=" + retorno.Data.Transportador + "&vigencia=" + "t" +"&formato=" + retorno.Data.Formato +"&modeloVeicular=" + retorno.Data.ModeloVeicular +"&tipoOperacao=" + retorno.Data.TipoOperacao + "&tipoCarga=" + retorno.Data.TipoCarga + "";
                //window.location.href = "#/Fretes/TabelaFrete";
                if (retorno.Data.Rotas) sessionStorage.setItem("rotas", retorno.Data.Rotas);
                if (retorno.Data.Descricao) sessionStorage.setItem("descricao", retorno.Data.Descricao);
                if (retorno.Data.TipoBidding) sessionStorage.setItem("tipoBidding", retorno.Data.TipoBidding);
                if (retorno.Data.Transportador) sessionStorage.setItem("transportador", retorno.Data.Transportador);
                if (retorno.Data.InicioVigencia) sessionStorage.setItem("inicioVigencia", retorno.Data.InicioVigencia);
                if (retorno.Data.FimVigencia) sessionStorage.setItem("fimVigencia", retorno.Data.FimVigencia);
                if (retorno.Data.Formato) sessionStorage.setItem("formato", retorno.Data.Formato);
                if (retorno.Data.ModeloVeicularTracao) sessionStorage.setItem("modeloVeicularTracao", retorno.Data.ModeloVeicularTracao);
                if (retorno.Data.ModeloVeicularReboque) sessionStorage.setItem("modeloVeicularReboque", retorno.Data.ModeloVeicularReboque);
                if (retorno.Data.TipoCarga) sessionStorage.setItem("tipoCarga", retorno.Data.TipoCarga);
                if (retorno.Data.FiliaisParticipantes) sessionStorage.setItem("filiaisParticipantes", retorno.Data.FiliaisParticipantes);
                if (retorno.Data.FaixaEntrega) sessionStorage.setItem("faixaEntrega", retorno.Data.FaixaEntrega);
                if (retorno.Data.FaixaPeso) sessionStorage.setItem("faixaPeso", retorno.Data.FaixaPeso);
                if (retorno.Data.FaixaAjudante) sessionStorage.setItem("faixaAjudante", retorno.Data.FaixaAjudante);
                window.open("#/Fretes/TabelaFrete", "_blank");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function criarTabelaFreteClienteClick(registroSelecionado) {
    executarReST("BiddingAvaliacao/ObterDadosParaTabelaFreteCliente", { Codigo: registroSelecionado.Codigo, CodigoBidding: _biddingResumo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.TabelaFrete) {
                    if (retorno.Data.TipoBidding) sessionStorage.setItem("tipoBiddingCli", retorno.Data.TipoBidding);
                    if (retorno.Data.ParametroBase) sessionStorage.setItem("parametroBase", retorno.Data.ParametroBase);
                    if (retorno.Data.TipoOferta) sessionStorage.setItem("tipoOferta", retorno.Data.TipoOferta);
                    if (retorno.Data.Origem) sessionStorage.setItem("origem", retorno.Data.Origem);
                    if (retorno.Data.Destino) sessionStorage.setItem("destino", retorno.Data.Destino);
                    if (retorno.Data.ClienteOrigem) sessionStorage.setItem("clienteOrigem", retorno.Data.ClienteOrigem);
                    if (retorno.Data.ClienteDestino) sessionStorage.setItem("clienteDestino", retorno.Data.ClienteDestino);
                    if (retorno.Data.EstadoOrigem) sessionStorage.setItem("estadoOrigem", retorno.Data.EstadoOrigem);
                    if (retorno.Data.EstadoDestino) sessionStorage.setItem("estadoDestino", retorno.Data.EstadoDestino);
                    if (retorno.Data.RegiaoOrigem) sessionStorage.setItem("regiaoOrigem", retorno.Data.RegiaoOrigem);
                    if (retorno.Data.RegiaoDestino) sessionStorage.setItem("regiaoDestino", retorno.Data.RegiaoDestino);
                    if (retorno.Data.RotasOrigem) sessionStorage.setItem("rotasOrigem", retorno.Data.RotasOrigem);
                    if (retorno.Data.RotasDestino) sessionStorage.setItem("rotasDestino", retorno.Data.RotasDestino);
                    if (retorno.Data.CepOrigem) sessionStorage.setItem("cepOrigem", retorno.Data.CepOrigem);
                    if (retorno.Data.CepDestino) sessionStorage.setItem("cepDestino", retorno.Data.CepDestino);
                    if (retorno.Data.PaisOrigem) sessionStorage.setItem("paisOrigem", retorno.Data.PaisOrigem);
                    if (retorno.Data.PaisDestino) sessionStorage.setItem("paisDestino", retorno.Data.PaisDestino);
                    if (retorno.Data.OfertasSemAjudante) sessionStorage.setItem("ofertasSemAjudante", retorno.Data.OfertasSemAjudante);
                    if (retorno.Data.OfertasComAjudantes) sessionStorage.setItem("ofertasComAjudantes", retorno.Data.OfertasComAjudantes);
                    if (retorno.Data.Oferta) sessionStorage.setItem("oferta", retorno.Data.Oferta);
                    sessionStorage.setItem("tabelaFrete", retorno.Data.TabelaFrete);
                    window.open("#/Fretes/TabelaFreteCliente", "_blank");
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", "Essa rota não possui uma tabela de frete.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function rotasVencedorasSelecionadas(e) {
    var possuiSelecionado = _gridFechamento.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _biddingFechamento.SelecionarTodos.val();

    if (possuiSelecionado || selecionadoTodos) {
        _biddingFechamento.CriarTabelaFrete.visible(true);
        _visibilidadeCriarTabelaFreteCliente = false;
    }
    else {
        _biddingFechamento.CriarTabelaFrete.visible(false);
        _visibilidadeCriarTabelaFreteCliente = true;
    }
}

function ProporMultiplosSelecionadosClick() {
    LimparCampos(_propostaNovasRodadas);
    LimparCampos(_propostaMultiplasNovasRodadas);

    const multiplosSelecionados = _gridOfertas.ObterMultiplosSelecionados();

    if (multiplosSelecionados.length > 1) {
        Global.abrirModal("divModalPropostaMultiplasNovasRodadas");
    } else {
        var menorValor = multiplosSelecionados[0].MenorValor;
        var baseline = multiplosSelecionados[0].Baseline;

        _propostaNovasRodadas.BaseCalculoMenorValor.val(menorValor);
        _propostaNovasRodadas.BaseCalculoBaseline.val(baseline);

        Global.abrirModal("divModalPropostaNovasRodadas");
    }
}

function ConfirmarNovasRodadas(e) {
    _selecionarVencedores.MultiplasRotasTransportadores.list = new Array();

    const multiplosSelecionados = _gridOfertas.ObterMultiplosSelecionados();

    var itemGrid = new Object();
    var linhaSelecionada = multiplosSelecionados[0];

    itemGrid.Codigo = linhaSelecionada.Codigo;
    itemGrid.Target = _propostaNovasRodadas.Target.val();

    _selecionarVencedores.MultiplasRotasTransportadores.list.push(itemGrid);


    Global.fecharModal("divModalPropostaMultiplasNovasRodadas");

    ProporMultiplasRodadas(_selecionarVencedores.MultiplasRotasTransportadores.list);
}

function ConfirmarMultiplasNovasRodadas(e) {
    _selecionarVencedores.MultiplasRotasTransportadores.list = new Array();

    const multiplosSelecionados = _gridOfertas.ObterMultiplosSelecionados();

    for (var i = 0; i < multiplosSelecionados.length; i++) {
        var itemGrid = new Object();
        var linhaSelecionada = multiplosSelecionados[i];
        var target = 0;

        if (linhaSelecionada.DT_RowId == 0 || linhaSelecionada.Codigo < 0)
            continue;

        if (_propostaMultiplasNovasRodadas.ProporBaselineOuMenorValor.val() == EnumNovasRodadasBaselineMenorValor.Baseline && linhaSelecionada.Baseline == 0) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Rota não contém Baseline.");
            return;
        }

        if (_propostaMultiplasNovasRodadas.ProporBaselineOuMenorValor.val() == EnumNovasRodadasBaselineMenorValor.MenorValor && linhaSelecionada.MenorValor == '') {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Rota não contém Menor Valor.");
            return;
        }

        if (_propostaMultiplasNovasRodadas.Baseline.val())
            target = calculoPropostaMultiplasRodadasBaseline(linhaSelecionada)
        else if (_propostaMultiplasNovasRodadas.MenorValor.val())
            target = calculoPropostaMultiplasRodadasMenorValor(linhaSelecionada)

        itemGrid.Codigo = linhaSelecionada.Codigo;
        itemGrid.Target = target;

        _selecionarVencedores.MultiplasRotasTransportadores.list.push(itemGrid);
    }

    Global.fecharModal("divModalPropostaMultiplasNovasRodadas");

    ProporMultiplasRodadas(_selecionarVencedores.MultiplasRotasTransportadores.list);
}

function ProporMultiplasRodadas(dados) {
    executarReST("BiddingAvaliacao/ProporMultiplasRodadas", { MultiplasRotasTransportadores: JSON.stringify(dados) }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Você propôs novas rodadas.");
                PreencherGridOfertas();
                PreencherOpcoesOfertasRota(retorno.Data.ListaRotas);
                PreencherOpcoesOfertasModeloVeicular(retorno.Data.ListaModelosVeiculares);

                _selecionarVencedores.MultiplasRotasTransportadores.list = new Array();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function calculoPropostaMultiplasRodadasBaseline(linhaSelecionada) {
    var baselineSelecionado = linhaSelecionada.Baseline.replace(/\./g, '').replace(',', '.');
    var baseline = parseFloat(baselineSelecionado);

    var novoValorBaseline = Globalize.parseFloat(_propostaMultiplasNovasRodadas.Baseline.val());

    var reducao = (novoValorBaseline / 100) * baseline;

    var proximoValorAlvoBaseline = baseline - reducao;

    return (Globalize.format(proximoValorAlvoBaseline, "n2"));
}

function calculoPropostaMultiplasRodadasMenorValor(linhaSelecionada) {
    var menorValorSelecionado = linhaSelecionada.MenorValor.replace(/\./g, '').replace(',', '.');
    var menorValor = parseFloat(menorValorSelecionado);

    var novoValorMenorValor = Globalize.parseFloat(_propostaMultiplasNovasRodadas.MenorValor.val());

    var reducao = (novoValorMenorValor / 100) * menorValor;

    var proximoValorAlvoMenorValor = menorValor - reducao;

    return (Globalize.format(proximoValorAlvoMenorValor, "n2"));
}

function MarcarVencedor(linhaSelecionada, colunaSelecionada) {
    const rotaCodigo = linhaSelecionada.DT_RowId;
    const colunasArray = JSON.parse(linhaSelecionada.ColunasTrasportadorString);
    const colunaAtual = colunasArray.find(
        (coluna) => coluna.CodigoTrasportador === colunaSelecionada.dynamicCode
    );

    if (colunaAtual.NaoOfertou) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "A coluna não possui oferta, portanto não é possível marcá-la como vencedora.");
        return;
    }

    const gridId = _gridOfertas.GetGridId();
    const trSelecionado = $("#" + gridId + " #" + linhaSelecionada.DT_RowId);
    const tdSelecionada = trSelecionado.find(`td:eq(${colunaSelecionada.position - 13})`);

    const tdJaSelecionado = trSelecionado.find("td span.selecionado");

    const itemIndex = _selecionarVencedores.VencedoresDefinidos.list.findIndex(
        (item) => item.RotaCodigo === rotaCodigo
    );

    if (tdJaSelecionado.length > 0) {
        tdJaSelecionado.removeClass("selecionado");
    }

    if (itemIndex !== -1) {
        _selecionarVencedores.VencedoresDefinidos.list.splice(itemIndex, 1);
    }

    _selecionarVencedores.VencedoresDefinidos.list.push({
        TransportadorCodigo: colunaSelecionada.dynamicCode,
        RotaCodigo: rotaCodigo
    });

    if (tdSelecionada.length > 0) {
        const spanDentroTd = tdSelecionada.find("span");

        if (spanDentroTd.length > 0) {
            spanDentroTd.addClass("selecionado");
        }
    }
}

function SelecionarVencedoresClick() {
    if (_biddingOfertas.SelecionarVencedoresSituacao.val()) {
        _biddingOfertas.SelecionarVencedoresSituacao.val(false);
        _biddingOfertas.SelecionarVencendorUnico.visible(false);
        _biddingOfertas.SelecionarVencedores.text("Selecionar Vencedores");
        exibirMensagem(tipoMensagem.aviso, "Selecionar Vencedores:", "Operação Desativada");
    } else {
        _biddingOfertas.SelecionarVencedoresSituacao.val(true);
        _biddingOfertas.SelecionarVencendorUnico.visible(true);
        _biddingOfertas.SelecionarVencedores.text("Selecionando Vencedores...");
        exibirMensagem(tipoMensagem.aviso, "Selecionar Vencedores:", "Operação Ativada");
    }
}

function CompararOfertaClick() {
    if (!ValidarCamposObrigatorios(_biddingOfertaDetalhes)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    executarReST("BiddingAvaliacao/CompararCargasJaFeitas", { Codigo: _biddingOfertaDetalhes.Oferta.val(), Mes: _biddingOfertaDetalhes.Mes.val(), Ano: _biddingOfertaDetalhes.Ano.val() }, function (retorno) {
        if (retorno.Success) {
            PreencherObjetoKnout(_biddingOfertaDetalhes, retorno);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function EnviarRespostaClick(e) {
    if (!ValidarCamposObrigatorios(e)) {
        exibirMensagem(tipoMensagem.atencao, "Preencha o(s) campo(s) obrigatório(s).");
        return;
    }

    executarReST("BiddingAvaliacao/ResponderDuvida", { Codigo: e.Codigo.val(), Resposta: e.Responder.val() }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Resposta enviada.");
            BuscarDuvidas();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}
//#endregion

//#region Funções Privadas

async function FiltroOfertas() {
    if (_buscarPorCodigo)
        return;

    if (_bidding.Codigo.val() > 0) {
        //Pegar dados dos filtros
        var codigoRota = _biddingOfertas.Rotas.val();
        var codigoModeloVeicular = _biddingOfertas.ModeloVeicular.val();
        var codigoFilial = _biddingOfertas.Filial.val();
        var codigoOrigem = _biddingOfertas.Origem.val();
        var codigoDestino = _biddingOfertas.Destino.val();
        var codigoMesorregiaoDestino = _biddingOfertas.MesorregiaoDestino.val();
        var codigoMesorregiaoOrigem = _biddingOfertas.MesorregiaoOrigem.val();
        var codigoQuantidadeEntregas = _biddingOfertas.QuantidadeEntregas.val();
        var codigoQuantidadeAjudantes = _biddingOfertas.QuantidadeAjudantes.val();
        var codigoQuantidadeViagensAno = _biddingOfertas.QuantidadeViagensAno.val();
        var codigoRegiaoDestino = _biddingOfertas.RegiaoDestino.val();
        var codigoRegiaoOrigem = _biddingOfertas.RegiaoOrigem.val();
        var codigoRotaDestino = _biddingOfertas.RotaDestino.val();
        var codigoRotaOrigem = _biddingOfertas.RotaOrigem.val();
        var codigoClienteDestino = _biddingOfertas.ClienteDestino.val();
        var codigoClienteOrigem = _biddingOfertas.ClienteOrigem.val();
        var codigoEstadoDestino = _biddingOfertas.EstadoDestino.val();
        var codigoEstadoOrigem = _biddingOfertas.EstadoOrigem.val();
        var codigoPaisDestino = _biddingOfertas.PaisDestino.val();
        var codigoPaisOrigem = _biddingOfertas.PaisOrigem.val();
        var CEPDestino = _biddingOfertas.CEPDestino.val();
        var CEPOrigem = _biddingOfertas.CEPOrigem.val();

        //Filtro Rota
        if (codigoRota != "")
            _biddingResumo.CodigoRota.val(codigoRota);
        else
            _biddingResumo.CodigoRota.val(0);
        //Filtro Modelo Veicular
        if (codigoModeloVeicular != "")
            _biddingResumo.CodigoModeloVeicular.val(codigoModeloVeicular);
        else
            _biddingResumo.CodigoModeloVeicular.val(0);

        //Filtro Filial
        if (codigoFilial != "")
            _biddingResumo.CodigoFilialParticipante.val(codigoFilial);
        else
            _biddingResumo.CodigoFilialParticipante.val(0);

        //Filtro Origem
        if (codigoOrigem != "")
            _biddingResumo.CodigoOrigem.val(codigoOrigem);
        else
            _biddingResumo.CodigoOrigem.val(0);

        //Filtro Destino
        if (codigoDestino != "")
            _biddingResumo.CodigoDestino.val(codigoDestino);
        else
            _biddingResumo.CodigoDestino.val(0);

        //Filtro Mesorregiao Origem
        if (codigoMesorregiaoOrigem != "")
            _biddingResumo.CodigoMesorregiaoOrigem.val(codigoMesorregiaoOrigem);
        else
            _biddingResumo.CodigoMesorregiaoOrigem.val(0);

        //Filtro Mesorregiao Destino
        if (codigoMesorregiaoDestino != "")
            _biddingResumo.CodigoMesorregiaoDestino.val(codigoMesorregiaoDestino);
        else
            _biddingResumo.CodigoMesorregiaoDestino.val(0);

        //Filtro Regiao Origem
        if (codigoRegiaoOrigem != "")
            _biddingResumo.CodigoRegiaoOrigem.val(codigoRegiaoOrigem);
        else
            _biddingResumo.CodigoRegiaoOrigem.val(0);

        //Filtro Regiao Destino
        if (codigoRegiaoDestino != "")
            _biddingResumo.CodigoRegiaoDestino.val(codigoRegiaoDestino);
        else
            _biddingResumo.CodigoRegiaoDestino.val(0);

        // Filtro Quantidade de Entregas
        if (codigoQuantidadeEntregas != "")
            _biddingResumo.QuantidadeEntregas.val(codigoQuantidadeEntregas);
        else
            _biddingResumo.QuantidadeEntregas.val(0);

        // Filtro Quantidade de Ajudantes
        if (codigoQuantidadeAjudantes != "")
            _biddingResumo.QuantidadeAjudantes.val(codigoQuantidadeAjudantes);
        else
            _biddingResumo.QuantidadeAjudantes.val(0);

        // Filtro Quantidade de Viagens por Ano
        if (codigoQuantidadeViagensAno != "")
            _biddingResumo.QuantidadeViagensAno.val(codigoQuantidadeViagensAno);
        else
            _biddingResumo.QuantidadeViagensAno.val(0);

        // Filtro Rota Destino
        if (codigoRotaDestino != "")
            _biddingResumo.RotaDestino.val(codigoRotaDestino);
        else
            _biddingResumo.RotaDestino.val(0);

        // Filtro Rota Origem
        if (codigoRotaOrigem != "")
            _biddingResumo.RotaOrigem.val(codigoRotaOrigem);
        else
            _biddingResumo.RotaOrigem.val(0);

        // Filtro Cliente Destino
        if (codigoClienteDestino != "")
            _biddingResumo.ClienteDestino.val(codigoClienteDestino);
        else
            _biddingResumo.ClienteDestino.val(0);

        // Filtro Cliente Origem
        if (codigoClienteOrigem != "")
            _biddingResumo.ClienteOrigem.val(codigoClienteOrigem);
        else
            _biddingResumo.ClienteOrigem.val(0);

        // Filtro Estado Destino
        if (codigoEstadoDestino != "")
            _biddingResumo.EstadoDestino.val(codigoEstadoDestino);
        else
            _biddingResumo.EstadoDestino.val(0);

        // Filtro Estado Origem
        if (codigoEstadoOrigem != "")
            _biddingResumo.EstadoOrigem.val(codigoEstadoOrigem);
        else
            _biddingResumo.EstadoOrigem.val(0);

        // Filtro País Destino
        if (codigoPaisDestino != "")
            _biddingResumo.PaisDestino.val(codigoPaisDestino);
        else
            _biddingResumo.PaisDestino.val(0);

        // Filtro País Origem
        if (codigoPaisOrigem != "")
            _biddingResumo.PaisOrigem.val(codigoPaisOrigem);
        else
            _biddingResumo.PaisOrigem.val(0);

        // Filtro CEP Destino
        if (CEPDestino != "")
            _biddingResumo.CEPDestino.val(CEPDestino);
        else
            _biddingResumo.CEPDestino.val("");

        // Filtro CEP Origem
        if (CEPOrigem != "")
            _biddingResumo.CEPOrigem.val(CEPOrigem);
        else
            _biddingResumo.CEPOrigem.val("");


        _biddingResumo.CarregarGrid.val(true);

        await PreencherGridOfertas();
        FiltrarOpcoesFiltrosPorRota();
        RecarregarSimulacoes();
        _biddingResumo.CarregarGrid.val(false);
    }
}

function SetarTabsDetalheOferta(tabs) {
    var PrimeiroVisivel = "";

    for (var i = 1; i <= 9; i++) {
        const tabId = "#tabDetalhe" + i;
        $(tabId).hide();
    }

    for (var i = 0; i < tabs.length; i++) {
        $(tabs[i].Identificador).show();
        if (i == 0)
            PrimeiroVisivel = "a[href='#tab" + tabs[i].Identificador.slice(tabs[i].Identificador.length - 1) + "']";
    }

    $(PrimeiroVisivel).tab('show');
}

function LimparCamposBiddingAvaliacao() {
    _biddingResumo.ExibirResumo.val(false);
    LimparCampos(_bidding);
    LimparCampos(_biddingResumo);
    LimparCampos(_biddingOfertas);
    LimparCampos(_selecionarVencedores);
}

function MostrarModalOfertaDetalhes() {
    Global.abrirModal('divModalOfertaDetalhes');
    $("#divModalOfertaDetalhes").on('hidden.bs.modal', function () {
        LimparCampos(_biddingOfertaDetalhes);
    });
}

function MostrarModalDuvidas() {
    BuscarDuvidas();
    Global.abrirModal('divModalDuvidas');
}

function FecharBidding() {
    exibirConfirmacao("Confirmação", "Realmente deseja fechar o Bidding?", function () {
        executarReST("BiddingAvaliacao/FecharBidding", { Codigo: _bidding.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Bidding Fechado com sucesso.");
                var registroSelecionado = new Object();
                registroSelecionado.Codigo = _bidding.Codigo.val();
                LimparCamposBiddingAvaliacao();
                CarregarClick(registroSelecionado);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function FinalizarEtapaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar a etapa do Bidding?", function () {
        executarReST("BiddingAvaliacao/FinalizarEtapa", { Codigo: _bidding.Codigo.val(), VencedoresDefinidos: JSON.stringify(_selecionarVencedores.VencedoresDefinidos.list) }, function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa finalizada com sucesso.");
                var registroSelecionado = new Object();
                registroSelecionado.Codigo = _bidding.Codigo.val();
                LimparCamposBiddingAvaliacao();
                CarregarClick(registroSelecionado);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function BuscarDuvidas() {
    _duvidas.Duvidas.val([]);
    executarReST("BiddingAvaliacao/BuscarDuvidas", { Codigo: _bidding.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                for (var i = 0; i < retorno.Data.Duvidas.length; i++) {
                    const knoutDuvida = new Duvida(retorno.Data.Duvidas[i]);
                    _duvidas.Duvidas.val.push(knoutDuvida);
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function NotificarInteressadosClick() {
    exibirConfirmacao("Confirmação", "Notificar Interessados?", function () {
        executarReST("BiddingAvaliacao/NotificarInteressados", { Codigo: _bidding.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Notificações enviadas.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function RecarregarBiddingClick() {
    _gridOfertas.CarregarGrid();
    _gridOfertasComponente.CarregarGrid();
    RecarregarGridConviteChecklist();
    RecarregarSimulacoes();
    PreencherGridLinhasTotais();
}

function RecarregarSimulacoes() {

    _biddingResumo.GridRankingOferas.val(JSON.stringify(_gridRankingOfertasDados));
    _biddingResumo.GridRankingCherryPickingOferas.val(JSON.stringify(_gridRankingCherryPickingOfertasDados));
    _gridRankingOfertas.CarregarGrid();
    _gridRankingCherryPickingOfertas.CarregarGrid();
}

//#endregion

//#region Funções Preencher

function preencherPrazos(prazos) {
    if (_timerBiddingConvite != null) {
        clearInterval(_timerBiddingConvite);
        _timerBiddingConvite = null;
        _biddingConvites.TempoRestante.val("");
    }

    if (_timerBiddingCheckList != null) {
        clearInterval(_timerBiddingCheckList);
        _timerBiddingCheckList = null;
        _biddingChecklist.TempoRestante.val("");
    }

    if (_timerBiddingOferta != null) {
        clearInterval(_timerBiddingOferta);
        _timerBiddingOferta = null;
        _biddingOfertas.TempoRestante.val("");
    }

    var dataBiddingOferta = new Date();
    var dataBiddingConvite = new Date();

    if (prazos.PrazoConvite != "" && prazos.PrazoConvite != 0)
        dataBiddingConvite = Global.criarData(prazos.PrazoConvite).getTime();

    if (prazos.PrazoOferta != "" && prazos.PrazoOferta != 0)
        dataBiddingOferta = Global.criarData(prazos.PrazoOferta).getTime();

    var dataBiddingCheckList = new Date().getTime();

    if (prazos.PrazoCheckList != null && prazos.PrazoCheckList != 0)
        dataBiddingCheckList = Global.criarData(prazos.PrazoCheckList).getTime();

    _timerBiddingConvite = setInterval(function () {
        const now = new Date().getTime();
        const distance = dataBiddingConvite - now;

        const days = Math.floor(distance / (1000 * 60 * 60 * 24));
        const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((distance % (1000 * 60)) / 1000);

        _biddingConvites.TempoRestante.val(days + "d " + hours + "h "
            + minutes + "m " + seconds + "s ");

        if (distance < 0) {
            clearInterval(_timerBiddingConvite);
            _biddingConvites.TempoRestante.val("Prazo encerrado");
        }

        if (_CONFIGURACAO_TMS.PermiteRemoverObrigatoriedadeDatas) {
            _biddingOfertas.TempoRestante.val("");
        }
    }, 1000);

    _timerBiddingCheckList = setInterval(function () {
        const now = new Date().getTime();
        const distance = dataBiddingCheckList - now;

        const days = Math.floor(distance / (1000 * 60 * 60 * 24));
        const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((distance % (1000 * 60)) / 1000);

        _biddingChecklist.TempoRestante.val(days + "d " + hours + "h "
            + minutes + "m " + seconds + "s ");

        if (distance < 0) {
            clearInterval(_timerBiddingCheckList);
            _biddingChecklist.TempoRestante.val("Prazo encerrado");
        }

        if (_CONFIGURACAO_TMS.PermiteRemoverObrigatoriedadeDatas) {
            _biddingOfertas.TempoRestante.val("");
        }
    }, 1000);

    _timerBiddingOferta = setInterval(function () {
        const now = new Date().getTime();
        const distance = dataBiddingOferta - now;

        const days = Math.floor(distance / (1000 * 60 * 60 * 24));
        const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((distance % (1000 * 60)) / 1000);

        _biddingOfertas.TempoRestante.val(days + "d " + hours + "h "
            + minutes + "m " + seconds + "s ");

        if (distance < 0) {
            clearInterval(_timerBiddingOferta);
            _biddingOfertas.TempoRestante.val("Prazo encerrado");
        }

        if (_CONFIGURACAO_TMS.PermiteRemoverObrigatoriedadeDatas) {
            _biddingOfertas.TempoRestante.val("");
        }
    }, 1000);
}

function PreencherGridConvites(convites) {
    _gridConvites.CarregarGrid(convites);
}

function PreencherGridChecklist(checklist) {
    _gridChecklist.CarregarGrid(checklist);
}

async function PreencherGridOfertas() {
    _gridOfertas.AtualizarRegistrosSelecionados([]);
    await _gridOfertas.CarregarGrid(VerificarBotaoSelecionarVencedores);
}

function VerificarBotaoSelecionarVencedores(retornoData) {
    _biddingOfertas.SelecionarVencedores.visible(true);
    if (retornoData.recordsTotal - 2 <= 0) {
        _biddingOfertas.SelecionarVencedores.visible(false);
    }
    PreencherGridOfertasComponente();
}

function PreencherGridOfertasComponente() {

    _biddingResumo.GridOfertas.val(JSON.stringify(_gridOfertas.GridViewTableData()))

    _gridOfertasComponente.AtualizarRegistrosSelecionados([]);
    _gridOfertasComponente.CarregarGrid();


    _gridRankingOfertas.CarregarGrid(function () {
        _gridRankingOfertasDados = _gridRankingOfertas.GridViewTableData();
    });

    _gridRankingCherryPickingOfertas.CarregarGrid(function () {
        _gridRankingCherryPickingOfertasDados = _gridRankingCherryPickingOfertas.GridViewTableData();
    });

}

function PreencherOpcoesOfertasRota(listaRotas) {
    var rotas = [{ text: "Todas", value: "" }];

    for (var i = 0; i < listaRotas.length; i++) {
        rotas.push({
            text: listaRotas[i].Descricao,
            value: listaRotas[i].Codigo
        });
    }

    _biddingOfertas.Rotas.options(rotas);

}

function PreencherOpcoesOfertasModeloVeicular(listaModelosVeiculares) {
    var modelosVeiculares = [{ text: "Todas", value: "" }];

    for (var i = 0; i < listaModelosVeiculares.length; i++) {
        modelosVeiculares.push({
            text: listaModelosVeiculares[i].Descricao,
            value: listaModelosVeiculares[i].Codigo
        });
    }

    _biddingOfertas.ModeloVeicular.options(modelosVeiculares);
}

function PreencherOpcoesOfertasFilial(listaFilial) {
    var filial = [{ text: "Todas", value: "" }];

    if (listaFilial.length == 0)
        _biddingOfertas.Filial.visible(false)
    else {

        for (var i = 0; i < listaFilial.length; i++) {
            filial.push({
                text: listaFilial[i].Descricao,
                value: listaFilial[i].Codigo
            });
        }

        _biddingOfertas.Filial.options(filial);
    }
}

function PreencherOpcoesOfertasOrigem(listaOrigem) {
    var origem = [{ text: "Todas", value: "" }];

    if (listaOrigem.length == 0)
        _biddingOfertas.Origem.visible(false)
    else {

        for (var i = 0; i < listaOrigem.length; i++) {
            origem.push({
                text: listaOrigem[i].Descricao,
                value: listaOrigem[i].Codigo
            });
        }

        _biddingOfertas.Origem.options(origem);
    }
}

function PreencherOpcoesOfertasDestino(listaDestino) {
    var destino = [{ text: "Todas", value: "" }];

    if (listaDestino.length == 0)
        _biddingOfertas.Destino.visible(false)
    else {

        for (var i = 0; i < listaDestino.length; i++) {
            destino.push({
                text: listaDestino[i].Descricao,
                value: listaDestino[i].Codigo
            });
        }

        _biddingOfertas.Destino.options(destino);
    }
}

function PreencherOpcoesOfertasMesorregiaoDestino(listaMesorregiaoDestino) {
    var mesorregiaoDestino = [{ text: "Todas", value: "" }];

    if (listaMesorregiaoDestino.length == 0)
        _biddingOfertas.MesorregiaoDestino.visible(false)
    else {

        for (var i = 0; i < listaMesorregiaoDestino.length; i++) {
            mesorregiaoDestino.push({
                text: listaMesorregiaoDestino[i].Descricao,
                value: listaMesorregiaoDestino[i].Codigo
            });
        }

        _biddingOfertas.MesorregiaoDestino.options(mesorregiaoDestino);
    }
}

function PreencherOpcoesOfertasMesorregiaoOrigem(listaMesorregiaoOrigem) {
    var mesorregiaoOrigem = [{ text: "Todas", value: "" }];

    if (listaMesorregiaoOrigem.length == 0)
        _biddingOfertas.MesorregiaoOrigem.visible(false)
    else {

        for (var i = 0; i < listaMesorregiaoOrigem.length; i++) {
            mesorregiaoOrigem.push({
                text: listaMesorregiaoOrigem[i].Descricao,
                value: listaMesorregiaoOrigem[i].Codigo
            });
        }

        _biddingOfertas.MesorregiaoOrigem.options(mesorregiaoOrigem);
    }
}

function PreencherOpcoesOfertasRegiaoDestino(listaRegiaoDestino) {
    var regiaoDestino = [{ text: "Todas", value: "" }];

    if (listaRegiaoDestino.length == 0)
        _biddingOfertas.RegiaoDestino.visible(false)
    else {

        for (var i = 0; i < listaRegiaoDestino.length; i++) {
            regiaoDestino.push({
                text: listaRegiaoDestino[i].Descricao,
                value: listaRegiaoDestino[i].Codigo
            });
        }

        _biddingOfertas.RegiaoDestino.options(regiaoDestino);
    }
}

function PreencherOpcoesOfertasRegiaoOrigem(listaRegiaoOrigem) {
    var regiaoOrigem = [{ text: "Todas", value: "" }];

    if (listaRegiaoOrigem.length == 0)
        _biddingOfertas.RegiaoOrigem.visible(false)
    else {

        for (var i = 0; i < listaRegiaoOrigem.length; i++) {
            regiaoOrigem.push({
                text: listaRegiaoOrigem[i].Descricao,
                value: listaRegiaoOrigem[i].Codigo
            });
        }

        _biddingOfertas.RegiaoOrigem.options(regiaoOrigem);
    }
}

function PreencherOpcoesOfertasRotaDestino(listaRotaDestino) {
    var rotaDestino = [{ text: "Todas", value: "" }];

    if (listaRotaDestino.length == 0)
        _biddingOfertas.RotaDestino.visible(false)
    else {

        for (var i = 0; i < listaRotaDestino.length; i++) {
            rotaDestino.push({
                text: listaRotaDestino[i].Descricao,
                value: listaRotaDestino[i].Codigo
            });
        }

        _biddingOfertas.RotaDestino.options(rotaDestino);
    }
}

function PreencherOpcoesOfertasRotaOrigem(listaRotaOrigem) {
    var rotaOrigem = [{ text: "Todas", value: "" }];

    if (listaRotaOrigem.length == 0)
        _biddingOfertas.RotaOrigem.visible(false)
    else {

        for (var i = 0; i < listaRotaOrigem.length; i++) {
            rotaOrigem.push({
                text: listaRotaOrigem[i].Descricao,
                value: listaRotaOrigem[i].Codigo
            });
        }

        _biddingOfertas.RotaOrigem.options(rotaOrigem);
    }
}

function PreencherOpcoesOfertasClienteDestino(listaClienteDestino) {
    var ClienteDestino = [{ text: "Todas", value: "" }];

    if (listaClienteDestino.length == 0)
        _biddingOfertas.ClienteDestino.visible(false)
    else {

        for (var i = 0; i < listaClienteDestino.length; i++) {
            ClienteDestino.push({
                text: listaClienteDestino[i].Descricao,
                value: listaClienteDestino[i].Codigo
            });
        }

        _biddingOfertas.ClienteDestino.options(ClienteDestino);
    }
}

function PreencherOpcoesOfertasClienteOrigem(listaClienteOrigem) {
    var ClienteOrigem = [{ text: "Todas", value: "" }];

    if (listaClienteOrigem.length == 0)
        _biddingOfertas.ClienteOrigem.visible(false)
    else {

        for (var i = 0; i < listaClienteOrigem.length; i++) {
            ClienteOrigem.push({
                text: listaClienteOrigem[i].Descricao,
                value: listaClienteOrigem[i].Codigo
            });
        }

        _biddingOfertas.ClienteOrigem.options(ClienteOrigem);
    }
}

function PreencherOpcoesOfertasEstadoDestino(listaEstadoDestino) {
    var EstadoDestino = [{ text: "Todas", value: "" }];

    if (listaEstadoDestino.length == 0)
        _biddingOfertas.EstadoDestino.visible(false)
    else {

        for (var i = 0; i < listaEstadoDestino.length; i++) {
            EstadoDestino.push({
                text: listaEstadoDestino[i].Descricao,
                value: listaEstadoDestino[i].Codigo
            });
        }

        _biddingOfertas.EstadoDestino.options(EstadoDestino);
    }
}

function PreencherOpcoesOfertasEstadoOrigem(listaEstadoOrigem) {
    var EstadoOrigem = [{ text: "Todas", value: "" }];

    if (listaEstadoOrigem.length == 0)
        _biddingOfertas.EstadoOrigem.visible(false)
    else {

        for (var i = 0; i < listaEstadoOrigem.length; i++) {
            EstadoOrigem.push({
                text: listaEstadoOrigem[i].Descricao,
                value: listaEstadoOrigem[i].Codigo
            });
        }

        _biddingOfertas.EstadoOrigem.options(EstadoOrigem);
    }
}

function PreencherOpcoesOfertasPaisDestino(listaPaisDestino) {
    var PaisDestino = [{ text: "Todas", value: "" }];

    if (listaPaisDestino.length == 0)
        _biddingOfertas.PaisDestino.visible(false)
    else {

        for (var i = 0; i < listaPaisDestino.length; i++) {
            PaisDestino.push({
                text: listaPaisDestino[i].Descricao,
                value: listaPaisDestino[i].Codigo
            });
        }

        _biddingOfertas.PaisDestino.options(PaisDestino);
    }
}

function PreencherOpcoesOfertasPaisOrigem(listaPaisOrigem) {
    var PaisOrigem = [{ text: "Todas", value: "" }];

    if (listaPaisOrigem.length == 0)
        _biddingOfertas.PaisOrigem.visible(false)
    else {

        for (var i = 0; i < listaPaisOrigem.length; i++) {
            PaisOrigem.push({
                text: listaPaisOrigem[i].Descricao,
                value: listaPaisOrigem[i].Codigo
            });
        }

        _biddingOfertas.PaisOrigem.options(PaisOrigem);
    }
}

function PreencherOpcoesOfertasCEPDestino(possuiCEPDestino) {
    _biddingOfertas.CEPDestino.visible(possuiCEPDestino)
}

function PreencherOpcoesOfertasCEPOrigem(possuiCEPOrigem) {
    _biddingOfertas.CEPOrigem.visible(possuiCEPOrigem)
}

function PreencherOpcoesQuantidadeEntregas(listaQuantidadeEntregas) {
    var entregas = [{ text: "Todas", value: "" }];

    if (listaQuantidadeEntregas.length == 0)
        _biddingOfertas.QuantidadeEntregas.visible(false);
    else {
        for (var i = 0; i < listaQuantidadeEntregas.length; i++) {
            entregas.push({
                text: listaQuantidadeEntregas[i],
                value: listaQuantidadeEntregas[i]
            });
        }
        _biddingOfertas.QuantidadeEntregas.options(entregas);
    }
}

function PreencherOpcoesQuantidadeAjudantes(listaQuantidadeAjudantes) {
    var ajudantes = [{ text: "Todas", value: "" }];

    if (listaQuantidadeAjudantes.length == 0)
        _biddingOfertas.QuantidadeAjudantes.visible(false);
    else {
        for (var i = 0; i < listaQuantidadeAjudantes.length; i++) {
            ajudantes.push({
                text: listaQuantidadeAjudantes[i],
                value: listaQuantidadeAjudantes[i]
            });
        }
        _biddingOfertas.QuantidadeAjudantes.options(ajudantes);
    }
}

function PreencherOpcoesQuantidadeViagensAno(listaQuantidadeViagensAno) {
    var viagens = [{ text: "Todas", value: "" }];

    if (listaQuantidadeViagensAno.length == 0)
        _biddingOfertas.QuantidadeViagensAno.visible(false);
    else {
        for (var i = 0; i < listaQuantidadeViagensAno.length; i++) {
            viagens.push({
                text: listaQuantidadeViagensAno[i],
                value: listaQuantidadeViagensAno[i]
            });
        }
        _biddingOfertas.QuantidadeViagensAno.options(viagens);
    }
}

function PreencherGridsDetalheOferta(ofertas) {
    _gridEquipamento.CarregarGrid(ofertas.Equipamento);
    _gridPorcentagem.CarregarGrid(ofertas.PorcentagemSobreNota);
    _gridFrotaFixaFranquia.CarregarGrid(ofertas.FrotaFixaFranquia);
    _gridFretePorPeso.CarregarGrid(ofertas.FretePorPeso);
    _gridFretePorCapacidade.CarregarGrid(ofertas.FretePorCapacidade);
    _gridFretePorViagem.CarregarGrid(ofertas.FretePorViagem);
    _gridViagemEntregaAjudante.CarregarGrid(ofertas.ViagemEntregaAjudante);
    _gridFrotaFixaKm.CarregarGrid(ofertas.FrotaFixaKm);
    _gridViagemAdicional.CarregarGrid(ofertas.ViagemAdicional);
}

function PreencherOfertaOptions(ofertas) {
    var arrayOpcoes = [];

    for (var i = 0; i < ofertas.length; i++) {
        var opcao = { text: ofertas[i].Descricao, value: ofertas[i].Codigo };
        arrayOpcoes.push(opcao);
    }

    _biddingOfertaDetalhes.Oferta.options(arrayOpcoes);
}

function PreencherSelecionarVencedores(dadosVencedores) {
    for (var i = 0; i < dadosVencedores.length; i++) {
        _selecionarVencedores.TransportadoresDasRotas.list.push(dadosVencedores[i]);
    }
}

function PreencherTransportadoresFiltroRanking(transportadores) {

    _biddingRankingOfertas.CodigosTransportadores.val(transportadores.map(item => item.TransportadorCodigo))
}

function CallbackFiltroTransportadorRanking(transportador) {

    _biddingRankingOfertas.Transportador.val(transportador.Descricao);
    _biddingRankingOfertas.Transportador.codEntity = transportador.Codigo;

    var registosSelecionados = _gridRankingOfertas.ObterMultiplosSelecionados().map(function (registro) {
        return registro.Codigo;
    });

    registosSelecionados.push(_linhaTransportadorSimulado)

    var dados = _gridRankingOfertas.GridViewTableData();

    registosSelecionados.forEach(function (registro) {

        var linha = dados.find(item => item.Codigo === registro);
        linha.CodigoTransportadorSimulado = transportador.Codigo;

        linha = _gridRankingOfertasDados.find(item => item.Codigo === registro);

        if (linha)
            linha.CodigoTransportadorSimulado = transportador.Codigo;

    });

    _biddingResumo.GridRankingOferas.val(JSON.stringify(dados));

    _gridRankingOfertas.CarregarGrid();

    _gridRankingOfertas.AtualizarRegistrosNaoSelecionados(new Array());
    _gridRankingOfertas.AtualizarRegistrosSelecionados(new Array());
    _biddingRankingOfertas.SelecionarTodos.val(false);
}

function CallbackFiltroTransportadorRankingCherryPicking(transportador) {

    _biddingRankingCherryPickingOfertas.Transportador.val(transportador.Descricao);
    _biddingRankingCherryPickingOfertas.Transportador.codEntity = transportador.Codigo;

    var registosSelecionados = _gridRankingCherryPickingOfertas.ObterMultiplosSelecionados().map(function (registro) {
        return registro.Codigo;
    });

    registosSelecionados.push(_linhaTransportadorSimulado)

    var dados = _gridRankingCherryPickingOfertas.GridViewTableData();

    registosSelecionados.forEach(function (registro) {

        var linha = dados.find(item => item.Codigo === registro);
        linha.CodigoTransportadorSimulado = transportador.Codigo;

        linha = _gridRankingCherryPickingOfertasDados.find(item => item.Codigo === registro);
        if (linha)
            linha.CodigoTransportadorSimulado = transportador.Codigo;
    });

    _biddingResumo.GridRankingCherryPickingOferas.val(JSON.stringify(dados));

    _gridRankingCherryPickingOfertas.CarregarGrid();

    _gridRankingCherryPickingOfertas.AtualizarRegistrosNaoSelecionados(new Array());
    _gridRankingCherryPickingOfertas.AtualizarRegistrosSelecionados(new Array());
    _biddingRankingCherryPickingOfertas.SelecionarTodos.val(false);
}

function PreencherModalOfertasComponentes(dadosModalOfertasComponente) {
    _modalOfertasComponente.ModalOfertasComponentes.list = [];

    for (var i = 0; i < dadosModalOfertasComponente.length; i++) {
        _modalOfertasComponente.ModalOfertasComponentes.list.push(dadosModalOfertasComponente[i]);
    }
}

function PreencherGridFechamento() {
    _gridFechamento.CarregarGrid();
}

function PreencherOpcoesFechamentoRota(listaRotas) {
    var rotas = [{ text: "Todas", value: "" }];

    for (var i = 0; i < listaRotas.length; i++) {
        rotas.push({
            text: listaRotas[i].Descricao,
            value: listaRotas[i].Codigo
        });
    }

    _biddingFechamento.Rotas.options(rotas);
}
function PreencherGridLinhasTotais(api) {
    PreencherLinhasTotaisUnificado(api);
}

function PreencherLinhasTotaisUnificado(api) {
    const gridId = `#${_gridOfertas.GetGridId()} tbody`;

    // 🔹 Remover linhas de TOTAL e TOTAL SPEND se já existirem
    $(gridId + " tr#-1").remove();
    $(gridId + " tr#-2").remove();

    // Captura os dados da grid
    const dados = _gridOfertas.GridViewTableData();
    const dadosTabela = _gridOfertas.GetHeader();

    // 🔹 Verificações para evitar erros
    if (!dados || dados.length === 0) {
        console.warn("Nenhum dado encontrado na tabela.");
        return;
    }
    if (!dadosTabela || dadosTabela.length === 0) {
        console.warn("Nenhuma configuração de colunas encontrada.");
        return;
    }

    // Obtém dinamicamente os índices das colunas fixas
    let columnFixedIndexes = [];
    api.columns().every(function (index) {
        let headerText = api.column(index).header().textContent.trim();
        if (["Rota", "Origem", "Destino", "Quantidade Viagem Ano", "Quantidade de Entregas", "Quantidade de Ajudantes por Veículo", "Baseline", "Volume (Ton) Ano"].includes(headerText)) {
            columnFixedIndexes.push(index);
        }
    });

    let dadosDinamicosTotal = "";
    let dadosDinamicosTotalSpend = "";
    let encontrouTotal = false;
    let encontrouTotalSpend = false;

    for (let i = 0; i < dados.length; i++) {
        if (dados[i].DT_RowId == -1 && !encontrouTotal) {
            encontrouTotal = true;

            let arrayColunasTrasportador = []
            if (dados[i]['ColunasTrasportadorString']) {
                arrayColunasTrasportador = JSON.parse(dados[i]['ColunasTrasportadorString']);
            }
            const nomesColunasTrasportadorComNome = arrayColunasTrasportador.map(x => {
                return { ...x, NomeColuna: `ColunaTransportador${x.CodigoTrasportador}` }
            });

            for (let j = 0; j < dadosTabela.length; j++) {
                if (!dadosTabela[j] || !dadosTabela[j].data) continue;
                if (dadosTabela[j].visible === true && dadosTabela[j].data !== "RotaDescricao") {
                    let valorCelula = dados[i][dadosTabela[j].data] || "";

                    if (typeof valorCelula === "string" && !valorCelula.includes("%")) {
                        valorCelula = valorCelula.replace(/\./g, "").replace(",", ".");
                    }

                    if (!isNaN(parseFloat(valorCelula)) && isFinite(valorCelula)) {
                        valorCelula = parseFloat(valorCelula)
                            .toLocaleString("pt-BR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    }

                    const colunaTransportador = nomesColunasTrasportadorComNome.find(x => x.NomeColuna === dadosTabela[j].data);
                    if (colunaTransportador) {
                        valorCelula = colunaTransportador.Valor;
                    }

                    let destaqueClass = (dadosTabela[j].data === "ImpactoReais" || dadosTabela[j].data === "ImpactoPercentual") ? 'class="destaque-positivo"' : '';

                    let classDinamicos = "text-center";
                    if (columnFixedIndexes.includes(j)) {
                        classDinamicos = "text-start grid-view-fixed-left-column-scroll";
                    }

                    dadosDinamicosTotal += `<td class="${classDinamicos}" data-column-index="${j}">
                        <input type="checkbox" data-bind="checked" hidden="">
                        <span ${destaqueClass} title="${valorCelula}">${valorCelula}</span>
                    </td>`;
                }
            }

            $(gridId).append(`
                <tr id="-1" class="odd total" role="row" style="cursor: pointer;">
                    <td class="text-start grid-view-fixed-left-column-scroll" style="left:0px;"><span title="TOTAL">TOTAL</span></td>${dadosDinamicosTotal}
                </tr>
            `);
        }

        if (dados[i].DT_RowId == -2 && !encontrouTotalSpend) {
            encontrouTotalSpend = true;

            let arrayColunasTrasportador = dados[i]['ColunasTrasportadorString'] ? JSON.parse(dados[i]['ColunasTrasportadorString']) : [];
            const nomesColunasTrasportadorComNome = arrayColunasTrasportador.map(x => ({ ...x, NomeColuna: `ColunaTransportador${x.CodigoTrasportador}` }));

            let dadosDinamicosTotalSpend = "";

            for (let j = 0; j < dadosTabela.length; j++) {
                const coluna = dadosTabela[j];
                if (!coluna || !coluna.data || coluna.data === "RotaDescricao" || !coluna.visible) continue;

                let valorCelula = dados[i][coluna.data] || "";

                if (typeof valorCelula === "string" && !valorCelula.includes("%")) {
                    valorCelula = valorCelula.replace(/\./g, "").replace(",", ".");
                }

                if (!isNaN(parseFloat(valorCelula)) && isFinite(valorCelula)) {
                    valorCelula = parseFloat(valorCelula).toLocaleString("pt-BR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                }

                const colunaTransportador = nomesColunasTrasportadorComNome.find(x => x.NomeColuna === coluna.data);
                if (colunaTransportador) {
                    valorCelula = colunaTransportador.Valor;
                }

                let destaqueClass = (coluna.data === "ImpactoReais" || coluna.data === "ImpactoPercentual") ? 'class="destaque-positivo"' : '';
                let classDinamicos = columnFixedIndexes.includes(j) ? "text-start grid-view-fixed-left-column-scroll" : "text-center";

                dadosDinamicosTotalSpend += `<td class="${classDinamicos}" data-column-index="${j}">
                    <input type="checkbox" data-bind="checked" hidden="">
                    <span ${destaqueClass} title="${valorCelula}">${valorCelula}</span>
                </td>`;
            }

            $(gridId).append(`
            <tr id="-2" class="even total-spend" role="row" style="cursor: pointer;">
                <td class="text-start grid-view-fixed-left-column-scroll" style="left:0px;">
                    <span title="TOTAL SPEND">TOTAL SPEND</span>
                </td>${dadosDinamicosTotalSpend}
            </tr>
            `);
        }
    }
}
function FiltrarOpcoesFiltrosPorRota() {
    if (_bidding.Codigo.val() <= 0 || _biddingResumo.CodigoBiddingOferta <= 0) {
        return;
    }
    var data = RetornarObjetoPesquisa(_biddingResumo);
    data.CodigoBiddingOferta = JSON.stringify(data.CodigoBiddingOferta);
    executarReST("BiddingAvaliacao/FiltrarOpcoesFiltrosPorRota", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                SetarVisibilidadeDosFiltros(true);
                //// PREENCHIMENTO DOS FILTROS DE PESQUISA
                PreencherOpcoesOfertasRota(retorno.Data.Filtros.ListaRotas);
                PreencherOpcoesOfertasModeloVeicular(retorno.Data.Filtros.ListaModelosVeiculares);
                PreencherOpcoesOfertasFilial(retorno.Data.Filtros.ListaFilial);
                PreencherOpcoesQuantidadeEntregas(retorno.Data.Filtros.ListaQuantidadeEntregas);
                PreencherOpcoesQuantidadeAjudantes(retorno.Data.Filtros.ListaQuantidadeAjudantes);
                PreencherOpcoesQuantidadeViagensAno(retorno.Data.Filtros.ListaQuantidadeViagensAno);
                PreencherOpcoesOfertasOrigem(retorno.Data.Filtros.ListaOrigem); // Cidades Origem
                PreencherOpcoesOfertasDestino(retorno.Data.Filtros.ListaDestino); // Cidades Destino
                PreencherOpcoesOfertasMesorregiaoDestino(retorno.Data.Filtros.ListaMesorregiaoDestino);
                PreencherOpcoesOfertasMesorregiaoOrigem(retorno.Data.Filtros.ListaMesorregiaoOrigem);
                PreencherOpcoesOfertasRegiaoDestino(retorno.Data.Filtros.ListaRegiaoDestino); //Regioes Brasil Destino
                PreencherOpcoesOfertasRegiaoOrigem(retorno.Data.Filtros.ListaRegiaoOrigem); //Regioes Brasil Origem
                PreencherOpcoesOfertasRotaDestino(retorno.Data.Filtros.ListaRotaDestino); // Rotas Destino
                PreencherOpcoesOfertasRotaOrigem(retorno.Data.Filtros.ListaRotaOrigem); // Rotas Origem
                PreencherOpcoesOfertasClienteDestino(retorno.Data.Filtros.ListaClienteDestino); // Clientes Destino
                PreencherOpcoesOfertasClienteOrigem(retorno.Data.Filtros.ListaClienteOrigem); // Clientes Origem
                PreencherOpcoesOfertasEstadoDestino(retorno.Data.Filtros.ListaEstadoDestino); // Estados Destino
                PreencherOpcoesOfertasEstadoOrigem(retorno.Data.Filtros.ListaEstadoOrigem); // Estados Origem
                PreencherOpcoesOfertasPaisDestino(retorno.Data.Filtros.ListaPaisDestino); // Paises Destino
                PreencherOpcoesOfertasPaisOrigem(retorno.Data.Filtros.ListaPaisOrigem); // Paises Origem
                PreencherOpcoesOfertasCEPDestino(retorno.Data.Filtros.PossuiCEPDestino); // CEPs Destino
                PreencherOpcoesOfertasCEPOrigem(retorno.Data.Filtros.PossuiCEPOrigem); // CEPs Origem
                PreencherOpcoesFechamentoRota(retorno.Data.Filtros.ListaRotas);
            }
        }
    });
}

function VisibilidadeOpcaoInformacoes(data) {
    return data.Codigo != 0;
}

function exibirMultiplasOpcoesOferta() {
    var existemRegistrosSelecionados = _gridOfertas.ObterMultiplosSelecionados();

    for (var i = existemRegistrosSelecionados.length; i > 0; i--) {
        var indice = (i - 1);

        if (existemRegistrosSelecionados[indice].Codigo == 0) {
            _gridOfertas.AtualizarRegistrosNaoSelecionados([existemRegistrosSelecionados[indice]]);
            existemRegistrosSelecionados.splice(indice, 1);
        }
    }
    _gridOfertas.AtualizarRegistrosSelecionados(existemRegistrosSelecionados);
    _gridOfertas.DrawTable(true);

    var selecionadoTodos = _biddingOfertas.SelecionarTodos.val();
    existemRegistrosSelecionados = existemRegistrosSelecionados.length > 0

    _biddingOfertas.ProporRodadaSelecionados.visible((existemRegistrosSelecionados || selecionadoTodos) && _bidding.Situacao.val() != EnumStatusBidding.Fechamento);
}

function SetarVisibilidadeDosFiltros(bool) {
    _biddingOfertas.Filial.visible(bool);
    _biddingOfertas.Origem.visible(bool);
    _biddingOfertas.Origem.visible(bool);
    _biddingOfertas.Destino.visible(bool)
    _biddingOfertas.Destino.visible(bool)
    _biddingOfertas.MesorregiaoDestino.visible(bool);
    _biddingOfertas.MesorregiaoOrigem.visible(bool);
    _biddingOfertas.QuantidadeAjudantes.visible(bool);
    _biddingOfertas.QuantidadeViagensAno.visible(true);
    _biddingOfertas.QuantidadeEntregas.visible(bool);
    _biddingOfertas.RegiaoDestino.visible(bool);
    _biddingOfertas.RegiaoOrigem.visible(bool);
    _biddingOfertas.RotaDestino.visible(bool);
    _biddingOfertas.RotaOrigem.visible(bool);
    _biddingOfertas.ClienteDestino.visible(bool);
    _biddingOfertas.ClienteOrigem.visible(bool);
    _biddingOfertas.EstadoDestino.visible(bool);
    _biddingOfertas.EstadoOrigem.visible(bool);
    _biddingOfertas.PaisDestino.visible(bool);
    _biddingOfertas.PaisOrigem.visible(bool);
    _biddingOfertas.CEPOrigem.visible(bool);
    _biddingOfertas.CEPDestino.visible(bool);
}

function SetarVisibilidadeFiltrosPorTipoLance(tipoLance) {
    if (tipoLance == EnumTipoLanceBidding.LancePorPeso || tipoLance == EnumTipoLanceBidding.LancePorCapacidade || tipoLance == EnumTipoLanceBidding.LancePorPeso.LancePorFreteViagem) {
        _biddingOfertas.MesorregiaoDestino.visible(false);
        _biddingOfertas.MesorregiaoOrigem.visible(false);
        _biddingOfertas.QuantidadeEntregas.visible(false);
        _biddingOfertas.QuantidadeAjudantes.visible(false);
    }
}

function limparFiltrosConsultaBidding() {
    LimparCampos(_biddingOfertas);
}

//#endregion