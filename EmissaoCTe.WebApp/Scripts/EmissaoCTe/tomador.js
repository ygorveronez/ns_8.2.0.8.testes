$(document).ready(function () {
    $("#txtCEPTomador").mask("99.999-999");
    $("#txtCPFCNPJTomador").focusout(function () {
        BuscarTomador(true);
    });
    $("#selTomadorServico").change(function () {
        BloquearAbaTomador();
    });
    $("#ddlEstadoTomador").change(function () {
        BuscarLocalidades($("#ddlEstadoTomador").val(), "selCidadeTomador");
    });
    $("#chkTomadorExportacao").click(function () {
        var checked = $(this)[0].checked;
        LimparCamposTomador();
        if (checked) {
            BloquearCamposTomadorExportacao();
            $("#ddlEstadoTomador").val("EX");
            BuscarLocalidades("EX", "selCidadeTomador");
        }
    });
    $("#txtRazaoSocialTomador").focusout(function () {
        BuscarTomadorExportacaoPorNome();
    });
    CarregarConsultadeClientes("btnBuscarTomador", "btnBuscarTomador", RetornoConsultaTomador, true, false);
    CarregarConsultaDeAtividades("btnBuscarAtividadeTomador", "btnBuscarAtividadeTomador", RetornoConsultaAtividadeTomador, true, false);

    $("#txtRGIETomador").on('change', function () {
        SetarIndicadorTomador();
    });
});
function BloquearCamposTomadorExportacao() {
    $("#hddTomador").val('');
    $("#txtCPFCNPJTomador").val('');
    $("#chkTomadorExportacao").prop("checked", true);
    $("#chkSalvarEnderecoTomador").prop("disabled", true);
    $("#txtCPFCNPJTomador").prop("disabled", true);
    $("#btnBuscarTomador").prop("disabled", true);
    $("#txtRGIETomador").prop("disabled", true);
    $("#txtNomeFantasiaTomador").prop("disabled", true);
    $("#txtTelefone1Tomador").prop("disabled", true);
    $("#txtTelefone2Tomador").prop("disabled", true);
    $("#txtAtividadeTomador").prop("disabled", true);
    $("#btnBuscarAtividadeTomador").prop("disabled", true);
    $("#txtCEPTomador").prop("disabled", true);
    $("#ddlEstadoTomador").prop("disabled", true);
    $("#selCidadeTomador").prop("disabled", true);
    $("#chkStatusEmailsTomador").prop("disabled", true);
    $("#txtEmailsContatoTomador").prop("disabled", true);
    $("#chkStatusEmailsContatoTomador").prop("disabled", true);
    $("#txtEmailsContadorTomador").prop("disabled", true);
    $("#chkStatusEmailsContadorTomador").prop("disabled", true);
    $("#ddlPaisTomador").prop("disabled", false);
    $("#divFieldCidadeExportacaoTomador").show();
    $("#txtEmailsTransportadorTomador").prop("disabled", true);
    $("#chkStatusEmailsTransportadorTomador").prop("disabled", true);
    $("#divFieldCidadeTomador").hide();
}
function DesbloquearCamposTomadorExportacao() {
    $("#chkSalvarEnderecoTomador").prop("disabled", false);
    $("#txtCPFCNPJTomador").prop("disabled", false);
    $("#btnBuscarTomador").prop("disabled", false);
    $("#txtRGIETomador").prop("disabled", false);
    $("#txtNomeFantasiaTomador").prop("disabled", false);
    $("#txtTelefone1Tomador").prop("disabled", false);
    $("#txtTelefone2Tomador").prop("disabled", false);
    $("#txtAtividadeTomador").prop("disabled", false);
    $("#btnBuscarAtividadeTomador").prop("disabled", false);
    $("#txtCEPTomador").prop("disabled", false);
    $("#ddlEstadoTomador").prop("disabled", false);
    $("#selCidadeTomador").prop("disabled", false);
    $("#chkStatusEmailsTomador").prop("disabled", false);
    $("#txtEmailsContatoTomador").prop("disabled", false);
    $("#chkStatusEmailsContatoTomador").prop("disabled", false);
    $("#txtEmailsContadorTomador").prop("disabled", false);
    $("#chkStatusEmailsContadorTomador").prop("disabled", false);
    $("#ddlPaisTomador").prop("disabled", true);
    $("#txtEmailsTransportadorTomador").prop("disabled", false);
    $("#chkStatusEmailsTransportadorTomador").prop("disabled", false);
    $("#divFieldCidadeExportacaoTomador").hide();
    $("#divFieldCidadeTomador").show();
}
function RetornoConsultaAtividadeTomador(atividade, naoAtualizarIndicadorTomador) {
    $("#hddAtividadeTomador").val(atividade.Codigo);
    $("#txtAtividadeTomador").val(atividade.Codigo + " - " + atividade.Descricao);

    if (atividade.Codigo == 7)
        $("#txtRGIETomador").val("ISENTO");

    if (!(naoAtualizarIndicadorTomador === false))
        SetarIndicadorTomador();
}
function RetornoConsultaTomador(cliente) {
    BuscarTomador(true, cliente.CPFCNPJ, null, true);
}
function BuscarTomador(carregarLocalidade, cpfCnpj, dados, atualizarIndicadorTomador) {
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

                        if (atualizarIndicadorTomador)
                            SetarIndicadorTomador();
                    } else {
                        LimparCamposTomador();
                        $("#hddTomador").val($("#txtCPFCNPJTomador").val());
                        jAlert("<b>Tomador não encontrado.</b><br/><br/> Preencha corretamente os campos e o tomador será cadastrado ao salvar/emitir o CT-e.", "Atenção");
                    }
                } else {
                    jAlert(r.Erro, "Erro");
                    LimparCamposTomador();
                    $("#hddTomador").val('');
                    $("#txtCPFCNPJTomador").val('');
                }
                if (carregarLocalidade)
                    BuscarApoliceDeSeguroDoTomador();
            });
        } else {
            jAlert("O CPF/CNPJ digitado é inválido!", "Atenção");
            LimparCamposTomador();
            $("#hddTomador").val('');
            $("#txtCPFCNPJTomador").val('');
        }
    } else {
        LimparCamposTomador();
        $("#hddTomador").val('');
        $("#txtCPFCNPJTomador").val('');
    }

    if (carregarLocalidade)
        BuscarApoliceDeSeguroDoTomador();
}

