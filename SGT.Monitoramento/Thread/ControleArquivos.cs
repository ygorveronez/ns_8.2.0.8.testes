using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Collections.Specialized;
using Servicos.Embarcador.Monitoramento;
using Repositorio;
using AdminMultisoftware.Dominio.Enumeradores;

namespace SGT.Monitoramento.Thread
{
    public class ControleArquivos : AbstractThreadProcessamento
    {

        #region Atributos privados

        private static ControleArquivos Instante;
        private static System.Threading.Thread controleArquivos;

        protected bool enable = true;
        private int limiteArquivosProcessarEventos = 150;
        private int limiteArquivosProcessarTrocasAlvo = 150;
        private string diretorioFilaEventos;
        private string diretorioFilaTrcocaAlvo;
        private DateTime dataUltimoAvisoFilaArquivos;
        private DateTime dataAtual;
        private string arquivoNivelLog;
        private int tempoSleep = 30;

        #endregion

        protected override void Executar(UnitOfWork unitOfWork, string stringConexao, string ambiente, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            MonitorarAlertaQuantidadeArquivosEventosProcessar(stringConexao, unitOfWork);
            MonitorarAlertaQuantidadeArquivosTrocaAlvoProcessar(stringConexao, unitOfWork);
        }

        protected override void Parar()
        {
            if (controleArquivos != null)
            {
                controleArquivos.Abort();
                controleArquivos = null;
            }
        }

        #region Métodos públicos

        // Singleton
        public static ControleArquivos GetInstance(string stringConexao)
        {
            if (Instante == null) Instante = new ControleArquivos(stringConexao);
            return Instante;
        }

        public System.Threading.Thread Iniciar(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (enable)
                controleArquivos = base.IniciarThread(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware, arquivoNivelLog, tempoSleep);

            return controleArquivos;
        }

        public void Finalizar()
        {
            if (enable)
                Parar();
        }


        #region Construtor privado

        private ControleArquivos(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = string.IsNullOrWhiteSpace(stringConexao) ? null : new Repositorio.UnitOfWork(stringConexao);
            this.dataAtual = DateTime.Now;
            try
            {
                this.diretorioFilaEventos = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().DiretorioFila;
                this.limiteArquivosProcessarEventos = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().LimiteFilaArquivos;
                this.tempoSleep = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().TempoSleepThread;
                this.arquivoNivelLog = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().ArquivoNivelLog;

                this.limiteArquivosProcessarTrocasAlvo = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().LimiteFilaArquivos;
                this.diretorioFilaTrcocaAlvo = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().DiretorioFila;
            }
            catch (Exception e)
            {
                Log(e.Message);
                throw e;
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }

        #endregion

        public void MonitorarAlertaQuantidadeArquivosEventosProcessar(string stringConexao, UnitOfWork unitOfWork)
        {
            int numArquivosAProcessar = base.ContarPendenciasFila(this.diretorioFilaEventos);
            Log("Arquivos Pendentes Eventos " + numArquivosAProcessar);

            DateTime dataAtual = DateTime.Now;
            TimeSpan atrasoAviso = dataAtual - dataUltimoAvisoFilaArquivos;
            string emailsParaAviso = "fernando@multisoftware.com.br,guilherme.romanini@multisoftware.com.br,rodolfo.trevisol@multisoftware.com.br";

            if (numArquivosAProcessar >= limiteArquivosProcessarEventos)
            {
                //fixo 15 minutos.
                if (atrasoAviso.TotalMinutes > 15)
                {
                    dataUltimoAvisoFilaArquivos = dataAtual;
                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = Program.Cliente; // base.ObterCliente();
                    if (cliente != null)
                    {
                        string maskDate = "dd/MM/yyyy HH:mm:ss";

                        string nomeCliente = cliente.NomeFantasia;
                        string subject = $"Alerta Monitoramento: Quantidade de Eventos a Processar em ( {nomeCliente} )";
                        string body = $"<h1>Atenção!</h1>";
                        body += "<p>";
                        body += $"Ambiente: {nomeCliente}<br/>";
                        body += $"Serviço Multisoftware: {cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString()}<br/>";
                        body += $"Data atual: {dataAtual.ToString(maskDate)}<br/><br/>";
                        body += $"<b>Fila de arquivos para processar Eventos acima do normal, total de Arquivos: {numArquivosAProcessar}</b><br/>";
                        body += $"Em: {diretorioFilaEventos}<br/>";
                        body += "</p>";

                        List<string> emails = new List<string>();
                        string[] emailsDestino = emailsParaAviso.Split(',');
#if !DEBUG
                            emails = emailsDestino.ToList();
#else
                        emails.Add("fernando@multisoftware.com.br");
#endif
                        if (emails.Count() > 0)
                        {
                            Servicos.Email svcEmail = new Servicos.Email(stringConexao);
                            svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, subject, body, string.Empty, null, string.Empty, true, "cte@multisoftware.com.br", 0, unitOfWork, 0, true, emails, false);
                        }
                    }
                }
            }
        }

        private void MonitorarAlertaQuantidadeArquivosTrocaAlvoProcessar(string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {

            int numArquivosAProcessar = base.ContarPendenciasFila(diretorioFilaTrcocaAlvo);
            Log("Arquivos Pendentes Troca Alvo " + numArquivosAProcessar);

            DateTime dataAtual = DateTime.Now;
            TimeSpan atrasoAviso = dataAtual - dataUltimoAvisoFilaArquivos;
            string emailsParaAviso = "fernando@multisoftware.com.br,guilherme.romanini@multisoftware.com.br,rodolfo.trevisol@multisoftware.com.br";

            if (numArquivosAProcessar >= limiteArquivosProcessarTrocasAlvo)
            {
                //fixo 15 minutos.
                if (atrasoAviso.TotalMinutes > 15)
                {
                    dataUltimoAvisoFilaArquivos = dataAtual;
                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = Program.Cliente; //base.ObterCliente();
                    if (cliente != null)
                    {
                        string maskDate = "dd/MM/yyyy HH:mm:ss";

                        string nomeCliente = cliente.NomeFantasia;
                        string subject = $"Alerta Monitoramento: Quantidade de Trocas de Alvo a Processar em ( {nomeCliente} )";
                        string body = $"<h1>Atenção!</h1>";
                        body += "<p>";
                        body += $"Ambiente: {nomeCliente}<br/>";
                        body += $"Serviço Multisoftware: {cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString()}<br/>";
                        body += $"Data atual: {dataAtual.ToString(maskDate)}<br/><br/>";
                        body += $"<b>Fila de arquivos para processar TROCA DE ALVO acima do normal ({limiteArquivosProcessarTrocasAlvo}), total de Arquivos: {numArquivosAProcessar}</b><br/>";
                        body += $"Em: {diretorioFilaTrcocaAlvo}<br/>";
                        body += "</p>";

                        List<string> emails = new List<string>();
                        string[] emailsDestino = emailsParaAviso.Split(',');
#if !DEBUG
                            emails = emailsDestino.ToList();
#else
                        emails.Add("fernando@multisoftware.com.br");
#endif
                        if (emails.Count() > 0)
                        {
                            Servicos.Email svcEmail = new Servicos.Email(stringConexao);
                            svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, subject, body, string.Empty, null, string.Empty, true, "cte@multisoftware.com.br", 0, unitOfWork, 0, true, emails, false);
                        }
                    }
                }
            }
        }


        #endregion
    }
}
