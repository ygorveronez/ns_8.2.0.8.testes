$(document).ready(function () {
    $("#txtCEPIntermediario").mask("99.999-999");
    $("#txtCPFCNPJIntermediario").focusout(function () {
        BuscarIntermediario(true);
    });
    $("#selEstadoIntermediario").change(function () {
        BuscarLocalidades($(this).val(), "selCidadeIntermediario");
    });

    CarregarConsultadeClientes("btnBuscarIntermediario", "btnBuscarIntermediario", RetornoConsultaIntermediario, true, false);
    CarregarConsultaDeAtividades("btnBuscarAtividadeIntermediario", "btnBuscarAtividadeIntermediario", RetornoConsultaAtividadeIntermediario, true, false);

    $("#chkIntermediarioExportacao").click(function () {
        var checked = $(this)[0].checked;
        LimparCamposIntermediario();
        if (checked) {
            BloquearCamposIntermediarioExportacao();
            $("#selEstadoIntermediario").val("EX");
            BuscarLocalidades("EX", "selCidadeIntermediario");
        }
    });
});

function RetornoConsultaAtividadeIntermediario(atividade) {
    $("#txtAtividadeIntermediario").val(atividade.Codigo + " - " + atividade.Descricao);
    $("body").data("codigoAtividadeIntermediario", atividade.Codigo);

    if (atividade.Codigo == 7)
        $("#txtRGIEIntermediario").val("ISENTO");
}

function RetornoConsultaIntermediario(cliente) {
    BuscarIntermediario(true, cliente.CPFCNPJ);
}

function BuscarIntermediario(carregarLocalidade, cpfCnpj, dados) {
    if (dados != null) {
        PreencherCamposIntermediario(dados);
        return;
    }

    if (cpfCnpj == null || cpfCnpj == "")
        cpfCnpj = $("#txtCPFCNPJIntermediario").val();

    cpfCnpj = cpfCnpj.replace(/[^0-9]/g, '');

    if (cpfCnpj != "") {
        if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        PreencherCamposIntermediario(r.Objeto);
                    } else {
                        LimparCamposIntermediario();
                        jAlert("<b>Intermediario não encontrado.</b><br/><br/> Preencha corretamente os campos e o Intermediario será cadastrado ao salvar/emitir a NFS-e.", "Atenção");
                    }
                } else {
                    LimparCamposIntermediario();
                    jAlert(r.Erro, "Erro");
                    $("#txtCPFCNPJIntermediario").val('');
                }
            });
        } else {
            jAlert("O CPF/CNPJ digitado é inválido.", "Atenção");
            LimparCamposIntermediario();
            $("#txtCPFCNPJIntermediario").val('');
        }
    } else {
        LimparCamposIntermediario();
        $("#txtCPFCNPJIntermediario").val('');
    }
}

function PreencherCamposIntermediario(cliente) {
    $("body").data("codigoAtividadeIntermediario", cliente.CodigoAtividade);
    $("#txtAtividadeIntermediario").val(cliente.CodigoAtividade + " - " + cliente.DescricaoAtividade);
    $("#txtCPFCNPJIntermediario").val(cliente.CPF_CNPJ);
    $("#txtRGIEIntermediario").val(cliente.IE_RG);
    $("#txtIMIntermediario").val(cliente.InscricaoMunicipal);
    $("#txtRazaoSocialIntermediario").val(cliente.Nome);
    $("#txtNomeFantasiaIntermediario").val(cliente.NomeFantasia);
    $("#txtTelefone1Intermediario").val(cliente.Telefone1).change();
    $("#txtTelefone2Intermediario").val(cliente.Telefone2).change();
    $("#txtEnderecoIntermediario").val(cliente.Endereco);
    $("#txtNumeroIntermediario").val(cliente.Numero);
    $("#txtBairroIntermediario").val(cliente.Bairro);
    $("#txtComplementoIntermediario").val(cliente.Complemento);
    $("#txtCEPIntermediario").val(cliente.CEP);
    $("#txtEmailsIntermediario").val(cliente.Email);
    $("#txtEmailsContatoIntermediario").val(cliente.EmailContato);
    $("#txtEmailsContadorIntermediario").val(cliente.EmailContador);
    $("#chkSalvarEnderecoIntermediario").prop("checked", cliente.SalvarEndereco);
    $("#selPaisIntermediario").val("01058");

    if (cliente.EmailStatus == "A" || cliente.EmailStatus == true) {
        $("#chkStatusEmailsIntermediario").prop("checked", true);
    } else {
        $("#chkStatusEmailsIntermediario").prop("checked", false);
    }

    if (cliente.EmailContatoStatus == "A" || cliente.EmailContatoStatus == true) {
        $("#chkStatusEmailsContatoIntermediario").prop("checked", true);
    } else {
        $("#chkStatusEmailsContatoIntermediario").prop("checked", false);
    }

    if (cliente.EmailContadorStatus == "A" || cliente.EmailContadorStatus == true) {
        $("#chkStatusEmailsContadorIntermediario").prop("checked", true);
    } else {
        $("#chkStatusEmailsContadorIntermediario").prop("checked", false);
    }

    $("#selEstadoIntermediario").val(cliente.UF);
    BuscarLocalidades(cliente.UF, 'selCidadeIntermediario', cliente.CodigoLocalidade);

    DesbloquearCamposIntermediarioExportacao();
}

