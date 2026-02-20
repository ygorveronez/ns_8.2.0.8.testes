$(document).ready(function () {
    $("#txtCEPRecebedor").mask("99.999-999");
    $("#txtCPFCNPJRecebedor").focusout(function () {
        BuscarRecebedor(true);
    });
    $("#ddlEstadoRecebedor").change(function () {
        BuscarLocalidades($(this).val(), "selCidadeRecebedor");
    });
    CarregarConsultadeClientes("btnBuscarRecebedor", "btnBuscarRecebedor", RetornoConsultaRecebedor, true, false);
    CarregarConsultaDeAtividades("btnBuscarAtividadeRecebedor", "btnBuscarAtividadeRecebedor", RetornoConsultaAtividadeRecebedor, true, false);
    $("#chkRecebedorExportacao").click(function () {
        var checked = $(this)[0].checked;
        LimparCamposRecebedor();
        if (checked) {
            BloquearCamposRecebedorExportacao();
            $("#ddlEstadoRecebedor").val("EX");
            BuscarLocalidades("EX", "selCidadeRecebedor");
        }
    });
    $("#txtRazaoSocialRecebedor").focusout(function () {
        BuscarRecebedorExportacaoPorNome();
    });

    $("#txtRGIERecebedor").on('change', function () {
        SetarIndicadorTomador();
    });

    $("#selCidadeRecebedor").change(function () {
        SetarTerminoPrestacao();
    });
});
function RetornoConsultaAtividadeRecebedor(atividade, naoAtualizarIndicadorTomador) {
    $("#hddAtividadeRecebedor").val(atividade.Codigo);
    $("#txtAtividadeRecebedor").val(atividade.Codigo + " - " + atividade.Descricao);

    if (atividade.Codigo == 7)
        $("#txtRGIERecebedor").val("ISENTO");

    if (!(naoAtualizarIndicadorTomador === false))
        SetarIndicadorTomador();
}
function RetornoConsultaRecebedor(cliente) {
    BuscarRecebedor(true, cliente.CPFCNPJ);
}
function BuscarRecebedor(carregarLocalidade, cpfCnpj, dados, atualizarIndicadorTomador) {
    if (dados != null) {
        PreencherCamposRecebedor(dados);
        return;
    }

    if (cpfCnpj == null || cpfCnpj == "")
        cpfCnpj = $("#txtCPFCNPJRecebedor").val();

    cpfCnpj = cpfCnpj.replace(/[^0-9]/g, '');

    if (cpfCnpj != "") {
        if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        PreencherCamposRecebedor(r.Objeto);

                        $.ajaxSetup({ async: false });
                        ObterDadosSemelhantesPorEstado();
                        $.ajaxSetup({ async: true });

                        if (atualizarIndicadorTomador)
                            SetarIndicadorTomador();
                    } else {
                        LimparCamposRecebedor();
                        jAlert("<b>Recebedor não encontrado.</b><br/><br/> Preencha corretamente os campos e o recebedor será cadastrado ao salvar/emitir o CT-e.", "Atenção");
                        $("#hddRecebedor").val($("#txtCPFCNPJRecebedor").val());
                    }
                } else {
                    LimparCamposRecebedor();
                    jAlert(r.Erro, "Erro");
                    $("#hddRecebedor").val('');
                    $("#txtCPFCNPJRecebedor").val('');
                }
                if (carregarLocalidade)
                    BuscarApoliceDeSeguroDoTomador();
            });
        } else {
            jAlert("O CPF/CNPJ digitado é inválido.", "Atenção");
            LimparCamposRecebedor();
            $("#hddRecebedor").val('');
            $("#txtCPFCNPJRecebedor").val('');
        }
    } else {
        LimparCamposRecebedor();
        $("#hddRecebedor").val('');
        $("#txtCPFCNPJRecebedor").val('');
    }

    if (carregarLocalidade)
        BuscarApoliceDeSeguroDoTomador();
}

