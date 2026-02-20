/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="Pessoa.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _areaRedex, _gridAreasRedex;

var AreaRedex = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.ClienteRedex = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ClienteRedex.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoOperacaoRedespacho = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.OperacaoRedespachoAreaRedex, type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Adicionar, eventClick: adicionarClienteRedex, type: types.event, visible: ko.observable(true) });
};

function loadAreaRedex() {
    _areaRedex = new AreaRedex();
    KoBindings(_areaRedex, "knockoutAreasRedex");

    habilitarAreaRedex();

    new BuscarClientes(_areaRedex.ClienteRedex, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    new BuscarTiposOperacao(_areaRedex.TipoOperacaoRedespacho);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirAreaRedex }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "ClienteRedex", title: Localization.Resources.Pessoas.Pessoa.ClienteRedex, width: "90%" }
    ];

    _gridAreasRedex = new BasicDataTable(_areaRedex.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridAreaRedex();
}

function recarregarGridAreaRedex() {
    var data = new Array();
    $.each(_pessoa.AreasRedex.list, function (i, areaRedex) {
        var gridAreaRedex = new Object();

        gridAreaRedex.Codigo = 0;
        gridAreaRedex.ClienteRedex = areaRedex.ClienteRedex.val;

        data.push(gridAreaRedex);
    });

    _gridAreasRedex.CarregarGrid(data);
}

function excluirAreaRedex(data) {
    $.each(_pessoa.AreasRedex.list, function (i, areaRedex) {
        if (data.ClienteRedex == areaRedex.ClienteRedex.val) {
            _pessoa.AreasRedex.list.splice(i, 1);
            return false;
        }
    });

    recarregarGridAreaRedex();
}

function adicionarClienteRedex(e, sender) {
    var valido = true;

    valido = _areaRedex.ClienteRedex.codEntity() > 0;
    _areaRedex.ClienteRedex.requiredClass("form-control");

    if (valido) {
        var existe = false;
        $.each(_pessoa.AreasRedex.list, function (i, areaRedex) {
            if (areaRedex.ClienteRedex.val == _areaRedex.ClienteRedex.val()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Pessoa.AreaRedexJaInformada, Localization.Resources.Pessoas.Pessoa.AreaRedexJaEstaCadastrada.format(_areaRedex.ClienteRedex.val()));
            return;
        }

        _pessoa.AreasRedex.list.push(SalvarListEntity(_areaRedex));
        recarregarGridAreaRedex();
        $("#" + _areaRedex.ClienteRedex.id).focus();
        limparCamposAreaRedex();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        _areaRedex.ClienteRedex.requiredClass("form-control is-invalid");
    }
}

function limparCamposAreaRedex() {
    LimparCampos(_areaRedex);
    _areaRedex.ClienteRedex.requiredClass("form-control");
    $("#liTabAreasRedex").hide();
}

function preencherAreaRedex(dados) {
    PreencherObjetoKnout(_areaRedex, { Data: dados.AreaRedex });
}

function habilitarAreaRedex2(dados) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador)
        return;

    if (dados.PossuiAreaRedex && !_pessoaAdicional.AreaRedex.val()) {
        _areaRedex.TipoOperacaoRedespacho.codEntity(dados.CodigoOperacaoRedespachoAreaRedex);
        _areaRedex.TipoOperacaoRedespacho.val(dados.OperacaoRedespachoAreaRedex);

        $("#liTabAreasRedex").show();
    } else
        $("#liTabAreasRedex").hide();
}

function habilitarAreaRedex() {
    if (_CONFIGURACAO_TMS.PossuiAreaRedex && _pessoaAdicional.AreaRedex.val())
        $("#liTabAreasRedex").show();
    else
        $("#liTabAreasRedex").hide();
}

function desabilitarAreaRedex() {
    if (!_pessoaAdicional.AreaRedex.val())
        $("#liTabAreasRedex").hide();
    else
        $("#liTabAreasRedex").show();
}