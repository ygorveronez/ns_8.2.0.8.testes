$(document).ready(function () {
    CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false);

    $("#txtPlacaVeiculo").mask("*******");

    $("#btnAdicionarVeiculo").click(function () {
        AdicionarVeiculo();
    });
});

function RetornoConsultaVeiculo(veiculo) {
    $("#txtPlacaVeiculo").val(veiculo.Placa);
}

function AdicionarVeiculo() {
    if ($("#txtPlacaVeiculo").val() != "") {
        executarRest("/Veiculo/BuscarPorPlaca?callback=?", { Placa: $("#txtPlacaVeiculo").val() }, function (r) {
            if (r.Sucesso) {
                SalvarVeiculo(r.Objeto);

                if (r.Objeto.VeiculosVinculados != null)
                    for (var i = 0; i < r.Objeto.VeiculosVinculados.length; i++)
                        SalvarVeiculo(r.Objeto.VeiculosVinculados[i]);

                AdicionarMotorista(r.Objeto.CodigoMotorista, r.Objeto.NomeMotorista, r.Objeto.CPFMotorista);
            } else {
                jAlert(r.Erro, "Atenção");
            }
        });
    }
}

function AdicionarMotorista(codigo, nome, cpf) {
    if ((codigo != null && codigo > 0) && (nome != null && nome != "") && (cpf != null && cpf != "")) {
        $("body").data("codigoMotorista", codigo);
        $("#txtNomeMotorista").val(nome);
        $("#txtCPFMotorista").val(cpf);
        SalvarMotorista();
    }
}

function SalvarVeiculo(veiculo) {
    var veiculos = $("body").data('veiculos') == null ? new Array() : $("body").data('veiculos');

    for (var i = 0; i < veiculos.length; i++) {
        if (veiculos[i].Codigo == veiculo.Codigo) {
            veiculos.splice(i, 1);
            break;
        }
    }

    veiculos.push(veiculo);

    $("body").data('veiculos', veiculos);

    RenderizarVeiculos();
    LimparCamposVeiculo();
}

function ExcluirVeiculo(veiculo) {
    jConfirm("Deseja realmente excluir este veículo?", "Atenção", function (r) {
        if (r) {
            var veiculos = $("body").data('veiculos') == null ? new Array() : $("body").data('veiculos');

            for (var i = 0; i < veiculos.length; i++) {
                if (veiculos[i].Codigo == veiculo.Codigo) {
                    veiculos.splice(i, 1);
                    break;
                }
            }

            $("body").data('veiculos', veiculos);

            RenderizarVeiculos();
            LimparCamposVeiculo();
        }
    });
}

function LimparCamposVeiculo() {
    $("#txtPlacaVeiculo").val('');
}

function RenderizarVeiculos() {
    var veiculos = $("body").data('veiculos') == null ? new Array() : $("body").data('veiculos');

    $("#tblVeiculos tbody").html("");

    for (var i = 0; i < veiculos.length; i++) {
        if (!veiculos[i].Excluir)
            $("#tblVeiculos tbody").append("<tr><td>" + veiculos[i].Placa + "</td><td>" + veiculos[i].UF + "</td><td>" + veiculos[i].Renavam + "</td><td>" + veiculos[i].DescricaoTipo + "</td><td>" + veiculos[i].DescricaoTipoRodado + "</td><td>" + veiculos[i].DescricaoTipoCarroceria + "</td><td>" + veiculos[i].Tara + "</td><td>" + veiculos[i].CapacidadeKG + "</td><td>" + veiculos[i].CapacidadeM3 + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirVeiculo(" + JSON.stringify(veiculos[i]) + ")'>Excluir</button></td></tr>");
    }

    if ($("#tblVeiculos tbody").html() == "")
        $("#tblVeiculos tbody").html("<tr><td colspan='10'>Nenhum registro encontrado.</td></tr>");
}