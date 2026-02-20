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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="OperadorTipoOperacao.js" />
/// <reference path="ConfigOperador.js" />
/// <reference path="OperadorTipoCarga.js" />
/// <reference path="TabelaFrete.js" />
/// <reference path="OperadorModeloVeicularCarga.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _operadorFilial;
var _gridFilial;

var OperadorFilial = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Operacional.ConfigOperador.Filial.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarOperadorFilialClick, type: types.event, text: Localization.Resources.Operacional.ConfigOperador.Adicionar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadOperadorFiliais() {
    _operadorFilial = new OperadorFilial();
    KoBindings(_operadorFilial, "knockoutOperadorFiliais");
    new BuscarFilial(_operadorFilial.Filial, undefined, undefined, false);

    preencherOperadorFiliais();

}

function adicionarOperadorFilialClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_operadorFilial);
    if (tudoCerto) {
        var existe = false;
        $.each(_operador.OperadorFiliais.list, function (i, filial) {
            if (filial.Filial.codEntity == _operadorFilial.Filial.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _operador.OperadorFiliais.list.push(SalvarListEntity(_operadorFilial));
            recarregarGridOperadorFilial();
            $("#" + _operadorFilial.Filial.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Operacional.ConfigOperador.FilialJaCadastrada, Localization.Resources.Operacional.ConfigOperador.FilialXJaEstaCadastradaParaOperador.format(_operadorFilial.Filial.val()));
        }
        limparOperadorFilial();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Operacional.ConfigOperador.CamposObrigatorios, Localization.Resources.Operacional.ConfigOperador.InformeCamposObrigatorios);
    }
}

function excluirOperadorFilial(e) {

    exibirConfirmacao(Localization.Resources.Operacional.ConfigOperador.Confirmacao, Localization.Resources.Operacional.ConfigOperador.RealmenteDesejaRemoverFilialXParaOperador.format(e.Descricao), function () {
        $.each(_operador.OperadorFiliais.list, function (i, filial) {
            if (e.Codigo == filial.Filial.codEntity)
                _operador.OperadorFiliais.list.splice(i, 1);
        });

        recarregarGridOperadorFilial();
    });
}

function preencherOperadorFiliais() {

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: excluirOperadorFilial }] };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "90%" }];

    _gridFilial = new BasicDataTable(_operadorFilial.Grid.id, header, menuOpcoes);
    recarregarGridOperadorFilial();
}


function recarregarGridOperadorFilial() {
    var data = new Array();
    $.each(_operador.OperadorFiliais.list, function (i, filial) {
        var operadorFilial = new Object();
        operadorFilial.Codigo = filial.Filial.codEntity;
        operadorFilial.Descricao = filial.Filial.val;
        data.push(operadorFilial);
    });
    _gridFilial.CarregarGrid(data);
}

function limparOperadorFilial() {
    LimparCampos(_operadorFilial);
}