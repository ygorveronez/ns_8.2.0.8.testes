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
/// <reference path="../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../ViewsScripts/Consultas/TipoOleo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLocalArmazenamentoProduto;
var _localArmazenamentoProduto;
var _pesquisaLocalArmazenamentoProduto;

var PesquisaLocalArmazenamentoProduto = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoOleo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Óleo:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLocalArmazenamentoProduto.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var LocalArmazenamentoProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", required: ko.observable(false), maxlength: 50 });
    this.TipoOleo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Óleo:", idBtnSearch: guid() });
    this.CapacidadeTotalLitros = PropertyEntity({ text: "Capacidade Total (Litros):", getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, maxlength: 10 });
    this.QuantidadeSinalizacaoLitros = PropertyEntity({ text: "Quantidade de Sinalizacao (Litros):", getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, maxlength: 10 });
    this.Regua = PropertyEntity({ text: "Régua:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 5 });
    this.Densidade = PropertyEntity({ text: "Densidade:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 5 });
    this.ControleAbastecimentoDisponivel = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "O tanque está disponível para controle de abastecimento" });
    this.Posto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Posto:", idBtnSearch: guid(), issue: 171, enable: ko.observable(true) });

    //Aba transferencia
    this.Transferencias = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });
    this.CodigoTransferencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoTransferencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataTransferencia = PropertyEntity({ text: "*Data Transferência:", required: false, getType: typesKnockout.date });
    this.LocalArmazenamentoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Local Armazenamento Destino:"), idBtnSearch: guid(), required: false });
    this.DescricaoTransferencia = PropertyEntity({ text: "*Descrição: ", required: ko.observable(false), maxlength: 500 });
    this.Saldo = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), required: true, text: "*Saldo", configDecimal: { precision: 4, allowZero: false }, maxlength: 10, enable: ko.observable(false), visible: true });
    this.QuantidadeTransferida = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), required: false, text: "*Quantidade", configDecimal: { precision: 0, allowZero: false }, maxlength: 10, enable: ko.observable(true), visible: true });
    this.AdicionarTransferencia = PropertyEntity({ eventClick: adicionarTransferenciaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.ExcluirTransferencia = PropertyEntity({ eventClick: excluirTransferenciaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.CancelarTransferencia = PropertyEntity({ eventClick: limparCamposTransferencias, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AtualizarTransferencia = PropertyEntity({ eventClick: atualizarTransferenciaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
};

var CRUDLocalArmazenamentoProduto = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};
 
//*******EVENTOS*******

function loadLocalArmazenamentoProduto() {
    _localArmazenamentoProduto = new LocalArmazenamentoProduto();
    KoBindings(_localArmazenamentoProduto, "knockoutCadastroLocalArmazenamentoProduto");

    HeaderAuditoria("LocalArmazenamentoProduto", _localArmazenamentoProduto);

    _crudLocalArmazenamentoProduto = new CRUDLocalArmazenamentoProduto();
    KoBindings(_crudLocalArmazenamentoProduto, "knockoutCRUDLocalArmazenamentoProduto");

    _pesquisaLocalArmazenamentoProduto = new PesquisaLocalArmazenamentoProduto();
    KoBindings(_pesquisaLocalArmazenamentoProduto, "knockoutPesquisaLocalArmazenamentoProduto", false, _pesquisaLocalArmazenamentoProduto.Pesquisar.id);

    BuscarEmpresa(_localArmazenamentoProduto.Empresa);
    BuscarTipoOleo(_localArmazenamentoProduto.TipoOleo);
    BuscarClientes(_localArmazenamentoProduto.Posto, RetornoPosto, false, [EnumModalidadePessoa.Fornecedor]);

    BuscarEmpresa(_pesquisaLocalArmazenamentoProduto.Empresa);
    BuscarTipoOleo(_pesquisaLocalArmazenamentoProduto.TipoOleo);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _localArmazenamentoProduto.Empresa.visible(true);
        _pesquisaLocalArmazenamentoProduto.Empresa.visible(true);
    }
    BuscarLocalArmazenamentoProduto(_localArmazenamentoProduto.LocalArmazenamentoDestino);

    buscarLocalArmazenamentoProduto();
    loadTransferencia();
}

function adicionarClick(e, sender) {
    Salvar(_localArmazenamentoProduto, "LocalArmazenamentoProduto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridLocalArmazenamentoProduto.CarregarGrid();
                limparCamposLocalArmazenamentoProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_localArmazenamentoProduto, "LocalArmazenamentoProduto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridLocalArmazenamentoProduto.CarregarGrid();
                limparCamposLocalArmazenamentoProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Local de Armazenamento " + _localArmazenamentoProduto.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_localArmazenamentoProduto, "LocalArmazenamentoProduto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridLocalArmazenamentoProduto.CarregarGrid();
                    limparCamposLocalArmazenamentoProduto();
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
    limparCamposLocalArmazenamentoProduto();
}

//*******MÉTODOS*******

function buscarLocalArmazenamentoProduto() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarLocalArmazenamentoProduto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridLocalArmazenamentoProduto = new GridView(_pesquisaLocalArmazenamentoProduto.Pesquisar.idGrid, "LocalArmazenamentoProduto/Pesquisa", _pesquisaLocalArmazenamentoProduto, menuOpcoes, null);
    _gridLocalArmazenamentoProduto.CarregarGrid();
}

function editarLocalArmazenamentoProduto(localArmazenamentoProdutoGrid) {
    limparCamposLocalArmazenamentoProduto();
    _localArmazenamentoProduto.Codigo.val(localArmazenamentoProdutoGrid.Codigo);


    BuscarPorCodigo(_localArmazenamentoProduto, "LocalArmazenamentoProduto/BuscarPorCodigo", function (arg) {
        _pesquisaLocalArmazenamentoProduto.ExibirFiltros.visibleFade(false);
        _crudLocalArmazenamentoProduto.Atualizar.visible(true);
        _crudLocalArmazenamentoProduto.Cancelar.visible(true);
        _crudLocalArmazenamentoProduto.Excluir.visible(true);
        _crudLocalArmazenamentoProduto.Adicionar.visible(false);
        carregarGridTransferencias(arg.Data.Transferencias);
    }, null);
}

function limparCamposLocalArmazenamentoProduto() {
    _crudLocalArmazenamentoProduto.Atualizar.visible(false);
    _crudLocalArmazenamentoProduto.Cancelar.visible(false);
    _crudLocalArmazenamentoProduto.Excluir.visible(false);
    _crudLocalArmazenamentoProduto.Adicionar.visible(true);
    LimparCampos(_localArmazenamentoProduto);

    recarregarGridTransferencias();
}

function RetornoPosto(data) {
    _localArmazenamentoProduto.Posto.val(data.Descricao);
    _localArmazenamentoProduto.Posto.codEntity(data.Codigo);
}