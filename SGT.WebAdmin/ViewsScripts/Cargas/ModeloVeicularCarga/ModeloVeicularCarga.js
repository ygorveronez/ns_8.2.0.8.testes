/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumUnidadeCapacidade.js" />
/// <reference path="../../Consultas/GrupoModeloVeicular.js" />
/// <reference path="../../Consultas/TipoContainer.js" />
/// <reference path="CodigosIntegracao.js" />
/// <reference path="DivisaoCapacidade.js" />
/// <reference path="Eixo.js" />
/// <reference path="GoldenService.js" />
/// <reference path="Pamcard.js" />
/// <reference path="Produto.js" />
/// <reference path="Repom.js" />
/// <reference path="Gadle.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridModeloVeicularCarga;
var _configuracoesModeloVeicularCarga;
var _modeloVeicularCarga;
var _pesquisaModeloVeicularCarga;

var _numeroReboques = [
    { text: "0", value: 0 },
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 }
];

/*
* Declaração das Classes
*/

var PesquisaModeloVeicularCarga = function () {
    this.CapacidadePesoTransporte = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 10, text: Localization.Resources.Cargas.ModeloVeicularCarga.CapacidadeDeCarregamento.getFieldDescription() });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.CodigoDeIntegracao.getFieldDescription(), maxlength: 50, issue: 15 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable(3), def: 3 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridModeloVeicularCarga.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, id: guid(), idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Cargas.ModeloVeicularCarga.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ModeloVeicularCarga = function () {
    var self = this;
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 586, required: true });
    this.CodigoModeloVeicularDeCargaEmbarcador = PropertyEntity({ maxlength: 50, text: Localization.Resources.Cargas.ModeloVeicularCarga.CodigoDeIntegracao.getFieldDescription(), issue: 15 });
    this.LayoutSuperAppId = PropertyEntity({ maxlength: 50, text: Localization.Resources.Cargas.ModeloVeicularCarga.CodigoIntegracaoTrizy.getFieldDescription(), enable: ko.observable(false) });
    this.CodigoIntegracaoGerenciadoraRisco = PropertyEntity({ maxlength: 50, text: Localization.Resources.Cargas.ModeloVeicularCarga.CodigoGerenciadoraDeRisco.getFieldDescription() });

    this.ModeloControlaCubagem = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, eventClick: verificarPossuiPalete, text: Localization.Resources.Cargas.ModeloVeicularCarga.ControlarCapacidadeCubicaDoModelo, def: false });
    this.Cubagem = PropertyEntity({ maxlength: 12, getType: typesKnockout.decimal, text: Localization.Resources.Cargas.ModeloVeicularCarga.CapacidadeCubicaM.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.ToleranciaMinimaCubagem = PropertyEntity({ maxlength: 12, getType: typesKnockout.decimal, text: Localization.Resources.Cargas.ModeloVeicularCarga.TolerânciaMinimaDaCapacidadeCubicaM.getFieldDescription(), required: false });

    this.Altura = PropertyEntity({ maxlength: 12, getType: typesKnockout.decimal, text: Localization.Resources.Cargas.ModeloVeicularCarga.Altura.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.Largura = PropertyEntity({ maxlength: 12, getType: typesKnockout.decimal, text: Localization.Resources.Cargas.ModeloVeicularCarga.Largura.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.Comprimento = PropertyEntity({ maxlength: 12, getType: typesKnockout.decimal, text: Localization.Resources.Cargas.ModeloVeicularCarga.Comprimento.getFieldDescription(), required: false, visible: ko.observable(false) });

    this.CodigosIntegracao = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.DivisoesCapacidade = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.UnidadeCapacidade = PropertyEntity({ val: ko.observable(EnumUnidadeCapacidade.Peso), options: EnumUnidadeCapacidade.obterOpcoes(), def: EnumUnidadeCapacidade.Peso, text: Localization.Resources.Cargas.ModeloVeicularCarga.UnidadeDaCapacidade.getFieldDescription() });
    this.CapacidadePesoTransporte = PropertyEntity({ maxlength: 10, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.ModeloVeicularCarga.CapacidadeDeCarregamento.getRequiredFieldDescription()), required: true });
    this.ToleranciaPesoExtra = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, maxlength: 9, text: Localization.Resources.Cargas.ModeloVeicularCarga.QuantidadeExtraExcedenteTolerado.getRequiredFieldDescription(), required: true });
    this.ToleranciaPesoMenor = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, maxlength: 9, text: Localization.Resources.Cargas.ModeloVeicularCarga.ToleranciaMinimaParaCarregamento.getRequiredFieldDescription(), required: true });
    this.FatorEmissaoCO2 = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, maxlength: 9, text: Localization.Resources.Cargas.ModeloVeicularCarga.FatorDeEmissaoDeCO.getFieldDescription(), required: false });
    this.CodigoTipoCargaANTT = PropertyEntity({ maxlength: 50, text: Localization.Resources.Cargas.ModeloVeicularCarga.CodigoTipoDeCargaParaANTT.getFieldDescription() });
    this.VelocidadeMedia = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.ModeloVeicularCarga.VelocidadeMediaKMH.getFieldDescription() });
    this.QuantidadePaletes = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.ModeloVeicularCarga.QuantidadeDePaletes.getFieldDescription(), visible: ko.observable(false) });
    this.TempoEmissaoFluxoPatio = PropertyEntity({ type: types.map, text: ko.observable(Localization.Resources.Cargas.ModeloVeicularCarga.TempoEmissaoFluxoPatio.getFieldDescription()), val: ko.observable(""), def: "", getType: typesKnockout.int, visible: ko.observable(_CONFIGURACAO_TMS.ExibirTempoEmissaoFluxoPatio), required: ko.observable(true), maxlength: 4 });

    this.VeiculoPaletizado = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, eventClick: verificarPossuiPalete, text: Localization.Resources.Cargas.ModeloVeicularCarga.ModeloPaletizado.getFieldDescription(), def: true });
    this.NumeroPaletes = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.ModeloVeicularCarga.NumeroDePallets.getRequiredFieldDescription(), maxlength: 2, required: true, visible: ko.observable(true) });
    this.ToleranciaMinimaPaletes = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.ModeloVeicularCarga.ToleranciaMinimaDePallets.getRequiredFieldDescription(), maxlength: 2, required: true });
    this.OcupacaoCubicaPaletes = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.ModeloVeicularCarga.OcupacaoCubicaPaletes.getFieldDescription(), maxlength: 10 });

    this.ModeloCalculoFranquia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ModeloVeicularCarga.ModeloDeVeiculoBaseParaCalculoDaFranquia.getFieldDescription(), issue: 2043, required: false, idBtnSearch: guid(), visible: ko.observable(false) });
    this.ContainerTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ModeloVeicularCarga.TipoDoContainer.getFieldDescription(), required: false, idBtnSearch: guid(), visible: _configuracoesModeloVeicularCarga.PossuiTipoContainer });
    this.ExigirDefinicaoReboquePedido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.ModeloVeicularCarga.ExigirDefinicaoDoReboqueNosPedidos, def: false });
    this.ValidarLicencaVeiculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.ModeloVeicularCarga.ValidarLicencaDosVeiculos, def: false });
    this.AlertarOperadorPesoExcederCapacidade = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.ModeloVeicularCarga.AlertarOperadorAntesDaEmissaoDosDocumentosSePesoDaCargaExcederCapacidadeDoVeiculo, def: false });
    this.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoNaCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.ModeloVeicularCarga.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoMotorista, visible: ko.observable(false) });
    this.TipoSemirreboque = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.ModeloVeicularCarga.TipoSemirreboque, visible: ko.observable(false) });
    this.ModeloVeicularAceitaLocalizador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.ModeloVeicularCarga.EsseModeloVeicularAceitaLocalizador, def: false });
    this.NaoSolicitarNoChecklist = PropertyEntity({ val: ko.observable(false), visible: _CONFIGURACAO_TMS.ExibirOpcaoNaoSolicitarNoChecklist, getType: typesKnockout.bool, text: Localization.Resources.Cargas.ModeloVeicularCarga.EsseModeloVeicularNaoDeveSerSolicitadoChecklist, def: false });
    this.ExigirInformacaoLacreJanelaCarregamentoPortalTransportador = PropertyEntity({ val: ko.observable(false), visible: _CONFIGURACAO_TMS.PermitirInformarLacreJanelaCarregamentoTransportador, getType: typesKnockout.bool, text: Localization.Resources.Cargas.ModeloVeicularCarga.ExigirInformacaoLacreJanelaCarregamentoPortalTransportador, def: false });
    this.NumeroEixos = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.ModeloVeicularCarga.NumeroDeEixos.getRequiredFieldDescription(), issue: 828, maxlength: 2, configInt: { precision: 0, allowZero: true }, required: true, visible: ko.observable(true) });
    this.NumeroEixosSuspensos = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.ModeloVeicularCarga.NumeroDeEixosSuspensos.getRequiredFieldDescription(), issue: 0, maxlength: 2, configInt: { precision: 0, allowZero: true }, required: false, visible: ko.observable(true) });
    this.PadraoEixos = PropertyEntity({ val: ko.observable(EnumPadraoEixosVeiculo.Duplo), options: EnumPadraoEixosVeiculo.obterOpcoes(), def: EnumPadraoEixosVeiculo.Duplo, getType: typesKnockout.int, text: Localization.Resources.Cargas.ModeloVeicularCarga.PadraoDeEixos.getFieldDescription() });
    this.PadraoEixos.visible = ko.computed(function () {
        if (self.NumeroEixos.val() < 3) {
            return true;
        }

        self.PadraoEixos.val(null);
        return false;
    });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557 });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoModeloVeicularCarga.Geral), options: EnumTipoModeloVeicularCarga.obterOpcoes(), def: EnumTipoModeloVeicularCarga.Geral, text: Localization.Resources.Gerais.Geral.Tipo.getRequiredFieldDescription(), issue: 43 });
    this.NumeroReboques = PropertyEntity({ val: ko.observable(0), options: _numeroReboques, def: 0, text: Localization.Resources.Cargas.ModeloVeicularCarga.NumeroReboques.getRequiredFieldDescription() });
    this.DiasRealizarProximoChecklist = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.ModeloVeicularCarga.DiasProximoChecklist.getFieldDescription(), maxlength: 3, configInt: { precision: 0, allowZero: true, thousands: "" } });
    this.Grupo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ModeloVeicularCarga.Grupo.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });

    this.ArrobaMinima = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.ArrobaMinima.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.ArrobaMaxima = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.ArrobaMaxima.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.CabecaMinima = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.CabecaMinima.getFieldDescription(), getType: typesKnockout.int, maxlength: 11 });
    this.CabecaMaxima = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.CabecaMaxima.getFieldDescription(), getType: typesKnockout.int, maxlength: 11 });

    // Temporário. O que funciona mesmo é o do DivisaoCapacidade
    this.ValidarCapacidadeMaximaNoApp = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Tipo.val.subscribe(tipoModeloVeicularCargaChange);
    this.Altura.val.subscribe(calcularMetrosCubicos);
    this.Largura.val.subscribe(calcularMetrosCubicos);
    this.Comprimento.val.subscribe(calcularMetrosCubicos);
};

var CRUDModeloVeicularCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadConfiguracoesModeloVeicularCarga(callback) {
    executarReST("ModeloVeicularCarga/ObterConfiguracoes", {}, function (retorno) {
        if (retorno.Success) {
            _configuracoesModeloVeicularCarga = retorno.Data;
            callback();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function loadModeloVeicularCarga() {
    loadConfiguracoesModeloVeicularCarga(function () {
        _modeloVeicularCarga = new ModeloVeicularCarga();
        KoBindings(_modeloVeicularCarga, "knockoutCadastroModeloVeicularCarga");

        _CRUDModeloVeicularCarga = new CRUDModeloVeicularCarga();
        KoBindings(_CRUDModeloVeicularCarga, "knockoutCRUDModeloVeicularCarga");

        _pesquisaModeloVeicularCarga = new PesquisaModeloVeicularCarga();
        KoBindings(_pesquisaModeloVeicularCarga, "knockoutPesquisaModeloVeicularCarga", false, _pesquisaModeloVeicularCarga.Pesquisar.id);

        HeaderAuditoria("ModeloVeicularCarga", _modeloVeicularCarga);

        new BuscarModelosVeicularesCarga(_modeloVeicularCarga.ModeloCalculoFranquia);
        new BuscarTiposContainer(_modeloVeicularCarga.ContainerTipo);
        new BuscarGrupoModeloVeicular(_modeloVeicularCarga.Grupo, Localization.Resources.Cargas.ModeloVeicularCarga.BuscarGrupoModeloVeicular, Localization.Resources.Cargas.ModeloVeicularCarga.BuscarGrupoModeloVeicular);

        if (_CONFIGURACAO_TMS.ObrigatorioInformarDadosContratoFrete)
            _modeloVeicularCarga.ModeloCalculoFranquia.visible(true);
        if (_CONFIGURACAO_TMS.ExibirInformacoesBovinos)
            $("#liExibirInformacoesBovinos").show();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
            _modeloVeicularCarga.Grupo.visible(false);
            $("#liGoldenService").hide();
        } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
            $("#liDivisaoCapacidade").show();
        }

        if (_CONFIGURACAO_TMS.UtilizarControlePaletesPorModeloVeicular)
            _modeloVeicularCarga.QuantidadePaletes.visible(true);

        $("#" + _modeloVeicularCarga.VeiculoPaletizado.id).click(verificarPossuiPalete);
        $("#" + _modeloVeicularCarga.ModeloControlaCubagem.id).click(verificarPossuiCubagem);

        loadEixo();
        loadCodigoIntegracao();
        LoadDivisaoCapacidade();
        loadModeloVeicularCargaProduto();
        loadModeloVeicularCargaGrupoProduto();
        buscarModeloVeicularCarga();
        configurarIntegracoesDisponiveis();
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (!ValidarCamposObrigatorios(_modeloVeicularCarga)) {
        exibirMensagemCamposObrigatorio();
        return;
    }

    executarReST("ModeloVeicularCarga/Adicionar", obterModeloVeicularCargaSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridModeloVeicularCarga.CarregarGrid();
                limparCamposModeloVeicularCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function atualizarClick() {
    if (!ValidarCamposObrigatorios(_modeloVeicularCarga)) {
        exibirMensagemCamposObrigatorio();
        return;
    }

    executarReST("ModeloVeicularCarga/Atualizar", obterModeloVeicularCargaSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridModeloVeicularCarga.CarregarGrid();
                limparCamposModeloVeicularCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function cancelarClick() {
    if (isNumeroEixosAlterado())
        _gridModeloVeicularCarga.CarregarGrid();

    limparCamposModeloVeicularCarga();
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ModeloVeicularCarga.RealmenteDesejaExcluirModeloVeicularDeCarga + _modeloVeicularCarga.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_modeloVeicularCarga, "ModeloVeicularCarga/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridModeloVeicularCarga.CarregarGrid();
                    limparCamposModeloVeicularCarga();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function calcularMetrosCubicos() {
    var altura = Globalize.parseFloat(_modeloVeicularCarga.Altura.val());
    var largura = Globalize.parseFloat(_modeloVeicularCarga.Largura.val());
    var comprimento = Globalize.parseFloat(_modeloVeicularCarga.Comprimento.val());

    if (isNaN(altura))
        altura = 0;
    if (isNaN(largura))
        largura = 0;
    if (isNaN(comprimento))
        comprimento = 0;

    if (comprimento > 0 && largura > 0 && comprimento > 0)
        _modeloVeicularCarga.Cubagem.val(Globalize.format((altura * largura * comprimento), "n2"));
}

function tipoModeloVeicularCargaChange() {
    _modeloVeicularCarga.CapacidadePesoTransporte.required = (_modeloVeicularCarga.Tipo.val() == EnumTipoModeloVeicularCarga.Geral) || (_modeloVeicularCarga.Tipo.val() == EnumTipoModeloVeicularCarga.Reboque);
    _modeloVeicularCarga.CapacidadePesoTransporte.text((_modeloVeicularCarga.CapacidadePesoTransporte.required ? "*" : "") + Localization.Resources.Cargas.ModeloVeicularCarga.CapacidadeDeCarregamento.getFieldDescription());
    _modeloVeicularCarga.TipoSemirreboque.visible(_modeloVeicularCarga.Tipo.val() == EnumTipoModeloVeicularCarga.Reboque);
    _modeloVeicularCarga.TipoSemirreboque.val(false);
}

/*
 * Declaração das Funções Públicas
 */

function verificarPossuiCubagem(e) {
    if (_modeloVeicularCarga.ModeloControlaCubagem.val()) {
        _modeloVeicularCarga.Altura.visible(true);
        _modeloVeicularCarga.Largura.visible(true);
        _modeloVeicularCarga.Comprimento.visible(true);
        _modeloVeicularCarga.Cubagem.visible(true);
        _modeloVeicularCarga.Cubagem.required = true;
        _modeloVeicularCarga.ToleranciaMinimaCubagem.required = true;
    } else {
        _modeloVeicularCarga.Altura.visible(false);
        _modeloVeicularCarga.Largura.visible(false);
        _modeloVeicularCarga.Comprimento.visible(false);
        _modeloVeicularCarga.Cubagem.visible(false);
        _modeloVeicularCarga.Cubagem.required = false;
        _modeloVeicularCarga.ToleranciaMinimaCubagem.required = false;
    }
}

function verificarPossuiPalete(e) {
    if (_modeloVeicularCarga.VeiculoPaletizado.val()) {
        _modeloVeicularCarga.NumeroPaletes.visible(true);
        _modeloVeicularCarga.NumeroPaletes.required = true;
        _modeloVeicularCarga.ToleranciaMinimaPaletes.required = true;
    }
    else {
        _modeloVeicularCarga.NumeroPaletes.visible(false);
        _modeloVeicularCarga.NumeroPaletes.required = false;
        _modeloVeicularCarga.ToleranciaMinimaPaletes.required = false;
    }
}

/*
 * Declaração das Funções Privadas
 */

function buscarModeloVeicularCarga() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarModeloVeicularCarga, tamanho: 15, icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var downloadModeloVeicular = {
        url: "ModeloVeicularCarga/Exportar",
        titulo: "Modelos Veículares de Carga"
    };

    _gridModeloVeicularCarga = new GridViewExportacao(_pesquisaModeloVeicularCarga.Pesquisar.idGrid, "ModeloVeicularCarga/Pesquisa", _pesquisaModeloVeicularCarga, menuOpcoes, downloadModeloVeicular, null, 10);
    _gridModeloVeicularCarga.CarregarGrid();
}

function configurarIntegracoesDisponiveis() {
    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (r) {
        if (r.Success && r.Data) {
            if (r.Data.TiposExistentes != null && r.Data.TiposExistentes.length > 0) {
                if (r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.GoldenService; }) && r.Data.PossuiIntegracaoGoldenService) {
                    LoadConfiguracaoGoldenService();
                    $("#liGoldenService").removeClass("d-none");
                }
                if (r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.A52; })) {
                    LoadConfiguracaoA52();
                    $("#liA52").removeClass("d-none");
                }

                if (r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Gadle; })) {
                    LoadConfiguracaoGadle();
                    $("#liGadle").removeClass("d-none");
                }

                if (r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Pamcard; })) {
                    LoadConfiguracaoPamcard();
                    $("#liPamcard").removeClass("d-none");
                }

                if (r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.BrasilRisk; }))
                    _modeloVeicularCarga.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoNaCarga.visible(true);
            }

            if (r.Data.OperadorasCIOTExistentes != null && r.Data.OperadorasCIOTExistentes.length > 0) {
                if (r.Data.OperadorasCIOTExistentes.some(function (o) { return o == EnumOperadoraCIOT.Repom || o == EnumOperadoraCIOT.RepomFrete; })) {
                    LoadConfiguracaoRepom();
                    $("#liRepom").removeClass("d-none");
                }
                if (r.Data.OperadorasCIOTExistentes.some(function (o) { return o == EnumOperadoraCIOT.Pamcard; }) || r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Pamcard; })) {
                    LoadConfiguracaoPamcard();
                    $("#liPamcard").removeClass("d-none");
                }
                if (r.Data.OperadorasCIOTExistentes.some(function (o) { return o == EnumOperadoraCIOT.Target; })) {
                    LoadConfiguracaoTarget();
                    $("#liTarget").removeClass("d-none");
                }
            }
        }
    });
}

