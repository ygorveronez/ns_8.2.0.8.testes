using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.CargaOcorrencia
{
    public class Ocorrencia : ServicoBase
    {
        #region Construtores
        
        public Ocorrencia() : base() { }
        public Ocorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Atributos Privados

        private Servicos.LeituraEDI _leituraEDI;
        private Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN _ocorren;
        private bool _threadExecutada = false;

        #endregion

        #region Métodos Públicos

        public async Task<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> GerarOcorrenciaNotasFiscais(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSManual, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, DateTime dataOcorrencia, string observacao, decimal valor, string latitude, string longitude, decimal quilometragem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
            Servicos.Embarcador.Carga.Ocorrencia servicoOcorrenciaCalculoFrete = new Servicos.Embarcador.Carga.Ocorrencia();

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia calculoFreteOcorrencia = null;

            if (tipoDeOcorrencia.CalculaValorPorTabelaFrete)
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia()
                {
                    CodigoCarga = carga.Codigo,
                    CodigoTipoOcorrencia = tipoDeOcorrencia.Codigo,
                    ListaCargaCTe = await srvOcorrencia.BuscarCTesSelecionadosOuCargas(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes(), carga, configuracao, unitOfWork, "", true, 0, 0, tipoServicoMultisoftware, usuario, 0),
                    KmInformado = (int)Math.Round(quilometragem)
                };

                calculoFreteOcorrencia = servicoOcorrenciaCalculoFrete.CalcularValorOcorrencia(parametrosCalcularValorOcorrencia, unitOfWork, configuracao, tipoServicoMultisoftware);
            }

            cargaOcorrencia.DataOcorrencia = dataOcorrencia;
            cargaOcorrencia.DataAlteracao = DateTime.Now;
            cargaOcorrencia.DataFinalizacaoEmissaoOcorrencia = DateTime.Now;
            cargaOcorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            cargaOcorrencia.Observacao = observacao;
            if (cargaOcorrencia.Observacao is null)
                cargaOcorrencia.Observacao = "";

            //cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada;
            cargaOcorrencia.ValorOcorrencia = calculoFreteOcorrencia != null && calculoFreteOcorrencia.ValorOcorrencia > 0 ? calculoFreteOcorrencia.ValorOcorrencia : valor;
            cargaOcorrencia.Quilometragem = quilometragem;
            cargaOcorrencia.ValorOcorrenciaOriginal = valor;
            cargaOcorrencia.Latitude = latitude;
            cargaOcorrencia.Longitude = longitude;
            cargaOcorrencia.ObservacaoCTe = "";
            cargaOcorrencia.ObservacaoCTes = "";
            cargaOcorrencia.Carga = carga;
            cargaOcorrencia.TipoOcorrencia = tipoDeOcorrencia;
            cargaOcorrencia.OrigemOcorrencia = cargaOcorrencia.TipoOcorrencia.OrigemOcorrencia;
            cargaOcorrencia.ComponenteFrete = tipoDeOcorrencia?.ComponenteFrete;
            cargaOcorrencia.IncluirICMSFrete = cargaCTEs.Count > 0 ? (cargaCTEs.FirstOrDefault().CTe != null ? (cargaCTEs.FirstOrDefault().CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false) : false) : false;

            repCargaOcorrencia.Inserir(cargaOcorrencia);

            Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(cargaOcorrencia, cargaCTEs, unitOfWork);

            string mensagemRetorno = string.Empty;
            bool tudoCerto = srvOcorrencia.FluxoGeralOcorrencia(ref cargaOcorrencia, cargaCTEs, cargaDocumentosParaEmissaoNFSManual, ref mensagemRetorno, unitOfWork, tipoServicoMultisoftware, usuario, configuracao, clienteMultisoftware, "", true);

            if (!tudoCerto || !string.IsNullOrWhiteSpace(mensagemRetorno))
                throw new ServicoException(mensagemRetorno);

            return tudoCerto ? cargaOcorrencia : null;
        }

        public bool GerarPedidoOcorrenciaNotas(List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscaisCarga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataOcorrencia, string observacao, string latitude, string longitude, OrigemSituacaoEntrega origemSituacaoEntrega, string pacote, int volumes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntregaIntegracao = null)
        {
            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente = Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in pedidoXMLNotaFiscaisCarga select obj.CargaPedido).Distinct().ToList();
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repositorioPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (!string.IsNullOrWhiteSpace(pacote))
                {
                    if (repositorioPedidoOcorrenciaColetaEntrega.BuscarPorPedidoPacoteTipoOcorrencia(cargaPedido.Pedido.Codigo, pacote, tipoDeOcorrencia.Codigo) != null)
                        return false;
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.EventoCargaEntregaAdicionar eventoCargaEntregaAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.EventoCargaEntregaAdicionar()
            {
                Carga = carga,
                CodigosCargaPedidos = (from o in cargaPedidos where o.Carga.Codigo == carga.Codigo select o.Codigo).Distinct().ToList(),
                DataOcorrencia = dataOcorrencia,
                Latitude = latitude,
                Longitude = longitude,
                TipoOcorrencia = tipoDeOcorrencia,
                Usuario = usuario,
                Pacote = pacote,
                Volumes = volumes,
                ValidarNotasFiscaisFinalizadas = true
            };

            bool gerouOcorrenciaPedido = Carga.ControleEntrega.OcorrenciaEntrega.GerarEventoCargaEntrega(eventoCargaEntregaAdicionar, configuracao, origemSituacaoEntrega, tipoServicoMultisoftware, auditado, unitOfWork, clienteMultisoftware, pacote, volumes);
            if (!gerouOcorrenciaPedido)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    Dominio.Entidades.Cliente tomador = cargaPedido.Pedido.ObterTomador() ?? cargaPedido.Pedido.Remetente;
                    if (cargaPedido.Pedido.CotacaoPedido != null)
                        tomador = cargaPedido.Pedido.Remetente;

                    Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarPedidoOcorrenciaColetaEntrega(tomador, cargaPedido.Pedido, carga, tipoDeOcorrencia, configuracaoPortalCliente, dataOcorrencia, observacao, pacote, volumes, configuracao, clienteMultisoftware, unitOfWork, pedidoOcorrenciaColetaEntregaIntegracao);
                }
            }
            return true;
        }

        public string ProcessarOcorren(Dominio.Entidades.LayoutEDI layoutEDI, Stream arquivo, string nomeArquivo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Usuario usuario, TipoEnvioArquivo tipoEnvioArquivo, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            try
            {
                _leituraEDI = new Servicos.LeituraEDI(null, layoutEDI, arquivo, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.GetEncoding("iso-8859-1"));

                if (!ThreadGerarOcorrencia())
                    return "Ocorreu uma falha ao ler o arquivo.";

                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia repositorioLogLeituraArquivoOcorrencia = new Repositorio.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia logLeituraArquivoOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia()
                {
                    NomeArquivo = nomeArquivo,
                    GuidArquivo = Guid.NewGuid().ToString(),
                    TipoEnvioArquivo = tipoEnvioArquivo,
                    DataRecebimento = DateTime.Now,
                    Usuario = usuario,
                    OcorrenciasGeradas = "",
                    MotivoInconsistencia = ""
                };


                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, "OcorenRecebidos");

                arquivo.Position = 0;

                Encoding encoding = (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().CodificacaoEDI == "UTF8") ? Encoding.UTF8 : Encoding.GetEncoding("iso-8859-1");

                using (StreamReader readerArquivo = new StreamReader(arquivo, encoding))
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(Utilidades.IO.FileStorageService.Storage.Combine(caminho, logLeituraArquivoOcorrencia.GuidArquivo), readerArquivo.ReadToEnd());

                repositorioLogLeituraArquivoOcorrencia.Inserir(logLeituraArquivoOcorrencia);

                int codigoLogLeituraArquivoOcorrencia = logLeituraArquivoOcorrencia.Codigo;

                foreach (Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento cabecalho in _ocorren.CabecalhosDocumento)
                {
                    int codigoEmpresa = repositorioEmpresa.BuscarCodigoPorCNPJ(cabecalho.Transportador.Pessoa.CPFCNPJ);

                    if (codigoEmpresa <= 0)
                        codigoEmpresa = repositorioEmpresa.BuscarCodigoPorCNPJ(cabecalho.Transportador.Pessoa.CPFCNPJSemFormato);

                    if (codigoEmpresa <= 0)
                    {
                        string motivoInconsistencia = $"Não existe uma transportadora cadastrada com o cnpj {cabecalho.Transportador.Pessoa.CPFCNPJ} na base.";
                        AtualizarLogLeituraArquivoOcorrencia(codigoLogLeituraArquivoOcorrencia, codigoEmpresa: 0, motivoInconsistencia, numeroOcorrenciaGerada: 0, unitOfWork);
                        return motivoInconsistencia;
                    }

                    AtualizarLogLeituraArquivoOcorrencia(codigoLogLeituraArquivoOcorrencia, codigoEmpresa, motivoInconsistencia: string.Empty, numeroOcorrenciaGerada: 0, unitOfWork);

                    List<string> codigosOcorrencia = (from notaFiscal in cabecalho.Transportador.NotasFiscais select notaFiscal.CodigoOcorrenciaEntrega).Distinct().ToList();

                    foreach (string codigoOcorrencia in codigosOcorrencia)
                    {
                        Repositorio.Global.EmpresaIntelipostTipoOcorrencia repEmpresaIntelipostTipoOcorrencia = new Repositorio.Global.EmpresaIntelipostTipoOcorrencia(unitOfWork);
                        Dominio.Entidades.EmpresaIntelipostTipoOcorrencia empresaIntelipostTipoOcorrencia = repEmpresaIntelipostTipoOcorrencia.BuscarPorEmpresaECodigoIntegracao(codigoEmpresa, codigoOcorrencia);
                        Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = empresaIntelipostTipoOcorrencia?.TipoOcorrencia;

                        if (tipoDeOcorrencia == null)
                            tipoDeOcorrencia = repositorioTipoDeOcorrenciaDeCTe.BuscarPorCodigoIntegracao(codigoOcorrencia);

                        if (tipoDeOcorrencia == null)
                        {
                            string motivoInconsistencia = $"Não existe uma ocorrência cadastrada com o código de integração {codigoOcorrencia} na base.";
                            AtualizarLogLeituraArquivoOcorrencia(codigoLogLeituraArquivoOcorrencia, codigoEmpresa, motivoInconsistencia, numeroOcorrenciaGerada: 0, unitOfWork);
                            return motivoInconsistencia;
                        }

                        List<Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia> notasFiscaisPorOcorrencia = (from notaFiscal in cabecalho.Transportador.NotasFiscais where notaFiscal.CodigoOcorrenciaEntrega == codigoOcorrencia select notaFiscal).ToList();
                        List<DateTime> datasOcorrencias = (from notaFiscal in notasFiscaisPorOcorrencia select notaFiscal.DataOcorrencia).Distinct().ToList();
                        bool possuiNotasComDataOcorrencia = (datasOcorrencias.Count > 0);

                        if (!possuiNotasComDataOcorrencia)
                            datasOcorrencias.Add(DateTime.MinValue);

                        for (int i = 0; i < datasOcorrencias.Count; i++)
                        {
                            DateTime dataOcorrencia = datasOcorrencias[i];
                            List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoXmlNotaFiscal> pedidoXmlNotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoXmlNotaFiscal>();
                            List<Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia> notasFiscaisPorDataOcorrencia = null;

                            if (possuiNotasComDataOcorrencia)
                                notasFiscaisPorDataOcorrencia = (from notaFiscal in notasFiscaisPorOcorrencia where notaFiscal.DataOcorrencia == dataOcorrencia select notaFiscal).ToList();
                            else
                                notasFiscaisPorDataOcorrencia = notasFiscaisPorOcorrencia;

                            foreach (Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia notaFiscal in notasFiscaisPorDataOcorrencia)
                            {
                                double cnpjRemetente = notaFiscal.CNPJEmissorNotaFiscal.ObterSomenteNumeros().ToDouble();
                                Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoXmlNotaFiscal pedidoXmlNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorNumeroNotaEmitenteTransportador(notaFiscal.NumeroNotaFiscal, codigoEmpresa, cnpjRemetente);

                                if (pedidoXmlNotaFiscal != null)
                                    pedidoXmlNotasFiscais.Add(pedidoXmlNotaFiscal);
                            }

                            List<int> codigosCargas = (from pedidoXmlNotaFiscal in pedidoXmlNotasFiscais select pedidoXmlNotaFiscal.CodigoCarga).Distinct().ToList();

                            foreach (int codigoCarga in codigosCargas)
                            {
                                try
                                {
                                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                                    List<int> codigosPedidoXmlNotasFiscais = (from pedidoXmlNotaFiscal in pedidoXmlNotasFiscais where pedidoXmlNotaFiscal.CodigoCarga == carga.Codigo select pedidoXmlNotaFiscal.Codigo).ToList();
                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXmlNotasFiscaisPorCarga = repositorioPedidoXMLNotaFiscal.BuscarPorCodigos(codigosPedidoXmlNotasFiscais);
                                    DateTime dataOcorrenciaParaGeracao = (dataOcorrencia == DateTime.MinValue) ? DateTime.Now : dataOcorrencia;

                                    if (!configuracao.PedidoOcorrenciaColetaEntregaIntegracaoNova)
                                    {
                                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                                        bool retornarPreCtes = ((carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos) && carga.AgImportacaoCTe) || tipoDeOcorrencia.PermitirEnviarOcorrenciaSemAprovacaoPreCTe;

                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXmlNotasFiscaisPorCarga)
                                            cargaCTEs.AddRange(repositorioCargaCte.BuscarPorCarga(carga.Codigo, true, true, pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ, pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ, retornarPreCtes));

                                        List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSManual = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCargaENotaFiscal(carga.Codigo, (from obj in pedidoXmlNotasFiscaisPorCarga select obj.XMLNotaFiscal.Codigo).ToList());

                                        cargaCTEs = cargaCTEs.Distinct().ToList();

                                        if (cargaCTEs.Count == 0 && cargaDocumentosParaEmissaoNFSManual.Count == 0)
                                            continue;

                                        unitOfWork.Start();

                                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = GerarOcorrenciaNotasFiscais(tipoDeOcorrencia, carga, cargaDocumentosParaEmissaoNFSManual, cargaCTEs, dataOcorrenciaParaGeracao, "", 0, "", "", 0, tipoServicoMultisoftware, configuracao, usuario, clienteMultisoftware, unitOfWork).Result;

                                        unitOfWork.CommitChanges();

                                        AtualizarLogLeituraArquivoOcorrencia(codigoLogLeituraArquivoOcorrencia, codigoEmpresa, motivoInconsistencia: string.Empty, cargaOcorrencia.NumeroOcorrencia, unitOfWork);
                                    }
                                    else
                                    {
                                        unitOfWork.Start();

                                        GerarPedidoOcorrenciaNotas(pedidoXmlNotasFiscaisPorCarga, tipoDeOcorrencia, carga, dataOcorrenciaParaGeracao, "", "", "", OrigemSituacaoEntrega.ArquivoEDI, "", 0, tipoServicoMultisoftware, configuracao, usuario, clienteMultisoftware, unitOfWork, auditado);

                                        unitOfWork.CommitChanges();
                                    }
                                }
                                catch (BaseException excecao)
                                {
                                    unitOfWork.Rollback();
                                    AtualizarLogLeituraArquivoOcorrencia(codigoLogLeituraArquivoOcorrencia, codigoEmpresa, motivoInconsistencia: excecao.Message, numeroOcorrenciaGerada: 0, unitOfWork);
                                }
                                catch (Exception excecao)
                                {
                                    unitOfWork.Rollback();
                                    Log.TratarErro($"ProcessarOcorren (Empresa: {codigoEmpresa} | Ocorrência: {codigoOcorrencia} | Data: {dataOcorrencia.ToDateTimeString()} | Carga: {codigoCarga}): {excecao}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception excecaoGenerica)
            {
                unitOfWork.Rollback();
                Log.TratarErro(excecaoGenerica);
                return "Erro ao processar arquivo.";
            }

            return "";
        }

        public decimal CalcularValorOcorrenciaPorTipoOcorrencia(int codigoTipoOcorrencia, DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, dynamic cargasComplementadasDias, out string erro)
        {
            erro = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork);

            // Valida dados de busca
            if (periodoInicial == DateTime.MinValue || periodoFim == DateTime.MinValue)
            {
                erro = "O período não foi definido.";
                return 0;
            }

            if (periodoInicial > periodoFim)
            {
                erro = "O período selecionado é inválido.";
                return 0;
            }

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoDeOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia);

            if (tipoOcorrencia == null)
            {
                erro = "Não foi possível buscar os dados do tipo da ocorrência.";
                return 0;
            }

            // Busca cargas sumarizadas
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado> cargasSumarizadas = repCargaOcorrenciaSumarizado.ConsultarCargasOcorrenciaPorPeriodo(periodoInicial, periodoFim, transportador, filial, proprietario, tipoOcorrencia.FiltrarCargasPeriodo, tipoOcorrencia.Codigo, "", "", 0, 0);

            // Valida consulta
            if (cargasSumarizadas.Count == 0)
            {
                erro = "O período selecionado não possui nenhuma carga.";
                return 0;
            }

            decimal valorDiaria = tipoOcorrencia.ValorBase;

            if (cargasSumarizadas.Where(o => o.ModeloVeicular == null).Count() > 0)
            {
                erro = "Veículo(s) sem modelo veícular cadastrado.";
                return 0;
            }


            return this.RegraCalculoValor(SubstituiDiasPorValoresInformados(cargasSumarizadas, cargasComplementadasDias), valorDiaria);
        }

        public decimal CalcularValorOcorrenciaPorTipoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado> cargasSumarizadas = repCargaOcorrenciaSumarizado.BuscarCargasOcorrenciaPorOcorrencia(ocorrencia.Codigo);
            decimal valorDiaria = ocorrencia.TipoOcorrencia.ValorBase;

            return this.RegraCalculoValor(ConverterEntidadesEmObjetosDeValor(cargasSumarizadas), valorDiaria);
        }

        public static void SalvarVeiculosImprodutivos(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo repOcorrenciaContratoVeiculo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo> veiculos = repCargaPercurso.ConsultarVeiculosImprodutivos(ocorrencia.PeriodoInicio.Value, ocorrencia.PeriodoFim.Value, ocorrencia.ContratoFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo veiculo in veiculos)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo veiculoOcorrenica = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo()
                {
                    Ocorrencia = ocorrencia,
                    Veiculo = veiculo.Veiculo
                };

                repOcorrenciaContratoVeiculo.Inserir(veiculoOcorrenica);
            }
        }

        public static List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo> SalvarVeiculosContrato(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, System.Web.HttpRequestBase Request, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo repOcorrenciaContratoVeiculo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            // Converte valores vindo do cliente
            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ContratoVeiculo> ocorrenciaContratoVeiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ContratoVeiculo>>(Request.Params["VeiculosContrato"]);

            // Retorno dos dados
            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo> veiculos = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo>();

            // Cria um registro no banco pra cada 
            foreach (Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ContratoVeiculo objVeiculo in ocorrenciaContratoVeiculos)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo veiculo = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo()
                {
                    Ocorrencia = ocorrencia,
                    Veiculo = repVeiculo.BuscarPorCodigo(objVeiculo.CodigoVeiculo),
                    QuantidadeDias = objVeiculo.QuantidadeDias,
                    ValorDiaria = objVeiculo.ValorDiaria,
                    ValorQuinzena = objVeiculo.ValorQuinzena,
                    Total = objVeiculo.Total,
                    QuantidadeDocumentos = objVeiculo.QuantidadeDocumentos,
                    ValorDocumentos = objVeiculo.ValorDocumentos,
                };
                if (veiculo.Veiculo != null)
                {
                    repOcorrenciaContratoVeiculo.Inserir(veiculo);
                    veiculos.Add(veiculo);
                }
            }

            return veiculos;
        }

        /// <summary>
        /// Cria os CT-es da ocorrência e envia para autorização
        /// </summary>
        public static void ExecutaProximoPassoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            if (ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.EmEmissaoCTeComplementar) //(ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgEmissaoCTeComplementar)
                return;

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            string codigoCFOPIntegracao = string.Empty;
            if (ocorrencia.Carga != null && ocorrencia.Carga.CargaCTes != null && ocorrencia.Carga.CargaCTes.Count > 0)
            {
                if (ocorrencia != null && ocorrencia.TipoOcorrencia != null)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (from o in ocorrencia.Carga.CargaCTes where o.CTe != null && o.CargaCTeComplementoInfo == null select o.CTe).FirstOrDefault();

                    if (cte != null && cte.LocalidadeInicioPrestacao.Estado.Sigla == cte.LocalidadeTerminoPrestacao.Estado.Sigla)
                    {
                        if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento))
                            codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento;
                        else
                            codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                    }
                    else if (cte != null)
                    {
                        if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento))
                            codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento;
                        else
                            codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual : !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                    }
                }

                if (!string.IsNullOrWhiteSpace(codigoCFOPIntegracao))
                {
                    ocorrencia.CFOP = codigoCFOPIntegracao;
                    repCargaOcorrencia.Atualizar(ocorrencia);
                }
            }

            Carga.Ocorrencia servicoOcorrencia = new Carga.Ocorrencia(unitOfWork);

            servicoOcorrencia.ValidarEnviarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork);

            AdicionarSaldoContratoPrestacaoServico(ocorrencia, unitOfWork);
        }

        public static Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista SalvarMotoristaContrato(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, System.Web.HttpRequestBase Request, Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoMotorista repOcorrenciaContratoMotorista = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoMotorista(unitOfWork);

            int.TryParse(Request.Params["QuantidadeMotorista"], out int quantidadeMotoristas);
            int.TryParse(Request.Params["QuantidadeDiasMotorista"], out int quantidadeDias);
            decimal total = contrato.ValorQuinzenaPorMotorista;
            if (total == 0)
                total = quantidadeMotoristas * quantidadeDias * contrato.ValorDiariaPorMotorista;

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista motorista = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista()
            {
                Ocorrencia = ocorrencia,
                QuantidadeDias = quantidadeDias,
                QuantidadeMotoristas = quantidadeMotoristas,
                Total = total,
                ValorDiaria = contrato.ValorDiariaPorMotorista,
                ValorQuinzena = contrato.ValorQuinzenaPorMotorista
            };
            repOcorrenciaContratoMotorista.Inserir(motorista);

            return motorista;
        }

        public static decimal CalcularValorOcorrenciaContrato(List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo> veiculosContrato, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista motoristaContrato)
        {
            decimal valorVeiculos = (from o in veiculosContrato select o.Total).Sum();
            decimal valorMotoristas = motoristaContrato.Total;
            decimal valorDescontos = (from o in veiculosContrato select o.ValorDocumentos).Sum();

            return valorVeiculos + valorMotoristas - valorDescontos;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesPorCargaEDTNatura(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in carga.CargaCTes)
            {
                if (
                    cargaCTe.CargaCTeComplementoInfo == null &&
                    cargaCTe.NotasFiscais.Any(n => dtNatura.NotasFiscais.Any(ndt =>
                        ndt.Chave == n.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave ||
                        (ndt.Numero == n.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero && ndt.Serie.ToString() == n.PedidoXMLNotaFiscal.XMLNotaFiscal.Serie))
                    )
                )
                {
                    cargaCTes.Add(cargaCTe);
                }
            }

            return cargaCTes;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> BuscarCTesSelecionadosOuCargas(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtro, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, string strCargaCTes, bool selecionarTodos, int numeroNF, double destinatario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, int numeroDocumento = 0, List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaCargaCTeNotaFiscal> listaOcorrenciaCargaCTeNotaFiscal = null)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<int> codigosCargaCTe = new List<int>();
            List<dynamic> listaSelecionados = !string.IsNullOrWhiteSpace(strCargaCTes) ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(strCargaCTes) : new List<dynamic>();

            ObterFiltroBuscarCTesSelecionadosOuCargas(filtro, carga, unitOfWork, tipoServicoMultisoftware, usuario);

            foreach (dynamic selecionado in listaSelecionados)
            {
                int codigoCargaCTe = selecionado.CodigoCargaCTe != null ? (int)selecionado.CodigoCargaCTe : (int)selecionado.Codigo;
                int codigoNotaFiscal = selecionado.CodigoNotaFiscal != null ? (int)selecionado.CodigoNotaFiscal : 0;

                codigosCargaCTe.Add(codigoCargaCTe);

                if (!selecionarTodos && listaOcorrenciaCargaCTeNotaFiscal != null && !listaOcorrenciaCargaCTeNotaFiscal.Any(o => o.CodigoCargaCTe == codigoCargaCTe && o.CodigoNotaFiscal == codigoNotaFiscal))
                    listaOcorrenciaCargaCTeNotaFiscal.Add(new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaCargaCTeNotaFiscal { CodigoCargaCTe = codigoCargaCTe, CodigoNotaFiscal = codigoNotaFiscal });
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = await repositorioCargaCTe.ObterRequisicoesConsultarCTes(filtro, selecionarTodos, codigosCargaCTe.ToList());

            return listaCargaCTe;
        }

        public void ObterFiltroBuscarCTesSelecionadosOuCargas(FiltroPesquisaMontarConsultaCtes filtro, Dominio.Entidades.Embarcador.Cargas.Carga carga, UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);

            bool permitirSelecionarCteApenasComNfeVinculadaOcorrencia = repConfiguracaoChamado.PermitirSelecionarCteApenasComNfeVinculadaOcorrenciao();

            string proprietarioVeiculo = string.Empty;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                proprietarioVeiculo = usuario?.ClienteTerceiro != null ? usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : null;

            filtro.BuscarPorCargaOrigem = !repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao().GerarOcorrenciaParaCargaAgrupada;
            filtro.RetornarPreCtes = (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos && carga.AgImportacaoCTe);
            filtro.TiposDocumentosDoCte = Dominio.Enumeradores.TipoDocumentoHelper.ObterTiposDocumentosPadraoCte();
            filtro.PermitirSelecionarCteApenasComNfeVinculadaOcorrencia = permitirSelecionarCteApenasComNfeVinculadaOcorrencia;
            filtro.ProprietarioVeiculo = proprietarioVeiculo;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasDoPeriodoSelecionado(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, int codigoTransportadora, int codigoFilial)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork);

            int transportador = 0;
            string proprietario = "";

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                Dominio.Entidades.Empresa empresaTerceiro = repEmpresa.BuscarPorCNPJ(usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);

                transportador = empresaTerceiro != null ? empresaTerceiro.Codigo : 0;
                proprietario = usuario.ClienteTerceiro != null ? usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : string.Empty;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                transportador = usuario.Empresa.Codigo;
                proprietario = usuario.Empresa != null ? usuario.Empresa.CNPJ_SemFormato : string.Empty;
            }
            else
            {
                transportador = codigoTransportadora;
                Dominio.Entidades.Empresa transportadoraSelecionada = repEmpresa.BuscarPorCodigo(transportador);
                proprietario = transportadoraSelecionada?.CNPJ_SemFormato ?? string.Empty;
            }

            int filial = codigoFilial;

            DateTime periodoInicial = ocorrencia.PeriodoInicio.Value;
            DateTime periodoFim = ocorrencia.PeriodoFim.Value;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaOcorrenciaSumarizado.ConsultarDocumentosCargasPorPeriodo(periodoInicial, periodoFim, transportador, filial, proprietario, 0, ocorrencia.TipoOcorrencia.FiltrarCargasPeriodo, ocorrencia.TipoOcorrencia.Codigo, "", "", 0, 0);

            return (from o in cargaCTes select o.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasDoPeriodoSelecionadoComVeiculo(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, int codigoTransportadora)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork);

            int transportador = 0;
            string proprietario = "";

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                Dominio.Entidades.Empresa empresaTerceiro = repEmpresa.BuscarPorCNPJ(usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);

                transportador = empresaTerceiro != null ? empresaTerceiro.Codigo : 0;
                proprietario = usuario.ClienteTerceiro != null ? usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : string.Empty;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                transportador = usuario.Empresa.Codigo;
                proprietario = usuario.Empresa != null ? usuario.Empresa.CNPJ_SemFormato : string.Empty;
            }
            else
            {
                transportador = codigoTransportadora;
                Dominio.Entidades.Empresa transportadoraSelecionada = repEmpresa.BuscarPorCodigo(transportador);
                proprietario = transportadoraSelecionada?.CNPJ_SemFormato ?? string.Empty;
            }

            DateTime periodoInicial = ocorrencia.PeriodoInicio.Value;
            DateTime periodoFim = ocorrencia.PeriodoFim.Value;

            string placa = ocorrencia.Veiculo.Placa;
            int contrato = ocorrencia.ContratoFrete.Codigo;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaOcorrenciaSumarizado.ConsultarDocumentosCargasPorContratoEVeiculo(periodoInicial, periodoFim, transportador, proprietario, placa, contrato, "", "", 0, "", "", 0, 0);

            return (from o in cargaCTes select o.Carga).Distinct().ToList();
        }

        public void SetaEmitenteOcorrencia(ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            if (ocorrencia.OrigemOcorrenciaPorPeriodo)
            {
                if (ocorrencia.TipoOcorrencia.EmitenteTipoOcorrencia == EmitenteTipoOcorrencia.Outros)
                {
                    if (ocorrencia.TipoOcorrencia.OutroEmitente != null)
                        ocorrencia.Emitente = ocorrencia.TipoOcorrencia.OutroEmitente;
                    else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                        ocorrencia.Emitente = repEmpresa.BuscarPorCNPJ(usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);
                    else
                        ocorrencia.Emitente = null;
                }
                else if (ocorrencia.Cargas != null && ocorrencia.Cargas.Count() > 0)
                    ocorrencia.Emitente = ocorrencia.Cargas.FirstOrDefault().Empresa;
            }
        }

        public bool ValidaSeExisteOcorrenciaPorPeriodo(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, out string erro, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario)
        {
            erro = string.Empty;

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            List<SituacaoOcorrencia> situacoes = new List<SituacaoOcorrencia>()
            {
                SituacaoOcorrencia.Cancelada,
                SituacaoOcorrencia.Rejeitada,
                SituacaoOcorrencia.RejeitadaEtapaEmissao
            };

            int codigoVeiculo = 0;
            if (ocorrencia.TipoOcorrencia != null && ocorrencia.TipoOcorrencia.OcorrenciaComVeiculo && ocorrencia.Veiculo != null)
                codigoVeiculo = ocorrencia.Veiculo.Codigo;

            int codigoEmitente = 0;
            if (ocorrencia.Emitente != null)
                codigoEmitente = ocorrencia.Emitente.Codigo;

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaExistente = repOcorrencia.ExisteOcorrenciaPorPeriodoETipoVeiculo(ocorrencia.PeriodoInicio.Value, ocorrencia.PeriodoFim.Value, ocorrencia.TipoOcorrencia.Codigo, usuario.Codigo, ocorrencia.Filial?.Codigo ?? 0, situacoes, codigoVeiculo, codigoEmitente);

            if (ocorrenciaExistente != null)
            {
                if (codigoVeiculo == 0)
                {
                    if (ocorrencia.Filial != null)
                        erro = "Já existe uma Ocorrência (" + ocorrenciaExistente.NumeroOcorrencia + ") para a filial " + ocorrencia.Filial.Descricao + " com o período informado e mesmo tipo de ocorrência.";
                    else
                        erro = "Já existe uma Ocorrência (" + ocorrenciaExistente.NumeroOcorrencia + ") com o período informado e mesmo tipo de ocorrência.";
                }
                else
                    erro = "Já existe uma Ocorrência (" + ocorrenciaExistente.NumeroOcorrencia + ") para o veículo " + ocorrencia.Veiculo.Placa + " com o período informado e mesmo tipo de ocorrência.";
                return false;
            }

            return true;
        }

        public bool ValidaSeExisteOcorrenciaPorCTe(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, out string erro, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoChamado = 0)
        {
            erro = string.Empty;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                List<int> numeroCTes = cargaCTes.Where(o => o.CTe.Ocorrencias.Any(oco => oco.Ocorrencia.Codigo == ocorrencia.TipoOcorrencia.Codigo)).Select(o => o.CTe.Numero).ToList();

                if (numeroCTes.Count > 0)
                {
                    erro = $"Não é possível lançar a ocorrência {ocorrencia.TipoOcorrencia.Descricao} pois já existe uma ocorrência deste tipo para o(s) CT-e(s) {string.Join(", ", numeroCTes)}.";
                    return false;
                }
            }
            else
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

                int codigoMotivoChamado = codigoChamado > 0 ? repMotivoChamado.BuscarCodigoMotivoPorChamado(codigoChamado) : 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargaCTes)
                {
                    List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrenciaLancadas = repOcorrencia.BuscarPorCargaCTeEMotivoChamado(cargaCte.Codigo, codigoMotivoChamado);
                    foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaLancada in listaOcorrenciaLancadas)
                    {
                        if (ocorrenciaLancada.TipoOcorrencia.Codigo == ocorrencia.TipoOcorrencia.Codigo && //Mesmo tipo de ocorrência
                            (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || ocorrenciaLancada.ValorOcorrencia > 0) && //Com valor maior que zero
                            ocorrenciaLancada.SituacaoOcorrencia != SituacaoOcorrencia.Rejeitada &&
                            ocorrenciaLancada.SituacaoOcorrencia != SituacaoOcorrencia.RejeitadaEtapaEmissao &&
                            ocorrenciaLancada.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada)
                        {
                            erro = $"Não é possível lançar ocorrência {ocorrencia.TipoOcorrencia.Descricao} pois já existe a ocorrência nº {ocorrenciaLancada.NumeroOcorrencia} para o CT-e {(cargaCte.CTe?.Numero.ToString() ?? "(pré CT-e selecionado)")}.";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool ValidaSeExisteOcorrenciaPorDocParaEmissaoNFSManual(List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSManual, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, out string erro, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoChamado = 0)
        {
            erro = string.Empty;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                List<int> numeroDocs = cargaDocumentosParaEmissaoNFSManual.Where(o => o.CTe.Ocorrencias.Any(oco => oco.Ocorrencia.Codigo == ocorrencia.TipoOcorrencia.Codigo)).Select(o => o.CTe.Numero).ToList();

                if (numeroDocs.Count > 0)
                {
                    erro = $"Não é possível lançar a ocorrência {ocorrencia.TipoOcorrencia.Descricao} pois já existe uma ocorrência deste tipo para o(s) Docs(s) para NFS Manual {string.Join(", ", numeroDocs)}.";
                    return false;
                }
            }
            else
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

                int codigoMotivoChamado = codigoChamado > 0 ? repMotivoChamado.BuscarCodigoMotivoPorChamado(codigoChamado) : 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual docParaNFSManual in cargaDocumentosParaEmissaoNFSManual)
                {
                    List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrenciaLancadas = repOcorrencia.BuscarPorDocParaNFSManualEMotivoChamado(docParaNFSManual.Codigo, codigoMotivoChamado);
                    foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaLancada in listaOcorrenciaLancadas)
                    {
                        if (ocorrenciaLancada.TipoOcorrencia.Codigo == ocorrencia.TipoOcorrencia.Codigo && //Mesmo tipo de ocorrência
                            (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || ocorrenciaLancada.ValorOcorrencia > 0) && //Com valor maior que zero
                            ocorrenciaLancada.SituacaoOcorrencia != SituacaoOcorrencia.Rejeitada &&
                            ocorrenciaLancada.SituacaoOcorrencia != SituacaoOcorrencia.RejeitadaEtapaEmissao &&
                            ocorrenciaLancada.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada)
                        {
                            erro = $"Não é possível lançar ocorrência {ocorrencia.TipoOcorrencia.Descricao} pois já existe a ocorrência nº {ocorrenciaLancada.NumeroOcorrencia} para o Doc para NFS Manual {docParaNFSManual.Numero.ToString()}.";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool ValidaOcorrenciaDuplicadaCargaMDFe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, out string erro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> ListMDFesCarga = repCargaMDFeManual.BuscarTodosPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual MDFeCarga in ListMDFesCarga)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaDentroMDFe in MDFeCarga.Cargas)
                {
                    List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaCargaOcorrencias = repCargaOcorrencia.BuscarPorCarga(cargaDentroMDFe.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia in listaCargaOcorrencias)
                    {
                        if (cargaOcorrencia.TipoOcorrencia == ocorrencia.TipoOcorrencia)
                        {
                            if (cargaOcorrencia.SituacaoOcorrencia != SituacaoOcorrencia.Rejeitada &&
                                cargaOcorrencia.SituacaoOcorrencia != SituacaoOcorrencia.RejeitadaEtapaEmissao &&
                                cargaOcorrencia.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada)
                            {
                                erro = "Essa ocorrência já está cadastrada com uma carga em mesmo MDFe.";
                                return false;
                            }
                        }
                    }
                }
            }

            erro = "";

            return true;
        }

        public bool SetaModeloDocumentoFiscal(ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, out string erro, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware)
        {
            erro = "";

            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (ocorrencia.ModeloDocumentoFiscal != null && ocorrencia.ModeloDocumentoFiscal.Numero == "39") //notafiscal serviço
                {
                    if (!ValidaSeExisteConfiguracaoTomador(cargaCTEs, ocorrencia, out erro, unitOfWork))
                        return false;
                }

                if (ocorrencia.ModeloDocumentoFiscal != null && (!ocorrencia.ModeloDocumentoFiscal.GerarMovimentoAutomatico || ocorrencia.ModeloDocumentoFiscal.TipoMovimentoEmissao == null || ocorrencia.ModeloDocumentoFiscal.TipoMovimentoCancelamento == null))
                {
                    erro = "Não existe configuração de movimento financeiro para o modelo de documento " + ocorrencia.ModeloDocumentoFiscal.Abreviacao + ", não sendo possível adicionar a ocorrência.";
                    return false;
                }
                else
                {
                    List<Dominio.Entidades.ModeloDocumentoFiscal> modelosDocumentos = cargaCTEs.Select(o => o.CTe.ModeloDocumentoFiscal).Distinct().ToList();

                    foreach (Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento in modelosDocumentos)
                    {
                        if (!modeloDocumento.GerarMovimentoAutomatico || modeloDocumento.TipoMovimentoEmissao == null || modeloDocumento.TipoMovimentoCancelamento == null)
                        {
                            erro = "Não existe configuração de movimento financeiro para o modelo de documento " + modeloDocumento.Abreviacao + ", não sendo possível adicionar a ocorrência.";
                            return false;
                        }
                    }
                }

                if (ocorrencia.ModeloDocumentoFiscal != null && ocorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                {
                    ocorrencia.ModeloDocumentoFiscalEmissaoMunicipal = null;

                    if (ocorrencia.Carga.TipoOperacao != null && ocorrencia.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                    {
                        if (ocorrencia.Carga.TipoOperacao.UtilizarOutroModeloDocumentoEmissaoMunicipal)
                            ocorrencia.ModeloDocumentoFiscalEmissaoMunicipal = ocorrencia.Carga.TipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal;
                    }
                    else
                    {
                        Dominio.Entidades.Cliente tomador = cargaCTEs.Select(o => o.CTe.TomadorPagador.Cliente).First();

                        if (tomador != null)
                        {
                            if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                            {
                                if (tomador.UtilizarOutroModeloDocumentoEmissaoMunicipal)
                                    ocorrencia.ModeloDocumentoFiscalEmissaoMunicipal = tomador.ModeloDocumentoFiscalEmissaoMunicipal;
                            }
                            else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.UtilizarOutroModeloDocumentoEmissaoMunicipal)
                                ocorrencia.ModeloDocumentoFiscalEmissaoMunicipal = tomador.GrupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal;
                        }
                    }

                    if (ocorrencia.ModeloDocumentoFiscalEmissaoMunicipal == null)
                    {
                        erro = "Não existe um modelo de documento para emissão municipal configurado no tipo de operação/tomador/grupo do tomador, não sendo possível gerar os controles para NFS Manual.";
                        return false;
                    }
                }
            }
            else
            {
                if (ocorrencia.TipoOcorrencia.TipoEmissaoIntramunicipal == TipoEmissaoIntramunicipal.SempreNFSe)
                    ocorrencia.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("39");
            }

            return true;
        }

        public bool ValidaSeExisteConfiguracaoTomador(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, out string erro, Repositorio.UnitOfWork unitOfWork)
        {
            erro = string.Empty;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.Embarcador.NFSe.NFSe serNFSe = new Servicos.Embarcador.NFSe.NFSe(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                Dominio.Entidades.Cliente tomador = null;
                if (ocorrencia.Responsavel.HasValue)
                {
                    if (ocorrencia.Responsavel.Value == Dominio.Enumeradores.TipoTomador.Remetente)
                        tomador = repCliente.BuscarPorCPFCNPJ(double.Parse(cargaCTe.CTe.Remetente.CPF_CNPJ));
                    else
                        tomador = repCliente.BuscarPorCPFCNPJ(double.Parse(cargaCTe.CTe.Destinatario.CPF_CNPJ));
                }

                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = null;

                if (tomador != null)
                    transportadorConfiguracaoNFSe = serNFSe.BuscarConfiguracaoEmissaoNFSe(cargaCTe.CTe.Empresa.Codigo, tomador.Localidade.Codigo, tomador?.Localidade?.Estado?.Sigla ?? "", tomador?.GrupoPessoas?.Codigo ?? 0, tomador?.Localidade?.Codigo ?? 0, ocorrencia?.Carga?.TipoOperacao?.Codigo ?? 0, tomador?.CPF_CNPJ ?? 0, ocorrencia.TipoOcorrencia?.Codigo ?? 0, unitOfWork);
                else
                    transportadorConfiguracaoNFSe = serNFSe.BuscarConfiguracaoEmissaoNFSe(cargaCTe.CTe.Empresa.Codigo, cargaCTe.CTe.LocalidadeTerminoPrestacao.Codigo, "", 0, 0, ocorrencia?.Carga.TipoOperacao?.Codigo ?? 0, tomador?.CPF_CNPJ ?? 0, ocorrencia.TipoOcorrencia?.Codigo ?? 0, unitOfWork);

                if (transportadorConfiguracaoNFSe == null)
                {
                    erro = "Não existe configuração para emissão de NFS-e para a localidade de " + tomador?.Localidade.DescricaoCidadeEstado ?? cargaCTe.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado + ". Por favor, configure a emissão para essa localidade e tente novamente.";
                    return false;
                }
            }

            return true;
        }

        public void SalvarCargasSumarizadas(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, int codigoTransportador, int codigoFilial, string strCargasComplementadasDias)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            DateTime periodoInicial = ocorrencia.PeriodoInicio.Value;
            DateTime periodoFim = ocorrencia.PeriodoFim.Value;

            int transportador = 0;
            string proprietario = "";

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                Dominio.Entidades.Empresa empresaTerceiro = repEmpresa.BuscarPorCNPJ(usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);

                transportador = empresaTerceiro != null ? empresaTerceiro.Codigo : 0;
                proprietario = usuario.ClienteTerceiro != null ? usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : string.Empty;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                transportador = usuario.Empresa.Codigo;
                proprietario = usuario.Empresa != null ? usuario.Empresa.CNPJ_SemFormato : string.Empty;
            }
            else
            {
                transportador = codigoTransportador;
                Dominio.Entidades.Empresa transportadoraSelecionada = repEmpresa.BuscarPorCodigo(transportador);
                proprietario = transportadoraSelecionada?.CNPJ_SemFormato ?? string.Empty;
            }

            int filial = codigoFilial;

            if (!string.IsNullOrWhiteSpace(strCargasComplementadasDias))
            {
                dynamic cargasComplementadasDias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(strCargasComplementadasDias);
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado> cargasSumarizadas = repCargaOcorrenciaSumarizado.ConsultarCargasOcorrenciaPorPeriodo(periodoInicial, periodoFim, transportador, filial, proprietario, ocorrencia.TipoOcorrencia.FiltrarCargasPeriodo, ocorrencia.TipoOcorrencia.Codigo, "", "", 0, 0);

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado carga in cargasSumarizadas)
                {
                    int diasSetado = 0;

                    try
                    {
                        diasSetado = (int)cargasComplementadasDias[carga.CodigoVeiculo.ToString()];
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao buscar dias complementados por código de veículo: {ex.ToString()}", "CatchNoAction");
                    }

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado cargaSumarizada = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado()
                    {
                        CargaOcorrencia = ocorrencia,
                        QuantidadeCargas = carga.QuantidadeCargas,
                        QuantidadeDias = diasSetado > 0 ? diasSetado : carga.QuantidadeDias,
                        QuantidadeDocumentos = carga.QuantidadeDocumentos,
                        ValorMercadoria = carga.ValorNotas,
                        Veiculo = repVeiculo.BuscarPorCodigo(carga.CodigoVeiculo)
                    };
                    repCargaOcorrenciaSumarizado.Inserir(cargaSumarizada);
                }
            }
        }

        public bool FluxoGeralOcorrencia(ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSManual, ref string mensagemRetorno, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, string strCargaCTesImportados, bool gerarEventosEntrega, bool permitirAbrirOcorrenciaAposPrazoSolicitacao = false, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, TipoRateioOcorrenciaLote? tipoRateioOcorrenciaLote = null, bool execucaoValePallet = false, string urlAcesso = "", List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaCargaCTeNotaFiscal> listaOcorrenciaCargaCTeNotaFiscal = null)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            // To Do:
            // Esse método possui mais que uma responsabilidade.
            // É preciso separar em outros métodos
            Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Servicos.Embarcador.Integracao.IntegracaoCTe serIntegracaoCte = new Servicos.Embarcador.Integracao.IntegracaoCTe(unitOfWork);
            Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
            Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);
            Servicos.Embarcador.CargaOcorrencia.RegraParcelamentoOcorrencia servicoRegraParcelamentoOcorrencia = new RegraParcelamentoOcorrencia(unitOfWork);

            Dominio.Enumeradores.TipoTomador? tipoTomador = ocorrencia.Responsavel;
            bool complementoICMS = false;

            if (ocorrencia.ComponenteFrete != null && ocorrencia.ComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS)
                complementoICMS = true;

            if (ocorrencia.ValorOcorrencia > 0m || complementoICMS)
            {
                if (ocorrencia.ComponenteFrete == null)
                {
                    mensagemRetorno = "É obrigatório informar um componente de frete para uma ocorrência com valor";
                    return false;
                }

                if (cargaCTEs.Any(obj => obj.CTe == null))
                {
                    mensagemRetorno = "Para gerar ctes complementarem é necessário que todos os CT-es da carga estejam importados";
                    return false;
                }
            }

            if (!this.GerarAprovacaoOcorrencia(ocorrencia, out List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao> notificoes, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, usuario, ConfiguracaoEmbarcador, execucaoValePallet, urlAcesso))
                return false;

            bool cteEmitidoNoEmbarcador = false;

            if (ocorrencia.OrigemOcorrencia == OrigemOcorrencia.PorCarga)
            {
                cteEmitidoNoEmbarcador = RetornarTomadorEmiteCTeNoEmbarcador(ocorrencia, ocorrencia.Carga.Codigo, tipoTomador, ocorrencia.Tomador?.CPF_CNPJ ?? 0d, unitOfWork);
            }

            if (ocorrencia.OrigemOcorrencia != OrigemOcorrencia.PorContrato && !ocorrencia.ComplementoValorFreteCarga)
            {
                if (!this.GerarDocumentosDaOcorrencia(cargaCTEs, ocorrencia, cteEmitidoNoEmbarcador, ref mensagemRetorno, unitOfWork, strCargaCTesImportados, ConfiguracaoEmbarcador, cargaDocumentosParaEmissaoNFSManual, listaOcorrenciaCargaCTeNotaFiscal))
                    return false;
            }

            if (ocorrencia.ValorOcorrencia <= 0m && !complementoICMS && (ocorrencia.TipoOcorrencia == null || !ocorrencia.TipoOcorrencia.NaoGerarIntegracao))
            {
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
                bool adicionouIntegracaoOcorrenciaCte = false;

                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao> integracoes = repOcorrenciaIntegracao.BuscarPorOcorrencia(ocorrencia.Codigo);

                if (integracoes.Any(o => o.TipoIntegracao != null && o.TipoIntegracao.Tipo == TipoIntegracao.Natura) && (configuracaoIntegracao == null || configuracaoIntegracao.EnviarOcorrenciaNaturaAutomaticamente))
                {
                    ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.AgIntegracao;

                    serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(ocorrencia, TipoIntegracao.Natura, unitOfWork, false);

                    adicionouIntegracaoOcorrenciaCte = true;

                }
                else if (integracoes.Any(o => o.TipoIntegracao != null && (o.TipoIntegracao.Tipo == TipoIntegracao.InteliPost
                                                                        || o.TipoIntegracao.Tipo == TipoIntegracao.MultiEmbarcador
                                                                        || o.TipoIntegracao.Tipo == TipoIntegracao.Riachuelo
                                                                        || o.TipoIntegracao.Tipo == TipoIntegracao.Electrolux)))
                {
                    ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.AgIntegracao;
                    ocorrencia.GerandoIntegracoes = true;
                    adicionouIntegracaoOcorrenciaCte = true;
                }

                if (!adicionouIntegracaoOcorrenciaCte)
                    Servicos.Embarcador.Integracao.IntegracaoEDI.AdicionarEDIParaIntegracao(ocorrencia, false, TipoServicoMultisoftware, unitOfWork);

                if (gerarEventosEntrega)
                    Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarEventoCargaEntregaPorOcorrencia(cargaCTEs, ocorrencia, ConfiguracaoEmbarcador, TipoServicoMultisoftware, auditado, clienteMultisoftware, unitOfWork);

                ocorrencia.ComponenteFrete = null;
            }

            if (((usuario != null && usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro) || ocorrencia.TipoOcorrencia.OcorrenciaTerceiros) && ocorrencia.ValorOcorrencia > 0 && cargaCTEs?.Count > 0)
            {
                decimal valorFreteDespesa = Math.Round((ocorrencia.ValorOcorrencia / cargaCTEs.Count), 2, MidpointRounding.ToEven);
                ocorrencia.ObservacaoCTe = "Frete Despesa: R$" + String.Format("{0:0.##}", valorFreteDespesa, cultura) + " / Tabela de Frete utilizada para o Calculo é a mesma da data da prestação do serviço;";
            }

            servicoRegraParcelamentoOcorrencia.DefinirParcelamento(ocorrencia);

            repOcorrencia.Atualizar(ocorrencia);

            SituacaoOcorrencia[] statusSucesso = new SituacaoOcorrencia[] {
                SituacaoOcorrencia.EmEmissaoCTeComplementar, //SituacaoOcorrencia.AgEmissaoCTeComplementar,
                SituacaoOcorrencia.SemRegraAprovacao,
                SituacaoOcorrencia.AgAprovacao,
                SituacaoOcorrencia.AgAutorizacaoEmissao
            };

            if (statusSucesso.Contains(ocorrencia.SituacaoOcorrencia))
            {
                if ((ocorrencia.ValorOcorrencia > 0 || complementoICMS) && !ocorrencia.NaoGerarDocumento)
                {
                    if (ocorrencia.ModeloDocumentoFiscal != null || !cteEmitidoNoEmbarcador)
                    {
                        switch (ocorrencia.OrigemOcorrencia)
                        {
                            case OrigemOcorrencia.PorCarga:
                                if (!this.GerarComplementoInfoCargaCTe(ocorrencia, cargaCTEs, cargaDocumentosParaEmissaoNFSManual, ref mensagemRetorno, unitOfWork, complementoICMS, TipoServicoMultisoftware, permitirAbrirOcorrenciaAposPrazoSolicitacao, tipoRateioOcorrenciaLote))
                                    return false;

                                break;

                            case OrigemOcorrencia.PorPeriodo:
                                if (!this.GerarComplementoInfoPeriodo(ocorrencia, ref mensagemRetorno, TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador, permitirAbrirOcorrenciaAposPrazoSolicitacao))
                                    return false;
                                break;

                            case OrigemOcorrencia.PorContrato:
                                if (!this.GerarComplementoInfoContrato(ocorrencia, ref mensagemRetorno, TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador, permitirAbrirOcorrenciaAposPrazoSolicitacao))
                                    return false;
                                break;
                        }
                    }
                    else
                    {
                        serCargaCTeComplementar.ImportarCTesComplementaresParaOcorrencia(ocorrencia, unitOfWork, TipoServicoMultisoftware);
                    }

                }

                if (!ocorrencia.ComplementoValorFreteCarga && !ocorrencia.NaoGerarDocumento && cargaCTEs.Count > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                    bool informacoesComp = repCargaCTeComplementoInfo.CargaCTeComplementoInfoPorOcorrencia(ocorrencia.Codigo);
                    if (!informacoesComp && ocorrencia.ValorOcorrencia > 0)
                    {
                        mensagemRetorno = "Não foi possível gerar as informações do complemento.";
                        Servicos.Log.TratarErro("Nenhum CargaCTeComplementoInfo foi gerado. Isso conflita na geração de CT-e! cteEmitidoNoEmbarcador: " + (cteEmitidoNoEmbarcador ? "true" : "false"));
                        return false;
                    }
                }
            }

            // Notifica alçadas
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, TipoServicoMultisoftware, string.Empty);
            foreach (Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao notificacao in notificoes)
            {
                string titulo = string.Concat(ocorrencia.Carga?.Filial?.Descricao, " - Ocorrência ", ocorrencia.NumeroOcorrencia, " - ", ocorrencia.TipoOcorrencia?.Descricao, " - Carga ", ocorrencia.Carga?.CodigoCargaEmbarcador);

                serNotificacao.GerarNotificacaoEmail(notificacao.Aprovador, usuario, ocorrencia.Codigo, "Ocorrencias/AutorizacaoOcorrencia", titulo, notificacao.Mensagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }

            Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa();

            if (!ocorrencia.TipoOcorrencia.TipoOcorrenciaControleEntrega && ocorrencia.TipoOcorrencia.NotificarTransportadorPorEmail)
                NotificarTransportadorPorEmail(ocorrencia, unitOfWork, ref notificacaoEmpresa);

            if (!ocorrencia.TipoOcorrencia.TipoOcorrenciaControleEntrega && ocorrencia.TipoOcorrencia.NotificarPorEmail)
                NotificarOcorrenciaPorEmail(ocorrencia, unitOfWork, notificacaoEmpresa);

            if (!ocorrencia.TipoOcorrencia.TipoOcorrenciaControleEntrega && ocorrencia.TipoOcorrencia.NotificarClientePorEmail)
                NotificarClientePorEmail(ocorrencia, unitOfWork, notificacaoEmpresa);

            if (ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.SemRegraEmissao ||
               ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAutorizacaoEmissao
               || ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.EmEmissaoCTeComplementar) //ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgEmissaoCTeComplementar)
                Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentosParaProvisaoOcorrencia(ocorrencia, TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador);

            if (ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.EmEmissaoCTeComplementar)
            {
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia.ExecutaProximoPassoOcorrencia(ocorrencia, unitOfWork.StringConexao, unitOfWork);
            }

            return true;
        }

        public void GerarOcorrenciaCargaPedidoEntregueForaPrazo(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataConfirmacao, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador)
        {
            bool atrasado = false;
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repxmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(cargaEntrega.Codigo);

            if (cargaEntrega.DataPrevista.HasValue && cargaEntrega.DataPrevista.Value < dataConfirmacao)
                atrasado = true;

            if (!cargaEntrega.DataPrevista.HasValue)//se a carga entrega nao tem data prevista, pega apenas os pedidos q estao atrasados pela previsao do pedido.
            {
                cargaPedidos = cargaPedidos.Where(x => x.Pedido.PrevisaoEntrega.HasValue && x.Pedido.PrevisaoEntrega.Value < dataConfirmacao).ToList();
                if (cargaPedidos.Count > 0)
                    atrasado = true;
            }

            if (atrasado)
            {
                //atrasados. devemos gerar uma ocorrencia;
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();
                    ocorrencia.Carga = cargaEntrega.Carga;
                    ocorrencia.Quantidade = 0;
                    ocorrencia.TipoOcorrencia = tipoOcorrencia;
                    ocorrencia.PercentualAcresciomoValor = tipoOcorrencia.PercentualAcrescimo;
                    ocorrencia.DataOcorrencia = DateTime.Now;
                    ocorrencia.DataEvento = DateTime.Now;
                    ocorrencia.PeriodoInicio = dataConfirmacao;
                    ocorrencia.Observacao = "Geração automática ao finalizar entrega com atrazo";
                    ocorrencia.ObservacaoCTe = "Geração automática ao finalizar entrega com atrazo";
                    ocorrencia.NumeroOcorrenciaCliente = "";
                    ocorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
                    ocorrencia.DataAlteracao = DateTime.Now;
                    ocorrencia.DataFinalizacaoEmissaoOcorrencia = DateTime.Now;
                    ocorrencia.Usuario = cargaEntrega.Carga.Operador;
                    ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada;
                    ocorrencia.DTNatura = null;
                    ocorrencia.ComplementoValorFreteCarga = ocorrencia.TipoOcorrencia?.OcorrenciaComplementoValorFreteCarga ?? false;
                    ocorrencia.Filial = cargaEntrega.Carga.Filial;
                    ocorrencia.NaoGerarDocumento = tipoOcorrencia.NaoGerarDocumento;
                    ocorrencia.OrigemOcorrencia = tipoOcorrencia.OrigemOcorrencia;
                    ocorrencia.ComponenteFrete = tipoOcorrencia.ComponenteFrete;
                    ocorrencia.CargaEntrega = cargaEntrega;
                    if (tipoOcorrencia.TomadorTipoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia.Outros && ocorrencia.Tomador == null && ocorrencia.TipoOcorrencia.OutroTomador != null)
                    {
                        ocorrencia.Responsavel = Dominio.Enumeradores.TipoTomador.Outros;
                        ocorrencia.Tomador = tipoOcorrencia.OutroTomador;
                    }
                    ocorrencia.ModeloDocumentoFiscal = tipoOcorrencia.ModeloDocumentoFiscal;

                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repxmlNotaFiscal.BuscarXMLNotaFiscalPorCargaPedido(cargaPedido.Codigo);
                    decimal ValorNotas = notasFiscais.Sum(x => x.Valor);

                    ocorrencia.ValorOcorrencia = tipoOcorrencia.GerarOcorrenciaComValorGrossPedido ? ValorNotas : 0;
                    ocorrencia.ValorOcorrenciaLiquida = tipoOcorrencia.GerarOcorrenciaComValorGrossPedido ? ValorNotas : 0;

                    srvOcorrencia.SetaEmitenteOcorrencia(ref ocorrencia, unitOfWork, TipoServicoMultisoftware, cargaEntrega.Carga.Operador);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = null;
                    cargaCTEs = repCargaEntregaNotaFiscal.BuscarCargaCTePorCargaEntrega(cargaEntrega.Codigo);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSManual = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCargaENotaFiscal(cargaEntrega.Carga.Codigo, (from obj in notasFiscais select obj.Codigo).ToList());

                    if (ocorrencia.TipoOcorrencia.DataOcorrenciaIgualDataCTeComplementado)
                        ocorrencia.DataOcorrencia = cargaCTEs.FirstOrDefault()?.CTe.DataEmissao ?? ocorrencia.DataOcorrencia;

                    repOcorrencia.Inserir(ocorrencia);

                    Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(ocorrencia, cargaCTEs, unitOfWork);

                    string mensagemRetorno = string.Empty;
                    srvOcorrencia.FluxoGeralOcorrencia(ref ocorrencia, cargaCTEs, cargaDocumentosParaEmissaoNFSManual, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, cargaEntrega.Carga.Operador, ConfiguracaoEmbarcador, clienteMultisoftware, "", false);

                    repOcorrencia.Atualizar(ocorrencia);
                }
            }
        }

        public void GerarOcorrenciaCargaValorFreteFechamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal ValorOcorrencia, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (carga != null && ValorOcorrencia > 0 && tipoOcorrencia != null && tipoOcorrencia.OcorrenciaDiferencaValorFechamento && !tipoOcorrencia.ImpedirCriarOcorrenciaCasoExistirCanhotosPendentes)
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();
                ocorrencia.Carga = carga;
                ocorrencia.Quantidade = 0;
                ocorrencia.TipoOcorrencia = tipoOcorrencia;
                ocorrencia.PercentualAcresciomoValor = tipoOcorrencia.PercentualAcrescimo;
                ocorrencia.DataOcorrencia = DateTime.Now;
                ocorrencia.DataEvento = DateTime.Now;
                ocorrencia.PeriodoInicio = DateTime.Now;
                ocorrencia.Observacao = "Geração automática atravéz do fechamento da carga, valor do frete recalculado maior ao valor da carga";
                ocorrencia.ObservacaoCTe = "Geração automática atravéz do fechamento da carga, valor do frete recalculado maior ao valor da carga";
                ocorrencia.NumeroOcorrenciaCliente = "";
                ocorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
                ocorrencia.DataAlteracao = DateTime.Now;
                ocorrencia.DataFinalizacaoEmissaoOcorrencia = DateTime.Now;
                ocorrencia.ComponenteFrete = tipoOcorrencia.ComponenteFrete;
                ocorrencia.Usuario = carga.Operador;
                ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao;
                ocorrencia.DTNatura = null;
                ocorrencia.ComplementoValorFreteCarga = ocorrencia.TipoOcorrencia?.OcorrenciaComplementoValorFreteCarga ?? false;
                ocorrencia.Filial = carga.Filial;
                ocorrencia.NaoGerarDocumento = tipoOcorrencia.NaoGerarDocumento;
                ocorrencia.OrigemOcorrencia = tipoOcorrencia.OrigemOcorrencia;
                if (tipoOcorrencia.TomadorTipoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia.Outros && ocorrencia.Tomador == null && ocorrencia.TipoOcorrencia.OutroTomador != null)
                {
                    ocorrencia.Responsavel = Dominio.Enumeradores.TipoTomador.Outros;
                    ocorrencia.Tomador = tipoOcorrencia.OutroTomador;
                }
                ocorrencia.ModeloDocumentoFiscal = tipoOcorrencia.ModeloDocumentoFiscal;
                ocorrencia.ValorOcorrencia = ValorOcorrencia;
                ocorrencia.ValorOcorrenciaLiquida = ValorOcorrencia;

                srvOcorrencia.SetaEmitenteOcorrencia(ref ocorrencia, unitOfWork, TipoServicoMultisoftware, carga.Operador);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCte.BuscarPorCarga(carga.Codigo);

                if (ocorrencia.TipoOcorrencia.DataOcorrenciaIgualDataCTeComplementado)
                    ocorrencia.DataOcorrencia = cargaCTEs.FirstOrDefault()?.CTe.DataEmissao ?? ocorrencia.DataOcorrencia;

                repOcorrencia.Inserir(ocorrencia);

                Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(ocorrencia, cargaCTEs, unitOfWork);

                string mensagemRetorno = string.Empty;
                srvOcorrencia.FluxoGeralOcorrencia(ref ocorrencia, cargaCTEs, null, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, carga.Operador, ConfiguracaoEmbarcador, clienteMultisoftware, "", false);

                repOcorrencia.Atualizar(ocorrencia);
            }
        }

        public SituacaoOcorrencia VerificarRegrasAutorizacaoAprovacaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, out List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao> notificoes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, string urlAcesso = "")
        {
            notificoes = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao>();

            // Quando responsável é definido no momento da solicitação da ocorrência
            if (ocorrencia != null && ocorrencia.UsuarioResponsavelAprovacao != null)
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao regraOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao
                {
                    CargaOcorrencia = ocorrencia,
                    Usuario = ocorrencia.UsuarioResponsavelAprovacao,
                    OrigemRegraOcorrencia = OrigemRegraOcorrencia.Delegada,
                    EtapaAutorizacaoOcorrencia = EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia,
                    NumeroAprovadores = 1,
                    NumeroReprovadores = 1,
                    PrioridadeAprovacao = (ocorrencia.EtapaAutorizacaoOcorrencia == EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia) ? ocorrencia.PrioridadeAprovacaoAtualEtapaAprovacao : ocorrencia.PrioridadeAprovacaoAtualEtapaEmissao
                };

                repCargaOcorrenciaAutorizacao.Inserir(regraOcorrencia);

                ocorrencia.DataBaseAprovacaoAutomatica = DateTime.Now;

                return SituacaoOcorrencia.AgAprovacao;
            }

            // Regras Aprovcao
            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaFiltradaAprovacao = Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia.VerificarRegrasAutorizacaoOcorrencia(ocorrencia, EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia, unitOfWork);

            if (listaFiltradaAprovacao.Count() > 0)
            {
                // Quando ocorre uma aprovacao automatica, ja passa para a proxima etapa
                if (!Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia.CriarRegrasAutorizacao(listaFiltradaAprovacao, ocorrencia, usuario, out notificoes, tipoServicoMultisoftware, unitOfWork.StringConexao, unitOfWork))
                {
                    // Regras Emissao
                    return VerificarRegrasAutorizacaoEmissaoOcorrencia(ocorrencia, out notificoes, tipoServicoMultisoftware, unitOfWork, usuario, urlAcesso);
                }
                else
                    return SituacaoOcorrencia.AgAprovacao;
            }

            return SituacaoOcorrencia.SemRegraAprovacao;
        }

        public SituacaoOcorrencia VerificarRegrasAutorizacaoEmissaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, out List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao> notificoes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, string urlAcesso = "")
        {
            // Regras Emissao
            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaFiltradaEmissao = Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia.VerificarRegrasAutorizacaoOcorrencia(ocorrencia, EtapaAutorizacaoOcorrencia.EmissaoOcorrencia, unitOfWork);
            notificoes = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao>();

            if (listaFiltradaEmissao.Count() > 0)
            {
                // Quando ocorre uma aprovacao automatica, finaliza
                if (!Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia.CriarRegrasAutorizacao(listaFiltradaEmissao, ocorrencia, usuario, out notificoes, tipoServicoMultisoftware, unitOfWork.StringConexao, unitOfWork))
                {
                    //return SituacaoOcorrencia.AgEmissaoCTeComplementar;
                    return SituacaoOcorrencia.EmEmissaoCTeComplementar;
                }
                else
                    return SituacaoOcorrencia.AgAutorizacaoEmissao;
            }
            return SituacaoOcorrencia.SemRegraEmissao;
        }

        public string ValidarHerarquiaDeCredito(ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string strCreditosUtilizados, int codigoCreditorSolicitar)
        {
            Servicos.Embarcador.Credito.SolicitacaoCredito serSolicitacaoCredito = new Servicos.Embarcador.Credito.SolicitacaoCredito(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);

            string retornoUtilizacao = "";
            if (!string.IsNullOrWhiteSpace(strCreditosUtilizados))
            {
                List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado>>(strCreditosUtilizados);
                decimal somaCreditosUtilizados = creditosUtilizados.Sum(obj => obj.ValorUtilizado);
                if (somaCreditosUtilizados < ocorrencia.ValorOcorrencia)
                {
                    decimal valorSolicitar = creditosUtilizados.Sum(obj => obj.ValorUtilizado);
                    retornoUtilizacao = serCreditoMovimentacao.ComprometerCreditos(creditosUtilizados, ocorrencia, unitOfWork);
                    Dominio.Entidades.Usuario solicitado = repUsuario.BuscarPorCodigo(codigoCreditorSolicitar);
                    Dominio.ObjetosDeValor.Embarcador.Creditos.SolicitacaoCreditoGerada solicitacaoGerada = serSolicitacaoCredito.GerarSolicitacaoCredito(ocorrencia.Carga, usuario, solicitado, ocorrencia.ComponenteFrete, ocorrencia.ValorOcorrencia - somaCreditosUtilizados, ocorrencia.Observacao, tipoServicoMultisoftware, unitOfWork);

                    if (solicitacaoGerada.GerouSolicitacao)
                    {
                        ocorrencia.SolicitacaoCredito = solicitacaoGerada.SolicitacaoCredito;
                        ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.AgAprovacao;
                    }
                    else
                    {
                        retornoUtilizacao = solicitacaoGerada.MensagemRetorno;
                    }
                }
                else
                {
                    if (somaCreditosUtilizados == ocorrencia.ValorOcorrencia)
                    {
                        retornoUtilizacao = serCreditoMovimentacao.UtilizarCreditos(creditosUtilizados, ocorrencia, unitOfWork);
                        //ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.AgEmissaoCTeComplementar;
                        ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.EmEmissaoCTeComplementar;
                    }
                    else
                    {
                        retornoUtilizacao = "O valor utilizado (" + somaCreditosUtilizados.ToString("n2") + ") não pode ser maior que o valor da ocorrência (" + ocorrencia.ValorOcorrencia.ToString("n2") + ")";
                    }
                }
            }

            return retornoUtilizacao;
        }

        public bool RetornarTomadorEmiteCTeNoEmbarcador(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, int codigoCarga, Dominio.Enumeradores.TipoTomador? tipoTomador, double cpfCnpjTomador, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaOcorrencia != null && cargaOcorrencia.CTeEmitidoNoEmbarcador.HasValue)
                return cargaOcorrencia.CTeEmitidoNoEmbarcador.Value;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(codigoCarga);

            return cargaPedido?.CTeEmitidoNoEmbarcador ?? false;

            //Dominio.Entidades.Cliente tomador = null;

            //if (!tipoTomador.HasValue)
            //    tomador = cargaPedido.ObterTomador();
            //else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
            //    tomador = cargaPedido.Pedido.Remetente;
            //else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
            //    tomador = cargaPedido.Pedido.Destinatario;
            //else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
            //    tomador = cargaPedido.Expedidor;
            //else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
            //    tomador = cargaPedido.Recebedor;
            //else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            //    tomador = repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);

            //bool cteEmitidoNoEmbarcador = false;
            //if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
            //    cteEmitidoNoEmbarcador = tomador.CTeEmitidoNoEmbarcador;
            //else
            //    cteEmitidoNoEmbarcador = tomador.GrupoPessoas.CTeEmitidoNoEmbarcador;

            //return cteEmitidoNoEmbarcador;
        }

        public Dominio.Entidades.OcorrenciaDeCTe GerarOcorrenciaCTe(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlnotafiscais = null)
        {
            Repositorio.OcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Dominio.Entidades.OcorrenciaDeCTe ocorrenciaDeCTe = new Dominio.Entidades.OcorrenciaDeCTe
            {
                DataDaOcorrencia = ocorrencia.DataOcorrencia,
                Observacao = ocorrencia.Observacao,
                Ocorrencia = ocorrencia.TipoOcorrencia,
                DataDeCadastro = DateTime.Now,
                CTe = cargaCTe.CTe,
                XMLNotaFiscais = xmlnotafiscais
            };

            repOcorrenciaDeCTe.Inserir(ocorrenciaDeCTe);

            if (ocorrencia.TipoOcorrencia?.EntregaRealizada ?? false)
            {
                cargaCTe.CTe.DataEntrega = ocorrencia.DataOcorrencia;

                repCTe.Atualizar(cargaCTe.CTe);
            }

            return ocorrenciaDeCTe;
        }

        public static void EnviarEmailGeracaoOcorrencia(int codigoCargaOcorrencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Email svcEmail = new Servicos.Email(unitOfWork);

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(codigoCargaOcorrencia);

            if (configEmail == null || cargaOcorrencia == null || cargaOcorrencia.TipoOcorrencia == null || !cargaOcorrencia.TipoOcorrencia.EnviarEmailGeracaoOcorrencia || string.IsNullOrWhiteSpace(cargaOcorrencia.TipoOcorrencia.EmailGeracaoOcorrencia))
                return;

            List<string> emailsCopia = new List<string>();
            string email = cargaOcorrencia.TipoOcorrencia.EmailGeracaoOcorrencia;

            if (email.Contains(";"))
            {
                string[] emailsSplit = email.Split(';');

                email = emailsSplit[0];

                for (int i = 1; i < emailsSplit.Length; i++)
                    emailsCopia.Add(emailsSplit[i]);
            }

            // Config
            string from = configEmail.Email;
            string user = configEmail.Email;
            string password = configEmail.Senha;
            string recepient = email;
            string[] bcc = new string[] { };
            string[] cc = emailsCopia.ToArray();
            string servidorSMTP = configEmail.Smtp;
            string signature = "";
            bool possuiSSL = configEmail.RequerAutenticacaoSmtp;
            List<System.Net.Mail.Attachment> attachments = null;

            //// Conteúdo
            string subject = $"Ocorrência {cargaOcorrencia.TipoOcorrencia.Descricao} nº {cargaOcorrencia.NumeroOcorrencia} gerada";

            StringBuilder body = new StringBuilder();

            body.Append($"Ocorrência {cargaOcorrencia.TipoOcorrencia.Descricao} nº {cargaOcorrencia.NumeroOcorrencia} gerada.<br/><br/>");

            if (cargaOcorrencia.Carga != null)
                body.Append($"Carga nº {cargaOcorrencia.Carga.CodigoCargaEmbarcador} ({cargaOcorrencia.Carga.PlacasVeiculos}).<br/>");

            if (cargaOcorrencia.ComponenteFrete != null)
                body.Append($"{cargaOcorrencia.ComponenteFrete.Descricao} R$ {cargaOcorrencia.ValorOcorrencia.ToString("n2")}.<br/>");

            body.Append("<br/>E-mail enviado automaticamente, por favor, não responda.");

            //// Enviar e-mail
            if (!Servicos.Email.EnviarEmail(from, user, password, recepient, bcc, cc, subject, body.ToString(), servidorSMTP, out string erro, configEmail.DisplayEmail, attachments, signature, possuiSSL, string.Empty, 0, unitOfWork))
                Servicos.Log.TratarErro(erro);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia> RetornoOcorrenciaIntegracao(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> ocorrencias)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia> listaFormatada = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia>();

            listaFormatada = (from o in ocorrencias
                              select new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia()
                              {
                                  Data = o.CargaOcorrencia?.DataOcorrencia.ToString("dd/MM/yyyy HH:mm:ss"),
                                  Observacao = o.CargaOcorrencia?.Observacao,
                                  Ocorrencia = o.CargaOcorrencia?.TipoOcorrencia?.Descricao ?? "",
                                  Situacao = o.CargaOcorrencia?.TipoOcorrencia?.DescricaoTipo ?? "",
                                  Status = o.CargaOcorrencia?.SituacaoOcorrencia.ObterDescricao()
                              }).ToList();

            return listaFormatada;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia> RetornoOcorrenciaIntegracao(List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> ocorrencias)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia> listaFormatada = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia>();

            listaFormatada = (from o in ocorrencias
                              select new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia()
                              {
                                  Data = o.DataOcorrencia.ToString("dd/MM/yyyy HH:mm:ss"),
                                  Observacao = "",
                                  Ocorrencia = ((o.TipoDeOcorrencia?.Descricao ?? "") + " " + o.Observacao),
                                  Situacao = "",
                                  Status = ""
                              }).ToList();

            return listaFormatada;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarOcorrencia(string dados, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            int contador = 0;
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                    //Leitura dos dados
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFilial = (from obj in linha.Colunas where obj.NomeCampo == "Filial" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                    if (colFilial != null)
                    {
                        string codigoIntegracaoFilial = (string)colFilial.Valor;
                        if (!string.IsNullOrWhiteSpace(codigoIntegracaoFilial))
                        {
                            filial = repFilial.buscarPorCodigoEmbarcador(codigoIntegracaoFilial);
                            if (filial == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A filial não existe na base da Multisoftware", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroNota = (from obj in linha.Colunas where obj.NomeCampo == "NumeroNota" select obj).FirstOrDefault();
                    int numeroNota = 0;
                    if (colNumeroNota != null)
                        numeroNota = ((string)colNumeroNota.Valor).ToInt();
                    else
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A coluna do número da nota não foi selecionada", i));
                        unitOfWork.Rollback();
                        continue;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroCarga = (from obj in linha.Colunas where obj.NomeCampo == "NumeroCarga" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                    if (colNumeroCarga != null)
                    {
                        string codigoIntegracaoCarga = Utilidades.String.RemoveAllSpecialCharacters((string)colNumeroCarga.Valor);
                        if (!string.IsNullOrWhiteSpace(codigoIntegracaoCarga))
                            carga = repCarga.BuscarPorCodigoEmbarcador(codigoIntegracaoCarga, filial?.Codigo ?? 0);

                        if (carga == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A carga não existe na base da Multisoftware", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                    }
                    else
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A coluna do número da carga não foi selecionada", i));
                        unitOfWork.Rollback();
                        continue;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colOcorrencia = (from obj in linha.Colunas where obj.NomeCampo == "Ocorrencia" select obj).FirstOrDefault();
                    Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = null;
                    if (colOcorrencia != null)
                    {
                        string codigoIntegracaoOcorrencia = (string)colOcorrencia.Valor;
                        if (!string.IsNullOrWhiteSpace(codigoIntegracaoOcorrencia))
                            tipoDeOcorrencia = repOcorrenciaDeCTe.BuscarPorCodigoIntegracao(codigoIntegracaoOcorrencia.Trim());

                        if (tipoDeOcorrencia == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O tipo da ocorrência não existe na base da Multisoftware", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                        else if (tipoDeOcorrencia.OrigemOcorrencia != OrigemOcorrencia.PorCarga)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O tipo da ocorrência informada não é uma ocorrência por carga", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                    }
                    else
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A coluna da ocorrência não foi selecionada", i));
                        unitOfWork.Rollback();
                        continue;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                    string observacao = "";
                    if (colObservacao != null)
                    {
                        observacao = (string)colObservacao.Valor ?? "";

                        if (tipoDeOcorrencia.ExigirInformarObservacao && string.IsNullOrWhiteSpace(observacao))
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("É obrigatório informar uma observação para esse tipo de ocorrência", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacaoCTe = (from obj in linha.Colunas where obj.NomeCampo == "ObservacaoCTe" select obj).FirstOrDefault();
                    string observacaoCTe = "";
                    if (colObservacaoCTe.Valor != null)
                    {
                        observacaoCTe = (string)colObservacaoCTe?.Valor ?? "";
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        continue;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValor = (from obj in linha.Colunas where obj.NomeCampo == "Valor" select obj).FirstOrDefault();
                    decimal valor = 0;
                    if (colValor != null)
                    {
                        decimal.TryParse((string)colValor.Valor, out valor);

                        if (valor > 10000000m)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O valor da ocorrência não pode ser maior que R$ 10.000.000,00.", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveCTe = (from obj in linha.Colunas where obj.NomeCampo == "ChaveCTe" select obj).FirstOrDefault();
                    string chaveCTe = "";
                    int numeroCTe = 0;
                    if (colChaveCTe != null)
                    {
                        chaveCTe = (string)colChaveCTe.Valor;
                        if (!Utilidades.Validate.ValidarChave(chaveCTe))
                            chaveCTe = "";
                        numeroCTe = Utilidades.Chave.ObterNumero(chaveCTe);
                    }

                    //Valida regras
                    if (numeroNota == 0 && numeroCTe == 0)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Não foi informado número da nota e ou chave de cte, pelo menos um deles é necessário.", i));
                        unitOfWork.Rollback();
                        continue;
                    }

                    double remetente = 0, destinatario = 0;
                    //Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = repCargaOcorrenciaDocumento.BuscarPorCargaRemetenteDestinatario(carga.Codigo, remetente, destinatario, tipoDeOcorrencia.Codigo, numeroCTe);
                    //if (cargaOcorrenciaDocumento != null)
                    //{
                    //    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Já existe uma ocorrência gerada e em aberto para os dados", i));
                    //    unitOfWork.Rollback();
                    //    continue;
                    //}

                    bool retornarPreCtes = false;
                    if (carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && carga.AgImportacaoCTe)
                        retornarPreCtes = true;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCte.BuscarPorCarga(carga.Codigo, true, false, false, false, true, numeroNota, destinatario, true, retornarPreCtes, numeroCTe);
                    if (cargaCTEs.Count == 0)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Não foi localizado nenhum CT-e autorizado para os dados", i));
                        unitOfWork.Rollback();
                        continue;
                    }

                    //Inserir ocorrência
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();

                    cargaOcorrencia.DataOcorrencia = DateTime.Now;
                    cargaOcorrencia.DataAlteracao = DateTime.Now;
                    cargaOcorrencia.DataFinalizacaoEmissaoOcorrencia = DateTime.Now;
                    cargaOcorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
                    cargaOcorrencia.Usuario = usuario;
                    cargaOcorrencia.Filial = filial;
                    cargaOcorrencia.Observacao = observacao;
                    cargaOcorrencia.ValorOcorrencia = valor;
                    cargaOcorrencia.ValorOcorrenciaOriginal = valor;
                    cargaOcorrencia.ObservacaoCTe = observacaoCTe;
                    cargaOcorrencia.Carga = carga;
                    cargaOcorrencia.TipoOcorrencia = tipoDeOcorrencia;
                    cargaOcorrencia.OrigemOcorrencia = cargaOcorrencia.TipoOcorrencia.OrigemOcorrencia;
                    cargaOcorrencia.ComponenteFrete = tipoDeOcorrencia?.ComponenteFrete;
                    cargaOcorrencia.IncluirICMSFrete = cargaCTEs.Count > 0 ? (cargaCTEs.FirstOrDefault().CTe != null ? (cargaCTEs.FirstOrDefault().CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false) : false) : false;

                    if (tipoDeOcorrencia.BloqueiaOcorrenciaDuplicada)
                    {
                        if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorCTe(cargaCTEs, cargaOcorrencia, out string erro, unitOfWork, tipoServicoMultisoftware, 0))
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(erro, i));
                            unitOfWork.Rollback();
                            continue;
                        }
                    }

                    repCargaOcorrencia.Inserir(cargaOcorrencia);

                    Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(cargaOcorrencia, cargaCTEs, unitOfWork);

                    string mensagemRetorno = string.Empty;
                    if (!srvOcorrencia.FluxoGeralOcorrencia(ref cargaOcorrencia, cargaCTEs, null, ref mensagemRetorno, unitOfWork, tipoServicoMultisoftware, usuario, configuracaoTMS, clienteMultisoftware, "", true, false, auditado))
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(mensagemRetorno, i));
                        unitOfWork.Rollback();
                        continue;
                    }
                    else
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaOcorrencia, null, "Criou Ocorrência via importação", unitOfWork);


                    contador++;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                    retornoImportacao.Retornolinhas.Add(retornoLinha);
                    unitOfWork.CommitChanges();
                }
                catch (Exception ex2)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex2);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    continue;
                }
            }

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }

        public bool AjustarCTeImportado(out string erro, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteValorLiquido, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcador configuracao = ObterConfiguracoesComponentes(carga, unitOfWork);

            Repositorio.ComponentePrestacaoCTE repComponenteCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesCTe = repComponenteCTe.BuscarPorCTe(cte.Codigo);

            bool possuiComponenteICMS = false, possuiComponenteFreteLiquido = false;

            foreach (Dominio.Entidades.ComponentePrestacaoCTE componenteCTe in componentesCTe)
            {
                string descricao = componenteCTe.Nome.ToLower().Trim();

                if (!string.IsNullOrWhiteSpace(configuracao.DescricaoComponenteFreteLiquido) && descricao == configuracao.DescricaoComponenteFreteLiquido.ToLower().Trim())
                {
                    possuiComponenteFreteLiquido = true;

                    if (configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS && cte.CST == "00" && cte.LocalidadeEmissao?.Estado?.Sigla == cte.LocalidadeInicioPrestacao?.Estado?.Sigla)
                        cte.ValorFrete = cte.ValorAReceber - cte.ValorICMS;
                    else if (configuracao.ValorFreteLiquidoDeveSerValorAReceber)
                        cte.ValorFrete = cte.ValorAReceber;
                    else if ((cte.CST == "60" || cte.CST == "20") && cte.ValorAReceber < cte.ValorPrestacaoServico && cte.ValorPresumido > 0m) //regra para JBS (Tombini)
                        cte.ValorFrete = cte.ValorAReceber - cte.ValorPresumido;
                    else if (cte.CST == "00" && cte.BaseCalculoICMS > cte.ValorAReceber && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Nao) //regra para JBS (Tombini)
                        cte.ValorFrete = cte.ValorAReceber - cte.ValorICMS;
                    else
                        cte.ValorFrete = componenteCTe.Valor;

                    repCTe.Atualizar(cte);

                    if (componenteFreteValorLiquido != null)
                    {
                        componenteCTe.ComponenteFrete = componenteFreteValorLiquido;

                        repComponenteCTe.Atualizar(componenteCTe);
                    }

                    continue;
                }

                Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcadorComponente configuracaoComponenteFrete = configuracao.Componentes.Where(o => o.OutraDescricaoCTe.ToLower().Trim() == descricao).FirstOrDefault();

                if (configuracaoComponenteFrete != null)
                {
                    componenteCTe.ComponenteFrete = repComponenteFrete.BuscarPorCodigo(configuracaoComponenteFrete.Codigo);

                    if (componenteCTe.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
                        possuiComponenteICMS = true;
                    else if (configuracaoComponenteFrete.IncluirICMS != componenteCTe.IncluiNaBaseDeCalculoDoICMS)
                        componenteCTe.IncluiNaBaseDeCalculoDoICMS = configuracaoComponenteFrete.IncluirICMS;

                    repComponenteCTe.Atualizar(componenteCTe);
                }
                else
                {
                    erro = $"Não foi encontrada uma configuração para o componente {componenteCTe.Nome}.";
                    return false;
                }
            }

            if (possuiComponenteICMS || ((cte.CST == "60" || cte.CST == "20") && cte.ValorPrestacaoServico > cte.ValorAReceber))
            {
                cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                cte.PercentualICMSIncluirNoFrete = 100;

                repCTe.Atualizar(cte);
            }

            if (!possuiComponenteFreteLiquido)
            {
                cte.ValorFrete = 0m;

                repCTe.Atualizar(cte);
            }

            erro = string.Empty;
            return true;
        }

        public void ProcessarOcorrenciaLote(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote ocorrenciaLote, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaLote repositorioOcorrenciaLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLote(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia repositorioOcorrenciaLoteCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Servicos.Embarcador.CargaOcorrencia.Ocorrencia servicoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia> listaOcorrenciaLoteCargaOcorrencia = repositorioOcorrenciaLoteCargaOcorrencia.BuscarNaoGeradasPorOcorrenciaLote(ocorrenciaLote.Codigo);

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = ocorrenciaLote.TipoOcorrencia;

            bool sucesso = true;
            string mensagem = string.Empty;
            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia ocorrenciaLoteCargaOcorrencia in listaOcorrenciaLoteCargaOcorrencia)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = ocorrenciaLoteCargaOcorrencia.Carga;
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia
                    {
                        Carga = carga,
                        ValorOcorrencia = ocorrenciaLoteCargaOcorrencia.ValorOcorrenciaRateado,
                        ValorOcorrenciaOriginal = ocorrenciaLoteCargaOcorrencia.ValorOcorrenciaRateado,
                        DataAlteracao = DateTime.Now,
                        DataOcorrencia = DateTime.Now,
                        DataFinalizacaoEmissaoOcorrencia = DateTime.Now,
                        NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork),
                        Observacao = "",
                        ObservacaoCTe = "",
                        ObservacaoCTes = "",
                        TipoOcorrencia = tipoOcorrencia,
                        OrigemOcorrencia = tipoOcorrencia.OrigemOcorrencia,
                        ComponenteFrete = tipoOcorrencia.ComponenteFrete
                    };

                    repositorioOcorrencia.Inserir(cargaOcorrencia);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaOcorrencia, "Adicionado pela ocorrência lote " + ocorrenciaLote.Numero, unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repositorioCargaCTe.BuscarPorCarga(carga.Codigo, false, false, false, false, false, 0, 0, true, false, 0);
                    if (listaCargaCTe.Count == 0)
                        throw new ServicoException($"A Carga {carga.CodigoCargaEmbarcador} não possui CT-es");

                    Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(cargaOcorrencia, listaCargaCTe, unitOfWork);

                    string mensagemRetorno = null;
                    if (!servicoOcorrencia.FluxoGeralOcorrencia(ref cargaOcorrencia, listaCargaCTe, null, ref mensagemRetorno, unitOfWork, tipoServicoMultisoftware, ocorrenciaLote.Usuario, configuracao, clienteMultisoftware, "", false, false, auditado, ocorrenciaLote.TipoRateio))
                    {
                        mensagemRetorno += $" (Carga {carga.CodigoCargaEmbarcador})";
                        throw new ServicoException(mensagemRetorno);
                    }

                    ocorrenciaLoteCargaOcorrencia.CargaOcorrencia = cargaOcorrencia;
                    repositorioOcorrenciaLoteCargaOcorrencia.Atualizar(ocorrenciaLoteCargaOcorrencia);

                    unitOfWork.CommitChanges();
                }
                catch (ServicoException ex)
                {
                    unitOfWork.Rollback();
                    sucesso = false;
                    if (string.IsNullOrWhiteSpace(mensagem))
                        mensagem = ex.Message;
                    else
                        mensagem += " - " + ex.Message;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    unitOfWork.Rollback();
                    sucesso = false;
                    if (string.IsNullOrWhiteSpace(mensagem))
                        mensagem = "Ocorreu uma falha ao gerar a ocorrência";
                    else
                        mensagem += " - Ocorreu uma falha ao gerar a ocorrência";
                }
            }

            if (!sucesso)
            {
                ocorrenciaLote.Situacao = SituacaoOcorrenciaLote.FalhaNaGeracao;
                ocorrenciaLote.MotivoRejeicao = mensagem;
            }
            else
                ocorrenciaLote.Situacao = SituacaoOcorrenciaLote.Finalizado;

            repositorioOcorrenciaLote.Atualizar(ocorrenciaLote);

            Servicos.Embarcador.Hubs.Ocorrencia hubOcorrencia = new Servicos.Embarcador.Hubs.Ocorrencia();
            hubOcorrencia.InformarOcorrenciaLoteAtualizada(ocorrenciaLote.Codigo);
        }

        public string RetornarTextoEmailTransportador(string textoEmail, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            return textoEmail
                .Replace("#NumeroOcorrencia", ocorrencia.NumeroOcorrencia.ToString())
                .Replace("#RazaoTransportador", ocorrencia.Carga?.Empresa?.RazaoSocial ?? "***")
                .Replace("#CNPJTransportador", ocorrencia.Carga?.Empresa?.CNPJ_Formatado ?? "***")
                .Replace("#TipoOcorrencia", ocorrencia.TipoOcorrencia?.Descricao ?? "***")
                .Replace("#ValorOcorrencia", ocorrencia.ValorOcorrencia.ToString("n2"))
                .Replace("#NumeroCarga", ocorrencia.Carga?.CodigoCargaEmbarcador ?? "***")
                .Replace("#NumeroPedido", ocorrencia.Carga?.DadosSumarizados?.NumeroPedidoEmbarcador ?? "***")
                .Replace("#NumeroOrdem", ocorrencia.Carga?.DadosSumarizados?.NumeroOrdem ?? "***");
        }

        public void GerarOcorrenciaDevolucaoPallet(Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucaoPallet, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork, int QteDias)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Pallets.DevolucaoPallet repositorioDevolucaoPallet = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repcargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = devolucaoPallet.CargaPedido;
            int totalDiasDesdeCriacao = (int)DateTime.Now.Subtract(cargaPedido.Carga.DataCriacaoCarga).TotalDays;

            if (totalDiasDesdeCriacao < QteDias)
                return;


            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();
            ocorrencia.Carga = cargaPedido.Carga;
            ocorrencia.Quantidade = 0;
            ocorrencia.TipoOcorrencia = tipoDeOcorrencia;
            ocorrencia.PercentualAcresciomoValor = tipoDeOcorrencia.PercentualAcrescimo;
            ocorrencia.DataOcorrencia = DateTime.Now;
            ocorrencia.DataEvento = DateTime.Now;
            ocorrencia.Observacao = "Geração automática ao superar a quantidade de dias configurados para a devolução de pallets. ";
            ocorrencia.ObservacaoCTe = "";
            ocorrencia.NumeroOcorrenciaCliente = "";
            ocorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            ocorrencia.DataAlteracao = DateTime.Now;
            ocorrencia.Usuario = cargaPedido.Carga.Operador;
            ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgInformacoes;
            ocorrencia.ComplementoValorFreteCarga = ocorrencia.TipoOcorrencia?.OcorrenciaComplementoValorFreteCarga ?? false;
            ocorrencia.Filial = cargaPedido.Carga.Filial;
            ocorrencia.NaoGerarDocumento = tipoDeOcorrencia.NaoGerarDocumento;
            ocorrencia.OrigemOcorrencia = tipoDeOcorrencia.OrigemOcorrencia;
            ocorrencia.ComponenteFrete = tipoDeOcorrencia.ComponenteFrete;
            ocorrencia.ValorOcorrencia = 0;
            ocorrencia.ValorOcorrenciaLiquida = 0;

            SetaEmitenteOcorrencia(ref ocorrencia, unitOfWork, tipoServicoMultisoftware, cargaPedido.Carga.Operador);

            repOcorrencia.Inserir(ocorrencia);

            Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(ocorrencia, new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>(), unitOfWork);

            string mensagemRetorno = string.Empty;

            FluxoGeralOcorrencia(ref ocorrencia, new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>(), null, ref mensagemRetorno, unitOfWork, tipoServicoMultisoftware, cargaPedido.Carga.Operador, configuracao, clienteMultisoftware, "", false, false, null, null, true);

            devolucaoPallet.Situacao = SituacaoDevolucaoPallet.Liquidado;

            repOcorrencia.Atualizar(ocorrencia);
            repositorioDevolucaoPallet.Atualizar(devolucaoPallet);
        }

        public void GerarOcorrenciaPorImportacaoPedido(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoOcorrencia pedidoImportacaoOcorrencia, OrigemSituacaoEntrega origemSituacaoEntrega, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente = Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorNumeroPedidoEmbarcador(pedidoImportacaoOcorrencia.NumeroPedido);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarCargaPedidoPorPedido(pedido?.Codigo ?? 0);

            if (cargaPedido == null)
                throw new ServicoException("Não foi possível localizar a Carga pelo pedido para gerar a Ocorrência");

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = repositorioTipoDeOcorrenciaDeCTe.BuscarPorCodigoIntegracao(pedidoImportacaoOcorrencia.CodigoOcorrencia);

            if (tipoDeOcorrencia == null)
                throw new ServicoException("Não foi possível localizar o Tipo de Ocorrência");

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.EventoCargaEntregaAdicionar eventoCargaEntregaAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.EventoCargaEntregaAdicionar()
            {
                Carga = cargaPedido.Carga,
                CodigosCargaPedidos = new List<int>(),
                DataOcorrencia = pedidoImportacaoOcorrencia?.DataHoraOcorrencia ?? DateTime.Now,
                Latitude = string.Empty,
                Longitude = string.Empty,
                TipoOcorrencia = tipoDeOcorrencia,
                Usuario = usuario,
                Pacote = string.Empty,
                Volumes = 0,
                ValidarNotasFiscaisFinalizadas = true
            };

            eventoCargaEntregaAdicionar.CodigosCargaPedidos.Add(cargaPedido.Codigo);

            bool gerouOcorrenciaCargaEntrega = Carga.ControleEntrega.OcorrenciaEntrega.GerarEventoCargaEntrega(eventoCargaEntregaAdicionar, configuracao, origemSituacaoEntrega, tipoServicoMultisoftware, auditado, unitOfWork, clienteMultisoftware, string.Empty, 0);

            if (!gerouOcorrenciaCargaEntrega)
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.Pedido.ObterTomador() ?? cargaPedido.Pedido.Remetente;

                if (cargaPedido.Pedido.CotacaoPedido != null)
                    tomador = cargaPedido.Pedido.Remetente;

                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega = Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarPedidoOcorrenciaColetaEntrega(tomador, cargaPedido.Pedido, cargaPedido.Carga, tipoDeOcorrencia, configuracaoPortalCliente, eventoCargaEntregaAdicionar.DataOcorrencia, "", "", 0, configuracao, clienteMultisoftware, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(auditado, pedidoOcorrenciaColetaEntrega.Pedido, "Inseriu Ocorrência do Pedido N°" + pedidoOcorrenciaColetaEntrega.Pedido?.NumeroPedidoEmbarcador + " Via importação de planilha", unitOfWork);
            }
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia GerarOcorrenciaPGT(Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal valorOcorrencia, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (carga == null || valorOcorrencia == 0 || tipoOcorrencia == null)
                return null;

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia
            {
                Carga = carga,
                Quantidade = 0,
                DataEvento = DateTime.Now,
                PeriodoInicio = DateTime.Now,
                DataAlteracao = DateTime.Now,
                DataOcorrencia = DateTime.Now,
                DataFinalizacaoEmissaoOcorrencia = DateTime.Now,
                Observacao = "Geração através do fechamento da bonificação do transportador",
                ObservacaoCTe = "Geração através do fechamento da bonificação do transportador",
                NumeroOcorrenciaCliente = "",
                NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork),
                Filial = carga.Filial,
                Usuario = carga.Operador,
                TipoOcorrencia = tipoOcorrencia,
                ComponenteFrete = tipoOcorrencia.ComponenteFrete,
                OrigemOcorrencia = tipoOcorrencia.OrigemOcorrencia,
                NaoGerarDocumento = tipoOcorrencia.NaoGerarDocumento,
                ModeloDocumentoFiscal = tipoOcorrencia.ModeloDocumentoFiscal,
                PercentualAcresciomoValor = tipoOcorrencia.PercentualAcrescimo,
                ComplementoValorFreteCarga = tipoOcorrencia.OcorrenciaComplementoValorFreteCarga,
                ValorOcorrencia = valorOcorrencia,
                ValorOcorrenciaLiquida = valorOcorrencia,
                SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao,
            };

            if (tipoOcorrencia.TomadorTipoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia.Outros && ocorrencia.Tomador == null && ocorrencia.TipoOcorrencia.OutroTomador != null)
            {
                ocorrencia.Responsavel = Dominio.Enumeradores.TipoTomador.Outros;
                ocorrencia.Tomador = tipoOcorrencia.OutroTomador;
            }

            SetaEmitenteOcorrencia(ref ocorrencia, unitOfWork, TipoServicoMultisoftware, carga.Operador);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCte.BuscarPorCarga(carga.Codigo);

            if (ocorrencia.TipoOcorrencia.DataOcorrenciaIgualDataCTeComplementado)
                ocorrencia.DataOcorrencia = cargaCTEs.FirstOrDefault()?.CTe.DataEmissao ?? ocorrencia.DataOcorrencia;

            repOcorrencia.Inserir(ocorrencia);

            Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(ocorrencia, cargaCTEs, unitOfWork);

            string mensagemRetorno = string.Empty;

            if (!FluxoGeralOcorrencia(ref ocorrencia, cargaCTEs, null, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, carga.Operador, configuracaoTMS, clienteMultisoftware, "", false))
            {
                mensagemRetorno += $" (Carga {carga.CodigoCargaEmbarcador})";
                throw new ServicoException(mensagemRetorno);
            }

            repOcorrencia.Atualizar(ocorrencia);

            return ocorrencia;
        }

        public bool ImportarCargaOcorrenciaNOTFIS(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.LayoutEDI layoutEDI, Stream arquivo, Dominio.Entidades.Empresa empresa, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, ref string erro)
        {
            Servicos.LeituraEDI leituraEDI = new LeituraEDI(empresa, layoutEDI, arquivo, unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = leituraEDI.GerarCargaOcorrencias(tipoOcorrencia);


            if (ocorrencias.Count <= 0)
            {
                erro = "Nenhuma ocorrência importada";
                return false;

            }
            erro = "";
            return false;
        }

        public async Task DeletarDadosCargaCTeOcorrenciaAsync(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repositorioComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repositorioCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);

            await repositorioComplementoInfoContaContabilContabilizacao.DeletarPorOcorrenciaAsync(ocorrencia.Codigo);
            await repositorioCargaCTeComplementoInfo.DeletarPorOcorrenciaAsync(ocorrencia.Codigo);
            await repositorioCargaOcorrenciaDocumento.DeletarPorOcorrenciaAsync(ocorrencia.Codigo);
        }

        #endregion

        #region Métodos Privados - Thread

        private void GerarOcorrencia()
        {
            try
            {
                _ocorren = _leituraEDI.GerarOcoren();
                _threadExecutada = true;
            }
            catch
            {
                _threadExecutada = false;
            }
        }

        private bool ThreadGerarOcorrencia()
        {
            Thread thread = new Thread(GerarOcorrencia, 80000000);

            int executionCount = 0;
            _threadExecutada = false;
            thread.Start();

            while (!_threadExecutada)
            {
                executionCount++;

                if (executionCount == 20)
                {
                    thread.Abort();
                    return false;
                }

                System.Threading.Thread.Sleep(500);
            }

            return _threadExecutada;
        }

        #endregion

        #region Métodos Privados

        private static void AdicionarSaldoContratoPrestacaoServico(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (ocorrencia.Carga != null)
            {
                Frete.ContratoPrestacaoServicoSaldo servicoSaldo = new Frete.ContratoPrestacaoServicoSaldo(unitOfWork);

                if (servicoSaldo.IsUtilizaContratoPrestacaoServico())
                {
                    Carga.Carga servicoCarga = new Carga.Carga(unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados dados = new Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados()
                    {
                        CodigoFilial = ocorrencia.Carga.Filial?.Codigo ?? 0,
                        CodigoTransportador = ocorrencia.Carga.Empresa?.Codigo ?? 0,
                        Descricao = $"Saldo referente a carga {servicoCarga.ObterNumeroCarga(ocorrencia.Carga, unitOfWork)}",
                        TipoLancamento = TipoLancamento.Automatico,
                        TipoMovimentacao = TipoMovimentacaoContratoPrestacaoServico.Saida,
                        Valor = ocorrencia.ValorOcorrencia
                    };

                    servicoSaldo.Adicionar(dados);
                }
            }
        }

        private void AtualizarLogLeituraArquivoOcorrencia(int logArquivoOcorrencia, int codigoEmpresa, string motivoInconsistencia, int numeroOcorrenciaGerada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia repositorioLogLeituraArquivoOcorrencia = new Repositorio.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia logLeituraArquivoOcorrencia = repositorioLogLeituraArquivoOcorrencia.BuscarPorCodigo(logArquivoOcorrencia);

            logLeituraArquivoOcorrencia.Empresa = (codigoEmpresa > 0) ? new Dominio.Entidades.Empresa() { Codigo = codigoEmpresa } : null;

            if (!string.IsNullOrWhiteSpace(motivoInconsistencia))
                logLeituraArquivoOcorrencia.MotivoInconsistencia += motivoInconsistencia;

            if (numeroOcorrenciaGerada > 0)
                logLeituraArquivoOcorrencia.OcorrenciasGeradas = $"{logLeituraArquivoOcorrencia.OcorrenciasGeradas}{numeroOcorrenciaGerada}. ";

            repositorioLogLeituraArquivoOcorrencia.Atualizar(logLeituraArquivoOcorrencia);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado> ConverterEntidadesEmObjetosDeValor(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado> cargas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado> objetos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado>();

            objetos.AddRange((from c in cargas
                              select new Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado
                              {
                                  CodigoVeiculo = c.Veiculo.Codigo,
                                  Veiculo = c.Veiculo.Placa,
                                  ModeloVeicular = c.Veiculo.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty,
                                  QuantidadeCargas = c.QuantidadeCargas,
                                  QuantidadeDias = c.QuantidadeDias,
                                  QuantidadeDocumentos = c.QuantidadeDocumentos,
                                  ValorNotas = c.ValorMercadoria,
                              }).ToList());

            return objetos;
        }

        private string CriarOcorrenciaDocumento(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteImportado, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaCargaCTeNotaFiscal> listaOcorrenciaCargaCTeNotaFiscal = null)
        {
            string retorno = "";
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (configuracao.NaoPermitirLancarOcorrenciasDepoisDeOcorrenciaFinalGerada)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaFinalizadora = repCargaOcorrenciaDocumento.BuscarOcorrenciaFinalPorDocumento(cargaCTe.Codigo);
                if (cargaOcorrenciaFinalizadora != null)
                {
                    if (cargaCTe.CTe != null)
                        return "Já foi gerada uma ocorrencia (" + cargaOcorrenciaFinalizadora.CargaOcorrencia.NumeroOcorrencia + ") final para o CT-e " + cargaCTe.CTe.Numero + "";
                    else
                        return "Já foi gerada uma ocorrencia (" + cargaOcorrenciaFinalizadora.CargaOcorrencia.NumeroOcorrencia + ") final para as notas fiscais (" + string.Join(", ", (from nf in cargaCTe.NotasFiscais select nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero).Distinct().ToList()) + ") do Pré CT-e selecionado";
                }
            }

            if (configuracao.NaoPermitirLancarOcorrenciasEmDuplicidadeNaSequencia)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento ultimaOcorrencia = repCargaOcorrenciaDocumento.BuscarUltimaOcorrenciaPorCargaCTe(cargaCTe.Codigo);

                if (ultimaOcorrencia != null && ultimaOcorrencia.CargaOcorrencia.TipoOcorrencia.Codigo == ocorrencia.TipoOcorrencia.Codigo)
                {
                    if (cargaCTe.CTe != null)
                        return "Não é possivel gerar ocorrências na sequencia com o mesmo tipo de ocorrência para o CT-e  " + cargaCTe.CTe.Numero + " (esse tipo de ocorrência já foi gerado na ocorrência " + ultimaOcorrencia.CargaOcorrencia.NumeroOcorrencia + " anteriormente). ";
                    else
                        return "Não é possivel gerar ocorrências na sequencia com o mesmo tipo de ocorrência para as notas fiscais (" + string.Join(", ", (from nf in cargaCTe.NotasFiscais select nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero).Distinct().ToList()) + ") do Pré CT-e selecionado (esse tipo de ocorrência já foi gerado na ocorrência " + ultimaOcorrencia.CargaOcorrencia.NumeroOcorrencia + " anteriormente). ";
                }

            }

            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaCargaCTeNotaFiscal> notasOcorrencia = null;
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlnotafiscais = null;
            if (listaOcorrenciaCargaCTeNotaFiscal != null)
                notasOcorrencia = listaOcorrenciaCargaCTeNotaFiscal.Where(o => o.CodigoCargaCTe == cargaCTe.Codigo).ToList();

            if (ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe)
            {
                if (notasOcorrencia == null || notasOcorrencia.Count() == 0)
                    return "Ocorrência configuração para inclusão por nota e não foi possível identificar as notas selecionadas";

                foreach (Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaCargaCTeNotaFiscal notaOcorrencia in notasOcorrencia)
                {
                    if (xmlnotafiscais == null)
                        xmlnotafiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                    xmlnotafiscais.Add(repXMLNotaFiscal.BuscarPorCodigo(notaOcorrencia.CodigoNotaFiscal));
                }
            }

            Dominio.Entidades.OcorrenciaDeCTe ocorrenciaDeCTe = null;
            if (cargaCTe.CTe != null)
                ocorrenciaDeCTe = GerarOcorrenciaCTe(ocorrencia, cargaCTe, unitOfWork, xmlnotafiscais);

            if (cteImportado != null)
            {
                if (!AjustarCTeImportado(out retorno, cteImportado, ocorrencia.Carga, ocorrencia.ComponenteFrete, unitOfWork))
                    return retorno;
            }

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento
            {
                CargaCTe = cargaCTe,
                CTeImportado = cteImportado,
                CargaOcorrencia = ocorrencia,
                OcorrenciaDeCTe = ocorrenciaDeCTe,
                XMLNotaFiscais = xmlnotafiscais
            };
            repCargaOcorrenciaDocumento.Inserir(cargaOcorrenciaDocumento);

            return retorno;
        }

        private bool GerarAprovacaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, out List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao> notificoes, ref string mensagemRetorno, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool execucaoValePallet, string urlAcesso = "")
        {
            notificoes = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao>();

            if ((ocorrencia.TipoOcorrencia?.TipoOcorrenciaControleEntrega ?? false) && configuracao.PedidoOcorrenciaColetaEntregaIntegracaoNova && (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador || tipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe))
            {
                mensagemRetorno = "Não é permitido gerar ocorrência de valor para Tipo de Ocorrência utilizado para controle de entrega";
                return false;
            }

            bool exigeRegrasComoAutorizacao = configuracao.ObrigatorioRegrasOcorrencia == SimNao.Sim;
            if (exigeRegrasComoAutorizacao)
            {
                ocorrencia.SituacaoOcorrencia = VerificarRegrasAutorizacaoAprovacaoOcorrencia(ocorrencia, out notificoes, tipoServicoMultisoftware, unitOfWork, usuario, urlAcesso);

                if ((ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.EmEmissaoCTeComplementar) && !ocorrencia.DataAprovacao.HasValue)
                    ocorrencia.DataAprovacao = DateTime.Now;
            }

            if (!exigeRegrasComoAutorizacao && !execucaoValePallet)
                ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.EmEmissaoCTeComplementar;

            return true;
        }

        private bool GerarComplementoInfoCargaCTe(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> listaCargaDocumentoParaEmissaoNFSManual, ref string mensagemRetorno, Repositorio.UnitOfWork unitOfWork, bool complementoICMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool permitirAbrirOcorrenciaAposPrazoSolicitacao = false, TipoRateioOcorrenciaLote? tipoRateioOcorrenciaLote = null)
        {
            Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);

            decimal valorOcorrencia = ocorrencia.ValorOcorrencia;
            if (ocorrencia.PercentualAcresciomoValor > 0)
                valorOcorrencia = Math.Round(valorOcorrencia + (valorOcorrencia * (ocorrencia.PercentualAcresciomoValor / 100)), 2, MidpointRounding.ToEven);

            string retornoComplementoInfo = serCargaCTeComplementar.CriarCargaCTeComplementoInfo(cargaCTEs, listaCargaDocumentoParaEmissaoNFSManual, valorOcorrencia, ocorrencia.ObservacaoCTe, ocorrencia, ocorrencia.IncluirICMSFrete, null, ocorrencia.ComponenteFrete, unitOfWork, tipoServicoMultisoftware, complementoICMS, ocorrencia.TipoOcorrencia.TipoEmissaoDocumentoOcorrencia, permitirAbrirOcorrenciaAposPrazoSolicitacao, tipoRateioOcorrenciaLote, ocorrencia.TipoOcorrencia.EmitirDocumentoParaFilialEmissoraComPreCTe);
            if (!string.IsNullOrWhiteSpace(retornoComplementoInfo))
            {
                mensagemRetorno = retornoComplementoInfo;
                Log.TratarErro($"GerarComplementoInfoCargaCTe retorno: {mensagemRetorno} ", "GATILHO");
                return false;
            }

            return true;
        }

        private bool GerarComplementoInfoContrato(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, ref string mensagemRetorno, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool permitirAbrirOcorrenciaAposPrazoSolicitacao = false)
        {
            Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);

            decimal valorOcorrencia = ocorrencia.ValorOcorrencia;
            if (ocorrencia.PercentualAcresciomoValor > 0)
                valorOcorrencia = Math.Round(valorOcorrencia + (valorOcorrencia * (ocorrencia.PercentualAcresciomoValor / 100)), 2, MidpointRounding.ToEven);

            switch (ocorrencia.TipoOcorrencia.TipoEmissaoIntramunicipal)
            {
                case TipoEmissaoIntramunicipal.SempreNFSe:
                    mensagemRetorno = serCargaCTeComplementar.CriaComplementoInfoContrato(valorOcorrencia, ocorrencia.ObservacaoCTe, ocorrencia, ocorrencia.IncluirICMSFrete, null, ocorrencia.ComponenteFrete, tipoServicoMultisoftware, unitOfWork, configuracao);
                    break;
                case TipoEmissaoIntramunicipal.NaoEspecificado:
                    mensagemRetorno = Servicos.Embarcador.Carga.CTeComplementar.CriaComplementoInfoDocumentosAgrupados(ocorrencia, valorOcorrencia, ocorrencia.ObservacaoCTe, ocorrencia.IncluirICMSFrete, ocorrencia.ComponenteFrete, tipoServicoMultisoftware, unitOfWork, configuracao);
                    break;
                default:
                    mensagemRetorno = "A forma de emissão não foi implementada.";
                    return false;
            }

            if (string.IsNullOrWhiteSpace(mensagemRetorno))
                return true;
            else
                return false;
        }

        private bool GerarComplementoInfoPeriodo(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, ref string mensagemRetorno, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool permitirAbrirOcorrenciaAposPrazoSolicitacao = false)
        {
            Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);

            switch (ocorrencia.TipoOcorrencia.TipoEmissaoIntramunicipal)
            {
                case TipoEmissaoIntramunicipal.SempreNFSe:
                case TipoEmissaoIntramunicipal.NaoEspecificado:
                    decimal valorOcorrencia = ocorrencia.ValorOcorrencia;
                    if (ocorrencia.PercentualAcresciomoValor > 0)
                        valorOcorrencia = Math.Round(valorOcorrencia + (valorOcorrencia * (ocorrencia.PercentualAcresciomoValor / 100)), 2, MidpointRounding.ToEven);

                    mensagemRetorno = serCargaCTeComplementar.CriaComplementoInfoPeriodo(valorOcorrencia, ocorrencia.ObservacaoCTe, ocorrencia, ocorrencia.IncluirICMSFrete, null, ocorrencia.ComponenteFrete, tipoServicoMultisoftware, unitOfWork, configuracao, permitirAbrirOcorrenciaAposPrazoSolicitacao);
                    break;
                default:
                    mensagemRetorno = "A forma de emissão não foi implementada.";
                    return false;
            }

            if (string.IsNullOrWhiteSpace(mensagemRetorno))
                return true;
            else
                return false;
        }

        private bool GerarDocumentosDaOcorrencia(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, bool cteEmitidoNoEmbarcador, ref string mensagemRetorno, Repositorio.UnitOfWork unitOfWork, string strCargaCTesImportados, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSManual, List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaCargaCTeNotaFiscal> listaOcorrenciaCargaCTeNotaFiscal = null)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);

            if (cargaDocumentosParaEmissaoNFSManual?.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CargaDocumentoParaEmissaoNFSManualComplementado in cargaDocumentosParaEmissaoNFSManual)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento
                    {
                        CargaOcorrencia = ocorrencia,
                        CargaDocumentoParaEmissaoNFSManualComplementado = CargaDocumentoParaEmissaoNFSManualComplementado
                    };
                    repCargaOcorrenciaDocumento.Inserir(cargaOcorrenciaDocumento);
                }
            }

            if (ocorrencia.ModeloDocumentoFiscal != null || !cteEmitidoNoEmbarcador)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTEs)
                {
                    string retorno = CriarOcorrenciaDocumento(ocorrencia, cargaCTe, null, unitOfWork, configuracao, listaOcorrenciaCargaCTeNotaFiscal);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        mensagemRetorno = retorno;
                        return false;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(strCargaCTesImportados))
                {
                    dynamic dynCtesImportados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(strCargaCTesImportados);
                    string retorno = GerarCargaOcorrenciaDocumentoDosCTesImportados(ocorrencia, dynCtesImportados, unitOfWork, configuracao);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        mensagemRetorno = retorno;
                        return false;
                    }
                }
            }

            return true;
        }

        private string GerarCargaOcorrenciaDocumentoDosCTesImportados(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, dynamic dynCtesImportados, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            string retorno = "";

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            foreach (dynamic dynCteImportado in dynCtesImportados)
            {
                int codigoCargaCTe = (int)dynCteImportado.CodigoCargaCTeParaComplementar;
                int codigoCTeImportado = (int)dynCteImportado.CodigoCTeComplemetarImportado;

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteImportado = repCTe.BuscarPorCodigo(codigoCTeImportado);

                if (cteImportado.CTeSemCarga)
                {
                    cteImportado.CTeSemCarga = false;
                    repCTe.Atualizar(cteImportado);
                    retorno = CriarOcorrenciaDocumento(cargaOcorrencia, cargaCTe, cteImportado, unitOfWork, configuracao);
                }
                else
                {
                    retorno = "O CT-e " + cteImportado.Numero + " já foi utilizado em outra ocorrência.";
                    break;
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcador ObterConfiguracoesComponentes(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

            Servicos.Embarcador.CTe.CTEsImportados svcCTesImportados = new CTe.CTEsImportados(unitOfWork);

            return svcCTesImportados.ObterConfiguracoesComponentes(cargaPedido);
        }

        private decimal RegraCalculoValor(IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado> cargas, decimal valorDiaria)
        {
            decimal totalOperacao = 0;

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado carga in cargas)
            {
                // Quando inicio do código de integração do modelo veicular for "VU" é 1 ajudante
                // Regra provisoria implementada automaticamente
                int numeroAjudantes = carga.ModeloVeicular.ToUpper().StartsWith("VU") ? 1 : 2;

                totalOperacao += carga.QuantidadeDias * valorDiaria * numeroAjudantes;
            }

            return totalOperacao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado> SubstituiDiasPorValoresInformados(IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado> cargas, dynamic cargasComplementadasDias)
        {
            for (int i = 0; i < cargas.Count; i++)
            {
                int diasSetado = 0;

                try
                {
                    diasSetado = (int)cargasComplementadasDias[cargas[i].CodigoVeiculo.ToString()];
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao buscar dias complementados por código de veículo em array: {ex.ToString()}", "CatchNoAction");
                }

                if (diasSetado > 0)
                    cargas[i].QuantidadeDias = diasSetado;
            }

            return cargas;
        }

        private void NotificarTransportadorPorEmail(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, ref Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa)
        {
            if (!string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.AssuntoEmailNotificacao) && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CorpoEmailNotificacao) && ocorrencia.Carga?.Empresa != null)
            {
                Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(unitOfWork);
                notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                {
                    AssuntoEmail = RetornarTextoEmailTransportador(ocorrencia.TipoOcorrencia.AssuntoEmailNotificacao, ocorrencia),
                    Mensagem = RetornarTextoEmailTransportador(ocorrencia.TipoOcorrencia.CorpoEmailNotificacao, ocorrencia),
                    Empresa = ocorrencia.Carga.Empresa,
                    NotificarSomenteEmailPrincipal = true
                };

                servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmpresa);
            }
        }

        private void NotificarOcorrenciaPorEmail(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa)
        {
            if (!string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.AssuntoEmailNotificacao) && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CorpoEmailNotificacao) && ocorrencia.TipoOcorrencia.EmailsNotificacao?.Count > 0)
            {
                EnviarEmailOcorrencia(ocorrencia, unitOfWork, notificacaoEmpresa);
            }
        }

        public void EnviarEmailOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa)
        {
            try
            {
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork).BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                string de = configuracaoEmail.Email;
                string login = configuracaoEmail.Email;
                string senha = configuracaoEmail.Senha;
                string[] copiaOcultaPara = new string[] { };
                string[] copiaPara = new string[] { };
                string corpo = ObterBodyEmailNotificacao(ocorrencia);
                string servidorSMTP = configuracaoEmail.Smtp;
                List<System.Net.Mail.Attachment> anexos = null;
                string assinatura = "";
                bool possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                string responderPara = "";
                int porta = configuracaoEmail.PortaSmtp;

                foreach (string para in ocorrencia.TipoOcorrencia.EmailsNotificacao.ToList().Distinct())
                {
                    if (!Servicos.Email.EnviarEmail(de, login, senha, para, copiaOcultaPara, copiaPara, notificacaoEmpresa.AssuntoEmail, corpo, servidorSMTP, out string erro, configuracaoEmail.DisplayEmail, anexos, assinatura, possuiSSL, responderPara, porta, unitOfWork))
                        Log.TratarErro(erro, "EnviarEmailOcorrencia");
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "EnviarEmailOcorrencia");
            }
        }

        public void NotificarClientePorEmail(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa)
        {
            if (!string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.AssuntoEmailNotificacao) && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CorpoEmailNotificacao) && !string.IsNullOrWhiteSpace(ocorrencia.Tomador.Email))
            {
                EnviarEmailClienteOcorrencia(ocorrencia, unitOfWork, notificacaoEmpresa);
            }
        }

        private void EnviarEmailClienteOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa)
        {
            try
            {
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork).BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                string de = configuracaoEmail.Email;
                string login = configuracaoEmail.Email;
                string senha = configuracaoEmail.Senha;
                string[] copiaOcultaPara = new string[] { };
                string[] copiaPara = new string[] { };
                string corpo = ObterBodyEmailNotificacao(ocorrencia);
                string assunto = RetornarTextoEmailTransportador(ocorrencia.TipoOcorrencia.AssuntoEmailNotificacao, ocorrencia);
                string servidorSMTP = configuracaoEmail.Smtp;
                List<System.Net.Mail.Attachment> anexos = null;
                string assinatura = "";
                bool possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                string responderPara = "";
                int porta = configuracaoEmail.PortaSmtp;
                string para = ocorrencia.Tomador.Email;

                if (!Servicos.Email.EnviarEmail(de, login, senha, para, copiaOcultaPara, copiaPara, assunto, corpo, servidorSMTP, out string erro, configuracaoEmail.DisplayEmail, anexos, assinatura, possuiSSL, responderPara, porta, unitOfWork))
                    Log.TratarErro(erro, "EnviarEmailClienteOcorrencia");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "EnviarEmailClienteOcorrencia");
            }
        }

        private string ObterBodyEmailNotificacao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine(@"<div style=""font-family: Arial;"">");
            body.AppendLine($@"    <p style=""margin:0px"">{RetornarTextoEmailTransportador(ocorrencia.TipoOcorrencia.CorpoEmailNotificacao, ocorrencia)}</p>");
            body.AppendLine($@"    <p style=""font-size: 12px; margin:0px"">{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");
            body.AppendLine("    <p></p>");
            body.AppendLine(@"    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>");
            body.AppendLine("</div>");

            return body.ToString();
        }

        #endregion
    }
}
