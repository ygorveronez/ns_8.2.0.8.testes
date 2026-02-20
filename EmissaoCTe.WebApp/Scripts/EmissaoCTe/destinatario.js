$(document).ready(function () {
    $("#txtCEPDestinatario").mask("99.999-999");
    $("#txtCEP_LocalEntregaDiferenteDestinatario").mask("99.999-999");

    CarregarConsultaDeAtividades("btnBuscarAtividadeDestinatario", "btnBuscarAtividadeDestinatario", RetornoConsultaAtividadeDestinatario, true, false);
    CarregarConsultadeClientes("btnBuscarDestinatario", "btnBuscarDestinatario", RetornoConsultaDestinatario, true, false);

    $("#txtCPFCNPJDestinatario").focusout(function () {
        BuscarDestinatario(true);
    });

    $("#ddlEstadoDestinatario").change(function () {
        $("#ddlUFTerminoPrestacao").val($(this).val());
        BuscarLocalidades($(this).val(), ["selCidadeDestinatario", "ddlMunicipioTerminoPrestacao"]);
    });

    $("#selCidadeDestinatario").change(function () {
        SetarTerminoPrestacao();
    });

    $("#chkDestinatarioExportacao").click(function () {
        var checked = $(this)[0].checked;
        LimparCamposDestinatario();
        if (checked) {
            BloquearCamposDestinatarioExportacao();
            $("#ddlEstadoDestinatario").val("EX");
            $("#ddlUFTerminoPrestacao").val("EX");
            BuscarLocalidades("EX", ["ddlMunicipioTerminoPrestacao", "selCidadeDestinatario"]);
        }
    });

    $("#txtRazaoSocialDestinatario").focusout(function () {
        BuscarDestinatarioExportacaoPorNome();
    });

    $("#ddlUFLocalEntregaDiferenteDestinatario").change(function () {
        BuscarLocalidades($("#ddlUFLocalEntregaDiferenteDestinatario").val(), "selLocalidade_LocalEntregaDiferenteDestinatario");
    });

    $("#txtCPFCNPJ_LocalEntregaDiferenteDestinatario").focusout(function () {
        BuscarLocalEntregaDiferenteDestinatario();
    });

    $("#txtRGIEDestinatario").on('change', function () {
        SetarIndicadorTomador();
    });

    CarregarConsultadeClientes("btnBuscarLocalEntregaDiferenteDestinatario", "btnBuscarLocalEntregaDiferenteDestinatario", RetornoConsultaClienteLocalDiferenteDestinatario, true, false);
    CarregarConsultaDeAtividades("btnBuscarAtividadeLocalEntregaDiferenteDestinatario", "btnBuscarAtividadeLocalEntregaDiferenteDestinatario", RetornoConsultaAtividadeLocalDiferenteDestinatario, true, false);
});

function RetornoConsultaAtividadeLocalDiferenteDestinatario(atividade) {
    $("#hddAtividadeLocalEntregaDiferenteDestinatario").val(atividade.Codigo);
    $("#txtAtividade_LocalEntregaDiferenteDestinatario").val(atividade.Codigo + " - " + atividade.Descricao);

    if(atividade.Codigo == 7)
        $("#txtRGIE_LocalEntregaDiferenteDestinatario").val("ISENTO");
}

function RetornoConsultaClienteLocalDiferenteDestinatario(cliente) {
    BuscarLocalEntregaDiferenteDestinatario(cliente.CPFCNPJ);
}

