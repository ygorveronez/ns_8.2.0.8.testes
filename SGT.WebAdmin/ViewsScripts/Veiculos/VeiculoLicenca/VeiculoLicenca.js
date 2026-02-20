/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Licenca.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Container.js" />
/// <reference path="../../Enumeradores/EnumStatusLicenca.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumControleAlertaForma.js" />
/// <reference path="VeiculoLicencaFaixasTemperatura.js" />
/// <reference path="VeiculoLicencaAnexo.js" />

// #region Objetos Globais do Arquivo

var _CRUDLicencaVeiculo;
var _gridLicencaCadastroVeiculo;
var _gridPesquisaLicenca;
var _knoutCadastroVeiculo;
var _licencaVeiculo;
var _pesquisaLicencaVeiculo;
var isEmbarcador;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CRUDLicencaVeiculo = function () {
    this.Adicionar = PropertyEntity({ type: types.event, eventClick: adicionarLicencaVeiculoClick, text: Localization.Resources.Gerais.Geral.Adicionar, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ type: types.event, eventClick: atualizarLicencaVeiculoClick, text: Localization.Resources.Gerais.Geral.Atualizar, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: limparCamposLicencaVeiculo, text: Localization.Resources.Veiculos.VeiculoLicenca.LimparNovo, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ type: types.event, eventClick: excluirLicencaVeiculoClick, text: Localization.Resources.Gerais.Geral.Excluir, idBtnSearch: guid(), visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "VeiculoLicenca/Importar",
        UrlConfiguracao: "VeiculoLicenca/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O043_VeiculoLicenca,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: false
            };
        },
        CallbackImportacao: function () {
            _gridPesquisaLicenca.CarregarGrid();
        }
    });
};

var LicencaVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ExibirSelecaoContainer = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.BloquearCriacaoPedidoLicencaVencida = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.BloquearCriacaoDoPedidoSeLicencaEstiverVencida, val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.BloquearCriacaoPlanejamentoPedidoLicencaVencida = PropertyEntity({ text: "Bloquear inserção na tela Planejamento de Pedidos se a licença estiver vencida", val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.ClassificacaoRiscoONU = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.VeiculoLicenca.ClassificacaoOnu.getFieldDescription()), required: false, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription()), maxlength: 200, required: ko.observable(true), enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Veiculos.VeiculoLicenca.DataDaEmissao.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Veiculos.VeiculoLicenca.DataDeVencimento.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.FormaAlerta = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.GerarAlertaAoResponsavel.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumControleAlertaForma.obterOpcoes(), def: [], enable: ko.observable(true), visible: ko.observable(true) });
    this.Licenca = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.VeiculoLicenca.Licenca.getFieldDescription()), required: false, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: ko.observable(Localization.Resources.Veiculos.VeiculoLicenca.NumeroDaLicenca.getRequiredFieldDescription()), maxlength: 20, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    var statusLicenca = isEmbarcador ? EnumStatusLicenca.obterOpcoesEmbarcador() : EnumStatusLicenca.obterOpcoes();
    var statusLicencaDefault = isEmbarcador ? EnumStatusLicenca.Aprovado : EnumStatusLicenca.Vigente;
    this.StatusLicenca = PropertyEntity({ val: ko.observable(statusLicencaDefault), options: ko.observable(statusLicenca), def: statusLicencaDefault, text: ko.observable(Localization.Resources.Gerais.Geral.Status.getRequiredFieldDescription()), enable: ko.observable(true) });
    this.Vencido = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.Vencido.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.StatusLicenca.Vencido, Localization.Resources.Enumeradores.StatusLicenca.Vigente), def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Veiculos.VeiculoLicenca.Veiculo.getRequiredFieldDescription()), required: true, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Veiculos.VeiculoLicenca.Filial.getRequiredFieldDescription()), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Veiculos.VeiculoLicenca.TipoDeCarga.getFieldDescription()), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Veiculos.VeiculoLicenca.Container.getRequiredFieldDescription()), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });

    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.NumeroContainer.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(false) });

    this.ExibirSelecaoContainer.val.subscribe(function (novoValor) {
        _licencaVeiculo.NumeroContainer.visible(!novoValor);
        _licencaVeiculo.Container.visible(novoValor);
        _licencaVeiculo.Container.required(novoValor);

        if (!novoValor && _licencaVeiculo.Codigo.val() == 0)
            LimparCampoEntity(_licencaVeiculo.Container);
    });
};

