/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSituacaoAgendamentoPallet.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumResponsavelPallet.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumCodigoFiltroPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSituacaoCargaJanelaCarregamento.js" />

var _pesquisaAgendamentoPallet;
var _gridPesquisaAgendamentoPallet;
var _etapaAgendamentoPallet;

function inicializarAgendamentoPallet() {
    _pesquisaAgendamentoPallet = new PesquisaAgendamentoPallet();
    KoBindings(_pesquisaAgendamentoPallet, "knockoutPesquisaAgendamentoPallet", false, _pesquisaAgendamentoPallet.Pesquisar.id);

    BuscarClientes(_pesquisaAgendamentoPallet.Destinatario);
    BuscarClientes(_pesquisaAgendamentoPallet.Cliente);
    BuscarTransportadores(_pesquisaAgendamentoPallet.Transportador);

    carregarGridPesquisaAgendamentoPallet();
    carregarEtapasAgendamentoPallet();
    carregarEtapaAgendamentoPallet();
    carregarEtapaNFePallet();
    carregarDadosAcompanhamentoPallet();

    $.get("Content/Static/JanelaDescarregamento/HorarioAgendamento.html?dyn=" + guid(), function (data) {
        $("#modalDataEntrega").html(data);

        carregarDataAngendamentoPallet();
    });
}

// Knouts

var PesquisaAgendamentoPallet = function () {
    this.Carga = PropertyEntity({ val: ko.observable(""), def: "", text: "Carga:" });
    this.DataAgendamento = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.dateTime, text: "Data Solicitada:", visible: ko.observable(true) });
    this.Senha = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string, text: "Senha:", visible: ko.observable(true), maxlength: 20 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamento.Todas), options: EnumSituacaoCargaJanelaCarregamento.obterOpcoesPesquisa(), def: EnumSituacaoCargaJanelaCarregamento.Todas, text: "Situação Janela:", visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });

    this.Cliente = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Transportador = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) });

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.AgendamentoPallet,
    });
    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.AgendamentoPallet, _pesquisaAgendamentoPallet) }, type: types.event, text: Localization.Resources.Gerais.Geral.SalvarFiltro, visible: ko.observable(true) });
    this.CarregarFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            abrirBuscaFiltrosManual(e);
        }, type: types.event, text: "Carregar Filtro", idFade: guid(), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            fecharFiltros();
            _gridPesquisaAgendamentoPallet.CarregarGrid();
            _pesquisaAgendamentoPallet.ExibirPesquisa.visibleFade(true);
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirPesquisa = PropertyEntity({
        eventClick: function (e) {
            e.ExibirPesquisa.visibleFade(!e.ExibirPesquisa.visibleFade());
        }, type: types.event, text: "Pesquisar Agendamentos", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var AgendamentoPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.EtapaAgendamentoPallet = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.PeriodoAgendamento = PropertyEntity({ text: ko.observable(""), val: ko.observable(0), def: "", enable: ko.observable(true) });
    this.QuantidadePallets = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, text: "Quantidade de Pallet:", required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.ResponsavelPallet = PropertyEntity({ text: "*Responsável Pallet", val: ko.observable(EnumResponsavelPallet.Todos), options: EnumResponsavelPallet.obterOpcoesPesquisa(), def: EnumResponsavelPallet.Todos, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Destinatário/Tomador:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Remetente:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.ModeloVeicular = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Modelo Veicular:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Tipo de Carga:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Transportador:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) });
    this.Veiculo = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Veículo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Motorista:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.Observacao = PropertyEntity({ val: ko.observable(""), required: ko.observable(false), getType: typesKnockout.text, text: "Observação:", enable: ko.observable(true), visible: ko.observable(true), maxlength: 300 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAgendamentoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAgendamentoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparAgendamentoClick, type: types.event, text: "Limpar", visible: ko.observable(true) });

    this.DataEntrega = PropertyEntity({
        eventClick: exibirDatasAgendamentoClick, val: ko.observable(""), text: "*Data Entrega:", enable: ko.observable(true), visible: ko.observable(true), required: ko.computed(function () {
            return (
                this.Destinatario.codEntity() !== 0 &&
                this.ModeloVeicular.codEntity() !== 0 &&
                this.TipoCarga.codEntity() !== 0 &&
                (this.Remetente.codEntity() !== 0 || !this.Remetente.visible()) &&
                (this.ResponsavelPallet.val() !== "" || !this.ResponsavelPallet.visible()) &&
                (this.Transportador.codEntity() !== 0 || !this.Transportador.visible())
            );
        }, this)
    });
}

