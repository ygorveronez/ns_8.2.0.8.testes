$(document).ready(function () {
    CarregarConsultaDeCTes("btnBuscarCTe", "btnBuscarCTe", 0, RetornoConsultaCTe, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
    CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);

    $("#txtDataPagamento").datepicker();
    $("#txtDataRecebimento").datepicker();
    $("#txtDataPagamento").mask("99/99/9999");
    $("#txtDataRecebimento").mask("99/99/9999");

    $("#txtoNumeroInicial").mask("9?9999", { placeholder: "      " });
    $("#txtoNumeroFinal").mask("9?9999", { placeholder: "      " });

    $("#txtValorFrete").priceFormat();
    $("#txtINSSSENAT").priceFormat();
    $("#txtSESTSENAT").priceFormat();
    $("#txtIR").priceFormat();
    $("#txtAdiantamento").priceFormat();
    $("#txtValorOutros").priceFormat();
    $("#txtValorPedagio").priceFormat();
    $("#txtSalarioMotorista").priceFormat();    

    $("#txtValorFrete").focusout(function () {
        AtualizarDeducaoDeValores();
        AtualizarSaldoPagar();
    });

    $("#txtINSSSENAT, #txtSESTSENAT, #txtIR, #txtAdiantamento, #txtValorOutros, #txtValorPedagio, #txtSalarioMotorista").focusout(function () {
        AtualizarSaldoPagar();
    });

    $("#selDeduzir").change(function () {
        AtualizarDeducaoDeValores();
        AtualizarSaldoPagar();
    });

    $("#txtCTe").keydown(function (e) {
        if (e.which !== 9 && e.which !== 16) {
            if (e.which === 8 || e.which === 46) {
                $(this).val("");
                $("body").data("cte", null);
            }
            e.preventDefault();
        }
    });

    $("#txtMotorista").keydown(function (e) {
        if (e.which !== 9 && e.which !== 16) {
            if (e.which === 8 || e.which === 46) {
                $(this).val("");
                $("body").data("motorista", null);
            }
            e.preventDefault();
        }
    });

    $("#txtVeiculo").keydown(function (e) {
        if (e.which !== 9 && e.which !== 16) {
            if (e.which === 8 || e.which === 46) {
                $(this).val("");
                $("body").data("veiculo", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#btnCancelar").click(function () {
        LimparCampos();
    });

    $("#btnSalvar").click(function () {
        Salvar();
    });

    LimparCampos();

    CarregarPagamento();
});

function CarregarPagamento() {

    var codigoPagamento = GetUrlParam("x");

    if (codigoPagamento !== null) {
        executarRest("/PagamentoMotorista/ObterDetalhesPagamento?callback=?", { CodigoPagamento: codigoPagamento }, function (r) {
            if (r.Sucesso) {
                $("body").data("codigo", r.Objeto.Codigo);
                $("body").data("cte", r.Objeto.CodigoCTe);
                $("body").data("motorista", r.Objeto.CodigoMotorista);
                $("#txtMotorista").val(r.Objeto.DescricaoMotorista);
                $("#txtValorFrete").val(r.Objeto.ValorFrete);
                $("#txtINSSSENAT").val(r.Objeto.ValorINSSSENAT);
                $("#txtSESTSENAT").val(r.Objeto.ValorSESTSENAT);
                $("#txtIR").val(r.Objeto.ValorImpostoRenda);
                $("#txtDataPagamento").val(r.Objeto.DataPagamento);
                $("#txtDataRecebimento").val(r.Objeto.DataRecebimento);
                $("#selDeduzir").val(r.Objeto.Deduzir);
                $("#txtObservacao").val(r.Objeto.Observacao);
                $("#txtAdiantamento").val(r.Objeto.ValorAdiantamento);
                $("#txtValorOutros").val(r.Objeto.ValorOutros);
                $("#txtValorPedagio").val(r.Objeto.ValorPedagio);
                $("#txtSalarioMotorista").val(r.Objeto.SalarioMotorista);
                $("#txtNumero").val(r.Objeto.Numero);
                $("#selStatus").val(r.Objeto.Status).change();

                $("body").data("ctes", r.Objeto.Ctes);
                RenderizarCtes();

                AtualizarSaldoPagar();
            } else {
                jAlert(r.Erro + "<br /><br />Não foi possível carregar pagamento.", "Atenção!", function () {
                });
            }
        });
    }
}

function GetUrlParam(name) {
    var url = window.location.search.replace("?", "");
    var itens = url.split("&");
    for (n in itens) {
        if (itens[n].match(name)) {
            return itens[n].replace(name + "=", "");
        }
    }
    return null;
}

function RetornoConsultaMotorista(motorista) {
    $("body").data("motorista", motorista.Codigo);
    $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
    $("#txtSalarioMotorista").val(motorista.Salario);
}

function RetornoConsultaCTe(cte) {
    if (cte.Codigo > 0) {
        $("#txtCTe").val(cte.Numero + " - " + cte.Serie);
        $("#hddDescricaoCte").val(cte.Numero + " - " + cte.Serie);
        $("#hddCodigoCte").val(cte.Codigo);

        if (ValidarCamposCte()) {
            var ctes = $("body").data("ctes") === null ? new Array() : $("body").data("ctes");

            var codigoCte = Globalize.parseInt($("#hddCodigoCte").val());
            var descricaoCte = $("#hddDescricaoCte").val();

            var _cte = {
                CodigoCte: codigoCte,
                DescricaoCte: descricaoCte,
                Excluir: false
            };

            ctes.push(_cte);

            $("body").data("ctes", ctes);

            $("#txtCTe").val("");
            $("#hddCodigoCte").val("0");
            $("#hddDescricaoCte").val("");

            RenderizarCtes();
        }
    }
}

function AtualizarDeducaoDeValores() {
    if ($("#selDeduzir").val() === "1") {
        $("#txtINSSSENAT").val(CalculoINSS());
        $("#txtSESTSENAT").val(CalculoSESTSENAT());
        $("#txtIR").val(CalculoIR());
    } else {
        $("#txtINSSSENAT").val("0,00");
        $("#txtSESTSENAT").val("0,00");
    }
}

function AtualizarSaldoPagar() {
    var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());
    var valorINSS = Globalize.parseFloat($("#txtINSSSENAT").val());
    var valorSEST = Globalize.parseFloat($("#txtSESTSENAT").val());
    var valorIR = Globalize.parseFloat($("#txtIR").val());
    var valorAdiantamento = Globalize.parseFloat($("#txtAdiantamento").val());
    var valorOutros = Globalize.parseFloat($("#txtValorOutros").val());
    var valorPedagio = Globalize.parseFloat($("#txtValorPedagio").val());
    var salarioMotorista = Globalize.parseFloat($("#txtSalarioMotorista").val());

    if ($("#selDeduzir").val() === "1") {
        $("#txtSaldoPagar").val(Globalize.format(valorFrete + valorPedagio + salarioMotorista - valorINSS - valorSEST - valorIR - valorAdiantamento - valorOutros, "n2"));
    } else {
        $("#txtSaldoPagar").val(Globalize.format(valorFrete + valorPedagio + salarioMotorista, "n2"));
    }
}

function LimparCampos() {
    $("body").data("codigo", null);
    $("#txtCTe").val('');
    $("body").data("cte", null);
    $("body").data("ctes", null);

    $("#txtMotorista").val('');
    $("body").data("motorista", null);

    $("#txtDataPagamento").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#txtDataRecebimento").val('');
    $("#txtValorFrete").val('0,00');
    $("#txtINSSSENAT").val('0,00');
    $("#txtSESTSENAT").val('0,00');
    $("#txtAdiantamento").val("0,00");
    $("#txtValorOutros").val("0,00");
    $("#txtIR").val('0,00');
    $("#txtSaldoPagar").val("0,00");
    $("#txtValorPedagio").val("0,00");
    $("#txtSalarioMotorista").val("0,00");
    $("#selDeduzir").val($("#selDeduzir option:first").val());
    $("#txtObservacao").val('');
    $("#selStatus").val($("#selStatus option:first").val()).change();
    $("#txtNumero").val('Automático');

    $("#txtVeiculo").val('');
    $("body").data("veiculo", null);

    RenderizarCtes();
}

function ValidarCampos() {
    var codigoCTe = $("body").data("cte");
    var codigoMotorista = $("body").data("motorista");
    var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());
    var dataPagamento = $("#txtDataPagamento").val();
    var dataRecebimento = $("#txtDataRecebimento").val();

    var valido = true;

    CampoSemErro("#txtCTe");
    if ($("body").data("ctes") === null || $("body").data("ctes").length === 0) {
        ExibirMensagemAlerta("Não foram adicionados CT-es!", "Atenção!", "messages-placeholder");
        CampoComErro("#txtCTe");
        valido = false;
    }
    else {
        var ctes = $("body").data("ctes") === null ? new Array() : $("body").data("ctes");
        var cteAdicionado = false;
        for (var i = 0; i < ctes.length; i++) {
            if (!ctes[i].Excluir) {
                cteAdicionado = true;
            }
        }
        if (!cteAdicionado) {
            CampoComErro("#txtCTe");
            ExibirMensagemAlerta("Não foram adicionados CT-es!", "Atenção!", "messages-placeholder");
            valido = false;
        }
    }

    if (isNaN(codigoMotorista) || codigoMotorista <= 0) {
        CampoComErro("#txtMotorista");
        valido = false;
    } else {
        CampoSemErro("#txtMotorista");
    }

    if (isNaN(valorFrete) || valorFrete <= 0) {
        CampoComErro("#txtValorFrete");
        valido = false;
    } else {
        CampoSemErro("#txtValorFrete");
    }

    if (dataPagamento === null || dataPagamento === "") {
        CampoComErro("#txtDataPagamento");
        valido = false;
    } else {
        CampoSemErro("#txtDataPagamento");
    }

    if (dataRecebimento === null || dataRecebimento === "") {
        CampoComErro("#txtDataRecebimento");
        valido = false;
    } else {
        CampoSemErro("#txtDataRecebimento");
    }

    return valido;
}

function Salvar() {
    if (ValidarCampos()) {
        var dados = {
            Codigo: $("body").data("codigo"),
            CodigoCTe: $("body").data("cte"),
            CodigoMotorista: $("body").data("motorista"),
            ValorFrete: $("#txtValorFrete").val(),
            INSSSENAT: $("#txtINSSSENAT").val(),
            SESTSENAT: $("#txtSESTSENAT").val(),
            IR: $("#txtIR").val(),
            DataPagamento: $("#txtDataPagamento").val(),
            DataRecebimento: $("#txtDataRecebimento").val(),
            Adiantamento: $("#txtAdiantamento").val(),
            ValorOutros: $("#txtValorOutros").val(),
            ValorPedagio: $("#txtValorPedagio").val(),
            SalarioMotorista: $("#txtSalarioMotorista").val(),
            Deduzir: $("#selDeduzir").val(),
            Observacao: $("#txtObservacao").val(),
            Status: $("#selStatus").val(),
            Veiculo: $("body").data("veiculo"),
            Ctes: JSON.stringify($("body").data("ctes"))
        };

        executarRest("/PagamentoMotorista/Salvar?callback=?", dados, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
                LimparCampos();
                CarregarPagamentos();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção!");
            }
        });
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
    }
}

