$(document).ready(function () {
    CarregarConsultaDeTiposDeVeiculos("btnBuscarTipoVeiculo", "btnBuscarTipoVeiculo", "A", RetornoTiposDeVeiculo, true, false);
    CarregarConsultadeClientes("btnBuscarEmbarcador", "btnBuscarEmbarcador", RetornoClienteAdicionais, true, false);

    $("#txtAdicionaisPeso").priceFormat({ centsLimit: 4 });
    $("#txtAdicionaisVolumes").priceFormat({ centsSeparator: "", centsLimit: 0 });

    RemoveConsulta("#txtTipoVeiculo, #txtEmbarcador", function ($this) {
        $this.val("").data("Codigo", 0);
    });

    $("#selAdicionaisUFOrigem").change(ObterCidadeOrigem);
    $("#selAdicionaisUFDestino").change(ObterCidadeDestino);

    ObterEstados();
});

function RetornoTiposDeVeiculo(tipo) {
    $("#txtTipoVeiculo").val(tipo.Descricao).data("Codigo", tipo.Codigo);
}
function RetornoClienteAdicionais(cliente) {
    $("#txtEmbarcador").val(cliente.CPFCNPJ + " - " + cliente.Nome).data("Codigo", cliente.CPFCNPJ);
}


function RenderizarAdicionais(dados) {
    $("#txtDadosBancarios").val(dados.DadosBancarios);

    if (dados.Embarcador != null)
        $("#txtEmbarcador").val(dados.Embarcador.Descricao).data("Codigo", dados.Embarcador.Codigo);

    if (dados.TipoVeiculo != null)
        $("#txtTipoVeiculo").val(dados.TipoVeiculo.Descricao).data("Codigo", dados.TipoVeiculo.Codigo);

    $("#txtAdicionaisPeso").val(dados.AdicionaisPeso);
    $("#txtAdicionaisVolumes").val(dados.AdicionaisVolumes);

    $("#selAdicionaisUFOrigem").val(dados.AdicionaisUFOrigem);
    ObterCidade($("#selAdicionaisUFOrigem").val(), "selAdicionaisCidadeOrigem", dados.AdicionaisCidadeOrigem);
    $("#selAdicionaisUFDestino").val(dados.AdicionaisUFDestino);
    ObterCidade($("#selAdicionaisUFDestino").val(), "selAdicionaisCidadeDestino", dados.AdicionaisCidadeDestino);
}

function LimparCamposAdicionais() {
    $("#txtDadosBancarios").val("");
    $("#txtTipoVeiculo").val("").data("Codigo", 0);
    $("#txtEmbarcador").val("").data("Codigo", 0);
    $("#selAdicionaisUFOrigem").val("").trigger("change");
    $("#selAdicionaisUFDestino").val("").trigger("change");
    $("#txtAdicionaisPeso").val("0,0000");
    $("#txtAdicionaisVolumes").val("0");
}

function ObterEstados() {
    executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {

            var selUFCarregamento = $("#selAdicionaisUFOrigem");
            var selUFDescarregamento = $("#selAdicionaisUFDestino");

            var opts = ['<option value="">Selecione</option>'];
            var htmlopts = "";

            for (var i = 0; i < r.Objeto.length; i++) 
                opts.push('<option value="' + r.Objeto[i].Sigla + '">' + r.Objeto[i].Nome + '</option>');
            htmlopts = opts.join("\n");

            selUFCarregamento.html(htmlopts);
            selUFDescarregamento.html(htmlopts);

        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function ObterCidadeOrigem() {
    ObterCidade($("#selAdicionaisUFOrigem").val(), "selAdicionaisCidadeOrigem");
}
function ObterCidadeDestino() {
    ObterCidade($("#selAdicionaisUFDestino").val(), "selAdicionaisCidadeDestino");
}

function ObterCidade(estado, select, codigo) {
    var selUF = $("#" + select);

    if (estado == "")
        return selUF.html("");
    
    executarRest("/Localidade/BuscarPorUF?callback=?", { UF: estado }, function (r) {
        if (r.Sucesso) {

            var opts = [];
            var htmlopts = "";

            for (var i = 0; i < r.Objeto.length; i++) {
                var selected = "";
                if (codigo != null && codigo == r.Objeto[i].Codigo)
                    selected = 'selected="selected"'
                opts.push('<option ' + selected + ' value="' + r.Objeto[i].Codigo + '">' + r.Objeto[i].Descricao + '</option>');
            }
            htmlopts = opts.join("\n");

            selUF.html(htmlopts);
        }
    });
}