/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoOcorrencia.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="ComponenteFrete.js" />
/// <reference path="DiasAbertura.js" />
/// <reference path="FilialEmissao.js" />
/// <reference path="TipoOcorrencia.js" />
/// <reference path="TipoOperacao.js" />
/// <reference path="TomadorOcorrencia.js" />
/// <reference path="ValorOcorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraOcorrencia;
var _gridAprovadores;
var _regraOcorrencia;
var _pesquisaRegraOcorrencia;

var _configRegras = {
    Aprovadores: 3,
    infoTable: "Mova as linhas conforme a prioridade"
};

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _condicaoAutorizaoOcorrenciaValor = [
    { text: EnumCondicaoAutorizaoOcorrencia.obterDescricao(EnumCondicaoAutorizaoOcorrencia.IgualA), value: EnumCondicaoAutorizaoOcorrencia.IgualA },
    { text: EnumCondicaoAutorizaoOcorrencia.obterDescricao(EnumCondicaoAutorizaoOcorrencia.DiferenteDe), value: EnumCondicaoAutorizaoOcorrencia.DiferenteDe },
    { text: EnumCondicaoAutorizaoOcorrencia.obterDescricao(EnumCondicaoAutorizaoOcorrencia.MaiorIgualQue), value: EnumCondicaoAutorizaoOcorrencia.MaiorIgualQue },
    { text: EnumCondicaoAutorizaoOcorrencia.obterDescricao(EnumCondicaoAutorizaoOcorrencia.MaiorQue), value: EnumCondicaoAutorizaoOcorrencia.MaiorQue },
    { text: EnumCondicaoAutorizaoOcorrencia.obterDescricao(EnumCondicaoAutorizaoOcorrencia.MenorIgualQue), value: EnumCondicaoAutorizaoOcorrencia.MenorIgualQue },
    { text: EnumCondicaoAutorizaoOcorrencia.obterDescricao(EnumCondicaoAutorizaoOcorrencia.MenorQue), value: EnumCondicaoAutorizaoOcorrencia.MenorQue }
];

var _condicaoAutorizaoOcorrenciaEntidade = [
    { text: EnumCondicaoAutorizaoOcorrencia.obterDescricao(EnumCondicaoAutorizaoOcorrencia.IgualA), value: EnumCondicaoAutorizaoOcorrencia.IgualA },
    { text: EnumCondicaoAutorizaoOcorrencia.obterDescricao(EnumCondicaoAutorizaoOcorrencia.DiferenteDe), value: EnumCondicaoAutorizaoOcorrencia.DiferenteDe }
];

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _juncaoAutorizaoOcorrencia = EnumJuncaoAutorizaoOcorrencia.obterOpcoes();
  


var PesquisaRegraOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.DataInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.DataLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Situacao.getFieldDescription()});
    this.Descricao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Descricao.getFieldDescription(), issue: 586, val: ko.observable(""), def: "" });
    this.EtapaAutorizacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.EtapaAutorizacao.getFieldDescription(), issue: 908, val: ko.observable(EnumEtapaAutorizacaoOcorrencia.Todas), options: EnumEtapaAutorizacaoOcorrencia.obterOpcoesPesquisa(), def: EnumEtapaAutorizacaoOcorrencia.Todas });
    this.Aprovador = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Aprovador.getFieldDescription(), issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOcorrencia.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraOcorrencia.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var RegraOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Descricao.getRequiredFieldDescription(), issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Vigencia.getFieldDescription(), issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: (_CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas ? Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.NumeroAprovadores.getRequiredFieldDescription() : Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.NumeroAprovadores.getFieldDescription()), issue: 873, getType: typesKnockout.int, required: _CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas, maxlength: 3 });
    this.NumeroReprovadores = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.NumeroReprovadores.getRequiredFieldDescription(), getType: typesKnockout.int, required: true, maxlength: 3 });
    this.DiasPrazoAprovacao = PropertyEntity({ text:Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.PrazoAprovacaoDias.getFieldDescription(), getType: typesKnockout.int });
    this.AprovacaoAutomaticaAposDias = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.PrazoParAprovacaoAutomaticaDas.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Observacao.getFieldDescription(), issue: 593, maxlength: 2000 });
    this.EtapaAutorizacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.EtapaAutorizacao.getFieldDescription(), issue: 908, val: ko.observable(EnumEtapaAutorizacaoOcorrencia.AprovacaoOcorrencia), options: EnumEtapaAutorizacaoOcorrencia.obterOpcoesDuas(), def: EnumEtapaAutorizacaoOcorrencia.AprovacaoOcorrencia });
    this.TipoDiasAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoDias.getFieldDescription(), issue: 908, val: ko.observable(EnumTipoDiasAprovacao.DiasCorridos), options: EnumTipoDiasAprovacao.obterOpcoes(), def: EnumTipoDiasAprovacao.DiasCorridos, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Situacao.getRequiredFieldDescription(), issue: 557 });
    this.PrioridadeAprovacao = PropertyEntity({ val: ko.observable(EnumPrioridadeAutorizacao.Zero), options: EnumPrioridadeAutorizacao.obterOpcoes(), def: EnumPrioridadeAutorizacao.Zero, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Prioridade.getRequiredFieldDescription() });
    this.EnviarLinkParaAprovacaoPorEmail = PropertyEntity({ val: ko.observable(false), def: false, text: "Enviar Link para aprovação por e-mail" });

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridAprovadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Adicionar, idBtnSearch: guid(), idGrid: guid() });
    this.GridAprovadores.val.subscribe(function () {
        _regraOcorrencia.Aprovadores.val(JSON.stringify(_regraOcorrencia.GridAprovadores.val()))
        RenderizarGridAprovadores();
    });

    // Regras
    this.UsarRegraPorTipoOcorrencia = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTipoOcorrencia = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorComponenteFrete = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasComponenteFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorFilialEmissao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasFilialEmissao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTomadorOcorrencia = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTomadorOcorrencia = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorValorOcorrencia = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasValorOcorrencia = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorDiasAbertura = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasDiasAbertura = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorExpedidor = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasExpedidor = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorCanalEntrega = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasCanalEntrega = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTipoCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTipoCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });
}

//*******EVENTOS*******

function loadRegrasAutorizacaoOcorrencia() {
    _regraOcorrencia = new RegraOcorrencia();
    KoBindings(_regraOcorrencia, "knockoutCadastroRegraOcorrencia");

    _pesquisaRegraOcorrencia = new PesquisaRegraOcorrencia();
    KoBindings(_pesquisaRegraOcorrencia, "knockoutPesquisaRegraOcorrencia", false, _pesquisaRegraOcorrencia.Pesquisar.id);

    HeaderAuditoria("RegrasAutorizacaoOcorrencia", _regraOcorrencia);

    //-- Grid Aprovadores
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Excluir,
                id: guid(),
                evento: "onclick",
                tamanho: "15",
                icone: "",
                metodo: function (data) {
                    RemoverAprovadorClick(_regraOcorrencia.GridAprovadores, data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Usuario, width: "100%", className: "text-align-left" }
    ];

    // Grid
    _gridAprovadores = new BasicDataTable(_regraOcorrencia.GridAprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridAprovadores);
    _gridAprovadores.CarregarGrid([]);



    //-- Pesquisa
    new BuscarFuncionario(_regraOcorrencia.GridAprovadores, RetornoInserirAprovador);
    new BuscarFuncionario(_pesquisaRegraOcorrencia.Aprovador);
    new BuscarTipoOcorrencia(_pesquisaRegraOcorrencia.TipoOcorrencia);

    //-- Carrega os loads
    loadCRUDRegras();
    loadTipoOcorrencia();
    loadComponenteFrete();
    loadFilialEmissao();
    loadExpedidor();
    loadTipoCarga();
    loadCanalEntrega();
    loadTomadorOcorrencia();
    loadValorOcorrencia();
    loadTipoOperacao();
    loadDiasAbertura();

    //-- Busca Regras
    buscarRegrasAutorizacaoOcorrencia();
}

