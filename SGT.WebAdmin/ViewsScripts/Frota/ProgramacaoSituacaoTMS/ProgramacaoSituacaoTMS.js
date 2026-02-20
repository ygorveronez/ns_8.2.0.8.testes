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

var _programacaoSituacaoTMS;
var _pesquisaProgramacaoSituacaoTMS;
var _gridProgramacaoSituacaoTMS;

var ProgramacaoSituacaoTMS = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.Finalizadora = PropertyEntity({ text: "Situação Finalizadora? (Veículo não estará mais disponível na programação)", val: ko.observable(false), getType: typesKnockout.bool });
    this.TipoEntidadeProgramacao = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoEntidadeProgramacao.Todos), options: EnumTipoEntidadeProgramacao.obterOpcoes(), def: EnumTipoEntidadeProgramacao.Todos });
    this.Cores = PropertyEntity({ text: "Cor: ", val: ko.observable(EnumCores.Branco), options: EnumCores.obterOpcoes(), def: EnumCores.Branco });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaProgramacaoSituacaoTMS = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProgramacaoSituacaoTMS.CarregarGrid();
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
function loadProgramacaoSituacaoTMS() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaProgramacaoSituacaoTMS = new PesquisaProgramacaoSituacaoTMS();
    KoBindings(_pesquisaProgramacaoSituacaoTMS, "knockoutPesquisaProgramacaoSituacaoTMS", false, _pesquisaProgramacaoSituacaoTMS.Pesquisar.id);

    // Instancia objeto principal
    _programacaoSituacaoTMS = new ProgramacaoSituacaoTMS();
    KoBindings(_programacaoSituacaoTMS, "knockoutProgramacaoSituacaoTMS");

    HeaderAuditoria("ProgramacaoSituacaoTMS", _programacaoSituacaoTMS);

    // Inicia busca
    buscarProgramacaoSituacaoTMS();
}

function adicionarClick(e, sender) {
    Salvar(_programacaoSituacaoTMS, "ProgramacaoSituacaoTMS/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridProgramacaoSituacaoTMS.CarregarGrid();
                limparCamposProgramacaoSituacaoTMS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_programacaoSituacaoTMS, "ProgramacaoSituacaoTMS/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProgramacaoSituacaoTMS.CarregarGrid();
                limparCamposProgramacaoSituacaoTMS();
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
        ExcluirPorCodigo(_programacaoSituacaoTMS, "ProgramacaoSituacaoTMS/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridProgramacaoSituacaoTMS.CarregarGrid();
                    limparCamposProgramacaoSituacaoTMS();
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
    limparCamposProgramacaoSituacaoTMS();
}

function editarProgramacaoSituacaoTMSClick(itemGrid) {
    // Limpa os campos
    limparCamposProgramacaoSituacaoTMS();

    // Seta o codigo do objeto
    _programacaoSituacaoTMS.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_programacaoSituacaoTMS, "ProgramacaoSituacaoTMS/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaProgramacaoSituacaoTMS.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _programacaoSituacaoTMS.Atualizar.visible(true);
                _programacaoSituacaoTMS.Excluir.visible(true);
                _programacaoSituacaoTMS.Cancelar.visible(true);
                _programacaoSituacaoTMS.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarProgramacaoSituacaoTMS() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProgramacaoSituacaoTMSClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridProgramacaoSituacaoTMS = new GridView(_pesquisaProgramacaoSituacaoTMS.Pesquisar.idGrid, "ProgramacaoSituacaoTMS/Pesquisa", _pesquisaProgramacaoSituacaoTMS, menuOpcoes, null);
    _gridProgramacaoSituacaoTMS.CarregarGrid();
}

function limparCamposProgramacaoSituacaoTMS() {
    _programacaoSituacaoTMS.Atualizar.visible(false);
    _programacaoSituacaoTMS.Cancelar.visible(false);
    _programacaoSituacaoTMS.Excluir.visible(false);
    _programacaoSituacaoTMS.Adicionar.visible(true);
    LimparCampos(_programacaoSituacaoTMS);
}