/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="Transportador.js" />
/// <reference path="../../Consultas/NaturezaNFSe.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/ServicoNFSe.js" />
/// <reference path="../../Consultas/SerieTransportador.js" />
/// <reference path="../../Enumeradores/EnumExigibilidadeISS.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoOcorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _exigibilidadeISS = [
    { text: "Exigível", value: EnumExigibilidadeISS.Exigivel },
    { text: "Não incidência", value: EnumExigibilidadeISS.NaoInicidencia },
    { text: "Isenção", value: EnumExigibilidadeISS.Isencao },
    { text: "Exportação", value: EnumExigibilidadeISS.Exportacao },
    { text: "Imunidade", value: EnumExigibilidadeISS.Imunidade },
    { text: "Suspensa por Decisão Judicial", value: EnumExigibilidadeISS.SuspensaDecisaoJudicial },
    { text: "Suspensa por Processo Administrativo", value: EnumExigibilidadeISS.SuspensaProcessoAdministrativo },
    { text: "Não Informado", value: EnumExigibilidadeISS.NaoInformado }
];

var _gridTransportadorConfiguracaoNFSe;
var _transportadorConfiguracaoNFSe;

var TransportadorPesquisaConfiguracaoNFSe = function () {
    this.Empresa = PropertyEntity({ val: ko.observable(0), def: 0, type: types.entity, codEntity: ko.observable(0) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Transportadores.Transportador.Servico.getFieldDescription(), issue: 768, idBtnSearch: guid() });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Transportadores.Transportador.LocalidadePrestacaoServico.getFieldDescription(), issue: 766, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Transportadores.Transportador.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.NBS = PropertyEntity({ text: "NBS: ", maxlength: 9 });

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGrid, type: types.event, text: "Pesquisar", visible: ko.observable(true) });
}

