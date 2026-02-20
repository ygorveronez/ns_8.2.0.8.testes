/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoOcorrencia.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="GRUDRegras.js" />
/// <reference path="RegraGrupoPessoa.js" />
/// <reference path="RegraTipoCarga.js" />
/// <reference path="RegraTipoOperacao.js" />
/// <reference path="RegraModeloVeicular.js" />
/// <reference path="RegraSituacaoColaborador.js" />
/// <reference path="RegraValorFrete.js" />
/// <reference path="RegraDistancia.js" />
/// <reference path="RegraDiferencaFreteLiquidoFreteTerceiro.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridRegraPedido;
var _gridAprovadores;
var _regraPedido;
var _pesquisaRegraPedido;

var _configRegras = {
    Aprovadores: 3,
    infoTable: "Mova as linhas conforme a prioridade"
};

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _condicaoAutorizaoValor = [
    { text: EnumCondicaoAutorizaoOcorrenciaDescricao(EnumCondicaoAutorizaoOcorrencia.IgualA), value: EnumCondicaoAutorizaoOcorrencia.IgualA },
    { text: EnumCondicaoAutorizaoOcorrenciaDescricao(EnumCondicaoAutorizaoOcorrencia.DiferenteDe), value: EnumCondicaoAutorizaoOcorrencia.DiferenteDe },
    { text: EnumCondicaoAutorizaoOcorrenciaDescricao(EnumCondicaoAutorizaoOcorrencia.MaiorIgualQue), value: EnumCondicaoAutorizaoOcorrencia.MaiorIgualQue },
    { text: EnumCondicaoAutorizaoOcorrenciaDescricao(EnumCondicaoAutorizaoOcorrencia.MaiorQue), value: EnumCondicaoAutorizaoOcorrencia.MaiorQue },
    { text: EnumCondicaoAutorizaoOcorrenciaDescricao(EnumCondicaoAutorizaoOcorrencia.MenorIgualQue), value: EnumCondicaoAutorizaoOcorrencia.MenorIgualQue },
    { text: EnumCondicaoAutorizaoOcorrenciaDescricao(EnumCondicaoAutorizaoOcorrencia.MenorQue), value: EnumCondicaoAutorizaoOcorrencia.MenorQue }
];

var _condicaoAutorizaoEntidade = [
    { text: EnumCondicaoAutorizaoOcorrenciaDescricao(EnumCondicaoAutorizaoOcorrencia.IgualA), value: EnumCondicaoAutorizaoOcorrencia.IgualA },
    { text: EnumCondicaoAutorizaoOcorrenciaDescricao(EnumCondicaoAutorizaoOcorrencia.DiferenteDe), value: EnumCondicaoAutorizaoOcorrencia.DiferenteDe }
];

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _juncaoAutorizao = [
    { text: EnumJuncaoAutorizaoOcorrenciaDescricao(EnumJuncaoAutorizaoOcorrencia.E), value: EnumJuncaoAutorizaoOcorrencia.E },
    { text: EnumJuncaoAutorizaoOcorrenciaDescricao(EnumJuncaoAutorizaoOcorrencia.Ou), value: EnumJuncaoAutorizaoOcorrencia.Ou }
];

var PesquisaRegraPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Descricao = PropertyEntity({ text: "Descrição:",issue: 586, val: ko.observable(""), def: "" });
    this.Aprovador = PropertyEntity({ text: "Aprovador:",issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraPedido.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var RegraPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "*Descrição: ",issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: (_CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas ? "*Número de Aprovadores: " : "Número de Aprovadores: "), issue: 873, getType: typesKnockout.int, required: _CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas });
    
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 2000 });

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridAprovadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });
    this.GridAprovadores.val.subscribe(function () {
        _regraPedido.Aprovadores.val(JSON.stringify(_regraPedido.GridAprovadores.val()))
        RenderizarGridAprovadores();
    });

    // Regras
    this.RegraPorGrupoPessoa = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasGrupoPessoa = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorTipoCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTipoCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorModeloVeicular = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasModeloVeicular = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorSituacaoColaborador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasSituacaoColaborador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorValorFrete = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasValorFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorDistancia = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasDistancia = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorDiferencaFreteLiquidoParaFreteTerceiro = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasDiferencaFreteLiquidoParaFreteTerceiro = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });
}



