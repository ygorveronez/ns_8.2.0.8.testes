/// <reference path="../../Enumeradores/EnumCondicaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizacao.js" />
/// <reference path="../../Enumeradores/EnumTipoPropriedadeAlcada.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAprovadores;
var _gridRegrasAprovacao;
var _pesquisaRegraAprovacao;
var _regrasAprovacao;

/*
 * Declaração de Objetos Globais de Configuração do Arquivo 
 * 
 * - Para adicionar uma nova alçada, adicionar na lista 'Alcadas' do objeto '_configuracaoRegras'.
 * - Para utilizar o arquivo em outro cadastro de regra, alterar o nome do controller no objeto '_nomeControllerRegrasAutorizacao'.
 */

var _alcadas = [
    {
        descricao: "Tempo Excedido",
        prop: "SolicitacaoGasData",
        tipoPropriedadeAlcada: EnumTipoPropriedadeAlcada.Inteiro
    }
];

var _configuracaoRegras = {
    NumeroAprovadores: 1,
    infoTable: "Mova as linhas conforme a prioridade",
    Alcadas: _alcadas
};

var _nomeControllerRegrasAutorizacao = "RegraAprovacaoSolicitacaoGas";

/*
 * Declaração das Classes
 */

var PesquisaRegrasAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, val: ko.observable(""), def: "" });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Aprovador = PropertyEntity({ text: "Aprovador:", issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegrasAprovacao.CarregarGrid();
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

var RegrasAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: "Número de Aprovadores: ", issue: 873, getType: typesKnockout.int });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });
    this.Observacoes = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000 });
    this.PrioridadeAprovacao = PropertyEntity({ val: ko.observable(EnumPrioridadeAutorizacao.Zero), options: EnumPrioridadeAutorizacao.obterOpcoes(), def: EnumPrioridadeAutorizacao.Zero, text: "*Prioridade: " });

    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: obterListaAprovadores, idGrid: guid() });
    this.AdicionarAprovador = PropertyEntity({ text: "Adicionar", idBtnSearch: guid() });

    this.Alcadas = PropertyEntity({ type: types.local, val: ko.observableArray([]), changeAba: cancelarAlcadaClick });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAlcadaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarAlcadaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirAlcadaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAlcadaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    _configuracaoRegras.Alcadas.forEach(function (alcada) {
        this["UsarRegraPor" + alcada.prop] = PropertyEntity({ getType: typesKnockout.bool, idFade: guid(), text: "Ativar Regra de autorização por " + alcada.descricao + ":", val: ko.observable(false), def: false });
        this["Alcadas" + alcada.prop] = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: [], idGrid: guid(), list: [], val: ko.observable([]) });

        this["Codigo" + alcada.prop] = PropertyEntity({ type: types.local, val: ko.observable(""), def: "", getType: typesKnockout.string });
        this["Ordem" + alcada.prop] = PropertyEntity({ type: types.local, val: ko.observable(0), def: 0, getType: typesKnockout.int });

        this["Condicao" + alcada.prop] = PropertyEntity({ type: types.local, text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: alcada.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.Entidade ? EnumCondicaoAutorizacaoEntidade.obterOpcoes() : EnumCondicaoAutorizacao.obterOpcoes(), def: EnumCondicaoAutorizao.IgualA });
        this["Juncao" + alcada.prop] = PropertyEntity({ type: types.local, text: "Junção: ", val: ko.observable(EnumJuncaoAutorizao.E), options: EnumJuncaoAutorizacao.obterOpcoes(), def: EnumJuncaoAutorizao.E });

        switch (alcada.tipoPropriedadeAlcada) {
            case EnumTipoPropriedadeAlcada.Decimal:
                this[alcada.prop] = PropertyEntity({ text: "*" + alcada.descricao + ":", type: types.map, getType: typesKnockout.decimal, def: "0,00" });
                break;

            case EnumTipoPropriedadeAlcada.Entidade:
                this[alcada.prop] = PropertyEntity({ text: "*" + alcada.descricao + ":", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
                break;

            case EnumTipoPropriedadeAlcada.Inteiro:
                this[alcada.prop] = PropertyEntity({ text: "*" + alcada.descricao + ":", type: types.map, getType: typesKnockout.int, def: "0" });
                break;
        }
    }, this);

    this.CRUDAdicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.CRUDCancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.CRUDAtualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.CRUDExcluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAprovadores() {
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
                    removerAprovadorClick(data);
                }
            }
        ]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: "Usuário", width: "100%", className: "text-align-left" }
    ];

    _gridAprovadores = new BasicDataTable(_regrasAprovacao.Aprovadores.idGrid, header, menuOpcoes, null, null, _configuracaoRegras.NumeroAprovadores);
    _gridAprovadores.CarregarGrid([]);
}

