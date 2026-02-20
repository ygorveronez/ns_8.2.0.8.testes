$(document).ready(function () {
    $("#btnFecharAverbacoes").click(function () {
        FecharTelaAverbacaoNFSe();
    });

    $("#btnReenviarAverbacao").click(function () {
        ReenviarAverbacao();
    });

    $("#btnConsultarAverbacoes").click(function () {
        CarregarAverbacoesNFSe($("body").data("codigoNFSeAverbacao"));
    });

});

function AbrirTelaAverbacaoNFSe() {
    $("#divAverbacaoNFSe").modal({ keyboard: false });
}

function FecharTelaAverbacaoNFSe() {
    $("#divAverbacaoNFSe").modal('hide');
}

function ReenviarAverbacao() {
    executarRest("/NotaFiscalDeServicosEletronica/ReenviarAverbacao?callback=?", { CodigoNFSe: $("body").data("codigoNFSeAverbacao") }, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Averbação reenviada com sucesso.", "Sucesso!", "placeholder-msgAverbacoes");
            CarregarAverbacoesNFSe($("body").data("codigoNFSeAverbacao"));
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgAverbacoes");
        }
    });
}

function CarregarAverbacoesNFSe(codigoNFSe) {
    executarRest("/NotaFiscalDeServicosEletronica/BuscarAverbacao?callback=?", { CodigoNFSe: codigoNFSe }, function (r) {
        if (r.Sucesso) {
            CriarGridView(null, null, "tbl_averbacao_table", "tbl_averbacao", "tbl_paginacao_averbacao", null, null, r, null);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgAverbacoes");
        }
    });
}

function ConsultarAverbacaoNFSe(nfse) {
    CarregarAverbacoesNFSe(nfse.data.Codigo);
    $("#tituloAverbacaoNFSe").text("Averbações do NFSe-e " + nfse.data.Numero + " - " + nfse.data.Serie);
    $("body").data("codigoNFSeAverbacao", nfse.data.Codigo);
    AbrirTelaAverbacaoNFSe();
}