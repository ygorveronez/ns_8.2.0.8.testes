$(document).ready(function () {
    CarregarConsultaDeMinutasDevolucaoContainer("default-search", "default-search", RetornoConsultaMinuta, true, false);

    CarregarConsultaDeCTes("btnBuscarCTe", "btnBuscarCTe", 0, RetornoConsultaCTe, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
    CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false, 0);
    CarregarConsultaDeVeiculos("btnBuscarReboque", "btnBuscarReboque", RetornoConsultaReboque, true, false, 1);
    CarregarConsultadeClientes("btnBuscarTerminal", "btnBuscarTerminal", RetornoConsultaTerminal, true, false);

    $("#txtPeso").priceFormat();

    $("#txtQuantidade").on('change', function () {
        var clearValue = this.value.replace(/[^0-9]/g, '');
        this.value = clearValue;
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

    $("#txtReboque").keydown(function (e) {
        if (e.which !== 9 && e.which !== 16) {
            if (e.which === 8 || e.which === 46) {
                $(this).val("");
                $("body").data("veiculo", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtTerminal").keydown(function (e) {
        if (e.which !== 9 && e.which !== 16) {
            if (e.which === 8 || e.which === 46) {
                $(this).val("");
                $("body").data("terminal", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnCancelar").click(function () {
        LimparCampos();
    });

    $("#btnDownloadVia1").click(function () {
        DownloadEspelho(1);
    });

    $("#btnDownloadVia2").click(function () {
        DownloadEspelho(2);
    });

    LimparCampos();
});

function LimparCampos() {
    $("body").data("codigo", null);

    $("#txtNumero").val("Automatico");
    $("#txtContainer").val("");
    $("#txtImportador").val("");
    $("#txtArmador").val("");
    $("#txtTipoEquipamento").val("");
    $("#txtQuantidade").val("");
    $("#txtPeso").val("0,00");
    $("#txtNavio").val("");
    $("#txtObservacao").val("");

    $("#txtTerminal").val('');
    $("body").data("terminal", null);

    $("#txtMotorista").val('');
    $("body").data("motorista", null);

    $("#txtVeiculo").val('');
    $("body").data("veiculo", null);

    $("#txtReboque").val('');
    $("body").data("reboque", null);

    $("#txtCTe").val('');
    $("body").data("cte", null);

    $("#selStatus").val($("#selStatus option:first").val());

    $("#btnDownloadVia1").hide();
    $("#btnDownloadVia2").hide();

    //BuscarProximoNumero();
}

function BuscarProximoNumero() {
    executarRest("/MinutaDevolucaoContainer/ObterProximoNumero?callback=?", '', function (r) {
        if (r.Sucesso) {
            $("#txtNumero").val(r.Objeto.numero);
        } else {
            $("#txtNumero").val("1");
        }
    });
}

function RetornoConsultaCTe(cte) {
    if (cte.Codigo > 0) {
        $("#txtCTe").val(cte.Numero + " - " + cte.Serie);
        $("body").data("codigoCTe", cte.Codigo);
    }
}

function RetornoConsultaMotorista(motorista) {
    $("body").data("motorista", motorista.Codigo);
    $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
}

function RetornoConsultaVeiculos(veiculo) {
    $("body").data("veiculo", veiculo.Codigo);
    $("#txtVeiculo").val(veiculo.Placa);
}

function RetornoConsultaReboque(veiculo) {
    $("body").data("reboque", veiculo.Codigo);
    $("#txtReboque").val(veiculo.Placa);
}

function RetornoConsultaTerminal(cliente) {
    ObterDadosTerminal(cliente.CPFCNPJ);
}

function ObterDadosTerminal(cpfCnpj) {
    executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto != null) {
                SetarDadosTerminal(r.Objeto);
            } else {
                LimparCamposTerminal();
                ExibirMensagemAlerta("Cliente não encontrado.", "Atenção!");
            }
        } else {
            LimparCamposTerminal();
            ExibirMensagemErro(r.Erro, "Erro!");
        }
    });
}

function LimparCamposTerminal() {
    $("body").data("terminal", null);
    $("#txtTerminal").val("");
}

function SetarDadosTerminal(cliente) {
    $("body").data("terminal", cliente.CPF_CNPJ);
    $("#txtTerminal").val(cliente.CPF_CNPJ + " - " + cliente.Nome);
}

function ValidarCampos() {
    var valido = true;

    var container = $("#txtContainer").val();
    var importador = $("#txtImportador").val();
    var codigoTerminal = $("body").data("terminal");
    var codigoCTe = $("body").data("codigoCTe");    

    if (codigoTerminal == null || codigoTerminal == "") {
        CampoComErro("#txtTerminal");
        valido = false;
    } else {
        CampoSemErro("#txtTerminal");
    }

    if (isNaN(codigoCTe) || codigoCTe <= 0) {
        CampoComErro("#txtCTe");
        valido = false;
    } else {
        CampoSemErro("#txtCTe");
    }

    if (container == null || container == "") {
        CampoComErro("#txtContainer");
        valido = false;
    } else {
        CampoSemErro("#txtContainer");
    }

    if (importador = null || importador == "") {
        CampoComErro("#txtImportador");
        valido = false;
    } else {
        CampoSemErro("#txtImportador");
    }

    return valido;
}

function Salvar() {
    if (ValidarCampos()) {
        var dados = {
            Codigo: $("body").data("codigo"),
            Container: $("#txtContainer").val(),
            Importador: $("#txtImportador").val(),
            Status: $("#selStatus").val(),
            Armador: $("#txtArmador").val(),
            TipoEquipamento: $("#txtTipoEquipamento").val(),
            Quantidade: $("#txtQuantidade").val(),
            Peso: $("#txtPeso").val(),
            Navio: $("#txtNavio").val(),
            Observacao: $("#txtObservacao").val(),
            CodigoTerminal: $("body").data("terminal"),
            CodigoMotorista: $("body").data("motorista"),
            CodigoTracao: $("body").data("veiculo"),
            CodigoReboque: $("body").data("reboque"),
            CodigoCTe: $("body").data("codigoCTe")
        };

        executarRest("/MinutaDevolucaoContainer/Salvar?callback=?", dados, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                LimparCampos();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção");
            }
        });

    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
    }
}

function RetornoConsultaMinuta(minuta) {
    executarRest("/MinutaDevolucaoContainer/ObterDetalhes?callback=?", { CodigoMinuta: minuta.Codigo }, function (r) {
        if (r.Sucesso) {

            $("body").data("codigo", r.Objeto.Codigo);
            $("#txtNumero").val(r.Objeto.Numero);
            $("#selStatus").val(r.Objeto.Status);

            $("#txtContainer").val(r.Objeto.Container);
            $("#txtImportador").val(r.Objeto.Importador);
            $("#txtArmador").val(r.Objeto.Armador);
            $("#txtTipoEquipamento").val(r.Objeto.TipoEquipamento);
            $("#txtQuantidade").val(r.Objeto.Quantidade);
            $("#txtPeso").val(r.Objeto.Peso);
            $("#txtNavio").val(r.Objeto.Navio);
            $("#txtObservacao").val(r.Objeto.Observacao);

            $("body").data("terminal", r.Objeto.CodigoTerminal);
            $("#txtTerminal").val(r.Objeto.NomeTerminal);

            $("body").data("veiculo", r.Objeto.CodigoTracao);
            $("#txtVeiculo").val(r.Objeto.PlacaTracao);

            $("body").data("reboque", r.Objeto.CodigoReboque);
            $("#txtReboque").val(r.Objeto.PlacaReboque);

            $("body").data("motorista", r.Objeto.CodigoMotorista);
            $("#txtMotorista").val(r.Objeto.NomeMotorista);

            $("body").data("codigoCTe", r.Objeto.CodigoCTe);
            $("#txtCTe").val(r.Objeto.DescricaoCTe);                                 

            $("#btnDownloadVia1").show();
            $("#btnDownloadVia2").show();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function DownloadEspelho(via) {
    executarDownload("/MinutaDevolucaoContainer/DownloadEspelho", { CodigoMinuta: $("body").data("codigo"), Via: via });
}