//*******EVENTOS*******
function loadRegrasPedido() {
    _regraPedido = new RegraPedido();
    KoBindings(_regraPedido, "knockoutCadastroRegraPedido");

    _pesquisaRegraPedido = new PesquisaRegraPedido();
    KoBindings(_pesquisaRegraPedido, "knockoutPesquisaRegraPedido", false, _pesquisaRegraPedido.Pesquisar.id);

    HeaderAuditoria("RegrasPedido", _regraPedido);

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
                    RemoverAprovadorClick(_regraPedido.GridAprovadores, data);
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
    _gridAprovadores = new BasicDataTable(_regraPedido.GridAprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridAprovadores);
    _gridAprovadores.CarregarGrid([]);

    //-- Pesquisa
    new BuscarFuncionario(_regraPedido.GridAprovadores, RetornoInserirAprovador);
    new BuscarFuncionario(_pesquisaRegraPedido.Aprovador);

    //-- Carrega os loads
    loadCRUDRegras();
    loadValorFrete();
    loadDistancia();
    loadRegraPorDiferencaFreteLiquidoParaFreteTerceiro();
    loadTipoCarga();
    loadGrupoPessoa();
    loadModeloVeicular();
    loadSituacaoColaborador();
    loadTipoOperacao();

    //-- Busca Regras
    buscarRegrasPedido();
}

function buscarRegrasPedido() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasPedido, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraPedido = new GridView(_pesquisaRegraPedido.Pesquisar.idGrid, "RegrasPedido/Pesquisa", _pesquisaRegraPedido, menuOpcoes);
    _gridRegraPedido.CarregarGrid();
}

function editarRegrasPedido(data) {
    LimparTodosCampos();

    _regraPedido.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraPedido, "RegrasPedido/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraPedido.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraPedido.GridAprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        _valorFrete.Regras.val(arg.Data.ValorFrete);
        _valorFrete.RegraPorValorFrete.val(arg.Data.RegraPorValorFrete);

        _tipoCarga.Regras.val(arg.Data.TipoCarga);
        _tipoCarga.RegraPorTipoCarga.val(arg.Data.RegraPorTipoCarga);

        _grupoPessoa.Regras.val(arg.Data.GrupoPessoa);
        _grupoPessoa.RegraPorGrupoPessoa.val(arg.Data.RegraPorGrupoPessoa);

        _tipoOperacao.Regras.val(arg.Data.TipoOperacao);
        _tipoOperacao.RegraPorTipoOperacao.val(arg.Data.RegraPorTipoOperacao);

        _modeloVeicular.Regras.val(arg.Data.ModeloVeicular);
        _modeloVeicular.RegraPorModeloVeicular.val(arg.Data.RegraPorModeloVeicular);

        _situacaoColaborador.Regras.val(arg.Data.SituacaoColaborador);
        _situacaoColaborador.RegraPorSituacaoColaborador.val(arg.Data.RegraPorSituacaoColaborador);

        _distancia.Regras.val(arg.Data.Distancia);
        _distancia.RegraPorDistancia.val(arg.Data.RegraPorDistancia);

        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Regras.val(arg.Data.DiferencaFreteLiquidoParaFreteTerceiro);
        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.RegraPorDiferencaFreteLiquidoParaFreteTerceiro.val(arg.Data.RegraPorDiferencaFreteLiquidoParaFreteTerceiro);

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
    var aprovadores = _regraPedido.GridAprovadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraPedido.GridAprovadores.val(aprovadores);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        // Pega registros
        var dataGrid = _regraPedido.GridAprovadores.val();

        // Objeto aprovador
        var aprovador = {
            Codigo: data.Codigo,
            Nome: data.Nome,
        };

        // Valida se ja nao existe o aprovador
        if (AprovadorJaExiste(dataGrid, aprovador)) {
            exibirMensagem(tipoMensagem.aviso, "Aprovador", "O usuário " + aprovador.Nome + " já consta da lista de aprovadores.");
            return;
        }

        // Adiciona a lista e atualiza a grid
        dataGrid.push(aprovador);
        _regraPedido.GridAprovadores.val(dataGrid);
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
    var aprovadores = _regraPedido.GridAprovadores.val();

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
    _regraPedido.RegraPorGrupoPessoa.val(_grupoPessoa.RegraPorGrupoPessoa.val());
    _regraPedido.RegrasGrupoPessoa.val(JSON.stringify(_grupoPessoa.Regras.val()));

    _regraPedido.RegraPorTipoCarga.val(_tipoCarga.RegraPorTipoCarga.val());
    _regraPedido.RegrasTipoCarga.val(JSON.stringify(_tipoCarga.Regras.val()));

    _regraPedido.RegraPorTipoOperacao.val(_tipoOperacao.RegraPorTipoOperacao.val());
    _regraPedido.RegrasTipoOperacao.val(JSON.stringify(_tipoOperacao.Regras.val()));

    _regraPedido.RegraPorModeloVeicular.val(_modeloVeicular.RegraPorModeloVeicular.val());
    _regraPedido.RegrasModeloVeicular.val(JSON.stringify(_modeloVeicular.Regras.val()));

    _regraPedido.RegraPorSituacaoColaborador.val(_situacaoColaborador.RegraPorSituacaoColaborador.val());
    _regraPedido.RegrasSituacaoColaborador.val(JSON.stringify(_situacaoColaborador.Regras.val()));

    _regraPedido.RegraPorValorFrete.val(_valorFrete.RegraPorValorFrete.val());
    _regraPedido.RegrasValorFrete.val(JSON.stringify(_valorFrete.Regras.val()));

    _regraPedido.RegraPorDistancia.val(_distancia.RegraPorDistancia.val());
    _regraPedido.RegrasDistancia.val(JSON.stringify(_distancia.Regras.val()));

    _regraPedido.RegraPorDiferencaFreteLiquidoParaFreteTerceiro.val(_regraPorDiferencaFreteLiquidoParaFreteTerceiro.RegraPorDiferencaFreteLiquidoParaFreteTerceiro.val());
    _regraPedido.RegrasDiferencaFreteLiquidoParaFreteTerceiro.val(JSON.stringify(_regraPorDiferencaFreteLiquidoParaFreteTerceiro.Regras.val()));
}