function PreencherCamposTomador(cliente) {
    $("#hddTomador").val(cliente.CPF_CNPJ);
    $("#txtAtividadeTomador").val(cliente.CodigoAtividade + " - " + cliente.DescricaoAtividade);
    $("#hddAtividadeTomador").val(cliente.CodigoAtividade);
    $("#txtCPFCNPJTomador").val(cliente.CPF_CNPJ);
    $("#txtRGIETomador").val(cliente.IE_RG);
    $("#txtRazaoSocialTomador").val(cliente.Nome);
    $("#txtNomeFantasiaTomador").val(cliente.NomeFantasia);
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
    $("#txtEmailsTransportadorTomador").val(cliente.EmailTransportador);
    $("#chkSalvarEnderecoTomador").prop("checked", cliente.SalvarEndereco);

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

    if (cliente.EmailTransportadorStatus == "A" || cliente.EmailTransportadorStatus == true) {
        $("#chkStatusEmailsTransportadorTomador").prop("checked", true);
    } else {
        $("#chkStatusEmailsTransportadorTomador").prop("checked", false);
    }

    $("#ddlEstadoTomador").val(cliente.UF);
    BuscarLocalidades(cliente.UF, 'selCidadeTomador', cliente.CodigoLocalidade);
    SetarIndicadorTomador();
}

function BuscarTomadorExportacaoPorNome() {
    if ($("#chkTomadorExportacao")[0].checked && $("#txtRazaoSocialTomador").val() != "") {
        executarRest("/ClienteExportacaoCTe/BuscarPorNome?callback=?", { Nome: $("#txtRazaoSocialTomador").val() }, function (r) {
            if (r.Sucesso) {
                PreencherCamposTomadorExportacao(r.Objeto);
            }
        });
    }
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
    $("#ddlPaisTomador").val(dados.SiglaPais);
    $("#ddlEstadoTomador").val("EX");
    $("#txtRGIETomador").val(dados.IE_RG);
}
function LimparCamposTomador() {
    $("#txtAtividadeTomador").val('');
    $("#hddAtividadeTomador").val('0');
    $("#txtRGIETomador").val('');
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
    $("#txtEmailsTransportadorTomador").val('');
    $("#chkStatusEmailsTransportadorTomador").prop("checked", false);
    $("#ddlEstadoTomador").val($("#ddlEstadoTomador option:first").val());
    $("#ddlPaisTomador").val("01058");
    $("#selCidadeTomador").html('');
    $("#chkTomadorExportacao").prop("checked", false);
    $("#chkSalvarEnderecoTomador").prop("checked", true);
    $("#txtCidadeTomadorExportacao").val("");
    DesbloquearCamposTomadorExportacao();
    BloquearAbaTomador();
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    if (configuracaoEmpresa != null && configuracaoEmpresa.CodigoAtividade > 0)
        RetornoConsultaAtividadeTomador({ Codigo: configuracaoEmpresa.CodigoAtividade, Descricao: configuracaoEmpresa.DescricaoAtividade }, false);
}
function BloquearAbaTomador() {
    if ($("#selTomadorServico").val() == "4") {
        $('a[href="#tabTomador"]').show();
    } else {
        $('a[href="#tabTomador"]').hide();
    }
}