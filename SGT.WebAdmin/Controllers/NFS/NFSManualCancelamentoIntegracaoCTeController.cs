using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ObterTotais" }, "NFS/NFSManualCancelamento")]
    public class NFSManualCancelamentoIntegracaoCTeController : BaseController
    {
		#region Construtores

		public NFSManualCancelamentoIntegracaoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("Tipo");

                int codigoNFSManualCancelamento = Request.GetIntParam("NFSManualCancelamento");

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

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repIntegracao = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unidadeDeTrabalho);

                int countCTes = repIntegracao.ContarConsulta(codigoNFSManualCancelamento, situacao, tipo);

                List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> listaIntegracao = new List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

                if (countCTes > 0)
                {
                    if (propOrdena == "CTe")
                        propOrdena = "LancamentoNFSManualCTe.CTe.Numero";
                    else if (propOrdena == "Emissor")
                        propOrdena = "LancamentoNFSManualCTe.CTe.Empresa.CNPJ";
                    else if (propOrdena == "TipoIntegracao")
                        propOrdena = "TipoIntegracao.Tipo";
                    else if (propOrdena == "Tentativas")
                        propOrdena = "NumeroTentativas";
                    else if (propOrdena == "DataEnvio")
                        propOrdena = "DataIntegracao";
                    else if (propOrdena == "Situacao")
                        propOrdena = "SituacaoIntegracao";

                    listaIntegracao = repIntegracao.Consultar(codigoNFSManualCancelamento, situacao, tipo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                }

                grid.setarQuantidadeTotal(countCTes);

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.SituacaoIntegracao,
                                       obj.TipoIntegracao.Tipo,
                                       CTe = obj.NFSManualCancelamento.LancamentoNFSManual.CTe.Numero,
                                       Emissor = obj.NFSManualCancelamento.LancamentoNFSManual.CTe.Empresa.CNPJ_Formatado,
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
                int codigoNFSManualCancelamento = Request.GetIntParam("NFSManualCancelamento");

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repNFSManualCTeIntegracao.ContarPorNFSManualCancelamento(codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repNFSManualCTeIntegracao.ContarPorNFSManualCancelamento(codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repNFSManualCTeIntegracao.ContarPorNFSManualCancelamento(codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repNFSManualCTeIntegracao.ContarPorNFSManualCancelamento(codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repLancamentoNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe nfsManualCTeIntegracao = repLancamentoNFSManualCTeIntegracao.BuscarPorCodigo(codigo, false);

                if (nfsManualCTeIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (nfsManualCTeIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
                    return new JsonpResult(false, "Não é possível enviar as integrações da natura individualmente. Utilize a opção reenviar todos.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManualCTeIntegracao, null, "Reenviou a Integração.", unidadeDeTrabalho);

                nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                nfsManualCTeIntegracao.Lote = null;

                repLancamentoNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("Tipo");

                int codigoNFSManualCancelamento = Request.GetIntParam("NFSManualCancelamento");

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repLancamentoNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> integracoes = repLancamentoNFSManualCTeIntegracao.BuscarPorNFSManualCancelamento(codigoNFSManualCancelamento, situacao, tipo);

                bool utilizarTransacao = false;
                if (integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                    utilizarTransacao = true;

                if (utilizarTransacao)
                    unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe integracao in integracoes)
                {
                    if (!utilizarTransacao)
                    {
                        unidadeDeTrabalho.FlushAndClear();
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a Integração.", unidadeDeTrabalho);

                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.Lote = null;

                    repLancamentoNFSManualCTeIntegracao.Atualizar(integracao);
                }

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repIntegracao = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe integracao = repIntegracao.BuscarPorCodigo(codigo, false);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repIntegracao = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta NFS Manual " + integracao.NFSManualCancelamento.LancamentoNFSManual.CTe.Numero + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da DACTE.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