var TransportadorConfiguracaoNFSe = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Empresa = PropertyEntity({ val: ko.observable(0), def: 0, type: types.entity, codEntity: ko.observable(0) });

    this.AliquotaISS = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.AliquotaISS.getRequiredFieldDescription(), issue: 770, required: true, getType: typesKnockout.decimal, def: "", enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true, allowNegative: false } });
    this.FraseSecreta = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Frase.getFieldDescription(), required: false, maxlength: 200 });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Transportadores.Transportador.LocalidadePrestacaoServico.getFieldDescription(), issue: 766, idBtnSearch: guid() });
    this.LoginSitePrefeitura = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.LoginSitePrefeitura.getFieldDescription(), issue: 772, required: false, maxlength: 500 });
    this.NaturezaNFSe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Transportadores.Transportador.Natureza.getRequiredFieldDescription(), issue: 769, idBtnSearch: guid() });
    this.ObservacaoIntegracao = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ObservacaoIntegracaoNFSe.getFieldDescription(), issue: 773, required: false, maxlength: 2000 });
    this.RetencaoISS = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.RetencaoISS.getRequiredFieldDescription()), issue: 771, maxlength: 6, required: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.decimal, def: "", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ClienteTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Transportadores.Transportador.ClienteTomador.getFieldDescription(), idBtnSearch: guid() });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Transportadores.Transportador.GrupoTomador.getFieldDescription(), idBtnSearch: guid() });
    this.LocalidadeTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Transportadores.Transportador.LocalidadeTomador.getFieldDescription(), issue: 766, idBtnSearch: guid() });
    this.UFTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Transportadores.Transportador.UFTomador.getFieldDescription(), idBtnSearch: guid() });
    this.SenhaSitePrefeitura = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.SenhaSitePrefeitura.getFieldDescription(), issue: 772, required: false, maxlength: 500 });
    this.SerieNFSe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Transportadores.Transportador.Serie.getRequiredFieldDescription(), issue: 756, idBtnSearch: guid() });
    this.SerieRPS = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.SerieRPS.getFieldDescription(), issue: 767, required: false, maxlength: 10 });
    this.ServicoNFSe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Transportadores.Transportador.Servico.getRequiredFieldDescription(), issue: 768, idBtnSearch: guid() });
    this.URLPrefeitura = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.SitePrefeitura.getFieldDescription(), issue: 772, required: false, maxlength: 500 });
    this.ExigibilidadeISS = PropertyEntity({ val: ko.observable(EnumExigibilidadeISS.NaoInformado), def: EnumExigibilidadeISS.NaoInformado, options: _exigibilidadeISS, text: Localization.Resources.Transportadores.Transportador.ExigibilidadeISS.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.IncluirISSBaseCalculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.IncluirValorISSBaseCalculo.getFieldDescription() });
    this.NBS = PropertyEntity({ text: "NBS: ", maxlength: 9 });

    this.RealizarArredondamentoCalculoIss = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: 'Realizar arredondamento do cálculo do ISS' });

    this.IncidenciaISSLocalidadePrestador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.IncidenciaISSLocalidadePrestador.getFieldDescription() });
    this.PermiteAnular = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.PermiteAnularNFSe, issue: 1570 });
    this.ReterIReDestacarNFs = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.ReterIReDestacarNFs });
    this.ConfiguracaoParaProvisaoDeISS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.DadosParaProvisaoISSNaoraEmitirNFSeAutomaticamenteCargaNFSeDeveraInformadaManualmenteDepois, visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? true : false });
    this.PrazoCancelamento = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, def: "", text: Localization.Resources.Transportadores.Transportador.PrazoParaCancelamentoDias.getRequiredFieldDescription() });
    this.DiscriminacaoNFSe = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DiscriminacaoNFSe.getFieldDescription(), required: false, maxlength: 2000 });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Transportadores.Transportador.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });

    this.NaoEnviarAliquotaEValorISS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.NaoEnviarAliquotaEValorISS.getFieldDescription() });

    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.TipoDaOcorrencia.getFieldDescription(), idBtnSearch: guid() });

    this.AliquotaIR = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.AliquotaIR.getRequiredFieldDescription()), maxlength: 6, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.decimal, def: "", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.BaseCalculoIR = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.BaseCalculoIR.getRequiredFieldDescription()), maxlength: 6, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.decimal, def: "", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });

    this.ReterIReDestacarNFs.val.subscribe(function (novoValor) {
        if (novoValor) {
            _transportadorConfiguracaoNFSe.AliquotaIR.visible(true);
            _transportadorConfiguracaoNFSe.BaseCalculoIR.visible(true);
            _transportadorConfiguracaoNFSe.AliquotaIR.required(true);
            _transportadorConfiguracaoNFSe.BaseCalculoIR.required(true);
        } else {
            _transportadorConfiguracaoNFSe.AliquotaIR.visible(false);
            _transportadorConfiguracaoNFSe.BaseCalculoIR.visible(false);
            _transportadorConfiguracaoNFSe.AliquotaIR.required(false);
            _transportadorConfiguracaoNFSe.BaseCalculoIR.required(false);
            LimparCampo(_transportadorConfiguracaoNFSe.AliquotaIR);
            LimparCampo(_transportadorConfiguracaoNFSe.BaseCalculoIR);
        }
    });

    this.TagCodigoDestinatario = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#CodigoDestinatario"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.CodigoDestinatario });
    this.TagCodigoRemetente = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#CodigoRemetente"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.CodigoRemetente });
    this.TagDataCarga = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#DataCarga"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.DataCarga });
    this.TagDestino = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#Destino"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.Destino });
    this.TagOrigem = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#Origem"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.Origem });
    this.TagNotasFiscais = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NotasFiscais"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.NotasFiscais });
    this.TagTipoCarga = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#TipoCarga"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.TipoCarga });
    this.TagPesoCarga = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#PesoCarga"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.PesoCarga });
    this.TagCnpjRemetente = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#CnpjRemetente"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.CnpjRemetente });
    this.TagNomeRemetente = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NomeRemetente"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.NomeRemetente });
    this.TagCnpjDestinatario = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#CnpjDestinatario"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.CnpjDestinatario });
    this.TagNomeDestinatario = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NomeDestinatario"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.NomeDestinatario });
    this.TagValorMercadoria = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#ValorMercadoria"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.ValorMercadoria });
    this.TagPlacaVeiculo = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#PlacaVeiculo"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.PlacaVeiculo });
    this.TagNomeMotorista = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NomeMotorista"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.NomeMotorista });
    this.TagNumeroCarga = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NumeroCarga"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.NumeroDaCarga });
    this.TagNumeroPedidoEmbarcador = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NumeroPedidoEmbarcador"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.NumeroPedidoEmbarcador });
    this.TagCPFMotorista = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#CPFMotorista"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.CPFMotorista });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoNFSeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarConfiguracaoNFSeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirConfiguracaoNFSeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarConfiguracaoNFSeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "Transportador/ImportarISS",
        UrlConfiguracao: "Transportador/ConfiguracaoImportacaoISS",
        CodigoControleImportacao: EnumCodigoControleImportacao.O054_ImportacaoDadosTransporte,
        CallbackImportacao: function () {
            _gridTransportador.CarregarGrid();
        }
    });
};

//*******EVENTOS*******

