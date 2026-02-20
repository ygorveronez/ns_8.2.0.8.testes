using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Servicos.EDI
{
    public class StartupEDIDanone : ServicoBase
    {
        #region Atributos Privados

        private int tempoThread = 5000;
        private string caminhoArquivos = Servicos.FS.GetPath(@"D:\Arquivos\FTP");
        private string caminhoRaiz = Servicos.FS.GetPath(@"D:\Arquivos");
        private string TipoArmazenamento = "pasta";
        private string EnderecoFTP = "";
        private string UsuarioFTP = "";
        private string SenhaFTP = "";
        private string CaminhoRaizFTP = "";
        private bool FTPPassivo = true;
        private string PortaFTP = "21";
        private bool UtilizaSFTP = false;
        private string AdminStringConexao = "";
        private string CaminhoBatReiniciar = "";
        private string CNPJsNovoPadrao = "";
        private bool ValidarPorCNPJNovoPadrao = false;
        private Dominio.Enumeradores.LoteCalculoFrete CargaPorLote = Dominio.Enumeradores.LoteCalculoFrete.Padrao;
        private int ExceptionsSeguidas = 0;
        private List<string> Prefixos = null;

        #endregion

        #region Construtores

        public StartupEDIDanone(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #endregion

        #region Métodos Públicos

        public void Iniciar(string caminhoRaizArquivos, string tipoArmazenamento, string enderecoFTP, string usuarioFTP, string senhaFTP, string caminhoRaizFTP, bool ftpPassivo, string portaFTP, bool utilizaSFTP, string adminMultisoftware, string caminhoBatReiniciar, string cnpjsNovoPadrao, bool validarPorCNPJNovoPadrao, int tamanhoStack, string prefixos)
        {
            Thread thread = new Thread(new ThreadStart(ExecutarThread), tamanhoStack);
            thread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
            thread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            thread.IsBackground = true;
            caminhoRaiz = caminhoRaizArquivos;
            caminhoArquivos = caminhoRaizArquivos + @"\FTP";
            TipoArmazenamento = tipoArmazenamento;
            EnderecoFTP = enderecoFTP;
            UsuarioFTP = usuarioFTP;
            SenhaFTP = senhaFTP;
            CaminhoRaizFTP = caminhoRaizFTP;
            FTPPassivo = ftpPassivo;
            CaminhoBatReiniciar = caminhoBatReiniciar;
            PortaFTP = portaFTP;
            UtilizaSFTP = utilizaSFTP;
            AdminStringConexao = adminMultisoftware;
            CNPJsNovoPadrao = cnpjsNovoPadrao;
            ValidarPorCNPJNovoPadrao = validarPorCNPJNovoPadrao;
            string[] splitPrefixos = prefixos?.Split(';');
            Prefixos = new List<string>();
            if (splitPrefixos != null)
                for (int i = 0; i < splitPrefixos.Length; i++)
                {
                    Prefixos.Add(splitPrefixos[i]);
                }
            thread.Start();
        }

        #endregion

        #region Métodos Privados

        private void ExecutarThread()
        {
            Log.TratarErro("Iniciou Task");

            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(tempoThread);
                    
                    using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho);

                        //IntegrarNotFisPendente(unidadeDeTrabalho);
                        IntegrarPendentes(unidadeDeTrabalho);
                        BuscarArquivos(unidadeDeTrabalho);
                        unidadeDeTrabalho.Dispose();
                    }
                }
                catch (System.ServiceModel.CommunicationException com)
                {
                    Servicos.Log.TratarErro("Comunication: " + com);
                    System.Threading.Thread.Sleep(tempoThread);
                }
                catch (TimeoutException ti)
                {
                    Servicos.Log.TratarErro("Time out: " + ti);
                    System.Threading.Thread.Sleep(tempoThread);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    System.Threading.Thread.Sleep(tempoThread);
                }
            }
        }

        private bool IsProcessarSomentePrioritario(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI repositorioConfiguracaoControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI(unitOfWork);
            bool processarSomentePrioritario = repositorioConfiguracaoControleIntegracaoCargaEDI.BuscarConfiguracao()?.ProcessarSomentePrioritario ?? false;

            return processarSomentePrioritario;
        }

        #endregion

        #region Métodos Privados para Buscar Arquivos

        private void BuscarArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                IEnumerable<string> arquivos = ObterArquivos();

                if ((arquivos == null) || (arquivos.Count() == 0))
                    return;

                BuscarArquivosNOTFIS(arquivos, unitOfWork);
                BuscarArquivosTransportationPlann(arquivos, unitOfWork);
            }
            catch (Exception ex)
            {

                if (!ex.ToString().Contains("deadlocked on lock") && !ex.ToString().Contains("current state is closed") && !ex.ToString().Contains("name is no longer available"))
                    throw ex;
                else
                {
                    Servicos.Log.TratarErro("pegou deadlocked");

                    if (!string.IsNullOrWhiteSpace(CaminhoBatReiniciar))
                    {
                        Servicos.Log.TratarErro("Solicitou bat reinicia");
                        System.Diagnostics.Process.Start(CaminhoBatReiniciar);
                    }
                    return;
                }
            }
            finally
            {
                unitOfWork.FlushAndClear();
            }
        }

        private void BuscarArquivosNOTFIS(IEnumerable<string> arquivos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            Repositorio.LayoutEDI repositorioLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            Dominio.Entidades.LayoutEDI layoutEDI = repositorioLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS, "INTDNE DANONE").FirstOrDefault();
            bool processarSomentePrioritario = IsProcessarSomentePrioritario(unitOfWork);

            foreach (string arquivo in arquivos)
            {
                string fileName = Path.GetFileName(arquivo);
                if (fileName.Substring(0, 2).ToLower() != "cl")
                    continue;

                using (MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo)))
                {
                    try
                    {
                        LeituraEDI leituraEDI = new LeituraEDI(null, layoutEDI, ms, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.UTF8);
                        Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = leituraEDI.GerarNotasFis();
                        Dominio.ObjetosDeValor.EDI.Notfis.CabecalhoDocumento cabecalhoDocumento = notfis.CabecalhosDocumento.FirstOrDefault();
                        Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI()
                        {
                            Data = DateTime.Now,
                            MensagemRetorno = "",
                            NumeroDT = cabecalhoDocumento.CodigoRemessa,
                            Placa = cabecalhoDocumento.Veiculo.Placa,
                            Transportador = ObterEmpresa(cabecalhoDocumento.Empresa.CNPJ, unitOfWork),
                            NomeArquivo = fileName,
                            GuidArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivo),
                            NumeroTentativas = 0,
                            SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.AgIntegracao
                        };

                        repositorioControleIntegracaoCargaEDI.Inserir(controleIntegracaoCargaEDI);

                        if (processarSomentePrioritario)
                        {
                            List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> controles = repositorioControleIntegracaoCargaEDI.BuscarPorDT(controleIntegracaoCargaEDI.NumeroDT, controleIntegracaoCargaEDI.Codigo);

                            foreach (Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controle in controles)
                            {
                                controle.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI.Cancelada;
                                controle.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
                                controle.MensagemRetorno = Localization.Resources.Cargas.ControleGeracaoEDI.NovoIDOCMesmoNumeroDT;

                                repositorioControleIntegracaoCargaEDI.Atualizar(controle);
                            }
                        }

                        MoverArquivoParaPastaProcessados(controleIntegracaoCargaEDI.GuidArquivo, arquivo);
                    }
                    catch (Exception excecao)
                    {
                        Log.TratarErro($"Não foi possível interpretar o arquivo [{fileName}] de NOTFIS da Danone");
                        Log.TratarErro(excecao);
                    }

                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                }
            }
        }

        private void BuscarArquivosTransportationPlann(IEnumerable<string> arquivos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            Repositorio.LayoutEDI repositorioLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            Dominio.Entidades.LayoutEDI layoutEDI = repositorioLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.TransportationPlann, "Transportation Plann DANONE").FirstOrDefault();

            foreach (string arquivo in arquivos)
            {
                string fileName = Path.GetFileName(arquivo);

                if (fileName.Substring(0, 5).ToLower() != "shpnf")
                    continue;

                using (MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo)))
                {
                    try
                    {
                        LeituraEDI leituraEDI = new LeituraEDI(null, layoutEDI, ms, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.UTF8);
                        Dominio.ObjetosDeValor.EDI.TransportationPlann.EDITransportationPlann transportationPlann = leituraEDI.GerarTransportationPlann();
                        IEnumerable<Dominio.ObjetosDeValor.EDI.TransportationPlann.PreCarga> preCargas = transportationPlann.PreCargas?.Distinct();

                        if (preCargas?.Count() > 0)
                        {
                            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI()
                            {
                                Data = DateTime.Now,
                                MensagemRetorno = "",
                                NumeroDT = string.Join(", ", (from preCarga in preCargas select preCarga.NumeroPreCarga).Distinct()),
                                Placa = string.Join(", ", (from preCarga in preCargas select preCarga.PlacaVeiculo).Distinct()),
                                NomeArquivo = fileName,
                                GuidArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivo),
                                NumeroTentativas = 0,
                                SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.AgIntegracao
                            };

                            repositorioControleIntegracaoCargaEDI.Inserir(controleIntegracaoCargaEDI);

                            MoverArquivoParaPastaProcessados(controleIntegracaoCargaEDI.GuidArquivo, arquivo);
                        }
                    }
                    catch (Exception excecao)
                    {
                        Log.TratarErro($"Não foi possível interpretar o arquivo [{fileName}] de Transportation Plann da Danone");
                        Log.TratarErro(excecao);
                    }

                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                }
            }
        }

        private void MoverArquivoParaPastaProcessados(string nomeArquivo, string fullName)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Utilidades.IO.FileStorageService.Storage.ReadAllText(fullName));

            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "Processados", nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoCompleto, bytes);
        }

        private IEnumerable<string> ObterArquivos()
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "Enviados", "Notfis\\");

            if (TipoArmazenamento == "ftp")
            {
                string pastaFTP = "input";
                string caminhoFTP = CaminhoRaizFTP + pastaFTP.Replace(@"\", "/");
                string erro = "";
                FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, caminho, out erro, UtilizaSFTP, false, "", true, false, false, true, Prefixos);

                if (!string.IsNullOrWhiteSpace(erro))
                {
                    Servicos.Log.TratarErro(erro);

                    if (!string.IsNullOrWhiteSpace(CaminhoBatReiniciar))
                    {
                        Servicos.Log.TratarErro("Solicitou bat reinicia");
                        System.Diagnostics.Process.Start(CaminhoBatReiniciar);
                    }

                    return null;
                }
            }

            IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*.txt", SearchOption.AllDirectories).AsParallel();

            return arquivos;
        }

        private Dominio.Entidades.Empresa ObterEmpresa(string cnpjEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            long? cnpj = cnpjEmpresa.ToNullableLong();

            if (!cnpj.HasValue)
                return null;

            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCNPJ(cnpj.Value.ToString("d14"));

            return empresa;
        }

        #endregion

        #region Métodos Privados para Integrar Pendentes

        private Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();
            Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();

            StringBuilder stMensagem = new StringBuilder();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga(unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Embarcador.Carga.RateioFrete(unitOfWork);
            Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Embarcador.PreCarga.PreCarga(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.IntegracaoCargaMultiEmbarcador,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Texto = "EDI Danone - StartupEDIDanone.AdicionarCarga"
            };

            try
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao ?? "");
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (cargaIntegracao.TipoOperacao != null) ? repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracao.TipoOperacao.CodigoIntegracao) : null;
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                unitOfWork.Start();
                int codigoCargaExistente = 0;
                int protocoloPedidoExistente = 0;

                Servicos.Log.TratarErro("Iniciou Criar Pedido" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");


                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, ref protocoloPedidoExistente, ref codigoCargaExistente, false, null, configuracao);
                Servicos.Log.TratarErro("Finalizou Criar Pedido" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                int cargaCodigo = 0;

                if (stMensagem.Length == 0 || protocoloPedidoExistente > 0)
                {
                    if (protocoloPedidoExistente == 0)
                    {
                        serProdutoPedidoWS.AdicionarProdutosPedido(pedido, configuracao, cargaIntegracao, ref stMensagem, unitOfWork, auditado);
                        AtualizarDadosVendedor(pedido, unitOfWork);
                    }

                    if (cargaIntegracao.CanalEntrega == null || cargaIntegracao.CanalEntrega.CodigoIntegracao != "PRODUTOR")
                    {
                        Servicos.Log.TratarErro("Iniciou Gerar Carga" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = serCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref stMensagem, ref codigoCargaExistente, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, false, false, auditado, configuracao, null, "", filial, tipoOperacao);
                        Servicos.Log.TratarErro("Finalizou Gerar Carga" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");

                        int codCarga = cargaPedido != null ? cargaPedido.Carga.Codigo : 0;

                        if (cargaPedido != null)
                        {
                            serRateioFrete.GerarComponenteICMS(cargaPedido, false, unitOfWork);
                            if (cargaPedido.CargaPedidoFilialEmissora)
                                serRateioFrete.GerarComponenteICMS(cargaPedido, true, unitOfWork);

                            serRateioFrete.GerarComponenteISS(cargaPedido, false, unitOfWork);
                            serCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref stMensagem, unitOfWork, false);
                            cargaCodigo = cargaPedido.Carga.Codigo;
                        }
                    }
                    else
                    {


                        Servicos.Embarcador.Pedido.Pedido serPedido = new Embarcador.Pedido.Pedido(unitOfWork);
                        pedido.ValorTotalNotasFiscais = 0;
                        serPedido.AdicionarNotaFiscal(pedido, cargaIntegracao.NotasFiscais.FirstOrDefault(), unitOfWork);

                        Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = servicoPreCarga.CriarPreCarga(cargaIntegracao, pedido, ref stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                        if (preCarga != null && stMensagem.Length == 0)
                        {
                            Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor repPedidoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor(unitOfWork);

                            pedido.PreCarga = preCarga;
                            repPedido.Atualizar(pedido);
                            Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor pedidoColetaProdutor = new Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor();
                            pedidoColetaProdutor.Pedido = pedido;
                            pedidoColetaProdutor.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor.AgFechamento;
                            repPedidoColetaProdutor.Inserir(pedidoColetaProdutor);
                        }

                    }
                }

                if (stMensagem.Length > 0)
                {

                    unitOfWork.Rollback();
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = stMensagem.ToString();
                    retorno.Objeto = null;
                    if (codigoCargaExistente > 0 && protocoloPedidoExistente > 0)
                    {
                        retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = codigoCargaExistente, protocoloIntegracaoPedido = protocoloPedidoExistente };
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                    }
                }
                else
                {
                    unitOfWork.CommitChanges();
                    retorno.Status = true;
                    retorno.Mensagem = "";
                    retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = cargaCodigo, protocoloIntegracaoPedido = pedido.Codigo };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                Servicos.Log.TratarErro("Carga: " + cargaIntegracao.NumeroCarga + " Retornou exceção a seguir:");
                retorno.Mensagem = "Ocorreu uma falha ao obter os dados das integrações. " + stMensagem.ToString();
                retorno.Objeto = null;
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            return retorno;
        }

        private void AdicionarCargaJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigoCargaCancelada, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (carga.TipoDeCarga == null)
                return;

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);

            if (repositorioCentroCarregamento.ContarPorTipoCargaEFilial(carga.TipoDeCarga.Codigo, carga.Filial.Codigo) == 0)
                return;

            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

            if (cargaJanelaCarregamento == null)
                cargaJanelaCarregamento = servicoCargaJanelaCarregamento.AdicionarPorCarga(carga.Codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            else
                servicoCargaJanelaCarregamento.AtualizarPorCarga(cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

            if ((codigoCargaCancelada > 0) && (cargaJanelaCarregamento != null))
            {
                cargaJanelaCarregamento.ObservacaoFluxoPatio = repositorioCargaJanelaCarregamento.BuscarObservacaoFluxoPatioPorCarga(codigoCargaCancelada);

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
            }
        }

        private void AtualizarDadosVendedor(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedido.Destinatario == null)
                return;

            Repositorio.Embarcador.Pessoas.PessoaFuncionario repositorioPessoaFuncionario = new Repositorio.Embarcador.Pessoas.PessoaFuncionario(unitOfWork);
            Dominio.Entidades.Usuario vendedor = repositorioPessoaFuncionario.BuscarVendedorPorPessoaETipoCarga(pedido.Destinatario.CPF_CNPJ, pedido.TipoDeCarga?.Codigo ?? 0);

            if (vendedor == null)
                return;

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            pedido.FuncionarioVendedor = vendedor;
            pedido.FuncionarioSupervisor = vendedor.Supervisor;
            pedido.FuncionarioGerente = vendedor.Gerente;

            repositorioPedido.Atualizar(pedido);
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.Produto CriarProduto(Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produtoNF, Dominio.ObjetosDeValor.Embarcador.NFe.ComplementoProduto complementoProduto, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = null;
            if (produtoNF != null && !string.IsNullOrWhiteSpace(produtoNF.Descricao))
            {
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repProdutoEmbarcador.buscarPorCodigoEmbarcador(produtoNF.Codigo);
                produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                if (produtoEmbarcador == null)
                {
                    produto.CodigoGrupoProduto = "1";
                    produto.DescricaoGrupoProduto = "Danone";
                }
                else
                {
                    produto.CodigoGrupoProduto = produtoEmbarcador.GrupoProduto != null ? produtoEmbarcador.GrupoProduto.CodigoGrupoProdutoEmbarcador : "1";
                    produto.DescricaoGrupoProduto = produtoEmbarcador.GrupoProduto != null ? produtoEmbarcador.GrupoProduto.Descricao : "Danone";
                }

                produto.CodigoNCM = string.IsNullOrWhiteSpace(produtoNF.NCM) ? produtoEmbarcador?.CodigoNCM : produtoNF.NCM;

                produto.CodigoProduto = produtoNF.Codigo;
                produto.DescricaoProduto = produtoNF.Descricao;
                produto.Quantidade = complementoProduto.Quantidade;
                produto.QuantidadeCaixa = produtoEmbarcador?.QuantidadeCaixa ?? 0;
                produto.PesoTotalEmbalagem = complementoProduto.PesoBruto - complementoProduto.PesoLiquido;
                if (produto.Quantidade > 0)
                {
                    produto.ValorUnitario = complementoProduto.Valor / produto.Quantidade;
                    produto.PesoUnitario = complementoProduto.PesoLiquido / produto.Quantidade;
                }
                else
                    produto.PesoUnitario = produtoEmbarcador?.PesoUnitario ?? 1;


            }
            return produto;
        }

        //private Dominio.ObjetosDeValor.Embarcador.Pedido.Produto CriarProduto(Dominio.ObjetosDeValor.EDI.Notfis.DadosMercadoria dadosMercadoria, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = null;
        //    if (dadosMercadoria != null && !string.IsNullOrWhiteSpace(dadosMercadoria.Mercadoria))
        //    {
        //        Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
        //        Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repProdutoEmbarcador.buscarPorCodigoEmbarcador(dadosMercadoria.Mercadoria);

        //        produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
        //        if (produtoEmbarcador == null)
        //        {
        //            produto.CodigoProduto = dadosMercadoria.Mercadoria;
        //            produto.DescricaoProduto = dadosMercadoria.Mercadoria + " Danone";
        //            produto.CodigoGrupoProduto = "1";
        //            produto.DescricaoGrupoProduto = "Danone";
        //            produto.Quantidade = dadosMercadoria.QuantidadeVolume;
        //            produto.PesoUnitario = 1;
        //        }
        //        else
        //        {
        //            produto.CodigoProduto = produtoEmbarcador.CodigoProdutoEmbarcador;
        //            produto.DescricaoProduto = produtoEmbarcador.Descricao;
        //            produto.CodigoGrupoProduto = produtoEmbarcador.GrupoProduto != null ? produtoEmbarcador.GrupoProduto.CodigoGrupoProdutoEmbarcador : "1";
        //            produto.DescricaoGrupoProduto = produtoEmbarcador.GrupoProduto != null ? produtoEmbarcador.GrupoProduto.Descricao : "Danone";
        //            produto.Quantidade = dadosMercadoria.QuantidadeVolume;
        //            produto.PesoUnitario = produtoEmbarcador.PesoUnitario;
        //        }
        //    }

        //    return produto;
        //}

        private void DefinirDadoscontroleIntegracaoCargaEDI(Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI, Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados dadosIntegracaoEDISumarizados, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork)
        {
            if (dadosIntegracaoEDISumarizados != null)
            {
                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao repositorioControleIntegracaoCargaEDIAlteracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao controleAnterior = repositorioControleIntegracaoCargaEDIAlteracao.BuscarUltimaPorCargaEFilial(dadosIntegracaoEDISumarizados.NumeroDT, dadosIntegracaoEDISumarizados.Filial);

                controleIntegracaoCargaEDI.NumeroDT = dadosIntegracaoEDISumarizados.NumeroDT;
                controleIntegracaoCargaEDI.IDOC = dadosIntegracaoEDISumarizados.IDDOC;
                controleIntegracaoCargaEDI.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI.AguardandoEmissao;
                controleIntegracaoCargaEDI.DataAtualizacaoSituacaoCarga = DateTime.Now;
                controleIntegracaoCargaEDI.Placa = dadosIntegracaoEDISumarizados.Placa;
                controleIntegracaoCargaEDI.Transportador = dadosIntegracaoEDISumarizados.Empresa > 0 ? new Dominio.Entidades.Empresa() { Codigo = dadosIntegracaoEDISumarizados.Empresa } : null;

                if (cargas != null && cargas.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                        DefinirDadoscontroleIntegracaoCargaEDIAlteracao(controleIntegracaoCargaEDI, controleAnterior, dadosIntegracaoEDISumarizados, carga, unitOfWork);
                }
                else
                    DefinirDadoscontroleIntegracaoCargaEDIAlteracao(controleIntegracaoCargaEDI, controleAnterior, dadosIntegracaoEDISumarizados, null, unitOfWork);
            }
        }

        private void DefinirDadoscontroleIntegracaoCargaEDIAlteracao(Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI, Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao controleAnterior, Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados dadosIntegracaoEDISumarizados, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao repositorioControleIntegracaoCargaEDIAlteracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao(unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao controleIntegracaoCargaEDIAlteracao = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao();

            controleIntegracaoCargaEDIAlteracao.Carga = carga;
            controleIntegracaoCargaEDIAlteracao.ControleIntegracaoCargaEDI = controleIntegracaoCargaEDI;
            controleIntegracaoCargaEDIAlteracao.MeioTransporteAnterior = controleAnterior?.MeioTransporteAtual ?? "";
            controleIntegracaoCargaEDIAlteracao.MeioTransporteAtual = dadosIntegracaoEDISumarizados.MeioTransporte;
            controleIntegracaoCargaEDIAlteracao.ModeloVeicularAnterior = controleAnterior?.ModeloVeicularAtual ?? "";
            controleIntegracaoCargaEDIAlteracao.ModeloVeicularAtual = dadosIntegracaoEDISumarizados.ModeloVeicular;
            controleIntegracaoCargaEDIAlteracao.PlacaAnterior = controleAnterior?.PlacaAtual ?? "";
            controleIntegracaoCargaEDIAlteracao.PlacaAtual = dadosIntegracaoEDISumarizados.Placa;
            controleIntegracaoCargaEDIAlteracao.QuantidadeNfsAnterior = controleAnterior?.QuantidadeNfsAtual ?? 0;
            controleIntegracaoCargaEDIAlteracao.QuantidadeNfsAtual = dadosIntegracaoEDISumarizados.QuantidadeNfs;
            controleIntegracaoCargaEDIAlteracao.RoteiroAnterior = controleAnterior?.RoteiroAtual ?? "";
            controleIntegracaoCargaEDIAlteracao.RoteiroAtual = dadosIntegracaoEDISumarizados.Roteiro;

            repositorioControleIntegracaoCargaEDIAlteracao.Inserir(controleIntegracaoCargaEDIAlteracao);
        }

        private void DefinirIntegracaoEDIRejeitada(int codigo, string mensagem, Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados dadosIntegracaoEDISumarizados, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();

            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = repositorioControleIntegracaoCargaEDI.BuscarPorCodigo(codigo);

            DefinirDadoscontroleIntegracaoCargaEDI(controleIntegracaoCargaEDI, dadosIntegracaoEDISumarizados, null, unitOfWork);

            controleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Falha;
            controleIntegracaoCargaEDI.MensagemRetorno = mensagem;
            controleIntegracaoCargaEDI.NumeroTentativas++;
            controleIntegracaoCargaEDI.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI.NaoImportada;

            repositorioControleIntegracaoCargaEDI.Atualizar(controleIntegracaoCargaEDI);

            unitOfWork.CommitChanges();
        }

        private void IntegrarNTOFISPendentes(Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.LayoutEDI repositorioLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPrecarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);


            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Embarcador.PreCarga.PreCarga(unitOfWork);
            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();

            Dominio.Entidades.LayoutEDI layoutEDI = repositorioLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS, "INTDNE DANONE").FirstOrDefault();
            Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados dadosIntegracaoEDISumarizados = null;

            try
            {
                string caminho = caminhoRaiz + @"\Processados\";
                string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleIntegracaoCargaEDI.GuidArquivo);
                System.IO.MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoCompleto));
                ms.Position = 0;
                Servicos.LeituraEDI leituraEDI = new Servicos.LeituraEDI(null, layoutEDI, ms, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.UTF8);
                Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = leituraEDI.GerarNotasFis();

                Servicos.Log.TratarErro("Iniciou processsamento arquivo" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");

                List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracoes = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>();
                List<(int CodigoCarga, string CodigoCargaEmbarcador, string CodigoIntegracaoFilial)> cargasCanceladas = new List<(int CodigoCarga, string CodigoCargaEmbarcador, string CodigoIntegracaoFilial)>();

                for (int cab = 0; cab < notfis.CabecalhosDocumento.Count; cab++)
                {
                    Dominio.ObjetosDeValor.EDI.Notfis.CabecalhoDocumento cabecalhoDocumento = notfis.CabecalhosDocumento[cab];
                    Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcador = cabecalhoDocumento.Embarcador;

                    List<Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal> todasNotas = cabecalhoDocumento.Destinatarios.SelectMany(obj => obj.NotasFiscais).ToList();
                    List<string> TiposOperacao = (from obj in todasNotas where obj.ComplementoNotaFiscal.CGCEntrega2 == "TIPO TRANSPORTE" select obj.ComplementoNotaFiscal.SerieEntrega2).Distinct().ToList();
                    TiposOperacao.AddRange((from obj in todasNotas where obj.ComplementoNotaFiscal.CGCEntrega3 == "TIPO TRANSPORTE" select obj.ComplementoNotaFiscal.SerieEntrega3).Distinct().ToList());

                    List<string> canaisEntrega = (from obj in todasNotas select obj.ComplementoNotaFiscal.SerieEntrega1).Distinct().ToList();

                    bool canalProdutor = (from obj in canaisEntrega where obj == "PRODUTOR" select obj).Any();

                    int sequenciaGlobal = 0;

                    string[] splitArquivo = controleIntegracaoCargaEDI.NomeArquivo.Split('_');

                    dadosIntegracaoEDISumarizados = new Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados();

                    if (splitArquivo.Length > 2)
                        dadosIntegracaoEDISumarizados.IDDOC = splitArquivo[2];

                    dadosIntegracaoEDISumarizados.MeioTransporte = string.Join(",", (from obj in TiposOperacao select obj).ToList());
                    dadosIntegracaoEDISumarizados.Filial = embarcador.Pessoa.CPFCNPJ;
                    dadosIntegracaoEDISumarizados.ModeloVeicular = cabecalhoDocumento.Veiculo.CodigoModeloVeicularEmbarcador;
                    dadosIntegracaoEDISumarizados.NumeroDT = cabecalhoDocumento.CodigoRemessa;
                    dadosIntegracaoEDISumarizados.Placa = cabecalhoDocumento.Veiculo.Placa;

                    foreach (string codigoTipoOperacao in TiposOperacao)
                    {
                        List<Dominio.ObjetosDeValor.EDI.Notfis.Destinatario> destinatariosPorTipo = (
                            from obj in cabecalhoDocumento.Destinatarios
                            where obj.NotasFiscais.Any(nf =>
                                (nf.ComplementoNotaFiscal.CGCEntrega2 == "TIPO TRANSPORTE" && nf.ComplementoNotaFiscal.SerieEntrega2 == codigoTipoOperacao) ||
                                (nf.ComplementoNotaFiscal.CGCEntrega3 == "TIPO TRANSPORTE" && nf.ComplementoNotaFiscal.SerieEntrega3 == codigoTipoOperacao)
                            )
                            select obj
                        ).ToList();

                        List<string> cnpjDestinatarioAgrupado = null;
                        if (!canalProdutor)
                            cnpjDestinatarioAgrupado = (from obj in destinatariosPorTipo select obj.Pessoa.CPFCNPJ).Distinct().ToList();
                        else
                            cnpjDestinatarioAgrupado = (from obj in destinatariosPorTipo select obj.Pessoa.CPFCNPJ).ToList();

                        List<string> chavesNF = new List<string>();

                        for (int idxCNPJDest = 0; idxCNPJDest < cnpjDestinatarioAgrupado.Count; idxCNPJDest++)
                        {
                            sequenciaGlobal++;
                            List<Dominio.ObjetosDeValor.EDI.Notfis.Destinatario> destinatarios = null;

                            if (!canalProdutor)
                                destinatarios = (from obj in destinatariosPorTipo where obj.Pessoa.CPFCNPJ == cnpjDestinatarioAgrupado[idxCNPJDest] select obj).ToList();
                            else
                            {
                                destinatarios = new List<Dominio.ObjetosDeValor.EDI.Notfis.Destinatario>();
                                destinatarios.Add(destinatariosPorTipo[idxCNPJDest]);
                            }

                            Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatario = destinatarios.FirstOrDefault();

                            //destinatario.Pessoa.AtualizarEnderecoPessoa = true;
                            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();
                            cargaIntegracao.DataCriacaoCarga = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                            double cnpjDestinatario = double.Parse(Utilidades.String.OnlyNumbers(destinatario.Pessoa.CPFCNPJ));
                            Dominio.Entidades.Cliente clienteDestinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario);
                            string retorno = clienteDestinatario == null ? ValidarEndereco(destinatario.Pessoa.Endereco, unitOfWork) : "";

                            cargaIntegracao.Distancia = cabecalhoDocumento.Distancia;

                            if (!string.IsNullOrWhiteSpace(retorno))
                                throw new ServicoException(retorno);

                            cargaIntegracao.Destinatario = destinatario.Pessoa;

                            cargaIntegracao.Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = embarcador.Pessoa.CPFCNPJ };

                            cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
                            cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular { CodigoIntegracao = cabecalhoDocumento.Veiculo.CodigoModeloVeicularEmbarcador };

                            int codEmpresa = 0;

                            if (string.IsNullOrWhiteSpace(cabecalhoDocumento.Empresa?.CNPJ ?? ""))
                                throw new ServicoException("Não foi informado o CNPJ do transportador no EDI");

                            long? cnpjEmpresa = cabecalhoDocumento.Empresa.CNPJ.ToNullableLong();

                            if (!cnpjEmpresa.HasValue)
                                throw new ServicoException("Não foi informado o CNPJ do transportador no EDI");

                            string cnpjEmpresaFormatado = cnpjEmpresa.Value.ToString("d14");

                            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresaFormatado);
                            if (empresa != null)
                            {
                                Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);
                                cargaIntegracao.TransportadoraEmitente = serEmpresa.ConverterObjetoEmpresa(empresa);
                                codEmpresa = empresa.Codigo;
                                dadosIntegracaoEDISumarizados.Empresa = codEmpresa;
                            }
                            else
                            {
                                cargaIntegracao.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = cnpjEmpresaFormatado };
                            }

                            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlacaVarrendoFiliais(codEmpresa, cabecalhoDocumento.Veiculo.Placa); //destinatario.NotasFiscais.FirstOrDefault().Placa);

                            if (destinatario.NotasFiscais.FirstOrDefault().Recebedor != null)
                            {
                                cargaIntegracao.Recebedor = destinatario.NotasFiscais.FirstOrDefault().Recebedor.Pessoa;
                                cargaIntegracao.Distribuicoes = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Distribuicao>();
                                Dominio.ObjetosDeValor.Embarcador.Carga.Distribuicao distribuicao = new Dominio.ObjetosDeValor.Embarcador.Carga.Distribuicao();

                                if (cargaIntegracao.Recebedor.CPFCNPJ == cargaIntegracao.Destinatario.CPFCNPJ)
                                {
                                    cargaIntegracao.Recebedor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                                    //todo: aqui fazer a regra do recebedor (pendente).
                                    //cargaIntegracao.Recebedor = null;
                                }
                                else
                                {
                                    Dominio.Entidades.Empresa distribuidor = repEmpresa.BuscarPorCNPJ(cargaIntegracao.Recebedor.CPFCNPJ);

                                    if (distribuidor == null || distribuidor.Status == "I")
                                    {
                                        distribuidor = repEmpresa.BuscarPorCodigoComercialDistribuidor(cargaIntegracao.Recebedor.CPFCNPJ);

                                        if (distribuidor != null)
                                            distribuicao.TransportadorDistribuidor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = distribuidor.CNPJ_SemFormato };
                                    }
                                    else
                                        distribuicao.TransportadorDistribuidor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = cargaIntegracao.Recebedor.CPFCNPJ };

                                    if (distribuidor == null)
                                        throw new ServicoException($"Não existe nenhum distribuidor cadastrado com o CPNJ {cargaIntegracao.Recebedor.CPFCNPJ} na base Multisoftware.");

                                    cargaIntegracao.Recebedor = Servicos.Embarcador.Pessoa.Pessoa.Converter(distribuidor);
                                }

                                if (destinatario.NotasFiscais.FirstOrDefault().Recebedor.MeioTransporte == "3")
                                    distribuicao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = "BROKERS" };
                                else if (destinatario.NotasFiscais.FirstOrDefault().Recebedor.MeioTransporte == "7")
                                    distribuicao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = "TP" };


                                cargaIntegracao.Distribuicoes.Add(distribuicao);
                            }

                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                            if (destinatario.NotasFiscais.FirstOrDefault().NFe.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago)
                                modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                            else
                                modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;

                            //cargaIntegracao.TipoCargaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = "DANONE" };

                            cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = codigoTipoOperacao.ToUpper() };

                            if (codigoTipoOperacao.ToUpper() == "PRIMARIO")
                                modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;//regra fixa da Danone

                            if (veiculo != null)
                            {
                                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                                cargaIntegracao.Veiculo = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();
                                cargaIntegracao.Veiculo.Placa = veiculo.Placa;
                                if (veiculoMotorista != null)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motorista = new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista();
                                    motorista.CPF = veiculoMotorista.CPF;
                                    motorista.Nome = veiculoMotorista.Nome;
                                    motorista.NumeroCartao = veiculoMotorista.NumeroCartao;
                                    motorista.Transportador = cargaIntegracao.TransportadoraEmitente;
                                    cargaIntegracao.Motoristas.Add(motorista);
                                }
                            }

                            cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                            cargaIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                            cargaIntegracao.CTes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
                            cargaIntegracao.NumeroCarga = cabecalhoDocumento.CodigoRemessa;

                            string codigoRota = "";
                            string canalEntrega = "";
                            string canalVenda = "";

                            for (int dest = 0; dest < destinatarios.Count; dest++)
                            {
                                Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatarioNF = destinatarios[dest];

                                for (int nf = 0; nf < destinatarioNF.NotasFiscais.Count; nf++)
                                {
                                    Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal = destinatarioNF.NotasFiscais[nf];
                                    notaFiscal.NFe.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();

                                    if (notaFiscal.ComplementoNotaFiscal.CGCEntrega4 == "ROTEIRO")
                                        codigoRota = notaFiscal.ComplementoNotaFiscal.SerieEntrega4;
                                    else if (notaFiscal.ComplementoNotaFiscal.CGCEntrega5 == "ROTEIRO")
                                        codigoRota = notaFiscal.ComplementoNotaFiscal.SerieEntrega5;

                                    canalEntrega = notaFiscal.ComplementoNotaFiscal.SerieEntrega1;
                                    canalVenda = notaFiscal.ComplementoNotaFiscal.SerieVenda1;

                                    List<Dominio.ObjetosDeValor.Embarcador.NFe.ComplementoProduto> complementoProdutosDistintos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.ComplementoProduto>();
                                    for (int me = 0; me < notaFiscal.Produtos.Count; me++)
                                    {
                                        Dominio.ObjetosDeValor.Embarcador.NFe.Produtos mercadoria = notaFiscal.Produtos[me];

                                        Dominio.ObjetosDeValor.Embarcador.NFe.ComplementoProduto complementoProduto = (from obj in notaFiscal.ComplementoProdutos where obj.Codigo == notaFiscal.Produtos[me].Codigo select obj).FirstOrDefault(); //notaFiscal.ComplementoProdutos[me];

                                        if (complementoProduto == null)
                                            throw new ServicoException($"Está falantado o complemento ao produto {notaFiscal.Produtos[me].Descricao} na nota fiscal {notaFiscal.NFe.Chave}.");

                                        complementoProdutosDistintos.Add(complementoProduto);

                                        Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = CriarProduto(mercadoria, complementoProduto, unitOfWork);
                                        notaFiscal.NFe.Produtos.Add(produto);
                                        cargaIntegracao.Produtos.Add(produto);

                                        if (produto.QuantidadeCaixa > 0)
                                            notaFiscal.NFe.VolumesTotal += produto.Quantidade / produto.QuantidadeCaixa;
                                    }

                                    notaFiscal.NFe.Valor = notaFiscal.ComplementoProdutos.Sum(obj => obj.Valor);
                                    notaFiscal.NFe.PesoBruto = complementoProdutosDistintos.Sum(obj => obj.PesoBruto);
                                    notaFiscal.NFe.PesoLiquido = complementoProdutosDistintos.Sum(obj => obj.PesoLiquido);

                                    cargaIntegracao.PesoBruto += notaFiscal.NFe.PesoBruto;
                                    cargaIntegracao.PesoLiquido += notaFiscal.NFe.PesoLiquido;
                                    //cargaIntegracao.QuantidadeVolumes += (int)notaFiscal.ComplementoProdutos.Sum(obj => obj.Volumes);
                                    cargaIntegracao.QuantidadeVolumes += (int)notaFiscal.NFe.VolumesTotal;
                                    cargaIntegracao.CubagemTotal += notaFiscal.PesoCubagem;

                                    if (canalEntrega == "PRODUTOR")
                                    {

                                        if (destinatarioNF.NotasFiscais.FirstOrDefault().MeioTransporte.Trim() == "4")
                                            notaFiscal.NFe.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                                        else
                                            notaFiscal.NFe.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;

                                        cargaIntegracao.UsarOutroEnderecoOrigem = true;
                                        cargaIntegracao.Origem = notaFiscal.NFe.Destinatario.Endereco;
                                        cargaIntegracao.Origem.InscricaoEstadual = destinatario?.Pessoa?.RGIE ?? "";

                                        string[] splitEndereco = cargaIntegracao.Origem.Logradouro.Split('-');

                                        if (splitEndereco.Length == 0)
                                            throw new ServicoException("É obrigatório informar o código do produtor Rural.");

                                        cargaIntegracao.Origem.CodigoIntegracao = splitEndereco[0] + "-" + splitEndereco[1].Substring(0, 1);
                                        cargaIntegracao.Origem.Logradouro = splitEndereco[1].Remove(0, 1);

                                        string[] splitBairro = cargaIntegracao.Origem.Bairro.Split('-');
                                        if (splitBairro.Length > 1)
                                            cargaIntegracao.Origem.Bairro = "ZONA RURAL";

                                        if (!string.IsNullOrWhiteSpace(notaFiscal.NFe.DataEmissao))
                                            cargaIntegracao.DataInicioCarregamento = notaFiscal.NFe.DataEmissao + " 00:00:00";
                                    }

                                    notaFiscal.NFe.Recebedor = cargaIntegracao.Recebedor;
                                    notaFiscal.NFe.Expedidor = cargaIntegracao.Expedidor;

                                    notaFiscal.NFe.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                                    notaFiscal.NFe.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;

                                    if (!string.IsNullOrWhiteSpace(notaFiscal.NFe.Chave))
                                    {
                                        string tipoDocumento = notaFiscal.NFe.Chave.Substring(20, 2);

                                        if (tipoDocumento != "55")
                                            throw new ServicoException("A chave da nota informada não é de uma NF-e.");

                                        chavesNF.Add(notaFiscal.NFe.Chave);
                                        notaFiscal.NFe.Modelo = "55";
                                    }
                                    else
                                        notaFiscal.NFe.Modelo = "99";

                                    cargaIntegracao.NotasFiscais.Add(notaFiscal.NFe);

                                    dadosIntegracaoEDISumarizados.QuantidadeNfs++;
                                }
                            }

                            dadosIntegracaoEDISumarizados.Roteiro = codigoRota;

                            if (!string.IsNullOrWhiteSpace(codigoRota))
                            {
                                cargaIntegracao.FreteRota = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteRota() { Codigo = codigoRota };

                                if (cargaIntegracao.Distribuicoes != null && cargaIntegracao.Distribuicoes.Count > 0)
                                {
                                    Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarPorCodigoIntegracao(codigoRota);

                                    if (rotaFrete?.FinalizarViagemAutomaticamente ?? false)
                                    {
                                        cargaIntegracao.Distribuicoes.Clear();
                                        cargaIntegracao.Distribuicoes = null;
                                    }
                                    else
                                    {
                                        if (rotaFrete?.UsarDistribuidorComoTransportadorRota ?? false)
                                        {
                                            cargaIntegracao.Recebedor = null;
                                            cargaIntegracao.TransportadoraEmitente = cargaIntegracao.Distribuicoes.FirstOrDefault().TransportadorDistribuidor;
                                            //cargaIntegracao.TipoOperacao = cargaIntegracao.Distribuicoes.FirstOrDefault().TipoOperacao;
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(canalEntrega))
                                cargaIntegracao.CanalEntrega = new Dominio.ObjetosDeValor.Embarcador.Pedido.CanalEntrega() { CodigoIntegracao = canalEntrega };

                            if (!string.IsNullOrWhiteSpace(canalVenda))
                                cargaIntegracao.CanalVenda = new Dominio.ObjetosDeValor.Embarcador.Pedido.CanalVenda() { CodigoIntegracao = canalVenda };

                            cargaIntegracao.Remetente = embarcador.Pessoa;

                            if (canalEntrega != "PRODUTOR")
                            {
                                cargaIntegracao.Distancia = 0;
                                cargaIntegracao.NumeroPedidoEmbarcador = cargaIntegracao.NumeroCarga + "_" + sequenciaGlobal;
                                if (modalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar)
                                {
                                    cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                                    cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                                }
                                else
                                {
                                    cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                                    cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                                }

                                if (ValidarPorCNPJNovoPadrao)
                                {
                                    if (cargaIntegracao.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                                    {
                                        if (!CNPJsNovoPadrao.Contains(cargaIntegracao.Remetente.CPFCNPJ))
                                            throw new ServicoException("O tomador desta carga não está apto a emtir sem passar pelo GKO, favor gerar o CONEMB no GKO.");
                                    }
                                    else if (!CNPJsNovoPadrao.Contains(cargaIntegracao.Destinatario.CPFCNPJ))
                                        throw new ServicoException("O tomador desta carga não está apto a emtir sem passar pelo GKO, favor gerar o CONEMB no GKO.");
                                }

                                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasExistentes = repCarga.BuscarTodasCargasPorCodigoEmbarcador(cargaIntegracao.NumeroCarga, cargaIntegracao.Filial.CodigoIntegracao, true);

                                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste in cargasExistentes)
                                {
                                    if (cargaExiste.CargaAgrupamento != null)
                                        throw new ServicoException($"A carga {cargaIntegracao.NumeroCarga} está agrupada na carga {cargaExiste.CargaAgrupamento.CodigoCargaEmbarcador}, para adicioná-la novamente é necessário cancelar toda a carga de agrupamento.");

                                    if (!cargaExiste.CalculandoFrete || repCargaPedido.VerificarPorCargaSePendenteDadosRecebedor(cargaExiste.Codigo))
                                    {
                                        if (!serCarga.VerificarSeCargaEstaNaLogistica(cargaExiste, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) || cargaExiste.DataInicioGeracaoCTes.HasValue)
                                            throw new ServicoException($"A carga {cargaIntegracao.NumeroCarga} não pode mais ser atualizada, é necessário cancelar a mesma para atualizar o registro.");
                                        else
                                        {
                                            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repFluxoGestaoPatio.BuscarPorCargaETipo(cargaExiste.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem);
                                            if (fluxoGestaoPatio != null && fluxoGestaoPatio.EtapaAtual > 0)
                                            {
                                                throw new ServicoException($"A carga {cargaIntegracao.NumeroCarga} não pode mais ser atualizada, pois seu fluxo de pátio já foi iniciado, sendo assim é necessário cancelar a mesma para atualizar o registro.");
                                            }
                                            else if (cargaExiste.Carregamento != null && cargaExiste.Carregamento.Cargas.Count > 0)
                                            {
                                                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoCarga in cargaExiste.Carregamento.Cargas.ToList())
                                                {
                                                    throw new ServicoException($"A carga {cargaIntegracao.NumeroCarga} está em um carregamento ({carregamentoCarga.Carregamento.NumeroCarregamento}), desta forma é necessário cancelar todas as cargas deste carregamento antes de integrar a carga novamente.");
                                                }
                                            }
                                        }
                                    }
                                    else
                                        throw new ServicoException($"A carga {cargaIntegracao.NumeroCarga} não pode ser cancelada enquanto o seu frete está sendo calculado, tente novamente em instantes.");
                                }

                                if (cargasExistentes.Count > 0)
                                {
                                    try
                                    {
                                        unitOfWork.Start();

                                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste in cargasExistentes)
                                        {
                                            if (cargaExiste.CalculandoFrete)
                                                throw new ServicoException($"A carga {cargaIntegracao.NumeroCarga} não pode ser cancelada enquanto o seu frete está sendo calculado, tente novamente em instantes.");

                                            if (!serCarga.VerificarSeCargaEstaNaLogistica(cargaExiste, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
                                                throw new ServicoException($"A situação da carga {cargaIntegracao.NumeroCarga} não permite que ela seja mais atualizada, é necessário cancelar a mesma para atualizar o registro.");

                                            Servicos.Log.TratarErro("Iniciou Cancelamento" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");

                                            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                                            {
                                                Carga = cargaExiste,
                                                GerarIntegracoes = false,
                                                MotivoCancelamento = "Viagem atualizada",
                                                TipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador
                                            };

                                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, unitOfWork);
                                            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, StringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                                            if (cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.Cancelada)
                                                throw new ServicoException($"A carga {cargaIntegracao.NumeroCarga} não pode ser atualizada, é necessário cancelar manualmente para atualizar o registro. ");

                                            cargasCanceladas.Add(ValueTuple.Create(cargaExiste.Codigo, cargaExiste.CodigoCargaEmbarcador, cargaIntegracao.Filial.CodigoIntegracao));
                                            Servicos.Log.TratarErro("Finalizou Cancelamento" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                                        }

                                        unitOfWork.CommitChanges();
                                    }
                                    catch (Exception)
                                    {
                                        unitOfWork.Rollback();
                                        Log.TratarErro("Rollback Cancelamento" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                                        throw;
                                    }
                                }
                            }
                            else
                            {
                                cargaIntegracao.NumeroCarga = destinatario.NotasFiscais.FirstOrDefault().NumeroRomaneio;
                                cargaIntegracao.NumeroPedidoEmbarcador = destinatario.NotasFiscais.FirstOrDefault().NFe.Numero.ToString();
                                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa remententeProdutor = cargaIntegracao.Destinatario;
                                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatarioProdutor = cargaIntegracao.Remetente;

                                remententeProdutor.ValidarCPFPrimeiro = true;

                                remententeProdutor.CPFCNPJ = remententeProdutor.CPFCNPJ.ToLong().ToString("d14");

                                cargaIntegracao.Remetente = remententeProdutor;
                                cargaIntegracao.Destinatario = destinatarioProdutor;

                                if (destinatario.NotasFiscais.FirstOrDefault().MeioTransporte.Trim() == "4")
                                {
                                    cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                                    cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                                }
                                else if (destinatario.NotasFiscais.FirstOrDefault().MeioTransporte.Trim() == "5")
                                {
                                    cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                                    cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                                }
                                else
                                {
                                    cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                                    cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                                }

                                cargaIntegracao.UtilizarTipoTomadorInformado = true;

                                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargaExistentes = repPrecarga.BuscarPorNumeroPreCargaFilial(cargaIntegracao.NumeroCarga, cargaIntegracao.Filial.CodigoIntegracao);

                                foreach (Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga in preCargaExistentes)
                                {
                                    if (preCarga.SituacaoPreCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.AguardandoGeracaoCarga)
                                        throw new ServicoException($"A pré Carga {cargaIntegracao.NumeroCarga} não pode mais ser atualizada, é necessário cancelar a mesma para atualizar o registro.");

                                    preCarga.SituacaoPreCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada;
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in preCarga.Pedidos)
                                    {
                                        pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;
                                        repPedido.Atualizar(pedido);
                                    }
                                    repPrecarga.Atualizar(preCarga);
                                }
                            }

                            cargasIntegracoes.Add(cargaIntegracao);
                        }

                        List<int> numerosExistentes = repXMLNotaFiscal.BuscarNotasAtivasPorChave(chavesNF, ignorarReentrega: configuracaoTMS.NotaUnicaEmCargas);

                        if (numerosExistentes.Count > 0)
                        {
                            List<string> numerosCargas = repXMLNotaFiscal.BuscarCargasAtivasPorChave(chavesNF, ignorarReentrega: configuracaoTMS.NotaUnicaEmCargas);
                            throw new ServicoException($"As notas fiscais ({string.Join(", ", numerosExistentes.ToList())}) já estão vinculadas a outras cargas ({string.Join(", ", numerosCargas.ToList())})");
                        }

                    }
                }

                Servicos.Log.TratarErro("Finalizou Leitura Arquivo" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

                for (int ci = 0; ci < cargasIntegracoes.Count; ci++)
                {
                    Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = cargasIntegracoes[ci];
                    Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> protocolo = AdicionarCarga(cargaIntegracao, unitOfWork);

                    if (protocolo.Status)
                    {
                        if (cargaIntegracao.CanalEntrega == null || cargaIntegracao.CanalEntrega.CodigoIntegracao != "PRODUTOR")
                        {
                            //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);
                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedido != null ? listaCargaPedido.FirstOrDefault() : null;

                            if (!cargas.Any(obj => obj.Codigo == cargaPedido.Carga.Codigo))
                                cargas.Add(cargaPedido.Carga);
                        }
                        else
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(protocolo.Objeto.protocoloIntegracaoPedido);

                            if (pedido.PreCarga != null)
                            {
                                if (!preCargas.Any(obj => obj.Codigo == pedido.PreCarga.Codigo))
                                    preCargas.Add(pedido.PreCarga);
                            }
                        }
                    }
                    else
                    {
                        if (protocolo.CodigoMensagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao)
                            throw new ServicoException(protocolo.Mensagem);

                        //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedido != null ? listaCargaPedido.FirstOrDefault() : null;

                        if (!cargas.Any(obj => obj.Codigo == cargaPedido.Carga.Codigo))
                            cargas.Add(cargaPedido.Carga);
                    }
                }

                Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI nControleIntegracaoCargaEDI = repControleIntegracaoCargaEDI.BuscarPorCodigo(controleIntegracaoCargaEDI.Codigo);
                nControleIntegracaoCargaEDI.Cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Embarcador.Carga.RateioFrete(unitOfWork);

                for (int c = 0; c < cargas.Count; c++)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaInformacao = cargas[c];

                    unitOfWork.FlushAndClear();

                    try
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaInformacao.Codigo);

                        if (!carga.CargaFechada)
                        {
                            Log.TratarErro("Inciou Fechamento (" + carga.CodigoCargaEmbarcador + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                            decimal valorICMS = repCargaPedido.BuscarValorICMSParaComponenteCarga(carga.Codigo);
                            decimal valorISS = repCargaPedido.BuscarValorISSParaComponenteCarga(carga.Codigo);
                            decimal valorPISCONFIS = repCargaPedido.BuscarValorPISCONFINSParaComponenteCarga(carga.Codigo);

                            unitOfWork.Start();

                            if (repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ortec))
                            {
                                Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec serIntegracaoOrtec = new Embarcador.Integracao.Ortec.IntegracaoOrtec(unitOfWork);
                                serIntegracaoOrtec.VincularCargaAosAgrupamentosSemCarga(carga);
                            }

                            bool EmissaoFilialEmissora = false;
                            if (carga.Filial != null && carga.TipoOperacao != null)
                            {
                                if (carga.Filial.EmpresaEmissora != null && carga.TipoOperacao.EmiteCTeFilialEmissora)
                                {
                                    EmissaoFilialEmissora = true;
                                    if (repCargaPedido.VerificarSeOperacaoTeraEmissaoFilialEmissoraPorCarga(carga.Codigo))
                                    {
                                        Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora repositorioEstadoDestinoEmpresaEmissora = new Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora(unitOfWork);
                                        List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> estadosDestinoEmpresaEmissora = carga.Filial != null ? repositorioEstadoDestinoEmpresaEmissora.BuscarPorFilial(carga.Filial.Codigo) : new List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>();

                                        Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora estadoDestino = estadosDestinoEmpresaEmissora.Find(e => e.Estado.Codigo == carga.Pedidos.FirstOrDefault()?.Destino?.Estado.Codigo);

                                        if ((estadosDestinoEmpresaEmissora.Count > 0) && estadoDestino != null)
                                            carga.EmpresaFilialEmissora = estadoDestino.Empresa;
                                        else
                                            carga.EmpresaFilialEmissora = carga.Filial.EmpresaEmissora;

                                        if ((estadosDestinoEmpresaEmissora.Count > 0) && estadoDestino != null)
                                            carga.EmiteMDFeFilialEmissora = carga.Filial.EmiteMDFeFilialEmissoraPorEstadoDestino;
                                        else
                                            carga.EmiteMDFeFilialEmissora = carga.Filial.EmiteMDFeFilialEmissora;

                                        if (carga.Filial.UtilizarCtesAnterioresComoCteFilialEmissora)
                                            carga.UtilizarCTesAnterioresComoCTeFilialEmissora = true;
                                    }
                                }
                            }

                            serCarga.SetarTipoContratacaoCarga(carga, unitOfWork);

                            serRateioFrete.GerarComponenteICMS(carga, valorICMS, false, unitOfWork, 0m);
                            if (EmissaoFilialEmissora)
                                serRateioFrete.GerarComponenteICMS(carga, valorICMS, true, unitOfWork, 0m);

                            serRateioFrete.GerarComponenteISS(carga, valorISS, unitOfWork);
                            serRateioFrete.GerarComponentePisCofins(carga, valorPISCONFIS, unitOfWork);


                            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dynRota = ObterDadosRotas(carga, unitOfWork, configuracaoPedido);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                            Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, cargaPedidos, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                            Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                            carga.DataEnvioUltimaNFe = DateTime.Now;
                            carga.DataInicioEmissaoDocumentos = DateTime.Now;
                            carga.PossuiPendencia = false;
                            carga.MotivoPendencia = "";

                            if (carga.Filial?.EmitirMDFeManualmente ?? false)
                                carga.NaoGerarMDFe = true;

                            if (carga.Filial != null)
                                carga.TipoDeCarga = carga.Filial.TipoDeCarga;

                            carga.CalcularFreteLote = CargaPorLote;
                            carga.CargaFechada = true;
                            //carga.OcultarNoPatio = false;
                            if (CargaPorLote == Dominio.Enumeradores.LoteCalculoFrete.Integracao)
                                CargaPorLote = Dominio.Enumeradores.LoteCalculoFrete.Padrao;
                            else
                                CargaPorLote = Dominio.Enumeradores.LoteCalculoFrete.Integracao;

                            Servicos.Log.TratarErro("2 - Fechou Carga (" + carga.CodigoCargaEmbarcador + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");

                            repCarga.Atualizar(carga);

                            carga.ExigeNotaFiscalParaCalcularFrete = true;


                            if (!carga.NaoExigeVeiculoParaEmissao)
                            {
                                //alterar para retornar a mesma pre carga
                                Dominio.Entidades.Embarcador.Cargas.Carga cargaPreCarga = repCarga.BuscarPreCargaPorNumeroCargaVincularPreCarga(carga.CodigoCargaEmbarcador);

                                if (cargaPreCarga == null)
                                {
                                    int codigoCargaCancelada = (
                                        from o in cargasCanceladas
                                        where (
                                            o.CodigoCargaEmbarcador == carga.CodigoCargaEmbarcador &&
                                            (
                                                string.IsNullOrWhiteSpace(o.CodigoIntegracaoFilial) ||
                                                (carga.Filial != null && (carga.Filial.CodigoFilialEmbarcador == o.CodigoIntegracaoFilial || carga.Filial.OutrosCodigosIntegracao.Contains(o.CodigoIntegracaoFilial)))
                                            )
                                        )
                                        orderby o.CodigoCarga
                                        select o.CodigoCarga
                                    ).LastOrDefault();

                                    AdicionarCargaJanelaCarregamento(carga, codigoCargaCancelada, unitOfWork);
                                }
                                else
                                {
                                    if (cargaPreCarga.CargaAgrupamento != null)
                                    {
                                        if (servicoPreCarga.AdicionarAlteracaoDadosPreCargaAgrupada(cargaPreCarga.CargaAgrupamento, carga))
                                            repCarga.Atualizar(cargaPreCarga.CargaAgrupamento);

                                        if (cargaPreCarga.CargaAgrupamento.Empresa != null && carga.Empresa != null)
                                        {
                                            string raizCNPJEmpresaAgrup = Utilidades.String.OnlyNumbers(cargaPreCarga.CargaAgrupamento.Empresa.CNPJ).Remove(8, 6);
                                            string raizEmpresa = Utilidades.String.OnlyNumbers(carga.Empresa.CNPJ).Remove(8, 6);

                                            if (raizCNPJEmpresaAgrup != raizEmpresa)
                                                carga.Empresa = cargaPreCarga.CargaAgrupamento.Empresa;
                                        }
                                        else if (cargaPreCarga.CargaAgrupamento.Empresa != null && carga.Empresa == null)
                                            carga.Empresa = cargaPreCarga.CargaAgrupamento.Empresa;
                                    }
                                    else
                                        servicoPreCarga.AdicionarAlteracaoDadosPreCarga(cargaPreCarga, carga);

                                    carga.CargaPreCarga = cargaPreCarga;

                                    servicoPreCarga.TrocarPreCarga(cargaPreCarga, carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, configuracaoTMS, null);
                                }
                            }

                            if (carga.CargaAgrupamento == null)
                            {
                                if (carga.TipoDeCarga != null && carga.ModeloVeicularCarga != null && !carga.Filial.ExigirConfirmacaoTransporte)
                                {
                                    Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                                    List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(carga.Codigo);

                                    if (cargaMotoristas.Count > 0 && carga.Empresa != null && carga.Veiculo != null && carga.TipoOperacao != null)
                                    {
                                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete; //Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                                        carga.CalculandoFrete = true;
                                        carga.DataInicioCalculoFrete = DateTime.Now;
                                    }
                                    else
                                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                                }
                                else
                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;

                                if (carga.Rota?.FinalizarViagemAutomaticamente ?? false)
                                {
                                    carga.DataInicioCalculoFrete = null;
                                    carga.CalculandoFrete = false;
                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada;
                                    carga = serCarga.AtualizarStatusCustoExtra(carga, servicoHubCarga, repCarga);
                                    Servicos.Auditoria.Auditoria.Auditar(new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema }, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada.ObterDescricao()}", unitOfWork);
                                }
                            }

                            repCarga.Atualizar(carga);

                            nControleIntegracaoCargaEDI.Cargas.Add(carga);
                            nControleIntegracaoCargaEDI.NumeroDT += carga.CodigoCargaEmbarcador;
                            if (cargas.Count > 1 && cargas.LastOrDefault().Codigo != carga.Codigo)
                                nControleIntegracaoCargaEDI.NumeroDT += ", ";

                            unitOfWork.CommitChanges();

                            Servicos.Log.TratarErro("Finalizou Fechamento (" + carga.CodigoCargaEmbarcador + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Exceção Fechamento (Codigo " + cargaInformacao.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex);

                        if (!ex.ToString().Contains("deadlocked on lock") && !ex.ToString().Contains("current state is closed") && !ex.ToString().Contains("name is no longer available"))
                            throw new ServicoException("Não foi possível finalizar a viagem do EDI");
                        else
                        {
                            Servicos.Log.TratarErro("pegou deadlocked");

                            if (!string.IsNullOrWhiteSpace(CaminhoBatReiniciar))
                            {
                                Servicos.Log.TratarErro("Solicitou bat reinicia");
                                System.Diagnostics.Process.Start(CaminhoBatReiniciar);
                            }

                            return;
                        }
                    }
                }

                for (int p = 0; p < preCargas.Count; p++)
                {
                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPrecarga.BuscarPorCodigo(preCargas[p].Codigo);
                    preCarga.CalculandoFrete = true;
                    repPrecarga.Atualizar(preCarga);
                }

                nControleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
                nControleIntegracaoCargaEDI.NumeroTentativas++;
                nControleIntegracaoCargaEDI.MensagemRetorno = "";

                DefinirDadoscontroleIntegracaoCargaEDI(nControleIntegracaoCargaEDI, dadosIntegracaoEDISumarizados, cargas, unitOfWork);

                repControleIntegracaoCargaEDI.Atualizar(nControleIntegracaoCargaEDI);
            }
            catch (ServicoException excecao)
            {
                DefinirIntegracaoEDIRejeitada(controleIntegracaoCargaEDI.Codigo, excecao.Message, dadosIntegracaoEDISumarizados, unitOfWork);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                unitOfWork.Clear();
                DefinirIntegracaoEDIRejeitada(controleIntegracaoCargaEDI.Codigo, "O arquivo está inconsistente, por favor verifique.", dadosIntegracaoEDISumarizados, unitOfWork);
                Log.TratarErro(excecao);
            }
        }

        private void IntegrarPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (ExceptionsSeguidas > 5)
                {
                    Log.TratarErro("Solicitou bat reinicia por exceções seguidas");
                    System.Diagnostics.Process.Start(CaminhoBatReiniciar);
                }

                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> controleIntegracoesCargaEDIs;

                if (!IsProcessarSomentePrioritario(unitOfWork))
                    controleIntegracoesCargaEDIs = repositorioControleIntegracaoCargaEDI.BuscarPendenteIntegracaoSemPrioridade(0, 1);
                else
                    controleIntegracoesCargaEDIs = repositorioControleIntegracaoCargaEDI.BuscarPendenteIntegracaoPrioritario(0, 1);

                List<int> codigosControleIntegracao = (from o in controleIntegracoesCargaEDIs select o.Codigo).ToList();

                foreach (int codigoControleIntegracao in codigosControleIntegracao)
                {
                    Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = repositorioControleIntegracaoCargaEDI.BuscarPorCodigo(codigoControleIntegracao);

                    if (controleIntegracaoCargaEDI.NomeArquivo.Substring(0, 2).ToLower() == "cl")
                        IntegrarNTOFISPendentes(controleIntegracaoCargaEDI, unitOfWork);
                    else if (controleIntegracaoCargaEDI.NomeArquivo.Substring(0, 5).ToLower() == "shpnf")
                        IntegrarTransportationPlannPendentes(controleIntegracaoCargaEDI, unitOfWork);
                }

                ExceptionsSeguidas = 0;
            }
            catch (Exception excecao)
            {
                ExceptionsSeguidas++;
                unitOfWork.Rollback();
                Log.TratarErro(excecao);
            }
            finally
            {
                unitOfWork.FlushAndClear();
            }
        }

        private void IntegrarTransportationPlannPendentes(Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.LayoutEDI repositorioLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            Dominio.Entidades.LayoutEDI layoutEDI = repositorioLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.TransportationPlann, "Transportation Plann DANONE").FirstOrDefault();

            try
            {
                string caminho = caminhoRaiz + @"\Processados\";
                string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleIntegracaoCargaEDI.GuidArquivo);

                using (MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoCompleto)))
                {
                    LeituraEDI leituraEDI = new LeituraEDI(null, layoutEDI, ms, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.UTF8);
                    Dominio.ObjetosDeValor.EDI.TransportationPlann.EDITransportationPlann transportationPlann = leituraEDI.GerarTransportationPlann();
                    IEnumerable<Dominio.ObjetosDeValor.EDI.TransportationPlann.PreCarga> preCargas = transportationPlann.PreCargas?.Distinct();

                    if ((preCargas == null) || (preCargas.Count() == 0))
                        return;

                    Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                    Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                    Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                    Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                    Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                    Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                    Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                    Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                    IEnumerable<string> numerosPreCarga = (from preCarga in preCargas select preCarga.NumeroPreCarga).Distinct();
                    Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);
                    Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Embarcador.Carga.CargaMotorista(unitOfWork);

                    try
                    {
                        foreach (string numeroPreCarga in numerosPreCarga)
                        {
                            Dominio.ObjetosDeValor.EDI.TransportationPlann.PreCarga preCarga = preCargas.Where(o => o.NumeroPreCarga == numeroPreCarga).FirstOrDefault();

                            if (string.IsNullOrWhiteSpace(preCarga.NumeroPreCarga))
                                throw new ServicoException("Não foi informado o número da pré carga no EDI");

                            //Dominio.Entidades.Embarcador.PreCargas.PreCarga preCargaDuplicada = repositorioPreCarga.BuscarPorNumeroPreCarga(preCarga.NumeroPreCarga);

                            //if (preCargaDuplicada != null)
                            //throw new ServicoException($"Já existe uma pré carga com o número {preCarga.NumeroPreCarga}");

                            if (string.IsNullOrWhiteSpace(preCarga.Filial))
                                throw new ServicoException($"Não foi informada a filial para a pré carga {preCarga.NumeroPreCarga} no EDI");

                            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.buscarPorCodigoEmbarcador(preCarga.Filial);

                            if (filial == null)
                                throw new ServicoException($"A filial informada para a pré carga {preCarga.NumeroPreCarga} não existe na base multisoftware");

                            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();
                            cargaIntegracao.Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = preCarga.Filial };
                            cargaIntegracao.NumeroCarga = preCarga.NumeroPreCarga;
                            cargaIntegracao.NumeroPedidoEmbarcador = preCarga.NumeroPreCarga;

                            List<Dominio.Entidades.Veiculo> veiculos = repositorioVeiculo.BuscarListaPorPlaca(preCarga.PlacaVeiculo);
                            Dominio.Entidades.Veiculo veiculo = null;
                            Dominio.Entidades.Usuario veiculoMotorista = null;

                            if (veiculos.Count == 1)
                            {
                                veiculo = veiculos.FirstOrDefault();
                                cargaIntegracao.Veiculo = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo() { Placa = veiculo.Placa };
                                cargaIntegracao.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = veiculo.Empresa.CNPJ };

                                veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                                if (veiculoMotorista != null)
                                {
                                    cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
                                    cargaIntegracao.Motoristas.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista() { CPF = veiculoMotorista.CPF, NumeroCartao = veiculoMotorista.NumeroCartao });
                                }

                                if (veiculo.ModeloVeicularCarga != null)
                                    cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular() { CodigoIntegracao = veiculo.ModeloVeicularCarga.CodigoIntegracao };
                            }
                            else
                            {
                                cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular() { CodigoIntegracao = preCarga.PlacaVeiculo };
                            }

                            cargaIntegracao.Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = filial.CNPJ };
                            cargaIntegracao.FreteRota = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteRota() { Codigo = preCarga.Rota };

                            if (filial != null && filial.TipoDeCarga != null)
                                cargaIntegracao.TipoCargaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = filial.TipoDeCarga.CodigoTipoCargaEmbarcador };

                            cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = "PreCarga" };

                            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasExistentes = repCarga.BuscarTodasCargasPorCodigoEmbarcador(cargaIntegracao.NumeroCarga, cargaIntegracao.Filial.CodigoIntegracao, false);

                            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

                            if (cargasExistentes.Any(obj => obj.CargaDePreCarga == false))
                                throw new ServicoException($"A carga {cargaIntegracao.NumeroCarga} já teve suas notas fiscais integradas não sendo possível adicioná-la novamente como pré carga.");

                            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste in cargasExistentes)
                            {
                                if (cargaExiste.CargaAgrupamento != null)
                                    throw new ServicoException($"A carga {cargaIntegracao.NumeroCarga} está agrupada na carga {cargaExiste.CargaAgrupamento.CodigoCargaEmbarcador}, para adicioná-la novamente é necessário cancelar toda a carga de agrupamento.");

                                carga = cargaExiste;
                                break;
                            }

                            cargaIntegracao.ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto() { DescricaoProduto = "Diversos", CodigoProduto = "Diversos" };
                            Dominio.Entidades.RotaFrete rotaFrete = null;

                            if (!string.IsNullOrWhiteSpace(cargaIntegracao.FreteRota.Codigo))
                                rotaFrete = repositorioRotaFrete.BuscarAtivaPorCodigoIntegracao(cargaIntegracao.FreteRota.Codigo);
                            else
                                throw new ServicoException($"É obrigatório informar a rota para integrar a pré carga.");


                            if (rotaFrete == null)
                                throw new ServicoException($"Não existe uma rota cadastrada na base multisoftware para o código {cargaIntegracao.FreteRota.Codigo}.");

                            if (rotaFrete?.TipoOperacaoPreCarga != null)
                                cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = rotaFrete.TipoOperacaoPreCarga.CodigoIntegracao };

                            if (!rotaFrete.HoraInicioCarregamento.HasValue)
                                throw new ServicoException($"Não foi configurado o horario de inicio de carregamento para a rota {cargaIntegracao.FreteRota.Codigo}, por favor verifique e tente novamente.");

                            if (carga == null)
                            {
                                cargaIntegracao.CargaDePreCarga = true;
                                Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> protocolo = AdicionarCarga(cargaIntegracao, unitOfWork);

                                if (protocolo.Status == true)
                                    carga = repCarga.BuscarPorCodigo(protocolo.Objeto.protocoloIntegracaoCarga);
                                else
                                    throw new ServicoException(protocolo.Mensagem);
                            }
                            else
                            {
                                carga.Rota = rotaFrete;

                                new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).ValidaAtualizaZonaExclusaoRota(rotaFrete);

                                if (veiculo != null)
                                {
                                    carga.Veiculo = veiculo;
                                    carga.Empresa = veiculo.Empresa;
                                    carga.ModeloVeicularCarga = veiculo.ModeloVeicularCarga;
                                    carga.VeiculosVinculados = veiculo.VeiculosVinculados.ToList();

                                    if (veiculoMotorista != null)
                                        servicoCargaMotorista.AtualizarMotorista(carga, veiculoMotorista);
                                }
                                else
                                {
                                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.buscarPorCodigoIntegracao(preCarga.PlacaVeiculo);
                                    if (modeloVeicularCarga != null)
                                        carga.ModeloVeicularCarga = modeloVeicularCarga;
                                }
                            }

                            unitOfWork.Start();

                            serCarga.FecharCarga(carga, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null);
                            carga.CargaFechada = true;

                            repCarga.Atualizar(carga);

                            if (repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ortec))
                            {
                                Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec serIntegracaoOrtec = new Embarcador.Integracao.Ortec.IntegracaoOrtec(unitOfWork);
                                serIntegracaoOrtec.VincularCargaAosAgrupamentosSemCarga(carga);
                            }

                            //Embarcador.GestaoPatio.GestaoPatio.AjustarFluxoGestaoPatio(carga, null, carga.JanelaCarregamento, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork);

                            unitOfWork.CommitChanges();
                        }

                        controleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
                        controleIntegracaoCargaEDI.NumeroTentativas++;
                        controleIntegracaoCargaEDI.MensagemRetorno = "";

                        repositorioControleIntegracaoCargaEDI.Atualizar(controleIntegracaoCargaEDI);
                    }
                    catch (ServicoException excecao)
                    {
                        unitOfWork.Rollback();
                        DefinirIntegracaoEDIRejeitada(controleIntegracaoCargaEDI.Codigo, excecao.Message, null, unitOfWork);
                    }
                    catch (Exception excecao)
                    {
                        unitOfWork.Rollback();
                        unitOfWork.Clear();
                        DefinirIntegracaoEDIRejeitada(controleIntegracaoCargaEDI.Codigo, "O arquivo está inconsistente, por favor verifique.", null, unitOfWork);
                        Log.TratarErro(excecao);
                    }
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (!ex.ToString().Contains("deadlocked on lock") && !ex.ToString().Contains("current state is closed") && !ex.ToString().Contains("name is no longer available"))
                {
                    DefinirIntegracaoEDIRejeitada(controleIntegracaoCargaEDI.Codigo, "O arquivo está inconsistente, por favor verifique.", null, unitOfWork);
                    Log.TratarErro(ex);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(CaminhoBatReiniciar))
                        System.Diagnostics.Process.Start(CaminhoBatReiniciar);
                    return;
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga ObterDadosRotas(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dadosRota = null;

            if (repCargaPercurso.ContarPorCarga(carga.Codigo) <= 0)
            {
                dadosRota = serCargaRota.CriarRota(carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork, configuracaoPedido);
            }
            else
            {
                dadosRota = new Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga();
                dadosRota.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoRotas.Valida;
            }

            return dadosRota;
        }

        private string ValidarEndereco(Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";

            using (AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoSessaoBancoDados.Nova))
            {
                AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                if (endereco.Cidade.IBGE == 0 || string.IsNullOrWhiteSpace(endereco.Bairro))
                {
                    AdminMultisoftware.Dominio.Entidades.Localidades.Endereco enderecoCEP = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(endereco.CEP)).ToString());
                    if (enderecoCEP != null)
                    {
                        if (endereco.Cidade.IBGE == 0)
                            endereco.Cidade.IBGE = int.Parse(enderecoCEP.Localidade.CodigoIBGE);

                        if (endereco.Cidade.IBGE == 0)
                        {
                            if (endereco.Cidade.IBGE == 0)
                            {
                                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(endereco.Cidade.Descricao), endereco.Cidade.SiglaUF);
                                if (localidade != null)
                                    endereco.Cidade.IBGE = localidade.CodigoIBGE;
                                else
                                    retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.NaoExisteCidadeCadastrada, endereco.Cidade.Descricao, endereco.Cidade.SiglaUF);
                            }
                        }

                        if (string.IsNullOrWhiteSpace(endereco.Bairro))
                            endereco.Bairro = enderecoCEP.Bairro?.Descricao;
                    }
                    else
                    {
                        if (endereco.Cidade.IBGE == 0)
                        {
                            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(endereco.Cidade.Descricao), endereco.Cidade.SiglaUF);
                            if (localidade != null)
                                endereco.Cidade.IBGE = localidade.CodigoIBGE;
                            else
                                retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.NaoExisteCidadeCadastrada, endereco.Cidade.Descricao, endereco.Cidade.SiglaUF);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(endereco.Bairro))
                        endereco.Bairro = "INDEFINIDO";

                    if (endereco.Bairro.Length < 3)
                        endereco.Bairro = "Bairro " + endereco.Bairro;
                }

                if (string.IsNullOrWhiteSpace(endereco.Telefone) || endereco.Telefone.Length < 7)
                    endereco.Telefone = "";
            }

            return retorno;
        }

        #endregion
    }

    public class Retorno<T>
    {
        public T Objeto { get; set; }

        public bool Status { get; set; }

        public string DataRetorno { get; set; }

        public string Mensagem { get; set; }

        public int CodigoMensagem { get; set; }
    }

    public class CargaCompartilhada
    {
        public string CodigoCompartilhado { get; set; }

        public string Placa { get; set; }

        public string Rota { get; set; }

        public string DataEmissao { get; set; }

        public List<string> CodigosCargas { get; set; }
    }
}
