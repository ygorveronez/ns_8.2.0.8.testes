using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.WebService.OrdemCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using CoreWCF;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Financeiro(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IFinanceiro
    {
        public Retorno<bool> IndicarAntecipacaoFreteDocumento(Dominio.ObjetosDeValor.WebService.Financeiro.DocumentoAntecipacao documentoAntecipacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).IndicarAntecipacaoFreteDocumento(documentoAntecipacao));
            });
        }

        public Retorno<bool> QuitarTituloPagar(int? protocolo, string dataPagamento, string observacao, decimal? valorAcrescimo, decimal? valorDesconto, string codigoIntegracaoFormaPagamento)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            protocolo ??= 0;
            valorAcrescimo ??= 0m;
            valorDesconto ??= 0m;

            try
            {
                ValidarToken();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo((int)protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar);

                if (titulo == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um título para o protocolo informado.");

                Retorno<bool> retorno = new Retorno<bool>();
                DateTime dataPagto;

                if (string.IsNullOrWhiteSpace(codigoIntegracaoFormaPagamento))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi informado a forma de pagamento para a quitação do título.";
                }
                else if (!DateTime.TryParseExact(dataPagamento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPagto))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "A data de pagamento não esta em um formato correto (dd/MM/yyyy)";
                }
                else if (titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                {
                    if (retorno.CodigoMensagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos)
                    {
                        string erro = "";
                        if (Servicos.Embarcador.Financeiro.Titulo.QuitarTituloAPagar(out erro, titulo, dataPagto, dataPagto, unitOfWork, titulo.Pessoa, null, null, TipoServicoMultisoftware, "BAIXA PELO SERVIÇO DE WS", (decimal)valorAcrescimo, true, (decimal)valorDesconto, true, codigoIntegracaoFormaPagamento))
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, "Confirmou o pagamento do título por serviço de WS", unitOfWork);
                            retorno.Status = true;
                            retorno.Objeto = true;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
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

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do título quitado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> QuitarTituloReceber(Dominio.ObjetosDeValor.WebService.Financeiro.QuitarTituloReceber quitarTituloReceber)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                ValidarToken();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(quitarTituloReceber.protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber);

                if (titulo == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um título para o protocolo informado.");

                Retorno<bool> retorno = new Retorno<bool>();
                DateTime dataPagto;

                if (string.IsNullOrWhiteSpace(quitarTituloReceber.codigoIntegracaoFormaPagamento))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi informado a forma de pagamento para a quitação do título.";
                }
                else if (!DateTime.TryParseExact(quitarTituloReceber.dataPagamento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPagto))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "A data de pagamento não esta em um formato correto (dd/MM/yyyy)";
                }
                else if (titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                {
                    if (retorno.CodigoMensagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos)
                    {
                        string erro = "";
                        if (Servicos.Embarcador.Financeiro.Titulo.QuitarTitulo(out erro, titulo, dataPagto, dataPagto, unitOfWork, titulo.Pessoa, titulo.GrupoPessoas, null, TipoServicoMultisoftware, "BAIXA PELO SERVIÇO DE WS", quitarTituloReceber.valorAcrescimo, false, quitarTituloReceber.valorDesconto, false, Auditado, quitarTituloReceber.codigoIntegracaoFormaPagamento))
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, "Confirmou o pagamento do título por serviço de WS", unitOfWork);
                            retorno.Status = true;
                            retorno.Objeto = true;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
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

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do título quitado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado>> BuscarTituloAReceberQuitadoPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite > 100)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100.");

                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado> listaTitulosQuitados = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado>();

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);

                int totalRegistros = repTituloBaixaAgrupado.ContarConsultaTituloAReceberQuitadoPendentesIntegracao();

                if (totalRegistros > 0)
                {
                    Servicos.WebService.Financeiro.Titulo serWSTitulo = new Servicos.WebService.Financeiro.Titulo(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosBaixa = repTituloBaixaAgrupado.ConsultarTituloAReceberQuitadoPendentesIntegracao("Codigo", "desc", (int)inicio, (int)limite);
                    foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixa in titulosBaixa)
                        listaTitulosQuitados.Add(serWSTitulo.ConverterObjetoTituloQuitado(tituloBaixa.Titulo, tituloBaixa.TituloBaixa, unitOfWork));
                }

                Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado> retorno = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado>()
                {
                    Itens = listaTitulosQuitados,
                    NumeroTotalDeRegistro = totalRegistros
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Títulos Quitados Pendentes de Integração", unitOfWork);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado>>.CriarRetornoSucesso(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os títulos quitados pendentes de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarIntegracaoTituloQuitado(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                ValidarToken();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(protocolo);

                if (titulo == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um título para o protocolo informado.");

                if (titulo.TipoTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber || titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Título para o protocolo informado não está quitado.");

                if (titulo.IntegradoQuitacao)
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao("Título quitado já foi integrado.");

                titulo.IntegradoQuitacao = true;
                repTitulo.Atualizar(titulo);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do título quitado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>> BuscarTitulosPagarPendenteIntegracao(int? inicio, int? limite, string codigoIntergracaoTipoMovimento = null)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite > 100)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100.");

                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo> listaTitulos = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                int totalRegistros = repTitulo.ContarConsultaTitulosPagarPendenteIntegracao();

                if (totalRegistros > 0)
                {
                    Servicos.WebService.Financeiro.Titulo serWSTitulo = new Servicos.WebService.Financeiro.Titulo(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.ConsultarTitulosPagarPendenteIntegracao("Codigo", "desc", (int)inicio, (int)limite, codigoIntergracaoTipoMovimento);
                    foreach (Dominio.Entidades.Embarcador.Financeiro.Titulo titulo in titulos)
                        listaTitulos.Add(serWSTitulo.ConverterObjetoTituloAPagar(titulo, unitOfWork));
                }

                Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo> retorno = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>()
                {
                    Itens = listaTitulos,
                    NumeroTotalDeRegistro = totalRegistros
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Títulos a Pagar Pendentes de Integração", unitOfWork);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>>.CriarRetornoSucesso(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os títulos a pagar pendentes de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarIntegracaoTituloPagar(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                ValidarToken();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(protocolo);

                if (titulo == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um título para o protocolo informado.");

                if (titulo.TipoTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Título informado não é do tipo a pagar.");

                if (titulo.Integrado)
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao("Título já foi integrado.");

                titulo.Integrado = true;
                repTitulo.Atualizar(titulo);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do título.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>> ConsultarDocumentoEntradaPorOrdemCompra(int protocoloOrdemCompra)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada> listaDocumentoEntrada = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>();

                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);

                int totalRegistros = repDocumentoEntradaTMS.ContarConsultarDocumentosVinculadosComOrdemCompra(protocoloOrdemCompra);

                if (totalRegistros > 0)
                {
                    Servicos.WebService.Financeiro.DocumentoEntrada serWSDocumentoEntrada = new Servicos.WebService.Financeiro.DocumentoEntrada(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentos = repDocumentoEntradaTMS.ConsultarDocumentosVinculadosComOrdemCompra(protocoloOrdemCompra);
                    foreach (var documento in documentos)
                        listaDocumentoEntrada.Add(serWSDocumentoEntrada.ConverterObjetoDocumentoEntrada(documento, unitOfWork));
                }

                Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada> retorno = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>()
                {
                    Itens = listaDocumentoEntrada,
                    NumeroTotalDeRegistro = totalRegistros
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Documento de entrada vinculada com ordem de compra", unitOfWork);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>>.CriarRetornoSucesso(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os documentos de entrada vinculada com ordem de compra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> EnviarFaturaCompleta(Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AdicionarFaturaCompleta(faturaIntegracao));
            });
        }

        public Retorno<bool> ConfirmarIntegracaoDocumentoEntrada(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoDocumento(protocolos));
            });
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>> BuscarDocumentoEntradaPendenteIntegracao(int? inicio, int? quantidadeRegistros, string codigoTipoMovimento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>>
                .CreateFrom(new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao)
                .BuscarDocumentoEntradaPendenteIntegracao(new Dominio.ObjetosDeValor.WebService.Rest.Financeiro.RequestDocumentoEntradaPendente()
                {
                    CodigoTipoMovimento = codigoTipoMovimento,
                    Inicio = inicio ?? 0,
                    QuantidadeRegistros = quantidadeRegistros ?? 0
                }));
            });
        }

        public Retorno<bool> ConfirmarIntegracaoTituloFinanceiro(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoTituloFinanceiro(protocolos));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>> BuscarTitulosPentendesIntegracaoERP(int? quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>>.CreateFrom(new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarTitulosPentendesIntegracao(quantidade ?? 0));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> RecebePDF(int codigo, string boleto, string numero)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SalvarPDF(codigo, boleto, numero));
            });
        }

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceFinanceiro;
        }

        public Retorno<bool> ReceberDocumentoEntradaTMS(Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntradaTMS)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ReceberDocumentoEntradaTMS(documentoEntradaTMS));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico>> ConsultarMovimentacoesPneusPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite > 100)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100.");

                if (limite == 0)
                    limite = 100;


                List<Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico> listaHistoricos = new List<Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico>();

                Repositorio.Embarcador.Frota.PneuHistorico repPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);

                int totalRegistros = repPneuHistorico.ContarConsultaMovimentacoesPneusPendentesIntegracao();

                if (totalRegistros > 0)
                {
                    Servicos.WebService.Frota.PneuHistorico serWSPneuHistorico = new Servicos.WebService.Frota.PneuHistorico(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Frota.PneuHistorico> historicos = repPneuHistorico.ConsultaMovimentacoesPneusPendentesIntegracao("Codigo", "desc", (int)inicio, (int)limite);
                    foreach (Dominio.Entidades.Embarcador.Frota.PneuHistorico historico in historicos)
                        listaHistoricos.Add(serWSPneuHistorico.ConverterObjetoPneuHistorico(historico, unitOfWork));
                }

                Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico> retorno = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico>()
                {
                    Itens = listaHistoricos,
                    NumeroTotalDeRegistro = totalRegistros
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Histórico de pneus Pendentes de Integração", unitOfWork);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico>>.CriarRetornoSucesso(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os históricos de pneus.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarMovimentacaoPneuPendenteIntegracao(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                ValidarToken();

                Repositorio.Embarcador.Frota.PneuHistorico repHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.PneuHistorico historico = repHistorico.BuscarPorCodigo(protocolo, true);

                if (historico == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um histórico de penu para o protocolo informado.");

                if (historico.Integrado)
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao("Pneu já foi integrado.");

                historico.Integrado = true;
                repHistorico.Atualizar(historico);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do Pneu.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir> GerarFaturaCompleta(Dominio.ObjetosDeValor.Embarcador.Fatura.GerarFaturaCompletaIntegracao envGerarFaturaCompletaIntegracao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Dominio.Entidades.WebService.Integradora integradora =  ValidarToken();

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                double codigoCliente = 0;
                if (!double.TryParse(envGerarFaturaCompletaIntegracao.cnpjCpfCliente, out codigoCliente))
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"CNPJ/CPF inválido.");

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(codigoCliente);
                if (cliente == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Cliente {envGerarFaturaCompletaIntegracao.cnpjCpfCliente} não localizado.");

                Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaInserir objValorInserirFatura = new Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaInserir();
                objValorInserirFatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento;
                objValorInserirFatura.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa;
                objValorInserirFatura.Cliente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = cliente.CPF_CNPJ.ToString() };

                if (string.IsNullOrEmpty(envGerarFaturaCompletaIntegracao.dataFatura))
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Necessário informar a data da fatura no formato (dd/MM/yyyy HH:mm:ss).");

                DateTime? dataFatura = envGerarFaturaCompletaIntegracao.dataFatura.ToNullableDateTime();
                if (!dataFatura.HasValue)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"A data da fatura não está em um formato correto (dd/MM/yyyy HH:mm:ss).");

                objValorInserirFatura.DataFatura = (DateTime)dataFatura;
                objValorInserirFatura.DataInicial = dataFatura;
                objValorInserirFatura.DataFinal = dataFatura;

                if (envGerarFaturaCompletaIntegracao.documentos == null || envGerarFaturaCompletaIntegracao.documentos.Count() == 0)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos("Necessario informar lista de documentos.");

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigoIntegracao(envGerarFaturaCompletaIntegracao.codigoIntegracaoOperador);

                if (usuario == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Operador código integração {envGerarFaturaCompletaIntegracao.codigoIntegracaoOperador} não localizado.");

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                foreach (Dominio.ObjetosDeValor.Embarcador.Fatura.GerarFaturaCompletaIntegracaoDocumentos documento in envGerarFaturaCompletaIntegracao.documentos)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                    int codigoEmpresa = 0;

                    if (string.IsNullOrEmpty(documento.chaveAcesso))
                    {
                        if ((documento.numero ?? 0) == 0)
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Documento inválido! necessário informar a chave de acesso ou o numero do documento.");

                        if (string.IsNullOrEmpty(documento.cnpjCpfEmitente))
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Documento inválido! quando não for informada a chave de acesso é necessário informar o emitente do documento.");

                        if ((documento.serie ?? 0) == 0)
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Documento inválido! quando não for informada a chave de acesso é necessário informar a serie do documento.");

                        if (string.IsNullOrEmpty(documento.modelo))
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Documento inválido! quando não for informada a chave de acesso é necessário informar o modelo do documento.");

                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(documento.cnpjCpfEmitente);

                        if (empresa == null)
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Documento inválido! cnpjCpfEmitente {documento.cnpjCpfEmitente} não localizado.");

                        codigoEmpresa = empresa.Codigo;
                    }

                    if (!string.IsNullOrEmpty(documento.chaveAcesso))
                        cte = repConhecimentoDeTransporteEletronico.BuscarPorChave(documento.chaveAcesso);
                    else
                        cte = repConhecimentoDeTransporteEletronico.BuscarPorNumeroModeloSerieEEmpresa((int)documento.numero, documento.modelo, (int)documento.serie, codigoEmpresa);

                    if (cte == null)
                    {
                        if (!string.IsNullOrEmpty(documento.chaveAcesso))
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Documento de chave de acesso {documento.chaveAcesso} não localizado.");
                        else
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Documento de nro {documento.numero} e modelo {documento.modelo} não localizado.");
                    }

                    if (cte.Status != "A")
                    {
                        if (!string.IsNullOrEmpty(documento.chaveAcesso))
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Documento de chave de acesso {documento.chaveAcesso} não encontra-se em situação que possa gerar fatura.");
                        else
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Documento de nro {documento.numero} e modelo {documento.modelo} não encontra-se em situação que possa gerar fatura.");
                    }

                    if (objValorInserirFatura.CodigosDocumentos == null)
                        objValorInserirFatura.CodigosDocumentos = new List<int>();


                    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(cte.Codigo);
                    if (documentoFaturamento == null)
                        return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Documento de nro {cte.Numero} não está disponível para faturamento.");

                    objValorInserirFatura.CodigosDocumentos.Add(documentoFaturamento.Codigo);
                }

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                string msgErro = "";
                string msgAlert = "";

                unitOfWork.Start();
                if (!servFatura.GerarFatura(objValorInserirFatura, out msgErro, out msgAlert, configuracao, TipoServicoMultisoftware, Auditado, usuario))
                {
                    unitOfWork.Rollback();
                    if (msgAlert != "")
                        return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos(msgAlert);
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos(msgErro);
                }
                else
                    unitOfWork.CommitChanges();
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir 
                {
                    Codigo = objValorInserirFatura.Codigo,
                    Situacao = objValorInserirFatura.Situacao,
                    Numero = objValorInserirFatura.Numero
                } );
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoExcecao("Ocorreu uma falha ao gerar fatura completa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir> CancelarFatura(int codigo, string motivo, string codigoIntegracaoOperador)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigoIntegracao(codigoIntegracaoOperador);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();


                if (usuario == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Operador código integração {codigoIntegracaoOperador} não localizado.");

                if (string.IsNullOrEmpty(motivo))
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Motivo Cancelamento não informado.");

                if (fatura == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Fatura {codigo} não localizada.");

                if (fatura.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos($"Fatura {codigo} já cancelada.");

                Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
                
                if (fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                {
                    servicoFatura.ValidarCancelamentoFatura(codigo, DateTime.Now);

                    //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                    //if (!(usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteReAbrirFatura)))
                    //    return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos("Seu usuário não possui permissão para reabrir a fatura e posteriormente cancelar.");                                      

                    unitOfWork.Start();                    

                    servicoFatura.IniciarCancelamentoFatura(codigo, motivo, usuario, configuracaoEmbarcador, Auditado, false, DateTime.Now,0,false);               
                }
                else
                {
                    if (fatura.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas)
                        return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos("A fatura ainda está em processo de lançamento de documentos, não sendo possível realizar o cancelamento.");

                    unitOfWork.Start();

                    fatura.UsuarioCancelamento = usuario;
                    fatura.NotificadoOperador = false;
                    fatura.SituacaoNoCancelamento = fatura.Situacao;
                    fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmCancelamento;
                    fatura.DataCancelamentoFatura = DateTime.Now;
                    fatura.Duplicar = false;                    

                    repFatura.Atualizar(fatura, Auditado);

                    servicoFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.CancelouFatura, usuario);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Cancelou a fatura via integração.", unitOfWork);                    
                }

                unitOfWork.CommitChanges();

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir
                {
                    Codigo = codigo,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmCancelamento,
                    Numero = fatura.Numero
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);                
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir>.CriarRetornoDadosInvalidos("Ocorreu uma falha ao cancelar a fatura. ERRO: "+ex.Message);                
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
