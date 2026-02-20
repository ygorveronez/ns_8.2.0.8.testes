$(document).ready(function () {
    $("#btnFecharAverbacoes").click(function () {
        FecharTelaAverbacaoCTe();
    });

    $("#btnReenviarAverbacao").click(function () {
        ReenviarAverbacao();
    });

    $("#btnConsultarAverbacoes").click(function () {
        CarregarAverbacoesCTe($("body").data("codigoCTeAverbacao"));
    });
});

function AbrirTelaAverbacaoCTe() {
    $("#divAverbacaoCTe").modal({ keyboard: false });
}

function FecharTelaAverbacaoCTe() {
    $("#divAverbacaoCTe").modal('hide');
}

function ReenviarAverbacao() {
    executarRest("/AverbacaoCTe/ReenviarAverbacao?callback=?", { CodigoCTe: $("body").data("codigoCTeAverbacao") }, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Averbação reenviada com sucesso.", "Sucesso!", "placeholder-msgAverbacoes");
            CarregarAverbacoesCTe($("body").data("codigoCTeAverbacao"));
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgAverbacoes");
        }
    });
}

function CarregarAverbacoesCTe(codigoCTe) {
    executarRest("/AverbacaoCTe/BuscarPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            var opcoesAverbacoes = [
                { Descricao: "Copiar", Evento: CopiarDadosAverbacao }
            ];
            CriarGridView(null, null, "tbl_averbacao_table", "tbl_averbacao", "tbl_paginacao_averbacao", opcoesAverbacoes, [0], r, null);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgAverbacoes");
        }
    });
}

function ConsultarAverbacaoCTe(cte) {
    CarregarAverbacoesCTe(cte.data.Codigo);
    $("#tituloAverbacaoCTe").text("Averbações do CT-e " + cte.data.Numero + " - " + cte.data.Serie);
    $("body").data("codigoCTeAverbacao", cte.data.Codigo);
    AbrirTelaAverbacaoCTe();
}

function CopiarDadosAverbacao(evt) {
    var dado = evt.data;
    var valor = dado.NumeroAverbacao || dado.Protocolo;

    // Cria o modal
    var $modal = $(
        '<div class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">' +
            '<div class="modal-dialog modal-sm">' +
                '<div class="modal-content">' +
                    '<div class="modal-header">' +
                        '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                        '<h4 class="modal-title">Dado de Averbação CT-e</h4>' +
                    '</div>' +
                    '<div class="modal-body">' +
                        '<div class="row">' +
                            '<div class="col-xs-6 col-sm-12"><input type="text" readonly class="form-control" /></div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>' +
        '</div>'
    );
    var $input = $modal.find('input');

    // Insere no body
    $('body').append($modal);

    // Colocar o valor
    $input.val(valor);

    // Adiciona evento
    var _selecionaTudo = function () {
        $input[0].select();
    };
    var _removeDOM = function () {
        $modal.remove();
    };
    // - Remove quando fechado
    $modal.one('hidden.bs.modal', _removeDOM);
    $modal.one('shown.bs.modal', _selecionaTudo);

    // - Seleciona texto quando carregado
    $input.on('focus', _selecionaTudo);

    // Abre modal
    $modal.modal();
}