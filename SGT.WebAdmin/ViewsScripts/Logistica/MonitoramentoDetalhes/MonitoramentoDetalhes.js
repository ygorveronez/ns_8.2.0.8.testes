/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatus.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatusViagemTipoRegra.js" />
/// <reference path="../Tracking/Tracking.lib.js" />

// #region Objetos Globais do Arquivo

var _detalheMonitoramento;
var _detalheMonitoramentoAlterarDatasRaioEntrega;
var _detalheMonitoramentoAlterarHistoricoStatusViagem;
var _detalheMonitoramentoAlterarStatus;
var _gridDetalheMonitoramentoDestinos;
var _gridDetalheMonitoramentoPedidos;
var _gridDetalheMonitoramentoHistoricoStatusViagem;
var _gridDetalheMonitoramentoHistoricoVeiculos;
var _gridDetalheMonitoramentoPermanenciaClientes;
var _gridDetalheMonitoramentoPermanenciaSubareas;
var _headerDetalhesMonitoramento;
var _qualidadeMonitoramento;
var _listaEntregaDestino = [];
var _listaStatusViagem = [];
var _listaSubarea = [];

// #endregion Objetos Globais do Arquivo 

// #region Classes

var DetalheMonitoramento = function () {
    this.Codigo = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Codigo.getFieldDescription() });
    this.Data = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Criacao.getFieldDescription(), visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Inicio.getFieldDescription(), visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Fim.getFieldDescription(), visible: ko.observable(true) });

    this.Motorista = PropertyEntity({});
    this.MotoristaNome = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Nome.getFieldDescription() });
    this.MotoristaCPF = PropertyEntity({ text: "CPF: " });
    this.MotoristaTelefone = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Telefone.getFieldDescription() });

    this.Cavalo = PropertyEntity({ visible: ko.observable(true) });
    this.CavaloPlaca = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Placa.getFieldDescription() });
    this.CavaloTara = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Tara.getFieldDescription() });
    this.CavaloCapacidade = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Capacidade.getFieldDescription() });

    this.Reboque = PropertyEntity({ visible: ko.observable(true) });
    this.ReboquePlaca = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Placa.getFieldDescription() });
    this.ReboqueTara = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Tara.getFieldDescription() });
    this.ReboqueCapacidade = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Capacidade.getFieldDescription() });

    this.PosicaoAtualData = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.PosicaoAtual.getFieldDescription() });
    this.PosicaoAtualLatitude = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Latitude.getFieldDescription() });
    this.PosicaoAtualLongitude = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Longitude.getFieldDescription() });
    this.Velocidade = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Velocidade.getFieldDescription() });
    this.Temperatura = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Temperatura.getFieldDescription() });
    this.Ignicao = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Ignicao.getFieldDescription() });
    this.Tecnologia = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Tecnologia.getFieldDescription() });
    this.NomeConta = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.NomeDaConta.getFieldDescription(), visible: ko.observable(true) });
    this.NumeroEquipamento = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Equipamento.getFieldDescription() });
    this.Rastreador = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Rastreador.getFieldDescription() });
    this.DescricaoRastreador = PropertyEntity({ val: ko.observable("") });

    this.Observacao = PropertyEntity({ val: ko.observable(""), maxlength: 150, visible: ko.observable(true) });
    this.AlterarObservacao = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: AlterarObservacaoMonitoramentoClick, text: Localization.Resources.Consultas.MonitoramentoDetalhes.Salvar, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) });

    this.AlterarStatus = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: AlterarStatusViagemMonitoramentoClick, text: Localization.Resources.Consultas.MonitoramentoDetalhes.Alterar, visible: ko.observable(true) });
    this.FinalizarMonitoramento = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: FinalizarMonitoramentoClick, text: Localization.Resources.Consultas.MonitoramentoDetalhes.FinalizarMonitoramento, visible: ko.observable(true) });
    this.StatusCodigo = PropertyEntity({});
    this.StatusDescricao = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Situacao.getFieldDescription() });
    this.StatusViagemCodigo = PropertyEntity({});
    this.StatusViagemDescricao = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.SituacaoDaViagem.getFieldDescription() });

    this.DistanciaPrevista = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Prevista.getFieldDescription() });
    this.DistanciaRealizada = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Realizada.getFieldDescription() });
    this.DistanciaAteOrigem = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.AteOrigem.getFieldDescription() });
    this.DistanciaAteDestino = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.AteDestino.getFieldDescription() });

    this.TempoEstimado = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Estimado.getFieldDescription() });
    this.TempoRealizado = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Realizada.getFieldDescription() });

    this.ChegadaAgendada = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Saida.getFieldDescription() });
    this.ChegadaPrevistaInicial = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.ChegadaPrevista.getFieldDescription() });
    this.ChegadaPrevistaAtual = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.ChegadaPlanejada.getFieldDescription() });
    this.ChegadaReal = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.ChegadaReal.getFieldDescription() });
    this.NumeroMonitoramentoCarga = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.NumMonitoramentos, val: ko.observable(0), getType: typesKnockout.int, visible: ko.observable(true) });
    this.Critico = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.MonitoramentoCritico.getFieldDescription(), val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });

};

