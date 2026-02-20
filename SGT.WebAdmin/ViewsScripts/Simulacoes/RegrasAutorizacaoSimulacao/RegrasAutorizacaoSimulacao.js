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
/// <reference path="FilialEmissao.js" />
/// <reference path="TipoSimulacao.js" />
/// <reference path="TipoOperacao.js" />
/// <reference path="Origem.js" />
/// <reference path="Destino.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraSimulacao;
var _gridAprovadores;
var _regraSimulacao;
var _pesquisaRegraAutorizacaoSimulacao;

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
var _juncaoAutorizaoSimulacao = EnumJuncaoAutorizaoOcorrencia.obterOpcoes();

var PesquisaRegraAutorizacaoSimulacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data Inicio", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Limite", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação"});
    this.Descricao = PropertyEntity({ text: "Descrição", issue: 586, val: ko.observable(""), def: "" });
    this.Aprovador = PropertyEntity({ text: "Aprovador", issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraSimulacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var RegraAutorizacaoSimulacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "Descrição", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: "Número de Aprovadores", issue: 873, getType: typesKnockout.int, required: true, maxlength: 3 });
    this.NumeroReprovadores = PropertyEntity({ text: "Número de Reprovadores", getType: typesKnockout.int, required: true, maxlength: 3 });
    this.Observacao = PropertyEntity({ text: "Observação", issue: 593, maxlength: 2000 });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Situação", issue: 557 });
    this.EnviarLinkParaAprovacaoPorEmail = PropertyEntity({ val: ko.observable(false), def: false, text: "Enviar Link para aprovação por e-mail" });

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridAprovadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });
    this.GridAprovadores.val.subscribe(function () {
        _regraSimulacao.Aprovadores.val(JSON.stringify(_regraSimulacao.GridAprovadores.val()))
        RenderizarGridAprovadores();
    });

    // Regras
    //this.UsarRegraPorTipoSimulacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    //this.RegrasTipoSimulacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorFilialEmissao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasFilialEmissao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorOrigem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorDestino = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTransportador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });
}

//*******EVENTOS*******

function loadRegrasAutorizacaoSimulacao() {
    _regraSimulacao = new RegraAutorizacaoSimulacao();
    KoBindings(_regraSimulacao, "knockoutCadastroRegraSimulacao");

    _pesquisaRegraAutorizacaoSimulacao = new PesquisaRegraAutorizacaoSimulacao();
    KoBindings(_pesquisaRegraAutorizacaoSimulacao, "knockoutPesquisaRegraAutorizacaoSimulacao", false, _pesquisaRegraAutorizacaoSimulacao.Pesquisar.id);

    HeaderAuditoria("RegrasAutorizacaoSimulacao", _regraSimulacao);

    //-- Grid Aprovadores
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "15",
                icone: "",
                metodo: function (data) {
                    RemoverAprovadorClick(_regraSimulacao.GridAprovadores, data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: "Usuário", width: "100%", className: "text-align-left" }
    ];

    // Grid
    _gridAprovadores = new BasicDataTable(_regraSimulacao.GridAprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridAprovadores);
    _gridAprovadores.CarregarGrid([]);



    //-- Pesquisa
    new BuscarFuncionario(_regraSimulacao.GridAprovadores, RetornoInserirAprovador);
    new BuscarFuncionario(_pesquisaRegraAutorizacaoSimulacao.Aprovador);
    //new BuscarTipoOcorrencia(_pesquisaRegraAutorizacaoSimulacao.TipoOcorrencia);

    //-- Carrega os loads
    loadCRUDRegras();
    loadFilialEmissao();
    loadTipoOperacao();
    loadTransportador();
    loadOrigem();
    loadDestino();

    //-- Busca Regras
    buscarRegrasAutorizacaoSimulacao();
}

function buscarRegrasAutorizacaoSimulacao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasAutorizacaoSimulacao, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraSimulacao = new GridView(_pesquisaRegraAutorizacaoSimulacao.Pesquisar.idGrid, "RegrasAutorizacaoSimulacao/Pesquisa", _pesquisaRegraAutorizacaoSimulacao, menuOpcoes);
    _gridRegraSimulacao.CarregarGrid();
}