function Editar(pagamento) {
    executarRest("/PagamentoMotorista/ObterDetalhes?callback=?", { Codigo: pagamento.data.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("codigo", r.Objeto.Codigo);
            $("body").data("cte", r.Objeto.CodigoCTe);
            //$("#txtCTe").val(r.Objeto.DescricaoCTe);
            $("body").data("motorista", r.Objeto.CodigoMotorista);
            $("#txtMotorista").val(r.Objeto.DescricaoMotorista);
            $("#txtValorFrete").val(r.Objeto.ValorFrete);
            $("#txtINSSSENAT").val(r.Objeto.ValorINSSSENAT);
            $("#txtSESTSENAT").val(r.Objeto.ValorSESTSENAT);
            $("#txtIR").val(r.Objeto.ValorImpostoRenda);
            $("#txtDataPagamento").val(r.Objeto.DataPagamento);
            $("#txtDataRecebimento").val(r.Objeto.DataRecebimento);
            $("#selDeduzir").val(r.Objeto.Deduzir);
            $("#txtObservacao").val(r.Objeto.Observacao);
            $("#txtAdiantamento").val(r.Objeto.ValorAdiantamento);
            $("#txtValorOutros").val(r.Objeto.ValorOutros);
            $("#txtValorPedagio").val(r.Objeto.ValorPedagio);
            $("#txtSalarioMotorista").val(r.Objeto.SalarioMotorista);
            $("#txtNumero").val(r.Objeto.Numero);
            $("#selStatus").val(r.Objeto.Status).change();

            $("#txtVeiculo").val(r.Objeto.Veiculo);
            $("body").data("veiculo", r.Objeto.CodigoVeiculo);

            $("body").data("ctes", r.Objeto.Ctes);
            RenderizarCtes();

            AtualizarSaldoPagar();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function CalculoIR() {
    var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());

    if (valorFrete === 0) return "0,00"; // Nao calcula quando o valor do frete esta zerado

    // Verifica se ha alguma configuracao
    if (("IR" in impostosDaEmpresa) && impostosDaEmpresa.IR.length > 0) {
        var valorIR = 0;
        var valorICMSTotal = Globalize.parseFloat($("#txtINSSSENAT").val());
        var baseCalculoIR = (valorFrete * (impostosDaEmpresa.PercentualBCIR / 100)) - valorICMSTotal;

        // Percorre configuracoes
        for (var config in impostosDaEmpresa.IR) {
            config = impostosDaEmpresa.IR[config];

            // Verifica intervalo
            if ((baseCalculoIR >= config.ValorInicial) && (baseCalculoIR <= config.ValorFinal) && config.PercentualAplicar > 0) {
                valorIR = (baseCalculoIR * (config.PercentualAplicar / 100)) - config.ValorDeduzir;

                return Globalize.format(valorIR, "n2");
            }
        }
    }

    // Quando nao ha configuracao, seta o valor padrao
    return "0,00";
}

function CalculoSESTSENAT() {
    var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());

    if (valorFrete === 0) return "0,00"; // Nao calcula quando o valor do frete esta zerado

    // Verifica se ha alguma configuracao
    if (("AliquotaSENAT" in impostosDaEmpresa && "AliquotaSEST" in impostosDaEmpresa) && (impostosDaEmpresa.AliquotaSENAT > 0 || impostosDaEmpresa.AliquotaSEST > 0)) {
        var valorSest = (valorFrete * impostosDaEmpresa.PercentualBCINSS / 100) * (impostosDaEmpresa.AliquotaSENAT / 100);
        var valorSenat = (valorFrete * impostosDaEmpresa.PercentualBCINSS / 100) * (impostosDaEmpresa.AliquotaSEST / 100);

        return Globalize.format(valorSest + valorSenat, "n2");
    } else {
        // Quando nao ha configuracao, seta o valor padrao
        return Globalize.format(((valorFrete * 0.2) * 0.025), "n2");
    }
}

function CalculoINSS() {
    var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());

    if (valorFrete === 0) return "0,00"; // Nao calcula quando o valor do frete esta zerado

    // Verifica se ha alguma configuracao
    if (("INSS" in impostosDaEmpresa) && impostosDaEmpresa.INSS.length > 0) {
        var valorINSS = 0;

        // Percorre configuracoes
        for (var config in impostosDaEmpresa.INSS) {
            config = impostosDaEmpresa.INSS[config];

            // Verifica intervalo
            if ((valorFrete >= config.ValorInicial) && (valorFrete <= config.ValorFinal) && config.PercentualAplicar > 0) {
                valorINSS = (valorFrete * (impostosDaEmpresa.PercentualBCINSS / 100)) * (config.PercentualAplicar / 100); // /100 para transformar o valor em %

                if (impostosDaEmpresa.ValorTetoRetencaoINSS > 0 && valorINSS > impostosDaEmpresa.ValorTetoRetencaoINSS) //Se o valor teto já foi atingido
                    valorINSS = impostosDaEmpresa.ValorTetoRetencaoINSS;

                return Globalize.format(valorINSS, "n2");
            }
        }

        // Quando nao ha configuracao, seta o valor padrao
        return "0,00";
    } else {
        // Quando nao ha configuracao, seta o valor padrao
        return Globalize.format(((valorFrete * 0.2) * 0.11), "n2");
    }
}

