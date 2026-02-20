using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoNotificacao
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoNotificacao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, null, null) { }

        public CargaJanelaCarregamentoNotificacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _configuracaoJanelaCarregamento = configuracaoJanelaCarregamento;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void EnviarEmailCargaDisponibilizadaParaTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

            // TODO: Processo de envio de e-mail não está preparado para transportador terceiro.
            if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                return;

            Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(_unitOfWork, configuracaoEmail);

            Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
            {
                AssuntoEmail = "Carga ofertada para carregamento.",
                Empresa = cargaJanelaCarregamentoTransportador.Transportador,
                Mensagem = ObterMensagemEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador),
                NotificarSomenteEmailPrincipal = true
            };

            servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmpresa);
        }

        private void EnviarEmailCargaTempoAceiteExpirandoParaTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail)
        {
            Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(_unitOfWork, configuracaoEmail);

            Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
            {
                AssuntoEmail = "Carga pendente de confirmação.",
                Empresa = cargaJanelaCarregamentoTransportador.Transportador,
                Mensagem = ObterMensagemEmailCargaTempoAceiteExpirandoParaTransportador(cargaJanelaCarregamentoTransportador),
                NotificarSomenteEmailPrincipal = true
            };

            servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmpresa);
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        private string ObterCorpoEmail(string mensagem)
        {
            StringBuilder corpoEmail = new StringBuilder();

            corpoEmail.AppendLine(@"<div style=""font-family: Arial;"">");
            corpoEmail.AppendLine($@"    <p style=""margin:0px"">{mensagem}</p>");
            corpoEmail.AppendLine($@"    <p style=""font-size: 12px; margin:0px"">{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");
            corpoEmail.AppendLine(@"    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>");
            corpoEmail.AppendLine("</div>");

            return corpoEmail.ToString();
        }

        private string ObterAssuntoEmailCargaDisponibilizadaParaCotacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.CentroCarregamento?.EnviarEmailAlertaLeilaoParaTransportadorOfertado ?? false)
                return $"A carga {cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador} está disponível para lances no seu portal";

            return "Nova carga disponível para leilão.";
        }

        private string ObterAssuntoEmailParaTransportadorEscolhido(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.CentroCarregamento?.EnviarEmailAlertaLeilaoParaTransportadorOfertado ?? false)
                return $"Você venceu o leilão da carga {cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador}";

            return "Transporte de Carga";
        }

        private string ObterMensagemEmailCargaDisponibilizadaParaCotacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador)
        {
            StringBuilder mensagem = new StringBuilder();

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento;

            if (cargaJanelaCarregamento.CentroCarregamento?.EnviarEmailAlertaLeilaoParaTransportadorOfertado ?? false)
            {
                mensagem
                    .AppendLine($"Olá, {cargaJanelaCarregamentoTransportador.Transportador.Descricao}")
                    .AppendLine("Você está sendo convidado para participar de um leilão de Carga.")
                    .AppendLine("<p>Ressaltamos que os valores inseridos no leilão devem estar no formato Líquido Parcial (sem ICMS ou ISS). O único componente de frete que será acrescido ao valor total é o \"Taxa de Descarga\", caso o Cliente possua Tabela.</p>");
            }
            else
            {
                string Origens = $" Origem: {cargaJanelaCarregamento.Carga.DadosSumarizados.Origens} ";
                string Destinos = $" Destino: {cargaJanelaCarregamento.Carga.DadosSumarizados.Destinos} ";

                mensagem
                    .AppendLine($"Olá Transportador")
                    .AppendLine()
                    .AppendLine($"A carga {cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador} ")
                    .AppendLine($"{Origens}")
                    .AppendLine($"{Destinos}")
                    .AppendLine(" Foi disponibilizada em seu portal, se desejar indique o interesse nela.")
                    .AppendLine();

                if (!string.IsNullOrWhiteSpace(cargaJanelaCarregamento.ObservacaoTransportador))
                    mensagem
                        .AppendLine($"<b>Observação:</b> {cargaJanelaCarregamento.ObservacaoTransportador}")
                        .AppendLine();
            }

            return mensagem.ToString();
        }

        private string ObterMensagemEmailCargaDisponibilizadaParaTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador)
        {
            if (!string.IsNullOrWhiteSpace(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.TipoOperacao?.EmailDisponibilidadeCarga))
                return cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.TipoOperacao.EmailDisponibilidadeCarga
                    .Replace("#CNPJCliente", cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Pedidos?.FirstOrDefault()?.ObterDestinatario()?.CPF_CNPJ_Formatado ?? "")
                    .Replace("#Cliente", cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Pedidos?.FirstOrDefault()?.ObterDestinatario()?.Nome ?? "")
                    .Replace("#CNPJTransportador", cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Empresa?.CNPJ_Formatado ?? "")
                    .Replace("#NumeroCarga", cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador)
                    .Replace("#NumeroPedido", cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Pedidos?.FirstOrDefault()?.Pedido?.NumeroPedidoEmbarcador ?? "")
                    .Replace("#DataHoraColeta", cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"))
                    .Replace("#DataHoraPrevisaoEntrega", cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Pedidos?.FirstOrDefault()?.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "")
                    .Replace("#CNPJEmitente", cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Filial?.CNPJ_Formatado ?? "")
                    .Replace("#Emitente", cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Filial?.Descricao ?? "");

            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repositorioAgendamentoColeta.BuscarPorCarga(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo);
            string mensagemAgendamento = (agendamento?.DataColeta != null) ? $", com coleta prevista para o dia {agendamento.DataColeta.Value.ToString("dd/MM/yyyy")} as {agendamento.DataColeta.Value.ToString("HH:mm")}," : "";
            StringBuilder mensagemBase = new StringBuilder();

            mensagemBase
                .AppendLine($"Olá ({cargaJanelaCarregamentoTransportador.Transportador.CNPJ_Formatado}) {cargaJanelaCarregamentoTransportador.Transportador.Descricao},")
                .AppendLine()
                .AppendLine($"A carga {cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador}{mensagemAgendamento} foi ofertada para transporte.")
                .AppendLine($"O carregamento ocorrerá em {cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy")} as {cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.InicioCarregamento.ToString("HH:mm")}.", !cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Excedente)
                .AppendLine("Favor informar os dados de transporte.", cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao)
                .AppendLine($"Favor confirmar até {cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga?.ToString("dd/MM/yyyy")} as {cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga?.ToString("HH:mm")} e informar os dados de transporte.", cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgAceite)
                .AppendLine();

            if (!string.IsNullOrWhiteSpace(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.ObservacaoTransportador))
                mensagemBase
                    .AppendLine($"<b>Observação:</b> {cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.ObservacaoTransportador}")
                    .AppendLine();

            return mensagemBase.ToString();
        }

        private string ObterMensagemEmailCargaTempoAceiteExpirandoParaTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador)
        {
            StringBuilder mensagemBase = new StringBuilder();

            mensagemBase
                .AppendLine($"Olá ({cargaJanelaCarregamentoTransportador.Transportador.CNPJ_Formatado}) {cargaJanelaCarregamentoTransportador.Transportador.Descricao},")
                .AppendLine()
                .AppendLine($"O tempo para confirmação da carga {cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador} está próximo de encerrar.")
                .AppendLine($"Favor confirmar até {cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga?.ToString("dd/MM/yyyy")} as {cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga?.ToString("HH:mm")} e informar os dados de transporte.")
                .AppendLine();

            if (!string.IsNullOrWhiteSpace(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.ObservacaoTransportador))
                mensagemBase
                    .AppendLine($"<b>Observação:</b> {cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.ObservacaoTransportador}")
                    .AppendLine();

            return mensagemBase.ToString();
        }

        private string ObterMensagemEmailParaTransportadorEscolhido(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.CentroCarregamento?.EnviarEmailAlertaLeilaoParaTransportadorOfertado ?? false)
            {
                StringBuilder mensagem = new StringBuilder()
                    .AppendLine("Parabéns, Fornecedor!")
                    .AppendLine($"Seu lance foi o ganhador e você será o responsável pela carga {cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador} ofertada nesse leilão")
                    .AppendLine("Atente-se aos prazos e especificações do carregamento.")
                    .Append("Agradecemos a parceria.");

                return mensagem.ToString();
            }

            return $"Você foi escolhido para transportar a carga {cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador}. Por favor, informe o veículo e o motorista para a carga.";
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void EnviarEmailCargaDisponibilizadaParaTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            bool enviarEmailParaTransportador = (configuracaoEmbarcador.NotificarCargaAgConfirmacaoTransportador || (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento?.EnviarEmailParaTransportadorAoDisponibilizarCarga ?? false));