function loadRegrasAutorizacao() {
    _regrasAprovacao = new RegrasAutorizacao();
    _regrasAprovacao.Alcadas.val(_configuracaoRegras.Alcadas);

    KoBindings(_regrasAprovacao, "knockoutCadastroRegrasAutorizacao");

    _configuracaoRegras.Alcadas.forEach(function (alcada) {
        if (alcada.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.Entidade)
            alcada.busca(_regrasAprovacao[alcada.prop]);

        alcada.grid = new GridReordering(_configuracaoRegras.infoTable, _regrasAprovacao["Alcadas" + alcada.prop].idGrid, obterCabecalhoTabela(alcada.descricao));
        alcada.grid.CarregarGrid();

        $("#" + _regrasAprovacao["Alcadas" + alcada.prop].idGrid).on('sortstop', function () {
            reordenarLinhasGrid("Alcadas" + alcada.prop);
        });

        _regrasAprovacao["Alcadas" + alcada.prop].val.subscribe(function () {
            renderizarGridRegras(alcada.prop, alcada.tipoPropriedadeAlcada, alcada.grid);
            atualizaListaAlcadas(alcada.prop);
        });
    });

    _pesquisaRegraAprovacao = new PesquisaRegrasAutorizacao();
    KoBindings(_pesquisaRegraAprovacao, "knockoutPesquisaRegrasAutorizacao", false, _pesquisaRegraAprovacao.Pesquisar.id);

    HeaderAuditoria(_nomeControllerRegrasAutorizacao, _regrasAprovacao);

    loadGridAprovadores();

    new BuscarFuncionario(_regrasAprovacao.AdicionarAprovador, AdicionarAprovadorRetorno, _gridAprovadores);
    new BuscarFuncionario(_pesquisaRegraAprovacao.Aprovador);

    buscarRegrasAutorizacao();
}

/*
 * Declaração das Funções Associadas a Eventos das Regras
 */

function adicionarClick(e, sender) {
    salvarRegras(_nomeControllerRegrasAutorizacao + "/Adicionar", "Cadastrado com sucesso.", e, sender);
}

function atualizarClick(e, sender) {
    salvarRegras(_nomeControllerRegrasAutorizacao + "/Atualizar", "Atualizado com sucesso.", e, sender);
}

function cancelarClick(e, sender) {
    limparTodosCampos();
}

function editarRegraAprovacao(data) {
    limparTodosCampos();

    _regrasAprovacao.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regrasAprovacao, _nomeControllerRegrasAutorizacao + "/BuscarPorCodigo", function (arg) {
        _pesquisaRegraAprovacao.ExibirFiltros.visibleFade(false);

        _regrasAprovacao.Aprovadores.val(arg.Data.Aprovadores);

        _regrasAprovacao.CRUDAdicionar.visible(false);
        _regrasAprovacao.CRUDCancelar.visible(true);
        _regrasAprovacao.CRUDAtualizar.visible(true);
        _regrasAprovacao.CRUDExcluir.visible(true);
    }, null);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as regras?", function () {
        ExcluirPorCodigo(_regrasAprovacao, _nomeControllerRegrasAutorizacao + "/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegrasAprovacao.CarregarGrid();
                    limparTodosCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function removerAprovadorClick(data) {
    var dataGrid = _gridAprovadores.BuscarRegistros().slice();

    dataGrid = dataGrid.filter(function (a) {
        return a.Codigo != data.Codigo;
    });

    _gridAprovadores.CarregarGrid(dataGrid);
}

/*
 * Declaração das Funções Associadas a Eventos das Alçadas
 */

function adicionarAlcadaClick(prop) {
    if (!validarCamposAlcada(prop))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var regra = obterObjetoAlcada(prop);
    var listaRegras = obterRegrasOrdenadas(_regrasAprovacao["Alcadas" + prop]);

    if (isRegraDuplicada(listaRegras, regra))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);

    _regrasAprovacao["Alcadas" + prop].val(listaRegras);

    limparCamposAlcada(prop);
}