function editarRegrasAutorizacaoSimulacao(data) {
    LimparTodosCampos();

    _regraSimulacao.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraSimulacao, "RegrasAutorizacaoSimulacao/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraAutorizacaoSimulacao.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraSimulacao.GridAprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        //_tipoSimulacao.Regras.val(arg.Data.TipoSimulacao);
        //_tipoSimulacao.UsarRegraPorTipoSimulacao.val(arg.Data.UsarRegraPorTipoSimulacao);

        _filialEmissao.Regras.val(arg.Data.FilialEmissao);
        _filialEmissao.UsarRegraPorFilialEmissao.val(arg.Data.UsarRegraPorFilialEmissao);

        _tipoOperacao.Regras.val(arg.Data.TipoOperacao);
        _tipoOperacao.UsarRegraPorTipoOperacao.val(arg.Data.UsarRegraPorTipoOperacao);

        _origem.Regras.val(arg.Data.Origem);
        _origem.UsarRegraPorOrigem.val(arg.Data.UsarRegraPorOrigem);

        _destino.Regras.val(arg.Data.Destino);
        _destino.UsarRegraPorDestino.val(arg.Data.UsarRegraPorDestino);

        _transportador.Regras.val(arg.Data.Destino);
        _transportador.UsarRegraPorTransportador.val(arg.Data.UsarRegraPorTransportador);

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
    var aprovadores = _regraSimulacao.GridAprovadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraSimulacao.GridAprovadores.val(aprovadores);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        // Pega registros
        var dataGrid = _regraSimulacao.GridAprovadores.val();

        // Objeto aprovador
        var aprovador = {
            Codigo: data.Codigo,
            Nome: data.Nome,
        };

        // Valida se ja nao existe o aprovador
        if (AprovadorJaExiste(dataGrid, aprovador)) {
            exibirMensagem(tipoMensagem.aviso, "Aprovador", `Usuário ${aprovador.Nome} já consta na lista de aprovadores`);
            return;
        }

        // Adiciona a lista e atualiza a grid
        dataGrid.push(aprovador);
        _regraSimulacao.GridAprovadores.val(dataGrid);
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
    var aprovadores = _regraSimulacao.GridAprovadores.val();

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
    //_regraSimulacao.UsarRegraPorTipoSimulacao.val(_tipoSimulacao.UsarRegraPorTipoSimulacao.val());
    //_regraSimulacao.RegrasTipoSimulacao.val(JSON.stringify(_tipoSimulacao.Regras.val()));

    _regraSimulacao.UsarRegraPorFilialEmissao.val(_filialEmissao.UsarRegraPorFilialEmissao.val());
    _regraSimulacao.RegrasFilialEmissao.val(JSON.stringify(_filialEmissao.Regras.val()));

    _regraSimulacao.UsarRegraPorTipoOperacao.val(_tipoOperacao.UsarRegraPorTipoOperacao.val());
    _regraSimulacao.RegrasTipoOperacao.val(JSON.stringify(_tipoOperacao.Regras.val()));

    _regraSimulacao.UsarRegraPorOrigem.val(_origem.UsarRegraPorOrigem.val());
    _regraSimulacao.RegrasOrigem.val(JSON.stringify(_origem.Regras.val()));

    _regraSimulacao.UsarRegraPorDestino.val(_destino.UsarRegraPorDestino.val());
    _regraSimulacao.RegrasDestino.val(JSON.stringify(_destino.Regras.val()));

    _regraSimulacao.UsarRegraPorTransportador.val(_transportador.UsarRegraPorTransportador.val());
    _regraSimulacao.RegrasTransportador.val(JSON.stringify(_transportador.Regras.val()));
}

function LimparTodosCampos() {
    LimparCampos(_regraSimulacao);
    //LimparCampos(_tipoSimulacao);
    LimparCampos(_filialEmissao);
    LimparCampos(_tipoOperacao);
    LimparCampos(_origem);
    LimparCampos(_destino);
    LimparCampos(_transportador);

    _regraSimulacao.GridAprovadores.val([]);

    $("#myTab li:first a").click();

    _CRUDRegras.Adicionar.visible(true);
    _CRUDRegras.Cancelar.visible(true);
    _CRUDRegras.Atualizar.visible(false);
    _CRUDRegras.Excluir.visible(false);
}

function GeraHeadTable(nomeCampo) { 

    return '<tr>' +
        '<th width="15%" class="text-align-center"> Ordem </th>' +
        '<th width="30%" class="text-align-center"> Junção </th>' +
        '<th width="30%" class="text-align-center"> Condição </th>' +
        '<th width="40%" class="text-align-left">' + nomeCampo +  '</th>' +
        '<th width="15%" class="text-align-center"> Editar </th>' +
        '</tr>';
}

function ObterRegrasOrdenadas(kout) {
    var regras = kout.Regras.val().slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });
    return regras;
}

function LinhasReordenadasSimulacao(kout) {
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