var DetalheMonitoramentoAlterarDatasRaioEntrega = function () {
    this.Codigo = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Codigo.getFieldDescription() });
    this.DataEntradaRaio = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.DataEntrada.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), required: true });
    this.DataSaidaRaio = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.DataSaida.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), required: false });

    this.Confirmar = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Confirmar, type: types.event, val: ko.observable(false), eventClick: ConfirmarAlterarHistoricoDataRaioClienteClick, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Cancelar, type: types.event, val: ko.observable(false), eventClick: CancelarAlterarHistoricoDataRaioClienteClick, visible: ko.observable(true) });
};

var DetalheMonitoramentoAlterarHistoricoStatusViagem = function () {
    this.Codigo = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Codigo.getFieldDescription() });
    this.CodigoMonitoramento = PropertyEntity({});
    this.StatusViagem = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.StatusDaViagem.getFieldDescription(), val: ko.observable(null), options: ko.observable([]), def: null });
    this.DataInicioStatus = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.DataInicio.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), required: true });
    this.DataPrevisaoChegadaPlanta = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.DataPrevisaoChegada.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), required: false, visible: ko.observable(false) });

    this.Confirmar = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Confirmar, type: types.event, val: ko.observable(false), eventClick: ConfirmarAlterarHistoricoStatusViagemMonitoramentoClick, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Cancelar, type: types.event, val: ko.observable(false), eventClick: CancelarAlterarHistoricoStatusViagemMonitoramentoClick, visible: ko.observable(true) });
};

var DetalheMonitoramentoAlterarStatus = function () {
    this.Codigo = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Codigo.getFieldDescription() });
    this.StatusViagem = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.StatusDaViagem.getRequiredFieldDescription(), val: ko.observable(null), options: ko.observable([]), def: null, required: true });
    this.DataInicioStatus = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.DataInicio.getRequiredFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), required: true });
    this.EntregaDestino = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Destino.getFieldDescription(), val: ko.observable([]), def: null, options: ko.observable([]), visible: ko.observable(true) });
    this.DataPrevisaoChegadaPlanta = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.DataPrevisaoChegada.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), required: false, visible: ko.observable(false) });
    this.IndicarEntradaEmSubarea = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.IndicarEntradaSaidaAlgumaSubarea, val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true) });
    this.Subarea = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Subarea.getFieldDescription(), val: ko.observable([]), def: null, options: ko.observable([]) });
    this.DataInicioSubarea = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.DataInicio.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable() });
    this.DataFimSubarea = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.DataFim.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable() });

    this.Confirmar = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Confirmar, type: types.event, val: ko.observable(false), eventClick: ConfirmarAlterarStatusViagemMonitoramentoClick, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Cancelar, type: types.event, val: ko.observable(false), eventClick: CancelarAlterarStatusViagemMonitoramentoClick, visible: ko.observable(true) });
};

var HeaderDetalhesMonitoramento = function () {
    this.Auditoria = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Auditoria, type: types.event, eventClick: auditoriaMonitoramentoClick });
    this.Atualizar = PropertyEntity({ text: Localization.Resources.Consultas.MonitoramentoDetalhes.Atualizar, type: types.event, eventClick: atualizarDetalhesMonitoramentoClick });
};

var QualidadeMonitoramento = function () {
    this.ListaRegrasQualidadeMonitoramento = PropertyEntity({ val: ko.observableArray([]) });
}
// #endregion Classes

// #region Funções de Inicialização

