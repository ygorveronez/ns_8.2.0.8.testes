$(document).ready(function () {
    $("#btnSalvarProdutoPerigoso").click(function () {
        SalvarProdutoPerigoso();
    });
    $("#btnCancelarProdutoPerigoso").click(function () {
        LimparCamposProdutosPerigosos();
    });
    $("#btnExcluirProdutoPerigoso").click(function () {
        ExcluirProdutoPerigoso();
    });
    $("#btnSalvarAlteracoesProdutoPerigoso").click(function () {
        SalvarAlteracoesProdutoPerigoso();
    });

    $("#divProdutosPerigosos").on("hide.bs.modal", function (e) {
        if (VerificarFechamentoProdutosPerigosos && HouveEdicaoProdutosPerigosos) {
            e.preventDefault();
            e.stopImmediatePropagation();

            jConfirm('Tem certeza que deseja descartar as alterações?', 'Confirmar Fechamento', function (r) {
                if (r) {
                    VerificarFechamentoProdutosPerigosos = false;
                    $("#divProdutosPerigosos").modal('hide');
                }
            });
        } else {
            ConfirmaFecharModal();
        }
    });
});

var StateProdutosPerigosos;
var IdProdutoPerigosoEdicao = 0;
var HouveEdicaoProdutosPerigosos = false;
var VerificarFechamentoProdutosPerigosos = true;
var CallbackProdutosPerigososCTe = function () { };
var CTeProdutosPerigosos = {};

function ConfirmaFecharModal() {
    $("#divDocumentosMunicipioDescarregamento").modal('show');
    FecharTelaProdutosPerigososCTe();
}

function AbrirTelaProdutosPerigososCTe(info, cb) {
    $("#spnCTeProdutosPerigosos").text(info.CTe.Numero);    
    $("#divDocumentosMunicipioDescarregamento").modal('hide');
    $("#divProdutosPerigosos").modal({ keyboard: false, backdrop: 'static' });

    StateProdutosPerigosos = new State({
        name: "ProdutosPerigosos",
        id: "Id",
        render: RenderizarProdutosPerigosos
    });
    VerificarFechamentoProdutosPerigosos = true;
    CallbackProdutosPerigososCTe = cb;
    CTeProdutosPerigosos = info;

    // Adiciona os produtos do document
    if (typeof info.CTe.ProdutosPerigosos != "undefined") {
        // Negativa Id para habilitar distincao entre edicao local e entidades
        var produtos = info.CTe.ProdutosPerigosos;
        for (var p in produtos)
            if (produtos[p].Id == 0)
                produtos[p].Id = -(++p);
        StateProdutosPerigosos.set(produtos);
    }

    StateProdutosPerigosos.render();
}

function FecharTelaProdutosPerigososCTe() {
    $("#spnCTeProdutosPerigosos").text('');
    LimparCamposProdutosPerigosos();
    StateProdutosPerigosos.clear();
    StateProdutosPerigosos = null;
    HouveEdicaoProdutosPerigosos = false;
    CallbackProdutosPerigososCTe = function () { };
    CTeProdutosPerigosos = {};
}

function LimparCamposProdutosPerigosos() {
    IdProdutoPerigosoEdicao = 0;
    $("#txtNumeroONU").val('');
    $("#txtNomeApropriadoEmbarqueProduto").val('');
    $("#txtClasseRisco").val('');
    $("#txtGrupoEmbalagem").val('');
    $("#txtQuantidadeTotalPorProduto").val('');
    $("#txtQuantidadeETipoDeVolumes").val('');
    $("#btnExcluirProdutoPerigoso").hide();

    // Remove validacoes
    CampoSemErro($("#txtNumeroONU"));
    CampoSemErro($("#txtQuantidadeTotalPorProduto"));
}

