using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Transportadores
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Transportadores/Transportadores")]
    public class TransportadorController : BaseController
    {
        #region Construtores

        public TransportadorController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados Somente Leitura

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R066_Transportador;

        private decimal TamanhoColunasMedia = 3.50m;
        private decimal TamanhoColunasDescritivos = 5.50m;
        private decimal TamanhoColunasPequeno = 2;

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Transportadores", "Transportadores", "Transportadores.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(await GridPadrao(unitOfWork, cancellationToken), relatorio);

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

                List<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores> listaReport = null;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                SetarPropriedadeOrdenacao(ref propOrdena);

                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);

                int totalRegistros = await repEmpresa.ContaRegistrosAsync(filtrosPesquisa, unitOfWork);
                listaReport = await repEmpresa.ConsultarRelatorioAsync(filtrosPesquisa, propOrdena, grid.group.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.setarQuantidadeTotal(totalRegistros);
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
                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                string stringConexao = _conexao.StringConexao;

                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador filtrosPesquisa = ObterFiltrosPesquisa();

                _ = Task.Factory.StartNew(() => GerarRelatorioTransportadores(relatorioControleGeracao, relatorioTemp, stringConexao, filtrosPesquisa, CancellationToken.None));

                return new JsonpResult(true);
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

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioTransportadores(
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp,
            string stringConexao,
            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador filtrosPesquisa,
        CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.AtualizarAtual);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            try
            {
                List<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores> listaReport = null;

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                SetarPropriedadeOrdenacao(ref propOrdena);

                List<Parametro> parametros = await ObterParametrosRelatorioAsync(unitOfWork, filtrosPesquisa, cancellationToken);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);

                listaReport = await repEmpresa.ConsultarRelatorioAsync(filtrosPesquisa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Transportadores/Transportadores", parametros, relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork);
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

        private async Task<Models.Grid.Grid> GridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();

            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.RazaoSocial, "RazaoSocial", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.NomeFantasia, "NomeFantasia", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.CNPJ, "CNPJ", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.CodigoIntegracao, "CodigoIntegracao", TamanhoColunasMedia, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.InscricaoEstadual, "InscricaoEstadual", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.RNTRC, "RNTRC", TamanhoColunasMedia, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Cidade, "Cidade", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Estado, "Estado", TamanhoColunasPequeno, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Endereco, "Endereco", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Numero, "Numero", TamanhoColunasMedia, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Bairro, "Bairro", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Email, "Email", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Telefone, "Telefone", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.CEP, "CEP", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Situacao, "Situacao", TamanhoColunasMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.EmiteEmbarcador, "DescricaoEmiteEmbarcador", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.OptanteSimplesNacional, "OptanteSimplesNacional", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, false);

            if (configuracaoEmbarcador?.PermitirAutomatizarPagamentoTransportador ?? false)
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.IntegracaoAutomaticaCteGold, "DescricaoLiberacaoParaPagamentoAutomatico", TamanhoColunasMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.VencimentoCertificado, "DataVencimentoCertificado", TamanhoColunasMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.ConfiguracaoNfse, "DescricaoConfiguracaoNFSe", TamanhoColunasMedia, Models.Grid.Align.center, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Bloqueado, "Bloqueado", 10, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.MotivoBloqueio, "MotivoBloqueio", 10, Models.Grid.Align.left, false, false, false, false, false);
            }
            grid.AdicionarCabecalho("Serie CT-e Dentro", "SerieCTeDentro", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Serie CT-e Fora", "SerieCTeFora", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Serie MDF-e", "SerieMDFe", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.DataCriacao, "DataCadastro", TamanhoColunasMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.UsuarioCriacao, "UsuarioCadastro", TamanhoColunasMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.DataUltimaAlteracao, "DataAtualizacao", TamanhoColunasMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.UsuarioUltimaAlteracao, "UsuarioAtualizacao", TamanhoColunasMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.OperadoraValePedagio, "OperadoraValePedagio", TamanhoColunasMedia, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private void SetarPropriedadeOrdenacao(ref string propOrdena)
        {
            if (propOrdena == "RNTRC")
                propOrdena = "RegistroANTT";
            else if (propOrdena == "EmiteEmbarcador")
                propOrdena = "EmissaoDocumentosForaDoSistema";
            else if (propOrdena == "CertificadoVencido")
                propOrdena = "DataFinalCertificado";
            else if (propOrdena == "DataVencimentoCertificado")
                propOrdena = "DataFinalCertificado";
            else if (propOrdena == "Cidade")
                propOrdena = "Localidade.Descricao";
            else if (propOrdena == "Estado")
                propOrdena = "Localidade.Estado.Sigla";
            else if (propOrdena == "Situacao")
                propOrdena = "Status";
            else if (propOrdena == "DescricaoEmiteEmbarcador")
                propOrdena = "EmissaoDocumentosForaDoSistema";
        }

        private async Task<List<Parametro>> ObterParametrosRelatorioAsync(
            Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador filtrosPesquisa,
            CancellationToken cancellationToken)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork, cancellationToken);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork, cancellationToken);

            Dominio.Entidades.Localidade localidade = filtrosPesquisa.Localidade > 0 ? await repLocalidade.BuscarPorCodigoAsync(filtrosPesquisa.Localidade) : null;
            Dominio.Entidades.Estado estado = !string.IsNullOrWhiteSpace(filtrosPesquisa.Estado) ? await repEstado.BuscarPorSiglaAsync(filtrosPesquisa.Estado) : null;

            parametros.Add(new Parametro("Localidade", localidade?.DescricaoCidadeEstado));
            parametros.Add(new Parametro("Estado", estado?.Nome));
            parametros.Add(new Parametro("Situacao", filtrosPesquisa.Status.ObterDescricao()));
            parametros.Add(new Parametro("EmiteEmbarcador", filtrosPesquisa.EmiteEmbarcador.ObterDescricao()));
            parametros.Add(new Parametro("PrazoValidade", filtrosPesquisa.PrazoValidade));

            if (filtrosPesquisa.CertificadosVencidos)
                parametros.Add(new Parametro("CertificadosVencidos", filtrosPesquisa.CertificadosVencidos ? "Sim" : "Não", true));
            else
                parametros.Add(new Parametro("CertificadosVencidos", false));

            if (filtrosPesquisa.LiberacaoParaPagamentoAutomatico.HasValue)
                parametros.Add(new Parametro("LiberacaoParaPagamentoAutomatico", filtrosPesquisa.LiberacaoParaPagamentoAutomatico.Value ? "Sim" : "Não", true));
            else
                parametros.Add(new Parametro("LiberacaoParaPagamentoAutomatico", false));

            parametros.Add(new Parametro("OptanteSimplesNacional", filtrosPesquisa.OptanteSimplesNacional.ObterDescricao()));
            parametros.Add(new Parametro("ConfiguracaoNFSe", filtrosPesquisa.ConfiguracaoNFSe.ObterDescricao()));
            parametros.Add(new Parametro("Bloqueado", filtrosPesquisa.Bloqueado));
            parametros.Add(new Parametro("DataInicioVencimentoCertificado", filtrosPesquisa.DataInicioVencimentoCertificado));
            parametros.Add(new Parametro("DataFinalVencimentoCertificado", filtrosPesquisa.DataFinalVencimentoCertificado));

            parametros.Add(new Parametro("DataCriacaoInicial", filtrosPesquisa.DataCriacaoInicial?.ToString() ?? ""));
            parametros.Add(new Parametro("DataCriacaoFinal", filtrosPesquisa.DataCriacaoFinal?.ToString() ?? ""));
            parametros.Add(new Parametro("DataAlteracaoInicial", filtrosPesquisa.DataAlteracaoInicial?.ToString() ?? ""));
            parametros.Add(new Parametro("DataAlteracaoFinal", filtrosPesquisa.DataAlteracaoFinal?.ToString() ?? ""));
            return parametros;
        }

        private Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador()
            {
                Localidade = Request.GetIntParam("Localidade"),
                Estado = Request.GetStringParam("Estado").Replace("0", ""),
                CertificadosVencidos = Request.GetBoolParam("CertificadosVencidos"),
                LiberacaoParaPagamentoAutomatico = Request.GetNullableBoolParam("LiberacaoParaPagamentoAutomatico"),
                PrazoValidade = Request.GetDateTimeParam("PrazoValidade"),
                OptanteSimplesNacional = Request.GetEnumParam<OpcaoSimNaoPesquisa>("OptanteSimplesNacional"),
                EmiteEmbarcador = Request.GetEnumParam<OpcaoSimNaoPesquisa>("EmiteEmbarcador"),
                Status = Request.GetEnumParam<SituacaoAtivoPesquisa>("Status"),
                ConfiguracaoNFSe = Request.GetEnumParam<OpcaoSimNaoPesquisa>("ConfiguracaoNFSe"),
                DataInicioVencimentoCertificado = Request.GetDateTimeParam("DataInicioVencimentoCertificado"),
                DataFinalVencimentoCertificado = Request.GetDateTimeParam("DataFinalVencimentoCertificado"),
                DataCriacaoInicial = Request.GetNullableDateTimeParam("DataCriacaoInicial"),
                DataCriacaoFinal = Request.GetNullableDateTimeParam("DataCriacaoFinal"),
                DataAlteracaoInicial = Request.GetNullableDateTimeParam("DataAlteracaoInicial"),
                DataAlteracaoFinal = Request.GetNullableDateTimeParam("DataAlteracaoFinal"),
            };

            OpcaoSimNaoPesquisa bloqueado = Request.GetEnumParam<OpcaoSimNaoPesquisa>("Bloqueado");

            if (bloqueado == OpcaoSimNaoPesquisa.Sim)
                filtrosPesquisa.Bloqueado = true;
            else if (bloqueado == OpcaoSimNaoPesquisa.Nao)
                filtrosPesquisa.Bloqueado = false;

            return filtrosPesquisa;
        }

        #endregion
    }
}
