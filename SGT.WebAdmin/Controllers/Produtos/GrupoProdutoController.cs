using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Produtos
{
    public class GrupoProdutoController : BaseController
    {
        #region Construtores

        public GrupoProdutoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string codigoGrupoProdutoEmbarcador = Request.Params("CodigoGrupoProdutoEmbarcador");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                bool somenteChecklist = Request.GetBoolParam("SomenteChecklist");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "CodigoGrupoProdutoEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 70, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("NaoPermitirCarregamento", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
                int quantidade = repGrupoProduto.ContarConsulta(descricao, codigoGrupoProdutoEmbarcador, ativo, somenteChecklist);
                List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> produtos = quantidade > 0 ? repGrupoProduto.Consultar(descricao, codigoGrupoProdutoEmbarcador, ativo, somenteChecklist, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();
                grid.setarQuantidadeTotal(quantidade);

                var lista = (from p in produtos select new { p.Codigo, p.Descricao, p.CodigoGrupoProdutoEmbarcador, p.DescricaoAtivo, p.NaoPermitirCarregamento }).ToList();

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga repGrupoProdutoTipoCarga = new Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = new Dominio.Entidades.Embarcador.Produtos.GrupoProduto();
                grupoProduto.Ativo = bool.Parse(Request.Params("Ativo"));
                grupoProduto.Descricao = Request.Params("Descricao");
                grupoProduto.QuantidadePorCaixa = Request.GetIntParam("QuantidadePorCaixa");
                grupoProduto.PorcentagemCorrecao = Request.GetDecimalParam("PorcentagemCorrecao");
                grupoProduto.CodigoGrupoProdutoEmbarcador = Request.Params("CodigoGrupoProdutoEmbarcador");
                grupoProduto.ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio = Request.GetBoolParam("ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio");
                grupoProduto.ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio = Request.GetBoolParam("RetornarNoChecklist");
                grupoProduto.ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio = Request.GetBoolParam("NaoPermitirCarregamento");

                if (!string.IsNullOrEmpty(grupoProduto.CodigoGrupoProdutoEmbarcador))
                {
                    Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProdutoExistente = repGrupoProduto.BuscarPorCodigoEmbarcador(grupoProduto.CodigoGrupoProdutoEmbarcador);
                    if (grupoProdutoExistente != null)
                        throw new ControllerException(Localization.Resources.Produtos.GrupoProduto.JaExisteGrupoProdutoCadastradoParaCodigoInformado);
                }

                repGrupoProduto.Inserir(grupoProduto, Auditado);

                dynamic jTiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("TiposCarga"));
                foreach (var jTipoCarga in jTiposCarga)
                {
                    Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga grupoProdutoTipoCarga = new Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga();
                    grupoProdutoTipoCarga.Posicao = (int)jTipoCarga.Posicao;
                    grupoProdutoTipoCarga.TipoDeCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = (int)jTipoCarga.TipoCarga.Codigo };
                    grupoProdutoTipoCarga.GrupoProduto = grupoProduto;
                    repGrupoProdutoTipoCarga.Inserir(grupoProdutoTipoCarga);
                }

                SalvarProdutoOpenTech(grupoProduto, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
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

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga repGrupoProdutoTipoCarga = new Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                grupoProduto.Ativo = bool.Parse(Request.Params("Ativo"));
                grupoProduto.Descricao = Request.Params("Descricao");
                grupoProduto.QuantidadePorCaixa = Request.GetIntParam("QuantidadePorCaixa");
                grupoProduto.PorcentagemCorrecao = Request.GetDecimalParam("PorcentagemCorrecao");
                grupoProduto.CodigoGrupoProdutoEmbarcador = Request.Params("CodigoGrupoProdutoEmbarcador");
                grupoProduto.ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio = Request.GetBoolParam("ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio");
                grupoProduto.RetornarNoChecklist = Request.GetBoolParam("RetornarNoChecklist");
                grupoProduto.NaoPermitirCarregamento = Request.GetBoolParam("NaoPermitirCarregamento");

                if (!string.IsNullOrEmpty(grupoProduto.CodigoGrupoProdutoEmbarcador))
                {
                    Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProdutoExistente = repGrupoProduto.BuscarPorCodigoEmbarcador(grupoProduto.CodigoGrupoProdutoEmbarcador);
                    if (grupoProdutoExistente != null && grupoProdutoExistente.Codigo != grupoProduto.Codigo)
                        throw new ControllerException(Localization.Resources.Produtos.GrupoProduto.JaExisteGrupoProdutoCadastradoParaCodigoInformado);
                }

                List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga> grupoProdutoTiposCargaAtivos = new List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga>();

                dynamic jTiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposCarga"));
                foreach (var jTipoCarga in jTiposCarga)
                {

                    Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga grupoProdutoTipoCarga = repGrupoProdutoTipoCarga.ConsultarPorGrupoProdutoTipoCarga(grupoProduto.Codigo, (int)jTipoCarga.TipoCarga.Codigo);

                    if (grupoProdutoTipoCarga == null)
                    {
                        grupoProdutoTipoCarga = new Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga();
                        grupoProdutoTipoCarga.Posicao = (int)jTipoCarga.Posicao;
                        grupoProdutoTipoCarga.TipoDeCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = (int)jTipoCarga.TipoCarga.Codigo };
                        grupoProdutoTipoCarga.GrupoProduto = grupoProduto;
                        repGrupoProdutoTipoCarga.Inserir(grupoProdutoTipoCarga);
                    }
                    else
                    {
                        grupoProdutoTipoCarga.Posicao = (int)jTipoCarga.Posicao;
                        grupoProdutoTipoCarga.TipoDeCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = (int)jTipoCarga.TipoCarga.Codigo };
                        grupoProdutoTipoCarga.GrupoProduto = grupoProduto;
                        repGrupoProdutoTipoCarga.Atualizar(grupoProdutoTipoCarga);
                    }
                    grupoProdutoTiposCargaAtivos.Add(grupoProdutoTipoCarga);

                }

                List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga> grupoProdutoTipoCargaSalvosNoBanco = repGrupoProdutoTipoCarga.ConsultarPorGrupoProduto(grupoProduto.Codigo);
                foreach (Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga grupoProdutoTipoCargaSalvoNoBanco in grupoProdutoTipoCargaSalvosNoBanco)
                {
                    if (!grupoProdutoTiposCargaAtivos.Exists(obj => obj.Codigo == grupoProdutoTipoCargaSalvoNoBanco.Codigo))
                    {
                        repGrupoProdutoTipoCarga.Deletar(grupoProdutoTipoCargaSalvoNoBanco);
                    }
                }

                repGrupoProduto.Atualizar(grupoProduto, Auditado);

                SalvarProdutoOpenTech(grupoProduto, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));

                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga repGrupoProdutoTipoCarga = new Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProdutoOpenTech repGrupoProdutoOpenTech = new Repositorio.Embarcador.Produtos.GrupoProdutoOpenTech(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga> grupoProdutoTiposCarga = repGrupoProdutoTipoCarga.ConsultarPorGrupoProduto(grupoProduto.Codigo);

                Dominio.Entidades.Embarcador.Produtos.GrupoProdutoOpenTech grupoProdutoOpenTech = repGrupoProdutoOpenTech.BuscarPorGrupoProduto(grupoProduto.Codigo);

                var dynGrupoProduto = new
                {
                    grupoProduto.Ativo,
                    grupoProduto.Codigo,
                    grupoProduto.Descricao,
                    grupoProduto.QuantidadePorCaixa,
                    grupoProduto.PorcentagemCorrecao,
                    grupoProduto.ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio,
                    grupoProduto.RetornarNoChecklist,
                    grupoProduto.NaoPermitirCarregamento,
                    CodigoGrupoProdutoEmbarcador = grupoProduto.CodigoGrupoProdutoEmbarcador != null ? grupoProduto.CodigoGrupoProdutoEmbarcador : "",
                    TiposCarga = (from p in grupoProdutoTiposCarga
                                  select new
                                  {
                                      TipoCarga = new { p.TipoDeCarga.Descricao, p.TipoDeCarga.Codigo },
                                      Posicao = p.Posicao
                                  }).ToList(),
                    ProdutoValorMaiorOpenTech = grupoProdutoOpenTech?.CodigoProdutoValorMaior.ToString() ?? string.Empty,
                    ProdutoValorMenorOpenTech = grupoProdutoOpenTech?.CodigoProdutoValorMenor.ToString() ?? string.Empty
                };

                return new JsonpResult(dynGrupoProduto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
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

                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga repGrupoProdutoTipoCarga = new Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.GrupoProduto GrupoProduto = repGrupoProduto.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga> grupoProdutoTiposCarga = repGrupoProdutoTipoCarga.ConsultarPorGrupoProduto(grupoProduto.Codigo);
                foreach (Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga grupoProdutoTipoCarga in grupoProdutoTiposCarga)
                {
                    repGrupoProdutoTipoCarga.Deletar(grupoProdutoTipoCarga);
                }
                repGrupoProduto.Deletar(grupoProduto, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Produtos.GrupoProduto.NaoFoiPossivelExcluirPoisPossuiVinculo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarProdutosOpenTech()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string mensagemErro = string.Empty;

                var produtos = Servicos.Embarcador.Integracao.OpenTech.IntegracaoProdutoOpenTech.ObterProdutosOpenTech(unitOfWork, out mensagemErro);

                if (produtos.IsNullOrEmpty() && !string.IsNullOrWhiteSpace(mensagemErro))
                    return new JsonpResult(false, Localization.Resources.Produtos.GrupoProduto.OcorreuUmaFalhaAoObterProdutosOpenTech + ": " + mensagemErro);

                return new JsonpResult(produtos);
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoObterTiposIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarProdutoOpenTech(Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoProdutoValorMaior, codigoProdutoValorMenor;
            int.TryParse(Request.Params("ProdutoValorMaiorOpenTech"), out codigoProdutoValorMaior);
            int.TryParse(Request.Params("ProdutoValorMenorOpenTech"), out codigoProdutoValorMenor);

            Repositorio.Embarcador.Produtos.GrupoProdutoOpenTech repGrupoProdutoOpenTech = new Repositorio.Embarcador.Produtos.GrupoProdutoOpenTech(unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.GrupoProdutoOpenTech grupoProdutoOpenTech = repGrupoProdutoOpenTech.BuscarPorGrupoProduto(grupoProduto.Codigo);

            if (grupoProdutoOpenTech == null)
            {
                if (codigoProdutoValorMaior <= 0 && codigoProdutoValorMenor <= 0)
                    return;

                grupoProdutoOpenTech = new Dominio.Entidades.Embarcador.Produtos.GrupoProdutoOpenTech();
            }

            grupoProdutoOpenTech.GrupoProduto = grupoProduto;
            grupoProdutoOpenTech.CodigoProdutoValorMaior = codigoProdutoValorMaior;
            grupoProdutoOpenTech.CodigoProdutoValorMenor = codigoProdutoValorMenor;

            repGrupoProdutoOpenTech.Inserir(grupoProdutoOpenTech);
        }

        #endregion
    }
}
