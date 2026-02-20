/// <reference path="../../Consultas/PortifolioAtendimento.js" />
/// <reference path="../../Consultas/MotivoChamado.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="EtapaCRM.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="AtendimentoCRMNotaFiscal.js" />
/// <reference path="AtendimentoCRMEntrega.js" />


var _novoAtendimento;
var codigoCarga;
var _gridNotas;

var NovoAtendimento = function () {

    self.selectedOption = ko.observable();
    this.TipoAtendimento = PropertyEntity({ text: "Tipo de Atendimento", val: ko.computed(function () { return self.selectedOption() === "numeroDT" ? "Por n\u00FAmero da DT" : "Por coleta"; }), def: "", visible: ko.observable(true) });
    this.NumeroDT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "N\u00FAmero da DT", idBtnSearch: guid(), eventChange: cargaBlur, enable: ko.observable(true), visible: ko.observable(true), issue: 195 });
    this.Portifolio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Portifolio da Ocorr\u00eancia:", idBtnSearch: guid(), visible: true });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid(), visible: true });
    this.NFe = PropertyEntity({ type: types.event, text: "Nota Fiscal", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.NotasFiscais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.PerguntasAnalise = PropertyEntity({ eventClick: PerguntasAnaliseClick, type: types.event, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: "Perguntas de An\u00e1lise:", idBtnSearch: guid(), visible: true });
    this.ChegadaEstadia = PropertyEntity({ text: "Chegada:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.SaidaEstadia = PropertyEntity({ text: "Saida:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataAgendamentoEstadia = PropertyEntity({ text: "Data do Agendamento:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.QuantidadeEstadia = PropertyEntity({ type: typesKnockout.date, visible: ko.observable(0), text: "Quantidade" });
    this.ClassificacaoEstadia = PropertyEntity({ type: typesKnockout.string, visible: ko.observable(''), text: "Classifica\u00e7\u00e3o" });

    this.DataPrimeiraTentativaInicio = PropertyEntity({ text: "Data/Hora Inicial", getType: typesKnockout.dateTime, required: false, onChange: verificarDatasLiberarModalNovaEntrega, visible: ko.observable(false) });
    this.DataPrimeiraTentativaFim = PropertyEntity({ text: "Data/Hora Final", getType: typesKnockout.dateTime, required: false, eventChange: verificarDatasLiberarModalNovaEntrega, visible: ko.observable(false) });
    this.DataSegundaTentativaInicio = PropertyEntity({ text: "Data/Hora Inicial", getType: typesKnockout.dateTime, required: false, eventChange: verificarDatasLiberarModalNovaEntrega, visible: ko.observable(false) });
    this.DataSegundaTentativaFim = PropertyEntity({ text: "Data/Hora Final", getType: typesKnockout.dateTime, required: false, eventChange: verificarDatasLiberarModalNovaEntrega, visible: ko.observable(false) });

    this.NovaTentativaEntrega = PropertyEntity({ eventClick: exibirModalNovaTentativaEntrega, type: types.event, text: "Tentativa Entrega", id: guid(), visible: ko.observable(true) });   

    this.ConfirmaEntrega = PropertyEntity({
        eventClick: function (e) {
            confirmaEntregaDocumento();
        }, type: types.event, text: "Confirmar entrega", idGrid: guid(), icon: ko.observable("fal fa-share"), visible: ko.observable(true)
    });

    this.EntregaNaoRealizada = PropertyEntity({
        eventClick: function (e) {
            entregaNaoRealizada();
        }, type: types.event, text: "Entrega não realizada", idGrid: guid(), icon: ko.observable("fal fa-share"), visible: ko.observable(true)
    });

    this.NovoAtendimento = PropertyEntity({
        eventClick: function (e) {
            if (e.NovoAtendimento.visibleFade()) {
                e.NovoAtendimento.visibleFade(false);
            } else {
                e.NovoAtendimento.visibleFade(true);
                e.Continuar.visible(true)
            }
        }, type: types.event, text: "Novo atendimento", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Continuar = PropertyEntity({
        eventClick: function (e) {
            let radioButtonSelecionado = self.selectedOption();
            if (radioButtonSelecionado === "numeroDT" || "coleta") {                
                e.Continuar.visibleFade(true);
                e.Voltar.visible(true);
            } else {
                // Nenhum radio button selecionado, nenhuma ação será realizada             
            }
        },
        type: types.event,
        text: "Continuar",
        idFade: guid(),
        visibleFade: ko.observable(false),
        visible: ko.observable(false)
    });

    this.Voltar = PropertyEntity({
        eventClick: function (e) {
            e.NovoAtendimento.visibleFade(true);
            e.Continuar.visibleFade(false);
            e.Voltar.visible(false);
        },
        type: types.event,
        text: "Voltar",
        visible: ko.observable(false)
    });
};

function confirmaEntregaDocumento() {
    carregarModalAtendimentoCrmHTML(loadAtendimentoCRMEntrega, 'divConfirmaEntrega');
}

function entregaNaoRealizada() {
    carregarModalAtendimentoCrmHTML(loadAtendimentoCRMEntrega, 'divEntregaNaoRealizada');
}

function verificarDatasLiberarModalNovaEntrega() {
    _novoAtendimento.NovaTentativaEntrega.visible(_novoAtendimento.DataPrimeiraTentativaInicio.val() != '' && _novoAtendimento.DataPrimeiraTentativaFim.val() != '' && _novoAtendimento.DataSegundaTentativaInicio.val() != '' && _novoAtendimento.DataSegundaTentativaFim.val() != '');
    console.log("as");
}

function exibirModalNovaTentativaEntrega() {    
    carregarModalAtendimentoCrmHTML(loadAtendimentoCRMEntrega, 'divNovaTentativaEntrega');
}

function verificarSelecaoRadio() {
    let radioButtonSelecionado = $("input[name='classificacao']:checked").val();

    if (radioButtonSelecionado === "operacional") {
        _novoAtendimento.DataSegundaTentativaFim.visible(true);
        _novoAtendimento.DataSegundaTentativaInicio.visible(true);
        _novoAtendimento.DataPrimeiraTentativaFim.visible(true);
        _novoAtendimento.DataPrimeiraTentativaInicio.visible(true);
    }
    else {
        _novoAtendimento.DataSegundaTentativaFim.visible(false);
        _novoAtendimento.DataSegundaTentativaInicio.visible(false);
        _novoAtendimento.DataPrimeiraTentativaFim.visible(false);
        _novoAtendimento.DataPrimeiraTentativaInicio.visible(false);
    }
}

function loadAtendimentoCRM() {

    _novoAtendimento = new NovoAtendimento();
    KoBindings(_novoAtendimento, "knockoutAtendimentoCRM", false, _novoAtendimento.NovoAtendimento.id);   

    //LoadEtapaAtendimentoCRM();

    carregarConteudosAtendimentoCrmHTML();

    new BuscarMotivoChamado(_novoAtendimento.Motivo, null, null, null);
    new BuscarPortifolioAtendimento(_novoAtendimento.Portifolio, null, null);

    BuscarCargas(_novoAtendimento.NumeroDT, retornoCarga, null, null, null, null, null, null, true);
    loadAtendimentoCRMNotaFiscal();
    $("input[name='classificacao']").on('change', verificarSelecaoRadio);
}

function cargaBlur() {
    if (_novoAtendimento.NumeroDT.val() == "") {
        _novoAtendimento.NumeroDT.codEntity(0);
    }
}

function retornoCarga(data) {
    executarReST("Ocorrencia/ObterDetalhesCarga", { Carga: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {

                codigoCarga = data.Codigo;
                _novoAtendimento.NumeroDT.codEntity(data.Codigo);
                _novoAtendimento.NumeroDT.entityDescription(data.CodigoCargaEmbarcador);
                _novoAtendimento.NumeroDT.val(data.CodigoCargaEmbarcador)

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function PerguntasAnaliseClick(e, data) {
    Global.abrirModal('divPerguntasAnalise');
}

function carregarConteudosAtendimentoCrmHTML() {
    $.get("Content/Static/AtendimentoCRM/AtendimentoCRMModais.html?dyn=" + guid(), function (data) {
        $("#ModaisAtendimentoCRM").html(data);
    });
}
