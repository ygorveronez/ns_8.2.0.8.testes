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
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="RegraFuncionarioComissaoCRUD.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _gridRegraFuncionarioComissao;
var _gridAprovadores;
var _regraFuncionarioComissao;
var _pesquisaRegraFuncionarioComissao;

var _configRegras = {
    Aprovadores: 3,
    infoTable: "Mova as linhas conforme a prioridade",
    Alcadas: [
        {
            descricao: "Funcionário",
            prop: "Funcionario",
            tipoEntidade: true,
            busca: function (ko) {
                new BuscarFuncionario(ko);
            }
        },
        {
            descricao: "Valor",
            prop: "Valor",
            tipoEntidade: false
        },
    ]
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

var PesquisaRegraFuncionarioComissao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, val: ko.observable(""), def: "" });
    this.Aprovador = PropertyEntity({ text: "Aprovador:", issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraFuncionarioComissao.CarregarGrid();
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

var RegraFuncionarioComissao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: "Número de Aprovadores: ", issue: 873, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000 });

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ListaAprovadores, idGrid: guid() });
    this.AdicionarAprovador = PropertyEntity({ text: "Adicionar", idBtnSearch: guid() });

    this.Alcadas = PropertyEntity({ type: types.local, val: ko.observableArray([]), changeAba: CancelarAlcadaClick });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAlcadaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarAlcadaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirAlcadaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarAlcadaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    // Regras
    _configRegras.Alcadas.forEach(function (alcada) {
        this["UsarRegraPor" + alcada.prop] = PropertyEntity({ getType: typesKnockout.bool, idFade: guid(), text: "Ativar Regra de autorização por " + alcada.descricao + ":", val: ko.observable(false), def: false });
        this["Alcadas" + alcada.prop] = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: [], idGrid: guid(), list: [], val: ko.observable([]) });

        this["Codigo" + alcada.prop] = PropertyEntity({ type: types.local, val: ko.observable(""), def: "", getType: typesKnockout.string });
        this["Ordem" + alcada.prop] = PropertyEntity({ type: types.local, val: ko.observable(0), def: 0, getType: typesKnockout.int });

        this["Condicao" + alcada.prop] = PropertyEntity({ type: types.local, text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: alcada.tipoEntidade ? _condicaoAutorizaoEntidade : _condicaoAutorizaoValor, def: EnumCondicaoAutorizao.IgualA });
        this["Juncao" + alcada.prop] = PropertyEntity({ type: types.local, text: "Junção: ", val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });

        if (alcada.tipoEntidade)
            this[alcada.prop] = PropertyEntity({ text: alcada.descricao + ":", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
        else
            this[alcada.prop] = PropertyEntity({ text: alcada.descricao + ":", type: types.map, getType: typesKnockout.decimal, def: "0,00" });
    }, this);


    this.CRUDAdicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.CRUDCancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.CRUDAtualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.CRUDExcluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}



//*******EVENTOS*******
function loadRegraFuncionarioComissao() {
    _regraFuncionarioComissao = new RegraFuncionarioComissao();
    _regraFuncionarioComissao.Alcadas.val(_configRegras.Alcadas);

    KoBindings(_regraFuncionarioComissao, "knockoutCadastroRegraFuncionarioComissao");


    _configRegras.Alcadas.forEach(function (alcada) {
        if (alcada.tipoEntidade)
            alcada.busca(_regraFuncionarioComissao[alcada.prop]);

        alcada.grid = new GridReordering(_configRegras.infoTable, _regraFuncionarioComissao["Alcadas" + alcada.prop].idGrid, GeraHeadTable(alcada.descricao));
        alcada.grid.CarregarGrid();

        $("#" + _regraFuncionarioComissao["Alcadas" + alcada.prop].idGrid).on('sortstop', function () {
            LinhasReordenadasGrid("Alcadas" + alcada.prop);
        });


        _regraFuncionarioComissao["Alcadas" + alcada.prop].val.subscribe(function () {
            RenderizarGridRegras(alcada.prop, alcada.grid);
            AtualizaListAlcada(alcada.prop);
        });
    });

    _pesquisaRegraFuncionarioComissao = new PesquisaRegraFuncionarioComissao();
    KoBindings(_pesquisaRegraFuncionarioComissao, "knockoutPesquisaRegraFuncionarioComissao", false, _pesquisaRegraFuncionarioComissao.Pesquisar.id);

    HeaderAuditoria("RegraFuncionarioComissao", _regraFuncionarioComissao);

    GridAprovadores();

    //-- Pesquisa
    new BuscarFuncionario(_regraFuncionarioComissao.AdicionarAprovador, RetornoInserirAprovador, _gridAprovadores);
    new BuscarFuncionario(_pesquisaRegraFuncionarioComissao.Aprovador);

    //-- Busca Regras
    BuscarAlcadas();
}

