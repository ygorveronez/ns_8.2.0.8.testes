//*******MAPEAMENTO KNOUCKOUT*******

var ComponenteDeFrete = function () {
    var self = this;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.ValorComponente = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable(Localization.Resources.Cargas.Carga.ValorDoComponente.getRequiredFieldDescription()), required: true, getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorSugerido = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable(Localization.Resources.Cargas.Carga.ValorSugerido.getFieldDescription()), getType: typesKnockout.decimal });
    this.Percentual = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.Percentual.getRequiredFieldDescription(), required: false, getType: typesKnockout.decimal, visible: ko.observable(false), configDecimal: { precision: 3, allowZero: false, allowNegative: false } });

    this.Moeda = PropertyEntity({ enable: ko.observable(false), text: Localization.Resources.Cargas.Carga.Moeda.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, issue: 0 });
    this.ValorTotalMoeda = PropertyEntity({ enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira), text: Localization.Resources.Cargas.Carga.ValorEmMoeda.getFieldDescription(), def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorCotacaoMoeda = PropertyEntity({ enable: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira), text: Localization.Resources.Cargas.Carga.CotacaoDaMoeda.getFieldDescription(), def: "1,0000000000", val: ko.observable("1,0000000000"), getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 10, allowZero: false, allowNegative: false } });

    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.ComponenteDeFrete.getRequiredFieldDescription(), required: true, idBtnSearch: guid(), enable: ko.observable(true) });

    this.CobrarOutroDocumento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CobrarValorEmOutroDocumentoFiscal, enable: ko.observable(true), required: false, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });

    this.Adicionar = PropertyEntity({ eventClick: adicionarComponenteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarComponenteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirComponenteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarComponenteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: ko.observable(true) });

    this.ValorTotalMoeda.val.subscribe(function () { ConverterValorMoedaCargaComponenteFrete(self); });
};

//*******MÉTODOS*******
var _componenteFrete;
var _gridCompentesDeFrete;

function ConverterValorMoedaCargaComponenteFrete(e) {
    var valorCotacaoMoeda = 0;
    if (e.ValorCotacaoMoeda != null && e.ValorCotacaoMoeda != undefined && e.ValorCotacaoMoeda.val() != null && e.ValorCotacaoMoeda.val() != undefined)
        valorCotacaoMoeda = Globalize.parseFloat(e.ValorCotacaoMoeda.val());
    var valorTotalMoeda = 0;
    if (e.ValorTotalMoeda != null && e.ValorTotalMoeda != undefined && e.ValorTotalMoeda.val() != null && e.ValorTotalMoeda.val() != undefined)
        valorTotalMoeda = Globalize.parseFloat(e.ValorTotalMoeda.val());

    if (isNaN(valorCotacaoMoeda))
        valorCotacaoMoeda = 0;
    if (isNaN(valorTotalMoeda))
        valorTotalMoeda = 0;

    var valorTotalConvertido = valorCotacaoMoeda * valorTotalMoeda;

    if (valorTotalConvertido > 0)
        e.ValorComponente.val(Globalize.format(valorTotalConvertido, "n2"));
    else
        e.ValorComponente.val("");
}

