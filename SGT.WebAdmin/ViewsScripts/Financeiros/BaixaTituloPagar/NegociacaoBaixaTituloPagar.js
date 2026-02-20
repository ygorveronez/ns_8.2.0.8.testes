/// <reference path="../../Consultas/TipoPagamento.js" />
/// <reference path="IntegracaoBaixaTituloPagar.js" />
/// <reference path="../../Consultas/Justificativa.js" />
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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CabecalhoBaixaTituloPagar.js" />
/// <reference path="BaixaTituloPagar.js" />
/// <reference path="../../Consultas/Conhecimento.js" />
/// <reference path="EtapaBaixaTituloPagar.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _negociacaoBaixa;
var _gridParcelasNegociacao;
var _HTMLNegociacaoBaixa;
var _detalheParcela;
var _novoDescontoAcrescimo;
var _gridAcrescimosDescontos;
var _gridTiposDesPagamentos;

var NegociacaoBaixa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroTitulo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorOriginal = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorABaixar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DataBaixar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoDevedor = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: false });
    this.Operador = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Descontos = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Acrescimos = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorPendente = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.MoedaCotacaoBancoCentralNegociacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.DataBaseCRTNegociacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.ValorMoedaCotacaoNegociacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.ValorOriginalMoedaEstrangeiraNegociacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.SaldoContaAdiantamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });

    this.TipoDePagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Tipo de Pagamento:"), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorTipoPagamento = PropertyEntity({ text: "*Valor: ", getType: typesKnockout.decimal, enable: ko.observable(true), required: ko.observable(false), val: ko.observable("") });
    this.AdicionarTipoDePagamento = PropertyEntity({ eventClick: AdicionarTipoDePagamentoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.TiposDesPagamentos = PropertyEntity({ type: types.map, required: false, text: "Contas do Pagamento", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.AdicionarAcrescimoDesconto = PropertyEntity({ eventClick: InformarAcrescimoDescontoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.AcrescimosDescontos = PropertyEntity({ type: types.map, required: false, text: "Acréscimos / Descontos", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    //this.QuantidadeParcelas = PropertyEntity({ val: ko.observable("1"), options: _qtdParcelas, text: "Qtd. Parcelas: ", def: "1", enable: ko.observable(true) });
    this.QuantidadeParcelas = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Qtd. Parcelas:", maxlength: 100, enable: ko.observable(true), onfigInt: { precision: 0, allowZero: false } });
    this.IntervaloDeDias = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Intervalo de Dias:", maxlength: 10, enable: ko.observable(true) });
    this.DataPrimeiroVencimento = PropertyEntity({ text: "Data Vencimento: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissao = PropertyEntity({ text: "Data Última Emissão: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.TipoArredondamento = PropertyEntity({ val: ko.observable("1"), options: EnumTipoArredondamento.ObterOpcoes(), text: "Arredondar Valor: ", def: "1", enable: ko.observable(true) });
    this.GerarParcelas = PropertyEntity({ eventClick: GerarParcelasClick, type: types.event, text: "Gerar Parcelas", visible: ko.observable(true), enable: ko.observable(true) });
    this.Parcelas = PropertyEntity({ type: types.map, required: false, text: "Parcelas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });

    this.PessoaNegociacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Fornecedor:"), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true), required: ko.observable(false) });

    this.FecharBaixa = PropertyEntity({ eventClick: SalvarFecharBaixaClick, type: types.event, text: "Fechar Baixa", visible: ko.observable(true), enable: ko.observable(true) });
    this.ImprimirRecibo = PropertyEntity({ eventClick: ImprimirReciboClick, type: types.event, text: "Imprimir Recibo", visible: ko.observable(false), enable: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
    this.ChequeBaixa = PropertyEntity({ type: types.event, text: "Adicionar Cheques", idBtnSearch: guid(), visible: ko.observable(false) });
    this.AdicionarCheque = PropertyEntity({ eventClick: AdicionarChequeClick, type: types.event, text: "Novo Cheque", visible: ko.observable(true), enable: ko.observable(true) });
};

var AdicionarDescontoAcrescimo = function () {
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 15 });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.MoedaCotacaoBancoCentral.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeiraDescontoAcrescimo();
    });

    this.DataBaseCRT.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeiraDescontoAcrescimo();
    });

    this.ValorMoedaCotacao.val.subscribe(function (novoValor) {
        ConverterValorDescontoAcrescimo();
    });

    this.ValorOriginalMoedaEstrangeira.val.subscribe(function (novoValor) {
        ConverterValorOriginalMoedaEstrangeiraDescontoAcrescimo();
    });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarDescontoAcrescimoClick, text: "Adicionar", visible: ko.observable(true) });
};

var DetalheParcela = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Sequencia = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Sequência:", maxlength: 10, enable: ko.observable(false) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor Parcela:", maxlength: 12, enable: ko.observable(true) });
    this.Desconto = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Desconto:", maxlength: 10, enable: ko.observable(false) });
    this.Acrescimo = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Acréscimo:", maxlength: 10, enable: ko.observable(false) });
    this.Total = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Total:", maxlength: 10, enable: ko.observable(false) });
    this.ValorDesconto = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor Desconto:", maxlength: 10, enable: ko.observable(true), visible: ko.observable(false) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, required: false, text: "Data Emissão:", enable: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data Vencimento:", enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: false, enable: ko.observable(true) });
    this.NumeroBoleto = PropertyEntity({ text: "Número Boleto:", val: ko.observable(""), def: "", required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.CodigoBarrasParaLinhaDigitavel = PropertyEntity({
        eventClick: CodigoBarrasParaLinhaDigitavelBaixaClick, type: types.event, text: ko.observable("Código de Barras para Linha Digitável"), visible: ko.observable(true), enable: ko.observable(true), icon: "fa fa-fw fa-gears"
    });
    this.LinhaDigitavelParaCodigoBarras = PropertyEntity({
        eventClick: LinhaDigitavelParaCodigoBarrasBaixaClick, type: types.event, text: ko.observable("Linha Digitável para Código de Barras"), visible: ko.observable(true), enable: ko.observable(true), icon: "fa fa-fw fa-gears"
    });
    this.ValidarValorBruto = PropertyEntity({
        eventClick: ValidarValorBrutoBaixaClick, type: types.event, text: ko.observable("Validar Valor Bruto"), visible: ko.observable(true), enable: ko.observable(true), icon: "fa fa-fw fa-check"
    });
    this.Portador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Portador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor Original Moeda Estrangeira:", maxlength: 10, enable: ko.observable(true), visible: ko.observable(false) });

    this.SalvarParcela = PropertyEntity({ type: types.event, eventClick: SalvarParcelaClick, text: "Salvar Parcela", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadNegociacaoBaixa() {
    carregarConteudosHTMLNegociacaoBaixa(function () {
        $("#contentNegociacaoTituloPagar").html("");
        var idDiv = guid();
        $("#contentNegociacaoTituloPagar").append(_HTMLNegociacaoBaixa.replace(/#divNegociacaoBaixaTituloPagar/g, idDiv));
        _negociacaoBaixa = new NegociacaoBaixa();
        KoBindings(_negociacaoBaixa, idDiv);

        new BuscarClientes(_negociacaoBaixa.PessoaNegociacao, null, null, null, null, null, null, null, _baixaTituloPagar);

        LoadChequeBaixa();

        _novoDescontoAcrescimo = new AdicionarDescontoAcrescimo();
        KoBindings(_novoDescontoAcrescimo, "knoutAdicionarAcrescimoDesconto");

        new BuscarJustificativas(_novoDescontoAcrescimo.Justificativa, null, null, EnumTipoFinalidadeJustificativa.TitulosPagar);

        _detalheParcela = new DetalheParcela();
        KoBindings(_detalheParcela, "knoutDetalheParcela");

        new BuscarTipoPagamento(_negociacaoBaixa.TipoDePagamento);
        new BuscarClientes(_detalheParcela.Portador);

        var detalhe = { descricao: "Detalhe", id: guid(), evento: "onclick", metodo: DetalheParcelaNegociacaoClick, tamanho: "10", icone: "" };
        var menuOpcoes = new Object();
        menuOpcoes.tipo = TypeOptionMenu.link;
        menuOpcoes.opcoes = new Array();
        menuOpcoes.opcoes.push(detalhe);

        _gridParcelasNegociacao = new GridView(_negociacaoBaixa.Parcelas.idGrid, "BaixaTituloPagar/PesquisaParcelasNegociacao", _negociacaoBaixa, menuOpcoes, { column: 2, dir: orderDir.asc }, null, null);
        _gridParcelasNegociacao.CarregarGrid();

        if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
            _negociacaoBaixa.MoedaCotacaoBancoCentralNegociacao.visible(true);
            _negociacaoBaixa.DataBaseCRTNegociacao.visible(true);
            _negociacaoBaixa.ValorMoedaCotacaoNegociacao.visible(true);
            _negociacaoBaixa.ValorOriginalMoedaEstrangeiraNegociacao.visible(true);
            _detalheParcela.ValorOriginalMoedaEstrangeira.visible(true);

            _novoDescontoAcrescimo.MoedaCotacaoBancoCentral.visible(true);
            _novoDescontoAcrescimo.DataBaseCRT.visible(true);
            _novoDescontoAcrescimo.ValorMoedaCotacao.visible(true);
            _novoDescontoAcrescimo.ValorOriginalMoedaEstrangeira.visible(true);
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
            _negociacaoBaixa.TipoDePagamento.text("*Tipo de Pagamento:");
            _negociacaoBaixa.TipoDePagamento.visible(true);
            _negociacaoBaixa.TipoDePagamento.required(false);
        }

        carregarGridAcrescimosDescontos();
        carregarGridTiposDesPagamentos();

    });
}

function CodigoBarrasParaLinhaDigitavelBaixaClick(e, sender) {
    if (_detalheParcela.NumeroBoleto.val() !== "") {
        var linhaDigitavel = calcula_linha(_detalheParcela.NumeroBoleto.val());
        if (linhaDigitavel !== undefined && linhaDigitavel !== "")
            _detalheParcela.NumeroBoleto.val(linhaDigitavel);
    }
}

function LinhaDigitavelParaCodigoBarrasBaixaClick(e, sender) {
    if (_detalheParcela.NumeroBoleto.val() !== "") {
        var codigoBarras = calcula_barra(_detalheParcela.NumeroBoleto.val());
        if (codigoBarras !== undefined && codigoBarras !== "")
            _detalheParcela.NumeroBoleto.val(codigoBarras);
    }
}

function ValidarValorBrutoBaixaClick(e, sender) {
    if (_detalheParcela.NumeroBoleto.val() !== "" && _detalheParcela.Valor.val() !== "") {
        validar_valor(_detalheParcela.NumeroBoleto.val(), _detalheParcela.Valor.val());
    }
}

function AdicionarDescontoAcrescimoClick(e, sender) {
    if (_baixaTituloPagar.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    if (ValidarCamposObrigatorios(e)) {
        var data = {
            Codigo: _baixaTituloPagar.Codigo.val(),
            Valor: e.Valor.val(),
            Justificativa: e.Justificativa.codEntity(),
            MoedaCotacaoBancoCentral: e.MoedaCotacaoBancoCentral.val(),
            DataBaseCRT: e.DataBaseCRT.val(),
            ValorMoedaCotacao: e.ValorMoedaCotacao.val(),
            ValorOriginalMoedaEstrangeira: e.ValorOriginalMoedaEstrangeira.val()
        };
        executarReST("BaixaTituloPagar/InserirAcrescimoDesconto", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    LimparCampos(e);

                    _gridAcrescimosDescontos.CarregarGrid();
                    CarregarNegociacaoBaixa();
                    Global.abrirModal("divAdicionarAcrescimoDesconto");
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function AdicionarTipoDePagamentoClick(e, sender) {
    if (_baixaTituloPagar.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    _negociacaoBaixa.ValorTipoPagamento.required(true);
    _negociacaoBaixa.TipoDePagamento.required(true);

    if (ValidarCamposObrigatorios(_negociacaoBaixa)) {

        _negociacaoBaixa.ValorTipoPagamento.required(false);
        _negociacaoBaixa.TipoDePagamento.required(false);
        var data = {
            Codigo: _baixaTituloPagar.Codigo.val(),
            Valor: _negociacaoBaixa.ValorTipoPagamento.val(),
            TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity(),
            MoedaCotacaoBancoCentral: _baixaTituloPagar.MoedaCotacaoBancoCentral.val(),
            DataBaseCRT: _baixaTituloPagar.DataBaseCRT.val(),
            ValorMoedaCotacao: _baixaTituloPagar.ValorMoedaCotacao.val()
        };
        executarReST("BaixaTituloPagar/InserirTipoPagamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    LimparCampoEntity(_negociacaoBaixa.TipoDePagamento);
                    _negociacaoBaixa.ValorTipoPagamento.val(arg.Data.ValorTipoPagamento);

                    _gridTiposDesPagamentos.CarregarGrid();
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    } else {
        _negociacaoBaixa.ValorTipoPagamento.required(false);
        _negociacaoBaixa.TipoDePagamento.required(false);
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function InformarAcrescimoDescontoClick(e, sender) {
    if (_baixaTituloPagar.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }
    LimparCampos(_novoDescontoAcrescimo);

    _novoDescontoAcrescimo.MoedaCotacaoBancoCentral.val(_baixaTituloPagar.MoedaCotacaoBancoCentral.val());
    _novoDescontoAcrescimo.DataBaseCRT.val(_baixaTituloPagar.DataBaseCRT.val());
    _novoDescontoAcrescimo.ValorMoedaCotacao.val(_baixaTituloPagar.ValorMoedaCotacao.val());

    Global.abrirModal("divAdicionarAcrescimoDesconto");
}

function GerarParcelasClick(e, sender) {
    if (_baixaTituloPagar.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    _negociacaoBaixa.IntervaloDeDias.requiredClass("form-control");
    _negociacaoBaixa.DataPrimeiroVencimento.requiredClass("form-control");
    _negociacaoBaixa.DataEmissao.requiredClass("form-control");
    _negociacaoBaixa.QuantidadeParcelas.requiredClass("form-control");

    var valido = true;
    if (_negociacaoBaixa.DataPrimeiroVencimento.val() == "") {
        valido = false;
        _negociacaoBaixa.DataPrimeiroVencimento.requiredClass("form-control is-invalid");
    }
    if (_negociacaoBaixa.DataEmissao.val() == "") {
        valido = false;
        _negociacaoBaixa.DataEmissao.requiredClass("form-control is-invalid");
    }
    if (_negociacaoBaixa.QuantidadeParcelas.val() == "" || _negociacaoBaixa.QuantidadeParcelas.val() == 0) {
        valido = false;
        _negociacaoBaixa.QuantidadeParcelas.requiredClass("form-control is-invalid");
    }

    if (valido) {
        exibirConfirmacao("Confirmação", "Realmente deseja gerar as parcelas?", function () {
            var data = {
                Codigo: _baixaTituloPagar.Codigo.val(),
                QuantidadeParcelas: _negociacaoBaixa.QuantidadeParcelas.val(),
                IntervaloDeDias: _negociacaoBaixa.IntervaloDeDias.val(),
                DataPrimeiroVencimento: _negociacaoBaixa.DataPrimeiroVencimento.val(),
                DataEmissao: _negociacaoBaixa.DataEmissao.val(),
                TipoArredondamento: _negociacaoBaixa.TipoArredondamento.val(),
                TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity(),
                PessoaNegociacao: _negociacaoBaixa.PessoaNegociacao.codEntity(),
                FormaTitulo: EnumFormaTitulo.Outros
            };
            executarReST("BaixaTituloPagar/GerarParcelas", data, function (arg) {
                if (arg.Success) {
                    CarregarNegociacaoBaixa();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            });
        });
    } else
        exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Por favor verifique os campos obrigatórios.");
}

function SalvarParcelaClick(e, sender) {
    if (_baixaTituloPagar.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    if (ValidarCamposObrigatorios(e)) {
        var data = {
            Codigo: _baixaTituloPagar.Codigo.val(),
            CodigoParcela: e.Codigo.val(),
            Sequencia: e.Sequencia.val(),
            Valor: e.Valor.val(),
            ValorDesconto: e.ValorDesconto.val(),
            DataEmissao: e.DataEmissao.val(),
            DataVencimento: e.DataVencimento.val(),
            FormaTitulo: e.FormaTitulo.val(),
            NumeroBoleto: e.NumeroBoleto.val(),
            Portador: e.Portador.codEntity(),
            ValorOriginalMoedaEstrangeira: e.ValorOriginalMoedaEstrangeira.val()
        };
        executarReST("BaixaTituloPagar/AtualizarParcela", data, function (arg) {
            if (arg.Success) {
                LimparCampos(e);
                Global.fecharModal("divDetalheParcela");
                CarregarNegociacaoBaixa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 8000);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function ImprimirReciboClick(e, sender) {
    var data = { Codigo: _baixaTituloPagar.Codigo.val(), TituloPagar: true };
    executarReST("MovimentoFinanceiro/GerarRecibo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function FecharBaixaClick(e, sender) {
    if (_baixaTituloPagar.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    if (ValidarCamposObrigatorios(_negociacaoBaixa)) {
        exibirConfirmacao("Confirmação", "Realmente deseja fechar a baixa selecionada?", function () {
            var data = {
                Codigo: _baixaTituloPagar.Codigo.val(),
                Etapa: EnumEtapasBaixaTituloPagar.Finalizada,
                TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity(),
                PessoaNegociacao: _negociacaoBaixa.PessoaNegociacao.codEntity()
            };
            executarReST("BaixaTituloPagar/FecharBaixa", data, function (arg) {
                if (arg.Success) {
                    _baixaTituloPagar.Etapa.val(EnumEtapasBaixaTituloPagar.Finalizada);
                    CarregarDadosCabecalho(arg.Data);
                    PosicionarEtapa(arg.Data);
                    VerificarBotoes();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            });
        });
    } else {
        exibirCamposObrigatorio();
    }
}

function AdicionarChequeClick(e, sender) {
    limparCamposCheque();

    Global.abrirModal("divModalCheque");
}

function SalvarFecharBaixaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_negociacaoBaixa);

    if (valido) {
        var data = {
            BaixaTitulo: _baixaTituloPagar.Codigo.val()
        };

        executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != true && arg.Data.Mensagem != "") {
                    if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                        exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                            FecharBaixaClick(e, sender);
                        });
                    } else
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                } else {
                    FecharBaixaClick(e, sender);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function DetalheParcelaNegociacaoClick(e, sender) {
    if (_baixaTituloPagar == null || _baixaTituloPagar.Codigo == null || _baixaTituloPagar.Codigo.val() == null || _baixaTituloPagar.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de título antes.");
        return;
    }

    LimparCampos(_detalheParcela);

    if (e.Codigo > 0 && e.Codigo != "") {
        var data =
        {
            Codigo: e.Codigo,
            TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity()
        };
        executarReST("BaixaTituloPagar/CarregarDadosParcela", data, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataParcela = { Data: e.Data };
                    PreencherObjetoKnout(_detalheParcela, dataParcela);
                    Global.abrirModal("divDetalheParcela");
                }
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", e.Msg);
            }
        });
    }
}

function RemoverTipodePagamentoClick(e) {
    if (_baixaTituloPagar.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo de Pagamento selecionado?", function () {
        var data = {
            Codigo: e.Codigo,
            CodigoBaixa: _baixaTituloPagar.Codigo.val()
        };
        executarReST("BaixaTituloPagar/RemoverTipoPagamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _negociacaoBaixa.ValorTipoPagamento.val(arg.Data.ValorTipoPagamento);
                    _gridTiposDesPagamentos.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function RemoverAcrescimoDescontoClick(e) {

    if (_baixaTituloPagar.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Acréscimo/Desconto selecionado?", function () {
        var data = {
            Codigo: e.Codigo,
            CodigoBaixa: _baixaTituloPagar.Codigo.val()
        };
        executarReST("BaixaTituloPagar/RemoverAcrescimoDesconto", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridAcrescimosDescontos.CarregarGrid();
                    CarregarNegociacaoBaixa();
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function carregarGridAcrescimosDescontos() {
    var editar = { descricao: "Excluir", id: "clasEditar", evento: "onclick", metodo: RemoverAcrescimoDescontoClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAcrescimosDescontos = new GridView(_negociacaoBaixa.AcrescimosDescontos.idGrid, "BaixaTituloPagar/PesquisarAcrescimoDesconto", _negociacaoBaixa, menuOpcoes, null, null, null);
    _gridAcrescimosDescontos.CarregarGrid();
}

function carregarGridTiposDesPagamentos() {
    var editar = { descricao: "Excluir", id: "clasEditar", evento: "onclick", metodo: RemoverTipodePagamentoClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTiposDesPagamentos = new GridView(_negociacaoBaixa.TiposDesPagamentos.idGrid, "BaixaTituloPagar/PesquisarTiposDesPagamentos", _negociacaoBaixa, menuOpcoes, null, null, null);
    _gridTiposDesPagamentos.CarregarGrid();
}

function LimparCamposNegociacao() {
    LimparCampos(_negociacaoBaixa);
    _negociacaoBaixa.PessoaNegociacao.visible(false);
    _negociacaoBaixa.PessoaNegociacao.required(false);
    _negociacaoBaixa.PessoaNegociacao.text("Fornecedor:");
    _gridParcelasNegociacao.CarregarGrid();
    _gridAcrescimosDescontos.CarregarGrid();
    _gridTiposDesPagamentos.CarregarGrid();
    LimparCamposChequeBaixa();
}

function CarregarNegociacaoBaixa() {
    var codigoPessoaNegociacao = _negociacaoBaixa.PessoaNegociacao.codEntity();
    var codigoTipoDePagamento = _negociacaoBaixa.TipoDePagamento.codEntity();
    LimparCampos(_negociacaoBaixa);

    if (_baixaTituloPagar.Codigo.val() > 0 && _baixaTituloPagar.Codigo.val() != "") {
        var data =
        {
            Codigo: _baixaTituloPagar.Codigo.val(),
            TipoDePagamento: codigoTipoDePagamento,
            PessoaNegociacao: codigoPessoaNegociacao
        };
        executarReST("BaixaTituloPagar/CarregarNegociacaoBaixa", data, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataNegociacao = { Data: e.Data };
                    PreencherObjetoKnout(_negociacaoBaixa, dataNegociacao);

                    _baixaTituloPagar.ValorBaixado.val(dataNegociacao.Data.ValorABaixar);

                    _gridParcelasNegociacao.CarregarGrid();
                    _gridAcrescimosDescontos.CarregarGrid();
                    _gridTiposDesPagamentos.CarregarGrid();
                    carregarDadosChequeBaixa();

                    _negociacaoBaixa.PessoaNegociacao.visible(false);
                    _negociacaoBaixa.PessoaNegociacao.required(false);
                    _negociacaoBaixa.PessoaNegociacao.text("Fornecedor:");

                    if (_baixaTituloPagar.Situacao.val() == 1 || _baixaTituloPagar.Situacao.val() == 2)
                        _negociacaoBaixa.SaldoContaAdiantamento.visible(true);
                    else
                        _negociacaoBaixa.SaldoContaAdiantamento.visible(false);

                    if (dataNegociacao.Data.CodigoFornecedor > 0 && dataNegociacao.Data.QuantidadePessoa <= 1)
                        $('#divParcelasNegociacal').show();
                    else if (dataNegociacao.Data.QuantidadePessoa > 1) {
                        $('#divParcelasNegociacal').show();
                        _negociacaoBaixa.PessoaNegociacao.visible(true);
                        if (Globalize.parseFloat(dataNegociacao.Data.ValorPendente) > 0) {
                            _negociacaoBaixa.PessoaNegociacao.required(true);
                            _negociacaoBaixa.PessoaNegociacao.text("*Fornecedor:");
                        }
                        else {
                            _negociacaoBaixa.PessoaNegociacao.required(false);
                            _negociacaoBaixa.PessoaNegociacao.text("Fornecedor:");
                        }
                    } else
                        $('#divParcelasNegociacal').hide();

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
                        _negociacaoBaixa.TipoDePagamento.text("*Tipo de Pagamento:");
                        _negociacaoBaixa.TipoDePagamento.visible(true);
                        _negociacaoBaixa.TipoDePagamento.required(false);
                    } else if (parseFloat(_baixaTituloPagar.ValorBaixado.val().toString().replace(".", "").replace(",", ".")) > 0) {
                        _negociacaoBaixa.TipoDePagamento.text("*Tipo de Pagamento:");
                        _negociacaoBaixa.TipoDePagamento.required(false);
                    } else {
                        _negociacaoBaixa.TipoDePagamento.text("*Tipo de Pagamento:");
                        _negociacaoBaixa.TipoDePagamento.required(false);
                    }
                }
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", e.Msg);
            }
        });
    } else if (_gridParcelasNegociacao != null) {
        _gridParcelasNegociacao.CarregarGrid();
    }
}

function carregarConteudosHTMLNegociacaoBaixa(callback) {
    $.get("Content/Static/Financeiro/NegociacaoBaixaTituloPagar.html?dyn=" + guid(), function (data) {
        _HTMLNegociacaoBaixa = data;
        $.get("Content/Static/Financeiro/BaixaTituloPagarModais.html?dyn=" + guid(), function (data) {
            $("#ModaisTituloPagar").html(data);
            callback();
        });
    });
}

function ConverterValorNegociacao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_baixaTituloPagar.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_detalheParcela.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _detalheParcela.Valor.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorEstrangeiraNegociacao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_baixaTituloPagar.ValorMoedaCotacao.val());
        var valor = Globalize.parseFloat(_detalheParcela.Valor.val());
        if (valor > 0 && valorMoedaCotacao > 0) {
            _detalheParcela.ValorOriginalMoedaEstrangeira.val(Globalize.format(valor / valorMoedaCotacao, "n2"));
        }
    }
}

function CalcularMoedaEstrangeiraDescontoAcrescimo() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira && _novoDescontoAcrescimo.DataBaseCRT.val() != null && _novoDescontoAcrescimo.DataBaseCRT.val() != undefined && _novoDescontoAcrescimo.DataBaseCRT.val() != "") {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _novoDescontoAcrescimo.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _novoDescontoAcrescimo.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                if (r.Data != null && r.Data != undefined && r.Data > 0)
                    _novoDescontoAcrescimo.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValorDescontoAcrescimo();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorDescontoAcrescimo() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novoDescontoAcrescimo.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novoDescontoAcrescimo.Valor.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0)
            _novoDescontoAcrescimo.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        else
            _novoDescontoAcrescimo.ValorOriginalMoedaEstrangeira.val("0,00");
    }
}

function ConverterValorOriginalMoedaEstrangeiraDescontoAcrescimo() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira && _novoDescontoAcrescimo.MoedaCotacaoBancoCentral.val() != EnumMoedaCotacaoBancoCentral.Real) {
        var valorMoedaCotacao = Globalize.parseFloat(_novoDescontoAcrescimo.ValorMoedaCotacao.val());
        var valorOriginalMoedaEstrangeira = Globalize.parseFloat(_novoDescontoAcrescimo.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginalMoedaEstrangeira > 0 && valorMoedaCotacao > 0) {
            _novoDescontoAcrescimo.Valor.val(Globalize.format(valorOriginalMoedaEstrangeira * valorMoedaCotacao, "n2"));
        }
        else
            _novoDescontoAcrescimo.Valor.val("0,00");
    }
}