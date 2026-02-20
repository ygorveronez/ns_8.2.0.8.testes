/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizacao.js" />
/// <reference path="../../Enumeradores/EnumTipoPropriedadeAlcada.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAprovadoresProvisaoPendente;
var _gridRegrasAutorizacaoProvisaoPendente;
var _pesquisaRegraAutorizacaoProvisaoPendente;
var _regrasAutorizacaoProvisaoPendente;

/*
 * Declaração de Objetos Globais de Configuração do Arquivo 
 * 
 * - Para adicionar uma nova alçada, adicionar na lista 'Alcadas' do objeto '_configuracaoRegrasProvisaoPendente'.
 * - Para utilizar o arquivo em outro cadastro de regra, alterar o nome do controller no objeto '_nomeControllerRegrasAutorizacaoProvisaoPendente'.
 */

var _configuracaoRegrasProvisaoPendente = {
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
            descricao: "Valor de Provisão",
            prop: "ValorProvisao",
            tipoPropriedadeAlcada: EnumTipoPropriedadeAlcada.Decimal,
            visivel: true
        }
    ]
};

var _nomeControllerRegrasAutorizacaoProvisaoPendente = "RegraAutorizacaoProvisaoPendente";

/*
 * Declaração das Classes
 */

var PesquisaRegrasAutorizacaoProvisaoPendente = function () {
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
            _gridRegrasAutorizacaoProvisaoPendente.CarregarGrid();
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

var RegrasAutorizacaoProvisaoPendente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: "Número de Aprovadores: ", issue: 873, getType: typesKnockout.int, enable: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });
    this.Observacoes = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000 });
    this.PrioridadeAprovacao = PropertyEntity({ val: ko.observable(EnumPrioridadeAutorizacao.Zero), options: EnumPrioridadeAutorizacao.obterOpcoes(), def: EnumPrioridadeAutorizacao.Zero, text: "*Prioridade: " });
    this.TipoGeracaoRegraProvisao = PropertyEntity({ val: ko.observable(EnumTipoGeracaoRegraProvisao.TermoQuitacao), options: EnumTipoGeracaoRegraProvisao.obterOpcoes(), def: EnumTipoGeracaoRegraProvisao.TermoQuitacao, text: "*Tipo Geração: "});

    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: obterListaAprovadoresProvisaoPendente, idGrid: guid() });
    this.AdicionarAprovador = PropertyEntity({ text: "Adicionar", idBtnSearch: guid() });

    this.Alcadas = PropertyEntity({ type: types.local, val: ko.observableArray([]), changeAba: cancelarAlcadaProvisaoPendenteClick });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAlcadaProvisaoPendenteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarAlcadaProvisaoPendenteClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirAlcadaProvisaoPendenteClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAlcadaProvisaoPendenteClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    _configuracaoRegrasProvisaoPendente.Alcadas.forEach(function (alcada) {
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

    this.CRUDAdicionar = PropertyEntity({ eventClick: adicionarProvisaoPendenteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.CRUDCancelar = PropertyEntity({ eventClick: cancelarProvisaoPendenteClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.CRUDAtualizar = PropertyEntity({ eventClick: atualizarProvisaoPendenteClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.CRUDExcluir = PropertyEntity({ eventClick: excluirProvisaoPendenteClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
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
                    removerAprovadorProvisaoPendenteClick(data);
                }
            }
        ]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: "Usuário", width: "100%", className: "text-align-left" }
    ];

    _gridAprovadoresProvisaoPendente = new BasicDataTable(_regrasAutorizacaoProvisaoPendente.Aprovadores.idGrid, header, menuOpcoes, null, null, _configuracaoRegrasProvisaoPendente.NumeroAprovadores);
    _gridAprovadoresProvisaoPendente.CarregarGrid([]);
}

function loadRegrasAutorizacaoProvisaoPendente() {
    _regrasAutorizacaoProvisaoPendente = new RegrasAutorizacaoProvisaoPendente();
    _regrasAutorizacaoProvisaoPendente.Alcadas.val(_configuracaoRegrasProvisaoPendente.Alcadas);

    KoBindings(_regrasAutorizacaoProvisaoPendente, "knockoutCadastroRegrasAutorizacao");

    _configuracaoRegrasProvisaoPendente.Alcadas.forEach(function (alcada) {
        if ((alcada.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.Entidade) || (alcada.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.MultiplasEntidades))
            alcada.busca(_regrasAutorizacaoProvisaoPendente[alcada.prop]);

        alcada.grid = new GridReordering(_configuracaoRegrasProvisaoPendente.infoTable, _regrasAutorizacaoProvisaoPendente["Alcadas" + alcada.prop].idGrid, obterCabecalhoTabelaProvisaoPendente(alcada.descricao));
        alcada.grid.CarregarGrid();

        $("#" + _regrasAutorizacaoProvisaoPendente["Alcadas" + alcada.prop].idGrid).on('sortstop', function () {
            reordenarLinhasGridProvisaoPendente("Alcadas" + alcada.prop);
        });

        _regrasAutorizacaoProvisaoPendente["Alcadas" + alcada.prop].val.subscribe(function () {
            renderizarGridRegrasProvisaoPendente(alcada.prop, alcada.tipoPropriedadeAlcada, alcada.grid);
            atualizaListaAlcadasProvisaoPendente(alcada.prop);
        });
    });

    _pesquisaRegraAutorizacaoProvisaoPendente = new PesquisaRegrasAutorizacaoProvisaoPendente();
    KoBindings(_pesquisaRegraAutorizacaoProvisaoPendente, "knockoutPesquisaRegrasAutorizacao", false, _pesquisaRegraAutorizacaoProvisaoPendente.Pesquisar.id);

    HeaderAuditoria(_nomeControllerRegrasAutorizacaoProvisaoPendente, _regrasAutorizacaoProvisaoPendente);

    loadGridAprovadores();

    new BuscarFuncionario(_regrasAutorizacaoProvisaoPendente.AdicionarAprovador, AdicionarAprovadorRetornoProvisaoPendente, _gridAprovadoresProvisaoPendente);
    new BuscarFuncionario(_pesquisaRegraAutorizacaoProvisaoPendente.Aprovador);

    buscarRegrasAutorizacaoProvisaoPendente();

}

/*
 * Declaração das Funções Associadas a Eventos das Regras
 */

function adicionarProvisaoPendenteClick(e, sender) {
    salvarRegrasProvisaoPendente(_nomeControllerRegrasAutorizacaoProvisaoPendente + "/Adicionar", "Cadastrado com sucesso.", e, sender);
}

function atualizarProvisaoPendenteClick(e, sender) {
    salvarRegrasProvisaoPendente(_nomeControllerRegrasAutorizacaoProvisaoPendente + "/Atualizar", "Atualizado com sucesso.", e, sender);
}

function cancelarProvisaoPendenteClick() {
    limparTodosCamposProvisaoPendente();
}

function editarRegraAprovacaoProvisaoPendente(data) {
    limparTodosCamposProvisaoPendente();

    _regrasAutorizacaoProvisaoPendente.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regrasAutorizacaoProvisaoPendente, _nomeControllerRegrasAutorizacaoProvisaoPendente + "/BuscarPorCodigo", function (arg) {
        _pesquisaRegraAutorizacaoProvisaoPendente.ExibirFiltros.visibleFade(false);
        
        _regrasAutorizacaoProvisaoPendente.Aprovadores.val(arg.Data.Aprovadores);

        _regrasAutorizacaoProvisaoPendente.CRUDAdicionar.visible(false);
        _regrasAutorizacaoProvisaoPendente.CRUDCancelar.visible(true);
        _regrasAutorizacaoProvisaoPendente.CRUDAtualizar.visible(true);
        _regrasAutorizacaoProvisaoPendente.CRUDExcluir.visible(true);
    }, null);
}

function excluirProvisaoPendenteClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as regras?", function () {
        ExcluirPorCodigo(_regrasAutorizacaoProvisaoPendente, _nomeControllerRegrasAutorizacaoProvisaoPendente + "/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegrasAutorizacaoProvisaoPendente.CarregarGrid();
                    limparTodosCamposProvisaoPendente();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function removerAprovadorProvisaoPendenteClick(data) {
    var dataGrid = _gridAprovadoresProvisaoPendente.BuscarRegistros().slice();

    dataGrid = dataGrid.filter(function (a) {
        return a.Codigo != data.Codigo;
    });

    _gridAprovadoresProvisaoPendente.CarregarGrid(dataGrid);
}

/*
 * Declaração das Funções Associadas a Eventos das Alçadas
 */

function adicionarAlcadaProvisaoPendenteClick(prop) {
    if (!validarCamposAlcadaProvisaoPendente(prop))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var listaRegrasAdicionar = obterListaObjetoAlcadaPorTipoPropriedadeAlcadaProvisaoPendente(prop);
    var listaRegras = obterRegrasOrdenadasProvisaoPendente(_regrasAutorizacaoProvisaoPendente["Alcadas" + prop]);

    if (listaRegrasAdicionar.length == 1) {
        if (isRegraDuplicadaProvisaoPendente(listaRegras, listaRegrasAdicionar[0]))
            return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

        listaRegras.push(listaRegrasAdicionar[0]);
    }
    else {
        listaRegrasAdicionar.forEach(function (regraAdicionar) {
            if (!isRegraDuplicadaProvisaoPendente(listaRegras, regraAdicionar))
                listaRegras.push(regraAdicionar);
        });
    }

    _regrasAutorizacaoProvisaoPendente["Alcadas" + prop].val(listaRegras);

    limparCamposAlcadaProvisaoPendente(prop);
}

function atualizarAlcadaProvisaoPendenteClick(prop) {
    if (!validarCamposAlcadaProvisaoPendente(prop))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var listaRegrasAtualizar = obterListaObjetoAlcadaPorTipoPropriedadeAlcadaProvisaoPendente(prop);
    var listaRegras = obterRegrasOrdenadasProvisaoPendente(_regrasAutorizacaoProvisaoPendente["Alcadas" + prop]);

    if (listaRegrasAtualizar.length == 1) {
        var regra = listaRegrasAtualizar[0];

        if (isRegraDuplicadaProvisaoPendente(listaRegras, regra))
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
            if (!isRegraDuplicadaProvisaoPendente(listaRegras, regraAtualizar))
                listaRegras.push(regraAtualizar);
        })
    }

    _regrasAutorizacaoProvisaoPendente["Alcadas" + prop].val(listaRegras);

    limparCamposAlcadaProvisaoPendente(prop);
}

function cancelarAlcadaProvisaoPendenteClick(prop) {
    limparCamposAlcadaProvisaoPendente(prop);
}

function editarRegraAlcadaProvisaoPendenteClick(prop, codigo) {
    var listaRegras = _regrasAutorizacaoProvisaoPendente["Alcadas" + prop].val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _regrasAutorizacaoProvisaoPendente["Codigo" + prop].val(regra.Codigo);
        _regrasAutorizacaoProvisaoPendente["Ordem" + prop].val(regra.Ordem);
        _regrasAutorizacaoProvisaoPendente["Condicao" + prop].val(regra.Condicao);
        _regrasAutorizacaoProvisaoPendente["Juncao" + prop].val(regra.Juncao);

        if (regra.Entidade) {
            var knoutEntidade = _regrasAutorizacaoProvisaoPendente[prop];

            if (knoutEntidade.type == types.multiplesEntities) {
                var multiplaEntidade = new Object();

                multiplaEntidade[knoutEntidade.multiplesEntitiesConfig.propCodigo] = regra.Entidade.Codigo;
                multiplaEntidade[knoutEntidade.multiplesEntitiesConfig.propDescricao] = regra.Entidade.Descricao;

                knoutEntidade.multiplesEntities([multiplaEntidade]);
            }
            else {
                _regrasAutorizacaoProvisaoPendente[prop].val(regra.Entidade.Descricao);
                _regrasAutorizacaoProvisaoPendente[prop].codEntity(regra.Entidade.Codigo);
            }
        }
        else
            _regrasAutorizacaoProvisaoPendente[prop].val(regra.Valor);

        _regrasAutorizacaoProvisaoPendente.Adicionar.visible(false);
        _regrasAutorizacaoProvisaoPendente.Atualizar.visible(true);
        _regrasAutorizacaoProvisaoPendente.Excluir.visible(true);
        _regrasAutorizacaoProvisaoPendente.Cancelar.visible(true);
    }
}

function excluirAlcadaProvisaoPendenteClick(prop) {
    var codigo = _regrasAutorizacaoProvisaoPendente["Codigo" + prop].val();

    excluirAlcadaProvisaoPendente(prop, codigo);

    limparCamposAlcadaProvisaoPendente(prop);
}

/*
 * Declaração das Funções das Regras de Autorização
 */

function AdicionarAprovadorRetornoProvisaoPendente(data) {
    if (data != null) {
        if (!$.isArray(data))
            data = [data];

        var dataGrid = _gridAprovadoresProvisaoPendente.BuscarRegistros();

        data = data.map(function (a) {
            return {
                Codigo: a.Codigo,
                Nome: a.Nome,
            };
        });

        dataGrid = dataGrid.concat(data);

        _gridAprovadoresProvisaoPendente.CarregarGrid(dataGrid);
    }
}

function buscarRegrasAutorizacaoProvisaoPendente() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraAprovacaoProvisaoPendente, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();

    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegrasAutorizacaoProvisaoPendente = new GridView(_pesquisaRegraAutorizacaoProvisaoPendente.Pesquisar.idGrid, _nomeControllerRegrasAutorizacaoProvisaoPendente + "/Pesquisa", _pesquisaRegraAutorizacaoProvisaoPendente, menuOpcoes);
    _gridRegrasAutorizacaoProvisaoPendente.CarregarGrid();
}

