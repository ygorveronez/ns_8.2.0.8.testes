/// <reference path="IntegracaoBaixaTituloReceber.js" />
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
/// <reference path="CabecalhoBaixaTituloReceber.js" />
/// <reference path="BaixaTituloReceber.js" />
/// <reference path="../../Consultas/Conhecimento.js" />
/// <reference path="EtapaBaixaTituloReceber.js" />
/// <reference path="../../Consultas/TipoPagamento.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />

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

var CTeRemovidoMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
};

var _negociacaoBaixa;
var _gridParcelasNegociacao;
var _gridConhecimentosRemovidos;
var _HTMLNegociacaoBaixa;
var _detalheParcela;
var _detalheConhecimento;
var _novoDescontoAcrescimo;
var _gridAcrescimosDescontos;
var _codigoTituloSelecionado;

var NegociacaoBaixa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroFatura = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.NumeroTitulo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorOriginal = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorABaixar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DataBaixar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DataBase = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoDevedor = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: false });
    this.Operador = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Descontos = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Acrescimos = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorPendente = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorTotalCTesRemovidos = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.ValorTotalCTesNaoRemovidos = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.SaldoContaAdiantamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });

    this.TipoDePagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Tipo de Pagamento:"), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.AdicionarAcrescimoDesconto = PropertyEntity({ eventClick: InformarAcrescimoDescontoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.AcrescimosDescontos = PropertyEntity({ type: types.map, required: false, text: "Acréscimos / Descontos", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.QuantidadeParcelas = PropertyEntity({ val: ko.observable("1"), options: _qtdParcelas, text: "Qtd. Parcelas: ", def: "1", enable: ko.observable(true) });
    this.IntervaloDeDias = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Intervalo de Dias:", maxlength: 10, enable: ko.observable(true) });
    this.DataPrimeiroVencimento = PropertyEntity({ text: "Data Vencimento: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissao = PropertyEntity({ text: "Data Última Emissão: ", getType: typesKnockout.date, enable: ko.observable(true), required: false, visible: false });
    this.TipoArredondamento = PropertyEntity({ val: ko.observable("1"), options: EnumTipoArredondamento.ObterOpcoes(), text: "Arredondar Valor: ", def: "1", enable: ko.observable(true) });
    this.GerarParcelas = PropertyEntity({ eventClick: GerarParcelasClick, type: types.event, text: "Gerar Parcelas", visible: ko.observable(true), enable: ko.observable(true) });
    this.AtualizarParcelas = PropertyEntity({ eventClick: AtualizarParcelasClick, type: types.event, text: "Atualizar Parcelas", visible: ko.observable(true), enable: ko.observable(true) });
    this.GerarBoleto = PropertyEntity({ type: types.event, text: "Gerar Boleto", visible: ko.observable(false), enable: ko.observable(false), idBtnSearch: guid() });
    this.Parcelas = PropertyEntity({ type: types.map, required: false, text: "Parcelas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });

    this.Descricao = PropertyEntity({ getType: typesKnockout.text, text: "Número do documento: ", val: ko.observable(""), enable: ko.observable(true) });
    this.PesquisarDocumento = PropertyEntity({
        eventClick: function (e) {
            _gridConhecimentosRemovidos.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
    this.SalvarConhecimentosRemovidos = PropertyEntity({ eventClick: SalvarConhecimentosRemovidosClick, type: types.event, text: ko.observable("Salvar CT-e Removidos"), visible: ko.observable(true), enable: ko.observable(true) });

    this.CodigoFatura = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CTesParaRemover = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.FecharBaixa = PropertyEntity({ eventClick: SalvarFecharBaixaClick, type: types.event, text: "Fechar Baixa", visible: ko.observable(true), enable: ko.observable(true) });
    this.ImprimirRecibo = PropertyEntity({ eventClick: ImprimirReciboClick, type: types.event, text: "Imprimir Recibo", visible: ko.observable(false), enable: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
    this.ChequeBaixa = PropertyEntity({ type: types.event, text: "Adicionar Cheques", idBtnSearch: guid(), visible: ko.observable(false) });
    this.AdicionarCheque = PropertyEntity({ eventClick: AdicionarChequeClick, type: types.event, text: "Novo Cheque", visible: ko.observable(true), enable: ko.observable(true) });
};

var AdicionarDescontoAcrescimo = function () {
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 15 });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarDescontoAcrescimoClick, text: "Adicionar", visible: ko.observable(true) });
};

var DetalheParcela = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Sequencia = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Sequência:", maxlength: 10, enable: ko.observable(false) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10, enable: ko.observable(true) });
    this.ValorDesconto = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor Desconto:", maxlength: 10, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, required: false, text: "Data Emissão:", enable: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data Vencimento:", enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: false, enable: ko.observable(true) });

    this.SalvarParcela = PropertyEntity({ type: types.event, eventClick: SalvarParcelaClick, text: "Salvar Parcela", visible: ko.observable(true), enable: ko.observable(true) });
};

var DetalheConhecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Número:", maxlength: 10, enable: ko.observable(false) });
    this.Serie = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Série:", maxlength: 10, enable: ko.observable(false) });
    this.Tomador = PropertyEntity({ getType: typesKnockout.string, required: false, text: "Tomador:", maxlength: 1000, enable: ko.observable(false) });
    this.ValorAReceber = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor a receber:", maxlength: 10, enable: ko.observable(false), def: ko.observable("0,00") });

    this.ValorPago = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor Pago:", maxlength: 10, enable: ko.observable(true), def: ko.observable("0,00") });
    this.ValorDesconto = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Desconto:", maxlength: 10, enable: ko.observable(true), def: ko.observable("0,00") });
    this.ValorAcrescimo = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Acréscimo:", maxlength: 10, enable: ko.observable(true), def: ko.observable("0,00") });
    this.JustificativaDesconto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa do Desconto:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.JustificativaAcrescimo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa do Acréscimo:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 300, enable: ko.observable(true) });

    this.SalvarDetalhe = PropertyEntity({ type: types.event, eventClick: SalvarDetalheConhecimentoClick, text: "Salvar Detalhe", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadNegociacaoBaixa() {
    carregarConteudosHTMLNegociacaoBaixa(function () {
        $("#contentNegociacaoTituloReceber").html("");
        var idDiv = guid();
        $("#contentNegociacaoTituloReceber").append(_HTMLNegociacaoBaixa.replace(/#divNegociacaoBaixaTituloReceber/g, idDiv));
        _negociacaoBaixa = new NegociacaoBaixa();
        KoBindings(_negociacaoBaixa, idDiv);

        new BuscarTipoPagamento(_negociacaoBaixa.TipoDePagamento);

        LoadChequeBaixa();

        _novoDescontoAcrescimo = new AdicionarDescontoAcrescimo();
        KoBindings(_novoDescontoAcrescimo, "knoutAdicionarAcrescimoDesconto");

        new BuscarJustificativas(_novoDescontoAcrescimo.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.TitulosReceber, EnumTipoFinalidadeJustificativa.Todas]);

        _detalheParcela = new DetalheParcela();
        KoBindings(_detalheParcela, "knoutDetalheParcela");

        _detalheConhecimento = new DetalheConhecimento();
        KoBindings(_detalheConhecimento, "knoutDetalheConhecimento");

        new BuscarJustificativas(_detalheConhecimento.JustificativaDesconto, null, EnumTipoJustificativa.Desconto, [EnumTipoFinalidadeJustificativa.TitulosReceber, EnumTipoFinalidadeJustificativa.Todas]);
        new BuscarJustificativas(_detalheConhecimento.JustificativaAcrescimo, null, EnumTipoJustificativa.Acrescimo, [EnumTipoFinalidadeJustificativa.TitulosReceber, EnumTipoFinalidadeJustificativa.Todas]);

        var editar = { descricao: "Editar", id: guid(), metodo: DetalheParcelaNegociacaoClick, icone: "" };
        var imprimirBoleto = { descricao: "Imprimir Boleto", id: "clasImprimirBoleto", evento: "onclick", metodo: ImprimirBoletoClick, tamanho: "10", icone: "" };
        var gerarBoleto = { descricao: "Gerar Boleto", id: "clasGerarBoleto", evento: "onclick", metodo: GerarBoletoFaturaClick, tamanho: "10", icone: "" };
        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [editar, imprimirBoleto, gerarBoleto] };

        new BuscarBoletoConfiguracao(_negociacaoBaixa.GerarBoleto, RetornoGerarBoletoClick);

        _negociacaoBaixa.CodigoFatura.val(_baixaTituloReceber.CodigoFatura.val());
        _gridParcelasNegociacao = new GridView(_negociacaoBaixa.Parcelas.idGrid, "BaixaTituloReceber/PesquisaParcelasNegociacao", _negociacaoBaixa, menuOpcoes, { column: 2, dir: orderDir.asc }, null, null);
        _gridParcelasNegociacao.CarregarGrid();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
            _negociacaoBaixa.ValorTotalCTesRemovidos.visible(false);
            _negociacaoBaixa.ValorTotalCTesNaoRemovidos.visible(false);
            //_negociacaoBaixa.TipoDePagamento.text("Tipo de Pagamento:");
            //_negociacaoBaixa.TipoDePagamento.required(false);
        }

        carregarGridAcrescimosDescontos();

        //new BuscarBoletoConfiguracao(_tituloFinanceiro.GerarBoleto, GerarBoletoClick);
    });
}

function AdicionarChequeClick(e, sender) {
    limparCamposCheque();

    Global.abrirModal('divModalCheque');
}

function GerarBoletoFaturaClick(e, sender) {
    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() !== EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa ainda não foi finalizada.");
        return;
    }

    _codigoTituloSelecionado = 0;
    if (e.Codigo > 0 && e.Codigo != "" && e.CodigoTitulo > 0 && e.CodigoTitulo != "") {
        _codigoTituloSelecionado = e.CodigoTitulo;
        $('#' + _negociacaoBaixa.GerarBoleto.idBtnSearch).trigger('click');
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor finalize a fatura para geração dos títulos.");
    }
}

