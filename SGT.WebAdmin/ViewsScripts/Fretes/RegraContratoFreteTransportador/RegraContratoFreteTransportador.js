/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumTipoAprovadorRegra.js" />
/// <reference path="../../Consultas/Usuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridRegraContratoFreteTransportador;
var _gridAprovadores;
var _regraContratoFreteTransportador;
var _pesquisaRegraContratoFreteTransportador;

var _configRegras = {
    Aprovadores: 3,
    infoTable: "Mova as linhas conforme a prioridade"
};

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _condicaoAutorizaoValor = [
    { text: EnumCondicaoAutorizaoDescricao(EnumCondicaoAutorizao.IgualA), value: EnumCondicaoAutorizao.IgualA },
    { text: EnumCondicaoAutorizaoDescricao(EnumCondicaoAutorizao.DiferenteDe), value: EnumCondicaoAutorizao.DiferenteDe },
    { text: EnumCondicaoAutorizaoDescricao(EnumCondicaoAutorizao.MaiorIgualQue), value: EnumCondicaoAutorizao.MaiorIgualQue },
    { text: EnumCondicaoAutorizaoDescricao(EnumCondicaoAutorizao.MaiorQue), value: EnumCondicaoAutorizao.MaiorQue },
    { text: EnumCondicaoAutorizaoDescricao(EnumCondicaoAutorizao.MenorIgualQue), value: EnumCondicaoAutorizao.MenorIgualQue },
    { text: EnumCondicaoAutorizaoDescricao(EnumCondicaoAutorizao.MenorQue), value: EnumCondicaoAutorizao.MenorQue }
];

var _condicaoAutorizaoEntidade = [
    { text: EnumCondicaoAutorizaoDescricao(EnumCondicaoAutorizao.IgualA), value: EnumCondicaoAutorizao.IgualA },
    { text: EnumCondicaoAutorizaoDescricao(EnumCondicaoAutorizao.DiferenteDe), value: EnumCondicaoAutorizao.DiferenteDe }
];

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _juncaoAutorizao = [
    { text: EnumJuncaoAutorizaoDescricao(EnumJuncaoAutorizao.E), value: EnumJuncaoAutorizao.E },
    { text: EnumJuncaoAutorizaoDescricao(EnumJuncaoAutorizao.Ou), value: EnumJuncaoAutorizao.Ou }
];

var PesquisaRegraContratoFreteTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, val: ko.observable(""), def: "" });
    this.Aprovador = PropertyEntity({ text: "Aprovador:", issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraContratoFreteTransportador.CarregarGrid();
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
};

var RegraContratoFreteTransportador = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557 });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: (_CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas ? "*Número de Aprovadores: " : "Número de Aprovadores: "), issue: 873, getType: typesKnockout.int, required: _CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000 });
    this.PrioridadeAprovacao = PropertyEntity({ val: ko.observable(EnumPrioridadeAutorizacao.Zero), options: EnumPrioridadeAutorizacao.obterOpcoes(), def: EnumPrioridadeAutorizacao.Zero, text: "*Prioridade: " });
    this.TipoAprovadorRegra = PropertyEntity({ val: ko.observable(EnumTipoAprovadorRegra.Usuario), options: EnumTipoAprovadorRegra.obterOpcoesPorTransportador(), def: EnumTipoAprovadorRegra.Usuario, text: "*Tipo do Aprovador: " });

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ListaAprovadores, idGrid: guid() });
    this.AdicionarAprovador = PropertyEntity({ text: "Adicionar", idBtnSearch: guid() });

    // Regras
    this.UsarRegraPorTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlcadasTransportador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorFilial = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlcadasFilial = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorValorContrato = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasValorContrato = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.TipoAprovadorRegra.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoAprovadorRegra.Transportador) {
            self.NumeroAprovadores.val("1");
            self.NumeroAprovadores.enable(false);
        }
        else
            self.NumeroAprovadores.enable(true);
    });

};



//*******EVENTOS*******
function loadRegraContratoFreteTransportador() {
    
    _regraContratoFreteTransportador = new RegraContratoFreteTransportador();
    KoBindings(_regraContratoFreteTransportador, "knockoutCadastroRegraContratoFreteTransportador");
    
    
    _pesquisaRegraContratoFreteTransportador = new PesquisaRegraContratoFreteTransportador();
    KoBindings(_pesquisaRegraContratoFreteTransportador, "knockoutPesquisaRegraContratoFreteTransportador", false, _pesquisaRegraContratoFreteTransportador.Pesquisar.id);

    HeaderAuditoria("RegraContratoFreteTransportador", _regraContratoFreteTransportador);

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
                metodo: RemoverAprovadorClick
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: "Usuário", width: "100%", className: "text-align-left" }
    ];

    // Grid
    _gridAprovadores = new BasicDataTable(_regraContratoFreteTransportador.Aprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.Aprovadores);
    _gridAprovadores.CarregarGrid([]);



    //-- Pesquisa
    new BuscarFuncionario(_regraContratoFreteTransportador.AdicionarAprovador, RetornoInserirAprovador, _gridAprovadores);
    new BuscarFuncionario(_pesquisaRegraContratoFreteTransportador.Aprovador);



    //-- Carrega os loads
    loadCRUDRegras();
    loadTransportador();
    loadFilial();
    loadValorContrato();
    

    //-- Busca Regras
    BuscarRegrasAutorizacaoContratoFreteTransportador();
}