var PesquisaLicencaVeiculo = function () {
    this.DataEmissaoInicial = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.DataEmissaoInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataEmissaoLimite = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.DataEmissaoLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataVencimentoInicial = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.DataDeVencimentoInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataVencimentoLimite = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.DataDeVencimentoLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.VeiculoLicenca.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.VeiculoLicenca.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Placa = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.Placa.getFieldDescription() });
    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.NumeroContainer.getFieldDescription(), visible: ko.observable(false) });

    var statusLicencaPesquisa = isEmbarcador ? EnumStatusLicenca.obterOpcoesPesquisaEmbarcador() : EnumStatusLicenca.obterOpcoesPesquisa();
    this.StatusLicenca = PropertyEntity({ val: ko.observable(EnumStatusLicenca.Todas), options: ko.observable(statusLicencaPesquisa), def: EnumStatusLicenca.Todas, text: Localization.Resources.Gerais.Geral.Status.getFieldDescription() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.VeiculoLicenca.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.StatusVigencia = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoLicenca.Vencido.getFieldDescription(), options: EnumStatusLicenca.obterOpcoesPesquisa(), val: ko.observable(EnumStatusLicenca.Todas), def: EnumStatusLicenca.Todas });

    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoLimite;
    this.DataEmissaoLimite.dateRangeInit = this.DataEmissaoInicial;
    this.DataVencimentoInicial.dateRangeLimit = this.DataVencimentoLimite;
    this.DataVencimentoLimite.dateRangeInit = this.DataVencimentoInicial;

    this.ExibirFiltros = PropertyEntity({ eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()) }, type: types.event, text: Localization.Resources.Veiculos.VeiculoLicenca.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPesquisaLicenca, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

// #endregion Classes

// #region Funções de Inicialização

function loadGridLicencaCadastroVeiculo() {
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("LicencaVeiculo"), icone: "", visibilidade: true };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarLicencaCadastroVeiculo, tamanho: "15", icone: "", visible: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoEditar, auditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: isEmbarcador ? Localization.Resources.Gerais.Geral.Observacao : Localization.Resources.Gerais.Geral.Descricao, width: "10%", className: "text-align-left" },
        { data: "Numero", title: Localization.Resources.Veiculos.VeiculoLicenca.Numero, width: "10%", className: "text-align-center", visible: !isEmbarcador },
        { data: "DataEmissao", title: Localization.Resources.Veiculos.VeiculoLicenca.DataDaEmissao, width: "10%", className: "text-align-center" },
        { data: "DataVencimento", title: Localization.Resources.Veiculos.VeiculoLicenca.DataDeVencimento, width: "10%", className: "text-align-center" },
        { data: "ClassificacaoRiscoONU", title: Localization.Resources.Veiculos.VeiculoLicenca.ClassificacaoOnu, width: "10%", className: "text-align-left", visible: !isEmbarcador },
        { data: "Licenca", title: Localization.Resources.Veiculos.VeiculoLicenca.Licenca, width: "10%", className: "text-align-left" },
        { data: "FaixaTemperatura", title: Localization.Resources.Veiculos.VeiculoLicenca.FaixaDeTemperatura, width: "10%", className: "text-align-left", visible: isEmbarcador },
        { data: "StatusLicenca", title: Localization.Resources.Gerais.Geral.Status, width: "10%", className: "text-align-left" }
    ];

    _gridLicencaCadastroVeiculo = new BasicDataTable(_knoutCadastroVeiculo.Licencas.idGrid, header, menuOpcoes);
    _gridLicencaCadastroVeiculo.CarregarGrid([]);
}

function loadGridPesquisaLicenca() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarLicenca, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "VeiculoLicenca/ExportarPesquisa", titulo: Localization.Resources.Veiculos.VeiculoLicenca.LicencasDosVeiculos };

    _gridPesquisaLicenca = new GridViewExportacao("grid-pesquisa-veiculo-licencas", "VeiculoLicenca/Pesquisa", _pesquisaLicencaVeiculo, menuOpcoes, configuracoesExportacao);
    _gridPesquisaLicenca.SetPermitirEdicaoColunas(true);
    _gridPesquisaLicenca.SetSalvarPreferenciasGrid(true);
    _gridPesquisaLicenca.CarregarGrid();
}