function BuscarLocalEntregaDiferenteDestinatario(cpfCnpj) {
    if (cpfCnpj == null || cpfCnpj == "")
        cpfCnpj = $("#txtCPFCNPJ_LocalEntregaDiferenteDestinatario").val();
    cpfCnpj = cpfCnpj.replace(/[^0-9]/g, '');
    if (cpfCnpj != "") {
        if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        $("#hddLocalEntregaDiferenteDestinatario").val(r.Objeto.CPF_CNPJ);
                        $("#hddAtividadeLocalEntregaDiferenteDestinatario").val(r.Objeto.CodigoAtividade);
                        $("#txtAtividade_LocalEntregaDiferenteDestinatario").val(r.Objeto.CodigoAtividade + " - " + r.Objeto.DescricaoAtividade);
                        $("#txtCPFCNPJ_LocalEntregaDiferenteDestinatario").val(r.Objeto.CPF_CNPJ);
                        $("#txtRGIE_LocalEntregaDiferenteDestinatario").val(r.Objeto.IE_RG);
                        $("#txtRazaoSocial_LocalEntregaDiferenteDestinatario").val(r.Objeto.Nome);
                        $("#txtNomeFantasia_LocalEntregaDiferenteDestinatario").val(r.Objeto.NomeFantasia);
                        $("#txtTelefone1_LocalEntregaDiferenteDestinatario").val(r.Objeto.Telefone1).change();
                        $("#txtTelefone2_LocalEntregaDiferenteDestinatario").val(r.Objeto.Telefone2).change();
                        $("#txtLogradouro_LocalEntregaDiferenteDestinatario").val(r.Objeto.Endereco);
                        $("#txtNumero_LocalEntregaDiferenteDestinatario").val(r.Objeto.Numero);
                        $("#txtBairro_LocalEntregaDiferenteDestinatario").val(r.Objeto.Bairro);
                        $("#txtComplemento_LocalEntregaDiferenteDestinatario").val(r.Objeto.Complemento);
                        $("#txtCEP_LocalEntregaDiferenteDestinatario").val(r.Objeto.CEP);
                        $("#ddlUFLocalEntregaDiferenteDestinatario").val(r.Objeto.UF);
                        BuscarLocalidades($("#ddlUFLocalEntregaDiferenteDestinatario").val(), 'selLocalidade_LocalEntregaDiferenteDestinatario', r.Objeto.CodigoLocalidade);
                    } else {
                        LimparCamposLocalEntregaDiferenteDestinatario();
                        jAlert("<b>Local de entrega não encontrado.</b><br/><br/> Preencha corretamente os campos e o local de entrega será cadastrado ao salvar/emitir o CT-e.", "Atenção");
                        $("#hddLocalEntregaDiferenteDestinatario").val($("#txtCPFCNPJ_LocalEntregaDiferenteDestinatario").val());
                    }
                } else {
                    LimparCamposLocalEntregaDiferenteDestinatario();
                    jAlert(r.Erro, "Erro");
                    $("#hddLocalEntregaDiferenteDestinatario").val('');
                    $("#txtCPFCNPJ_LocalEntregaDiferenteDestinatario").val('');
                }
            });
        } else {
            jAlert("O CPF/CNPJ digitado é inválido.", "Atenção");
            $("#hddLocalEntregaDiferenteDestinatario").val('');
            $("#txtCPFCNPJ_LocalEntregaDiferenteDestinatario").val('');
        }
    } else {
        LimparCamposLocalEntregaDiferenteDestinatario();
        $("#hddLocalEntregaDiferenteDestinatario").val('');
        $("#txtCPFCNPJ_LocalEntregaDiferenteDestinatario").val('');
    }
}

function LimparCamposLocalEntregaDiferenteDestinatario() {
    $("#hddLocalEntregaDiferenteDestinatario").val("");
    $("#hddAtividadeLocalEntregaDiferenteDestinatario").val("0");
    $("#txtAtividade_LocalEntregaDiferenteDestinatario").val("");
    $("#txtCPFCNPJ_LocalEntregaDiferenteDestinatario").val("");
    $("#txtRGIE_LocalEntregaDiferenteDestinatario").val("");
    $("#txtRazaoSocial_LocalEntregaDiferenteDestinatario").val("");
    $("#txtNomeFantasia_LocalEntregaDiferenteDestinatario").val("");
    $("#txtTelefone1_LocalEntregaDiferenteDestinatario").val("").change();
    $("#txtTelefone2_LocalEntregaDiferenteDestinatario").val("").change();
    $("#txtLogradouro_LocalEntregaDiferenteDestinatario").val("");
    $("#txtNumero_LocalEntregaDiferenteDestinatario").val("");
    $("#txtBairro_LocalEntregaDiferenteDestinatario").val("");
    $("#txtComplemento_LocalEntregaDiferenteDestinatario").val("");
    $("#txtCEP_LocalEntregaDiferenteDestinatario").val("");
    $("#ddlUFLocalEntregaDiferenteDestinatario").val($("#ddlUFLocalEntregaDiferenteDestinatario option:first").val());
    $("#selLocalidade_LocalEntregaDiferenteDestinatario").html("");

    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());

    if (configuracaoEmpresa != null && configuracaoEmpresa.CodigoAtividade > 0)
        RetornoConsultaAtividadeLocalDiferenteDestinatario({ Codigo: configuracaoEmpresa.CodigoAtividade, Descricao: configuracaoEmpresa.DescricaoAtividade });
}

