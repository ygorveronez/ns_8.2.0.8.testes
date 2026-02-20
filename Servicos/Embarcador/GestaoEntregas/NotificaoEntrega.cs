
using RestSharp.Extensions;
using Servicos.Embarcador.Configuracoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.GestaoEntregas
{
    public class NotificaoEntrega
    {
        public static void VerificarEntregasPendentesNotificacao(int codigoClienteAdminMultisoftware, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repositorioOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOCorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTemplateWhatsApp repConfiguracaoTemplateWhatsApp = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTemplateWhatsApp(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOCorrencia.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp> templatesPendentesAprovacao = repConfiguracaoTemplateWhatsApp.BuscarTemplatesPendentesAprovacao();
                Servicos.Embarcador.Integracao.META.Templates.TemplateMensagemWhatsApp svcTemplates = new Integracao.META.Templates.TemplateMensagemWhatsApp(unitOfWork);

                if (templatesPendentesAprovacao.Count() > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp templatePendentesAprovacao in templatesPendentesAprovacao)
                        svcTemplates.AtualizarStatusTemplate(templatePendentesAprovacao);

                }

                string urlAcesso = ObterURLBase(codigoClienteAdminMultisoftware, tipoServicoMultisoftware, unitOfWorkAdminMultisoftware, unitOfWorkAdminMultisoftware.StringConexao, unitOfWork);
                string tokenSMS = configuracao.TokenSMS;
                string cnpjRemetente = configuracao.SenderSMS;

                Servicos.Global.OrquestradorFila orquestrador = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.VerificarEntregasPendentesNotificacao);
                int codigoOcorrencia = 0;
                List<int> ocorrencias = orquestrador.Ordenar((limiteRegistros) => repPedidoOcorrenciaColetaEntrega.BuscarCodigosPendentesEnvio(limiteRegistros));
                for (int i = 0; i < ocorrencias.Count; i++)
                {
                    codigoOcorrencia = ocorrencias[i];
                    try
                    {
                        unitOfWork.Start();
                        bool enviarEmail = false;

                        Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrencia = repPedidoOcorrenciaColetaEntrega.BuscarPorCodigo(codigoOcorrencia, false);

                        string codigoRastreamento = ObterCodigoRastreamentoPedido(ocorrencia.Pedido, unitOfWork);
                        string urlRastreamento = ObterURLRastreamentoPedido(codigoRastreamento, urlAcesso);

                        if (ocorrencia.PendenteEnvioSMS)
                        {
                            NotificarEntregaPorSMS(ocorrencia, urlRastreamento, tokenSMS, cnpjRemetente, out string msgErro, unitOfWork);

                            ocorrencia.RetornoEnvioSMS = msgErro;
                            ocorrencia.PendenteEnvioSMS = false;
                        }

                        if (ocorrencia.PendenteEnvioEmail)
                        {
                            enviarEmail = true;
                            ocorrencia.RetornoEnvioEmail = "";
                            ocorrencia.PendenteEnvioEmail = false;
                        }

                        if (ocorrencia.PendenteEnvioWhatsApp)
                        {
                            Servicos.Embarcador.Integracao.META.WhatsApp.WhatsApp svcWhatsApp = new Integracao.META.WhatsApp.WhatsApp(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp> templateMensagemWhatsApp = repConfiguracaoTemplateWhatsApp.BuscarPorTipoTemplate(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTemplateWhatsApp.AcomapanhamentoEntrega);

                            if (templateMensagemWhatsApp.Count > 0 && svcWhatsApp.EnviarNotificaçãoWhatsApp(ocorrencia, templateMensagemWhatsApp, unitOfWork, urlRastreamento))
                            {
                                ocorrencia.PendenteEnvioWhatsApp = false;
                                ocorrencia.RetornoEnvioWhatsApp = "";
                            }
                        }

                        repPedidoOcorrenciaColetaEntrega.Atualizar(ocorrencia);
                        unitOfWork.CommitChanges();
                        orquestrador.RegistroLiberadoComSucesso(codigoOcorrencia);

                        if (enviarEmail)
                        {
                            NotificarEntregaPorEmail(ocorrencia, urlRastreamento, out string msgErro, configuracaoOcorrencia, unitOfWork);
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        orquestrador.RegistroComFalha(codigoOcorrencia, ex.Message);
                        unitOfWork.Rollback();
                    }
                }


                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> ocorrenciasEntregas = repositorioOcorrenciaColetaEntrega.BuscarPendentesEnvio(0);
                List<int> codigosCargaEntrega = ocorrenciasEntregas.Select(obj => obj.CargaEntrega.Codigo).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargasEntregaPedido = repositorioCargaEntregaPedido.BuscarPorCargaEntregasFetchRemetente(codigosCargaEntrega);

                for (int i = 0; i < ocorrenciasEntregas.Count; i++)
                {
                    try
                    {
                        unitOfWork.Start();
                        bool enviarEmail = false;

                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrenciaEntrega = ocorrenciasEntregas[i];
                        string codigoRastreamento = ObterCodigoRastreamentoEntrega(ocorrenciaEntrega.CargaEntrega, unitOfWork);
                        string urlRastreamento = ObterURLRastreamentoEntrega(codigoRastreamento, urlAcesso);

                        if (ocorrenciaEntrega.PendenteEnvioEmail)
                        {
                            enviarEmail = true;
                            ocorrenciaEntrega.RetornoEnvioEmail = "";
                            ocorrenciaEntrega.PendenteEnvioEmail = false;
                        }

                        repositorioOcorrenciaColetaEntrega.Atualizar(ocorrenciaEntrega);
                        unitOfWork.CommitChanges();


                        if (enviarEmail)
                            NotificarEntregaPorEmail(ocorrenciaEntrega, (from o in cargasEntregaPedido where o.CargaEntrega.Codigo == ocorrenciaEntrega.CargaEntrega.Codigo select o.CargaPedido.Pedido.Remetente)?.FirstOrDefault(), urlRastreamento, out string msgErro, unitOfWork);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        unitOfWork.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public static bool NotificarEntregaPorEmail(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrenciaEntrega, Dominio.Entidades.Cliente remetente, string urlRastreamento, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Cliente cliente = ocorrenciaEntrega.CargaEntrega.Cliente;

            string evento = ObterDescricaoPortalOcorrencia(ocorrenciaEntrega.TipoDeOcorrencia, ocorrenciaEntrega.CargaEntrega.Carga, cliente, remetente);
            string nomeCliente = cliente.Nome;
            string emailCliente = cliente.Email;
            string subject = ObterAssuntoNotificacao(evento, null);
            string body = ObterBodyNotificaoEntrega(evento, urlRastreamento, unitOfWork);

#if DEBUG
            emailCliente = "";
#endif
            Log.TratarErro($"Enviando email Entrega: {ocorrenciaEntrega.CargaEntrega.Codigo} ", "NotificaoEntrega");

            if (!Servicos.Email.EnviarEmailAutenticado(emailCliente, subject, body, unitOfWork, out msgErro, nomeCliente))
            {
                Log.TratarErro(msgErro, "NotificaoEntrega");
                return false;
            }

            msgErro = "";
            return true;
        }

        public static bool NotificarEntregaPorEmail(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrenciaColeta, string urlRastreamento, out string msgErro, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = ocorrenciaColeta.Pedido;
            Dominio.Entidades.Cliente cliente = pedido.Destinatario;

            string evento = ObterDescricaoPortalOcorrencia(ocorrenciaColeta.TipoDeOcorrencia, ocorrenciaColeta.Carga, cliente, pedido.Remetente);
            string nomeCliente = pedido.Remetente.NomeFantasia;
            string emailCliente = cliente.Email;
            string[] emailsGerenteSupervisorVendedor = ObterEmailVendedor(pedido);
            string subject = ObterAssuntoNotificacao(evento, ocorrenciaColeta.Pedido);
            string body = ObterBodyNotificaoPedido(evento, urlRastreamento, ocorrenciaColeta.Pedido, ocorrenciaColeta.Carga, unitOfWork);

            List<System.Net.Mail.Attachment> anexos = null;
            if (configuracaoOcorrencia != null && configuracaoOcorrencia.EnviarXMLDANFEClienteOcorrenciaPedido && (ocorrenciaColeta.EventoColetaEntrega != null && ocorrenciaColeta.EventoColetaEntrega.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.PedidoFaturado))
            {
                anexos = new List<System.Net.Mail.Attachment>();
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorPedido(pedido.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = (from notas in pedidosXMLNotaFiscal where notas.XMLNotaFiscal != null select notas.XMLNotaFiscal).Distinct().ToList();
                if (pedido.NotasFiscais?.Count > 0)
                    notasFiscais.AddRange(pedido.NotasFiscais.ToList());

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in notasFiscais)
                {
                    if (!string.IsNullOrEmpty(xmlNotaFiscal.XML))
                    {
                        int MesNota = xmlNotaFiscal.DataEmissao.Date.Month;
                        string xml = xmlNotaFiscal.XML;
                        System.IO.MemoryStream streamNota = new System.IO.MemoryStream(System.Text.Encoding.Unicode.GetBytes(xml));
                        System.Net.Mail.Attachment anexoNota = new System.Net.Mail.Attachment(streamNota, $"{xmlNotaFiscal.Chave}.xml");
                        anexos.Add(anexoNota);

                        string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                        string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", "DANFE", MesNota.ToString(), xmlNotaFiscal.Chave + ".pdf");
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                        {
                            System.IO.MemoryStream streamDanfe = new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE));
                            System.Net.Mail.Attachment anexoDanfe = new System.Net.Mail.Attachment(streamDanfe, $"{xmlNotaFiscal.Chave}.pdf");
                            anexos.Add(anexoDanfe);
                        }
                    }
                }
            }


#if DEBUG
            emailCliente = "";
            emailsGerenteSupervisorVendedor = null;
#endif
            if (!Servicos.Email.EnviarEmailAutenticado(emailCliente, subject, body, unitOfWork, out msgErro, nomeCliente, anexos, null, emailsGerenteSupervisorVendedor))
            {
                Log.TratarErro(msgErro, "NotificaoEntrega");
                return false;
            }

            msgErro = "";
            return true;
        }


        public static bool NotificarEntregaPorSMS(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrenciaColeta, string urlRastreamento, string tokenSMS, string sender, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = ocorrenciaColeta.Pedido;
                Dominio.Entidades.Cliente cliente = pedido.Destinatario;

                string evento = ObterDescricaoPortalOcorrencia(ocorrenciaColeta.TipoDeOcorrencia, ocorrenciaColeta.Carga, cliente, pedido.Remetente);
                string numeroCliente = cliente.Celular;
                string texto = ObterSMSNotificao(evento, urlRastreamento);

                SMS srvSMS = new SMS(unitOfWork);
                if (!srvSMS.EnviarSMS(tokenSMS, sender, numeroCliente, texto, unitOfWork, out msgErro))
                {
                    return false;
                }

                msgErro = "";
                return true;
            }
            catch (Exception e)
            {
                Log.TratarErro(e.Message, "NotificaoEntrega");
                msgErro = "Falha ao enviar SMS";
                return false;
            }
        }

        public static string ObterURLRastreamentoPedido(string codigoRastreamento, string urlAcesso)
        {
            if (urlAcesso.Contains("192.168.0.125")) urlAcesso += "/Embarcador";

            string urlRastreamento = $"{urlAcesso}/sua-entrega/{codigoRastreamento}";

            return urlRastreamento;
        }

        public static string ObterURLRastreamentVisualizacaoMonitoramento(int codigoCarga, string urlAcesso)
        {
            if (urlAcesso.Contains("192.168.0.125")) urlAcesso += "/Embarcador";
            string token = Servicos.Criptografia.Criptografar(codigoCarga.ToString(), "46ad3d5c9f7f34e502c5c0caa196e91d98b23bba7e6d8a89b2282a92b957d1fd").UrlEncode();
            string urlRastreamento = $"{urlAcesso}/visualizacao-monitoramento/{token}";

            return urlRastreamento;
        }

        public static string ObterURLRastreamentoEntrega(string codigoRastreamento, string urlAcesso)
        {
            if (urlAcesso.Contains("192.168.0.125")) urlAcesso += "/Embarcador";

            string urlRastreamento = $"{urlAcesso}/rastreio-entrega/{codigoRastreamento}";

            return urlRastreamento;
        }

        public static string ObterCodigoRastreamentoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (!string.IsNullOrWhiteSpace(pedido.CodigoRastreamento))
            {
                return pedido.CodigoRastreamento;
            }

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            bool codigoRastreioJaExistente;
            string codigoRastreamento;
            do
            {
                codigoRastreamento = Guid.NewGuid().ToString().Replace("-", "");
                codigoRastreioJaExistente = repPedido.CodigoRastreamentoJaExistente(codigoRastreamento);
            } while (codigoRastreioJaExistente);

            pedido.CodigoRastreamento = codigoRastreamento;
            repPedido.Atualizar(pedido);

            return codigoRastreamento;
        }

        public static string ObterCodigoRastreamentoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            if (!string.IsNullOrWhiteSpace(cargaEntrega.CodigoRastreio))
                return cargaEntrega.CodigoRastreio;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            bool codigoRastreioJaExistente;
            string codigoRastreamento;
            do
            {
                codigoRastreamento = Guid.NewGuid().ToString().Replace("-", "");
                codigoRastreioJaExistente = repCargaEntrega.CodigoRastreioExiste(codigoRastreamento);
            } while (codigoRastreioJaExistente);

            cargaEntrega.CodigoRastreio = codigoRastreamento;
            repCargaEntrega.Atualizar(cargaEntrega);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

            return codigoRastreamento;
        }

        private static string ObterSMSNotificao(string evento, string urlRastreamento)
        {
            return $@"
{Localization.Resources.GestaoEntregas.Acompanhamento.TemosAtualizacoesDoSeuPedido}\n\r
<strong>{evento}</strong>\n\r
{Localization.Resources.GestaoEntregas.Acompanhamento.VejaMaisDetalhesEm} {urlRastreamento}
";
        }

        private static string ObterBodyNotificaoPedido(string evento, string urlRastreamento, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorPedido(pedido.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = (from notas in pedidosXMLNotaFiscal where notas.XMLNotaFiscal != null select notas.XMLNotaFiscal).Distinct().ToList();
            List<int> numeroNotas = (from nota in notasFiscais select nota.Numero).ToList();

            StringBuilder sb = new StringBuilder();

            sb.Append($"{pedido.Destinatario.Nome} {Localization.Resources.GestaoEntregas.Acompanhamento.TemosAtualizacoesDoSeuPedido}<br>");
            sb.Append($"<strong>{evento}</strong><br>");
            sb.Append($"{Localization.Resources.GestaoEntregas.Acompanhamento.VejaMaisDetalhesEm} <a href=\"{urlRastreamento}\" title=\"{Localization.Resources.GestaoEntregas.Acompanhamento.DetalhesDoSeuPedido}\">{urlRastreamento}</a>");
            sb.Append("<br>");
            sb.Append($"{Localization.Resources.GestaoEntregas.Acompanhamento.Pedido}: {pedido.NumeroPedidoEmbarcador}");
            sb.Append("<br>");

            if (numeroNotas.Count > 0)
            {
                sb.Append("<br>");
                sb.Append($"{Localization.Resources.GestaoEntregas.Acompanhamento.NotasFiscais}: {string.Join(", ", numeroNotas)}");
            }

            if (configuracao.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior)
            {
                sb.Append("<br>");
                sb.Append($"{Localization.Resources.GestaoEntregas.Acompanhamento.NumeroPedidoCliente}: {pedido?.CodigoPedidoCliente ?? string.Empty}");
                sb.Append("<br>");
                sb.Append($"{Localization.Resources.GestaoEntregas.Acompanhamento.NumeroOrdem}: {pedido?.NumeroOrdem ?? carga?.DadosSumarizados?.NumeroOrdem ?? string.Empty}");
            }

            sb.Append("");

            return sb.ToString();
        }

        private static string ObterBodyNotificaoEntrega(string evento, string urlRastreamento, Repositorio.UnitOfWork unitOfWork)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{Localization.Resources.GestaoEntregas.Acompanhamento.TemosAtualizacoesDaSuaEntrega}<br>");
            sb.Append($"<strong>{evento}</strong><br>");
            sb.Append($"{Localization.Resources.GestaoEntregas.Acompanhamento.VejaMaisDetalhesEm} <a href=\"{urlRastreamento}\" title=\"{Localization.Resources.GestaoEntregas.Acompanhamento.DetalhesDaSuaEntrega}\">{urlRastreamento}</a>");
            sb.Append("");

            return sb.ToString();
        }

        private static string ObterAssuntoNotificacao(string evento, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido != null)
                return $"{Localization.Resources.GestaoEntregas.Acompanhamento.SuaEntrega} - " + evento + $" - {Localization.Resources.GestaoEntregas.Acompanhamento.Pedido}: {pedido.NumeroPedidoEmbarcador}" + $" - {Localization.Resources.GestaoEntregas.Acompanhamento.Remetente}: {pedido.Remetente.NomeFantasia}";
            else
                return "";
        }

        private static string[] ObterEmailVendedor(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            List<string> emails = new List<string>();

            if (pedido?.FuncionarioGerente != null)
            {

                // Gerente Área
                if (!string.IsNullOrWhiteSpace(pedido?.FuncionarioGerente?.Email))
                    emails.Add(pedido?.FuncionarioGerente?.Email);

                // Gerente Regional 
                if (!string.IsNullOrWhiteSpace(pedido?.FuncionarioGerente?.Gerente?.Email))
                    emails.Add(pedido?.FuncionarioGerente?.Gerente?.Email);

                // Gerente Nacional 
                if (!string.IsNullOrWhiteSpace(pedido?.FuncionarioGerente?.Gerente?.Gerente?.Email))
                    emails.Add(pedido?.FuncionarioGerente?.Gerente?.Gerente?.Email);

            }
            else if (pedido?.FuncionarioSupervisor != null)
            {
                // Gerente Área
                if (!string.IsNullOrWhiteSpace(pedido?.FuncionarioSupervisor?.Gerente?.Email))
                    emails.Add(pedido?.FuncionarioSupervisor?.Gerente?.Email);

                // Gerente Regional 
                if (!string.IsNullOrWhiteSpace(pedido?.FuncionarioSupervisor?.Gerente?.Gerente?.Email))
                    emails.Add(pedido?.FuncionarioSupervisor?.Gerente?.Gerente?.Email);

                // Gerente Nacional 
                if (!string.IsNullOrWhiteSpace(pedido?.FuncionarioSupervisor?.Gerente?.Gerente?.Gerente?.Email))
                    emails.Add(pedido?.FuncionarioSupervisor?.Gerente?.Gerente?.Gerente?.Email);
            }

            // Vendedor
            if (!string.IsNullOrWhiteSpace(pedido?.FuncionarioVendedor?.Email))
                emails.Add(pedido?.FuncionarioVendedor?.Email);

            // Supervisor
            if (!string.IsNullOrWhiteSpace(pedido?.FuncionarioSupervisor?.Email))
                emails.Add(pedido?.FuncionarioSupervisor?.Email);

            return emails.ToArray();
        }

        public static string ObterURLBase(int codigoClienteAdminMultisoftware, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftware, string ConexaoAdmin, Repositorio.UnitOfWork unitOfWork = null)
        {

            if (!unitOfWorkAdminMultisoftware.IsOpenSession())
                unitOfWorkAdminMultisoftware = new AdminMultisoftware.Repositorio.UnitOfWork(ConexaoAdmin);

            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdminMultisoftware);
            List<AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso> clienteURLAcesso = repClienteURLAcesso.BuscarPorClientes(new List<int> { codigoClienteAdminMultisoftware });

            bool ambienteProducao = Ambiente.Producao(unitOfWork);
            bool ambienteSeguro = Ambiente.Seguro(unitOfWork);

            var clienteURL = (from cliente in clienteURLAcesso
                              where
                              cliente.TipoServicoMultisoftware == tipoServicoMultisoftware
                              && cliente.URLHomologacao != ambienteProducao
                              select cliente).FirstOrDefault();

            return (ambienteSeguro ? "https" : "http") + "://" + clienteURL?.URLAcesso ?? string.Empty;
        }

        public static string ObterDescricaoPortalOcorrencia(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Cliente remetente, bool descricaoCompleta = false)
        {
            return ObterDescricaoPortalOcorrencia(tipoOcorrencia, carga?.CodigoCargaEmbarcador ?? string.Empty, cliente?.Nome ?? null, cliente?.Localidade?.Descricao ?? string.Empty, remetente?.Nome ?? string.Empty, remetente?.Localidade?.Descricao ?? string.Empty, descricaoCompleta);
        }

        public static string ObterDescricaoPortalOcorrencia(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, string codigoCargaEmbarcador, string clienteNome, string clienteLocalidade, string remetenteNome, string remetenteLocalidade, bool descricaoCompleta = false)
        {
            if (tipoOcorrencia == null || clienteNome == null)
                return "";

            string descricao = descricaoCompleta ? tipoOcorrencia.DescricaoCompleta : tipoOcorrencia.DescricaoVisualizacao;

            return descricao
                .Replace("#NomeCliente", clienteNome)
                .Replace("#NumeroCarga", codigoCargaEmbarcador)
                .Replace("#NomeRemetente", remetenteNome)
                .Replace("#CidadeOrigem", remetenteLocalidade)
                .Replace("#CidadeDestino", clienteLocalidade)
                .Replace("#MotivoOcorrencia", tipoOcorrencia.Descricao ?? "");
        }
    }
}