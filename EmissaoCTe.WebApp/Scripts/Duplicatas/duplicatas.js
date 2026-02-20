$(document).ready(function () {
    CarregarConsultaDuplicatas("default-search", "default-search", "", RetornoConsultaDuplicatas, true, false);
    CarregarConsultadeClientes("btnBuscarPessoa", "btnBuscarPessoa", RetornoConsultaPessoa, true, false, "");
    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, true, false);
    CarregarConsultaDeVeiculos("btnBuscarVeiculo1", "btnBuscarVeiculo1", RetornoConsultaVeiculos1, true, false);
    CarregarConsultaDeVeiculos("btnBuscarVeiculo2", "btnBuscarVeiculo2", RetornoConsultaVeiculos2, true, false);
    CarregarConsultaDeVeiculos("btnBuscarVeiculo3", "btnBuscarVeiculo3", RetornoConsultaVeiculos3, true, false);
    CarregarConsultaDePlanosDeContas("btnBuscarPlanoDeConta", "btnBuscarPlanoDeConta", "A", "A", RetornoConsultaPlanoDeConta, true, false);
    CarregarConsultaDeCtesDuplicatas("btnBuscarCte", "btnBuscarCte", RetornoConsultaCTe, true, false, "hddURLBuscaCTe");
    FormatarCampoDate("txtDataLcto");
    FormatarCampoDate("txtDataDocumento");
    $("#btnEDICaterpillar").hide();
    $("#txtValor").priceFormat();
    $("#txtAcrescimo").priceFormat();
    $("#txtDesconto").priceFormat();
    $("#txtValorParcela").priceFormat();

    $("#txtPessoa").keydown(function (e) {
        if (PermiteAlterarPessoa()) {
            if (e.which != 9 && e.which != 16) {
                if (e.which == 8 || e.which == 46) {
                    $(this).val("");
                    $("body").data("pessoa", null);
                }
                //e.preventDefault();
            }
        } else {
            e.preventDefault();
            ExibirMensagemAlerta("Para alterar o tomador é necessário remover o(s) CTe(s)!", "Atenção!");
        };
    });

    $("#txtPessoa").focusout(function () {
        if (PermiteAlterarPessoa()) {
            BuscarPessoaFiltro();
        }
    });


    $("#txtMotorista").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("codigoMotorista", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtVeiculo1").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("veiculo1", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtVeiculo2").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("veiculo2", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtVeiculo3").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("veiculo3", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtPlanoDeConta").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("planoDeConta", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#selTipo").change(function () {
        BloquearAbaCTes();
    });

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnCancelar").click(function () {
        LimparCamposDuplicata();
        ControlarCampos();
    });

    $("#btnVisualizar").click(function () {
        Visualizar();
    });

    $("#btnEDICaterpillar").click(function () {
        BaixarEDICaterpillar();
    });

    LimparCamposDuplicata();
    BuscarConfiguracoesEmpresa();
});
var ConfiguracoesEmpresa = {};
function RetornoConsultaDuplicatas(duplicata) {
    executarRest("/Duplicatas/ObterDetalhes?callback=?", { Codigo: duplicata.Codigo }, function (r) {
        if (r.Sucesso) {
            RenderizarDuplicata(r.Objeto);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function BuscarPessoaFiltro() {
    if ($("body").data("pessoa") != $("#txtPessoa").val()) {
        var cpfCnpj = $("#txtPessoa").val().replace(/[^0-9]/g, '');
        if (cpfCnpj != "") {
            if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto != null) {
                            $("body").data("pessoa", r.Objeto.CPF_CNPJ);
                            $("#txtPessoa").val(r.Objeto.CPF_CNPJ + " - " + r.Objeto.Nome);
                            $("#hddURLBuscaCTe").val("/Duplicatas/ConsultarCtesSemDuplicata?callback=?&Pessoa=" + r.Objeto.CPF_CNPJ);
                        } else {
                            $("#txtPessoa").val("");
                            $("body").data("pessoa", null);
                            jAlert("Cliente não encontrado.", "Atenção!");
                        }
                    } else {
                        $("#txtPessoa").val("");
                        $("body").data("pessoa", null);
                        jAlert(r.Erro, "Erro!");
                    }
                });
            } else {
                $("#txtPessoa").val("");
                $("body").data("pessoa", null);
                jAlert("O CPF/CNPJ digitado é inválido.", "Atenção!");
            }
        } else {
            $("#txtPessoa").val("");
            $("body").data("pessoa", null);
        }
    }
}

