<%@ Page Title="Acesso Não Autorizado" Language="C#" MasterPageFile="Site.Master"
    AutoEventWireup="true" CodeBehind="AvisoEmpresaBloqueioFinanceiro.aspx.cs" Inherits="EmissaoCTe.WebApp.AvisoEmpresaBloqueioFinanceiro" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .unauth-box
        {
            height: 100px;
            background: #FFF url('Images/lock-icon.png') no-repeat left top;
            padding-left: 130px;
            padding-top: 25px;
            padding-bottom: 0;
        }
        .fields .fields-title h2
        {
            font-size: 2.4em;
            padding: 0 0 8px 8px;
            font-family: "Segoe UI" , "Frutiger" , "Tahoma" , "Helvetica" , "Helvetica Neue" , "Arial" , "sans-serif";
            color: #000;
            letter-spacing: -1px;
            font-weight: bold;
        }
        .fields .fields-title h3
        {
            font-size: 1.8em;
            padding: 10px 0 8px 8px;
            font-family: "Segoe UI" , "Frutiger" , "Tahoma" , "Helvetica" , "Helvetica Neue" , "Arial" , "sans-serif";
            color: #000;
            letter-spacing: -1px;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="content-box">
                <div class="form">
                    <div class="fields">
                        <div class="fieldzao">
                            <div class="unauth-box">
                                <div class="fields-title">
                                    <h2>Sistema Bloqueado</h2>
                                    <p>
                                        Notamos que há pendências financeiras em sua conta. 
                                        Para garantir que você possa continuar utilizando nosso sistema, pedimos que regularize sua situação.
                                        <a href="http://wa.me/554933055124" target="_blank" class="contact-link">Clique aqui</a> para falar com nosso Setor Financeiro.
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
