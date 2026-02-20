/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoAlerta.js" />
/// <reference path="../../Enumeradores/EnumTipoMonitoramentoEvento.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="MonitoramentoEventoGatilho.js" />
/// <reference path="MonitoramentoEventoHorario.js" />
/// <reference path="MonitoramentoEventoTratativa.js" />
/// <reference path="MonitoramentoEventoCausa.js" />
/// <reference path="MonitoramentoEventoAcaoTratativa.js" />
/// <reference path="MonitoramentoEventoTratativaAutomatica.js" />
/// <reference path="MonitoramentoEventoStatusViagem.js" />
/// <reference path="MonitoramentoEventoTipoDeCarga.js" />
/// <reference path="MonitoramentoEventoTipoDeOperacao.js" />


/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMonitoramentoEvento;
var _gridMonitoramentoEvento;
var _monitoramentoEvento;
var _pesquisaMonitoramentoEvento;
var _monitoramentoEventoOpcoesTipoIntegracao;
var _estaEditando = false;

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

var CRUDMonitoramentoEvento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Limpar" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MonitoramentoEvento = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.TipoAlerta = PropertyEntity({ text: "*Tipo Alerta: ", val: ko.observable(EnumTipoAlerta.SemAlerta), options: EnumTipoAlerta.obterOpcoes(), def: EnumTipoAlerta.SemAlerta, visible: false });
    this.Icone = PropertyEntity({ text: "Icone: ", visible: true });
    this.DescricaoAlerta = PropertyEntity({ text: ko.observable(""), visible: ko.observable(false) });
    this.TipoMonitoramentoEvento = PropertyEntity({ text: "*Tipo: ", val: ko.observable(""), options: EnumTipoMonitoramentoEvento.obterOpcoes(), def: "", required: true });
    this.Prioridade = PropertyEntity({ text: "*Prioridade:", maxlength: 2, required: true, getType: typesKnockout.int, val: ko.observable(1), def: 1 });
    this.Cor = PropertyEntity({ text: "*Cor: ", val: ko.observable(_corPadrao), options: _ListaCores });
    this.ExibirControleEntrega = PropertyEntity({ text: "Exibir na tela de controle de entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirApp = PropertyEntity({ text: "Exibir no aplicativo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarParaPreCarga = PropertyEntity({ text: "Não gerar para Pré Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarAlertaAcompanhamentoCarga = PropertyEntity({ text: "Gerar notificação no acompanhamento de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirDescricaoAlerta = PropertyEntity({ text: "Exibir descrição do alerta", getType: typesKnockout.bool, val: ko.observable(false), enable: ko.observable(this.GerarAlertaAcompanhamentoCarga.val()) });
    this.ExibirDataeHoraGeracaoAlerta = PropertyEntity({ text: "Exibir Data e Hora da geração do alerta ao passar o mouse", getType: typesKnockout.bool, val: ko.observable(false), enable: ko.observable(this.GerarAlertaAcompanhamentoCarga.val()) });
    this.ConsiderarParaSemaforo = PropertyEntity({ text: "Considerar para Semáforo", getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.GerarAtendimento = PropertyEntity({ text: "Gerar Atendimento", getType: typesKnockout.bool, val: ko.observable(true), def: false });
    this.MotivoChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo Atendimento", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(this.GerarAtendimento.val()) });
    this.IntegrarEvento = PropertyEntity({ text: "Integrar Evento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TipoIntegracao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.Integracao, getType: typesKnockout.selectMultiple, val: ko.observable([]), options: ko.observable(_monitoramentoEventoOpcoesTipoIntegracao), def: [], required: ko.observable(false), visible: ko.observable(this.IntegrarEvento.val()) });

    this.QuandoProcessar = PropertyEntity({ text: "Quando processar eventos: ", val: ko.observable(EnumQuandoProcessarMonitoramento.AoIniciarMonitoramento), options: EnumQuandoProcessarMonitoramento.obterOpcoes(), visible: true });
    this.RestringirHorario = PropertyEntity({});
    this.VerificarStatusViagem = PropertyEntity({});
    this.VerificarTipoDeCarga = PropertyEntity({});
    this.TratativaAutomatica = PropertyEntity({});

    this.GerarAtendimento.val.subscribe((val) => {
        _monitoramentoEvento.MotivoChamado.required(val);
        _monitoramentoEvento.MotivoChamado.enable(val);
    });

    this.IntegrarEvento.val.subscribe((val) => _monitoramentoEvento.TipoIntegracao.visible(val))

    this.GerarAlertaAcompanhamentoCarga.val.subscribe((val) => {
        _monitoramentoEvento.ExibirDescricaoAlerta.enable(val);
        _monitoramentoEvento.ExibirDataeHoraGeracaoAlerta.enable(val)
    })

    this.TipoAlerta.val.subscribe(controlarExibicaoAbaTratativa);

    this.TipoMonitoramentoEvento.val.subscribe(function (tipoMonitoramentoEvento) {
        self.TipoAlerta.val(EnumTipoMonitoramentoEvento.obterTipoAlerta(tipoMonitoramentoEvento));

        controlarExibicaoCamposMonitoramentoEventoGatilho(tipoMonitoramentoEvento);
    });
}

var PesquisaMonitoramentoEvento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.TipoMonitoramentoEvento = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoMonitoramentoEvento.Todos), options: EnumTipoMonitoramentoEvento.obterOpcoesPesquisa(), def: EnumTipoMonitoramentoEvento.Todos });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMonitoramentoEvento, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMonitoramentoEvento() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MonitoramentoEvento/ExportarPesquisa", titulo: "Eventos de Monitoramento" };

    _gridMonitoramentoEvento = new GridViewExportacao(_pesquisaMonitoramentoEvento.Pesquisar.idGrid, "MonitoramentoEvento/Pesquisa", _pesquisaMonitoramentoEvento, menuOpcoes, configuracoesExportacao);
    _gridMonitoramentoEvento.CarregarGrid();
}


function setarCor(cor) {
    $("#" + _monitoramentoEvento.Cor.id).colorselector("setValue", cor);

}

function loadIndicadorCores() {
    $("#" + _monitoramentoEvento.Cor.id).colorselector({
        callback: function (value) {
            _monitoramentoEvento.Cor.val(value);
        }
    });

    setarCor(_corPadrao);
}

function loadMonitoramentoEvento() {
    ObterTiposIntegracaoMonitoramentoEvento().then(
        function () {
            _monitoramentoEvento = new MonitoramentoEvento();
            KoBindings(_monitoramentoEvento, "knockoutMonitoramentoEvento");

            HeaderAuditoria("MonitoramentoEvento", _monitoramentoEvento);

            _CRUDMonitoramentoEvento = new CRUDMonitoramentoEvento();
            KoBindings(_CRUDMonitoramentoEvento, "knockoutCRUDMonitoramentoEvento");

            _pesquisaMonitoramentoEvento = new PesquisaMonitoramentoEvento();
            KoBindings(_pesquisaMonitoramentoEvento, "knockoutPesquisaMonitoramentoEvento", false, _pesquisaMonitoramentoEvento.Pesquisar.id);

            loadMonitoramentoEventoGatilho();
            loadMonitoramentoEventoHorario();
            loadMonitoramentoEventoStatusViagem();
            loadMonitoramentoEventoTipoDeCarga();
            loadMonitoramentoEventoTipoDeOperacao();
            loadMonitoramentoEventoTratativa();
            loadMonitoramentoEventoTratativaAutomatica();
            loadMonitoramentoEventoCausa();
            loadMonitoramentoEventoAcaoTratativa();
            loadGridMonitoramentoEvento();

            loadIndicadorCores();

            new BuscarMotivoChamado(_monitoramentoEvento.MotivoChamado);
        }
    );
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (_monitoramentoEventoGatilho.EventoContinuo.val()) {
        exibirConfirmacao("Atenção", "Evento contínuo esta marcado, isso significa que este evento pode ser gerado mais de uma vez até a finalização do monitoramento, deseja continuar?", function () {
            adicionarEvento();
        });
    } else {
        adicionarEvento();
    }
}


function adicionarEvento() {
    if (validarCamposObrigatoriosMonitoramentoEvento()) {
        executarReST("MonitoramentoEvento/Adicionar", obterMonitoramentoEventoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                    recarregarGridMonitoramentoEvento();
                    limparCamposMonitoramentoEvento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function atualizarClick() {
    if (_monitoramentoEventoGatilho.EventoContinuo.val()) {
        exibirConfirmacao("Atenção", "Evento contínuo esta marcado, isso significa que este evento pode ser gerado mais de uma vez até a finalização do monitoramento, deseja continuar?", function () {
            atualizarEvento();
        });
    } else {
        atualizarEvento();
    }
}


function atualizarEvento() {
    if (validarCamposObrigatoriosMonitoramentoEvento()) {
        executarReST("MonitoramentoEvento/Atualizar", obterMonitoramentoEventoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                    recarregarGridMonitoramentoEvento();
                    limparCamposMonitoramentoEvento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function cancelarClick() {
    limparCamposMonitoramentoEvento();
}

function editarClick(registroSelecionado) {
    _estaEditando = true;
    limparCamposMonitoramentoEvento();

    executarReST("MonitoramentoEvento/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMonitoramentoEvento.ExibirFiltros.visibleFade(false);

                PreencherObjetoKnout(_monitoramentoEvento, { Data: retorno.Data.Evento });
                preencherMonitoramentoEventoGatilho(retorno.Data.Gatilho);
                preencherMonitoramentoEventoHorario(retorno.Data.Horario);
                preencherMonitoramentoEventoStatusViagem(retorno.Data.StatusViagem);
                preencherMonitoramentoEventoTipoDeCarga(retorno.Data.TipoDeCarga);
                preencherMonitoramentoEventoTipoDeOperacao(retorno.Data.TipoDeOperacao);
                preencherMonitoramentoEventoTratativa(retorno.Data.Tratativas);
                preencherMonitoramentoEventoTratativaAutomatica(retorno.Data.TratativaAutomatica);
                preencherMonitoramentoEventoCausa(retorno.Data.Causa);
                preencherMonitoramentoEventoAcaoTratativa(retorno.Data.AcaoTratativa);

                setarCor(retorno.Data.Evento.Cor);

                controlarBotoesHabilitados();
                preencherIconeEDescricao(retorno.Data.Evento.TipoMonitoramentoEvento);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_monitoramentoEvento, "MonitoramentoEvento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMonitoramentoEvento();
                    limparCamposMonitoramentoEvento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _monitoramentoEvento.Codigo.val() > 0;

    _CRUDMonitoramentoEvento.Atualizar.visible(isEdicao);
    _CRUDMonitoramentoEvento.Excluir.visible(isEdicao);
    _CRUDMonitoramentoEvento.Adicionar.visible(!isEdicao);

}

function controlarExibicaoAbaTratativa(tipoAlerta) {
    if (tipoAlerta === EnumTipoAlerta.SemAlerta)
        $("#abaMonitoramentoEventoTratativa").hide();
    else
        $("#abaMonitoramentoEventoTratativa").show();

    if (!_estaEditando)
        ObterCausasEAcoesPorTipoAlerta(tipoAlerta);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function limparCamposMonitoramentoEvento() {
    LimparCampos(_monitoramentoEvento);
    limparCamposMonitoramentoEventoGatilho();
    limparCamposMonitoramentoEventoHorario();
    limparCamposMonitoramentoEventoStatusViagem();
    limparCamposMonitoramentoEventoTipoDeCarga();
    limparCamposMonitoramentoEventoTratativa();
    limparCamposMonitoramentoEventoTratativaAutomatica();
    limparCamposMonitoramentoEventoCausa();
    limparCamposMonitoramentoEventoAcaoTratativa();
    controlarBotoesHabilitados();

    setarCor(_corPadrao);

    let triggerEl = document.querySelector('a[href="#tabMonitoramentoEventoDados"]');
    let firstTab = new bootstrap.Tab(triggerEl);
    firstTab.show();
    _estaEditando = false;
}

function obterMonitoramentoEventoSalvar() {
    var monitoramentoEvento = RetornarObjetoPesquisa(_monitoramentoEvento);

    monitoramentoEvento["Gatilho"] = obterMonitoramentoEventoGatilhoSalvar();
    monitoramentoEvento["Horario"] = obterMonitoramentoEventoHorarioSalvar();
    monitoramentoEvento["StatusViagem"] = obterMonitoramentoEventoStatusViagemSalvar();
    monitoramentoEvento["TipoDeCarga"] = obterMonitoramentoEventoTipoDeCargaSalvar();
    monitoramentoEvento["TipoDeOperacao"] = obterMonitoramentoEventoTipoDeOperacaoSalvar();
    monitoramentoEvento["Tratativas"] = obterMonitoramentoEventoTratativasSalvar();
    monitoramentoEvento["TratativaAutomatica"] = obterMonitoramentoEventoTratativaAutomaticaSalvar();
    monitoramentoEvento["Causa"] = obterMonitoramentoEventoCausasSalvar();
    monitoramentoEvento["AcaoTratativa"] = obterMonitoramentoEventoAcaoTratativaSalvar();

    return monitoramentoEvento;
}

function recarregarGridMonitoramentoEvento() {
    _gridMonitoramentoEvento.CarregarGrid();
}

function validarCamposObrigatoriosMonitoramentoEvento() {
    var camposInformados = ValidarCamposObrigatorios(_monitoramentoEvento);

    return camposInformados && validarCamposObrigatoriosMonitoramentoEventoGatilho() && validarCamposObrigatoriosMonitoramentoEventoHorario() && validarCamposObrigatoriosMonitoramentoEventoStatusViagem() && validarCamposObrigatoriosMonitoramentoEventoTipoDeCarga() && validarCamposObrigatoriosMonitoramentoEventoTratativaAutomatica();
}

function ObterTiposIntegracaoMonitoramentoEvento() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.LoggiEventosEntrega
        ])
    }, function (r) {
        if (r.Success) {
            _monitoramentoEventoOpcoesTipoIntegracao = new Array();

            for (var i = 0; i < r.Data.length; i++)
                _monitoramentoEventoOpcoesTipoIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
};

function ObterCausasEAcoesPorTipoAlerta(tipoAlerta) {
    executarReST("MonitoramentoEvento/ObterCausasEAcoesPorTipoAlerta", { TipoAlerta: tipoAlerta }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                preencherMonitoramentoEventoCausa(retorno.Data.Causa);
                preencherMonitoramentoEventoAcaoTratativa(retorno.Data.AcaoTratativa);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null, false);
}

function preencherIconeEDescricao(dados) {
    var descricaoAlerta = EnumTipoMonitoramentoEvento.obterDescricaoPorTipo(parseInt(dados));
    _monitoramentoEvento.DescricaoAlerta.visible(true);
    _monitoramentoEvento.DescricaoAlerta.text(descricaoAlerta);

    var iconeAlerta = EnumTipoMonitoramentoEvento.obterIconePorTipo(parseInt(dados));

    $.get(iconeAlerta, function (data) {
        var svg = $(data).find('svg');
        $('#icone-alerta').empty().append(svg);
    });

}