function RenderizarDuplicata(duplicata) {
    LimparCamposDuplicata();

    $("body").data("duplicata", duplicata);

    $("#txtNumero").val(duplicata.Numero);
    $("#txtFuncionario").val(duplicata.Funcionario);
    $("#selTipo").val(duplicata.Tipo).change();
    $("#txtDataLcto").val(duplicata.DataLancamento);
    $("#selStatus").val(duplicata.Status).change();
    $("#txtDocumento").val(duplicata.Documento).change();
    $("#txtDataDocumento").val(duplicata.DataDocumento);
    $("#txtValor").val(duplicata.Valor);
    $("#txtAcrescimo").val(duplicata.Acrescimo);
    $("#txtDesconto").val(duplicata.Desconto);
    $("#txtPessoa").val(duplicata.Pessoa);
    $("body").data("pessoa", duplicata.CPFCNPJPessoa);
    $("#hddURLBuscaCTe").val("/Duplicatas/ConsultarCtesSemDuplicata?callback=?&Pessoa=" + duplicata.CPFCNPJPessoa);
    $("#txtMotorista").val(duplicata.Motorista);
    $("body").data("codigoMotorista", duplicata.CodigoMotorista);
    $("#txtVeiculo1").val(duplicata.Veiculo1);
    $("body").data("veiculo1", duplicata.CodigoVeiculo1);
    $("#txtVeiculo2").val(duplicata.Veiculo2);
    $("body").data("veiculo2", duplicata.CodigoVeiculo2);
    $("#txtVeiculo3").val(duplicata.Veiculo3);
    $("body").data("veiculo3", duplicata.CodigoVeiculo3);
    $("#txtPlanoDeConta").val(duplicata.PlanoDeConta);
    $("body").data("planoDeConta", duplicata.CodigoPlanoConta);
    $("#txtObservacao").val(duplicata.Observacao);

    $("#hddCodigoDocumentoEntrada").val(duplicata.DocumentoEntrada);

    $("body").data("parcelas", duplicata.Parcelas);
    RenderizarParcelas();
    BuscarDadosParcelas()

    $("body").data("ctes", duplicata.Ctes);
    RenderizarCtes();

    RenderizarAdicionais(duplicata);

    ControlarCampos()

    if (duplicata.Ctes.length > 0)
        document.getElementById("selTipo").disabled = true;
}

function LimparCamposDuplicata() {
    $("body").data("duplicata", null);
    $("#hddURLBuscaCTe").val("/Duplicatas/ConsultarCtesSemDuplicata?callback=?&Pessoa=0")
    $("#txtNumero").val('Automático');
    $("#txtFuncionario").val('Automático');
    $("#selTipo").val($("#selTipo option:first").val()).change();
    $("#txtDataLcto").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#selStatus").val($("#selStatus option:first").val()).change();
    $("#txtDocumento").val('');
    $("#txtDataDocumento").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#txtValor").val('0,00');
    $("#txtAcrescimo").val('0,00');
    $("#txtDesconto").val('0,00');
    $("#txtPessoa").val('');
    $("body").data("pessoa", null);
    $("#txtMotorista").val('');
    $("body").data("codigoMotorista", null);
    $("#txtVeiculo1").val('');
    $("body").data("veiculo1", null);
    $("#txtVeiculo2").val('');
    $("body").data("veiculo2", null);
    $("#txtVeiculo3").val('');
    $("body").data("veiculo3", null);
    $("#txtPlanoDeConta").val('');
    $("body").data("planoDeConta", null);
    $("#txtObservacao").val('');

    $("#hddCodigoDocumentoEntrada").val('0');

    LimparCamposParcelas();
    $("body").data("parcelas", null);
    RenderizarParcelas();

    LimparCamposCtes();
    $("body").data("ctes", null);
    RenderizarCtes();
    LimparCamposAdicionais();

    $("#divAlterarParcela").hide();
}

