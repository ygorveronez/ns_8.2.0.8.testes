using Dominio.Entidades.Global;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.Gerais
{
    [CustomAuthorize("Sistema/FiltroPesquisaPersonalizado")]
    public class FiltroPesquisaPersonalizadoController : BaseController
    {
		#region Construtores

		public FiltroPesquisaPersonalizadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Publicos

        [AllowAuthenticate]
        public async Task<IActionResult> ObterFiltroPesquisaPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.AbaFiltroPesquisaPersonalizado repositorioAbaFiltroPesquisa = new Repositorio.AbaFiltroPesquisaPersonalizado(unitOfWork);
                Repositorio.FiltroPesquisaPersonalizado repositorioFiltrosPesquisa = new Repositorio.FiltroPesquisaPersonalizado(unitOfWork);

                List<Dominio.Entidades.Global.FiltroPersonalizado> filtros = repositorioFiltrosPesquisa.BuscarFiltrosCliente(this.Cliente.Codigo);
                Dominio.Entidades.Global.AbaFiltroPersonalizado aba = repositorioAbaFiltroPesquisa.BuscarFiltroPadrao();


                var retorno = new
                {
                    NomeFiltros = (from obj in filtros
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.NomeFiltro
                                   }).ToList(),

                    Aba = (new {Codigo = aba?.Codigo, Descricao = aba?.Descricao})
                };

                return new JsonpResult(retorno);
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
        public async Task<IActionResult> AdicionarFiltrosPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.AbaFiltroPesquisaPersonalizado repositorioFiltroPesquisa = new Repositorio.AbaFiltroPesquisaPersonalizado(unitOfWork);
                Repositorio.FiltroPesquisaPersonalizado repositorioFiltrosPesquisa = new Repositorio.FiltroPesquisaPersonalizado(unitOfWork);
                List<Dominio.Entidades.Global.FiltroPersonalizado> filtrosPersonalizados = new List<FiltroPersonalizado>();

                string nomeAba = Request.GetStringParam("NomeAba");
                var nomeFiltrosSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("FiltrosSelecionados"));
                List<string> listaNomeFiltros = new List<string>();

                if (nomeAba == string.Empty)
                    return new JsonpResult(false, true, "É obrigatório salvar o nome da aba.");

                foreach (var filtro in nomeFiltrosSelecionados)
                {
                    if (filtro == string.Empty)
                        continue;

                    listaNomeFiltros.Add((string)filtro);
                }

                if (listaNomeFiltros.Count < 4)
                    return new JsonpResult(false, true, "É obrigatório ter ao menos 4 filtros selecionados.");

                unitOfWork.Start();


                Dominio.Entidades.Global.AbaFiltroPersonalizado filtroPesquisa = repositorioFiltroPesquisa.BuscarFiltroPadrao();

                if (filtroPesquisa != null)
                {

                    filtroPesquisa.Descricao = nomeAba;
                    repositorioFiltroPesquisa.Atualizar(filtroPesquisa);

                    filtrosPersonalizados = repositorioFiltrosPesquisa.BuscarFiltrosPorAba(filtroPesquisa.Codigo);

                    if (filtrosPersonalizados != null)
                    {
                        List<Dominio.Entidades.Global.FiltroPersonalizado> filtrosDeletar = filtrosPersonalizados.Where(o => !listaNomeFiltros.Contains(o.NomeFiltro)).ToList();

                        foreach (FiltroPersonalizado filtroDeletar in filtrosDeletar)
                            repositorioFiltrosPesquisa.Deletar(filtroDeletar);
                    }
                }
                else
                {

                    filtroPesquisa = new Dominio.Entidades.Global.AbaFiltroPersonalizado()
                    {
                        Descricao = nomeAba,
                        Cliente = this.Cliente.Codigo,
                        CodigoFiltroPesquisa = Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa.Monitoramento,
                        Data = DateTime.Now,
                    };

                    repositorioFiltroPesquisa.Inserir(filtroPesquisa);
                }

                foreach (string nomeFiltro in listaNomeFiltros)
                {

                    if (!repositorioFiltrosPesquisa.ExisteFiltroJaCadastrado(nomeFiltro))
                    {
                        Dominio.Entidades.Global.FiltroPersonalizado filtroPersonalizado = new Dominio.Entidades.Global.FiltroPersonalizado()
                        {
                            NomeFiltro = nomeFiltro,
                            AbaFiltroPersonalizado = filtroPesquisa,
                        };

                        repositorioFiltrosPesquisa.Inserir(filtroPersonalizado);
                    }
                }

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
        public async Task<IActionResult> RemoverAbaFiltrosPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.AbaFiltroPesquisaPersonalizado repositorioAbaFiltroPesquisa = new Repositorio.AbaFiltroPesquisaPersonalizado(unitOfWork);
                Repositorio.FiltroPesquisaPersonalizado repositorioFiltrosPesquisa = new Repositorio.FiltroPesquisaPersonalizado(unitOfWork);

                List<Dominio.Entidades.Global.FiltroPersonalizado> filtros = repositorioFiltrosPesquisa.BuscarFiltrosCliente(this.Cliente.Codigo);
                Dominio.Entidades.Global.AbaFiltroPersonalizado aba = repositorioAbaFiltroPesquisa.BuscarFiltroPadrao();

                if (aba == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Global.FiltroPersonalizado filtro in filtros)
                    repositorioFiltrosPesquisa.Deletar(filtro);

                repositorioAbaFiltroPesquisa.Deletar(aba);

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

        #region Métodos privados

        #endregion
    }
}
