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
/// <reference path="Pessoa.js" />
/// <reference path="../../Consultas/Licenca.js" />
/// <reference path="../../Enumeradores/EnumStatusLicenca.js" />

var _pessoaLicencas;
var _gridPessoaLicencas;

//*******MAPEAMENTO KNOUCKOUT*******

var PessoaLicencaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.Numero = PropertyEntity({ type: types.map, val: "" });
    this.DataEmissao = PropertyEntity({ type: types.map, val: "" });
    this.DataVencimento = PropertyEntity({ type: types.map, val: "" });
    this.FormaAlerta = PropertyEntity({ type: types.map, val: "" });
    this.Status = PropertyEntity({ type: types.map, val: "" });
    this.CodigoLicenca = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoLicenca = PropertyEntity({ type: types.map, val: "" });
};

var PessoaLicenca = function () {
    this.CodigoLicenca = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Licenca = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Pessoas.Pessoa.Licenca), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), maxlength: 200});
    this.Numero = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.NumeroLicenca.getRequiredFieldDescription(), maxlength: 20});
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Pessoas.Pessoa.DataEmissao.getRequiredFieldDescription() });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Pessoas.Pessoa.DataVencimento.getRequiredFieldDescription() });
    this.FormaAlerta = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.GerarAlertaAoResponsavel.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumControleAlertaForma.obterOpcoes(), def: [] });
    this.StatusLicenca = PropertyEntity({ val: ko.observable(EnumStatusLicenca.Vigente), options: EnumStatusLicenca.obterOpcoes(), def: EnumStatusLicenca.Vigente, text: Localization.Resources.Pessoas.Pessoa.Status.getRequiredFieldDescription() });
    this.GridPessoaLicencas = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AdicionarLicenca = PropertyEntity({ eventClick: adicionarPessoaLicencaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarLicenca = PropertyEntity({ eventClick: atualizarPessoaLicencaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirLicenca = PropertyEntity({ eventClick: excluirPessoaLicencaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarLicenca = PropertyEntity({ eventClick: LimparCamposPessoaLicencas, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
};

//*******EVENTOS*******

function loadPessoaLicenca() {
    _pessoaLicencas = new PessoaLicenca();
    KoBindings(_pessoaLicencas, "knockoutLicenca");

    new BuscarLicenca(_pessoaLicencas.Licenca);

    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("PessoaLicenca"), icone: "", visibilidade: true };
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarPessoaLicenca, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar, auditar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoLicenca", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "Numero", title: Localization.Resources.Pessoas.Pessoa.Numero, width: "15%", className: "text-align-left" },
        { data: "DataEmissao", title: Localization.Resources.Pessoas.Pessoa.DataEmissao, width: "15%", className: "text-align-center" },
        { data: "DataVencimento", title: Localization.Resources.Pessoas.Pessoa.DataVencimento, width: "15%", className: "text-align-center" },
        { data: "DescricaoLicenca", title: Localization.Resources.Pessoas.Pessoa.Licenca, width: "20%", className: "text-align-left" },
        { data: "FormaAlerta", visible: false },
        { data: "Status", visible: false },
    ];

    _gridPessoaLicencas = new BasicDataTable(_pessoaLicencas.GridPessoaLicencas.idGrid, header, menuOpcoes);
    recarregarGridPessoaLicencas();
}

function adicionarPessoaLicencaClick() {
    var tudoCerto = true;
    if (_pessoaLicencas.Descricao.val() == "")
        tudoCerto = false;
    if (_pessoaLicencas.Numero.val() == "")
        tudoCerto = false;
    if (_pessoaLicencas.DataEmissao.val() == "")
        tudoCerto = false;
    if (_pessoaLicencas.DataVencimento.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        var existe = false;
        if (!existe) {
            var pessoaLicenca = new PessoaLicencaMap();
            pessoaLicenca.Codigo.val = guid();
            pessoaLicenca.Descricao.val = _pessoaLicencas.Descricao.val();
            pessoaLicenca.Numero.val = _pessoaLicencas.Numero.val();
            pessoaLicenca.DataEmissao.val = _pessoaLicencas.DataEmissao.val();
            pessoaLicenca.DataVencimento.val = _pessoaLicencas.DataVencimento.val();
            pessoaLicenca.Status.val = _pessoaLicencas.StatusLicenca.val();
            pessoaLicenca.FormaAlerta.val = JSON.stringify(_pessoaLicencas.FormaAlerta.val());
            pessoaLicenca.DescricaoLicenca.val = _pessoaLicencas.Licenca.val();
            pessoaLicenca.CodigoLicenca.val = _pessoaLicencas.Licenca.codEntity();

            _pessoa.ListaLicencas.list.push(pessoaLicenca);
            recarregarGridPessoaLicencas();
            $("#" + _pessoaLicencas.Descricao.id).focus();
        }
        LimparCamposPessoaLicencas();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarPessoaLicencaClick() {
    var tudoCerto = true;
    if (_pessoaLicencas.Descricao.val() == "")
        tudoCerto = false;
    if (_pessoaLicencas.Numero.val() == "")
        tudoCerto = false;
    if (_pessoaLicencas.DataEmissao.val() == "")
        tudoCerto = false;
    if (_pessoaLicencas.DataVencimento.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        $.each(_pessoa.ListaLicencas.list, function (i, pessoaLicenca) {
            if (pessoaLicenca.Codigo.val == _pessoaLicencas.CodigoLicenca.val()) {

                pessoaLicenca.Numero.val = _pessoaLicencas.Numero.val();
                pessoaLicenca.Descricao.val = _pessoaLicencas.Descricao.val();
                pessoaLicenca.DataEmissao.val = _pessoaLicencas.DataEmissao.val();
                pessoaLicenca.DataVencimento.val = _pessoaLicencas.DataVencimento.val();
                pessoaLicenca.Status.val = _pessoaLicencas.StatusLicenca.val();
                pessoaLicenca.FormaAlerta.val = JSON.stringify(_pessoaLicencas.FormaAlerta.val());
                pessoaLicenca.DescricaoLicenca.val = _pessoaLicencas.Licenca.val();
                pessoaLicenca.CodigoLicenca.val = _pessoaLicencas.Licenca.codEntity();

                return false;
            }
        });
        recarregarGridPessoaLicencas();
        LimparCamposPessoaLicencas();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirPessoaLicencaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Pessoa.RealmenteDesejaExcluirLicencaSelecionada, function () {
        var listaAtualizada = new Array();
        $.each(_pessoa.ListaLicencas.list, function (i, pessoaLicenca) {
            if (pessoaLicenca.Codigo.val != _pessoaLicencas.CodigoLicenca.val()) {
                listaAtualizada.push(pessoaLicenca);
            }
        });
        _pessoa.ListaLicencas.list = listaAtualizada;
        recarregarGridPessoaLicencas();
        LimparCamposPessoaLicencas();
    });
}

//*******MÉTODOS*******

function recarregarGridPessoaLicencas() {
    var data = new Array();
    $.each(_pessoa.ListaLicencas.list, function (i, pessoa) {
        var pessoaLicenca = new Object();

        pessoaLicenca.Codigo = pessoa.Codigo.val;
        pessoaLicenca.Descricao = pessoa.Descricao.val;
        pessoaLicenca.Numero = pessoa.Numero.val;
        pessoaLicenca.DataEmissao = pessoa.DataEmissao.val;
        pessoaLicenca.DataVencimento = pessoa.DataVencimento.val;
        pessoaLicenca.Status = pessoa.Status.val;
        pessoaLicenca.FormaAlerta = pessoa.FormaAlerta.val;
        pessoaLicenca.CodigoLicenca = pessoa.CodigoLicenca.val;
        pessoaLicenca.DescricaoLicenca = pessoa.DescricaoLicenca.val;

        data.push(pessoaLicenca);
    });
    _gridPessoaLicencas.CarregarGrid(data);
}

function editarPessoaLicenca(data) {
    LimparCamposPessoaLicencas();
    $.each(_pessoa.ListaLicencas.list, function (i, pessoaLicenca) {
        if (pessoaLicenca.Codigo.val == data.Codigo) {
            _pessoaLicencas.CodigoLicenca.val(pessoaLicenca.Codigo.val);
            _pessoaLicencas.Descricao.val(pessoaLicenca.Descricao.val);
            _pessoaLicencas.Numero.val(pessoaLicenca.Numero.val);
            _pessoaLicencas.DataEmissao.val(pessoaLicenca.DataEmissao.val);
            _pessoaLicencas.DataVencimento.val(pessoaLicenca.DataVencimento.val);
            _pessoaLicencas.StatusLicenca.val(pessoaLicenca.Status.val);
            _pessoaLicencas.FormaAlerta.val(JSON.parse(pessoaLicenca.FormaAlerta.val));
            _pessoaLicencas.Licenca.val(pessoaLicenca.DescricaoLicenca.val);
            _pessoaLicencas.Licenca.codEntity(pessoaLicenca.CodigoLicenca.codEntity);

            $("#" + _pessoaLicencas.FormaAlerta.id).trigger('change');

            return false;
        }
    });

    _pessoaLicencas.AdicionarLicenca.visible(false);
    _pessoaLicencas.AtualizarLicenca.visible(true);
    _pessoaLicencas.ExcluirLicenca.visible(true);
    _pessoaLicencas.CancelarLicenca.visible(true);
}

function LimparCamposPessoaLicencas() {
    _pessoaLicencas.Descricao.val("");
    _pessoaLicencas.Numero.val("");
    _pessoaLicencas.DataEmissao.val("");
    _pessoaLicencas.DataVencimento.val("");
    _pessoaLicencas.StatusLicenca.val(EnumStatusLicenca.Vigente);
    _pessoaLicencas.FormaAlerta.val(new Array());
    LimparCampoEntity(_pessoaLicencas.Licenca);
    LimparCampo(_pessoaLicencas.FormaAlerta);

    _pessoaLicencas.AdicionarLicenca.visible(true);
    _pessoaLicencas.AtualizarLicenca.visible(false);
    _pessoaLicencas.ExcluirLicenca.visible(false);
    _pessoaLicencas.CancelarLicenca.visible(false);
}