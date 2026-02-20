$(document).ready(function () {
    $("#txtCEPExpedidor").mask("99.999-999");
    $("#txtCPFCNPJExpedidor").focusout(function () {
        BuscarExpedidor(true);
    });
    CarregarConsultadeClientes("btnBuscarExpedidor", "btnBuscarExpedidor", RetornoConsultaExpedidor, true, false);
    CarregarConsultaDeAtividades("btnBuscarAtividadeExpedidor", "btnBuscarAtividadeExpedidor", RetornoConsultaAtividadeExpedidor, true, false);
    $("#ddlEstadoExpedidor").change(function () {
        BuscarLocalidades($(this).val(), "selCidadeExpedidor");
    });
    $("#chkExpedidorExportacao").click(function () {
        var checked = $(this)[0].checked;
        LimparCamposExpedidor();
        if (checked) {
            BloquearCamposExpedidorExportacao();
            $("#ddlEstadoExpedidor").val("EX");
            BuscarLocalidades("EX", "selCidadeExpedidor");
        }
    });
    $("#txtRazaoSocialExpedidor").focusout(function () {
        BuscarExpedidorExportacaoPorNome();
    });

    $("#txtRGIEExpedidor").on('change', function () {
        SetarIndicadorTomador();
    });

    $("#selCidadeExpedidor").change(function () {
        //$("#ddlMunicipioInicioPrestacao").val($(this).val());
        SetarInicioPrestacao();
    });
});
function RetornoConsultaAtividadeExpedidor(atividade, naoAtualizarIndicadorTomador) {
    $("#hddAtividadeExpedidor").val(atividade.Codigo);
    $("#txtAtividadeExpedidor").val(atividade.Codigo + " - " + atividade.Descricao);

    if (atividade.Codigo == 7)
        $("#txtRGIEExpedidor").val("ISENTO");

    if (!(naoAtualizarIndicadorTomador === false))
        SetarIndicadorTomador();
}
function RetornoConsultaExpedidor(cliente) {
    BuscarExpedidor(true, cliente.CPFCNPJ);
}
function BuscarExpedidor(carregarLocalidade, cpfCnpj, dados, atualizarIndicadorTomador) {
    if (dados != null) {
        PreencherCamposExpedidor(dados);
        return;
    }

    if (cpfCnpj == null || cpfCnpj == "")
        cpfCnpj = $("#txtCPFCNPJExpedidor").val();

    cpfCnpj = cpfCnpj.replace(/[^0-9]/g, '');

    if (cpfCnpj != "") {
        if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        PreencherCamposExpedidor(r.Objeto);

                        $.ajaxSetup({ async: false });
                        ObterDadosSemelhantesPorEstado();
                        $.ajaxSetup({ async: true });

                        if (atualizarIndicadorTomador)
                            SetarIndicadorTomador();
                    } else {
                        jAlert("<b>Expedidor não encontrado.</b><br/><br/> Preencha corretamente os campos e o expedidor será cadastrado ao salvar/emitir o CT-e.", "Atenção");
                        LimparCamposExpedidor();
                        $("#hddExpedidor").val($("#txtCPFCNPJExpedidor").val());
                    }
                } else {
                    jAlert(r.Erro, "Erro");
                    LimparCamposExpedidor();
                    $("#hddExpedidor").val('');
                    $("#txtCPFCNPJExpedidor").val('');
                }
                if (carregarLocalidade)
                    BuscarApoliceDeSeguroDoTomador();
            });
        } else {
            jAlert("O CPF/CNPJ digitado é inválido.", "Atenção");
            LimparCamposExpedidor();
            $("#hddExpedidor").val('');
            $("#txtCPFCNPJExpedidor").val('');
        }
    } else {
        LimparCamposExpedidor();
        $("#hddExpedidor").val('');
        $("#txtCPFCNPJExpedidor").val('');
    }
    if (carregarLocalidade)
        BuscarApoliceDeSeguroDoTomador();
}

