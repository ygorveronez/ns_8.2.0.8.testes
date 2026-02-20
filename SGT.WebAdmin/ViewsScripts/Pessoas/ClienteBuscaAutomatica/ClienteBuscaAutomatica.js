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
/// <reference path="../../Enumeradores/EnumTipoParticipacao.js" />

var _pesquisaBuscaAutomaticaCliente;
var _buscaAutomaticaCliente;
var _gridPesquisaBuscaAutomatica;

function PesquisaBuscaAutomaticaCliente() {
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col-12 col-md-6") });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade Origem:", idBtnSearch: guid(), required: true, issue: 16, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Gerais.Geral.Filial.getFieldDescription()), issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Cliente.getFieldDescription(), idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Filtrar = PropertyEntity({
        eventClick: function (e) {
            _gridPesquisaBuscaAutomatica.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Filtrar, idGrid: guid(), visible: ko.observable(true)
    });
}

function BuscaAutomaticaCliente() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col-12 col-md-6") });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade Origem:", idBtnSearch: guid(), required: true, issue: 16, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Gerais.Geral.Filial.getFieldDescription()), issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Cliente.getFieldDescription(), idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Situação: " });
    this.TipoParticipante = PropertyEntity({ val: ko.observable(EnumTipoParticipacao.Expedidor), options: EnumTipoParticipacao.obterOpcoesClienteBuscaAutomatica(), def: EnumTipoParticipacao.Expedidor, text: "Tipo de participante", enabled: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 100 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "ClienteBuscaAutomatica/Importar",
        UrlConfiguracao: "ClienteBuscaAutomatica/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O084_ClienteBuscaAutomatica,
        CallbackImportacao: function () {
            _gridPesquisaBuscaAutomatica.CarregarGrid();
        }
    });
};


function loadBuscaAutomaticaCliente() {
    _pesquisaBuscaAutomaticaCliente = new PesquisaBuscaAutomaticaCliente();
    KoBindings(_pesquisaBuscaAutomaticaCliente, "knockoutPesquisaBuscaAutomaticaCliente");

    _buscaAutomaticaCliente = new BuscaAutomaticaCliente();
    KoBindings(_buscaAutomaticaCliente, "knockoutCadastroBuscaAutomaticaCliente");

    BuscarClientes(_pesquisaBuscaAutomaticaCliente.Cliente);
    BuscarClientes(_pesquisaBuscaAutomaticaCliente.Remetente);
    BuscarClientes(_pesquisaBuscaAutomaticaCliente.Destinatario);
    BuscarLocalidades(_pesquisaBuscaAutomaticaCliente.Origem);
    BuscarFilial(_pesquisaBuscaAutomaticaCliente.Filial);


    BuscarClientes(_buscaAutomaticaCliente.Cliente);
    BuscarClientes(_buscaAutomaticaCliente.Remetente);
    BuscarClientes(_buscaAutomaticaCliente.Destinatario);
    BuscarLocalidades(_buscaAutomaticaCliente.Origem);
    BuscarFilial(_buscaAutomaticaCliente.Filial);

    loadGridPesquisaBuscaAutomatica();
}

function loadGridPesquisaBuscaAutomatica() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarCadastro, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "ClienteBuscaAutomatica/ExportarPesquisa",
        titulo: "Registros Clientes"
    };

    _gridPesquisaBuscaAutomatica = new GridViewExportacao("grid-pesquisa-clientes", "ClienteBuscaAutomatica/Pesquisa", _pesquisaBuscaAutomaticaCliente, menuOpcoes, configExportacao, { column: 6, dir: orderDir.asc });
    _gridPesquisaBuscaAutomatica.CarregarGrid();
}

function adicionarClick() {
    var dados = RetornarObjetoPesquisa(_buscaAutomaticaCliente);

    executarReST('ClienteBuscaAutomatica/Adicionar', dados, function (arg) {
        if (arg.Success) {
            _gridPesquisaBuscaAutomatica.CarregarGrid();
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
            LimparCampos(_buscaAutomaticaCliente);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function atualizarClick() {
    var dados = RetornarObjetoPesquisa(_buscaAutomaticaCliente);

    exibirConfirmacao("Confirmação", "Tem certeza que deseja atualizar o registro?", function () {
        executarReST('ClienteBuscaAutomatica/Atualizar', dados, function (arg) {
            if (arg.Success) {
                _gridPesquisaBuscaAutomatica.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadaComSucesso);
                LimparCampos(_buscaAutomaticaCliente);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function excluirClick() {
    exibirConfirmacao("Atenção", "Tem certeza que deseja excluir o registro?", function () {
        executarReST('ClienteBuscaAutomatica/Excluir', { Codigo: _buscaAutomaticaCliente.Codigo.val() }, function (arg) {
            if (arg.Success) {
                _gridPesquisaBuscaAutomatica.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidaComSucesso);
                LimparCampos(_buscaAutomaticaCliente);

                _buscaAutomaticaCliente.Adicionar.visible(true);
                _buscaAutomaticaCliente.Importar.visible(true);
                _buscaAutomaticaCliente.Atualizar.visible(false);
                _buscaAutomaticaCliente.Excluir.visible(false);
                _buscaAutomaticaCliente.Cancelar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    })
}

function editarCadastro(dados) {
    executarReST('ClienteBuscaAutomatica/BuscarPorCodigo', { Codigo: dados.Codigo }, function (arg) {
        if (arg.Success) {
            var data = { Data: arg.Data };

            PreencherObjetoKnout(_buscaAutomaticaCliente, data);

            _pesquisaBuscaAutomaticaCliente.ExibirFiltros.visibleFade(false);
            _buscaAutomaticaCliente.Adicionar.visible(false);
            _buscaAutomaticaCliente.Importar.visible(false);
            _buscaAutomaticaCliente.Atualizar.visible(true);
            _buscaAutomaticaCliente.Excluir.visible(true);
            _buscaAutomaticaCliente.Cancelar.visible(true);
        }
    });
}

function cancelarClick() {
    LimparCampos(_buscaAutomaticaCliente);

    _buscaAutomaticaCliente.Adicionar.visible(true);
    _buscaAutomaticaCliente.Importar.visible(true);
    _buscaAutomaticaCliente.Atualizar.visible(false);
    _buscaAutomaticaCliente.Excluir.visible(false);
    _buscaAutomaticaCliente.Cancelar.visible(false);
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}