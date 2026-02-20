using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WebApplication1
{
    public partial class Carga : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfSituacaoCargaTRAZTvlX retornoCarga = wsCarga.SolicitarCancelamentoDaCarga(int.Parse(this.TxtNumeroCarga.Text), this.txtNotaCancelamento.Text, this.txtNotaCancelamento.Text);

            if (retornoCarga.Status)
            {
                this.lblSituacaoCarga.Text = retornoCarga.Objeto.ToString();
            }
            else
            {
                this.lblMensagem.Text = retornoCarga.Mensagem;
            }
        }

        protected void btnConsultarCarga_Click(object sender, EventArgs e)
        {
            
            
            string userToken = "bla";
            
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();

            Servicos.InspectorBehavior inspector = new Servicos.InspectorBehavior();
            wsCarga.Endpoint.EndpointBehaviors.Add(inspector);
            OperationContextScope scope = new OperationContextScope(wsCarga.InnerChannel);
            MessageHeader header = MessageHeader.CreateHeader("Token", "TokenAutorizacao", userToken);
            OperationContext.Current.OutgoingMessageHeaders.Add(header);
            
            ServiceReference6.RetornoOfRetornoCarga4PBTRYa8 retornoCarga = wsCarga.BuscarPorCarga(int.Parse(this.txtNumeroCargaConsulta.Text));

            if (retornoCarga.Status)
            {


                this.lblSituacaoConsulta.Text = retornoCarga.Objeto.SituacaoCarga.ToString();
                string doc = "";

                if (retornoCarga.Objeto.CTEs != null)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CTe cte in retornoCarga.Objeto.CTEs)
                    {
                        doc += " CTe: " + cte.NumeroProtocolo;
                    }
                    if (retornoCarga.Objeto.MDFEs != null)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Carga.MDFe mdfe in retornoCarga.Objeto.MDFEs)
                        {
                            doc += " MDFe: " + mdfe.NumeroProtocolo;
                        }
                    }

                }

                lblDocumentos.Text = doc;
                if (!string.IsNullOrWhiteSpace(retornoCarga.Objeto.MensagemRejeicao))
                {
                    this.lblRejeicao.Text = retornoCarga.Objeto.MensagemRejeicao.ToString();
                }

            }
            else
            {
                this.lblMensagem.Text = retornoCarga.Mensagem;
            }
        }

        protected void btnBaixarCTE_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfRetornoCTe4PBTRYa8 retornoCarga = wsCarga.BuscarCTe(int.Parse(this.txtNumeroProtocoloCTe.Text), Dominio.Enumeradores.TipoRetornoIntegracao.PDF);

            if (retornoCarga.Status)
            {
                this.retornoDownload.Text = "Status: " + retornoCarga.Objeto.StatusCTe + " Mensagem:" + retornoCarga.Objeto.MensagemRetorno;

                if (retornoCarga.Objeto.PDF != "")
                {
                    string fileLocation = System.IO.Path.Combine(ConfigurationManager.AppSettings["CaminhoTempArquivosImportacao"], retornoCarga.Objeto.ChaveCTe + ".pdf");
                    System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    byte[] decodedData = encoding.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, encoding, Convert.FromBase64String(retornoCarga.Objeto.PDF))).ToArray();
                    System.IO.File.WriteAllBytes(fileLocation, decodedData);
                }
            }
            else
            {
                this.retornoDownload.Text = "falha";
            }
        }

        protected void btnBaixarMDF_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfRetornoMDFe4PBTRYa8 retornoCarga = wsCarga.BuscarMDFe(int.Parse(this.txtNumeroProtocoloMDFe.Text), Dominio.Enumeradores.TipoRetornoIntegracao.Todos);

            if (retornoCarga.Status)
            {
                if (retornoCarga.Objeto.PDF != null)
                {
                    string fileLocation = System.IO.Path.Combine(ConfigurationManager.AppSettings["CaminhoTempArquivosImportacao"], retornoCarga.Objeto.ChaveMDFe + ".pdf");
                    System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    byte[] decodedData = encoding.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, encoding, Convert.FromBase64String(retornoCarga.Objeto.PDF))).ToArray();

                    System.IO.File.WriteAllBytes(fileLocation, decodedData);

                }

                this.retornoDownload.Text = "Status: " + retornoCarga.Objeto.StatusMDFe + " Mensagem:" + retornoCarga.Objeto.MensagemRetorno;

            }
            else
            {
                this.retornoDownload.Text = "falha";
            }
        }

        protected void btnBuscarAgImpressao_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfRetornoCargaPorSituacao4PBTRYa8 retornoCarga = wsCarga.BuscarCargasPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos, 0, 50);

            if (retornoCarga.Status)
            {

                string codigo = "";
                foreach (int cd in retornoCarga.Objeto.CodigosDasCargas)
                {
                    codigo += " Carga : " + cd;
                }
                this.lblListaCargas.Text = codigo;

            }
            else
            {
                this.lblListaCargas.Text = "falha";
            }
        }

        protected void btnBuscarRateio_Click(object sender, EventArgs e)
        {
            int codigoCarga = int.Parse(this.txtCodigoParaRateio.Text);
            ServiceReference6.CargasClient WSCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfArrayOfRetornoRateioProduto4PBTRYa8 retorno = WSCarga.BuscarRateioPorCarga(codigoCarga);
            divRetornoRateio.InnerHtml = "";
            if (retorno.Status == true)
            {
                foreach (ServiceReference6.RetornoRateioProduto rateio in retorno.Objeto)
                {
                    divRetornoRateio.InnerHtml += "Pedido :" + rateio.ProtocoloPedido.ToString() + " <br/> Produto: " +
                     rateio.CodigoProdutoEmbarcador.ToString() + " <br/> Valor Frete:" + rateio.ValorTotalRateado.ToString("n2") + "<br/> Valor Unitário: " +
                     rateio.ValorUnitarioRateado + "<br/> Valor Unitário Produto: " + rateio.ValorUnitarioProduto + "<br/> Pedágio:" + rateio.ValorPedagio +
                     "<br/> Valor ICMS:" + rateio.ValorICMS + "<br/> Valor ICMS ST: " + rateio.ValorICMSST + " <br/> Peso:" + rateio.Peso + " <br/>" +
                     "Valor ADValorem Rateado: " + rateio.ValorADValoremRateado + "<br/> Valor Complemento de Frete Rateado :" + rateio.ValorComplementoFreteRateado +
                     "<br/> Valor Complemento ICMS Rateado: " + rateio.ValorComplementoICMSRateado + "<br/> Valor Descarga Rateado :" + rateio.ValorDescargaRateado +
                     "<br/> Valor Outros Rateado: " + rateio.ValorOutrosRateado + "<br/> Valor Pedagio Rateado :" + rateio.ValorPedagioRateado +
                     "<br/> Protocolo CTe: " + rateio.NumeroProtocoloCTe + "<br/> Protocolo NFs:" + rateio.NumeroProtocoloNFS +
                     "<br/><hr/>";

                }
            }
            else
            {
                divRetornoRateio.InnerHtml = retorno.Mensagem;
            }

        }



        protected void btnbuscarRateioCTe_Click(object sender, EventArgs e)
        {
            int numeroProtocolo = int.Parse(this.txtNumeroProtocoloCTeRateio.Text);
            ServiceReference6.CargasClient WSCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfArrayOfRetornoRateioProduto4PBTRYa8 retorno = WSCarga.BuscarRateioPorCTe(numeroProtocolo);
            divRateioProtocoloCTe.InnerHtml = "";
            if (retorno.Status == true)
            {
                foreach (ServiceReference6.RetornoRateioProduto rateio in retorno.Objeto)
                {
                    divRateioProtocoloCTe.InnerHtml += "Carga :" + rateio.CodigoCargaMultiEmbarcador.ToString() + " <br/> Produto: " +
                        rateio.CodigoProdutoEmbarcador.ToString() + " <br/> Valor Frete:" + rateio.ValorTotalRateado.ToString("n2") + "<br/> Valor Unitário: " +
                        rateio.ValorUnitarioRateado + "<br/> Valor Unitário Produto: " + rateio.ValorUnitarioProduto + "<br/> Pedágio:" + rateio.ValorPedagio +
                        "<br/> Valor ICMS:" + rateio.ValorICMS + "<br/> Valor ICMS ST: " + rateio.ValorICMSST + " <br/> Peso:" + rateio.Peso + " <br/>" +
                        "Valor ADValorem Rateado: " + rateio.ValorADValoremRateado + "<br/> Valor Complemento de Frete Rateado :" + rateio.ValorComplementoFreteRateado +
                        "<br/> Valor Complemento ICMS Rateado: " + rateio.ValorComplementoICMSRateado + "<br/> Valor Descarga Rateado :" + rateio.ValorDescargaRateado +
                        "<br/> Valor Outros Rateado: " + rateio.ValorOutrosRateado + "<br/> Valor Pedagio Rateado :" + rateio.ValorPedagioRateado +
                        "<br/> Protocolo CTe: " + rateio.NumeroProtocoloCTe + "<br/> Protocolo NFs:" + rateio.NumeroProtocoloNFS;
                        
                    foreach (ServiceReference6.RetornoComponenteFrete componenteFrete in rateio.ComponentesRateio)
                    {
                        divRateioProtocoloCTe.InnerHtml += "<hr/><br/>COMPONENTES";
                        if (componenteFrete.ComponenteFrete != null)
                        {
                            divRateioProtocoloCTe.InnerHtml += "<br/> Componente:" + componenteFrete.ComponenteFrete.Descricao + "<br/> Codigo Embarcador: " + componenteFrete.ComponenteFrete.CodigoEmbarcador;
                        }
                        divRateioProtocoloCTe.InnerHtml += "<br/> Tipo Componente:" + componenteFrete.TipoComponenteFrete.ToString() + "<br/> Valor: " + componenteFrete.ValorComponente;

                    }
                    divRateioProtocoloCTe.InnerHtml += "<br/><hr/>";
                }
            }
            else
            {
                divRateioProtocoloCTe.InnerHtml = retorno.Mensagem;
            }
        }





        protected void btnConsultarPagamentoLiberado_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfRetornoCargaPorSituacao4PBTRYa8 retornoCarga = wsCarga.BuscarCargasPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento, 0, 50);

            if (retornoCarga.Status)
            {

                string codigo = "";
                foreach (int cd in retornoCarga.Objeto.CodigosDasCargas)
                {
                    codigo += " Carga : " + cd;
                }
                this.lblPagamentoLiberado.Text = codigo;

            }
            else
            {
                this.lblPagamentoLiberado.Text = retornoCarga.Mensagem;
            }
        }


        protected void btnFinalizarCarga_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfboolean retornoCarga = wsCarga.EncerrarCargaMultiEmbarcador(int.Parse(txtNumeroCargaFinalizar.Text), "");

            if (retornoCarga.Status)
            {

                this.lblRetornoFinalizacao.Text = "Finalizado com sucesso";

            }
            else
            {
                this.lblRetornoFinalizacao.Text = retornoCarga.Mensagem;
            }
        }

        protected void btnAtualzarCarga_Click(object sender, EventArgs e)
        {

            Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoCarga integracao = new Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoCarga();

            integracao.CNPJTransportador = txtCNPJEmpresa.Text;
            integracao.CodigoCarga = int.Parse(txtNumeroCargaAlterar.Text);
            integracao.CodigoTipoCargaEmbarcador = txtTipoCarga.Text;
            integracao.CodigoTipoVeiculoEmbarcador = txtModeloVeicular.Text;
            integracao.PlacaVeiculo = txtPlacaVeiculo.Text;

            integracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
            integracao.Motoristas.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista() { CPF = txtCPFMOtorista.Text, Nome = txtNomeMotorista.Text });
            integracao.Redespacho = false;

            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();

            ServiceReference6.RetornoOfRetornoAlteracaoCarga4PBTRYa8 retornoCarga = wsCarga.AlterarCarga(integracao);

            if (retornoCarga.Status)
            {
                this.lblRetornoAlteracao.Text = "Finalizado com sucesso" + " Situacao Carga : " + retornoCarga.Objeto.SituacaoCarga.ToString() + " Mensagem:" + retornoCarga.Objeto.MensagemPendeciaCalculoFrete;

            }
            else
            {
                this.lblRetornoAlteracao.Text = retornoCarga.Mensagem;
            }
        }

        protected void btnAgConfirmacaoCancelamento_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfRetornoCargaPorSituacao4PBTRYa8 retornoCarga = wsCarga.BuscarCargasAguardandoConfirmacaoCancelamento(0, 50);

            if (retornoCarga.Status)
            {

                string codigo = "";
                foreach (int cd in retornoCarga.Objeto.CodigosDasCargas)
                {
                    codigo += " Carga : " + cd;
                }
                this.lblRetornoAgConfirmacaoCancelamento.Text = codigo;

            }
            else
            {
                this.lblRetornoAgConfirmacaoCancelamento.Text = "falha";
            }
        }

        protected void btnConfirmarCancelamento_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfboolean retornoCarga = wsCarga.ConfirmarCancelamentoDaCarga(int.Parse(txtCodigoCargaConfirmacaoCancelamento.Text), "");

            if (retornoCarga.Status)
            {

                this.lblRetornoConfirmacaoCancelamento.Text = "Confirmado com sucesso";

            }
            else
            {
                this.lblRetornoConfirmacaoCancelamento.Text = retornoCarga.Mensagem;
            }
        }

        protected void btnVerDetalhesCancelamento_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfRetornoDetalhesCancelamento4PBTRYa8 retornoCarga = wsCarga.BuscarDetalhesCancelamento(int.Parse(txtCodigoCargaDetalhesCancelamento.Text));

            if (retornoCarga.Status)
            {

                this.lblRetornoDetalhesCancelamento.Text = "Motivo: :" + retornoCarga.Objeto.MotivoCancelamento + "   Data:" + retornoCarga.Objeto.DataCancelamento + " Situacao:" + retornoCarga.Objeto.SituacaoCancelamentoCarga;

            }
            else
            {
                this.lblRetornoDetalhesCancelamento.Text = retornoCarga.Mensagem;
            }
        }

        protected void btnCargaFrete_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfRetornoDetalhesFrete4PBTRYa8 retornoCarga = wsCarga.BuscarDetalhesFretePorCarga(int.Parse(txtCargaFrete.Text));

            if (retornoCarga.Status)
            {

                this.lblCargaFrete.Text = "Codigo Frete: :" + retornoCarga.Objeto.CodigoFreteEmbarcador;

            }
            else
            {
                this.lblCargaFrete.Text = retornoCarga.Mensagem;
            }
        }

        protected void btnCargasProblemaEmissao_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfRetornoCargaPorSituacao4PBTRYa8 retornoCarga = wsCarga.BuscarCargasComProblemaEmissao(0, 50);

            if (retornoCarga.Status)
            {

                string codigo = "";
                foreach (int cd in retornoCarga.Objeto.CodigosDasCargas)
                {
                    ServiceReference6.RetornoOfRetornoCarga4PBTRYa8 retornoCargaID = wsCarga.BuscarPorCarga(cd);

                    codigo += " Carga : " + cd + " Mensagem:" + retornoCargaID.Objeto.MotivoPendenciaEmissao;
                }
                this.lblRetornoRetornoProblemaEmissao.Text = codigo;

            }
            else
            {
                this.lblRetornoRetornoProblemaEmissao.Text = "falha";
            }
        }

        protected void btnCodigoCargaFechar_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfboolean retornoCarga = wsCarga.FecharCarga(int.Parse(txtCodigoCargaFechar.Text));

            if (retornoCarga.Status)
            {

                this.lblRetornoCargaFechar.Text = "Sucesso!";

            }
            else
            {
                this.lblRetornoCargaFechar.Text = retornoCarga.Mensagem;
            }
        }

        protected void btnBuscarNFe_Click(object sender, EventArgs e)
        {
            WSNFe.NFeClient wsNFe = new WSNFe.NFeClient();
            WSNFe.RetornoOfArrayOfDadosNotaFiscalknUzgH84 retorno = wsNFe.BuscarNotasPorCTe(int.Parse(txtProtocoloCTeNFe.Text));

            if (retorno.Status)
            {
                this.retornoDownload.Text = "";
                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.DadosNotaFiscal dadosNF in retorno.Objeto)
                {
                    this.retornoDownload.Text += " Chave: " + dadosNF.Chave;
                }
            }
            else
            {
                this.lblRetornoCargaFechar.Text = retorno.Mensagem;
            }
        }

        protected void btnConfirmarImpressao_Click(object sender, EventArgs e)
        {
            ServiceReference6.CargasClient wsCarga = new ServiceReference6.CargasClient();
            ServiceReference6.RetornoOfboolean retornoCarga = wsCarga.ConfirmarImpressaoDocumentos(int.Parse(txtCodigoImpressao.Text));

            if (retornoCarga.Status)
            {

                this.lblConfirmarImpresssao.Text = "Confirmado com sucesso";

            }
            else
            {
                this.lblConfirmarImpresssao.Text = retornoCarga.Mensagem;
            }
        }
    }
}