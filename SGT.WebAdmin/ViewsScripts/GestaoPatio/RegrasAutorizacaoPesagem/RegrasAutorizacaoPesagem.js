/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizacao.js" />
/// <reference path="../../Enumeradores/EnumTipoPropriedadeAlcada.js" />
/// <reference path="../../Enumeradores/EnumTipoRegraAutorizacaoToleranciaPesagem.js" />


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
            descricao: "Filial",
            prop: "Filial",
            tipoPropriedadeAlcada: EnumTipoPropriedadeAlcada.MultiplasEntidades,
            busca: function (ko) {
                new BuscarFilial(ko);
            },
            visivel: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS
        },
        {
            descricao: "Modelo Veicular",
            prop: "ModeloVeicularCarga",
            tipoPropriedadeAlcada: EnumTipoPropriedadeAlcada.MultiplasEntidades,
            busca: function (ko) {
                new BuscarModelosVeicularesCarga(ko);
            },
            visivel: true
        },
        {
            descricao: "Tipo de Carga",
            prop: "TipoCarga",
            tipoPropriedadeAlcada: EnumTipoPropriedadeAlcada.MultiplasEntidades,
            busca: function (ko) {
                new BuscarTiposdeCarga(ko);
            },
            visivel: true
        },
        {
            descricao: "Tipo de Operação",
            prop: "TipoOperacao",
            tipoPropriedadeAlcada: EnumTipoPropriedadeAlcada.MultiplasEntidades,
            busca: function (ko) {
                new BuscarTiposOperacao(ko);
            },
            visivel: true
        },
    ]
};

