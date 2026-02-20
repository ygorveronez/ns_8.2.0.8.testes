$(document).ready(function () {
    CarregarConsultaDeProdutoFornecedor("default-search", "default-search", RetornoConsultaProdutoFornecedor, true, false);
    CarregarConsultaDeProdutos("btnBuscarProduto", "btnBuscarProduto", "", RetornoConsultaProduto, true, false);
    CarregarConsultadeClientes("btnBuscarFornecedor", "btnBuscarFornecedor", RetornoConsultaFornecedor, true, false);
    
    RemoveConsulta($("#txtProduto"), function ($this) {
        $this.val("");
        CodigoProduto = 0;
    });

    RemoveConsulta($("#txtFornecedor"), function ($this) {
        $this.val("");
        CodigoFornecedor = 0;
    });

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnCancelar").click(function () {
        LimparCampos();
    });

    $("#btnExcluir").click(function () {
        Excluir();
    });
});

var CodigoEmEdicao = 0;
var CodigoFornecedor = 0;
var CodigoProduto = 0;

function RetornoConsultaProdutoFornecedor(produtoFornecedor) {
    CodigoEmEdicao = produtoFornecedor.Codigo;
    $("#txtNumeroFornecedor").val(produtoFornecedor.NumeroFornecedor);
    $("#txtProduto").val(produtoFornecedor.ProdutoDescricao);
    CodigoProduto = produtoFornecedor.ProdutoCodigo;
    $("#txtFornecedor").val(produtoFornecedor.FornecedorDescricao);
    CodigoFornecedor = produtoFornecedor.FornecedorCodigo;

    $("#btnExcluir").show();
}

function RetornoConsultaProduto(produto) {
    $("#txtProduto").val(produto.Descricao);
    CodigoProduto = produto.Codigo;
}

function RetornoConsultaFornecedor(cliente) {
    $("#txtFornecedor").val(cliente.CPFCNPJ + ' - ' + cliente.Nome);
    CodigoFornecedor = cliente.CPFCNPJ.replace(/[^0-9]/g, '');
}

function LimparCampos() {
    CodigoEmEdicao = 0;
    $("#txtNumeroFornecedor").val("");
    $("#txtProduto").val("");
    $("#txtFornecedor").val("");
    CodigoProduto = 0;
    CodigoFornecedor = 0;

    CampoSemErro($("#txtNumeroFornecedor"));
    CampoSemErro($("#txtProduto"));
    CampoSemErro($("#txtFornecedor"));

    $("#btnExcluir").hide();
}

function ValidarCampos() {
    var valido = [];

    if ($("#txtNumeroFornecedor").val() == "") {
        valido.push("Número do fornecedor é obrigatório.");
        CampoComErro($("#txtNumeroFornecedor"));
    } else {
        CampoSemErro($("#txtNumeroFornecedor"));
    }

    if (CodigoProduto == 0) {
        valido.push("Nenhum produto selecionado.");
        CampoComErro($("#txtProduto"));
    } else {
        CampoSemErro($("#txtProduto"));
    }

    if (CodigoFornecedor == 0) {
        valido.push("Nenhum fornecedor selecionado.");
        CampoComErro($("#txtFornecedor"));
    } else {
        CampoSemErro($("#txtFornecedor"));
    }

    return valido;
}

function Salvar() {
    var erros = ValidarCampos();

    if (erros.length == 0) {
        dados = {
            Codigo: CodigoEmEdicao,
            NumeroFornecedor: $("#txtNumeroFornecedor").val(),
            Fornecedor: CodigoFornecedor,
            Produto: CodigoProduto
        };

        executarRest("/ProdutoFornecedor/Salvar?callback=?", dados, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
                LimparCampos();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção!");
            }
        });
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>";

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:");
    }
}

function Excluir() {
    jConfirm("Tem certeza que deseja excluir os dados?", "Confirmar exclusão", function (r) {
        if (r) {
            executarRest("/ProdutoFornecedor/Excluir?callback=?", { Codigo: CodigoEmEdicao }, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Dados excluídos com sucesso!", "Sucesso!");
                    LimparCampos();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    });
}