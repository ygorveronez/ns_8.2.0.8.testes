using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoOrquestradorFila")]
    public class ConfiguracaoOrquestradorFilaController : BaseController
    {
        #region Construtores

        public ConfiguracaoOrquestradorFilaController(Conexao conexao) : base(conexao) { }

        #endregion Construtores

        #region Métodos Globais

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                Repositorio.ConfiguracaoOrquestradorFila repositorioConfiguracaoOrquestradorFila = new Repositorio.ConfiguracaoOrquestradorFila(unitOfWork, cancellationToken);
                Dominio.Entidades.ConfiguracaoOrquestradorFila configuracaoOrquestradorFila = new Dominio.Entidades.ConfiguracaoOrquestradorFila();

                PreencherConfiguracaoOrquestradorFila(configuracaoOrquestradorFila);
                await repositorioConfiguracaoOrquestradorFila.InserirAsync(configuracaoOrquestradorFila, Auditado);
                await unitOfWork.CommitChangesAsync();

                Servicos.Global.OrquestradorFilaConfiguracao.GetInstance().RecarrregarConfiguracoes(unitOfWork);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
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
                await unitOfWork.StartAsync();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.ConfiguracaoOrquestradorFila repositorioConfiguracaoOrquestradorFila = new Repositorio.ConfiguracaoOrquestradorFila(unitOfWork, cancellationToken);
                Dominio.Entidades.ConfiguracaoOrquestradorFila configuracaoOrquestradorFila = await repositorioConfiguracaoOrquestradorFila.BuscarPorCodigoAsync(codigo, auditavel: true);

                if (configuracaoOrquestradorFila == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                PreencherConfiguracaoOrquestradorFila(configuracaoOrquestradorFila);
                await repositorioConfiguracaoOrquestradorFila.AtualizarAsync(configuracaoOrquestradorFila, Auditado);
                await unitOfWork.CommitChangesAsync();

                Servicos.Global.OrquestradorFilaConfiguracao.GetInstance().RecarrregarConfiguracoes(unitOfWork);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.ConfiguracaoOrquestradorFila repositorioConfiguracaoOrquestradorFila = new Repositorio.ConfiguracaoOrquestradorFila(unitOfWork, cancellationToken);
                Dominio.Entidades.ConfiguracaoOrquestradorFila configuracaoOrquestradorFila = await repositorioConfiguracaoOrquestradorFila.BuscarPorCodigoAsync(codigo, auditavel: false);

                if (configuracaoOrquestradorFila == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    configuracaoOrquestradorFila.Codigo,
                    configuracaoOrquestradorFila.Identificador,
                    QuantidadeRegistrosConsulta = configuracaoOrquestradorFila.QuantidadeRegistrosConsulta.ToString("n0"),
                    QuantidadeRegistrosRetorno = configuracaoOrquestradorFila.QuantidadeRegistrosRetorno.ToString("n0"),
                    LimiteTentativas = (configuracaoOrquestradorFila.LimiteTentativas > 0) ? configuracaoOrquestradorFila.LimiteTentativas.ToString("n0") : string.Empty,
                    configuracaoOrquestradorFila.TratarRegistrosComFalha
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.ConfiguracaoOrquestradorFila repositorioConfiguracaoOrquestradorFila = new Repositorio.ConfiguracaoOrquestradorFila(unitOfWork, cancellationToken);
                Dominio.Entidades.ConfiguracaoOrquestradorFila configuracaoOrquestradorFila = await repositorioConfiguracaoOrquestradorFila.BuscarPorCodigoAsync(codigo, auditavel: true);

                if (configuracaoOrquestradorFila == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                await repositorioConfiguracaoOrquestradorFila.DeletarAsync(configuracaoOrquestradorFila, Auditado);
                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridPesquisa(cancellationToken));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private async Task<Models.Grid.Grid> ObterGridPesquisa(CancellationToken cancellationToken)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Identificador", "Identificador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Quantidade Consultar", "QuantidadeRegistrosConsulta", 20, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Quantidade Retornar", "QuantidadeRegistrosConsulta", 20, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Limite de Tentativas", "LimiteTentativas", 20, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Tratar Falha", "TratarRegistrosComFalha", 15, Models.Grid.Align.center, true);

                IdentificadorControlePosicaoThread? identificador = Request.GetNullableEnumParam<IdentificadorControlePosicaoThread>("Identificador");
                bool? tratarRegistrosComFalha = Request.GetNullableBoolParam("TratarRegistrosComFalha");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.ConfiguracaoOrquestradorFila repositorioConfiguracaoOrquestradorFila = new Repositorio.ConfiguracaoOrquestradorFila(unitOfWork, cancellationToken);

                int totalRegistros = await repositorioConfiguracaoOrquestradorFila.ContarConsultaAsync(identificador, tratarRegistrosComFalha);
                List<Dominio.Entidades.ConfiguracaoOrquestradorFila> listaConfiguracaoOrquestradorFila = totalRegistros > 0 ? await repositorioConfiguracaoOrquestradorFila.ConsultarAsync(identificador, tratarRegistrosComFalha, parametrosConsulta) : new List<Dominio.Entidades.ConfiguracaoOrquestradorFila>();

                var listaConfiguracaoOrquestradorFilaRetornar = (
                    from configuracaoOrquestradorFila in listaConfiguracaoOrquestradorFila
                    select new
                    {
                        configuracaoOrquestradorFila.Codigo,
                        Identificador = configuracaoOrquestradorFila.Identificador.ObterDescricao(),
                        Descricao = configuracaoOrquestradorFila.Identificador.ObterDescricaoAdicional(),
                        QuantidadeRegistrosConsulta = configuracaoOrquestradorFila.QuantidadeRegistrosConsulta.ToString("n0"),
                        QuantidadeRegistrosRetorno = configuracaoOrquestradorFila.QuantidadeRegistrosRetorno.ToString("n0"),
                        LimiteTentativas = (configuracaoOrquestradorFila.LimiteTentativas > 0) ? configuracaoOrquestradorFila.LimiteTentativas.ToString("n0") : string.Empty,
                        TratarRegistrosComFalha = configuracaoOrquestradorFila.TratarRegistrosComFalha.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaConfiguracaoOrquestradorFilaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private void PreencherConfiguracaoOrquestradorFila(Dominio.Entidades.ConfiguracaoOrquestradorFila configuracaoOrquestradorFila)
        {
            configuracaoOrquestradorFila.Identificador = Request.GetEnumParam<IdentificadorControlePosicaoThread>("Identificador");
            configuracaoOrquestradorFila.QuantidadeRegistrosConsulta = Request.GetIntParam("QuantidadeRegistrosConsulta");
            configuracaoOrquestradorFila.QuantidadeRegistrosRetorno = Request.GetIntParam("QuantidadeRegistrosRetorno");
            configuracaoOrquestradorFila.LimiteTentativas = Request.GetIntParam("LimiteTentativas");
            configuracaoOrquestradorFila.TratarRegistrosComFalha = Request.GetBoolParam("TratarRegistrosComFalha");

            if (configuracaoOrquestradorFila.QuantidadeRegistrosConsulta <= 0)
                throw new ControllerException("A quantidade de registros para consultar deve ser maior do que zero.");

            if (configuracaoOrquestradorFila.QuantidadeRegistrosRetorno <= 0)
                throw new ControllerException("A quantidade de registros para retornar deve ser maior do que zero.");

            if (configuracaoOrquestradorFila.QuantidadeRegistrosConsulta < configuracaoOrquestradorFila.QuantidadeRegistrosRetorno)
                throw new ControllerException("A quantidade de registros para consultar deve ser maior ou igual a quantidade para retornar.");
        }

        #endregion Métodos Privados
    }
}
