using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Relatorios;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/ContratoFreteTransportador")]
    public class ContratoFreteTransportadorController : BaseController
    {
		#region Construtores

		public ContratoFreteTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R105_ContratoFreteTransportador;

        private Models.Grid.Grid GridPadrao()
        {
            decimal tamanhoColunasPequeno = (decimal)1.75;
            decimal tamanhoColunasGrande = (decimal)5.50;
            decimal tamanhoColunasMedio = (decimal)3.5;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);

            grid.AdicionarCabecalho("Número", "Numero", tamanhoColunasPequeno, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Vigência Inical", "VigenciaInicial", tamanhoColunasMedio, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Vigência Final", "VigenciaFinal", tamanhoColunasMedio, Models.Grid.Align.left, true, false, false, true, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Empresa/Filial", "Transportador", tamanhoColunasGrande, Models.Grid.Align.left, true, false, false, true, true);
            else
                grid.AdicionarCabecalho("Transportador", "Transportador", tamanhoColunasGrande, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("Descrição", "Descricao", tamanhoColunasGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo do Contrato de Frete", "TipoContratoFrete", tamanhoColunasGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo de Veículo", "TipoVeiculo", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Cavalo", "Cavalo", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Carreta", "Carreta", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Contrato Transportador", "ContratoTransportador", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("ID Externo", "IDExterno", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status Aceite Contrato", "StatusAceiteContrato", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo Fechamento", "DescricaoTipoFechamento", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Acordado", "ValorAcordado", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Quantidade Veiculo", "QuantidadeVeiculo", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Quantidade Aproximada Cargas Mensal", "QuantidadeAproxCargasMensal", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Mensal", "ValorMensal", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Canal Entrega", "CanalEntrega", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Ponto Planejamento Transporte", "DescricaoPontoPlanejamentoTransporte", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo Integracao", "DescricaoTipoIntegracao", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Carga", "DescricaoGrupoCarga", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tabelas Frete", "TabelasFrete", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Observação", "Observacao", tamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, true, false);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Contrato Frete Transportador", "Fretes", "RelatorioContratoFreteTransportador.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Numero", "desc", "", "", codigoRelatorio, unitOfWork, false, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

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

                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.TipoContratoFrete repTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork, cancellationToken);

                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.RelatorioContratoFreteTransportador> reportResult = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioContratoFreteTransportador filtrosPesquisa = ObterFiltrosPesquisa();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                #region Parametros
                if (parametros != null)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);

                    if (filtrosPesquisa.DiasParaVencimento > 0)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DiasParaVencimento", filtrosPesquisa.DiasParaVencimento.ToString() + " dias", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DiasParaVencimento", false));

                    if (filtrosPesquisa.TipoContratoFrete > 0)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoContratoFrete", repTipoContratoFrete.BuscarPorCodigo(filtrosPesquisa.TipoContratoFrete)?.Descricao ?? "", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoContratoFrete", false));

                    if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy"), true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

                    if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy"), true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));


                    if (filtrosPesquisa.CodigosTransportador.Count() == 1)
                    {
                        Dominio.Entidades.Empresa empresa = await repEmpresa.BuscarPorCodigoAsync(filtrosPesquisa.CodigosTransportador[0]);
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                    }
                    else if (filtrosPesquisa.CodigosTransportador.Count() > 1)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", "Múltiplos Registros Selecionados", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                    if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao.ToString("G"), true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

                    if (filtrosPesquisa.EmVigencia)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EmVigencia", "Sim", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EmVigencia", false));

                    if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEmbarcador))
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroEmbarcador", filtrosPesquisa.NumeroEmbarcador, true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroEmbarcador", false));
                }
                #endregion

                SetarPropriedadeOrdenacao(ref propOrdena);
                SetarPropriedadeOrdenacao(ref propAgrupa);

                SetarPropriedadeOrdenacao(ref propOrdena);
                SetarPropriedadeOrdenacao(ref propAgrupa);

                reportResult = await repContratoFreteTransportador.ConsultarRelatorioContratoFreteTransportadorAsync(filtrosPesquisa, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(reportResult.Count);
                grid.AdicionaRows(reportResult);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioAsync(agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private async Task GerarRelatorioAsync(
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao,
            CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Empresa empresaRelatorio = await repEmpresa.BuscarPorCodigoAsync(Empresa.Codigo);

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.RelatorioContratoFreteTransportador> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.TipoContratoFrete repTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioContratoFreteTransportador filtrosPesquisa = ObterFiltrosPesquisa();

                #region Parametros
                if (parametros != null)
                {
                    if (filtrosPesquisa.DiasParaVencimento > 0)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DiasParaVencimento", filtrosPesquisa.DiasParaVencimento.ToString() + " dias", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DiasParaVencimento", false));

                    if (filtrosPesquisa.TipoContratoFrete > 0)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoContratoFrete", repTipoContratoFrete.BuscarPorCodigo(filtrosPesquisa.TipoContratoFrete)?.Descricao ?? "", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoContratoFrete", false));

                    if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy"), true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

                    if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy"), true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));


                    if (filtrosPesquisa.CodigosTransportador.Count() == 1)
                    {
                        Dominio.Entidades.Empresa empresa = await repEmpresa.BuscarPorCodigoAsync(filtrosPesquisa.CodigosTransportador[0]);
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                    }
                    else if (filtrosPesquisa.CodigosTransportador.Count() > 1)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", "Múltiplos Registros Selecionados", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                    if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao.ToString("G"), true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

                    if (filtrosPesquisa.EmVigencia)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EmVigencia", "Sim", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EmVigencia", false));

                    if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEmbarcador))
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroEmbarcador", filtrosPesquisa.NumeroEmbarcador, true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroEmbarcador", false));
                }
                #endregion

                SetarPropriedadeOrdenacao(ref propOrdena);
                SetarPropriedadeOrdenacao(ref propAgrupa);

                listaReport = await repContratoFreteTransportador.ConsultarRelatorioContratoFreteTransportadorAsync(filtrosPesquisa, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
                //quantidade = repContratoFreteTransportador.ContarConsultaRelatorioContratoFreteTransportador(filtrosPesquisa, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena);


                ////ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                serRelatorio.GerarRelatorioDinamico( "Relatorios/Fretes/ContratoFreteTransportador", parametros,relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
            }
            catch (Exception ex)
            {
                await serRelatorio.RegistrarFalhaGeracaoRelatorioAsync(relatorioControleGeracao, unitOfWork, ex, cancellationToken);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private void SetarPropriedadeOrdenacao(ref string propOrdena)
        {
            if (propOrdena == "DescricaoSituacao")
                propOrdena = "Situacao";
            else if (propOrdena == "Transportador")
                propOrdena = "Empresa.EMP_RAZAO";
            else if (propOrdena == "Vigencia")
                propOrdena = "Contrato.CFT_DATA_INICIAL, Contrato.CFT_DATA_FINAL";
            else if (propOrdena == "Numero")
                propOrdena = "Contrato.CFT_NUMERO_EMBARCADOR, Contrato.CFT_NUMERO";
        }

        //private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Fretes.RelatorioContratoFreteTransportador> reportResult, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
        //    Repositorio.Embarcador.Frete.TipoContratoFrete repTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork);

        //    Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioContratoFreteTransportador filtrosPesquisa = ObterFiltrosPesquisa();

        //    #region Parametros
        //    if (parametros != null)
        //    {
        //        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

        //        if (filtrosPesquisa.DiasParaVencimento > 0)
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DiasParaVencimento", filtrosPesquisa.DiasParaVencimento.ToString() + " dias", true));
        //        else
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DiasParaVencimento", false));

        //        if (filtrosPesquisa.TipoContratoFrete > 0)
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoContratoFrete", repTipoContratoFrete.BuscarPorCodigo(filtrosPesquisa.TipoContratoFrete)?.Descricao ?? "", true));
        //        else
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoContratoFrete", false));

        //        if (filtrosPesquisa.DataInicial != DateTime.MinValue)
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy"), true));
        //        else
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

        //        if (filtrosPesquisa.DataFinal != DateTime.MinValue)
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy"), true));
        //        else
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));


        //        if (filtrosPesquisa.CodigosTransportador.Count() == 1)
        //        {
        //            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigosTransportador[0]);
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
        //        }
        //        else if (filtrosPesquisa.CodigosTransportador.Count() > 1)
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", "Múltiplos Registros Selecionados", true));
        //        else
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

        //        if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao.ToString("G"), true));
        //        else
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

        //        if (filtrosPesquisa.EmVigencia)
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EmVigencia", "Sim", true));
        //        else
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EmVigencia", false));

        //        if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEmbarcador))
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroEmbarcador", filtrosPesquisa.NumeroEmbarcador, true));
        //        else
        //            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroEmbarcador", false));
        //    }
        //    #endregion

        //    SetarPropriedadeOrdenacao(ref propOrdena);
        //    SetarPropriedadeOrdenacao(ref propAgrupa);

        //    reportResult = repContratoFreteTransportador.ConsultarRelatorioContratoFreteTransportador(filtrosPesquisa, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
        //    quantidade = repContratoFreteTransportador.ContarConsultaRelatorioContratoFreteTransportador(filtrosPesquisa, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena);
        //}



        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioContratoFreteTransportador ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioContratoFreteTransportador()
            {
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                TipoContratoFrete = Request.GetIntParam("TipoContratoFrete"),
                DiasParaVencimento = Request.GetIntParam("DiasParaVencimento"),
                EmVigencia = Request.GetBoolParam("EmVigencia"),
                NumeroEmbarcador = Request.Params("NumeroEmbarcador") ?? string.Empty,
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Situacao"),
                DataInicial = Request.GetDateTimeParam("DataInicio"),
                DataFinal = Request.GetDateTimeParam("DataFim"),
            };
        }
    }
}
