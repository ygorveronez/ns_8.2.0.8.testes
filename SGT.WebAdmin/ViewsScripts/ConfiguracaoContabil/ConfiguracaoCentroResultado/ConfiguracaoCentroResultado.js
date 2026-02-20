/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
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
/// <reference path="../../Enumeradores/EnumTipoCliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridConfiguracaoCentroResultado;
var _configuracaoCentroResultado;
var _pesquisaConfiguracaoCentroResultado;

var PesquisaConfiguracaoCentroResultado = function () {

    this.TipoRemetente = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Remetente: ", required: false, eventChange: function () { tipoRemetenteChange(_pesquisaConfiguracaoCentroResultado); } });
    this.TipoDestinatario = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Destinatário: ", required: false, eventChange: function () { tipoDestinatarioChange(_pesquisaConfiguracaoCentroResultado); } });
    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Tomador: ", required: false, eventChange: function () { tipoTomadorChange(_pesquisaConfiguracaoCentroResultado); } });

    this.CategoriaRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.GrupoRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:",issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:",issue:52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:",issue: 972, idBtnSearch: guid(), visible: ko.observable(false) });

    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:",issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:",issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", issue: 972, idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tipo de Operação: "), issue: 121, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: false, text: "Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410 });
    this.Empresas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de resultado:", idBtnSearch: guid(), required: false });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoCentroResultado.CarregarGrid();
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

var ConfiguracaoCentroResultado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });


    this.TipoRemetente = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Remetente: ", required: false, eventChange: function () { tipoRemetenteChange(_configuracaoCentroResultado); }  });
    this.TipoDestinatario = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Destinatario: ", required: false, eventChange: function () { tipoDestinatarioChange(_configuracaoCentroResultado); } });
    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: EnumTipoCliente.Pessoa, text: "Tipo Tomador: ", required: false, eventChange: function () { tipoTomadorChange(_configuracaoCentroResultado); } });

    this.CategoriaRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.GrupoRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota:", idBtnSearch: guid() });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid() });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });


    this.ValorMaximoCentroContabilizacao = PropertyEntity({ text: "Valor máximo centro para contabilização:", getType: typesKnockout.decimal, visible: ko.observable(false), enable: ko.observable(true) });
    this.CentroResultadoEscrituracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de resultado Escrituração:", idBtnSearch: guid(), required: true });
    this.CentroResultadoContabilizacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de resultado Contabilização:", idBtnSearch: guid(), required: true });

    this.CentroResultadoICMS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de resultado ICMS:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.CentroResultadoPedidoObrigatorio) });
    this.CentroResultadoPIS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de resultado PIS:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.CentroResultadoPedidoObrigatorio) });
    this.CentroResultadoCOFINS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de resultado COFINS:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.CentroResultadoPedidoObrigatorio) });


    this.ItemServico = PropertyEntity({ text: "Item do Serviço: ", maxlength: 30 });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: false, text: "Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410 });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Copiar = PropertyEntity({ eventClick: copiarDadosClick, type: types.event, text: "Copiar", visible: ko.observable(false) });


    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "ConfiguracaoCentroResultado/Importar",
        UrlConfiguracao: "ConfiguracaoCentroResultado/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O016_ConfiguracaoCentroResultado,
        CallbackImportacao: function () {
            _gridConfiguracaoCentroResultado.CarregarGrid();
        }
    });
}


//*******EVENTOS*******