function LimparCamposIntermediario() {
    $("#txtCPFCNPJIntermediario").val('');
    $("#txtRGIEIntermediario").val('');
    $("#txtIMIntermediario").val('');
    $("#txtRazaoSocialIntermediario").val('');
    $("#txtNomeFantasiaIntermediario").val('');
    $("#txtTelefone1Intermediario").val('').change();
    $("#txtTelefone2Intermediario").val('').change();
    $("#txtEnderecoIntermediario").val('');
    $("#txtNumeroIntermediario").val('');
    $("#txtBairroIntermediario").val('');
    $("#txtComplementoIntermediario").val('');
    $("#txtCEPIntermediario").val('');
    $("#txtEmailsIntermediario").val('');
    $("#chkStatusEmailsIntermediario").prop("checked", false);
    $("#txtEmailsContatoIntermediario").val('');
    $("#chkStatusEmailsContatoIntermediario").prop("checked", false);
    $("#txtEmailsContadorIntermediario").val('');
    $("#chkStatusEmailsContadorIntermediario").prop("checked", false);
    $("#txtAtividadeIntermediario").val('');
    $("body").data("codigoAtividadeIntermediario", null);
    $("#selEstadoIntermediario").val("");
    $("#selCidadeIntermediario").html("");
    $("#selPaisIntermediario").val("01058");
    $("#selCidadeIntermediario").html("");
    $("#chkIntermediarioExportacao").prop("checked", false);
    $("#chkSalvarEnderecoIntermediario").prop("checked", true);
    $("#txtCidadeIntermediarioExportacao").val("");

    DesbloquearCamposIntermediarioExportacao();

    //var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());

    //if (configuracaoEmpresa != null && configuracaoEmpresa.CodigoAtividade > 0)
    //    RetornoConsultaAtividadeIntermediario({ Codigo: configuracaoEmpresa.CodigoAtividade, Descricao: configuracaoEmpresa.DescricaoAtividade });
}
function BloquearCamposIntermediarioExportacao() {
    $("#txtCPFCNPJIntermediario").val('');
    $("#txtCPFCNPJIntermediario").prop("disabled", true);
    $("#chkIntermediarioExportacao").prop("checked", true);
    $("#chkSalvarEnderecoIntermediario").prop("disabled", true);
    $("#btnBuscarIntermediario").prop("disabled", true);
    $("#txtRGIEIntermediario").prop("disabled", true);
    $("#txtIMIntermediario").prop("disabled", true);
    $("#txtNomeFantasiaIntermediario").prop("disabled", true);
    $("#txtTelefone1Intermediario").prop("disabled", true);
    $("#txtTelefone2Intermediario").prop("disabled", true);
    $("#txtAtividadeIntermediario").prop("disabled", true);
    $("#btnBuscarAtividadeIntermediario").prop("disabled", true);
    $("#txtCEPIntermediario").prop("disabled", true);
    $("#selEstadoIntermediario").prop("disabled", true);
    $("#selCidadeIntermediario").prop("disabled", true);
    $("#chkStatusEmailsIntermediario").prop("disabled", true);
    $("#txtEmailsContatoIntermediario").prop("disabled", true);
    $("#chkStatusEmailsContatoIntermediario").prop("disabled", true);
    $("#txtEmailsContadorIntermediario").prop("disabled", true);
    $("#chkStatusEmailsContadorIntermediario").prop("disabled", true);
    $("#selPaisIntermediario").prop("disabled", false);
    $("#divFieldCidadeExportacaoIntermediario").show();
    $("#divFieldNumeroDocumentoIntermediario").show();
    $("#divFieldCPFCNPJIntermediario").hide();
    $("#divFieldCidadeIntermediario").hide();
}
function DesbloquearCamposIntermediarioExportacao() {
    $("#chkSalvarEnderecoIntermediario").prop("disabled", false);
    $("#txtCPFCNPJIntermediario").prop("disabled", false);
    $("#btnBuscarIntermediario").prop("disabled", false);
    $("#txtRGIEIntermediario").prop("disabled", false);
    $("#txtIMIntermediario").prop("disabled", false);
    $("#txtNomeFantasiaIntermediario").prop("disabled", false);
    $("#txtTelefone1Intermediario").prop("disabled", false);
    $("#txtTelefone2Intermediario").prop("disabled", false);
    $("#txtAtividadeIntermediario").prop("disabled", false);
    $("#btnBuscarAtividadeIntermediario").prop("disabled", false);
    $("#txtCEPIntermediario").prop("disabled", false);
    $("#selEstadoIntermediario").prop("disabled", false);
    $("#selCidadeIntermediario").prop("disabled", false);
    $("#chkStatusEmailsIntermediario").prop("disabled", false);
    $("#txtEmailsContatoIntermediario").prop("disabled", false);
    $("#chkStatusEmailsContatoIntermediario").prop("disabled", false);
    $("#txtEmailsContadorIntermediario").prop("disabled", false);
    $("#chkStatusEmailsContadorIntermediario").prop("disabled", false);
    $("#selPaisIntermediario").prop("disabled", true);
    $("#divFieldCidadeExportacaoIntermediario").hide();
    $("#divFieldNumeroDocumentoIntermediario").hide();
    $("#divFieldCPFCNPJIntermediario").show();
    $("#divFieldCidadeIntermediario").show();
}

function PreencherCamposIntermediarioExportacao(dados) {
    BloquearCamposIntermediarioExportacao();
    $("#txtBairroIntermediario").val(dados.Bairro);
    $("#txtCidadeIntermediarioExportacao").val(dados.Cidade);
    $("#txtComplementoIntermediario").val(dados.Complemento);
    $("#txtEmailsIntermediario").val(dados.Email);
    $("#txtEnderecoIntermediario").val(dados.Endereco);
    $("#chkIntermediarioExportacao").prop("checked", true);
    $("#txtRazaoSocialIntermediario").val(dados.Nome);
    $("#txtNumeroIntermediario").val(dados.Numero);
    $("#selPaisIntermediario").val(dados.SiglaPais);
    $("#selEstadoIntermediario").val("EX");
}