$(document).ready(function () {
    CarregarConsultaOrdemDeCompra("default-search", "default-search", RetornoConsultaOrdemDeCompra, true, false);

    CarregarConsultaUsuario("btnBuscarSolicitate", "btnBuscarSolicitate", "A", "U", RetornoConsultaSolicitante, true, false);
    CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, true, "0");
    CarregarConsultaDeModelosDeVeiculos("btnBuscarModelo", "btnBuscarModelo", status, RetornoConsultaModelo, true, true);
    CarregarConsultadeClientes("btnBuscarFornecedor", "btnBuscarFornecedor", RetornoConsultaFornecedor, true, false);

    RemoveConsulta($("#txtVeiculo, #txtModelo"), LimpaCampoConsulta);
    RemoveConsulta($("#txtFornecedor, #txtFornecedor"), LimpaCampoConsulta);
    RemoveConsultaSolicitante();
});

var SolicitantePesquisado = false;

function LimpaCampoConsulta($this) {
    $this.val("");
    $this.data("codigo", 0);
}

function RemoveConsultaSolicitante() {
    $("#txtNomeSolicitante").keydown(function (e) {
        var stop = function (evt) {
            if (evt && evt.preventDefault) evt.preventDefault();
        }

        // 8 => Backspace 46 => Del 86 => v 88 => x
        if ((e.keyCode == 8 || e.keyCode == 46 || e.which == 8 || e.which == 46) && SolicitantePesquisado) {
            LimpaCampoConsulta($(this));
            SolicitantePesquisado = false;
            $("#btnBuscarSolicitate").prop('disabled', false);
            return;
        } else if (!SolicitantePesquisado) {
            $(this).data("codigo", 0);
            if ($(this).val() != "")
                $("#btnBuscarSolicitate").prop('disabled', true);
        }

        // Cancela ação quando o CTRL esta clicado (exceto CTRL + C)
        if (((e.keyCode != 67 && e.which != 67) && e.ctrlKey == true) && SolicitantePesquisado) stop(e);
    });

    $("#txtNomeSolicitante").keypress(function (evt) {
        if ((evt && evt.preventDefault) && SolicitantePesquisado) evt.preventDefault();
    });

    $("#txtNomeSolicitante").keyup(function (e) {
        // Permite pesquisar 
        if ($(this).val() == "")
            $("#btnBuscarSolicitate").prop('disabled', false);
    });
}
function RetornoConsultaOrdemDeCompra(ordemDeCompra) {
    var dados = {
        Codigo: ordemDeCompra.Codigo
    };

    ObterDetalhesOrdem(dados);
}

function RetornoConsultaSolicitante(solicitante) {
    SolicitantePesquisado = true;
    $("#txtNomeSolicitante").data('codigo', solicitante.Codigo);
    $("#txtNomeSolicitante").val(solicitante.Nome);
}

function RetornoConsultaVeiculo(veiculo) {
    $("#txtVeiculo").data('codigo', veiculo.Codigo);
    $("#txtVeiculo").val(veiculo.Placa);
}

function RetornoConsultaModelo(modelo) {
    $("#txtModelo").data('codigo', modelo.Codigo);
    $("#txtModelo").val(modelo.Descricao);
}

function RetornoConsultaFornecedor(cliente) {
    $("#txtFornecedor").data("codigo", cliente.CPFCNPJ);
    $("#txtFornecedor").val(cliente.CPFCNPJ + " - " + cliente.Nome);
}