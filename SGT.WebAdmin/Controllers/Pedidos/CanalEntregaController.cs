using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/CanalEntrega")]
    public class CanalEntregaController : BaseController
    {
		#region Construtores

		public CanalEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
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
                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = repCanalEntrega.BuscarPorCodigo(codigo);

                // Valida
                if (canalEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                // Formata retorno
                var retorno = new
                {
                    canalEntrega.Codigo,
                    canalEntrega.CodigoIntegracao,
                    canalEntrega.Descricao,
                    canalEntrega.NivelPrioridade,
                    canalEntrega.Ativo,
                    canalEntrega.Circuito,
                    canalEntrega.GerarCargaAutomaticamente,
                    canalEntrega.LiberarPedidoSemNFeAutomaticamente,
                    Observacao = canalEntrega.Observacao ?? string.Empty,
                    Filial = canalEntrega.Filial != null ? new { canalEntrega.Filial.Codigo, canalEntrega.Filial.Descricao } : null,
                    canalEntrega.Principal,
                    CanalEntregaPrincipal = new
                    {
                        Codigo = canalEntrega.CanalEntregaPrincipal?.Codigo ?? 0,
                        Descricao = canalEntrega.CanalEntregaPrincipal?.Descricao ?? string.Empty
                    },
                    QuantidadePedidosPermitidosNoCanal = canalEntrega.QuantidadePedidosPermitidosNoCanal ?? 0,
                    canalEntrega.NaoUtilizarCapacidadeVeiculoMontagemCarga
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = new Dominio.Entidades.Embarcador.Pedidos.CanalEntrega();

                // Preenche entidade com dados
                PreencheEntidade(ref canalEntrega, unitOfWork);

                if (!string.IsNullOrWhiteSpace(canalEntrega.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntregaExiste = repCanalEntrega.BuscarPorCodigoIntegracao(canalEntrega.CodigoIntegracao);
                    if (canalEntregaExiste != null)
                        return new JsonpResult(false, true, Localization.Resources.Pedidos.CanalEntrega.JaExisteCanalEntregaCadastradoCodigoIntegracao + canalEntrega.CodigoIntegracao + ".");
                }

                // Valida entidade
                string erro;
                if (!ValidaEntidade(canalEntrega, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repCanalEntrega.Inserir(canalEntrega, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
        
                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = repCanalEntrega.BuscarPorCodigo(codigo, true);

                // Valida
                if (canalEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);


                int.TryParse(Request.Params("CanalEntregaPrincipal"), out int codigoCanalEntregaPrincipal);
                bool.TryParse(Request.Params("Principal"), out bool principal);
                //Se for principal e estiver vindo como não principal nao podeoms deixar salvar se existir algum outro tipo relacionaod a esse tipo
                if (canalEntrega.Principal && !principal)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> canais = repCanalEntrega.BuscarPorCanalEntregaPrincipal(canalEntrega.Codigo);
                    if (canais?.Count > 0)
                    {
                        unitOfWork.Dispose();
                        return new JsonpResult(false, true, Localization.Resources.Pedidos.CanalEntrega.NaoPossivelAlterarCanalEntregaPrincipalParaNaoPrincipal);
                    }
                }

                // Preenche entidade com dados
                PreencheEntidade(ref canalEntrega, unitOfWork);
                if (!string.IsNullOrWhiteSpace(canalEntrega.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntregaExiste = repCanalEntrega.BuscarPorCodigoIntegracao(canalEntrega.CodigoIntegracao);
                    if (canalEntregaExiste != null && canalEntregaExiste.Codigo != canalEntrega.Codigo)
                        return new JsonpResult(false, true, Localization.Resources.Pedidos.CanalEntrega.JaExisteCanalEntregaCadastradoCodigoIntegracao + canalEntrega.CodigoIntegracao + ".");

                }
                // Valida entidade
                string erro;
                if (!ValidaEntidade(canalEntrega, out erro))
                    return new JsonpResult(false, true, erro);

                // Inicia transacao
                unitOfWork.Start();

                // Persiste dados
                repCanalEntrega.Atualizar(canalEntrega, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = repCanalEntrega.BuscarPorCodigo(codigo);

                // Valida
                if (canalEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                // Persiste dados
                repCanalEntrega.Deletar(canalEntrega, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRemover);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            // Instancia Repositorios
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilialCanalEntrega = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            string codigoIntegracao = Request.Params("CodigoIntegracao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            bool ativo = false;
            bool.TryParse(Request.Params("Ativo"), out ativo);

            bool gerarCargaAutomaticamente = false;
            bool.TryParse(Request.Params("GerarCargaAutomaticamente"), out gerarCargaAutomaticamente);

            bool liberarPedidoSemNFeAutomaticamente = false;
            bool.TryParse(Request.Params("LiberarPedidoSemNFeAutomaticamente"), out liberarPedidoSemNFeAutomaticamente);

            string observacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(observacao)) observacao = string.Empty;

            int nivelPrioridade = Request.GetIntParam("NivelPrioridade");
            int codigoFilial = Request.GetIntParam("Filial");
            
            int.TryParse(Request.Params("CanalEntregaPrincipal"), out int codigoCanalEntregaPrincipal);

            // Vincula dados
            canalEntrega.Descricao = descricao;
            canalEntrega.CodigoIntegracao = codigoIntegracao;
            canalEntrega.Ativo = ativo;
            canalEntrega.GerarCargaAutomaticamente = gerarCargaAutomaticamente;
            canalEntrega.LiberarPedidoSemNFeAutomaticamente = liberarPedidoSemNFeAutomaticamente;
            canalEntrega.Observacao = observacao;
            canalEntrega.NivelPrioridade = nivelPrioridade;
            canalEntrega.Filial = codigoFilial > 0 ? repFilialCanalEntrega.BuscarPorCodigo(codigoFilial) : null;
            canalEntrega.Principal = bool.Parse(Request.Params("Principal"));
            if (!canalEntrega.Principal)
                canalEntrega.CanalEntregaPrincipal = (codigoCanalEntregaPrincipal > 0 ? new Dominio.Entidades.Embarcador.Pedidos.CanalEntrega() { Codigo = codigoCanalEntregaPrincipal } : null);
            else
                canalEntrega.CanalEntregaPrincipal = null;

            canalEntrega.Circuito = Request.GetBoolParam("Circuito");
            canalEntrega.QuantidadePedidosPermitidosNoCanal = Request.GetIntParam("QuantidadePedidosPermitidosNoCanal");
            canalEntrega.NaoUtilizarCapacidadeVeiculoMontagemCarga = Request.GetBoolParam("NaoUtilizarCapacidadeVeiculoMontagemCarga");
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (string.IsNullOrWhiteSpace(canalEntrega.Descricao))
            {
                msgErro = Localization.Resources.Pedidos.CanalEntrega.DescricaoObrigatoria;
                return false;
            }

            if (canalEntrega.Descricao.Length > 200)
            {
                msgErro = Localization.Resources.Pedidos.CanalEntrega.DescricaoNaoPodePassar200Caracteres;
                return false;
            }


            if (canalEntrega.Observacao.Length > 2000)
            {
                msgErro = Localization.Resources.Pedidos.CanalEntrega.ObsNaoPodePassar2000Caracteres;
                return false;
            }
            
            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
            if (!string.IsNullOrWhiteSpace(Request.Params("Ativo")))
                Enum.TryParse(Request.Params("Ativo"), out ativo);
            else
                ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            string descricao = Request.Params("Descricao");

            string codigoIntegracao = Request.Params("CodigoIntegracao");

            int codigoFilial = 0;
            int.TryParse(Request.Params("Filial"), out codigoFilial);

            bool.TryParse(Request.Params("FiltrarCanaisEntregaPrincipal"), out bool filtrarCanaisEntregaPrincipal);

            // Consulta
            List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> listaGrid = repCanalEntrega.Consultar(codigoFilial, descricao, codigoIntegracao, filtrarCanaisEntregaPrincipal, ativo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCanalEntrega.ContarConsulta(codigoFilial, descricao, codigoIntegracao, filtrarCanaisEntregaPrincipal, ativo);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                            obj.CodigoIntegracao,
                            obj.NivelPrioridade,
                            DescricaoAtivo = obj.DescricaoAtivo,
                            DescricaoFilial = obj.Filial?.Descricao ?? "",
                            CanalEntregaPrincipal = obj.CanalEntregaPrincipal?.Descricao ?? ""
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.CodigoIntegracao, "CodigoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.CanalEntrega.NivelDePrioridade, "NivelPrioridade", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Ativo, "DescricaoAtivo", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.CanalEntrega.Filial, "DescricaoFilial", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.CanalEntrega.Principal, "CanalEntregaPrincipal", 15, Models.Grid.Align.left, false);

            return grid;
        }
        #endregion
    }
}
