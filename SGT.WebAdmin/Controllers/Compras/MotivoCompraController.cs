using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Compras
{
    [CustomAuthorize("Compras/MotivoCompra")]
    public class MotivoCompraController : BaseController
    {
		#region Construtores

		public MotivoCompraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Compras.MotivoCompra motivo = repMotivoCompra.BuscarPorCodigo(codigo);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    motivo.Codigo,
                    motivo.Descricao,
                    Status = motivo.Ativo,
                    motivo.CodigoIntegracao,
                    motivo.Observacao,
                    motivo.ExigeInformarVeiculoObrigatoriamente,
                    motivo.Forma,
                    motivo.GerarImpressaoOC
                };

                return new JsonpResult(retorno);
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

                Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.MotivoCompra motivo = new Dominio.Entidades.Embarcador.Compras.MotivoCompra();

                PreencheEntidade(motivo, unitOfWork);

                if (!ValidaEntidade(motivo, out string erro))
                    return new JsonpResult(false, true, erro);

                repMotivoCompra.Inserir(motivo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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

                Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Compras.MotivoCompra motivo = repMotivoCompra.BuscarPorCodigo(codigo, true);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencheEntidade(motivo, unitOfWork);

                if (!ValidaEntidade(motivo, out string erro))
                    return new JsonpResult(false, true, erro);

                repMotivoCompra.Atualizar(motivo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                unitOfWork.Start();

                Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Compras.MotivoCompra motivo = repMotivoCompra.BuscarPorCodigo(codigo, true);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repMotivoCompra.Deletar(motivo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("ExigeInformarVeiculoObrigatoriamente");
            grid.Prop("Descricao").Nome("Descrição").Tamanho(35).Align(Models.Grid.Align.left);

            if (Request.GetEnumParam<SituacaoAtivoPesquisa>("Status") == SituacaoAtivoPesquisa.Todos)
                grid.Prop("Ativo").Nome("Status").Tamanho(25).Align(Models.Grid.Align.left);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);

            SituacaoAtivoPesquisa status = Request.GetEnumParam<SituacaoAtivoPesquisa>("Status");

            string descricao = Request.Params("Descricao");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            List<Dominio.Entidades.Embarcador.Compras.MotivoCompra> listaGrid = repMotivoCompra.Consultar(codigoEmpresa, descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMotivoCompra.ContarConsulta(codigoEmpresa, descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            Ativo = obj.DescricaoAtivo,
                            obj.ExigeInformarVeiculoObrigatoriamente
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(Dominio.Entidades.Embarcador.Compras.MotivoCompra motivo, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            bool.TryParse(Request.Params("Status"), out bool ativo);

            motivo.Descricao = descricao;
            motivo.Observacao = observacao;
            motivo.Ativo = ativo;
            motivo.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            motivo.ExigeInformarVeiculoObrigatoriamente = Request.GetBoolParam("ExigeInformarVeiculoObrigatoriamente");
            motivo.Forma = Request.GetEnumParam<FormaRequisicaoMercadoria>("Forma");
            motivo.GerarImpressaoOC = Request.GetBoolParam("GerarImpressaoOC");

            if (motivo.Codigo == 0)
                motivo.Empresa = this.Usuario.Empresa;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Compras.MotivoCompra motivo, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(motivo.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
        }

        #endregion
    }
}
