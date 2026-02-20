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
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/CategoriaPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/CanalVenda.js" />
/// <reference path="../../Consultas/TipoDocumentoTransporte.js" />
/// <reference path="../../Enumeradores/EnumTipoCliente.js" />
/// <reference path="../../Enumeradores/EnumTipoContaContabil.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoContaContabil;
var _configuracaoContaContabil;
var _pesquisaConfiguracaoContaContabil;
var _CRUDconfiguracaoContaContabil;

var PesquisaConfiguracaoContaContabil = function () {

    this.TipoRemetente = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Remetente: ", required: false, eventChange: function () { tipoRemetenteChange(_pesquisaConfiguracaoContaContabil); } });
    this.TipoDestinatario = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Destinatário: ", required: false, eventChange: function () { tipoDestinatarioChange(_pesquisaConfiguracaoContaContabil); } });
    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Tomador: ", required: false, eventChange: function () { tipoTomadorChange(_pesquisaConfiguracaoContaContabil); } });

    this.CategoriaRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.GrupoRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", issue: 972, idBtnSearch: guid(), visible: ko.observable(false) });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", issue: 972, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tipo de Operação: "), issue: 121, idBtnSearch: guid(), visible: ko.observable(true) });

    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do Documento", enable: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(true), issue: 370 });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: false, text: "Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410 });

    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Canal de Entrega:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CanalVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Canal de Venda:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoDT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de DT:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoContaContabil.CarregarGrid();
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

var ConfiguracaoContaContabil = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });


    this.TipoRemetente = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Remetente: ", required: false, eventChange: function () { tipoRemetenteChange(_configuracaoContaContabil); } });
    this.TipoDestinatario = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Destinatário: ", required: false, eventChange: function () { tipoDestinatarioChange(_configuracaoContaContabil); } });
    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Tomador: ", required: false, eventChange: function () { tipoTomadorChange(_configuracaoContaContabil); } });

    this.CategoriaRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.GrupoRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", issue: 972, idBtnSearch: guid(), visible: ko.observable(false) });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", issue: 972, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", issue: 121, idBtnSearch: guid() });
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota:", issue: 830, idBtnSearch: guid() });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", issue: 549, idBtnSearch: guid() });

    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Canal de Entrega:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CanalVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Canal de Venda:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoDT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de DT:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.ConfiguracaoContaContabilContabilizacaos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ConfiguracaoContaContabilEscrituracaos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ConfiguracaoContaContabilProvisoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do Documento", enable: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(true), issue: 370 });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: false, text: "Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410 });

}