function EditarProdutoPerigoso(info) {
    IdProdutoPerigosoEdicao = info.Id;
    $("#txtNumeroONU").val(info.NumeroONU);
    $("#txtNomeApropriadoEmbarqueProduto").val(info.NomeApropriado);
    $("#txtClasseRisco").val(info.ClasseRisco);
    $("#txtGrupoEmbalagem").val(info.GrupoEmbalagem);
    $("#txtQuantidadeTotalPorProduto").val(info.QuantidadeTotal);
    $("#txtQuantidadeETipoDeVolumes").val(info.QuantidadeETipo);
    $("#btnExcluirProdutoPerigoso").show();
}

function ExcluirProdutoPerigoso() {
    StateProdutosPerigosos.remove({ Id: IdProdutoPerigosoEdicao });
    LimparCamposProdutosPerigosos();
    HouveEdicaoProdutosPerigosos = true;
}

function SalvarProdutoPerigoso() {
    var erros = ValidarProdutoPerigoso();

    if (erros.length == 0) {
        var produto = {
            Id: IdProdutoPerigosoEdicao,
            NumeroONU: $("#txtNumeroONU").val(),
            NomeApropriado: $("#txtNomeApropriadoEmbarqueProduto").val(),
            ClasseRisco: $("#txtClasseRisco").val(),
            GrupoEmbalagem: $("#txtGrupoEmbalagem").val(),
            QuantidadeTotal: $("#txtQuantidadeTotalPorProduto").val(),
            QuantidadeETipo: $("#txtQuantidadeETipoDeVolumes").val()
        };

        InsereProdutoPerigoso(produto);
        LimparCamposProdutosPerigosos();
        HouveEdicaoProdutosPerigosos = true;
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        // Limpa quaisquer erros existentes
        $("#placeholder-validacao-seguro").html("");

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:", "placeholder-msgProdutosPerigososCTe");
    }
}

function ValidarProdutoPerigoso() {
    var valido = [];

    if ($("#txtNumeroONU").val() == "") {
        valido.push("Número ONU/UN é obrigatório.");
        CampoComErro($("#txtNumeroONU"));
    } else {
        CampoSemErro($("#txtNumeroONU"));
    }

    if ($("#txtQuantidadeTotalPorProduto").val() == "") {
        valido.push("Quantidade total por produto é obrigatório.");
        CampoComErro($("#txtQuantidadeTotalPorProduto"));
    } else {
        CampoSemErro($("#txtQuantidadeTotalPorProduto"));
    }

    return valido;
}

function SalvarAlteracoesProdutoPerigoso() {
    CallbackProdutosPerigososCTe(CTeProdutosPerigosos, StateProdutosPerigosos.get());
    VerificarFechamentoProdutosPerigosos = false;
    $("#divProdutosPerigosos").modal('hide');
}

function InsereProdutoPerigoso(obj) {
    var obj = $.extend({
        Id: 0,
        NumeroONU: '',
        NomeApropriado: '',
        ClasseRisco: '',
        GrupoEmbalagem: '',
        QuantidadeTotal: '',
        QuantidadeETipo: '',
        Excluir: false
    }, obj);

    if (obj.Id != 0)
        StateProdutosPerigosos.update(obj);
    else
        StateProdutosPerigosos.insert(obj);
}

function RenderizarProdutosPerigosos() {
    var itens = StateProdutosPerigosos.get();
    var $tabela = $("#tblProdutosPerigosos");
    var $tbody = $tabela.find("tbody");
    var $rows = [];

    $tbody.html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {

            var $row = $("<tr>" +
                "<td>" + info.NumeroONU + "</td>" +
                "<td>" + info.NomeApropriado + "</td>" +
                "<td>" + info.ClasseRisco + "</td>" +
                "<td>" + info.GrupoEmbalagem + "</td>" +
                "<td>" + info.QuantidadeTotal + "</td>" +
                "<td>" + info.QuantidadeETipo + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Editar</button></td>" +
            "</tr>");

            $row.on("click", "button", function () {
                EditarProdutoPerigoso(info);
            });

            $rows.push($row);
        }
    });

    $tbody.append.apply($tbody, $rows);

    if ($tbody.find("tr").length == 0)
        $tbody.html("<tr><td colspan='" + $tabela.find("thead th").length + "' class='text-center'>Nenhum registro encontrado.</td></tr>");
}