function PreencherCamposExpedidor(cliente) {
    $("#hddExpedidor").val(cliente.CPF_CNPJ);
    $("#txtAtividadeExpedidor").val(cliente.CodigoAtividade + " - " + cliente.DescricaoAtividade);
    $("#hddAtividadeExpedidor").val(cliente.CodigoAtividade);
    $("#txtCPFCNPJExpedidor").val(cliente.CPF_CNPJ);
    $("#txtRGIEExpedidor").val(cliente.IE_RG);
    $("#txtRazaoSocialExpedidor").val(cliente.Nome);
    $("#txtNomeFantasiaExpedidor").val(cliente.NomeFantasiaTransportador != null && cliente.NomeFantasiaTransportador != "" ? cliente.NomeFantasiaTransportador : cliente.NomeFantasia);
    $("#txtTelefone1Expedidor").val(cliente.Telefone1).change();
    $("#txtTelefone2Expedidor").val(cliente.Telefone2).change();
    $("#txtEnderecoExpedidor").val(cliente.Endereco);
    $("#txtNumeroExpedidor").val(cliente.Numero);
    $("#txtBairroExpedidor").val(cliente.Bairro);
    $("#txtComplementoExpedidor").val(cliente.Complemento);
    $("#txtCEPExpedidor").val(cliente.CEP);
    $("#txtEmailsExpedidor").val(cliente.Email);
    $("#txtEmailsContatoExpedidor").val(cliente.EmailContato);
    $("#txtEmailsContadorExpedidor").val(cliente.EmailContador);
    $("#txtEmailsTransportadorExpedidor").val(cliente.EmailTransportador);
    $("#chkSalvarEnderecoExpedidor").prop("checked", cliente.SalvarEndereco);

    if (cliente.EmailStatus == "A" || cliente.EmailStatus == true) {
        $("#chkStatusEmailsExpedidor").prop("checked", true);
    } else {
        $("#chkStatusEmailsExpedidor").prop("checked", false);
    }

    if (cliente.EmailContatoStatus == "A" || cliente.EmailContatoStatus == true) {
        $("#chkStatusEmailsContatoExpedidor").prop("checked", true);
    } else {
        $("#chkStatusEmailsContatoExpedidor").prop("checked", false);
    }

    if (cliente.EmailContadorStatus == "A" || cliente.EmailContadorStatus == true) {
        $("#chkStatusEmailsContadorExpedidor").prop("checked", true);
    } else {
        $("#chkStatusEmailsContadorExpedidor").prop("checked", false);
    }

    if (cliente.EmailTransportadorStatus == "A" || cliente.EmailTransportadorStatus == true) {
        $("#chkStatusEmailsTransportadorExpedidor").prop("checked", true);
    } else {
        $("#chkStatusEmailsTransportadorExpedidor").prop("checked", false);
    }

    $("#ddlEstadoExpedidor").val(cliente.UF);
    BuscarLocalidades(cliente.UF, 'selCidadeExpedidor', cliente.CodigoLocalidade);
    SetarIndicadorTomador();
}

