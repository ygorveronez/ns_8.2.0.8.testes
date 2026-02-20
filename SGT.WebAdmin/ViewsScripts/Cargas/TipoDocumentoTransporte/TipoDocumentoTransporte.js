//#region Referencias
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
//#endregion 

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoDocumentoTransporte;
var _pesquisaTipoDocumentoTransporte;
var _gridTipoDocumentoTransporte;

var TipoDocumentoTransporte = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "*Código Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 556, val: ko.observable(true), options: _status, def: true });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaTipoDocumentoTransporte = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(true), options: _status, def: 0 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoDocumentoTransporte.CarregarGrid();
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
function loadTipoDocumentoTransporte() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaTipoDocumentoTransporte = new PesquisaTipoDocumentoTransporte();
    KoBindings(_pesquisaTipoDocumentoTransporte, "knockoutTipoDocumentoTransporte", false, _pesquisaTipoDocumentoTransporte.Pesquisar.id);

    // Instancia objeto principal
    _tipoDocumentoTransporte = new TipoDocumentoTransporte();
    KoBindings(_tipoDocumentoTransporte, "knockoutDocumentoTransporte");

    HeaderAuditoria("TipoDocumentoTransporte", _tipoDocumentoTransporte);

    // Instancia buscas

    // Inicia busca
    BuscarTipoDocumentoTransporte();
}

function adicionarClick(e, sender) {
    Salvar(_tipoDocumentoTransporte, "TipoDocumentoTransporte/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTipoDocumentoTransporte.CarregarGrid();
                LimparCamposTipoDocumentoTransporte();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoDocumentoTransporte, "TipoDocumentoTransporte/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoDocumentoTransporte.CarregarGrid();
                LimparCamposTipoDocumentoTransporte();
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
        ExcluirPorCodigo(_tipoDocumentoTransporte, "TipoDocumentoTransporte/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoDocumentoTransporte.CarregarGrid();
                    LimparCamposTipoDocumentoTransporte();
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
    LimparCamposTipoDocumentoTransporte();
}

function editarTipoDocumentoTransporteClick(itemGrid) {
    // Limpa os campos
    LimparCamposTipoDocumentoTransporte();

    // Seta o codigo do objeto
    _tipoDocumentoTransporte.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tipoDocumentoTransporte, "TipoDocumentoTransporte/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaTipoDocumentoTransporte.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _tipoDocumentoTransporte.Atualizar.visible(true);
                _tipoDocumentoTransporte.Excluir.visible(true);
                _tipoDocumentoTransporte.Cancelar.visible(true);
                _tipoDocumentoTransporte.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarTipoDocumentoTransporte() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoDocumentoTransporteClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "TipoDocumentoTransporte/ExportarPesquisa",
        titulo: "Tipo de Documento de Transporte"
    };

    // Inicia Grid de busca
    _gridTipoDocumentoTransporte = new GridView(_pesquisaTipoDocumentoTransporte.Pesquisar.idGrid, "TipoDocumentoTransporte/Pesquisa", _pesquisaTipoDocumentoTransporte, menuOpcoes, null, 20, null, null, null, null, null, null, configExportacao);
    _gridTipoDocumentoTransporte.CarregarGrid();
}

function LimparCamposTipoDocumentoTransporte() {
    _tipoDocumentoTransporte.Atualizar.visible(false);
    _tipoDocumentoTransporte.Cancelar.visible(false);
    _tipoDocumentoTransporte.Excluir.visible(false);
    _tipoDocumentoTransporte.Adicionar.visible(true);
    LimparCampos(_tipoDocumentoTransporte);
}