function atualizarAlcadaClick(prop) {
    if (!validarCamposAlcada(prop))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var regra = obterObjetoAlcada(prop);
    var codigo = _regrasAprovacao["Codigo" + prop].val();
    var listaRegras = obterRegrasOrdenadas(_regrasAprovacao["Alcadas" + prop]);

    if (isRegraDuplicada(listaRegras, regra))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            listaRegras[i] = regra;
            break;
        }
    }

    _regrasAprovacao["Alcadas" + prop].val(listaRegras);

    limparCamposAlcada(prop);
}

function cancelarAlcadaClick(prop) {
    limparCamposAlcada(prop);
}

function editarRegraAlcadaClick(prop, codigo) {
    var listaRegras = _regrasAprovacao["Alcadas" + prop].val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _regrasAprovacao["Codigo" + prop].val(regra.Codigo);
        _regrasAprovacao["Ordem" + prop].val(regra.Ordem);
        _regrasAprovacao["Condicao" + prop].val(regra.Condicao);
        _regrasAprovacao["Juncao" + prop].val(regra.Juncao);

        if (regra.Entidade) {
            _regrasAprovacao[prop].val(regra.Entidade.Descricao);
            _regrasAprovacao[prop].codEntity(regra.Entidade.Codigo);
        }
        else
            _regrasAprovacao[prop].val(regra.Valor);

        _regrasAprovacao.Adicionar.visible(false);
        _regrasAprovacao.Atualizar.visible(true);
        _regrasAprovacao.Excluir.visible(true);
        _regrasAprovacao.Cancelar.visible(true);
    }
}

function excluirAlcadaClick(prop) {
    var listaRegras = obterRegrasOrdenadas(_regrasAprovacao["Alcadas" + prop]);
    var codigo = _regrasAprovacao["Codigo" + prop].val();
    var index = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            index = parseInt(i);
            break;
        }
    }

    listaRegras.splice(index, 1);

    for (i = 1; i <= listaRegras.length; i++)
        listaRegras[i - 1].Ordem = i;

    _regrasAprovacao["Alcadas" + prop].val(listaRegras);

    limparCamposAlcada(prop);
}

/*
 * Declaração das Funções das Regras de Autorização
 */

function AdicionarAprovadorRetorno(data) {
    if (data != null) {
        if (!$.isArray(data))
            data = [data];

        var dataGrid = _gridAprovadores.BuscarRegistros();

        data = data.map(function (a) {
            return {
                Codigo: a.Codigo,
                Nome: a.Nome,
            };
        });

        dataGrid = dataGrid.concat(data);

        _gridAprovadores.CarregarGrid(dataGrid);
    }
}

function buscarRegrasAutorizacao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraAprovacao, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();

    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegrasAprovacao = new GridView(_pesquisaRegraAprovacao.Pesquisar.idGrid, _nomeControllerRegrasAutorizacao + "/Pesquisa", _pesquisaRegraAprovacao, menuOpcoes);
    _gridRegrasAprovacao.CarregarGrid();
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function limparTodosCampos() {
    LimparCampos(_regrasAprovacao);

    _configuracaoRegras.Alcadas.forEach(function (alcada) {
        limparCamposAlcada(alcada.prop);
        _regrasAprovacao["Alcadas" + alcada.prop].val([]);
    });

    _gridAprovadores.CarregarGrid([]);

    $("#myTab li:first a").click();

    _regrasAprovacao.CRUDAdicionar.visible(true);
    _regrasAprovacao.CRUDCancelar.visible(true);
    _regrasAprovacao.CRUDAtualizar.visible(false);
    _regrasAprovacao.CRUDExcluir.visible(false);
}

function obterCabecalhoTabela(nomeCampo) {
    return '<tr>' +
        '<th width="15%" class="text-align-center">Ordem</th>' +
        '<th width="30%" class="text-align-center">Junção</th>' +
        '<th width="30%" class="text-align-center">Condição</th>' +
        '<th width="40%" class="text-align-left">' + nomeCampo + '</th>' +
        '<th width="15%" class="text-align-center">Editar</th>' +
        '</tr>';
}

