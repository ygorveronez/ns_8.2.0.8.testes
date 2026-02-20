using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class ServicoIntegracaoValePedagio
    {
        private int Tempo = 60000; //1 MINUTO

        private ConcurrentDictionary<int, Task> ListaTasks;
        private static ServicoIntegracaoValePedagio Instance;

        public static ServicoIntegracaoValePedagio GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoIntegracaoValePedagio();

            return Instance;
        }


        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

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

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao))
                        {
                            VerificarIntegracoesValePedagioPendentes(unidadeDeTrabalho);
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
                        Servicos.Log.TratarErro(string.Concat("Task de integracao vale pedágio cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de integracao vale pedágio  cancelada: ", abortThread));
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
                Servicos.Log.TratarErro("Não foi possível adicionar a Task de integracao vale pedágio à fila.");
        }


        private void VerificarIntegracoesValePedagioPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unitOfWork);
            Repositorio.ValePedagioMDFe repValePedagioMDFe = new Repositorio.ValePedagioMDFe(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

            List<Dominio.Entidades.ValePedagioMDFeCompra> listaValePedagioMDFeCompra = repValePedagioMDFeCompra.BuscarPorStatusETipo(Dominio.Enumeradores.StatusIntegracaoValePedagio.Pendente, Dominio.Enumeradores.TipoIntegracaoValePedagio.Autorizacao);

            Servicos.Target servicoTarget = new Servicos.Target(unitOfWork);
            Servicos.SemParar semParar = new Servicos.SemParar(unitOfWork);
            Servicos.MDFe servicoMDFE = new Servicos.MDFe(unitOfWork);

            for (var i = 0; i < listaValePedagioMDFeCompra.Count; i++)
            {
                Dominio.Entidades.ValePedagioMDFeCompra valePedagioCompra = repValePedagioMDFeCompra.BuscarPorCodigo(listaValePedagioMDFeCompra[i].Codigo);

                try
                {

                    bool sucessoCompra = false;

                    switch (valePedagioCompra.Integradora)
                    {
                        case Dominio.Enumeradores.IntegradoraValePedagio.Target:
                            sucessoCompra = servicoTarget.ComprarValePedagioMDFe(ref valePedagioCompra, unitOfWork);
                            break;

                        case Dominio.Enumeradores.IntegradoraValePedagio.SemParar:
                            {
                                Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = semParar.Autenticar(valePedagioCompra, unitOfWork);

                                if (!credencial.Autenticado)
                                {
                                    valePedagioCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                                    valePedagioCompra.Mensagem = credencial.Retorno;

                                    repValePedagioMDFeCompra.Atualizar(valePedagioCompra);

                                    break;
                                }

                                sucessoCompra = !string.IsNullOrWhiteSpace(valePedagioCompra.NumeroComprovante);

                                if (!sucessoCompra)
                                    sucessoCompra = semParar.ComprarValePedagioMDFe(ref valePedagioCompra, credencial, unitOfWork);

                                sucessoCompra = !string.IsNullOrWhiteSpace(valePedagioCompra.NumeroComprovante) && valePedagioCompra.Valor <= 0;

                                if (sucessoCompra)
                                    sucessoCompra = semParar.ObterReciboCompraValePedagio(ref valePedagioCompra, credencial, unitOfWork);

                                if (sucessoCompra)
                                    sucessoCompra = semParar.ConsultarIdVpo(valePedagioCompra, credencial, unitOfWork);

                                break;
                            }

                        default:
                            var valePedagioMDFeDefault = new Dominio.Entidades.ValePedagioMDFe
                            {
                                MDFe = valePedagioCompra.MDFe,
                                ValorValePedagio = 0,
                                NumeroComprovante = "0",
                                CNPJFornecedor = valePedagioCompra.CNPJFornecedor,
                                CNPJResponsavel = valePedagioCompra.CNPJResponsavel
                            };

                            repValePedagioMDFe.Inserir(valePedagioMDFeDefault);

                            valePedagioCompra.MDFe.MensagemRetornoSefaz = valePedagioCompra.Mensagem;
                            repMDFe.Atualizar(valePedagioCompra.MDFe);
                            break;
                    }

                    if (sucessoCompra)
                    {
                        if (valePedagioCompra.Status == Dominio.Enumeradores.StatusIntegracaoValePedagio.Sucesso)
                        {
                            if (valePedagioCompra.MDFe.Status == Dominio.Enumeradores.StatusMDFe.AguardandoCompraValePedagio)
                            {
                                List<Dominio.Entidades.ValePedagioMDFe> listaValePedagioMDFe = repValePedagioMDFe.BuscarPorMDFe(valePedagioCompra.MDFe.Codigo);

                                foreach (Dominio.Entidades.ValePedagioMDFe valePedagio in listaValePedagioMDFe)
                                {
                                    if (string.IsNullOrWhiteSpace(valePedagio.NumeroComprovante) || valePedagio.NumeroComprovante == "0")
                                        repValePedagioMDFe.Deletar(valePedagio);
                                }
                            }

                            var novoValePedagioMDFe = new Dominio.Entidades.ValePedagioMDFe
                            {
                                MDFe = valePedagioCompra.MDFe,
                                NumeroComprovante = !string.IsNullOrEmpty(valePedagioCompra.CodigoEmissaoValePedagioANTT) ? valePedagioCompra.CodigoEmissaoValePedagioANTT : valePedagioCompra.NumeroComprovante,
                                ValorValePedagio = valePedagioCompra.Valor,
                                CNPJFornecedor = valePedagioCompra.CNPJFornecedor,
                                CNPJResponsavel = valePedagioCompra.CNPJResponsavel,
                                TipoCompra = valePedagioCompra.TipoCompra,
                                QuantidadeEixos = valePedagioCompra.QuantidadeEixos
                            };

                            repValePedagioMDFe.Inserir(novoValePedagioMDFe);
                        }

                        if (valePedagioCompra.MDFe.Status == Dominio.Enumeradores.StatusMDFe.AguardandoCompraValePedagio)
                        {
                            List<Dominio.Entidades.ValePedagioMDFeCompra> listaComprasPedagioMDFe = repValePedagioMDFeCompra.BuscarPorMDFeTipo(valePedagioCompra.MDFe.Codigo, Dominio.Enumeradores.TipoIntegracaoValePedagio.Autorizacao);
                            int quantidadePendentes = listaComprasPedagioMDFe.Count(obj => obj.Status == Dominio.Enumeradores.StatusIntegracaoValePedagio.Pendente);
                            int quantidadeRejeitados = listaComprasPedagioMDFe.Count(obj => obj.Status == Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra);

                            if (quantidadePendentes == 0 && quantidadeRejeitados == 0)
                            {
                                if (servicoMDFE.Emitir(valePedagioCompra.MDFe, unitOfWork))
                                    servicoMDFE.AdicionarMDFeNaFilaDeConsulta(valePedagioCompra.MDFe, unitOfWork);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problema ao comprar vale pedágio codigo: " + valePedagioCompra.Codigo + ": " + ex.Message);
                }
            }

            //Buscar os pendentes de cancelamento
        }

    }
}