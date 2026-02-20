using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using AdminMultisoftware.Dominio.Entidades.Pessoas;
using Utilidades.Extensions;
using System.Web;
using System.Linq;

namespace Servicos.Embarcador.Integracao.ConfirmaFacil
{
    public partial class IntegracaoConfirmaFacil
    {
        #region Métodos Globais

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrenciaPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, Repositorio.UnitOfWork unitOfWork, string urlAcessoCliente)
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
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConfirmaFacil configConfirmaFacil = new Repositorio.Embarcador.Configuracoes.IntegracaoConfirmaFacil(unitOfWork).Buscar();

                if (configConfirmaFacil == null || !configConfirmaFacil.PossuiIntegracao)
                    throw new ServicoException("Falha ao buscar configuração da integração com a Confirma Fácil");

                object objetoEnvio = null;
                bool ocorrenciaEntrega = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.TipoAplicacaoColetaEntrega == TipoAplicacaoColetaEntrega.Coleta ? true : false;
                
                objetoEnvio = GerarOcorrencia(integracao, unitOfWork, urlAcessoCliente);

                Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.retornoWebService retWS = Transmitir(configConfirmaFacil, configConfirmaFacil.URL, objetoEnvio);

                httpRequisicaoResposta.conteudoRequisicao = retWS.jsonRequisicao;
                httpRequisicaoResposta.conteudoResposta = retWS.jsonRetorno;

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;                    
                }
                else
                {
                    integracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                if (retWS.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                {                    
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

                httpRequisicaoResposta.mensagem = "Problema ao tentar integrar com Confirma Fácil.";
            }

            repPedidoOcorrenciaIntegracao.Atualizar(integracao);
            return httpRequisicaoResposta;
        }

        #endregion


        #region Métodos Privados

        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.nfeocorrencia> GerarOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, Repositorio.UnitOfWork unitOfWork, string urlAcessoCliente)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.nfeocorrencia> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.nfeocorrencia>();
            
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.ToList();
            if (integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.Count <= 0)
                notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

            if (notasFiscais.Count == 0)
                throw new ServicoException("Não possui nota fiscal para integrar!");

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNota in notasFiscais ?? new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>())
            {
                nfeocorrencia nota = new nfeocorrencia();

                int codigoOcorrencia = 0;
                if (!string.IsNullOrEmpty(integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.CodigoIntegracao))
                    int.TryParse(integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.CodigoIntegracao, out codigoOcorrencia);

                var embarque = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.Embarque();
                embarque.numero = XMLNota.Numero.ToString();
                embarque.serie = XMLNota.Serie.ToString();
                nota.embarque = embarque;

                var embarcador = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.Embarcador();
                embarcador.cnpj = integracao.PedidoOcorrenciaColetaEntrega.Pedido?.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty;
                nota.embarcador = embarcador;

                var transportadora = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.Transportadora();
                transportadora.cnpj = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Empresa?.CNPJ_SemFormato ?? string.Empty;
                transportadora.nome = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Empresa?.NomeCNPJ ?? string.Empty;
                nota.transportadora = transportadora;

                var ocorrencia = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.Ocorrencia();
                ocorrencia.tipoEntrega = codigoOcorrencia.ToString();
                ocorrencia.dtOcorrencia = integracao.PedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("dd-MM-yyyy") ?? "00-00-0000";
                ocorrencia.hrOcorrencia = integracao.PedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("HH:mm:ss") ?? "00:00:00";                

                List<string> fotos = new List<string>();
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo repAnexo = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo(unitOfWork);
                IList<int> anexos = repAnexo.BuscarCodigoAnexosCarga(integracao.PedidoOcorrenciaColetaEntrega.Carga.Codigo);                
                foreach (int anexo in anexos)
                {                        
                    string url = "https://" + urlAcessoCliente + "/Link/BuscarFoto/?";                    
                    string key = Servicos.Criptografia.Criptografar(anexo.ToString(), "CT3##MULT1@#$S0FTW4R3");
                    fotos.Add(url + key);                        
                }
                
                ocorrencia.fotos = fotos.ToArray() ?? null;
                nota.ocorrencia = ocorrencia;                

                retorno.Add(nota);
            }

            return retorno;
        }                

        #endregion
    }
}