function preecherComponentesFrete(e, componentes) {

    if (_gridCompentesDeFrete != null)
        _gridCompentesDeFrete.Destroy();

    var desabilitado = false;

    if (e.TipoCalculoTabelaFrete.val() === EnumTipoCalculoTabelaFrete.PorDocumentoEmitido) {
        desabilitado = true;
        e.ComponenteFrete.visible(false);
    }

    if (componentes != null) {
        e.ComponenteFrete.visibleFade(true);
        _componenteFrete = new ComponenteDeFrete();
        _componenteFrete.Carga.val(_cargaAtual.Codigo.val());
        KoBindings(_componenteFrete, "knoutComponentesFrete");

        new BuscarModeloDocumentoFiscal(_componenteFrete.ModeloDocumentoFiscal, null, null, true);
        new BuscarComponentesDeFrete(_componenteFrete.ComponenteFrete, RetornoConsultaComponenteFrete);

        var possuiPercentualSobreNota = false;
        $.each(componentes, function (i, componente) {
            componente.Valor = Globalize.format(componente.Valor, "n2");
            componente.ValorTotalMoeda = Globalize.format(componente.ValorTotalMoeda, "n2");
            if (componente.TipoValor != EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal) {
                componente.Percentual = "";
            } else {
                possuiPercentualSobreNota = true;
                componente.Percentual = Globalize.format(componente.Percentual, "n3");
            }
        });

        var Editar = { descricao: Localization.Resources.Gerais.Geral.Editar, tamanho: 15, id: guid(), metodo: editarComponenteClick, icone: "" };
        var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [Editar] };

        if (!_cargaAtual.EtapaFreteEmbarcador.enable()) {
            menuOpcoes = null;
        } else {
            possuiPercentualSobreNota = true;
        }

        if (_cargaAtual.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Embarcador)
            possuiPercentualSobreNota = false;

        var header = [
            { data: "Codigo", visible: false },
            { data: "ComponenteFrete", visible: false },
            { data: "DescricaoComponente", visible: false },
            { data: "TipoComponenteFrete", visible: false },
            { data: "CobrarOutroDocumento", visible: false },
            { data: "TipoValor", visible: false },
            { data: "DescontarValorTotalAReceber", visible: false },
            { data: "Tipo", visible: false },
            { data: "DescricaoModeloDocumentoFiscal", visible: false },
            { data: "CodigoModeloDocumentoFiscal", visible: false },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%" },
            { data: "AbreviacaoDocumentoFiscal", title: Localization.Resources.Cargas.Carga.DocumentoPonto, width: "10%" },
            { data: "DescricaoInformadoManualmente", title: Localization.Resources.Cargas.Carga.InformadoPor, width: "40%" },
            { data: "Percentual", title: Localization.Resources.Cargas.Carga.Percentual, width: "15%", className: "text-align-right", visible: possuiPercentualSobreNota },

        ];

        var moeda = _cargaAtual.Moeda.val();

        if (moeda !== null && moeda !== EnumMoedaCotacaoBancoCentral.Real)
            header.push({ data: "ValorTotalMoeda", title: Localization.Resources.Cargas.Carga.ValorMoeda, width: "15%", className: "text-align-right" });
        else
            header.push({ data: "ValorTotalMoeda", visible: false });

        header.push({ data: "Valor", title: Localization.Resources.Cargas.Carga.Valor, width: "15%", className: "text-align-right" });

        _gridCompentesDeFrete = new BasicDataTable(e.ComponenteFrete.idGrid, header, menuOpcoes);
        _gridCompentesDeFrete.CarregarGrid(componentes, !desabilitado);

    } else {
        e.ComponenteFrete.visibleFade(false);
    }
}

function RetornoConsultaComponenteFrete(arg) {
    _componenteFrete.ComponenteFrete.val(arg.Descricao);
    _componenteFrete.ComponenteFrete.codEntity(arg.Codigo);

    if (_cargaAtual.TipoFreteEscolhido.val() !== EnumTipoFreteEscolhido.Embarcador) {
        var moeda = _cargaAtual.Moeda.val();

        if (arg.TipoValor === EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal) {

            if (moeda !== null && moeda !== EnumMoedaCotacaoBancoCentral.Real) {
                _componenteFrete.ValorTotalMoeda.visible(false);
                _componenteFrete.Moeda.visible(false);
                _componenteFrete.ValorCotacaoMoeda.visible(false);
            }

            setarPadraoPercentual();
        }
        else {
            if (moeda !== null && moeda !== EnumMoedaCotacaoBancoCentral.Real) {
                _componenteFrete.ValorTotalMoeda.visible(true);
                _componenteFrete.Moeda.visible(true);
                _componenteFrete.ValorCotacaoMoeda.visible(true);
            }

            setarPadraoValores();

            ObterSugestaoValorComponenteFreteCalculado(arg.Codigo, _cargaAtual.Codigo.val());
        }

        if (!arg.DescontarValorTotalAReceber)
            _componenteFrete.ValorComponente.text(Localization.Resources.Cargas.Carga.ValorDoComponente.getRequiredFieldDescription());
        else
            _componenteFrete.ValorComponente.text(Localization.Resources.Cargas.Carga.ValorDeDesconto.getRequiredFieldDescription());
    }
}