function loadDetalhesMonitoramento() {
    loadHtmlDetalhesMonitoramento(function () {
        _detalheMonitoramento = new DetalheMonitoramento();
        KoBindings(_detalheMonitoramento, "knockoutDetalhesMonitoramento");
        $("#" + _detalheMonitoramento.Critico.id).click(monitoramentoCriticoClick);

        _detalheMonitoramentoAlterarStatus = new DetalheMonitoramentoAlterarStatus();
        KoBindings(_detalheMonitoramentoAlterarStatus, "knockoutDetalhesMonitoramentoAlterarStatus");

        _detalheMonitoramentoAlterarHistoricoStatusViagem = new DetalheMonitoramentoAlterarHistoricoStatusViagem();
        KoBindings(_detalheMonitoramentoAlterarHistoricoStatusViagem, "knockoutDetalhesMonitoramentoAlterarHistoricoStatusViagem");

        _detalheMonitoramentoAlterarDatasRaioEntrega = new DetalheMonitoramentoAlterarDatasRaioEntrega();
        KoBindings(_detalheMonitoramentoAlterarDatasRaioEntrega, "knockoutDetalhesMonitoramentoAlterarDatasRaioEntrega");

        $("#" + _detalheMonitoramentoAlterarStatus.StatusViagem.id).change(StatusViagemChange);
        $("#" + _detalheMonitoramentoAlterarStatus.IndicarEntradaEmSubarea.id).click(VisualizarIndicarEntradaEmSubareaClick);

        _headerDetalhesMonitoramento = new HeaderDetalhesMonitoramento();
        KoBindings(_headerDetalhesMonitoramento, "knockoutHeaderDetalhesMonitoramento");

        _qualidadeMonitoramento = new QualidadeMonitoramento();
        KoBindings(_qualidadeMonitoramento, "knockoutQualidadeMonitoramento");
    });
}

function loadHtmlDetalhesMonitoramento(callback) {
    $.get("Content/Static/Logistica/MonitoramentoDetalhes.html?dyn=" + guid(), function (data) {
        var $containerModalMonitoramentoDetalhes = $("#containerModalMonitoramentoDetalhes");

        if ($containerModalMonitoramentoDetalhes.length == 0) {
            $("#js-page-content").append("<div id='containerModalMonitoramentoDetalhes'></div>");
            $containerModalMonitoramentoDetalhes = $("#containerModalMonitoramentoDetalhes");
        }

        $containerModalMonitoramentoDetalhes.html(data);
        callback();
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function AlterarObservacaoMonitoramentoClick() {
    exibirConfirmacao(Localization.Resources.Consultas.MonitoramentoDetalhes.Confirmacao, Localization.Resources.Consultas.MonitoramentoDetalhes.VoceRealmenteDesejaAlterarObservacaoMonitoramento, function () {
        executarReST("Monitoramento/SalvarObservacaoMonitoramento", { Codigo: _detalheMonitoramento.Codigo.val(), Observacao: _detalheMonitoramento.Observacao.val() }, function (retorno) {
            if (retorno.Success)
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.MonitoramentoDetalhes.Sucesso, retorno.Msg);
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, retorno.Msg);
        });
    });
}

function AlterarStatusViagemMonitoramentoClick() {

    _detalheMonitoramentoAlterarStatus.StatusViagem.options([]);
    _detalheMonitoramentoAlterarStatus.EntregaDestino.options([]);
    _detalheMonitoramentoAlterarStatus.Subarea.options([]);

    executarReST("Monitoramento/BuscarStatusViagemSubareas", { codigo: _detalheMonitoramento.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                _listaStatusViagem = arg.Data.StatusViagem;
                PreencherSelectStatusViagem();
                _detalheMonitoramentoAlterarStatus.Codigo.val(_detalheMonitoramento.Codigo.val());
                _detalheMonitoramentoAlterarStatus.StatusViagem.val(_detalheMonitoramento.StatusViagemCodigo.val());
                _listaEntregaDestino = arg.Data.EntregaDestino;
                _listaSubarea = arg.Data.Subarea;
                StatusViagemChange();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Consultas.MonitoramentoDetalhes.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, arg.Msg);
        }
    });

    _detalheMonitoramentoAlterarStatus.IndicarEntradaEmSubarea.val(false);
    VisualizarIndicarEntradaEmSubareaClick();
    Global.abrirModal('divModalDetalhesMonitoramentoAlterarStatus');
}

function FinalizarMonitoramentoClick() {
    exibirConfirmacao("Confirmação", "Tem certeza que deseja finalizar este Monitoramento?", function () {
        executarReST("Monitoramento/FinalizarMonitoramentoManualmente", { codigo: _detalheMonitoramento.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.MonitoramentoDetalhes.Sucesso, retorno.Msg);
                atualizarDetalhesMonitoramentoClick();

                var pagina = window.location.href;
                if (pagina.includes('MonitoramentoNovo')) {
                    recarregarDadosMonitoramentoNovo();
                } else
                    recarregarDados();
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, retorno.Msg);
        });
    });
}


function atualizarDetalhesMonitoramentoClick() {
    carregarDadosDetalhesMonitoramento();
}

function auditoriaMonitoramentoClick() {
    var data = { Codigo: _detalheMonitoramento.Codigo.val() };
    var closureAuditoria = OpcaoAuditoria("Monitoramento");
    closureAuditoria(data);
}

function CancelarAlterarHistoricoDataRaioClienteClick() {
    Global.fecharModal("divModalDetalhesMonitoramentoAlterarDatasRaioEntrega");
}

