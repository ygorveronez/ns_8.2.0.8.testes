using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Integracao.Email
{
    public class IntegracaoEmail : ServicoBase
    {
        public IntegracaoEmail() : base() { }

        #region Métodos públicos



        public static void EnviarEDI(Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEdi, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (controleIntegracaoCargaEdi.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                return;

            string mensagemErro = string.Empty,
                   emails = string.Empty;

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (email == null)
            {
                controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Falha;
                mensagemErro = "Não existe configuração de e-mail ativa para envio dos documentos.";
                return;
            }


            Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = controleIntegracaoCargaEdi.Transportador?.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Codigo == controleIntegracaoCargaEdi.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP).FirstOrDefault();

            try
            {
                foreach (var carga in controleIntegracaoCargaEdi.Cargas)
                {

                    Dominio.Entidades.Empresa empresa = controleIntegracaoCargaEdi.Transportador;
                    int codigoLayoutEDI = controleIntegracaoCargaEdi.LayoutEDI.Codigo;

                    Dominio.Entidades.Cliente tomador = carga.Pedidos.FirstOrDefault().ObterTomador();
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = carga.TipoOperacao;

                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacao = null;

                    if (tipoOperacao != null)
                        configuracaoTipoOperacao = tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                    Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;
                    if (configuracaoTipoOperacao == null)
                        configuracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                    if (configuracaoCliente == null && configuracaoTipoOperacao == null)
                        configuracaoGrupoPessoas = tomador.GrupoPessoas?.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                    Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI transportadorLayoutEDI = null;
                    if (configuracaoGrupoPessoas == null && configuracaoTipoOperacao == null && configuracaoCliente == null)
                        transportadorLayoutEDI = empresa.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                    emails = transportadorLayoutEDI?.Emails ?? (configuracaoGrupoPessoas?.Emails ?? (configuracaoCliente?.Emails ?? configuracaoGrupoPessoas?.Emails));

                    if (string.IsNullOrWhiteSpace(emails))
                    {
                        controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Falha;
                        mensagemErro = "E-mail para envio dos documentos é inválido.";
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(emails))
                    {
                        controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Falha;
                        mensagemErro = "E-mail para envio dos documentos é inválido.";
                        return;
                    }


                    string extensao = string.Empty;

                    Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = Servicos.Embarcador.Carga.CargaIntegracaoEDI.ConverterCargaEmNotFis(carga, controleIntegracaoCargaEdi.LayoutEDI, unidadeDeTrabalho);

                    Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unidadeDeTrabalho, controleIntegracaoCargaEdi.LayoutEDI, null);
                    MemoryStream arquivoEDI = serGeracaoEDI.GerarArquivoRecursivo(notfis);

                    if (arquivoEDI == null)
                    {
                        throw new Exception("Arquivo EDI não encontrado");
                    }

                    if (Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, "Arquivo EDI " + controleIntegracaoCargaEdi.LayoutEDI.Tipo.ToString("g") + "da carga", "Segue em anexo o arquivo EDI referente a Carga.", email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(arquivoEDI, controleIntegracaoCargaEdi.NomeArquivo, extensao == ".zip" ? "application/zip" : "text/plain") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Integrado;
                    }
                    else
                        controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Falha;

                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + controleIntegracaoCargaEdi.LayoutEDI.Descricao + " para o(s) e-mail(s) '" + emails + "'";
                controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Falha;
            }
            finally
            {
                controleIntegracaoCargaEdi.MensagemRetorno = mensagemErro;
                controleIntegracaoCargaEdi.NumeroTentativas++;
                repControleIntegracaoCargaEDI.Atualizar(controleIntegracaoCargaEdi);
            }

        }

        public static void EnviarEDI(ref Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI cargaCancelamentoIntegracaoEDI, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (cargaCancelamentoIntegracaoEDI.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                return;

            string mensagemErro = string.Empty,
                   emails = string.Empty;

            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email == null)
                {
                    cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "Não existe configuração de e-mail ativa para envio dos documentos.";
                    return;
                }
                Dominio.Entidades.Cliente tomador = cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.Pedidos.First().ObterTomador();
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.TipoOperacao;

                int codigoLayoutEDI = cargaCancelamentoIntegracaoEDI.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacao = null;
                if (tipoOperacao != null)
                    configuracaoTipoOperacao = tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;
                if (configuracaoTipoOperacao == null)
                    configuracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                if (configuracaoCliente == null && configuracaoTipoOperacao == null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                emails = configuracaoGrupoPessoas?.Emails ?? (configuracaoCliente?.Emails ?? configuracaoGrupoPessoas?.Emails);

                if (string.IsNullOrWhiteSpace(emails))
                {
                    cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "E-mail para envio dos documentos é inválido.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(emails))
                {
                    cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "E-mail para envio dos documentos é inválido.";
                    return;
                }

                string extensao = string.Empty;

                using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(cargaCancelamentoIntegracaoEDI, unidadeDeTrabalho))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(cargaCancelamentoIntegracaoEDI, unidadeDeTrabalho);

                    if (Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, "Arquivo EDI " + cargaCancelamentoIntegracaoEDI.LayoutEDI.Tipo.ToString("g") + " do Cancelamento de carga", "Segue em anexo o arquivo EDI referente ao Cancelamento de Carga.", email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(arquivoEDI, nomeArquivo, extensao == ".zip" ? "application/zip" : "text/plain") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + cargaCancelamentoIntegracaoEDI.LayoutEDI.Descricao + " para o(s) e-mail(s) '" + emails + "'";

                cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                cargaCancelamentoIntegracaoEDI.ProblemaIntegracao = mensagemErro;
                cargaCancelamentoIntegracaoEDI.DataIntegracao = DateTime.Now;
                cargaCancelamentoIntegracaoEDI.NumeroTentativas++;
            }
        }

        public static void EnviarEDI(ref Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao nfsManualEDIIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            if (nfsManualEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                return;

            string mensagemErro = string.Empty,
                   emails = string.Empty;

            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email == null)
                {
                    nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "Não existe configuração de e-mail ativa para envio dos documentos.";
                    return;
                }

                Dominio.Entidades.Cliente tomador = nfsManualEDIIntegracao.LancamentoNFSManual.Tomador;

                int codigoLayoutEDI = nfsManualEDIIntegracao.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                if (configuracaoCliente == null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                emails = configuracaoCliente?.Emails ?? configuracaoGrupoPessoas?.Emails;

                if (string.IsNullOrWhiteSpace(emails))
                {
                    nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "E-mail para envio dos documentos é inválido.";
                    return;
                }

                string extensao = string.Empty;

                using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(nfsManualEDIIntegracao, tipoServicoMultisoftware, unidadeDeTrabalho, stringConexao, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(nfsManualEDIIntegracao, extensao, unidadeDeTrabalho);

                    if (Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, "Arquivo EDI " + nfsManualEDIIntegracao.LayoutEDI.Tipo.ToString("g") + " da Nota de Serviço Nº " + nfsManualEDIIntegracao.LancamentoNFSManual.DadosNFS.Numero, "Segue em anexo o arquivo EDI referente à Nota de Serviço número " + nfsManualEDIIntegracao.LancamentoNFSManual.DadosNFS.Numero + ".", email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(arquivoEDI, nomeArquivo, extensao == ".zip" ? "application/zip" : "text/plain") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + nfsManualEDIIntegracao.LayoutEDI.Descricao + " para o(s) e-mail(s) '" + emails + "'";

                nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                nfsManualEDIIntegracao.ProblemaIntegracao = mensagemErro;
                nfsManualEDIIntegracao.DataIntegracao = DateTime.Now;
                nfsManualEDIIntegracao.NumeroTentativas++;
            }
        }

        public static void EnviarEDI(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI nfsManualCancelamentoIntegracaoEDI, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (nfsManualCancelamentoIntegracaoEDI.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                return;

            string mensagemErro = string.Empty,
                   emails = string.Empty;

            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email == null)
                {
                    nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "Não existe configuração de e-mail ativa para envio dos documentos.";
                    return;
                }

                Dominio.Entidades.Cliente tomador = nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.Tomador;

                int codigoLayoutEDI = nfsManualCancelamentoIntegracaoEDI.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                if (configuracaoCliente == null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                emails = configuracaoCliente?.Emails ?? configuracaoGrupoPessoas?.Emails;

                if (string.IsNullOrWhiteSpace(emails))
                {
                    nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "E-mail para envio dos documentos é inválido.";
                    return;
                }

                string extensao = string.Empty;

                using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(nfsManualCancelamentoIntegracaoEDI, tipoServicoMultisoftware, unidadeDeTrabalho, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(nfsManualCancelamentoIntegracaoEDI, extensao, unidadeDeTrabalho);

                    if (Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, "Arquivo EDI " + nfsManualCancelamentoIntegracaoEDI.LayoutEDI.Tipo.ToString("g") + " ref. ao cancelamento da Nota de Serviço Nº " + nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.DadosNFS.Numero, "Segue em anexo o arquivo EDI referente ao cancelamento da Nota de Serviço número " + nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.DadosNFS.Numero + ".", email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(arquivoEDI, nomeArquivo, extensao == ".zip" ? "application/zip" : "text/plain") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + nfsManualCancelamentoIntegracaoEDI.LayoutEDI.Descricao + " para o(s) e-mail(s) '" + emails + "'";

                nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                nfsManualCancelamentoIntegracaoEDI.ProblemaIntegracao = mensagemErro;
                nfsManualCancelamentoIntegracaoEDI.DataIntegracao = DateTime.Now;
                nfsManualCancelamentoIntegracaoEDI.NumeroTentativas++;
            }
        }

        public static void EnviarEDI(ref Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao LoteEscrituracaoEDIIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            if (LoteEscrituracaoEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                return;

            string mensagemErro = string.Empty,
                   emails = string.Empty;

            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email == null)
                {
                    LoteEscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "Não existe configuração de e-mail ativa para envio dos documentos.";
                    return;
                }

                Dominio.Entidades.Cliente tomador = LoteEscrituracaoEDIIntegracao.LoteEscrituracao.Tomador;
                Dominio.Entidades.Empresa empresa = LoteEscrituracaoEDIIntegracao.Empresa;

                int codigoLayoutEDI = LoteEscrituracaoEDIIntegracao.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;
                if (empresa != null)
                    configuracaoTransportador = empresa.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP).FirstOrDefault();

                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;

                if (configuracaoTransportador == null)
                    configuracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP).FirstOrDefault();

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                if (configuracaoCliente == null && configuracaoTransportador == null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP).FirstOrDefault();

                emails = configuracaoTransportador?.Emails ?? (configuracaoCliente?.Emails ?? configuracaoGrupoPessoas?.Emails);

                if (string.IsNullOrWhiteSpace(emails))
                {
                    LoteEscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "E-mail para envio dos documentos é inválido.";
                    return;
                }

                string extensao = string.Empty;

                using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(LoteEscrituracaoEDIIntegracao, tipoServicoMultisoftware, unidadeDeTrabalho, stringConexao, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(LoteEscrituracaoEDIIntegracao, extensao, unidadeDeTrabalho);

                    if (Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, "Arquivo EDI " + LoteEscrituracaoEDIIntegracao.LayoutEDI.Tipo.ToString("g") + " da escrituração Nº " + LoteEscrituracaoEDIIntegracao.LoteEscrituracao.Numero, "Segue em anexo o arquivo EDI referente ao lote de escrituração número " + LoteEscrituracaoEDIIntegracao.LoteEscrituracao.Numero + ".", email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(arquivoEDI, nomeArquivo, extensao == ".zip" ? "application/zip" : "text/plain") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        LoteEscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        LoteEscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + LoteEscrituracaoEDIIntegracao.LayoutEDI.Descricao + " para o(s) e-mail(s) '" + emails + "'";

                LoteEscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                LoteEscrituracaoEDIIntegracao.ProblemaIntegracao = mensagemErro;
                LoteEscrituracaoEDIIntegracao.DataIntegracao = DateTime.Now;
                LoteEscrituracaoEDIIntegracao.NumeroTentativas++;
            }
        }

        public static void EnviarEDI(ref Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (loteEscrituracaoCancelamentoEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                return;

            string mensagemErro = string.Empty,
                   emails = string.Empty;

            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email == null)
                {
                    loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "Não existe configuração de e-mail ativa para envio dos documentos.";
                    return;
                }

                Dominio.Entidades.Cliente tomador = loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.Tomador;
                Dominio.Entidades.Empresa empresa = loteEscrituracaoCancelamentoEDIIntegracao.Empresa;

                int codigoLayoutEDI = loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;
                if (empresa != null)
                    configuracaoTransportador = empresa.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP).FirstOrDefault();

                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;

                if (configuracaoTransportador == null)
                    configuracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP).FirstOrDefault();

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                if (configuracaoCliente == null && configuracaoTransportador == null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP).FirstOrDefault();

                emails = configuracaoTransportador?.Emails ?? configuracaoCliente?.Emails ?? configuracaoGrupoPessoas?.Emails;

                if (string.IsNullOrWhiteSpace(emails))
                {
                    loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "E-mail para envio dos documentos é inválido.";
                    return;
                }

                string extensao = string.Empty;

                using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(loteEscrituracaoCancelamentoEDIIntegracao, tipoServicoMultisoftware, unitOfWork, unitOfWork.StringConexao, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(loteEscrituracaoCancelamentoEDIIntegracao, extensao, unitOfWork);

                    if (Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, "Arquivo EDI " + loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Tipo.ToString("g") + " da escrituração de cancelamentos Nº " + loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.Numero, "Segue em anexo o arquivo EDI referente ao lote de escrituração de cancelamentos número " + loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.Numero + ".", email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(arquivoEDI, nomeArquivo, extensao == ".zip" ? "application/zip" : "text/plain") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Descricao + " para o(s) e-mail(s) '" + emails + "'";

                loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                loteEscrituracaoCancelamentoEDIIntegracao.ProblemaIntegracao = mensagemErro;
                loteEscrituracaoCancelamentoEDIIntegracao.DataIntegracao = DateTime.Now;
                loteEscrituracaoCancelamentoEDIIntegracao.NumeroTentativas++;
            }
        }

        public static void EnviarEDI(ref Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            if (cargaEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                return;

            string mensagemErro = string.Empty,
                   emails = string.Empty;

            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email == null)
                {
                    cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "Não existe configuração de e-mail ativa para envio dos documentos.";
                    return;
                }

                Dominio.Entidades.Cliente tomador = null;
                if (cargaEDIIntegracao.Pedidos != null && cargaEDIIntegracao.Pedidos.Count > 0)
                    tomador = cargaEDIIntegracao.Pedidos.First().ObterTomador();
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = cargaEDIIntegracao.Carga.TipoOperacao;
                Dominio.Entidades.Empresa empresa = cargaEDIIntegracao.Carga.Empresa;

                int codigoLayoutEDI = cargaEDIIntegracao.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoEmpresa = null;
                if (empresa != null && empresa.TransportadorLayoutsEDI != null && empresa.TransportadorLayoutsEDI.Count > 0)
                    configuracaoEmpresa = empresa.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacao = null;
                if (tipoOperacao != null)
                    configuracaoTipoOperacao = tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;
                if (configuracaoTipoOperacao == null && tomador != null)
                    configuracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;
                if (configuracaoCliente == null && configuracaoTipoOperacao == null && configuracaoEmpresa == null && tomador != null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();
                else if (configuracaoCliente == null && configuracaoTipoOperacao == null && configuracaoEmpresa == null && cargaEDIIntegracao.Carga.GrupoPessoaPrincipal != null)
                    configuracaoGrupoPessoas = cargaEDIIntegracao.Carga.GrupoPessoaPrincipal.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                emails = configuracaoEmpresa?.Emails ?? configuracaoTipoOperacao?.Emails ?? configuracaoGrupoPessoas?.Emails ?? (configuracaoCliente?.Emails ?? configuracaoGrupoPessoas?.Emails);

                if (string.IsNullOrWhiteSpace(emails))
                {
                    cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "E-mail para envio dos documentos é inválido.";
                    return;
                }

                string extensao = string.Empty;

                using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(cargaEDIIntegracao, tipoServicoMultisoftware, unidadeDeTrabalho, stringConexao, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(cargaEDIIntegracao, extensao, unidadeDeTrabalho);

                    if (Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, "Arquivo EDI " + cargaEDIIntegracao.LayoutEDI.Tipo.ToString("g") + " da Carga Nº " + cargaEDIIntegracao.Carga.CodigoCargaEmbarcador, "Segue em anexo o arquivo EDI referente à carga número " + cargaEDIIntegracao.Carga.CodigoCargaEmbarcador + ".", email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(arquivoEDI, nomeArquivo, extensao == ".zip" ? "application/zip" : "text/plain") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaEDIIntegracao.DestinoEnvio = emails;
                    }
                    else
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + cargaEDIIntegracao.LayoutEDI.Descricao + " para o(s) e-mail(s) '" + emails + "'";

                cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                cargaEDIIntegracao.ProblemaIntegracao = mensagemErro;
                cargaEDIIntegracao.DataIntegracao = DateTime.Now;
                cargaEDIIntegracao.NumeroTentativas++;
            }
        }

        public static void EnviarEDI(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao ocorrenciaEDIIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (ocorrenciaEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                return;

            string mensagemErro = string.Empty,
                   emails = string.Empty, notasFiscais = string.Empty;

            try
            {
                Dominio.Entidades.Cliente tomador = ocorrenciaEDIIntegracao.Pedidos.First().ObterTomador();
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = ocorrenciaEDIIntegracao.CargaOcorrencia.Carga != null ? ocorrenciaEDIIntegracao.CargaOcorrencia.Carga.TipoOperacao : null;
                emails = ObterEmails(tipoOperacao, tomador, ocorrenciaEDIIntegracao.LayoutEDI);
                if (string.IsNullOrWhiteSpace(emails))
                {
                    mensagemErro = "E-mail para envio dos documentos é inválido.";
                    return;
                }

                using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(ocorrenciaEDIIntegracao, unidadeDeTrabalho))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(ocorrenciaEDIIntegracao, false, unidadeDeTrabalho);

                    List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(arquivoEDI, nomeArquivo, "text/plain") };

                    if (ocorrenciaEDIIntegracao.CargaOcorrencia.TipoOcorrencia.OcorrenciaExclusivaParaCanhotosDigitalizados)
                    {
                        if (ocorrenciaEDIIntegracao.XMLNotaFiscal != null)
                        {
                            notasFiscais += ocorrenciaEDIIntegracao.XMLNotaFiscal.Numero;
                            string nomeArquivoCanhoto = IntegracaoEDI.ObterNomeArquivoEDI(ocorrenciaEDIIntegracao, false, unidadeDeTrabalho, true);
                            System.Net.Mail.Attachment anexo = retornarAnexoCanhoto(ocorrenciaEDIIntegracao.XMLNotaFiscal.Canhoto, System.IO.Path.GetFileNameWithoutExtension(nomeArquivoCanhoto), unidadeDeTrabalho);
                            if (anexo != null)
                                anexos.Add(anexo);
                        }
                        else
                        {
                            //todo: implementar envio multiplos canhotos se necessário (quando não temos uma integração por nota).
                        }
                    }

                    try
                    {
                        EnviarEmail(ocorrenciaEDIIntegracao.LayoutEDI, ocorrenciaEDIIntegracao.CargaOcorrencia.NumeroOcorrencia.ToString(), notasFiscais, emails, anexos, unidadeDeTrabalho);
                        mensagemErro = "Envio realizado com sucesso.";
                        ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    catch (Exception e)
                    {
                        mensagemErro = e.Message;
                        ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + ocorrenciaEDIIntegracao.LayoutEDI.Descricao + " para o(s) e-mail(s) '" + emails + "'";

                ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                ocorrenciaEDIIntegracao.ProblemaIntegracao = mensagemErro;
                ocorrenciaEDIIntegracao.DataIntegracao = DateTime.Now;
                ocorrenciaEDIIntegracao.NumeroTentativas++;
            }
        }

        public static void EnviarEmail(Dominio.Entidades.LayoutEDI layoutEDI, string numeroOcorrencia, string NumerosNotasFiscais, string emails, List<System.Net.Mail.Attachment> anexos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            if (email == null) throw new Exception("Não existe configuração de e-mail ativa para envio dos documentos.");

            string mensagemErro;
            string Notas = !string.IsNullOrEmpty(NumerosNotasFiscais) ? " Notas Fiscais: " + NumerosNotasFiscais : "";

            bool retorno = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, "Arquivo EDI " + layoutEDI.Tipo.ToString("g") + " da Ocorrência Nº " + numeroOcorrencia, "Segue em anexo o arquivo EDI referente à ocorrência número " + numeroOcorrencia + "." + Notas, email.Smtp, out mensagemErro, email.DisplayEmail, anexos, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
            if (!retorno) throw new Exception(mensagemErro);
        }

        public static string ObterEmails(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Cliente tomador, Dominio.Entidades.LayoutEDI layoutEDI)
        {
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacao = null;
            if (tipoOperacao != null)
                configuracaoTipoOperacao = tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == layoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

            Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;
            if (configuracaoTipoOperacao == null)
                configuracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == layoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;
            if (configuracaoTipoOperacao == null && configuracaoCliente == null)
                configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == layoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

            string emails = configuracaoGrupoPessoas?.Emails ?? (configuracaoCliente?.Emails ?? configuracaoTipoOperacao?.Emails);
            return emails;

        }

        public static void EnviarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (faturaIntegracao.TipoIntegracao.Tipo != TipoIntegracao.Email)
                return;
            if (faturaIntegracao.Fatura == null)
                return;

            List<int> codigosFatura = new List<int>();
            codigosFatura.Add(faturaIntegracao.Fatura.Codigo);

            Servicos.Embarcador.Fatura.Fatura.EnviarFaturaLote(codigosFatura, stringConexao, tipoServicoMultisoftware, faturaIntegracao, unidadeTrabalho);
        }

        public static void EnviarEDI(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (faturaIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                return;

            if (faturaIntegracao.Fatura == null || faturaIntegracao.LayoutEDI == null)
                return;

            string mensagemErro = string.Empty,
                   emails = string.Empty;

            try
            {
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unidadeTrabalho);
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeTrabalho);

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (email == null)
                {
                    mensagemErro = "Não existe configuração de e-mail ativa para envio dos documentos.";
                    return;
                }

                Dominio.Entidades.Cliente tomador = faturaIntegracao.Fatura.Cliente;

                int codigoLayoutEDI = faturaIntegracao.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                if (tomador != null)
                {
                    configuracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == faturaIntegracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();

                    if (configuracaoCliente == null)
                        configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == faturaIntegracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();
                }

                if (configuracaoCliente == null && faturaIntegracao.Fatura.GrupoPessoas != null)
                {
                    configuracaoGrupoPessoas = faturaIntegracao.Fatura.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Codigo == faturaIntegracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email).FirstOrDefault();
                }

                emails = configuracaoGrupoPessoas?.Emails ?? configuracaoCliente?.Emails;

                if (string.IsNullOrWhiteSpace(emails))
                {
                    mensagemErro = "E-mail para envio dos documentos é inválido.";
                    return;
                }

                using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(faturaIntegracao, unidadeTrabalho))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(faturaIntegracao, unidadeTrabalho, configuracaoTMS.UtilizaEmissaoMultimodal);

                    if (Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, "Arquivo EDI " + faturaIntegracao.LayoutEDI.Tipo.ToString("g") + " da Fatura Nº " + faturaIntegracao.Fatura.Numero, "Segue em anexo o arquivo EDI referente à fatura número " + faturaIntegracao.Fatura.Numero + ".", email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(arquivoEDI, nomeArquivo, "text/plain") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                        servFatura.InserirLog(faturaIntegracao.Fatura, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.EnviouEDI, faturaIntegracao.Fatura.Usuario, emails);
                        serCargaDadosSumarizados.AtualizarDadosCTesFaturadosIntegrados(faturaIntegracao.Fatura.Codigo, unidadeTrabalho);
                    }
                    else
                        faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + faturaIntegracao.LayoutEDI.Descricao + " para o(s) e-mail(s) '" + emails + "'";

                faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                faturaIntegracao.MensagemRetorno = mensagemErro;
                faturaIntegracao.DataEnvio = DateTime.Now;
                faturaIntegracao.Tentativas++;
            }
        }

        public static void EnviarEDI(Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI loteContabilizacaoIntegracaoEDI, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (loteContabilizacaoIntegracaoEDI.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                return;

            string mensagemErro = string.Empty,
                   emails = string.Empty;

            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email == null)
                {
                    loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "Não existe configuração de e-mail ativa para envio dos documentos.";
                    return;
                }

                Dominio.Entidades.Empresa empresa = loteContabilizacaoIntegracaoEDI.Empresa;

                int codigoLayoutEDI = loteContabilizacaoIntegracaoEDI.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;

                if (empresa != null)
                    configuracaoTransportador = empresa.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP).FirstOrDefault();

                emails = configuracaoTransportador?.Emails;

                if (string.IsNullOrWhiteSpace(emails))
                {
                    loteContabilizacaoIntegracaoEDI.NumeroTentativas++;
                    loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = "E-mail para envio dos documentos é inválido.";
                    return;
                }

                string extensao = string.Empty;

                using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(loteContabilizacaoIntegracaoEDI, tipoServicoMultisoftware, unitOfWork, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(loteContabilizacaoIntegracaoEDI, extensao, unitOfWork);

                    if (Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, "Arquivo EDI " + loteContabilizacaoIntegracaoEDI.LayoutEDI.Tipo.ToString("g") + " da contabilização nº " + loteContabilizacaoIntegracaoEDI.LoteContabilizacao.Numero, "Segue em anexo o arquivo EDI referente ao lote de contabilização número " + loteContabilizacaoIntegracaoEDI.LoteContabilizacao.Numero + ".", email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(arquivoEDI, nomeArquivo, extensao == ".zip" ? "application/zip" : "text/plain") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + loteContabilizacaoIntegracaoEDI.LayoutEDI.Descricao + " para o(s) e-mail(s) '" + emails + "'";

                loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                loteContabilizacaoIntegracaoEDI.ProblemaIntegracao = mensagemErro;
                loteContabilizacaoIntegracaoEDI.DataIntegracao = DateTime.Now;
                loteContabilizacaoIntegracaoEDI.NumeroTentativas++;
            }
        }

        public static void EnviarEmailDestiantarioTransportadorRetira(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, string titulo, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (pedido == null && (cargaPedido == null || cargaPedido.Carga == null))
                    return;

                Dominio.Entidades.Empresa empresa = pedido.Empresa;
                if (cargaPedido != null)
                    empresa = cargaPedido.Carga.Empresa;

                if (empresa == null || !empresa.EmpresaRetiradaProduto || !empresa.NotificarDestinatarioAgendamentoColeta)
                    return;

                if (string.IsNullOrEmpty(pedido?.Destinatario?.Email ?? ""))
                    return;

                List<Dominio.ObjetosDeValor.Email.Mensagem> mensagens = new List<Dominio.ObjetosDeValor.Email.Mensagem>();

                List<KeyValuePair<string, string>> linhas = new List<KeyValuePair<string, string>>();
                if (!string.IsNullOrWhiteSpace(cargaPedido?.Carga?.CodigoCargaEmbarcador ?? string.Empty))
                    linhas.Add(new KeyValuePair<string, string>("Carga", cargaPedido.Carga.CodigoCargaEmbarcador));

                linhas.Add(new KeyValuePair<string, string>("Nº Pedido", pedido.NumeroPedidoEmbarcador));
                linhas.Add(new KeyValuePair<string, string>("Transportador", empresa.Descricao));

                if (cargaPedido != null)
                {
                    if (cargaPedido.PrevisaoEntrega.HasValue)
                        linhas.Add(new KeyValuePair<string, string>("Previsão entrega", cargaPedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm")));

                    else if (cargaPedido.Carga.DataCarregamentoCarga.HasValue)
                        linhas.Add(new KeyValuePair<string, string>("Previsão entrega", cargaPedido.Carga.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm")));

                    else if (pedido.PrevisaoEntrega.HasValue)
                        linhas.Add(new KeyValuePair<string, string>("Previsão entrega", pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm")));
                }
                else if (pedido.PrevisaoEntrega.HasValue)
                    linhas.Add(new KeyValuePair<string, string>("Previsão entrega", pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm")));

                mensagens.Add(new Dominio.ObjetosDeValor.Email.Mensagem
                {
                    Destinatarios = new List<string> { pedido.Destinatario.Email },
                    Assunto = $"Pedido Nº {pedido.NumeroPedidoEmbarcador}",
                    Corpo = Servicos.Email.TemplateCorpoEmail(titulo, linhas, null, null, null, "E-mail enviado automaticamente.")
                });

                Servicos.Email.EnviarMensagensAsync(mensagens, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void EnviarEmailTransportadorRetira(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos, string titulo, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (carregamento == null || carregamento.Empresa == null)
                    return;

                if (string.IsNullOrEmpty(carregamento?.Empresa?.Email ?? ""))
                    return;

                List<Dominio.ObjetosDeValor.Email.Mensagem> mensagens = new List<Dominio.ObjetosDeValor.Email.Mensagem>();

                List<KeyValuePair<string, string>> linhas = new List<KeyValuePair<string, string>>();
                if (!string.IsNullOrWhiteSpace(carregamento.NumeroCarregamento))
                    linhas.Add(new KeyValuePair<string, string>("Carga", carregamento.NumeroCarregamento));
                linhas.Add(new KeyValuePair<string, string>("Transportador", carregamento.Empresa.Descricao));
                if (carregamento.DataCarregamentoCarga.HasValue)
                    linhas.Add(new KeyValuePair<string, string>("Data carregamento", carregamento.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm")));

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido in carregamentoPedidos)
                    linhas.Add(new KeyValuePair<string, string>("Nº Pedido", carregamentoPedido.Pedido.NumeroPedidoEmbarcador));

                mensagens.Add(new Dominio.ObjetosDeValor.Email.Mensagem
                {
                    Destinatarios = new List<string> { carregamento.Empresa.Email },
                    Assunto = $"Carregamento Nº {carregamento.NumeroCarregamento}",
                    Corpo = Servicos.Email.TemplateCorpoEmail(titulo, linhas, null, null, null, "E-mail enviado automaticamente.")
                });

                Servicos.Email.EnviarMensagensAsync(mensagens, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void EnviarEmailCargaTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (carga == null || carga.Filial == null)
                    return;

                if (string.IsNullOrEmpty(carga.Filial?.Email ?? ""))
                    return;

                List<Dominio.ObjetosDeValor.Email.Mensagem> mensagens = new List<Dominio.ObjetosDeValor.Email.Mensagem>();

                mensagens.Add(new Dominio.ObjetosDeValor.Email.Mensagem
                {
                    Destinatarios = new List<string> { carga.Filial?.Email ?? "" },
                    Assunto = $"Carga {carga.CodigoCargaEmbarcador} foi gerada pelo transportador {carga.Empresa.Descricao}",
                    Corpo = PreencherCorpoEmail(cargaPedidos)
                });

                Servicos.Email.EnviarMensagensAsync(mensagens, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }


        #endregion

        #region Métodos privados

        private static System.Net.Mail.Attachment retornarAnexoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, string nomeAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            System.Net.Mail.Attachment anexoCanhoto = null;

            if (canhoto != null)
            {
                string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

                string caminhoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.NomeArquivo);

                string nome = System.IO.Path.GetFileName(caminhoOriginal);
                if (!string.IsNullOrWhiteSpace(nomeAnexo))
                    nome = nomeAnexo + extensao;

                if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                {
                    byte[] bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);
                    Stream stream = new MemoryStream(bufferCanhoto);
                    anexoCanhoto = new System.Net.Mail.Attachment(stream, nome);
                }
            }
            return anexoCanhoto;
        }

        private static string PreencherCorpoEmail(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            StringBuilder corpoEmail = new StringBuilder();
            corpoEmail.AppendLine($"</span><span style=\"width: 100%; display: inline-block\">Carga gerada via Montagem de Carga com {cargaPedidos.Count} Pedido(s): </span>");
            corpoEmail.AppendLine("<table style =\"margin: 30px 0 30px 0; border: 1px solid #b9b5b5; border-collapse: collapse; border-collapse: collapse;\">");
            corpoEmail.AppendLine("<thead style=\"background-color: #d9e1f2; color: black;\">");
            corpoEmail.AppendLine("<tr>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Número Pedido </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Destinatário  </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Notas Fiscais </th>");
            corpoEmail.AppendLine("</tr>");
            corpoEmail.AppendLine("</thead>");
            corpoEmail.AppendLine("<tbody>");
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                corpoEmail.AppendLine("<tr>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{cargaPedido.Pedido?.NumeroPedidoEmbarcador ?? ""}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{cargaPedido.Pedido?.Destinatario?.Descricao ?? ""}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{string.Join(", ", cargaPedido.NotasFiscais?.Select(o => o.XMLNotaFiscal.Numero)) ?? ""}</td>");
                corpoEmail.AppendLine("</tr>");
            }
            corpoEmail.AppendLine("</tbody>");
            corpoEmail.AppendLine("</table>");

            corpoEmail.AppendLine("<small>Esse e-mail é gerado automaticamente. Não responda.</small>");

            return corpoEmail.ToString();
        }


        #endregion
    }
}
