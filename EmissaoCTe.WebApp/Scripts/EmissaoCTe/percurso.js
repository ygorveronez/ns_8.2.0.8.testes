$(document).ready(function () {

    $("#btnSalvarPercurso").click(function () {
        SalvarPercurso();
    });

    $("#btnExcluirPercurso").click(function () {
        ExcluirPercurso();
    });

    $("#btnCancelarPercurso").click(function () {
        LimparCamposPercurso();
    });

    ObterEstadosPercursos();
});

function ObterEstadosPercursos() {
    executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {

            var selUFPercurso = document.getElementById("selUFPercurso");

            selUFPercurso.options.length = 0;

            var optnTodos = document.createElement("option");
            optnTodos.text = "Todos";
            optnTodos.value = "";

            for (var i = 0; i < r.Objeto.length; i++) {
                var optn = document.createElement("option");
                optn.text = r.Objeto[i].Nome;
                optn.value = r.Objeto[i].Sigla;
                selUFPercurso.options.add(optn.cloneNode(true));
            }

            $("#selUFPercurso").val("");

        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function ValidarCamposPercurso() {
    var percurso = $("#selUFPercurso").val();

    //if (percurso == $("#selUFCarregamento").val() || percurso == $("#selUFDescarregamento").val()) {
    //    ExibirMensagemAlerta("Não é necessário informar um percurso igual ao Estado de carregamento ou descarregamento.", "Atenção!", "placeholder-msgEmissaoMDFe");
    //    return false;
    //}

    var percursos = $("body").data("percursos") == null ? new Array() : $("body").data("percursos");
    var dataPercurso = $("body").data("percurso") != null ? $("body").data("percurso") : {};

    if (percursos.length > 0) {
        for (var i = 0; i < percursos.length; i++) {
            if (percursos[i].Sigla == percurso && percursos[i].Codigo != dataPercurso.Codigo && !percursos[i].Excluir) {
                ExibirMensagemAlerta("Este percurso já foi informado.", "Atenção!", "placeholder-msgEmissaoMDFe");
                return false;
            }
        }
    }

    return true;
}

function ExistePercurso() {
    var percursos = $("body").data("percursos") == null ? new Array() : $("body").data("percursos");

    for (var i = 0; i < percursos.length; i++)
        if (!percursos[i].Excluir)
            return true;

    if (percursos.length > 0)
        return true;

    return false;
}

function SalvarPercurso(percursoGerado) {
    if (ValidarCamposPercurso() || percursoGerado) {
        var percurso = {
            Codigo: $("body").data("percurso") != null ? $("body").data("percurso").Codigo : 0,
            Sigla: $("#selUFPercurso").val(),
            Descricao: $("#selUFPercurso option:selected").text(),
            Excluir: false
        };

        if (percursoGerado)
            percurso = $.extend(percurso, percursoGerado);

        var percursos = $("body").data("percursos") == null ? new Array() : $("body").data("percursos");

        // Se eh um item ja existente, altera o indice especifico para nao alterar a ordem
        // Ou apenas cria o objeto e adiciona no array
        if (percurso.Codigo == 0) {
            percurso.Codigo = BuscaMenorProximoCodigo(percursos, "Codigo");
            percursos.push(percurso);
        } else {
            for (var i = 0; i < percursos.length; i++) {
                if (percursos[i].Codigo == percurso.Codigo) {
                    percursos[i] = percurso;
                    break;
                }
            }
        }

        $("body").data("percursos", percursos);

        RenderizarPercursos();
        LimparCamposPercurso();
    }
}

function EditarPercurso(percurso) {
    $("body").data("percurso", percurso);
    $("#selUFPercurso").val(percurso.Sigla);
    $("#btnExcluirPercurso").show();
}

function ExcluirPercurso() {
    var percurso = $("body").data("percurso");

    var percursos = $("body").data("percursos") == null ? new Array() : $("body").data("percursos");

    for (var i = 0; i < percursos.length; i++) {
        if (percursos[i].Codigo == percurso.Codigo) {
            if (percurso.Codigo <= 0)
                percursos.splice(i, 1);
            else
                percursos[i].Excluir = true;
            break;
        }
    }

    $("body").data("percursos", percursos);

    RenderizarPercursos();
    LimparCamposPercurso();
}

function RenderizarPercursos(disabled) {
    var percursos = $("body").data("percursos") == null ? new Array() : $("body").data("percursos");

    $("#tblPercurso tbody").html("");

    for (var i = 0; i < percursos.length; i++) {
        if (!percursos[i].Excluir)
            $("#tblPercurso tbody").append("<tr><td>" + percursos[i].Descricao + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' " + (disabled ? "disabled" : "") + " onclick='EditarPercurso(" + JSON.stringify(percursos[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblPercurso tbody").html() == "")
        $("#tblPercurso tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposPercurso() {
    $("body").data("percurso", null);
    $("#selUFPercurso").val($("#selUFPercurso option:first").val());
    $("#btnExcluirPercurso").hide();
}