function CancelarAlterarHistoricoStatusViagemMonitoramentoClick() {
    Global.fecharModal("divModalDetalhesMonitoramentoAlterarHistoricoStatusViagem");
}

function CancelarAlterarHistoricoStatusViagemMonitoramentoClick() {
    Global.fecharModal("divModalDetalhesMonitoramentoAlterarHistoricoStatusViagem");
}

function CancelarAlterarStatusViagemMonitoramentoClick() {
    Global.fecharModal("divModalDetalhesMonitoramentoAlterarStatus");
    limparCamposAlterarStatusViagemMonitoramento();
}

function ConfirmarAlterarHistoricoDataRaioClienteClick(e, sender) {
    if (ValidarCamposObrigatorios(_detalheMonitoramentoAlterarDatasRaioEntrega)) {
        exibirConfirmacao(Localization.Resources.Consultas.MonitoramentoDetalhes.Confirmacao, Localization.Resources.Consultas.MonitoramentoDetalhes.VoceRealmenteDesejaAlterarData, function () {
            Salvar(_detalheMonitoramentoAlterarDatasRaioEntrega, "Monitoramento/AlterarEntradaSaidaRaioCliente", function (retorno) {
                if (retorno.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.MonitoramentoDetalhes.Sucesso, retorno.Msg);
                    CancelarAlterarHistoricoDataRaioClienteClick();
                    carregarDadosDetalhesMonitoramento();
                } else exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, retorno.Msg);
            }, sender);
        });
    } else {
        var campo = _detalheMonitoramentoAlterarDatasRaioEntrega.DataEntradaRaio;
        var estaInformado = true;

        if (campo) {
            if (campo.val() === null || campo.val() === undefined)
                estaInformado = false;
            else {
                var normalized = (campo.val() || "").replace(/\//g, "").replace(/:/g, "").replace(/_/g, "");
                if (normalized === "" || normalized.length < 12) // 12 para datetime ddmmyyyyHHMM mínimo
                    estaInformado = false;
            }
        }

        if (!estaInformado) {
            // Mensagem personalizada — altere o texto conforme desejar
            exibirMensagem(tipoMensagem.atencao, "Campo obrigatório", "Informe pelo menos o horário de entrada ou o de saída para salvar.");
        }
    }


}

function ConfirmarAlterarHistoricoStatusViagemMonitoramentoClick(e, sender) {
    if (ValidarCamposObrigatorios(_detalheMonitoramentoAlterarHistoricoStatusViagem)) {
        exibirConfirmacao(Localization.Resources.Consultas.MonitoramentoDetalhes.Confirmacao, Localization.Resources.Consultas.MonitoramentoDetalhes.VoceRealmenteDesejaAlterarDataXParaX.format(_detalheMonitoramentoAlterarHistoricoStatusViagem.StatusViagem.val(), _detalheMonitoramentoAlterarHistoricoStatusViagem.DataInicioStatus.val()), function () {
            Salvar(_detalheMonitoramentoAlterarHistoricoStatusViagem, "Monitoramento/AlterarHistoricoStatusViagem", function (retorno) {
                if (retorno.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.MonitoramentoDetalhes.Sucesso, retorno.Msg);
                    CancelarAlterarHistoricoStatusViagemMonitoramentoClick();
                    carregarDadosDetalhesMonitoramento();
                } else exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, retorno.Msg);
            }, sender);
        });
    }
}

function ConfirmarAlterarStatusViagemMonitoramentoClick(e, sender) {
    var novoStatus;
    var options = _detalheMonitoramentoAlterarStatus.StatusViagem.options();
    for (var i = 0; i < options.length; i++) {
        if (options[i].value == _detalheMonitoramentoAlterarStatus.StatusViagem.val()) {
            novoStatus = options[i].text;
            break;
        }
    }
    if (ValidarCamposObrigatorios(_detalheMonitoramentoAlterarStatus)) {
        exibirConfirmacao(Localization.Resources.Consultas.MonitoramentoDetalhes.Confirmacao, Localization.Resources.Consultas.MonitoramentoDetalhes.VoceRealmenteDesejaAlterarStatusDeXParaX.format(_detalheMonitoramento.StatusViagemDescricao.val(), novoStatus, _detalheMonitoramentoAlterarStatus.DataInicioStatus.val()), function () {
            Salvar(_detalheMonitoramentoAlterarStatus, "Monitoramento/AlterarStatusViagem", function (retorno) {
                if (retorno.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.MonitoramentoDetalhes.Sucesso, retorno.Msg);
                    CancelarAlterarStatusViagemMonitoramentoClick();
                    carregarDadosDetalhesMonitoramento();
                } else exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, retorno.Msg);
            }, sender);
        });
    }
}

