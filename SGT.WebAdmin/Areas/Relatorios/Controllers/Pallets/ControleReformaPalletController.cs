using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Entidades.Embarcador.Relatorios;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pallets
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pallets/ControleReformaPallet")]
    public class ControleReformaPalletController : BaseController
    {
		#region Construtores

		public ControleReformaPalletController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R138_ControleReformaPallet;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                var codigoRelatorio = Request.GetIntParam("Codigo");
                var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                var relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Controle de Reforma de Pallets", "Pallets", "ControleReformaPallet.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                var gridRelatorio = new Models.Grid.Relatorio();
                var dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                var filtrosPesquisa = ObterFiltrosPesquisa();
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var propriedadeAgrupar = grid.group.enable ? grid.group.propAgrupa : "";
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var listaReformaPallet = repositorio.ConsultarRelatorio(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repositorio.ContarConsultaRelatorio(filtrosPesquisa));

                grid.AdicionaRows((
                    from reforma in listaReformaPallet
                    select new
                    {
                        reforma.Codigo,
                        Data = reforma.Envio.Data.ToString("dd/MM/yyyy"),
                        DataRetorno = reforma.DataRetorno?.ToString("dd/MM/yyyy") ?? "",
                        Filial = reforma.Envio.Filial?.Descricao,
                        FilialCnpj = reforma.Envio.Filial?.CNPJ_Formatado,
                        FilialCodigoIntegracao = reforma.Envio.Filial?.CodigoFilialEmbarcador,
                        Fornecedor = reforma.Envio.Fornecedor.Nome,
                        FornecedorCpfCnpj = reforma.Envio.Fornecedor.CPF_CNPJ_Formatado,
                        FornecedorCodigoIntegracao = reforma.Envio.Fornecedor.CodigoIntegracao,
                        Transportador = reforma.Envio.Transportador?.Descricao,
                        TransportadorCnpj = reforma.Envio.Transportador?.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = reforma.Envio.Transportador?.CodigoIntegracao,
                        Nfe = String.Join(", ", (from nota in reforma.NotasFiscaisSaida select nota.XmlNotaFiscal.Numero)),
                        NfeValorTotal = (from nota in reforma.NotasFiscaisSaida select (decimal?)nota.XmlNotaFiscal.Valor).Sum() ?? 0,
                        NfeRetorno = String.Join(", ", (from nota in reforma.NotasFiscaisRetorno select nota.XmlNotaFiscal.Numero)),
                        NfeRetornoValorTotal = (from nota in reforma.NotasFiscaisRetorno select (decimal?)nota.XmlNotaFiscal.Valor).Sum() ?? 0,
                        Nfs = String.Join(", ", (from nota in reforma.NotasServicoRetorno select nota.Numero)),
                        NfsValorTotal = (from nota in reforma.NotasServicoRetorno select (decimal?)nota.ValorPrestacaoServico).Sum() ?? 0,
                        Quantidade = (from quantidadeReforma in reforma.Envio.QuantidadesReforma select quantidadeReforma.Quantidade).Sum(),
                        QuantidadeRetorno = (from nota in reforma.NotasFiscaisRetorno select (int?)nota.XmlNotaFiscal.QuantidadePallets).Sum() ?? 0,
                    }
                ).ToList());

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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var filtrosPesquisa = ObterFiltrosPesquisa();
                var repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                var gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                var dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                var relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                var relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                var relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                var mdlRelatorio = new Models.Grid.Relatorio();
                var grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemporario.PropriedadeOrdena);
                var stringConexao = _conexao.StringConexao;
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propriedadeOrdenar, relatorioTemporario.PropriedadeAgrupa);

                _ = Task.Factory.StartNew(() => GerarRelatorioControleReformaPallet(agrupamentos, filtrosPesquisa, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioControleReformaPallet(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleReformaPallet filtrosPesquisa, RelatorioControleGeracao relatorioControleGeracao, Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                var repositorioReformaPallet = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemporario.PropriedadeOrdena);
                var listaReformaPallet = repositorioReformaPallet.ConsultarRelatorio(filtrosPesquisa, propriedadeOrdenar, relatorioTemporario.OrdemOrdenacao, 0, 0);

                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleReformaPallet> dataSourceReformaPallet = (
                    from reforma in listaReformaPallet
                    select new Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleReformaPallet()
                    {
                        Codigo = reforma.Codigo,
                        Data = reforma.Envio.Data,
                        DataRetorno = reforma.DataRetorno?.ToString("dd/MM/yyyy") ?? "",
                        Filial = reforma.Envio.Filial?.Descricao,
                        FilialCnpj = reforma.Envio.Filial?.CNPJ_Formatado,
                        FilialCodigoIntegracao = reforma.Envio.Filial?.CodigoFilialEmbarcador,
                        Fornecedor = reforma.Envio.Fornecedor.Nome,
                        FornecedorCpfCnpj = reforma.Envio.Fornecedor.CPF_CNPJ_Formatado,
                        FornecedorCodigoIntegracao = reforma.Envio.Fornecedor.CodigoIntegracao,
                        Transportador = reforma.Envio.Transportador?.Descricao,
                        TransportadorCnpj = reforma.Envio.Transportador?.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = reforma.Envio.Transportador?.CodigoIntegracao,
                        Nfe = String.Join(", ", (from nota in reforma.NotasFiscaisSaida select nota.XmlNotaFiscal.Numero)),
                        NfeValorTotal = (from nota in reforma.NotasFiscaisSaida select (decimal?)nota.XmlNotaFiscal.Valor).Sum() ?? 0,
                        NfeRetorno = String.Join(", ", (from nota in reforma.NotasFiscaisRetorno select nota.XmlNotaFiscal.Numero)),
                        NfeRetornoValorTotal = (from nota in reforma.NotasFiscaisRetorno select (decimal?)nota.XmlNotaFiscal.Valor).Sum() ?? 0,
                        Nfs = String.Join(", ", (from nota in reforma.NotasServicoRetorno select nota.Numero)),
                        NfsValorTotal = (from nota in reforma.NotasServicoRetorno select (decimal?)nota.ValorPrestacaoServico).Sum() ?? 0,
                        Quantidade = (from quantidadeReforma in reforma.Envio.QuantidadesReforma select quantidadeReforma.Quantidade).Sum(),
                        QuantidadeRetorno = (from nota in reforma.NotasFiscaisRetorno select (int?)nota.XmlNotaFiscal.QuantidadePallets).Sum() ?? 0,
                    }
                ).ToList();

                var dataSourceNfsRetorno = new List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleReformaPalletNfs>();

                if (filtrosPesquisa.ExibirNfs)
                {
                    var listaNfsRetorno = (from reforma in listaReformaPallet from nfs in reforma.NotasServicoRetorno select nfs);

                    dataSourceNfsRetorno = (
                        from nfsRetorno in listaNfsRetorno
                        select new Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleReformaPalletNfs()
                        {
                            CodigoReforna = nfsRetorno.ReformaPallet.Codigo,
                            DataEmissao = nfsRetorno.DataEmissao?.ToString("dd/MM/yyyy"),
                            Numero = nfsRetorno.Numero,
                            ValorPrestacaoServico = nfsRetorno.ValorPrestacaoServico
                        }
                    ).ToList();
                }

                var dataSourceSubreport = new List<KeyValuePair<string, dynamic>>() { new KeyValuePair<string, dynamic>("ControleReformaPalletNFS.rpt", dataSourceNfsRetorno) };
                var parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);
                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/ControleReformaPallet",parametros,relatorioControleGeracao, relatorioTemporario, dataSourceReformaPallet, unitOfWork, null, dataSourceSubreport, true, TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleReformaPallet ObterFiltrosPesquisa()
        {
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleReformaPallet()
            {
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                DataRetornoInicio = Request.GetNullableDateTimeParam("DataRetornoInicio"),
                DataRetornoLimite = Request.GetNullableDateTimeParam("DataRetornoLimite"),
                ExibirNfs = Request.GetBoolParam("ExibirNfs"),
                ListaCodigoFilial = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Filial")),
                ListaCodigoTransportador = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Transportador")),
                ListaCpfCnpjFornecedor = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Fornecedor")),
                NumeroNfe = Request.GetIntParam("Nfe"),
                NumeroNfeRetorno = Request.GetIntParam("NfeRetorno"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("Empresa/Filial", "Transportador", 18, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("CNPJ Empresa/Filial", "TransportadorCnpj", 9, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Código integração Empresa/Filial", "TransportadorCodigoIntegracao", 9, Models.Grid.Align.center, false, false, false, false, false);
            }
            else
            {
                grid.AdicionarCabecalho("Filial", "Filial", 18, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("CNPJ Filial", "FilialCnpj", 9, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Código integração Filial", "FilialCodigoIntegracao", 9, Models.Grid.Align.center, false, false, false, false, false);
            }

            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 18, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("CPF/CNPJ Fornecedor", "FornecedorCpfCnpj", 9, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Código integração Fornecedor", "FornecedorCodigoIntegracao", 9, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("NF-e", "Nfe", 8, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("NF-e Valor Total", "NfeValorTotal", 7, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 7, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Data Retorno", "DataRetorno", 8, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("NF-e Retorno", "NfeRetorno", 7, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("NF-e Retorno Valor Total", "NfeRetornoValorTotal", 7, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("NFS", "Nfs", 8, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("NFS Valor Total", "NfsValorTotal", 7, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Quantidade Retorno", "QuantidadeRetorno", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);

            return grid;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleReformaPallet filtrosPesquisa)
        {
            var parametros = new List<Parametro>();

            if (filtrosPesquisa.DataInicio.HasValue || filtrosPesquisa.DataLimite.HasValue)
            {
                var periodo = $"{(filtrosPesquisa.DataInicio.HasValue ? $"{filtrosPesquisa.DataInicio.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataLimite.HasValue ? $"até {filtrosPesquisa.DataLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Parametro("Periodo", periodo, true));
            }
            else
                parametros.Add(new Parametro("Periodo", false));

            if (filtrosPesquisa.DataRetornoInicio.HasValue || filtrosPesquisa.DataRetornoLimite.HasValue)
            {
                var periodo = $"{(filtrosPesquisa.DataRetornoInicio.HasValue ? $"{filtrosPesquisa.DataRetornoInicio.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataRetornoLimite.HasValue ? $"até {filtrosPesquisa.DataRetornoLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Parametro("PeriodoRetorno", periodo, true));
            }
            else
                parametros.Add(new Parametro("PeriodoRetorno", false));

            if (filtrosPesquisa.ListaCodigoFilial?.Count > 0)
            {
                if (filtrosPesquisa.ListaCodigoFilial.Count == 1)
                {
                    var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                    var filial = repositorioFilial.BuscarPorCodigo(filtrosPesquisa.ListaCodigoFilial.First());

                    parametros.Add(new Parametro("Filial", filial.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Filial", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Filial", false));

            if (filtrosPesquisa.ListaCpfCnpjFornecedor?.Count > 0)
            {
                if (filtrosPesquisa.ListaCpfCnpjFornecedor.Count == 1)
                {
                    var repositorioCliente = new Repositorio.Cliente(unitOfWork);
                    var fornecedor = repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.ListaCpfCnpjFornecedor.First());

                    parametros.Add(new Parametro("Fornecedor", fornecedor.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Fornecedor", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Fornecedor", false));

            if (filtrosPesquisa.ListaCodigoTransportador?.Count > 0)
            {
                if (filtrosPesquisa.ListaCodigoTransportador.Count == 1)
                {
                    Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo(filtrosPesquisa.ListaCodigoTransportador.First());

                    parametros.Add(new Parametro("Transportador", transportador.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Transportador", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Transportador", false));

            if (filtrosPesquisa.NumeroNfe > 0)
                parametros.Add(new Parametro("Nfe", filtrosPesquisa.NumeroNfe.ToString(), true));
            else
                parametros.Add(new Parametro("Nfe", false));

            if (filtrosPesquisa.NumeroNfeRetorno > 0)
                parametros.Add(new Parametro("NfeRetorno", filtrosPesquisa.NumeroNfeRetorno.ToString(), true));
            else
                parametros.Add(new Parametro("NfeRetorno", false));

            if (filtrosPesquisa.ExibirNfs)
                parametros.Add(new Parametro("ExibirNfs", "Sim", visivel: true));
            else
                parametros.Add(new Parametro("ExibirNfs", visivel: false));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string nomePropriedadeOrdenar)
        {
            if (nomePropriedadeOrdenar == "Data")
                return "Envio.Data";

            if (nomePropriedadeOrdenar == "Filial")
                return "Envio.Filial.Descricao";

            if (nomePropriedadeOrdenar == "Transportador")
                return "Envio.Transportador.RazaoSocial";

            return nomePropriedadeOrdenar;
        }

        #endregion
    }
}
