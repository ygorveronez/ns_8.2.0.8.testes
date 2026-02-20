/// <reference path="Etapas.js" />
/// <reference path="AnexoQuestionario.js" />
/// <reference path="../../Enumeradores/EnumTipoOfertaBidding.js" />
/// <reference path="../../Enumeradores/EnumBiddingOfertaSituacao.js" />
/// <reference path="../../Enumeradores/EnumTipoLanceBidding.js" />

//#region Variáveis Globais

var _pesquisaBiddingConvite, _biddingAceitacao, _modalRotas, _duvidas, _CRUDQuestao, _CRUDBidding, _naoOfertar;
var _gridChecklistQuestionarios, _gridRotasBasicTable, _gridAnexo, _gridAnexoTipoBidding, _gridAnexoQuestionario, _gridOrigem, _gridDestino, _gridCarga, _gridVeiculo, _gridOfertasRotas, _gridResultados;
var _gridEquipamento;
var _gridPorcentagem;
var _gridFrotaFixaKm;
var _gridViagemAdicional;
var _gridFrotaFixaFranquia;
var _gridFretePorPeso;
var _gridFretePorCapacidade;
var _gridFretePorViagem;
var _gridViagemEntregaAjudante;
var _codigoPergunta;
var dataFinalAceitamentoConvite, dataFinalChecklist;
var rotas;
var todosResultados = [];
var _listaIds = [];
var _kmMedio = 0;
var _listaRotasNaoOfertar = new Array();

var _respostaOptions = [
    { text: "Não", value: 0 },
    { text: "Sim", value: 1 }
];

var _situacaoOptions = [
    { text: "Todas", value: -1 },
    { text: "Rejeitada", value: 2 },
    { text: "Aprovada", value: 3 }
];

var _rotaOptions = [];

//#endregion

//#region Pesquisa

var PesquisaBiddingConvite = function () {
    this.Descricao = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: "Descrição:" });
    this.DataInicio = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.date, text: "Data Inicio:" });
    this.DataLimite = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.date, text: "Data Limite" });
    this.NumeroBidding = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.int, text: "Número Bidding:" });

    this.ResumoTodos = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.ResumoAgConvite = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.ResumoAgChecklist = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.ResumoAgOfertas = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.ResumoFinalizados = PropertyEntity({ type: types.string, val: ko.observable("") });

    this.Pesquisar = PropertyEntity({ eventClick: PesquisarConvite, type: types.event, text: "Pesquisar", visible: ko.observable(true), idGrid: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({
        val: ko.observable([EnumSituacaoBiddingConvite.Aguardando, EnumSituacaoBiddingConvite.Checklist, EnumSituacaoBiddingConvite.Fechamento, EnumSituacaoBiddingConvite.Ofertas]),
        def: [EnumSituacaoBiddingConvite.Aguardando, EnumSituacaoBiddingConvite.Checklist, EnumSituacaoBiddingConvite.Fechamento, EnumSituacaoBiddingConvite.Ofertas],
        getType: typesKnockout.selectMultiple, text: "Situação:", options: EnumSituacaoBiddingConvite.ObterOpcoesPesquisa()
    });

    this.ClickResumoTodos = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            _pesquisaBiddingConvite.Situacao.val([EnumSituacaoBiddingConvite.Aguardando, EnumSituacaoBiddingConvite.Checklist, EnumSituacaoBiddingConvite.Fechamento, EnumSituacaoBiddingConvite.Ofertas]);
            _gridBiddingConvite.CarregarGrid();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickResumoAgConvite = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            _pesquisaBiddingConvite.Situacao.val([EnumSituacaoBiddingConvite.Aguardando]);
            _gridBiddingConvite.CarregarGrid();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickResumoAgChecklist = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            _pesquisaBiddingConvite.Situacao.val([EnumSituacaoBiddingConvite.Checklist]);
            _gridBiddingConvite.CarregarGrid();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickResumoAgOfertas = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            _pesquisaBiddingConvite.Situacao.val([EnumSituacaoBiddingConvite.Ofertas]);
            _gridBiddingConvite.CarregarGrid();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickResumoFinalizados = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            _pesquisaBiddingConvite.Situacao.val([EnumSituacaoBiddingConvite.Fechamento]);
            _gridBiddingConvite.CarregarGrid();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

//#endregion

//#region Aba Convite

var BiddingAceitacao = function () {
    //Etapa 1
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.Status = PropertyEntity({ getType: typesKnockout.int });
    this.Descritivo = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.text });
    this.AceitoTermo = PropertyEntity({ val: ko.observable(false), text: "Eu aceito os termos de participação.", required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.TempoRestante = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable(""), color: ko.observable("black"), visible: ko.observable(true) });
    this.TipoFrete = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable(""), visible: ko.observable(false) });
    this.AguardandoProximaEtapa = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable("Aguarde a liberação da próxima etapa."), visible: ko.observable(false) });
    this.DataInicioVigencia = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable(""), visible: ko.observable(false) });

    this.Questionarios = PropertyEntity({ type: types.local });
    this.Rotas = PropertyEntity({ type: types.local });
    this.Anexos = PropertyEntity({ type: types.local });
    this.AnexosTipoBidding = PropertyEntity({ type: types.local });
}

var RotaModal = function () {
    this.Origens = PropertyEntity({ type: types.local });
    this.Destinos = PropertyEntity({ type: types.local });
    this.TiposCarga = PropertyEntity({ type: types.local });
    this.ModelosVeiculares = PropertyEntity({ type: types.local });
    this.Descricao = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.QuilometragemMedia = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.Peso = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.AdicionalAPartirDaEntregaNumero = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.Volume = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.Frequencia = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.NumeroEntrega = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.ValorCargaMes = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.Observacao = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.Tomador = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.GrupoModeloVeicular = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.CarroceriaVeiculo = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.FrequenciaMensalComAjudante = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.QuantidadeAjudantesPorVeiculo = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.MediaEntregasFracionadas = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.MaximaEntregasFracionadas = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.Inconterm = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.QuantidadeViagensPorAno = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.VolumeTonAno = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.VolumeTonViagem = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.TempoColeta = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.TempoDescarga = PropertyEntity({ type: types.string, text: ko.observable("") });
    this.Compressor = PropertyEntity({ type: types.string, text: ko.observable("") });
}

//#endregion

//#region Aba Checklist

var BiddingAceitacaoChecklist = function (questao) {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.Pergunta = PropertyEntity({ getType: typesKnockout.string });
    this.Resposta = PropertyEntity({ text: "Resposta:", required: true, getType: typesKnockout.dynamic, options: _respostaOptions, val: ko.observable("1"), def: "1", enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), getType: typesKnockout.text, maxlength: 150, enable: ko.observable(true) });
    this.Anexos = PropertyEntity({ type: types.local });

    this.VerAnexos = PropertyEntity({ eventClick: verAnexos, type: types.event, text: "Ver Anexos", visible: ko.observable(true) });
    this.AnexosQuestionario = PropertyEntity({ eventClick: verAnexosQuestionario, type: types.event, text: "Anexos Questionário", visible: ko.observable(true) });
    this.EnviarAnexo = PropertyEntity({ eventClick: enviarAnexo, type: types.event, text: "Enviar Anexo", visible: ko.observable(true) });

    PreencherObjetoKnout(this, { Data: questao });
}

var Questoes = function () {
    this.TempoRestante = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable(""), color: ko.observable("black"), visible: ko.observable(true) });
    this.Descritivo = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.text, visible: ko.observable(false) });
    this.Questoes = ko.observableArray([]);
}

var QuestionarioAnexo = function () {
    this.Anexos = PropertyEntity({ type: types.local });
}

//#endregion

//#region Aba Ofertas

var BiddingOfertaDetalhes = function () {
    this.Rodada = PropertyEntity({ val: ko.observable("") });
    this.Codigo = PropertyEntity({ val: ko.observable("") });
    this.Equipamento = PropertyEntity({ type: types.map });
    this.FrotaFixaKm = PropertyEntity({ type: types.map });
    this.PorcentagemSobreNota = PropertyEntity({ type: types.map });
    this.ViagemAdicional = PropertyEntity({ type: types.map });
    this.FrotaFixaFranquia = PropertyEntity({ type: types.map });
    this.FretePorPeso = PropertyEntity({ type: types.map });
    this.FretePorCapacidade = PropertyEntity({ type: types.map });
    this.FretePorViagem = PropertyEntity({ type: types.map });
    this.ViagemEntregaAjudante = PropertyEntity({ type: types.map });
}

var TabPaiOferta = function (ofertaRota) {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.CodigoRota = PropertyEntity({ getType: typesKnockout.int });
    this.OfertarNessaModalidade = PropertyEntity({ val: ko.observable(false), text: "Ofertar nesta modalidade.", required: true, getType: typesKnockout.bool });
    this.ValorTransportado = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable("Valor carga mês: "), color: ko.observable("black"), visible: ko.observable(true) });
    this.VolumeTransportado = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable("Volume transportado: "), color: ko.observable("black"), visible: ko.observable(true) });
    this.PesoTransportado = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable("Peso transportado: "), color: ko.observable("black"), visible: ko.observable(true) });
    this.FrequenciaMensal = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable("Frequência mensal: "), color: ko.observable("black"), visible: ko.observable(true) });
    this.NumeroEntregas = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable("Número entregas: "), color: ko.observable("black"), visible: ko.observable(true) });
    this.QuilometragemMedia = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable("Quilometragem média: "), color: ko.observable("black"), visible: ko.observable(true) });
    this.AdicionalAPartirDaEntregaNumero = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable("Adicional a partir da entrega n°: "), color: ko.observable("black"), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable("Tipo de Carga: "), color: ko.observable("black"), visible: ko.observable(true) });
    this.ModelosVeiculares = PropertyEntity({ val: ko.observable([]) });
    this.Rodada = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable("Rodada: "), color: ko.observable("black"), visible: ko.observable(true) });
    this.KnoutsOfertaEquipamento = PropertyEntity({ val: ko.observableArray([]) });
    this.KnoutsOfertaFrotaFixaKmRodado = PropertyEntity({ val: ko.observableArray([]) });
    this.KnoutsOfertaPorcentagem = PropertyEntity({ val: ko.observableArray([]) });
    this.KnoutsOfertaViagemAdicional = PropertyEntity({ val: ko.observableArray([]) });
    this.KnoutsOfertaFrotaFixaFranquia = PropertyEntity({ val: ko.observableArray([]) });
    this.KnoutsOfertaFretePorPeso = PropertyEntity({ val: ko.observableArray([]) });
    this.KnoutsOfertaFretePorCapacidade = PropertyEntity({ val: ko.observableArray([]) });
    this.KnoutsOfertaFretePorViagem = PropertyEntity({ val: ko.observableArray([]) });
    this.Tomador = PropertyEntity({ text: "Tomador: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable("") });
    this.GrupoModeloVeicular = PropertyEntity({ text: "Grupo Modelo Veicular: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable("") });
    this.CarroceriaVeiculo = PropertyEntity({ text: "Carroceria Veículo: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable("") });
    this.FrequenciaMensalComAjudante = PropertyEntity({ text: "Frequência Mensal com Ajudante: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable(0) });
    this.QuantidadeAjudantesPorVeiculo = PropertyEntity({ text: "Quantidade de Ajudantes por Veículo: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable(0) });
    this.MediaEntregasFracionadas = PropertyEntity({ text: "Média de Entregas Fracionadas: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable(0) });
    this.MaximaEntregasFracionadas = PropertyEntity({ text: "Máxima de Entregas Fracionadas: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable(0) });
    this.Inconterm = PropertyEntity({ text: "Inconterm: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable("") });
    this.QuantidadeViagensPorAno = PropertyEntity({ text: "Quantidade de Viagens por Ano: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable(0) });
    this.VolumeTonAno = PropertyEntity({ text: "Volume (Ton) Ano: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable(0) });
    this.VolumeTonViagem = PropertyEntity({ text: "Volume (Ton) Viagem: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable(0) });
    this.TempoColeta = PropertyEntity({ text: "Tempo de Coleta: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable("") });
    this.TempoDescarga = PropertyEntity({ text: "Tempo de Descarga: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable("") });
    this.Compressor = PropertyEntity({ text: "Compressor: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação: ", visible: ko.observable(true), getType: typesKnockout.string, val: ko.observable("") });
    this.KnoutsOfertaViagemEntregaAjudante = PropertyEntity({ val: ko.observableArray([]) });

    PreencherObjetoKnout(this, { Data: ofertaRota });
}

var LanceEquipamento = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ getType: typesKnockout.int });
    this.IdTab = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.OfertarModeloVeiculo = PropertyEntity({ val: ko.observable(false), text: "Ofertar para esse Modelo de Veículo.", required: true, getType: typesKnockout.bool });
    this.ValorFixoMensal = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("Valor Fixo Mensal: "), required: true, enable: ko.observable(false) });
}

var LanceFrotaFixaKmRodado = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ getType: typesKnockout.int });
    this.IdTab = PropertyEntity({ getType: typesKnockout.string });
    this.OfertarModeloVeiculo = PropertyEntity({ val: ko.observable(false), text: "Ofertar para esse Modelo de Veículo.", required: true, getType: typesKnockout.bool });
    this.ValorFixoMensal = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("Valor Fixo Mensal: "), required: true, enable: ko.observable(false) });
    this.ValorKmRodado = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("Valor KM Rodado: "), required: true, enable: ko.observable(false) });
}

var LancePorcentagemNota = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ getType: typesKnockout.int });
    this.IdTab = PropertyEntity({ getType: typesKnockout.string });
    this.OfertarModeloVeiculo = PropertyEntity({ val: ko.observable(false), text: "Ofertar para esse Modelo de Veículo.", required: true, getType: typesKnockout.bool });
    this.PorcentagemNota = PropertyEntity({ val: ko.observable(""), text: "Porcentagem sobre nota: ", required: true, getType: typesKnockout.decimal, enable: ko.observable(false) });
}

var LanceViagemAdicional = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.OfertarModeloVeiculo = PropertyEntity({ val: ko.observable(false), text: "Ofertar para esse Modelo de Veículo.", required: true, getType: typesKnockout.bool });
    this.CodigoVeiculo = PropertyEntity({ getType: typesKnockout.int });
    this.IdTab = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ValorViagem = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("Valor por viagem: "), required: true, enable: ko.observable(false) });
    this.ValorAdicionalEntrega = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("Adicional por Entrega (R$): "), required: true, enable: ko.observable(false) });
}

var LanceFrotaFixaFranquia = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.OfertarModeloVeiculo = PropertyEntity({ val: ko.observable(false), text: "Ofertar para esse Modelo de Veículo.", required: true, getType: typesKnockout.bool });
    this.CodigoVeiculo = PropertyEntity({ getType: typesKnockout.int });
    this.IdTab = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ValorFixo = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("Valor Fixo: "), required: true, enable: ko.observable(false) });
    this.Franquia = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("Valor Por Franquia: "), required: true, enable: ko.observable(false) });
    this.Quilometragem = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("Quilometragem: "), required: true, enable: ko.observable(false) });
}

