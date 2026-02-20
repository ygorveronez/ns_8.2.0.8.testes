//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaNotaFiscalDimensao;
var _cargaNotaFiscalDimensao;

var CargaNotaFiscalDimensao = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Volumes = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.Volumes.getRequiredFieldDescription(), required: true, maxlength: 7, configInt: { precision: 0, allowZero: false, thousand: '' }, enable: ko.observable(true) });
    this.Altura = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.Altura.getRequiredFieldDescription(), required: true, maxlength: 15, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, enable: ko.observable(true) });
    this.Largura = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.Largura.getRequiredFieldDescription(), required: true, maxlength: 15, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, enable: ko.observable(true) });
    this.Comprimento = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.Comprimento.getRequiredFieldDescription(), required: true, maxlength: 15, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, enable: ko.observable(true) });
    this.MetrosCubicos = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.MetrosCubicos.getFieldDescription(), maxlength: 15, configDecimal: { precision: 6, allowZero: false, allowNegative: false }, enable: ko.observable(false) });

    this.Atualizar = PropertyEntity({ eventClick: AtualizarCargaNotaFiscalDimensaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: ko.observable(PodeEditarValoresDaNota()) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirCargaNotaFiscalDimensaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: ko.observable(PodeEditarValoresDaNota()) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarCargaNotaFiscalDimensaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: ko.observable(PodeEditarValoresDaNota()) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarCargaNotaFiscalDimensaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(PodeEditarValoresDaNota()) });

    this.Altura.val.subscribe(function () {
        AjustarMetrosCubicosCargaNotaFiscalDimensao();
    });

    this.Largura.val.subscribe(function () {
        AjustarMetrosCubicosCargaNotaFiscalDimensao();
    });

    this.Comprimento.val.subscribe(function () {
        AjustarMetrosCubicosCargaNotaFiscalDimensao();
    });

    this.Volumes.val.subscribe(function () {
        AjustarMetrosCubicosCargaNotaFiscalDimensao();
    });

    //Variaveis Tradução DocumentosParaEmissao.html
    this.ExcluirTodasNotasFiscais = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ExcluirTodasAsNotasFiscais });
    this.ExcluirTodosCTes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ExcluirTodosOsCTes });
    this.NFeEmitidaEmContigencia = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NotaFiscalEmitidaEmContingenciaFSDA });
    this.DocumentoVinculadoEmOutraCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DocumentoVinculadoEmOutraCarga });
    this.EspelhoIntercement = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EspelhoIntercement });
    this.Validar = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Validar });
    this.AdicionarDocumentosManualmente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AdicionarDocumentosManualmente });
    this.NotasNaoRecebidas = PropertyEntity({ text: Localization.Resources.Cargas.NotaNaoRecebidas });
    this.NotasCompativeisComCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NotasCompativeisComCarga });
    this.DropZone = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Dropzone });
    this.MultiplasChaves = PropertyEntity({ text: Localization.Resources.Cargas.Carga.MultiplasChaves });
    this.EnvioDeArquivos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EnvioDeArquivos });
    this.LancamentoDeChaves = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LancamentoDeChaves });
    this.EditarDadosDaNotaFiscal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EditarDadosDaNotaFiscal });
    this.Detalhes = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Detalhes });
    this.EditarDadosDoCTeDeSubcontratacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EditarDadosDoCTeDeSubcontratacao });
    this.Dimensoes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Dimensoes });

    this.CTesVinculadosOSMae = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CTesVinculadosOSMae });
};

//*******EVENTOS*******

