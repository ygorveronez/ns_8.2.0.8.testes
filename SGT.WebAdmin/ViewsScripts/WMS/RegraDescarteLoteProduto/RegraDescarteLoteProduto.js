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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizao.js" />
/// <reference path="../../Consultas/Usuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridRegraDescarteLoteProduto;
var _gridAprovadores;
var _regraDescarteLoteProduto;
var _pesquisaRegraDescarteLoteProduto;

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

var PesquisaRegraDescarteLoteProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, val: ko.observable(""), def: "" });
    this.Aprovador = PropertyEntity({ text: "Aprovador:", issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraDescarteLoteProduto.CarregarGrid();
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

var RegraDescarteLoteProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: "Número de Aprovadores: ", issue: 873, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000 });

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridAprovadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });
    this.GridAprovadores.val.subscribe(function () {
        _regraDescarteLoteProduto.Aprovadores.val(JSON.stringify(_regraDescarteLoteProduto.GridAprovadores.val()))
        RenderizarGridAprovadores();
    });

    // Regras
    this.UsarRegraPorProdutoEmbarcador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlcadasProdutoEmbarcador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorDeposito = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlcadasDeposito = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorRua = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlcadasRua = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorBloco = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlcadasBloco = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorPosicao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlcadasPosicao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorQuantidade = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlcadasQuantidade = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });
}



//*******EVENTOS*******
function loadRegraDescarteLoteProduto() {
    _regraDescarteLoteProduto = new RegraDescarteLoteProduto();
    KoBindings(_regraDescarteLoteProduto, "knockoutCadastroRegraDescarteLoteProduto");

    _pesquisaRegraDescarteLoteProduto = new PesquisaRegraDescarteLoteProduto();
    KoBindings(_pesquisaRegraDescarteLoteProduto, "knockoutPesquisaRegraDescarteLoteProduto", false, _pesquisaRegraDescarteLoteProduto.Pesquisar.id);

    HeaderAuditoria("RegraDescarte", _regraDescarteLoteProduto);

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
                    RemoverAprovadorClick(_regraDescarteLoteProduto.GridAprovadores, data);
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
    _gridAprovadores = new BasicDataTable(_regraDescarteLoteProduto.GridAprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridAprovadores);
    _gridAprovadores.CarregarGrid([]);



    //-- Pesquisa
    new BuscarFuncionario(_regraDescarteLoteProduto.GridAprovadores, RetornoInserirAprovador);
    new BuscarFuncionario(_pesquisaRegraDescarteLoteProduto.Aprovador);



    //-- Carrega os loads
    loadCRUDRegras();
    loadProdutoEmbarcador();
    loadDeposito(); 
    loadRua();
    loadBloco();
    loadPosicao();
    loadQuantidade();



    //-- Busca Regras
   buscarRegrasAutorizacaoDescarteLoteProduto();
}

function buscarRegrasAutorizacaoDescarteLoteProduto() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasDescarteLoteProduto, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraDescarteLoteProduto = new GridView(_pesquisaRegraDescarteLoteProduto.Pesquisar.idGrid, "RegraDescarteLoteProduto/Pesquisa", _pesquisaRegraDescarteLoteProduto, menuOpcoes);
    _gridRegraDescarteLoteProduto.CarregarGrid();
}

function editarRegrasDescarteLoteProduto(data) {
    LimparTodosCampos();

    _regraDescarteLoteProduto.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraDescarteLoteProduto, "RegraDescarteLoteProduto/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraDescarteLoteProduto.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraDescarteLoteProduto.GridAprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        _produtoEmbarcador.Alcadas.val(arg.Data.ProdutoEmbarcador);
        _produtoEmbarcador.UsarRegraPorProdutoEmbarcador.val(arg.Data.UsarRegraPorProdutoEmbarcador);

        _deposito.Alcadas.val(arg.Data.Deposito);
        _deposito.UsarRegraPorDeposito.val(arg.Data.UsarRegraPorDeposito);

        _rua.Alcadas.val(arg.Data.Rua);
        _rua.UsarRegraPorRua.val(arg.Data.UsarRegraPorRua);

        _bloco.Alcadas.val(arg.Data.Bloco);
        _bloco.UsarRegraPorBloco.val(arg.Data.UsarRegraPorBloco);

        _posicao.Alcadas.val(arg.Data.Posicao);
        _posicao.UsarRegraPorPosicao.val(arg.Data.UsarRegraPorPosicao);

        _quantidade.Alcadas.val(arg.Data.Quantidade);
        _quantidade.UsarRegraPorQuantidade.val(arg.Data.UsarRegraPorQuantidade);


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
    var aprovadores = _regraDescarteLoteProduto.GridAprovadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraDescarteLoteProduto.GridAprovadores.val(aprovadores);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        // Pega registros
        var dataGrid = _regraDescarteLoteProduto.GridAprovadores.val();

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
        _regraDescarteLoteProduto.GridAprovadores.val(dataGrid);
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
    var aprovadores = _regraDescarteLoteProduto.GridAprovadores.val();

    // E chama o metodo da grid
    _gridAprovadores.CarregarGrid(aprovadores);
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
    _regraDescarteLoteProduto.UsarRegraPorProdutoEmbarcador.val(_produtoEmbarcador.UsarRegraPorProdutoEmbarcador.val());
    _regraDescarteLoteProduto.AlcadasProdutoEmbarcador.val(JSON.stringify(_produtoEmbarcador.Alcadas.val()));

    _regraDescarteLoteProduto.UsarRegraPorDeposito.val(_deposito.UsarRegraPorDeposito.val());
    _regraDescarteLoteProduto.AlcadasDeposito.val(JSON.stringify(_deposito.Alcadas.val()));

    _regraDescarteLoteProduto.UsarRegraPorRua.val(_rua.UsarRegraPorRua.val());
    _regraDescarteLoteProduto.AlcadasRua.val(JSON.stringify(_rua.Alcadas.val()));

    _regraDescarteLoteProduto.UsarRegraPorBloco.val(_bloco.UsarRegraPorBloco.val());
    _regraDescarteLoteProduto.AlcadasBloco.val(JSON.stringify(_bloco.Alcadas.val()));

    _regraDescarteLoteProduto.UsarRegraPorPosicao.val(_posicao.UsarRegraPorPosicao.val());
    _regraDescarteLoteProduto.AlcadasPosicao.val(JSON.stringify(_posicao.Alcadas.val()));

    _regraDescarteLoteProduto.UsarRegraPorQuantidade.val(_quantidade.UsarRegraPorQuantidade.val());
    _regraDescarteLoteProduto.AlcadasQuantidade.val(JSON.stringify(_quantidade.Alcadas.val()));
}

function LimparTodosCampos() {
    LimparCampos(_regraDescarteLoteProduto);
    LimparCampos(_produtoEmbarcador)
    LimparCampos(_deposito)
    LimparCampos(_rua)
    LimparCampos(_bloco)
    LimparCampos(_posicao)
    LimparCampos(_quantidade)
    _regraDescarteLoteProduto.GridAprovadores.val([]);

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

function LinhasReordenadasDescarteLoteProduto(kout) {
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
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_tipoDescarteLoteProduto_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
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