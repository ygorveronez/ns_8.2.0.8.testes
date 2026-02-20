using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Servicos.Embarcador.Email
{
    public class EnvioEmailDocumentacao
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;

        #endregion

        #region Construtores

        public EnvioEmailDocumentacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _auditado = auditado;
            _clienteMultisoftware = cliente;
        }

        #endregion

        #region Métodos Públicos

        public void EnviarEmailDocumentacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            try
            {
                if (carga.TipoOperacao == null)
                    throw new Exception($"Carga {carga.CodigoCargaEmbarcador} não possui tipo de operação para enviar documentação");

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
                Repositorio.Embarcador.Email.EmailDocumentacaoCarga repositorioEmailDocumentacaoCarga = new Repositorio.Embarcador.Email.EmailDocumentacaoCarga(_unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa repositorioConfiguracaoPessoa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa configuracaoPessoa = repositorioConfiguracaoPessoa.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(carga.Codigo);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
                if (configuracaoEmail == null)
                    throw new Exception("Não possui e-mail configurado para enviar documentação");

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repositorioContratoFrete.BuscarPorCarga(carga.Codigo);

                Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoDoTerceiro = contratoFrete != null ? repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacao(contratoFrete.TransportadorTerceiro.CPF_CNPJ, carga.TipoOperacao.Codigo) : null;

                if (emailDocumentacaoDoTerceiro?.AgruparEnvioEmUmUnicoEmail ?? false)
                {
                    EnviarEmailUnico(carga, contratoFrete, cargaCIOT, emailDocumentacaoDoTerceiro, configuracaoEmail, configuracaoPessoa);
                    EnviarEmailDosCTes(carga, emailDocumentacaoDoTerceiro, configuracaoEmail, configuracaoPessoa);
                    EnviarEmailDosMDFes(carga, emailDocumentacaoDoTerceiro, configuracaoEmail);
                }
                else
                {
                    EnviarEmailDoContratoFrete(carga, contratoFrete, emailDocumentacaoDoTerceiro, configuracaoEmail);
                    EnviarEmailDosCTes(carga, emailDocumentacaoDoTerceiro, configuracaoEmail, configuracaoPessoa);
                    EnviarEmailDosMDFes(carga, emailDocumentacaoDoTerceiro, configuracaoEmail);
                    EnviarEmailCIOT(carga, cargaCIOT, emailDocumentacaoDoTerceiro, configuracaoEmail);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            carga.SituacaoEnvioEmailDocumentacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioEmailDocumentacaoCarga.EnviadoComSucesso;
            repositorioCarga.Atualizar(carga);
        }

        public void EnviarEmailIrregularidades()
        {
            Repositorio.Embarcador.Notificacoes.AlertaEmail repositorioalertaEmail = new Repositorio.Embarcador.Notificacoes.AlertaEmail(_unitOfWork);
            Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail> configuracaoesEmail = repositorioalertaEmail.BuscarTodos();
            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);

            foreach (var configuracaoEmail in configuracaoesEmail)
            {
                DateTime dateTimeHoje = DateTime.Now;
                DateTime dataUltimaVerificao = configuracaoEmail.DataUltimoEnvio.HasValue ? configuracaoEmail.DataUltimoEnvio.Value : DateTime.Now;

                if (configuracaoEmail.DataUltimoEnvio.HasValue && dataUltimaVerificao.Date == dateTimeHoje.Date)
                    continue;

                if (!(dataUltimaVerificao.Date == dateTimeHoje.Date))
                {
                    if (dataUltimaVerificao.Date != ObterDataComparacao(dateTimeHoje, configuracaoEmail.NumeroRepeticoes, configuracaoEmail.PeriodoNotificacoes))
                        continue;

                    if (!(dateTimeHoje >= configuracaoEmail.DataHoraInicio && configuracaoEmail.DataHoraFim >= dateTimeHoje))
                        continue;
                }
                else if (!(dateTimeHoje >= configuracaoEmail.DataHoraInicio && configuracaoEmail.DataHoraFim >= dateTimeHoje))
                    continue;

                var codigosIrregularidade = configuracaoEmail.Irregularidade != null ? (from obj in configuracaoEmail.Irregularidade select obj.Codigo).ToList() : new List<int>();
                var codigosPortifolio = configuracaoEmail.Portfolio != null ? (from obj in configuracaoEmail.Portfolio select obj.Codigo).ToList() : new List<int>();
                var codigosSetores = configuracaoEmail.Setor != null ? (from obj in configuracaoEmail.Setor select obj.Codigo).ToList() : new List<int>();
                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.PendenciasControleDocumento> pendenciasControleDocumento = repositorioControleDocumento.BuscarPendenciasControleDocumento(codigosIrregularidade, codigosPortifolio, codigosSetores);

                if (pendenciasControleDocumento.Count == 0)
                    continue;

                bool envioEmail = false;
                foreach (var setor in configuracaoEmail.Setor)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Documentos.PendenciasControleDocumento> pendenciasPorSetor = pendenciasControleDocumento.Where(x => x.CodigoSetor == setor.Codigo).ToList();

                    if (pendenciasPorSetor.Count == 0)
                        continue;

                    var usuariosDoSetor = configuracaoEmail.Usuarios.Where(x => x.Setor.Codigo == setor.Codigo).ToList();

                    StringBuilder mensagem = new StringBuilder();

                    mensagem.AppendLine($"Caro setor de {setor.Descricao}");
                    mensagem.AppendLine("");
                    mensagem.AppendLine($"Seguem abaixo as pendências até a data {dateTimeHoje.ToString("dd/MM/yyyy")} no Controle de documentos da Unilever. Por favor entrar acesse o sistema para dar prosseguimento nas pendências:");
                    mensagem.AppendLine("");

                    foreach (var pendencia in pendenciasControleDocumento)
                        mensagem.AppendLine($"{pendencia.Gatilho.ObterDescricao()} ({pendencia.Quantidade})");

                    foreach (var usuario in usuariosDoSetor)
                    {
                        envioEmail = true;
                        servicoEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, usuario.Email, "", "", $"Irregularidades Pendentes", mensagem.ToString(), string.Empty, null, string.Empty, false, string.Empty, 0, _unitOfWork);
                    }
                }

                if (envioEmail)
                {
                    configuracaoEmail.DataUltimoEnvio = DateTime.Now;
                    repositorioalertaEmail.Atualizar(configuracaoEmail);
                }
            }

        }
        #endregion

        #region Métodos Privados

        private void EnviarEmailUnico(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoDoTerceiro, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa configuracaoPessoa)
        {
            Repositorio.Embarcador.Email.EmailDocumentacaoCarga repositorioEmailDocumentacaoCarga = new Repositorio.Embarcador.Email.EmailDocumentacaoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);
            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);

            if (!(emailDocumentacaoDoTerceiro?.EnviarCTe ?? false) && !(emailDocumentacaoDoTerceiro?.EnviarMDFe ?? false) && !(emailDocumentacaoDoTerceiro?.EnviarContratoFrete ?? false) && !(emailDocumentacaoDoTerceiro?.EnviarCIOT ?? false))
                return;

            List<Attachment> attachments = new List<Attachment>();

            bool temCTe = false;
            if (emailDocumentacaoDoTerceiro.EnviarCTe)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCTe.BuscarPorCarga(carga.Codigo);
                if (cargaCTes.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                    {
                        if (cargaCTe.CTe == null)
                            continue;

                        temCTe = true;
                        this.retornarAnexoCTe(cargaCTe.CTe, emailDocumentacaoDoTerceiro).ForEach(o => attachments.Add(o));
                    }
                }
            }

            bool temMDFe = false;
            if (emailDocumentacaoDoTerceiro.EnviarMDFe)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repositorioCargaMDFe.BuscarPorCarga(carga.Codigo);
                if (cargaMDFes.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFes)
                    {
                        if (cargaMDFe.MDFe == null)
                            continue;

                        temMDFe = true;
                        this.retornarAnexoMDFe(cargaMDFe.MDFe).ForEach(o => attachments.Add(o));
                    }
                }
            }

            bool temContratoFrete = false;
            if (contratoFrete != null && emailDocumentacaoDoTerceiro.EnviarContratoFrete)
            {
                temContratoFrete = true;
                Attachment anexo = retornarAnexoContratoFrete(contratoFrete);
                if (anexo != null)
                    attachments.Add(anexo);
            }

            if (cargaCIOT?.CIOT != null && (emailDocumentacaoDoTerceiro.EnviarCIOT ?? true))
            {
                Attachment anexo = retornarAnexoCIOT(cargaCIOT);
                if (anexo != null)
                    attachments.Add(anexo);
            }

            if (!temCTe && !temContratoFrete)
                return;

            string placas = RetornarPlacasFormatada(carga);
            string assunto = $"Carga {carga.CodigoCargaEmbarcador} - {placas} - {contratoFrete.TransportadorTerceiro.Nome}";
            StringBuilder mensagem = new StringBuilder();

            mensagem.AppendLine($"Prezado Transportador {contratoFrete.TransportadorTerceiro.Nome},");
            mensagem.AppendLine();
            string mensagemDocs = "Segue em anexo ";
            if (temCTe)
                mensagemDocs += "arquivo XML, DACTE";
            if (temMDFe)
                mensagemDocs += ", DAMDFE";
            if (temContratoFrete)
                mensagemDocs += $" e Contrato de Frete N° {contratoFrete.NumeroContrato}";
            mensagemDocs += $" referente a carga {carga.CodigoCargaEmbarcador}.";
            mensagem.AppendLine(mensagemDocs);
            mensagem.AppendLine();
            mensagem.AppendLine("Resumo Carga:");
            mensagem.AppendLine($"Origem: {carga?.Pedidos?.FirstOrDefault()?.Origem?.DescricaoCidadeEstado ?? ""}");
            mensagem.AppendLine($"Destino: {carga?.Pedidos?.FirstOrDefault()?.Destino?.DescricaoCidadeEstado ?? ""}");
            mensagem.AppendLine($"Cliente: {carga?.Pedidos?.FirstOrDefault()?.ObterTomador()?.Nome ?? ""}");
            mensagem.AppendLine($"Placas: {placas}");

            if (temContratoFrete)
            {
                mensagem.AppendLine();
                mensagem.AppendLine("Resumo do Contrato:");
                mensagem.AppendLine($"Número: {contratoFrete.NumeroContrato}");
                mensagem.AppendLine($"Data Emissão: {contratoFrete.DataEmissaoContrato.ToDateTimeString()}");
            }

            mensagem.AppendLine();
            mensagem.AppendLine("Envio de e-mail automático. Favor não responder");

            servicoEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, emailDocumentacaoDoTerceiro.Emails, null, null, assunto, mensagem.ToString(), configuracaoEmail.Smtp, attachments, null, configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, $"Enviada a documentação para o(s) e-mail(s): {emailDocumentacaoDoTerceiro.Emails}", _unitOfWork);
        }

        private void EnviarEmailDoContratoFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoDoTerceiro, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail)
        {
            if (contratoFrete == null || emailDocumentacaoDoTerceiro == null || !emailDocumentacaoDoTerceiro.EnviarContratoFrete)
                return;

            string placas = RetornarPlacasFormatada(carga);
            string assunto = $"Contrato de Frete {contratoFrete.NumeroContrato} - {placas} - {contratoFrete.TransportadorTerceiro.Nome}";
            StringBuilder mensagem = new StringBuilder();

            mensagem.AppendLine("Prezado Transportador,");
            mensagem.AppendLine();
            mensagem.AppendLine($"Segue em anexo Contrato de Frete N° {contratoFrete.NumeroContrato}");
            mensagem.AppendLine("Resumo do Contrato:");
            mensagem.AppendLine($"Número: {contratoFrete.NumeroContrato}");
            mensagem.AppendLine($"Data Emissão: {contratoFrete.DataEmissaoContrato.ToDateTimeString()}");
            mensagem.AppendLine($"Placas: {placas}");

            mensagem.AppendLine();
            mensagem.AppendLine("Envio de e-mail automático. Favor não responder");

            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);

            Attachment anexo = retornarAnexoContratoFrete(contratoFrete);
            if (anexo == null)
                return;

            List<Attachment> attachments = new List<Attachment>();
            attachments.Add(anexo);

            servicoEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, emailDocumentacaoDoTerceiro.Emails, null, null, assunto, mensagem.ToString(), configuracaoEmail.Smtp, attachments, null, configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, contratoFrete, $"Enviada a documentação para o(s) e-mail(s): {emailDocumentacaoDoTerceiro.Emails}", _unitOfWork);
        }

        private Attachment retornarAnexoContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete)
        {
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(_unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(_unitOfWork);

            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = servicoRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R011_ContratoDeFrete, _tipoServicoMultisoftware, "Contrato de Transporte", "Terceiros", "ContratoFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, _unitOfWork, false, false, 9, "Arial", false, 120);
            byte[] buffer = servicoContratoFrete.GerarFaturaPadrao(contratoFrete, _unitOfWork, _tipoServicoMultisoftware, relatorioTemp);
            if (buffer == null)
                return null;

            MemoryStream stream = new MemoryStream(buffer);
            Attachment anexo = new Attachment(stream, $"Contrato_Frete_{contratoFrete.NumeroContrato}.pdf", "application/pdf");

            return anexo;
        }

        private void EnviarEmailDosCTes(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoDoTerceiro, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa configuracaoPessoa)
        {
            Repositorio.Embarcador.Email.EmailDocumentacaoCarga repositorioEmailDocumentacaoCarga = new Repositorio.Embarcador.Email.EmailDocumentacaoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCTe.BuscarPorCarga(carga.Codigo);
            if (cargaCTes.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTe.CTe;
                if (cte == null)
                    continue;

                if ((emailDocumentacaoDoTerceiro?.EnviarCTe ?? false) && !(emailDocumentacaoDoTerceiro?.AgruparEnvioEmUmUnicoEmail ?? false))
                    EnviarEmailPorCTe(cte, carga, emailDocumentacaoDoTerceiro, configuracaoEmail);

                if (cte.Remetente != null)
                {
                    if (configuracaoPessoa.NaoEnviarXMLCTEPorEmailParaTipoServico && configuracaoPessoa.TiposServicosCTe.Contains(cte.TipoServico))
                        continue;

                    Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacaoEnvioCTe(cte.Remetente.CPF_CNPJ.ToDouble(), carga.TipoOperacao.Codigo);
                    if (emailDocumentacaoCarga != null)
                        EnviarEmailPorCTe(cte, carga, emailDocumentacaoCarga, configuracaoEmail);
                }

                if (cte.Destinatario != null)
                {
                    Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacaoEnvioCTe(cte.Destinatario.CPF_CNPJ.ToDouble(), carga.TipoOperacao.Codigo);
                    if (emailDocumentacaoCarga != null)
                        EnviarEmailPorCTe(cte, carga, emailDocumentacaoCarga, configuracaoEmail);
                }

                if (cte.Recebedor != null)
                {
                    Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacaoEnvioCTe(cte.Recebedor.CPF_CNPJ.ToDouble(), carga.TipoOperacao.Codigo);
                    if (emailDocumentacaoCarga != null)
                        EnviarEmailPorCTe(cte, carga, emailDocumentacaoCarga, configuracaoEmail);
                }

                if (cte.Expedidor != null)
                {
                    Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacaoEnvioCTe(cte.Expedidor.CPF_CNPJ.ToDouble(), carga.TipoOperacao.Codigo);
                    if (emailDocumentacaoCarga != null)
                        EnviarEmailPorCTe(cte, carga, emailDocumentacaoCarga, configuracaoEmail);
                }

                if (cte.OutrosTomador != null)
                {
                    Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacaoEnvioCTe(cte.OutrosTomador.CPF_CNPJ.ToDouble(), carga.TipoOperacao.Codigo);
                    if (emailDocumentacaoCarga != null)
                        EnviarEmailPorCTe(cte, carga, emailDocumentacaoCarga, configuracaoEmail);
                }
            }
        }

        private void EnviarEmailDosMDFes(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoDoTerceiro, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail)
        {
            Repositorio.Embarcador.Email.EmailDocumentacaoCarga repositorioEmailDocumentacaoCarga = new Repositorio.Embarcador.Email.EmailDocumentacaoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioManifestoEletronicoDeDocumentosFiscais = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repositorioCargaMDFe.BuscarPorCarga(carga.Codigo);
            if (cargaMDFes.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFes)
            {
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = cargaMDFe.MDFe;
                if (mdfe == null)
                    continue;

                if ((emailDocumentacaoDoTerceiro?.EnviarMDFe ?? false) && !(emailDocumentacaoDoTerceiro?.AgruparEnvioEmUmUnicoEmail ?? false))
                    EnviarEmailPorMDFe(mdfe, carga, emailDocumentacaoDoTerceiro, configuracaoEmail);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioManifestoEletronicoDeDocumentosFiscais.BuscarPrimeiroCTePorMDFe(mdfe.Codigo);
                if (cte == null)
                    continue;

                if (cte.Remetente != null)
                {
                    Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacaoEnvioMDFe(cte.Remetente.CPF_CNPJ.ToDouble(), carga.TipoOperacao.Codigo);
                    if (emailDocumentacaoCarga != null)
                        EnviarEmailPorMDFe(mdfe, carga, emailDocumentacaoCarga, configuracaoEmail);
                }

                if (cte.Destinatario != null)
                {
                    Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacaoEnvioMDFe(cte.Destinatario.CPF_CNPJ.ToDouble(), carga.TipoOperacao.Codigo);
                    if (emailDocumentacaoCarga != null)
                        EnviarEmailPorMDFe(mdfe, carga, emailDocumentacaoCarga, configuracaoEmail);
                }

                if (cte.Recebedor != null)
                {
                    Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacaoEnvioMDFe(cte.Recebedor.CPF_CNPJ.ToDouble(), carga.TipoOperacao.Codigo);
                    if (emailDocumentacaoCarga != null)
                        EnviarEmailPorMDFe(mdfe, carga, emailDocumentacaoCarga, configuracaoEmail);
                }

                if (cte.Expedidor != null)
                {
                    Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacaoEnvioMDFe(cte.Expedidor.CPF_CNPJ.ToDouble(), carga.TipoOperacao.Codigo);
                    if (emailDocumentacaoCarga != null)
                        EnviarEmailPorMDFe(mdfe, carga, emailDocumentacaoCarga, configuracaoEmail);
                }

                if (cte.OutrosTomador != null)
                {
                    Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repositorioEmailDocumentacaoCarga.BuscarPorPessoaTipoOperacaoEnvioMDFe(cte.OutrosTomador.CPF_CNPJ.ToDouble(), carga.TipoOperacao.Codigo);
                    if (emailDocumentacaoCarga != null)
                        EnviarEmailPorMDFe(mdfe, carga, emailDocumentacaoCarga, configuracaoEmail);
                }
            }
        }

        private void EnviarEmailPorCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail)
        {
            string placas = RetornarPlacasFormatada(carga);
            string grupoPessoas = carga.GrupoPessoaPrincipal?.Descricao;
            string assunto = $"CT-e {cte.Numero} Série {cte.Serie.Numero} - {placas} - {cte.Empresa.RazaoSocial}{(!string.IsNullOrWhiteSpace(grupoPessoas) ? " - " + grupoPessoas : "")}";

            StringBuilder mensagem = new StringBuilder();
            mensagem.AppendLine("Prezado Cliente,");
            mensagem.AppendLine();
            mensagem.AppendLine("Segue em anexo arquivo XML e DACTE referente ao Conhecimento de Transporte Eletrônico");
            mensagem.AppendLine($"Chave: {cte.Chave}");
            mensagem.AppendLine($"Emitido por {cte.Empresa.NomeCNPJ}");
            mensagem.AppendLine();
            mensagem.AppendLine("Resumo do CT-e:");
            mensagem.AppendLine($"Número: {cte.Numero}");
            mensagem.AppendLine($"Série: {cte.Serie.Numero}");
            mensagem.AppendLine($"Protocolo: {cte.Protocolo}");
            mensagem.AppendLine($"Data Emissão: {cte.DataEmissao?.ToDateTimeString()}");
            mensagem.AppendLine($"Placas: {placas}");
            mensagem.AppendLine($"Grupo de Pessoas: {grupoPessoas}");

            mensagem.AppendLine();
            mensagem.AppendLine("Envio de e-mail automático. Favor não responder");

            List<Attachment> attachments = this.retornarAnexoCTe(cte);

            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);
            servicoEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, emailDocumentacaoCarga.Emails, null, null, assunto, mensagem.ToString(), configuracaoEmail.Smtp, attachments, null, configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, cte, $"Enviada a documentação para o(s) e-mail(s): {emailDocumentacaoCarga.Emails}", _unitOfWork);
        }

        private List<Attachment> retornarAnexoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            List<Attachment> retorno = new List<Attachment>();

            if (cte.ModeloDocumentoFiscal.Numero == "39")
            {
                Servicos.NFSe svcNFSe = new Servicos.NFSe();
                string nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".xml";
                byte[] data = svcNFSe.ObterXMLAutorizacaoCTe(cte.Codigo, _unitOfWork);
                if (data != null)
                {
                    Stream stream = new MemoryStream(data);
                    retorno.Add(new Attachment(stream, nomeArquivo));
                }

                string nomeArquivoPdf = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".pdf";
                byte[] dataPdf = svcNFSe.ObterDANFSECTe(cte.Codigo, _unitOfWork);
                if (dataPdf != null)
                {
                    Stream stream = new MemoryStream(dataPdf);
                    retorno.Add(new Attachment(stream, nomeArquivoPdf));
                }
            }
            else
            {
                Servicos.CTe svcCTe = new Servicos.CTe(_unitOfWork);
                string nomeArquivo = string.Concat(cte.Chave, ".xml");
                byte[] data = svcCTe.ObterXMLAutorizacao(cte, _unitOfWork);
                if (data != null)
                {
                    Stream stream = new MemoryStream(data);
                    retorno.Add(new Attachment(stream, nomeArquivo));
                }

                string nomeArquivoPdf = string.Concat(cte.Chave, ".pdf");
                byte[] dataPdf = svcCTe.ObterDACTE(cte, _unitOfWork);
                if (dataPdf != null)
                {
                    Stream stream = new MemoryStream(dataPdf);
                    retorno.Add(new Attachment(stream, nomeArquivoPdf));
                }
            }

            return retorno;
        }

        private List<Attachment> retornarAnexoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoDoTerceiro)
        {
            List<Attachment> retorno = new List<Attachment>();

            if (cte.ModeloDocumentoFiscal.Numero == "39")
            {
                Servicos.NFSe svcNFSe = new Servicos.NFSe();
                string nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".xml";
                byte[] data = svcNFSe.ObterXMLAutorizacaoCTe(cte.Codigo, _unitOfWork);
                if (data != null && emailDocumentacaoDoTerceiro.EnviarCTeXML)
                {
                    Stream stream = new MemoryStream(data);
                    retorno.Add(new Attachment(stream, nomeArquivo));
                }

                string nomeArquivoPdf = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".pdf";
                byte[] dataPdf = svcNFSe.ObterDANFSECTe(cte.Codigo, _unitOfWork);
                if (dataPdf != null && emailDocumentacaoDoTerceiro.EnviarCTePDF)
                {
                    Stream stream = new MemoryStream(dataPdf);
                    retorno.Add(new Attachment(stream, nomeArquivoPdf));
                }
            }
            else
            {
                Servicos.CTe svcCTe = new Servicos.CTe(_unitOfWork);
                string nomeArquivo = string.Concat(cte.Chave, ".xml");
                byte[] data = svcCTe.ObterXMLAutorizacao(cte, _unitOfWork);
                if (data != null && emailDocumentacaoDoTerceiro.EnviarCTeXML)
                {
                    Stream stream = new MemoryStream(data);
                    retorno.Add(new Attachment(stream, nomeArquivo));
                }

                string nomeArquivoPdf = string.Concat(cte.Chave, ".pdf");
                byte[] dataPdf = svcCTe.ObterDACTE(cte, _unitOfWork);
                if (dataPdf != null && emailDocumentacaoDoTerceiro.EnviarCTePDF)
                {
                    Stream stream = new MemoryStream(dataPdf);
                    retorno.Add(new Attachment(stream, nomeArquivoPdf));
                }
            }

            return retorno;
        }

        private void EnviarEmailPorMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail)
        {
            string placas = RetornarPlacasFormatada(carga);
            string grupoPessoas = carga.GrupoPessoaPrincipal?.Descricao;
            string assunto = $"MDF-e {mdfe.Numero} Série {mdfe.Serie.Numero} - {placas} - {mdfe.Empresa.RazaoSocial}{(!string.IsNullOrWhiteSpace(grupoPessoas) ? " - " + grupoPessoas : "")}";

            StringBuilder mensagem = new StringBuilder();
            mensagem.AppendLine("Prezado Cliente,");
            mensagem.AppendLine();
            mensagem.AppendLine("Segue em anexo arquivo XML e DAMDFE referente ao Manifesto de Transporte Eletrônico");
            mensagem.AppendLine($"Chave: {mdfe.Chave}");
            mensagem.AppendLine($"Emitido por {mdfe.Empresa.NomeCNPJ}");
            mensagem.AppendLine();
            mensagem.AppendLine("Resumo do MDF-e:");
            mensagem.AppendLine($"Número: {mdfe.Numero}");
            mensagem.AppendLine($"Série: {mdfe.Serie.Numero}");
            mensagem.AppendLine($"Protocolo: {mdfe.Protocolo}");
            mensagem.AppendLine($"Data Emissão: {mdfe.DataEmissao?.ToDateTimeString()}");
            mensagem.AppendLine($"Placas: {placas}");
            mensagem.AppendLine($"Grupo de Pessoas: {grupoPessoas}");

            mensagem.AppendLine();
            mensagem.AppendLine("Envio de e-mail automático. Favor não responder");

            List<Attachment> attachments = this.retornarAnexoMDFe(mdfe);

            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);
            servicoEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, emailDocumentacaoCarga.Emails, null, null, assunto, mensagem.ToString(), configuracaoEmail.Smtp, attachments, null, configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, mdfe, $"Enviada a documentação para o(s) e-mail(s): {emailDocumentacaoCarga.Emails}", _unitOfWork);
        }

        private List<Attachment> retornarAnexoMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            List<Attachment> retorno = new List<Attachment>();

            Servicos.MDFe servicoMDFe = new Servicos.MDFe(_unitOfWork);
            string nomeArquivo = string.Concat(mdfe.Chave, ".xml");
            byte[] data = servicoMDFe.ObterXMLAutorizacao(mdfe, _unitOfWork);
            if (data != null)
            {
                Stream stream = new MemoryStream(data);
                retorno.Add(new Attachment(stream, nomeArquivo));
            }

            string nomeArquivoPdf = string.Concat(mdfe.Chave, ".pdf");
            byte[] dataPdf = servicoMDFe.ObterDAMDFE(mdfe, _unitOfWork);
            if (dataPdf != null)
            {
                Stream stream = new MemoryStream(dataPdf);
                retorno.Add(new Attachment(stream, nomeArquivoPdf));
            }

            return retorno;
        }

        private void EnviarEmailCIOT(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoDoTerceiro, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail)
        {
            if (cargaCIOT == null || cargaCIOT.CIOT == null || !(emailDocumentacaoDoTerceiro?.EnviarCIOT ?? true))
                return;

            string placas = RetornarPlacasCIOTFormatada(cargaCIOT);
            string assunto = $"CIOT {cargaCIOT.CIOT.Numero} - {cargaCIOT.CIOT.Veiculo.Placa_Formatada} - {cargaCIOT.CIOT.Motorista.Nome} - {cargaCIOT.CIOT.Transportador.NomeFantasia}";
            StringBuilder mensagem = new StringBuilder();

            mensagem.AppendLine("Prezado,");
            mensagem.AppendLine();
            mensagem.AppendLine($"Segue em anexo arquivo PDF referente ao Codigo Identificador de Operação de Transporte - CIOT emitido por {_clienteMultisoftware.NomeFantasia}");
            mensagem.AppendLine("Resumo do CIOT");
            mensagem.AppendLine($"Número: {cargaCIOT.CIOT.Numero}");
            mensagem.AppendLine($"Data Emissão: {cargaCIOT.CIOT.DataAbertura.ToDateString()}");
            mensagem.AppendLine($"Placas: {placas}");

            mensagem.AppendLine();
            mensagem.AppendLine("Envio de e-mail automático. Favor não responder");

            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);

            List<Attachment> attachments = new List<Attachment>();
            Attachment anexo = retornarAnexoCIOT(cargaCIOT);
            if (anexo != null)
                attachments.Add(anexo);

            servicoEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, emailDocumentacaoDoTerceiro.Emails, null, null, assunto, mensagem.ToString(), configuracaoEmail.Smtp, attachments, null, configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, $"Enviada a documentação para o(s) e-mail(s): {emailDocumentacaoDoTerceiro?.Emails}", _unitOfWork);
        }

        private Attachment retornarAnexoCIOT(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            byte[] arquivo = null;
            string mensagemErro = string.Empty;
            if (cargaCIOT.CIOT.Operadora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete)
            {
                CIOT.EFrete serEfrete = new Servicos.Embarcador.CIOT.EFrete();
                arquivo = serEfrete.ObterOperacaoTransportePdf(cargaCIOT, _unitOfWork, out string erro);
                if (arquivo == null)
                    return null;
            }
            else
                arquivo = new Servicos.Embarcador.CIOT.CIOT().GerarContratoFrete(cargaCIOT.CIOT.Codigo, _unitOfWork, out mensagemErro);

            string nomeArquivo = "CIOT - " + cargaCIOT.CIOT.Numero.ToString() + ".pdf";

            Attachment anexo = null;
            if (arquivo != null)
            {
                Stream stream = new MemoryStream(arquivo);
                anexo = new Attachment(stream, nomeArquivo);
            }

            return anexo;
        }

        private string RetornarPlacasFormatada(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<string> placas = new List<string>();

            if (carga.Veiculo != null)
                placas.Add(carga.Veiculo.Placa);

            if (carga.VeiculosVinculados?.Count > 0)
                placas.AddRange(from veiculo in carga.VeiculosVinculados select veiculo.Placa);

            return string.Join(" / ", placas);
        }

        private string RetornarPlacasCIOTFormatada(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<string> placas = new List<string>();

            if (cargaCIOT.CIOT.VeiculosVinculados != null)
                placas.Add(cargaCIOT.CIOT.Veiculo.Placa);

            if (cargaCIOT.CIOT.VeiculosVinculados?.Count > 0)
                placas.AddRange(from veiculo in cargaCIOT.CIOT.VeiculosVinculados select veiculo.Placa);

            return string.Join(" / ", placas);
        }

        private DateTime ObterDataComparacao(DateTime atual, int numeroRepeticoes, EnumIntervaloAlertaEmail intervado)
        {
            DateTime dataRetorno = atual;

            if (intervado == EnumIntervaloAlertaEmail.Mes)
                dataRetorno = dataRetorno.AddMonths(-numeroRepeticoes);
            else if (intervado == EnumIntervaloAlertaEmail.Ano)
                dataRetorno = dataRetorno.AddYears(-numeroRepeticoes);
            else if (intervado == EnumIntervaloAlertaEmail.Semana)
                dataRetorno = dataRetorno.AddDays(-(numeroRepeticoes * 7));
            else
                dataRetorno = dataRetorno.AddDays(-numeroRepeticoes);

            return dataRetorno.Date;
        }

        #endregion
    }
}