function RetornoConsultaPessoa(cliente) {
    if (PermiteAlterarPessoa()) {
        $("body").data("pessoa", cliente.CPFCNPJ);
        $("#txtPessoa").val(cliente.CPFCNPJ + " - " + cliente.Nome);
        $("#hddURLBuscaCTe").val("/Duplicatas/ConsultarCtesSemDuplicata?callback=?&Pessoa=" + cliente.CPFCNPJ);
    } else ExibirMensagemAlerta("Para alterar o tomador é necessário remover o(s) CTe(s)!", "Atenção!");
}

function RetornoConsultaMotorista(motorista) {
    $("body").data("codigoMotorista", motorista.Codigo);
    $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
}

function RetornoConsultaVeiculos1(veiculo) {
    $("body").data("veiculo1", veiculo.Codigo);
    $("#txtVeiculo1").val(veiculo.Placa);

    if (veiculo.Placa != "") {
        executarRest("/Veiculo/BuscarPorPlaca?callback=?", { Placa: veiculo.Placa }, function (r) {
            if (r.Sucesso) {
                if (r.Objeto.VeiculosVinculados != null) {
                    if (r.Objeto.VeiculosVinculados.length > 0) {
                        placasVinculadas = "";
                        for (var i = 0; i < r.Objeto.VeiculosVinculados.length; i++) {

                            if (i == 0) {
                                $("body").data("veiculo2", r.Objeto.VeiculosVinculados[i].Codigo);
                                $("#txtVeiculo2").val(r.Objeto.VeiculosVinculados[i].Placa);
                            }
                            else if (i == 1) {
                                $("body").data("veiculo3", r.Objeto.VeiculosVinculados[i].Codigo);
                                $("#txtVeiculo3").val(r.Objeto.VeiculosVinculados[i].Placa);
                            }
                        }
                    }
                }

                if (r.Objeto.CodigoMotorista > 0 && $("#txtMotorista").val() == "") {
                    $("body").data("codigoMotorista", r.Objeto.CodigoMotorista);
                    $("#txtMotorista").val(r.Objeto.CPFMotorista + " - " + r.Objeto.NomeMotorista);
                }

            } else {
                jAlert(r.Erro, "Atenção");
            }
        });
    }
}

function RetornoConsultaVeiculos2(veiculo) {
    $("body").data("veiculo2", veiculo.Codigo);
    $("#txtVeiculo2").val(veiculo.Placa);
}

function RetornoConsultaVeiculos3(veiculo) {
    $("body").data("veiculo3", veiculo.Codigo);
    $("#txtVeiculo3").val(veiculo.Placa);
}

function RetornoConsultaPlanoDeConta(plano) {
    $("body").data("planoDeConta", plano.Codigo);
    $("#txtPlanoDeConta").val(plano.Conta + " - " + plano.Descricao);
}

function ValidarCampos() {
    var numero = $("#txtNumero").val();
    var dataLancamento = $("#txtDataLcto").val();
    var documento = $("#txtDocumento").val();
    var dataDocumento = $("#txtDataDocumento").val();
    var valor = Globalize.parseFloat($("#txtValor").val());
    var desconto = Globalize.parseFloat($("#txtDesconto").val());
    var acrescimo = Globalize.parseFloat($("#txtAcrescimo").val());
    var valorTotal = valor - desconto + acrescimo;
    var pessoa = $("body").data("pessoa");
    var planoDeConta = $("body").data("planoDeConta");
    var valido = true;

    if (dataLancamento == null || dataLancamento == "") {
        CampoComErro("#txtDataLcto");
        valido = false;
    } else {
        CampoSemErro("#txtDataLcto");
    }

    if (documento == null || documento == "") {
        CampoComErro("#txtDocumento");
        valido = false;
    } else {
        CampoSemErro("#txtDocumento");
    }

    if (dataDocumento == null || dataDocumento == "") {
        CampoComErro("#txtDataDocumento");
        valido = false;
    } else {
        CampoSemErro("#txtDataDocumento");
    }

    if (pessoa == null || pessoa == "") {
        CampoComErro("#txtPessoa");
        valido = false;
    } else {
        CampoSemErro("#txtPessoa");
    }

    if (planoDeConta == null || planoDeConta == "") {
        CampoComErro("#txtPlanoDeConta");
        valido = false;
    } else {
        CampoSemErro("#txtPlanoDeConta");
    }

    if (isNaN(valorTotal) || valorTotal <= 0) {
        CampoComErro("#txtValor");
        valido = false;
    } else {
        CampoSemErro("#txtValor");
    }

    if (!valido)
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!");

    return valido;
}

