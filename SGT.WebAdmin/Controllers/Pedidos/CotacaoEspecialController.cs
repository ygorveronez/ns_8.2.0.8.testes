using Dominio.Entidades;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/CotacaoEspecial")]
    public class CotacaoEspecialController : BaseController
    {
        public CotacaoEspecialController(Conexao conexao) : base(conexao) { }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaCotacaoEspecial filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("Número Cotação", "Cotacao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Solicitação", "DataSolicitacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status Cotação", "DescricaoStatusCotacaoEspecial", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Peso Total", "PesoTotal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Cotação", "ValorCotacao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tipo Modal", "TipoModal", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Pedidos", "Pedidos", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor Total NFes", "ValorTotalNotasFiscais", 10, Models.Grid.Align.center, false);


                Repositorio.Embarcador.Pedidos.CotacaoEspecial repCotacaoEspecial = new Repositorio.Embarcador.Pedidos.CotacaoEspecial(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();


                List<Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial> cotacoes = repCotacaoEspecial.ConsultarParaCotacaoEspecial(filtrosPesquisa, parametrosConsulta);

                int totalRegistros = repCotacaoEspecial.ContarConsultaParaCotacaoEspecial(filtrosPesquisa);

                grid.AdicionaRows((
                    from o in cotacoes
                    select new
                    {
                        o.Codigo,
                        Cotacao = o.Codigo,
                        DataSolicitacao = o.DataSolicitacao?.ToDateTimeString() ?? string.Empty,
                        Status = o.StatusCotacaoEspecial,
                        DescricaoStatusCotacaoEspecial = o.StatusCotacaoEspecial?.Descricao() ?? string.Empty,
                        PesoTotal = o.PesoTotal.ToString("n2") + " KG",
                        ValorCotacao = o.ValorCotacao.ToString("N2"),
                        TipoModal = o.TipoModal?.ObterDescricao() ?? string.Empty,
                        Pedidos = string.Join(", ", o.Pedidos.Select(o => o.NumeroPedidoEmbarcador).Distinct()),
                        ValorTotalNotasFiscais = o.ValorTotalNotasFiscais.ToString("N2")
                    }).ToList()
                );

                grid.setarQuantidadeTotal(totalRegistros);
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
        public async Task<IActionResult> SimularFreteCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                Repositorio.Embarcador.Pedidos.CotacaoEspecial repCotacaoEspecial = new Repositorio.Embarcador.Pedidos.CotacaoEspecial(unitOfWork);

                int codigoCotacao = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial cotacao = await repCotacaoEspecial.BuscarPorCodigoAsync(codigoCotacao, false);

                if (cotacao == null)
                    throw new ControllerException("Registro não encontrado.");

                PreencherCotacao(cotacao, unitOfWork);

                cotacao.ValorCotacao = Servicos.Embarcador.Carga.Frete.CalcularFretePorPedidos(cotacao, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                await repCotacaoEspecial.AtualizarAsync(cotacao);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(new { ValorSimulado = cotacao.ValorCotacao }, true, "Frete simulado com sucesso");
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu algum erro a simular o frete");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfirmarCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                Repositorio.Embarcador.Pedidos.CotacaoEspecial repCotacao = new Repositorio.Embarcador.Pedidos.CotacaoEspecial(unitOfWork);

                int codigoCotacao = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial cotacao = await repCotacao.BuscarPorCodigoAsync(codigoCotacao, false);

                if (cotacao == null)
                    throw new ControllerException("Registro não encontrado.");

                cotacao.Initialize();

                PreencherCotacao(cotacao, unitOfWork);

                cotacao.StatusCotacaoEspecial = StatusCotacaoEspecial.AguardandoAprovacao;

                await Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadasAsync(Auditado, cotacao, cotacao.GetChanges(), "Alterou a Cotação de Frete", unitOfWork);

                await repCotacao.AtualizarAsync(cotacao);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Erro ao confirmar a cotação.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AprovarCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync();

                Repositorio.Embarcador.Pedidos.CotacaoEspecial repositorioCotacao = new Repositorio.Embarcador.Pedidos.CotacaoEspecial(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                int CodigoCotacao = Request.GetIntParam("CodigoCotacao");

                Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial cotacao = await repositorioCotacao.BuscarPorCodigoAsync(CodigoCotacao, false);

                if (cotacao == null)
                    throw new ControllerException("Cotação não encontrada");

                if (cotacao.StatusCotacaoEspecial == StatusCotacaoEspecial.Aprovado)
                    return new JsonpResult(true, "Cotação já está aprovada.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = GerarCarga(ref cotacao, unitOfWork);

                await repositorioCotacao.AtualizarAsync(cotacao);
                await repositorioCarga.AtualizarAsync(carga);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true, true, $"Cotação Aprovada com sucesso! Carga {carga.CodigoCargaEmbarcador}. ");
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Erro ao aprovar a cotação.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RejeitarCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.CotacaoEspecial repCotacao = new Repositorio.Embarcador.Pedidos.CotacaoEspecial(unitOfWork);

                int CodigoCotacao = Request.GetIntParam("CodigoCotacao");

                Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial cotacaoExistente = await repCotacao.BuscarPorCodigoAsync(CodigoCotacao, false);

                if (cotacaoExistente == null)
                    throw new ControllerException("Registro não encontrado.");

                if (cotacaoExistente.StatusCotacaoEspecial == StatusCotacaoEspecial.Reprovado)
                    return new JsonpResult(true, "Cotação já está rejeitada.");

                cotacaoExistente.StatusCotacaoEspecial = StatusCotacaoEspecial.Reprovado;
                cotacaoExistente.ValorCotacao = 0;

                await repCotacao.AtualizarAsync(cotacaoExistente);

                return new JsonpResult(true, true, "Cotação Rejeitada com sucesso!");
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Erro ao rejeitar a cotação");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaCotacaoEspecial ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaCotacaoEspecial()
            {
                DataCotacaoInicial = Request.GetDateTimeParam("DataCotacaoInicial"),
                DataCotacaoFinal = Request.GetDateTimeParam("DataCotacaoFinal"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                NumeroCotacao = Request.GetIntParam("NumeroCotacao"),
                StatusCotacaoEspecial = Request.GetEnumParam<StatusCotacaoEspecial>("StatusCotacaoEspecial"),
                TipoModal = Request.GetEnumParam<TipoModalCotacaoEspecial>("TipoModal"),
                CodigoFornecedor = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor ? Usuario.Cliente?.CPF_CNPJ ?? 0 : 0,
            };
        }

        private void PreencherCotacao(Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial cotacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            int codigoTransportador = Request.GetIntParam("Transportador");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");
            decimal valorSimulado = Request.GetDecimalParam("ValorSimulado");

            cotacao.Transportador = codigoTransportador > 0 ? repositorioEmpresa.BuscarPorCodigo(codigoTransportador) : null;
            cotacao.TipoOperacao = codigoTipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
            cotacao.ModeloVeicularCarga = codigoModeloVeicular > 0 ? repositorioModeloVeicular.BuscarPorCodigo(codigoModeloVeicular) : null;
            cotacao.ValorCotacao = valorSimulado;
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga GerarCarga(ref Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial cotacao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serRegraICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga();
            string mensagemRetornoCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out carga, cotacao.Pedidos.ToList(), unitOfWork, TipoServicoMultisoftware, null, ConfiguracaoEmbarcador, true, false, false, false);

            if (!string.IsNullOrWhiteSpace(mensagemRetornoCarga))
                throw new ControllerException(mensagemRetornoCarga);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCarga(carga).FirstOrDefault();

            carga.Empresa = cotacao.Transportador;
            carga.TipoOperacao = cotacao.TipoOperacao;
            carga.ModeloVeicularCarga = cotacao.ModeloVeicularCarga;
            cotacao.StatusCotacaoEspecial = StatusCotacaoEspecial.Aprovado;
            cotacao.Carga = carga;

            bool incluirBase = true;
            decimal percentualIncluir = 100;
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serRegraICMS.BuscarRegraICMS(carga, cargaPedido, carga.Empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirBase, ref percentualIncluir, cotacao.ValorCotacao, null, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

            carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
            carga.ValorFreteLiquido = cotacao.ValorCotacao;
            carga.ValorFreteAPagar = Math.Round((carga.ValorFreteLiquido + regraICMS.ValorICMS), 2, MidpointRounding.AwayFromZero);
            carga.ValorICMS = regraICMS.ValorICMS;
            carga.ValorFrete = cotacao.ValorCotacao;

            if (cotacao.Usuario?.Cliente?.CotacaoEspecial > 0)
                carga.ValorFreteTabelaFrete -= Math.Round(cotacao.ValorCotacao * (cotacao.Usuario.Cliente.CotacaoEspecial / 100), 2, MidpointRounding.AwayFromZero);
            else
                carga.ValorFreteTabelaFrete = cotacao.ValorCotacao;

            serCarga.InformarSituacaoCargaFreteValido(ref carga, TipoServicoMultisoftware, unitOfWork);

            return carga;
        }
    }
}
