/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumTipoRotaSemParar.js" />
/// <reference path="../../Enumeradores/EnumPadraoTempoDiasMinutos.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Enumeradores/EnumTipoRotaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoUltimoPontoRoteirizacao.js" />
/// <reference path="Coleta.js" />
/// <reference path="Destinatario.js" />
/// <reference path="Empresa.js" />
/// <reference path="EmpresaExclusiva.js" />
/// <reference path="Localidades.js" />
/// <reference path="PontosPassagemPreDefinido.js" />
/// <reference path="Restricao.js" />
/// <reference path="RotaFreteIntegracao.js" />

// #region Objetos Globais do Arquivo

var _gridRotaFrete;
var _gridRotaFreteCEP;
var _rotaFrete;
var _pesquisaRotaFrete;
var _crudRotaFrete;
var _integracaoSemParar;
var _tipoIntegracoesDisponiveis = new Array();


// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaRotaFrete = function () {

    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-7 col-lg-7") });
    this.FilialDistribuidora = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.FilialDistribuidora.getFieldDescription(), visible: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), visible: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-2 col-lg-2") });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.SomenteGrupo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription(), required: false, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription(), required: false, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.TransportadorExclusivo.getFieldDescription(), required: false, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RotaExclusivaCompraValePedagio = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.RotaExclusivaCompraValePedagio, val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), def: "", visible: ko.observable(false) });
    this.CEPDestino = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.CEPDestino.getFieldDescription(), required: false, maxlength: 15, val: ko.observable("") });
    this.SituacaoRoteirizacao = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.SituacaoRoteirizacao.getFieldDescription(), val: ko.observable(EnumSituacaoRoteirizacao.Todas), options: EnumSituacaoRoteirizacao.obterOpcoes(), def: EnumSituacaoRoteirizacao.Todas });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRotaFrete.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var RotaFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), issue: 586, required: true, maxlength: 100 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.CodigoIntegracao.getFieldDescription(), maxlength: 50, issue: 15 });
    this.CodigoIntegracaoValePedagio = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.CodigoRotaValePedagio.getFieldDescription(), maxlength: 100, cssClass: ko.observable("col col-xs-12 col-md-6 col-lg-6") });
    this.CodigoIntegracaoValePedagioRetorno = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.CodigoRotaValePedagioRetorno.getFieldDescription(), maxlength: 100, visible: ko.observable(false) });
    this.CodigoIntegracaoGerenciadoraRisco = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.CodigoIntegracaoGerenciadoraRisco.getFieldDescription(), maxlength: 100, visible: ko.observable(false) });
    this.Quilometros = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.KMdaRota.getFieldDescription(), issue: 0, val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, maxlength: 10 });
    this.PermiteAgruparCargas = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Logistica.RotaFrete.ParaRotaPermitidoAgrupamentoCargas, issue: 835, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.VincularMotoristaFilaCarregamentoManualmente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.RotaFrete.VincularMotoristaFilaCarregamentoManualmente, def: false, visible: ko.observable(_CONFIGURACAO_TMS.UtilizarFilaCarregamento) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), issue: 557 });
    this.Detalhes = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.DetalhamentoDaRota.getFieldDescription(), issue: 0, maxlength: 10000, visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), issue: 0, maxlength: 300 });
    this.OrdenarLocalidades = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.OrdenarLocalidades, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.OrdenarLocalidades) });

    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid(), issue: 58, visible: ko.observable(false), required: false });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.Remetente.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });
    this.RemetenteOutroEndereco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.RemetenteOutroEndereco, idBtnSearch: guid(), visible: ko.observable(false), required: false });
    this.UsarOutroEnderecoRemetente = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.UsarOutroEnderecoRemetente, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.Distribuidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.Distribuidor.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false, eventChange: remetenteBlur });
    this.ExpedidorPedidosDiferenteOrigemRota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.Expedidor.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.TipoOperacao.getFieldDescription(), required: false, idBtnSearch: guid() });
    this.TipoOperacaoPreCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.TipoOperacaoPreCarga.getFieldDescription(), required: false, idBtnSearch: guid() });
    this.Classificacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.Classificacao.getFieldDescription(), required: false, idBtnSearch: guid() });
    this.LocalidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Origem, idBtnSearch: guid(), issue: 16, required: false });
    this.RegiaoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.RegiaoDeDestino, idBtnSearch: guid(), issue: 110, required: false });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.CanalEntrega, idBtnSearch: guid(), required: false });
    this.CanalVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.CanalVenda, idBtnSearch: guid(), required: false });
    this.FilialDistribuidora = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.FilialDistribuidora.getFieldDescription(), required: false, maxlength: 100, visible: ko.observable(true) });

    this.Estados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.EstadosOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Destinatarios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Fronteiras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Coletas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.PostosFiscais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.PontosPassagemPreDefinido = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.PracaPedagios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.CEPs = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Restricoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.Veiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TipoCargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.RotaRoteirizada = PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false) });
    this.VelocidadeMediaCarregado = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.VelocidadeMediaCarregado.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 3 });
    this.PadraoTempo = PropertyEntity({ val: ko.observable(EnumPadraoTempoDiasMinutos.Minutos), options: EnumPadraoTempoDiasMinutos.obterOpcoes(), def: EnumPadraoTempoDiasMinutos.Minutos, text: Localization.Resources.Logistica.RotaFrete.PadraoTempo });
    this.VelocidadeMediaVazio = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.VelocidadeMediaVazio.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 3 });

    this.TempoDeViagemEmMinutos = PropertyEntity({ val: ko.observable(true), text: ko.observable((_CONFIGURACAO_TMS.NaoCalcularTempoDeViagemAutomatico ? "*" : "") + Localization.Resources.Logistica.RotaFrete.TempoDeViagemMinutos.getFieldDescription()), issue: 834, required: ko.observable(_CONFIGURACAO_TMS.NaoCalcularTempoDeViagemAutomatico), visible: ko.observable(true) });

    this.TempoDeViagemEmDias = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.TempoDeViagemDias.getFieldDescription(), issue: 834, val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 2, visible: ko.observable(false) });
    this.GerarRedespachoAutomaticamente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Logistica.RotaFrete.GerarAutomaticamenteCargaRedespachoRota, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.RotaExclusivaCompraValePedagio = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.RotaExclusivaCompraDeValePedagio, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.UtilizarDistanciaRotaCarga = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.UtilizarDistanciaRotaCarga, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.VoltarPeloMesmoCaminhoIda = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.VoltarPeloMesmoCaminhoIda, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });

    this.RestricaoEntrega = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Logistica.RotaFrete.RestricaoDeEntrega.getFieldDescription(), val: ko.observable(new Array()), def: new Array(), options: EnumDiaSemana.obterOpcoes(), visible: ko.observable(false) });
    this.TipoUltimoPontoRoteirizacaoPorEstado = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.UltimoPontoPorEstado.getFieldDescription(), val: ko.observable(EnumTipoUltimoPontoRoteirizacao.Todos), options: EnumTipoUltimoPontoRoteirizacao.obterOpcoesNaoSelecionado(), def: EnumTipoUltimoPontoRoteirizacao.Todos, issue: 1292, visible: ko.observable(false) });

    this.PadraoTempo.val.subscribe(function (valor) {
        if (valor == EnumPadraoTempoDiasMinutos.Minutos) {
            _rotaFrete.TempoDeViagemEmMinutos.visible(true);
            _rotaFrete.TempoDeViagemEmDias.visible(false);
        }
        else if (valor == EnumPadraoTempoDiasMinutos.Dias) {
            _rotaFrete.TempoDeViagemEmDias.visible(true);
            _rotaFrete.TempoDeViagemEmMinutos.visible(false);
        }
    });

    this.RotaExclusivaCompraValePedagio.val.subscribe(function (valor) {
        if (_crudMapaPontosDePassagem != null) {
            _crudMapaPontosDePassagem.Localidade.visible(valor)
        }

        if (valor) {
            $("#liTabFiliais").show();
            $("#liTabtipoCargas").show();
        } else {
            $("#liTabFiliais").hide();
            $("#liTabtipoCargas").hide();
        }

    });

    this.TipoRota = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.TipoDeRotaEixosSuspensos.getFieldDescription(), options: EnumTipoRotaFrete.ObterOpcoes(), val: ko.observable(EnumTipoRotaFrete.Ida), def: EnumTipoRotaFrete.Ida });
    this.TipoCarregamentoIda = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.TipoCarregamentoIda.getFieldDescription(), options: EnumRetornoCargaTipo.obterOpcoes(), val: ko.observable(EnumRetornoCargaTipo.Carregado), def: EnumRetornoCargaTipo.Carregado, issue: 0, visible: ko.observable(true) });
    this.TipoCarregamentoVolta = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.TipoCarregamentoVolta.getFieldDescription(), options: EnumRetornoCargaTipo.obterOpcoes(), val: ko.observable(EnumRetornoCargaTipo.Carregado), def: EnumRetornoCargaTipo.Carregado, issue: 0, visible: ko.observable(false) });
    this.HoraInicioCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.HoraInicioDoCarregamento.getFieldDescription(), getType: typesKnockout.time });
    this.HoraLimiteSaidaCD = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.HoraLimiteParaSaidaCD.getFieldDescription(), getType: typesKnockout.time });

    this.TempoCarregamento = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.RotaFrete.TempoDeCarregamento.getFieldDescription()), getType: typesKnockout.mask, mask: "000:00", visible: ko.observable(true), required: ko.observable(false) });
    this.TempoDescarga = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.RotaFrete.TempoDeDescarga.getFieldDescription()), getType: typesKnockout.mask, mask: "000:00", visible: ko.observable(true), required: ko.observable(false) });

    this.PontosDaRota = PropertyEntity({});
    this.PolilinhaRota = PropertyEntity({});
    this.SituacaoDaRoteirizacao = PropertyEntity({});
    this.MotivoFalhaRoteirizacao = PropertyEntity({});
    this.ApenasObterPracasPedagio = PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false) });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        this.Quilometros.getType = typesKnockout.int;
        this.Quilometros.def = "0";
    }

    this.TipoOperacao.codEntity.subscribe(function (valor) {
        _roteirizador.TipoUltimoPontoRoteirizacao.enable(valor == 0);
    });

    this.TipoRota.val.subscribe(function (tipoRota) {
        _rotaFrete.TipoCarregamentoIda.visible(tipoRota === EnumTipoRotaFrete.Ida || tipoRota === EnumTipoRotaFrete.IdaVolta);
        _rotaFrete.TipoCarregamentoVolta.visible(tipoRota === EnumTipoRotaFrete.IdaVolta);
    });

    this.Remetente.codEntity.subscribe(function (valor) {
        if (valor == 0) {
            _rotaFrete.UsarOutroEnderecoRemetente.visible(false);
            _rotaFrete.RemetenteOutroEndereco.visible(false);

            _rotaFrete.RemetenteOutroEndereco.codEntity(0);
            _rotaFrete.RemetenteOutroEndereco.val("");
            _rotaFrete.UsarOutroEnderecoRemetente.val(false);
            clienteOrigem = null;
        }
    });

    this.LocalidadeOrigem.codEntity.subscribe(function (valor) {
        if (valor == 0) {
            _rotaFreteOrigem = null;
        }
    });

    this.ExpedidorPedidosDiferenteOrigemRota.codEntity.subscribe(function (valor) {
        if (valor == 0) {
            _rotaFreteExpedidor = null;
        }
    });

};