function ValidarParcelas() {
    var parcelas = $("body").data("parcelas") == null ? new Array() : $("body").data("parcelas");

    var valor = Globalize.parseFloat($("#txtValor").val());
    var desconto = Globalize.parseFloat($("#txtDesconto").val());
    var acrescimo = Globalize.parseFloat($("#txtAcrescimo").val());
    var valorTotal = valor - desconto + acrescimo;
    var valorParcelas = 0;

    var valido = true;
    var mensagem = "<br/>";

    if ($("body").data("parcelas") == null || $("body").data("parcelas").length == 0) {
        mensagem += "Não foram geradas parcelas.<br/>"
        valido = false;
    }

    for (var i = 0; i < parcelas.length; i++) {
        valorParcelas = valorParcelas + parcelas[i].Valor;
    }

    var diferencaValor = valorTotal - valorParcelas;
    if (diferencaValor > 0.05 || diferencaValor < -0.05) {
        mensagem += "Somatório das parcelas não fecha com o valor total da duplicata.<br/>"
        valido = false;
    }

    if (!valido) {
        ExibirMensagemAlerta(mensagem, "Atenção!");
        return false;
    } else {
        return true;
    }
}

function Salvar() {
    if (!ValidarCampos() || !ValidarParcelas() || VerificaParcelaPaga())
        return;

    var duplicata = {
        Codigo: $("body").data("duplicata") != null ? $("body").data("duplicata").Codigo : 0,
        Numero: $("#txtNumero").val(),
        Tipo: $("#selTipo").val(),
        DataLancamento: $("#txtDataLcto").val(),
        Status: $("#selStatus").val(),
        Documento: $("#txtDocumento").val(),
        DataDocumento: $("#txtDataDocumento").val(),
        Valor: $("#txtValor").val(),
        Acrescimo: $("#txtAcrescimo").val(),
        Desconto: $("#txtDesconto").val(),
        Pessoa: $("body").data("pessoa"),
        Motorista: $("body").data("codigoMotorista"),
        Veiculo1: $("body").data("veiculo1"),
        Veiculo2: $("body").data("veiculo2"),
        Veiculo3: $("body").data("veiculo3"),
        PlanoDeConta: $("body").data("planoDeConta"),
        Observacao: $("#txtObservacao").val(),
        Parcelas: JSON.stringify($("body").data("parcelas")),
        Ctes: JSON.stringify($("body").data("ctes")),
        DadosBancarios: $("#txtDadosBancarios").val(),
        Embarcador: $("#txtEmbarcador").data("Codigo"),
        AdicionaisCidadeOrigem: $("#selAdicionaisCidadeOrigem").val(),
        AdicionaisCidadeDestino: $("#selAdicionaisCidadeDestino").val(),
        AdicionaisPeso: $("#txtAdicionaisPeso").val(),
        AdicionaisVolumes: $("#txtAdicionaisVolumes").val(),
        TipoVeiculo: $("#txtTipoVeiculo").data("Codigo")
    };

    executarRest("/Duplicatas/Salvar?callback=?", duplicata, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
            LimparCamposDuplicata();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function ControlarCampos() {
    var desabilitar = VerificaParcelaPaga();

    if (!desabilitar) {
        desabilitar = ($("#selStatus").val() == "I")
        if (desabilitar)
            ExibirMensagemAlerta("Duplicata foi inativada, impossível alterar!", "Atenção!");
    };

    if (!desabilitar) {
        desabilitar = ($("#hddCodigoDocumentoEntrada").val() > "0")
        if (desabilitar)
            ExibirMensagemAlerta("Duplicata pertence a um Documento de Entrada, impossível alterar!", "Atenção!");
    };

    document.getElementById("selTipo").disabled = desabilitar;
    document.getElementById("txtDataLcto").disabled = desabilitar;
    document.getElementById("selStatus").disabled = desabilitar;
    document.getElementById("txtDocumento").disabled = desabilitar;
    document.getElementById("txtDataDocumento").disabled = desabilitar;
    document.getElementById("txtValor").disabled = desabilitar;
    document.getElementById("txtAcrescimo").disabled = desabilitar;
    document.getElementById("txtDesconto").disabled = desabilitar;
    document.getElementById("txtPessoa").disabled = desabilitar;
    document.getElementById("btnBuscarPessoa").disabled = desabilitar;
    document.getElementById("txtMotorista").disabled = desabilitar;
    document.getElementById("btnBuscarMotorista").disabled = desabilitar;
    document.getElementById("txtVeiculo1").disabled = desabilitar;
    document.getElementById("btnBuscarVeiculo1").disabled = desabilitar;
    document.getElementById("txtVeiculo2").disabled = desabilitar;
    document.getElementById("btnBuscarVeiculo2").disabled = desabilitar;
    document.getElementById("txtVeiculo3").disabled = desabilitar;
    document.getElementById("btnBuscarVeiculo3").disabled = desabilitar;
    document.getElementById("txtPlanoDeConta").disabled = desabilitar;
    document.getElementById("btnBuscarPlanoDeConta").disabled = desabilitar;
    document.getElementById("txtObservacao").disabled = desabilitar;

    document.getElementById("txtParcelas").disabled = desabilitar;
    document.getElementById("txtIntervaloDias").disabled = desabilitar;
    document.getElementById("txtDataVcto").disabled = desabilitar;
    document.getElementById("selArredondar").disabled = desabilitar;
    document.getElementById("btnGerarParcelas").disabled = desabilitar;

    document.getElementById("txtCte").disabled = desabilitar;
    document.getElementById("btnBuscarCte").disabled = desabilitar;

    document.getElementById("btnSalvar").disabled = desabilitar;
}

function PermiteAlterarPessoa() {
    var valido = true;
    var quantidade = 0;

    var ctes = $("body").data("ctes") == null ? new Array() : $("body").data("ctes");
    for (var i = 0; i < ctes.length; i++) {
        if (!ctes[i].Excluir)
            quantidade = quantidade + 1;
    }

    valido = !(quantidade > 0);

    return valido;
}

function BloquearAbaCTes() {
    if ($("#selTipo").val() == "0") {
        $('#tabsDetalhes a[href="#ctes"]').show();
    } else {
        $('#tabsDetalhes a[href="#ctes"]').hide();
        LimparCamposCtes();
    }
}

function Visualizar() {
    if (!ValidarVisualizacao())
        return;

    var dados = {
        Codigo: $("body").data("duplicata") != null ? $("body").data("duplicata").Codigo : 0,
        TipoArquivo: "PDF"
    };

    executarDownload("/Duplicatas/Visualizar", dados);
}

function BaixarEDICaterpillar() {
    if (!ValidarVisualizacao())
        return;

    var dados = {
        Codigo: $("body").data("duplicata") != null ? $("body").data("duplicata").Codigo : 0
    };

    executarDownload("/Duplicatas/BaixarEDICaterpillar", dados);
}

function ValidarVisualizacao() {
    var codigo = $("body").data("duplicata") != null ? $("body").data("duplicata").Codigo : 0;
    var valido = true;
    var mensagem = "<br/>";

    if (codigo == 0) {
        mensagem += "Nenhuma duplicata carregada/salva para visualização!"
        valido = false;
    }

    if (!valido) {
        ExibirMensagemAlerta(mensagem, "Atenção!");
        return false;
    } else {
        return true;
    }
}

function BuscarConfiguracoesEmpresa() {
    executarRest("/ConfiguracaoEmpresa/ObterDetalhes?callback=?", {}, function (r) {
        if (r.Sucesso) {
            ConfiguracoesEmpresa = r.Objeto;
            if (r.Objeto.ExibirOpcaoEDICaterpillarDuplicata)
                $("#btnEDICaterpillar").show();
            else
                $("#btnEDICaterpillar").hide();

        }
    });
}