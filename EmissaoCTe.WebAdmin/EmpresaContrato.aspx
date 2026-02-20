<%@ Page Title="Cadastro Contrato Empresa" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EmpresaContrato.aspx.cs" Inherits="EmissaoCTe.WebAdmin.EmpresaContrato" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {

            CarregarConsultaDeContratosDaEmpresa("default-search", "default-search", RetornoConsultaContratoEmpresa, true, false);

            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresas, true, false);


            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            $("#btnExcluir").click(function () {
                Excluir();
            });

            $("#btnRazaoSocialTransportadora").click(function () {
                InserirTag("txtContrato", "#RazaoSocialTransportadora#");
            });

            $("#btnCNPJTransportadora").click(function () {
                InserirTag("txtContrato", "#CNPJTransportadora#");
            });

            $("#btnEnderecoTransportadora").click(function () {
                InserirTag("txtContrato", "#EnderecoTransportadora#");
            });

            $("#btnComplementoTransportadora").click(function () {
                InserirTag("txtContrato", "#ComplementoTransportadora#");
            });

            $("#btnBairroTransportadora").click(function () {
                InserirTag("txtContrato", "#BairroTransportadora#");
            });

            $("#btnCidadeUFTransportadora").click(function () {
                InserirTag("txtContrato", "#CidadeUFTransportadora#");
            });

            $("#btnPlanoDeEmissaoTransportadora").click(function () {
                InserirTag("txtContrato", "#PlanoDeEmissaoTransportadora#");
            });

            $("#btnRazaoSocialEmpresaPai").click(function () {
                InserirTag("txtContrato", "#RazaoSocialEmpresaPai#");
            });

            $("#btnCNPJEmpresaPai").click(function () {
                InserirTag("txtContrato", "#CNPJEmpresaPai#");
            });

            $("#btnEnderecoEmpresaPai").click(function () {
                InserirTag("txtContrato", "#EnderecoEmpresaPai#");
            });

            $("#btnComplementoEmpresaPai").click(function () {
                InserirTag("txtContrato", "#ComplementoEmpresaPai#");
            });

            $("#btnBairroEmpresaPai").click(function () {
                InserirTag("txtContrato", "#BairroEmpresaPai#");
            });

            $("#btnCidadeUFEmpresaPai").click(function () {
                InserirTag("txtContrato", "#CidadeUFEmpresaPai#");
            });

            $("#btnContatoEmpresaPai").click(function () {
                InserirTag("txtContrato", "#ContatoEmpresaPai#");
            });

            $("#btnDataAtual").click(function () {
                InserirTag("txtContrato", "#DataAtual#");
            });

            $("#btnDataCadastro").click(function () {
                InserirTag("txtContrato", "#DataCadastro#");
            });
        }); 

        function Excluir() {
            jConfirm("Deseja realmente excluir o contrato?", "Atenção!", function (retorno) {
                if (retorno) {
                    executarRest("/EmpresaContrato/Excluir?callback=?", { Codigo: $("#hddCodigo").val() }, function (r) {
                        if (r.Sucesso) {
                            LimparCampos();
                            ExibirMensagemSucesso("Dados excluidos com sucesso!", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }

        function RetornoConsultaContratoEmpresa(contrato) {
            executarRest("/EmpresaContrato/ObterDetalhes?callback=?", { Codigo: contrato.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#hddCodigoEmpresa").val(r.Objeto.CodigoEmpresa);
                    $("#txtEmpresa").val(r.Objeto.DescricaoEmpresa);
                    $("#txtContrato").val(r.Objeto.Contrato);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RetornoConsultaEmpresas(empresa) {
            $("#hddCodigoEmpresa").val(empresa.Codigo);
            $("#txtEmpresa").val(empresa.CNPJ + " - " + empresa.NomeFantasia)
        }

        function Salvar() {
            if (ValidarDados()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    CodigoEmpresa: $("#hddCodigoEmpresa").val(),
                    Contrato: $("#txtContrato").val()
                };

                executarRest("/EmpresaContrato/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            }
        }

        function LimparCampos() {
            $("#hddCodigoEmpresa").val('0');
            $("#txtEmpresa").val('');
            $("#txtContrato").val('');
        }

        function ValidarDados() {
            var codigoEmpresa = Globalize.parseInt($("#hddCodigoEmpresa").val());
            var valido = true;

            if (!isNaN(codigoEmpresa) && codigoEmpresa > 0) {
                CampoSemErro("#txtEmpresa");
            } else {
                CampoComErro("#txtEmpresa");
                valido = false;
            }

            return valido;
        }
        function InserirTag(id, text) {
            if (id != null && id.trim() != "") {
                var txtarea = document.getElementById(id);
                var scrollPos = txtarea.scrollTop;
                var strPos = 0;
                var br = ((txtarea.selectionStart || txtarea.selectionStart == '0') ? "ff" : (document.selection ? "ie" : false));
                if (br == "ie") {
                    txtarea.focus();
                    var range = document.selection.createRange();
                    range.moveStart('character', -txtarea.value.length);
                    strPos = range.text.length;
                } else if (br == "ff") {
                    strPos = txtarea.selectionStart;
                }
                var front = (txtarea.value).substring(0, strPos);
                var back = (txtarea.value).substring(strPos, txtarea.value.length);
                txtarea.value = front + text + back;
                strPos = strPos + text.length;
                if (br == "ie") {
                    txtarea.focus();
                    var range = document.selection.createRange();
                    range.moveStart('character', -txtarea.value.length);
                    range.moveStart('character', strPos);
                    range.moveEnd('character', 0);
                    range.select();
                } else if (br == "ff") {
                    txtarea.selectionStart = strPos;
                    txtarea.selectionEnd = strPos;
                    txtarea.focus();
                }
                txtarea.scrollTop = scrollPos;
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoEmpresa" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Cadastro de Contrato Empresa
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
                        <div class="fieldzao" style="margin-bottom: 15px;">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        Empresa*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmpresa" />
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarEmpresa" value="Buscar" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao" style="margin-bottom: 15px;">
                            <div class="field fieldoito">
                                <span><b>Tags</b>:</span>
                                <input type="button" id="btnRazaoSocialTransportadora" value="Razão Social Transportadora" />
                                <input type="button" id="btnCNPJTransportadora" value="CNPJ Transportadora" />
                                <input type="button" id="btnEnderecoTransportadora" value="Endereço Transportadora" />
                                <input type="button" id="btnComplementoTransportadora" value="Complemento Transportadora" />
                                <input type="button" id="btnBairroTransportadora" value="Bairro Transportadora" />
                                <input type="button" id="btnCidadeUFTransportadora" value="Cidade-UF Transportadora" />
                                <input type="button" id="btnPlanoDeEmissaoTransportadora" value="Plano de Emissão" />

                                <input type="button" id="btnRazaoSocialEmpresaPai" value="Razão Social Empresa ADM" />
                                <input type="button" id="btnCNPJEmpresaPai" value="CNPJ Empresa ADM" />
                                <input type="button" id="btnEnderecoEmpresaPai" value="Endereço Empresa ADM" />
                                <input type="button" id="btnComplementoEmpresaPai" value="Complemento Empresa ADM" />
                                <input type="button" id="btnBairroEmpresaPai" value="Bairro Empresa ADM" />
                                <input type="button" id="btnCidadeUFEmpresaPai" value="Cidade-UF Empresa ADM" />
                                <input type="button" id="btnContatoEmpresaPai" value="Responsável Empresa ADM" />
                                
                                <input type="button" id="btnDataAtual" value="Data Atual" />     
                                <input type="button" id="btnDataCadastro" value="Data Cadastro" />                                
                            </div>
                        </div>
                        <div class="fieldzao" style="margin-bottom: 15px;">
                            <div class="field fieldoito">
                                <div class="label">
                                    <label>
                                        Contrato:
                                    </label>
                                </div>
                                <div class="input">
                                    <textarea id="txtContrato" rows="50" cols="10" style="width: 99.5%"></textarea>
                                </div>
                            </div>
                        </div>

                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnSalvar" value="Salvar" />
                            <input type="button" id="btnExcluir" value="Excluir" />
                            <input type="button" id="btnCancelar" value="Cancelar" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