function loadVeiculoLicenca(knoutCadastroVeiculo) {
    $.get("Content/Static/Veiculo/VeiculoLicenca.html?dyn=" + guid(), function (data) {
        $("#container-licenca-veiculo").html(data);

        LocalizeCurrentPage();

        _knoutCadastroVeiculo = knoutCadastroVeiculo;

        isEmbarcador = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe;

        _licencaVeiculo = new LicencaVeiculo();
        KoBindings(_licencaVeiculo, "knockoutLicencaVeiculo");

        _CRUDLicencaVeiculo = new CRUDLicencaVeiculo();
        KoBindings(_CRUDLicencaVeiculo, "knockoutCRUDLicencaVeiculo");

        new BuscarClassificacaoRiscoONU(_licencaVeiculo.ClassificacaoRiscoONU);
        new BuscarLicenca(_licencaVeiculo.Licenca);
        new BuscarFilial(_licencaVeiculo.Filial);
        new BuscarTiposdeCarga(_licencaVeiculo.TipoCarga);
        new BuscarContainers(_licencaVeiculo.Container);

        loadFaixasTemperatura();
        loadLicencaVeiculoAnexos();
        controlarExibicaoCadastroAnexo();

        if (_knoutCadastroVeiculo) {
            _licencaVeiculo.Veiculo.visible(false);
            _CRUDLicencaVeiculo.Cancelar.visible(false);
            _CRUDLicencaVeiculo.Importar.visible(false);

            loadGridLicencaCadastroVeiculo();
        }
        else {
            _pesquisaLicencaVeiculo = new PesquisaLicencaVeiculo();
            KoBindings(_pesquisaLicencaVeiculo, "knockoutPesquisaLicencaVeiculo", false, _pesquisaLicencaVeiculo.Pesquisar.id);

            HeaderAuditoria("LicencaVeiculo", _licencaVeiculo);

            new BuscarMotoristas(_pesquisaLicencaVeiculo.Motorista);
            new BuscarTransportadores(_pesquisaLicencaVeiculo.Empresa);
            new BuscarVeiculos(_licencaVeiculo.Veiculo, RetornoVeiculoLicenca);
            new BuscarFilial(_pesquisaLicencaVeiculo.Filial);

            if (isEmbarcador) {
                _pesquisaLicencaVeiculo.Filial.visible(true);
                _pesquisaLicencaVeiculo.NumeroContainer.visible(true);
            }
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)
                _pesquisaLicencaVeiculo.Empresa.visible(false);

            loadGridPesquisaLicenca();
        }

        configurarLayoutVeiculoLicencaPorTipoSistema();
    });
}

function configurarLayoutVeiculoLicencaPorTipoSistema() {
    if (isEmbarcador) {
        $("#liTabLicencaVeiculoFaixasTemperatura").show();

        _licencaVeiculo.Descricao.required(false);
        _licencaVeiculo.Descricao.text(Localization.Resources.Gerais.Geral.Observacao.getFieldDescription());
        _licencaVeiculo.Numero.required(false);
        _licencaVeiculo.Numero.visible(false);
        _licencaVeiculo.ClassificacaoRiscoONU.visible(false);
        _licencaVeiculo.FormaAlerta.visible(false);
        _licencaVeiculo.Filial.visible(true);
        _licencaVeiculo.Filial.required(true);
        _licencaVeiculo.Vencido.visible(true);
        _licencaVeiculo.StatusLicenca.text(Localization.Resources.Veiculos.VeiculoLicenca.StatusTeste.getRequiredFieldDescription());
        _licencaVeiculo.NumeroContainer.visible(true);
        _licencaVeiculo.TipoCarga.visible(true);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _licencaVeiculo.BloquearCriacaoPedidoLicencaVencida.visible(true);
        _licencaVeiculo.BloquearCriacaoPlanejamentoPedidoLicencaVencida.visible(true);
    }

}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarLicencaVeiculoClick() {
    if (!validarLicencaVeiculo())
        return;

    if (_knoutCadastroVeiculo)
        adicionarLicencaCadastroVeiculo();
    else
        adicionarLicenca();
}

