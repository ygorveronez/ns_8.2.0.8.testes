using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.YPE
{
    public class IntegracaoYPE
    {
        #region Atributos
        private readonly Repositorio.UnitOfWork _unitOfWork;
        #endregion

        #region Constructores
        public IntegracaoYPE(Repositorio.UnitOfWork unitOfWork) : base() { _unitOfWork = unitOfWork; }
        #endregion

        #region Metodos Publicos

        public void IntegrarPagamentos(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            pagamentoIntegracao.NumeroTentativas++;
            pagamentoIntegracao.DataIntegracao = DateTime.Now;

            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;
            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYPE configuracaoIntegracao = ObterConfiguracaoIntegracao();

                if (string.IsNullOrEmpty(configuracaoIntegracao.URLintegracao))
                    throw new ServicoException("Url de integração YPE não configurada");

                ServicosYPE.IntegrarPagamento.DT_RecebeContabilizacaoRow[] objetoRequisicao = ObterObjetoRequisicao(pagamentoIntegracao);
                xmlRequest = Utilidades.XML.Serializar(objetoRequisicao);

                ServicosYPE.IntegrarPagamento.SI_OS_RecebeContabilizacaoClient cliente = ObterCliente(configuracaoIntegracao.URLintegracao, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                cliente.Endpoint.EndpointBehaviors.Add(inspector);

                ServicosYPE.IntegrarPagamento.DT_RecebeContabilizacao_Resp retornoWS = cliente.SI_OS_RecebeContabilizacao(objetoRequisicao);

                pagamentoIntegracao.ProblemaIntegracao = "";
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                servicoArquivoTransacao.Adicionar(pagamentoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }
            catch (ServicoException ex)
            {
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = ex.Message;
                servicoArquivoTransacao.Adicionar(pagamentoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }

            repositorioPagamentoIntegracao.Atualizar(pagamentoIntegracao);
        }

        public void IntegrarGestaoDevolucaoSAPLaudo(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao gestaoDevolucaoIntegracao)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao repositorioGestaoDevolucaoIntegracao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracaoArquivo>(_unitOfWork);

            gestaoDevolucaoIntegracao.NumeroTentativas++;
            gestaoDevolucaoIntegracao.DataIntegracao = DateTime.Now;

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYPE configuracaoIntegracao = ObterConfiguracaoIntegracao();

                if (string.IsNullOrEmpty(configuracaoIntegracao.URLintegracao))
                    throw new ServicoException("Url de integração YPE não configurada");

                ServicosYPE.IntegrarGestaoDevolucao.SI_OS_RecebeDadosLaudoClient client = ObterClientGestaoDevolucao(configuracaoIntegracao.URLintegracaoRecebeDadosLaudo, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                client.Endpoint.EndpointBehaviors.Add(inspector);

                Servicos.ServicosYPE.IntegrarGestaoDevolucao.SI_OS_RecebeDadosLaudoRequest objetoRequisicao = ObterObjetoRequisicaoDadosLaudo(gestaoDevolucaoIntegracao.GestaoDevolucao);

                Servicos.ServicosYPE.IntegrarGestaoDevolucao.SI_OS_RecebeDadosLaudoResponse retornoWS = client.SI_OS_RecebeDadosLaudo(objetoRequisicao);

                gestaoDevolucaoIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                gestaoDevolucaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                gestaoDevolucaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                gestaoDevolucaoIntegracao.ProblemaIntegracao = ex.Message;
            }

            servicoArquivoTransacao.Adicionar(gestaoDevolucaoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            repositorioGestaoDevolucaoIntegracao.Atualizar(gestaoDevolucaoIntegracao);
        }

        #endregion

        #region Metodos Privados
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYPE ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoYPE repositorioIntegracaoYpe = new Repositorio.Embarcador.Configuracoes.IntegracaoYPE(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYPE configuracaoIntegracao = repositorioIntegracaoYpe.BuscarPrimeiroRegistro();

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Integração com YPE não configurada");

            if (string.IsNullOrEmpty(configuracaoIntegracao.Usuario) || string.IsNullOrEmpty(configuracaoIntegracao.Senha))
                throw new ServicoException("Dados de autenticação não configurados");

            return configuracaoIntegracao;
        }

        private ServicosYPE.IntegrarPagamento.SI_OS_RecebeContabilizacaoClient ObterCliente(string url, string usuario, string senha)
        {
            ServicosYPE.IntegrarPagamento.SI_OS_RecebeContabilizacaoClient cliente = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
                cliente = new ServicosYPE.IntegrarPagamento.SI_OS_RecebeContabilizacaoClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                cliente = new ServicosYPE.IntegrarPagamento.SI_OS_RecebeContabilizacaoClient(binding, endpointAddress);
            }
            cliente.ClientCredentials.UserName.UserName = usuario;
            cliente.ClientCredentials.UserName.Password = senha;

            return cliente;
        }

        private ServicosYPE.IntegrarPagamento.DT_RecebeContabilizacaoRow[] ObterObjetoRequisicao(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            ServicosYPE.IntegrarPagamento.DT_RecebeContabilizacaoRow[] pagametosAintegrar = new ServicosYPE.IntegrarPagamento.DT_RecebeContabilizacaoRow[1];
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repositorioDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentosContabeis = repositorioDocumentoContabil.BuscarPorPagamento(pagamentoIntegracao.Pagamento.Codigo, pagamentoIntegracao.Pagamento.LotePagamentoLiberado);
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCTe(pagamentoIntegracao.DocumentoFaturamento?.CTe.Codigo ?? 0);

            string tipoPlacamentoFixoPagamento = "2";

            pagametosAintegrar[0] = new ServicosYPE.IntegrarPagamento.DT_RecebeContabilizacaoRow()
            {
                REG_070 = new ServicosYPE.IntegrarPagamento.DT_070()
                {
                    TPREGISTRO = "070",
                    IDDOC = pagamentoIntegracao?.Pagamento?.Numero.ToString() ?? "",
                    TPLANCAMENTO = tipoPlacamentoFixoPagamento,
                    SISCORPORATIVO = "",
                    CDREFERENCIA = pagamentoIntegracao.DocumentoFaturamento.CTe?.NumeroNotas ?? pedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero.ToString() ?? string.Empty,
                    CDCOMPANHIA = pagamentoIntegracao?.DocumentoFaturamento?.Tomador?.CPF_CNPJ_SemFormato ?? string.Empty,
                    DTLANCAMENTO = DateTime.Now.ToString("dd/MM/yyyy"),
                    DTVENCIMENTO = DateTime.Now.ToString("dd/MM/yyyy"),
                    DTEMISSAO = pagamentoIntegracao.DocumentoFaturamento.DataEmissao.ToString("dd/MM/yyyy"),
                    NONF = pagamentoIntegracao.DocumentoFaturamento?.CTe?.Numero.ToString() ?? string.Empty,
                    CDSERIE = pagamentoIntegracao.DocumentoFaturamento?.CTe?.Serie?.Descricao ?? string.Empty,
                    CDEVENTO = !string.IsNullOrEmpty(pagamentoIntegracao.DocumentoFaturamento.CargaOcorrenciaPagamento?.TipoOcorrencia?.CodigoIntegracao) ? pagamentoIntegracao.DocumentoFaturamento.CargaOcorrenciaPagamento?.TipoOcorrencia?.CodigoIntegracao : pagamentoIntegracao.DocumentoFaturamento.ModeloDocumentoFiscal?.TipoDocumentoEmissao.ObterDescricao(),
                    VALTOTPROV = pagamentoIntegracao.DocumentoFaturamento?.CTe.ValorAReceber.ToString("n2"),
                    TRANSPORTADORA = pagamentoIntegracao.DocumentoFaturamento?.CTe?.Empresa?.CNPJ_SemFormato ?? "",
                    DESTINO = pagamentoIntegracao?.DocumentoFaturamento?.Destino?.Estado?.Sigla ?? "",
                    ORIGEM = pagamentoIntegracao?.DocumentoFaturamento?.Origem?.Estado?.Sigla ?? "",
                    REMETENTE = pagamentoIntegracao?.DocumentoFaturamento?.Remetente?.CPF_CNPJ_SemFormato ?? ""
                }//,
                //REG_075 = new ServicosYPE.IntegrarPagamento.DT_075[documentosContabeis.Count]
            };

            List<ServicosYPE.IntegrarPagamento.DT_075> pagamentos = new List<ServicosYPE.IntegrarPagamento.DT_075>();

            for (int i = 0; i < documentosContabeis.Count; i++)
            {
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documentoContabil = documentosContabeis[i];

                string tipoContabilicacao = string.Empty;

                if (documentoContabil.TipoContabilizacao == TipoContabilizacao.Debito)
                    tipoContabilicacao = "D";
                else if (documentoContabil.TipoContabilizacao == TipoContabilizacao.Credito)
                    tipoContabilicacao = "C";

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCte.BuscarPorCodigo(documentoContabil?.CTe?.Codigo ?? 0);

                List<int> notas = (from obj in cte.XMLNotaFiscais select obj.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xMLNotaFiscalProdutos = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(notas);

                if (xMLNotaFiscalProdutos.Count > 1)
                    xMLNotaFiscalProdutos = (from obj in xMLNotaFiscalProdutos where obj.Produto.CodigoProdutoEmbarcador != "90.9000" select obj).ToList();

                decimal valorTotal = 0;
                decimal PesoTotal = xMLNotaFiscalProdutos.Count > 0 ? xMLNotaFiscalProdutos.Sum(obj => obj.PesoProduto) : (notas.Count > 0 ? cte.XMLNotaFiscais.Select(o => o.Peso).Sum() : 0);

                if (PesoTotal == 0)
                    throw new ServicoException("Peso total dos produtos é 0, Validar quantidade informada na nota e o peso do produto.");

                if (documentoContabil.TipoContaContabil.ObterCodigo() == "10")
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto ultimoProduto = xMLNotaFiscalProdutos.LastOrDefault();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto produto in xMLNotaFiscalProdutos)
                    {
                        decimal valorItem = Math.Round((documentoContabil.ValorContabilizacao * produto.PesoProduto) / PesoTotal, 4);
                        valorTotal += valorItem;
                        if (produto.Equals(ultimoProduto))
                            valorItem = (valorTotal - documentoContabil.ValorContabilizacao) + valorItem;

                        pagamentos.Add(GerarRegistro075(documentoContabil, valorItem, tipoContabilicacao, produto.Produto.CodigoProdutoEmbarcador != "90.9000" ? produto.Produto.CodigoProdutoEmbarcador : ""));
                    }
                }
                else
                    pagamentos.Add(GerarRegistro075(documentoContabil, documentoContabil.ValorContabilizacao, tipoContabilicacao, ""));
            }

            pagametosAintegrar[0].REG_075 = pagamentos.ToArray();

            return pagametosAintegrar;
        }

        private ServicosYPE.IntegrarPagamento.DT_075 GerarRegistro075(Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documentoContabil, decimal valorContabilizacao, string tipoContabilicacao, string codigoProduto)
        {
            ServicosYPE.IntegrarPagamento.DT_075 r075 = new ServicosYPE.IntegrarPagamento.DT_075()
            {
                TPREGISTRO = "075",
                CDCONTACONTABIL = documentoContabil?.PlanoConta?.PlanoContabilidade.ToString() ?? string.Empty,
                DC = tipoContabilicacao,
                VRLANCAMENTO = valorContabilizacao.ToString(),
                CDITEM = documentoContabil.TipoContaContabil.ObterCodigo() != "10" ? "" : codigoProduto,
                CDCENTROCUSTO = string.Empty,
                NONF = documentoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido ? documentoContabil?.XMLNotaFiscal?.Numero.ToString() ?? string.Empty : documentoContabil?.CTe?.Numero.ToString() ?? string.Empty,
                NC = "",
                TPRESPDESPACOMP = "",
                CDODESPCOMP = "",
                IDDOCPROV = "",
                TPCONTA = $"{documentoContabil.TipoContaContabil.ObterCodigo()}"
            };
            return r075;
        }

        private ServicosYPE.IntegrarGestaoDevolucao.SI_OS_RecebeDadosLaudoClient ObterClientGestaoDevolucao(string url, string usuario, string senha)
        {
            ServicosYPE.IntegrarGestaoDevolucao.SI_OS_RecebeDadosLaudoClient cliente = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
                cliente = new ServicosYPE.IntegrarGestaoDevolucao.SI_OS_RecebeDadosLaudoClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                cliente = new ServicosYPE.IntegrarGestaoDevolucao.SI_OS_RecebeDadosLaudoClient(binding, endpointAddress);
            }
            cliente.ClientCredentials.UserName.UserName = usuario;
            cliente.ClientCredentials.UserName.Password = senha;

            return cliente;
        }

        private Servicos.ServicosYPE.IntegrarGestaoDevolucao.SI_OS_RecebeDadosLaudoRequest ObterObjetoRequisicaoDadosLaudo(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO repositorioGestaoDevolucaoNFDxNFO = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO(_unitOfWork);
            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(_unitOfWork);
            gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(gestaoDevolucao.Codigo);

            List<ServicosYPE.IntegrarGestaoDevolucao.DT_RecebeDadosLaudo_ReqDadosLaudo> objetoDadosLaudo = new List<ServicosYPE.IntegrarGestaoDevolucao.DT_RecebeDadosLaudo_ReqDadosLaudo>();

            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO> listaNotasOrigem = repositorioGestaoDevolucaoNFDxNFO.BuscarPorNFD(gestaoDevolucao.NotasFiscaisDevolucao.Select(nota => nota.XMLNotaFiscal.Codigo).ToList());

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNFD = gestaoDevolucao.NotasFiscaisDevolucao.Select(nota => nota.XMLNotaFiscal).ToList();
            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NFD in listaNFD)
            {
                objetoDadosLaudo.Add(new ServicosYPE.IntegrarGestaoDevolucao.DT_RecebeDadosLaudo_ReqDadosLaudo()
                {
                    NumeroLaudo = gestaoDevolucao.Laudo.Codigo.ToString(),
                    NumeroCompensacao = gestaoDevolucao.Laudo.NumeroCompensacao ?? string.Empty,
                    DataCompensacao = gestaoDevolucao.Laudo.DataCompensacao?.ToString() ?? string.Empty,
                    DataDocumento = gestaoDevolucao.Laudo.DataAnalise?.ToString() ?? string.Empty,
                    CodFornecedor = gestaoDevolucao.Laudo.Transportador.CNPJ ?? string.Empty,
                    ChaveNFeReferencia = listaNotasOrigem.Find(n => n.NFD.Codigo == NFD.Codigo)?.NFO.Chave ?? string.Empty,
                    ChaveNFD = NFD.Chave,
                    Valor = gestaoDevolucao.Laudo.Produtos.Sum(p => p.ValorTotal).ToString().Replace(",", "."),
                    Observacao = string.Empty,
                    NumeroDocumento = string.Empty,
                    TipoDocumento = string.Empty,
                    Atribuicao = string.Empty,
                    NumeroDias = string.Empty,
                    DataBase = string.Empty,
                    DataEmissao = string.Empty,
                    DataVencimento = string.Empty,
                    PossuiBloqueio = string.Empty,
                    Pagamento = string.Empty,
                    Moeda = string.Empty,
                    Cabecalho = string.Empty,
                });
            }

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNFO = gestaoDevolucao.NotasFiscaisDeOrigem.Where(nota => nota.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet && !listaNotasOrigem.Select(n => n.NFO.Codigo).Contains(nota.XMLNotaFiscal.Codigo)).Select(nota => nota.XMLNotaFiscal).ToList();
            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NFO in listaNFO)
            {
                objetoDadosLaudo.Add(new ServicosYPE.IntegrarGestaoDevolucao.DT_RecebeDadosLaudo_ReqDadosLaudo()
                {
                    NumeroLaudo = gestaoDevolucao.Laudo.Codigo.ToString(),
                    NumeroCompensacao = gestaoDevolucao.Laudo.NumeroCompensacao ?? string.Empty,
                    DataCompensacao = gestaoDevolucao.Laudo.DataCompensacao?.ToString() ?? string.Empty,
                    DataDocumento = gestaoDevolucao.Laudo.DataAnalise?.ToString() ?? string.Empty,
                    CodFornecedor = gestaoDevolucao.Laudo.Transportador.CNPJ ?? string.Empty,
                    ChaveNFeReferencia = NFO.Chave,
                    ChaveNFD = string.Empty,
                    Valor = gestaoDevolucao.Laudo.Produtos.Sum(p => p.ValorTotal).ToString().Replace(",", "."),
                    Observacao = string.Empty,
                    NumeroDocumento = string.Empty,
                    TipoDocumento = string.Empty,
                    Atribuicao = string.Empty,
                    NumeroDias = string.Empty,
                    DataBase = string.Empty,
                    DataEmissao = string.Empty,
                    DataVencimento = string.Empty,
                    PossuiBloqueio = string.Empty,
                    Pagamento = string.Empty,
                    Moeda = string.Empty,
                    Cabecalho = string.Empty,
                });
            }

            return new Servicos.ServicosYPE.IntegrarGestaoDevolucao.SI_OS_RecebeDadosLaudoRequest(objetoDadosLaudo.ToArray());
        }
        #endregion
    }
}
