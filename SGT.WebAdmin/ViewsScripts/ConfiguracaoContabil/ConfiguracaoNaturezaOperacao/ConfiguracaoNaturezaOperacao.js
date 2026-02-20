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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoNaturezaOperacao;
var _configuracaoNaturezaOperacao;
var _pesquisaConfiguracaoNaturezaOperacao;
var _CRUDconfiguracaoNaturezaOperacao;

var _estados = [
    { text: "Não Selecionado", value: "" },
    { text: "Acre", value: "AC" },
    { text: "Alagoas", value: "AL" },
    { text: "Amapá", value: "AP" },
    { text: "Amazonas", value: "AM" },
    { text: "Bahia", value: "BA" },
    { text: "Ceará", value: "CE" },
    { text: "Distrito Federal", value: "DF" },
    { text: "Espírito Santo", value: "ES" },
    { text: "Goiás", value: "GO" },
    { text: "Maranhão", value: "MA" },
    { text: "Mato Grosso", value: "MT" },
    { text: "Mato Grosso do Sul", value: "MS" },
    { text: "Minas Gerais", value: "MG" },
    { text: "Pará", value: "PA" },
    { text: "Paraíba", value: "PB" },
    { text: "Paraná", value: "PR" },
    { text: "Pernambuco", value: "PE" },
    { text: "Piauí", value: "PI" },
    { text: "Rio de Janeiro", value: "RJ" },
    { text: "Rio Grande do Norte", value: "RN" },
    { text: "Rio Grande do Sul", value: "RS" },
    { text: "Rondônia", value: "RO" },
    { text: "Roraima", value: "RR" },
    { text: "Santa Catarina", value: "SC" },
    { text: "São Paulo", value: "SP" },
    { text: "Sergipe", value: "SE" },
    { text: "Tocantins", value: "TO" },
    { text: "Exterior", value: "EX" }];

