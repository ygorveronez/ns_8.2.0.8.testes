/// <reference path="../../Enumeradores/EnumTipoFinalidadeJustificativa.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="IntegracaoFatura.js" />
/// <reference path="../../Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Enumeradores/EnumTipoArquivoRelatorio.js" />
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
/// <reference path="Fatura.js" />
/// <reference path="EtapaFatura.js" />
/// <reference path="CargaFatura.js" />
/// <reference path="CabecalhoFatura.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _qtdParcelas = [
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
    { text: "7", value: 7 },
    { text: "8", value: 8 },
    { text: "9", value: 9 },
    { text: "10", value: 10 },
    { text: "11", value: 11 },
    { text: "12", value: 12 },
    { text: "13", value: 13 },
    { text: "14", value: 14 },
    { text: "15", value: 15 },
    { text: "16", value: 16 },
    { text: "17", value: 17 },
    { text: "18", value: 18 },
    { text: "19", value: 19 },
    { text: "20", value: 20 }
];

var _fechamentoFatura;
var _gridParcelasFatura;
var _HTMLFechamentoFatura;
var _detalheParcela;
var _liquidarFatura;
var _gridRelatorio;
var _novoDescontoAcrescimo;
var _gridAcrescimosDescontos;
var _gridCTesRemoverIntegracao;
var _codigoTituloSelecionado;
var _modalAdicionarAcrescimoDescontoFatura;
var _modalSelecaoFormaPagamentoLiquidarFatura;
var _modalDetalheParcela;

var FechamentoFatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ValorTotal = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Descontos = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Acrescimos = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalGeral = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TipoMoeda = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });

    this.Moeda = PropertyEntity({ text: "Moeda:", visible: ko.observable(false) });

    this.DescontosMoeda = PropertyEntity({ text: "Descontos em Moeda:", visible: ko.observable(false) });
    this.AcrescimosMoeda = PropertyEntity({ text: "Acréscimos em Moeda:", visible: ko.observable(false) });
    this.ValorTotalMoeda = PropertyEntity({ text: "Valor em Moeda:", visible: ko.observable(false) });

    this.AdicionarAcrescimoDesconto = PropertyEntity({ eventClick: InformarAcrescimoDescontoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.AcrescimosDescontos = PropertyEntity({ type: types.map, required: false, text: "Acréscimos / Descontos", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.GerarTituloPorDocumentoFiscal = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Os títulos desta fatura são gerados por documento fiscal, de acordo com a configuração do tomador da fatura.", def: false, enable: ko.observable(true) });

    this.ObservacaoFatura = PropertyEntity({ text: "Observação da Fatura:", maxlength: 1000, enable: ko.observable(true) });
    this.ImprimirObservacaoFatura = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Imprimir observação na Fatura", def: false, enable: ko.observable(true) });
    this.SalvarValores = PropertyEntity({ eventClick: SalvarValoresFaturaClick, type: types.event, text: ko.observable("Salvar"), visible: ko.observable(false), enable: ko.observable(true) });
    this.SalvarObservacao = PropertyEntity({ eventClick: SalvarObservacaoFaturaClick, type: types.event, text: ko.observable("Salvar Obsevação"), visible: ko.observable(false), enable: ko.observable(true) });

    this.TomadorFatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador da Fatura: ", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.EmpresaFatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa: ", idBtnSearch: guid(), required: false, enable: ko.observable(true) });

    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banco: ", idBtnSearch: guid(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Agencia = PropertyEntity({ text: ko.observable("Agência: "), required: false, visible: ko.observable(true), maxlength: 10, enable: ko.observable(true) });
    this.Digito = PropertyEntity({ text: ko.observable("Dígito: "), required: false, visible: ko.observable(true), maxlength: 1, enable: ko.observable(true) });
    this.NumeroConta = PropertyEntity({ text: ko.observable("Número Conta: "), required: false, visible: ko.observable(true), maxlength: 10, enable: ko.observable(true) });
    this.TipoConta = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoes(), def: EnumTipoConta.Corrente, text: "Tipo: ", required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.QuantidadeParcelas = PropertyEntity({ val: ko.observable("1"), options: _qtdParcelas, text: "Qtd. Parcelas: ", def: "1", enable: ko.observable(true) });
    this.IntervaloDeDias = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Intervalo de Dias:", maxlength: 10, enable: ko.observable(true) });
    this.DataPrimeiroVencimento = PropertyEntity({ text: "Data Vencimento: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.TipoArredondamento = PropertyEntity({ val: ko.observable("1"), options: EnumTipoArredondamento.ObterOpcoes(), text: "Arredondar Valor: ", def: "1", enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "Forma do Título: ", def: EnumFormaTitulo.Outros, enable: ko.observable(true) });
    this.GerarParcelas = PropertyEntity({ eventClick: GerarParcelasClick, type: types.event, text: "Gerar Parcelas", visible: ko.observable(true), enable: ko.observable(true) });
    this.AtualizarParcelas = PropertyEntity({ eventClick: AtualizarParcelasClick, type: types.event, text: "Atualizar Parcelas", visible: ko.observable(true), enable: ko.observable(true) });
    this.GerarBoleto = PropertyEntity({ type: types.event, text: "Gerar Boleto", visible: ko.observable(false), enable: ko.observable(false), idBtnSearch: guid() });
    this.Parcelas = PropertyEntity({ type: types.map, required: false, text: "Parcelas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });

    this.PermiteRemoverCTesIntegracao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.CTesRemoverIntegracao = PropertyEntity({ type: types.map, visible: ko.observable(true), idGrid: guid() });
    this.SalvarCTeRemoverIntegracao = PropertyEntity({ eventClick: SalvarCTeRemoverIntegracaoClick, type: types.event, text: "Adicionar CT-e", visible: ko.observable(true), enable: ko.observable(true) });

    this.ReAbrirFatura = PropertyEntity({ eventClick: ReAbrirFaturaClick, type: types.event, text: ko.observable("Cancelar Fatura"), visible: ko.observable(false), enable: ko.observable(true) });
    this.FecharFatura = PropertyEntity({ eventClick: FecharFaturaClick, type: types.event, text: "Fechar Fatura", visible: ko.observable(true), enable: ko.observable(true) });
    this.VisualizarFatura = PropertyEntity({ eventClick: VisualizarFaturaClick, type: types.event, text: ko.observable("Visualizar Fatura"), visible: ko.observable(true), enable: ko.observable(true) });
    this.LiquidarFatura = PropertyEntity({ eventClick: AbrirTelaLiquidarFatura, type: types.event, text: "Liquidar Fatura", visible: ko.observable(false), enable: ko.observable(true) });
    this.DownloadPreviewDOCCOB = PropertyEntity({ eventClick: DownloadPreviewDOCCOBClick, type: types.event, text: "Preview DOCCOB", icon: "fal fa-download", visible: ko.observable(false), enable: ko.observable(true) });

    this.PercentualProcessadoFechamento = PropertyEntity({ val: ko.observable("0%"), def: "0%", text: ko.observable("Finalizando esta Fatura"), visible: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.GerarTituloPorDocumentoFiscal.val.subscribe(function (novoValor) {
        _fechamentoFatura.AdicionarAcrescimoDesconto.visible(!novoValor);
        _fechamentoFatura.AcrescimosDescontos.visible(!novoValor);
    });
};

var AdicionarDescontoAcrescimo = function () {
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10 });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarDescontoAcrescimoClick, text: "Adicionar", visible: ko.observable(true) });
};

var DetalheParcela = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Sequencia = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Sequência:", maxlength: 10, enable: ko.observable(false) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10, enable: ko.observable(true) });
    this.ValorDesconto = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor Desconto:", maxlength: 10, enable: ko.observable(true), visible: ko.observable(false) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "Forma do Título: ", def: EnumFormaTitulo.Outros, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, required: false, text: "Data Emissão:", enable: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data Vencimento:", enable: ko.observable(true) });

    this.SalvarParcela = PropertyEntity({ type: types.event, eventClick: SalvarParcelaClick, text: "Salvar Parcela", visible: ko.observable(true), enable: ko.observable(true) });
};

var LiquidarFatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.FormaPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Forma de Pagamento:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true), issue: 464, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.DataBaixa = PropertyEntity({ text: "*Data Baixa:", getType: typesKnockout.date, enable: ko.observable(true), required: true, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.DataBase = PropertyEntity({ text: "*Data Base:", getType: typesKnockout.date, enable: ko.observable(true), required: true, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.DataBaseCRT = PropertyEntity({ text: "*Data Base CRT: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: false, visible: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-2 col-lg-2") });

    this.GridAcrescimosDescontos = PropertyEntity({ type: types.local, idGrid: guid() });
    this.ListaAcrescimosDescontos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, val: ko.observable([]), def: [] });
    this.AcrescimosDescontos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.AdicionarAcrescimoDesconto = PropertyEntity({ eventClick: AbrirTelaAcrescimoDescontoLiquidacaoFatura, type: types.event, text: "Adicionar Acréscimo/Desconto", visible: ko.observable(true), enable: ko.observable(true) });
    this.LiquidarFatura = PropertyEntity({ eventClick: LiquidarFaturaClick, type: types.event, text: "Liquidar Fatura", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarLiquidarFaturaClick, type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadFechamentoFatura() {
    carregarConteudosHTMLFechamentoCarga(function () {
        $("#contentFinalizacaoFatura").html("");
        var idDiv = guid();
        $("#contentFinalizacaoFatura").append(_HTMLFechamentoFatura.replace(/#divFechamentoFatura/g, idDiv));
        _fechamentoFatura = new FechamentoFatura();
        KoBindings(_fechamentoFatura, idDiv);

        _detalheParcela = new DetalheParcela();
        KoBindings(_detalheParcela, "knoutDetalheParcela");

        _liquidarFatura = new LiquidarFatura();
        KoBindings(_liquidarFatura, "divModalSelecaoFormaPagamentoLiquidarFatura");

        _novoDescontoAcrescimo = new AdicionarDescontoAcrescimo();
        KoBindings(_novoDescontoAcrescimo, "knoutAdicionarAcrescimoDesconto");

        new BuscarJustificativas(_novoDescontoAcrescimo.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.Fatura, EnumTipoFinalidadeJustificativa.Todas]);

        new BuscarBanco(_fechamentoFatura.Banco);
        new BuscarClientes(_fechamentoFatura.TomadorFatura, retornoTomadorFatura);
        new BuscarEmpresa(_fechamentoFatura.EmpresaFatura, retornoEmpresaFatura);
        new BuscarTipoPagamentoRecebimento(_liquidarFatura.FormaPagamento);

        new BuscarConhecimentoNotaReferencia(_fechamentoFatura.CTe, RetornoConsultaCTeParaRemoverIntegracao);

        var detalhe = { descricao: "Detalhe", id: guid(), evento: "onclick", metodo: DetalheFaturaClick, tamanho: "10", icone: "" };
        var imprimirBoleto = { descricao: "Imprimir Boleto", id: guid(), evento: "onclick", metodo: ImprimirBoletoClick, tamanho: "10", icone: "" };
        var gerarBoleto = { descricao: "Gerar Boleto", id: guid(), evento: "onclick", metodo: GerarBoletoFaturaClick, tamanho: "10", icone: "" };

        new BuscarBoletoConfiguracao(_fechamentoFatura.GerarBoleto, RetornoGerarBoletoClick);

        var menuOpcoes = new Object();
        menuOpcoes.tipo = TypeOptionMenu.list;
        menuOpcoes.opcoes = new Array();
        menuOpcoes.opcoes.push(detalhe);
        menuOpcoes.opcoes.push(gerarBoleto);
        menuOpcoes.opcoes.push(imprimirBoleto);

        _gridParcelasFatura = new GridView(_fechamentoFatura.Parcelas.idGrid, "FaturaFechamento/PesquisaParcelaFatura", _fechamentoFatura, menuOpcoes, { column: 0, dir: orderDir.asc }, null, null);
        _gridParcelasFatura.CarregarGrid();

        CarregarGridCTesRemoverIntegracao();
        carregarGridAcrescimosDescontos();

        LoadAcrescimoDescontoLiquidacaoFatura();

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_DownloadArquivoIntegracoes, _PermissoesPersonalizadas) && _CONFIGURACAO_TMS.GerarPreviewDOCCOBFatura === true)
            _fechamentoFatura.DownloadPreviewDOCCOB.visible(true);

        _modalAdicionarAcrescimoDescontoFatura = new bootstrap.Modal(document.getElementById("divAdicionarAcrescimoDesconto"), { backdrop: 'static', keyboard: true });
        _modalSelecaoFormaPagamentoLiquidarFatura = new bootstrap.Modal(document.getElementById("divModalSelecaoFormaPagamentoLiquidarFatura"), { backdrop: 'static', keyboard: true });
        _modalDetalheParcela = new bootstrap.Modal(document.getElementById("divDetalheParcela"), { backdrop: 'static', keyboard: true });
    });

}

function RetornoConsultaCTeParaRemoverIntegracao(data) {
    _fechamentoFatura.CTe.val(data.Numero + "-" + data.Serie);
    _fechamentoFatura.CTe.codEntity(data.Codigo);
}

function DownloadPreviewDOCCOBClick() {
    executarDownload("FaturaFechamento/DownloadPreviewDOCCOB", { Codigo: _fatura.Codigo.val() });
}

function SalvarCTeRemoverIntegracaoClick(e, sender) {
    executarReST("FaturaIntegracao/AdicionarCTeRemoverIntegracao", { CTe: _fechamentoFatura.CTe.codEntity(), Fatura: _fatura.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _fechamentoFatura.CTe.val('');
                _fechamentoFatura.CTe.codEntity(0);

                _gridCTesRemoverIntegracao.CarregarGrid();

                exibirMensagem(tipoMensagem.ok, "Sucesso", "CT-e adicionado com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}

function ExcluirCTeRemoverIntegracaoClick(cteGrid) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o CT-e " + cteGrid.Numero + "?", function () {
        executarReST("FaturaIntegracao/ExcluirCTeRemoverIntegracao", { Codigo: cteGrid.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridCTesRemoverIntegracao.CarregarGrid();

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "CT-e removido com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
            }
        });
    });
}

function AdicionarDescontoAcrescimoClick(e, sender) {
    if (_fatura.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor seleciona uma fatura.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    if (ValidarCamposObrigatorios(e)) {
        var data = {
            Codigo: _fatura.Codigo.val(),
            Valor: e.Valor.val(),
            Justificativa: e.Justificativa.codEntity(),
            ObservacaoFatura: _fechamentoFatura.ObservacaoFatura.val(),
            ImprimirObservacaoFatura: _fechamentoFatura.ImprimirObservacaoFatura.val(),
            Banco: _fechamentoFatura.Banco.codEntity(),
            Agencia: _fechamentoFatura.Agencia.val(),
            Digito: _fechamentoFatura.Digito.val(),
            NumeroConta: _fechamentoFatura.NumeroConta.val(),
            TipoConta: _fechamentoFatura.TipoConta.val()
        };
        executarReST("FaturaFechamento/InserirAcrescimoDesconto", data, function (arg) {
            if (arg.Success) {
                LimparCampos(e);

                _gridAcrescimosDescontos.CarregarGrid();
                CarregarFechamentoFatura();
                _modalAdicionarAcrescimoDescontoFatura.hide();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function RemoverAcrescimoDescontoClick(e) {

    if (_fatura.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor seleciona uma fatura.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Acréscimo/Desconto selecionado?", function () {
        var data = {
            Codigo: e.Codigo,
            CodigoFatura: _fatura.Codigo.val(),
            ObservacaoFatura: _fechamentoFatura.ObservacaoFatura.val(),
            ImprimirObservacaoFatura: _fechamentoFatura.ImprimirObservacaoFatura.val(),
            Banco: _fechamentoFatura.Banco.codEntity(),
            Agencia: _fechamentoFatura.Agencia.val(),
            Digito: _fechamentoFatura.Digito.val(),
            NumeroConta: _fechamentoFatura.NumeroConta.val(),
            TipoConta: _fechamentoFatura.TipoConta.val()
        };
        executarReST("FaturaFechamento/RemoverAcrescimoDesconto", data, function (arg) {
            if (arg.Success) {

                _gridAcrescimosDescontos.CarregarGrid();
                CarregarFechamentoFatura();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}

function InformarAcrescimoDescontoClick(e, sender) {
    if (_fatura.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor seleciona uma fatura.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    LimparCampos(_novoDescontoAcrescimo);

    _modalAdicionarAcrescimoDescontoFatura.show();
}

function retornoTomadorFatura(data) {
    if (data != null) {
        _fechamentoFatura.TomadorFatura.val(data.Nome + " ( " + data.CPF_CNPJ + " ) ");
        _fechamentoFatura.TomadorFatura.codEntity(data.Codigo);
        executarReST("FaturaFechamento/ObterTipoGeracaoTitulosTomador", { CPFCNPJTomador: data.CPF_CNPJ }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _fechamentoFatura.GerarTituloPorDocumentoFiscal.val(r.Data.GerarTituloPorDocumentoFiscal);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    } else {
        LimparCampoEntity(_fechamentoFatura.TomadorFatura);
        _fechamentoFatura.GerarTituloPorDocumentoFiscal.val(false);
    }
}

function retornoEmpresaFatura(data) {
    if (data != null) {
        _fechamentoFatura.EmpresaFatura.val(data.RazaoSocial + " ( " + data.DescricaoCidadeEstado + " ) ");
        _fechamentoFatura.EmpresaFatura.codEntity(data.Codigo);
    } else
        LimparCampoEntity(_fechamentoFatura.EmpresaFatura);
}

function SalvarValoresFaturaClick(e, sender) {
    if (_fatura.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor seleciona uma fatura.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    _fechamentoFatura.ObservacaoFatura.requiredClass("form-control ");
    _fechamentoFatura.TomadorFatura.requiredClass("form-control ");
    _fechamentoFatura.EmpresaFatura.requiredClass("form-control ");

    var valido = true;
    if (_fechamentoFatura.ObservacaoFatura.val() == "" && _fechamentoFatura.ImprimirObservacaoFatura.val() == true) {
        valido = false;
        _fechamentoFatura.ObservacaoFatura.requiredClass("form-control  is-invalid");
    }
    if (_fechamentoFatura.TomadorFatura.val() == "") {
        valido = false;
        _fechamentoFatura.TomadorFatura.requiredClass("form-control  is-invalid");
    }
    if (_fechamentoFatura.EmpresaFatura.val() == "") {
        valido = false;
        _fechamentoFatura.EmpresaFatura.requiredClass("form-control  is-invalid");
    }
    if (valido) {
        var msgAdicional = "";
        if (_fechamentoFatura.Parcelas.val() != null && _fechamentoFatura.Parcelas.val().length > 0)
            msgAdicional = " A(s) parcela(s) devera(ão) ser gerada(s) novamente.";
        exibirConfirmacao("Confirmação", "Realmente deseja alterar todos os valores informados desta fatura?" + msgAdicional, function () {
            var data = {
                Codigo: _fatura.Codigo.val(),
                ObservacaoFatura: _fechamentoFatura.ObservacaoFatura.val(),
                ImprimirObservacaoFatura: _fechamentoFatura.ImprimirObservacaoFatura.val(),
                TomadorFatura: _fechamentoFatura.TomadorFatura.codEntity(),
                EmpresaFatura: _fechamentoFatura.EmpresaFatura.codEntity(),
                Banco: _fechamentoFatura.Banco.codEntity(),
                Agencia: _fechamentoFatura.Agencia.val(),
                Digito: _fechamentoFatura.Digito.val(),
                NumeroConta: _fechamentoFatura.NumeroConta.val(),
                TipoConta: _fechamentoFatura.TipoConta.val()
            };
            executarReST("FaturaFechamento/SalvarValores", data, function (arg) {
                if (arg.Success) {
                    CarregarFechamentoFatura();
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    } else
        exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Por favor verifique os campos obrigatórios.");
}

function SalvarObservacaoFaturaClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente alterar a observação da fatura?", function () {
        var data = {
            Codigo: _fatura.Codigo.val(),
            ObservacaoFatura: _fechamentoFatura.ObservacaoFatura.val(),
            ImprimirObservacaoFatura: _fechamentoFatura.ImprimirObservacaoFatura.val()
        };
        executarReST("FaturaFechamento/SalvarObservacaoFatura", data, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Observação alterada com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function AtualizarParcelasClick(e, sender) {
    _gridParcelasFatura.CarregarGrid();
}

function GerarParcelasClick(e, sender) {
    if (_fatura.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor, selecione uma fatura.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    _fechamentoFatura.IntervaloDeDias.requiredClass("form-control ");
    _fechamentoFatura.DataPrimeiroVencimento.requiredClass("form-control ");

    var valido = true;
    if (_fechamentoFatura.DataPrimeiroVencimento.val() == "") {
        valido = false;
        _fechamentoFatura.DataPrimeiroVencimento.requiredClass("form-control  is-invalid");
    }

    if (valido) {
        exibirConfirmacao("Confirmação", "Realmente deseja gerar as parcelas?", function () {
            var data = {
                Codigo: _fatura.Codigo.val(),
                QuantidadeParcelas: _fechamentoFatura.QuantidadeParcelas.val(),
                IntervaloDeDias: _fechamentoFatura.IntervaloDeDias.val(),
                DataPrimeiroVencimento: _fechamentoFatura.DataPrimeiroVencimento.val(),
                TipoArredondamento: _fechamentoFatura.TipoArredondamento.val(),
                FormaTitulo: _fechamentoFatura.FormaTitulo.val(),
                ObservacaoFatura: _fechamentoFatura.ObservacaoFatura.val(),
                ImprimirObservacaoFatura: _fechamentoFatura.ImprimirObservacaoFatura.val(),
                Banco: _fechamentoFatura.Banco.codEntity(),
                Agencia: _fechamentoFatura.Agencia.val(),
                Digito: _fechamentoFatura.Digito.val(),
                NumeroConta: _fechamentoFatura.NumeroConta.val(),
                TipoConta: _fechamentoFatura.TipoConta.val()
            };
            executarReST("FaturaFechamento/GerarParcelas", data, function (arg) {
                if (arg.Success) {
                    CarregarFechamentoFatura();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            });
        });
    } else
        exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Por favor verifique os campos obrigatórios.");
}

function SalvarParcelaClick(e, sender) {
    if (_fatura == null || _fatura.Codigo == null || _fatura.Codigo.val() == null || _fatura.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma fatura antes.");
        return;
    }

    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    if (ValidarCamposObrigatorios(e)) {
        var data = {
            Codigo: _fatura.Codigo.val(),
            CodigoParcela: e.Codigo.val(),
            Sequencia: e.Sequencia.val(),
            Valor: e.Valor.val(),
            ValorDesconto: e.ValorDesconto.val(),
            DataEmissao: e.DataEmissao.val(),
            DataVencimento: e.DataVencimento.val(),
            FormaTitulo: e.FormaTitulo.val(),
            ObservacaoFatura: _fechamentoFatura.ObservacaoFatura.val(),
            ImprimirObservacaoFatura: _fechamentoFatura.ImprimirObservacaoFatura.val(),
            Banco: _fechamentoFatura.Banco.codEntity(),
            Agencia: _fechamentoFatura.Agencia.val(),
            Digito: _fechamentoFatura.Digito.val(),
            NumeroConta: _fechamentoFatura.NumeroConta.val(),
            TipoConta: _fechamentoFatura.TipoConta.val(),
            CodigoTitulo: _codigoTituloSelecionado
        };
        executarReST("FaturaFechamento/AtualizarParcela", data, function (arg) {
            if (arg.Success) {
                _codigoTituloSelecionado = 0;
                LimparCampos(e);
                _modalDetalheParcela.hide();
                CarregarFechamentoFatura();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function ReAbrirFaturaClick(e, sender) {
    AbrirTelaCancelamentoFatura();
}

function LiquidarFaturaClick(e, sender) {
    if (ValidarCamposObrigatorios(e)) {
        exibirConfirmacao("Confirmação", "Deseja realmente liquidar esta fatura?", function () {
            _liquidarFatura.AcrescimosDescontos.val(JSON.stringify(_liquidarFatura.ListaAcrescimosDescontos.val()));

            Salvar(_liquidarFatura, "FaturaFechamento/LiquidarFatura", function (r) {
                if (r.Success) {
                    if (r.Data) {
                        _modalSelecaoFormaPagamentoLiquidarFatura.hide();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Fatura liquidada com sucesso.");
                        _fatura.Situacao.val(EnumSituacoesFatura.Liquidado);
                        CarregarDadosCabecalho(r.Data);
                        PosicionarEtapa(r.Data);
                        VerificarBotoes();
                        LimparCampos(_liquidarFatura);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
                }
            });
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function FecharFaturaClick(e, sender) {

    _fechamentoFatura.TomadorFatura.requiredClass("form-control ");
    _fechamentoFatura.EmpresaFatura.requiredClass("form-control ");

    var valido = true;
    if (_fechamentoFatura.TomadorFatura.val() == "") {
        valido = false;
        _fechamentoFatura.TomadorFatura.requiredClass("form-control  is-invalid");
    }
    if (_fechamentoFatura.EmpresaFatura.val() == "") {
        valido = false;
        _fechamentoFatura.EmpresaFatura.requiredClass("form-control  is-invalid");
    }

    if (!valido) {
        exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Por favor verifique os campos obrigatórios.");
        return;
    }

    executarReST("FaturaFechamento/ValidarFechamentoFatura", { Codigo: _fatura.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (!r.Data.Valido) {
                    if (r.Data.PermiteFecharFatura) {
                        exibirConfirmacao("Confirmação", r.Data.Mensagem, function () {
                            FinalizarFechamentoFatura();
                        });
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", r.Data.Mensagem, 30000);
                    }
                } else {
                    exibirConfirmacao("Confirmação", "Deseja realmente fechar esta fatura?", function () {
                        FinalizarFechamentoFatura();
                    });
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });

}

function FinalizarFechamentoFatura() {
    var data = {
        Codigo: _fatura.Codigo.val(),
        TomadorFatura: _fechamentoFatura.TomadorFatura.codEntity(),
        EmpresaFatura: _fechamentoFatura.EmpresaFatura.codEntity(),
        Banco: _fechamentoFatura.Banco.codEntity(),
        Agencia: _fechamentoFatura.Agencia.val(),
        Digito: _fechamentoFatura.Digito.val(),
        NumeroConta: _fechamentoFatura.NumeroConta.val(),
        TipoConta: _fechamentoFatura.TipoConta.val(),
        ObservacaoFatura: _fechamentoFatura.ObservacaoFatura.val(),
        ImprimirObservacaoFatura: _fechamentoFatura.ImprimirObservacaoFatura.val()
    };

    executarReST("FaturaFechamento/FecharFatura", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Fatura fechada com sucesso.");
                if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
                    ImprimirFatura();
                _fatura.Situacao.val(arg.Data.Situacao);
                CarregarDadosCabecalho(arg.Data);
                PosicionarEtapa(arg.Data);
                VerificarBotoes();
            } else {
                if (arg.Msg.indexOf("Existem os seguintes canhotos pendentes:") > -1) {
                    $('#contentInformacaoImportacao').html("");
                    $('#contentInformacaoImportacao').html(arg.Msg);
                    _modalInformacaoImportacao = new bootstrap.Modal(document.getElementById("divModalInformacaoImportacao"), { backdrop: true, keyboard: true });
                    _modalInformacaoImportacao.show();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ImprimirFatura() {
    executarDownload("Fatura/DownloadPDFFatura", { Codigo: _fechamentoFatura.Codigo.val() }, null, function () {
        executarDownloadArquivo("Relatorios/FaturaRelatorio/GerarRelatorio", { Codigo: _fechamentoFatura.Codigo.val() });
    });
}

function VisualizarFaturaClick(e, sender) {
    if (_fatura == null || _fatura.Codigo == null || _fatura.Codigo.val() == null || _fatura.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma fatura antes de visualizar o relatório.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }
    if (!_fatura.NovoModelo.val()) {
        if (_fatura.Situacao.val() != EnumSituacoesFatura.Fechado && _fatura.Situacao.val() != EnumSituacoesFatura.ProblemaIntegracao) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura não se encontra fechada.");
            return;
        }
    }

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Fatura_PermiteVisualizarFatura, _PermissoesPersonalizadas)) {
        ImprimirFatura();
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Sem Permissão", "Seu usuário não possui permissão para visualizar a fatura.");
}

function RetornoGerarBoletoClick(data) {
    if (data != null) {
        if (data.Codigo > 0 && data.Codigo != "" && _codigoTituloSelecionado > 0 && _codigoTituloSelecionado != "") {
            var data =
            {
                Codigo: data.Codigo,
                CodigoTitulo: _codigoTituloSelecionado
            };
            executarReST("TituloFinanceiro/GerarBoleto", data, function (e) {
                if (!e.Success) {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", e.Msg);
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Boleto enviado ao integrador, aguarde a sua geração.");
                    _gridParcelasFatura.CarregarGrid();
                }
            });
        }
    }
}

function GerarBoletoFaturaClick(e, sender) {
    if (_fatura == null || _fatura.Codigo == null || _fatura.Codigo.val() == null || _fatura.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma fatura antes de visualizar o relatório.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }
    if (!_fatura.NovoModelo.val()) {
        if (_fatura.Situacao.val() != EnumSituacoesFatura.Fechado && _fatura.Situacao.val() != EnumSituacoesFatura.ProblemaIntegracao) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura não se encontra fechada.");
            return;
        }
    }
    _codigoTituloSelecionado = 0;
    if (e.Codigo > 0 && e.Codigo != "" && e.CodigoTitulo > 0 && e.CodigoTitulo != "") {
        _codigoTituloSelecionado = e.CodigoTitulo;
        $('#' + _fechamentoFatura.GerarBoleto.idBtnSearch).trigger('click');
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor finalize a fatura para geração dos títulos.");
    }
}

function ImprimirBoletoClick(e, sender) {
    if (_fatura == null || _fatura.Codigo == null || _fatura.Codigo.val() == null || _fatura.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma fatura antes de visualizar o relatório.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }
    if (!_fatura.NovoModelo.val()) {
        if (_fatura.Situacao.val() != EnumSituacoesFatura.Fechado && _fatura.Situacao.val() != EnumSituacoesFatura.ProblemaIntegracao) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura não se encontra fechada.");
            return;
        }
    }

    if (e.Codigo > 0 && e.Codigo != "" && e.CodigoTitulo > 0 && e.CodigoTitulo != "") {
        var dados = { Codigo: e.CodigoTitulo }
        executarDownload("TituloFinanceiro/DownloadBoleto", dados);
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor finalize a fatura para geração dos títulos.");
    }
}

function DetalheFaturaClick(e, sender) {
    if (_fatura == null || _fatura.Codigo == null || _fatura.Codigo.val() == null || _fatura.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma fatura antes.");
        return;
    }

    //if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
    //    if (_fatura.Situacao.val() != EnumSituacoesFatura.EmAndamento) {
    //        exibirMensagem(tipoMensagem.aviso, "Aviso", "A situação atual da fatura não permite a sua edição da parcela.");
    //        return;
    //    }
    //}//Comentado para editar a parcela no go-live

    LimparCampos(_detalheParcela);

    if (e.Codigo > 0 && e.Codigo != "") {
        _codigoTituloSelecionado = e.CodigoTitulo;
        var data =
        {
            Codigo: e.Codigo
        };
        executarReST("FaturaFechamento/CarregarDadosParcela", data, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataParcela = { Data: e.Data };
                    PreencherObjetoKnout(_detalheParcela, dataParcela);
                    _detalheParcela.DataVencimento.enable(true);
                    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Fatura_NaoPermitirEditarDataVencimentoParcela, _PermissoesPersonalizadas) && !_CONFIGURACAO_TMS.UsuarioAdministrador)
                        _detalheParcela.DataVencimento.enable(false);

                    _detalheParcela.FormaTitulo.enable(true);
                    _detalheParcela.SalvarParcela.enable(true);
                    _modalDetalheParcela.show();
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
            }
        });
    }
}

function CancelarLiquidarFaturaClick(e, sender) {
    _modalSelecaoFormaPagamentoLiquidarFatura.hide();
    LimparCampos(_liquidarFatura);
}

//*******MÉTODOS*******

function carregarGridAcrescimosDescontos() {
    var editar = { descricao: "Excluir", id: "clasEditar", evento: "onclick", metodo: RemoverAcrescimoDescontoClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAcrescimosDescontos = new GridView(_fechamentoFatura.AcrescimosDescontos.idGrid, "FaturaFechamento/PesquisarAcrescimoDesconto", _fechamentoFatura, menuOpcoes, null, null, null);
    _gridAcrescimosDescontos.CarregarGrid();
}

function CarregarGridCTesRemoverIntegracao() {
    var editar = { descricao: "Excluir", id: "clasEditar", evento: "onclick", metodo: ExcluirCTeRemoverIntegracaoClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCTesRemoverIntegracao = new GridView(_fechamentoFatura.CTesRemoverIntegracao.idGrid, "FaturaIntegracao/PesquisaCTeRemoverIntegracao", _fechamentoFatura, menuOpcoes, { column: 1, dir: orderDir.asc }, 5);
}

function AbrirTelaLiquidarFatura(e, sender) {
    LimparCampos(_liquidarFatura);

    _liquidarFatura.ListaAcrescimosDescontos.val([]);
    RecarregarGridAcrescimoDescontoLiquidacaoFatura();

    _liquidarFatura.Codigo.val(_fatura.Codigo.val());

    executarReST("Fatura/ObterFormaPagamentoPadraoFatura", { Codigo: _fatura.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _liquidarFatura.FormaPagamento.val(r.Data.Descricao);
                _liquidarFatura.FormaPagamento.codEntity(r.Data.Codigo);

                if (r.Data.PossuiMoedaEstrangeira === true) {
                    _liquidarFatura.DataBaseCRT.visible(true);
                    _liquidarFatura.DataBaseCRT.required = true;
                    _liquidarFatura.DataBase.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
                    _liquidarFatura.DataBaixa.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
                } else {
                    _liquidarFatura.DataBaseCRT.visible(false);
                    _liquidarFatura.DataBaseCRT.required = false;
                    _liquidarFatura.DataBase.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
                    _liquidarFatura.DataBaixa.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
                }

                _modalSelecaoFormaPagamentoLiquidarFatura.show();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function CarregarFechamentoFatura() {

    AlterarDisplayFechamentoFatura();

    if (_fatura.Codigo.val() > 0 && _fatura.Codigo.val() != "") {

        var data = { CodigoFatura: _fatura.Codigo.val() };

        executarReST("FaturaFechamento/CarregarDadosFechamento", data, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    PreencherObjetoKnout(_fechamentoFatura, e);

                    if (e.Data.TipoMoeda != null && e.Data.TipoMoeda != EnumMoedaCotacaoBancoCentral.Real) {
                        _fechamentoFatura.Moeda.visible(true);
                        _fechamentoFatura.ValorTotalMoeda.visible(true);
                        _fechamentoFatura.AcrescimosMoeda.visible(true);
                        _fechamentoFatura.DescontosMoeda.visible(true);
                    }

                    _gridParcelasFatura.CarregarGrid();
                    _gridCTesRemoverIntegracao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
            }
        });

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
            ObterDetalhesFaturaContabilizacao();
        }

    }
}

function AlterarDisplayFechamentoFatura() {
    if (_fatura.NovoModelo.val()) {
        _fechamentoFatura.AcrescimosDescontos.visible(false);
    } else {
        _fechamentoFatura.AcrescimosDescontos.visible(true);
    }
}

function carregarConteudosHTMLFechamentoCarga(callback) {
    $.get("Content/Static/Fatura/FechamentoFatura.html?dyn=" + guid(), function (data) {
        _HTMLFechamentoFatura = data;
        LoadCancelamentoFatura();
        LoadAcrescimoDescontoDocumentos();

        carregarHTMLFaturaContabilizacao(callback);

        _modalHistoricoIntegracaoFatura = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracaoFatura"), { backdrop: 'static', keyboard: true });
    });
}

function LimparFechamentoFatura() {
    LimparCampos(_fechamentoFatura);
    LimparCampos(_detalheParcela);
    LimparCampos(_liquidarFatura);
    _gridParcelasFatura.CarregarGrid();
}