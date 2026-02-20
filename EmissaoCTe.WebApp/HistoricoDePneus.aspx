<%@ Page Title="Cadastro de Históricos de Pneus" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="HistoricoDePneus.aspx.cs" Inherits="EmissaoCTe.WebApp.HistoricoDePneus" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtData").mask("99/99/9999");
            $("#txtData").datepicker();
            $("#txtCalibragemPneu").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#txtKM").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false);
            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $("#hddCodigoVeiculo").val("0");
                        $(this).val("");
                        $("#hddCodigoEixo").val("0");
                        $("#txtEixo").val("");
                        $("#hddCodigoPneu").val("0");
                        $("#txtPneu").val("");
                        $("#btnBuscarEixo").off();
                        CarregarConsultaDeEixosDeVeiculosPorHistorico("btnBuscarEixo", "btnBuscarEixo", $("#selTipo").val(), $("#hddCodigoVeiculo").val(), RetornoConsultaEixo, true, false);
                        $("#btnBuscarPneu").off();
                        CarregarConsultaDePneusPorTipoDeHistorico("btnBuscarPneu", "btnBuscarPneu", $(this).val(), $("#hddCodigoVeiculo").val(), RetornoConsultaPneu, true, false);
                        $("#txtKM").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtPneu").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoPneu").val("0");
                        if ($("#selTipo").val() == "S") {
                            $("#txtEixo").val("");
                            $("#hddCodigoEixo").val("0");
                        }
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtEixo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoEixo").val("0");
                        if ($("#selTipo").val() == "S") {
                            $("#hddCodigoPneu").val("0");
                            $("#txtPneu").val("");
                        }
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtStatusDoPneu").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoStatusPneu").val("0");
                        $("#hddCodigoStatusTipo").val("");                        
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#selTipo").change(function () {
                $("#hddCodigoPneu").val("0");
                $("#txtPneu").val("");
                $("#hddCodigoEixo").val("0");
                $("#txtEixo").val("");
                $("#hddCodigoStatusPneu").val('0');
                $("#hddCodigoStatusTipo").val('');                
                $("#txtStatusDoPneu").val('');
                $("#btnBuscarPneu").off();
                $("#btnBuscarStatusDoPneu").off();
                $("#btnBuscarEixo").off();
                CarregarConsultaDePneusPorTipoDeHistorico("btnBuscarPneu", "btnBuscarPneu", $(this).val(), $("#hddCodigoVeiculo").val(), RetornoConsultaPneu, true, false);
                CarregarConsultaDeEixosDeVeiculosPorHistorico("btnBuscarEixo", "btnBuscarEixo", $("#selTipo").val(), $("#hddCodigoVeiculo").val(), RetornoConsultaEixo, true, false);
                var tipo = "";
                var codigoVeiculo = Globalize.parseInt($("#hddCodigoVeiculo").val());
                if (codigoVeiculo > 0) {
                    switch ($("#selTipo").val()) {
                        case "S":
                            tipo = "A";
                            break;
                        case "E":
                            tipo = "E";
                            break;
                        default:
                            tipo = "";
                            return;
                    }
                }
                CarregarConsultaDeStatusDePneus("btnBuscarStatusDoPneu", "btnBuscarStatusDoPneu", "A", tipo, RetornoConsultaStatusPneu, true, false);
                BuscarUltimoStatusPneu();
            });
            LimparCampos();
            CarregarConsultaDeHistoricosDePneus("default-search", "default-search", RetornoConsultaHistoricoPneu, true, false);
        });
        function RetornoConsultaHistoricoPneu(historico) {
            executarRest("/HistoricoDePneu/ObterDetalhes?callback=?", { Codigo: historico.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#hddCodigoPneu").val(r.Objeto.CodigoPneu);
                    $("#txtPneu").val(r.Objeto.DescricaoPneu);
                    $("#hddCodigoVeiculo").val(r.Objeto.CodigoVeiculo);
                    $("#txtVeiculo").val(r.Objeto.DescricaoVeiculo);
                    $("#hddCodigoEixo").val(r.Objeto.CodigoEixo);
                    $("#txtEixo").val(r.Objeto.DescricaoEixo);
                    $("#selTipo").val(r.Objeto.Tipo);
                    $("#txtData").val(r.Objeto.Data);
                    $("#txtKM").val(r.Objeto.Kilometragem);
                    $("#txtCalibragemPneu").val(r.Objeto.Calibragem);
                    $("#txtObservacao").val(r.Objeto.Observacao);
                    $("#btnBuscarPneu").off();
                    $("#btnBuscarStatusDoPneu").off();
                    $("#btnBuscarEixo").off();
                    CarregarConsultaDePneusPorTipoDeHistorico("btnBuscarPneu", "btnBuscarPneu", $("#selTipo").val(), $("#hddCodigoVeiculo").val(), RetornoConsultaPneu, true, false);
                    CarregarConsultaDeEixosDeVeiculosPorHistorico("btnBuscarEixo", "btnBuscarEixo", $("#selTipo").val(), $("#hddCodigoVeiculo").val(), RetornoConsultaEixo, true, false);
                    var tipo = "";
                    var codigoVeiculo = Globalize.parseInt($("#hddCodigoVeiculo").val());
                    if (codigoVeiculo > 0) {
                        switch ($("#selTipo").val()) {
                            case "S":
                                tipo = "A";
                                break;
                            case "E":
                                tipo = "E";
                                break;
                            default:
                                tipo = "";
                                return;
                        }
                    }
                    CarregarConsultaDeStatusDePneus("btnBuscarStatusDoPneu", "btnBuscarStatusDoPneu", "A", tipo, RetornoConsultaStatusPneu, true, false);
                    $("#txtStatusDoPneu").val('');
                    $("#hddCodigoStatusPneu").val('0');
                    $("#hddCodigoStatusTipo").val('');
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RetornoConsultaStatusPneu(status) {
            if (status.Codigo == undefined) status = status.data;
            $("#txtStatusDoPneu").val(status.Descricao);
            $("#hddCodigoStatusPneu").val(status.Codigo);
            $("#hddCodigoStatusTipo").val(status.Tipo);            
        }
        function BuscarUltimoStatusPneu() {
            var codigoVeiculo = Globalize.parseInt($("#hddCodigoVeiculo").val());
            if (codigoVeiculo > 0) {
                var tipo = "";
                switch ($("#selTipo").val()) {
                    case "S":
                        tipo = "A";
                        break;
                    case "E":
                        tipo = "E";
                        break;
                    default:
                        ExibirMensagemErro("O tipo do histórico é inválido para consulta.", "Atenção");
                        return;
                }
                executarRest("/StatusDePneu/ObterUltimoRegistroPorTipo?callback=?", { Tipo: tipo }, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto != null) {
                            $("#hddCodigoStatusPneu").val(r.Objeto.Codigo);
                            $("#txtStatusDoPneu").val(r.Objeto.Descricao);
                            $("#hddCodigoStatusTipo").val(r.Objeto.Tipo);                            
                        }
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            }
        }
        function RetornoConsultaPneu(pneu) {
            $(".modal").modal("hide");
            if (pneu.Codigo == undefined) pneu = pneu.data;
            $("#hddCodigoPneu").val(pneu.Codigo);
            $("#txtPneu").val(pneu.Serie + " - " + pneu.ModeloPneu);
            if ($("#selTipo").val() == "S") {
                executarRest("/Pneu/ObterEixo?callback=?", { Codigo: pneu.Codigo, CodigoVeiculo: $("#hddCodigoVeiculo").val() }, function (r) {
                    if (r.Sucesso) {
                        $("#txtEixo").val(r.Objeto.Descricao);
                        $("#hddCodigoEixo").val(r.Objeto.Codigo);
                    } else {
                        $("#txtEixo").val("");
                        $("#hddCodigoEixo").val("0");
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            }
        }
        function RetornoConsultaVeiculo(veiculo) {
            $(".modal").modal("hide");
            $("#hddCodigoVeiculo").val(veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
            $("#hddCodigoPneu").val("0");
            $("#txtPneu").val("");
            $("#hddCodigoEixo").val("0");
            $("#txtEixo").val("");
            $("#btnBuscarEixo").off();
            $("#btnBuscarPneu").off();
            CarregarConsultaDeEixosDeVeiculosPorHistorico("btnBuscarEixo", "btnBuscarEixo", $("#selTipo").val(), $("#hddCodigoVeiculo").val(), RetornoConsultaEixo, true, false);
            CarregarConsultaDePneusPorTipoDeHistorico("btnBuscarPneu", "btnBuscarPneu", $("#selTipo").val(), $("#hddCodigoVeiculo").val(), RetornoConsultaPneu, true, false);
            executarRest("/Veiculo/BuscarKilometragemPorPlaca?callback=?", { Placa: veiculo.Placa }, function (r) {
                if (r.Sucesso) {
                    $("#txtKM").val(Globalize.format(r.Objeto.KilometragemAtual, "n0"));
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RetornoConsultaEixo(eixo) {
            $(".modal").modal("hide");
            if (eixo.Codigo == undefined) eixo = eixo.data;
            $("#hddCodigoEixo").val(eixo.Codigo);
            $("#txtEixo").val(eixo.Descricao);
            if ($("#selTipo").val() == "S") {
                executarRest("/EixoDeVeiculo/ObterPneu?callback=?", { Codigo: eixo.Codigo, CodigoVeiculo: $("#hddCodigoVeiculo").val() }, function (r) {
                    if (r.Sucesso) {
                        $("#hddCodigoPneu").val(r.Objeto.Codigo);
                        $("#txtPneu").val(r.Objeto.Serie + " - " + r.Objeto.ModeloPneu);
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                        $("#hddCodigoPneu").val("0");
                        $("#txtPneu").val("");
                    }
                });
            }
        }
        function LimparCampos() {
            $("#hddCodigo").val("0");
            $("#hddCodigoPneu").val("0");
            $("#txtPneu").val("");
            $("#hddCodigoVeiculo").val("0");
            $("#txtVeiculo").val("");
            $("#hddCodigoEixo").val("0");
            $("#txtEixo").val("");
            $("#selTipo").val($("#selTipo option:first").val());
            $("#txtData").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtKM").val('0');
            $("#txtCalibragemPneu").val('0');
            $("#txtObservacao").val('');
            $("#txtStatusDoPneu").val('');
            $("#hddCodigoStatusPneu").val('0');
            $("#hddCodigoStatusTipo").val('');
            $("#btnBuscarEixo").off();
            $("#btnBuscarPneu").off();
            CarregarConsultaDePneusPorTipoDeHistorico("btnBuscarPneu", "btnBuscarPneu", "E", 0, RetornoConsultaPneu, true, false);
            CarregarConsultaDeEixosDeVeiculosPorHistorico("btnBuscarEixo", "btnBuscarEixo", "E", 0, RetornoConsultaEixo, true, false);
            CarregarConsultaDeStatusDePneus("btnBuscarStatusDoPneu", "btnBuscarStatusDoPneu", "A", "", RetornoConsultaStatusPneu, true, false);
            BuscarUltimoStatusPneu();
        }
        function ValidarCampos() {
            var codigoPneu = Globalize.parseInt($("#hddCodigoPneu").val());
            var codigoVeiculo = Globalize.parseInt($("#hddCodigoVeiculo").val());
            var codigoEixo = Globalize.parseInt($("#hddCodigoEixo").val());
            var codigoStatusPneu = Globalize.parseInt($("#hddCodigoStatusPneu").val());
            var codigoStatusTipo = $("#hddCodigoStatusTipo").val();
            var valido = true;

            if (isNaN(codigoPneu)) codigoPneu = 0;
            if (isNaN(codigoVeiculo)) codigoVeiculo = 0;
            if (isNaN(codigoEixo)) codigoEixo = 0;
            if (isNaN(codigoStatusPneu)) codigoStatusPneu = 0;

            if (codigoPneu > 0) {
                CampoSemErro("#txtPneu");
            } else {
                CampoComErro("#txtPneu");
                valido = false;
            }
            if (codigoStatusPneu > 0) {
                CampoSemErro("#txtStatusDoPneu");
            } else {
                CampoComErro("#txtStatusDoPneu");
                valido = false;
            }

            if (codigoStatusTipo != "A") {
                if (codigoEixo > 0) {
                    CampoSemErro("#txtEixo");
                } else {
                    CampoComErro("#txtEixo");
                    valido = false;
                }
                if (codigoVeiculo > 0) {
                    CampoSemErro("#txtVeiculo");
                } else {
                    CampoComErro("#txtVeiculo");
                    valido = false;
                }
            }

            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    CodigoPneu: $("#hddCodigoPneu").val(),
                    CodigoVeiculo: $("#hddCodigoVeiculo").val(),
                    CodigoEixo: $("#hddCodigoEixo").val(),
                    CodigoStatusPneu: $("#hddCodigoStatusPneu").val(),
                    Tipo: $("#selTipo").val(),
                    Data: $("#txtData").val(),
                    KM: $("#txtKM").val(),
                    Calibragem: $("#txtCalibragemPneu").val(),
                    Observacao: $("#txtObservacao").val()
                };
                executarRest("/HistoricoDePneu/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoPneu" value="0" />
        <input type="hidden" id="hddCodigoVeiculo" value="0" />
        <input type="hidden" id="hddCodigoEixo" value="0" />
        <input type="hidden" id="hddCodigoStatusPneu" value="0" />
        <input type="hidden" id="hddCodigoStatusTipo" value="" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Históricos de Pneus
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data*:
                </span>
                <input type="text" id="txtData" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Veículo:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo*:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="E">Entrada</option>
                    <option value="S">Saída</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Pneu*:
                </span>
                <input type="text" id="txtPneu" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPneu" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Status Pneu*:
                </span>
                <input type="text" id="txtStatusDoPneu" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarStatusDoPneu" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Eixo:
                </span>
                <input type="text" id="txtEixo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarEixo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Km:
                </span>
                <input type="text" id="txtKM" class="form-control" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Calibragem Pneu:
                </span>
                <input type="text" id="txtCalibragemPneu" class="form-control" value="0" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Observação:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