var LancePorPeso = function (icms) {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.OfertarModeloVeiculo = PropertyEntity({ val: ko.observable(false), text: "Ofertar para esse Modelo de Veículo.", required: true, getType: typesKnockout.bool });
    this.CodigoVeiculo = PropertyEntity({ getType: typesKnockout.int });
    this.IdTab = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ICMS = PropertyEntity({ val: ko.observable(icms), getType: typesKnockout.decimal, text: ko.observable("ICMS(%): "), required: true, enable: ko.observable(false) });
    this.ReplicarICMSDesteModeloVeicular = PropertyEntity({ val: ko.observable(false), text: "Replicar ICMS(%) deste modelo veicular aos demais veículos.", required: true, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.FreteTonelada = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable(""), required: true, enable: ko.observable(false) });
    this.PedagioEixo = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable(""), required: true, enable: ko.observable(false), visible: ko.observable(true) });
}

var LancePorCapacidade = function (icms) {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.OfertarModeloVeiculo = PropertyEntity({ val: ko.observable(false), text: "Ofertar para esse Modelo de Veículo.", required: true, getType: typesKnockout.bool });
    this.CodigoVeiculo = PropertyEntity({ getType: typesKnockout.int });
    this.IdTab = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ICMS = PropertyEntity({ val: ko.observable(icms), getType: typesKnockout.decimal, text: ko.observable("ICMS(%): "), required: true, enable: ko.observable(false) });
    this.ReplicarICMSDesteModeloVeicular = PropertyEntity({ val: ko.observable(false), text: "Replicar ICMS(%) deste modelo veicular aos demais veículos.", required: true, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.FreteTonelada = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable(""), required: true, enable: ko.observable(false) });
    this.PedagioEixo = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable(""), required: true, enable: ko.observable(false), visible: ko.observable(true) });
}

var LancePorFreteViagem = function (icms) {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.OfertarModeloVeiculo = PropertyEntity({ val: ko.observable(false), text: "Ofertar para esse Modelo de Veículo.", required: true, getType: typesKnockout.bool });
    this.CodigoVeiculo = PropertyEntity({ getType: typesKnockout.int });
    this.IdTab = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ICMS = PropertyEntity({ val: ko.observable(icms), getType: typesKnockout.decimal, text: ko.observable("ICMS(%): "), required: true, enable: ko.observable(false) });
    this.ReplicarICMSDesteModeloVeicular = PropertyEntity({ val: ko.observable(false), text: "Replicar ICMS(%) deste modelo veicular aos demais veículos.", required: true, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.FreteTonelada = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable(""), required: true, enable: ko.observable(false) });
    this.PedagioEixo = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable(""), required: true, enable: ko.observable(false), visible: ko.observable(true) });
}

var LanceViagemEntregaAjudante = function (icms) {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.OfertarModeloVeiculo = PropertyEntity({ val: ko.observable(false), text: "Ofertar para esse Modelo de Veículo.", required: true, getType: typesKnockout.bool });
    this.CodigoVeiculo = PropertyEntity({ getType: typesKnockout.int });
    this.IdTab = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ICMS = PropertyEntity({ val: ko.observable(icms), getType: typesKnockout.decimal, text: ko.observable("ICMS(%): "), required: true, enable: ko.observable(false) });
    this.ReplicarICMSDesteModeloVeicular = PropertyEntity({ val: ko.observable(false), text: "Replicar ICMS(%) deste modelo veicular aos demais veículos.", required: true, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.FreteTonelada = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable(""), required: true, enable: ko.observable(false) });
    this.PedagioEixo = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable(""), required: true, enable: ko.observable(false), visible: ko.observable(true) });
    this.Ajudante = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("Ajudante (R$) por Modelo de Veículo: "), required: true, enable: ko.observable(false) });
    this.AdicionalPorEntrega = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("R$ por Entrega Adicional: "), required: true, enable: ko.observable(false) });
}

var BiddingAceitacaoOfertas = function () {
    this.Rotas = PropertyEntity({ type: types.local, id: guid() });
    this.TempoRestante = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: ko.observable(""), color: ko.observable("black"), visible: ko.observable(true) });
    this.Codigo = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
}

var TabsOfertas = function () {
    this.Tabs = PropertyEntity({ val: ko.observable([]) });
}

var TabsDetalhesOfertas = function () {
    this.Tabs = PropertyEntity({ val: ko.observable([]) });
}

var OfertaPai = function () {
    this.EnviarOferta = PropertyEntity({ eventClick: enviarOfertasClick, type: types.event, text: "Enviar Ofertas", visible: ko.observable(true) });
    this.Ofertas = ko.observableArray([]);
    this.Oferta = ko.observableArray([]);
    this.ReplicarICMSDesteModeloVeicular = PropertyEntity({ val: ko.observable(false), text: "Replicar ICMS(%) deste modelo veicular aos demais veículos.", required: true, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.InformarVeiculosVerdes = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(false), def: false, text: "Informar veículos verdes" });
    this.VeiculosVerdes = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, text: ko.observable("Veículos Verdes"), required: ko.observable(false) });

    this.InformarVeiculosVerdes.val.subscribe(function (novoValor) {
        if (novoValor) {
            _ofertasPai.VeiculosVerdes.required(true);
            _ofertasPai.VeiculosVerdes.text("*Veículos Verdes");
        } else {
            _ofertasPai.VeiculosVerdes.required(false);
            _ofertasPai.VeiculosVerdes.text("Veículos Verdes");
            _ofertasPai.VeiculosVerdes.val("");
        }
    });
}

var NaoOfertar = function () {
    this.NaoOfertarMultiplosSituacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoOfertarMultiplos = PropertyEntity({ eventClick: naoOfertarMultiplosClick, type: types.event, text: ko.observable("Selecionar Rotas Para Não Ofertar"), idGrid: guid(), visible: ko.observable(true) });
    this.ConfirmarNaoOfertarMultiplos = PropertyEntity({ eventClick: confirmarNaoOfertarMultiplosClick, type: types.event, text: ko.observable("Confirmar Rotas Para Não Ofertar"), idGrid: guid(), visible: ko.observable(false) });
}

var DetalhesOfertaPrincipal = function () {
    this.RotaDescricao = PropertyEntity({ getType: typesKnockout.string, text: "" });
    this.Rodada = PropertyEntity({ getType: typesKnockout.string, text: "" });
    this.FranquiaKm = PropertyEntity({ getType: typesKnockout.string, text: "" });
    this.Frequencia = PropertyEntity({ getType: typesKnockout.string, text: "" });
    this.ValorCargaMes = PropertyEntity({ getType: typesKnockout.string, text: "" });
    this.Volume = PropertyEntity({ getType: typesKnockout.string, text: "" });
    this.TiposCarga = PropertyEntity({ getType: typesKnockout.string, text: "" });
    this.OfertasKmRodado = PropertyEntity({ val: ko.observableArray([]) });
    this.OfertasEquipamento = PropertyEntity({ val: ko.observableArray([]) });
    this.OfertasViagemAdicional = PropertyEntity({ val: ko.observableArray([]) });
    this.OfertasPorcentagem = PropertyEntity({ val: ko.observableArray([]) });
    this.OfertasFranquiaKm = PropertyEntity({ val: ko.observableArray([]) });
    this.OfertasFretePorPeso = PropertyEntity({ val: ko.observableArray([]) });
    this.OfertasFretePorCapacidade = PropertyEntity({ val: ko.observableArray([]) });
    this.OfertasFretePorViagem = PropertyEntity({ val: ko.observableArray([]) });
    this.OfertasViagemEntregaAjudante = PropertyEntity({ val: ko.observableArray([]) });
}


//#endregion

//#region Dúvidas

var Duvidas = function () {
    this.Duvidas = PropertyEntity({ val: ko.observable([]) });
}

var EnviarDuvida = function () {
    this.DuvidaPergunta = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.text, text: "Qual é a sua dúvida?", required: true });
    this.Enviar = PropertyEntity({ eventClick: enviarDuvidaClick, type: types.event, text: "Enviar", visible: ko.observable(true) });
}

//#endregion

//#region CRUD

var CRUDBidding = function () {
    this.Aceitar = PropertyEntity({ eventClick: aceitarClick, type: types.event, text: "Aceitar convite", visible: ko.observable(true) });
    this.Recusar = PropertyEntity({ eventClick: recusarClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Duvidas = PropertyEntity({ eventClick: duvidasClick, type: types.event, text: "Dúvidas", visible: ko.observable(true) });
}

var CRUDQuestionarios = function () {
    this.EnviarQuestionario = PropertyEntity({ eventClick: enviarQuestionarioClick, type: types.event, text: "Enviar Questionário", visible: ko.observable(true) });
    this.Duvidas = PropertyEntity({ eventClick: duvidasClick, type: types.event, text: "Dúvidas", visible: ko.observable(true) });
}

var CRUDOfertas = function () {
    this.Duvidas = PropertyEntity({ eventClick: duvidasClick, type: types.event, text: "Dúvidas", visible: ko.observable(true) });
    this.ObterTemplateOferta = PropertyEntity({ eventClick: ObterTemplateOfertaClick, type: types.event, text: "Obter Template de Oferta", visible: ko.observable(true) });
    this.ImportarOfertas = PropertyEntity({
        type: types.local,
        text: "Importar Ofertas",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        ManterArquivoServidor: false,
        UrlImportacao: "BiddingAceitamento/ImportarOfertas",
        UrlConfiguracao: "BiddingAceitamento/ConfiguracaoImportarOfertas",
        CodigoControleImportacao: EnumCodigoControleImportacao.O073_BiddingAceitacaoOferta,
        CallbackImportacao: function () {
            _gridOfertasRotas.CarregarGrid();
            PesquisarConvite();
        }
    });
}
//#endregion

//#region Aba Resultado

var Resultado = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ text: "Situação:", required: true, getType: typesKnockout.dynamic, options: _situacaoOptions, val: ko.observable("1") });
    this.Rota = PropertyEntity({ text: "Rota:", required: true, getType: typesKnockout.dynamic, options: ko.observableArray([]), val: ko.observable("1") });
    this.Resultados = PropertyEntity({ type: types.local, id: guid() });

    this.Situacao.val.subscribe(pesquisarResultados, this);
    this.Rota.val.subscribe(pesquisarResultados, this);
}

var DetalhesResultado = function () {
    this.Frequencia = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.ValorCargaMes = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.Volume = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.FranquiaKm = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
}

//#endregion

//#region Funçoes Load

function loadBiddingAceitacao() {
    _pesquisaBiddingConvite = new PesquisaBiddingConvite();
    KoBindings(_pesquisaBiddingConvite, "knockoutPesquisaBiddingConvite");

    _CRUDBidding = new CRUDBidding();
    KoBindings(_CRUDBidding, "knockoutCRUDBidding");

    _biddingAceitacao = new BiddingAceitacao();
    KoBindings(_biddingAceitacao, "knockoutConvite");

    _biddingAceitacaoChecklist = new Questoes();
    KoBindings(_biddingAceitacaoChecklist, "knockoutConviteChecklist");

    _modalRotas = new RotaModal();
    KoBindings(_modalRotas, "knockoutModalOrigensDestinos");

    _questionarioAnexo = new QuestionarioAnexo();
    KoBindings(_questionarioAnexo, "knockoutQuestionarioAnexos");

    _biddingAceitacaoOfertas = new BiddingAceitacaoOfertas();
    KoBindings(_biddingAceitacaoOfertas, "knockoutOfertas");

    _duvidas = new Duvidas();
    KoBindings(_duvidas, "knockoutDuvidas");

    _enviarDuvida = new EnviarDuvida();
    KoBindings(_enviarDuvida, "knockoutEnviarDuvida");

    _CRUDQuestao = new CRUDQuestionarios();
    KoBindings(_CRUDQuestao, "CRUDChecklistQuestionarios");

    _CRUDOfertas = new CRUDOfertas();
    KoBindings(_CRUDOfertas, "knockoutOfertasTab");

    _tabsOfertas = new TabsOfertas();
    KoBindings(_tabsOfertas, "knockoutTabsOferta");

    _tabDetalhesOfertas = new TabsDetalhesOfertas();
    KoBindings(_tabDetalhesOfertas, "knockoutTabsDetalhesOferta");

    _detalhesOfertaPrincipal = new DetalhesOfertaPrincipal();
    KoBindings(_detalhesOfertaPrincipal, "knockoutDetalhesOfertaPrincipal");

    _ofertasPai = new OfertaPai();
    KoBindings(_ofertasPai, "knockoutTabOfertaPai");

    _naoOfertar = new NaoOfertar();
    KoBindings(_naoOfertar, "knockoutNaoOfertar");

    _resultado = new Resultado();
    KoBindings(_resultado, "knockoutResultado");

    _detalhesResultado = new DetalhesResultado();
    KoBindings(_detalhesResultado, "knockoutDetalhesResultado");

    _biddingOfertaDetalhes = new BiddingOfertaDetalhes();
    KoBindings(_biddingOfertaDetalhes, "knockoutDetalhesOferta");

    LoadEtapasBidding();
    loadGridBidding();
    loadGridQuestionarios();
    loadGridRotas();
    loadGridOrigem();
    loadGridDestino();
    loadGridCargas();
    loadGridVeiculos();
    loadGridAnexos();
    loadGridAnexosTipoBidding();
    loadGridQuestionarioAnexos();
    loadGridOfertasRotas();
    loadGridResultados();
    loadAnexoQuestionario();
    RegistrarComponente();

    ObterResumosBidding();
}

function loadGridBidding() {
    const opcaoInformacoes = { descricao: "Carregar", id: guid(), evento: "onclick", metodo: CarregarClick, tamanho: "10", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoInformacoes] };
    _gridBiddingConvite = new GridViewExportacao(_pesquisaBiddingConvite.Pesquisar.idGrid, "BiddingConvite/Pesquisar", _pesquisaBiddingConvite, menuOpcoes);
    _gridBiddingConvite.CarregarGrid();
}

function loadGridQuestionarios() {
    const linhasPorPagina = 10;
    const opcaoEditar = { descricao: "Anexos", id: guid(), evento: "onclick", metodo: anexosQuestionarioClick, icone: "", visiblidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "ChecklistAnexo", visible: false },
        { data: "Descricao", title: "Pergunta", width: "100%", className: "text-align-left" }
    ];
    _gridChecklistQuestionarios = new BasicDataTable(_biddingAceitacao.Questionarios.id, header, menuOpcoes, null, null, linhasPorPagina);
    _gridChecklistQuestionarios.CarregarGrid([]);
}

function loadGridResultados() {
    const configExportacao = {
        url: "BiddingAceitamento/ExportarPesquisaResultados",
        titulo: "Resultados Bidding",
        id: "btnExportarDocumento"
    };

    _gridResultados = new GridView(_resultado.Resultados.id, "BiddingAceitamento/PesquisarResultados", _resultado, null, null, 10, null, null, null, null, null, null, configExportacao);
    _gridResultados.CarregarGrid();
}

