/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoAlerta.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAtivoPesquisa.js" />
/// <reference path="../../Consultas/Usuario.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDConfiguracaoNCPendente;
var _configuracaoNCPendente;
var _configuracaoNCPendenteFilial;
var _configuracaoNCPendenteSetor;
var _configuracaoNCPendenteTipoOperacao;
var _configuracaoNCPendenteItemNC;
var _pesquisaConfiguracaoNCPendente;
var _gridConfiguracaoNCPendente;
var _gridConfiguracaoNCPendenteUsuario;

/*
 * Declaração das Classes
 */

var CRUDConfiguracaoNCPendente = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false)});
}

var ConfiguracaoNCPendente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Nome = PropertyEntity({ text: "Nome: ", val: ko.observable(""), required: true, visible: true });
    this.Tipo = PropertyEntity({ text: "Tipo: ", val: ko.observable(""), def: "", required: true, getType: typesKnockout.select, options: EnumTipoConfiguracaoNCPendente.obterOpcoes(), });
    this.Situacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.select, options: _status , val: ko.observable(""), def: "" });
    this.NotificarTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Notificar transportador", required: true});

    this.ListaUsuario = PropertyEntity({ type: types.map, required: false, text: "Adicionar Usuário", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) }); 

    this.Codigo.val.subscribe(function (novoValor) {
        if (novoValor > 0) {
            _CRUDConfiguracaoNCPendente.Atualizar.visible(true);
            _CRUDConfiguracaoNCPendente.Excluir.visible(true);
            _CRUDConfiguracaoNCPendente.Adicionar.visible(false);
        } else {
            _CRUDConfiguracaoNCPendente.Atualizar.visible(false);
            _CRUDConfiguracaoNCPendente.Excluir.visible(false);
            _CRUDConfiguracaoNCPendente.Adicionar.visible(true);
        }
    });
}

var PesquisaConfiguracaoNCPendente = function () {
    this.Nome = PropertyEntity({ text: "Nome: ", val: ko.observable(""), def: "", maxlentgh: 100 });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(""), getType: typesKnockout.select, options: _statusPesquisa , def: ""});

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridConfiguracaoNCPendentes, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridConfiguracaoNCPendente() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoEditar]
    };

    _gridConfiguracaoNCPendente = new GridView(_pesquisaConfiguracaoNCPendente.Pesquisar.idGrid, "ConfiguracaoNCPendente/Pesquisa", _pesquisaConfiguracaoNCPendente, menuOpcoes);
    _gridConfiguracaoNCPendente.CarregarGrid();
}

function loadGridConfiguracaoNCPendenteUsuario() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerConfiguracaoNCPendenteUsuario, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridConfiguracaoNCPendenteUsuario = new BasicDataTable(_configuracaoNCPendente.ListaUsuario.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarFuncionario(_configuracaoNCPendente.ListaUsuario, null, _gridConfiguracaoNCPendenteUsuario);

    _gridConfiguracaoNCPendenteUsuario.CarregarGrid([]);
}

function loadConfiguracaoNCPendente() {
    _configuracaoNCPendente = new ConfiguracaoNCPendente();
    KoBindings(_configuracaoNCPendente, "knockoutConfiguracaoNCPendenteGeral");

    _configuracaoNCPendenteItemNC = new ItemNC();
    KoBindings(_configuracaoNCPendenteItemNC, "knockoutConfiguracaoNCPendenteItemNC");

    _configuracaoNCPendenteTipoOperacao = new TipoOperacao();
    KoBindings(_configuracaoNCPendenteTipoOperacao, "knockoutConfiguracaoNCPendenteTipoOperacao");

    _configuracaoNCPendenteFilial = new Filial();
    KoBindings(_configuracaoNCPendenteFilial, "knockoutConfiguracaoNCPendenteFilial");

    _configuracaoNCPendenteSetor = new Setor();
    KoBindings(_configuracaoNCPendenteSetor, "knockoutConfiguracaoNCPendenteSetor");

    HeaderAuditoria("ConfiguracaoNCPendente", _configuracaoNCPendente);

    _CRUDConfiguracaoNCPendente = new CRUDConfiguracaoNCPendente();
    KoBindings(_CRUDConfiguracaoNCPendente, "knockoutCRUDConfiguracaoNCPendente");

    _pesquisaConfiguracaoNCPendente = new PesquisaConfiguracaoNCPendente();
    KoBindings(_pesquisaConfiguracaoNCPendente, "knockoutPesquisaConfiguracaoNCPendente", false, _pesquisaConfiguracaoNCPendente.Pesquisar.id);

    loadGridConfiguracaoNCPendente();
    loadGridConfiguracaoNCPendenteUsuario();
    loadGridConfiguracaoNCPendenteFilial();
    loadGridConfiguracaoNCPendenteItemNC();
    loadGridConfiguracaoNCPendenteSetor();
    loadGridConfiguracaoNCPendenteTipoOperacao();
}

