var errosConsultas, codigosBarrasConsultaMercadoLivre, codigosBarrasImportadas;

$(document).ready(function () {
    $("#txtValorFreteMercadoLivre").priceFormat({ prefix: '' });
    $("#txtValorPedagioMercadoLivre").priceFormat({ prefix: '' });
    $("#txtValorOutrosMercadoLivre").priceFormat({ prefix: '' });
    $("#txtPercentualGrisMercadoLivre").priceFormat({ prefix: '' });    

    CarregarConsultadeClientes("btnBuscarTomadorMercadoLivre", "btnBuscarTomadorMercadoLivre", RetornoConsultaTomadorMercadoLivre, true, false);
    CarregarConsultadeClientes("btnBuscarTomador2MercadoLivre", "btnBuscarTomador2MercadoLivre", RetornoConsultaTomador2MercadoLivre, true, false);
    CarregarConsultadeClientes("btnBuscarExpedidorMercadoLivre", "btnBuscarExpedidorMercadoLivre", RetornoConsultaExpedidorMercadoLivre, true, false);
    CarregarConsultadeClientes("btnBuscarRecebedorMercadoLivre", "btnBuscarRecebedorMercadoLivre", RetornoConsultaRecebedorMercadoLivre, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotoristaMercadoLivre", "btnBuscarMotoristaMercadoLivre", "A", RetornoConsultaMotoristaMercadoLivre, true, false)
    CarregarConsultaDeVeiculos("btnBuscarVeiculoMercadoLivre", "btnBuscarVeiculoMercadoLivre", RetornoConsultaVeiculoMercadoLivre, true, false, 0);
    CarregarConsultaDeVeiculos("btnBuscarReboqueMercadoLivre", "btnBuscarReboqueMercadoLivre", RetornoConsultaReboqueMercadoLivre, true, false, 1);

    $("#btnConsultarMercadoLivre").click(function () {
        AbrirDivConsultaMercadoLivre();
    });

    $("#btnFecharConsultaMercadoLivre").click(function () {
        LimparConsultaMercadoLivre();
        $('#divConsultaMercadoLivre').modal("hide");
    });

    $("#btnGerarCTeMercadoLivre").click(function () {
        if (ValidaGerarCTeMecadoLivre()) {
            GerarCTeMercadoLivre();
        }
    });

    $("#btnAdicionarCodigoBarrasMercadoLivre").click(function () {
        AdicionarCodigoBarrasMercadoLivre();
    });

    LimparConsultaMercadoLivre();
});

function AbrirDivConsultaMercadoLivre() {
    $("#tituloConsultaMercadoLivre").text("Integração Mercado Livre");
    $('#divConsultaMercadoLivre').modal("show");
}


function AdicionarCodigoBarrasMercadoLivre() {
    //var stringList = [];
    //strValues = $("#txtCodigoBarrasConsultaMercadoLivre").map(function () {
    //    return this.value;
    //}).get().join(',');

    if (codigosBarrasConsultaMercadoLivre != null && $("#txtCodigoBarrasConsultaMercadoLivre").val() != "") {
        var codigoBarras = {
            CodigoBarras: $("#txtCodigoBarrasConsultaMercadoLivre").val()
        };

        var jaAdicionado = false;
        for (var i = 0; i < codigosBarrasConsultaMercadoLivre.length; i++) {
            if (codigosBarrasConsultaMercadoLivre[i].CodigoBarras == $("#txtCodigoBarrasConsultaMercadoLivre").val()) {
                jaAdicionado = true;
            }
        }

        if (!jaAdicionado) {
            codigosBarrasConsultaMercadoLivre.push(codigoBarras);
            RenderizarConsultaMercadoLivre();
        }

        $("#txtCodigoBarrasConsultaMercadoLivre").val("");
        RenderizarConsultaMercadoLivre();
    }
}

function ValidaGerarCTeMecadoLivre() {
    return true;
}

function GerarCTeMercadoLivre() {

    var dados = {
        CodigosBarras: JSON.stringify(codigosBarrasConsultaMercadoLivre),
        ValorFrete: $("#txtValorFreteMercadoLivre").val(),
        ValorPedagio: $("#txtValorPedagioMercadoLivre").val(),
        ValorOutros: $("#txtValorOutrosMercadoLivre").val(),
        PercentualGris: $("#txtPercentualGrisMercadoLivre").val(),
        Tomador: $("#hddCodigoTomadorMercadoLivre").val(),
        Tomador2: $("#hddCodigoTomador2MercadoLivre").val(),
        Expedidor: $("#hddCodigoExpedidorMercadoLivre").val(),
        Recebedor: $("#hddCodigoRecebedorMercadoLivre").val(),
        CodigoVeiculo: $("#hddCodigoVeiculoMercadoLivre").val(),
        CodigoReboque: $("#hddCodigoReboqueMercadoLivre").val(),
        CodigoMotorista: $("#hddCodigoMotoristaMercadoLivre").val(),
        ObservacaoCTe: $("#txtObservacaoMercadoLivre").val()
    };

    executarRest("/ConhecimentoDeTransporteEletronico/GerarCTeMercadoLivre?callback=?", dados, function (r) {
        if (r.Sucesso) {
            LimparConsultaMercadoLivre();
            $('#divConsultaMercadoLivre').modal("hide");
            AtualizarGridCTes();
            ExibirMensagemSucesso(r.Erro, "Sucesso!");
        } else {
            jAlert(r.Erro, "Integração Mercado Livre");
            //ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function LimparConsultaMercadoLivre() {

    errosConsultas = "";
    codigosBarrasConsultaMercadoLivre = null;
    codigosBarrasConsultaMercadoLivre = new Array();
    RenderizarConsultaMercadoLivre();

    LimparImportacaoCodigosBarras();
}

function RenderizarConsultaMercadoLivre() {
    $("#tblMercadoLivre tbody").html("");

    if (codigosBarrasConsultaMercadoLivre != null) {
        for (var i = 0; i < codigosBarrasConsultaMercadoLivre.length; i++) {
            if (!codigosBarrasConsultaMercadoLivre[i].Excluir)
                $("#tblMercadoLivre tbody").append("<tr><td>" + codigosBarrasConsultaMercadoLivre[i].CodigoBarras + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirCodigoBarrasConsultaSefaz(" + JSON.stringify(codigosBarrasConsultaMercadoLivre[i]) + ")'>Excluir</button></td></tr>");
        }
    }

    if ($("#tblMercadoLivre tbody").html() == "")
        $("#tblMercadoLivre tbody").html("<tr><td colspan='4'>Nenhum código de barras importado.</td></tr>");
}


function ExcluirCodigoBarrasConsultaSefaz(codigoBarras) {
    for (var i = 0; i < codigosBarrasConsultaMercadoLivre.length; i++) {
        if (codigosBarrasConsultaMercadoLivre[i].CodigoBarras == codigoBarras.CodigoBarras) {
            codigosBarrasConsultaMercadoLivre.splice(i, 1);
            break;
        }
    }
    RenderizarConsultaMercadoLivre();
}


function LimparImportacaoCodigosBarras() {
    codigosBarrasImportadas = null;
    codigosBarrasImportadas = new Array();
    //RenderizarRetornoImportacaoChavesNFe();
}

function RetornoConsultaTomadorMercadoLivre(cliente) {
    $("#hddCodigoTomadorMercadoLivre").val(cliente.CPFCNPJ);
    $("#txtTomadorMercadoLivre").val(cliente.CPFCNPJ + " - " + cliente.Nome);
}

function RetornoConsultaTomador2MercadoLivre(cliente) {
    $("#hddCodigoTomador2MercadoLivre").val(cliente.CPFCNPJ);
    $("#txtTomador2MercadoLivre").val(cliente.CPFCNPJ + " - " + cliente.Nome);
}


function RetornoConsultaExpedidorMercadoLivre(cliente) {
    $("#hddCodigoExpedidorMercadoLivre").val(cliente.CPFCNPJ);
    $("#txtExpedidorMercadoLivre").val(cliente.CPFCNPJ + " - " + cliente.Nome);
}

function RetornoConsultaRecebedorMercadoLivre(cliente) {
    $("#hddCodigoRecebedorMercadoLivre").val(cliente.CPFCNPJ);
    $("#txtRecebedorMercadoLivre").val(cliente.CPFCNPJ + " - " + cliente.Nome);
}

function RetornoConsultaMotoristaMercadoLivre(motorista) {
    $("#hddCodigoMotoristaMercadoLivre").val(motorista.Codigo);
    $("#txtMotoristaMercadoLivre").val(motorista.Nome + " - " + motorista.CPFCNPJ);
}

function RetornoConsultaVeiculoMercadoLivre(veiculo) {
    $("#hddCodigoVeiculoMercadoLivre").val(veiculo.Codigo);
    $("#txtVeiculoMercadoLivre").val(veiculo.Placa);
}

function RetornoConsultaReboqueMercadoLivre(veiculo) {
    $("#hddCodigoReboqueMercadoLivre").val(veiculo.Codigo);
    $("#txtReboqueMercadoLivre").val(veiculo.Placa);
}