function ImprimirBoletoClick(e, sender) {
    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() !== EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa ainda não foi finalizada.");
        return;
    }

    if (e.Codigo > 0 && e.Codigo != "" && e.CodigoTitulo > 0 && e.CodigoTitulo != "") {
        var dados = { Codigo: e.CodigoTitulo }
        executarDownload("TituloFinanceiro/DownloadBoleto", dados);
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor finalize a fatura para geração dos títulos.");
    }
}

function AtualizarParcelasClick(e, sender) {
    _gridParcelasNegociacao.CarregarGrid();
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
                    _gridParcelasNegociacao.CarregarGrid();
                }
            });
        }
    }
}

function AdicionarDescontoAcrescimoClick(e, sender) {
    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    if (ValidarCamposObrigatorios(e)) {
        var data = {
            Codigo: _baixaTituloReceber.Codigo.val(),
            Valor: e.Valor.val(),
            Justificativa: e.Justificativa.codEntity(),
            TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity(),
            MoedaCotacaoBancoCentral: _baixaTituloReceber.MoedaCotacaoBancoCentral.val(),
            DataBaseCRT: _baixaTituloReceber.DataBaseCRT.val(),
            ValorMoedaCotacao: _baixaTituloReceber.ValorMoedaCotacao.val()
        };
        executarReST("BaixaTituloReceber/InserirAcrescimoDesconto", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    LimparCampos(e);

                    _gridAcrescimosDescontos.CarregarGrid();
                    CarregarNegociacaoBaixa();
                    Global.fecharModal('divAdicionarAcrescimoDesconto');
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

function InformarAcrescimoDescontoClick(e, sender) {
    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }
    LimparCampos(_novoDescontoAcrescimo);

    Global.abrirModal('divAdicionarAcrescimoDesconto');
}

function buscarCTesParaRemover(ctesFatura) {
    if (_baixaTituloReceber.CodigoFatura.val() > 0) {
        var naoselecionados = new Array();
        var somenteLeitura = false;
        _negociacaoBaixa.SelecionarTodos.visible(false);
        _negociacaoBaixa.SelecionarTodos.val(true);
        if (ctesFatura != null) {
            somenteLeitura = false;
            _negociacaoBaixa.SelecionarTodos.visible(false);
            $.each(ctesFatura, function (i, obj) {
                var objCTe = { DT_RowId: obj.CodigoCTe, Codigo: obj.CodigoCTe };
                naoselecionados.push(objCTe);
            });
        }
        var multiplaescolha = {
            basicGrid: null,
            eventos: function () { },
            selecionados: new Array(),
            naoSelecionados: naoselecionados,
            SelecionarTodosKnout: _negociacaoBaixa.SelecionarTodos,
            somenteLeitura: somenteLeitura
        };

        var baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: baixarDacteClick, icone: "" };
        var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "" };
        var detalheConhecimento = { descricao: "Detalhe CTe", id: guid(), metodo: DetalheConhecimentoClick, icone: "" };
        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarXML, detalheConhecimento] };

        _negociacaoBaixa.CodigoFatura.val(_baixaTituloReceber.CodigoFatura.val());
        _gridConhecimentosRemovidos = new GridView(_negociacaoBaixa.CTesParaRemover.idGrid, "FaturaCarga/PesquisaConhecimentosFatura", _negociacaoBaixa, menuOpcoes, null, null, null, null, null, multiplaescolha);
        _gridConhecimentosRemovidos.CarregarGrid();
    }
}

