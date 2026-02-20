using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Gerais
{
    [CustomAuthorize("Sistema/ModeloFiltroPesquisa")]
    public class ModeloFiltroPesquisaController : BaseController
    {
        #region Construtores

        public ModeloFiltroPesquisaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Publicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                string descricaoModelo = Request.GetStringParam("Modelo");
                Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa tipoFiltro = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa>("TipoFiltro");

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoUsuario", false);
                grid.AdicionarCabecalho("ModeloPadrao", false);
                grid.AdicionarCabecalho("ModeloExclusivoUsuario", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Modelo, "ModeloDescricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.ModeloPadrao, "ModeloPadraoDescricao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.ModeloExclusivoUsuario, "ModeloExclusivoUsuarioDescricao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Dados", false);

                Repositorio.FiltroPesquisa repositorioFiltroPesquisa = new Repositorio.FiltroPesquisa(unitOfWork);
                List<Dominio.Entidades.Global.FiltroPesquisa> registros = repositorioFiltroPesquisa.BuscarFiltrosPesquisaModelo(tipoFiltro, this.Usuario.Codigo, descricaoModelo);

                var lista = (
                    from p in registros
                    select new
                    {
                        p.Codigo,
                        CodigoUsuario = p.Usuario.Codigo,
                        ModeloPadrao = p.Modelo?.ModeloPadrao ?? false,
                        ModeloExclusivoUsuario = p.Modelo?.ModeloExclusivoUsuario ?? false,
                        AvancarDatasAutomaticamente = p.Modelo?.AvancarDatasAutomaticamente ?? false,
                        ModeloDescricao = p.Modelo?.Descricao ?? string.Empty,
                        ModeloPadraoDescricao = p.Modelo?.ModeloPadrao ?? false ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                        ModeloExclusivoUsuarioDescricao = p.Modelo?.ModeloExclusivoUsuario ?? false ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                        Dados = CalcularDatasDinamicas(p.Dados, p.Modelo?.AvancarDatasAutomaticamente ?? false, p.DataInclusaoFiltro)
                    }
                ).ToList();

                grid.setarQuantidadeTotal(lista.Count);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarModelos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricaoModelo = Request.GetStringParam("Modelo");
                Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa tipoFiltro = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa>("TipoFiltro");

                Repositorio.FiltroPesquisa repositorioFiltroPesquisa = new Repositorio.FiltroPesquisa(unitOfWork);
                List<Dominio.Entidades.Global.FiltroPesquisa> registros = repositorioFiltroPesquisa.BuscarFiltrosPesquisaModelo(tipoFiltro, this.Usuario.Codigo, descricaoModelo);

                var lista = (
                    from p in registros
                    select new
                    {
                        p.Codigo,
                        CodigoUsuario = p.Usuario.Codigo,
                        ModeloPadrao = p.Modelo?.ModeloPadrao ?? false,
                        ModeloExclusivoUsuario = p.Modelo?.ModeloExclusivoUsuario ?? false,
                        AvancarDatasAutomaticamente = p.Modelo?.AvancarDatasAutomaticamente ?? false,
                        ModeloDescricao = p.Modelo?.Descricao ?? string.Empty,
                        ModeloPadraoDescricao = p.Modelo?.ModeloPadrao ?? false ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                        ModeloExclusivoUsuarioDescricao = p.Modelo?.ModeloExclusivoUsuario ?? false ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                        Dados = CalcularDatasDinamicas(p.Dados, p.Modelo?.AvancarDatasAutomaticamente ?? false, p.DataInclusaoFiltro)
                    }
                ).ToList();

                return new JsonpResult(lista);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterFiltroPesquisaPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.FiltroPesquisa repositorioFiltroPesquisa = new Repositorio.FiltroPesquisa(unitOfWork);

                Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa tipoFiltro = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa>("TipoFiltro");

                // Busca filtro pesquisa padrão do usuário
                Dominio.Entidades.Global.FiltroPesquisa filtroPesquisa = repositorioFiltroPesquisa.BuscarFiltroPesquisaPadrao(tipoFiltro, this.Usuario.Codigo, true);

                // Busca filtro pesquisa padrão da tela se não encontrar pelo usuário
                if (filtroPesquisa == null)
                    filtroPesquisa = repositorioFiltroPesquisa.BuscarFiltroPesquisaPadrao(tipoFiltro, this.Usuario.Codigo, false);

                if (filtroPesquisa != null)
                {
                    return new JsonpResult(
                        new
                        {
                            filtroPesquisa.Codigo,
                            filtroPesquisa.Modelo.Descricao,
                            Dados = CalcularDatasDinamicas(filtroPesquisa.Dados, filtroPesquisa.Modelo?.AvancarDatasAutomaticamente ?? false, filtroPesquisa.DataInclusaoFiltro)
                        }
                    );
                }

                return new JsonpResult(null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarModeloFiltroPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.FiltroPesquisa repositorioFiltroPesquisa = new Repositorio.FiltroPesquisa(unitOfWork);
                Repositorio.FiltroPesquisaModelo repositorioFiltroPesquisaModelo = new Repositorio.FiltroPesquisaModelo(unitOfWork);

                Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa tipoFiltro = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa>("TipoFiltro");
                string filtros = Request.GetStringParam("Filtros");
                string descricao = Request.GetStringParam("Descricao");
                bool modeloExclusivoUsuario = Request.GetBoolParam("ModeloExclusivoUsuario");
                bool modeloPadrao = Request.GetBoolParam("ModeloPadrao");
                bool avancarDatasAutomaticamente = Request.GetBoolParam("AvancarDatasAutomaticamente");

                unitOfWork.Start();

                // Quando o novo modelo for padrão remove o modelo padrão de outros modelos, caso haja
                if (modeloPadrao)
                {
                    Dominio.Entidades.Global.FiltroPesquisa filtroPesquisaModeloPadrao = repositorioFiltroPesquisa.BuscarFiltroPesquisaPadrao(tipoFiltro, this.Usuario.Codigo, modeloExclusivoUsuario);
                    if (filtroPesquisaModeloPadrao != null)
                    {
                        Dominio.Entidades.Global.FiltroPesquisaModelo filtroPesquisaModelo = filtroPesquisaModeloPadrao.Modelo;
                        filtroPesquisaModelo.ModeloPadrao = false;
                        repositorioFiltroPesquisaModelo.Atualizar(filtroPesquisaModelo);
                    }
                }

                Dominio.Entidades.Global.FiltroPesquisaModelo filtroPesquisaModeloNovo = new Dominio.Entidades.Global.FiltroPesquisaModelo()
                {
                    Descricao = descricao,
                    ModeloExclusivoUsuario = modeloExclusivoUsuario,
                    ModeloPadrao = modeloPadrao,
                    AvancarDatasAutomaticamente = avancarDatasAutomaticamente,
                };

                repositorioFiltroPesquisaModelo.Inserir(filtroPesquisaModeloNovo);

                Dominio.Entidades.Global.FiltroPesquisa filtroPesquisaNovo = new Dominio.Entidades.Global.FiltroPesquisa()
                {
                    CodigoFiltroPesquisa = tipoFiltro,
                    Dados = filtros,
                    Usuario = this.Usuario,
                    Modelo = filtroPesquisaModeloNovo,
                    DataInclusaoFiltro = DateTime.Now
                };

                repositorioFiltroPesquisa.Inserir(filtroPesquisaNovo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverModeloFiltroPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.FiltroPesquisa repositorioFiltroPesquisa = new Repositorio.FiltroPesquisa(unitOfWork);
                Repositorio.FiltroPesquisaModelo repositorioFiltroPesquisaModelo = new Repositorio.FiltroPesquisaModelo(unitOfWork);

                int codigoModelo = Request.GetIntParam("CodigoModelo");

                Dominio.Entidades.Global.FiltroPesquisa filtroPesquisa = repositorioFiltroPesquisa.BuscarPorCodigo(codigoModelo, false);

                if (filtroPesquisa == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                Dominio.Entidades.Global.FiltroPesquisaModelo filtroPesquisaModelo = filtroPesquisa.Modelo;

                unitOfWork.Start();

                repositorioFiltroPesquisa.Deletar(filtroPesquisa);

                repositorioFiltroPesquisaModelo.Deletar(filtroPesquisaModelo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados
        private string CalcularDatasDinamicas(string dados, bool avancarDatasAutomaticamente, DateTime? dataInclusaoFiltro)
        {
            if (!avancarDatasAutomaticamente || !dataInclusaoFiltro.HasValue)
                return dados;

            var objFiltrosDeserializados = JsonConvert.DeserializeObject<JObject>(dados);

            foreach (var property in objFiltrosDeserializados.Properties())
            {
                JToken propertyValue = property.Value;

                if (propertyValue is JObject valueObject && valueObject.ContainsKey("type"))
                {
                    string tipoProperty = valueObject["type"]?.ToString();

                    if (tipoProperty == "dateTime" || tipoProperty == "date")
                    {
                        if (DateTime.TryParse(valueObject["val"]?.ToString(), out DateTime parsedDate))
                        {
                            var horasFiltroOriginal = parsedDate.TimeOfDay;
                            var dateDiff = parsedDate.Date - dataInclusaoFiltro.Value.Date;
                            double minutosDiferenca = dateDiff.TotalMinutes;
                            DateTime dataFinal = DateTime.UtcNow.AddMinutes(minutosDiferenca);

                            dataFinal = new DateTime(dataFinal.Year, dataFinal.Month, dataFinal.Day,
                                                                         horasFiltroOriginal.Hours, horasFiltroOriginal.Minutes, horasFiltroOriginal.Seconds);

                            valueObject["val"] = tipoProperty == "date" ? dataFinal.ToDateString() : dataFinal.ToDateTimeString();
                        }
                    }
                }
            }

            dados = JsonConvert.SerializeObject(objFiltrosDeserializados);
            return dados;
        }
        #endregion
    }
}
