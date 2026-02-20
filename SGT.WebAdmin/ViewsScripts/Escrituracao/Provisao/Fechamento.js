/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ImpostoValorAgregado.js" />
/// <reference path="../../Enumeradores/EnumSituacaoProvisao.js" />

// #region Objetos Globais do Arquivo

var _percentualFechamentoProvisao;
var _fechamentoProvisao;
var _detalhamentoRateio;
var _fechamentoProvisaoDefinirIVA;
var _CRUDFechamentoProvisao;
var _gridDocumentosContabeis;
var _gridDetalhamentoRateio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PercentualFechamentoProvisao = function () {
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var FechamentoProvisao = function () {
    let dataAtual = moment().format("DD/MM/YYYY");

    this.Contabilizacao = PropertyEntity({ text: "Resumo Provisão: ", idGrid: guid() });
    this.DataLancamento = PropertyEntity({ text: "Data do Lançamento: ", getType: typesKnockout.date, visible: ko.observable(true), val: ko.observable(dataAtual), def: dataAtual, enable: false });
    this.PossuiStage = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.DocumentosContabeisPorStage = ko.observableArray([]);
};

var DetalhamentoRateio = function () {
    this.DetalhamentoRateio = PropertyEntity({ idGrid: guid(), visible: ko.observable(_CONFIGURACAO_TMS.RateioProvisaoPorGrupoProduto) });  

    if (this.DetalhamentoRateio.visible()) {
        $("#tabDetalhamentoNav").show();
    } else {
        $("#tabDetalhamentoNav").hide();
    }
};

var FechamentoProvisaoDefinirIVA = function () {
    this.DocumentoContabilPorStage = null;
    this.CodigoProvisao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoStage = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ImpostoValorAgregado = PropertyEntity({ text: "*Imposto sobre Valor Agregado:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });

    this.Definir = PropertyEntity({ eventClick: definirIVAClick, type: types.event, text: "Definir", enable: ko.observable(true) });
}

var CRUDFechamentoProvisao = function () {
    this.ConfirmarProvisao = PropertyEntity({ eventClick: confirmarProvisaoClick, type: types.event, text: "Confirmar Fechamento", idGrid: guid(), visible: ko.observable(false) });
    this.ProcessarNovamente = PropertyEntity({ eventClick: reprocessarProvisaoClick, type: types.event, text: "Reprocessar Provisão", idGrid: guid(), visible: ko.observable(false) });
    this.CancelarProvisao = PropertyEntity({ eventClick: cancelarFechamentoProvisaoClick, type: types.event, text: "Remover Provisão", idGrid: guid(), visible: ko.observable(true) });
};

var DocumentoContabilPorStage = function (documentoContabilPorStage) {
    this.CodigoStage = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroStage = PropertyEntity({ text: "Stage" });
    this.IVA = PropertyEntity({ text: "IVA:" });
    this.AdValorem = PropertyEntity({ text: "Ad Valorem:" });
    this.AliquotaCofins = PropertyEntity({ text: "Alíquota Cofins:" });
    this.AliquotaPis = PropertyEntity({ text: "Alíquota PIS:" });
    this.AliquotaIcms = PropertyEntity({ text: "Alíquota ICMS:" });
    this.AliquotaIss = PropertyEntity({ text: "Alíquota ISS:" });
    this.Cofins = PropertyEntity({ text: "Cofins:" });
    this.Icms = PropertyEntity({ text: "ICMS:" });
    this.IcmsST = PropertyEntity({ text: "ICMS ST:" });
    this.Iss = PropertyEntity({ text: "ISS:" });
    this.IssRetido = PropertyEntity({ text: "ISS Retido:" });
    this.FreteLiquido = PropertyEntity({ text: "Frete Líquido:" });
    this.CustoFixo = PropertyEntity({ text: "Custo Fixo:" });
    this.FreteCaixa = PropertyEntity({ text: "Frete Caixa:" });
    this.FreteKM = PropertyEntity({ text: "Frete KM:" });
    this.FretePeso = PropertyEntity({ text: "Frete Peso:" });
    this.FreteViagem = PropertyEntity({ text: "Frete Viagem:" });
    this.FreteTotal = PropertyEntity({ text: "Frete Total:" });
    this.Gris = PropertyEntity({ text: "Gris:" });
    this.Pedagio = PropertyEntity({ text: "Pedágio:" });
    this.Pis = PropertyEntity({ text: "PIS:" });
    this.TaxaDescarga = PropertyEntity({ text: "Taxa de Descarga:" });
    this.TaxaEntrega = PropertyEntity({ text: "Taxa de Entrega:" });
    this.TaxaTotal = PropertyEntity({ text: "Taxa Total:" });
    this.Pernoite = PropertyEntity({ text: "Pernoite:" });

    this.DefinirIVA = PropertyEntity({ eventClick: definirIVAModalClick, type: types.event, text: "Definir" });

    PreencherObjetoKnout(this, { Data: documentoContabilPorStage });
};

// #endregion Classes

// #region Funções de Inicialização

function loadFechamentoProvisao() {
    carregarHTMLFechamentoProvisao().then(function () {
        _fechamentoProvisao = new FechamentoProvisao();
        KoBindings(_fechamentoProvisao, "knoutFechamentoProvisao");

        _percentualFechamentoProvisao = new PercentualFechamentoProvisao();
        KoBindings(_percentualFechamentoProvisao, "knockoutPercentualFechamentoProvisao");

        _CRUDFechamentoProvisao = new CRUDFechamentoProvisao();
        KoBindings(_CRUDFechamentoProvisao, "knoutCRUDFechamentoProvisao");

        _fechamentoProvisaoDefinirIVA = new FechamentoProvisaoDefinirIVA();
        KoBindings(_fechamentoProvisaoDefinirIVA, "knoutFechamentoProvisaoDefinirIVA");

        _detalhamentoRateio = new DetalhamentoRateio();
        KoBindings(_detalhamentoRateio, "knockoutGridDetalhamentoRateio");

        BuscarImpostoValorAgregado(_fechamentoProvisaoDefinirIVA.ImpostoValorAgregado, null, true);

        loadGridDocumentosContabeis();
        loadGridDetalhamentoRateio();
       
    });
}

function loadGridDocumentosContabeis() {
    let header = [
        { data: "CodigoContaContabil", title: "Código", width: "15%" },
        { data: "DescricaoContaContabil", title: "Conta Contábil", width: "30%" },
        { data: "CodigoCentroResultado", title: "Centro de Resultado", width: "15%" },
        { data: "ValorContabilizacaoFormatado", title: "Valor", width: "15%" },
        { data: "DescricaoTipoContabilizacao", title: "Contabilização", width: "15%" }
    ];

    _gridDocumentosContabeis = new BasicDataTable(_fechamentoProvisao.Contabilizacao.idGrid, header, null, { column: 0, dir: orderDir.asc });
    _gridDocumentosContabeis.CarregarGrid([]);
}

function loadGridDetalhamentoRateio() {
    let header = [
        { data: "CodigoGrupoProduto", title: "Código", width: "33%" },
        { data: "Descricao", title: "Descrição", width: "33%" },
        { data: "ValorDetalhamentoFormatado", title: "Valor", width: "33%" },
    ];

    _gridDetalhamentoRateio = new BasicDataTable(_detalhamentoRateio.DetalhamentoRateio.idGrid, header, null, { column: 0, dir: orderDir.asc });
    _gridDetalhamentoRateio.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function cancelarFechamentoProvisaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja remover a provisão?", function () {
        let data = { Codigo: _provisao.Codigo.val() };
        executarReST("ProvisaoFechamento/CancelarProvisao", data, function (e) {
            if (e.Success) {
                if (e.Data !== false) {
                    exibirMensagem(tipoMensagem.Success, "Sucesso", "Cancelamento Confirmado");
                    LimparCamposProvisao();
                    _gridProvisao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "atenção", e.Msg);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
            }
        });
    });
}

function confirmarProvisaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar o processo de provisão?", function () {
        if (ValidarCamposObrigatorios(_fechamentoProvisao)) {
            let data = { Codigo: _provisao.Codigo.val(), DataLancamento: _fechamentoProvisao.DataLancamento.val() };
            executarReST("ProvisaoFechamento/ConfirmarFechamentoProvisao", data, function (e) {
                if (e.Success) {
                    if (e.Data !== false) {
                        exibirMensagem(tipoMensagem.Success, "Sucesso", "Provisão Finalizada com Sucesso");
                        BuscarProvisaoPorCodigo(_provisao.Codigo.val());
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "atenção", e.Msg);
                    }
                }
                else {
                    exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
                }
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        }
    });
}

