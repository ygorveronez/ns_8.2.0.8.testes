using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Logistica
{
    public class AgendamentoColeta
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos

        #region Construtores

        public AgendamentoColeta(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AgendamentoColeta(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void EnviarEmailAgendamentoColeta(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, string email)
        {
            Servicos.Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Servicos.Embarcador.Notificacao.NotificacaoEmpresa(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmailEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
            {
                AssuntoEmail = "Novo agendamento de coleta",
                CabecalhoMensagem = "Novo agendamento de coleta",
                Empresa = agendamentoColeta.Transportador,
                Mensagem = ObterEmailAgendamentoColeta(email, agendamentoColeta)
            };

            servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmailEmpresa);
        }

        public void EnviarEmailDesagendamento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, List<string> emails, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string assunto = $"O agendamento da carga  {agendamentoColeta.Carga.CodigoCargaEmbarcador} foi Desagendado";
            string mensagem = ObterCorpoEmailDesagendamentoCarga(agendamentoColeta, cliente);

            EnviarEmailAgendamento(assunto, mensagem, emails);
        }

        public void EnviarEmailCancelamentoAgendamentoPedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido pedidoRemovido, List<string> emails, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string assunto = ObterRazaoSocial() + "Alteração no agendamento - Pedido removido";
            string mensagem = ObterCorpoEmailCancelamentoAgendamentoPedido(pedidoRemovido, cliente);

            EnviarEmailAgendamento(assunto, mensagem, emails);
        }

        public void EnviarEmailAdicaoAgendamentoPedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido pedidoAdicionado, List<string> emails, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string assunto = ObterRazaoSocial() + "Alteração no agendamento - Pedido adicionado";
            string mensagem = ObterCorpoEmailAdicaoAgendamentoPedido(pedidoAdicionado, cliente);

            EnviarEmailAgendamento(assunto, mensagem, emails);
        }

        public void EnviarEmailAlteracaoHorario(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, DateTime novoHorario, List<string> emails, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string assunto = ObterRazaoSocial() + "Alteração no horário do agendamento";
            string mensagem = ObterCorpoEmailAlteracaoHorario(agendamentoColeta, novoHorario, cliente);

            EnviarEmailAgendamento(assunto, mensagem, emails);
        }

        public void EnviarEmailNaoComparecimento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, List<string> emails, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string assunto = ObterRazaoSocial() + "Não Comparecimento";
            string mensagem = ObterCorpoEmailNaoComparecimento(agendamentoColeta, cliente);

            EnviarEmailAgendamento(assunto, mensagem, emails);
        }

        public void EnviarEmailCancelamentoAgendamento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, string motivoCancelamento, List<string> emails, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string assunto = ObterRazaoSocial() + "Cancelamento de Agenda";
            string mensagem = ObterCorpoEmailCancelamentoAgendamento(agendamentoColeta, motivoCancelamento, cliente);

            EnviarEmailAgendamento(assunto, mensagem, emails);
        }

        public void EnviarEmailConfirmacaoAgendamentoColeta(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, List<string> emails, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware clienteMultisoftware)
        {
            string assunto = ObterRazaoSocial() + "Agendamento Confirmado";
            string mensagem = ObterCorpoEmailConfirmacaoAgendamentoColeta(agendamentoColeta, clienteMultisoftware);

            EnviarEmailAgendamento(assunto, mensagem, emails);
        }

        public void EnviarEmailConfirmacaoAgendamentoEntrega(Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega, List<string> emails, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware clienteMultisoftware)
        {

            string assunto = $"O agendamento  {agendamentoEntrega.Carga.CodigoCargaEmbarcador} foi confirmado";
            string mensagem = ObterCorpoEmailConfirmacaoAgendamentoEntrega(agendamentoEntrega, clienteMultisoftware);

            EnviarEmailAgendamento(assunto, mensagem, emails);
        }

        public void EnviarEmailAgendamentoAdicionadoParaRemetentePedido(List<string> emails, string numerosPedidos, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string assunto = ObterRazaoSocial() + "Agendamento adicionado";
            string mensagem = ObterCorpoEmailConfirmacaoAgendamentoAdicionado(numerosPedidos, cliente);

            EnviarEmailAgendamento(assunto, mensagem, emails);
        }

        public bool IsForcarEtapaNFe(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta)
        {
            if (agendamentoColeta == null)
                return false;

            if (!(agendamentoColeta.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.ObrigarInformarCTePortalFornecedor ?? false))
                return false;

            if (agendamentoColeta.Carga == null)
                return false;

            if (agendamentoColeta.Carga.ExigeNotaFiscalParaCalcularFrete)
                return agendamentoColeta.Carga.SituacaoCarga == SituacaoCarga.Nova || agendamentoColeta.Carga.SituacaoCarga == SituacaoCarga.AgTransportador;

            return agendamentoColeta.Carga.SituacaoCarga == SituacaoCarga.Nova || agendamentoColeta.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete || agendamentoColeta.Carga.SituacaoCarga == SituacaoCarga.AgTransportador;
        }

        public byte[] ResumoAgendamentoColeta(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            return ReportRequest.WithType(ReportType.ResumoAgendamentoColeta)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoAgendamento", agendamento.Codigo.ToString())
                .AddExtraData("CodigoCargaJanelaDescarregamento", cargaJanelaDescarregamento.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public byte[] ResumoAgendamentoColetaSams(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            return ReportRequest.WithType(ReportType.ResumoAgendamentoColetaSams)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoAgendamento", agendamento.Codigo.ToString())
                .AddExtraData("CodigoCargaJanelaDescarregamento", cargaJanelaDescarregamento.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public string ObterSenhaAgendamentoColeta(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoPedidos = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork).BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);

            if (string.IsNullOrWhiteSpace(agendamentoColeta.Senha) && !(cargaJanelaDescarregamento?.CentroDescarregamento?.BuscarSenhaViaIntegracao ?? false) && !configuracaoAgendamentoColeta.GerarAutomaticamenteSenhaPedidosAgendas)
                return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();

            if (cargaJanelaDescarregamento != null)
            {
                if (!string.IsNullOrWhiteSpace(agendamentoColeta.Senha) || agendamentoPedidos.Count == 0)
                    return agendamentoColeta.Senha;

                if ((cargaJanelaDescarregamento.CentroDescarregamento?.BuscarSenhaViaIntegracao ?? false) && ValidarConfirmarAgendamentoAutomaticamente(cargaJanelaDescarregamento))
                {
                    Integracao.SAD.IntegracaoSAD servicoIntegracaoSad = new Integracao.SAD.IntegracaoSAD(_unitOfWork, _tipoServicoMultisoftware);

                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta retorno = servicoIntegracaoSad.ObterSenhaAgendamento(agendamentoPedidos);

                        if (!retorno.Pedidos.FirstOrDefault().Sucesso)
                            agendamentoColeta.ErroBuscarSenhaAutomaticamente = retorno.Mensagem.Substring(0, retorno.Mensagem.Length > 150 ? 150 : retorno.Mensagem.Length);
                        else
                            return retorno.Pedidos.FirstOrDefault().Senha;
                    }
                    catch (System.Exception excecao)
                    {
                        agendamentoColeta.ErroBuscarSenhaAutomaticamente = excecao.Message.Substring(0, excecao.Message.Length > 150 ? 150 : excecao.Message.Length);
                    }

                    return "";
                }
            }

            if (configuracaoAgendamentoColeta.GerarAutomaticamenteSenhaPedidosAgendas)
            {
                int senhaSequencial = repositorioAgendamentoColeta.ObterProximaSenhaSequencial();

                agendamentoColeta.SenhaSequencial = senhaSequencial;

                return $"M{senhaSequencial}";
            }

            return "";
        }

        public bool ValidarConfirmarAgendamentoAutomaticamente(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            if (!(cargaJanelaDescarregamento.CentroDescarregamento?.AprovarAutomaticamenteDescargaComHorarioDisponivel ?? false))
                return false;

            if (cargaJanelaDescarregamento.Excedente || (cargaJanelaDescarregamento.Situacao != SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento))
                return false;

            Repositorio.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento repositorioAprovacaoAutomatica = new Repositorio.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento> horariosAprovacaoAutomatica = repositorioAprovacaoAutomatica.BuscarPorCentroDescarregamento(cargaJanelaDescarregamento.CentroDescarregamento.Codigo);

            if (horariosAprovacaoAutomatica.Count == 0)
                return true;

            DateTime dataDescarga = cargaJanelaDescarregamento.InicioDescarregamento.Date;

            return horariosAprovacaoAutomatica.Any(obj => obj.DataInicial <= dataDescarga && obj.DataFinal >= dataDescarga);
        }

        public void AtualizarDataEntregaPorCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if (!cargaPedido.Pedido.DataInicialColeta.HasValue)
                return;

            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaPedido.Carga.Codigo);

            if (agendamentoColeta == null)
                return;

            agendamentoColeta.DataEntrega = cargaPedido.Pedido.DataInicialColeta;
            repositorioAgendamentoColeta.Atualizar(agendamentoColeta);
        }

        public void EnviarEmailAgendamento(string assunto, string mensagem, List<string> emails)
        {
            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                string de = configuracaoEmail.Email;
                string usuario = configuracaoEmail.Email;
                string senha = configuracaoEmail.Senha;
                string[] copiaOcultaPara = new string[] { };
                string[] copiaPara = new string[] { };
                string servidorSMTP = configuracaoEmail.Smtp;
                List<System.Net.Mail.Attachment> anexos = null;
                string assinatura = "";
                bool possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                string responderPara = "";
                int porta = configuracaoEmail.PortaSmtp;
                StringBuilder corpoMensagem = new StringBuilder();

                corpoMensagem.AppendLine(@"<div style=""font-family: Arial;"">");
                corpoMensagem.AppendLine($@"    <p style=""margin:0px"">{mensagem}</p>");
                corpoMensagem.AppendLine($@"    <p style=""font-size: 12px; margin:0px"">{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");
                corpoMensagem.AppendLine("    <p></p>");
                corpoMensagem.AppendLine(@"    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>");
                corpoMensagem.AppendLine("</div>");

                if (emails.Count == 0)
                    throw new ServicoException("Não foi possível encontrar e-mails para o envio.");

                foreach (string email in emails)
                {

                    if (!Servicos.Email.EnviarEmail(de, usuario, senha, email, copiaOcultaPara, copiaPara, assunto, corpoMensagem.ToString(), servidorSMTP, out string erro, configuracaoEmail.DisplayEmail, anexos, assinatura, possuiSSL, responderPara, porta, _unitOfWork))
                        Log.TratarErro($"Falha ao enviar o e-mail do chamado: {erro}");
                }

            }
            catch (System.Exception excecao)
            {
                Log.TratarErro("Falha ao enviar o e-mail do chamado: " + excecao);
            }
        }

        public void SubtrairSaldoVolumesPendentesPedidos(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork, bool atualizarSituacaoPedido = false, bool apenasAtualizarPedido = false)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto repositorioAgendamentoColetaPedidoProduto = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoPedidos = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);

                if (agendamentoPedidos?.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listaAgendamentoColetaPedidoProdutos = repositorioAgendamentoColetaPedidoProduto.BuscarPorPedidoAgendamentoColetaAgendado(agendamentoPedidos.Select(x => x.Pedido.Codigo).ToList());

                    foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido pedidoAgendamento in agendamentoPedidos)
                    {
                        if (apenasAtualizarPedido)
                        {
                            pedidoAgendamento.Pedido.SituacaoPedido = SituacaoPedido.Aberto;
                            repositorioPedido.Atualizar(pedidoAgendamento.Pedido);
                            continue;
                        }

                        var quantidadeSaldoVolumesRestantePedido = pedidoAgendamento.Pedido.QtVolumes;

                        List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto = listaAgendamentoColetaPedidoProdutos.Where(x => x.AgendamentoColetaPedido.Pedido.Codigo == pedidoAgendamento.Pedido.Codigo).ToList();

                        if (listAgendamentoColetaPedidoProduto.Count > 0)
                            quantidadeSaldoVolumesRestantePedido -= listAgendamentoColetaPedidoProduto.Sum(x => x.Quantidade);
                        else if (pedidoAgendamento.Pedido.SaldoVolumesRestante > 0)
                            quantidadeSaldoVolumesRestantePedido -= pedidoAgendamento.VolumesEnviar;

                        if (quantidadeSaldoVolumesRestantePedido < 0)
                            throw new ServicoException("A quantidade de volumes alterados não pode ser maior que o saldo de volume do pedido.");

                        if (quantidadeSaldoVolumesRestantePedido == 0)
                            pedidoAgendamento.Pedido.PedidoTotalmenteCarregado = true;
                        else if (atualizarSituacaoPedido)
                        {
                            pedidoAgendamento.Pedido.PedidoTotalmenteCarregado = false;
                            pedidoAgendamento.Pedido.SituacaoPedido = SituacaoPedido.Aberto;
                        }

                        pedidoAgendamento.Pedido.SaldoVolumesRestante = quantidadeSaldoVolumesRestantePedido;
                        repositorioPedido.Atualizar(pedidoAgendamento.Pedido);
                    }
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao.Message);
                throw new ServicoException(excecao.Message);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao.Message);
                throw new ServicoException("Erro ao atualizar saldo do pedido.");
            }
        }

        public void DuplicarAgendamentoColeta(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamentoNovo, Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoAnterior = repositorioAgendamentoColeta.BuscarPorCarga(cargaAntiga.Codigo);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoCargaNova = repositorioAgendamentoColeta.BuscarPorCarga(cargaJanelaDescarregamentoNovo.Carga.Codigo);
            bool inserir = false;

            if (agendamentoCargaNova == null)
            {
                agendamentoCargaNova = agendamentoAnterior.Clonar<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
                inserir = true;
            }

            if (agendamentoAnterior != null)
            {
                agendamentoCargaNova.AgendamentoPai = false;
                agendamentoAnterior.AgendamentoPai = true;
                agendamentoCargaNova.Carga = cargaJanelaDescarregamentoNovo.Carga;
                agendamentoCargaNova.CodigoControle = cargaJanelaDescarregamentoNovo.Carga.Codigo;
                agendamentoCargaNova.Destinatario = cargaJanelaDescarregamentoNovo.CentroDescarregamento.Destinatario;
                agendamentoCargaNova.Pedidos = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();

                if (inserir)
                    repositorioAgendamentoColeta.Inserir(agendamentoCargaNova);
                else
                    repositorioAgendamentoColeta.Atualizar(agendamentoCargaNova);

                repositorioAgendamentoColeta.Atualizar(agendamentoAnterior);
            }
            else
                throw new ControllerException("Não foi possível gerar o novo agendamento de coleta para carga de Redespacho.");
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private string ObterEmailAgendamentoColeta(string email, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta)
        {
            string emailConfigurado = email.Replace("#CNPJFornecedor", agendamentoColeta.Remetente.CPF_CNPJ_Formatado)
                .Replace("#Fornecedor", agendamentoColeta.Remetente.NomeFantasia)
                .Replace("#Transportador", agendamentoColeta.Transportador.NomeFantasia)
                .Replace("#CNPJTransportador", agendamentoColeta.Transportador.CNPJ_Formatado)
                .Replace("#NumeroCarga", agendamentoColeta.Carga?.CodigoCargaEmbarcador ?? string.Empty)
                .Replace("#NumeroPedido", agendamentoColeta.Pedido?.NumeroPedidoEmbarcador ?? string.Empty)
                .Replace("#DataColeta", agendamentoColeta.DataColeta?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty)
                .Replace("#EmailSolicitante", agendamentoColeta.EmailSolicitante);

            return emailConfigurado;
        }

        private string ObterCorpoEmailDesagendamentoCarga(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string mensagem = $"<b>{cliente.RazaoSocial} informa:</b>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"Prezado Fornecedor, ";
            mensagem += $"informamos que a carga {agendamentoColeta.Carga.CodigoCargaEmbarcador} foi Desagendada.";
            mensagem += $"</br>";

            return mensagem;
        }

        private string ObterCorpoEmailCancelamentoAgendamentoPedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido pedidoRemovido, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string mensagem;

            string senha = !string.IsNullOrWhiteSpace(pedidoRemovido.AgendamentoColeta.Senha) ? $"senha {pedidoRemovido.AgendamentoColeta.Senha}" : string.Empty;

            mensagem = $"<b>{cliente.RazaoSocial} informa:</b>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem = $"Prezado Fornecedor, ";
            mensagem += $"informamos que os pedidos em relação ao seu agendamento {senha} sofreu alterações.</br></br>";
            mensagem += ObterStringParaMaisDetalhes();
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"MOTIVO: Pedido Removido.";

            return mensagem;
        }

        private string ObterCorpoEmailAdicaoAgendamentoPedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido pedidoAdicionado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string mensagem;

            string senha = !string.IsNullOrWhiteSpace(pedidoAdicionado.AgendamentoColeta.Senha) ? $"senha {pedidoAdicionado.AgendamentoColeta.Senha}" : string.Empty;

            mensagem = $"<b>{cliente.RazaoSocial} informa:</b>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem = $"Prezado Fornecedor, ";
            mensagem += $"informamos que os pedidos em relação ao seu agendamento {senha} sofreu alterações.</br></br>";
            mensagem += ObterStringParaMaisDetalhes();
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"MOTIVO: Pedido Adicionado.";

            return mensagem;
        }

        private string ObterCorpoEmailAlteracaoHorario(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, DateTime novoHorario, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string mensagem;

            string senha = !string.IsNullOrWhiteSpace(agendamentoColeta.Senha) ? $"senha {agendamentoColeta.Senha}" : string.Empty;

            mensagem = $"<b>{cliente.RazaoSocial} informa:</b>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem = $"Prezado Fornecedor, ";
            mensagem += $"informamos que os pedidos em relação ao seu agendamento {senha} sofreu alterações.</br></br>";
            mensagem += ObterStringParaMaisDetalhes();
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"MOTIVO: Alteração de Horário - Nova data: {novoHorario.ToString("dd/MM/yyyy HH:mm")}";

            return mensagem;
        }

        private string ObterCorpoEmailNaoComparecimento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string senha = !string.IsNullOrWhiteSpace(agendamentoColeta.Senha) ? $"senha {agendamentoColeta.Senha}" : string.Empty;

            string mensagem = $"<b>{cliente.RazaoSocial} informa:</b>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"Prezado Fornecedor, ";
            mensagem += $"informamos que o pedido em relação ao seu agendamento {senha} foi marcado com <b>não comparecimento</b>.</br></br>";
            mensagem += ObterStringParaMaisDetalhes();
            mensagem += $"</br>";

            return mensagem;
        }

        private string ObterCorpoEmailCancelamentoAgendamento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, string motivo, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string senha = !string.IsNullOrWhiteSpace(agendamentoColeta.Senha) ? $"senha {agendamentoColeta.Senha}" : string.Empty;

            string mensagem = $"<b>{cliente.RazaoSocial} informa:</b>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"Prezado Fornecedor, ";
            mensagem += $"informamos que os pedidos em relação ao seu agendamento {senha} foi cancelado.</br></br>";
            mensagem += ObterStringParaMaisDetalhes();
            mensagem += $"</br>";

            if (!string.IsNullOrEmpty(motivo))
            {
                mensagem += $"</br>";
                mensagem += $"MOTIVO DO CANCELAMENTO: {motivo}";
                mensagem += $"</br>";
            }

            return mensagem;
        }

        private string ObterCorpoEmailConfirmacaoAgendamentoColeta(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware clienteMultisoftware)
        {
            string mensagem = string.Empty;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repCargaJanelaDescarregamento.BuscarPorCarga(agendamentoColeta.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> listaPedidos = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);

            string senha = !string.IsNullOrWhiteSpace(agendamentoColeta.Senha) ? $"</br>Senha: {agendamentoColeta.Senha}</br>" : string.Empty;

            string tipoCarga = agendamentoColeta.TipoCarga?.Descricao;
            if (string.IsNullOrEmpty(tipoCarga) && listaPedidos.Any())
                tipoCarga = string.Join(", ", listaPedidos.Select(o => o.Pedido?.TipoDeCarga?.Descricao)
                                                          .Where(descricao => !string.IsNullOrEmpty(descricao))
                                                          .Distinct());

            if (!string.IsNullOrEmpty(clienteMultisoftware?.RazaoSocial))
            {
                mensagem += $"<b>{clienteMultisoftware.RazaoSocial} informa:</b>";
            }
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"Prezado Fornecedor, ";
            mensagem += $"informamos que o agendamento {agendamentoColeta.Carga.CodigoCargaEmbarcador} foi confirmado.";
            mensagem += $"</br></br>";
            mensagem += $"<table style='width:60%; border-collapse: collapse; border-spacing: 0;'>";
            mensagem += $"<tr>";
            mensagem += $"<th style='text-align: left;'>Número pedido:</th>";
            mensagem += $"<th style='text-align: left;'>Fornecedor:</th>";
            mensagem += $"<th>  </th>";
            mensagem += $"</tr>";

            foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido pedido in listaPedidos)
            {
                mensagem += $"<tr>";
                mensagem += $"<td style='text-align: left;'>{pedido.Pedido.NumeroPedidoEmbarcador}</td>";
                mensagem += $"<td style='text-align: left;'>{pedido.Pedido.Remetente.Nome}</td>";
                mensagem += $"<td>  </td>";
                mensagem += $"</tr>";
            }

            mensagem += $"</table>";
            mensagem += $"</br>";
            mensagem += $"Data da agenda: {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm")}";
            mensagem += senha;
            mensagem += $"Tipo de Carga: {tipoCarga}</br>";
            mensagem += $"Status do Agendamento: {agendamentoColeta.DescricaoSituacao}</br>";
            mensagem += $"Centro Descarregamento: {cargaJanelaDescarregamento.CentroDescarregamento.Descricao}</br>";
            mensagem += $"Setor: {repCargaJanelaDescarregamento.ObterSetoresAgendamentoColeta(agendamentoColeta)}</br></br>";
            mensagem += ObterStringParaMaisDetalhes();

            return mensagem;
        }

        private string ObterCorpoEmailConfirmacaoAgendamentoEntrega(Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware clienteMultisoftware)
        {
            string mensagem = string.Empty;

            if (!string.IsNullOrEmpty(clienteMultisoftware?.RazaoSocial))
            {
                mensagem += $"<b>{clienteMultisoftware.RazaoSocial} informa:</b>";
            }

            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"Prezado Fornecedor, ";
            mensagem += $"informamos que o agendamento {agendamentoEntrega.Carga.CodigoCargaEmbarcador} foi Confirmado.";
            mensagem += $"</br>";

            return mensagem;
        }

        private string ObterCorpoEmailConfirmacaoAgendamentoAdicionado(string numerosPedidos, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            string mensagem = $"<b>{cliente.RazaoSocial} informa:</b>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"</br>";
            mensagem += $"Prezado Fornecedor, ";
            mensagem += $"Informamos que o agendamento referente ao pedido {numerosPedidos} foi adicionado.</br></br>";
            mensagem += ObterStringParaMaisDetalhes();

            return mensagem;
        }

        private string ObterStringParaMaisDetalhes()
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware repositorioConfiguracaoClienteMultisoftware = new Repositorio.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware configuracaoClienteMultisoftware = repositorioConfiguracaoClienteMultisoftware.BuscarConfiguracaoPadrao();

            if (configuracaoClienteMultisoftware == null)
                return string.Empty;

            return $"Para mais detalhes, acesse o portal: <a href=\"https://{configuracaoClienteMultisoftware.URLFornecedor}/Login\">https://{configuracaoClienteMultisoftware.URLFornecedor}/Login</a>";
        }

        private string ObterRazaoSocial()
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware repositorioConfiguracaoClienteMultisoftware = new Repositorio.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware configuracaoClienteMultisoftware = repositorioConfiguracaoClienteMultisoftware.BuscarConfiguracaoPadrao();

            if (configuracaoClienteMultisoftware == null)
                return string.Empty;
            return $"{configuracaoClienteMultisoftware.RazaoSocial} - ";
        }

        #endregion Métodos Privados
    }
}
