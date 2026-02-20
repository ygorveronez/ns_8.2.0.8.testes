<%@ Page Title="Serviços para NFS-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ServicosNFSe.aspx.cs" Inherits="EmissaoCTe.WebApp.ServicosNFSe" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/validaCampos") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtNumero").mask("9?99999999999999");
            $("#txtAliquota").priceFormat({ prefix: '', centsLimit: 4 });

            CarregarConsultaDeServicosNFSe("default-search", "default-search", "", RetornoConsultaServico, true, false);

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });

        function RetornoConsultaServico(servico) {
            executarRest("/ServicoNFSe/ObterDetalhes?callback=?", { CodigoServico: servico.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#txtNumero").val(r.Objeto.Numero);
                    $("#txtDescricao").val(r.Objeto.Descricao);
                    $("#txtAliquota").val(r.Objeto.Aliquota);
                    $("#txtCNAE").val(r.Objeto.CNAE);
                    $("#txtCodigoTributacao").val(r.Objeto.CodigoTributacao);
                    $("#selStatus").val(r.Objeto.Status);
                    $("#selISSRetido").val(r.Objeto.ISSRetido);       
                    $("#txtNBS").val(r.Objeto.NBS);       
                    $("#selISSIncluso").val(r.Objeto.ISSIncluso);      
                    $("body").data("codigo", r.Objeto.Codigo);
                } else {
                    ExibirMensagemAlerta(r.Erro, "Atenção!");
                }
            });
        }

        function LimparCampos() {
            $("#txtNumero").val("");
            $("#txtDescricao").val('');
            $("#txtAliquota").val("0,0000");
            $("#txtCNAE").val("");
            $("#txtNBS").val("");
            $("#txtCodigoTributacao").val("");
            $("#selStatus").val($("#selStatus option:first").val());
            $("#selISSRetido").val($("#selISSRetido option:first").val());    
            $("#selISSIncluso").val($("#selISSIncluso option:first").val());                
            $("body").data("codigo", null);
        }

        function ValidarCampos() {
            var descricao = $("#txtDescricao").val().trim();
            var numero = Globalize.parseInt($("#txtNumero").val());
            var valido = true;

            if (descricao != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }

            if (!isNaN(numero) && numero > 0) {
                CampoSemErro("#txtNumero");
            } else {
                CampoComErro("#txtNumero");
                valido = false;
            }

            return valido;
        }

        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("body").data("codigo"),
                    Numero: $("#txtNumero").val(),
                    Descricao: $("#txtDescricao").val(),
                    Aliquota: $("#txtAliquota").val(),
                    CNAE: $("#txtCNAE").val(),
                    NBS: $("#txtNBS").val(),
                    CodigoTributacao: $("#txtCodigoTributacao").val(),
                    Status: $("#selStatus").val(),
                    ISSRetido: $("#selISSRetido").val(),
                    ISSIncluso: $("#selISSIncluso").val()
                };

                executarRest("/ServicoNFSe/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Serviços para NFS-e
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Número*:
                </span>
                <input type="text" id="txtNumero" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <input type="text" id="txtDescricao" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Alíquota*:
                </span>
                <input type="text" id="txtAliquota" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">CNAE:
                </span>
                <input type="text" id="txtCNAE" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">NBS:
                </span>
                <input type="text" id="txtNBS" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Cod. Tributação:
                </span>
                <input type="text" id="txtCodigoTributacao" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">ISS Retido (Integração):
                </span>
                <select id="selISSRetido" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">ISS Incluso (Integração):
                </span>
                <select id="selISSIncluso" class="form-control">
                    <option value="0">Padrão Integração</option>
                    <option value="1">Sempre Incluir</option>
                    <option value="2">Nunca Incluir</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
