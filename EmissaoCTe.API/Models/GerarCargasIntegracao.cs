using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class GerarCargasIntegracao
    {
        private int Tempo = 1000;

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaGerarCargasIntegracao;
        private static GerarCargasIntegracao Instance;

        public static GerarCargasIntegracao GetInstance()
        {
            if (Instance == null)
                Instance = new GerarCargasIntegracao();

            return Instance;
        }


        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaGerarCargasIntegracao == null)
                ListaGerarCargasIntegracao = new ConcurrentQueue<int>();

            if (!ListaTasks.ContainsKey(idEmpresa))
            {
                this.IniciarThread(idEmpresa, stringConexao);
            }
        }

        private void IniciarThread(int idEmpresa, string stringConexao)
        {
            var filaConsulta = new ConcurrentQueue<int>();

            filaConsulta.Enqueue(idEmpresa);

            Task task = new Task(() =>
            {
#if DEBUG
                System.Threading.Thread.Sleep(6666);
                Tempo = 5000;
#endif
                if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["IntervaloGeracaoCargaMultiCTe"]))
                {
                    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["IntervaloGeracaoCargaMultiCTe"], out Tempo);
                    Tempo = Tempo * 60000;
                }

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            VerificarCargaIntegracao(stringConexao, unidadeDeTrabalho);
                            VerificarCargasPendenteCancelamento(stringConexao, unidadeDeTrabalho);
                            VerificarMDFesPendentesCarga(stringConexao, unidadeDeTrabalho);
                            VerificarCargasPendentesComCTe(stringConexao, unidadeDeTrabalho);

                            unidadeDeTrabalho.Dispose();
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(Tempo);

                        if (!filaConsulta.TryDequeue(out idEmpresa))
                        {
                            Servicos.Log.TratarErro("Task parou a execução", "GerarCarga");
                            break;
                        }

                    }
                    catch (TaskCanceledException abort)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de geração de carga de MDFes Integrados cancelada: ", abort.ToString()), "GerarCarga");
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de geração de carga de MDFes Integrados cancelada: ", abortThread), "GerarCarga");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "GerarCarga");
                        System.Threading.Thread.Sleep(Tempo / 2);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task de geração de carga de MDFes Integrados à fila.", "GerarCarga");
        }


        private void VerificarMDFesPendentesCarga(string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unitOfWork);
            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);

            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

            int quantidadePendentes = repIntegracaoMDFe.ContarPendentesIntegracao();
            Servicos.Log.TratarErro("MDFes pendentes de geração de carga: " + quantidadePendentes, "GerarCargaMDFes");
            if (quantidadePendentes > 0)
            {
                List<int> listaIntegracaoMDFe = repIntegracaoMDFe.BuscarPendentesIntegracao(50);
                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

                for (var i = 0; i < listaIntegracaoMDFe.Count; i++)
                {
                    Dominio.Entidades.IntegracaoMDFe integracaoMDFe = repIntegracaoMDFe.BuscarPorCodigo(listaIntegracaoMDFe[i]);

                    if (integracaoMDFe != null)
                    {
                        try
                        {
                            List<int> listaIntegracaoCTe = repDocumentoMDFe.BuscarCodigosDeCTesPorMDFe(integracaoMDFe.MDFe.Codigo);

                            for (var j = 0; j < listaIntegracaoCTe.Count; j++)
                            {
                                Dominio.Entidades.IntegracaoCTe integracaoCTe = repIntegracaoCTe.BuscarPorCTe(listaIntegracaoCTe[j]);
                                if (integracaoCTe != null)
                                {
                                    try
                                    {
                                        string tipoVeiculo = "";
                                        try
                                        {
                                            if (integracaoCTe.CTe.ObservacoesGerais != null && integracaoCTe.CTe.ObservacoesGerais.Contains("TIPO VEICULO:"))
                                            {
                                                int posicao = integracaoCTe.CTe.ObservacoesGerais.IndexOf("TIPO VEICULO:");
                                                int posicaoFim = posicao + 13 + 8;
                                                if (posicao > -1 && posicaoFim > -1)
                                                {
                                                    int inicio = posicao + 13;
                                                    int tamanho = posicaoFim - (posicao + 13);
                                                    tipoVeiculo = integracaoCTe.CTe.ObservacoesGerais.Substring(inicio, tamanho).Replace(" ", "");
                                                }
                                            }
                                            else if (integracaoCTe.CTe.ObservacoesGerais != null && integracaoCTe.CTe.ObservacoesGerais.Contains("TIPO: VEICULO"))
                                            {
                                                int posicao = integracaoCTe.CTe.ObservacoesGerais.IndexOf("TIPO: VEICULO");
                                                int posicaoFim = posicao + 13 + 8;
                                                if (posicao > -1 && posicaoFim > -1)
                                                    tipoVeiculo = integracaoCTe.CTe.ObservacoesGerais.Substring(posicao + 13, posicaoFim - (posicao + 13)).Replace(" ", "");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Servicos.Log.TratarErro("Problema ao gerar definir tipo de veiculo: " + ex.Message, "GerarCarga");
                                        }

                                        Servicos.Log.TratarErro("Gerando carga CTe: " + integracaoCTe.CTe.Codigo, "GerarCarga");
                                        int codigoCarga = servicoCTe.GerarCargaCTe(integracaoCTe.CTe.Codigo, integracaoCTe.NumeroDaUnidade.ToString(), integracaoCTe.NumeroDaCarga.ToString(), tipoVeiculo, "Todas", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte, stringConexao, unitOfWork);
                                        if (codigoCarga > 0)
                                        {
                                            integracaoCTe.GerouCargaEmbarcador = true;
                                            repIntegracaoCTe.Atualizar(integracaoCTe);

                                            integracaoMDFe.GerouCargaEmbarcador = true;
                                            repIntegracaoMDFe.Atualizar(integracaoMDFe);
                                        }
                                        else
                                        {
                                            Servicos.Log.TratarErro("Problema ao gerar carga CTe codigo : " + integracaoCTe.CTe.Codigo, "GerarCargaMDFes");

                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Servicos.Log.TratarErro("Problema ao gerar carga CTe codigo " + integracaoCTe.CTe.Codigo + ": " + ex.Message, "GerarCargaMDFes");

                                        throw;
                                    }
                                }
                                unitOfWork.FlushAndClear();
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Problema ao gerar carga MDFe codigo " + integracaoMDFe.MDFe.Codigo + ": " + ex.Message, "GerarCargaMDFes");

                            throw;
                        }
                    }
                }
            }
        }

        private void VerificarCargaIntegracao(string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.IntegracaoCarga repIntegracaoCarga = new Repositorio.IntegracaoCarga(unitOfWork);
            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);
            Repositorio.IntegracaoNFSe repIntegracaoNFSe = new Repositorio.IntegracaoNFSe(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            int quantidadePendentes = repIntegracaoCarga.ContarPendentesIntegracao();
            Servicos.Log.TratarErro("Cargas pendentes de geração: " + quantidadePendentes, "GerarCarga");

            if (quantidadePendentes > 0)
            {
                List<Dominio.Entidades.IntegracaoCarga> listaIntegracaoCargas = repIntegracaoCarga.BuscarPendentesIntegracao(5);

                Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

                for (var i = 0; i < listaIntegracaoCargas.Count; i++)
                {
                    Dominio.Entidades.IntegracaoCarga integracaoCarga = repIntegracaoCarga.BuscarPorCodigo(listaIntegracaoCargas[i].Codigo);

                    int.TryParse(integracaoCarga.NumeroDaCarga, out int numeroCarga);
                    int.TryParse(integracaoCarga.NumeroDaUnidade, out int numeroDaUnidade);
                    List<int> listaIntegracaoCTe = repIntegracaoCTe.BuscarPorCargaPendentes(numeroCarga, numeroDaUnidade, 0);
                    Servicos.Log.TratarErro("CTes da carga pendentes de geração: " + listaIntegracaoCTe.Count(), "GerarCarga");

                    if (listaIntegracaoCTe == null || listaIntegracaoCTe.Count() == 0)
                    {
                        List<int> listaIntegracaoNFSe = repIntegracaoNFSe.BuscarPorCargasPendentes(numeroCarga, numeroDaUnidade);
                        if (listaIntegracaoNFSe == null || listaIntegracaoNFSe.Count() == 0)
                        {
                            Servicos.Log.TratarErro("Carga: " + integracaoCarga.NumeroDaCarga + "/" + integracaoCarga.NumeroDaUnidade + " sem CTes", "GerarCarga");
                            integracaoCarga.Status = Dominio.Enumeradores.StatusIntegracaoCarga.Erro;
                            repIntegracaoCarga.Atualizar(integracaoCarga);
                        }
                    }

                    int codigoCarga = 0;
                    Dominio.Entidades.Empresa empresa = null;

                    if (listaIntegracaoCTe.Count() > 0)
                    {
                        for (var j = 0; j < listaIntegracaoCTe.Count; j++)
                        {
                            Dominio.Entidades.IntegracaoCTe integracaoCTe = repIntegracaoCTe.BuscarPorCodigo(listaIntegracaoCTe[j]);
                            empresa = integracaoCTe.CTe.Empresa;
                            if (integracaoCTe != null && (integracaoCTe.CTe.Status == "A" || (integracaoCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && integracaoCTe.CTe.Status == "N")))
                            {
                                try
                                {
                                    string tipoVeiculo = "";
                                    try
                                    {
                                        if (integracaoCTe.CTe.ObservacoesGerais != null && integracaoCTe.CTe.ObservacoesGerais.Contains("TIPO VEICULO:"))
                                        {
                                            int posicao = integracaoCTe.CTe.ObservacoesGerais.IndexOf("TIPO VEICULO:");
                                            int posicaoFim = posicao + 13 + 8;
                                            if (posicao > -1 && posicaoFim > -1)
                                            {
                                                int inicio = posicao + 13;
                                                int tamanho = posicaoFim - (posicao + 13);
                                                tipoVeiculo = integracaoCTe.CTe.ObservacoesGerais.Substring(inicio, tamanho).Replace(" ", "");
                                            }
                                        }
                                        else if (integracaoCTe.CTe.ObservacoesGerais != null && integracaoCTe.CTe.ObservacoesGerais.Contains("TIPO: VEICULO"))
                                        {
                                            int posicao = integracaoCTe.CTe.ObservacoesGerais.IndexOf("TIPO: VEICULO");
                                            int posicaoFim = posicao + 13 + 8;
                                            if (posicao > -1 && posicaoFim > -1)
                                                tipoVeiculo = integracaoCTe.CTe.ObservacoesGerais.Substring(posicao + 13, posicaoFim - (posicao + 13)).Replace(" ", "");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Servicos.Log.TratarErro("Problema ao gerar definir tipo de veiculo: " + ex.Message, "GerarCarga");
                                    }

                                    try
                                    {
                                        if (integracaoCTe.CTe.ObservacoesContribuinte != null)// && integracaoCTe.CTe.ObservacoesContribuinte.Any(o => o.Identificador == "ValorSub"))
                                        {
                                            Repositorio.SubcontratacaoDocumentos repSubcontratacaoDocumentos = new Repositorio.SubcontratacaoDocumentos(unitOfWork); ;

                                            string cnpjTranspSub = (from o in integracaoCTe.CTe.ObservacoesContribuinte where o.Identificador == "CNPJdotransportador" select o.Descricao).FirstOrDefault()?.ToString();

                                            if (!string.IsNullOrWhiteSpace(cnpjTranspSub) && repSubcontratacaoDocumentos.ContarPorCTe(integracaoCTe.CTe.Codigo) <= 0)
                                            {
                                                string observacaoSubcontratacao = string.Empty;
                                                if (integracaoCTe.CTe.ObservacoesGerais != null && integracaoCTe.CTe.ObservacoesGerais.Contains("ProtocoloSAP:"))
                                                {
                                                    int posicao = integracaoCTe.CTe.ObservacoesGerais.IndexOf("ProtocoloSAP:");
                                                    int posicaoFim = posicao + 13 + 20;
                                                    if (posicao > -1 && posicaoFim > -1)
                                                    {
                                                        int inicio = posicao;
                                                        int tamanho = posicaoFim - posicao;
                                                        observacaoSubcontratacao = integracaoCTe.CTe.ObservacoesGerais.Substring(inicio, tamanho);
                                                    }
                                                }

                                                string observacaoContainerNavio = VerificarObservacaoIntegracaoContainerNavio(integracaoCTe.Arquivo);
                                                if (!string.IsNullOrWhiteSpace(observacaoContainerNavio))
                                                    observacaoSubcontratacao = string.Concat(observacaoSubcontratacao, " ", observacaoContainerNavio);

                                                Servicos.Subcontratacao svcSubcontratacao = new Servicos.Subcontratacao(unitOfWork);
                                                Dominio.ObjetosDeValor.Subcontratacao subcontratacaoImportada = new Dominio.ObjetosDeValor.Subcontratacao();
                                                subcontratacaoImportada.Documentos = new List<Dominio.ObjetosDeValor.SubcontratacaoDocumentos>();

                                                subcontratacaoImportada.TipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;
                                                subcontratacaoImportada.CNPJSubcontratado = cnpjTranspSub;// (from o in observacoesContribuinte where o.Campo == "CNPJdoTransportador" select o.Texto).ToString();
                                                subcontratacaoImportada.ValorFrete = (from o in integracaoCTe.CTe.ObservacoesContribuinte where o.Identificador == "ValorSub" select o.Descricao).FirstOrDefault()?.ToString();
                                                subcontratacaoImportada.codigoProcessoTransporte = integracaoCTe.NumeroDaCarga.ToString();
                                                subcontratacaoImportada.CNPJCTe = integracaoCTe.CTe.Empresa.CNPJ;
                                                subcontratacaoImportada.observacaoSubcontratacao = observacaoSubcontratacao;

                                                Dominio.ObjetosDeValor.SubcontratacaoDocumentos doc = new Dominio.ObjetosDeValor.SubcontratacaoDocumentos();
                                                doc.Numero = integracaoCTe.CTe.Numero.ToString();
                                                doc.Serie = integracaoCTe.CTe.Serie.Numero.ToString();
                                                subcontratacaoImportada.Documentos.Add(doc);

                                                Dominio.Entidades.Subcontratacao subcontratacao = svcSubcontratacao.SalvarSubcontratacao(subcontratacaoImportada, unitOfWork);
                                                if (subcontratacao == null)
                                                    Servicos.Log.TratarErro("Subcontratação do CTe " + integracaoCTe.CTe.Codigo.ToString() + " não foi gerada", "SubContratacaoCTe");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Servicos.Log.TratarErro("Problema ao gerar registro para subcontratação do CTe codigo " + integracaoCTe.CTe.Codigo.ToString() + ": " + ex.Message, "SubContratacaoCTe");
                                    }

                                    Servicos.Log.TratarErro("Gerando carga CTe: " + integracaoCTe.CTe.Codigo, "GerarCarga");
                                    codigoCarga = servicoCTe.GerarCargaCTe(integracaoCTe.CTe.Codigo, numeroDaUnidade.ToString(), numeroCarga.ToString(), tipoVeiculo, "Todas", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos, stringConexao, unitOfWork);
                                    if (codigoCarga > 0)
                                    {
                                        integracaoCTe.GerouCargaEmbarcador = true;
                                        repIntegracaoCTe.Atualizar(integracaoCTe);
                                        Servicos.Log.TratarErro("CTe: " + integracaoCTe.CTe.Codigo + " adicionado com sucesso na carga", "GerarCarga");
                                    }
                                    else
                                    {
                                        Servicos.Log.TratarErro("Problema ao gerar carga CTe codigo : " + integracaoCTe.CTe.Codigo, "GerarCarga");

                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("Problema ao gerar carga CTe codigo " + integracaoCTe.CTe.Codigo + ": " + ex.Message, "GerarCarga");

                                    throw;
                                }
                            }
                            unitOfWork.FlushAndClear();
                        }
                    }
                    else
                    { //Verificar se CTes já estãp com carga Gerada
                        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> integracoesCTes = repIntegracaoCTe.BuscarCTesPorCarga(0, integracaoCarga.NumeroDaUnidade.ToInt(), integracaoCarga.NumeroDaCarga.ToInt(), "");
                        if (integracoesCTes.Count() > 0)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(integracoesCTes[0].Codigo);
                            if (cargaCTe != null)
                                codigoCarga = cargaCTe.Carga.Codigo;
                        }
                    }

                    if (codigoCarga > 0)
                    {
                        List<int> listaCTePendentes = repIntegracaoCTe.BuscarPorCargaPendentes(numeroCarga, numeroDaUnidade, 0);
                        List<int> listaNFSePendentes = repIntegracaoNFSe.BuscarPorCargasPendentes(numeroCarga, numeroDaUnidade);

                        if (listaCTePendentes.Count == 0 && listaNFSePendentes.Count == 0)
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                            if (carga != null)
                            {
                                try
                                {
                                    unitOfWork.Start();

                                    integracaoCarga.Status = Dominio.Enumeradores.StatusIntegracaoCarga.Gerado;
                                    integracaoCarga.CodigoCarga = codigoCarga;
                                    repIntegracaoCarga.Atualizar(integracaoCarga);

                                    servicoCTe.AtualizarValoresCarga(carga, unitOfWork);

                                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                                    serCarga.FecharCarga(carga, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null);

                                    Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
                                    serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacaoPorCTe(carga, repCargaPedido.BuscarPorCarga(carga.Codigo), repCargaCTe.BuscarPorCarga(carga.Codigo), unitOfWork, configuracaoPedido);

                                    Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro svcFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
                                    svcFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, carga.TipoFreteEscolhido, unitOfWork, false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, "");
                                    string retornoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterCIOTCarga(carga, configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork);

                                    if (!string.IsNullOrWhiteSpace(retornoCIOT))
                                        Servicos.Log.TratarErro("CIOT da carga " + integracaoCarga.NumeroDaCarga + "/" + integracaoCarga.NumeroDaUnidade + " não gerado: " + retornoCIOT, "GerarCarga");

                                    Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos;
                                    if (empresa != null)
                                        carga.Empresa = empresa;

                                    if (carga.FreteDeTerceiro && carga.Terceiro == null && carga.Veiculo != null && carga.Veiculo.Proprietario != null)
                                        carga.Terceiro = carga.Veiculo.Proprietario;
                                    else if (carga.ProvedorOS != null)
                                        carga.Terceiro = carga.ProvedorOS;

                                    carga.NumeroImpressora = integracaoCarga.NumeroDaUnidade;
                                    carga.CargaFechada = true;
                                    Servicos.Log.TratarErro("17 - Fechou Carga (" + carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");

                                    carga.DataInicioCalculoFrete = null;
                                    carga.CalculandoFrete = false;
                                    carga.CalcularFreteSemEstornarComplemento = false;
                                    carga.EmitindoCTes = false;
                                    carga.PossuiPendencia = false;
                                    carga.AgImportacaoCTe = false;
                                    repCarga.Atualizar(carga);

                                    unitOfWork.CommitChanges();

                                    Servicos.Log.TratarErro("Carga: " + integracaoCarga.NumeroDaCarga + "/" + integracaoCarga.NumeroDaUnidade + " gerada com sucesso.", "GerarCarga");
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("Falha Carga: " + integracaoCarga.NumeroDaCarga + "/" + integracaoCarga.NumeroDaUnidade + ": " + ex, "GerarCarga");
                                    if (unitOfWork != null)
                                        unitOfWork.Rollback();
                                    Tempo = Tempo / 2;
                                }
                            }
                        }
                    }
                }

                if (quantidadePendentes > listaIntegracaoCargas.Count())
                    Tempo = Tempo / 2;

            }

        }

        private void VerificarCargasPendenteCancelamento(string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.IntegracaoCarga repIntegracaoCarga = new Repositorio.IntegracaoCarga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            int quantidadePendentes = repIntegracaoCarga.ContarPendentesCancelamento();
            Servicos.Log.TratarErro("Cargas pendentes de cancelamento: " + quantidadePendentes, "GerarCarga");

            if (quantidadePendentes > 0)
            {
                List<Dominio.Entidades.IntegracaoCarga> listaIntegracaoCargas = repIntegracaoCarga.BuscarPendentesCancelamento(5);
                for (var i = 0; i < listaIntegracaoCargas.Count; i++)
                {
                    Dominio.Entidades.IntegracaoCarga integracaoCarga = repIntegracaoCarga.BuscarPorCodigo(listaIntegracaoCargas[i].Codigo);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(listaIntegracaoCargas[i].CodigoCarga);
                    if (carga != null)
                    {
                        unitOfWork.Start();

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                        {
                            Servicos.Log.TratarErro("Carga " + integracaoCarga.CodigoCarga + " já cancelada.", "GerarCarga");
                            integracaoCarga.Status = Dominio.Enumeradores.StatusIntegracaoCarga.SolicitadoCancelamento;
                            repIntegracaoCarga.Atualizar(integracaoCarga);
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                            {
                                Carga = carga,
                                DefinirSituacaoEmCancelamento = true,
                                MotivoCancelamento = "Cancelamento solicitado por integracao",
                                TipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador,
                                UsuarioERPSolicitouCancelamento = "Integracao"
                            };

                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, unitOfWork);
                            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, unitOfWork.StringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                            Servicos.Log.TratarErro("Carga " + integracaoCarga.CodigoCarga + " solicitado cancelamento com sucesso.", "GerarCarga");
                            integracaoCarga.Status = Dominio.Enumeradores.StatusIntegracaoCarga.SolicitadoCancelamento;
                            repIntegracaoCarga.Atualizar(integracaoCarga);
                        }

                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Carga " + integracaoCarga.CodigoCarga + " não localizada para solicitar cancelamento.", "GerarCarga");
                        integracaoCarga.Status = Dominio.Enumeradores.StatusIntegracaoCarga.CancelamentoNaoEfetuado;
                        repIntegracaoCarga.Atualizar(integracaoCarga);
                    }

                    unitOfWork.FlushAndClear();
                }

                if (quantidadePendentes > listaIntegracaoCargas.Count())
                    Tempo = Tempo / 2;

            }

        }

        private void VerificarCargasPendentesComCTe(string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["VerificarCargasPendentesComMDFe"] == "SIM")
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<int> listaCargas = repCarga.BuscarCargasPendentesComCTeGerado(10);

                foreach (int codigoCarga in listaCargas)
                {
                    var carga = repCarga.BuscarPorCodigo(codigoCarga);

                    if (carga != null)
                    {
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos;
                        repCarga.Atualizar(carga);
                    }
                }
            }

        }

        private string VerificarObservacaoIntegracaoContainerNavio(string arquivoIntegracao)
        {
            if (string.IsNullOrWhiteSpace(arquivoIntegracao))
                return string.Empty;

            try
            {

                Dominio.ObjetosDeValor.CTe.CTeNFSe documentoCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.CTe.CTeNFSe>(arquivoIntegracao);

                string observacao = string.Empty;

                if (!string.IsNullOrWhiteSpace(documentoCTe.NumeroLacre1))
                    observacao = "Lacre: " + documentoCTe.NumeroLacre1 + "; ";

                if (!string.IsNullOrWhiteSpace(documentoCTe.NumeroContainer))
                    observacao = string.Concat(observacao, "Container: " + documentoCTe.NumeroContainer + "; ");

                if (!string.IsNullOrWhiteSpace(documentoCTe.NomeNavio))
                    observacao = string.Concat(observacao, "Navio: " + documentoCTe.NomeNavio + "; ");

                if (!string.IsNullOrWhiteSpace(documentoCTe.PortoOrigem))
                    observacao = string.Concat(observacao, "Porto Origem: " + documentoCTe.PortoOrigem + "; ");

                if (!string.IsNullOrWhiteSpace(documentoCTe.PortoDestino))
                    observacao = string.Concat(observacao, "Porto Destino: " + documentoCTe.PortoDestino + "; ");

                return observacao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problema ao gerar obter observação container/navio: " + ex.Message, "GerarCargasIntegracao");
                return string.Empty;
            }

        }

    }
}