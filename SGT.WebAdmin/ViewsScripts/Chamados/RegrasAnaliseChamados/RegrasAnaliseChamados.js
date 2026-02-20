/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoAvaria.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoAvaria.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="MotivoChamado.js" />
/// <reference path="Filial.js" />
/// <reference path="RegiaoDestino.js" />
/// <reference path="CargaDescarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraChamado;
var _gridAprovadores;
var _regraChamado;
var _pesquisaRegraChamado;

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

var PesquisaRegraChamado = function () {
    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, val: ko.observable(""), def: "" });
    this.Aprovador = PropertyEntity({ text: "Aprovador:", issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraChamado.CarregarGrid();
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

var RegraChamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000 });

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridAprovadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });
    this.GridAprovadores.val.subscribe(function () {
        _regraChamado.Aprovadores.val(JSON.stringify(_regraChamado.GridAprovadores.val()))
        RenderizarGridAprovadores();
    });

    // Regras
    this.UsarRegraPorMotivoChamado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasMotivoChamado = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorFilial = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasFilial = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorRegiaoDestino = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasRegiaoDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraCargaDescarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegraCargaDescarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });
};

//*******EVENTOS*******

function loadRegrasAnaliseChamado() {
    _regraChamado = new RegraChamado();
    KoBindings(_regraChamado, "knockoutCadastroRegraAnalise");

    _pesquisaRegraChamado = new PesquisaRegraChamado();
    KoBindings(_pesquisaRegraChamado, "knockoutPesquisaRegraAnalise", false, _pesquisaRegraChamado.Pesquisar.id);

    HeaderAuditoria("RegrasAnaliseChamados", _regraChamado, "Codigo", {
        RegraPorMotivoChamado: "Regra Por Motivo do Chamado",
        RegraPorFilial: "Regra Por Filial",
        RegrasChamadosFilial: "Regra Filial",
        RegrasMotivoChamado: "Regra Motivo"
    });

    SetarLayoutPorTipoServico();

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
                    RemoverAprovadorClick(_regraChamado.GridAprovadores, data);
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
    _gridAprovadores = new BasicDataTable(_regraChamado.GridAprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridAprovadores);
    _gridAprovadores.CarregarGrid([]);

    //-- Pesquisa
    new BuscarFuncionario(_regraChamado.GridAprovadores, RetornoInserirAprovador);
    new BuscarFuncionario(_pesquisaRegraChamado.Aprovador);

    //-- Carrega os loads
    loadCRUDRegras();
    loadMotivoChamado();
    loadFilial();
    loadRegiaoDestino();
    loadCargaDescarga();

    //-- Busca Regras
    buscarRegrasAnaliseChamado();
}

function buscarRegrasAnaliseChamado() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasChamado, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);


    var configExportacao = {
        url: "RegrasAnaliseChamados/ExportarPesquisa",
        titulo: "Regras Análise Chamado"
    };

    _gridRegraChamado = new GridViewExportacao(_pesquisaRegraChamado.Pesquisar.idGrid, "RegrasAnaliseChamados/Pesquisa", _pesquisaRegraChamado, menuOpcoes, configExportacao);
    _gridRegraChamado.CarregarGrid();
}

function editarRegrasChamado(data) {
    LimparTodosCampos();

    _regraChamado.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraChamado, "RegrasAnaliseChamados/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraChamado.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraChamado.GridAprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        _motivoChamado.Regras.val(arg.Data.MotivoChamado);
        _motivoChamado.UsarRegraPorMotivoChamado.val(arg.Data.UsarRegraPorMotivoChamado);

        _filial.Regras.val(arg.Data.Filial);
        _filial.UsarRegraPorFilial.val(arg.Data.UsarRegraPorFilial);

        _regiaoDestino.Regras.val(arg.Data.RegiaoDestino);
        _regiaoDestino.UsarRegraPorRegiaoDestino.val(arg.Data.UsarRegraPorRegiaoDestino);

        _cargaDescarga.Regras.val(arg.Data.CargaDescarga);
        _cargaDescarga.UsarRegraCargaDescarga.val(arg.Data.UsarRegraCargaDescarga);

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
    var aprovadores = _regraChamado.GridAprovadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraChamado.GridAprovadores.val(aprovadores);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        // Pega registros
        var dataGrid = _regraChamado.GridAprovadores.val();

        // Objeto aprovador
        var aprovador = {
            Codigo: data.Codigo,
            Nome: data.Nome,
        };

        // Valida se ja nao existe o aprovador
        if (AprovadorJaExiste(dataGrid, aprovador)) {
            exibirMensagem(tipoMensagem.aviso, "Aprovador", "O usuário " + aprovador.Nome + " já consta na lista de aprovadores.");
            return;
        }

        // Adiciona a lista e atualiza a grid
        dataGrid.push(aprovador);
        _regraChamado.GridAprovadores.val(dataGrid);
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
    var aprovadores = _regraChamado.GridAprovadores.val();

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
    _regraChamado.UsarRegraPorMotivoChamado.val(_motivoChamado.UsarRegraPorMotivoChamado.val());
    _regraChamado.RegrasMotivoChamado.val(JSON.stringify(_motivoChamado.Regras.val()));

    _regraChamado.UsarRegraPorFilial.val(_filial.UsarRegraPorFilial.val());
    _regraChamado.RegrasFilial.val(JSON.stringify(_filial.Regras.val()));

    _regraChamado.UsarRegraPorRegiaoDestino.val(_regiaoDestino.UsarRegraPorRegiaoDestino.val());
    _regraChamado.RegrasRegiaoDestino.val(JSON.stringify(_regiaoDestino.Regras.val()));

    _regraChamado.UsarRegraCargaDescarga.val(_cargaDescarga.UsarRegraCargaDescarga.val());
    _regraChamado.RegraCargaDescarga.val(JSON.stringify(_cargaDescarga.Regras.val()))
}

function LimparTodosCampos() {
    LimparCampos(_regraChamado);
    LimparCampos(_motivoChamado);
    LimparCampos(_filial);
    LimparCampos(_regiaoDestino);
    LimparCampos(_cargaDescarga);
    _regraChamado.GridAprovadores.val([]);

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

function LinhasReordenadasChamado(kout) {
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
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_tipoChamado_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
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

function SetarLayoutPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        $("#liTabFilial").addClass("hidden");
    }
}