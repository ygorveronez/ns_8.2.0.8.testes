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
/// <reference path="CRUDRegrasAtendimentoChamado.js" />
/// <reference path="RegraCanalVenda.js" />
/// <reference path="RegraFilial.js" />
/// <reference path="RegraTransportador.js" />
/// <reference path="RegraEstado.js" />
/// <reference path="RegraTipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridRegraAtendimentoChamado;
var _gridAprovadores;
var _regraAtendimentoChamado;
var _pesquisaRegraAtendimentoChamado;

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

var PesquisaRegraAtendimentoChamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Descricao = PropertyEntity({ text: "Descrição:",issue: 586, val: ko.observable(""), def: "" });
    this.Aprovador = PropertyEntity({ text: "Aprovador:",issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraAtendimentoChamado.CarregarGrid();
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

var RegrasAtendimentoChamado = function () {
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
        _regraAtendimentoChamado.Aprovadores.val(JSON.stringify(_regraAtendimentoChamado.GridAprovadores.val()))
        RenderizarGridAprovadores();
    });

    // Regras
    this.RegraPorCanalVenda = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasCanalVenda = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorFilial = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasFilial = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTransportador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorEstado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasEstado = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });
}



//*******EVENTOS*******
function loadRegrasRegrasAtendimentoChamado() {
    _regraAtendimentoChamado = new RegrasAtendimentoChamado();
    KoBindings(_regraAtendimentoChamado, "knockoutCadastroRegraAtendimentoChamado");

    _pesquisaRegraAtendimentoChamado = new PesquisaRegraAtendimentoChamado();
    KoBindings(_pesquisaRegraAtendimentoChamado, "knockoutPesquisaRegraAtendimentoChamado", false, _pesquisaRegraAtendimentoChamado.Pesquisar.id);

    HeaderAuditoria("RegrasAtendimentoChamado", _regraAtendimentoChamado);

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
                    RemoverAprovadorClick(_regraAtendimentoChamado.GridAprovadores, data);
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
    _gridAprovadores = new BasicDataTable(_regraAtendimentoChamado.GridAprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridAprovadores);
    _gridAprovadores.CarregarGrid([]);

    //-- Pesquisa
    new BuscarFuncionario(_regraAtendimentoChamado.GridAprovadores, RetornoInserirAprovador);
    new BuscarFuncionario(_pesquisaRegraAtendimentoChamado.Aprovador);

    //-- Carrega os loads
    loadRegraCanalVenda();
    loadRegraFilial();
    loadRegraTransportador();
    loadRegraTipoOperacao();
    loadRegraEstado();
    loadCRUDRegras();

    //-- Busca Regras
    buscarRegrasAtendimentoChamado();
}

function buscarRegrasAtendimentoChamado() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasAtendimentoChamado, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraAtendimentoChamado = new GridView(_pesquisaRegraAtendimentoChamado.Pesquisar.idGrid, "RegrasAtendimentoChamados/Pesquisa", _pesquisaRegraAtendimentoChamado, menuOpcoes);
    _gridRegraAtendimentoChamado.CarregarGrid();
}

function editarRegrasAtendimentoChamado(data) {
    LimparTodosCampos();

    _regraAtendimentoChamado.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraAtendimentoChamado, "RegrasAtendimentoChamados/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraAtendimentoChamado.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraAtendimentoChamado.GridAprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        _canalVenda.Regras.val(arg.Data.CanalVenda);
        _canalVenda.RegraPorCanalVenda.val(arg.Data.RegraPorCanalVenda);

        _filial.Regras.val(arg.Data.Filial);
        _filial.RegraPorFilial.val(arg.Data.RegraPorFilial);

        _tipoOperacao.Regras.val(arg.Data.TipoOperacao);
        _tipoOperacao.RegraPorTipoOperacao.val(arg.Data.RegraPorTipoOperacao);

        _transportador.Regras.val(arg.Data.Transportador);
        _transportador.RegraPorTransportador.val(arg.Data.RegraPorTransportador);

        _estado.Regras.val(arg.Data.Estado);
        _estado.RegraPorEstado.val(arg.Data.RegraPorEstado);

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
    var aprovadores = _regraAtendimentoChamado.GridAprovadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraAtendimentoChamado.GridAprovadores.val(aprovadores);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        // Pega registros
        var dataGrid = _regraAtendimentoChamado.GridAprovadores.val();

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
        _regraAtendimentoChamado.GridAprovadores.val(dataGrid);
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
    var aprovadores = _regraAtendimentoChamado.GridAprovadores.val();

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
    _regraAtendimentoChamado.RegraPorCanalVenda.val(_canalVenda.RegraPorCanalVenda.val());
    _regraAtendimentoChamado.RegrasCanalVenda.val(JSON.stringify(_canalVenda.Regras.val()));

    _regraAtendimentoChamado.RegraPorFilial.val(_filial.RegraPorFilial.val());
    _regraAtendimentoChamado.RegrasFilial.val(JSON.stringify(_filial.Regras.val()));

    _regraAtendimentoChamado.RegraPorTipoOperacao.val(_tipoOperacao.RegraPorTipoOperacao.val());
    _regraAtendimentoChamado.RegrasTipoOperacao.val(JSON.stringify(_tipoOperacao.Regras.val()));

    _regraAtendimentoChamado.RegraPorTransportador.val(_transportador.RegraPorTransportador.val());
    _regraAtendimentoChamado.RegrasTransportador.val(JSON.stringify(_transportador.Regras.val()));

    _regraAtendimentoChamado.RegraPorEstado.val(_estado.RegraPorEstado.val());
    _regraAtendimentoChamado.RegrasEstado.val(JSON.stringify(_estado.Regras.val()));
}

function LimparTodosCampos() {
    LimparCampos(_regraAtendimentoChamado);
    LimparCampos(_canalVenda);
    LimparCampos(_filial);
    LimparCampos(_tipoOperacao);
    LimparCampos(_transportador);
    LimparCampos(_estado);

    _regraAtendimentoChamado.GridAprovadores.val([]);

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

function LinhasReordenadas(kout) {
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