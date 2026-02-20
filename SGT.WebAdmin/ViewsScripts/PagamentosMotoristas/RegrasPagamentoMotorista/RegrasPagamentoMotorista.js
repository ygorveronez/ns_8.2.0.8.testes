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
/// <reference path="RegraEmpresa.js" />
/// <reference path="RegraTipoPagamento.js" />
/// <reference path="RegraValor.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridRegraPagamentoMotorista;
var _gridAprovadores;
var _regraPagamentoMotorista;
var _pesquisaRegraPagamentoMotorista;

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

var PesquisaRegraPagamentoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data inicio: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "" });    
    this.Aprovador = PropertyEntity({ text: "Aprovador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraPagamentoMotorista.CarregarGrid();
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

var RegraPagamentoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "*Descrição: ", maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: "Número de Aprovadores: ", getType: typesKnockout.int, required: false, onfigInt: { precision: 0, allowZero: true } });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 2000 });    

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridAprovadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });
    this.GridAprovadores.val.subscribe(function () {
        _regraPagamentoMotorista.Aprovadores.val(JSON.stringify(_regraPagamentoMotorista.GridAprovadores.val()))
        RenderizarGridAprovadores();
    });

    // Regras
    this.RegraPorEmpresa = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasEmpresa = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorTipo = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTipo = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.RegraPorValor = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasValor = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });
}



//*******EVENTOS*******
function loadRegrasPagamentoMotorista() {
    _regraPagamentoMotorista = new RegraPagamentoMotorista();
    KoBindings(_regraPagamentoMotorista, "knockoutCadastroRegraPagamentoMotorista");

    _pesquisaRegraPagamentoMotorista = new PesquisaRegraPagamentoMotorista();
    KoBindings(_pesquisaRegraPagamentoMotorista, "knockoutPesquisaRegraPagamentoMotorista", false, _pesquisaRegraPagamentoMotorista.Pesquisar.id);

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
                    RemoverAprovadorClick(_regraPagamentoMotorista.GridAprovadores, data);
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
    _gridAprovadores = new BasicDataTable(_regraPagamentoMotorista.GridAprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridAprovadores);
    _gridAprovadores.CarregarGrid([]);

    //-- Pesquisa
    new BuscarFuncionario(_regraPagamentoMotorista.GridAprovadores, RetornoInserirAprovador);
    new BuscarFuncionario(_pesquisaRegraPagamentoMotorista.Aprovador);

    //-- Carrega os loads
    loadCRUDRegras();
    loadValor();
    loadTipoPagamento();
    loadEmpresa();

    //-- Busca Regras
    buscarRegrasPagamentoMotorista();
}

function buscarRegrasPagamentoMotorista() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasPagamentoMotorista, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraPagamentoMotorista = new GridView(_pesquisaRegraPagamentoMotorista.Pesquisar.idGrid, "RegrasPagamentoMotorista/Pesquisa", _pesquisaRegraPagamentoMotorista, menuOpcoes);
    _gridRegraPagamentoMotorista.CarregarGrid();
}

function editarRegrasPagamentoMotorista(data) {
    LimparTodosCampos();

    _regraPagamentoMotorista.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraPagamentoMotorista, "RegrasPagamentoMotorista/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraPagamentoMotorista.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraPagamentoMotorista.GridAprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        _valor.Regras.val(arg.Data.Valor);
        _valor.RegraPorValor.val(arg.Data.RegraPorValor);        

        _tipoPagamento.Regras.val(arg.Data.TipoPagamento);
        _tipoPagamento.RegraPorTipo.val(arg.Data.RegraPorTipo);

        _empresa.Regras.val(arg.Data.Empresa);
        _empresa.RegraPorEmpresa.val(arg.Data.RegraPorEmpresa);

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
    var aprovadores = _regraPagamentoMotorista.GridAprovadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraPagamentoMotorista.GridAprovadores.val(aprovadores);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        // Pega registros
        var dataGrid = _regraPagamentoMotorista.GridAprovadores.val();

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
        _regraPagamentoMotorista.GridAprovadores.val(dataGrid);
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
    var aprovadores = _regraPagamentoMotorista.GridAprovadores.val();

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
    _regraPagamentoMotorista.RegraPorEmpresa.val(_empresa.RegraPorEmpresa.val());
    _regraPagamentoMotorista.RegrasEmpresa.val(JSON.stringify(_empresa.Regras.val()));

    _regraPagamentoMotorista.RegraPorTipo.val(_tipoPagamento.RegraPorTipo.val());
    _regraPagamentoMotorista.RegrasTipo.val(JSON.stringify(_tipoPagamento.Regras.val()));

    _regraPagamentoMotorista.RegraPorValor.val(_valor.RegraPorValor.val());
    _regraPagamentoMotorista.RegrasValor.val(JSON.stringify(_valor.Regras.val()));
}

function LimparTodosCampos() {
    LimparCampos(_regraPagamentoMotorista);
    LimparCampos(_tipoPagamento);
    LimparCampos(_empresa);
    LimparCampos(_valor);
    _regraPagamentoMotorista.GridAprovadores.val([]);

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