function ObterSugestaoValorComponenteFreteCalculado(codigoComponente, codigoCarga) {
    executarReST("CargaComponenteFrete/ObterValorComponenteFreteCalculado", { ComponenteFrete: codigoComponente, Carga: codigoCarga }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _componenteFrete.ValorComponente.val(r.Data.Valor);
                _componenteFrete.ValorSugerido.val(r.Data.Valor);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function editarComponenteClick(data) {
    limparCamposComponente();

    if (data.TipoValor === EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal) {
        setarPadraoPercentual();
    } else {
        setarPadraoValores();
    }

    var moeda = _cargaAtual.Moeda.val();
    var cotacaoMoeda = _cargaAtual.ValorCotacaoMoeda.val();

    if (moeda !== null && moeda !== EnumMoedaCotacaoBancoCentral.Real && data.TipoValor !== EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal) {
        _componenteFrete.Moeda.val(moeda);
        _componenteFrete.ValorCotacaoMoeda.val(cotacaoMoeda);
        _componenteFrete.ValorTotalMoeda.visible(true);
        _componenteFrete.Moeda.visible(true);
        _componenteFrete.ValorCotacaoMoeda.visible(true);
        _componenteFrete.ValorComponente.enable(false);
        _componenteFrete.ModeloDocumentoFiscal.visible(false);
    } else {
        _componenteFrete.ValorTotalMoeda.visible(false);
        _componenteFrete.Moeda.visible(false);
        _componenteFrete.ValorCotacaoMoeda.visible(false);
    }

    if (!data.DescontarValorTotalAReceber) {
        _componenteFrete.ValorComponente.val(data.Valor);
        _componenteFrete.ValorComponente.text(Localization.Resources.Cargas.Carga.ValorDoComponente.getRequiredFieldDescription());
    } else {
        _componenteFrete.ValorComponente.text(Localization.Resources.Cargas.Carga.ValorDeDesconto);
        _componenteFrete.ValorComponente.val(data.Valor.replace("-", ""));
    }

    _componenteFrete.ValorTotalMoeda.val(data.ValorTotalMoeda);
    _componenteFrete.ComponenteFrete.val(data.DescricaoComponente);
    _componenteFrete.ComponenteFrete.codEntity(data.ComponenteFrete);
    _componenteFrete.Percentual.val(data.Percentual != "" ? data.Percentual : 0);

    _componenteFrete.ModeloDocumentoFiscal.val(data.DescricaoModeloDocumentoFiscal);
    _componenteFrete.ModeloDocumentoFiscal.codEntity(data.CodigoModeloDocumentoFiscal);
    _componenteFrete.CobrarOutroDocumento.val(data.CobrarOutroDocumento);

    if (_cargaDadosEmissaoGeral.CobrarOutroDocumento.val()) {
        _componenteFrete.CobrarOutroDocumento.val(true);
        _componenteFrete.CobrarOutroDocumento.enable(false);
    } else {
        _componenteFrete.CobrarOutroDocumento.enable(true);
    }

    _componenteFrete.Atualizar.visible(true);
    _componenteFrete.Cancelar.visible(true);
    _componenteFrete.Excluir.visible(true);
    _componenteFrete.Adicionar.visible(false);

    if (_cargaAtual.TipoFreteEscolhido.val() != EnumTipoFreteEscolhido.Embarcador && (data.TipoComponenteFrete == EnumTipoComponenteFrete.ICMS || data.TipoComponenteFrete == EnumTipoComponenteFrete.ISS)) {
        _componenteFrete.Atualizar.visible(false);
        _componenteFrete.Excluir.visible(false);
        _componenteFrete.ModeloDocumentoFiscal.visible(false);
        _componenteFrete.Percentual.visible(false);
        _componenteFrete.ValorComponente.enable(false);
        _componenteFrete.ComponenteFrete.enable(false);
    }

    if (_cargaAtual.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Embarcador) {
        _componenteFrete.Codigo.val(data.Codigo);
        _componenteFrete.Excluir.visible(false);
        _componenteFrete.ModeloDocumentoFiscal.visible(false);
        _componenteFrete.Percentual.visible(false);
        _componenteFrete.ValorComponente.enable(false);
    }

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AdicionarComponentes, _PermissoesPersonalizadasCarga)) {
        _componenteFrete.Atualizar.enable(false);
        _componenteFrete.Excluir.enable(false);
        _componenteFrete.Adicionar.enable(false);
    }

    Global.abrirModal("divModalComponentesFrete");
}

function adicionarComponenteFreteClick(e) {
    limparCamposComponente();

    Global.abrirModal("divModalComponentesFrete");
}

