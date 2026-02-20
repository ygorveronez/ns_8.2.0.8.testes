$(document).ready(function () {
    CarregarConsultaDeTiposDeColetas("btnBuscarTipoColeta", "btnBuscarTipoColeta", "A", RetornoConsultaTipoColeta, true, false);
    CarregarConsultaDeColetas("default-search", "default-search", "A", RetornoConsultaColeta, true, false);
    CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaRemetente, true, false);
    CarregarConsultadeClientes("btnBuscarDestinatario", "btnBuscarDestinatario", RetornoConsultaDestinatario, true, false);
    CarregarConsultadeClientes("btnBuscarTomador", "btnBuscarTomador", RetornoConsultaTomador, true, false);
    CarregarConsultaDeLocalidades("btnBuscarOrigem", "btnBuscarOrigem", RetornoConsultaOrigem, true, false);
    CarregarConsultaDeLocalidades("btnBuscarDestino", "btnBuscarDestino", RetornoConsultaDestino, true, false);
    CarregarConsultaDeTiposDeCargas("btnBuscarTipoCarga", "btnBuscarTipoCarga", "A", RetornoConsultaTipoCarga, true, false);

    $("#txtCPFCNPJRemetente").focusout(function () {
        BuscarRemetente();
    });

    $("#txtCPFCNPJDestinatario").focusout(function () {
        BuscarDestinatario();
    });

    $("#txtCPFCNPJTomador").focusout(function () {
        BuscarTomador();
    });

    RemoveConsulta($("#txtRemetente"), function ($this) {
        $this.val("");
        $("body").data("codigoRemetente", null);
        $("#txtCPFCNPJRemetente").val("");
    });

    RemoveConsulta($("#txtDestinatario"), function ($this) {
        $this.val("");
        $("body").data("codigoDestinatario", null);
        $("#txtCPFCNPJDestinatario").val("");
    });

    RemoveConsulta($("#txtTomador"), function ($this) {
        $this.val("");
        $("body").data("codigoTomador", null);
        $("#txtCPFCNPJTomador").val("");
    });

    RemoveConsulta($("#txtOrigem"), function ($this) {
        $this.val("");
        $("body").data("codigoOrigem", null);
    });

    RemoveConsulta($("#txtDestino"), function ($this) {
        $this.val("");
        $("body").data("codigoDestino", null);
    });

    RemoveConsulta($("#txtTipoColeta"), function ($this) {
        $this.val("");
        $("body").data("codigoTipoColeta", null);
    });

    RemoveConsulta($("#txtTipoCarga"), function ($this) {
        $this.val("");
        $("body").data("codigoTipoCarga", null);
    });
});

