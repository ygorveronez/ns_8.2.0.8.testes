<%@ Page Title="Baixa de Parcelas Duplicatas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="BaixaDeParcelasDuplicatas.aspx.cs" Inherits="EmissaoCTe.WebApp.BaixaDeParcelasDuplicatas" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<style type="text/css">
        div.tfs-tags {
            margin-bottom: 6px;
            line-height: 25px;
            margin-left: 5px;
        }

        div.tags-label-container {
            float: left;
            padding-left: 4px;
            padding-right: 10px;
            font-size: 11px;
            color: #000;
        }

        div.tags-items-container {
            overflow: hidden;
        }

        .tags-items-container ul {
            list-style-type: none;
            margin: 0;
            padding: 0;
            display: block;
            -webkit-margin-before: 1em;
            -webkit-margin-after: 1em;
            -webkit-margin-start: 0;
            -webkit-margin-end: 0px;
            -webkit-padding-start: 0;
        }

            .tags-items-container ul > li {
                display: inline-block;
                margin-right: 5px;
                padding: 0;
                text-align: -webkit-match-parent;
            }

        .tag-item-delete-experience {
            white-space: nowrap;
            overflow: hidden;
        }

        .tag-container-delete-experience {
            cursor: pointer;
        }

        .tag-container {
            outline: none;
            padding-top: 2px;
            padding-bottom: 2px;
            border: 1px solid #fff !important;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
        }


        .tag-box, .tag-delete {
            cursor: default;
            margin: 0;
            padding-left: 6px;
            padding-top: 2px;
            padding-right: 6px;
            padding-bottom: 2px;
            font-size: 12px;
            color: #4f4f4f;
            background-color: #d7e6f3;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
            font-family: Segoe UI,Tahoma,Arial,Verdana;
            border-radius: 2px 0 0 2px;
        }

        .tag-delete {
            padding-left: 9px;
            padding-right: 9px;
            background: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJAgMAAACd/+6DAAAACVBMVEUAAABlZWVlZWXtPovbAAAAAnRSTlMAnxYjQ+0AAAAlSURBVHheLcgxDQAACMTAsrwH3GDj/SGUELikS1mpQoboS773BjdcAscFjXmNAAAAAElFTkSuQmCC') /*images/icon-close-small.png*/ no-repeat 50% 50%;
            background-color: #d7e6f3;
            border-radius: 0 2px 2px 0;
        }

            .tag-delete:focus, .tag-delete:hover {
                cursor: pointer;
                color: #fff;
                background-color: #b4c8d7;
            }
    </style>


    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker") %>
        <%: Scripts.Render("~/bundle/scripts/json",
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
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/baixadeparcelasduplicatas") %>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigoPessoa" value="" />
        <input type="hidden" id="hddParcelas" value="" />
        <input type="hidden" id="hddStatus" value="0" />
        <input type="hidden" id="hddPlanoDeConta" value="" />
    </div>
    <div class="page-header">
        <h2>Baixa de Parcelas de Duplicatas
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Documento:
                </span>
                <input type="text" id="txtDocumento" class="form-control" maxlength="12" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">CT-e:
                </span>
                <input type="text" id="txtCTe" class="form-control" maxlength="12" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Vencimento Inicial">Vcto. Inicial</abbr>:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Vencimento Final">Vcto. Final</abbr>:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Pessoa:
                </span>
                <input type="text" id="txtPessoa" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPessoa" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo*:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="0">A Receber</option>
                    <option value="1">A Pagar</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridParcelas" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <button type="button" id="btnSelecionarTodasParcelas" class="btn btn-default pull-right">Selecionar Todas</button>
    <div id="tbl_parcelas" class="table-responsive" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_parcelas">
    </div>
    <div class="clearfix"></div>
    <h3 style="margin-bottom: 10px;">Parcelas Selecionadas
    </h3>
    <div class="clearfix"></div>
    <span id="lblSemParcelass">Nenhuma parcela selecionada.</span>
    <div class="tfs-tags">
        <div class="tags-items-container">
            <ul id="containerParcelasSelecionadas">
            </ul>
        </div>
    </div>
    <div class="clearfix"></div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Plano de Conta*:
                </span>
                <input type="text" id="txtPlanoDeConta" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPlanoDeConta" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row" style="margin-top: 10px;">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Obs. Baixa:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Baixar Parcelas</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
