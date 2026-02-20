<%@ Page Title="Cadastro de Coletas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Coletas.aspx.cs" Inherits="EmissaoCTe.WebApp.Coletas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datetimepicker") %>
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/coletas") %>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Coletas
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Número para controle interno">Número</abbr>:
                </span>
                <input type="text" id="txtNumero" class="form-control" maxlength="100" />
            </div>
        </div>               
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data inicial da coleta">Dt. Inicial</abbr>*:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data final da coleta">Dt. Final</abbr>*:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data prevista da entrega">Dt. Entrega</abbr>*:
                </span>
                <input type="text" id="txtDataEntrega" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">CPF/CNPJ Rem.:
                </span>
                <input type="text" id="txtCPFCNPJRemetente" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">Nome Rem.:
                </span>
                <input type="text" id="txtRemetente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">CPF/CNPJ Dest.:
                </span>
                <input type="text" id="txtCPFCNPJDestinatario" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">Nome Dest.:
                </span>
                <input type="text" id="txtDestinatario" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarDestinatario" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">CPF/CNPJ Toma.:
                </span>
                <input type="text" id="txtCPFCNPJTomador" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">Nome Toma.:
                </span>
                <input type="text" id="txtTomador" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTomador" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Localidade de início da prestação">Origem</abbr>*:
                </span>
                <input type="text" id="txtOrigem" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarOrigem" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Localidade de término da prestação">Destino</abbr>*:
                </span>
                <input type="text" id="txtDestino" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarDestino" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Tipo de coleta">Tp. Coleta</abbr>:
                </span>
                <input type="text" id="txtTipoColeta" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoColeta" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Tipo da carga transportada">Tp. Carga</abbr>:
                </span>
                <input type="text" id="txtTipoCarga" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoCarga" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Requisitante*:
                </span>
                <select id="selRequisitante" class="form-control">
                    <option value="0">Remetente</option>
                    <option value="1">Destinatário</option>
                    <option value="2">Outros</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tp. Pagamento*:
                </span>
                <select id="selTipoPagamento" class="form-control">
                    <option value="0">Pago</option>
                    <option value="1">A pagar</option>
                    <option value="2">Outros</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Peso (Kg):
                </span>
                <input type="text" id="txtPeso" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Vl. NFs:
                </span>
                <input type="text" id="txtValorNFs" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Código do pedido do cliente">Ped. Cliente</abbr>:
                </span>
                <input type="text" id="txtCodigoPedidoCliente" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Situação*:
                </span>
                <select id="selSituacao" class="form-control">
                    <option value="1">Aberta</option>
                    <option value="2">Cancelada</option>
                    <option value="3">Finalizada</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Vl. Frete:
                </span>
                <input type="text" id="txtValorFrete" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Número NF:
                </span>
                <input type="text" id="txtNumeroNotaCliente" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-md-offset-2">
            <div class="input-group">
                <span class="input-group-addon">Qt. Volumes:
                </span>
                <input type="text" id="txtQtVolumes" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Obs. Coleta:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3" maxlength="2000"></textarea>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Obs. CT-e:
                </span>
                <textarea id="txtObservacaoCTe" class="form-control" rows="3" maxlength="2000"></textarea>
            </div>
        </div>
    </div>
    <div class="panel panel-default" style="margin-top: 20px;">
        <div class="panel-heading">
            Veículos
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                    <div class="input-group">
                        <span class="input-group-addon">Placa*:
                        </span>
                        <input type="text" id="txtPlacaVeiculo" class="form-control maskedInput" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                    <div class="input-group">
                        <button type="button" id="btnAdicionarVeiculo" class="btn btn-primary">Salvar</button>
                    </div>
                </div>
            </div>
            <div class="table-responsive" style="margin-top: 10px;">
                <table id="tblVeiculos" class="table table-bordered table-hover table-condensed">
                    <thead>
                        <tr>
                            <th>Placa
                            </th>
                            <th>UF
                            </th>
                            <th>Renavam
                            </th>
                            <th>Tipo
                            </th>
                            <th>Rodado
                            </th>
                            <th>Carroc.
                            </th>
                            <th>Tara(kg)
                            </th>
                            <th>Cap.(kg)
                            </th>
                            <th>Cap.(m³)
                            </th>
                            <th>Opções
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="10">Nenhum registro encontrado.
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <div class="panel panel-default">
        <div class="panel-heading">
            Motoristas
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">CPF*:
                        </span>
                        <input type="text" id="txtCPFMotorista" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Nome*:
                        </span>
                        <input type="text" id="txtNomeMotorista" class="form-control" maxlength="60" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                    <div class="input-group">
                        <button type="button" id="btnSalvarMotorista" class="btn btn-primary">Salvar</button>
                    </div>
                </div>
            </div>
            <div class="table-responsive" style="margin-top: 10px; max-height: 400px; overflow-y: scroll;">
                <table id="tblMotoristas" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 20%;">CPF
                            </th>
                            <th style="width: 65%;">Nome
                            </th>
                            <th style="width: 15%;">Opções
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="3">Nenhum registro encontrado!
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnDownloadEspelho" class="btn btn-primary" style="display: none;"><span class="glyphicon glyphicon-download"></span>&nbsp;Visualizar Coleta</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
