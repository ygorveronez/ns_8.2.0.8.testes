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
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/SerieTransportador.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoIntramunicipal.js" />
/// <reference path="Transportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _htmlConfiguracaoTipoOperacao;
var _gridConfiguracaoTipoOperacao;
var _configuracaoTipoOperacao;
var _statusConfiguracaoTipoOperacao = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var ConfiguracaoTipoOperacao = function () {

    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });

    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.TipoOperacao.getRequiredFieldDescription(), required: true, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.SerieIntraestadual = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.SerieCTeDentroEstado.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.SerieInterestadual = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.SerieCTeForaEstado.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.SerieMDFe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.SerieMDFe.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoEmissaoIntramunicipal = PropertyEntity({ val: ko.observable(EnumTipoEmissaoIntramunicipal.NaoEspecificado), options: EnumTipoEmissaoIntramunicipal.obterOpcoes(), def: EnumTipoEmissaoIntramunicipal.NaoEspecificado, text: Localization.Resources.Transportadores.Transportador.TipoDocumentoParaFretesMunicipais.getFieldDescription(), required: true, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoTipoOperacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirConfiguracaoTipoOperacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarConfiguracaoTipoOperacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarConfiguracaoTipoOperacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadConfiguracaoTipoOperacao(callback) {
    BuscarHTMLConfiguracaoTipoOperacao(function () {
        $("#knockoutCadastroConfiguracaoTipoOperacao").html(_htmlConfiguracaoTipoOperacao);
        _configuracaoTipoOperacao = new ConfiguracaoTipoOperacao()
        _configuracaoTipoOperacao.Codigo.val(guid());
        KoBindings(_configuracaoTipoOperacao, "knockoutCadastroConfiguracaoTipoOperacao");
        LocalizeCurrentPage();

        new BuscarSeriesCTeTransportador(_configuracaoTipoOperacao.SerieIntraestadual, null, null, null, null, _configuracao.Empresa);
        new BuscarSeriesCTeTransportador(_configuracaoTipoOperacao.SerieInterestadual, null, null, null, null, _configuracao.Empresa);
        new BuscarSeriesMDFeTransportador(_configuracaoTipoOperacao.SerieMDFe, null, null, null, null, _configuracao.Empresa);
        new BuscarTiposOperacao(_configuracaoTipoOperacao.TipoOperacao);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarConfiguracaoTipoOperacaoClick }] };
        var header = [
            { data: "Codigo", visible: false },
            { data: "TipoOperacao", title: Localization.Resources.Transportadores.Transportador.TipoOperacao, width: "40%" },
            { data: "SerieDentroEstado", title: Localization.Resources.Transportadores.Transportador.SerieEstado, width: "20%" },
            { data: "SerieForaEstado", title: Localization.Resources.Transportadores.Transportador.SerieForaEstado, width: "20%" }
        ];

        _gridConfiguracaoTipoOperacao = new BasicDataTable(_configuracaoTipoOperacao.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.desc });
        recarregarGridConfiguracaoTipoOperacao();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
            $("#liTabConfiguracaoTipoOperacao").show();

        callback();
    });
}

function BuscarHTMLConfiguracaoTipoOperacao(callback) {
    $.get("Content/Static/Transportador/ConfiguracaoTipoOperacao.html?dyn=" + guid(), function (data) {
        _htmlConfiguracaoTipoOperacao = data;
        if (callback != null)
            callback();
    });
}
function DescricaoTipoConfiguracaoTipoOperacao(tipo) {
    for (var i = 0; i < _tipoConfiguracaoTipoOperacao.length; i++) {
        if (_tipoConfiguracaoTipoOperacao[i].value == tipo)
            return _tipoConfiguracaoTipoOperacao[i].text;
    }
}

function DescricaoStatus(status) {
    for (var i = 0; i < _statusConfiguracaoTipoOperacao.length; i++) {
        if (_statusConfiguracaoTipoOperacao[i].value == status)
            return _status[i].text;
    }
}

