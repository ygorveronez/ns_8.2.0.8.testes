function RenderizarVeiculosVinculados() {
    $("#tblVeiculosVinculados tbody").html("");
    var veiculos = $("#hddVeiculosVinculados").val() == "" ? new Array() : JSON.parse($("#hddVeiculosVinculados").val());
    for (var i = 0; i < veiculos.length; i++) {
        if (!veiculos[i].Excluir)
            $("#tblVeiculosVinculados tbody").append("<tr><td>" + veiculos[i].Placa + "</td><td>" + veiculos[i].Renavam + "</td>><td>" + veiculos[i].DescricaoTipoVeiculo + "</td></tr>");
    }
    if ($("#tblVeiculosVinculados tbody").html() == "")
        $("#tblVeiculosVinculados tbody").html("<td colspan=\"3\" class=\"text-center\">Nenhum registro encontrado.</td>");
}