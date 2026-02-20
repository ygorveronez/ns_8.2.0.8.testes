/// <reference path="IntegracaoBaixaTituloReceber.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
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

var _negociacaoBaixa, _gridParcelasNegociacao, _detalheParcela, _crudNegociacaoBaixa, _fechamentoBaixa, _progressNegociacaoBaixaReceber;

var ProgressNegociacaoBaixaReceber = function () {
    this.PercentualProcessadoFinalizacao = PropertyEntity({ val: ko.observable("0%"), def: "0%", text: ko.observable("Finalizando esta Baixa"), visible: ko.observable(false) });
};

var NegociacaoBaixa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPessoa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoFatura = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroFatura = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DataBaixa = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DataBase = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Operador = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TipoPagamentoRecebimento = PropertyEntity({ getType: typesKnockout.string, text: "Tipo de pagamento:", val: ko.observable(""), visible: ko.observable(false) });
    this.PlanoConta = PropertyEntity({ getType: typesKnockout.string, text: "Conta:", val: ko.observable(""), visible: ko.observable(false) });

    this.Valor = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorPago = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorAcrescimo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorDesconto = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorTotalAPagar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoPendente = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoContaAdiantamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });

    this.Moeda = PropertyEntity({ getType: typesKnockout.string, text: ko.observable("Real"), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, visible: ko.observable(false) });
    this.ValorCotacaoMoeda = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ValorMoeda = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ValorPagoMoeda = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ValorAcrescimoMoeda = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ValorDescontoMoeda = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ValorTotalAPagarMoeda = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.SaldoPendenteMoeda = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
};

var ParcelaNegociacaoBaixa = function () {
    this.QuantidadeParcelas = PropertyEntity({ val: ko.observable("1"), options: _qtdParcelas, text: "Qtd. Parcelas: ", def: "1", enable: ko.observable(true) });
    this.IntervaloDeDias = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Intervalo de Dias:", maxlength: 10, enable: ko.observable(true) });
    this.DataPrimeiroVencimento = PropertyEntity({ text: "Data Vencimento: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissao = PropertyEntity({ text: "Data Última Emissão: ", getType: typesKnockout.date, enable: ko.observable(true), required: false, visible: false });
    this.TipoArredondamento = PropertyEntity({ val: ko.observable("1"), options: EnumTipoArredondamento.ObterOpcoes(), text: "Arredondar Valor: ", def: "1", enable: ko.observable(true) });

    this.Parcelas = PropertyEntity({ type: types.map, required: false, text: "Parcelas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });

    this.GerarParcelas = PropertyEntity({ eventClick: GerarParcelasClick, type: types.event, text: "Gerar Parcelas", visible: ko.observable(true), enable: ko.observable(true) });
};

var DetalheParcela = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Sequencia = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Sequencia:", maxlength: 10, enable: ko.observable(false) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "Valor:", maxlength: 10, enable: ko.observable(true) });
    this.ValorDesconto = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor Desconto:", maxlength: 10, enable: ko.observable(false), visible: ko.observable(false) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, required: false, text: "Data Emissão:", enable: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data Vencimento:", enable: ko.observable(true) });

    this.SalvarParcela = PropertyEntity({ type: types.event, eventClick: SalvarParcelaClick, text: "Salvar Parcela", visible: ko.observable(true), enable: ko.observable(true) });
};

var CRUDNegociacaoBaixa = function () {
    this.FecharBaixa = PropertyEntity({ eventClick: FecharBaixaClick, type: types.event, text: "Fechar Baixa", visible: ko.observable(true), enable: ko.observable(true) });
};

var FechamentoBaixa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Tipo de Pagamento:"), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.FinalizarBaixa = PropertyEntity({ eventClick: FinalizarBaixaClick, type: types.event, text: "Finalizar a Baixa", visible: ko.observable(true), enable: ko.observable(true), icon: "fa fa-chevron-down" });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaFinalizacaoBaixaTituloReceber, type: types.event, text: "Fechar", visible: ko.observable(true), enable: ko.observable(true), icon: "fa fa-window-close" });
};

//*******EVENTOS*******

