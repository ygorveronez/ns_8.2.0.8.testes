/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoFalhaNotaFiscal;
var _motivoFalhaNotaFiscal;
var _pesquisaMotivoFalhaNotaFiscal;
var _gridMotivoFalhaNotaFiscal;

/*
 * Declaração das Classes
 */

var CRUDMotivoFalhaNotaFiscal = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var MotivoFalhaNotaFiscal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "*Observação:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
};

var PesquisaMotivoFalhaNotaFiscal = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridMotivoFalhaNotaFiscal, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridMotivoFalhaNotaFiscal() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoFalhaNotaFiscal/ExportarPesquisa", titulo: "Motivos de Rejeição de Coleta" };

    _gridMotivoFalhaNotaFiscal = new GridViewExportacao(_pesquisaMotivoFalhaNotaFiscal.Pesquisar.idGrid, "MotivoFalhaNotaFiscal/Pesquisa", _pesquisaMotivoFalhaNotaFiscal, menuOpcoes, configuracoesExportacao);
    _gridMotivoFalhaNotaFiscal.CarregarGrid();
}

function LoadMotivoFalhaNotaFiscal() {
    _motivoFalhaNotaFiscal = new MotivoFalhaNotaFiscal();
    KoBindings(_motivoFalhaNotaFiscal, "knockoutMotivoFalhaNotaFiscal");

    HeaderAuditoria("MotivoFalhaNotaFiscal", _motivoFalhaNotaFiscal);

    _CRUDMotivoFalhaNotaFiscal = new CRUDMotivoFalhaNotaFiscal();
    KoBindings(_CRUDMotivoFalhaNotaFiscal, "knockoutCRUDMotivoFalhaNotaFiscal");

    _pesquisaMotivoFalhaNotaFiscal = new PesquisaMotivoFalhaNotaFiscal();
    KoBindings(_pesquisaMotivoFalhaNotaFiscal, "knockoutPesquisaMotivoFalhaNotaFiscal", false, _pesquisaMotivoFalhaNotaFiscal.Pesquisar.id);

    LoadGridMotivoFalhaNotaFiscal();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_motivoFalhaNotaFiscal, "MotivoFalhaNotaFiscal/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridMotivoFalhaNotaFiscal();
                LimparCamposMotivoFalhaNotaFiscal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_motivoFalhaNotaFiscal, "MotivoFalhaNotaFiscal/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridMotivoFalhaNotaFiscal();
                LimparCamposMotivoFalhaNotaFiscal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposMotivoFalhaNotaFiscal();
}

function EditarClick(registroSelecionado) {
    LimparCamposMotivoFalhaNotaFiscal();

    _motivoFalhaNotaFiscal.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoFalhaNotaFiscal, "MotivoFalhaNotaFiscal/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var isEdicao = true;

                _pesquisaMotivoFalhaNotaFiscal.ExibirFiltros.visibleFade(false);

                ControlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir este cadastro?", function () {
        ExcluirPorCodigo(_motivoFalhaNotaFiscal, "MotivoFalhaNotaFiscal/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridMotivoFalhaNotaFiscal();
                    LimparCamposMotivoFalhaNotaFiscal();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDMotivoFalhaNotaFiscal.Atualizar.visible(isEdicao);
    _CRUDMotivoFalhaNotaFiscal.Excluir.visible(isEdicao);
    _CRUDMotivoFalhaNotaFiscal.Cancelar.visible(isEdicao);
    _CRUDMotivoFalhaNotaFiscal.Adicionar.visible(!isEdicao);
}

function LimparCamposMotivoFalhaNotaFiscal() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_motivoFalhaNotaFiscal);

    Global.ResetarAbas();
}

function RecarregarGridMotivoFalhaNotaFiscal() {
    _gridMotivoFalhaNotaFiscal.CarregarGrid();
}