function RetornoConsultaAtividadeDestinatario(atividade, naoAtualizarIndicadorTomador) {
    $("#hddAtividadeDestinatario").val(atividade.Codigo);
    $("#txtAtividadeDestinatario").val(atividade.Codigo + " - " + atividade.Descricao);

    if (atividade.Codigo == 7)
        $("#txtRGIEDestinatario").val("ISENTO");

    if (!(naoAtualizarIndicadorTomador === false))
        SetarIndicadorTomador();
}

function RetornoConsultaDestinatario(cliente) {
    BuscarDestinatario(true, cliente.CPFCNPJ);
}

function BuscarDestinatario(carregarLocalidade, cpfCnpj, dados, naoSalvarEndereco, atualizarIndicadorTomador) {
    if (dados != null) {
        PreencherCamposDestinatario(dados, naoSalvarEndereco);
        return;
    }

    if (cpfCnpj == null || cpfCnpj == "")
        cpfCnpj = $("#txtCPFCNPJDestinatario").val();

    cpfCnpj = cpfCnpj.replace(/[^0-9]/g, '');

    if (cpfCnpj != "") {
        if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        PreencherCamposDestinatario(r.Objeto);

                        if (carregarLocalidade) {
                            $("#ddlUFTerminoPrestacao").val(r.Objeto.UF);

                            $.ajaxSetup({ async: false });
                            BuscarLocalidades(r.Objeto.UF, "ddlMunicipioTerminoPrestacao", r.Objeto.CodigoLocalidade);
                            //BuscarFrete();
                            ObterDadosSemelhantesPorEstado();
                            $.ajaxSetup({ async: true });
                        }

                        if (atualizarIndicadorTomador)
                            SetarIndicadorTomador();
                    } else {
                        LimparCamposDestinatario();
                        jAlert("<b>Destinatário não encontrado.</b><br/><br/> Preencha corretamente os campos e o destinatário será cadastrado ao salvar/emitir o CT-e.", "Atenção");
                        $("#hddDestinatario").val($("#txtCPFCNPJDestinatario").val());
                    }
                } else {
                    LimparCamposDestinatario();
                    jAlert(r.Erro, "Erro");
                    $("#hddDestinatario").val('');
                    $("#txtCPFCNPJDestinatario").val('');
                }

                if (carregarLocalidade) {
                    BuscarApoliceDeSeguroDoTomador();
                    //BuscarFrete();
                }
            });
        } else {
            jAlert("O CPF/CNPJ digitado é inválido.", "Atenção");
            LimparCamposDestinatario();
            $("#hddDestinatario").val('');
            $("#txtCPFCNPJDestinatario").val('');
        }
    } else {
        LimparCamposDestinatario();
        $("#hddDestinatario").val('');
        $("#txtCPFCNPJDestinatario").val('');
    }

    if (carregarLocalidade) {
        BuscarApoliceDeSeguroDoTomador();
        //BuscarFrete();
    }
}

