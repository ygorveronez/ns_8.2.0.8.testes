$(document).ready(function () {
    $("#txtCustosAdicionais, #txtFormaCobranca, #txtCTRN").ckeditor();

    // Persiste valor inteiro
    $("#txtDiasValidade").change(function () {
        if (isNaN($("#txtDiasValidade").val()))
            $("#txtDiasValidade").val('');
        else
            $("#txtDiasValidade").val(parseInt($("#txtDiasValidade").val()));
    });

    $("#btnSalvar").on("click", function () {
        Salvar();
    });

    BuscaConfiguracoes();
});

function Salvar() {
    var dados = {
        TextoCustosAdicionais: $("#txtCustosAdicionais").val(),
        TextoFormaCobranca: $("#txtFormaCobranca").val(),
        TextoCTRN: $("#txtCTRN").val(),
        DiasValidade: $("#txtDiasValidade").val()
    };

    executarRest("/PropostaConfiguracao/Salvar?callback=?", dados, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function BuscaConfiguracoes() {
    executarRest("/PropostaConfiguracao/ObterDetalhes?callback=?", {}, function (r) {
        if (r.Sucesso) {
            $("#txtCustosAdicionais").val(r.Objeto.TextoCustosAdicionais);
            $("#txtFormaCobranca").val(r.Objeto.TextoFormaCobranca);
            $("#txtCTRN").val(r.Objeto.TextoCTRN);
            $("#txtDiasValidade").val(r.Objeto.DiasValidade);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}