/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Filial.js" />

var _gridProcedimentoEmbarque, _procedimentoEmbarque, _CRUDProcedimentoEmbarque, _pesquisaProcedimentoEmbarque;

var ProcedimentoEmbarque = function () {
    this.Codigo = PropertyEntity({ getType: types.int, val: ko.observable("") });
    this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid() });
    this.ProcedimentoEmbarque = PropertyEntity({ getType: typesKnockout.int, text: "Procedimento de Embarque:", val: ko.observable(""), required: true });
    this.ModeloContratacao = PropertyEntity({ getType: typesKnockout.int, text: "Modelo de Contratação:", val: ko.observable("") });
    this.TempoEntrega = PropertyEntity({ getType: typesKnockout.int, text: "Tempo para Entrega(minutos):", val: ko.observable("") });
    this.NaoEnviarDataInicioTermino = PropertyEntity({ getType: typesKnockout.bool, text: "Não enviar data de início e término de viagem", val: ko.observable(false) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação:", issue: 557 });

}

var CRUDProcedimentoEmbarque = function () {
    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", eventClick: AdicionarProcedimentoEmbarque, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ type: types.event, text: "Atualizar", eventClick: AtualizarProcedimentoEmbarque, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ type: types.event, text: "Excluir", eventClick: ExcluirProcedimentoEmbarque, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ type: types.event, text: "Cancelar", eventClick: CancelarProcedimentoEmbarque, visible: ko.observable(true) });
}

var PesquisaProcedimentoEmbarque = function () {
    this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação:", cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-2 col-lg-2") });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridProcedimentoEmbarque, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

//Funções Load
function LoadProcedimentoEmbarque() {
    _procedimentoEmbarque = new ProcedimentoEmbarque();
    KoBindings(_procedimentoEmbarque, "knockoutProcedimentoEmbarque");

    _CRUDProcedimentoEmbarque = new CRUDProcedimentoEmbarque();
    KoBindings(_CRUDProcedimentoEmbarque, "knockoutCRUDProcedimentoEmbarque");

    _pesquisaProcedimentoEmbarque = new PesquisaProcedimentoEmbarque();
    KoBindings(_pesquisaProcedimentoEmbarque, "knockoutPesquisaProcedimentoEmbarque");

    HeaderAuditoria("ProcedimentoEmbarque", _procedimentoEmbarque);

    new BuscarTiposOperacao(_procedimentoEmbarque.TipoOperacao);
    new BuscarFilial(_procedimentoEmbarque.Filial);

    new BuscarTiposOperacao(_pesquisaProcedimentoEmbarque.TipoOperacao);
    new BuscarFilial(_pesquisaProcedimentoEmbarque.Filial);

    LoadGridProcedimentoEmbarque();
}

function LoadGridProcedimentoEmbarque() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "ProcedimentoEmbarque/ExportarPesquisa", titulo: "Procedimento Embarque" };

    _gridProcedimentoEmbarque = new GridViewExportacao(_pesquisaProcedimentoEmbarque.Pesquisar.idGrid, "ProcedimentoEmbarque/Pesquisa", _pesquisaProcedimentoEmbarque, menuOpcoes, configuracoesExportacao);
    _gridProcedimentoEmbarque.CarregarGrid();
}

//Funções Click
function AdicionarProcedimentoEmbarque(e, sender) {
    if (!ValidarCamposObrigatorios(_procedimentoEmbarque)) {
        exibirMensagem("atencao", "Preencha os campos obrigatórios.")
        return;
    }

    Salvar(_procedimentoEmbarque, "ProcedimentoEmbarque/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridProcedimentoEmbarque();
                LimparCampos(_procedimentoEmbarque);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarProcedimentoEmbarque(e, sender) {
    Salvar(_procedimentoEmbarque, "ProcedimentoEmbarque/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridProcedimentoEmbarque();
                LimparCampos(_procedimentoEmbarque);
                ControlarBotoesHabilitados(false);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function ExcluirProcedimentoEmbarque() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_procedimentoEmbarque, "ProcedimentoEmbarque/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridProcedimentoEmbarque();
                    LimparCampos(_procedimentoEmbarque);
                    ControlarBotoesHabilitados(false);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function CancelarProcedimentoEmbarque() {
    ControlarBotoesHabilitados(false);
    LimparCampos(_procedimentoEmbarque);
}

function EditarClick(registroSelecionado) {
    LimparCampos(_procedimentoEmbarque);
    _procedimentoEmbarque.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_procedimentoEmbarque, "ProcedimentoEmbarque/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaProcedimentoEmbarque.ExibirFiltros.visibleFade(false);
                _procedimentoEmbarque.TipoOperacao.codEntity(retorno.Data.CodigoTipoOperacao);
                _procedimentoEmbarque.Filial.codEntity(retorno.Data.CodigoFilial);
                _procedimentoEmbarque.TipoOperacao.val(retorno.Data.TipoOperacao);
                _procedimentoEmbarque.Filial.val(retorno.Data.Filial);
                var isEdicao = true;

                ControlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

//Funções privadas
function RecarregarGridProcedimentoEmbarque() {
    _gridProcedimentoEmbarque.CarregarGrid();
}

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDProcedimentoEmbarque.Atualizar.visible(isEdicao);
    _CRUDProcedimentoEmbarque.Excluir.visible(isEdicao);
    _CRUDProcedimentoEmbarque.Cancelar.visible(isEdicao);
    _CRUDProcedimentoEmbarque.Adicionar.visible(!isEdicao);
}
