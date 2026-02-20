$(document).ready(function () {
    $("#txtCidadePercentualPorCTe").priceFormat();

    StateCidades = new State({
        name: "Cidades",
        id: "Id",
        render: RenderizarTabelaCidades
    });

    $("#btnAdicionarCidade").click(function () {
        AdicionarCidade();
    });

    $("#selEstado").change(function () {
        BuscarLocalidades($(this).val(), "selCidade", null);
    });
    
    LimparCamposCidade();
    CarregarEstados();
    RenderizarTabelaCidades();
});

var StateCidades;

function LimparCamposCidade() {
    $("#txtCidadePercentualPorCTe").val("0,00");
    $("#selEstado").val($("#selEstado option:first").val());
    $("#selEstado").trigger("change");
}

function AdicionarCidade() {
    var dados = {
        Id: 0,
        Percentual: Globalize.parseFloat($("#txtCidadePercentualPorCTe").val()),
        Estado: $("#selEstado").val(),
        Cidade: parseInt($("#selCidade").val()),
        CidadeDescricao: $("#selCidade option:selected").text().trim(),
    };

    if (dados.Cidade == 0) 
        return ExibirMensagemErro("Nenhuma cidade selecionada.", "Percentual por Cidade.");

    if (!ValidaCidadeAdicionar(dados)) 
        return ExibirMensagemErro("Já existe um percentual com essa cidade.", "Percentual por Cidade.");

    InsereCidade(dados);
    LimparCamposCidade();
}

function InsereCidade(obj) {
    var obj = $.extend({
        Id: 0,
        Percentual: 0,
        Estado: "",
        Cidade: 0,
        CidadeDescricao: "",
        Excluir: false
    }, obj);

    StateCidades.insert(obj);
}

function CarregarEstados() {
    executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {
            RenderizarEstados(r.Objeto, "selEstado");
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function RenderizarEstados(ufs, idSelect) {
    var selUFs = document.getElementById(idSelect);
    selUFs.options.length = 0;

    var optn = document.createElement("option");
    optn.text = 'Selecione';
    optn.value = '';

    selUFs.options.add(optn);
    for (var i = 0; i < ufs.length; i++) {
        var optn = document.createElement("option");
        optn.text = ufs[i].Sigla + " - " + ufs[i].Nome;
        optn.value = ufs[i].Sigla;
        selUFs.options.add(optn);
    }
}

function RenderizarLocalidades(localidades, idSelect, codigo) {
    var selLocalidades = document.getElementById(idSelect);
    selLocalidades.options.length = 0;

    var optn = document.createElement("option");
    optn.text = 'Selecione o Estado';
    optn.value = '';
    selLocalidades.options.add(optn);

    for (var i = 0; i < localidades.length; i++) {
        var optn = document.createElement("option");
        optn.text = localidades[i].Descricao;
        optn.value = localidades[i].Codigo;
        if (codigo != null) {
            if (codigo == localidades[i].Codigo) {
                optn.setAttribute("selected", "selected");
            }
        }
        selLocalidades.options.add(optn);
    }
}

function RenderizarTabelaCidades() {
    var itens = StateCidades.get();
    var $tabela = $("#tblCidades");

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {

            var $row = $("<tr>" +
                "<td>" + info.CidadeDescricao + "</td>" +
                "<td>" + Globalize.format(info.Percentual, "n2") + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Excluir</button></td>" +
                "</tr>");

            $row.on("click", "button", function () {
                ExcluirCidade(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}

function ExcluirCidade(info) {
    StateCidades.remove({ Id: info.Id });
}

function BuscarLocalidades(uf, idSelect, codigo) {
    if (uf == "")
        return RenderizarLocalidades([], idSelect, codigo);
    executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
        if (r.Sucesso) {
            RenderizarLocalidades(r.Objeto, idSelect, codigo);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function ValidaCidadeAdicionar(dados) {
    var valido = true;

    if (StateCidades.get({ Cidade: dados.Cidade }).length > 0)
        valido = false;

    return valido;
}