var PesquisaConfiguracaoNaturezaOperacao = function () {

    this.TipoRemetente = PropertyEntity({ val: ko.observable(1), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: 1, text: "Tipo Remetente: ", required: false, eventChange: function () { tipoRemetenteChange(_pesquisaConfiguracaoNaturezaOperacao); } });
    this.TipoDestinatario = PropertyEntity({ val: ko.observable(1), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: 1, text: "Tipo Destinatário: ", required: false, eventChange: function () { tipoDestinatarioChange(_pesquisaConfiguracaoNaturezaOperacao); } });
    this.TipoTomador = PropertyEntity({ val: ko.observable(1), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: 1, text: "Tipo Tomador: ", required: false, eventChange: function () { tipoTomadorChange(_pesquisaConfiguracaoNaturezaOperacao); } });

    this.CategoriaRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.GrupoRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:",issue: 972, idBtnSearch: guid(), visible: ko.observable(false) });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:",issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:",issue: 52,  idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", issue: 972, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tipo de Operação: "),issue: 121, idBtnSearch: guid(), visible: ko.observable(true) });


    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoNaturezaOperacao.CarregarGrid();
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

var ConfiguracaoNaturezaOperacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });


    this.TipoRemetente = PropertyEntity({ val: ko.observable(1), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: 1, text: "Tipo Remetente: ", required: false, eventChange: function () { tipoRemetenteChange(_configuracaoNaturezaOperacao); } });
    this.TipoDestinatario = PropertyEntity({ val: ko.observable(1), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: 1, text: "Tipo Destinatário: ", required: false, eventChange: function () { tipoDestinatarioChange(_configuracaoNaturezaOperacao); } });
    this.TipoTomador = PropertyEntity({ val: ko.observable(1), type: types.local, options: EnumTipoCliente.obterOpcoes(), def: 1, text: "Tipo Tomador: ", required: false, eventChange: function () { tipoTomadorChange(_configuracaoNaturezaOperacao); } });

    this.CategoriaRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CategoriaTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.GrupoRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:",issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:",issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:",issue: 972, idBtnSearch: guid(), visible: ko.observable(false) });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:",issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", issue: 972, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:",issue: 121, idBtnSearch: guid() });
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota:",issue: 830, idBtnSearch: guid() });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:",issue:  549,  idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Documento Fiscal:",issue: 370, idBtnSearch: guid(), visible: ko.observable(true) });

    this.AtividadeRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Atividade Remetente:", idBtnSearch: guid() });
    this.AtividadeDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Atividade Destinatário:", idBtnSearch: guid() });
    this.AtividadeTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Atividade Tomador:", idBtnSearch: guid() });

    this.UFOrigem = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Origem: ", required: false, visible: ko.observable(true) });
    this.UFDestino = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Destino: ", required: false, visible: ko.observable(true) });

    this.EstadoEmissorDiferenteUFOrigem = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Estado do emissor (transportador) é diferente do estado de origem?", visible: ko.observable(true) });
    this.EstadoOrigemDiferente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Estado de Origem diferente do selecionado?", visible: ko.observable(true) });
    this.EstadoDestinoDiferente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Estado de Destino diferente do selecionado?", visible: ko.observable(true) });

    this.EstadoOrigemDiferenteUFDestino = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Quando o Estado de Origem for Diferente do Estado de Destino (Operação Fora do Estado) ?" });
    this.EstadoOrigemIgualUFDestino = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Quando o Estado de Origem for Igual ao Estado de Destino (Operação Dentro do Estado)?" });

    this.ConfiguracaoNaturezaOperacaoContabilizacaos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ConfiguracaoNaturezaOperacaoEscrituracaos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.NaturezaDaOperacaoCompra = PropertyEntity({ text: "*Natureza da Operação Compra:", required: true, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.NaturezaDaOperacaoVenda = PropertyEntity({ text: "*Natureza da Operação Venda:", required: true, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
}

var CRUDConfiguracaoNaturezaOperacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Copiar = PropertyEntity({ eventClick: copiarDadosClick, type: types.event, text: "Copiar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadConfiguracaoNaturezaOperacao() {

    _CRUDconfiguracaoNaturezaOperacao = new CRUDConfiguracaoNaturezaOperacao();
    KoBindings(_CRUDconfiguracaoNaturezaOperacao, "knockoutCRUDCadastroConfiguracaoNaturezaOperacao");

    _configuracaoNaturezaOperacao = new ConfiguracaoNaturezaOperacao();
    KoBindings(_configuracaoNaturezaOperacao, "knockoutCadastroConfiguracaoNaturezaOperacao");

    _pesquisaConfiguracaoNaturezaOperacao = new PesquisaConfiguracaoNaturezaOperacao();
    KoBindings(_pesquisaConfiguracaoNaturezaOperacao, "knockoutPesquisaConfiguracaoNaturezaOperacao", false, _pesquisaConfiguracaoNaturezaOperacao.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoNaturezaOperacao", _configuracaoNaturezaOperacao);

    loadConfiguracaoNaturezaOperacaoContabilizacao(function () {
        loadConfiguracaoNaturezaOperacaoEscrituracao(function () {
            new BuscarClientes(_pesquisaConfiguracaoNaturezaOperacao.Remetente);
            new BuscarClientes(_pesquisaConfiguracaoNaturezaOperacao.Destinatario);
            new BuscarClientes(_pesquisaConfiguracaoNaturezaOperacao.Tomador);
            new BuscarCategoriaPessoa(_pesquisaConfiguracaoNaturezaOperacao.CategoriaDestinatario);
            new BuscarCategoriaPessoa(_pesquisaConfiguracaoNaturezaOperacao.CategoriaRemetente);
            new BuscarCategoriaPessoa(_pesquisaConfiguracaoNaturezaOperacao.CategoriaTomador);
            new BuscarGruposPessoas(_pesquisaConfiguracaoNaturezaOperacao.GrupoDestinatario, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarGruposPessoas(_pesquisaConfiguracaoNaturezaOperacao.GrupoRemetente, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarGruposPessoas(_pesquisaConfiguracaoNaturezaOperacao.GrupoTomador, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarTiposOperacao(_pesquisaConfiguracaoNaturezaOperacao.TipoOperacao);
            new BuscarTransportadores(_pesquisaConfiguracaoNaturezaOperacao.Empresa);

            new BuscarClientes(_configuracaoNaturezaOperacao.Remetente);
            new BuscarClientes(_configuracaoNaturezaOperacao.Destinatario);
            new BuscarClientes(_configuracaoNaturezaOperacao.Tomador);
            new BuscarCategoriaPessoa(_configuracaoNaturezaOperacao.CategoriaDestinatario);
            new BuscarCategoriaPessoa(_configuracaoNaturezaOperacao.CategoriaRemetente);
            new BuscarCategoriaPessoa(_configuracaoNaturezaOperacao.CategoriaTomador);
            new BuscarGruposPessoas(_configuracaoNaturezaOperacao.GrupoDestinatario, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarGruposPessoas(_configuracaoNaturezaOperacao.GrupoRemetente, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarGruposPessoas(_configuracaoNaturezaOperacao.GrupoTomador, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarTiposOperacao(_configuracaoNaturezaOperacao.TipoOperacao);
            new BuscarRotasFrete(_configuracaoNaturezaOperacao.RotaFrete);
            new BuscarGruposProdutos(_configuracaoNaturezaOperacao.GrupoProduto);
            new BuscarTransportadores(_configuracaoNaturezaOperacao.Empresa);

            new BuscarModeloDocumentoFiscal(_configuracaoNaturezaOperacao.ModeloDocumentoFiscal)
            new BuscarNaturezasOperacoes(_configuracaoNaturezaOperacao.NaturezaDaOperacaoVenda);
            new BuscarNaturezasOperacoes(_configuracaoNaturezaOperacao.NaturezaDaOperacaoCompra);

            new BuscarAtividades(_configuracaoNaturezaOperacao.AtividadeDestinatario);
            new BuscarAtividades(_configuracaoNaturezaOperacao.AtividadeRemetente);
            new BuscarAtividades(_configuracaoNaturezaOperacao.AtividadeTomador);

            $("#" + _configuracaoNaturezaOperacao.EstadoOrigemDiferenteUFDestino.id).click(setarVisibilidadeEstados);
            $("#" + _configuracaoNaturezaOperacao.EstadoOrigemIgualUFDestino.id).click(setarVisibilidadeEstados);

            buscarConfiguracaoNaturezaOperacao();
        });
    });
}

function setarVisibilidadeEstados() {
    if (_configuracaoNaturezaOperacao.EstadoOrigemDiferenteUFDestino.val() || _configuracaoNaturezaOperacao.EstadoOrigemIgualUFDestino.val()) {
        _configuracaoNaturezaOperacao.UFOrigem.visible(false);
        _configuracaoNaturezaOperacao.UFDestino.visible(false);
        _configuracaoNaturezaOperacao.EstadoOrigemDiferente.visible(false);
        _configuracaoNaturezaOperacao.EstadoDestinoDiferente.visible(false);
    } else {
        _configuracaoNaturezaOperacao.UFOrigem.visible(true);
        _configuracaoNaturezaOperacao.UFDestino.visible(true);
        _configuracaoNaturezaOperacao.EstadoOrigemDiferente.visible(true);
        _configuracaoNaturezaOperacao.EstadoDestinoDiferente.visible(true);
    }
}

function adicionarClick(e, sender) {
    Salvar(_configuracaoNaturezaOperacao, "ConfiguracaoNaturezaOperacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridConfiguracaoNaturezaOperacao.CarregarGrid();
                limparCamposConfiguracaoNaturezaOperacao();
            } else { 
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_configuracaoNaturezaOperacao, "ConfiguracaoNaturezaOperacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridConfiguracaoNaturezaOperacao.CarregarGrid();
                limparCamposConfiguracaoNaturezaOperacao();
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
        ExcluirPorCodigo(_configuracaoNaturezaOperacao, "ConfiguracaoNaturezaOperacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridConfiguracaoNaturezaOperacao.CarregarGrid();
                limparCamposConfiguracaoNaturezaOperacao();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function copiarDadosClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja copiar os dados dessa configuração para outra configuração? lembrando que a atual configuração não será alterada após essa ação.", function () {
        _configuracaoNaturezaOperacao.Codigo.val(0);
        resetarBotoes();
    });
}

function cancelarClick(e) {
    limparCamposConfiguracaoNaturezaOperacao();
}

//*******MÉTODOS*******

function buscarConfiguracaoNaturezaOperacao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoNaturezaOperacao, tamanho: "8", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridConfiguracaoNaturezaOperacao = new GridView(_pesquisaConfiguracaoNaturezaOperacao.Pesquisar.idGrid, "ConfiguracaoNaturezaOperacao/Pesquisa", _pesquisaConfiguracaoNaturezaOperacao, menuOpcoes);
    _gridConfiguracaoNaturezaOperacao.CarregarGrid();
}

function controlarExibicaoCampo(campo, exibirCampo) {
    if (exibirCampo)
        campo.visible(true);
    else {
        campo.visible(false);
        LimparCampo(campo);
    }
}

function editarConfiguracaoNaturezaOperacao(configuracaoNaturezaOperacaoGrid) {
    limparCamposConfiguracaoNaturezaOperacao();
    limparCamposConfiguracaoNaturezaOperacaoContabilizacao();
    limparCamposConfiguracaoNaturezaOperacaoEscrituracao();
    _configuracaoNaturezaOperacao.Codigo.val(configuracaoNaturezaOperacaoGrid.Codigo);
    BuscarPorCodigo(_configuracaoNaturezaOperacao, "ConfiguracaoNaturezaOperacao/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoNaturezaOperacao.ExibirFiltros.visibleFade(false);
        _CRUDconfiguracaoNaturezaOperacao.Atualizar.visible(true);
        _CRUDconfiguracaoNaturezaOperacao.Cancelar.visible(true);
        _CRUDconfiguracaoNaturezaOperacao.Excluir.visible(true);
        _CRUDconfiguracaoNaturezaOperacao.Copiar.visible(true);
        _CRUDconfiguracaoNaturezaOperacao.Adicionar.visible(false);
        setarVisibilidadeEstados();
        recarregarGridConfiguracaoNaturezaOperacaoContabilizacao();
        recarregarGridConfiguracaoNaturezaOperacaoEscrituracao();

        if (_configuracaoNaturezaOperacao.CategoriaDestinatario.codEntity() > 0)
            _configuracaoNaturezaOperacao.TipoDestinatario.val(EnumTipoCliente.CategoriaPessoa);
        else if (_configuracaoNaturezaOperacao.GrupoDestinatario.codEntity() > 0)
            _configuracaoNaturezaOperacao.TipoDestinatario.val(EnumTipoCliente.GrupoPessoa);
        else
            _configuracaoNaturezaOperacao.TipoDestinatario.val(EnumTipoCliente.Pessoa);

        tipoDestinatarioChange(_configuracaoNaturezaOperacao);

        if (_configuracaoNaturezaOperacao.CategoriaRemetente.codEntity() > 0)
            _configuracaoNaturezaOperacao.TipoRemetente.val(EnumTipoCliente.CategoriaPessoa);
        else if (_configuracaoNaturezaOperacao.GrupoRemetente.codEntity() > 0)
            _configuracaoNaturezaOperacao.TipoRemetente.val(EnumTipoCliente.GrupoPessoa);
        else
            _configuracaoNaturezaOperacao.TipoRemetente.val(EnumTipoCliente.Pessoa);

        tipoRemetenteChange(_configuracaoNaturezaOperacao);

        if (_configuracaoNaturezaOperacao.CategoriaTomador.codEntity() > 0)
            _configuracaoNaturezaOperacao.TipoTomador.val(EnumTipoCliente.CategoriaPessoa);
        else if (_configuracaoNaturezaOperacao.GrupoTomador.codEntity() > 0)
            _configuracaoNaturezaOperacao.TipoTomador.val(EnumTipoCliente.GrupoPessoa);
        else
            _configuracaoNaturezaOperacao.TipoTomador.val(EnumTipoCliente.Pessoa);

        tipoTomadorChange(_configuracaoNaturezaOperacao);
    }, null);
}

function limparCamposConfiguracaoNaturezaOperacao() {
    _configuracaoNaturezaOperacao.TipoTomador.val(1);
    _configuracaoNaturezaOperacao.TipoDestinatario.val(1);
    _configuracaoNaturezaOperacao.TipoRemetente.val(1);
    resetarBotoes();
    _configuracaoNaturezaOperacao.UFOrigem.visible(true);
    _configuracaoNaturezaOperacao.UFDestino.visible(true);
    _configuracaoNaturezaOperacao.EstadoOrigemDiferente.visible(true);
    _configuracaoNaturezaOperacao.EstadoDestinoDiferente.visible(true);
    LimparCampos(_configuracaoNaturezaOperacao);
    limparCamposConfiguracaoNaturezaOperacaoEscrituracao();
    recarregarGridConfiguracaoNaturezaOperacaoEscrituracao();
    limparCamposConfiguracaoNaturezaOperacaoContabilizacao();
    recarregarGridConfiguracaoNaturezaOperacaoContabilizacao();
}

function resetarBotoes() {
    _CRUDconfiguracaoNaturezaOperacao.Atualizar.visible(false);
    _CRUDconfiguracaoNaturezaOperacao.Cancelar.visible(false);
    _CRUDconfiguracaoNaturezaOperacao.Excluir.visible(false);
    _CRUDconfiguracaoNaturezaOperacao.Adicionar.visible(true);
    _CRUDconfiguracaoNaturezaOperacao.Copiar.visible(false);
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