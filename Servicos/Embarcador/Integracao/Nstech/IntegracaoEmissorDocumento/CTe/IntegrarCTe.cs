using Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool EnviarCteEmissor(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, string statusPosEmissao = "E")
        {
            bool sucesso = false;
            string mensagemErro = string.Empty;
            string id = string.Empty;

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unitOfWork);

            Dominio.Entidades.ErroSefaz erroSefaz = repErroSefaz.BuscarPorCodigoDoErro(8888, Dominio.Enumeradores.TipoErroSefaz.CTe);

            if (empresa == null)
                empresa = cte.Empresa;

            try
            {
                bool sincronizarDocumento = false;
                cte.LogIntegracao += "Enviado para o emissor NSTech (" + _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICte + "). ";
                cte.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS tipoMetodo = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.POST;
                object envioWS = null;
                string metodo = null;
                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Simplificado || cte.TipoCTE == Dominio.Enumeradores.TipoCTE.SimplificadoSubstituto)
                {
                    metodo = "cte-v4/simplified";
                    envioWS = this.obterEnvioCteSimplificado(cte, empresa, unitOfWork);
                }
                else
                {
                    metodo = "cte-v4";
                    envioWS = this.obterEnvioCte(cte, empresa, unitOfWork);
                }

                //Transmite
                var retornoWS = this.TransmitirEmissor(tipoMetodo, envioWS, metodo, _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICte);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Emissor NSTech: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    sucesso = false;

                    try
                    {
                        if (retornoWS.StatusCode == 400 && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                        {
                            dynamic objetoRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retornoWS.jsonRetorno);
                            if (objetoRetorno?.type == "document_is_authorized")
                            {
                                mensagemErro = string.Empty;
                                sucesso = true;
                                sincronizarDocumento = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Emissor NSTech: Ocorreu uma falha ao processar o retorno do envio do cte");
                        Servicos.Log.TratarErro(ex);
                    }
                }
                else
                {
                    dynamic retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<dynamic>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar retorno JSON CTe Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao efetuar o envio do cte; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        id = (string)retorno.id;
                        sucesso = true;
                    }
                }

                if (sucesso)
                {
                    cte.CodigoCTeIntegrador = 0;
                    cte.MensagemRetornoSefaz = "CT-e em processamento.";
                    cte.Status = statusPosEmissao;
                    cte.DataIntegracao = DateTime.Now;

                    repCTe.Atualizar(cte);

                    bool atualizarTipoEmpresa = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().AtualizarTipoEmpresa.Value;
                    if (atualizarTipoEmpresa && (cte.Usuario?.Callcenter ?? false) && cte.Empresa.StatusEmissao != "C")
                    {
                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                        cte.Empresa.StatusEmissao = "C";
                        repEmpresa.Atualizar(cte.Empresa);

                        Servicos.Log.TratarErro(cte.Empresa.Descricao + " alterada para callcenter devido a emissão do CTe " + cte.Descricao + " por " + cte.Usuario.Descricao, "StatusEmissaoEmpresa");
                    }

                    if (sincronizarDocumento)
                        this.ConsultarCte(cte, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unitOfWork);
                }
                else
                {
                    Servicos.Log.TratarErro(mensagemErro);
                    cte.MensagemRetornoSefaz = mensagemErro; //string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - ERRO: Sefaz indisponível no momento. Tente novamente.");
                    cte.MensagemStatus = null;
                    cte.Status = "R";
                }

                repCTe.Atualizar(cte);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Emissor NSTech: Ocorreu uma falha ao efetuar o envio do cte");
                Servicos.Log.TratarErro(ex);
                try
                {
                    cte.Status = "R";
                    cte.MensagemRetornoSefaz = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - ERRO: Sefaz indisponível no momento. Tente novamente.");
                    if (erroSefaz != null)
                        cte.MensagemStatus = erroSefaz;
                    repCTe.Atualizar(cte);

                    SalvarRetornoSefaz(cte, "A", 0, 8888, "ERRO: Sefaz indisponível no momento. Tente novamente.", unitOfWork);

                    enviarEmail(cte, ex.ToString(), unitOfWork);
                }
                catch (Exception exptEmail)
                {
                    Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
                    throw;
                }

                sucesso = false;
            }

            return sucesso;
        }


        #endregion

        #region Métodos Privados

        private void enviarEmail(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string ex, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.Email svcEmail = new Servicos.Email(unidadeTrabalho);

            string ambiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoAmbiente().IdentificacaoAmbiente;
            string assunto = ambiente + " - Problemas na emissão de CT-e!";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Atenção, problemas na emissão de CT-e no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
            sb.Append("CTe ").Append(cte.Numero).Append("/").Append(cte.Serie.Numero.ToString()).Append(" transportador ").Append(cte.Empresa.CNPJ).Append(" ").Append(cte.Empresa.RazaoSocial).Append("<br /> <br />");
            sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

            System.Text.StringBuilder ss = new System.Text.StringBuilder();
            ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
            Task.Factory.StartNew(() => svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unidadeTrabalho, 0, true, null, false));
#else
            Task.Factory.StartNew(() => svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "cesar@multisoftware.com.br", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unidadeTrabalho, 0, true, null, false));
            Task.Factory.StartNew(() => svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "cesar@multisoftware.com.br", "rodrigo@multisoftware.com.br", "willian@multisoftware.com.br", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unidadeTrabalho, 0, true, null, false));
