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

var _gridRegraAutomatizacaoEmissoesEmail;
var _regraAutomatizacaoEmissoesEmail;
var _pesquisaRegraAutomatizacaoEmissoesEmail;
var _gridRemetente;
var _gridDestinatario;

var PesquisaRegraAutomatizacaoEmissoesEmail = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.EmailDestino = PropertyEntity({ text: "E-mail de Destino: " });
    this.Remetente = PropertyEntity({ type: types.event, text: "Remetente", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.event, text: "Destinatário", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraAutomatizacaoEmissoesEmail.CarregarGrid();
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

var RegraAutomatizacaoEmissoesEmail = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.EmailDestino = PropertyEntity({ text: "*E-mail de Destino: ", required: ko.observable(true), maxlength: 500 });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });

    this.Remetente = PropertyEntity({ type: types.event, text: "Adicionar Remetente", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.event, text: "Adicionar Destinatário", idBtnSearch: guid() });

    this.Remetentes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Destinatarios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.GridRemetentes = PropertyEntity({ type: types.local });
    this.GridDestinatarios = PropertyEntity({ type: types.local });

    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDRegraAutomatizacaoEmissoesEmail = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadRegraAutomatizacaoEmissoesEmail() {
    HeaderAuditoria("RegraAutomatizacaoEmissoesEmail", _regraAutomatizacaoEmissoesEmail);

    _pesquisaRegraAutomatizacaoEmissoesEmail = new PesquisaRegraAutomatizacaoEmissoesEmail();
    KoBindings(_pesquisaRegraAutomatizacaoEmissoesEmail, "knockoutPesquisaRegraAutomatizacaoEmissoesEmail", false, _pesquisaRegraAutomatizacaoEmissoesEmail.Pesquisar.id);

    _regraAutomatizacaoEmissoesEmail = new RegraAutomatizacaoEmissoesEmail();
    KoBindings(_regraAutomatizacaoEmissoesEmail, "knockoutCadastroRegraAutomatizacaoEmissoesEmail");

    _crudRegraAutomatizacaoEmissoesEmail = new CRUDRegraAutomatizacaoEmissoesEmail();
    KoBindings(_crudRegraAutomatizacaoEmissoesEmail, "knockoutCRUDRegraAutomatizacaoEmissoesEmail");

    var excluirRemetente = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (data) { excluirRemetenteClick(data); }, tamanho: 15, icone: "" };
    var excluirDestinatario = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (data) { excluirDestinatarioClick(data); }, tamanho: 15, icone: "" };

    var menuOpcoesRemetente = { tipo: TypeOptionMenu.link, opcoes: [excluirRemetente] };
    var menuOpcoesDestinatario = { tipo: TypeOptionMenu.link, opcoes: [excluirDestinatario] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridRemetente = new BasicDataTable(_regraAutomatizacaoEmissoesEmail.GridRemetentes.id, header, menuOpcoesRemetente, { column: 1, dir: orderDir.asc });
    _gridDestinatario = new BasicDataTable(_regraAutomatizacaoEmissoesEmail.GridDestinatarios.id, header, menuOpcoesDestinatario, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_regraAutomatizacaoEmissoesEmail.TipoOperacao);
    new BuscarClientes(_regraAutomatizacaoEmissoesEmail.Remetente, null, null, null, null, _gridRemetente);
    new BuscarClientes(_regraAutomatizacaoEmissoesEmail.Destinatario, null, null, null, null, _gridDestinatario);

    _regraAutomatizacaoEmissoesEmail.Remetente.basicTable = _gridRemetente;
    _regraAutomatizacaoEmissoesEmail.Destinatario.basicTable = _gridDestinatario;

    recarregarGridRemetente();
    recarregarGridDestinatario();

    buscarRegraAutomatizacaoEmissoesEmail();
}

