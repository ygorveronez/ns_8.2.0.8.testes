<%@ Page Title="SPED Contribuições" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SPEDContribuicoes.aspx.cs" Inherits="EmissaoCTe.WebApp.SPEDContribuicoes" %>

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
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");

            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();

            $("#btnGerarSPED").click(function () {
                GerarSPED();
            });

            SetarDadosPadrao();

            $("#selTipoEscrituracao").change(function () {
                AlterarTipoEscrituracao();
            });

            AlterarTipoEscrituracao();
        });

        function AlterarTipoEscrituracao() {
            if ($("#selTipoEscrituracao").val() == "1") {
                $("#divNumeroRecibo").removeClass("hidden");
            } else {
                $("#divNumeroRecibo").addClass("hidden");
            }

            $("#txtNumeroReciboAnterior").val("");
        }

        function GerarSPED() {
            executarDownload("/SPEDContribuicoes/GerarSPED", {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                AtoCOTEPE: $("#selAtoCOTEPE").val(),
                TipoEscrituracao: $("#selTipoEscrituracao").val(),
                NumeroReciboAnterior: $("#txtNumeroReciboAnterior").val(),
                IndicadorSituacaoEspecial: $("#selIndicadorSituacaoEspecial").val(),
                IndicadorApropriacaoCreditos: $("#selIndicadorApropriacaoCreditos").val(),
                IndicadorTipoContribuicao: $("#selIndicadorTipoContribuicao").val()
            });
        }

        function SetarDadosPadrao() {
            var date = new Date();
            $("#txtDataInicial").val(Globalize.format(new Date(date.getFullYear(), date.getMonth() - 1, 1), "dd/MM/yyyy"));
            $("#txtDataFinal").val(Globalize.format(new Date(date.getFullYear(), date.getMonth(), 0), "dd/MM/yyyy"));
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>SPED Contribuições
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial*:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final*:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Versão*:
                </span>
                <select id="selAtoCOTEPE" class="form-control">
                    <option value="002">Versão 1.01 - de 01/01/2011</option>
                    <option value="003">Versão 1.01 - de 01/07/2012</option>                    
                    <option value="004">Versão 1.27 - de 05/07/2018</option>
                    <option value="005">Versão 1.30 - de 28/02/2019</option>
                    <option value="006">Versão 1.33 - de 16/12/2019</option>
                    <option value="006" selected="selected">Versão 1.35 - de 18/06/2021</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Escrituração*:
                </span>
                <select id="selTipoEscrituracao" class="form-control">
                    <option value="0" selected="selected">Normal</option>
                    <option value="1">Retificadora</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6" id="divNumeroRecibo">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Número do Rebibo da Escrituração Anterior a ser Retificada">Nº Rec. Ant.</abbr>*:
                </span>
                <input type="text" id="txtNumeroReciboAnterior" class="form-control" maxlength="41" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Situação*:
                </span>
                <select id="selIndicadorSituacaoEspecial" class="form-control">
                    <option value="0" selected="selected">Abertura</option>
                    <option value="1">Cisão</option>
                    <option value="2">Fusão</option>
                    <option value="3">Incorporação</option>
                    <option value="4">Encerramento</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Indicador de Método de Apropriação de Créditos Comuns no Caso de Incidência no Regime Não-Cumulativo">Mét. Apropr.</abbr>*:
                </span>
                <select id="selIndicadorApropriacaoCreditos" class="form-control">
                    <option value="1" selected="selected">Método de Apropriação Direta</option>
                    <option value="2">Método de Rateio Proporcional (Receita Bruta)</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Indicador do Tipo de Contribuição Apurada no Período">Tp. Contr.</abbr>*:
                </span>
                <select id="selIndicadorTipoContribuicao" class="form-control">
                    <option value="1" selected="selected">Apuração da Contribuição Exclusivamente a Alíquota Básica</option>
                    <option value="2">Apuração da Contribuição a Aliquotas Específicas (Diferenciadas e/ou por Unidade de Medida do Produto)</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarSPED" class="btn btn-primary" style="margin-top: 10px;">Gerar SPED</button>
</asp:Content>