function GridAprovadores() {
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
                    RemoverAprovadorClick(data);
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
    _gridAprovadores = new BasicDataTable(_regraFuncionarioComissao.Aprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.Aprovadores);
    _gridAprovadores.CarregarGrid([]);
}

function BuscarAlcadas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarRegrasAprovacao, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraFuncionarioComissao = new GridView(_pesquisaRegraFuncionarioComissao.Pesquisar.idGrid, "RegraFuncionarioComissao/Pesquisa", _pesquisaRegraFuncionarioComissao, menuOpcoes);
    _gridRegraFuncionarioComissao.CarregarGrid();
}

function EditarRegrasAprovacao(data) {
    LimparTodosCampos();

    _regraFuncionarioComissao.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraFuncionarioComissao, "RegraFuncionarioComissao/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraFuncionarioComissao.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraFuncionarioComissao.Aprovadores.val(arg.Data.Aprovadores);

        // Alterna os botões
        _regraFuncionarioComissao.CRUDAdicionar.visible(false);
        _regraFuncionarioComissao.CRUDCancelar.visible(true);
        _regraFuncionarioComissao.CRUDAtualizar.visible(true);
        _regraFuncionarioComissao.CRUDExcluir.visible(true);


        console.log(_regraFuncionarioComissao);
    }, null);
}

function AdicionarAlcadaClick(prop) {
    // Validacao de campos
    if (!ValidarCamposAlcada(prop))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoAlcada(prop);

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_regraFuncionarioComissao["Alcadas" + prop]);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    console.log(regra);
    listaRegras.push(regra);
    _regraFuncionarioComissao["Alcadas" + prop].val(listaRegras);

    // Limpa campos
    LimparCamposAlcada(prop);
}

function AtualizarAlcadaClick(prop) {
    // Validacao de campos
    if (!ValidarCamposAlcada(prop))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoAlcada(prop);
    var codigo = _regraFuncionarioComissao["Codigo" + prop].val();
    console.log(regra);
    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_regraFuncionarioComissao["Alcadas" + prop]);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _regraFuncionarioComissao["Alcadas" + prop].val(listaRegras);

    // Limpa campos
    LimparCamposAlcada(prop);
}

function ExcluirAlcadaClick(prop) {
    var listaRegras = ObterRegrasOrdenadas(_regraFuncionarioComissao["Alcadas" + prop]);
    var codigo = _regraFuncionarioComissao["Codigo" + prop].val();
    var index = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            index = parseInt(i);
            break;
        }
    }

    // Remove a regra especifica
    listaRegras.splice(index, 1);

    // Itera para corrigir o numero da ordem
    for (i = 1; i <= listaRegras.length; i++)
        listaRegras[i - 1].Ordem = i;

    // Atuliza o componente de regras
    _regraFuncionarioComissao["Alcadas" + prop].val(listaRegras);

    // Limpa o crud
    LimparCamposAlcada(prop);
}

function CancelarAlcadaClick(prop) {
    LimparCamposAlcada(prop);
}

function EditarRegraAlcadaClick(prop, codigo) {
    // Buscar todas regras
    var listaRegras = _regraFuncionarioComissao["Alcadas" + prop].val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {

        _regraFuncionarioComissao["Codigo" + prop].val(regra.Codigo);
        _regraFuncionarioComissao["Ordem" + prop].val(regra.Ordem);
        _regraFuncionarioComissao["Condicao" + prop].val(regra.Condicao);
        _regraFuncionarioComissao["Juncao" + prop].val(regra.Juncao);

        if ("Entidade" in regra) {
            _regraFuncionarioComissao[prop].val(regra.Entidade.Descricao);
            _regraFuncionarioComissao[prop].codEntity(regra.Entidade.Codigo);
        } else {
            _regraFuncionarioComissao[prop].val(regra.Valor);
        }

        _regraFuncionarioComissao.Adicionar.visible(false);
        _regraFuncionarioComissao.Atualizar.visible(true);
        _regraFuncionarioComissao.Excluir.visible(true);
        _regraFuncionarioComissao.Cancelar.visible(true);
    }
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
                Nome: a.Nome,
            };
        });

        // Adiciona a lista e atualiza a grid
        dataGrid = dataGrid.concat(data);
        _gridAprovadores.CarregarGrid(dataGrid);
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
    var aprovadores = _regraFuncionarioComissao.GridAprovadores.val();

    // E chama o metodo da grid
    _gridAprovadores.CarregarGrid(aprovadores);
}