function adicionarClick(e, sender) {
    preencherListasSelecao();
    Salvar(_regraAutomatizacaoEmissoesEmail, "RegraAutomatizacaoEmissoesEmail/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridRegraAutomatizacaoEmissoesEmail.CarregarGrid();
                limparCamposRegraAutomatizacaoEmissoesEmail();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecao();
    Salvar(_regraAutomatizacaoEmissoesEmail, "RegraAutomatizacaoEmissoesEmail/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridRegraAutomatizacaoEmissoesEmail.CarregarGrid();
                limparCamposRegraAutomatizacaoEmissoesEmail();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Marca EPI " + _regraAutomatizacaoEmissoesEmail.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_regraAutomatizacaoEmissoesEmail, "RegraAutomatizacaoEmissoesEmail/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegraAutomatizacaoEmissoesEmail.CarregarGrid();
                    limparCamposRegraAutomatizacaoEmissoesEmail();
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
    limparCamposRegraAutomatizacaoEmissoesEmail();
}

//*******MÉTODOS*******

function buscarRegraAutomatizacaoEmissoesEmail() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraAutomatizacaoEmissoesEmail, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraAutomatizacaoEmissoesEmail = new GridView(_pesquisaRegraAutomatizacaoEmissoesEmail.Pesquisar.idGrid, "RegraAutomatizacaoEmissoesEmail/Pesquisa", _pesquisaRegraAutomatizacaoEmissoesEmail, menuOpcoes);
    _gridRegraAutomatizacaoEmissoesEmail.CarregarGrid();
}

function editarRegraAutomatizacaoEmissoesEmail(regraAutomatizacaoEmissoesEmailGrid) {
    limparCamposRegraAutomatizacaoEmissoesEmail();
    _regraAutomatizacaoEmissoesEmail.Codigo.val(regraAutomatizacaoEmissoesEmailGrid.Codigo);
    BuscarPorCodigo(_regraAutomatizacaoEmissoesEmail, "RegraAutomatizacaoEmissoesEmail/BuscarPorCodigo", function (arg) {
        recarregarGridRemetente();
        recarregarGridDestinatario();

        _pesquisaRegraAutomatizacaoEmissoesEmail.ExibirFiltros.visibleFade(false);
        _crudRegraAutomatizacaoEmissoesEmail.Atualizar.visible(true);
        _crudRegraAutomatizacaoEmissoesEmail.Cancelar.visible(true);
        _crudRegraAutomatizacaoEmissoesEmail.Excluir.visible(true);
        _crudRegraAutomatizacaoEmissoesEmail.Adicionar.visible(false);
    }, null);
}

function limparCamposRegraAutomatizacaoEmissoesEmail() {
    _crudRegraAutomatizacaoEmissoesEmail.Atualizar.visible(false);
    _crudRegraAutomatizacaoEmissoesEmail.Cancelar.visible(false);
    _crudRegraAutomatizacaoEmissoesEmail.Excluir.visible(false);
    _crudRegraAutomatizacaoEmissoesEmail.Adicionar.visible(true);
    LimparCampos(_regraAutomatizacaoEmissoesEmail);
    limparCamposDestinatario();
    limparCamposRemetente();
}

function recarregarGridRemetente() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_regraAutomatizacaoEmissoesEmail.Remetentes.val())) {
        $.each(_regraAutomatizacaoEmissoesEmail.Remetentes.val(), function (i, remetente) {
            var remetenteGrid = new Object();

            remetenteGrid.Codigo = remetente.Codigo;
            remetenteGrid.Descricao = remetente.Descricao;

            data.push(remetenteGrid);
        });
    }
    _gridRemetente.CarregarGrid(data);
}

function excluirRemetenteClick(data) {
    var remetenteGrid = _regraAutomatizacaoEmissoesEmail.Remetente.basicTable.BuscarRegistros();

    for (var i = 0; i < remetenteGrid.length; i++) {
        if (data.Codigo == remetenteGrid[i].Codigo) {
            remetenteGrid.splice(i, 1);
            break;
        }
    }

    _regraAutomatizacaoEmissoesEmail.Remetente.basicTable.CarregarGrid(remetenteGrid);
}

function limparCamposRemetente() {
    LimparCampo(_regraAutomatizacaoEmissoesEmail.Remetente);
    LimparCampo(_regraAutomatizacaoEmissoesEmail.GridRemetentes);
    _regraAutomatizacaoEmissoesEmail.Remetente.basicTable.CarregarGrid(new Array());
}

function recarregarGridDestinatario() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_regraAutomatizacaoEmissoesEmail.Destinatarios.val())) {
        $.each(_regraAutomatizacaoEmissoesEmail.Destinatarios.val(), function (i, destinatario) {
            var destinatarioGrid = new Object();

            destinatarioGrid.Codigo = destinatario.Codigo;
            destinatarioGrid.Descricao = destinatario.Descricao;

            data.push(destinatarioGrid);

        });

    }
    _gridDestinatario.CarregarGrid(data);
}

function excluirDestinatarioClick(data) {
    var destinatarioGrid = _regraAutomatizacaoEmissoesEmail.Destinatario.basicTable.BuscarRegistros();

    for (var i = 0; i < destinatarioGrid.length; i++) {
        if (data.Codigo == destinatarioGrid[i].Codigo) {
            destinatarioGrid.splice(i, 1);
            break;
        }
    }

    _regraAutomatizacaoEmissoesEmail.Destinatario.basicTable.CarregarGrid(destinatarioGrid);
}

function limparCamposDestinatario() {
    LimparCampo(_regraAutomatizacaoEmissoesEmail.Destinatario);
    LimparCampo(_regraAutomatizacaoEmissoesEmail.GridDestinatarios);
    _regraAutomatizacaoEmissoesEmail.Destinatario.basicTable.CarregarGrid(new Array());
}

function preencherListasSelecao() {
    _regraAutomatizacaoEmissoesEmail.Remetentes.val(JSON.stringify(_regraAutomatizacaoEmissoesEmail.Remetente.basicTable.BuscarRegistros()));
    _regraAutomatizacaoEmissoesEmail.Destinatarios.val(JSON.stringify(_regraAutomatizacaoEmissoesEmail.Destinatario.basicTable.BuscarRegistros()));
}