function LimparTodosCampos() {
    LimparCampos(_regraPedido);
    LimparCampos(_grupoPessoa);
    LimparCampos(_tipoCarga);
    LimparCampos(_tipoOperacao);
    LimparCampos(_modeloVeicular);
    LimparCampos(_situacaoColaborador);
    LimparCampos(_valorFrete);
    LimparCampos(_distancia);
    LimparCampos(_regraPorDiferencaFreteLiquidoParaFreteTerceiro);
    _regraPedido.GridAprovadores.val([]);

    $("#myTab li:first a").click();

    _CRUDRegras.Adicionar.visible(true);
    _CRUDRegras.Cancelar.visible(true);
    _CRUDRegras.Atualizar.visible(false);
    _CRUDRegras.Excluir.visible(false);
}

function GeraHeadTable(nomeCampo) {
    return '<tr>' +
        '<th width="15%" class="text-align-center">Ordem</th>' +
        '<th width="30%" class="text-align-center">Junção</th>' +
        '<th width="30%" class="text-align-center">Condição</th>' +
        '<th width="40%" class="text-align-left">' + nomeCampo + '</th>' +
        '<th width="15%" class="text-align-center">Editar</th>' +
        '</tr>';
}

function ObterRegrasOrdenadas(kout) {
    var regras = kout.Regras.val().slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });
    return regras;
}

function LinhasReordenadasPagamentoMotorista(kout) {
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

function RenderizarGridRegras(kout, grid, fnEditarRegra, usarValor) {
    var html = "";
    var listaRegras = ObterRegrasOrdenadas(kout)

    $.each(listaRegras, function (i, regra) {
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_tipoPagamentoMotorista_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
        html += '<td>' + EnumJuncaoAutorizaoOcorrenciaDescricao(regra.Juncao) + '</td>';
        html += '<td>' + EnumCondicaoAutorizaoOcorrenciaDescricao(regra.Condicao) + '</td>';
        if (!usarValor)
            html += '<td>' + regra.Entidade.Descricao + '</td>';
        else
            html += '<td>' + Globalize.format(regra.Valor, "n2") + '</td>';
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