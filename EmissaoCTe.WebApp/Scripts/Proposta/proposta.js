$(document).ready(function () {
    $("#txtData").mask("99/99/9999");
    FormatarCampoDate("txtData");

    $("#txtCustosAdicionais, #txtFormaCobranca, #txtCTRN").ckeditor();

    $("#txtPeso").priceFormat({ prefix: '' });
    $("#txtValorMercadoria").priceFormat({ prefix: '' });

    // Remove o X
    $("#txtVolumes, #txtDimensoesA, #txtDimensoesL, #txtDimensoesP").change(function () {
        var $this = $(this);
        var valor = $this.val();

        $this.val(valor.replace(/x/g, ''));
    });

    $("#btnSalvar").on("click", function () {
        Salvar();
    });

    $("#btnCancelar").on("click", function () {
        LimparCampos();
    });

    $("#btnExcluir").on("click", function () {
        Excluir();
    });

    $("#btnDownloadProposta").on("click", function () {
        DownloadProposta();
    });

    LimparCampos();
});

function LimparCampos() {
    $("body").data('codigo', 0);

    // Datas
    $("#txtDataLancamento").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#txtData").val(Globalize.format((new Date()), "dd/MM/yyyy"));

    // Campos
    $("#txtNumero").val(0);
    $("#txtTelefone").val("");
    $("#txtEmail").val("");
    $("#txtPeso").val("");
    $("#txtVolumes").val("");
    $("#txtDimensoesA").val("");
    $("#txtDimensoesL").val("");
    $("#txtDimensoesP").val("");
    $("#txtDiasValidade").val("");
    $("#txtObservacoes").val("");
    $("#txtValorMercadoria").val("0,00");
    $("#txtUnidadeMonetaria").val("");

    // Editor
    $("#txtCustosAdicionais").val("");
    $("#txtFormaCobranca").val("");
    $("#txtCTRN").val("");

    // Consultas
    $("#txtNome").val("").data("codigo", 0);
    $("#txtTipoColeta").val("").data("codigo", 0);
    $("#txtTipoCarga").val("").data("codigo", 0);
    $("#txtOrigem").val("").data("codigo", 0);
    $("#txtDestino").val("").data("codigo", 0);
    $("#txtClienteOrigem").val("").data("codigo", 0);
    $("#txtClienteDestino").val("").data("codigo", 0);

    // Selects
    $("#selModalProposta").val($("#selModalProposta option:first").val());
    $("#selTipoVeiculo").val($("#selTipoVeiculo option:first").val());
    $("#selTipoCarroceria").val($("#selTipoCarroceria option:first").val());
    $("#selRastreador").val($("#selRastreador option:first").val());

    // Itens
    LimparCamposItem();
    LimparTodosItens();

    // Botoes
    $("#btnDownloadProposta").hide();
    $("#btnExcluir").hide();

    // Busca config
    BuscaConfiguracoes();
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

    executarRest("/Proposta/ObterProximoNumero?callback=?", {}, function (r) {
        if (r.Sucesso) {
            $("#txtNumero").val(r.Objeto.Numero);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function Salvar() {
    var proposta = {
        Codigo: $("body").data('codigo'),

        // Datas
        Data: $("#txtData").val(),

        // Campos
        Peso: $("#txtPeso").val(),
        Volumes: $("#txtVolumes").val(),
        Nome: $("#txtNome").val(),
        Email: $("#txtEmail").val(),
        Telefone: $("#txtTelefone").val(),
        Dimensoes: $("#txtDimensoesA").val() + ' x ' + $("#txtDimensoesL").val() + ' x ' + $("#txtDimensoesP").val(),
        DiasValidade: $("#txtDiasValidade").val(),
        Observacoes: $("#txtObservacoes").val(),
        ValorMercadoria: $("#txtValorMercadoria").val(),
        UnidadeMonetaria: $("#txtUnidadeMonetaria").val(),

        // Editor
        TextoCustosAdicionais: $("#txtCustosAdicionais").val(),
        TextoFormaCobranca: $("#txtFormaCobranca").val(),
        TextoCTRN: $("#txtCTRN").val(),

        // Consultas
        Cliente: $("#txtCliente").data("codigo"),
        TipoColeta: $("#txtTipoColeta").data("codigo"),
        TipoCarga: $("#txtTipoCarga").data("codigo"),
        Origem: $("#txtOrigem").data("codigo"),
        Destino: $("#txtDestino").data("codigo"),
        ClienteOrigem: $("#txtClienteOrigem").data("codigo"),
        ClienteDestino: $("#txtClienteDestino").data("codigo"),

        // Selects
        ModalProposta: $("#selModalProposta").val(),
        TipoVeiculo: $("#selTipoVeiculo").val(),
        TipoCarroceria: $("#selTipoCarroceria").val(),
        Rastreador: $("#selRastreador").val(),

        // Itens
        Itens: ItensJson()
    };

    executarRest("/Proposta/Salvar?callback=?", proposta, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Dados salvor com sucesso", "Atenção!");
            BuscarDadosProposta(r.Objeto.Codigo);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function Excluir() {
    var proposta = {
        Codigo: $("body").data('codigo')
    };

    executarRest("/Proposta/Excluir?callback=?", proposta, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Exluído com sucesso", "Atenção!");
            BuscarDadosProposta(r.Objeto.Codigo);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function BuscarDadosProposta(codigo) {
    executarRest("/Proposta/ObterDetalhes?callback=?", { Codigo: codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data('codigo', r.Objeto.Codigo);

            // Datas
            $("#txtData").val(r.Objeto.Data);
            $("#txtDataLancamento").val(r.Objeto.DataLancamento);

            // Campos
            $("#txtNumero").val(r.Objeto.Numero);
            $("#txtPeso").val(r.Objeto.Peso);
            $("#txtVolumes").val(r.Objeto.Volumes);
            $("#txtDiasValidade").val(r.Objeto.DiasValidade);
            $("#txtEmail").val(r.Objeto.Email);
            $("#txtTelefone").val(r.Objeto.Telefone);
            $("#txtNome").val(r.Objeto.Nome);
            $("#txtObservacoes").val(r.Objeto.Observacoes);
            $("#txtValorMercadoria").val(r.Objeto.ValorMercadoria);
            $("#txtUnidadeMonetaria").val(r.Objeto.UnidadeMonetaria);

            var dimensoes = r.Objeto.Dimensoes.split(' x ');
            $("#txtDimensoesA").val(dimensoes[0]);
            $("#txtDimensoesL").val(dimensoes[1]);
            $("#txtDimensoesP").val(dimensoes[2]);

            // Editor
            $("#txtCustosAdicionais").val(r.Objeto.TextoCustosAdicionais);
            $("#txtFormaCobranca").val(r.Objeto.TextoFormaCobranca);
            $("#txtCTRN").val(r.Objeto.TextoCTRN);

            // Consultas
            PreencheConsulta($("#txtCliente"), r.Objeto.Cliente);
            PreencheConsulta($("#txtTipoColeta"), r.Objeto.TipoColeta);
            PreencheConsulta($("#txtTipoCarga"), r.Objeto.TipoCarga);
            PreencheConsulta($("#txtOrigem"), r.Objeto.Origem);
            PreencheConsulta($("#txtDestino"), r.Objeto.Destino);
            PreencheConsulta($("#txtClienteOrigem"), r.Objeto.ClienteOrigem);
            PreencheConsulta($("#txtClienteDestino"), r.Objeto.ClienteDestino);

            // Selects
            $("#selModalProposta").val(r.Objeto.ModalProposta);
            $("#selTipoVeiculo").val(r.Objeto.TipoVeiculo);
            $("#selTipoCarroceria").val(r.Objeto.TipoCarroceria);
            $("#selRastreador").val(r.Objeto.Rastreador);

            // Itens
            CarregaItens(r.Objeto.Itens);

            // Botoes
            $("#btnDownloadProposta").show();
            $("#btnExcluir").show();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function PreencheConsulta($input, data) {
    if (data == null) data = { Codigo: 0, Descricao: "" };

    $input.val(data.Descricao).data('codigo', data.Codigo);
}

function DownloadProposta() {
    var proposta = {
        Codigo: $("body").data('codigo')
    };

    executarDownload("/Proposta/Visualizar?callback=?", proposta);
}