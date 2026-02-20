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
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Localidade.js" />

var _gridTransportador;
var _pesquisaTransportador;

var PesquisaTransportador = function () {

    this.Nome = PropertyEntity({ text: "Razão Social: " });
    this.NomeFantasia = PropertyEntity({ text: "Nome Fantasia: " });
    this.CNPJ = PropertyEntity({ text: ko.observable("CNPJ: "), maxlength: 20, getType: typesKnockout.cnpj });
    this.Placa = PropertyEntity({ text: "Placa: ", maxlength: 7 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTransportador.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

function CarregarTransportador() {
    var menuOpcoes = new Object();
    menuOpcoes.opcoes = new Array();

    menuOpcoes.tipo = TypeOptionMenu.list;

    menuOpcoes.opcoes.push({ descricao: "Acessar Sistema", id: "optnAcessarSistema", evento: "onclick", metodo: AcessarSistema, tamanho: "15", icone: "" });

    _gridTransportador = new GridView(_pesquisaTransportador.Pesquisar.idGrid, "GerenciarTransportadores/Consultar", _pesquisaTransportador, menuOpcoes, { column: 1, dir: orderDir.desc }, 10, null);
    _gridTransportador.CarregarGrid();
}

function loadTransportador() {
    _pesquisaTransportador = new PesquisaTransportador();
    KoBindings(_pesquisaTransportador, "knockoutPesquisaTransportadores", false, _pesquisaTransportador.Pesquisar.id);

    CarregarTransportador();

    $("#" + _pesquisaTransportador.Nome.id).focus();
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin) {
        $("#divTituloPrincipal").text("Gerenciar Empresas");
        $("#divTituloPesquisa").text("Pesquisar Empresas");
        $("#divTituloCadastro").text("Empresas");

        $("#" + _pesquisaTransportador.CNPJ.id).unmask();
        _pesquisaTransportador.CNPJ.text("CNPJ/CPF");
        _pesquisaTransportador.CNPJ.getType = typesKnockout.cpfCnpj;
        $("#" + _pesquisaTransportador.CNPJ.id).mask("00000000000999", { selectOnFocus: true, clearIfNotMatch: true });
    }
}

function AcessarSistema(e) {
    executarReST("GerenciarTransportadores/BuscarDadosParaAcesso?callback=?", { CodigoEmpresa: e.Codigo }, function (r) {
        if (r.Success) {
            var win = window.open();

            var uriAcesso = r.Data.UriAcesso + "?x=" + r.Data.Login + "&y=" + r.Data.Senha + "&z=" + r.Data.Usuario;

            win.location = uriAcesso;
            win.focus();
        } else {
            exibirMensagem(tipoMensagem.falha, "Atenção", r.Msg);
        }
    });
}