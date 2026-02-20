using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Integracao
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ObterTotais" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaIntegracaoCTeController : BaseController
    {
		#region Construtores

		public CargaIntegracaoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);
                grid.AdicionarCabecalho("CT-e", "CTe", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emissor", "Emissor", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Retorno", 20, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);

                int countCTes = repIntegracao.ContarConsulta(codigoCarga, situacao, tipo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> listaIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

                if (countCTes > 0)
                {
                    if (propOrdena == "CTe")
                        propOrdena = "CargaCTe.CTe.Numero";
                    else if (propOrdena == "Emissor")
                        propOrdena = "CargaCTe.CTe.Empresa.CNPJ";
                    else if (propOrdena == "TipoIntegracao")
                        propOrdena = "TipoIntegracao.Tipo";
                    else if (propOrdena == "Tentativas")
                        propOrdena = "NumeroTentativas";
                    else if (propOrdena == "DataEnvio")
                        propOrdena = "DataIntegracao";
                    else if (propOrdena == "Situacao")
                        propOrdena = "SituacaoIntegracao";

                    listaIntegracao = repIntegracao.Consultar(codigoCarga, situacao, tipo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                }

                grid.setarQuantidadeTotal(countCTes);

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.SituacaoIntegracao,
                                       obj.TipoIntegracao.Tipo,
                                       CTe = obj.CargaCTe.CTe?.Numero.ToString() ?? "",
                                       Emissor = obj.CargaCTe.CTe?.Empresa.CNPJ_Formatado ?? obj.CargaCTe.PreCTe.Empresa.CNPJ_Formatado,
                                       Situacao = obj.DescricaoSituacaoIntegracao,
                                       TipoIntegracao = obj.TipoIntegracao.DescricaoTipo,
                                       Retorno = obj.ProblemaIntegracao,
                                       Tentativas = obj.NumeroTentativas,
                                       DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                       DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo :
                                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                                       DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repCargaEDIIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repCargaEDIIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repCargaEDIIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repCargaEDIIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações de CT-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao = repCargaCTeIntegracao.BuscarPorCodigo(codigo);

                if (cargaCTeIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (cargaCTeIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
                    return new JsonpResult(false, "Não é possível enviar as integrações da natura individualmente. Utilize a opção reenviar todos.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes) && cargaCTeIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unidadeDeTrabalho.Start();

                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                cargaCTeIntegracao.Lote = null;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeIntegracao, null, "Reenviou a integração", unidadeDeTrabalho);

                repCargaCTeIntegracao.Atualizar(cargaCTeIntegracao);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> ReenviarTodos()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoCarga = 0;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> integracoes = repCargaCTeIntegracao.BuscarPorCarga(codigoCarga, situacao, tipo);

                bool utilizarTransacao = false;
                if (integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                    utilizarTransacao = true;

                if (utilizarTransacao)
                    unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao integracao in integracoes)
                {
                    if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado && !integracao.TipoIntegracao.PermitirReenvioExcecao)
                        continue;

                    if (!utilizarTransacao)
                    {
                        unidadeDeTrabalho.FlushAndClear();
                    }

                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.Lote = null;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a integração", unidadeDeTrabalho);

                    repCargaCTeIntegracao.Atualizar(integracao);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Reenviou as integrações da Carga", unidadeDeTrabalho);

                if (utilizarTransacao)
                    unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
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
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta CT-e " + integracao.CargaCTe.CTe.Numero + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da DACTE.");
            }
        }
    }
}
