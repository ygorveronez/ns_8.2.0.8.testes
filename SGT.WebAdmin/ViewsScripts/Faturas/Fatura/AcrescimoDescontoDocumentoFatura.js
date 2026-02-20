var _acrescimoDescontoDocumentoFatura, _gridAcrescimoDescontoDocumentoFatura, _acrescimoDescontoDocumentos, _modalAcrescimoDocumentoFatura;

var AcrescimoDescontoDocumentoFatura = function () {
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid() });
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Documento = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor:", val: ko.observable(""), def: "", required: true });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true, issue: 382, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", required: false, maxlength: 500 })

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(false) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAcrescimoDescontoDocumentoFaturaClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarAcrescimoDescontoDocumentoFaturaClick, type: types.event, text: "Atualizar", icon: "fal fa-save", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirAcrescimoDescontoDocumentoFaturaClick, type: types.event, text: "Excluir", icon: "fal fa-close", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarAcrescimoDescontoDocumentoFaturaClick, type: types.event, text: "Cancelar", icon: "fal fa-rotate-left", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaAcrescimoDescontoDocumentoFatura, type: types.event, text: "Fechar", icon: "fal fa-window-close", visible: ko.observable(true) });
}

var AcrescimoDescontoDocumentos = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor:", val: ko.observable(""), def: "", required: true });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true, issue: 382, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", required: false, maxlength: 500 });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(false) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAcrescimoDescontoDocumentosClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarAcrescimoDescontoDocumentosClick, type: types.event, text: "Limpar", icon: "fal fa-rotate-left", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharAcrescimoDescontoDocumentosClick, type: types.event, text: "Fechar", icon: "fal fa-window-close", visible: ko.observable(true) });
}

////*******EVENTOS*******

function LoadAcrescimoDescontoDocumentoFatura() {
    _acrescimoDescontoDocumentoFatura = new AcrescimoDescontoDocumentoFatura();
    KoBindings(_acrescimoDescontoDocumentoFatura, "knockoutAcrescimoDescontoDocumentoFatura");

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        ;
        _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.visible(true);
        _acrescimoDescontoDocumentoFatura.DataBaseCRT.visible(true);
        _acrescimoDescontoDocumentoFatura.ValorMoedaCotacao.visible(true);
        _acrescimoDescontoDocumentoFatura.ValorOriginalMoedaEstrangeira.visible(true);
    }

    new BuscarJustificativas(_acrescimoDescontoDocumentoFatura.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.Fatura, EnumTipoFinalidadeJustificativa.Todas]);
    _modalAcrescimoDocumentoFatura = new bootstrap.Modal(document.getElementById("knockoutAcrescimoDescontoDocumentoFatura"), { backdrop: true, keyboard: true });
}

function LoadAcrescimoDescontoDocumentos() {
    _acrescimoDescontoDocumentos = new AcrescimoDescontoDocumentos();
    KoBindings(_acrescimoDescontoDocumentos, "knockoutAcrescimoDescontoDocumentos");

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _acrescimoDescontoDocumentos.MoedaCotacaoBancoCentral.visible(true);
        _acrescimoDescontoDocumentos.DataBaseCRT.visible(true);
        _acrescimoDescontoDocumentos.ValorMoedaCotacao.visible(true);
        _acrescimoDescontoDocumentos.ValorOriginalMoedaEstrangeira.visible(true);
    }

    new BuscarJustificativas(_acrescimoDescontoDocumentos.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.Fatura, EnumTipoFinalidadeJustificativa.Todas]);

    _modalAcrescimoDescontoDocumentoFatura = new bootstrap.Modal(document.getElementById("knockoutAcrescimoDescontoDocumentoFatura"), { backdrop: 'static', keyboard: true });
}