function editarHistoricoPermanenciaClienteClick(row) {
    ExibirModalAlterarHistoricoPermanenciaCliente();
    _detalheMonitoramentoAlterarDatasRaioEntrega.Codigo.val(row.Codigo);
    _detalheMonitoramentoAlterarDatasRaioEntrega.DataEntradaRaio.val(row.DataEntrada);
    _detalheMonitoramentoAlterarDatasRaioEntrega.DataSaidaRaio.val(row.DataSaida);
}

function editarHistoricoStatusMonitoramentoClick(row) {
    _detalheMonitoramentoAlterarHistoricoStatusViagem.DataPrevisaoChegadaPlanta.visible(false);

    if (permiteAlterarOuExcluirHistoricoStatus(row)) {
        ExibirModalAlterarHistoricoStatusViagem();
        _detalheMonitoramentoAlterarHistoricoStatusViagem.Codigo.val(row.Codigo);
        _detalheMonitoramentoAlterarHistoricoStatusViagem.CodigoMonitoramento.val(row.CodigoMonitoramento);
        _detalheMonitoramentoAlterarHistoricoStatusViagem.StatusViagem.val(row.Status);
        _detalheMonitoramentoAlterarHistoricoStatusViagem.DataInicioStatus.val(row.DataInicio);
        _detalheMonitoramentoAlterarHistoricoStatusViagem.DataPrevisaoChegadaPlanta.val(row.DataPrevistaChegadaPlanta);
        if (row.TipoRegra == EnumMonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta || row.TipoRegra == EnumMonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta || row.TipoRegra == EnumMonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento) {
            _detalheMonitoramentoAlterarHistoricoStatusViagem.DataPrevisaoChegadaPlanta.visible(true);
        }

    } else {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.NaoPermitido, Localization.Resources.Consultas.MonitoramentoDetalhes.MonitoramentoFinalizado);
    }
}

function excluirHistoricoStatusMonitoramentoClick(row) {
    if (permiteAlterarOuExcluirHistoricoStatus(row)) {
        if (row.Ultimo) {
            exibirConfirmacao(Localization.Resources.Consultas.MonitoramentoDetalhes.Confirmacao, Localization.Resources.Consultas.MonitoramentoDetalhes.VoceRealmenteDesejaExcluirUltimoStatusX.format(row.Status), function () {
                executarReST(
                    "Monitoramento/ExcluirHistoricoStatusViagem",
                    {
                        Codigo: row.Codigo,
                        CodigoMonitoramento: row.CodigoMonitoramento
                    },
                    function (retorno) {
                        if (retorno.Success) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.MonitoramentoDetalhes.Sucesso, retorno.Msg);
                            carregarDadosDetalhesMonitoramento();
                        } else exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, retorno.Msg);
                    }
                );
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.NaoPermitido, Localization.Resources.Consultas.MonitoramentoDetalhes.PossivelExcluirApenasUltimoStatus);
        }
    } else {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.NaoPermitido, Localization.Resources.Consultas.MonitoramentoDetalhes.MonitoramentoFinalizado);
    }
}

function monitoramentoCriticoClick() {
    if (statusMonitoramentoNaoEncerrado(_detalheMonitoramento.StatusCodigo.val())) {
        var mensagem = (_detalheMonitoramento.Critico.val()) ? Localization.Resources.Consultas.MonitoramentoDetalhes.MarcarMonitoramentoComoCritico : Localization.Resources.Consultas.MonitoramentoDetalhes.RemoverCriticidadeMonitoramento;
        exibirConfirmacao(Localization.Resources.Consultas.MonitoramentoDetalhes.Confirmacao, Localization.Resources.Consultas.MonitoramentoDetalhes.VoceRealmenteDesejaX.format(mensagem), function () {
            executarReST(
                "Monitoramento/MonitoramentoCritico",
                {
                    Codigo: _detalheMonitoramento.Codigo.val(),
                    Critico: _detalheMonitoramento.Critico.val()
                },
                function (retorno) {
                    if (retorno.Success) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.MonitoramentoDetalhes.Sucesso, retorno.Msg);
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, retorno.Msg);
                        _detalheMonitoramento.Critico.val(!_detalheMonitoramento.Critico.val());
                    }
                    corDetalheMonitoramentoCritico();
                }
            );
        }, function () {
            _detalheMonitoramento.Critico.val(!_detalheMonitoramento.Critico.val());
            corDetalheMonitoramentoCritico();
        });
    }
}