function loadGridRotas() {
    const linhasPorPagina = 10;
    const opcaoInformacoes = { descricao: "Informações", id: guid(), evento: "onclick", metodo: informacoesRotaClick, icone: "", visiblidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoInformacoes] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", visible: false },
        { data: "Frequencia", visible: false },
        { data: "Volume", visible: false },
        { data: "ValorCargaMes", visible: false },
        { data: "QuilometragemMedia", visible: false },
        { data: "TipoCarga", visible: false },
        { data: "ModeloVeicular", visible: false },
        { data: "FlagOrigem", visible: false },
        { data: "FlagDestino", visible: false },
        { data: "CidadeOrigem", visible: false },
        { data: "ClienteOrigem", visible: false },
        { data: "EstadoOrigem", visible: false },
        { data: "RegiaoOrigem", visible: false },
        { data: "RotaOrigem", visible: false },
        { data: "PaisOrigem", visible: false },
        { data: "CEPOrigem", visible: false },
        { data: "CidadeDestino", visible: false },
        { data: "ClienteDestino", visible: false },
        { data: "EstadoDestino", visible: false },
        { data: "RegiaoDestino", visible: false },
        { data: "RotaDestino", visible: false },
        { data: "PaisDestino", visible: false },
        { data: "CEPDestino", visible: false },
        { data: "Observacao", visible: false },
        { data: "Rota", title: "Rota", width: "100%", className: "text-align-left" }
    ];
    _gridRotasBasicTable = new BasicDataTable(_biddingAceitacao.Rotas.id, header, menuOpcoes, null, null, linhasPorPagina);
    _gridRotasBasicTable.CarregarGrid([]);
}

function loadGridOfertasRotas() {
    const opcaoOfertar = { descricao: "Ofertar", id: guid(), metodo: ofertarClick, icone: "", visibilidade: visibilidadeOfertar };
    const opcaoNaoOfertar = { descricao: "Não Ofertar", id: guid(), metodo: naoOfertarClick, icone: "", visibilidade: visibilidadeOfertar };
    const opcaoVerOferta = { descricao: "Ver Oferta", id: guid(), metodo: verOfertaClick, icone: "", visibilidade: visibilidadeVerOferta };
    const opcaoEditarOferta = { descricao: "Editar Oferta", id: guid(), metodo: editarOfertaClick, icone: "", visibilidade: visibilidadeEditarOferta };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoOfertar, opcaoNaoOfertar, opcaoVerOferta, opcaoEditarOferta] };

    const configExportacao = {
        url: "BiddingAceitamento/ExportarPesquisaOfertaAvaliacaoTransportadores",
        btnText: "Exportar Excel (Dados Salvos)",
        funcaoObterParametros: function () {
            return { Codigo: _biddingAceitacaoOfertas.Codigo.val() };
        }
    }

    var editarColuna = { permite: true, callback: undefined, atualizarRow: false, functionPermite: (data) => VisibilidadeOpcaoInformacoes(data) };
    _gridOfertasRotas = new GridView(_biddingAceitacaoOfertas.Rotas.id, "BiddingAceitamento/PesquisarAvaliacaoOfertasTransportadores", _biddingAceitacaoOfertas, menuOpcoes, null, 15, null, null, null, null, null, editarColuna, configExportacao, null, null, null, callbackColumnDefaultGridOfertas);
    habilitarEditarGridAOfertas();
    _gridOfertasRotas.CarregarGrid();
}

function callbackColumnDefaultGridOfertas(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name.includes("ColunaDinamicaTransportador")) {
        cabecalho.editableCell.type = EnumTipoColunaEditavelGrid.bool;
        return '<input type="checkbox" data-bind="checked" hidden/><span title="' + valorColuna + '">' + valorColuna + '</span>';
    }
}

function habilitarEditarGridAOfertas() {
    var editarColuna = { permite: true, callback: callbackEditarColunaOfertas, atualizarRow: false, functionPermite: (data) => VisibilidadeOpcaoInformacoes(data) };
    _gridOfertasRotas.SetarEditarColunas(editarColuna);
}

function callbackEditarColunaOfertas(dataRow, row, head) {
    if (_naoOfertar.NaoOfertarMultiplosSituacao)
        MarcarOfertasNaoOfertar(dataRow)
}

function VisibilidadeOpcaoInformacoes(data) {
    return data.Codigo != 0;
}

function visibilidadeOfertar(e) {
    _kmMedio = e.QuilometragemMedia;
    if (e.SituacaoCodigo == EnumBiddingOfertaSituacao.Aguardando && _biddingAceitacao.Status.val() == EnumStatusBidding.Ofertas) {
        return true;
    }
    return false;
}

function visibilidadeVerOferta(e) {
    if (e.SituacaoCodigo != EnumBiddingOfertaSituacao.Aguardando) {
        return true;
    }
    return false;
}

function visibilidadeEditarOferta(e) {
    if (e.SituacaoCodigo != EnumBiddingOfertaSituacao.Aguardando && e.NaoOfertar == false) {
        return true;
    }
    return false;
}

function loadGridOrigem() {
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Origem", visible: true }
    ];
    _gridOrigem = new BasicDataTable(_modalRotas.Origens.id, header, null, null, null, 2);
    _gridOrigem.CarregarGrid([]);
}

function loadGridDestino() {
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Destino", visible: true }
    ];
    _gridDestino = new BasicDataTable(_modalRotas.Destinos.id, header, null, null, null, 2);
    _gridDestino.CarregarGrid([]);
}

function loadGridCargas() {
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Destino", visible: true }
    ];
    _gridCarga = new BasicDataTable(_modalRotas.TiposCarga.id, header, null, null, null, 2);
    _gridCarga.CarregarGrid([]);
}

function loadGridVeiculos() {
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Destino", visible: true }
    ];
    _gridVeiculo = new BasicDataTable(_modalRotas.ModelosVeiculares.id, header, null, null, null, 2);
    _gridVeiculo.CarregarGrid([]);
}

function loadGridAnexos() {
    const linhasPorPaginas = 10;
    const opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_biddingAceitacao.Anexos.id, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

function loadGridAnexosTipoBidding() {
    const linhasPorPaginas = 10;
    const opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoTipoBiddingClick, icone: "", visibilidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoTipoBidding = new BasicDataTable(_biddingAceitacao.AnexosTipoBidding.id, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoTipoBidding.CarregarGrid([]);
}

function loadGridQuestionarioAnexos() {
    const linhasPorPaginas = 10;
    const opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadQuestionarioAnexoClick, icone: "", visibilidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoQuestionario = new BasicDataTable(_questionarioAnexo.Anexos.id, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoQuestionario.CarregarGrid([]);
}

function loadGridsDetalhesOferta(naoIncluirImpostoICMS) {
    let header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "80%", className: "text-align-left" },
        { data: "ValorMes", title: "Valor Mês", width: "20%", className: "text-align-center" },
        { data: "VeiculosVerdes", title: "Veículos Verdes", width: "20%", className: "text-align-center" }
    ];
    _gridEquipamento = new BasicDataTable(_biddingOfertaDetalhes.Equipamento.id, header, null, null, null, 10);
    _gridEquipamento.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "60%", className: "text-align-left" },
        { data: "ValorFixo", title: "Valor Fixo", width: "20%", className: "text-align-center" },
        { data: "ValorKm", title: "Valor por Km", width: "20%", className: "text-align-center" },
        { data: "CustoEstimado", title: "Custo Estimado", width: "20%", className: "text-align-center" },
        { data: "VeiculosVerdes", title: "Veículos Verdes", width: "20%", className: "text-align-center" }
    ];
    _gridFrotaFixaKm = new BasicDataTable(_biddingOfertaDetalhes.FrotaFixaKm.id, header, null, null, null, 10);
    _gridFrotaFixaKm.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "80%", className: "text-align-left" },
        { data: "Porcentagem", title: "Porcentagem Sobre a Nota", width: "20%", className: "text-align-center" },
        { data: "CustoEstimado", title: "Custo Estimado", width: "20%", className: "text-align-center" },
        { data: "VeiculosVerdes", title: "Veículos Verdes", width: "20%", className: "text-align-center" }
    ];
    _gridPorcentagem = new BasicDataTable(_biddingOfertaDetalhes.PorcentagemSobreNota.id, header, null, null, null, 10);
    _gridPorcentagem.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "60%", className: "text-align-left" },
        { data: "ValorViagem", title: "Valor Viagem", width: "20%", className: "text-align-center" },
        { data: "Adicional", title: "Adicional por Entrega", width: "20%", className: "text-align-center" },
        { data: "CustoEstimado", title: "Custo Estimado", width: "20%", className: "text-align-center" },
        { data: "VeiculosVerdes", title: "Veículos Verdes", width: "20%", className: "text-align-center" }
    ];
    _gridViagemAdicional = new BasicDataTable(_biddingOfertaDetalhes.ViagemAdicional.id, header, null, null, null, 10);
    _gridViagemAdicional.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "40%", className: "text-align-left" },
        { data: "ValorFixo", title: "Valor Fixo", width: "20%", className: "text-align-center" },
        { data: "ValorFranquia", title: "Valor Franquia", width: "20%", className: "text-align-center" },
        { data: "Quilometragem", title: "Quilometragem", width: "20%", className: "text-align-center" },
        { data: "CustoEstimado", title: "Custo Estimado", width: "20%", className: "text-align-center" },
        { data: "VeiculosVerdes", title: "Veículos Verdes", width: "20%", className: "text-align-center" }
    ];
    _gridFrotaFixaFranquia = new BasicDataTable(_biddingOfertaDetalhes.FrotaFixaFranquia.id, header, null, null, null, 10);
    _gridFrotaFixaFranquia.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "40%", className: "text-align-left" },
        { data: "ICMS", title: "ICMS (%)", width: "20%", className: "text-align-center" },
        { data: "FreteTonelada", title: "Frete TON (R$)", width: "20%", className: "text-align-center" },
        { data: "PedagioEixo", title: "Pedágio p/ eixo (R$)", width: "20%", className: "text-align-center" },
        { data: "FreteComICMS", title: "Frete com ICMS", width: "20%", className: "text-align-center" },
        { data: "PedagioComICMS", title: "Pedágio com ICMS", width: "20%", className: "text-align-center" },
        { data: "NumeroEixos", title: "Número de Eixos do veículo", width: "20%", className: "text-align-center" },
        { data: "Capacidade", title: "Capacidade do veículo", width: "20%", className: "text-align-center" },
        { data: "TotalBruto", title: "Total bruto", width: "20%", className: "text-align-center" },
        { data: "TotalLiquido", title: "Total líquido", width: "20%", className: "text-align-center" },
        { data: "VeiculosVerdes", title: "Veículos Verdes", width: "20%", className: "text-align-center" }
    ];
    _gridFretePorPeso = new BasicDataTable(_biddingOfertaDetalhes.FretePorPeso.id, header, null, null, null, 10);
    _gridFretePorPeso.CarregarGrid([]);


    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "40%", className: "text-align-left" },
        { data: "ICMS", title: "ICMS (%)", width: "20%", className: "text-align-center" },
        { data: "FreteTonelada", title: "Frete TON (R$)", width: "20%", className: "text-align-center" },
        { data: "PedagioEixo", title: "Pedágio p/ eixo (R$)", width: "20%", className: "text-align-center" },
        { data: "FreteComICMS", title: "Frete com ICMS", width: "20%", className: "text-align-center" },
        { data: "PedagioComICMS", title: "Pedágio com ICMS", width: "20%", className: "text-align-center" },
        { data: "NumeroEixos", title: "Número de Eixos do veículo", width: "20%", className: "text-align-center" },
        { data: "Capacidade", title: "Capacidade do veículo", width: "20%", className: "text-align-center" },
        { data: "TotalBruto", title: "Total bruto", width: "20%", className: "text-align-center" },
        { data: "TotalLiquido", title: "Total líquido", width: "20%", className: "text-align-center" },
        { data: "VeiculosVerdes", title: "Veículos Verdes", width: "20%", className: "text-align-center" }
    ];
    _gridFretePorCapacidade = new BasicDataTable(_biddingOfertaDetalhes.FretePorCapacidade.id, header, null, null, null, 10);
    _gridFretePorCapacidade.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "40%", className: "text-align-left" },
        { data: "ICMS", title: "ICMS (%)", width: "20%", className: "text-align-center" },
        { data: "FreteTonelada", title: "Frete (27 tons) - sem ICMS", width: "20%", className: "text-align-center" },
        { data: "PedagioEixo", title: "Pedágio (quantidade de eixos do modelo)", width: "20%", className: "text-align-center" },
        { data: "NumeroEixos", title: "Número de Eixos do veículo", width: "20%", className: "text-align-center" },
        { data: "Capacidade", title: "Capacidade do veículo", width: "20%", className: "text-align-center" },
        { data: "TotalBruto", title: "Total bruto", width: "20%", className: "text-align-center" },
        { data: "TotalLiquido", title: "Total líquido", width: "20%", className: "text-align-center" },
        { data: "VeiculosVerdes", title: "Veículos Verdes", width: "20%", className: "text-align-center" }
    ];
    _gridFretePorViagem = new BasicDataTable(_biddingOfertaDetalhes.FretePorViagem.id, header, null, null, null, 10);
    _gridFretePorViagem.CarregarGrid([]);

    header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "40%", className: "text-align-left" },
        { data: "ICMS", title: "ICMS (%)", width: "20%", className: "text-align-center" },
        { data: "FreteTonelada", title: "Frete viagem com ICMS (R$)", width: "20%", className: "text-align-center" },
        { data: "PedagioEixo", title: "Pedágio para todos os eixos c/ICMS (R$)", width: "20%", className: "text-align-center" },
        { data: "Ajudante", title: "Ajudante (R$)", width: "20%", className: "text-align-center" },
        { data: "AdicionalPorEntrega", title: "Adicional por entrega (R$)", width: "20%", className: "text-align-center" },
        { data: "ViagemComPedagio", title: "Frete fechado + pedágio " + (naoIncluirImpostoICMS ? "sem ICMS" : " (com ICMS)"), width: "20%", className: "text-align-center" },
        { data: "AdicionalAjudanteComICMS", title: "Frete fechado + pedágio com ajudante " + (naoIncluirImpostoICMS ? "sem ICMS" : " (com ICMS)"), width: "20%", className: "text-align-center" },
        { data: "VeiculosVerdes", title: "Veículos Verdes", width: "20%", className: "text-align-center" }
    ];
    _gridViagemEntregaAjudante = new BasicDataTable(_biddingOfertaDetalhes.ViagemEntregaAjudante.id, header, null, null, null, 10);
    _gridViagemEntregaAjudante.CarregarGrid([]);
}

//#endregion

function RegistrarComponente() {
    if (!ko.components.isRegistered('duvida')) {
        ko.components.register('duvida', {
            template: {
                element: 'duvida-template'
            }
        });
    }
}

//#region Funções click

