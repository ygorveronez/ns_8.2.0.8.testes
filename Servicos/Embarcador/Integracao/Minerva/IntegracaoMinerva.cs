using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Servicos.Embarcador.Integracao.Minerva
{
    public class IntegracaoMinerva : ServicoBase
    {
        #region Construtores        

        public IntegracaoMinerva(Repositorio.UnitOfWork _unitOfWork, CancellationToken cancelationToken = default) : base(_unitOfWork, cancelationToken)
        {
        }

        public IntegracaoMinerva(Repositorio.UnitOfWork _unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(_unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #endregion

        #region Métodos Públicos

        public async Task<bool> BuscarNotasFiscaisPorCNPJAsync(string cnpjEmitente, string codigoFilial, string codigoOperacao, DateTime dataInicio, DateTime dataFim, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                int.TryParse(codigoFilial, out int codigoFilialInt);

                using (ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoMinerva.IntegraCargas.IntegraCargasSoapClient, ServicoMinerva.IntegraCargas.IntegraCargasSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Minerva_IntegraCargas))
                {
                    ServicoMinerva.IntegraCargas.NotasFiscaisTerceiros[] notasFiscais = integraCargasSoapClient.BuscarNotasFiscaisPorCNPJ(cnpjEmitente, dataInicio, dataFim, codigoFilialInt, codigoOperacao);

                    Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(_unitOfWork, _cancellationToken);
                    Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(_unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

                    string path = Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = await repConfiguracaoWebService.BuscarConfiguracaoPadraoAsync();

                    foreach (var nota in notasFiscais)
                    {
                        if (!string.IsNullOrWhiteSpace(nota.xml_nfe))
                        {
                            try
                            {
                                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(nota.chave_acesso_nfe, ".xml"));
                                XmlDocument xdoc = new XmlDocument();
                                xdoc.LoadXml(nota.xml_nfe);

                                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                                {
                                    xdoc.Save(stream);
                                    Utilidades.IO.FileStorageService.Storage.SaveStream(caminho, stream);
                                }

                                System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));
                                Servicos.NFe svcNFe = new Servicos.NFe(_unitOfWork);
                                dynamic nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, _unitOfWork);

                                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFiscal.BuscarPorChave(nota.chave_acesso_nfe);

                                if (xmlNotaFiscalExiste == null)
                                {
                                    serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, reader, _unitOfWork, nfXml, true, false, false, null, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false);

                                    xmlNotaFiscal.TipoNotaFiscalIntegrada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.Faturamento;
                                    xmlNotaFiscal.SemCarga = true;

                                    await repXMLNotaFiscal.InserirAsync(xmlNotaFiscal);

                                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(_unitOfWork).BuscarConfiguracaoPadraoAsync();
                                    serCanhoto.SalvarCanhotoNota(xmlNotaFiscal, null, null, new List<Dominio.Entidades.Usuario>(), AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, configuracao, _unitOfWork, configuracaoCanhoto);
                                }
                            }
                            catch (Exception)
                            {
                                Servicos.Log.TratarErro("Não foi possível importar XML NFe " + nota.chave_acesso_nfe + " sem CTe.", "IntegracaoMinervaNFe");
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("NFe não retornou XML " + nota.chave_acesso_nfe + " sem CTe.", "IntegracaoMinervaNFe");
                        }

                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "IntegracaoMinervaNFe");
                return false;
            }
        }

        public byte[] BuscarPDFNFe(string chaveNFe)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoMinerva.IntegraCargas.IntegraCargasSoapClient, ServicoMinerva.IntegraCargas.IntegraCargasSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Minerva_IntegraCargas))
            {
                return integraCargasSoapClient.BuscaDanfeNFe(chaveNFe);
            }
        }

        public byte[] BuscarPDFNFePorXML(string xmlNFe)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoMinerva.IntegraCargas.IntegraCargasSoapClient, ServicoMinerva.IntegraCargas.IntegraCargasSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Minerva_IntegraCargas))
            {
                return integraCargasSoapClient.GerarDanfeNFe(xmlNFe);
            }
        }

        public async Task CancelarCargaAsync(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = await repositorioConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLMinerva))
            {
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Configuração para integração com Minerva inválida.";
                cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
                cargaCancelamentoCargaIntegracao.NumeroTentativas++;
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                await repositorioCargaCancelamentoCargaIntegracao.AtualizarAsync(cargaCancelamentoCargaIntegracao);

                return;
            }

            string mensagem = string.Empty;
            bool retorno = false;
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = null;

            int codigoCarga = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo;

            retorno = RetornarCancelamentoCarga(out arquivoIntegracao, out mensagem, codigoCarga);

            if (arquivoIntegracao != null)
                cargaCancelamentoCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mensagem;
            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaIntegracao.NumeroTentativas++;

            if (retorno)
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            else
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            await repositorioCargaCancelamentoCargaIntegracao.AtualizarAsync(cargaCancelamentoCargaIntegracao);
        }

        public async Task IntegrarCargaAsync(int codigoCargaIntegracao)
        {
            Servicos.Log.GravarInfo($"Iniciando {codigoCargaIntegracao}", $"IntegracaoMinerva");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork, _cancellationToken);
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = await repositorioCargaCargaIntegracao.BuscarPorCodigoAsync(codigoCargaIntegracao, false);

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, _cancellationToken);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = await repositorioConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLMinerva))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Minerva.";
            }
            else
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(cargaIntegracao.Carga.Codigo);

                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;
                cargaIntegracao.ProblemaIntegracao = string.Empty;

                bool situacaoIntegracao = true;
                string mensagemErro = string.Empty;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repositorioCargaPedido.BuscarPorCargaSemNfsManual(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega);

                if (carga.CargaEmitidaParcialmente)
                    listaCargaPedidos = (from obj in listaCargaPedidos where obj.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada select obj).ToList();

                using (ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoMinerva.IntegraCargas.IntegraCargasSoapClient, ServicoMinerva.IntegraCargas.IntegraCargasSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Minerva_IntegraCargas, out Models.Integracao.InspectorBehavior inspector))
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedidos)
                    {
                        if (!cargaPedido.PedidoSemNFe)
                        {
                            mensagemErro = string.Empty;
                            string clientRequestContent = string.Empty;
                            string clientResponseContent = string.Empty;

                            string codigoFilial = cargaPedido.CargaOrigem != null && cargaPedido.CargaOrigem.Filial != null ? cargaPedido.CargaOrigem.Filial.CodigoFilialEmbarcador : carga.Filial?.CodigoFilialEmbarcador ?? "";

                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCTe.BuscarTodosPorCargaPedido(cargaPedido.Codigo, true);

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                            {
                                bool retorno = RetornarCTeCargaPedido(cargaPedido, cargaCTe, codigoFilial, integraCargasSoapClient, inspector, ref clientRequestContent, ref clientResponseContent, out mensagemErro, unitOfWork);

                                if (!retorno)
                                {
                                    situacaoIntegracao = false;

                                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                    arquivoIntegracao.Data = DateTime.Now;
                                    arquivoIntegracao.Mensagem = mensagemErro;
                                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientRequestContent, "xml", unitOfWork);
                                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "xml", unitOfWork);

                                    await repositorioCargaCTeIntegracaoArquivo.InserirAsync(arquivoIntegracao);

                                    cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                    cargaIntegracao.ProblemaIntegracao = mensagemErro;
                                    await repositorioCargaIntegracao.AtualizarAsync(cargaIntegracao);
                                    break;
                                }
                                else
                                {
                                    try
                                    {
                                        situacaoIntegracao = true;

                                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                        arquivoIntegracao.Data = DateTime.Now;
                                        arquivoIntegracao.Mensagem = $"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} Integrado com Sucesso!";
                                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientRequestContent, "xml", unitOfWork);
                                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "xml", unitOfWork);

                                        await repositorioCargaCTeIntegracaoArquivo.InserirAsync(arquivoIntegracao);

                                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                                    }
                                    catch (Exception excecao)
                                    {
                                        Servicos.Log.TratarErro(excecao, "IntegracaoMinerva");

                                        mensagemErro = "Falha de conunicação com o WebService da Minerva, verifique com a Minerva."; ;
                                        situacaoIntegracao = false;

                                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                        arquivoIntegracao.Data = DateTime.Now;
                                        arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;


                                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientRequestContent, "xml", unitOfWork);
                                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "xml", unitOfWork);

                                        await repositorioCargaCTeIntegracaoArquivo.InserirAsync(arquivoIntegracao);

                                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                                        Servicos.Log.TratarErro(clientResponseContent, "IntegracaoMinerva");

                                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                        cargaIntegracao.ProblemaIntegracao = mensagemErro;
                                        await repositorioCargaIntegracao.AtualizarAsync(cargaIntegracao);

                                        break;
                                    }
                                }
                            }

                            if (!situacaoIntegracao)
                                break;
                        }
                    }
                }

                if (!situacaoIntegracao)
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = mensagemErro;
                }
                else
                {
                    cargaIntegracao.ProblemaIntegracao = string.Empty;
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
            }

            await repositorioCargaIntegracao.AtualizarAsync(cargaIntegracao);

            Servicos.Log.GravarInfo($"Finalizado {codigoCargaIntegracao}", $"IntegracaoMinerva");
        }

        public void IntegrarLancamentoNFSManual(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualIntegracaoCTe)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repositorioNfsManualCteIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLMinerva))
            {
                nfsManualIntegracaoCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                nfsManualIntegracaoCTe.ProblemaIntegracao = "Não existe configuração de integração disponível para a Minerva.";
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> listaDocumentosNFSManual = repCargaDocumentoParaEmissaoNFSManual.BuscarPorLancamentoNFsManual(nfsManualIntegracaoCTe.LancamentoNFSManual.Codigo);

                bool integrou = true;
                string mensagemErro = string.Empty;
                nfsManualIntegracaoCTe.NumeroTentativas += 1;
                nfsManualIntegracaoCTe.DataIntegracao = DateTime.Now;

                using (ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoMinerva.IntegraCargas.IntegraCargasSoapClient, ServicoMinerva.IntegraCargas.IntegraCargasSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Minerva_IntegraCargas, out Models.Integracao.InspectorBehavior inspector))
                {
                    //foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedidos)
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documentoNFSManual in listaDocumentosNFSManual)
                    {
                        var cargaPedido = documentoNFSManual.PedidoXMLNotaFiscal?.CargaPedido ?? null;

                        if (cargaPedido != null && !cargaPedido.PedidoSemNFe)
                        {
                            string xmlRequisicao = string.Empty;
                            string xmlRetorno = string.Empty;

                            string codigoFilial = cargaPedido.CargaOrigem != null && cargaPedido.CargaOrigem.Filial != null ? cargaPedido.CargaOrigem.Filial.CodigoFilialEmbarcador : cargaPedido.Carga.Filial?.CodigoFilialEmbarcador ?? "";

                            bool retorno = false;

                            if (documentoNFSManual.CargaOcorrencia != null)
                                retorno = IntegrarOcorrenciaNFSManual(documentoNFSManual.CargaOcorrencia, ref xmlRequisicao, ref xmlRetorno, out mensagemErro);
                            else
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCargaPedido(cargaPedido.Codigo);
                                retorno = RetornarCTeCargaPedido(cargaPedido, cargaCTe, codigoFilial, integraCargasSoapClient, inspector, ref xmlRequisicao, ref xmlRetorno, out mensagemErro, _unitOfWork);
                            }

                            try
                            {
                                AdicionarArquivoTransacaoIntegracaoNFSManual(nfsManualIntegracaoCTe, xmlRequisicao, xmlRetorno);

                                repositorioNfsManualCteIntegracao.Atualizar(nfsManualIntegracaoCTe);

                                if (retorno)
                                    nfsManualIntegracaoCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                else
                                {
                                    integrou = false;
                                    break;
                                }
                            }
                            catch (Exception excecao)
                            {
                                Servicos.Log.TratarErro(excecao, "IntegracaoMinerva");
                                mensagemErro = "Ocorreu uma falha ao comunicar com o Web Service da Minerva.";
                                integrou = false;

                                nfsManualIntegracaoCTe.ProblemaIntegracao = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;

                                AdicionarArquivoTransacaoIntegracaoNFSManual(nfsManualIntegracaoCTe, xmlRequisicao, xmlRetorno);

                                Servicos.Log.TratarErro(xmlRetorno, "IntegracaoMinerva");

                                break;
                            }
                        }
                    }

                    if (integrou)
                    {
                        nfsManualIntegracaoCTe.ProblemaIntegracao = string.Empty;
                        nfsManualIntegracaoCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                    {
                        nfsManualIntegracaoCTe.ProblemaIntegracao = mensagemErro;
                        nfsManualIntegracaoCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }

            repositorioNfsManualCteIntegracao.Atualizar(nfsManualIntegracaoCTe);
        }

        public void IntegrarOcorrenciaIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao?.URLMinerva))
            {
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Minerva.";
                repositorioOcorrenciaCTeIntegracao.Atualizar(integracao);
                return;
            }

            integracao.NumeroTentativas += 1;
            integracao.DataIntegracao = DateTime.Now;

            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            try
            {
                IntegrarOcorrencia(integracao, ref xmlRequisicao, ref xmlRetorno);

                integracao.ProblemaIntegracao = string.Empty;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                SalvarArquivoIntegracaoOcorrencia(integracao, xmlRequisicao, xmlRetorno, string.Empty);
            }
            catch (ServicoException excecao)
            {
                integracao.ProblemaIntegracao = excecao.Message;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                SalvarArquivoIntegracaoOcorrencia(integracao, xmlRequisicao, xmlRetorno, excecao.Message);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                Log.TratarErro(xmlRetorno);

                integracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Minerva.";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                SalvarArquivoIntegracaoOcorrencia(integracao, xmlRequisicao, xmlRetorno, "Falha ao retornar ocorrência para a Minerva");
            }

            repositorioOcorrenciaCTeIntegracao.Atualizar(integracao);
        }

        public void IntegrarOcorrenciaCancelamentoIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao integracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao repositorioOcorrenciaCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao?.URLMinerva))
            {
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Minerva.";
                repositorioOcorrenciaCancelamentoIntegracao.Atualizar(integracao);
                return;
            }

            integracao.NumeroTentativas += 1;
            integracao.DataIntegracao = DateTime.Now;

            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            try
            {
                IntegrarOcorrenciaCancelamento(integracao, ref xmlRequisicao, ref xmlRetorno);

                integracao.ProblemaIntegracao = string.Empty;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                SalvarArquivoIntegracaoOcorrenciaCancelamento(integracao, xmlRequisicao, xmlRetorno, string.Empty);
            }
            catch (ServicoException excecao)
            {
                integracao.ProblemaIntegracao = excecao.Message;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                SalvarArquivoIntegracaoOcorrenciaCancelamento(integracao, xmlRequisicao, xmlRetorno, excecao.Message);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                Log.TratarErro(xmlRetorno);

                integracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Minerva.";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                SalvarArquivoIntegracaoOcorrenciaCancelamento(integracao, xmlRequisicao, xmlRetorno, "Falha ao retornar o cancelamento da ocorrência para a Minerva");
            }

            repositorioOcorrenciaCancelamentoIntegracao.Atualizar(integracao);
        }

        public async Task VerificarCargaIntegracaoMinervaPendentesAsync(bool cargasEmLote)
        {
            _unitOfWork.FlushAndClear();

            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 5;

            if (!cargasEmLote)
                numeroRegistrosPorVez = 8;

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork, _cancellationToken);

            List<Task> tasks = new List<Task>();
            List<int> integracoesPendentes = await repositorioCargaCargaIntegracao.BuscarIntegracoesPendentesPorTipoIntegracaoAsync(cargasEmLote, numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, TipoIntegracao.Minerva);

            foreach (int integracaoPendente in integracoesPendentes)
                tasks.Add(Task.Run(() => IntegrarCargaAsync(integracaoPendente)));

            await Task.WhenAll(tasks);
        }

        public async Task VerificarCargaIntegracaoMinervaMinervaPendentesAtualizacaoSituacaoAsync(bool cargasEmLote)
        {
            _unitOfWork.FlushAndClear();

            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork, _cancellationToken);

            List<int> codigosCargas = await repositorioCargaCargaIntegracao.BuscarIntegracoesConcluidasPorTipoIntegracaoAsync(cargasEmLote, SituacaoCarga.AgIntegracao, "Codigo", "asc", numeroRegistrosPorVez, TipoIntegracao.Minerva);

            IntegracaoCarga servicoIntegracaoCarga = new IntegracaoCarga(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            foreach (int codigoCarga in codigosCargas)
                await servicoIntegracaoCarga.AtualizarSituacaoCargaIntegracaoAsync(codigoCarga);
        }

        public void IntegrarCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao?.URLMinerva))
            {
                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Minerva.";
                repCanhotoIntegracao.Atualizar(canhotoIntegracao);
                return;
            }

            canhotoIntegracao.DataIntegracao = DateTime.Now;
            canhotoIntegracao.NumeroTentativas++;

            string mensagem;
            bool sucesso = false;

            using (ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoMinerva.IntegraCargas.IntegraCargasSoapClient, ServicoMinerva.IntegraCargas.IntegraCargasSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Minerva_IntegraCargas, out Models.Integracao.InspectorBehavior inspector))
            {
                try
                {
                    ServicoMinerva.IntegraCargas.Canhoto dadosCanhoto = ObterDadosCanhoto(canhotoIntegracao);
                    ServicoMinerva.IntegraCargas.RetornoPadrao retorno = integraCargasSoapClient.GravarDadosCanhoto(dadosCanhoto);

                    if (retorno.Retorno)
                    {
                        sucesso = true;
                        mensagem = "Integrado com sucesso.";
                    }
                    else
                        mensagem = retorno.RetornoMsg;
                }
                catch (Exception ex)
                {
                    Log.TratarErro(ex);
                    mensagem = "Ocorreu uma falha ao integrar com a Minerva";
                }

                canhotoIntegracao.SituacaoIntegracao = sucesso ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = mensagem;

                servicoArquivoTransacao.Adicionar(canhotoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

                repCanhotoIntegracao.Atualizar(canhotoIntegracao);
            }
        }

        #endregion

        #region Métodos Privados

        private ServicoMinerva.IntegraCargas.Canhoto ObterDadosCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(_unitOfWork);

            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = canhotoIntegracao.Canhoto;
            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> listaHistorico = repCanhotoHistorico.BuscarPorCanhoto(canhoto.Codigo);

            string tipoEvento = "Aprovado";
            if (canhoto.SituacaoCanhoto == SituacaoCanhoto.Extraviado)
                tipoEvento = "EXTRAVIADO";
            else if (canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado)
                tipoEvento = "APROVADO";
            else if (listaHistorico != null && listaHistorico.Count > 0 && listaHistorico.Any(o => o.Observacao.ToLower().Contains("reverteu")))
                tipoEvento = "REVOGADO";
            else
                tipoEvento = canhoto.DescricaoDigitalizacao;

            ServicoMinerva.IntegraCargas.Canhoto dadoCanhoto = new ServicoMinerva.IntegraCargas.Canhoto
            {
                CodFilial = canhoto.Filial?.CodigoFilialEmbarcador.ToInt() ?? 0,
                ProtocoloCarga = canhoto.Carga?.Protocolo ?? 0,
                NumCarregamentoMulti = canhoto.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                IndTipoCanhoto = canhoto.TipoCanhoto.ObterDescricao(),
                DtaEmissao = canhoto.DataEmissao,
                DtaDigitalizacao = canhoto.DataDigitalizacao.HasValue ? canhoto.DataDigitalizacao.Value : DateTime.Today,
                Situacao = canhoto.SituacaoCanhoto.ObterDescricao(),
                SituacaoDigitalizacao = canhoto.SituacaoDigitalizacaoCanhoto.ObterDescricao(),
                ChaveAcesso = canhoto.XMLNotaFiscal?.Chave ?? string.Empty,
                DtaEvento = canhoto.DataDigitalizacao.HasValue ? canhoto.DataDigitalizacao.Value : DateTime.Now,
                TipoEvento = tipoEvento,
                UsuarioAprovacao = canhoto.UsuarioDigitalizacao?.Nome ?? string.Empty,
                NumeroCanhoto = canhoto.Numero.ToString()
            };

            return dadoCanhoto;
        }

        private Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo AdicionarArquivoTransacao(string xmlRequisicao, string xmlRetorno, DateTime data, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(xmlRequisicao) && string.IsNullOrWhiteSpace(xmlRetorno))
                return null;

            Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo repositorio = new Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequisicao, "xml", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "xml", _unitOfWork),
                Data = data,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorio.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        private void AdicionarArquivoTransacaoIntegracaoNFSManual(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualIntegracaoCTe, string xmlRequisicao, string xmlRetorno)
        {
            Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(xmlRequisicao, xmlRetorno, nfsManualIntegracaoCTe.DataIntegracao, nfsManualIntegracaoCTe.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (nfsManualIntegracaoCTe.ArquivosTransacao == null)
                nfsManualIntegracaoCTe.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo>();

            nfsManualIntegracaoCTe.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, ref string xmlRequisicao, ref string xmlResposta)
        {
            if (ocorrenciaCTeIntegracao?.CargaCTe == null)
                throw new ServicoException("Nenhuma integracao/ocorrencia localizada para retornar");

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            List<string> listaCTes = repositorioCargaCTe.BuscarCodigosCTesPorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo);

            if ((listaCTes == null) || (listaCTes.Count == 0))
                throw new ServicoException("Nenhum documento complementar localizado para retornar");

            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
            Servicos.CTe servicoCTE = new Servicos.CTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao aprovador = repCargaOcorrenciaAutorizacao.BuscarUltimoAprovadorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo, EtapaAutorizacaoOcorrencia.EmissaoOcorrencia);

            ServicoMinerva.IntegraCargas.CTeComplementar complementar = new ServicoMinerva.IntegraCargas.CTeComplementar()
            {
                ProtocoloCarga = ocorrenciaCTeIntegracao.CargaCTe.CargaOrigem.Codigo,
                ProtocoloOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.NumeroOcorrencia,
                TipoOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.Descricao,
                ComponenteFrete = ocorrenciaCTeIntegracao.CargaOcorrencia.ComponenteFrete.Descricao,
                ValorOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.ValorOcorrencia,
                ObsOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.Observacao,
                CodigoIntegracaoComponente = ocorrenciaCTeIntegracao.CargaOcorrencia.ComponenteFrete?.CodigoIntegracao ?? string.Empty,
                cTes = new ServicoMinerva.IntegraCargas.CTe[listaCTes.Count],
                CpfUsuario = aprovador?.Usuario.CPF,
                DataOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia,
                Provisionada = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.OcorrenciaProvisionada
            };

            for (var i = 0; i < listaCTes.Count; i++)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementar = repositorioCTe.BuscarPorCodigo(int.Parse(listaCTes[i]));

                if (cteComplementar == null)
                    continue;

                string xml = servicoCTE.ObterStringXMLAutorizacao(cteComplementar, _unitOfWork);

                ServicoMinerva.IntegraCargas.CTe cte = new ServicoMinerva.IntegraCargas.CTe()
                {
                    AliquotaICMS = cteComplementar.AliquotaICMS,
                    ChaveCTe = cteComplementar.Chave,
                    ValorFrete = cteComplementar.ValorFrete,
                    ValorICMS = cteComplementar.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && cteComplementar.CST != "60" ? cteComplementar.ValorICMS : 0,
                    ValorISS = cteComplementar.ISSRetido ? cteComplementar.ValorISS * -1 : cteComplementar.ValorISS,
                    XMLCTe = xml != null ? xml : string.Empty,
                    DataEmissao = cteComplementar.DataEmissao.HasValue ? cteComplementar.DataEmissao.Value : DateTime.Now
                };

                complementar.cTes[i] = cte;
            }

            using (ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoMinerva.IntegraCargas.IntegraCargasSoapClient, ServicoMinerva.IntegraCargas.IntegraCargasSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Minerva_IntegraCargas, out Models.Integracao.InspectorBehavior inspector))
            {
                ServicoMinerva.IntegraCargas.RetornoPadrao retorno = integraCargasSoapClient.IntegraCTeComplementar(complementar);

                xmlRequisicao = inspector.LastRequestXML;
                xmlResposta = inspector.LastResponseXML;

                if (!retorno.Retorno)
                    throw new ServicoException(retorno.RetornoMsg);
            }
        }

        private bool IntegrarOcorrenciaNFSManual(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, ref string xmlRequisicao, ref string xmlResposta, out string erro)
        {
            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                erro = string.Empty;

                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                List<string> listaCTes = repositorioCargaCTe.BuscarCodigosCTesPorOcorrencia(cargaOcorrencia.Codigo);

                if ((listaCTes == null) || (listaCTes.Count == 0))
                    throw new ServicoException("Nenhum documento complementar localizado para retornar");

                Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
                Servicos.CTe servicoCTE = new Servicos.CTe(_unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao aprovador = repCargaOcorrenciaAutorizacao.BuscarUltimoAprovadorOcorrencia(cargaOcorrencia.Codigo, EtapaAutorizacaoOcorrencia.EmissaoOcorrencia);

                ServicoMinerva.IntegraCargas.CTeComplementar complementar = new ServicoMinerva.IntegraCargas.CTeComplementar()
                {
                    ProtocoloCarga = cargaOcorrencia.Carga.Codigo,
                    ProtocoloOcorrencia = cargaOcorrencia.NumeroOcorrencia,
                    TipoOcorrencia = cargaOcorrencia.TipoOcorrencia.Descricao,
                    ComponenteFrete = cargaOcorrencia.ComponenteFrete.Descricao,
                    ValorOcorrencia = cargaOcorrencia.ValorOcorrencia,
                    ObsOcorrencia = cargaOcorrencia.Observacao,
                    CodigoIntegracaoComponente = cargaOcorrencia.ComponenteFrete?.CodigoIntegracao ?? string.Empty,
                    cTes = new ServicoMinerva.IntegraCargas.CTe[listaCTes.Count],
                    CpfUsuario = aprovador?.Usuario.CPF,
                    DataOcorrencia = cargaOcorrencia.DataOcorrencia,
                    Provisionada = cargaOcorrencia.TipoOcorrencia.OcorrenciaProvisionada
                };

                for (var i = 0; i < listaCTes.Count; i++)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementar = repositorioCTe.BuscarPorCodigo(int.Parse(listaCTes[i]));

                    if (cteComplementar == null)
                        continue;

                    string xml = servicoCTE.ObterStringXMLAutorizacao(cteComplementar, _unitOfWork);

                    ServicoMinerva.IntegraCargas.CTe cte = new ServicoMinerva.IntegraCargas.CTe()
                    {
                        AliquotaICMS = cteComplementar.AliquotaICMS,
                        ChaveCTe = cteComplementar.Chave,
                        ValorFrete = cteComplementar.ValorFrete,
                        ValorICMS = cteComplementar.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && cteComplementar.CST != "60" ? cteComplementar.ValorICMS : 0,
                        ValorISS = cteComplementar.ISSRetido ? cteComplementar.ValorISS * -1 : cteComplementar.ValorISS,
                        XMLCTe = xml != null ? xml : string.Empty,
                        DataEmissao = cteComplementar.DataEmissao.HasValue ? cteComplementar.DataEmissao.Value : DateTime.Now
                    };

                    complementar.cTes[i] = cte;
                }

                using (ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoMinerva.IntegraCargas.IntegraCargasSoapClient, ServicoMinerva.IntegraCargas.IntegraCargasSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Minerva_IntegraCargas, out inspector))
                {
                    ServicoMinerva.IntegraCargas.RetornoPadrao retorno = integraCargasSoapClient.IntegraCTeComplementar(complementar);

                    xmlRequisicao = inspector.LastRequestXML;
                    xmlResposta = inspector.LastResponseXML;

                    if (!retorno.Retorno)
                    {
                        erro = retorno.RetornoMsg;
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "IntegracaoMinerva");

                xmlRequisicao = inspector.LastRequestXML;
                xmlResposta = inspector.LastResponseXML;
                Servicos.Log.TratarErro(xmlRequisicao, "IntegracaoMinerva");
                Servicos.Log.TratarErro(xmlResposta, "IntegracaoMinerva");

                string erroResponse = string.Empty;
                try
                {
                    erroResponse = Utilidades.XML.ObterConteudoTag(xmlResposta, "faultstring", false);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex.Message);
                }
                if (!string.IsNullOrWhiteSpace(erroResponse))
                    erro = erroResponse;
                else if (!string.IsNullOrWhiteSpace(xmlResposta))
                    erro = "Falha de conunicação com o WebService da Minerva, verifique com a Minerva.";
                else
                    erro = "Falha na rotina de integração.";

                return false;
            }
        }

        private void IntegrarOcorrenciaCancelamento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao ocorrenciaCancelamentoIntegracao, ref string xmlRequisicao, ref string xmlResposta)
        {
            if (ocorrenciaCancelamentoIntegracao.OcorrenciaCancelamento.Ocorrencia.Carga == null)
                throw new ServicoException("A ocorrência não possui uma carga");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoMinerva.IntegraCargas.IntegraCargasSoapClient, ServicoMinerva.IntegraCargas.IntegraCargasSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Minerva_IntegraCargas, out Models.Integracao.InspectorBehavior inspector))
            {
                ServicoMinerva.IntegraCargas.RetornoPadrao retorno = integraCargasSoapClient.IntegrarCancelamentoOcorrencia(
                    ocorrenciaCancelamentoIntegracao.OcorrenciaCancelamento.Ocorrencia.Carga.Protocolo,
                    ocorrenciaCancelamentoIntegracao.OcorrenciaCancelamento.Ocorrencia.NumeroOcorrencia,
                    ocorrenciaCancelamentoIntegracao.OcorrenciaCancelamento.Usuario?.Descricao,
                    ocorrenciaCancelamentoIntegracao.OcorrenciaCancelamento.MotivoCancelamento
                );

                xmlRequisicao = inspector.LastRequestXML;
                xmlResposta = inspector.LastResponseXML;

                if (!retorno.Retorno)
                    throw new ServicoException(retorno.RetornoMsg);
            }
        }

        private bool RetornarCancelamentoCarga(out Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao, out string mensagem, int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            arquivoIntegracao = null;

            bool retorno = false;
            string xmlRequest = "";
            string xmlResponse = "";
            mensagem = "";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                if (carga != null)
                {
                    using (ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoMinerva.IntegraCargas.IntegraCargasSoapClient, ServicoMinerva.IntegraCargas.IntegraCargasSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Minerva_IntegraCargas, out Models.Integracao.InspectorBehavior inspector))
                    {
                        ServicoMinerva.IntegraCargas.CargaCancelamento cargaCancelamento = new ServicoMinerva.IntegraCargas.CargaCancelamento();

                        int.TryParse(carga.Filial?.CodigoFilialEmbarcador ?? "0", out int codigoFilial);

                        cargaCancelamento.CodFilial = codigoFilial;
                        cargaCancelamento.Protocolo_Carga = carga.Codigo;

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);
                        if (cargaPedidos != null && cargaPedidos.Count > 0)
                        {
                            cargaCancelamento.NumPedidoEmbarcador = new ServicoMinerva.IntegraCargas.ArrayOfInt();
                            for (var i = 0; i < cargaPedidos.Count; i++)
                                cargaCancelamento.NumPedidoEmbarcador.Add(cargaPedidos[i].Pedido?.Codigo ?? 0);
                        }

                        ServicoMinerva.IntegraCargas.RetornoPadrao retornoIntegracao = integraCargasSoapClient.IntegrarCancelamentoCarga(cargaCancelamento);

                        xmlRequest = inspector.LastRequestXML;
                        xmlResponse = inspector.LastResponseXML;

                        if (!retornoIntegracao.Retorno)
                        {
                            mensagem = retornoIntegracao.RetornoMsg;
                            retorno = false;
                        }
                        else
                            retorno = true;
                    }
                }
                else
                {
                    mensagem = "Carga protocolo " + codigoCarga + " não encontrada.";
                    retorno = false;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("RetonarCancelamentoCarga: " + ex, "IntegracaoMinerva");
                mensagem = "Falha ao enviar o cancelamento da carga protocolo " + codigoCarga;
                retorno = false;
            }

            if (!string.IsNullOrWhiteSpace(xmlRequest) || !string.IsNullOrWhiteSpace(xmlResponse))
            {
                arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", _unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", _unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                    Mensagem = mensagem
                };

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }

            return retorno;
        }

        private bool RetornarCTeCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, string codFilial, ServicoMinerva.IntegraCargas.IntegraCargasSoapClient integraCargasSoapClient, Servicos.Models.Integracao.InspectorBehavior inspector, ref string xmlRequest, ref string xmlResponse, out string erro, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                erro = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);

                Repositorio.XMLMDFe repXMLMDFe = new Repositorio.XMLMDFe(unitOfWork);
                Servicos.CTe svcCTE = new Servicos.CTe(unitOfWork);

                if (cargaCTe != null && cargaCTe.CTe != null)
                {
                    ServicoMinerva.IntegraCargas.DocumentosCarga[] documentosCargas = new ServicoMinerva.IntegraCargas.DocumentosCarga[1];
                    int.TryParse(cargaPedido.Pedido.NumeroPedidoEmbarcador, out int numeroPedidoEmbarcador);

                    ServicoMinerva.IntegraCargas.DocumentosCarga documento = new ServicoMinerva.IntegraCargas.DocumentosCarga();
                    int.TryParse(codFilial, out int codigoFilial);
                    documento.CodFilial = codigoFilial;
                    documento.NumPedidoEmbarcador = cargaPedido.Pedido.Codigo;
                    documento.cTe = new ServicoMinerva.IntegraCargas.CTe();
                    documento.cTe.ChaveCTe = cargaCTe.CTe.Chave;
                    documento.cTe.XMLCTe = svcCTE.ObterStringXMLAutorizacao(cargaCTe.CTe, unitOfWork) ?? string.Empty;
                    documento.cTe.ValorICMS = cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && cargaCTe.CTe.CST != "60" ? cargaCTe.CTe.ValorICMS : 0;
                    documento.cTe.AliquotaICMS = cargaCTe.CTe.AliquotaICMS;
                    documento.cTe.ValorISS = cargaCTe.CTe.ISSRetido ? cargaCTe.CTe.ValorISS * -1 : cargaCTe.CTe.ValorISS;
                    documento.cTe.AliquotaISS = cargaCTe.CTe.AliquotaISS;
                    documento.cTe.ValorFrete = cargaCTe.CTe.ValorAReceber;
                    documento.cTe.DataEmissao = cargaCTe.CTe.DataEmissao.Value;

                    documento.NumProtocolo = cargaCTe.CargaOrigem.Protocolo;
                    if (cargaCTe.CargaOrigem.Carregamento != null)
                        documento.NumCarregamento = cargaCTe.CargaOrigem.Carregamento.NumeroCarregamento;
                    if (cargaCTe.Carga.Redespacho != null)
                        documento.NumRedespacho = cargaCTe.Carga.Redespacho.NumeroRedespacho;

                    List<Dominio.Entidades.AverbacaoCTe> listaAverbacaoCTe = repAverbacaoCTe.BuscarPorCTeESituacao(cargaCTe.CTe.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso);
                    documento.Averbacao = new ServicoMinerva.IntegraCargas.DadosAverbacao();
                    if (listaAverbacaoCTe != null && listaAverbacaoCTe.Count > 0)
                    {
                        documento.Averbacao.Apolice = listaAverbacaoCTe.FirstOrDefault().ApoliceSeguroAverbacao?.ApoliceSeguro?.NumeroApolice ?? string.Empty;
                        documento.Averbacao.NumAverbacao = listaAverbacaoCTe.FirstOrDefault().Averbacao;
                        documento.Averbacao.Averbadora = listaAverbacaoCTe.FirstOrDefault().ApoliceSeguroAverbacao?.ApoliceSeguro?.Seguradora?.Descricao ?? string.Empty;
                        documento.Averbacao.MsgRetorno = listaAverbacaoCTe.FirstOrDefault().MensagemRetorno;
                        documento.Averbacao.DtaRetorno = listaAverbacaoCTe.FirstOrDefault().DataRetorno.HasValue ? listaAverbacaoCTe.FirstOrDefault().DataRetorno.Value : DateTime.Now;
                    }
                    else
                    {
                        documento.Averbacao.Apolice = string.Empty;
                        documento.Averbacao.NumAverbacao = string.Empty;
                        documento.Averbacao.Averbadora = string.Empty;
                        documento.Averbacao.MsgRetorno = string.Empty;
                        documento.Averbacao.DtaRetorno = DateTime.Now;
                    }


                    documento.Pedagio = new ServicoMinerva.IntegraCargas.DadosPedagio();
                    List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaValePedagio = repCargaIntegracaoValePedagio.BuscarPorCarga(cargaCTe.Carga.Codigo, SituacaoIntegracao.Integrado);
                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio valePedagio = listaValePedagio.FirstOrDefault();
                    if (valePedagio != null)
                    {
                        documento.Pedagio.NumValePedagio = valePedagio.NumeroValePedagio;
                        documento.Pedagio.Integradora = valePedagio.TipoIntegracao.Descricao;
                        documento.Pedagio.SituacaoPedagio = valePedagio.SituacaoValePedagio.ObterDescricao();
                        documento.Pedagio.ValorPedagio = valePedagio.ValorValePedagio;
                        documento.Pedagio.Tentativas = valePedagio.NumeroTentativas;
                        documento.Pedagio.SituacaoIntegracao = valePedagio.DescricaoSituacaoIntegracao;
                        documento.Pedagio.DtaRetorno = valePedagio.DataIntegracao;
                        documento.Pedagio.MsgRetorno = valePedagio.ProblemaIntegracao;
                    }
                    else
                    {
                        documento.Pedagio.NumValePedagio = string.Empty;
                        documento.Pedagio.Integradora = string.Empty;
                        documento.Pedagio.SituacaoPedagio = string.Empty;
                        documento.Pedagio.ValorPedagio = 0;
                        documento.Pedagio.Tentativas = 0;
                        documento.Pedagio.SituacaoIntegracao = string.Empty;
                        documento.Pedagio.DtaRetorno = DateTime.Now;
                    }

                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigoCTe(cargaCTe.CTe.Codigo);
                    documento.mDFe = new ServicoMinerva.IntegraCargas.MDFe();
                    if (mdfe != null)
                    {
                        Servicos.MDFe svcMDFe = new Servicos.MDFe();
                        Dominio.Entidades.XMLMDFe xMLMDFe = repXMLMDFe.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao);
                        string xmlMDFe = svcMDFe.ObterStringXMLAutorizacao(xMLMDFe);
                        documento.mDFe.ChaveMDFe = mdfe.Chave;
                        documento.mDFe.XMLMDFe = xmlMDFe;
                        documento.mDFe.UFCarregamento = mdfe.EstadoCarregamento.Sigla;
                        documento.mDFe.UFDescarregamento = mdfe.EstadoDescarregamento.Sigla;
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> listaComponentesCTes = repCargaCTeComponentesFrete.BuscarPorCargaCTe(cargaCTe.Codigo);
                    if (listaComponentesCTes != null && listaComponentesCTes.Count > 0)
                    {
                        documento.ComponentesDoFrete = new ServicoMinerva.IntegraCargas.ComponentesFrete[listaComponentesCTes.Count];
                        for (var i = 0; i < listaComponentesCTes.Count; i++)
                        {
                            ServicoMinerva.IntegraCargas.ComponentesFrete componente = new ServicoMinerva.IntegraCargas.ComponentesFrete();
                            componente.DescontarValorTotalAReceber = listaComponentesCTes[i].DescontarValorTotalAReceber;
                            componente.DescricaoComponente = listaComponentesCTes[i].ComponenteFrete.Descricao;
                            componente.IncluirBaseCalculoICMS = listaComponentesCTes[i].IncluirBaseCalculoICMS;
                            componente.TipoComponente = listaComponentesCTes[i].DescricaoComponente;
                            componente.ValorComponente = listaComponentesCTes[i].ValorComponente;
                            componente.CodigoIntegracao = listaComponentesCTes[i].ComponenteFrete.CodigoIntegracao;

                            documento.ComponentesDoFrete[i] = componente;
                        }
                    }
                    else
                    {
                        documento.ComponentesDoFrete = new ServicoMinerva.IntegraCargas.ComponentesFrete[1];
                        ServicoMinerva.IntegraCargas.ComponentesFrete componente = new ServicoMinerva.IntegraCargas.ComponentesFrete();
                        componente.DescontarValorTotalAReceber = false;
                        componente.DescricaoComponente = string.Empty;
                        componente.IncluirBaseCalculoICMS = false;
                        componente.TipoComponente = string.Empty;
                        componente.ValorComponente = 0;
                        componente.CodigoIntegracao = "";

                        documento.ComponentesDoFrete[0] = componente;
                    }

                    documentosCargas[0] = documento;

                    ServicoMinerva.IntegraCargas.RetornoPadrao retorno = integraCargasSoapClient.IntegrarDocumentosCarga(documentosCargas);

                    xmlRequest = inspector.LastRequestXML;
                    xmlResponse = inspector.LastResponseXML;

                    if (!retorno.Retorno)
                    {
                        erro = retorno.RetornoMsg;
                        return false;
                    }
                    else
                        return true;
                }
                else
                {
                    Servicos.Log.TratarErro("Carga pedido " + cargaPedido.Codigo.ToString() + " sem CTe.", "IntegracaoMinerva");
                    return false;
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "IntegracaoMinerva");

                xmlRequest = inspector.LastRequestXML;
                xmlResponse = inspector.LastResponseXML;
                Servicos.Log.TratarErro(xmlRequest, "IntegracaoMinerva");
                Servicos.Log.TratarErro(xmlResponse, "IntegracaoMinerva");

                string erroResponse = xmlResponse != null ? Utilidades.XML.ObterConteudoTag(xmlResponse, "faultstring", false) : "";
                if (!string.IsNullOrWhiteSpace(erroResponse))
                    erro = erroResponse;
                else if (!string.IsNullOrWhiteSpace(xmlResponse))
                    erro = "Falha de conunicação com o WebService da Minerva, verifique com a Minerva.";
                else
                    erro = "Falha na rotina de integração.";

                return false;
            }
        }

        private void SalvarArquivoIntegracaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, string xmlRequisicao, string xmlResposta, string mensagemErro)
        {
            if (string.IsNullOrWhiteSpace(xmlRequisicao) && string.IsNullOrWhiteSpace(xmlResposta))
                return;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repositorioOcorrenciaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo()
            {
                Data = integracao.DataIntegracao,
                Mensagem = mensagemErro,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequisicao, "xml", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(xmlResposta, "xml", _unitOfWork)
            };

            repositorioOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (integracao.ArquivosTransacao == null)
                integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>();

            integracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void SalvarArquivoIntegracaoOcorrenciaCancelamento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao integracao, string xmlRequisicao, string xmlResposta, string mensagemErro)
        {
            if (string.IsNullOrWhiteSpace(xmlRequisicao) && string.IsNullOrWhiteSpace(xmlResposta))
                return;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo repositorioOcorrenciaCancelamentoIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo()
            {
                Data = integracao.DataIntegracao,
                Mensagem = mensagemErro,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequisicao, "xml", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(xmlResposta, "xml", _unitOfWork)
            };

            repositorioOcorrenciaCancelamentoIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (integracao.ArquivosTransacao == null)
                integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo>();

            integracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        #endregion
    }
}
