using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "Pesquisa" }, "Pallets/FechamentoPallets")]
    public class FechamentoPalletsDevolucaoValePalletController : BaseController
    {
		#region Construtores

		public FechamentoPalletsDevolucaoValePalletController(Conexao conexao) : base(conexao) { }

		#endregion

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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pallets.DevolucaoValePallet repDevolucaoValePallet = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucao = repDevolucaoValePallet.BuscarPorCodigo(codigo);

                // Valida
                if (devolucao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                bool adicionar = Request.GetBoolParam("Adicionar");

                devolucao.AdicionarAoFechamento = adicionar;

                // Persiste dados
                unitOfWork.Start();

                repDevolucaoValePallet.Atualizar(devolucao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, devolucao, (adicionar ? "Adicionou ao" : "Removeu do") + " fechamento", unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
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

        #region Métodos Privados


        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("AdicionarAoFechamento", false);
            grid.Prop("Numero").Nome("Número").Tamanho(7).Align(Models.Grid.Align.right);
            grid.Prop("Data").Nome("Data").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("NFe").Nome("NF-e").Tamanho(7).Align(Models.Grid.Align.right);
            grid.Prop("Transportador").Nome("Transportador").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Motorista").Nome("Motorista").Tamanho(10).Align(Models.Grid.Align.left).Ord(false);
            grid.Prop("Cliente").Nome("Cliente").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Quantidade").Nome("Quantidade").Tamanho(7).Align(Models.Grid.Align.right);
            grid.Prop("Cidade").Nome("Cidade").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Representante").Nome("Representante").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Situacao").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pallets.DevolucaoValePallet repDevolucaoValePallet = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(unitOfWork);

            // Dados do filtro
            int fechamento = Request.GetIntParam("Codigo");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> listaGrid = repDevolucaoValePallet.ConsultaPorFechamento(fechamento, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repDevolucaoValePallet.ContarConsultaPorFechamento(fechamento);

            var lista = from obj in listaGrid
                        select new
                        {

                            obj.Codigo,
                            obj.AdicionarAoFechamento,
                            Data = obj.ValePallet.DataLancamento.ToString("dd/MM/yyyy"),
                            obj.ValePallet.Numero,
                            NFe = obj.ValePallet.Devolucao.XMLNotaFiscal?.Numero.ToString() ?? string.Empty,
                            Transportador = obj.ValePallet.Devolucao?.Transportador?.Descricao ?? string.Empty,
                            Motorista = obj.ValePallet.Devolucao.CargaPedido?.Carga?.NomeMotoristas ?? string.Empty,
                            Cliente = obj.ValePallet.Devolucao.XMLNotaFiscal?.Destinatario?.Descricao ?? string.Empty,
                            obj.ValePallet.Quantidade,
                            Cidade = obj.ValePallet.Devolucao.Filial?.Localidade?.Descricao ?? string.Empty,
                            Representante = obj.ValePallet.Representante?.Descricao ?? string.Empty,
                            Situacao = obj.ValePallet.Situacao.ObterDescricao(),
                            DT_RowColor = obj.AdicionarAoFechamento ? "" : "#949494",
                            DT_FontColor = obj.AdicionarAoFechamento ? "" : "#fff",
                        };

            return lista.ToList();
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Data")
                propOrdenar = "ValePallet.DataLancamento";
            else if (propOrdenar == "Numero")
                propOrdenar = "ValePallet.Numero";
            else if (propOrdenar == "NFe")
                propOrdenar = "ValePallet.Devolucao.XMLNotaFiscal.Numero";
            else if (propOrdenar == "Transportador")
                propOrdenar = "ValePallet.Devolucao.Transportador.RazaoSocial";
            else if (propOrdenar == "Cliente")
                propOrdenar = "ValePallet.Devolucao.XMLNotaFiscal.Destinatario.Nome";
            else if (propOrdenar == "Quantidade")
                propOrdenar = "ValePallet.Quantidade";
            else if (propOrdenar == "Cidade")
                propOrdenar = "ValePallet.Devolucao.Filial.Localidade.Descricao";
            else if (propOrdenar == "Representante")
                propOrdenar = "ValePallet.Representante.Descricao";
        }
        #endregion
    }
}
