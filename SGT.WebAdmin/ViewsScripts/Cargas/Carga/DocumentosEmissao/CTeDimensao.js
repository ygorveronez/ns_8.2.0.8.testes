/// <reference path="CTe.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCTeTerceiroDimensao;
var _cteTerceiroDimensao;
var _CRUDCTeTerceiroDimensao;

var CTeTerceiroDimensao = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Volumes = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.Volumes.getRequiredFieldDescription(), required: true, maxlength: 7, configInt: { precision: 0, allowZero: false, thousand: '' }, enable: ko.observable(true) });
    this.Altura = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.Altura.getRequiredFieldDescription(), required: true, maxlength: 15, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, enable: ko.observable(true) });
    this.Largura = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.Largura.getRequiredFieldDescription(), required: true, maxlength: 15, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, enable: ko.observable(true) });
    this.Comprimento = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.Comprimento.getRequiredFieldDescription(), required: true, maxlength: 15, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, enable: ko.observable(true) });
    this.MetrosCubicos = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.MetrosCubicos.getFieldDescription(), maxlength: 15, configDecimal: { precision: 6, allowZero: false, allowNegative: false }, enable: ko.observable(false) });

    this.Altura.val.subscribe(function () {
        AjustarMetrosCubicosCTeTerceiroDimensao();
    });

    this.Largura.val.subscribe(function () {
        AjustarMetrosCubicosCTeTerceiroDimensao();
    });

    this.Comprimento.val.subscribe(function () {
        AjustarMetrosCubicosCTeTerceiroDimensao();
    });

    this.Volumes.val.subscribe(function () {
        AjustarMetrosCubicosCTeTerceiroDimensao();
    });
};

var CRUDCTeTerceiroDimensao = function () {
    this.Atualizar = PropertyEntity({ eventClick: AtualizarCTeTerceiroDimensaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirCTeTerceiroDimensaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarCTeTerceiroDimensaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarCTeTerceiroDimensaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(false) });
};

//*******EVENTOS*******

function LoadCTeTerceiroDimensao() {

    _cteTerceiroDimensao = new CTeTerceiroDimensao();
    KoBindings(_cteTerceiroDimensao, "knoutDimensoesCTeTerceiro");

    _CRUDCTeTerceiroDimensao = new CRUDCTeTerceiroDimensao();
    KoBindings(_CRUDCTeTerceiroDimensao, "knoutCRUDDimensoesCTeTerceiro");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: EditarCTeTerceiroDimensaoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Volumes", title: Localization.Resources.Cargas.Carga.Volumes, width: "17%" },
        { data: "Altura", title: Localization.Resources.Cargas.Carga.Altura, width: "17%" },
        { data: "Largura", title: Localization.Resources.Cargas.Carga.Largura, width: "17%" },
        { data: "Comprimento", title: Localization.Resources.Cargas.Carga.Comprimento, width: "17%" },
        { data: "MetrosCubicos", title: Localization.Resources.Cargas.Carga.MetrosCubicos, width: "17%" }
    ];

    _gridCTeTerceiroDimensao = new BasicDataTable(_cteTerceiroDimensao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.desc });

    RecarregarGridCTeTerceiroDimensao();

    if (_cargaAtual.SituacaoCarga.val() !== EnumSituacoesCarga.AgNFe) {
        _CRUDCTeTerceiroDimensao.Adicionar.visible(false);
        SetarEnableCamposKnockout(_cteTerceiroDimensao, false);
    }
}

function RecarregarGridCTeTerceiroDimensao() {

    var data = new Array();

    $.each(_cteTerceiro.Dimensoes.list, function (i, dimensao) {
        var dimensaoGrid = new Object();

        dimensaoGrid.Codigo = dimensao.Codigo.val;
        dimensaoGrid.Altura = dimensao.Altura.val;
        dimensaoGrid.Largura = dimensao.Largura.val;
        dimensaoGrid.Volumes = dimensao.Volumes.val;
        dimensaoGrid.Comprimento = dimensao.Comprimento.val;
        dimensaoGrid.MetrosCubicos = dimensao.MetrosCubicos.val;

        data.push(dimensaoGrid);
    });

    _gridCTeTerceiroDimensao.CarregarGrid(data);
}

function AtualizarCTeTerceiroDimensaoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_cteTerceiroDimensao);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    for (var i = 0; i < _cteTerceiro.Dimensoes.list.length; i++) {
        if (_cteTerceiroDimensao.Codigo.val() == _cteTerceiro.Dimensoes.list[i].Codigo.val) {
            _cteTerceiro.Dimensoes.list[i].Altura.val = _cteTerceiroDimensao.Altura.val();
            _cteTerceiro.Dimensoes.list[i].Largura.val = _cteTerceiroDimensao.Largura.val();
            _cteTerceiro.Dimensoes.list[i].Comprimento.val = _cteTerceiroDimensao.Comprimento.val();
            _cteTerceiro.Dimensoes.list[i].Volumes.val = _cteTerceiroDimensao.Volumes.val();
            _cteTerceiro.Dimensoes.list[i].MetrosCubicos.val = _cteTerceiroDimensao.MetrosCubicos.val();
            break;
        }
    }

    SumarizarInformacoesCTeTerceiroDimensao();
    RecarregarGridCTeTerceiroDimensao();
    LimparCamposCTeTerceiroDimensao();
}

function ExcluirCTeTerceiroDimensaoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteExcluirEssaDimensao, function () {
        for (var i = 0; i < _cteTerceiro.Dimensoes.list.length; i++) {
            if (_cteTerceiroDimensao.Codigo.val() == _cteTerceiro.Dimensoes.list[i].Codigo.val) {
                _cteTerceiro.Dimensoes.list.splice(i, 1);
                break;
            }
        }

        SumarizarInformacoesCTeTerceiroDimensao();
        RecarregarGridCTeTerceiroDimensao();
        LimparCamposCTeTerceiroDimensao();
    });
}

function CancelarCTeTerceiroDimensaoClick(e, sender) {
    LimparCamposCTeTerceiroDimensao();
}

function EditarCTeTerceiroDimensaoClick(data) {
    for (var i = 0; i < _cteTerceiro.Dimensoes.list.length; i++) {
        if (data.Codigo == _cteTerceiro.Dimensoes.list[i].Codigo.val) {
            var dimensaoCTeTerceiro = _cteTerceiro.Dimensoes.list[i];

            _cteTerceiroDimensao.Codigo.val(dimensaoCTeTerceiro.Codigo.val);
            _cteTerceiroDimensao.Altura.val(dimensaoCTeTerceiro.Altura.val);
            _cteTerceiroDimensao.Largura.val(dimensaoCTeTerceiro.Largura.val);
            _cteTerceiroDimensao.Comprimento.val(dimensaoCTeTerceiro.Comprimento.val);
            _cteTerceiroDimensao.Volumes.val(dimensaoCTeTerceiro.Volumes.val);
            _cteTerceiroDimensao.MetrosCubicos.val(dimensaoCTeTerceiro.MetrosCubicos.val);

            break;
        }
    }

    if (_cargaAtual.SituacaoCarga.val() === EnumSituacoesCarga.AgNFe) {
        _CRUDCTeTerceiroDimensao.Atualizar.visible(true);
        _CRUDCTeTerceiroDimensao.Excluir.visible(true);
    } else {
        SetarEnableCamposKnockout(_cteTerceiroDimensao, false);
    }

    _CRUDCTeTerceiroDimensao.Cancelar.visible(true);
    _CRUDCTeTerceiroDimensao.Adicionar.visible(false);
}

function AdicionarCTeTerceiroDimensaoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_cteTerceiroDimensao);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    _cteTerceiroDimensao.Codigo.val(guid());
    _cteTerceiro.Dimensoes.list.push(SalvarListEntity(_cteTerceiroDimensao));

    SumarizarInformacoesCTeTerceiroDimensao();
    RecarregarGridCTeTerceiroDimensao();
    LimparCamposCTeTerceiroDimensao();
}

function LimparCamposCTeTerceiroDimensao() {
    LimparCampos(_cteTerceiroDimensao);
    _CRUDCTeTerceiroDimensao.Atualizar.visible(false);
    _CRUDCTeTerceiroDimensao.Excluir.visible(false);
    _CRUDCTeTerceiroDimensao.Cancelar.visible(false);

    if (_cargaAtual.SituacaoCarga.val() === EnumSituacoesCarga.AgNFe)
        _CRUDCTeTerceiroDimensao.Adicionar.visible(true);
}

function SumarizarInformacoesCTeTerceiroDimensao() {
    var totalMetrosCubicos = 0, totalVolumes = 0;

    for (var i = 0; i < _cteTerceiro.Dimensoes.list.length; i++) {
        var dimensaoCTeTerceiro = _cteTerceiro.Dimensoes.list[i];

        var volumes = Globalize.parseFloat(dimensaoCTeTerceiro.Volumes.val);
        var metrosCubicos = Globalize.parseFloat(dimensaoCTeTerceiro.MetrosCubicos.val);

        if (isNaN(volumes))
            volumes = 0;
        if (isNaN(metrosCubicos))
            metrosCubicos = 0;

        totalMetrosCubicos += metrosCubicos;
        totalVolumes += volumes;
    }

    _cteTerceiro.MetrosCubicos.val(Globalize.format(totalMetrosCubicos, "n6"));
    _cteTerceiro.Volumes.val(Globalize.format(totalVolumes, "n0"));
}

function AjustarMetrosCubicosCTeTerceiroDimensao() {
    var altura = Globalize.parseFloat(_cteTerceiroDimensao.Altura.val());
    var largura = Globalize.parseFloat(_cteTerceiroDimensao.Largura.val());
    var comprimento = Globalize.parseFloat(_cteTerceiroDimensao.Comprimento.val());
    var volumes = Globalize.parseFloat(_cteTerceiroDimensao.Volumes.val());

    if (isNaN(volumes))
        volumes = 0;
    if (isNaN(altura))
        altura = 0;
    if (isNaN(largura))
        largura = 0;
    if (isNaN(comprimento))
        comprimento = 0;

    _cteTerceiroDimensao.MetrosCubicos.val(Globalize.format((altura * largura * comprimento * volumes), "n6"));
}