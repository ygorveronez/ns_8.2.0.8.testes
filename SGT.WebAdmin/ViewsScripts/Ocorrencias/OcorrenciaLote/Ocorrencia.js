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
/// <reference path="OcorrenciaLote.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ocorrenciaLoteOcorrencia;
var _CRUDOcorrenciaLoteOcorrencia;
var _gridOcorrencia;

var OcorrenciaLoteOcorrencia = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.MotivoRejeicao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Motivo da rejeição na geração de ocorrências:", enable: ko.observable(true), visible: ko.observable(false) });

    this.Ocorrencias = PropertyEntity({ idGrid: guid() });
};

var CRUDOcorrenciaLoteOcorrencia = function () {
    this.Reprocessar = PropertyEntity({ eventClick: ReprocessarFalhaGeracaoClick, type: types.event, text: "Reprocessar geração das ocorrências de cargas com falha", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadOcorrenciaLoteOcorrencia() {
    _ocorrenciaLoteOcorrencia = new OcorrenciaLoteOcorrencia();
    KoBindings(_ocorrenciaLoteOcorrencia, "knockoutOcorrencia");

    _CRUDOcorrenciaLoteOcorrencia = new CRUDOcorrenciaLoteOcorrencia();
    KoBindings(_CRUDOcorrenciaLoteOcorrencia, "knockoutCRUDOcorrencia");

    BuscarOcorrencias();
}

function EtapaOcorrenciaLoteOcorrenciaClick() {
    _ocorrenciaLoteOcorrencia.Codigo.val(_ocorrenciaLote.Codigo.val());

    _gridOcorrencia.CarregarGrid();
}

function ReprocessarFalhaGeracaoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente reprocessar a geração das ocorrências pendentes?", function () {
        executarReST("OcorrenciaLote/ReprocessarOcorrenciasNaoGeradas", { Codigo: _ocorrenciaLote.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitado a geração com sucesso, aguarde o processamento.");
                    _gridOcorrenciaLote.CarregarGrid();
                    LimparCamposOcorrenciaLote();

                    preencherOcorrenciaLoteRetorno(retorno);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        });
    });
}

////*******MÉTODOS*******

function BuscarOcorrencias() {
    _gridOcorrencia = new GridView(_ocorrenciaLoteOcorrencia.Ocorrencias.idGrid, "OcorrenciaLote/PesquisaOcorrencias", _ocorrenciaLoteOcorrencia);
}

function ControleCamposOcorrenciaLoteOcorrencia() {
    var situacao = _ocorrenciaLote.Situacao.val();

    _CRUDOcorrenciaLoteOcorrencia.Reprocessar.visible(false);
    _ocorrenciaLoteOcorrencia.MotivoRejeicao.visible(false);

    if (situacao === EnumSituacaoOcorrenciaLote.FalhaNaGeracao) {
        _CRUDOcorrenciaLoteOcorrencia.Reprocessar.visible(true);
        _ocorrenciaLoteOcorrencia.MotivoRejeicao.visible(true);
    }
}

function LimparCamposOcorrenciaLoteOcorrencia() {
    LimparCampos(_ocorrenciaLoteCarga);
}