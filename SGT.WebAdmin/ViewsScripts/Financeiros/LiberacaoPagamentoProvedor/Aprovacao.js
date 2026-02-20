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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="LiberacaoPagamentoEtapa.js" />
/// <reference path="LiberacaoPagamentoProvedor.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _documentosProvedor;
var _detalheDocumentoRecebido;
var _detalheDocumento;

var DetalheDocumento = function () {
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int, icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.ValorAReceber = PropertyEntity({ text: "Valor a Receber:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.ValorTotalProvedor = PropertyEntity({ text: "Valor Total Provedor:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable("") });
    this.AliquotaICMS = PropertyEntity({ text: "Aliquota ICMS:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
    this.ValorICMS = PropertyEntity({ text: "Valor ICMS:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
    this.Tomador = PropertyEntity({ text: "Tomador:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
    this.Remetente = PropertyEntity({ text: "Remetente:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ text: "Destinatário:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
    this.Expedidor = PropertyEntity({ text: "Expedidor:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
    this.Recebedor = PropertyEntity({ text: "Recebedor:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
    this.Emissor = PropertyEntity({ text: "Emissor:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
    this.DataEmissaoNFSe = PropertyEntity({ text: "Data Emissão:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
    this.NumeroNFSe = PropertyEntity({ text: "Número NFS-e:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
    this.HouveDivergenciaEntreDetalhesDocumentoComDocumentoRecebido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.RegraICMS = PropertyEntity({ text: "Regra ICMS:", val: ko.observable(""), icon: ko.observable("fas fa-check"), color: ko.observable("green"), bgColor: ko.observable(""), visible: ko.observable(false) });
}

var CRUDAprovacao = function () {
    this.DownloadXML = PropertyEntity({ type: types.event, eventClick: downloadXMLClick, text: "XML", visible: ko.observable(true) });
    this.DownloadDACTE = PropertyEntity({ type: types.event, eventClick: downloadDACTElick, text: ko.observable("Download DACTE"), visible: ko.observable(true) });
    this.DetalhesCTe = PropertyEntity({ type: types.event, eventClick: detalhesCTeClick, text: "Detalhes CT-e", visible: ko.observable(false) });
    this.Anexos = PropertyEntity({ type: types.event, eventClick: anexosClick, text: "Anexos", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Novo", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadAprovacao() {
    _detalheDocumento = new DetalheDocumento();
    KoBindings(_detalheDocumento, "knoutDetalhesDocumento");

    _detalheDocumentoRecebido = new DetalheDocumento();
    KoBindings(_detalheDocumentoRecebido, "knoutDocumentoRecebido");

    _CRUDAprovacao = new CRUDAprovacao();
    KoBindings(_CRUDAprovacao, "knockoutCRUDAprovacao");

    visibilidadeCampoCRUDAprovacao(true);
}

function preencherDocumentosAprovacao() {
    executarReST("LiberacaoPagamentoProvedor/PreencherDocumentosAprovacao", { CodigoPagamentoProvedor: _documentosProvedor.CodigoPagamentoProvedor.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmado com sucesso");

                PreencherObjetoKnout(_detalheDocumentoRecebido, { Data: arg.Data.DetalhesDocumentoRecebido });
                PreencherObjetoKnout(_regraPagamentoProvedor, { Data: arg.Data.Autorizacao });
                _gridRegrasLiberacaoPagamentoProvedor.CarregarGrid();

                verificarTratarDivergencias(arg.Data.DetalhesDocumento, _detalheDocumento, _detalheDocumentoRecebido, false);

                if ((_documentosProvedor.TipoDocumentoProvedor.val() == 'CTe' || _documentosProvedor.TipoDocumentoProvedor.val() == 'CTe Complementar') && _documentosProvedor.MultiplosCTe.val() != true) {
                    visibilidadeCTeAprovacao();
                } else if (_documentosProvedor.TipoDocumentoProvedor.val() == 'NFSe') {
                    visibilidadeNFSeAprovacao();
                }
                else if (_documentosProvedor.MultiplosCTe.val() == true) {
                    visibilidadeMultiploCTeAprovacao();
                }

            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function verificarTratarDivergencias(detalhesDocumento, detalheDocumento, detalheDocumentoRecebido, editado) {
    if (detalhesDocumento != null) {
        PreencherObjetoKnout(detalheDocumento, { Data: detalhesDocumento });
        verificarDivergenciasEntreDocumentoEsperadoERecebido(detalheDocumento, detalheDocumentoRecebido, editado);
    } else {
        definirDocumentoRecebidoComoDivergente(detalheDocumento, detalheDocumentoRecebido);
    }
}

function verificarDivergenciasEntreDocumentoEsperadoERecebido(detalhesDocumentoEsperado, detalhesDocumentoRecebido, editado) {
    $.each(detalhesDocumentoRecebido, function (nomePropriedade, prop) {
        compararDocumentoEsperadoERecebido(detalhesDocumentoEsperado[nomePropriedade], detalhesDocumentoRecebido[nomePropriedade], nomePropriedade);
    });

    compararDocumentoEsperadoERecebido(detalhesDocumentoEsperado["ValorTotalProvedor"], detalhesDocumentoRecebido["ValorAReceber"], "true");

    if (!editado)
        CriarAprovacao();
}

function compararDocumentoEsperadoERecebido(propriedadeDocumentoEsperado, propriedadeDocumentoRecebido, nomePropriedade) {
    if (nomePropriedade == '' || nomePropriedade == 'HouveDivergenciaEntreDetalhesDocumentoComDocumentoRecebido')
        return;

    var propriedadeNaoValidarDados = (
        nomePropriedade == "DataEmissaoNFSe" ||
        nomePropriedade == "NumeroNFSe"
    );

    var naoCompararPropriedades = _CONFIGURACAO_TMS.NaoValidarDadosParticipantesNaImportacaoCTe || propriedadeNaoValidarDados;

    var propriedadesCNPJ = ["Tomador", "Remetente", "Destinatario", "Expedidor", "Recebedor", "Emissor"];

    if (propriedadesCNPJ.includes(nomePropriedade)) {
        var cnpjEsperado = extrairCNPJ(propriedadeDocumentoEsperado.val());
        var cnpjRecebido = extrairCNPJ(propriedadeDocumentoRecebido.val());

        if (naoCompararPropriedades || (cnpjEsperado == cnpjRecebido) || propriedadeDocumentoRecebido.aprovado)
            definirPropriedadeDocumentoRecebidoComoIgual(propriedadeDocumentoRecebido);
        else {
            definirPropriedadeDocumentoRecebidoComoDivergente(propriedadeDocumentoRecebido);
            _detalheDocumento.HouveDivergenciaEntreDetalhesDocumentoComDocumentoRecebido.val(true);
        }
    } else {
        if (naoCompararPropriedades || (propriedadeDocumentoRecebido.val() == propriedadeDocumentoEsperado.val()) || propriedadeDocumentoRecebido.aprovado)
            definirPropriedadeDocumentoRecebidoComoIgual(propriedadeDocumentoRecebido);
        else
            definirPropriedadeDocumentoRecebidoComoDivergente(propriedadeDocumentoRecebido);
    }
}

function CriarAprovacao() {
    var data = { CodigoPagamentoProvedor: _documentosProvedor.CodigoPagamentoProvedor.val(), HouveDivergenciaEntreCampos: _detalheDocumento.HouveDivergenciaEntreDetalhesDocumentoComDocumentoRecebido.val() };
    executarReST("LiberacaoPagamentoProvedor/CriarAprovacao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _gridRegrasLiberacaoPagamentoProvedor.CarregarGrid();

                SetarEtapasPagamentoProvedor(arg.Data.Status, arg.Data.Situacao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function definirPropriedadeDocumentoRecebidoComoIgual(propriedade) {
    propriedade.icon("fas fa-check");
    propriedade.color("green");
    propriedade.bgColor("");
}

function definirPropriedadeDocumentoRecebidoComoDivergente(propriedade) {
    propriedade.icon("fas fa-ban");
    propriedade.color("red");
    propriedade.bgColor("rgba(201, 76, 76, 0.3)");
}

function extrairCNPJ(texto) {
    var match = texto.match(/\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}/);
    return match ? match[0] : null;
}

function downloadXMLClick() {
    var data = { CodigoPagamentoProvedor: _documentosProvedor.CodigoPagamentoProvedor.val() };
    executarDownload("LiberacaoPagamentoProvedor/DownloadXML", data);
}

function downloadDACTElick() {
    var data = { CodigoPagamentoProvedor: _documentosProvedor.CodigoPagamentoProvedor.val() };
    executarDownload("LiberacaoPagamentoProvedor/DownloadDACTE", data);
}

function detalhesCTeClick() {
    var data = { CodigoPagamentoProvedor: _documentosProvedor.CodigoPagamentoProvedor.val() };
    executarReST("LiberacaoPagamentoProvedor/DetalhesCTe", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var codigo = arg.Data.Codigo;
                var permissoesSomenteLeituraCTe = new Array();
                permissoesSomenteLeituraCTe.push(EnumPermissoesEdicaoCTe.Nenhuma);
                var instancia = new EmissaoCTe(codigo, function () {
                    instancia.CRUDCTe.Emitir.visible(false);
                    instancia.CRUDCTe.Salvar.visible(false);
                    instancia.CRUDCTe.Salvar.eventClick = function () {
                        var objetoCTe = ObterObjetoCTe(instancia);
                        SalvarCTe(objetoCTe, codigo, instancia);
                    }
                }, permissoesSomenteLeituraCTe);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function anexosClick() {
    adicionarAnexoModalClick();
}

function visibilidadeCampoCRUDAprovacao(valor) {
   _CRUDAprovacao.DetalhesCTe.visible(valor);

    if (valor)
        _CRUDAprovacao.DownloadDACTE.text("Download DACTE");
    else
        _CRUDAprovacao.DownloadDACTE.text("Download DANFSE");
}