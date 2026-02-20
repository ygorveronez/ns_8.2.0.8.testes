$(document).ready(function () {
    CarregarConsultaProposta("default-search", "default-search", RetornoProposta, true, false);

    CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoClientePrincipal, true, false);
    CarregarConsultaDeTiposDeColetas("btnBuscarTipoColeta", "btnBuscarTipoColeta", "A", RetornoTipoColeta, true, false)
    CarregarConsultaDeTiposDeCargas("btnBuscarTipoCarga", "btnBuscarTipoCarga", "A", RetornoTipoCarga, true, false);
    CarregarConsultaDeLocalidades("btnBuscarOrigem", "btnBuscarOrigem", RetornoLocalidade, true, false);
    CarregarConsultaDeLocalidades("btnBuscarDestino", "btnBuscarDestino", RetornoLocalidade, true, false);
    CarregarConsultadeClientes("btnBuscarClienteOrigem", "btnBuscarClienteOrigem", RetornoCliente, true, false);
    CarregarConsultadeClientes("btnBuscarClienteDestino", "btnBuscarClienteDestino", RetornoCliente, true, false);


    RemoveConsulta("#txtOrigem, #txtDestino, #txtClienteOrigem, #txtClienteDestino, #txtTipoCarga, #txtTipoColeta", function ($this) {
        $this.val("");
        $this.data('codigo', 0);
    });

    RemoveConsulta("#txtCliente", LimparCamposUsuario);
});

function RetornoProposta(proposta) {
    BuscarDadosProposta(proposta.Codigo);
}

function LimparCamposUsuario() {
    $("#txtCliente").val("").data('codigo', 0);

    //$("#txtTelefone").val("");
    //$("#txtEmail").val("");
} 

function RetornoClientePrincipal(cliente) {
    ObterDadosCliente(cliente.CPFCNPJ.replace(/[^0-9]/g, ''));
}

function RetornoCliente(cliente, e) {
    var consulta = e.target.dataset.consulta;
    var $input = $("#txt" + consulta);

    $input.val(cliente.Nome);
    $input.data('codigo', cliente.CPFCNPJ.replace(/[^0-9]/g, ''));
}

function RetornoLocalidade(localidade, e) {
    var consulta = e.target.dataset.consulta;
    var $input = $("#txt" + consulta);

    $input.val(localidade.Descricao);
    $input.data('codigo', localidade.Codigo);
}

function RetornoTipoCarga(tipoCarga) {
    var $input = $("#txtTipoCarga");
    
    $input.val(tipoCarga.Descricao);
    $input.data('codigo', tipoCarga.Codigo);
}

function RetornoTipoColeta(tipoColeta) {
    var $input = $("#txtTipoColeta");
    
    $input.val(tipoColeta.Descricao);
    $input.data('codigo', tipoColeta.Codigo);
}

function ObterDadosCliente(codigo) {
    executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: codigo }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto != null) {
                $("#txtCliente").val(r.Objeto.Nome).data('codigo', codigo);
                $("#txtTelefone").val(r.Objeto.Telefone1);
                $("#txtEmail").val(r.Objeto.Email);
            } else {
                LimparCamposUsuario();
                ExibirMensagemAlerta("Usuário não encontrado.", "Atenção!");
            }
        } else {
            LimparCamposUsuario();
            ExibirMensagemErro(r.Erro, "Erro!");
        }
    });
}