function loadConfiguracaoCentroResultado() {

    _configuracaoCentroResultado = new ConfiguracaoCentroResultado();
    KoBindings(_configuracaoCentroResultado, "knockoutCadastroConfiguracaoCentroResultado");

    _pesquisaConfiguracaoCentroResultado = new PesquisaConfiguracaoCentroResultado();
    KoBindings(_pesquisaConfiguracaoCentroResultado, "knockoutPesquisaConfiguracaoCentroResultado", false, _pesquisaConfiguracaoCentroResultado.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoCentroResultado", _configuracaoCentroResultado);

    new BuscarClientes(_pesquisaConfiguracaoCentroResultado.Expedidor);
    new BuscarClientes(_pesquisaConfiguracaoCentroResultado.Recebedor);
    new BuscarClientes(_pesquisaConfiguracaoCentroResultado.Remetente);
    new BuscarClientes(_pesquisaConfiguracaoCentroResultado.Destinatario);
    new BuscarClientes(_pesquisaConfiguracaoCentroResultado.Tomador);
    new BuscarCategoriaPessoa(_pesquisaConfiguracaoCentroResultado.CategoriaDestinatario);
    new BuscarCategoriaPessoa(_pesquisaConfiguracaoCentroResultado.CategoriaRemetente);
    new BuscarCategoriaPessoa(_pesquisaConfiguracaoCentroResultado.CategoriaTomador);
    new BuscarGruposPessoas(_pesquisaConfiguracaoCentroResultado.GrupoDestinatario, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_pesquisaConfiguracaoCentroResultado.GrupoRemetente, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_pesquisaConfiguracaoCentroResultado.GrupoTomador, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarTiposOperacao(_pesquisaConfiguracaoCentroResultado.TipoOperacao);
    new BuscarTipoOcorrencia(_pesquisaConfiguracaoCentroResultado.TipoOcorrencia);
    new BuscarTransportadores(_pesquisaConfiguracaoCentroResultado.Empresas);
    new BuscarCentroResultado(_pesquisaConfiguracaoCentroResultado.CentroResultado);


    new BuscarClientes(_configuracaoCentroResultado.Remetente);
    new BuscarClientes(_configuracaoCentroResultado.Destinatario);
    new BuscarClientes(_configuracaoCentroResultado.Tomador);
    new BuscarClientes(_configuracaoCentroResultado.Expedidor);
    new BuscarClientes(_configuracaoCentroResultado.Recebedor);
    new BuscarCategoriaPessoa(_configuracaoCentroResultado.CategoriaDestinatario);
    new BuscarCategoriaPessoa(_configuracaoCentroResultado.CategoriaRemetente);
    new BuscarCategoriaPessoa(_configuracaoCentroResultado.CategoriaTomador);
    new BuscarGruposPessoas(_configuracaoCentroResultado.GrupoDestinatario, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_configuracaoCentroResultado.GrupoRemetente, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_configuracaoCentroResultado.GrupoTomador, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarTiposOperacao(_configuracaoCentroResultado.TipoOperacao);
    new BuscarFilial(_configuracaoCentroResultado.Filial);
    new BuscarLocalidades(_configuracaoCentroResultado.Origem);

    new BuscarRotasFrete(_configuracaoCentroResultado.RotaFrete);
    new BuscarGruposProdutos(_configuracaoCentroResultado.GrupoProduto);
    new BuscarCentroResultado(_configuracaoCentroResultado.CentroResultadoContabilizacao);
    new BuscarCentroResultado(_configuracaoCentroResultado.CentroResultadoICMS);
    new BuscarCentroResultado(_configuracaoCentroResultado.CentroResultadoPIS);
    new BuscarCentroResultado(_configuracaoCentroResultado.CentroResultadoCOFINS);
    new BuscarCentroResultado(_configuracaoCentroResultado.CentroResultadoEscrituracao);
    new BuscarTransportadores(_configuracaoCentroResultado.Empresa);
    new BuscarTipoOcorrencia(_configuracaoCentroResultado.TipoOcorrencia);

    buscarConfiguracaoCentroResultado();

}

function copiarDadosClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja copiar os dados dessa configuração para outra configuração? lembrando que a atual configuração não será alterada após essa ação.", function () {
        _configuracaoCentroResultado.Codigo.val(0);
        resetarBotoes();
    });
}

