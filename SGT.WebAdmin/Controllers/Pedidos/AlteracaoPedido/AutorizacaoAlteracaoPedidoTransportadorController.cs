using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos.AlteracaoPedido
{
    [CustomAuthorize("Pedidos/AutorizacaoAlteracaoPedidoTransportador")]
    public class AutorizacaoAlteracaoPedidoTransportadorController : BaseController
    {
		#region Construtores

		public AutorizacaoAlteracaoPedidoTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Aprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorioAprovacaoTransportador = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador aprovacaoTransportador = repositorioAprovacaoTransportador.BuscarPorAlteracaoPedidoETransportador(codigo, this.Usuario.Empresa.Codigo);

                if (aprovacaoTransportador == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                if (aprovacaoTransportador.Situacao != SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações.");

                if (!aprovacaoTransportador.IsPermitirAprovacaoOuReprovacao(this.Usuario.Empresa.Codigo))
                    return new JsonpResult(false, "Aprovação não permite alterações.");

                Aprovar(aprovacaoTransportador, unitOfWork);

                VerificarSituacaoAlteracaoPedido(aprovacaoTransportador.AlteracaoPedido, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a regra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplosItens()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorioAprovacaoTransportador = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(unitOfWork);
                List<int> codigosAlteracoesPedidos = ObterCodigosAlteracoesPedidosSelecionadas(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador> aprovacoesTransportadores = repositorioAprovacaoTransportador.BuscarPendentes(codigosAlteracoesPedidos, this.Usuario.Empresa.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador aprovacao in aprovacoesTransportadores)
                {
                    if (aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Empresa.Codigo))
                        Aprovar(aprovacao, unitOfWork);
                }

                IEnumerable<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido> alteracoesPedidosAprovadas = (from aprovacao in aprovacoesTransportadores select aprovacao.AlteracaoPedido).Distinct();

                foreach (Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido in alteracoesPedidosAprovadas)
                    VerificarSituacaoAlteracaoPedido(alteracaoPedido, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RegrasModificadas = aprovacoesTransportadores.Count()
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as regras.");
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
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorioAprovacaoTransportador = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador aprovacaoTransportador = repositorioAprovacaoTransportador.BuscarPorAlteracaoPedidoETransportador(codigo, this.Usuario.Empresa.Codigo);

                if (aprovacaoTransportador == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido = aprovacaoTransportador.AlteracaoPedido;
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorPedido(alteracaoPedido.Pedido.Codigo);

                return new JsonpResult(new
                {
                    alteracaoPedido.Codigo,
                    alteracaoPedido.Pedido.NumeroPedidoEmbarcador,
                    Filial = alteracaoPedido.Pedido.Filial?.Descricao,
                    SituacaoAlteracaoPedido = alteracaoPedido.Situacao,
                    TipoCarga = alteracaoPedido.Pedido.TipoDeCarga?.Descricao,
                    TipoOperacao = alteracaoPedido.Pedido.TipoOperacao?.Descricao,
                    Cargas = (cargas.Count > 0) ? string.Join(",", (from o in cargas select o.CodigoCargaEmbarcador)) : "",
                    AprovacaoPendente = aprovacaoTransportador.Situacao == SituacaoAlcadaRegra.Pendente
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
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

        public async Task<IActionResult> Reprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorioAprovacaoTransportador = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador aprovacaoTransportador = repositorioAprovacaoTransportador.BuscarPorAlteracaoPedidoETransportador(codigo, this.Usuario.Empresa.Codigo);

                if (!aprovacaoTransportador.IsPermitirAprovacaoOuReprovacao(this.Usuario.Empresa.Codigo))
                    return new JsonpResult(false, "Aprovação não permite alterações.");

                unitOfWork.Start();

                PreencherDadosRejeicaoAprovacaoTransportador(aprovacaoTransportador, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacaoTransportador.AlteracaoPedido, $"Transportador {Auditado.Usuario.Empresa.Descricao} reprovou a alteração do pedido. Motivo: {aprovacaoTransportador.Motivo}", unitOfWork);

                repositorioAprovacaoTransportador.Atualizar(aprovacaoTransportador);

                VerificarSituacaoAlteracaoPedido(aprovacaoTransportador.AlteracaoPedido, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarMultiplosItens()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorioAprovacaoTransportador = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(unitOfWork);
                List<int> codigosAlteracoesPedidos = ObterCodigosAlteracoesPedidosSelecionadas(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador> aprovacoesTransportadores = repositorioAprovacaoTransportador.BuscarPendentes(codigosAlteracoesPedidos, this.Usuario.Empresa.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador aprovacao in aprovacoesTransportadores)
                {
                    if (aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Empresa.Codigo))
                    {
                        PreencherDadosRejeicaoAprovacaoTransportador(aprovacao, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacao.AlteracaoPedido, null, $"Transportador {Auditado.Usuario.Empresa.Descricao} reprovou a alteração do pedido. Motivo: {aprovacao.Motivo}", unitOfWork);

                        repositorioAprovacaoTransportador.Atualizar(aprovacao);
                    }
                }

                IEnumerable<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido> alteracoesPedidosReprovadas = (from aprovacao in aprovacoesTransportadores select aprovacao.AlteracaoPedido).Distinct();

                foreach (Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedidoReprovada in alteracoesPedidosReprovadas)
                    VerificarSituacaoAlteracaoPedido(alteracaoPedidoReprovada, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RegrasModificadas = aprovacoesTransportadores.Count()
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar múltiplos registros.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void Aprovar(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador aprovacaoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorioAprovacaoTransportador = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(unitOfWork);

            aprovacaoTransportador.Data = DateTime.Now;
            aprovacaoTransportador.Situacao = SituacaoAlcadaRegra.Aprovada;

            repositorioAprovacaoTransportador.Atualizar(aprovacaoTransportador);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacaoTransportador.AlteracaoPedido, $"Transportador {Auditado.Usuario.Empresa.Descricao} aprovou a alteração do pedido", unitOfWork);
        }

        private List<int> ObterCodigosAlteracoesPedidosSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido> alteracoesPedidos;
            bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacaoTransportador filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorioAprovacaoTransportador = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(unitOfWork);

                alteracoesPedidos = repositorioAprovacaoTransportador.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    alteracoesPedidos.Remove(new Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorioAlteracaoPedido = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                alteracoesPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    alteracoesPedidos.Add(repositorioAlteracaoPedido.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return (from alteracaoPedido in alteracoesPedidos select alteracaoPedido.Codigo).ToList();
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacaoTransportador ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacaoTransportador()
            {
                CodigoTransportador = Usuario.Empresa.Codigo,
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                SituacaoAprovacao = Request.GetNullableEnumParam<SituacaoAlcadaRegra>("SituacaoAprovacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número do Pedido", propriedade: "NumeroPedidoEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tipo de Carga", propriedade: "TipoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Operação", propriedade: "TipoOperacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação Alteração Pedido", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacaoTransportador filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorio = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido> alteracoesPedidos = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido>();

                var lista = (
                    from alteracaoPedido in alteracoesPedidos
                    select new
                    {
                        alteracaoPedido.Codigo,
                        alteracaoPedido.Pedido.NumeroPedidoEmbarcador,
                        Filial = alteracaoPedido.Pedido.Filial?.Descricao,
                        Situacao = alteracaoPedido.Situacao.ObterDescricao(),
                        TipoCarga = alteracaoPedido.Pedido.TipoDeCarga?.Descricao,
                        TipoOperacao = alteracaoPedido.Pedido.TipoOperacao?.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Filial")
                return "Pedido.Filial.Descricao";

            if (propriedadeOrdenar == "TipoCarga")
                return "Pedido.TipoDeCarga.Descricao";

            if (propriedadeOrdenar == "TipoOperacao")
                return "Pedido.TipoOperacao.Descricao";

            return propriedadeOrdenar;
        }

        private SituacaoRegrasAutorizacao ObterSituacaoAutorizacao(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorioAprovacaoTransportador = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(unitOfWork);
            int reprovacoes = repositorioAprovacaoTransportador.ContarReprovacoes(alteracaoPedido.Codigo);

            if (reprovacoes > 0)
                return SituacaoRegrasAutorizacao.Reprovadas;

            int pendentes = repositorioAprovacaoTransportador.ContarPendentes(alteracaoPedido.Codigo);

            if (pendentes > 0)
                return SituacaoRegrasAutorizacao.Aguardando;

            return SituacaoRegrasAutorizacao.Aprovadas;
        }

        private void PreencherDadosRejeicaoAprovacaoTransportador(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador aprovacaoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoMotivoRejeicao = Request.GetIntParam("Motivo");
            Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido repositorioMotivoRejeicao = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido motivoRejeicao = repositorioMotivoRejeicao.BuscarPorCodigo(codigoMotivoRejeicao, auditavel: false) ?? throw new ControllerException("Motivo é obrigatório.");

            aprovacaoTransportador.Data = DateTime.Now;
            aprovacaoTransportador.Situacao = SituacaoAlcadaRegra.Rejeitada;
            aprovacaoTransportador.Motivo = motivoRejeicao.Descricao;
            aprovacaoTransportador.AlteracaoPedido.MotivoRejeicao = motivoRejeicao;
        }

        private void VerificarSituacaoAlteracaoPedido(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (alteracaoPedido.Situacao != SituacaoAlteracaoPedido.AguardandoAprovacaoTransportador)
                return;

            SituacaoRegrasAutorizacao situacaoAutorizacao = ObterSituacaoAutorizacao(alteracaoPedido, unitOfWork);

            if (situacaoAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            if (situacaoAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                alteracaoPedido.Situacao = SituacaoAlteracaoPedido.Aprovada;

                Servicos.Embarcador.Pedido.AlteracaoPedido servicoAlteracaoPedido = new Servicos.Embarcador.Pedido.AlteracaoPedido(unitOfWork);

                servicoAlteracaoPedido.Aplicar(alteracaoPedido, TipoServicoMultisoftware);
            }
            else
                alteracaoPedido.Situacao = SituacaoAlteracaoPedido.Reprovada;

            Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorioAlteracaoPedido = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(unitOfWork);

            repositorioAlteracaoPedido.Atualizar(alteracaoPedido);
        }

        #endregion
    }
}
