/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoAlertaCarga.js" />
/// <reference path="../../Enumeradores/EnumCargaVisualizacaoAlerta.js" />
/// <reference path="../../Enumeradores/EnumDataBaseAlertas.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDAlertaCarga;
var _gridAlertaCarga;
var _alertaCarga;
var _alertaCargaGatilho;
var _alertaCargaTratativa;

var _pesquisaAlertaCarga;

var _corPadrao = '#ED6464';

var _ListaCores = [
    { value: '#ED6464' },
    { value: '#ED8664' },
    { value: '#EDA864' },
    { value: '#EDCB64' },
    { value: '#EDED64' },
    { value: '#CBED64' },
    { value: '#A8ED64' },
    { value: '#86ED64' },
    { value: '#64ED64' },
    { value: '#64ED86' },
    { value: '#64EDA8' },
    { value: '#64EDCB' },
    { value: '#64EDED' },
    { value: '#64CBED' },
    { value: '#64A8ED' },
    { value: '#6495ED' },
    { value: '#6486ED' },
    { value: '#6464ED' },
    { value: '#8664ED' },
    { value: '#A864ED' },
    { value: '#CB64ED' },
    { value: '#ED64ED' },
    { value: '#ED64CB' },
    { value: '#ED64A8' },
    { value: '#ED6486' },
    { value: '#ED6464' },
    { value: '#8B4513' },
    { value: '#E06F1F' },
    { value: '#EDA978' },
    { value: '#000000' },
    { value: '#F9E2D2' },
    { value: '#708090' },
    { value: '#9AA6B1' },
    { value: '#C5CCD3' },
    { value: '#F1F2F4' }
];

/*
 * Declaração das Classes
 */

var CRUDAlertaCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar + " / " + Localization.Resources.Gerais.Geral.Novo });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var AlertaCarga = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557, val: ko.observable(true), options: _status, def: true });
    this.VisualizacaoAlerta = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.VisualizacaoAlerta.getFieldDescription(), val: ko.observable(EnumCargaVisualizacaoAlerta.AcompanhamentoCargas), options: EnumCargaVisualizacaoAlerta.obterOpcoes(), def: EnumCargaVisualizacaoAlerta.AcompanhamentoCargas, visible: true });
    this.TipoCargaAlerta = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TipoAlerta.getRequiredFieldDescription(), val: ko.observable(EnumTipoAlertaCarga.SemAlerta), options: ko.observable(EnumTipoAlertaCarga.obterOpcoesAcompanhamentoCarga()), def: EnumTipoAlertaCarga.SemAlerta, visible: true });
    this.Cor = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.Cor.getRequiredFieldDescription(), val: ko.observable(_corPadrao), options: _ListaCores });
    this.NaoGerarParaPreCarga = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.NaoGerarParaPreCarga, getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarAlertaAcompanhamentoCarga = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.GerarNotificacaoAcompanhamentoCarga, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.VisualizacaoAlerta.val.subscribe(function (VisualizacaoAlerta) {
        if (VisualizacaoAlerta == EnumCargaVisualizacaoAlerta.AcompanhamentoCargas) {
            _alertaCarga.GerarAlertaAcompanhamentoCarga.visible(true);
            _alertaCarga.TipoCargaAlerta.options(EnumTipoAlertaCarga.obterOpcoesAcompanhamentoCarga());
        } else {
            _alertaCarga.GerarAlertaAcompanhamentoCarga.visible(false);
            _alertaCarga.TipoCargaAlerta.options(EnumTipoAlertaCarga.obterOpcoesTorreMonitoramento());
        }
    });
    this.TipoCargaAlerta.val.subscribe(function (TipoCargaAlerta) {
        controlarExibicaoCamposGatilho(TipoCargaAlerta);
    });


}