function obterConfiguracaoNCPendentesSalvar() {
    var configuracaoNCPendente = RetornarObjetoPesquisa(_configuracaoNCPendente);

    configuracaoNCPendente["Usuarios"] = obterListaUsuarioSalvar();
    configuracaoNCPendente["Setores"] = obterListaSetorSalvar();
    configuracaoNCPendente["Filiais"] = obterListaFilialSalvar();
    configuracaoNCPendente["TiposOperacao"] = obterListaTipoOperacaoSalvar();
    configuracaoNCPendente["ItensNC"] = obterListaItensSalvar();


    return configuracaoNCPendente;
}
function obterListaUsuario() {
    return _gridConfiguracaoNCPendenteUsuario.BuscarRegistros();
}

function obterListaUsuarioSalvar() {
    var listaUsuario = obterListaUsuario();
    var listaUsuarioRetornar = new Array();

    listaUsuario.forEach(function (usuario) {
        listaUsuarioRetornar.push(Number(usuario.Codigo));
    });

    return JSON.stringify(listaUsuarioRetornar);
}

function adicionarClick() {
    if (!validarCamposObrigatoriosConfiguracaoNCPendentes())
        return;

    executarReST("ConfiguracaoNCPendente/Adicionar", obterConfiguracaoNCPendentesSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                recarregarGridConfiguracaoNCPendentes();
                limparCamposConfiguracaoNCPendentes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick() {
    if (!validarCamposObrigatoriosConfiguracaoNCPendentes())
        return;

    executarReST("ConfiguracaoNCPendente/Atualizar", obterConfiguracaoNCPendentesSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                recarregarGridConfiguracaoNCPendentes();
                limparCamposConfiguracaoNCPendentes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarClick() {
    limparCamposConfiguracaoNCPendentes();
}

function editarClick(registroSelecionado) {
    limparCamposConfiguracaoNCPendentes();

    _configuracaoNCPendente.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_configuracaoNCPendente, "ConfiguracaoNCPendente/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaConfiguracaoNCPendente.ExibirFiltros.visibleFade(false);
                recarregarGridConfiguracaoNCPendentes();
                CarregarGridsTabs(retorno.Data)
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse registro?", function () {
        ExcluirPorCodigo(_configuracaoNCPendente, "ConfiguracaoNCPendente/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro excluído com sucesso");
                    recarregarGridConfiguracaoNCPendentes();
                    limparCamposConfiguracaoNCPendentes();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function validarCamposObrigatoriosConfiguracaoNCPendentes() {
    if (!ValidarCamposObrigatorios(_configuracaoNCPendente)) {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios");
        return false;
    }
    return true;
}


function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function recarregarGridConfiguracaoNCPendentes() {
    _gridConfiguracaoNCPendente.CarregarGrid();
}

function limparCamposConfiguracaoNCPendentes() {
    LimparCampos(_configuracaoNCPendente);
    LimparCampos(_configuracaoNCPendenteFilial);
    LimparCampos(_configuracaoNCPendenteSetor);
    LimparCampos(_configuracaoNCPendenteTipoOperacao);
    LimparCampos(_configuracaoNCPendenteItemNC);

    _gridFiliais.CarregarGrid([]);
    _gridSetor.CarregarGrid([])
    _gridConfiguracaoNCPendenteUsuario.CarregarGrid([])
    _gridItensNaoConformidade.CarregarGrid([])
    _gridTipoOperacao.CarregarGrid([])

}

function removerConfiguracaoNCPendenteUsuario(registroSelecionado) {
    var listaUsuario = obterListaUsuario();

    for (var i = 0; i < listaUsuario.length; i++) {
        if (registroSelecionado.Codigo == listaUsuario[i].Codigo) {
            listaUsuario.splice(i, 1);
            break;
        }
    }

    _gridConfiguracaoNCPendenteUsuario.CarregarGrid(listaUsuario);
}

function PreecherListasGrids(list, grid) {
    grid.CarregarGrid(list);
}

function CarregarGridsTabs(retorno) {
    PreecherListasGrids(retorno.ListaFiliais, _gridFiliais);
    PreecherListasGrids(retorno.ListaSetor, _gridSetor);
    PreecherListasGrids(retorno.ListaItensNC, _gridItensNaoConformidade);
    PreecherListasGrids(retorno.ListaTiposOperacao, _gridTipoOperacao);
    PreecherListasGrids(retorno.ListaUsuario, _gridConfiguracaoNCPendenteUsuario);
}
