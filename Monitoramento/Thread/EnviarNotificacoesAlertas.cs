using Servicos.Embarcador.Monitoramento;

namespace Monitoramento.Thread
{
    public class EnviarNotificacoesAlertas : AbstractThreadProcessamento
    {

        #region Propriedades privadas

        private static EnviarNotificacoesAlertas Instance;
        private static System.Threading.Thread NotificacoesAlertaThread;

        private int tempoSleep = 5;
        private bool enable = true;
        private int limiteRegistros = 100;
        private string arquivoNivelLog;
        private string enviroment = "Homologacao";

        #endregion

        #region Métodos públicos

        public static EnviarNotificacoesAlertas GetInstance(string stringConexao)
        {
            if (Instance == null) Instance = new EnviarNotificacoesAlertas(stringConexao);
            return Instance;
        }

        public System.Threading.Thread Iniciar(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancellationToken)
        {
            if (enable)
                NotificacoesAlertaThread = base.IniciarThread(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware, arquivoNivelLog, tempoSleep, cancellationToken);

            return NotificacoesAlertaThread;
        }

        public void Finalizar()
        {
            if (enable)
                Parar();
        }

        #endregion

        #region Implementação dos métodos abstratos

        override protected void Executar(Repositorio.UnitOfWork unitOfWork, string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            this.enviroment = ambiente;
            GerarAlertaNotificacao(unitOfWork, stringConexao, tipoServicoMultisoftware);
        }

        override protected void Parar()
        {
            if (NotificacoesAlertaThread != null)
            {
                NotificacoesAlertaThread.Interrupt();
                NotificacoesAlertaThread = null;
            }
        }

        #endregion

        #region Construtor privado