var CRUDConfiguracaoContaContabil = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Copiar = PropertyEntity({ eventClick: copiarDadosClick, type: types.event, text: "Copiar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadConfiguracaoContaContabil() {

    _CRUDconfiguracaoContaContabil = new CRUDConfiguracaoContaContabil();
    KoBindings(_CRUDconfiguracaoContaContabil, "knockoutCRUDCadastroConfiguracaoContaContabil");

    _configuracaoContaContabil = new ConfiguracaoContaContabil();
    KoBindings(_configuracaoContaContabil, "knockoutCadastroConfiguracaoContaContabil");

    _pesquisaConfiguracaoContaContabil = new PesquisaConfiguracaoContaContabil();
    KoBindings(_pesquisaConfiguracaoContaContabil, "knockoutPesquisaConfiguracaoContaContabil", false, _pesquisaConfiguracaoContaContabil.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoContaContabil", _configuracaoContaContabil);

    loadConfiguracaoContaContabilContabilizacao(function () {
        loadConfiguracaoContaContabilEscrituracao(function () {
            loadConfiguracaoContaContabilProvisao(function () {
                new BuscarClientes(_pesquisaConfiguracaoContaContabil.Remetente);
                new BuscarClientes(_pesquisaConfiguracaoContaContabil.Destinatario);
                new BuscarClientes(_pesquisaConfiguracaoContaContabil.Tomador);
                new BuscarCategoriaPessoa(_pesquisaConfiguracaoContaContabil.CategoriaDestinatario);
                new BuscarCategoriaPessoa(_pesquisaConfiguracaoContaContabil.CategoriaRemetente);
                new BuscarCategoriaPessoa(_pesquisaConfiguracaoContaContabil.CategoriaTomador);
                new BuscarGruposPessoas(_pesquisaConfiguracaoContaContabil.GrupoDestinatario, null, null, null, EnumTipoGrupoPessoas.Clientes);
                new BuscarGruposPessoas(_pesquisaConfiguracaoContaContabil.GrupoRemetente, null, null, null, EnumTipoGrupoPessoas.Clientes);
                new BuscarGruposPessoas(_pesquisaConfiguracaoContaContabil.GrupoTomador, null, null, null, EnumTipoGrupoPessoas.Clientes);
                new BuscarTiposOperacao(_pesquisaConfiguracaoContaContabil.TipoOperacao);
                new BuscarTransportadores(_pesquisaConfiguracaoContaContabil.Empresa);
                new BuscarModeloDocumentoFiscal(_pesquisaConfiguracaoContaContabil.ModeloDocumentoFiscal, null, null, false, true, null, true);
                new BuscarTipoOcorrencia(_pesquisaConfiguracaoContaContabil.TipoOcorrencia);
                new BuscarCanaisEntrega(_pesquisaConfiguracaoContaContabil.CanalEntrega);
                new BuscarCanaisVenda(_pesquisaConfiguracaoContaContabil.CanalVenda);
                new BuscarTipoDocumentoTransporte(_pesquisaConfiguracaoContaContabil.TipoDT);

                new BuscarClientes(_configuracaoContaContabil.Remetente);
                new BuscarClientes(_configuracaoContaContabil.Destinatario);
                new BuscarClientes(_configuracaoContaContabil.Tomador);
                new BuscarCategoriaPessoa(_configuracaoContaContabil.CategoriaDestinatario);
                new BuscarCategoriaPessoa(_configuracaoContaContabil.CategoriaRemetente);
                new BuscarCategoriaPessoa(_configuracaoContaContabil.CategoriaTomador);
                new BuscarGruposPessoas(_configuracaoContaContabil.GrupoDestinatario, null, null, null, EnumTipoGrupoPessoas.Clientes);
                new BuscarGruposPessoas(_configuracaoContaContabil.GrupoRemetente, null, null, null, EnumTipoGrupoPessoas.Clientes);
                new BuscarGruposPessoas(_configuracaoContaContabil.GrupoTomador, null, null, null, EnumTipoGrupoPessoas.Clientes);
                new BuscarTiposOperacao(_configuracaoContaContabil.TipoOperacao);
                new BuscarRotasFrete(_configuracaoContaContabil.RotaFrete);
                new BuscarGruposProdutos(_configuracaoContaContabil.GrupoProduto);
                new BuscarTransportadores(_configuracaoContaContabil.Empresa);
                new BuscarModeloDocumentoFiscal(_configuracaoContaContabil.ModeloDocumentoFiscal, null, null, false, true, null, true);
                new BuscarTipoOcorrencia(_configuracaoContaContabil.TipoOcorrencia);
                new BuscarCanaisEntrega(_configuracaoContaContabil.CanalEntrega);
                new BuscarCanaisVenda(_configuracaoContaContabil.CanalVenda);
                new BuscarTipoDocumentoTransporte(_configuracaoContaContabil.TipoDT);
                buscarConfiguracaoContaContabil();
                buscarConfiguracaoIVA();
            });
        });
    });
}

function adicionarClick(e, sender) {
    Salvar(_configuracaoContaContabil, "ConfiguracaoContaContabil/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Cadastrado com sucesso");
                _gridConfiguracaoContaContabil.CarregarGrid();
                limparCamposConfiguracaoContaContabil();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function copiarDadosClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja copiar os dados dessa configuração para outra configuração? lembrando que a atual configuração não será alterada após essa ação.", function () {
        _configuracaoContaContabil.Codigo.val(0);
        resetarBotoes();
    });
}

