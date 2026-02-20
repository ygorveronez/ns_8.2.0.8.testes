using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/VeiculoLicenca")]
    public class VeiculoLicencaController : BaseController
    {
		#region Construtores

		public VeiculoLicencaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoVeiculo = Request.GetIntParam("Veiculo");

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo) ?? throw new ControllerException("Não foi possível encontrar o veículo.");
                Dominio.ObjetosDeValor.Embarcador.Veiculos.LicencaVeiculoSalvar licenca = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Veiculos.LicencaVeiculoSalvar>(Request.Params("Licenca"));

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    List<string> faixasTemperatura = JsonConvert.DeserializeObject<List<string>>(licenca.FaixaTemperatura);
                    if (faixasTemperatura.Count == 0)
                        throw new ControllerException(Localization.Resources.Veiculos.VeiculoLicenca.FavorInformarAlgumaFaixaDeTemperatura);
                }

                Servicos.Embarcador.Veiculo.LicencaVeiculo servicoLicencaVeiculo = new Servicos.Embarcador.Veiculo.LicencaVeiculo(unitOfWork, TipoServicoMultisoftware);

                servicoLicencaVeiculo.AdicionarOuAtualizar(veiculo, licenca, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Veiculos.VeiculoLicenca.OcorreuUmaFalhaAoAdicionarLicencaDoVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoVeiculo = Request.GetIntParam("Veiculo");

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo) ?? throw new ControllerException(Localization.Resources.Veiculos.VeiculoLicenca.NaoFoiPossivelEncontrarVeiculo);
                Dominio.ObjetosDeValor.Embarcador.Veiculos.LicencaVeiculoSalvar licenca = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Veiculos.LicencaVeiculoSalvar>(Request.Params("Licenca"));

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    List<string> faixasTemperatura = JsonConvert.DeserializeObject<List<string>>(licenca.FaixaTemperatura);
                    if (faixasTemperatura.Count == 0)
                        throw new ControllerException(Localization.Resources.Veiculos.VeiculoLicenca.FavorInformarAlgumaFaixaDeTemperatura);
                }

                Servicos.Embarcador.Veiculo.LicencaVeiculo servicoLicencaVeiculo = new Servicos.Embarcador.Veiculo.LicencaVeiculo(unitOfWork, TipoServicoMultisoftware);

                servicoLicencaVeiculo.AdicionarOuAtualizar(veiculo, licenca, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Veiculos.VeiculoLicenca.OcorreuUmaFalhaAoAtualizarLicencaDoVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Veiculo.LicencaVeiculo servicoLicencaVeiculo = new Servicos.Embarcador.Veiculo.LicencaVeiculo(unitOfWork, TipoServicoMultisoftware);
                Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorio = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca = repositorio.BuscarPorCodigo(codigo);

                if (licenca == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                return new JsonpResult(servicoLicencaVeiculo.ObterDetalhes(licenca));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Veiculos.VeiculoLicenca.OcorreuUmaFalhaAoConsultarLicencaDoVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorio = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (licenca == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculoAnexo, Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> servicoLicencaAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculoAnexo, Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>(unitOfWork);

                servicoLicencaAnexo.ExcluirAnexos(licenca);
                repositorio.Deletar(licenca, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Veiculos.VeiculoLicenca.OcorreuUmaFalhaAoRemoverLicencaDoVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoVeiculoLicenca();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoVeiculoLicenca();
                List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasVeiculo = new List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, licencasVeiculo, ((dados) =>
                {
                    Servicos.Embarcador.Veiculo.VeiculoLicencaImportar servicoVeiculoImportar = new Servicos.Embarcador.Veiculo.VeiculoLicencaImportar(dados, Usuario, TipoServicoMultisoftware, unitOfWork);

                    return servicoVeiculoImportar.ObterVeiculoLicencaImportar();
                }));

                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);

                int totalRegistrosImportados = 0;
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                bool permiteInserir = (bool)parametro.Inserir;
                bool permiteAtualizar = (bool)parametro.Atualizar;
                Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorioLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licencaVeiculo in licencasVeiculo)
                {
                    if (licencaVeiculo.Codigo == 0 && permiteInserir)
                    {
                        repositorioLicencaVeiculo.Inserir(licencaVeiculo, Auditado);
                        totalRegistrosImportados++;
                    }
                    //Não permite atualizar
                }

                unitOfWork.CommitChanges();

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVeiculo = Request.GetIntParam("Veiculo");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                if (veiculo == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    ExibirSelecaoContainer = veiculo.ModeloVeicularCarga?.ContainerTipo != null
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaLicencaVeiculo ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaLicencaVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaLicencaVeiculo()
            {
                CodigoMotorista = Request.GetIntParam("Motorista"),
                DataEmissaoInicial = Request.GetNullableDateTimeParam("DataEmissaoInicial"),
                DataEmissaoLimite = Request.GetNullableDateTimeParam("DataEmissaoLimite"),
                DataVencimentoInicial = Request.GetNullableDateTimeParam("DataVencimentoInicial"),
                DataVencimentoLimite = Request.GetNullableDateTimeParam("DataVencimentoLimite"),
                Placa = Request.GetStringParam("Placa"),
                StatusLicenca = Request.GetNullableEnumParam<StatusLicenca>("StatusLicenca"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                CodigoFilial = Request.GetIntParam("Filial"),
                NumeroContainer = Request.GetStringParam("NumeroContainer"),
                StatusVigencia = Request.GetNullableEnumParam<StatusLicenca>("StatusVigencia"),
            };

            int codigoEmpresa = Request.GetIntParam("Empresa");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                filtrosPesquisa.Empresa = Usuario.Empresa;
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                filtrosPesquisa.Proprietario = Usuario.ClienteTerceiro ?? null;
            else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && codigoEmpresa > 0)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                filtrosPesquisa.Empresa = repositorioEmpresa.BuscarPorCodigo(codigoEmpresa);
            }

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                bool isEmbarcador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe;

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.Veiculo, "Placa", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.ModeloVeicular, "ModeloVeicular", 10, Models.Grid.Align.center, true, false);
                grid.AdicionarCabecalho(isEmbarcador ? Localization.Resources.Gerais.Geral.Observacao : Localization.Resources.Gerais.Geral.Descricao, "Descricao", 30, Models.Grid.Align.left, false);
                if (!isEmbarcador)
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.Numero, "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.DataDaEmissao, "DataEmissao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.DataDeVencimento, "DataVencimento", 10, Models.Grid.Align.center, true);
                if (!isEmbarcador)
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.ClassificacaoOnu, "ClassificacaoRiscoONU", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.Licenca, "Licenca", 20, Models.Grid.Align.left, false);
                if (!isEmbarcador)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "Status", 10, Models.Grid.Align.left, false);

                if (isEmbarcador)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.StatusTeste, "Status", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.Vencido, "DescricaoVencido", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.Transportador, "Transportador", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.Filial, "Filial", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.FaixaDeTemperatura, "FaixaTemperatura", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.UltimaCarga, "UltimaCarga", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.NumeroContainer, "NumeroContainer", 10, Models.Grid.Align.left, false, false);
                }

                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.UsuarioAlteracao, "UsuarioAlteracao", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoLicenca.DataAlteracao, "DataAlteracao", 10, Models.Grid.Align.left, false, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "VeiculoLicenca/Pesquisa", "grid-pesquisa-veiculo-licencas");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaLicencaVeiculo filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorio = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaLicenca repositorioCargaLicenca = new Repositorio.Embarcador.Cargas.CargaLicenca(unitOfWork);

                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> listaLicenca = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();

                var listaLicencaRetornar = (
                    from licenca in listaLicenca
                    select new
                    {
                        licenca.Codigo,
                        licenca.Veiculo.Placa,
                        ModeloVeicular = licenca.Veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                        licenca.Descricao,
                        licenca.Numero,
                        DataEmissao = licenca.DataEmissao.Value.ToString("dd/MM/yyyy"),
                        DataVencimento = licenca.DataVencimento.Value.ToString("dd/MM/yyyy"),
                        ClassificacaoRiscoONU = licenca.ClassificacaoRiscoONU?.Descricao ?? string.Empty,
                        Licenca = licenca.Licenca?.Descricao ?? "",
                        FaixaTemperatura = string.Join(", ", (from o in licenca.FaixasTemperatura select o.Descricao)),
                        Transportador = licenca.Veiculo?.Empresa?.RazaoSocial ?? string.Empty,
                        Filial = licenca.Filial?.Descricao ?? string.Empty,
                        Status = licenca.Status.ObterDescricao(),
                        DataAlteracao = licenca.DataAlteracao?.ToDateTimeString() ?? string.Empty,
                        UsuarioAlteracao = licenca.UsuarioAlteracao?.Nome ?? string.Empty,
                        licenca.DescricaoVencido,
                        UltimaCarga = isEmbarcador ? repositorioCargaLicenca.BuscarNumeroUltimaCarga(licenca.Codigo) : string.Empty,
                        licenca.NumeroContainer
                    }
                ).ToList();

                grid.AdicionaRows(listaLicencaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Placa")
                return "Veiculo.Placa";

            return propriedadeOrdenar;
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoVeiculoLicenca()
        {
            List<ConfiguracaoImportacao> configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;
            bool tipoServicoMultiEmbarcador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Veiculos.VeiculoLicenca.Veiculo, Propriedade = "Veiculo", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Veiculos.VeiculoLicenca.Licenca, Propriedade = "Licenca", Tamanho = tamanho, CampoInformacao = true });
            if (tipoServicoMultiEmbarcador)
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Veiculos.VeiculoLicenca.Filial, Propriedade = "Filial", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Veiculos.VeiculoLicenca.DataDaEmissao, Propriedade = "DataEmissao", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Veiculos.VeiculoLicenca.DataDeVencimento, Propriedade = "DataVencimento", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });

            if (tipoServicoMultiEmbarcador)
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Gerais.Geral.Observacao, Propriedade = "Descricao", Tamanho = tamanho, CampoInformacao = true });
            else
            {
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Gerais.Geral.Descricao, Propriedade = "Descricao", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Veiculos.VeiculoLicenca.NumeroDaLicenca, Propriedade = "Numero", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            }

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Gerais.Geral.Status, Propriedade = "Status", Tamanho = tamanho, CampoInformacao = true });

            if (tipoServicoMultiEmbarcador)
            {
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Veiculos.VeiculoLicenca.FaixasDeTemperatura, Propriedade = "FaixasDeTemperatura", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Veiculos.VeiculoLicenca.TransportadorCnpj, Propriedade = "CnpjTransportador", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            }

            return configuracoes;
        }

        #endregion
    }
}
