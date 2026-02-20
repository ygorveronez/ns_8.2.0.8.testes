<%@ Page Title="Cadastro de Clientes" Language="C#" MasterPageFile="Site.Master"
    AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="EmissaoCTe.WebApp.Clientes" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%: Styles.Render("~/bundle/styles/datepicker") %>
    <%: Scripts.Render("~/bundle/scripts/blockui",
                       "~/bundle/scripts/maskedinput",
                       "~/bundle/scripts/datatables",
                       "~/bundle/scripts/ajax",
                       "~/bundle/scripts/gridview",
                       "~/bundle/scripts/consulta",
                       "~/bundle/scripts/baseConsultas",
                       "~/bundle/scripts/mensagens",
                       "~/bundle/scripts/validaCampos",
                       "~/bundle/scripts/priceformat",
                       "~/bundle/scripts/datepicker") %>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            HeaderAuditoria("Cliente");

            $("#txtPercentualRetencaoICMSST").priceFormat();

            $("#<%=txtCEP.ClientID%>").mask("99.999-999");

            $("#txtDataNascimento").datepicker();
            $("#txtDataNascimento").mask("99/99/9999");

            AlterarCamposPorTipoDePessoa();

            CarregarConsultadeClientes("default-search", "default-search", RetornoConsultaClientes, true, false);
            CarregarConsultaDeAtividades("txtAtividade", "btnBuscarAtividade", RetornoConsultaAtividades, true);
            CarregarConsultaDeBancos("btnBuscarBanco", "btnBuscarBanco", RetornoConsultaBanco, true, false);

            $("#<%=ddlTipo.ClientID%>").change(function () {
                AlterarCamposPorTipoDePessoa();
                if ($("#<%=ddlTipo.ClientID%>").val() == "F") {
                    $("#<%=txtRGIE.ClientID%>").removeAttr("disabled");
                } else {
                    $("#<%=chkIsento.ClientID%>").removeAttr("checked");
                }
            });

            $("#<%=txtCPFCNPJ.ClientID%>").focusout(function () {
                BuscarDadosDoCliente();
                HeaderAuditoriaCodigo($("#<%=txtCPFCNPJ.ClientID%>").val().replace(/[^0-9]/g, ''));
            });

            $("#btnCancelarJS").click(function () {
                LimparCampos();
            });

            $("#btnSalvarJS").click(function () {
                Salvar();
            });

            $("#<%=ddlUF.ClientID%>").change(function () {
                BuscarLocalidades();
            });

            $("#selCidade").change(function () {
                var x = 0;
                $("#<%=hddIdLocalidade.ClientID%>").val($(this).val());
            });

            $("#<%=txtEmails.ClientID%>").focusout(function () {
                var texto = $(this).val();
                $(this).val(texto.toLowerCase());
            });

            $("#<%=txtEmailsContato.ClientID%>").focusout(function () {
                var texto = $(this).val();
                $(this).val(texto.toLowerCase());
            });

            $("#<%=txtEmailsContador.ClientID%>").focusout(function () {
                var texto = $(this).val();
                $(this).val(texto.toLowerCase());
            });

            $("#<%=txtEmailsTransportador.ClientID%>").focusout(function () {
                var texto = $(this).val();
                $(this).val(texto.toLowerCase());
            });

            $("#<%=chkIsento.ClientID%>").click(function () {
                if ($(this)[0].checked) {
                    $("#<%=txtRGIE.ClientID%>").attr("disabled", "disabled");
                    $("#<%=txtRGIE.ClientID%>").val("ISENTO");
                } else {
                    $("#<%=txtRGIE.ClientID%>").removeAttr("disabled");
                    $("#<%=txtRGIE.ClientID%>").val("");
                }
            });

            $("#txtBanco").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddIdBanco").val("0");
                    }
                    e.preventDefault();
                }
            });

            $("#btnConsultarReceita").click(function () {
                if ($("#<%=ddlTipo.ClientID%>").val() == "F")
                    jAlert("Consulta disponível apenas para clientes pessoas Jurídicas (CNPJ)!", "Atenção");
                else {
                    if ($("#<%=txtRazaoSocial.ClientID%>").val() != "") {
                        jConfirm("Deseja atualizar os dados do cliente? ", "Atenção", function (ret) {
                            if (ret)
                                ConsultarCNPJReceitaWS(); //AbrirConsultaReceita(); //ConsultarDadosCentralizado();
                        });
                    }
                    else
                        ConsultarCNPJReceitaWS(); //AbrirConsultaReceita(); //ConsultarDadosCentralizado();
                }
            });

            $("#btnCaptchaReceita").click(function () {
                ConsultarDadosReceita();
            });

            $("#btnAtualizarCaptchaReceita").click(function () {
                AbrirConsultaReceita();
            });

            BuscarDadosPadroes();
        });
        function RetornoConsultaClientes(cliente) {
            HeaderAuditoriaCodigo(cliente.CPFCNPJ.replace(/[^0-9]/g, ''));
            $("#ddlTipo").val(cliente.Tipo);
            AlterarCamposPorTipoDePessoa();
            $("#txtCPFCNPJ").val(cliente.CPFCNPJ);
            BuscarDadosDoCliente();
        }
        function BuscarDadosPadroes() {
            executarRest("/ConfiguracaoEmpresa/Buscar?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    $("#hddIdAtividade").val(r.Objeto.CodigoAtividade);
                    $("#txtAtividade").val(r.Objeto.CodigoAtividade > 0 ? r.Objeto.CodigoAtividade + " - " + r.Objeto.DescricaoAtividade : "");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function BuscarLocalidades() {
            var uf = $("#<%=ddlUF.ClientID%>").val();
            executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
                if (r.Sucesso) {
                    RenderizarLocalidades(r.Objeto);
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarLocalidades(localidades) {
            var selLocalidades = document.getElementById("selCidade");
            selLocalidades.options.length = 0;
            var optn = document.createElement("option");
            optn.text = "Selecione";
            optn.value = 0;
            selLocalidades.options.add(optn);
            for (var i = 0; i < localidades.length; i++) {
                var optn = document.createElement("option");
                optn.text = localidades[i].Descricao;
                optn.value = localidades[i].Codigo;
                selLocalidades.options.add(optn);
            }
        }
        function RetornoConsultaBanco(banco) {
            $("#hddIdBanco").val(banco.Codigo);
            $("#txtBanco").val(banco.Descricao);
        }
        function RetornoConsultaAtividades(atividade) {
            $("#<%=hddIdAtividade.ClientID%>").val(atividade.Codigo);
            $("#<%=txtAtividade.ClientID%>").val(atividade.Codigo + " - " + atividade.Descricao);

            //if (atividade.Codigo == 7)
            //    $("#<%=txtRGIE.ClientID%>").val("ISENTO");
        }
        function AlterarCamposPorTipoDePessoa() {
            if ($("#<%=ddlTipo.ClientID%>").val() == "F") {
                $("#divCPFCNPJ span").text("CPF*:");
                $("#<%=txtCPFCNPJ.ClientID%>").unmask();
                $("#<%=txtCPFCNPJ.ClientID%>").mask("999.999.999-99");
                $("#divNome span").text("Nome*:");
                $("#divRGIE span").text("IE:");
                $("#divIsento").css("display", "none");
                $("#divInscricaoST").css("display", "none");
                $(".campoPessoaFisica").each(function () {
                    $(this).show();
                });
            } else {
                $("#divCPFCNPJ span").text("CNPJ*:");
                $("#<%=txtCPFCNPJ.ClientID%>").unmask();
                $("#<%=txtCPFCNPJ.ClientID%>").mask("99.999.999/9999-99");
                $("#divNome span").text("Razão Social*:");
                $("#divRGIE span").text("IE:");
                $("#divIsento").css("display", "");
                $("#divInscricaoST").css("display", "");
                $(".campoPessoaFisica").each(function () {
                    $(this).hide();
                });
            }
        }
        function BuscarDadosDoCliente() {
            var cpfCnpj = $("#<%=txtCPFCNPJ.ClientID%>").val();
            cpfCnpj = cpfCnpj.replace(/[^0-9]/g, '');
            if (cpfCnpj != "") {
                if ($("#<%=ddlTipo.ClientID%>").val() == "J") {
                    if (!ValidarCNPJ(cpfCnpj)) {
                        jAlert("O CNPJ digitado é inválido!", "Atenção");
                        CampoComErro("#<%=txtCPFCNPJ.ClientID%>");
                        return;
                    } else {
                        CampoSemErro("#<%=txtCPFCNPJ.ClientID%>");
                    }
                } else {
                    if (!ValidarCPF(cpfCnpj)) {
                        jAlert("O CPF digitado é inválido!", "Atenção");
                        CampoComErro("#<%=txtCPFCNPJ.ClientID%>");
                        return;
                    } else {
                        CampoSemErro("#<%=txtCPFCNPJ.ClientID%>");
                    }
                }
                executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto != null) {
                            $("#<%=ddlTipo.ClientID%>").val(r.Objeto.Tipo);
                            AlterarCamposPorTipoDePessoa();
                            $("#<%=txtCPFCNPJ.ClientID%>").val(r.Objeto.CPF_CNPJ);
                            $("#<%=txtRGIE.ClientID%>").val(r.Objeto.IE_RG);

                            if (r.Objeto.IE_RG == "ISENTO" && r.Objeto.Tipo == "J") {
                                $("#<%=chkIsento.ClientID%>").attr("checked", "checked");
                                $("#<%=txtRGIE.ClientID%>").attr("disabled", "disabled");
                            } else {
                                $("#<%=txtRGIE.ClientID%>").removeAttr("disabled");
                                $("#<%=chkIsento.ClientID%>").removeAttr("checked");
                            }

                            $("#<%=txtDataCadastro.ClientID%>").val(r.Objeto.DataCadastro);
                            $("#<%=txtRazaoSocial.ClientID%>").val(r.Objeto.Nome);
                            $("#<%=txtNomeFantasia.ClientID%>").val(r.Objeto.NomeFantasia);
                            $("#<%=txtTelefone1.ClientID%>").val(r.Objeto.Telefone1).change();
                            $("#<%=txtTelefone2.ClientID%>").val(r.Objeto.Telefone2).change();
                            $("#<%=txtEndereco.ClientID%>").val(r.Objeto.Endereco);
                            $("#<%=txtNumero.ClientID%>").val(r.Objeto.Numero);
                            $("#<%=txtBairro.ClientID%>").val(r.Objeto.Bairro);
                            $("#<%=txtComplemento.ClientID%>").val(r.Objeto.Complemento);
                            $("#<%=txtCEP.ClientID%>").val(r.Objeto.CEP);

                            $("#<%=txtEmails.ClientID%>").val(r.Objeto.Email);

                            if (r.Objeto.EmailStatus == "A") {
                                $("#<%=chkEmailsStatus.ClientID%>").attr("checked", "checked");
                            } else {
                                $("#<%=chkEmailsStatus.ClientID%>").removeAttr("checked");
                            }

                            $("#<%=txtEmailsContato.ClientID%>").val(r.Objeto.EmailContato);

                            if (r.Objeto.EmailContatoStatus == "A") {
                                $("#<%=chkEmailsContatoStatus.ClientID%>").attr("checked", "checked");
                            } else {
                                $("#<%=chkEmailsContatoStatus.ClientID%>").removeAttr("checked");
                            }

                            $("#<%=txtEmailsContador.ClientID%>").val(r.Objeto.EmailContador);

                            if (r.Objeto.EmailContadorStatus == "A") {
                                $("#<%=chkEmailsContadorStatus.ClientID%>").attr("checked", "checked");
                            } else {
                                $("#<%=chkEmailsContadorStatus.ClientID%>").removeAttr("checked");
                            }

                            $("#<%=ddlUF.ClientID%>").val(r.Objeto.UF);

                            RenderizarLocalidades(r.Objeto.Cidades);

                            $("#selCidade").val(r.Objeto.CodigoLocalidade);
                            $("#<%=hddIdLocalidade.ClientID%>").val(r.Objeto.CodigoLocalidade);
                            $("#<%=txtAtividade.ClientID%>").val(r.Objeto.CodigoAtividade + " - " + r.Objeto.DescricaoAtividade);
                            $("#<%=hddIdAtividade.ClientID%>").val(r.Objeto.CodigoAtividade);

                            $("#txtAgencia").val(r.Objeto.Agencia);
                            $("#txtDigitoAgencia").val(r.Objeto.DigitoAgencia);
                            $("#txtNumeroConta").val(r.Objeto.NumeroConta);
                            $("#ddlTipoConta").val(r.Objeto.TipoConta);
                            $("#hddIdBanco").val(r.Objeto.CodigoBanco);
                            $("#txtBanco").val(r.Objeto.DescricaoBanco);
                            $("#txtNumeroCartao").val(r.Objeto.NumeroCartao);
                            $("#txtPercentualRetencaoICMSST").val(r.Objeto.PercentualRetencaoICMSST);
                            $("#txtPIS").val(r.Objeto.PIS);
                            $("#txtNomeFantasiaTransportador").val(r.Objeto.NomeFantasiaTransportador);                          

                            $("#<%=txtEmailsTransportador.ClientID%>").val(r.Objeto.EmailTransportador);

                            if (r.Objeto.EmailTransportadorStatus == "A") {
                                $("#<%=chkEmailsTransportadorStatus.ClientID%>").attr("checked", "checked");
                            } else {
                                $("#<%=chkEmailsTransportadorStatus.ClientID%>").removeAttr("checked");
                            }

                            if (r.Objeto.ArmazenaNotasParaGerarPorPeriodo == true)
                                $("#<%=chkArmazenaNotasParaGerarPorPeriodo.ClientID%>").attr("checked", "checked");
                            else
                                $("#<%=chkArmazenaNotasParaGerarPorPeriodo.ClientID%>").removeAttr("checked");

                            if (r.Objeto.NaoAverbarQuandoTerceiro == true)
                                $("#<%=chkNaoAverbarQuandoTerceiro.ClientID%>").attr("checked", "checked");
                            else
                                $("#<%=chkNaoAverbarQuandoTerceiro.ClientID%>").removeAttr("checked");
                            
                            $("#ddlOrgaoEmissorRG").val(r.Objeto.OrgaoEmissorRG);
                            $("#ddlUFRG").val(r.Objeto.EstadoRG);
                            $("#txtDataNascimento").val(r.Objeto.DataNascimento);
                            $("#ddlSexo").val(r.Objeto.Sexo);
                            $("#txtInscricaoST").val(r.Objeto.InscricaoST);
                            $("#txtSuframa").val(r.Objeto.InscricaoSuframa);

                        }
                    } else {
                        jAlert(r.Erro, "Erro");
                    }
                });
            }
        }
        function LimparCampos() {
            document.location = "Clientes.aspx";
        }
        function Salvar() {
            var valido = true;
            var idAtividade = parseInt($("#<%=hddIdAtividade.ClientID%>").val());
            var idLocalidade = parseInt($("<%=hddIdLocalidade.ClientID%>").val());
            if (idAtividade <= 0) {
                valido = false;
                CampoComErro("#<%=txtAtividade.ClientID%>");
            } else {
                CampoSemErro("#<%=txtAtividade.ClientID%>");
            }
            if (idLocalidade <= 0) {
                valido = false;
                CampoComErro("#selCidade");
            } else {
                CampoSemErro("#selCidade");
            }
            if (!valido) {
                $("#divMensagemAlerta .mensagem").text("Os campos com asterísco (*) ou em vermelho são obrigatórios.");
                $("#divMensagemAlerta").show();
            } else {
                $("#<%=btnSalvarASP.ClientID%>").trigger("click");
            }
        }

        function AbrirConsultaReceita() {
            $("#txtCaptchaReceita").val("");
            executarRest("/Cliente/ConsultarClienteReceita?callback=?", { CNPJ: $("#txtCPFCNPJ").val() }, function (r) {
                if (r.Sucesso) {

                    $('#txtCaptchaReceita').val();
                    $('#imgCaptcha').prop('src', r.Objeto.chaptcha);
                    cookies = r.Objeto.Cookies;

                    $("#divCaptchaReceita").modal({ keyboard: false, backdrop: 'static' });

                } else {
                    jAlert(r.Erro, "Consulta de Cliente Receita");
                }
            });
        }

        function ConsultarDadosReceita() {
            executarRest("/Cliente/InformarCaptchaReceita?callback=?", { CNPJ: $("#txtCPFCNPJ").val(), Captcha: $("#txtCaptchaReceita").val(), Cookies: JSON.stringify(cookies) }, function (r) {
                if (r.Sucesso) {

                    $("#<%=txtRazaoSocial.ClientID%>").val(r.Objeto.Nome);
                    if (r.Objeto.Fantasia != "")
                        $("#<%=txtNomeFantasia.ClientID%>").val(r.Objeto.Fantasia);
                    $("#<%=txtCEP.ClientID%>").val(r.Objeto.CEP);
                    $("#<%=txtEndereco.ClientID%>").val(r.Objeto.Endereco);
                    $("#<%=txtComplemento.ClientID%>").val(r.Objeto.Complemento);
                    $("#<%=txtNumero.ClientID%>").val(r.Objeto.Numero);
                    $("#<%=txtBairro.ClientID%>").val(r.Objeto.Bairro);
                    if (r.Objeto.TelefonePrincipal != "")
                        $("#<%=txtTelefone1.ClientID%>").val(r.Objeto.TelefonePrincipal).change();

                    if (r.Objeto.Localidade != null) {
                        $("#<%=ddlUF.ClientID%>").val(r.Objeto.Localidade.UF);

                        RenderizarLocalidades(r.Objeto.Cidades);

                        $("#selCidade").val(r.Objeto.Localidade.Codigo);
                        $("#<%=hddIdLocalidade.ClientID%>").val(r.Objeto.Localidade.Codigo);
                    }

                    $('#txtCaptchaReceita').val();
                    $("#divCaptchaReceita").modal('hide');

                } else {
                    jAlert(r.Erro, "Consulta de Cliente Receita");
                }
            });
        }

        function ConsultarCNPJReceitaWS() {
            executarRest("/Cliente/ConsultarCNPJReceitaWS?callback=?", { CNPJ: $("#txtCPFCNPJ").val() }, function (r) {
                if (r.Sucesso) {

                    if (r.Objeto.Nome != "")
                        $("#txtRazaoSocial").val(r.Objeto.Nome);
                    if (r.Objeto.Fantasia != "")
                        $("#txtNomeFantasia").val(r.Objeto.Fantasia);
                    if (r.Objeto.CEP != "")
                        $("#body_txtCEP").val(r.Objeto.CEP);
                    if (r.Objeto.Endereco != "")
                        $("#txtEndereco").val(r.Objeto.Endereco);
                    $("#body_txtComplemento").val(r.Objeto.Complemento);
                    if (r.Objeto.Numero != "")
                        $("#txtNumero").val(r.Objeto.Numero);
                    if (r.Objeto.Bairro != "")
                        $("#txtBairro").val(r.Objeto.Bairro);
                    if (r.Objeto.TelefonePrincipal != "")
                        $("#body_txtTelefone1").val(r.Objeto.TelefonePrincipal).change();
                    if (r.Objeto.InscricaoEstadual != "")
                        $("#txtRGIE").val(r.Objeto.InscricaoEstadual);
                    if (r.Objeto.Localidade != null) {
                        $("#ddlUF").val(r.Objeto.Localidade.UF);

                        RenderizarLocalidades(r.Objeto.Cidades);

                        $("#selCidade").val(r.Objeto.Localidade.Codigo);
                        $("#body_hddIdLocalidade").val(r.Objeto.Localidade.Codigo);
                    }

                } else {
                    jAlert(r.Erro, "Consulta de Cliente Sintegra");
                }
            });
        }

        function ConsultarDadosCentralizado() {
            executarRest("/Cliente/ConsultarClienteSintegraCentralizado?callback=?", { CNPJ: $("#txtCPFCNPJ").val() }, function (r) {
                if (r.Sucesso) {

                    if (r.Objeto.Nome != "")
                        $("#<%=txtRazaoSocial.ClientID%>").val(r.Objeto.Nome);
                    if (r.Objeto.Fantasia != "")
                        $("#<%=txtNomeFantasia.ClientID%>").val(r.Objeto.Fantasia);
                    if (r.Objeto.CEP != "")
                        $("#<%=txtCEP.ClientID%>").val(r.Objeto.CEP);
                    if (r.Objeto.Endereco != "")
                        $("#<%=txtEndereco.ClientID%>").val(r.Objeto.Endereco);
                    $("#<%=txtComplemento.ClientID%>").val(r.Objeto.Complemento);
                    if (r.Objeto.Numero != "")
                        $("#<%=txtNumero.ClientID%>").val(r.Objeto.Numero);
                    if (r.Objeto.Bairro != "")
                        $("#<%=txtBairro.ClientID%>").val(r.Objeto.Bairro);
                    if (r.Objeto.TelefonePrincipal != "")
                        $("#<%=txtTelefone1.ClientID%>").val(r.Objeto.TelefonePrincipal).change();
                    if (r.Objeto.InscricaoEstadual != "")
                        $("#<%=txtRGIE.ClientID%>").val(r.Objeto.InscricaoEstadual);
                    if (r.Objeto.Localidade != null) {
                        $("#<%=ddlUF.ClientID%>").val(r.Objeto.Localidade.UF);

                        RenderizarLocalidades(r.Objeto.Cidades);

                        $("#selCidade").val(r.Objeto.Localidade.Codigo);
                        $("#<%=hddIdLocalidade.ClientID%>").val(r.Objeto.Localidade.Codigo);
                    }

                    $.fancybox.close();

                } else {
                    jAlert(r.Erro, "Consulta de Cliente Sintegra");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <asp:HiddenField ID="hddIdAtividade" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hddIdLocalidade" runat="server" Value="0" />
        <asp:HiddenField ID="hddIdBanco" runat="server" Value="0" ClientIDMode="Static" />
        <asp:Button ID="btnSalvarASP" runat="server" OnClick="btnSalvarASP_Click" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Clientes
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
        <div class="alert alert-danger" id="divMensagemErro" style="display: none;">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
            <strong>Erro!</strong> Ocorreu uma erro no sistema ao salvar, atualize a página e tente novamente.
        </div>
        <div class="alert alert-warning" id="divMensagemAlerta" style="display: none;">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
            <strong>Atenção!</strong> Os campos com asterísco (*) ou em vermelho são obrigatórios.
        </div>
        <div class="alert alert-success" id="divMensagemSucesso" style="display: none;">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
            <strong>Sucesso!</strong> Dados salvos com sucesso.
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Cadastro:
                </span>
                <asp:TextBox ID="txtDataCadastro" runat="server" Enabled="false" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo*:
                </span>
                <asp:DropDownList ID="ddlTipo" runat="server" ClientIDMode="Static" CssClass="form-control">
                    <asp:ListItem Value="J" Text="Jurídica"></asp:ListItem>
                    <asp:ListItem Value="F" Text="Física"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3" id="divCPFCNPJ">
            <div class="input-group">
                <span class="input-group-addon">CNPJ*:
                </span>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtCPFCNPJ"
                    runat="server" CssClass="hidden"></asp:RequiredFieldValidator>
                <asp:TextBox ID="txtCPFCNPJ" runat="server" MaxLength="20" CssClass="form-control maskedInput"
                    ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-6 col-sm-3 col-md-3 col-lg-2">
            <div class="input-group">
                <span class="input-group-btn">
                    <button type="button" id="btnConsultarReceita" class="btn btn-primary">Consultar CNPJ</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3" id="divRGIE">
            <div class="input-group">
                <span class="input-group-addon">IE:
                </span>
                <asp:TextBox ID="txtRGIE" runat="server" MaxLength="14" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4 campoPessoaFisica">
            <div class="input-group">
                <span class="input-group-addon">Emissor RG:
                </span>
                <asp:DropDownList ID="ddlOrgaoEmissorRG" runat="server" CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Value="" Text="Nenhum"></asp:ListItem>
                    <asp:ListItem Value="1" Text="SSP"></asp:ListItem>
                    <asp:ListItem Value="2" Text="CNH"></asp:ListItem>
                    <asp:ListItem Value="3" Text="MMA"></asp:ListItem>
                    <asp:ListItem Value="4" Text="DIC"></asp:ListItem>
                    <asp:ListItem Value="5" Text="POF"></asp:ListItem>
                    <asp:ListItem Value="6" Text="IFP"></asp:ListItem>
                    <asp:ListItem Value="7" Text="POM"></asp:ListItem>
                    <asp:ListItem Value="8" Text="IPF"></asp:ListItem>
                    <asp:ListItem Value="9" Text="SES"></asp:ListItem>
                    <asp:ListItem Value="10" Text="MAE"></asp:ListItem>
                    <asp:ListItem Value="11" Text="MEX"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6 campoPessoaFisica">
            <div class="input-group">
                <span class="input-group-addon">UF RG:
                </span>
                <asp:DropDownList ID="ddlUFRG" runat="server" CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Value="" Text="Nenhum"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3 campoPessoaFisica">
            <div class="input-group">
                <span class="input-group-addon">Data Nasc.:
                </span>
                <asp:TextBox ID="txtDataNascimento" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3 campoPessoaFisica">
            <div class="input-group">
                <span class="input-group-addon">Sexo:
                </span>
                <asp:DropDownList ID="ddlSexo" runat="server" CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Value="" Text="Nenhum"></asp:ListItem>
                    <asp:ListItem Value="1" Text="Masculino"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Feminino"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-xs-2 col-sm-2 col-md-2 col-lg-1" id="divIsento">
            <div class="input-group">
                <div class="checkbox">
                    <label>
                        <asp:CheckBox ID="chkIsento" runat="server" ClientIDMode="Static" />
                        Isento
                    </label>
                </div>
            </div>
        </div>
        <div class="col-xs-6 col-sm-3 col-md-3 col-lg-3" id="divInscricaoST">
            <div class="input-group">
                <span class="input-group-addon">Inscrição ST:
                </span>
                <asp:TextBox ID="txtInscricaoST" runat="server" MaxLength="14" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="divNome">
            <div class="input-group">
                <span class="input-group-addon">Razão Social*:
                </span>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtRazaoSocial"
                    runat="server" CssClass="hidden"></asp:RequiredFieldValidator>
                <asp:TextBox ID="txtRazaoSocial" runat="server" MaxLength="80" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Nome Fantasia:
                </span>
                <asp:TextBox ID="txtNomeFantasia" runat="server" MaxLength="80" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Suframa:
                </span>
                <asp:TextBox ID="txtSuframa" runat="server" MaxLength="9" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Atividade*:
                </span>
                <asp:TextBox ID="txtAtividade" runat="server" ReadOnly="true" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarAtividade" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Telefone 1*:
                </span>
                <asp:TextBox ID="txtTelefone1" runat="server" MaxLength="20" CssClass="maskedInput form-control phone"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Telefone 2:
                </span>
                <asp:TextBox ID="txtTelefone2" runat="server" MaxLength="20" CssClass="maskedInput form-control phone"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Endereço*:
                </span>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtEndereco"
                    runat="server" CssClass="hidden"></asp:RequiredFieldValidator>
                <asp:TextBox ID="txtEndereco" runat="server" MaxLength="80" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>

            </div>
        </div>
        <div class="col-xs-6 col-sm-3 col-md-3 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Número*:
                </span>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtNumero"
                    runat="server" CssClass="hidden"></asp:RequiredFieldValidator>
                <asp:TextBox ID="txtNumero" runat="server" MaxLength="60" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>

            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Bairro*:
                </span>
                <asp:TextBox ID="txtBairro" runat="server" MaxLength="40" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ControlToValidate="txtBairro"
                    runat="server"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="col-xs-6 col-sm-5 col-md-5 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Complemento:
                </span>
                <asp:TextBox ID="txtComplemento" runat="server" MaxLength="60" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-6 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">CEP*:
                </span>
                <asp:TextBox ID="txtCEP" runat="server" MaxLength="20" CssClass="maskedInput form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" ControlToValidate="txtCEP"
                    runat="server"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Estado*:
                </span>
                <asp:DropDownList ID="ddlUF" runat="server" CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Value="" Text="Selecione"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cidade*:
                </span>
                <select id="selCidade" class="form-control">
                </select>
            </div>
        </div>
    </div>
    <ul class="nav nav-tabs" role="tablist" style="margin-bottom: 10px;">
        <li role="presentation" class="active"><a href="#emails" aria-controls="emails" role="tab" data-toggle="tab">E-mails</a></li>
        <li role="presentation"><a href="#bancarios" aria-controls="bancarios" role="tab" data-toggle="tab">Dados Bancários</a></li>
        <li role="presentation"><a href="#tributos" aria-controls="tributos" role="tab" data-toggle="tab">Outros</a></li>
    </ul>
    <div class="tab-content">
        <div role="tabpanel" class="tab-pane active" id="emails">
            <div class="row">
                <div class="col-xs-8 col-sm-8 col-md-8 col-lg-8">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="E-mails da Empresa">@ Empresa</abbr>:
                        </span>
                        <asp:TextBox ID="txtEmails" runat="server" MaxLength="1000" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="chkEmailsStatus" runat="server" ClientIDMode="Static" />
                            Enviar XML Automático
                        </label>
                    </div>
                </div>
                <div class="col-xs-8 col-sm-8 col-md-8 col-lg-8">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="E-mails dos Contatos">@ Contato</abbr>:
                        </span>
                        <asp:TextBox ID="txtEmailsContato" runat="server" MaxLength="1000" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="chkEmailsContatoStatus" runat="server" ClientIDMode="Static" />
                            Enviar XML Automático
                        </label>
                    </div>
                </div>
                <div class="col-xs-8 col-sm-8 col-md-8 col-lg-8">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="E-mails dos Contadores">@ Contador</abbr>:
                        </span>
                        <asp:TextBox ID="txtEmailsContador" runat="server" MaxLength="1000" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="chkEmailsContadorStatus" runat="server" ClientIDMode="Static" />
                            Enviar XML Automático
                        </label>
                    </div>
                </div>
                <div class="col-xs-8 col-sm-8 col-md-8 col-lg-8">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="E-mails Específico por Transportador">@ Por Transportador</abbr>:
                        </span>
                        <asp:TextBox ID="txtEmailsTransportador" runat="server" MaxLength="1000" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="chkEmailsTransportadorStatus" runat="server" ClientIDMode="Static" />
                            Enviar XML Automático
                        </label>
                    </div>
                </div>
            </div>
        </div>
        <div role="tabpanel" class="tab-pane" id="bancarios">
            <div class="row">
                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Agência:
                        </span>
                        <asp:TextBox ID="txtAgencia" runat="server" MaxLength="10" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Dígito:
                        </span>
                        <asp:TextBox ID="txtDigitoAgencia" runat="server" MaxLength="1" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Nº Conta:
                        </span>
                        <asp:TextBox ID="txtNumeroConta" runat="server" MaxLength="10" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Tipo:
                        </span>
                        <asp:DropDownList ID="ddlTipoConta" runat="server" CssClass="form-control" ClientIDMode="Static">
                            <asp:ListItem Value="" Text="Nenhum"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Corrente"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Poupança"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Banco:
                        </span>
                        <asp:TextBox ID="txtBanco" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarBanco" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Nº Cartão:
                        </span>
                        <asp:TextBox ID="txtNumeroCartao" runat="server" MaxLength="16" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>
        <div role="tabpanel" class="tab-pane" id="tributos">
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Nome Fantasia Por Transportador:
                        </span>
                        <asp:TextBox ID="txtNomeFantasiaTransportador" runat="server" MaxLength="80" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
            </div>            
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="chkArmazenaNotasParaGerarPorPeriodo" runat="server" ClientIDMode="Static" />
                            Armazenar notas municipais para geração de NFSe por período
                        </label>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="chkNaoAverbarQuandoTerceiro" runat="server" ClientIDMode="Static" />
                            Não averbar quando terceiro (proprietário veículo)
                        </label>
                    </div>
                </div>
            </div>            
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Percentual Retenção ICMS ST:
                        </span>
                        <asp:TextBox ID="txtPercentualRetencaoICMSST" runat="server" MaxLength="18" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">PIS/PASEP:
                        </span>
                        <asp:TextBox ID="txtPIS" runat="server" MaxLength="18" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvarJS" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelarJS" class="btn btn-default">Cancelar</button>

    <div class="modal fade" id="divCaptchaReceita" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Consulta CNPJ Receita</h4>
                </div>
                <div class="modal-body">
                    <div id="divCaptchaReceitaBody" class="row" style="padding: 15px;">
                        <div class="smart-form">
                            <section>
                                <div class="well">
                                    <div class="row">
                                        <div style="float: left; margin-top: 6px; margin-left: 100px;">
                                            <img src="" id="imgCaptcha" style="float: left; margin-top: 6px; border: 1px solid #CCC; width: 260px; height: 80px;" /><a href="javascript:;void(0)" style="float: left; margin-top: 25px; margin-left: 5px;"><i class="fa fa-refresh"></i></a>
                                        </div>
                                        <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group" style="margin-top: 40px;">
                                                <span class="input-group-addon" style="margin-top: 6px">Captcha:
                                                </span>
                                                <input type="text" id="txtCaptchaReceita" class="form-control" autofocus />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnCaptchaReceita" class="btn btn-primary"><span class="glyphicon glyphicon-search"></span>&nbsp;Consultar</button>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <span class="input-group-btn">
                                            <button type="button" id="btnAtualizarCaptchaReceita" class="btn btn-link" style="margin-left: 135px"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Gerar novo Captcha</button>
                                        </span>
                                    </div>
                                </div>
                            </section>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
