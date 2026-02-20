
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDBombaAbastecimento;
var _bombaAbastecimento;
var _pesquisaBombaAbastecimento;
var _gridBombaAbastecimento;

/*
 * Declaração das Classes
 */

var CRUDBombaAbastecimento = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var BombaAbastecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 150, required: true });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Local Armazenamento", idBtnSearch: guid(), required: true });
    this.CodigoBombaIntegracao = PropertyEntity({ text: "Código Bomba Integração:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoBicoIntegracao = PropertyEntity({ text: "Código Bico Integração:", required: false, getType: typesKnockout.string, val: ko.observable("") });

};

var PesquisaBombaAbastecimento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 150 });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local Armazenamento", idBtnSearch: guid() });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridBombaAbastecimento, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridBombaAbastecimento() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridBombaAbastecimento = new GridViewExportacao(_pesquisaBombaAbastecimento.Pesquisar.idGrid, "BombaAbastecimento/Pesquisa", _pesquisaBombaAbastecimento, menuOpcoes);
    _gridBombaAbastecimento.CarregarGrid();
}

function LoadBombaAbastecimento() {
    _bombaAbastecimento = new BombaAbastecimento();
    KoBindings(_bombaAbastecimento, "knockoutBombaAbastecimento");

    _CRUDBombaAbastecimento = new CRUDBombaAbastecimento();
    KoBindings(_CRUDBombaAbastecimento, "knockoutCRUDBombaAbastecimento");

    _pesquisaBombaAbastecimento = new PesquisaBombaAbastecimento();
    KoBindings(_pesquisaBombaAbastecimento, "knockoutPesquisaBombaAbastecimento", false, _pesquisaBombaAbastecimento.Pesquisar.id);

    BuscarLocalArmazenamentoProduto(_pesquisaBombaAbastecimento.LocalArmazenamento);
    BuscarLocalArmazenamentoProduto(_bombaAbastecimento.LocalArmazenamento, callbackLocalArmazenamentoProduto);

    LoadGridBombaAbastecimento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_bombaAbastecimento, "BombaAbastecimento/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridBombaAbastecimento();
                LimparCamposBombaAbastecimento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_bombaAbastecimento, "BombaAbastecimento/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridBombaAbastecimento();
                LimparCamposBombaAbastecimento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposBombaAbastecimento();
}

function EditarClick(registroSelecionado) {
    LimparCamposBombaAbastecimento();

    _bombaAbastecimento.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_bombaAbastecimento, "BombaAbastecimento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaBombaAbastecimento.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                ControlarBotoesHabilitados(isEdicao);
                RecarregarGridBombaAbastecimento();
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
        ExcluirPorCodigo(_bombaAbastecimento, "BombaAbastecimento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");

                    RecarregarGridBombaAbastecimento();
                    LimparCamposBombaAbastecimento();
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

function callbackLocalArmazenamentoProduto(data) {
    _bombaAbastecimento.LocalArmazenamento.codEntity(data.Codigo);
    _bombaAbastecimento.LocalArmazenamento.val(data.Descricao);
}

/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDBombaAbastecimento.Atualizar.visible(isEdicao);
    _CRUDBombaAbastecimento.Excluir.visible(isEdicao);
    _CRUDBombaAbastecimento.Cancelar.visible(isEdicao);
    _CRUDBombaAbastecimento.Adicionar.visible(!isEdicao);
}

function LimparCamposBombaAbastecimento() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_bombaAbastecimento);
}

function RecarregarGridBombaAbastecimento() {
    _gridBombaAbastecimento.CarregarGrid();
}