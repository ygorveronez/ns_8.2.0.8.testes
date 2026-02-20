using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/ObservacaoFiscal")]
    public class ObservacaoFiscalController : BaseController
    {
		#region Construtores

		public ObservacaoFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal repObservacaoFiscal = new Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal(unitOfWork);

                string descricao = Request.Params("Descricao");
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Observação", "Observacao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("NCM", "NCMProduto", 15, Models.Grid.Align.left, true);

                List<Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal> listaObservacaoFiscal = repObservacaoFiscal.Consultar(descricao, codigoEmpresa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repObservacaoFiscal.ContarConsulta(descricao, codigoEmpresa));
                var lista = (from p in listaObservacaoFiscal
                            select new
                            {
                                p.Codigo,
                                p.Observacao,
                                p.NCMProduto
                            }).ToList();
                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal repObservacaoFiscal = new Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal observacaoFiscal = new Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal();

                int empresa, atividade, naturezaOperacao, codigocfop = 0;                
                
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS cstICMS;
                
                string observacao = Request.Params("Observacao");
                string ncmProduto = Request.Params("NCMProduto");
                string estado = Request.Params("Estado");

                int.TryParse(Request.Params("Empresa"), out empresa);
                int.TryParse(Request.Params("Atividade"), out atividade);
                int.TryParse(Request.Params("NaturezaDaOperacao"), out naturezaOperacao);
                int.TryParse(Request.Params("CFOPNotaFiscal"), out codigocfop);
                empresa = this.Usuario.Empresa.Codigo;
                
                Enum.TryParse(Request.Params("CSTICMS"), out cstICMS);

                if (atividade > 0)
                    observacaoFiscal.Atividade = repAtividade.BuscarPorCodigo(atividade);
                else
                    observacaoFiscal.Atividade = null;
                if (codigocfop > 0)
                    observacaoFiscal.CFOP = repCFOP.BuscarPorCodigo(codigocfop);
                else
                    observacaoFiscal.CFOP = null;
                if ((int)cstICMS > 0)
                    observacaoFiscal.CSTICMS = cstICMS;
                else
                    observacaoFiscal.CSTICMS = null;                
                if (!string.IsNullOrWhiteSpace(estado))
                    observacaoFiscal.Estado = repEstado.BuscarPorSigla(estado);
                else
                    observacaoFiscal.Estado = null;
                if (naturezaOperacao > 0)
                    observacaoFiscal.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(naturezaOperacao);
                else
                    observacaoFiscal.NaturezaDaOperacao = null;

                observacaoFiscal.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                observacaoFiscal.NCMProduto = ncmProduto;
                observacaoFiscal.Observacao = observacao;

                repObservacaoFiscal.Inserir(observacaoFiscal, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal repObservacaoFiscal = new Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal observacaoFiscal = repObservacaoFiscal.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                int empresa, atividade, naturezaOperacao, codigocfop = 0;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS cstICMS;

                string observacao = Request.Params("Observacao");
                string ncmProduto = Request.Params("NCMProduto");
                string estado = Request.Params("Estado");

                int.TryParse(Request.Params("Empresa"), out empresa);
                int.TryParse(Request.Params("Atividade"), out atividade);
                int.TryParse(Request.Params("NaturezaDaOperacao"), out naturezaOperacao);
                int.TryParse(Request.Params("CFOPNotaFiscal"), out codigocfop);
                empresa = this.Usuario.Empresa.Codigo;

                Enum.TryParse(Request.Params("CSTICMS"), out cstICMS);

                if (atividade > 0)
                    observacaoFiscal.Atividade = repAtividade.BuscarPorCodigo(atividade);
                else
                    observacaoFiscal.Atividade = null;
                if (codigocfop > 0)
                    observacaoFiscal.CFOP = repCFOP.BuscarPorCodigo(codigocfop);
                else
                    observacaoFiscal.CFOP = null;
                if ((int)cstICMS > 0)
                    observacaoFiscal.CSTICMS = cstICMS;
                else
                    observacaoFiscal.CSTICMS = null;
                if (!string.IsNullOrWhiteSpace(estado))
                    observacaoFiscal.Estado = repEstado.BuscarPorSigla(estado);
                else
                    observacaoFiscal.Estado = null;
                if (naturezaOperacao > 0)
                    observacaoFiscal.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(naturezaOperacao);
                else
                    observacaoFiscal.NaturezaDaOperacao = null;

                observacaoFiscal.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                observacaoFiscal.NCMProduto = ncmProduto;
                observacaoFiscal.Observacao = observacao;


                repObservacaoFiscal.Atualizar(observacaoFiscal, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal repObservacaoFiscal = new Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal observacaoFiscal = repObservacaoFiscal.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {                    
                    Atividade = observacaoFiscal.Atividade != null ? new { Codigo = observacaoFiscal.Atividade.Codigo, Descricao = observacaoFiscal.Atividade.Descricao } : null,
                    CFOPNotaFiscal = observacaoFiscal.CFOP != null ? new { Codigo = observacaoFiscal.CFOP.Codigo, Descricao = observacaoFiscal.CFOP.CodigoCFOP } : null,
                    observacaoFiscal.Codigo,
                    observacaoFiscal.CSTICMS,                    
                    Estado = observacaoFiscal.Estado != null ? new { Codigo = observacaoFiscal.Estado.Sigla, Descricao = observacaoFiscal.Estado.Nome } : null,
                    NaturezaDaOperacao = observacaoFiscal.NaturezaDaOperacao != null ? new { Codigo = observacaoFiscal.NaturezaDaOperacao.Codigo, Descricao = observacaoFiscal.NaturezaDaOperacao.Descricao } : null,
                    observacaoFiscal.NCMProduto,
                    observacaoFiscal.Observacao,                    
                    Empresa = observacaoFiscal.Empresa != null ? new { Codigo = observacaoFiscal.Empresa.Codigo, Descricao = observacaoFiscal.Empresa.RazaoSocial } : null
                };
                return new JsonpResult(dynProcessoMovimento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal repObservacaoFiscal = new Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal observacaoFiscal = repObservacaoFiscal.BuscarPorCodigo(codigo);
                repObservacaoFiscal.Deletar(observacaoFiscal, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