function editarModeloVeicularCarga(modeloVeicularCargaGrid) {
    limparCamposModeloVeicularCarga();

    executarReST("ModeloVeicularCarga/BuscarPorCodigo", { Codigo: modeloVeicularCargaGrid.Codigo }, function (retorno) {
        if (retorno.Success) {
            PreencherObjetoKnout(_modeloVeicularCarga, retorno);

            RecarregarGridCodigoIntegracao();
            RecarregarGridDivisaoCapacidade();

            _pesquisaModeloVeicularCarga.ExibirFiltros.visibleFade(false);
            _CRUDModeloVeicularCarga.Atualizar.visible(true);
            _CRUDModeloVeicularCarga.Cancelar.visible(true);
            _CRUDModeloVeicularCarga.Excluir.visible(true);
            _CRUDModeloVeicularCarga.Adicionar.visible(false);

            preencherEixo(retorno.Data.Eixo);
            preencherModeloVeicularCargaProduto(retorno.Data.Produtos);
            preencherModeloVeicularCargaGrupoProduto(retorno.Data.GruposProdutos);

            verificarPossuiPalete();
            verificarPossuiCubagem();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.ModeloVeicularCarga.PorFavorInformeOsCamposObrigatorios);
}

function limparCamposModeloVeicularCarga() {
    _CRUDModeloVeicularCarga.Atualizar.visible(false);
    _CRUDModeloVeicularCarga.Cancelar.visible(false);
    _CRUDModeloVeicularCarga.Excluir.visible(false);
    _CRUDModeloVeicularCarga.Adicionar.visible(true);

    _modeloVeicularCarga.NumeroPaletes.visible(true);
    _modeloVeicularCarga.NumeroPaletes.required = true;
    _modeloVeicularCarga.ToleranciaMinimaPaletes.required = true;
    _modeloVeicularCarga.TipoSemirreboque.visible(false);

    _modeloVeicularCarga.Altura.visible(false);
    _modeloVeicularCarga.Largura.visible(false);
    _modeloVeicularCarga.Comprimento.visible(false);
    _modeloVeicularCarga.Cubagem.visible(false);
    _modeloVeicularCarga.Cubagem.required = false;
    _modeloVeicularCarga.ToleranciaMinimaCubagem.required = false;
    _modeloVeicularCarga.ModeloControlaCubagem.val(false);

    limparEixo();
    LimparCamposCodigoIntegracao();
    LimparCampos(_modeloVeicularCarga);
    LimparCamposDivisaoCapacidade();
    RecarregarGridDivisaoCapacidade();
    limparCamposModeloVeicularCargaProduto();
    limparCamposModeloVeicularCargaGrupoProduto();

    Global.ResetarAbas();
}

function obterModeloVeicularCargaSalvar() {
    var modeloVeicularCarga = RetornarObjetoPesquisa(_modeloVeicularCarga);

    modeloVeicularCarga["Produtos"] = obterModeloVeicularCargaProdutoSalvar();
    modeloVeicularCarga["GruposProdutos"] = obterModeloVeicularCargaGrupoProdutoSalvar();
    modeloVeicularCarga["ValidarCapacidadeMaximaNoApp"] = _divisaoCapacidade.ValidarCapacidadeMaximaNoApp.val();

    return modeloVeicularCarga;
}