function loadTransportadorConfiguracaoNFSe() {

    _transportadorPesquisaConfiguracaoNFSe = new TransportadorPesquisaConfiguracaoNFSe();
    KoBindings(_transportadorPesquisaConfiguracaoNFSe, "knockoutPesquisaConfiguracaoNFSe");

    _transportadorConfiguracaoNFSe = new TransportadorConfiguracaoNFSe();
    KoBindings(_transportadorConfiguracaoNFSe, "knoutConfiguracaoNFSe");

    new BuscarLocalidades(_transportadorConfiguracaoNFSe.LocalidadePrestacao, null, null, ValidarLocalidadeSelecionada);
    new BuscarLocalidades(_transportadorConfiguracaoNFSe.LocalidadeTomador);
    new BuscarNaturezaNFSe(_transportadorConfiguracaoNFSe.NaturezaNFSe, _transportador.Localidade);
    new BuscarServicoNFSe(_transportadorConfiguracaoNFSe.ServicoNFSe, _transportador.Localidade);
    new BuscarSeriesNFSeTransportador(_transportadorConfiguracaoNFSe.SerieNFSe, null, null, null, null, _transportadorConfiguracaoNFSe.Empresa);
    new BuscarClientes(_transportadorConfiguracaoNFSe.ClienteTomador);
    new BuscarGruposPessoas(_transportadorConfiguracaoNFSe.GrupoTomador);
    new BuscarEstados(_transportadorConfiguracaoNFSe.UFTomador);
    new BuscarTiposOperacao(_transportadorConfiguracaoNFSe.TipoOperacao);
    new BuscarTipoOcorrencia(_transportadorConfiguracaoNFSe.TipoOcorrencia);

    new BuscarServicoNFSe(_transportadorPesquisaConfiguracaoNFSe.Servico, _transportador.Localidade);
    new BuscarLocalidades(_transportadorPesquisaConfiguracaoNFSe.LocalidadePrestacao);
    new BuscarTiposOperacao(_transportadorPesquisaConfiguracaoNFSe.TipoOperacao);

    buscarTransportadorConfiguracaoNFSes();
}

