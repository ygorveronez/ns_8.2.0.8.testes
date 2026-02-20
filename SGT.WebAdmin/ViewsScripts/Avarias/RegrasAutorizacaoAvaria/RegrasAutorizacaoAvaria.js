/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoAvaria.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoAvaria.js" />
/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoAvaria.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="MotivoAvaria.js" />
/// <reference path="Origem.js" />
/// <reference path="Destino.js" />
/// <reference path="Filial.js" />
/// <reference path="Transportador.js" />
/// <reference path="ValorAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraAvaria;
var _gridAprovadores;
var _regraAvaria;
var _pesquisaRegraAvaria;

var _configRegras = {
    Aprovadores: 3,
    infoTable: "Mova as linhas conforme a prioridade"
};

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _condicaoAutorizaoAvariaValor = [
    { text: EnumCondicaoAutorizaoAvariaDescricao(EnumCondicaoAutorizaoAvaria.IgualA), value: EnumCondicaoAutorizaoAvaria.IgualA },
    { text: EnumCondicaoAutorizaoAvariaDescricao(EnumCondicaoAutorizaoAvaria.DiferenteDe), value: EnumCondicaoAutorizaoAvaria.DiferenteDe },
    { text: EnumCondicaoAutorizaoAvariaDescricao(EnumCondicaoAutorizaoAvaria.MaiorIgualQue), value: EnumCondicaoAutorizaoAvaria.MaiorIgualQue },
    { text: EnumCondicaoAutorizaoAvariaDescricao(EnumCondicaoAutorizaoAvaria.MaiorQue), value: EnumCondicaoAutorizaoAvaria.MaiorQue },
    { text: EnumCondicaoAutorizaoAvariaDescricao(EnumCondicaoAutorizaoAvaria.MenorIgualQue), value: EnumCondicaoAutorizaoAvaria.MenorIgualQue },
    { text: EnumCondicaoAutorizaoAvariaDescricao(EnumCondicaoAutorizaoAvaria.MenorQue), value: EnumCondicaoAutorizaoAvaria.MenorQue }
];

var _condicaoAutorizaoAvariaEntidade = [
    { text: EnumCondicaoAutorizaoAvariaDescricao(EnumCondicaoAutorizaoAvaria.IgualA), value: EnumCondicaoAutorizaoAvaria.IgualA },
    { text: EnumCondicaoAutorizaoAvariaDescricao(EnumCondicaoAutorizaoAvaria.DiferenteDe), value: EnumCondicaoAutorizaoAvaria.DiferenteDe }
];

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _juncaoAutorizaoAvaria = [
    { text: EnumJuncaoAutorizaoAvariaDescricao(EnumJuncaoAutorizaoAvaria.E), value: EnumJuncaoAutorizaoAvaria.E },
    { text: EnumJuncaoAutorizaoAvariaDescricao(EnumJuncaoAutorizaoAvaria.Ou), value: EnumJuncaoAutorizaoAvaria.Ou }
];

var _etapaAutorizacaoAvaria = [
    { text: "Aprovação", value: EnumEtapaAutorizacaoAvaria.Aprovacao },
    { text: "Lote da Avaria", value: EnumEtapaAutorizacaoAvaria.Lote },
    { text: "Integração da Avaria", value: EnumEtapaAutorizacaoAvaria.Integracao }
];

var _pesquisaEtapaAutorizacaoAvaria = [
    { text: "Todas", value: EnumEtapaAutorizacaoAvaria.Todas },
    { text: "Lote da Avaria", value: EnumEtapaAutorizacaoAvaria.Lote },
    { text: "Integração da Avaria", value: EnumEtapaAutorizacaoAvaria.Integracao }
];

var PesquisaRegraAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, val: ko.observable(""), def: "" });
    this.EtapaAutorizacao = PropertyEntity({ text: "Etapa da Autorização: ", issue: 954, val: ko.observable(EnumEtapaAutorizacaoAvaria.Todas), options: _pesquisaEtapaAutorizacaoAvaria, def: EnumEtapaAutorizacaoAvaria.Todas });
    this.Aprovador = PropertyEntity({ text: "Aprovador:", issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraAvaria.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
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

var RegraAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: (_CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas ? "*Número de Aprovadores: " : "Número de Aprovadores: "), issue: 873, getType: typesKnockout.int, required: _CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas });
    this.Observacao = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000 });
    this.EtapaAutorizacao = PropertyEntity({ text: "Etapa da Autorização: ", issue: 954, val: ko.observable(EnumEtapaAutorizacaoAvaria.Aprovacao), options: _etapaAutorizacaoAvaria, def: EnumEtapaAutorizacaoAvaria.Aprovacao });
    this.PrioridadeAprovacao = PropertyEntity({ val: ko.observable(EnumPrioridadeAutorizacao.Zero), options: EnumPrioridadeAutorizacao.obterOpcoes(), def: EnumPrioridadeAutorizacao.Zero, text: "*Prioridade: " });
    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridAprovadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });
    this.GridAprovadores.val.subscribe(function () {
        _regraAvaria.Aprovadores.val(JSON.stringify(_regraAvaria.GridAprovadores.val()))
        RenderizarGridAprovadores();
    });

    // Regras
    this.UsarRegraPorMotivoAvaria = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasMotivoAvaria = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorOrigem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorDestino = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorFilial = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasFilial = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTransportador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorValorAvaria = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasValorAvaria = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });
};



//*******EVENTOS*******
function loadRegrasAutorizacaoAvaria() {
    _regraAvaria = new RegraAvaria();
    KoBindings(_regraAvaria, "knockoutCadastroRegraAvaria");

    _pesquisaRegraAvaria = new PesquisaRegraAvaria();
    KoBindings(_pesquisaRegraAvaria, "knockoutPesquisaRegraAvaria", false, _pesquisaRegraAvaria.Pesquisar.id);

    HeaderAuditoria("RegrasAutorizacaoAvaria", null, _regraAvaria, {
        RegraPorMotivoAvaria: "Regra por Motivo de Avaria",
        RegraPorOrigem: "Regra por Origem",
        RegraPorDestino: "Regra por Destino",
        RegraPorFilial: "Regra por Filial",
        RegraPorTransportador: "Regra por Transportador",
        RegraPorTipoOperacao: "Regra por Tipo de Operação",
        RegraPorValorAvaria: "Regra por Valor da Avaria",
    });

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
                    RemoverAprovadorClick(_regraAvaria.GridAprovadores, data);
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
    _gridAprovadores = new BasicDataTable(_regraAvaria.GridAprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridAprovadores);
    _gridAprovadores.CarregarGrid([]);



    //-- Pesquisa
    new BuscarFuncionario(_regraAvaria.GridAprovadores, RetornoInserirAprovador);
    new BuscarFuncionario(_pesquisaRegraAvaria.Aprovador);



    //-- Carrega os loads
    loadCRUDRegras();
    loadMotivoAvaria();
    loadOrigem();
    loadDestino();
    loadFilial();
    loadTransportador();
    loadTipoOperacao();
    loadValorAvaria();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        $("#liTabRegrasFilial").hide();

    //-- Busca Regras
    buscarRegrasAutorizacaoAvaria();
}

function buscarRegrasAutorizacaoAvaria() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasAvaria, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);


    var configExportacao = {
        url: "RegrasAutorizacaoAvaria/ExportarPesquisa",
        titulo: "Regras Autorização Avarias"
    };

    _gridRegraAvaria = new GridViewExportacao(_pesquisaRegraAvaria.Pesquisar.idGrid, "RegrasAutorizacaoAvaria/Pesquisa", _pesquisaRegraAvaria, menuOpcoes, configExportacao);
    _gridRegraAvaria.CarregarGrid();
}

function editarRegrasAvaria(data) {
    LimparTodosCampos();

    _regraAvaria.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraAvaria, "RegrasAutorizacaoAvaria/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraAvaria.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraAvaria.GridAprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        _motivoAvaria.Regras.val(arg.Data.MotivoAvaria);
        _motivoAvaria.UsarRegraPorMotivoAvaria.val(arg.Data.UsarRegraPorMotivoAvaria);

        _origem.Regras.val(arg.Data.Origem);
        _origem.UsarRegraPorOrigem.val(arg.Data.UsarRegraPorOrigem);

        _destino.Regras.val(arg.Data.Destino);
        _destino.UsarRegraPorDestino.val(arg.Data.UsarRegraPorDestino);

        _filial.Regras.val(arg.Data.Filial);
        _filial.UsarRegraPorFilial.val(arg.Data.UsarRegraPorFilial);

        _transportador.Regras.val(arg.Data.Transportador);
        _transportador.UsarRegraPorTransportador.val(arg.Data.UsarRegraPorTransportador);

        _tipoOperacao.Regras.val(arg.Data.TipoOperacao);
        _tipoOperacao.UsarRegraPorTipoOperacao.val(arg.Data.UsarRegraPorTipoOperacao);

        _valorAvaria.Regras.val(arg.Data.ValorAvaria);
        _valorAvaria.UsarRegraPorValorAvaria.val(arg.Data.UsarRegraPorValorAvaria);

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
    var aprovadores = _regraAvaria.GridAprovadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraAvaria.GridAprovadores.val(aprovadores);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        // Pega registros
        var dataGrid = _regraAvaria.GridAprovadores.val();

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
        _regraAvaria.GridAprovadores.val(dataGrid);
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
    var aprovadores = _regraAvaria.GridAprovadores.val();

    // E chama o metodo da grid
    _gridAprovadores.CarregarGrid(aprovadores);
}



