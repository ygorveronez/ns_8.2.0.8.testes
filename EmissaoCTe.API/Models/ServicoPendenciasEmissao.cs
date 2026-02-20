using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class ServicoPendenciasEmissao
    {
        private int Tempo = 150000; //2,5 MINUTOS

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaConsultaEmailCaixaEntrada;
        private static ServicoPendenciasEmissao Instance;

        public static ServicoPendenciasEmissao GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoPendenciasEmissao();

            return Instance;
        }

        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaConsultaEmailCaixaEntrada == null)
                ListaConsultaEmailCaixaEntrada = new ConcurrentQueue<int>();

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
                Tempo = 150000;
#endif

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            VerificarCTesPendentes(unidadeDeTrabalho, stringConexao);
                            unidadeDeTrabalho.Dispose();
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(Tempo);

                        if (!filaConsulta.TryDequeue(out idEmpresa))
                        {
                            Servicos.Log.TratarErro("Task parou a execução");
                            break;
                        }

                    }
                    catch (TaskCanceledException abort)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de pendencias de emissão cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de geração de carga de CTes Integrados cancelada: ", abortThread));
                        break;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        System.Threading.Thread.Sleep(Tempo);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task de geração de carga de CTes Integrados à fila.");
        }

        private void VerificarCTesPendentes(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = null;
            List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFes = null;

            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);
            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TempoPendenciasEmissao"], out int tempoEmissaoCTe);
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TempoPendenciasEmissaoMDFe"], out int tempoEmissaoMDFe);
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TempoReenvio"], out int tempoReenvio);
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TentativasReenvio"], out int tentativasReenvio);

            string configAdicionarCTesFilaConsulta = System.Configuration.ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"];
            if (configAdicionarCTesFilaConsulta == null || configAdicionarCTesFilaConsulta == "")
                configAdicionarCTesFilaConsulta = "SIM";

            if (tempoReenvio == 0)
                tempoReenvio = 5;
            if (tentativasReenvio == 0)
                tentativasReenvio = 5;

            if (System.Configuration.ConfigurationManager.AppSettings["VerificarPendenciasEmissao"] == "SIM")
            {
                var qtdCTesEnviados = 0;

                if (repCTe.ContarCTesPorTempoLimiteEmissao(tempoEmissaoCTe > 0 ? tempoEmissaoCTe : 10) > 0)
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["VerificarPendenciasEmissaoReiniciaFila"] == "SIM")
                    {
                        servicoCTe.NotificarCTeEnviado(null, qtdCTesEnviados, unitOfWork);

                        FilaConsultaCTe.GetNewInstance();
                        FilaConsultaCTe.GetInstance().LimparListas();
                        this.CarregarConsultaDeMDFes(unitOfWork, stringConexao);
                        this.CarregarConsultaDeCTes(unitOfWork, stringConexao);
                        this.CarregarConsultaDeNFSes(unitOfWork, stringConexao);
                        this.CarregarConsultaDeCCes(unitOfWork, stringConexao);
                    }
                    else
                    {
                        //unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);
                        listaCTes = repCTe.BuscarCTesPorTempoLimiteEmissao(tempoEmissaoCTe);
                        //unitOfWork.CommitChanges();

                        for (var i = 0; i < listaCTes.Count; i++)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(listaCTes[i].Codigo);
                            if ((cte.ModeloDocumentoFiscal.Numero == "67" || cte.ModeloDocumentoFiscal.Numero == "57"))
                            {
                                if (cte.Status == "E")
                                {
                                    servicoCTe.Consultar(ref cte, unitOfWork);

                                    if (cte.Status == "A")
                                    {
                                        bool averbaCTe = (cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.AverbaAutomaticoATM == 1) || (cte.Empresa.Configuracao != null && cte.Empresa.EmpresaPai != null && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM == 1);

                                        if (averbaCTe && cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && cte.TipoServico != Dominio.Enumeradores.TipoServico.SubContratacao)
                                        {
                                            Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unitOfWork);
                                            if (svcAverbacao.VerificaAverbacao(cte.Codigo, Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao, unitOfWork))
                                                FilaConsultaCTe.GetInstance().QueueItem(5, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao, stringConexao);
                                        }

                                        servicoCTe.AtualizarIntegracaoRetornoCTe(listaCTes[i], unitOfWork);

                                        Servicos.LsTranslog svcLsTranslog = new Servicos.LsTranslog(unitOfWork);
                                        svcLsTranslog.SalvarCTeParaIntegracao(cte.Codigo, cte.Empresa.Codigo, unitOfWork);

                                        Servicos.CIOT svcCIOT = new Servicos.CIOT(unitOfWork);
                                        svcCIOT.VincularCTeCIOTEFrete(cte.Codigo, unitOfWork);
                                    }

                                    if (cte.Status == "E")
                                    {
                                        Servicos.Log.TratarErro("CT-e " + cte.Chave + " com status enviado desde " + cte.DataIntegracao.Value.ToString("dd/MM/yyyy hh:mm:ss"), "PendenciasEmissao");
                                        qtdCTesEnviados = qtdCTesEnviados + 1;
                                    }
                                }
                            }
                            unitOfWork.FlushAndClear();
                        }
                    }
                }

                if (qtdCTesEnviados > 0)
                    servicoCTe.NotificarCTeEnviado(null, qtdCTesEnviados, unitOfWork);
            }

            //Buscar CTes com rejeição
            if (System.Configuration.ConfigurationManager.AppSettings["ReenviarRejeicaoCTe"] == "SIM")
            {
                List<int> codigosErrosCTe = new List<int> { 8888, 678, 109, 105, 217 };

                unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);
                listaCTes = repCTe.BuscarCTesPorTempoEmissaoFalhaSefaz(tempoReenvio, codigosErrosCTe, tentativasReenvio);
                unitOfWork.CommitChanges();
                for (var i = 0; i < listaCTes.Count; i++)
                {
                    Servicos.Log.TratarErro("CT-e " + listaCTes[i].Chave + " com rejeição " + listaCTes[i].MensagemRetornoSefaz + " as " + listaCTes[i].DataIntegracao.Value.ToString("dd/MM/yyyy hh:mm:ss"), "ReenvioCTe");
                    if (servicoCTe.Emitir(listaCTes[i].Codigo, 0, unitOfWork))
                    {
                        if (configAdicionarCTesFilaConsulta.Equals("SIM"))
                            servicoCTe.AdicionarCTeNaFilaDeConsulta(listaCTes[i], unitOfWork);
                        servicoCTe.AtualizarIntegracaoRetornoCTe(listaCTes[i], unitOfWork);
                    }
                }
            }

            if (System.Configuration.ConfigurationManager.AppSettings["VerificarPendenciasEmissaoMDFe"] == "SIM")
            {
                var qtdMDFesEnviados = 0;

                //unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);
                //listaMDFes = repMDFe.BuscarMDFesPorTempoLimiteEmissao(10);
                //unitOfWork.CommitChanges();
                //for (var i = 0; i < listaMDFes.Count; i++)
                //{
                //    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(listaMDFes[i].Codigo);

                //    servicoMDFe.Consultar(ref mdfe, unitOfWork, stringConexao);

                //    if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado)
                //    {
                //        Servicos.Log.TratarErro("MDF-e " + listaMDFes[i].Chave + " com status enviado desde " + listaMDFes[i].DataIntegracao.Value.ToString("dd/MM/yyyy hh:mm:ss"), "PendenciasEmissao");
                //        qtdMDFesEnviados = qtdMDFesEnviados + 1;
                //    }
                //}

                qtdMDFesEnviados = repMDFe.ContarMDFesPorTempoLimiteEmissao(tempoEmissaoMDFe > 0 ? tempoEmissaoMDFe : 10);

                if (qtdMDFesEnviados > 0)
                {
                    servicoMDFe.NotificarMDFeEnviado(null, qtdMDFesEnviados, tempoEmissaoMDFe > 0 ? tempoEmissaoMDFe.ToString() : "10", unitOfWork);

                    FilaConsultaCTe.GetNewInstance();
                    FilaConsultaCTe.GetInstance().LimparListas();
                    this.CarregarConsultaDeMDFes(unitOfWork, stringConexao);
                    this.CarregarConsultaDeCTes(unitOfWork, stringConexao);
                    this.CarregarConsultaDeNFSes(unitOfWork, stringConexao);
                    this.CarregarConsultaDeCCes(unitOfWork, stringConexao);
                }
            }

            if (System.Configuration.ConfigurationManager.AppSettings["ReenviarRejeicaoMDFe"] == "SIM")
            {
                List<int> codigosErrosMDFe = new List<int> { 8888, 678, 109, 105, 686, 611 };

                unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);
                listaMDFes = repMDFe.BuscarMDFesPorTempoEmissaoFalhaSefaz(tempoReenvio, tentativasReenvio, codigosErrosMDFe);
                unitOfWork.CommitChanges();

                for (var i = 0; i < listaMDFes.Count; i++)
                {
                    Servicos.Log.TratarErro("MDFe-e " + listaMDFes[i].Chave + " com rejeição " + listaMDFes[i].MensagemRetornoSefaz + " as " + listaMDFes[i].DataIntegracao.Value.ToString("dd/MM/yyyy hh:mm:ss"), "ReenvioMDFe");
                    if (servicoMDFe.Emitir(listaMDFes[i], unitOfWork))
                    {
                        if (configAdicionarCTesFilaConsulta.Equals("SIM"))
                            servicoMDFe.AdicionarMDFeNaFilaDeConsulta(listaMDFes[i], unitOfWork);
                        servicoMDFe.AtualizarIntegracaoRetornoMDFe(listaMDFes[i], unitOfWork);
                        servicoMDFe.RemoverPendenciaMDFeCarga(listaMDFes[i], null, unitOfWork);
                    }
                }
            }

            if (System.Configuration.ConfigurationManager.AppSettings["ReenviarEncerramento"] == "SIM")
            {
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TempoReenvioEncerramento"], out int tempoReenvioEncerramento);
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TentativasReenvioEncerramento"], out int tentativasReenvioEncerramento);

                if (tempoReenvioEncerramento == 0)
                    tempoReenvioEncerramento = 60;
                if (tentativasReenvioEncerramento == 0)
                    tentativasReenvioEncerramento = 5;

                List<int> codigosRejeicaoEncerramento = new List<int> { 670, 686, 611, 462, 662 };

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFesPendentes = repMDFe.BuscarMDFEsRejeitadosPorPendenciaEncerramento(tempoReenvioEncerramento, codigosRejeicaoEncerramento, tentativasReenvioEncerramento);
                for (var i = 0; i < listaMDFesPendentes.Count; i++)
                {
                    listaMDFesPendentes[i].TentativaReenvio += 1;
                    if (servicoMDFe.Emitir(listaMDFesPendentes[i], unitOfWork))
                    {
                        if (configAdicionarCTesFilaConsulta.Equals("SIM"))
                            servicoMDFe.AdicionarMDFeNaFilaDeConsulta(listaMDFesPendentes[i], unitOfWork);
                    }
                }
            }

            if (System.Configuration.ConfigurationManager.AppSettings["ReenviarRejeicaoValePedagio"] == "SIM")
            {
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TempoReenvioValePedagio"], out int tempoReenvioValePedagio);
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TentativasReenvioValePedagio"], out int tentativasReenvioValePedagio);
                if (tempoReenvioValePedagio == 0)
                    tempoReenvioValePedagio = 15;
                if (tentativasReenvioValePedagio == 0)
                    tentativasReenvioValePedagio = 96;

                Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unitOfWork);
                Repositorio.ValePedagioMDFe repValePedagioMDFe = new Repositorio.ValePedagioMDFe(unitOfWork);
                Servicos.Target svcTarget = new Servicos.Target(unitOfWork);
                Servicos.SemParar semParar = new Servicos.SemParar(unitOfWork);

                List<Dominio.Entidades.ValePedagioMDFeCompra> listaValePedagioMDFeCompra = repValePedagioMDFeCompra.BuscarRejeitadosPorTempo(tentativasReenvioValePedagio, tempoReenvioValePedagio);
                for (var i = 0; i < listaValePedagioMDFeCompra.Count; i++)
                {
                    Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra = listaValePedagioMDFeCompra[i];

                    bool sucesso = false;

                    switch (valePedagioMDFeCompra.Integradora)
                    {
                        case Dominio.Enumeradores.IntegradoraValePedagio.Target:
                            sucesso = svcTarget.ComprarValePedagioMDFe(ref valePedagioMDFeCompra, unitOfWork);
                            break;

                        case Dominio.Enumeradores.IntegradoraValePedagio.SemParar:
                            {
                                Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = semParar.Autenticar(valePedagioMDFeCompra, unitOfWork);

                                if (!credencial.Autenticado)
                                {
                                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                                    valePedagioMDFeCompra.Mensagem = credencial.Retorno;

                                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                                    break;
                                }

                                sucesso = !string.IsNullOrWhiteSpace(valePedagioMDFeCompra.NumeroComprovante);

                                if (!sucesso)
                                    sucesso = semParar.ComprarValePedagioMDFe(ref valePedagioMDFeCompra, credencial, unitOfWork);

                                sucesso = !string.IsNullOrWhiteSpace(valePedagioMDFeCompra.NumeroComprovante) && valePedagioMDFeCompra.Valor <= 0;

                                if (sucesso)
                                    sucesso = semParar.ObterReciboCompraValePedagio(ref valePedagioMDFeCompra, credencial, unitOfWork);

                                if (sucesso)
                                    sucesso = semParar.ConsultarIdVpo(valePedagioMDFeCompra, credencial, unitOfWork);
                            }
                            break;

                        default:
                            valePedagioMDFeCompra.Mensagem = "Integradora de vale-pedágio não suportada.";
                            break;
                    }


                    if (!sucesso)
                    {
                        if (valePedagioMDFeCompra.MDFe.Status == Dominio.Enumeradores.StatusMDFe.AguardandoCompraValePedagio)
                        {
                            valePedagioMDFeCompra.MDFe.MensagemRetornoSefaz = valePedagioMDFeCompra.Mensagem;
                            repMDFe.Atualizar(valePedagioMDFeCompra.MDFe);
                        }

                        string notificarEmail = System.Configuration.ConfigurationManager.AppSettings["NotificarEmailFalhaSefaz"];
                        if (notificarEmail == "SIM" && tempoReenvioValePedagio == valePedagioMDFeCompra.TentativaReenvio)
                            EnviarEmail("Alerta Vale Pedágio MDFe-e", "MDF-e", "Emissor " + valePedagioMDFeCompra.MDFe.Empresa.Descricao + " Número " + valePedagioMDFeCompra.MDFe.Numero.ToString() + ": " + (!string.IsNullOrWhiteSpace(valePedagioMDFeCompra.Mensagem) ? valePedagioMDFeCompra.Mensagem : "Não foi possível comprar vale pedágio"));
                    }
                    else
                    {
                        if (valePedagioMDFeCompra.Status == Dominio.Enumeradores.StatusIntegracaoValePedagio.Sucesso)
                        {
                            if (valePedagioMDFeCompra.MDFe.Status == Dominio.Enumeradores.StatusMDFe.AguardandoCompraValePedagio)
                            {
                                List<Dominio.Entidades.ValePedagioMDFe> listaValePedagioMDFe = repValePedagioMDFe.BuscarPorMDFe(valePedagioMDFeCompra.MDFe.Codigo);
                                foreach (Dominio.Entidades.ValePedagioMDFe valePedagio in listaValePedagioMDFe)
                                {
                                    if (string.IsNullOrWhiteSpace(valePedagio.NumeroComprovante) || valePedagio.NumeroComprovante == "0")
                                        repValePedagioMDFe.Deletar(valePedagio);
                                }
                            }

                            Dominio.Entidades.ValePedagioMDFe valePedagioMDFe = new Dominio.Entidades.ValePedagioMDFe();
                            valePedagioMDFe.MDFe = valePedagioMDFeCompra.MDFe;
                            valePedagioMDFe.NumeroComprovante = !string.IsNullOrEmpty(valePedagioMDFeCompra.CodigoEmissaoValePedagioANTT) ? valePedagioMDFeCompra.CodigoEmissaoValePedagioANTT : valePedagioMDFeCompra.NumeroComprovante;
                            valePedagioMDFe.ValorValePedagio = valePedagioMDFeCompra.Valor;
                            valePedagioMDFe.CNPJFornecedor = valePedagioMDFeCompra.CNPJFornecedor;
                            valePedagioMDFe.CNPJResponsavel = valePedagioMDFeCompra.CNPJResponsavel;
                            valePedagioMDFe.QuantidadeEixos = valePedagioMDFeCompra.QuantidadeEixos;
                            valePedagioMDFe.TipoCompra = valePedagioMDFeCompra.TipoCompra;

                            repValePedagioMDFe.Inserir(valePedagioMDFe);
                        }

                        if (valePedagioMDFeCompra.MDFe.Status == Dominio.Enumeradores.StatusMDFe.AguardandoCompraValePedagio)
                        {
                            List<Dominio.Entidades.ValePedagioMDFeCompra> listaComprasPedagioMDFe = repValePedagioMDFeCompra.BuscarPorMDFeTipo(valePedagioMDFeCompra.MDFe.Codigo, Dominio.Enumeradores.TipoIntegracaoValePedagio.Autorizacao);
                            int quantidadePendentes = (from obj in listaComprasPedagioMDFe where obj.Status == Dominio.Enumeradores.StatusIntegracaoValePedagio.Pendente select obj).Count();
                            int quantidadeRejeitados = (from obj in listaComprasPedagioMDFe where obj.Status == Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra select obj).Count();

                            if (quantidadePendentes == 0 && quantidadeRejeitados == 0)
                            {
                                if (servicoMDFe.Emitir(valePedagioMDFeCompra.MDFe, unitOfWork))
                                    servicoMDFe.AdicionarMDFeNaFilaDeConsulta(valePedagioMDFeCompra.MDFe, unitOfWork);
                            }
                        }
                    }

                }

            }
        }

        private void CarregarConsultaDeCTes(Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            string configAdicionarCTesFilaConsulta = System.Configuration.ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"];
            if (configAdicionarCTesFilaConsulta == null || configAdicionarCTesFilaConsulta == "")
                configAdicionarCTesFilaConsulta = "SIM";

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.BuscarTodosPorStatus(configAdicionarCTesFilaConsulta == "SIM" ? new string[] { "E", "K", "L", "X", "V", "B" } : new string[] { "K", "L", "X", "V", "B" });

            for (var i = 0; i < listaCTes.Count; i++)
            {
                if (listaCTes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    FilaConsultaCTe.GetInstance().QueueItem(5, listaCTes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, unidadeDeTrabalho.StringConexao);
            }
        }

        private void CarregarConsultaDeCCes(Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);

            List<Dominio.Entidades.CartaDeCorrecaoEletronica> listaCCes = repCCe.BuscarPorStatus(new Dominio.Enumeradores.StatusCCe[] { Dominio.Enumeradores.StatusCCe.Enviado });

            for (var i = 0; i < listaCCes.Count; i++)
            {
                if (listaCCes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    FilaConsultaCTe.GetInstance().QueueItem(5, listaCCes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CCe, stringConexao);
            }
        }

        private void CarregarConsultaDeMDFes(Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFes = repMDFe.BuscarPorStatus(new Dominio.Enumeradores.StatusMDFe[] { Dominio.Enumeradores.StatusMDFe.Enviado, Dominio.Enumeradores.StatusMDFe.EmCancelamento, Dominio.Enumeradores.StatusMDFe.EmEncerramento, Dominio.Enumeradores.StatusMDFe.EmitidoContingencia });

            for (var i = 0; i < listaMDFes.Count; i++)
            {
                if (listaMDFes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    FilaConsultaCTe.GetInstance().QueueItem(5, listaMDFes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, stringConexao);
            }
        }

        private void CarregarConsultaDeNFSes(Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

            List<Dominio.Entidades.NFSe> listaNFSes = repNFSe.BuscarPorStatus(new Dominio.Enumeradores.StatusNFSe[] { Dominio.Enumeradores.StatusNFSe.Enviado, Dominio.Enumeradores.StatusNFSe.EmCancelamento });

            for (var i = 0; i < listaNFSes.Count; i++)
            {
                FilaConsultaCTe.GetInstance().QueueItem(5, listaNFSes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe, stringConexao);
            }
        }

        private void EnviarEmail(string assunto, string documento, string texto)
        {
            try
            {
                Servicos.Email svcEmail = new Servicos.Email();

                string emailsNotificacao = System.Configuration.ConfigurationManager.AppSettings["EmailsNotificacao"];
                string emailCopia = System.Configuration.ConfigurationManager.AppSettings["EmailCopiaNotificacao"];

                if (string.IsNullOrWhiteSpace(emailsNotificacao))
                {
                    var listaEmails = emailsNotificacao.Split(';');
                    foreach (var email in listaEmails)
                    {
                        if (Utilidades.Validate.ValidarEmail(email))
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append("<p>Alerta emissão ").Append(documento).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                            sb.Append(texto).Append("</p><br /> <br />");

                            System.Text.StringBuilder ss = new System.Text.StringBuilder();
                            ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

                            svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", email, emailCopia, "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br");
                        }
                    }
                }
            }
            catch (Exception exptEmail)
            {
                Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
            }
        }

    }
}