function adicionarClick(e, sender) {
    Salvar(e, "ConfiguracaoCentroResultado/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridConfiguracaoCentroResultado.CarregarGrid();
                limparCamposConfiguracaoCentroResultado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ConfiguracaoCentroResultado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridConfiguracaoCentroResultado.CarregarGrid();
                limparCamposConfiguracaoCentroResultado();
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
        ExcluirPorCodigo(_configuracaoCentroResultado, "ConfiguracaoCentroResultado/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridConfiguracaoCentroResultado.CarregarGrid();
                limparCamposConfiguracaoCentroResultado();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposConfiguracaoCentroResultado();
}

//*******MÉTODOS*******

function buscarConfiguracaoCentroResultado() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoCentroResultado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridConfiguracaoCentroResultado = new GridView(_pesquisaConfiguracaoCentroResultado.Pesquisar.idGrid, "ConfiguracaoCentroResultado/Pesquisa", _pesquisaConfiguracaoCentroResultado, menuOpcoes);
    _gridConfiguracaoCentroResultado.CarregarGrid();
}

function controlarExibicaoCampo(campo, exibirCampo) {
    if (exibirCampo)
        campo.visible(true);
    else {
        campo.visible(false);
        LimparCampo(campo);
    }
}

function editarConfiguracaoCentroResultado(configuracaoCentroResultadoGrid) {
    limparCamposConfiguracaoCentroResultado();
    _configuracaoCentroResultado.Codigo.val(configuracaoCentroResultadoGrid.Codigo);
    BuscarPorCodigo(_configuracaoCentroResultado, "ConfiguracaoCentroResultado/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoCentroResultado.ExibirFiltros.visibleFade(false);
        _configuracaoCentroResultado.Atualizar.visible(true);
        _configuracaoCentroResultado.Cancelar.visible(true);
        _configuracaoCentroResultado.Excluir.visible(true);
        _configuracaoCentroResultado.Copiar.visible(true);
        _configuracaoCentroResultado.Adicionar.visible(false);

        if (_configuracaoCentroResultado.CategoriaDestinatario.codEntity() > 0)
            _configuracaoCentroResultado.TipoDestinatario.val(EnumTipoCliente.CategoriaPessoa);
        else if (_configuracaoCentroResultado.GrupoDestinatario.codEntity() > 0)
            _configuracaoCentroResultado.TipoDestinatario.val(EnumTipoCliente.GrupoPessoa);
        else
            _configuracaoCentroResultado.TipoDestinatario.val(EnumTipoCliente.Pessoa);

        tipoDestinatarioChange(_configuracaoCentroResultado);

        if (_configuracaoCentroResultado.CategoriaRemetente.codEntity() > 0)
            _configuracaoCentroResultado.TipoRemetente.val(EnumTipoCliente.CategoriaPessoa);
        else if (_configuracaoCentroResultado.GrupoRemetente.codEntity() > 0)
            _configuracaoCentroResultado.TipoRemetente.val(EnumTipoCliente.GrupoPessoa);
        else
            _configuracaoCentroResultado.TipoRemetente.val(EnumTipoCliente.Pessoa);

        tipoRemetenteChange(_configuracaoCentroResultado);

        if (_configuracaoCentroResultado.CategoriaTomador.codEntity() > 0)
            _configuracaoCentroResultado.TipoTomador.val(EnumTipoCliente.CategoriaPessoa);
        else if (_configuracaoCentroResultado.GrupoTomador.codEntity() > 0)
            _configuracaoCentroResultado.TipoTomador.val(EnumTipoCliente.GrupoPessoa);
        else
            _configuracaoCentroResultado.TipoTomador.val(EnumTipoCliente.Pessoa);

        tipoTomadorChange(_configuracaoCentroResultado);
    }, null);
}

function limparCamposConfiguracaoCentroResultado() {
    _configuracaoCentroResultado.TipoTomador.val(1);
    _configuracaoCentroResultado.TipoDestinatario.val(1);
    _configuracaoCentroResultado.TipoRemetente.val(1);
    resetarBotoes();
    LimparCampos(_configuracaoCentroResultado);
}

function resetarBotoes() {
    _configuracaoCentroResultado.Atualizar.visible(false);
    _configuracaoCentroResultado.Cancelar.visible(false);
    _configuracaoCentroResultado.Excluir.visible(false);
    _configuracaoCentroResultado.Copiar.visible(false);
    _configuracaoCentroResultado.Adicionar.visible(true);
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