/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/bootstrap/bootstrap.js" />
/// <reference path="../../js/libs/jquery.blockui.js" />
/// <reference path="../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="AlertaLicenca.js" />
/// <reference path="Fornecedor.js" />
/// <reference path="ArrumarIssoPqTavaDiretoNaPagina.js" />
/// <reference path="MensagemAviso.js" />
/// <reference path="ConfiguracaoCertificado.js" />
/// <reference path="DashboardDocumentacao.js" />
/// <reference path="ModalMensagemAviso.js" />

//var _gridAtividadesPendentes;

function LoadHome() {
    LoadMensagemAviso();
    LoadHomeFornecedor();
    LoadAlertaLicencaTMS();

    if (_CONFIGURACAO_TMS.NovoLayoutCabotagem) {
        LoadDashboardDocumentacao();
        LoadAcessoRapidoModulosFavoritosUsuario();
    }
    else
        ArrumarIssoPqTavaDiretoNaPagina();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        $.get("Content/Static/Home/ModalMensagemAviso.html?dyn=" + guid(), (data) => {
            $('#divModalAvisoMensagem').html(data);
            LoadModalMensagemAviso();
        });
    }
}