var CRUDRotaFrete = function () {

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "RotaFrete/Importar",
        UrlConfiguracao: "RotaFrete/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O028_RotaFrete,
        CallbackImportacao: function () {
            _gridRotaFrete.CarregarGrid();
        }
    });

    this.ImportarShare = PropertyEntity({
        type: types.local,
        text: "Importar Share",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "RotaFrete/ImportarShare",
        UrlConfiguracao: "RotaFrete/ConfiguracaoImportacaoShare",
        CodigoControleImportacao: EnumCodigoControleImportacao.O028_RotaFrete,
        CallbackImportacao: function () {
            _gridRotaFrete.CarregarGrid();
        }
    });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.BuscarPracas = PropertyEntity({ eventClick: BuscarPracasClick, type: types.event, text: Localization.Resources.Logistica.RotaFrete.BuscarPracas, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function LoadRotaFrete() {
    _rotaFrete = new RotaFrete();
    KoBindings(_rotaFrete, "knockoutDetalhes");

    _crudRotaFrete = new CRUDRotaFrete();
    KoBindings(_crudRotaFrete, "knockoutCRUDRotaFrete");

    _pesquisaRotaFrete = new PesquisaRotaFrete();
    KoBindings(_pesquisaRotaFrete, "knockoutPesquisaRotaFrete", false, _pesquisaRotaFrete.Pesquisar.id);

    HeaderAuditoria("RotaFrete", _rotaFrete);

    BuscarGruposPessoas(_pesquisaRotaFrete.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    BuscarClientes(_pesquisaRotaFrete.Remetente);
    BuscarClientes(_pesquisaRotaFrete.Destinatario);
    BuscarLocalidades(_pesquisaRotaFrete.Origem, Localization.Resources.Gerais.Geral.BuscarCidadeOrigem, Localization.Resources.Gerais.Geral.CidadeOrigem);
    BuscarLocalidades(_pesquisaRotaFrete.Destino, Localization.Resources.Logistica.RotaFrete.BuscarCidadesDestino, Localization.Resources.Logistica.RotaFrete.CidadesDeDestino);
    BuscarTransportadores(_pesquisaRotaFrete.Transportador);

    BuscarLocalidades(_rotaFrete.LocalidadeOrigem, null, null, callbackLocalidadeOrigem);
    BuscarRegioes(_rotaFrete.RegiaoDestino);
    BuscarCanaisEntrega(_rotaFrete.CanalEntrega);
    BuscarCanaisVenda(_rotaFrete.CanalVenda);
    BuscarRotaFreteClassificacao(_rotaFrete.Classificacao);
    BuscarClientes(_rotaFrete.Remetente, callbackConsultaRemetente);
    BuscarClientes(_rotaFrete.Distribuidor);
    BuscarClientes(_rotaFrete.ExpedidorPedidosDiferenteOrigemRota, callbackExpedidorPedidosDiferenteOrigemRota);
    BuscarGruposPessoas(_rotaFrete.GrupoPessoas, callbackConsultarGrupoPessoas, null, null, EnumTipoGrupoPessoas.Clientes);
    BuscarTiposOperacao(_rotaFrete.TipoOperacaoPreCarga);
    BuscarTiposOperacao(_rotaFrete.TipoOperacao, callbackConsultarTipoOperacao);

    BuscarClienteOutroEndereco(_rotaFrete.RemetenteOutroEndereco, null, _rotaFrete.Remetente);

    $("#liTabRotaCEP").hide();

    if (_ConfiguracaoControleEntrega.ObrigatorioInformarFreetime) {
        _rotaFrete.TempoCarregamento.required(true);
        _rotaFrete.TempoCarregamento.text("*" + _rotaFrete.TempoCarregamento.text());
        _rotaFrete.TempoDescarga.required(true);
        _rotaFrete.TempoDescarga.text("*" + _rotaFrete.TempoDescarga.text());
    }

    ConfigurarLayoutPorTipoServico();

    BuscarRotaFretes();
    loadRoteirizacao();
    LoadEstadoDestino();
    LoadEstadoOrigem();
    loadLocalidade();
    loadRotaCEP();
    LoadDestinatarioRotaFrete();
    LoadFronteiraRotaFrete();
    LoadColetaRotaFrete();
    LoadPontosPassagemPreDefinidoRotaFrete();
    LoadPostoFiscal();
    loadRestricao();
    loadEmpresa();
    loadEmpresaExclusiva();
    //loadGeoLocalizacao();
    LoadPracaPedagioRotaFrete();
    LoadVeiculoRotaFrete();
    LoadFilialRotaFrete();
    LoadTipoCargaRotaFrete();
    ConfigurarIntegracoesDisponiveis();
    ConfigurarIntegracaoSemParar();
    ConfigurarIntegracaoValePedagio();

    LimparCampos(_rotaFrete);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _rotaFrete.VoltarPeloMesmoCaminhoIda.visible(true);
    }
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function AdicionarClick() {
    var rotaFrete = obterRotaFreteSalvar();

    if (!rotaFrete)
        return;

    executarReST("RotaFrete/Adicionar", rotaFrete, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridRotaFrete.CarregarGrid();
                LimparCamposRotaFrete();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function AtualizarClick() {
    var rotaFrete = obterRotaFreteSalvar();

    if (!rotaFrete)
        return;

    executarReST("RotaFrete/Atualizar", rotaFrete, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridRotaFrete.CarregarGrid();
                LimparCamposRotaFrete();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function CancelarClick(e) {
    LimparCamposRotaFrete();
}

function ExcluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, (Localization.Resources.Logistica.RotaFrete.DesejaRealmenteExcluirRota + " " + _rotaFrete.Descricao.val() + "?"), function () {
        ExcluirPorCodigo(_rotaFrete, "RotaFrete/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridRotaFrete.CarregarGrid();
                    LimparCamposRotaFrete();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function remetenteBlur() {
    if (_rotaFrete.Remetente.val() == "") {
        clienteOrigem = null;
        _rotaFrete.Remetente.codEntity(0);
    }
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function CarregarRotaFreteCEP() {
    var editarCEP = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: EditarCEPClick, tamanho: "15", icone: "" };
    var excluirCEP = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: ExcluirCEPClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editarCEP, excluirCEP] };

    _gridRotaFreteCEP = new GridView(_cep.CEPs.idGrid, "RotaFrete/PesquisaCEP", _rotaFrete, menuOpcoes, null);
    _gridRotaFreteCEP.CarregarGrid();
}

function ResetarTabs() {
    $("#tabRotaFrete a:first").tab("show");
    $("#ulRotaFreteEmpresa a:first").tab("show");
}

// #endregion Funções Públicas

// #region Funções Privadas

function BuscarRotaFretes() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: function (itemGrid) { EditarRotaFrete(itemGrid, false); }, tamanho: "15", icone: "" };
    var duplicar = { descricao: Localization.Resources.Gerais.Geral.Duplicar, id: guid(), evento: "onclick", metodo: function (itemGrid) { EditarRotaFrete(itemGrid, true); }, tamanho: "15", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, duplicar], tamanho: "10" };

    _gridRotaFrete = new GridView(_pesquisaRotaFrete.Pesquisar.idGrid, "RotaFrete/Pesquisa", _pesquisaRotaFrete, menuOpcoes, null, null, null, null, null, null, 10);
    _gridRotaFrete.CarregarGrid();
}

function callbackConsultaRemetente(dados) {
    clienteOrigem = dados;
    _rotaFrete.Remetente.codEntity(dados.Codigo);
    _rotaFrete.Remetente.val(dados.Descricao);

    if (dados.TempoCarregamento != "")
        _rotaFrete.TempoCarregamento.val(dados.TempoCarregamento);

    if (dados.TempoDescarregamento != "")
        _rotaFrete.TempoDescarga.val(dados.TempoDescarregamento);

    console.log(dados);

    if (dados.PossuiOutrosEnderecos) {
        _rotaFrete.UsarOutroEnderecoRemetente.visible(true);
        _rotaFrete.RemetenteOutroEndereco.visible(true);
    }
}

function callbackLocalidadeOrigem(dados) {
    _rotaFreteOrigem = dados;
    _rotaFrete.LocalidadeOrigem.codEntity(dados.Codigo);
    _rotaFrete.LocalidadeOrigem.val(dados.Descricao);
    _rotaFreteOrigem.UtilizaLocalidade = true;
}

function callbackExpedidorPedidosDiferenteOrigemRota(dados) {
    _rotaFreteExpedidor = dados;
    _rotaFrete.ExpedidorPedidosDiferenteOrigemRota.codEntity(dados.Codigo);
    _rotaFrete.ExpedidorPedidosDiferenteOrigemRota.val(dados.Descricao);
}

function callbackConsultarGrupoPessoas(dados) {
    _rotaFrete.GrupoPessoas.codEntity(dados.Codigo);
    _rotaFrete.GrupoPessoas.val(dados.Descricao);

    if (dados.TempoCarregamento != "")
        _rotaFrete.TempoCarregamento.val(dados.TempoCarregamento);

    if (dados.TempoDescarregamento != "")
        _rotaFrete.TempoDescarga.val(dados.TempoDescarregamento);
}

function callbackConsultarTipoOperacao(dados) {
    _rotaFrete.TipoOperacao.codEntity(dados.Codigo);
    _rotaFrete.TipoOperacao.val(dados.Descricao);
    _roteirizador.TipoUltimoPontoRoteirizacao.val(dados.TipoUltimoPontoRoteirizacao);

    if (dados.TempoCarregamento != "")
        _rotaFrete.TempoCarregamento.val(dados.TempoCarregamento);

    if (dados.TempoDescarregamento != "")
        _rotaFrete.TempoDescarga.val(dados.TempoDescarregamento);
}

function ConfigurarIntegracaoSemParar() {
    executarReST("ConfiguracaoSemParar/ObterConfiguracao", {}, function (r) {
        if (r.Success && r.Data) {

            if (r.Data.TipoRotaSemParar === EnumTipoRotaSemParar.RotaTemporaria) {
                _integracaoSemParar = true;
                $("#liTabPracaPedagio").removeClass("d-none");
            }

            if (r.Data.TipoRotaSemParar === EnumTipoRotaSemParar.RotaFixa) {
                _integracaoSemParar = false;
            }
        }
    });
}

function ConfigurarIntegracoesDisponiveis() {
    _integracaoSemParar = false;
    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (r) {
        if (r.Success && r.Data) {
            if (r.Data.TiposExistentes != null && r.Data.TiposExistentes.length > 0) {
                _tipoIntegracoesDisponiveis = r.Data.TiposExistentes;

                if (_tipoIntegracoesDisponiveis.some(function (o) { return o == EnumTipoIntegracao.NOX; }) && r.Data.PossuiIntegracaoNOX) {
                    LoadConfiguracaoNOX();
                    $("#liTabNOX").removeClass("d-none");
                }

                if (_tipoIntegracoesDisponiveis.some(function (o) { return o == EnumTipoIntegracao.Trafegus || o == EnumTipoIntegracao.BrasilRisk; })) {
                    _rotaFrete.CodigoIntegracaoGerenciadoraRisco.visible(true);
                }

                if (_tipoIntegracoesDisponiveis.some(function (o) { return o == EnumTipoIntegracao.SemParar; })) {
                    _rotaFrete.CodigoIntegracaoValePedagio.cssClass("col col-xs-12 col-md-6 col-lg-3");
                    _rotaFrete.CodigoIntegracaoValePedagioRetorno.visible(true);
                    VisibilidadeRotaExclusivaCompraValePedagio();
                    ConfigurarIntegracaoSemParar();
                }

                //todo: tratar para carregar o mapa apenas nas integrações corretas.
                if (_tipoIntegracoesDisponiveis.some(function (o) { return (o == EnumTipoIntegracao.AngelLira || o == EnumTipoIntegracao.APIGoogle); })) {
                    loadMapaGoogle();
                    $("#liRoteirizacao").removeClass("d-none");
                }
            }
        }
    });
}

function ConfigurarIntegracaoValePedagio() {
    executarReST("RotaFrete/ObterConfiguracao", {}, function (r) {
        if (r.Success && r.Data) {
            if (r.Data.TemIntegracaoRepomRest)
                VisibilidadeRotaExclusivaCompraValePedagio();
        }
    });
}

function VisibilidadeRotaExclusivaCompraValePedagio() {
    _rotaFrete.RotaExclusivaCompraValePedagio.visible(true);
    _pesquisaRotaFrete.RotaExclusivaCompraValePedagio.visible(true);
}

function ControleVisibilidadeAbaRoteirizacao() {

    if (!PossuiIntegracoesParaRoteirizacao()) {
        if (_CONFIGURACAO_TMS.ExigirRotaRoteirizadaNaCarga)
            $("#liRoteirizacao").removeClass("d-none");

        return;
    }

    if (_estadoDestino.Estado.basicTable.BuscarRegistros().length > 0 || _destinatarioRotaFrete.ValidarParaQualquerDestinatarioInformado.val())
        $("#liRoteirizacao").addClass("d-none");
    else
        $("#liRoteirizacao").removeClass("d-none");
}

function PossuiIntegracoesParaRoteirizacao() {
    return _tipoIntegracoesDisponiveis.some(function (o) { return (o == EnumTipoIntegracao.AngelLira || o == EnumTipoIntegracao.APIGoogle); });
}

function ConfigurarLayoutPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaRotaFrete.GrupoPessoas.visible(true);
        _rotaFrete.GrupoPessoas.visible(true);
        _rotaFrete.Detalhes.visible(true);
        _rotaFrete.Distribuidor.visible(false);
        _rotaFrete.ExpedidorPedidosDiferenteOrigemRota.visible(false);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _rotaFrete.FilialDistribuidora.visible(false);
        _pesquisaRotaFrete.FilialDistribuidora.visible(false);
        _pesquisaRotaFrete.Descricao.cssClass("col col-xs-12 col-sm-12 col-md-10 col-lg-10");
        _pesquisaRotaFrete.Origem.visible(true);
        _pesquisaRotaFrete.Remetente.visible(true);
        _pesquisaRotaFrete.Destino.visible(true);

        $("#liTabEmpresa").show();
    }
}

function EditarRotaFrete(rotaFreteGrid, duplicar) {
    LimparCamposRotaFrete();

    executarReST("RotaFrete/BuscarPorCodigo", { Codigo: rotaFreteGrid.Codigo, Duplicar: duplicar }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_rotaFrete, retorno);

                _pesquisaRotaFrete.ExibirFiltros.visibleFade(false);

                _crudRotaFrete.Atualizar.visible(!duplicar);
                _crudRotaFrete.Cancelar.visible(!duplicar);
                _crudRotaFrete.Excluir.visible(!duplicar);
                _crudRotaFrete.Adicionar.visible(duplicar);
                _crudRotaFrete.BuscarPracas.visible(duplicar);
                _crudRotaFrete.ImportarShare.visible(false);
                _rotaFrete.TempoDeViagemEmMinutos.required(true);
                _rotaFrete.TempoDeViagemEmMinutos.text(Localization.Resources.Logistica.RotaFrete.TempoDeViagemMinutos.getFieldDescription());
                _pracaPedagioRotaFrete.HistoricoIntegracao.visible(true);

                clienteOrigem = retorno.Data.Remetente;
                _rotaFreteOrigem = retorno.Data.LocalidadeOrigem;
                _rotaFreteExpedidor = retorno.Data.ExpedidorPedidosDiferenteOrigemRota;

                RecarregarGridEstadoDestino();
                RecarregarGridEstadoOrigem();
                RecarregarGridDestinatarioRotaFrete();
                RecarregarGridFronteiraRotaFrete();
                RecarregarGridColetaRotaFrete();
                RecarregarGridPracaPedagioRotaFrete();
                RecarregarGridPontosPassagemPreDefinidoRotaFrete();
                RecarregarGridPostoFiscal();
                RecarregarGridVeiculoRotaFrete();
                RecarregarGridFilialRotaFrete();
                RecarregarGridTipoCargaRotaFrete();
                preencherLocalidade(retorno.Data.DadosLocalidades);
                preencherRestricao(retorno.Data.Restricoes);
                preencherEmpresa(retorno.Data.Empresas);
                preencherEmpresaExclusiva(retorno.Data.EmpresasExclusivas);

                if (retorno.Data.UsarOutroEnderecoRemetente) {
                    _rotaFrete.UsarOutroEnderecoRemetente.visible(true);
                    _rotaFrete.RemetenteOutroEndereco.visible(true);
                }

                if (!duplicar) {
                    _gridRotaFreteCEP.CarregarGrid();

                    $("#liTabRotaCEP").show();
                }
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, null);
}