var AlertaCargaGatilho = function () {
    this.Tempo = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoAlertaMinutos.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, required: ko.observable(true) });
    this.TempoEvento = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoEventoMinutos.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, required: ko.observable(false), visible: ko.observable(true) });


    //estes campos estao na T_ACOMPANHAMENTO_ENTREGA_TEMPO_CONFIGURACAO
    this.AlertarAdiantado = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.GerarAlerta.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TempoAcompanhamentoEntregaAdiantado = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoEntregaAdiantado.getRequiredFieldDescription(), getType: typesKnockout.time, type: types.time, required: ko.observable(false), visible: ko.observable(false) });
    this.AlertarNoHorario = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.GerarAlerta.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TempoAcompanhamentoEntregaNoHorario = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoEntregaNoPrazo.getRequiredFieldDescription(), getType: typesKnockout.time, type: types.time, required: ko.observable(false), visible: ko.observable(false) });
    this.AlertarPoucoAtrasado = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.GerarAlerta.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TempoAcompanhamentoEntregaPoucoAtrasado = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoEntregaTendenciaAtraso.getRequiredFieldDescription(), getType: typesKnockout.time, type: types.time, required: ko.observable(false), visible: ko.observable(false) });
    this.AlertarAtrasado = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.GerarAlerta.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TempoAcompanhamentoEntregaAtrasado = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoEntregaAtrasada.getRequiredFieldDescription(), getType: typesKnockout.time, type: types.time, required: ko.observable(false), visible: ko.observable(false) });

    this.AlertarAdiantadoColeta = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.GerarAlerta.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TempoAcompanhamentoColetaAdiantado = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoColetaAdiantado.getRequiredFieldDescription(), getType: typesKnockout.time, type: types.time, required: ko.observable(false), visible: ko.observable(false) });
    this.AlertarNoHorarioColeta = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.GerarAlerta.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TempoAcompanhamentoColetaNoHorario = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoColetaNoPrazo.getRequiredFieldDescription(), getType: typesKnockout.time, type: types.time, required: ko.observable(false), visible: ko.observable(false) });
    this.AlertarPoucoAtrasadoColeta = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.GerarAlerta.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TempoAcompanhamentoColetaPoucoAtrasado = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoColetaTendenciaAtraso.getRequiredFieldDescription(), getType: typesKnockout.time, type: types.time, required: ko.observable(false), visible: ko.observable(false) });
    this.AlertarAtrasadoColeta = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.GerarAlerta.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TempoAcompanhamentoColetaAtrasado = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoColetaAtrasada.getRequiredFieldDescription(), getType: typesKnockout.time, type: types.time, required: ko.observable(false), visible: ko.observable(false) });

}

var AlertaCargaTratativa = function () {
    this.EnvioEmailCliente = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.EnviarEmailClienteCarga.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnvioEmailTransportador = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.EnviarEmailTransportador.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TempoLimiteTratativaAutomatica = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TempoTratativaAutomatica.getFieldDescription(), getType: typesKnockout.timeSec, type: types.timeSec, required: ko.observable(false), visible: ko.observable(true) });
}

var PesquisaAlertaCarga = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.TipoCargaAlerta = PropertyEntity({ text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TipoCargaAlerta.getRequiredFieldDescription(), val: ko.observable(EnumTipoAlertaCarga.Todos), options: EnumTipoAlertaCarga.obterOpcoes(), def: EnumTipoAlertaCarga.Todos, visible: true });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGrid, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAlertaCarga() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "ConfiguracaoAlertaCarga/ExportarPesquisa", titulo: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.EventosMonitoramento };

    _gridAlertaCarga = new GridViewExportacao(_pesquisaAlertaCarga.Pesquisar.idGrid, "ConfiguracaoAlertaCarga/Pesquisa", _pesquisaAlertaCarga, menuOpcoes, configuracoesExportacao);
    _gridAlertaCarga.CarregarGrid();
}


function setarCor(cor) {
    $("#" + _alertaCarga.Cor.id).colorselector("setValue", cor);
}

function loadIndicadorCores() {
    $("#" + _alertaCarga.Cor.id).colorselector({
        callback: function (value) {
            _alertaCarga.Cor.val(value);
        }
    });

    setarCor(_corPadrao);
}

function loadAlertaCarga() {

    _alertaCarga = new AlertaCarga();
    KoBindings(_alertaCarga, "knockoutAlertaDados");

    HeaderAuditoria("AlertaCarga", _alertaCarga);

    _CRUDAlertaCarga = new CRUDAlertaCarga();
    KoBindings(_CRUDAlertaCarga, "knockoutCRUDAlerta");

    _pesquisaAlertaCarga = new PesquisaAlertaCarga();
    KoBindings(_pesquisaAlertaCarga, "knockoutPesquisaAlertaCarga", false, _pesquisaAlertaCarga.Pesquisar.id);

    _alertaCargaGatilho = new AlertaCargaGatilho();
    KoBindings(_alertaCargaGatilho, "knockoutAlertaCargaGatilho");

    _alertaCargaTratativa = new AlertaCargaTratativa();
    KoBindings(_alertaCargaTratativa, "knockoutAlertaCargaTratativa");

    loadGridAlertaCarga();
    loadIndicadorCores();
}