function loadNegociacaoBaixa() {
    _negociacaoBaixa = new NegociacaoBaixa();
    KoBindings(_negociacaoBaixa, "knockoutDetalhesNegociacaoBaixa");

    _detalheParcela = new DetalheParcela();
    KoBindings(_detalheParcela, "knoutDetalheParcela");

    _crudNegociacaoBaixa = new CRUDNegociacaoBaixa();
    KoBindings(_crudNegociacaoBaixa, "knockoutCRUDNegociacaoBaixaTituloReceber");

    _parcelaNegociacaoBaixa = new ParcelaNegociacaoBaixa();
    KoBindings(_parcelaNegociacaoBaixa, "knockoutParcelasNegociacao");

    _fechamentoBaixa = new FechamentoBaixa();
    KoBindings(_fechamentoBaixa, "knockoutFechamentoBaixa");

    _progressNegociacaoBaixaReceber = new ProgressNegociacaoBaixaReceber();
    KoBindings(_progressNegociacaoBaixaReceber, "knockoutProgressFinalizacaoBaixa");

    new BuscarTipoPagamento(_fechamentoBaixa.TipoPagamento);

    _negociacaoBaixa.CodigoFatura.val(_baixaTituloReceber.CodigoFatura.val());

    var detalhe = { descricao: "Detalhe", id: guid(), evento: "onclick", metodo: DetalheParcelaNegociacaoClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhe);

    _gridParcelasNegociacao = new GridView(_parcelaNegociacaoBaixa.Parcelas.idGrid, "BaixaTituloReceberNovo/PesquisaParcelasNegociacao", _negociacaoBaixa, menuOpcoes, { column: 2, dir: orderDir.asc });
    _gridParcelasNegociacao.CarregarGrid();

    LoadDocumentoNegociacaoBaixaTituloReceber();
    LoadChequeBaixa();
}

function GerarParcelasClick(e, sender) {

    _parcelaNegociacaoBaixa.IntervaloDeDias.requiredClass("form-control");
    _parcelaNegociacaoBaixa.DataPrimeiroVencimento.requiredClass("form-control");
    _parcelaNegociacaoBaixa.DataEmissao.requiredClass("form-control");

    var valido = true;
    if (_parcelaNegociacaoBaixa.DataPrimeiroVencimento.val() == "") {
        valido = false;
        _parcelaNegociacaoBaixa.DataPrimeiroVencimento.requiredClass("form-control is-invalid");
    }

    if (valido) {
        exibirConfirmacao("Confirmação", "Realmente deseja gerar as parcelas?", function () {
            var data = {
                Codigo: _baixaTituloReceber.Codigo.val(),
                QuantidadeParcelas: _parcelaNegociacaoBaixa.QuantidadeParcelas.val(),
                IntervaloDeDias: _parcelaNegociacaoBaixa.IntervaloDeDias.val(),
                DataPrimeiroVencimento: _parcelaNegociacaoBaixa.DataPrimeiroVencimento.val(),
                DataEmissao: _parcelaNegociacaoBaixa.DataEmissao.val(),
                TipoArredondamento: _parcelaNegociacaoBaixa.TipoArredondamento.val()
            };

            executarReST("BaixaTituloReceberNovo/GerarParcelas", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        CarregarNegociacaoBaixa();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Aviso", arg.Msg);
                }
            });
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Por favor verifique os campos obrigatórios.");
    }
}

