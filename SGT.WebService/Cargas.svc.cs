using CoreWCF;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Carga;
using Dominio.ObjetosDeValor.WebService.Rest;
using System.Diagnostics;
using System.Text;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Cargas(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), ICargas
    {
        #region Métodos de Consulta

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> BuscaTiposDeOperacaoPorCNPJ(string cnpj)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>>.CreateFrom(new Servicos.WebService.Carga.TipoOperacao(unitOfWork).BuscarTiposOperacaoPorCNPJ(cnpj));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> BuscaTiposDeOperacao(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>>();
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>();
                    Servicos.WebService.Carga.TipoOperacao serWSTipoOperacao = new Servicos.WebService.Carga.TipoOperacao(Conexao.createInstance(_serviceProvider).StringConexao);
                    Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao()
                    {
                        Ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo
                    };
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        DirecaoOrdenar = "desc",
                        InicioRegistros = (int)inicio,
                        LimiteRegistros = (int)limite,
                        PropriedadeOrdenar = "Codigo",
                    };
                    List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposDeOperacao = repTipoOperacao.Consultar(filtrosPesquisa, parametrosConsulta);
                    retorno.Objeto.NumeroTotalDeRegistro = repTipoOperacao.ContarConsulta(filtrosPesquisa);
                    retorno.Objeto.Itens = serWSTipoOperacao.RetornarTiposDeOperacao(tiposDeOperacao);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou tipos de operação", unitOfWork);
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os tipos de Operação";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>> BuscarModelosVeiculares(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>>();
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.WebService.Carga.ModeloVeicularCarga serWSModeloVeicularCarga = new Servicos.WebService.Carga.ModeloVeicularCarga(unitOfWork);

            try
            {
                if (limite <= 100)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga> tiposModelo = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga>();
                    tiposModelo.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral);
                    tiposModelo.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Reboque);
                    tiposModelo.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>();
                    Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
                    Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> ModelosVeicularCarga = repModeloVeicularCarga.Consultar(null, "", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, tiposModelo, null, new List<int>(), "Codigo", "desc", (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repModeloVeicularCarga.ContarConsulta(null, "", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, tiposModelo, null, new List<int>());
                    retorno.Objeto.Itens = serWSModeloVeicularCarga.RetornarModelosVeiculares(ModelosVeicularCarga);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou modelos veiculares", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os modelos veículares";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>> BuscarTiposDeCargaEmbarcador(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>>();
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>();
                    Servicos.WebService.Carga.TipoCarga serWSTipoCarga = new Servicos.WebService.Carga.TipoCarga(unitOfWork);

                    Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCarga = repTipoDeCarga.Consultar("", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, 0, null, null, null, 0, "Codigo", "desc", (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repTipoDeCarga.ContarConsulta("", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, 0, null, null, null, 0);
                    retorno.Objeto.Itens = serWSTipoCarga.RetornarTipoCargaEmbarcador(tiposCarga);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou tipos de carga", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os tipos de carga";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>> BuscarTiposDeCargaEmbarcadorPorCNPJ(string cnpj)
        {
            ValidarToken();

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>>();
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
                Servicos.WebService.Carga.TipoCarga serWSTipoCarga = new Servicos.WebService.Carga.TipoCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cnpj)));
                if (cliente != null)
                {
                    retorno.Objeto = serWSTipoCarga.RetornarTipoCargaEmbarcador(repTipoCarga.BuscarPorCliente(cliente));
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou tipos de carga por CNPJ", unitOfWork);
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O CNPJ " + cnpj + " não existe na base multisoftware";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os tipos de carga por CNPJ";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao>> BuscarSituacaoCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarSituacaoCarga(protocolo));
            });
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCargaService(protocolo));
            });
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao>> BuscarCargasAgrupadasAguardandoIntegracao()
        {
            ValidarToken();
            Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao>>();
            retorno.Status = true;
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao> objetoRetorno = repCarga.BuscarCargasAgrupadasAguardandoIntegracao();
                retorno.Objeto = objetoRetorno;
                retorno.Status = true;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a cargas agrupadas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao>> BuscarCargasAgrupadasAguardandoIntegracaoPaginado(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao>>();
            retorno.Status = true;
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao> objetoRetorno = repCarga.BuscarCargasAgrupadasAguardandoIntegracao((int)inicio, (int)limite);
                retorno.Objeto = objetoRetorno;
                retorno.Status = true;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a cargas agrupadas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        /// <summary>
        /// Confirma que o Integrador já viu quais cargas foram agrupadas (usado o método acima BuscarCargasAgrupadas()
        /// </summary>
        /// <param name="ListaProtocoloCarga"></param>
        /// <returns></returns>
        public Retorno<bool> ConfirmarIntegracaoCargasAgrupadas(List<int> ListaProtocoloCarga)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupadas = repCarga.BuscarPorProtocolos(ListaProtocoloCarga);
                List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao> objeto = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao>();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada in cargasAgrupadas)
                {
                    if (cargaAgrupada.AgIntegracaoAgrupamentoCarga)
                    {
                        cargaAgrupada.AgIntegracaoAgrupamentoCarga = false;
                        repCarga.Atualizar(cargaAgrupada);
                    }
                }

                retorno.Objeto = true;
                retorno.Status = true;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a visualização de cargas agrupadas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCargaPorCodigosIntegracao(Dominio.ObjetosDeValor.WebService.Carga.CodigosIntegracao codigosIntegracao)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
            Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>();
            retorno.Status = true;

            try
            {
                if (codigosIntegracao != null)
                {
                    if (!string.IsNullOrWhiteSpace(codigosIntegracao.NumeroCarga) || !string.IsNullOrWhiteSpace(codigosIntegracao.NumeroPedidoEmbarcador))
                    {
                        string mensagem = "";
                        retorno.Objeto = serWSCarga.BuscarCarga(codigosIntegracao.NumeroCarga, codigosIntegracao.NumeroPedidoEmbarcador, codigosIntegracao.CodigoIntegracaoFilial, ref mensagem);
                        if (!string.IsNullOrWhiteSpace(mensagem))
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = mensagem;
                        }
                        else
                            Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou carga por códigos de integração", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Por favor, informe os códigos de integração";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "É obrigatório informar o código de integração";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(codigosIntegracao.NumeroCarga + " - " + codigosIntegracao.NumeroPedidoEmbarcador);
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.Resumo.Carga> BuscarResumoCargaPorCodigosIntegracao(Dominio.ObjetosDeValor.WebService.Carga.CodigosIntegracao codigosIntegracao)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
            Retorno<Dominio.ObjetosDeValor.WebService.Carga.Resumo.Carga> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Carga.Resumo.Carga>();
            retorno.Status = true;

            try
            {
                if (codigosIntegracao != null)
                {
                    if (!string.IsNullOrWhiteSpace(codigosIntegracao.CodigoIntegracaoFilial))
                    {
                        if (!string.IsNullOrWhiteSpace(codigosIntegracao.NumeroCarga) || !string.IsNullOrWhiteSpace(codigosIntegracao.NumeroPedidoEmbarcador))
                        {
                            string mensagem = "";

                            retorno.Objeto = serWSCarga.BuscarResumoCarga(codigosIntegracao.NumeroCarga, codigosIntegracao.NumeroPedidoEmbarcador, codigosIntegracao.CodigoIntegracaoFilial, ref mensagem);

                            if (!string.IsNullOrWhiteSpace(mensagem))
                            {
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                retorno.Mensagem = mensagem;
                            }
                            else
                                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou carga por códigos de integração", unitOfWork);
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Por favor, informe os códigos de integração carga/pedido";
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "É obrigatório informar o código de integração da filial";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "É obrigatório informar o código de integração";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>> BuscarCargaPedidoDestinado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>>() { Status = true };

            try
            {
                var integradora = ValidarToken(carregarClientes: true);

                if (integradora.Clientes.Count > 0)
                {
                    Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
                    retorno.Objeto = serWSCarga.BuscarCargasPorRecebedor((from obj in integradora.Clientes select obj.CPF_CNPJ).ToList(), unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O integrador informado não está apto a obter pedidos destinados";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.ValePedagio>> BuscarDetalhesValePedagio(int protocoloCarga)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca repositorioValePedagioPraca = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                List<int> protocolosCargas = new List<int>() { protocoloCarga };

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

                if (configuracaoWebService.RetornarDadosRedespachoTransbordoComInformacoesCargaOrigemConsultada)
                {
                    Repositorio.Embarcador.Cargas.Transbordo repositorioTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
                    Repositorio.Embarcador.Cargas.Redespacho repositorioRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                    List<int> listaCodigoProtocolo = repositorioCargaPedido.BuscarListaPedidoCargasPorProtocolo(protocoloCarga);

                    if (listaCodigoProtocolo.Count > 0)
                    {
                        List<int> protocolosTransbordos = repositorioTransbordo.BuscarPorProtocoloIntegracaoCargaOrigem(listaCodigoProtocolo);
                        List<int> protocolosRedespachos = repositorioRedespacho.BuscarPorProtocoloIntegracaoCargaOrigem(listaCodigoProtocolo);

                        protocolosCargas.AddRange(protocolosTransbordos);
                        protocolosCargas.AddRange(protocolosRedespachos);
                    }

                }

                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca> listaPracas = repositorioValePedagioPraca.BuscarPorProtocoloCarga(protocolosCargas);
                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaValePedagio = repositorioValePedagio.BuscarPorProtocoloCarga(protocolosCargas);

                List<Dominio.ObjetosDeValor.WebService.Carga.ValePedagio> listaValePedagioRetornar = new List<Dominio.ObjetosDeValor.WebService.Carga.ValePedagio>();

                foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio valePedagio in listaValePedagio)
                {
                    Dominio.ObjetosDeValor.WebService.Carga.ValePedagio valePedagioRetornar = new Dominio.ObjetosDeValor.WebService.Carga.ValePedagio()
                    {
                        NumeroValePedagio = valePedagio.NumeroValePedagio,
                        CompraComEixosSuspensos = valePedagio.CompraComEixosSuspensos,
                        QuantidadeEixos = valePedagio.QuantidadeEixos,
                        ProtocoloCarga = valePedagio.Carga.Protocolo,
                        ValorTotalValePedagio = valePedagio.ValorValePedagio,
                        TipoIntegradora = valePedagio.TipoIntegracao?.Tipo ?? TipoIntegracao.NaoInformada,
                        PracasValePedagio = new List<Dominio.ObjetosDeValor.WebService.Carga.PracaValePedagio>()
                    };

                    foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca praca in (from p in listaPracas where p.CargaValePedagioDadosCompra.CodigoViagem == valePedagio.NumeroValePedagio.ToInt() select p))
                    {
                        valePedagioRetornar.PracasValePedagio.Add(new Dominio.ObjetosDeValor.WebService.Carga.PracaValePedagio()
                        {
                            CodigoPraca = praca.CodigoPraca,
                            ConcessionariaCodigo = praca.ConcessionariaCodigo,
                            ConcessionariaNome = praca.ConcessionariaNome,
                            NomePraca = praca.NomePraca,
                            NomeRodovia = praca.NomeRodovia,
                            Valor = praca.Valor
                        });
                    }

                    listaValePedagioRetornar.Add(valePedagioRetornar);
                }

                Paginacao<Dominio.ObjetosDeValor.WebService.Carga.ValePedagio> paginacaoOcorrenciasColetaEntrega = new Paginacao<Dominio.ObjetosDeValor.WebService.Carga.ValePedagio>()
                {
                    NumeroTotalDeRegistro = listaValePedagioRetornar.Count,
                    Itens = listaValePedagioRetornar
                };

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.ValePedagio>>.CriarRetornoSucesso(paginacaoOcorrenciasColetaEntrega);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.ValePedagio>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os detalhes do vale pedágio da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> DisponibilizarCargaPedidosDestinadoParaSeparacao(List<Dominio.ObjetosDeValor.WebService.Carga.Pedido> pedidos)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                unitOfWork.Start();

                foreach (Dominio.ObjetosDeValor.WebService.Carga.Pedido pedido in pedidos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoDisponibilizar = repositorioPedido.BuscarPorCodigo(pedido.Protocolo);

                    if (pedidoDisponibilizar == null)
                        throw new WebServiceException($"O pedido informado ({pedido.Protocolo}) não existe no Multi Embarcador");

                    pedidoDisponibilizar.DisponivelParaSeparacao = true;
                    pedidoDisponibilizar.PalletAgrupamento = pedido.PalletAgrupamento;

                    repositorioPedido.Atualizar(pedidoDisponibilizar);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoDisponibilizar, "Liberou o pedido para separação.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao disponibilizar o pedido para separação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarIntegracaoCargaPedidoDestinado(int protocolo)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(protocolo);

                if (cargaPedido != null)
                {
                    if (!cargaPedido.CargaPedidoIntegrada)
                    {

                        cargaPedido.CargaPedidoIntegrada = true;
                        repCargaPedido.Atualizar(cargaPedido);
                        retorno.Objeto = true;
                        retorno.Status = true;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Pedido, "Confirmou integração do pedido.", unitOfWork);

                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                        retorno.Mensagem = "O pedido já teve sua confirmação de integração realizada anteriormente";
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O pedido informada não existe no Multi Embarcador";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao confirmar";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Documentos.Protocolos>> BuscarDocumentacoesPendentesIntegracao(int? inicio, int? limite)
        {
            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Documentos.Protocolos>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Documentos.Protocolos>>();
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            try
            {
                Repositorio.Embarcador.Documentos.TrackingDocumentacao repTrackingDocumentacao = new Repositorio.Embarcador.Documentos.TrackingDocumentacao(unitOfWork);

                if (limite <= 1000)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Documentos.Protocolos>();
                    Servicos.WebService.Carga.TipoCarga serWSTipoCarga = new Servicos.WebService.Carga.TipoCarga(unitOfWork);
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Documentos.Protocolos>();

                    List<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao> trakings = repTrackingDocumentacao.BuscarDocumentacoesAgIntegracao((int)inicio, (int)limite);
                    foreach (Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao traking in trakings)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Documentos.Protocolos protocolo = new Dominio.ObjetosDeValor.Embarcador.Documentos.Protocolos();
                        protocolo.Protocolo = traking.Codigo;
                        retorno.Objeto.Itens.Add(protocolo);
                    }

                    retorno.Objeto.NumeroTotalDeRegistro = repTrackingDocumentacao.ContarDocumentacoesAgIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou documentações pendentes de integração", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 1000";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as documentações pendentes integração";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao>> BuscarDocumentacao(Dominio.ObjetosDeValor.Embarcador.Documentos.Protocolos protocolo)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao>>();
            retorno.Status = true;
            try
            {
                if (protocolo != null)
                {
                    if (protocolo.Protocolo > 0)
                    {
                        string mensagem = "";
                        retorno.Objeto = serWSCarga.BuscarDocumentacao(protocolo.Protocolo, ref mensagem, Auditado);
                        if (!string.IsNullOrWhiteSpace(mensagem))
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = mensagem;
                        }
                        else
                            Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou documentacação", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Por favor, informe os códigos de integração";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "É obrigatório informar o protocolo de integração";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Documentação";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarCargasPendentesIntegracao(int? inicio, int? limite, string codigoIntegracaoTipoOperacao, string codigoIntegracaoFilial)
        {
            return ProcessarRequisicaoAsync<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CreateFrom(await new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado).BuscarCargasPendentesIntegracaoAsync(new RequestCargasIntegracaoPendentes() { Inicio = inicio ?? 0, Limite = limite ?? 0, CodigoIntegracaoFilial = codigoIntegracaoFilial, CodigoIntegracaoTipoOperacao = codigoIntegracaoTipoOperacao }, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CreateFrom(retorno.Objeto)
                };
            }).GetAwaiter().GetResult();
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarCargasRedespachoPendenteIntegracao(int? inicio, int? limite)
        {
            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>();
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                if (limite <= 1000)
                {
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();
                    Servicos.WebService.Carga.TipoCarga serWSTipoCarga = new Servicos.WebService.Carga.TipoCarga(unitOfWork);
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();

                    DateTime dataFinalizacaoEmissao = DateTime.Now.AddHours(-configuracao.TempoHorasParaRetornoCTeAposFinalizacaoEmissao);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos;
                    if (!configuracao.BuscarPorCargaPedidoCargasPendentesIntegracao)
                        cargaPedidos = repCargaPedido.BuscarCargasRedespachoAgIntegracaoEmbarcador(configuracao.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais, dataFinalizacaoEmissao, integradora?.Codigo ?? 0, integradora?.GrupoPessoas?.Codigo ?? 0, (int)inicio, (int)limite);
                    else
                        cargaPedidos = repCargaPedido.BuscarCargasPedidoRedespachoAgIntegracaoEmbarcador(configuracao.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais, dataFinalizacaoEmissao, integradora?.Codigo ?? 0, integradora?.GrupoPessoas?.Codigo ?? 0, (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos();
                        protocolo.protocoloIntegracaoCarga = cargaPedido.Carga.Protocolo;
                        protocolo.protocoloIntegracaoPedido = cargaPedido.Pedido.Protocolo;
                        retorno.Objeto.Itens.Add(protocolo);
                    }

                    if (!configuracao.BuscarPorCargaPedidoCargasPendentesIntegracao)
                        retorno.Objeto.NumeroTotalDeRegistro = repCargaPedido.ContarCargasRedespachoAgIntegracaoEmbarcador(configuracao.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais, dataFinalizacaoEmissao, integradora?.Codigo ?? 0, integradora?.GrupoPessoas?.Codigo ?? 0);
                    else
                        retorno.Objeto.NumeroTotalDeRegistro = repCargaPedido.ContarCargasPedidoRedespachoAgIntegracaoEmbarcador(configuracao.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais, dataFinalizacaoEmissao, integradora?.Codigo ?? 0, integradora?.GrupoPessoas?.Codigo ?? 0);

                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou cargas pendentes de integração", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 1000";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as cargas pendentes integração";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentoEmMontagemPendenteIntegracao(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>();
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                    Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repositorioBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(unitOfWork);

                    Servicos.WebService.Empresa.Empresa servicoEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);
                    Servicos.WebService.Filial.Filial servicoFilial = new Servicos.WebService.Filial.Filial(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>();

                    List<int> codigosCarregamentos = repositorioCarregamento.BuscarCodigosCarregamentosPendenteIntegracao((int)inicio, (int)limite, configuracaoWebService.RetornarApenasCarregamentosPendentesComTransportadora);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedido = repositorioCarregamentoPedido.BuscarPorCarregamentosSemFetch(codigosCarregamentos);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocosCarregamento = repositorioBlocoCarregamento.BuscarPorPedidos(carregamentosPedido.Select(o => o.Pedido.Codigo).ToList());
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> carregamentoRoteirizacaoClientesRota = repCarregamentoRoteirizacaoClientesRota.BuscarPorCarregamentos(codigosCarregamentos);


                    foreach (int codigoCarregamento in codigosCarregamentos)
                    {
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repositorioCarregamento.BuscarPorCodigo(codigoCarregamento);
                        Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento objCarregamento = new Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento();

                        objCarregamento.NumeroCarregamento = carregamento.NumeroCarregamento;
                        objCarregamento.ProtocoloIntegracaoCarga = carregamento.Codigo;
                        objCarregamento.ProtocoloPreCarga = carregamento.PreCarga?.Codigo.ToString();
                        objCarregamento.ProtocoloCarregamento = carregamento.Codigo;
                        objCarregamento.CodigoIntegracaoFilial = carregamento.Filial?.CodigoFilialEmbarcador;
                        objCarregamento.Filial = servicoFilial.ConverterObjetoFilial(carregamento.Filial);
                        objCarregamento.CodigoIntegracaoTipoOperacao = carregamento.TipoOperacao?.CodigoIntegracao;
                        objCarregamento.TransportadoraEmitente = servicoEmpresa.ConverterObjetoEmpresa(carregamento.Empresa);

                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedido = (from obj in carregamentosPedido where obj.Carregamento.Codigo == carregamento.Codigo select obj).ToList();

                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> carregamentoRotaFreteCliente = (from obj in carregamentoRoteirizacaoClientesRota where obj.CarregamentoRoteirizacao.Carregamento.Codigo == carregamento.Codigo && !obj.Coleta select obj).ToList();

                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidoOrdenada = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

                        if (carregamentoRotaFreteCliente.Count > 1 && carregamento.Recebedor == null)
                        {
                            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota carregamentoRotaFrete in carregamentoRotaFreteCliente)
                                carregamentoPedidoOrdenada.AddRange((from obj in carregamentoPedido where obj.Pedido.ObterDestinatario().CPF_CNPJ == carregamentoRotaFrete.Cliente.CPF_CNPJ select obj).ToList());
                        }
                        else
                            carregamentoPedidoOrdenada = carregamentoPedido;

                        objCarregamento.ProtocolosPedidos = carregamentoPedidoOrdenada.Select(a => a.Pedido.Protocolo).Distinct().ToList();
                        objCarregamento.BlocosPedidosCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.BlocosPedidosCarregamento>();


                        foreach (int protocoloPedido in objCarregamento.ProtocolosPedidos)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Pedido.BlocosPedidosCarregamento blocosPedidosCarregamentoInsercao = new Dominio.ObjetosDeValor.Embarcador.Pedido.BlocosPedidosCarregamento()
                            {
                                ProtocoloPedido = protocoloPedido,
                                BlocosCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.BlocoCarregamento>()
                            };

                            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento blocoCarregamento in blocosCarregamento.Where(o => o.Pedido.Codigo == protocoloPedido))
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.BlocoCarregamento blocoInsercao = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.BlocoCarregamento()
                                {
                                    Codigo = blocoCarregamento.Codigo,
                                    Bloco = blocoCarregamento.Bloco
                                };

                                blocosPedidosCarregamentoInsercao.BlocosCarregamento.Add(blocoInsercao);
                            }

                            objCarregamento.BlocosPedidosCarregamento.Add(blocosPedidosCarregamentoInsercao);
                        }

                        retorno.Objeto.Itens.Add(objCarregamento);
                    }

                    retorno.Objeto.NumeroTotalDeRegistro = codigosCarregamentos.Count();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou carregamentos pendentes de integração", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as cargas pendentes integração";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentosPendentesIntegracao(int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado).BuscarCarregamentosPendentesIntegracao(new RequestCarregamentosPendentesIntegracao() { Inicio = inicio ?? 0, Limite = limite ?? 0 }, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentosPendentesIntegracaoPorFilial(int? inicio, int? limite, string codigoFilial)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado).BuscarCarregamentosPendentesIntegracao(new RequestCarregamentosPendentesIntegracao() { Inicio = inicio ?? 0, Limite = limite ?? 0, CodigoFilial = codigoFilial }, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentosPendentesIntegracaoV2(Dominio.ObjetosDeValor.WebService.Carga.RequestCarregamentosPendentesIntegracao requestCarregamentosPendentesIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado).BuscarCarregamentosPendentesIntegracao(requestCarregamentosPendentesIntegracao, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoPreCarga> BuscarPreCarga(int protocoloPreCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repositorioPreCarga.BuscarPorCodigo(protocoloPreCarga);

                if (preCarga == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoPreCarga>.CriarRetornoDadosInvalidos("A pré carga informada não foi encontrada");

                Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoPreCarga preCargaRetornar = new Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoPreCarga()
                {
                    NumeroPreCarga = preCarga.NumeroPreCarga,
                    Motoristas = preCarga.Motoristas?.Count > 0 ? new Servicos.WebService.Empresa.Motorista(unitOfWork).ConverterListaObjetoMotorista(preCarga.Motoristas) : null,
                    Transportador = preCarga.Empresa != null ? new Servicos.WebService.Empresa.Empresa(unitOfWork).ConverterObjetoEmpresa(preCarga.Empresa) : null,
                    Veiculo = preCarga.Veiculo != null ? new Servicos.WebService.Frota.Veiculo(unitOfWork).ConverterObjetoVeiculo(preCarga.Veiculo, unitOfWork) : null
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou pré carga", unitOfWork);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoPreCarga>.CriarRetornoSucesso(preCargaRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoPreCarga>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as cargas pendentes integração");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<int>> BuscarCargasAguardandoConfirmacaoCancelamento(int? inicio, int? limite)
        {
            Retorno<Paginacao<int>> retorno = new Retorno<Paginacao<int>>();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite <= 100)
                {
                    Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCargaCancelamento.BuscarPorCargaPendentesConfirmacao(integradora?.GrupoPessoas?.Codigo ?? 0, (int)inicio, (int)limite);
                    retorno.Objeto = new Paginacao<int>();
                    retorno.Objeto.NumeroTotalDeRegistro = repCargaCancelamento.ContarPorCargaPendentesConfirmacao(integradora?.GrupoPessoas?.Codigo ?? 0);
                    retorno.Objeto.Itens = new List<int>();
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                    {
                        retorno.Objeto.Itens.Add(carga.Protocolo /*carga.Codigo*/);
                    }
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou cargas aguardando confirmação de cancelamento", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar a carga";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento> ConsultarDetalhesCancelamentoDaCarga(int protocoloIntegracaoCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConsultarDetalhesCancelamentoDaCarga(protocoloIntegracaoCarga));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento> ConsultarDetalhesCancelamentoDocumentosDaCarga(int protocoloIntegracaoCarga)
        {
            ValidarToken();
            Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento>();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                //Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCargaDuplicada(protocoloIntegracaoCarga);
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCargaDuplicadaPorProtocolo(protocoloIntegracaoCarga);
                if (cargaCancelamento != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCancelamento.Carga.Codigo);

                    retorno.Status = true;
                    retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento();
                    retorno.Objeto.NumeroOS = cargaPedido?.Pedido?.NumeroOS ?? "";
                    retorno.Objeto.CodigoIntegracaoOperador = cargaCancelamento.UsuarioSolicitouCancelamento?.CodigoIntegracao ?? (cargaCancelamento.Usuario != null ? cargaCancelamento.Usuario.CodigoIntegracao : "");
                    retorno.Objeto.DescricaoTipoOperacao = cargaCancelamento.Carga.TipoOperacao?.Descricao;
                    retorno.Objeto.StatusCustoExtra = cargaCancelamento.Carga.TipoOperacao?.ConfiguracaoCarga?.DirecionamentoCustoExtra.ObterDescricao() ?? TipoDirecionamentoCustoExtra.Nenhum.ObterDescricao();
                    retorno.Objeto.DataCancelamento = cargaCancelamento.DataCancelamento != null ? cargaCancelamento.DataCancelamento.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
                    retorno.Objeto.MotivoRejeicaoCancelamento = cargaCancelamento.MensagemRejeicaoCancelamento;
                    retorno.Objeto.MotivoCancelamento = cargaCancelamento.MotivoCancelamento;
                    retorno.Objeto.UsuarioMultisoftware = cargaCancelamento.Usuario != null ? cargaCancelamento.Usuario.Nome : "";
                    retorno.Objeto.UsuarioERP = cargaCancelamento.UsuarioSolicitouCancelamento?.Nome ?? (cargaCancelamento.Usuario != null ? cargaCancelamento.Usuario.Nome : "");

                    if (cargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento)
                        retorno.Objeto.SituacaoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.EmCancelamento;
                    else if (cargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.Cancelada)
                        retorno.Objeto.SituacaoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.Cancelada;
                    else
                        retorno.Objeto.SituacaoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.CancelamentoRejeitado;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou detalhes do cancelamento dos documentos da carga", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "A carga informada não foi cancelada";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a carga";
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pedido.RateioFreteProduto>> ConsultarRateioFreteProdutos(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            ValidarToken();
            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pedido.RateioFreteProduto>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pedido.RateioFreteProduto>>();
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto repRateioCargaPedidoCTeProduto = new Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete repRateioProdutoComponenteFrete = new Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);

                List<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto> listaRateioNotaCteProduto = repRateioCargaPedidoCTeProduto.BuscarPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);

                if ((from obj in listaRateioNotaCteProduto select obj.ValorValePedagioRateio).Sum() <= 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaValePedagio = repCargaIntegracaoValePedagio.BuscarPorProtocoloCarga(protocolo.protocoloIntegracaoCarga);

                    decimal valorValePedagio = (from obj in listaValePedagio
                                                where obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado
                                                select obj.ValorValePedagio).Sum();

                    if (valorValePedagio <= 0)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> listaCargaValePedagios = repCargaValePedagio.BuscarPorCarga(protocolo.protocoloIntegracaoCarga, TipoServicoMultisoftware);
                        valorValePedagio = (from obj in listaCargaValePedagios select obj.Valor).Sum();
                    }

                    if (valorValePedagio > 0)
                    {
                        Servicos.Embarcador.Carga.RateioProduto serRateioProduto = new Servicos.Embarcador.Carga.RateioProduto(unitOfWork);
                        serRateioProduto.RatearValePedagioPorCarga(protocolo.protocoloIntegracaoCarga, valorValePedagio, unitOfWork);
                    }

                    listaRateioNotaCteProduto = repRateioCargaPedidoCTeProduto.BuscarPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);
                }



                if (listaRateioNotaCteProduto != null && listaRateioNotaCteProduto.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorProtocoloCarga(protocolo.protocoloIntegracaoCarga);

                    decimal valorTotalItens = (from obj in cargaPedidoProdutos select (obj.ValorUnitarioProduto * obj.Quantidade)).Sum();
                    decimal pesoTotalItens = (from obj in cargaPedidoProdutos select ((obj.PesoUnitario * obj.Quantidade) + obj.PesoTotalEmbalagem)).Sum();

                    retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.RateioFreteProduto>();

                    int countRegistro = 0;
                    //decimal somaRateioValePedagio = 0;
                    foreach (Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto rateio in listaRateioNotaCteProduto)
                    {
                        countRegistro += 1;

                        Dominio.ObjetosDeValor.Embarcador.Pedido.RateioFreteProduto objRateio = new Dominio.ObjetosDeValor.Embarcador.Pedido.RateioFreteProduto();

                        objRateio.CodigoProduto = rateio.ProdutoEmbarcador.CodigoProdutoEmbarcador;
                        objRateio.DescricaoProduto = rateio.ProdutoEmbarcador.Descricao;
                        objRateio.NumeroPedido = rateio.CargaPedido.Pedido.NumeroPedidoEmbarcador;
                        objRateio.NumeroCarga = rateio.CargaPedido.Pedido.CodigoCargaEmbarcador;
                        objRateio.ValorRateado = rateio.ValorTotalRateio;
                        objRateio.ValorRateadoFreteLiquido = rateio.ValorFreteRateio;
                        objRateio.NumeroOcorrencia = rateio.CargaCTeComplementoInfo?.CargaOcorrencia.NumeroOcorrencia.ToString() ?? rateio.CargaDocumentoParaEmissaoNFSManual?.CargaCTeComplementoInfo?.CargaOcorrencia?.NumeroOcorrencia.ToString() ?? "";
                        objRateio.ValorRateadoICMS = rateio.ValorICMS;
                        objRateio.ChaveNFe = rateio.CargaPedido.NotasFiscais != null && rateio.CargaPedido.NotasFiscais.Count > 0 ? rateio.CargaPedido.NotasFiscais.FirstOrDefault().XMLNotaFiscal.Chave : string.Empty;

                        List<Dominio.Entidades.Embarcador.Rateio.RateioProdutoComponenteFrete> listaComponenteRateio = repRateioProdutoComponenteFrete.BuscarPorRateioCargaProduto(rateio.Codigo);
                        if (listaComponenteRateio != null && listaComponenteRateio.Count > 0)
                            objRateio.ListaRateioComponentes = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.RateioFreteProdutoComponentes>();

                        foreach (Dominio.Entidades.Embarcador.Rateio.RateioProdutoComponenteFrete componenteRateio in listaComponenteRateio)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Pedido.RateioFreteProdutoComponentes objRateioComponente = new Dominio.ObjetosDeValor.Embarcador.Pedido.RateioFreteProdutoComponentes();
                            objRateioComponente.CodigoIntegracaoComponente = componenteRateio.ComponenteFrete?.CodigoIntegracao;
                            objRateioComponente.DescricaoComponente = componenteRateio.ComponenteFrete?.Descricao;
                            objRateioComponente.ValorRateadoComponete = componenteRateio.ValorComponente;

                            objRateio.ListaRateioComponentes.Add(objRateioComponente);
                        }

                        objRateio.ValorRateadoValePedagio = rateio.ValorValePedagioRateio;

                        //Diferença do rateio de valor Vale Pedágio
                        //objRateio.ValorRateadoValePedagio = RatearValorValePedagio(valorValePedagio, valorTotalItens, pesoTotalItens, rateio.ValorUnitarioProduto * rateio.Quantidade, rateio.Peso, rateio.CargaPedido.Carga.TipoRateioProdutos);
                        //somaRateioValePedagio += objRateio.ValorRateadoValePedagio;
                        //if (countRegistro >= listaRateioNotaCteProduto.Count())
                        //{
                        //    decimal diferenca = valorValePedagio - somaRateioValePedagio;
                        //    objRateio.ValorRateadoValePedagio += diferenca;
                        //}

                        retorno.Objeto.Add(objRateio);
                    }

                    retorno.Status = true;
                }
                else
                {
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                    retorno.Mensagem = "Carga não possui rateio de produtos disponível.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar rateio dos produtos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>> BuscarCargasPorTransportador()
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCargasPorTransportador(integradora.Empresa, null));
            });
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>> BuscarCargasPorTransportadorV2(bool? naoRetornarCargasComplementares)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCargasPorTransportador(integradora.Empresa, naoRetornarCargasComplementares));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.Carga> BuscarCargaPorTransportador(int protocoloCarga)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (integradora.Empresa == null)
                    return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Carga>.CriarRetornoDadosInvalidos("Dados inválidos para esta integração.");

                Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);

                Dominio.ObjetosDeValor.WebService.Carga.Carga carga = serWSCarga.BuscarCargaPorTransportador(protocoloCarga, integradora.Empresa, unitOfWork);

                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Carga>.CriarRetornoSucesso(carga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Carga>.CriarRetornoExcecao("Ocorreu uma falha ao consultar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> ConfirmarIntegracaoCargaTransportador(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<string> retorno = new Retorno<string>() { Status = true };

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (integradora.Empresa == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados inválidos para esta integração.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocolo, true);

                if (!carga.Empresa.CNPJ.StartsWith(integradora.Empresa.CNPJ.Left(8)))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Protocolo inválido para este transportador.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                unitOfWork.Start();

                if (carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos)
                    carga.IntegrouTerceiroTransportador = true;
                else carga.IntegrouTransportador = true;

                repCarga.Atualizar(carga, Auditado);

                unitOfWork.CommitChanges();

                retorno.Objeto = "Confirmação realizada com sucesso.";
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da carga.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> AlterarQuantidadeContainerBooking(int novaQuantidade, string numeroBooking)
        {
            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();
            StringBuilder stMensagem = new StringBuilder();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorNumeroBooking(numeroBooking);
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    pedido.QuantidadeContainerBooking = novaQuantidade;

                    repPedido.Atualizar(pedido);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, "Alterou a quantidade de container via integração.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                retorno.Status = true;
                retorno.Objeto = true;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                retorno.Mensagem = "Quantidade de container alterada com sucesso.";
                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao alterar a quantidade de container.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> SalvarNavioViagemDirecao(Dominio.ObjetosDeValor.Embarcador.Carga.Viagem viagem)
        {
            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();
            StringBuilder stMensagem = new StringBuilder();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagemSalvar = null;
                Dominio.Entidades.Embarcador.Pedidos.Navio navio = null;
                viagemSalvar = repPedidoViagemNavio.BuscarPorCodigoIntegracao(viagem.CodigoIntegracao.ToString("D"));

                if (viagem == null && (viagem.CodigoIntegracao <= 0 || string.IsNullOrWhiteSpace(viagem.Descricao) || viagem.Navio == null || viagem.NumeroViagem <= 0))
                {
                    stMensagem.Append("Dados obrigatórios para o preenchimento da viagem não foram informados. ");
                }
                if (viagem.Navio != null)
                {
                    if (string.IsNullOrWhiteSpace(viagem.Navio.CodigoIntegracao))
                        stMensagem.Append("Não foi informado o código de integração do Navio. ");
                    else
                    {
                        navio = repNavio.BuscarPorCodigoIntegracao(viagem.Navio.CodigoIntegracao);
                        if (navio == null && (string.IsNullOrWhiteSpace(viagem.Navio.Descricao) || string.IsNullOrWhiteSpace(viagem.Navio.CodigoIRIN) || string.IsNullOrWhiteSpace(viagem.Navio.CodigoEmbarcacao)))
                        {
                            stMensagem.Append("Dados obrigatórios para o preenchimento do navio informado na viagem não foram informados. ");
                        }
                    }
                }

                if (stMensagem.Length > 0)
                {
                    retorno.Status = false;
                    retorno.Objeto = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = stMensagem.ToString();
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                unitOfWork.Start();

                Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
                viagemSalvar = serPedidoWS.SalvarViagem(viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);

                if (stMensagem.Length > 0)
                {
                    unitOfWork.Rollback();
                    retorno.Status = false;
                    retorno.Objeto = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = stMensagem.ToString();
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }
                else if (viagemSalvar == null)
                {
                    unitOfWork.Rollback();
                    retorno.Status = false;
                    retorno.Objeto = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Viagem não localizada.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }
                else
                {
                    unitOfWork.CommitChanges();
                    retorno.Status = true;
                    retorno.Objeto = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                    retorno.Mensagem = "Viagem informada com sucesso.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao salvar os dados da viagem.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.CargaCancelamento.CargaCancelamento>> BuscarCargasCanceladasPorTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<List<Dominio.ObjetosDeValor.WebService.CargaCancelamento.CargaCancelamento>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.CargaCancelamento.CargaCancelamento>>() { Status = true };

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (integradora.Empresa == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados inválidos para esta integração.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);

                retorno.Objeto = serWSCarga.BuscarCargasCanceladasPorTransportador(integradora.Empresa, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<string> ConfirmarIntegracaoCancelamentoTransportador(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<string> retorno = new Retorno<string>() { Status = true };

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (integradora.Empresa == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados inválidos para esta integração.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(protocolo, true);

                if (!cargaCancelamento.Carga.Empresa.CNPJ.StartsWith(integradora.Empresa.CNPJ.Left(8)))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Protocolo inválido para este transportador.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                unitOfWork.Start();

                cargaCancelamento.IntegrouTransportador = true;

                repCargaCancelamento.Atualizar(cargaCancelamento, Auditado);

                unitOfWork.CommitChanges();

                retorno.Objeto = "Confirmação realizada com sucesso.";
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da carga.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConsultarCargaPedido(int protocoloPedido)
        {
            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(protocoloPedido);

                if (pedido == null)
                {
                    retorno.Status = false;
                    retorno.Objeto = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Pedido não localizado pelo protocolo " + protocoloPedido.ToString();
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorPedido(protocoloPedido);
                if (listaCargaPedido == null || listaCargaPedido.Count == 0)
                {
                    retorno.Status = true;
                    retorno.Objeto = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                    retorno.Mensagem = "Pedido protocolo " + protocoloPedido.ToString() + " não está alocado em nanhuma carga.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }
                else
                {
                    retorno.Status = true;
                    retorno.Objeto = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                    retorno.Mensagem = "Pedido protocolo " + protocoloPedido.ToString() + " está alocado a carga " + listaCargaPedido.FirstOrDefault().Carga.CodigoCargaEmbarcador;
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da carga.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> InformarInicioViagemCarga(int protocoloCarga, string dataHora)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                DateTime? dataInicioViagem = dataHora.ToNullableDateTime();

                if (!dataInicioViagem.HasValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("A data não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(protocoloCarga);

                if (carga == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Carga não localizada pelo protocolo {protocoloCarga}");

                if (
                    carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                    carga.SituacaoCarga != SituacaoCarga.AgImpressaoDocumentos &&
                    carga.SituacaoCarga != SituacaoCarga.ProntoTransporte &&
                    carga.SituacaoCarga != SituacaoCarga.AgIntegracao
                )
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"A situação da carga ({carga.DescricaoSituacaoCarga}) não permite informar o início da viagem.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                bool inicioViagemInformado = carga.DataInicioViagem.HasValue;

                if (inicioViagemInformado && !configuracaoEmbarcador.PermitirAtualizarInicioViagem)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("A carga já possui a data de início de viagem informada.");

                unitOfWork.Start();

                if (inicioViagemInformado)
                {
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarInicioViagem(carga.Codigo, dataInicioViagem.Value, configuracaoEmbarcador, TipoServicoMultisoftware, Cliente, Auditado, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Atualizou o início da viagem.", unitOfWork);
                }
                else
                {
                    //Alterar app para mandar latitude/longitude
                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                    auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                    auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas;

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(carga.Codigo, dataInicioViagem.Value, OrigemSituacaoEntrega.WebService, null, configuracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, auditado, unitOfWork);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>() {
                        new Dominio.Entidades.Auditoria.HistoricoPropriedade("Protocolo Carga", "", protocoloCarga.ToString()),
                        new Dominio.Entidades.Auditoria.HistoricoPropriedade("Data Hora", "", dataHora),
                    };
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, alteracoes, "Informou o início da viagem.", unitOfWork);
                    repositorioCarga.Atualizar(carga);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("correu uma falha ao informar o início da viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> BuscarDiarioBordo(int protocoloCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                byte[] todosPdfDiarioBordoCompactado = servicoImpressao.ObterTodosPdfDiarioBordoCompactado(protocoloCarga);
                string todosPdfDiarioBordoBase64 = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, todosPdfDiarioBordoCompactado));

                return Retorno<string>.CriarRetornoSucesso(todosPdfDiarioBordoBase64);
            }
            catch (ServicoException excecao)
            {
                return Retorno<string>.CriarRetornoExcecao(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<string>.CriarRetornoExcecao("Ocorreu uma falha ao consultar o diário de bordo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> BuscarReciboValePedagio(int protocoloCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocoloCarga);
                if (carga == null)
                    return Retorno<string>.CriarRetornoDadosInvalidos($"Não foi localizada nenhuma carga para o protocolo informado ({protocoloCarga}).");

                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaIntegracaoValePedagio = repCargaValePedagio.BuscarPorCarga(carga.CargaAgrupamento != null ? carga.CargaAgrupamento.Codigo : carga.Codigo);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = (from o in listaIntegracaoValePedagio where o.SituacaoIntegracao == SituacaoIntegracao.Integrado && (o.SituacaoValePedagio == SituacaoValePedagio.Confirmada || o.SituacaoValePedagio == SituacaoValePedagio.Comprada) select o).FirstOrDefault();
                if (cargaIntegracaoValePedagio == null)
                    return Retorno<string>.CriarRetornoDadosInvalidos("Não foi encontrado nenhum Vale Pedágio para a carga protocolo " + protocoloCarga.ToString());

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                string pdfValePedagio = string.Empty;
                byte[] arquivo = null;

                string mensagemRetorno = servicoValePedagio.ObterArquivoValePedagio(cargaIntegracaoValePedagio, ref arquivo, TipoServicoMultisoftware);
                if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                    return Retorno<string>.CriarRetornoDadosInvalidos("PDF vale pedágio não disponível: " + mensagemRetorno);

                if (arquivo == null)
                    return Retorno<string>.CriarRetornoDadosInvalidos("Vale Pedágio está com pendência, PDF não disponível.");

                if (configuracao.UtilizarCodificacaoUTF8ConversaoPDF)
                    pdfValePedagio = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, arquivo));
                else
                    pdfValePedagio = Convert.ToBase64String(arquivo);

                return Retorno<string>.CriarRetornoSucesso(pdfValePedagio);
            }
            catch (ServicoException excecao)
            {
                return Retorno<string>.CriarRetornoExcecao(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<string>.CriarRetornoExcecao("Ocorreu uma falha ao consultar o recibo vale pedágio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<List<string>> BuscarListaReciboValePedagio(int protocoloCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocoloCarga);
                if (carga == null)
                    return Retorno<List<string>>.CriarRetornoDadosInvalidos($"Não foi localizada nenhuma carga para o protocolo informado ({protocoloCarga}).");

                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaIntegracaoValePedagio = repCargaValePedagio.BuscarPorCarga(carga.CargaAgrupamento != null ? carga.CargaAgrupamento.Codigo : carga.Codigo);

                if (listaIntegracaoValePedagio == null || listaIntegracaoValePedagio.Count <= 0)
                    return Retorno<List<string>>.CriarRetornoDadosInvalidos("Não foi encontrado nenhum Vale Pedágio para a carga protocolo " + protocoloCarga.ToString());

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                List<string> listaPDFs = new List<string>();

                foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio in listaIntegracaoValePedagio)
                {
                    string pdfValePedagio = string.Empty;
                    byte[] arquivo = null;

                    string mensagemRetorno = servicoValePedagio.ObterArquivoValePedagio(cargaIntegracaoValePedagio, ref arquivo, TipoServicoMultisoftware);
                    if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                        return Retorno<List<string>>.CriarRetornoDadosInvalidos("PDF vale pedágio não disponível: " + mensagemRetorno);

                    if (arquivo == null)
                        return Retorno<List<string>>.CriarRetornoDadosInvalidos("Vale Pedágio está com pendência, PDF não disponível.");

                    if (configuracao.UtilizarCodificacaoUTF8ConversaoPDF)
                        pdfValePedagio = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, arquivo));
                    else
                        pdfValePedagio = Convert.ToBase64String(arquivo);

                    listaPDFs.Add(pdfValePedagio);
                }

                return Retorno<List<string>>.CriarRetornoSucesso(listaPDFs);
            }
            catch (ServicoException excecao)
            {
                return Retorno<List<string>>.CriarRetornoExcecao(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<string>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar a lista de recibo de vale pedágio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> BuscarDocumentoCIOT(int protocoloCarga)
        {
            Retorno<RetornoCIOT> busca = BuscarDocumentoCiotPorProtocolo(protocoloCarga);
            return new Retorno<string>
            {
                CodigoMensagem = busca.CodigoMensagem,
                DataRetorno = busca.DataRetorno,
                Mensagem = busca.Mensagem,
                Objeto = busca.Objeto.Arquivo,
                Status = busca.Status,
            };
        }

        public Retorno<RetornoCIOT> BuscarCIOT(int protocoloCarga)
        {
            return BuscarDocumentoCiotPorProtocolo(protocoloCarga);
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoAlteracaoPedido>> ConsultarAlteracoesPedidosFinalizadas(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorioAlteracaoPedido = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(unitOfWork);
                int totalRegistros = repositorioAlteracaoPedido.ContarConsultaFinalizadas();
                List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoAlteracaoPedido> alteracoesPedidos = (totalRegistros > 0) ? repositorioAlteracaoPedido.ConsultarFinalizadas((int)inicio, (int)limite) : new List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoAlteracaoPedido>();

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoAlteracaoPedido>>.CriarRetornoSucesso(new Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoAlteracaoPedido>
                {
                    NumeroTotalDeRegistro = totalRegistros,
                    Itens = alteracoesPedidos
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoAlteracaoPedido>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as alterações de pedidos finalizadas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<int>> BuscarCargasComCanhotosDigitalizados(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (limite > 100)
                    return Retorno<Paginacao<int>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Paginacao<int> retorno = new Paginacao<int>();

                retorno.NumeroTotalDeRegistro = repositorioCarga.ContarProtocolosCargasComTodosCanhotosDigitalizados();
                retorno.Itens = (retorno.NumeroTotalDeRegistro > 0) ? repositorioCarga.BuscarProtocolosCargasComTodosCanhotosDigitalizados((int)inicio, (int)limite) : new List<int>();

                return Retorno<Paginacao<int>>.CriarRetornoSucesso(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<int>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as cargas com todos os canhotos digitalizados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> CriarFaixaTemperatura(string codigoIntegracao, string descricao, decimal? faixaInicial, decimal? faixaFinal)
        {
            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperaturaJaExistente = repFaixaTemperatura.BuscarPorCodigoIntegracao(codigoIntegracao);
                if (faixaTemperaturaJaExistente != null)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "Já existe uma faixa de temperatura com esse código de integração";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = new Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura();

                if (string.IsNullOrEmpty(descricao))
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.RegistroIndisponivel;
                    retorno.Status = false;
                    retorno.Mensagem = "A descrição não pode ser nula";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                if (string.IsNullOrEmpty(codigoIntegracao))
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.RegistroIndisponivel;
                    retorno.Status = false;
                    retorno.Mensagem = "o código de integração não pode ser nulo";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                faixaTemperatura.Descricao = descricao;
                faixaTemperatura.FaixaInicial = faixaInicial ?? 0m;
                faixaTemperatura.FaixaFinal = faixaFinal ?? 0m;
                faixaTemperatura.CodigoIntegracao = codigoIntegracao;
                faixaTemperatura.DataUltimaModificacao = DateTime.Now;

                repFaixaTemperatura.Inserir(faixaTemperatura);

                retorno.Objeto = true;
                retorno.Status = true;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao criar a faixa de temperatura";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        /// <summary>
        /// Atualiza todos os dados de uma faixa de temperatura.
        /// </summary>
        /// <param name="ListaProtocoloCarga"></param>
        /// <returns></returns>
        public Retorno<bool> AtualizarFaixaTemperatura(string codigoIntegracao, string descricao, decimal? faixaInicial, decimal? faixaFinal)
        {
            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = repFaixaTemperatura.BuscarPorCodigoIntegracao(codigoIntegracao);

                if (faixaTemperatura == null)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.RegistroIndisponivel;
                    retorno.Status = false;
                    retorno.Mensagem = "A faixa de temperatura com esse código de integração não existe";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                if (!string.IsNullOrEmpty(descricao))
                {
                    faixaTemperatura.Descricao = descricao;
                }

                if (faixaInicial.HasValue)
                {
                    faixaTemperatura.FaixaInicial = faixaInicial.Value;
                }

                if (faixaFinal.HasValue)
                {
                    faixaTemperatura.FaixaFinal = faixaFinal.Value;
                }

                faixaTemperatura.DataUltimaModificacao = DateTime.Now;

                repFaixaTemperatura.Atualizar(faixaTemperatura);

                retorno.Objeto = true;
                retorno.Status = true;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao atualizar a faixa de temperatura";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.EDINotfis>> BuscarEDINotfisCargas(string dataHoraInicio)
        {
            Dominio.Entidades.WebService.Integradora token = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.Empresa transportador = token.Empresa;

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Carga.EDINotfis> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Carga.EDINotfis>();
                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                DateTime? dataInicio = dataHoraInicio.ToNullableDateTime();

                if (transportador == null)
                    throw new WebServiceException("Esse token não está vinculado com nenhum transportador");

                if (!dataInicio.HasValue)
                    throw new WebServiceException("A data de início e data final deve ser enviada");


                List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> listaControleIntegracaoCargaEDI = repControleIntegracaoCargaEDI.BuscarPorTransportadorTipoLayoutEDIPeriodo(transportador.Codigo, Dominio.Enumeradores.TipoLayoutEDI.NOTFIS, dataInicio.Value, DateTime.Now);

                foreach (Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEdi in listaControleIntegracaoCargaEDI)
                {
                    controleIntegracaoCargaEdi.Integradora = token;
                    controleIntegracaoCargaEdi.DataConsumoArquivo = DateTime.Now;

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in controleIntegracaoCargaEdi.Cargas)
                    {


                        Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = Servicos.Embarcador.Carga.CargaIntegracaoEDI.ConverterCargaEmNotFis(carga, controleIntegracaoCargaEdi.LayoutEDI, unitOfWork);

                        Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, controleIntegracaoCargaEdi.LayoutEDI, null);
                        MemoryStream arquivoEDI = serGeracaoEDI.GerarArquivoRecursivo(notfis);

                        retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.EDINotfis
                        {
                            ProtocoloCarga = carga.Protocolo,
                            DataCriacaoEDI = controleIntegracaoCargaEdi.Data.ToString("dd/MM/yyyy HH:mm"),
                            EDI = Encoding.UTF8.GetString(arquivoEDI.ToArray()),
                        });
                    }
                    repControleIntegracaoCargaEDI.Atualizar(controleIntegracaoCargaEdi);
                }

                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.EDINotfis>>.CriarRetornoSucesso(retorno);
            }
            catch (WebServiceException ex)
            {
                Servicos.Log.TratarErro(ex, "RequestLog");
                unitOfWork.Rollback();
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.EDINotfis>>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.EDINotfis>>.CriarRetornoExcecao("Ocorreu uma falha ao buscar os EDIs NOTFIS do transportador.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Carga.AgrupamentoCarga> BuscarAgrupamentoCarga(int protocoloCarga)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
            Retorno<Dominio.ObjetosDeValor.Embarcador.Carga.AgrupamentoCarga> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.Carga.AgrupamentoCarga>();
            retorno.Status = true;
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloCarga);

                if (carga == null)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "Protocolo da carga não encontrado.";
                }
                else
                {
                    if (carga.CargaAgrupamento == null)
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Status = false;
                        retorno.Mensagem = "Carga não pertence a um agrupamento.";
                    }
                    else
                    {
                        List<int> protocolosCargas = repCarga.BuscarProtocolosCargasAgrupadas(carga.CargaAgrupamento.Protocolo);
                        if (protocolosCargas != null && protocolosCargas.Count > 0)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.AgrupamentoCarga agrupamentoCarga = new Dominio.ObjetosDeValor.Embarcador.Carga.AgrupamentoCarga();
                            agrupamentoCarga.ProtocoloCargaAgrupadora = carga.CargaAgrupamento.Protocolo;
                            agrupamentoCarga.ProtocoloCargasAgrupadas = protocolosCargas;

                            retorno.Objeto = agrupamentoCarga;
                            retorno.Status = true;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                        }
                        else
                        {
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Status = false;
                            retorno.Mensagem = "Não existe agrupamento para a carga protocolo " + protocoloCarga;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a visualização de cargas agrupadas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<int> AdicionarCargaCompleta(CargaIntegracaoCompleta cargaIntegracaoCompleta)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<int>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AdicionarCargaCompleta(cargaIntegracaoCompleta));
            });
        }

        public Retorno<int> ConsultarCargaPorNumeroCarregamento(string pedidoNumeroCarregamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<int>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConsultarCargaPorNumeroCarregamento(pedidoNumeroCarregamento));
            });
        }

        public Retorno<bool> SolicitarEmissaoDocumentos(int protocoloCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarFrete(new Protocolos() { protocoloIntegracaoCarga = protocoloCarga }));
            });
        }

        public Retorno<bool> RetornarEtapaNota(int protocoloCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).RetornarEtapaNota(protocoloCarga));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.DocumentosCarga> BuscarDocumentosCarga(int? protocoloCarga, string numeroCarga)
        {
            ValidarToken();

            protocoloCarga ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

                if (protocoloCarga > 0)
                    carga = repCarga.BuscarPorProtocolo((int)protocoloCarga);
                if (carga == null)
                    carga = repCarga.BuscarPorCodigoCargaEmbarcador(numeroCarga);

                if (carga == null)
                    return Retorno<DocumentosCarga>.CriarRetornoDadosInvalidos("Não foi localizada Carga para o protocolo informado.");

                Repositorio.Embarcador.Cargas.CargaCTe repCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes = repCTe.BuscarPorCarga(carga.Codigo, false, false, false, true);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMdfes = repMDFe.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaNFSes = repCTe.BuscarPorCarga(carga.Codigo, false, false, false, false, false, 0, 0, false, false, 0, 0, true);

                Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
                Servicos.WebService.MDFe.MDFe serMDFe = new Servicos.WebService.MDFe.MDFe(unitOfWork);
                Servicos.WebService.NFS.NFS serNFSe = new Servicos.WebService.NFS.NFS(unitOfWork);

                DocumentosCarga docsCarga = new DocumentosCarga();
                docsCarga.CTes = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();
                docsCarga.MDFes = new List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>();
                docsCarga.NFSes = new List<Dominio.ObjetosDeValor.WebService.NFS.NFS>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargaCtes)
                    docsCarga.CTes.Add(serCTe.ConverterObjetoCTe(cargaCte.CTe, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), TipoDocumentoRetorno.Nenhum, unitOfWork, false));

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMdfes)
                    docsCarga.MDFes.Add(serMDFe.ConverterObjetoCargaMDFe(cargaMDFe, TipoDocumentoRetorno.XML, unitOfWork, false));

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaNFSe in cargaNFSes)
                    docsCarga.NFSes.Add(serNFSe.ConverterObjetoCargaNFS(cargaNFSe, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), TipoDocumentoRetorno.XML, false, unitOfWork));

                if (docsCarga.CTes?.Count == 0 && docsCarga.MDFes?.Count == 0 && docsCarga.NFSes?.Count == 0)
                {
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCarga(carga.Codigo);
                    List<string> tagsPedido = new List<string>();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                        tagsPedido.Add(pedido.NumeroPedidoEmbarcador);

                    string tags = string.Join(", ", tagsPedido);

                    return Retorno<DocumentosCarga>.CriarRetornoDadosInvalidos($"Não foram localizados documentos na Carga Informada. Pedido(s): {tags}. Situação da carga: {carga.DescricaoSituacaoCarga}");
                }

                Dominio.ObjetosDeValor.WebService.Retorno<DocumentosCarga> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<DocumentosCarga>()
                {
                    Objeto = docsCarga,
                };

                return Retorno<DocumentosCarga>.CriarRetornoSucesso(retorno.Objeto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<DocumentosCarga>.CriarRetornoExcecao("Ocorreu uma falha ao buscar os documentos da Carga!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCargaPorDatas(DateTime dataDe, DateTime dataAte)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCargaService(dataDe, dataAte, integradora.Empresa.Codigo));
            });
        }

        #endregion Métodos de Consulta

        #region Métodos de Requisição

        public Retorno<bool> InformarSeparacaoMercadoria(int protocolo, int percentualSeparacao, bool separacaoMercadoriaConfirmada)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaPorProtocolo = repositorioCarga.BuscarPorProtocolo(protocolo);

                if (cargaPorProtocolo == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O protocolo informado não pertence a uma carga válida.");

                if (cargaPorProtocolo.SeparacaoMercadoriaConfirmada)
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao("A confirmação de separação já foi informada anterioremente para essa carga.");

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.Carga carga;
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupamento;
                bool cargaAgrupamento = cargaPorProtocolo.CargaAgrupamento != null;

                cargaPorProtocolo.Initialize();

                if (cargaAgrupamento)
                {
                    cargaPorProtocolo.CargaAgrupamento.Initialize();

                    cargasAgrupamento = repositorioCarga.BuscarCargasOriginais(cargaPorProtocolo.CargaAgrupamento.Codigo);
                    carga = cargaPorProtocolo.CargaAgrupamento;
                }
                else
                {
                    cargasAgrupamento = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                    carga = cargaPorProtocolo;
                }

                bool permitirAlterarSituacaoCargaParaAguardandoNFe = (
                    !carga.ExigeNotaFiscalParaCalcularFrete &&
                    carga.SituacaoCarga == SituacaoCarga.AgTransportador &&
                    (
                        (carga.Veiculo != null && carga.Empresa != null && carga.Motoristas.Count > 0) ||
                        (carga.TipoOperacao != null && carga.TipoOperacao.NaoExigeVeiculoParaEmissao && carga.Empresa != null)
                    )
                );

                DateTime dataAtualizacaoSeparacaoMercadoria = DateTime.Now;

                cargaPorProtocolo.DataInicioSeparacaoMercadoria = cargaPorProtocolo.DataInicioSeparacaoMercadoria ?? dataAtualizacaoSeparacaoMercadoria;
                cargaPorProtocolo.DataAtualizacaoSeparacaoMercadoria = dataAtualizacaoSeparacaoMercadoria;

                if (separacaoMercadoriaConfirmada)
                {
                    cargaPorProtocolo.SeparacaoMercadoriaConfirmada = true;
                    cargaPorProtocolo.PercentualSeparacaoMercadoria = 100;

                    if (permitirAlterarSituacaoCargaParaAguardandoNFe)
                    {
                        cargaPorProtocolo.SituacaoCarga = SituacaoCarga.AgNFe;

                        if (cargaPorProtocolo.Carregamento != null)
                        {
                            cargaPorProtocolo.Carregamento.SituacaoCarregamento = SituacaoCarregamento.Fechado;

                            repositorioCarregamento.Atualizar(cargaPorProtocolo.Carregamento);
                        }

                        if (!cargaAgrupamento)
                            Servicos.Embarcador.Carga.CargaIntegracao.AdicionarIntegracoesEtapaNFe(cargaPorProtocolo, configuracao, TipoServicoMultisoftware, unitOfWork);
                    }
                }
                else
                    cargaPorProtocolo.PercentualSeparacaoMercadoria = percentualSeparacao;

                repositorioCarga.Atualizar(cargaPorProtocolo, Auditado);

                if (cargaAgrupamento)
                {
                    cargaPorProtocolo.CargaAgrupamento.DataInicioSeparacaoMercadoria = cargaPorProtocolo.CargaAgrupamento.DataInicioSeparacaoMercadoria ?? cargasAgrupamento.Min(o => o.DataInicioSeparacaoMercadoria);
                    cargaPorProtocolo.CargaAgrupamento.DataAtualizacaoSeparacaoMercadoria = dataAtualizacaoSeparacaoMercadoria;

                    if (cargasAgrupamento.All(o => o.SeparacaoMercadoriaConfirmada))
                    {
                        cargaPorProtocolo.CargaAgrupamento.SeparacaoMercadoriaConfirmada = true;
                        cargaPorProtocolo.CargaAgrupamento.PercentualSeparacaoMercadoria = 100;

                        if (permitirAlterarSituacaoCargaParaAguardandoNFe)
                        {
                            cargaPorProtocolo.CargaAgrupamento.SituacaoCarga = SituacaoCarga.AgNFe;

                            Servicos.Embarcador.Carga.CargaIntegracao.AdicionarIntegracoesEtapaNFe(cargaPorProtocolo.CargaAgrupamento, configuracao, TipoServicoMultisoftware, unitOfWork);
                        }
                    }
                    else
                        cargaPorProtocolo.CargaAgrupamento.PercentualSeparacaoMercadoria = cargasAgrupamento.Sum(o => o.PercentualSeparacaoMercadoria / cargasAgrupamento.Count);

                    repositorioCarga.Atualizar(cargaPorProtocolo.CargaAgrupamento, Auditado);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao informar a separação da mercadoria");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> TransferirPedidoEntreCargas(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocoloOriginal, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocoloDestino)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();
            try
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocoloOriginal.protocoloIntegracaoCarga, protocoloOriginal.protocoloIntegracaoPedido);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaDestino = repCarga.BuscarPorProtocolo(protocoloDestino.protocoloIntegracaoCarga);

                if (cargaDestino != null)
                {
                    if (cargaDestino.SituacaoCarga != SituacaoCarga.AgNFe && cargaDestino.SituacaoCarga != SituacaoCarga.Nova)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A situação atual da carga de destino não permite receber o pedido.";
                        retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        return retorno;
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi encontrado nenhuma carga de destino com o protocolo informado";
                }

                if (cargasPedidos != null && cargasPedidos.Count > 0)
                {
                    unitOfWork.Start();

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidos)
                    {
                        if (cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe && cargaPedido.Carga.SituacaoCarga != SituacaoCarga.Nova)
                        {
                            unitOfWork.Rollback();
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "A situação atual da carga de destino não permite remover/transferir o pedido.";
                            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            return retorno;
                        }
                        else
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOriginal, null, "Transferio o pedido vinculado por integração para a carga " + cargaOriginal.CodigoCargaEmbarcador + ".", unitOfWork);

                            if (cargaPedido.Pedido != null && cargaPedido.Pedido.TipoOperacao != null && cargaPedido.Pedido.TerminalOrigem != null && cargaPedido.Pedido.TerminalOrigem.Terminal != null && (cargaPedido.Pedido.TipoOperacao.ImportarTerminalOrigemComoExpedidor || cargaPedido.Pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorta || cargaPedido.Pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorto))
                                cargaPedido.Expedidor = cargaPedido.Pedido.TerminalOrigem.Terminal;

                            if (cargaPedido.Pedido != null && cargaPedido.Pedido.TipoOperacao != null && cargaPedido.Pedido.TerminalDestino != null && cargaPedido.Pedido.TerminalDestino.Terminal != null && (cargaPedido.Pedido.TipoOperacao.ImportarTerminalDestinoComoRecebedor || cargaPedido.Pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortaPorto || cargaPedido.Pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorto))
                                cargaPedido.Recebedor = cargaPedido.Pedido.TerminalDestino.Terminal;

                            cargaPedido.Carga = cargaDestino;
                            cargaPedido.CargaOrigem = cargaDestino;
                            repCargaPedido.Atualizar(cargaPedido);

                            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCargaOrigem = repCargaPedido.BuscarPedidosPorCarga(cargaOriginal.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosCargaOrigem)
                            {
                                pedido.QuantidadeContainerBooking = pedido.QuantidadeContainerBooking - 1;
                                if (pedido.QuantidadeContainerBooking < 0)
                                    pedido.QuantidadeContainerBooking = 0;
                                repPedido.Atualizar(pedido);
                            }

                            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCargaDestino = repCargaPedido.BuscarPedidosPorCarga(cargaDestino.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosCargaDestino)
                            {
                                if (pedido.TipoOperacao != null && pedido.TerminalOrigem != null && pedido.TerminalOrigem.Terminal != null && (pedido.TipoOperacao.ImportarTerminalOrigemComoExpedidor || pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorta || pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorto))
                                    pedido.Expedidor = pedido.TerminalOrigem.Terminal;

                                if (pedido.TipoOperacao != null && pedido.TerminalDestino != null && pedido.TerminalDestino.Terminal != null && (pedido.TipoOperacao.ImportarTerminalDestinoComoRecebedor || pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortaPorto || pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorto))
                                    pedido.Recebedor = pedido.TerminalDestino.Terminal;

                                pedido.QuantidadeContainerBooking = pedido.QuantidadeContainerBooking + 1;
                                repPedido.Atualizar(pedido);
                            }

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaDestino, null, "Recebeu o pedido que estava vinculado a carga " + cargaPedido.Carga.CodigoCargaEmbarcador + " por integração.", unitOfWork);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosOriginal = repCargaPedido.BuscarPorCarga(cargaOriginal.Codigo);
                            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaOriginal, cargaPedidosOriginal, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDestino = repCargaPedido.BuscarPorCarga(cargaDestino.Codigo);
                            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaDestino, cargaPedidosDestino, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                        }
                    }

                    retorno.Objeto = true;
                    retorno.Status = true;
                    unitOfWork.CommitChanges();
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi encontrado nenhuma carga/pedido de origem com o protocolo informado";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao remover/transferir o pedido da carga";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<bool> RemoverPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            Servicos.Log.TratarErro("RemoverPedido - Protocolo: " + (protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty), "Request");

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if ((cargasPedidos == null) || (cargasPedidos.Count == 0))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possível encontrar nenhum pedido com os protocolos informados");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidos)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (cargaPedido.Pedido.Container != null)
                            throw new WebServiceException("O pedido selecionado já possui container vinculado.");

                        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoVinculadoCarga(cargaPedido, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, removerPedido: false);

                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCarga = repositorioCargaPedido.BuscarPedidosPorCarga(carga.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoCarga in pedidosCarga)
                        {
                            pedidoCarga.QuantidadeContainerBooking = pedidoCarga.QuantidadeContainerBooking - 1;

                            if (pedidoCarga.QuantidadeContainerBooking < 0)
                                pedidoCarga.QuantidadeContainerBooking = 0;

                            repositorioPedido.Atualizar(pedidoCarga);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Excluiu pedido vinculado por Integração.", unitOfWork);
                    }
                    else
                    {
                        bool permitirRemoverTodos = !configuracaoGeralCarga.NaoPermitirRemoverUltimoPedidoCarga;

                        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(carga, cargaPedido, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, null, permitirRemoverTodos, false, configuracaoWebService.NaoRecalcularFreteAoAdicionarRemoverPedido);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Removeu o pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} via integração", unitOfWork);

                        if (permitirRemoverTodos && !repositorioCargaPedido.PossuiCargaPedidoPorCarga(carga.Codigo))//Se era o último carga pedido, solicita o cancelamento da carga
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                            {
                                Carga = carga,
                                MotivoCancelamento = "Cancelamento por remoção do último pedido via integração",
                                TipoServicoMultisoftware = TipoServicoMultisoftware,
                                Usuario = Auditado.Usuario
                            };

                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoEmbarcador, unitOfWork);
                            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, unitOfWork.StringConexao, TipoServicoMultisoftware);

                            if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                                throw new WebServiceException(cargaCancelamento.MensagemRejeicaoCancelamento);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento, null, "Adicionou o cancelamento da Carga ao remover o seu último pedido via integração", unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Adicionou o cancelamento da Carga ao remover o seu último pedido via integração", unitOfWork);
                        }
                        else
                        {
                            if (!(cargaPedido.Carga.TipoOperacao?.NaoIntegrarOpentech ?? false) && !(cargaPedido.Carga.Veiculo?.NaoIntegrarOpentech ?? false))
                                cargaPedido.Carga.AguardarIntegracaoEtapaTransportador = Servicos.WebService.NFe.NotaFiscal.AdicionarIntegracaoSM(cargaPedido.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, configuracaoEmbarcador.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, cargaPedido.Carga.CargaEmitidaParcialmente, unitOfWork);

                            carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;
                            if (carga.SituacaoCarga == SituacaoCarga.AgNFe && carga.ExigeNotaFiscalParaCalcularFrete)
                                carga.ProcessandoDocumentosFiscais = true;

                            repositorioCarga.Atualizar(carga);
                        }
                    }
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao remover o pedido da carga");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Servicos.Log.TratarErro($"AdicionarPedido: {(cargaIntegracao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cargaIntegracao) : string.Empty)}", "Request");

            if (cargaIntegracao == null)
            {
                string json = "{'ProtocoloCarga':0,'ProtocoloPedido':0,'NumeroControlePedido':null,'NumeroCarga':'T2022Dez27126','NumeroPreCarga':null,'IdentificacaoAdicional':null,'NumeroPedidoEmbarcador':'T2022Dez2726','CFOP':0,'ValorFreteCalculado':false,'Filial':{'CodigoIntegracao':'33453598000123','Descricao':null,'CNPJ':null,'TipoFilial':0,'CodigoAtividade':0,'Endereco':null,'Ativo':false},'FilialVenda':null,'TransportadoraEmitente':{'Atividade':0,'CNPJ':'21849120000138','RazaoSocial':null,'NomeFantasia':null,'CodigoIntegracao':null,'IE':null,'InscricaoMunicipal':null,'InscricaoST':null,'RNTRC':null,'SimplesNacional':false,'Emails':null,'EmissaoDocumentosForaDoSistema':false,'LiberacaoParaPagamentoAutomatico':false,'Endereco':null,'DadosBancarios':null,'Certificado':null,'CodigoDocumento':null,'RegimeTributario':0,'Ativo':null},'UsarOutroEnderecoOrigem':false,'Origem':null,'UsarOutroEnderecoDestino':false,'Destino':null,'RegiaoDestino':null,'Fronteira':null,'PontoPartida':null,'Remetente':{'ClienteExterior':false,'CPFCNPJ':'33.453.598/0465-49','CodigoIntegracao':null,'TipoPessoa':0,'CodigoAtividade':0,'RGIE':null,'IM':null,'Observacao':null,'RazaoSocial':null,'NomeFantasia':null,'CNAE':null,'Endereco':null,'Email':null,'AtualizarEnderecoPessoa':false,'EmailFatura':null,'Codigo':null,'CodigoCategoria':null,'CodigoDocumento':null,'RNTRC':null,'NumeroCartaoCIOT':null,'GerarCIOT':null,'TipoFavorecidoCIOT':null,'TipoPagamentoCIOT':null,'GrupoPessoa':null,'ExigirNumeroControleCliente':false,'ExigirNumeroNumeroReferenciaCliente':false,'TipoEmissaoCTeDocumentosExclusivo':0,'ParametroRateioFormulaExclusivo':0,'AdicionarComoOutroEndereco':false,'InativarCliente':false,'CodigoDocumentoFornecedor':null,'Cliente':false,'NaoEnviarParaDocsys':false,'Fornecedor':false,'RaioEmMetros':0,'RegimeTributario':0,'ExigeAgendamento':null,'NumeroCUITRUIT':null,'IndicadorIE':null,'DadosBancarios':null,'NaoEObrigatorioInformarNfeNaColeta':null,'NaoExigePreenchimentoDeChecklistEntrega':null,'MesoRegiao':null},'Destinatario':{'ClienteExterior':false,'CPFCNPJ':'33.453.598/0108-62','CodigoIntegracao':null,'TipoPessoa':0,'CodigoAtividade':2,'RGIE':null,'IM':null,'Observacao':null,'RazaoSocial':null,'NomeFantasia':null,'CNAE':null,'Endereco':null,'Email':null,'AtualizarEnderecoPessoa':false,'EmailFatura':null,'Codigo':null,'CodigoCategoria':null,'CodigoDocumento':null,'RNTRC':null,'NumeroCartaoCIOT':null,'GerarCIOT':null,'TipoFavorecidoCIOT':null,'TipoPagamentoCIOT':null,'GrupoPessoa':null,'ExigirNumeroControleCliente':false,'ExigirNumeroNumeroReferenciaCliente':false,'TipoEmissaoCTeDocumentosExclusivo':0,'ParametroRateioFormulaExclusivo':0,'AdicionarComoOutroEndereco':false,'InativarCliente':false,'CodigoDocumentoFornecedor':null,'Cliente':false,'NaoEnviarParaDocsys':false,'Fornecedor':false,'RaioEmMetros':0,'RegimeTributario':0,'ExigeAgendamento':null,'NumeroCUITRUIT':null,'IndicadorIE':null,'DadosBancarios':null,'NaoEObrigatorioInformarNfeNaColeta':null,'NaoExigePreenchimentoDeChecklistEntrega':null,'MesoRegiao':null},'Tomador':null,'Recebedor':null,'Expedidor':null,'FreteRota':null,'CanalEntrega':{'Descricao':null,'CodigoIntegracao':'DISTRIBUICAO','Filial':null},'CanalVenda':null,'Deposito':null,'DataInicioCarregamento':'19/12/2022 11:00:01','DataFinalCarregamento':null,'DataTerminoCarregamento':null,'DataValidade':null,'DataAgendamento':null,'DataColeta':null,'DataPrevisaoEntrega':null,'HoraPrevisaoEntrega':null,'PrevisaoEntregaTransportador':null,'EntregaAgendada':false,'SenhaAgendamentoEntrega':null,'DataCriacaoCarga':null,'DataCancelamentoCarga':null,'DataAnulacaoCarga':null,'DataUltimaLiberacao':null,'UsuarioCriacaoRemessa':null,'NumeroOrdem':null,'NumeroPaletes':0,'NumeroPaletesPagos':0.0,'NumeroSemiPaletes':0.0,'NumeroSemiPaletesPagos':0.0,'NumeroPaletesFracionado':0.0,'NumeroCombis':0.0,'NumeroCombisPagas':0.0,'PesoTotalPaletes':0.0,'ValorTotalPaletes':0.0,'PesoBruto':0.01,'PesoLiquido':0.0,'CubagemTotal':1.0,'QuantidadeVolumes':0,'Distancia':0.0,'Produtos':[{'Codigo':0,'CodigoProduto':'COMBUSTIVEL','DescricaoProduto':null,'CodigoGrupoProduto':'COMBUSTIVEIS','CodigocEAN':null,'DescricaoGrupoProduto':'COMBUSTIVEIS','ValorUnitario':0.01,'PesoUnitario':1.00,'Quantidade':1.00,'QuantidadePlanejada':0.0,'UnidadeMedida':null,'QuantidadeEmbalagem':0.0,'PesoTotalEmbalagem':0.0,'MetroCubito':0.0,'CodigoDocumentacao':null,'Observacao':null,'ObservacaoCarga':null,'InativarCadastro':false,'Atualizar':true,'SetorLogistica':null,'ClasseLogistica':null,'CodigoNCM':null,'ProdutoLotes':null,'ProdutoDivisoesCapacidade':null,'QuantidadePallet':0.0,'QuantidadePorCaixa':0,'QuantidadeCaixasVazias':0,'QuantidadeCaixasVaziasPlanejadas':0,'QuantidadeCaixaPorPallet':0,'Lastro':0.0,'Camada':0.0,'Altura':1.0,'Largura':0.0,'Comprimento':0.0,'LinhaSeparacao':null,'EnderecoProduto':null,'MarcaProduto':null,'TipoEmbalagem':null,'PalletFechado':false,'CSTICMS':null,'OrigemMercadoria':null,'CodigoNFCI':null,'CodigoEAN':null,'CanalDistribuicao':null,'SiglaModalidade':null,'Imuno':null,'ValorTotal':0.0,'ValorDesconto':0.0,'FinalidadeProduto':null,'IdDemanda':null,'SiglaUnidade':null,'TemperaturaTransporte':null,'PesoLiquidoUnitario':0.0,'QtdPalet':0.0,'AlturaCM':0.0,'LarguraCM':0.0,'ComprimentoCM':0.0,'QuantidadeCaixa':0,'CodigoOrganizacao':null,'Canal':null,'Setor':null,'Organizacao':null,'Filiais':null}],'ProdutoPredominante':null,'UtilizarTipoTomadorInformado':false,'TipoTomador':0,'TipoPagamento':0,'Motoristas':[{'Codigo':0,'CPF':'973.783.530-19','CategoriaCNH':null,'CodigoIntegracao':null,'Nome':null,'DataHabilitacao':null,'DataAdmissao':null,'DataNascimento':null,'DataValidadeGR':null,'tipoMotorista':0,'Escolaridade':null,'EstadoCivil':null,'RG':null,'EstadoRG':null,'NumeroHabilitacao':null,'DadosBancarios':null,'ListaDadosBancarios':null,'ListaContatos':null,'Email':null,'DataVencimentoHabilitacao':null,'Endereco':null,'Ativo':null,'MotivoBloqueio':null,'DataSuspensaoInicio':null,'DataSuspensaoFim':null,'Transportador':null,'NumeroCartao':null,'NumeroPISPASEP':null,'DataEmissaoRG':null,'OrgaoEmissorRG':null}],'Veiculo':{'Placa':'QQO2335','Renavam':'12547898744','UF':null,'RNTC':null,'Tara':0,'CapacidadeKG':0,'CapacidadeM3':0,'NumeroFrota':null,'NumeroChassi':null,'NumeroMotor':null,'DataAquisicao':null,'AnoFabricacao':0,'AnoModelo':0,'Ativo':false,'DataValidadeGR':null,'Transportador':null,'TipoCarroceria':0,'TipoVeiculo':1,'TipoRodado':0,'ModeloVeicular':null,'GrupoPessoaSegmento':null,'TipoPropriedadeVeiculo':1,'Proprietario':null,'Motoristas':null,'Modelo':null,'Codigo':0,'Reboques':null,'XTexto':null,'XCampo':null,'MotivoBloqueio':null,'DataSuspensaoInicio':null,'DataSuspensaoFim':null,'PossuiTagValePedagio':null,'OrdemReboque':0,'DataRetiradaCtrn':null,'NumeroContainer':null,'TaraContainer':0,'MaxGross':0,'Anexos':null,'ValorContainerAverbacao':0.0,'TipoTagValePedagio':null,'NumeroCartaoValePedagio':null,'Cor':null,'PossuiRastreador':false,'NumeroEquipamentoRastreador':null,'TecnologiaRastreador':null,'TipoComunicacaoRastreador':null,'CentroResultado':null,'SegmentoVeiculo':null,'KilometragemAtual':0,'CapacidadeMaximaTanque':0.0,'DescricaoModeloVeiculo':null,'DescricaoMarcaVeiculo':null,'Equipamentos':null},'VeiculoDaNota':null,'ModeloVeicular':{'CodigoIntegracao':'CCC6','Descricao':null,'TipoModeloVeicular':0,'DivisaoCapacidade':null,'QuantidadeExtraExcedenteTolerado':0.0,'CodigoAgrupamentoCarga':null,'CodigoInterno':0},'TipoOperacao':{'CodigoIntegracao':'VP_RAIZEN','Descricao':null,'CNPJsDaOperacao':null,'BloquearEmissaoDosDestinatario':false,'BloquearEmissaoDeEntidadeSemCadastro':false,'TipoCobrancaMultimodal':0,'ModalPropostaMultimodal':0,'TipoServicoMultimodal':0,'TipoPropostaMultimodal':0,'CNPJsDestinatariosNaoAutorizados':null,'Atualizar':false},'FuncionarioVendedor':null,'FuncionarioSupervisor':null,'FuncionarioGerente':null,'TipoCargaEmbarcador':{'CodigoIntegracao':'CargaPerigosa','Descricao':null,'CNPJsDoTipoCargaNoEmbarcador':null,'ClasseONU':null,'SequenciaONU':null,'CodigoPSNONU':null,'ObservacaoONU':null},'ValorFrete':{'FreteProprio':0.01,'ComponentesAdicionais':null,'ICMS':null,'ISS':null,'ValorTotalAReceber':0.0,'ValorPrestacaoServico':0.0,'ValorAReceberSemImpostoIncluso':0.0,'FreteFilialEmissora':0.0},'ValorFreteFilialEmissora':null,'FecharCargaAutomaticamente':true,'ViagemJaFoiFinalizada':false,'PedidoPallet':false,'Observacao':null,'ObservacaoTransportador':null,'ObservacaoInterna':null,'ObservacaoCTe':null,'CodigoAgrupamento':null,'ObservacaoLocalEntrega':null,'TipoRateioProdutos':0,'ImpressoraNumero':null,'Lacres':null,'ValorDescarga':0.0,'ValorPedagio':0.0,'NotasFiscais':[{'Protocolo':0,'Chave':'51220379379491005061550010002907701685176592','Rota':null,'Numero':1,'Serie':null,'Modelo':null,'Valor':0.01,'BaseCalculoICMS':0.00,'ValorICMS':0.0,'BaseCalculoST':0.00,'ValorFreteLiquido':0.0,'ValorFrete':0.0,'ValorST':0.0,'ValorTotalProdutos':0.0,'ValorSeguro':0.0,'ValorDesconto':0.0,'ValorImpostoImportacao':0.0,'ValorPIS':0.0,'ValorCOFINS':0.0,'ValorOutros':0.0,'ValorIPI':0.0,'PesoBruto':0.0,'PesoLiquido':0.0,'Cubagem':0.0,'MetroCubico':0.0,'VolumesTotal':0.0,'DataEmissao':'28/07/22 02:34','NaturezaOP':null,'QuantidadePallets':0.0,'InformacoesComplementares':null,'TipoDocumento':null,'TipoDeCarga':null,'NumeroCarregamento':null,'ModalidadeFrete':0,'Emitente':null,'Destinatario':null,'Recebedor':null,'Expedidor':null,'Tomador':null,'Transportador':null,'Veiculo':null,'Volumes':null,'TipoCarga':null,'TipoOperacaoNotaFiscal':0,'SituacaoNFeSefaz':0,'Produtos':null,'Contabilizacao':null,'Canhoto':null,'NumeroDT':null,'DataEmissaoDT':null,'CodigoIntegracaoCliente':null,'NumeroSolicitacao':null,'NumeroPedido':null,'Ultimos7DigitosNumeroPedido':null,'NumeroRomaneio':null,'SubRota':null,'GrauRisco':null,'AliquotaICMS':0.00,'Observacao':null,'DataPrevisao':null,'KMRota':0.0,'ValorComponenteFreteCrossDocking':0.0,'ValorComponenteAdValorem':0.0,'ValorComponenteDescarga':0.0,'ValorComponentePedagio':0.0,'ValorComponenteAdicionalEntrega':0.0,'NCMPredominante':null,'CodigoProduto':null,'CFOPPredominante':'5102','NumeroControleCliente':null,'NumeroCanhoto':null,'NumeroReferenciaEDI':null,'PINSuframa':null,'DescricaoMercadoria':null,'PesoAferido':0.0,'TipoOperacao':null,'ModeloVeicular':null,'ObsPlaca':null,'ObsTransporte':null,'ChaveCTe':null,'NumeroDocumentoEmbarcador':null,'NumeroTransporte':null,'DataHoraCriacaoEmbrcador':null,'IBGEInicioPrestacao':null,'MasterBL':null,'NumeroOutroDocumento':null,'PesoMiligrama':'0','DocumentoRecebidoViaNOTFIS':false,'Containeres':null,'ClassificacaoNFe':null}],'CTes':null,'BlocosCarregamento':null,'Distribuicoes':null,'Doca':null,'Averbacao':null,'Temperatura':null,'FaixaTemperatura':null,'Vendedor':null,'Ordem':null,'OrdemColetaProgramada':0,'PortoSaida':null,'PortoChegada':null,'Companhia':null,'Navio':null,'Reserva':null,'Resumo':null,'ETA':null,'TipoEmbarque':null,'ValorFreteCobradoCliente':0.0,'ValorCustoFrete':0.0,'ValorFreteInformativo':0.0,'CodigoIntegracaoRota':null,'DescricaoRota':null,'DataInclusaoPCP':null,'DataInclusaoBooking':null,'DeliveryTerm':null,'IdAutorizacao':null,'PossuiGenset':false,'TipoPedido':null,'OrdemEntrega':0,'OrdemColeta':0,'FreteNegociado':null,'PedidoTrocaNota':false,'NumeroPedidoTrocaNota':null,'NumeroCIOT':null,'NaoGlobalizarPedido':false,'Adicional1':null,'Adicional2':null,'Adicional3':null,'Adicional4':null,'Adicional5':null,'Adicional6':null,'Adicional7':null,'PermiteQuebraPedidoMultiplosCarregamentos':false,'ClienteAdicional':null,'Despachante':null,'ViaTransporte':null,'PortoViagemOrigem':null,'PortoViagemDestino':null,'InLand':null,'PagamentoMaritimo':null,'ClienteDonoContainer':null,'TipoProbe':null,'CargaPaletizada':false,'NavioViagem':null,'ETS':null,'FreeDeten':0,'NumeroEXP':null,'RefEXPTransferencia':null,'StatusEXP':null,'NumeroPedidoProvisorio':null,'Especie':null,'StatusPedidoEmbarcador':null,'AcondicionamentoCarga':null,'DataEstufagem':null,'Onda':null,'ClusterRota':null,'DataPrevisaoInicioViagem':null,'DataPrevisaoChegadaDestinatario':null,'NumeroPager':null,'DataInicioViagem':null,'DataSeparacaoMercadoria':null,'RotaEmbarcador':null,'NumeroBooking':null,'Embarcador':null,'Viagem':null,'ViagemLongoCurso':null,'PortoOrigem':null,'PortoDestino':null,'TerminalPortoOrigem':null,'TerminalPortoDestino':null,'TipoContainerReserva':null,'ContemCargaPerigosa':false,'ContemCargaRefrigerada':false,'TemperaturaObservacao':null,'ValidarNumeroContainer':false,'PropostaComercial':null,'Transbordo':null,'CodigoOrdemServico':0,'NumeroOrdemServico':null,'Embarque':null,'MasterBL':null,'NumeroDIEmbarque':null,'ProvedorOS':null,'Container':null,'TaraContainer':0,'NumeroLacre1':null,'NumeroLacre2':null,'NumeroLacre3':null,'CodigoBooking':0,'NumeroBL':null,'NecessitaAverbacao':false,'CargaRefrigeradaPrecisaEnergia':false,'QuantidadeTipoContainerReserva':0,'FormaAverbacaoCTE':null,'PercentualADValorem':0.0,'TipoDocumentoAverbacao':null,'ObservacaoProposta':null,'TipoPropostaFeeder':null,'DescricaoTipoPropostaFeeder':null,'DescricaoCarrierNavioViagem':null,'RealizarCobrancaTaxaDocumentacao':false,'QuantidadeConhecimentosTaxaDocumentacao':0,'ValorTaxaDocumento':0.0,'ContainerADefinir':false,'ValorCusteioSVM':0.0,'QuantidadeContainerBooking':0,'EmpresaResponsavel':null,'CentroCusto':null,'CNPJsDestinatariosNaoAutorizados':null,'ParametroIdentificacaoCliente':null,'PedidoDeSVMTerceiro':false,'NumeroCE':null,'ValorTaxaFeeder':0.0,'TipoCalculoCargaFracionada':null,'CargaDePreCarga':false,'SituacaoCarga':0,'OperadorCargaNome':null,'OperadorCargaEmail':null,'OperadorCargaCPF':null,'NaoAtualizarDadosDoPedido':false,'IMOUnidade':null,'IMOClasse':null,'IMOSequencia':null,'NecessarioAjudante':false,'AntecipacaoICMS':false,'DiasItinerario':0,'DiasUteisPrazoTransportador':0,'NumeroEntregasFinais':0,'PossuiPendenciaRoteirizacao':false,'NumeroPedidoCliente':null,'IDPropostaTrizy':0,'IDLoteTrizy':0,'DadosTransporteMaritimo':null,'ValorTotalPedido':0.0,'PedidoDeDevolucao':false,'NumeroPedidoDevolucao':null,'ProcessamentoEspecial':null,'HorarioEntrega':null,'DiasRestricaoEntrega':null,'NumeroPedidoICT':null,'CondicaoExpedicao':null,'GrupoFreteMaterial':null,'RestricaoEntrega':null,'DataCriacaoRemessa':null,'DataCriacaoVenda':null,'IndicadorPOF':null,'NumeroRastreioCorreios':null,'ZonaTransporte':null,'PeriodoEntrega':null,'DetalheEntrega':null,'ProdutoVolumoso':false,'ProtocoloCotacao':0,'IndicativoColetaEntrega':0,'TipoServico':null,'NumeroAutorizacaoColetaEntrega':null,'ClientePropostaComercial':null,'TipoSeguro':null,'NumeroOSMae':null,'CodigoPedidoCliente':null,'ChavesCTes':null,'KMAsfaltoAteDestino':0.0,'KMChaoAteDestino':0.0,'CodigoAgrupamentoCarregamento':null,'NumeroAcerto':0,'DataInicialAcerto':null,'DataFinalAcerto':null,'TipoServicoCarga':null,'ExecaoCab':null}";
                cargaIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>(json);
            }

            ValidarToken();

            StringBuilder mensagemErro = new StringBuilder();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

                int codigoPersonalizado = 0;

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;

                bool ignorarTransportadorNaoCadastrado = false;
                Servicos.WebService.Carga.Carga.ValidarCamposIntegracaoCarga(cargaIntegracao, configuracaoTMS.ReplicarCadastroVeiculoIntegracaoTransportadorDiferente, ignorarTransportadorNaoCadastrado, configuracaoTMS.BuscarClientesCadastradosNaIntegracaoDaCarga, configuracaoTMS.UtilizarProdutosDiversosNaIntegracaoDaCarga, ref mensagemErro, unitOfWork, configuracaoTMS, TipoServicoMultisoftware, out codigoPersonalizado, out tipoOperacao, out filial);

                if (mensagemErro.Length > 0)
                {
                    Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} Retornou essa mensagem (validação): {mensagemErro.ToString()}", "RequestLog");

                    Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });

                    if (codigoPersonalizado > 0)
                    {
                        retorno.CodigoMensagem = codigoPersonalizado;
                        AuditarRetornoDadosInvalidosCNPJTransportador(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);
                    }
                    else
                        AuditarRetornoDadosInvalidos(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);

                    return retorno;
                }

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarPorProtocoloCarga(protocolo?.protocoloIntegracaoCarga ?? 0);
                if (cargasPedidos == null || cargasPedidos.Count == 0)
                {
                    Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos("Não foi encontrado nenhuma carga/pedido com o protocolo informado.", new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                    return retorno;
                }
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoEncaixe = cargasPedidos.FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = cargaPedidoEncaixe.Carga;
                if (cargaOrigem.SituacaoCarga != SituacaoCarga.AgNFe && cargaOrigem.SituacaoCarga != SituacaoCarga.Nova)
                {
                    Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos("A situação atual da carga não permite remover o pedido.", new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                    return retorno;
                }
                if (cargaOrigem.ProcessandoDocumentosFiscais)
                {
                    Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos("A atual situação da carga (Processando Documentos Fiscais) não permite que ela seja modificada; ", new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                }

                int codigoCargaExistente = 0;
                int protocoloPedidoExistente = 0;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Servicos.WebService.Carga.Pedido(unitOfWork).CriarPedido(cargaIntegracao, filial, tipoOperacao, ref mensagemErro, TipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, false, Auditado, configuracaoTMS, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao, false, false, true, false);

                if (mensagemErro.Length == 0 || protocoloPedidoExistente > 0)
                {
                    if (protocoloPedidoExistente == 0)
                        new Servicos.WebService.Carga.ProdutosPedido(unitOfWork).AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref mensagemErro, unitOfWork, Auditado);

                    if (cargaIntegracao.Transbordo != null && cargaIntegracao.Transbordo.Count > 0)
                        new Servicos.WebService.Carga.ProdutosPedido(unitOfWork).SalvarTransbordo(pedido, cargaIntegracao.Transbordo, ref mensagemErro, unitOfWork, unitOfWork.StringConexao, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);

                    Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga(unitOfWork);
                    cargaPedido = Servicos.Embarcador.Carga.CargaPedido.CriarCargaPedido(cargaOrigem, pedido, null, unitOfWork, unitOfWork.StringConexao, TipoServicoMultisoftware, configuracaoTMS, false, configuracaoGeralCarga);

                    if (cargaPedido != null)
                    {
                        servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, "Adicionou pedido para a carga. Protocolo " + cargaPedido.Pedido.Protocolo.ToString(), unitOfWork);
                    }

                    if (cargaOrigem.CargaAgrupamento != null)
                    {
                        Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupamento = cargaOrigem.CargaAgrupamento;
                        serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaAgrupamento, configuracaoTMS, unitOfWork, TipoServicoMultisoftware);
                    }

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCarga = repCargaPedido.BuscarPedidosPorCarga(cargaOrigem.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoCarga in pedidosCarga)
                    {
                        pedidoCarga.QuantidadeContainerBooking = pedidoCarga.QuantidadeContainerBooking + 1;
                        repPedido.Atualizar(pedidoCarga);
                    }
                }

                if (mensagemErro.Length > 0)
                {
                    Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} Retornou essa mensagem: {mensagemErro.ToString()}", "RequestLog");
                    unitOfWork.Rollback();

                    if ((codigoCargaExistente > 0 && protocoloPedidoExistente > 0) || (protocoloPedidoExistente > 0 && string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga)))
                    {
                        Servicos.Log.TratarErro($"codigoCargaExistente: {codigoCargaExistente} protocoloPedidoExistente: {protocoloPedidoExistente}", "RequestLog");
                        bool retornarDuplicidade = true;
                        if (configuracaoTMS.RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto)
                        {
                            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(codigoCargaExistente);
                            if (cargaCancelamento != null)
                                retornarDuplicidade = false;
                        }
                        if (retornarDuplicidade)
                        {
                            if (configuracaoTMS.RetornosDuplicidadeWSSubstituirPorSucesso)
                            {
                                if (cargaIntegracao.FecharCargaAutomaticamente && configuracaoTMS.FecharCargaPorThread && codigoCargaExistente > 0)
                                {
                                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                                    Dominio.Entidades.Embarcador.Cargas.Carga cargaFechamento = repCarga.BuscarPorCodigo(codigoCargaExistente);
                                    if (cargaFechamento != null && !cargaFechamento.CargaFechada)
                                    {
                                        cargaFechamento.FechandoCarga = true;
                                        repCarga.Atualizar(cargaFechamento);
                                    }
                                }

                                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = codigoCargaExistente, protocoloIntegracaoPedido = protocoloPedidoExistente, ParametroIdentificacaoCliente = cargaIntegracao.ParametroIdentificacaoCliente });
                            }
                            else
                            {
                                AuditarRetornoDuplicidadeDaRequisicao(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);
                                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDuplicidadeRequisicao(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = codigoCargaExistente, protocoloIntegracaoPedido = protocoloPedidoExistente, ParametroIdentificacaoCliente = cargaIntegracao.ParametroIdentificacaoCliente });
                            }
                        }
                        else
                        {
                            AuditarRetornoDadosInvalidos(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);
                            return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                        }
                    }
                    else
                    {
                        AuditarRetornoDadosInvalidos(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);

                        return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                    }
                }

                unitOfWork.CommitChanges();

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                Servicos.Log.TratarErro($"AdicionarPedido Retorno: Protocolo carga = {cargaPedido?.Carga.Codigo ?? 0}, protocolo pedido = {pedido?.Codigo ?? 0} | Tempo total levado: {ts.ToString(@"mm\:ss\:fff")}", "TempoExecucao");

                if (cargaPedido != null && cargaPedido.Carga != null)
                    Servicos.Log.TratarErro($"AdicionarPedido Retorno: Protocolo carga = {cargaPedido.Carga.Codigo}, protocolo pedido = {pedido.Codigo}", "RequestLog");
                else if (pedido != null)
                    Servicos.Log.TratarErro($"AdicionarPedido Retorno: Protocolo pedido = {pedido.Codigo}", "RequestLog");

                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo /*cargaPedido.Carga.Codigo*/ : 0, protocoloIntegracaoPedido = pedido?.Protocolo /*pedido?.Codigo*/ ?? 0 });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} retornou exceção a seguir: {excecao.Message}");

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                    return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDuplicidadeRequisicao(excecao.Message, new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { ParametroIdentificacaoCliente = cargaIntegracao.ParametroIdentificacaoCliente });

                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} retornou exceção a seguir:", "RequestLog");
                ArmazenarLogIntegracao(cargaIntegracao, unitOfWork);
                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoExcecao($"Ocorreu uma falha ao obter os dados das integrações. {mensagemErro.ToString()}");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AlterarPedido(Dominio.ObjetosDeValor.WebService.Pedido.AlteracaoPedido alteracaoPedido)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Pedido.AlteracaoPedido servicoAlteracaoPedido = new Servicos.Embarcador.Pedido.AlteracaoPedido(unitOfWork);

                servicoAlteracaoPedido.Adicionar(alteracaoPedido, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao alterar o pedido");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarConsultaAlteracoesPedidosFinalizadas(List<int> protocolosIntegracaoPedido)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if ((protocolosIntegracaoPedido == null) || (protocolosIntegracaoPedido.Count == 0))
                    throw new WebServiceException("Um ou mais protocolos de integração de pedido deve ser informado para confirmar a consulta das alterações de pedidos finalizadas");

                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorioAlteracaoPedido = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(unitOfWork);

                repositorioAlteracaoPedido.ConfirmarConsultaFinalizadas(protocolosIntegracaoPedido);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoExcecao(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a consulta das alterações de pedidos finalizadas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> BloquearCargaFilaCarregamento(int protocolo)
        {
            Servicos.Log.TratarErro("BloquearCargaFilaCarregamento Protocolo Carga: " + protocolo.ToString(), "RequestLog");

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoFilaCarregamento(unitOfWork).BloquearCargaFilaCarregamentoPorCarga(protocolo);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception)
            {
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao bloquear a carga para a fila de carregamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> LiberarCargaFilaCarregamento(int protocolo)
        {
            Servicos.Log.TratarErro("LiberarCargaFilaCarregamento Protocolo Carga: " + protocolo.ToString(), "RequestLog");

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoFilaCarregamento(unitOfWork).LiberarCargaFilaCarregamentoPorCarga(protocolo);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception)
            {
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao liberar a carga para a fila de carregamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarIntegracaoCarregamento(int protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoCarregamento(protocolo));
            });
        }

        public Retorno<bool> ConfirmarIntegracaoCarregamentoEmMontagem(int protocolo)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);

            try
            {

                //Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocolo);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(protocolo);
                if (carregamento != null)
                {
                    if (!carregamento.CarregamentoIntegradoERP)
                    {
                        unitOfWork.Start();

                        carregamento.CarregamentoIntegradoERP = true;
                        repCarregamento.Atualizar(carregamento);

                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();
                        if (configuracaoMontagemCarga.GerarCargaAoConfirmarIntegracaoCarregamento)
                        {
                            Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                            servicoMontagemCarga.GerarCargaCarregamentoWS(carregamento, configuracaoMontagemCarga, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso);
                            if (carregamento.SituacaoCarregamento == SituacaoCarregamento.EmMontagem)
                            {
                                carregamento.SituacaoCarregamento = SituacaoCarregamento.Fechado;
                                repCarregamento.Atualizar(carregamento);
                            }
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carregamento, "Confirmou integração do carregamento.", unitOfWork);
                        retorno.Objeto = true;
                        retorno.Status = true;

                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                        retorno.Status = true;
                        retorno.Mensagem = "A confirmação da integração já foi realizada anteriormente.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi encontrado um carregamento para o protocolo informado";
                }
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "A carga informada não existe no Multi Embarcador";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<bool> ConfirmarImpressaoCarga(int protocolo)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            try
            {

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocolo);
                if (carga != null)
                {
                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos)
                    {
                        carga.SituacaoCarga = configuracaoTMS.SituacaoCargaAposConfirmacaoImpressao;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Confirmou impressão dos documentos.", unitOfWork);

                        if (carga.SituacaoCarga == SituacaoCarga.EmTransporte)
                        {
                            new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unitOfWork).GerarIntegracaoNotificacao(carga, TipoNotificacaoApp.MotoristaPodeSeguirViagem);
                            if (configuracaoTMS.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte)
                                Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(carga, configuracaoTMS, null, "Carga em transporte", unitOfWork);
                        }

                        repCarga.Atualizar(carga);

                        retorno.Objeto = true;
                        retorno.Status = true;

                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                        retorno.Status = true;
                        retorno.Mensagem = "Situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite confirmar a impressão.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi encontrado uma carga para o protocolo informado";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "A carga informada não existe no Multi Embarcador";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<bool> ConfirmarIntegracaoCargaComCanhotosDigitalizados(int protocolo)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocolo);

                if (carga == null)
                    throw new WebServiceException("Não foi possível encontrar a carga com o protocolo informado.");

                Repositorio.Embarcador.Cargas.CargaConfirmacao repositorioCargaConfirmacao = new Repositorio.Embarcador.Cargas.CargaConfirmacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaConfirmacao cargaConfirmacao = repositorioCargaConfirmacao.BuscarPorCarga(carga.Codigo);

                if (cargaConfirmacao?.DataCorfirmacaoCanhotosDigitalizados.HasValue ?? false)
                    throw new WebServiceException($"A integração da carga foi confirmada em {cargaConfirmacao.DataCorfirmacaoCanhotosDigitalizados.Value.ToString("dd/MM/yyyy")} as {cargaConfirmacao.DataCorfirmacaoCanhotosDigitalizados.Value.ToString("HH:mm")} horas.");

                if (cargaConfirmacao == null)
                {
                    cargaConfirmacao = new Dominio.Entidades.Embarcador.Cargas.CargaConfirmacao()
                    {
                        Carga = carga,
                        DataCorfirmacaoCanhotosDigitalizados = DateTime.Now
                    };

                    repositorioCargaConfirmacao.Inserir(cargaConfirmacao);
                }
                else
                {
                    cargaConfirmacao.DataCorfirmacaoCanhotosDigitalizados = DateTime.Now;

                    repositorioCargaConfirmacao.Atualizar(cargaConfirmacao);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração da carga com todos os canhotos digitalizados");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AlterarNumeroCarga(int? protocoloCarga, string numeroCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                AlterarNumeroCarga alterarNumeroCarga = new AlterarNumeroCarga()
                {
                    protocoloCarga = protocoloCarga ?? 0,
                    numeroCarga = numeroCarga
                };

                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AlterarNumeroCarga(alterarNumeroCarga));
            });
        }

        public Retorno<bool> RemoverOutrosNumerosCarga(int? protocoloCarga, string numeroCarga)
        {
            ValidarToken();

            protocoloCarga ??= 0;

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo((int)protocoloCarga);
                if (carga != null)
                {
                    string numeroCargaEmbarcador = Utilidades.String.Left(numeroCarga, 50);

                    if (carga.CodigosAgrupados != null && carga.CodigosAgrupados.Contains(numeroCargaEmbarcador))
                    {
                        carga.CodigosAgrupados.Remove(numeroCargaEmbarcador);

                        if (carga.CodigosAgrupados == null || carga.CodigosAgrupados.Count == 0)
                            carga.CargaPossuiOutrosNumerosEmbarcador = false;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Removeu o número da carga embarcador " + numeroCargaEmbarcador + ".", unitOfWork);

                        retorno.Objeto = true;
                        retorno.Status = true;
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Não foi encontrado o número " + numeroCargaEmbarcador + " na carga protocolo " + protocoloCarga.ToString();
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi encontrado uma carga para o protocolo informado";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "A carga informada não existe no MultiEmbarcador";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<bool> VincularCargas(List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> protocolos)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            try
            {
                List<int> codigos = (from o in protocolos select o.protocoloIntegracaoCarga).Distinct().ToList();
                //List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorCodigos(codigos);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorProtocolos(codigos);

                if (cargas.Count >= 2)
                {
                    serCarga.CriarVinculosEntreCargas(cargas, unitOfWork);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    unitOfWork.CommitChanges();
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "É necessário informar no mínimo duas cargas para vincular";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao vincular as cargas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<bool> EncerrarCarga(int protocoloIntegracaoCarga, string ObservacaoEncerramento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EncerrarCarga(protocoloIntegracaoCarga, ObservacaoEncerramento));
            });
        }

        public Retorno<int> AdicionarAgrupamentoDeCargas(Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.Agrupador Agrupador)
        {
            ValidarToken();
            Servicos.Log.TratarErro($"Agrupador: {(Agrupador != null ? Newtonsoft.Json.JsonConvert.SerializeObject(Agrupador) : string.Empty)}", "agrupador");
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec servicoIntegracaoOrtec = new Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec(unitOfWork);
                int protocolo = servicoIntegracaoOrtec.AdicionarPreAgrupamentoCarga(Agrupador, Auditado);

                return Retorno<int>.CriarRetornoSucesso(protocolo);
            }
            catch (ServicoException excecao)
            {
                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao adicionar o agrupamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<int> AgruparCargas(List<int> ProtocoloCargas)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<int>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AgruparCargas(ProtocoloCargas));
            });
        }

        public Retorno<bool> AtualizarPedido(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, int? protocoloCarga, int? protocoloPedido)
        {
            Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo = new Protocolos()
            {
                protocoloIntegracaoCarga = protocoloCarga ?? 0,
                protocoloIntegracaoPedido = protocoloPedido ?? 0
            };
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AtualizarCargaPadrao(cargaIntegracao, protocolo, unitOfWork.StringConexao));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AdicionarPedidoAsync(cargaIntegracao, true, default).Result);
            });
        }

        public async Task<Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> AdicionarCargaNovoAsync(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CreateFrom(await new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AdicionarPedidoAsync(cargaIntegracao, true, default));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> ConsultarCargaAtivaPorNumero(string numeroCarga)
        {
            Servicos.Log.TratarErro($"ConsultarCargaAtivaPorNumeroCarga: {numeroCarga}", "RequestLog");

            ValidarToken();

            StringBuilder mensagemErro = new StringBuilder();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaAtiva(numeroCarga);

                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo : 0, protocoloIntegracaoPedido = cargaPedido != null && cargaPedido.Pedido != null ? cargaPedido.Pedido.Codigo : 0 });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro($"Carga: {numeroCarga} retornou exceção a seguir:", "RequestLog");

                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoExcecao($"Ocorreu uma falha ao obter os dados das integrações. {mensagemErro.ToString()}");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AtualizarRolagemCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaAtualizacaoRolagem cargaAtualizacaoRolagem, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            Servicos.Log.TratarErro($"CargaAtualizacaoRolagem: {(cargaAtualizacaoRolagem != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cargaAtualizacaoRolagem) : string.Empty)}", "Request");

            Servicos.Log.TratarErro("Está chamando o autalizar rolagem de carga Pedido" + protocolo.protocoloIntegracaoCarga + " Carga" + protocolo.protocoloIntegracaoCarga, "RequestLog");
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            StringBuilder stMensagem = new StringBuilder();
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas,
            };

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);
            Repositorio.Embarcador.CTe.CTeContainerDocumento repCTeContainerDocumento = new Repositorio.Embarcador.CTe.CTeContainerDocumento(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntercab.BuscarIntegracao();
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga(unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido(unitOfWork);
            Servicos.Embarcador.Pedido.RolagemContainer serRolagemContainer = new Servicos.Embarcador.Pedido.RolagemContainer(unitOfWork, auditado, TipoServicoMultisoftware);

            if (protocolo.protocoloIntegracaoCarga > 0 && (protocolo.protocoloIntegracaoPedido > 0 || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
            {
                try
                {
                    unitOfWork.Start();
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);
                    carga.Initialize();

                    string numeroContainerPedido = "";
                    string numeroBookingAnterior = "";

                    if (carga != null)
                    {
                        if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        {
                            carga.PedidoViagemNavio = serPedidoWS.SalvarViagem(cargaAtualizacaoRolagem.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                            carga.PortoOrigem = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoOrigem, ref stMensagem, Auditado);
                            carga.PortoDestino = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoDestino, ref stMensagem, Auditado);
                            carga.TerminalOrigem = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoOrigem, ref stMensagem, Auditado);
                            carga.TerminalDestino = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoDestino, ref stMensagem, Auditado);

                            if (stMensagem.Length == 0)
                            {
                                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repCargaPedido.BuscarPedidosPorCarga(carga.Codigo);
                                if (pedidos != null && pedidos.Count > 0)
                                {
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                                    {
                                        numeroContainerPedido = pedido.Container?.Numero ?? "";
                                        numeroBookingAnterior = pedido.NumeroBooking;
                                        pedido.Initialize();

                                        pedido.PedidoViagemNavio = serPedidoWS.SalvarViagem(cargaAtualizacaoRolagem.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                        pedido.Porto = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoOrigem, ref stMensagem, Auditado);
                                        pedido.PortoDestino = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoDestino, ref stMensagem, Auditado);
                                        pedido.TerminalOrigem = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoOrigem, ref stMensagem, Auditado);
                                        pedido.TerminalDestino = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoDestino, ref stMensagem, Auditado);
                                        if (cargaAtualizacaoRolagem.Container != null)
                                        {
                                            pedido.Container = serPedidoWS.SalvarContainer(cargaAtualizacaoRolagem.Container, ref stMensagem, Auditado);
                                            if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.Container.Lacre1))
                                                pedido.LacreContainerUm = cargaAtualizacaoRolagem.Container.Lacre1;
                                            if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.Container.Lacre2))
                                                pedido.LacreContainerDois = cargaAtualizacaoRolagem.Container.Lacre2;
                                            if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.Container.Lacre3))
                                                pedido.LacreContainerTres = cargaAtualizacaoRolagem.Container.Lacre3;
                                        }

                                        if (cargaAtualizacaoRolagem.Transbordo != null && cargaAtualizacaoRolagem.Transbordo.Count > 0)
                                            new Servicos.WebService.Carga.ProdutosPedido(unitOfWork).SalvarTransbordo(pedido, cargaAtualizacaoRolagem.Transbordo, ref stMensagem, unitOfWork, unitOfWork.StringConexao, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                        else
                                            new Servicos.WebService.Carga.ProdutosPedido(unitOfWork).RemoverTransbordo(pedido, cargaAtualizacaoRolagem.Transbordo, ref stMensagem, unitOfWork, unitOfWork.StringConexao, Auditado);

                                        if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.NumeroBooking) && pedido.NumeroBooking != cargaAtualizacaoRolagem.NumeroBooking)
                                            pedido.NumeroBooking = cargaAtualizacaoRolagem.NumeroBooking;

                                        repPedido.Atualizar(pedido, Auditado);
                                    }
                                }
                                if (stMensagem.Length == 0)
                                {
                                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCTe.BuscarCTePorCarga(carga.Codigo);
                                    if (ctes != null && ctes.Count > 0)
                                    {
                                        foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                                        {
                                            Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe camposOriginaisCTe = PreencherObjetoCamposOriginais(cte);

                                            cte.Initialize();

                                            cte.Viagem = serPedidoWS.SalvarViagem(cargaAtualizacaoRolagem.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                            cte.PortoOrigem = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoOrigem, ref stMensagem, Auditado);
                                            cte.PortoDestino = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoDestino, ref stMensagem, Auditado);
                                            cte.TerminalOrigem = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoOrigem, ref stMensagem, Auditado);
                                            cte.TerminalDestino = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoDestino, ref stMensagem, Auditado);
                                            cte.Navio = cte.Viagem?.Navio;
                                            cte.Direcao = cte.Viagem != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodalHelper.ObterAbreviacao(cte.Viagem.DirecaoViagemMultimodal) : "";
                                            cte.NumeroViagem = cte.Viagem?.NumeroViagem.ToString("D") ?? "";

                                            if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.NumeroBooking) && cte.NumeroBooking != cargaAtualizacaoRolagem.NumeroBooking)
                                                cte.NumeroBooking = cargaAtualizacaoRolagem.NumeroBooking;

                                            if (cargaAtualizacaoRolagem.Transbordo != null && cargaAtualizacaoRolagem.Transbordo.Count > 0)
                                            {
                                                int contadorTransbordo = 0;
                                                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo transbordo in cargaAtualizacaoRolagem.Transbordo)
                                                {
                                                    if (contadorTransbordo == 0)
                                                    {
                                                        cte.PortoPassagemUm = serPedidoWS.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                                                        cte.ViagemPassagemUm = serPedidoWS.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                                    }
                                                    else if (contadorTransbordo == 1)
                                                    {
                                                        cte.PortoPassagemDois = serPedidoWS.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                                                        cte.ViagemPassagemDois = serPedidoWS.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                                    }
                                                    else if (contadorTransbordo == 2)
                                                    {
                                                        cte.PortoPassagemTres = serPedidoWS.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                                                        cte.ViagemPassagemTres = serPedidoWS.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                                    }
                                                    else if (contadorTransbordo == 4)
                                                    {
                                                        cte.PortoPassagemQuatro = serPedidoWS.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                                                        cte.ViagemPassagemQuatro = serPedidoWS.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                                    }
                                                    else if (contadorTransbordo == 5)
                                                    {
                                                        cte.PortoPassagemCinco = serPedidoWS.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                                                        cte.ViagemPassagemCinco = serPedidoWS.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                                    }
                                                    contadorTransbordo += 1;
                                                }
                                            }
                                            else
                                            {
                                                cte.PortoPassagemUm = null;
                                                cte.ViagemPassagemUm = null;

                                                cte.PortoPassagemDois = null;
                                                cte.ViagemPassagemDois = null;

                                                cte.PortoPassagemTres = null;
                                                cte.ViagemPassagemTres = null;

                                                cte.PortoPassagemQuatro = null;
                                                cte.ViagemPassagemQuatro = null;

                                                cte.PortoPassagemCinco = null;
                                                cte.ViagemPassagemCinco = null;
                                            }
                                            cte.ObservacoesGerais += "Alteração de dados via rolagem: ";

                                            if (cte.Viagem != null)
                                                cte.ObservacoesGerais += "Viagem: " + cte.Viagem.Descricao + " ";
                                            if (cte.PortoOrigem != null)
                                                cte.ObservacoesGerais += "Porto Origem: " + cte.PortoOrigem.Descricao + " ";
                                            if (cte.PortoDestino != null)
                                                cte.ObservacoesGerais += "Porto Destino: " + cte.PortoDestino.Descricao + " ";
                                            if (cte.TerminalOrigem != null)
                                                cte.ObservacoesGerais += "Terminal Origem: " + cte.TerminalOrigem.Descricao + " ";
                                            if (cte.TerminalDestino != null)
                                                cte.ObservacoesGerais += "Terminal Destino: " + cte.TerminalDestino.Descricao + " ";
                                            if (cte.PortoPassagemUm != null)
                                                cte.ObservacoesGerais += "Porto Passagem 1: " + cte.PortoPassagemUm.Descricao + " ";
                                            if (cte.PortoPassagemDois != null)
                                                cte.ObservacoesGerais += "Porto Passagem 2: " + cte.PortoPassagemDois.Descricao + " ";
                                            if (cte.PortoPassagemTres != null)
                                                cte.ObservacoesGerais += "Porto Passagem 3: " + cte.PortoPassagemTres.Descricao + " ";
                                            if (cte.PortoPassagemQuatro != null)
                                                cte.ObservacoesGerais += "Porto Passagem 4: " + cte.PortoPassagemQuatro.Descricao + " ";
                                            if (cte.PortoPassagemCinco != null)
                                                cte.ObservacoesGerais += "Porto Passagem 5: " + cte.PortoPassagemCinco.Descricao + " ";

                                            cte.PedidoViagemNavioSchedule = repPedidoNavioSchedule.BuscarPorCodigo(repCTe.BuscarCodigoSchedule(cte.ViagemPassagemCinco?.Codigo ?? 0, cte.ViagemPassagemQuatro?.Codigo ?? 0, cte.ViagemPassagemTres?.Codigo ?? 0, cte.ViagemPassagemDois?.Codigo ?? 0, cte.ViagemPassagemUm?.Codigo ?? 0, cte.Viagem?.Codigo ?? 0, cte.PortoDestino?.Codigo ?? 0, cte.TerminalDestino?.Codigo ?? 0));

                                            if (cargaAtualizacaoRolagem.Container != null)
                                            {
                                                Dominio.Entidades.ContainerCTE containerCTE = new Dominio.Entidades.ContainerCTE()
                                                {
                                                    Container = serPedidoWS.SalvarContainer(cargaAtualizacaoRolagem.Container, ref stMensagem, Auditado),
                                                    CTE = cte,
                                                    Lacre1 = cargaAtualizacaoRolagem.Lacre1,
                                                    Lacre2 = cargaAtualizacaoRolagem.Lacre2,
                                                    Lacre3 = cargaAtualizacaoRolagem.Lacre3
                                                };
                                                if (containerCTE != null && containerCTE.Container != null)
                                                {
                                                    containerCTE.Numero = containerCTE.Container.Numero;
                                                    repContainerCTE.Inserir(containerCTE);

                                                    if (cte.Documentos != null && cte.Documentos.Count > 0)
                                                    {
                                                        foreach (Dominio.Entidades.DocumentosCTE nota in cte.Documentos)
                                                        {
                                                            Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento containerDocumento = new Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento()
                                                            {
                                                                DocumentosCTE = nota,
                                                                Chave = nota.ChaveNFE,
                                                                ContainerCTE = containerCTE,
                                                                Numero = !string.IsNullOrEmpty(nota.Numero) ? nota.Numero : "1",
                                                                Serie = !string.IsNullOrEmpty(nota.Serie) ? nota.Serie : "1",
                                                                TipoDocumento = !string.IsNullOrEmpty(nota.ChaveNFE) && nota.ChaveNFE.Length == 44 ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.NF,
                                                                UnidadeMedidaRateada = 0
                                                            };

                                                            if (!string.IsNullOrWhiteSpace(containerDocumento.Chave))
                                                                containerDocumento.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(containerDocumento.Chave);

                                                            repCTeContainerDocumento.Inserir(containerDocumento);
                                                        }
                                                    }
                                                }
                                            }

                                            repCTe.Atualizar(cte, Auditado);

                                            if (integracaoIntercab?.AtivarGeracaoCCePelaRolagemWS ?? false)
                                            {
                                                List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> camposCartaCorrecao = GerarCamposCartaCorrecao(cargaAtualizacaoRolagem, camposOriginaisCTe, carga.PedidoViagemNavio, cte.PortoPassagemUm);
                                                if (camposCartaCorrecao.Count > 0)
                                                    serRolagemContainer.GerarCartaCorrecao(cte, camposCartaCorrecao, camposOriginaisCTe);
                                            }

                                            serRolagemContainer.ReenviarIntegracaoFaturaEMP(cte);

                                            List<Dominio.Entidades.Embarcador.Fatura.Fatura> faturasCTe = repFatura.BuscarFaturasPorNumeroCTeSituacoes(cte.Codigo);
                                            foreach (Dominio.Entidades.Embarcador.Fatura.Fatura fatura in faturasCTe)
                                            {
                                                if (fatura.PedidoViagemNavio != null && cte.Viagem != null)
                                                    fatura.PedidoViagemNavio = cte.Viagem;

                                                if (!string.IsNullOrWhiteSpace(fatura.NumeroBooking) && !string.IsNullOrWhiteSpace(cte.NumeroBooking))
                                                    fatura.NumeroBooking = cte.NumeroBooking;

                                                fatura.RolagemCarga = true;

                                                repFatura.Atualizar(fatura);
                                            }
                                        }
                                    }
                                }
                            }

                            carga.RolagemCarga = true;
                            repCarga.Atualizar(carga, Auditado);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasSVM = repCargaPedido.BuscarCargasSVMPorContainerEBooking(cargaAtualizacaoRolagem.Container?.Numero ?? numeroContainerPedido, numeroBookingAnterior);

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaSVM in cargasSVM)
                            {
                                cargaSVM.Carga.Initialize();
                                cargaSVM.Pedido.Initialize();

                                if (cargaAtualizacaoRolagem.Viagem != null)
                                    cargaSVM.Carga.PedidoViagemNavio = serPedidoWS.SalvarViagem(cargaAtualizacaoRolagem.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                if (cargaAtualizacaoRolagem.PortoOrigem != null)
                                    cargaSVM.Carga.PortoOrigem = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoOrigem, ref stMensagem, Auditado);
                                if (cargaAtualizacaoRolagem.PortoDestino != null)
                                    cargaSVM.Carga.PortoDestino = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoDestino, ref stMensagem, Auditado);
                                if (cargaAtualizacaoRolagem.TerminalPortoOrigem != null)
                                    cargaSVM.Carga.TerminalOrigem = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoOrigem, ref stMensagem, Auditado);
                                if (cargaAtualizacaoRolagem.TerminalPortoDestino != null)
                                    cargaSVM.Carga.TerminalDestino = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoDestino, ref stMensagem, Auditado);

                                if (cargaAtualizacaoRolagem.Viagem != null)
                                    cargaSVM.Pedido.PedidoViagemNavio = serPedidoWS.SalvarViagem(cargaAtualizacaoRolagem.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                if (cargaAtualizacaoRolagem.PortoOrigem != null)
                                    cargaSVM.Pedido.Porto = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoOrigem, ref stMensagem, Auditado);
                                if (cargaAtualizacaoRolagem.PortoDestino != null)
                                    cargaSVM.Pedido.PortoDestino = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoDestino, ref stMensagem, Auditado);
                                if (cargaAtualizacaoRolagem.TerminalPortoOrigem != null)
                                    cargaSVM.Pedido.TerminalOrigem = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoOrigem, ref stMensagem, Auditado);
                                if (cargaAtualizacaoRolagem.TerminalPortoDestino != null)
                                    cargaSVM.Pedido.TerminalDestino = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoDestino, ref stMensagem, Auditado);

                                if (cargaAtualizacaoRolagem.Container != null)
                                {
                                    cargaSVM.Pedido.Container = serPedidoWS.SalvarContainer(cargaAtualizacaoRolagem.Container, ref stMensagem, Auditado);
                                    if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.Container.Lacre1))
                                        cargaSVM.Pedido.LacreContainerUm = cargaAtualizacaoRolagem.Container.Lacre1;
                                    if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.Container.Lacre2))
                                        cargaSVM.Pedido.LacreContainerDois = cargaAtualizacaoRolagem.Container.Lacre2;
                                    if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.Container.Lacre3))
                                        cargaSVM.Pedido.LacreContainerTres = cargaAtualizacaoRolagem.Container.Lacre3;
                                }

                                if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.NumeroBooking) && cargaSVM.Pedido.NumeroBooking != cargaAtualizacaoRolagem.NumeroBooking)
                                    cargaSVM.Pedido.NumeroBooking = cargaAtualizacaoRolagem.NumeroBooking;

                                if (cargaAtualizacaoRolagem.Transbordo != null && cargaAtualizacaoRolagem.Transbordo.Count > 0)
                                    new Servicos.WebService.Carga.ProdutosPedido(unitOfWork).SalvarTransbordo(cargaSVM.Pedido, cargaAtualizacaoRolagem.Transbordo, ref stMensagem, unitOfWork, unitOfWork.StringConexao, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                else
                                    new Servicos.WebService.Carga.ProdutosPedido(unitOfWork).RemoverTransbordo(cargaSVM.Pedido, cargaAtualizacaoRolagem.Transbordo, ref stMensagem, unitOfWork, unitOfWork.StringConexao, Auditado);

                                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCTe.BuscarCTePorCarga(cargaSVM.Carga.Codigo);

                                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                                {
                                    if (cte.Status != "A")
                                        continue;

                                    Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe camposOriginaisCTe = PreencherObjetoCamposOriginais(cte);

                                    if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.NumeroBooking) && cte.NumeroBooking != cargaAtualizacaoRolagem.NumeroBooking)
                                        cte.NumeroBooking = cargaAtualizacaoRolagem.NumeroBooking;

                                    if (cargaAtualizacaoRolagem.Viagem != null)
                                    {
                                        cte.Viagem = serPedidoWS.SalvarViagem(cargaAtualizacaoRolagem.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                        cte.Navio = cte.Viagem?.Navio;
                                        cte.Direcao = cte.Viagem != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodalHelper.ObterAbreviacao(cte.Viagem.DirecaoViagemMultimodal) : "";
                                        cte.NumeroViagem = cte.Viagem?.NumeroViagem.ToString("D") ?? "";
                                    }
                                    if (cargaAtualizacaoRolagem.PortoOrigem != null)
                                        cte.PortoOrigem = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoOrigem, ref stMensagem, Auditado);
                                    if (cargaAtualizacaoRolagem.PortoDestino != null)
                                        cte.PortoDestino = serPedidoWS.SalvarPorto(cargaAtualizacaoRolagem.PortoDestino, ref stMensagem, Auditado);
                                    if (cargaAtualizacaoRolagem.TerminalPortoOrigem != null)
                                        cte.TerminalOrigem = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoOrigem, ref stMensagem, Auditado);
                                    if (cargaAtualizacaoRolagem.TerminalPortoDestino != null)
                                        cte.TerminalDestino = serPedidoWS.SalvarTerminalPorto(cargaAtualizacaoRolagem.TerminalPortoDestino, ref stMensagem, Auditado);

                                    if (cargaAtualizacaoRolagem.Container != null)
                                    {
                                        Dominio.Entidades.ContainerCTE containerCTE = new Dominio.Entidades.ContainerCTE()
                                        {
                                            Container = serPedidoWS.SalvarContainer(cargaAtualizacaoRolagem.Container, ref stMensagem, Auditado),
                                            CTE = cte,
                                            Lacre1 = cargaAtualizacaoRolagem.Lacre1,
                                            Lacre2 = cargaAtualizacaoRolagem.Lacre2,
                                            Lacre3 = cargaAtualizacaoRolagem.Lacre3
                                        };
                                        if (containerCTE != null && containerCTE.Container != null)
                                        {
                                            containerCTE.Numero = containerCTE.Container.Numero;
                                            repContainerCTE.Inserir(containerCTE);

                                            if (cte.Documentos != null && cte.Documentos.Count > 0)
                                            {
                                                foreach (Dominio.Entidades.DocumentosCTE nota in cte.Documentos)
                                                {
                                                    Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento containerDocumento = new Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento()
                                                    {
                                                        DocumentosCTE = nota,
                                                        Chave = nota.ChaveNFE,
                                                        ContainerCTE = containerCTE,
                                                        Numero = !string.IsNullOrEmpty(nota.Numero) ? nota.Numero : "1",
                                                        Serie = !string.IsNullOrEmpty(nota.Serie) ? nota.Serie : "1",
                                                        TipoDocumento = !string.IsNullOrEmpty(nota.ChaveNFE) && nota.ChaveNFE.Length == 44 ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.NF,
                                                        UnidadeMedidaRateada = 0
                                                    };

                                                    if (!string.IsNullOrWhiteSpace(containerDocumento.Chave))
                                                        containerDocumento.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(containerDocumento.Chave);

                                                    repCTeContainerDocumento.Inserir(containerDocumento);
                                                }
                                            }
                                        }
                                    }

                                    if (cargaAtualizacaoRolagem.Transbordo != null && cargaAtualizacaoRolagem.Transbordo.Count > 0)
                                    {
                                        int contadorTransbordo = 0;
                                        foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo transbordo in cargaAtualizacaoRolagem.Transbordo)
                                        {
                                            if (contadorTransbordo == 0)
                                            {
                                                cte.PortoPassagemUm = serPedidoWS.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                                                cte.ViagemPassagemUm = serPedidoWS.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                            }
                                            else if (contadorTransbordo == 1)
                                            {
                                                cte.PortoPassagemDois = serPedidoWS.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                                                cte.ViagemPassagemDois = serPedidoWS.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                            }
                                            else if (contadorTransbordo == 2)
                                            {
                                                cte.PortoPassagemTres = serPedidoWS.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                                                cte.ViagemPassagemTres = serPedidoWS.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                            }
                                            else if (contadorTransbordo == 4)
                                            {
                                                cte.PortoPassagemQuatro = serPedidoWS.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                                                cte.ViagemPassagemQuatro = serPedidoWS.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                            }
                                            else if (contadorTransbordo == 5)
                                            {
                                                cte.PortoPassagemCinco = serPedidoWS.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                                                cte.ViagemPassagemCinco = serPedidoWS.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                                            }
                                            contadorTransbordo += 1;
                                        }
                                    }
                                    else
                                    {
                                        cte.PortoPassagemUm = null;
                                        cte.ViagemPassagemUm = null;

                                        cte.PortoPassagemDois = null;
                                        cte.ViagemPassagemDois = null;

                                        cte.PortoPassagemTres = null;
                                        cte.ViagemPassagemTres = null;

                                        cte.PortoPassagemQuatro = null;
                                        cte.ViagemPassagemQuatro = null;

                                        cte.PortoPassagemCinco = null;
                                        cte.ViagemPassagemCinco = null;
                                    }
                                    cte.ObservacoesGerais += "Alteração de dados via rolagem: ";

                                    if (cte.Viagem != null)
                                        cte.ObservacoesGerais += "Viagem: " + cte.Viagem.Descricao + " ";
                                    if (cte.PortoOrigem != null)
                                        cte.ObservacoesGerais += "Porto Origem: " + cte.PortoOrigem.Descricao + " ";
                                    if (cte.PortoDestino != null)
                                        cte.ObservacoesGerais += "Porto Destino: " + cte.PortoDestino.Descricao + " ";
                                    if (cte.TerminalOrigem != null)
                                        cte.ObservacoesGerais += "Terminal Origem: " + cte.TerminalOrigem.Descricao + " ";
                                    if (cte.TerminalDestino != null)
                                        cte.ObservacoesGerais += "Terminal Destino: " + cte.TerminalDestino.Descricao + " ";
                                    if (cte.PortoPassagemUm != null)
                                        cte.ObservacoesGerais += "Porto Passagem 1: " + cte.PortoPassagemUm.Descricao + " ";
                                    if (cte.PortoPassagemDois != null)
                                        cte.ObservacoesGerais += "Porto Passagem 2: " + cte.PortoPassagemDois.Descricao + " ";
                                    if (cte.PortoPassagemTres != null)
                                        cte.ObservacoesGerais += "Porto Passagem 3: " + cte.PortoPassagemTres.Descricao + " ";
                                    if (cte.PortoPassagemQuatro != null)
                                        cte.ObservacoesGerais += "Porto Passagem 4: " + cte.PortoPassagemQuatro.Descricao + " ";
                                    if (cte.PortoPassagemCinco != null)
                                        cte.ObservacoesGerais += "Porto Passagem 5: " + cte.PortoPassagemCinco.Descricao + " ";

                                    cte.PedidoViagemNavioSchedule = repPedidoNavioSchedule.BuscarPorCodigo(repCTe.BuscarCodigoSchedule(cte.ViagemPassagemCinco?.Codigo ?? 0, cte.ViagemPassagemQuatro?.Codigo ?? 0, cte.ViagemPassagemTres?.Codigo ?? 0, cte.ViagemPassagemDois?.Codigo ?? 0, cte.ViagemPassagemUm?.Codigo ?? 0, cte.Viagem?.Codigo ?? 0, cte.PortoDestino?.Codigo ?? 0, cte.TerminalDestino?.Codigo ?? 0));

                                    repCTe.Atualizar(cte);

                                    if (integracaoIntercab?.AtivarGeracaoCCePelaRolagemWS ?? false)
                                    {
                                        List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> camposCartaCorrecao = GerarCamposCartaCorrecao(cargaAtualizacaoRolagem, camposOriginaisCTe, carga.PedidoViagemNavio, cte.PortoPassagemUm);
                                        if (camposCartaCorrecao.Count > 0)
                                            serRolagemContainer.GerarCartaCorrecao(cte, camposCartaCorrecao, camposOriginaisCTe);
                                    }
                                }

                                cargaSVM.Carga.RolagemCarga = true;
                                repCarga.Atualizar(cargaSVM.Carga, Auditado);
                                repPedido.Atualizar(cargaSVM.Pedido, Auditado);
                            }

                            List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> integracoesEMP = repCargaIntegracao.BuscarPorCarga(carga.Codigo, null, TipoIntegracao.EMP);
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao in integracoesEMP)
                            {
                                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                                repCargaIntegracao.Atualizar(integracao);
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "O status da carga ou os conhecimentos contidos na carga selecionada não permite a atualização dos seus dados via rolagem.";
                            retorno.Objeto = false;
                            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                            return retorno;
                        }

                        if (stMensagem.Length > 0)
                        {
                            Servicos.Log.TratarErro("Carga: " + cargaAtualizacaoRolagem.NumeroCarga + " Retornou essa mensagem: " + stMensagem.ToString(), "RequestLog");
                            unitOfWork.Rollback();
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = stMensagem.ToString();
                            retorno.Objeto = false;
                            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                            return retorno;
                        }
                        else
                        {
                            unitOfWork.CommitChanges();
                            retorno.Status = true;
                            if (repCargaCTe.ContemCTeAutorizadoPorCarga(carga.Codigo))
                            {
                                retorno.Mensagem = "Atenção, favor gerar a carta de correção para os conhecimentos autorizados para esta carga.";
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Atualizou a rolagem da carga e informado que se faz necessário a geração da carta de correção para os CT-es.", unitOfWork);
                            }
                            else
                            {
                                retorno.Mensagem = "";
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Atualizou a rolagem da carga.", unitOfWork);
                            }
                            retorno.Objeto = true;
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Não foi localizada nenhuma carga para o protocolo informado (" + protocolo.protocoloIntegracaoCarga + ").";
                        retorno.Objeto = false;
                        retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                        return retorno;
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    ArmazenarLogIntegracao(cargaAtualizacaoRolagem, unitOfWork);

                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Status = false;
                    Servicos.Log.TratarErro("Carga: " + cargaAtualizacaoRolagem.NumeroCarga + " Retornou exceção a seguir:");
                    retorno.Mensagem = "Ocorreu uma falha ao obter os dados das integrações. " + stMensagem.ToString();
                    retorno.Objeto = false;

                    return retorno;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            else
            {
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Status = false;
                retorno.Mensagem = "É obrigatório informar o protocolo da carga. ";
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> AtualizarCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AtualizarCargaPadrao(cargaIntegracao, protocolo, Conexao.createInstance(_serviceProvider).StringConexao)); ;
            });
        }

        public Retorno<bool> AtualizarValorFrete(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor valorFrete)
        {
            Servicos.Log.TratarErro($"AtualizarValorFrete - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | valorFrete {(valorFrete != null ? Newtonsoft.Json.JsonConvert.SerializeObject(valorFrete) : string.Empty)}", "Request");

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (protocolo.protocoloIntegracaoCarga <= 0)
                    throw new WebServiceException("O protocolo da carga não foi informado");

                if (valorFrete == null)
                    throw new WebServiceException("O valor de frete não foi informado");

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);

                if (carga == null)
                    throw new WebServiceException("Não foi possível encontrar a carga com o protocolo informado");

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    throw new WebServiceException($"A situação atual da carga ({carga.DescricaoSituacaoCarga}) não permite a atualização do valor de frete");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork, configuracaoEmbarcador);

                if (servicoCargaAprovacaoFrete.IsUtilizarAlcadaAprovacaoAlteracaoValorFrete() && (carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.Aprovada))
                    throw new WebServiceException("O valor de frete está aprovado e não pode mais ser alterado");

                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro servicoFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
                Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
                Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork, configuracaoEmbarcador);

                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFreteAuxiliar repCargaComponentesFreteAuxiliar = new Repositorio.Embarcador.Cargas.CargaComponentesFreteAuxiliar(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = carga.CargaAgrupamento != null ? repositorioCargaPedido.BuscarPorCargaOrigem(carga.Codigo) : repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                carga.MotivoPendencia = "";
                carga.PossuiPendencia = false;
                carga.TipoFreteEscolhido = TipoFreteEscolhido.Operador;
                carga.ValorFrete = valorFrete.FreteProprio;

                List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentesCarga = repCargaComponentesFrete.BuscarPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componenteCarga in componentesCarga)
                    repCargaComponentesFrete.Deletar(componenteCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar> componentesCargaAuxiliar = repCargaComponentesFreteAuxiliar.BuscarPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar componenteCargaAuxiliar in componentesCargaAuxiliar)
                    repCargaComponentesFreteAuxiliar.Deletar(componenteCargaAuxiliar);

                if (valorFrete.ComponentesAdicionais != null && valorFrete.ComponentesAdicionais.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional in valorFrete.ComponentesAdicionais)
                    {
                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;
                        if (componenteAdicional.Componente.TipoComponenteFrete != TipoComponenteFrete.OUTROS && componenteAdicional.Componente.TipoComponenteFrete != TipoComponenteFrete.TODOS)
                            componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(componenteAdicional.Componente.TipoComponenteFrete);
                        else
                            componenteFrete = repComponenteFrete.buscarPorCodigoEmbarcador(componenteAdicional.Componente.CodigoIntegracao);

                        if (componenteFrete != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFrete = repCargaComponentesFrete.BuscarPrimeiroPorCargaPorCompomente(carga.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete);
                            bool inserir = false;
                            if (cargaComponentesFrete == null)
                            {
                                cargaComponentesFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();
                                inserir = true;
                            }
                            cargaComponentesFrete.ComponenteFrete = componenteFrete;
                            cargaComponentesFrete.IncluirBaseCalculoICMS = true;
                            cargaComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                            if (componenteFrete.ImprimirOutraDescricaoCTe)
                                cargaComponentesFrete.OutraDescricaoCTe = componenteFrete.DescricaoCTe;

                            cargaComponentesFrete.ComponenteFilialEmissora = false;
                            cargaComponentesFrete.Carga = carga;
                            cargaComponentesFrete.TipoComponenteFrete = componenteFrete.TipoComponenteFrete;
                            cargaComponentesFrete.ValorComponente += componenteAdicional.ValorComponente;

                            if (inserir)
                                repCargaComponentesFrete.Inserir(cargaComponentesFrete);
                            else
                                repCargaComponentesFrete.Atualizar(cargaComponentesFrete);

                            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar cargaComponentesFreteAuxiliar = repCargaComponentesFreteAuxiliar.BuscarPrimeiroPorCargaPorCompomente(carga.Codigo, componenteFrete);
                            bool inserirAuxiliar = false;
                            if (cargaComponentesFreteAuxiliar == null)
                            {
                                cargaComponentesFreteAuxiliar = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar();
                                inserirAuxiliar = true;
                            }
                            cargaComponentesFreteAuxiliar.ComponenteFrete = componenteFrete;
                            cargaComponentesFreteAuxiliar.Carga = carga;
                            cargaComponentesFreteAuxiliar.ValorComponente += componenteAdicional.ValorComponente;

                            if (inserirAuxiliar)
                                repCargaComponentesFreteAuxiliar.Inserir(cargaComponentesFreteAuxiliar);
                            else
                                repCargaComponentesFreteAuxiliar.Atualizar(cargaComponentesFreteAuxiliar);

                        }
                        else
                            throw new WebServiceException("Não existe um componente de frete cadastrado do tipo ." + componenteAdicional.Componente.CodigoIntegracao);
                    }
                }

                if (valorFrete.ICMS != null)
                {
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(TipoComponenteFrete.ICMS);
                    if (componenteFrete != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar cargaComponentesFreteAuxiliar = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar();
                        cargaComponentesFreteAuxiliar.ComponenteFrete = componenteFrete;
                        cargaComponentesFreteAuxiliar.Carga = carga;
                        cargaComponentesFreteAuxiliar.ValorComponente = valorFrete.ICMS.ValorICMS;
                        repCargaComponentesFreteAuxiliar.Inserir(cargaComponentesFreteAuxiliar);
                    }
                }

                servicoRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);

                if (carga.EmpresaFilialEmissora != null || carga.CalcularFreteCliente)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = Servicos.Embarcador.Carga.FreteFilialEmissora.CalcularFreteFilialEmissora(carga, false, false, true, unitOfWork, TipoServicoMultisoftware, configuracaoEmbarcador, configuracaoPedido);

                    if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador && (retornoFrete.situacao == SituacaoRetornoDadosFrete.ProblemaCalcularFrete))
                        servicoCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Frete.NaoFoiPossivelCalcularFreteCarga, carga.CodigoCargaEmbarcador), unitOfWork);
                }
                else if (carga.TipoOperacao != null && carga.TipoOperacao.EmiteCTeFilialEmissora && carga.Filial != null && carga.Filial.EmpresaEmissora != null)
                    Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref carga, false, TipoServicoMultisoftware, unitOfWork, configuracaoEmbarcador);

                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = (Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Operador", $" Valor Informado = {carga.ValorFrete.ToString("n2")}", carga.ValorFrete, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Operador", 0, carga.ValorFrete));
                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicaoFrete, unitOfWork, null);
                servicoFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, TipoFreteEscolhido.Operador, unitOfWork, false, TipoServicoMultisoftware, unitOfWork.StringConexao);

                carga.ValorFreteOperador = carga.ValorFrete;                

                if (!configuracaoGeralCarga.NaoAplicarICMSMetodoAtualizarFrete)
                {
                    carga.ValorFreteAPagar = carga.ValorFrete + (valorFrete.ICMS?.ValorICMS ?? 0);
                    carga.ValorICMS = valorFrete.ICMS?.ValorICMS ?? 0;
                }
                else
                {
                    carga.ValorFreteAPagar = carga.ValorFrete;
                }               

                repositorioCarga.Atualizar(carga);

                servicoCargaJanelaCarregamento.AtualizarSituacao(carga, TipoServicoMultisoftware);
                servicoCargaAprovacaoFrete.CriarAprovacao(carga, TipoRegraAutorizacaoCarga.InformadoManualmente, TipoServicoMultisoftware);
                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, configuracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Atualizou valores de frete.", unitOfWork);

                unitOfWork.CommitChanges();

                if (carga.CargaAgrupamento != null)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupamento = carga.CargaAgrupamento;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosAgrupados = repositorioCargaPedido.BuscarPorCarga(cargaAgrupamento.Codigo);

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupadas = repositorioCarga.BuscarCargasOriginais(cargaAgrupamento.Codigo);

                    cargaAgrupamento.MotivoPendencia = "";
                    cargaAgrupamento.PossuiPendencia = false;
                    cargaAgrupamento.TipoFreteEscolhido = TipoFreteEscolhido.Operador;
                    cargaAgrupamento.ValorFrete = cargasAgrupadas.Sum(o => o.ValorFreteOperador);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentesCargaAgrupamento = repCargaComponentesFrete.BuscarPorCarga(cargaAgrupamento.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componenteCarga in componentesCargaAgrupamento)
                        repCargaComponentesFrete.Deletar(componenteCarga);

                    decimal valorICMSTotal = 0;
                    decimal valorTotalComponentes = 0;
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada in cargasAgrupadas)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar> componentesCargasAgrupadas = repCargaComponentesFreteAuxiliar.BuscarPorCarga(cargaAgrupada.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar componeteCargaAgrupada in componentesCargasAgrupadas)
                        {
                            valorTotalComponentes += componeteCargaAgrupada.ValorComponente;

                            if (componeteCargaAgrupada.ComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS)
                                valorICMSTotal += componeteCargaAgrupada.ValorComponente;
                            else
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFrete = repCargaComponentesFrete.BuscarPrimeiroPorCargaPorCompomente(cargaAgrupamento.Codigo, componeteCargaAgrupada.ComponenteFrete.TipoComponenteFrete, componeteCargaAgrupada.ComponenteFrete);

                                bool inserir = false;
                                if (cargaComponentesFrete == null)
                                {
                                    cargaComponentesFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();
                                    inserir = true;
                                }

                                cargaComponentesFrete.ComponenteFrete = componeteCargaAgrupada.ComponenteFrete;
                                cargaComponentesFrete.IncluirBaseCalculoICMS = true;
                                cargaComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                                if (componeteCargaAgrupada.ComponenteFrete.ImprimirOutraDescricaoCTe)
                                    cargaComponentesFrete.OutraDescricaoCTe = componeteCargaAgrupada.ComponenteFrete.DescricaoCTe;
                                cargaComponentesFrete.ComponenteFilialEmissora = false;
                                cargaComponentesFrete.Carga = cargaAgrupamento;
                                cargaComponentesFrete.TipoComponenteFrete = componeteCargaAgrupada.ComponenteFrete.TipoComponenteFrete;
                                cargaComponentesFrete.ValorComponente += componeteCargaAgrupada.ValorComponente;


                                if (inserir)
                                    repCargaComponentesFrete.Inserir(cargaComponentesFrete);
                                else
                                    repCargaComponentesFrete.Atualizar(cargaComponentesFrete);
                            }
                        }
                    }


                    servicoRateioFrete.RatearValorDoFrenteEntrePedidos(cargaAgrupamento, cargaPedidosAgrupados, configuracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);

                    if (cargaAgrupamento.EmpresaFilialEmissora != null || cargaAgrupamento.CalcularFreteCliente)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = Servicos.Embarcador.Carga.FreteFilialEmissora.CalcularFreteFilialEmissora(cargaAgrupamento, false, false, true, unitOfWork, TipoServicoMultisoftware, configuracaoEmbarcador, configuracaoPedido);

                        if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador && (retornoFrete.situacao == SituacaoRetornoDadosFrete.ProblemaCalcularFrete))
                            servicoCarga.NotificarAlteracaoAoOperador(cargaAgrupamento, $"Não foi possível calcular o frete da carga n° {cargaAgrupamento.CodigoCargaEmbarcador}", unitOfWork);
                    }
                    else if (cargaAgrupamento.TipoOperacao != null && cargaAgrupamento.TipoOperacao.EmiteCTeFilialEmissora && cargaAgrupamento.Filial != null && cargaAgrupamento.Filial.EmpresaEmissora != null)
                        Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref cargaAgrupamento, false, TipoServicoMultisoftware, unitOfWork, configuracaoEmbarcador);

                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFreteCargaAgrupada = (Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Operador", $" Valor Informado = {cargaAgrupamento.ValorFrete.ToString("n2")}", cargaAgrupamento.ValorFrete, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Operador", 0, cargaAgrupamento.ValorFrete));
                    Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(cargaAgrupamento, null, null, null, false, composicaoFreteCargaAgrupada, unitOfWork, null);
                    servicoFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(cargaAgrupamento, TipoFreteEscolhido.Operador, unitOfWork, false, TipoServicoMultisoftware, unitOfWork.StringConexao);

                    if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().AtualizarValorFrete_AtualizarICMS.Value)
                    {
                        cargaAgrupamento.ValorFreteAPagar = cargaAgrupamento.ValorFrete + valorTotalComponentes;
                        cargaAgrupamento.ValorICMS = valorICMSTotal;

                        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componenteICMSCarga = repCargaComponentesFrete.BuscarPorCargaPorCompomente(cargaAgrupamento.Codigo, TipoComponenteFrete.ICMS, null);
                        if (componenteICMSCarga != null && componenteICMSCarga.Count > 0)
                        {
                            componenteICMSCarga.FirstOrDefault().ValorComponente = valorICMSTotal;
                            repCargaComponentesFrete.Atualizar(componenteICMSCarga.FirstOrDefault());
                        }

                    }

                    cargaAgrupamento.ValorFreteOperador = cargaAgrupamento.ValorFrete;

                    repositorioCarga.Atualizar(cargaAgrupamento);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaAgrupamento, "Atualizou valores de frete nas cargas origens.", unitOfWork);

                    unitOfWork.CommitChanges();
                }



                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao atualizar o valor de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AtualizarStatusPreCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.WebService.GestaoPatio.AtualizacaoStatusEtapa> etapas)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (protocolo.protocoloIntegracaoCarga <= 0)
                    throw new WebServiceException("O protocolo da carga não foi informado");

                if (etapas == null || etapas.Count == 0)
                    throw new WebServiceException("Nenhuma etapa informada");

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCarregamentoSituacao repPedidoSituacao = new Repositorio.Embarcador.Pedidos.PedidoCarregamentoSituacao(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);

                Servicos.Embarcador.Logistica.CentroCarregamento servicoCentroCarregamento = new Servicos.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                if (carga == null)
                    throw new WebServiceException("Não foi possível encontrar a carga com o protocolo informado");

                if (carga.Filial == null)
                    throw new WebServiceException("Carga não possui filial");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoPatio.ObterFluxoGestaoPatio(carga);

                if (fluxoGestaoPatio == null)
                    throw new WebServiceException("Fluxo de pátio não foi encontrado");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);

                if (sequenciaGestaoPatio == null)
                    throw new WebServiceException($"Gestão de pátio não está configurado na filial ${fluxoGestaoPatio.Filial.Descricao}");

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(carga.Filial.Codigo);

                List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AtualizacaoEtapaPatio> etapasAtualizar = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterEtapasAtualizacao(TipoFluxoGestaoPatio.Origem, carga.Filial.Codigo, carga.TipoOperacao?.Codigo ?? 0, etapas);

                string codigoIntegracaoBloqueiaCarregamento = "00";
                string codigoIntegracaoChegadaVeiculo = "01";
                string codigoIntegracaoCarregamento = "10";
                string codigoIntegracaoFaturamento = "11";

                bool contemEtapaBloqueioCarregamento = etapas.Any(obj => obj.Status == codigoIntegracaoBloqueiaCarregamento);
                bool contemEtapaChegadaVeiculo = etapas.Any(obj => obj.Status == codigoIntegracaoChegadaVeiculo && obj.Data != null && obj.Hora != null);
                bool contemEtapaCarregamento = etapas.Any(obj => obj.Status == codigoIntegracaoCarregamento && obj.Data != null && obj.Hora != null);
                bool contemEtapaFaturamento = etapas.Any(obj => obj.Status == codigoIntegracaoFaturamento && obj.Data != null && obj.Hora != null);
                bool bloquarCarregamentoPortalRetira = contemEtapaBloqueioCarregamento && carga.Carregamento != null;

                if (bloquarCarregamentoPortalRetira)
                {
                    carga.Carregamento.SituacaoCarregamento = SituacaoCarregamento.Bloqueado;

                    if (centroCarregamento?.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado ?? false)
                        servicoCentroCarregamento.EnviarNotificacaoConfirmacaoCarga(carga.Carregamento.Codigo, TipoServicoMultisoftware, Cliente.Codigo, Conexao.createInstance(_serviceProvider).AdminStringConexao, unitOfWork);

                    repCarregamento.Atualizar(carga.Carregamento);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Solicitou o bloqueio do carregamento", unitOfWork);

                }
                if (carga.Carregamento != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido pedido in carga.Carregamento.Pedidos)
                    {
                        if (contemEtapaChegadaVeiculo)
                        {
                            pedido.Pedido.SituacaoAtualPedidoRetirada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada.Remessa;
                            pedido.Pedido.PedidoCarregamentoSituacao.SituacaoAtualPedidoRetirada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada.Remessa;
                            pedido.Pedido.PedidoCarregamentoSituacao.DataRemessaConcluida = DateTime.Now;

                        }
                        if (contemEtapaCarregamento)
                        {
                            pedido.Pedido.PedidoCarregamentoSituacao.SituacaoAtualPedidoRetirada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada.Carregamento;
                            pedido.Pedido.SituacaoAtualPedidoRetirada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada.Carregamento;
                            pedido.Pedido.PedidoCarregamentoSituacao.DataCarregamentoConcluido = DateTime.Now;
                        }
                        if (contemEtapaFaturamento)
                        {
                            pedido.Pedido.PedidoCarregamentoSituacao.SituacaoAtualPedidoRetirada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada.Faturamento;
                            pedido.Pedido.SituacaoAtualPedidoRetirada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada.Faturamento;
                            pedido.Pedido.PedidoCarregamentoSituacao.DataFaturamentoConcluido = DateTime.Now;
                        }

                        repPedidoSituacao.Atualizar(pedido.Pedido.PedidoCarregamentoSituacao);
                        repPedido.Atualizar(pedido.Pedido);
                    }
                }


                foreach (Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AtualizacaoEtapaPatio etapa in etapasAtualizar)
                {
                    if (!etapa.DataHora.HasValue)
                    {
                        servicoFluxoPatio.VoltarAteEtapa(fluxoGestaoPatio, etapa.Etapa.Etapa);
                        break;
                    }

                    servicoFluxoPatio.LiberarProximaEtapa(fluxoGestaoPatio, etapa.Etapa.Etapa, etapa.DataHora.Value);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao atualizar o fluxo de pátio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarIntegracaoCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoCarga(protocolo)); ;
            });
        }

        public Retorno<bool> LiberarEmissaoSemNFe(int protocoloIntegracaoCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocoloIntegracaoCarga);

                if (carga == null)
                    throw new WebServiceException("A carga informada não existe no Multi Embarcador");

                if (carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    throw new WebServiceException("A carga informada não esta pendente de NF-e no MultiEmbarcador");

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                servicoCarga.LiberarCargaSemNFe(carga, cargaPedidos, configuracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, Auditado);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (WebServiceException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao solicitar a liberação da carga");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> BloquearCancelamentoCarga(int? protocoloIntegracaoCarga, int? codigoBooking, string numeroBooking, int? codigoOrdemServico, string numeroOrgemServico)
        {
            Servicos.Log.TratarErro("BloquearCancelamentoCarga - protocoloIntegracaoCarga: " + protocoloIntegracaoCarga.ToString(), "RequestLog");
            ValidarToken();

            protocoloIntegracaoCarga ??= 0;
            codigoBooking ??= 0;
            codigoOrdemServico ??= 0;

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (protocoloIntegracaoCarga > 0 || codigoBooking > 0 || !string.IsNullOrWhiteSpace(numeroBooking) || codigoOrdemServico > 0 || !string.IsNullOrWhiteSpace(numeroOrgemServico))
                {
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarCargasPorProtocoloBookingOrdemServico((int)protocoloIntegracaoCarga, (int)codigoBooking, numeroBooking, (int)codigoOrdemServico, numeroOrgemServico);

                    if (cargas != null && cargas.Count > 0)
                    {
                        unitOfWork.Start();
                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                        {
                            carga.BloquearCancelamentoCarga = true;
                            repCarga.Atualizar(carga);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Realizou o bloqueio do cancelamento via WebService.", unitOfWork);
                        }
                        retorno.CodigoMensagem = 0;
                        retorno.Status = true;
                        retorno.Mensagem = "Solicitação de bloqueio de cancelamento realizada com sucesso.";
                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A carga informada não existe no Multi TMS, ou os parâmetros estão inválidos.";
                        Servicos.Log.TratarErro("BloquearCancelamentoCarga - retorno: " + retorno.Mensagem, "RequestLog");
                        AuditarRetornoDadosInvalidos(unitOfWork, retorno.Mensagem, protocoloIntegracaoCarga.ToString());
                        unitOfWork.Rollback();
                    }
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Favor informe ao menus um parametro para localizar a carga..";
                    Servicos.Log.TratarErro("BloquearCancelamentoCarga - retorno: " + retorno.Mensagem, "RequestLog");
                    AuditarRetornoDadosInvalidos(unitOfWork, retorno.Mensagem, protocoloIntegracaoCarga.ToString());
                    unitOfWork.Rollback();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao solicitar a bloquear a carga";
                Servicos.Log.TratarErro("BloquearCancelamentoCarga - retorno: " + retorno.Mensagem);
                unitOfWork.Rollback();
            }
            finally
            {
                unitOfWork.Dispose();
            }

            if (retorno.Status == true)
                Servicos.Log.TratarErro("BloquearCancelamentoCarga - retorno: Sucesso", "RequestLog");
            else
                Servicos.Log.TratarErro("BloquearCancelamentoCarga - retorno: " + retorno.Mensagem, "RequestLog");
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> InformarSinitroAvariaCarga(int? protocoloIntegracaoCarga, int? codigoBooking, string numeroBooking, int? codigoOrdemServico, string numeroOrgemServico, string chaveCTe)
        {
            Servicos.Log.TratarErro("InformarSinitroAvariaCarga - protocoloIntegracaoCarga: " + protocoloIntegracaoCarga.ToString(), "RequestLog");
            ValidarToken();

            protocoloIntegracaoCarga ??= 0;
            codigoBooking ??= 0;
            codigoOrdemServico ??= 0;

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (protocoloIntegracaoCarga > 0 || codigoBooking > 0 || !string.IsNullOrWhiteSpace(numeroBooking) || codigoOrdemServico > 0 || !string.IsNullOrWhiteSpace(numeroOrgemServico) || !string.IsNullOrWhiteSpace(chaveCTe))
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargas = repCargaCTe.BuscarCargasPorProtocoloBookingOrdemServico((int)protocoloIntegracaoCarga, (int)codigoBooking, numeroBooking, (int)codigoOrdemServico, numeroOrgemServico, chaveCTe);

                    if (cargas != null && cargas.Count > 0)
                    {
                        unitOfWork.Start();
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe carga in cargas)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(carga.CTe.Codigo);
                            cte.OcorreuSinistroAvaria = true;
                            repCTe.Atualizar(cte);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, "Registrou o sinistro/avaria pelo Webservice.", unitOfWork);
                        }
                        unitOfWork.CommitChanges();
                        retorno.CodigoMensagem = 0;
                        retorno.Status = true;
                        retorno.Mensagem = "Informação de sinistro e avaria realizada com sucesso.";
                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A carga/CT-e informada não existe no Multi TMS, ou os parâmetros estão inválidos.";
                        Servicos.Log.TratarErro("InformarSinitroAvariaCarga - retorno: " + retorno.Mensagem, "RequestLog");
                        AuditarRetornoDadosInvalidos(unitOfWork, retorno.Mensagem, protocoloIntegracaoCarga.ToString());
                        unitOfWork.Rollback();
                    }
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Favor informe ao menus um parametro para localizar a carga.";
                    Servicos.Log.TratarErro("InformarSinitroAvariaCarga - retorno: " + retorno.Mensagem, "RequestLog");
                    AuditarRetornoDadosInvalidos(unitOfWork, retorno.Mensagem, protocoloIntegracaoCarga.ToString());
                    unitOfWork.Rollback();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao registrar o sinistro/avaria";
                Servicos.Log.TratarErro("InformarSinitroAvariaCarga - retorno: " + retorno.Mensagem);
                unitOfWork.Rollback();
            }
            finally
            {
                unitOfWork.Dispose();
            }

            if (retorno.Status == true)
                Servicos.Log.TratarErro("InformarSinitroAvariaCarga - retorno: Sucesso");
            else
                Servicos.Log.TratarErro("InformarSinitroAvariaCarga - retorno: " + retorno.Mensagem);
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> AverbarCarga(int? protocoloIntegracaoCarga, int? codigoBooking, string numeroBooking, int? codigoOrdemServico, string numeroOrgemServico, Dominio.Enumeradores.FormaAverbacaoCTE? forma, decimal? novoValorMercadoria)
        {
            Servicos.Log.TratarErro("AverbarCarga - protocoloIntegracaoCarga: " + protocoloIntegracaoCarga.ToString(), "RequestLog");
            ValidarToken();

            protocoloIntegracaoCarga ??= 0;
            codigoBooking ??= 0;
            codigoOrdemServico ??= 0;
            forma ??= Dominio.Enumeradores.FormaAverbacaoCTE.Definitiva;
            novoValorMercadoria ??= 0m;

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (protocoloIntegracaoCarga > 0 || codigoBooking > 0 || !string.IsNullOrWhiteSpace(numeroBooking) || codigoOrdemServico > 0 || !string.IsNullOrWhiteSpace(numeroOrgemServico))
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
                    Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
                    Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
                    Repositorio.Embarcador.Transportadores.TransportadorAverbacao repTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarCargaPorProtocoloBookingOrdemServico((int)protocoloIntegracaoCarga, (int)codigoBooking, numeroBooking, (int)codigoOrdemServico, numeroOrgemServico);

                    if (carga != null)
                    {
                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiraCargaPedido = repCargaPedido.BuscarUltimaEntregaCarga(carga.Codigo);
                        Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados dadosSumarizados = repCargaDadosSumarizados.BuscarPorCodigo(carga.DadosSumarizados.Codigo, false);

                        List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> listaTransportadorAverbacao = null;
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCarga(carga.Codigo);
                        List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro = null;
                        if (primeiraCargaPedido.ApoliceSeguroAverbacao != null && primeiraCargaPedido.ApoliceSeguroAverbacao.Count() > 0)
                            apolicesSeguro = new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>(primeiraCargaPedido.ApoliceSeguroAverbacao);
                        else
                        {
                            if (carga.Empresa != null)
                                listaTransportadorAverbacao = repTransportadorAverbacao.BuscarPorTransportador(carga.Empresa.Codigo);
                            if ((listaTransportadorAverbacao == null || listaTransportadorAverbacao.Count == 0) && carga.TipoOperacao != null)
                                listaTransportadorAverbacao = repTransportadorAverbacao.BuscarPorTipoOperacao(carga.Empresa.Codigo, carga.TipoOperacao.Codigo);
                            if ((listaTransportadorAverbacao == null || listaTransportadorAverbacao.Count == 0) && carga.EmpresaFilialEmissora != null)
                                listaTransportadorAverbacao = repTransportadorAverbacao.BuscarPorTransportador(carga.EmpresaFilialEmissora.Codigo);
                            if (listaTransportadorAverbacao != null && listaTransportadorAverbacao.Count > 0)
                            {
                                apolicesSeguro = new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>();
                                foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao averbacao in listaTransportadorAverbacao)
                                {
                                    Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao apolice = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao()
                                    {
                                        ApoliceSeguro = averbacao.ApoliceSeguro,
                                        CargaPedido = primeiraCargaPedido,
                                        Desconto = 0
                                    };
                                    repApoliceSeguroAverbacao.Inserir(apolice);

                                    apolicesSeguro.Add(apolice);
                                }
                            }
                        }

                        if (apolicesSeguro != null && apolicesSeguro.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE in cargasCTe)
                            {
                                if (cargaCTE.CTe != null && cargaCTE.CTe.ModeloDocumentoFiscal.AverbarDocumento)
                                {
                                    foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao seguro in apolicesSeguro)
                                    {
                                        if (seguro.ApoliceSeguro.SeguradoraAverbacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido)
                                        {
                                            if (cargaCTE.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || seguro.ApoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM)
                                            {
                                                if (!repAverbacaoCTe.ContemAverbacaoAutorizada(cargaCTE.CTe.Codigo, (Dominio.Enumeradores.FormaAverbacaoCTE)forma))
                                                {
                                                    Dominio.Entidades.AverbacaoCTe averbaCTe = repAverbacaoCTe.BuscarAverbacaoPendenteIntegracao(cargaCTE.CTe.Codigo, (Dominio.Enumeradores.FormaAverbacaoCTE)forma);
                                                    if (averbaCTe == null)
                                                        averbaCTe = new Dominio.Entidades.AverbacaoCTe();

                                                    averbaCTe.Carga = cargaCTE.Carga;
                                                    averbaCTe.CTe = cargaCTE.CTe;
                                                    averbaCTe.ApoliceSeguroAverbacao = seguro;
                                                    averbaCTe.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                                                    averbaCTe.SeguradoraAverbacao = Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido;
                                                    averbaCTe.Desconto = seguro.Desconto.HasValue && seguro.Desconto.Value > 0 ? cargaCTE.CTe.ValorAReceber * (seguro.Desconto.Value / 100) : 0;
                                                    averbaCTe.Percentual = seguro.Desconto;
                                                    averbaCTe.Forma = (Dominio.Enumeradores.FormaAverbacaoCTE)forma;

                                                    if (cargaCTE.CTe.Status == "A")
                                                        averbaCTe.Status = Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao;
                                                    else
                                                        averbaCTe.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;

                                                    if (novoValorMercadoria > 0 && forma == Dominio.Enumeradores.FormaAverbacaoCTE.Definitiva)
                                                    {
                                                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTE.BuscarPorCodigo(cargaCTE.CTe.Codigo);
                                                        cte.ValorTotalMercadoria = (decimal)novoValorMercadoria;
                                                        repCTE.Atualizar(cte);
                                                    }

                                                    if (averbaCTe.Codigo > 0)
                                                        repAverbacaoCTe.Atualizar(averbaCTe);
                                                    else
                                                        repAverbacaoCTe.Inserir(averbaCTe);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            dadosSumarizados.PossuiAverbacaoCTe = true;
                            repCargaDadosSumarizados.Atualizar(dadosSumarizados);

                            retorno.CodigoMensagem = 0;
                            retorno.Status = true;
                            retorno.Mensagem = "Solicitação de averbação realizada com sucesso.";

                            unitOfWork.CommitChanges();
                        }
                        else
                        {
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Não foi localizada nenhuma apólice de seguro associada a carga ou a empresa emissora.";
                            Servicos.Log.TratarErro("AverbarCarga - retorno: " + retorno.Mensagem, "RequestLog");
                            AuditarRetornoDadosInvalidos(unitOfWork, retorno.Mensagem, protocoloIntegracaoCarga.ToString());
                            unitOfWork.Rollback();
                        }
                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A carga informada não existe no Multi TMS, ou os parâmetros estão inválidos.";
                        Servicos.Log.TratarErro("AverbarCarga - retorno: " + retorno.Mensagem, "RequestLog");
                        AuditarRetornoDadosInvalidos(unitOfWork, retorno.Mensagem, protocoloIntegracaoCarga.ToString());
                        unitOfWork.Rollback();
                    }
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Favor informe ao menus um parametro para localizar a carga..";
                    Servicos.Log.TratarErro("AverbarCarga - retorno: " + retorno.Mensagem, "RequestLog");
                    AuditarRetornoDadosInvalidos(unitOfWork, retorno.Mensagem, protocoloIntegracaoCarga.ToString());
                    unitOfWork.Rollback();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao solicitar a averbação da carga";
                Servicos.Log.TratarErro("AverbarCarga - retorno: " + retorno.Mensagem);
                unitOfWork.Rollback();
            }
            finally
            {
                unitOfWork.Dispose();
            }

            if (retorno.Status == true)
                Servicos.Log.TratarErro("AverbarCarga - retorno: Sucesso", "RequestLog");
            else
                Servicos.Log.TratarErro("AverbarCarga - retorno: " + retorno.Mensagem, "RequestLog");
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> FecharCarga(int protocoloIntegracaoCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Carga.ProtocoloIntegracao integracao = new ProtocoloIntegracao();


                integracao.protocoloIntegracaoCarga = protocoloIntegracaoCarga;
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).FecharCarga(integracao));
            });
        }

        public Retorno<bool> ConfirmarRecebimentoCancelamentoCarga(int protocoloIntegracaoCarga)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                //Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocoloIntegracaoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloIntegracaoCarga);

                if (carga != null)
                {
                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(carga.Codigo);
                        if (!cargaCancelamento.confirmacaoERP)
                        {
                            cargaCancelamento.confirmacaoERP = true;
                            repCargaCancelamento.Atualizar(cargaCancelamento);
                            retorno.Objeto = true;
                            retorno.Status = true;

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Confirmou o recebimento do cancelamento da carga.", unitOfWork);

                            unitOfWork.CommitChanges();
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                            retorno.Mensagem = "A confirmação do cancelamento já foi feita.";
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite confirmar o cancelamento";
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "A carga informada não existe no Multi Embarcador";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "A carga informada não existe no Multi Embarcador";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDosDocumentosDaCarga(int protocoloIntegracaoCarga, string motivoDoCancelamento, string usuarioERPSolicitouCancelamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SolicitarCancelamentoDosDocumentosDaCarga(protocoloIntegracaoCarga, motivoDoCancelamento, usuarioERPSolicitouCancelamento));
            }, RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDaCarga(int protocoloIntegracaoCarga, string motivoDoCancelamento, string usuarioERPSolicitouCancelamento, string controleIntegracaoEmbarcador)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SolicitarCancelamentoDaCarga(protocoloIntegracaoCarga, motivoDoCancelamento, usuarioERPSolicitouCancelamento, controleIntegracaoEmbarcador, false));
            }, RetornoSolicitacaoCancelamento.EmCancelamento);
        }

        public Retorno<RetornoSolicitacaoCancelamento> SolicitarCancelamentoDoPedido(int protocoloIntegracaoPedido, string motivoDoCancelamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<RetornoSolicitacaoCancelamento>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SolicitarCancelamentoDoPedido(protocoloIntegracaoPedido, motivoDoCancelamento));
            }, RetornoSolicitacaoCancelamento.EmCancelamento);
        }

        public Retorno<bool> SolicitarEnvioEmailDocumentosDaCarga(int protocoloIntegracaoCarga, string emails)
        {
            Servicos.Log.TratarErro("SolicitarEnvioEmailDocumentosDaCarga - protocoloIntegracaoCarga: " + protocoloIntegracaoCarga.ToString() + " Emails: " + emails, "RequestLog");
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloIntegracaoCarga);

                if (carga != null)
                {
                    if (Utilidades.Email.ValidarEmails(emails, ';'))
                    {
                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos ||
                            carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao ||
                            carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte ||
                            carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada)
                        {
                            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repCargaCTe.BuscarPorCarga(protocoloIntegracaoCarga);
                            if (listaCargaCTe != null && listaCargaCTe.Count > 0)
                            {
                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in listaCargaCTe)
                                {
                                    if (cargaCTe.CTe != null)
                                    {
                                        if (!svcCTe.EnviarEmail(cargaCTe.CTe.Codigo, cargaCTe.CTe.Empresa.Codigo, emails, unitOfWork))
                                            Servicos.Log.TratarErro("Falha ao solicitar e-mail carga CTe codigo: " + cargaCTe.CTe.Codigo, "RequestLog");
                                    }
                                }
                                retorno.Objeto = true;
                                retorno.Status = true;

                                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Solicitou envio por e-mail dos documentos da carga.", unitOfWork);
                            }
                            else
                            {
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                retorno.Mensagem = "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite solicitar envio dos documentos";
                            }
                        }
                        else
                        {
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite solicitar envio dos documentos";
                        }
                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "E-mails inválidos (" + emails + "), deve-se utilizar o separador ; quando mais de um e-mail";
                    }
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "A carga informada não existe no Multi Embarcador";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "A carga informada não existe no Multi Embarcador";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> AtualizarModeloVeicularCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular modeloVeicular, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                if (protocolo.protocoloIntegracaoCarga <= 0 || (protocolo.protocoloIntegracaoPedido <= 0 && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "É obrigatório informar o protocolo da carga.";
                    retorno.Objeto = false;
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);

                if (carga == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = $"Não foi localizada nenhuma carga para o protocolo informado ({protocolo.protocoloIntegracaoCarga}).";
                    retorno.Objeto = false;
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                carga.Initialize();

                if (modeloVeicular == null)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "É obrigatório informar o modelo veicular.";
                    retorno.Objeto = false;
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.buscarPorCodigoIntegracao(modeloVeicular.CodigoIntegracao);

                if (modeloVeicularCarga == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = $"Não foi localizado nenhum modelo veicular para o código de integração informado ({modeloVeicular.CodigoIntegracao}).";
                    retorno.Objeto = false;
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                if (!svcCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware) ||
                   (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = $"A situação da carga ({carga.SituacaoCarga.ObterDescricao()}) não permite que o modelo veicular seja alterado.";
                    retorno.Objeto = false;
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                unitOfWork.Start();

                carga.ModeloVeicularCarga = modeloVeicularCarga;

                repCarga.Atualizar(carga, Auditado);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCarga(carga.Codigo);
                    if (pedidos.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                        {
                            pedido.ModeloVeicularCarga = modeloVeicularCarga;
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Alterado o Modelo Veicular do Pedido através do método 'AtualizarModeloVeicularCarga'", unitOfWork);
                            repPedido.Atualizar(pedido, Auditado);
                        }
                    }
                }

                unitOfWork.CommitChanges();

                retorno.Status = true;
                retorno.Mensagem = "Modelo veicular da carga alterado com sucesso.";
                retorno.Objeto = true;
                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                return retorno;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao realizar a operação.";
                retorno.Objeto = false;
                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                return retorno;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SalvarProduto(produto, Auditado));
            });
        }

        public Retorno<bool> IniciarViagem(string chaveCTe, string dataInicioViagem, string nomeEmpurrador)
        {
            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();
            DateTime? data = dataInicioViagem.ToNullableDateTime();
            if (string.IsNullOrWhiteSpace(chaveCTe) || data == null || data == DateTime.MinValue)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = $"chaveCTe e dataInicioViagem são parâmetros obrigatórios.";
            }
            else
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
                try
                {

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;
                    try
                    {
                        cargaCTes = BuscarCargasPorChaveCTe(unitOfWork, chaveCTe);
                    }
                    catch (Exception e)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = e.Message;
                    }
                    int totalCargaCTes = cargaCTes?.Count ?? 0;
                    if (totalCargaCTes > 0)
                    {
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                        unitOfWork.Start();
                        int totalCargasIniciadas = 0;
                        for (int i = 0; i < totalCargaCTes; i++)
                        {
                            if (cargaCTes[i].Carga.DataInicioViagem == null)
                            {
                                totalCargasIniciadas++;

                                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas;

                                if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(cargaCTes[i].Carga.Codigo, data.Value, OrigemSituacaoEntrega.WebService, null, configuracaoEmbarcador, TipoServicoMultisoftware, Cliente, auditado, unitOfWork))
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTes[i].Carga, null, "Informou o início da viagem.", unitOfWork);

                                cargaCTes[i].Carga.NomeEmpurrador = !string.IsNullOrWhiteSpace(nomeEmpurrador) ? nomeEmpurrador : null;
                                repCarga.Atualizar(cargaCTes[i].Carga);

                            }
                        }
                        unitOfWork.CommitChanges();

                        if (totalCargasIniciadas > 0)
                        {
                            retorno.Status = true;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                            retorno.Mensagem = $"Início de viagem registrado para {totalCargasIniciadas} carga(s).";
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = $"Nenhum início de viagem registrado, {totalCargaCTes} carga(s) já iniciaram viagem anteriormente.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Mensagem = "Ocorreu uma falha ao iniciar a viagem.";
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> FinalizarViagem(string chaveCTe, string dataFinalViagem)
        {
            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();
            DateTime? data = dataFinalViagem.ToNullableDateTime();
            if (string.IsNullOrWhiteSpace(chaveCTe) || data == null || data == DateTime.MinValue)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = $"chaveCTe e dataFinalViagem são parâmetros obrigatórios.";
            }
            else
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
                try
                {

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;
                    try
                    {
                        cargaCTes = BuscarCargasPorChaveCTe(unitOfWork, chaveCTe);
                    }
                    catch (Exception e)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = e.Message;
                    }
                    int totalCargaCTes = cargaCTes?.Count ?? 0;
                    if (totalCargaCTes > 0)
                    {
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

                        int totalCargasFinalizadas = 0;
                        unitOfWork.Start();
                        for (int i = 0; i < totalCargaCTes; i++)
                        {
                            if (cargaCTes[i].Carga.DataFimViagem == null)
                            {
                                totalCargasFinalizadas++;

                                if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarViagem(cargaCTes[i].Carga.Codigo, data.Value, Auditado, TipoServicoMultisoftware, Cliente, OrigemSituacaoEntrega.WebService, unitOfWork))
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTes[i].Carga, null, "Informou o fim da viagem.", unitOfWork);
                            }
                        }
                        unitOfWork.CommitChanges();

                        if (totalCargasFinalizadas > 0)
                        {
                            retorno.Status = true;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                            retorno.Mensagem = $"Fim de viagem registrado para {totalCargasFinalizadas} carga(s).";
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = $"Nenhum fim de viagem registrado, {totalCargaCTes} carga(s) já finalizaram viagem anteriormente.";
                        }
                    }
                }
                catch (ServicoException excecao)
                {
                    unitOfWork.Rollback();
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Mensagem = excecao.Message;
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Mensagem = "Ocorreu uma falha ao finalizar a viagem.";
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> AtualizarPrevisaoEntrega(string chaveCTe, string dataPrevisaoEntrega)
        {
            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();
            DateTime? data = dataPrevisaoEntrega.ToNullableDateTime();
            if (string.IsNullOrWhiteSpace(chaveCTe) || data == null || data == DateTime.MinValue)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = $"chaveCTe e dataPrevisaoEntrega são parâmetros obrigatórios.";
            }
            else
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
                try
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;
                    try
                    {
                        cargaCTes = BuscarCargasPorChaveCTe(unitOfWork, chaveCTe);
                    }
                    catch (Exception e)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = e.Message;
                    }
                    int totalCargaCTes = cargaCTes?.Count ?? 0;
                    if (totalCargaCTes > 0)
                    {
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

                        bool cargasFinalizadas = false;
                        for (int i = 0; i < totalCargaCTes; i++)
                        {
                            if (cargaCTes[i].Carga.DataFimViagem != null)
                            {
                                cargasFinalizadas = true;
                                break;
                            }
                        }
                        if (cargasFinalizadas)
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = $"Carga(s) com viagem já finalizada.";
                        }
                        else
                        {
                            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

                            int totalPrevisoes = 0;
                            unitOfWork.Start();
                            for (int i = 0; i < totalCargaCTes; i++)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(cargaCTes[i].Carga.Codigo);
                                int totalCargaEntregas = cargaEntregas?.Count ?? 0;
                                if (totalCargaEntregas > 0)
                                {
                                    for (int j = 0; j < totalCargaEntregas; j++)
                                    {
                                        if (cargaEntregas[j].DataConfirmacao != null)
                                        {
                                            cargaEntregas[j].DataPrevista = data;
                                            repCargaEntrega.Atualizar(cargaEntregas[j]);
                                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntregas[j], repCargaEntrega, unitOfWork, configControleEntrega);
                                        }
                                    }
                                    totalPrevisoes++;
                                }
                            }
                            unitOfWork.CommitChanges();

                            if (totalPrevisoes > 0)
                            {
                                retorno.Status = true;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                                retorno.Mensagem = $"Previsão de entrega registrada para {totalPrevisoes} carga(s).";
                            }
                            else
                            {
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                retorno.Mensagem = $"Nenhuma previsão de entrega registrada.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Mensagem = "Ocorreu uma falha ao atualizar a previsão de chegada.";
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> GerarEncaixeRetira(Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Carregamento carregamentoIntegracao)
        {
            Servicos.Log.TratarErro($"GerarEncaixeRetira: {(carregamentoIntegracao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(carregamentoIntegracao) : string.Empty)}", "Request");

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                Servicos.Embarcador.Pedido.RetiradaProduto servicoRetiradaProduto = new Servicos.Embarcador.Pedido.RetiradaProduto(unitOfWork, TipoServicoMultisoftware, Cliente, configuracaoTMS, Auditado, ClienteAcesso.URLAcesso);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = servicoRetiradaProduto.PreencherCarregamentoIntegracao(carregamentoIntegracao);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = servicoRetiradaProduto.GerarCarga(carregamento.Codigo);

                List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>() { };
                string mensagemRetornoCarga = "";

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    mensagemRetornoCarga = $"CARGA:{carga.CodigoCargaEmbarcador}";

                    retorno.Add(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos()
                    {
                        protocoloIntegracaoCarga = carga.Protocolo,
                    });
                }

                unitOfWork.CommitChanges();

                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoSucesso(retorno, mensagemRetornoCarga);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "RequestLog");

                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoExcecao($"Ocorreu uma falha ao gerar o carregamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AtualizarPLPeObejtoPostalCorreios(List<Dominio.ObjetosDeValor.WebService.Carga.PostagemCorreios> postagensCorreios)
        {
            Servicos.Log.TratarErro($"AtualizarPLPeObejtoPostalCorreios: {(postagensCorreios != null ? Newtonsoft.Json.JsonConvert.SerializeObject(postagensCorreios) : string.Empty)}", "Request");
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                string mensagemValidacao = string.Empty;
                foreach (Dominio.ObjetosDeValor.WebService.Carga.PostagemCorreios postagem in postagensCorreios)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorNumeroPedidoEmbarcador(postagem.NumeroPedidoEmbarcador);
                    if (pedido == null)
                        mensagemValidacao += "Pedido " + postagem.NumeroPedidoEmbarcador + " não localizado; ";
                    else
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarCargaAtualPorPedido(pedido.Codigo);
                        if (cargaPedido != null && cargaPedido.Carga.SituacaoCarga != SituacaoCarga.Nova && cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe && cargaPedido.Carga.SituacaoCarga != SituacaoCarga.CalculoFrete && cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgTransportador)
                            mensagemValidacao += "Situação da carga do pedido " + postagem.NumeroPedidoEmbarcador + " não aceita atualização " + cargaPedido.Carga.DescricaoSituacaoCarga + "; ";
                        else
                        {
                            pedido.PLPCorreios = postagem.PLP;
                            pedido.NumeroEtiquetaCorreios = postagem.NumeroEtiqueta;
                            repPedido.Atualizar(pedido);
                            listaCargas.Add(cargaPedido.Carga);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, "Informou dados postagem correios.", unitOfWork);

                            Servicos.Embarcador.Integracao.Correios.IntegracaoCorreios.GerarIntegracaoPendente(pedido, cargaPedido.Carga.Empresa?.IntegrarCorreios ?? true, unitOfWork);
                        }
                    }
                }

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in listaCargas.Distinct().ToList())
                {
                    serCargaDadosSumarizados.AtualizarDadosCorreios(carga, unitOfWork);
                }

                if (string.IsNullOrWhiteSpace(mensagemValidacao))
                {
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.Mensagem = "Informações atualizadas com sucesso.";
                }
                else
                {
                    retorno.Objeto = false;
                    retorno.Status = true;
                    retorno.Mensagem = mensagemValidacao;
                }

            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao integrar postagem correios.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> RecebimentoBooking(Dominio.ObjetosDeValor.WebService.Carga.Booking booking)
        {
            Servicos.Log.TratarErro($"RecebimentoBooking: {(booking != null ? Newtonsoft.Json.JsonConvert.SerializeObject(booking) : string.Empty)}", "Request");
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Objeto = false;
            retorno.Status = true;
            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
            retorno.Mensagem = "Indisponivel, em desenvolvimento ...";
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            return retorno;
        }

        public Retorno<bool> SolicitarPreCalculoCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaPreCalculoFrete cargaIntegracaoPreCalculo)
        {

            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.Log.TratarErro($"SolicitarPreCalculoCarga: {(cargaIntegracaoPreCalculo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cargaIntegracaoPreCalculo) : string.Empty)}", "Request");
            Servicos.Log.TratarErro("1 - Iniciou PreCalculo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SolicitarPreCalculoCarga");
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

            Retorno<bool> retorno = new Retorno<bool>();
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repIntegracaoCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                if (cargaIntegracaoPreCalculo == null || cargaIntegracaoPreCalculo.ProtocoloCarga == 0 || (cargaIntegracaoPreCalculo.DadosPedidos == null || cargaIntegracaoPreCalculo.DadosPedidos.Count <= 0))
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo da carga e protocolo pedido");
                }

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = !string.IsNullOrEmpty(cargaIntegracaoPreCalculo.ProtocoloTipoOperacao) ? repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracaoPreCalculo.ProtocoloTipoOperacao) : null;
                if (tipoOperacao == null)
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi localizada nenhum Tipo Operação com o protocolo informado ({cargaIntegracaoPreCalculo.ProtocoloTipoOperacao}).");
                }

                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

                if (cargaIntegracaoPreCalculo.ProtocoloCarga > 0)
                    carga = repCarga.BuscarPorProtocolo(cargaIntegracaoPreCalculo.ProtocoloCarga);
                else
                {
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorProtocoloPedido(cargaIntegracaoPreCalculo.DadosPedidos[0].ProtocoloPedido);

                        if (listaCargaPedido != null && listaCargaPedido.Count > 0)
                            carga = listaCargaPedido.FirstOrDefault().Carga;
                    }
                    else
                        carga = repCargaPedido.BuscarCargaPorProtocoloPedido(cargaIntegracaoPreCalculo.DadosPedidos[0].ProtocoloPedido);
                }

                if (carga == null)
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi localizada nenhuma carga para o protocolo informado ({cargaIntegracaoPreCalculo.ProtocoloCarga}).");
                }

                decimal pesototal = 0;
                foreach (Dominio.ObjetosDeValor.WebService.Carga.CargaPreCalculoFreteDadosPedido dadoPedido in cargaIntegracaoPreCalculo.DadosPedidos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.Where(obj => obj.Pedido.Protocolo == dadoPedido.ProtocoloPedido).FirstOrDefault();
                    if (cargaPedido != null)
                    {
                        //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                        Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {dadoPedido.PesoCargaPedido}. Cargas.svc.SolicitarPreCalculoCarga", "PesoCargaPedido");
                        cargaPedido.Peso = dadoPedido.PesoCargaPedido;
                        cargaPedido.RecebeuDadosPreCalculoFrete = true;
                        pesototal += dadoPedido.PesoCargaPedido;
                        repCargaPedido.Atualizar(cargaPedido);
                    }
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listcargaPedido = repCargaPedido.BuscarPorCarga(carga.Codigo);
                bool existePedidosSemDadosCalculoFrete = listcargaPedido.Where(x => x.RecebeuDadosPreCalculoFrete == false).Any();
                if (existePedidosSemDadosCalculoFrete)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Solicitação de Pré Calculo Negada. Não foi recebido todos os pedidos da carga ({cargaIntegracaoPreCalculo.ProtocoloCarga}).");

                carga.DataInicioCalculoFrete = DateTime.Now;
                carga.SituacaoCarga = SituacaoCarga.CalculoFrete;
                carga.CalculandoFrete = true;
                carga.DadosSumarizados.PesoTotal = pesototal;
                carga.TipoOperacao = tipoOperacao;
                carga.CargaPossuiPreCalculoFrete = true;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();

                serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Solicitou Pré Calculo Frete Carga", unitOfWork);
                repCarga.Atualizar(carga, Auditado);


                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Marfrig, false);
                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaDadosTransporteIntegracao(carga, tipoIntegracao, unitOfWork, true, false);
                if (cargaDadosTransporteIntegracao != null)
                {
                    repIntegracaoCargaDadosTransporte.Atualizar(cargaDadosTransporteIntegracao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaDadosTransporteIntegracao, "Solicitação Pré Calculo Frete Carga ", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao realizar a operação.");

            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> InformarRotaCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaRotaFrete rotaFrete)
        {
            ValidarToken();

            if (rotaFrete == null || rotaFrete.ProtocoloCarga == 0)
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo da carga.");

            if (string.IsNullOrWhiteSpace(rotaFrete.Polilinha))
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar a Polilinha.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(rotaFrete.ProtocoloCarga);
                if (carga == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi localizada nenhuma carga com o protocolo informado ({rotaFrete.ProtocoloCarga}).");

                if (!carga.OrdemRoteirizacaoDefinida)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Ordem das entregas para a roteirização ainda não foram definidas.");

                if (!carga.SituacaoCarga.IsSituacaoCargaNaoEmitida())
                    return Retorno<bool>.CriarRetornoDadosInvalidos("A Carga já está em uma situação que não permite ser alterada.");

                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

                unitOfWork.Start();

                if (cargaRotaFrete != null)
                {
                    repCargaRotaFretePontosPassagem.DeletarPorCargaRotaFrete(cargaRotaFrete.Codigo);
                    repCargaRotaFrete.Deletar(cargaRotaFrete);
                }

                cargaRotaFrete = new Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete();
                cargaRotaFrete.Carga = carga;
                cargaRotaFrete.PolilinhaRota = rotaFrete.Polilinha;
                cargaRotaFrete.TempoDeViagemEmMinutos = rotaFrete.TempoViagemMinutos;
                cargaRotaFrete.TipoUltimoPontoRoteirizacao = rotaFrete.TipoUltimoPontoRoteirizacao ?? TipoUltimoPontoRoteirizacao.PontoMaisDistante;

                repCargaRotaFrete.Inserir(cargaRotaFrete);

                carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Concluido;
                carga.CargaRotaFreteInformadaViaIntegracao = true;
                repCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Informou Carga Rota Frete via integração", unitOfWork);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao informar a rota da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> IntegrarValePedagioCarga(Dominio.ObjetosDeValor.WebService.Carga.ValePedagio integracaoValePedagio)
        {
            ValidarToken();

            if (integracaoValePedagio == null || integracaoValePedagio.ProtocoloCarga == 0)
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo da carga.");

            if ((integracaoValePedagio.PracasValePedagio == null || integracaoValePedagio.PracasValePedagio.Count <= 0))
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar ao menos uma Praça de Vale Pedágio.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra repValePedagioDadosCompra = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca repValePedagioDadosCompraPraca = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao reptipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(integracaoValePedagio.ProtocoloCarga);
                if (carga == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi localizada nenhuma carga com o protocolo informado ({integracaoValePedagio.ProtocoloCarga}).");

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio objetoIntegracaoValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio
                {
                    Carga = carga,
                    SituacaoValePedagio = SituacaoValePedagio.Confirmada,
                    TipoIntegracao = reptipoIntegracao.BuscarPorTipo(integracaoValePedagio.TipoIntegradora),
                    ProblemaIntegracao = "Recebido por Integração",
                    Observacao1 = integracaoValePedagio.Observacao,
                    NumeroValePedagio = integracaoValePedagio.NumeroValePedagio,
                    ValorValePedagio = integracaoValePedagio.ValorTotalValePedagio,
                    DataIntegracao = DateTime.Now,
                    SituacaoIntegracao = SituacaoIntegracao.Integrado,
                    CompraComEixosSuspensos = integracaoValePedagio.CompraComEixosSuspensos,
                    RecebidoPorIntegracao = true
                };
                repCargaIntegracaoValePedagio.Inserir(objetoIntegracaoValePedagio);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra dadosCompra = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra
                {
                    Carga = carga,
                    DataEmissao = DateTime.Now,
                    ValorTotalPedagios = integracaoValePedagio.ValorTotalValePedagio
                };
                repValePedagioDadosCompra.Inserir(dadosCompra);

                int quantidadeDePracas = integracaoValePedagio.PracasValePedagio.Count();
                for (int i = 0; i < quantidadeDePracas; i++)
                {
                    Dominio.ObjetosDeValor.WebService.Carga.PracaValePedagio pracaPedagio = integracaoValePedagio.PracasValePedagio[i];
                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca dadosCompraPraca = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca
                    {
                        CargaValePedagioDadosCompra = dadosCompra,
                        CodigoPraca = pracaPedagio.CodigoPraca,
                        ConcessionariaCodigo = pracaPedagio.ConcessionariaCodigo,
                        ConcessionariaNome = pracaPedagio.ConcessionariaNome.Length > 200 ? pracaPedagio.ConcessionariaNome.Substring(0, 200) : pracaPedagio.ConcessionariaNome,
                        NomePraca = pracaPedagio.NomePraca.Length > 200 ? pracaPedagio.NomePraca.Substring(0, 200) : pracaPedagio.NomePraca,
                        NumeroEixos = integracaoValePedagio.QuantidadeEixos,
                        Valor = pracaPedagio.Valor,
                        NomeRodovia = pracaPedagio.NomeRodovia
                    };

                    repValePedagioDadosCompraPraca.Inserir(dadosCompraPraca);
                }

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Integrou Vale Pedagio Carga: " + carga.CodigoCargaEmbarcador, unitOfWork);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao integrar vale pedagio para a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> InformarEmbarqueContainer(Dominio.ObjetosDeValor.WebService.Carga.EmbarqueContainer embarqueContainer)
        {
            ValidarToken();

            if (embarqueContainer == null || string.IsNullOrEmpty(embarqueContainer.CodigoIntegracaoContainer))
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar o codigo integração do container.");

            DateTime dataEmbarque;
            if ((embarqueContainer.DataEmbarque == null || DateTime.TryParse(embarqueContainer.DataEmbarque, out dataEmbarque)))
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar uma Data de Embarque válida.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.Embarcador.Pedido.ColetaContainer servColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
            Repositorio.Embarcador.Pedidos.ColetaContainerCargaEntrega repColetaContainerCargaEntrega = new Repositorio.Embarcador.Pedidos.ColetaContainerCargaEntrega(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Pedidos.Container container = repContainer.BuscarPorCodigoIntegracao(embarqueContainer.CodigoIntegracaoContainer);
                if (container == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi localizada nenhum container com código integração informado ({embarqueContainer.CodigoIntegracaoContainer}).");

                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repColetaContainer.BuscarPorContainerEmEsperaCarregando(container.Codigo);
                if (coletaContainer != null)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repColetaContainerCargaEntrega.BuscarCargaEntregaPorColetaContainer(coletaContainer.Codigo);
                    servColetaContainer.InformarEmbarqueContainer(dataEmbarque, cargaEntrega?.Cliente, coletaContainer, Auditado.Usuario, OrigemMovimentacaoContainer.WebService);

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Informou embarque container: " + container.Codigo + " Data Embarque:" + dataEmbarque.ToString("dd/MM/yyyy"), unitOfWork);

                    unitOfWork.CommitChanges();

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
                else
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi localizada nenhum container com código integração informado ({embarqueContainer.CodigoIntegracaoContainer}) e com situação Em espera carregado.");
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao informar o enbarque container.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> InformarDadosTransporteCarga(Dominio.ObjetosDeValor.WebService.Carga.DadosTransporte dadosTransporte)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao, WebServiceConsultaCTe).InformarDadosTransporteCarga(dadosTransporte, integradora));
            });
        }

        public Retorno<int> GerarCarregamento(Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<int>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).GerarCarregamento(carregamento));
            });
        }

        public Retorno<bool> AjustarDatasPedido(Dominio.ObjetosDeValor.WebService.Carga.AjusteDatasPedido ajusteDatasPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AjustarDatasPedido(ajusteDatasPedido));
            });
        }

        public Retorno<bool> AjustarNumeroOrdemPedido(Dominio.ObjetosDeValor.WebService.Carga.AjustarNumeroOrdemPedido ajusteNumeroOrdemPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AjustarNumeroOrdemPedido(ajusteNumeroOrdemPedido));
            });
        }

        public Retorno<bool> CallbackIntegracaoBooking(Dominio.ObjetosDeValor.WebService.Carga.RetornoBooking retornoBooking)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {

                if (retornoBooking == null)
                    throw new WebServiceException("É obrigatório informar o objeto retornoBooking");

                int protocolo;
                int.TryParse(retornoBooking.ProtocoloReferencia, out protocolo);

                if (string.IsNullOrEmpty(retornoBooking.ProtocoloReferencia) || protocolo <= 0)
                    throw new WebServiceException("Protocolo referencia booking inválido.");

                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao pedidoDadosTransporteIntegracao = repositorioIntegracao.BuscarPorCodigoBooking(protocolo);

                if (pedidoDadosTransporteIntegracao == null)
                    throw new WebServiceException("Integração booking não encontrada com o Protocolo referencia: " + retornoBooking.ProtocoloReferencia);

                Servicos.Embarcador.Integracao.ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new Servicos.Embarcador.Integracao.ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

                string response = Newtonsoft.Json.JsonConvert.SerializeObject(retornoBooking);

                new Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo(unitOfWork).TratarEntidadeRetornoIntegracao(pedidoDadosTransporteIntegracao, retornoBooking);

                if (retornoBooking.Status)
                    pedidoDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                else
                    pedidoDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                pedidoDadosTransporteIntegracao.ProblemaIntegracao = retornoBooking.CodigoMensagem.ToString() + retornoBooking.Mensagem;

                servicoArquivoTransacao.Adicionar(pedidoDadosTransporteIntegracao, "", response, "json");

                repositorioIntegracao.Atualizar(pedidoDadosTransporteIntegracao);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException exception)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(exception.Message);
            }
            catch (Exception exception)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exception);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar integracao booking.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<int> SalvarDocumentoTransporte(DocumentoTransporte documentoTransporte)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<int>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SalvarDocumentoTransporte(documentoTransporte, integradora));
            });
        }

        public Retorno<bool> EnviarCancelamentoCarga(Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoCarga envioCancelamentoCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarCancelamentoCarga(envioCancelamentoCarga));
            });
        }

        public Retorno<bool> EnviarCargaDocumentos(Dominio.ObjetosDeValor.WebService.Carga.CargaDocumentos cargaDocumentos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarCargaDocumentos(cargaDocumentos));
            });
        }

        public Retorno<bool> ConfirmarFrete(Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado).ConfirmarFrete(protocolo));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>> BuscarModelosVeicularesPendentesIntegracao(int quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarModelosVeicularesPendentesIntegracao(quantidade));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> ConfirmarIntegracaoModeloVeicular(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoModeloVeicular(protocolos));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> ObterCargasAguardandoEnvioDocumentos(int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ObterCargasAguardandoEnvioDocumentos(new Dominio.ObjetosDeValor.WebService.RequestPaginacao() { Inicio = inicio ?? 0, Limite = limite ?? 0 }, integradora));
            });
        }

        public Retorno<bool> ConfirmarRecebimentoCargaAguardandoEnvioDocumentos(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarRecebimentoCargaAguardandoEnvioDocumentos(protocolos));
            });
        }

        public Retorno<List<PreDocumento>> RetornarPreDocumentosPorCarga(int protocoloCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<PreDocumento>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).RetornarPreDocumentosPorCarga(protocoloCarga));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.FilaHResponse> TotemFilaH(FilaHRequest request)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.FilaHResponse>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ObterDadosTotemFilaH(request));
            });
        }

        public Retorno<bool> AlterarDataAgendamentoEntregaPorProtocoloPedido(Dominio.ObjetosDeValor.WebService.Carga.AjusteDataAgendamentoEntrega ajusteDataAgendamentoEntregaIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AlterarDataAgendamentoEntregaPorProtocoloPedido(ajusteDataAgendamentoEntregaIntegracao));
            });
        }

        public Retorno<bool> AlterarDadosAgendamentoPedido(AjusteDadosAgendamentoPedido ajusteDadosAgendamentoPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AlterarDadosAgendamentoPedido(ajusteDadosAgendamentoPedido));
            });
        }

        public Retorno<bool> IntegrarAE(IntegracaoCargaAE cargaAE)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).IntegrarAE(cargaAE));
            });
        }

        public Retorno<bool> AtualizarDadosPagamentoProvedor(Dominio.ObjetosDeValor.WebService.Carga.DadosPagamentoProvedor dadosPagamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AtualizarDadosPagamentoProvedor(dadosPagamento));
            });
        }

        public Retorno<bool> RetornoConfirmacaoColeta(Dominio.ObjetosDeValor.WebService.Carga.RetornoConfirmacaoColeta retornoConfirmacaoColeta)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao)
                    .RetornoConfirmacaoColeta(retornoConfirmacaoColeta));
            });
        }

        public Retorno<bool> InformarDadosTransportador(Dominio.ObjetosDeValor.WebService.Carga.DadosTransportador DadosTransportador)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao, WebServiceConsultaCTe).InformarDadosTransportador(DadosTransportador, integradora));
            });
        }

        public Retorno<bool> SetarCargaCritica(int protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao, WebServiceConsultaCTe).SetarCargaCritica(protocolo));
            });
        }

        public Retorno<bool> SetarPedidoCritico(Dominio.ObjetosDeValor.Embarcador.Carga.PedidoCritico pedidoCritico)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao, WebServiceConsultaCTe).SetarPedidoCritico(pedidoCritico));
            });
        }

        #endregion Métodos de Requisição

        #region Métodos Privados 

        private Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe PreencherObjetoCamposOriginais(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe()
            {
                NumeroBooking = cte.NumeroBooking,
                CodigoPortoPassagemUm = cte.PortoPassagemUm?.Codigo ?? 0,
                CodigoViagem = cte.Viagem?.Codigo ?? 0,
            };
        }

        private List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> GerarCamposCartaCorrecao(Dominio.ObjetosDeValor.WebService.Carga.CargaAtualizacaoRolagem cargaAtualizacaoRolagem, Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe camposOriginaisCTe, Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavioNovo, Dominio.Entidades.Embarcador.Pedidos.Porto portoTransbordo)
        {
            int sequencial = 0;
            List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> listaCampos = new List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe>();

            //if (camposOriginaisCTe.CodigoNavio != (pedidoViagemNavioNovo?.Navio?.Codigo ?? 0))
            //    listaCampos.Add(AdicionarCampoCCe("Observação (navio)", "xObs", "compl", Dominio.Enumeradores.TipoCampoCCe.Texto, 800, 0, 0, false, pedidoViagemNavioNovo?.Navio?.Descricao, Dominio.Enumeradores.TipoCampoCCeAutomatico.Navio, ref sequencial));

            if (camposOriginaisCTe.CodigoViagem != (pedidoViagemNavioNovo?.Codigo ?? 0))
                listaCampos.Add(AdicionarCampoCCe("Observação Cont (navio)", "xTexto", "ObsCont", Dominio.Enumeradores.TipoCampoCCe.Texto, 500, 0, 0, true, pedidoViagemNavioNovo?.Descricao, Dominio.Enumeradores.TipoCampoCCeAutomatico.Nenhum, ref sequencial));

            if (!string.IsNullOrWhiteSpace(cargaAtualizacaoRolagem.NumeroBooking) && !string.IsNullOrWhiteSpace(camposOriginaisCTe.NumeroBooking) && !camposOriginaisCTe.NumeroBooking.Equals(cargaAtualizacaoRolagem.NumeroBooking))
                listaCampos.Add(AdicionarCampoCCe("Observação Cont (navio)", "xTexto", "ObsCont", Dominio.Enumeradores.TipoCampoCCe.Texto, 500, 0, 0, true, cargaAtualizacaoRolagem.NumeroBooking, Dominio.Enumeradores.TipoCampoCCeAutomatico.Booking, ref sequencial));

            if (camposOriginaisCTe.CodigoPortoPassagemUm != (portoTransbordo?.Codigo ?? 0))
                listaCampos.Add(AdicionarCampoCCe("Porto Passagem", "xPass", "pass", Dominio.Enumeradores.TipoCampoCCe.Texto, 500, 0, 0, true, portoTransbordo?.Descricao, Dominio.Enumeradores.TipoCampoCCeAutomatico.PortoPassagem, ref sequencial));

            return listaCampos;
        }

        private Dominio.ObjetosDeValor.WebService.CTe.CampoCCe AdicionarCampoCCe(string descricao, string nomeCampo, string grupoCampo, Dominio.Enumeradores.TipoCampoCCe tipoCampo, int qtdeCaracteres, int qtdedecimais, int qtdeInteiros, bool repeticao, string valorAlterado, Dominio.Enumeradores.TipoCampoCCeAutomatico tipoCampoCCeAutomatico, ref int sequencial)
        {
            sequencial++;

            return new Dominio.ObjetosDeValor.WebService.CTe.CampoCCe()
            {
                Descricao = descricao,
                GrupoCampo = grupoCampo,
                NomeCampo = nomeCampo,
                TipoCampo = tipoCampo,
                QuantidadeCaracteres = qtdeCaracteres,
                QuantidadeDecimais = qtdedecimais,
                QuantidadeInteiros = qtdeInteiros,
                IndicadorRepeticao = repeticao,
                NumeroItemAlterado = sequencial,
                ValorAlterado = valorAlterado,
                TipoCampoCCeAutomatico = tipoCampoCCeAutomatico,
            };
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargasPorChaveCTe(Repositorio.UnitOfWork unitOfWork, string chaveCTe)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repConhecimentoDeTransporteEletronico.BuscarPorChave(chaveCTe);
            if (cte == null)
            {
                throw new Exception($"Chave do CTe \"{chaveCTe}\" nao encontrado.");
            }
            else
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarTodosPorCTe(cte.Codigo);
                int totalCargaCTes = cargaCTes?.Count ?? 0;
                if (totalCargaCTes == 0)
                {
                    throw new Exception($"Chave do CTe \"{chaveCTe} ({cte.Codigo})\" encontrado mas carga não relacionada.");
                }
                else
                {
                    return cargaCTes;
                }
            }
        }

        private Retorno<RetornoCIOT> BuscarDocumentoCiotPorProtocolo(int protocoloCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = null;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocoloCarga);

                if (carga == null)
                    return Retorno<RetornoCIOT>.CriarRetornoDadosInvalidos("Não foi possível buscar a carga pelo protocolo " + protocoloCarga.ToString());

                cargaCIOT = repCargaCIOT.BuscarPorCarga(carga.Codigo);
                if (cargaCIOT == null)
                    return Retorno<RetornoCIOT>.CriarRetornoDadosInvalidos("Não foi encontrado nenhum CIOT para a carga " + protocoloCarga.ToString());

                if (cargaCIOT.CIOT.Situacao != SituacaoCIOT.Aberto && cargaCIOT.CIOT.Situacao != SituacaoCIOT.Encerrado && cargaCIOT.CIOT.Situacao != SituacaoCIOT.PagamentoAutorizado)
                    return Retorno<RetornoCIOT>.CriarRetornoDadosInvalidos("CIOT está com pendência, PDF não disponível " + protocoloCarga.ToString());

                string mensagemErro = string.Empty;
                byte[] arquivo = null;

                if (cargaCIOT.CIOT.Operadora == OperadoraCIOT.eFrete)
                    arquivo = new Servicos.Embarcador.CIOT.EFrete().ObterOperacaoTransportePdf(cargaCIOT, unitOfWork, out mensagemErro);
                else
                {
                    Servicos.Embarcador.CIOT.CIOT ciot = new Servicos.Embarcador.CIOT.CIOT();
                    arquivo = ciot.GerarContratoFrete(cargaCIOT.CIOT.Codigo, unitOfWork, out mensagemErro);
                }


                if (arquivo == null)
                {
                    if (!string.IsNullOrWhiteSpace(mensagemErro))
                        Servicos.Log.TratarErro(mensagemErro, "CIOT");

                    if (cargaCIOT.CIOT.Operadora == OperadoraCIOT.eFrete)
                        return Retorno<RetornoCIOT>.CriarRetornoDadosInvalidos("PDF não disponibilizado pela eFrete.");
                    else
                        return Retorno<RetornoCIOT>.CriarRetornoDadosInvalidos("Não foi possível gerar o PDF.");
                }

                string pdfCIOT = string.Empty;

                if (configuracao.UtilizarCodificacaoUTF8ConversaoPDF)
                    pdfCIOT = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, arquivo));
                else
                    pdfCIOT = Convert.ToBase64String(arquivo);

                RetornoCIOT buscarCiot = new RetornoCIOT()
                {
                    Arquivo = pdfCIOT,
                    NumeroContrato = cargaCIOT.CIOT.Descricao
                };
                return Retorno<RetornoCIOT>.CriarRetornoSucesso(buscarCiot);
            }
            catch (ServicoException excecao)
            {
                return Retorno<RetornoCIOT>.CriarRetornoExcecao(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<RetornoCIOT>.CriarRetornoExcecao("Ocorreu uma falha ao consultar o PDF do CIOT.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas;
        }

        #endregion Métodos Privado
    }
}