function PreencherCamposRecebedor(cliente) {
    $("#hddRecebedor").val(cliente.CPF_CNPJ);
    $("#hddAtividadeRecebedor").val(cliente.CodigoAtividade);
    $("#txtAtividadeRecebedor").val(cliente.CodigoAtividade + " - " + cliente.DescricaoAtividade);
    $("#txtCPFCNPJRecebedor").val(cliente.CPF_CNPJ);
    $("#txtRGIERecebedor").val(cliente.IE_RG);
    $("#txtRazaoSocialRecebedor").val(cliente.Nome);
    $("#txtNomeFantasiaRecebedor").val(cliente.NomeFantasiaTransportador != null && cliente.NomeFantasiaTransportador != "" ? cliente.NomeFantasiaTransportador : cliente.NomeFantasia);
    $("#txtTelefone1Recebedor").val(cliente.Telefone1).change();
    $("#txtTelefone2Recebedor").val(cliente.Telefone2).change();
    $("#txtEnderecoRecebedor").val(cliente.Endereco);
    $("#txtNumeroRecebedor").val(cliente.Numero);
    $("#txtBairroRecebedor").val(cliente.Bairro);
    $("#txtComplementoRecebedor").val(cliente.Complemento);
    $("#txtCEPRecebedor").val(cliente.CEP);
    $("#txtEmailsRecebedor").val(cliente.Email);
    $("#txtEmailsContatoRecebedor").val(cliente.EmailContato);
    $("#txtEmailsContadorRecebedor").val(cliente.EmailContador);
    $("#txtEmailsTransportadorRecebedor").val(cliente.EmailTransportador);
    $("#chkSalvarEnderecoRecebedor").prop("checked", cliente.SalvarEndereco);

    if (cliente.EmailStatus == "A" || cliente.EmailStatus == true) {
        $("#chkStatusEmailsRecebedor").prop("checked", true);
    } else {
        $("#chkStatusEmailsRecebedor").prop("checked", false);
    }

    if (cliente.EmailContatoStatus == "A" || cliente.EmailContatoStatus == true) {
        $("#chkStatusEmailsContatoRecebedor").prop("checked", true);
    } else {
        $("#chkStatusEmailsContatoRecebedor").prop("checked", false);
    }

    if (cliente.EmailContadorStatus == "A" || cliente.EmailContadorStatus == true) {
        $("#chkStatusEmailsContadorRecebedor").prop("checked", true);
    } else {
        $("#chkStatusEmailsContadorRecebedor").prop("checked", false);
    }

    if (cliente.EmailTransportadorStatus == "A" || cliente.EmailTransportadorStatus == true) {
        $("#chkStatusEmailsTransportadorRecebedor").prop("checked", true);
    } else {
        $("#chkStatusEmailsTransportadorRecebedor").prop("checked", false);
    }

    $("#ddlEstadoRecebedor").val(cliente.UF);
    BuscarLocalidades(cliente.UF, 'selCidadeRecebedor', cliente.CodigoLocalidade);
    SetarIndicadorTomador();
}

