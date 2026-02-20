$(document).ready(function () {
    $("#btnSalvarProdutoPerigoso").click(function () {
        SalvarProdutoPerigoso();
    });
    $("#btnExcluirProdutoPerigoso").click(function () {
        ExcluirProdutoPerigoso();
    });
    $("#btnCancelarProdutoPerigoso").click(function () {
        LimparCamposProdutoPerigoso();
    });
});
function LimparCamposProdutoPerigoso() {
    $("#hddProdutoPerigosoEmEdicao").val("0");
    $("#txtNumeroONU").val("");
    $("#txtNomeApropriadoEmbarqueProduto").val("");
    $("#txtClasseRisco").val("");
    $("#txtGrupoEmbalagem").val("");
    $("#txtQuantidadeTotalPorProduto").val("");
    $("#txtQuantidadeETipoDeVolumes").val("");
    $("#txtPontoDeFulgor").val("");
    $("#btnExcluirProdutoPerigoso").hide();
    $("#btnCancelarProdutoPerigoso").hide();
}
function ValidarCamposProdutoPerigoso() {
    var numeroONU = $("#txtNumeroONU").val();
    var nomeProduto = $("#txtNomeApropriadoEmbarqueProduto").val();
    var classeRisco = $("#txtClasseRisco").val();
    var quantidadeTotal = $("#txtQuantidadeTotalPorProduto").val();
    var valido = true;
    if (numeroONU != "") {
        CampoSemErro("#txtNumeroONU");
    } else {
        CampoComErro("#txtNumeroONU");
        valido = false;
    }
    if (nomeProduto != "") {
        CampoSemErro("#txtNomeApropriadoEmbarqueProduto");
    } else {
        CampoComErro("#txtNomeApropriadoEmbarqueProduto");
        valido = false;
    }
    if (classeRisco != "") {
        CampoSemErro("#txtClasseRisco");
    } else {
        CampoComErro("#txtClasseRisco");
        valido = false;
    }
    if (quantidadeTotal != "") {
        CampoSemErro("#txtQuantidadeTotalPorProduto");
    } else {
        CampoComErro("#txtQuantidadeTotalPorProduto");
        valido = false;
    }
    return valido;
}
function SalvarProdutoPerigoso() {
    if (ValidarCamposProdutoPerigoso()) {
        var produto = {
            Codigo: Globalize.parseInt($("#hddProdutoPerigosoEmEdicao").val()),
            NumeroONU: $("#txtNumeroONU").val().toUpperCase(),
            ClasseRisco: $("#txtClasseRisco").val(),
            GrupoEmbalagem: $("#txtGrupoEmbalagem").val(),
            QuantidadeTotal: $("#txtQuantidadeTotalPorProduto").val(),
            QuantidadeETipo: $("#txtQuantidadeETipoDeVolumes").val(),
            PontoDeFulgor: $("#txtPontoDeFulgor").val(),
            NomeApropriado: $("#txtNomeApropriadoEmbarqueProduto").val(),
            Excluir: false
        };
        var produtos = $("#hddProdutosPerigosos").val() == "" ? new Array() : JSON.parse($("#hddProdutosPerigosos").val());
        if (produto.Codigo == 0)
            produto.Codigo = -(produtos.length + 1);
        if (produtos.length > 0) {
            for (var i = 0; i < produtos.length; i++) {
                if (produtos[i].Codigo == produto.Codigo) {
                    produtos.splice(i, 1);
                    break;
                }
            }
        }
        produtos.push(produto);
        produtos.sort();
        $("#hddProdutosPerigosos").val(JSON.stringify(produtos));
        RenderizarProdutoPerigoso();
        LimparCamposProdutoPerigoso();
    }
}
function EditarProdutoPerigoso(produto) {
    $("#hddProdutoPerigosoEmEdicao").val(produto.Codigo);
    $("#txtNumeroONU").val(produto.NumeroONU);
    $("#txtClasseRisco").val(produto.ClasseRisco);
    $("#txtGrupoEmbalagem").val(produto.GrupoEmbalagem);
    $("#txtQuantidadeTotalPorProduto").val(produto.QuantidadeTotal);
    $("#txtQuantidadeETipoDeVolumes").val(produto.QuantidadeETipo);
    $("#txtPontoDeFulgor").val(produto.PontoDeFulgor);
    $("#txtNomeApropriadoEmbarqueProduto").val(produto.NomeApropriado);
    $("#btnExcluirProdutoPerigoso").show();
    $("#btnCancelarProdutoPerigoso").show();
}
function ExcluirProdutoPerigoso() {
    jConfirm("Deseja realmente excluir este produto perigoso?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddProdutoPerigosoEmEdicao").val());
            var produtos = $("#hddProdutosPerigosos").val() == "" ? new Array() : JSON.parse($("#hddProdutosPerigosos").val());
            for (var i = 0; i < produtos.length; i++) {
                if (produtos[i].Codigo == codigo) {
                    if (codigo > 0) {
                        produtos[i].Excluir = true;
                    } else {
                        produtos.splice(i, 1);
                    }
                    break;
                }
            }
            $("#hddProdutosPerigosos").val(JSON.stringify(produtos));
            RenderizarProdutoPerigoso();
            LimparCamposProdutoPerigoso();
        }
    });
}
function RenderizarProdutoPerigoso() {
    $("#tblProdutosPerigosos tbody").html("");
    var produtos = $("#hddProdutosPerigosos").val() == "" ? new Array() : JSON.parse($("#hddProdutosPerigosos").val());
    for (var i = 0; i < produtos.length; i++) {
        if (!produtos[i].Excluir) {
            $("#tblProdutosPerigosos tbody").append("<tr class='linha'><td>" + produtos[i].NumeroONU + "</td><td>" + produtos[i].NomeApropriado + "</td><td>" + produtos[i].ClasseRisco + "</td><td>" + produtos[i].GrupoEmbalagem + "</td><td>" + produtos[i].QuantidadeTotal + "</td><td>" + produtos[i].QuantidadeETipo + "</td><td>" + produtos[i].PontoDeFulgor + "</td><td><a onclick='EditarProdutoPerigoso(" + JSON.stringify(produtos[i]) + ")'>Editar</a></td></tr>");
        }
    }
    if ($("#tblProdutosPerigosos tbody").html() == "") {
        $("#tblProdutosPerigosos tbody").html("<tr class='linha'><td colspan='8'>Nenhum registro encontrado!</td></tr>");
    }
}