// Knouts

function carregarEtapaAgendamentoPallet() {
    _etapaAgendamentoPallet = new AgendamentoPallet();
    KoBindings(_etapaAgendamentoPallet, "knockoutAgendamentoPallet");

    HeaderAuditoria("AgendamentoPallet", _etapaAgendamentoPallet);

    BuscarTransportadores(_etapaAgendamentoPallet.Transportador);
    BuscarClientes(_etapaAgendamentoPallet.Destinatario);
    BuscarTiposdeCarga(_etapaAgendamentoPallet.TipoCarga);
    BuscarModelosVeicularesCarga(_etapaAgendamentoPallet.ModeloVeicular);
    BuscarVeiculos(_etapaAgendamentoPallet.Veiculo, null, _etapaAgendamentoPallet.Transportador, _etapaAgendamentoPallet.ModeloVeicular);
    BuscarMotoristas(_etapaAgendamentoPallet.Motorista);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _etapaAgendamentoPallet.Remetente.visible(true);
        _etapaAgendamentoPallet.Remetente.required(true);

        _etapaAgendamentoPallet.ResponsavelPallet.visible(true);
        _etapaAgendamentoPallet.ResponsavelPallet.required(true);

        BuscarClientes(_etapaAgendamentoPallet.Remetente);
    }
}