function AdicionarAcrescimoDescontoDocumentoFaturaClick(e, sender) {
    Salvar(_acrescimoDescontoDocumentoFatura, "FaturaDocumentoAcrescimoDesconto/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor adicionado com sucesso!");
                _gridAcrescimoDescontoDocumentoFatura.CarregarGrid();
                _gridDocumentosFatura.CarregarGrid();
                LimparCamposAcrescimoDescontoDocumentoFatura();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AtualizarAcrescimoDescontoDocumentoFaturaClick(e, sender) {
    Salvar(_acrescimoDescontoDocumentoFatura, "FaturaDocumentoAcrescimoDesconto/Atualizar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor atualizado com sucesso!");
                _gridAcrescimoDescontoDocumentoFatura.CarregarGrid();
                _gridDocumentosFatura.CarregarGrid();
                LimparCamposAcrescimoDescontoDocumentoFatura();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ExcluirAcrescimoDescontoDocumentoFaturaClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o valor de " + _acrescimoDescontoDocumentoFatura.Valor.val() + "?", function () {
        ExcluirPorCodigo(_acrescimoDescontoDocumentoFatura, "FaturaDocumentoAcrescimoDesconto/Excluir", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor excluído com sucesso!");
                    _gridAcrescimoDescontoDocumentoFatura.CarregarGrid();
                    _gridDocumentosFatura.CarregarGrid();
                    LimparCamposAcrescimoDescontoDocumentoFatura();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function CancelarAcrescimoDescontoDocumentoFaturaClick(e, sender) {
    var moedaAnterior = e.MoedaCotacaoBancoCentral.val();
    LimparCamposAcrescimoDescontoDocumentoFatura();
    _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.val(moedaAnterior);

    if (_acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.val() !== null && _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.val() !== undefined && _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.val() !== EnumMoedaCotacaoBancoCentral.Real) {
        _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.visible(true);
        _acrescimoDescontoDocumentoFatura.DataBaseCRT.visible(true);
        _acrescimoDescontoDocumentoFatura.ValorMoedaCotacao.visible(true);
        _acrescimoDescontoDocumentoFatura.ValorOriginalMoedaEstrangeira.visible(true);
    }
    else {
        _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.visible(false);
        _acrescimoDescontoDocumentoFatura.DataBaseCRT.visible(false);
        _acrescimoDescontoDocumentoFatura.ValorMoedaCotacao.visible(false);
        _acrescimoDescontoDocumentoFatura.ValorOriginalMoedaEstrangeira.visible(false);
    }
}

function EditarAcrescimoDescontoDocumentoFaturaClick(dadosGrid) {
    LimparCamposAcrescimoDescontoDocumentoFatura();
    _acrescimoDescontoDocumentoFatura.Codigo.val(dadosGrid.Codigo);
    BuscarPorCodigo(_acrescimoDescontoDocumentoFatura, "FaturaDocumentoAcrescimoDesconto/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _acrescimoDescontoDocumentoFatura.Justificativa.enable(false);
                _acrescimoDescontoDocumentoFatura.Adicionar.visible(false);
                _acrescimoDescontoDocumentoFatura.Atualizar.visible(true);
                _acrescimoDescontoDocumentoFatura.Excluir.visible(true);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

////*******METODOS*******

function CarregarGridAcrescimoDescontoDocumentoFatura() {
    var permiteAlterar = false;

    if (_fatura.Situacao.val() == EnumSituacoesFatura.EmAndamento)
        permiteAlterar = true;

    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarAcrescimoDescontoDocumentoFaturaClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    if (!permiteAlterar)
        menuOpcoes = null;

    if (_gridAcrescimoDescontoDocumentoFatura != null)
        _gridAcrescimoDescontoDocumentoFatura.Destroy();

    _gridAcrescimoDescontoDocumentoFatura = new GridView(_acrescimoDescontoDocumentoFatura.Grid.idGrid, "FaturaDocumentoAcrescimoDesconto/Pesquisa", _acrescimoDescontoDocumentoFatura, menuOpcoes, { column: 0, dir: orderDir.asc }, 5, null, false);
    _gridAcrescimoDescontoDocumentoFatura.CarregarGrid();
}

function LimparCamposAcrescimoDescontoDocumentoFatura() {
    var moedaAnterior = _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.val();

    _acrescimoDescontoDocumentoFatura.Justificativa.enable(true);
    _acrescimoDescontoDocumentoFatura.Justificativa.val("");
    _acrescimoDescontoDocumentoFatura.Justificativa.codEntity(0);
    _acrescimoDescontoDocumentoFatura.Valor.val("");
    _acrescimoDescontoDocumentoFatura.Observacao.val("");
    _acrescimoDescontoDocumentoFatura.Adicionar.visible(true);
    _acrescimoDescontoDocumentoFatura.Atualizar.visible(false);
    _acrescimoDescontoDocumentoFatura.Excluir.visible(false);

    _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.val(EnumMoedaCotacaoBancoCentral.Real);
    _acrescimoDescontoDocumentoFatura.DataBaseCRT.val("");
    _acrescimoDescontoDocumentoFatura.ValorMoedaCotacao.val("");
    _acrescimoDescontoDocumentoFatura.ValorOriginalMoedaEstrangeira.val("");

    _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.val(moedaAnterior);
}

function AbrirTelaAcrescimoDescontoFatura(dadosGrid) {
    LimparCamposAcrescimoDescontoDocumentoFatura();
    _acrescimoDescontoDocumentoFatura.Documento.val(dadosGrid.Codigo);
    _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.val(dadosGrid.Moeda);
    CarregarGridAcrescimoDescontoDocumentoFatura();
    _modalAcrescimoDocumentoFatura.show();
}

function FecharTelaAcrescimoDescontoDocumentoFatura() {
    _modalAcrescimoDocumentoFatura.hide();
    LimparCamposAcrescimoDescontoDocumentoFatura();
}

function CalcularMoedaEstrangeiraMultiplo() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _acrescimoDescontoDocumentos.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _acrescimoDescontoDocumentos.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _acrescimoDescontoDocumentos.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValor();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorMultiplo() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_acrescimoDescontoDocumentos.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_acrescimoDescontoDocumentos.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _acrescimoDescontoDocumentos.Valor.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorEstrangeiraMultiplo() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_acrescimoDescontoDocumentos.ValorMoedaCotacao.val());
        var valorBaixado = Globalize.parseFloat(_acrescimoDescontoDocumentos.Valor.val());
        if (valorBaixado > 0 && valorMoedaCotacao > 0) {
            _acrescimoDescontoDocumentos.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorBaixado / valorMoedaCotacao, "n2"));
        }
    }
}

function CalcularMoedaEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _acrescimoDescontoDocumentoFatura.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _acrescimoDescontoDocumentoFatura.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _acrescimoDescontoDocumentoFatura.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValor();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValor() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_acrescimoDescontoDocumentoFatura.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_acrescimoDescontoDocumentoFatura.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _acrescimoDescontoDocumentoFatura.Valor.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_acrescimoDescontoDocumentoFatura.ValorMoedaCotacao.val());
        var valorBaixado = Globalize.parseFloat(_acrescimoDescontoDocumentoFatura.Valor.val());
        if (valorBaixado > 0 && valorMoedaCotacao > 0) {
            _acrescimoDescontoDocumentoFatura.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorBaixado / valorMoedaCotacao, "n2"));
        }
    }
}

function limparCamposAcrescimoDescontoDocumentos() {
    LimparCampos(_acrescimoDescontoDocumentos);
}