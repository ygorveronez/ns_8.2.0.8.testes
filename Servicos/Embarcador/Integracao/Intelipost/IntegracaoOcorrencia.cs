using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Integracao.Intelipost
{
    public class IntegracaoOcorrencia
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoOcorrencia(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void EnviarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            integracao.DataIntegracao = DateTime.Now;
            integracao.NumeroTentativas++;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(integracao.CargaCTe.Codigo);
            if (cargaPedidos.Count == 0)
            {
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Não foi encontrado um pedido compatível com o documento da ocorrência.";
                repOcorrenciaCTeIntegracao.Atualizar(integracao);
                return;
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                try
                {
                    if (integracao.CargaOcorrencia?.Carga?.Empresa == null)
                        throw new ServicoException("Empresa não encontrada");

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = pedido.NotasFiscais.ToList();
                    if (notasFiscais.Count <= 0)
                        notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(pedido.Codigo);

                    if (notasFiscais.Count == 0)
                        throw new Exception("Não possui nota fiscal para integrar!");

                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = EnviarOcorrencia(pedido, integracao.CargaOcorrencia.TipoOcorrencia, notaFiscal, integracao.CargaOcorrencia.Carga.Empresa, integracao.CargaOcorrencia.DataOcorrencia, integracao.CargaOcorrencia.Carga, clienteMultisoftware);
                        integracao.ProblemaIntegracao = httpRequisicaoResposta.mensagem;
                        integracao.SituacaoIntegracao = httpRequisicaoResposta.sucesso ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        SalvarArquivosIntegracao(integracao, httpRequisicaoResposta, pedido);

                        repOcorrenciaCTeIntegracao.Atualizar(integracao);
                    }
                }
                catch (ServicoException ex)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    integracao.ProblemaIntegracao = ex.Message;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    integracao.ProblemaIntegracao = ex.Message;
                }

                repOcorrenciaCTeIntegracao.Atualizar(integracao);
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta EnviarOcorrencia(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, Dominio.Entidades.Empresa empresa, DateTime data, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            IntegracaoIntelipost servicoIntegracaoIntelipost = new IntegracaoIntelipost(_unitOfWork);

            string token = servicoIntegracaoIntelipost.ObterToken(empresa?.Codigo ?? 0, pedido.CanalEntrega?.Codigo ?? 0);
            if (string.IsNullOrWhiteSpace(token))
                throw new ServicoException("Não foi encontrada configuração para o token no transportador.");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.EventoRastreamento eventoRastreamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.EventoRastreamento()
            {
                logistic_provider = empresa?.RazaoSocial,
                logistic_provider_id = empresa?.CodigoIntegracao,
                logistic_provider_federal_tax_id = empresa?.CNPJ,
                shipper = clienteMultisoftware?.RazaoSocial,
                shipper_federal_tax_id = clienteMultisoftware?.CNPJ,
                invoice_key = notaFiscal.Chave,
                invoice_series = notaFiscal.Serie,
                invoice_number = notaFiscal.Numero.ToString(),
                tracking_code = pedido.CodigoRastreamento,
                order_number = pedido.NumeroPedidoEmbarcador,
                //volume_number = notaFiscal.Volumes.ToString(),
                events = ObterEventos(data, tipoDeOcorrencia, empresa, pedido, carga)
            };

            return servicoIntegracaoIntelipost.EnviarEventoRastreamento(token, eventoRastreamento);
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.Evento> ObterEventos(DateTime data, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Global.EmpresaIntelipostTipoOcorrencia repEmpresaIntelipostTipoOcorrencia = new Repositorio.Global.EmpresaIntelipostTipoOcorrencia(_unitOfWork);
            Dominio.Entidades.EmpresaIntelipostTipoOcorrencia configuracaoOcorrencia = repEmpresaIntelipostTipoOcorrencia.BuscarPorTipoOcorrencia(empresa?.Codigo ?? 0, tipoDeOcorrencia?.Codigo ?? 0);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.Evento> eventos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.Evento>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.Evento evento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.Evento()
            {
                original_code = configuracaoOcorrencia != null ? configuracaoOcorrencia.CodigoIntegracao : tipoDeOcorrencia.CodigoIntegracao,
                original_message = configuracaoOcorrencia != null ? configuracaoOcorrencia.MacroStatus : tipoDeOcorrencia.Descricao,
                event_date = string.Format("{0:s}", data) + "-03:00",
                attachments = ObterAnexosEvento(pedido, carga)
            };

            eventos.Add(evento);

            return eventos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.EventoAnexo> ObterAnexosEvento(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null || carga.Motoristas == null || carga.Motoristas.Count == 0)
                return null;

            Dominio.Entidades.Usuario motorista = carga.Motoristas.FirstOrDefault();

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.EventoAnexo> anexos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.EventoAnexo>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.EventoAnexo anexo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.EventoAnexo()
            {
                url = "http://intelipost-s3-storage.s3.amazonaws.com/3606/file_attachment.jpg",
                content_in_base64 = "",
                type = "POD",
                file_name = "01-05-18-assinatura-entrega",
                mime_type = "image/jpeg",
                additional_information = new Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.EventoAnexoInformacaoAdicional()
                {
                    nome = motorista.Nome,
                    cpf = motorista.CPF,
                    CodigoRota = pedido.CodigoCargaEmbarcador ?? carga.CodigoCargaEmbarcador
                }
            };

            anexos.Add(anexo);

            return anexos;
        }

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repOcorrenciaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();
            arquivoIntegracao.Data = integracao.DataIntegracao;
            arquivoIntegracao.Mensagem = pedido.NumeroPedidoEmbarcador + " - " + integracao.ProblemaIntegracao;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(httpRequisicaoResposta.conteudoRequisicao, httpRequisicaoResposta.extensaoRequisicao, _unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(httpRequisicaoResposta.conteudoResposta, httpRequisicaoResposta.extensaoResposta, _unitOfWork);

            repOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            integracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        #endregion
    }
}
