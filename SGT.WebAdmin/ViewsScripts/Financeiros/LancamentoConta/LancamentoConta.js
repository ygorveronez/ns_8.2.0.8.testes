/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/TipoPagamentoRecebimento.js" />
/// <reference path="../../Consultas/Veiculo.js" />
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
/// <reference path="../../Enumeradores/EnumTipoTitulo.js" />
/// <reference path="../../Enumeradores/EnumPeriodicidade.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../RateioDespesaVeiculo/RateioDespesaVeiculo.js" />

var _HTMLLancamentoConta = "";
var _gridTitulosSimulacao;
var _gridLancamentoContaVeiculo;

var _periodo = [
    { text: "Mensal", value: EnumPeriodicidade.Mensal },
    { text: "Semanal", value: EnumPeriodicidade.Semanal },
    { text: "Bimestral", value: EnumPeriodicidade.Bimestral },
    { text: "Trimestral", value: EnumPeriodicidade.Trimestral },
    { text: "Semestral", value: EnumPeriodicidade.Semestral },
    { text: "Anual", value: EnumPeriodicidade.Anual }
];

var LancamentoConta = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true) });
    this.IdModal = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });

    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, text: "*Data de Emissão:", required: true, enable: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, text: "*Data de Vencimento:", required: true, enable: ko.observable(true) });
    this.DataRecebimento = PropertyEntity({ getType: typesKnockout.date, text: "Data:", required: false, visible: ko.observable(false), enable: ko.observable(true) });

    this.Valor = PropertyEntity({ text: "*Valor Documento: ", required: true, getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorMulta = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Multa/Juros:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorDesconto = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Desconto/Taxas:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorRecebido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Baixado:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.TipoPagamentoRecebimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Conta:"), idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false) });

    this.NumeroOcorrencia = PropertyEntity({ getType: typesKnockout.int, text: "Número Ocorrência:", visible: ko.observable(false), enable: ko.observable(true) });
    this.Recebido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "O título que esta lançando já foi pago?", def: false });
    this.Repetir = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable("Deseja REPETIR o lançamento? Isso gerará mais parcelas do mesmo valor informado."), def: false });
    this.Dividir = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable("Deseja DIVIDIR o lançamento? Isso gerará mais parcelas com o valor informado dividido entre as parcelas."), def: false });
    this.Provisao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Este título é uma Provisão?", def: false, visible: ko.observable(true) });
    this.AdiantamentoMultiploFornecedor = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Este titulo é referente a adiantamento de Fornecedor?", def: false, visible: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Movimento:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoTitulo.AReceber), def: EnumTipoTitulo.AReceber, options: EnumTipoTitulo.obterOpcoes(), text: "*Tipo:", required: true, enable: ko.observable(true) });
    this.TipoRepetir = PropertyEntity({ val: ko.observable(EnumPeriodicidade.Mensal), def: EnumPeriodicidade.Mensal, options: _periodo, text: "Repetição:", required: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "*Descrição/Observação:", required: true, maxlength: 5000 });

    this.TipoDocumento = PropertyEntity({ text: "Tipo Documento:", required: false, maxlength: 100 });
    this.Documento = PropertyEntity({ text: "*Documento:", required: true, maxlength: 300 });
    this.TipoMovimentoJuros = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Movimento do Juros:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.ValorJuros = PropertyEntity({ text: "Valor Juros: ", required: false, getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true) });
    this.DiaVencimento = PropertyEntity({ getType: typesKnockout.int, text: "Dia do Vencimento:", visible: ko.observable(false), enable: ko.observable(true) });
    this.SimularParcelas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deseja simular/editar as parcelas?", def: false, visible: ko.observable(false) });
    this.Titulos = PropertyEntity({ type: types.map, val: ko.observable(""), list: new Array(), visible: ko.observable(false) });
    this.ListaTitulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(false), visible: ko.observable(false) });

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(true), def: true, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: false });

    //Aba Veículos    
    this.VeiculoLancamentoConta = PropertyEntity({ type: types.event, text: "Adicionar Veículo", idBtnSearch: guid(), idGrid: guid() });
    this.ListaVeiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //CRUD
    this.Salvar = PropertyEntity({ type: types.event, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
    this.VeiculoLancamentoConta = PropertyEntity({ type: types.event, text: "Adicionar Veículo", idBtnSearch: guid(), idGrid: guid() });
};

function LancarContas(callbackInit) {

    var lancarContas = this;
    var valorJuroAnterior = 0;
    var valorDescontoAnterior = 0;

    this.LoadLancarContas = function () {
        lancarContas.IdModal = guid();
        lancarContas.IdKnockoutLancarConta = "knockoutLancamentoConta_" + lancarContas.IdModal;

        lancarContas.LancamentoConta = new LancamentoConta();
        lancarContas.LancamentoConta.IdModal.val(lancarContas.IdModal);
        lancarContas.LancamentoConta.DataRecebimento.val(moment().format("DD/MM/YYYY"));
        lancarContas.RenderizarModalContas(lancarContas, callbackInit);

        if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
            lancarContas.LancamentoConta.MoedaCotacaoBancoCentral.visible(true);
            lancarContas.LancamentoConta.DataBaseCRT.visible(true);
            lancarContas.LancamentoConta.ValorMoedaCotacao.visible(true);
            lancarContas.LancamentoConta.ValorOriginalMoedaEstrangeira.visible(true);
        }
    };

    this.FecharModal = function () {
        Global.fecharModal(lancarContas.IdModal);
    };

    this.Destroy = function () {
        $("#" + lancarContas.IdModal).remove();
        lancarContas = null;
    };

    this.HabilitarCamposRecebido = function () {

        if (lancarContas.LancamentoConta.Recebido.val() === false) {
            lancarContas.LancamentoConta.DataRecebimento.val(moment().format("DD/MM/YYYY"));
            lancarContas.LancamentoConta.ValorMulta.val("0,00");
            lancarContas.LancamentoConta.ValorDesconto.val("0,00");
            lancarContas.LancamentoConta.ValorRecebido.val("0,00");

            lancarContas.LancamentoConta.DataRecebimento.visible(false);
            lancarContas.LancamentoConta.ValorMulta.visible(false);
            lancarContas.LancamentoConta.ValorDesconto.visible(false);
            lancarContas.LancamentoConta.ValorRecebido.visible(false);
            lancarContas.LancamentoConta.TipoPagamentoRecebimento.visible(false);

            lancarContas.LancamentoConta.Repetir.text("Deseja REPETIR o lançamento? Isso gerará mais parcelas do mesmo valor informado.");
            lancarContas.LancamentoConta.Dividir.text("Deseja DIVIDIR o lançamento? Isso gerará mais parcelas com o valor informado dividido entre as parcelas.");
        } else {
            lancarContas.LancamentoConta.DataRecebimento.visible(true);
            lancarContas.LancamentoConta.ValorMulta.visible(true);
            lancarContas.LancamentoConta.ValorDesconto.visible(true);
            lancarContas.LancamentoConta.ValorRecebido.visible(true);
            lancarContas.LancamentoConta.TipoPagamentoRecebimento.visible(true);

            lancarContas.LancamentoConta.Repetir.text("Deseja REPETIR o lançamento? Isso gerará mais parcelas do mesmo valor informado (Primeira parcela será quitada).");
            lancarContas.LancamentoConta.Dividir.text("Deseja DIVIDIR o lançamento? Isso gerará mais parcelas com o valor informado dividido entre as parcelas (Primeira parcela será quitada).");
            lancarContas.LancamentoConta.ValorRecebido.val(lancarContas.LancamentoConta.Valor.val());
        }
    };

    this.HabilitarCamposDividir = function () {
        lancarContas.LancamentoConta.Repetir.val(false);

        if (lancarContas.LancamentoConta.Dividir.val() === false) {
            lancarContas.LancamentoConta.TipoRepetir.val(EnumPeriodicidade.Mensal);
            lancarContas.LancamentoConta.NumeroOcorrencia.val("");
            lancarContas.LancamentoConta.DiaVencimento.val("");
            lancarContas.LancamentoConta.SimularParcelas.val(false);

            lancarContas.LancamentoConta.TipoRepetir.visible(false);
            lancarContas.LancamentoConta.NumeroOcorrencia.visible(false);
            lancarContas.LancamentoConta.DiaVencimento.visible(false);
            lancarContas.LancamentoConta.SimularParcelas.visible(false);
        } else {
            lancarContas.LancamentoConta.TipoRepetir.visible(true);
            lancarContas.LancamentoConta.NumeroOcorrencia.visible(true);
            lancarContas.LancamentoConta.DiaVencimento.visible(true);
            lancarContas.LancamentoConta.SimularParcelas.visible(true);
        }
    };

    this.HabilitarCamposRepetir = function () {
        lancarContas.LancamentoConta.Dividir.val(false);

        if (lancarContas.LancamentoConta.Repetir.val() === false) {
            lancarContas.LancamentoConta.TipoRepetir.val(EnumPeriodicidade.Mensal);
            lancarContas.LancamentoConta.NumeroOcorrencia.val("");
            lancarContas.LancamentoConta.DiaVencimento.val("");
            lancarContas.LancamentoConta.SimularParcelas.val(false);

            lancarContas.LancamentoConta.TipoRepetir.visible(false);
            lancarContas.LancamentoConta.NumeroOcorrencia.visible(false);
            lancarContas.LancamentoConta.DiaVencimento.visible(false);
            lancarContas.LancamentoConta.SimularParcelas.visible(false);
        } else {
            lancarContas.LancamentoConta.TipoRepetir.visible(true);
            lancarContas.LancamentoConta.NumeroOcorrencia.visible(true);
            lancarContas.LancamentoConta.DiaVencimento.visible(true);
            lancarContas.LancamentoConta.SimularParcelas.visible(true);
        }
    };

    this.CalcularValorJuro = function () {
        var valorBaixado = Globalize.parseFloat(lancarContas.LancamentoConta.ValorRecebido.val());
        var valorJuro = Globalize.parseFloat(lancarContas.LancamentoConta.ValorMulta.val());

        if (valorJuro !== valorJuroAnterior) {
            lancarContas.LancamentoConta.ValorRecebido.val(Globalize.format((valorBaixado - valorJuroAnterior) + valorJuro, "n2"));
            valorJuroAnterior = valorJuro;
        }
    };

    this.CalcularValorDesconto = function () {
        var valorBaixado = Globalize.parseFloat(lancarContas.LancamentoConta.ValorRecebido.val());
        var valorDesconto = Globalize.parseFloat(lancarContas.LancamentoConta.ValorDesconto.val());

        if (valorDesconto !== valorDescontoAnterior && valorBaixado > valorDesconto) {
            lancarContas.LancamentoConta.ValorRecebido.val(Globalize.format((valorBaixado + valorDescontoAnterior) - valorDesconto, "n2"));
            valorDescontoAnterior = valorDesconto;
        }
    };

    this.HabilitarCamposSimular = function () {

        if (lancarContas.LancamentoConta.SimularParcelas.val() === false) {
            lancarContas.LancamentoConta.Titulos.visible(false);
        } else {
            var valido = ValidarCamposObrigatorios(lancarContas.LancamentoConta);
            lancarContas.LancamentoConta.NumeroOcorrencia.requiredClass("form-control");

            if (lancarContas.LancamentoConta.Repetir.val() === true || lancarContas.LancamentoConta.Dividir.val() === true) {
                if (lancarContas.LancamentoConta.TipoRepetir.val() === "" || lancarContas.LancamentoConta.NumeroOcorrencia.val() <= 0) {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o Tipo e também o número de ocorrências (parcelas)!");
                    lancarContas.LancamentoConta.TipoRepetir.requiredClass("form-control");
                    lancarContas.LancamentoConta.NumeroOcorrencia.requiredClass("form-control is-invalid");
                }
            }

            if (valido) {
                lancarContas.LancamentoConta.Titulos.visible(true);
                lancarContas.LancamentoConta.SelecionarTodos.val(true);

                var objeto = ObterObjetoContas(lancarContas);
                var dados = { Conta: objeto };
                executarReST("LancamentoConta/PesquisaTitulos", dados, function (arg) {
                    if (arg.Success) {
                        recarregarGridTitulosSimulacao(arg.Data, lancarContas);
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            } else {
                lancarContas.LancamentoConta.Titulos.visible(false);
                lancarContas.LancamentoConta.SimularParcelas.val(false);
            }
        }
    };

    this.AdicionarTitulos = function () {
        var valido = ValidarCamposObrigatorios(lancarContas.LancamentoConta);
        lancarContas.LancamentoConta.TipoPagamentoRecebimento.requiredClass("form-control");
        lancarContas.LancamentoConta.DataRecebimento.requiredClass("form-control");
        lancarContas.LancamentoConta.ValorRecebido.requiredClass("form-control");
        lancarContas.LancamentoConta.TipoRepetir.requiredClass("form-control");
        lancarContas.LancamentoConta.NumeroOcorrencia.requiredClass("form-control");
        lancarContas.LancamentoConta.TipoMovimentoJuros.requiredClass("form-control");
        lancarContas.LancamentoConta.ValorJuros.requiredClass("form-control");

        if (valido) {
            if (lancarContas.LancamentoConta.Recebido.val() === true) {
                if (lancarContas.LancamentoConta.DataRecebimento.val() === "" || Globalize.parseFloat(lancarContas.LancamentoConta.ValorRecebido.val()) <= 0 || lancarContas.LancamentoConta.TipoPagamentoRecebimento.codEntity() === 0) {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Data, Valor de Recebimento e o tipo do recebimento/pagamento!");
                    lancarContas.LancamentoConta.TipoPagamentoRecebimento.requiredClass("form-control is-invalid");
                    lancarContas.LancamentoConta.DataRecebimento.requiredClass("form-control is-invalid");
                    lancarContas.LancamentoConta.ValorRecebido.requiredClass("form-control is-invalid");
                    return;
                }
            }
            if (lancarContas.LancamentoConta.Repetir.val() === true || lancarContas.LancamentoConta.Dividir.val() === true) {
                if (lancarContas.LancamentoConta.TipoRepetir.val() === "" || lancarContas.LancamentoConta.NumeroOcorrencia.val() <= 0) {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o Tipo e também o número de ocorrências (parcelas)!");
                    lancarContas.LancamentoConta.TipoRepetir.requiredClass("form-control");
                    lancarContas.LancamentoConta.NumeroOcorrencia.requiredClass("form-control is-invalid");
                    return;
                }
            }
            if (lancarContas.LancamentoConta.ValorJuros.val() !== "" && lancarContas.LancamentoConta.ValorJuros.val() > 0 && lancarContas.LancamentoConta.TipoMovimentoJuros.codEntity() === 0) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe tipo do movimento do juros!");
                lancarContas.LancamentoConta.ValorJuros.requiredClass("form-control is-invalid");
                lancarContas.LancamentoConta.TipoMovimentoJuros.requiredClass("form-control is-invalid");
                return;
            }
        }

        PreencherListaTitulosSimulacao(lancarContas);
        PreencherListaVeiculos(lancarContas);

        var objeto = ObterObjetoContas(lancarContas);
        if (valido) {
            var dados = {
                Conta: objeto,
                ListaTitulos: lancarContas.LancamentoConta.ListaTitulos.val(),
                ListaVeiculos: lancarContas.LancamentoConta.ListaVeiculos.val()
            };
            executarReST("LancamentoConta/Salvar", dados, function (arg) {
                if (arg.Success) {
                    lancarContas.FecharModal();
                    if (arg.Data) {
                        var data = arg.Data;
                        if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente && lancarContas.LancamentoConta.Tipo.val() === EnumTipoTitulo.APagar) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Título(s) salvo(s) com sucesso, favor digite o lançamento de rateio.");

                            LimparCamposRateioDespesa();
                            Global.abrirModal('divModalDespesaVeiculo');
                            _rateioDespesa.MultiplosTitulos.val(true);
                            _rateioDespesa.NumeroDocumento.val(data.NumeroDocumento);
                            _rateioDespesa.TipoDocumento.val(data.TipoDocumento);
                            _rateioDespesa.Valor.val(data.Valor);

                            _rateioDespesa.Pessoa.val(data.NomePessoa);
                            _rateioDespesa.Pessoa.codEntity(data.CNPJPessoa);

                            _rateioDespesa.Colaborador.val(data.NomeColaborador);
                            _rateioDespesa.Colaborador.codEntity(data.CodigoColaborador);
                        }
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Título(s) salvo(s) com sucesso");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            });
        }
        else
            exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    };

    this.SalvarClick = function () {
        var valido = ValidarCamposObrigatorios(lancarContas.LancamentoConta);

        if (valido) {
            var data = {
                TipoMovimento: lancarContas.LancamentoConta.TipoMovimento.codEntity(),
                DataEmissao: Global.DataAtual()
            };

            executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== true && arg.Data.Mensagem !== "") {
                        if (arg.Data.TipoLancamentoFinanceiroSemOrcamento === EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                            exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                                lancarContas.AdicionarTitulos();
                            });
                        } else
                            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    } else {
                        lancarContas.AdicionarTitulos();
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        } else
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    };

    this.RenderizarModalContas = function () {
        lancarContas.ObterHTMLLancamentoContas().then(function () {
            var html = _HTMLLancamentoConta.replace(/#divModalLancamentoConta/g, lancarContas.IdModal);
            $('#js-page-content').append(html);
            lancarContas.LancamentoConta.Cancelar.eventClick = function (e) {
                lancarContas.FecharModal();
            };
            lancarContas.LancamentoConta.Salvar.eventClick = function (e) {
                lancarContas.SalvarClick();
            };

            lancarContas.LancamentoConta.MoedaCotacaoBancoCentral.val.subscribe(function (novoValor) {
                CalcularMoedaEstrangeiraMultiploTitulo(lancarContas);
            });

            lancarContas.LancamentoConta.DataBaseCRT.val.subscribe(function (novoValor) {
                CalcularMoedaEstrangeiraMultiploTitulo(lancarContas);
            });

            lancarContas.LancamentoConta.ValorMoedaCotacao.val.subscribe(function (novoValor) {
                ConverterValorMultiploTitulo(lancarContas);
            });

            lancarContas.LancamentoConta.Valor.val.subscribe(function (novoValor) {
                ConverterValorMultiploTitulo(lancarContas);
            });

            KoBindings(lancarContas.LancamentoConta, lancarContas.IdKnockoutLancarConta);

            $("#" + lancarContas.LancamentoConta.Recebido.id).click(lancarContas.HabilitarCamposRecebido);
            $("#" + lancarContas.LancamentoConta.Repetir.id).click(lancarContas.HabilitarCamposRepetir);
            $("#" + lancarContas.LancamentoConta.Dividir.id).click(lancarContas.HabilitarCamposDividir);
            $("#" + lancarContas.LancamentoConta.SimularParcelas.id).click(lancarContas.HabilitarCamposSimular);

            new BuscarEmpresa(lancarContas.LancamentoConta.Empresa);
            new BuscarClientes(lancarContas.LancamentoConta.Pessoa, lancarContas.RetornoPessoaLancamentoConta);
            new BuscarTipoMovimento(lancarContas.LancamentoConta.TipoMovimento, null, null, null, null, EnumFinalidadeTipoMovimento.MultiploTitulo);
            new BuscarTipoMovimento(lancarContas.LancamentoConta.TipoMovimentoJuros, null, null, null, null, EnumFinalidadeTipoMovimento.MultiploTitulo);
            new BuscarTipoPagamentoRecebimento(lancarContas.LancamentoConta.TipoPagamentoRecebimento);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                lancarContas.LancamentoConta.Empresa.visible(true);
                lancarContas.LancamentoConta.Empresa.required = true;
            }

            Global.abrirModal(lancarContas.IdModal);

            $('#' + lancarContas.IdModal).on('hidden.bs.modal', function () {
                lancarContas.Destroy();
            });

            $("#" + lancarContas.LancamentoConta.ValorDesconto.id).change(function () { lancarContas.CalcularValorDesconto(); });
            $("#" + lancarContas.LancamentoConta.ValorMulta.id).change(function () { lancarContas.CalcularValorJuro(); });

            var editarRespostaTitulo = {
                permite: true,
                callback: lancarContas.SalvarRetornoTituloGrid,
                atualizarRow: true
            };

            var _editableDecimalConfig = {
                editable: true,
                type: EnumTipoColunaEditavelGrid.decimal,
                numberMask: ConfigDecimal({ allowZero: true })
            };

            var _editableStringConfig = {
                editable: true,
                type: EnumTipoColunaEditavelGrid.string
            };

            var _editableDateConfig = {
                editable: true,
                type: EnumTipoColunaEditavelGrid.data
            };

            var _editableIntConfig = {
                editable: true,
                type: EnumTipoColunaEditavelGrid.int,
                numberMask: ConfigInt()
            };

            var header = [
                { data: "Codigo", visible: false },
                { data: "Sequencia", title: "Sequência", width: "10%", editableCell: _editableIntConfig },
                { data: "Observacao", title: "Observação", width: "20%", editableCell: _editableStringConfig },
                { data: "Documento", title: "Nº Documento", width: "10%", editableCell: _editableStringConfig },
                { data: "TipoDocumento", title: "Tipo Documento", width: "10%", editableCell: _editableStringConfig },
                { data: "DataVencimento", title: "Vencimento", width: "10%", editableCell: _editableDateConfig },
                { data: "Valor", title: "Valor", width: "10%", editableCell: _editableDecimalConfig },
                { data: "Juros", title: "Juros", width: "10%", editableCell: _editableDecimalConfig }
            ];

            _gridTitulosSimulacao = new BasicDataTable(lancarContas.LancamentoConta.Titulos.id, header, null, { column: 1, dir: orderDir.asc }, null, 5000, null, null, editarRespostaTitulo);

            var menuOpcoesVeiculo = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirVeiculoLancamentoContaClick }] };
            var headerVeiculo = [
                { data: "Codigo", visible: false },
                { data: "Placa", title: "Placa", width: "12%" },
                { data: "NumeroFrota", title: "Nº Frota", width: "12%" },
                { data: "ModeloVeicularCarga", title: "Modelo Veicular", width: "66%" }
            ];

            _gridLancamentoContaVeiculo = new BasicDataTable(lancarContas.LancamentoConta.VeiculoLancamentoConta.idGrid, headerVeiculo, menuOpcoesVeiculo, { column: 1, dir: orderDir.asc });
            new BuscarVeiculos(lancarContas.LancamentoConta.VeiculoLancamentoConta, null, null, null, null, null, null, null, null, null, null, null, null, _gridLancamentoContaVeiculo);
            _gridLancamentoContaVeiculo.CarregarGrid([]);

            if (callbackInit !== null && callbackInit !== undefined) {
                callbackInit();
            }
        });
    };

    this.RetornoPessoaLancamentoConta = function (pessoa) {
        lancarContas.LancamentoConta.Pessoa.codEntity(pessoa.Codigo);
        lancarContas.LancamentoConta.Pessoa.val(pessoa.Descricao);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe && lancarContas.LancamentoConta.Tipo.val() === EnumTipoTitulo.APagar) {
            executarReST("Pessoa/BuscarDadosPessoaPorEmpresa", { Codigo: pessoa.Codigo }, function (r) {
                if (r.Success) {
                    if (r.Data) {
                        if (r.Data.TipoMovimento != null) {
                            lancarContas.LancamentoConta.TipoMovimento.codEntity(r.Data.TipoMovimento.Codigo);
                            lancarContas.LancamentoConta.TipoMovimento.val(r.Data.TipoMovimento.Descricao);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
                }
            });
        }
    };

    this.SalvarRetornoTituloGrid = function (dataRow) {
        var data = lancarContas.GetTitulos();

        for (var i in data) {
            if (data[i].Codigo === dataRow.Codigo) {
                data[i].Sequencia = dataRow.Sequencia;
                data[i].Observacao = dataRow.Observacao;
                data[i].Documento = dataRow.Documento;
                data[i].TipoDocumento = dataRow.TipoDocumento;
                data[i].DataVencimento = dataRow.DataVencimento;
                data[i].Valor = dataRow.Valor;
                data[i].Juros = dataRow.Juros;
                break;
            }
        }

        lancarContas.SetTitulos(data);
    };

    this.GetTitulos = function () {
        return lancarContas.LancamentoConta.Titulos.list.slice();
    };

    this.SetTitulos = function (data) {
        return lancarContas.LancamentoConta.Titulos.list = data.slice();
    };

    this.ObterHTMLLancamentoContas = function () {
        var p = new promise.Promise();
        if (_HTMLLancamentoConta === "") {
            $.get("Content/Static/Financeiro/LancamentoConta.html?dyn=" + lancarContas.IdModal, function (data) {
                _HTMLLancamentoConta = data;
                p.done();
            });
        } else {
            p.done();
        }
        return p;
    };

    setTimeout(function () {
        lancarContas.LoadLancarContas();
    }, 50);
}

