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
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridControleTacografo;
var _controleTacografo;
var _pesquisaControleTacografo;

var _situacao = [
    { text: "Entregue", value: 1},
    { text: "Recebido", value: 2 },
    { text: "Perdido", value: 3 },
    { text: "Extraviado", value: 4 }
];

var PesquisaControleTacografo = function () {
    this.Codigo = PropertyEntity({ text: "Nº Disco: ", getType: typesKnockout.int });
    this.DataRecebimentoInicial = PropertyEntity({ text: "Data Recebimento Inicial: ", getType: typesKnockout.date });
    this.DataRecebimentoFinal = PropertyEntity({ text: "Data Recebimento Final: ", getType: typesKnockout.date });
    this.Excesso = PropertyEntity({ text: "Excesso de Velocidade:", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleTacografo.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ControleTacografo = function () {
    this.Codigo = PropertyEntity({ text: "*Nº Disco:", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false)});
    this.DataRecebimento = PropertyEntity({ text: "*Data Recebimento: ", getType: typesKnockout.date, required: ko.observable(true) });
    this.DataRetorno = PropertyEntity({ text: "Data Retorno do Motorista: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ text: "Situacao: ", options: _situacao, def: 1});
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true) });
    this.Excesso = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Excesso de Velocidade?" });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, val: ko.observable(""), enable: ko.observable(true) });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), required: ko.observable(true) });
};

var CRUDControleTacografo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.GerarEtiqueta = PropertyEntity({ eventClick: gerarEtiquetaClick, type: types.event, text: "Etiqueta", visible: ko.observable(false) });
    this.GerarLoteTacografo = PropertyEntity({ eventClick: gerarLoteTacografoClick, type: types.event, text: "Gerar Lote de Tacógrafo", visible: ko.observable(true) });
};


//*******EVENTOS*******


function loadControleTacografo() {
    _controleTacografo = new ControleTacografo();
    KoBindings(_controleTacografo, "knockoutCadastroControleTacografo");

    HeaderAuditoria("ControleTacografo", _controleTacografo);

    _crudControleTacografo = new CRUDControleTacografo();
    KoBindings(_crudControleTacografo, "knockoutCRUDControleTacografo");

    _pesquisaControleTacografo = new PesquisaControleTacografo();
    KoBindings(_pesquisaControleTacografo, "knockoutPesquisaControleTacografo", false, _pesquisaControleTacografo.Pesquisar.id);

    new BuscarVeiculos(_pesquisaControleTacografo.Veiculo);
    new BuscarMotoristas(_pesquisaControleTacografo.Motorista);
    new BuscarVeiculos(_controleTacografo.Veiculo);
    new BuscarMotoristas(_controleTacografo.Motorista, RetornoMotorista);

    buscarControleTacografo();
    LoadLoteControleTacografo();
}

function adicionarClick(e, sender) {
    Salvar(_controleTacografo, "ControleTacografo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridControleTacografo.CarregarGrid();
                limparCamposControleTacografo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_controleTacografo, "ControleTacografo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridControleTacografo.CarregarGrid();
                limparCamposControleTacografo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Controle de Tacógrafo?", function () {
        ExcluirPorCodigo(_controleTacografo, "ControleTacografo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridControleTacografo.CarregarGrid();
                limparCamposControleTacografo();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposControleTacografo();
}

function gerarLoteTacografoClick() {
    abrirTelaLoteTacografo();
}

function gerarEtiquetaClick(e, sender) {
    executarReST("ControleTacografo/ImprimirEtiqueta", { Codigo: _controleTacografo.Codigo.val() }, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde a geração do arquivo da impressão de etiqueta.");
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function RetornoMotorista(data) {
    _controleTacografo.Motorista.codEntity(data.Codigo);
    _controleTacografo.Motorista.val(data.Descricao);

    executarReST("Veiculo/BuscarVeiculoDoMotorista", { Codigo: data.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _controleTacografo.Veiculo.codEntity(arg.Data.Codigo);
                _controleTacografo.Veiculo.val(arg.Data.Placa);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function buscarControleTacografo() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarControleTacografo, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "ControleTacografo/ExportarPesquisa",
        titulo: "Controle de Tacógrafo"
    };

    _gridControleTacografo = new GridViewExportacao(_pesquisaControleTacografo.Pesquisar.idGrid, "ControleTacografo/Pesquisa", _pesquisaControleTacografo, menuOpcoes, configExportacao);
    _gridControleTacografo.CarregarGrid();
}

function editarControleTacografo(controleTacografoGrid) {
    limparCamposControleTacografo();
    _controleTacografo.Codigo.val(controleTacografoGrid.Codigo);
    BuscarPorCodigo(_controleTacografo, "ControleTacografo/BuscarPorCodigo", function (arg) {
        _pesquisaControleTacografo.ExibirFiltros.visibleFade(false);
        _crudControleTacografo.Atualizar.visible(true);
        _crudControleTacografo.Cancelar.visible(true);
        _crudControleTacografo.Excluir.visible(true);
        _crudControleTacografo.GerarEtiqueta.visible(true);
        _crudControleTacografo.Adicionar.visible(false);
    }, null);
}

function limparCamposControleTacografo() {
    _crudControleTacografo.Atualizar.visible(false);
    _crudControleTacografo.Cancelar.visible(false);
    _crudControleTacografo.Excluir.visible(false);
    _crudControleTacografo.Adicionar.visible(true);
    _crudControleTacografo.GerarEtiqueta.visible(false);
    LimparCampos(_controleTacografo);
}