<%@ Page Title="Cadastro de Perfil Permissão" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="PerfilPermissao.aspx.cs" Inherits="EmissaoCTe.WebApp.PerfilPermissao" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" id="ScriptPermissoes" type="text/javascript">
        $(document).ready(function () {
            ObterFormularios();
        });

        function ObterFormulariosEmpresa() {
            executarRest("/Pagina/BuscarPaginasDaEmpresa?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarFormularios(r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function ObterFormularios() {
            executarRest("/PerfilPermissao/BuscarPaginas?callback=?", { Codigo: $("#hddCodigo").val() }, function (r) {
                if (r.Sucesso) {
                    RenderizarFormularios(r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RenderizarFormularios(formularios) {
            $("#divPermissoesPaginas").html("");
            var idDivPai = 'divPermissoesPaginas';
            for (var i = 0; i < formularios.length; i++) {

                var nomeGrupo = formularios[i].Grupo.replace(/\s/g, "");
                var idBody = "divBodyPermissoes_" + nomeGrupo;
                var idCheckbox = "chkCheckUncheck_" + nomeGrupo;

                var html = '';
                html += '<div class="panel panel-default"><div class="panel-heading">';
                html += '    <h4 class="panel-title">';
                html += '        <input type="checkbox" id="' + idCheckbox + '" />';
                html += '        <a class="accordion-toggle" data-toggle="collapse" data-parent="#' + idDivPai + '" href="#' + idBody + '">' + formularios[i].Grupo + '</a>';
                html += '    </h4>';
                html += '</div>';
                html += '<div id="' + idBody + '" class="panel-collapse collapse">';
                html += '    <div class="panel-body">';
                html += '        <div class="table-responsive">'
                html += '            <table class="table table-bordered table-condensed table-hover">';
                html += '                <thead>';
                html += '                    <th>Página</th>';
                html += '                    <th style="width: 60px; text-align: center;">Acesso</th>';
                html += '                    <th style="width: 60px; text-align: center;">Incluir</th>';
                html += '                    <th style="width: 60px; text-align: center;">Alterar</th>';
                html += '                    <th style="width: 60px; text-align: center;">Excluir</th>';
                html += '                </thead>';
                html += '                <tbody>';

                for (var j = 0; j < formularios[i].Paginas.length; j++) {
                    html += '                <tr class="tr_pagina_permissao" id="tr_' + formularios[i].Paginas[j].Codigo + '">';
                    html += '                    <td>' + formularios[i].Paginas[j].Descricao + '</td>';
                    html += '                    <td style="text-align: center;"><input type="checkbox" class="chkPermissaoAcesso" onclick="AlterarEstadoAcesso(this);" id="chkAcesso_' + formularios[i].Paginas[j].Codigo + '" /></td>';
                    html += '                    <td style="text-align: center;"><input type="checkbox" disabled="disabled" id="chkIncluir_' + formularios[i].Paginas[j].Codigo + '" /></td>';
                    html += '                    <td style="text-align: center;"><input type="checkbox" disabled="disabled" id="chkAlterar_' + formularios[i].Paginas[j].Codigo + '" /></td>';
                    html += '                    <td style="text-align: center;"><input type="checkbox" disabled="disabled" id="chkExcluir_' + formularios[i].Paginas[j].Codigo + '" /></td>';
                    html += '                </tr>'
                }

                html += '                </tbody>';
                html += '            </table>';
                html += '        </div>';
                html += '    </div>';
                html += '</div></div>';

                $("#divPermissoesPaginas").append(html);

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
                    $(this).prop('disabled', !chk.checked);
                $(this).prop('checked', chk.checked);
            });
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
                $("#chkAcesso_" + permissoes[i].Codigo).prop("checked", permissoes[i].Acesso);
                $("#chkIncluir_" + permissoes[i].Codigo).prop({ checked: permissoes[i].Incluir, disabled: !permissoes[i].Acesso });
                $("#chkAlterar_" + permissoes[i].Codigo).prop({ checked: permissoes[i].Alterar, disabled: !permissoes[i].Acesso });
                $("#chkExcluir_" + permissoes[i].Codigo).prop({ checked: permissoes[i].Excluir, disabled: !permissoes[i].Acesso });
            }
        }
        function LimparPermissoes() {
            $(".chkPermissaoAcesso").each(function () {
                $(this).prop("checked", false);
                AlterarEstadoAcesso(this);
            });
        }
        function AlterarEstadoAcesso(chk) {
            var id = chk.id.split('_')[1];
            if (chk.checked) {
                $("#chkIncluir_" + id).prop("disabled", false);
                $("#chkAlterar_" + id).prop("disabled", false);
                $("#chkExcluir_" + id).prop("disabled", false);
            } else {
                $("#chkIncluir_" + id).prop({ disabled: true, checked: false });
                $("#chkAlterar_" + id).prop({ disabled: true, checked: false });
                $("#chkExcluir_" + id).prop({ disabled: true, checked: false });
            }
        }
    </script>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {

            CarregarConsultaDePerfil("default-search", "default-search", Editar, true, false);

            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            LimparCampos();
        });
        function LimparCampos() {
            $("#hddCodigo").val('0');
            $("#txtDescricao").val('');
            $("#selStatus").val($("#selStatus option:first").val());
            $("checkbox").each(function () {
                $(this).attr("checked", false);
            });
            LimparPermissoes();
        }
        function ValidarCampos() {
            var nome = $("#txtDescricao").val().trim();
            var valido = true;
            if (nome != "") {
                CampoSemErro("#txtDescricao");
            } else {
                valido = false;
                CampoComErro("#txtDescricao");
            }
            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    Permissoes: JSON.stringify(ObterPermissoes()),
                    Descricao: $("#txtDescricao").val(),
                    Status: $("#selStatus").val()
                };
                executarRest("/PerfilPermissao/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            }
        }
        function Editar(perfil) {
            LimparCampos();
            executarRest("/PerfilPermissao/ObterDetalhes?callback=?", { Codigo: perfil.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#txtDescricao").val(r.Objeto.Descricao);
                    $("#selStatus").val(r.Objeto.Ativo);
                    PreencherPermissoes(r.Objeto.Permissoes);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Perfil Permissão
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-7 col-md-5 col-lg-5">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <input type="text" id="txtDescricao" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="True">Ativo</option>
                    <option value="False">Inativo</option>
                </select>
            </div>
        </div>
    </div>
    <h3>Permissões
    </h3>
    <div class="panel-group" id="divPermissoesPaginas" style="margin-top: 10px; margin-bottom: 15px;">
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
