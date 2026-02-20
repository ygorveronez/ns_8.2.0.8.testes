$(document).ready(function () {
    CarregarConsultaFreteSubcontratado("default-search", "default-search", "", RetornoConsultaFreteSubcontratado, true, false);
    CarregarConsultadeClientes("btnBuscarParceiro", "btnBuscarParceiro", RetornoConsultaParceiro, true, false, "");
    CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaRemetente, true, false, "");
    CarregarConsultadeClientes("btnBuscarDestinatario", "btnBuscarDestinatario", RetornoConsultaDestinatario, true, false, "");
    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, true, false);

    FormatarCampoDate("txtDataEntrada");
    FormatarCampoDateTime("txtDataEntrega");

    $("#txtPercentualComissao").priceFormat();
    $("#txtPeso").priceFormat();
    $("#txtValorFrete").priceFormat();
    $("#txtValorICMS").priceFormat();
    $("#txtValorFreteLiquido").priceFormat();
    $("#txtValorTaxaAdicional").priceFormat();
    $("#txtValorTDA").priceFormat();
    $("#txtValorTDE").priceFormat();
    $("#txtValorCarroDedicado").priceFormat();
    $("#txtValorComissao").priceFormat();
    $("#txtValorTotalComissao").priceFormat();
    $("#txtValorComissaoMinimo").priceFormat();

    $("#txtCTe").mask("9?9999999");
    $("#txtNFe").mask("9?9999999");
    $("#txtQuantidade").mask("9?9999999");

    $("#txtParceiro").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("CNPJCPFParceiro", null);
                $("#txtPercComissao").Val("");
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtRemetente").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("CNPJCPFRemetente", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtDestiantario").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("CNPJCPFDestinatario", null);
                $("#txtLocalidade").Val("");
                $("body").data("localidade", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtMotorista").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("motorista", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtValorFrete").focusout(function () {
        CalculaComissao();
    });

    $("#txtValorICMS").focusout(function () {
        CalculaComissao();
    });

    $("#txtValorTaxaAdicional").focusout(function () {
        CalculaComissao();
    });

    $("#txtPercentualComissao").focusout(function () {
        CalculaComissao();
    });

    $("#txtValorTDA").focusout(function () {
        CalculaTotalComissao();
    });

    $("#txtValorTDE").focusout(function () {
        CalculaTotalComissao();
    });

    $("#txtValorCarroDedicado").focusout(function () {
        CalculaTotalComissao();
    });

    $("#btnSalvar").click(function () {
        ValidaDuplicidade();
    });

    $("#btnCancelar").click(function () {
        LimparCampos();
        ControlarCampos();
    });

    $("#selTipo").change(function () {
        AtualizarComissao();
    });

    LimparCampos();
});

function RetornoConsultaFreteSubcontratado(freteSubcontratado) {
    executarRest("/FreteSubcontratado/ObterDetalhes?callback=?", { Codigo: freteSubcontratado.Codigo }, function (r) {
        if (r.Sucesso) {
            RenderizarFreteSubcontratado(r.Objeto);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function RetornoConsultaParceiro(cliente) {
    $("body").data("parceiro", cliente.CPFCNPJ);
    $("#txtParceiro").val(cliente.CPFCNPJ + " - " + cliente.Nome);    
    BuscaComissao();
    CalculaComissao();
}

function RetornoConsultaRemetente(cliente) {
    $("body").data("remetente", cliente.CPFCNPJ);
    $("#txtRemetente").val(cliente.CPFCNPJ + " - " + cliente.Nome);
    if ($("#selTipo").val() == "2") {
        if (cliente.CPFCNPJ != "") {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cliente.CPFCNPJ }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        $("#txtLocalidade").val(r.Objeto.Localidade);
                        $("body").data("localidade", r.Objeto.CodigoLocalidade);
                        BuscaComissao();
                        CalculaComissao();
                    } else {
                        $("#txtLocalidade").val('');
                        $("body").data("localidade", null);
                        CalculaComissao();
                        jAlert("<b>Localidade origem não encontrada.</b>", "Atenção");
                    }
                } else {
                    jAlert(r.Erro, "Erro");
                    $("#txtLocalidade").val('');
                    $("body").data("localidade", null);
                    CalculaComissao();
                }
            });
        }
    }

}

