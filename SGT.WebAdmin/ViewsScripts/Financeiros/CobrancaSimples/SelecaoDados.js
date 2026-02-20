/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CobrancaSimplesEtapa.js" />
/// <reference path="GeracaoBoleto.js" />
/// <reference path="EnvioEmail.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _selecaoDado;

var SelecaoDado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataVencimento = PropertyEntity({ text: "*Data Vencimento: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.ValorTitulo = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Título:", getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: false, enable: ko.observable(true) });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pessoa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), issue: 52 });
    this.ConfiguracaoBanco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Configuração Banco:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Movimento:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.Proximo = PropertyEntity({ eventClick: SalvarProximoSelecaoDadoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadSelecaoDado() {
    _selecaoDado = new SelecaoDado();
    KoBindings(_selecaoDado, "knockoutSelecaoDados");

    new BuscarClientes(_selecaoDado.Pessoa);
    new BuscarBoletoConfiguracao(_selecaoDado.ConfiguracaoBanco, RetornoConfiguracaoBanco);
    new BuscarTipoMovimento(_selecaoDado.TipoMovimento);

    loadEtapaCobrancaSimples();
    loadGeracaoBoleto();
    loadEnvioEmail();

    buscarConfiguracaoBanco();
}

function ProximoSelecaoDadoClick(e, sender) {
    if (_selecaoDado.Codigo.val() == 0) {
        var valido = ValidarCamposObrigatorios(_selecaoDado);
        _selecaoDado.ValorTitulo.requiredClass("form-control");
        if (Globalize.parseFloat(_selecaoDado.ValorTitulo.val()) <= 0) {
            valido = false;
            _selecaoDado.ValorTitulo.requiredClass("form-control is-invalid");
        }

        if (valido) {
            if (Globalize.parseDate(Global.DataAtual()) <= Globalize.parseDate(_selecaoDado.DataVencimento.val())) {
                Salvar(_selecaoDado, "CobrancaSimples/SalvarTitulo", function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            _selecaoDado.Codigo.val(arg.Data.Codigo);
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Título inserido com sucesso");

                            DesabilitarCampos(_selecaoDado);
                            AtualizarBoletosClick();
                            _etapaAtual = 2;
                            $("#" + _etapaCobrancaSimples.Etapa2.idTab).click();
                            $("#knockoutGeracaoBoleto").show();
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                }, null, function () {
                    exibirMensagem(tipoMensagem.aviso, "Seleção de dados", "Por favor informar os campos obrigatórios para geração do título.");
                });
            } else
                exibirMensagem(tipoMensagem.aviso, "Seleção de dados", "Data de vencimento deve ser maior ou igual da atual.");
        } else {
            exibirMensagem(tipoMensagem.aviso, "Seleção de dados", "Por favor informar os campos obrigatórios para geração do título.");
        }
    }
}

function SalvarProximoSelecaoDadoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_selecaoDado);
    _selecaoDado.ValorTitulo.requiredClass("form-control");
    if (Globalize.parseFloat(_selecaoDado.ValorTitulo.val()) <= 0) {
        valido = false;
        _selecaoDado.ValorTitulo.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var data = {
            TipoMovimento: _selecaoDado.TipoMovimento.codEntity(),
            DataEmissao: Global.DataAtual()
        };

        executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != true && arg.Data.Mensagem != "") {
                    if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                        exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                            ProximoSelecaoDadoClick(e, sender);
                        });
                    } else
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                } else {
                    ProximoSelecaoDadoClick(e, sender);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

//*******MÉTODOS*******

function buscarConfiguracaoBanco() {
    executarReST("CobrancaSimples/BuscarConfiguracaoBanco", null, function (r) {
        if (r.Success) {
            if (r.Data != null) {
                _selecaoDado.ConfiguracaoBanco.codEntity(r.Data.Codigo);
                _selecaoDado.ConfiguracaoBanco.val(r.Data.Nome);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function RetornoConfiguracaoBanco(data) {
    _selecaoDado.ConfiguracaoBanco.codEntity(data.Codigo);
    _selecaoDado.ConfiguracaoBanco.val(data.DescricaoBanco);
}

function limparCamposCobrancaSimples() {
    LimparCampos(_selecaoDado);
    LimparCampos(_geracaoBoleto);
    LimparCampos(_envioEmail);
    HabilitarCampos(_selecaoDado);
    buscarConfiguracaoBanco();

    AtualizarBoletosClick();
    $("#knockoutGeracaoBoleto").hide();
    $("#knockoutEnvioEmail").hide();
    _etapaAtual = 1;
    $("#" + _etapaCobrancaSimples.Etapa1.idTab).click();
}

function DesabilitarCampos(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === true || knout.enable === false)
                knout.enable = false;
            else
                knout.enable(false);
        }
    });
}

function HabilitarCampos(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === false || knout.enable === true)
                knout.enable = true;
            else
                knout.enable(true);
        }
    });
}