var _nomeControllerRegrasAutorizacao = "RegrasAutorizacaoPesagem";

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
            _gridRegrasAutorizacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

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

    this.CRUDAdicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.CRUDCancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.CRUDAtualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.CRUDExcluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });

    this.TipoRegraAutorizacaoToleranciaPesagem = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoRegraAutorizacaoToleranciaPesagem.Todos), options: EnumTipoRegraAutorizacaoToleranciaPesagem.obterOpcoes(), def: EnumTipoRegraAutorizacaoToleranciaPesagem.Peso });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAprovadores() {
    let menuOpcoes = {
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

    let header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: "Usuário", width: "100%", className: "text-align-left" }
    ];

    _gridAprovadores = new BasicDataTable(_regrasAutorizacao.Aprovadores.idGrid, header, menuOpcoes, null, null, _configuracaoRegras.NumeroAprovadores);
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

        _regrasAutorizacao.Aprovadores.val(arg.Data.Aprovadores);
        _regrasAutorizacao.TipoRegraAutorizacaoToleranciaPesagem.val(arg.Data.TipoRegraAutorizacaoToleranciaPesagem);

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
    let dataGrid = _gridAprovadores.BuscarRegistros().slice();

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

    let listaRegrasAdicionar = obterListaObjetoAlcadaPorTipoPropriedadeAlcada(prop);
    let listaRegras = obterRegrasOrdenadas(_regrasAutorizacao["Alcadas" + prop]);

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

    let listaRegrasAtualizar = obterListaObjetoAlcadaPorTipoPropriedadeAlcada(prop);
    let listaRegras = obterRegrasOrdenadas(_regrasAutorizacao["Alcadas" + prop]);

    if (listaRegrasAtualizar.length == 1) {
        let regra = listaRegrasAtualizar[0];

        if (isRegraDuplicada(listaRegras, regra))
            return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

        for (let i in listaRegras) {
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
    let listaRegras = _regrasAutorizacao["Alcadas" + prop].val();
    let regra = null;

    for (let i in listaRegras) {
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
            let knoutEntidade = _regrasAutorizacao[prop];

            if (knoutEntidade.type == types.multiplesEntities) {
                let multiplaEntidade = new Object();

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
    let codigo = _regrasAutorizacao["Codigo" + prop].val();

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

        let dataGrid = _gridAprovadores.BuscarRegistros();

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
    let editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraAprovacao, tamanho: "20", icone: "" };
    let menuOpcoes = new Object();

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
    let config = null;

    for (let i in _configuracaoRegras.Alcadas) {
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
    let dados = {
        Codigo: _regrasAutorizacao.Codigo.val(),
        Descricao: _regrasAutorizacao.Descricao.val(),
        Vigencia: _regrasAutorizacao.Vigencia.val(),
        NumeroAprovadores: _regrasAutorizacao.NumeroAprovadores.val(),
        Observacoes: _regrasAutorizacao.Observacoes.val(),
        PrioridadeAprovacao: _regrasAutorizacao.PrioridadeAprovacao.val(),
        Status: _regrasAutorizacao.Status.val(),
        Aprovadores: obterListaAprovadores(),
        TipoRegraAutorizacaoToleranciaPesagem: _regrasAutorizacao.TipoRegraAutorizacaoToleranciaPesagem.val()
    };

    _configuracaoRegras.Alcadas.forEach(function (alcada) {
        dados["UsarRegraPor" + alcada.prop] = _regrasAutorizacao["UsarRegraPor" + alcada.prop].val();
        dados["Alcadas" + alcada.prop] = JSON.stringify(_regrasAutorizacao["Alcadas" + alcada.prop].val());
    });

    return dados;
}

function obterRegrasOrdenadas(kout) {
    let regras = kout.val().slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });

    return regras;
}

function renderizarGridRegras(prop, tipoPropriedadeAlcada, grid) {
    let html = "";
    let kout = _regrasAutorizacao["Alcadas" + prop];
    let listaRegras = obterRegrasOrdenadas(kout)

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
    let kout = _regrasAutorizacao[prop];
    let listaRegrasAtualizada = [];
    let listaRegras = kout.val();

    let BuscaRegraPorCodigo = function (codigo) {
        for (let i in listaRegras)
            if (listaRegras[i].Codigo == codigo)
                return listaRegras[i];

        return null;
    }

    $("#" + _regrasAutorizacao[prop].idGrid + " table tbody tr").each(function (i) {
        let regra = BuscaRegraPorCodigo($(this).data('codigo'));
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
    let listaMensagemErros = [];

    if (_gridAprovadores.BuscarRegistros().length < _regrasAutorizacao.NumeroAprovadores.val())
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
    let kout = _regrasAutorizacao["Alcadas" + prop];
    let listaRegras = obterRegrasOrdenadas(kout)

    kout.list = JSON.stringify(listaRegras);
}

function excluirAlcada(prop, codigo) {
    let listaRegras = obterRegrasOrdenadas(_regrasAutorizacao["Alcadas" + prop]);
    let index = null;

    for (let i in listaRegras) {
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
    let usarValor = "Valor" in regra;

    for (let i in listaRegras) {
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
    let config = obterConfiguracaoRegraPorPropriedade(prop);

    let codigo = _regrasAutorizacao["Codigo" + prop].val();
    let ordem = _regrasAutorizacao["Ordem" + prop].val();
    let regra = {
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
    let codigo = _regrasAutorizacao["Codigo" + prop].val();
    let ordem = _regrasAutorizacao["Ordem" + prop].val();
    let regra = {
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
    let listaRegra = new Array();
    let knoutMultiplaEntidade = _regrasAutorizacao[prop];
    let entidades = knoutMultiplaEntidade.multiplesEntities();
    let codigo = _regrasAutorizacao["Codigo" + prop].val();
    let ordem = _regrasAutorizacao["Ordem" + prop].val();

    if (codigo && (entidades.length > 1)) {
        excluirAlcada(prop, codigo);

        codigo = 0;
        ordem = 0;
    }

    ordem = ordem != 0 ? ordem : config.grid.ObterOrdencao().length + 1;

    entidades.forEach(function (entidade) {
        let regra = {
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
    let config = obterConfiguracaoRegraPorPropriedade(prop);

    if (config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.MultiplasEntidades)
        return obterListaObjetoAlcadaPorMultiplasEntidades(prop, config);

    return obterListaObjetoAlcada(prop, config);
}

function validarCamposAlcada(prop) {
    let valido = true;
    let config = obterConfiguracaoRegraPorPropriedade(prop);
    let regra = _regrasAutorizacao[prop];

    regra.required = true;

    if (((config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.Entidade) || (config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.MultiplasEntidades)) && !ValidarCampoObrigatorioEntity(regra))
        valido = false;
    else if (!ValidarCampoObrigatorioMap(regra))
        valido = false;

    regra.required = false;

    return valido;
}
