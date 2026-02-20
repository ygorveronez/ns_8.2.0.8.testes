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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoNFSManual.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoNFSManual.js" />
/// <reference path="../../Enumeradores/EnumPrioridadeAutorizacao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="Filial.js" />
/// <reference path="Transportador.js" />
/// <reference path="Tomador.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridRegraNFSManual;
var _gridAprovadores;
var _regraNFSManual;
var _pesquisaRegraNFSManual;

var _configRegras = {
    Aprovadores: 3,
    infoTable: "Mova as linhas conforme a prioridade"
};

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _condicaoAutorizaoNFSManualValor = [
    { text: EnumCondicaoAutorizaoNFSManualDescricao(EnumCondicaoAutorizaoNFSManual.IgualA), value: EnumCondicaoAutorizaoNFSManual.IgualA },
    { text: EnumCondicaoAutorizaoNFSManualDescricao(EnumCondicaoAutorizaoNFSManual.DiferenteDe), value: EnumCondicaoAutorizaoNFSManual.DiferenteDe },
    { text: EnumCondicaoAutorizaoNFSManualDescricao(EnumCondicaoAutorizaoNFSManual.MaiorIgualQue), value: EnumCondicaoAutorizaoNFSManual.MaiorIgualQue },
    { text: EnumCondicaoAutorizaoNFSManualDescricao(EnumCondicaoAutorizaoNFSManual.MaiorQue), value: EnumCondicaoAutorizaoNFSManual.MaiorQue },
    { text: EnumCondicaoAutorizaoNFSManualDescricao(EnumCondicaoAutorizaoNFSManual.MenorIgualQue), value: EnumCondicaoAutorizaoNFSManual.MenorIgualQue },
    { text: EnumCondicaoAutorizaoNFSManualDescricao(EnumCondicaoAutorizaoNFSManual.MenorQue), value: EnumCondicaoAutorizaoNFSManual.MenorQue }
];

var _condicaoAutorizaoNFSManualEntidade = [
    { text: EnumCondicaoAutorizaoNFSManualDescricao(EnumCondicaoAutorizaoNFSManual.IgualA), value: EnumCondicaoAutorizaoNFSManual.IgualA },
    { text: EnumCondicaoAutorizaoNFSManualDescricao(EnumCondicaoAutorizaoNFSManual.DiferenteDe), value: EnumCondicaoAutorizaoNFSManual.DiferenteDe }
];

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _juncaoAutorizaoNFSManual = [
    { text: EnumJuncaoAutorizaoNFSManualDescricao(EnumJuncaoAutorizaoNFSManual.E), value: EnumJuncaoAutorizaoNFSManual.E },
    { text: EnumJuncaoAutorizaoNFSManualDescricao(EnumJuncaoAutorizaoNFSManual.Ou), value: EnumJuncaoAutorizaoNFSManual.Ou }
];


var PesquisaRegraNFSManual = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, val: ko.observable(""), def: "" });
    this.Aprovador = PropertyEntity({ text: "Aprovador:",issue: 930,  type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraNFSManual.CarregarGrid();
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

var RegraNFSManual = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "*Descrição: ",issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: (_CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas ? "*Número de Aprovadores: " : "Número de Aprovadores: "), issue: 873, getType: typesKnockout.int, required: _CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000 });
    this.PrioridadeAprovacao = PropertyEntity({ val: ko.observable(EnumPrioridadeAutorizacao.Zero), options: EnumPrioridadeAutorizacao.obterOpcoes(), def: EnumPrioridadeAutorizacao.Zero, text: "*Prioridade: " });

    this.Requisito = PropertyEntity({ text: "Requisito: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridAprovadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });
    this.GridAprovadores.val.subscribe(function () {
        _regraNFSManual.Aprovadores.val(JSON.stringify(_regraNFSManual.GridAprovadores.val()))
        RenderizarGridAprovadores();
    });

    // Regras
    this.UsarRegraPorFilial = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasFilial = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTransportador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTomador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTomador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorValorPrestacaoServico = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasValorPrestacaoServico = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });
}



//*******EVENTOS*******
function loadRegrasAutorizacaoNFSManual() {
    _regraNFSManual = new RegraNFSManual();
    KoBindings(_regraNFSManual, "knockoutCadastroRegraNFSManual");

    HeaderAuditoria("RegrasAutorizacaoNFSManual", _regraNFSManual);

    _pesquisaRegraNFSManual = new PesquisaRegraNFSManual();
    KoBindings(_pesquisaRegraNFSManual, "knockoutPesquisaRegraNFSManual", false, _pesquisaRegraNFSManual.Pesquisar.id);

    new BuscarRegraNFSManual(_regraNFSManual.Requisito, _regraNFSManual);

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
                    RemoverAprovadorClick(_regraNFSManual.GridAprovadores, data);
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
    _gridAprovadores = new BasicDataTable(_regraNFSManual.GridAprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridAprovadores);
    _gridAprovadores.CarregarGrid([]);



    //-- Pesquisa
    new BuscarFuncionario(_regraNFSManual.GridAprovadores, RetornoInserirAprovador);
    new BuscarFuncionario(_pesquisaRegraNFSManual.Aprovador);



    //-- Carrega os loads
    loadCRUDRegras();
    loadFilial();
    loadTransportador();
    loadTomador();
    loadValorPrestacaoServico();



    //-- Busca Regras
    buscarRegrasAutorizacaoNFSManual();
}