function RetornoConsultaDestinatario(cliente) {
    $("body").data("destinatario", cliente.CPFCNPJ);
    $("#txtDestinatario").val(cliente.CPFCNPJ + " - " + cliente.Nome);
    if ($("#selTipo").val() != "2") {
        if (cliente.CPFCNPJ != "") {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cliente.CPFCNPJ }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        $("#txtLocalidade").val(r.Objeto.Localidade);
                        $("body").data("localidade", r.Objeto.CodigoLocalidade);
                        $("#txtValorTDE").val(Globalize.format(r.Objeto.ValorTDE, "n2"));
                        BuscaComissao();
                        CalculaComissao();
                    } else {
                        $("#txtLocalidade").val('');
                        $("body").data("localidade", null);
                        $("#txtValorTDE").val("0,00");
                        CalculaComissao();
                        jAlert("<b>Localidade destino não encontrada.</b>", "Atenção");
                    }
                } else {
                    jAlert(r.Erro, "Erro");
                    $("#txtLocalidade").val('');
                    $("body").data("localidade", null);
                    CalculaComissao();
                }
            });
        }
    }
}

function RetornoConsultaMotorista(motorista) {
    $("body").data("motorista", motorista.Codigo);
    $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
}

function LimparCampos() {
    $("body").data("freteSubcontratado", null);

    $("#txtParceiro").val('');
    $("body").data("parceiro", null);
    $("#txtFilial").val('');
    $("#txtCTe").val('');
    $("#txtNFe").val('');
    $("#txtDataEntrada").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#selStatus").val($("#selStatus option:first").val()).change();
    $("#txtRemetente").val('');
    $("body").data("remetente", null);
    $("#txtDestinatario").val('');
    $("body").data("destinatario", null);
    $("#txtLocalidade").val('');
    $("body").data("localidade", null);
    $("#txtPercentualComissao").val('0,00');
    $("#txtValorComissaoMinimo").val('0,00');
    $("#txtPeso").val('0,00');
    $("#txtQuantidade").val('0');
    $("#txtValorFrete").val('0,00');
    $("#txtValorICMS").val('0,00');
    $("#txtValorFreteLiquido").val('0,00');
    $("#txtValorTaxaAdicional").val('0,00');
    $("#txtValorTDA").val('0,00');
    $("#txtValorTDE").val('0,00');
    $("#txtValorCarroDedicado").val('0,00');
    $("#txtValorComissao").val('0,00');
    $("#txtValorTotalComissao").val('0,00');
    $("#txtDataEntrega").val('');
    $("#txtMotorista").val('');
    $("body").data("motorista", null);
    $("#txtObservacao").val('');
    $("#txtRecebedorDocumento").val('');
    $("#selTipo").val($("#selTipo option:first").val()).change();
}