function exibirMensagemCamposObrigatorioProvisaoPendente() {
    exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function limparTodosCamposProvisaoPendente() {
    LimparCampos(_regrasAutorizacaoProvisaoPendente);

    _configuracaoRegrasProvisaoPendente.Alcadas.forEach(function (alcada) {
        limparCamposAlcadaProvisaoPendente(alcada.prop);
        _regrasAutorizacaoProvisaoPendente["Alcadas" + alcada.prop].val([]);
    });

    _gridAprovadoresProvisaoPendente.CarregarGrid([]);

    $("#myTab li:first a").click();

    _regrasAutorizacaoProvisaoPendente.CRUDAdicionar.visible(true);
    _regrasAutorizacaoProvisaoPendente.CRUDCancelar.visible(true);
    _regrasAutorizacaoProvisaoPendente.CRUDAtualizar.visible(false);
    _regrasAutorizacaoProvisaoPendente.CRUDExcluir.visible(false);
}

function obterCabecalhoTabelaProvisaoPendente(nomeCampo) {
    return '<tr>' +
        '<th width="15%" class="text-align-center">Ordem</th>' +
        '<th width="30%" class="text-align-center">Junção</th>' +
        '<th width="30%" class="text-align-center">Condição</th>' +
        '<th width="40%" class="text-align-left">' + nomeCampo + '</th>' +
        '<th width="15%" class="text-align-center">Editar</th>' +
        '</tr>';
}

function obterConfiguracaoRegraPorPropriedadeProvisaoPendente(prop) {
    var config = null;

    for (var i in _configuracaoRegrasProvisaoPendente.Alcadas) {
        if (_configuracaoRegrasProvisaoPendente.Alcadas[i].prop == prop) {
            config = _configuracaoRegrasProvisaoPendente.Alcadas[i];
            break;
        }
    }

    return config;
}

function obterListaAprovadoresProvisaoPendente() {
    if (arguments.length > 0 && arguments[0] != "")
        _gridAprovadoresProvisaoPendente.CarregarGrid(arguments[0]);
    else
        return JSON.stringify(_gridAprovadoresProvisaoPendente.BuscarRegistros());
}

function obterObjetoRegraSalvarProvisaoPendente() {
    var dados = {
        Codigo: _regrasAutorizacaoProvisaoPendente.Codigo.val(),
        Descricao: _regrasAutorizacaoProvisaoPendente.Descricao.val(),
        Vigencia: _regrasAutorizacaoProvisaoPendente.Vigencia.val(),
        NumeroAprovadores: _regrasAutorizacaoProvisaoPendente.NumeroAprovadores.val(),
        Observacoes: _regrasAutorizacaoProvisaoPendente.Observacoes.val(),
        PrioridadeAprovacao: _regrasAutorizacaoProvisaoPendente.PrioridadeAprovacao.val(),
        Status: _regrasAutorizacaoProvisaoPendente.Status.val(),
        Aprovadores: obterListaAprovadoresProvisaoPendente(),
        TipoGeracaoRegraProvisao: _regrasAutorizacaoProvisaoPendente.TipoGeracaoRegraProvisao.val()
    };

    _configuracaoRegrasProvisaoPendente.Alcadas.forEach(function (alcada) {
        dados["UsarRegraPor" + alcada.prop] = _regrasAutorizacaoProvisaoPendente["UsarRegraPor" + alcada.prop].val();
        dados["Alcadas" + alcada.prop] = JSON.stringify(_regrasAutorizacaoProvisaoPendente["Alcadas" + alcada.prop].val());
    });

    return dados;
}

function obterRegrasOrdenadasProvisaoPendente(kout) {
    var regras = kout.val().slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });

    return regras;
}