function CalcularMoedaEstrangeiraMultiploTitulo(lancarContas) {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: lancarContas.LancamentoConta.MoedaCotacaoBancoCentral.val(), DataBaseCRT: lancarContas.LancamentoConta.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                lancarContas.LancamentoConta.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValor();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorMultiploTitulo(lancarContas) {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(lancarContas.LancamentoConta.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(lancarContas.LancamentoConta.Valor.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            lancarContas.LancamentoConta.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorMoedaCotacao * valorOriginal, "n2"));
        }
        else
            lancarContas.LancamentoConta.ValorOriginalMoedaEstrangeira.val("0,00");
    }
}

function ObterObjetoContas(lancarContas) {
    var lancarConta = new Object();
    lancarConta = RetornarObjetoPesquisa(lancarContas.LancamentoConta);

    return JSON.stringify(lancarConta);
}

function PreencherListaTitulosSimulacao(lancarContas) {
    var listaTitulo = new Array();

    $.each(_gridTitulosSimulacao.BuscarRegistros(), function (i, titulo) {
        listaTitulo.push({ Titulo: titulo });
    });

    lancarContas.LancamentoConta.ListaTitulos.val(JSON.stringify(listaTitulo));
}

function PreencherListaVeiculos(lancarContas) {
    var listaVeiculos = new Array();

    $.each(_gridLancamentoContaVeiculo.BuscarRegistros(), function (i, veiculo) {
        listaVeiculos.push(veiculo.Codigo);
    });

    lancarContas.LancamentoConta.ListaVeiculos.val(JSON.stringify(listaVeiculos));
}

