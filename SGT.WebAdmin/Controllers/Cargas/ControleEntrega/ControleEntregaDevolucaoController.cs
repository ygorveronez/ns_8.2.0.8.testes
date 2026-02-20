using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize(new string[] { "ObterDadosDevolucaoEntrega" }, "Cargas/ControleEntrega", "Logistica/Monitoramento", "Chamados/ChamadoOcorrencia")]
    public class ControleEntregaDevolucaoController : BaseController
    {
        #region Construtores

        public ControleEntregaDevolucaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterDadosDevolucaoEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaEntrega = Request.GetIntParam("Codigo");
                int codigoMotivoChamado = Request.GetIntParam("CodigoMotivoChamado");
                int codigoChamado = Request.GetIntParam("CodigoChamado");
                var modalRejeicaoControleEntrega = Request.GetNullableBoolParam("ModalRejeicaoControleEntrega") ?? false;

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repositorioConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado repositorioCargaEntregaProdutoChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao repositorioCargaEntregaNotaFiscalItemDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repositorioMotivoChamado.BuscarPorCodigo(codigoMotivoChamado);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigoChamado);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos = repositorioCargaEntregaProduto.BuscarPorCargaEntrega(codigoCargaEntrega);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> cargaEntregaProdutoChamados = repositorioCargaEntregaProdutoChamado.BuscarPorChamado(codigoChamado);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao> itensDevolucao = repositorioCargaEntregaNotaFiscalItemDevolucao.BuscarPorCargaEntrega(codigoCargaEntrega);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repositorioConfiguracaoChamado.BuscarConfiguracaoPadrao();

                dynamic notasFiscaisRetornar = servicoControleEntrega.ObterNotasFiscais(cargaEntrega, cargaEntregaProdutos, configuracaoEmbarcador, configuracaoChamado, unitOfWork, chamado, cargaEntregaProdutoChamados, modalRejeicaoControleEntrega, TipoServicoMultisoftware);
                dynamic produtosRetornar = servicoControleEntrega.ObterProdutos(cargaEntregaProdutos, configuracaoEmbarcador, configuracaoChamado, unitOfWork, cargaEntregaProdutoChamados);
                dynamic itensRetornar = servicoControleEntrega.ObterItensDevolucao(itensDevolucao);

                return new JsonpResult(new
                {
                    DevolucaoPorPeso = cargaEntrega.Carga.TipoOperacao?.DevolucaoProdutosPorPeso ?? false,
                    ExigeConferenciaProdutosConfirmarEntrega = false,
                    NotasFiscais = notasFiscaisRetornar,
                    Produtos = produtosRetornar,
                    ItensDevolucao = itensRetornar,
                    ObrigarInformacaoLote = motivoChamado?.ObrigarInformacaoLote ?? false,
                    ObrigarDataCritica = motivoChamado?.ObrigarDataCritica ?? false,
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

        public async Task<IActionResult> SalvarObservacaoProdutoDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);

                int codigoCargaEntregaProduto = Request.GetIntParam("CodigoCargaEntregaProduto");
                string observacao = Request.GetStringParam("Observacao");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto = repositorioCargaEntregaProduto.BuscarPorCodigo(codigoCargaEntregaProduto);

                if (cargaEntregaProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (string.IsNullOrWhiteSpace(observacao))
                    return new JsonpResult(false, true, "Necessário informar algo na observação.");

                cargaEntregaProduto.ObservacaoProdutoDevolucao = observacao;

                repositorioCargaEntregaProduto.Atualizar(cargaEntregaProduto, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a observação do produto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarItemDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao repItemDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao(unitOfWork);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal notaFiscal = repNotaFiscal.BuscarPorCodigo(codigo);

                if (notaFiscal == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao itemDevolucao = CriarItemDevolucao(notaFiscal);
                unitOfWork.Start();

                repItemDevolucao.Inserir(itemDevolucao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(servicoControleEntrega.ObterItensDevolucao(repItemDevolucao.BuscarPorCargaEntregaNotaFiscal(notaFiscal.Codigo)));
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a devolução.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverItemDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao repItemDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao(unitOfWork);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao itemDevolucao = repItemDevolucao.BuscarPorCodigo(codigo);

                if (itemDevolucao == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal notaFiscal = itemDevolucao.CargaEntregaNotaFiscal;

                unitOfWork.Start();

                repItemDevolucao.Deletar(itemDevolucao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(servicoControleEntrega.ObterItensDevolucao(repItemDevolucao.BuscarPorCargaEntregaNotaFiscal(notaFiscal.Codigo)));
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a devolução.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterCargaEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                double codigoCliente = Request.GetDoubleParam("Cliente");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorClienteECarga(codigoCarga, codigoCliente) ?? repositorioCargaEntrega.BuscarPorCargaEClientePedido(codigoCarga, codigoCliente);

                if (cargaEntrega == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                var retorno = new
                {
                    cargaEntrega.Codigo,
                    QuantidadeNotas = cargaEntrega.NotasFiscais?.Count ?? 0
                };

                return new JsonpResult(retorno);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Não foi possivel obter as notas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion Métodos Globais

        #region Métodos Privados
        private dynamic CriarItemDevolucao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal entregaNF)
        {
            decimal quantidadeDevolucao = Request.GetDecimalParam("QuantidadeDevolucao");
            decimal valorDevolucao = Request.GetDecimalParam("ValorDevolucao");
            int nfDevolucao = Request.GetIntParam("NFDevolucao");

            return new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao
            {
                CargaEntregaNotaFiscal = entregaNF,
                QuantidadeDevolucao = quantidadeDevolucao,
                ValorDevolucao = valorDevolucao,
                NFDevolucao = nfDevolucao,
            };
        }

        #endregion Métodos Privados
    }
}