function carregarGridPesquisaAgendamentoPallet() {
    var configuracaoExportacao = {
        url: "AgendamentoPallet/ExportarPesquisa",
        titulo: "Agendamento de Abastecimento de Pallet"
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

    _gridPesquisaAgendamentoPallet = new GridView("grid-gestao-pallet-pesquisa-agendamento-abastecimento-pallet", "AgendamentoPallet/Pesquisa", _pesquisaAgendamentoPallet, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao);
    _gridPesquisaAgendamentoPallet.CarregarGrid();
}

function abrirBuscaFiltrosManual(e) {
    var buscaFiltros = new BuscarModeloFiltroPesquisa(e.ModeloFiltrosPesquisa, function (retorno) {
        if (retorno.Codigo == 0) return;

        e.ModeloFiltrosPesquisa.codEntity(retorno.Codigo);
        e.ModeloFiltrosPesquisa.val(retorno.ModeloDescricao);

        PreencherJsonFiltroPesquisa(_pesquisaAgendamentoPallet, retorno.Dados);
    }, EnumCodigoFiltroPesquisa.AgendamentoPallet);

    buscaFiltros.AbrirBusca();
}

function carregarAgendamentoClick(agendamentoPalletSelecionado) {
    _pesquisaAgendamentoPallet.ExibirPesquisa.visibleFade(false);

    LimparTodosCamposPallet();
    BuscarAgendamentoPallet(agendamentoPalletSelecionado);
}

function LimparTodosCamposPallet() {
    LimparCampos(_etapaAgendamentoPallet);
    LimparCampos(_retornoAcompanhamentoPallet);
    LimparUploadNFePallet();

    _etapaAgendamentoPallet.PeriodoAgendamento.text("");

    SetarEnableCamposKnockout(_etapaAgendamentoPallet, true);

    _etapaAgendamentoPallet.Adicionar.visible(true);
    _etapaAgendamentoPallet.Limpar.visible(true);
    _botoesNFePallet.Imprimir.visible(false);
    setarBotaoCancelar(false);
}

function setarBotaoCancelar(visivel) {
    _etapaAgendamentoPallet.Cancelar.visible(visivel);
    _botoesNFePallet.Cancelar.visible(visivel);
    _botoesAcompanhamentoPallet.Cancelar.visible(visivel);
}

function adicionarAgendamentoClick() {
    if (!ValidarCamposObrigatorios(_etapaAgendamentoPallet)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    adicionarAgendamento();
}

function cancelarAgendamentoClick() {
    exibirConfirmacao("Confirmação", "Deseja cancelar o agendamento do pallet selecionado?",
        function () {
            confirmarCancelamento();
        });
}

function limparAgendamentoClick() {
    SetarEtapasRequisicaoAgendamentoPallet();
    LimparTodosCamposPallet();
}

function exibirFiltros() {
    Global.abrirModal('divModalFiltroPesquisaAgendamentoPallet');
}

function fecharFiltros() {
    Global.fecharModal('divModalFiltroPesquisaAgendamentoPallet');
}

function EtapaAgendamentoPallet() {
    EtapaDadosAcompanhamentoDesabilitada();
    EtapaDadosNFeDesabilitada();

    SetarCorEtapa(_etapas.Etapa1.idTab);
    SetarEnableCamposKnockout(_etapaAgendamentoPallet, false);

    _etapaAgendamentoPallet.Adicionar.visible(false);
    _etapaAgendamentoPallet.Limpar.visible(true);

    Global.ExibirStep(_etapas.Etapa1.idTab);
}

function EtapaDadosNFePalletLiberada() {
    EtapaDadosAgendamentoPalletAceita();
    EtapaDadosAcompanhamentoDesabilitada();

    SetarCorEtapa(_etapas.Etapa2.idTab);

    _etapaAgendamentoPallet.Adicionar.visible(false);

    SetarEnableCamposKnockout(_etapaAgendamentoPallet, false);
    SetarEnableCamposKnockout(_retornoAcompanhamentoPallet, false);

    Global.ExibirStep(_etapas.Etapa2.idTab);
}

function EtapaDadosAcompanhamentoPalletLiberada() {
    EtapaDadosAgendamentoPalletAceita();
    EtapaDadosNFePalletAceita();

    SetarCorEtapa(_etapas.Etapa3.idTab);

    _etapaAgendamentoPallet.Adicionar.visible(false);

    SetarEnableCamposKnockout(_etapaAgendamentoPallet, false);
    SetarEnableCamposKnockout(_retornoAcompanhamentoPallet, false);

    Global.ExibirStep(_etapas.Etapa3.idTab);
}

function BuscarAgendamentoPallet(agendamentoPallet, callback) {
    var dados = { Codigo: agendamentoPallet.Codigo };

    executarReST("AgendamentoPallet/BuscarPorCodigo", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_etapaAgendamentoPallet, { Data: arg.Data.AgendamentoPallet });
                PreencherObjetoKnout(_retornoAcompanhamentoPallet, { Data: arg.Data.RetornoAcompanhamentoPallet });
                SetarEtapasRequisicaoAgendamentoPallet(arg.Data.AgendamentoPallet.EtapaAgendamentoPallet, arg.Data.RetornoAcompanhamentoPallet.SituacaoCodigo);

                _NFeAgendamentoPallet.Dropzone.visible((arg.Data.AgendamentoPallet.EtapaAgendamentoPallet == EnumEtapaAgendamentoPallet.NFe) && arg.Data.RetornoAcompanhamentoPallet.SituacaoCodigo !== EnumSituacaoAgendamentoPallet.Cancelado);
                setarBotaoCancelar(arg.Data.RetornoAcompanhamentoPallet.SituacaoCodigo !== EnumSituacaoAgendamentoPallet.Cancelado && arg.Data.RetornoAcompanhamentoPallet.SituacaoCodigo !== EnumSituacaoAgendamentoPallet.Finalizado);
                _botoesNFePallet.Imprimir.visible(true);

                if (_gridNFeAgendamentoPallet)
                    _gridNFeAgendamentoPallet.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function adicionarAgendamento() {
    var dados = RetornarObjetoPesquisa(_etapaAgendamentoPallet);

    executarReST("AgendamentoPallet/Adicionar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Agendamento Adicionado.");

                PreencherObjetoKnout(_etapaAgendamentoPallet, { Data: arg.Data });

                SetarEtapasRequisicaoAgendamentoPallet(EnumEtapaAgendamentoPallet.NFe);
                setarBotaoCancelar(true);
                _NFeAgendamentoPallet.Dropzone.visible(true);
                _botoesNFePallet.Imprimir.visible(true);

                if (_gridNFeAgendamentoPallet)
                    _gridNFeAgendamentoPallet.CarregarGrid();

                if (_gridPesquisaAgendamentoPallet)
                    _gridPesquisaAgendamentoPallet.CarregarGrid();

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function confirmarCancelamento() {
    var dados = { Codigo: _etapaAgendamentoPallet.Codigo.val() };

    executarReST("AgendamentoPallet/Cancelar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "O cancelamento do agendamento foi solicitado.");

                if (_gridPesquisaAgendamentoPallet)
                    _gridPesquisaAgendamentoPallet.CarregarGrid();

                LimparTodosCamposPallet();
                SetarEtapasRequisicaoAgendamentoPallet();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null)
}