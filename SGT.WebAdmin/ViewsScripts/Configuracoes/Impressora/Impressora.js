//*******MAPEAMENTO KNOUCKOUT*******

var _gridImpressora, _impressora, _pesquisaImpressora, _crudImpressora;

var _opcoesPesquisaTipoImpressora = [
    { value: "", text: "Todos" },
    { value: "C", text: "CTe/MDFe" },
    { value: "N", text: "NFe/Boleto" }
];

var _opcoesTipoImpressora = [
    { value: "", text: "Todos" },
    { value: "C", text: "CTe/MDFe" },
    { value: "N", text: "NFe/Boleto" }
];

var PesquisaImpressora = function () {
    this.Impressora = PropertyEntity({ text: "Impressora: " });
    this.Unidade = PropertyEntity({ text: "Unidade: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridImpressora.CarregarGrid();
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

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var Impressora = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Impressora = PropertyEntity({ text: "*Impressora: ", required: true });
    this.Unidade = PropertyEntity({ text: "*Número Unidade Impressão: ", required: true, getType: typesKnockout.int });
    this.Documento = PropertyEntity({ text: "*Documento:", options: _opcoesTipoImpressora, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });    
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", required: false, maxlength: 20, });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });
}

var CRUDImpressora = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadImpressora() {

    _impressora = new Impressora();
    KoBindings(_impressora, "knockoutCadastro");

    _crudImpressora = new CRUDImpressora();
    KoBindings(_crudImpressora, "knockoutCRUD");

    _pesquisaImpressora = new PesquisaImpressora();
    KoBindings(_pesquisaImpressora, "knockoutPesquisaImpressora", false, _pesquisaImpressora.Pesquisar.id);

    HeaderAuditoria("Impressora", _impressora, "Codigo");

    buscarImpressora();
}

function adicionarClick(e, sender) {
    Salvar(_impressora, "Impressora/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridImpressora.CarregarGrid();
                limparCamposImpressora();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_impressora, "Impressora/Atualizar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            _gridImpressora.CarregarGrid();
            limparCamposImpressora();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Impressora?", function () {
        ExcluirPorCodigo(_impressora, "Impressora/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridImpressora.CarregarGrid();
                    limparCamposImpressora();
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
    limparCamposImpressora();
}

//*******MÉTODOS*******

function buscarImpressora() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarImpressora, tamanho: "9", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridImpressora = new GridView(_pesquisaImpressora.Pesquisar.idGrid, "Impressora/Pesquisa", _pesquisaImpressora, menuOpcoes, null);
    _gridImpressora.CarregarGrid();
}

function editarImpressora(arquivoGrid) {
    limparCamposImpressora();
    _impressora.Codigo.val(arquivoGrid.Codigo);

    BuscarPorCodigo(_impressora, "Impressora/BuscarPorCodigo", function (arg) {
        _pesquisaImpressora.ExibirFiltros.visibleFade(false);
        _crudImpressora.Atualizar.visible(true);
        _crudImpressora.Cancelar.visible(true);
        _crudImpressora.Excluir.visible(true);
        _crudImpressora.Adicionar.visible(false);

    }, null);
}

function limparCamposImpressora() {
    _crudImpressora.Atualizar.visible(false);
    _crudImpressora.Cancelar.visible(false);
    _crudImpressora.Excluir.visible(false);
    _crudImpressora.Adicionar.visible(true);

    LimparCampos(_impressora);
}