function renderizarGridRegrasProvisaoPendente(prop, tipoPropriedadeAlcada, grid) {
    var html = "";
    var kout = _regrasAutorizacaoProvisaoPendente["Alcadas" + prop];
    var listaRegras = obterRegrasOrdenadasProvisaoPendente(kout)

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

        html += '<td class="text-align-center"><a href="javascript:;" onclick="editarRegraAlcadaProvisaoPendenteClick(\'' + prop + '\', \'' + regra.Codigo + '\')">Editar</a></td></tr>';
    });

    grid.RecarregarGrid(html);
}

function reordenarLinhasGridProvisaoPendente(prop) {
    var kout = _regrasAutorizacaoProvisaoPendente[prop];
    var listaRegrasAtualizada = [];
    var listaRegras = kout.val();

    var BuscaRegraPorCodigo = function (codigo) {
        for (var i in listaRegras)
            if (listaRegras[i].Codigo == codigo)
                return listaRegras[i];

        return null;
    }

    $("#" + _regrasAutorizacaoProvisaoPendente[prop].idGrid + " table tbody tr").each(function (i) {
        var regra = BuscaRegraPorCodigo($(this).data('codigo'));
        regra.Ordem = i + 1;
        listaRegrasAtualizada.push(regra);
    });

    kout.val(listaRegrasAtualizada);
}