function buscarRegrasAutorizacaoOcorrencia() {
    var editar = { descricao: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Editar, id: "clasEditar", evento: "onclick", metodo: editarRegrasOcorrencia, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraOcorrencia = new GridView(_pesquisaRegraOcorrencia.Pesquisar.idGrid, "RegrasAutorizacaoOcorrencia/Pesquisa", _pesquisaRegraOcorrencia, menuOpcoes);
    _gridRegraOcorrencia.CarregarGrid();
}

function editarRegrasOcorrencia(data) {
    LimparTodosCampos();

    _regraOcorrencia.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraOcorrencia, "RegrasAutorizacaoOcorrencia/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraOcorrencia.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraOcorrencia.GridAprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        _tipoOcorrencia.Regras.val(arg.Data.TipoOcorrencia);
        _tipoOcorrencia.UsarRegraPorTipoOcorrencia.val(arg.Data.UsarRegraPorTipoOcorrencia);

        _componenteFrete.Regras.val(arg.Data.ComponenteFrete);
        _componenteFrete.UsarRegraPorComponenteFrete.val(arg.Data.UsarRegraPorComponenteFrete);

        _filialEmissao.Regras.val(arg.Data.FilialEmissao);
        _filialEmissao.UsarRegraPorFilialEmissao.val(arg.Data.UsarRegraPorFilialEmissao);

        _tomadorOcorrencia.Regras.val(arg.Data.TomadorOcorrencia);
        _tomadorOcorrencia.UsarRegraPorTomadorOcorrencia.val(arg.Data.UsarRegraPorTomadorOcorrencia);

        _valorOcorrencia.Regras.val(arg.Data.ValorOcorrencia);
        _valorOcorrencia.UsarRegraPorValorOcorrencia.val(arg.Data.UsarRegraPorValorOcorrencia);

        _tipoOperacao.Regras.val(arg.Data.TipoOperacao);
        _tipoOperacao.UsarRegraPorTipoOperacao.val(arg.Data.UsarRegraPorTipoOperacao);

        _diasAbertura.Regras.val(arg.Data.DiasAbertura);
        _diasAbertura.UsarRegraPorDiasAbertura.val(arg.Data.UsarRegraPorDiasAbertura);

        _expedidor.Regras.val(arg.Data.Expedidor);
        _expedidor.UsarRegraPorExpedidor.val(arg.Data.UsarRegraPorExpedidor);

        _TipoCarga.Regras.val(arg.Data.TipoCarga);
        _TipoCarga.UsarRegraPorTipoCarga.val(arg.Data.UsarRegraPorTipoCarga);

        _CanalEntrega.Regras.val(arg.Data.CanalEntrega);
        _CanalEntrega.UsarRegraPorCanalEntrega.val(arg.Data.UsarRegraPorCanalEntrega);

        // Alterna os botões
        _CRUDRegras.Adicionar.visible(false);
        _CRUDRegras.Cancelar.visible(true);
        _CRUDRegras.Atualizar.visible(true);
        _CRUDRegras.Excluir.visible(true);
    }, null);
}

//*******MÉTODOS*******

function RemoverAprovadorClick(e, sender) {
    // Busca lista de aprovadores
    var aprovadores = _regraOcorrencia.GridAprovadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraOcorrencia.GridAprovadores.val(aprovadores);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        // Pega registros
        var dataGrid = _regraOcorrencia.GridAprovadores.val();

        // Objeto aprovador
        var aprovador = {
            Codigo: data.Codigo,
            Nome: data.Nome,
        };

        // Valida se ja nao existe o aprovador
        if (AprovadorJaExiste(dataGrid, aprovador)) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Aprovador, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.UsuarioJaConstaListaAprovadores.format(aprovador.Nome));
            return;
        }

        // Adiciona a lista e atualiza a grid
        dataGrid.push(aprovador);
        _regraOcorrencia.GridAprovadores.val(dataGrid);
    }
}

