using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.IO;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]

    public class Canhotos(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), ICanhotos
    {
        #region Métodos Globais

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosNotasFiscaisDigitalizados(int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>.CreateFrom(new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarCanhotosNotasFiscaisDigitalizados(new Dominio.ObjetosDeValor.WebService.RequestPaginacao() { Inicio = inicio ?? 0, Limite = limite ?? 0 }, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosDigitalizadoseAgAprovacao(int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>.CreateFrom(new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao)
                    .BuscarCanhotosDigitalizadoseAgAprovacao(new Dominio.ObjetosDeValor.WebService.RequestPaginacao() { Inicio = inicio ?? 0, Limite = limite ?? 0 }, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(int? protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(new List<int>() { protocolo ?? 0 }));
            });
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.NFe.ResponseCanhoto>> ConfirmarIntegracoesDigitalizacaoCanhotosNotasFiscais(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.NFe.ResponseCanhoto>>.CreateFrom(new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracoesDigitalizacaoCanhotosNotasFiscais(protocolos));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosPorCarga(int? protocolo)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocolo ?? 0);

                if (carga == null)
                    throw new WebServiceException("Não foi possível encontrar a carga com o protocolo informado.");

                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repositorioCanhoto.BuscarPorCarga(carga.Codigo);
                Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto> retorno = new Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>();

                if ((canhotos == null || canhotos.Count == 0) && carga.CargaAgrupamento != null)
                    canhotos = repositorioCanhoto.BuscarPorCargaOrigem(carga.Codigo);

                retorno.NumeroTotalDeRegistro = canhotos.Count;
                retorno.Itens = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>();

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                    string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
                    string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);
                    byte[] bufferCanhoto = null;

                    if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

                    retorno.Itens.Add(new Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto()
                    {
                        Protocolo = canhoto.Codigo,
                        ChaveAcesso = canhoto.XMLNotaFiscal.Chave,
                        DataEnvioCanhoto = canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy HH:mm:ss"),
                        SituacaoCanhoto = canhoto.SituacaoCanhoto,
                        SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto,
                        Observacao = canhoto.Observacao,
                        ImagemCanhotoBase64 = bufferCanhoto != null ? Convert.ToBase64String(bufferCanhoto) : "",
                        NomeImagemCanhoto = canhoto?.NomeArquivo ?? "",
                        TipoCanhoto = canhoto.TipoCanhoto,
                        Latitude = !string.IsNullOrEmpty(canhoto.Latitude) ? canhoto.Latitude : "",
                        Longitude = !string.IsNullOrEmpty(canhoto.Longitude) ? canhoto.Longitude : "",
                        DataEntregaNota = canhoto.DataEntregaNotaCliente.HasValue ? canhoto.DataEntregaNotaCliente.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                        NumeroCanhoto = canhoto.Numero,
                        Digitalizacao = !string.IsNullOrEmpty(canhoto.DescricaoDigitalizacao) ? canhoto.DescricaoDigitalizacao : "",
                        DataDigitalizacao = canhoto.DataDigitalizacao.HasValue ? canhoto.DataDigitalizacao?.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    });
                }

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>.CriarRetornoSucesso(retorno);
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os canhotos digitalizados da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> EnviarImagemCanhoto(Stream arquivo)
        {
            ValidarToken();
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
                string caminhoCanhotosNaoIntegrados = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoCanhotos, "CanhotosNaoIntegrados");
                string nomeArquivo = Servicos.Arquivo.SalvarArquivoJPG(arquivo, caminhoCanhotosNaoIntegrados);
                Servicos.Log.TratarErro("EnviarImagemCanhoto: " + nomeArquivo);
                return Retorno<string>.CriarRetornoSucesso(nomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<string>.CriarRetornoExcecao("Ocorreu uma falha ao salvar o arquivo.");
            }
        }

        public Retorno<bool> EnviarCanhoto(string token, Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal entidadeNotaFiscal = BuscarEntidadeNotaFiscal(notaFiscal, unitOfWork);

                if (entidadeNotaFiscal == null)
                    throw new WebServiceException("Nota fiscal não encontrada");

                if (entidadeNotaFiscal.Canhoto == null)
                    throw new WebServiceException("Não existe um canhoto para essa nota fiscal");

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = entidadeNotaFiscal.Canhoto;

                if (canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado ||
                                canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao)
                    throw new WebServiceException("Esse canhoto já foi digitalizado ");

                string caminhoCanhotosNaoIntegrados = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoCanhotos, "CanhotosNaoIntegrados");
                string caminhoCanhotoNaoIntegrado = caminhoCanhotosNaoIntegrados + "/" + token + ".jpg";

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCanhotoNaoIntegrado))
                    throw new WebServiceException("Esse token não remete à nenhum canhoto enviado previamente");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                unitOfWork.Start();
                if (!configuracao.ExigeAprovacaoDigitalizacaoCanhoto)
                {
                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                    canhoto.DigitalizacaoIntegrada = false;
                    canhoto.DataDigitalizacao = DateTime.Now;
                    canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Integracao;

                    Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, configuracao, unitOfWork, TipoServicoMultisoftware, Cliente);
                    Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracao, TipoServicoMultisoftware, Cliente, unitOfWork);
                    Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);
                }
                else
                {
                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                    Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, configuracao, unitOfWork);
                }

                string caminhoCanhoto = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);

                string extensao = ".jpg";
                canhoto.GuidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "");
                canhoto.NomeArquivo = canhoto.Numero.ToString() + extensao;

                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhoto, canhoto.GuidNomeArquivo + extensao);

                // Mover a imagem da pasta de canhotos não integrados para a pasta certa do canhoto
                Utilidades.IO.FileStorageService.Storage.Move(caminhoCanhotoNaoIntegrado, fileLocation);

                canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Integracao;
                serCanhoto.GerarHistoricoCanhoto(canhoto, null, $"Imagem do Canhoto digitalizada via Mobile com o nome {canhoto.GuidNomeArquivo}", unitOfWork);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                {
                    serCanhoto.GerarHistoricoCanhoto(canhoto, null, $"Falha ao salvar imagem do Canhoto", unitOfWork);
                    Servicos.Log.TratarErro($"Falha ao salvar imagem do Canhoto de id {canhoto.Codigo} e guid {canhoto.GuidNomeArquivo}");
                }

                canhoto.DataEnvioCanhoto = DateTime.Now;
                canhoto.DataUltimaModificacao = DateTime.Now;

                canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Pendente;
                repCanhoto.Atualizar(canhoto);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Falha ao enviar a imagem do canhoto");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarEntidadeNotaFiscal(Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota = null;

            if (!string.IsNullOrEmpty(notaFiscal.Chave))
                nota = repXMLNotaFiscal.BuscarPorChave(notaFiscal.Chave);

            if (nota != null)
                return nota;

            string cpfCnpj = notaFiscal.Emitente?.CPFCNPJ?.Replace(".", "").Replace("-", "").Replace("/", "") ?? "";
            if (double.TryParse(cpfCnpj, out double cnpjEmitente))
                nota = repXMLNotaFiscal.BuscarPorNumeroSerieEmitente(notaFiscal.Numero, notaFiscal.Serie, cnpjEmitente);

            if (nota != null)
                return nota;

            return null;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCanhotos;
        }

        #endregion
    }
}
