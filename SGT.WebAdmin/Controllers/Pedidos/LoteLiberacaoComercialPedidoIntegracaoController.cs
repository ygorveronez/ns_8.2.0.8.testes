using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/LoteLiberacaoComercialPedido")]
    public class LoteLiberacaoComercialPedidoIntegracaoController : BaseController
    {
		#region Construtores

		public LoteLiberacaoComercialPedidoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repositorioLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = repositorioLoteLiberacaoComercialPedidoIntegracao.BuscarArquivosPorIntegracao(codigo, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repositorioLoteLiberacaoComercialPedidoIntegracao.ContarBuscarArquivosPorIntegracao(codigo));

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Integrar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repositorioLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido repositorioLoteLiberacaoComercialPedido = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.Buscar();
                Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao integracao = repositorioLoteLiberacaoComercialPedidoIntegracao.BuscarPorCodigo(codigo, false);


                if (integracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (integracao.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Não é possível integrar nessa situação!");

                Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido loteLiberacaoComercialPedido = repositorioLoteLiberacaoComercialPedidoIntegracao.BuscarPorLote(integracao.LoteLiberacaoComercialPedido.Codigo).LoteLiberacaoComercialPedido;

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.ProblemaIntegracao = string.Empty;
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.LoteLiberacaoComercialPedido.Situacao = SituacaoLoteLiberacaoComercialPedido.EmIntegracao;

                repositorioLoteLiberacaoComercialPedidoIntegracao.Atualizar(integracao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.LoteLiberacaoComercialPedido, "Solicitou o reenvio da integração", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                unitOfWork.Rollback();

                return new JsonpResult(false, "Ocorreu Uma Falha Ao Adicionar Integracao");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

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
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Integracao", "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("DataDoEnvio", "DataIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situacao", "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Mensagem, "Retorno", 30, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repositorioLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao> listaIntegracoes = repositorioLoteLiberacaoComercialPedidoIntegracao.Consultar(codigo, situacao, parametrosConsulta);
                int totalIntegracoes = repositorioLoteLiberacaoComercialPedidoIntegracao.ContarConsulta(codigo, situacao);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repositorioLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repositorioLoteLiberacaoComercialPedidoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repositorioLoteLiberacaoComercialPedidoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repositorioLoteLiberacaoComercialPedidoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repositorioLoteLiberacaoComercialPedidoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoObterOsTotaisDasIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repositorioLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracao = repositorioLoteLiberacaoComercialPedidoIntegracao.BuscarArquivosPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracao.ArquivoRequisicao, integracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivo Integracao Lote Liberacao Comercial_" + integracao.Codigo + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoRealizarDownloadIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarcomIntegracaoFalha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao loteLiberacaoComercialPedidoIntegracao = repLoteLiberacaoComercialPedidoIntegracao.BuscarPorLote(Request.GetIntParam("Codigo"));

                if (loteLiberacaoComercialPedidoIntegracao == null)
                    throw new ControllerException("Lote Liberação Comercial Pedido Não econtrado.");

                if (loteLiberacaoComercialPedidoIntegracao.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Só é possível liberar quando a integração estiver com falha.");

                loteLiberacaoComercialPedidoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                loteLiberacaoComercialPedidoIntegracao.LoteLiberacaoComercialPedido.Situacao = SituacaoLoteLiberacaoComercialPedido.Finalizado;

                repLoteLiberacaoComercialPedidoIntegracao.Atualizar(loteLiberacaoComercialPedidoIntegracao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, loteLiberacaoComercialPedidoIntegracao, "Liberou integração com falha", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Falha ao liberar Integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