function adicionarConfiguracaoNFSeClick(e, sender) {
    Salvar(e, "TransportadorConfiguracaoNFSe/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Transportadores.Transportador.Cadastrado);
                _gridTransportadorConfiguracaoNFSe.CarregarGrid();
                limparCamposTransportadorConfiguracaoNFSe();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarConfiguracaoNFSeClick(e, sender) {
    Salvar(e, "TransportadorConfiguracaoNFSe/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridTransportadorConfiguracaoNFSe.CarregarGrid();
                limparCamposTransportadorConfiguracaoNFSe();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function excluirConfiguracaoNFSeClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Transportador.RealmenteDesejaExcluirConfiguracaoNFSe, function () {
        ExcluirPorCodigo(_transportadorConfiguracaoNFSe, "TransportadorConfiguracaoNFSe/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridTransportadorConfiguracaoNFSe.CarregarGrid();
                    limparCamposTransportadorConfiguracaoNFSe();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarConfiguracaoNFSeClick(e) {
    limparCamposTransportadorConfiguracaoNFSe();
}

//*******MÉTODOS*******

function recarregarGrid() {
    _gridTransportadorConfiguracaoNFSe.CarregarGrid();
}

function buscarTransportadorConfiguracaoNFSes() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarTransportadorConfiguracaoNFSe, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    let configExportacao = {
        url: "TransportadorConfiguracaoNFSe/ExportarPlanilhaISS",
        titulo: "ConfiguraçãoNFSe"
    };
    _gridTransportadorConfiguracaoNFSe = new GridViewExportacao(_transportadorConfiguracaoNFSe.Empresa.id, "TransportadorConfiguracaoNFSe/Pesquisa", _transportadorPesquisaConfiguracaoNFSe, menuOpcoes, configExportacao);
}

function editarTransportadorConfiguracaoNFSe(transportadorConfiguracaoNFSeGrid) {
    limparCamposTransportadorConfiguracaoNFSe();
    _transportadorConfiguracaoNFSe.Codigo.val(transportadorConfiguracaoNFSeGrid.Codigo);
    BuscarPorCodigo(_transportadorConfiguracaoNFSe, "TransportadorConfiguracaoNFSe/BuscarPorCodigo", function (arg) {
        _transportadorConfiguracaoNFSe.Atualizar.visible(true);
        _transportadorConfiguracaoNFSe.Cancelar.visible(true);
        _transportadorConfiguracaoNFSe.Excluir.visible(true);
        _transportadorConfiguracaoNFSe.Adicionar.visible(false);
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _CONFIGURACAO_TMS.ExigirRetencaoISSQuandoMunicipioPrestacaoForDiferenteTransportador) {
            _transportadorConfiguracaoNFSe.AliquotaISS.enable(false);
        }
    }, null);
}

function limparCamposTransportadorConfiguracaoNFSe() {
    _transportadorConfiguracaoNFSe.Atualizar.visible(false);
    _transportadorConfiguracaoNFSe.Cancelar.visible(false);
    _transportadorConfiguracaoNFSe.Excluir.visible(false);
    _transportadorConfiguracaoNFSe.Adicionar.visible(true);
    LimparCampos(_transportadorConfiguracaoNFSe);
    _transportadorConfiguracaoNFSe.Empresa.val(_transportador.Codigo.val());
    _transportadorConfiguracaoNFSe.Empresa.codEntity(_transportador.Codigo.val());
    _transportadorPesquisaConfiguracaoNFSe.Empresa.val(_transportador.Codigo.val());
    _transportadorPesquisaConfiguracaoNFSe.Empresa.codEntity(_transportador.Codigo.val());
}

function alterarEstadoCadastroConfiguracaoNFSe() {
    if (_transportador.Codigo.val() > 0) {
        _transportadorConfiguracaoNFSe.Empresa.val(_transportador.Codigo.val());
        _transportadorConfiguracaoNFSe.Empresa.codEntity(_transportador.Codigo.val());
        _transportadorPesquisaConfiguracaoNFSe.Empresa.val(_transportador.Codigo.val());
        _transportadorPesquisaConfiguracaoNFSe.Empresa.codEntity(_transportador.Codigo.val());
        _gridTransportadorConfiguracaoNFSe.CarregarGrid();

        $("#liTabConfiguracaoNFSe").removeClass("d-none");

    } else {
        $("#liTabConfiguracaoNFSe").addClass("d-none");
    }
}

function ValidarLocalidadeSelecionada(dadosRetorno) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _CONFIGURACAO_TMS.ExigirRetencaoISSQuandoMunicipioPrestacaoForDiferenteTransportador) {
        executarReST("TransportadorConfiguracaoNFSe/ValidarLocalidadeSelecionada", dadosRetorno, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _transportadorConfiguracaoNFSe.LocalidadePrestacao.codEntity(dadosRetorno.Codigo);
                    _transportadorConfiguracaoNFSe.LocalidadePrestacao.val(dadosRetorno.Descricao);
                    if (arg.Data.Existe) {
                        if (arg.Data.TipoUnilever)
                            verificarRetencaoISSUnilever(arg.Data);
                        else
                            verificarRetencaoISS(arg.Data);
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
    else {
        _transportadorConfiguracaoNFSe.LocalidadePrestacao.codEntity(dadosRetorno.Codigo);
        _transportadorConfiguracaoNFSe.LocalidadePrestacao.val(dadosRetorno.Descricao);
    }
}

function verificarRetencaoISS(data) {
    if (data.Aliquota != "") {
        _transportadorConfiguracaoNFSe.AliquotaISS.val(data.Aliquota);
        _transportadorConfiguracaoNFSe.AliquotaISS.enable(false);
    } else {
        _transportadorConfiguracaoNFSe.AliquotaISS.enable(true);
    }
    if (_transportador.Localidade.codEntity() == data.CodigoLocalidadeNFSe) {
        if (data.RetemISS) {
            _transportadorConfiguracaoNFSe.RetencaoISS.required(true);
            _transportadorConfiguracaoNFSe.RetencaoISS.enable(true);
        }
        else {
            _transportadorConfiguracaoNFSe.RetencaoISS.required(false);
            _transportadorConfiguracaoNFSe.RetencaoISS.enable(false);
            _transportadorConfiguracaoNFSe.RetencaoISS.text(Localization.Resources.Transportadores.Transportador.RetencaoISS.getFieldDescription());
        }
    } else if (_transportador.Localidade.codEntity() != data.CodigoLocalidadeNFSe) {
        _transportadorConfiguracaoNFSe.RetencaoISS.required(true);
    } else {
        _transportadorConfiguracaoNFSe.RetencaoISS.required(false);
    }
}

function verificarRetencaoISSUnilever(data) {

    let aliquotaISS = _transportadorConfiguracaoNFSe.AliquotaISS;
    let retencaoISS = _transportadorConfiguracaoNFSe.RetencaoISS;

    retencaoISS.enable(false);
    aliquotaISS.enable(false);

    if (_transportador.Localidade.codEntity() != data.CodigoLocalidadeNFSe) {
        retencaoISS.val("100,00");
        aliquotaISS.val(data.Aliquota);
    }
    if (_transportador.Localidade.codEntity() == data.CodigoLocalidadeNFSe) {
        if (data.RetemISS) {
            retencaoISS.val("100,00")
            aliquotaISS.val(data.Aliquota)
        }
        else {
            retencaoISS.val("0,00")
            aliquotaISS.val(data.Aliquota)
        }
    }
}