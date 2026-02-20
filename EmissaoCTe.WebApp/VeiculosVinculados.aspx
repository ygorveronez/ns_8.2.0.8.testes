<%@ Page Title="Veículos Vinculados" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="VeiculosVinculados.aspx.cs" Inherits="EmissaoCTe.WebApp.VeiculosVinculados" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtPlacaVeiculoPai").mask("*******", { placeholder: " " });
            $("#txtPlacaVeiculoVinculado").mask("*******", { placeholder: " " });
            CarregarConsultaDeVeiculos("btnBuscarVeiculoPai", "btnBuscarVeiculoPai", RetornoConsultaVeiculoPai, true, true, null);
            CarregarConsultaDeVeiculos("btnBuscarVeiculoVinculado", "btnBuscarVeiculoVinculado", RetornoConsultaVeiculoVinculado, true, true, 1);
            $("#btnSalvarVeiculoVinculado").click(function () {
                SalvarVeiculoVinculado();
            });
            $("#txtPlacaVeiculoPai").focusout(function () {
                BuscarVeiculoPai();
            });
            $("#txtPlacaVeiculoVinculado").focusout(function () {
                BuscarVeiculoVinculado();
            });
        });
        function BuscarVeiculoPai() {
            if ($("#txtPlacaVeiculoPai").val().trim() != "") {
                //executarRest("/Veiculo/BuscarPorPlacaSimples?callback=?&TipoVeiculo=0", { Placa: $("#txtPlacaVeiculoPai").val() }, function (r) {
                executarRest("/Veiculo/BuscarPorPlacaSimples?callback=?&", { Placa: $("#txtPlacaVeiculoPai").val() }, function (r) {
                    if (r.Sucesso) {
                        $("#hddCodigoVeiculoPai").val(r.Objeto.Codigo);
                        $("#hddTipoVeiculoPai").val(r.Objeto.DescricaoTipoVeiculo);
                        $("#txtPlacaVeiculoPai").val(r.Objeto.Placa);
                        $("#txtRenavamVeiculoPai").val(r.Objeto.Renavam);
                        $("#hddVeiculosVinculados").val(JSON.stringify(r.Objeto.VeiculosVinculados));
                        RenderizarVeiculosVinculados();
                    } else {
                        LimparCamposVeiculoPai();
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            } else {
                LimparCamposVeiculoPai();
            }
        }
        function BuscarVeiculoVinculado() {
            if ($("#txtPlacaVeiculoVinculado").val().trim() != "") {
                executarRest("/Veiculo/BuscarPorPlacaSimples?callback=?&TipoVeiculo=1", { Placa: $("#txtPlacaVeiculoVinculado").val() }, function (r) {
                    if (r.Sucesso) {
                        $("#hddCodigoVeiculoFilho").val(r.Objeto.Codigo);
                        $("#txtPlacaVeiculoVinculado").val(r.Objeto.Placa);
                        $("#txtRenavamVeiculoVinculado").val(r.Objeto.Renavam);
                    } else {
                        LimparCamposVeiculoVinculado();
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                LimparCamposVeiculoVinculado();
            }
        }
        function LimparCamposVeiculoPai() {
            $("#hddCodigoVeiculoPai").val('0');
            $("#hddTipoVeiculoPai").val('');
            $("#txtPlacaVeiculoPai").val('');
            $("#txtRenavamVeiculoPai").val('');
            $("#hddVeiculosVinculados").val('');
            RenderizarVeiculosVinculados();
            LimparCamposVeiculoVinculado();
        }
        function LimparCamposVeiculoVinculado() {
            $("#hddCodigoVeiculoFilho").val('0');
            $("#txtPlacaVeiculoVinculado").val('');
            $("#txtRenavamVeiculoVinculado").val('');
        }
        function SalvarVeiculoVinculado() {

            if ($("#hddTipoVeiculoPai").val() == "Reboque") {
                jConfirm("Realmente deseja vincular veículos do tipo Reboques?", "Atenção", function (r) {
                    if (r) Salvar();
                });
            }
            else {
                Salvar();
            }
        }

        function Salvar() {
            executarRest("/Veiculo/SalvarVeiculoVinculado?callback=?", { CodigoVeiculoPai: $("#hddCodigoVeiculoPai").val(), CodigoVeiculoFilho: $("#hddCodigoVeiculoFilho").val() }, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                    LimparCamposVeiculoVinculado();
                    BuscarVeiculosVinculados();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function ExcluirVeiculoVinculado(veiculo) {
            jConfirm("Deseja realmente excluir este vínculo?", "Atenção", function (r) {
                if (r) {
                    executarRest("/Veiculo/ExcluirVeiculoVinculado?callback=?", { CodigoVeiculoPai: $("#hddCodigoVeiculoPai").val(), CodigoVeiculoFilho: veiculo.Codigo }, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("Vinculo excluído com sucesso.", "Sucesso!");
                            BuscarVeiculosVinculados();
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }
        function RetornoConsultaVeiculoPai(veiculo) {
            $("#hddCodigoVeiculoPai").val(veiculo.Codigo);
            $("#hddTipoVeiculoPai").val(veiculo.DescricaoTipoVeiculo);
            $("#txtPlacaVeiculoPai").val(veiculo.Placa);
            $("#txtRenavamVeiculoPai").val(veiculo.Renavam);
            BuscarVeiculosVinculados();
        }
        function RetornoConsultaVeiculoVinculado(veiculo) {
            $("#hddCodigoVeiculoFilho").val(veiculo.Codigo);
            $("#txtPlacaVeiculoVinculado").val(veiculo.Placa);
            $("#txtRenavamVeiculoVinculado").val(veiculo.Renavam);
        }
        function BuscarVeiculosVinculados() {
            executarRest("/Veiculo/BuscarVeiculosVinculados?callback=?", { Codigo: $("#hddCodigoVeiculoPai").val() }, function (r) {
                if (r.Sucesso) {
                    $("#hddVeiculosVinculados").val(JSON.stringify(r.Objeto));
                    RenderizarVeiculosVinculados();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
        function RenderizarVeiculosVinculados() {
            $("#tblVeiculosVinculados tbody").html("");
            var veiculos = $("#hddVeiculosVinculados").val() == "" ? new Array() : JSON.parse($("#hddVeiculosVinculados").val());
            for (var i = 0; i < veiculos.length; i++) {
                $("#tblVeiculosVinculados tbody").append("<tr><td>" + veiculos[i].Placa + "</td><td>" + veiculos[i].Renavam + "</td><td>" + veiculos[i].DescricaoTipoVeiculo + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirVeiculoVinculado(" + JSON.stringify(veiculos[i]) + ");'>Excluir</button></td></tr>");
            }
            if ($("#tblVeiculosVinculados tbody").html() == "")
                $("#tblVeiculosVinculados tbody").html("<tr><td colspan=\"4\" class=\"text-center\">Nenhum veículo encontrado.</td></tr>");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigoVeiculoPai" value="0" />
        <input type="hidden" id="hddTipoVeiculoPai" value="0" />
        <input type="hidden" id="hddCodigoVeiculoFilho" value="0" />
        <input type="hidden" id="hddVeiculosVinculados" value="" />
    </div>
    <div class="page-header">
        <h2>Veículos Vinculados
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Placa*:
                </span>
                <input type="text" id="txtPlacaVeiculoPai" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">RENAVAM*:
                </span>
                <input type="text" id="txtRenavamVeiculoPai" class="form-control" disabled="disabled" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculoPai" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <h3 style="margin-top: 15px; margin-bottom: 10px;">Veículos Vinculados ao Veículo Selecionado
    </h3>
    <table id="tblVeiculosVinculados" class="table table-bordered table-condensed table-hover" style="max-width: 400px;">
        <thead>
            <tr>
                <th style="width: 25%">Placa</th>
                <th style="width: 25%">RENAVAM</th>
                <th style="width: 20%">Tipo</th>
                <th>Opções</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td colspan="4" class="text-center">Nenhum veículo encontrado.</td>
            </tr>
        </tbody>
    </table>
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Placa*:
                </span>
                <input type="text" id="txtPlacaVeiculoVinculado" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">RENAVAM*:
                </span>
                <input type="text" id="txtRenavamVeiculoVinculado" class="form-control" disabled="disabled" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculoVinculado" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <button type="button" id="btnSalvarVeiculoVinculado" class="btn btn-primary">Salvar Vínculo</button>
            </div>
        </div>
    </div>
</asp:Content>
