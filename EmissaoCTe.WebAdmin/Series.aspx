<%@ Page Title="Cadastro de Séries" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Series.aspx.cs" Inherits="EmissaoCTe.WebAdmin.Series" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            LimparCampos();

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#txtSerie").mask("9?99");

            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresa, true, false);
        });

        function CarregarSeries() {
            CriarGridView("/EmpresaSerie/BuscarTodos?callback=?", { inicioRegistros: 0, CodigoEmpresa: $("#hddCodigoEmpresa").val() }, "tbl_series_table", "div_table_series", "div_paginacao_series", [{ Descricao: "Editar", Evento: Editar }], [0, 1]);
        }

        function RetornoConsultaEmpresa(empresa) {
            $("#hddCodigoEmpresa").val(empresa.Codigo);
            $("#txtEmpresa").val(empresa.CNPJ + " - " + empresa.RazaoSocial);
            CarregarSeries();
            LimparCampos();
        }
        
        function ValidarCampos() {
            var serie = $("#txtSerie").val().trim();
            var valido = true;
            if (serie != "") {
                CampoSemErro("#txtSerie");
            } else {
                CampoComErro("#txtSerie");
                valido = false;
            }
            return valido;
        }

        function LimparCampos() {
            $("#txtSerie").val('');
            $("#hddCodigo").val('');
            $("#selStatus").val($("#selStatus option:first").val());
            $("#selTipoSerie").val($("#selTipoSerie option:first").val());
            $("#btnCancelar").hide();
            $("#btnSalvar").show();
        }

        function Salvar() {
            if (ValidarCampos()) {
                executarRest("/EmpresaSerie/Salvar?callback=?", { CodigoEmpresa: $("#hddCodigoEmpresa").val(), Serie: $("#txtSerie").val(), Status: $("#selStatus").val(), Tipo: $("#selTipoSerie").val() }, function (r) {
                    if (r.Sucesso) {
                        CarregarSeries();
                        LimparCampos();
                    } else {
                        jAlert(r.Erro, "Atenção");
                    }
                });
            }
        }

        function Editar(serie) {
            $("#hddCodigo").val(serie.data.Codigo);
            $("#txtSerie").val(serie.data.Numero);
            $("#selStatus").val(serie.data.Status);
            $("#selTipoSerie").val(serie.data.Tipo);
            $("#btnCancelar").show();
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
                <h3>Cadastro de Séries
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div class="fields">
                        <div class="fieldzao">
                            <div class="field fieldcinco">
                                <div class="label">
                                    <label>
                                        Empresa*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmpresa" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarEmpresa" value="Buscar" />
                                </div>
                            </div>
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="div_table_series">
                            </div>
                            <div id="div_paginacao_series" class="pagination">
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Série*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtSerie" class="maskedInput" maxlength="3" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Tipo*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selTipoSerie" class="select">
                                        <option value="0">CT-e</option>
                                        <option value="1">MDF-e</option>
                                        <option value="2">NFS-e</option>
                                    </select>
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
                            <div class="field fieldquatro">
                                <div class="buttons">
                                    <input type="button" id="btnSalvar" value="Salvar" />
                                    <input type="button" id="btnCancelar" value="Cancelar" style="display: none;" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
