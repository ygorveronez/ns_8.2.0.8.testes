$(document).ready(function () {
    $("#txtCEPRemetente").mask("99.999-999");

    CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaRemetente, true, false);

    $("#txtCPFCNPJRemetente").focusout(function () {
        BuscarRemetente(true, null, null, true, true);
    });

    $("#selTipoDocumentoRemetente").change(function () {
        BloquearAbasRemetente();
    });

    $("#ddlEstadoRemetente").change(function () {
        $("#ddlUFInicioPrestacao").val($(this).val());
        BuscarLocalidades($(this).val(), ["selCidadeRemetente", "ddlMunicipioInicioPrestacao"]);
    });

    $("#selCidadeRemetente").change(function () {
        //$("#ddlMunicipioInicioPrestacao").val($(this).val());
        SetarInicioPrestacao();
    });

    $("#chkRemetenteExportacao").click(function () {
        var checked = $(this)[0].checked;
        LimparCamposRemetente();
        if (checked) {
            BloquearCamposRemetenteExportacao();
            $("#ddlEstadoRemetente").val("EX");
            $("#ddlUFInicioPrestacao").val("EX");
            BuscarLocalidades("EX", ["selCidadeRemetente", "ddlMunicipioInicioPrestacao"]);
        }
    });

    $("#txtRGIERemetente").on('change', function () {
        SetarIndicadorTomador();
    });

    $("#txtRazaoSocialRemetente").focusout(function () {
        BuscarRemetenteExportacaoPorNome();
    });

    CarregarConsultaDeAtividades("btnBuscarAtividadeRemetente", "btnBuscarAtividadeRemetente", RetornoConsultaAtividadeRemetente, true, false);
});

function RetornoConsultaAtividadeRemetente(atividade, naoAtualizarIndicadorTomador) {
    $("#hddAtividadeRemetente").val(atividade.Codigo);
    $("#txtAtividadeRemetente").val(atividade.Codigo + " - " + atividade.Descricao);

    if (atividade.Codigo == 7)
        $("#txtRGIERemetente").val("ISENTO");

    if (!(naoAtualizarIndicadorTomador === false))
        SetarIndicadorTomador();
}

function RetornoConsultaRemetente(cliente) {
    BuscarRemetente(true, cliente.CPFCNPJ, null, null, true);
}

function BuscarRemetente(carregarLocalidade, cpfCnpj, dados, naoSalvarEndereco, atualizarIndicadorTomador) {
    if (dados != null) {
        PreencherCamposRemetente(dados, naoSalvarEndereco);
        BuscarDadosRemetente();
        AtualizarValoresGerais();
        return;
    }

    if (cpfCnpj == null || cpfCnpj == "")
        cpfCnpj = $("#txtCPFCNPJRemetente").val();

    cpfCnpj = cpfCnpj.replace(/[^0-9]/g, '');

    if (cpfCnpj != "") {
        if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        $("#hddDadosRemetente").val(JSON.stringify(r.Objeto));

                        PreencherCamposRemetente(r.Objeto, naoSalvarEndereco);

                        if (carregarLocalidade) {
                            $.ajaxSetup({ async: false });
                            $("#ddlUFInicioPrestacao").val(r.Objeto.UF);
                            BuscarLocalidades(r.Objeto.UF, "ddlMunicipioInicioPrestacao", r.Objeto.CodigoLocalidade);
                            ObterDadosSemelhantesPorEstado();
                            $.ajaxSetup({ async: true });
                        }                        

                        if (atualizarIndicadorTomador)
                            SetarIndicadorTomador();

                        AtualizarValoresGerais();
                    } else {
                        LimparCamposRemetente();
                        AtualizarValoresGerais();
                        $("#hddRemetente").val($("#txtCPFCNPJRemetente").val());

                        jAlert("<b>Remetente não encontrado.</b><br/><br/> Preencha corretamente os campos e o remetente será cadastrado ao salvar/emitir o CT-e.", "Atenção");
                    }
                } else {
                    jAlert(r.Erro, "Erro");
                    LimparCamposRemetente();
                    AtualizarValoresGerais();
                    $("#hddRemetente").val('');
                    $("#txtCPFCNPJRemetente").val('');
                }
                if (carregarLocalidade) {
                    BuscarApoliceDeSeguroDoTomador();
                }
            });
        } else {
            jAlert("O CPF/CNPJ digitado é inválido.", "Atenção");
            LimparCamposRemetente();
            AtualizarValoresGerais();
            $("#hddRemetente").val('');
            $("#txtCPFCNPJRemetente").val('');
        }
    } else {
        LimparCamposRemetente();
        AtualizarValoresGerais();
        $("#hddRemetente").val('');
        $("#txtCPFCNPJRemetente").val('');
    }

    if (carregarLocalidade) {
        BuscarApoliceDeSeguroDoTomador();
    }
}