function StatusViagemChange() {

    // Destinos
    var statusSelected = null;
    _detalheMonitoramentoAlterarStatus.EntregaDestino.visible(false);
    _detalheMonitoramentoAlterarStatus.DataPrevisaoChegadaPlanta.visible(false);

    _detalheMonitoramentoAlterarStatus.EntregaDestino.options([]);
    for (var i = 0; i < _listaStatusViagem.length; i++) {
        if (_detalheMonitoramentoAlterarStatus.StatusViagem.val() == _listaStatusViagem[i].Codigo) {
            statusSelected = _listaStatusViagem[i];
            if (_listaStatusViagem[i].Destino) {
                _detalheMonitoramentoAlterarStatus.EntregaDestino.visible(true);
                PreencherSelectEntregaDestino();
            }
            if (_listaStatusViagem[i].EmPlanta) {
                _detalheMonitoramentoAlterarStatus.DataPrevisaoChegadaPlanta.visible(true);
            }

            break;
        }
    }

    // Subáreas
    var options = [];
    for (var i = 0; i < statusSelected.TiposSubareas.length; i++) {
        for (var j = 0; j < _listaSubarea.length; j++) {
            if (statusSelected.TiposSubareas[i] == _listaSubarea[j].Tipo) {
                options.push({
                    value: _listaSubarea[j].Codigo,
                    text: _listaSubarea[j].Descricao
                });
            }
        }
    }
    _detalheMonitoramentoAlterarStatus.Subarea.options(options);
    if (options.length == 0) {
        _detalheMonitoramentoAlterarStatus.IndicarEntradaEmSubarea.val(false);
        _detalheMonitoramentoAlterarStatus.IndicarEntradaEmSubarea.enable(false);
        VisualizarIndicarEntradaEmSubareaClick();
    } else {
        _detalheMonitoramentoAlterarStatus.IndicarEntradaEmSubarea.enable(true);
    }
}

