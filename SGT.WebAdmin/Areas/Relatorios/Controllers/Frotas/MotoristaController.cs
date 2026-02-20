using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Frotas;
using Repositorio;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frotas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frotas/Motorista")]
    public class MotoristaController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista>
    {
		#region Construtores

		public MotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Atributos Privados

		Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R062_Motorista;
        private int UltimaColunaDinanica = 1;
        private int NumeroMaximoEPIs = 20;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, Localization.Resources.Relatorios.Frotas.Motorista.RelatorioMotoristas, "Frotas", "Motorista.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Nome", "desc", "", "", codigoRelatorio, unitOfWork, false, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(await GridPadraoAsync(unitOfWork, cancellationToken), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.Frotas.Motorista.OcorreuUmaFalhaAoBuscarDadosRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frotas.Motorista servicoRelatorioMotorista = new Servicos.Embarcador.Relatorios.Frotas.Motorista(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioMotorista.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Frota.Motorista> listaMotorista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaMotorista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.Frotas.Motorista.OcorreuUmaFalhaAoGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista()
            {
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : Request.GetIntParam("Transportador"),
                CodigoIntegracao = Request.GetStringParam("Codigo"),
                CodigoTipoLicenca = Request.GetIntParam("Licenca"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                Nome = Request.GetStringParam("Nome"),
                CPF = Request.GetStringParam("CPF").ObterSomenteNumeros(),
                TipoMotorista = Request.GetEnumParam<TipoMotorista>("TipoMotorista"),
                SituacaoColaborador = Request.GetEnumParam<SituacaoColaborador>("SituacaoColaborador"),
                Status = Request.GetEnumParam<SituacaoAtivoPesquisa>("Status"),
                Aposentadoria = Request.GetEnumParam<Aposentadoria>("Aposentadoria"),
                CargoMotorista = Request.GetIntParam("CargoMotorista"),
                UsuarioMobile = Request.GetNullableBoolParam("UsuarioMobile"),
                NaoBloquearAcessoSimultaneo = Request.GetNullableBoolParam("NaoBloquearAcessoSimultaneo"),
                CodigoGestor = Request.GetIntParam("Gestor")
            };

            OpcaoSimNaoPesquisa bloqueado = Request.GetEnumParam<OpcaoSimNaoPesquisa>("Bloqueado");

            if (bloqueado == OpcaoSimNaoPesquisa.Sim)
                filtrosPesquisa.Bloqueado = true;
            else if (bloqueado == OpcaoSimNaoPesquisa.Nao)
                filtrosPesquisa.Bloqueado = false;

            return filtrosPesquisa;
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Privados

        private async Task<Models.Grid.Grid> GridPadraoAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pessoas.EPI repEPI = new Repositorio.Embarcador.Pessoas.EPI(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.EPI> epis = await repEPI.BuscarTodosAtivos();

            bool disponivelApenasTMS = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
            bool disponivelApenasEmbarcador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 8, Models.Grid.Align.right, true, disponivelApenasTMS);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.CodigoIntegracao, "CodigoIntegracao", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.CPF , "CPF", 12, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.RG, "RG", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.DataNascimento, "DataNascimentoFormatada", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.CNH, "CNH", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.ValidadeCNH, "DataValidadeCNHFormatada", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.ValidadeSeguradora, "DataValidadeSeguradoraFormatada", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Telefone, "Telefone", 8, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Endereco, "Endereco", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.CEP, "CEP", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Cidade, "Cidade", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Estado, "Estado", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.TipoMotorista, "TipoMotorista", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.DataEmissaoCNH, "DataEmissaoCNHFormatada", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.NumeroProntuario, "NumeroProntuario", 8, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.FichaAtiva, "FichaAtiva", 8, Models.Grid.Align.center, true, false, false, true, disponivelApenasTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.GerarComissao, "GerarComissao", 8, Models.Grid.Align.center, true, false, false, true, disponivelApenasTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Banco, "Banco", 5, Models.Grid.Align.left, false, disponivelApenasTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Agencia, "Agencia", 8, Models.Grid.Align.left, false, disponivelApenasTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Digito, "Digito", 4, Models.Grid.Align.left, false, disponivelApenasTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.NumeroConta, "NumeroConta", 8, Models.Grid.Align.left, false, disponivelApenasTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.TipoConta, "TipoContaFormatada", 8, Models.Grid.Align.center, false, disponivelApenasTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.NumeroCartao, "NumeroCartao", 8, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Filial, "Filial", 15, Models.Grid.Align.left, false, disponivelApenasEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Transportadora, "Transportadora", 15, Models.Grid.Align.left, false, disponivelApenasEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Veiculo, "Veiculo", 8, Models.Grid.Align.left, false, disponivelApenasEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.SaldoDiaria, "SaldoDiaria", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.SaldoAdiantamento, "SaldoAdiantamento", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.DataEmissaoRG, "DataEmissaoRGFormatada", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.EmissorRG, "EmissorRGDescricao", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.CategoriaCNH, "CategoriaCNH", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Celular, "Celular", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("PIS-PASEP", "PISPASEP", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.DataAdmissao, "DataAdmissaoFormatada", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Renach", "RenachCNH", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.DiasTrabalhado, "DiasTrabalhado", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.DiasFolgaRetirada, "DiasFolgaRetirado", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.CentroResultado, "CentroResultado", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Cargo, "Cargo", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Frota, "NumeroFrota", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.UtilizaAplicativo, "UsuarioMobile", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.MudarAparelhoSemBloquear, "NaoBloquearAcessoSimultaneo", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Aposentado, "AposentadoriaFormatada", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.CargoMotorista, "CargoMotorista", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ do Transportador", "CNPJTransportadorFormatado", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Estado RG", "EstadoRG", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Registro CNH", "NumRegistroCNH", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Primeira CNH", "DataPrimeiraCNHFormatada", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Email", "Email", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Bairro", "Bairro", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Logradouro", "TipoLogradouroFormatada", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Estado Civil", "EstadoCivilFormatada", 10, Models.Grid.Align.left, false, false, false, false, false);
            //grid.AdicionarCabecalho("Cor/Raça", "CorRacaFormatada", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Escolaridade", "EscolaridadeFormatada", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Título Eleitoral", "TituloEleitoral", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Zona Eleitoral", "ZonaEleitoral", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Seção Eleitoral", "SecaoEleitoral", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Expedição CTPS", "DataExpedicaoCTPSFormatada", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Estado CTPS", "EstadoExpedicaoCTPS", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Localidade Nascimento", "LocalidadeNascimento", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grau Parentesco", "TipoParentescoFormatada", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Pessoa Agregado", "PessoaAgregado", 10, Models.Grid.Align.left, false, false, false, false, false); //Localization.Resources.Relatorios.Frotas.Motorista.PessoaAgregado //Colocar localization e apagar string, naõ consegui add, resxmanager não cria/salva, já tentado instalar e desinstalar;
            grid.AdicionarCabecalho("Complemento Endereço", "ComplementoEndereco", 10, Models.Grid.Align.left, false, false, false, false, false); //Localization.Resources.Relatorios.Frotas.Motorista.ComplementoEndereco
            grid.AdicionarCabecalho("Número CTPS", "NumeroCTPS", 10, Models.Grid.Align.left, false, false, false, false, false); //Localization.Resources.Relatorios.Frotas.Motorista.NumeroCTPS
            grid.AdicionarCabecalho("Serie CTPS", "SerieCTPS", 10, Models.Grid.Align.left, false, false, false, false, false); //Localization.Resources.Relatorios.Frotas.Motorista.SerieCTPS
            grid.AdicionarCabecalho("Dados Bancários", "DadosBancarios", 10, Models.Grid.Align.left, false, false, false, false, false); //Localization.Resources.Relatorios.Frotas.Motorista.DadosBancarios
            grid.AdicionarCabecalho("Contato Nenhum", "ContatoNenhum", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoNenhum
            grid.AdicionarCabecalho("Contato Outro", "ContatoOutro", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoOutro
            grid.AdicionarCabecalho("Contato Pai", "ContatoPai", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoPai
            grid.AdicionarCabecalho("Contato Mãe", "ContatoMae", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoMae
            grid.AdicionarCabecalho("Contato Filhos", "ContatoFilhos", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoFilhos
            grid.AdicionarCabecalho("Contato Irmao", "ContatoIrmao", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoIrmao
            grid.AdicionarCabecalho("Contato Avó", "ContatoAvo", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoAvo
            grid.AdicionarCabecalho("Contato Neto", "ContatoNeto", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoNeto
            grid.AdicionarCabecalho("Contato Tio", "ContatoTio", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoTio
            grid.AdicionarCabecalho("Contato Sobrinho", "ContatoSobrinho", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoSobrinho
            grid.AdicionarCabecalho("Contato Bisavo", "ContatoBisavo", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoBisavo
            grid.AdicionarCabecalho("Contato Bisneto", "ContatoBisneto", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoBisneto
            grid.AdicionarCabecalho("Contato Primo", "ContatoPrimo", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoPrimo
            grid.AdicionarCabecalho("Contato Trisavo", "ContatoTrisavo", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoTrisavo
            grid.AdicionarCabecalho("Contato Trineto", "ContatoTrineto", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoTrineto
            grid.AdicionarCabecalho("Contato TipoAvo", "ContatoTipoAvo", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoTipoAvo
            grid.AdicionarCabecalho("Contato SobrinhoNeto", "ContatoSobrinhoNeto", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoSobrinhoNeto
            grid.AdicionarCabecalho("Contato Esposo", "ContatoEsposo", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.ContatoEsposo         
            grid.AdicionarCabecalho("Munícipio/UF CNH", "LocalidadeMunicipioEstadoCNH", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.LocalidadeMunicipioEstadoCNH         
            grid.AdicionarCabecalho("Cód. Seg. CNH", "CodigoSegurancaCNH", 10, Models.Grid.Align.left, false, false, false, false, false);//Localization.Resources.Relatorios.Frotas.Motorista.CodigoSegurancaCNH         
            grid.AdicionarCabecalho("Gestor", "Gestor", 10, Models.Grid.Align.left, true, false, false, true, true);//Localization.Resources.Relatorios.Frotas.Motorista.Gestor

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.Bloqueado, "BloqueadoDescricao", 10, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Frotas.Motorista.MotivoBloqueio, "MotivoBloqueio", 10, Models.Grid.Align.left, false, false, false, false, false);
            }

            //Colunas montadas dinamicamente
            for (int i = 0; i < epis.Count; i++)
            {
                if (i < NumeroMaximoEPIs)
                {
                    grid.AdicionarCabecalho(epis[i].Descricao, "QuantidadeEPI" + UltimaColunaDinanica.ToString(), 8, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum, epis[i].Codigo);

                    UltimaColunaDinanica++;
                }
                else
                    break;
            }

            return grid;
        }

        protected override Task<FiltroPesquisaRelatorioMotorista> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}