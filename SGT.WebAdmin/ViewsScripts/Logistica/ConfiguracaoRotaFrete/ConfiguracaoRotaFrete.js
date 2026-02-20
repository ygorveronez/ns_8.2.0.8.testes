/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="Empresa.js" />
/// <reference path="Estado.js" />
/// <reference path="ModeloVeicularCarga.js" />
/// <reference path="TipoCarga.js" />
/// <reference path="TipoOperacao.js" />
/// <reference path="GrupoTransportadoresHUBOfertas.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _configuracaoRotaFrete;
var _crudConfiguracaoRotaFrete;
var _gridConfiguracaoRotaFrete;
var _pesquisaConfiguracaoRotaFrete;

/*
 * Declaração das Classes\
 */

var ConfiguracaoRotaFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação:", issue: 557 });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 200 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.HoraEnvioTransportadorRota = PropertyEntity({ getType: typesKnockout.time, val: ko.observable(""), def: "", text: "Hora de Envio:", visible: ko.observable(true) });
    this.DiasAntecedenciaEnvioTransportadorRota = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: "Dias de Antecedência:", visible: ko.observable(true) });
    this.EnviarTransportadorRotaSegunda = PropertyEntity({ getType: typesKnockout.bool, text: "Segunda-Feira", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaTerca = PropertyEntity({ getType: typesKnockout.bool, text: "Terça-Feira", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaQuarta = PropertyEntity({ getType: typesKnockout.bool, text: "Quarta-Feira", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaQuinta = PropertyEntity({ getType: typesKnockout.bool, text: "Quinta-Feira", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaSexta = PropertyEntity({ getType: typesKnockout.bool, text: "Sexta-Feira", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaSabado = PropertyEntity({ getType: typesKnockout.bool, text: "Sábado", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaDomingo = PropertyEntity({ getType: typesKnockout.bool, text: "Domingo", val: ko.observable(false), def: false });
    this.GrupoTransportadoresHUBOfertas = PropertyEntity({ visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoHUBOfertas) });
};

var CRUDConfiguracaoRotaFrete = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoRotaFreteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarConfiguracaoRotaFreteClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarConfiguracaoRotaFreteClick, type: types.event, text: "Cancelar" });
    this.Excluir = PropertyEntity({ eventClick: excluirConfiguracaoRotaFreteClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var PesquisaConfiguracaoRotaFrete = function () {
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação:" });
    this.Descricao = PropertyEntity({ text: "Descrição:", maxlength: 200 });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            recarregarGridConfiguracaoRotaFrete();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadConfiguracaoRotaFrete() {
    _configuracaoRotaFrete = new ConfiguracaoRotaFrete();
    KoBindings(_configuracaoRotaFrete, "knockoutDetalhes");

    HeaderAuditoria("ConfiguracaoRotaFrete", _configuracaoRotaFrete);

    _crudConfiguracaoRotaFrete = new CRUDConfiguracaoRotaFrete();
    KoBindings(_crudConfiguracaoRotaFrete, "knockoutCRUDConfiguracaoRotaFrete");

    _pesquisaConfiguracaoRotaFrete = new PesquisaConfiguracaoRotaFrete();
    KoBindings(_pesquisaConfiguracaoRotaFrete, "knockoutPesquisaConfiguracaoRotaFrete", false, _pesquisaConfiguracaoRotaFrete.Pesquisar.id);

    new BuscarFilial(_configuracaoRotaFrete.Filial);

    if (!_CONFIGURACAO_TMS.PossuiIntegracaoHUBOfertas) {
        $("#liTabGrupoTransportadoresHUBOfertas").hide();
        $("#tabGrupoTransportadoresHUBOfertas").hide();
    }

    loadEmpresa();
    loadEstado();
    loadLocalidadeOrigem();
    loadLocalidadeDestino();
    loadModeloVeicularCarga();
    loadTipoCarga();
    loadTipoOperacao();

    if (_CONFIGURACAO_TMS.PossuiIntegracaoHUBOfertas)
        loadGrupoTransportadoresHUBOfertas();
    loadGridConfiguracaoRotaFretes();
}

function loadGridConfiguracaoRotaFretes() {
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarConfiguracaoRotaFreteClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };

    _gridConfiguracaoRotaFrete = new GridView(_pesquisaConfiguracaoRotaFrete.Pesquisar.idGrid, "ConfiguracaoRotaFrete/Pesquisa", _pesquisaConfiguracaoRotaFrete, menuOpcoes);
    _gridConfiguracaoRotaFrete.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarConfiguracaoRotaFreteClick() {
    if (!ValidarCamposObrigatorios(_configuracaoRotaFrete)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }
    
    executarReST("ConfiguracaoRotaFrete/Adicionar", obterConfiguracaoRotaFreteSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso!");
                recarregarGridConfiguracaoRotaFrete();
                limparCamposConfiguracaoRotaFrete();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarConfiguracaoRotaFreteClick() {
    if (!ValidarCamposObrigatorios(_configuracaoRotaFrete)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    executarReST("ConfiguracaoRotaFrete/Atualizar", obterConfiguracaoRotaFreteSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso!");
                recarregarGridConfiguracaoRotaFrete();
                limparCamposConfiguracaoRotaFrete();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarConfiguracaoRotaFreteClick() {
    limparCamposConfiguracaoRotaFrete();
    controlarBotoesConfiguracaoRotaFreteHabilitados();
}

function editarConfiguracaoRotaFreteClick(registroSelecionado) {
    limparCamposConfiguracaoRotaFrete();

    executarReST("ConfiguracaoRotaFrete/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaConfiguracaoRotaFrete.ExibirFiltros.visibleFade(false);
                PreencherObjetoKnout(_configuracaoRotaFrete, { Data: retorno.Data.Detalhes });
                controlarBotoesConfiguracaoRotaFreteHabilitados();
                preencherEmpresa(retorno.Data.Empresas);
                preencherLocalidadeOrigem(retorno.Data.LocalidadesOrigem);
                preencherLocalidadeDestino(retorno.Data.LocalidadesDestino);
                preencherEstado(retorno.Data.Estados);
                preencherTipoOperacao(retorno.Data.TiposOperacao);
                preencherModeloVeicularCarga(retorno.Data.ModelosVeicularesCarga);
                preencherTipoCarga(retorno.Data.TiposCarga);

                if (_CONFIGURACAO_TMS.PossuiIntegracaoHUBOfertas)
                    preencherGrupoTransportadoresHUBOfertas(retorno.Data.GrupoTransportadoresHUBOfertas, retorno.Data.Detalhes);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirConfiguracaoRotaFreteClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir a configuração de rota de frete?", function () {
        executarReST("ConfiguracaoRotaFrete/ExcluirporCodigo", { Codigo: _configuracaoRotaFrete.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");
                    recarregarGridConfiguracaoRotaFrete();
                    limparCamposConfiguracaoRotaFrete();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções Privadas
 */

function controlarBotoesConfiguracaoRotaFreteHabilitados() {
    var isEdicao = _configuracaoRotaFrete.Codigo.val() > 0;

    _crudConfiguracaoRotaFrete.Adicionar.visible(!isEdicao);
    _crudConfiguracaoRotaFrete.Atualizar.visible(isEdicao);
    _crudConfiguracaoRotaFrete.Excluir.visible(isEdicao);
}

function limparCamposConfiguracaoRotaFrete() {
    LimparCampos(_configuracaoRotaFrete);
    limparCamposEmpresa();
    limparCamposLocalidadeOrigem();
    limparCamposLocalidadeDestino();
    limparCamposEstado();
    limparCamposTipoOperacao();
    limparCamposModeloVeicularCarga();
    limparCamposTipoCarga();
    if (_CONFIGURACAO_TMS.PossuiIntegracaoHUBOfertas)
        limparCamposGrupoTransportadoresHUBOfertas();
    controlarBotoesConfiguracaoRotaFreteHabilitados();
    resetarTabs();
}

function obterConfiguracaoRotaFreteSalvar() {
    var configuracaoRotaFrete = RetornarObjetoPesquisa(_configuracaoRotaFrete);

    preencherEmpresaSalvar(configuracaoRotaFrete);
    preencherLocalidadeOrigemSalvar(configuracaoRotaFrete);
    preencherLocalidadeDestinoSalvar(configuracaoRotaFrete);
    preencherEstadoSalvar(configuracaoRotaFrete);
    preencherModeloVeicularCargaSalvar(configuracaoRotaFrete);
    preencherTipoCargaSalvar(configuracaoRotaFrete);
    preencherTipoOperacaoSalvar(configuracaoRotaFrete);
    if (_CONFIGURACAO_TMS.PossuiIntegracaoHUBOfertas)
        preencherGrupoTransportadoresHUBOfertasSalvar(configuracaoRotaFrete);

    return configuracaoRotaFrete;
}

function recarregarGridConfiguracaoRotaFrete() {
    _gridConfiguracaoRotaFrete.CarregarGrid()
}

function resetarTabs() {
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}
