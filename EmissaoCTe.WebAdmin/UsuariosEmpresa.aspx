<%@ Page Title="CT-e Admin - Usuários da Empresa" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="UsuariosEmpresa.aspx.cs" Inherits="EmissaoCTe.WebAdmin.UsuariosEmpresa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .divPermissoes {
            margin-bottom: 15px;
            display: block;
            width: 100%;
        }

        .divHeaderPermissoes {
            display: block;
            width: 99.1%;
            padding: 5px 5px 5px 7px;
            background-color: #EEEEEE;
            border: 1px solid #CDCDCD;
            border-bottom: 0px;
            cursor: pointer;
        }

            .divHeaderPermissoes span {
                font-size: 12px;
                font-weight: bold;
            }

        .divBodyPermissoes {
            display: block;
            width: 100%;
        }

        .fields .fields-title {
            border-bottom: #cdcdcd solid 1px;
            margin: 0 2px 5px 5px;
        }

            .fields .fields-title h3 {
                font-size: 1.5em;
                padding: 0 0 8px 8px;
                font-family: "Segoe UI", "Frutiger", "Tahoma", "Helvetica", "Helvetica Neue", "Arial", "sans-serif";
                color: #000;
                letter-spacing: -1px;
                font-weight: bold;
            }
    </style>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" id="ScriptLocalidades" type="text/javascript">
        $(document).ready(function () {
            BuscarUFs("selUF");
            $("#selUF").change(function () {
                BuscarLocalidades($(this).val(), "selLocalidade", null);
            });
        });
        function BuscarLocalidades(uf, idSelect, codigo) {
            executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
                if (r.Sucesso) {
                    RenderizarLocalidades(r.Objeto, idSelect, codigo);
                } else {
                    jAlert(r.Erro, "Atenção");
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
        function BuscarUFs(idSelect) {
            executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarUFs(r.Objeto, idSelect);
                } else {
                    jAlert(r.Erro, "Atenção");
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
    </script>
    <script defer="defer" id="ScriptPermissoes" type="text/javascript">
        $(document).ready(function () {
            ObterFormularios();
        });
        function ObterFormularios() {
            executarRest("/Pagina/BuscarPaginasPorEmpresa?callback=?", { CodigoEmpresa: GetUrlParam("x") }, function (r) {
                if (r.Sucesso) {
                    RenderizarFormularios(r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarFormularios(formularios) {
            $("#divPermissoesPaginas").html("");
            for (var i = 0; i < formularios.length; i++) {
                var html = "<div class='divPermissoes'>";
                html += "<div id='divHeaderPermissoes_" + formularios[i].Grupo.replace(/\s/g, "") + "' class='divHeaderPermissoes'>";
                html += "<input type='checkbox' id='chkCheckUncheck_" + formularios[i].Grupo.replace(/\s/g, "") + "' /><span>" + formularios[i].Grupo + "</span>";
                html += "</div><div class='divBodyPermissoes' id='divBodyPermissoes_" + formularios[i].Grupo.replace(/\s/g, "") + "'><table><theader><tr>";
                html += "<th>Página</th>";
                html += "<th style='width: 60px; text-align: center;'>Acesso</th>";
                html += "<th style='width: 60px; text-align: center;'>Incluir</th>";
                html += "<th style='width: 60px; text-align: center;'>Alterar</th>";
                html += "<th style='width: 60px; text-align: center;'>Excluir</th></tr></theader><tbody>";
                for (var j = 0; j < formularios[i].Paginas.length; j++) {
                    html += "<tr class='tr_pagina_permissao' id='tr_" + formularios[i].Paginas[j].Codigo + "'>";
                    html += "<td>" + formularios[i].Paginas[j].Descricao + "</td>";
                    html += "<td style='text-align: center;'><input type='checkbox' class='chkPermissaoAcesso' onclick='AlterarEstadoAcesso(this);' id='chkAcesso_" + formularios[i].Paginas[j].Codigo + "' /></td>";
                    html += "<td style='text-align: center;'><input type='checkbox' disabled='disabled' id='chkIncluir_" + formularios[i].Paginas[j].Codigo + "' /></td>";
                    html += "<td style='text-align: center;'><input type='checkbox' disabled='disabled' id='chkAlterar_" + formularios[i].Paginas[j].Codigo + "' /></td>";
                    html += "<td style='text-align: center;'><input type='checkbox' disabled='disabled' id='chkExcluir_" + formularios[i].Paginas[j].Codigo + "' /></td></tr>";
                }
                html += "</tbody></div></div>";
                $("#divPermissoesPaginas").append(html);
                $("#divHeaderPermissoes_" + formularios[i].Grupo.replace(/\s/g, "")).click(function (e) {
                    AlterarEstado(this);
                    e.stopPropagation();
                });
                $("#chkCheckUncheck_" + formularios[i].Grupo.replace(/\s/g, "")).click(function (e) {
                    SelecionarTodosDoGrupo(this);
                    e.stopPropagation();
                });
            }
        }
        function SelecionarTodosDoGrupo(chk) {
            var grupo = chk.id.split("_")[1];
            $('#divBodyPermissoes_' + grupo + ' input[type=checkbox]').each(function () {
                if (this.id.indexOf('chkIncluir_') > -1 || this.id.indexOf('chkAlterar_') > -1 || this.id.indexOf('chkExcluir_') > -1)
                    $(this).attr('disabled', !chk.checked);
                $(this).attr('checked', chk.checked);
            });
        }
        function AlterarEstado(div) {
            var id = div.id.split('_')[1];
            if ($("#divBodyPermissoes_" + id).css("display") == "none") {
                $("#divBodyPermissoes_" + id).slideDown();
            } else {
                $("#divBodyPermissoes_" + id).slideUp();
            }
        }
        function ObterPermissoes() {
            var permissoes = new Array();
            $("#divPermissoesPaginas .tr_pagina_permissao").each(function () {
                var id = this.id.split('_')[1];
                permissoes.push({
                    Codigo: id,
                    Acesso: $("#chkAcesso_" + id)[0].checked,
                    Incluir: $("#chkIncluir_" + id)[0].checked,
                    Alterar: $("#chkAlterar_" + id)[0].checked,
                    Excluir: $("#chkExcluir_" + id)[0].checked
                });
            });
            return permissoes;
        }
        function PreencherPermissoes(permissoes) {
            for (var i = 0; i < permissoes.length; i++) {
                $("#chkAcesso_" + permissoes[i].Codigo).attr("checked", permissoes[i].Acesso);
                $("#chkIncluir_" + permissoes[i].Codigo).attr({ checked: permissoes[i].Incluir, disabled: !permissoes[i].Acesso });
                $("#chkAlterar_" + permissoes[i].Codigo).attr({ checked: permissoes[i].Alterar, disabled: !permissoes[i].Acesso });
                $("#chkExcluir_" + permissoes[i].Codigo).attr({ checked: permissoes[i].Excluir, disabled: !permissoes[i].Acesso });
            }
        }
        function LimparPermissoes() {
            $(".chkPermissaoAcesso").each(function () {
                $(this).attr("checked", false);
                AlterarEstadoAcesso(this);
            });
        }
        function AlterarEstadoAcesso(chk) {
            var id = chk.id.split('_')[1];
            if (chk.checked) {
                $("#chkIncluir_" + id).attr("disabled", false);
                $("#chkAlterar_" + id).attr("disabled", false);
                $("#chkExcluir_" + id).attr("disabled", false);
            } else {
                $("#chkIncluir_" + id).attr({ disabled: true, checked: false });
                $("#chkAlterar_" + id).attr({ disabled: true, checked: false });
                $("#chkExcluir_" + id).attr({ disabled: true, checked: false });
            }
        }
    </script>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtCPFCNPJ").mask("99999999999?999");
            $("#txtDataNascimento").datepicker({ changeMonth: true, changeYear: true });
            $("#txtDataAdmissao").datepicker({ changeMonth: true, changeYear: true });
            $("#txtSalario").priceFormat({ prefix: '' });
            $("#txtDataNascimento").mask("99/99/9999");
            $("#txtDataAdmissao").mask("99/99/9999");
            CarregarConsultaDeUsuarios("default-search", "default-search", GetUrlParam("x"), Editar, true, false);
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });
        function LimparCampos() {
            $("#hddCodigo").val('0');
            $("#txtNome").val('');
            $("#txtCPFCNPJ").val('');
            $("#txtRGIE").val('');
            $("#txtDataNascimento").val('');
            $("#txtDataAdmissao").val('');
            $("#txtTelefone").val('').change();
            $("#selUF").val($("#selUF option:first").val());
            $("#txtDataNascimento").val('');
            $("#txtDataAdmissao").val('');
            $("#txtSalario").val('0,00');
            $("#selLocalidade").html("");
            $("#txtEndereco").val("");
            $("#txtComplemento").val("");
            $("#txtEmail").val('');
            $("#txtUsuario").val('');
            $("#txtSenha").val('');
            $("#txtConfirmacaoSenha").val('');
            $("#selStatus").val($("#selStatus option:first").val());
            $("#btnCancelar").hide();
            $("checkbox").each(function () {
                $(this).attr("checked", false);
            });
            LimparPermissoes();
            LimparCamposSerie();
            $("body").data("series", null);
            RenderizarSeries();
        }
        function ValidarCampos() {
            var nome = $("#txtNome").val().trim();
            var cpfCnpj = $("#txtCPFCNPJ").val().trim();
            var localidade = $("#selLocalidade").val();
            var email = $("#txtEmail").val();
            var usuario = $("#txtUsuario").val();
            var senha = $("#txtSenha").val();
            var confirmacaoSenha = $("#txtConfirmacaoSenha").val();
            var valido = true;
            if (nome != "") {
                CampoSemErro("#txtNome");
            } else {
                valido = false;
                CampoComErro("#txtNome");
            }
            if (cpfCnpj.length == 11) {
                if (cpfCnpj != "11111111111") {
                    if (ValidarCPF(cpfCnpj)) {
                        CampoSemErro("#txtCPFCNPJ");
                    } else {
                        CampoComErro("#txtCPFCNPJ");
                        valido = false;
                    }
                }
            } else if (cpfCnpj.length == 14) {
                if (ValidarCNPJ(cpfCnpj)) {
                    CampoSemErro("#txtCPFCNPJ");
                } else {
                    CampoComErro("#txtCPFCNPJ");
                    valido = false;
                }
            } else {
                CampoComErro("#txtCPFCNPJ");
                valido = false;
            }
            if (localidade != "0") {
                CampoSemErro("#selLocalidade");
            } else {
                CampoComErro("#selLocalidade");
                valido = false;
            }
            if (ValidarEmail(email)) {
                CampoSemErro("#txtEmail");
            } else {
                CampoComErro("#txtEmail");
                valido = false;
            }
            if (usuario.length >= 5) {
                CampoSemErro("#txtUsuario");
            } else {
                CampoComErro("#txtUsuario");
                valido = false;
            }
            if (senha.length >= 5 && (senha == confirmacaoSenha)) {
                CampoSemErro("#txtSenha");
                CampoSemErro("#txtConfirmacaoSenha");
            } else {
                CampoComErro("#txtSenha");
                CampoComErro("#txtConfirmacaoSenha");
                valido = false;
            }
            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    CodigoEmpresa: GetUrlParam("x"),
                    Permissoes: JSON.stringify(ObterPermissoes()),
                    Nome: $("#txtNome").val(),
                    CPFCNPJ: $("#txtCPFCNPJ").val(),
                    RGIE: $("#txtRGIE").val(),
                    DataNascimento: $("#txtDataNascimento").val(),
                    DataAdmissao: $("#txtDataAdmissao").val(),
                    Salario: $("#txtSalario").val(),
                    Telefone: $("#txtTelefone").val(),
                    Localidade: $("#selLocalidade").val(),
                    Endereco: $("#txtEndereco").val(),
                    Complemento: $("#txtComplemento").val(),
                    Email: $("#txtEmail").val(),
                    Usuario: $("#txtUsuario").val(),
                    Senha: $("#txtSenha").val(),
                    Status: $("#selStatus").val(),
                    AlterarSenhaAcesso: false,
                    ConfirmacaoSenha: $("#txtConfirmacaoSenha").val(),
                    Series: ($("body").data("series") != null ? JSON.stringify($("body").data("series")) : "")
                };
                executarRest("/Usuario/SalvarPorEmpresa?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            }
        }
        function Editar(usuario) {
            LimparCampos();
            executarRest("/Usuario/ObterDetalhesPorEmpresa?callback=?", { CodigoUsuario: usuario.Codigo, CodigoEmpresa: GetUrlParam("x") }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#txtNome").val(r.Objeto.Nome);
                    $("#txtCPFCNPJ").val(r.Objeto.CPFCNPJ);
                    $("#txtRGIE").val(r.Objeto.RGIE);
                    $("#txtDataNascimento").val(r.Objeto.DataNascimento);
                    $("#txtDataAdmissao").val(r.Objeto.DataAdmissao);
                    $("#txtSalario").val(r.Objeto.Salario);
                    $("#txtTelefone").val(r.Objeto.Telefone).change();
                    $("#selUF").val(r.Objeto.SiglaUF);
                    BuscarLocalidades(r.Objeto.SiglaUF, "selLocalidade", r.Objeto.Localidade);
                    $("#txtEndereco").val(r.Objeto.Endereco);
                    $("#txtComplemento").val(r.Objeto.Complemento);
                    $("#txtEmail").val(r.Objeto.Email);
                    $("#txtUsuario").val(r.Objeto.Usuario);
                    $("#txtSenha").val(r.Objeto.Senha);
                    $("#txtConfirmacaoSenha").val(r.Objeto.Senha);
                    $("#selStatus").val(r.Objeto.Status);
                    PreencherPermissoes(r.Objeto.Permissoes);
                    $("body").data("series", r.Objeto.Series);
                    RenderizarSeries();
                    $("#btnCancelar").show();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function GetUrlParam(name) {
            var url = window.location.search.replace("?", "");
            var itens = url.split("&");
            for (n in itens) {
                if (itens[n].match(name)) {
                    return itens[n].replace(name + "=", "");
                }
            }
            return null;
        }
    </script>
    <script defer="defer" type="text/javascript" id="ScriptSeries">
        $(document).ready(function () {

            CarregarConsultaDeSeries("btnBuscarSerie", "btnBuscarSerie", "A", GetUrlParam("x"), RetornoConsultaSerie, true, false);

            $("#txtSerie").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("serie", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnSalvarSerie").click(function () {
                SalvarSerie();
            });

            $("#btnExcluirSerie").click(function () {
                ExcluirSerie();
            });

            $("#btnCancelarSerie").click(function () {
                LimparCamposSerie();
            });

        });

        function RetornoConsultaSerie(serie) {
            $("#txtSerie").val(serie.Numero);
            $("body").data("serie", serie);
        }

        function ValidarSerie() {
            var serie = $("body").data("serie");
            var valido = true;

            if (serie != null && serie.Codigo > 0) {
                CampoSemErro("#txtSerie");
            } else {
                CampoComErro("#txtSerie");
                valido = false;
            }

            return valido;
        }

        function SalvarSerie() {
            if (ValidarSerie()) {
                var series = $("body").data("series") == null ? new Array() : $("body").data("series");
                var serie = $("body").data("serie");

                for (var i = 0; i < series.length; i++) {
                    if (serie.Codigo == series[i].Codigo) {
                        series.splice(i, 1);
                    }
                }

                series.push(serie);
                $("body").data("series", series);
                RenderizarSeries();
                LimparCamposSerie();
            }
        }

        function EditarSerie(serie) {
            $("#txtSerie").val(serie.Numero);
            $("body").data("serie", serie);
            $("#btnExcluirSerie").show();
        }
        function ExcluirSerie() {
            jConfirm("Deseja realmente excluir esta série?", "Atenção", function (r) {
                if (r) {
                    var serie = $("body").data("serie");
                    var series = $("body").data("series") == null ? new Array() : $("body").data("series");

                    for (var i = 0; i < series.length; i++) {
                        if (serie.Codigo == series[i].Codigo) {
                            series.splice(i, 1);
                        }
                    }

                    $("body").data("series", series);
                    RenderizarSeries();
                    LimparCamposSerie();
                }
            });
        }
        function LimparCamposSerie() {
            $("#txtSerie").val('');
            $("body").data("serie", null);
            $("#btnExcluirSerie").hide();
        }
        function RenderizarSeries() {
            var series = $("body").data("series") == null ? new Array() : $("body").data("series");

            $("#tblSeries tbody").html("");

            if (series.length > 0) {

                for (var i = 0; i < series.length; i++) {
                    $("#tblSeries tbody").append("<tr><td>" + series[i].Numero + "</td><td>" + series[i].DescricaoTipo + "</td><td><a href='javascript:void(0);' onclick='EditarSerie(" + JSON.stringify(series[i]) + ");'>Editar</a></td></tr>");
                }

            }

            if ($("#tblSeries tbody").html() == "")
                $("#tblSeries tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Cadastro de Usuários
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
                        <div class="fieldzao">
                            <div class="field fieldquatro">
                                <div class="label">
                                    <label>
                                        Nome*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtNome" maxlength="80" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        CPF/CNPJ*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtCPFCNPJ" maxlength="20" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        RG/IE:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtRGIE" maxlength="20" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Nascimento:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataNascimento" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Admissão:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataAdmissao" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Salário:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtSalario" value="0,00" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Telefone:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtTelefone" class="maskedInput phone" />
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
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Endereço:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEndereco" maxlength="80" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Complemento:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtComplemento" maxlength="20" />
                                </div>
                            </div>
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        E-mail*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmail" maxlength="200" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao" style="margin-bottom: 20px;">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Usuário*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtUsuario" maxlength="20" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Senha*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtUsuarioAutoComplete" style="display: none;" />
                                    <input type="password" id="txtSenhaAutoComplete" style="display: none;" />
                                    <input type="password" id="txtSenha" maxlength="15" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Confirmação Senha*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="password" id="txtConfirmacaoSenha" maxlength="15" />
                                </div>
                            </div>
                            <div class="field fielddois">
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
                        <div class="fields-title">
                            <h3>Séries
                            </h3>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Série:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtSerie" />
                                </div>
                            </div>
                            <div class="field fieldsete">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarSerie" value="Buscar" />
                                    <input type="button" id="btnSalvarSerie" value="Salvar" style="margin-left: 10px;" />
                                    <input type="button" id="btnExcluirSerie" value="Excluir" style="display: none;" />
                                    <input type="button" id="btnCancelarSerie" value="Cancelar" />
                                </div>
                            </div>
                        </div>
                        <div class="table" style="margin-left: 5px; margin-bottom: 20px;">
                            <table id="tblSeries">
                                <thead>
                                    <tr>
                                        <th style="width: 10%;">Numero
                                        </th>
                                        <th style="width: 80%;">Tipo
                                        </th>
                                        <th style="width: 10%">Opções
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="3">Nenhum registro encontrado.</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="fields-title">
                            <h3>Permissões
                            </h3>
                        </div>
                        <div class="table" id="divPermissoesPaginas" style="margin-left: 5px;">
                        </div>
                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnSalvar" value="Salvar" />
                            <input type="button" id="btnCancelar" value="Cancelar" style="display: none;" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