function VisualizarIndicarEntradaEmSubareaClick() {
    if (_detalheMonitoramentoAlterarStatus.IndicarEntradaEmSubarea.val())
        $("#divVisualizarIndicarEntradaEmSubarea").show();
    else
        $("#divVisualizarIndicarEntradaEmSubarea").hide();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDetalhesMonitoramentoPorCodigo(codigoMonitoramento) {
    _detalheMonitoramento.Codigo.val(codigoMonitoramento);

    carregarDadosDetalhesMonitoramento();
}

// #endregion Funções Públicas

// #region Métodos Privados

function callbackRowDetalheMonitoramentoDestinos(nRow, aData) {
    if (aData.OrdemRealizada < 0) {
        var indice = _gridDetalheMonitoramentoDestinos.GetColumnIndex('OrdemRealizada');
        if (indice == undefined) return;
        var coluna = $(nRow).find('td').eq(indice);
        if (coluna) $(coluna).html("");
    }
}

function callbackRowDetalheMonitoramentoHistoricoStatusViagem(nRow, aData) {
    if (aData.DataFim != "") {
        var colunaOpcoes = $(nRow).find('td.sorting_disabled_opcao');
        if (colunaOpcoes) {
            colunaOpcoes.find('div').hide();
        }
    }
}

function callbackRowDetalheMonitoramentoPermanencia(grid, nRow, aData) {
    if (aData.DataSaida == "") {
        var indice = grid.GetColumnIndex('Tempo');
        if (indice == undefined) return;
        var coluna = $(nRow).find('td').eq(indice);
        if (coluna) $(coluna).html("");
    }
}

function callbackRowDetalheMonitoramentoPermanenciaClente(nRow, aData) {
    callbackRowDetalheMonitoramentoPermanencia(_gridDetalheMonitoramentoPermanenciaClientes, nRow, aData);
}

function callbackRowDetalheMonitoramentoPermanenciaSubarea(nRow, aData) {
    callbackRowDetalheMonitoramentoPermanencia(_gridDetalheMonitoramentoPermanenciaSubareas, nRow, aData);
}

function carregarDadosDetalhesMonitoramento() {
    executarReST("Monitoramento/ObterDetalhesMonitoramento", { Codigo: _detalheMonitoramento.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            ExibirModalDetalhesMonitoramento();
            PreencherObjetoKnout(_detalheMonitoramento, retorno);

            $(".title-carga-codigo-embarcador").html(retorno.Data.CodigoCargaEmbarcador);
            $(".title-carga-placa").html(retorno.Data.CavaloPlaca + " " + retorno.Data.ReboquePlaca);

            _detalheMonitoramento.DataInicio.visible(retorno.Data.DataInicio != null);
            _detalheMonitoramento.DataFim.visible(retorno.Data.DataFim != null);
            _detalheMonitoramento.Cavalo.visible(retorno.Data.CavaloPlaca != "");
            _detalheMonitoramento.Reboque.visible(retorno.Data.ReboquePlaca != "");

            corDetalheMonitoramentoCritico();

            var selector = "#" + _detalheMonitoramento.Ignicao.id + " i";

            if (retorno.Data.Ignicao) {
                _detalheMonitoramento.Ignicao.val("Ligada");
                $(selector).css("color", TRACKING_IGNICAO_COR_LIGADO);
            }
            else {
                _detalheMonitoramento.Ignicao.val("Desligada");
                $(selector).css("color", TRACKING_IGNICAO_COR_DELIGADO);
            }

            selector = "#" + _detalheMonitoramento.Rastreador.id + " i";

            if (retorno.Data.Rastreador) {
                _detalheMonitoramento.Rastreador.val("Online");
                $(selector).css("color", TRACKING_RASTREADOR_COR_ONLINE);
            }
            else {
                _detalheMonitoramento.Rastreador.val("Offline");
                $(selector).css("color", TRACKING_RASTREADOR_COR_OFFLINE);
            }

            if (retorno.Data.DescricaoRastreador != "") {
                _detalheMonitoramento.DescricaoRastreador.val("(" + retorno.Data.DescricaoRastreador + ")");
            }

            if (!retorno.Data.NomeConta)
                _detalheMonitoramento.NomeConta.visible(false);

            var monitoramentoAberto = statusMonitoramentoNaoEncerrado(retorno.Data.StatusCodigo);

            _detalheMonitoramento.AlterarStatus.visible(monitoramentoAberto && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe);
            _detalheMonitoramento.FinalizarMonitoramento.visible(monitoramentoAberto);
            _detalheMonitoramento.Critico.visible(monitoramentoAberto);

            var menuOpcoesRaioCliente = null;

            if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Monitoramento_InformarDataEntradaSaidaRaio, _PermissoesPersonalizadasMonitoramento)) {
                menuOpcoesRaioCliente = {
                    tipo: TypeOptionMenu.list,
                    descricao: Localization.Resources.Consultas.MonitoramentoDetalhes.Opcoes,
                    opcoes: [
                        { descricao: Localization.Resources.Consultas.MonitoramentoDetalhes.Editar, id: guid(), evento: "onclick", metodo: editarHistoricoPermanenciaClienteClick, tamanho: "5", icone: "" }
                    ],
                    tamanho: 7,
                };
            }

            controlarVisibilidadeAbaQualidadeMonitoramento();

            if (retorno.Data.MonitoramentoQualidade.length > 0)
                PreencherListaRegrasQualidadeMonitoramento(retorno.Data.MonitoramentoQualidade);

            _gridDetalheMonitoramentoDestinos = new GridView("grid-detalhe-monitoramento-destinos", "Monitoramento/ObterDetalhesMonitoramentoDestinos?carga=" + retorno.Data.CodigoCarga, null, menuOpcoesRaioCliente, null, 10, null, true, null, null, 1000, true, null, null, true, callbackRowDetalheMonitoramentoDestinos, false);
            _gridDetalheMonitoramentoDestinos.CarregarGrid();

            _gridDetalheMonitoramentoPedidos = new GridView("grid-detalhe-monitoramento-pedidos", "Monitoramento/ObterDetalhesMonitoramentoPedidos?carga=" + retorno.Data.CodigoCarga, null, null, null, 10, null, true, null, null, 1000, true, null, null, true, null, false);
            _gridDetalheMonitoramentoPedidos.CarregarGrid();

            var menuOpcoesStatus = null;

            if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Monitoramento_AlterarOuExcluirHistoricoDeStatusDoMonitoramento, _PermissoesPersonalizadasMonitoramento)) {
                menuOpcoesStatus = {
                    tipo: TypeOptionMenu.list,
                    descricao: Localization.Resources.Consultas.MonitoramentoDetalhes.Opcoes,
                    opcoes: [
                        { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarHistoricoStatusMonitoramentoClick, tamanho: "5", icone: "" },
                        { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: excluirHistoricoStatusMonitoramentoClick, tamanho: "5", icone: "" }
                    ],
                    tamanho: 7,
                };
            }

            _gridDetalheMonitoramentoHistoricoVeiculos = new GridView("grid-detalhe-monitoramento-historico-veiculos", "Monitoramento/ObterHistoricoVeiculos?codigo=" + _detalheMonitoramento.Codigo.val(), null, null, null, 10, null, true, null, null, 1000, true, null, null, true, null, false);
            _gridDetalheMonitoramentoHistoricoVeiculos.CarregarGrid();

            _gridDetalheMonitoramentoHistoricoStatusViagem = new GridView("grid-detalhe-monitoramento-historico-status-viagem", "Monitoramento/ObterDetalhesMonitoramentoHistoricoStatusViagem?codigo=" + _detalheMonitoramento.Codigo.val(), null, menuOpcoesStatus, null, 10, null, true, null, null, 1000, true, null, null, true, callbackRowDetalheMonitoramentoHistoricoStatusViagem, false);
            _gridDetalheMonitoramentoHistoricoStatusViagem.CarregarGrid();

            let configuracaoExportacaoPermanenciaCliente = {
                url: "Monitoramento/ExportarGridPermanenciaCliente?carga=" + + retorno.Data.CodigoCarga,
                titulo: "Permanência Cliente"
            };

            _gridDetalheMonitoramentoPermanenciaClientes = new GridView("grid-detalhe-monitoramento-permanencia-cliente", "Monitoramento/ObterDetalhesMonitoramentoPermanenciaCliente?carga=" + retorno.Data.CodigoCarga, null, null, null, 10, null, true, null, null, 1000, true, configuracaoExportacaoPermanenciaCliente, null, true, callbackRowDetalheMonitoramentoPermanenciaClente, false);
            _gridDetalheMonitoramentoPermanenciaClientes.CarregarGrid();

            _gridDetalheMonitoramentoPermanenciaSubareas = new GridView("grid-detalhe-monitoramento-permanencia-subarea", "Monitoramento/ObterDetalhesMonitoramentoPermanenciaSubarea?carga=" + retorno.Data.CodigoCarga, null, null, null, 10, null, true, null, null, 1000, true, null, null, true, callbackRowDetalheMonitoramentoPermanenciaSubarea, false);
            _gridDetalheMonitoramentoPermanenciaSubareas.CarregarGrid();

        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, retorno.Msg);
    });
}