function PreencherCamposRemetente(cliente, naoSalvarEndereco) {
    if (typeof naoSalvarEndereco == "undefined")
        naoSalvarEndereco = false;

    $("#hddRemetente").val(cliente.CPF_CNPJ);
    $("#txtAtividadeRemetente").val(cliente.CodigoAtividade + " - " + cliente.DescricaoAtividade);
    $("#hddAtividadeRemetente").val(cliente.CodigoAtividade);
    $("#txtCPFCNPJRemetente").val(cliente.CPF_CNPJ);
    $("#txtRGIERemetente").val(cliente.IE_RG);
    $("#txtRazaoSocialRemetente").val(cliente.Nome);
    $("#txtNomeFantasiaRemetente").val(cliente.NomeFantasiaTransportador != null && cliente.NomeFantasiaTransportador != "" ? cliente.NomeFantasiaTransportador : cliente.NomeFantasia);
    $("#txtTelefone1Remetente").val(cliente.Telefone1).change();
    $("#txtTelefone2Remetente").val(cliente.Telefone2).change();
    $("#txtEnderecoRemetente").val(cliente.Endereco);
    $("#txtNumeroRemetente").val(cliente.Numero);
    $("#txtBairroRemetente").val(cliente.Bairro);
    $("#txtComplementoRemetente").val(cliente.Complemento);
    $("#txtCEPRemetente").val(cliente.CEP);
    $("#txtEmailsRemetente").val(cliente.Email);
    $("#txtEmailsContatoRemetente").val(cliente.EmailContato);
    $("#txtEmailsContadorRemetente").val(cliente.EmailContador);
    $("#txtEmailsTransportadorRemetente").val(cliente.EmailTransportador);
    $("#chkSalvarEnderecoRemetente").prop("checked", naoSalvarEndereco ? false : cliente.SalvarEndereco);

    if (cliente.EmailStatus == "A" || cliente.EmailStatus == true) {
        $("#chkStatusEmailsRemetente").prop("checked", true);
    } else {
        $("#chkStatusEmailsRemetente").prop("checked", false);
    }

    if (cliente.EmailContatoStatus == "A" || cliente.EmailContatoStatus == true) {
        $("#chkStatusEmailsContatoRemetente").prop("checked", true);
    } else {
        $("#chkStatusEmailsContatoRemetente").prop("checked", false);
    }

    if (cliente.EmailContadorStatus == "A" || cliente.EmailContadorStatus == true) {
        $("#chkStatusEmailsContadorRemetente").prop("checked", true);
    } else {
        $("#chkStatusEmailsContadorRemetente").prop("checked", false);
    }

    if (cliente.EmailTransportadorStatus == "A" || cliente.EmailTransportadorStatus == true) {
        $("#chkStatusEmailsTransportadorRemetente").prop("checked", true);
    } else {
        $("#chkStatusEmailsTransportadorRemetente").prop("checked", false);
    }

    $("#ddlEstadoRemetente").val(cliente.UF);
    BuscarLocalidades(cliente.UF, 'selCidadeRemetente', cliente.CodigoLocalidade);
    SetarIndicadorTomador();
}

