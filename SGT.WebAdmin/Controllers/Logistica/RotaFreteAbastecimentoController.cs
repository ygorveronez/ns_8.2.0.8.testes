using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/RotaFreteAbastecimento")]
    public class RotaFreteAbastecimentoController : BaseController
    {
		#region Construtores

		public RotaFreteAbastecimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento rotaFreteAbastecimento = new Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento();

                PreencherRotaFreteAbastecimento(rotaFreteAbastecimento, unitOfWork);

                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.RotaFreteAbastecimento repositorio = new Repositorio.Embarcador.Logistica.RotaFreteAbastecimento(unitOfWork);

                repositorio.Inserir(rotaFreteAbastecimento, Auditado);
                SalvarPreAbastecimento(rotaFreteAbastecimento, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                Repositorio.Embarcador.Logistica.RotaFreteAbastecimento repositorio = new Repositorio.Embarcador.Logistica.RotaFreteAbastecimento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento rotaFreteAbastecimento = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (rotaFreteAbastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRotaFreteAbastecimento(rotaFreteAbastecimento, unitOfWork);

                unitOfWork.Start();

                SalvarPreAbastecimento(rotaFreteAbastecimento, unitOfWork);

                repositorio.Atualizar(rotaFreteAbastecimento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Logistica.RotaFreteAbastecimento repositorio = new Repositorio.Embarcador.Logistica.RotaFreteAbastecimento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento rotaFreteAbastecimento = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (rotaFreteAbastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    rotaFreteAbastecimento.Codigo,
                    RotaEmbarcador = new
                    {
                        Codigo = rotaFreteAbastecimento.RotaFrete?.Codigo ?? 0,
                        Descricao = rotaFreteAbastecimento.RotaFrete?.Descricao ?? ""
                    },
                    ModeloVeicular = new
                    {
                        Codigo = rotaFreteAbastecimento.ModeloVeicular?.Codigo ?? 0,
                        Descricao = rotaFreteAbastecimento.ModeloVeicular?.Descricao ?? ""
                    },
                    PreAbastecimentos = ObterPreAbastecimento(rotaFreteAbastecimento)
                });
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.RotaFreteAbastecimento repositorio = new Repositorio.Embarcador.Logistica.RotaFreteAbastecimento(unitOfWork);
                Repositorio.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento repPreAbastecimento = new Repositorio.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento(unitOfWork);


                Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento rotaFreteAbastecimento = repositorio.BuscarPorCodigo(codigo, auditavel: true);
                List<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento> preAbastecimentos = repPreAbastecimento.BuscarPorRotaFreteAbastecimento(codigo);


                if (rotaFreteAbastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento preAbastecimento in preAbastecimentos)
                    repPreAbastecimento.Deletar(preAbastecimento, Auditado);

                repositorio.Deletar(rotaFreteAbastecimento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherRotaFreteAbastecimento(Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento rotaFreteAbastecimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.RotaFrete reRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            int.TryParse(Request.Params("ModeloVeicular"), out int codigoModeloVeicular);
            int.TryParse(Request.Params("RotaEmbarcador"), out int codigoRotaFrete);

            rotaFreteAbastecimento.ModeloVeicular = repModeloVeicular.BuscarPorCodigo(codigoModeloVeicular);
            rotaFreteAbastecimento.RotaFrete = reRotaFrete.BuscarPorCodigo(codigoRotaFrete);
            rotaFreteAbastecimento.Descricao = "";
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Rota Embarcador", "RotaEmbarcador", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 15, Models.Grid.Align.left, true);

                int codigoRotaEmbarcador = Request.GetIntParam("RotaEmbarcador");
                int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Logistica.RotaFreteAbastecimento repositorio = new Repositorio.Embarcador.Logistica.RotaFreteAbastecimento(unitOfWork);

                int totalRegistros = repositorio.ContarConsulta(codigoRotaEmbarcador, codigoModeloVeicular);
                List<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento> listaRotaFreteAbastecimento = (totalRegistros > 0) ? repositorio.Consultar(codigoRotaEmbarcador, codigoModeloVeicular, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento>();

                var listaRotaFreteAbastecimentoRetornar = (
                    from Abastecimento in listaRotaFreteAbastecimento
                    select new
                    {
                        Abastecimento.Codigo,
                        RotaEmbarcador = Abastecimento.RotaFrete?.Descricao ?? "",
                        ModeloVeicular = Abastecimento.ModeloVeicular?.Descricao ?? ""
                        
                    }
                ).ToList();

                grid.AdicionaRows(listaRotaFreteAbastecimentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private void SalvarPreAbastecimento(Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento rotaFreteAbastecimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento repositorioRotaFreteAbastecimentoPre = new Repositorio.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento(unitOfWork);

            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PreAbastecimentos"));

            if (rotaFreteAbastecimento.PreAbastecimentos?.Count > 0)
            {
                List<int> codigos = new List<int>();
                foreach (dynamic preAbastecimento in listaDinamica)
                {
                    int.TryParse((string)preAbastecimento.Codigo, out int codigoLiberacao);
                    if (codigoLiberacao > 0)
                        codigos.Add((int)preAbastecimento.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento> PreAbastecimentoDeletar = (from obj in rotaFreteAbastecimento.PreAbastecimentos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < PreAbastecimentoDeletar.Count; i++)
                    repositorioRotaFreteAbastecimentoPre.Deletar(PreAbastecimentoDeletar[i]);
            }
            else
                rotaFreteAbastecimento.PreAbastecimentos = new List<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento>();

            foreach (var dadoDinamico in listaDinamica)
            {
                int codigo = ((string)dadoDinamico.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento entidade = codigo > 0 ? repositorioRotaFreteAbastecimentoPre.BuscarPorCodigo(codigo, false) : null;

                if (entidade == null)
                    entidade = new Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento();

                int.TryParse((string)dadoDinamico.Produto, out int codigoProduto);
                double.TryParse((string)dadoDinamico.Posto, out double codigoPosto);

                string valorunitario = dadoDinamico.ValorUnitario;
                decimal litros = 0;

                decimal.TryParse((string)dadoDinamico.Litros, out litros);


                entidade.Produto = codigoProduto > 0 ? repProduto.BuscarPorCodigo(codigoProduto) : null;
                entidade.Posto = codigoPosto > 0 ? repCliente.BuscarPorCPFCNPJ(codigoPosto) : null;
                entidade.ValorUnitario = decimal.Parse(valorunitario);
                entidade.Litros = litros;
                entidade.TipoAbastecimento = dadoDinamico.TipoAbastecimento;
                entidade.RotaFreteAbastecimento = rotaFreteAbastecimento;
                entidade.Descricao = "";
                entidade.TanqueCheio = dadoDinamico.TanqueCheio;

                if (entidade.Codigo > 0)
                    repositorioRotaFreteAbastecimentoPre.Atualizar(entidade, Auditado);
                else
                    repositorioRotaFreteAbastecimentoPre.Inserir(entidade, Auditado);
            }
        }
        
        private dynamic ObterPreAbastecimento(Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento rotaFreteAbastecimento)
        {
            return (from obj in rotaFreteAbastecimento.PreAbastecimentos
                    select new
                    {
                        Codigo = obj.Codigo,
                        Posto = obj.Posto.Codigo,
                        DescricaoPosto = obj.Posto.Descricao,
                        Produto = obj.Produto.Codigo,
                        DescricaoProduto = obj.Produto.Descricao,
                        ValorUnitario = obj.ValorUnitario.ToString("n4"),
                        Litros = obj.Litros.ToString("n4"),
                        TipoAbastecimento = obj.TipoAbastecimento,
                        TipoAbastecimentoDescricao = obj.TipoAbastecimento.ObterDescricao(),
                        ValorTotal = obj.ValorTotal.ToString("n2"),
                        TanqueCheio = obj.TanqueCheio,
                        TanqueCheioDescricao = obj.TanqueCheioDescricao
                    }).ToList();
        }
        #endregion
    }
}
