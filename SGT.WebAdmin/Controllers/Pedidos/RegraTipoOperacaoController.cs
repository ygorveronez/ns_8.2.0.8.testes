using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/RegraTipoOperacao")]
    public class RegraTipoOperacaoController : BaseController
    {
		#region Construtores

		public RegraTipoOperacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork);
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
                Repositorio.Embarcador.Pedidos.RegraTipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.RegraTipoOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao tipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao();

                PreencherEntidade(tipoOperacao, unitOfWork);

                SalvarRegraTipoOperacao(tipoOperacao, unitOfWork);
                SalvarExpedidores(tipoOperacao, unitOfWork);
                SalvarRecebedores(tipoOperacao, unitOfWork);
                SalvarCanalVenda(tipoOperacao, unitOfWork);
                SalvarCanalEntrega(tipoOperacao, unitOfWork);
                SalvarDestinatarios(tipoOperacao, unitOfWork);
                SalvarTiposOperacao(tipoOperacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                unitOfWork.Start();
                Repositorio.Embarcador.Pedidos.RegraTipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.RegraTipoOperacao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigo, true);

                if (tipoOperacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(tipoOperacao, unitOfWork);

                SalvarRegraTipoOperacao(tipoOperacao, unitOfWork);
                SalvarExpedidores(tipoOperacao, unitOfWork);
                SalvarRecebedores(tipoOperacao, unitOfWork);
                SalvarCanalVenda(tipoOperacao, unitOfWork);
                SalvarCanalEntrega(tipoOperacao, unitOfWork);
                SalvarDestinatarios(tipoOperacao, unitOfWork);
                SalvarTiposOperacao(tipoOperacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Pedidos.RegraTipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.RegraTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoFilial repositorioRegraTipoOperacaoFilial = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoFilial(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao regraTipoOperacao = repTipoOperacao.BuscarPorCodigo(codigo);

                if (regraTipoOperacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial> regraTipoOperacaoFiliais = repositorioRegraTipoOperacaoFilial.BuscarPorRegraTipoOperacao(regraTipoOperacao.Codigo);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial regraTipoOperacaoFilial in regraTipoOperacaoFiliais)
                    repositorioRegraTipoOperacaoFilial.Deletar(regraTipoOperacaoFilial, Auditado);

                repTipoOperacao.Deletar(regraTipoOperacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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

                Repositorio.Embarcador.Pedidos.RegraTipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.RegraTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoFilial repositorioRegraTipoOperacaoFilial = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoFilial(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoExpedidor repositorioRegraTipoOperacaoExpedidor = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoExpedidor(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoRecebedor repositorioRegraTipoOperacaoRecebedor = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoRecebedor(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda repositorioRegraTipoOperacaoCanalVenda = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega repositorioRegraTipoOperacaoCanalEntrega = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoDestinatario repositorioRegraTipoOperacaoDestinatario = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoDestinatario(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao repositorioRegraTipoOperacaoTiposOperacao = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao dadosTipoOperacao = repTipoOperacao.BuscarPorCodigo(codigo, true);

                if (dadosTipoOperacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial> regraTipoOperacaoFiliais = repositorioRegraTipoOperacaoFilial.BuscarPorRegraTipoOperacao(dadosTipoOperacao.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor> regraTipoOperacaoExpedidor = repositorioRegraTipoOperacaoExpedidor.BuscarPorRegraTipoOperacao(dadosTipoOperacao.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor> regraTipoOperacaoRecebedor = repositorioRegraTipoOperacaoRecebedor.BuscarPorRegraTipoOperacao(dadosTipoOperacao.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda> regraTipoOperacaoVendaCanal = repositorioRegraTipoOperacaoCanalVenda.BuscarPorRegraTipoOperacao(dadosTipoOperacao.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega> regraTipoOperacaoVendaEntrega = repositorioRegraTipoOperacaoCanalEntrega.BuscarPorRegraTipoOperacao(dadosTipoOperacao.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario> regraTipoOperacaoDestinatario = repositorioRegraTipoOperacaoDestinatario.BuscarPorRegraTipoOperacao(dadosTipoOperacao.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao> regraTipoOperacaoTiposOperacao = repositorioRegraTipoOperacaoTiposOperacao.BuscarPorRegraTipoOperacao(dadosTipoOperacao.Codigo);

                dynamic dynPedido = new
                {
                    MultiplaEtapa = dadosTipoOperacao.QuantidadeEtapas,
                    TipoDocumentoTransporte = new { Codigo = dadosTipoOperacao.TipoDocumentoTransporte?.Codigo ?? 0, Descricao = dadosTipoOperacao.TipoDocumentoTransporte?.Descricao ?? string.Empty },
                    CategoriaCliente = new { Codigo = dadosTipoOperacao.Categoria?.Codigo ?? 0, Descricao = dadosTipoOperacao.Categoria?.Descricao ?? string.Empty },
                    TipoOperacao = new { Codigo = dadosTipoOperacao.TipoOperacao?.Codigo ?? 0, Descricao = dadosTipoOperacao.TipoOperacao?.Descricao ?? string.Empty },
                    TipoModal = dadosTipoOperacao.TipoModal,
                    Ativo = dadosTipoOperacao.Ativo,
                    dadosTipoOperacao.CteGlobalizado,
                    Filiais = (from obj in regraTipoOperacaoFiliais
                               select new
                               {
                                   Codigo = obj.Filial.Codigo,
                                   Descricao = obj.Filial.Descricao,
                               }).ToList(),

                    Expedidores = (from obj in regraTipoOperacaoExpedidor
                                   select new
                                   {
                                       Codigo = obj.Expedidor.Codigo,
                                       Descricao = $"{obj.Expedidor.Descricao} ({obj.Expedidor.CodigoIntegracao})",
                                   }).ToList(),

                    Recebedores = (from obj in regraTipoOperacaoRecebedor
                                   select new
                                   {
                                       Codigo = obj.Recebedor.Codigo,
                                       Descricao = $"{obj.Recebedor.Descricao} ({obj.Recebedor.CodigoIntegracao})",
                                   }).ToList(),

                    CanalVenda = (from obj in regraTipoOperacaoVendaCanal
                                  select new
                                  {
                                      Codigo = obj.CanalVenda.Codigo,
                                      Descricao = $"{obj.CanalVenda.Descricao} ({obj.CanalVenda.CodigoIntegracao})",
                                  }).ToList(),

                    CanalEntrega = (from obj in regraTipoOperacaoVendaEntrega
                                    select new
                                    {
                                        Codigo = obj.CanalEntrega.Codigo,
                                        Descricao = $"{obj.CanalEntrega.Descricao} ({obj.CanalEntrega.CodigoIntegracao})",
                                    }).ToList(),
                    Destinatario = (from obj in regraTipoOperacaoDestinatario
                                    select new
                                    {
                                        Codigo = obj.Destinatario.Codigo,
                                        Descricao = $"{obj.Destinatario.Descricao} ({obj.Destinatario.CodigoIntegracao})",
                                    }).ToList(),
                    TiposOperacao = (from obj in regraTipoOperacaoTiposOperacao
                                     select new
                               {
                                   Codigo = obj.TipoOperacao.Codigo,
                                   Descricao = obj.TipoOperacao.Descricao,
                               }).ToList(),
                };
                return new JsonpResult(dynPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unidadeTrabalho, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Metódos Privados

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unidadeTrabalho, bool exportacao = false)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.RegraTipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.RegraTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoFilial repositorioTipoOperacaoFilial = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoFilial(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoExpedidor repositorioTipoOperacaoExpedidor = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoExpedidor(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoRecebedor repositorioTipoOperacaoRecebedor = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoRecebedor(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda repositorioTipoOperacaoCanalVenda = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega repositorioTipoOperacaoCanalEntrega = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraTipoOperacaoDestinatario repositorioTipoOperacaoDestinatario = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoDestinatario(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraTipoOperacao filtrosPesquisa = ObterFiltrosPesquisaRegraTipoOperacao();
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial> filiais = repositorioTipoOperacaoFilial.BuscarTodos();
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor> expedidores = repositorioTipoOperacaoExpedidor.BuscarTodos();
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor> recebedores = repositorioTipoOperacaoRecebedor.BuscarTodos();
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda> canaisVenda = repositorioTipoOperacaoCanalVenda.BuscarTodos();
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega> canaisEntrega = repositorioTipoOperacaoCanalEntrega.BuscarTodos();
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario> detinatarios = repositorioTipoOperacaoDestinatario.BuscarTodos();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Tipo Documento Transporte", "TipoDocumentoTransporte", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Categoria Pessoa", "Categoria", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Múltiplas Etapas", "QuantidadeEtapas", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Tipo Modal", "TipoModal", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("CT-e Globalizado", "CTeGlobalizado", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Filiais", "Filiais", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Expedidores", "Expedidores", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Recebedores", "Recebedores", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Canal Venda", "CanalVenda", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Canal Entrega", "CanalEntrega", 10, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioTipoOperacao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao> listaTipoOperacao = totalRegistros > 0 ? repositorioTipoOperacao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao>();

                grid.AdicionaRows((
                    from o in listaTipoOperacao
                    select new
                    {
                        o.Codigo,
                        Categoria = o.Categoria?.Descricao ?? string.Empty,
                        QuantidadeEtapas = o?.QuantidadeEtapas.ObterDescricao() ?? "Não",
                        TipoDocumentoTransporte = o.TipoDocumentoTransporte?.Descricao ?? string.Empty,
                        TipoOperacao = o.TipoOperacao?.Descricao ?? string.Empty,
                        TipoModal = o.TipoModal.ObterDescricao(),
                        CTeGlobalizado = o.CteGlobalizado.ObterDescricao(),
                        Filiais = string.Join(", ", from obj in filiais where o.Codigo == obj.RegraTipoOperacao.Codigo select obj.Filial.Descricao),
                        Expedidores = string.Join(", ", from obj in expedidores where o.Codigo == obj.RegraTipoOperacao.Codigo select obj.Expedidor.Descricao),
                        Recebedores = string.Join(", ", from obj in recebedores where o.Codigo == obj.RegraTipoOperacao.Codigo select obj.Recebedor.Descricao),
                        CanalVenda = string.Join(", ", from obj in canaisVenda where o.Codigo == obj.RegraTipoOperacao.Codigo select obj.CanalVenda.Descricao),
                        CanalEntrega = string.Join(", ", from obj in canaisEntrega where o.Codigo == obj.RegraTipoOperacao.Codigo select obj.CanalEntrega.Descricao),
                        Detinatarios = string.Join(", ", from obj in detinatarios where o.Codigo == obj.RegraTipoOperacao.Codigo select obj.Destinatario.Descricao),
                    }).ToList()
                );

                grid.setarQuantidadeTotal(totalRegistros);

                if (exportacao)
                {
                    byte[] bArquivo = grid.GerarExcel();

                    if (bArquivo != null)
                        return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                    else
                        return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
                }
                else
                {
                    return new JsonpResult(grid);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraTipoOperacao ObterFiltrosPesquisaRegraTipoOperacao()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraTipoOperacao()
            {
                CodigoTipoDocumentoTransporte = Request.GetIntParam("TipoDocumentoTransporte"),
                QuantidadeEtapas = Request.GetNullableEnumParam<SimNao>("MultiplaEtapa"),
                CodigoCategoriaPessoa = Request.GetIntParam("CategoriaCliente"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                Ativo = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao regraTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte repositorioTipoDocumentoTransporte = new Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte(unitOfWork);
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repositorioCategoriaPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
            Repositorio.Embarcador.Pedidos.RegraTipoOperacao repRegraTipoOperacao = new Repositorio.Embarcador.Pedidos.RegraTipoOperacao(unitOfWork);

            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoTipoDocumentoTransporte = Request.GetIntParam("TipoDocumentoTransporte");
            int codigoCategoriaPessoa = Request.GetIntParam("CategoriaCliente");

            regraTipoOperacao.TipoOperacao = codigoTipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : throw new ControllerException("Tipo de Operação não encontrado.");
            regraTipoOperacao.Categoria = codigoCategoriaPessoa > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(codigoCategoriaPessoa) : null;
            regraTipoOperacao.TipoDocumentoTransporte = codigoTipoDocumentoTransporte > 0 ? repositorioTipoDocumentoTransporte.BuscarPorCodigo(codigoTipoDocumentoTransporte) : null;
            regraTipoOperacao.TipoModal = Request.GetEnumParam<TipoModal>("TipoModal");
            regraTipoOperacao.CteGlobalizado = Request.GetBoolParam("CteGlobalizado");
            regraTipoOperacao.Ativo = Request.GetBoolParam("Ativo");
            regraTipoOperacao.QuantidadeEtapas = Request.GetEnumParam<SimNao>("MultiplaEtapa");

            if (repRegraTipoOperacao.ExisteRegraDuplicada(regraTipoOperacao))
                throw new ControllerException("Já existe uma regra cadastrada com os mesmos dados.");

            if (regraTipoOperacao.Codigo > 0)
                repRegraTipoOperacao.Atualizar(regraTipoOperacao);
            else
                repRegraTipoOperacao.Inserir(regraTipoOperacao);
        }

        private void SalvarRegraTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao regraTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.RegraTipoOperacaoFilial repositorioRegraTipoOperacaoFilial = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoFilial(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            dynamic dynRegraTipoOperacaoFilial = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

            if (dynRegraTipoOperacaoFilial == null)
                return;

            List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial> filiais = repositorioRegraTipoOperacaoFilial.BuscarPorRegraTipoOperacao(regraTipoOperacao.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (filiais.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic regraTipoOperacaoFilial in dynRegraTipoOperacaoFilial)
                {
                    int codigo = ((string)regraTipoOperacaoFilial.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial> listaDeletar = (from obj in filiais where !codigos.Contains(obj.Filial.Codigo) select obj).ToList();

                foreach (var deletar in listaDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Filial",
                        De = $"{deletar.Filial.Descricao}",
                        Para = ""
                    });

                    repositorioRegraTipoOperacaoFilial.Deletar(deletar);
                }
            }

            foreach (dynamic regraTipoOperacaoFilial in dynRegraTipoOperacaoFilial)
            {
                int codigo = ((string)regraTipoOperacaoFilial.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial tipoRegraTipoOperacao = codigo > 0 ? repositorioRegraTipoOperacaoFilial.BuscarPorRegraTipoOperacaoEFilial(regraTipoOperacao.Codigo, codigo) : null;

                if (tipoRegraTipoOperacao == null)
                    tipoRegraTipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial();

                int codigoFilial = ((string)regraTipoOperacaoFilial.Codigo).ToInt();

                tipoRegraTipoOperacao.RegraTipoOperacao = regraTipoOperacao;
                tipoRegraTipoOperacao.Filial = repositorioFilial.BuscarPorCodigo(codigoFilial);

                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Filial",
                    De = "",
                    Para = $"{tipoRegraTipoOperacao.Filial.Descricao}"
                });

                if (tipoRegraTipoOperacao.Codigo > 0)
                    repositorioRegraTipoOperacaoFilial.Atualizar(tipoRegraTipoOperacao);
                else
                    repositorioRegraTipoOperacaoFilial.Inserir(tipoRegraTipoOperacao);
            }

            regraTipoOperacao.SetExternalChanges(alteracoes);
        }

        private void SalvarExpedidores(Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao regraTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.RegraTipoOperacaoExpedidor repositorioRegraTipoOperacaoExpedidor = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoExpedidor(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            dynamic expedidores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Expedidores"));

            List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor> listaExpedidores = repositorioRegraTipoOperacaoExpedidor.BuscarPorRegraTipoOperacao(regraTipoOperacao.Codigo);

            if (listaExpedidores != null && listaExpedidores.Count > 0)
            {
                List<long> codigos = new List<long>();

                foreach (dynamic expedidor in expedidores)
                    if (expedidor.Codigo != null)
                        codigos.Add((long)expedidor.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor> listaExpedoresRemover = listaExpedidores.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < listaExpedoresRemover.Count; i++)
                    repositorioRegraTipoOperacaoExpedidor.Deletar(listaExpedoresRemover[i]);
            }
            else
                listaExpedidores = new List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor>();

            foreach (dynamic expedidor in expedidores)
            {
                Dominio.Entidades.Cliente clienteExpedidor = repositorioCliente.BuscarPorCPFCNPJ((double)expedidor?.Codigo);

                if (clienteExpedidor == null)
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor existeExpedidor = repositorioRegraTipoOperacaoExpedidor.BuscarPorRegraTipoOperacaoEExpedidor(regraTipoOperacao.Codigo, (long)expedidor?.Codigo);

                if (existeExpedidor == null)
                    existeExpedidor = new Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor();

                existeExpedidor.RegraTipoOperacao = regraTipoOperacao;
                existeExpedidor.Expedidor = clienteExpedidor;

                if (existeExpedidor.Codigo > 0)
                    repositorioRegraTipoOperacaoExpedidor.Atualizar(existeExpedidor);
                else
                    repositorioRegraTipoOperacaoExpedidor.Inserir(existeExpedidor);
            }
        }

        private void SalvarRecebedores(Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao regraTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.RegraTipoOperacaoRecebedor repositorioRegraTipoOperacaoRecebedor = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoRecebedor(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            dynamic recebedores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Recebedores"));

            List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor> listaRecebedores = repositorioRegraTipoOperacaoRecebedor.BuscarPorRegraTipoOperacao(regraTipoOperacao.Codigo);

            if (listaRecebedores != null && listaRecebedores.Count > 0)
            {
                List<long> codigos = new List<long>();

                foreach (dynamic recebedor in recebedores)
                    if (recebedor.Codigo != null)
                        codigos.Add((long)recebedor.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor> listaRecebedoresRemover = listaRecebedores.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < listaRecebedoresRemover.Count; i++)
                    repositorioRegraTipoOperacaoRecebedor.Deletar(listaRecebedoresRemover[i]);
            }
            else
                listaRecebedores = new List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor>();

            foreach (dynamic recebedor in recebedores)
            {
                Dominio.Entidades.Cliente clienteRecebedor = repositorioCliente.BuscarPorCPFCNPJ((double)recebedor?.Codigo);

                if (clienteRecebedor == null)
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor existeRecebedor = repositorioRegraTipoOperacaoRecebedor.BuscarPorRegraTipoOperacaoERecebedor(regraTipoOperacao.Codigo, (long)recebedor?.Codigo);

                if (existeRecebedor == null)
                    existeRecebedor = new Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor();

                existeRecebedor.RegraTipoOperacao = regraTipoOperacao;
                existeRecebedor.Recebedor = clienteRecebedor;

                if (existeRecebedor.Codigo > 0)
                    repositorioRegraTipoOperacaoRecebedor.Atualizar(existeRecebedor);
                else
                    repositorioRegraTipoOperacaoRecebedor.Inserir(existeRecebedor);
            }
        }

        private void SalvarCanalVenda(Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao regraTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda repositorioRegraTipoOperacaoCanalVenda = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda(unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);

            dynamic canaisVenda = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CanalVenda"));

            List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda> listaCanalVenda = repositorioRegraTipoOperacaoCanalVenda.BuscarPorRegraTipoOperacao(regraTipoOperacao.Codigo);

            if (listaCanalVenda != null && listaCanalVenda.Count > 0)
            {
                List<long> codigos = new List<long>();

                foreach (dynamic canalVenda in canaisVenda)
                    if (canalVenda.Codigo != null)
                        codigos.Add((long)canalVenda.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda> listaCanalVendaRemover = listaCanalVenda.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < listaCanalVendaRemover.Count; i++)
                    repositorioRegraTipoOperacaoCanalVenda.Deletar(listaCanalVendaRemover[i]);
            }
            else
                listaCanalVenda = new List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda>();

            foreach (dynamic canalVenda in canaisVenda)
            {
                Dominio.Entidades.Embarcador.Pedidos.CanalVenda pedidoCanalVenda = repositorioCanalVenda.BuscarPorCodigo((int)canalVenda?.Codigo);

                if (pedidoCanalVenda == null)
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda existeCanalVenda = repositorioRegraTipoOperacaoCanalVenda.BuscarPorRegraTipoOperacaoECanalVenda(regraTipoOperacao.Codigo, (int)canalVenda?.Codigo);

                if (existeCanalVenda == null)
                    existeCanalVenda = new Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda();

                existeCanalVenda.RegraTipoOperacao = regraTipoOperacao;
                existeCanalVenda.CanalVenda = pedidoCanalVenda;

                if (existeCanalVenda.Codigo > 0)
                    repositorioRegraTipoOperacaoCanalVenda.Atualizar(existeCanalVenda);
                else
                    repositorioRegraTipoOperacaoCanalVenda.Inserir(existeCanalVenda);

            }
        }

        private void SalvarCanalEntrega(Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao regraTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega repositorioRegraTipoOperacaoCanalEntrega = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);

            dynamic canaisEntrega = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CanalEntrega"));

            List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega> listaCanalEntrega = repositorioRegraTipoOperacaoCanalEntrega.BuscarPorRegraTipoOperacao(regraTipoOperacao.Codigo);

            if (listaCanalEntrega != null && listaCanalEntrega.Count > 0)
            {
                List<long> codigos = new List<long>();

                foreach (dynamic canalEntrega in canaisEntrega)
                    if (canalEntrega.Codigo != null)
                        codigos.Add((long)canalEntrega.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega> listaCanalEntregaRemover = listaCanalEntrega.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < listaCanalEntregaRemover.Count; i++)
                    repositorioRegraTipoOperacaoCanalEntrega.Deletar(listaCanalEntregaRemover[i]);
            }
            else
                listaCanalEntrega = new List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega>();

            foreach (dynamic canalEntrega in canaisEntrega)
            {
                Dominio.Entidades.Embarcador.Pedidos.CanalEntrega pedidoCanalEntrega = repositorioCanalEntrega.BuscarPorCodigo((int)canalEntrega?.Codigo);

                if (pedidoCanalEntrega == null)
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega existeCanalEntrega = repositorioRegraTipoOperacaoCanalEntrega.BuscarPorRegraTipoOperacaoECanalEntrega(regraTipoOperacao.Codigo, (int)canalEntrega?.Codigo);

                if (existeCanalEntrega == null)
                    existeCanalEntrega = new Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega();

                existeCanalEntrega.RegraTipoOperacao = regraTipoOperacao;
                existeCanalEntrega.CanalEntrega = pedidoCanalEntrega;

                if (existeCanalEntrega.Codigo > 0)
                    repositorioRegraTipoOperacaoCanalEntrega.Atualizar(existeCanalEntrega);
                else
                    repositorioRegraTipoOperacaoCanalEntrega.Inserir(existeCanalEntrega);

            }
        }

        private void SalvarDestinatarios(Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao regraTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.RegraTipoOperacaoDestinatario repositorioRegraTipoOperacaoDestinatario = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoDestinatario(unitOfWork);
            Repositorio.Cliente repositorioDestinatario = new Repositorio.Cliente(unitOfWork);

            dynamic destinatarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinatario"));

            List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario> listaDestinatario = repositorioRegraTipoOperacaoDestinatario.BuscarPorRegraTipoOperacao(regraTipoOperacao.Codigo);

            if (listaDestinatario != null && listaDestinatario.Count > 0)
            {
                List<long> codigos = new List<long>();

                foreach (dynamic destinatario in destinatarios)
                    if (destinatario.Codigo != null)
                        codigos.Add((long)destinatario.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario> listaDestinatarioRemover = listaDestinatario.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < listaDestinatarioRemover.Count; i++)
                    repositorioRegraTipoOperacaoDestinatario.Deletar(listaDestinatarioRemover[i]);
            }
            else
                listaDestinatario = new List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario>();

            foreach (dynamic destinatario in destinatarios)
            {
                Dominio.Entidades.Cliente existeDestinatario = repositorioDestinatario.BuscarPorCPFCNPJ((double)destinatario?.Codigo);

                if (existeDestinatario == null)
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario existeRegraDestinatario = repositorioRegraTipoOperacaoDestinatario.BuscarPorRegraTipoOperacaoEDestinatario(regraTipoOperacao.Codigo, (double)destinatario?.Codigo);

                if (existeRegraDestinatario == null)
                    existeRegraDestinatario = new Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario();

                existeRegraDestinatario.RegraTipoOperacao = regraTipoOperacao;
                existeRegraDestinatario.Destinatario = existeDestinatario;

                if (existeRegraDestinatario.Codigo > 0)
                    repositorioRegraTipoOperacaoDestinatario.Atualizar(existeRegraDestinatario);
                else
                    repositorioRegraTipoOperacaoDestinatario.Inserir(existeRegraDestinatario);

            }
        }

        private void SalvarTiposOperacao(Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao regraTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao repositorioRegraTipoOperacaoTiposOperacao = new Repositorio.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));

            List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao> listaTiposOperacao = repositorioRegraTipoOperacaoTiposOperacao.BuscarPorRegraTipoOperacao(regraTipoOperacao.Codigo);

            if (listaTiposOperacao != null && listaTiposOperacao.Count > 0)
            {
                List<long> codigos = new List<long>();

                foreach (dynamic tipoOperacao in tiposOperacao)
                    if (tipoOperacao.Codigo != null)
                        codigos.Add((long)tipoOperacao.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao> listaTiposOperacaoRemover = listaTiposOperacao.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < listaTiposOperacaoRemover.Count; i++)
                    repositorioRegraTipoOperacaoTiposOperacao.Deletar(listaTiposOperacaoRemover[i]);
            }
            else
                listaTiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao>();

            foreach (dynamic tipoOperacao in tiposOperacao)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao pedidoTipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)tipoOperacao?.Codigo);

                if (pedidoTipoOperacao == null)
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao existeTipoOperacao = repositorioRegraTipoOperacaoTiposOperacao.BuscarPorRegraTipoOperacaoETipoOperacao(regraTipoOperacao.Codigo, (int)tipoOperacao?.Codigo);

                if (existeTipoOperacao == null)
                    existeTipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao();

                existeTipoOperacao.RegraTipoOperacao = regraTipoOperacao;
                existeTipoOperacao.TipoOperacao = pedidoTipoOperacao;

                if (existeTipoOperacao.Codigo > 0)
                    repositorioRegraTipoOperacaoTiposOperacao.Atualizar(existeTipoOperacao);
                else
                    repositorioRegraTipoOperacaoTiposOperacao.Inserir(existeTipoOperacao);
            }
        }

        #endregion
    }
}
