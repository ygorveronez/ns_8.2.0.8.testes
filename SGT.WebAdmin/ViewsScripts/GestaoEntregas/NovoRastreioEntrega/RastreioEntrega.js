/// <reference path="../../../js/libs/jquery.filedownload.js" />

var TipoMensagem = {
    Success: 1,
    Warning: 2,
    Danger: 3
};

function Feedback(type, title, msg) {
    var typeClass = "";

    if (type == TipoMensagem.Success)
        typeClass = "success";
    else if (type == TipoMensagem.Warning)
        typeClass = "warning";
    else
        typeClass = "danger";

    var $alert = $(
        '<div class="alert alert-' + typeClass + '" style="display: none">' +
        '<div><strong class="title"></strong></div>' +
        '<div class="content"></div>' +
        '</div>'
    );

    $alert.find(".title").text(title);
    $alert.find(".content").text(msg);

    $(".feedback-container")
        .empty()
        .append($alert);

    $alert.slideDown();
}

$(document).ready(function () {
    var $btn = $("#btn-save");
    var $btnSalvarObservacaoAvaliacao = $("#botao-salvar-observacao-avaliacao");

    $btn.on('click', SalvarDados);
    $btnSalvarObservacaoAvaliacao.on('click', SalvarObservacaoAvaliacao);

    $("input[name=evaluate]").on("change", ValidarCampoMotivoAvaliacao);

    function ValidarCampoMotivoAvaliacao() {
        var numeroEstrelas = $("input[name=evaluate]:checked").val();
        if (numeroEstrelas <= 3) {
            $("#motivo-avaliacao").show();
        }
        else {
            $("#motivo-avaliacao").hide();
        }
    }

    function BloqueiaCampos() {
        $btn.hide();
        $("input[name=evaluate]").prop('disabled', true);
        $("textarea[name=observation]").prop('readonly', true);
    }

    function ObterQuestionarioPedido() {
        var respostasAvalicacoes = [];

        $("[data-question]").each(function (i) {
            var $this = $(this);
            var idPergunta = $this.data("question");
            var valPergunta = $("input[name=question-" + idPergunta + "]:checked", $this).val() || null;

            respostasAvalicacoes.push({
                Codigo: idPergunta,
                Resposta: valPergunta
            });
        });

        return respostasAvalicacoes;
    }

    function ObterAvaliacaoPedido() {
        return $("input[name=evaluate]:checked").val() || null;
    }

    function SalvarObservacaoAvaliacao() {     
        let $textarea = $("#textarea-observacao-avaliacao");
        let observacao = $textarea.val();

        var data = {
            Token: page_data.Token,
            Observacao: observacao
        };

        $.post("/sua-entrega/SalvarObservacaoAvaliacao", data)
            .done(function (r) {
                $textarea.attr('disabled', true);
                $btnSalvarObservacaoAvaliacao.prop('disabled', true);
                $btnSalvarObservacaoAvaliacao.hide();
                $("#salvar-observacao-avaliacao").hide();
            })
            .fail(function () {
                Feedback(TipoMensagem.Danger, "Falha ao salvar os dados", "Ocorreu uma falha ao salvar a observação.");
            });
    }

    function SalvarDados() {
        $btn.prop('disabled', true);

        var data = {
            Questionario: JSON.stringify(ObterQuestionarioPedido()),
            Avaliacao: ObterAvaliacaoPedido(),
            Observacao: $("textarea[name=observation]").val(),
            MotivoAvaliacao: $("select[name=select-motivo-avaliacao]").val(),
            Token: page_data.Token
        };

        $.post(page_data.URL, data)
            .done(function (response) {
                $btn.prop('disabled', false);
                var res = JSON.parse(response);

                if ("Msg" in res && res.Msg != "" && res.Msg != null) {
                    Feedback(TipoMensagem.Warning, "Falha ao salvar os dados", res.Msg);
                } else {
                    Feedback(TipoMensagem.Success, "Dados salvos com sucesso", "Agradecemos o feedback.");
                    BloqueiaCampos();
                }
            })
            .fail(function () {
                Feedback(TipoMensagem.Danger, "Falha ao salvar os dados", "Ocorreu uma falha ao salvar o feedback.");
            });
    }

    var listaTrDetalhePedido = document.querySelectorAll("#detalhe-pedido tr");
    listaTrDetalhePedido.forEach((element, index) => { element.classList.toggle("table-secondary", index % 2 == 0); });
});

function downloadAnexoClick(codigoAnexo) {
    var dados = { Codigo: codigoAnexo };
    executarDownload("DownloadAnexo", dados);
}

function executarDownload(relativeUrl, dados, sucessCallback, errorCallback, exibirLoading, executarComoMetodoPost) {

    if (!$.fileDownload) return;

    if (exibirLoading == null)
        exibirLoading = true;

    var isGet = !executarComoMetodoPost;
    
    $.fileDownload(relativeUrl, {
        httpMethod: isGet ? 'GET' : 'POST',
        data: dados,
        prepareCallback: function (url) {
            //iniciarRequisicao();
        },
        successCallback: function (url) {
            //finalizarRequisicao();
            if (sucessCallback) {
                sucessCallback(url);
            }
        },
        failCallback: function (html, url) {
            //finalizarRequisicao();
            if (errorCallback) {
                errorCallback(html, url);
            } else {
                try {

                    if ((/^<pre/i).test(html)) //hack para quando vem a tag <pre> (em alguns casos retorna <pre>{...}</pre>, acredito que devido ao server)
                        html = $(html).text();

                    var retorno = JSON.parse(html.replace("(", "").replace(");", ""));

                    if (retorno.Success) {
                        Feedback(TipoMensagem.Warning, "Atenção", retorno.Msg);
                    } else {
                        Feedback(TipoMensagem.Danger, "Falha", retorno.Msg);
                    }
                } catch (ex) {
                    Feedback(TipoMensagem.Danger, "Atenção", "Não foi possível realizar o download do arquivo. Atualize a página e tente novamente.");
                }
            }
        }
    });
    return false;
}