function AprovadorJaExiste(listaAprovadores, aprovador) {
    // Percorre lista para averiguar duplicidade
    for (var i in listaAprovadores) {
        if (listaAprovadores[i].Codigo == aprovador.Codigo)
            return true;
    }

    return false;
}

function RenderizarGridAprovadores() {
    // Apensa pega os valores
    var aprovadores = _regraOcorrencia.GridAprovadores.val();

    // E chama o metodo da grid
    _gridAprovadores.CarregarGrid(aprovadores);
}

//*******GLOBAL*******

function EnumCondicaoAutorizaoOcorrenciaDescricao(valor) {
    switch (valor) {
        case EnumCondicaoAutorizaoOcorrencia.IgualA: return "Igual a (==)";
        case EnumCondicaoAutorizaoOcorrencia.DiferenteDe: return "Diferente de (!=)";
        case EnumCondicaoAutorizaoOcorrencia.MaiorIgualQue: return "Maior ou igual que (>=)";
        case EnumCondicaoAutorizaoOcorrencia.MaiorQue: return "Maior que (>)";
        case EnumCondicaoAutorizaoOcorrencia.MenorIgualQue: return "Menor ou igual que (<=)";
        case EnumCondicaoAutorizaoOcorrencia.MenorQue: return "Menor que (<)";
        default: return "";
    }
}

function EnumJuncaoAutorizaoOcorrenciaDescricao(valor) {
    switch (valor) {
        case EnumJuncaoAutorizaoOcorrencia.E: return "E (Todas verdadeiras)";
        case EnumJuncaoAutorizaoOcorrencia.Ou: return "Ou (Apenas uma verdadeira)";
        default: return "";
    }
}

function SincronzarRegras() {
    _regraOcorrencia.UsarRegraPorTipoOcorrencia.val(_tipoOcorrencia.UsarRegraPorTipoOcorrencia.val());
    _regraOcorrencia.RegrasTipoOcorrencia.val(JSON.stringify(_tipoOcorrencia.Regras.val()));

    _regraOcorrencia.UsarRegraPorComponenteFrete.val(_componenteFrete.UsarRegraPorComponenteFrete.val());
    _regraOcorrencia.RegrasComponenteFrete.val(JSON.stringify(_componenteFrete.Regras.val()));

    _regraOcorrencia.UsarRegraPorFilialEmissao.val(_filialEmissao.UsarRegraPorFilialEmissao.val());
    _regraOcorrencia.RegrasFilialEmissao.val(JSON.stringify(_filialEmissao.Regras.val()));

    _regraOcorrencia.UsarRegraPorTomadorOcorrencia.val(_tomadorOcorrencia.UsarRegraPorTomadorOcorrencia.val());
    _regraOcorrencia.RegrasTomadorOcorrencia.val(JSON.stringify(_tomadorOcorrencia.Regras.val()));

    _regraOcorrencia.UsarRegraPorValorOcorrencia.val(_valorOcorrencia.UsarRegraPorValorOcorrencia.val());
    _regraOcorrencia.RegrasValorOcorrencia.val(JSON.stringify(_valorOcorrencia.Regras.val()));

    _regraOcorrencia.UsarRegraPorTipoOperacao.val(_tipoOperacao.UsarRegraPorTipoOperacao.val());
    _regraOcorrencia.RegrasTipoOperacao.val(JSON.stringify(_tipoOperacao.Regras.val()));

    _regraOcorrencia.UsarRegraPorDiasAbertura.val(_diasAbertura.UsarRegraPorDiasAbertura.val());
    _regraOcorrencia.RegrasDiasAbertura.val(JSON.stringify(_diasAbertura.Regras.val()));

    _regraOcorrencia.UsarRegraPorExpedidor.val(_expedidor.UsarRegraPorExpedidor.val());
    _regraOcorrencia.RegrasExpedidor.val(JSON.stringify(_expedidor.Regras.val()));

    _regraOcorrencia.UsarRegraPorCanalEntrega.val(_CanalEntrega.UsarRegraPorCanalEntrega.val());
    _regraOcorrencia.RegrasCanalEntrega.val(JSON.stringify(_CanalEntrega.Regras.val()));

    _regraOcorrencia.UsarRegraPorTipoCarga.val(_TipoCarga.UsarRegraPorTipoCarga.val());
    _regraOcorrencia.RegrasTipoCarga.val(JSON.stringify(_TipoCarga.Regras.val()));
}