function atualizarLicencaVeiculoClick() {
    if (!validarLicencaVeiculo())
        return;

    if (_knoutCadastroVeiculo)
        atualizarLicencaCadastroVeiculo();
    else
        atualizarLicenca();
}

function excluirLicencaVeiculoClick() {
    if (_knoutCadastroVeiculo)
        excluirLicencaCadastroVeiculo();
    else
        excluirLicenca();
}

function RetornoVeiculoLicenca(data) {
    _licencaVeiculo.Veiculo.val(data.Descricao);
    _licencaVeiculo.Veiculo.codEntity(data.Codigo);

    obterDetalhesVeiculoLicenca();
}

function obterDetalhesVeiculoLicenca() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        return;

    executarReST("VeiculoLicenca/ObterDetalhesVeiculo", { Veiculo: _licencaVeiculo.Veiculo.codEntity() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _licencaVeiculo.ExibirSelecaoContainer.val(arg.Data.ExibirSelecaoContainer);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

// #endregion Funções Associadas a Eventos

// #region Métodos Privados

function controlarExibicaoBotoesLicencaVeiculo(isEdicao) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _CRUDLicencaVeiculo.Adicionar.visible(false);
        _CRUDLicencaVeiculo.Atualizar.visible(false);
        _CRUDLicencaVeiculo.Excluir.visible(false);
        _CRUDLicencaVeiculo.Importar.visible(false);
    }
    else {
        _CRUDLicencaVeiculo.Adicionar.visible(!isEdicao);
        _CRUDLicencaVeiculo.Atualizar.visible(isEdicao);
        _CRUDLicencaVeiculo.Excluir.visible(isEdicao);
    }
}

function controlarExibicaoCadastroAnexo() {
    if (_licencaVeiculo.Codigo.val() > 0) {
        $("#mensagemLicencaVeiculoAnexo").hide();
        $("#knockoutLicencaVeiculoAnexos").show();
    }
    else {
        $("#mensagemLicencaVeiculoAnexo").show();
        $("#knockoutLicencaVeiculoAnexos").hide();
    }
}

function limparCamposLicencaVeiculo() {
    LimparCampos(_licencaVeiculo);
    LimparCampos(_licencaVeiculoAnexo);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe)
        _licencaVeiculo.Veiculo.enable(true);
    _licencaVeiculoAnexo.Anexos.val([]);
    _listaFaixaTemperatura = [];

    controlarExibicaoBotoesLicencaVeiculo(false)
    controlarExibicaoCadastroAnexo();
    recarregarGridFaixaTemperatura();
    Global.ResetarAba("veiculo-licenca-abas");
}

function preencherLicencaVeiculo(dadosLicenca) {
    PreencherObjetoKnout(_licencaVeiculo, { Data: dadosLicenca });

    _listaFaixaTemperatura = dadosLicenca.ListaFaixaTemperatura;

    recarregarGridFaixaTemperatura();
    controlarExibicaoCadastroAnexo();

    if (_licencaVeiculo.Codigo.val() > 0) {
        executarReST("VeiculoLicencaAnexo/ObterAnexo", { Codigo: _licencaVeiculo.Codigo.val() }, function (retorno) {
            if (retorno.Success)
                _licencaVeiculoAnexo.Anexos.val(retorno.Data.Anexos);
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Veiculos.VeiculoLicenca.OcorreuUmaFalhaAoBuscarAnexosDaLicenca);
        });
    }
}

function validarLicencaVeiculo() {
    if (!ValidarCamposObrigatorios(_licencaVeiculo)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return false;
    }

    if (Boolean(_knoutCadastroVeiculo) && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermitirSalvarLicenca, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);
        return false;
    }

    return true;
}

// #endregion Métodos Privados

// #region Métodos Privados - Cadastro de Licenca de Veículo

