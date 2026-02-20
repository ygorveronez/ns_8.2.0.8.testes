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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="RegraCotacaoPedidoCRUD.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _gridRegraCotacaoPedido;
var _gridAprovadores;
var _regraCotacaoPedido;
var _pesquisaRegraCotacaoPedido;

var _configRegras = {
    Aprovadores: 3,
    infoTable: "Mova as linhas conforme a prioridade",
    Alcadas: [
        {
            descricao: "Tipo Carga",
            prop: "TipoCarga",
            tipoEntidade: true,
            busca: function (ko) {
                new BuscarTiposdeCarga(ko);
            }
        },
        {
            descricao: "Tipo Operação",
            prop: "TipoOperacao",
            tipoEntidade: true,
            busca: function (ko) {
                new BuscarTiposOperacao(ko);
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

var PesquisaRegraCotacaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, val: ko.observable(""), def: "" });
    this.Aprovador = PropertyEntity({ text: "Aprovador:", issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraCotacaoPedido.CarregarGrid();
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

var RegraCotacaoPedido = function () {
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
function loadRegraCotacaoPedido() {
    _regraCotacaoPedido = new RegraCotacaoPedido();
    _regraCotacaoPedido.Alcadas.val(_configRegras.Alcadas);

    KoBindings(_regraCotacaoPedido, "knockoutCadastroRegraCotacaoPedido");


    _configRegras.Alcadas.forEach(function (alcada) {
        if (alcada.tipoEntidade)
            alcada.busca(_regraCotacaoPedido[alcada.prop]);

        alcada.grid = new GridReordering(_configRegras.infoTable, _regraCotacaoPedido["Alcadas" + alcada.prop].idGrid, GeraHeadTable(alcada.descricao));
        alcada.grid.CarregarGrid();

        $("#" + _regraCotacaoPedido["Alcadas" + alcada.prop].idGrid).on('sortstop', function () {
            LinhasReordenadasGrid("Alcadas" + alcada.prop);
        });


        _regraCotacaoPedido["Alcadas" + alcada.prop].val.subscribe(function () {
            RenderizarGridRegras(alcada.prop, alcada.grid);
            AtualizaListAlcada(alcada.prop);
        });
    });

    _pesquisaRegraCotacaoPedido = new PesquisaRegraCotacaoPedido();
    KoBindings(_pesquisaRegraCotacaoPedido, "knockoutPesquisaRegraCotacaoPedido", false, _pesquisaRegraCotacaoPedido.Pesquisar.id);

    HeaderAuditoria("RegraCotacaoPedido", _regraCotacaoPedido);

    GridAprovadores();

    //-- Pesquisa
    new BuscarFuncionario(_regraCotacaoPedido.AdicionarAprovador, RetornoInserirAprovador, _gridAprovadores);
    new BuscarFuncionario(_pesquisaRegraCotacaoPedido.Aprovador);

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
    _gridAprovadores = new BasicDataTable(_regraCotacaoPedido.Aprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.Aprovadores);
    _gridAprovadores.CarregarGrid([]);
}

function BuscarAlcadas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarRegrasAprovacao, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraCotacaoPedido = new GridView(_pesquisaRegraCotacaoPedido.Pesquisar.idGrid, "RegraCotacaoPedido/Pesquisa", _pesquisaRegraCotacaoPedido, menuOpcoes);
    _gridRegraCotacaoPedido.CarregarGrid();
}

function EditarRegrasAprovacao(data) {
    LimparTodosCampos();

    _regraCotacaoPedido.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraCotacaoPedido, "RegraCotacaoPedido/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraCotacaoPedido.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraCotacaoPedido.Aprovadores.val(arg.Data.Aprovadores);

        // Alterna os botões
        _regraCotacaoPedido.CRUDAdicionar.visible(false);
        _regraCotacaoPedido.CRUDCancelar.visible(true);
        _regraCotacaoPedido.CRUDAtualizar.visible(true);
        _regraCotacaoPedido.CRUDExcluir.visible(true);
               
    }, null);
}

function AdicionarAlcadaClick(prop) {
    // Validacao de campos
    if (!ValidarCamposAlcada(prop))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, inform254354e os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoAlcada(prop);

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_regraCotacaoPedido["Alcadas" + prop]);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _regraCotacaoPedido["Alcadas" + prop].val(listaRegras);

    // Limpa campos
    LimparCamposAlcada(prop);
}

function AtualizarAlcadaClick(prop) {
    // Validacao de campos
    if (!ValidarCamposAlcada(prop))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe 2354432os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoAlcada(prop);
    var codigo = _regraCotacaoPedido["Codigo" + prop].val();
    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_regraCotacaoPedido["Alcadas" + prop]);

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
    _regraCotacaoPedido["Alcadas" + prop].val(listaRegras);

    // Limpa campos
    LimparCamposAlcada(prop);
}

function ExcluirAlcadaClick(prop) {
    var listaRegras = ObterRegrasOrdenadas(_regraCotacaoPedido["Alcadas" + prop]);
    var codigo = _regraCotacaoPedido["Codigo" + prop].val();
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
    _regraCotacaoPedido["Alcadas" + prop].val(listaRegras);

    // Limpa o crud
    LimparCamposAlcada(prop);
}