function LimparCamposAlcada(prop) {
    var config = ObterConfigPorPropriedade(prop);

    _regraFuncionarioComissao["Codigo" + prop].val(_regraFuncionarioComissao["Codigo" + prop].def);
    _regraFuncionarioComissao["Ordem" + prop].val(_regraFuncionarioComissao["Ordem" + prop].def);
    _regraFuncionarioComissao["Condicao" + prop].val(_regraFuncionarioComissao["Condicao" + prop].def);
    _regraFuncionarioComissao["Juncao" + prop].val(_regraFuncionarioComissao["Juncao" + prop].def);

    if (config.tipoEntidade)
        LimparCampoEntity(_regraFuncionarioComissao[prop]);
    else
        _regraFuncionarioComissao[prop].val(_regraFuncionarioComissao[prop].def);

    _regraFuncionarioComissao.Adicionar.visible(true);
    _regraFuncionarioComissao.Atualizar.visible(false);
    _regraFuncionarioComissao.Excluir.visible(false);
    _regraFuncionarioComissao.Cancelar.visible(false);
}

function ObterConfigPorPropriedade(prop) {
    var config = null;

    for (var i in _configRegras.Alcadas) {
        if (_configRegras.Alcadas[i].prop == prop) {
            config = _configRegras.Alcadas[i];
            break;
        }
    }

    return config;
}

function ValidarCamposAlcada(prop) {
    var valido = true;
    var config = ObterConfigPorPropriedade(prop);

    if (config.tipoEntidade && !ValidarCampoObrigatorioEntity(_regraFuncionarioComissao[prop])) {
        valido = false;
    } else if (!ValidarCampoObrigatorioMap(_regraFuncionarioComissao[prop])) {
        valido = false;
    }

    return valido;
}

function ObjetoAlcada(prop) {
    var config = ObterConfigPorPropriedade(prop);

    var codigo = _regraFuncionarioComissao["Codigo" + prop].val();
    var ordem = _regraFuncionarioComissao["Ordem" + prop].val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : config.grid.ObterOrdencao().length + 1,
        Juncao: _regraFuncionarioComissao["Juncao" + prop].val(),
        Condicao: _regraFuncionarioComissao["Condicao" + prop].val(),

    };

    if (config.tipoEntidade) {
        regra.Entidade = {
            Codigo: parseInt(_regraFuncionarioComissao[prop].codEntity()),
            Descricao: _regraFuncionarioComissao[prop].val()
        };
    } else {
        regra.Valor = Globalize.parseFloat(_regraFuncionarioComissao[prop].val()) || 0;
    }

    return regra;
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

function LimparTodosCampos() {
    LimparCampos(_regraFuncionarioComissao);

    _configRegras.Alcadas.forEach(function (alcada) {
        LimparCamposAlcada(alcada.prop);
        _regraFuncionarioComissao["Alcadas" + alcada.prop].val([]);
    });

    _gridAprovadores.CarregarGrid([]);

    $("#myTab li:first a").click();

    _regraFuncionarioComissao.CRUDAdicionar.visible(true);
    _regraFuncionarioComissao.CRUDCancelar.visible(true);
    _regraFuncionarioComissao.CRUDAtualizar.visible(false);
    _regraFuncionarioComissao.CRUDExcluir.visible(false);
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
    var regras = kout.val().slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });
    return regras;
}

function LinhasReordenadasGrid(prop) {
    var kout = _regraFuncionarioComissao[prop];
    var listaRegrasAtualizada = [];
    var listaRegras = kout.val();

    var BuscaRegraPorCodigo = function (codigo) {
        for (var i in listaRegras)
            if (listaRegras[i].Codigo == codigo)
                return listaRegras[i];

        return null;
    }

    $("#" + _regraFuncionarioComissao[prop].idGrid + " table tbody tr").each(function (i) {
        var regra = BuscaRegraPorCodigo($(this).data('codigo'));
        regra.Ordem = i + 1;
        listaRegrasAtualizada.push(regra);
    });

    kout.val(listaRegrasAtualizada);
}

function RenderizarGridRegras(prop, grid) {
    var html = "";
    var kout = _regraFuncionarioComissao["Alcadas" + prop];
    var listaRegras = ObterRegrasOrdenadas(kout)

    $.each(listaRegras, function (i, regra) {
        var usarValor = regra.Entidade == null;
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_regra_' + prop + '_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
        html += '<td>' + EnumJuncaoAutorizaoDescricao(regra.Juncao) + '</td>';
        html += '<td>' + EnumCondicaoAutorizaoDescricao(regra.Condicao) + '</td>';
        if (!usarValor)
            html += '<td>' + regra.Entidade.Descricao + '</td>';
        else
            html += '<td>' + Globalize.format(regra.Valor, "n2") + '</td>';
        html += '<td class="text-align-center"><a href="javascript:;" onclick="EditarRegraAlcadaClick(\'' + prop + '\', \'' + regra.Codigo + '\')">Editar</a></td></tr>';
    });
    grid.RecarregarGrid(html);
}

function AtualizaListAlcada(prop) {
    var kout = _regraFuncionarioComissao["Alcadas" + prop];
    var listaRegras = ObterRegrasOrdenadas(kout)

    kout.list = JSON.stringify(regras);
}

function ValidarRegraDuplicada(listaRegras, regra) {
    var usarValor = "Valor" in regra;

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