function LoadCargaNotaFiscalDimensao(idElemento) {

    _cargaNotaFiscalDimensao = new CargaNotaFiscalDimensao();
    KoBindings(_cargaNotaFiscalDimensao, "divDimensoes_" + idElemento + "_knoutDocumentosParaEmissao");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: EditarCargaNotaFiscalDimensaoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Volumes", title: Localization.Resources.Cargas.Carga.Volumes, width: "17%" },
        { data: "Altura", title: Localization.Resources.Cargas.Carga.Altura, width: "17%" },
        { data: "Largura", title: Localization.Resources.Cargas.Carga.Largura, width: "17%" },
        { data: "Comprimento", title: Localization.Resources.Cargas.Carga.Comprimento, width: "17 % " },
        { data: "MetrosCubicos", title: Localization.Resources.Cargas.Carga.MetrosCubicos, width: "17%" }
    ];

    _gridCargaNotaFiscalDimensao = new BasicDataTable(_cargaNotaFiscalDimensao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.desc });

    RecarregarGridCargaNotaFiscalDimensao();

    if (_cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe) {
        _cargaNotaFiscalDimensao.Adicionar.visible(false);
        SetarEnableCamposKnockout(_cargaNotaFiscalDimensao, false);
    }
}

function RecarregarGridCargaNotaFiscalDimensao() {

    var data = new Array();

    $.each(_notaFiscalEdicao.Dimensoes.list, function (i, dimensao) {
        var dimensaoGrid = new Object();

        dimensaoGrid.Codigo = dimensao.Codigo.val;
        dimensaoGrid.Altura = dimensao.Altura.val;
        dimensaoGrid.Largura = dimensao.Largura.val;
        dimensaoGrid.Volumes = dimensao.Volumes.val;
        dimensaoGrid.Comprimento = dimensao.Comprimento.val;
        dimensaoGrid.MetrosCubicos = dimensao.MetrosCubicos.val;

        data.push(dimensaoGrid);
    });

    _gridCargaNotaFiscalDimensao.CarregarGrid(data);
}

function AtualizarCargaNotaFiscalDimensaoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_cargaNotaFiscalDimensao);

    if (valido) {

        for (var i = 0; i < _notaFiscalEdicao.Dimensoes.list.length; i++) {
            if (_cargaNotaFiscalDimensao.Codigo.val() == _notaFiscalEdicao.Dimensoes.list[i].Codigo.val) {
                _notaFiscalEdicao.Dimensoes.list[i].Altura.val = _cargaNotaFiscalDimensao.Altura.val();
                _notaFiscalEdicao.Dimensoes.list[i].Largura.val = _cargaNotaFiscalDimensao.Largura.val();
                _notaFiscalEdicao.Dimensoes.list[i].Comprimento.val = _cargaNotaFiscalDimensao.Comprimento.val();
                _notaFiscalEdicao.Dimensoes.list[i].Volumes.val = _cargaNotaFiscalDimensao.Volumes.val();
                _notaFiscalEdicao.Dimensoes.list[i].MetrosCubicos.val = _cargaNotaFiscalDimensao.MetrosCubicos.val();
                break;
            }
        }
        SumarizarInformacoesNotaFiscalDimensao();
        RecarregarGridCargaNotaFiscalDimensao();
        LimparCamposCargaNotaFiscalDimensao();

    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function ExcluirCargaNotaFiscalDimensaoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteExcluirEssaDimensao, function () {
        for (var i = 0; i < _notaFiscalEdicao.Dimensoes.list.length; i++) {
            if (_cargaNotaFiscalDimensao.Codigo.val() == _notaFiscalEdicao.Dimensoes.list[i].Codigo.val) {
                _notaFiscalEdicao.Dimensoes.list.splice(i, 1);
                break;
            }
        }
        SumarizarInformacoesNotaFiscalDimensao();
        RecarregarGridCargaNotaFiscalDimensao();
        LimparCamposCargaNotaFiscalDimensao();
    });
}

function CancelarCargaNotaFiscalDimensaoClick(e, sender) {
    LimparCamposCargaNotaFiscalDimensao();
}

