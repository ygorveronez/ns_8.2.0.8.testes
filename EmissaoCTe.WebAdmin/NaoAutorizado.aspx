<%@ Page Title="Acesso Não Autorizado" Language="C#" MasterPageFile="Site.Master"
    AutoEventWireup="true" CodeBehind="NaoAutorizado.aspx.cs" Inherits="EmissaoCTe.WebAdmin.NaoAutorizado" %>

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
                                    <h2>
                                        Acesso Não Autorizado!
                                    </h2>
                                    <h3>
                                        A permissão para acesso à página solicitada foi negada. Contate o administrador do sistema para maiores informações. 
                                    </h3>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
