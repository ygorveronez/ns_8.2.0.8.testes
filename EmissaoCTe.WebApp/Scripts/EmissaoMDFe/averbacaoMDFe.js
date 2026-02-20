$(document).ready(function () {
    $("#btnFecharAverbacoes").click(function () {
        FecharTelaAverbacaoMDFe();
    });

    $("#btnReenviarAverbacao").click(function () {
        ReenviarAverbacao();
    });

    $("#btnConsultarAverbacoes").click(function () {
        CarregarAverbacoesMDFe($("body").data("codigoMDFeAverbacao"));
    });
    //document.getElementById('btnReenviarAverbacao').style.visibility = 'hidden';
});

function AbrirTelaAverbacaoMDFe() {
    $("#divAverbacaoMDFe").modal({ keyboard: false });
}

function FecharTelaAverbacaoMDFe() {
    $("#divAverbacaoMDFe").modal('hide');
}

function ReenviarAverbacao() {
    executarRest("/AverbacaoMDFe/ReenviarAverbacao?callback=?", { CodigoMDFe: $("body").data("codigoMDFeAverbacao") }, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Averbação reenviada com sucesso.", "Sucesso!", "placeholder-msgAverbacoes");
            CarregarAverbacoesMDFe($("body").data("codigoMDFeAverbacao"));
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgAverbacoes");
        }
    });
}

function CarregarAverbacoesMDFe(codigoMDFe) {
    executarRest("/AverbacaoMDFe/BuscarPorMDFe?callback=?", { CodigoMDFe: codigoMDFe }, function (r) {
        if (r.Sucesso) {
            CriarGridView(null, null, "tbl_averbacao_table", "tbl_averbacao", "tbl_paginacao_averbacao", null, null, r, null);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgAverbacoes");
        }
    });
}

function ConsultarAverbacaoMDFe(mdfe) {
    CarregarAverbacoesMDFe(mdfe.data.Codigo);
    $("#tituloAverbacaoMDFe").text("Averbações do MDFe-e " + mdfe.data.Numero + " - " + mdfe.data.Serie);
    $("body").data("codigoMDFeAverbacao", mdfe.data.Codigo);
    AbrirTelaAverbacaoMDFe();
}