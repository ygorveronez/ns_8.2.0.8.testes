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
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _inscricaoST;
var _gridInscricaoST;

var InscricaoST = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.Estado.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.InscricaoEstadual = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.IESubstTrib.getRequiredFieldDescription(), issue: 745, maxlength: 20, required: true });

    this.Grid = PropertyEntity({ type: types.local });
    this.Adicionar = PropertyEntity({ eventClick: adicionarInscricaoSTClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadInscricaoST() {
    _inscricaoST = new InscricaoST();
    KoBindings(_inscricaoST, "tabInscricaoST");

    new BuscarEstados(_inscricaoST.Estado);

    loadGridInscricaoST();
}

function loadGridInscricaoST() {

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirInscricaoSTClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Estado", title: Localization.Resources.Transportadores.Transportador.Estado, width: "50%" },
        { data: "InscricaoEstadual", title: Localization.Resources.Transportadores.Transportador.IESubstTrib, width: "50%" }
    ];

    _gridInscricaoST = new BasicDataTable(_inscricaoST.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridInscricaoST();
}

function recarregarGridInscricaoST() {
    var data = new Array();

    $.each(_transportador.InscricoesST.list, function (i, inscricaoST) {
        var inscricaoSTGrid = new Object();

        inscricaoSTGrid.Codigo = inscricaoST.Codigo.val;
        inscricaoSTGrid.Estado = inscricaoST.Estado.val;
        inscricaoSTGrid.InscricaoEstadual = inscricaoST.InscricaoEstadual.val;

        data.push(inscricaoSTGrid);
    });

    _gridInscricaoST.CarregarGrid(data);
}

function excluirInscricaoSTClick(data) {
    for (var i = 0; i < _transportador.InscricoesST.list.length; i++) {
        inscricaoSTExcluir = _transportador.InscricoesST.list[i];
        if (data.Codigo == inscricaoSTExcluir.Codigo.val)
            _transportador.InscricoesST.list.splice(i, 1);
    }

    recarregarGridInscricaoST();
}

function adicionarInscricaoSTClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_inscricaoST);
    if (!valido)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);

    var existe = false;
    $.each(_transportador.InscricoesST.list, function (i, inscricaoST) {
        if (inscricaoST.Estado.codEntity == _inscricaoST.Estado.codEntity()) {
            existe = true;
            return;
        }
    });

    if (existe)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.EstadoExistente, Localization.Resources.Transportadores.Transportador.EstadoAdicionadoLista.format(_inscricaoST.Estado.val()));

    _inscricaoST.Codigo.val(guid());
    _transportador.InscricoesST.list.push(SalvarListEntity(_inscricaoST));

    $("#" + _inscricaoST.Estado.id).focus();

    limparCamposInscricaoST();
}

function limparCamposInscricaoST() {
    LimparCampos(_inscricaoST);
    recarregarGridInscricaoST();
}