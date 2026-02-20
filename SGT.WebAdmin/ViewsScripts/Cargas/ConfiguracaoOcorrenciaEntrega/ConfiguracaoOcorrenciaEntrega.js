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

var _configuracaoOcorrenciaEntrega;
var _pesquisaConfiguracaoOcorrenciaEntrega;
var _gridConfiguracaoOcorrenciaEntrega;


var _Reentrega = [
    { text: "Sim", value: true },
    { text: "Não", value: false }
];
var _AlvoDoPedido = [
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var ConfiguracaoOcorrenciaEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoAplicacaoColetaEntrega = PropertyEntity({ val: ko.observable(EnumTipoAplicacaoColetaEntrega.Entrega), options: EnumTipoAplicacaoColetaEntrega.obterOpcoes(), def: EnumTipoAplicacaoColetaEntrega.Entrega, text: "Aplicação da regra para: ", visible: ko.observable(true) });
    this.EventoColetaEntrega = PropertyEntity({ val: ko.observable(EnumEventoColetaEntrega.Confirma), options: EnumEventoColetaEntrega.obterOpcoesOcorrenciaEntrega(), def: EnumEventoColetaEntrega.Confirma, text: "Evento para automação: ", visible: ko.observable(true) });
    this.AlvoDoPedido = PropertyEntity({ val: ko.observable(false), options: _AlvoDoPedido, def: false, text: "Alvo do Pedido: ", visible: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: true, text: "*Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410, tipoEmissaoDocumentoOcorrencia: EnumTipoEmissaoDocumentoOcorrencia.Todos });
    this.Reentrega = PropertyEntity({ val: ko.observable(false), options: _Reentrega, def: false, text: "Reentrega: ", visible: ko.observable(true) });
    this.TempoRecalculo = PropertyEntity({ text: "*A cada (tempo em minutos): ", val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: false, text: "Tipo Operação:", idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaConfiguracaoOcorrenciaEntrega = function () {
    this.TipoAplicacaoColetaEntrega = PropertyEntity({ val: ko.observable(EnumTipoAplicacaoColetaEntrega.Todos), options: EnumTipoAplicacaoColetaEntrega.obterOpcoesPesquisa(), def: EnumTipoAplicacaoColetaEntrega.Todos, text: "Aplicação da regra para: ", visible: ko.observable(true) });
    this.EventoColetaEntrega = PropertyEntity({ val: ko.observable(EnumEventoColetaEntrega.Todos), options: EnumEventoColetaEntrega.obterOpcoesPesquisaEntrega(), def: EnumEventoColetaEntrega.Todos, text: "Evento para automação: ", visible: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: true, text: "*Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410, tipoEmissaoDocumentoOcorrencia: EnumTipoEmissaoDocumentoOcorrencia.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoOcorrenciaEntrega.CarregarGrid();
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
function loadConfiguracaoOcorrenciaEntrega() {
    _pesquisaConfiguracaoOcorrenciaEntrega = new PesquisaConfiguracaoOcorrenciaEntrega();
    KoBindings(_pesquisaConfiguracaoOcorrenciaEntrega, "knockoutPesquisaConfiguracaoOcorrenciaEntrega", false, _pesquisaConfiguracaoOcorrenciaEntrega.Pesquisar.id);

    _configuracaoOcorrenciaEntrega = new ConfiguracaoOcorrenciaEntrega();
    KoBindings(_configuracaoOcorrenciaEntrega, "knockoutConfiguracaoOcorrenciaEntrega");

    _configuracaoOcorrenciaEntrega.EventoColetaEntrega.val.subscribe(function (novoValor) {
        if (novoValor == EnumEventoColetaEntrega.RecalculoPrevisao) {
            _configuracaoOcorrenciaEntrega.TempoRecalculo.visible(true);
        } else
            _configuracaoOcorrenciaEntrega.TempoRecalculo.visible(false);
    });

    HeaderAuditoria("ConfiguracaoOcorrenciaEntrega", _configuracaoOcorrenciaEntrega);

    new BuscarTipoOcorrencia(_configuracaoOcorrenciaEntrega.TipoOcorrencia, null, null, null, null, null, null, _configuracaoOcorrenciaEntrega.TipoAplicacaoColetaEntrega, true);
    new BuscarTipoOcorrencia(_pesquisaConfiguracaoOcorrenciaEntrega.TipoOcorrencia, null, null, null, null, null, null, _pesquisaConfiguracaoOcorrenciaEntrega.TipoAplicacaoColetaEntrega, true);

    new BuscarTiposOperacao(_configuracaoOcorrenciaEntrega.TipoOperacao);
    buscarConfiguracaoOcorrenciaEntrega();
}

function adicionarClick(e, sender) {
    Salvar(_configuracaoOcorrenciaEntrega, "ConfiguracaoOcorrenciaEntrega/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridConfiguracaoOcorrenciaEntrega.CarregarGrid();
                limparCamposConfiguracaoOcorrenciaEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_configuracaoOcorrenciaEntrega, "ConfiguracaoOcorrenciaEntrega/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridConfiguracaoOcorrenciaEntrega.CarregarGrid();
                limparCamposConfiguracaoOcorrenciaEntrega();
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
        ExcluirPorCodigo(_configuracaoOcorrenciaEntrega, "ConfiguracaoOcorrenciaEntrega/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridConfiguracaoOcorrenciaEntrega.CarregarGrid();
                    limparCamposConfiguracaoOcorrenciaEntrega();
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
    limparCamposConfiguracaoOcorrenciaEntrega();
}

function editarConfiguracaoOcorrenciaEntregaClick(itemGrid) {
    // Limpa os campos
    limparCamposConfiguracaoOcorrenciaEntrega();

    _configuracaoOcorrenciaEntrega.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_configuracaoOcorrenciaEntrega, "ConfiguracaoOcorrenciaEntrega/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaConfiguracaoOcorrenciaEntrega.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _configuracaoOcorrenciaEntrega.Atualizar.visible(true);
                _configuracaoOcorrenciaEntrega.Excluir.visible(true);
                _configuracaoOcorrenciaEntrega.Cancelar.visible(true);
                _configuracaoOcorrenciaEntrega.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarConfiguracaoOcorrenciaEntrega() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoOcorrenciaEntregaClick, tamanho: "7", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "ConfiguracaoOcorrenciaEntrega/ExportarPesquisa",
        titulo: "Motivo Rejeição"
    };


    // Inicia Grid de busca
    _gridConfiguracaoOcorrenciaEntrega = new GridViewExportacao(_pesquisaConfiguracaoOcorrenciaEntrega.Pesquisar.idGrid, "ConfiguracaoOcorrenciaEntrega/Pesquisa", _pesquisaConfiguracaoOcorrenciaEntrega, menuOpcoes, configExportacao);
    _gridConfiguracaoOcorrenciaEntrega.CarregarGrid();
}

function limparCamposConfiguracaoOcorrenciaEntrega() {
    _configuracaoOcorrenciaEntrega.Atualizar.visible(false);
    _configuracaoOcorrenciaEntrega.Cancelar.visible(false);
    _configuracaoOcorrenciaEntrega.Excluir.visible(false);
    _configuracaoOcorrenciaEntrega.Adicionar.visible(true);
    LimparCampos(_configuracaoOcorrenciaEntrega);
}