function PreencherCamposDestinatario(cliente, naoSalvarEndereco) {
    if (typeof naoSalvarEndereco == "undefined")
        naoSalvarEndereco = false;

    $("#hddDestinatario").val(cliente.CPF_CNPJ);
    $("#hddAtividadeDestinatario").val(cliente.CodigoAtividade);
    $("#txtAtividadeDestinatario").val(cliente.CodigoAtividade + " - " + cliente.DescricaoAtividade);
    $("#txtCPFCNPJDestinatario").val(cliente.CPF_CNPJ);
    $("#txtRGIEDestinatario").val(cliente.IE_RG);
    $("#txtRazaoSocialDestinatario").val(cliente.Nome);
    $("#txtNomeFantasiaDestinatario").val(cliente.NomeFantasiaTransportador != null && cliente.NomeFantasiaTransportador != "" ? cliente.NomeFantasiaTransportador : cliente.NomeFantasia);
    $("#txtTelefone1Destinatario").val(cliente.Telefone1).change();
    $("#txtTelefone2Destinatario").val(cliente.Telefone2).change();
    $("#txtEnderecoDestinatario").val(cliente.Endereco);
    $("#txtNumeroDestinatario").val(cliente.Numero);
    $("#txtBairroDestinatario").val(cliente.Bairro);
    $("#txtComplementoDestinatario").val(cliente.Complemento);
    $("#txtCEPDestinatario").val(cliente.CEP);
    $("#txtEmailsDestinatario").val(cliente.Email);
    $("#txtEmailsContatoDestinatario").val(cliente.EmailContato);
    $("#txtEmailsContadorDestinatario").val(cliente.EmailContador);
    $("#chkSalvarEnderecoDestinatario").prop("checked", naoSalvarEndereco ? false : cliente.SalvarEndereco);
    $("#txtEmailsTransportadorDestinatario").val(cliente.EmailTransportador);
    $("#txtSuframaDestinatario").val(cliente.InscricaoSuframa);

    if (cliente.EmailStatus == "A" || cliente.EmailStatus == true) {
        $("#chkStatusEmailsDestinatario").prop("checked", true);
    } else {
        $("#chkStatusEmailsDestinatario").prop("checked", false);
    }

    if (cliente.EmailContatoStatus == "A" || cliente.EmailContatoStatus == true) {
        $("#chkStatusEmailsContatoDestinatario").prop("checked", true);
    } else {
        $("#chkStatusEmailsContatoDestinatario").prop("checked", false);
    }

    if (cliente.EmailContadorStatus == "A" || cliente.EmailContadorStatus == true) {
        $("#chkStatusEmailsContadorDestinatario").prop("checked", true);
    } else {
        $("#chkStatusEmailsContadorDestinatario").prop("checked", false);
    }

    if (cliente.EmailTransportadorStatus == "A" || cliente.EmailTransportadorStatus == true) {
        $("#chkStatusEmailsTransportadorDestinatario").prop("checked", true);
    } else {
        $("#chkStatusEmailsTransportadorDestinatario").prop("checked", false);
    }

    $("#ddlEstadoDestinatario").val(cliente.UF);
    BuscarLocalidades($("#ddlEstadoDestinatario").val(), 'selCidadeDestinatario', cliente.CodigoLocalidade);
    SetarIndicadorTomador();
}

function LimparCamposDestinatario() {
    $("#txtRGIEDestinatario").val('');
    $("#txtRazaoSocialDestinatario").val('');
    $("#txtNomeFantasiaDestinatario").val('');
    $("#txtTelefone1Destinatario").val('').change();
    $("#txtTelefone2Destinatario").val('').change();
    $("#txtEnderecoDestinatario").val('');
    $("#txtNumeroDestinatario").val('');
    $("#txtBairroDestinatario").val('');
    $("#txtComplementoDestinatario").val('');
    $("#txtCEPDestinatario").val('');
    $("#txtEmailsDestinatario").val('');
    $("#chkStatusEmailsDestinatario").prop("checked", false);
    $("#txtEmailsContatoDestinatario").val('');
    $("#chkStatusEmailsContatoDestinatario").prop("checked", false);
    $("#txtEmailsContadorDestinatario").val('');
    $("#chkStatusEmailsContadorDestinatario").prop("checked", false);
    $("#hddAtividadeDestinatario").val('0');
    $("#txtAtividadeDestinatario").val('');
    $("#ddlEstadoDestinatario").val($("#ddlEstadoDestinatario option:first").val());
    $("#selCidadeDestinatario").html("");
    $("#ddlPaisDestinatario").val("01058");
    $("#selCidadeDestinatario").html("");
    $("#chkDestinatarioExportacao").prop("checked", false);
    $("#chkSalvarEnderecoDestinatario").prop("checked", true);
    $("#txtCidadeDestinatarioExportacao").val("");
    $("#txtEmailsTransportadorDestinatario").val('');
    $("#chkStatusEmailsTransportadorDestinatario").prop("checked", false);
    $("#txtSuframaDestinatario").val('');
    DesbloquearCamposDestinatarioExportacao();
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    if (configuracaoEmpresa != null && configuracaoEmpresa.CodigoAtividade > 0)
        RetornoConsultaAtividadeDestinatario({ Codigo: configuracaoEmpresa.CodigoAtividade, Descricao: configuracaoEmpresa.DescricaoAtividade }, false);
}