function RetornoConsultaColeta(coleta) {
    executarRest("/Coleta/ObterDetalhes?callback=?", { CodigoColeta: coleta.Codigo }, function (r) {
        if (r.Sucesso) {

            $("#txtNumero").val(r.Objeto.Numero);

            $("body").data("codigoTipoCarga", r.Objeto.CodigoTipoCarga);
            $("#txtTipoCarga").val(r.Objeto.DescricaoTipoCarga);

            $("body").data("codigoTipoColeta", r.Objeto.CodigoTipoColeta);
            $("#txtTipoColeta").val(r.Objeto.DescricaoTipoColeta);

            $("body").data("codigoOrigem", r.Objeto.CodigoOrigem);
            $("#txtOrigem").val(r.Objeto.DescricaoOrigem);

            $("body").data("codigoDestino", r.Objeto.CodigoDestino);
            $("#txtDestino").val(r.Objeto.DescricaoDestino);

            $("body").data("codigoRemetente", r.Objeto.CPFCNPJRemetente);
            $("#txtCPFCNPJRemetente").val(r.Objeto.CPFCNPJRemetente);
            $("#txtRemetente").val(r.Objeto.NomeRemetente);

            if (r.Objeto.CPFCNPJRemetente != null && r.Objeto.CPFCNPJRemetente != "") {
                $("#txtOrigem").attr("disabled", true);
                $("#btnBuscarOrigem").attr("disabled", true);
            }

            $("body").data("codigoDestinatario", r.Objeto.CPFCNPJDestinatario);
            $("#txtCPFCNPJDestinatario").val(r.Objeto.CPFCNPJDestinatario);
            $("#txtDestinatario").val(r.Objeto.NomeDestinatario);

            if (r.Objeto.CPFCNPJDestinatario != null && r.Objeto.CPFCNPJDestinatario != "") {
                $("#txtDestino").attr("disabled", true);
                $("#btnBuscarDestino").attr("disabled", true);
            }

            $("body").data("codigoTomador", r.Objeto.CPFCNPJTomador);
            $("#txtCPFCNPJTomador").val(r.Objeto.CPFCNPJTomador);
            $("#txtTomador").val(r.Objeto.NomeTomador);

            $("#txtDataInicial").val(r.Objeto.DataInicial);
            $("#txtDataFinal").val(r.Objeto.DataFinal);
            $("#txtDataEntrega").val(r.Objeto.DataEntrega);
            $("#txtPeso").val(r.Objeto.Peso);
            $("#txtValorNFs").val(r.Objeto.ValorNFs);
            $("#txtValorFrete").val(r.Objeto.ValorFrete);
            $("#selRequisitante").val(r.Objeto.Requisitante);
            $("#selTipoPagamento").val(r.Objeto.TipoPagamento);
            $("#selSituacao").val(r.Objeto.Situacao);
            $("#txtCodigoPedidoCliente").val(r.Objeto.CodigoPedidoCliente);
            $("#txtObservacao").val(r.Objeto.Observacao);
            $("#txtObservacaoCTe").val(r.Objeto.ObservacaoCTe);
            $("#txtQtVolumes").val(r.Objeto.QtVolumes);
            $("#txtNumeroNotaCliente").val(r.Objeto.NumeroNotaCliente);

            $("body").data("veiculos", r.Objeto.Veiculos);
            RenderizarVeiculos();

            $("body").data("motoristas", r.Objeto.Motoristas);
            RenderizarMotoristas();

            $("body").data("codigo", r.Objeto.Codigo);

            $("#btnDownloadEspelho").show();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function RetornoConsultaTipoCarga(tipoCarga) {
    $("body").data("codigoTipoCarga", tipoCarga.Codigo);
    $("#txtTipoCarga").val(tipoCarga.Descricao);
}

function RetornoConsultaTipoColeta(tipo) {
    $("#txtTipoColeta").val(tipo.Descricao);
    $("body").data("codigoTipoColeta", tipo.Codigo);
}

function RetornoConsultaOrigem(localidade) {
    $("body").data("codigoOrigem", localidade.Codigo);
    $("#txtOrigem").val(localidade.Descricao + " - " + localidade.UF);
}

function RetornoConsultaDestino(localidade) {
    $("body").data("codigoDestino", localidade.Codigo);
    $("#txtDestino").val(localidade.Descricao + " - " + localidade.UF);
}

function BuscarRemetente() {
    if ($("body").data("codigoRemetente") != $("#txtCPFCNPJRemetente").val()) {
        var cpfCnpj = $("#txtCPFCNPJRemetente").val().replace(/[^0-9]/g, '');
        if (cpfCnpj != "") {
            if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                ObterDadosRemetente(cpfCnpj);
            } else {
                LimparCamposRemetente();
                ExibirMensagemAlerta("O CPF/CNPJ digitado é inválido.", "Atenção!");
            }
        } else if ($("body").data("codigoRemetente") != null) {
            LimparCamposRemetente();
        }
    }
}
function ObterDadosRemetente(cpfCnpj) {
    executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto != null) {
                SetarDadosRemetente(r.Objeto);
            } else {
                LimparCamposRemetente();
                ExibirMensagemAlerta("Remetente não encontrado.", "Atenção!");
            }
        } else {
            LimparCamposRemetente();
            ExibirMensagemErro(r.Erro, "Erro!");
        }
    });
}
function RetornoConsultaRemetente(remetente) {
    ObterDadosRemetente(remetente.CPFCNPJ);
}
function SetarDadosRemetente(remetente) {
    $("body").data("codigoRemetente", remetente.CPF_CNPJ);
    $("#txtCPFCNPJRemetente").val(remetente.CPF_CNPJ);
    $("#txtRemetente").val(remetente.Nome);

    $("body").data("codigoOrigem", remetente.CodigoLocalidade);
    $("#txtOrigem").val(remetente.Localidade + " - " + remetente.UF);

    $("#txtOrigem").attr("disabled", true);
    $("#btnBuscarOrigem").attr("disabled", true);
}
function LimparCamposRemetente() {
    $("body").data("codigoRemetente", null);
    $("#txtCPFCNPJRemetente").val("");
    $("#txtRemetente").val("");

    $("body").data("codigoOrigem", null);
    $("#txtOrigem").val("");

    $("#txtOrigem").removeAttr("disabled");
    $("#btnBuscarOrigem").removeAttr("disabled");
}

