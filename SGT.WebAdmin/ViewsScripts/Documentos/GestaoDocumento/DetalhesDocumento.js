/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/EmissaoCTe/EmissaoCTe.js" />


_fluxoInternacional = false;

// #region Classes

var DetalheDocumento = function () {
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int, icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.CodigoEmpresa = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int, icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.ValorDesconto = PropertyEntity({ text: "Valor de Desconto:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.ValorAReceber = PropertyEntity({ text: "Valor a Receber:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.AliquotaICMS = PropertyEntity({ text: "Aliquota ICMS:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), aprovado: _CONFIGURACAO_TMS.ValidarSomenteFreteLiquidoNaImportacaoCTe, visible: ko.observable(!_fluxoInternacional)});
    this.BaseCalculoICMS = PropertyEntity({ text: "Base de Calculo:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), aprovado: _CONFIGURACAO_TMS.ValidarSomenteFreteLiquidoNaImportacaoCTe, visible: ko.observable(!_fluxoInternacional)});
    this.ValorICMS = PropertyEntity({ text: "Valor ICMS:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), aprovado: _CONFIGURACAO_TMS.ValidarSomenteFreteLiquidoNaImportacaoCTe, visible: ko.observable(!_fluxoInternacional)});
    this.ValorFrete = PropertyEntity({ text: "Valor Do Frete:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(_CONFIGURACAO_TMS.ValidarSomenteFreteLiquidoNaImportacaoCTe), aprovado: _CONFIGURACAO_TMS.ValidarSomenteFreteLiquidoNaImportacaoCTe });
    this.CST = PropertyEntity({ text: "CST:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), aprovado: _CONFIGURACAO_TMS.ValidarSomenteFreteLiquidoNaImportacaoCTe, visible: ko.observable(!_fluxoInternacional) });
    this.AliquotaCBS = PropertyEntity({ text: "Alíquota CBS:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.ValorCBS = PropertyEntity({ text: "Valor CBS:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.AliquotaIBSUF = PropertyEntity({ text: "Alíquota IBS UF:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.ValorIBSUF = PropertyEntity({ text: "Valor IBS UF:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.AliquotaIBSMunicipal = PropertyEntity({ text: "Alíquota IBS Municipal:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.ValorIBSMunicipal = PropertyEntity({ text: "Valor IBS Municipal:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.BaseCalculoIBSCBS = PropertyEntity({ text: "Base do Calculo IBS/CBS:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.CSTIBSCBS = PropertyEntity({ text: "CST IBS/CBS:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.ClassificacaoTributariaIBSCBS = PropertyEntity({ text: "C Class Trib IBS/CBS:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.TipoAmbiente = PropertyEntity({ text: "Tipo do Ambiente:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.Tomador = PropertyEntity({ text: "Tomador:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.Remetente = PropertyEntity({ text: "Remetente:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: "Destinatário:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.Expedidor = PropertyEntity({ text: "Expedidor:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.Recebedor = PropertyEntity({ text: "Recebedor:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.Origem = PropertyEntity({ text: "Origem:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.Documentos = PropertyEntity({ text: "NF-e:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.NumeroCTe = PropertyEntity({ text: "N° CTe:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.Destino = PropertyEntity({ text: "Destino:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.Emissor = PropertyEntity({ text: "Emissor:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.Rota = PropertyEntity({ text: "Rota:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(!_fluxoInternacional)});
    this.CFOP = PropertyEntity({ text: "CFOP:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(!_CONFIGURACAO_TMS.ValidarSomenteFreteLiquidoNaImportacaoCTe && !_fluxoInternacional) });
    this.Peso = PropertyEntity({ text: "Peso:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(!_CONFIGURACAO_TMS.ValidarSomenteFreteLiquidoNaImportacaoCTe || _fluxoInternacional) });
    this.ValorMercadoria = PropertyEntity({ text: "Valor Mercadoria:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: !_CONFIGURACAO_TMS.ValidarSomenteFreteLiquidoNaImportacaoCTe });
    this.Complemento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.ComponentesFrete = PropertyEntity({ type: types.map, val: ko.observableArray([]), icon: ko.observable(""), color: ko.observable(""), bgColor: ko.observable("") });

    //Controle das abas
    this.DescricaoDocumento = PropertyEntity({ val: ko.observable(""), def: "", icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.IndexTab = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.IdTab = PropertyEntity({ val: ko.observable(""), def: "", icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });

    this.DownloadXMLCTe = PropertyEntity({ eventClick: downloadXMLClick, type: types.event, text: "XML", idGrid: guid(), visible: ko.observable(true), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.DownloadXMLPreCTe = PropertyEntity({ eventClick: downloadPreCTeClick, type: types.event, text: "XML (Pré CT-e)", idGrid: guid(), visible: ko.observable(false), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.DonwloadDacteCTe = PropertyEntity({ eventClick: baixarDacteGestaoDocumentoClick, type: types.event, text: "Download DACTE", idGrid: guid(), visible: ko.observable(true), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.DetalhesCTe = PropertyEntity({ eventClick: detalhesCTeGestaoDocumentoClick, type: types.event, text: "Detalhes CT-e", idGrid: guid(), visible: ko.observable(true), icon: ko.observable("fa fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
}

var DetalhesDocumentosCTes = function () {
    this.CTes = PropertyEntity({ val: ko.observableArray([]), def: [] });
}

// #endregion Classes

// #region Funções Associadas a Eventos

function downloadPreCTeClick(detalhesDocumento) {
    var data = { CodigoPreCTE: detalhesDocumento.Codigo.val(), CodigoEmpresa: detalhesDocumento.CodigoEmpresa.val() };

    executarDownload("CargaPreCTe/DownloadPreXML", data);
}

function downloadXMLClick(detalhesDocumento) {
    var data = { CodigoCTe: detalhesDocumento.Codigo.val(), CodigoEmpresa: detalhesDocumento.CodigoEmpresa.val() };

    executarDownload("CargaCTe/DownloadXML", data);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function verificarDivergenciasEntreDocumentoEsperadoERecebido(detalhesDocumentoEsperado, detalhesDocumentoRecebido) {
    $.each(detalhesDocumentoRecebido, function (nomePropriedade, prop) {
        compararDocumentoEsperadoERecebido(detalhesDocumentoEsperado[nomePropriedade], detalhesDocumentoRecebido[nomePropriedade], nomePropriedade);
    });
}

function definirDocumentoRecebidoComoDivergente(detalhesDocumentoEsperado, detalhesDocumentoRecebido) {
    $.each(detalhesDocumentoEsperado, function (i, prop) {
        definirPropriedadeDocumentoRecebidoComoDivergente(detalhesDocumentoRecebido[i]);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function compararDocumentoEsperadoERecebido(propriedadeDocumentoEsperado, propriedadeDocumentoRecebido, nomePropriedade) {
    var propriedadeNaoValidarDados = (
        nomePropriedade == "Tomador" ||
        nomePropriedade == "Remetente" ||
        nomePropriedade == "Destinatario" ||
        nomePropriedade == "Expedidor" ||
        nomePropriedade == "Recebedor" ||
        nomePropriedade == "Emissor" ||
        nomePropriedade == "TipoAmbiente" ||
        nomePropriedade == "Origem" ||
        nomePropriedade == "Documentos" ||
        nomePropriedade == "NumeroCTe" ||
        nomePropriedade == "Destino" ||
        nomePropriedade == "Rota" ||
        nomePropriedade == "CFOP" ||
        nomePropriedade == "Peso" ||
        nomePropriedade == "ValorMercadoria"
    );

    var naoCompararPropriedades = _CONFIGURACAO_TMS.NaoValidarDadosParticipantesNaImportacaoCTe && propriedadeNaoValidarDados;

    if (naoCompararPropriedades || (propriedadeDocumentoRecebido.val() == propriedadeDocumentoEsperado.val()) || propriedadeDocumentoRecebido.aprovado)
        definirPropriedadeDocumentoRecebidoComoIgual(propriedadeDocumentoRecebido);
    else
        definirPropriedadeDocumentoRecebidoComoDivergente(propriedadeDocumentoRecebido);
}

function definirPropriedadeDocumentoRecebidoComoDivergente(propriedade) {
    propriedade.icon("fas fa-ban");
    propriedade.color("red");
    propriedade.bgColor("rgba(201, 76, 76, 0.3)");
}

function definirPropriedadeDocumentoRecebidoComoIgual(propriedade) {
    propriedade.icon("fas fa-check");
    propriedade.color("green");
    propriedade.bgColor("");
}

function baixarDacteGestaoDocumentoClick(detalhesDocumento) {
    var data = { CodigoCTe: detalhesDocumento.Codigo.val(), CodigoEmpresa: detalhesDocumento.CodigoEmpresa.val() };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function detalhesCTeGestaoDocumentoClick(detalhesDocumento) {
    var codigo = parseInt(detalhesDocumento.Codigo.val());
    var permissoesSomenteLeituraCTe = new Array();
    permissoesSomenteLeituraCTe.push(EnumPermissoesEdicaoCTe.Nenhuma);
    var instancia = new EmissaoCTe(codigo, function () {
        instancia.CRUDCTe.Emitir.visible(false);
        instancia.CRUDCTe.Salvar.visible(false);
        instancia.CRUDCTe.Salvar.eventClick = function () {
            var objetoCTe = ObterObjetoCTe(instancia);
            SalvarCTe(objetoCTe, detalhesDocumento.Codigo.val(), instancia);
        }
    }, permissoesSomenteLeituraCTe);
}

function PreencherDetalhesCTes(data, knout) {
    _fluxoInternacional = data.FluxoInternacional == 1;

    var cteAtual = new DetalheDocumento();
    var cteAnterior;

    PreencherObjetoKnout(cteAtual, { Data: data.CTe });
    cteAtual.DescricaoDocumento.val("Doc. Recebido");
    cteAtual.IndexTab.val(0);
    cteAtual.IdTab.val(guid());
    cteAtual.ComponentesFrete.val(data.ComponentesFrete);

    _detalhePreCTeAprovacao.ComponentesFrete.val(data.ComponentesFrete);

    if (data.PreCTe != null) {
        PreencherObjetoKnout(_detalhePreCTeAprovacao, { Data: data.PreCTe });
        ControlarVisibilidadeCampos(cteAtual);

        _detalhePreCTeAprovacao.DownloadXMLPreCTe.visible(true);

        verificarDivergenciasEntreDocumentoEsperadoERecebido(_detalhePreCTeAprovacao, cteAtual);
    }
    else {
        _detalhePreCTeAprovacao.DownloadXMLPreCTe.visible(false);

        definirDocumentoRecebidoComoDivergente(_detalhePreCTeAprovacao, cteAtual);
    }

    knout.CTes.val.push(cteAtual);

    if (data.CTesAnteriores != null && Array.isArray(data.CTesAnteriores)) {
        data.CTesAnteriores.forEach((e, i) => {
            cteAnterior = new DetalheDocumento();
            PreencherObjetoKnout(cteAnterior, { Data: e });

            cteAnterior.DescricaoDocumento.val(`Doc. Anterior ${i + 1}`);
            cteAnterior.IndexTab.val(i + 1);
            cteAnterior.IdTab.val(guid());
            cteAnterior.DownloadXMLCTe.visible(false);
            cteAnterior.DownloadXMLPreCTe.visible(false);
            cteAnterior.DonwloadDacteCTe.visible(false);
            cteAnterior.DetalhesCTe.visible(false);

            verificarDivergenciasEntreDocumentoEsperadoERecebido(_detalhePreCTeAprovacao, cteAnterior);

            knout.CTes.val.push(cteAnterior);
        })
    }
}

function LimparCamposCTesAprovacao() {
    _ctesAprovacao.CTes.val.removeAll();
}

function ControlarVisibilidadeCampos(knouReferencia) {
    _detalhePreCTeAprovacao.AliquotaICMS.visible(knouReferencia.AliquotaICMS.visible());
    _detalhePreCTeAprovacao.BaseCalculoICMS.visible(knouReferencia.BaseCalculoICMS.visible());
    _detalhePreCTeAprovacao.ValorICMS.visible(knouReferencia.ValorICMS.visible());
    _detalhePreCTeAprovacao.CST.visible(knouReferencia.CST.visible());
    _detalhePreCTeAprovacao.Rota.visible(knouReferencia.Rota.visible());
    _detalhePreCTeAprovacao.CFOP.visible(knouReferencia.CFOP.visible());
    _detalhePreCTeAprovacao.Peso.visible(knouReferencia.Peso.visible());
}

// #endregion Funções Privadas
