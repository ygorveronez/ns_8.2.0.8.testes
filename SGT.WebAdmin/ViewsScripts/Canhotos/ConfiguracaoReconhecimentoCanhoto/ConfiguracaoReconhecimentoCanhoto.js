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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoReconhecimentoCanhoto;
var _configuracaoReconhecimentoCanhoto;
var _pesquisaConfiguracaoReconhecimentoCanhoto;

var PesquisaConfiguracaoReconhecimentoCanhoto = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoReconhecimentoCanhoto.CarregarGrid();
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

var ConfiguracaoReconhecimentoCanhoto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.PalavrasChaves = PropertyEntity({ text: "*Palavras Chaves (separado por \";\"): ", required: true });
    this.QuantidadePalavras = PropertyEntity({ text: "Quantidade palavras: ", value: ko.computed(ContadorPalavras(this)), def: 0 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadConfiguracaoReconhecimentoCanhoto() {
    _configuracaoReconhecimentoCanhoto = new ConfiguracaoReconhecimentoCanhoto();
    KoBindings(_configuracaoReconhecimentoCanhoto, "knockoutCadastroConfiguracaoReconhecimentoCanhoto");

    _pesquisaConfiguracaoReconhecimentoCanhoto = new PesquisaConfiguracaoReconhecimentoCanhoto();
    KoBindings(_pesquisaConfiguracaoReconhecimentoCanhoto, "knockoutPesquisaConfiguracaoReconhecimentoCanhoto", false, _pesquisaConfiguracaoReconhecimentoCanhoto.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoReconhecimentoCanhoto", _configuracaoReconhecimentoCanhoto);

    BuscarConfiguracaoReconhecimentoCanhotos();
}

function adicionarClick(e, sender) {
    Salvar(e, "ConfiguracaoReconhecimentoCanhoto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridConfiguracaoReconhecimentoCanhoto.CarregarGrid();
                LimparCamposConfiguracaoReconhecimentoCanhoto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ConfiguracaoReconhecimentoCanhoto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridConfiguracaoReconhecimentoCanhoto.CarregarGrid();
                LimparCamposConfiguracaoReconhecimentoCanhoto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração de reconhecimento " + _configuracaoReconhecimentoCanhoto.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_configuracaoReconhecimentoCanhoto, "ConfiguracaoReconhecimentoCanhoto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridConfiguracaoReconhecimentoCanhoto.CarregarGrid();
                LimparCamposConfiguracaoReconhecimentoCanhoto();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    LimparCamposConfiguracaoReconhecimentoCanhoto();
}

//*******MÉTODOS*******

function ContadorPalavras(context) {
    return function () {
        var texto = context.PalavrasChaves.val() || "";
        var palavras = texto.split(';');
        var palavrasValidas = palavras.filter(function (palavra) {
            return palavra.trim().length;
        });

        return palavrasValidas.length;
    }
}

function BuscarConfiguracaoReconhecimentoCanhotos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarConfiguracaoReconhecimentoCanhoto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridConfiguracaoReconhecimentoCanhoto = new GridView(_pesquisaConfiguracaoReconhecimentoCanhoto.Pesquisar.idGrid, "ConfiguracaoReconhecimentoCanhoto/Pesquisa", _pesquisaConfiguracaoReconhecimentoCanhoto, menuOpcoes, null);
    _gridConfiguracaoReconhecimentoCanhoto.CarregarGrid();
}

function EditarConfiguracaoReconhecimentoCanhoto(item) {
    LimparCamposConfiguracaoReconhecimentoCanhoto();
    _configuracaoReconhecimentoCanhoto.Codigo.val(item.Codigo);
    BuscarPorCodigo(_configuracaoReconhecimentoCanhoto, "ConfiguracaoReconhecimentoCanhoto/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoReconhecimentoCanhoto.ExibirFiltros.visibleFade(false);
        _configuracaoReconhecimentoCanhoto.Atualizar.visible(true);
        _configuracaoReconhecimentoCanhoto.Cancelar.visible(true);
        _configuracaoReconhecimentoCanhoto.Excluir.visible(true);
        _configuracaoReconhecimentoCanhoto.Adicionar.visible(false);
    }, null);
}

function LimparCamposConfiguracaoReconhecimentoCanhoto() {
    _configuracaoReconhecimentoCanhoto.Atualizar.visible(false);
    _configuracaoReconhecimentoCanhoto.Cancelar.visible(false);
    _configuracaoReconhecimentoCanhoto.Excluir.visible(false);
    _configuracaoReconhecimentoCanhoto.Adicionar.visible(true);
    LimparCampos(_configuracaoReconhecimentoCanhoto);
}
