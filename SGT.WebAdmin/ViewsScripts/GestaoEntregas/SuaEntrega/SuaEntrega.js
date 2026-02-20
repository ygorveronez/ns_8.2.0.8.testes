$(document).ready(function () {
    var TipoMensagem = {
        Success: 1,
        Warning: 2,
        Danger: 3
    };
    var $btn = $("#btn-save");

    $btn.on('click', SalvarDados);

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
});