function adicionarLicenca() {
    var licenca = obterLicencaSalvar();

    executarReST("VeiculoLicenca/Adicionar", { Veiculo: _licencaVeiculo.Veiculo.codEntity(), Licenca: licenca }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Licença do veículo adicionada com sucesso!");
                limparCamposLicencaVeiculo();
                recarregarGridPesquisaLicenca();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarLicenca() {
    var licenca = obterLicencaSalvar();

    executarReST("VeiculoLicenca/Atualizar", { Veiculo: _licencaVeiculo.Veiculo.codEntity(), Licenca: licenca }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Licença do veículo atualizada com sucesso!");
                limparCamposLicencaVeiculo();
                recarregarGridPesquisaLicenca();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function editarLicenca(registroSelecionado) {
    limparCamposLicencaVeiculo();

    _licencaVeiculo.Veiculo.enable(false);

    executarReST("VeiculoLicenca/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            _pesquisaLicencaVeiculo.ExibirFiltros.visibleFade(false);
            preencherLicencaVeiculo(retorno.Data);
            controlarExibicaoBotoesLicencaVeiculo(true);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirLicenca() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Veiculos.VeiculoLicenca.DesejaRealmenteExcluirLicencaDoVeiculo, function () {
        executarReST("VeiculoLicenca/ExcluirPorCodigo", { Codigo: _licencaVeiculo.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Veiculos.VeiculoLicenca.LicencaDoVeiculoExcluidaComSucesso);
                    limparCamposLicencaVeiculo();
                    recarregarGridPesquisaLicenca();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function obterLicencaSalvar() {
    var licenca = RetornarObjetoPesquisa(_licencaVeiculo);

    licenca["FaixaTemperatura"] = obterListaFaixaTemperaturaSalvar(_listaFaixaTemperatura);

    return JSON.stringify(licenca);
}

function recarregarGridPesquisaLicenca() {
    _gridPesquisaLicenca.CarregarGrid();
}

// #endregion Métodos Privados - Cadastro de Licenca de Veículo

// #region Métodos Privados - Cadastro de Veículo

function adicionarLicencaCadastroVeiculo() {
    var veiculoLicenca = obterLicencaCadastroVeiculoSalvar();

    _knoutCadastroVeiculo.Licencas.val().push(veiculoLicenca);

    recarregarGridLicencaCadastroVeiculo();

    Global.fecharModal('divModalLicencaVeiculo');
}

function atualizarLicencaCadastroVeiculo() {
    var veiculoLicenca = obterLicencaCadastroVeiculoSalvar();

    for (var i = 0; i < _knoutCadastroVeiculo.Licencas.val().length; i++) {
        if (_knoutCadastroVeiculo.Licencas.val()[i].Codigo == _licencaVeiculo.Codigo.val()) {
            _knoutCadastroVeiculo.Licencas.val()[i] = veiculoLicenca;
            break;
        }
    }

    recarregarGridLicencaCadastroVeiculo();

    Global.fecharModal('divModalLicencaVeiculo');
}

function editarLicencaCadastroVeiculo(registroSelecionado) {
    for (var i = 0; i < _knoutCadastroVeiculo.Licencas.val().length; i++) {
        var licenca = _knoutCadastroVeiculo.Licencas.val()[i];

        if (licenca.Codigo == registroSelecionado.Codigo) {
            preencherLicencaVeiculo(licenca);
            break;
        }
    }

    exibirModalLicencaCadastroVeiculo(true);
}

function excluirLicencaCadastroVeiculo() {
    for (var i = 0; i < _knoutCadastroVeiculo.Licencas.val().length; i++) {
        if (_knoutCadastroVeiculo.Licencas.val()[i].Codigo == _licencaVeiculo.Codigo.val()) {
            _knoutCadastroVeiculo.Licencas.val().splice(i, 1);
            break;
        }
    }

    recarregarGridLicencaCadastroVeiculo();

    Global.fecharModal('divModalLicencaVeiculo');
}

function exibirModalLicencaCadastroVeiculo(isEdicao) {
    controlarExibicaoBotoesLicencaVeiculo(isEdicao);

    Global.abrirModal('divModalLicencaVeiculo');
    $("#divModalLicencaVeiculo").one("hidden.bs.modal", function () {
        limparCamposLicencaVeiculo();
    });
}

function obterLicencaCadastroVeiculoSalvar() {
    return {
        BloquearCriacaoPedidoLicencaVencida: _licencaVeiculo.BloquearCriacaoPedidoLicencaVencida.val(),
        BloquearCriacaoPlanejamentoPedidoLicencaVencida: _licencaVeiculo.BloquearCriacaoPlanejamentoPedidoLicencaVencida.val(),
        Codigo: _licencaVeiculo.Codigo.val(),
        Descricao: _licencaVeiculo.Descricao.val(),
        DataEmissao: _licencaVeiculo.DataEmissao.val(),
        DataVencimento: _licencaVeiculo.DataVencimento.val(),
        FormaAlerta: _licencaVeiculo.FormaAlerta.val(),
        Numero: _licencaVeiculo.Numero.val(),
        StatusLicenca: _licencaVeiculo.StatusLicenca.val(),
        ClassificacaoRiscoONU: {
            Codigo: _licencaVeiculo.ClassificacaoRiscoONU.codEntity(),
            Descricao: _licencaVeiculo.ClassificacaoRiscoONU.val()
        },
        Licenca: {
            Codigo: _licencaVeiculo.Licenca.codEntity(),
            Descricao: _licencaVeiculo.Licenca.val()
        },
        Veiculo: {
            Codigo: _licencaVeiculo.Veiculo.codEntity(),
            Descricao: _licencaVeiculo.Veiculo.val()
        },
        ListaFaixaTemperatura: _listaFaixaTemperatura,
        Filial: {
            Codigo: _licencaVeiculo.Filial.codEntity(),
            Descricao: _licencaVeiculo.Filial.val()
        },
        TipoCarga: {
            Codigo: _licencaVeiculo.TipoCarga.codEntity(),
            Descricao: _licencaVeiculo.TipoCarga.val()
        },
        Container: {
            Codigo: _licencaVeiculo.Container.codEntity(),
            Descricao: _licencaVeiculo.Container.val()
        }
    }
}

function recarregarGridLicencaCadastroVeiculo() {
    var licencas = new Array();

    for (var i = 0; i < _knoutCadastroVeiculo.Licencas.val().length; i++) {
        var licenca = _knoutCadastroVeiculo.Licencas.val()[i];

        licencas.push({
            Codigo: licenca.Codigo,
            Descricao: licenca.Descricao,
            Numero: licenca.Numero,
            DataEmissao: licenca.DataEmissao,
            DataVencimento: licenca.DataVencimento,
            ClassificacaoRiscoONU: licenca.ClassificacaoRiscoONU.Descricao,
            Licenca: licenca.Licenca.Descricao,
            FaixaTemperatura: obterDescricaoFaixaTemperatura(licenca.ListaFaixaTemperatura),
            StatusLicenca: EnumStatusLicenca.obterDescricao(licenca.StatusLicenca)
        });
    }

    _gridLicencaCadastroVeiculo.CarregarGrid(licencas);
}

// #endregion Métodos Privados - Cadastro de Veículo

// #region Métodos Públicos - Cadastro de Veículo

function adicionarVeiculoLicenca() {
    _licencaVeiculo.Codigo.val(guid());

    exibirModalLicencaCadastroVeiculo(false);
}

function obterListaLicencasSalvar() {
    var licencas = new Array();

    for (var i = 0; i < _knoutCadastroVeiculo.Licencas.val().length; i++) {
        var licenca = _knoutCadastroVeiculo.Licencas.val()[i];

        licencas.push({
            Codigo: licenca.Codigo,
            BloquearCriacaoPedidoLicencaVencida: licenca.BloquearCriacaoPedidoLicencaVencida,
            BloquearCriacaoPlanejamentoPedidoLicencaVencida: licenca.BloquearCriacaoPlanejamentoPedidoLicencaVencida,
            ClassificacaoRiscoONU: licenca.ClassificacaoRiscoONU.Codigo,
            Descricao: licenca.Descricao,
            DataEmissao: licenca.DataEmissao,
            DataVencimento: licenca.DataVencimento,
            FaixaTemperatura: obterListaFaixaTemperaturaSalvar(licenca.ListaFaixaTemperatura),
            FormaAlerta: JSON.stringify(licenca.FormaAlerta),
            Licenca: licenca.Licenca.Codigo,
            Numero: licenca.Numero,
            StatusLicenca: licenca.StatusLicenca,
            Filial: licenca.Filial.Codigo,
            TipoCarga: licenca.TipoCarga.Codigo,
            Container: licenca.Container.Codigo,
        });
    }

    return JSON.stringify(licencas);
}

// #endregion Métodos Públicos - Cadastro de Veículo