function LimparCamposRotaFrete() {
    _crudRotaFrete.Atualizar.visible(false);
    _crudRotaFrete.Cancelar.visible(false);
    _crudRotaFrete.Excluir.visible(false);
    _crudRotaFrete.Adicionar.visible(true);
    _crudRotaFrete.BuscarPracas.visible(false);
    _crudRotaFrete.ImportarShare.visible(true);
    _pracaPedagioRotaFrete.HistoricoIntegracao.visible(false);
    clienteOrigem = null;
    _rotaFreteOrigem = null;
    _rotaFreteExpedidor = null;
    LimparCampos(_rotaFrete);
    LimparCamposEstadoDestino();
    LimparCamposEstadoOrigem();
    LimparCamposDestinatarioRotaFrete();
    LimparCamposFronteiraRotaFrete();
    LimparCamposPracaPedagioRotaFrete();
    LimparCamposVeiculoRotaFrete();
    LimparCamposTipoCargaRotaFrete();
    LimparCamposFilialRotaFrete();
    limparCEP();

    RecarregarGridEstadoDestino();
    RecarregarGridEstadoOrigem();
    RecarregarGridDestinatarioRotaFrete();
    RecarregarGridFronteiraRotaFrete();
    RecarregarGridColetaRotaFrete();
    RecarregarGridPontosPassagemPreDefinidoRotaFrete();
    RecarregarGridPostoFiscal();
    RecarregarGridPracaPedagioRotaFrete();
    RecarregarGridVeiculoRotaFrete();
    RecarregarGridFilialRotaFrete();
    RecarregarGridTipoCargaRotaFrete();
    limparCamposLocalidade();
    limparCamposRestricao();
    limparCamposEmpresa();
    limparCamposEmpresaExclusiva();
    limparMapa();
    buscarZonasExclusaoRota();
    _gridRotaFreteCEP.CarregarGrid();

    $("#liTabRotaCEP").hide();

    ResetarTabs();
}