function BuscarDestinatario() {
    if ($("#txtCPFCNPJDestinatario").val() != $("body").data("codigoDestinatario")) {
        var cpfCnpj = $("#txtCPFCNPJDestinatario").val().replace(/[^0-9]/g, '');
        if (cpfCnpj != "") {
            if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                ObterDadosDestinatario(cpfCnpj)
            } else {
                LimparCamposDestinatario();
                ExibirMensagemAlerta("O CPF/CNPJ digitado é inválido.", "Atenção!");
            }
        } else if ($("body").data("codigoDestinatario") != null) {
            LimparCamposDestinatario();
        }
    }
}
function ObterDadosDestinatario(cpfCnpj) {
    executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto != null) {
                SetarDadosDestinatario(r.Objeto);
            } else {
                LimparCamposDestinatario();
                ExibirMensagemAlerta("Destinatário não encontrado.", "Atenção!");
            }
        } else {
            LimparCamposDestinatario();
            ExibirMensagemErro(r.Erro, "Erro!");
        }
    });
}
function RetornoConsultaDestinatario(destinatario) {
    ObterDadosDestinatario(destinatario.CPFCNPJ);
}
function SetarDadosDestinatario(destinatario) {
    $("body").data("codigoDestinatario", destinatario.CPF_CNPJ);
    $("#txtCPFCNPJDestinatario").val(destinatario.CPF_CNPJ);
    $("#txtDestinatario").val(destinatario.Nome);

    $("body").data("codigoDestino", destinatario.CodigoLocalidade);
    $("#txtDestino").val(destinatario.Localidade + " - " + destinatario.UF);

    $("#txtDestino").attr("disabled", true);
    $("#btnBuscarDestino").attr("disabled", true);
}
function LimparCamposDestinatario() {
    $("body").data("codigoDestinatario", null);
    $("#txtCPFCNPJDestinatario").val("");
    $("#txtDestinatario").val("");

    $("body").data("codigoDestino", null);
    $("#txtDestino").val("");

    $("#txtDestino").removeAttr("disabled");
    $("#btnBuscarDestino").removeAttr("disabled");
}

function BuscarTomador() {
    if ($("#txtCPFCNPJTomador").val() != $("body").data("codigoTomador")) {
        var cpfCnpj = $("#txtCPFCNPJTomador").val().replace(/[^0-9]/g, '');
        if (cpfCnpj != "") {
            if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto != null) {
                            $("body").data("codigoTomador", r.Objeto.CPF_CNPJ);
                            $("#txtCPFCNPJTomador").val(r.Objeto.CPF_CNPJ);
                            $("#txtTomador").val(r.Objeto.Nome);
                        } else {
                            LimparCamposTomador();
                            ExibirMensagemAlerta("Destinatário não encontrado.", "Atenção!");
                        }
                    } else {
                        LimparCamposTomador();
                        ExibirMensagemErro(r.Erro, "Erro!");
                    }
                });
            } else {
                LimparCamposTomador();
                ExibirMensagemAlerta("O CPF/CNPJ digitado é inválido!", "Atenção!");
            }
        } else {
            LimparCamposTomador();
        }
    }
}
function LimparCamposTomador() {
    $("body").data("codigoTomador", null);
    $("#txtCPFCNPJTomador").val("");
    $("#txtTomador").val("");
}
function RetornoConsultaTomador(tomador) {
    $("body").data("codigoTomador", tomador.CPFCNPJ);
    $("#txtTomador").val(tomador.Nome);
    $("#txtCPFCNPJTomador").val(tomador.CPFCNPJ);
}