#if DEBUG
            enviarEmailParaTransportador = false;
#endif

            if (!enviarEmailParaTransportador)
                return;

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (configuracaoEmail == null)
                return;

            EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador, configuracaoEmail);
        }

        public void EnviarEmailCargaDisponibilizadaParaTransportadoresPorPrazoEsgotado()
        {
            Repositorio.Embarcador.Logistica.PrazoSituacaoCarga repositorioPrazoSituacaoCarga = new Repositorio.Embarcador.Logistica.PrazoSituacaoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga prazoSituacaoCargaAguardandoConfirmacaoTransportador = repositorioPrazoSituacaoCarga.BuscarPorSituacao(SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador);

            if (!(prazoSituacaoCargaAguardandoConfirmacaoTransportador?.NotificarTransportadorPorEmailAoEsgotarPrazo ?? false))
                return;

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (configuracaoEmail == null)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPendenteEnviarEmailPorPrazoEsgotado(prazoSituacaoCargaAguardandoConfirmacaoTransportador.Tempo, limiteRegistros: 5);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
            {
                EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador, configuracaoEmail);

                cargaJanelaCarregamentoTransportador.EmailEnviado = true;

                repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
            }
        }

        public void EnviarEmailCargaLiberadaParaCotacaoParaTranportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaTransportador = null)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (configuracaoEmail == null || cargaJanelaCarregamento == null)
                return;

            string stringConexao = _unitOfWork.StringConexao;

            Task task = new Task(() =>
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(unitOfWork, configuracaoEmail);
                    Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listacargaJanelaCarregamentoTransportadores = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaJanelaCarregamentoDisponivel(cargaJanelaCarregamento.Codigo);

                    if (!(listacargaJanelaCarregamentoTransportadores.Count > 0) && listaTransportador != null)
                        listacargaJanelaCarregamentoTransportadores.AddRange(listaTransportador);

                    try
                    {
                        if (listacargaJanelaCarregamentoTransportadores.Count > 0)
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa> notificacoes = new List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa>();

                            foreach (var cargaJanela in listacargaJanelaCarregamentoTransportadores)
                            {
                                notificacoes.Add(new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                                {
                                    AssuntoEmail = ObterAssuntoEmailCargaDisponibilizadaParaCotacao(cargaJanela.CargaJanelaCarregamento),
                                    Mensagem = ObterMensagemEmailCargaDisponibilizadaParaCotacao(cargaJanela),
                                    Empresa = cargaJanela.Transportador
                                });
                            }

                            servicoNotificacaoEmpresa.EnviarEmailCotacaoDisponivel(notificacoes);
                        }
                    }
                    catch (TaskCanceledException excecao)
                    {
                        Servicos.Log.TratarErro(excecao);
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(string.Concat("Falha no envio do e-mail para transportador carga em leilao carga : ", cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador, " - ", excecao));
                    }
                }
            });

            task.Start();
        }


        public void EnviarEmailCargaSemTransportadorEscolhido(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (!(cargaJanelaCarregamento.CentroCarregamento?.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente ?? false) || (cargaJanelaCarregamento.CentroCarregamento.Emails.Count == 0))
                return;

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork).BuscarEmailEnviaDocumentoAtivo();

            if (configuracaoEmail == null)
                return;

            StringBuilder mensagem = new StringBuilder()
                .AppendLine("Olá.")
                .AppendLine($"<p>O Leilão da Carga: {cargaJanelaCarregamento.Carga?.CodigoCargaEmbarcador} contendo a(s) Origem(ns): {cargaJanelaCarregamento.Carga?.DadosSumarizados?.Origens}, Destino(s): {cargaJanelaCarregamento.Carga?.DadosSumarizados?.Destinos} e a Filial: {cargaJanelaCarregamento.Carga?.Filial?.Descricao}, foi finalizado e não foi possível definir um Vencedor automaticamente.</p>");

            string assunto = $"O Leilão da Carga: {cargaJanelaCarregamento.Carga?.CodigoCargaEmbarcador} foi finalizado sem um vencedor";
            string corpoEmail = ObterCorpoEmail(mensagem.ToString());
            List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>();

            foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail email in cargaJanelaCarregamento.CentroCarregamento.Emails)
            {
                if (!Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, email.Email, new string[] { }, new string[] { }, assunto, corpoEmail.ToString(), configuracaoEmail.Smtp, out string erro, configuracaoEmail.DisplayEmail, anexos, "", false, "", configuracaoEmail.PortaSmtp, _unitOfWork))
                    Log.TratarErro($"Falha ao enviar e-mail de vencedor de leilão não definido: {erro}");
            }
        }

        public void EnviarEmailCargaTempoAceiteExpirandoParaTransportadores()
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            List<int> listaCodigoCargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarCodigosPorTempoAceiteExpirandoParaNotificarPorEmail(limiteRegistros: 5);

            if (listaCodigoCargaJanelaCarregamentoTransportador.Count == 0)
                return;

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (configuracaoEmail == null)
                return;

            foreach (int codigoCargaJanelaCarregamentoTransportador in listaCodigoCargaJanelaCarregamentoTransportador)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCodigo(codigoCargaJanelaCarregamentoTransportador);

                    EnviarEmailCargaTempoAceiteExpirandoParaTransportador(cargaJanelaCarregamentoTransportador, configuracaoEmail);

                    cargaJanelaCarregamentoTransportador.EmailTempoAceiteExpirandoEnviado = true;

                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                }
            }
        }

        public void EnviarEmailParaTransportadorEscolhido(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador)
        {
            Servicos.Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Servicos.Embarcador.Notificacao.NotificacaoEmpresa(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmailEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
            {
                AssuntoEmail = ObterAssuntoEmailParaTransportadorEscolhido(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento),
                Empresa = cargaJanelaCarregamentoTransportador.Transportador,
                Mensagem = ObterMensagemEmailParaTransportadorEscolhido(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento)
            };

            servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmailEmpresa);
        }

        public void NotificarAlteracaoCotacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, string mensagemNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((cargaJanelaCarregamento.CentroCarregamento.UsuariosNotificacao?.Count ?? 0) <= 0)
                return;

            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
            List<Dominio.Entidades.Usuario> usuarios = cargaJanelaCarregamento.CentroCarregamento.UsuariosNotificacao.Distinct().ToList();

            foreach (Dominio.Entidades.Usuario usuario in usuarios)
                servicoNotificacao.GerarNotificacao(usuario, cargaJanelaCarregamento.Codigo, "Logistica/JanelaCarregamento", mensagemNotificacao, IconesNotificacao.janelaMarcouInteresse, TipoNotificacao.janelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, _unitOfWork);
        }

        public void NotificarCotacaoComTransportadorEscolhido(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Notificacao.Notificacao servicoNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware, adminStringConexao: string.Empty);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, null);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoViagem servicoCargaJanelaCarregamentoViagem = new CargaJanelaCarregamentoViagem(_unitOfWork, tipoServicoMultisoftware);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);


            List<int> codigosCargaJanelaCarregamento = cargasJanelaCarregamentoTransportador.Select(o => o.CargaJanelaCarregamento.Codigo).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> todosTransportadoresInteressados = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargasESituacao(codigosCargaJanelaCarregamento, SituacaoCargaJanelaCarregamentoTransportador.ComInteresse);
            List<Dominio.Entidades.Usuario> usuariosNotificacao = cargasJanelaCarregamentoTransportador.Where(o => o.UsuarioResponsavel != null).Select(o => o.UsuarioResponsavel).Distinct().ToList();
            string numeroCarga = servicoCarga.ObterNumeroCarga(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga, configuracaoEmbarcador);
            string mensagemTransportadorEscolhido = Localization.Resources.Cargas.Carga.VoceNaoFoiEscolhidoParaTransportarCarga + numeroCarga;
            string mensagemUsuarioTransportadorEscolhido = Localization.Resources.Cargas.Carga.VoceFoiEscolhidoParaTransportarCarga + numeroCarga + Localization.Resources.Cargas.Carga.PorFavorInformeVeiculoMotoristaParaCarga;
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao existeTipoIntegracaLeilaoManual = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverLeilaoManual);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador interesseTransportador in todosTransportadoresInteressados)
            {
                if ((interesseTransportador.Transportador.Codigo == cargaJanelaCarregamentoTransportadorReferencia.Transportador.Codigo) || (interesseTransportador.CargaJanelaCarregamento.Carga.CargaAgrupamento != null))
                    continue;

                servicoNotificacao.GerarNotificacao(interesseTransportador.UsuarioResponsavel, cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Codigo, "Logistica/JanelaCarregamentoTransportador", mensagemTransportadorEscolhido, IconesNotificacao.janelaTransportadorEscolhido, TipoNotificacao.janelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, _unitOfWork);
            }

            foreach (Dominio.Entidades.Usuario notificar in usuariosNotificacao)
                servicoNotificacao.GerarNotificacao(notificar, cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Codigo, "Logistica/JanelaCarregamentoTransportador", mensagemUsuarioTransportadorEscolhido, IconesNotificacao.janelaTransportadorEscolhido, TipoNotificacao.janelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, _unitOfWork);

            servicoCargaJanelaCarregamentoViagem.GerarIntegracaoVencedorLeilao(cargaJanelaCarregamentoTransportadorReferencia);

            if (existeTipoIntegracaLeilaoManual != null)
                Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaDadosTransporteIntegracao(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga,existeTipoIntegracaLeilaoManual,_unitOfWork,true,false);

            if (cargaJanelaCarregamentoTransportadorReferencia != null)
                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailParaTransportadorEscolhido(cargaJanelaCarregamentoTransportadorReferencia);
        }

        #endregion Métodos Públicos
    }
}
