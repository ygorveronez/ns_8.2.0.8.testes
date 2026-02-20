using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;


namespace EmissaoCTe.API
{

    public class ConsultarCaixaEntradaEmail
    {
        private int Tempo = 150000; //2,5 MINUTOS

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaConsultaEmailCaixaEntrada;
        private static ConsultarCaixaEntradaEmail Instance;

        public static ConsultarCaixaEntradaEmail GetInstance()
        {
            if (Instance == null)
                Instance = new ConsultarCaixaEntradaEmail();

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
                                Tempo = 500;
                #endif

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            verificarCaixaDeEntrada(unidadeDeTrabalho, stringConexao);
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
                        Servicos.Log.TratarErro(string.Concat("Task de consulta de objetos cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de consulta de objetos cancelada: ", abortThread));
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
                Servicos.Log.TratarErro("Não foi possível adicionar a task à fila.");
        }

        private void verificarCaixaDeEntrada(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork);
            List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> listaConfigEmail = repConfiguracaoEmissaoEmail.BuscarListaEmailAtivo();

            //Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            //List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> listaConfigEmail = repConfigEmailDocTransporte.BuscarListaEmailAtivo();

            foreach (Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email in listaConfigEmail)
            {
                Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                serEmail.ReceberEmail(email, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, email.Email, email.Senha, email.Pop3, email.RequerAutenticacaoPop3, email.PortaPop3, unitOfWork);
            }
        }

        private void enviarEmailProblemaCTe(string remetente, string conteudo, string stringConexao, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = null, Repositorio.UnitOfWork unitOfWork = null, System.Net.Mail.Attachment xmlAnexo = null)
        {
            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            if (email != null)
            {
                Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                Servicos.PreCTe serPreCte = new Servicos.PreCTe(unitOfWork);
                List<System.Net.Mail.Attachment> anexos = null;
                if (preCte != null)
                {
                    anexos = new List<System.Net.Mail.Attachment>();
                    string xml = serPreCte.BuscarXMLPreCte(preCte);
                    MemoryStream stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(xml));
                    System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(stream, string.Concat("pre_cte_" + preCte.Codigo, ".xml"));
                    anexos.Add(anexo);
                }

                conteudo += "<hr/>";

                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                {
                    conteudo += email.MensagemRodape.Replace("#qLinha#", "<br/>");
                }

                serEmail.EnviarEmail(email.Email, email.Email, email.Senha, remetente, "", "", "Problemas emissão automática CTe", conteudo, email.Smtp, anexos, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp);
            }
        }

    }
}