<%@ Page Title="Relatório Duplicatas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioDuplicatas.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioDuplicatas" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
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
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtDataLctoInicial").mask("99/99/9999");
            $("#txtDataLctoFinal").mask("99/99/9999");

            $("#txtDataLctoFinal").datepicker();
            $("#txtDataLctoInicial").datepicker();

            $("#txtDataVctoInicial").mask("99/99/9999");
            $("#txtDataVctoFinal").mask("99/99/9999");

            $("#txtDataVctoFinal").datepicker();
            $("#txtDataVctoInicial").datepicker();

            $("#txtPessoa").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("pessoa", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtMotorista").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("motorista", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtVeiculo1").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("veiculo1", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtVeiculo2").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("veiculo2", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtVeiculo3").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("veiculo3", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });

            var dataInicio = new Date();
            var dataFim = new Date();
            dataInicio.setDate(dataFim.getDate() - 30);

            $("#txtDataLctoInicial").val(Globalize.format(dataInicio, "dd/MM/yyyy"));
            $("#txtDataLctoFinal").val(Globalize.format(dataFim, "dd/MM/yyyy"));

            CarregarConsultaDeVeiculos("btnBuscarVeiculo1", "btnBuscarVeiculo1", RetornoConsultaVeiculos1, true, false);
            CarregarConsultaDeVeiculos("btnBuscarVeiculo2", "btnBuscarVeiculo2", RetornoConsultaVeiculos2, true, false);
            CarregarConsultaDeVeiculos("btnBuscarVeiculo3", "btnBuscarVeiculo3", RetornoConsultaVeiculos3, true, false);
            CarregarConsultadeClientes("btnBuscarPessoa", "btnBuscarPessoa", RetornoConsultaPessoa, true, false, "");
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, true, false);
        });

        function DownloadRelatorio() {
            var dados = {
                Veiculo1: $("body").data("veiculo1"),
                Veiculo2: $("body").data("veiculo2"),
                Veiculo3: $("body").data("veiculo3"),
                Pessoa: $("body").data("pessoa"),
                Motorista: $("body").data("motorista"),
                DataLctoInicial: $("#txtDataLctoInicial").val(),
                DataLctoFinal: $("#txtDataLctoFinal").val(),
                DataVctoInicial: $("#txtDataVctoInicial").val(),
                DataVctoFinal: $("#txtDataVctoFinal").val(),
                StatusPgto: $("#selStatusPgto").val(),
                Tipo: $("#selTipo").val(),
                Agrupamento: $("#selAgrupamento").val(),
                TipoArquivo: $("#selTipoArquivo").val(),
                CTesSemDuplicata: $("#chkCTesSemDuplicata")[0].checked,
                Ordenacao: $("#selOrdenação").val(),
                RaizCNPJ: $("#chkRaizCNPJ")[0].checked,
                Status: $("#selStatus").val()
            };

            executarDownload("/RelatorioDuplicatas/DownloadRelatorio", dados);
        }

        function RetornoConsultaVeiculos1(veiculo) {
            $("body").data("veiculo1", veiculo.Codigo);
            $("#txtVeiculo1").val(veiculo.Placa);
        }

        function RetornoConsultaVeiculos2(veiculo) {
            $("body").data("veiculo2", veiculo.Codigo);
            $("#txtVeiculo2").val(veiculo.Placa);
        }

        function RetornoConsultaVeiculos3(veiculo) {
            $("body").data("veiculo3", veiculo.Codigo);
            $("#txtVeiculo3").val(veiculo.Placa);
        }

        function RetornoConsultaPessoa(pessoa) {
            $("body").data("pessoa", pessoa.CPFCNPJ);
            $("#txtPessoa").val(pessoa.CPFCNPJ + " - " + pessoa.Nome);
        }

        function RetornoConsultaMotorista(usuario) {
            $("body").data("motorista", usuario.Codigo);
            $("#txtMotorista").val(usuario.CPFCNPJ + " - " + usuario.Nome);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Duplicatas
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Lcto. Inicial:
                </span>
                <input type="text" id="txtDataLctoInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Lcto. Final:
                </span>
                <input type="text" id="txtDataLctoFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Vcto. Inicial:
                </span>
                <input type="text" id="txtDataVctoInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Vcto.Final:
                </span>
                <input type="text" id="txtDataVctoFinal" class="form-control maskedInput" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Pessoa:
                </span>
                <input type="text" id="txtPessoa" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPessoa" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <div class="checkbox">
                    <input type="checkbox" id="chkRaizCNPJ" />Filtrar pela Raiz do CNPJ
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Motorista:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Veículo 1:
                </span>
                <input type="text" id="txtVeiculo1" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo1" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Veículo 2:
                </span>
                <input type="text" id="txtVeiculo2" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo2" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Veículo3:
                </span>
                <input type="text" id="txtVeiculo3" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo3" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="0">A Receber</option>
                    <option value="1">A Pagar</option>
                    <option value="">Todos</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status Pgto.:
                </span>
                <select id="selStatusPgto" class="form-control">
                    <option value="0">Pendentes</option>
                    <option value="1">Quitados</option>
                    <option value="">Todos</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Agrupamento:
                </span>
                <select id="selAgrupamento" class="form-control">
                    <option value="Duplicata">Por Duplicata</option>
                    <option value="Parcela">Por Parcela</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Ordenação:
                </span>
                <select id="selOrdenação" class="form-control">
                    <option value="0">Por Documento</option>
                    <option value="1">Por Remetente</option>
                    <option value="2">Por Destinatário</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Arquivo:
                </span>
                <select id="selTipoArquivo" class="form-control">
                    <option value="PDF">PDF</option>
                    <option value="Excel">Excel</option>
                    <option value="Image">Imagem</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="A" selected="selected">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <div class="checkbox">
                    <input type="checkbox" id="chkCTesSemDuplicata" />Apenas CT-es pendentes geração duplicatas
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarRelatorio" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>

