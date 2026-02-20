<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pedido.aspx.cs" Inherits="WebApplication1.Pedido" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Enviar Pedido
            </h2>
            Pedido:
            <br />
            <asp:TextBox runat="server" ID="txtCodigoPedido" Width="200px"></asp:TextBox><br />
            Carga:
            <br />
            <asp:TextBox runat="server" ID="txtCodigoCarga" Width="200px"></asp:TextBox><br />
            Codigo Carga Multi Embarcador Origem Cancelada:
            <br />
            <asp:TextBox runat="server" ID="txtCodigoCargaSubistituicao" Width="200px"></asp:TextBox><br />
            Cliente:
            <br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rdbSaoPauloPotenza" Text="(06128376000162) POTENZA SUPER MERCADOS - SÃO PAULO-SP" /><br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rdbSaoPaulo" Text="(31555584000195) SUPER MERCADOS SAO PAULO - SÃO PAULO-SP" /><br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rdbPontaGrossa" Text="(89073784000191) SUPER MERCADO PONTA GROSSA - PONTA GROSSA - PR" /><br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rdbCuiaba" Text="(71056183000105) SUPER MERCADO CUIABA - CUIABA - MT" /><br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rdbCuritiba" Text="(81378548000171) SUPER MERCADO CURITIBA - CURITIBA-PR" /><br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rdbJoinvile" Text="(66725387000151) SUPER MERCADO JOINVILLE - JOINVILLE-SC" /><br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rbdSaoGoncalo" Text="(22287551000110) SUPER MERCADOS SÃO GONÇALO - SÃO GONÇALO-RJ" /><br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rbdClienteTesteProsyst" Text="(00.297.455/0001-10) BAR E MERCEARIA - CAMPO LARGO-PR" /><br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rbdClienteTesteProsystErro" Text="(00.297.455/0001-10) BAR E MERCEARIA - CAMPO LARGO-PR COM ERRO" /><br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rdbToledo" Text="(21523052000112) UNIAO E ALIANCA SUPERMERCAD LTDA ME HEBROM SUPERMERCADO - TOLEDO-PR" /><br />
            <asp:RadioButton runat="server" GroupName="clientes" ID="rdbCascavel" Text="(75864728000160) SUPERMERCADOS IRANI LTDA - CASCAVEL-PR" /><br />
            <br />
            Produtos:<br />
            <br />
            <b>
                <asp:CheckBox ID="ckbQueijo" runat="server" Text="4587 - Queijo Prata" /></b><br />
            Quantidade:
            <asp:TextBox runat="server" ID="txtQuantidadeQueijo"></asp:TextBox>
            Peso Unit:
            <asp:TextBox runat="server" ID="txtPesoUnitQueijo"></asp:TextBox>
            <br />
            <b>
                <asp:CheckBox ID="ckbIogurte" runat="server" Text="1569 - Iogurte Morango" /></b><br />
            Quantidade:
            <asp:TextBox runat="server" ID="txtQuantidadeIogurte"></asp:TextBox>
            Peso Unit:
            <asp:TextBox runat="server" ID="txtPesoUnitIogurte"></asp:TextBox>
            <br />
            <b>
                <asp:CheckBox ID="ckbLeiteIntegral" runat="server" Text="2357 - Leite Integral" /></b><br />
            Quantidade:
            <asp:TextBox runat="server" ID="txtQuantidadeLeiteIntegral"></asp:TextBox>
            Peso Unit:
            <asp:TextBox runat="server" ID="txtPesoUnitLeiteIntegral"></asp:TextBox>
            <br />
            <b>
                <asp:CheckBox ID="ckbLeiteEmPo" runat="server" Text="555 - Leite em Pó" /></b><br />
            Quantidade:
            <asp:TextBox runat="server" ID="txtQuantidadeLeiteEmPo"></asp:TextBox>
            Peso Unit:
            <asp:TextBox runat="server" ID="txtPesoUnitLeiteEmPo"></asp:TextBox>
            <br />
            <b>
                <asp:CheckBox ID="ckbProdutoProsyst" runat="server" Text="2064 - BEBIDA LACTEA FERM.MORANGO TIROL 900G" /></b><br />
            Quantidade:
            <asp:TextBox runat="server" ID="txtQuantidadeProdutoProsyst"></asp:TextBox>
            Peso Unit:
            <asp:TextBox runat="server" ID="txtPesoUnitProdutoProsyst"></asp:TextBox>
            <br />
            <br />


            <asp:Button ID="btnEnviar" runat="server" Text="Enviar" OnClick="btnEnviar_Click" />

            <asp:Button ID="btnAtualizarPedido" runat="server" Text="Atualizar" OnClick="btnAtualizarPedido_Click" />


            <asp:Button ID="btnEnviarNovoWS" runat="server" Text="Enviar Novo WS" OnClick="btnEnviarNovoWS_Click"/>

            <asp:DropDownList runat="server" ID="ddlOperacao">
                <asp:ListItem Value="1">Venda Normal</asp:ListItem>
                <asp:ListItem Value="2">Venda Com Redespacho</asp:ListItem>
                <asp:ListItem Value="3">Engraga Armazem</asp:ListItem>
                <asp:ListItem Value="4">Venda Armazem Cliente</asp:ListItem>
            </asp:DropDownList>

            <br />
            <br />
            <h4>Retorno Envio do Pedido</h4>
            Protocolo:
            <asp:Label runat="server" ID="lblProtocolo"></asp:Label>
            <br />
            Carga:
            <asp:Label runat="server" ID="lblNumeroCarga"></asp:Label><br />
            Mensagem:
            <asp:Label runat="server" ID="lblMensagem"></asp:Label><br />
            Data:
            <asp:Label runat="server" ID="lblData"></asp:Label><br />

            <hr />
            <br />
            <hr />

            <h2>Refaturar Carga
            </h2>
            Número Protocolo Refatura:
            <br />
            <asp:TextBox runat="server" ID="txtProtocoloRefatura"></asp:TextBox><br />
            <br />
            Número Carga Refatura:
            <br />
            <asp:TextBox runat="server" ID="txtNumeroCargaRefatura"></asp:TextBox><br />
            <br />
            <asp:Button ID="btnRefaturar" runat="server" Text="buscar" OnClick="btnRefaturar_Click" />

            <hr />
            <br />
            <hr />

            <h2>Consultar Por Numero Protocolo
            </h2>
            Número Protocolo:
            <br />
            <asp:TextBox runat="server" ID="txtNumeroProtocoloPedido"></asp:TextBox><br />
            <br />
            <asp:Button ID="btnBuscarPedido" runat="server" Text="buscar" OnClick="btnBuscarPedido_Click" />
            <br />
            <h4>Retorno Consultar Por Numero Protocolo</h4>
            <asp:Label ID="lblRetornoPedido" runat="server"></asp:Label>

            <hr />
            <br />
            <hr />

            <h2>Consultar Por Carga Multi Embarcador
            </h2>
            Número Carga Multi Embarcador: 
            <br />
            <asp:TextBox runat="server" ID="txtNumeroCarga"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="btnNumeroCarga" runat="server" Text="buscar" OnClick="btnNumeroCarga_Click" />
            <br />
            <h4>Retorno Consultar Por Numero Protocolo</h4>
            <asp:Label ID="lblCarga" runat="server"></asp:Label>


            <hr />
            <br />
            <hr />

            <h2>Verificar Pedidos Ag Notas Fiscais
            </h2>
            <asp:Button ID="btnBuscarPendentes" OnClick="btnBuscarPendentes_Click" Text="Ag Notas Fiscais" runat="server" />
            <br />
            <h4>Retorno Pedidos Ag Notas Fiscais</h4>
            <asp:Label ID="lblPendentesNF" runat="server"></asp:Label>

            <hr />
            <br />
            <hr />

            <h2>Enviar Notas Fiscais
            </h2>

            Selecione o XML da Nota
            <br />
            <input type="file" runat="server" id="fleNF" /><br />
            <br />
            <asp:Button ID="btnEnviarNF" Text="enviarNF" OnClick="btnEnviarNF_Click" runat="server" />
            <br />
            <h4>Retorno Token Arquivos Enviados</h4>
            <asp:Label ID="lblNumeroNF" runat="server"></asp:Label>
            <br />
            <br />

            Numero Protocolo:<br />
            <asp:TextBox runat="server" ID="protocoloPedidoNF"></asp:TextBox>
            <br />
            Numero Carga: 
            <br />
            <asp:TextBox runat="server" ID="numeroCargaNF"></asp:TextBox>
            <br />
            GUID NF (separar por / ):<br />
            <asp:TextBox runat="server" ID="guidNFs" TextMode="MultiLine" Width="50%" Rows="5"></asp:TextBox>
            <br />
            <br />
            <asp:Button runat="server" ID="bntEnviarNFs" Text="enviar NFs" OnClick="bntEnviarNFs_Click" />
            <br />
            <h4>Retorno Notas Enviadas</h4>
            <asp:Label ID="lblRetornoEnvioNF" runat="server"></asp:Label>


            <hr />
            <br />
            <hr />


            <h2>Consultar Rateio 
            </h2>
            Número Protocolo:
            <br />
            <asp:TextBox runat="server" ID="txtProdutoParaRateio"></asp:TextBox><br />
            <br />
            <asp:Button ID="btnBuscarRateio" runat="server" Text="buscar" OnClick="btnBuscarRateio_Click" />
            <br />
            <h4>Retorno Rateio</h4>
            <div runat="server" id="divRetornoRateio"></div>

            <hr />
            <br />
            <hr />

        </div>
    </form>
</body>
</html>