function CancelarAlcadaClick(prop) {
    LimparCamposAlcada(prop);
}

function EditarRegraAlcadaClick(prop, codigo) {
    // Buscar todas regras
    var listaRegras = _regraCotacaoPedido["Alcadas" + prop].val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {

        _regraCotacaoPedido["Codigo" + prop].val(regra.Codigo);
        _regraCotacaoPedido["Ordem" + prop].val(regra.Ordem);
        _regraCotacaoPedido["Condicao" + prop].val(regra.Condicao);
        _regraCotacaoPedido["Juncao" + prop].val(regra.Juncao);

        if ("Entidade" in regra) {
            _regraCotacaoPedido[prop].val(regra.Entidade.Descricao);
            _regraCotacaoPedido[prop].codEntity(regra.Entidade.Codigo);
        } else {
            _regraCotacaoPedido[prop].val(regra.Valor);
        }

        _regraCotacaoPedido.Adicionar.visible(false);
        _regraCotacaoPedido.Atualizar.visible(true);
        _regraCotacaoPedido.Excluir.visible(true);
        _regraCotacaoPedido.Cancelar.visible(true);
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
    var aprovadores = _regraCotacaoPedido.GridAprovadores.val();

    // E chama o metodo da grid
    _gridAprovadores.CarregarGrid(aprovadores);
}

function LimparCamposAlcada(prop) {
    var config = ObterConfigPorPropriedade(prop);

    _regraCotacaoPedido["Codigo" + prop].val(_regraCotacaoPedido["Codigo" + prop].def);
    _regraCotacaoPedido["Ordem" + prop].val(_regraCotacaoPedido["Ordem" + prop].def);
    _regraCotacaoPedido["Condicao" + prop].val(_regraCotacaoPedido["Condicao" + prop].def);
    _regraCotacaoPedido["Juncao" + prop].val(_regraCotacaoPedido["Juncao" + prop].def);

    if (config.tipoEntidade)
        LimparCampoEntity(_regraCotacaoPedido[prop]);
    else
        _regraCotacaoPedido[prop].val(_regraCotacaoPedido[prop].def);

    _regraCotacaoPedido.Adicionar.visible(true);
    _regraCotacaoPedido.Atualizar.visible(false);
    _regraCotacaoPedido.Excluir.visible(false);
    _regraCotacaoPedido.Cancelar.visible(false);
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

    if (config.tipoEntidade && !ValidarCampoObrigatorioEntity(_regraCotacaoPedido[prop])) {
        valido = false;
    } else if (!ValidarCampoObrigatorioMap(_regraCotacaoPedido[prop])) {
        valido = false;
    }

    return valido;
}

function ObjetoAlcada(prop) {
    var config = ObterConfigPorPropriedade(prop);

    var codigo = _regraCotacaoPedido["Codigo" + prop].val();
    var ordem = _regraCotacaoPedido["Ordem" + prop].val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : config.grid.ObterOrdencao().length + 1,
        Juncao: _regraCotacaoPedido["Juncao" + prop].val(),
        Condicao: _regraCotacaoPedido["Condicao" + prop].val(),

    };

    if (config.tipoEntidade) {
        regra.Entidade = {
            Codigo: parseInt(_regraCotacaoPedido[prop].codEntity()),
            Descricao: _regraCotacaoPedido[prop].val()
        };
    } else {
        regra.Valor = Globalize.parseFloat(_regraCotacaoPedido[prop].val()) || 0;
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
    LimparCampos(_regraCotacaoPedido);

    _configRegras.Alcadas.forEach(function (alcada) {
        LimparCamposAlcada(alcada.prop);
        _regraCotacaoPedido["Alcadas" + alcada.prop].val([]);
    });

    _gridAprovadores.CarregarGrid([]);

    $("#myTab li:first a").click();

    _regraCotacaoPedido.CRUDAdicionar.visible(true);
    _regraCotacaoPedido.CRUDCancelar.visible(true);
    _regraCotacaoPedido.CRUDAtualizar.visible(false);
    _regraCotacaoPedido.CRUDExcluir.visible(false);
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
    var kout = _regraCotacaoPedido[prop];
    var listaRegrasAtualizada = [];
    var listaRegras = kout.val();

    var BuscaRegraPorCodigo = function (codigo) {
        for (var i in listaRegras)
            if (listaRegras[i].Codigo == codigo)
                return listaRegras[i];

        return null;
    }

    $("#" + _regraCotacaoPedido[prop].idGrid + " table tbody tr").each(function (i) {
        var regra = BuscaRegraPorCodigo($(this).data('codigo'));
        regra.Ordem = i + 1;
        listaRegrasAtualizada.push(regra);
    });

    kout.val(listaRegrasAtualizada);
}

function RenderizarGridRegras(prop, grid) {
    var html = "";
    var kout = _regraCotacaoPedido["Alcadas" + prop];
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
    var kout = _regraCotacaoPedido["Alcadas" + prop];
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