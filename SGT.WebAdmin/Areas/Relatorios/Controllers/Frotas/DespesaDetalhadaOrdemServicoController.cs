using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frotas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frotas/DespesaDetalhadaOrdemServico")]
    public class DespesaDetalhadaOrdemServicoController : BaseController
    {
		#region Construtores

		public DespesaDetalhadaOrdemServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R229_DespesaDetalhadaOrdemServico;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório Detalhado de Despesa por Ordem de Serviço", "Frotas", "DespesaDetalhadaOrdemServico.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Modelo", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
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

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frotas.DespesaDetalhadaOrdemServico servicoRelatorioDespesaDetalhada = new Servicos.Embarcador.Relatorios.Frotas.DespesaDetalhadaOrdemServico(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioDespesaDetalhada.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaDetalhadaOrdemServico> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);
                grid.setarQuantidadeTotal(totalRegistros);
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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorioAuditoriaDeOs(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtros = ObterFiltrosPesquisa();
                 
                IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico> listaReport = await Servicos.Embarcador.Frota.OrdemServicoManutencao.RetornarCabecalhoRelatorioAuditoriaOrdemDeOs(unitOfWork, filtros, cancellationToken);
                IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoServico> listaServicos = listaReport.Count > 0 ? await Servicos.Embarcador.Frota.OrdemServicoManutencao.RetornarServicosRelatorioAuditoriaOrdemDeOs(unitOfWork, listaReport.Select(o => o.Codigo).ToList(), cancellationToken) : new List<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoServico>();
                IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto> listaProdutos = listaServicos.Count > 0 ? await Servicos.Embarcador.Frota.OrdemServicoManutencao.RetornarProdutosRelatorioAuditoriaOrdemDeOs(unitOfWork, listaReport.Select(o => o.Codigo).ToList(), cancellationToken) : new List<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto>();

                if (listaReport.Count > 0)
                {
                    List<Parametro> listaParametros = ObterParametrosAuditoriaOs(filtros, unitOfWork);
                    byte[] pdf = Servicos.Embarcador.Frota.OrdemServicoManutencao.GerarRelatorioAuditoriaOrdemServico(listaReport, listaServicos, listaProdutos, listaParametros);

                    if (pdf == null)
                        throw new Exception();

                    return Arquivo(pdf, "application/pdf", "Relatório de Auditoria de Ordem de Serviço.pdf");
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro encontrado para gerar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                NumeroOS = Request.GetIntParam("NumeroOS"),
                MarcaVeiculo = Request.GetListParam<int>("MarcaVeiculo"),
                ModeloVeiculo = Request.GetListParam<int>("ModeloVeiculo"),
                Veiculo = Request.GetListParam<int>("Veiculo"),
                TipoOrdemServico = Request.GetListParam<long>("Tipo"),
                LocalManutencao = Request.GetListParam<double>("LocalManutencao"),
                Situacoes = Request.GetListEnumParam<SituacaoOrdemServicoFrota>("Situacao"),
                Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Usuario.Empresa.Codigo : 0,
                Servico = Request.GetListParam<int>("Servico"),
                Produto = Request.GetListParam<int>("Produto"),
                Equipamento = Request.GetListParam<int>("Equipamento"),
                Responsavel = Request.GetIntParam("Responsavel"),
                CodigoGrupoProduto = Request.GetIntParam("GrupoProduto"),
                CentroResultado = Request.GetListParam<int>("CentroResultado"),
                Mecanicos = Request.GetListParam<int>("Mecanicos")
            };
        }    

        private Models.Grid.Grid GridPadrao()
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Placa").Nome("Placa").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("Modelo").Nome("Modelo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Marca").Nome("Marca").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Ano").Nome("Ano do Veículo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("LocalManutencao").Nome("Local Manutenção").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(false, true).Visibilidade(false);
            grid.Prop("DataFormatada").Nome("Data").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("MesAnoOS").Nome("Mês e ano OS").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("Numero").Nome("Número OS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).OrdAgr(true, true).Visibilidade(true);
            grid.Prop("ValorProdutos").Nome("Vlr. Produtos Orçado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ValorServicos").Nome("Vlr. Serviços Orçado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ValorTotal").Nome("Vlr. Total Orçado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("Servico").Nome("Serviços").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Produto").Nome("Produtos").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ValorProdutosFechamento").Nome("Vlr. Produtos Realizado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorServicosFechamento").Nome("Vlr. Serviços Realizado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorTotalFechamento").Nome("Vlr. Total Realizado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("Tipo").Nome("Tipo").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("DescricaoSituacao").Nome("Situação").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).OrdAgr(false, true).Visibilidade(false);
            grid.Prop("Equipamento").Nome("Equipamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("Observacao").Nome("Observação").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ModeloVeicular").Nome("Modelo Veícular").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("KMExecucao").Nome("KM da Execução").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("HorimetroExecucao").Nome("Horímetro da Execução").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("KMUltimoAbastecimento").Nome("KM Último Abastecimento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("CodigoProduto").Nome("Cód. Produto").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("QuantidadeProduto").Nome("Qtd. Produto").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("NotaFiscal").Nome("Nº Nota Fiscal").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CentroResultado").Nome("Centro de Resultado").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Operador").Nome("Operador da O.S.").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ValorProduto").Nome("Valor Produto").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorServico").Nome("Valor Serviço").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("LocalArmazenamento").Nome("Local Armazenamento").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Responsavel").Nome("Responsável").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("ObservacaoServicos").Nome("Observações de Serviços").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(false, false).Visibilidade(false);
            grid.Prop("GrupoProduto").Nome("Grupo Produto").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(false, false).Visibilidade(false);
            grid.AdicionarCabecalho("Valor orçado em produtos", "ValorOrcadoEmProdutos", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor orçado em serviços", "ValorOrcadoEmServicos", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Mecânico", "Mecanicos", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo Previsto", "TempoPrevisto", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo Executado", "TempoExecutado", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);

            return grid;
        }

        private List<Parametro> ObterParametrosAuditoriaOs(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtros, Repositorio.UnitOfWork unitOfWork)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
            Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unitOfWork);
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo> tipoOrdemServico = filtros.TipoOrdemServico.Count > 0 ? repTipoOrdemServico.BuscarPorCodigo(filtros.TipoOrdemServico) : null;
            List<Dominio.Entidades.Veiculo> _veiculo = filtros.Veiculo.Count > 0 ? repVeiculo.BuscarPorCodigo(filtros.Veiculo) : null;
            List<Dominio.Entidades.ModeloVeiculo> _modeloVeiculo = filtros.ModeloVeiculo.Count > 0 ? repModeloVeiculo.BuscarPorCodigo(filtros.ModeloVeiculo) : null;
            List<Dominio.Entidades.MarcaVeiculo> _marcaVeiculo = filtros.MarcaVeiculo.Count > 0 ? repMarcaVeiculo.BuscarPorCodigo(filtros.MarcaVeiculo, 0) : null;
            List<Dominio.Entidades.Cliente> _localManutencao = filtros.LocalManutencao.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtros.LocalManutencao) : null;
            Dominio.Entidades.Empresa empresa = filtros.Empresa > 0 ? repEmpresa.BuscarPorCodigo(filtros.Empresa) : null;
            List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> _servicoVeiculo = filtros.Servico.Count > 0 ? repServicoVeiculoFrota.BuscarPorCodigo(filtros.Servico) : null;
            List<Dominio.Entidades.Produto> _produto = filtros.Produto.Count > 0 ? repProduto.BuscarPorCodigo(filtros.Produto.ToArray()) : null;
            List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> _equipamento = filtros.Equipamento.Count > 0 ? repEquipamento.BuscarPorCodigo(filtros.Equipamento) : null;
            Dominio.Entidades.Usuario _usuario = filtros.Responsavel > 0 ? repUsuario.BuscarPorCodigo(filtros.Responsavel) : null;

            string data = "";
            data += filtros.DataInicial != DateTime.MinValue ? filtros.DataInicial.ToString("dd/MM/yyyy") + " " : "";
            data += filtros.DataFinal != DateTime.MinValue ? "até " + filtros.DataFinal.ToString("dd/MM/yyyy") : "";

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", filtros.NumeroOS > 0 ? filtros.NumeroOS.ToString() : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", _veiculo != null && _veiculo.Count > 0 ? string.Join(", ", _veiculo.Select(o => o.Placa).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", _modeloVeiculo != null && _modeloVeiculo.Count > 0 ? string.Join(", ", _modeloVeiculo.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MarcaVeiculo", _marcaVeiculo != null && _marcaVeiculo.Count > 0 ? string.Join(", ", _marcaVeiculo.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalManutencao", _localManutencao != null && _localManutencao.Count > 0 ? string.Join(", ", _localManutencao.Select(o => o.Nome).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", "Codigo"));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.RazaoSocial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", tipoOrdemServico != null && tipoOrdemServico.Count > 0 ? string.Join(", ", tipoOrdemServico.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", string.Join(", ", filtros.Situacoes?.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Servico", _servicoVeiculo != null && _servicoVeiculo.Count > 0 ? string.Join(", ", _servicoVeiculo.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", _produto != null && _produto.Count > 0 ? string.Join(", ", _produto.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Equipamento", _equipamento != null && _equipamento.Count > 0 ? string.Join(", ", _equipamento.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Responsavel", _usuario?.Nome));

            return parametros;
        }

        #endregion
    }
}
