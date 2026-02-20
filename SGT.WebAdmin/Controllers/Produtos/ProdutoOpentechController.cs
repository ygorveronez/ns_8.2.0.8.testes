using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/ProdutoOpentech")]
    public class ProdutoOpentechController : BaseController
    {
        #region Construtores

        public ProdutoOpentechController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(await ObterGridPesquisa(cancellationToken));
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

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.ProdutoOpentech repProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = await repProdutoOpentech.BuscarPorCodigoAsync(codigo);

                if (produtoOpentech == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    produtoOpentech.Codigo,
                    Status = produtoOpentech.Ativo,
                    TipoOperacao = new { Codigo = produtoOpentech.TipoOperacao?.Codigo ?? 0, Descricao = produtoOpentech.TipoOperacao?.Descricao ?? string.Empty },
                    Apolice = new { Codigo = produtoOpentech.ApoliceSeguro?.Codigo ?? 0, Descricao = produtoOpentech.ApoliceSeguro?.Descricao + " - " + produtoOpentech.ApoliceSeguro?.Seguradora.Nome ?? string.Empty },
                    ValorDe = produtoOpentech.ValorDe.ToString("n2"),
                    ValorAte = produtoOpentech.ValorAte.ToString("n2"),
                    CodigoEmbarcador = produtoOpentech.CodigoEmbarcador,
                    TipoCarga = new { Codigo = produtoOpentech.TipoDeCarga?.Codigo ?? 0, Descricao = produtoOpentech.TipoDeCarga?.Descricao ?? string.Empty },
                    Estados = (from obj in produtoOpentech.Estados
                               select new
                               {
                                   Codigo = obj.Sigla,
                                   Descricao = obj.Nome
                               }).ToList(),
                    Localidades = (from obj in produtoOpentech.Localidades
                                   select new
                                   {
                                       Codigo = obj.Codigo,
                                       Descricao = obj.Descricao,
                                       Estado = !string.IsNullOrWhiteSpace(obj.Estado.Abreviacao) ? obj.Estado.Abreviacao : obj.Estado.Sigla,
                                   }).ToList(),
                    ProdutoOpentech = produtoOpentech.CodigoProdutoOpentech.ToString()
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                dynamic estados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Estados"));
                if (estados == null || estados.GetType() != typeof(Newtonsoft.Json.Linq.JArray) || estados.Count <= 0)
                    return new JsonpResult(false, true, "Obrigatório informar um estado.");

                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Integracao.ProdutoOpentech repProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = new Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech();

                await PreencheEntidade(produtoOpentech, unitOfWork, cancellationToken);

                await repProdutoOpentech.InserirAsync(produtoOpentech, Auditado);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                dynamic estados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Estados"));
                if (estados == null || estados.GetType() != typeof(Newtonsoft.Json.Linq.JArray) || estados.Count <= 0)
                    return new JsonpResult(false, true, "Obrigatório informar um estado.");

                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Integracao.ProdutoOpentech repProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = await repProdutoOpentech.BuscarPorCodigoAsync(codigo, true);

                if (produtoOpentech == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                await PreencheEntidade(produtoOpentech, unitOfWork, cancellationToken);

                await repProdutoOpentech.AtualizarAsync(produtoOpentech, Auditado);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            try
            {
                Models.Grid.Grid grid = await ObterGridPesquisa(cancellationToken);
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

        public async Task<IActionResult> ExcluirPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Integracao.ProdutoOpentech repProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = await repProdutoOpentech.BuscarPorCodigoAsync(codigo);

                if (produtoOpentech == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                await repProdutoOpentech.DeletarAsync(produtoOpentech, Auditado);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarProdutosOpenTech()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string mensagemErro = string.Empty;

                List<object> produtos = Servicos.Embarcador.Integracao.OpenTech.IntegracaoProdutoOpenTech.ObterProdutosOpenTech(unidadeTrabalho, out mensagemErro);

                if (produtos.IsNullOrEmpty() && !string.IsNullOrWhiteSpace(mensagemErro))
                    return new JsonpResult(produtos, false, "Ocorreu uma falha ao obter os produtos da OpenTech: " + mensagemErro);

                return new JsonpResult(produtos);
            }
            catch (BaseException ex)
            {
                return new JsonpResult(false, "Ocorreu uma falha ao obter os produtos da OpenTech: " + ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os produtos da OpenTech");
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("TipoOperacao").Nome("Tipo Operação").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("TipoCarga").Nome("Tipo Carga").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Apolice").Nome("Apolice").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Estados").Nome("Estados").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("DescricaoAtivo").Nome("Status").Tamanho(5).Align(Models.Grid.Align.left);

            return grid;
        }


        private async Task<(dynamic, int)> ExecutaPesquisa(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaProdutoOpentech filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Integracao.ProdutoOpentech repProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(unitOfWork);
            List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> listaGrid = await repProdutoOpentech.ConsultarAsync(filtrosPesquisa, parametrosConsulta);

            int totalRegistros = await repProdutoOpentech.ContarConsultaAsync(filtrosPesquisa);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            TipoOperacao = obj.TipoOperacao?.Descricao ?? string.Empty,
                            TipoCarga = obj.TipoDeCarga?.Descricao ?? string.Empty,
                            Apolice = obj.ApoliceSeguro?.Descricao + " - " + obj.ApoliceSeguro?.Seguradora.Nome ?? string.Empty,
                            Estados = obj.ListaEstados,
                            obj.DescricaoAtivo
                        };

            return (lista.ToList(), totalRegistros);
        }

        private async Task PreencheEntidade(Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork, cancellationToken);

            produtoOpentech.Ativo = Request.GetBoolParam("Status");

            int tipoOperacao = Request.GetIntParam("TipoOperacao");
            if (tipoOperacao > 0)
                produtoOpentech.TipoOperacao = await repTipoOperacao.BuscarPorCodigoAsync(tipoOperacao);
            else
                produtoOpentech.TipoOperacao = null;

            int TipoCarga = Request.GetIntParam("TipoCarga");
            if (TipoCarga > 0)
                produtoOpentech.TipoDeCarga = await repTipoCarga.BuscarPorCodigoAsync(TipoCarga);
            else
                produtoOpentech.TipoDeCarga = null;

            int apoliceSeguro = Request.GetIntParam("Apolice");
            if (apoliceSeguro > 0)
                produtoOpentech.ApoliceSeguro = await repApoliceSeguro.BuscarPorCodigoAsync(apoliceSeguro);
            else
                produtoOpentech.ApoliceSeguro = null;

            int.TryParse(Request.Params("ProdutoOpentech"), out int codigoProdutoOpentech);
            produtoOpentech.CodigoProdutoOpentech = codigoProdutoOpentech;

            produtoOpentech.ValorDe = Request.GetDecimalParam("ValorDe");
            produtoOpentech.ValorAte = Request.GetDecimalParam("ValorAte");
            produtoOpentech.CodigoEmbarcador = Request.GetIntParam("CodigoEmbarcador");

            await SetarEstados(produtoOpentech, unitOfWork, cancellationToken);
            await SetarLocalidades(produtoOpentech, unitOfWork, cancellationToken);
        }

        private async Task SetarEstados(Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork, cancellationToken);
            dynamic estados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Estados"));

            if (produtoOpentech.Estados == null)
                produtoOpentech.Estados = new List<Dominio.Entidades.Estado>();
            else
                produtoOpentech.Estados.Clear();

            foreach (var objEstado in estados)
            {
                Dominio.Entidades.Estado estado = await repEstado.BuscarPorSiglaAsync((string)objEstado.Codigo);
                produtoOpentech.Estados.Add(estado);
            }
        }

        private async Task SetarLocalidades(Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork, cancellationToken);
            dynamic localidadesJson = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Localidades"));

            produtoOpentech.Localidades ??= new List<Dominio.Entidades.Localidade>();
            produtoOpentech.Localidades.Clear();

            List<int> codigos = ((IEnumerable<dynamic>)localidadesJson)
                     .Select(l => (int)l.Codigo)
                     .ToList();

            List<Dominio.Entidades.Localidade> todasLocalidade = await repositorioLocalidade.BuscarPorCodigosAsync(codigos);
            Dictionary<int, Dominio.Entidades.Localidade> mapaLocalidades = todasLocalidade.ToDictionary(l => l.Codigo, l => l);

            foreach (var item in localidadesJson)
            {
                int codigo = (int)item.Codigo;
                if (mapaLocalidades.TryGetValue(codigo, out Dominio.Entidades.Localidade localidade))
                {
                    produtoOpentech.Localidades.Add(localidade);
                }
            }
        }

        private async Task<Models.Grid.Grid> ObterGridPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaProdutoOpentech filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                (dynamic lista, int totalRegistros) = await ExecutaPesquisa(filtrosPesquisa, parametrosConsulta, unitOfWork, cancellationToken);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaProdutoOpentech ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaProdutoOpentech filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaProdutoOpentech()
            {
                CodigoOperacao = Request.GetIntParam("TipoOperacao"),
                UfDestino = Request.GetStringParam("Estado"),
                CodigoApolice = Request.GetIntParam("Apolice"),
                Ativo = !string.IsNullOrEmpty(Request.Params("Status")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Status")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo,
                TipoCarga = Request.GetListParam<int>("TipoCarga")
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