function LimparCamposRemetente() {
    $("#txtRGIERemetente").val('');
    $("#txtRazaoSocialRemetente").val('');
    $("#txtNomeFantasiaRemetente").val('');
    $("#txtTelefone1Remetente").val('').change();
    $("#txtTelefone2Remetente").val('').change();
    $("#txtEnderecoRemetente").val('');
    $("#txtNumeroRemetente").val('');
    $("#txtBairroRemetente").val('');
    $("#txtComplementoRemetente").val('');
    $("#txtCEPRemetente").val('');
    $("#txtEmailsRemetente").val('');
    $("#chkStatusEmailsRemetente").prop("checked", false);
    $("#txtEmailsContatoRemetente").val('');
    $("#chkStatusEmailsContatoRemetente").prop("checked", false);
    $("#txtEmailsContadorRemetente").val('');
    $("#chkStatusEmailsContadorRemetente").prop("checked", false);
    $("#ddlEstadoRemetente").val($("#ddlEstadoRemetente option:first").val());
    $("#selCidadeRemetente").html('');
    $("#txtAtividadeRemetente").val('');
    $("#hddAtividadeRemetente").val('0');
    $("#ddlPaisRemetente").val("01058");
    $("#selCidadeRemetente").html('');
    $("#chkRemetenteExportacao").prop("checked", false);
    $("#chkSalvarEnderecoRemetente").prop("checked", true);
    $("#txtCidadeRemetenteExportacao").val("");
    $("#txtEmailsTransportadorRemetente").val('');
    $("#chkStatusEmailsTransportadorRemetente").prop("checked", false);
    $("#hddDadosRemetente").val("");

    DesbloquearCamposRemetenteExportacao();

    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());

    if (configuracaoEmpresa != null && configuracaoEmpresa.CodigoAtividade > 0)
        RetornoConsultaAtividadeRemetente({ Codigo: configuracaoEmpresa.CodigoAtividade, Descricao: configuracaoEmpresa.DescricaoAtividade }, false);
}

function BloquearAbasRemetente() {
    switch ($("#selTipoDocumentoRemetente").val()) {
        case '1':
            $('#tabsRemetente a[href="#tabNotasFiscaisRemetente"]').hide();
            $('#tabsRemetente a[href="#tabOutrosRemetente"]').hide();
            $('#tabsRemetente a[href="#tabNFeRemetente"]').show();

            //$('#tabsRemetente a[href="#tabNFeRemetente"]').tab("show");
            break;
        case '2':
            $('#tabsRemetente a[href="#tabNFeRemetente"]').hide();
            $('#tabsRemetente a[href="#tabOutrosRemetente"]').hide();
            $('#tabsRemetente a[href="#tabNotasFiscaisRemetente"]').show();

            //$('#tabsRemetente a[href="#tabNotasFiscaisRemetente"]').tab("show");
            break;
        case '3':
            $('#tabsRemetente a[href="#tabNFeRemetente"]').hide();
            $('#tabsRemetente a[href="#tabNotasFiscaisRemetente"]').hide();
            $('#tabsRemetente a[href="#tabOutrosRemetente"]').show();

            //$('#tabsRemetente a[href="#tabOutrosRemetente"]').tab("show");
            break;
    }
    $("#tabsRemetente a[href='#tabDadosRemetente']").tab("show");
}

function BloquearCamposRemetenteExportacao() {
    $("#hddRemetente").val('');
    $("#txtCPFCNPJRemetente").val('');
    $("#chkRemetenteExportacao").prop("checked", true);
    $("#chkSalvarEnderecoRemetente").prop("disabled", true);
    $("#txtCPFCNPJRemetente").prop("disabled", true);
    $("#btnBuscarRemetente").prop("disabled", true);
    $("#txtRGIERemetente").prop("disabled", true);
    $("#txtNomeFantasiaRemetente").prop("disabled", true);
    $("#txtTelefone1Remetente").prop("disabled", true);
    $("#txtTelefone2Remetente").prop("disabled", true);
    $("#txtAtividadeRemetente").prop("disabled", true);
    $("#btnBuscarAtividadeRemetente").prop("disabled", true);
    $("#txtCEPRemetente").prop("disabled", true);
    $("#ddlEstadoRemetente").prop("disabled", true);
    $("#selCidadeRemetente").prop("disabled", true);
    $("#chkStatusEmailsRemetente").prop("disabled", true);
    $("#txtEmailsContatoRemetente").prop("disabled", true);
    $("#chkStatusEmailsContatoRemetente").prop("disabled", true);
    $("#txtEmailsContadorRemetente").prop("disabled", true);
    $("#chkStatusEmailsContadorRemetente").prop("disabled", true);
    $("#ddlPaisRemetente").prop("disabled", false);
    $("#txtEmailsTransportadorRemetente").prop("disabled", true);
    $("#chkStatusEmailsTransportadorRemetente").prop("disabled", true);
    $("#divFieldCidadeExportacaoRemetente").show();
    $("#divFieldCidadeRemetente").hide();
}