function ControlarCampos() {
    desabilitar = ($("#selStatus").val() == "1")
    if (desabilitar)
        ExibirMensagemAlerta("Frete Subcontratado foi Fechado, impossível alterar!", "Atenção!");

    document.getElementById("txtParceiro").disabled = desabilitar;
    document.getElementById("btnBuscarParceiro").disabled = desabilitar;
    document.getElementById("txtFilial").disabled = desabilitar;
    document.getElementById("txtCTe").disabled = desabilitar;
    document.getElementById("txtNFe").disabled = desabilitar;
    document.getElementById("txtDataEntrada").disabled = desabilitar;
    document.getElementById("selTipo").disabled = desabilitar;
    //document.getElementById("selStatus").disabled = desabilitar;
    document.getElementById("txtRemetente").disabled = desabilitar;
    document.getElementById("btnBuscarRemetente").disabled = desabilitar;
    document.getElementById("txtDestinatario").disabled = desabilitar;
    document.getElementById("btnBuscarDestinatario").disabled = desabilitar;
    document.getElementById("txtPeso").disabled = desabilitar;
    document.getElementById("txtQuantidade").disabled = desabilitar;
    document.getElementById("txtValorFrete").disabled = desabilitar;
    document.getElementById("txtValorICMS").disabled = desabilitar;
    document.getElementById("txtValorTaxaAdicional").disabled = desabilitar;
    document.getElementById("txtValorTDA").disabled = desabilitar;
    document.getElementById("txtValorTDE").disabled = desabilitar;
    document.getElementById("txtValorCarroDedicado").disabled = desabilitar;
    document.getElementById("txtDataEntrega").disabled = desabilitar;
    document.getElementById("txtMotorista").disabled = desabilitar;
    document.getElementById("btnBuscarMotorista").disabled = desabilitar;
    document.getElementById("txtRecebedorDocumento").disabled = desabilitar;
    if ($("#selTipo").val() == "1" || $("#selTipo").val() == "2") {
        document.getElementById("txtPercentualComissao").disabled = false;
    }
    else {
        document.getElementById("txtPercentualComissao").disabled = true;
    }
}

