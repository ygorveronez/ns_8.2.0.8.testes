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
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="CTeComplementar.js" />
/// <reference path="NFSComplementar.js" />


//*******EVENTOS*******

var _codigoPreCte;
var _codigoCargaCTE;
var _rowPreCTeOcorrencia;
var _globalDataPreCTeOcorrencia;
var _gridCargaPreCTeOcorrencia;

function AbrirEnvioCTeDoPreCTeOcorrenciaClick(data, row) {
    _codigoPreCte = data.CodigoPreCTE;
    _codigoCargaCTE = data.Codigo;
    _rowPreCTeOcorrencia = row;
    _globalDataPreCTeOcorrencia = data;
    $('#FileEnviarCTeDoPreCTeOcorrencia').val("");
    $("#FileEnviarCTeDoPreCTeOcorrencia").trigger("click");
}

function carregarGridPreCTeComplementares() {

    var enviarCTe = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.EnviarXMLdoCte, id: guid(), metodo: function (data, row) {AbrirEnvioCTeDoPreCTeOcorrenciaClick(data, row);}, icone: "", visibilidade: VisibilidadeEnvioPreCTeOcorrencia};
    var baixarXMLPreCTeOcorrencia = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.DownloadPreCTe, id: guid(), metodo: baixarPreCTeOcorrenciaClick, icone: "", visibilidade: VisibilidadeBaixarXMLPreCTeOcorrencia};
    var lancarNFSeManual = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.LancarNFSeManual, id: guid(), metodo: abrirEmissaoNFSeManual, icone: "", visibilidade: VisibilidadeLancarNFSeManual };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 7, opcoes: [lancarNFSeManual, enviarCTe, baixarXMLPreCTeOcorrencia] };

    _gridCargaPreCTeOcorrencia = new GridView(_documentoComplementar.PreCTesComplementares.idGrid, "OcorrenciaPreCTe/ConsultarPreCTesOcorrencia", _ocorrencia, menuOpcoes, null);
}

function lancarNFSeManualClick() {

}

function VisibilidadeEnvioPreCTeOcorrencia(data) {
    return !data.CteEnviado && !_ocorrencia.EmiteNFSeFora.val();
}

function VisibilidadeBaixarXMLPreCTeOcorrencia(data) {
    return !_ocorrencia.EmiteNFSeFora.val();
}

function VisibilidadeLancarNFSeManual(data) {
    return _ocorrencia.EmiteNFSeFora.val();
}

function EnviarCTeDoPreCTeOcorrenciaClick() {
    if ($('#FileEnviarCTeDoPreCTeOcorrencia').val() != "") {
        var file = document.getElementById("FileEnviarCTeDoPreCTeOcorrencia");
        exibirConfirmacao(Localization.Resources.Ocorrencias.Ocorrencia.Confirmacao, Localization.Resources.Ocorrencias.Ocorrencia.RealmenteDesejaEnviarArquivoComoCTe.format(file.files[0].name) , function () {

            var formData = new FormData();
            formData.append("upload", file.files[0]);
            var data = {
                CodigoPreCTe: _codigoPreCte,
                Codigo: _codigoCargaCTE,
                Ocorrencia: _ocorrencia.Codigo.val()
            };
            enviarArquivo("OcorrenciaPreCTe/EnviarCTe?callback=?", data, formData, function (arg) {
                file.value = null;
                if (arg.Success) {
                    if (arg.Data !== false) {
                        if (arg.Data !== true) {
                            CompararEAtualizarGridEditableDataRow(_globalDataPreCTeOcorrencia, arg.Data)
                            _gridCargaPreCTeOcorrencia.AtualizarDataRow(_rowPreCTeOcorrencia, _globalDataPreCTeOcorrencia);
                        }
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.Ocorrencia.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.CTeEnviadComSucesso);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 30000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        });
    }
}

function buscarNotfisCarga(callback) {
    //loadDropZonePreCTeOcorrencia();
    //var baixarNotFisCarga = { descricao: "Donwload", id: guid(), metodo: baixarNotfis, icone: "", tamanho: 10 };
    //var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: "Opções", opcoes: [baixarNotFisCarga] };

    //var gridNotfis = new GridView(_documentoComplementar.NotFis.idGrid, "ControleGeracaoEDI/Pesquisa", _documentoComplementar, menuOpcoes, null);
    //gridNotfis.CarregarGrid(function () {
    //    if (gridNotfis.NumeroRegistros() > 0) {
    //        _documentoComplementar.NotFis.visible(true);
    //    }
    //    callback();
    //});
}

function PreencherGridPreCTesComplementar() {
    loadDropZonePreCTeOcorrencia();
    _gridCargaPreCTeOcorrencia.CarregarGrid(function () {
        _documentoComplementar.PreCTesComplementares.visible(true);
        _documentoComplementar.CTesComplementares.visible(false);
    });
}

function baixarPreCTeOcorrenciaClick(e) {
    var data = { CodigoPreCTE: e.CodigoPreCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaPreCTe/DownloadPreXML", data);
}

function baixarNotfis(e) {
    var data = { Codigo: e.Codigo };
    executarDownload("ControleGeracaoEDI/DownloadArquivoEDI", data);
}