function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.Codigo, CodigoEmpresa: 0 };
    executarDownload("CargaCTe/DownloadXML", data);
}

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.Codigo, CodigoEmpresa: 0 };
    executarDownload("CargaCTe/DownloadDacte", data);
}


function SalvarConhecimentosRemovidosClick(e, sender) {
    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    _baixaTituloReceber.CTesRemovidos.list = new Array();

    if (_gridConhecimentosRemovidos != undefined) {
        var ctesSelecionados;
        ctesSelecionados = _gridConhecimentosRemovidos.ObterMultiplosNaoSelecionados();

        if (ctesSelecionados.length > 0) {
            $.each(ctesSelecionados, function (i, cte) {
                var map = new CTeRemovidoMap();
                map.Codigo.val = cte.Codigo;
                _baixaTituloReceber.CTesRemovidos.list.push(map);
            });
        }
    }

    var data = {
        Codigo: _baixaTituloReceber.Codigo.val(),
        CTesRemovidos: JSON.stringify(_baixaTituloReceber.CTesRemovidos.list),
        TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity()
    };
    exibirConfirmacao("Confirmação", "Realmente deseja salvar os conhecimentos removidos?", function () {
        executarReST("BaixaTituloReceber/SalvarConhecimentosRemovidos", data, function (arg) {
            if (arg.Success) {
                CarregarNegociacaoBaixa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}

function SalvarValoresBaixaClick(e, sender) {
    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    var valido = true;

    _baixaTituloReceber.CTesRemovidos.list = new Array();

    var ctesSelecionados;
    ctesSelecionados = _gridConhecimentosRemovidos.ObterMultiplosNaoSelecionados();

    if (ctesSelecionados.length > 0) {
        $.each(ctesSelecionados, function (i, cte) {
            var map = new CTeRemovidoMap();
            map.Codigo.val = cte.Codigo;
            _baixaTituloReceber.CTesRemovidos.list.push(map);
        });
    }

    var data = {
        Codigo: _baixaTituloReceber.Codigo.val(),
        CTesRemovidos: JSON.stringify(_baixaTituloReceber.CTesRemovidos.list),
        TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity()
    };
    if (valido) {
        var msgAdicional = "";
        if (_negociacaoBaixa.Parcelas.val() != null && _negociacaoBaixa.Parcelas.val().length > 0)
            msgAdicional = " A(s) parcela(s) devera(ão) ser gerada(s) novamente."
        exibirConfirmacao("Confirmação", "Realmente deseja lançar os valores informados nesta baixa?" + msgAdicional, function () {
            executarReST("BaixaTituloReceber/SalvarValores", data, function (arg) {
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

function GerarParcelasClick(e, sender) {
    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    _negociacaoBaixa.IntervaloDeDias.requiredClass("form-control");
    _negociacaoBaixa.DataPrimeiroVencimento.requiredClass("form-control");
    _negociacaoBaixa.DataEmissao.requiredClass("form-control");

    var valido = true;
    if (_negociacaoBaixa.DataPrimeiroVencimento.val() == "") {
        valido = false;
        _negociacaoBaixa.DataPrimeiroVencimento.requiredClass("form-control is-invalid");
    }

    //if (_negociacaoBaixa.DataEmissao.val() == "") {
    //    valido = false;
    //    _negociacaoBaixa.DataEmissao.requiredClass("form-control is-invalid");
    //}

    if (valido) {
        exibirConfirmacao("Confirmação", "Realmente deseja gerar as parcelas?", function () {
            _baixaTituloReceber.CTesRemovidos.list = new Array();

            if (_gridConhecimentosRemovidos != undefined) {
                var ctesSelecionados;
                ctesSelecionados = _gridConhecimentosRemovidos.ObterMultiplosNaoSelecionados();

                if (ctesSelecionados.length > 0) {
                    $.each(ctesSelecionados, function (i, cte) {
                        var map = new CTeRemovidoMap();
                        map.Codigo.val = cte.Codigo;
                        _baixaTituloReceber.CTesRemovidos.list.push(map);
                    });
                }
            }

            var data = {
                Codigo: _baixaTituloReceber.Codigo.val(),
                QuantidadeParcelas: _negociacaoBaixa.QuantidadeParcelas.val(),
                IntervaloDeDias: _negociacaoBaixa.IntervaloDeDias.val(),
                DataPrimeiroVencimento: _negociacaoBaixa.DataPrimeiroVencimento.val(),
                DataEmissao: _negociacaoBaixa.DataEmissao.val(),
                TipoArredondamento: _negociacaoBaixa.TipoArredondamento.val(),
                CTesRemovidos: JSON.stringify(_baixaTituloReceber.CTesRemovidos.list),
                TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity(),
                FormaTitulo: EnumFormaTitulo.Outros
            };
            executarReST("BaixaTituloReceber/GerarParcelas", data, function (arg) {
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
    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    if (ValidarCamposObrigatorios(e)) {
        var data = {
            Codigo: _baixaTituloReceber.Codigo.val(),
            CodigoParcela: e.Codigo.val(),
            Sequencia: e.Sequencia.val(),
            Valor: e.Valor.val(),
            ValorDesconto: e.ValorDesconto.val(),
            DataEmissao: e.DataEmissao.val(),
            DataVencimento: e.DataVencimento.val(),
            TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity(),
            FormaTitulo: e.FormaTitulo.val()
        };
        executarReST("BaixaTituloReceber/AtualizarParcela", data, function (arg) {
            if (arg.Success) {
                LimparCampos(e);
                Global.fecharModal('divDetalheParcela');
                CarregarNegociacaoBaixa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function SalvarDetalheConhecimentoClick(e, sender) {
    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    _detalheConhecimento.JustificativaDesconto.requiredClass("form-control");
    _detalheConhecimento.JustificativaAcrescimo.requiredClass("form-control");

    var valido = true;
    if (_detalheConhecimento.ValorDesconto.val() != "" && _detalheConhecimento.ValorDesconto.val() != "0,00" && (_detalheConhecimento.JustificativaDesconto.codEntity() <= 0 || _detalheConhecimento.JustificativaDesconto.codEntity() == undefined)) {
        valido = false;
        _detalheConhecimento.JustificativaDesconto.requiredClass("form-control is-invalid");
    }
    if (_detalheConhecimento.ValorAcrescimo.val() != "" && _detalheConhecimento.ValorAcrescimo.val() != "0,00" && (_detalheConhecimento.JustificativaAcrescimo.codEntity() <= 0 || _detalheConhecimento.JustificativaAcrescimo.codEntity() == undefined)) {
        valido = false;
        _detalheConhecimento.JustificativaAcrescimo.requiredClass("form-control is-invalid");
    }


    if (valido) {
        if (ValidarCamposObrigatorios(e)) {
            var data = {
                Codigo: e.Codigo.val(),
                CodigoCTe: e.CodigoCTe.val(),
                CodigoBaixaTitulo: _baixaTituloReceber.Codigo.val(),
                ValorPago: _detalheConhecimento.ValorPago.val(),
                ValorDesconto: _detalheConhecimento.ValorDesconto.val(),
                ValorAcrescimo: _detalheConhecimento.ValorAcrescimo.val(),
                JustificativaDesconto: _detalheConhecimento.JustificativaDesconto.codEntity(),
                JustificativaAcrescimo: _detalheConhecimento.JustificativaAcrescimo.codEntity(),
                Observacao: _detalheConhecimento.Observacao.val(),
                TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity()
            };
            executarReST("BaixaTituloReceber/AtualizarDetalheConhecimento", data, function (arg) {
                if (arg.Success) {
                    LimparCampos(e);
                    Global.fecharModal('divDetalheConhecimento');
                    CarregarNegociacaoBaixa();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            });

        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
        }
    } else
        exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Por favor verifique os campos obrigatórios.");
}

function ImprimirReciboClick(e, sender) {
    var data = { Codigo: _baixaTituloReceber.Codigo.val(), TituloReceber: true };
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
    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    if (ValidarCamposObrigatorios(_negociacaoBaixa)) {

        exibirConfirmacao("Confirmação", "Realmente deseja fechar a baixa selecionada?", function () {

            _baixaTituloReceber.CTesRemovidos.list = new Array();

            if (_gridConhecimentosRemovidos != undefined) {
                var ctesSelecionados;
                ctesSelecionados = _gridConhecimentosRemovidos.ObterMultiplosNaoSelecionados();

                if (ctesSelecionados.length > 0) {
                    $.each(ctesSelecionados, function (i, cte) {
                        var map = new CTeRemovidoMap();
                        map.Codigo.val = cte.Codigo;
                        _baixaTituloReceber.CTesRemovidos.list.push(map);
                    });
                }
            }

            var data = {
                Codigo: _baixaTituloReceber.Codigo.val(),
                Etapa: EnumEtapasBaixaTituloReceber.Finalizada,
                CTesRemovidos: JSON.stringify(_baixaTituloReceber.CTesRemovidos.list),
                TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity()
            };
            executarReST("BaixaTituloReceber/FecharBaixa", data, function (arg) {
                if (arg.Success) {
                    _baixaTituloReceber.Etapa.val(EnumEtapasBaixaTituloReceber.Finalizada);
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

function SalvarFecharBaixaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_negociacaoBaixa);

    if (valido) {
        var data = {
            BaixaTitulo: _baixaTituloReceber.Codigo.val()
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
    if (_baixaTituloReceber == null || _baixaTituloReceber.Codigo == null || _baixaTituloReceber.Codigo.val() == null || _baixaTituloReceber.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de título antes.");
        return;
    }

    LimparCampos(_detalheParcela);

    if (e.Codigo > 0 && e.Codigo != "") {
        var data =
        {
            Codigo: e.Codigo
        }
        executarReST("BaixaTituloReceber/CarregarDadosParcela", data, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataParcela = { Data: e.Data };
                    PreencherObjetoKnout(_detalheParcela, dataParcela);                    
                    Global.abrirModal('divDetalheParcela');
                }
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", e.Msg);
            }
        });
    }
}

function DetalheConhecimentoClick(e, sender) {
    if (_baixaTituloReceber == null || _baixaTituloReceber.Codigo == null || _baixaTituloReceber.Codigo.val() == null || _baixaTituloReceber.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de informar os detalhes do conhecimento.");
        return;
    }

    LimparCampos(_detalheConhecimento);

    if (e.Codigo > 0 && e.Codigo != "") {
        var data =
        {
            Codigo: e.Codigo,
            CodigoBaixaTitulo: _baixaTituloReceber.Codigo.val(),
        }
        executarReST("BaixaTituloReceber/CarregarDadosConhecimento", data, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataConhecimento = { Data: e.Data };
                    PreencherObjetoKnout(_detalheConhecimento, dataConhecimento);                    
                    Global.abrirModal('divDetalheConhecimento');
                }
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", e.Msg);
            }
        });
    }
}

function RemoverAcrescimoDescontoClick(e) {

    if (_baixaTituloReceber.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa de títulos.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa se encontra cancelada.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Acréscimo/Desconto selecionado?", function () {
        var data = {
            Codigo: e.Codigo,
            TipoDePagamento: _negociacaoBaixa.TipoDePagamento.codEntity(),
            CodigoBaixa: _baixaTituloReceber.Codigo.val()
        };
        executarReST("BaixaTituloReceber/RemoverAcrescimoDesconto", data, function (arg) {
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

    _gridAcrescimosDescontos = new GridView(_negociacaoBaixa.AcrescimosDescontos.idGrid, "BaixaTituloReceber/PesquisarAcrescimoDesconto", _negociacaoBaixa, menuOpcoes, null, null, null);
    _gridAcrescimosDescontos.CarregarGrid();
}

function LimparCamposNegociacao() {
    LimparCampos(_negociacaoBaixa);
    _gridParcelasNegociacao.CarregarGrid();
    _gridAcrescimosDescontos.CarregarGrid();
    LimparCamposChequeBaixa();
}

function CarregarNegociacaoBaixa() {
    LimparCampos(_negociacaoBaixa);
    carregarListaTitulos();
    if (_baixaTituloReceber.Codigo.val() > 0 && _baixaTituloReceber.Codigo.val() != "") {
        var data =
        {
            Codigo: _baixaTituloReceber.Codigo.val(),
            ListaTitulos: _baixaTituloReceber.ListaTitulos.val()
        }
        executarReST("BaixaTituloReceber/CarregarNegociacaoBaixa", data, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataNegociacao = { Data: e.Data };
                    PreencherObjetoKnout(_negociacaoBaixa, dataNegociacao);

                    _baixaTituloReceber.ValorBaixado.val(dataNegociacao.Data.ValorABaixar);

                    _gridParcelasNegociacao.CarregarGrid();
                    _gridAcrescimosDescontos.CarregarGrid();
                    carregarDadosChequeBaixa();

                    if (_baixaTituloReceber.Situacao.val() == 1 || _baixaTituloReceber.Situacao.val() == 2)
                        _negociacaoBaixa.SaldoContaAdiantamento.visible(true);
                    else
                        _negociacaoBaixa.SaldoContaAdiantamento.visible(false);

                    if (dataNegociacao.Data.CodigoPessoa > 0)
                        $('#divParcelasNegociacal').show();
                    else
                        $('#divParcelasNegociacal').hide();

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
                        _negociacaoBaixa.TipoDePagamento.text("*Tipo de Pagamento:");
                        _negociacaoBaixa.TipoDePagamento.visible(true);
                        _negociacaoBaixa.TipoDePagamento.required(true);
                    }
                    else if (parseFloat(_baixaTituloReceber.ValorBaixado.val().toString().replace(".", "").replace(",", ".")) > 0) {
                        _negociacaoBaixa.TipoDePagamento.text("*Tipo de Pagamento:");
                        _negociacaoBaixa.TipoDePagamento.required(true);
                    } else {
                        _negociacaoBaixa.TipoDePagamento.text("Tipo de Pagamento:");
                        _negociacaoBaixa.TipoDePagamento.required(false);
                    }

                    _baixaTituloReceber.CodigoFatura.val(dataNegociacao.Data.CodigoFatura);
                    VerificarBotoes();
                    if (_baixaTituloReceber.CodigoFatura.val() > 0) {
                        var data = { Codigo: _baixaTituloReceber.Codigo.val() };
                        executarReST("BaixaTituloReceber/PesquisaConhecimentosRemovidos", data, function (arg) {
                            if (arg.Success) {
                                buscarCTesParaRemover(arg.Data);
                            } else {
                                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                            }
                        });
                    }
                }
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", e.Msg);
            }
        });
    } else if (_gridParcelasNegociacao != null && _gridConhecimentosRemovidos != null) {
        _gridParcelasNegociacao.CarregarGrid();
    }
}

function carregarConteudosHTMLNegociacaoBaixa(callback) {
    $.get("Content/Static/Financeiro/NegociacaoBaixaTituloReceber.html?dyn=" + guid(), function (data) {
        _HTMLNegociacaoBaixa = data;
        $.get("Content/Static/Financeiro/BaixaTituloReceberModais.html?dyn=" + guid(), function (data) {
            $("#ModaisTituloReceber").html(data);
            callback();
        });
    });
}

//function GerarBoletoClick(data) {
//    if (data != null) {
//        if (data.Codigo > 0 && data.Codigo != "") {
//            var data =
//                {
//                    Codigo: data.Codigo,
//                    CodigoTitulo: _tituloFinanceiro.Codigo.val()
//                }
//            executarReST("TituloFinanceiro/GerarBoleto", data, function (e) {
//                if (!e.Success) {
//                    exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
//                } else {
//                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Boleto enviado ao integrador.");
//                }
//            });
//        }
//    }
//}

//function VisualizarBoletoClick(e) {
//    var dados = { Codigo: _tituloFinanceiro.Codigo.val() }
//    executarDownload("TituloFinanceiro/DownloadBoleto", dados);
//}