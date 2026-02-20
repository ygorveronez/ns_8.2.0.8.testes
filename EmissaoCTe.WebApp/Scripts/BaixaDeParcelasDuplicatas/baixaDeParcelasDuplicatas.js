$(document).ready(function () {
    CarregarConsultadeClientes("btnBuscarPessoa", "btnBuscarPessoa", RetornoConsultaPessoa, true, false);
    CarregarConsultaDePlanosDeContas("btnBuscarPlanoDeConta", "btnBuscarPlanoDeConta", "A", "A", RetornoConsultaPlanoDeConta, true, false);

    FormatarCampoDate("txtDataInicial");
    FormatarCampoDate("txtDataFinal");

    $("#txtPessoa").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoPessoa").val("");
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtPlanoDeConta").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddPlanoDeConta").val("");
            } else {
                e.preventDefault();
            }
        }
    });

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnCancelar").click(function () {
        LimparCampos();
    });

    $("#btnAtualizarGridParcelas").click(function () {
        AtualizarGridParcelas();
    });

    $("#btnSelecionarTodasParcelas").click(function () {
        AdicionarTodasParcelas();
    });

    LimparCampos();

    CarregarParcela();
});


function CarregarParcela() {

    var codigoParcela = GetUrlParam("x");

    if (codigoParcela != null) {
        executarRest("/BaixaDeParcelasDuplicatas/ObterInformacoesParcela?callback=?", { CodigoParcela: codigoParcela }, function (r) {
            if (r.Sucesso) {

                var parcelas = $("#hddParcelas").val() == "" ? new Array() : JSON.parse($("#hddParcelas").val());

                parcelas.push(r.Objeto);
                $("#hddParcelas").val(JSON.stringify(parcelas));

                var tag = document.createElement("li");
                tag.className = "tag-item tag-item-delete-experience";
                tag.id = "parcelaSelecionada_" + r.Objeto.Codigo;

                var container = document.createElement("span");
                container.className = "tag-container tag-container-delete-experience";

                var descricao = document.createElement("span");
                descricao.className = "tag-box tag-box-delete-experience";
                descricao.innerHTML = "<b>" + r.Objeto.Numero + "</b> | <b>" + r.Objeto.Valor + "</b> | <b>" + r.Objeto.DataVencimento + "</b>: " + r.Objeto.Cliente

                var opcaoExcluir = document.createElement("span");
                opcaoExcluir.className = "tag-delete tag-box tag-box-delete-experience";
                opcaoExcluir.innerHTML = "&nbsp;";
                opcaoExcluir.onclick = function () { RemoverDuplicataSelecionada(r.Objeto.Codigo) };

                container.appendChild(descricao);
                container.appendChild(opcaoExcluir);

                tag.appendChild(container);

                document.getElementById("containerParcelasSelecionadas").appendChild(tag);

                $("#lblSemParcelas").hide();

            } else {
                jAlert(r.Erro + "<br /><br />Não foi possível carregar parcela.", "Atenção!", function () {
                });
            }
        });
    }
}

function GetUrlParam(name) {
    var url = window.location.search.replace("?", "");
    var itens = url.split("&");
    for (n in itens) {
        if (itens[n].match(name)) {
            return itens[n].replace(name + "=", "");
        }
    }
    return null;
}

function RetornoConsultaPessoa(cliente) {
    $("#txtPessoa").val(cliente.CPFCNPJ + " - " + cliente.Nome);
    $("#hddCodigoPessoa").val(cliente.CPFCNPJ);
}

function RetornoConsultaPlanoDeConta(plano) {
    $("#txtPlanoDeConta").val(plano.Conta + " - " + plano.Descricao);
    $("#hddPlanoDeConta").val(plano.Codigo);
}

