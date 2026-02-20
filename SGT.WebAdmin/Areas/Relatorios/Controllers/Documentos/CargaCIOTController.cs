using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Documentos
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Documentos/CargaCIOT")]
    public class CargaCIOTController : BaseController
    {
        #region Construtores

        public CargaCIOTController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R135_CargaCIOT;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Carga CIOT", "Documentos", "CargaCIOT.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Numero", "asc", "ProprietarioFormatado", "asc", codigoRelatorio, unitOfWork, false, true);
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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCIOT filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeAgrupar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeAgrupar);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                int totalRegistros = repositorioCargaCIOT.ContarConsultaRelatorio(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOT> listaCargaCIOT = totalRegistros > 0 ? repositorioCargaCIOT.ConsultarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOT>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaCargaCIOT);

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
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCIOT filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeAgrupar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeAgrupar);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeOrdenar);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioAsync(filtrosPesquisa, agrupamentos, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

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

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCIOT filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOT> listaCargaCIOT = repositorioCargaCIOT.ConsultarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta);
                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = servicoRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemporario, listaCargaCIOT, unitOfWork, null, null, true, TipoServicoMultisoftware);
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = ObterParametros(filtrosPesquisa, unitOfWork);

                //servicoRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemporario, parametros);
                //servicoRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Documentos/CargaCIOT", unitOfWork);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Documentos/CargaCIOT", parametros, relatorioControleGeracao, relatorioTemporario, listaCargaCIOT, unitOfWork, null, null, true, TipoServicoMultisoftware);
            }
            catch (Exception ex)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCIOT ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCIOT()
            {
                Carga = Request.GetNullableStringParam("Carga"),
                DataEncerramentoInicial = Request.GetDateTimeParam("DataEncerramentoInicial"),
                DataEncerramentoFinal = Request.GetDateTimeParam("DataEncerramentoFinal"),
                Numero = Request.GetNullableStringParam("Numero"),
                Proprietario = Request.GetDoubleParam("Proprietario"),
                DataAberturaInicial = Request.GetDateTimeParam("DataAberturaInicial"),
                DataAberturaFinal = Request.GetDateTimeParam("DataAberturaFinal"),
                Veiculo = Request.GetIntParam("Veiculo"),
                Motorista = Request.GetIntParam("Motorista"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT>("Situacao"),
                Transportador = Request.GetIntParam("Transportador"),
                CodigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                RegimeTributario = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario>("RegimeTributario"),
            };
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("ProprietarioFormatado").Nome("Proprietário").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("EmpresaFormatado").Nome("Transportador").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("MotoristaFormatado").Nome("Motorista/Fretista").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("MotoristaDataNascimentoFormatada").Nome("Data Nascimento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("MotoristaPisPasep").Nome("PIS/PASEP").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("MotoristaCBO").Nome("CBO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("Carga").Nome("Carga").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("DataCargaFormatada").Nome("Data Carga").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DataPagamentoAdiantamentoFreteFormatada").Nome("Data Pagto. Adiantamento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DataPagamentoSaldoFreteFormatada").Nome("Data Pagto. Saldo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("PesoBruto").Nome("Peso Bruto").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorMercadoriaKG").Nome("Valor Mercadoria KG").Tamanho(TamanhoColunasPequeno).Visibilidade(false).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorTotalMercadoria").Nome("Valor Total Mercadoria").Tamanho(TamanhoColunasPequeno).Visibilidade(false).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorTarifaFrete").Nome("Tarifa Frete").Tamanho(TamanhoColunasPequeno).Visibilidade(false).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorFrete").Nome("Frete").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("PercentualTolerancia").Nome("Perc. Tolerância").Tamanho(TamanhoColunasPequeno).Visibilidade(false).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("PercentualToleranciaSuperior").Nome("Perc. Tolerância Superior").Visibilidade(false).Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorAdiantamento").Nome("Adiantamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorSeguro").Nome("Seguro").Tamanho(TamanhoColunasPequeno).Visibilidade(false).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorPedagio").Nome("Pedágio").Tamanho(TamanhoColunasPequeno).Visibilidade(false).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("BaseCalculoIRRF").Nome("Base IRRF").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);
            grid.Prop("ValorIRRF").Nome("IRRF").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("BaseCalculoINSS").Nome("Base INSS/SEST/SENAT").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);
            grid.Prop("ValorINSS").Nome("INSS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorSEST").Nome("SEST").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorSENAT").Nome("SENAT").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorOutrosDescontos").Nome("Outros Descontos").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("Saldo").Nome("Saldo").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("DescricaoSituacao").Nome("Situação").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("MensagemCIOT").Nome("Mensagem").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ProtocoloAutorizacao").Nome("Protocolo Autorização").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("VeiculoTracao").Nome("Véiculo Tração").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("VeiculosReboques").Nome("Véiculos Reboques").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("PedidosFormatado").Nome("Número do Pedido").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DescricaoRegimeTributario").Nome("Regime Tributário").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(true);

            return grid;
        }

        private List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCIOT filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            if (filtrosPesquisa.DataEncerramentoInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoInicial", filtrosPesquisa.DataEncerramentoInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoInicial", false));

            if (filtrosPesquisa.DataEncerramentoFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoFinal", filtrosPesquisa.DataEncerramentoFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoFinal", false));

            if (filtrosPesquisa.DataAberturaInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAberturaInicial", filtrosPesquisa.DataAberturaInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAberturaInicial", false));

            if (filtrosPesquisa.DataAberturaFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAberturaFinal", filtrosPesquisa.DataAberturaFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAberturaFinal", false));

            if (filtrosPesquisa.Proprietario > 0)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente _proprietario = repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Proprietario);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Proprietario", _proprietario.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Proprietario", false));

            if (filtrosPesquisa.Veiculo > 0)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo _veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", _veiculo.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

            if (filtrosPesquisa.Motorista > 0)
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario _motorista = repUsuario.BuscarPorCodigo(filtrosPesquisa.Motorista);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", _motorista.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Numero))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", filtrosPesquisa.Numero, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", filtrosPesquisa.Carga, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", false));

            if (filtrosPesquisa.Situacao != null)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOTHelper.ObterDescricao(filtrosPesquisa.Situacao), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

            if (filtrosPesquisa.RegimeTributario != null)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RegimeTributario", Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioHelper.ObterDescricao(filtrosPesquisa.RegimeTributario), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RegimeTributario", false));

            if (filtrosPesquisa.Transportador > 0)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(filtrosPesquisa.Transportador);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportador.RazaoSocial, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

            return parametros;
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Contains("Formatado"))
                return propriedadeOrdenar.Replace("Formatado", "");

            return propriedadeOrdenar;
        }

        #endregion
    }
}