function LimparCamposExpedidor() {
    $("#txtRGIEExpedidor").val('');
    $("#txtRazaoSocialExpedidor").val('');
    $("#txtNomeFantasiaExpedidor").val('');
    $("#txtTelefone1Expedidor").val('').change();
    $("#txtTelefone2Expedidor").val('').change();
    $("#txtEnderecoExpedidor").val('');
    $("#txtNumeroExpedidor").val('');
    $("#txtBairroExpedidor").val('');
    $("#txtComplementoExpedidor").val('');
    $("#txtCEPExpedidor").val('');
    $("#txtEmailsExpedidor").val('');
    $("#chkStatusEmailsExpedidor").prop("checked", false);
    $("#txtEmailsContatoExpedidor").val('');
    $("#chkStatusEmailsContatoExpedidor").prop("checked", false);
    $("#txtEmailsContadorExpedidor").val('');
    $("#chkStatusEmailsContadorExpedidor").prop("checked", false);
    $("#hddAtividadeExpedidor").val('0');
    $("#txtAtividadeExpedidor").val('');
    $("#ddlEstadoExpedidor").val($("#ddlEstadoExpedidor option:first").val());
    $("#ddlPaisExpedidor").val("01058");
    $("#selCidadeExpedidor").html("");
    $("#chkExpedidorExportacao").prop("checked", false);
    $("#chkSalvarEnderecoExpedidor").prop("checked", true);
    $("#txtCidadeExpedidorExportacao").val("");
    $("#txtEmailsTransportadorExpedidor").val('');
    $("#chkStatusEmailsTransportadorExpedidor").prop("checked", false);
    DesbloquearCamposExpedidorExportacao();
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    if (configuracaoEmpresa != null && configuracaoEmpresa.CodigoAtividade > 0)
        RetornoConsultaAtividadeExpedidor({ Codigo: configuracaoEmpresa.CodigoAtividade, Descricao: configuracaoEmpresa.DescricaoAtividade }, false);
    SetarInicioPrestacao();
}
function BloquearCamposExpedidorExportacao() {
    $("#hddExpedidor").val('');
    $("#txtCPFCNPJExpedidor").val('');
    $("#txtCPFCNPJExpedidor").prop("disabled", true);
    $("#chkExpedidorExportacao").prop("checked", true);
    $("#chkSalvarEnderecoExpedidor").prop("disabled", true);
    $("#btnBuscarExpedidor").prop("disabled", true);
    $("#txtRGIEExpedidor").prop("disabled", true);
    $("#txtNomeFantasiaExpedidor").prop("disabled", true);
    $("#txtTelefone1Expedidor").prop("disabled", true);
    $("#txtTelefone2Expedidor").prop("disabled", true);
    $("#txtAtividadeExpedidor").prop("disabled", true);
    $("#btnBuscarAtividadeExpedidor").prop("disabled", true);
    $("#txtCEPExpedidor").prop("disabled", true);
    $("#ddlEstadoExpedidor").prop("disabled", true);
    $("#selCidadeExpedidor").prop("disabled", true);
    $("#chkStatusEmailsExpedidor").prop("disabled", true);
    $("#txtEmailsContatoExpedidor").prop("disabled", true);
    $("#chkStatusEmailsContatoExpedidor").prop("disabled", true);
    $("#txtEmailsContadorExpedidor").prop("disabled", true);
    $("#chkStatusEmailsContadorExpedidor").prop("disabled", true);
    $("#ddlPaisExpedidor").prop("disabled", false);
    $("#divFieldCidadeExportacaoExpedidor").show();
    $("#txtEmailsTransportadorExpedidor").prop("disabled", true);
    $("#chkStatusEmailsTransportadorExpedidor").prop("disabled", true);
    $("#divFieldCidadeExpedidor").hide();
}
function DesbloquearCamposExpedidorExportacao() {
    $("#chkSalvarEnderecoExpedidor").prop("disabled", false);
    $("#txtCPFCNPJExpedidor").prop("disabled", false);
    $("#btnBuscarExpedidor").prop("disabled", false);
    $("#txtRGIEExpedidor").prop("disabled", false);
    $("#txtNomeFantasiaExpedidor").prop("disabled", false);
    $("#txtTelefone1Expedidor").prop("disabled", false);
    $("#txtTelefone2Expedidor").prop("disabled", false);
    $("#txtAtividadeExpedidor").prop("disabled", false);
    $("#btnBuscarAtividadeExpedidor").prop("disabled", false);
    $("#txtCEPExpedidor").prop("disabled", false);
    $("#ddlEstadoExpedidor").prop("disabled", false);
    $("#selCidadeExpedidor").prop("disabled", false);
    $("#chkStatusEmailsExpedidor").prop("disabled", false);
    $("#txtEmailsContatoExpedidor").prop("disabled", false);
    $("#chkStatusEmailsContatoExpedidor").prop("disabled", false);
    $("#txtEmailsContadorExpedidor").prop("disabled", false);
    $("#chkStatusEmailsContadorExpedidor").prop("disabled", false);
    $("#ddlPaisExpedidor").prop("disabled", true);
    $("#txtEmailsTransportadorExpedidor").prop("disabled", false);
    $("#chkStatusEmailsTransportadorExpedidor").prop("disabled", false);
    $("#divFieldCidadeExportacaoExpedidor").hide();
    $("#divFieldCidadeExpedidor").show();
}
function BuscarExpedidorExportacaoPorNome() {
    if ($("#chkExpedidorExportacao")[0].checked && $("#txtRazaoSocialExpedidor").val() != "") {
        executarRest("/ClienteExportacaoCTe/BuscarPorNome?callback=?", { Nome: $("#txtRazaoSocialExpedidor").val() }, function (r) {
            if (r.Sucesso) {
                PreencherCamposExpedidorExportacao(r.Objeto);
            }
        });
    }
}

function PreencherCamposExpedidorExportacao(dados) {
    BloquearCamposExpedidorExportacao();
    $("#txtBairroExpedidor").val(dados.Bairro);
    $("#txtCidadeExpedidorExportacao").val(dados.Cidade);
    $("#txtComplementoExpedidor").val(dados.Complemento);
    $("#txtEmailsExpedidor").val(dados.Email);
    $("#txtEnderecoExpedidor").val(dados.Endereco);
    $("#chkExpedidorExportacao").prop("checked", true);
    $("#txtRazaoSocialExpedidor").val(dados.Nome);
    $("#txtNumeroExpedidor").val(dados.Numero);
    $("#ddlPaisExpedidor").val(dados.SiglaPais);
    $("#ddlEstadoExpedidor").val("EX");
    $("#txtRGIEExpedidor").val(dados.IE_RG);
}