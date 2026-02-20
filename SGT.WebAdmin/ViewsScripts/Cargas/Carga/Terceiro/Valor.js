/// <reference path="../../../Consultas/Justificativa.js" />
/// <reference path="../../../Consultas/TaxaTerceiro.js" />

var _valorContratoFrete, _gridValorContratoFrete, _knoutContratoFreteAtual;

var ValorContratoFrete = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ContratoFrete = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.Valor.getRequiredFieldDescription(), val: ko.observable(""), def: "", required: true });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), val: ko.observable(""), def: "", required: false, maxlength: 400, visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Justificativa.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 382, enable: ko.observable(true) });
    this.TaxaTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.TaxaTerceiro.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.UtilizarTaxaPagamentoContratoFreteTerceiro) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarValorContratoFreteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, icon: "fal fa-plus", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarValorContratoFreteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, icon: "fal fa-save", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirValorContratoFreteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, icon: "fal fa-trash", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarValorContratoFreteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, icon: "fal fa-undo", visible: ko.observable(true) });
};

////*******EVENTOS*******

function LoadValorContratoFrete() {

    _valorContratoFrete = new ValorContratoFrete();
    KoBindings(_valorContratoFrete, "knockoutValorContratoFrete");

    new BuscarJustificativas(_valorContratoFrete.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete, EnumTipoFinalidadeJustificativa.Todas]);
    new BuscarTaxaTerceiro(_valorContratoFrete.TaxaTerceiro, RetornoTaxaTerceiroValorContratoFrete, null, _contratoFrete.Carga);

    //CarregarGridValorContratoFrete();
}

function RetornoTaxaTerceiroValorContratoFrete(dados) {
    _valorContratoFrete.TaxaTerceiro.codEntity(dados.Codigo);
    _valorContratoFrete.TaxaTerceiro.val(dados.Descricao);

    if (dados.CodigoJustificativa > 0) {
        _valorContratoFrete.Justificativa.codEntity(dados.CodigoJustificativa);
        _valorContratoFrete.Justificativa.val(dados.Justificativa);
    }
}

function AbrirTelaValorContratoFreteClick() {
    LimparCamposValorContratoFrete();
    Global.abrirModal("knockoutValorContratoFrete");
}

function AdicionarValorContratoFreteClick(e, sender) {
    Salvar(_valorContratoFrete, "ContratoFrete/AdicionarValor", function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.fecharModal("knockoutValorContratoFrete");
                preecherDadosContrato(r.Data, _detalhesContratoFrete);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValorAdicionadoComSucesso);
                _gridValorContratoFrete.CarregarGrid();
                LimparCamposValorContratoFrete();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function AtualizarValorContratoFreteClick(e, sender) {
    Salvar(_valorContratoFrete, "ContratoFrete/AtualizarValor", function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.fecharModal("knockoutValorContratoFrete");
                preecherDadosContrato(r.Data, _detalhesContratoFrete);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValorAtualizadoComSucesso);
                _gridValorContratoFrete.CarregarGrid();
                LimparCamposValorContratoFrete();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExcluirValorContratoFreteClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteExcluirValorDe.format(_valorContratoFrete.Valor.val()), function () {
        ExcluirPorCodigo(_valorContratoFrete, "ContratoFrete/ExcluirValor", function (r) {
            if (r.Success) {
                if (r.Data) {
                    Global.fecharModal("knockoutValorContratoFrete");
                    preecherDadosContrato(r.Data, _detalhesContratoFrete);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValorExcluidoComSucesso);
                    _gridValorContratoFrete.CarregarGrid();
                    LimparCamposValorContratoFrete();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function CancelarValorContratoFreteClick(e, sender) {
    Global.fecharModal("knockoutValorContratoFrete");
    LimparCamposValorContratoFrete();
}

function EditarValorContratoFreteClick(dadosGrid) {
    LimparCamposValorContratoFrete();
    _valorContratoFrete.Codigo.val(dadosGrid.Codigo);
    BuscarPorCodigo(_valorContratoFrete, "ContratoFrete/BuscarValorPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {

                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarAcrescimoDesconto, _PermissoesPersonalizadas)) {
                    _valorContratoFrete.Justificativa.enable(false);
                    _valorContratoFrete.TaxaTerceiro.enable(false);
                    _valorContratoFrete.Adicionar.visible(false);
                    _valorContratoFrete.Atualizar.visible(true);
                    _valorContratoFrete.Excluir.visible(true);
                } else {
                    SetarEnableCamposKnockout(_valorContratoFrete, false);
                    _valorContratoFrete.Cancelar.enable(true);
                }

                Global.abrirModal("knockoutValorContratoFrete");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

////*******METODOS*******

function CarregarGridValorContratoFrete(knout) {
    var permiteAlterar = false;

    if (knout.AdicionarValor.visible() === true)
        permiteAlterar = true;

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: EditarValorContratoFreteClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    if (!permiteAlterar)
        menuOpcoes = null;

    if (_gridValorContratoFrete != null)
        _gridValorContratoFrete.Destroy();

    _gridValorContratoFrete = new GridView(knout.AdicionarValor.idGrid, "ContratoFrete/ConsultarValor", knout, menuOpcoes, { column: 0, dir: orderDir.desc }, 5, null, false);

    _gridValorContratoFrete.CarregarGrid();
}

function LimparCamposValorContratoFrete() {
    _valorContratoFrete.Justificativa.enable(true);
    _valorContratoFrete.Justificativa.val("");
    _valorContratoFrete.Justificativa.codEntity(0);
    _valorContratoFrete.Valor.val("");
    _valorContratoFrete.TaxaTerceiro.enable(true);
    LimparCampoEntity(_valorContratoFrete.TaxaTerceiro);

    _valorContratoFrete.Adicionar.visible(true);
    _valorContratoFrete.Atualizar.visible(false);
    _valorContratoFrete.Excluir.visible(false);
}