function Salvar() {
    if (!ValidarCampos())
        return;

    var freteSubcontratado = {
        Codigo: $("body").data("freteSubcontratado") != null ? $("body").data("freteSubcontratado").Codigo : 0,
        Parceiro: $("body").data("parceiro"),
        Filial: $("#txtFilial").val(),
        NumeroCTe: $("#txtCTe").val(),
        NumeroNFe: $("#txtNFe").val(),
        DataEntrada: $("#txtDataEntrada").val(),
        Tipo: $("#selTipo").val(),
        Status: $("#selStatus").val(),
        Remetente: $("body").data("remetente"),
        Destinatario: $("body").data("destinatario"),
        Localidade: $("body").data("localidade"),
        PercentualComissao: $("#txtPercentualComissao").val(),
        Peso: $("#txtPeso").val(),
        Quantidade: $("#txtQuantidade").val(),
        ValorFrete: $("#txtValorFrete").val(),
        ValorICMS: $("#txtValorICMS").val(),
        valorFreteLiquido: $("#txtValorFreteLiquido").val(),
        ValorTaxaAdicional: $("#txtValorTaxaAdicional").val(),
        ValorTDA: $("#txtValorTDA").val(),
        ValorTDE: $("#txtValorTDE").val(),
        ValorCarroDedicado: $("#txtValorCarroDedicado").val(),
        ValorComissao: $("#txtValorComissao").val(),
        DataEntrega: $("#txtDataEntrega").val(),
        Motorista: $("body").data("motorista"),
        RecebedorDocumento: $("#txtRecebedorDocumento").val(),
        Observacao: $("#txtObservacao").val()
    };

    executarRest("/FreteSubcontratado/Salvar?callback=?", freteSubcontratado, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
            LimparCampos();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function RenderizarFreteSubcontratado(freteSubcontratado) {
    LimparCampos();

    $("body").data("freteSubcontratado", freteSubcontratado);
    $("body").data("parceiro", freteSubcontratado.CNPJCPFParceiro);
    $("#txtParceiro").val(freteSubcontratado.Parceiro);
    $("#txtFilial").val(freteSubcontratado.Filial);
    $("#txtCTe").val(freteSubcontratado.NumeroCTe);
    $("#txtNFe").val(freteSubcontratado.NumeroNFe);
    $("#txtDataEntrada").val(freteSubcontratado.DataEntrada);
    $("body").data("remetente", freteSubcontratado.CNPJCPFRemetente);
    $("#txtRemetente").val(freteSubcontratado.Remetente);
    $("body").data("destinatario", freteSubcontratado.CNPJCPFDestinatario);
    $("#txtDestinatario").val(freteSubcontratado.Destinatario);
    $("body").data("localidade", freteSubcontratado.CodigoLocalidade);
    $("#txtLocalidade").val(freteSubcontratado.Localidade);
    $("#selTipo").val(freteSubcontratado.Tipo);
    $("#selStatus").val(freteSubcontratado.Status).change();
    $("#txtPeso").val(freteSubcontratado.Peso);
    $("#txtQuantidade").val(freteSubcontratado.Quantidade);
    $("#txtValorFrete").val(freteSubcontratado.ValorFrete);
    $("#txtValorICMS").val(freteSubcontratado.ValorICMS);
    $("#txtValorFreteLiquido").val(freteSubcontratado.ValorFreteLiquido);
    $("#txtValorTaxaAdicional").val(freteSubcontratado.ValorTaxaAdicional);
    $("#txtValorTDA").val(freteSubcontratado.ValorTDA);
    $("#txtValorTDE").val(freteSubcontratado.ValorTDE);
    $("#txtValorCarroDedicado").val(freteSubcontratado.ValorCarroDedicado);
    $("#txtValorComissao").val(freteSubcontratado.ValorComissao);
    $("body").data("motorista", freteSubcontratado.CodigoMotorista);
    $("#txtMotorista").val(freteSubcontratado.Motorista);
    $("#txtRecebedorDocumento").val(freteSubcontratado.RecebedorDocumento);
    $("#txtDataEntrega").val(freteSubcontratado.DataEntrega);
    $("#txtObservacao").val(freteSubcontratado.Observacao);
    $("#txtValorComissaoMinimo").val(freteSubcontratado.ValorMinimo);
    $("#txtPercentualComissao").val(freteSubcontratado.PercentualComissao);

    CalculaComissao();
    ControlarCampos();
}

function ValidarCampos() {
    var parceiro = $("body").data("parceiro");
    var remetente = $("body").data("remetente");
    var destinatario = $("body").data("destinatario");
    var localidade = $("body").data("localidade");
    var filial = $("#txtFilial").val();
    var dataEntrada = $("#txtDataEntrada").val();
    var cte = $("#txtCTe").val();
    var nfe = $("#txtNFe").val();
    var peso = Globalize.parseFloat($("#txtPeso").val());
    var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());
    var valorComissao = Globalize.parseFloat($("#txtValorComissao").val());
    var dataEntrega = $("#txtDataEntrega").val();
    var valido = true;

    if (parceiro == null || parceiro == "") {
        CampoComErro("#txtParceiro");
        valido = false;
    } else {
        CampoSemErro("#txtParceiro");
    }

    if (remetente == null || remetente == "") {
        CampoComErro("#txtRemetente");
        valido = false;
    } else {
        CampoSemErro("#txtRemetente");
    }

    if (destinatario == null || destinatario == "") {
        CampoComErro("#txtDestinatario");
        valido = false;
    } else {
        CampoSemErro("#txtDestinatario");
    }

    if (localidade == null || localidade == "") {
        CampoComErro("#txtLocalidade");
        valido = false;
    } else {
        CampoSemErro("#txtLocalidade");
    }

    if (filial == null || filial == "") {
        CampoComErro("#txtFilial");
        valido = false;
    } else {
        CampoSemErro("#txtFilial");
    }

    if (isNaN(cte) || cte <= 0) {
        CampoComErro("#txtCTe");
        valido = false;
    } else {
        CampoSemErro("#txtCTe");
    }

    if (isNaN(nfe) || nfe <= 0) {
        CampoComErro("#txtNFe");
        valido = false;
    } else {
        CampoSemErro("#txtNFe");
    }

    if (dataEntrada == null || dataEntrada == "") {
        CampoComErro("#txtDataEntrada");
        valido = false;
    } else {
        CampoSemErro("#txtDataEntrada");
    }

    if (isNaN(peso) || peso <= 0) {
        CampoComErro("#txtPeso");
        valido = false;
    } else {
        CampoSemErro("#txtPeso");
    }

    if (isNaN(valorFrete) || valorFrete <= 0) {
        CampoComErro("#txtValorFrete");
        valido = false;
    } else {
        CampoSemErro("#txtValorFrete");
    }

    if (isNaN(valorComissao) || valorComissao <= 0) {
        CampoComErro("#txtValorComissao");
        valido = false;
    } else {
        CampoSemErro("#txtValorComissao");
    }

    var status = ($("#selStatus").val() == "1")
    if (status) {
        if (dataEntrega == null || dataEntrega == "") {
            CampoComErro("#txtDataEntrega");
            valido = false;
        } else {
            CampoSemErro("#txtDataEntrega");
        }
    }

    if (!valido)
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!");

    return valido;
}

function CalculaComissao() {
    var parceiro = $("body").data("parceiro");
    var localidade = $("body").data("localidade");
    var percentualComissao = Globalize.parseFloat($("#txtPercentualComissao").val());
    var valorMinimo = Globalize.parseFloat($("#txtValorComissaoMinimo").val());

    var valorFreteLiquido, valorComissao, percentualComissao, valorMinimo;
    valorFreteLiquido = Globalize.parseFloat($("#txtValorFrete").val()) - Globalize.parseFloat($("#txtValorICMS").val()) - Globalize.parseFloat($("#txtValorTaxaAdicional").val());

    $("#txtValorFreteLiquido").val(Globalize.format(valorFreteLiquido, "n2"));

    if (parceiro == null || parceiro == "" || localidade == null || localidade == "") {
        $("#txtValorComissao").val("0,00");
        $("#txtPercentualComissao").val("0,00");
        $("#txtValorTotalComissao").val("0,00");
        $("#txtValorComissaoMinimo").val("0,00");
    } else {

        valorComissao = valorFreteLiquido * (percentualComissao / 100);
        if (!(valorComissao > valorMinimo))
            valorComissao = valorMinimo;

        $("#txtValorComissao").val(Globalize.format(valorComissao, "n2"));

        CalculaTotalComissao();
    }
}

function CalculaTotalComissao() {
    var valorTotalComissao;
    valorTotalComissao = Globalize.parseFloat($("#txtValorComissao").val()) + Globalize.parseFloat($("#txtValorTDA").val()) + +Globalize.parseFloat($("#txtValorTDE").val()) + Globalize.parseFloat($("#txtValorCarroDedicado").val());
    $("#txtValorTotalComissao").val(Globalize.format(valorTotalComissao, "n2"));
}


function AtualizarComissao() {
    var cnpjClienteDestinatario = $("body").data("destinatario");
    var cnpjClienteRemetente = $("body").data("remetente");

    if ($("#selTipo").val() != "2") {
        if (cnpjClienteDestinatario != null && cnpjClienteDestinatario != "") {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cnpjClienteDestinatario }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        $("#txtLocalidade").val(r.Objeto.Localidade);
                        $("body").data("localidade", r.Objeto.CodigoLocalidade);
                        BuscaComissao();
                        CalculaComissao();
                    } else {
                        $("#txtLocalidade").val('');
                        $("body").data("localidade", null);
                        CalculaComissao();
                        jAlert("<b>Localidade destino não encontrada.</b>", "Atenção");
                    }
                } else {
                    jAlert(r.Erro, "Erro");
                    $("#txtLocalidade").val('');
                    $("body").data("localidade", null);
                    CalculaComissao();
                }
            });
        }
        else {
            $("#txtLocalidade").val('');
            $("body").data("localidade", null);
            CalculaComissao();
        }
    }
    else {

        if (cnpjClienteRemetente != null && cnpjClienteRemetente != "") {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cnpjClienteRemetente }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        $("#txtLocalidade").val(r.Objeto.Localidade);
                        $("body").data("localidade", r.Objeto.CodigoLocalidade);
                        BuscaComissao();
                        CalculaComissao();
                    } else {
                        $("#txtLocalidade").val('');
                        $("body").data("localidade", null);
                        CalculaComissao();
                        jAlert("<b>Localidade destino não encontrada.</b>", "Atenção");
                    }
                } else {
                    jAlert(r.Erro, "Erro");
                    $("#txtLocalidade").val('');
                    $("body").data("localidade", null);
                    CalculaComissao();
                }
            });
        }
        else {
            $("#txtLocalidade").val('');
            $("body").data("localidade", null);
            CalculaComissao();
        }

    }
    if ($("#selTipo").val() == "1" || $("#selTipo").val() == "2") {
        document.getElementById("txtPercentualComissao").disabled = false;
    }
    else {
        document.getElementById("txtPercentualComissao").disabled = true;
    }
}

