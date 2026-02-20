//#region Variaveis Globais

var _pesquisaMotivoAvaliacao;
var _motivoAvaliacao;
var _gridMotivoAvaliacao;
var _CRUDMotivoAvaliacao;

//#endregion

//#region Mapeamento Knockout

var MotivoAvaliacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.GerarAtendimentoAutomaticoQuandoAvalicaoForUmaEstrela = PropertyEntity({ val: ko.observable(false), visible: ko.observable(true), getType: typesKnockout.bool, text: "Gerar atendimento automático quando avaliação for 1 estrela", def: false });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Motivo:"), idBtnSearch: guid() });

    this.GerarAtendimentoAutomaticoQuandoAvalicaoForUmaEstrela.val.subscribe(function (novoValor) {
        if (novoValor) {
            _motivoAvaliacao.Motivo.text("*Motivo:");
            _motivoAvaliacao.Motivo.required = true;
        } else {
            _motivoAvaliacao.Motivo.text("Motivo:");
            _motivoAvaliacao.Motivo.required = false;
        }
    });
}

var PesquisaMotivoAvaliacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridMotivoAvaliacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

var CRUDMotivoAvaliacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

function LoadGridMotivoAvaliacao() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridMotivoAvaliacao = new GridViewExportacao(_pesquisaMotivoAvaliacao.Pesquisar.idGrid, "MotivoAvaliacao/Pesquisa", _pesquisaMotivoAvaliacao, menuOpcoes, null);
    _gridMotivoAvaliacao.CarregarGrid();
}

function LoadMotivoAvaliacao() {
    _pesquisaMotivoAvaliacao = new PesquisaMotivoAvaliacao();
    KoBindings(_pesquisaMotivoAvaliacao, "knockoutPesquisaMotivoAvaliacao");

    _motivoAvaliacao = new MotivoAvaliacao();
    KoBindings(_motivoAvaliacao, "knockoutMotivoAvaliacao");

    _CRUDMotivoAvaliacao = new CRUDMotivoAvaliacao();
    KoBindings(_CRUDMotivoAvaliacao, "knockoutCRUDMotivoAvaliacao");

    new BuscarMotivoChamado(_motivoAvaliacao.Motivo);

    LoadGridMotivoAvaliacao();
}

//#endregion

function RecarregarGridMotivoAvaliacao() {
    _gridMotivoAvaliacao.CarregarGrid();
}

function AdicionarClick(e, sender) {
    Salvar(_motivoAvaliacao, "MotivoAvaliacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridMotivoAvaliacao();
                LimparCamposMotivoAvaliacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_motivoAvaliacao, "MotivoAvaliacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridMotivoAvaliacao();
                LimparCamposMotivoAvaliacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposMotivoAvaliacao();
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_motivoAvaliacao, "MotivoAvaliacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridMotivoAvaliacao();
                    LimparCamposMotivoAvaliacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function EditarClick(registroSelecionado) {
    LimparCamposMotivoAvaliacao();

    _motivoAvaliacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoAvaliacao, "MotivoAvaliacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoAvaliacao.ExibirFiltros.visibleFade(false);

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

function LimparCamposMotivoAvaliacao() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_motivoAvaliacao);
}

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDMotivoAvaliacao.Atualizar.visible(isEdicao);
    _CRUDMotivoAvaliacao.Excluir.visible(isEdicao);
    _CRUDMotivoAvaliacao.Cancelar.visible(isEdicao);
    _CRUDMotivoAvaliacao.Adicionar.visible(!isEdicao);
}