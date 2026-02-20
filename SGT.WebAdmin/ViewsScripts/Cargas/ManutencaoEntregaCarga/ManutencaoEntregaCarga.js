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
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _manutencaoEntregaCarga;
var _gridManutencaoEntregaCarga;

var ManutencaoEntregaCarga = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Carga:", idBtnSearch: guid(), required: ko.observable(true) });

    this.Grid = PropertyEntity({ idGrid: guid() });

    this.Pesquisa = PropertyEntity({ eventClick: pesquisarEntregaCargaClick, type: types.event, text: "Pesquisar" });
};

//*******EVENTOS*******

function loadManutencaoEntregaCarga() {
    _manutencaoEntregaCarga = new ManutencaoEntregaCarga();
    KoBindings(_manutencaoEntregaCarga, "knockoutManutencaoEntregaCarga");

    new BuscarCargas(_manutencaoEntregaCarga.Carga, null, null, null, null, [EnumSituacoesCarga.Cancelada, EnumSituacoesCarga.EmCancelamento, EnumSituacoesCarga.Anulada]);

    montarGridEntregas();
}

function pesquisarEntregaCargaClick() {
    var valido = ValidarCamposObrigatorios(_manutencaoEntregaCarga);
    if (valido) {
        _gridManutencaoEntregaCarga.CarregarGrid();
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function callbackEditarColuna(cargaEntrega) {
    var dados = {
        Codigo: cargaEntrega.Codigo,
        DataEntradaRaio: cargaEntrega.DataEntradaRaio,
        DataSaidaRaio: cargaEntrega.DataSaidaRaio
    };
    executarReST("ManutencaoEntregaCarga/AtualizarEntregaCarga", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro alterado com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function montarGridEntregas() {
    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: true };

    _gridManutencaoEntregaCarga = new GridView(_manutencaoEntregaCarga.Grid.idGrid, "ManutencaoEntregaCarga/Pesquisa", _manutencaoEntregaCarga, null, { column: 1, dir: orderDir.asc }, 10, null, null, null, null, null, editarColuna);
}