function recarregarGridConfiguracaoTipoOperacao() {
    //if (_permissao_acesso_configuracaoTipoOperacaos == null || !_permissao_acesso_configuracaoTipoOperacaos.Acesso) 
    //    return;

    var data = new Array();
    $.each(_transportador.ConfiguracaoTipoOperacaos.list, function (i, configuracaoTipoOperacao) {
        var configuracaoTipoOperacaoGrid = new Object();
        configuracaoTipoOperacaoGrid.Codigo = configuracaoTipoOperacao.TipoOperacao.codEntity;
        configuracaoTipoOperacaoGrid.TipoOperacao = configuracaoTipoOperacao.TipoOperacao.val;

        configuracaoTipoOperacaoGrid.SerieDentroEstado = configuracaoTipoOperacao.SerieIntraestadual.val;
        configuracaoTipoOperacaoGrid.SerieForaEstado = configuracaoTipoOperacao.SerieInterestadual.val;

        data.push(configuracaoTipoOperacaoGrid);
    });
    _gridConfiguracaoTipoOperacao.CarregarGrid(data);
}

function editarConfiguracaoTipoOperacaoClick(data) {
    var dadosEditar = null;
    $.each(_transportador.ConfiguracaoTipoOperacaos.list, function (i, configuracaoTipoOperacao) {
        if (data.Codigo == configuracaoTipoOperacao.TipoOperacao.codEntity) {
            dadosEditar = _transportador.ConfiguracaoTipoOperacaos.list[i];
            return false;
        }
    });

    _configuracaoTipoOperacao.Atualizar.visible(true);
    _configuracaoTipoOperacao.Cancelar.visible(true);
    _configuracaoTipoOperacao.Excluir.visible(true);
    _configuracaoTipoOperacao.Adicionar.visible(false);
    PreencherEditarListEntity(_configuracaoTipoOperacao, dadosEditar);
}

function adicionarConfiguracaoTipoOperacaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoTipoOperacao);
    if (tudoCerto) {
        var existe = false;
        $.each(_transportador.ConfiguracaoTipoOperacaos.list, function (i, configuracaoTipoOperacao) {
            if (configuracaoTipoOperacao.TipoOperacao.codEntity == _configuracaoTipoOperacao.TipoOperacao.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {

            _transportador.ConfiguracaoTipoOperacaos.list.push(SalvarListEntity(_configuracaoTipoOperacao));
            recarregarGridConfiguracaoTipoOperacao();
            $("#" + _configuracaoTipoOperacao.TipoOperacao.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.TipoOperacaoConfigurada, Localization.Resources.Transportadores.Transportador.ExisteUmaConfiguracaoParaTipoOperacao.format(_configuracaoTipoOperacao.TipoOperacao.val()));
        }
        limparCamposConfiguracaoTipoOperacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
    }
}

function atualizarConfiguracaoTipoOperacaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoTipoOperacao);
    if (tudoCerto) {
        $.each(_transportador.ConfiguracaoTipoOperacaos.list, function (i, configuracaoTipoOperacao) {
            if (configuracaoTipoOperacao.Codigo.val == _configuracaoTipoOperacao.Codigo.val()) {
                _transportador.ConfiguracaoTipoOperacaos.list[i] = SalvarListEntity(_configuracaoTipoOperacao);
                //AtualizarListEntity(_configuracaoTipoOperacao, configuracaoTipoOperacao)
                return false;
            }
        });
        recarregarGridConfiguracaoTipoOperacao();
        limparCamposConfiguracaoTipoOperacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
    }
}

function excluirConfiguracaoTipoOperacaoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Transportador.RealmenteDesejaExcluirConfiguracaoParaTipoOperacao.format(_configuracaoTipoOperacao.TipoOperacao.val()), function () {
        $.each(_transportador.ConfiguracaoTipoOperacaos.list, function (i, configuracao) {
            if (_configuracaoTipoOperacao.Codigo.val() == configuracao.Codigo.val)
                _transportador.ConfiguracaoTipoOperacaos.list.splice(i, 1);
        });
        limparCamposConfiguracaoTipoOperacao();
        recarregarGridConfiguracaoTipoOperacao();
    });
}

function cancelarConfiguracaoTipoOperacaoClick(e) {
    limparCamposConfiguracaoTipoOperacao();
}

function limparCamposConfiguracaoTipoOperacao() {
    _configuracaoTipoOperacao.Codigo.val(guid());
    _configuracaoTipoOperacao.Atualizar.visible(false);
    _configuracaoTipoOperacao.Cancelar.visible(false);
    _configuracaoTipoOperacao.Excluir.visible(false);
    _configuracaoTipoOperacao.Adicionar.visible(true);
    LimparCampos(_configuracaoTipoOperacao);
}
