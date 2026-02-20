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


//*******MAPEAMENTO KNOUCKOUT*******

var _motivoRetificacaoColeta;
var _pesquisaMotivoRetificacaoColeta;
var _gridMotivoRetificacaoColeta;

var MotivoRetificacaoColeta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoAplicacaoColetaEntrega = PropertyEntity({ val: ko.observable(EnumTipoAplicacaoColetaEntrega.Entrega), options: EnumTipoAplicacaoColetaEntrega.obterOpcoes(), def: EnumTipoAplicacaoColetaEntrega.Entrega, text: "Aplicação da retificação para: ", visible: ko.observable(false) });

    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", issue: 556, val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.ReabrirEntregaZerarData = PropertyEntity({ text: "Reabrir Entrega e Zerar Data Fim Entrega", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false), visible: ko.observable(true) })

}

var PesquisaMotivoRetificacaoColeta = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", issue: 557, val: ko.observable(true), options: _statusPesquisa, def: true });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoRetificacaoColeta.CarregarGrid();
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


//*******EVENTOS*******
function loadMotivoRetificacaoColeta() {
    _pesquisaMotivoRetificacaoColeta = new PesquisaMotivoRetificacaoColeta();
    KoBindings(_pesquisaMotivoRetificacaoColeta, "knockoutPesquisaMotivoRetificacaoColeta", false, _pesquisaMotivoRetificacaoColeta.Pesquisar.id);
    new BuscarTiposOperacao(_pesquisaMotivoRetificacaoColeta.TipoOperacao);

    _motivoRetificacaoColeta = new MotivoRetificacaoColeta();
    KoBindings(_motivoRetificacaoColeta, "knockoutMotivoRetificacaoColeta");

    new BuscarTiposOperacao(_motivoRetificacaoColeta.TipoOperacao);

    HeaderAuditoria("MotivoRetificacaoColeta", _motivoRetificacaoColeta);

    buscarMotivoRetificacaoColeta();
}

function adicionarClick(e, sender) {
    Salvar(_motivoRetificacaoColeta, "MotivoRetificacaoColeta/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoRetificacaoColeta.CarregarGrid();
                limparCamposMotivoRetificacaoColeta();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoRetificacaoColeta, "MotivoRetificacaoColeta/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoRetificacaoColeta.CarregarGrid();
                limparCamposMotivoRetificacaoColeta();
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
        ExcluirPorCodigo(_motivoRetificacaoColeta, "MotivoRetificacaoColeta/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoRetificacaoColeta.CarregarGrid();
                    limparCamposMotivoRetificacaoColeta();
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
    limparCamposMotivoRetificacaoColeta();
}

function editarMotivoRetificacaoColetaClick(itemGrid) {
    // Limpa os campos
    limparCamposMotivoRetificacaoColeta();

    _motivoRetificacaoColeta.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_motivoRetificacaoColeta, "MotivoRetificacaoColeta/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMotivoRetificacaoColeta.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _motivoRetificacaoColeta.Atualizar.visible(true);
                _motivoRetificacaoColeta.Excluir.visible(true);
                _motivoRetificacaoColeta.Cancelar.visible(true);
                _motivoRetificacaoColeta.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarMotivoRetificacaoColeta() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoRetificacaoColetaClick, tamanho: "7", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "MotivoRetificacaoColeta/ExportarPesquisa",
        titulo: "Motivo Rejeição"
    };


    // Inicia Grid de busca
    _gridMotivoRetificacaoColeta = new GridViewExportacao(_pesquisaMotivoRetificacaoColeta.Pesquisar.idGrid, "MotivoRetificacaoColeta/Pesquisa", _pesquisaMotivoRetificacaoColeta, menuOpcoes, configExportacao);
    _gridMotivoRetificacaoColeta.CarregarGrid();
}

function limparCamposMotivoRetificacaoColeta() {
    _motivoRetificacaoColeta.Atualizar.visible(false);
    _motivoRetificacaoColeta.Cancelar.visible(false);
    _motivoRetificacaoColeta.Excluir.visible(false);
    _motivoRetificacaoColeta.Adicionar.visible(true);
    LimparCampos(_motivoRetificacaoColeta);
}