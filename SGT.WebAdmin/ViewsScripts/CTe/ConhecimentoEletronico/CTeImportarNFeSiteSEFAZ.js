/// <reference path="ConhecimentoEletronico.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../CTe/CTe/CTe.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ObjetoRequisicaoSEFAZ, _CTeImportarNFeSEFAZ, _NFEsImportadas, _modalCTeImportarNFeSEFAZ;

var CTeImportarNFeSEFAZ = function () {
    this.Chave = PropertyEntity({ text: "*Chave da NF-e:", val: ko.observable(""), def: "", visible: ko.observable(true), required: true });
    this.Captcha = PropertyEntity({ text: "*Captcha:", val: ko.observable(""), def: "", visible: ko.observable(true), required: true });

    this.GerarNovoCaptcha = PropertyEntity({ eventClick: ObterRequisicaoSEFAZ, type: types.event, text: "Gerar novo captcha" });
    this.ConsultarNFe = PropertyEntity({ eventClick: ConsultarNFeClick, type: types.event, text: "Consultar" });
    this.GerarCTe = PropertyEntity({ eventClick: GerarCTeClick, type: types.event, text: "Gerar CT-e" });
}

//*******EVENTOS*******

function LoadImportarNFeSiteSEFAZ() {

    _CTeImportarNFeSEFAZ = new CTeImportarNFeSEFAZ();
    KoBindings(_CTeImportarNFeSEFAZ, "divModalCTeImportarNFeSEFAZ");

    $('#' + _CTeImportarNFeSEFAZ.Captcha.id).bind('keypress', function (event) {
        if (event.keyCode === 13)
            ConsultarNFeClick();
    });

    _modalCTeImportarNFeSEFAZ = new bootstrap.Modal(document.getElementById("divModalCTeImportarNFeSEFAZ"), { backdrop: 'static', keyboard: true });
}

function CTeImportarNFeSEFAZClick() {
    _NFEsImportadas = new Array();
    RecarregarGridNFesImportadas();

    ObterRequisicaoSEFAZ();

    _modalCTeImportarNFeSEFAZ.show();
}

function ConsultarNFeClick() {
    var chave = _CTeImportarNFeSEFAZ.Chave.val().replace(/\D/g, '');
    for (var i = 0; i < _NFEsImportadas.length; i++) {
        if (chave == _NFEsImportadas[i].Chave) {
            exibirMensagem(tipoMensagem.atencao, "Chave já registrada", "A chave informada já foi consultada.");
            return;
        }
    }

    if (!(ValidarCamposObrigatorios(_CTeImportarNFeSEFAZ))) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Verifique os campos obrigatórios!");
        return;
    }

    if (!(ValidarChaveAcesso(_CTeImportarNFeSEFAZ.Chave.val()))) {
        exibirMensagem(tipoMensagem.atencao, "Chave de NF-e inválida", "A chave da NF-e é inválida.");
        return;
    }

    executarReST("ConsultaSiteSEFAZ/ConsultarNFe", { Requisicao: JSON.stringify(_ObjetoConsultaSEFAZ), Chave: _CTeImportarNFeSEFAZ.Chave.val(), Captcha: _CTeImportarNFeSEFAZ.Captcha.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _NFEsImportadas.push(r.Data);
                RecarregarGridNFesImportadas();
                LimparCampos(_CTeImportarNFeSEFAZ);
                ObterRequisicaoSEFAZ();
                $("#" + _CTeImportarNFeSEFAZ.Chave.id).focus();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function GerarCTeClick() {
    if (_NFEsImportadas.length <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Quantidade de NF-e Inválida", "É necessário consultar uma ou mais chaves de NF-e para gerar o CT-e.");
        return;
    }

    _modalCTeImportarNFeSEFAZ.hide();
    abrirModalCTe(0, _NFEsImportadas);
}

////*******MÉTODOS*******

function RecarregarGridNFesImportadas() {
    if (_NFEsImportadas.length > 0) {
        $("#tblNFeSEFAZ tbody").html("");
        for (var i = 0; i < _NFEsImportadas.length; i++) {
            var nfeImportada = _NFEsImportadas[i];
            $("#tblNFeSEFAZ tbody").append('<tr id="xmlNFeImportar_' + i + '"><td>' + nfeImportada.Numero + '</td><td>' + nfeImportada.Chave + '</td><td>' +
                Globalize.format(nfeImportada.Peso, "n2") + '</td><td>' + Globalize.format(nfeImportada.ValorTotal, "n2") + '</td><td class="text-align-center"><a href="javascript:void(0);" onclick="ExcluirNFeSEFAZ(' + i + ');">Excluir</a></td></tr>');
        }
    } else {
        $("#tblNFeSEFAZ tbody").html('<tr><td colspan="5">Nenhuma NF-e importada.</td></tr>');
    }
}

function ExcluirNFeSEFAZ(indice) {
    _NFEsImportadas.splice(indice, 1);
    RecarregarGridNFesImportadas();
}

function ObterRequisicaoSEFAZ() {
    executarReST("ConsultaSiteSEFAZ/ObterRequisicao", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                $("#imgCaptchaSEFAZ").prop("src", r.Data.Objeto.Captcha);
                _ObjetoConsultaSEFAZ = r.Data.Objeto;
                delete _ObjetoConsultaSEFAZ.Captcha;
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}