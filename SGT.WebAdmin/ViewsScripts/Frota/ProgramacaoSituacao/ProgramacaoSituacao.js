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
/// <reference path="../../Enumeradores/EnumCores.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _programacaoSituacao;
var _pesquisaProgramacaoSituacao;
var _gridProgramacaoSituacao;

var ProgramacaoSituacao = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.TipoEntidadeProgramacao = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoEntidadeProgramacao.Todos), options: EnumTipoEntidadeProgramacao.obterOpcoes(), def: EnumTipoEntidadeProgramacao.Todos });
    this.Cores = PropertyEntity({ text: "Cor: ", val: ko.observable(EnumCores.Branco), options: EnumCores.obterOpcoes(), def: EnumCores.Branco });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaProgramacaoSituacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProgramacaoSituacao.CarregarGrid();
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
function loadProgramacaoSituacao() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaProgramacaoSituacao = new PesquisaProgramacaoSituacao();
    KoBindings(_pesquisaProgramacaoSituacao, "knockoutPesquisaProgramacaoSituacao", false, _pesquisaProgramacaoSituacao.Pesquisar.id);

    // Instancia objeto principal
    _programacaoSituacao = new ProgramacaoSituacao();
    KoBindings(_programacaoSituacao, "knockoutProgramacaoSituacao");

    HeaderAuditoria("ProgramacaoSituacao", _programacaoSituacao);

    // Inicia busca
    buscarProgramacaoSituacao();
}

function adicionarClick(e, sender) {
    Salvar(_programacaoSituacao, "ProgramacaoSituacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridProgramacaoSituacao.CarregarGrid();
                limparCamposProgramacaoSituacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_programacaoSituacao, "ProgramacaoSituacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProgramacaoSituacao.CarregarGrid();
                limparCamposProgramacaoSituacao();
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
        ExcluirPorCodigo(_programacaoSituacao, "ProgramacaoSituacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridProgramacaoSituacao.CarregarGrid();
                    limparCamposProgramacaoSituacao();
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
    limparCamposProgramacaoSituacao();
}

function editarProgramacaoSituacaoClick(itemGrid) {
    // Limpa os campos
    limparCamposProgramacaoSituacao();

    // Seta o codigo do objeto
    _programacaoSituacao.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_programacaoSituacao, "ProgramacaoSituacao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaProgramacaoSituacao.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _programacaoSituacao.Atualizar.visible(true);
                _programacaoSituacao.Excluir.visible(true);
                _programacaoSituacao.Cancelar.visible(true);
                _programacaoSituacao.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarProgramacaoSituacao() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProgramacaoSituacaoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridProgramacaoSituacao = new GridView(_pesquisaProgramacaoSituacao.Pesquisar.idGrid, "ProgramacaoSituacao/Pesquisa", _pesquisaProgramacaoSituacao, menuOpcoes, null);
    _gridProgramacaoSituacao.CarregarGrid();
}

function limparCamposProgramacaoSituacao() {
    _programacaoSituacao.Atualizar.visible(false);
    _programacaoSituacao.Cancelar.visible(false);
    _programacaoSituacao.Excluir.visible(false);
    _programacaoSituacao.Adicionar.visible(true);
    LimparCampos(_programacaoSituacao);
}