function EditarCargaNotaFiscalDimensaoClick(data) {
    for (var i = 0; i < _notaFiscalEdicao.Dimensoes.list.length; i++) {
        if (data.Codigo == _notaFiscalEdicao.Dimensoes.list[i].Codigo.val) {
            var dimensaoNotaFiscal = _notaFiscalEdicao.Dimensoes.list[i];

            _cargaNotaFiscalDimensao.Codigo.val(dimensaoNotaFiscal.Codigo.val);
            _cargaNotaFiscalDimensao.Altura.val(dimensaoNotaFiscal.Altura.val);
            _cargaNotaFiscalDimensao.Largura.val(dimensaoNotaFiscal.Largura.val);
            _cargaNotaFiscalDimensao.Comprimento.val(dimensaoNotaFiscal.Comprimento.val);
            _cargaNotaFiscalDimensao.Volumes.val(dimensaoNotaFiscal.Volumes.val);
            _cargaNotaFiscalDimensao.MetrosCubicos.val(dimensaoNotaFiscal.MetrosCubicos.val);

            break;
        }
    }

    if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe) {
        _cargaNotaFiscalDimensao.Atualizar.visible(true);
        _cargaNotaFiscalDimensao.Excluir.visible(true);
    } else {
        SetarEnableCamposKnockout(_cargaNotaFiscalDimensao, false);
    }

    _cargaNotaFiscalDimensao.Cancelar.visible(true);
    _cargaNotaFiscalDimensao.Adicionar.visible(false);
}

function AdicionarCargaNotaFiscalDimensaoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_cargaNotaFiscalDimensao);

    if (valido) {
        _cargaNotaFiscalDimensao.Codigo.val(guid());
        _notaFiscalEdicao.Dimensoes.list.push(SalvarListEntity(_cargaNotaFiscalDimensao));

        SumarizarInformacoesNotaFiscalDimensao();

        RecarregarGridCargaNotaFiscalDimensao();
        LimparCamposCargaNotaFiscalDimensao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function LimparCamposCargaNotaFiscalDimensao() {
    LimparCampos(_cargaNotaFiscalDimensao);
    _cargaNotaFiscalDimensao.Atualizar.visible(false);
    _cargaNotaFiscalDimensao.Excluir.visible(false);
    _cargaNotaFiscalDimensao.Cancelar.visible(false);

    if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe)
        _cargaNotaFiscalDimensao.Adicionar.visible(true);
}

function SumarizarInformacoesNotaFiscalDimensao() {
    var totalMetrosCubicos = 0, totalVolumes = 0;

    for (var i = 0; i < _notaFiscalEdicao.Dimensoes.list.length; i++) {
        var dimensaoNotaFiscal = _notaFiscalEdicao.Dimensoes.list[i];

        var volumes = Globalize.parseFloat(dimensaoNotaFiscal.Volumes.val);
        var metrosCubicos = Globalize.parseFloat(dimensaoNotaFiscal.MetrosCubicos.val);

        if (isNaN(volumes))
            volumes = 0;
        if (isNaN(metrosCubicos))
            metrosCubicos = 0;

        totalMetrosCubicos += metrosCubicos;
        totalVolumes += volumes;
    }

    _notaFiscalEdicao.MetrosCubicos.val(Globalize.format(totalMetrosCubicos, "n6"));
    _notaFiscalEdicao.Volumes.val(Globalize.format(totalVolumes, "n0"));
}

function AjustarMetrosCubicosCargaNotaFiscalDimensao() {
    var altura = Globalize.parseFloat(_cargaNotaFiscalDimensao.Altura.val());
    var largura = Globalize.parseFloat(_cargaNotaFiscalDimensao.Largura.val());
    var comprimento = Globalize.parseFloat(_cargaNotaFiscalDimensao.Comprimento.val());
    var volumes = Globalize.parseFloat(_cargaNotaFiscalDimensao.Volumes.val());

    if (isNaN(volumes))
        volumes = 0;
    if (isNaN(altura))
        altura = 0;
    if (isNaN(largura))
        largura = 0;
    if (isNaN(comprimento))
        comprimento = 0;

    _cargaNotaFiscalDimensao.MetrosCubicos.val(Globalize.format((altura * largura * comprimento * volumes), "n6"));
}