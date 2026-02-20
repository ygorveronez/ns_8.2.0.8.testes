$(document).ready(function () {
    $("#btnSalvarTomador").click(function () {
        SalvarTomador();
    });

    $("#btnCancelarTomador").click(function () {
        LimparCamposTomador();
    });

    $("#btnExcluirTomador").click(function () {
        ExcluirTomador();
    });

    $("#divEmissaoMDFe").on("hide.bs.modal", function () {
        StateTomadores.clear();
    });

    RemoveConsulta("#txtCNPJTomador", LimparCamposTomador);
    $("#txtCNPJTomador").data("CPFCNPJ", "");
    CarregarConsultadeClientes("btnBuscarTomador", "btnBuscarTomador", RetornoConsultaTomador, true, false);

    StateTomadores = new State({
        name: "tomadores",
        id: "Id",
        render: RenderizarTomadores
    });
});

var StateTomadores;

function LimparCamposTomador(){
    $("#txtCNPJTomador").val("");
    $("#txtCNPJTomador").data("CPFCNPJ", "");
    CampoSemErro($("#txtCNPJTomador"));
}

function RetornoConsultaTomador(tomador) {
    var descricaoTomador = (tomador.CPFCNPJ + " - " + tomador.Nome);
    $("#txtCNPJTomador").val( descricaoTomador );
    $("#txtCNPJTomador").data("CPFCNPJ", tomador.CPFCNPJ.replace(/[^0-9]/g, ''));
}

function SalvarTomador() {
    if (ValidaTomador()) {
        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: $("#txtCNPJTomador").data("CPFCNPJ") }, function (r) {
            if (r.Sucesso) {
                var tomador = {
                    Nome: r.Objeto.Nome,
                    CPF_CNPJ: r.Objeto.CPF_CNPJ,
                };

                InsereTomador(tomador);
                LimparCamposTomador();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção");
            }
        });
    }
}

function ValidaTomador() {
    var valido = true;
    if ($("#txtCNPJTomador").data("CPFCNPJ") == "") {
        valido = false;
        CampoComErro($("#txtCNPJTomador"));
    } else {
        CampoSemErro($("#txtCNPJTomador"));
    }

    return valido;
}

function InsereTomador(obj) {
    var obj = $.extend({
        Id: 0,
        Nome: 0,
        CPF_CNPJ: "",
        Excluir: false
    }, obj);

    obj.CPF_CNPJ = obj.CPF_CNPJ.replace(/[^0-9]/g, '');

    if (obj.Id != 0)
        StateTomadores.update(obj);
    else
        StateTomadores.insert(obj);
}

function ExcluirTomador(info) {
    StateTomadores.remove({ Id: info.Id });
}

function RenderizarTomadores() {
    var tomadores = StateTomadores.get();
    var $tabela = $("#tblTomadores");

    $tabela.find("tbody").html("");

    $rows = tomadores.forEach(function (info) {
        var cpfcnpj;

        if (info.CPF_CNPJ != undefined) {
            if (info.CPF_CNPJ.length == 14)
                cpfcnpj = FormataMascara(info.CPF_CNPJ, "##.###.###/####-##");
            else if (info.CPF_CNPJ.length == 11)
                cpfcnpj = FormataMascara(info.CPF_CNPJ, "###.###.###-##");
        }

        if (!info.Excluir) {
            var $row = $("<tr>" +
                "<td>" + info.Nome + "</td>" +
                "<td>" + cpfcnpj + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Excluir</button></td>" +
            "</tr>");

            $row.on("click", "button", function () {
                ExcluirTomador(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}