function buscarRegrasAutorizacaoNFSManual() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasNFSManual, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);


    var configExportacao = {
        url: "RegrasAutorizacaoNFSManual/ExportarPesquisa",
        titulo: "Regras Autorização NFSManuals"
    };

    _gridRegraNFSManual = new GridViewExportacao(_pesquisaRegraNFSManual.Pesquisar.idGrid, "RegrasAutorizacaoNFSManual/Pesquisa", _pesquisaRegraNFSManual, menuOpcoes, configExportacao);
    _gridRegraNFSManual.CarregarGrid();
}

function editarRegrasNFSManual(data) {
    LimparTodosCampos();

    _regraNFSManual.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraNFSManual, "RegrasAutorizacaoNFSManual/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraNFSManual.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraNFSManual.GridAprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        _filial.Regras.val(arg.Data.Filial);
        _filial.UsarRegraPorFilial.val(arg.Data.UsarRegraPorFilial); 

        _transportador.Regras.val(arg.Data.Transportador);
        _transportador.UsarRegraPorTransportador.val(arg.Data.UsarRegraPorTransportador);

        _tomador.Regras.val(arg.Data.Tomador);
        _tomador.UsarRegraPorTomador.val(arg.Data.UsarRegraPorTomador);

        _valorPrestacaoServico.Regras.val(arg.Data.ValorPrestacaoServico);
        _valorPrestacaoServico.UsarRegraPorValorPrestacaoServico.val(arg.Data.UsarRegraPorValorPrestacaoServico);
        
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
    var aprovadores = _regraNFSManual.GridAprovadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraNFSManual.GridAprovadores.val(aprovadores);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        // Pega registros
        var dataGrid = _regraNFSManual.GridAprovadores.val();

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
        _regraNFSManual.GridAprovadores.val(dataGrid);
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
    var aprovadores = _regraNFSManual.GridAprovadores.val();

    // E chama o metodo da grid
    _gridAprovadores.CarregarGrid(aprovadores);
}



//*******GLOBAL*******
function EnumCondicaoAutorizaoNFSManualDescricao(valor) {
    switch (valor) {
        case EnumCondicaoAutorizaoNFSManual.IgualA: return "Igual a (==)";
        case EnumCondicaoAutorizaoNFSManual.DiferenteDe: return "Diferente de (!=)";
        case EnumCondicaoAutorizaoNFSManual.MaiorIgualQue: return "Maior ou igual que (>=)";
        case EnumCondicaoAutorizaoNFSManual.MaiorQue: return "Maior que (>)";
        case EnumCondicaoAutorizaoNFSManual.MenorIgualQue: return "Menor ou igual que (<=)";
        case EnumCondicaoAutorizaoNFSManual.MenorQue: return "Menor que (<)";
        default: return "";
    }
}

function EnumJuncaoAutorizaoNFSManualDescricao(valor) {
    switch (valor) {
        case EnumJuncaoAutorizaoNFSManual.E: return "E (Todas verdadeiras)";
        case EnumJuncaoAutorizaoNFSManual.Ou: return "Ou (Apenas uma verdadeira)";
        default: return "";
    }
}

function SincronzarRegras() {
    _regraNFSManual.UsarRegraPorFilial.val(_filial.UsarRegraPorFilial.val());
    _regraNFSManual.RegrasFilial.val(JSON.stringify(_filial.Regras.val())); 

    _regraNFSManual.UsarRegraPorTransportador.val(_transportador.UsarRegraPorTransportador.val());
    _regraNFSManual.RegrasTransportador.val(JSON.stringify(_transportador.Regras.val()));

    _regraNFSManual.UsarRegraPorTomador.val(_tomador.UsarRegraPorTomador.val());
    _regraNFSManual.RegrasTomador.val(JSON.stringify(_tomador.Regras.val()));

    _regraNFSManual.UsarRegraPorValorPrestacaoServico.val(_valorPrestacaoServico.UsarRegraPorValorPrestacaoServico.val());
    _regraNFSManual.RegrasValorPrestacaoServico.val(JSON.stringify(_valorPrestacaoServico.Regras.val()));
    
}

function LimparTodosCampos() {
    LimparCampos(_regraNFSManual);
    LimparCampos(_filial);
    LimparCampos(_transportador);
    LimparCampos(_tomador);
    LimparCampos(_valorPrestacaoServico);
    
    _regraNFSManual.GridAprovadores.val([]);

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

function LinhasReordenadasNFSManual(kout) {
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
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_tipoNFSManual_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
        html += '<td>' + EnumJuncaoAutorizaoNFSManualDescricao(regra.Juncao) + '</td>';
        html += '<td>' + EnumCondicaoAutorizaoNFSManualDescricao(regra.Condicao) + '</td>';
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