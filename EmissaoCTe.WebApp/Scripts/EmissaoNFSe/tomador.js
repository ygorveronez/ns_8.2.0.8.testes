$(document).ready(function () {
    $("#txtCEPTomador").mask("99.999-999");
    $("#txtCPFCNPJTomador").focusout(function () {
        BuscarTomador(true);
    });
    $("#selEstadoTomador").change(function () {
        BuscarLocalidades($(this).val(), "selCidadeTomador");
    });

    CarregarConsultadeClientes("btnBuscarTomador", "btnBuscarTomador", RetornoConsultaTomador, true, false);
    CarregarConsultaDeAtividades("btnBuscarAtividadeTomador", "btnBuscarAtividadeTomador", RetornoConsultaAtividadeTomador, true, false);

    $("#chkTomadorExportacao").click(function () {
        var checked = $(this)[0].checked;
        LimparCamposTomador();
        if (checked) {
            BloquearCamposTomadorExportacao();
            $("#selEstadoTomador").val("EX");
            BuscarLocalidades("EX", "selCidadeTomador");
        }
    });
});

function RetornoConsultaAtividadeTomador(atividade) {
    $("#txtAtividadeTomador").val(atividade.Codigo + " - " + atividade.Descricao);
    $("body").data("codigoAtividadeTomador", atividade.Codigo);

    if (atividade.Codigo == 7)
        $("#txtRGIETomador").val("ISENTO");
}

function RetornoConsultaTomador(cliente) {
    BuscarTomador(true, cliente.CPFCNPJ);
}

function BuscarTomador(carregarLocalidade, cpfCnpj, dados) {
    if (dados != null) {
        PreencherCamposTomador(dados);
        return;
    }

    if (cpfCnpj == null || cpfCnpj == "")
        cpfCnpj = $("#txtCPFCNPJTomador").val();

    cpfCnpj = cpfCnpj.replace(/[^0-9]/g, '');

    if (cpfCnpj != "") {
        if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        PreencherCamposTomador(r.Objeto);
                    } else {
                        LimparCamposTomador();
                        jAlert("<b>Tomador não encontrado.</b><br/><br/> Preencha corretamente os campos e o Tomador será cadastrado ao salvar/emitir a NFS-e.", "Atenção");
                    }
                } else {
                    LimparCamposTomador();
                    jAlert(r.Erro, "Erro");
                    $("#txtCPFCNPJTomador").val('');
                }
            });
        } else {
            jAlert("O CPF/CNPJ digitado é inválido.", "Atenção");
            LimparCamposTomador();
            $("#txtCPFCNPJTomador").val('');
        }
    } else {
        LimparCamposTomador();
        $("#txtCPFCNPJTomador").val('');
    }
}

function PreencherCamposTomador(cliente) {
    $("body").data("codigoAtividadeTomador", cliente.CodigoAtividade);
    $("#txtAtividadeTomador").val(cliente.CodigoAtividade + " - " + cliente.DescricaoAtividade);
    $("#txtCPFCNPJTomador").val(cliente.CPF_CNPJ);
    $("#txtRGIETomador").val(cliente.IE_RG);
    $("#txtIMTomador").val(cliente.InscricaoMunicipal);
    $("#txtRazaoSocialTomador").val(cliente.Nome);
    $("#txtNomeFantasiaTomador").val(cliente.NomeFantasiaTransportador != null && cliente.NomeFantasiaTransportador != "" ? cliente.NomeFantasiaTransportador : cliente.NomeFantasia);
    $("#txtTelefone1Tomador").val(cliente.Telefone1).change();
    $("#txtTelefone2Tomador").val(cliente.Telefone2).change();
    $("#txtEnderecoTomador").val(cliente.Endereco);
    $("#txtNumeroTomador").val(cliente.Numero);
    $("#txtBairroTomador").val(cliente.Bairro);
    $("#txtComplementoTomador").val(cliente.Complemento);
    $("#txtCEPTomador").val(cliente.CEP);
    $("#txtEmailsTomador").val(cliente.Email);
    $("#txtEmailsContatoTomador").val(cliente.EmailContato);
    $("#txtEmailsContadorTomador").val(cliente.EmailContador);
    $("#chkSalvarEnderecoTomador").prop("checked", cliente.SalvarEndereco);
    $("#selPaisTomador").val("01058");

    if (cliente.EmailStatus == "A" || cliente.EmailStatus == true) {
        $("#chkStatusEmailsTomador").prop("checked", true);
    } else {
        $("#chkStatusEmailsTomador").prop("checked", false);
    }

    if (cliente.EmailContatoStatus == "A" || cliente.EmailContatoStatus == true) {
        $("#chkStatusEmailsContatoTomador").prop("checked", true);
    } else {
        $("#chkStatusEmailsContatoTomador").prop("checked", false);
    }

    if (cliente.EmailContadorStatus == "A" || cliente.EmailContadorStatus == true) {
        $("#chkStatusEmailsContadorTomador").prop("checked", true);
    } else {
        $("#chkStatusEmailsContadorTomador").prop("checked", false);
    }

    $("#selEstadoTomador").val(cliente.UF);
    BuscarLocalidades(cliente.UF, 'selCidadeTomador', cliente.CodigoLocalidade);

    DesbloquearCamposTomadorExportacao();
}

