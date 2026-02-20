<%@ Page Title="Cadastro de Motoristas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Motoristas.aspx.cs" Inherits="EmissaoCTe.WebApp.Motoristas" %>

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
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/motoristas") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Motoristas
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-5">
            <div class="input-group">
                <span class="input-group-addon">Nome*:
                </span>
                <input type="text" id="txtNome" class="form-control" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">CPF/CNPJ*:
                </span>
                <input type="text" id="txtCPFCNPJ" class="form-control maskedInput" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">RG/IE:
                </span>
                <input type="text" id="txtRGIE" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Emissor RG:
                </span>
                <select id="selOrgaoEmissorRG" class="form-control">
                    <option value="">Nenhum</option>
                    <option value="1">SSP</option>
                    <option value="2">CNH</option>
                    <option value="3">MMA</option>
                    <option value="4">DIC</option>
                    <option value="5">POF</option>
                    <option value="6">IFP</option>
                    <option value="7">POM</option>
                    <option value="8">IPF</option>
                    <option value="9">SES</option>
                    <option value="10">MAE</option>
                    <option value="11">MEX</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
            <div class="input-group">
                <span class="input-group-addon">UF RG:
                </span>
                <select id="selUFRG" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Sexo:
                </span>
                <select id="selSexo" class="form-control">
                    <option value="">Nenhum</option>
                    <option value="1">Masculino</option>
                    <option value="2">Feminino</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Telefone:
                </span>
                <input type="text" id="txtTelefone" class="form-control maskedInput phone" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Nascimento">Data Nasc.</abbr>:
                </span>
                <input type="text" id="txtDataNascimento" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Admissão">Data Adm.</abbr>:
                </span>
                <input type="text" id="txtDataAdmissao" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Salário:
                </span>
                <input type="text" id="txtSalario" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Tipo Sanguíneo">Tipo Sang.</abbr>:
                </span>
                <input type="text" id="txtTipoSanguineo" class="form-control" maxlength="3" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Número da Habilitação">Núm. Hab.</abbr>:
                </span>
                <input type="text" id="txtNumeroHabilitacao" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data da Habilitação">Data Hab.</abbr>:
                </span>
                <input type="text" id="txtDataHabilitacao" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Vencimento da Habilitação">Vcto. Hab.</abbr>:
                </span>
                <input type="text" id="txtDataVencimentoHabilitacao" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Categoria da Habilitação">Cat. Hab.</abbr>:
                </span>
                <input type="text" id="txtCategoriaHabilitacao" class="form-control" maxlength="5" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Validade da Seguradora (GR)">Validade Seg..</abbr>:
                </span>
                <input type="text" id="txtDataValidadeSeguradora" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">MOOP:</span>
                <input type="text" id="txtMOOP" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">% Comissão:</span>
                <input type="text" id="txtPercentualComissao" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">PIS/PASEP:</span>
                <input type="text" id="txtPIS" class="form-control" maxlength="30" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Nº Cartão:</span>
                <input type="text" id="txtNumeroCartao" class="form-control" maxlength="16" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">UF*:</span>
                <select id="selUF" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Município*:</span>
                <select id="selLocalidade" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">CEP:</span>
                <input type="text" id="txtCEP" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
            <div class="input-group">
                <span class="input-group-addon">Endereço:</span>
                <input type="text" id="txtEndereco" class="form-control" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Bairro:</span>
                <input type="text" id="txtBairro" class="form-control" maxlength="40" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Complemento:</span>
                <input type="text" id="txtComplemento" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">E-mail*:</span>
                <input type="text" id="txtRemoverAutoFill" style="height: 0px; width: 0px; border: 0px; display: block;" />
                <input type="text" id="txtEmail" class="form-control" maxlength="200" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status*:</span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
    </div>
    <div class="panel-group" id="dadosUsuario" style="margin-bottom: 10px; margin-top: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#dadosUsuario" href="#dados">Dados de Usuário do Sistema
                    </a>
                </h4>
            </div>
            <div id="dados" class="panel-collapse collapse">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Usuário*:
                                </span>
                                <input type="text" id="txtUsuario" class="form-control" maxlength="20" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Senha*:
                                </span>
                                <input type="password" id="txtRemoverAutoFill2" style="height: 0px; width: 0px; border: 0px; display: block;" />
                                <input type="password" id="txtSenha" class="form-control" maxlength="15" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Confirmação da Senha">Conf. Senha</abbr>:
                                </span>
                                <input type="password" id="txtConfirmacaoSenha" class="form-control" maxlength="15" />
                            </div>
                        </div>
                    </div>
                    <h3>Permissões
                    </h3>
                    <div class="panel-group" id="divPermissoesPaginas" style="margin-top: 10px; margin-bottom: 15px;">
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
