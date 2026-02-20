using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/ApuracaoICMS")]
    public class ApuracaoICMSController : BaseController
    {
		#region Construtores

		public ApuracaoICMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R213_ApuracaoICMS;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Apuração de ICMS", "CTe", "ApuracaoICMS.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

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
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.ConhecimentoDeTransporteEletronico repositorio = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork, cancellationToken);
                int totalRegistros = await repositorio.ContarConsultaRelatorioApuracaoICMSAsync(filtrosPesquisa, agrupamentos);
                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ApuracaoICMS> dataSource = (totalRegistros > 0) ? await repositorio.ConsultarRelatorioApuracaoICMSAsync(filtrosPesquisa, agrupamentos, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.CTe.ApuracaoICMS>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(dataSource);

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
                await unitOfWork.StartAsync(cancellationToken);

                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                _ = Task.Factory.StartNew(() => GerarRelatorio(agrupamentos, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorio(
            List<PropriedadeAgrupamento> agrupamentos,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario,
            string stringConexao,
            CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.ConhecimentoDeTransporteEletronico repositorio = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork, cancellationToken);
                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ApuracaoICMS> dataSource = await repositorio.ConsultarRelatorioApuracaoICMSAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);
                List<Parametro> parametros = await ObterParametros(filtrosPesquisa, unitOfWork, parametrosConsulta, cancellationToken);
                servicoRelatorio.GerarRelatorioDinamico("Relatorios/CTe/ApuracaoICMS", parametros, relatorioControleGeracao, relatorioTemporario, dataSource, unitOfWork);
                await unitOfWork.DisposeAsync();
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                CnpjCpfRemetente = Request.GetDoubleParam("Remetente"),
                CnpjCpfDestinatario = Request.GetDoubleParam("Destinatario"),
                CnpjCpfTomador = Request.GetDoubleParam("Tomador"),
                CnpjCpfRecebedor = Request.GetDoubleParam("Recebedor"),
                CnpjCpfExpedidor = Request.GetDoubleParam("Expedidor")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);

            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetenteFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("IE Remetente", "IERemetente", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("CPF/CNPJ Destinatário", "CPFCNPJDestinatarioFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("IE Destinatário", "IEDestinatario", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomadorFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("IE Tomador", "IETomador", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("CPF/CNPJ Expedidor", "CPFCNPJExpedidorFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("IE Expedidor", "IEExpedidor", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Expedidor", "Expedidor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Recebedor", "CPFCNPJRecebedorFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("IE Recebedor", "IERecebedor", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Nº CT-e", "NumeroCTe", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº Controle", "NumeroControle", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Alíquota ICMS Interna", "AliquotaICMSInterna", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("% ICMS Partilha", "PercentualICMSPartilha", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ICMS UF Origem", "ValorICMSUFOrigem", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ICMS UF Destino", "ValorICMSUFDestino", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ICMS FCP Fim", "ValorICMSFCPFim", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Característica Transporte", "CaracteristicaTransporteCTe", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Proposta", "TipoProposta", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private async Task<List<Parametro>> ObterParametros(
            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS filtrosPesquisa,
            Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta,
            CancellationToken cancellationToken)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);

            Dominio.Entidades.Cliente remetente = filtrosPesquisa.CnpjCpfRemetente > 0 ? await repCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.CnpjCpfRemetente) : null;
            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.CnpjCpfDestinatario > 0 ? await repCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.CnpjCpfDestinatario) : null;
            Dominio.Entidades.Cliente tomador = filtrosPesquisa.CnpjCpfTomador > 0 ? await repCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.CnpjCpfTomador) : null;
            Dominio.Entidades.Cliente recebedor = filtrosPesquisa.CnpjCpfRecebedor > 0 ? await repCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.CnpjCpfRecebedor) : null;
            Dominio.Entidades.Cliente expedidor = filtrosPesquisa.CnpjCpfExpedidor > 0 ? await repCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.CnpjCpfExpedidor) : null;

            parametros.Add(new Parametro("DataEmissao", filtrosPesquisa.DataEmissaoInicial, filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Parametro("Remetente", remetente?.Descricao));
            parametros.Add(new Parametro("Destinatario", destinatario?.Descricao));
            parametros.Add(new Parametro("Tomador", tomador?.Descricao));
            parametros.Add(new Parametro("Recebedor", recebedor?.Descricao));
            parametros.Add(new Parametro("Expedidor", expedidor?.Descricao));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataEmissaoFormatada")
                return "DataEmissao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
