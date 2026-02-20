using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize(new string[] { "Pesquisa" }, "Veiculos/ModeloVeiculo")]
    public class ModeloVeiculoController : BaseController
    {
		#region Construtores

		public ModeloVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                int codigoMarca = 0;
                if (!string.IsNullOrWhiteSpace(Request.Params("Marca")) && int.Parse(Request.Params("Marca")) > 0)
                    codigoMarca = int.Parse(Request.Params("Marca"));

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("NumeroEixo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ModeloVeiculo.Marca, "DescricaoMarca", 40, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 10, Models.Grid.Align.center, false);

                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propriedadeOrdenar == "DescricaoMarca")
                    propriedadeOrdenar = "MarcaVeiculo.Descricao";

                Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
                List<Dominio.Entidades.ModeloVeiculo> listaModelo = repModeloVeiculo.Consulta(descricao, ativo, codigoMarca, codigoEmpresa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repModeloVeiculo.ContaConsulta(descricao, ativo, codigoMarca, codigoEmpresa));

                var lista = (from p in listaModelo
                             select new
                             {
                                 p.Codigo,
                                 NumeroEixo = p.MarcaVeiculo?.Codigo ?? 0,
                                 p.Descricao,
                                 DescricaoMarca = p.MarcaVeiculo?.Descricao ?? string.Empty,
                                 p.DescricaoStatus
                             }).ToList();

                grid.AdicionaRows(lista);
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

        #endregion
    }
}
