using Dominio.Enumeradores;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool EnviarMDFeEmissor(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork, bool solicitarEmissaoContingencia = false)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            bool sucesso = false;
            string id = string.Empty;
            string mensagemErro = string.Empty;

            try
            {
                Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                mdfe = repMDFe.BuscarPorCodigo(mdfe.Codigo);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (!solicitarEmissaoContingencia)
                    svcMDFe.EncerrarMDFesAnteriores(mdfe, empresa.Codigo, unitOfWork);

                bool sincronizarDocumento = false;
                mdfe.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech;
                var envioWS = this.obterEnvioMDFe(mdfe, empresa, unitOfWork, solicitarEmissaoContingencia);

                //Transmite
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.POST, envioWS, "mdfe-v3", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPIMDFe);

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
                        Servicos.Log.TratarErro("Emissor NSTech: Ocorreu uma falha ao processar o retorno do envio do mdfe");
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar retorno JSON MDFe Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao efetuar o envio do mdfe; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        id = (string)retorno.id;
                        sucesso = true;
                    }
                }

                if (!solicitarEmissaoContingencia)
                {
                    if (sucesso)
                    {
                        mdfe.MensagemRetornoSefaz = "MDF-e em processamento.";
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Enviado;
                        mdfe.DataIntegracao = DateTime.Now;
                        repMDFe.Atualizar(mdfe);

                        if (sincronizarDocumento)
                            this.ConsultarMdfe(mdfe, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unitOfWork);
                    }
                    else
                    {
                        mdfe.MensagemRetornoSefaz = mensagemErro;
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                        repMDFe.Atualizar(mdfe);

                        Servicos.Log.TratarErro(mensagemErro);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Emissor NSTech: Ocorreu uma falha ao efetuar o envio do mdfe");
                Servicos.Log.TratarErro(ex);

                if (!solicitarEmissaoContingencia)
                {
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                    mdfe.MensagemRetornoSefaz = string.Concat("ERRO: Sefaz indisponível no momento. Tente novamente.");
                    repMDFe.Atualizar(mdfe);
                }

                Servicos.Email svcEmail = new Servicos.Email(unitOfWork);

                string ambiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().IdentificacaoAmbiente;
                string assunto = ambiente + " - Problemas na emissão de MDF-e!";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<p>Atenção, problemas na emissão de MDF-e no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                sb.Append("MDFe ").Append(mdfe.Numero).Append("/").Append(mdfe.Serie.Numero.ToString()).Append(" transportador ").Append(mdfe.Empresa.CNPJ).Append(" ").Append(mdfe.Empresa.RazaoSocial).Append("<br /> <br />");
                sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                System.Text.StringBuilder ss = new System.Text.StringBuilder();
                ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                System.Threading.Tasks.Task.Factory.StartNew(() => svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unitOfWork, 0, true, null, false));
#else
                System.Threading.Tasks.Task.Factory.StartNew(() => svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "cesar@multisoftware.com.br", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unitOfWork, 0, true, null, false));
