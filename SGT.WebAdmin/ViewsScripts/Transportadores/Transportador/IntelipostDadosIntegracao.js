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
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridIntelipostDadosIntegracao;
var _intelipostDadosIntegracao;

var IntelipostDadosIntegracao = function () {

    this.Grid = PropertyEntity({ type: types.local });
    this.Token = PropertyEntity({ visible: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.Token.getRequiredFieldDescription(), val: ko.observable(""), required: true });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Transportadores.Transportador.CanalEntrega.getFieldDescription(), issue: 121, visible: ko.observable(true), idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarIntelipostDadosIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadIntelipostDadosIntegracao() {

    _intelipostDadosIntegracao = new IntelipostDadosIntegracao();
    KoBindings(_intelipostDadosIntegracao, "knockoutIntelipostDadosIntegracao");

    new BuscarCanaisEntrega(_intelipostDadosIntegracao.CanalEntrega, null);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirIntelipostDadosIntegracaoClick }] };

    var header = [
        { data: "CodigoCanalEntrega", visible: false },
        { data: "Token", title: Localization.Resources.Transportadores.Transportador.Token, width: "50%" },
        { data: "CanalEntrega", title: Localization.Resources.Transportadores.Transportador.CanalEntrega, width: "30%" }
    ];

    _gridIntelipostDadosIntegracao = new BasicDataTable(_intelipostDadosIntegracao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridIntelipostDadosIntegracao();
}

function recarregarGridIntelipostDadosIntegracao() {
    var data = new Array();

    $.each(_transportador.IntelipostDadosIntegracao.list, function (i, IntelipostDadosIntegracao) {
        var IntelipostDadosIntegracaoGrid = new Object();

        IntelipostDadosIntegracaoGrid.Token = IntelipostDadosIntegracao.Token.val;
        IntelipostDadosIntegracaoGrid.CodigoCanalEntrega = IntelipostDadosIntegracao.CanalEntrega.codEntity;
        IntelipostDadosIntegracaoGrid.CanalEntrega = IntelipostDadosIntegracao.CanalEntrega.val;

        data.push(IntelipostDadosIntegracaoGrid);
    });

    _gridIntelipostDadosIntegracao.CarregarGrid(data);
}

function excluirIntelipostDadosIntegracaoClick(data) {
    for (var i = 0; i < _transportador.IntelipostDadosIntegracao.list.length; i++) {
        IntelipostDadosIntegracaoExcluir = _transportador.IntelipostDadosIntegracao.list[i];
        if (data.Token == IntelipostDadosIntegracaoExcluir.Token.val && data.CodigoCanalEntrega == IntelipostDadosIntegracaoExcluir.CanalEntrega.codEntity)
            _transportador.IntelipostDadosIntegracao.list.splice(i, 1);
    }
    recarregarGridIntelipostDadosIntegracao();
}

function adicionarIntelipostDadosIntegracaoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_intelipostDadosIntegracao);

    if (valido) {
        var existe = false;
        $.each(_transportador.IntelipostDadosIntegracao.list, function (i, IntelipostDadosIntegracao) {

            if (IntelipostDadosIntegracao.Token.val == _intelipostDadosIntegracao.Token.val() && IntelipostDadosIntegracao.CanalEntrega.codEntity == _intelipostDadosIntegracao.CanalEntrega.codEntity()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.TokenParaEsteCanalEntregaExiste);
            return;
        }

        _transportador.IntelipostDadosIntegracao.list.push(SalvarListEntity(_intelipostDadosIntegracao));

        recarregarGridIntelipostDadosIntegracao();

        $("#" + _intelipostDadosIntegracao.CanalEntrega.id).focus();

        limparCamposIntelipostDadosIntegracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function limparCamposIntelipostDadosIntegracao() {
    LimparCampos(_intelipostDadosIntegracao);
}