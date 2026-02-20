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

var _programacaoLicenciamento;
var _pesquisaProgramacaoLicenciamento;
var _gridProgramacaoLicenciamento;

var ProgramacaoLicenciamento = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });    

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaProgramacaoLicenciamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProgramacaoLicenciamento.CarregarGrid();
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
function loadProgramacaoLicenciamento() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaProgramacaoLicenciamento = new PesquisaProgramacaoLicenciamento();
    KoBindings(_pesquisaProgramacaoLicenciamento, "knockoutPesquisaProgramacaoLicenciamento", false, _pesquisaProgramacaoLicenciamento.Pesquisar.id);

    // Instancia objeto principal
    _programacaoLicenciamento = new ProgramacaoLicenciamento();
    KoBindings(_programacaoLicenciamento, "knockoutProgramacaoLicenciamento");

    HeaderAuditoria("ProgramacaoLicenciamento", _programacaoLicenciamento);

    // Inicia busca
    buscarProgramacaoLicenciamento();
}

function adicionarClick(e, sender) {
    Salvar(_programacaoLicenciamento, "ProgramacaoLicenciamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridProgramacaoLicenciamento.CarregarGrid();
                limparCamposProgramacaoLicenciamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_programacaoLicenciamento, "ProgramacaoLicenciamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProgramacaoLicenciamento.CarregarGrid();
                limparCamposProgramacaoLicenciamento();
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
        ExcluirPorCodigo(_programacaoLicenciamento, "ProgramacaoLicenciamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridProgramacaoLicenciamento.CarregarGrid();
                    limparCamposProgramacaoLicenciamento();
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
    limparCamposProgramacaoLicenciamento();
}

function editarProgramacaoLicenciamentoClick(itemGrid) {
    // Limpa os campos
    limparCamposProgramacaoLicenciamento();

    // Seta o codigo do objeto
    _programacaoLicenciamento.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_programacaoLicenciamento, "ProgramacaoLicenciamento/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaProgramacaoLicenciamento.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _programacaoLicenciamento.Atualizar.visible(true);
                _programacaoLicenciamento.Excluir.visible(true);
                _programacaoLicenciamento.Cancelar.visible(true);
                _programacaoLicenciamento.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarProgramacaoLicenciamento() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProgramacaoLicenciamentoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridProgramacaoLicenciamento = new GridView(_pesquisaProgramacaoLicenciamento.Pesquisar.idGrid, "ProgramacaoLicenciamento/Pesquisa", _pesquisaProgramacaoLicenciamento, menuOpcoes, null);
    _gridProgramacaoLicenciamento.CarregarGrid();
}

function limparCamposProgramacaoLicenciamento() {
    _programacaoLicenciamento.Atualizar.visible(false);
    _programacaoLicenciamento.Cancelar.visible(false);
    _programacaoLicenciamento.Excluir.visible(false);
    _programacaoLicenciamento.Adicionar.visible(true);
    LimparCampos(_programacaoLicenciamento);
}