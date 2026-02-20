/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoRejeicaoColeta;
var _motivoRejeicaoColeta;
var _pesquisaMotivoRejeicaoColeta;
var _gridMotivoRejeicaoColeta;

/*
 * Declaração das Classes
 */

var CRUDMotivoRejeicaoColeta = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var MotivoRejeicaoColeta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });

    this.Produtos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var PesquisaMotivoRejeicaoColeta = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridMotivoRejeicaoColeta, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridMotivoRejeicaoColeta() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoRejeicaoColeta/ExportarPesquisa", titulo: "Motivos de Rejeição de Coleta" };

    _gridMotivoRejeicaoColeta = new GridViewExportacao(_pesquisaMotivoRejeicaoColeta.Pesquisar.idGrid, "MotivoRejeicaoColeta/Pesquisa", _pesquisaMotivoRejeicaoColeta, menuOpcoes, configuracoesExportacao);
    _gridMotivoRejeicaoColeta.CarregarGrid();
}

function LoadMotivoRejeicaoColeta() {
    _motivoRejeicaoColeta = new MotivoRejeicaoColeta();
    KoBindings(_motivoRejeicaoColeta, "knockoutMotivoRejeicaoColeta");

    HeaderAuditoria("MotivoRejeicaoColeta", _motivoRejeicaoColeta);

    _CRUDMotivoRejeicaoColeta = new CRUDMotivoRejeicaoColeta();
    KoBindings(_CRUDMotivoRejeicaoColeta, "knockoutCRUDMotivoRejeicaoColeta");

    _pesquisaMotivoRejeicaoColeta = new PesquisaMotivoRejeicaoColeta();
    KoBindings(_pesquisaMotivoRejeicaoColeta, "knockoutPesquisaMotivoRejeicaoColeta", false, _pesquisaMotivoRejeicaoColeta.Pesquisar.id);

    LoadProdutoMotivoRejeicaoColeta();

    LoadGridMotivoRejeicaoColeta();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    _motivoRejeicaoColeta.Produtos.val(JSON.stringify(_produtoMotivoRejeicaoColeta.Produto.basicTable.BuscarRegistros()));

    Salvar(_motivoRejeicaoColeta, "MotivoRejeicaoColeta/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridMotivoRejeicaoColeta();
                LimparCamposMotivoRejeicaoColeta();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    _motivoRejeicaoColeta.Produtos.val(JSON.stringify(_produtoMotivoRejeicaoColeta.Produto.basicTable.BuscarRegistros()));

    Salvar(_motivoRejeicaoColeta, "MotivoRejeicaoColeta/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridMotivoRejeicaoColeta();
                LimparCamposMotivoRejeicaoColeta();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposMotivoRejeicaoColeta();
}

function EditarClick(registroSelecionado) {
    LimparCamposMotivoRejeicaoColeta();

    _motivoRejeicaoColeta.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoRejeicaoColeta, "MotivoRejeicaoColeta/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var isEdicao = true;

                _pesquisaMotivoRejeicaoColeta.ExibirFiltros.visibleFade(false);

                RecarregarGridProdutoMotivoRejeicaoColeta();
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
        ExcluirPorCodigo(_motivoRejeicaoColeta, "MotivoRejeicaoColeta/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridMotivoRejeicaoColeta();
                    LimparCamposMotivoRejeicaoColeta();
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
    _CRUDMotivoRejeicaoColeta.Atualizar.visible(isEdicao);
    _CRUDMotivoRejeicaoColeta.Excluir.visible(isEdicao);
    _CRUDMotivoRejeicaoColeta.Cancelar.visible(isEdicao);
    _CRUDMotivoRejeicaoColeta.Adicionar.visible(!isEdicao);
}

function LimparCamposMotivoRejeicaoColeta() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_motivoRejeicaoColeta);

    LimparCamposProdutoMotivoRejeicaoColeta();
    RecarregarGridProdutoMotivoRejeicaoColeta();

    Global.ResetarAbas();
}

function RecarregarGridMotivoRejeicaoColeta() {
    _gridMotivoRejeicaoColeta.CarregarGrid();
}