function obterConfiguracaoRegraPorPropriedade(prop) {
    var config = null;

    for (var i in _configuracaoRegras.Alcadas) {
        if (_configuracaoRegras.Alcadas[i].prop == prop) {
            config = _configuracaoRegras.Alcadas[i];
            break;
        }
    }

    return config;
}

function obterListaAprovadores() {
    if (arguments.length > 0 && arguments[0] != "")
        _gridAprovadores.CarregarGrid(arguments[0]);
    else
        return JSON.stringify(_gridAprovadores.BuscarRegistros());
}

function obterObjetoRegraSalvar() {
    var dados = {
        Codigo: _regrasAprovacao.Codigo.val(),
        Descricao: _regrasAprovacao.Descricao.val(),
        Vigencia: _regrasAprovacao.Vigencia.val(),
        NumeroAprovadores: _regrasAprovacao.NumeroAprovadores.val(),
        Observacoes: _regrasAprovacao.Observacoes.val(),
        PrioridadeAprovacao: _regrasAprovacao.PrioridadeAprovacao.val(),
        Status: _regrasAprovacao.Status.val(),
        Aprovadores: obterListaAprovadores(),
    };

    _configuracaoRegras.Alcadas.forEach(function (alcada) {
        dados["UsarRegraPor" + alcada.prop] = _regrasAprovacao["UsarRegraPor" + alcada.prop].val();
        dados["Alcadas" + alcada.prop] = JSON.stringify(_regrasAprovacao["Alcadas" + alcada.prop].val());
    });

    return dados;
}

function obterRegrasOrdenadas(kout) {
    var regras = kout.val().slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });

    return regras;
}

function renderizarGridRegras(prop, tipoPropriedadeAlcada, grid) {
    var html = "";
    var kout = _regrasAprovacao["Alcadas" + prop];
    var listaRegras = obterRegrasOrdenadas(kout)

    $.each(listaRegras, function (i, regra) {
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_regra_' + prop + '_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
        html += '<td>' + EnumJuncaoAutorizacao.obterDescricao(regra.Juncao) + '</td>';
        html += '<td>' + EnumCondicaoAutorizacao.obterDescricao(regra.Condicao) + '</td>';

        switch (tipoPropriedadeAlcada) {
            case EnumTipoPropriedadeAlcada.Decimal:
                html += '<td>' + Globalize.format(regra.Valor, "n2") + '</td>';
                break;

            case EnumTipoPropriedadeAlcada.Entidade:
                html += '<td>' + regra.Entidade.Descricao + '</td>';
                break;

            case EnumTipoPropriedadeAlcada.Inteiro:
                html += '<td>' + regra.Valor + '</td>';
                break;
        }

        html += '<td class="text-align-center"><a href="javascript:;" onclick="editarRegraAlcadaClick(\'' + prop + '\', \'' + regra.Codigo + '\')">Editar</a></td></tr>';
    });

    grid.RecarregarGrid(html);
}

function reordenarLinhasGrid(prop) {
    var kout = _regrasAprovacao[prop];
    var listaRegrasAtualizada = [];
    var listaRegras = kout.val();

    var BuscaRegraPorCodigo = function (codigo) {
        for (var i in listaRegras)
            if (listaRegras[i].Codigo == codigo)
                return listaRegras[i];

        return null;
    }

    $("#" + _regrasAprovacao[prop].idGrid + " table tbody tr").each(function (i) {
        var regra = BuscaRegraPorCodigo($(this).data('codigo'));
        regra.Ordem = i + 1;
        listaRegrasAtualizada.push(regra);
    });

    kout.val(listaRegrasAtualizada);
}

