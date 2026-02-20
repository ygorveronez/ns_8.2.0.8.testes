/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />

//*******MAPEAMENTO*******

//*******EVENTOS*******

function buscarCargasMICDTA() {
    var download = { descricao: "Download MIC/DTA", id: guid(), metodo: downloadMICDTA, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [download] };

    _gridCargaMICDTA = new GridView(_cargaCTe.PesquisarMICDTA.idGrid, "CargaCTe/ConsultarCargaMICDTA", _cargaCTe, menuOpcoes);
    _gridCargaMICDTA.CarregarGrid(callbackGridMICDTA);
}

function callbackGridMICDTA() {
    if (_gridCargaMICDTA.NumeroRegistros() > 0)
        $("#tabMICDTA_" + _cargaAtual.DadosCTes.id + "_li").show();
    else
        $("#tabMICDTA_" + _cargaAtual.DadosCTes.id + "_li").hide();
}

//*******METODOS*******

function downloadMICDTA(datagrid) {
    executarDownload("CargaCTe/DownloadMicDta", { Codigo: datagrid.Codigo });
}