function BloquearCamposDestinatarioExportacao() {
    $("#hddDestinatario").val('');
    $("#txtCPFCNPJDestinatario").val('');
    $("#txtCPFCNPJDestinatario").prop("disabled", true);
    $("#chkDestinatarioExportacao").prop("checked", true);
    $("#chkSalvarEnderecoDestinatario").prop("disabled", true);
    $("#btnBuscarDestinatario").prop("disabled", true);
    $("#txtRGIEDestinatario").prop("disabled", true);
    $("#txtNomeFantasiaDestinatario").prop("disabled", true);
    $("#txtTelefone1Destinatario").prop("disabled", true);
    $("#txtTelefone2Destinatario").prop("disabled", true);
    $("#txtAtividadeDestinatario").prop("disabled", true);
    $("#btnBuscarAtividadeDestinatario").prop("disabled", true);
    $("#txtCEPDestinatario").prop("disabled", true);
    $("#ddlEstadoDestinatario").prop("disabled", true);
    $("#selCidadeDestinatario").prop("disabled", true);
    $("#chkStatusEmailsDestinatario").prop("disabled", true);
    $("#txtEmailsContatoDestinatario").prop("disabled", true);
    $("#chkStatusEmailsContatoDestinatario").prop("disabled", true);
    $("#txtEmailsContadorDestinatario").prop("disabled", true);
    $("#chkStatusEmailsContadorDestinatario").prop("disabled", true);
    $("#ddlPaisDestinatario").prop("disabled", false);
    $("#divFieldCidadeExportacaoDestinatario").show();
    $("#txtEmailsTransportadorDestinatario").prop("disabled", true);
    $("#chkStatusEmailsTransportadorDestinatario").prop("disabled", true);
    $("#txtSuframaDestinatario").prop("disabled", true);
    $("#divFieldCidadeDestinatario").hide();
}

function DesbloquearCamposDestinatarioExportacao() {
    $("#chkSalvarEnderecoDestinatario").prop("disabled", false);
    $("#txtCPFCNPJDestinatario").prop("disabled", false);
    $("#btnBuscarDestinatario").prop("disabled", false);
    $("#txtRGIEDestinatario").prop("disabled", false);
    $("#txtNomeFantasiaDestinatario").prop("disabled", false);
    $("#txtTelefone1Destinatario").prop("disabled", false);
    $("#txtTelefone2Destinatario").prop("disabled", false);
    $("#txtAtividadeDestinatario").prop("disabled", false);
    $("#btnBuscarAtividadeDestinatario").prop("disabled", false);
    $("#txtCEPDestinatario").prop("disabled", false);
    $("#ddlEstadoDestinatario").prop("disabled", false);
    $("#selCidadeDestinatario").prop("disabled", false);
    $("#chkStatusEmailsDestinatario").prop("disabled", false);
    $("#txtEmailsContatoDestinatario").prop("disabled", false);
    $("#chkStatusEmailsContatoDestinatario").prop("disabled", false);
    $("#txtEmailsContadorDestinatario").prop("disabled", false);
    $("#chkStatusEmailsContadorDestinatario").prop("disabled", false);
    $("#ddlPaisDestinatario").prop("disabled", true);
    $("#divFieldCidadeExportacaoDestinatario").hide();
    $("#txtEmailsTransportadorDestinatario").prop("disabled", false);
    $("#chkStatusEmailsTransportadorDestinatario").prop("disabled", false);
    $("#txtSuframaDestinatario").prop("disabled", false);
    $("#divFieldCidadeDestinatario").show();
}

function BuscarDestinatarioExportacaoPorNome() {
    if ($("#chkDestinatarioExportacao")[0].checked && $("#txtRazaoSocialDestinatario").val() != "") {
        executarRest("/ClienteExportacaoCTe/BuscarPorNome?callback=?", { Nome: $("#txtRazaoSocialDestinatario").val() }, function (r) {
            if (r.Sucesso) {
                PreencherCamposDestinatarioExportacao(r.Objeto);
            }
        });
    }
}

function PreencherCamposDestinatarioExportacao(dados) {
    $("#txtBairroDestinatario").val(dados.Bairro);
    $("#txtCidadeDestinatarioExportacao").val(dados.Cidade);
    $("#txtComplementoDestinatario").val(dados.Complemento);
    $("#txtEmailsDestinatario").val(dados.Email);
    $("#txtEnderecoDestinatario").val(dados.Endereco);
    $("#chkDestinatarioExportacao").prop("checked", true);
    $("#txtRazaoSocialDestinatario").val(dados.Nome);
    $("#txtNumeroDestinatario").val(dados.Numero);
    $("#ddlPaisDestinatario").val(dados.SiglaPais);
    $("#ddlEstadoDestinatario").val("EX");
    $("#txtRGIEDestinatario").val(dados.IE_RG);
    BloquearCamposDestinatarioExportacao();
}