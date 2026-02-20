using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize(new string[] { "PesquisaCTeRemoverIntegracao", "CarregarDadosTotalizadores", "PesquisaIntegracaoEDI", "PesquisaIntegracaoFatura", "ObterTotaisEDI", "ObterTotaisFatura" }, "Faturas/Fatura", "Cargas/Carga", "SAC/AtendimentoCliente")]
    public class FaturaIntegracaoController : BaseController
    {
		#region Construtores

		public FaturaIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> CarregarDadosTotalizadores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);

                int codigoFatura = 0;
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                var dynRetorno = new
                {
                    Codigo = fatura.Codigo,
                    TotalEDI = repFaturaIntegracao.TotalArquivos(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI).ToString("n0"),
                    AguardandoIntegracaoEDI = repFaturaIntegracao.TotalArquivosStatus(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao).ToString("n0"),
                    IntegradoEDI = repFaturaIntegracao.TotalArquivosStatus(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado).ToString("n0"),
                    RejeitadoEDI = repFaturaIntegracao.TotalArquivosStatus(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao).ToString("n0"),
                    TotalFatura = repFaturaIntegracao.TotalArquivos(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura).ToString("n0"),
                    AguardandoIntegracaoFatura = repFaturaIntegracao.TotalArquivosStatus(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao).ToString("n0"),
                    IntegradoFatura = repFaturaIntegracao.TotalArquivosStatus(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado).ToString("n0"),
                    RejeitadoFatura = repFaturaIntegracao.TotalArquivosStatus(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao).ToString("n0")
                };

                return new JsonpResult(dynRetorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os totalizadores da integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracaoEDI()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFatura = Request.GetIntParam("Codigo");
                int codigoCarga = Request.GetIntParam("Carga");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("TipoConsultaEDI") ?? Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("FaturaRecebidaDeIntegracao", false);
                grid.AdicionarCabecalho("Layout EDI", "LayoutEDI", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "MensagemRetorno", 30, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "TipoIntegracao")
                    propOrdenar = "TipoIntegracao.Descricao";

                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> listaFaturaIntegracao = repFaturaIntegracao.BuscarPorFatura(codigoFatura, codigoCarga, situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repFaturaIntegracao.ContarBuscarPorFatura(codigoFatura, codigoCarga, situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI));

                var dynRetorno = (from obj in listaFaturaIntegracao
                                  select new
                                  {
                                      obj.Codigo,
                                      FaturaRecebidaDeIntegracao = obj.Fatura.FaturaRecebidaDeIntegracao,
                                      LayoutEDI = obj.LayoutEDI?.Descricao ?? string.Empty,
                                      SituacaoIntegracao = obj.DescricaoSituacaoIntegracao,
                                      TipoIntegracao = obj.TipoIntegracao?.Descricao ?? "",
                                      Tentativas = obj.Tentativas.ToString(),
                                      DataEnvio = obj.DataEnvio?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                                      obj.MensagemRetorno,
                                      DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ? "#ADD8E6" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? "#DFF0D8" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? "#C16565" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? "#F7F7BA" :
                                                    "#FFFFFF",
                                      //DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? "#FFFFFF" : "#CCCCCC"
                                      DT_FontColor = ""
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as integrações de EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracaoFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFatura = Request.GetIntParam("Codigo");
                int codigoCarga = Request.GetIntParam("Carga");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("TipoConsultaFatura") ?? Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("FaturaRecebidaDeIntegracao", false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "MensagemRetorno", 45, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "TipoIntegracao")
                    propOrdenar = "TipoIntegracao.Descricao";

                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> listaFaturaIntegracao = repFaturaIntegracao.BuscarPorFatura(codigoFatura, codigoCarga, situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturaIntegracao.ContarBuscarPorFatura(codigoFatura, codigoCarga, situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura));
                var dynRetorno = (from obj in listaFaturaIntegracao
                                  select new
                                  {
                                      obj.Codigo,
                                      FaturaRecebidaDeIntegracao = obj.Fatura.FaturaRecebidaDeIntegracao,
                                      TipoIntegracao = obj.TipoIntegracao?.Descricao ?? "",
                                      SituacaoIntegracao = obj.DescricaoSituacaoIntegracao,
                                      Tentativas = obj.Tentativas.ToString(),
                                      DataEnvio = obj.DataEnvio?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                                      obj.MensagemRetorno,
                                      DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ? "#ADD8E6" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? "#DFF0D8" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? "#C16565" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? "#F7F7BA" :
                                                    "#FFFFFF",
                                      DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? "#FFFFFF" : "#666666"
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as integrações de Fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarLayoutEDI()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteEnviarEDI)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para enviar o arquivo de layout de EDI.");

                int codigoIntegracao = 0;
                int.TryParse(Request.Params("Codigo"), out codigoIntegracao);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = repFaturaIntegracao.BuscarPorCodigo(codigoIntegracao);

                if (integracao.Fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, "Não é possível executar esta ação para fatura recebida pela integração.");

                integracao.DataEnvio = DateTime.Now;
                integracao.Tentativas = integracao.Tentativas + 1;
                integracao.IniciouConexaoExterna = false;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repFaturaIntegracao.Atualizar(integracao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a integração.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Fatura, null, "Reenviou a integração " + integracao.Descricao + ".", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar layout EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarLayoutFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteEnviarFatura)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para enviar o arquivo de layout de Fatura.");

                int codigoIntegracao = 0;
                int.TryParse(Request.Params("Codigo"), out codigoIntegracao);

                int codigoFatura = 0;
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = null;
                if (codigoIntegracao > 0)
                    integracao = repFaturaIntegracao.BuscarPorCodigo(codigoIntegracao);
                else if (codigoFatura > 0)
                    integracao = repFaturaIntegracao.BuscarLayoutFaturaPorFatura(codigoFatura);

                if (integracao == null)
                    return new JsonpResult(false, "Não foi possível localizar a integração da fatura selecionada.");

                if ((integracao.Fatura.Situacao != SituacaoFatura.Cancelado) && (integracao.Fatura.Situacao != SituacaoFatura.Fechado) && (integracao.Fatura.Situacao != SituacaoFatura.ProblemaIntegracao) && (integracao.TipoIntegracao.Tipo != TipoIntegracao.Intercab))
                    return new JsonpResult(false, "Esta fatura não se encontra fechada para enviar o layout.");

                if (integracao.Fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, "Não é possível executar esta ação para fatura recebida pela integração.");

                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.IniciouConexaoExterna = false;
                integracao.MensagemRetorno = string.Empty;

                repFaturaIntegracao.Atualizar(integracao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a fatura.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Fatura, null, "Reenviou a fatura " + integracao.Descricao + ".", unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração da fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarTodosLayoutFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteEnviarEDI)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para enviar o arquivo de layout de EDI.");

                int codigoFatura = 0;
                int.TryParse(Request.Params("Codigo"), out codigoFatura);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> listaIntegracao = repFaturaIntegracao.BuscarPorFatura(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura);
                unitOfWork.Start();
                foreach (var integracao in listaIntegracao)
                {
                    if (integracao.Fatura.FaturaRecebidaDeIntegracao)
                        return new JsonpResult(false, "Não é possível executar esta ação para fatura recebida pela integração.");

                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.IniciouConexaoExterna = false;
                    integracao.MensagemRetorno = string.Empty;

                    repFaturaIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a fatura.", unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Fatura, null, "Reenviou a fatura " + integracao.Descricao + ".", unitOfWork);
                }
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar layout EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarTodosLayoutEDI()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteEnviarEDI)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para enviar o arquivo de layout de EDI.");

                int codigoFatura = 0;
                int.TryParse(Request.Params("Codigo"), out codigoFatura);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> listaIntegracao = repFaturaIntegracao.BuscarPorFatura(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI);

                for (int i = 0; i < listaIntegracao.Count(); i++)
                {

                    if (listaIntegracao[i].Fatura.FaturaRecebidaDeIntegracao)
                        return new JsonpResult(false, "Não é possível executar esta ação para fatura recebida pela integração.");

                    listaIntegracao[i].DataEnvio = DateTime.Now;
                    listaIntegracao[i].Tentativas = listaIntegracao[i].Tentativas + 1;
                    listaIntegracao[i].SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    listaIntegracao[i].IniciouConexaoExterna = false;
                    repFaturaIntegracao.Atualizar(listaIntegracao[i]);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, listaIntegracao[i], null, "Reenviou a integração.", unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, listaIntegracao[i].Fatura, null, "Reenviou a integração " + listaIntegracao[i].Descricao + ".", unitOfWork);
                }

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar layout EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLayout()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoIntegracao = 0;
                int.TryParse(Request.Params("Codigo"), out codigoIntegracao);

                if (codigoIntegracao > 0)
                {
                    Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
                    Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = repFaturaIntegracao.BuscarPorCodigo(codigoIntegracao);

                    //if (integracao != null && !string.IsNullOrWhiteSpace(integracao.ArquivoRetorno))
                    //{

                    //    byte[] data = System.Text.Encoding.Default.GetBytes(integracao.ArquivoRetorno);

                    //    if (data != null)
                    //    {
                    //        return Arquivo(data, "text/xml", string.Concat(integracao.Codigo, ".txt"));
                    //    }
                    //}
                    //else
                    return new JsonpResult(false, false, "Este layout não possui arquivo de retorno disponível para download.");
                }

                return new JsonpResult(false, false, "Arquivo não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadEDI()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_DownloadArquivoIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoIntegracao;
                int.TryParse(Request.Params("Codigo"), out codigoIntegracao);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = repFaturaIntegracao.BuscarPorCodigo(codigoIntegracao);

                if (integracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (integracao.Fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, "Não é possível executar esta ação para fatura recebida pela integração.");

                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(integracao, unidadeDeTrabalho);
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao, unidadeDeTrabalho, ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);

                return Arquivo(edi, "plain/text", nomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadTodosEDI()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_DownloadArquivoIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoFatura;
                int.TryParse(Request.Params("Codigo"), out codigoFatura);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unidadeDeTrabalho);
                List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> integracoes = repFaturaIntegracao.BuscarPorFatura(codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI);

                if (integracoes == null || integracoes.Count == 0)
                    return new JsonpResult(false, "Integrações não encontradas.");

                if (integracoes.FirstOrDefault().Fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, "Não é possível executar esta ação para fatura recebida pela integração.");

                System.IO.MemoryStream arquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarLoteEDIFatura(integracoes, TipoServicoMultisoftware, unidadeDeTrabalho, _conexao.StringConexao);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(integracoes[0].Fatura.Numero.ToString("D")), "_EDIs.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarCTeRemoverIntegracao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("CTe"), out int codigoCTe);
                int.TryParse(Request.Params("Fatura"), out int codigoFatura);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaIntegracaoCTeRemover repFaturaIntegracaoCTeRemover = new Repositorio.Embarcador.Fatura.FaturaIntegracaoCTeRemover(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover cteRemover = repFaturaIntegracaoCTeRemover.BuscarPorCTeEFatura(codigoCTe, codigoFatura);

                if (cteRemover != null)
                    return new JsonpResult(false, true, "O CT-e já foi adicionado.");

                cteRemover = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover()
                {
                    CTe = repCTe.BuscarPorCodigo(codigoCTe),
                    Fatura = repFatura.BuscarPorCodigo(codigoFatura)
                };

                repFaturaIntegracaoCTeRemover.Inserir(cteRemover);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar o CT-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirCTeRemoverIntegracao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Fatura.FaturaIntegracaoCTeRemover repFaturaIntegracaoCTeRemover = new Repositorio.Embarcador.Fatura.FaturaIntegracaoCTeRemover(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover cteRemover = repFaturaIntegracaoCTeRemover.BuscarPorCodigo(codigo, false);

                if (cteRemover == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                repFaturaIntegracaoCTeRemover.Deletar(cteRemover);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar o CT-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaCTeRemoverIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoFatura);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CT-e", "Numero", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "ValorAReceber", 11, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar != "Codigo")
                    propOrdenar = "CTe." + propOrdenar;

                Repositorio.Embarcador.Fatura.FaturaIntegracaoCTeRemover repFaturaIntegracaoCTeRemover = new Repositorio.Embarcador.Fatura.FaturaIntegracaoCTeRemover(unitOfWork);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover> listaFaturaIntegracaoCTeRemover = repFaturaIntegracaoCTeRemover.Consultar(codigoFatura, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repFaturaIntegracaoCTeRemover.ContarConsulta(codigoFatura));

                var dynRetorno = (from obj in listaFaturaIntegracaoCTeRemover
                                  select new
                                  {
                                      obj.Codigo,
                                      Numero = obj.CTe.Numero + " - " + obj.CTe.Serie.Numero,
                                      DataEmissao = obj.CTe.DataEmissao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                      ValorAReceber = obj.CTe.ValorAReceber.ToString("n2")
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosIntegracao.Count());

                var retorno = (from obj in integracao.ArquivosIntegracao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoEnvioEDI()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFaturaIntegracao = 0;
                int.TryParse(Request.Params("Codigo"), out codigoFaturaIntegracao);

                Repositorio.Embarcador.Fatura.FaturaLog repFaturaLog = new Repositorio.Embarcador.Fatura.FaturaLog(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "DataHora", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoLogFatura", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", "EmailDestinoEDI", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaLog> logs = repFaturaLog.BuscarPorCodigoFaturaIntegracaoTipo(codigoFaturaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.EnviouEDI);
                grid.setarQuantidadeTotal(logs.Count());

                var retorno = (from obj in logs.OrderByDescending(o => o.DataHora).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   DataHora = obj.DataHora.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipoLogFatura,
                                   obj.EmailDestinoEDI
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo arquivoIntegracao = integracao.ArquivosIntegracao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Fatura " + integracao.Fatura.Numero + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da DACTE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotaisEDI()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);


                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repFaturaIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI);
                int totalIntegrado = repFaturaIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI);
                int totalProblemaIntegracao = repFaturaIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.EDI);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações de EDI.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotaisFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repFaturaIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura);
                int totalIntegrado = repFaturaIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura);
                int totalProblemaIntegracao = repFaturaIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações de EDI.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
