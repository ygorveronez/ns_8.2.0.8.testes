using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Integracao.Runtec
{
    public partial class IntegracaoRuntec
    {
        #region Métodos Globais

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrenciaPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);

            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRuntec configRuntec = new Repositorio.Embarcador.Configuracoes.IntegracaoRuntec(unitOfWork).Buscar();

                if (configRuntec == null || !configRuntec.PossuiIntegracao)
                    throw new ServicoException("Falha ao buscar configuração da integração com a Runtec");


                object objetoEnvio = null;
                bool ocorrenciaEntrega = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.TipoAplicacaoColetaEntrega == TipoAplicacaoColetaEntrega.Coleta ? true : false;
                string urlWebService = configRuntec.URL;
                if (urlWebService.EndsWith("/"))
                    urlWebService += (ocorrenciaEntrega ? "nfe/entrega" : "nfe/ocorrencia");
                else
                    urlWebService += (ocorrenciaEntrega ? "/nfe/entrega" : "/nfe/ocorrencia");

                if (ocorrenciaEntrega)
                    objetoEnvio = GerarEntrega(integracao, unitOfWork);
                else
                    objetoEnvio = GerarOcorrencia(integracao, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.retornoWebService retWS = Transmitir(configRuntec, urlWebService, objetoEnvio);

                httpRequisicaoResposta.conteudoRequisicao = retWS.jsonRequisicao;
                httpRequisicaoResposta.conteudoResposta = retWS.jsonRetorno;

                if (retWS.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                {
                    Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

                    httpRequisicaoResposta.sucesso = true;
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso";
                }
                else
                {
                    throw new ServicoException(retWS.ProblemaIntegracao);
                }
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                httpRequisicaoResposta.mensagem = "Problema ao tentar integrar com Runtec.";
            }

            repPedidoOcorrenciaIntegracao.Atualizar(integracao);
            return httpRequisicaoResposta;
        }

        #endregion


        #region Métodos Privados

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeocorrencia GerarOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeocorrencia retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeocorrencia();

            retorno.embarcador = integracao.PedidoOcorrenciaColetaEntrega.Pedido?.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty;

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNota in integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais ?? new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>())
            {
                nfeocorrencianota nota = new nfeocorrencianota();

                int codigoOcorrencia = 0;
                if (!string.IsNullOrEmpty(integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.CodigoIntegracao))
                    int.TryParse(integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.CodigoIntegracao, out codigoOcorrencia);

                nota.motivo_ocorrencia = codigoOcorrencia;
                nota.data_problema = integracao.PedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("yyyy-MM-ddTHH:mm:ss") ?? "0000-00-00T00:00:00";
                nota.motorista = integracao.PedidoOcorrenciaColetaEntrega?.Carga?.Motoristas?.FirstOrDefault().CPF ?? string.Empty;
                nota.veiculo = integracao.PedidoOcorrenciaColetaEntrega?.Carga?.Veiculo?.Placa ?? string.Empty;
                nota.placa = integracao.PedidoOcorrenciaColetaEntrega?.Carga?.Veiculo?.Placa ?? string.Empty;
                nota.data_chegada = null;
                nota.chave = XMLNota.Chave;
                nota.transportadora = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Empresa?.CNPJ_SemFormato ?? string.Empty;
                nota.latitude = Convert.ToDecimal(XMLNota?.Destinatario?.Latitude ?? "0.0");
                nota.longitude = Convert.ToDecimal(XMLNota?.Destinatario?.Longitude ?? "0.0");

                retorno.notas.Add(nota);
            }

            return retorno;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeentrega GerarEntrega(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeentrega retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeentrega();

            retorno.embarcador = integracao.PedidoOcorrenciaColetaEntrega.Pedido?.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty;

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNota in integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais ?? new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>())
            {
                nfeentreganota nota = new nfeentreganota();

                nota.data_entrega = integracao.PedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("yyyy-MM-ddTHH:mm:ss") ?? "0000-00-00T00:00:00";
                nota.recebedor_nome = "0";
                nota.recebedor_rg = "0";
                nota.chave = XMLNota.Chave;
                nota.transportadora = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Empresa?.CNPJ_SemFormato ?? string.Empty;
                nota.latitude = Convert.ToDecimal(XMLNota?.Destinatario?.Latitude ?? "0.0");
                nota.longitude = Convert.ToDecimal(XMLNota?.Destinatario?.Longitude ?? "0.0");

                retorno.notas.Add(nota);
            }

            return retorno;
        }

        #endregion
    }
}