function PesquisarConvite() {
    _gridBiddingConvite.CarregarGrid();
    ObterResumosBidding();
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function CarregarClick(registroSelecionado) {
    $("#LancePorcentagemNota").removeClass("active");
    $("#LanceViagemAdicional").removeClass("active");
    $("#LanceFrotaFixaKmRodado").removeClass("active");
    $("#LancePorEquipamento").removeClass("active");
    $("#LanceFrotaFixaFranquia").removeClass("active");
    $("#LancePorPeso").removeClass("active");
    $("#LancePorCapacidade").removeClass("active");
    $("#LancePorFreteViagem").removeClass("active");
    $("#LanceViagemEntregaAjudante").removeClass("active");

    _biddingAceitacaoChecklist.Questoes.removeAll();
    LimparGridseCampos();
    ResetarNaoOfertar();
    _biddingAceitacao.Codigo.val(registroSelecionado.Codigo);
    BuscarPorCodigo(_biddingAceitacao, "BiddingAceitamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                SetarEtapasRequisicao(retorno.Data.Status, retorno.Data.DadosBiddingConvite.RemoverEtapaAceite, retorno.Data.Checklist.TipoPreenchimentoChecklist);
                _pesquisaBiddingConvite.ExibirFiltros.visibleFade(false);
                _biddingAceitacao.Descritivo.val(retorno.Data.DadosBiddingConvite.DescritivoTransportador);
                _biddingAceitacaoChecklist.Descritivo.val(retorno.Data.DadosBiddingConvite.DescritivoConvite);
                _biddingAceitacao.Anexos.val(retorno.Data.Anexos.slice());
                _gridAnexo.CarregarGrid(retorno.Data.Anexos.slice());
                _biddingAceitacao.AnexosTipoBidding.val(retorno.Data.AnexosTipoBidding.slice());
                _gridAnexoTipoBidding.CarregarGrid(retorno.Data.AnexosTipoBidding.slice());
                _biddingAceitacao.Questionarios.val(retorno.Data.Questionarios.slice());
                _gridChecklistQuestionarios.CarregarGrid(retorno.Data.Questionarios.slice());
                _resultado.Codigo.val(retorno.Data.Oferta.Codigo);
                todosResultados = retorno.Data.Resultado;
                preencherTabOfertaPai(retorno.Data.OfertasRotas, retorno.Data.NaoPossuiPedagioFluxoOferta, retorno.Data.NaoIncluirImpostoValorTotalOferta);
                preencherOpcoesRotas(retorno.Data.OpcoesRota);
                _biddingAceitacao.Rotas.val(retorno.Data.Rotas.slice());
                rotas = retorno.Data.Rotas.slice();
                _gridRotasBasicTable.CarregarGrid(rotas);
                VerificarInformacoesBiddingConviteVisiveis(retorno)
                _biddingAceitacao.TempoRestante.text("Prazo Limite: " + (retorno.Data.DadosBiddingConvite.PrazoAceiteConvite ? retorno.Data.DadosBiddingConvite.PrazoAceiteConvite : (_CONFIGURACAO_TMS.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding ? "Não informado" : "")));
                _biddingAceitacaoChecklist.TempoRestante.text("Prazo Limite: " + (retorno.Data.Checklist.Prazo ? retorno.Data.Checklist.Prazo : (_CONFIGURACAO_TMS.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding ? "Não informado" : "")));
                _biddingAceitacaoOfertas.TempoRestante.text("Prazo Limite: " + (retorno.Data.Oferta.DataPrazoOferta ? retorno.Data.Oferta.DataPrazoOferta : (_CONFIGURACAO_TMS.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding ? "Não informado" : "")));
                _biddingAceitacaoOfertas.Codigo.val(registroSelecionado.Codigo);
                _duvidas.Duvidas.val(retorno.Data.Duvidas);
                pesquisarResultados();
                verificarTiposOfertas(retorno.Data.Oferta);
                VerificarStatus(retorno.Data.Status);
                VerificarStatusConviteConvidado(retorno.Data.DadosBiddingConvite.StatusConviteConvidado);
                ControlarVisibilidadeVeiculosVerdes(retorno.Data.InformarVeiculosVerdes);
                const listaAnexos = [];
                for (let i = 0; i < retorno.Data.Questionarios.length; i++) {
                    const questao = retorno.Data.Questionarios[i];
                    const knoutQuestao = new BiddingAceitacaoChecklist(questao);
                    knoutQuestao.Pergunta.val(retorno.Data.Questionarios[i].Descricao);
                    _gridAnexoQuestionario.CarregarGrid(retorno.Data.Questionarios[i].ChecklistAnexo);
                    if (retorno.Data.Questionarios[i].Respostas.length > 0) {
                        knoutQuestao.EnviarAnexo.visible(false);
                        esconderBotoesChecklist();
                        knoutQuestao.Resposta.val(retorno.Data.Questionarios[i].Respostas[i].Resposta);
                        knoutQuestao.Observacao.val(retorno.Data.Questionarios[i].Respostas[i].Observacao);
                        for (let x = 0; x < retorno.Data.Questionarios[i].Respostas[i].AnexosResposta.length; x++) {
                            listaAnexos.push(retorno.Data.Questionarios[i].Respostas[i].AnexosResposta[x]);
                        }
                    } else {
                        knoutQuestao.EnviarAnexo.visible(true);
                    }
                    _listaAnexoChecklist.Anexos.val(listaAnexos.slice());
                    _biddingAceitacaoChecklist.Questoes.push(knoutQuestao);
                }

                _gridOfertasRotas.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function VerificarInformacoesBiddingConviteVisiveis(retorno) {

    if (retorno.Data.DadosBiddingConvite.TipoFrete != "") {
        _biddingAceitacao.TipoFrete.text("Tipo de frete: " + retorno.Data.DadosBiddingConvite.TipoFrete);
        _biddingAceitacao.TipoFrete.visible(true);
    }
    if (retorno.Data.DadosBiddingConvite.DataInicioVigencia != "") {
        _biddingAceitacao.DataInicioVigencia.text("Data início vigência: " + retorno.Data.DadosBiddingConvite.DataInicioVigencia);
        _biddingAceitacao.DataInicioVigencia.visible(true);
    }
}

function aceitarClick() {
    if (!_biddingAceitacao.AceitoTermo.val()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Você precisa aceitar os termos de participação.");
        return;
    }

    if (_biddingAceitacao.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Você precisa selecionar um bididng.");
        return;
    }

    if (_biddingAceitacao.Codigo.val() > 0) {
        executarReST("BiddingAceitamento/AceitarConvite", { Codigo: _biddingAceitacao.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Você aceitou o convite.");
                    PesquisarConvite();
                    CarregarClick({ Codigo: _biddingAceitacao.Codigo.val() });
                    _CRUDBidding.Aceitar.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                PesquisarConvite();
                LimparGridseCampos();
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function enviarOfertasClick(e) {

    const oferta = e.Oferta()[0];
    const arrayOfertas = new Array();
    for (let i = 0; i < oferta?.KnoutsOfertaEquipamento.val().length; i++) {
        const ofertaEquipamento = oferta.KnoutsOfertaEquipamento.val()[i];
        if (ofertaEquipamento.OfertarModeloVeiculo.val()) {
            const valorCampos = ({
                ValorFixo: ofertaEquipamento.ValorFixoMensal.val(),
                ModeloVeicular: ofertaEquipamento.CodigoVeiculo.val,
                Codigo: ofertaEquipamento.Codigo.val
            });
            arrayOfertas.push({ Tipo: EnumTipoLanceBidding.LancePorEquipamento, Oferta: valorCampos, NaoOfertar: false });
        }
    }

    for (let i = 0; i < oferta?.KnoutsOfertaFrotaFixaFranquia.val().length; i++) {
        const ofertaEquipamento = oferta.KnoutsOfertaFrotaFixaFranquia.val()[i];
        if (ofertaEquipamento.OfertarModeloVeiculo.val()) {
            const valorCampos = ({
                ValorFixo: ofertaEquipamento.ValorFixo.val(),
                ValorPorFranquia: ofertaEquipamento.Franquia.val(),
                Quilometragem: ofertaEquipamento.Quilometragem.val(),
                ModeloVeicular: ofertaEquipamento.CodigoVeiculo.val,
                Codigo: ofertaEquipamento.Codigo.val
            });
            arrayOfertas.push({ Tipo: EnumTipoLanceBidding.LanceFrotaFixaFranquia, Oferta: valorCampos, NaoOfertar: false });
        }
    }

    for (let i = 0; i < oferta?.KnoutsOfertaFrotaFixaKmRodado.val().length; i++) {
        const ofertaEquipamento = oferta.KnoutsOfertaFrotaFixaKmRodado.val()[i];
        if (ofertaEquipamento.OfertarModeloVeiculo.val()) {
            const valorCampos = ({
                ValorFixoMensal: ofertaEquipamento.ValorFixoMensal.val(),
                ValorKmRodado: ofertaEquipamento.ValorKmRodado.val(),
                ModeloVeicular: ofertaEquipamento.CodigoVeiculo.val,
                Codigo: ofertaEquipamento.Codigo.val
            });
            arrayOfertas.push({ Tipo: EnumTipoLanceBidding.LanceFrotaFixaKmRodado, Oferta: valorCampos, NaoOfertar: false });
        }
    }

    for (let i = 0; i < oferta?.KnoutsOfertaPorcentagem.val().length; i++) {
        const ofertaEquipamento = oferta.KnoutsOfertaPorcentagem.val()[i];
        if (ofertaEquipamento.OfertarModeloVeiculo.val()) {
            const valorCampos = ({
                PorcentagemNota: ofertaEquipamento.PorcentagemNota.val(),
                ModeloVeicular: ofertaEquipamento.CodigoVeiculo.val,
                Codigo: ofertaEquipamento.Codigo.val
            });
            arrayOfertas.push({ Tipo: EnumTipoLanceBidding.LancePorcentagemNota, Oferta: valorCampos, NaoOfertar: false });
        }
    }

    for (let i = 0; i < oferta?.KnoutsOfertaViagemAdicional.val().length; i++) {
        const ofertaEquipamento = oferta.KnoutsOfertaViagemAdicional.val()[i];
        if (ofertaEquipamento.OfertarModeloVeiculo.val()) {
            const valorCampos = ({
                ValorViagem: ofertaEquipamento.ValorViagem.val(),
                ValorEntrega: ofertaEquipamento.ValorAdicionalEntrega.val(),
                ModeloVeicular: ofertaEquipamento.CodigoVeiculo.val,
                Codigo: ofertaEquipamento.Codigo.val
            });
            arrayOfertas.push({ Tipo: EnumTipoLanceBidding.LanceViagemAdicional, Oferta: valorCampos, NaoOfertar: false });
        }
    }

    for (let i = 0; i < oferta?.KnoutsOfertaFretePorPeso.val().length; i++) {
        const ofertaEquipamento = oferta.KnoutsOfertaFretePorPeso.val()[i];
        if (ofertaEquipamento.OfertarModeloVeiculo.val()) {
            const valorCampos = ({
                ICMS: ofertaEquipamento.ICMS.val(),
                ReplicarICMSDesteModeloVeicular: ofertaEquipamento.ReplicarICMSDesteModeloVeicular.val(),
                FreteTonelada: ofertaEquipamento.FreteTonelada.val(),
                PedagioEixo: ofertaEquipamento.PedagioEixo.val(),
                ModeloVeicular: ofertaEquipamento.CodigoVeiculo.val,
                Codigo: ofertaEquipamento.Codigo.val
            });
            arrayOfertas.push({ Tipo: EnumTipoLanceBidding.LancePorPeso, Oferta: valorCampos, NaoOfertar: false });
        }
    }

    for (let i = 0; i < oferta?.KnoutsOfertaFretePorCapacidade.val().length; i++) {
        const ofertaEquipamento = oferta.KnoutsOfertaFretePorCapacidade.val()[i];
        if (ofertaEquipamento.OfertarModeloVeiculo.val()) {
            const valorCampos = ({
                ICMS: ofertaEquipamento.ICMS.val(),
                ReplicarICMSDesteModeloVeicular: ofertaEquipamento.ReplicarICMSDesteModeloVeicular.val(),
                FreteTonelada: ofertaEquipamento.FreteTonelada.val(),
                PedagioEixo: ofertaEquipamento.PedagioEixo.val(),
                ModeloVeicular: ofertaEquipamento.CodigoVeiculo.val,
                Codigo: ofertaEquipamento.Codigo.val
            });
            arrayOfertas.push({ Tipo: EnumTipoLanceBidding.LancePorCapacidade, Oferta: valorCampos, NaoOfertar: false });
        }
    }
    for (let i = 0; i < oferta?.KnoutsOfertaFretePorViagem.val().length; i++) {
        const ofertaEquipamento = oferta.KnoutsOfertaFretePorViagem.val()[i];
        if (ofertaEquipamento.OfertarModeloVeiculo.val()) {
            const valorCampos = ({
                ICMS: ofertaEquipamento.ICMS.val(),
                ReplicarICMSDesteModeloVeicular: ofertaEquipamento.ReplicarICMSDesteModeloVeicular.val(),
                FreteTonelada: ofertaEquipamento.FreteTonelada.val(),
                PedagioEixo: ofertaEquipamento.PedagioEixo.val(),
                ModeloVeicular: ofertaEquipamento.CodigoVeiculo.val,
                Codigo: ofertaEquipamento.Codigo.val
            });
            arrayOfertas.push({ Tipo: EnumTipoLanceBidding.LancePorFreteViagem, Oferta: valorCampos, NaoOfertar: false });
        }
    }

    for (let i = 0; i < oferta?.KnoutsOfertaViagemEntregaAjudante.val().length; i++) {
        const ofertaEquipamento = oferta.KnoutsOfertaViagemEntregaAjudante.val()[i];
        if (ofertaEquipamento.OfertarModeloVeiculo.val()) {
            const valorCampos = ({
                ICMS: ofertaEquipamento.ICMS.val(),
                ReplicarICMSDesteModeloVeicular: ofertaEquipamento.ReplicarICMSDesteModeloVeicular.val(),
                FreteTonelada: ofertaEquipamento.FreteTonelada.val(),
                PedagioEixo: ofertaEquipamento.PedagioEixo.val(),
                Ajudante: ofertaEquipamento.Ajudante.val(),
                AdicionalPorEntrega: ofertaEquipamento.AdicionalPorEntrega.val(),
                ModeloVeicular: ofertaEquipamento.CodigoVeiculo.val,
                Codigo: ofertaEquipamento.Codigo.val
            });
            arrayOfertas.push({ Tipo: EnumTipoLanceBidding.LancePorViagemEntregaAjudante, Oferta: valorCampos, NaoOfertar: false });
        }
    }

    if (arrayOfertas.length <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Você precisa selecionar pelo menos um modelo veicular.");
        return;
    }

    enviarOfertas(arrayOfertas, oferta.Codigo.val());
}

function enviarOfertas(arrayOfertas, codigoOfertaRota) {
    executarReST("BiddingAceitamento/EnviarOfertas", { CodigoOfertaRota: codigoOfertaRota, Ofertas: JSON.stringify(arrayOfertas), InformarVeiculosVerdes: _ofertasPai.InformarVeiculosVerdes.val(), VeiculosVerdes: _ofertasPai.VeiculosVerdes.val(), }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Sua oferta para essa rota foi enviada.");
                _gridOfertasRotas.CarregarGrid();

                Global.fecharModal("divModalOferta");
                PesquisarConvite();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function editarOfertaClick(rotaSelecionada) {
    ofertarClick(rotaSelecionada)
    _ofertasPai.EnviarOferta.visible(true);

    executarReST("BiddingAceitamento/BuscarOferta", { Codigo: rotaSelecionada.Codigo, CodigoBiddingConvite: _biddingAceitacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherEditarDetalhesOferta(retorno.Data, rotaSelecionada);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function PreencherEditarDetalhesOferta(oferta, rotaSelecionada) {
    const knoutOferta = _ofertasPai.Ofertas().filter(oferta => oferta.CodigoRota.val() == rotaSelecionada.Codigo)[0];

    for (let i = 0; i < oferta?.Equipamento.length; i++) {
        const ofertaEquipamento = oferta.Equipamento[i];
        if (ofertaEquipamento) {
            knoutOferta.KnoutsOfertaEquipamento.val()[0].Codigo.val = ofertaEquipamento.Codigo;
            knoutOferta.KnoutsOfertaEquipamento.val()[0].OfertarModeloVeiculo.val(true);
            knoutOferta.KnoutsOfertaEquipamento.val()[0].ValorFixoMensal.val(ofertaEquipamento.ValorMes);
            knoutOferta.KnoutsOfertaEquipamento.val()[0].CodigoVeiculo.val = ofertaEquipamento.CodigoModeloVeicular;
        }
    }

    for (let i = 0; i < oferta?.FrotaFixaFranquia.length; i++) {
        const ofertaFrotaFixaFranquia = oferta.FrotaFixaFranquia[i];
        if (ofertaFrotaFixaFranquia) {
            knoutOferta.KnoutsOfertaFrotaFixaFranquia.val()[0].Codigo.val = ofertaFrotaFixaFranquia.Codigo;
            knoutOferta.KnoutsOfertaFrotaFixaFranquia.val()[0].OfertarModeloVeiculo.val(true);
            knoutOferta.KnoutsOfertaFrotaFixaFranquia.val()[0].ValorFixo.val(ofertaFrotaFixaFranquia.ValorFixo);
            knoutOferta.KnoutsOfertaFrotaFixaFranquia.val()[0].Franquia.val(ofertaFrotaFixaFranquia.ValorFranquia);
            knoutOferta.KnoutsOfertaFrotaFixaFranquia.val()[0].Quilometragem.val(ofertaFrotaFixaFranquia.Quilometragem);
            knoutOferta.KnoutsOfertaFrotaFixaFranquia.val()[0].CodigoVeiculo.val = ofertaFrotaFixaFranquia.CodigoModeloVeicular;
        }
    }

    for (let i = 0; i < oferta?.FrotaFixaKm.length; i++) {
        const ofertaFrotaFixaKm = oferta.FrotaFixaKm[i];
        if (ofertaFrotaFixaKm) {
            knoutOferta.KnoutsOfertaFrotaFixaKmRodado.val()[0].Codigo.val = ofertaFrotaFixaKm.Codigo;
            knoutOferta.KnoutsOfertaFrotaFixaKmRodado.val()[0].OfertarModeloVeiculo.val(true);
            knoutOferta.KnoutsOfertaFrotaFixaKmRodado.val()[0].ValorFixoMensal.val(ofertaFrotaFixaKm.ValorFixo);
            knoutOferta.KnoutsOfertaFrotaFixaKmRodado.val()[0].ValorKmRodado.val(ofertaFrotaFixaKm.ValorKm);
            knoutOferta.KnoutsOfertaFrotaFixaKmRodado.val()[0].CodigoVeiculo.val = ofertaFrotaFixaKm.CodigoModeloVeicular;
        }
    }

    for (let i = 0; i < oferta?.PorcentagemSobreNota.length; i++) {
        const ofertaPorcentagemSobreNota = oferta.PorcentagemSobreNota[i];
        if (ofertaPorcentagemSobreNota) {
            knoutOferta.KnoutsOfertaPorcentagem.val()[0].Codigo.val = ofertaPorcentagemSobreNota.Codigo;
            knoutOferta.KnoutsOfertaPorcentagem.val()[0].OfertarModeloVeiculo.val(true);
            knoutOferta.KnoutsOfertaPorcentagem.val()[0].PorcentagemNota.val(ofertaPorcentagemSobreNota.Porcentagem);
            knoutOferta.KnoutsOfertaPorcentagem.val()[0].CodigoVeiculo.val = ofertaPorcentagemSobreNota.CodigoModeloVeicular;
        }
    }

    for (let i = 0; i < oferta?.ViagemAdicional.length; i++) {
        const ofertaViagemAdicional = oferta.ViagemAdicional[i];
        if (ofertaViagemAdicional) {
            knoutOferta.KnoutsOfertaViagemAdicional.val()[0].Codigo.val = ofertaViagemAdicional.Codigo;
            knoutOferta.KnoutsOfertaViagemAdicional.val()[0].OfertarModeloVeiculo.val(true);
            knoutOferta.KnoutsOfertaViagemAdicional.val()[0].ValorViagem.val(ofertaViagemAdicional.ValorViagem);
            knoutOferta.KnoutsOfertaViagemAdicional.val()[0].ValorAdicionalEntrega.val(ofertaViagemAdicional.Adicional);
            knoutOferta.KnoutsOfertaViagemAdicional.val()[0].CodigoVeiculo.val = ofertaViagemAdicional.CodigoModeloVeicular;
        }
    }

    for (let i = 0; i < oferta?.FretePorPeso.length; i++) {
        const ofertaFretePorPeso = oferta.FretePorPeso[i];
        if (ofertaFretePorPeso) {
            knoutOferta.KnoutsOfertaFretePorPeso.val()[0].Codigo.val = ofertaFretePorPeso.Codigo;
            knoutOferta.KnoutsOfertaFretePorPeso.val()[0].OfertarModeloVeiculo.val(true);
            knoutOferta.KnoutsOfertaFretePorPeso.val()[0].ICMS.val(ofertaFretePorPeso.ICMS);
            knoutOferta.KnoutsOfertaFretePorPeso.val()[0].FreteTonelada.val(ofertaFretePorPeso.FreteTonelada);
            knoutOferta.KnoutsOfertaFretePorPeso.val()[0].PedagioEixo.val(ofertaFretePorPeso.PedagioEixo);
            knoutOferta.KnoutsOfertaFretePorPeso.val()[0].CodigoVeiculo.val = ofertaFretePorPeso.CodigoModeloVeicular;
        }
    }

    for (let i = 0; i < oferta?.FretePorCapacidade.length; i++) {
        const ofertaFretePorCapacidade = oferta.FretePorCapacidade[i];
        if (ofertaFretePorCapacidade) {
            knoutOferta.KnoutsOfertaFretePorCapacidade.val()[0].Codigo.val = ofertaFretePorCapacidade.Codigo;
            knoutOferta.KnoutsOfertaFretePorCapacidade.val()[0].OfertarModeloVeiculo.val(true);
            knoutOferta.KnoutsOfertaFretePorCapacidade.val()[0].ICMS.val(ofertaFretePorCapacidade.ICMS);
            knoutOferta.KnoutsOfertaFretePorCapacidade.val()[0].FreteTonelada.val(ofertaFretePorCapacidade.FreteTonelada);
            knoutOferta.KnoutsOfertaFretePorCapacidade.val()[0].PedagioEixo.val(ofertaFretePorCapacidade.PedagioEixo);
            knoutOferta.KnoutsOfertaFretePorCapacidade.val()[0].CodigoVeiculo.val = ofertaFretePorCapacidade.CodigoModeloVeicular;
        }
    }
    for (let i = 0; i < oferta?.FretePorViagem.length; i++) {
        const ofertaFretePorViagem = oferta.FretePorViagem[i];
        if (ofertaFretePorViagem) {
            knoutOferta.KnoutsOfertaFretePorViagem.val()[0].Codigo.val = ofertaFretePorViagem.Codigo;
            knoutOferta.KnoutsOfertaFretePorViagem.val()[0].OfertarModeloVeiculo.val(true);
            knoutOferta.KnoutsOfertaFretePorViagem.val()[0].ICMS.val(ofertaFretePorViagem.ICMS);
            knoutOferta.KnoutsOfertaFretePorViagem.val()[0].FreteTonelada.val(ofertaFretePorViagem.FreteTonelada);
            knoutOferta.KnoutsOfertaFretePorViagem.val()[0].PedagioEixo.val(ofertaFretePorViagem.PedagioEixo);
            knoutOferta.KnoutsOfertaFretePorViagem.val()[0].CodigoVeiculo.val = ofertaFretePorViagem.CodigoModeloVeicular;
        }
    }

    for (let i = 0; i < oferta?.ViagemEntregaAjudante.length; i++) {
        const ofertaViagemEntregaAjudante = oferta.ViagemEntregaAjudante[i];
        if (ofertaViagemEntregaAjudante) {
            knoutOferta.KnoutsOfertaViagemEntregaAjudante.val()[0].Codigo.val = ofertaViagemEntregaAjudante.Codigo;
            knoutOferta.KnoutsOfertaViagemEntregaAjudante.val()[0].OfertarModeloVeiculo.val(true);
            knoutOferta.KnoutsOfertaViagemEntregaAjudante.val()[0].ICMS.val(ofertaViagemEntregaAjudante.ICMS);
            knoutOferta.KnoutsOfertaViagemEntregaAjudante.val()[0].FreteTonelada.val(ofertaViagemEntregaAjudante.FreteTonelada);
            knoutOferta.KnoutsOfertaViagemEntregaAjudante.val()[0].PedagioEixo.val(ofertaViagemEntregaAjudante.PedagioEixo);
            knoutOferta.KnoutsOfertaViagemEntregaAjudante.val()[0].Ajudante.val(ofertaViagemEntregaAjudante.Ajudante);
            knoutOferta.KnoutsOfertaViagemEntregaAjudante.val()[0].AdicionalPorEntrega.val(ofertaViagemEntregaAjudante.AdicionalPorEntrega);
            knoutOferta.KnoutsOfertaViagemEntregaAjudante.val()[0].CodigoVeiculo.val = ofertaViagemEntregaAjudante.CodigoModeloVeicular;
        }
    }
}

function recusarClick() {
    if (_biddingAceitacao.Codigo.val() > 0) {
        exibirConfirmacao("Confirmação", "Deseja rejeitar o convite?", function () {
            executarReST("BiddingAceitamento/RejeitarConvite", { Codigo: _biddingAceitacao.Codigo.val() }, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Você rejeitou o convite.");
                        LimparGridseCampos();
                        PesquisarConvite();
                        esconderBotoes();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, "Nenhum Bidding Carregado", "Você precisa selecionar um bidding!");
    }
}

function duvidasClick() {
    if (_biddingAceitacao.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Você precisa selecionar um bididng.");
        return;
    }

    Global.abrirModal('divDuvidas');

    $("#divDuvidas").on('shown.bs.modal', function () {
        scrollDuvidas();
    });

    $("#divDuvidas").one('hidden.bs.modal', function () {
        LimparCampos(_enviarDuvida);
    });
}

function ObterTemplateOfertaClick() {
    const dados = { Codigo: _biddingAceitacao.Codigo.val() };

    executarDownload("BiddingAceitamento/ObterTemplateOferta", dados);
}

function enviarDuvidaClick() {
    if (!ValidarCamposObrigatorios(_enviarDuvida)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Você precisa preencher os campos obrigatórios.");
        return;
    }
    executarReST("BiddingAceitamento/EnviarDuvida", { Codigo: _biddingAceitacao.Codigo.val(), Pergunta: _enviarDuvida.DuvidaPergunta.val() }, function (arg) {
        if (arg.Success && arg.Data !== false) {
            LimparCampos(_enviarDuvida);
            _duvidas.Duvidas.val(arg.Data.Duvidas);
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Sua dúvida foi enviada.");
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", "Não foi possível enviar a dúvida.");
        }
    });
}

function ofertarClick(rotaSelecionada) {
    if (rotaSelecionada.SituacaoCodigo == EnumBiddingOfertaSituacao.Aguardando) {
        _ofertasPai.ReplicarICMSDesteModeloVeicular.val(existeReplicarICMSDesteModeloVeicularMarcadoPorOferta(rotaSelecionada.Codigo));
        _ofertasPai.EnviarOferta.visible(true);
        _ofertasPai.InformarVeiculosVerdes.val(false);
    }
    else
        _ofertasPai.EnviarOferta.visible(false);

    $(_tabsOfertas.Tabs.val()[0].valor).addClass("active");

    Global.abrirModal('divModalOferta');

    setTimeout(function () {
        for (let i = 0; i < _ofertasPai.Ofertas().length; i++) {
            for (let j = 0; j < _ofertasPai.Ofertas()[i].KnoutsOfertaEquipamento.val().length; j++) {
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaEquipamento.val()[j].ValorFixoMensal.id).maskMoney(ConfigDecimal());
            }
            for (let j = 0; j < _ofertasPai.Ofertas()[i].KnoutsOfertaFrotaFixaKmRodado.val().length; j++) {
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFrotaFixaKmRodado.val()[j].ValorFixoMensal.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFrotaFixaKmRodado.val()[j].ValorKmRodado.id).maskMoney(ConfigDecimal());
            }
            for (let j = 0; j < _ofertasPai.Ofertas()[i].KnoutsOfertaPorcentagem.val().length; j++) {
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaPorcentagem.val()[j].PorcentagemNota.id).maskMoney(ConfigDecimal());
            }
            for (let j = 0; j < _ofertasPai.Ofertas()[i].KnoutsOfertaViagemAdicional.val().length; j++) {
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaViagemAdicional.val()[j].ValorViagem.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaViagemAdicional.val()[j].ValorAdicionalEntrega.id).maskMoney(ConfigDecimal());
            }
            for (let j = 0; j < _ofertasPai.Ofertas()[i].KnoutsOfertaFrotaFixaFranquia.val().length; j++) {
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFrotaFixaFranquia.val()[j].ValorFixo.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFrotaFixaFranquia.val()[j].Franquia.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFrotaFixaFranquia.val()[j].Quilometragem.id).maskMoney(ConfigDecimal());
            }
            for (let j = 0; j < _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorPeso.val().length; j++) {
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorPeso.val()[j].ICMS.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorPeso.val()[j].FreteTonelada.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorPeso.val()[j].PedagioEixo.id).maskMoney(ConfigDecimal());
            }
            for (let j = 0; j < _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorCapacidade.val().length; j++) {
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorCapacidade.val()[j].ICMS.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorCapacidade.val()[j].FreteTonelada.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorCapacidade.val()[j].PedagioEixo.id).maskMoney(ConfigDecimal());
            }
            for (let j = 0; j < _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorViagem.val().length; j++) {
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorViagem.val()[j].ICMS.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorViagem.val()[j].FreteTonelada.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaFretePorViagem.val()[j].PedagioEixo.id).maskMoney(ConfigDecimal());
            }
            for (let j = 0; j < _ofertasPai.Ofertas()[i].KnoutsOfertaViagemEntregaAjudante.val().length; j++) {
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaViagemEntregaAjudante.val()[j].ICMS.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaViagemEntregaAjudante.val()[j].FreteTonelada.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaViagemEntregaAjudante.val()[j].PedagioEixo.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaViagemEntregaAjudante.val()[j].Ajudante.id).maskMoney(ConfigDecimal());
                $("#" + _ofertasPai.Ofertas()[i].KnoutsOfertaViagemEntregaAjudante.val()[j].AdicionalPorEntrega.id).maskMoney(ConfigDecimal());
            }
        }
    }, 100);

    _ofertasPai.Oferta.removeAll();
    for (let i = 0; i < _ofertasPai.Ofertas().length; i++) {
        const atual = _ofertasPai.Ofertas()[i];
        atual.NomeRota = rotaSelecionada.Rota;
        if (atual.CodigoRota.val() == rotaSelecionada.Codigo) {
            _ofertasPai.Oferta.push(atual);
            break;
        }
    }

    $("#divModalOferta").one('hidden.bs.modal', function () {
        for (let x = 0; x < _tabsOfertas.Tabs.val().length; x++) {
            $(_tabsOfertas.Tabs.val()[x].valor).removeClass("active");
            Global.ResetarAba("knockoutTabsOferta");
        }
    });
}

function naoOfertarClick(rotaSelecionada) {

    _ofertasPai.Oferta.removeAll();
    for (let i = 0; i < _ofertasPai.Ofertas().length; i++) {
        const atual = _ofertasPai.Ofertas()[i];
        atual.NomeRota = rotaSelecionada.Rota;
        if (atual.CodigoRota.val() == rotaSelecionada.Codigo) {
            _ofertasPai.Oferta.push(atual);
            break;
        }
    }

    const arrayOfertas = new Array();
    arrayOfertas.push({ NaoOfertar: true, ModeloVeicular: rotaSelecionada.ModeloVeicular, Oferta: { Codigo: 0 } });

    exibirConfirmacao("Confirmação", "Você realmente não deseja enviar ofertas para essa rota?", function () {
        EnviarRotaNaoOfertada(rotaSelecionada, true);
    });
}

function confirmarNaoOfertarMultiplosClick() {
    exibirConfirmacao("Confirmação", "Você realmente não deseja enviar ofertas para as rotas selecionadas?", function () {
        iniciarRequisicao();
        for (let i = 0; i < _listaRotasNaoOfertar.length; i++) {
            EnviarRotaNaoOfertada(_listaRotasNaoOfertar[i], false);
        }
        finalizarRequisicao();
        exibirMensagem(tipoMensagem.ok, "Sucesso", "Você escolheu não ofertar para essas rotas.");
        ResetarNaoOfertar();
    });
}

function verOfertaClick(rotaSelecionada) {
    executarReST("BiddingAceitamento/BuscarOferta", { Codigo: rotaSelecionada.Codigo, CodigoBiddingConvite: _biddingAceitacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                loadGridsDetalhesOferta(retorno.Data.NaoIncluirImpostoValorTotalOferta);
                SetarTabsDetalheOferta(retorno.Data.Tabs);
                PreencherGridsDetalheOferta(retorno.Data);
                PreencherObjetoKnout(_biddingOfertaDetalhes, retorno);
                MostrarModalOfertaDetalhes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function anexosQuestionarioClick(registroSelecionado) {
    _gridAnexoQuestionario.CarregarGrid(registroSelecionado.ChecklistAnexo);
    Global.abrirModal('divModalInformacoesQuestionario');
    $("#divModalInformacoesQuestionario").one('hidden.bs.modal', function () {
        _gridAnexoQuestionario.CarregarGrid([]);
    });
}

function informacoesRotaClick(registroSelecionado) {
    const origens = obterOrigens(registroSelecionado);
    const destinos = obterDestinos(registroSelecionado);
    const veiculos = registroSelecionado.ModeloVeicular;
    const cargas = registroSelecionado.TipoCarga;

    for (let i = 0; i < origens.length; i++) {
        if (origens[i].Descricao == undefined)
            origens[i]["Descricao"] = origens[i].CEPInicial + " até " + origens[i].CEPFinal;
    }

    for (let i = 0; i < destinos.length; i++) {
        if (destinos[i].Descricao == undefined)
            destinos[i]["Descricao"] = destinos[i].CEPInicial + " até " + destinos[i].CEPFinal;
    }

    _gridOrigem.CarregarGrid(origens);
    _gridDestino.CarregarGrid(destinos);
    _gridCarga.CarregarGrid(cargas);
    _gridVeiculo.CarregarGrid(veiculos);

    _modalRotas.Descricao.text("Descrição: " + registroSelecionado.Descricao);
    _modalRotas.QuilometragemMedia.text("Quilometragem Média: " + registroSelecionado.QuilometragemMedia);
    _modalRotas.Frequencia.text("Frequência: " + registroSelecionado.Frequencia);
    _modalRotas.ValorCargaMes.text("Valor Carga Mês: " + registroSelecionado.ValorCargaMes);
    _modalRotas.Volume.text("Volume: " + registroSelecionado.Volume);
    _modalRotas.Peso.text("Peso: " + registroSelecionado.Peso);
    _modalRotas.AdicionalAPartirDaEntregaNumero.text("Adicional a Partir Da Entrega Número: " + registroSelecionado.AdicionalAPartirDaEntregaNumero);
    _modalRotas.NumeroEntrega.text("Número entrega: " + registroSelecionado.NumeroEntrega);
    _modalRotas.Tomador.text("Tomador: " + registroSelecionado.Tomador);
    _modalRotas.GrupoModeloVeicular.text("Grupo Modelo Veicular: " + registroSelecionado.GrupoModeloVeicular);
    _modalRotas.CarroceriaVeiculo.text("Carroceria Veículo: " + registroSelecionado.CarroceriaVeiculo);
    _modalRotas.FrequenciaMensalComAjudante.text("Frequência Mensal Com Ajudante: " + registroSelecionado.FrequenciaMensalComAjudante);
    _modalRotas.QuantidadeAjudantesPorVeiculo.text("Quantidade de Ajudantes Por Veículo: " + registroSelecionado.QuantidadeAjudantesPorVeiculo);
    _modalRotas.MediaEntregasFracionadas.text("Média de Entregas Fracionadas: " + registroSelecionado.MediaEntregasFracionadas);
    _modalRotas.MaximaEntregasFracionadas.text("Máxima de Entregas Fracionadas: " + registroSelecionado.MaximaEntregasFracionadas);
    _modalRotas.Inconterm.text("Inconterm: " + registroSelecionado.Inconterm);
    _modalRotas.QuantidadeViagensPorAno.text("Quantidade de Viagens Por Ano: " + registroSelecionado.QuantidadeViagensPorAno);
    _modalRotas.VolumeTonAno.text("Volume Ton. Por Ano: " + registroSelecionado.VolumeTonAno);
    _modalRotas.VolumeTonViagem.text("Volume Ton. Por Viagem: " + registroSelecionado.VolumeTonViagem);
    _modalRotas.TempoColeta.text("Tempo de Coleta: " + registroSelecionado.TempoColeta);
    _modalRotas.TempoDescarga.text("Tempo de Descarga: " + registroSelecionado.TempoDescarga);
    _modalRotas.Compressor.text("Compressor: " + registroSelecionado.Compressor);
    if (registroSelecionado.Observacao != "")
        _modalRotas.Observacao.text("Observação: " + registroSelecionado.Observacao);

    Global.abrirModal('divModalInformacoesRota');
    $("#divModalInformacoesRota").one('hidden.bs.modal', function () {
        _gridOrigem.CarregarGrid([]);
        _gridDestino.CarregarGrid([]);
        _gridVeiculo.CarregarGrid([]);
        _gridCarga.CarregarGrid([]);
    });
}

function detalhesOfertaClick(registroSelecionado) {
    Global.abrirModal('divModalDetalhesResultado');
    _detalhesResultado.Frequencia.val(registroSelecionado.Frequencia);
    _detalhesResultado.ValorCargaMes.val(registroSelecionado.ValorCargaMes);
    _detalhesResultado.FranquiaKm.val(registroSelecionado.FranquiaKm);
    _detalhesResultado.Volume.val(registroSelecionado.Volume);
}

function downloadAnexoClick(registroSelecionado) {
    const dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("BiddingConviteAnexo/DownloadAnexo", dados);
}

function downloadAnexoTipoBiddingClick(registroSelecionado) {
    const dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("TipoBiddingAnexo/DownloadAnexo", dados);
}

function downloadQuestionarioAnexoClick(registroSelecionado) {
    const dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("BiddingChecklistAnexo/DownloadAnexo", dados);
}

function enviarQuestionarioClick() {
    const dados = obterDadosChecklist();
    executarReST("BiddingAceitamento/EnviarRespostas", { Dados: dados, CodigoBidding: _biddingAceitacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Respostas enviadas com sucesso");
                let CodigosRespostas = retorno.Data.CodigosRespostas;
                enviarAceitamentoRespostaPerguntaAnexos(CodigosRespostas);
                PesquisarConvite();
                CarregarClick({ Codigo: _biddingAceitacao.Codigo.val() });
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function checarValor(e) {
    if (parseFloat(e.Quilometragem.val()) > parseFloat(_kmMedio))
        e.Quilometragem.val(_kmMedio);
}

function naoOfertarMultiplosClick() {
    if (_naoOfertar.NaoOfertarMultiplosSituacao.val()) {
        ResetarNaoOfertar();
        exibirMensagem(tipoMensagem.aviso, "Selecionar Rotas Para Não Ofertar:", "Operação Desativada");
    } else {
        _naoOfertar.NaoOfertarMultiplosSituacao.val(true);
        _naoOfertar.ConfirmarNaoOfertarMultiplos.visible(true);
        _naoOfertar.NaoOfertarMultiplos.text("Desativar Seleção de Rotas");
        exibirMensagem(tipoMensagem.aviso, "Selecionar Rotas Para Não Ofertar:", "Operação Ativada");
    }
}

function EnviarRotaNaoOfertada(rotaSelecionada, recarregarGrid) {
    _ofertasPai.Oferta.removeAll();
    for (let i = 0; i < _ofertasPai.Ofertas().length; i++) {
        const atual = _ofertasPai.Ofertas()[i];
        atual.NomeRota = rotaSelecionada.Rota;
        if (atual.CodigoRota.val() == rotaSelecionada.Codigo) {
            _ofertasPai.Oferta.push(atual);
            break;
        }
    }

    const arrayOfertas = new Array();
    arrayOfertas.push({ NaoOfertar: true, Oferta: { Codigo: 0, ModeloVeicular: rotaSelecionada.CodigoModeloVeicular } });

    executarReST("BiddingAceitamento/EnviarOfertas", { CodigoOfertaRota: _ofertasPai.Oferta()[0].Codigo.val(), Ofertas: JSON.stringify(arrayOfertas) }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (recarregarGrid) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Você escolheu não ofertar para essa rota.");
                    LimparGridNaoOfertar();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function MarcarOfertasNaoOfertar(linhaSelecionada) {
    if (linhaSelecionada.Situacao != "Aguardando Oferta") return;

    const gridId = _gridOfertasRotas.GetGridId();
    var jaOfertado = false;

    for (var i = 0; i < _listaRotasNaoOfertar.length; i++) {
        if (linhaSelecionada.Codigo === _listaRotasNaoOfertar[i].Codigo.toString()) {
            _listaRotasNaoOfertar.splice(i, 1);
            $("#" + gridId + " #" + linhaSelecionada.DT_RowId).css({ 'background-color': '#7eacd4' });
            jaOfertado = true;
            break;
        }
    }

    if (jaOfertado == false) {
        _listaRotasNaoOfertar.push(linhaSelecionada);
        $("#" + gridId + " #" + linhaSelecionada.DT_RowId).css({ 'background-color': '#2196F3' });
    }
}

function ObterResumosBidding() {
    executarReST("BiddingAceitamento/ObterResumosBidding", { DataInicial: _pesquisaBiddingConvite.DataInicio.val(), DataLimite: _pesquisaBiddingConvite.DataLimite.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaBiddingConvite.ResumoTodos.val(retorno.Data.ResumoTodos);
                _pesquisaBiddingConvite.ResumoAgConvite.val(retorno.Data.ResumoAgConvite);
                _pesquisaBiddingConvite.ResumoAgChecklist.val(retorno.Data.ResumoAgChecklist);
                _pesquisaBiddingConvite.ResumoAgOfertas.val(retorno.Data.ResumoAgOfertas);
                _pesquisaBiddingConvite.ResumoFinalizados.val(retorno.Data.ResumoFinalizados);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//#endregion

//#region Funções privadas
function LimparGridseCampos() {
    tempoRestante = undefined;
    LimparCampos(_biddingAceitacao);
    _ofertasPai.Ofertas.removeAll();

    _gridAnexo.CarregarGrid([]);
    _gridAnexoTipoBidding.CarregarGrid([]);
    _gridChecklistQuestionarios.CarregarGrid([]);
    _gridRotasBasicTable.CarregarGrid([]);

    _biddingAceitacao.TempoRestante.text("");
    _biddingAceitacao.TipoFrete.text("");
}

function ResetarNaoOfertar() {
    _naoOfertar.NaoOfertarMultiplosSituacao.val(false);
    _naoOfertar.ConfirmarNaoOfertarMultiplos.visible(false);
    LimparGridNaoOfertar();
    _naoOfertar.NaoOfertarMultiplos.text("Selecionar Rotas Para Não Ofertar");
}

function LimparGridNaoOfertar() {
    _gridOfertasRotas.CarregarGrid();
    PesquisarConvite();
    _listaRotasNaoOfertar = [];
}

function obterOrigens(registroSelecionado) {
    if (registroSelecionado.FlagOrigem == "Cidade") {
        return registroSelecionado.CidadeOrigem;
    }
    else if (registroSelecionado.FlagOrigem == "Cliente") {
        return registroSelecionado.ClienteOrigem;
    }
    else if (registroSelecionado.FlagOrigem == "Estado") {
        return registroSelecionado.EstadoOrigem;
    }
    else if (registroSelecionado.FlagOrigem == "Regiao") {
        return registroSelecionado.RegiaoOrigem;
    }
    else if (registroSelecionado.FlagOrigem == "Rota") {
        return registroSelecionado.RotaOrigem;
    }
    else if (registroSelecionado.FlagOrigem == "Pais") {
        return registroSelecionado.PaisOrigem;
    }
    else if (registroSelecionado.FlagOrigem == "CEP") {
        return registroSelecionado.CEPOrigem;
    }
}

function obterDestinos(registroSelecionado) {
    if (registroSelecionado.FlagDestino == "Cidade") {
        return registroSelecionado.CidadeDestino;
    }
    else if (registroSelecionado.FlagDestino == "Cliente") {
        return registroSelecionado.ClienteDestino;
    }
    else if (registroSelecionado.FlagDestino == "Estado") {
        return registroSelecionado.EstadoDestino;
    }
    else if (registroSelecionado.FlagDestino == "Regiao") {
        return registroSelecionado.RegiaoDestino;
    }
    else if (registroSelecionado.FlagDestino == "Rota") {
        return registroSelecionado.RotaDestino;
    }
    else if (registroSelecionado.FlagDestino == "Pais") {
        return registroSelecionado.PaisDestino;
    }
    else if (registroSelecionado.FlagDestino == "CEP") {
        return registroSelecionado.CEPDestino;
    }
}

function esconderBotoes() {
    _CRUDBidding.Aceitar.visible(false);
    _CRUDBidding.Recusar.visible(false);
}

function esconderBotoesChecklist() {
    _CRUDQuestao.EnviarQuestionario.visible(false);
}

function mostrarBotoes() {
    _CRUDBidding.Aceitar.visible(true);
    _CRUDBidding.Recusar.visible(true);
}

function mostrarBotoesChecklist() {
    _CRUDQuestao.EnviarQuestionario.visible(true);
}

function scrollDuvidas() {
    let scrollMax = $("#duvidas").prop('scrollHeight');
    $("#duvidas").scrollTop(scrollMax);
}

function VerificarStatus(status) {
    $("#knockoutCRUDBidding").slideDown(0);
    $("#CRUDChecklistQuestionarios").slideDown(0);
    $("#knockoutOfertasTab").slideDown(0);
    if (status == EnumStatusBidding.Checklist) {
        esconderBotoes();
        _biddingAceitacao.AceitoTermo.visible(false);
        mostrarBotoesChecklist();
    }
    else if (status == EnumStatusBidding.Ofertas) {
        esconderBotoes();
        esconderBotoesChecklist();
        _biddingAceitacao.AceitoTermo.visible(false);
        _naoOfertar.NaoOfertarMultiplos.visible(true);
        _CRUDOfertas.ImportarOfertas.visible(true);
        _CRUDOfertas.ObterTemplateOferta.visible(true);
        esconderBotoesChecklist();
        for (let i = 0; i < _biddingAceitacaoChecklist.Questoes().length; i++) {
            const knout = _biddingAceitacaoChecklist.Questoes()[i];
            knout.Resposta.enable(false);
            knout.Observacao.enable(false);
        }
    }
    else if (status == EnumStatusBidding.Fechamento) {
        esconderBotoes();
        esconderBotoesChecklist();
        _biddingAceitacao.AceitoTermo.visible(false);
        _naoOfertar.NaoOfertarMultiplos.visible(false);
        $("#knockoutCRUDBidding").slideUp(0);
        $("#CRUDChecklistQuestionarios").slideUp(0);
        $("#knockoutOfertasTab").slideUp(0);
        esconderBotoesChecklist();
        for (let i = 0; i < _biddingAceitacaoChecklist.Questoes().length; i++) {
            const knout = _biddingAceitacaoChecklist.Questoes()[i];
            knout.Resposta.enable(false);
            knout.Observacao.enable(false);
        }
    }
    else {
        mostrarBotoes();
        mostrarBotoesChecklist();
        _biddingAceitacao.AceitoTermo.visible(true);
        _CRUDQuestao.EnviarQuestionario.visible(true);
    }
}

function VerificarStatusConviteConvidado(status) {
    if (status == 1 || status == 2) {
        _CRUDBidding.Aceitar.visible(false);
        _biddingAceitacao.AceitoTermo.visible(false);
    }
}

function verAnexos(e) {
    _codigoPergunta = e.Codigo.val();
    recarregarGrid();
    Global.abrirModal('divModalListaAnexo');
    $("#divModalListaAnexo").one('hidden.bs.modal', function () {
        _codigoPergunta = 0;
    });
}
function verAnexosQuestionario() {
    Global.abrirModal('divModalInformacoesQuestionario');
}

function enviarAnexo(e) {
    _codigoPergunta = e.Codigo.val();
    Global.abrirModal('divModalAnexo');
    $("#divModalAnexo").one('hidden.bs.modal', function () {
        _codigoPergunta = 0;
        LimparCampos(_anexoChecklist);
    });
}

function obterDadosChecklist() {
    let arrayDados = new Array();
    for (let i = 0; i < _biddingAceitacaoChecklist.Questoes.slice().length; i++) {
        const dado = ({
            CodigoPergunta: _biddingAceitacaoChecklist.Questoes.slice()[i].Codigo.val(),
            Resposta: _biddingAceitacaoChecklist.Questoes.slice()[i].Resposta.val(),
            Observacao: _biddingAceitacaoChecklist.Questoes.slice()[i].Observacao.val()
        });

        arrayDados.push(dado);
    }
    return JSON.stringify(arrayDados);
}

function enviarAceitamentoRespostaPerguntaAnexos(listaCodigosRespostas) {
    const arrayTodosAnexos = _listaAnexoChecklist.Anexos.val();
    for (let i = 0; i < listaCodigosRespostas.length; i++) {
        const listaAnexosPorResposta = new Array();
        for (let x = 0; x < arrayTodosAnexos.length; x++) {
            if (listaCodigosRespostas[i].CodigoPergunta == arrayTodosAnexos[x].CodigoPergunta) {
                listaAnexosPorResposta.push(arrayTodosAnexos[x]);
                arrayTodosAnexos.splice(x, 1);
            }
        }
        const formData = obterFormDataRespostaAnexo(listaAnexosPorResposta);
        salvarAceitamentoRespostaAnexo(formData, listaCodigosRespostas[i].Codigo);
    }
}

function obterFormDataRespostaAnexo(anexos) {
    if (anexos.length > 0) {
        const formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function salvarAceitamentoRespostaAnexo(formData, CodigoResposta) {
    if (formData) {
        enviarArquivo("BiddingAceitamentoAnexo/AnexarArquivos", { Codigo: CodigoResposta }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function verificarTiposOfertas(oferta) {
    let arrayOfertas = new Array();
    if (oferta.TipoLance == EnumTipoLanceBidding.LanceFrotaFixaFranquia)
        arrayOfertas.push({
            texto: "Lance frota fixa com franquia de km",
            valor: "#LanceFrotaFixaFranquia",
            valorDetalhes: "#LanceFrotaFixaFranquiaDetalhes"
        });
    if (oferta.TipoLance == EnumTipoLanceBidding.LanceFrotaFixaKmRodado)
        arrayOfertas.push({
            texto: "Lance frota fixa + km rodado",
            valor: "#LanceFrotaFixaKmRodado",
            valorDetalhes: "#LanceFrotaFixaKmRodadoDetalhes"
        });
    if (oferta.TipoLance == EnumTipoLanceBidding.LancePorcentagemNota)
        arrayOfertas.push({
            texto: "% sobre nota",
            valor: "#LancePorcentagemNota",
            valorDetalhes: "#LancePorcentagemNotaDetalhes"
        });
    if (oferta.TipoLance == EnumTipoLanceBidding.LancePorEquipamento)
        arrayOfertas.push({
            texto: "Equipamento (frota fixa)",
            valor: "#LancePorEquipamento",
            valorDetalhes: "#LancePorEquipamentoDetalhes"
        });
    if (oferta.TipoLance == EnumTipoLanceBidding.LanceViagemAdicional)
        arrayOfertas.push({
            texto: "Por viagem + adicional",
            valor: "#LanceViagemAdicional",
            valorDetalhes: "#LanceViagemAdicionalDetalhes"
        });
    if (oferta.TipoLance == EnumTipoLanceBidding.LancePorPeso)
        arrayOfertas.push({
            texto: "Lance frete por Peso",
            valor: "#LancePorPeso",
            valorDetalhes: "#LancePorPesoDetalhes"
        });

    if (oferta.TipoLance == EnumTipoLanceBidding.LancePorCapacidade)
        arrayOfertas.push({
            texto: "Lance de Frete por Capacidade",
            valor: "#LancePorCapacidade",
            valorDetalhes: "#LancePorCapacidadeDetalhes"
        });

    if (oferta.TipoLance == EnumTipoLanceBidding.LancePorFreteViagem)
        arrayOfertas.push({
            texto: "Lance de Frete por Viagem",
            valor: "#LancePorFreteViagem",
            valorDetalhes: "#LancePorViagemDetalhes"
        });
    if (oferta.TipoLance == EnumTipoLanceBidding.LancePorViagemEntregaAjudante)
        arrayOfertas.push({
            texto: "Lance por Viagem com Entrega e Ajudante",
            valor: "#LancePorViagemEntregaAjudante",
            valorDetalhes: "#LancePorViagemEntregaAjudanteDetalhes"
        });

    _tabsOfertas.Tabs.val(arrayOfertas);

    for (let x = 0; x < _tabsOfertas.Tabs.val().length; x++) {
        $(_tabsOfertas.Tabs.val()[x].valorDetalhes).removeClass("active");
        $(_tabsOfertas.Tabs.val()[x].valor).removeClass("active");
    }

    if (_tabsOfertas.Tabs.val() != "") {
        $(_tabsOfertas.Tabs.val()[0].valor).addClass("active");
        $(_tabsOfertas.Tabs.val()[0].valorDetalhes).addClass("active");
    }


    _tabDetalhesOfertas.Tabs.val(arrayOfertas);
}

function preencherTabOfertaPai(ofertasRotas, naoPossuiPedagioFluxoOferta, naoIncluirImpostoValorTotalOferta) {

    for (let i = 0; i < ofertasRotas.length; i++) {
        const ofertaRota = ofertasRotas[i];
        let arrayTiposCarga = new Array();

        for (let x = 0; x < ofertaRota.TipoCarga.length; x++) {
            arrayTiposCarga.push(ofertaRota.TipoCarga[x].Descricao);
        }
        const arrayKnoutsEquipamento = new Array();
        const arrayKnoutsFrotaFixaKmRodado = new Array();
        const arrayKnoutsPorcentagemNota = new Array();
        const arrayKnoutsViagemAdicional = new Array();
        const arrayKnoutsFrotaFixaFranquia = new Array();
        const arrayKnoutsFretePorPeso = new Array();
        const arrayKnoutsFretePorCapacidade = new Array();
        const arrayKnoutsFretePorViagem = new Array();
        const arrayKnoutsViagemEntregaAjudante = new Array();

        for (let x = 0; x < ofertaRota.ModelosVeiculares.length; x++) {
            const veiculo = ofertaRota.ModelosVeiculares[x];
            const CodigoVeiculo = veiculo.Codigo;
            const knoutEquipamento = new LanceEquipamento();
            const knoutFixaKmRodado = new LanceFrotaFixaKmRodado();
            const knoutPorcentagemNota = new LancePorcentagemNota();
            const knoutViagemAdicional = new LanceViagemAdicional();
            const knoutFixaFranquia = new LanceFrotaFixaFranquia();
            const knoutFretePorPeso = new LancePorPeso(ofertaRota.AlicotaPadraoICMS);
            const knoutFretePorCapacidade = new LancePorCapacidade(ofertaRota.AlicotaPadraoICMS);
            const knoutFretePorViagem = new LancePorFreteViagem(ofertaRota.AlicotaPadraoICMS);
            const knoutViagemEntregaAjudante = new LanceViagemEntregaAjudante(ofertaRota.AlicotaPadraoICMS);

            knoutViagemEntregaAjudante.PedagioEixo.visible(!naoPossuiPedagioFluxoOferta);
            knoutFretePorViagem.PedagioEixo.visible(!naoPossuiPedagioFluxoOferta);
            knoutFretePorCapacidade.PedagioEixo.visible(!naoPossuiPedagioFluxoOferta);
            knoutFretePorPeso.PedagioEixo.visible(!naoPossuiPedagioFluxoOferta);

            knoutViagemEntregaAjudante.ICMS.enable(!naoIncluirImpostoValorTotalOferta);
            knoutFretePorViagem.ICMS.enable(!naoIncluirImpostoValorTotalOferta);
            knoutFretePorCapacidade.ICMS.enable(!naoIncluirImpostoValorTotalOferta);
            knoutFretePorPeso.ICMS.enable(!naoIncluirImpostoValorTotalOferta);

            setarCodigoId(knoutEquipamento, "Equipamento", CodigoVeiculo);
            setarCodigoId(knoutFixaKmRodado, "FrotaFixaKmRodado", CodigoVeiculo);
            setarCodigoId(knoutPorcentagemNota, "PorcentagemNota", CodigoVeiculo);
            setarCodigoId(knoutViagemAdicional, "ViagemAdicional", CodigoVeiculo);
            setarCodigoId(knoutFixaFranquia, "FrotaFixaFranquia", CodigoVeiculo);
            setarCodigoId(knoutFretePorPeso, "FretePorPeso", CodigoVeiculo);
            setarCodigoId(knoutFretePorCapacidade, "FretePorCapacidade", CodigoVeiculo);
            setarCodigoId(knoutFretePorViagem, "FretePorViagem", CodigoVeiculo);
            setarCodigoId(knoutViagemEntregaAjudante, "ViagemEntregaAjudante", CodigoVeiculo);

            arrayKnoutsEquipamento.push(knoutEquipamento);
            arrayKnoutsFrotaFixaKmRodado.push(knoutFixaKmRodado);
            arrayKnoutsPorcentagemNota.push(knoutPorcentagemNota);
            arrayKnoutsViagemAdicional.push(knoutViagemAdicional);
            arrayKnoutsFrotaFixaFranquia.push(knoutFixaFranquia);
            arrayKnoutsFretePorPeso.push(knoutFretePorPeso);
            arrayKnoutsFretePorCapacidade.push(knoutFretePorCapacidade);
            arrayKnoutsFretePorViagem.push(knoutFretePorViagem);
            arrayKnoutsViagemEntregaAjudante.push(knoutViagemEntregaAjudante);

            adicionarControleReplicarICMSModeloVeicular(knoutFretePorPeso, ofertaRota.Codigo);
            adicionarControleReplicarICMSModeloVeicular(knoutFretePorCapacidade, ofertaRota.Codigo);
            adicionarControleReplicarICMSModeloVeicular(knoutFretePorViagem, ofertaRota.Codigo);
            adicionarControleReplicarICMSModeloVeicular(knoutViagemEntregaAjudante, ofertaRota.Codigo);

            knoutEquipamento.OfertarModeloVeiculo.val.subscribe(function (novoValor) {
                if (novoValor) {
                    knoutEquipamento.ValorFixoMensal.enable(true);
                } else {
                    knoutEquipamento.ValorFixoMensal.enable(false);
                    LimparCampo(knoutEquipamento.ValorFixoMensal);
                }
            });

            knoutFixaKmRodado.OfertarModeloVeiculo.val.subscribe(function (novoValor) {
                if (novoValor) {
                    knoutFixaKmRodado.ValorFixoMensal.enable(true);
                    knoutFixaKmRodado.ValorKmRodado.enable(true);
                } else {
                    knoutFixaKmRodado.ValorFixoMensal.enable(false);
                    LimparCampo(knoutFixaKmRodado.ValorFixoMensal);
                    knoutFixaKmRodado.ValorKmRodado.enable(false);
                    LimparCampo(knoutFixaKmRodado.ValorKmRodado);
                }
            });

            knoutPorcentagemNota.OfertarModeloVeiculo.val.subscribe(function (novoValor) {
                if (novoValor) {
                    knoutPorcentagemNota.PorcentagemNota.enable(true);
                } else {
                    knoutPorcentagemNota.PorcentagemNota.enable(false);
                    LimparCampo(knoutPorcentagemNota.PorcentagemNota);
                }
            });

            knoutViagemAdicional.OfertarModeloVeiculo.val.subscribe(function (novoValor) {
                if (novoValor) {
                    knoutViagemAdicional.ValorViagem.enable(true);
                    knoutViagemAdicional.ValorAdicionalEntrega.enable(true);
                } else {
                    knoutViagemAdicional.ValorViagem.enable(false);
                    LimparCampo(knoutViagemAdicional.ValorViagem);
                    knoutViagemAdicional.ValorAdicionalEntrega.enable(false);
                    LimparCampo(knoutViagemAdicional.ValorAdicionalEntrega);
                }
            });

            knoutFixaFranquia.OfertarModeloVeiculo.val.subscribe(function (novoValor) {
                if (novoValor) {
                    knoutFixaFranquia.ValorFixo.enable(true);
                    knoutFixaFranquia.Franquia.enable(true);
                    knoutFixaFranquia.Quilometragem.enable(true);
                } else {
                    knoutFixaFranquia.ValorFixo.enable(false);
                    LimparCampo(knoutFixaFranquia.ValorFixo);
                    knoutFixaFranquia.Franquia.enable(false);
                    LimparCampo(knoutFixaFranquia.Franquia);
                    knoutFixaFranquia.Quilometragem.enable(false);
                    LimparCampo(knoutFixaFranquia.Quilometragem);
                }
            });

            knoutFretePorPeso.FreteTonelada.text("Frete (R$ por TON) - Sem ICMS p/" + CalcularValorToneladas(veiculo.CapacidadePesoTransporte) + ":");
            knoutFretePorPeso.PedagioEixo.text("Pedágio (R$ por Eixo) - Sem ICMS - p/" + veiculo.NumeroEixos + " Eixos:");

            knoutFretePorPeso.OfertarModeloVeiculo.val.subscribe(function (novoValor) {
                if (novoValor) {
                    var ativarICMS = naoIncluirImpostoValorTotalOferta || _ofertasPai.ReplicarICMSDesteModeloVeicular.val();

                    knoutFretePorPeso.ICMS.enable(!ativarICMS);
                    knoutFretePorPeso.FreteTonelada.enable(true);
                    knoutFretePorPeso.PedagioEixo.enable(true);
                } else {
                    knoutFretePorPeso.ICMS.enable(false);
                    LimparCampo(knoutFretePorPeso.ICMS);
                    knoutFretePorPeso.FreteTonelada.enable(false);
                    LimparCampo(knoutFretePorPeso.FreteTonelada);
                    knoutFretePorPeso.PedagioEixo.enable(false);
                    LimparCampo(knoutFretePorPeso.PedagioEixo);
                }
            });

            knoutFretePorCapacidade.FreteTonelada.text("Frete (R$ por TON) - Sem ICMS p/" + CalcularValorToneladas(veiculo.CapacidadePesoTransporte) + ":");
            knoutFretePorCapacidade.PedagioEixo.text("Pedágio (R$ por Eixo) - Sem ICMS - p/" + veiculo.NumeroEixos + " Eixos:");

            knoutFretePorCapacidade.OfertarModeloVeiculo.val.subscribe(function (novoValor) {
                if (novoValor) {
                    var ativarICMS = naoIncluirImpostoValorTotalOferta || _ofertasPai.ReplicarICMSDesteModeloVeicular.val();

                    knoutFretePorCapacidade.ICMS.enable(!ativarICMS);
                    knoutFretePorCapacidade.FreteTonelada.enable(true);
                    knoutFretePorCapacidade.PedagioEixo.enable(true);
                } else {
                    knoutFretePorCapacidade.ICMS.enable(false);
                    LimparCampo(knoutFretePorCapacidade.ICMS);
                    knoutFretePorCapacidade.FreteTonelada.enable(false);
                    LimparCampo(knoutFretePorCapacidade.FreteTonelada);
                    knoutFretePorCapacidade.PedagioEixo.enable(false);
                    LimparCampo(knoutFretePorCapacidade.PedagioEixo);
                }
            });

            knoutFretePorViagem.FreteTonelada.text("Frete - Sem ICMS (R$ por Viagem):");
            knoutFretePorViagem.PedagioEixo.text("Pedágio (R$ por Eixo) - Sem ICMS - p/" + veiculo.NumeroEixos + " Eixos:");

            knoutFretePorViagem.OfertarModeloVeiculo.val.subscribe(function (novoValor) {
                if (novoValor) {
                    var ativarICMS = naoIncluirImpostoValorTotalOferta || _ofertasPai.ReplicarICMSDesteModeloVeicular.val();

                    knoutFretePorViagem.ICMS.enable(!ativarICMS);
                    knoutFretePorViagem.FreteTonelada.enable(true);
                    knoutFretePorViagem.PedagioEixo.enable(true);
                } else {
                    knoutFretePorViagem.ICMS.enable(false);
                    LimparCampo(knoutFretePorViagem.ICMS);
                    knoutFretePorViagem.FreteTonelada.enable(false);
                    LimparCampo(knoutFretePorViagem.FreteTonelada);
                    knoutFretePorViagem.PedagioEixo.enable(false);
                    LimparCampo(knoutFretePorViagem.PedagioEixo);
                }
            });

            knoutViagemEntregaAjudante.FreteTonelada.text("Frete (R$ por Viagem):");
            knoutViagemEntregaAjudante.PedagioEixo.text("Pedágio (R$ por Eixo):");

            knoutViagemEntregaAjudante.OfertarModeloVeiculo.val.subscribe(function (novoValor) {
                if (novoValor) {
                    var ativarICMS = naoIncluirImpostoValorTotalOferta || _ofertasPai.ReplicarICMSDesteModeloVeicular.val();

                    knoutViagemEntregaAjudante.ICMS.enable(!ativarICMS);
                    knoutViagemEntregaAjudante.FreteTonelada.enable(true);
                    knoutViagemEntregaAjudante.PedagioEixo.enable(true);
                    knoutViagemEntregaAjudante.Ajudante.enable(true);
                    knoutViagemEntregaAjudante.AdicionalPorEntrega.enable(true);
                } else {
                    knoutViagemEntregaAjudante.ICMS.enable(false);
                    LimparCampo(knoutViagemEntregaAjudante.ICMS);
                    knoutViagemEntregaAjudante.FreteTonelada.enable(false);
                    LimparCampo(knoutViagemEntregaAjudante.FreteTonelada);
                    knoutViagemEntregaAjudante.PedagioEixo.enable(false);
                    LimparCampo(knoutViagemEntregaAjudante.PedagioEixo);
                    knoutViagemEntregaAjudante.Ajudante.enable(false);
                    LimparCampo(knoutViagemEntregaAjudante.Ajudante);
                    knoutViagemEntregaAjudante.AdicionalPorEntrega.enable(false);
                    LimparCampo(knoutViagemEntregaAjudante.AdicionalPorEntrega);
                }
            });
        }
        ofertaRota.TipoCarga = arrayTiposCarga.join(', ');
        const knoutOfertaRota = new TabPaiOferta(ofertaRota);
        knoutOfertaRota.KnoutsOfertaEquipamento.val(arrayKnoutsEquipamento);
        knoutOfertaRota.KnoutsOfertaFrotaFixaKmRodado.val(arrayKnoutsFrotaFixaKmRodado);
        knoutOfertaRota.KnoutsOfertaPorcentagem.val(arrayKnoutsPorcentagemNota);
        knoutOfertaRota.KnoutsOfertaFrotaFixaFranquia.val(arrayKnoutsFrotaFixaFranquia);
        knoutOfertaRota.KnoutsOfertaViagemAdicional.val(arrayKnoutsViagemAdicional);
        knoutOfertaRota.KnoutsOfertaFretePorPeso.val(arrayKnoutsFretePorPeso);
        knoutOfertaRota.KnoutsOfertaFretePorCapacidade.val(arrayKnoutsFretePorCapacidade);
        knoutOfertaRota.KnoutsOfertaFretePorViagem.val(arrayKnoutsFretePorViagem);
        knoutOfertaRota.KnoutsOfertaViagemEntregaAjudante.val(arrayKnoutsViagemEntregaAjudante);
        _ofertasPai.Ofertas.push(knoutOfertaRota);
    }
}

function adicionarControleReplicarICMSModeloVeicular(knoutTipoLance, codigoOfertaRota) {
    knoutTipoLance.OfertarModeloVeiculo.val.subscribe(function (ofertarModeloVeiculoChecked) {
        var possuiReplicarICMSMarcadoNaTela = _ofertasPai.ReplicarICMSDesteModeloVeicular.val();
        if (!possuiReplicarICMSMarcadoNaTela) {
            controlarCampoReplicarICMSModeloVeicular(codigoOfertaRota, true);
        }

        var replicarICMSDesteModeloVeicular = knoutTipoLance.ReplicarICMSDesteModeloVeicular.val();
        if (ofertarModeloVeiculoChecked && !replicarICMSDesteModeloVeicular && possuiReplicarICMSMarcadoNaTela) {
            knoutTipoLance.ICMS.val(obterICMSParaReplicar(codigoOfertaRota));
            knoutTipoLance.ICMS.enable(false);
        } else {
            knoutTipoLance.ICMS.enable(true);

            if (replicarICMSDesteModeloVeicular) {
                knoutTipoLance.ReplicarICMSDesteModeloVeicular.val(false);
            }
        }
    });

    knoutTipoLance.ReplicarICMSDesteModeloVeicular.val.subscribe(function (replicarICMSDesteModeloVeicularChecked) {
        _ofertasPai.ReplicarICMSDesteModeloVeicular.val(replicarICMSDesteModeloVeicularChecked);

        if (replicarICMSDesteModeloVeicularChecked) {
            controlarCampoReplicarICMSModeloVeicular(codigoOfertaRota, false, knoutTipoLance);

            replicarICMS(codigoOfertaRota, knoutTipoLance.ICMS.val(), knoutTipoLance);
        } else {
            controlarCampoReplicarICMSModeloVeicular(codigoOfertaRota, true);
        }
    });

    knoutTipoLance.ICMS.val.subscribe(function (novoValorICMS) {
        if (knoutTipoLance.ReplicarICMSDesteModeloVeicular.val()) {
            replicarICMS(codigoOfertaRota, novoValorICMS, knoutTipoLance);
        }
    });
}

function obterICMSParaReplicar(codigoOfertaRota) {
    var knoutAbas = obterKnoutAbasPorTipoOferta(codigoOfertaRota);

    for (var i = 0; i < knoutAbas.length; i++) {
        var knout = knoutAbas[i];

        if (knout.ReplicarICMSDesteModeloVeicular.val()) {
            return knout.ICMS.val();
        }
    }

    return 0;
}

function existeReplicarICMSDesteModeloVeicularMarcadoPorOferta(codigoRota) {
    var oferta = _ofertasPai.Ofertas().filter(oferta => oferta.CodigoRota.val() == codigoRota)[0];
    var knoutAbas = obterKnoutAbasPorOferta(oferta);

    for (var i = 0; i < knoutAbas.length; i++) {
        var knout = knoutAbas[i];

        if (knout.ReplicarICMSDesteModeloVeicular.val()) {
            return true;
        }
    }

    return false;
}

function replicarICMS(codigoOfertaRota, valorICMS, knoutNaoAlterar) {
    var knoutAbas = obterKnoutAbasPorTipoOferta(codigoOfertaRota);

    for (var i = 0; i < knoutAbas.length; i++) {
        var knout = knoutAbas[i];

        if (knoutNaoAlterar && knout.IdTab.val == knoutNaoAlterar.IdTab.val) {
            continue;
        }

        if (knout.OfertarModeloVeiculo.val()) {
            knout.ICMS.val(valorICMS);
            knout.ICMS.enable(false);
        }
    }
}

function controlarCampoReplicarICMSModeloVeicular(codigoOfertaRota, mostrar, knoutNaoAlterar) {
    var knoutAbas = obterKnoutAbasPorTipoOferta(codigoOfertaRota);

    for (var i = 0; i < knoutAbas.length; i++) {
        var knout = knoutAbas[i];

        if (knoutNaoAlterar && knout.IdTab.val == knoutNaoAlterar.IdTab.val) {
            continue;
        }

        if (knout.OfertarModeloVeiculo.val()) {
            knout.ReplicarICMSDesteModeloVeicular.visible(mostrar);
            knout.ICMS.enable(mostrar);
        } else {
            knout.ReplicarICMSDesteModeloVeicular.visible(false);
        }
    }
}

function obterKnoutAbasPorOferta(oferta) {
    var knoutsOfertaFretePorPeso = oferta.KnoutsOfertaFretePorPeso.val();
    var knoutsOfertaFretePorCapacidade = oferta.KnoutsOfertaFretePorCapacidade.val();
    var knoutsOfertaFretePorViagem = oferta.KnoutsOfertaFretePorViagem.val();
    var knoutsOfertaViagemEntregaAjudante = oferta.KnoutsOfertaViagemEntregaAjudante.val();

    var campos = []
    campos = campos.concat(knoutsOfertaFretePorPeso, knoutsOfertaFretePorCapacidade, knoutsOfertaFretePorViagem, knoutsOfertaViagemEntregaAjudante)
    return campos;
}

function obterKnoutAbasPorTipoOferta(codigoOfertaRota) {
    var oferta = _ofertasPai.Ofertas().filter(oferta => oferta.Codigo.val() == codigoOfertaRota)[0];
    return obterKnoutAbasPorOferta(oferta);
}

function preencherOpcoesRotas(listaRotas) {
    const rotas = [{ text: "Todas", value: "" }];

    for (let i = 0; i < listaRotas.length; i++) {
        rotas.push({
            text: listaRotas[i].Rota,
            value: listaRotas[i].Codigo
        });
    }

    _resultado.Rota.options(rotas);
}

function setarCodigoId(knout, idTab, codigoVeiculo) {
    knout.CodigoVeiculo.val = codigoVeiculo;
    knout.IdTab.val = idTab + codigoVeiculo;
}

function SetarTabsDetalheOferta(tabs) {

    for (let i = 1; i <= 9; i++) {
        const tabId = "#tabDetalhe" + i;
        $(tabId).hide();
        $("#tab" + i).removeClass("active show");
    }

    if (tabs.length > 0) {
        $(tabs[0].Identificador).show();
        var primeiroVisivel = tabs[0].Identificador.slice(tabs[0].Identificador.length - 1);
        $("#tab" + primeiroVisivel).addClass("active show");
    }

}

function PreencherGridsDetalheOferta(ofertas) {
    _gridEquipamento.CarregarGrid(ofertas.Equipamento);
    _gridPorcentagem.CarregarGrid(ofertas.PorcentagemSobreNota);
    _gridFrotaFixaFranquia.CarregarGrid(ofertas.FrotaFixaFranquia);
    _gridFretePorPeso.CarregarGrid(ofertas.FretePorPeso);
    _gridFretePorCapacidade.CarregarGrid(ofertas.FretePorCapacidade);
    _gridFretePorViagem.CarregarGrid(ofertas.FretePorViagem);
    _gridFrotaFixaKm.CarregarGrid(ofertas.FrotaFixaKm);
    _gridViagemAdicional.CarregarGrid(ofertas.ViagemAdicional);
    _gridViagemEntregaAjudante.CarregarGrid(ofertas.ViagemEntregaAjudante);
}

function MostrarModalOfertaDetalhes() {
    Global.abrirModal('divModalOfertaDetalhe');
    $("#divModalOfertaDetalhe").on('hidden.bs.modal', function () {
        LimparCampos(_biddingOfertaDetalhes);
    });
}

function pesquisarResultados() {
    if (_gridResultados != undefined)
        _gridResultados.CarregarGrid();
}

function CalcularValorToneladas(pesoEmKg) {
    pesoEmKg = pesoEmKg.replace('.', '');
    let pesoEmKgAux = pesoEmKg.split(",");
    pesoEmKg = pesoEmKgAux[0];

    return pesoEmKg / 1000 + " TON";
}

function ControlarVisibilidadeVeiculosVerdes(permitirInformarVeiculosVerdes) {
    if (permitirInformarVeiculosVerdes) {
        _ofertasPai.InformarVeiculosVerdes.visible(true);
    } else {
        _ofertasPai.InformarVeiculosVerdes.visible(false);
    }
}

//#endregion