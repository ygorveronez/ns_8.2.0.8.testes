/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizacao.js" />
/// <reference path="../../Enumeradores/EnumTipoAprovadorRegra.js" />
/// <reference path="../../Enumeradores/EnumTipoPropriedadeAlcada.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAprovadores;
var _gridRegrasAutorizacao;
var _pesquisaRegraAutorizacao;
var _regrasAutorizacao;

/*
 * Declaração de Objetos Globais de Configuração do Arquivo 
 * 
 * - Para adicionar uma nova alçada, adicionar na lista 'Alcadas' do objeto '_configuracaoRegras'.
 * - Para utilizar o arquivo em outro cadastro de regra, alterar o nome do controller no objeto '_nomeControllerRegrasAutorizacao'.
 */

var _configuracaoRegras = {
    NumeroAprovadores: 1,
    infoTable: "Mova as linhas conforme a prioridade",
    Alcadas: [
        {
            descricao: "Transportador",
            prop: "Transportador",
            tipoPropriedadeAlcada: EnumTipoPropriedadeAlcada.Entidade,
            busca: function (ko) {
                new BuscarTransportadores(ko);
            }
        },
        {
            descricao: "Filial",
            prop: "Filial",
            tipoPropriedadeAlcada: EnumTipoPropriedadeAlcada.MultiplasEntidades,
            busca: function (ko) {
                new BuscarFilial(ko);
            }
        },
    ]
};

var _nomeControllerRegrasAutorizacao = "RegraAutorizacaoRecusaCheckin";

/*
 * Declaração das Classes
 */

var PesquisaRegrasAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, val: ko.observable(""), def: "" });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Aprovador = PropertyEntity({ text: "Aprovador:", issue: 930, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe });

    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegrasAutorizacao.CarregarGrid();
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
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: "Número de Aprovadores: ", issue: 873, getType: typesKnockout.int, enable: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });
    this.Observacoes = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000 });
    this.PrioridadeAprovacao = PropertyEntity({ val: ko.observable(EnumPrioridadeAutorizacao.Zero), options: EnumPrioridadeAutorizacao.obterOpcoes(), def: EnumPrioridadeAutorizacao.Zero, text: "*Prioridade: " });
    this.TipoAprovadorRegra = PropertyEntity({ val: ko.observable(EnumTipoAprovadorRegra.Usuario), options: EnumTipoAprovadorRegra.obterOpcoesPorSetor(), def: EnumTipoAprovadorRegra.Usuario, text: "*Tipo do Aprovador: ", visible: ko.observable(false) });

    this.AdicionarAprovador = PropertyEntity({ text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });

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

        this["Condicao" + alcada.prop] = PropertyEntity({ type: types.local, text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: (alcada.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.Entidade || alcada.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.MultiplasEntidades) ? EnumCondicaoAutorizacaoEntidade.obterOpcoes() : EnumCondicaoAutorizacao.obterOpcoes(), def: EnumCondicaoAutorizao.IgualA });
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

            case EnumTipoPropriedadeAlcada.MultiplasEntidades:
                this[alcada.prop] = PropertyEntity({ text: "*" + alcada.descricao + ":", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
                break;
        }
    }, this);

    this.TipoAprovadorRegra.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoAprovadorRegra.Transportador) {
            self.NumeroAprovadores.val("1");
            self.NumeroAprovadores.enable(false);
        }
        else
            self.NumeroAprovadores.enable(true);
    });

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

    _gridAprovadores = new BasicDataTable(_regrasAutorizacao.AdicionarAprovador.idGrid, header, menuOpcoes, null, null, _configuracaoRegras.NumeroAprovadores);
    _gridAprovadores.CarregarGrid([]);
}

