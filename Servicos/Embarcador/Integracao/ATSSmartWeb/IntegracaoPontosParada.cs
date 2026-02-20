using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.ATSSmartWeb
{
    public partial class IntegracaoATSSmartWeb
    {
        #region Metodos Publicos

        public bool IntegrarPontoParada(ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            bool sucesso = false;

            try
            {
                object request = this.obterPontosParada(cargaIntegracao.Carga);
                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("GestaoViagemAtualizarPontosIntegracao/Integrar", request);

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    sucesso = true;
                else
                    throw new ServicoException(retWS.ProblemaIntegracao);

            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaIntegracao.ProblemaIntegracao = message;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.ProblemaIntegracao = "Erro ao tentar integrar Pontos de parada com a ATS Smart Web";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração de Pontos de parada");

            repCargaIntegracao.Atualizar(cargaIntegracao);

            return sucesso;
        }

        public bool IntegrarPontoParada(ref Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, bool controleColeta = false)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            bool sucesso = false;

            try
            {
                object request = this.obterPontosParada(cargaDadosTransporteIntegracao.Carga, controleColeta);
                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("GestaoViagemAtualizarPontosIntegracao/Integrar", request);

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    sucesso = true;
                else
                    throw new ServicoException(retWS.ProblemaIntegracao);

            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaDadosTransporteIntegracao.ProblemaIntegracao = message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Erro ao tentar integrar pontos de parada com a ATS Smart Web";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            
            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração de Pontos de parada");

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

            return sucesso;
        }
        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontosParadaViagem obterPontosParada(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool controleColeta = false)
        {
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido repPontoPassagemPreDefinido = new Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontosParadaViagem retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontosParadaViagem();
            Dominio.Entidades.Veiculo veiculo = carga.Veiculo;
            Dominio.Entidades.Usuario motorista = null;

            if (veiculo == null)
                veiculo = carga.VeiculosVinculados.FirstOrDefault();

            if (veiculo == null)
                throw new ServicoException(@"Veículo não definido na carga");

            motorista = carga.Motoristas != null && carga.Motoristas.Count > 0 ? carga.Motoristas.FirstOrDefault() : null;

            if (motorista == null)
                motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

            if (motorista == null)
                throw new ServicoException(@"Motorista não definido na carga");


            retorno.CodigoExterno = carga.Codigo.ToString(); 
            retorno.Tipo = 2;
            retorno.PontosControle = obterPontosControlePontosParada(carga, controleColeta);
            retorno.DataHoraPrevisaoInicioViagem = carga.DataInicioViagem ?? carga.DataInicioViagemReprogramada ?? carga.DataInicioViagemPrevista ?? carga.DataCarregamentoCarga ?? carga.DataCriacaoCarga;

            DateTime? dataUltimaEntrega = retorno.PontosControle.Max(o => o.DataHoraPrevisaoFim);
            DateTime? dataFimViagem = carga.DataFimViagemPrevista ?? dataUltimaEntrega;

            if (dataFimViagem < dataUltimaEntrega)
                dataFimViagem = dataUltimaEntrega;

            retorno.DataHoraPrevisaoFimViagem = dataFimViagem;

            retorno.ValorTotalCarga = carga.DadosSumarizados?.ValorTotalProdutos ?? 0;
            retorno.VinculoCondutor = motorista.TipoMotorista == TipoMotorista.Proprio ? 1 : 3;
            retorno.VinculoVeiculoTracao = veiculo.Tipo == "P" ? 1 : 3;
            retorno.CodigoExternoOperacao = carga.TipoOperacao?.Descricao.ToString() ?? "";


            return retorno;
        }
        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper> obterPontosControlePontosParada(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool controleColeta = false)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioTMS.BuscarPrimeiroRegistro();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper> dadosPontosControle = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repositorioCargaEntrega.BuscarPorCarga(carga.Codigo);

            if (controleColeta)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault();
                DateTime DataHoraPrevisaoFim = cargaEntregas.FirstOrDefault()?.DataPrevista.Value ?? DateTime.Now;

                if (cargaPedido != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper();

                    Dominio.Entidades.Cliente cliente = cargaPedido.Pedido?.Expedidor ?? cargaPedido.Pedido?.Remetente;
                    ponto.PontoControle = this.obterPontoControle(cliente);

                    int minutosPadrao = configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao;
                    ponto.DataHoraPrevisaoInicio = (carga.DataInicioViagem ?? carga.DataInicioViagemReprogramada ?? carga.DataInicioViagemPrevista ?? cargaPedido.Pedido.DataCarregamentoPedido);
                    ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio?.AddMinutes(minutosPadrao);
                    ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem;

                    dadosPontosControle.Add(ponto);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in carga.Pedidos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper();

                    Dominio.Entidades.Cliente cliente = pedido.Pedido?.Recebedor ?? pedido.Pedido?.Destinatario;
                    ponto.PontoControle = this.obterPontoControle(cliente);

                    int minutosPadrao = configuracaoTMS.TempoPadraoDeEntregaParaCalcularPrevisao;

                    ponto.DataHoraPrevisaoInicio = pedido.Pedido.PrevisaoEntrega;
                    ponto.DataHoraPrevisaoFim = pedido.Pedido.PrevisaoEntrega?.AddMinutes(minutosPadrao);

                    //Se for a primeira Coleta, manda como Origem.
                    if (dadosPontosControle.Count == 0)
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem;

                    //Se for a ultima entrega, manda como Destino.
                    else if (pedido == carga.Pedidos.ToList()[carga.Pedidos.Count() - 1])
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Destino;

                    //Entrega.
                    else
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Entrega;

                    dadosPontosControle.Add(ponto);
                }
            }
            else
            {

                if (cargaEntregas.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault();
                    DateTime DataHoraPrevisaoFim = cargaEntregas.FirstOrDefault()?.DataPrevista.Value ?? DateTime.Now;

                    if (!cargaEntregas.Exists(ce => ce.Coleta))
                    {
                        if (cargaPedido == null)
                            throw new ServicoException("Carga sem Coleta e sem Pedido para integrar ponto de controle.");

                        Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper();

                        Dominio.Entidades.Cliente cliente = cargaPedido.Pedido?.Expedidor ?? cargaPedido.Pedido?.Remetente;
                        ponto.PontoControle = this.obterPontoControle(cliente);
                        int minutosPadrao = configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao;

                        ponto.DataHoraPrevisaoInicio = (carga.DataInicioViagem ?? carga.DataInicioViagemReprogramada ?? carga.DataInicioViagemPrevista ?? cargaPedido.Pedido.DataCarregamentoPedido);
                        ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio?.AddMinutes(minutosPadrao);
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem;
                        ponto.Produtos = null;
                        ponto.DescricaoVisita = null;
                        ponto.Visita = false;

                        dadosPontosControle.Add(ponto);
                    }


                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleWrapper();

                        Dominio.Entidades.Cliente cliente = cargaEntrega.Cliente;

                        ponto.PontoControle = this.obterPontoControle(cliente);
                        ponto.Produtos = null;
                        ponto.DescricaoVisita = null;
                        ponto.Visita = false;

                        int minutosPadrao = 60;
                        if (cargaEntrega.Coleta && configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao > 0)
                            minutosPadrao = configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao;
                        else if (!cargaEntrega.Coleta && configuracaoTMS.TempoPadraoDeEntregaParaCalcularPrevisao > 0)
                            minutosPadrao = configuracaoTMS.TempoPadraoDeEntregaParaCalcularPrevisao;

                        ponto.DataHoraPrevisaoInicio = cargaEntrega.DataPrevista.Value;
                        ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio?.AddMinutes(minutosPadrao);

                        while (ponto.DataHoraPrevisaoInicio <= DataHoraPrevisaoFim)
                        {
                            ponto.DataHoraPrevisaoInicio = ponto.DataHoraPrevisaoInicio?.AddMinutes(minutosPadrao);
                            ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio?.AddMinutes(minutosPadrao);
                        }
                        //Se for a primeira Coleta, manda como Origem.
                        if (dadosPontosControle.Count == 0)
                            ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem;

                        //Se for a ultima entrega, manda como Destino.
                        else if (cargaEntrega == cargaEntregas[cargaEntregas.Count - 1])
                            ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Destino;

                        //Coleta.
                        else if (cargaEntrega.Coleta)
                            ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Coleta;

                        //Entrega.
                        else if (!cargaEntrega.Coleta)
                            ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Entrega;

                        dadosPontosControle.Add(ponto);
                    }
                }
            }
        
            return dadosPontosControle;
        }

        #endregion
    }
}
