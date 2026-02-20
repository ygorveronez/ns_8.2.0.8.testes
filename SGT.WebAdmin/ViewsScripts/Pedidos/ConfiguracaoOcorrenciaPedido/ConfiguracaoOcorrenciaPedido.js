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
/// <reference path="../../Enumeradores/EnumEventoColetaEntrega.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoOcorrenciaPedido;
var _pesquisaConfiguracaoOcorrenciaPedido;
var _gridConfiguracaoOcorrenciaPedido;


var ConfiguracaoOcorrenciaPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EventoColetaPedido = PropertyEntity({ val: ko.observable(EnumEventoColetaEntrega.PedidoGerado), options: EnumEventoColetaEntrega.obterOpcoesOcorrenciaPedido(), def: EnumEventoColetaEntrega.PedidoGerado, text: "Evento para automação: ", visible: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: true, text: "*Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410, tipoEmissaoDocumentoOcorrencia: EnumTipoEmissaoDocumentoOcorrencia.Todos });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaConfiguracaoOcorrenciaPedido = function () {
    this.EventoColetaPedido = PropertyEntity({ val: ko.observable(EnumEventoColetaEntrega.Todos), options: EnumEventoColetaEntrega.obterOpcoesPesquisaPedido(), def: EnumEventoColetaEntrega.Todos, text: "Evento para automação: ", visible: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: true, text: "*Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410, tipoEmissaoDocumentoOcorrencia: EnumTipoEmissaoDocumentoOcorrencia.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoOcorrenciaPedido.CarregarGrid();
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
function loadConfiguracaoOcorrenciaPedido() {
    _pesquisaConfiguracaoOcorrenciaPedido = new PesquisaConfiguracaoOcorrenciaPedido();
    KoBindings(_pesquisaConfiguracaoOcorrenciaPedido, "knockoutPesquisaConfiguracaoOcorrenciaPedido", false, _pesquisaConfiguracaoOcorrenciaPedido.Pesquisar.id);

    _configuracaoOcorrenciaPedido = new ConfiguracaoOcorrenciaPedido();
    KoBindings(_configuracaoOcorrenciaPedido, "knockoutConfiguracaoOcorrenciaPedido");

    
    HeaderAuditoria("ConfiguracaoOcorrenciaPedido", _configuracaoOcorrenciaPedido);

    new BuscarTipoOcorrencia(_configuracaoOcorrenciaPedido.TipoOcorrencia, null, null, null, null, null, null, null, true);
    new BuscarTipoOcorrencia(_pesquisaConfiguracaoOcorrenciaPedido.TipoOcorrencia, null, null, null, null, null, null, null, true);
    buscarConfiguracaoOcorrenciaPedido();
}

function adicionarClick(e, sender) {
    Salvar(_configuracaoOcorrenciaPedido, "ConfiguracaoOcorrenciaPedido/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridConfiguracaoOcorrenciaPedido.CarregarGrid();
                limparCamposConfiguracaoOcorrenciaPedido();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_configuracaoOcorrenciaPedido, "ConfiguracaoOcorrenciaPedido/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridConfiguracaoOcorrenciaPedido.CarregarGrid();
                limparCamposConfiguracaoOcorrenciaPedido();
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
        ExcluirPorCodigo(_configuracaoOcorrenciaPedido, "ConfiguracaoOcorrenciaPedido/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridConfiguracaoOcorrenciaPedido.CarregarGrid();
                    limparCamposConfiguracaoOcorrenciaPedido();
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
    limparCamposConfiguracaoOcorrenciaPedido();
}

function editarConfiguracaoOcorrenciaPedidoClick(itemGrid) {
    // Limpa os campos
    limparCamposConfiguracaoOcorrenciaPedido();

    _configuracaoOcorrenciaPedido.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_configuracaoOcorrenciaPedido, "ConfiguracaoOcorrenciaPedido/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaConfiguracaoOcorrenciaPedido.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _configuracaoOcorrenciaPedido.Atualizar.visible(true);
                _configuracaoOcorrenciaPedido.Excluir.visible(true);
                _configuracaoOcorrenciaPedido.Cancelar.visible(true);
                _configuracaoOcorrenciaPedido.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarConfiguracaoOcorrenciaPedido() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoOcorrenciaPedidoClick, tamanho: "7", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "ConfiguracaoOcorrenciaPedido/ExportarPesquisa",
        titulo: "Configurações Ocorrencia Pedido"
    };


    // Inicia Grid de busca
    _gridConfiguracaoOcorrenciaPedido = new GridViewExportacao(_pesquisaConfiguracaoOcorrenciaPedido.Pesquisar.idGrid, "ConfiguracaoOcorrenciaPedido/Pesquisa", _pesquisaConfiguracaoOcorrenciaPedido, menuOpcoes, configExportacao);
    _gridConfiguracaoOcorrenciaPedido.CarregarGrid();
}

function limparCamposConfiguracaoOcorrenciaPedido() {
    _configuracaoOcorrenciaPedido.Atualizar.visible(false);
    _configuracaoOcorrenciaPedido.Cancelar.visible(false);
    _configuracaoOcorrenciaPedido.Excluir.visible(false);
    _configuracaoOcorrenciaPedido.Adicionar.visible(true);
    LimparCampos(_configuracaoOcorrenciaPedido);
}