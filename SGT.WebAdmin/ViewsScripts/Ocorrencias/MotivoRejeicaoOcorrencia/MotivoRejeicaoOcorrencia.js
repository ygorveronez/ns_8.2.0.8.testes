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
/// <reference path="../../Enumeradores/EnumAprovacaoRejeicao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _motivoRejeicaoOcorrencia;
var _pesquisaMotivoRejeicaoOcorrencia;
var _CRUDMotivoRejeicaoOcorrencia;
var _gridMotivoRejeicaoOcorrencia;

var MotivoRejeicaoOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "*Status: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumAprovacaoRejeicao.Rejeicao), options: EnumAprovacaoRejeicao.obterOpcoes(), def: EnumAprovacaoRejeicao.Rejeicao });
    this.NaoPermitirAbrirOcorrenciaDuplicadaRejeicao = PropertyEntity({ text: "Não permitir abrir uma nova Ocorrência para uma Carga e Tipo de Ocorrência que já contenham uma Ocorrência rejeitada com este Motivo", val: ko.observable(false), getType: typesKnockout.bool, visible: ko.observable(this.Tipo.val() === EnumAprovacaoRejeicao.Rejeicao) });
    this.Observacao = PropertyEntity({ text: "Observação:", issue: 593, getType: typesKnockout.string, val: ko.observable("") });

    this.Tipo.val.subscribe((value) => {
        if (value === EnumAprovacaoRejeicao.Rejeicao)
            this.NaoPermitirAbrirOcorrenciaDuplicadaRejeicao.visible(true);
        else {
            this.NaoPermitirAbrirOcorrenciaDuplicadaRejeicao.visible(false);
        }
    });
};

var CRUDMotivoRejeicaoOcorrencia = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaMotivoRejeicaoOcorrencia = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Tipo = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumAprovacaoRejeicao.Todos), options: EnumAprovacaoRejeicao.obterOpcoesPesquisa(), def: EnumAprovacaoRejeicao.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoRejeicaoOcorrencia.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadMotivoRejeicaoOcorrencia() {

    _pesquisaMotivoRejeicaoOcorrencia = new PesquisaMotivoRejeicaoOcorrencia();
    KoBindings(_pesquisaMotivoRejeicaoOcorrencia, "knockoutPesquisaMotivoRejeicaoOcorrencia", false, _pesquisaMotivoRejeicaoOcorrencia.Pesquisar.id);

    _motivoRejeicaoOcorrencia = new MotivoRejeicaoOcorrencia();
    KoBindings(_motivoRejeicaoOcorrencia, "knockoutMotivoRejeicaoOcorrencia");

    HeaderAuditoria("MotivoRejeicaoOcorrencia", _motivoRejeicaoOcorrencia);

    _CRUDMotivoRejeicaoOcorrencia = new CRUDMotivoRejeicaoOcorrencia();
    KoBindings(_CRUDMotivoRejeicaoOcorrencia, "knockoutCRUDMotivoRejeicaoOcorrencia");

    BuscarMotivoRejeicaoOcorrencia();
}

function adicionarClick(e, sender) {
    Salvar(_motivoRejeicaoOcorrencia, "MotivoRejeicaoOcorrencia/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoRejeicaoOcorrencia.CarregarGrid();
                limparCamposMotivoRejeicaoOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoRejeicaoOcorrencia, "MotivoRejeicaoOcorrencia/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoRejeicaoOcorrencia.CarregarGrid();
                limparCamposMotivoRejeicaoOcorrencia();
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
        ExcluirPorCodigo(_motivoRejeicaoOcorrencia, "MotivoRejeicaoOcorrencia/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoRejeicaoOcorrencia.CarregarGrid();
                    limparCamposMotivoRejeicaoOcorrencia();
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
    limparCamposMotivoRejeicaoOcorrencia();
}

function editarMotivoRejeicaoOcorrenciaClick(itemGrid) {
    limparCamposMotivoRejeicaoOcorrencia();

    _motivoRejeicaoOcorrencia.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_motivoRejeicaoOcorrencia, "MotivoRejeicaoOcorrencia/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMotivoRejeicaoOcorrencia.ExibirFiltros.visibleFade(false);

                _CRUDMotivoRejeicaoOcorrencia.Atualizar.visible(true);
                _CRUDMotivoRejeicaoOcorrencia.Excluir.visible(true);
                _CRUDMotivoRejeicaoOcorrencia.Cancelar.visible(true);
                _CRUDMotivoRejeicaoOcorrencia.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function BuscarMotivoRejeicaoOcorrencia() {

    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoRejeicaoOcorrenciaClick, tamanho: "10", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "MotivoRejeicaoOcorrencia/ExportarPesquisa",
        titulo: "Motivos de Rejeição de Ocorrência"
    };

    _gridMotivoRejeicaoOcorrencia = new GridViewExportacao(_pesquisaMotivoRejeicaoOcorrencia.Pesquisar.idGrid, "MotivoRejeicaoOcorrencia/Pesquisa", _pesquisaMotivoRejeicaoOcorrencia, menuOpcoes, configExportacao);
    _gridMotivoRejeicaoOcorrencia.CarregarGrid();
}

function limparCamposMotivoRejeicaoOcorrencia() {
    _CRUDMotivoRejeicaoOcorrencia.Atualizar.visible(false);
    _CRUDMotivoRejeicaoOcorrencia.Cancelar.visible(false);
    _CRUDMotivoRejeicaoOcorrencia.Excluir.visible(false);
    _CRUDMotivoRejeicaoOcorrencia.Adicionar.visible(true);
    LimparCampos(_motivoRejeicaoOcorrencia);
}