function atualizarClick(e, sender) {
    Salvar(_configuracaoContaContabil, "ConfiguracaoContaContabil/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridConfiguracaoContaContabil.CarregarGrid();
                limparCamposConfiguracaoContaContabil();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração?", function () {
        ExcluirPorCodigo(_configuracaoContaContabil, "ConfiguracaoContaContabil/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridConfiguracaoContaContabil.CarregarGrid();
                limparCamposConfiguracaoContaContabil();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposConfiguracaoContaContabil();
}

//*******MÉTODOS*******

function buscarConfiguracaoContaContabil() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoContaContabil, tamanho: "5", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var configuracoesExportacao = { url: "ConfiguracaoContaContabil/ExportarPesquisa", titulo: "Configuração de Contas Contábeis" };

    _gridConfiguracaoContaContabil = new GridViewExportacao(_pesquisaConfiguracaoContaContabil.Pesquisar.idGrid, "ConfiguracaoContaContabil/Pesquisa", _pesquisaConfiguracaoContaContabil, menuOpcoes, configuracoesExportacao);
    _gridConfiguracaoContaContabil.CarregarGrid();
}

function controlarExibicaoCampo(campo, exibirCampo) {
    if (exibirCampo)
        campo.visible(true);
    else {
        campo.visible(false);
        LimparCampo(campo);
    }
}

function editarConfiguracaoContaContabil(configuracaoContaContabilGrid) {
    limparCamposConfiguracaoContaContabil();
    limparCamposConfiguracaoContaContabilContabilizacao();
    limparCamposConfiguracaoContaContabilEscrituracao();
    limparCamposConfiguracaoContaContabilProvisao();
    _configuracaoContaContabil.Codigo.val(configuracaoContaContabilGrid.Codigo);
    BuscarPorCodigo(_configuracaoContaContabil, "ConfiguracaoContaContabil/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoContaContabil.ExibirFiltros.visibleFade(false);
        _CRUDconfiguracaoContaContabil.Atualizar.visible(true);
        _CRUDconfiguracaoContaContabil.Copiar.visible(true);
        _CRUDconfiguracaoContaContabil.Cancelar.visible(true);
        _CRUDconfiguracaoContaContabil.Excluir.visible(true);
        _CRUDconfiguracaoContaContabil.Adicionar.visible(false);
        recarregarGridConfiguracaoContaContabilContabilizacao();
        recarregarGridConfiguracaoContaContabilEscrituracao();
        recarregarGridConfiguracaoContaContabilProvisao();

        if (_configuracaoContaContabil.CategoriaDestinatario.codEntity() > 0)
            _configuracaoContaContabil.TipoDestinatario.val(EnumTipoCliente.CategoriaPessoa);
        else if (_configuracaoContaContabil.GrupoDestinatario.codEntity() > 0)
            _configuracaoContaContabil.TipoDestinatario.val(EnumTipoCliente.GrupoPessoa);
        else
            _configuracaoContaContabil.TipoDestinatario.val(EnumTipoCliente.Pessoa);

        tipoDestinatarioChange(_configuracaoContaContabil);

        if (_configuracaoContaContabil.CategoriaRemetente.codEntity() > 0)
            _configuracaoContaContabil.TipoRemetente.val(EnumTipoCliente.CategoriaPessoa);
        else if (_configuracaoContaContabil.GrupoRemetente.codEntity() > 0)
            _configuracaoContaContabil.TipoRemetente.val(EnumTipoCliente.GrupoPessoa);
        else
            _configuracaoContaContabil.TipoRemetente.val(EnumTipoCliente.Pessoa);

        tipoRemetenteChange(_configuracaoContaContabil);

        if (_configuracaoContaContabil.CategoriaTomador.codEntity() > 0)
            _configuracaoContaContabil.TipoTomador.val(EnumTipoCliente.CategoriaPessoa);
        else if (_configuracaoContaContabil.GrupoTomador.codEntity() > 0)
            _configuracaoContaContabil.TipoTomador.val(EnumTipoCliente.GrupoPessoa);
        else
            _configuracaoContaContabil.TipoTomador.val(EnumTipoCliente.Pessoa);

        tipoTomadorChange(_configuracaoContaContabil);
    }, null);
}

function limparCamposConfiguracaoContaContabil() {
    _configuracaoContaContabil.TipoTomador.val(1);
    _configuracaoContaContabil.TipoDestinatario.val(1);
    _configuracaoContaContabil.TipoRemetente.val(1);
    resetarBotoes();
    LimparCampos(_configuracaoContaContabil);
    limparCamposConfiguracaoContaContabilEscrituracao();
    recarregarGridConfiguracaoContaContabilEscrituracao();

    limparCamposConfiguracaoContaContabilProvisao();
    recarregarGridConfiguracaoContaContabilProvisao();

    limparCamposConfiguracaoContaContabilContabilizacao();
    recarregarGridConfiguracaoContaContabilContabilizacao();
}