#endif     

                sucesso = false;
            }

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfe obterEnvioMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, bool solicitarEmissaoContingencia)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfe retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfe();
            retorno.data = this.obterEnvioMDFeData(mdfe, empresa, unitOfWork, solicitarEmissaoContingencia);
            retorno.options = this.obterEnvioMDFeOptions(mdfe, unitOfWork);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeData obterEnvioMDFeData(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, bool solicitarEmissaoContingencia)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeData retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeData();
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCargaMDFe.BuscarCargaPorMDFe(mdfe.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = null;

            if (carga != null)
                cargaCIOT = repositorioCargaCIOT.BuscarPorCarga(carga.Codigo);

            Repositorio.MDFeContratante repMDFeContratante = new Repositorio.MDFeContratante(unitOfWork);
            List<Dominio.Entidades.MDFeContratante> listaContratante = repMDFeContratante.BuscarPorMDFe(mdfe.Codigo);
            Repositorio.VeiculoMDFe repVeiculo = new Repositorio.VeiculoMDFe(unitOfWork);
            Dominio.Entidades.VeiculoMDFe veiculo = repVeiculo.BuscarPorMDFe(mdfe.Codigo);

            retorno.externalId = mdfe.Codigo.ToString();
            retorno.issuerType = this.obterIssuerTypeMDFe(mdfe.TipoEmitente);
            retorno.transporterType = this.obterTransporterType(veiculo);
            retorno.serie = mdfe.Serie.Numero;
            retorno.number = mdfe.Numero;
            retorno.numericCode = Convert.ToInt64(Utilidades.String.Left(mdfe.Numero.ToString(), 7) + "9");
            retorno.modal = this.obterModalMDFe(mdfe.Modal);

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(mdfe.DataEmissao.HasValue ? mdfe.DataEmissao.Value : DateTime.Today);
            string fusohorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);
            retorno.issueDate = ((DateTime)mdfe.DataEmissao).ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "T") + fusohorario;

            if (solicitarEmissaoContingencia)
            {
                retorno.issueType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueTypeMDFe.offline_contingency; // this.obterIssueTypeMDFe(mdfe.TipoEmissao);
                retorno.offlineEmissionMode = null; // Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumOfflineEmissionMode.disabled;
            }
            else
                retorno.issueType = null;

            retorno.originState = mdfe.EstadoCarregamento.Sigla;
            retorno.destinationState = mdfe.EstadoDescarregamento.Sigla;
            retorno.indCanalVerde = null;
            retorno.subsequentLoad = null;

            DateTime? dataHoraPrevistaInicioViagem = null;
            retorno.routeInfo = this.obterEnvioMDFeRouteInfo(mdfe, out dataHoraPrevistaInicioViagem, unitOfWork);
            retorno.startDate = null; // dataHoraPrevistaInicioViagem != null ? ((DateTime)dataHoraPrevistaInicioViagem).ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "T") + fusohorario : null;
            retorno.issuer = this.obterEnvioMDFeIssuer(empresa);

            string cepMunicipioCarregamento = string.Empty;
            decimal latitudeMunicipioCarregamento = 0;
            decimal longitudeMunicipioCarregamento = 0;
            string cepMunicipioDescarregamento = string.Empty;
            decimal latitudeMunicipioDescarregamento = 0;
            decimal longitudeMunicipioDescarregamento = 0;
            int qtdCte = 0;
            int qtdNfe = 0;
            int qtdMdfe = 0;

            retorno.loadingCitiesInfo = this.obterEnvioMDFeLoadingCitiesInfo(mdfe, out cepMunicipioCarregamento, out latitudeMunicipioCarregamento, out longitudeMunicipioCarregamento, unitOfWork);
            retorno.docInfo = this.obterEnvioMDFeDocInfo(mdfe, out cepMunicipioDescarregamento, out latitudeMunicipioDescarregamento, out longitudeMunicipioDescarregamento, ref qtdCte, ref qtdNfe, ref qtdMdfe, unitOfWork);
            retorno.modalInformation = this.obterEnvioMDFeModalInformation(mdfe, carga, listaContratante, veiculo, cargaCIOT, qtdCte + qtdNfe, unitOfWork);
            retorno.insurance = this.obterEnvioMDFeInsurance(mdfe, unitOfWork);
            retorno.predProduct = this.obterEnvioMDFePredProduct(mdfe, cepMunicipioCarregamento, latitudeMunicipioCarregamento, longitudeMunicipioCarregamento, cepMunicipioDescarregamento, latitudeMunicipioDescarregamento, longitudeMunicipioDescarregamento, carga, unitOfWork);
            retorno.tot = this.obterEnvioMDFeTot(mdfe, qtdCte, qtdNfe, qtdMdfe);
            retorno.seal = this.obterEnvioMDFeSeal(mdfe, unitOfWork);
            retorno.downloadAuthorization = this.obterEnvioMDFeDownloadAuthorization(mdfe, listaContratante, unitOfWork);
            retorno.additionalInfo = this.obterEnvioMDFeAdditionalInfo(mdfe);
            retorno.technicalManager = this.obterEnvioMDFeTechnicalManager();
            retorno.nffInfo = null;
            retorno.paaInfo = null;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptions obterEnvioMDFeOptions(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptions retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptions();
            retorno.removeSpecialsChars = true;
            retorno.damdfe = this.obterEnvioMDFeOptionsDacte(mdfe, unitOfWork);
            return retorno;
        }

        private List<string> obterEnvioMDFeRouteInfo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, out DateTime? dataHoraPrevistaInicioViagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.PercursoMDFe repPercurso = new Repositorio.PercursoMDFe(unitOfWork);
            List<Dominio.Entidades.PercursoMDFe> percursos = repPercurso.BuscarPorMDFe(mdfe.Codigo);

            dataHoraPrevistaInicioViagem = null;

            if (percursos != null && percursos.Count() > 0)
            {
                List<string> percursosIntegrar = new List<string>();

                foreach (Dominio.Entidades.PercursoMDFe percurso in percursos)
                {
                    percursosIntegrar.Add(percurso.Estado.Sigla);

                    if (dataHoraPrevistaInicioViagem == null)
                        dataHoraPrevistaInicioViagem = percurso.DataInicioViagem;
                }

                return percursosIntegrar;
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataIssuer obterEnvioMDFeIssuer(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataIssuer empresaEmitente = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataIssuer();

            empresaEmitente.type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
            empresaEmitente.document = Utilidades.String.OnlyNumbers(empresa.CNPJ);
            empresaEmitente.stateRegistration = string.IsNullOrWhiteSpace(empresa.InscricaoEstadual) ? "ISENTO" : Utilidades.String.Left(empresa.InscricaoEstadual, 14);
            empresaEmitente.name = Utilidades.String.Left(empresa.RazaoSocial, 60);
            empresaEmitente.tradeName = Utilidades.String.Left(empresa.NomeFantasia, 60);
            empresaEmitente.address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataIssuerAddress();
            empresaEmitente.address.street = String.IsNullOrWhiteSpace(empresa.Endereco) ? "RUA" : empresa.Endereco.Length < 2 ? "RUA " + Utilidades.String.RemoveSpecialCharacters(empresa.Endereco) : Utilidades.String.Left(Utilidades.String.RemoveSpecialCharacters(empresa.Endereco), 255);
            empresaEmitente.address.number = !string.IsNullOrWhiteSpace(empresa.Numero) ? Utilidades.String.Left(empresa.Numero.Trim(), 60) : "S/N";
            empresaEmitente.address.complement = !String.IsNullOrWhiteSpace(empresa.Complemento) && empresa.Complemento.Length > 2 ? Utilidades.String.Left(empresa.Complemento.Trim(), 60) : null;
            empresaEmitente.address.neighborhood = String.IsNullOrWhiteSpace(empresa.Bairro) ? "BAIRRO" : empresa.Bairro.Length < 2 ? "BAIRRO " + empresa.Bairro : Utilidades.String.Left(empresa.Bairro.Trim(), 60);
            empresaEmitente.address.city = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataIssuerAddressCity();
            empresaEmitente.address.city.name = Utilidades.String.Left(empresa.Localidade.Descricao, 60);
            empresaEmitente.address.city.code = empresa.Localidade.CodigoIBGE.ToString();
            empresaEmitente.address.city.state = empresa.Localidade.Estado.Sigla;
            empresaEmitente.address.zipCode = Utilidades.String.OnlyNumbers(empresa.CEP);
            empresaEmitente.address.phone = !string.IsNullOrWhiteSpace(empresa.Telefone) ? Utilidades.String.Left(Utilidades.String.OnlyNumbers(empresa.Telefone), 14) : null;
            empresaEmitente.address.email = !string.IsNullOrEmpty(empresa.Email) ? Utilidades.String.Left(empresa.Email.Trim(), 60) : null;

            return empresaEmitente;
        }

        private object obterEnvioMDFeModalInformation(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Embarcador.Cargas.Carga cargaMDFE, List<Dominio.Entidades.MDFeContratante> listaContratante, Dominio.Entidades.VeiculoMDFe veiculo, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, int quantidadeInfDoc, Repositorio.UnitOfWork unitOfWork)
        {
            object retorno = null;

            switch (this.obterModalMDFe(mdfe.Modal))
            {
                case Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModalMDFe.road:
                    retorno = this.obterEnvioMDFeModalInformationRoad(mdfe, listaContratante, cargaMDFE, veiculo, cargaCIOT, quantidadeInfDoc, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModalMDFe.water:
                    retorno = this.obterEnvioMDFeModalInformationWater(mdfe, unitOfWork);
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoad obterEnvioMDFeModalInformationRoad(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.MDFeContratante> listaContratante, Dominio.Entidades.Embarcador.Cargas.Carga cargaMDFE, Dominio.Entidades.VeiculoMDFe veiculo, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, int quantidadeInfDoc, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoad retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoad();
            retorno.regAgencyInfo = this.obterEnvioMDFeModalInformationRoadRegAgencyInfo(mdfe, cargaMDFE, listaContratante, veiculo, cargaCIOT, quantidadeInfDoc, unitOfWork);
            retorno.tractorVehicle = this.obterEnvioMDFeModalInformationRoadTractorVehicle(mdfe, veiculo, unitOfWork);
            retorno.trailerVehicle = this.obterEnvioMDFeModalInformationRoadTrailerVehicle(mdfe, unitOfWork);
            retorno.portSchedulingCode = null;
            retorno.seal = null;
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfo obterEnvioMDFeModalInformationRoadRegAgencyInfo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Embarcador.Cargas.Carga cargaMDFE, List<Dominio.Entidades.MDFeContratante> listaContratante, Dominio.Entidades.VeiculoMDFe veiculo, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, int quantidadeInfDoc, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ValePedagioMDFe repValePedagio = new Repositorio.ValePedagioMDFe(unitOfWork);
            List<Dominio.Entidades.ValePedagioMDFe> valesPedagio = repValePedagio.BuscarPorMDFe(mdfe.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfo retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfo();
            retorno.rntrc = mdfe.RNTRC;
            retorno.paymentInfo = this.ObterMdfeDataInfPag(mdfe, cargaMDFE, veiculo, cargaCIOT, quantidadeInfDoc, unitOfWork);
            retorno.ciotData = this.obterEnvioMDFeModalInformationRoadRegAgencyInfoCiotData(mdfe, unitOfWork);
            retorno.tollVoucherInfo = this.obterEnvioMDFeModalInformationRoadRegAgencyInfoTollVoucherInfo(mdfe, valesPedagio, unitOfWork);
            retorno.contractorInfo = this.obterEnvioMDFeModalInformationRoadRegAgencyInfoContractorInfo(mdfe, listaContratante, veiculo, cargaCIOT, unitOfWork);
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoCiotData> obterEnvioMDFeModalInformationRoadRegAgencyInfoCiotData(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MDFeCIOT repMDFeCIOT = new Repositorio.MDFeCIOT(unitOfWork);
            List<Dominio.Entidades.MDFeCIOT> listaCIOT = repMDFeCIOT.BuscarPorMDFe(mdfe.Codigo);

            if (listaCIOT != null && listaCIOT.Count() > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoCiotData> listaCIOTIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoCiotData>();

                foreach (Dominio.Entidades.MDFeCIOT ciot in listaCIOT)
                {
                    listaCIOTIntegrar.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoCiotData()
                    {
                        ciot = ciot.NumeroCIOT,
                        ciotIssuer = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoCiotDataCiotIssuer()
                        {
                            type = ciot.Responsavel.Length == 11 ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal,
                            document = ciot.Responsavel
                        }
                    });
                }

                return listaCIOTIntegrar;
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfo obterEnvioMDFeModalInformationRoadRegAgencyInfoTollVoucherInfo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ValePedagioMDFe> valesPedagio, Repositorio.UnitOfWork unitOfWork)
        {
            if (valesPedagio != null && valesPedagio.Count() > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfo retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfo();
                retorno.tollVoucherDeviceInfo = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfoTollVoucherDeviceInfo>();
                retorno.vehicleCategory = this.obterVehicleCategory(valesPedagio.FirstOrDefault().QuantidadeEixos);

                foreach (Dominio.Entidades.ValePedagioMDFe valePedagio in valesPedagio)
                {
                    if (retorno.tollVoucherDeviceInfo == null)
                        retorno.tollVoucherDeviceInfo = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfoTollVoucherDeviceInfo>();

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfoTollVoucherDeviceInfo registroValePedagio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfoTollVoucherDeviceInfo();
                    registroValePedagio.supplier = valePedagio.CNPJFornecedor;

                    if (!string.IsNullOrEmpty(valePedagio?.CNPJResponsavel))
                    {
                        registroValePedagio.payer = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfoTollVoucherDeviceInfoPayer()
                        {
                            type = valePedagio?.CNPJResponsavel?.Length == 11 ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal,
                            document = valePedagio?.CNPJResponsavel,
                        };
                    }

                    registroValePedagio.purchaseNumber = Utilidades.String.OnlyNumbers(valePedagio.NumeroComprovante);
                    registroValePedagio.tollVoucherValue = valePedagio.ValorValePedagio;
                    registroValePedagio.tollVoucherType = this.obterTollVoucherType(valePedagio.TipoCompra);

                    retorno.tollVoucherDeviceInfo.Add(registroValePedagio);
                }

                return retorno;
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoContractorInfo> obterEnvioMDFeModalInformationRoadRegAgencyInfoContractorInfo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.MDFeContratante> listaContratante, Dominio.Entidades.VeiculoMDFe veiculoMdfe, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new (unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoContractorInfo> listaContratanteIntegrar = new();

            if (veiculoMdfe.TipoProprietario == "0" || veiculoMdfe.TipoProprietario == "1" || veiculoMdfe.TipoProprietario == "2" || veiculoMdfe.TipoProprietario == "3")
            {
                Dominio.Entidades.Empresa contratante;
                if (repVeiculo.TipoTerceiro(veiculoMdfe.Placa))
                    contratante = mdfe.Empresa;
                else
                    contratante = cargaCIOT?.CIOT?.Contratante ?? mdfe.Empresa;

                listaContratanteIntegrar.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoContractorInfo()
                {
                    name = contratante.NomeCNPJ.Left(60),
                    contractorDocument = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoContractorInfoContractorDocument()
                    {
                        type = contratante.Tipo == "E" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.foreign : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.legal,
                        document = contratante.Tipo != "E" ? contratante.CNPJ_SemFormato : null,
                        foreignId = contratante.Tipo == "E" ? contratante.CNPJ_SemFormato : null,
                    }
                });
                return listaContratanteIntegrar;

            }
            else if (listaContratante != null && listaContratante.Count() > 0)
            {

                foreach (Dominio.Entidades.MDFeContratante contratante in listaContratante)
                {
                    if (!string.IsNullOrEmpty(contratante.IDEstrangeiro))
                    {
                        if (listaContratanteIntegrar.Exists(o => o.contractorDocument.foreignId == contratante.IDEstrangeiro))
                            continue;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(contratante.Contratante))
                            continue;

                        if (listaContratanteIntegrar.Exists(o => o.contractorDocument.document == contratante.Contratante))
                            continue;
                    }

                    listaContratanteIntegrar.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoContractorInfo()
                    {
                        name = contratante.NomeContratante.Left(60),
                        contractorDocument = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoContractorInfoContractorDocument()
                        {
                            type = !string.IsNullOrEmpty(contratante.IDEstrangeiro) ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.foreign : (contratante.Contratante.Length == 1 ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.legal),
                            document = string.IsNullOrEmpty(contratante.IDEstrangeiro) ? contratante.Contratante : null,
                            foreignId = !string.IsNullOrEmpty(contratante.IDEstrangeiro) ? contratante.IDEstrangeiro : null
                        }
                    });
                }

                return listaContratanteIntegrar;
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadTractorVehicle obterEnvioMDFeModalInformationRoadTractorVehicle(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.VeiculoMDFe veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            if (veiculo != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadTractorVehicle veiculoIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadTractorVehicle();

                veiculoIntegrar.internalCode = veiculo.Codigo.ToString();
                veiculoIntegrar.plate = veiculo.Placa;
                veiculoIntegrar.renavam = !string.IsNullOrWhiteSpace(veiculo.RENAVAM) && veiculo.RENAVAM.Length > 11 ? Utilidades.String.Right(veiculo.RENAVAM, 11) : veiculo.RENAVAM;
                veiculoIntegrar.tara = veiculo.Tara;
                veiculoIntegrar.kgCapacity = veiculo.CapacidadeKG;
                veiculoIntegrar.m3Capacity = veiculo.CapacidadeM3;
                veiculoIntegrar.owner = this.obterEnvioMDFeModalInformationRoadTractorVehicleOwner(veiculo);
                veiculoIntegrar.driver = this.obterEnvioMDFeModalInformationRoadTractorVehicleDriver(mdfe, unitOfWork);
                veiculoIntegrar.wheelType = this.obterWheelType(veiculo.TipoRodado);
                veiculoIntegrar.bodyType = this.obterBodyType(veiculo.TipoCarroceria);
                veiculoIntegrar.state = veiculo.UF != null ? veiculo.UF.Sigla : string.Empty;

                return veiculoIntegrar;
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadOwner obterEnvioMDFeModalInformationRoadTractorVehicleOwner(Dominio.Entidades.VeiculoMDFe veiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadOwner retorno = null;

            if (!string.IsNullOrEmpty(veiculo.CPFCNPJProprietario))
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadOwner();
                retorno.documentType = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadOwnerDocumentType();
                retorno.documentType.type = veiculo.CPFCNPJProprietario.Length == 11 ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
                retorno.documentType.document = veiculo.CPFCNPJProprietario;
                retorno.rntrc = veiculo.RNTRC;
                retorno.name = veiculo.NomeProprietario;
                retorno.stateRegistration = veiculo.IEProprietario;
                retorno.state = veiculo.UFProprietario != null ? veiculo.UFProprietario.Sigla : null;

                /*
                TACAgregado = 0,
                TACIndependente = 1,
                Outros = 2,
                NaoAplicado = 3,
                Todos = 4
                */
                if (veiculo.TipoProprietario == "0")
                    retorno.ownerType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumOwnerType.tac_aggregated;
                else if (veiculo.TipoProprietario == "1")
                    retorno.ownerType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumOwnerType.tac_independent;
                else
                    retorno.ownerType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumOwnerType.other;
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadDriver> obterEnvioMDFeModalInformationRoadTractorVehicleDriver(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MotoristaMDFe repMotorista = new Repositorio.MotoristaMDFe(unitOfWork);
            List<Dominio.Entidades.MotoristaMDFe> motoristas = repMotorista.BuscarPorMDFe(mdfe.Codigo);

            if (motoristas != null && motoristas.Count() > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadDriver> motoristasIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadDriver>();

                foreach (Dominio.Entidades.MotoristaMDFe motorista in motoristas)
                {
                    motoristasIntegrar.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadDriver()
                    {
                        document = motorista.CPF,
                        name = motorista.Nome
                    });
                }

                return motoristasIntegrar;
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadTrailerVehicle> obterEnvioMDFeModalInformationRoadTrailerVehicle(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ReboqueMDFe repReboque = new Repositorio.ReboqueMDFe(unitOfWork);
            List<Dominio.Entidades.ReboqueMDFe> reboques = repReboque.BuscarPorMDFe(mdfe.Codigo);

            if (reboques != null && reboques.Count() > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadTrailerVehicle> reboquesIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadTrailerVehicle>();

                foreach (Dominio.Entidades.ReboqueMDFe reboque in reboques)
                {
                    reboquesIntegrar.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadTrailerVehicle()
                    {
                        internalCode = reboque.Codigo.ToString(),
                        plate = reboque.Placa,
                        renavam = !string.IsNullOrWhiteSpace(reboque.RENAVAM) && reboque.RENAVAM.Length > 11 ? Utilidades.String.Right(reboque.RENAVAM, 11) : reboque.RENAVAM,
                        tara = reboque.Tara,
                        m3Capacity = reboque.CapacidadeM3,
                        owner = this.obterEnvioMDFeModalInformationRoadTrailerVehicleOwner(reboque),
                        bodyType = this.obterBodyType(reboque.TipoCarroceria),
                        state = reboque.UF != null ? reboque.UF.Sigla : string.Empty,
                        kgCapacity = reboque.CapacidadeKG
                    });
                }

                return reboquesIntegrar;
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadOwner obterEnvioMDFeModalInformationRoadTrailerVehicleOwner(Dominio.Entidades.ReboqueMDFe reboque)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadOwner retorno = null;

            if (!string.IsNullOrEmpty(reboque.CPFCNPJProprietario))
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadOwner();
                retorno.documentType = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadOwnerDocumentType();
                retorno.documentType.type = reboque.CPFCNPJProprietario.Length == 11 ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
                retorno.documentType.document = reboque.CPFCNPJProprietario;
                retorno.rntrc = reboque.RNTRC;
                retorno.name = reboque.NomeProprietario;
                retorno.stateRegistration = reboque.IEProprietario;
                retorno.state = reboque.UFProprietario != null ? reboque.UFProprietario.Sigla : null;

                /*
                TACAgregado = 0,
                TACIndependente = 1,
                Outros = 2,
                NaoAplicado = 3,
                Todos = 4
                */
                if (reboque.TipoProprietario == "0")
                    retorno.ownerType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumOwnerType.tac_aggregated;
                else if (reboque.TipoProprietario == "1")
                    retorno.ownerType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumOwnerType.tac_independent;
                else
                    retorno.ownerType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumOwnerType.other;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWater obterEnvioMDFeModalInformationWater(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWater retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWater();
            retorno.irin = !string.IsNullOrEmpty(mdfe.PedidoViagemNavio?.Navio?.Irin) ? mdfe.PedidoViagemNavio?.Navio?.Irin : null;
            retorno.vesselTypeCode = mdfe.PedidoViagemNavio != null && mdfe.PedidoViagemNavio.Navio != null ? "04" : null;//mdfe.PedidoViagemNavio?.Navio?.TipoEmbarcacao ?? "";
            retorno.vesselCode = !string.IsNullOrEmpty(mdfe.PedidoViagemNavio?.Navio?.CodigoIMO) ? mdfe.PedidoViagemNavio?.Navio?.CodigoIMO : null;
            retorno.vesselName = !string.IsNullOrEmpty(mdfe.PedidoViagemNavio?.Navio?.Descricao) ? mdfe.PedidoViagemNavio?.Navio?.Descricao : null;
            retorno.journeyNumber = mdfe.PedidoViagemNavio?.NumeroViagem.ToString("D");
            retorno.embarkationPortCode = !string.IsNullOrEmpty(mdfe.PortoOrigem?.CodigoIATA) ? mdfe.PortoOrigem?.CodigoIATA : null;
            retorno.destinationPortCode = !string.IsNullOrEmpty(mdfe.PortoDestino?.CodigoIATA) ? mdfe.PortoDestino?.CodigoIATA : null;
            retorno.transshipmentPort = null;
            retorno.navigationType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumNavigationType.cabotagem;
            retorno.loadingTerminalsInfo = this.obterEnvioMDFeModalInformationWaterLoadingTerminalsInfo(mdfe, unitOfWork);
            retorno.unloadingTerminalsInfo = this.obterEnvioMDFeModalInformationWaterUnloadingTerminalsInfo(mdfe, unitOfWork);
            retorno.convoyVesselInfo = null;
            retorno.emptyCargoUnitInfo = null;
            retorno.emptyTransportUnitInfo = null;

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWaterLoadingTerminalsInfo> obterEnvioMDFeModalInformationWaterLoadingTerminalsInfo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            if (mdfe.TerminalCarregamento != null && mdfe.TerminalCarregamento.Count() > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWaterLoadingTerminalsInfo> terminais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWaterLoadingTerminalsInfo>();

                foreach (var terminal in mdfe.TerminalCarregamento)
                {
                    terminais.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWaterLoadingTerminalsInfo()
                    {
                        loadingTerminalCode = terminal.CodigoTerminal,
                        loadingTerminalName = terminal.Descricao
                    });
                }

                return terminais;
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWaterUnloadingTerminalsInfo> obterEnvioMDFeModalInformationWaterUnloadingTerminalsInfo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            if (mdfe.TerminalDescarregamento != null && mdfe.TerminalDescarregamento.Count() > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWaterUnloadingTerminalsInfo> terminais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWaterUnloadingTerminalsInfo>();

                foreach (var terminal in mdfe.TerminalDescarregamento)
                {
                    terminais.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationWaterUnloadingTerminalsInfo()
                    {
                        unloadingTerminalCode = terminal.CodigoTerminal,
                        unloadingTerminalName = terminal.Descricao
                    });
                }

                return terminais;
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataLoadingCitiesInfo> obterEnvioMDFeLoadingCitiesInfo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, out string cepMunicipioCarregamento, out decimal latitudeMunicipioCarregamento, out decimal longitudeMunicipioCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MunicipioCarregamentoMDFe repMunicipio = new Repositorio.MunicipioCarregamentoMDFe(unitOfWork);
            List<Dominio.Entidades.MunicipioCarregamentoMDFe> municipios = repMunicipio.BuscarPorMDFe(mdfe.Codigo);
            cepMunicipioCarregamento = string.Empty;
            latitudeMunicipioCarregamento = 0;
            longitudeMunicipioCarregamento = 0;

            if (municipios != null && municipios.Count() > 0)
            {
                cepMunicipioCarregamento = Utilidades.String.OnlyNumbers(municipios.FirstOrDefault().Municipio.CEP).PadLeft(8, '0').Left(8);
                latitudeMunicipioCarregamento = municipios.FirstOrDefault().Municipio.Latitude ?? 0;
                longitudeMunicipioCarregamento = municipios.FirstOrDefault().Municipio.Longitude ?? 0;

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataLoadingCitiesInfo> municipiosIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataLoadingCitiesInfo>();

                foreach (Dominio.Entidades.MunicipioCarregamentoMDFe municipio in municipios)
                {
                    municipiosIntegrar.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataLoadingCitiesInfo()
                    {
                        code = municipio.Municipio.CodigoIBGE.ToString(),
                        name = municipio.Municipio.Descricao
                    });
                }

                return municipiosIntegrar;
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfo obterEnvioMDFeDocInfo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, out string cepMunicipioDescarregamento, out decimal latitudeMunicipioDescarregamento, out decimal longitudeMunicipioDescarregamento, ref int qtdCte, ref int qtdNfe, ref int qtdMdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MunicipioDescarregamentoMDFe repMunicipio = new Repositorio.MunicipioDescarregamentoMDFe(unitOfWork);
            List<Dominio.Entidades.MunicipioDescarregamentoMDFe> municipios = repMunicipio.BuscarPorMDFe(mdfe.Codigo);
            cepMunicipioDescarregamento = string.Empty;
            latitudeMunicipioDescarregamento = 0;
            longitudeMunicipioDescarregamento = 0;

            if (municipios != null && municipios.Count() > 0)
            {
                cepMunicipioDescarregamento = Utilidades.String.OnlyNumbers(municipios.FirstOrDefault().Municipio.CEP ?? "").PadLeft(8, '0').Left(8);
                latitudeMunicipioDescarregamento = municipios.FirstOrDefault().Municipio.Latitude ?? 0;
                longitudeMunicipioDescarregamento = municipios.FirstOrDefault().Municipio.Longitude ?? 0;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfo retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfo();
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfo> municipiosIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfo>();

                foreach (Dominio.Entidades.MunicipioDescarregamentoMDFe municipio in municipios)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfo unloadCityInfo = this.obterEnvioMDFeDocInfoUnloadCityInfo(municipio, ref qtdCte, ref qtdNfe, ref qtdMdfe, unitOfWork);

                    if (unloadCityInfo != null)
                        municipiosIntegrar.Add(unloadCityInfo);
                }

                retorno.unloadCityInfo = municipiosIntegrar;
                return retorno;
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfo obterEnvioMDFeDocInfoUnloadCityInfo(Dominio.Entidades.MunicipioDescarregamentoMDFe municipio, ref int qtdCte, ref int qtdNfe, ref int qtdMdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfo retorno = null;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCte> infCte = null;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfNfe> infNfe = null;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfMdfe> infMdfe = null;

            Repositorio.NotaFiscalEletronicaMDFe repNotaFiscalMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unitOfWork);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
            Repositorio.CTeMDFe repCTeMDFe = new Repositorio.CTeMDFe(unitOfWork);

            List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentos = repDocumento.BuscarPorMunicipio(municipio.Codigo);
            List<Dominio.Entidades.NotaFiscalEletronicaMDFe> notasFiscais = repNotaFiscalMDFe.BuscarPorMunicipio(municipio.Codigo);
            List<Dominio.Entidades.CTeMDFe> chavesCTes = repCTeMDFe.BuscarPorMunicipio(municipio.Codigo);

            if (documentos != null && documentos.Count() > 0)
            {
                infCte = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCte>();

                int contadorCTeTerceiro = 0;
                foreach (Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento in documentos)
                {
                    if (documento.CTe != null)
                    {
                        qtdCte++;
                        infCte.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCte()
                        {
                            key = documento.CTe.Chave,
                            barcode = documento.CTe.TipoEmissao == "5" ? documento.CTe.ChaveContingencia : null,
                            hazmat = this.obterEnvioMDFeDocInfoUnloadCityInfoHazmat(documento, unitOfWork),
                            transportUnitInfo = this.obterEnvioMDFeDocInfoUnloadCityInfoTransportUnitInfo(documento, unitOfWork)
                        });
                    }
                    else if (documento.CTeTerceiro != null)
                    {
                        qtdCte++;
                        contadorCTeTerceiro++;
                        infCte.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCte()
                        {
                            key = documento.CTeTerceiro.ChaveAcesso,
                        });

                        if (contadorCTeTerceiro == 1000)
                            break;
                    }
                }
            }

            if (chavesCTes != null && chavesCTes.Count() > 0)
            {
                if (infCte == null)
                    infCte = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCte>();

                foreach (Dominio.Entidades.CTeMDFe cte in chavesCTes)
                {
                    qtdCte++;
                    infCte.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCte()
                    {
                        key = cte.Chave
                    });
                }
            }

            if (notasFiscais != null && notasFiscais.Count() > 0)
            {
                infNfe = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfNfe>();

                foreach (Dominio.Entidades.NotaFiscalEletronicaMDFe notaFiscal in notasFiscais)
                {
                    qtdNfe++;
                    infNfe.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfNfe()
                    {
                        key = notaFiscal.Chave,
                        barcode = !string.IsNullOrWhiteSpace(notaFiscal.SegundoCodigoDeBarra) ? notaFiscal.SegundoCodigoDeBarra : null
                    });
                }
            }

            if (infCte != null || infNfe != null || infMdfe != null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfo();
                retorno.code = municipio.Municipio.CodigoIBGE.ToString();
                retorno.name = municipio.Municipio.Descricao;
                retorno.infCte = infCte;
                retorno.infNfe = infNfe;
                retorno.infMdfe = infMdfe;
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteHazmat> obterEnvioMDFeDocInfoUnloadCityInfoHazmat(Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos repProdPerigosos = new Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos(unitOfWork);
            List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos> produtosPerigosos = repProdPerigosos.BuscarPorDocumento(documento.Codigo);

            if (produtosPerigosos != null && produtosPerigosos.Count() > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteHazmat> prodPerigosoIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteHazmat>();

                foreach (Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos produto in produtosPerigosos)
                {
                    prodPerigosoIntegrar.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteHazmat()
                    {
                        onuNumber = produto.NumeroONU,
                        productShippingName = produto.Nome,
                        hazardClass = produto.ClasseRisco,
                        packagingGroup = produto.GrupoEmbalagem,
                        totalQtyProduct = !string.IsNullOrEmpty(produto.QuantidadeTotalProduto) ? Convert.ToInt32(produto.QuantidadeTotalProduto) : 0,
                        qtyVolumeType = produto.QuantidadeTipoVolumes
                    });
                }

                return prodPerigosoIntegrar;
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteTransportUnitInfo> obterEnvioMDFeDocInfoUnloadCityInfoTransportUnitInfo(Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);

            if (documento.CTe != null && documento.CTe.Viagem != null && documento.CTe.Viagem.Navio != null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteTransportUnitInfo> unidadesIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteTransportUnitInfo>();

                unidadesIntegrar.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteTransportUnitInfo()
                {
                    transportUnitType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTransportUnitTypeMDFe.ship,
                    transportUnitId = documento.CTe?.Viagem?.Navio?.Irin ?? null,
                    transportUnitSeal = this.obterEnvioMDFeDocInfoUnloadCityInfoTransportUnitInfoTransportUnitSeal(documento, unitOfWork),
                    loadUnit = this.obterEnvioMDFeDocInfoUnloadCityInfoTransportUnitInfoLoadUnit(documento, unitOfWork),
                });

                return unidadesIntegrar;
            }

            return null;
        }

        private List<string> obterEnvioMDFeDocInfoUnloadCityInfoTransportUnitInfoTransportUnitSeal(Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento, Repositorio.UnitOfWork unitOfWork)
        {
            if (documento.CTe != null && documento.CTe.Viagem != null && documento.CTe.Viagem.Navio != null)
            {
                List<string> lacresIntegrar = new List<string>();
                lacresIntegrar.Add(documento.CTe?.Viagem?.Navio?.Irin);
                return lacresIntegrar;
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteLoadUnit> obterEnvioMDFeDocInfoUnloadCityInfoTransportUnitInfoLoadUnit(Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);

            if (documento.CTe != null)
            {
                List<Dominio.Entidades.ContainerCTE> listaContainers = repContainerCTE.BuscarPorCTe(documento.CTe.Codigo);

                if (listaContainers != null && listaContainers.Count > 0)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteLoadUnit> unidadesIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteLoadUnit>();

                    foreach (var container in listaContainers)
                    {
                        unidadesIntegrar.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDocInfoUnloadCityInfoInfCteLoadUnit()
                        {
                            loadUnitType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadUnitType.container,
                            loadUnitId = (Utilidades.String.RemoveDiacritics(container.Container?.Numero ?? container.Numero)?.Replace("-", "").Replace(" ", "") ?? "").Trim(),
                            loadUnitSeal = this.obterEnvioMDFeDocInfoUnloadCityInfoTransportUnitInfoLoadUnitLoadUnitSeal(container, unitOfWork),
                            allocatedQty = 0
                        });
                    }

                    return unidadesIntegrar;
                }
            }

            return null;
        }

        private List<string> obterEnvioMDFeDocInfoUnloadCityInfoTransportUnitInfoLoadUnitLoadUnitSeal(Dominio.Entidades.ContainerCTE container, Repositorio.UnitOfWork unitOfWork)
        {
            if (container != null)
            {
                List<string> lacresIntegrar = new List<string>();

                if (!string.IsNullOrWhiteSpace(container.Lacre1))
                    lacresIntegrar.Add(Utilidades.String.RemoveDiacritics(container.Lacre1));
                if (!string.IsNullOrWhiteSpace(container.Lacre2))
                    lacresIntegrar.Add(Utilidades.String.RemoveDiacritics(container.Lacre2));
                if (!string.IsNullOrWhiteSpace(container.Lacre3))
                    lacresIntegrar.Add(Utilidades.String.RemoveDiacritics(container.Lacre3));

                return lacresIntegrar;
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataInsurance> obterEnvioMDFeInsurance(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unitOfWork);
            List<Dominio.Entidades.MDFeSeguro> listaSeguro = repMDFeSeguro.BuscarPorMDFe(mdfe.Codigo);

            if (listaSeguro != null && listaSeguro.Count() > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataInsurance> listaSeguroIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataInsurance>();

                foreach (Dominio.Entidades.MDFeSeguro seguro in listaSeguro)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataInsurance insurance = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataInsurance();

                    insurance.infResp = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataInsuranceInfResp();
                    insurance.infResp.respSeg = seguro.TipoResponsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumRespSeg.contractor : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumRespSeg.issuer;

                    string cnpjCpfResponsavelSeguro = this.CNPJResponsavelSeguro(seguro.Responsavel, mdfe);
                    insurance.infResp.respSegPersonDocument = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataInsuranceInfRespRespSegPersonDocument();
                    insurance.infResp.respSegPersonDocument.type = cnpjCpfResponsavelSeguro.Length == 11 ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
                    insurance.infResp.respSegPersonDocument.document = cnpjCpfResponsavelSeguro;

                    if (!string.IsNullOrEmpty(seguro.CNPJSeguradora))
                    {
                        insurance.infSeg = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataInsuranceInfSeg();
                        insurance.infSeg.document = seguro.CNPJSeguradora;
                        insurance.infSeg.name = seguro.NomeSeguradora.Trim();
                    }

                    string numeroApolice = !string.IsNullOrEmpty(seguro.NumeroApolice) ? seguro.NumeroApolice.Trim() : null;

                    insurance.policyNumber = numeroApolice;

                    if (!string.IsNullOrEmpty(seguro.NumeroAverbacao))
                    {
                        insurance.endorsementNumber = new List<string>();
                        insurance.endorsementNumber.Add(this.NumeroAverbacaoSeguro(seguro.NumeroAverbacao.Trim(), numeroApolice, mdfe));
                    }

                    listaSeguroIntegrar.Add(insurance);
                }

                return listaSeguroIntegrar;
            }
            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProduct obterEnvioMDFePredProduct(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, string cepMunicipioCarregamento, decimal latitudeMunicipioCarregamento, decimal longitudeMunicipioCarregamento, string cepMunicipioDescarregamento, decimal latitudeMunicipioDescarregamento, decimal longitudeMunicipioDescarregamento, Dominio.Entidades.Embarcador.Cargas.Carga cargaMDFE, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProduct produtoPredominante = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProduct();

            produtoPredominante.loadType = this.obterLoadType(mdfe.TipoCargaMDFe);
            produtoPredominante.loadDesc = !string.IsNullOrWhiteSpace(mdfe.ProdutoPredominanteDescricao) ? mdfe.ProdutoPredominanteDescricao : "DIVERSOS";
            produtoPredominante.cEAN = mdfe.ProdutoPredominanteCEAN;
            produtoPredominante.NCM = mdfe.ProdutoPredominanteNCM;

            object loadPlaceInfo = null;
            if (mdfe.EstadoCarregamento.Sigla == "EX" && string.IsNullOrWhiteSpace(mdfe.CEPCarregamentoLotacao) && !string.IsNullOrWhiteSpace(mdfe.Empresa.Localidade.CEP) && (!mdfe.LatitudeCarregamentoLotacao.HasValue || mdfe.LatitudeCarregamentoLotacao == 0))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode carregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode();
                carregamento.zipCode = Utilidades.String.OnlyNumbers(mdfe.Empresa.Localidade.CEP).PadLeft(8, '0').Left(8);
                loadPlaceInfo = carregamento;
            }
            else if (mdfe.LatitudeCarregamentoLotacao.HasValue && mdfe.LatitudeCarregamentoLotacao != 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong carregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong();
                carregamento.latitude = (mdfe.LatitudeCarregamentoLotacao.HasValue ? mdfe.LatitudeCarregamentoLotacao.Value : 0).ToString("n6").Replace(",", ".");
                carregamento.longitude = (mdfe.LongitudeCarregamentoLotacao.HasValue ? mdfe.LongitudeCarregamentoLotacao.Value : 0).ToString("n6").Replace(",", ".");
                loadPlaceInfo = carregamento;
            }
            else if (!string.IsNullOrWhiteSpace(mdfe.CEPCarregamentoLotacao))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode carregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode();
                carregamento.zipCode = Utilidades.String.OnlyNumbers(mdfe.CEPCarregamentoLotacao).PadLeft(8, '0').Left(8);
                loadPlaceInfo = carregamento;
            }
            else if (mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte && latitudeMunicipioCarregamento != 0 && longitudeMunicipioCarregamento != 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong carregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong();
                carregamento.latitude = latitudeMunicipioCarregamento.ToString("n6").Replace(",", ".");
                carregamento.longitude = longitudeMunicipioCarregamento.ToString("n6").Replace(",", ".");
                loadPlaceInfo = carregamento;
            }
            else if (mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte && !string.IsNullOrWhiteSpace(cepMunicipioCarregamento))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode carregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode();
                carregamento.zipCode = Utilidades.String.OnlyNumbers(cepMunicipioCarregamento).PadLeft(8, '0').Left(8);
                loadPlaceInfo = carregamento;
            }
            //Enviar coordenadas do transportador quando ficou sem CEP ou Latitude
            else if (mdfe.Empresa.Localidade.Latitude.HasValue && mdfe.Empresa.Localidade.Latitude != 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong carregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong();
                carregamento.latitude = (mdfe.Empresa.Localidade.Latitude.HasValue ? mdfe.Empresa.Localidade.Latitude.Value : 0).ToString("n6").Replace(",", ".");
                carregamento.longitude = (mdfe.Empresa.Localidade.Longitude.HasValue ? mdfe.Empresa.Localidade.Longitude.Value : 0).ToString("n6").Replace(",", ".");
                loadPlaceInfo = carregamento;
            }
            else if (!string.IsNullOrWhiteSpace(mdfe.Empresa.Localidade.CEP))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode carregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode();
                carregamento.zipCode = Utilidades.String.OnlyNumbers(mdfe.Empresa.Localidade.CEP).PadLeft(8, '0').Left(8);
                loadPlaceInfo = carregamento;
            }

            object unloadPlaceInfo = null;
            if (mdfe.EstadoDescarregamento.Sigla == "EX" && string.IsNullOrWhiteSpace(mdfe.CEPDescarregamentoLotacao) && !string.IsNullOrWhiteSpace(mdfe.Empresa.Localidade.CEP) && (!mdfe.LatitudeDescarregamentoLotacao.HasValue || mdfe.LatitudeDescarregamentoLotacao == 0))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode descarregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode();
                descarregamento.zipCode = Utilidades.String.OnlyNumbers(mdfe.Empresa.Localidade.CEP).PadLeft(8, '0').Left(8);
                unloadPlaceInfo = descarregamento;
            }
            else if (mdfe.LatitudeDescarregamentoLotacao.HasValue && mdfe.LatitudeDescarregamentoLotacao != 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong descarregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong();
                descarregamento.latitude = (mdfe.LatitudeDescarregamentoLotacao.HasValue ? mdfe.LatitudeDescarregamentoLotacao.Value : 0).ToString("n6").Replace(",", ".");
                descarregamento.longitude = (mdfe.LatitudeDescarregamentoLotacao.HasValue ? mdfe.LatitudeDescarregamentoLotacao.Value : 0).ToString("n6").Replace(",", ".");
                unloadPlaceInfo = descarregamento;
            }
            else if (!string.IsNullOrWhiteSpace(mdfe.CEPDescarregamentoLotacao))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode descarregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode();
                descarregamento.zipCode = Utilidades.String.OnlyNumbers(mdfe.CEPDescarregamentoLotacao).PadLeft(8, '0').Left(8);
                unloadPlaceInfo = descarregamento;
            }
            else if (mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte && latitudeMunicipioDescarregamento != 0 && longitudeMunicipioDescarregamento != 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong descarregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong();
                descarregamento.latitude = longitudeMunicipioDescarregamento.ToString("n6").Replace(",", ".");
                descarregamento.longitude = longitudeMunicipioDescarregamento.ToString("n6").Replace(",", ".");
                unloadPlaceInfo = descarregamento;
            }
            else if (mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte && !string.IsNullOrWhiteSpace(cepMunicipioDescarregamento))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode descarregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode();
                descarregamento.zipCode = Utilidades.String.OnlyNumbers(cepMunicipioDescarregamento).PadLeft(8, '0').Left(8);
                unloadPlaceInfo = descarregamento;
            }
            //Enviar coordenadas do transportador quando ficou sem CEP ou Latitude
            else if (mdfe.Empresa.Localidade.Latitude.HasValue && mdfe.Empresa.Localidade.Latitude != 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong descarregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoLatLong();
                descarregamento.latitude = (mdfe.Empresa.Localidade.Latitude.HasValue ? mdfe.Empresa.Localidade.Latitude.Value : 0).ToString("n6").Replace(",", ".");
                descarregamento.longitude = (mdfe.Empresa.Localidade.Longitude.HasValue ? mdfe.Empresa.Localidade.Longitude.Value : 0).ToString("n6").Replace(",", ".");
                unloadPlaceInfo = descarregamento;
            }
            else if (!string.IsNullOrWhiteSpace(mdfe.Empresa.Localidade.CEP))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode descarregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoadPlaceInfoZipcode();
                descarregamento.zipCode = Utilidades.String.OnlyNumbers(mdfe.Empresa.Localidade.CEP).PadLeft(8, '0').Left(8);
                unloadPlaceInfo = descarregamento;
            }

            if (loadPlaceInfo != null && unloadPlaceInfo != null)
            {
                produtoPredominante.infCargoLoad = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataPredProductInfCargoLoad();
                produtoPredominante.infCargoLoad.loadPlaceInfo = loadPlaceInfo;
                produtoPredominante.infCargoLoad.unloadPlaceInfo = unloadPlaceInfo;
            }

            return produtoPredominante;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataTot obterEnvioMDFeTot(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int qtdCte, int qtdNfe, int qtdMdfe)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataTot retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataTot();
            retorno.cteQty = qtdCte;
            retorno.nfeQty = qtdNfe;
            retorno.mdfeQty = qtdMdfe;
            retorno.loadValue = mdfe.ValorTotalMercadoria;
            retorno.unitCode = mdfe.UnidadeMedidaMercadoria == UnidadeMedidaMDFe.KG ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumUnitCode.kg : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumUnitCode.ton;
            retorno.grossWeight = mdfe.PesoBrutoMercadoria;
            return retorno;
        }

        private List<string> obterEnvioMDFeSeal(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.LacreMDFe repLacre = new Repositorio.LacreMDFe(unitOfWork);
            List<Dominio.Entidades.LacreMDFe> lacres = repLacre.BuscarPorMDFe(mdfe.Codigo);

            if (lacres != null && lacres.Count() > 0)
            {
                List<string> lacresIntegrar = new List<string>();

                foreach (Dominio.Entidades.LacreMDFe lacre in lacres)
                {
                    lacresIntegrar.Add(lacre.Numero);
                }

                return lacresIntegrar;
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDownloadAuthorization> obterEnvioMDFeDownloadAuthorization(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.MDFeContratante> listaContratante, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDownloadAuthorization> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDownloadAuthorization>();

            // Emitente MDFe
            retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDownloadAuthorization()
            {
                type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal,
                document = mdfe.Empresa.CNPJ
            });

            // ANTT
            retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDownloadAuthorization()
            {
                type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal,
                document = "04898488000177"
            });

            // TECH
            if (!string.IsNullOrEmpty(_configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoCNPJ))
            {
                retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDownloadAuthorization()
                {
                    type = _configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoCNPJ.Length == 11 ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal,
                    document = _configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoCNPJ
                });
            }

            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
            IList<string> cnpjsAutorizados = repDocMDFe.BuscarCNPJsAutorizadosDFe(mdfe.Codigo);

            if (cnpjsAutorizados != null)
            {
                foreach (string cnpjCpf in cnpjsAutorizados)
                {
                    if (!retorno.Exists(obj => obj.document == cnpjCpf))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDownloadAuthorization autorizado = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataDownloadAuthorization();
                        autorizado.type = cnpjCpf.Length == 11 ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
                        autorizado.document = cnpjCpf;
                        retorno.Add(autorizado);
                    }
                }
            }

            return retorno.Take(10).ToList();
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataAdditionalInfo obterEnvioMDFeAdditionalInfo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataAdditionalInfo retorno = null;

            string observacaoContribuinte = Utilidades.String.RemoveDiacritics(mdfe.ObservacaoContribuinte);
            string observacaoFisco = Utilidades.String.RemoveDiacritics(mdfe.ObservacaoFisco);

            if (!string.IsNullOrEmpty(observacaoFisco) || !string.IsNullOrEmpty(observacaoContribuinte))
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataAdditionalInfo();
                retorno.fiscalInfo = !string.IsNullOrWhiteSpace(observacaoFisco) ? observacaoFisco.Substring(0, Math.Min(observacaoFisco.Length, 2000)) : null;
                retorno.cplInfo = !string.IsNullOrWhiteSpace(observacaoContribuinte) ? observacaoContribuinte.Substring(0, Math.Min(observacaoContribuinte.Length, 4000)).Replace("\n", " ").Trim() : null;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataTechnicalManager obterEnvioMDFeTechnicalManager()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataTechnicalManager retorno = null;
            if (!string.IsNullOrEmpty(_configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoCNPJ))
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataTechnicalManager();
                retorno.document = _configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoCNPJ;
                retorno.name = _configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoNomeContato;
                retorno.email = _configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoEmail;
                retorno.phone = _configuracaoIntegracaoEmissorDocumento.ResponsavelTecnicoTelefone;
            }
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfo> ObterMdfeDataInfPag(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Embarcador.Cargas.Carga cargaMDFE, Dominio.Entidades.VeiculoMDFe veiculo, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, int quantidadeInfDoc, Repositorio.UnitOfWork unitOfWork)
        {
            bool deveInformarPagamento = DeveInformarPagamento(veiculo, mdfe, quantidadeInfDoc, unitOfWork);

            if (!deveInformarPagamento) 
                return null;

            Dominio.Entidades.MDFeInformacoesBancarias mdfeInformacoesBancarias = new Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias(unitOfWork).BuscarPorMDFe(mdfe.Codigo);

            if (mdfeInformacoesBancarias == null)
                return null;
            
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoPaymentComponents> paymentComponents = ObterPaymentComponents(mdfeInformacoesBancarias, unitOfWork);

            decimal valorFrete = paymentComponents.Where(x => x.type == Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPaymentComponentType.freight).Sum(x => x.value);
            decimal valorICMS = paymentComponents.Where(x => x.type == Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPaymentComponentType.taxes).Sum(x => x.value);
            decimal valesPedagio = paymentComponents.Where(x => x.type == Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPaymentComponentType.toll_voucher).Sum(x => x.value);
            decimal valorOutros = paymentComponents.Where(x => x.type == Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPaymentComponentType.other).Sum(x => x.value);
            decimal valorContrato = valorFrete + valorICMS + valesPedagio + valorOutros;

            List<Dominio.ObjetosDeValor.ContratantesMDFeGroupByCnpjNome> contratantes = new Repositorio.MDFeContratante(unitOfWork).BuscarPorMDFeGroupByCnpjNome(mdfe.Codigo);
            Dominio.Entidades.Empresa contratante = veiculo.TipoProprietario == "0" || veiculo.TipoProprietario == "1" ? (cargaCIOT?.CIOT?.Contratante ?? mdfe.Empresa) : mdfe.Empresa;

            string nameInfoPayment = veiculo.TipoProprietario == "0" || veiculo.TipoProprietario == "1" ? contratante.RazaoSocial : !string.IsNullOrWhiteSpace(contratantes.FirstOrDefault()?.NomeContratante) ? contratantes.FirstOrDefault().NomeContratante : (cargaMDFE?.Filial?.Descricao ?? cargaMDFE?.Empresa?.RazaoSocial ?? "");

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfo> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfo>() {
                new()
                {
                    name = nameInfoPayment.Left(60),
                    personPayment = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoPersonPayment
                    {
                        document = veiculo.TipoProprietario == "0" || veiculo.TipoProprietario == "1" ? contratante.CNPJ_SemFormato : !string.IsNullOrWhiteSpace(contratantes.FirstOrDefault()?.CnpjContratante) ? contratantes.FirstOrDefault().CnpjContratante : (cargaMDFE?.Filial?.CNPJ ?? cargaMDFE?.Empresa?.CNPJ ?? ""),
                        type = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumParticipantType.legal
                    },
                    contractValue = valorContrato,
                    highPerformance = (mdfeInformacoesBancarias?.IndicadorAltoDesempenho ?? false) ? "1" : null,
                    paymentMethod = ObterEnumpaymentMethod(mdfeInformacoesBancarias),
                    paymentComponents = paymentComponents,
                    prepaymentType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPrepaymentType.do_not_allow,
                    advanceAmount = mdfeInformacoesBancarias.ValorAdiantamento ?? 0,
                    advancePrepaymentInd = mdfeInformacoesBancarias.ValorAdiantamento > 0 ? "1" : null,
                    installmentDetail = ObterInstallmentDetail(mdfeInformacoesBancarias, unitOfWork),
                    bankDetail = ObterBankDetail(mdfeInformacoesBancarias)
                }
            };

            return retorno;
        }

        private bool DeveInformarPagamento(Dominio.Entidades.VeiculoMDFe veiculo, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int quantidadeInfDoc, Repositorio.UnitOfWork unitOfWork)
        {
            bool temCIOT = new Repositorio.MDFeCIOT(unitOfWork).ExistePorMDFe(mdfe.Codigo);
            bool tpTranspInformado = false;

            if (!string.IsNullOrWhiteSpace(veiculo?.CPFCNPJProprietario))
            {
                string cpfCnpjProprietario = veiculo.CPFCNPJProprietario.ObterSomenteNumeros(); 
                tpTranspInformado = cpfCnpjProprietario.Length == 14 || cpfCnpjProprietario.Length == 11;
            }
            
            return temCIOT || (quantidadeInfDoc == 1 && (mdfe.TipoEmitente != TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte || tpTranspInformado));
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumpaymentMethod ObterEnumpaymentMethod(Dominio.Entidades.MDFeInformacoesBancarias mdfeInformacoesBancarias)
            => mdfeInformacoesBancarias.TipoPagamento switch
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento.Prazo => Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumpaymentMethod.installment_payment,
                _ => Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumpaymentMethod.cash_payment
            };

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoPaymentComponents> ObterPaymentComponents(Dominio.Entidades.MDFeInformacoesBancarias mdfeInformacoesBancarias, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.MDFE.MDFePagamentoComponente repPagamentoComponente = new Repositorio.Embarcador.MDFE.MDFePagamentoComponente(unitOfWork);
            List<Dominio.Entidades.MDFePagamentoComponente> pagamentoComponentes = repPagamentoComponente.BuscarPorInformacoesBancarias(mdfeInformacoesBancarias.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoPaymentComponents> listaPaymentComponents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoPaymentComponents>();
            foreach (Dominio.Entidades.MDFePagamentoComponente pagamentoComponente in pagamentoComponentes)
            {
                if (pagamentoComponente.ValorComponente > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPaymentComponentType type = pagamentoComponente.TipoComponente switch
                    {
                        Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.ValePedagio => Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPaymentComponentType.toll_voucher,
                        Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.Impostos => Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPaymentComponentType.taxes,
                        Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.FreteValor => Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPaymentComponentType.freight,
                        Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.Despesas => Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPaymentComponentType.expenses,
                        _ => Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumPaymentComponentType.other
                    };

                    string tipoDescricao = pagamentoComponente.TipoComponente switch
                    {
                        Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.ValePedagio => "Pedagio",
                        Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.Impostos => "ICMS",
                        Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.FreteValor => "Frete",
                        Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.Despesas => "Despesas",
                        _ => "Outros"
                    };

                    listaPaymentComponents.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoPaymentComponents
                    {
                        value = pagamentoComponente.ValorComponente ?? 0,
                        type = type,
                        description = $"{tipoDescricao} no valor de {pagamentoComponente.ValorComponente ?? 0:N4}"
                    });
                }
            }
            return listaPaymentComponents;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoInstallmentDetail> ObterInstallmentDetail(Dominio.Entidades.MDFeInformacoesBancarias mdfeInformacoesBancarias, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.MDFE.MDFePagamentoParcela repPagamentoParcela = new Repositorio.Embarcador.MDFE.MDFePagamentoParcela(unitOfWork);
            List<Dominio.Entidades.MDFePagamentoParcela> pagamentoParcelas = repPagamentoParcela.BuscarPorInformacoesBancarias(mdfeInformacoesBancarias.Codigo);
            if (!pagamentoParcelas.Any()) return null;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoInstallmentDetail> listaInstallmentDetail = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoInstallmentDetail>();
            foreach (Dominio.Entidades.MDFePagamentoParcela pagamentoParcela in pagamentoParcelas)
                listaInstallmentDetail.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoInstallmentDetail
                {
                    dueDate = (pagamentoParcela.DataVencimentoParcela ?? DateTime.Now).ToString("yyyy-MM-dd"),
                    number = (pagamentoParcela.NumeroParcela ?? 1).ToString().PadLeft(3, '0'),
                    value = pagamentoParcela.ValorParcela ?? 0
                });
            return listaInstallmentDetail;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoBankDetail ObterBankDetail(Dominio.Entidades.MDFeInformacoesBancarias mdfeInformacoesBancarias)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBankDetailType type = mdfeInformacoesBancarias.TipoInformacaoBancaria switch
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe.PIX => Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBankDetailType.pix,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe.Ipef => Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBankDetailType.ipef,
                _ => Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBankDetailType.bank
            };


            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoBankDetail
            {
                bankCode = !string.IsNullOrEmpty(mdfeInformacoesBancarias.Conta) ? mdfeInformacoesBancarias.Conta : "000",
                type = type == Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBankDetailType.bank && (!string.IsNullOrEmpty(mdfeInformacoesBancarias.Conta) || !string.IsNullOrEmpty(mdfeInformacoesBancarias.Agencia)) ? !string.IsNullOrEmpty(mdfeInformacoesBancarias.ChavePIX) ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBankDetailType.pix : !string.IsNullOrEmpty(mdfeInformacoesBancarias.Ipef) ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBankDetailType.ipef : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBankDetailType.bank : type,
                agencyCode = !string.IsNullOrEmpty(mdfeInformacoesBancarias.Agencia) ? mdfeInformacoesBancarias.Agencia : "000",
                document = !string.IsNullOrEmpty(mdfeInformacoesBancarias?.Ipef ?? "") ? mdfeInformacoesBancarias.Ipef.ObterSomenteNumeros() : "",
                pix = mdfeInformacoesBancarias.ChavePIX
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfe obterEnvioMDFeOptionsDacte(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfe retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfe();
            retorno.enabled = true;
            retorno.notifications = this.obterEnvioMDFeOptionsDamdfeNotifications(mdfe, unitOfWork);

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfeNotifications obterEnvioMDFeOptionsDamdfeNotifications(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfeNotifications retorno = null;

            string emails = obterEmailParaEnvioDADFE(mdfe.Empresa);

            if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnviarEmailMDFeClientes.Value)
                emails += this.ObterEmailsClientes(mdfe, unitOfWork);

            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasMDFe = repDocMDFe.BuscarGrupoPessoasPorMDFe(mdfe.Codigo);
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail grupoPessoasModeloDocumentoEmail = grupoPessoasMDFe?.EmailsModeloDocumento?.Where(o => o.ModeloDocumentoFiscal == mdfe.Modelo).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(grupoPessoasModeloDocumentoEmail?.Emails))
                emails += grupoPessoasModeloDocumentoEmail.Emails + ";";

            if (string.IsNullOrEmpty(emails))
                return retorno;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfeNotificationsContacts> contatos = this.obterEnvioMDFeOptionsDamdfeNotificationsContacts(emails);

            if (contatos?.Count > 0)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfeNotifications();
                retorno.enabled = true;
                retorno.contacts = contatos;
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfeNotificationsContacts> obterEnvioMDFeOptionsDamdfeNotificationsContacts(string emails)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfeNotificationsContacts> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfeNotificationsContacts>();

            List<string> emailLista = emails.Split(';').ToList();
            foreach (string email in emailLista)
            {
                if (!string.IsNullOrEmpty(email))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfeNotificationsContacts contato = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptionsDamdfeNotificationsContacts();
                    contato.type = "email";
                    contato.email = email.Trim();
                    retorno.Add(contato);
                }
            }

            return retorno;
        }

        private string obterEmailParaEnvioDADFE(Dominio.Entidades.Empresa empresa)
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

        private string CNPJResponsavelSeguro(string CNPJResponsavel, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            // Recebe valor setado da aplicacao e verifica se e nullo
            // Se for, verifica se a configuracao e para usar o CNPJ da transportadora como CNPJ do seguro
            if (string.IsNullOrWhiteSpace(CNPJResponsavel) && this.UsarCNPJTransportadorComoCNPJSeguradora(mdfe))
            {
                return mdfe.Empresa.CNPJ;
            }

            return CNPJResponsavel;
        }

        private string NumeroAverbacaoSeguro(string NumeroAverbacao, string NumeroApolice, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            // Recebe valor setado da aplicacao e verifica se e nullo
            // Se for, verifica se a configuracao e para usar o numero da averbacao como numero da apolice
            if (string.IsNullOrWhiteSpace(NumeroAverbacao) && this.UsarNumeroApoliceComoNumeroAverbacao(mdfe))
            {
                return NumeroApolice;
            }

            return NumeroAverbacao;
        }

        private bool UsarCNPJTransportadorComoCNPJSeguradora(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            // Define retorno padrao
            Dominio.Enumeradores.OpcaoSimNao? config = Dominio.Enumeradores.OpcaoSimNao.Sim;

            // Busca da configuracao local
            if (mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.CNPJTransportadorComoCNPJSeguradora != null)
                config = mdfe.Empresa.Configuracao.CNPJTransportadorComoCNPJSeguradora;
            // Busca da configuracao pai
            else if (mdfe.Empresa.EmpresaPai != null && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora != null)
                config = mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora;

            // Compara
            return config == Dominio.Enumeradores.OpcaoSimNao.Sim;
        }

        private bool UsarNumeroApoliceComoNumeroAverbacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            // Define retorno padrao
            Dominio.Enumeradores.OpcaoSimNao? config = Dominio.Enumeradores.OpcaoSimNao.Sim;

            // Busca da configuracao local
            if (mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.NumeroApoliceComoNumeroAverbacao != null)
                config = mdfe.Empresa.Configuracao.NumeroApoliceComoNumeroAverbacao;
            // Busca da configuracao pai
            else if (mdfe.Empresa.EmpresaPai != null && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao != null)
                config = mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao;

            // Compara
            return config == Dominio.Enumeradores.OpcaoSimNao.Sim;
        }

        private string ObterEmailsClientes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTEs = repDocMDFe.BuscarCTesPorMDFe(mdfe.Codigo);

            string emails = string.Empty;

            for (var i = 0; i < listaCTEs.Count(); i++)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(listaCTEs[i].Codigo);

                if (cte.OutrosTomador != null)
                {
                    if (cte.OutrosTomador.EmailStatus && !string.IsNullOrWhiteSpace(cte.OutrosTomador.Email))
                        emails += string.Concat(cte.OutrosTomador.Email, ";");

                    if (cte.OutrosTomador.EmailContadorStatus && !string.IsNullOrWhiteSpace(cte.OutrosTomador.EmailContador))
                        emails += string.Concat(cte.OutrosTomador.EmailContador, ";");

                    if (cte.OutrosTomador.EmailContatoStatus && !string.IsNullOrWhiteSpace(cte.OutrosTomador.EmailContato))
                        emails += string.Concat(cte.OutrosTomador.EmailContato, ";");

                    if (cte.OutrosTomador.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(cte.OutrosTomador.EmailTransportador))
                        emails += string.Concat(cte.OutrosTomador.EmailTransportador, ";");
                }
                if (cte.Remetente != null)
                {
                    if (cte.Remetente.EmailStatus && !string.IsNullOrWhiteSpace(cte.Remetente.Email))
                        emails += string.Concat(cte.Remetente.Email, ";");

                    if (cte.Remetente.EmailContadorStatus && !string.IsNullOrWhiteSpace(cte.Remetente.EmailContador))
                        emails += string.Concat(cte.Remetente.EmailContador, ";");

                    if (cte.Remetente.EmailContatoStatus && !string.IsNullOrWhiteSpace(cte.Remetente.EmailContato))
                        emails += string.Concat(cte.Remetente.EmailContato, ";");

                    if (cte.Remetente.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(cte.Remetente.EmailTransportador))
                        emails += string.Concat(cte.Remetente.EmailTransportador, ";");
                }
                if (cte.Expedidor != null)
                {
                    if (cte.Expedidor.EmailStatus && !string.IsNullOrWhiteSpace(cte.Expedidor.Email))
                        emails += string.Concat(cte.Expedidor.Email, ";");

                    if (cte.Expedidor.EmailContadorStatus && !string.IsNullOrWhiteSpace(cte.Expedidor.EmailContador))
                        emails += string.Concat(cte.Expedidor.EmailContador, ";");

                    if (cte.Expedidor.EmailContatoStatus && !string.IsNullOrWhiteSpace(cte.Expedidor.EmailContato))
                        emails += string.Concat(cte.Expedidor.EmailContato, ";");

                    if (cte.Expedidor.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(cte.Expedidor.EmailTransportador))
                        emails += string.Concat(cte.Expedidor.EmailTransportador, ";");
                }
                if (cte.Destinatario != null)
                {
                    if (cte.Destinatario.EmailStatus && !string.IsNullOrWhiteSpace(cte.Destinatario.Email))
                        emails += string.Concat(cte.Destinatario.Email, ";");

                    if (cte.Destinatario.EmailContadorStatus && !string.IsNullOrWhiteSpace(cte.Destinatario.EmailContador))
                        emails += string.Concat(cte.Destinatario.EmailContador, ";");

                    if (cte.Destinatario.EmailContatoStatus && !string.IsNullOrWhiteSpace(cte.Destinatario.EmailContato))
                        emails += string.Concat(cte.Destinatario.EmailContato, ";");

                    if (cte.Destinatario.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(cte.Destinatario.EmailTransportador))
                        emails += string.Concat(cte.Destinatario.EmailTransportador, ";");
                }
                if (cte.Recebedor != null)
                {
                    if (cte.Recebedor.EmailStatus && !string.IsNullOrWhiteSpace(cte.Recebedor.Email))
                        emails += string.Concat(cte.Recebedor.Email, ";");

                    if (cte.Recebedor.EmailContadorStatus && !string.IsNullOrWhiteSpace(cte.Recebedor.EmailContador))
                        emails += string.Concat(cte.Recebedor.EmailContador, ";");

                    if (cte.Recebedor.EmailContatoStatus && !string.IsNullOrWhiteSpace(cte.Recebedor.EmailContato))
                        emails += string.Concat(cte.Recebedor.EmailContato, ";");

                    if (cte.Recebedor.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(cte.Recebedor.EmailTransportador))
                        emails += string.Concat(cte.Recebedor.EmailTransportador, ";");
                }
            }

            return Utilidades.String.Left(emails, 1000);
        }

        #endregion

        #region Conversão de Enums

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerTypeMDFe obterIssuerTypeMDFe(Dominio.Enumeradores.TipoEmitenteMDFe tipoEmitente)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerTypeMDFe retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerTypeMDFe();

            switch (tipoEmitente)
            {
                case TipoEmitenteMDFe.PrestadorDeServicoDeTransporte:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerTypeMDFe.service_provider;
                    break;

                case TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerTypeMDFe.own_cargo;
                    break;

                case TipoEmitenteMDFe.TransporteCTeGlobalizado:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerTypeMDFe.globalized_cte;
                    break;

                case TipoEmitenteMDFe.PrestadorDeServicoDeTransporteApenasChaveCTe:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerTypeMDFe.service_provider;
                    break;
            }

            return retorno;
        }


        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueTypeMDFe obterIssueTypeMDFe(Dominio.Enumeradores.TipoEmissaoMDFe tipoEmissao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueTypeMDFe retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueTypeMDFe();

            switch (tipoEmissao)
            {
                case Dominio.Enumeradores.TipoEmissaoMDFe.Normal:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueTypeMDFe.default_;
                    break;

                case Dominio.Enumeradores.TipoEmissaoMDFe.Contingencia:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueTypeMDFe.offline_contingency;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTransporterType? obterTransporterType(Dominio.Entidades.VeiculoMDFe veiculo)
        {
            if (string.IsNullOrWhiteSpace(veiculo?.CPFCNPJProprietario))
                return null;

            var documentoNumerico = veiculo.CPFCNPJProprietario.ObterSomenteNumeros();

            if (documentoNumerico.Length == 14)
                return Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTransporterType.ETC;

            if (documentoNumerico.Length == 11)
                return Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTransporterType.TAC;

            return null;
        }


        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModalMDFe obterModalMDFe(Dominio.Entidades.ModalTransporte modal)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModalMDFe retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModalMDFe();

            // RODOVIARIO  01
            // AQUAVIARIO  03
            // FERROVIARIO 04
            // MULTIMODAL  06

            switch (modal?.Numero ?? "01")
            {
                case "01":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModalMDFe.road;
                    break;
                case "03":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModalMDFe.water;
                    break;
                case "04":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModalMDFe.rail;
                    break;
                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumModalMDFe.road;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType obterLoadType(Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType();

            if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.GranelSolido)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.solid_bulk;
            else if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.GranelLiquido)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.liquid_bulk;
            else if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.Frigorificada)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.refrigerated;
            else if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.Conteinerizada)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.containerized;
            else if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.CargaGeral)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.general_cargo;
            else if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.Neogranel)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.neobulk;
            else if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.PerigosaGranelSolido)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.dangerous_solid_bulk;
            else if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.PerigosaGranelLiquido)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.dangerous_liquid_bulk;
            else if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.PerigosaFrigorificada)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.dangerous_refrigerated_cargo;
            else if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.PerigosaConteinerizada)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.dangerous_containerized;
            else if (tipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.PerigosaCargaGeral)
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.dangerous_general_cargo;
            else
                retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumLoadType.general_cargo;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumWheelType obterWheelType(string tipoRodado)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumWheelType retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumWheelType();

            /// 01 - Truck;
            /// 02 - Toco;
            /// 03 - Cavalo Mecânico;
            /// 04 - VAN;
            /// 05 - Utilitário;
            /// 06 - Outros
            switch (tipoRodado)
            {
                case "01":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumWheelType.truck;
                    break;

                case "02":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumWheelType.semi_truck;
                    break;

                case "03":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumWheelType.tractor_unit;
                    break;

                case "04":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumWheelType.van;
                    break;

                case "05":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumWheelType.utility_vehicle;
                    break;

                case "06":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumWheelType.other;
                    break;

                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumWheelType.other;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBodyType obterBodyType(string tipoCarroceria)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBodyType retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBodyType();

            /// 00 - não aplicável;
            /// 01 - Aberta;
            /// 02 - Fechada/Baú;
            /// 03 - Granelera;
            /// 04 - Porta Container;
            /// 05 - Sider
            switch (tipoCarroceria)
            {
                case "00":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBodyType.not_applicable;
                    break;

                case "01":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBodyType.open;
                    break;

                case "02":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBodyType.closed_box;
                    break;

                case "03":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBodyType.bulk_carrier;
                    break;

                case "04":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBodyType.container_chassis;
                    break;

                case "05":
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBodyType.sider;
                    break;

                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumBodyType.not_applicable;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory obterVehicleCategory(int quantidadeEixos)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory();

            switch (quantidadeEixos)
            {
                case 2:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory.two_axle;
                    break;

                case 3:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory.three_axle;
                    break;

                case 4:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory.four_axle;
                    break;

                case 5:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory.five_axle;
                    break;

                case 6:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory.six_axle;
                    break;

                case 7:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory.seven_axle;
                    break;

                case 8:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory.eight_axle;
                    break;

                case 9:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory.nine_axle;
                    break;

                case 10:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory.ten_axle;
                    break;

                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumVehicleCategory.over_ten_axle;
                    break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTollVoucherType obterTollVoucherType(Dominio.Enumeradores.TipoCompraValePedagio tipoCompra)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTollVoucherType retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTollVoucherType();

            switch (tipoCompra)
            {
                case TipoCompraValePedagio.Tag:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTollVoucherType.tag;
                    break;

                case TipoCompraValePedagio.Cupom:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTollVoucherType.voucher;
                    break;

                case TipoCompraValePedagio.Cartao:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTollVoucherType.card;
                    break;

                default:
                    retorno = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTollVoucherType.tag;
                    break;
            }

            return retorno;
        }

        #endregion Conversão de Enums

    }
}

