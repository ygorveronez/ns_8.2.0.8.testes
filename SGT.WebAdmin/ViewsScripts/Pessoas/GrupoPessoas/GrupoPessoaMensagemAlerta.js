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
/// <reference path="GrupoPessoas.js" />

var _grupoPessoasMensagemAlerta;
var _gridGrupoPessoasMensagemAlerta;

//*******MAPEAMENTO KNOUCKOUT*******

var GrupoPessoasMensagemAlertaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Tag = PropertyEntity({ type: types.map, val: "" });
    this.MensagemAlerta = PropertyEntity({ type: types.map, val: "" });
}

var GrupoPessoasMensagemAlerta = function () {
    this.CodigoMensagemAlerta = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Tag = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.TagNFe.getRequiredFieldDescription(), getType: typesKnockout.string, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.MensagemAlerta = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.MensagemAlerta.getRequiredFieldDescription(), getType: typesKnockout.string, required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.GridGrupoPessoasMensagemAlerta = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AdicionarAlerta = PropertyEntity({ eventClick: adicionarGrupoPessoasMensagemAlertaClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarAlerta = PropertyEntity({ eventClick: atualizarGrupoPessoasMensagemAlertaClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirAlerta = PropertyEntity({ eventClick: excluirGrupoPessoasMensagemAlertaClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarAlerta = PropertyEntity({ eventClick: LimparCamposGrupoPessoasMensagemAlerta, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
}

//*******EVENTOS*******

function loadGrupoPessoasMensagemAlerta() {
    _grupoPessoasMensagemAlerta = new GrupoPessoasMensagemAlerta();
    KoBindings(_grupoPessoasMensagemAlerta, "knockoutMensagemAlerta");    

    var editar = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), evento: "onclick", metodo: editarGrupoPessoasMensagemAlerta, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "Tag", title: "Tag", width: "20%", className: "text-align-left" },
        { data: "MensagemAlerta", title: Localization.Resources.Pessoas.GrupoPessoas.Mensagem, width: "70%", className: "text-align-left" }
    ];

    _gridGrupoPessoasMensagemAlerta = new BasicDataTable(_grupoPessoasMensagemAlerta.GridGrupoPessoasMensagemAlerta.idGrid, header, menuOpcoes);
    recarregarGridGrupoPessoasMensagemAlerta();
}

function adicionarGrupoPessoasMensagemAlertaClick() {
    var valido = ValidarCamposObrigatorios(_grupoPessoasMensagemAlerta);
    
    if (valido) {

        var grupoPessoasMensagemAlerta = new GrupoPessoasMensagemAlertaMap();
        grupoPessoasMensagemAlerta.Codigo.val = guid();        
        grupoPessoasMensagemAlerta.Tag.val = _grupoPessoasMensagemAlerta.Tag.val();
        grupoPessoasMensagemAlerta.MensagemAlerta.val = _grupoPessoasMensagemAlerta.MensagemAlerta.val();

        _grupoPessoas.ListaMensagemAlerta.list.push(grupoPessoasMensagemAlerta);
        recarregarGridGrupoPessoasMensagemAlerta();
        $("#" + _grupoPessoasMensagemAlerta.Tag.id).focus();
        LimparCamposGrupoPessoasMensagemAlerta();
    } else {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
    }
}

function atualizarGrupoPessoasMensagemAlertaClick() {
    var valido = ValidarCamposObrigatorios(_grupoPessoasMensagemAlerta);
    if (valido) {
        $.each(_grupoPessoas.ListaMensagemAlerta.list, function (i, grupoPessoasMensagemAlerta) {
            if (grupoPessoasMensagemAlerta.Codigo.val == _grupoPessoasMensagemAlerta.CodigoMensagemAlerta.val()) {

                grupoPessoasMensagemAlerta.Tag.val = _grupoPessoasMensagemAlerta.Tag.val();
                grupoPessoasMensagemAlerta.MensagemAlerta.val = _grupoPessoasMensagemAlerta.MensagemAlerta.val();                

                return false;
            }
        });
        recarregarGridGrupoPessoasMensagemAlerta();
        LimparCamposGrupoPessoasMensagemAlerta();
    } else {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
    }
}

function excluirGrupoPessoasMensagemAlertaClick() {
    exibirConfirmacao(Localization.Resources.Pessoas.GrupoPessoas.Confirmacao, Localization.Resources.Pessoas.GrupoPessoas.RealmenteDesejaExcluirAlerta, function () {
        var listaAtualizada = new Array();
        $.each(_grupoPessoas.ListaMensagemAlerta.list, function (i, grupoPessoasMensagemAlerta) {
            if (grupoPessoasMensagemAlerta.Codigo.val != _grupoPessoasMensagemAlerta.CodigoMensagemAlerta.val()) {
                listaAtualizada.push(grupoPessoasMensagemAlerta);
            }
        });
        _grupoPessoas.ListaMensagemAlerta.list = listaAtualizada;
        recarregarGridGrupoPessoasMensagemAlerta();
        LimparCamposGrupoPessoasMensagemAlerta();
    });
}

//*******MÉTODOS*******

function recarregarGridGrupoPessoasMensagemAlerta() {
    var data = new Array();
    $.each(_grupoPessoas.ListaMensagemAlerta.list, function (i, grupoPessoas) {
        var grupoPessoasMensagemAlerta = new Object();

        grupoPessoasMensagemAlerta.Codigo = grupoPessoas.Codigo.val;
        grupoPessoasMensagemAlerta.Tag = grupoPessoas.Tag.val;
        grupoPessoasMensagemAlerta.MensagemAlerta = grupoPessoas.MensagemAlerta.val;

        data.push(grupoPessoasMensagemAlerta);
    });
    _gridGrupoPessoasMensagemAlerta.CarregarGrid(data);
}

function editarGrupoPessoasMensagemAlerta(data) {
    LimparCamposGrupoPessoasMensagemAlerta();
    $.each(_grupoPessoas.ListaMensagemAlerta.list, function (i, grupoPessoasMensagemAlerta) {
        if (grupoPessoasMensagemAlerta.Codigo.val == data.Codigo) {

            _grupoPessoasMensagemAlerta.CodigoMensagemAlerta.val(grupoPessoasMensagemAlerta.Codigo.val);            
            _grupoPessoasMensagemAlerta.Tag.val(grupoPessoasMensagemAlerta.Tag.val);
            _grupoPessoasMensagemAlerta.MensagemAlerta.val(grupoPessoasMensagemAlerta.MensagemAlerta.val);

            return false;
        }
    });

    _grupoPessoasMensagemAlerta.AdicionarAlerta.visible(false);
    _grupoPessoasMensagemAlerta.AtualizarAlerta.visible(true);
    _grupoPessoasMensagemAlerta.ExcluirAlerta.visible(true);
    _grupoPessoasMensagemAlerta.CancelarAlerta.visible(true);
}

function LimparCamposGrupoPessoasMensagemAlerta() {
    LimparCampos(_grupoPessoasMensagemAlerta);

    _grupoPessoasMensagemAlerta.AdicionarAlerta.visible(true);
    _grupoPessoasMensagemAlerta.AtualizarAlerta.visible(false);
    _grupoPessoasMensagemAlerta.ExcluirAlerta.visible(false);
    _grupoPessoasMensagemAlerta.CancelarAlerta.visible(false);
}