        private EnviarNotificacoesAlertas(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = string.IsNullOrWhiteSpace(stringConexao) ? null : new Repositorio.UnitOfWork(stringConexao);
            try
            {
                enable = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoEnviarNotificacoesAlerta().Ativo;
                tempoSleep = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoEnviarNotificacoesAlerta().TempoSleepThread;
                limiteRegistros = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoEnviarNotificacoesAlerta().LimiteRegistros;
                arquivoNivelLog = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoEnviarNotificacoesAlerta().ArquivoNivelLog;
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

        #region Métodos privados

        private void GerarAlertaNotificacao(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            DateTime dataAtual = DateTime.Now;

            var repAlertaNotificacao = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTratativa(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa> monitoramentoEventoTratativas = repAlertaNotificacao.BuscarTodosAtivos();
            var totalTratativas = monitoramentoEventoTratativas.Count;
            Log($"Encontradas {totalTratativas} tratativas");
            if (totalTratativas > 0)
            {

                var repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertasEmAberto = repAlertaMonitor.BuscarTodosEmAbertoQueEnviaEmailEPossuiSequenciaTratativaMenor();
                var totalAlertas = alertasEmAberto.Count;
                Log($"Processando {totalAlertas} alertas em aberto");
                if (totalAlertas > 0)
                {
                    var repAlertaMonitorNotificacao = new Repositorio.Embarcador.Logistica.AlertaMonitorNotificacao(unitOfWork);

                    Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> monitoramentosEventos = repMonitoramentoEvento.BuscarTodosAtivos();

                    var repCargaResponsavel = new Repositorio.Embarcador.Cargas.CargaResponsavel(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaResponsavel> cargasResponsavel = repCargaResponsavel.BuscarTodosResponsaveis(DateTime.Now);

                    for (int i = 0; i < totalAlertas; i++)
                    {
                        var ultimaNotificacao = repAlertaMonitorNotificacao.BuscarUltimoPorAlerta(alertasEmAberto[i].Codigo); //alertasEmAberto[i].AlertaMonitorNotificacao.LastOrDefault();
                        if (ultimaNotificacao == null)
                        {
                            // Inicializa controle de notificaçao
                            var novaNotificacao = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao
                            {
                                Data = dataAtual,
                                Sequencia = 0,
                                AlertaMonitor = alertasEmAberto[i]
                            };
                            repAlertaMonitorNotificacao.Inserir(novaNotificacao);
                        }
                        else
                        {
                            int sequencia = 1;
                            DateTime dataUltimaNotificacao = dataAtual;
                            if (ultimaNotificacao != null)
                            {
                                sequencia = ultimaNotificacao.Sequencia + 1;
                                dataUltimaNotificacao = ultimaNotificacao.Data;
                            }

                            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa monitoramentoEventoTratativa = (
                                from conf in monitoramentoEventoTratativas
                                where conf.Sequencia == sequencia && conf.MonitoramentoEvento.TipoAlerta == alertasEmAberto[i].TipoAlerta
                                select conf
                            ).FirstOrDefault();

                            if (monitoramentoEventoTratativa != null && (monitoramentoEventoTratativa.EnvioEmail || monitoramentoEventoTratativa.EnvioEmailTransportador || monitoramentoEventoTratativa.EnvioEmailCliente))
                            {
                                bool tempoNotificacao = dataAtual > dataUltimaNotificacao.AddMinutes(monitoramentoEventoTratativa.TempoEmMinutos);
                                if (tempoNotificacao)
                                {
                                    string mensagem = "";
                                    string assunto = "";

                                    //propriedade setada direto no banco por enquanto, valido apenas para Atraso na descarga e Aguardando Liberacao (estadia)
                                    if (monitoramentoEventoTratativa.MonitoramentoEvento != null && monitoramentoEventoTratativa.MonitoramentoEvento.TipoAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.AtrasoNaDescarga && monitoramentoEventoTratativa.ModeloEmailPadrao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoModeloEmail.ModeloDPA)
                                    {
                                        DateTime DataEntregaPrevista = alertasEmAberto[i].CargaEntrega != null && alertasEmAberto[i].CargaEntrega.DataPrevista.HasValue ? alertasEmAberto[i].CargaEntrega.DataPrevista.Value : new DateTime();
                                        DateTime DataEntradaRaio = alertasEmAberto[i].CargaEntrega != null && alertasEmAberto[i].CargaEntrega.DataEntradaRaio.HasValue ? alertasEmAberto[i].CargaEntrega.DataEntradaRaio.Value : new DateTime();
                                        TimeSpan diff = new TimeSpan();
                                        if (DataEntregaPrevista != DateTime.MinValue)
                                            diff = DataEntradaRaio - DataEntregaPrevista;

                                        string NomeFantasiaCliente = alertasEmAberto[i].CargaEntrega != null ? alertasEmAberto[i].CargaEntrega.Cliente?.NomeFantasia : "";
                                        string NotasFiscais = alertasEmAberto[i].CargaEntrega != null && alertasEmAberto[i].CargaEntrega.NotasFiscais != null && alertasEmAberto[i].CargaEntrega.NotasFiscais.Count > 0 ? string.Join(",", (from notas in alertasEmAberto[i].CargaEntrega?.NotasFiscais select notas?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero)) : string.Empty;
                                        assunto = $@"[Monitoramento] {alertasEmAberto[i].Veiculo.Placa} ATRASO NA DESCARGA";

                                        mensagem = MontarEmailAlertaAtrasoDescarga(alertasEmAberto[i], DataEntregaPrevista, DataEntradaRaio, NotasFiscais, NomeFantasiaCliente, diff, tipoServicoMultisoftware);
                                    }
                                    else if (monitoramentoEventoTratativa.MonitoramentoEvento != null && monitoramentoEventoTratativa.MonitoramentoEvento.TipoAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PermanenciaNoRaioEntrega && monitoramentoEventoTratativa.ModeloEmailPadrao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoModeloEmail.ModeloDPA)
                                    {
                                        DateTime DataEntregaPrevista = alertasEmAberto[i].CargaEntrega != null && alertasEmAberto[i].CargaEntrega.DataPrevista.HasValue ? alertasEmAberto[i].CargaEntrega.DataPrevista.Value : new DateTime();
                                        DateTime DataEntradaRaio = alertasEmAberto[i].CargaEntrega != null && alertasEmAberto[i].CargaEntrega.DataEntradaRaio.HasValue ? alertasEmAberto[i].CargaEntrega.DataEntradaRaio.Value : new DateTime();

                                        if (DataEntregaPrevista == DateTime.MinValue || DataEntradaRaio == DateTime.MinValue)
                                            continue;

                                        DateTime dataMaximoLiberacao;
                                        if (DataEntradaRaio < DataEntregaPrevista)
                                            dataMaximoLiberacao = DataEntregaPrevista.AddHours(5);
                                        else
                                            dataMaximoLiberacao = DataEntradaRaio.AddHours(5);

                                        if (dataAtual < dataMaximoLiberacao) // nao envia o email ate a data maximo liberacao
                                            continue;

                                        string NomeFantasiaCliente = alertasEmAberto[i].CargaEntrega != null ? alertasEmAberto[i].CargaEntrega.Cliente?.NomeFantasia : "";
                                        string NotasFiscais = alertasEmAberto[i].CargaEntrega != null && alertasEmAberto[i].CargaEntrega.NotasFiscais != null && alertasEmAberto[i].CargaEntrega.NotasFiscais.Count > 0 ? string.Join(",", (from notas in alertasEmAberto[i].CargaEntrega?.NotasFiscais select notas?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero)) : string.Empty;
                                        assunto = $@"[Monitoramento] {alertasEmAberto[i].Veiculo.Placa} AGUARDANDO LIBERAÇÃO (ESTADIA)";

                                        mensagem = MontarEmailAlertaPermanenciaRaioCliente(alertasEmAberto[i], DataEntregaPrevista, DataEntradaRaio, NotasFiscais, NomeFantasiaCliente, tipoServicoMultisoftware);
                                    }
                                    else
                                    {
                                        string tituloAlerta = BuscarDescricaoAlerta(alertasEmAberto[i].TipoAlerta, monitoramentosEventos);
                                        Log($"Alerta {alertasEmAberto[i].Codigo}-{alertasEmAberto[i].Descricao} {alertasEmAberto[i].Veiculo.Placa}", 1);
                                        assunto = $@"[Monitoramento] {alertasEmAberto[i].Veiculo.Placa} {tituloAlerta}";
                                        mensagem = MontarEmailAlertaPadrao(alertasEmAberto[i], tituloAlerta, sequencia, tipoServicoMultisoftware);
                                    }

                                    if (monitoramentoEventoTratativa.EnvioEmail)
                                    {
                                        List<Dominio.Entidades.Usuario> funcionarios = new List<Dominio.Entidades.Usuario>();
                                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                            funcionarios = cargasResponsavel.Where(cr => cr.CategoriaResponsavel.Codigo == monitoramentoEventoTratativa.CategoriaResponsavel.Codigo).Select(o => o?.Funcionario).Distinct().ToList();
                                        else
                                            funcionarios = cargasResponsavel.Where(cr => cr.CategoriaResponsavel.Codigo == monitoramentoEventoTratativa.CategoriaResponsavel.Codigo && cr.Filiais
                                                                                      .Where(crf => crf.Codigo == alertasEmAberto[i]?.Carga?.Filial.Codigo).Any()).Select(o => o?.Funcionario).Distinct().ToList();

                                        foreach (var funcionario in funcionarios)
                                        {
                                            if (funcionario != null)
                                            {
                                                EnviarEmailNotificacao(funcionario?.Email, mensagem, assunto, stringConexao, unitOfWork);
                                                Log($"Envio email para funcionario {funcionario?.Email}, sequencia {sequencia}", 2);
                                            }
                                        }
                                    }

                                    if (monitoramentoEventoTratativa.EnvioEmailTransportador)
                                    {
                                        EnviarEmailNotificacao(alertasEmAberto[i]?.Carga?.Empresa?.Email ?? string.Empty, mensagem, assunto, stringConexao, unitOfWork);
                                        Log($"Envio email para transportador {alertasEmAberto[i]?.Carga?.Empresa?.Email ?? string.Empty}, sequencia {sequencia}", 2);
                                    }

                                    if (monitoramentoEventoTratativa.EnvioEmailCliente)
                                    {

                                        if (alertasEmAberto[i].CargaEntrega == null)
                                        {
                                            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(alertasEmAberto[i].Carga.Codigo);

                                            if ((cargaPedidos != null) || (cargaPedidos.Count > 0))
                                            {
                                                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from o in cargaPedidos select o.Pedido).Distinct().ToList();
                                                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                                                {
                                                    if (pedido.Destinatario != null)
                                                    {

                                                        List<string> emails = new List<string>();
                                                        if (!string.IsNullOrWhiteSpace(pedido.Destinatario?.Email))
                                                        {
                                                            if (pedido.Destinatario.Email.Contains(";"))
                                                            {
                                                                string[] emailsSeparados = pedido.Destinatario.Email.Split(';');
                                                                for (int k = 0; k < emailsSeparados.Count(); k++)
                                                                    emails.Add(emailsSeparados[k]);
                                                            }
                                                            else
                                                                emails.Add(pedido.Destinatario?.Email);
                                                        }

                                                        foreach (string email in emails)
                                                        {
                                                            EnviarEmailNotificacao(email, mensagem, assunto, stringConexao, unitOfWork);
                                                            Log($"Envio email para cliente do Pedido {email}, sequencia {sequencia}", 2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //envia para o cliente da CargaEntrega.
                                            List<string> emails = new List<string>();
                                            if (!string.IsNullOrWhiteSpace(alertasEmAberto[i].CargaEntrega.Cliente?.Email))
                                            {
                                                if (alertasEmAberto[i].CargaEntrega.Cliente.Email.Contains(";"))
                                                {
                                                    string[] emailsSeparados = alertasEmAberto[i].CargaEntrega.Cliente.Email.Split(';');
                                                    for (int k = 0; k < emailsSeparados.Count(); k++)
                                                        emails.Add(emailsSeparados[k]);
                                                }
                                                else
                                                    emails.Add(alertasEmAberto[i].CargaEntrega.Cliente?.Email);
                                            }

                                            foreach (string email in emails)
                                            {
                                                EnviarEmailNotificacao(email, mensagem, assunto, stringConexao, unitOfWork);
                                                Log($"Envio email para cliente do Pedido {email}, sequencia {sequencia}", 2);
                                            }
                                        }
                                    }

                                    Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao novaNotificacao = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao
                                    {
                                        Data = dataAtual,
                                        Sequencia = sequencia,
                                        AlertaMonitor = alertasEmAberto[i]
                                    };
                                    repAlertaMonitorNotificacao.Inserir(novaNotificacao);

                                    if (alertasEmAberto[i].AlertaTrativaAutomatica)
                                    {
                                        Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);

                                        // agora que enviou o alerta para o email, devemos finalizar o alerta (trativa automatica).
                                        alertasEmAberto[i].DataFim = dataAtual;
                                        alertasEmAberto[i].Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado;

                                        repAlertaMonitor.Atualizar(alertasEmAberto[i]);
                                        servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(alertasEmAberto[i], null);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        private string MontarEmailAlertaPadrao(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor Alerta, string tituloAlerta, int sequencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string mensagem = $@"<table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""100%"">
                                    <tr>
                                        <td style=""padding:20px; background-color:#EEE;""> 
                                            <table border=""0"" cellspacing=""0"" cellpadding=""0"" style=""width:500px"" align=""center"">
                                                <tr>
                                                    <td>
                                                        <div style=""border-bottom: 1px solid #EEE;padding:40px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 36px; font-weight: bold;"">Alerta {tituloAlerta}</div>
                                                        <div style=""border-bottom: 1px solid #EEE;padding:40px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 14px;line-height: 150%; "">
                                                            <p style=""margin:0 0 20px 0"">Foi identificado um comportamento anormal com o veículo monitorado e um alerta foi registrado.</p>
                                                            <p style=""margin:0 0 20px 0; padding:10px; border: 1px solid #DDD; border-radius: 4px;line-height: 150%; "">
                                                                <label style=""display: inline-block; width: 100px; font-style: italic;"">Data:</label> {Alerta.Data.ToString("dd/MM/yyyy HH:mm:ss")}<br/>
                                                                <label style=""display: inline-block; width: 100px; font-style: italic;"">Veiculo:</label> {Alerta.Veiculo?.Placa ?? ""}<br/>
                                                                <label style=""display: inline-block; width: 100px; font-style: italic;"">Motorista(s):</label> {Alerta.Carga?.NomeMotoristas ?? ""}<br/>
                                                                <label style=""display: inline-block; width: 100px; font-style: italic;"">Transportador:</label> {Alerta.Carga?.Empresa?.CNPJ_Formatado ?? ""} - {Alerta.Carga?.Empresa?.RazaoSocial ?? ""}<br/>
                                                                <label style=""display: inline-block; width: 100px; font-style: italic;"">Alerta:</label> {Alerta.Codigo} - {Alerta.Descricao}<br/>
                                                                <label style=""display: inline-block; width: 100px; font-style: italic;"">Valor:</label> {Alerta.AlertaDescricao}<br/>
                                                                <label style=""display: inline-block; width: 100px; font-style: italic;"">Sequência:</label> {sequencia}
                                                            </p>
                                                            <p style=""margin: 0px; line-height: 150%; "">{(!Alerta.AlertaTrativaAutomatica ? "Será necessário registrar a tratativa no sistema." : "Alerta com tratativa Automática")}</p>
                                                        </div>
                                                        <div style=""padding:40px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 14px;line-height: 150%; "">
                                                            <strong>Monitoramento {tipoServicoMultisoftware}</strong><br/>
                                                            <span style=""font-style:italic"">Multisoftware</span>
                                                        </div>
                                                        <div style=""padding:20px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; font-size: 10px; color:#CCC"">E-mail enviado automaticamente.</div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>";

            return mensagem;
        }

        private string MontarEmailAlertaAtrasoDescarga(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor Alerta, DateTime DataEntregaPrevista, DateTime DataEntradaRaio, string NotasFiscais, string NomeFantasiaCliente, TimeSpan diff, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {

            string tituloAlerta = "ATRASO NA DESCARGA";

            string mensagem = $@"<table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""100%"">
                                    <tr>
                                        <td style=""padding:10px; background-color:#FFF;""> 
                                            <table border=""0"" cellspacing=""0"" cellpadding=""0"" style=""width:800px"" align=""center"">
                                                <tr>
                                                    <td style=""padding:20px; text-align:center; border: 2px solid #000; border-radius: 4px;"" >
                                                        <div style=""background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 36px; font-weight: bold;""><label style=""color:red"">ALERTA: </label> {tituloAlerta}</div>
                                                        <div style=""background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 14px;line-height: 150%; text-align:left"">
                                                            <p style=""margin:0 0 30px 0; padding:20px; line-height: 150%; "">
                                                                <label style=""display: inline-block; width: 100px;""><b>VEÍCULO:</b></label><label style=""margin-left:100px""> {Alerta.Veiculo?.Placa}</label><br/>
                                                                <label style=""display: inline-block; width: 100px;""><b>CLIENTE:</b></label><label style=""margin-left:100px""> {NomeFantasiaCliente}</label><br/>
                                                                <label style=""display: inline-block; width: 100px;""><b>NOTAS FISCAIS:</b></label><label style=""margin-left:100px""> {NotasFiscais}</label><br/>
                                                                <label style=""display: inline-block; width: 100px;""><b>MOTORISTA:</b></label> <label style=""margin-left:100px"">{Alerta.Carga.NomeMotoristas}</label><br/>
                                                                <label style=""display: inline-block; width: 200px;""><b>TRANSPORTADORA:</b></label> <label style=""margin-left:0px"">{Alerta.Carga.Empresa.NomeFantasia}</label><br/>
                                                                <label style=""width: 100px;""><b>DATA DA AGENDA:</b></label> <label style=""margin-left:70px"">{(DataEntregaPrevista != DateTime.MinValue ? DataEntregaPrevista.ToString("dd/MM/yyyy HH:mm:ss") : "Sem informação de data Prevista")}</label><br/>
                                                                <label style=""width: 100px;""><b>DATA CHEGADA VEÍCULO:</b></label> <label style=""margin-left:17px"">{(DataEntradaRaio != DateTime.MinValue ? DataEntradaRaio.ToString("dd/MM/yyyy HH:mm:ss") : "Sem informação de data Chegada")}</label><br/>
                                                                <label style=""width: 100px;""><b>TEMPO DE ATRASO:</b></label><label style=""margin-left:34px; background-color:yellow"">{String.Format("{0}:{1}:{2}", diff.Hours.ToString("00"), diff.Minutes.ToString("00"), diff.Seconds.ToString("00"))}</label><br/>
                                                                <br/>
                                                                </p>
                                                                <p style = ""margin:0 0 30px 0; padding: 20px; line-height: 150%;"">{(!Alerta.AlertaTrativaAutomatica ? "Será necessário registrar a tratativa no sistema." : "Alerta com tratativa Automática")}</p>
                                                                </div>
                                                        <div style = ""padding: 20px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size:14px;line-height: 150%;"">
                                                        <strong> Monitoramento {tipoServicoMultisoftware} </strong><br/>
                                                        <span style = ""font-style:italic""> Multisoftware </span>
                                                        </div>
                                                        <div style = ""padding: 20px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; font-size: 10px; color:#CCC"">E-mail enviado automaticamente.</div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table> ";


            return mensagem;

        }

        private string MontarEmailAlertaPermanenciaRaioCliente(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor Alerta, DateTime DataEntregaPrevista, DateTime DataEntradaRaio, string NotasFiscais, string NomeFantasiaCliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string tituloAlerta = "AGUARDANDO LIBERAÇÃO (ESTADIA)";

            string maximoLiberacao;
            TimeSpan diff;
            DateTime dataAtual = DateTime.Now;

            if (DataEntradaRaio < DataEntregaPrevista)
            {
                maximoLiberacao = DataEntregaPrevista.AddHours(5).ToString("dd/MM/yyyy HH:mm:ss");
                diff = DataEntregaPrevista - dataAtual;
            }
            else
            {
                maximoLiberacao = DataEntradaRaio.AddHours(5).ToString("dd/MM/yyyy HH:mm:ss");
                diff = DataEntradaRaio - dataAtual;
            }

            string mensagem = $@"<table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""100%"">
                                    <tr>
                                        <td style=""padding:10px; background-color:#FFF;""> 
                                            <table border=""0"" cellspacing=""0"" cellpadding=""0"" style=""width:800px"" align=""center"">
                                                <tr>
                                                    <td style=""padding:20px; text-align:center; border: 2px solid #000; border-radius: 4px;"" >
                                                        <div style=""background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 36px; font-weight: bold;""><label style=""color:red"">ALERTA: </label> {tituloAlerta}</div>
                                                        <div style=""background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 14px;line-height: 150%; text-align:left"">
                                                            <p style=""margin:0 0 30px 0; padding:20px; line-height: 150%; "">
                                                                <label style=""display: inline-block; width: 100px;""><b>VEÍCULO:</b></label><label style=""margin-left:100px""> {Alerta.Veiculo?.Placa}</label><br/>
                                                                <label style=""display: inline-block; width: 100px;""><b>CLIENTE:</b></label><label style=""margin-left:100px""> {NomeFantasiaCliente}</label><br/>
                                                                <label style=""display: inline-block; width: 100px;""><b>NOTAS FISCAIS:</b></label><label style=""margin-left:100px""> {NotasFiscais}</label><br/>
                                                                <label style=""display: inline-block; width: 100px;""><b>MOTORISTA:</b></label> <label style=""margin-left:100px"">{Alerta.Carga.NomeMotoristas}</label><br/>
                                                                <label style=""display: inline-block; width: 200px;""><b>TRANSPORTADORA:</b></label> <label style=""margin-left:0px"">{Alerta.Carga.Empresa.NomeFantasia}</label><br/>
                                                                <label style=""width: 100px;""><b>DATA DA AGENDA:</b></label> <label style=""margin-left:70px"">{(DataEntregaPrevista != DateTime.MinValue ? DataEntregaPrevista.ToString("dd/MM/yyyy HH:mm:ss") : "Sem informação de data Prevista")}</label><label style="" width: 100px; margin-left:20px""><b> MÁXIMO PARA LIBERAÇÃO:</b></label> <label style=""margin-left:10px; background-color:yellow"">{maximoLiberacao}</label><br/>
                                                                <label style=""width: 100px;""><b>DATA CHEGADA VEÍCULO:</b></label> <label style=""margin-left:17px"">{(DataEntradaRaio != DateTime.MinValue ? DataEntradaRaio.ToString("dd/MM/yyyy HH:mm:ss") : "Sem informação de data Chegada")}</label><br/>
                                                                <label style=""width: 100px;""><b>TEMPO AG. DESCARGA:</b></label><label style=""margin-left:34px; background-color:yellow"">{String.Format("{0}:{1}:{2}", diff.Hours.ToString("00"), diff.Minutes.ToString("00"), diff.Seconds.ToString("00"))}</label><br/>
                                                                <br/>
                                                                <label style =""width: 100px;""><label style=""color: red"">OBS:</label> O TEMPO É CALCULADO BASEADO NO HORÁRIO DA AGENDA (-) O HORÁRIO ATUAL.</label><br/>
                                                                <label style =""width: 100px;""><b> Estadia é gerada apos 05:00 do veículo dentro da unidade.</b></label><br/>
                                                                </p>
                                                                <p style = ""margin:0 0 30px 0; padding: 20px; line-height: 150%;"">{(!Alerta.AlertaTrativaAutomatica ? "Será necessário registrar a tratativa no sistema." : "Alerta com tratativa Automática")}</p>
                                                                </div>
                                                        <div style = ""padding: 20px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size:14px;line-height: 150%;"">
                                                        <strong> Monitoramento {tipoServicoMultisoftware} </strong><br/>
                                                        <span style = ""font-style:italic""> Multisoftware </span>
                                                        </div>
                                                        <div style = ""padding: 20px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; font-size: 10px; color:#CCC"">E-mail enviado automaticamente.</div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table> ";

            return mensagem;

        }

        private void EnviarEmailNotificacao(string email, string mensagem, string assunto, string stringConexao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (!enviroment.Equals("PRODUCAO")) email = "";
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    Servicos.Email serEmail = new Servicos.Email(unidadeTrabalho);
                    serEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, email.Trim(), "", "", assunto, mensagem, string.Empty, null, "", true, string.Empty, 0, unidadeTrabalho, 0, false);
                }
                catch (Exception ex)
                {
                    Log($"Erro " + ex.Message, 3);
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private string BuscarDescricaoAlerta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> monitoramentosEventos)
        {
            if (monitoramentosEventos != null)
            {
                int total = monitoramentosEventos.Count;
                for (int i = 0; i < total; i++)
                {
                    if (monitoramentosEventos[i].TipoAlerta == tipoAlerta)
                    {
                        return monitoramentosEventos[i].Descricao;
                    }
                }
            }
            return String.Empty;
        }

        #endregion

    }

}
