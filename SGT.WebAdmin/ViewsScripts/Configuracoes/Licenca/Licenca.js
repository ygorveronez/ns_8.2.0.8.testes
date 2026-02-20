/// <reference path="../../Enumeradores/EnumTipoLicenca.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _gridLicenca, _licenca, _pesquisaLicenca, _crudLicenca;

var PesquisaLicenca = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoLicenca.Todos), options: EnumTipoLicenca.obterOpcoesPesquisa(), def: EnumTipoLicenca.Todos, text: Localization.Resources.Consultas.Licenca.Tipo.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLicenca.CarregarGrid();
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

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Licenca = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoLicenca.Geral), options: EnumTipoLicenca.obterOpcoes(), def: EnumTipoLicenca.Geral, text: Localization.Resources.Consultas.Licenca.Tipo.getFieldDescription() });
    this.Email = PropertyEntity({ text: "E-mail(s): ", getType: typesKnockout.multiplesEmails, maxlength: 1000 });
    this.GerarRequisicao = PropertyEntity({ text: "Gerar Requisição?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.BloquearCheckListComLicencaInvalida = PropertyEntity({ text: "Bloquear checklist se a licença não estiver vigente", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
};

var CRUDLicenca = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadLicenca() {
    _licenca = new Licenca();
    KoBindings(_licenca, "knockoutCadastro");

    HeaderAuditoria("Licenca", _licenca);

    _crudLicenca = new CRUDLicenca();
    KoBindings(_crudLicenca, "knockoutCRUD");

    _pesquisaLicenca = new PesquisaLicenca();
    KoBindings(_pesquisaLicenca, "knockoutPesquisaLicenca", false, _pesquisaLicenca.Pesquisar.id);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _licenca.BloquearCheckListComLicencaInvalida.visible(true);
    }

    buscarLicenca();
}

function adicionarClick(e, sender) {
    Salvar(_licenca, "Licenca/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridLicenca.CarregarGrid();
                limparCamposLicenca();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_licenca, "Licenca/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridLicenca.CarregarGrid();
                limparCamposLicenca();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a licença " + _licenca.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_licenca, "Licenca/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridLicenca.CarregarGrid();
                    limparCamposLicenca();
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
    limparCamposLicenca();
}

//*******MÉTODOS*******

function buscarLicenca() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarLicenca, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridLicenca = new GridView(_pesquisaLicenca.Pesquisar.idGrid, "Licenca/Pesquisa", _pesquisaLicenca, menuOpcoes, null);
    _gridLicenca.CarregarGrid();
}

function editarLicenca(arquivoGrid) {
    limparCamposLicenca();
    _licenca.Codigo.val(arquivoGrid.Codigo);

    BuscarPorCodigo(_licenca, "Licenca/BuscarPorCodigo", function (arg) {
        _pesquisaLicenca.ExibirFiltros.visibleFade(false);
        _crudLicenca.Atualizar.visible(true);
        _crudLicenca.Cancelar.visible(true);
        _crudLicenca.Excluir.visible(true);
        _crudLicenca.Adicionar.visible(false);

    }, null);
}

function limparCamposLicenca() {
    _crudLicenca.Atualizar.visible(false);
    _crudLicenca.Cancelar.visible(false);
    _crudLicenca.Excluir.visible(false);
    _crudLicenca.Adicionar.visible(true);

    LimparCampos(_licenca);
}