function BuscaComissao() {
    var parceiro = $("body").data("parceiro");
    var localidade = $("body").data("localidade");
    var valorFreteLiquido, valorComissao, percentualComissao, valorMinimo;
    valorFreteLiquido = Globalize.parseFloat($("#txtValorFrete").val()) - Globalize.parseFloat($("#txtValorICMS").val()) - Globalize.parseFloat($("#txtValorTaxaAdicional").val());

    $("#txtValorFreteLiquido").val(Globalize.format(valorFreteLiquido, "n2"));

    if (parceiro == null || parceiro == "" || localidade == null || localidade == "") {
        $("#txtValorComissao").val("0,00");
        $("#txtPercentualComissao").val("0,00");
        $("#txtValorTotalComissao").val("0,00");
        $("#txtValorComissaoMinimo").val("0,00");
        $("#txtValorTDA").val("0,00");
    } else {
        executarRest("/ClienteComissao/ConsultarDetalhes?callback=?", { Parceiro: parceiro, Localidade: localidade }, function (r) {
            if (r.Sucesso) {
                if (r.Objeto != null) {
                    valorMinimo = r.Objeto.ValorMinimo;
                    percentualComissao = r.Objeto.PercentualComissao;
                    valorTDA = r.Objeto.ValorTDA;

                    $("#txtPercentualComissao").val(Globalize.format(percentualComissao, "n2"));
                    $("#txtValorComissaoMinimo").val(Globalize.format(valorMinimo, "n2"));
                    $("#txtValorTDA").val(Globalize.format(valorTDA, "n2"));

                    CalculaComissao();

                } else {
                    $("#txtValorComissao").val("0,00");
                    $("#txtPercentualComissao").val("0,00");
                    $("#txtValorTotalComissao").val("0,00");
                    $("#txtValorComissaoMinimo").val("0,00");
                    $("#txtValorTDA").val("0,00");
                    jAlert("<b>Comissão não encontrada.</b>", "Atenção");
                }
            } else {
                jAlert(r.Erro, "Erro");
                $("#txtValorComissao").val("0,00");
                $("#txtPercentualComissao").val("0,00");
                $("#txtValorTotalComissao").val("0,00");
                $("#txtValorComissaoMinimo").val("0,00");
                $("#txtValorTDA").val("0,00");
            }
        });
    }
}

function ValidaDuplicidade() {
    var parceiro = $("body").data("parceiro");
    var cte = $("#txtCTe").val();
    var tipo = $("#selTipo").val();
    var codigo = 0;
    if ($("body").data("freteSubcontratado") != null)
     codigo = $("body").data("freteSubcontratado").Codigo;
    var valido = true;

    executarRest("/FreteSubcontratado/VerificaDuplicidade?callback=?", { Codigo: codigo, Parceiro: parceiro, NumeroCTe: cte, Tipo: tipo }, function (r) {
        if (r.Sucesso) {
            CampoComErro("#txtCTe");
            valido = false;
        } else {
            CampoSemErro("#txtCTe");
        }
        if (!valido) {
            ExibirMensagemAlerta("Já existe um lançamento deste tipo para este parceiro com o mesmo número de CTe!", "Atenção!");
        }
        else
            Salvar();
    });
}