#endif
        }

        private void SalvarRetornoSefaz(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string tipo, int codigoCTeIntegrador, int codigoRetornoSefaz, string mensagemRetornoSefaz, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho);
                Repositorio.CTeRetornoSefaz repCTeRetornoSefaz = new Repositorio.CTeRetornoSefaz(unidadeDeTrabalho);

                Dominio.Entidades.CTeRetornoSefaz cteRetornoSefaz = new Dominio.Entidades.CTeRetornoSefaz();

                string mensagemRetorno = codigoRetornoSefaz.ToString() + " - " + mensagemRetornoSefaz;

                cteRetornoSefaz.CTe = cte;
                cteRetornoSefaz.Tipo = tipo;
                cteRetornoSefaz.CodigoCTeIntegrador = codigoCTeIntegrador;
                cteRetornoSefaz.DataHora = DateTime.Now;
                cteRetornoSefaz.MensagemRetorno = mensagemRetorno.Length > 5000 ? mensagemRetorno.Substring(0, 5000) : mensagemRetorno;
                cteRetornoSefaz.CodStatusProtocolo = codigoRetornoSefaz.ToString();
                cteRetornoSefaz.ErroSefaz = repErroSefaz.BuscarPorCodigoDoErro(codigoRetornoSefaz, Dominio.Enumeradores.TipoErroSefaz.CTe);
                repCTeRetornoSefaz.Inserir(cteRetornoSefaz);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao salvar retorno sefaz CTe: " + ex);
            }
        }


        #region CT-e Padrão

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cte obterEnvioCte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cte retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cte();
            retorno.data = this.obterEnvioCteData(cte, empresa, unitOfWork);
            retorno.options = this.obterEnvioCteOptions(cte);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteData obterEnvioCteData(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteData retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteData();
            retorno.externalId = cte.Codigo.ToString();
            retorno.operationNatureCode = cte.CFOP.CodigoCFOP.ToString();
            retorno.operationNature = Utilidades.String.Left(cte.NaturezaDaOperacao.Descricao, 60);
            retorno.numericCode = Convert.ToInt64(Utilidades.String.Left(cte.Numero.ToString(), 7) + "9");
            retorno.serie = cte.Serie.Numero;
            retorno.number = cte.Numero;

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(cte.DataEmissao.HasValue ? cte.DataEmissao.Value : DateTime.Today);
            string fusohorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);
            retorno.issueDate = ((DateTime)cte.DataEmissao).ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "T") + fusohorario;

            retorno.printFormat = this.obterPrintFormat(cte.TipoImpressao);
            retorno.cteType = this.obterCteType(cte.TipoCTE);
            retorno.issueType = this.obterIssueType(cte.TipoEmissao, cte);
            retorno.globalized = cte.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            retorno.modal = this.obterModal(cte.ModalTransporte);
            retorno.serviceType = this.obterServiceType(cte.TipoServico);
            retorno.originCity = this.obterEnvioCteOriginCity(cte.LocalidadeInicioPrestacao);
            retorno.destinationCity = this.obterEnvioCteDestinationCity(cte.LocalidadeTerminoPrestacao);
            retorno.pickup = cte.Retira == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            retorno.pickupDetail = !string.IsNullOrWhiteSpace(cte.DetalhesRetira) ? cte.DetalhesRetira.Length > 80 ? cte.DetalhesRetira.Substring(0, 80) : cte.DetalhesRetira : null;
            retorno.takerTaxIdication = this.obterTakerTaxIdication(cte.IndicadorIETomador);
            retorno.issuer = this.obterEnvioCteIssuer(cte, unitOfWork);

            if (cte.TipoServico != Dominio.Enumeradores.TipoServico.RedIntermediario)
            {
                retorno.sender = this.obterEnvioCteSender(cte.ObterParticipante(Dominio.Enumeradores.TipoTomador.Remetente));
                retorno.recipient = this.obterEnvioCteRecipient(cte.ObterParticipante(Dominio.Enumeradores.TipoTomador.Destinatario));
            }

            retorno.shipper = this.obterEnvioCteShipper(cte.ObterParticipante(Dominio.Enumeradores.TipoTomador.Expedidor));
            retorno.receiver = this.obterEnvioCteReceiver(cte.ObterParticipante(Dominio.Enumeradores.TipoTomador.Recebedor));
            retorno.taker = this.obterEnvioCteTaker(cte);
            retorno.additionalData = this.obterEnvioCteAdditionalData(cte, empresa, unitOfWork);

            decimal valorTotalComponetes = 0;
            retorno.serviceAmount = this.obterEnvioCteServiceAmount(out valorTotalComponetes, cte, empresa, unitOfWork);
            retorno.tax = this.obterEnvioCteTax(valorTotalComponetes, cte, empresa, unitOfWork);
            retorno.cteNormal = this.obterEnvioCteNormal(cte, empresa, unitOfWork);
            retorno.cteComplement = this.obterEnvioCteComplement(cte);
            retorno.downloadAuthorization = this.obterEnvioCteDownloadAuthorization(cte);
            retorno.technicalManager = this.obterEnvioCteTechnicalManager(cte);

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptions obterEnvioCteOptions(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptions retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptions();
            retorno.removeSpecialsChars = true;
            retorno.dacte = this.obterEnvioCteOptionsDacte(cte);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacte obterEnvioCteOptionsDacte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacte retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacte();
            retorno.enabled = true;
            retorno.notifications = this.obterEnvioCteOptionsDacteNotifications(cte);

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacteNotifications obterEnvioCteOptionsDacteNotifications(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string emails = null)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacteNotifications retorno = null;

            if (string.IsNullOrWhiteSpace(emails))
            {
                emails = obterEmailParaEnvioDACTE(cte.Empresa);

                Dominio.Entidades.ParticipanteCTe tomador = cte.ObterParticipante(cte.TipoTomador);
                if (tomador != null)
                {
                    if (!tomador.Exterior)
                        emails += this.obterEmailParaEnvioDACTE(tomador);
                    else
                        emails += tomador.EmailStatus && !string.IsNullOrWhiteSpace(tomador.Email) ? tomador.Email : null;
                }

                if (string.IsNullOrEmpty(emails))
                    return retorno;
            }

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacteNotificationsContacts> contatos = this.obterEnvioCteOptionsDacteNotificationsContacts(emails);

            if (contatos?.Count > 0)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacteNotifications();
                retorno.enabled = true;
                retorno.contacts = contatos;
            }


            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacteNotificationsContacts> obterEnvioCteOptionsDacteNotificationsContacts(string emails)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacteNotificationsContacts> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacteNotificationsContacts>();

            List<string> emailLista = emails.Split(';').ToList();
            foreach (string email in emailLista)
            {
                if (!string.IsNullOrEmpty(email))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacteNotificationsContacts contato = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptionsDacteNotificationsContacts();
                    contato.type = "email";
                    contato.email = email.Trim();
                    retorno.Add(contato);
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataOriginCity obterEnvioCteOriginCity(Dominio.Entidades.Localidade localidadeInicioPrestacao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataOriginCity retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataOriginCity();
            retorno.name = Utilidades.String.Left(localidadeInicioPrestacao.Descricao, 60);
            retorno.code = localidadeInicioPrestacao.CodigoIBGE.ToString();
            retorno.state = localidadeInicioPrestacao.Estado.Sigla;
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataDestinationCity obterEnvioCteDestinationCity(Dominio.Entidades.Localidade localidadeTerminoPrestacao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataDestinationCity retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataDestinationCity();
            retorno.name = localidadeTerminoPrestacao.CodigoIBGE == 9999999 ? "EXTERIOR" : Utilidades.String.Left(localidadeTerminoPrestacao.Descricao, 60);
            retorno.code = localidadeTerminoPrestacao.CodigoIBGE.ToString();
            retorno.state = localidadeTerminoPrestacao.Estado.Sigla;
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataIssuer obterEnvioCteIssuer(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataIssuer retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataIssuer();
            Dominio.Entidades.Empresa empresa = cte.Empresa;

            retorno.type = empresa.Tipo == "F" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
            retorno.document = Utilidades.String.OnlyNumbers(empresa.CNPJ);
            retorno.name = Utilidades.String.Left(empresa.RazaoSocial, 60);
            retorno.tradeName = Utilidades.String.Left(empresa.NomeFantasia, 60);
            retorno.phone = !string.IsNullOrWhiteSpace(empresa.Telefone) ? Utilidades.String.Left(Utilidades.String.OnlyNumbers(empresa.Telefone), 14) : null;
            retorno.stateRegistration = string.IsNullOrWhiteSpace(empresa.InscricaoEstadual) ? "ISENTO" : Utilidades.String.Left(empresa.InscricaoEstadual, 14);
            retorno.stateRegistrationST = this.obterInscricaoST(cte.Empresa, cte.LocalidadeInicioPrestacao, unitOfWork);
            retorno.taxRegime = obterTaxRegime(cte.Empresa.RegimeTributarioCTe);

            retorno.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataIssuerAddress()
            {
                street = String.IsNullOrWhiteSpace(empresa.Endereco) ? "RUA" : empresa.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(empresa.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(empresa.Endereco), 255),
                number = !string.IsNullOrWhiteSpace(empresa.Numero) ? Utilidades.String.Left(empresa.Numero.Trim(), 60) : "S/N",
                complement = !String.IsNullOrWhiteSpace(empresa.Complemento) && empresa.Complemento.Length > 2 ? Utilidades.String.Left(empresa.Complemento.Trim(), 60) : null,
                neighborhood = String.IsNullOrWhiteSpace(empresa.Bairro) ? "BAIRRO" : empresa.Bairro.Length < 2 ? "BAIRRO " + empresa.Bairro : Utilidades.String.Left(empresa.Bairro.Trim(), 60),
                zipCode = this.obterCEP(empresa.CEP),
                city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataIssuerAddressCity()
                {
                    code = empresa.Localidade.CodigoIBGE.ToString(),
                    name = Utilidades.String.Left(empresa.Localidade.Descricao, 60),
                    state = empresa.Localidade.Estado.Sigla,
                }
            };

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataSender obterEnvioCteSender(Dominio.Entidades.ParticipanteCTe remetente)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataSender retorno = null;

            if (remetente != null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataSender();

                if (!remetente.Exterior)
                {
                    retorno.type = remetente.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.legal : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.individual;
                    retorno.document = remetente.CPF_CNPJ_SemFormato;
                    retorno.name = Utilidades.String.Left(remetente.Nome, 60);
                    retorno.tradeName = Utilidades.String.Left(remetente.NomeFantasia, 60);
                    retorno.phone = !string.IsNullOrWhiteSpace(remetente.Telefone1) ? Utilidades.String.Left(Utilidades.String.OnlyNumbers(remetente.Telefone1), 14) : null;
                    retorno.email = this.obterEmailParticipante(remetente);
                    retorno.stateRegistration = Utilidades.String.Left(remetente.IE_RG, 14);

                    retorno.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataSenderAddress()
                    {
                        street = String.IsNullOrWhiteSpace(remetente.Endereco) ? "RUA" : remetente.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(remetente.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(remetente.Endereco), 255),
                        number = !string.IsNullOrWhiteSpace(remetente.Numero) ? Utilidades.String.Left(remetente.Numero.Trim(), 60) : "S/N",
                        complement = !String.IsNullOrWhiteSpace(remetente.Complemento) && remetente.Complemento.Length > 2 ? Utilidades.String.Left(remetente.Complemento.Trim(), 60) : null,
                        neighborhood = String.IsNullOrWhiteSpace(remetente.Bairro) ? "BAIRRO" : remetente.Bairro.Length < 2 ? "BAIRRO " + remetente.Bairro : Utilidades.String.Left(remetente.Bairro.Trim(), 60),
                        zipCode = this.obterCEP(remetente.CEP),
                        city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataSenderAddressCity()
                        {
                            code = remetente.Localidade?.CodigoIBGE.ToString(),
                            name = Utilidades.String.Left(remetente.Localidade.Descricao, 60),
                            state = remetente.Localidade.Estado.Sigla,
                        },
                        country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataSenderAddressCountry()
                        {
                            name = remetente.Pais != null ? Utilidades.String.Left(remetente.Pais.Nome, 60) : Utilidades.String.Left(remetente.Localidade.Estado.Pais.Nome, 60),
                            code = remetente.Pais != null ? remetente.Pais.Codigo.ToString() : "1058",
                        }
                    };
                }
                else
                {
                    retorno.type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.foreign;
                    retorno.name = Utilidades.String.Left(remetente.Nome, 60);
                    retorno.tradeName = Utilidades.String.Left(remetente.NomeFantasia, 60);
                    retorno.phone = null;
                    retorno.email = this.obterEmailParticipante(remetente);
                    retorno.stateRegistration = Utilidades.String.Left(remetente.IE_RG, 14);

                    retorno.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataSenderAddress()
                    {
                        street = String.IsNullOrWhiteSpace(remetente.Endereco) ? "RUA" : remetente.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(remetente.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(remetente.Endereco), 255),
                        number = !string.IsNullOrWhiteSpace(remetente.Numero) ? Utilidades.String.Left(remetente.Numero.Trim(), 60) : "S/N",
                        complement = !String.IsNullOrWhiteSpace(remetente.Complemento) && remetente.Complemento.Length > 2 ? Utilidades.String.Left(remetente.Complemento.Trim(), 60) : null,
                        neighborhood = String.IsNullOrWhiteSpace(remetente.Bairro) ? "BAIRRO" : remetente.Bairro.Length < 2 ? "BAIRRO " + remetente.Bairro : Utilidades.String.Left(remetente.Bairro.Trim(), 60),
                        zipCode = null,
                        city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataSenderAddressCity()
                        {
                            code = "9999999",
                            name = Utilidades.String.Left(remetente.Cidade, 60),
                            state = "EX"
                        },
                        country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataSenderAddressCountry()
                        {
                            name = remetente.Pais != null ? Utilidades.String.Left(remetente.Pais.Nome, 60) : "EXTERIOR",
                            code = remetente.Pais != null ? remetente.Pais?.Codigo.ToString() : "1058",
                        }
                    };
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataRecipient obterEnvioCteRecipient(Dominio.Entidades.ParticipanteCTe destinatario)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataRecipient retorno = null;

            if (destinatario != null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataRecipient();

                if (!destinatario.Exterior)
                {
                    retorno.type = destinatario.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.legal : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.individual;
                    retorno.document = destinatario.CPF_CNPJ_SemFormato;
                    retorno.name = Utilidades.String.Left(destinatario.Nome, 60);
                    retorno.phone = !string.IsNullOrWhiteSpace(destinatario.Telefone1) ? Utilidades.String.Left(Utilidades.String.OnlyNumbers(destinatario.Telefone1), 14) : null;
                    retorno.email = this.obterEmailParticipante(destinatario);
                    retorno.stateRegistration = Utilidades.String.Left(destinatario.IE_RG, 14);
                    retorno.suframaId = string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(destinatario.InscricaoSuframa)) ? null : Utilidades.String.OnlyNumbers(destinatario.InscricaoSuframa);

                    retorno.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataRecipientAddress()
                    {
                        street = String.IsNullOrWhiteSpace(destinatario.Endereco) ? "RUA" : destinatario.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(destinatario.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(destinatario.Endereco), 255),
                        number = !string.IsNullOrWhiteSpace(destinatario.Numero) ? Utilidades.String.Left(destinatario.Numero.Trim(), 60) : "S/N",
                        complement = !String.IsNullOrWhiteSpace(destinatario.Complemento) && destinatario.Complemento.Length > 2 ? Utilidades.String.Left(destinatario.Complemento.Trim(), 60) : null,
                        neighborhood = String.IsNullOrWhiteSpace(destinatario.Bairro) ? "BAIRRO" : destinatario.Bairro.Length < 2 ? "BAIRRO " + destinatario.Bairro : Utilidades.String.Left(destinatario.Bairro.Trim(), 60),
                        zipCode = this.obterCEP(destinatario.CEP),
                        city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataRecipientAddressCity()
                        {
                            code = destinatario.Localidade?.CodigoIBGE.ToString(),
                            name = Utilidades.String.Left(destinatario.Localidade.Descricao, 60),
                            state = destinatario.Localidade.Estado.Sigla,
                        },
                        country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataRecipientAddressCountry()
                        {
                            name = destinatario.Pais != null ? Utilidades.String.Left(destinatario.Pais.Nome, 60) : Utilidades.String.Left(destinatario.Localidade.Estado.Pais.Nome, 60),
                            code = destinatario.Pais != null ? destinatario.Pais.Codigo.ToString() : "1058",
                        }
                    };
                }
                else
                {
                    retorno.type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.foreign;
                    retorno.name = Utilidades.String.Left(destinatario.Nome, 60);
                    retorno.phone = null;
                    retorno.email = this.obterEmailParticipante(destinatario);
                    retorno.stateRegistration = Utilidades.String.Left(destinatario.IE_RG, 14);

                    retorno.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataRecipientAddress()
                    {
                        street = String.IsNullOrWhiteSpace(destinatario.Endereco) ? "RUA" : destinatario.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(destinatario.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(destinatario.Endereco), 255),
                        number = !string.IsNullOrWhiteSpace(destinatario.Numero) ? Utilidades.String.Left(destinatario.Numero.Trim(), 60) : "S/N",
                        complement = !String.IsNullOrWhiteSpace(destinatario.Complemento) && destinatario.Complemento.Length > 2 ? Utilidades.String.Left(destinatario.Complemento.Trim(), 60) : null,
                        neighborhood = String.IsNullOrWhiteSpace(destinatario.Bairro) ? "BAIRRO" : destinatario.Bairro.Length < 2 ? "BAIRRO " + destinatario.Bairro : Utilidades.String.Left(destinatario.Bairro.Trim(), 60),
                        zipCode = null,
                        city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataRecipientAddressCity()
                        {
                            code = "9999999",
                            name = Utilidades.String.Left(destinatario.Cidade, 60),
                            state = "EX"
                        },
                        country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataRecipientAddressCountry()
                        {
                            name = destinatario.Pais != null ? Utilidades.String.Left(destinatario.Pais.Nome, 60) : "EXTERIOR",
                            code = destinatario.Pais != null ? destinatario.Pais?.Codigo.ToString() : "1058"
                        }
                    };
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataShipper obterEnvioCteShipper(Dominio.Entidades.ParticipanteCTe expedidor)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataShipper retorno = null;

            if (expedidor != null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataShipper();

                if (!expedidor.Exterior)
                {
                    retorno.type = expedidor.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.legal : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.individual;
                    retorno.document = expedidor.CPF_CNPJ_SemFormato;
                    retorno.name = Utilidades.String.Left(expedidor.Nome, 60);
                    retorno.phone = !string.IsNullOrWhiteSpace(expedidor.Telefone1) ? Utilidades.String.Left(Utilidades.String.OnlyNumbers(expedidor.Telefone1), 14) : null;
                    retorno.email = this.obterEmailParticipante(expedidor);
                    retorno.stateRegistration = Utilidades.String.Left(expedidor.IE_RG, 14);

                    retorno.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataShipperAddress()
                    {
                        street = String.IsNullOrWhiteSpace(expedidor.Endereco) ? "RUA" : expedidor.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(expedidor.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(expedidor.Endereco), 255),
                        number = !string.IsNullOrWhiteSpace(expedidor.Numero) ? Utilidades.String.Left(expedidor.Numero.Trim(), 60) : "S/N",
                        complement = !String.IsNullOrWhiteSpace(expedidor.Complemento) && expedidor.Complemento.Length > 2 ? Utilidades.String.Left(expedidor.Complemento.Trim(), 60) : null,
                        neighborhood = String.IsNullOrWhiteSpace(expedidor.Bairro) ? "BAIRRO" : expedidor.Bairro.Length < 2 ? "BAIRRO " + expedidor.Bairro : Utilidades.String.Left(expedidor.Bairro.Trim(), 60),
                        zipCode = this.obterCEP(expedidor.CEP),
                        city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataShipperAddressCity()
                        {
                            code = expedidor.Localidade?.CodigoIBGE.ToString(),
                            name = Utilidades.String.Left(expedidor.Localidade.Descricao, 60),
                            state = expedidor.Localidade.Estado.Sigla,
                        },
                        country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataShipperAddressCountry()
                        {
                            name = expedidor.Pais != null ? Utilidades.String.Left(expedidor.Pais.Nome, 60) : Utilidades.String.Left(expedidor.Localidade.Estado.Pais.Nome, 60),
                            code = expedidor.Pais != null ? expedidor.Pais.Codigo.ToString() : "1058",
                        }
                    };
                }
                else
                {
                    retorno.type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.foreign;
                    retorno.name = Utilidades.String.Left(expedidor.Nome, 60);
                    retorno.phone = null;
                    retorno.email = this.obterEmailParticipante(expedidor);
                    retorno.stateRegistration = Utilidades.String.Left(expedidor.IE_RG, 14);

                    retorno.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataShipperAddress()
                    {
                        street = String.IsNullOrWhiteSpace(expedidor.Endereco) ? "RUA" : expedidor.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(expedidor.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(expedidor.Endereco), 255),
                        number = !string.IsNullOrWhiteSpace(expedidor.Numero) ? Utilidades.String.Left(expedidor.Numero.Trim(), 60) : "S/N",
                        complement = !String.IsNullOrWhiteSpace(expedidor.Complemento) && expedidor.Complemento.Length > 2 ? Utilidades.String.Left(expedidor.Complemento.Trim(), 60) : null,
                        neighborhood = String.IsNullOrWhiteSpace(expedidor.Bairro) ? "BAIRRO" : expedidor.Bairro.Length < 2 ? "BAIRRO " + expedidor.Bairro : Utilidades.String.Left(expedidor.Bairro.Trim(), 60),
                        zipCode = null,
                        city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataShipperAddressCity()
                        {
                            code = "9999999",
                            name = Utilidades.String.Left(expedidor.Cidade, 60),
                            state = "EX"
                        },
                        country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataShipperAddressCountry()
                        {
                            name = expedidor.Pais != null ? Utilidades.String.Left(expedidor.Pais.Nome, 60) : "EXTERIOR",
                            code = expedidor.Pais != null ? expedidor.Pais?.Codigo.ToString() : "1058",
                        }
                    };
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataReceiver obterEnvioCteReceiver(Dominio.Entidades.ParticipanteCTe recebedor)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataReceiver retorno = null;

            if (recebedor != null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataReceiver();

                if (!recebedor.Exterior)
                {
                    retorno.type = recebedor.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.legal : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.individual;
                    retorno.document = recebedor.CPF_CNPJ_SemFormato;
                    retorno.name = Utilidades.String.Left(recebedor.Nome, 60);
                    retorno.phone = !string.IsNullOrWhiteSpace(recebedor.Telefone1) ? Utilidades.String.Left(Utilidades.String.OnlyNumbers(recebedor.Telefone1), 14) : null;
                    retorno.email = this.obterEmailParticipante(recebedor);
                    retorno.stateRegistration = Utilidades.String.Left(recebedor.IE_RG, 14);

                    retorno.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataReceiverAddress()
                    {
                        street = String.IsNullOrWhiteSpace(recebedor.Endereco) ? "RUA" : recebedor.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(recebedor.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(recebedor.Endereco), 255),
                        number = !string.IsNullOrWhiteSpace(recebedor.Numero) ? Utilidades.String.Left(recebedor.Numero.Trim(), 60) : "S/N",
                        complement = !String.IsNullOrWhiteSpace(recebedor.Complemento) && recebedor.Complemento.Length > 2 ? Utilidades.String.Left(recebedor.Complemento.Trim(), 60) : null,
                        neighborhood = String.IsNullOrWhiteSpace(recebedor.Bairro) ? "BAIRRO" : recebedor.Bairro.Length < 2 ? "BAIRRO " + recebedor.Bairro : Utilidades.String.Left(recebedor.Bairro.Trim(), 60),
                        zipCode = this.obterCEP(recebedor.CEP),
                        city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataReceiverAddressCity()
                        {
                            code = recebedor.Localidade?.CodigoIBGE.ToString(),
                            name = Utilidades.String.Left(recebedor.Localidade.Descricao, 60),
                            state = recebedor.Localidade.Estado.Sigla,
                        },
                        country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataReceiverAddressCountry()
                        {
                            name = recebedor.Pais != null ? Utilidades.String.Left(recebedor.Pais.Nome, 60) : Utilidades.String.Left(recebedor.Localidade.Estado.Pais.Nome, 60),
                            code = recebedor.Pais != null ? recebedor.Pais.Codigo.ToString() : "1058",
                        }
                    };
                }
                else
                {
                    retorno.type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.foreign;
                    retorno.name = Utilidades.String.Left(recebedor.Nome, 60);
                    retorno.phone = null;
                    retorno.email = this.obterEmailParticipante(recebedor);
                    retorno.stateRegistration = Utilidades.String.Left(recebedor.IE_RG, 14);

                    retorno.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataReceiverAddress()
                    {
                        street = String.IsNullOrWhiteSpace(recebedor.Endereco) ? "RUA" : recebedor.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(recebedor.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(recebedor.Endereco), 255),
                        number = !string.IsNullOrWhiteSpace(recebedor.Numero) ? Utilidades.String.Left(recebedor.Numero.Trim(), 60) : "S/N",
                        complement = !String.IsNullOrWhiteSpace(recebedor.Complemento) && recebedor.Complemento.Length > 2 ? Utilidades.String.Left(recebedor.Complemento.Trim(), 60) : null,
                        neighborhood = String.IsNullOrWhiteSpace(recebedor.Bairro) ? "BAIRRO" : recebedor.Bairro.Length < 2 ? "BAIRRO " + recebedor.Bairro : Utilidades.String.Left(recebedor.Bairro.Trim(), 60),
                        zipCode = null,
                        city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataReceiverAddressCity()
                        {
                            code = "9999999",
                            name = Utilidades.String.Left(recebedor.Cidade, 60),
                            state = "EX"
                        },
                        country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataReceiverAddressCountry()
                        {
                            name = recebedor.Pais != null ? Utilidades.String.Left(recebedor.Pais.Nome, 60) : "EXTERIOR",
                            code = recebedor.Pais != null ? recebedor.Pais?.Codigo.ToString() : "1058",
                        }
                    };
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaker obterEnvioCteTaker(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaker retorno = null;

            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                Dominio.Entidades.ParticipanteCTe tomador = cte.ObterParticipante(Dominio.Enumeradores.TipoTomador.Outros);
                if (tomador != null)
                {
                    retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaker();
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTaker outroTomador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTaker();

                    if (!tomador.Exterior)
                    {
                        outroTomador.type = tomador.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.legal : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.individual;
                        outroTomador.document = tomador.CPF_CNPJ_SemFormato;
                        outroTomador.name = Utilidades.String.Left(tomador.Nome, 60);
                        outroTomador.tradeName = Utilidades.String.Left(tomador.NomeFantasia, 60);
                        outroTomador.phone = !string.IsNullOrWhiteSpace(tomador.Telefone1) ? Utilidades.String.Left(Utilidades.String.OnlyNumbers(tomador.Telefone1), 14) : null;
                        outroTomador.email = this.obterEmailParticipante(tomador);
                        outroTomador.stateRegistration = Utilidades.String.Left(tomador.IE_RG, 14);

                        outroTomador.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddress()
                        {
                            street = String.IsNullOrWhiteSpace(tomador.Endereco) ? "RUA" : tomador.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(tomador.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(tomador.Endereco), 255),
                            number = !string.IsNullOrWhiteSpace(tomador.Numero) ? Utilidades.String.Left(tomador.Numero.Trim(), 60) : "S/N",
                            complement = !String.IsNullOrWhiteSpace(tomador.Complemento) && tomador.Complemento.Length > 2 ? Utilidades.String.Left(tomador.Complemento.Trim(), 60) : null,
                            neighborhood = String.IsNullOrWhiteSpace(tomador.Bairro) ? "BAIRRO" : tomador.Bairro.Length < 2 ? "BAIRRO " + tomador.Bairro : Utilidades.String.Left(tomador.Bairro.Trim(), 60),
                            zipCode = this.obterCEP(tomador.CEP),
                            city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddressCity()
                            {
                                code = tomador.Localidade?.CodigoIBGE.ToString(),
                                name = Utilidades.String.Left(tomador.Localidade.Descricao, 60),
                                state = tomador.Localidade.Estado.Sigla,
                            },
                            country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddressCountry()
                            {
                                name = tomador.Pais != null ? Utilidades.String.Left(tomador.Pais.Nome, 60) : Utilidades.String.Left(tomador.Localidade.Estado.Pais.Nome, 60),
                                code = tomador.Pais != null ? tomador.Pais.Codigo.ToString() : "1058",
                            }
                        };
                    }
                    else
                    {
                        outroTomador.type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.foreign;
                        outroTomador.name = Utilidades.String.Left(tomador.Nome, 60);
                        outroTomador.tradeName = Utilidades.String.Left(tomador.NomeFantasia, 60);
                        outroTomador.phone = null;
                        outroTomador.email = tomador.EmailStatus && !string.IsNullOrWhiteSpace(tomador.Email) ? obterPrimeiroEmail(tomador.Email) : null;
                        outroTomador.stateRegistration = Utilidades.String.Left(tomador.IE_RG, 14);

                        outroTomador.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddress()
                        {
                            street = String.IsNullOrWhiteSpace(tomador.Endereco) ? "RUA" : tomador.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(tomador.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(tomador.Endereco), 255),
                            number = !string.IsNullOrWhiteSpace(tomador.Numero) ? Utilidades.String.Left(tomador.Numero.Trim(), 60) : "S/N",
                            complement = !String.IsNullOrWhiteSpace(tomador.Complemento) && tomador.Complemento.Length > 2 ? Utilidades.String.Left(tomador.Complemento.Trim(), 60) : null,
                            neighborhood = String.IsNullOrWhiteSpace(tomador.Bairro) ? "BAIRRO" : tomador.Bairro.Length < 2 ? "BAIRRO " + tomador.Bairro : Utilidades.String.Left(tomador.Bairro.Trim(), 60),
                            zipCode = null,
                            city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddressCity()
                            {
                                code = "9999999",
                                name = Utilidades.String.Left(tomador.Cidade, 60),
                                state = "EX"
                            },
                            country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddressCountry()
                            {
                                name = tomador.Pais != null ? Utilidades.String.Left(tomador.Pais.Nome, 60) : "EXTERIOR",
                                code = tomador.Pais != null ? tomador.Pais?.Codigo.ToString() : "1058",
                            }
                        };
                    }

                    retorno.type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType.others;
                    retorno.taker = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTaker();
                    retorno.taker = outroTomador;
                }
            }
            else
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaker();
                retorno.type = this.obterTakerType(cte.TipoTomador);
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalData obterEnvioCteAdditionalData(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalData retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalData();

            retorno.additionalCharacteristicTransport = !string.IsNullOrEmpty(cte.CaracteristicaTransporte) ? cte.CaracteristicaTransporte : null;
            retorno.additionalCharacteristicService = !string.IsNullOrEmpty(cte.CaracteristicaServico) ? cte.CaracteristicaServico : null;
            retorno.issuingEmployee = Utilidades.String.Left(cte.Usuario?.Nome, 20) ?? null;
            retorno.flow = this.obterEnvioCteAdditionalDataFlow(cte);
            retorno.issuerObservation = this.obterEnvioCteAdditionalDataIssuerObservation(cte, empresa, unitOfWork);
            retorno.fiscalObservation = this.obterEnvioCteAdditionalDataFiscalObservation(cte, empresa, unitOfWork);
            retorno.deliveryDate = this.obterEnvioCteAdditionalDataFiscalDeliveryDate(cte);
            retorno.deliveryTime = this.obterEnvioCteAdditionalDataFiscalDeliveryTime(cte);
            retorno.commercialOriginCity = null;
            retorno.commercialDestinationCity = null;

            retorno.observation = Utilidades.String.ReplaceInvalidCharacters(cte.ObservacoesGerais);
            if (!string.IsNullOrWhiteSpace(cte.ObservacoesAvancadas))
                retorno.observation += (string.IsNullOrWhiteSpace(retorno.observation) ? "" : " - ") + cte.ObservacoesAvancadas.Trim();

            if (string.IsNullOrEmpty(retorno.observation))
                retorno.observation = null;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlow obterEnvioCteAdditionalDataFlow(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlow retorno = null;

            if (!string.IsNullOrEmpty(cte.PortoOrigem?.Descricao))
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlow();
                retorno.originAcronym = Utilidades.String.Left(cte.PortoOrigem?.Descricao, 60) ?? "";
                retorno.transit = this.obterEnvioCteAdditionalDataFlowTransit(cte);
                retorno.destinationAcronym = Utilidades.String.Left(cte.PortoDestino?.Descricao, 60) ?? "";
                retorno.deliveryRoute = null;
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit> obterEnvioCteAdditionalDataFlowTransit(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit> retorno = null;

            if (!string.IsNullOrEmpty(cte.PortoPassagemUm?.Descricao))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit>();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit();
                ponto.transitAcronym = Utilidades.String.Left(cte.PortoPassagemUm?.Descricao, 15);
                retorno.Add(ponto);
            }

            if (!string.IsNullOrEmpty(cte.PortoPassagemDois?.Descricao))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit>();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit();
                ponto.transitAcronym = Utilidades.String.Left(cte.PortoPassagemDois?.Descricao, 15);
                retorno.Add(ponto);
            }

            if (!string.IsNullOrEmpty(cte.PortoPassagemTres?.Descricao))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit>();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit();
                ponto.transitAcronym = Utilidades.String.Left(cte.PortoPassagemTres?.Descricao, 15);
                retorno.Add(ponto);
            }

            if (!string.IsNullOrEmpty(cte.PortoPassagemQuatro?.Descricao))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit>();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit();
                ponto.transitAcronym = Utilidades.String.Left(cte.PortoPassagemQuatro?.Descricao, 15);
                retorno.Add(ponto);
            }

            if (!string.IsNullOrEmpty(cte.PortoPassagemCinco?.Descricao))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit>();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFlowTransit();
                ponto.transitAcronym = Utilidades.String.Left(cte.PortoPassagemCinco?.Descricao, 15);
                retorno.Add(ponto);
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataIssuerObservation> obterEnvioCteAdditionalDataIssuerObservation(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataIssuerObservation> retorno = null;

            Repositorio.ObservacaoContribuinteCTE repObservacaoContribuinteCTe = new Repositorio.ObservacaoContribuinteCTE(unitOfWork);
            List<Dominio.Entidades.ObservacaoContribuinteCTE> listaObservacoes = repObservacaoContribuinteCTe.BuscarPorCTe(empresa.Codigo, cte.Codigo);

            int qtdObservacos = 0;
            foreach (Dominio.Entidades.ObservacaoContribuinteCTE observacao in listaObservacoes)
            {
                if (qtdObservacos < 10)//Tratativa da SEFAZ
                {
                    if (retorno == null)
                        retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataIssuerObservation>();

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataIssuerObservation obsCont = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataIssuerObservation();
                    obsCont.field = string.IsNullOrWhiteSpace(observacao.Identificador) ? "OBS CONTRIBUINTE" : observacao.Identificador.Trim().Left(20);
                    obsCont.text = observacao.Descricao.Trim().Left(160);

                    retorno.Add(obsCont);
                    qtdObservacos++;
                }
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFiscalObservation> obterEnvioCteAdditionalDataFiscalObservation(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFiscalObservation> retorno = null;

            Repositorio.ObservacaoFiscoCTE repObservacaoFiscoCTe = new Repositorio.ObservacaoFiscoCTE(unitOfWork);
            List<Dominio.Entidades.ObservacaoFiscoCTE> listaObservacoes = repObservacaoFiscoCTe.BuscarPorCTe(empresa.Codigo, cte.Codigo);

            foreach (Dominio.Entidades.ObservacaoFiscoCTE observacao in listaObservacoes)
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFiscalObservation>();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFiscalObservation obsCont = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataFiscalObservation();
                obsCont.field = string.IsNullOrWhiteSpace(observacao.Identificador) ? "OBS FISCO" : observacao.Identificador.Trim().Left(20);
                obsCont.text = observacao.Descricao.Trim().Left(160);
                retorno.Add(obsCont);
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataDeliveryDate obterEnvioCteAdditionalDataFiscalDeliveryDate(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataDeliveryDate retorno = null;

            if (cte.DataPrevistaEntrega != null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataDeliveryDate();
                retorno.deliveryDateType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumDeliveryDateType.on_defined_date;
                retorno.deliveryDate = ((DateTime)cte.DataPrevistaEntrega).ToString("yyyy-MM-dd");
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataDeliveryTime obterEnvioCteAdditionalDataFiscalDeliveryTime(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataDeliveryTime retorno = null;

            if (cte.DataPrevistaEntrega != null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataAdditionalDataDeliveryTime();
                retorno.deliveryTimeType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumDeliveryTimeType.no_defined_time;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmount obterEnvioCteServiceAmount(out decimal valorTotalComponetes, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmount retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmount();
            retorno.serviceAmount = Math.Round(cte.ValorPrestacaoServico, 2, MidpointRounding.AwayFromZero);
            retorno.receivableServiceAmount = Math.Round(cte.ValorAReceber, 2, MidpointRounding.AwayFromZero);
            retorno.serviceCompoment = this.obterEnvioCteServiceAmountServiceCompoment(out valorTotalComponetes, cte, empresa, unitOfWork);
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment> obterEnvioCteServiceAmountServiceCompoment(out decimal valorTotalComponetes, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment>();

            Repositorio.ComponentePrestacaoCTE repComponentesPrestacao = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
            List<Dominio.Entidades.ComponentePrestacaoCTE> listaComponentesDaPrestacao = repComponentesPrestacao.BuscarPorCTe(empresa.Codigo, cte.Codigo);

            valorTotalComponetes = 0;
            if (listaComponentesDaPrestacao.Count > 0)
            {
                retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment>();
                foreach (Dominio.Entidades.ComponentePrestacaoCTE componente in listaComponentesDaPrestacao)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment complemento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment();
                    complemento.compomentName = cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && componente.Nome == "FRETE VALOR" && !string.IsNullOrWhiteSpace(cte.DescricaoComplemento) ? cte.DescricaoComplemento.ToUpper() : componente.Nome;
                    complemento.compomentAmount = Math.Round(componente.Valor, 2, MidpointRounding.AwayFromZero);
                    retorno.Add(complemento);

                    valorTotalComponetes += complemento.compomentAmount;
                }
            }
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTax obterEnvioCteTax(decimal valorTotalComponetes, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTax retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTax();


            if (cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim)
                retorno.tax = this.obterEnvioCteTaxFiscalCodeSimplesNacional(cte);
            else if (cte.CST == "00")
                retorno.tax = this.obterEnvioCteTaxFiscalCode00(cte);
            else if (cte.CST == "20")
                retorno.tax = this.obterEnvioCteTaxFiscalCode20(cte);
            else if (cte.CST == "40" || cte.CST == "41" || cte.CST == "51")
                retorno.tax = this.obterEnvioCteTaxFiscalCode40_41_51(cte);
            else if (cte.CST == "60")
                retorno.tax = this.obterEnvioCteTaxFiscalCode60(cte);
            else if (cte.CST == "90")
                retorno.tax = this.obterEnvioCteTaxFiscalCodeOtherState(cte);
            else if (cte.CST == "91")
                retorno.tax = this.obterEnvioCteTaxFiscalCode90(cte);

            retorno.destinationState = this.obterEnvioCteTaxDestinationState(cte);
            Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas outrasAliquotas = cte.OutrasAliquotas;
            if (outrasAliquotas == null && !string.IsNullOrEmpty(cte.CSTIBSCBS) && !string.IsNullOrEmpty(cte.ClassificacaoTributariaIBSCBS))
                outrasAliquotas = repositorioOutrasAliquotas.BuscarPorCSTClassificacaoTributaria(cte.CSTIBSCBS, cte.ClassificacaoTributariaIBSCBS);

            retorno.IBSCBS = this.obterImpostoIBSCBS(cte, outrasAliquotas);

            if (cte.ValorTotalDocumentoFiscal > 0)
                retorno.ValorTotalDocumento = cte.ValorTotalDocumentoFiscal;
            else if (retorno.IBSCBS != null && outrasAliquotas != null)
            {
                decimal valorTotalDocumento = cte.ValorPrestacaoServico;
                Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota impostoIBSCSB = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork).ObterOutrasAliquotasIBSCBS(outrasAliquotas.Codigo);
                if ((impostoIBSCSB?.SomarImpostosDocumento ?? false) || (outrasAliquotas.CalcularImpostoDocumento))
                    valorTotalDocumento = valorTotalDocumento + cte.ValorIBSMunicipal + cte.ValorIBSEstadual + cte.ValorCBS;

                retorno.ValorTotalDocumento = Math.Round(valorTotalDocumento, 2, MidpointRounding.AwayFromZero);
            }

            Repositorio.ImpostoIBPT repImpostoIBPT = new Repositorio.ImpostoIBPT(unitOfWork);
            Dominio.Entidades.ImpostoIBPT imposto = repImpostoIBPT.BuscarPorEstado(empresa.Localidade.Estado.Sigla);
            decimal percentualImpostoMunicipal = 0;
            decimal valorImpostoMunicipal = 0;
            decimal percentualImpostoEstadual = 0;
            decimal valorImpostoEstadual = 0;
            decimal percentualImpostoFederal = 0;
            decimal valorImpostoFederal = 0;

            if (imposto != null)
            {
                percentualImpostoMunicipal = imposto.PercentualMunicipal;
                if (percentualImpostoMunicipal > 0)
                    valorImpostoMunicipal = Math.Round(valorTotalComponetes * (percentualImpostoMunicipal / 100), 2, MidpointRounding.AwayFromZero);

                percentualImpostoEstadual = imposto.PercentualEstadual;
                if (percentualImpostoEstadual > 0)
                    valorImpostoEstadual = Math.Round(valorTotalComponetes * (percentualImpostoEstadual / 100), 2, MidpointRounding.AwayFromZero);

                percentualImpostoFederal = imposto.PercentualFederalNacional;
                if (percentualImpostoFederal > 0)
                    valorImpostoFederal = Math.Round(valorTotalComponetes * (percentualImpostoFederal / 100), 2, MidpointRounding.AwayFromZero);
            }

            decimal valorTotalImposto = valorImpostoMunicipal + valorImpostoEstadual + valorImpostoFederal;
            if (valorTotalImposto > 0)
                retorno.totalTaxAmount = Math.Round(valorTotalImposto, 2, MidpointRounding.AwayFromZero);

            CultureInfo culture = new CultureInfo("pt-BR");
            string observacaoFisco = $@"O valor aproximado de tributos incidentes sobre o valor deste servico e de R$ {valorImpostoMunicipal.ToString("0.00", culture)} ({percentualImpostoMunicipal.ToString("0.00", culture)}% Municipal), R$ {valorImpostoEstadual.ToString("0.00", culture)} ({percentualImpostoEstadual.ToString("0.00", culture)}% Estadual), R$ {valorImpostoFederal.ToString("0.00", culture)} ({percentualImpostoFederal.ToString("0.00", culture)}% Federal). Fonte: IBPT";
            retorno.observation = !string.IsNullOrEmpty(cte.InformacaoAdicionalFisco) ? cte.InformacaoAdicionalFisco + " " + observacaoFisco : observacaoFisco;


            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteIBSCBS obterImpostoIBSCBS(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas outrasAliquotas)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteIBSCBS retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteIBSCBS();

            if (outrasAliquotas == null || cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim || !(DateTime.Now >= _configuracaoIntegracaoEmissorDocumento.DataLiberacaoImpostos || (cte.Empresa.Configuracao?.EnviarNovoImposto ?? false)))
                return null;

            retorno.CodigoFiscal = cte.CSTIBSCBS;
            retorno.ClassificacaoTributaria = cte.ClassificacaoTributariaIBSCBS;

            if (cte.BaseCalculoIBSCBS == 0)
                return retorno;

            retorno.GrupoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.GrupoIBSCBS();
            retorno.GrupoIBSCBS.ValorIBS = cte.ValorIBSMunicipal + cte.ValorIBSEstadual;
            retorno.GrupoIBSCBS.BaseCalculo = cte.BaseCalculoIBSCBS;

            retorno.GrupoIBSCBS.IBSEstado = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.IBSEstado();
            retorno.GrupoIBSCBS.IBSEstado.AliquotaIBSEstadual = cte.AliquotaIBSEstadual;
            retorno.GrupoIBSCBS.IBSEstado.ValorIBSEstadual = cte.ValorIBSEstadual;

            retorno.GrupoIBSCBS.IBSMunicipal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.IBSMunicipal();
            retorno.GrupoIBSCBS.IBSMunicipal.AliquotaIBSMunicipal = cte.AliquotaIBSMunicipal;
            retorno.GrupoIBSCBS.IBSMunicipal.ValorIBSMunicipal = cte.ValorIBSMunicipal;

            retorno.GrupoIBSCBS.CBS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.CBS();
            retorno.GrupoIBSCBS.CBS.AliquotaCBS = cte.AliquotaCBS;
            retorno.GrupoIBSCBS.CBS.ValorCBS = cte.ValorCBS;

            if (cte.PercentualReducaoIBSEstadual > 0)
            {
                retorno.GrupoIBSCBS.IBSEstado.ReducaoAliquota = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.ReducaoAliquota();
                retorno.GrupoIBSCBS.IBSEstado.ReducaoAliquota.PercentualReducao = cte.PercentualReducaoIBSEstadual;
                retorno.GrupoIBSCBS.IBSEstado.ReducaoAliquota.AliquotaEfetiva = cte.AliquotaIBSEstadualEfetiva;
            }

            if (cte.PercentualReducaoIBSMunicipal > 0)
            {
                retorno.GrupoIBSCBS.IBSMunicipal.ReducaoAliquota = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.ReducaoAliquota();
                retorno.GrupoIBSCBS.IBSMunicipal.ReducaoAliquota.PercentualReducao = cte.PercentualReducaoIBSMunicipal;
                retorno.GrupoIBSCBS.IBSMunicipal.ReducaoAliquota.AliquotaEfetiva = cte.AliquotaIBSMunicipalEfetiva;
            }

            if (cte.PercentualReducaoCBS > 0)
            {
                retorno.GrupoIBSCBS.CBS.ReducaoAliquota = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.ReducaoAliquota();
                retorno.GrupoIBSCBS.CBS.ReducaoAliquota.PercentualReducao = cte.PercentualReducaoCBS;
                retorno.GrupoIBSCBS.CBS.ReducaoAliquota.AliquotaEfetiva = cte.AliquotaCBSEfetiva;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxDestinationState obterEnvioCteTaxDestinationState(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxDestinationState retorno = null;
            if (cte.ValorICMSFCPFim > 0)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxDestinationState();
                retorno.taxSateDestinationBase = cte.BaseCalculoICMS;
                retorno.fcpStateDestinationRate = cte.ValorICMSFCPFim > 0 && cte.BaseCalculoICMS > 0 ? Math.Round(((cte.ValorICMSFCPFim * 100) / cte.BaseCalculoICMS), 2) : 0;
                retorno.taxSateDestinationRate = cte.PercentualICMSPartilha;
                retorno.interstateTaxtRate = cte.AliquotaICMSInterna;
                retorno.fcpStateDestinationAmount = cte.ValorICMSFCPFim;
                retorno.taxStateDestinationAmount = cte.ValorICMSUFDestino;
                retorno.taxStateSenderAmount = cte.ValorICMSUFOrigem;
            }
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode00 obterEnvioCteTaxFiscalCode00(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode00 retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode00();
            retorno.taxBaseAmount = Math.Round(cte.BaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxRate = Math.Round(cte.AliquotaICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxAmount = Math.Round(cte.ValorICMS, 2, MidpointRounding.AwayFromZero);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode20 obterEnvioCteTaxFiscalCode20(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode20 retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode20();
            retorno.taxBaseAmountReduction = Math.Round(cte.PercentualReducaoBaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxBaseAmount = Math.Round(cte.BaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxRate = Math.Round(cte.AliquotaICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxAmount = Math.Round(cte.ValorICMS, 2, MidpointRounding.AwayFromZero);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode40_41_51 obterEnvioCteTaxFiscalCode40_41_51(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode40_41_51 retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode40_41_51();
            switch (cte.CST)
            {
                case "40":
                    retorno.fiscalCode = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumFiscalCode40_41_51.cst40;
                    break;
                case "41":
                    retorno.fiscalCode = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumFiscalCode40_41_51.cst41;
                    break;
                case "51":
                    retorno.fiscalCode = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumFiscalCode40_41_51.cst51;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode60 obterEnvioCteTaxFiscalCode60(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode60 retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode60();
            retorno.withheldTaxBase = Math.Round(cte.BaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
            retorno.withheldTaxRate = Math.Round(cte.AliquotaICMS, 2, MidpointRounding.AwayFromZero);
            retorno.withheldTaxAmount = Math.Round(cte.ValorICMS, 2, MidpointRounding.AwayFromZero);
            retorno.creditAmount = Math.Round(cte.ValorPresumido, 2, MidpointRounding.AwayFromZero);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode90 obterEnvioCteTaxFiscalCode90(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode90 retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCode90();
            retorno.taxBaseAmountReduction = Math.Round(cte.PercentualReducaoBaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxBaseAmount = Math.Round(cte.BaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxRate = Math.Round(cte.AliquotaICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxAmount = Math.Round(cte.ValorICMS, 2, MidpointRounding.AwayFromZero);
            retorno.creditAmount = Math.Round(cte.ValorPresumido, 2, MidpointRounding.AwayFromZero);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCodeOtherState obterEnvioCteTaxFiscalCodeOtherState(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCodeOtherState retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCodeOtherState();
            retorno.taxBaseAmountReduction = Math.Round(cte.PercentualReducaoBaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxBaseAmount = Math.Round(cte.BaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxRate = Math.Round(cte.AliquotaICMS, 2, MidpointRounding.AwayFromZero);
            retorno.taxAmount = Math.Round(cte.ValorICMS, 2, MidpointRounding.AwayFromZero);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCodeSimplesNacional obterEnvioCteTaxFiscalCodeSimplesNacional(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCodeSimplesNacional retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTaxFiscalCodeSimplesNacional();
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormal obterEnvioCteNormal(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormal retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormal();

            retorno.cargoInformation = this.obterEnvioCteNormalCargoInformation(cte, empresa, unitOfWork);
            retorno.document = this.obterEnvioCteNormalDocumentDocument(cte, empresa, unitOfWork);
            retorno.beforeDocument = this.obterEnvioCteNormalDocumentBeforeDocument(cte, empresa, unitOfWork);
            retorno.newVehicles = null;
            retorno.billing = this.obterEnvioCteNormalBilling(cte, empresa, unitOfWork);
            retorno.substitutionCte = this.obterEnvioCteNormalDocumentSubstitutionCte(cte);
            retorno.globalizedDescription = null; //TODO:
            retorno.multimodalCTe = this.obterEnvioCteNormalDocumentBeforeMultiModal(cte, empresa, unitOfWork);

            switch (this.obterModal(cte.ModalTransporte))
            {
                case Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal.road:
                    retorno.modalInformation = this.obterEnvioCteNormalModalInformationRoad(cte);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal.water:
                    retorno.modalInformation = this.obterEnvioCteNormalModalInformationWater(cte, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal.multimodal:
                    retorno.modalInformation = this.obterEnvioCteNormalModalInformationMultimodal(cte);
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationRoad obterEnvioCteNormalModalInformationRoad(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationRoad retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationRoad();

            retorno.rntrc = cte.RNTRC;
            retorno.collectionOrders = null;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWater obterEnvioCteNormalModalInformationWater(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWater retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWater();

            retorno.afrmmBaseAmount = cte.ValorPrestacaoAFRMM.HasValue ? cte.ValorPrestacaoAFRMM.Value : (decimal)0;
            retorno.afrmmAmount = cte.ValorAdicionalAFRMM.HasValue ? cte.ValorAdicionalAFRMM.Value : (decimal)0;
            retorno.shipId = cte.Navio?.Descricao;
            retorno.journeyNumber = cte.Viagem?.NumeroViagem.ToString("D");
            retorno.direction = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumDirection.north;
            retorno.irin = cte.Navio?.Irin;
            retorno.container = this.obterEnvioCteNormalModalInformationWaterContainer(cte, unitOfWork);
            retorno.shipType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumShipType.cabotagem;

            if (cte.Balsas != null && cte.Balsas.Count > 0)
            {
                retorno.ferryId = new List<string>();

                for (int i = 0; i < cte.Balsas.Count; i++)
                {
                    if (i == 0)
                        retorno.ferryId.Add(cte.Balsas[i].Descricao);
                    if (i == 1)
                        retorno.ferryId.Add(cte.Balsas[i].Descricao);
                    if (i == 2)
                        retorno.ferryId.Add(cte.Balsas[i].Descricao);
                }
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainer> obterEnvioCteNormalModalInformationWaterContainer(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.TipoServico != Dominio.Enumeradores.TipoServico.RedIntermediario && cte.TipoServico != Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainer> retorno = null;

            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);
            Repositorio.Embarcador.CTe.CTeContainerDocumento repCTeContainerDocumento = new Repositorio.Embarcador.CTe.CTeContainerDocumento(unitOfWork);
            List<Dominio.Entidades.ContainerCTE> containerCTEs = repContainerCTE.BuscarPorCTe(cte.Codigo);

            if (containerCTEs != null && containerCTEs.Count > 0)
            {
                retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainer>();

                foreach (Dominio.Entidades.ContainerCTE container in containerCTEs)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainer envContainer = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainer();
                    envContainer.containerId = !string.IsNullOrWhiteSpace(container.Numero) ? Utilidades.String.RemoveDiacritics(container.Numero.Replace("-", "").Replace(" ", "")) : (container.Container?.Numero ?? "").Replace("-", "").Replace(" ", "");

                    #region Lacres

                    if (!string.IsNullOrWhiteSpace(container.Lacre1))
                    {
                        if (envContainer.sealNumber == null)
                            envContainer.sealNumber = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerSealNumber>();

                        envContainer.sealNumber.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerSealNumber() { sealNumber = Utilidades.String.RemoveDiacritics(container.Lacre1) });
                    }

                    if (!string.IsNullOrWhiteSpace(container.Lacre2))
                    {
                        if (envContainer.sealNumber == null)
                            envContainer.sealNumber = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerSealNumber>();

                        envContainer.sealNumber.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerSealNumber() { sealNumber = Utilidades.String.RemoveDiacritics(container.Lacre2) });
                    }

                    if (!string.IsNullOrWhiteSpace(container.Lacre3))
                    {
                        if (envContainer.sealNumber == null)
                            envContainer.sealNumber = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerSealNumber>();

                        envContainer.sealNumber.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerSealNumber() { sealNumber = Utilidades.String.RemoveDiacritics(container.Lacre3) });
                    }

                    #endregion Lacres

                    #region Containers

                    List<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento> notas = repCTeContainerDocumento.BuscarPorContainerCTe(container.Codigo);

                    if (notas != null && notas.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento> documentoNf = notas.Where(o => o.TipoDocumento != Dominio.Enumeradores.TipoDocumentoCTe.NFe).ToList();
                        List<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento> documentoNfe = notas.Where(o => o.TipoDocumento == Dominio.Enumeradores.TipoDocumentoCTe.NFe).ToList();

                        if (documentoNfe.Count() > 0)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerDocumentNfe doc = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerDocumentNfe();
                            doc.documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerDocumentNfeDocument>();

                            foreach (Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento documento in documentoNfe)
                            {
                                doc.documents.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerDocumentNfeDocument() { key = documento.Chave, proRatedUnit = 0 });
                            }

                            envContainer.document = doc;
                        }
                        else if (documentoNf.Count() > 0)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerDocumentNf doc = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerDocumentNf();
                            doc.documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerDocumentNfDocument>();

                            foreach (Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento documento in documentoNf)
                            {
                                doc.documents.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationWaterContainerDocumentNfDocument()
                                {
                                    number = documento.Numero,
                                    serie = !string.IsNullOrWhiteSpace(documento.Serie) ? documento.Serie : "1",
                                    proRatedUnit = 0
                                });
                            }

                            envContainer.document = doc;
                        }
                    }

                    #endregion Containers

                    retorno.Add(envContainer);
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationMultimodal obterEnvioCteNormalModalInformationMultimodal(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationMultimodal retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalModalInformationMultimodal();
            retorno.multimodalNumber = cte.Empresa.COTM;
            retorno.allowsNegotiation = false;
            retorno.insurance = null;
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalCargoInformation obterEnvioCteNormalCargoInformation(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalCargoInformation retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalCargoInformation();
            retorno.cargoAmount = Math.Round(cte.ValorTotalMercadoria, 2, MidpointRounding.AwayFromZero);
            retorno.predominantProduct = cte.ProdutoPredominante;
            retorno.otherCharacteristics = null;
            retorno.quantity = this.obterEnvioCteNormalCargoInformationQuantity(cte, empresa, unitOfWork);
            retorno.endorsementAmount = cte.ValorCarbaAverbacao > 0 ? Math.Round(cte.ValorCarbaAverbacao, 2, MidpointRounding.AwayFromZero) : retorno.cargoAmount;
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalCargoInformationQuantity> obterEnvioCteNormalCargoInformationQuantity(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalCargoInformationQuantity> retorno = null;

            Repositorio.InformacaoCargaCTE repInformacaoDaCarga = new Repositorio.InformacaoCargaCTE(unitOfWork);
            List<Dominio.Entidades.InformacaoCargaCTE> listaInformacaoCarga = repInformacaoDaCarga.BuscarPorCTe(empresa.Codigo, cte.Codigo);

            if (listaInformacaoCarga.Count > 0)
            {
                retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalCargoInformationQuantity>();

                foreach (Dominio.Entidades.InformacaoCargaCTE informacao in listaInformacaoCarga)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalCargoInformationQuantity quantidade = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalCargoInformationQuantity();

                    if (informacao.UnidadeMedida == "01" && cte.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.DescricaoMedidaKgCTe) && (informacao.Tipo == "KG" || informacao.Tipo == "Quilograma"))
                        quantidade.typeMeasure = cte.Empresa.Configuracao.DescricaoMedidaKgCTe;
                    else
                        quantidade.typeMeasure = informacao.Tipo;

                    quantidade.unit = this.obterQuantityUnit(informacao.UnidadeMedida);
                    quantidade.quantity = informacao.Quantidade;

                    retorno.Add(quantidade);
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBilling obterEnvioCteNormalBilling(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBilling retorno = null;

            Repositorio.CobrancaCTe repCobrancaCTe = new Repositorio.CobrancaCTe(unitOfWork);
            Repositorio.ParcelaCobrancaCTe repParcelaCobrancaCTe = new Repositorio.ParcelaCobrancaCTe(unitOfWork);
            Dominio.Entidades.CobrancaCTe cobranca = repCobrancaCTe.BuscarPorCTe(empresa.Codigo, cte.Codigo);

            if (cobranca != null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBilling();
                retorno.number = cobranca.Numero.ToString();
                retorno.originalAmount = cobranca.Valor;
                retorno.discountAmount = cobranca.ValorDesconto;
                retorno.netAmount = cobranca.ValorLiquido;

                List<Dominio.Entidades.ParcelaCobrancaCTe> parcelas = repParcelaCobrancaCTe.BuscarPorCTe(empresa.Codigo, cte.Codigo);

                if (parcelas.Count > 0)
                {
                    retorno.duplicates = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBillingDuplicates>();

                    foreach (Dominio.Entidades.ParcelaCobrancaCTe parcela in parcelas)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBillingDuplicates duplicata = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBillingDuplicates();
                        duplicata.number = parcela.DataVencimento.ToString("yyyy-MM-dd");
                        duplicata.expirationOn = parcela.Numero.ToString();
                        duplicata.amount = parcela.Valor;
                        retorno.duplicates.Add(duplicata);
                    }
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalSubstitutionCte obterEnvioCteNormalDocumentSubstitutionCte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalSubstitutionCte retorno = null;

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalSubstitutionCte();
                retorno.key = cte.ChaveCTESubComp;
                retorno.takerChange = cte.SubstituicaoTomador;
            }

            return retorno;
        }

        private object obterEnvioCteNormalDocumentDocument(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            object retorno = null;

            if (cte.TipoServico != Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal && cte.TipoServico != Dominio.Enumeradores.TipoServico.RedIntermediario)
            {
                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                List<Dominio.Entidades.DocumentosCTE> listaDocumentos = repDocumentosCTe.BuscarPorCTe(empresa.Codigo, cte.Codigo);

                if (listaDocumentos.Count > 0)
                {
                    List<Dominio.Entidades.DocumentosCTE> documentosNf = listaDocumentos.Where(o => o.ModeloDocumentoFiscal != null && (o.ModeloDocumentoFiscal.Numero == "01" || o.ModeloDocumentoFiscal.Numero == "04")).ToList();
                    List<Dominio.Entidades.DocumentosCTE> documentosNfe = listaDocumentos.Where(o => o.ModeloDocumentoFiscal == null || o.ModeloDocumentoFiscal.Numero == "55").ToList();
                    List<Dominio.Entidades.DocumentosCTE> documentosOutros = listaDocumentos.Where(o => o.ModeloDocumentoFiscal != null && (o.ModeloDocumentoFiscal.Numero == "00" || o.ModeloDocumentoFiscal.Numero == "99")).ToList();

                    if (documentosNfe.Count > 0)
                        retorno = this.obterEnvioCteNormalDocumentNfe(cte, documentosNfe);
                    else if (documentosNf.Count > 0)
                        retorno = this.obterEnvioCteNormalDocumentNf(cte, documentosNf);
                    else if (documentosOutros.Count > 0)
                        retorno = this.obterEnvioCteNormalDocumentOther(cte, documentosOutros);
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNf obterEnvioCteNormalDocumentNf(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.DocumentosCTE> documentos)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNf retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNf();
            retorno.documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfDocuments>();

            foreach (Dominio.Entidades.DocumentosCTE documento in documentos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfDocuments doc = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfDocuments();

                int.TryParse(documento.Numero, out int numeroDocumento);

                doc.model = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumDocumentNfModel.single;
                doc.number = numeroDocumento == 0 && !string.IsNullOrWhiteSpace(documento.Numero) ? Utilidades.String.Left(documento.Numero, 20) : numeroDocumento.ToString();
                doc.serie = documento.Serie;
                doc.issueDate = documento.DataEmissao.ToString("yyyy-MM-dd");
                doc.taxBaseAmount = Math.Round(documento.BaseCalculoICMS, 2, MidpointRounding.AwayFromZero); ;
                doc.taxAmount = Math.Round(documento.ValorICMS, 2, MidpointRounding.AwayFromZero); ;
                doc.taxSTBaseAmount = Math.Round(documento.BaseCalculoICMSST, 2, MidpointRounding.AwayFromZero);
                doc.taxSTAmount = Math.Round(documento.ValorICMSST, 2, MidpointRounding.AwayFromZero); ;
                doc.productAmount = Math.Round(documento.ValorProdutos, 2, MidpointRounding.AwayFromZero);
                doc.documentAmount = Math.Round(documento.Valor, 2, MidpointRounding.AwayFromZero); ;
                doc.operationNatureCode = !string.IsNullOrWhiteSpace(documento.CFOP) ? documento.CFOP : null;
                doc.weight = Math.Round(documento.Peso, 2, MidpointRounding.AwayFromZero);
                doc.pin = string.IsNullOrWhiteSpace(documento.PINSuframa) ? null : documento.PINSuframa;
                doc.deliveryDate = null;
                doc.shipmentNumber = null;
                doc.orderNumber = null;
                doc.unit = null;

                retorno.documents.Add(doc);
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfe obterEnvioCteNormalDocumentNfe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.DocumentosCTE> documentos)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfe retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfe();
            retorno.documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfeDocuments>();

            foreach (Dominio.Entidades.DocumentosCTE documento in documentos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfeDocuments doc = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfeDocuments();
                doc.key = documento.ChaveNFE;
                retorno.documents.Add(doc);
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentOther obterEnvioCteNormalDocumentOther(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.DocumentosCTE> documentos)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentOther retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentOther();
            retorno.documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentOtherDocuments>();

            foreach (Dominio.Entidades.DocumentosCTE documento in documentos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentOtherDocuments doc = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentOtherDocuments();

                int.TryParse(documento.Numero, out int numeroDocumento);
                doc.number = numeroDocumento == 0 && !string.IsNullOrWhiteSpace(documento.Numero) ? Utilidades.String.Left(documento.Numero, 20) : numeroDocumento.ToString();
                doc.documentType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumOtherDocumentType.outros;
                doc.description = documento.Descricao;
                doc.issueDate = documento.DataEmissao.ToString("yyyy-MM-dd");
                doc.documentAmount = Math.Round(documento.Valor, 2, MidpointRounding.AwayFromZero);
                doc.deliveryDate = null;
                doc.unit = null;

                retorno.documents.Add(doc);
            }

            return retorno;
        }

        private List<object> obterEnvioCteNormalDocumentBeforeDocument(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<object> retorno = null;

            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoAnteriorCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
            List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosAnteriores = repDocumentoAnteriorCTe.BuscarPorCTeComLimite(empresa.Codigo, cte.Codigo, 2000); // 2000 replica solução que tinha no antigo emissor

            if (documentosAnteriores.Count > 0)
            {
                retorno = new List<object>();
                var queryEmissor = documentosAnteriores.GroupBy(o => o.Emissor).Select(g => new { emissor = g.Key, listaDoc = g });

                foreach (var emissor in queryEmissor)
                {
                    List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosNf = emissor.listaDoc.Where(o => string.IsNullOrEmpty(o.Chave)).ToList();
                    List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosNfe = emissor.listaDoc.Where(o => !string.IsNullOrEmpty(o.Chave)).ToList();

                    if (documentosNfe.Count > 0)
                        retorno.Add(obterEnvioCteNormalDocumentBeforeDocumentNfe(emissor.emissor, documentosNfe));
                    else
                        retorno.Add(obterEnvioCteNormalDocumentBeforeDocumentNf(emissor.emissor, documentosNf));
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentPaper obterEnvioCteNormalDocumentBeforeDocumentNf(Dominio.Entidades.Cliente emissor, List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentos)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentPaper retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentPaper();
            retorno.issuer = this.obterEnvioCteNormalDocumentBeforeDocumentIssuer(emissor);
            retorno.documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentPaperDocument>();

            foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento in documentos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentPaperDocument doc = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentPaperDocument();
                doc.documentType = this.obterPaperDocumentType(documento.Tipo);
                doc.number = documento.Numero;
                doc.issueDate = documento.DataEmissao != null ? documento.DataEmissao.Value.ToString("yyyy-MM-dd") : null;
                doc.serie = documento.Serie;
                doc.subSerie = null;
                retorno.documents.Add(doc);
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentElectronic obterEnvioCteNormalDocumentBeforeDocumentNfe(Dominio.Entidades.Cliente emissor, List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentos)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentElectronic retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentElectronic();
            retorno.issuer = this.obterEnvioCteNormalDocumentBeforeDocumentIssuer(emissor);
            retorno.documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentElectronicDocument>();

            foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento in documentos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentElectronicDocument doc = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentElectronicDocument();
                doc.key = documento.Chave;
                retorno.documents.Add(doc);
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentIssuer obterEnvioCteNormalDocumentBeforeDocumentIssuer(Dominio.Entidades.Cliente emissor)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentIssuer retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalBeforeDocumentIssuer();

            if (emissor.Tipo == "E")
                retorno.type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.foreign;
            else
                retorno.type = emissor.Tipo == "J" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.legal : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.individual;

            retorno.document = emissor.Tipo == "J" ? string.Format("{0:00000000000000}", emissor.CPF_CNPJ) : string.Empty;
            retorno.name = Utilidades.String.Left(emissor.Nome, 60);
            retorno.stateRegistration = string.IsNullOrWhiteSpace(emissor.IE_RG) || emissor.IE_RG.ToLower().Contains("isento") ? "00000000000000" : Utilidades.String.Left(Utilidades.String.OnlyNumbers(emissor.IE_RG), 14);
            retorno.state = emissor.Localidade.Estado.Sigla;
            retorno.phone = null;
            retorno.email = null;

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteComplement> obterEnvioCteComplement(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteComplement> retorno = null;

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento || cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
            {
                retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteComplement>();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteComplement chaveComplementar = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteComplement();
                chaveComplementar.key = cte.ChaveCTESubComp;
                retorno.Add(chaveComplementar);
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataDownloadAuthorization> obterEnvioCteDownloadAuthorization(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataDownloadAuthorization> retorno = null;

            if (cte.TomadorPagador?.GrupoPessoas?.AutorizadosDownloadDFe != null && cte.TomadorPagador.GrupoPessoas.AutorizadosDownloadDFe.Count > 0)
            {
                foreach (Dominio.Entidades.Cliente autorizadosDownloadDFe in cte.Tomador.GrupoPessoas.AutorizadosDownloadDFe)
                {
                    if (retorno == null)
                        retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataDownloadAuthorization>();

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataDownloadAuthorization autorizadoDownload = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataDownloadAuthorization();
                    autorizadoDownload.type = autorizadosDownloadDFe.Tipo == "F" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
                    autorizadoDownload.document = autorizadosDownloadDFe.CPF_CNPJ_SemFormato;
                    retorno.Add(autorizadoDownload);
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTechnicalManager obterEnvioCteTechnicalManager(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTechnicalManager retorno = null;
            if (!string.IsNullOrEmpty(_configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoCNPJ))
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTechnicalManager();
                retorno.document = _configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoCNPJ;
                retorno.name = Utilidades.String.Left(_configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoNomeContato, 60);
                retorno.email = obterPrimeiroEmail(_configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoEmail);
                retorno.phone = Utilidades.String.OnlyNumbers(_configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoTelefone);
            }
            return retorno;
        }

        private List<cteDataCteNormalMultimodalCTe> obterEnvioCteNormalDocumentBeforeMultiModal(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.TipoServico != Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                return null;

            List<cteDataCteNormalMultimodalCTe> retorno = new List<cteDataCteNormalMultimodalCTe>();

            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoAnteriorCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
            List<string> documentosAnteriores = repDocumentoAnteriorCTe.BuscarChavesCteAnterioresPorCTe(empresa.Codigo, cte.Codigo);

            if (documentosAnteriores.Count > 0)
                foreach (var chave in documentosAnteriores)
                    retorno.Add(new cteDataCteNormalMultimodalCTe() { key = chave });

            return retorno;
        }

        #endregion CT-e Padrão


        #region CT-e Simplificado

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplified obterEnvioCteSimplificado(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplified retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplified();
            retorno.data = this.obterEnvioCteDataSimplificado(cte, empresa, unitOfWork);
            retorno.options = this.obterEnvioCteOptions(cte);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedData obterEnvioCteDataSimplificado(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedData retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedData();
            retorno.externalId = cte.Codigo.ToString();
            retorno.operationNatureCode = cte.CFOP.CodigoCFOP.ToString();
            retorno.operationNature = Utilidades.String.Left(cte.NaturezaDaOperacao.Descricao, 60);
            retorno.numericCode = Convert.ToInt64(Utilidades.String.Left(cte.Numero.ToString(), 7) + "9");
            retorno.serie = cte.Serie.Numero;
            retorno.number = cte.Numero;

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(cte.DataEmissao.HasValue ? cte.DataEmissao.Value : DateTime.Today);
            string fusohorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);
            retorno.issueDate = ((DateTime)cte.DataEmissao).ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "T") + fusohorario;

            retorno.printFormat = this.obterPrintFormat(cte.TipoImpressao);
            retorno.cteType = retorno.cteType = this.obterCteType(cte.TipoCTE);
            retorno.issueType = this.obterIssueType(cte.TipoEmissao, cte);
            retorno.modal = this.obterModal(cte.ModalTransporte);
            retorno.serviceType = this.obterServiceType(cte.TipoServico);
            retorno.originState = cte.LocalidadeInicioPrestacao?.Estado?.Sigla;
            retorno.destinationState = cte.LocalidadeTerminoPrestacao?.Estado?.Sigla;
            retorno.pickup = cte.Retira == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            retorno.pickupDetail = !string.IsNullOrWhiteSpace(cte.DetalhesRetira) ? cte.DetalhesRetira.Length > 80 ? cte.DetalhesRetira.Substring(0, 80) : cte.DetalhesRetira : null;
            retorno.issuer = this.obterEnvioCteIssuer(cte, unitOfWork);
            retorno.taker = this.obterEnvioCteSimplificadoDataTaker(cte);
            retorno.additionalData = this.obterEnvioCteAdditionalData(cte, empresa, unitOfWork);
            retorno.cargoInformation = this.obterEnvioCteSimplificadoDataCargoInformation(cte, empresa, unitOfWork);

            decimal valorTotalComponetes = 0;
            retorno.delivery = this.obterEnvioCteSimplificadoDataDelivery(ref valorTotalComponetes, cte, empresa, unitOfWork);

            switch (this.obterModal(cte.ModalTransporte))
            {
                case Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal.road:
                    retorno.modalInformation = this.obterEnvioCteNormalModalInformationRoad(cte);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal.water:
                    retorno.modalInformation = this.obterEnvioCteNormalModalInformationWater(cte, unitOfWork);
                    break;
            }

            retorno.substitutionCte = this.obterEnvioCteSimplificadoSubstitutionCte(cte);
            retorno.tax = this.obterEnvioCteTax(valorTotalComponetes, cte, empresa, unitOfWork);
            retorno.total = this.obterEnvioCteSimplificadoDataTotal(cte);
            retorno.downloadAuthorization = this.obterEnvioCteDownloadAuthorization(cte);
            retorno.technicalManager = this.obterEnvioCteTechnicalManager(cte);

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDelivery> obterEnvioCteSimplificadoDataDelivery(ref decimal valorTotalComponetes, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDelivery> retorno = null;

            Repositorio.EntregaCTe repositorioEntregaCTe = new Repositorio.EntregaCTe(unitOfWork);

            List<Dominio.Entidades.EntregaCTe> entregasCTe = repositorioEntregaCTe.BuscarPorCTe(cte.Codigo);

            if (entregasCTe.Count == 0)
                return retorno;

            Repositorio.EntregaCTeDocumento repositorioCTeDocumento = new Repositorio.EntregaCTeDocumento(unitOfWork);
            Repositorio.EntregaCTeDocumentoTransporteAnterior repositorioCTeDocumentoTransporteAnterior = new Repositorio.EntregaCTeDocumentoTransporteAnterior(unitOfWork);
            Repositorio.EntregaCTeComponentePrestacao repositorioEntregaComponentePrestacao = new Repositorio.EntregaCTeComponentePrestacao(unitOfWork);

            List<Dominio.Entidades.EntregaCTeComponentePrestacao> listaComponentesPrestacao = repositorioEntregaComponentePrestacao.BuscarPorCTe(cte.Codigo);
            List<Dominio.Entidades.EntregaCTeDocumento> listaDocumentosEntregaCTe = repositorioCTeDocumento.BuscarPorCTe(cte.Codigo);
            List<Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior> listaDocumentosEntregaTransporteAnterior = repositorioCTeDocumentoTransporteAnterior.BuscarPorCTe(cte.Codigo);

            int sequencia = 0;

            foreach (Dominio.Entidades.EntregaCTe entregaCTe in entregasCTe.OrderBy(o => o.Codigo))
            {
                retorno ??= new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDelivery>();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDelivery entrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDelivery();

                List<Dominio.Entidades.EntregaCTeComponentePrestacao> componentesPrestacao = listaComponentesPrestacao.FindAll(componentePrestacao => componentePrestacao.EntregaCTe.Codigo == entregaCTe.Codigo);
                List<Dominio.Entidades.EntregaCTeDocumento> documentosEntregaCTe = listaDocumentosEntregaCTe.FindAll(documentoEntregaCTe => documentoEntregaCTe.EntregaCTe.Codigo == entregaCTe.Codigo);
                List<Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior> documentosEntregaCTeTransporteAnterior = listaDocumentosEntregaTransporteAnterior.FindAll(documentoEntregaCTe => documentoEntregaCTe.EntregaCTe.Codigo == entregaCTe.Codigo);

                sequencia++;
                entrega.itemNumber = sequencia;
                entrega.originCity = this.obterEnvioCteSimplificadoDataDeliveryCity(entregaCTe.Origem);
                entrega.destinationCity = this.obterEnvioCteSimplificadoDataDeliveryCity(entregaCTe.Destino);
                entrega.serviceAmount = Math.Round(entregaCTe.ValorPrestacaoServico, 2, MidpointRounding.AwayFromZero);
                entrega.receivableServiceAmount = Math.Round(entregaCTe.ValorAReceber, 2, MidpointRounding.AwayFromZero);
                entrega.serviceCompoment = this.obterEnvioCteSimplificadoDataDeliveryServiceCompoment(ref valorTotalComponetes, componentesPrestacao);

                if (cte.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho || cte.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                    entrega.documents = this.obterEnvioCteSimplificadoDataDeliveryDocumentAnt(documentosEntregaCTeTransporteAnterior);
                else
                    entrega.documents = this.obterEnvioCteSimplificadoDataDeliveryDocumentNFe(documentosEntregaCTe);


                retorno.Add(entrega);
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDeliveryCity obterEnvioCteSimplificadoDataDeliveryCity(Dominio.Entidades.Localidade localidade)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDeliveryCity retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDeliveryCity();
            retorno.name = Utilidades.String.Left(localidade.Descricao, 60);
            retorno.code = localidade.CodigoIBGE.ToString();
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment> obterEnvioCteSimplificadoDataDeliveryServiceCompoment(ref decimal valorTotalComponetes, List<Dominio.Entidades.EntregaCTeComponentePrestacao> ComponentesPrestacao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment>();

            if (ComponentesPrestacao.Count > 0)
            {
                retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment>();
                foreach (Dominio.Entidades.EntregaCTeComponentePrestacao componente in ComponentesPrestacao)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment complemento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataServiceAmountServiceCompoment();
                    complemento.compomentName = componente.Nome;
                    complemento.compomentAmount = Math.Round(componente.Valor, 2, MidpointRounding.AwayFromZero);
                    retorno.Add(complemento);

                    valorTotalComponetes += complemento.compomentAmount;
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalSubstitutionCte obterEnvioCteSimplificadoSubstitutionCte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalSubstitutionCte retorno = null;

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.SimplificadoSubstituto && !string.IsNullOrEmpty(cte.ChaveCTESubComp))
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalSubstitutionCte();
                retorno.key = cte.ChaveCTESubComp;
                retorno.takerChange = cte.SubstituicaoTomador;
            }

            return retorno;
        }

        private object obterEnvioCteSimplificadoDataDeliveryDocumentNFe(List<Dominio.Entidades.EntregaCTeDocumento> documentos)
        {
            object retorno = null;

            if (documentos.Count > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDeliveryDocumentsNFe documentosNfe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDeliveryDocumentsNFe();
                documentosNfe.documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfeDocuments>();

                foreach (Dominio.Entidades.EntregaCTeDocumento documento in documentos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfeDocuments doc = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataCteNormalDocumentNfeDocuments();
                    doc.key = documento.DocumentosCTE.ChaveNFE;
                    documentosNfe.documents.Add(doc);
                }

                retorno = documentosNfe;
            }

            return retorno;
        }

        private object obterEnvioCteSimplificadoDataDeliveryDocumentAnt(List<Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior> documentos)
        {
            object retorno = null;

            if (documentos.Count > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDeliveryDocumentsBefore documentosBefore = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDeliveryDocumentsBefore();
                documentosBefore.documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDeliveryDocumentsBeforeDocuments>();

                foreach (Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior documento in documentos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDeliveryDocumentsBeforeDocuments doc = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataDeliveryDocumentsBeforeDocuments();
                    doc.cteKey = documento.DocumentoTransporteAnterior.CTe.Chave;
                    documentosBefore.documents.Add(doc);
                }

                retorno = documentosBefore;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataTotal obterEnvioCteSimplificadoDataTotal(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataTotal retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataTotal();
            retorno.serviceAmount = Math.Round(cte.ValorPrestacaoServico, 2, MidpointRounding.AwayFromZero);
            retorno.receivableServiceAmount = Math.Round(cte.ValorAReceber, 2, MidpointRounding.AwayFromZero);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataTaker obterEnvioCteSimplificadoDataTaker(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataTaker retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataTaker(); ;

            retorno.type = this.obterTakerType(cte.TipoTomador);
            retorno.takerTaxIdication = this.obterTakerTaxIdication(cte.IndicadorIETomador);

            Dominio.Entidades.ParticipanteCTe tomador = cte.ObterParticipante(cte.TipoTomador);
            if (tomador != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTaker outroTomador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTaker();

                if (!tomador.Exterior)
                {
                    outroTomador.type = tomador.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.legal : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.individual;
                    outroTomador.document = tomador.CPF_CNPJ_SemFormato;
                    outroTomador.name = Utilidades.String.Left(tomador.Nome, 60);
                    outroTomador.tradeName = Utilidades.String.Left(tomador.NomeFantasia, 60);
                    outroTomador.phone = !string.IsNullOrWhiteSpace(tomador.Telefone1) ? Utilidades.String.Left(Utilidades.String.OnlyNumbers(tomador.Telefone1), 14) : null;
                    outroTomador.email = this.obterEmailParticipante(tomador);
                    outroTomador.stateRegistration = Utilidades.String.Left(tomador.IE_RG, 14);

                    outroTomador.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddress()
                    {
                        street = String.IsNullOrWhiteSpace(tomador.Endereco) ? "RUA" : tomador.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(tomador.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(tomador.Endereco), 255),
                        number = !string.IsNullOrWhiteSpace(tomador.Numero) ? Utilidades.String.Left(tomador.Numero.Trim(), 60) : "S/N",
                        complement = !String.IsNullOrWhiteSpace(tomador.Complemento) && tomador.Complemento.Length > 2 ? Utilidades.String.Left(tomador.Complemento.Trim(), 60) : null,
                        neighborhood = String.IsNullOrWhiteSpace(tomador.Bairro) ? "BAIRRO" : tomador.Bairro.Length < 2 ? "BAIRRO " + tomador.Bairro : Utilidades.String.Left(tomador.Bairro.Trim(), 60),
                        zipCode = this.obterCEP(tomador.CEP),
                        city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddressCity()
                        {
                            code = tomador.Localidade?.CodigoIBGE.ToString(),
                            name = Utilidades.String.Left(tomador.Localidade.Descricao, 60),
                            state = tomador.Localidade.Estado.Sigla,
                        },
                        country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddressCountry()
                        {
                            name = tomador.Pais != null ? Utilidades.String.Left(tomador.Pais.Nome, 60) : Utilidades.String.Left(tomador.Localidade.Estado.Pais.Nome, 60),
                            code = tomador.Pais != null ? tomador.Pais.Codigo.ToString() : "1058",
                        }
                    };
                }
                else
                {
                    outroTomador.type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.foreign;
                    outroTomador.name = Utilidades.String.Left(tomador.Nome, 60);
                    outroTomador.tradeName = Utilidades.String.Left(tomador.NomeFantasia, 60);
                    outroTomador.phone = null;
                    outroTomador.email = tomador.EmailStatus && !string.IsNullOrWhiteSpace(tomador.Email) ? obterPrimeiroEmail(tomador.Email) : null;
                    outroTomador.stateRegistration = Utilidades.String.Left(tomador.IE_RG, 14);

                    outroTomador.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddress()
                    {
                        street = String.IsNullOrWhiteSpace(tomador.Endereco) ? "RUA" : tomador.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(tomador.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(tomador.Endereco), 255),
                        number = !string.IsNullOrWhiteSpace(tomador.Numero) ? Utilidades.String.Left(tomador.Numero.Trim(), 60) : "S/N",
                        complement = !String.IsNullOrWhiteSpace(tomador.Complemento) && tomador.Complemento.Length > 2 ? Utilidades.String.Left(tomador.Complemento.Trim(), 60) : null,
                        neighborhood = String.IsNullOrWhiteSpace(tomador.Bairro) ? "BAIRRO" : tomador.Bairro.Length < 2 ? "BAIRRO " + tomador.Bairro : Utilidades.String.Left(tomador.Bairro.Trim(), 60),
                        zipCode = null,
                        city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddressCity()
                        {
                            code = "9999999",
                            name = Utilidades.String.Left(tomador.Cidade, 60),
                            state = "EX"
                        },
                        country = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTakerAddressCountry()
                        {
                            name = tomador.Pais != null ? Utilidades.String.Left(tomador.Pais.Nome, 60) : "EXTERIOR",
                            code = tomador.Pais != null ? tomador.Pais?.Codigo.ToString() : "1058",
                        }
                    };
                }

                retorno.taker = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteDataTakerTaker();
                retorno.taker = outroTomador;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataCargoInformation obterEnvioCteSimplificadoDataCargoInformation(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataCargoInformation retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataCargoInformation();
            retorno.cargoAmount = Math.Round(cte.ValorTotalMercadoria, 2, MidpointRounding.AwayFromZero);
            retorno.predominantProduct = cte.ProdutoPredominante;
            retorno.otherCharacteristics = null;
            retorno.quantity = this.obterEnvioCteSimplificadoDataCargoInformationQuantity(cte, empresa, unitOfWork);
            retorno.endorsementAmount = cte.ValorCarbaAverbacao > 0 ? Math.Round(cte.ValorCarbaAverbacao, 2, MidpointRounding.AwayFromZero) : retorno.cargoAmount;
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataCargoInformationQuantity> obterEnvioCteSimplificadoDataCargoInformationQuantity(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataCargoInformationQuantity> retorno = null;

            Repositorio.InformacaoCargaCTE repInformacaoDaCarga = new Repositorio.InformacaoCargaCTE(unitOfWork);
            List<Dominio.Entidades.InformacaoCargaCTE> listaInformacaoCarga = repInformacaoDaCarga.BuscarPorCTe(empresa.Codigo, cte.Codigo);

            if (listaInformacaoCarga.Count > 0)
            {
                retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataCargoInformationQuantity>();

                foreach (Dominio.Entidades.InformacaoCargaCTE informacao in listaInformacaoCarga)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataCargoInformationQuantity quantidade = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteSimplifiedDataCargoInformationQuantity();

                    if (informacao.UnidadeMedida == "01" && cte.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.DescricaoMedidaKgCTe) && (informacao.Tipo == "KG" || informacao.Tipo == "Quilograma"))
                        quantidade.typeMeasure = this.obterTypeMeasure(cte.Empresa.Configuracao.DescricaoMedidaKgCTe);
                    else
                        quantidade.typeMeasure = this.obterTypeMeasure(informacao.Tipo);

                    quantidade.unit = this.obterQuantityUnit(informacao.UnidadeMedida);
                    quantidade.quantity = informacao.Quantidade;

                    retorno.Add(quantidade);
                }
            }

            return retorno;
        }

        #endregion CT-e Simplificado

        private string obterCEP(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return null;

            string valor = cep.ObterSomenteNumeros();

            if (valor.Length == 8)
                return valor;

            if (valor.Length > 8)
                return valor.Substring(0, 8);

            return valor.PadLeft(8, '0');
        }

        private string obterEmailParticipante(Dominio.Entidades.ParticipanteCTe participante)
        {
            string emails = string.Empty;

            if (!participante.Exterior)
            {
                if (participante.EmailStatus && !string.IsNullOrWhiteSpace(participante.Email))
                    emails += string.Concat(participante.Email, ";");
                else if (participante.Cliente != null && participante.Cliente.GrupoPessoas != null && participante.Cliente.GrupoPessoas.EnviarXMLCTePorEmail && !string.IsNullOrWhiteSpace(participante.Cliente.GrupoPessoas.Email))
                    emails += string.Concat(participante.Cliente.GrupoPessoas.Email, ";");

                if (participante.EmailContadorStatus && !string.IsNullOrWhiteSpace(participante.EmailContador))
                    emails += string.Concat(participante.EmailContador, ";");

                if (participante.EmailContatoStatus && !string.IsNullOrWhiteSpace(participante.EmailContato))
                    emails += string.Concat(participante.EmailContato, ";");// emails += participante.EmailContato;

                if (participante.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(participante.EmailTransportador))
                    emails += string.Concat(participante.EmailTransportador, ";");

                if (string.IsNullOrEmpty(emails))
                    emails = null;
            }
            else
            {
                emails = participante.EmailStatus && !string.IsNullOrWhiteSpace(participante.Email) ? participante.Email : null;
            }

            return obterPrimeiroEmail(emails);
        }

        private string obterPrimeiroEmail(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                List<string> emailLista = email.Split(';').ToList();
                return Utilidades.String.Left(emailLista.FirstOrDefault().Trim(), 60);
            }

            return null;
        }

        private string obterEmailParaEnvioDACTE(Dominio.Entidades.ParticipanteCTe participante)
        {
            string emails = string.Empty;

            if (participante.EmailStatus && !string.IsNullOrWhiteSpace(participante.Email))
                emails += string.Concat(participante.Email, ";");
            else if (participante.Cliente != null && participante.Cliente.GrupoPessoas != null && participante.Cliente.GrupoPessoas.EnviarXMLCTePorEmail && !string.IsNullOrWhiteSpace(participante.Cliente.GrupoPessoas.Email))
                emails += string.Concat(participante.Cliente.GrupoPessoas.Email, ";");

            if (participante.EmailContadorStatus && !string.IsNullOrWhiteSpace(participante.EmailContador))
                emails += string.Concat(participante.EmailContador, ";");

            if (participante.EmailContatoStatus && !string.IsNullOrWhiteSpace(participante.EmailContato))
                emails += string.Concat(participante.EmailContato, ";");// emails += participante.EmailContato;

            if (participante.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(participante.EmailTransportador))
                emails += string.Concat(participante.EmailTransportador, ";");

            if (string.IsNullOrEmpty(emails))
                emails = null;
            return Utilidades.String.Left(emails, 1000);
        }

        private string obterEmailParaEnvioDACTE(Dominio.Entidades.Empresa empresa)
        {
            string emails = string.Empty;

            if (empresa == null)
                return emails;

            if (empresa.StatusEmail == "A" && !string.IsNullOrWhiteSpace(empresa.Email))
                emails += string.Concat(empresa.Email, ";");

            if (empresa.StatusEmailContador == "A" && !string.IsNullOrWhiteSpace(empresa.EmailContador))
                emails += string.Concat(empresa.EmailContador, ";");

            return emails;
        }

        private string obterInscricaoST(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Localidade origem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorInscricaoST repTransportadorInscricaoST = new Repositorio.Embarcador.Transportadores.TransportadorInscricaoST(unitOfWork);

            Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST transportadorInscricaoST = repTransportadorInscricaoST.BuscarPorEmpresaEEstado(empresa.Codigo, origem.Estado.Sigla);

            return transportadorInscricaoST?.InscricaoST ?? null;
        }

        private string AjustarFuso(int hora, int minutos)
        {
            string minutosString = minutos.ToString();

            if (minutosString.Length == 1)
                minutosString = string.Concat("0", minutosString);

            switch (hora)
            {
                case -12:
                    return "-12:" + minutosString;
                case -11:
                    return "-11:" + minutosString;
                case -10:
                    return "-10:" + minutosString;
                case -9:
                    return "-09:" + minutosString;
                case -8:
                    return "-08:" + minutosString;
                case -7:
                    return "-07:" + minutosString;
                case -6:
                    return "-06:" + minutosString;
                case -5:
                    return "-05:" + minutosString;
                case -4:
                    return "-04:" + minutosString;
                case -3:
                    return "-03:" + minutosString;
                case -2:
                    return "-02:" + minutosString;
                case -1:
                    return "-01:" + minutosString;
                case 0:
                    return "00:" + minutosString;
                case 1:
                    return "+01:" + minutosString;
                case 2:
                    return "+02:" + minutosString;
                case 3:
                    return "+03:" + minutosString;
                case 4:
                    return "+04:" + minutosString;
                case 5:
                    return "+05:" + minutosString;
                case 6:
                    return "+06:" + minutosString;
                case 7:
                    return "+07:" + minutosString;
                case 8:
                    return "+08:" + minutosString;
                case 9:
                    return "+09:" + minutosString;
                case 10:
                    return "+10:" + minutosString;
                case 11:
                    return "+11:" + minutosString;
                case 12:
                    return "+12:" + minutosString;
                case 13:
                    return "+13:" + minutosString;
                default:
                    return "-03:00";
            }

        }

        #region Conversão de Enums

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPrintFormat obterPrintFormat(Dominio.Enumeradores.TipoImpressao tipoImpressao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPrintFormat retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPrintFormat();

            switch (tipoImpressao)
            {
                case Dominio.Enumeradores.TipoImpressao.Retrato:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPrintFormat.portrait;
                    break;
                case Dominio.Enumeradores.TipoImpressao.Paisagem:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPrintFormat.landscape;
                    break;
                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPrintFormat.portrait;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumCteType obterCteType(Dominio.Enumeradores.TipoCTE tipoCTE)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumCteType retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumCteType();

            switch (tipoCTE)
            {
                case Dominio.Enumeradores.TipoCTE.Normal:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumCteType.normal;
                    break;
                case Dominio.Enumeradores.TipoCTE.Complemento:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumCteType.complement;
                    break;
                case Dominio.Enumeradores.TipoCTE.Substituto:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumCteType.substitution;
                    break;
                case Dominio.Enumeradores.TipoCTE.Simplificado:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumCteType.simplified;
                    break;
                case Dominio.Enumeradores.TipoCTE.SimplificadoSubstituto:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumCteType.simplified_substitution;
                    break;
                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumCteType.normal;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType obterIssueType(string tipoEmissao, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType();

            // if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && !string.IsNullOrWhiteSpace(cte.Empresa.Localidade.Estado.TipoEmissao) && cte.Empresa.Localidade.Estado.TipoEmissao != "1")
            //      tipoEmissao = cte.Empresa.Localidade.Estado.TipoEmissao;

            /// 1 - Normal
            /// 4 - EPEC pela SVC
            /// 5 - Contingência FSDA
            /// 7 - SVC-RS
            /// 8 - SVC-SP
            switch (tipoEmissao)
            {
                case "1":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType.default_;
                    break;
                case "4":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType.epec;
                    break;
                case "5":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType.fsda;
                    break;
                case "7":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType.svc;
                    break;
                case "8":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType.svc;
                    break;
                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType.default_;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal obterModal(Dominio.Entidades.ModalTransporte modal)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal();

            // RODOVIARIO  01
            // AQUAVIARIO  03
            // FERROVIARIO 04
            // MULTIMODAL  06

            switch (modal?.Numero ?? "01")
            {
                case "01":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal.road;
                    break;
                case "03":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal.water;
                    break;
                case "04":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal.rail;
                    break;
                case "06":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal.multimodal;
                    break;
                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModal.road;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumServiceType obterServiceType(Dominio.Enumeradores.TipoServico tipoServico)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumServiceType retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumServiceType();

            switch (tipoServico)
            {
                case Dominio.Enumeradores.TipoServico.Normal:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumServiceType.normal;
                    break;
                case Dominio.Enumeradores.TipoServico.SubContratacao:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumServiceType.subcontracting;
                    break;
                case Dominio.Enumeradores.TipoServico.Redespacho:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumServiceType.transshipment;
                    break;
                case Dominio.Enumeradores.TipoServico.RedIntermediario:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumServiceType.intermediate_transshipment;
                    break;
                case Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumServiceType.multimodal_associated;
                    break;
                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumServiceType.normal;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerTaxIdication obterTakerTaxIdication(Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE? indicadorIETomador)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerTaxIdication retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerTaxIdication();

            switch (indicadorIETomador ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerTaxIdication.non_taxpayer;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteIsento:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerTaxIdication.exempt_taxpayer;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerTaxIdication.taxpayer;
                    break;
                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerTaxIdication.non_taxpayer;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType obterTakerType(Dominio.Enumeradores.TipoTomador tipoTomador)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType();

            switch (tipoTomador)
            {
                case Dominio.Enumeradores.TipoTomador.NaoInformado:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType.others;
                    break;
                case Dominio.Enumeradores.TipoTomador.Remetente:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType.sender;
                    break;
                case Dominio.Enumeradores.TipoTomador.Destinatario:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType.recipient;
                    break;
                case Dominio.Enumeradores.TipoTomador.Expedidor:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType.shipper;
                    break;
                case Dominio.Enumeradores.TipoTomador.Recebedor:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType.receiver;
                    break;
                case Dominio.Enumeradores.TipoTomador.Intermediario:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType.others;
                    break;
                case Dominio.Enumeradores.TipoTomador.Tomador:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType.others;
                    break;
                case Dominio.Enumeradores.TipoTomador.Outros:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType.others;
                    break;
                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTakerType.others;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTaxRegime obterTaxRegime(Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe regimeTributarioCTe)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTaxRegime retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTaxRegime();

            switch (regimeTributarioCTe)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe.SimplesNacional:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTaxRegime.simples_nacional;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe.SimplesNacionalExcessoReceita:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTaxRegime.lucro_presumido;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe.RegimeNormal:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTaxRegime.lucro_real;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe.SimplesNacionalMEI:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTaxRegime.micro_empreendedor_individual;
                    break;
                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTaxRegime.lucro_real;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit obterQuantityUnit(string unidadeMedida)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit();

            switch (unidadeMedida)
            {
                /// 00 - M3
                /// 01 - KG
                /// 02 - TON
                /// 03 - UN
                /// 04 - LT
                /// 05 - MMBTU
                /// 99 - M3 ST (Unidade de medida feita para a Sintravir)
                case "00":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit.m3;
                    break;
                case "01":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit.kg;
                    break;
                case "02":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit.ton;
                    break;
                case "03":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit.unidade;
                    break;
                case "04":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit.litros;
                    break;
                case "05":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit.mmbtu;
                    break;
                case "99":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit.m3;
                    break;
                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumQuantityUnit.kg;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure obterTypeMeasure(string descricao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure();

            if (string.IsNullOrEmpty(descricao))
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.outros;
            else if (descricao.ToUpper().Contains("PESO") || descricao.ToUpper().Contains("QUILOGRAMA"))
            {
                if (descricao.ToUpper().Contains("CUBADO"))
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.peso_cubado;
                else
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.peso_bruto_aferido_transportador;
            }
            else if (descricao.ToUpper().Contains("CAIXA"))
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.caixas;
            else if (descricao.ToUpper().Contains("LITRO") || descricao.ToUpper().Contains("LITRAGEM"))
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.litragem;
            else if (descricao.ToUpper().Contains("BOMBONA"))
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.bombonas;
            else if (descricao.ToUpper().Contains("PALETE"))
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.paletes;
            else if (descricao.ToUpper().Contains("SACA"))
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.sacas;
            else if (descricao.ToUpper().Contains("MMBTU"))
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.milhao_btu;
            else if (descricao.ToUpper().Contains("ROLO"))
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.rolos;
            else if (descricao.ToUpper().Contains("LATA"))
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.latas;
            else
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTypeMeasure.outros;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType obterPaperDocumentType(string tipo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType();

            /*
            this.CTRC = 00;
            this.CTAC = 01;
            this.ACT = 02;
            this.NFModelo7 = 03;
            this.NFModelo27 = 04;
            this.ConhecimentoAereoNacional = 05;
            this.CTMC = 06;
            this.ATRE = 07;
            this.DTA = 08;
            this.ConhecimentoAereoInternacional = 09;
            this.ConhecimentoCartaPorteInternacional = 10;
            this.ConhecimentoAvulso = 11;
            this.TIF = 12;
            this.BIL = 13;
            this.Outros = 99;
            */

            switch (tipo)
            {
                case "00": // CTRC
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.individual_bill_lading;
                    break;

                case "01":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.individual_bill_lading; //TODO:
                    break;

                case "02":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.individual_bill_lading; //TODO:
                    break;

                case "03":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.individual_bill_lading; //TODO:
                    break;

                case "04":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.individual_bill_lading; //TODO:
                    break;

                case "05":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.international_air_waybill;
                    break;

                case "06":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.individual_bill_lading; //TODO:
                    break;

                case "07":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.atre;
                    break;

                case "08":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.dta;
                    break;

                case "09":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.international_air_waybill;
                    break;

                case "10":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.international_waybill;
                    break;

                case "11":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.individual_bill_lading;
                    break;

                case "12":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.tif;
                    break;

                case "13":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.bl;
                    break;

                case "99":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.individual_bill_lading; //TODO:
                    break;

                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.PaperDocumentType.individual_bill_lading; //TODO:
                    break;
            }

            return retorno;
        }

        #endregion Conversão de Enums

        #endregion
    }
}