function LimparCamposTomador() {
    $("#txtCPFCNPJTomador").val('');
    $("#txtRGIETomador").val('');
    $("#txtIMTomador").val('');
    $("#txtRazaoSocialTomador").val('');
    $("#txtNomeFantasiaTomador").val('');
    $("#txtTelefone1Tomador").val('').change();
    $("#txtTelefone2Tomador").val('').change();
    $("#txtEnderecoTomador").val('');
    $("#txtNumeroTomador").val('');
    $("#txtBairroTomador").val('');
    $("#txtComplementoTomador").val('');
    $("#txtCEPTomador").val('');
    $("#txtEmailsTomador").val('');
    $("#chkStatusEmailsTomador").prop("checked", false);
    $("#txtEmailsContatoTomador").val('');
    $("#chkStatusEmailsContatoTomador").prop("checked", false);
    $("#txtEmailsContadorTomador").val('');
    $("#chkStatusEmailsContadorTomador").prop("checked", false);
    $("#txtAtividadeTomador").val('');
    $("body").data("codigoAtividadeTomador", null);
    $("#selEstadoTomador").val("");
    $("#selCidadeTomador").html("");
    $("#selPaisTomador").val("01058");
    $("#selCidadeTomador").html("");
    $("#chkTomadorExportacao").prop("checked", false);
    $("#chkSalvarEnderecoTomador").prop("checked", true);
    $("#txtCidadeTomadorExportacao").val("");

    DesbloquearCamposTomadorExportacao();

    //var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());

    //if (configuracaoEmpresa != null && configuracaoEmpresa.CodigoAtividade > 0)
    //    RetornoConsultaAtividadeTomador({ Codigo: configuracaoEmpresa.CodigoAtividade, Descricao: configuracaoEmpresa.DescricaoAtividade });
}
function BloquearCamposTomadorExportacao() {
    $("#txtCPFCNPJTomador").val('');
    $("#txtCPFCNPJTomador").prop("disabled", true);
    $("#chkTomadorExportacao").prop("checked", true);
    $("#chkSalvarEnderecoTomador").prop("disabled", true);
    $("#btnBuscarTomador").prop("disabled", true);
    $("#txtRGIETomador").prop("disabled", true);
    $("#txtIMTomador").prop("disabled", true);
    $("#txtNomeFantasiaTomador").prop("disabled", true);
    $("#txtTelefone1Tomador").prop("disabled", true);
    $("#txtTelefone2Tomador").prop("disabled", true);
    $("#txtAtividadeTomador").prop("disabled", true);
    $("#btnBuscarAtividadeTomador").prop("disabled", true);
    $("#txtCEPTomador").prop("disabled", true);
    $("#selEstadoTomador").prop("disabled", true);
    $("#selCidadeTomador").prop("disabled", true);
    $("#chkStatusEmailsTomador").prop("disabled", true);
    $("#txtEmailsContatoTomador").prop("disabled", true);
    $("#chkStatusEmailsContatoTomador").prop("disabled", true);
    $("#txtEmailsContadorTomador").prop("disabled", true);
    $("#chkStatusEmailsContadorTomador").prop("disabled", true);
    $("#selPaisTomador").prop("disabled", false);
    $("#divFieldCidadeExportacaoTomador").show();
    $("#divFieldNumeroDocumentoTomador").show();
    $("#divFieldCPFCNPJTomador").hide();
    $("#divFieldCidadeTomador").hide();
}
function DesbloquearCamposTomadorExportacao() {
    $("#chkSalvarEnderecoTomador").prop("disabled", false);
    $("#txtCPFCNPJTomador").prop("disabled", false);
    $("#btnBuscarTomador").prop("disabled", false);
    $("#txtRGIETomador").prop("disabled", false);
    $("#txtIMTomador").prop("disabled", false);
    $("#txtNomeFantasiaTomador").prop("disabled", false);
    $("#txtTelefone1Tomador").prop("disabled", false);
    $("#txtTelefone2Tomador").prop("disabled", false);
    $("#txtAtividadeTomador").prop("disabled", false);
    $("#btnBuscarAtividadeTomador").prop("disabled", false);
    $("#txtCEPTomador").prop("disabled", false);
    $("#selEstadoTomador").prop("disabled", false);
    $("#selCidadeTomador").prop("disabled", false);
    $("#chkStatusEmailsTomador").prop("disabled", false);
    $("#txtEmailsContatoTomador").prop("disabled", false);
    $("#chkStatusEmailsContatoTomador").prop("disabled", false);
    $("#txtEmailsContadorTomador").prop("disabled", false);
    $("#chkStatusEmailsContadorTomador").prop("disabled", false);
    $("#selPaisTomador").prop("disabled", true);
    $("#divFieldCidadeExportacaoTomador").hide();
    $("#divFieldNumeroDocumentoTomador").hide();
    $("#divFieldCPFCNPJTomador").show();
    $("#divFieldCidadeTomador").show();
}

function PreencherCamposTomadorExportacao(dados) {
    BloquearCamposTomadorExportacao();
    $("#txtBairroTomador").val(dados.Bairro);
    $("#txtCidadeTomadorExportacao").val(dados.Cidade);
    $("#txtComplementoTomador").val(dados.Complemento);
    $("#txtEmailsTomador").val(dados.Email);
    $("#txtEnderecoTomador").val(dados.Endereco);
    $("#chkTomadorExportacao").prop("checked", true);
    $("#txtRazaoSocialTomador").val(dados.Nome);
    $("#txtNumeroTomador").val(dados.Numero);
    $("#selPaisTomador").val(dados.SiglaPais);
    $("#selEstadoTomador").val("EX");
}