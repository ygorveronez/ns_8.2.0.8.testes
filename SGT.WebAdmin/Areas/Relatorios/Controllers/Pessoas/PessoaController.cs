using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pessoas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pessoas/Pessoa")]
    public class PessoaController : BaseController
    {
		#region Construtores

		public PessoaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R136_Pessoa;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pessoas", "Pessoas", "Pessoa.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "RazaoSocial", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoBuscarOsDadosDoRelatario);
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

                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Pessoas.Pessoa servicoRelatorioPessoa = new Servicos.Embarcador.Relatorios.Pessoas.Pessoa(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioPessoa.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.Pessoa> listaPessoa, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaPessoa);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoConsultar);
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
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa filtrosPesquisa = ObterFiltrosPesquisa();

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
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            decimal TamanhoColunaPequena = 1.75m;
            decimal TamanhoColunaGrande = 5.50m;
            decimal TamanhoColunaMedia = 3m;

            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.CodigoIntegracao, "CodigoIntegracao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.CPFCNPJ, "CPFCNPJFormatado", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("IE", "IE", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.RazaoSocial, "RazaoSocial", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.NomeFantasia, "NomeFantasia", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Atividade, "Atividade", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.GrupoDePessoas, "GrupoPessoas", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TelefonePrincipal, "TelefonePrincipal", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TelefoneSecundario, "TelefoneSecundario", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.CEP, "CEP", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Endereco, "Endereco", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Bairro, "Bairro", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Cidade, "Cidade", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.UF, "UF", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Complemento, "Complemento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Numero, "Numero", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Email, "Email", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Observacao, "Observacao", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.PortadorConta, "PortadorConta", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Banco, "Banco", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Agencia, "Agencia", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Digito, "Digito", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.NumeroConta, "NumeroConta", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TipoConta, "TipoContaFormatada", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("PIS/PASEP", "PISPASEP", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.DataNascimento, "DataNascimentoFormatada", TamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Latitude, "LatitudeFormatada", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Longitude, "LongitudeFormatada", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.IndicadorIE, "IndicadorIEFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.CodigoDocumento, "CodigoDocumento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Bloqueado, "Bloqueado", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.DataBloqueio, "DataBloqueioFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.AguardandoConferenciaDeInformacao, "AguardandoConferenciaInformacao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TipoEnvioDaFatura, "TipoEnvioFaturaFormatado", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TipoPrazoFaturamento, "TipoPrazoFaturamentoFormatado", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.FormaGeracaoDosTitulosNaFatura, "FormaGeracaoTituloFaturaFormatado", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.DiasDePrazoFaturamento, "DiasPrazoFaturamento", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.DiasDaSemana, "DiaSemana", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.DiasDoMes, "DiaMes", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.ContaContabil, "ContaContabil", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.EstadoCivil, "EstadoCivilFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Sexo, "SexoFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TipoDoFornecedor, "TipoFornecedor", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.CodigoCategoriaTrabalhador, "CodigoCategoriaTrabalhador", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Funcao, "Funcao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.PagamentoEmBanco, "PagamentoEmBanco", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.FormaPagamentoESocial, "FormaPagamentoeSocial", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.BancoDOC, "BancoDOC", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TipoAutonomo, "TipoAutonomo", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.CodigoReceita, "CodigoReceita", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TipoPagamentoBancario, "TipoPagamentoBancario", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.NaoDescontaIRRF, "NaoDescontaIRRF", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Categoria, "Categoria", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.RegimeTributario, "RegimeTributarioFormatado", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("RNTRC", "RNTRC", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Raio, "Raio", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TipoArea, "TipoAreaDescricao", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.ExigeAgendamento, "ExigeAgendamentoDescricao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Modalidade, "Modalidade", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.FiliaisCliente, "Filiais", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TipoClienteIntegracao, "TipoClienteIntegracaoFormatado", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.DataIntegracao, "DataIntegracaoFormatada", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Pais, "Pais", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.TipoTerceiro, "TipoTerceiro", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Vendedor", "Vendedor", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, false);


            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa()
            {
                DataInicial = Request.GetDateTimeParam("DataInicio"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                TipoPessoa = Request.GetEnumParam<TipoPessoaCadastro>("TipoPessoa"),
                ModalidadePessoa = Request.GetListEnumParam<TipoModalidade>("ModalidadePessoa"),
                Estado = Request.GetListParam<string>("Estado"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoas"),
                CodigoLocalidade = Request.GetIntParam("Localidade"),
                CodigoAtividade = Request.GetIntParam("Atividade"),
                Situacao = Request.GetNullableBoolParam("Situacao"),
                SomenteSemCodigoIntegracao = Request.GetNullableBoolParam("SomenteSemCodigoIntegracao"),
                ExibeSomenteComCodigoIntegracao = Request.GetNullableBoolParam("ExibeSomenteComCodigoIntegracao"),
                Bloqueado = Request.GetEnumParam<OpcaoSimNaoPesquisa>("Bloqueado"),
                AguardandoConferenciaInformacao = Request.GetEnumParam<OpcaoSimNaoPesquisa>("AguardandoConferenciaInformacao"),
                ComGeolocalizacao = Request.GetEnumParam<OpcaoSimNaoPesquisa>("ComGeolocalizacao"),
                SomenteSemContaContabil = Request.GetNullableBoolParam("SomenteSemContaContabil"),
                CodigoCategoria = Request.GetIntParam("Categoria"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && Empresa.VisualizarSomenteClientesAssociados ? Empresa.Codigo : 0,
                Vendedor = Request.GetStringParam("Vendedor")
            };
        }

        #endregion
    }
}
