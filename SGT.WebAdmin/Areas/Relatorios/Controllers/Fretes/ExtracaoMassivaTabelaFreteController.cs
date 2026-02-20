using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Excecoes.Embarcador;
using SGT.WebAdmin.Controllers;
using System.Linq.Dynamic.Core;
using System.Linq;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/ExtracaoMassivaTabelaFrete")]
    public class ExtracaoMassivaTabelaFreteController : BaseController
    {
		#region Construtores

		public ExtracaoMassivaTabelaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R335_ExtracaoMassivaTabelaFrete;
        private readonly decimal _tamanhoColunasValores = (decimal)1.75;
        private readonly decimal _tamanhoColunasTexto = 3;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Extração Massiva de Tabelas de Frete", "Fretes", "ExtracaoMassivaTabelaFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", 0, unitOfWork, true, true, 8);

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Tabela de Frete", "DescricaoTabelaFrete", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, true, true);
                grid.AdicionarCabecalho("Código Alteração (Origem)", "CodigoAlteracaoOrigem", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, true, false);
                grid.AdicionarCabecalho("Código Alteração (Atual)", "CodigoAlteracaoAtual", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, true, false);
                grid.AdicionarCabecalho("Situação Tabela (Atual)", "DescricaoSituacaoTabelaFrete", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, true, false);
                grid.AdicionarCabecalho("Data da Alteração", "DescricaoDataAlteracao", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Usuário Alteração", "UsuarioAlteracao", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Data Inicial da Vigência (Antes)", "DataInicialVigenciaAntes", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Data Inicial da Vigência (Depois)", "DataInicialVigenciaDepois", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Data Final da Vigência (Antes)", "DataFinalVigenciaAntes", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Data Final da Vigência (Depois)", "DataFinalVigenciaDepois", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Origem (Antes)", "DescricaoOrigemAntes", _tamanhoColunasTexto, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Origem (Depois)", "DescricaoOrigemDepois", _tamanhoColunasTexto, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Destino (Antes)", "DescricaoDestinoAntes", _tamanhoColunasTexto, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Destino (Depois)", "DescricaoDestinoDepois", _tamanhoColunasTexto, Models.Grid.Align.left, false, false, false, false, true);

                grid.AdicionarCabecalho("Tipo Parâmetro Base", "DescricaoTipoParametroBaseTabelaFrete", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Parâmetro Base", "DescricaoObjetoParametroBaseCalculo", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, true);

                grid.AdicionarCabecalho("Valor Mínimo Garantido do Parâmetro (Antes)", "DescricaoValorMinimoGarantidoParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Mínimo Garantido do Parâmetro (Depois)", "DescricaoValorMinimoGarantidoParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Entrega Excedente do Parâmetro (Antes)", "DescricaoValorEntregaExcedenteParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Entrega Excedente do Parâmetro (Depois)", "DescricaoValorEntregaExcedenteParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Pallet Excedente do Parâmetro (Antes)", "DescricaoValorPalletExcedenteParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Pallet Excedente do Parâmetro (Depois)", "DescricaoValorPalletExcedenteParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Quilometragem Excedente do Parâmetro (Antes)", "DescricaoValorQuilometragemExcedenteParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Quilometragem Excedente do Parâmetro (Depois)", "DescricaoValorQuilometragemExcedenteParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Peso Excedente do Parâmetro (Antes)", "DescricaoValorPesoExcedenteParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Peso Excedente do Parâmetro (Depois)", "DescricaoValorPesoExcedenteParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Ajudante Excedente do Parâmetro (Antes)", "DescricaoValorAjudanteExcedenteParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Ajudante Excedente do Parâmetro (Depois)", "DescricaoValorAjudanteExcedenteParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Máximo do Parâmetro (Antes)", "DescricaoValorMaximoParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Máximo do Parâmetro (Depois)", "DescricaoValorMaximoParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Percentual de Pagamento do Parâmetro (Antes)", "DescricaoPercentualPagamentoAgregadoParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Percentual de Pagamento do Parâmetro (Depois)", "DescricaoPercentualPagamentoAgregadoParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Base do Parâmetro (Antes)", "DescricaoValorBaseParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Base do Parâmetro (Depois)", "DescricaoValorBaseParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Hora Excedente do Parâmetro (Antes)", "DescricaoValorHoraExcedenteParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Hora Excedente do Parâmetro (Depois)", "DescricaoValorHoraExcedenteParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Pacote Excedente do Parâmetro (Antes)", "DescricaoValorPacoteExcedenteParametroBaseCalculoAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Pacote Excedente do Parâmetro (Depois)", "DescricaoValorPacoteExcedenteParametroBaseCalculoDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);

                grid.AdicionarCabecalho("Código da Tarifa", "CodigoObjetoItem", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Tarifa", "DescricaoObjetoItem", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Tipo de Valor da Tarifa", "DescricaoTipoValorItem", _tamanhoColunasTexto, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor da Tarifa (Antes)", "DescricaoValorItemAntes", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Valor da Tarifa (Depois)", "DescricaoValorItemDepois", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, true);

                grid.AdicionarCabecalho("Aprovador Tabela", "AprovadorTabelaFreteCliente", _tamanhoColunasTexto, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("AÃ§Ã£o", "TipoAcao", _tamanhoColunasTexto, Models.Grid.Align.left, false, false, false, false, false);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(grid, relatorio);

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
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Servicos.Embarcador.Frete.Consulta.ExtracaoMassivaTabelaFrete servicoExtracaoMassivaTabelaFrete = new Servicos.Embarcador.Frete.Consulta.ExtracaoMassivaTabelaFrete(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                int totalRegistros = propriedades.Count == 0 ? 0 : repositorioTabelaFreteCliente.ContarConsultaExtracaoMassiva(filtrosPesquisa, propriedades);

                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> listaConsultaTabelaFrete = totalRegistros > 0 ? repositorioTabelaFreteCliente.ConsultaExtracaoMassiva(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete>();
                servicoExtracaoMassivaTabelaFrete.CarregarDadosAnteriores(filtrosPesquisa, propriedades, listaConsultaTabelaFrete);

                grid.AdicionaRows(listaConsultaTabelaFrete);
                grid.setarQuantidadeTotal(totalRegistros);

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
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoRelatorio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string jsonRelatorio = Request.Params("Relatorio");
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(jsonRelatorio);

                Models.ServicoRelatorio svcRelatorio = new Models.ServicoRelatorio();

                dynRelatorio.Descricao = "Extração Massiva de Tabelas de Frete";
                dynRelatorio.NovoRelatorio = true;

                unidadeDeTrabalho.Start();

                svcRelatorio.SalvarConfiguracaoRelatorio(jsonRelatorio, this.Usuario, unidadeDeTrabalho, TipoServicoMultisoftware, ConfiguracaoEmbarcador.UsaPermissaoControladorRelatorios);


                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a configuração do relatório.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente()
            {
                DataInicialAlteracao = Request.GetNullableDateTimeParam("DataInicialAlteracao"),
                DataFinalAlteracao = Request.GetNullableDateTimeParam("DataFinalAlteracao"),
                CodigosTabelasFrete = Request.GetListParam<int>("TabelaFrete")
            };

            filtrosPesquisa.TabelasFreteClienteHistorico = ObterTabelasFreteClienteHistorico(filtrosPesquisa.CodigosTabelasFrete, unitOfWork);

            if (filtrosPesquisa.CodigosTabelasFrete.Count == 0)
                filtrosPesquisa.CodigosTabelasFrete.Add(0);


            return filtrosPesquisa;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Frete.TabelaFreteClienteHistorico> ObterTabelasFreteClienteHistorico(List<int> codigosTabelasFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Frete.TabelaFreteClienteHistorico> tabelasFreteClienteHistorico = new List<Dominio.ObjetosDeValor.Embarcador.Frete.TabelaFreteClienteHistorico>();

            foreach (int codigoTabelaFrete in codigosTabelasFrete)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.TabelaFreteClienteHistorico tabelaFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.TabelaFreteClienteHistorico();
                tabelaFrete.CodigoTabelaFrete = codigoTabelaFrete;
                tabelaFrete.CodigosTabelasFreteClienteHistorico = new Dictionary<int, List<int>>();

                List<int> codigosTabelasClienteOriginais = repositorioTabelaFreteCliente.BuscarCodigosTabelasClienteOriginaisPorTabelaFrete(codigoTabelaFrete);

                codigosTabelasClienteOriginais.ForEach(codigoTabelaClienteOriginal =>
                {
                    List<int> codigosTabelasFreteClienteHistorico = repositorioTabelaFreteCliente.BuscarCodigosTabelasClientePorTabelaFreteEOriginal(codigoTabelaFrete, codigoTabelaClienteOriginal);
                    tabelaFrete.CodigosTabelasFreteClienteHistorico.Add(codigoTabelaClienteOriginal, codigosTabelasFreteClienteHistorico);
                });

                tabelasFreteClienteHistorico.Add(tabelaFrete);
            }

            return tabelasFreteClienteHistorico;
        }


        #endregion
    }
}
