<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Carga.aspx.cs" Inherits="WebApplication1.Carga" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <h2>Fechar Carga
            </h2>
            Codigo Carga:
            <br />
            <asp:TextBox runat="server" ID="txtCodigoCargaFechar"></asp:TextBox><br />
            <br />
            <asp:Button ID="btnCodigoCargaFechar" runat="server" Text="Fechar Carga" OnClick="btnCodigoCargaFechar_Click" />
            <br />
            <h4>Retorno Frete</h4>
            <asp:Label runat="server" ID="lblRetornoCargaFechar"></asp:Label>
            <br />

            <hr />
            <br />
            <hr />

            <h4>Solicitar Cancelamento Carga</h4>
            Número da Carga:
            <asp:TextBox runat="server" ID="TxtNumeroCarga"></asp:TextBox><br />
            Nota Cancelamento :
            <asp:TextBox runat="server" ID="txtNotaCancelamento"></asp:TextBox><br />
            Usuario ERP:
            <asp:TextBox runat="server" ID="txtUsuarioERP"></asp:TextBox><br />
            <br />
            <br />

            <asp:Button ID="btnCancelar" runat="server" Text="Solicitar Cancelamento" OnClick="btnCancelar_Click" />

            <br />
            <br />
            <h4>Retorno solicitacao Cancelamento</h4>
            Situação Carga:
            <asp:Label runat="server" ID="lblSituacaoCarga"></asp:Label>
            <br />
            Mensagem:
            <asp:Label runat="server" ID="lblMensagem"></asp:Label><br />

            <hr />
            <br />
            <hr />

            <h4>Verificar Carga</h4>
            Número da Carga:
            <asp:TextBox runat="server" ID="txtNumeroCargaConsulta"></asp:TextBox><br />
            <br />
            <br />

            <asp:Button ID="btnConsultarCarga" runat="server" Text="Consultar" OnClick="btnConsultarCarga_Click" />

            <br />
            <br />
            <h4>Retorno</h4>
            Situação Carga:
            <asp:Label runat="server" ID="lblSituacaoConsulta"></asp:Label>
            <br />
            Documentos Carga:
            <asp:Label runat="server" ID="lblDocumentos"></asp:Label>
            <br />
            Rejeição Carga:
            <asp:Label runat="server" ID="lblRejeicao"></asp:Label>
            <br />
            Mensagem:
            <asp:Label runat="server" ID="lblMensagemConsulta"></asp:Label><br />

            <hr />
            <br />
            <hr />

            <h4>buscar Arquivos</h4>
            Numero Protocolo CTe:
            <asp:TextBox runat="server" ID="txtNumeroProtocoloCTe"></asp:TextBox><br />
            <br />
            <br />

            <asp:Button ID="btnBaixarCTE" runat="server" Text="Baixar CTE" OnClick="btnBaixarCTE_Click" />

            <br />
            <br />


            <h4>buscar Dados NFes do CT-e</h4>
            Numero Protocolo CTe:
            <asp:TextBox runat="server" ID="txtProtocoloCTeNFe"></asp:TextBox><br />
            <br />
            <br />

            <asp:Button ID="btnBuscarNFe" runat="server" Text="Buscar NFe" OnClick="btnBuscarNFe_Click" />

            <br />
            <br />

            Numero Protocolo MDFe:
            <asp:TextBox runat="server" ID="txtNumeroProtocoloMDFe"></asp:TextBox><br />
            <br />
            <br />

            <asp:Button ID="btnBaixarMDF" runat="server" Text="Baixar MDFe" OnClick="btnBaixarMDF_Click" />

            <br />
            <br />

            <h4>Retorno</h4>
            Retorno:
            <asp:Label runat="server" ID="retornoDownload"></asp:Label>
            <br />

            <hr />
            <br />
            <hr />


            <h4>Buscar todas ag. Impressão</h4>
            <asp:Button ID="btnBuscarAgImpressao" runat="server" Text="Verificar AG Impressao" OnClick="btnBuscarAgImpressao_Click" />

            <h4>Retorno</h4>
            Retorno:
            <asp:Label runat="server" ID="lblListaCargas"></asp:Label>
            <br />

            <hr />
            <br />
            <hr />


            <h2>Consultar Rateio 
            </h2>
            Codigo Carga:
            <br />
            <asp:TextBox runat="server" ID="txtCodigoParaRateio"></asp:TextBox><br />
            <br />
            <asp:Button ID="btnBuscarRateio" runat="server" Text="buscar" OnClick="btnBuscarRateio_Click" />
            <br />
            <h4>Retorno Rateio</h4>
            <div runat="server" id="divRetornoRateio"></div>

            <hr />
            <br />
            <hr />

            
            <h2>Consultar Rateio Por CTe
            </h2>
            Número Protocolo CTe:
            <br />
            <asp:TextBox runat="server" ID="txtNumeroProtocoloCTeRateio"></asp:TextBox><br />
            <br />
            <asp:Button ID="btnbuscarRateioCTe" runat="server" Text="buscar" OnClick="btnbuscarRateioCTe_Click" />
            <br />
            <h4>Retorno Rateio</h4>
            <div runat="server" id="divRateioProtocoloCTe"></div>

            <hr />
            <br />
            <hr />


            <h4>Buscar todas Pagamento Liberado</h4>
            <asp:Button ID="btnConsultarPagamentoLiberado" runat="server" Text="Verificar pagamento liberado" OnClick="btnConsultarPagamentoLiberado_Click" />

            <h4>Retorno</h4>
            Retorno:
            <asp:Label runat="server" ID="lblPagamentoLiberado"></asp:Label>
            <br />

            <hr />
            <br />
            <hr />


            <h4>Finalizar Carga</h4>
            Número da Carga:
            <asp:TextBox runat="server" ID="txtNumeroCargaFinalizar"></asp:TextBox><br />
            <br />
            <br />

            <asp:Button ID="btnFinalizarCarga" runat="server" Text="Finalizar" OnClick="btnFinalizarCarga_Click" />

            <br />
            <br />
            <h4>Retorno finalização</h4>
            <asp:Label runat="server" ID="lblRetornoFinalizacao"></asp:Label>
            <br />

            <hr />
            <br />
            <hr />


            <h4>Confimar Impressão Carga</h4>
            Número da Carga:
            <asp:TextBox runat="server" ID="txtCodigoImpressao"></asp:TextBox><br />
            <br />
            <br />

            <asp:Button ID="btnConfirmarImpressao" runat="server" Text="Finalizar" OnClick="btnConfirmarImpressao_Click" />

            <br />
            <br />
            <h4>Retorno confirmação Impressão</h4>
            <asp:Label runat="server" ID="lblConfirmarImpresssao"></asp:Label>
            <br />

            <hr />
            <br />
            <hr />


            <h4>Alterar Carga</h4>
            Número da Carga:
            <asp:TextBox runat="server" ID="txtNumeroCargaAlterar"></asp:TextBox><br />
            <br />
            <br />
            Modelo Veicular:
            <asp:TextBox runat="server" ID="txtModeloVeicular"></asp:TextBox><br />
            <br />
            <br />
            Tipo Carga:
            <asp:TextBox runat="server" ID="txtTipoCarga"></asp:TextBox><br />
            <br />
            <br />
            CNPJ Empresa:
            <asp:TextBox runat="server" ID="txtCNPJEmpresa"></asp:TextBox><br />
            <br />
            <br />
            Placa Veículo:
            <asp:TextBox runat="server" ID="txtPlacaVeiculo"></asp:TextBox><br />
            <br />
            <br />
            Nome Motorista:
            <asp:TextBox runat="server" ID="txtNomeMotorista"></asp:TextBox>
            CPF Motorista:
            <asp:TextBox runat="server" ID="txtCPFMOtorista"></asp:TextBox><br />
            <br />

            <asp:Button ID="btnAtualzarCarga" runat="server" Text="Finalizar" OnClick="btnAtualzarCarga_Click" />

            <br />
            <br />
            <h4>Retorno Alteracao</h4>
            <asp:Label runat="server" ID="lblRetornoAlteracao"></asp:Label>
            <br />


            <hr />
            <br />
            <hr />

            <h4>Buscar cargas ag. Confirmacao Cancelamento</h4>
            <asp:Button ID="btnAgConfirmacaoCancelamento" runat="server" Text="Verificar AG confirmacao Cancelamento" OnClick="btnAgConfirmacaoCancelamento_Click" />

            <h4>Retorno</h4>
            Retorno:
            <asp:Label runat="server" ID="lblRetornoAgConfirmacaoCancelamento"></asp:Label>
            <br />



            <hr />
            <br />
            <hr />


            <h4>Confirmar Cancelamento Carga</h4>
            Número da Carga:
            <asp:TextBox runat="server" ID="txtCodigoCargaConfirmacaoCancelamento"></asp:TextBox><br />
            <br />
            <br />

            <asp:Button ID="btnConfirmarCancelamento" runat="server" Text="Finalizar" OnClick="btnConfirmarCancelamento_Click" />

            <br />
            <br />
            <h4>Retorno finalização</h4>
            <asp:Label runat="server" ID="lblRetornoConfirmacaoCancelamento"></asp:Label>
            <br />

            <hr />
            <br />
            <hr />


            <h4>Ver detalhes Cancelamento Carga</h4>
            Número da Carga:
            <asp:TextBox runat="server" ID="txtCodigoCargaDetalhesCancelamento"></asp:TextBox><br />
            <br />
            <br />

            <asp:Button ID="btnVerDetalhesCancelamento" runat="server" Text="Finalizar" OnClick="btnVerDetalhesCancelamento_Click" />

            <br />
            <br />
            <h4>Detalhes Cancelamento</h4>
            <asp:Label runat="server" ID="lblRetornoDetalhesCancelamento"></asp:Label>
            <br />

            <hr />
            <br />
            <hr />


            <h2>Consultar Frete Carga 
            </h2>
            Codigo Carga:
            <br />
            <asp:TextBox runat="server" ID="txtCargaFrete"></asp:TextBox><br />
            <br />
            <asp:Button ID="btnCargaFrete" runat="server" Text="buscar" OnClick="btnCargaFrete_Click" />
            <br />
            <h4>Retorno Frete</h4>
            <asp:Label runat="server" ID="lblCargaFrete"></asp:Label>

            <br />


            <hr />
            <br />
            <hr />

            <h4>Buscar cargas Problema Emissão</h4>
            <asp:Button ID="btnCargasProblemaEmissao" runat="server" Text="Verificar Problema Emissão" OnClick="btnCargasProblemaEmissao_Click" />

            <h4>Retorno</h4>
            <asp:Label runat="server" ID="lblRetornoRetornoProblemaEmissao"></asp:Label>
            <br />



        </div>
    </form>
</body>
</html>