/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (validarCamposObrigatorios()) {
        executarReST("ConfiguracaoAlertaCarga/Adicionar", obterAlertaCargaSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                    recarregarGrid();
                    limparCamposAlertaCarga();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function atualizarClick() {
    if (validarCamposObrigatorios()) {
        executarReST("ConfiguracaoAlertaCarga/Atualizar", obterAlertaCargaSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);

                    recarregarGrid();
                    limparCamposAlertaCarga();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function cancelarClick() {
    limparCamposAlertaCarga();
}

function editarClick(registroSelecionado) {
    limparCamposAlertaCarga();

    executarReST("ConfiguracaoAlertaCarga/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaAlertaCarga.ExibirFiltros.visibleFade(false);

                PreencherObjetoKnout(_alertaCarga, { Data: retorno.Data.Alerta });
                PreencherObjetoKnout(_alertaCargaGatilho, { Data: retorno.Data.Gatilho });
                PreencherObjetoKnout(_alertaCargaTratativa, { Data: retorno.Data.Tratativa });

                setarCor(retorno.Data.Alerta.Cor);

                controlarBotoesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.RealmenteDesejaExcluirORegistro, function () {
        ExcluirPorCodigo(_alertaCarga, "ConfiguracaoAlertaCarga/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);

                    recarregarGrid();
                    limparCamposAlertaCarga();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */


function controlarExibicaoCamposGatilho(tipoAlertaCarga) {
    _alertaCargaGatilho.TempoEvento.visible(true);
    controlarExibicaoCampoTemposAcompanhamentosEntregaColeta(false);

    switch (tipoAlertaCarga) {
        case EnumTipoAlertaCarga.AtrasoInicioViagem:
            //controlarExibicaoCampoTemposAcompanhamentosEntrega(true);
            break;
    }
}

function controlarExibicaoCampoTemposAcompanhamentosEntregaColeta(isExibirCampo) {


    _alertaCargaGatilho.TempoAcompanhamentoEntregaAdiantado.visible(isExibirCampo);
    _alertaCargaGatilho.TempoAcompanhamentoEntregaAdiantado.required(isExibirCampo);

    _alertaCargaGatilho.TempoAcompanhamentoEntregaNoHorario.visible(isExibirCampo);
    _alertaCargaGatilho.TempoAcompanhamentoEntregaNoHorario.required(isExibirCampo);

    _alertaCargaGatilho.TempoAcompanhamentoEntregaPoucoAtrasado.visible(isExibirCampo);
    _alertaCargaGatilho.TempoAcompanhamentoEntregaPoucoAtrasado.required(isExibirCampo);

    _alertaCargaGatilho.TempoAcompanhamentoEntregaAtrasado.visible(isExibirCampo);
    _alertaCargaGatilho.TempoAcompanhamentoEntregaAtrasado.required(isExibirCampo);

    _alertaCargaGatilho.TempoAcompanhamentoColetaAdiantado.visible(isExibirCampo);
    _alertaCargaGatilho.TempoAcompanhamentoColetaAdiantado.required(isExibirCampo);

    _alertaCargaGatilho.TempoAcompanhamentoColetaNoHorario.visible(isExibirCampo);
    _alertaCargaGatilho.TempoAcompanhamentoColetaNoHorario.required(isExibirCampo);

    _alertaCargaGatilho.TempoAcompanhamentoColetaPoucoAtrasado.visible(isExibirCampo);
    _alertaCargaGatilho.TempoAcompanhamentoColetaPoucoAtrasado.required(isExibirCampo);

    _alertaCargaGatilho.TempoAcompanhamentoColetaAtrasado.visible(isExibirCampo);
    _alertaCargaGatilho.TempoAcompanhamentoColetaAtrasado.required(isExibirCampo);

}


function controlarBotoesHabilitados() {
    var isEdicao = _alertaCarga.Codigo.val() > 0;

    _CRUDAlertaCarga.Atualizar.visible(isEdicao);
    _CRUDAlertaCarga.Excluir.visible(isEdicao);
    _CRUDAlertaCarga.Adicionar.visible(!isEdicao);
}

function controlarExibicaoAbaTratativa(tipoAlerta) {
    if (tipoAlerta === EnumTipoAlerta.SemAlerta)
        $("#abaMonitoramentoEventoTratativa").hide();
    else
        $("#abaMonitoramentoEventoTratativa").show();
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
}

function limparCamposAlertaCarga() {
    LimparCampos(_alertaCarga);
    LimparCampos(_alertaCargaGatilho);
    LimparCampos(_alertaCargaTratativa);
    controlarBotoesHabilitados();

    setarCor(_corPadrao);

    $("#tabMonitoramentoEventoDados").click();
}

function obterAlertaCargaSalvar() {
    var alertaCarga = RetornarObjetoPesquisa(_alertaCarga);


    alertaCarga["Gatilho"] = obterDadosGatilhoSalvar();
    alertaCarga["Tratativa"] = obterDadosTratativaSalvar();


    return alertaCarga;
}

function recarregarGrid() {
    _gridAlertaCarga.CarregarGrid();
}

function validarCamposObrigatorios() {
    var camposInformados = ValidarCamposObrigatorios(_alertaCarga);

    return camposInformados;
}

function obterDadosGatilhoSalvar() {
    return JSON.stringify(RetornarObjetoPesquisa(_alertaCargaGatilho));
}

function obterDadosTratativaSalvar() {
    return JSON.stringify(RetornarObjetoPesquisa(_alertaCargaTratativa));
}