function SalvarParcelaClick(e, sender) {
    if (ValidarCamposObrigatorios(e)) {
        var data = {
            Codigo: _baixaTituloReceber.Codigo.val(),
            CodigoParcela: e.Codigo.val(),
            Sequencia: e.Sequencia.val(),
            Valor: e.Valor.val(),
            ValorDesconto: e.ValorDesconto.val(),
            DataEmissao: e.DataEmissao.val(),
            DataVencimento: e.DataVencimento.val(),
        };

        executarReST("BaixaTituloReceberNovo/AtualizarParcela", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    LimparCampos(e);
                    Global.fecharModal('divDetalheParcela');
                    CarregarNegociacaoBaixa();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function FecharBaixaClick(e, sender) {
    executarReST("BaixaTituloReceberNovo/ObterDetalhesFinalizacaoBaixa", { Codigo: _baixaTituloReceber.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (!r.Data.Validacao.Valido) {
                    if (r.Data.Validacao.PermiteFinalizarBaixa) {
                        exibirConfirmacao("Confirmação", r.Data.Validacao.Mensagem, function () {
                            if (r.Data.ValorPago > 0)
                                AbrirTelaFinalizacaoBaixaTituloReceber(r.Data);
                            else {
                                FinalizarBaixa();
                            }
                        }, null, "Confirmar", "Cancelar");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", r.Data.Validacao.Mensagem, 30000);
                    }
                } else {
                    if (r.Data.ValorPago > 0)
                        AbrirTelaFinalizacaoBaixaTituloReceber(r.Data);
                    else {
                        exibirConfirmacao("Confirmação", "Deseja realmente finalizar esta baixa?", function () {
                            FinalizarBaixa();
                        });
                    }
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AbrirTelaFinalizacaoBaixaTituloReceber(dados) {
    LimparCampos(_fechamentoBaixa);

    _fechamentoBaixa.TipoPagamento.val(dados.TipoPagamento.Descricao);
    _fechamentoBaixa.TipoPagamento.codEntity(dados.TipoPagamento.Codigo);

    Global.abrirModal('knockoutFechamentoBaixa');
}

function FecharTelaFinalizacaoBaixaTituloReceber() {
    LimparCampos(_fechamentoBaixa);
    Global.fecharModal('knockoutFechamentoBaixa');    
}

function FinalizarBaixaClick(e, sender) {
    if (ValidarCamposObrigatorios(_fechamentoBaixa)) {
        FinalizarBaixa();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");
    }
}

function FinalizarBaixa() {
    var data = {
        Codigo: _baixaTituloReceber.Codigo.val(),
        TipoPagamento: _fechamentoBaixa.TipoPagamento.codEntity()
    };
    executarReST("BaixaTituloReceberNovo/FecharBaixa", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _baixaTituloReceber.Etapa.val(EnumEtapasBaixaTituloReceber.Finalizada);
                FecharTelaFinalizacaoBaixaTituloReceber();
                CarregarDadosCabecalho(arg.Data);
                PosicionarEtapa(arg.Data);
                VerificarBotoes();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function DetalheParcelaNegociacaoClick(e, sender) {

    LimparCampos(_detalheParcela);

    var data = { Codigo: e.Codigo };

    executarReST("BaixaTituloReceberNovo/CarregarDadosParcela", data, function (e) {
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

//*******MÉTODOS*******

function LimparCamposNegociacao() {
    LimparCampos(_negociacaoBaixa);
    _gridParcelasNegociacao.CarregarGrid();
    LimparCamposChequeBaixa();
}

function CarregarNegociacaoBaixa() {
    LimparCampos(_negociacaoBaixa);

    if (_baixaTituloReceber.Codigo.val() > 0) {
        var data = { Codigo: _baixaTituloReceber.Codigo.val() };

        executarReST("BaixaTituloReceberNovo/CarregarNegociacaoBaixa", data, function (e) {
            if (e.Success) {
                if (e.Data) {

                    PreencherObjetoKnout(_negociacaoBaixa, e);

                    if (e.Data.Moeda != EnumMoedaCotacaoBancoCentral.Real) {
                        _negociacaoBaixa.Moeda.text(EnumMoedaCotacaoBancoCentral.obterDescricao(e.Data.Moeda));
                        _negociacaoBaixa.Moeda.visible(true);
                    } else
                        _negociacaoBaixa.Moeda.visible(false);


                    if (e.Data.Situacao === EnumSituacaoBaixaTitulo.Finalizada || e.Data.Situacao === EnumSituacaoBaixaTitulo.Cancelada) {
                        _negociacaoBaixa.TipoPagamentoRecebimento.visible(true);
                        _negociacaoBaixa.PlanoConta.visible(true);
                    } else {
                        _negociacaoBaixa.TipoPagamentoRecebimento.visible(false);
                        _negociacaoBaixa.PlanoConta.visible(false);
                    }

                    if (e.Data.Situacao === 1 || e.Data.Situacao == 2)
                        _negociacaoBaixa.SaldoContaAdiantamento.visible(true);
                    else
                        _negociacaoBaixa.SaldoContaAdiantamento.visible(false);

                    _gridParcelasNegociacao.CarregarGrid();

                    carregarDadosChequeBaixa();

                    if (e.Data.CodigoPessoa > 0)
                        $('#knockoutParcelasNegociacao').show();
                    else
                        $('#knockoutParcelasNegociacao').hide();

                    _baixaTituloReceber.CodigoFatura.val(e.Data.CodigoFatura);

                    VerificarBotoes();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", e.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
        });
    }
    else if (_gridParcelasNegociacao != null)
        _gridParcelasNegociacao.CarregarGrid();
}