function LimparCamposRecebedor() {
    $("#txtRGIERecebedor").val('');
    $("#txtRazaoSocialRecebedor").val('');
    $("#txtNomeFantasiaRecebedor").val('');
    $("#txtTelefone1Recebedor").val('').change();
    $("#txtTelefone2Recebedor").val('').change();
    $("#txtEnderecoRecebedor").val('');
    $("#txtNumeroRecebedor").val('');
    $("#txtBairroRecebedor").val('');
    $("#txtComplementoRecebedor").val('');
    $("#txtCEPRecebedor").val('');
    $("#txtEmailsRecebedor").val('');
    $("#chkStatusEmailsRecebedor").prop("checked", false);
    $("#txtEmailsContatoRecebedor").val('');
    $("#chkStatusEmailsContatoRecebedor").prop("checked", false);
    $("#txtEmailsContadorRecebedor").val('');
    $("#chkStatusEmailsContadorRecebedor").prop("checked", false);
    $("#txtAtividadeRecebedor").val('');
    $("#hddAtividadeRecebedor").val('0');
    $("#ddlEstadoRecebedor").val($("#ddlEstadoRecebedor option:first").val());
    $("#selCidadeRecebedor").html("");
    $("#ddlPaisRecebedor").val("01058");
    $("#selCidadeRecebedor").html("");
    $("#chkRecebedorExportacao").prop("checked", false);
    $("#chkSalvarEnderecoRecebedor").prop("checked", true);
    $("#txtCidadeRecebedorExportacao").val("");
    $("#txtEmailsTransportadorRecebedor").val('');
    $("#chkStatusEmailsTransportadorRecebedor").prop("checked", false);
    DesbloquearCamposRecebedorExportacao();
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    if (configuracaoEmpresa != null && configuracaoEmpresa.CodigoAtividade > 0)
        RetornoConsultaAtividadeRecebedor({ Codigo: configuracaoEmpresa.CodigoAtividade, Descricao: configuracaoEmpresa.DescricaoAtividade }, false);
    SetarTerminoPrestacao();
}
function BloquearCamposRecebedorExportacao() {
    $("#hddRecebedor").val('');
    $("#txtCPFCNPJRecebedor").val('');
    $("#txtCPFCNPJRecebedor").prop("disabled", true);
    $("#chkRecebedorExportacao").prop("checked", true);
    $("#chkSalvarEnderecoRecebedor").prop("disabled", true);
    $("#btnBuscarRecebedor").prop("disabled", true);
    $("#txtRGIERecebedor").prop("disabled", true);
    $("#txtNomeFantasiaRecebedor").prop("disabled", true);
    $("#txtTelefone1Recebedor").prop("disabled", true);
    $("#txtTelefone2Recebedor").prop("disabled", true);
    $("#txtAtividadeRecebedor").prop("disabled", true);
    $("#btnBuscarAtividadeRecebedor").prop("disabled", true);
    $("#txtCEPRecebedor").prop("disabled", true);
    $("#ddlEstadoRecebedor").prop("disabled", true);
    $("#selCidadeRecebedor").prop("disabled", true);
    $("#chkStatusEmailsRecebedor").prop("disabled", true);
    $("#txtEmailsContatoRecebedor").prop("disabled", true);
    $("#chkStatusEmailsContatoRecebedor").prop("disabled", true);
    $("#txtEmailsContadorRecebedor").prop("disabled", true);
    $("#chkStatusEmailsContadorRecebedor").prop("disabled", true);
    $("#ddlPaisRecebedor").prop("disabled", false);
    $("#divFieldCidadeExportacaoRecebedor").show();
    $("#divFieldCidadeRecebedor").hide();
}
function DesbloquearCamposRecebedorExportacao() {
    $("#chkSalvarEnderecoRecebedor").prop("disabled", false);
    $("#txtCPFCNPJRecebedor").prop("disabled", false);
    $("#btnBuscarRecebedor").prop("disabled", false);
    $("#txtRGIERecebedor").prop("disabled", false);
    $("#txtNomeFantasiaRecebedor").prop("disabled", false);
    $("#txtTelefone1Recebedor").prop("disabled", false);
    $("#txtTelefone2Recebedor").prop("disabled", false);
    $("#txtAtividadeRecebedor").prop("disabled", false);
    $("#btnBuscarAtividadeRecebedor").prop("disabled", false);
    $("#txtCEPRecebedor").prop("disabled", false);
    $("#ddlEstadoRecebedor").prop("disabled", false);
    $("#selCidadeRecebedor").prop("disabled", false);
    $("#chkStatusEmailsRecebedor").prop("disabled", false);
    $("#txtEmailsContatoRecebedor").prop("disabled", false);
    $("#chkStatusEmailsContatoRecebedor").prop("disabled", false);
    $("#txtEmailsContadorRecebedor").prop("disabled", false);
    $("#chkStatusEmailsContadorRecebedor").prop("disabled", false);
    $("#ddlPaisRecebedor").prop("disabled", true);
    $("#txtEmailsTransportadorRecebedor").prop("disabled", false);
    $("#chkStatusEmailsTransportadorRecebedor").prop("disabled", false);
    $("#divFieldCidadeExportacaoRecebedor").hide();
    $("#divFieldCidadeRecebedor").show();
}
function BuscarRecebedorExportacaoPorNome() {
    if ($("#chkRecebedorExportacao")[0].checked && $("#txtRazaoSocialRecebedor").val() != "") {
        executarRest("/ClienteExportacaoCTe/BuscarPorNome?callback=?", { Nome: $("#txtRazaoSocialRecebedor").val() }, function (r) {
            if (r.Sucesso) {
                PreencherCamposRecebedorExportacao(r.Objeto);
            }
        });
    }
}

function PreencherCamposRecebedorExportacao(dados) {
    BloquearCamposRecebedorExportacao();
    $("#txtBairroRecebedor").val(dados.Bairro);
    $("#txtCidadeRecebedorExportacao").val(dados.Cidade);
    $("#txtComplementoRecebedor").val(dados.Complemento);
    $("#txtEmailsRecebedor").val(dados.Email);
    $("#txtEnderecoRecebedor").val(dados.Endereco);
    $("#chkRecebedorExportacao").prop("checked", true);
    $("#txtRazaoSocialRecebedor").val(dados.Nome);
    $("#txtNumeroRecebedor").val(dados.Numero);
    $("#ddlPaisRecebedor").val(dados.SiglaPais);
    $("#ddlEstadoRecebedor").val("EX");
    $("#txtRGIERecebedor").val(dados.IE_RG);
}