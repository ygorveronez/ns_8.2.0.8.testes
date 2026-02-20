using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/PreFaturaNatura")]
    public class PreFaturaNaturaController : BaseController
    {
		#region Construtores

		public PreFaturaNaturaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoEmpresa;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                long numeroPreFatura;
                long.TryParse(Request.Params("NumeroPreFatura"), out numeroPreFatura);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Pré Fat.", "NumeroPreFatura", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Fatura", "NumeroFatura", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Pré Fat.", "DataPreFatura", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Pré Fat. ", "ValorPreFatura", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Desc.", "ValorDesconto", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorReceber", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Docs.", "QuantidadeDocumentos", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mensagem", "MensagemSituacao", 15, Models.Grid.Align.left, true);

                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propriedadeOrdenar == "NumeroFatura")
                    propriedadeOrdenar = "Fatura.Numero";
                else if (propriedadeOrdenar == "ValorPreFatura")
                    propriedadeOrdenar = "ValorFrete";
                else if (propriedadeOrdenar == "ValorDesconto")
                    propriedadeOrdenar = "ValorTotalDescontoItens";
                else if (propriedadeOrdenar == "ValorReceber")
                    propriedadeOrdenar = "ValorTotalReceberCTes";
                else if (propriedadeOrdenar == "QuantidadeDocumentos")
                    propriedadeOrdenar = "QuantidadeItens";

                Repositorio.Embarcador.Integracao.PreFaturaNatura repPreFatura = new Repositorio.Embarcador.Integracao.PreFaturaNatura(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura> preFaturas = repPreFatura.Consultar(situacao, codigoEmpresa, numeroPreFatura, dataInicial, dataFinal, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repPreFatura.ContarConsulta(situacao, codigoEmpresa, numeroPreFatura, dataInicial, dataFinal));

                grid.AdicionaRows((from obj in preFaturas
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.NumeroPreFatura,
                                       NumeroFatura = obj.Fatura?.Numero,
                                       DataPreFatura = obj.DataPreFatura.ToString("dd/MM/yyyy"),
                                       ValorPreFatura = obj.ValorFrete,
                                       ValorDesconto = obj.ValorTotalDescontoItens.ToString("n2"), //obj.Itens.Sum(o => o.ValorDoDesconto).ToString("n2"),
                                       ValorReceber = obj.ValorTotalReceberCTes.ToString("n2"), //obj.Itens.Sum(o => o.CargaCTe.CTe.ValorAReceber).ToString("n2"),
                                       QuantidadeDocumentos = obj.QuantidadeItens.ToString(),
                                       Situacao = obj.DescricaoSituacao,
                                       obj.MensagemSituacao
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar as pré faturas.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarPreFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long numeroPreFatura;
                long.TryParse(Request.Params("NumeroPreFatura"), out numeroPreFatura);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int codigoEmpresa;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                Servicos.Embarcador.Integracao.Natura.IntegracaoFaturaNatura svcPreFatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoFaturaNatura();
                svcPreFatura.ConsultarPreFaturas(Usuario, empresa, numeroPreFatura, dataInicial, dataFinal, false, unidadeDeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar as pré faturas.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarDocumentos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int numero, codigoPreFatura;
                int.TryParse(Request.Params("Numero"), out numero);
                int.TryParse(Request.Params("Codigo"), out codigoPreFatura);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoItem", false);
                grid.AdicionarCabecalho("Número", "Numero", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("DT", "DT", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Empresa", "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorAReceber", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Status", 15, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Empresa")
                    propOrdena = "CargaCTe.CTe.Empresa.RazaoSocial";
                else if (propOrdena == "ValorAReceber" || propOrdena == "DataEmissao" || propOrdena == "Numero" || propOrdena == "Status")
                    propOrdena = "CargaCTe.CTe." + propOrdena;
                else if (propOrdena == "DT")
                    propOrdena = "DocumentoTransporte.Numero";

                Repositorio.Embarcador.Integracao.ItemPreFaturaNatura repItemPreFatura = new Repositorio.Embarcador.Integracao.ItemPreFaturaNatura(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura> itens = repItemPreFatura.Consultar(codigoPreFatura, numero, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repItemPreFatura.ContarConsulta(codigoPreFatura, numero));

                grid.AdicionaRows((from obj in itens
                                   select new
                                   {
                                       obj.CargaCTe.CTe.Codigo,
                                       CodigoItem = obj.Codigo,
                                       Numero = obj.CargaCTe.CTe.Numero.ToString() + "-" + obj.CargaCTe.CTe.Serie.Numero.ToString(),
                                       DataEmissao = obj.CargaCTe.CTe.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                       DT = obj.DocumentoTransporte.Numero.ToString(),
                                       Empresa = obj.CargaCTe.CTe.Empresa.RazaoSocial,
                                       ValorAReceber = obj.CargaCTe.CTe.ValorAReceber.ToString("n2"),
                                       Status = obj.CargaCTe.CTe.DescricaoStatus
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os documentos.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirDocumento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoItemPreFatura);

                Repositorio.Embarcador.Integracao.ItemPreFaturaNatura repItemPreFatura = new Repositorio.Embarcador.Integracao.ItemPreFaturaNatura(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura itemPreFatura = repItemPreFatura.BuscarPorCodigo(codigoItemPreFatura);

                repItemPreFatura.Deletar(itemPreFatura, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao remover o item da fatura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirDocumentosCancelados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoPreFatura);

                Repositorio.Embarcador.Integracao.ItemPreFaturaNatura repItemPreFatura = new Repositorio.Embarcador.Integracao.ItemPreFaturaNatura(unitOfWork);
                Repositorio.Embarcador.Integracao.PreFaturaNatura repPreFaturaNatura = new Repositorio.Embarcador.Integracao.PreFaturaNatura(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura preFatura = repPreFaturaNatura.BuscarPorCodigo(codigoPreFatura);

                List<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura> itensPreFatura = repItemPreFatura.BuscarCanceladosPorPreFatura(codigoPreFatura);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura itemPreFatura in itensPreFatura)
                    repItemPreFatura.Deletar(itemPreFatura);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, preFatura, "Removeu os documentos cancelados/anulados da pré fatura.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao remover o item da fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarPreFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPreFatura;
                int.TryParse(Request.Params("Codigo"), out codigoPreFatura);

                Repositorio.Embarcador.Integracao.PreFaturaNatura repPreFatura = new Repositorio.Embarcador.Integracao.PreFaturaNatura(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura preFatura = repPreFatura.BuscarPorCodigo(codigoPreFatura);

                if (preFatura.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura.Gerada)
                    return new JsonpResult(false, true, "Não é possível atualizar uma fatura emitida.");

                Servicos.Embarcador.Integracao.Natura.IntegracaoFaturaNatura svcPreFatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoFaturaNatura();
                svcPreFatura.ConsultarPreFaturas(Usuario, preFatura.Empresa, preFatura.NumeroPreFatura, preFatura.DataPreFatura, preFatura.DataPreFatura, true, unidadeDeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, preFatura, null, "Atualizou a Pre Fatura.", unidadeDeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os documentos de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> GerarFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPreFatura;
                int.TryParse(Request.Params("Codigo"), out codigoPreFatura);

                Repositorio.Embarcador.Integracao.PreFaturaNatura repPreFatura = new Repositorio.Embarcador.Integracao.PreFaturaNatura(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura preFatura = repPreFatura.BuscarPorCodigo(codigoPreFatura);

                Servicos.Embarcador.Integracao.Natura.IntegracaoFaturaNatura svcPreFatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoFaturaNatura();
                string erro = string.Empty;

                unidadeDeTrabalho.Start();

                if (!svcPreFatura.GerarFatura(out erro, preFatura, Usuario, unidadeDeTrabalho))
                {
                    unidadeDeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, preFatura, null, "Gerou Fatura da Pre Fatura.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar a fatura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracaoGeral()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoNatura repIntegracaoNatura = new Repositorio.Embarcador.Integracao.IntegracaoNatura(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Protocolo", "Protocolo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 20, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura> integracoes = repIntegracaoNatura.Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura.Fatura, false, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repIntegracaoNatura.ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura.Fatura, false));

                var retorno = (from obj in integracoes
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.Protocolo,
                                   obj.Retorno,
                                   Usuario = obj.Usuario?.Nome
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

        public async Task<IActionResult> ConsultarHistoricoIntegracaoPreFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.PreFaturaNatura repPreFaturaNatura = new Repositorio.Embarcador.Integracao.PreFaturaNatura(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura preFatura = repPreFaturaNatura.BuscarPorCodigo(codigo);

                if (preFatura == null)
                    return new JsonpResult(false, true, "Documento de transporte não encontrado.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Protocolo", "Protocolo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 20, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(preFatura.Integracoes.Count());

                var retorno = (from obj in preFatura.Integracoes.OrderByDescending(o => o.DataConsulta).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.Protocolo,
                                   obj.Retorno,
                                   Usuario = obj.Usuario?.Nome
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
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoPreFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoNatura repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoNatura(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura integracao = repIntegracao.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Integração não encontrada.");

                if (integracao.ArquivoRequisicao == null && integracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracao.ArquivoRequisicao, integracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta Pré Fatura.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #region Métodos Privados

        #endregion
    }
}
