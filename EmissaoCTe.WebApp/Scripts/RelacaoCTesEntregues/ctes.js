$(document).ready(function () {
    $("#txtChave").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");
    CarregarConsultaDeCTesPortal("btnBuscarCTe", "btnBuscarCTe", SalvarCTePortal, true);

    $("#btnCaptchaCTeSefaz").click(function () {
        EnviarCaptchaSefaz();
    });

    $("#btnAtualizarCaptchaCTeSefaz").click(function () {
        AtualizarCaptchaCTeSefaz();
    });

    $("a[href=#divCTes]").on('shown.bs.tab', function () {
        $("#txtChave").focus();
    });

    StateCTes = new State({
        name: "ctes",
        id: "Id",
        render: RenderizarCTes
    });

    StateCTes.__proto__.proximaOrdem = function () {
        var data = this.get();
        var proximaOrdem = 0;
        for (var i in data) {
            if (data[i].Ordem > proximaOrdem)
                proximaOrdem = data[i].Ordem;
        }

        return (proximaOrdem + 1);
    };

    var KEY_CODE_ENTER = 13;
    var KEY_CODE_CTRL  = 17;
    $("#txtChave").keyup(function (e) {
        if (e.which != KEY_CODE_CTRL && e.keyCode != KEY_CODE_CTRL) {
            var cte = ObjetoCTe();
            //if (cte.Chave.length == 44)
            //    SalvarCTe();
        }
    });

    $("#txtChave").keyup(function (e) {
        if (e.which == KEY_CODE_ENTER || e.keyCode == KEY_CODE_ENTER) {
            SalvarCTe();
        }
    });

    $("#txtCaptchaCTeSefaz").keyup(function (e) {
        if (e.which == KEY_CODE_ENTER || e.keyCode == KEY_CODE_ENTER) {
            EnviarCaptchaSefaz();
        }
    });
    $modalCaptcha = $('#divCaptchaCTeSefaz');
});

var StateCTes;
var STATE_CONSULTA_CTE_SEFAZ = null;
var $modalCaptcha;

function LimparCTes() {
    StateCTes.clear();
    StateCTes.set([]);
    LimparCamposCTe();
}

function LimparCamposCTe() {
    $("#txtChave").val("");
    $("#btnExcluirChave").hide();
}

function ObjetoCTe() {
    return cte = {
        Chave: $("#txtChave").val().replace(/[^0-9]/g, '')
    };
}

function EnviarCaptchaSefaz() {
    if (ValidarCaptchaSefaz()) {
        // Limpa validações
        CampoSemErro("#txtCaptchaCTeSefaz");

        // Seta no state da consulta o captcha digitado
        STATE_CONSULTA_CTE_SEFAZ.Captcha = $("#txtCaptchaCTeSefaz").val();
        STATE_CONSULTA_CTE_SEFAZ.Chave = ObjetoCTe().Chave;

        // Envia captcha
        executarRest("/RelacaoCTesEntregues/InformarCaptchaCTeSefaz?callback=?", STATE_CONSULTA_CTE_SEFAZ, function (r) {
            if (r.Sucesso) {
                InsereCTe(r.Objeto.CTe);
                LimparCamposCTe();
                $("#txtChave").focus();

                $modalCaptcha.one('hidden.bs.modal', function () {
                    // Foca no campo
                    $("#txtChave").focus();
                });

                $modalCaptcha.modal('hide');
            } else {
                jAlert(r.Erro, "Retorno Consulta de NF-e Sefaz", function () {
                    AtualizarCaptchaCTeSefaz();
                });
            }
        });
    }
}

function SalvarCTePortal(cte) {
    $("#txtChave").val(cte.Chave).trigger("blur");
    SalvarCTe();
}

function SalvarCTe() {
    var erros = ValidaCTe();
    if (erros.length == 0) {
        var cte = ObjetoCTe();

        RequisitaDadosCTe(cte);
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        // Limpa quaisquer erros existentes
        $("#placeholder-validacao-cte").html("");

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:", "placeholder-validacao-cte");
    }
}