function salvarRegrasProvisaoPendente(url, mensagemSucesso) {
    listaMensagemErros = validarRegrasProvisaoPendente();

    if (listaMensagemErros.length > 0)
        return exibirMensagem(tipoMensagem.atencao, "Regra(s) inválida(s)", listaMensagemErros.join("<br>"));

    if (!ValidarCamposObrigatorios(_regrasAutorizacaoProvisaoPendente))
        return exibirMensagemCamposObrigatorioProvisaoPendente();

    executarReST(url, obterObjetoRegraSalvarProvisaoPendente(), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                _gridRegrasAutorizacaoProvisaoPendente.CarregarGrid();
                limparTodosCamposProvisaoPendente();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function validarRegrasProvisaoPendente() {
    var listaMensagemErros = [];

    if (_gridAprovadoresProvisaoPendente.BuscarRegistros().length < _regrasAutorizacaoProvisaoPendente.NumeroAprovadores.val())
        return ["O número de aprovadores selecionados deve ser maior ou igual a " + _regrasAutorizacaoProvisaoPendente.NumeroAprovadores.val()];

    _configuracaoRegrasProvisaoPendente.Alcadas.forEach(function (alcada) {
        if (_regrasAutorizacaoProvisaoPendente["UsarRegraPor" + alcada.prop].val() && _regrasAutorizacaoProvisaoPendente["Alcadas" + alcada.prop].val().length == 0)
            listaMensagemErros.push("Nenhuma regra por " + alcada.descricao + " cadastrada.");
    });

    return listaMensagemErros;
}

/*
 * Declaração das Funções das Alçadas
 */

function atualizaListaAlcadasProvisaoPendente(prop) {
    var kout = _regrasAutorizacaoProvisaoPendente["Alcadas" + prop];
    var listaRegras = obterRegrasOrdenadasProvisaoPendente(kout)

    kout.list = JSON.stringify(listaRegras);
}

function excluirAlcadaProvisaoPendente(prop, codigo) {
    var listaRegras = obterRegrasOrdenadasProvisaoPendente(_regrasAutorizacaoProvisaoPendente["Alcadas" + prop]);
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

    _regrasAutorizacaoProvisaoPendente["Alcadas" + prop].val(listaRegras);
}

function isRegraDuplicadaProvisaoPendente(listaRegras, regra) {
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

function limparCamposAlcadaProvisaoPendente(prop) {
    _regrasAutorizacaoProvisaoPendente["Codigo" + prop].val(_regrasAutorizacaoProvisaoPendente["Codigo" + prop].def);
    _regrasAutorizacaoProvisaoPendente["Ordem" + prop].val(_regrasAutorizacaoProvisaoPendente["Ordem" + prop].def);
    _regrasAutorizacaoProvisaoPendente["Condicao" + prop].val(_regrasAutorizacaoProvisaoPendente["Condicao" + prop].def);
    _regrasAutorizacaoProvisaoPendente["Juncao" + prop].val(_regrasAutorizacaoProvisaoPendente["Juncao" + prop].def);

    LimparCampo(_regrasAutorizacaoProvisaoPendente[prop]);

    _regrasAutorizacaoProvisaoPendente.Adicionar.visible(true);
    _regrasAutorizacaoProvisaoPendente.Atualizar.visible(false);
    _regrasAutorizacaoProvisaoPendente.Excluir.visible(false);
    _regrasAutorizacaoProvisaoPendente.Cancelar.visible(false);
}

function obterObjetoAlcadaProvisaoPendente(prop) {
    var config = obterConfiguracaoRegraPorPropriedadeProvisaoPendente(prop);

    var codigo = _regrasAutorizacaoProvisaoPendente["Codigo" + prop].val();
    var ordem = _regrasAutorizacaoProvisaoPendente["Ordem" + prop].val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : config.grid.ObterOrdencao().length + 1,
        Juncao: _regrasAutorizacaoProvisaoPendente["Juncao" + prop].val(),
        Condicao: _regrasAutorizacaoProvisaoPendente["Condicao" + prop].val()
    };

    switch (config.tipoPropriedadeAlcada) {
        case EnumTipoPropriedadeAlcada.Decimal:
            regra.Valor = Globalize.parseFloat(_regrasAutorizacaoProvisaoPendente[prop].val()) || 0;
            break;

        case EnumTipoPropriedadeAlcada.Entidade:
            regra.Entidade = {
                Codigo: parseInt(_regrasAutorizacaoProvisaoPendente[prop].codEntity()),
                Descricao: _regrasAutorizacaoProvisaoPendente[prop].val()
            };
            break;

        case EnumTipoPropriedadeAlcada.Inteiro:
            regra.Valor = Globalize.parseInt(_regrasAutorizacaoProvisaoPendente[prop].val()) || 0;
            break;
    }

    return regra;
}

function obterListaObjetoAlcadaProvisaoPendente(prop, config) {
    var codigo = _regrasAutorizacaoProvisaoPendente["Codigo" + prop].val();
    var ordem = _regrasAutorizacaoProvisaoPendente["Ordem" + prop].val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : config.grid.ObterOrdencao().length + 1,
        Juncao: _regrasAutorizacaoProvisaoPendente["Juncao" + prop].val(),
        Condicao: _regrasAutorizacaoProvisaoPendente["Condicao" + prop].val()
    };

    switch (config.tipoPropriedadeAlcada) {
        case EnumTipoPropriedadeAlcada.Decimal:
            regra.Valor = Globalize.parseFloat(_regrasAutorizacaoProvisaoPendente[prop].val()) || 0;
            break;

        case EnumTipoPropriedadeAlcada.Entidade:
            regra.Entidade = {
                Codigo: parseInt(_regrasAutorizacaoProvisaoPendente[prop].codEntity()),
                Descricao: _regrasAutorizacaoProvisaoPendente[prop].val()
            };
            break;

        case EnumTipoPropriedadeAlcada.Inteiro:
            regra.Valor = Globalize.parseInt(_regrasAutorizacaoProvisaoPendente[prop].val()) || 0;
            break;
    }

    return [regra];
}

function obterListaObjetoAlcadaPorMultiplasEntidadesProvisaoPendente(prop, config) {
    var listaRegra = new Array();
    var knoutMultiplaEntidade = _regrasAutorizacaoProvisaoPendente[prop];
    var entidades = knoutMultiplaEntidade.multiplesEntities();
    var codigo = _regrasAutorizacaoProvisaoPendente["Codigo" + prop].val();
    var ordem = _regrasAutorizacaoProvisaoPendente["Ordem" + prop].val();

    if (codigo && (entidades.length > 1)) {
        excluirAlcadaProvisaoPendente(prop, codigo);

        codigo = 0;
        ordem = 0;
    }

    ordem = ordem != 0 ? ordem : config.grid.ObterOrdencao().length + 1;

    entidades.forEach(function (entidade) {
        var regra = {
            Codigo: codigo ? codigo : guid(),
            Ordem: ordem,
            Juncao: _regrasAutorizacaoProvisaoPendente["Juncao" + prop].val(),
            Condicao: _regrasAutorizacaoProvisaoPendente["Condicao" + prop].val(),
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

function obterListaObjetoAlcadaPorTipoPropriedadeAlcadaProvisaoPendente(prop) {
    var config = obterConfiguracaoRegraPorPropriedadeProvisaoPendente(prop);

    if (config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.MultiplasEntidades)
        return obterListaObjetoAlcadaPorMultiplasEntidadesProvisaoPendente(prop, config);

    return obterListaObjetoAlcadaProvisaoPendente(prop, config);
}

function validarCamposAlcadaProvisaoPendente(prop) {
    var valido = true;
    var config = obterConfiguracaoRegraPorPropriedadeProvisaoPendente(prop);
    var regra = _regrasAutorizacaoProvisaoPendente[prop];

    regra.required = true;

    if (((config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.Entidade) || (config.tipoPropriedadeAlcada === EnumTipoPropriedadeAlcada.MultiplasEntidades)) && !ValidarCampoObrigatorioEntity(regra))
        valido = false;
    else if (!ValidarCampoObrigatorioMap(regra))
        valido = false;

    regra.required = false;

    return valido;
}