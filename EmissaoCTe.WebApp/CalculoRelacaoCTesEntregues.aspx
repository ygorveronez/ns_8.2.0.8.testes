<%@ Page Title="Configuração Valores Relação CT-es Entregues" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="CalculoRelacaoCTesEntregues.aspx.cs" Inherits="EmissaoCTe.WebApp.CalculoRelacaoCTesEntregues" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: System.Web.Optimization.Scripts.Render(
                           "~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/calculoRelacaoCTesEntregues") %>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server">
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Configuração Valores Relação CT-es Entregues</h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    
    <div class="row">
<%--        <div class="col-xs-12 col-sm-6">
            <div class="input-group">
                <span class="input-group-addon">Emissor/Transportador:
                </span>
                <input type="text" id="txtEmissor" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarEmissor" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>--%>
        <div class="col-xs-12 col-sm-6">
            <div class="input-group">
                <span class="input-group-addon">Cliente:
                </span>
                <input type="text" id="txtCliente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCliente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Valor Diária:
                </span>
                <input type="text" id="txtValorDiaria" class="form-control" />
            </div>
        </div>

        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Valor Meia Diária:
                </span>
                <input type="text" id="txtValorMeiaDiaria" class="form-control" />
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon">% por CTe:
                </span>
                <input type="text" id="txtPercentualPorCTe" class="form-control" />
            </div>
        </div>

        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon"><abbr title="Valor Mínimo por CT-e">Vl Mín CT-e</abbr>:
                </span>
                <input type="text" id="txtValorMinimoPorCTe" class="form-control" />
            </div>
        </div>

        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon"><abbr title="Valor Mínimo para CT-es com mesmo destino">Vl Mín CT-es mesmo dest.</abbr>:
                </span>
                <input type="text" id="txtValorMinimoCTeMesmoDestino" class="form-control" />
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon">Fração (Kg):
                </span>
                <input type="text" id="txtFracaoKG" class="form-control" />
            </div>
        </div>

        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon">Valor por Fração:
                </span>
                <input type="text" id="txtValorPorFracao" class="form-control" />
            </div>
        </div>

        <div class="col-xs-12 col-sm-4 hide">
            <div class="input-group">
                <span class="input-group-addon"><abbr title="Valor por fração para entregas iguais">Vl Fração CT-es entregas iguais</abbr>:
                </span>
                <input type="text" id="txtValorPorFracaoEmEntregasIguais" class="form-control" />
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon">Franquia KM:
                </span>
                <input type="text" id="txtFranquiaKM" class="form-control" />
            </div>
        </div>

        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon">Valor KM Excedente:
                </span>
                <input type="text" id="txtValorKMExcedente" class="form-control" />
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Coleta Valor por Evento:
                </span>
                <input type="text" id="txtColetaValorPorEvento" class="form-control" />
            </div>
        </div>

        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon">Coleta Fração (Kg):
                </span>
                <input type="text" id="txtColetaFracao" class="form-control" />
            </div>
        </div>

        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon">Coleta Valor por Fração:
                </span>
                <input type="text" id="txtColetaValorPorFracao" class="form-control" />
            </div>
        </div>
    </div>

    <br />
    <br />

    <div class="row">
        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon">% Por CT-e:
                </span>
                <input type="text" id="txtCidadePercentualPorCTe" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon">Estado:</span>
                <select id="selEstado" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Cidade:</span>
                <select id="selCidade" class="form-control">
                </select>
            </div>
        </div>
    </div>

    <div class="row" style="margin-bottom: 10px">
        <div class="col-xs-12 col-sm-6">
            <button type="button" id="btnAdicionarCidade" class="btn btn-primary">Adicionar</button>
        </div>
    </div>

    <table id="tblCidades" class="table table-bordered table-condensed table-hover">
        <thead>
            <tr>
                <th width="50%">Cidade</th>
                <th width="30%">Percentual</th>
                <th width="20%">Opção</th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
    
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
