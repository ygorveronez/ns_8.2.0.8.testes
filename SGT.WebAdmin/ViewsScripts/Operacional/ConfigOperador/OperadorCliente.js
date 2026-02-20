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

var _operadorCliente;
var _gridCliente;

var OperadorCliente = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Operacional.ConfigOperador.Cliente.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarOperadorClienteClick, type: types.event, text: Localization.Resources.Operacional.ConfigOperador.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadOperadorClientes() {
    _operadorCliente = new OperadorCliente();
    KoBindings(_operadorCliente, "knockoutOperadorClientes");

    new BuscarClientes(_operadorCliente.Cliente);

    preencherOperadorClientes();
}

function adicionarOperadorClienteClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_operadorCliente);
    if (tudoCerto) {
        var existe = false;
        $.each(_operador.OperadorClientes.list, function (i, cliente) {
            if (cliente.Cliente.codEntity == _operadorCliente.Cliente.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _operador.OperadorClientes.list.push(SalvarListEntity(_operadorCliente));
            recarregarGridOperadorCliente();
            $("#" + _operadorCliente.Cliente.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Operacional.ConfigOperador.ClienteJaCadastrado, Localization.Resources.Operacional.ConfigOperador.ClienteXJaEstaCadastradoParaOperador.format(_operadorCliente.Cliente.val()));
        }
        limparOperadorCliente();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Operacional.ConfigOperador.CamposObrigatorios, Localization.Resources.Operacional.ConfigOperador.InformeCamposObrigatorios);
    }
}

function excluirOperadorCliente(e) {
    exibirConfirmacao(Localization.Resources.Operacional.ConfigOperador.Confirmacao, Localization.Resources.Operacional.ConfigOperador.RealmenteDesejaRemoverClienteX.format(e.Descricao), function () {
        $.each(_operador.OperadorClientes.list, function (i, cliente) {
            if (e.Codigo == cliente.Cliente.codEntity)
                _operador.OperadorClientes.list.splice(i, 1);
        });

        recarregarGridOperadorCliente();
    });
}

function preencherOperadorClientes() {

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: excluirOperadorCliente }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "90%" }
    ];

    _gridCliente = new BasicDataTable(_operadorCliente.Grid.id, header, menuOpcoes);
    recarregarGridOperadorCliente();
}

function recarregarGridOperadorCliente() {
    var data = new Array();
    $.each(_operador.OperadorClientes.list, function (i, cliente) {
        var operadorCliente = new Object();
        operadorCliente.Codigo = cliente.Cliente.codEntity;
        operadorCliente.Descricao = cliente.Cliente.val;
        data.push(operadorCliente);
    });
    _gridCliente.CarregarGrid(data);
}

function limparOperadorCliente() {
    LimparCampos(_operadorCliente);
}