using System;
using System.Collections.Generic;
using System.Text;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class ValePedagio(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IValePedagio
    {
        public void DoWork()
        {
        }

        #region Métodos Públicos

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio>> BuscarValePedagioPorCarga(int? protocoloCarga, int? inicio, int? limite)
        {
            ValidarToken();

            protocoloCarga ??= 0;
            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 50)
                {
                    if (protocoloCarga > 0)
                    {
                        Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                        Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                        Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);

                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                        string mensagem = "";

                        retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio>();

                        retorno.Objeto.NumeroTotalDeRegistro = repCargaValePedagio.ContarConsulta((int)protocoloCarga);
                        retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio>();
                        if (retorno.Objeto.NumeroTotalDeRegistro > 0)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> cargaValePedagios = repCargaValePedagio.Consultar((int)protocoloCarga, "Codigo", "desc", (int)inicio, (int)limite);
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaValePedagio cargaValePedagio in cargaValePedagios)
                            {
                                Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio valePedagio = new Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio();
                                valePedagio.Fornecedor = serWSPessoa.ConverterObjetoPessoa(cargaValePedagio.Fornecedor);
                                valePedagio.Responsavel = serWSPessoa.ConverterObjetoPessoa(cargaValePedagio.Responsavel);
                                valePedagio.NumeroValePedagio = cargaValePedagio.NumeroComprovante;
                                valePedagio.ValorValePedagio = cargaValePedagio.Valor;
                                valePedagio.TipoCompra = cargaValePedagio.TipoCompra;

                                byte[] arquivo = null;
                                servicoValePedagio.ObterArquivoValePedagio(cargaValePedagio.CargaIntegracaoValePedagio, ref arquivo, TipoServicoMultisoftware);
                                if (arquivo != null)
                                {
                                    if (configuracao.UtilizarCodificacaoUTF8ConversaoPDF)
                                        valePedagio.PDF = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, arquivo));
                                    else
                                        valePedagio.PDF = Convert.ToBase64String(arquivo);
                                }

                                retorno.Objeto.Itens.Add(valePedagio);
                            }
                        }

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
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os vale pedágios";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.ValePedagio.ConsultaValePedagio> ObterConsultaValePedagioPorCarga(int protocoloCarga)
        {
            ValidarToken();

            Retorno<Dominio.ObjetosDeValor.WebService.ValePedagio.ConsultaValePedagio> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.ValePedagio.ConsultaValePedagio>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (protocoloCarga > 0)
                {

                    Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValorPedagioIntegracao = repCargaConsultaValePedagio.BuscarIntegracaoConsultaporProtocoloCarga(protocoloCarga);

                    if (cargaConsultaValorPedagioIntegracao != null)
                    {

                        retorno.Objeto = new Dominio.ObjetosDeValor.WebService.ValePedagio.ConsultaValePedagio();
                        retorno.Objeto.DataIntegracao = cargaConsultaValorPedagioIntegracao.DataIntegracao;
                        retorno.Objeto.NumeroValePedagio = cargaConsultaValorPedagioIntegracao.NumeroValePedagio;
                        retorno.Objeto.ProblemaIntegracao = cargaConsultaValorPedagioIntegracao.ProblemaIntegracao;
                        retorno.Objeto.SituacaoIntegracao = cargaConsultaValorPedagioIntegracao.SituacaoIntegracao.ObterDescricao();
                        retorno.Objeto.ValorValePedagio = cargaConsultaValorPedagioIntegracao.ValorValePedagio;
                        retorno.Objeto.QuantidadeEixos = cargaConsultaValorPedagioIntegracao.QuantidadeEixos;
                        retorno.Objeto.TipoRota = cargaConsultaValorPedagioIntegracao.TipoRota;
                        retorno.Objeto.ProtocoloCarga = cargaConsultaValorPedagioIntegracao.Carga.Protocolo;

                        retorno.Status = true;
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.RegistroIndisponivel;
                        retorno.Mensagem = "Registro não encontrado para o protocolo carga: " + protocoloCarga;
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "É obrigatório informar o protocolo de integração. ";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as consulta de vale pedágio da carga";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;

        }

        public Retorno<bool> IntegrarValePedagio(Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio valePedagio)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.ValePedagio.ValePedadigo(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).IntegraValePedagio(valePedagio, integradora));
            });
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceValePedagio;
        }

        #endregion
    }
}
