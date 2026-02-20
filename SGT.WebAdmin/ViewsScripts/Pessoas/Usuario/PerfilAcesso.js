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


//*******EVENTOS*******
function loadPerfilAcesso() {

}

//*******MÉTODOS*******

function PerfilAcessoOnChange(itemGrid) {
    executarReST("PerfilAcesso/BuscarPorPerfil", itemGrid, function (e) {
        if (e.Success) {
            if (e.Data) {
                _usuario.UsuarioAdministrador.val(false);
                limparPermissoesModulosFormularios();
                PreencherObjetoKnout(_usuario, e);
                setarPermissoesModulosFormularios();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, e.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
        }
    });
}