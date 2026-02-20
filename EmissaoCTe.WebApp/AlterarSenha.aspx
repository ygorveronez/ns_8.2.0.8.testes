<%@ Page Title="Alterar a Senha do Usuário" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="AlterarSenha.aspx.cs" Inherits="EmissaoCTe.WebApp.AlterarSenha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/json",
                                               "~/bundle/scripts/blockui",
                                               "~/bundle/scripts/maskedinput",
                                               "~/bundle/scripts/ajax",
                                               "~/bundle/scripts/mensagens",
                                               "~/bundle/scripts/validaCampos") %>

    <script type="text/javascript">
        $(function () {
            $("#btnAlterarSenha").click(function () {
                Salvar();
            });
        });

        function ValidarCampos() {
            var senha = $("#txtSenha").val();
            var confirmacaoSenha = $("#txtConfirmacaoSenha").val();
            var senhaAtual = $("#txtSenhaAtual").val();
            var valido = true;

            if (senha.length >= 5 && (senha == confirmacaoSenha)) {
                CampoSemErro("#txtSenha");
                CampoSemErro("#txtConfirmacaoSenha");
            } else {
                CampoComErro("#txtSenha");
                CampoComErro("#txtConfirmacaoSenha");
                valido = false;
                ExibirMensagemErro("Não foi possível alterar a senha. Certifique-se que a nova senha possua 6 caracteres ou mais e seja igual à confirmação!", "Erro!");
            }

            if (senhaAtual.length > 0) {
                CampoSemErro("#txtSenhaAtual");
            } else {
                CampoComErro("#txtSenhaAtual");
                valido = false;
                ExibirMensagemErro("Senha atual invalida!", "Erro!");
            }

            return valido;
        }

        function Salvar() {
            if (ValidarCampos()) {
                jConfirm("Deseja realmente alterar a senha do usuário atual?", "Atenção!", function (r) {
                    if (r) {

                        var dados = {
                            Senha: $("#txtSenha").val(),
                            ConfirmacaoSenha: $("#txtConfirmacaoSenha").val(),
                            SenhaAtual: $("#txtSenhaAtual").val()
                        };

                        executarRest("/Usuario/AlterarSenha?callback=?", dados, function (r) {
                            if (r.Sucesso) {
                                jAlert("Senha alterada com sucesso, favor acessar sistema novamente!", "Sucesso!", function (r) {
                                    LimparCampos();
                                    location.href = "Logout.aspx";
                                });
                            } else {
                                ExibirMensagemErro(r.Erro, "Erro!");
                            }
                        });
                    }
                });
            }
        }

        function LimparCampos() {
            $("#txtSenha").val("");
            $("#txtConfirmacaoSenha").val("");
            $("#txtSenhaAtual").val("");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Alterar a Senha do Usuário
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Senha atual*:
                </span>
                <input type="password" id="txtSenhaAtual" autocomplete="off" class="form-control" maxlength="15" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Senha*:
                </span>
                <input type="password" id="txtSenha" autocomplete="off" class="form-control" maxlength="15" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Conf. Senha*:
                </span>
                <input type="password" id="txtConfirmacaoSenha" autocomplete="off" class="form-control" maxlength="15" />
            </div>
        </div>
    </div>
    <button type="button" class="btn btn-primary" id="btnAlterarSenha">Alterar Senha</button>
</asp:Content>