function definirIVAClick() {
    if (!ValidarCamposObrigatorios(_fechamentoProvisaoDefinirIVA))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, informe os campos obrigatórios");

    executarReST("ProvisaoFechamento/DefirnirImpostoValorAgregado", RetornarObjetoPesquisa(_fechamentoProvisaoDefinirIVA), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Imposto sobre valor agregado definido com sucesso");
                Global.fecharModal('divModalFechamentoProvisaoDefinirIVA');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function definirIVAModalClick(documentoContabilPorStage) {
    _fechamentoProvisaoDefinirIVA.CodigoProvisao.val(_provisao.Codigo.val());
    _fechamentoProvisaoDefinirIVA.CodigoStage.val(documentoContabilPorStage.CodigoStage.val());
    _fechamentoProvisaoDefinirIVA.DocumentoContabilPorStage = documentoContabilPorStage;

    Global.abrirModal('divModalFechamentoProvisaoDefinirIVA');
    $("#divModalFechamentoProvisaoDefinirIVA").one('hidden.bs.modal', function () {
        LimparCampos(_fechamentoProvisaoDefinirIVA);
        _fechamentoProvisaoDefinirIVA.DocumentoContabilPorStage = null;
    });
}

function reprocessarProvisaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja reprocessar a provisão?", function () {
        let data = { Codigo: _provisao.Codigo.val() };
        executarReST("ProvisaoFechamento/ReprocessarProvisao", data, function (e) {
            if (e.Success) {
                if (e.Data !== false) {
                    exibirMensagem(tipoMensagem.Success, "Sucesso", "Provisão Finalizada com Sucesso");
                    BuscarProvisaoPorCodigo(_provisao.Codigo.val());
                } else {
                    exibirMensagem(tipoMensagem.atencao, "atenção", e.Msg);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
            }
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function buscarDadosFechamentoProvisao() {
    ocultarEtapasFechamentoTodos();
    if (_provisao.Situacao.val() === EnumSituacaoProvisao.PendenciaFechamento) {
        _CRUDFechamentoProvisao.ProcessarNovamente.visible(true);
        _CRUDFechamentoProvisao.ConfirmarProvisao.visible(false);
        $("#divMotivoProblemaFechamento").show();
        $("#pMotivoProblemaFechamento").text(_provisao.MotivoRejeicaoFechamentoProvisao.val());
    } else {
        if (_provisao.GerandoMovimentoFinanceiroProvisao.val()) {
            $("#knockoutPercentualFechamentoProvisao").show();
        } else {
            obterDetalhesFechamentoProvisao();
        }
    }
}

function limparCamposProcessaomentoFechamentoProvisao() {
    setarPercentualProcessamentoFechamentoProvisao(0);
    LimparCampos(_percentualFechamentoProvisao);
}

function ocultarBotoesFechamento() {
    _CRUDFechamentoProvisao.ConfirmarProvisao.visible(false);
    _CRUDFechamentoProvisao.ProcessarNovamente.visible(false);
}

function ocultarEtapasFechamentoTodos() {
    setarPercentualProcessamentoFechamentoProvisao(0);
    $("#knoutFechamentoProvisao").hide();
    $("#knockoutPercentualFechamentoProvisao").hide();
    $("#divMotivoProblemaFechamento").hide();
    _CRUDFechamentoProvisao.ConfirmarProvisao.visible(true);
    _CRUDFechamentoProvisao.ProcessarNovamente.visible(false);
}

function setarPercentualProcessamentoFechamentoProvisao(percentual) {
    let strPercentual = parseInt(percentual) + "%";
    _percentualFechamentoProvisao.PercentualProcessado.val(strPercentual);
    $("#" + _percentualFechamentoProvisao.PercentualProcessado.id).css("width", strPercentual);
}

// #endregion Funções Públicas

// #region Funções Privadas

function carregarDocumentosContabeisPorStage(documentosContabeisPorStage) {
    _fechamentoProvisao.DocumentosContabeisPorStage.removeAll();

    for (let i = 0; i < documentosContabeisPorStage.length; i++)
        _fechamentoProvisao.DocumentosContabeisPorStage.push(new DocumentoContabilPorStage(documentosContabeisPorStage[i]));
}

function carregarGridDocumentosContabeis(documentosContabeis) {
    _gridDocumentosContabeis.CarregarGrid(documentosContabeis);
}

function carregarGridDetalhamentoRateio(detalhesRateio) {
    _gridDetalhamentoRateio.CarregarGrid(detalhesRateio);
}

function carregarHTMLFechamentoProvisao(callback) {
    let prom = new promise.Promise();
    $.get("Content/Static/Escrituracao/FechamentoProvisao.html?dyn=" + guid(), function (data) {
        $("#contentFechamentoProvisao").html(data);
        prom.done();
    });
    return prom;
}

function obterDetalhesFechamentoProvisao() {
    let data = { Codigo: _provisao.Codigo.val() };
    executarReST("ProvisaoFechamento/ObterDetalhesFechamento", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarGridDocumentosContabeis(retorno.Data.DocumentosContabeis);
                carregarDocumentosContabeisPorStage(retorno.Data.DocumentosContabeisPorStage);
                carregarGridDetalhamentoRateio(retorno.Data.DetalhamentoRateio);

                _fechamentoProvisao.DataLancamento.val(retorno.Data.DataLancamento);
                _fechamentoProvisao.PossuiStage.val(retorno.Data.PossuiStage);
            }
            else
                exibirMensagem(tipoMensagem.atencao, "atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);

        if (_provisao.Situacao.val() === EnumSituacaoProvisao.EmFechamento)
            _CRUDFechamentoProvisao.ProcessarNovamente.visible(true);

        $("#knoutFechamentoProvisao").show();
    });
}

// #endregion Funções Privadas