function RetornoConsultaVeiculos(veiculo) {
    $("body").data("veiculo", veiculo.Codigo);
    $("#txtVeiculo").val(veiculo.Placa);
}

// Objeto global para gravar as informacoes para calculo dos valores
var impostosDaEmpresa = {};

$(document).ready(function () {
    CarregarConsultaDeCTes("btnBuscarCTeFiltro", "btnBuscarCTeFiltro", 0, RetornoConsultaCTeFiltro, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotoristaFiltro", "btnBuscarMotoristaFiltro", RetornoConsultaMotoristaFiltro, true, false);

    $("#txtDataInicial").mask("99/99/9999");
    $("#txtDataInicial").datepicker();

    $("#txtDataFinal").mask("99/99/9999");
    $("#txtDataFinal").datepicker();

    $("#txtCTeFiltro").keydown(function (e) {
        if (e.which !== 9 && e.which !== 16) {
            if (e.which === 8 || e.which === 46) {
                $(this).val("");
                $("body").data("cteFiltro", null);
            }
            e.preventDefault();
        }
    });

    $("#txtMotoristaFiltro").keydown(function (e) {
        if (e.which !== 9 && e.which !== 16) {
            if (e.which === 8 || e.which === 46) {
                $(this).val("");
                $("body").data("motoristaFiltro", null);
            }
            e.preventDefault();
        }
    });

    $("#btnConsultarPagamentos").click(function () {
        CarregarPagamentos();
    });

    CarregarPagamentos();

    // Busca informacoes da empresa
    executarRest("/ImpostoContratoFrete/ObterImpostosDaEmpresa?", {}, function (r) {
        impostosDaEmpresa = r.Objeto;

        if (typeof impostosDaEmpresa === "boolean") {
            impostosDaEmpresa = {};
        }
    });
});

function RetornoConsultaMotoristaFiltro(motorista) {
    $("body").data("motoristaFiltro", motorista.Codigo);
    $("#txtMotoristaFiltro").val(motorista.CPFCNPJ + " - " + motorista.Nome);
}

function RetornoConsultaCTeFiltro(cte) {
    $("#txtCTeFiltro").val(cte.Numero + " - " + cte.Serie);
    $("body").data("cteFiltro", cte.Codigo);
}

function CarregarPagamentos() {

    var dados = {
        inicioRegistros: 0,
        DataInicial: $("#txtDataInicial").val(),
        DataFinal: $("#txtDataFinal").val(),
        CodigoMotorista: $("body").data("motoristaFiltro"),
        CodigoCTe: $("body").data("cteFiltro"),
        Status: $("#selStatusFiltro").val(),
        NumeroInicial: $("#txtoNumeroInicial").val(),
        NumeroFinal: $("#txtoNumeroFinal").val()
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Editar", Evento: Editar });
    opcoes.push({ Descricao: "Gerar Recibo", Evento: GerarRecibo });
    opcoes.push({ Descricao: "Gerar Contrato", Evento: GerarContrato });

    CriarGridView("/PagamentoMotorista/Consultar?callback=?", dados, "tbl_pagamentos_table", "tbl_pagamentos", "tbl_paginacao_pagamentos", opcoes, [0], null);
}

function GerarRecibo(pagamento) {
    executarDownload("/PagamentoMotorista/DownloadRecibo", { Codigo: pagamento.data.Codigo });
}

function GerarContrato(pagamento) {
    executarDownload("/PagamentoMotorista/DownloadContrato", { Codigo: pagamento.data.Codigo });
}