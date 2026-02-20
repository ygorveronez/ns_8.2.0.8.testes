/// <reference path="Infracao.js" />
/// <reference path="ComissaoMotorista.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumResponsavelPagamentoInfracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoInfracao.js" />
/// <reference path="../../Financeiros/RateioDespesaVeiculo/RateioDespesaVeiculo.js" />
/// <reference path="../../Ocorrencias/Ocorrencia/Ocorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoTitulo.js" /> 
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _processamentoInfracao, _gridParcelasInfracao, _modalDespesaVeiculoInfracao, _modalOcorrenciaInfracao;

/*
 * Declaração das Classes
 */

var ProcessamentoInfracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //Processamento da Ocorrência
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: ko.observable("*Motorista:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(false) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable("*Funcionário:"), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable("*Pessoa:"), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false), cssClass: ko.observable("col col-xs-8") });
    this.ResponsavelPagamento = PropertyEntity({ val: ko.observable(EnumResponsavelPagamentoInfracao.Condutor), options: EnumResponsavelPagamentoInfracao.obterOpcoes(), def: EnumResponsavelPagamentoInfracao.Condutor, text: ko.observable("*Pago pelo(a): "), enable: ko.observable(false), cssClass: ko.observable("col col-xs-4") });
    this.TipoTitulo = PropertyEntity({ val: ko.observable(EnumTipoTitulo.AReceber), options: EnumTipoTitulo.obterOpcoes(), def: EnumTipoTitulo.AReceber, text: "Tipo do Título:", visible: ko.observable(false), enable: ko.observable(false) });
    this.GerarOcorrencia = PropertyEntity({ text: "Gerar Ocorrência? ", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });

    //Descontar do Condutor
    this.TipoMovimentoTitulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: ko.observable("*Tipo Movimento p/ Título:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(false) });
    this.DataCompensacao = PropertyEntity({ text: ko.observable("*Compensação: "), getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ text: ko.observable("*Vencimento: "), getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(false) });
    this.Valor = PropertyEntity({ text: ko.observable("*Valor até Vencimento:"), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorAposVencimento = PropertyEntity({ text: ko.observable("*Valor após Vencimento:"), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false } });

    //Parcelas
    this.Parcelas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.GridParcelas = PropertyEntity({ type: types.local });

    this.QuantidadeParcela = PropertyEntity({ text: "Qtd. Parcelas: ", getType: typesKnockout.int, def: "1", val: ko.observable("1"), required: false, enable: ko.observable(false) });
    this.IntervaloDiasParcela = PropertyEntity({ text: "Intervalo (dias): ", getType: typesKnockout.int, required: false, enable: ko.observable(false) });
    this.TipoArredondamentoParcela = PropertyEntity({ text: "Arredondar:", options: EnumTipoArredondamento.ObterOpcoes(), val: ko.observable(EnumTipoArredondamento.PrimeiroItem), def: EnumTipoArredondamento.PrimeiroItem, issue: 0, required: false, enable: ko.observable(false) });
    this.DataVencimentoParcela = PropertyEntity({ text: "Vencimento: ", getType: typesKnockout.date, required: false, enable: ko.observable(false) });

    this.GerarParcelas = PropertyEntity({ eventClick: GerarParcelasClick, type: types.event, text: "Gerar Parcelas", enable: ko.observable(false) });

    this.DescontarLancamentoAgregadoTerceiro = PropertyEntity({ text: "Descontar lançamento do próximo fechamento", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.GerarTituloEmpresa = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Deseja gerar um título a pagar para a empresa?", enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoMovimentoEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Tipo Movimento p/ Título a Pagar:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.PessoaTituloEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Pessoa:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });

    this.GerarTituloEmpresa.val.subscribe(function (novoValor) {
        if (!novoValor) {
            _processamentoInfracao.TipoMovimentoEmpresa.visible(false);
            _processamentoInfracao.TipoMovimentoEmpresa.required(false);
            _processamentoInfracao.PessoaTituloEmpresa.visible(false);
            _processamentoInfracao.PessoaTituloEmpresa.required(false);
            LimparCampoEntity(_processamentoInfracao.PessoaTituloEmpresa);
            LimparCampoEntity(_processamentoInfracao.TipoMovimentoEmpresa);
        } else if (novoValor) {
            _processamentoInfracao.TipoMovimentoEmpresa.visible(true);
            _processamentoInfracao.TipoMovimentoEmpresa.required(true);
            _processamentoInfracao.PessoaTituloEmpresa.visible(true);
            _processamentoInfracao.PessoaTituloEmpresa.required(true);
        }
    });

    this.GerarOcorrencia.val.subscribe(function (novoValor) {
        if (novoValor) {
            $("#divTitulosMulta").hide();
            _processamentoInfracao.DataCompensacao.required(false);
            _processamentoInfracao.DataVencimento.required(false);
            _processamentoInfracao.TipoMovimentoTitulo.required(false);
            _processamentoInfracao.Valor.required(false);
            _processamentoInfracao.ValorAposVencimento.required(false);
            LimparCampoEntity(_processamentoInfracao.TipoMovimentoTitulo);
        }
        //else if (!novoValor) {
        //    $("#divTitulosMulta").show();
        //    _processamentoInfracao.DataCompensacao.required(true);
        //    _processamentoInfracao.DataVencimento.required(true);
        //    _processamentoInfracao.TipoMovimentoTitulo.required(true);
        //    _processamentoInfracao.Valor.required(true);
        //    _processamentoInfracao.ValorAposVencimento.required(true);
        //    LimparCampoEntity(_processamentoInfracao.TipoMovimentoTitulo);;
        //}
    });

    this.ResponsavelPagamento.val.subscribe(controlarResponsavelPagamento);
    this.ComissaoMotorista = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.DataVencimento.val.subscribe(ReplicarDataVencimento);
};

/*
 * Declaração das Funções de Inicialização
 */

function loadProcessamentoInfracao() {
    _processamentoInfracao = new ProcessamentoInfracao();
    KoBindings(_processamentoInfracao, "knockoutProcessamentoInfracao");

    new BuscarMotoristas(_processamentoInfracao.Motorista, null, null, null, null, EnumSituacaoColaborador.Todos);
    new BuscarClientes(_processamentoInfracao.Pessoa);
    new BuscarTipoMovimento(_processamentoInfracao.TipoMovimentoTitulo);
    new BuscarClientes(_processamentoInfracao.PessoaTituloEmpresa);
    new BuscarTipoMovimento(_processamentoInfracao.TipoMovimentoEmpresa);
    new BuscarFuncionario(_processamentoInfracao.Funcionario);

    carregarDespesaVeiculo("conteudoDespesaVeiculo");
    carregarLancamentoOcorrencia("conteudoOcorrencia", "modaisOcorrencia");

    LoadGridParcelasInfracao();

    _modalDespesaVeiculoInfracao = new bootstrap.Modal(document.getElementById("divModalDespesaVeiculo"), { backdrop: 'static', keyboard: true });
    _modalOcorrenciaInfracao = new bootstrap.Modal(document.getElementById("divModalOcorrencia"), { backdrop: 'static', keyboard: true });
}

function LoadGridParcelasInfracao() {

    var configuracaoEdicaoCelula = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.decimal,
        mask: "",
        maxlength: 10 + _CONFIGURACAO_TMS.NumeroCasasDecimaisQuantidadeProduto,
        numberMask: ConfigDecimal({ precision: _CONFIGURACAO_TMS.NumeroCasasDecimaisQuantidadeProduto }),
        allowZero: false,
        precision: 0,
        thousands: ".",
        type: 1
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Parcela", title: "Parcela", width: "13%" },
        { data: "DataVencimento", title: "Vencimento", width: "29%" },
        { data: "Valor", title: "Valor", width: "29%", className: "text-align-left", editableCell: configuracaoEdicaoCelula },
        { data: "ValorAposVencimento", title: "Valor Após Vencimento", width: "29%", className: "text-align-left", editableCell: configuracaoEdicaoCelula }
    ];

    var editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: SalvarNovosValores,
    };

    _gridParcelasInfracao = new BasicDataTable(_processamentoInfracao.GridParcelas.id, header, null, { column: 1, dir: orderDir.asc }, null, null, null, null, editarColuna);

    RecarregarGridParcelasInfracao();
}

function RecarregarGridParcelasInfracao() {
    _gridParcelasInfracao.CarregarGrid(_processamentoInfracao.Parcelas.val());
}

/*
 * Declaração das Funções
 */

function ReplicarDataVencimento(e, sender) {
    if (_processamentoInfracao.DataVencimento.val() !== null && _processamentoInfracao.DataVencimento.val() !== undefined && _processamentoInfracao.DataVencimento.val() !== "")
        _processamentoInfracao.DataVencimentoParcela.val(_processamentoInfracao.DataVencimento.val());
}

function GerarParcelasClick() {

    var quantidade = Globalize.parseInt(_processamentoInfracao.QuantidadeParcela.val());
    var tipoArredondamento = _processamentoInfracao.TipoArredondamentoParcela.val();
    var dataVencimento = moment(_processamentoInfracao.DataVencimentoParcela.val(), "DD/MM/YYYY");
    var intervaloDias = Globalize.parseInt(_processamentoInfracao.IntervaloDiasParcela.val());
    var valorTotal = Globalize.parseFloat(_processamentoInfracao.Valor.val());
    var valorTotalAposVencimento = Globalize.parseFloat(_processamentoInfracao.ValorAposVencimento.val());

    var parcelas = new Array();

    if (quantidade <= 0)
        quantidade = 1;

    if (isNaN(valorTotal))
        valorTotal = 0;

    if (isNaN(valorTotalAposVencimento))
        valorTotalAposVencimento = 0;

    if (!dataVencimento.isValid())
        dataVencimento = moment();

    var valorParcela = Globalize.parseFloat(Globalize.format(valorTotal / quantidade, "n2"));
    var valorDiferenca = Globalize.parseFloat(Globalize.format(valorTotal - (valorParcela * quantidade), "n2"));

    var valorAposVencimentoParcela = Globalize.parseFloat(Globalize.format(valorTotalAposVencimento / quantidade, "n2"));
    var valorAposVencimentoDiferenca = Globalize.parseFloat(Globalize.format(valorTotalAposVencimento - (valorAposVencimentoParcela * quantidade), "n2"));

    for (var i = 0; i < quantidade; i++) {

        var parcela = {
            Codigo: guid(),
            Parcela: (i + 1)
        };

        if (i > 0)
            dataVencimento = dataVencimento.add(intervaloDias, 'd');

        parcela["DataVencimento"] = dataVencimento.format("DD/MM/YYYY");

        if ((tipoArredondamento == EnumTipoArredondamento.PrimeiroItem && i == 0) || (tipoArredondamento == EnumTipoArredondamento.UltimoItem && i == (quantidade - 1))) {
            parcela["Valor"] = Globalize.format(valorParcela + valorDiferenca, "n2");
            parcela["ValorAposVencimento"] = Globalize.format(valorAposVencimentoParcela + valorAposVencimentoDiferenca, "n2");
        } else {
            parcela["Valor"] = Globalize.format(valorParcela, "n2");
            parcela["ValorAposVencimento"] = Globalize.format(valorAposVencimentoParcela, "n2");
        }

        parcelas.push(parcela);
    }

    _processamentoInfracao.Parcelas.Lista = parcelas;
    _gridParcelasInfracao.CarregarGrid(parcelas);
}

function adicionarProcessamentoInfracao() {
    if (!ValidarCamposObrigatorios(_processamentoInfracao) || !ValidarCamposObrigatorios(_comissaoMotorista))
        return exibirMensagemCamposObrigatorio();

    exibirConfirmacao("Confirmação", "Realmente deseja processar a ocorrência?", function () {
        if (_processamentoInfracao.Parcelas.Lista != null)
            _processamentoInfracao.Parcelas.val(JSON.stringify(_processamentoInfracao.Parcelas.Lista));
        else
            _processamentoInfracao.Parcelas.val("[]");

        _processamentoInfracao.Codigo.val(_infracao.Codigo.val());
        _processamentoInfracao.ComissaoMotorista.val(JSON.stringify(RetornarObjetoPesquisa(_comissaoMotorista)));

        executarReST("Infracao/AdicionarProcessamento", RetornarObjetoPesquisa(_processamentoInfracao), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    var mensagem = "Ocorrência processada com sucesso";
                    if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente && _processamentoInfracao.GerarOcorrencia.val())
                        mensagem = "Processada com sucesso, favor digite o lançamento de rateio e preencha a Ocorrência";
                    else if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente)
                        mensagem = "Processada com sucesso, favor digite o lançamento de rateio.";
                    else if (_processamentoInfracao.GerarOcorrencia.val())
                        mensagem = "Processada com sucesso, favor preencher a Ocorrência conforme marcado.";

                    exibirMensagem(tipoMensagem.ok, "Sucesso", mensagem);
                    _gridInfracao.CarregarGrid();

                    if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente) {
                        LimparCamposRateioDespesa();
                        _modalDespesaVeiculoInfracao.show();
                        _rateioDespesa.Infracao.val(retorno.Data.Codigo);
                        _rateioDespesa.NumeroDocumento.val(retorno.Data.NumeroDocumento);
                        _rateioDespesa.TipoDocumento.val(retorno.Data.TipoDocumento);
                        _rateioDespesa.Valor.val(retorno.Data.Valor);
                        _rateioDespesa.Colaborador.codEntity(retorno.Data.Colaborador.Codigo);
                        _rateioDespesa.Colaborador.val(retorno.Data.Colaborador.Descricao);
                    }

                    if (_processamentoInfracao.GerarOcorrencia.val())
                        abrirModalOcorrenciaProcessamentoInfracao();

                    limparCamposInfracao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function abrirModalOcorrenciaProcessamentoInfracao() {
    limparCamposOcorrencia();

    _ocorrencia.Infracao.val(_dadosInfracao.Numero.val());
    _ocorrencia.Infracao.codEntity(_dadosInfracao.Codigo.val());
    _ocorrencia.Infracao.visible(true);
    _ocorrencia.Infracao.enable(false);
    _ocorrencia.NaoLimparCarga.val(true);

    _modalOcorrenciaInfracao.show();
    visiblidadeValorOcorrencia();

    if (!string.IsNullOrWhiteSpace(_dadosInfracao.Observacao.val())) {
        _ocorrencia.Observacao.val(_dadosInfracao.Observacao.val());
        _ocorrencia.ObservacaoCTe.val(_dadosInfracao.Observacao.val());
    }
    else {
        _ocorrencia.Observacao.enable(true);
        _ocorrencia.ObservacaoCTe.enable(true);
    }
}

function controlarCamposProcessamentoInfracao() {
    var habilitarCampo = (_infracao.Situacao.val() === EnumSituacaoInfracao.AguardandoProcessamento);

    _processamentoInfracao.DataCompensacao.enable(habilitarCampo);
    _processamentoInfracao.DataVencimento.enable(habilitarCampo);
    _processamentoInfracao.Motorista.enable(habilitarCampo);
    _processamentoInfracao.Pessoa.enable(habilitarCampo);
    _processamentoInfracao.ResponsavelPagamento.enable(habilitarCampo);
    _processamentoInfracao.TipoMovimentoTitulo.enable(habilitarCampo);
    _processamentoInfracao.GerarTituloEmpresa.enable(habilitarCampo);
    _processamentoInfracao.TipoMovimentoEmpresa.enable(habilitarCampo);
    _processamentoInfracao.PessoaTituloEmpresa.enable(habilitarCampo);
    _processamentoInfracao.GerarParcelas.enable(habilitarCampo);
    _processamentoInfracao.DataVencimentoParcela.enable(habilitarCampo);
    _processamentoInfracao.IntervaloDiasParcela.enable(habilitarCampo);
    _processamentoInfracao.QuantidadeParcela.enable(habilitarCampo);
    _processamentoInfracao.TipoArredondamentoParcela.enable(habilitarCampo);
    _processamentoInfracao.Funcionario.enable(habilitarCampo);
    _processamentoInfracao.DescontarLancamentoAgregadoTerceiro.enable(habilitarCampo);
    _processamentoInfracao.TipoTitulo.enable(habilitarCampo);
    _processamentoInfracao.GerarOcorrencia.enable(habilitarCampo);

    _comissaoMotorista.LancarDescontoMotorista.enable(habilitarCampo);
    _comissaoMotorista.DescontoComissaoMotorista.enable(habilitarCampo);
    _comissaoMotorista.JustificativaDesconto.enable(habilitarCampo);
    _comissaoMotorista.ReduzirPercentualComissaoMotorista.enable(habilitarCampo);
    _comissaoMotorista.PercentualReducaoComissaoMotorista.enable(habilitarCampo);
}

function controlarResponsavelPagamento() {
    _processamentoInfracao.Motorista.visible(false);
    _processamentoInfracao.Motorista.required(false);
    _processamentoInfracao.Pessoa.visible(false);
    _processamentoInfracao.Pessoa.required(false);
    _processamentoInfracao.Funcionario.visible(false);
    _processamentoInfracao.Funcionario.required(false);
    _processamentoInfracao.TipoTitulo.visible(false);
    _processamentoInfracao.Pessoa.cssClass("col col-xs-8");
    _processamentoInfracao.ResponsavelPagamento.cssClass("col col-xs-4");

    LimparCampoEntity(_processamentoInfracao.Motorista);
    LimparCampoEntity(_processamentoInfracao.Pessoa);

    _processamentoInfracao.DescontarLancamentoAgregadoTerceiro.visible(false);
    if (_processamentoInfracao.ResponsavelPagamento.val() !== EnumResponsavelPagamentoInfracao.AgregadoTerceiro)
        _processamentoInfracao.DescontarLancamentoAgregadoTerceiro.val(false);

    if (_processamentoInfracao.ResponsavelPagamento.val() === EnumResponsavelPagamentoInfracao.Outro) {
        _processamentoInfracao.GerarOcorrencia.visible(true);
    } else {
        _processamentoInfracao.GerarOcorrencia.visible(false);
        _processamentoInfracao.GerarOcorrencia.val(false);
    }

    if (_processamentoInfracao.ResponsavelPagamento.val() === EnumResponsavelPagamentoInfracao.Condutor) {
        _processamentoInfracao.Motorista.visible(true);
        _processamentoInfracao.Motorista.required(true);
        _processamentoInfracao.GerarTituloEmpresa.visible(true);
        _processamentoInfracao.GerarTituloEmpresa.val(false);
        $("#lblTitle").text("Descontar do Condutor");

        if (_processamentoInfracao.Motorista.codEntity() === 0 && _dadosInfracao.Motorista.codEntity() !== 0) {
            _processamentoInfracao.Motorista.codEntity(_dadosInfracao.Motorista.codEntity());
            _processamentoInfracao.Motorista.val(_dadosInfracao.Motorista.val());
        }
    } else if (_processamentoInfracao.ResponsavelPagamento.val() === EnumResponsavelPagamentoInfracao.Funcionario) {
        _processamentoInfracao.Funcionario.visible(true);
        _processamentoInfracao.Funcionario.required(true);
        _processamentoInfracao.GerarTituloEmpresa.visible(true);
        _processamentoInfracao.GerarTituloEmpresa.val(false);
        $("#lblTitle").text("Descontar do Funcionário");
    } else {
        _processamentoInfracao.Pessoa.visible(true);
        _processamentoInfracao.Pessoa.required(true);
        _processamentoInfracao.GerarTituloEmpresa.visible(false);
        _processamentoInfracao.GerarTituloEmpresa.val(false);

        _processamentoInfracao.TipoMovimentoEmpresa.visible(false);
        _processamentoInfracao.TipoMovimentoEmpresa.required(false);
        _processamentoInfracao.PessoaTituloEmpresa.visible(false);
        _processamentoInfracao.PessoaTituloEmpresa.required(false);
        LimparCampoEntity(_processamentoInfracao.PessoaTituloEmpresa);
        LimparCampoEntity(_processamentoInfracao.TipoMovimentoEmpresa);
        if (_processamentoInfracao.ResponsavelPagamento.val() === EnumResponsavelPagamentoInfracao.Empresa)
            $("#lblTitle").text("Descontar da Empresa");
        else if (_processamentoInfracao.ResponsavelPagamento.val() === EnumResponsavelPagamentoInfracao.AgregadoTerceiro) {
            $("#lblTitle").text("Descontar do Agregado/Terceiro");
            _processamentoInfracao.DescontarLancamentoAgregadoTerceiro.visible(true);
        }
        else {
            $("#lblTitle").text("Descontar de Outro");
            _processamentoInfracao.TipoTitulo.visible(true);
            _processamentoInfracao.Pessoa.cssClass("col col-xs-6");
            _processamentoInfracao.ResponsavelPagamento.cssClass("col col-xs-3");
        }
    }

    controleCamposProcessamento();
}

function limparCamposProcessamentoInfracao() {
    LimparCampos(_processamentoInfracao);
    limparComissaoMotorista();
    RecarregarGridParcelasInfracao();
    controleCamposProcessamento();
}

function preencherProcessamentoInfracao(processamentoInfracao, comissaoMotorista) {
    if (processamentoInfracao)
        PreencherObjetoKnout(_processamentoInfracao, { Data: processamentoInfracao });

    if (comissaoMotorista)
        PreencherObjetoKnout(_comissaoMotorista, { Data: comissaoMotorista });

    RecarregarGridParcelasInfracao();
    controleCamposProcessamento();
}

function controleCamposProcessamento() {
    if ((_dadosInfracao.GerarMovimentoFichaMotorista.val() === true || _dadosInfracao.NaoGerarTituloFinanceiro.val() === true) && (_processamentoInfracao.ResponsavelPagamento.val() === EnumResponsavelPagamentoInfracao.Condutor || _processamentoInfracao.ResponsavelPagamento.val() === EnumResponsavelPagamentoInfracao.Empresa || _processamentoInfracao.ResponsavelPagamento.val() === EnumResponsavelPagamentoInfracao.Funcionario)) {
        if (_processamentoInfracao.ResponsavelPagamento.val() === EnumResponsavelPagamentoInfracao.Condutor) {
            if (_processamentoInfracao.Motorista.codEntity() === 0 && _dadosInfracao.Motorista.codEntity() !== 0) {
                _processamentoInfracao.Motorista.codEntity(_dadosInfracao.Motorista.codEntity());
                _processamentoInfracao.Motorista.val(_dadosInfracao.Motorista.val());
            }
        }
        else
            LimparCampoEntity(_processamentoInfracao.Motorista);

        _processamentoInfracao.DataCompensacao.text("Compensação: ");
        _processamentoInfracao.DataVencimento.text("Vencimento: ");
        _processamentoInfracao.Motorista.text("Motorista:");
        _processamentoInfracao.Funcionario.text("Funcionário:");
        _processamentoInfracao.Pessoa.text("Pessoa:");
        _processamentoInfracao.ResponsavelPagamento.text("Pago pelo(a): ");
        _processamentoInfracao.TipoMovimentoTitulo.text("Tipo Movimento p/ Título:");
        _processamentoInfracao.Valor.text("Valor até Vencimento:");
        _processamentoInfracao.ValorAposVencimento.text("Valor após Vencimento:");

        if (_dadosInfracao.NaoGerarTituloFinanceiro.val() === true) {
            $("#divTitulosMulta").show();
            _processamentoInfracao.DataCompensacao.required(false);
            _processamentoInfracao.DataVencimento.required(false);
            _processamentoInfracao.TipoMovimentoTitulo.required(false);
            _processamentoInfracao.Valor.required(false);
            _processamentoInfracao.ValorAposVencimento.required(false);
        } else {
            $("#divTitulosMulta").hide();
            _processamentoInfracao.DataCompensacao.required(false);
            _processamentoInfracao.DataVencimento.required(false);
            _processamentoInfracao.TipoMovimentoTitulo.required(false);
            _processamentoInfracao.Valor.required(false);
            _processamentoInfracao.ValorAposVencimento.required(false);
        }
    }
    else {
        $("#divTitulosMulta").show();
        _processamentoInfracao.DataCompensacao.required(true);
        _processamentoInfracao.DataVencimento.required(true);
        _processamentoInfracao.TipoMovimentoTitulo.required(true);
        _processamentoInfracao.Valor.required(true);
        _processamentoInfracao.ValorAposVencimento.required(true);

        _processamentoInfracao.DataCompensacao.text("*Compensação: ");
        _processamentoInfracao.DataVencimento.text("*Vencimento: ");
        _processamentoInfracao.Motorista.text("*Motorista:");
        _processamentoInfracao.Funcionario.text("*Funcionário:");
        _processamentoInfracao.Pessoa.text("*Pessoa:");
        _processamentoInfracao.ResponsavelPagamento.text("*Pago pelo(a): ");
        _processamentoInfracao.TipoMovimentoTitulo.text("*Tipo Movimento p/ Título:");
        _processamentoInfracao.Valor.text("*Valor até Vencimento:");
        _processamentoInfracao.ValorAposVencimento.text("*Valor após Vencimento:");
    }

    if (_processamentoInfracao.GerarOcorrencia.val()) {
        $("#divTitulosMulta").hide();
        _processamentoInfracao.DataCompensacao.required(false);
        _processamentoInfracao.DataVencimento.required(false);
        _processamentoInfracao.TipoMovimentoTitulo.required(false);
        _processamentoInfracao.Valor.required(false);
        _processamentoInfracao.ValorAposVencimento.required(false);
        LimparCampoEntity(_processamentoInfracao.TipoMovimentoTitulo);
    }
    //else if (!_processamentoInfracao.GerarOcorrencia.val()) {
    //    $("#divTitulosMulta").show();
    //    _processamentoInfracao.DataCompensacao.required(true);
    //    _processamentoInfracao.DataVencimento.required(true);
    //    _processamentoInfracao.TipoMovimentoTitulo.required(true);
    //    _processamentoInfracao.Valor.required(true);
    //    _processamentoInfracao.ValorAposVencimento.required(true);
    //    LimparCampoEntity(_processamentoInfracao.TipoMovimentoTitulo);;
    //}
}

function SalvarNovosValores(novoValor) {
    executarReST("Infracao/BuscarValorAtualizadoPorInfracao", { Codigo: _infracao.Codigo.val(), CodigoParcela: novoValor.Parcela, ValorAposVencimento: novoValor.ValorAposVencimento, Valor: novoValor.Valor }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            _processamentoInfracao.ValorAposVencimento.val(retorno.Data.ValorAposVencimento);
            _processamentoInfracao.Valor.val(retorno.Data.Valor);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}