function LimparTodosCampos() {
    LimparCampos(_regraOcorrencia);
    LimparCampos(_tipoOcorrencia);
    LimparCampos(_componenteFrete);
    LimparCampos(_filialEmissao);
    LimparCampos(_tomadorOcorrencia);
    LimparCampos(_valorOcorrencia);
    LimparCampos(_tipoOperacao);
    LimparCampos(_diasAbertura);
    LimparCampos(_expedidor);
    LimparCampos(_TipoCarga);
    LimparCampos(_CanalEntrega);
    _regraOcorrencia.GridAprovadores.val([]);

    $("#myTab li:first a").click();

    _CRUDRegras.Adicionar.visible(true);
    _CRUDRegras.Cancelar.visible(true);
    _CRUDRegras.Atualizar.visible(false);
    _CRUDRegras.Excluir.visible(false);
}

function GeraHeadTable(nomeCampo) { 

    return '<tr>' +
        '<th width="15%" class="text-align-center">' + Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Ordem +'</th>' +
        '<th width="30%" class="text-align-center">' + Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Juncao + '</th>' +
        '<th width="30%" class="text-align-center">' + Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Condicao + '</th>' +
        '<th width="40%" class="text-align-left">' + nomeCampo + '</th>' +
        '<th width="15%" class="text-align-center">' + Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Editar + '</th>' +
        '</tr>';
}

function ObterRegrasOrdenadas(kout) {
    var regras = kout.Regras.val().slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });
    return regras;
}

function LinhasReordenadasOcorrencia(kout) {
    var listaRegrasAtualizada = [];
    var listaRegras = kout.Regras.val();

    var BuscaRegraPorCodigo = function (codigo) {
        for (var i in listaRegras)
            if (listaRegras[i].Codigo == codigo)
                return listaRegras[i];

        return null;
    }

    $("#" + kout.Regras.idGrid + " table tbody tr").each(function (i) {
        var regra = BuscaRegraPorCodigo($(this).data('codigo'));
        regra.Ordem = i + 1;
        listaRegrasAtualizada.push(regra);
    });

    kout.Regras.val(listaRegrasAtualizada);
}

function RenderizarGridRegras(kout, grid, fnEditarRegra, tipo) {
    var html = "";
    var listaRegras = ObterRegrasOrdenadas(kout)

    $.each(listaRegras, function (i, regra) {
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_tipoOcorrencia_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
        html += '<td>' + EnumJuncaoAutorizaoOcorrenciaDescricao(regra.Juncao) + '</td>';
        html += '<td>' + EnumCondicaoAutorizaoOcorrenciaDescricao(regra.Condicao) + '</td>';
        if (!tipo)
            html += '<td>' + regra.Entidade.Descricao + '</td>';
        else if (tipo == typesKnockout.decimal)
            html += '<td>' + Globalize.format(regra.Valor, "n2") + '</td>';
        else
            html += '<td>' + regra.Valor + '</td>';

        html += '<td class="text-align-center"><a href="javascript:;" onclick="' + fnEditarRegra + '(\'' + regra.Codigo + '\')">Editar</a></td></tr>';
    });
    grid.RecarregarGrid(html);
}

function ValidarRegraDuplicada(listaRegras, regra, usarValor) {
    // Percorre lista para averiguar duplicidade
    for (var i in listaRegras) {
        if (
            (listaRegras[i].Codigo != regra.Codigo) &&
            (listaRegras[i].Condicao == regra.Condicao) &&
            (listaRegras[i].Juncao == regra.Juncao) &&
            ((!usarValor && listaRegras[i].Entidade.Codigo == regra.Entidade.Codigo) || usarValor && listaRegras[i].Valor == regra.Valor)
        )
            return false;
    }

    return true;
}
