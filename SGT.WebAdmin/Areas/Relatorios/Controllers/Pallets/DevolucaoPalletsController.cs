using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Entidades.Embarcador.Relatorios;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pallets
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pallets/DevolucaoPallets")]
    public class DevolucaoPalletsController : BaseController
    {
		#region Construtores

		public DevolucaoPalletsController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R038_DevolucaoPallets;
        private readonly int _numeroMaximoSituacoes = 10;
        private readonly decimal _tamanhoColunaPequena = 1.75m;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Devolução de Pallets", "Pallets", "DevolucaoPallets.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes repConfiguracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = repConfiguracaoPaletes.BuscarConfiguracaoPadrao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                int codigoTransportador, codigoVeiculo, codigoMotorista, numeroNotaFiscal;
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("NumeroNotaFiscal"), out numeroNotaFiscal);
                List<int> codigosTransportador = Request.GetListParam<int>("Transportador");
                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                string numeroCarga = Request.Params("NumeroCarga");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    codigosTransportador = new List<int>() { Empresa.Codigo };

                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucaoPallets = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                //List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet repSituacaoDevolucaoPallet = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet> situacoesDevolucaoPallet = repSituacaoDevolucaoPallet.BuscarAtivos();
                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.Devolucao> listaDevolucao = repDevolucaoPallets.ConsultarRelatorio(0, codigosTransportador, codigoVeiculo, codigoMotorista, numeroNotaFiscal, numeroCarga, configuracaoPaletes.NaoExibirDevolucaoPaletesSemNotaFiscal, situacao, dataInicio, dataFim, situacoesDevolucaoPallet, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repDevolucaoPallets.ContarConsultaRelatorio(0, codigosTransportador, codigoVeiculo, codigoMotorista, numeroNotaFiscal, numeroCarga, configuracaoPaletes.NaoExibirDevolucaoPaletesSemNotaFiscal, situacao, dataInicio, dataFim));

                grid.AdicionaRows(listaDevolucao);

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
                int codigoVeiculo, codigoMotorista, numeroNotaFiscal;
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("NumeroNotaFiscal"), out numeroNotaFiscal);
                List<int> codigosTransportador = Request.GetListParam<int>("Transportador");
                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                string numeroCarga = Request.Params("NumeroCarga");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    codigosTransportador = new List<int>() { Empresa.Codigo };

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

                _ = Task.Factory.StartNew(() => GerarRelatorioDevolucaoPallets(agrupamentos, codigosTransportador, codigoVeiculo, codigoMotorista, numeroNotaFiscal, numeroCarga, situacao, dataInicio, dataFim, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet repSituacaoDevolucaoPallet = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet> situacoesDevolucaoPallet = repSituacaoDevolucaoPallet.BuscarAtivos();

            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("Cliente", "Cliente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("CPF/CNPJ Cliente", "ClienteCpfCnpj", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Código Integração Cliente", "ClienteCodigoIntegracao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            }
            else
            {
                grid.AdicionarCabecalho("Filial", "Filial", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("CNPJ Filial", "FilialCnpj", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Código Integração Filial", "FilialCodigoIntegracao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Nota Fiscal", "NumeroNotaFiscal", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("Emissão NF-e", "DataEmissaoNotaFiscal", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            }

            grid.AdicionarCabecalho("Qtd. Pallets", "NumeroPallets", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Qtd. Entregue", "NumeroPalletsEntregues", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Data Transporte", "DescricaoDataTransporte", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Data Devolução", "DescricaoDataDevolucao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("Empresa/Filial", "Transportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ Empresa/Filial", "TransportadorCnpj", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Código Integração Empresa/Filial", "TransportadorCodigoIntegracao", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            }
            else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.AdicionarCabecalho("Transportador", "Transportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ Transportador", "TransportadorCnpj", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Código Integração Transportador", "TransportadorCodigoIntegracao", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            }

            grid.AdicionarCabecalho("Motorista", "Motorista", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Veiculo", "Veiculo", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Destino", "Destino", _tamanhoColunaMedia, Models.Grid.Align.left,false, false, false, false, true);

            var ultimaColunaDinanica = 1;

            for (int i = 0; i < situacoesDevolucaoPallet.Count; i++)
            {
                if (i < _numeroMaximoSituacoes)
                {
                    grid.AdicionarCabecalho(situacoesDevolucaoPallet[i].Descricao, "SituacaoPallet" + ultimaColunaDinanica.ToString(), _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum, situacoesDevolucaoPallet[i].Codigo);

                    ultimaColunaDinanica++;
                }
                else
                    break;
            }
            grid.AdicionarCabecalho("Valor Total Cobrado", "ValorTotalCobrado", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);


            return grid;
        }

        private async Task GerarRelatorioDevolucaoPallets(List<PropriedadeAgrupamento> agrupamentos, List<int> codigosTransportador, int codigoVeiculo, int codigoMotorista, int numeroNotaFiscal, string numeroCarga, SituacaoDevolucaoPallet? situacao, DateTime dataInicio, DateTime dataFim, RelatorioControleGeracao relatorioControleGeracao, Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes repConfiguracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = repConfiguracaoPaletes.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet repSituacaoDevolucaoPallet = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet> situacoesDevolucaoPallet = repSituacaoDevolucaoPallet.BuscarAtivos();

                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.Devolucao> listaValorDescarga = repDevolucao.ConsultarRelatorio(0, codigosTransportador, codigoVeiculo, codigoMotorista, numeroNotaFiscal, numeroCarga, configuracaoPaletes.NaoExibirDevolucaoPaletesSemNotaFiscal, situacao, dataInicio, dataFim, situacoesDevolucaoPallet, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0);

                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (codigosTransportador?.Count() > 0)
                {
                    if (codigosTransportador.Count() == 1)
                    {
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigosTransportador.FirstOrDefault());

                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.RazaoSocial, true));
                    }
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", "Múltiplos Registros Selecionados", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                if (codigoMotorista > 0)
                {
                    Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

                if (codigoVeiculo > 0)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Placa, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

                if (numeroNotaFiscal > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNotaFiscal", numeroNotaFiscal.ToString("n0"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNotaFiscal", false));

                if (!string.IsNullOrWhiteSpace(numeroCarga))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCarga", numeroCarga, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCarga", false));

                if (situacao.HasValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", new Dominio.Relatorios.Embarcador.DataSource.Pallets.Devolucao() { Situacao = situacao.Value }.DescricaoSituacao, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/DevolucaoPallets",parametros,relatorioControleGeracao, relatorioTemp, listaValorDescarga, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        #endregion
    }
}
