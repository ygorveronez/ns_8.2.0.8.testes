using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoEntregas
{
    [CustomAuthorize("GestaoEntregas/ConfiguracaoPortalCliente")]
    public class ConfiguracaoPortalClienteController : BaseController
    {
		#region Construtores

		public ConfiguracaoPortalClienteController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao = ObterEntidade(unitOfWork);

                if (configuracao == null)
                    return new JsonpResult(true);

                var retorno = new
                {
                    configuracao.ExibirMapa,
                    configuracao.ExibirDetalhesPedido,
                    configuracao.ExibirHistoricoPedido,
                    configuracao.ExibirDetalhesMotorista,
                    configuracao.ExibirDetalhesProduto,
                    configuracao.ExibirProduto,
                    configuracao.HabilitarAvaliacao,
                    configuracao.HabilitarPrevisaoEntrega,
                    configuracao.HabilitarObservacao,
                    configuracao.HabilitarNumeroPedidoCliente,
                    configuracao.HabilitarNumeroOrdemCompra,
                    configuracao.PesoBruto,
                    configuracao.PesoLiquido,
                    configuracao.QuantidadeVolumes,
                    configuracao.LinkAvaliacaoExterna,
                    configuracao.HabilitarAcessoPortalMultiCliFor,
                    configuracao.LinkAcessoPortalMultiCliFor,
                    configuracao.TipoAvaliacao,
                    configuracao.EnviarSMS,
                    configuracao.EnviarEmail,
                    configuracao.PermitirAdicionarAnexos,
                    configuracao.HabilitarVisualizacaoFotosPortal,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.OcorreuUmaFalhaAoObterConfiguracao);
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

                Repositorio.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente repConfiguracaoPortalCliente = new Repositorio.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao = ObterEntidade(unitOfWork);
                configuracao.Initialize();

                PreencheEntidade(ref configuracao);

                repConfiguracaoPortalCliente.Atualizar(configuracao, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.OcorreuUmaFalhaAoAtualizarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Perguntas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPerguntas();

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> AdicionarPergunta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao repPortalClientePerguntaAvaliacao = new Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao pergunta = new Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao();

                PreencheEntidadePergunta(ref pergunta, unitOfWork);

                if (!ValidaEntidadePergunta(pergunta, out string erro))
                    return new JsonpResult(false, true, erro);

                repPortalClientePerguntaAvaliacao.Inserir(pergunta, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.OcorreuUmaFalhaAoAdicionarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarPergunta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao repPortalClientePerguntaAvaliacao = new Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao pergunta = repPortalClientePerguntaAvaliacao.BuscarPorCodigo(codigo, true);

                if (pergunta == null)
                    return new JsonpResult(false, true, Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.ErroAoBuscarRegistro);

                PreencheEntidadePergunta(ref pergunta, unitOfWork);

                if (!ValidaEntidadePergunta(pergunta, out string erro))
                    return new JsonpResult(false, true, erro);

                repPortalClientePerguntaAvaliacao.Atualizar(pergunta, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.OcorreuUmaFalhaAoAtualizarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPergunta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao repPortalClientePerguntaAvaliacao = new Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao pergunta = repPortalClientePerguntaAvaliacao.BuscarPorCodigo(codigo, true);

                if (pergunta == null)
                    return new JsonpResult(false, true, Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.ErroAoBuscarRegistro);

                repPortalClientePerguntaAvaliacao.Deletar(pergunta, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.OcorreuUmaFalhaAoDeletarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        private Models.Grid.Grid GridPerguntas()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Ordem", false);
            grid.AdicionarCabecalho("Conteudo", false);
            grid.AdicionarCabecalho("Titulo", "Titulo", 50, Models.Grid.Align.left, true);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao repPortalClientePerguntaAvaliacao = new Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao> listaGrid = repPortalClientePerguntaAvaliacao.BuscarPerguntas(inicio, limite);
            totalRegistros = repPortalClientePerguntaAvaliacao.ContarBuscaPerguntas();

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Titulo,
                            obj.Conteudo,
                            obj.Ordem,
                        };

            return lista.ToList();
        }


        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao)
        {
            configuracao.ExibirMapa = Request.GetBoolParam("ExibirMapa");
            configuracao.ExibirDetalhesPedido = Request.GetBoolParam("ExibirDetalhesPedido");
            configuracao.ExibirHistoricoPedido = Request.GetBoolParam("ExibirHistoricoPedido");
            configuracao.ExibirDetalhesMotorista = Request.GetBoolParam("ExibirDetalhesMotorista");
            configuracao.ExibirDetalhesProduto = Request.GetBoolParam("ExibirDetalhesProduto");
            configuracao.ExibirProduto = Request.GetBoolParam("ExibirProduto");
            configuracao.HabilitarAvaliacao = Request.GetBoolParam("HabilitarAvaliacao");
            configuracao.HabilitarPrevisaoEntrega = Request.GetBoolParam("HabilitarPrevisaoEntrega");
            configuracao.HabilitarObservacao = Request.GetBoolParam("HabilitarObservacao");
            configuracao.HabilitarNumeroPedidoCliente = Request.GetBoolParam("HabilitarNumeroPedidoCliente");
            configuracao.HabilitarNumeroOrdemCompra = Request.GetBoolParam("HabilitarNumeroOrdemCompra");
            configuracao.PesoBruto = Request.GetBoolParam("PesoBruto");
            configuracao.PesoLiquido = Request.GetBoolParam("PesoLiquido");
            configuracao.QuantidadeVolumes = Request.GetBoolParam("QuantidadeVolumes");
            configuracao.EnviarSMS = Request.GetBoolParam("EnviarSMS");
            configuracao.EnviarEmail = Request.GetBoolParam("EnviarEmail");
            configuracao.LinkAvaliacaoExterna = Request.GetStringParam("LinkAvaliacaoExterna");
            configuracao.LinkAcessoPortalMultiCliFor = Request.GetStringParam("LinkAcessoPortalMultiCliFor");
            configuracao.HabilitarAcessoPortalMultiCliFor = Request.GetBoolParam("HabilitarAcessoPortalMultiCliFor");
            configuracao.TipoAvaliacao = Request.GetEnumParam<TipoAvaliacaoPortalCliente>("TipoAvaliacao");
            configuracao.PermitirAdicionarAnexos = Request.GetBoolParam("PermitirAdicionarAnexos");
            configuracao.HabilitarVisualizacaoFotosPortal = Request.GetBoolParam("HabilitarVisualizacaoFotosPortal");
        }

        private void PreencheEntidadePergunta(ref Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao pergunta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao repPortalClientePerguntaAvaliacao = new Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao(unitOfWork);

            pergunta.Titulo = Request.GetStringParam("Titulo");
            pergunta.Conteudo = Request.GetStringParam("Descricao");
            pergunta.Ordem = Request.GetIntParam("Ordem");

            if(pergunta.Ordem == 0)
            {
                pergunta.Ordem = repPortalClientePerguntaAvaliacao.ProximaOrdem();
            }
        }

        private Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente ObterEntidade(Repositorio.UnitOfWork unitOfWork)
        {
            return Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);
        }

        private bool ValidaEntidadePergunta(Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao pergunta, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(pergunta.Titulo))
            {
                msgErro = Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.TituloDaPerguntaObrigatorio;
                return false;
            }

            return true;
        }
        #endregion
    }
}
