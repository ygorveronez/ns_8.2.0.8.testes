<%@ Page Title="Cadastro de Empresas Gerenciadoras" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EmpresasGerenciadoras.aspx.cs" Inherits="EmissaoCTe.InternalWebAdmin.EmpresasGerenciadoras" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/plupload/jquery.plupload.queue.min.css" rel="stylesheet" type="text/css" />
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/plupload.full.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/jquery.plupload.queue.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/pt-br.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var path = "";
        var countArquivos = 0;
        $(document).ready(function () {
            ObterFusosHorarios();
            if (document.location.pathname.split("/").length > 1) {
                var paths = document.location.pathname.split("/");
                for (var i = 0; (paths.length - 1) > i; i++) {
                    if (paths[i] != "") {
                        path += "/" + paths[i];
                    }
                }
            }
            $("#txtRNTRC").mask("99999999");
            $("#txtCNPJ").mask("99.999.999/9999-99");
            $("#txtCEP").mask("99.999-999");
            $("#txtTelefone1").mask("(99) 9999-9999?9");
            $("#txtTelefone2").mask("(99) 9999-9999?9");
            $("#txtTelefoneContador").mask("(99) 9999-9999?9");
            $("#txtTelefoneContato").mask("(99) 9999-9999?9");
            $("#txtDataInicialCertificado").mask("99/99/9999");
            $("#txtDataInicialCertificado").datepicker({ changeMonth: true, changeYear: true });
            $("#txtDataFinalCertificado").mask("99/99/9999");
            $("#txtDataFinalCertificado").datepicker({ changeMonth: true, changeYear: true });
            CarregarConsultaDeEmpresasGerenciadoras("default-search", "default-search", "", BuscarDetalhes, true, false);
            BuscarUFs("selUF");
            $("#selUF").change(function () {
                BuscarLocalidades($(this).val(), "selLocalidade", null);
            });
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnExcluir").click(function () {
                Excluir();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            $("#btnUsuarios").click(function () {
                location.href = 'UsuariosEmpresa.aspx?x=' + $("#hddCodigoCriptografado").val();
            });
        });
        function BuscarUFs(idSelect) {
            executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarUFs(r.Objeto, idSelect);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarUFs(ufs, idSelect) {
            var selUFs = document.getElementById(idSelect);
            selUFs.options.length = 0;
            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '0';
            selUFs.options.add(optn);
            for (var i = 0; i < ufs.length; i++) {
                var optn = document.createElement("option");
                optn.text = ufs[i].Sigla + " - " + ufs[i].Nome;
                optn.value = ufs[i].Sigla;
                selUFs.options.add(optn);
            }
        }
        function ValidarCampos() {
            var razao = $("#txtRazaoSocial").val().trim();
            var nome = $("#txtNomeFantasia").val().trim();
            var cnpj = $("#txtCNPJ").val().trim();
            var ie = $("#txtInscricaoEstadual").val().trim();
            var cep = $("#txtCEP").val().trim();
            var logradouro = $("#txtLogradouro").val().trim();
            var numero = $("#txtNumero").val().trim();
            var bairro = $("#txtBairro").val().trim();
            var telefone1 = $("#txtTelefone1").val().trim();
            var rntrc = $("#txtRNTRC").val().trim();
            var valido = true;
            if (razao == "") {
                CampoComErro("#txtRazaoSocial");
                valido = false;
            } else {
                CampoSemErro("#txtRazaoSocial");
            }
            if (nome == "") {
                CampoComErro("#txtNomeFantasia");
                valido = false;
            } else {
                CampoSemErro("#txtNomeFantasia");
            }
            if (cnpj == "") {
                CampoComErro("#txtCNPJ");
                valido = false;
            } else {
                CampoSemErro("#txtCNPJ");
            }
            if (ie == "") {
                CampoComErro("#txtInscricaoEstadual");
                valido = false;
            } else {
                CampoSemErro("#txtInscricaoEstadual");
            }
            if (cep == "") {
                CampoComErro("#txtCEP");
                valido = false;
            } else {
                CampoSemErro("#txtCEP");
            }
            if (logradouro == "") {
                CampoComErro("#txtLogradouro");
                valido = false;
            } else {
                CampoSemErro("#txtLogradouro");
            }
            if (numero == "") {
                CampoComErro("#txtNumero");
                valido = false;
            } else {
                CampoSemErro("#txtNumero");
            }
            if (bairro == "") {
                CampoComErro("#txtBairro");
                valido = false;
            } else {
                CampoSemErro("#txtBairro");
            }
            if (telefone1 == "") {
                CampoComErro("#txtTelefone1");
                valido = false;
            } else {
                CampoSemErro("#txtTelefone1");
            }
            if (rntrc == "" || rntrc.length != 8) {
                CampoComErro("#txtRNTRC");
                valido = false;
            } else {
                CampoSemErro("#txtRNTRC");
            }
            return valido;
        }
        function LimparCampos() {
            $("#hddCodigo").val('0');
            $("#hddCodigoCriptografado").val('');
            $("#txtRazaoSocial").val('');
            $("#txtNomeFantasia").val('');
            $("#txtCNPJ").val('');
            $("#txtInscricaoEstadual").val('');
            $("#txtInscricaoEstadualSubstituicao").val('');
            $("#txtCNAE").val('');
            $("#txtSUFRAMA").val('');
            $("#txtCEP").val('');
            $("#txtLogradouro").val('');
            $("#txtComplemento").val('');
            $("#txtNumero").val('');
            $("#txtBairro").val('');
            $("#txtTelefone1").val('');
            $("#txtTelefone2").val('');
            $("#selUF").val($("#selUF option:first").val());
            $("#selLocalidade").html('');
            $("#txtContato").val('');
            $("#txtTelefoneContato").val('');
            $("#txtEmails").val('');
            $("#txtEmailsAdministrativos").val('');
            $("#txtEmailsContador").val('');
            $("#txtTelefoneContador").val("");
            $("#txtNomeContador").val("");
            $("#selStatus").val($("#selStatus option:first").val());
            $("#txtRNTRC").val('');
            $("#selSimplesNacional").val($("#selSimplesNacional option:first").val());
            $("#selFusoHorario").val($("#selFusoHorario option:first").val());
            $("#btnUsuarios").hide();
        }
        function BuscarLocalidades(uf, idSelect, codigo) {
            executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
                if (r.Sucesso) {
                    RenderizarLocalidades(r.Objeto, idSelect, codigo);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarLocalidades(localidades, idSelect, codigo) {
            var selLocalidades = document.getElementById(idSelect);
            selLocalidades.options.length = 0;
            for (var i = 0; i < localidades.length; i++) {
                var optn = document.createElement("option");
                optn.text = localidades[i].Descricao;
                optn.value = localidades[i].Codigo;
                if (codigo != null) {
                    if (codigo == localidades[i].Codigo) {
                        optn.setAttribute("selected", "selected");
                    }
                }
                selLocalidades.options.add(optn);
            }
        }
        function Salvar() {
            if (ValidarCampos()) {
                var empresa = {
                    Codigo: $("#hddCodigo").val(),
                    RazaoSocial: $("#txtRazaoSocial").val(),
                    NomeFantasia: $("#txtNomeFantasia").val(),
                    CNPJ: $("#txtCNPJ").val(),
                    InscricaoEstadual: $("#txtInscricaoEstadual").val(),
                    InscricaoEstadualSubstituicao: $("#txtInscricaoEstadualSubstituicao").val(),
                    CNAE: $("#txtCNAE").val(),
                    SUFRAMA: $("#txtSUFRAMA").val(),
                    CEP: $("#txtCEP").val(),
                    Logradouro: $("#txtLogradouro").val(),
                    Complemento: $("#txtComplemento").val(),
                    Numero: $("#txtNumero").val(),
                    Bairro: $("#txtBairro").val(),
                    Telefone: $("#txtTelefone1").val(),
                    Telefone2: $("#txtTelefone2").val(),
                    Localidade: $("#selLocalidade").val(),
                    Contato: $("#txtContato").val(),
                    TelefoneContato: $("#txtTelefoneContato").val(),
                    Emails: $("#txtEmails").val(),
                    EmailsAdministrativos: $("#txtEmailsAdministrativos").val(),
                    EmailsContador: $("#txtEmailsContador").val(),
                    NomeContador: $("#txtNomeContador").val(),
                    TelefoneContador: $("#txtTelefoneContador").val(),
                    Status: $("#selStatus").val(),
                    RNTRC: $("#txtRNTRC").val(),
                    SimplesNacional: $("#selSimplesNacional").val(),
                    FusoHorario: $("#selFusoHorario").val()
                };
                executarRest("/Empresa/SalvarEmpresaGerenciadora?callback=?", empresa, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }
        function BuscarDetalhes(empresa) {
            executarRest("/Empresa/ObterDetalhes?callback=?", { Codigo: empresa.Codigo }, function (r) {
                if (r.Sucesso) {
                    empresa = r.Objeto;
                    $("#hddCodigo").val(empresa.Codigo);
                    $("#txtRazaoSocial").val(empresa.RazaoSocial);
                    $("#txtNomeFantasia").val(empresa.NomeFantasia);
                    $("#txtCNPJ").val(empresa.CNPJ);
                    $("#txtInscricaoEstadual").val(empresa.InscricaoEstadual);
                    $("#txtInscricaoEstadualSubstituicao").val(empresa.InscricaoEstadualSubstituicao);
                    $("#txtCNAE").val(empresa.CNAE);
                    $("#txtSUFRAMA").val(empresa.SUFRAMA);
                    $("#txtCEP").val(empresa.CEP);
                    $("#txtLogradouro").val(empresa.Logradouro);
                    $("#txtComplemento").val(empresa.Complemento);
                    $("#txtNumero").val(empresa.Numero);
                    $("#txtBairro").val(empresa.Bairro);
                    $("#txtTelefone1").val(empresa.Telefone);
                    $("#txtTelefone2").val(empresa.Telefone2);
                    $("#selUF").val(empresa.SiglaUF);
                    BuscarLocalidades(empresa.SiglaUF, "selLocalidade", empresa.Localidade);
                    $("#txtContato").val(empresa.Contato);
                    $("#txtTelefoneContato").val(empresa.TelefoneContato);
                    $("#txtTelefoneContador").val(empresa.TelefoneContador);
                    $("#txtNomeContador").val(empresa.NomeContador);
                    $("#txtEmails").val(empresa.Emails);
                    $("#txtEmailsAdministrativos").val(empresa.EmailsAdministrativos);
                    $("#txtEmailsContador").val(empresa.EmailsContador);
                    $("#selStatus").val(empresa.Status);
                    $("#txtRNTRC").val(empresa.RNTRC);
                    $("#selSimplesNacional").val(empresa.SimplesNacional.toString());
                    $("#selFusoHorario").val(empresa.FusoHorario);
                    $("#hddCodigoCriptografado").val(empresa.CodigoCriptografado);
                    $("#btnUsuarios").show();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
    </script>
    <script type="text/javascript" id="ScriptFusoHorario">
        function ObterFusosHorarios() {
            executarRest("/FusoHorario/ObterListaDeFusos?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarFusosHorarios(r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarFusosHorarios(fusos) {
            var select = document.getElementById("selFusoHorario");
            for (var i = 0; i < fusos.length; i++) {
                var option = document.createElement("option");
                option.text = fusos[i].DisplayName;
                option.value = fusos[i].Id;
                select.appendChild(option);
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoCriptografado" value="" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Cadastro de Empresas Gerenciadoras
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div id="default-search" class="default-search">
                        Pesquisar
                    </div>
                    <div class="fields" style="margin-top: 15px;">
                        <div class="response-msg error ui-corner-all" id="divMensagemErro" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg notice ui-corner-all" id="divMensagemAlerta" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg success ui-corner-all" id="divMensagemSucesso" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="fields">
                            <div class="fieldzao">
                                <div class="field fieldquatro">
                                    <div class="label">
                                        <label>
                                            Razão Social*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtRazaoSocial" maxlength="80" />
                                    </div>
                                </div>
                                <div class="field fieldquatro">
                                    <div class="label">
                                        <label>
                                            Nome Fantasia*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtNomeFantasia" maxlength="80" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            CNPJ*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtCNPJ" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Inscrição Estadual*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtInscricaoEstadual" maxlength="20" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            IE de Subst. Trib.:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtInscricaoEstadualSubstituicao" maxlength="20" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            CNAE*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtCNAE" maxlength="100" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            SUFRAMA:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSUFRAMA" maxlength="100" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            CEP*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtCEP" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fieldquatro">
                                    <div class="label">
                                        <label>
                                            Logradouro*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtLogradouro" maxlength="100" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Complemento:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtComplemento" maxlength="100" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Número*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtNumero" maxlength="20" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Bairro*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtBairro" maxlength="80" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Telefone 1*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTelefone1" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Telefone 2:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTelefone2" class="maskedInput" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            UF*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selUF" class="select">
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Município*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selLocalidade" class="select">
                                        </select>
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Contato:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtContato" maxlength="80" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Telefone Contato:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTelefoneContato" class="maskedInput" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Nome Contador:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtNomeContador" maxlength="200" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Telefone Contador:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTelefoneContador" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            RNTRC*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtRNTRC" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Simples Nacional*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selSimplesNacional" class="select">
                                            <option value="false">Não</option>
                                            <option value="true">Sim</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Fuso Horário*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selFusoHorario" class="select">
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Status*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selStatus" class="select">
                                            <option value="A">Ativo</option>
                                            <option value="I">Inativo</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldsete">
                                    <div class="label">
                                        <label>
                                            E-mails:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmails" maxlength="1000" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldsete">
                                    <div class="label">
                                        <label>
                                            E-mails Administrativos:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmailsAdministrativos" maxlength="1000" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao" style="margin-bottom: 15px;">
                                <div class="field fieldsete">
                                    <div class="label">
                                        <label>
                                            E-mails do Contador:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmailsContador" maxlength="1000" />
                                    </div>
                                </div>
                            </div>
                            <div class="buttons" style="margin-left: 5px;">
                                <input type="button" id="btnSalvar" value="Salvar" />
                                <input type="button" id="btnCancelar" value="Cancelar" />
                                <input type="button" id="btnUsuarios" value="Usuários da Empresa" style="display: none; margin-left: 15px;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