function recarregarGridTitulosSimulacao(data, lancarContas) {
    var dataGrid = new Array();

    $.each(data.ListaTitulos, function (i, titulo) {
        var obj = new Object();
        obj.DT_Enable = true;
        obj.Codigo = titulo.Codigo;
        obj.Sequencia = titulo.Sequencia;
        obj.Observacao = titulo.Observacao;
        obj.Documento = titulo.Documento;
        obj.TipoDocumento = titulo.TipoDocumento;
        obj.DataVencimento = titulo.DataVencimento;
        obj.Valor = titulo.Valor;
        obj.Juros = titulo.Juros;

        dataGrid.push(obj);
    });
    lancarContas.SetTitulos(dataGrid);
    _gridTitulosSimulacao.CarregarGrid(dataGrid);
}

function ExcluirVeiculoLancamentoContaClick(data) {

    var veiculosGridLancamentoConta = _gridLancamentoContaVeiculo.BuscarRegistros();

    for (var i = 0; i < veiculosGridLancamentoConta.length; i++) {
        if (data.Codigo == veiculosGridLancamentoConta[i].Codigo) {
            veiculosGridLancamentoConta.splice(i, 1);
            break;
        }
    }
    _gridLancamentoContaVeiculo.CarregarGrid(veiculosGridLancamentoConta);
}