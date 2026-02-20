using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/Porto")]
    public class PortoController : BaseController
    {
		#region Construtores

		public PortoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 75, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("CodigoLocalidade", false);
                grid.AdicionarCabecalho("Localidade", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Pedidos.Porto> listaPorto = repPorto.Consultar(descricao, ativo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPorto.ContarConsulta(descricao, ativo));

                var retorno = (from obj in listaPorto
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.DescricaoAtivo,
                                   CodigoLocalidade = obj.Localidade?.Codigo ?? 0,
                                   Localidade = obj.Localidade?.DescricaoCidadeEstado ?? ""
                               }).ToList();
                grid.AdicionaRows(retorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.Porto porto = new Dominio.Entidades.Embarcador.Pedidos.Porto();

                PreencherPorto(porto, unidadeDeTrabalho);
                repPorto.Inserir(porto, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.Porto porto = repPorto.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                PreencherPorto(porto, unidadeDeTrabalho);
                repPorto.Atualizar(porto, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.Porto porto = repPorto.BuscarPorCodigo(codigo);
                var retorno = new
                {
                    porto.Ativo,
                    porto.Codigo,
                    porto.Descricao,
                    porto.CodigoIATA,
                    porto.CodigoIntegracao,
                    porto.CodigoDocumento,
                    porto.CodigoMercante,
                    Localidade = porto.Localidade != null ? new { porto.Localidade.Codigo, porto.Localidade.Descricao } : null,
                    Empresa = porto.Empresa != null ? new { porto.Empresa.Codigo, porto.Empresa.Descricao } : null,
                    porto.FormaEmissaoSVM,
                    porto.QuantidadeHorasFaturamentoAutomatico,
                    porto.DividirCargasAcordoComQuantidadeContainerRecebidoPortoDestino,
                    porto.DividirCargasAcordoComQuantidadeContainerRecebidoPortoOrigem,
                    porto.AtivarDespachanteComoConsignatario,
                    porto.DiasAntesDoPodParaEnvioDaDocumentacao,
                    porto.RKST,
                    porto.CriarSequenciaCargasMesmoComPedidoExistente,
                    porto.GerarUmaCargaSVMPorCargaMTLQuandoPortoDestino,
                    porto.GerarUmaCargaSVMPorCargaMTLQuandoPortoOrigem,
                };

                unidadeDeTrabalho.Dispose();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeDeTrabalho.Dispose();

                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.Porto porto = repPorto.BuscarPorCodigo(codigo);

                repPorto.Deletar(porto, Auditado);

                unidadeDeTrabalho.Dispose();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Dispose();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);

                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
        }

        #endregion


        #region Métodos Privados

        private void PreencherPorto(Dominio.Entidades.Embarcador.Pedidos.Porto porto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            int.TryParse(Request.Params("Localidade"), out int localidade);
            int.TryParse(Request.Params("Empresa"), out int empresa);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEmissaoSVM formaEmissaoSVM;
            Enum.TryParse(Request.Params("FormaEmissaoSVM"), out formaEmissaoSVM);

            porto.Ativo = bool.Parse(Request.Params("Ativo"));
            porto.Descricao = Request.Params("Descricao");
            porto.CodigoIATA = Request.Params("CodigoIATA");
            porto.CodigoMercante = Request.Params("CodigoMercante");
            porto.CodigoIntegracao = Request.Params("CodigoIntegracao");
            porto.Localidade = localidade > 0 ? repLocalidade.BuscarPorCodigo(localidade) : null;
            porto.Empresa = empresa > 0 ? repEmpresa.BuscarPorCodigo(empresa) : null;
            porto.FormaEmissaoSVM = formaEmissaoSVM;
            porto.CodigoDocumento = Request.GetStringParam("CodigoDocumento");
            porto.QuantidadeHorasFaturamentoAutomatico = Request.GetIntParam("QuantidadeHorasFaturamentoAutomatico");
            porto.AtivarDespachanteComoConsignatario = Request.GetBoolParam("AtivarDespachanteComoConsignatario");
            porto.DividirCargasAcordoComQuantidadeContainerRecebidoPortoDestino = Request.GetBoolParam("DividirCargasAcordoComQuantidadeContainerRecebidoPortoDestino");
            porto.DividirCargasAcordoComQuantidadeContainerRecebidoPortoOrigem = Request.GetBoolParam("DividirCargasAcordoComQuantidadeContainerRecebidoPortoOrigem");
            porto.DiasAntesDoPodParaEnvioDaDocumentacao = Request.GetNullableIntParam("DiasAntesDoPodParaEnvioDaDocumentacao");
            porto.RKST = Request.GetNullableStringParam("RKST");
            porto.CriarSequenciaCargasMesmoComPedidoExistente = Request.GetBoolParam("CriarSequenciaCargasMesmoComPedidoExistente");
            porto.GerarUmaCargaSVMPorCargaMTLQuandoPortoDestino = Request.GetBoolParam("GerarUmaCargaSVMPorCargaMTLQuandoPortoDestino");
            porto.GerarUmaCargaSVMPorCargaMTLQuandoPortoOrigem = Request.GetBoolParam("GerarUmaCargaSVMPorCargaMTLQuandoPortoOrigem");

            porto.Integrado = false;
        }

        #endregion
    }
}
