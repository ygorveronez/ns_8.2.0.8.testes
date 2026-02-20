/// <reference path="../../../js/libs/jquery-2.1.1.js" />
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
/// <reference path="../../Enumeradores/EnumTipoEntidadeProgramacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _programacaoAlocacao;
var _pesquisaProgramacaoAlocacao;
var _gridProgramacaoAlocacao;

var ProgramacaoAlocacao = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.TipoEntidadeProgramacao = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoEntidadeProgramacao.Todos), options: EnumTipoEntidadeProgramacao.obterOpcoes(), def: EnumTipoEntidadeProgramacao.Todos });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaProgramacaoAlocacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProgramacaoAlocacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadProgramacaoAlocacao() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaProgramacaoAlocacao = new PesquisaProgramacaoAlocacao();
    KoBindings(_pesquisaProgramacaoAlocacao, "knockoutPesquisaProgramacaoAlocacao", false, _pesquisaProgramacaoAlocacao.Pesquisar.id);

    // Instancia objeto principal
    _programacaoAlocacao = new ProgramacaoAlocacao();
    KoBindings(_programacaoAlocacao, "knockoutProgramacaoAlocacao");

    HeaderAuditoria("ProgramacaoAlocacao", _programacaoAlocacao);

    // Inicia busca
    buscarProgramacaoAlocacao();
}

function adicionarClick(e, sender) {
    Salvar(_programacaoAlocacao, "ProgramacaoAlocacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridProgramacaoAlocacao.CarregarGrid();
                limparCamposProgramacaoAlocacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_programacaoAlocacao, "ProgramacaoAlocacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProgramacaoAlocacao.CarregarGrid();
                limparCamposProgramacaoAlocacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_programacaoAlocacao, "ProgramacaoAlocacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridProgramacaoAlocacao.CarregarGrid();
                    limparCamposProgramacaoAlocacao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposProgramacaoAlocacao();
}

function editarProgramacaoAlocacaoClick(itemGrid) {
    // Limpa os campos
    limparCamposProgramacaoAlocacao();

    // Seta o codigo do objeto
    _programacaoAlocacao.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_programacaoAlocacao, "ProgramacaoAlocacao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaProgramacaoAlocacao.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _programacaoAlocacao.Atualizar.visible(true);
                _programacaoAlocacao.Excluir.visible(true);
                _programacaoAlocacao.Cancelar.visible(true);
                _programacaoAlocacao.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarProgramacaoAlocacao() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProgramacaoAlocacaoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridProgramacaoAlocacao = new GridView(_pesquisaProgramacaoAlocacao.Pesquisar.idGrid, "ProgramacaoAlocacao/Pesquisa", _pesquisaProgramacaoAlocacao, menuOpcoes, null);
    _gridProgramacaoAlocacao.CarregarGrid();
}

function limparCamposProgramacaoAlocacao() {
    _programacaoAlocacao.Atualizar.visible(false);
    _programacaoAlocacao.Cancelar.visible(false);
    _programacaoAlocacao.Excluir.visible(false);
    _programacaoAlocacao.Adicionar.visible(true);
    LimparCampos(_programacaoAlocacao);
}