function corDetalheMonitoramentoCritico() {
    var color = (_detalheMonitoramento.Critico.val()) ? TRACKING_MONITORAMENTO_CRITICO_COR : "";
    $("#" + _detalheMonitoramento.Critico.id).parent().parent().find("i").css("color", color);
}

function ExibirModalAlterarHistoricoPermanenciaCliente() {
    if ($("#divModalDetalhesMonitoramentoAlterarDatasRaioEntrega").is(":hidden"))
        Global.abrirModal('divModalDetalhesMonitoramentoAlterarDatasRaioEntrega');
}

function ExibirModalAlterarHistoricoStatusViagem() {
    if ($("#divModalDetalhesMonitoramentoAlterarHistoricoStatusViagem").is(":hidden"))
        Global.abrirModal('divModalDetalhesMonitoramentoAlterarHistoricoStatusViagem');
}

function ExibirModalDetalhesMonitoramento() {
    if ($("#divModalDetalhesMonitoramento").is(":hidden"))
        Global.abrirModal('divModalDetalhesMonitoramento');
}

function limparCamposAlterarStatusViagemMonitoramento() {
    LimparCampos(_detalheMonitoramentoAlterarStatus);
}

function permiteAlterarOuExcluirHistoricoStatus(row) {
    var valor = row.TipoRegra;
    return (valor != EnumMonitoramentoStatusViagemTipoRegra.Finalizado && valor != EnumMonitoramentoStatusViagemTipoRegra.Cancelado);
}

function PreencherSelectEntregaDestino() {
    var options = [];
    for (var i = 0; i < _listaEntregaDestino.length; i++) {
        options.push({
            value: _listaEntregaDestino[i].Codigo,
            text: _listaEntregaDestino[i].Descricao
        });
    }
    _detalheMonitoramentoAlterarStatus.EntregaDestino.options(options);
}

function PreencherSelectStatusViagem() {
    var options = [];
    for (var i = 0; i < _listaStatusViagem.length; i++) {
        options.push({
            value: _listaStatusViagem[i].Codigo,
            text: _listaStatusViagem[i].Descricao
        });
    }
    _detalheMonitoramentoAlterarStatus.StatusViagem.options(options);
}

function statusMonitoramentoNaoEncerrado(status) {
    return (status == EnumMonitoramentoStatus.Aguardando || status == EnumMonitoramentoStatus.Iniciado);
}

function PreencherListaRegrasQualidadeMonitoramento(data) {
    _qualidadeMonitoramento.ListaRegrasQualidadeMonitoramento.val([]);
    var lista = [];

    for (i = 0; i < data.length; i++) {
        lista.push({
            text: data[i].Regra,
            val: data[i].Resultado
        });
    }

    $('#liQualidadeMonitoramento').show();
    _qualidadeMonitoramento.ListaRegrasQualidadeMonitoramento.val(lista);
}

function controlarVisibilidadeAbaQualidadeMonitoramento() {
    $('#liQualidadeMonitoramento').hide();

    if ($('#tabQualidadeMonitoramento').hasClass('active'))
        $('#tabQualidadeMonitoramento').removeClass('active');

    $('#knockoutDetalhesMonitoramento').addClass('active');
    $('#liTabDetalhesMonitoramento').addClass('active');
    $('#liTabDetalhesMonitoramento').addClass('show');
}
// #endregion Métodos Privados