//*******GLOBAL*******
function EnumCondicaoAutorizaoAvariaDescricao(valor) {
    switch (valor) {
        case EnumCondicaoAutorizaoAvaria.IgualA: return "Igual a (==)";
        case EnumCondicaoAutorizaoAvaria.DiferenteDe: return "Diferente de (!=)";
        case EnumCondicaoAutorizaoAvaria.MaiorIgualQue: return "Maior ou igual que (>=)";
        case EnumCondicaoAutorizaoAvaria.MaiorQue: return "Maior que (>)";
        case EnumCondicaoAutorizaoAvaria.MenorIgualQue: return "Menor ou igual que (<=)";
        case EnumCondicaoAutorizaoAvaria.MenorQue: return "Menor que (<)";
        default: return "";
    }
}

function EnumJuncaoAutorizaoAvariaDescricao(valor) {
    switch (valor) {
        case EnumJuncaoAutorizaoAvaria.E: return "E (Todas verdadeiras)";
        case EnumJuncaoAutorizaoAvaria.Ou: return "Ou (Apenas uma verdadeira)";
        default: return "";
    }
}

function SincronzarRegras() {
    _regraAvaria.UsarRegraPorMotivoAvaria.val(_motivoAvaria.UsarRegraPorMotivoAvaria.val());
    _regraAvaria.RegrasMotivoAvaria.val(JSON.stringify(_motivoAvaria.Regras.val()));

    _regraAvaria.UsarRegraPorOrigem.val(_origem.UsarRegraPorOrigem.val());
    _regraAvaria.RegrasOrigem.val(JSON.stringify(_origem.Regras.val()));

    _regraAvaria.UsarRegraPorDestino.val(_destino.UsarRegraPorDestino.val());
    _regraAvaria.RegrasDestino.val(JSON.stringify(_destino.Regras.val()));

    _regraAvaria.UsarRegraPorFilial.val(_filial.UsarRegraPorFilial.val());
    _regraAvaria.RegrasFilial.val(JSON.stringify(_filial.Regras.val()));

    _regraAvaria.UsarRegraPorTransportador.val(_transportador.UsarRegraPorTransportador.val());
    _regraAvaria.RegrasTransportador.val(JSON.stringify(_transportador.Regras.val()));

    _regraAvaria.UsarRegraPorTipoOperacao.val(_tipoOperacao.UsarRegraPorTipoOperacao.val());
    _regraAvaria.RegrasTipoOperacao.val(JSON.stringify(_tipoOperacao.Regras.val()));

    _regraAvaria.UsarRegraPorValorAvaria.val(_valorAvaria.UsarRegraPorValorAvaria.val());
    _regraAvaria.RegrasValorAvaria.val(JSON.stringify(_valorAvaria.Regras.val()));
}

function LimparTodosCampos() {
    LimparCampos(_regraAvaria);
    LimparCampos(_motivoAvaria);
    LimparCampos(_origem);
    LimparCampos(_destino);
    LimparCampos(_filial);
    LimparCampos(_transportador);
    LimparCampos(_tipoOperacao);
    LimparCampos(_valorAvaria);
    _regraAvaria.GridAprovadores.val([]);

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

function LinhasReordenadasAvaria(kout) {
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
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_tipoAvaria_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
        html += '<td>' + EnumJuncaoAutorizaoAvariaDescricao(regra.Juncao) + '</td>';
        html += '<td>' + EnumCondicaoAutorizaoAvariaDescricao(regra.Condicao) + '</td>';
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