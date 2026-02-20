using CoreWCF;
using Dominio.ObjetosDeValor.WebService.MDFe;
using System.Text;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class MDFe(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IMDFe
    {
        #region Métodos Globais

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> BuscarMDFes(int? protocoloIntegracaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>>.CreateFrom(
                    new Servicos.WebService.MDFe.MDFe(
                        unitOfWork,
                        TipoServicoMultisoftware,
                        Cliente,
                        Auditado
                    ).BuscarMDFes(
                        new RequestMDFe()
                        {
                            Inicio = inicio ?? 0,
                            Limite = limite ?? 0,
                            ProtocoloIntegracaoCarga = protocoloIntegracaoCarga ?? 0,
                            TipoDocumentoRetorno = tipoDocumentoRetorno ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum
                        }
                    ));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> BuscarMDFesPorCarga(int? protocoloIntegracaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            protocoloIntegracaoCarga ??= 0;
            inicio ??= 0;
            limite ??= 0;
            tipoDocumentoRetorno ??= Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum;

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (limite <= 50)
                {
                    if (protocoloIntegracaoCarga > 0)
                    {
                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

                        if (integradora.Empresa != null)
                            carga = repCarga.BuscarPorCodigo((int)protocoloIntegracaoCarga);
                        else
                            carga = repCarga.BuscarPorProtocolo((int)protocoloIntegracaoCarga);

                        Servicos.WebService.MDFe.MDFe serWSMDFe = new Servicos.WebService.MDFe.MDFe(unitOfWork);

                        string mensagem = "";

                        retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>();
                        retorno.Objeto.Itens = serWSMDFe.BuscarMDFePorCarga(
                            carga,
                            (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno)tipoDocumentoRetorno,
                            TipoServicoMultisoftware,
                            (int)inicio,
                            (int)limite,
                            ref mensagem
                        );
                        retorno.Objeto.NumeroTotalDeRegistro = retorno.Objeto.Itens.Count;
                        retorno.Status = true;

                        if (!string.IsNullOrWhiteSpace(mensagem))
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = mensagem;
                        }
                        else
                            Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou MDF-es", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Por favor, informe o código de integração da carga.";
                    }
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 50";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os MDFEs";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> BuscarMDFesPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;
            tipoDocumentoRetorno ??= Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum;

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 50)
                {
                    DateTime _dataInicial = dataInicial.ToDateTime();
                    DateTime _dataFinal = dataFinal.ToDateTime();

                    if (_dataInicial != DateTime.MinValue && _dataFinal != DateTime.MinValue)
                    {
                        Servicos.WebService.MDFe.MDFe serWSMDFe = new Servicos.WebService.MDFe.MDFe(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                        string mensagem = "";

                        retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>
                        {
                            Itens = serWSMDFe.BuscarMDFesPeriodo(
                                _dataInicial,
                                _dataFinal,
                                integradora.Empresa?.Codigo ?? 0,
                                (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno)tipoDocumentoRetorno,
                                (int)inicio,
                                (int)limite,
                                ref mensagem
                            ),
                            NumeroTotalDeRegistro = serWSMDFe.ContarMDFesPeriodo(_dataInicial, _dataFinal, integradora.Empresa?.Codigo ?? 0)
                        };
                        retorno.Status = true;

                        if (!string.IsNullOrWhiteSpace(mensagem))
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = mensagem;
                        }
                        else
                            Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou CT-es", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "É obrigatório informar a data inicial e a data final.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 50";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os CTes";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> BuscarMDFePorProtocolo(int? protocoloMDFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno)
        {
            ValidarToken();

            protocoloMDFe ??= 0;
            tipoDocumentoRetorno ??= Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum;

            Retorno<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (protocoloMDFe > 0)
                {
                    Servicos.WebService.MDFe.MDFe serWSMDFe = new Servicos.WebService.MDFe.MDFe(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorMDFe((int)protocoloMDFe);

                    if (cargaMDFe != null)
                    {
                        retorno.Objeto = serWSMDFe.ConverterObjetoCargaMDFe(cargaMDFe, (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno)tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF);
                        retorno.Status = true;

                        Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou MDF-e por protocolo", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O protocolo informado não é de um MDF-e existente.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Por favor, informe o protocolo do MDF-e.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consulta o MDF-e";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> SolicitarEncerramentoMDFePorChaveCTe(string chaveCTe)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Servicos.Log.TratarErro($"SolicitarEncerramentoMDFePorChaveCTe: chaveCTe = {chaveCTe} (IP Origem {Auditado?.IP}", "SolicitarEncerramentoMDFePorChaveCTe");

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                if (!string.IsNullOrWhiteSpace(chaveCTe))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorChaveCTe(Utilidades.String.RemoveAllSpecialCharacters(chaveCTe));
                    if (cargaCte != null)
                    {
                        List<int> codigosMDFes = repDocumentoMunicipioDescarregamentoMDFe.BuscarCodigoDeMDFesPorCTe(cargaCte.CTe.Codigo);
                        if (codigosMDFes.Count > 0)
                        {
                            foreach (int codigoMDF in codigosMDFes)
                            {
                                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDF);
                                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorMDFe(mdfe.Codigo);
                                    Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe dadosEncerramento = serCargaMDFe.ObterDadosEncerramento(cargaMDFe.Codigo, unitOfWork);

                                    string retornoEncerramento = serCargaMDFe.EncerrarMDFe(dadosEncerramento.Codigo, cargaMDFe.Carga.Codigo, dadosEncerramento.Localidades[0].Codigo, dadosEncerramento.DataEncerramento, WebServiceConsultaCTe, null, TipoServicoMultisoftware, unitOfWork, Auditado);

                                    mdfe.Log = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - Solicitado encerramento via Integração pelo Integrador " + integradora.Descricao;
                                    if (Auditado != null && !string.IsNullOrWhiteSpace(Auditado.IP))
                                        mdfe.Log = mdfe.Log + " (IP Origem " + Auditado.IP + ")";

                                    repMDFe.Atualizar(mdfe);

                                    //serCargaMDFe.AdicionarMDFeNaFilaDeConsulta(mdfe, WebServiceConsultaCTe);
                                    if (string.IsNullOrWhiteSpace(retornoEncerramento))
                                    {
                                        retorno.Objeto = true;
                                        retorno.Status = true;

                                        Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, "Solicitou o encerramento do MDF-e.", unitOfWork);
                                    }
                                    else
                                    {
                                        retorno.Mensagem = retornoEncerramento;
                                        retorno.Objeto = false;
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    }
                                }
                                else
                                {
                                    if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                                    {
                                        retorno.Mensagem = "Já foi solicitado o encerramento deste MDF-e";
                                        retorno.Objeto = false;
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                    }
                                    else
                                    {
                                        retorno.Mensagem = "Não é possível solicitar o encerramento do MDF-e em sua atual situação (" + mdfe.DescricaoStatus + ")";
                                        retorno.Objeto = false;
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    }
                                }
                            }
                        }
                        else
                        {
                            retorno.Mensagem = "Não foi localizado nenhum MDF-e emitido para essa chave de CT-e (" + chaveCTe + ")";
                            retorno.Objeto = false;
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                    }
                    else
                    {
                        //Quando não localiza uma Carga CTe pesquisa diretamente nos CTes.
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(chaveCTe);

                        if (cte != null)
                        {
                            List<int> codigosMDFes = repDocumentoMunicipioDescarregamentoMDFe.BuscarCodigoDeMDFesPorCTe(cte.Codigo);
                            if (codigosMDFes != null && codigosMDFes.Count > 0)
                            {
                                foreach (int codigoMDF in codigosMDFes)
                                {
                                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDF);
                                    if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                                    {
                                        string retornoEncerramento = string.Empty;

                                        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                                        DateTime dataEvento = DateTime.Now;
                                        dataEvento = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);

                                        Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
                                        if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, dataEvento, unitOfWork, dataEvento))
                                            retornoEncerramento = "Não foi possível solicitar encerramento.";
                                        else
                                            svcMDFe.AdicionarMDFeNaFilaDeConsulta(mdfe, unitOfWork);

                                        string mensagemLog = "Solicitado encerramento via Integração pelo Integrador " + integradora.Descricao + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                        mdfe.Log = !string.IsNullOrWhiteSpace(mdfe.Log) ? String.Concat(mdfe.Log, " / ", mensagemLog) : mensagemLog;
                                        repMDFe.Atualizar(mdfe);

                                        svcMDFe.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, DateTime.Now, mdfe.Empresa, mdfe.Empresa.Localidade, mensagemLog, unitOfWork);

                                        if (string.IsNullOrWhiteSpace(retornoEncerramento))
                                        {
                                            retorno.Objeto = true;
                                            retorno.Status = true;

                                            Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, "Solicitou encerramento do MDF-es", unitOfWork);
                                        }
                                        else
                                        {
                                            retorno.Mensagem = retornoEncerramento;
                                            retorno.Objeto = false;
                                            retorno.Status = false;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;

                                        }
                                    }
                                    else
                                    {
                                        if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                                        {
                                            retorno.Mensagem = "Já foi solicitado o encerramento deste MDF-e";
                                            retorno.Objeto = false;
                                            retorno.Status = false;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                        }
                                        else
                                        {
                                            retorno.Mensagem = "Não é possível solicitar o encerramento do MDF-e em sua atual situação (" + mdfe.DescricaoStatus + ")";
                                            retorno.Objeto = false;
                                            retorno.Status = false;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                retorno.Mensagem = "Não foi localizado nenhum MDF-e emitido para essa chave de CT-e (" + chaveCTe + ")";
                                retorno.Objeto = false;
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            }
                        }
                        else
                        {
                            retorno.Mensagem = "Não foi localizado um CT-e válido para a chave informada (" + chaveCTe + ")";
                            retorno.Objeto = false;
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                    }
                }
                else
                {
                    retorno.Mensagem = "É obrigatório informar a chave do CT-e";
                    retorno.Objeto = false;
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao encerrar o MDF-e";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.Enumeradores.StatusMDFe> ConsultaStatusMDFePorChaveCTe(string chaveCTe)
        {
            ValidarToken();

            Retorno<Dominio.Enumeradores.StatusMDFe> retorno = new Retorno<Dominio.Enumeradores.StatusMDFe>();
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                if (!string.IsNullOrWhiteSpace(chaveCTe))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorChaveCTe(Utilidades.String.RemoveAllSpecialCharacters(chaveCTe));
                    if (cargaCte != null)
                    {
                        List<int> codigosMDFes = repDocumentoMunicipioDescarregamentoMDFe.BuscarCodigoDeMDFesPorCTe(cargaCte.CTe.Codigo);
                        if (codigosMDFes.Count > 0)
                        {
                            foreach (int codigoMDF in codigosMDFes)
                            {
                                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDF);
                                retorno.Mensagem = "";
                                retorno.Objeto = mdfe.Status;
                                retorno.Status = true;

                                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Consultou status do MDF-e por chave do CT-e", unitOfWork);
                            }
                        }
                        else
                        {
                            retorno.Mensagem = "Não foi localizado nenhum MDF-e emitido para essa chave de CT-e (" + cargaCte + ")";
                            retorno.Objeto = Dominio.Enumeradores.StatusMDFe.Autorizado;
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                    }
                    else
                    {
                        retorno.Mensagem = "Não foi localizado um CT-e válido para a chave informada (" + cargaCte + ")";
                        retorno.Objeto = Dominio.Enumeradores.StatusMDFe.Autorizado;
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    }
                }
                else
                {
                    retorno.Mensagem = "É obrigatório informar a chave do CT-e";
                    retorno.Objeto = Dominio.Enumeradores.StatusMDFe.Autorizado;
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao encerrar o MDF-e";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.MDFe.ProtocoloMDFeManual> SolicitarEmissaoMDFe(Dominio.ObjetosDeValor.WebService.MDFe.MDFeManual mdfeManual)
        {
            Servicos.Log.TratarErro("SolicitarEmissaoMDFe: " + (mdfeManual != null ? Newtonsoft.Json.JsonConvert.SerializeObject(mdfeManual) : string.Empty), "Request");

            Retorno<Dominio.ObjetosDeValor.WebService.MDFe.ProtocoloMDFeManual> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.MDFe.ProtocoloMDFeManual>();
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.WebService.MDFe.MDFe serMDFe = new Servicos.WebService.MDFe.MDFe(unitOfWork);
            string erro = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = serMDFe.GerarMDFeManual(mdfeManual, TipoServicoMultisoftware, WebServiceConsultaCTe, ref erro, unitOfWork);
                if (cargaMDFeManual != null)
                {
                    retorno.Mensagem = "";
                    retorno.Status = true;
                    retorno.Objeto = new Dominio.ObjetosDeValor.WebService.MDFe.ProtocoloMDFeManual() { protocoloMDFeManual = cargaMDFeManual.Codigo };

                    if (cargaMDFeManual.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmDigitacao || cargaMDFeManual.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Rejeicao)
                        retorno.Mensagem = "MDFe salvo porém não foi possível emitir, acesse o portal para verificação.";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManual, "Solicitouemissão do MDF-e manual", unitOfWork);

                    Servicos.Log.TratarErro("SolicitarEmissaoMDFe: Protocolo " + cargaMDFeManual.Codigo.ToString());
                }
                else
                {
                    Servicos.Log.TratarErro(erro);
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = !string.IsNullOrWhiteSpace(erro) ? erro : "Não foi possível gerar MDFe.";
                    retorno.Objeto = null;

                    unitOfWork.Rollback();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Falha genérica ao solicitar emissão de MDFe. " + erro;
                retorno.Objeto = null;

                unitOfWork.Rollback();
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> BuscarMDFesPorMDFeManual(int? protocoloIntegracaoMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            ValidarToken();

            protocoloIntegracaoMDFeManual ??= 0;
            inicio ??= 0;
            limite ??= 0;
            tipoDocumentoRetorno ??= Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum;

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 50)
                {
                    if (protocoloIntegracaoMDFeManual > 0)
                    {
                        Servicos.WebService.MDFe.MDFe serWSMDFe = new Servicos.WebService.MDFe.MDFe(unitOfWork);

                        string mensagem = "";

                        retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>();
                        retorno.Objeto.Itens = serWSMDFe.BuscarMDFePorMDFeManual(
                            (int)protocoloIntegracaoMDFeManual,
                            (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno)tipoDocumentoRetorno,
                            (int)inicio,
                            (int)limite,
                            ref mensagem
                        );
                        retorno.Objeto.NumeroTotalDeRegistro = serWSMDFe.ContarMDFePorMDFeManual((int)protocoloIntegracaoMDFeManual);
                        retorno.Status = true;

                        if (!string.IsNullOrWhiteSpace(mensagem))
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = mensagem;
                        }
                        else
                            Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou MDF-es", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Por favor, informe o código de integração do MDFe.";
                    }
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 50";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os MDFEs";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<string> EnviarArquivoXMLMDFe(Stream arquivo)
        {
            ValidarToken();
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
                Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
                Retorno<string> retorno = new Retorno<string>();

                string nomeArquivo = Guid.NewGuid().ToString();

                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, string.Concat(nomeArquivo, ".xml"));

                using (StreamReader reader = new StreamReader(arquivo))
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminho, RemoveTroublesomeCharacters(reader.ReadToEnd()), Encoding.UTF8);

                arquivo.Close();
                arquivo.Dispose();

                object retornoMDFe = null;

                using (System.IO.Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(caminho))
                    retornoMDFe = svcMDFe.GerarMDFeAnteriorAsync(stream, 0, unitOfWork);

                if (retornoMDFe != null)
                {
                    if (retornoMDFe.GetType() == typeof(string))
                    {
                        retorno.Status = false;
                        retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm:ss");
                        retorno.Objeto = (string)retornoMDFe;
                    }
                    else if (retornoMDFe.GetType() == typeof(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais))
                    {
                        retorno.Status = true;
                        retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm:ss");
                        retorno.Objeto = "MDF-e importado com sucesso";
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm:ss");
                        retorno.Objeto = "MDFe inválido.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm:ss");
                    retorno.Objeto = "MDFe inválido.";
                }

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao salvar o arquivo.", Status = false, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica };
            }
        }

        public Retorno<bool> EnviarMDFeAquaviario(Dominio.ObjetosDeValor.WebService.MDFe.MDFeAquaviario mdfeAquaviario)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.MDFe.MDFeAquaviario(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarMDFeAquaviario(mdfeAquaviario));
            });
        }

        public Retorno<bool> AtualizarSituacaoMDFeAquaviario(string chaveMDFe, Dominio.Enumeradores.StatusMDFe statusMDFe, string protocolo, DateTime dataEvento, string mensagemRetornoSefaz, string motivo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.MDFe.MDFeAquaviario(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AtualizarSituacaoMDFeAquaviario(chaveMDFe, statusMDFe, protocolo, dataEvento, mensagemRetornoSefaz, motivo));
            });
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMDFe;
        }

        private static string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                if ((ch < 0x00FD && ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();

        }

        #endregion
    }
}