function BuscarRegrasAutorizacaoContratoFreteTransportador() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarRegrasContratoFreteTransportador, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraContratoFreteTransportador = new GridView(_pesquisaRegraContratoFreteTransportador.Pesquisar.idGrid, "RegraContratoFreteTransportador/Pesquisa", _pesquisaRegraContratoFreteTransportador, menuOpcoes);
    _gridRegraContratoFreteTransportador.CarregarGrid();
}

function editarRegrasContratoFreteTransportador(data) {
    LimparTodosCampos();

    _regraContratoFreteTransportador.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraContratoFreteTransportador, "RegraContratoFreteTransportador/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraContratoFreteTransportador.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraContratoFreteTransportador.Aprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        _transportador.Alcadas.val(arg.Data.Transportador);
        _transportador.UsarRegraPorTransportador.val(arg.Data.UsarRegraPorTransportador);

        _filial.Alcadas.val(arg.Data.Filial);
        _filial.UsarRegraPorFilial.val(arg.Data.UsarRegraPorFilial);

        _valorContrato.Alcadas.val(arg.Data.RegrasValorContrato);
        _valorContrato.UsarRegraPorValorContrato.val(arg.Data.UsarRegraPorValorContrato);

        // Alterna os botões
        _CRUDRegras.Adicionar.visible(false);
        _CRUDRegras.Cancelar.visible(true);
        _CRUDRegras.Atualizar.visible(true);
        _CRUDRegras.Excluir.visible(true);
    }, null);
}



//*******MÉTODOS*******
function RemoverAprovadorClick(data) {
    // Busca lista de aprovadores
    var dataGrid = _gridAprovadores.BuscarRegistros().slice();

    // Remove
    dataGrid = dataGrid.filter(function (a) {
        return a.Codigo != data.Codigo;
    });

    _gridAprovadores.CarregarGrid(dataGrid);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        if (!$.isArray(data)) data = [data];

        // Pega registros
        var dataGrid = _gridAprovadores.BuscarRegistros();

        // Objeto aprovador
        data = data.map(function (a) {
            return {
                Codigo: a.Codigo,
                Nome: a.Nome
            };
        });

        // Adiciona a lista e atualiza a grid
        dataGrid = dataGrid.concat(data);
        _gridAprovadores.CarregarGrid(dataGrid);
    }
}



//*******GLOBAL*******
function EnumCondicaoAutorizaoDescricao(valor) {
    switch (valor) {
        case EnumCondicaoAutorizao.IgualA: return "Igual a (==)";
        case EnumCondicaoAutorizao.DiferenteDe: return "Diferente de (!=)";
        case EnumCondicaoAutorizao.MaiorIgualQue: return "Maior ou igual que (>=)";
        case EnumCondicaoAutorizao.MaiorQue: return "Maior que (>)";
        case EnumCondicaoAutorizao.MenorIgualQue: return "Menor ou igual que (<=)";
        case EnumCondicaoAutorizao.MenorQue: return "Menor que (<)";
        default: return "";
    }
}

function EnumJuncaoAutorizaoDescricao(valor) {
    switch (valor) {
        case EnumJuncaoAutorizao.E: return "E (Todas verdadeiras)";
        case EnumJuncaoAutorizao.Ou: return "Ou (Apenas uma verdadeira)";
        default: return "";
    }
}

function SincronzarRegras() {
    _regraContratoFreteTransportador.UsarRegraPorTransportador.val(_transportador.UsarRegraPorTransportador.val());
    _regraContratoFreteTransportador.AlcadasTransportador.val(JSON.stringify(_transportador.Alcadas.val()));

    _regraContratoFreteTransportador.UsarRegraPorFilial.val(_filial.UsarRegraPorFilial.val());
    _regraContratoFreteTransportador.AlcadasFilial.val(JSON.stringify(_filial.Alcadas.val()));

    _regraContratoFreteTransportador.UsarRegraPorValorContrato.val(_valorContrato.UsarRegraPorValorContrato.val());
    _regraContratoFreteTransportador.RegrasValorContrato.val(JSON.stringify(_valorContrato.Alcadas.val()));
}

function LimparTodosCampos() {
    LimparCampos(_regraContratoFreteTransportador);
    LimparCampos(_transportador);
    LimparCampos(_valorContrato);
    _gridAprovadores.CarregarGrid([]);

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
    var regras = kout.Alcadas.val().slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });
    return regras;
}

function LinhasReordenadasRegraContratoFreteTransportador(kout) {
    var listaRegrasAtualizada = [];
    var listaRegras = kout.Alcadas.val();

    var BuscaRegraPorCodigo = function (codigo) {
        for (var i in listaRegras)
            if (listaRegras[i].Codigo == codigo)
                return listaRegras[i];

        return null;
    }

    $("#" + kout.Alcadas.idGrid + " table tbody tr").each(function (i) {
        var regra = BuscaRegraPorCodigo($(this).data('codigo'));
        regra.Ordem = i + 1;
        listaRegrasAtualizada.push(regra);
    });

    kout.Alcadas.val(listaRegrasAtualizada);
}

function RenderizarGridRegras(kout, grid, fnEditarRegra, usarValor) {
    var html = "";
    var listaRegras = ObterRegrasOrdenadas(kout);

    $.each(listaRegras, function (i, regra) {
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_tipoContratoFreteTransportador_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
        html += '<td>' + EnumJuncaoAutorizaoDescricao(regra.Juncao) + '</td>';
        html += '<td>' + EnumCondicaoAutorizaoDescricao(regra.Condicao) + '</td>';
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

function ListaAprovadores() {
    if (arguments.length > 0 && arguments[0] != "")
        _gridAprovadores.CarregarGrid(arguments[0]);
    else
        return JSON.stringify(_gridAprovadores.BuscarRegistros());
}