function loadRegrasAutorizacao() {
    _regrasAutorizacao = new RegrasAutorizacao();
    _regrasAutorizacao.Alcadas.val(_configuracaoRegras.Alcadas);

    KoBindings(_regrasAutorizacao, "knockoutCadastroRegrasAutorizacao");

    _configuracaoRegras.Alcadas.forEach(function (alcada) {
        if ((alcada.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.Entidade) || (alcada.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.MultiplasEntidades))
            alcada.busca(_regrasAutorizacao[alcada.prop]);

        alcada.grid = new GridReordering(_configuracaoRegras.infoTable, _regrasAutorizacao["Alcadas" + alcada.prop].idGrid, obterCabecalhoTabela(alcada.descricao));
        alcada.grid.CarregarGrid();

        $("#" + _regrasAutorizacao["Alcadas" + alcada.prop].idGrid).on('sortstop', function () {
            reordenarLinhasGrid("Alcadas" + alcada.prop);
        });

        _regrasAutorizacao["Alcadas" + alcada.prop].val.subscribe(function () {
            renderizarGridRegras(alcada.prop, alcada.tipoPropriedadeAlcada, alcada.grid);
            atualizaListaAlcadas(alcada.prop);
        });
    });

    _pesquisaRegraAutorizacao = new PesquisaRegrasAutorizacao();
    KoBindings(_pesquisaRegraAutorizacao, "knockoutPesquisaRegrasAutorizacao", false, _pesquisaRegraAutorizacao.Pesquisar.id);

    HeaderAuditoria(_nomeControllerRegrasAutorizacao, _regrasAutorizacao);

    loadGridAprovadores();

    new BuscarFuncionario(_regrasAutorizacao.AdicionarAprovador, AdicionarAprovadorRetorno, _gridAprovadores);
    new BuscarFuncionario(_pesquisaRegraAutorizacao.Aprovador);

    buscarRegrasAutorizacao();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        $("#liRegrasFilial").hide();
        $("#liRegrasTransportador").hide();
    }
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

function cancelarClick() {
    limparTodosCampos();
}

