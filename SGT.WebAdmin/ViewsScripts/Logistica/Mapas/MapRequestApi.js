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



//*******MAPEAMENTO KNOUCKOUT*******


function buscarRouteViaMapRequestAPI(origem, destino, callback) {
    var data = { origem: origem, destino: destino };
    executarReST("MapRequestApi/Pesquisa", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                callback(arg.Data != false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//{ Localidade: "" } : Exemplo de um objeto no array localidadesBuscaAPI;
function buscarPassagensEntreEstadosViaMapRequestAPI(localidadesBuscaAPI, callback) {
    var data = { localidades: JSON.stringify(localidadesBuscaAPI) };
    executarReST("MapRequestApi/BuscarPassagemEntreEstados", data, function (arg) {
        if (arg.Success) {
            if (Object.prototype.toString.call(arg.Data) === "[object Array]") {
                callback(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}