function DesbloquearCamposRemetenteExportacao() {
    $("#chkSalvarEnderecoRemetente").prop("disabled", false);
    $("#txtCPFCNPJRemetente").prop("disabled", false);
    $("#btnBuscarRemetente").prop("disabled", false);
    $("#txtRGIERemetente").prop("disabled", false);
    $("#txtNomeFantasiaRemetente").prop("disabled", false);
    $("#txtTelefone1Remetente").prop("disabled", false);
    $("#txtTelefone2Remetente").prop("disabled", false);
    $("#txtAtividadeRemetente").prop("disabled", false);
    $("#btnBuscarAtividadeRemetente").prop("disabled", false);
    $("#txtCEPRemetente").prop("disabled", false);
    $("#ddlEstadoRemetente").prop("disabled", false);
    $("#selCidadeRemetente").prop("disabled", false);
    $("#chkStatusEmailsRemetente").prop("disabled", false);
    $("#txtEmailsContatoRemetente").prop("disabled", false);
    $("#chkStatusEmailsContatoRemetente").prop("disabled", false);
    $("#txtEmailsContadorRemetente").prop("disabled", false);
    $("#chkStatusEmailsContadorRemetente").prop("disabled", false);
    $("#ddlPaisRemetente").prop("disabled", true);
    $("#txtEmailsTransportadorRemetente").prop("disabled", false);
    $("#chkStatusEmailsTransportadorRemetente").prop("disabled", false);
    $("#divFieldCidadeExportacaoRemetente").hide();
    $("#divFieldCidadeRemetente").show();
}

function BuscarRemetenteExportacaoPorNome() {
    if ($("#chkRemetenteExportacao")[0].checked && $("#txtRazaoSocialRemetente").val() != "") {
        executarRest("/ClienteExportacaoCTe/BuscarPorNome?callback=?", { Nome: $("#txtRazaoSocialRemetente").val() }, function (r) {
            if (r.Sucesso) {
                PreencherCamposRemetenteExportacao(r.Objeto);
            }
        });
    }
}

function PreencherCamposRemetenteExportacao(dados) {
    BloquearCamposRemetenteExportacao();

    $("#txtBairroRemetente").val(dados.Bairro);
    $("#txtCidadeRemetenteExportacao").val(dados.Cidade);
    $("#txtComplementoRemetente").val(dados.Complemento);
    $("#txtEmailsRemetente").val(dados.Email);
    $("#txtEnderecoRemetente").val(dados.Endereco);
    $("#chkRemetenteExportacao").prop("checked", true);
    $("#txtRazaoSocialRemetente").val(dados.Nome);
    $("#txtNumeroRemetente").val(dados.Numero);
    $("#ddlPaisRemetente").val(dados.SiglaPais);
    $("#ddlEstadoRemetente").val("EX");
    $("#txtRGIERemetente").val(dados.IE_RG);
}

function BuscarDadosRemetente() {
    if ($("#txtCPFCNPJRemetente").val() != "") {
        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: $("#txtCPFCNPJRemetente").val() }, function (r) {
            if (r.Sucesso) {
                $("#hddDadosRemetente").val(JSON.stringify(r.Objeto));
            } else {
                jAlert(r.Erro, "Atenção");
            }
        });
    }
}