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

var _restricaoEntrega;
var _pesquisaRestricaoEntrega;
var _gridRestricaoEntrega;

var RestricaoEntrega = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Email = PropertyEntity({ text: "Email do Responsável:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.CorVisualizacao = PropertyEntity({ text: "*Cor para Visualização:", required: true, def: "#000000", getType: typesKnockout.string, val: ko.observable("#000000"), enable: ko.observable(false), idColor: guid() });
    this.CodigoIntegracao = PropertyEntity({ text: "Cód de integração:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });
    this.PrimeiraEntrega = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deve tornar a primeira entrega pedidos com essa restrição?", def: false });
    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaRestricaoEntrega = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRestricaoEntrega.CarregarGrid();
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
function loadRestricaoEntrega() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaRestricaoEntrega = new PesquisaRestricaoEntrega();
    KoBindings(_pesquisaRestricaoEntrega, "knockoutPesquisaRestricaoEntrega", false, _pesquisaRestricaoEntrega.Pesquisar.id);

    // Instancia ProdutoAvaria
    _restricaoEntrega = new RestricaoEntrega();
    KoBindings(_restricaoEntrega, "knockoutRestricaoEntrega");

    HeaderAuditoria("RestricaoEntrega", _restricaoEntrega);
    $("#" + _restricaoEntrega.CorVisualizacao.idColor).colorselector({
        callback: function (value, color, title) {
            _restricaoEntrega.CorVisualizacao.val(color);
        }
    });

    // Inicia busca
    buscarRestricaoEntrega();
}

function adicionarClick(e, sender) {
    Salvar(_restricaoEntrega, "RestricaoEntrega/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridRestricaoEntrega.CarregarGrid();
                limparCamposRestricaoEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_restricaoEntrega, "RestricaoEntrega/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridRestricaoEntrega.CarregarGrid();
                limparCamposRestricaoEntrega();
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
        ExcluirPorCodigo(_restricaoEntrega, "RestricaoEntrega/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRestricaoEntrega.CarregarGrid();
                    limparCamposRestricaoEntrega();
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
    limparCamposRestricaoEntrega();
}

function editarRestricaoEntregaClick(itemGrid) {
    // Limpa os campos
    limparCamposRestricaoEntrega();

    // Seta o codigo do ProdutoAvaria
    _restricaoEntrega.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_restricaoEntrega, "RestricaoEntrega/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaRestricaoEntrega.ExibirFiltros.visibleFade(false);
                $("#" + _restricaoEntrega.CorVisualizacao.idColor).colorselector("setColor", _restricaoEntrega.CorVisualizacao.val());
                // Alternas os campos de CRUD
                _restricaoEntrega.Atualizar.visible(true);
                _restricaoEntrega.Excluir.visible(true);
                _restricaoEntrega.Cancelar.visible(true);
                _restricaoEntrega.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarRestricaoEntrega() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRestricaoEntregaClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "RestricaoEntrega/ExportarPesquisa",
        titulo: "Motivo Avaria"
    };


    // Inicia Grid de busca
    _gridRestricaoEntrega = new GridView(_pesquisaRestricaoEntrega.Pesquisar.idGrid, "RestricaoEntrega/Pesquisa", _pesquisaRestricaoEntrega, menuOpcoes);
    _gridRestricaoEntrega.CarregarGrid();
}

function limparCamposRestricaoEntrega() {
    _restricaoEntrega.Atualizar.visible(false);
    _restricaoEntrega.Cancelar.visible(false);
    _restricaoEntrega.Excluir.visible(false);
    _restricaoEntrega.Adicionar.visible(true);
    $("#" + _restricaoEntrega.CorVisualizacao.idColor).colorselector("setColor", "#000000");
    LimparCampos(_restricaoEntrega);
}