function LimparCampos() {
    $("#hddCodigoCliente").val("");
    $("#txtDocumento").val("");
    $("#txtCTe").val("");
    $("#txtDataInicial").val("");
    $("#txtDataFinal").val("");
    $("#txtObservacao").val("");
    $("#selTipo").val($("#selTipo option:first").val());
    $("#hddStatus").val($("#selTipo").val());
    $("#hddParcelas").val("");
    $("#containerParcelasSelecionadas").html("");
    $("#lblSemParcelas").show();
    $("#txtPlanoDeConta").val("");
    $("#hddPlanoDeConta").val("");
    AtualizarGridParcelas();
}
function AtualizarGridParcelas() {
    var duplicatas = $("#hddParcelas").val() == "" ? new Array() : JSON.parse($("#hddParcelas").val());
    if ($("#hddStatus").val() != $("#selTipo").val() && duplicatas.length > 0) {
        jConfirm("Há duplicatas selecionadas, deseja realmente continuar? Ao clicar em 'Sim' as duplicatas selecionadas serão removidas.", "Atenção", function (r) {
            if (r) {
                $("#hddStatus").val($("#selTipo").val());
                $("#hddParcelas").val("");
                $("#containerParcelasSelecionadas").html("");
                $("#lblSemParcelas").show();
                CarregarInformacoesParcelas();
            }
        });
    } else {
        CarregarInformacoesParcelas();
    }
}
function CarregarInformacoesParcelas() {
    var dados = ObterFiltros();

    var colunas = new Array();

    colunas[0] = { Descricao: "Selecionar", Evento: AdicionarParcela };

    CriarGridView("/BaixaDeParcelasDuplicatas/Consultar?callback=?", dados, "tbl_parcelas_table", "tbl_parcelas", "tbl_paginacao_parcelas", colunas, [0], null);
}
function ObterFiltros() {
    var filtros = {
        inicioRegistros: 0,
        Tipo: $("#selTipo").val(),
        DataInicial: $("#txtDataInicial").val(),
        DataFinal: $("#txtDataFinal").val(),
        CpfCnpjCliente: $("#hddCodigoPessoa").val(),
        NumeroDocumento: $("#txtDocumento").val(),
        NumeroCTe: $("#txtCTe").val()
    };

    return filtros;
}
function AdicionarParcela(parcela) {
    var parcelas = $("#hddParcelas").val() == "" ? new Array() : JSON.parse($("#hddParcelas").val());

    for (var i = 0; i < parcelas.length; i++)
        if (parcelas[i].Codigo == parcela.data.Codigo)
            return;

    parcelas.push(parcela.data);
    $("#hddParcelas").val(JSON.stringify(parcelas));

    var tag = document.createElement("li");
    tag.className = "tag-item tag-item-delete-experience";
    tag.id = "parcelaSelecionada_" + parcela.data.Codigo;

    var container = document.createElement("span");
    container.className = "tag-container tag-container-delete-experience";

    var descricao = document.createElement("span");
    descricao.className = "tag-box tag-box-delete-experience";
    descricao.innerHTML = "<b>" + parcela.data.Numero + "</b> | <b>" + parcela.data.Valor + "</b> | <b>" + parcela.data.DataVencimento + "</b>: " + parcela.data.Cliente

    var opcaoExcluir = document.createElement("span");
    opcaoExcluir.className = "tag-delete tag-box tag-box-delete-experience";
    opcaoExcluir.innerHTML = "&nbsp;";
    opcaoExcluir.onclick = function () { RemoverDuplicataSelecionada(parcela.data.Codigo) };

    container.appendChild(descricao);
    container.appendChild(opcaoExcluir);

    tag.appendChild(container);

    document.getElementById("containerParcelasSelecionadas").appendChild(tag);

    $("#lblSemParcelas").hide();
}
function AdicionarTodasParcelas() {
    executarRest("/BaixaDeParcelasDuplicatas/Consultar?callback=?", ObterFiltros(), function (r) {
        if (r.Sucesso) {
            for (var i = 0; i < r.Objeto.length; i++) {
                AdicionarParcela({ data: r.Objeto[i] });
            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function Salvar() {
    if (ValidarBaixa()) {
        var parcelas = $("#hddParcelas").val() == "" ? new Array() : JSON.parse($("#hddParcelas").val());
        var parcelasSelecionadas = new Array();
        for (var i = 0; i < parcelas.length; i++)
            parcelasSelecionadas.push(parcelas[i].Codigo);

        var dados = {
            Parcelas: JSON.stringify(parcelasSelecionadas),
            PlanoDeConta: $("#hddPlanoDeConta").val(),
            Observacao: $("#txtObservacao").val()
        };

        executarRest("/BaixaDeParcelasDuplicatas/BaixarParcelas?callback=?", dados, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                LimparCampos();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção");
            }
        });
    }
}
function RemoverDuplicataSelecionada(codigo) {
    var parcelas = $("#hddParcelas").val() == "" ? new Array() : JSON.parse($("#hddParcelas").val());

    for (var i = 0; i < parcelas.length; i++) {
        if (parcelas[i].Codigo == codigo) {
            parcelas.splice(i, 1);
            $("#parcelaSelecionada_" + codigo).remove();
            break;
        }
    }

    $("#hddParcelas").val(JSON.stringify(parcelas));

    if (parcelas.length <= 0)
        $("#lblSemParcelas").show();
}

function ValidarBaixa() {
    var parcelas = $("#hddParcelas").val() == "" ? new Array() : JSON.parse($("#hddParcelas").val());
    var planoDeConta = $("#txtPlanoDeConta").val();
    var valido = true;

    if (!parcelas.length > 0) {
        ExibirMensagemAlerta("Selecione ao menos uma parcela para realizar a baixa.", "Atenção");
        valido = false;
    }

    if (planoDeConta == null || planoDeConta == "") {
        CampoComErro("#txtPlanoDeConta");
        valido = false;
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!");
    } else {
        CampoSemErro("#txtPlanoDeConta");
    }

    return valido;
}