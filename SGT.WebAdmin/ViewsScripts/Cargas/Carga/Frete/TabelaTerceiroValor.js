/// <reference path="../../../Consultas/Justificativa.js" />
/// <reference path="../../../Consultas/TaxaTerceiro.js" />

var _cargaTabelaTerceiroValor, _gridCargaTabelaTerceiroValor;

var CargaTabelaTerceiroValor = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.Valor.getRequiredFieldDescription(), val: ko.observable(""), def: "", required: true });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Cargas.Carga.Observacao.getFieldDescription(), val: ko.observable(""), def: "", required: false, maxlength: 400, visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Justificativa.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 382, enable: ko.observable(true) });
    this.TaxaTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.TaxaTerceiro.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.UtilizarTaxaPagamentoContratoFreteTerceiro) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarCargaTabelaTerceiroValorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, icon: "fal fa-plus", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarCargaTabelaTerceiroValorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, icon: "fal fa-save", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirCargaTabelaTerceiroValorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, icon: "fal fa-trash", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarCargaTabelaTerceiroValorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, icon: "fal fa-undo", visible: ko.observable(true) });
};

////*******EVENTOS*******

function SetarInformacoesCargaTabelaTerceiroValor(detalheFreteSubcontratacaoTerceiro) {
    _cargaTabelaTerceiroValor.DetalheFreteSubcontratacaoTerceiro = detalheFreteSubcontratacaoTerceiro;
    _cargaTabelaTerceiroValor.Carga.val(detalheFreteSubcontratacaoTerceiro.Carga.val());

    CarregarGridCargaTabelaTerceiroValor(detalheFreteSubcontratacaoTerceiro);
}

function LoadCargaTabelaTerceiroValor() {
    _cargaTabelaTerceiroValor = new CargaTabelaTerceiroValor();
    KoBindings(_cargaTabelaTerceiroValor, "knockoutModalCargaTabelaTerceiroValor");

    new BuscarJustificativas(_cargaTabelaTerceiroValor.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete, EnumTipoFinalidadeJustificativa.Todas]);
    new BuscarTaxaTerceiro(_cargaTabelaTerceiroValor.TaxaTerceiro, RetornoTaxaTerceiroCargaTabelaTerceiroValor, null, _cargaTabelaTerceiroValor.Carga);
}

function RetornoTaxaTerceiroCargaTabelaTerceiroValor(dados) {
    _cargaTabelaTerceiroValor.TaxaTerceiro.codEntity(dados.Codigo);
    _cargaTabelaTerceiroValor.TaxaTerceiro.val(dados.Descricao);

    if (dados.CodigoJustificativa > 0) {
        _cargaTabelaTerceiroValor.Justificativa.codEntity(dados.CodigoJustificativa);
        _cargaTabelaTerceiroValor.Justificativa.val(dados.Justificativa);
    }
}

function AbrirTelaCargaTabelaTerceiroValorClick(e) {
    LimparCamposCargaTabelaTerceiroValor();

    SetarInformacoesCargaTabelaTerceiroValor(e);

    Global.abrirModal("knockoutModalCargaTabelaTerceiroValor");
}

function AdicionarCargaTabelaTerceiroValorClick(e, sender) {
    Salvar(e, "CargaFreteTerceiro/AdicionarValor", function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.fecharModal("knockoutModalCargaTabelaTerceiroValor");
                informarValoresDeSubContratacao(e.DetalheFreteSubcontratacaoTerceiro, r.Data);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValorAdicionadoComSucesso);
                _gridCargaTabelaTerceiroValor.CarregarGrid();
                LimparCamposCargaTabelaTerceiroValor();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function AtualizarCargaTabelaTerceiroValorClick(e, sender) {
    Salvar(_cargaTabelaTerceiroValor, "CargaFreteTerceiro/AtualizarValor", function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.fecharModal("knockoutModalCargaTabelaTerceiroValor");
                informarValoresDeSubContratacao(e.DetalheFreteSubcontratacaoTerceiro, r.Data);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValorAtualizadoComSucesso);
                _gridCargaTabelaTerceiroValor.CarregarGrid();
                LimparCamposCargaTabelaTerceiroValor();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExcluirCargaTabelaTerceiroValorClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteExcluirValorDe.format(_cargaTabelaTerceiroValor.Valor.val()), function () {
        ExcluirPorCodigo(_cargaTabelaTerceiroValor, "CargaFreteTerceiro/ExcluirValor", function (r) {
            if (r.Success) {
                if (r.Data) {
                    Global.fecharModal("knockoutModalCargaTabelaTerceiroValor");
                    informarValoresDeSubContratacao(e.DetalheFreteSubcontratacaoTerceiro, r.Data);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValorExcluidoComSucesso);
                    _gridCargaTabelaTerceiroValor.CarregarGrid();
                    LimparCamposCargaTabelaTerceiroValor();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function CancelarCargaTabelaTerceiroValorClick(e, sender) {
    Global.fecharModal("knockoutModalCargaTabelaTerceiroValor");
    LimparCamposCargaTabelaTerceiroValor();
}

function EditarCargaTabelaTerceiroValorClick(dadosGrid) {
    LimparCamposCargaTabelaTerceiroValor();
    _cargaTabelaTerceiroValor.Codigo.val(dadosGrid.Codigo);
    BuscarPorCodigo(_cargaTabelaTerceiroValor, "CargaFreteTerceiro/BuscarValorPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _cargaTabelaTerceiroValor.Justificativa.enable(false);
                _cargaTabelaTerceiroValor.TaxaTerceiro.enable(false);
                _cargaTabelaTerceiroValor.Adicionar.visible(false);
                _cargaTabelaTerceiroValor.Atualizar.visible(true);
                _cargaTabelaTerceiroValor.Excluir.visible(true);
                Global.abrirModal("knockoutModalCargaTabelaTerceiroValor");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

////*******METODOS*******

function CarregarGridCargaTabelaTerceiroValor(knout) {
    var permiteAlterar = false;

    if (knout.AdicionarValor.visible() === true)
        permiteAlterar = true;

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: EditarCargaTabelaTerceiroValorClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    if (!permiteAlterar)
        menuOpcoes = null;

    if (_gridCargaTabelaTerceiroValor != null)
        _gridCargaTabelaTerceiroValor.Destroy();

    _gridCargaTabelaTerceiroValor = new GridView(knout.AdicionarValor.idGrid, "CargaFreteTerceiro/ConsultarValor", knout, menuOpcoes, { column: 0, dir: orderDir.desc }, 5, null, false);

    _gridCargaTabelaTerceiroValor.CarregarGrid();
}

function LimparCamposCargaTabelaTerceiroValor() {
    _cargaTabelaTerceiroValor.Justificativa.enable(true);
    _cargaTabelaTerceiroValor.Justificativa.val("");
    _cargaTabelaTerceiroValor.Justificativa.codEntity(0);
    _cargaTabelaTerceiroValor.Valor.val("");
    _cargaTabelaTerceiroValor.Observacao.val("");
    _cargaTabelaTerceiroValor.TaxaTerceiro.enable(true);
    LimparCampoEntity(_cargaTabelaTerceiroValor.TaxaTerceiro);

    _cargaTabelaTerceiroValor.Adicionar.visible(true);
    _cargaTabelaTerceiroValor.Atualizar.visible(false);
    _cargaTabelaTerceiroValor.Excluir.visible(false);
}