function resetarBotoes() {
    _CRUDconfiguracaoContaContabil.Atualizar.visible(false);
    _CRUDconfiguracaoContaContabil.Cancelar.visible(false);
    _CRUDconfiguracaoContaContabil.Excluir.visible(false);
    _CRUDconfiguracaoContaContabil.Copiar.visible(false);
    _CRUDconfiguracaoContaContabil.Adicionar.visible(true);
}

function tipoRemetenteChange(configuracao) {
    var exibirCategoriaRemetente = false;
    var exibirGrupoRemetente = false;
    var exibirRemetente = false;

    switch (configuracao.TipoRemetente.val()) {
        case EnumTipoCliente.CategoriaPessoa:
            exibirCategoriaRemetente = true;
            break;

        case EnumTipoCliente.GrupoPessoa:
            exibirGrupoRemetente = true;
            break;

        case EnumTipoCliente.Pessoa:
            exibirRemetente = true;
            break;
    }

    controlarExibicaoCampo(configuracao.CategoriaRemetente, exibirCategoriaRemetente);
    controlarExibicaoCampo(configuracao.GrupoRemetente, exibirGrupoRemetente);
    controlarExibicaoCampo(configuracao.Remetente, exibirRemetente);
}

function tipoDestinatarioChange(configuracao) {
    var exibirCategoriaDestinatario = false;
    var exibirGrupoDestinatario = false;
    var exibirDestinatario = false;

    switch (configuracao.TipoDestinatario.val()) {
        case EnumTipoCliente.CategoriaPessoa:
            exibirCategoriaDestinatario = true;
            break;

        case EnumTipoCliente.GrupoPessoa:
            exibirGrupoDestinatario = true;
            break;

        case EnumTipoCliente.Pessoa:
            exibirDestinatario = true;
            break;
    }

    controlarExibicaoCampo(configuracao.CategoriaDestinatario, exibirCategoriaDestinatario);
    controlarExibicaoCampo(configuracao.GrupoDestinatario, exibirGrupoDestinatario);
    controlarExibicaoCampo(configuracao.Destinatario, exibirDestinatario);
}

function tipoTomadorChange(configuracao) {
    var exibirCategoriaTomador = false;
    var exibirGrupoTomador = false;
    var exibirTomador = false;

    switch (configuracao.TipoTomador.val()) {
        case EnumTipoCliente.CategoriaPessoa:
            exibirCategoriaTomador = true;
            break;

        case EnumTipoCliente.GrupoPessoa:
            exibirGrupoTomador = true;
            break;

        case EnumTipoCliente.Pessoa:
            exibirTomador = true;
            break;
    }

    controlarExibicaoCampo(configuracao.CategoriaTomador, exibirCategoriaTomador);
    controlarExibicaoCampo(configuracao.GrupoTomador, exibirGrupoTomador);
    controlarExibicaoCampo(configuracao.Tomador, exibirTomador);
}

function buscarConfiguracaoIVA() {
    possuiIVA().then(alterarVisibilidadeCampos);
}

function possuiIVA() {
    return new Promise(resolve => {
        executarReST("ConfiguracaoContaContabil/PossuiIVA", {}, function (r) {
            if (r.Success) {
                if (r.Data) {
                    resolve(true);
                } else {
                    resolve(false);
                }
            } else {
                resolve(false);
            }
        });
    })
}

function alterarVisibilidadeCampos(status) {
    _pesquisaConfiguracaoContaContabil.CanalEntrega.visible(status);
    _pesquisaConfiguracaoContaContabil.CanalVenda.visible(status);
    _pesquisaConfiguracaoContaContabil.TipoDT.visible(status);

    _configuracaoContaContabil.CanalEntrega.visible(status);
    _configuracaoContaContabil.CanalVenda.visible(status);
    _configuracaoContaContabil.TipoDT.visible(status);

    if (status)
        _configuracaoContaContabilProvisao.TipoContaContabil.options(EnumTipoContaContabil.obterOpcoesIVA());
    else
        _configuracaoContaContabilProvisao.TipoContaContabil.options(EnumTipoContaContabil.obterOpcoes());
}