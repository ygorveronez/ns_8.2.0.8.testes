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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="ConfigOperador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _tomadorGestaoDocumentos;
var _gridTomador;

var TomadorGestaoDocumentos = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Operacional.ConfigOperador.Tomador.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarTomadorClick, type: types.event, text: Localization.Resources.Operacional.ConfigOperador.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadTomadoresGestaoDocumentos() {
    _tomadorGestaoDocumentos = new TomadorGestaoDocumentos();
    KoBindings(_tomadorGestaoDocumentos, "knockoutTomadoresGestaoDocumentos");

    new BuscarClientes(_tomadorGestaoDocumentos.Tomador);

    preencherTomadoresGestaoDocumentos();
}

function adicionarTomadorClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_tomadorGestaoDocumentos);
    if (tudoCerto) {
        var existe = false;
        $.each(_operador.OperadorTomadores.list, function (i, cliente) {
            if (cliente.Tomador.codEntity == _tomadorGestaoDocumentos.Tomador.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _operador.OperadorTomadores.list.push(SalvarListEntity(_tomadorGestaoDocumentos));
            recarregarGridOperadorTomador();
            $("#" + _tomadorGestaoDocumentos.Tomador.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Operacional.ConfigOperador.TomadorJaCadastrado, Localization.Resources.Operacional.ConfigOperador.TomadorXJaCadastradoParaOperador.format(_tomadorGestaoDocumentos.Tomador.val()));
        }
        limparOperadorTomador();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Operacional.ConfigOperador.CamposObrigatorios, Localization.Resources.Operacional.ConfigOperador.InformeCamposObrigatorios);
    }
}

function excluirOperadorTomador(e) {
    exibirConfirmacao(Localization.Resources.Operacional.ConfigOperador.Confirmacao, Localization.Resources.Operacional.ConfigOperador.RealmenteDesejaRemoverTomadorX.format(e.Descricao), function () {
        $.each(_operador.OperadorTomadores.list, function (i, cliente) {
            if (e.Codigo == cliente.Tomador.codEntity)
                _operador.OperadorTomadores.list.splice(i, 1);
        });

        recarregarGridOperadorTomador();
    });
}

function preencherTomadoresGestaoDocumentos() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: excluirOperadorTomador }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "90%" }
    ];

    _gridTomador = new BasicDataTable(_tomadorGestaoDocumentos.Grid.id, header, menuOpcoes);
    recarregarGridOperadorTomador();
}

function recarregarGridOperadorTomador() {
    var data = new Array();
    $.each(_operador.OperadorTomadores.list, function (i, tomador) {
        var operadorTomador = new Object();
        operadorTomador.Codigo = tomador.Tomador.codEntity;
        operadorTomador.Descricao = tomador.Tomador.val;
        data.push(operadorTomador);
    });
    _gridTomador.CarregarGrid(data);
}

function limparOperadorTomador() {
    LimparCampos(_tomadorGestaoDocumentos);
}