function salvarRegras(url, mensagemSucesso, e, sender) {
    listaMensagemErros = validarRegras();

    if (listaMensagemErros.length > 0)
        return exibirMensagem(tipoMensagem.atencao, "Regra(s) inválida(s)", listaMensagemErros.join("<br>"));

    if (!ValidarCamposObrigatorios(_regrasAprovacao))
        return exibirMensagemCamposObrigatorio();

    executarReST(url, obterObjetoRegraSalvar(), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                _gridRegrasAprovacao.CarregarGrid();
                limparTodosCampos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function validarRegras() {
    var listaMensagemErros = [];

    if (_gridAprovadores.BuscarRegistros().length < _regrasAprovacao.NumeroAprovadores.val())
        return ["O número de aprovadores selecionados deve ser maior ou igual a " + _regrasAprovacao.NumeroAprovadores.val()];

    _configuracaoRegras.Alcadas.forEach(function (alcada) {
        if (_regrasAprovacao["UsarRegraPor" + alcada.prop].val() && _regrasAprovacao["Alcadas" + alcada.prop].val().length == 0)
            listaMensagemErros.push("Nenhuma regra por " + alcada.descricao + " cadastrada.");
    });

    return listaMensagemErros;
}

/*
 * Declaração das Funções das Alçadas
 */

function atualizaListaAlcadas(prop) {
    var kout = _regrasAprovacao["Alcadas" + prop];
    var listaRegras = obterRegrasOrdenadas(kout)

    kout.list = JSON.stringify(listaRegras);
}

function isRegraDuplicada(listaRegras, regra) {
    var usarValor = "Valor" in regra;

    for (var i in listaRegras) {
        if (
            (listaRegras[i].Codigo != regra.Codigo) &&
            (listaRegras[i].Condicao == regra.Condicao) &&
            (listaRegras[i].Juncao == regra.Juncao) &&
            ((!usarValor && listaRegras[i].Entidade.Codigo == regra.Entidade.Codigo) || usarValor && listaRegras[i].Valor == regra.Valor)
        )
            return true;
    }

    return false;
}

function limparCamposAlcada(prop) {
    var config = obterConfiguracaoRegraPorPropriedade(prop);

    _regrasAprovacao["Codigo" + prop].val(_regrasAprovacao["Codigo" + prop].def);
    _regrasAprovacao["Ordem" + prop].val(_regrasAprovacao["Ordem" + prop].def);
    _regrasAprovacao["Condicao" + prop].val(_regrasAprovacao["Condicao" + prop].def);
    _regrasAprovacao["Juncao" + prop].val(_regrasAprovacao["Juncao" + prop].def);

    if (config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.Entidade)
        LimparCampoEntity(_regrasAprovacao[prop]);
    else
        _regrasAprovacao[prop].val(_regrasAprovacao[prop].def);

    _regrasAprovacao.Adicionar.visible(true);
    _regrasAprovacao.Atualizar.visible(false);
    _regrasAprovacao.Excluir.visible(false);
    _regrasAprovacao.Cancelar.visible(false);
}

function obterObjetoAlcada(prop) {
    var config = obterConfiguracaoRegraPorPropriedade(prop);

    var codigo = _regrasAprovacao["Codigo" + prop].val();
    var ordem = _regrasAprovacao["Ordem" + prop].val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : config.grid.ObterOrdencao().length + 1,
        Juncao: _regrasAprovacao["Juncao" + prop].val(),
        Condicao: _regrasAprovacao["Condicao" + prop].val(),

    };

    switch (config.tipoPropriedadeAlcada) {
        case EnumTipoPropriedadeAlcada.Decimal:
            regra.Valor = Globalize.parseFloat(_regrasAprovacao[prop].val()) || 0;
            break;

        case EnumTipoPropriedadeAlcada.Entidade:
            regra.Entidade = {
                Codigo: parseInt(_regrasAprovacao[prop].codEntity()),
                Descricao: _regrasAprovacao[prop].val()
            };
            break;

        case EnumTipoPropriedadeAlcada.Inteiro:
            regra.Valor = Globalize.parseInt(_regrasAprovacao[prop].val()) || 0;
            break;
    }

    return regra;
}

function validarCamposAlcada(prop) {
    var valido = true;
    var config = obterConfiguracaoRegraPorPropriedade(prop);
    var regra = _regrasAprovacao[prop];

    regra.required = true;

    if ((config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.Entidade) && !ValidarCampoObrigatorioEntity(regra))
        valido = false;
    else if (!ValidarCampoObrigatorioMap(regra))
        valido = false;

    regra.required = false;

    return valido;
}
