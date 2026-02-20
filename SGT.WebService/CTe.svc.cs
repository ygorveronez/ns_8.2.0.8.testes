using CoreWCF;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.CTe;
using Servicos.Embarcador.Integracao;
using System.Text;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CTe" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CTe.svc or CTe.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]

    public class CTe(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), ICTe
    {
        #region Métodos Globais

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao>> BuscarDadosAverbacao(string chaveCTe)
        {
            ValidarToken();

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao>>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.WebService.Carga.DadosAverbacao serWSDadosAverbacao = new Servicos.WebService.Carga.DadosAverbacao(unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            try
            {
                if (!string.IsNullOrWhiteSpace(chaveCTe))
                {
                    retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao>();
                    List<Dominio.Entidades.AverbacaoCTe> averbacoes = repAverbacaoCTe.BuscarPorCTe(chaveCTe);

                    foreach (var averbacao in averbacoes)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao dadosAverbacao = serWSDadosAverbacao.ConverterDadosAverbacaoCTe(averbacao, enviarCTeApenasParaTomador, unitOfWork);
                        retorno.Objeto.Add(dadosAverbacao);
                    }

                    retorno.Status = true;
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "Chave do CT-e não informada";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Falha ao consultar os dados de averbação";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> EnviarDadosAverbacao(Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao dadosAverbacao, string numeroCarga, string numeroBooking, string numeroOS)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Servicos.WebService.Carga.DadosAverbacao serWSDadosAverbacao = new Servicos.WebService.Carga.DadosAverbacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            //Servicos.Log.TratarErro("EnviarDadosAverbacao Carga: " + numeroCarga + " BK " + numeroBooking + " OS " + numeroOS);

            retorno.Status = true;
            try
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorNumeroCargaSituacao(numeroCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, true, false);
                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroCargaSituacao(numeroCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova, true, false);

                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroOSSituacao(numeroOS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, true, false);
                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroOSSituacao(numeroOS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova, true, false);

                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroOSMaeSituacao(numeroOS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, true, false);
                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroOSMaeSituacao(numeroOS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova, true, false);

                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroBookingSituacao(numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, true, false);
                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroBookingSituacao(numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova, true, false);

                if (dadosAverbacao != null)
                {
                    if (cargasPedido != null && cargasPedido.Count > 0)
                    {
                        try
                        {
                            unitOfWork.Start();
                            foreach (var cargaPedido in cargasPedido)
                            {
                                if (serWSDadosAverbacao.SalvarDadosAverbacaoCarga(dadosAverbacao, cargaPedido, unitOfWork))
                                {
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, $"Inseriu a averbação {dadosAverbacao.Protocolo} pelo serviço de integração recebida.", unitOfWork);
                                    retorno.Status = true;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                                    retorno.Mensagem = "Dados da averbação inserido na carga.";
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "Favor realize o cadastro da apólice antes da integração.";
                                    break;
                                }
                            }
                            unitOfWork.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Não foi possível inserir CT-e a carga " + ex.Message;
                            unitOfWork.Rollback();
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Não foi localizada nenhuma carga com o número/booking/OS.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados da averbação não informado";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar vincular os dados de averbação a alguma carga.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> EnviarCTeAnterior(Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, string numeroCarga, string numeroBooking, string numeroOS)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            //Servicos.Log.TratarErro("EnviarCTeAnterior Carga: " + numeroCarga + " BK " + numeroBooking + " OS " + numeroOS);

            retorno.Status = true;
            try
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorNumeroCargaSituacao(numeroCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, true, false);
                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroCargaSituacao(numeroCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova, true, false);

                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroOSSituacao(numeroOS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, true, false);
                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroOSSituacao(numeroOS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova, true, false);

                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroOSMaeSituacao(numeroOS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, true, false);
                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroOSMaeSituacao(numeroOS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova, true, false);

                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroBookingSituacao(numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, true, false);
                if (cargasPedido == null || cargasPedido.Count == 0)
                    cargasPedido = repCargaPedido.BuscarPorNumeroBookingSituacao(numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova, true, false);

                if (cte != null)
                {
                    if (cargasPedido != null && cargasPedido.Count > 0)
                    {
                        try
                        {
                            unitOfWork.Start();
                            foreach (var cargaPedido in cargasPedido)
                            {
                                serCargaCteParaSubContratacao.VincularCTeTerceiroACargaPedido(cte, cargaPedido, unitOfWork, TipoServicoMultisoftware);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, $"Inseriu CT-e anterior {cte.Chave} pelo serviço de integração recebida.", unitOfWork);
                            }
                            unitOfWork.CommitChanges();

                            retorno.Status = true;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                            retorno.Mensagem = "CT-e anterior inserido na carga.";
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Não foi possível inserir CT-e a carga " + ex.Message;
                            unitOfWork.Rollback();
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Não foi localizada nenhuma carga com o número/booking/OS.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "CT-e anterior não informado";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar vincular o ct-e anterior a alguma carga.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> BuscarCTePorChave(string chaveCTe)
        {
            ValidarToken();

            Retorno<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (!string.IsNullOrWhiteSpace(chaveCTe))
                {
                    Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(chaveCTe);

                    bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

                    if (cte != null)
                    {
                        List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTe(cte.Codigo);
                        retorno.Objeto = serCTe.ConverterEntidadeCTeParaObjeto(cte, enviarCTeApenasParaTomador, unitOfWork);
                        retorno.Status = true;
                        Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou CT-e pela Chave", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "CT-e não localizado pela chave. ";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "É obrigatório informar a chave do CT-e para a consulta. ";
                }
            }
            catch (Exception ex)
            {

                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar o CT-e pela chave";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao>> BuscarAverbacaoCTes(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            ValidarToken();

            tipoDocumentoRetorno ??= TipoDocumentoRetorno.Nenhum;
            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 50)
                {
                    if (protocolo != null)
                    {
                        if (protocolo.protocoloIntegracaoCarga > 0 || protocolo.protocoloIntegracaoPedido > 0)
                        {
                            Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
                            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                            Dominio.Entidades.Cliente remetente = null;
                            Dominio.Entidades.Cliente destinatario = null;
                            if (protocolo.Remetente != null)
                            {
                                double cnpj = 0;
                                double.TryParse(Utilidades.String.OnlyNumbers(protocolo.Remetente.CPFCNPJ), out cnpj);
                                remetente = repCliente.BuscarPorCPFCNPJ(cnpj);
                                if (remetente == null)
                                {
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "Não foi localizado um remetente cadastradado na base Multisoftware para o CNPJ " + protocolo.Remetente.CPFCNPJ + ". ";
                                }
                            }

                            if (retorno.CodigoMensagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos)
                            {
                                if (protocolo.Destinatario != null)
                                {
                                    double cnpj = 0;
                                    double.TryParse(Utilidades.String.OnlyNumbers(protocolo.Destinatario.CPFCNPJ), out cnpj);
                                    destinatario = repCliente.BuscarPorCPFCNPJ(cnpj);
                                    if (destinatario == null)
                                    {
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "Não foi localizado um destinatário cadastradado na base Multisoftware para o CNPJ " + protocolo.Remetente.CPFCNPJ + ". ";
                                    }
                                }
                            }

                            if (retorno.CodigoMensagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos)
                            {
                                string mensagem = "";
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedidoAutorizados(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                                if (cargaPedido != null && cargaPedido.Count > 0)
                                {
                                    if (cargaPedido.Any(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos))
                                    {
                                        if (remetente != null)
                                        {
                                            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);

                                            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumento = repCargaOcorrenciaDocumento.BuscarOcorrenciasPendentesCargaRemetenteDestinatario(protocolo.protocoloIntegracaoCarga, remetente.CPF_CNPJ, 0);
                                            if (cargaOcorrenciaDocumento.Count > 0)
                                            {
                                                if (cargaOcorrenciaDocumento.FirstOrDefault().CargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada)
                                                {
                                                    retorno.Status = false;
                                                    retorno.CodigoMensagem = 302;
                                                    retorno.Mensagem = "A Ocorrência foi rejeitada para o CT-e do remetente " + remetente.Nome + " na Carga.";
                                                }
                                                else if (cargaOcorrenciaDocumento.Any(obj => obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada))
                                                {
                                                    retorno.CodigoMensagem = 301;
                                                    retorno.Status = false;
                                                    retorno.Mensagem = "Existem ocorrências não aprovadas para o CT-e do remetente " + remetente.Nome + " na Carga.";
                                                }
                                            }
                                        }

                                        if (retorno.CodigoMensagem != 302 && retorno.CodigoMensagem != 301)
                                        {
                                            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao>();
                                            retorno.Objeto.Itens = serWSCTe.BuscarAverbacaoCTes(
                                                cargaPedido,
                                                (TipoDocumentoRetorno)tipoDocumentoRetorno,
                                                remetente?.CPF_CNPJ ?? 0,
                                                destinatario?.CPF_CNPJ ?? 0,
                                                (int)inicio,
                                                (int)limite,
                                                ref mensagem
                                            );
                                            retorno.Objeto.NumeroTotalDeRegistro = serWSCTe.ContarAverbacaoCTes(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, remetente?.CPF_CNPJ ?? 0, destinatario?.CPF_CNPJ ?? 0);
                                            retorno.Status = true;

                                            if (!string.IsNullOrWhiteSpace(mensagem))
                                            {
                                                retorno.Status = false;
                                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                retorno.Mensagem = mensagem;
                                            }
                                            else
                                                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Averbações dos CT-es", unitOfWork);
                                        }
                                    }
                                    else
                                    {
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "Os documentos do protocolo informado ainda estão em sendo emitidos.";
                                    }
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "Os protocolos informados não existem na base Multisoftware.";
                                }
                            }
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Por favor, informe os códigos de integração.";
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "É obrigatório informar o protocolo de integração. ";
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
                if (protocolo != null)
                    Servicos.Log.TratarErro("Carga " + protocolo.protocoloIntegracaoCarga + " Pedido " + protocolo.protocoloIntegracaoPedido);

                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as averbações dos CTes";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTes(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> retorno = new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCTes(protocolo, tipoDocumentoRetorno ?? TipoDocumentoRetorno.Nenhum, inicio ?? 0, limite ?? 0);

                var ret = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>.CreateFrom(retorno.Objeto)
                };

                string path = Utilidades.IO.FileStorageService.Storage.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogObject");
                if (System.IO.Directory.Exists(path))
                {
                    try
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(ret);
                        Servicos.Log.TratarErro($"{DateTime.Now.ToString()} Obj retorno >> {json}", "OBJs");
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                    }
                }
                return ret;
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesTk(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, string token, int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            tipoDocumentoRetorno ??= TipoDocumentoRetorno.Nenhum;
            inicio ??= 0;
            limite ??= 0;

            //ValidarToken();
            Repositorio.WebService.Integradora repIntegracadora = new Repositorio.WebService.Integradora(unitOfWork);
            Dominio.Entidades.WebService.Integradora integradora = repIntegracadora.BuscarPorToken(token);

            if (integradora == null || !integradora.Ativo)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Token inválido. Verifique se o token informado é o mesmo autorizado para a integração.");

            try
            {
                if (limite > 50)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50.");

                if (protocolo == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de integração.");

                if (protocolo.protocoloIntegracaoCarga <= 0 && protocolo.protocoloIntegracaoPedido <= 0)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Por favor, informe os códigos de integração.");

                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Cliente remetente = null;
                Dominio.Entidades.Cliente destinatario = null;

                if (protocolo.Remetente != null)
                {
                    remetente = repositorioCliente.BuscarPorCPFCNPJ(protocolo.Remetente.CPFCNPJ.ObterSomenteNumeros().ToDouble());

                    if (remetente == null)
                        return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos($"Não foi localizado um remetente cadastradado na base Multisoftware para o CNPJ {protocolo.Remetente.CPFCNPJ}.");
                }

                if (protocolo.Destinatario != null)
                {
                    destinatario = repositorioCliente.BuscarPorCPFCNPJ(protocolo.Destinatario.CPFCNPJ.ObterSomenteNumeros().ToDouble());

                    if (destinatario == null)
                        return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Não foi localizado um destinatário cadastradado na base Multisoftware para o CNPJ {protocolo.Remetente.CPFCNPJ}.");
                }

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = repositorioCargaPedido.BuscarPorProtocoloCargaEProtocoloPedidoAutorizados(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);
                //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedidoAutorizados(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if (cargaPedido == null || cargaPedido.Count == 0)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Os protocolos informados não existem na base Multisoftware.");

                if (!configuracao.RetornarCargaPendenciaEmissao && !configuracao.AgruparCargaAutomaticamente && cargaPedido.Any(o => o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos && !o.Carga.AgImportacaoCTe))
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Os documentos do protocolo informado ainda estão em sendo emitidos.");

                if (remetente != null)
                {
                    Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repositorioCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumento = repositorioCargaOcorrenciaDocumento.BuscarOcorrenciasPendentesCargaRemetenteDestinatario(protocolo.protocoloIntegracaoCarga, remetente.CPF_CNPJ, 0);

                    if (cargaOcorrenciaDocumento.Count > 0)
                    {
                        if (cargaOcorrenciaDocumento.FirstOrDefault().CargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada)
                            return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos($"A Ocorrência foi rejeitada para o CT-e do remetente {remetente.Nome} na Carga.", codigoMensagem: 302);

                        else if (cargaOcorrenciaDocumento.Any(obj => obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada))
                            return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos($"Existem ocorrências não aprovadas para o CT-e do remetente {remetente.Nome} na Carga.", codigoMensagem: 301);
                    }
                }

                Servicos.WebService.CTe.CTe servicoCte = new Servicos.WebService.CTe.CTe(unitOfWork);
                int totalRegistros = servicoCte.ContarCTes(configuracao.UtilizaEmissaoMultimodal, protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, remetente?.CPF_CNPJ ?? 0, destinatario?.CPF_CNPJ ?? 0, unitOfWork);
                List<Dominio.ObjetosDeValor.WebService.CTe.CTe> listaCte = (totalRegistros > 0) ? servicoCte.BuscarCTes(
                    configuracao.UtilizaEmissaoMultimodal,
                    cargaPedido,
                    (TipoDocumentoRetorno)tipoDocumentoRetorno,
                    remetente?.CPF_CNPJ ?? 0d,
                    destinatario?.CPF_CNPJ ?? 0d,
                    (int)inicio,
                    (int)limite,
                    unitOfWork
                ) : new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();
                Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe> retorno = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>()
                {
                    Itens = listaCte,
                    NumeroTotalDeRegistro = totalRegistros
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou CT-es", unitOfWork);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoSucesso(retorno);
            }
            catch (Exception excecao)
            {
                if (protocolo != null)
                    Servicos.Log.TratarErro($"Carga {protocolo.protocoloIntegracaoCarga} Pedido {protocolo.protocoloIntegracaoPedido}");

                Servicos.Log.TratarErro(excecao);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os CTes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorBoleto(string nossoNumeroBoleto, string numeroBanco, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            tipoDocumentoRetorno ??= TipoDocumentoRetorno.Nenhum;
            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (limite <= 50)
                {

                    if (!string.IsNullOrWhiteSpace(nossoNumeroBoleto) && !string.IsNullOrWhiteSpace(numeroBanco))
                    {
                        Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);

                        Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarTituloAReceberPorNossoNumero(nossoNumeroBoleto, numeroBanco);

                        if (titulo != null)
                        {
                            string mensagem = "";

                            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

                            retorno.Objeto.Itens = serWSCTe.BuscarCTesPorBoleto(
                                titulo,
                                (TipoDocumentoRetorno)tipoDocumentoRetorno,
                                (int)inicio, (int)limite,
                                ref mensagem,
                                integradora.Empresa,
                                unitOfWork
                            );
                            retorno.Objeto.NumeroTotalDeRegistro = serWSCTe.ContarCTesPorBoleto(titulo, integradora.Empresa, unitOfWork);
                            retorno.Status = true;

                            if (!string.IsNullOrWhiteSpace(mensagem))
                            {
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                retorno.Mensagem = mensagem;
                            }
                            else
                                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Consultou CT-es por Boleto", unitOfWork);
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "O protocolo informado (" + nossoNumeroBoleto + ", " + numeroBanco + ") não é de uma boleto válido, por favor verifique.";
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Por favor, informe o nosso número e o número do banco do boleto.";
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os CTes por Boleto";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite, string codigoTipoOperacao, string situacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>
                retorno = new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCTesPorPeriodo(dataInicial, dataFinal, tipoDocumentoRetorno ?? TipoDocumentoRetorno.Nenhum, inicio ?? 0, limite ?? 0, codigoTipoOperacao, situacao, integradora);

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorPeriodoHora(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite, string codigoTipoOperacao, string situacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>
                retorno = new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCTesPorPeriodo(dataInicial, dataFinal, tipoDocumentoRetorno ?? TipoDocumentoRetorno.Nenhum, inicio ?? 0, limite ?? 0, codigoTipoOperacao, situacao, integradora, true);

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorPeriodoTk(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite, string codigoTipoOperacao, string situacao, string token)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            //Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.WebService.Integradora repIntegracadora = new Repositorio.WebService.Integradora(unitOfWork);
            Dominio.Entidades.WebService.Integradora integradora = repIntegracadora.BuscarPorToken(token);

            if (integradora == null || !integradora.Ativo)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Token inválido. Verifique se o token informado é o mesmo autorizado para a integração.");

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            tipoDocumentoRetorno ??= TipoDocumentoRetorno.Nenhum;
            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite <= 50)
                {
                    DateTime _dataInicial = dataInicial.ToDateTime();
                    DateTime _dataFinal = dataFinal.ToDateTime();

                    if (_dataInicial != DateTime.MinValue && _dataFinal != DateTime.MinValue)
                    {
                        Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                        Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                        Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWS = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS = repConfiguracaoWS.BuscarConfiguracaoPadrao();

                        List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracaos = repTipoIntegracao.BuscarTipos();
                        string mensagem = "";

                        bool somentePosIntegracao = tipoIntegracaos.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain);

                        retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>
                        {
                            Itens = serWSCTe.BuscarCTesPeriodo(_dataInicial, _dataFinal, integradora.Empresa?.Codigo ?? 0, integradora.Cliente?.CPF_CNPJ ?? 0, somentePosIntegracao, (TipoDocumentoRetorno)tipoDocumentoRetorno, (int)inicio, (int)limite, codigoTipoOperacao, situacao, ref mensagem, unitOfWork, configuracaoWS),
                            NumeroTotalDeRegistro = serWSCTe.ContarCTesPeriodo(_dataInicial, _dataFinal, integradora.Empresa?.Codigo ?? 0, integradora.Cliente?.CPF_CNPJ ?? 0, somentePosIntegracao, codigoTipoOperacao, situacao, unitOfWork, configuracaoWS)
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

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorOcorrencia(int protocoloIntegracaoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                RequestCtePorOcorrencia dadosRequestCteOcorrencia = new RequestCtePorOcorrencia()
                {
                    Inicio = inicio ?? 0,
                    Limite = limite ?? 0,
                    ProtocoloIntegracaoOcorrencia = protocoloIntegracaoOcorrencia,
                    TipoDocumentoRetorno = tipoDocumentoRetorno ?? TipoDocumentoRetorno.Nenhum
                };
                Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> retorno = new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCTesPorOcorrencia(dadosRequestCteOcorrencia, integradora);

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTePorProtocolo(int protocoloCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCTePorProtocolo(protocoloCTe, tipoDocumentoRetorno ?? TipoDocumentoRetorno.Nenhum, integradora));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> BuscarCTesComplementaresAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, string codificarUTF8, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<CTeComplementar>> retorno = new Servicos.WebService.CTe.CTe(
                    unitOfWork,
                    TipoServicoMultisoftware,
                    Cliente,
                    Auditado,
                    ClienteAcesso,
                    Conexao.createInstance(_serviceProvider).AdminStringConexao
                ).BuscarCTesComplementaresAguardandoIntegracao(
                    tipoDocumentoRetorno ?? TipoDocumentoRetorno.Nenhum,
                    codificarUTF8,
                    inicio ?? 0,
                    limite ?? 0
                );
                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> BuscarCTesSubstitutosAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, string codificarUTF8, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<CTeComplementar>> retorno = new Servicos.WebService.CTe.CTe(
                    unitOfWork,
                    TipoServicoMultisoftware,
                    Cliente,
                    Auditado,
                    ClienteAcesso,
                    Conexao.createInstance(_serviceProvider).AdminStringConexao
                ).BuscarCTesSubstitutosAguardandoIntegracao(
                    tipoDocumentoRetorno ?? TipoDocumentoRetorno.Nenhum,
                    codificarUTF8,
                    inicio ?? 0,
                    limite ?? 0
                );
                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> ConfirmarIntegracaoCTeComplementar(int protocoloCTe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoCTeComplementar(protocoloCTe));
            });
        }

        public Retorno<bool> ConfirmarIntegracaoCTeSubstituto(int protocoloCTe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoCTeComplementar(protocoloCTe));
            });
        }

        public Retorno<bool> GerarCCe(Dominio.ObjetosDeValor.WebService.CTe.CamposCCe camposCCe, int protocoloCTe)
        {
            Servicos.Log.TratarErro($"GerarCCe: {(camposCCe != null ? Newtonsoft.Json.JsonConvert.SerializeObject(camposCCe) : string.Empty)}", "Request");

            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.CCe svcCCe = new Servicos.CCe(unitOfWork);

            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);
            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unitOfWork);
            Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            retorno.Status = true;
            try
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTE.BuscarPorCodigo(protocoloCTe);

                if (cte != null)
                {
                    if (camposCCe != null && camposCCe.CampoCCe != null && camposCCe.CampoCCe.Count > 0)
                    {
                        if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Normal)
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Não é possível gerar carta de correção para o CT-e informado.";
                        }
                        else if (cte.Status == "A")
                        {
                            unitOfWork.Start();
                            Dominio.Entidades.CartaDeCorrecaoEletronica cce = new Dominio.Entidades.CartaDeCorrecaoEletronica()
                            {
                                CTe = cte,
                                DataEmissao = DateTime.Now,
                                Log = "Gerado via integração WS",
                                Status = Dominio.Enumeradores.StatusCCe.Pendente,
                                NumeroSequencialEvento = (repCCe.BuscarUltimoNumeroSequencial(cte.Codigo) + 1)
                            };

                            repCCe.Inserir(cce);

                            foreach (var item in camposCCe.CampoCCe)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Descricao) || !string.IsNullOrWhiteSpace(item.GrupoCampo) || !string.IsNullOrWhiteSpace(item.NomeCampo))
                                {
                                    //Buscar alteraões anteriores
                                    Dominio.Entidades.CartaDeCorrecaoEletronica ultimaCarga = repCCe.BuscarUltimaCCeAutorizadaPorCTe(cce.CTe.Codigo);
                                    if (ultimaCarga != null)
                                    {
                                        List<Dominio.Entidades.ItemCCe> itensAnteriores = repItemCCe.BuscarPorCCe(ultimaCarga.Codigo);
                                        foreach (var itemAnterior in itensAnteriores)
                                        {
                                            Dominio.Entidades.ItemCCe itemAnteriorCCe = new Dominio.Entidades.ItemCCe();

                                            itemAnteriorCCe.CCe = cce;
                                            itemAnteriorCCe.CampoAlterado = itemAnterior.CampoAlterado;
                                            itemAnteriorCCe.NumeroItemAlterado = itemAnterior.NumeroItemAlterado;
                                            itemAnteriorCCe.ValorAlterado = itemAnterior.ValorAlterado;

                                            repItemCCe.Inserir(itemAnteriorCCe);
                                        }
                                    }

                                    Dominio.Entidades.CampoCCe campoCCe = repCampoCCe.BuscarPorNomeCampoGrupo(item.NomeCampo, item.GrupoCampo);
                                    if (campoCCe == null)
                                    {
                                        campoCCe = new Dominio.Entidades.CampoCCe()
                                        {
                                            Descricao = item.Descricao,
                                            GrupoCampo = item.GrupoCampo,
                                            IndicadorRepeticao = item.IndicadorRepeticao,
                                            NomeCampo = item.NomeCampo,
                                            Status = "A",
                                            TipoCampo = item.TipoCampo,
                                            QuantidadeCaracteres = item.QuantidadeCaracteres,
                                            QuantidadeDecimais = item.QuantidadeDecimais,
                                            QuantidadeInteiros = item.QuantidadeInteiros
                                        };
                                        repCampoCCe.Inserir(campoCCe);

                                    }
                                    Dominio.Entidades.ItemCCe itemCCe = new Dominio.Entidades.ItemCCe();

                                    itemCCe.CCe = cce;
                                    itemCCe.CampoAlterado = campoCCe;
                                    itemCCe.NumeroItemAlterado = item.NumeroItemAlterado;

                                    string valorAnterior = "";
                                    if (campoCCe.Descricao.ToLower().Contains("navio"))
                                    {
                                        if (cce.CTe.Navio != null && !string.IsNullOrWhiteSpace(cce.CTe.NumeroViagem) && !string.IsNullOrWhiteSpace(cce.CTe.Direcao))
                                        {
                                            valorAnterior = cce.CTe.Navio?.Descricao ?? "";
                                            valorAnterior += ("/" + cce.CTe.NumeroViagem + cce.CTe.Direcao);
                                        }
                                        else
                                            valorAnterior = cce.CTe.Viagem?.Descricao ?? "";
                                        valorAnterior = "Onde se lê: " + valorAnterior;
                                    }

                                    if (!string.IsNullOrWhiteSpace(valorAnterior))
                                        valorAnterior += " > Leia-se: " + item.ValorAlterado;
                                    else
                                        valorAnterior = item.ValorAlterado;


                                    item.ValorAlterado = valorAnterior;
                                    itemCCe.ValorAlterado = item.ValorAlterado;

                                    repItemCCe.Inserir(itemCCe);
                                }
                                else
                                {
                                    retorno.Status = false;
                                    unitOfWork.Rollback();
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "Dados faltentes para a geração da carta de correção, favor verifique os campos obrigatórios.";
                                }
                            }

                            if (retorno.Status)
                            {
                                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTe.EmitirCCe(cce, cce.CTe.Empresa.Codigo, unitOfWork))
                                {
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cce.CTe, "Gerou carta de correção via integração.", unitOfWork);
                                    unitOfWork.CommitChanges();
                                }
                                else
                                {
                                    retorno.Status = false;
                                    unitOfWork.Rollback();
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "Falha ao emitir a carta de correção, favor tente novamente.";
                                }
                            }
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "O CT-e informado não se encontra autorizado.";
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Favor informe os campos que deseja realizar a carta de correção.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O protocólo do CT-e informado não existe na base da Multisoftware";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar confirmar a integração do CT-e complementar.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> EnviarCCe(Dominio.ObjetosDeValor.WebService.CTe.CCe cce)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CCe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarCCe(cce));
            });
        }

        public Retorno<Paginacao<int>> BuscarCTesComplementaresAguardandoConfirmacaoCancelamento(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<int>> retorno = new Retorno<Paginacao<int>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Status = false;
            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
            retorno.Mensagem = "Ocorreu uma falha ao consultar os CTes";
            return retorno;
        }

        public Retorno<bool> ConfirmarRecebimentoCancelamentoCTeComplementar(int protocoloCTe)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = false;
            retorno.Objeto = false;
            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
            return retorno;
        }

        public Retorno<bool> InformarPrevisaoPagamentoCTe(int protocoloCTe, string dataPrevisaoPagamento, string observacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe informarPrevisaoPagamentoCTe = new Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe()
                {
                    ProtocoloCTe = protocoloCTe,
                    DataPrevisaoPagamento = dataPrevisaoPagamento,
                    Observacao = observacao
                };

                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).InformarPrevisaoPagamentoCTe(new List<InformarPrevisaoPagamentoCTe>() { informarPrevisaoPagamentoCTe }));
            });
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe>> InformarPrevisoesPagamentosCTe(List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe> InformacoesPrevisaoPagamentoCTe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe>>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).InformarPrevisoesPagamentosCTe(InformacoesPrevisaoPagamentoCTe));
            });
        }

        public Retorno<bool> InformarBloqueioDocumento(int protocoloCTe, string dataBloqueio, string observacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).InformarBloqueioDocumento(protocoloCTe, dataBloqueio, observacao));
            });
        }

        public Retorno<bool> InformarDesbloqueioDocumento(Dominio.ObjetosDeValor.WebService.CTe.InformarDesbloqueioDocumento informarDesbloqueioDocumento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).InformarDesbloqueioDocumento(informarDesbloqueioDocumento));
            });
        }

        public Retorno<bool> ConfirmarPagamentoCTe(int protocoloCTe, string dataPagamento, string observacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.CTe.ConfirmarPagamentoCTe confirmarPagamentoCTe = new Dominio.ObjetosDeValor.WebService.CTe.ConfirmarPagamentoCTe()
                {
                    ProtocoloCTe = protocoloCTe,
                    DataPagamento = dataPagamento,
                    Observacao = observacao
                };

                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarPagamentoCTe(confirmarPagamentoCTe));
            });
        }

        public Retorno<Paginacao<int>> BuscarProtocoloCTesCanceladosAguardandoConfirmacaoConsulta(int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>>.CreateFrom(
                    new Servicos.WebService.CTe.CTe(
                        unitOfWork,
                        TipoServicoMultisoftware,
                        Cliente,
                        Auditado,
                        ClienteAcesso,
                        Conexao.createInstance(_serviceProvider).AdminStringConexao
                    ).BuscarProtocoloCTesCanceladosAguardandoConfirmacaoConsulta(
                        inicio ?? 0,
                        limite ?? 0,
                        integradora
                    )
                );
                return new Retorno<Paginacao<int>>()
                {
                    Objeto = Paginacao<int>.CreateFrom(retorno.Objeto),
                    CodigoMensagem = retorno.CodigoMensagem,
                    Status = retorno.Status,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.Titulo>> BuscarTitulosPorPeriodo(int? inicio, int? limite, string dataInicial, string dataFinal)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite > 50)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.Titulo>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50.");

                DateTime dataInicio = dataInicial.ToDateTime();
                DateTime dataFim = dataFinal.ToDateTime();

                Paginacao<Dominio.ObjetosDeValor.WebService.CTe.Titulo> retorno = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.Titulo>();
                Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                int quantidadeRegistros = repositorioCTe.ContarConsultaCTesPorPeriodo(dataInicio, dataFim, configuracao.RetornarTitulosNaoGerados);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = quantidadeRegistros > 0 ? repositorioCTe.ConsultarCTesPorPeriodo(dataInicio, dataFim, configuracao.RetornarTitulosNaoGerados, (int)inicio, (int)limite) : new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

                retorno.Itens = serWSCTe.ConverterObjetoTitulo(listaCTes, unitOfWork);
                retorno.NumeroTotalDeRegistro = quantidadeRegistros;

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.Titulo>>.CriarRetornoSucesso(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.Titulo>>.CriarRetornoExcecao("Ocorreu uma falha ao buscar os títulos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTeCanceladoPorProtocolo(int protocoloCTe, string statusCTe)
        {
            var integradora = ValidarToken();

            Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (protocoloCTe > 0)
                {
                    Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);

                    int codigoTransportadora = integradora?.Empresa?.Codigo ?? 0;

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                    if (string.IsNullOrWhiteSpace(statusCTe))
                        cte = repCTe.BuscarPorCodigo(codigoTransportadora, protocoloCTe);
                    else
                        cte = repCTe.BuscarCodigoEStatus(protocoloCTe, statusCTe, codigoTransportadora);

                    if (cte != null)
                    {
                        List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTe(cte.Codigo);
                        retorno.Objeto = serWSCTe.ConverterObjetoCTe(cte, cTeContaContabilContabilizacaos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF);
                        retorno.Status = true;

                        Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou CT-es cancelados por protocolo", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O protocolo informado não é de um CT-e existente.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Por favor, informe o Protocolo do CT-e.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar o CTe";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> ConfirmarConsultaCTeCancelado(int protocoloCTe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarConsultaCTeCancelado(protocoloCTe, integradora));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarOutrosDocsPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;
            tipoDocumentoRetorno ??= TipoDocumentoRetorno.Nenhum;

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>();
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
                        Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                        string mensagem = "";

                        retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>
                        {
                            Itens = serWSCTe.BuscarOutrosDocsPeriodo(
                                _dataInicial,
                                _dataFinal,
                                integradora.Empresa?.Codigo ?? 0,
                                (TipoDocumentoRetorno)tipoDocumentoRetorno,
                                (int)inicio,
                                (int)limite,
                                ref mensagem,
                                unitOfWork
                            ),
                            NumeroTotalDeRegistro = serWSCTe.ContarOutrosDocsPeriodo(_dataInicial, _dataFinal, integradora.Empresa?.Codigo ?? 0, unitOfWork)
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

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>> BuscarFaturaCTes(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>> retorno = new Servicos.WebService.CTe.CTe(
                    unitOfWork,
                    TipoServicoMultisoftware,
                    Cliente,
                    Auditado,
                    ClienteAcesso,
                    Conexao.createInstance(_serviceProvider).AdminStringConexao
                ).BuscarFaturaCTes(
                    protocolo, tipoDocumentoRetorno ?? TipoDocumentoRetorno.Nenhum,
                    inicio ?? 0,
                    limite ?? 0
                );

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> QuitarTituloCTe(int? codigoTitulo, string dataPagamento, string observacao, decimal? valorAcrescimo, decimal? valorDesconto)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            retorno.Status = true;

            codigoTitulo ??= 0;
            valorAcrescimo ??= 0m;
            valorDesconto ??= 0m;

            try
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo((int)codigoTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber);
                if (titulo != null)
                {
                    DateTime dataPagto;
                    if (!DateTime.TryParseExact(dataPagamento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPagto))
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A data de pagamento não esta em um formato correto (dd/MM/yyyy)";
                    }
                    ;
                    if (titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && !repTituloBaixa.ContemTituloEmBaixaGerada(titulo.Codigo))
                    {
                        if (retorno.CodigoMensagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos)
                        {
                            string erro = "";
                            if (Servicos.Embarcador.Financeiro.Titulo.QuitarTitulo(out erro, titulo, dataPagto, dataPagto, unitOfWork, titulo.Pessoa, null, null, TipoServicoMultisoftware, "COMANDO DE BAIXA VIA ARQUIVO DE RETORNO", (decimal)valorAcrescimo, true, (decimal)valorDesconto, true, Auditado))
                            {
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, "Confirmou o pagamento do título por serviço de retorno", unitOfWork);
                            }
                            else
                            {
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, "Falha ao quitar o título por WS " + erro, unitOfWork);
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                retorno.Mensagem = erro;
                            }
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Objeto = true;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                        retorno.Mensagem = "Este título não se encontra em aberto.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O código do Título informado não existe na base da Multisoftware";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar quitar o pagamento do título.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.CTe.FaturasCTe> BuscarFaturasCTePorProtocolo(int protocoloCTe)
        {
            ValidarToken();

            Retorno<Dominio.ObjetosDeValor.WebService.CTe.FaturasCTe> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.CTe.FaturasCTe>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (protocoloCTe > 0)
                {
                    Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(protocoloCTe);
                    if (cargaCTe != null)
                    {
                        retorno.Objeto = serWSCTe.ConverterObjetoFaturasCTe(cargaCTe, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF);
                        retorno.Status = true;

                        Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Faturas do CT-e por protocolo", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O protocolo informado não é de um CT-e existente.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Por favor, informe o Protocolo do CT-e.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar o CTe";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe> BuscarNotaFiscal(string chaveNFe)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe>()
            {
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Mensagem = ""
            };

            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorChave(chaveNFe);

                if (xmlNotaFiscal != null && !string.IsNullOrWhiteSpace(xmlNotaFiscal.XML))
                {
                    retorno.Objeto = new Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe()
                    {
                        ChaveNFe = xmlNotaFiscal.Chave,
                        Numero = xmlNotaFiscal.Numero.ToString(),
                        Serie = xmlNotaFiscal.SerieOuSerieDaChave,
                        XML = xmlNotaFiscal.XML
                    };

                    return retorno;
                }

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa = repDocumentoDestinadoEmpresa.BuscarPorChave(chaveNFe);

                if (documentoDestinadoEmpresa != null)
                {
                    string caminho = Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterCaminhoDocumentoDestinado(documentoDestinadoEmpresa, unitOfWork);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    {
                        retorno.Objeto = new Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe()
                        {
                            ChaveNFe = documentoDestinadoEmpresa.Chave,
                            Numero = documentoDestinadoEmpresa.Numero.ToString(),
                            Serie = documentoDestinadoEmpresa.Serie.ToString(),
                            XML = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminho)
                        };

                        return retorno;
                    }
                }

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "NF-e não encontrada.";

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao buscar a nota fiscal.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<int>> BuscarCTesFaturadosPendenteDeIntegracao(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<int>> retorno = new Retorno<Paginacao<int>>();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (limite <= 100)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                    List<int> codigosCTes = repCTe.BuscarCTesFaturadosPendenteDeIntegracao((int)inicio, (int)limite);
                    retorno.Objeto = new Paginacao<int>();
                    retorno.Objeto.NumeroTotalDeRegistro = repCTe.ContarBuscarCTesFaturadosPendenteDeIntegracao();
                    retorno.Objeto.Itens = new List<int>();
                    foreach (var codigo in codigosCTes)
                    {
                        retorno.Objeto.Itens.Add(codigo);
                    }
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou CTes Faturados aguardando confirmação de integração", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os CT-es pendente de faturamento";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoCTeFaturado(int protocoloCTe)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            try
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(protocoloCTe);

                if (cte != null)
                {
                    cte.CTePendenteIntegracaoFatura = false;
                    repCTe.Atualizar(cte);

                    retorno.Objeto = true;
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, "Confirmou consulta de CT-es Faturado", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Protocolo do CTe não localizado";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Falha ao confirmar consulta CTe Faturado";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.CTe.SituacaoCTe>> VerificarSituacaoCTe(List<string> chavesCTe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Retorno<List<SituacaoCTe>> retorno = new Servicos.WebService.CTe.CTe(
                    unitOfWork,
                    TipoServicoMultisoftware,
                    Cliente,
                    Auditado,
                    ClienteAcesso,
                    Conexao.createInstance(_serviceProvider).AdminStringConexao
                ).VerificarSituacaoCTe(new Dominio.ObjetosDeValor.Embarcador.CTe.VerificarSituacaoCTe
                {
                    ChavesCTe = chavesCTe
                });

                return Retorno<List<Dominio.ObjetosDeValor.WebService.CTe.SituacaoCTe>>.CreateFrom(retorno);
            });
        }


        public Retorno<string> EnviarArquivoXMLCTe(Stream arquivo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<string>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarArquivoXMLCTe(arquivo));
            });
        }

        public Retorno<bool> AnularCTe(int? protocoloCTe, string dataEventoDesacordo, decimal? valorCTeSustituicao, string cnpjTomadorCTeSubstituto, string observacaoCTeAnulacao, string observacaoCTeSustituicao)
        {
            protocoloCTe ??= 0;
            valorCTeSustituicao ??= 0;

            Servicos.Log.TratarErro($"AnularCTe: protocoloCTe = {protocoloCTe.ToString()} dataEventoDesacordo = {dataEventoDesacordo} observacaoCTeAnulacao = {observacaoCTeAnulacao} observacaoCTeSustituicao = {observacaoCTeSustituicao} valorCTeSustituicao = {valorCTeSustituicao} cnpjTomadorCTeSubstituto = {cnpjTomadorCTeSubstituto}");

            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.CCe svcCCe = new Servicos.CCe(unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Servicos.Embarcador.CTe.CTe svcCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            retorno.Status = true;
            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe((int)protocoloCTe);

                if (cargaCTe != null)
                {
                    if (valorCTeSustituicao == 0)
                        valorCTeSustituicao = cargaCTe.CTe.ValorAReceber;

                    DateTime dataEvento = DateTime.MinValue;
                    double.TryParse(Utilidades.String.OnlyNumbers(cnpjTomadorCTeSubstituto), out double cnpjTomador);

                    if (cnpjTomador > 0 && repCliente.BuscarPorCPFCNPJ(cnpjTomador) == null)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = $"CNPJ do tomador {cnpjTomadorCTeSubstituto} não possui cadastro, favor cadastrar no portal.";
                    }
                    else if (string.IsNullOrWhiteSpace(dataEventoDesacordo))
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = $"Data de evento desacordo inválida, formato esperado  dd/MM/yyyy HH:mm:ss.";
                    }
                    else if (!DateTime.TryParseExact(dataEventoDesacordo, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEvento))
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = $"Data de evento desacordo {dataEventoDesacordo} inválida, formato esperado  dd/MM/yyyy HH:mm:ss.";
                    }
                    else if (dataEvento == DateTime.MinValue)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = $"Data de evento desacordo {dataEventoDesacordo} inválida, formato esperado  dd/MM/yyyy HH:mm:ss.";
                    }
                    else if (valorCTeSustituicao > cargaCTe.CTe.ValorAReceber)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O valor do frete para o CT-e de substituição deve ser menor que o valor do frete do CT-e original.";
                    }
                    else if (cargaCTe.CTe.Status != "A" && cargaCTe.CTe.Status != "Z")
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = $"A situação do CT-e ({cargaCTe.CTe.DescricaoStatus}) não permite a emissão de CT-e de anulação.";
                    }
                    else if (cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao &&
                             cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte &&
                             cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada &&
                             cargaCTe.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = $"A situação da carga não permite a emissão de CT-e de anulação.";
                    }
                    else if (cargaCTe.Carga.CargaTransbordo)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Não é permitido editar/gerar CT-es em uma carga de transbordo. Selecione a carga original do CT-e.";
                    }
                    else
                    {
                        if (!Servicos.Embarcador.CTe.CTe.GerarCTeAnulacao(out string mensagemErro, out Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, cargaCTe, cargaCTe.Carga.Codigo, dataEvento, (decimal)valorCTeSustituicao, observacaoCTeAnulacao, observacaoCTeSustituicao, unitOfWork, Auditado, cnpjTomador, configuracao, false))
                        {
                            if (controleGeracaoCTeAnulacao != null)
                            {
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                retorno.Mensagem = "Anulação não gerada, consulte o portal para verificar.";
                            }
                            else
                            {
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                retorno.Mensagem = mensagemErro;
                            }
                        }

                        if (configuracao?.DeixarCargaPendenteDeIntegracaoAposCTeManual ?? false)
                        {
                            Servicos.Log.TratarErro($"Atualização de CAR_CARGA_INTEGRADA_EMBARCADOR na carga {cargaCTe.Carga.Codigo} para false pelo CTe.svc.cs > AnularCTe", "AtualizacaoCargaIntegradaEmbarcador");

                            cargaCTe.Carga.CargaIntegradaEmbarcador = false;
                            repCarga.Atualizar(cargaCTe.Carga);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.Carga, null, "Alterou a carga para pendente de integração devido a anulação do CT-e.", unitOfWork);
                        }

                        retorno.Objeto = true;
                    }

                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O protocolo do CT-e informado não existe na base da Multisoftware";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao solicitar anulação do CT-e.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> IntegrarDadosCTesAnteriores(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> Ctes)
        {
            Servicos.Log.TratarErro("IntegrarDadosCTesAnteriores - Protocolo: " + (protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty), "Request");
            Servicos.Log.TratarErro("IntegrarDadosCTesAnteriores - Ctes: " + (Ctes != null ? Newtonsoft.Json.JsonConvert.SerializeObject(Ctes) : string.Empty), "Request");
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            retorno.Status = true;
            StringBuilder stMensagem = new StringBuilder();

            try
            {
                if (protocolo.protocoloIntegracaoCarga > 0 || protocolo.protocoloIntegracaoPedido > 0)
                {
                    Servicos.WebService.CTe.CTe servicoWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao);
                    Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);

                    Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

                    if (protocolo.protocoloIntegracaoCarga > 0 && !string.IsNullOrEmpty(protocolo.NumeroContainerPedido))
                        cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEContainerPedido(protocolo.protocoloIntegracaoCarga, protocolo.NumeroContainerPedido);
                    if (cargaPedido == null && protocolo.protocoloIntegracaoCarga > 0 && protocolo.protocoloIntegracaoPedido > 0)
                        cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);
                    else if (cargaPedido == null && protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido > 0)
                        cargaPedido = repCargaPedido.BuscarCargaAtualPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);
                    else if (cargaPedido == null && protocolo.protocoloIntegracaoCarga > 0 && protocolo.protocoloIntegracaoPedido == 0)
                        cargaPedido = repCargaPedido.BuscarCargaAtualPorProtocoloCarga(protocolo.protocoloIntegracaoCarga, !string.IsNullOrEmpty(protocolo.NumeroContainerPedido));

                    if (cargaPedido != null && cargaPedido.Pedido != null && !string.IsNullOrEmpty(protocolo.NumeroContainerPedido))
                    {
                        cargaPedido.Pedido.Container = repContainer.BuscarPorNumero(protocolo.NumeroContainerPedido);
                        if (cargaPedido.Pedido.Container == null)
                        {
                            cargaPedido.Pedido.Container = serPedidoWS.SalvarContainer(protocolo.Container, ref stMensagem, Auditado);
                            if (stMensagem.Length > 0)
                            {
                                retorno.Status = false;
                                retorno.Mensagem = stMensagem.ToString();
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                return retorno;
                            }
                        }
                        cargaPedido.Pedido.TaraContainer = protocolo.TaraContainer;
                        cargaPedido.Pedido.LacreContainerUm = protocolo.LacreContainerUm;
                        cargaPedido.Pedido.LacreContainerDois = protocolo.LacreContainerDois;
                        cargaPedido.Pedido.LacreContainerTres = protocolo.LacreContainerTres;

                        repPedido.Atualizar(cargaPedido.Pedido);
                    }

                    if (cargaPedido != null && Ctes != null && Ctes.Count > 0)
                    {
                        retorno.Mensagem = servicoWSCTe.VincularCteAnteriorNaCarga(Ctes, cargaPedido, configuracao, false, false);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Mensagem = "Protocolos informados são inválidos. ";
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "É obrigatório informar os protocolos de integração. ";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao enviar o(s) CT-e(s)";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            if (string.IsNullOrEmpty(retorno.Mensagem))
                retorno.Objeto = true;

            return retorno;
        }

        public Retorno<bool> IntegrarCTesAnteriores(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.CTe.TokenCte> TokensXMLCtes)
        {
            Servicos.Log.TratarErro("IntegrarNotasFiscais - Protocolo: " + (protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty), "Request");
            Servicos.Log.TratarErro("IntegrarNotasFiscais - TokensXMLNotasFiscais: " + (TokensXMLCtes != null ? Newtonsoft.Json.JsonConvert.SerializeObject(TokensXMLCtes) : string.Empty), "Request");

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.WebService.CTe.CTe servicoWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao);

            Retorno<bool> retorno = new Retorno<bool>();
            List<string> caminhosXMLTemp = new List<string>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            retorno.Status = true;

            try
            {
                unitOfWork.Start();
                if (protocolo.protocoloIntegracaoCarga > 0 || protocolo.protocoloIntegracaoPedido > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                    if (cargaPedido == null && protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido > 0)
                        cargaPedido = repCargaPedido.BuscarCargaAtualPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);

                    if (cargaPedido != null)
                    {
                        string path = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;

                        if (TokensXMLCtes != null && TokensXMLCtes.Count > 0)
                        {
                            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.TokenCte tokenXML in TokensXMLCtes)
                            {
                                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(tokenXML.Token, ".xml"));
                                bool tudoCerto = true;
                                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = null;
                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                {
                                    System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));
                                    try
                                    {
                                        Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);
                                        object objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(reader.BaseStream);
                                        cte = serCte.ConverterProcCTeParaCTePorObjeto(objCTe);
                                    }
                                    catch (Exception e)
                                    {
                                        retorno.Mensagem = "O xml enviado não é de uma nota fiscal autorizada.";
                                        retorno.Status = false;
                                        tudoCerto = false;
                                        Servicos.Log.TratarErro("O xml enviado não é de uma nota fiscal autorizada: " + caminho + " " + e.Message);
                                        reader.Dispose();
                                        break;
                                    }

                                    if (tudoCerto && cte != null)
                                    {
                                        List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> Ctes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
                                        Ctes.Add(cte);

                                        retorno.Mensagem = servicoWSCTe.VincularCteAnteriorNaCarga(Ctes, cargaPedido, configuracao, false, false);
                                    }

                                    unitOfWork.CommitChanges();
                                    caminhosXMLTemp.Add(caminho);
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    retorno.Mensagem += "O Token informado não existe mais físicamente no servidor, por favor, envie o XML da nota novamente e receba um novo token.";
                                    retorno.Status = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            retorno.Status = false;
                            retorno.Mensagem = "É obrigatório informar Token XML integração. ";
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        retorno.Status = false;
                        retorno.Mensagem += "protocolo de Carga e/ou protocolo de Pedido não encontrado.";
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    retorno.Status = false;
                    retorno.Mensagem = "É obrigatório informar os protocolos de integração. ";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
                //if (retorno.Status)
                //{

                //    foreach (string caminho in caminhosXMLTemp)
                //        Utilidades.IO.FileStorageService.Storage.Delete(caminho);
                //}

                Servicos.Log.TratarErro("IntegrarNotasFiscais - Retorno: " + (!retorno.Status ? retorno.Mensagem : "Sucesso"));

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                ArmazenarLogIntegracao(TokensXMLCtes, unitOfWork);

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a(s) CT-e(s)";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            if (string.IsNullOrEmpty(retorno.Mensagem))
                retorno.Objeto = true;

            return retorno;
        }

        public Retorno<bool> ReemitirCTesRejeitados(int protocoloCarga)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            retorno.Status = true;
            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloCarga);

                if (carga != null)
                {
                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
                    {
                        carga.EmitindoCTes = true;
                        carga.problemaCTE = false;
                        carga.problemaMDFe = false;
                        carga.MotivoPendencia = string.Empty;
                        carga.PossuiPendencia = false;

                        repCarga.Atualizar(carga);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Reemitiu os documentos.", unitOfWork);
                        retorno.Status = true;
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Situação da carga " + carga.DescricaoSituacaoCarga + " não permite reemitir documentos.";
                    }

                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Protocolo " + protocoloCarga.ToString() + " inexistente.";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar quitar o pagamento do título.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            if (string.IsNullOrEmpty(retorno.Mensagem))
                retorno.Objeto = true;

            return retorno;
        }

        public Retorno<bool> RetornarIntegracaoPagamento(Dominio.ObjetosDeValor.Embarcador.Escrituracao.RetornoIntegracaoPagamento retornoIntegracaoPagamento)
        {
            ValidarToken();

            //Retorno<bool> retorno = new Retorno<bool>();
            //retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);
            //retorno.Status = true;
            try
            {
                if (retornoIntegracaoPagamento != null)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao = repPagamentoIntegracao.BuscarPorCodigoCte(retornoIntegracaoPagamento.ProtolocoIntegracao);
                    if (pagamentoIntegracao != null)
                    {
                        if (retornoIntegracaoPagamento.ProcessadoSucesso)
                            pagamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        else
                        {
                            if (pagamentoIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                                pagamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            else
                            {
                                return Retorno<bool>.CriarRetornoDadosInvalidos("Não é possível rejeitar a integração pois ela já foi confirmada anteriormente");
                            }
                        }

                        Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);

                        pagamentoIntegracao.ProblemaIntegracao = retornoIntegracaoPagamento.MensagemRetorno;
                        string jsonRequisicao = Newtonsoft.Json.JsonConvert.SerializeObject(retornoIntegracaoPagamento);
                        string jsonRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);
                        AdicionarArquivoTransacaoIntegracaoPagamento(pagamentoIntegracao, pagamentoIntegracao.ProblemaIntegracao, jsonRequisicao, jsonRetorno, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoIntegracao, "Retornou a integração do documento", unitOfWork);
                        repPagamentoIntegracao.Atualizar(pagamentoIntegracao);

                        List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos = new List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
                        pagamentos.Add(pagamentoIntegracao.Pagamento);
                        Servicos.Embarcador.Integracao.IntegracaoPagamento.ValidarRetornoPagamentos(pagamentos, unitOfWork);

                        return retorno;
                        //Se ocorrer de não setar o status do pagamento é porque algo deu problema entre setar o status e confirmar a integração, neste caso o método abaixo (que também está comentado no serviço) pode ser utilizado para verificar tudo, que está em integração.
                        //Servicos.Embarcador.Integracao.IntegracaoPagamento.VerificarPagamentosEmIntegração(pagamentos, unitOfWork);

                    }
                    else
                    {
                        return Retorno<bool>.CriarRetornoDadosInvalidos("O protocólo do documento informado não existe na base da Multisoftware");
                    }
                }
                else
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar os dados do retorno da integração do pagamento");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao retornar a integração do documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarDadosDoMercante(Dominio.ObjetosDeValor.WebService.CTe.DadosDoMercante dadosDoMercante)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SalvarDadosDoMercante(dadosDoMercante));
            });
        }

        public Retorno<bool> IntegrarDadosCTeAnteriores(Dominio.ObjetosDeValor.WebService.CTe.DadosCTes dadosCtes)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).IntegrarDadosCTeAnteriores(dadosCtes));
            });
        }

        public Retorno<bool> RealizarAnulacaoGerencial(RequestAnulacaoGerencial requestAnulacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).RealizarAnulacaoGerencial(requestAnulacao));
            });
        }

        public Retorno<bool> EnviarCTe(Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, int? protocoloCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarCTe(cte, protocoloCarga ?? 0));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorCarga(int? protocoloIntegracaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                RequestCtePorCarga requestBuscarCte = new RequestCtePorCarga()
                {
                    TipoDocumentoRetorno = tipoDocumentoRetorno ?? TipoDocumentoRetorno.Nenhum,
                    Inicio = inicio ?? 0,
                    Limite = limite ?? 0,
                    ProtocoloCarga = protocoloIntegracaoCarga ?? 0
                };

                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCTesPorCarga(requestBuscarCte, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> InformarEscrituracaoCTe(int? protocoloCTe, string codigoEscrituracao)
        {
            ValidarToken();

            protocoloCTe ??= 0;

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            unitOfWork.Start();

            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            retorno.Status = true;

            try
            {
                if (!string.IsNullOrWhiteSpace(codigoEscrituracao))
                {

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCTe.BuscarPorCodigo((int)protocoloCTe);
                    if (cte != null)
                    {
                        cte.CodigoEscrituracao = codigoEscrituracao;

                        repositorioCTe.Atualizar(cte);

                        retorno.Status = true;
                        retorno.Objeto = true;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, "Informou a escrituração do CT-e", unitOfWork);

                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        retorno.Status = false;
                        unitOfWork.Rollback();
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O protocólo do CT-e informado não existe na base da Multisoftware";
                    }
                }
                else
                {
                    retorno.Status = false;
                    unitOfWork.Rollback();
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "É necessário informar o código da escrituração.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao informar a escrituração do CT-e.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> EnviarCTesAnteriores(Dominio.ObjetosDeValor.WebService.CTe.RequestCteAnteriores requestCteAnteriores)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarCTesAnteriores(requestCteAnteriores));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesAlteradosPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite, string codigoTipoOperacao, string situacao)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            inicio ??= 0;
            limite ??= 0;
            tipoDocumentoRetorno ??= TipoDocumentoRetorno.Nenhum;

            try
            {
                if (limite <= 50)
                {
                    DateTime _dataInicial = dataInicial.ToDateTime();
                    DateTime _dataFinal = dataFinal.ToDateTime();

                    if (_dataInicial != DateTime.MinValue && _dataFinal != DateTime.MinValue)
                    {
                        Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
                        Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                        Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWS = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS = repConfiguracaoWS.BuscarConfiguracaoPadrao();

                        List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracaos = repTipoIntegracao.BuscarTipos();

                        string mensagem = "";

                        bool somentePosIntegracao = tipoIntegracaos.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain);

                        retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>
                        {
                            Itens = serWSCTe.BuscarCTesAlteradosPeriodo(_dataInicial, _dataFinal, integradora.Empresa?.Codigo ?? 0, integradora.Cliente?.CPF_CNPJ ?? 0, somentePosIntegracao, (TipoDocumentoRetorno)tipoDocumentoRetorno, (int)inicio, (int)limite, codigoTipoOperacao, situacao, ref mensagem, unitOfWork, configuracaoWS),
                            NumeroTotalDeRegistro = serWSCTe.ContarCTesAlteradosPeriodo(_dataInicial, _dataFinal, integradora.Empresa?.Codigo ?? 0, integradora.Cliente?.CPF_CNPJ ?? 0, somentePosIntegracao, codigoTipoOperacao, situacao, unitOfWork, configuracaoWS)
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

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo AdicionarArquivoTransacao(string jsonRequisicao, string jsonRetorno, DateTime data, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Data = data,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        private void AdicionarArquivoTransacaoIntegracaoPagamento(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao, string mensagem, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, pagamentoIntegracao.DataIntegracao, mensagem, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (pagamentoIntegracao.ArquivosTransacao == null)
                pagamentoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            pagamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCTe;
        }

        #endregion
    }
}
