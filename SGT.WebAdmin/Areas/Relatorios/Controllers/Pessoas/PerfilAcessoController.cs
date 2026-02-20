using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Pessoas;
using Repositorio;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pessoas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pessoas/PerfilAcesso")]
    public class PerfilAcessoController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso>
    {
		#region Construtores

		public PerfilAcessoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Atributos Privados

		Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R112_PerfilAcesso;

        decimal TamanhoColunasMedia = 6;
        decimal TamanhoColunasDescritivos = 10;
        decimal TamanhoColunasPequeno = 4;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Perfil de Acesso", "Pessoas", "PerfilAcesso.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Nome", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Pessoas.PerfilAcesso servicoRelatorioPerfilAcesso = new Servicos.Embarcador.Relatorios.Pessoas.PerfilAcesso(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioPerfilAcesso.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso> listaPerfilAcesso, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(listaPerfilAcesso);
                grid.setarQuantidadeTotal(countRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso()
            {
                CodigoPerfil = Request.GetIntParam("PerfilAcesso"),
                Ativo = Request.GetNullableBoolParam("Ativo"),
                Cliente = Cliente?.Codigo ?? 0,
                ClienteAcesso = ClienteAcesso?.Codigo ?? 0,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                AdminStringConexao = _conexao.AdminStringConexao
            };

            return filtrosPesquisa;
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.Prop("Descricao").Nome("Descrição").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("CodigoIntegracao").Nome("Código Integração").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Modulo").Nome("Módulo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("Formulario").Nome("Formulário").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("PermissoesPersonalizadas").Nome("Permissão Personalizada").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TipoPermissao").Nome("Tipo de Permissão").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);

            return grid;
        }

        protected override Task<FiltroPesquisaRelatorioPerfilAcesso> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