function editarRegraAprovacao(data) {
    limparTodosCampos();

    _regrasAutorizacao.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regrasAutorizacao, _nomeControllerRegrasAutorizacao + "/BuscarPorCodigo", function (arg) {
        _pesquisaRegraAutorizacao.ExibirFiltros.visibleFade(false);

        _gridAprovadores.CarregarGrid(arg.Data.Aprovadores);

        _regrasAutorizacao.CRUDAdicionar.visible(false);
        _regrasAutorizacao.CRUDCancelar.visible(true);
        _regrasAutorizacao.CRUDAtualizar.visible(true);
        _regrasAutorizacao.CRUDExcluir.visible(true);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as regras?", function () {
        ExcluirPorCodigo(_regrasAutorizacao, _nomeControllerRegrasAutorizacao + "/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegrasAutorizacao.CarregarGrid();
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

    var listaRegrasAdicionar = obterListaObjetoAlcadaPorTipoPropriedadeAlcada(prop);
    var listaRegras = obterRegrasOrdenadas(_regrasAutorizacao["Alcadas" + prop]);

    if (listaRegrasAdicionar.length == 1) {
        if (isRegraDuplicada(listaRegras, listaRegrasAdicionar[0]))
            return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

        listaRegras.push(listaRegrasAdicionar[0]);
    }
    else {
        listaRegrasAdicionar.forEach(function (regraAdicionar) {
            if (!isRegraDuplicada(listaRegras, regraAdicionar))
                listaRegras.push(regraAdicionar);
        });
    }

    _regrasAutorizacao["Alcadas" + prop].val(listaRegras);

    limparCamposAlcada(prop);
}

function atualizarAlcadaClick(prop) {
    if (!validarCamposAlcada(prop))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var listaRegrasAtualizar = obterListaObjetoAlcadaPorTipoPropriedadeAlcada(prop);
    var listaRegras = obterRegrasOrdenadas(_regrasAutorizacao["Alcadas" + prop]);

    if (listaRegrasAtualizar.length == 1) {
        var regra = listaRegrasAtualizar[0];

        if (isRegraDuplicada(listaRegras, regra))
            return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

        for (var i in listaRegras) {
            if (listaRegras[i].Codigo == regra.Codigo) {
                listaRegras[i] = regra;
                break;
            }
        }
    }
    else {
        listaRegrasAtualizar.forEach(function (regraAtualizar) {
            if (!isRegraDuplicada(listaRegras, regraAtualizar))
                listaRegras.push(regraAtualizar);
        })
    }

    _regrasAutorizacao["Alcadas" + prop].val(listaRegras);

    limparCamposAlcada(prop);
}

function cancelarAlcadaClick(prop) {
    limparCamposAlcada(prop);
}

function editarRegraAlcadaClick(prop, codigo) {
    var listaRegras = _regrasAutorizacao["Alcadas" + prop].val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _regrasAutorizacao["Codigo" + prop].val(regra.Codigo);
        _regrasAutorizacao["Ordem" + prop].val(regra.Ordem);
        _regrasAutorizacao["Condicao" + prop].val(regra.Condicao);
        _regrasAutorizacao["Juncao" + prop].val(regra.Juncao);

        if (regra.Entidade) {
            var knoutEntidade = _regrasAutorizacao[prop];

            if (knoutEntidade.type == types.multiplesEntities) {
                var multiplaEntidade = new Object();

                multiplaEntidade[knoutEntidade.multiplesEntitiesConfig.propCodigo] = regra.Entidade.Codigo;
                multiplaEntidade[knoutEntidade.multiplesEntitiesConfig.propDescricao] = regra.Entidade.Descricao;

                knoutEntidade.multiplesEntities([multiplaEntidade]);
            }
            else {
                _regrasAutorizacao[prop].val(regra.Entidade.Descricao);
                _regrasAutorizacao[prop].codEntity(regra.Entidade.Codigo);
            }
        }
        else
            _regrasAutorizacao[prop].val(regra.Valor);

        _regrasAutorizacao.Adicionar.visible(false);
        _regrasAutorizacao.Atualizar.visible(true);
        _regrasAutorizacao.Excluir.visible(true);
        _regrasAutorizacao.Cancelar.visible(true);
    }
}

function excluirAlcadaClick(prop) {
    var codigo = _regrasAutorizacao["Codigo" + prop].val();

    excluirAlcada(prop, codigo);

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

    _gridRegrasAutorizacao = new GridView(_pesquisaRegraAutorizacao.Pesquisar.idGrid, _nomeControllerRegrasAutorizacao + "/Pesquisa", _pesquisaRegraAutorizacao, menuOpcoes);
    _gridRegrasAutorizacao.CarregarGrid();
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function limparTodosCampos() {
    LimparCampos(_regrasAutorizacao);

    _configuracaoRegras.Alcadas.forEach(function (alcada) {
        limparCamposAlcada(alcada.prop);
        _regrasAutorizacao["Alcadas" + alcada.prop].val([]);
    });

    _gridAprovadores.CarregarGrid([]);

    $("#myTab li:first a").click();

    _regrasAutorizacao.CRUDAdicionar.visible(true);
    _regrasAutorizacao.CRUDCancelar.visible(true);
    _regrasAutorizacao.CRUDAtualizar.visible(false);
    _regrasAutorizacao.CRUDExcluir.visible(false);
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
    return JSON.stringify(_gridAprovadores.BuscarRegistros());
}

function obterObjetoRegraSalvar() {
    var dados = {
        Codigo: _regrasAutorizacao.Codigo.val(),
        Descricao: _regrasAutorizacao.Descricao.val(),
        Vigencia: _regrasAutorizacao.Vigencia.val(),
        NumeroAprovadores: _regrasAutorizacao.NumeroAprovadores.val(),
        Observacoes: _regrasAutorizacao.Observacoes.val(),
        PrioridadeAprovacao: _regrasAutorizacao.PrioridadeAprovacao.val(),
        TipoAprovadorRegra: _regrasAutorizacao.TipoAprovadorRegra.val(),
        Status: _regrasAutorizacao.Status.val(),
        Aprovadores: obterListaAprovadores()
    };

    _configuracaoRegras.Alcadas.forEach(function (alcada) {
        dados["UsarRegraPor" + alcada.prop] = _regrasAutorizacao["UsarRegraPor" + alcada.prop].val();
        dados["Alcadas" + alcada.prop] = JSON.stringify(_regrasAutorizacao["Alcadas" + alcada.prop].val());
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
    var kout = _regrasAutorizacao["Alcadas" + prop];
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
            case EnumTipoPropriedadeAlcada.MultiplasEntidades:
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
    var kout = _regrasAutorizacao[prop];
    var listaRegrasAtualizada = [];
    var listaRegras = kout.val();

    var BuscaRegraPorCodigo = function (codigo) {
        for (var i in listaRegras)
            if (listaRegras[i].Codigo == codigo)
                return listaRegras[i];

        return null;
    }

    $("#" + _regrasAutorizacao[prop].idGrid + " table tbody tr").each(function (i) {
        var regra = BuscaRegraPorCodigo($(this).data('codigo'));
        regra.Ordem = i + 1;
        listaRegrasAtualizada.push(regra);
    });

    kout.val(listaRegrasAtualizada);
}

function salvarRegras(url, mensagemSucesso) {
    listaMensagemErros = validarRegras();

    if (listaMensagemErros.length > 0)
        return exibirMensagem(tipoMensagem.atencao, "Regra(s) inválida(s)", listaMensagemErros.join("<br>"));

    if (!ValidarCamposObrigatorios(_regrasAutorizacao))
        return exibirMensagemCamposObrigatorio();

    executarReST(url, obterObjetoRegraSalvar(), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                _gridRegrasAutorizacao.CarregarGrid();
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

    if ((_regrasAutorizacao.TipoAprovadorRegra.val() == EnumTipoAprovadorRegra.Usuario) && (_gridAprovadores.BuscarRegistros().length < _regrasAutorizacao.NumeroAprovadores.val()))
        return ["O número de aprovadores selecionados deve ser maior ou igual a " + _regrasAutorizacao.NumeroAprovadores.val()];

    _configuracaoRegras.Alcadas.forEach(function (alcada) {
        if (_regrasAutorizacao["UsarRegraPor" + alcada.prop].val() && _regrasAutorizacao["Alcadas" + alcada.prop].val().length == 0)
            listaMensagemErros.push("Nenhuma regra por " + alcada.descricao + " cadastrada.");
    });

    return listaMensagemErros;
}

/*
 * Declaração das Funções das Alçadas
 */

function atualizaListaAlcadas(prop) {
    var kout = _regrasAutorizacao["Alcadas" + prop];
    var listaRegras = obterRegrasOrdenadas(kout)

    kout.list = JSON.stringify(listaRegras);
}

function excluirAlcada(prop, codigo) {
    var listaRegras = obterRegrasOrdenadas(_regrasAutorizacao["Alcadas" + prop]);
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

    _regrasAutorizacao["Alcadas" + prop].val(listaRegras);
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
    _regrasAutorizacao["Codigo" + prop].val(_regrasAutorizacao["Codigo" + prop].def);
    _regrasAutorizacao["Ordem" + prop].val(_regrasAutorizacao["Ordem" + prop].def);
    _regrasAutorizacao["Condicao" + prop].val(_regrasAutorizacao["Condicao" + prop].def);
    _regrasAutorizacao["Juncao" + prop].val(_regrasAutorizacao["Juncao" + prop].def);

    LimparCampo(_regrasAutorizacao[prop]);

    _regrasAutorizacao.Adicionar.visible(true);
    _regrasAutorizacao.Atualizar.visible(false);
    _regrasAutorizacao.Excluir.visible(false);
    _regrasAutorizacao.Cancelar.visible(false);
}

function obterObjetoAlcada(prop) {
    var config = obterConfiguracaoRegraPorPropriedade(prop);

    var codigo = _regrasAutorizacao["Codigo" + prop].val();
    var ordem = _regrasAutorizacao["Ordem" + prop].val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : config.grid.ObterOrdencao().length + 1,
        Juncao: _regrasAutorizacao["Juncao" + prop].val(),
        Condicao: _regrasAutorizacao["Condicao" + prop].val()
    };

    switch (config.tipoPropriedadeAlcada) {
        case EnumTipoPropriedadeAlcada.Decimal:
            regra.Valor = Globalize.parseFloat(_regrasAutorizacao[prop].val()) || 0;
            break;

        case EnumTipoPropriedadeAlcada.Entidade:
            regra.Entidade = {
                Codigo: parseInt(_regrasAutorizacao[prop].codEntity()),
                Descricao: _regrasAutorizacao[prop].val()
            };
            break;

        case EnumTipoPropriedadeAlcada.Inteiro:
            regra.Valor = Globalize.parseInt(_regrasAutorizacao[prop].val()) || 0;
            break;
    }

    return regra;
}

function obterListaObjetoAlcada(prop, config) {
    var codigo = _regrasAutorizacao["Codigo" + prop].val();
    var ordem = _regrasAutorizacao["Ordem" + prop].val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : config.grid.ObterOrdencao().length + 1,
        Juncao: _regrasAutorizacao["Juncao" + prop].val(),
        Condicao: _regrasAutorizacao["Condicao" + prop].val()
    };

    switch (config.tipoPropriedadeAlcada) {
        case EnumTipoPropriedadeAlcada.Decimal:
            regra.Valor = Globalize.parseFloat(_regrasAutorizacao[prop].val()) || 0;
            break;

        case EnumTipoPropriedadeAlcada.Entidade:
            regra.Entidade = {
                Codigo: parseInt(_regrasAutorizacao[prop].codEntity()),
                Descricao: _regrasAutorizacao[prop].val()
            };
            break;

        case EnumTipoPropriedadeAlcada.Inteiro:
            regra.Valor = Globalize.parseInt(_regrasAutorizacao[prop].val()) || 0;
            break;
    }

    return [regra];
}

function obterListaObjetoAlcadaPorMultiplasEntidades(prop, config) {
    var listaRegra = new Array();
    var knoutMultiplaEntidade = _regrasAutorizacao[prop];
    var entidades = knoutMultiplaEntidade.multiplesEntities();
    var codigo = _regrasAutorizacao["Codigo" + prop].val();
    var ordem = _regrasAutorizacao["Ordem" + prop].val();

    if (codigo && (entidades.length > 1)) {
        excluirAlcada(prop, codigo);

        codigo = 0;
        ordem = 0;
    }

    ordem = ordem != 0 ? ordem : config.grid.ObterOrdencao().length + 1;

    entidades.forEach(function (entidade) {
        var regra = {
            Codigo: codigo ? codigo : guid(),
            Ordem: ordem,
            Juncao: _regrasAutorizacao["Juncao" + prop].val(),
            Condicao: _regrasAutorizacao["Condicao" + prop].val(),
            Entidade: {
                Codigo: parseInt(entidade[knoutMultiplaEntidade.multiplesEntitiesConfig.propCodigo]),
                Descricao: entidade[knoutMultiplaEntidade.multiplesEntitiesConfig.propDescricao]
            }
        };

        listaRegra.push(regra);

        ordem++;
    });

    return listaRegra;
}

function obterListaObjetoAlcadaPorTipoPropriedadeAlcada(prop) {
    var config = obterConfiguracaoRegraPorPropriedade(prop);

    if (config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.MultiplasEntidades)
        return obterListaObjetoAlcadaPorMultiplasEntidades(prop, config);

    return obterListaObjetoAlcada(prop, config);
}

function validarCamposAlcada(prop) {
    var valido = true;
    var config = obterConfiguracaoRegraPorPropriedade(prop);
    var regra = _regrasAutorizacao[prop];

    regra.required = true;

    if (((config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.Entidade) || (config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.MultiplasEntidades)) && !ValidarCampoObrigatorioEntity(regra))
        valido = false;
    else if (!ValidarCampoObrigatorioMap(regra))
        valido = false;

    regra.required = false;

    return valido;
}
