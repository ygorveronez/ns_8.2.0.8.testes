using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/PagamentoAgregado")]
    public class PagamentoAgregadoController : BaseController
    {
		#region Construtores

		public PagamentoAgregadoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R144_PagamentoAgregado;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Numero").Nome("Número").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("DescricaoDataPagamento").Nome("Data Pagamento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).OrdAgr(true, true);
            grid.Prop("Observacao").Nome("Observação").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DescricaoSituacao").Nome("Situação").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DescricaoStatus").Nome("Status").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Valor").Nome("Valor Pagamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("DescricaoDataInicialEmissao").Nome("Emissão Inicial").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DescricaoDataFinalEmissao").Nome("Emissão Final").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DescricaoDataInicialOcorrencia").Nome("Ocorrência Inicial").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DescricaoDataFinalOcorrencia").Nome("Ocorrência Final").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("NomeProprietario").Nome("Agregado").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("CNPJProprietario").Nome("CNPJ/CPF").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ValorPagamentoDocumento").Nome("Valor Pag. Doc.").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("NumeroDocumento").Nome("Nº Doc.").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("SerieDocumento").Nome("Série Doc.").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("ValorReceberDocumento").Nome("Val. Receb. Doc").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("Placas").Nome("Placas").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Motoristas").Nome("Motoristas").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Cargas").Nome("Cargas").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("TipoOperacao").Nome("Tipo da Operação").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Peso").Nome("Peso NFs").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("CodigoTitulo").Nome("Cód. Título").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DescricaoVencimentoTitulo").Nome("Vencimento Tit.").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DescricaoStatusTitulo").Nome("Status Tit.").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ValorPendenteTitulo").Nome("Val. Pend. Tit.").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pagamento de Agregado", "Financeiros", "PagamentoAgregado.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Numero", "desc", "", "", codigoRelatorio, unitOfWork, false, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

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

                List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PagamentoAgregado> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);


                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(listaReport);

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
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                GerarRelatorioAsync(agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, cancellationToken);

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

        private async Task GerarRelatorioAsync(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PagamentoAgregado> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                serRelatorio.GerarRelatorioDinamico( "Relatorios/Financeiros/PagamentoAgregado", parametros,relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private void SetarPropriedadeOrdenacao(ref string propOrdena)
        {
            if (propOrdena == "DescricaoVencimentoTitulo") propOrdena = "VencimentoTitulo";
            else if (propOrdena == "DescricaoDataFinalOcorrencia") propOrdena = "DataFinalOcorrencia";
            else if (propOrdena == "DescricaoDataInicialOcorrencia") propOrdena = "DataInicialOcorrencia";
            else if (propOrdena == "DescricaoDataFinalEmissao") propOrdena = "DataFinalEmissao";
            else if (propOrdena == "DescricaoDataInicialEmissao") propOrdena = "DataInicialEmissao";
            else if (propOrdena == "DescricaoDataPagamento") propOrdena = "DataPagamento";
            else if (propOrdena == "DescricaoSituacao") propOrdena = "Situacao";
            else if (propOrdena == "DescricaoStatusTitulo") propOrdena = "StatusTitulo";
            else if (propOrdena == "DescricaoStatus") propOrdena = "Status";
        }

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PagamentoAgregado> reportResult, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);

            int.TryParse(Request.Params("NumeroInicial"), out int numeroInicial);
            int.TryParse(Request.Params("NumeroFinal"), out int numeroFinal);
            DateTime.TryParse(Request.Params("DataPagamentoInicial"), out DateTime dataPagamentoInicial);
            DateTime.TryParse(Request.Params("DataPagamentoFinal"), out DateTime dataPagamentoFinal);

            int.TryParse(Request.Params("Motorista"), out int motorista);
            int.TryParse(Request.Params("Veiculo"), out int veiculo);
            int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
            int.TryParse(Request.Params("NumeroCTe"), out int numeroCTe);

            string numeroCarga = Request.Params("NumeroCarga");

            double.TryParse(Request.Params("Agregado"), out double agregado);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado situacaoAux))
                situacao = situacaoAux;

            #region Parametros
            if (parametros != null)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                Dominio.Relatorios.Embarcador.DataSource.Financeiros.PagamentoAgregado aux = new Dominio.Relatorios.Embarcador.DataSource.Financeiros.PagamentoAgregado();

                if (numeroInicial > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInicial", numeroInicial.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInicial", false));

                if (numeroFinal > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFinal", numeroFinal.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFinal", false));


                if (dataPagamentoInicial != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamentoInicial", dataPagamentoInicial.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamentoInicial", false));

                if (dataPagamentoFinal != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamentoFinal", dataPagamentoFinal.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamentoFinal", false));

                if (situacao.HasValue)
                {
                    aux.Situacao = situacao.Value;
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", aux.DescricaoSituacao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

                if (agregado > 0)
                {
                    Dominio.Entidades.Cliente _fornecedor = repCliente.BuscarPorCPFCNPJ(agregado);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agregado", _fornecedor.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agregado", false));

                if (veiculo > 0)
                {
                    Dominio.Entidades.Veiculo _veiculo = repVeiculo.BuscarPorCodigo(veiculo);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", _veiculo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

                if (motorista > 0)
                {
                    Dominio.Entidades.Usuario _operador = repUsuario.BuscarPorCodigo(motorista);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", _operador.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

                if (numeroCTe > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCTe", numeroCTe.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCTe", false));

                if (!string.IsNullOrWhiteSpace(numeroCarga))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCarga", numeroCarga, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCarga", false));

                if (tipoOperacao > 0)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao _tipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", _tipoOperacao.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", false));


            }
            #endregion

            SetarPropriedadeOrdenacao(ref propOrdena);
            // TODO: ToList cast
            reportResult = repPagamentoAgregado.ConsultarRelatorioPagamentoAgregado(numeroInicial, numeroFinal, dataPagamentoInicial, dataPagamentoFinal, motorista, veiculo, tipoOperacao, numeroCTe, numeroCarga, agregado, situacao, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite).ToList();
            quantidade = repPagamentoAgregado.ContarConsultaRelatorioPagamentoAgregado(numeroInicial, numeroFinal, dataPagamentoInicial, dataPagamentoFinal, motorista, veiculo, tipoOperacao, numeroCTe, numeroCarga, agregado, situacao, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena);
        }
    }
}
