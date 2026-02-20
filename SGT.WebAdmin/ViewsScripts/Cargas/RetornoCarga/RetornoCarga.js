/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoRetornoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridRetornoCarga;
var _retornoCarga;
var _pesquisaRetornoCarga;

/*
 * Declaração das Classes
 */

var PesquisaRetornoCarga = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.RetornoCarga.NumeroCarga.getFieldDescription() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.RetornoCarga.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.RetornoCarga.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoRetornoCarga.AgInformarRetorno), options: EnumSituacaoRetornoCarga.obterOpcoesPesquisa(), def: EnumSituacaoRetornoCarga.AgInformarRetorno, getType: typesKnockout.int, text: Localization.Resources.Cargas.RetornoCarga.Situacao.getFieldDescription() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.RetornoCarga.Veiculo.getFieldDescription(), issue: 143, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.RetornoCarga.Motorista.getFieldDescription(), issue: 145, idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.RetornoCarga.Origem.getFieldDescription(), idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.RetornoCarga.Destino.getFieldDescription(), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.RetornoCarga.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.RetornoCarga.TipoOperacao.getFieldDescription(), issue: 121, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.RetornoCarga.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            recarregarRetornoCarga();
        }, type: types.event, text: Localization.Resources.Cargas.RetornoCarga.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: Localization.Resources.Cargas.RetornoCarga.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            }
            else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Cargas.RetornoCarga.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var RetornoCarga = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoRetornoCarga.AgInformarRetorno), options: EnumSituacaoRetornoCarga.obterOpcoes(), def: EnumSituacaoRetornoCarga.AgInformarRetorno, getType: typesKnockout.int, text: Localization.Resources.Cargas.RetornoCarga.Situacao });
    this.ClienteColeta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.RetornoCarga.ClienteParaColeta.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), required: false });
    this.TipoRetornoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Cargas.RetornoCarga.TipoCargaRetorno.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), text: Localization.Resources.Cargas.RetornoCarga.Transportador, visible: ko.observable(true) });
    this.PrevisaoChegada = PropertyEntity({ text: Localization.Resources.Cargas.RetornoCarga.PrevisaoChegada, val: ko.observable(0), getType: typesKnockout.date, visible: ko.observable(true) });

    this.ExigeClienteColeta = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DescricaoSituacao = PropertyEntity({ text: Localization.Resources.Cargas.RetornoCarga.SituacaoRetorno.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ text: Localization.Resources.Cargas.RetornoCarga.NumeroCarga.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: Localization.Resources.Cargas.RetornoCarga.Filial.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Origem = PropertyEntity({ text: Localization.Resources.Cargas.RetornoCarga.Origem.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Destino = PropertyEntity({ text: Localization.Resources.Cargas.RetornoCarga.Destino.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Cargas.RetornoCarga.Remetente.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Destinatarios = PropertyEntity({ text: Localization.Resources.Cargas.RetornoCarga.Destinatario.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), text: Localization.Resources.Cargas.RetornoCarga.DestinatarioFinal.getFieldDescription(), visible: ko.observable(true) });
    this.NumeroReboques = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.RetornarSomenteComTracao = PropertyEntity({ val: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.RetornoCarga.RetornarSomenteComTracao, def: false });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), visible: ko.observable(false), required: false, text: ko.observable(Localization.Resources.Cargas.RetornoCarga.Reboque.getRequiredFieldDescription()), idBtnSearch: guid() });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), visible: ko.observable(false), required: false, text: ko.observable(Localization.Resources.Cargas.RetornoCarga.SegundoReboque.getRequiredFieldDescription()), idBtnSearch: guid() });
    this.PontosDeColeta = PropertyEntity({ val: ko.observable(0), def: 0, text: Localization.Resources.Cargas.RetornoCarga.PontosColeta.getFieldDescription(), options: ko.observableArray([]), enable: ko.observable(true) });
    
    this.ExigeClienteColeta.val.subscribe(exigeClienteSubscribe);
    this.Situacao.val.subscribe(situacaoSubscribe);
    this.TipoRetornoCarga.val.subscribe(tipoRetornoCargaSubscribe);
    this.NumeroReboques.val.subscribe(numeroReboquesSubscribe);
    this.RetornarSomenteComTracao.val.subscribe(retornarSomenteComTracaoSubscribe);

    this.GerarRetorno = PropertyEntity({ eventClick: gerarRetornoClick, type: types.event, text: Localization.Resources.Cargas.RetornoCarga.GerarCargaRetorno, visible: ko.observable(true) });
    this.CancelarRetorno = PropertyEntity({ eventClick: cancelarRetornoClick, type: types.event, text: Localization.Resources.Cargas.RetornoCarga.CancelarCargaRetorno, visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridRetornoCargas() {
    var editar = { descricao: Localization.Resources.Cargas.RetornoCarga.Detalhes, id: "clasEditar", evento: "onclick", metodo: detalheRetornoCargaClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };
    var configuracoesExportacao = { url: "RetornoCarga/ExportarPesquisa", titulo: Localization.Resources.Cargas.RetornoCarga.RetornoDeCarga };

    _gridRetornoCarga = new GridViewExportacao(_pesquisaRetornoCarga.Pesquisar.idGrid, "RetornoCarga/Pesquisa", _pesquisaRetornoCarga, menuOpcoes, configuracoesExportacao);
    _gridRetornoCarga.CarregarGrid();

    
}

function loadRetornoCarga() {
    _pesquisaRetornoCarga = new PesquisaRetornoCarga();
    KoBindings(_pesquisaRetornoCarga, "knockoutPesquisaRetornoCarga", false, _pesquisaRetornoCarga.Pesquisar.id);

    _retornoCarga = new RetornoCarga();
    KoBindings(_retornoCarga, "knoutRetornoCarga");

    new BuscarClientes(_retornoCarga.ClienteColeta);
    new BuscarClientes(_pesquisaRetornoCarga.Remetente);
    new BuscarClientes(_pesquisaRetornoCarga.Destinatario);
    new BuscarEmpresa(_pesquisaRetornoCarga.Empresa);
    new BuscarFilial(_pesquisaRetornoCarga.Filial);
    new BuscarLocalidades(_pesquisaRetornoCarga.Origem);
    new BuscarLocalidades(_pesquisaRetornoCarga.Destino);
    new BuscarMotoristas(_pesquisaRetornoCarga.Motorista, null, _pesquisaRetornoCarga.Empresa);
    new BuscarTiposOperacao(_pesquisaRetornoCarga.TipoOperacao);
    new BuscarTipoRetornoCarga(_retornoCarga.TipoRetornoCarga, callbackTipoRetornoCarga);
    new BuscarVeiculos(_pesquisaRetornoCarga.Veiculo, null, _pesquisaRetornoCarga.Empresa);
    new BuscarVeiculos(_retornoCarga.Reboque, null, _retornoCarga.Empresa, null, null, true, null, null, true, null, null, '1', null, null, null, null, _retornoCarga.Destinatario);
    new BuscarVeiculos(_retornoCarga.SegundoReboque, null, _retornoCarga.Empresa, null, null, true, null, null, true, null, null, '1', null, null, null, null, _retornoCarga.Destinatario);

    loadGridRetornoCargas();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaRetornoCarga.Filial.visible(false);
        _retornoCarga.Filial.visible(false);
    }
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function cancelarRetornoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar o retorno desta carga?", function () {
        executarReST("RetornoCarga/CancelarRetorno", { Codigo: _retornoCarga.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalDetalhesRetornoCargaColetaBackhaul();
                    recarregarRetornoCarga();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function detalheRetornoCargaClick(registroSelecionado) {
    _retornoCarga.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_retornoCarga, "RetornoCarga/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                controlarExibicaoCampoRetornarSomenteComTracao();
                exibirModalDetalhesRetornoCargaColetaBackhaul();
                preencherDestinatarios(retorno.Data.TodosDestinatarios);
                _retornoCarga.PontosDeColeta.val(retorno.Data.PontosDeColeta);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exigeClienteSubscribe(exigeCliente) {
    _retornoCarga.ClienteColeta.visible(exigeCliente);
    _retornoCarga.ClienteColeta.required = exigeCliente;

    controlarExibicaoCampoRetornarSomenteComTracao();
};

function gerarRetornoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja gerar a carga de retorno?", function () {
        Salvar(_retornoCarga, "RetornoCarga/GerarCargaRetorno", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalDetalhesRetornoCargaColetaBackhaul();
                    recarregarRetornoCarga();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function numeroReboquesSubscribe(val) {
    if (val == 0) {
        _retornoCarga.RetornarSomenteComTracao.visible(false);
        _retornoCarga.Reboque.visible(false);
        _retornoCarga.SegundoReboque.visible(false);
        _retornoCarga.SegundoReboque.required = false;
        _retornoCarga.Reboque.required = false;
    }
    else if (val == 1) {
        _retornoCarga.RetornarSomenteComTracao.visible(_retornoCarga.ExigeClienteColeta.val());
        _retornoCarga.Reboque.visible(true);
        _retornoCarga.SegundoReboque.visible(false);
        _retornoCarga.SegundoReboque.required = false;
        _retornoCarga.Reboque.required = !_retornoCarga.RetornarSomenteComTracao.val();
    }
    else {
        _retornoCarga.RetornarSomenteComTracao.visible(_retornoCarga.ExigeClienteColeta.val());
        _retornoCarga.Reboque.visible(true);
        _retornoCarga.SegundoReboque.visible(true);
        _retornoCarga.SegundoReboque.required = !_retornoCarga.RetornarSomenteComTracao.val();
        _retornoCarga.Reboque.required = !_retornoCarga.RetornarSomenteComTracao.val();
    }
}

function retornarSomenteComTracaoSubscribe(val) {
    if (val) {
        _retornoCarga.Reboque.visible(false);
        _retornoCarga.SegundoReboque.visible(false);
        _retornoCarga.SegundoReboque.required = false;
        _retornoCarga.Reboque.required = false;
    }
    else {
        if (_retornoCarga.NumeroReboques.val() > 0) {
            _retornoCarga.Reboque.required = true;
            _retornoCarga.Reboque.visible(true);
        }

        if (_retornoCarga.NumeroReboques.val() > 1) {
            _retornoCarga.SegundoReboque.required = true;
            _retornoCarga.SegundoReboque.visible(true);
        }
    }
}

function situacaoSubscribe(val) {
    if (val == EnumSituacaoRetornoCarga.AgInformarRetorno) {
        _retornoCarga.GerarRetorno.visible(true);
        _retornoCarga.CancelarRetorno.visible(true);
        _retornoCarga.ClienteColeta.enable(true);
        _retornoCarga.TipoRetornoCarga.enable(true);
        _retornoCarga.Reboque.enable(true);
        _retornoCarga.SegundoReboque.enable(true);
        _retornoCarga.PontosDeColeta.enable(true);
    }
    else {
        _retornoCarga.GerarRetorno.visible(false);
        _retornoCarga.CancelarRetorno.visible(false);
        _retornoCarga.ClienteColeta.enable(false);
        _retornoCarga.TipoRetornoCarga.enable(false);
        _retornoCarga.Reboque.enable(false);
        _retornoCarga.SegundoReboque.enable(false);
        _retornoCarga.PontosDeColeta.enable(false);
    }
}

function tipoRetornoCargaSubscribe(val) {
    if (val === "")
        _retornoCarga.ClienteColeta.visible(false);
}

/*
 * Declaração das Funções Privadas
 */

function callbackTipoRetornoCarga(data) {
    _retornoCarga.TipoRetornoCarga.codEntity(data.Codigo);
    _retornoCarga.TipoRetornoCarga.val(data.Descricao);
    _retornoCarga.ExigeClienteColeta.val(data.ExigeClienteColeta);
}

function controlarExibicaoCampoRetornarSomenteComTracao() {
    if (_retornoCarga.ExigeClienteColeta.val()) {
        _retornoCarga.RetornarSomenteComTracao.val(false);
        _retornoCarga.RetornarSomenteComTracao.visible(false);
    }
    else
        _retornoCarga.RetornarSomenteComTracao.visible(true);
}

function exibirModalDetalhesRetornoCargaColetaBackhaul() {
    Global.abrirModal('divModalDetalhesRetorno');
    $("#divModalDetalhesRetorno").one('hidden.bs.modal', function () {
        LimparCampos(_retornoCarga);
    });
}

function fecharModalDetalhesRetornoCargaColetaBackhaul() {
    Global.fecharModal('divModalDetalhesRetorno');
}

function recarregarRetornoCarga() {
    _gridRetornoCarga.CarregarGrid();
}

function preencherDestinatarios(destinatarios) {
    var listaDestinatarios = [
        {
            text: Localization.Resources.Cargas.RetornoCarga.UltimoPonto,
            value: 0
        }
    ];

    for (var i = 0; i < destinatarios.length; i++) {
        listaDestinatarios.push({
            text: destinatarios[i].Descricao,
            value: destinatarios[i].Codigo
        });
    }
    
    _retornoCarga.PontosDeColeta.options(listaDestinatarios);
}