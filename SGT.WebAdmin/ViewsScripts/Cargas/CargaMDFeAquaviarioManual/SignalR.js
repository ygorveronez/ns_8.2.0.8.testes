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
/// <reference path="Cargas.js" />
/// <reference path="CTes.js" />
/// <reference path="Etapas.js" />
/// <reference path="Impressao.js" />
/// <reference path="MDFe.js" />
/// <reference path="Terminais.js" />
/// <reference path="CargaMDFeAquaviarioManual.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />


function LoadConexaoSignalRCargaMDFeAquaviario() {
    SignalRCargaMDFeAquaviarioAlteradoEvent = VerificarCargaMDFeAquaviarioAlteradaEvent;
}

function VerificarCargaMDFeAquaviarioAlteradaEvent(retorno) {
    if (retorno.CodigoCargaMDFeAquaviario == _cargaMDFeAquaviario.Codigo.val()) {
        _RequisicaoIniciada = true;
        BuscarCargaMDFeAquaviarioPorCodigo(function () {
            _RequisicaoIniciada = false;
        });
    }
}