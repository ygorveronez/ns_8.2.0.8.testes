var _CRUDJustificativaCancelamentoCarga;
var _justificativaCancelamentoCarga;
var _pesquisaJustificativaCancelamentoCarga;
var _gridJustificativaCancelamentoCarga;

var CRUDJustificativaCancelamentoCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var JustificativaCancelamentoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.MotivoCancelamento = PropertyEntity({ text: "Motivo de Cancelamento:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
};

var PesquisaJustificativaCancelamentoCarga = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridJustificativaCancelamentoCarga, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

function LoadGridJustificativaCancelamentoCarga() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "JustificativaCancelamentoCarga/ExportarPesquisa", titulo: "Justificativa Cancelamento Carga" };

    _gridJustificativaCancelamentoCarga = new GridViewExportacao(_pesquisaJustificativaCancelamentoCarga.Pesquisar.idGrid, "JustificativaCancelamentoCarga/Pesquisa", _pesquisaJustificativaCancelamentoCarga, menuOpcoes, configuracoesExportacao);
    _gridJustificativaCancelamentoCarga.CarregarGrid();
}

function LoadJustificativaCancelamentoCarga() {
    _justificativaCancelamentoCarga = new JustificativaCancelamentoCarga();
    KoBindings(_justificativaCancelamentoCarga, "knockoutJustificativaCancelamentoCarga");

    HeaderAuditoria("JustificativaCancelamentoCarga", _justificativaCancelamentoCarga);

    _CRUDJustificativaCancelamentoCarga = new CRUDJustificativaCancelamentoCarga();
    KoBindings(_CRUDJustificativaCancelamentoCarga, "knockoutCRUDJustificativaCancelamentoCarga");

    _pesquisaJustificativaCancelamentoCarga = new PesquisaJustificativaCancelamentoCarga();
    KoBindings(_pesquisaJustificativaCancelamentoCarga, "knockoutPesquisaJustificativaCancelamentoCarga", false, _pesquisaJustificativaCancelamentoCarga.Pesquisar.id);

    LoadGridJustificativaCancelamentoCarga();
}

function AdicionarClick(e, sender) {
    Salvar(_justificativaCancelamentoCarga, "JustificativaCancelamentoCarga/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridJustificativaCancelamentoCarga();
                LimparCamposJustificativaCancelamentoCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_justificativaCancelamentoCarga, "JustificativaCancelamentoCarga/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridJustificativaCancelamentoCarga();
                LimparCamposJustificativaCancelamentoCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposJustificativaCancelamentoCarga();
}

function EditarClick(registroSelecionado) {
    LimparCamposJustificativaCancelamentoCarga();

    _justificativaCancelamentoCarga.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_justificativaCancelamentoCarga, "JustificativaCancelamentoCarga/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaJustificativaCancelamentoCarga.ExibirFiltros.visibleFade(false);

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

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_justificativaCancelamentoCarga, "JustificativaCancelamentoCarga/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridJustificativaCancelamentoCarga();
                    LimparCamposJustificativaCancelamentoCarga();
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
    _CRUDJustificativaCancelamentoCarga.Atualizar.visible(isEdicao);
    _CRUDJustificativaCancelamentoCarga.Excluir.visible(isEdicao);
    _CRUDJustificativaCancelamentoCarga.Cancelar.visible(isEdicao);
    _CRUDJustificativaCancelamentoCarga.Adicionar.visible(!isEdicao);
}

function LimparCamposJustificativaCancelamentoCarga() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_justificativaCancelamentoCarga);
}

function RecarregarGridJustificativaCancelamentoCarga() {
    _gridJustificativaCancelamentoCarga.CarregarGrid();
}