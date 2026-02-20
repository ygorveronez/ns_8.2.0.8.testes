using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Marilan
{
    public class IntegracaoMarilan
    {
        #region Propriedades Privadas

        readonly private Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoMarilan(Repositorio.UnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        #endregion

        #region Métodos Públicos

        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoMarilan repositorioMarilan = new Repositorio.Embarcador.Configuracoes.IntegracaoMarilan(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarilan integracaoMarilan = repositorioMarilan.BuscarPrimeiroRegistro();

            faturaIntegracao.DataEnvio = DateTime.Now;
            faturaIntegracao.Tentativas++;
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                if (integracaoMarilan == null || !integracaoMarilan.PossuiIntegracaoMarilan)
                    throw new ServicoException("Não existe integração com a marilan configurado");

                ServicoMarilan.AdicionarFatura.ZEME_FATURA[] objetoRequisicao = ObterObjetoRequest(faturaIntegracao.Fatura);

                xmlRequisicao = Utilidades.XML.Serializar(objetoRequisicao);

                ServicoMarilan.AdicionarFatura.AdicionarFaturaPortTypeClient cliente = ObterClient(integracaoMarilan.URLMarilan, integracaoMarilan.UsuarioMarilan, integracaoMarilan.SenhaMarilan);
                cliente.Endpoint.EndpointBehaviors.Add(inspector);

                string message = cliente.AdicionarFatura(objetoRequisicao, out xmlRetorno);

                if (string.IsNullOrEmpty(xmlRetorno))
                    xmlRetorno = message;

                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                faturaIntegracao.MensagemRetorno = message;
            }
            catch (ServicoException ex)
            {
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = ex.Message;
            }

            SalvarArquivosIntegracaoFatura(faturaIntegracao, xmlRequisicao, xmlRetorno);
            repositorioFaturaIntegracao.Atualizar(faturaIntegracao);
        }

        public void IntegrarChamadoOcorrencia(Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoMarilan repositorioMarilan = new Repositorio.Embarcador.Configuracoes.IntegracaoMarilan(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarilan integracaoMarilan = repositorioMarilan.BuscarPrimeiroRegistro();

            chamadoIntegracao.DataIntegracao = DateTime.Now;
            chamadoIntegracao.NumeroTentativas++;

            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            InspectorBehavior inspector = new InspectorBehavior();
            try
            {
                if (integracaoMarilan == null || !integracaoMarilan.PossuiIntegracaoMarilan)
                    throw new ServicoException("Não existe integração com a Marilan configurado.");

                Servicos.ServicoMarilan.ChamadoOcorrencia.ZEME_OCORRENCIA[] objetoRequisicao = ObterObjetoRequestChamadoOcorrencia(chamadoIntegracao.Chamado);

                xmlRequisicao = Utilidades.XML.Serializar(objetoRequisicao);

                ServicoMarilan.ChamadoOcorrencia.AdicionarOcorrenciaPortTypeClient cliente = ObterClientChamadoOcorrencia(integracaoMarilan.URLMarilanChamadoOcorrencia, integracaoMarilan.UsuarioMarilan, integracaoMarilan.SenhaMarilan);
                cliente.Endpoint.EndpointBehaviors.Add(inspector);

                string mensagem = cliente.AdicionarOcorrencia(objetoRequisicao, out xmlRetorno);

                if (!string.IsNullOrEmpty(mensagem))
                {
                    chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    chamadoIntegracao.ProblemaIntegracao = mensagem;
                }
                else
                {
                    chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    chamadoIntegracao.ProblemaIntegracao = "";
                }

                servicoArquivoTransacao.Adicionar(chamadoIntegracao, xmlRequisicao, inspector.LastResponseXML, "xml");
            }
            catch (ServicoException ex)
            {
                chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                chamadoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                chamadoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Marilan";

                servicoArquivoTransacao.Adicionar(chamadoIntegracao, xmlRequisicao, inspector.LastResponseXML, "xml");
            }

            repChamadoIntegracao.Atualizar(chamadoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private ServicoMarilan.AdicionarFatura.ZEME_FATURA[] ObterObjetoRequest(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumento repositorioFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXml = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentosFaturamento = repositorioFaturaDocumento.BuscarPorFatura(fatura.Codigo);

            Dominio.Entidades.Embarcador.Filiais.Filial existeFilial = repositorioFilial.BuscarPorCNPJ(fatura?.ClienteTomadorFatura?.CPF_CNPJ_SemFormato ?? "");

            ServicoMarilan.AdicionarFatura.ZEME_FATURA[] listaFaturas = new ServicoMarilan.AdicionarFatura.ZEME_FATURA[1];
            ServicoMarilan.AdicionarFatura.ZEMM_DOCUMENTOS[] objDocumentos = new ServicoMarilan.AdicionarFatura.ZEMM_DOCUMENTOS[documentosFaturamento.Count];


            listaFaturas[0] = new ServicoMarilan.AdicionarFatura.ZEME_FATURA()
            {
                ZZEMPRESA = "",
                ZZCENTRO = existeFilial?.CodigoFilialEmbarcador ?? string.Empty,
                ZZCODIGO_TRANSPORTADOR = fatura?.Transportador?.CodigoIntegracao ?? string.Empty,
                ZZCNPJ_TRANSPORTADOR = fatura?.Transportador?.CNPJ_SemFormato ?? string.Empty,
                ZZNUMERO_FATURA = fatura.Numero.ToString() ?? string.Empty,
                ZZNUMERO_SERIE_FATURA = "1",
                ZZFLAG_180_DIAS = (fatura?.GerarDocumentosApenasCanhotosAprovados ?? false) ? "" : "X",
                ZZDATA_PROCESSAMENTO = DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ""),
                ZZHORA_PROCESSAMENTO = DateTime.Now.ToString("hh-mm-ss").Replace("-", ""),
                ZZDOCUMENTOS = objDocumentos
            };

            for (int documentoIndex = 0; documentoIndex < documentosFaturamento.Count; documentoIndex++)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento documentoFaturar = documentosFaturamento[documentoIndex];
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedidoXml.BuscarCargaPedidoPorCodigoCte(new List<int>() { documentoFaturar.Documento.CTe.Codigo });
                var documento = new ServicoMarilan.AdicionarFatura.ZEMM_DOCUMENTOS()
                {
                    ZZNUMERO_CTE = "",
                    ZZNUMERO_SERIE = documentoFaturar.Documento.CTe.Serie.Numero.ToString(),
                    ZZPROTOCOLO_CARGA = documentoFaturar.Documento.CargaPagamento.Protocolo.ToString(),
                    ZZPROTOCOLO_PEDIDO = cargaPedido.Pedido.Protocolo.ToString(),
                    ZZNUMERO_NFS = "",
                    ZZNUMERO_MIRO_SAP = documentoFaturar.Documento?.CTe?.CodigoEscrituracao ?? "",
                    ZZEXERCICIO_MIRO_SAP = ""
                };

                if (documentoFaturar.Documento.DescricaoTipoDocumento == "NFS-e")
                    documento.ZZNUMERO_NFS = documentoFaturar.Documento.CTe.Numero.ToString();
                else
                    documento.ZZNUMERO_CTE = documentoFaturar.Documento.CTe.Numero.ToString();

                listaFaturas[0].ZZDOCUMENTOS[documentoIndex] = documento;
            }

            return listaFaturas;
        }

        private Servicos.ServicoMarilan.ChamadoOcorrencia.ZEME_OCORRENCIA[] ObterObjetoRequestChamadoOcorrencia(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento repositorioChamadoInformacaoFechamento = new Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.Transbordo repositorioTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento> informacoesFechamento = repositorioChamadoInformacaoFechamento.BuscarPorChamado(chamado.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotaFiscais = repositorioCargaEntregaNotaFiscal.BuscarPorCargaEntrega(chamado.CargaEntrega.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.Transbordo> transbordos = (chamado.Carga?.CargaAgrupada ?? false) ? repositorioTransbordo.BuscarPorCargaGerada(cargaEntregaNotaFiscais.Select(o => o.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Cargas.Transbordo>();

            List<Servicos.ServicoMarilan.ChamadoOcorrencia.ZEME_OCORRENCIA> listaOcorrencias = new List<ServicoMarilan.ChamadoOcorrencia.ZEME_OCORRENCIA>();
            foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento chamadoInformacaoFechamento in informacoesFechamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaEntregaNotaFiscais.Where(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == chamadoInformacaoFechamento.XMLNotaFiscal.Codigo).Select(o => o.PedidoXMLNotaFiscal.CargaPedido).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = cargaPedido != null ? transbordos.Where(o => o.CargaGerada.Codigo == cargaPedido.CargaOrigem.Codigo).FirstOrDefault() : null;

                Servicos.ServicoMarilan.ChamadoOcorrencia.ZEME_OCORRENCIA zeme_OCORRENCIA = new ServicoMarilan.ChamadoOcorrencia.ZEME_OCORRENCIA()
                {
                    ZZNUMERO_OCORRENCIA = chamado.Numero.ToString(),
                    ZZDATA_OCORRENCIA = chamado.DataCriacao.ToString("yyyyMMdd"),
                    ZZCENTRO = chamado.Carga?.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                    ZZTIPO_OCORRENCIA = chamadoInformacaoFechamento.MotivoProcesso.CodigoIntegracao,
                    ZZDESCRICAO_OCORRENCIA = chamado.Observacao,
                    ZZNUMERO_NFE = chamadoInformacaoFechamento.XMLNotaFiscal.Numero.ToString(),
                    ZZNUMERO_SERIE_NFE = chamadoInformacaoFechamento.XMLNotaFiscal.Serie.ToString(),
                    ZZPROTOCOLO_CARGA = (transbordo?.Carga?.Protocolo ?? chamado.Carga?.Protocolo).ToString(),
                    ZZPROTOCOLO_PEDIDO = cargaPedido?.Pedido?.Protocolo.ToString() ?? string.Empty,
                    ZZCUSTO_FRETE = cargaPedido?.Pedido?.CustoFrete ?? string.Empty,
                    ZZTRECHO = chamado.Carga?.TipoDeCarga?.CodigoTipoCargaEmbarcador ?? string.Empty,
                    ZZQTDE = chamadoInformacaoFechamento.QuantidadeDivergencia.ToString(),
                    ZZDATA_ENTREGA = chamado.CargaEntrega.DataAgendamento.ToDateString()
                };

                listaOcorrencias.Add(zeme_OCORRENCIA);
            }

            return listaOcorrencias.ToArray();
        }

        private ServicoMarilan.AdicionarFatura.AdicionarFaturaPortTypeClient ObterClient(string url, string username, string senha)
        {
            //url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            Servicos.ServicoMarilan.AdicionarFatura.AdicionarFaturaPortTypeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
                client = new ServicoMarilan.AdicionarFatura.AdicionarFaturaPortTypeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoMarilan.AdicionarFatura.AdicionarFaturaPortTypeClient(binding, endpointAddress);
            }
            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = senha;
            return client;
        }

        private ServicoMarilan.ChamadoOcorrencia.AdicionarOcorrenciaPortTypeClient ObterClientChamadoOcorrencia(string url, string username, string senha)
        {
            if (!url.EndsWith("/"))
                url += "/";

            Servicos.ServicoMarilan.ChamadoOcorrencia.AdicionarOcorrenciaPortTypeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
                client = new ServicoMarilan.ChamadoOcorrencia.AdicionarOcorrenciaPortTypeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoMarilan.ChamadoOcorrencia.AdicionarOcorrenciaPortTypeClient(binding, endpointAddress);
            }

            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = senha;

            return client;
        }

        private void SalvarArquivosIntegracaoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string arquivoRequisicao, string arquivoRetorno)
        {
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = AdicionarArquivoTransacaoFatura(faturaIntegracao, arquivoRequisicao, arquivoRetorno, faturaIntegracao.MensagemRetorno);

            if (faturaIntegracaoArquivo == null)
                return;

            if (faturaIntegracao.ArquivosIntegracao == null)
                faturaIntegracao.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo>();

            faturaIntegracao.ArquivosIntegracao.Add(faturaIntegracaoArquivo);
        }

        private Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo AdicionarArquivoTransacaoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string arquivoRequisicao, string arquivoRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(arquivoRequisicao) && string.IsNullOrWhiteSpace(arquivoRetorno))
                return null;

            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repositorioFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(arquivoRequisicao, "xml", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(arquivoRetorno, "xml", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioFaturaIntegracaoArquivo.Inserir(faturaIntegracaoArquivo);

            return faturaIntegracaoArquivo;
        }

        private Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura ObterFiltrosDocumentosParaFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
            {
                CodigoFatura = fatura.Codigo,
                CodigoGrupoPessoas = fatura.TipoPessoa == TipoPessoa.GrupoPessoa ? fatura.GrupoPessoas?.Codigo ?? 0 : 0,
                CPFCNPJTomador = fatura.TipoPessoa == TipoPessoa.Pessoa ? fatura.Cliente?.CPF_CNPJ ?? 0D : 0D,
                AliquotaICMS = fatura.AliquotaICMS,
                CodigoContainer = fatura.Container?.Codigo ?? 0,
                CodigoCTe = fatura.CTe?.Codigo ?? 0,
                CodigoMDFe = fatura.MDFe?.Codigo ?? 0,
                CodigoVeiculo = fatura.Veiculo?.Codigo ?? 0,
                Destino = fatura.Destino?.Codigo ?? 0,
                NumeroBooking = fatura.NumeroBooking,
                NumeroControleCliente = fatura.NumeroControleCliente,
                NumeroReferenciaEDI = fatura.NumeroReferenciaEDI,
                PedidoViagemNavio = fatura.PedidoViagemNavio?.Codigo ?? 0,
                Origem = fatura.Origem?.Codigo ?? 0,
                TerminalDestino = fatura.TerminalDestino?.Codigo ?? 0,
                TerminalOrigem = fatura.TerminalOrigem?.Codigo ?? 0,
                TipoCarga = fatura.TipoCarga?.Codigo ?? 0,
                TipoOperacao = fatura.TipoOperacao?.Codigo ?? 0,
                TipoPropostaMultimodal = fatura.TipoPropostaMultimodal?.ToList(),
                Empresa = fatura.Transportador?.Codigo ?? 0,
                PaisOrigem = fatura.PaisOrigem?.Codigo ?? 0,
            };

            if (!(filtros.DataInicial >= fatura.DataInicial && filtros.DataInicial <= fatura.DataFinal))
                filtros.DataInicial = fatura.DataInicial;

            if (!(filtros.DataFinal >= fatura.DataInicial && filtros.DataFinal <= fatura.DataFinal))
                filtros.DataFinal = fatura.DataFinal;

            return filtros;
        }

        #endregion
    }
}