using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Localidades
{
    [CustomAuthorize("Localidades/DistribuidorPorRegiao")]
    public class DistribuidorPorRegiaoController : BaseController
    {
		#region Construtores

		public DistribuidorPorRegiaoController(Conexao conexao) : base(conexao) { }

		#endregion


        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Região", "Regiao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Distribuidor", "Distribuidor", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipos de Carga", "TipoDeCarga", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaDistribuidorRegiao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Localidades.DistribuidorRegiao repositorioDistribuidorRegiao = new Repositorio.Embarcador.Localidades.DistribuidorRegiao(unitOfWork);

                int totalRegistro = repositorioDistribuidorRegiao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao> distribuidoresRegiao =
                    (totalRegistro > 0) ?
                    repositorioDistribuidorRegiao.Consultar(filtrosPesquisa, parametrosConsulta) :
                    new List<Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao>();

                var registros = (
                    from obj in distribuidoresRegiao
                    select new
                    {
                        obj.Codigo,
                        Regiao = obj.Regiao.Descricao.ToString() ?? "",
                        Distribuidor = obj.ClienteDistribuidor.Descricao ?? "",
                        Situacao = obj.Situacao == true ? "Ativo" : "Inativo",
                        TipoDeCarga = string.Join(", ", obj.TiposDeCargas.Select(o => o.Descricao))
                    }
                ).ToList();

                grid.AdicionaRows(registros);
                grid.setarQuantidadeTotal(totalRegistro);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaDistribuidorRegiao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaDistribuidorRegiao()
            {
                CodigoDistribuidor = Request.GetIntParam("Distribuidor"),
                CodigoRegiao = Request.GetIntParam("Regiao")
            };
        }

        private string ObterPropriedadeOrdenar(string prop)
        {
            return prop;
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Localidades.DistribuidorRegiao repositorioDistribuidorPorRegiao = new Repositorio.Embarcador.Localidades.DistribuidorRegiao(unitOfWork);

                Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao distribuidorPorRegiao = repositorioDistribuidorPorRegiao.BuscarPorCodigo(codigo);

                if (distribuidorPorRegiao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    Regiao = new { distribuidorPorRegiao.Regiao.Codigo, distribuidorPorRegiao.Regiao.Descricao },
                    Distribuidor = new { distribuidorPorRegiao.ClienteDistribuidor.Codigo, distribuidorPorRegiao.ClienteDistribuidor.Descricao },
                    Status = distribuidorPorRegiao.Situacao,
                    TiposDeCargas = (
                        from obj in distribuidorPorRegiao.TiposDeCargas
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                        }).ToList()

                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Localidades.DistribuidorRegiao repositorioDistribuidorRegiao = new Repositorio.Embarcador.Localidades.DistribuidorRegiao(unitOfWork);
                Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao distribuidorRegiao = new Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao();

                PreencherEntidade(distribuidorRegiao, unitOfWork);
                SalvarTipoDeCarga(ref distribuidorRegiao, unitOfWork);

                if (repositorioDistribuidorRegiao.ExisteRegistroDuplicado(distribuidorRegiao))
                    return new JsonpResult(false, "Já existe um Distribuidor cadastrado para essa região.");

                repositorioDistribuidorRegiao.Inserir(distribuidorRegiao);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                int codigo = Request.GetIntParam("Codigo");

                unitOfWork.Start();

                Repositorio.Embarcador.Localidades.DistribuidorRegiao repositorioDistribuidorRegiao = new Repositorio.Embarcador.Localidades.DistribuidorRegiao(unitOfWork);
                Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao distribuidorRegiao = repositorioDistribuidorRegiao.BuscarPorCodigo(codigo);

                PreencherEntidade(distribuidorRegiao, unitOfWork);
                SalvarTipoDeCarga(ref distribuidorRegiao, unitOfWork);

                repositorioDistribuidorRegiao.Atualizar(distribuidorRegiao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao distribuidorRegiao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Localidades.DistribuidorRegiao repositorioDistribuidorRegiao = new Repositorio.Embarcador.Localidades.DistribuidorRegiao(unitOfWork);
            Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            distribuidorRegiao.Codigo = Request.GetIntParam("Codigo");
            distribuidorRegiao.Regiao = repositorioRegiao.BuscarPorCodigo(Request.GetIntParam("Regiao"));
            distribuidorRegiao.ClienteDistribuidor = repositorioCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Distribuidor"));
            distribuidorRegiao.Situacao = Request.GetBoolParam("Status");

        }

        private void SalvarTipoDeCarga(ref Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao distribuidorPorRegiao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);

            dynamic tiposDeCargas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposDeCargas"));

            if (distribuidorPorRegiao.TiposDeCargas == null)
            {
                distribuidorPorRegiao.TiposDeCargas = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoDeCarga in tiposDeCargas)
                    codigos.Add((int)tipoDeCarga.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeletar = distribuidorPorRegiao.TiposDeCargas.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCargaDeletar in tiposDeletar)
                    distribuidorPorRegiao.TiposDeCargas.Remove(tipoDeCargaDeletar);
            }

            foreach (var tipoDeCarga in tiposDeCargas)
            {
                int codigo = 0;
                codigo = tipoDeCarga.Codigo;

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaAdicionar = repTipoCarga.BuscarPorCodigo(codigo);

                if (!distribuidorPorRegiao.TiposDeCargas.Any(o => o.Codigo == (int)tipoDeCarga.Codigo))
                    distribuidorPorRegiao.TiposDeCargas.Add(tipoCargaAdicionar);
            }

        }

    }
}
