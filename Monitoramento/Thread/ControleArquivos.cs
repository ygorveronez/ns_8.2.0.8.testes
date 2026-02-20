using AdminMultisoftware.Dominio.Enumeradores;
using Repositorio;
using Servicos.Embarcador.Monitoramento;

namespace Monitoramento.Thread
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
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

            if (configuracaoMonitoramento.EnviarAlertasMonitoramentoEmail)
            {
                MonitorarAlertaQuantidadeArquivosEventosProcessar(stringConexao, unitOfWork, clienteMultisoftware);
                MonitorarAlertaQuantidadeArquivosTrocaAlvoProcessar(stringConexao, unitOfWork, clienteMultisoftware);
            }
        }

        protected override void Parar()
        {
            if (controleArquivos != null)
            {
                controleArquivos.Interrupt();
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

        public System.Threading.Thread Iniciar(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancellationToken)
        {
            if (enable)
                controleArquivos = base.IniciarThread(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware, arquivoNivelLog, tempoSleep, cancellationToken);

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
                throw;
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }

        #endregion

        public void MonitorarAlertaQuantidadeArquivosEventosProcessar(string stringConexao, UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            int numArquivosAProcessar = base.ContarPendenciasFila(this.diretorioFilaEventos);
            Log("Arquivos Pendentes Eventos " + numArquivosAProcessar);

            DateTime dataAtual = DateTime.Now;
            TimeSpan atrasoAviso = dataAtual - dataUltimoAvisoFilaArquivos;

            if (numArquivosAProcessar >= limiteArquivosProcessarEventos)
            {
                //fixo 15 minutos.
                if (atrasoAviso.TotalMinutes > 15)
                {
                    dataUltimoAvisoFilaArquivos = dataAtual;

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

                        Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                        List<string> emails = new List<string>();
                        if (!string.IsNullOrWhiteSpace(configuracaoMonitoramento.EmailsAlertaMonitoramento))
                            emails.AddRange(configuracaoMonitoramento.EmailsAlertaMonitoramento.Split(';').ToList());

                        emails = emails.Distinct().ToList();
                        emails.Add("fernando.morh@multisoftware.com.br");
                        emails.Add("rodolfo.trevisol@multisoftware.com.br");
                        emails.Add("guilherme.romanini@multisoftware.com.br");

                        if (emails.Count() > 0)
                        {
                            Servicos.Email svcEmail = new Servicos.Email(unitOfWork);
                            svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, subject, body, string.Empty, null, string.Empty, true, "cte@multisoftware.com.br", 0, unitOfWork, 0, true, emails, false);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic fila = new Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic();
                        fila.ValorAlerta = numArquivosAProcessar;
                        fila.CaminhoLocal = diretorioFilaEventos;
                        fila.Cliente = nomeCliente;
                        fila.TipoServico = cliente.ClienteConfiguracao.TipoServicoMultisoftware;
                        fila.DataAtual = dataAtual.ToString(maskDate);
                        fila.DescricaoAlerta = "Fila de arquivos para processar Eventos acima do normal";
                        fila.CodigoTipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogElastic.FilasProcessarEventos;

                        Servicos.Log.TratarErro(fila, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Advertencia);
                    }
                }
            }
        }

        private void MonitorarAlertaQuantidadeArquivosTrocaAlvoProcessar(string stringConexao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {

            int numArquivosAProcessar = base.ContarPendenciasFila(diretorioFilaTrcocaAlvo);
            Log("Arquivos Pendentes Troca Alvo " + numArquivosAProcessar);

            DateTime dataAtual = DateTime.Now;
            TimeSpan atrasoAviso = dataAtual - dataUltimoAvisoFilaArquivos;

            if (numArquivosAProcessar >= limiteArquivosProcessarTrocasAlvo)
            {
                //fixo 15 minutos.
                if (atrasoAviso.TotalMinutes > 15)
                {
                    dataUltimoAvisoFilaArquivos = dataAtual;

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

                        Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                        List<string> emails = new List<string>();
                        if (!string.IsNullOrWhiteSpace(configuracaoMonitoramento.EmailsAlertaMonitoramento))
                            emails.AddRange(configuracaoMonitoramento.EmailsAlertaMonitoramento.Split(';').ToList());

                        emails = emails.Distinct().ToList();
                        emails.Add("fernando.morh@multisoftware.com.br");
                        emails.Add("rodolfo.trevisol@multisoftware.com.br");
                        emails.Add("guilherme.romanini@multisoftware.com.br");
                        if (emails.Count() > 0)
                        {
                            Servicos.Email svcEmail = new Servicos.Email(unitOfWork);
                            svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, subject, body, string.Empty, null, string.Empty, true, "cte@multisoftware.com.br", 0, unitOfWork, 0, true, emails, false);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic fila = new Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic();
                        fila.ValorAlerta = numArquivosAProcessar;
                        fila.CaminhoLocal = diretorioFilaEventos;
                        fila.Cliente = nomeCliente;
                        fila.TipoServico = cliente.ClienteConfiguracao.TipoServicoMultisoftware;
                        fila.DataAtual = dataAtual.ToString(maskDate);
                        fila.DescricaoAlerta = "Fila de arquivos para processar Eventos acima do normal";
                        fila.CodigoTipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogElastic.FilasProcessarTrocaAlvo;

                        Servicos.Log.TratarErro(fila, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Advertencia);
                    }
                }
            }
        }


        #endregion
    }
}
