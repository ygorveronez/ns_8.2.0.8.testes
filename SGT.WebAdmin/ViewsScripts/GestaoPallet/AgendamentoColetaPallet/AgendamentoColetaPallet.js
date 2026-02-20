/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumCodigoFiltroPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumStatusAgendamentoColetaPallet.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumResponsavelPallet.js" />
/// <reference path="../../../ViewsScripts/Consultas/Carga.js" />

var _pesquisaAgendamentoColeta;
var _gridPesquisaAgendamentoColeta;
var _etapaAgendamentoColetaPallet;

function inicializarAgendamentoColeta() {
    _pesquisaAgendamentoColeta = new PesquisaAgendamentoColeta();
    KoBindings(_pesquisaAgendamentoColeta, "knockoutPesquisaAgendamentoColetaPallet", false, _pesquisaAgendamentoColeta.Pesquisar.id);

    BuscarFilial(_pesquisaAgendamentoColeta.Filial);
    BuscarCargas(_pesquisaAgendamentoColeta.Carga);

    carregarGridPesquisaAgendamentoColetaPallet();
    carregarEtapasAgendamentoColetaPallet();
    carregarEtapaAgendamentoColetaPallet();

    carregarDadosAcompanhamentoAgendamentoPallet();
}

// Knouts

var PesquisaAgendamentoColeta = function () {
    this.NumeroOrdem = PropertyEntity({ text: "Número da ordem:", getType: typesKnockout.int, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial", idBtnSearch: guid(), visible: ko.observable(true) });
    this.StatusAgendamento = PropertyEntity({ text: "Status Agendamento", val: ko.observable(EnumStatusAgendamentoColetaPallet.Todos), options: EnumStatusAgendamentoColetaPallet.obterOpcoesPesquisa(), def: EnumStatusAgendamentoColetaPallet.Todos });
    this.DataOrdem = PropertyEntity({ text: "Data da Ordem", getType: typesKnockout.date, val: ko.observable(""), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            _gridPesquisaAgendamentoColeta.CarregarGrid();
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var AgendamentoColetaPallet = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.QuantidadePallets = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, text: "*Quantidade de Pallet:", required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Carga:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Motorista:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Veículo:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Transportador:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.ResponsavelPallet = PropertyEntity({ text: "Responsável Pallet", val: ko.observable(EnumResponsavelPallet.Todos), options: EnumResponsavelPallet.obterOpcoesPesquisaAgendamento(), def: EnumResponsavelPallet.Todos, required: ko.observable(true), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente", idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.DescricaoTransportador = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string, text: "Transportador:", required: ko.observable(false), enable: ko.observable(false), visible: ko.observable(false) });


    this.Adicionar = PropertyEntity({ eventClick: adicionarAgendamentoColetaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparAgendamentoColetaClick, type: types.event, text: "Limpar", visible: ko.observable(true) });

    this.ResponsavelPallet.val.subscribe(exibirCamposAgendamento);
}

// Knouts

function carregarEtapaAgendamentoColetaPallet() {
    _etapaAgendamentoColetaPallet = new AgendamentoColetaPallet();
    KoBindings(_etapaAgendamentoColetaPallet, "knockoutAgendamentoColetaPallet");

    BuscarTransportadores(_etapaAgendamentoColetaPallet.Transportador);
    BuscarClientes(_etapaAgendamentoColetaPallet.Cliente);
    BuscarCargas(_etapaAgendamentoColetaPallet.Carga, preencherCampoTransportador);
    BuscarVeiculos(_etapaAgendamentoColetaPallet.Veiculo);
    BuscarMotoristas(_etapaAgendamentoColetaPallet.Motorista);
}

function carregarGridPesquisaAgendamentoColetaPallet() {
    var configuracaoExportacao = {
        url: "AgendamentoColetaPallet/ExportarPesquisa",
        titulo: "Agendamento de Coleta de Pallet"
    };

    var detalhes = {
        descricao: "Carregar",
        id: guid(),
        evento: "onclick",
        metodo: carregarAgendamentoClick,
        tamanho: "5",
        icone: "",
        visibilidade: true
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [detalhes] };

    _gridPesquisaAgendamentoColeta = new GridView("grid-pesquisa-agendamento-coleta", "AgendamentoColetaPallet/Pesquisa", _pesquisaAgendamentoColeta, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao);
    _gridPesquisaAgendamentoColeta.CarregarGrid();
}

function exibirCamposAgendamento(newValue) {
    _etapaAgendamentoColetaPallet.Cliente.visible(newValue == EnumResponsavelPallet.Cliente);
    _etapaAgendamentoColetaPallet.Cliente.required(newValue == EnumResponsavelPallet.Cliente);

    _etapaAgendamentoColetaPallet.DescricaoTransportador.visible(newValue == EnumResponsavelPallet.Transportador);

    LimparCampo(_etapaAgendamentoColetaPallet.Cliente);
    LimparCampo(_etapaAgendamentoColetaPallet.Transportador);
}

function carregarAgendamentoClick(agendamentoPalletSelecionado) {
    _pesquisaAgendamentoColeta.ExibirFiltros.visibleFade(false);

    LimparTodosCamposAgendamentoColetaPallet();
    BuscarAgendamentoPallet(agendamentoPalletSelecionado);
}

function LimparTodosCamposAgendamentoColetaPallet() {
    LimparCampos(_etapaAgendamentoColetaPallet);
    LimparCampos(_etapaAcompanhamentoAgendamentoPallet);

    SetarEnableCamposKnockout(_etapaAgendamentoColetaPallet, true);

    _etapaAgendamentoColetaPallet.Adicionar.visible(true);
    _etapaAgendamentoColetaPallet.Limpar.visible(true);
}

function adicionarAgendamentoColetaClick() {
    if (!ValidarCamposObrigatorios(_etapaAgendamentoColetaPallet)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    adicionarAgendamentoColet();
}

function limparAgendamentoColetaClick() {
    SetarEtapasRequisicaoAgendamentoPallet();
    LimparTodosCamposAgendamentoColetaPallet();
}

function SetarEtapasRequisicaoAgendamentoPallet(status) {

    if (status == EnumStatusAgendamentoColetaPallet.EmAndamento) {
        EtapaDadosAcompanhamentoPalletLiberada();
    }
    else if (status == EnumStatusAgendamentoColetaPallet.Finalizado || status == EnumStatusAgendamentoColetaPallet.Cancelado) {
        EtapaAgendamentoFinalizado();
    }
    else
        EtapaAgendamentoColetaPallet();
}

function EtapaAgendamentoFinalizado() {
    EtapaDadosAgendamentoPalletAceita();
    EtapaDadosAcompanhamentoPalletAceita();

    $("#" + _etapas.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa3.idTab + " .step").attr("class", "step green");

    _etapaAgendamentoColetaPallet.Adicionar.visible(false);

    SetarEnableCamposKnockout(_etapaAgendamentoColetaPallet, false);
    SetarEnableCamposKnockout(_etapaAcompanhamentoAgendamentoPallet, false);

    Global.ExibirStep(_etapas.Etapa3.idTab);
}

function EtapaAgendamentoColetaPallet() {
    EtapaDadosAcompanhamentoDesabilitada();
    EtapaAgendamentoFinalizadoPalletDesabilitada();

    $("#" + _etapas.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa1.idTab + " .step").attr("class", "step yellow");

    SetarEnableCamposKnockout(_etapaAgendamentoColetaPallet, false);

    _etapaAgendamentoColetaPallet.Adicionar.visible(true);
    _etapaAgendamentoColetaPallet.Limpar.visible(true);

    Global.ExibirStep(_etapas.Etapa1.idTab);
}

function EtapaDadosAgendamentoPalletDesabilitada() {
    $("#" + _etapas.Etapa1.idTab).prop("disabled", true);
    $("#" + _etapas.Etapa1.idTab + " .step").attr("class", "step");
}

function EtapaDadosAgendamentoPalletAceita() {
    $("#" + _etapas.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa1.idTab + " .step").attr("class", "step green");
}

function EtapaDadosAcompanhamentoPalletAceita() {
    $("#" + _etapas.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa2.idTab + " .step").attr("class", "step green");
}

function EtapaDadosAcompanhamentoDesabilitada() {
    $("#" + _etapas.Etapa2.idTab).prop("disabled", true);
    $("#" + _etapas.Etapa2.idTab + " .step").attr("class", "step");
}

function EtapaAgendamentoFinalizadoPalletDesabilitada() {
    $("#" + _etapas.Etapa3.idTab).prop("disabled", true);
    $("#" + _etapas.Etapa3.idTab + " .step").attr("class", "step");
}

function EtapaDadosAcompanhamentoPalletLiberada() {
    EtapaDadosAgendamentoPalletAceita();
    EtapaAgendamentoFinalizadoPalletDesabilitada();

    $("#" + _etapas.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa2.idTab + " .step").attr("class", "step yellow");

    _etapaAgendamentoColetaPallet.Adicionar.visible(false);

    SetarEnableCamposKnockout(_etapaAgendamentoColetaPallet, false);
    SetarEnableCamposKnockout(_etapaAcompanhamentoAgendamentoPallet, false);

    Global.ExibirStep(_etapas.Etapa2.idTab);
}

function BuscarAgendamentoPallet(agendamentoPallet, callback) {
    var dados = { Codigo: agendamentoPallet.Codigo };

    executarReST("AgendamentoColetaPallet/BuscarPorCodigo", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_etapaAgendamentoColetaPallet, { Data: arg.Data.AgendamentoColetaPallet });
                SetarEtapasRequisicaoAgendamentoPallet(arg.Data.AgendamentoColetaPallet.Situacao);
                PreencherObjetoKnout(_retornoAcompanhamentoAgendamentoPallet, { Data: arg.Data.RetornoAcompanhamentoAgendamentoPallet });
                _etapaAgendamentoColetaPallet.DescricaoTransportador.val(arg.Data.AgendamentoColetaPallet.Transportador.Descricao);
                if (callback) callback();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function imprimirAgendamentoColetaClick(callback) {
    const dados = { Codigo: _etapaAgendamentoColetaPallet.Codigo.val() };

    executarDownload("AgendamentoColetaPallet/Imprimir", dados);
}

function cancelarAgendamentoColetaClick() {

    var dados = { Codigo: _etapaAgendamentoColetaPallet.Codigo.val() };

    executarReST("AgendamentoColetaPallet/Cancelar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Agendamento Cancelado.");
                LimparTodosCamposAgendamentoColetaPallet();
                _gridPesquisaAgendamentoColeta.CarregarGrid();

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function adicionarAgendamentoColet() {
    var dados = RetornarObjetoPesquisa(_etapaAgendamentoColetaPallet);

    executarReST("AgendamentoColetaPallet/Adicionar", dados, function (arg) {

        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Agendamento Adicionado.");
                LimparTodosCamposAgendamentoColetaPallet();
                _gridPesquisaAgendamentoColeta.CarregarGrid();

                if (arg.Msg)
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function preencherCampoTransportador(e) {
    _etapaAgendamentoColetaPallet.Carga.codEntity(e.Codigo);
    _etapaAgendamentoColetaPallet.Carga.val(e.CodigoCargaEmbarcador);
    _etapaAgendamentoColetaPallet.DescricaoTransportador.val(e.Transportador);
}