function obterRotaFreteSalvar() {
    _rotaFrete.Estados.val(JSON.stringify(_estadoDestino.Estado.basicTable.BuscarRegistros()));
    _rotaFrete.EstadosOrigem.val(JSON.stringify(_estadoOrigem.Estado.basicTable.BuscarRegistros()));
    _rotaFrete.Destinatarios.val(JSON.stringify(_destinatarioRotaFrete.Destinatario.basicTable.BuscarRegistros()));
    _rotaFrete.Fronteiras.val(JSON.stringify(_fronteiraRotaFrete.Fronteira.basicTable.BuscarRegistros()));
    _rotaFrete.Coletas.val(JSON.stringify(_coletaRotaFrete.Coleta.basicTable.BuscarRegistros()));
    _rotaFrete.PontosPassagemPreDefinido.val(JSON.stringify(_pontosPassagemPreDefinidoRotaFrete.PontosPassagemPreDefinido.basicTable.BuscarRegistros()));
    _rotaFrete.PostosFiscais.val(JSON.stringify(_postoFiscal.PostosFiscais.basicTable.BuscarRegistros()));
    _rotaFrete.PracaPedagios.val(JSON.stringify(_pracaPedagioIdaVolta.Pracas.basicTable.BuscarRegistros()));
    _rotaFrete.Veiculos.val(JSON.stringify(_veiculoRotaFrete.Veiculo.basicTable.BuscarRegistros()));
    _rotaFrete.Filiais.val(JSON.stringify(_FilialRotaFrete.Filial.basicTable.BuscarRegistros()));
    _rotaFrete.TipoCargas.val(JSON.stringify(_TipoCargaRotaFrete.TipoCarga.basicTable.BuscarRegistros()));


    if (_rotaFrete.TempoCarregamento.val() == "000:00")
        _rotaFrete.TempoCarregamento.val("");

    if (_rotaFrete.TempoDescarga.val() == "000:00")
        _rotaFrete.TempoDescarga.val("");

    if (!ValidarCamposObrigatorios(_rotaFrete)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        ResetarTabs();
        return undefined;
    }

    var rotaFrete = RetornarObjetoPesquisa(_rotaFrete);

    preencherLocalidadeSalvar(rotaFrete);
    preencherRestricaoSalvar(rotaFrete);
    preencherEmpresaSalvar(rotaFrete);
    preencherEmpresaExclusivaSalvar(rotaFrete);

    return rotaFrete;
}

function ProcessarRotasAntigas() {
    executarReST("RotaFrete/ProcessarRotasAntigas", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Logistica.RotaFrete.Processou);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}


// #endregion Funções Privadas