function adicionarComponenteClick(e, sender) {
    if (_componenteFrete.CobrarOutroDocumento.val())
        _componenteFrete.ModeloDocumentoFiscal.required = true;
    else
        _componenteFrete.ModeloDocumentoFiscal.required = false;

    Salvar(e, "CargaComponenteFrete/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                preecherRetornoFrete(_cargaAtual, arg.Data, false);
                limparCamposComponente();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function atualizarComponenteClick(e, sender) {
    if (_componenteFrete.CobrarOutroDocumento.val())
        _componenteFrete.ModeloDocumentoFiscal.required = true;
    else
        _componenteFrete.ModeloDocumentoFiscal.required = false;

    Salvar(e, "CargaComponenteFrete/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                preecherRetornoFrete(_cargaAtual, arg.Data, false);
                limparCamposComponente();
                Global.fecharModal("divModalComponentesFrete");
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function excluirComponenteClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaExcluirComponente.format(e.ComponenteFrete.val()), function () {
        Salvar(e, "CargaComponenteFrete/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    preecherRetornoFrete(_cargaAtual, arg.Data, false);
                    limparCamposComponente();
                    Global.fecharModal("divModalComponentesFrete");
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function cancelarComponenteClick(e) {
    limparCamposComponente();
    Global.fecharModal("divModalComponentesFrete");
}

function limparCamposComponente(e) {
    _componenteFrete.Atualizar.visible(false);
    _componenteFrete.Cancelar.visible(false);
    _componenteFrete.Excluir.visible(false);
    _componenteFrete.Adicionar.visible(true);

    setarPadraoValores();

    _componenteFrete.ModeloDocumentoFiscal.required = false;
    LimparCampos(_componenteFrete);
    _componenteFrete.Carga.val(_cargaAtual.Codigo.val());

    var moeda = _cargaAtual.Moeda.val();
    var cotacaoMoeda = _cargaAtual.ValorCotacaoMoeda.val();

    if (moeda !== null && moeda !== EnumMoedaCotacaoBancoCentral.Real) {
        _componenteFrete.Moeda.val(moeda);
        _componenteFrete.ValorCotacaoMoeda.val(cotacaoMoeda);
        _componenteFrete.ValorTotalMoeda.visible(true);
        _componenteFrete.Moeda.visible(true);
        _componenteFrete.ValorCotacaoMoeda.visible(true);
        _componenteFrete.ValorComponente.enable(false);
        _componenteFrete.ModeloDocumentoFiscal.visible(false);
    } else {
        _componenteFrete.ModeloDocumentoFiscal.visible(true);
        _componenteFrete.ModeloDocumentoFiscal.enable(true);
        _componenteFrete.ValorComponente.enable(true);

        _componenteFrete.ValorTotalMoeda.visible(false);
        _componenteFrete.Moeda.visible(false);
        _componenteFrete.ValorCotacaoMoeda.visible(false);
    }

    _componenteFrete.ComponenteFrete.enable(true);

    if (_cargaDadosEmissaoGeral.CobrarOutroDocumento.val()) {
        _componenteFrete.CobrarOutroDocumento.val(true);
        _componenteFrete.ModeloDocumentoFiscal.val(_cargaDadosEmissaoGeral.ModeloDocumentoFiscal.val());
        _componenteFrete.ModeloDocumentoFiscal.codEntity(_cargaDadosEmissaoGeral.ModeloDocumentoFiscal.codEntity());
        _componenteFrete.CobrarOutroDocumento.enable(false);
    } else {
        _componenteFrete.CobrarOutroDocumento.enable(true);
        _componenteFrete.CobrarOutroDocumento.val(false);
        _componenteFrete.ModeloDocumentoFiscal.val("");
        _componenteFrete.ModeloDocumentoFiscal.codEntity(0);
    }
}

function setarPadraoValores() {
    _componenteFrete.ValorComponente.visible(true);
    _componenteFrete.Percentual.visible(false);
    _componenteFrete.Percentual.required = false;
    _componenteFrete.ValorComponente.required = true;
    _componenteFrete.Percentual.val(0);
    _componenteFrete.ValorComponente.val("");
}

function setarPadraoPercentual() {
    _componenteFrete.ValorComponente.visible(false);
    _componenteFrete.Percentual.visible(true);
    _componenteFrete.Percentual.required = true;
    _componenteFrete.ValorComponente.required = false;
    _componenteFrete.Percentual.val("");
    _componenteFrete.ValorComponente.val(0);
}