function CriarCaptcha(cte) {
    $("#txtCaptchaCTeSefaz").val("");
    // Exibe captcha 
    executarRest("/RelacaoCTesEntregues/ConsultarCTeeSefaz?callback=?", cte, function (r) {
        if (r.Sucesso && r.Objeto != null && r.Objeto.DadosConsultar != null) {
            // Guarda o estado da requisição
            STATE_CONSULTA_CTE_SEFAZ = r.Objeto.DadosConsultar;

            // Coloca a img do captcha
            $('#imgCaptcha').attr('src', r.Objeto.DadosConsultar.imgCaptcha);

            // Se o modal ja esta aberto, foca
            if (($modalCaptcha.data('bs.modal') || {}).isShown) {
                // Foca no campo
                $("#txtCaptchaCTeSefaz").focus();
            } else {
                // Exibe modal
                $modalCaptcha.modal('show');

                // Caso contrário, espera abrir pra focar
                $modalCaptcha.one('shown.bs.modal', function () {
                    // Foca no campo
                    $("#txtCaptchaCTeSefaz").focus();
                });
            }
        } else {
            if (r != null && r.Erro != null)
                jAlert(r.Erro, "Consulta de NF-e Sefaz");
            else
                jAlert("Não foi possível consultar o CT-e, favor tentar novamente.", "Consulta de CT-e Sefaz");
        }
    });
}

function AtualizarCaptchaCTeSefaz() {
    var cte = ObjetoCTe();

    CriarCaptcha(cte);
}

function RequisitaDadosCTe(cte) {
    // Envia chave para consultar CT-e
    executarRest("/RelacaoCTesEntregues/BuscarDadosPorChave?callback=?", cte, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.ChaveExistente) {
                InsereCTe(r.Objeto.CTe);
                LimparCamposCTe();
                $("#txtChave").focus();
            } else {
                $("#txtCaptchaCTeSefaz").val("");

                CriarCaptcha(cte);
            }
        } else {
            ExibirMensagemErro(r.Erro, "Erro ao consultar chave", "placeholder-validacao-cte");
        }
    });
}

function ValidarCaptchaSefaz() {
    var captcha = $("#txtCaptchaCTeSefaz").val();
    var valido = true;

    if (captcha == null || captcha == "") {
        CampoComErro("#txtCaptchaCTeSefaz");
        valido = false;
    } else {
        CampoSemErro("#txtCaptchaCTeSefaz");
    }

    if (!valido)
        jAlert('Captcha não informado.', "Consulta de CT-e Sefaz");

    return valido;
}

function ValidaCTe() {
    var valido = [];

    var chave = $("#txtChave").val().replace(/[^0-9]/g, '');

    if (chave == "") {
        valido.push("Chave do CTe é obrigatório.");
        CampoComErro($("#txtChave"));
    } else {
        CampoSemErro($("#txtChave"));
    }

    var duplicado = false;
    var numeroCte = 0;
    StateCTes.get().forEach(function (info) {
        if (!info.Excluir) {
            if (chave != "" && info.Chave == chave)
            {
                duplicado = true;
                numeroCte = info.Numero;
            }
        }
    });

    if (duplicado)
        valido.push("Chave do CT-e já existe (Nº " + numeroCte + ").");

    return valido;
}

function InsereCTe(obj) {
    obj = $.extend({
        Id: 0,
        CTe: 0, 
        Destinatario: "", 
        Peso: 0, 
        ValorAReceber: 0,
        Chave: "",
        CNPJEmitente: "",
        Excluir: false
    }, obj);

    obj.Ordem = StateCTes.proximaOrdem();

    StateCTes.insert(obj);

    CalculaTotalCTes();
}

function ExcluirCTe(info) {
    StateCTes.remove({ Id: info.Id });

    // Alterar a ordem dos itens remandescentes
    var reordem = 0;
    var data = StateCTes.get().map(function (item) {
        item.Ordem = item.Excluir ? 0 : ++reordem;
        return item;
    });

    LimparCamposCTe();
    CalculaTotalCTes();
}

function RenderizarCTes() {
    var itens = StateCTes.get();
    var $tabela = $("#tblCTes");

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {
            var $row = $("<tr>" +
                "<td>" + info.Numero + "</td>" +
                "<td>" + info.Emitente + "</td>" +
                "<td>" + info.DataEmissao + "</td>" +
                "<td>" + info.TerminoPrestacao + "</td>" +
                "<td>" + Globalize.format(info.ValorAReceber, "n2") + "</td>" +
                "<td>" + Globalize.format(info.Peso, "n4") + "</td>" +
                ((STATUS_RELACAO_ABERTA == STATUS_RELACAO.Aberto || STATUS_RELACAO_ABERTA == STATUS_RELACAO.Todas) ? "<td><button type='button' class='btn btn-default btn-xs btn-block'>Excluir</button></td>" : "") +
                "</tr>");

            $row.on("click", "button", function () {
                ExcluirCTe(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}
function CTesGrid() {
    return StateCTes.toJson();
}