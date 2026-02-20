using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Configuracoes/Motivo")]
    public class MotivoController : BaseController
    {
        #region Construtores

        public MotivoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.Configuracoes.Motivo repositorioMotivo = new Repositorio.Embarcador.Configuracoes.Motivo(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.Motivo motivo = new Dominio.Entidades.Embarcador.Configuracoes.Motivo();

                PreencherMotivo(motivo);

                await unitOfWork.StartAsync(cancellationToken);

                await repositorioMotivo.InserirAsync(motivo);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.Motivo repositorioMotivo = new Repositorio.Embarcador.Configuracoes.Motivo(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.Motivo motivo = await repositorioMotivo.BuscarPorCodigoAsync(codigo, auditavel: false);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMotivo(motivo);

                await unitOfWork.StartAsync(cancellationToken);

                await repositorioMotivo.AtualizarAsync(motivo);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
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
                Repositorio.Embarcador.Configuracoes.Motivo repositorioMotivo = new Repositorio.Embarcador.Configuracoes.Motivo(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.Motivo motivo = await repositorioMotivo.BuscarPorCodigoAsync(codigo, auditavel: false);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivo.Codigo,
                    motivo.Descricao,
                    motivo.Tipo,
                    motivo.Ativo
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Configuracoes.Motivo repositorioMotivo = new Repositorio.Embarcador.Configuracoes.Motivo(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.Motivo motivo = await repositorioMotivo.BuscarPorCodigoAsync(codigo, auditavel: true);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                await unitOfWork.StartAsync(cancellationToken);

                await repositorioMotivo.DeletarAsync(motivo, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover os dados.");
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
                var grid = await ObterGridPesquisa(cancellationToken);

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
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridPesquisa(cancellationToken));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherMotivo(Dominio.Entidades.Embarcador.Configuracoes.Motivo motivo)
        {
            motivo.Descricao = Request.GetNullableStringParam("Descricao") ?? throw new ControllerException("A descrição é obrigatória.");
            motivo.Tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivo>("Tipo");
            motivo.Ativo = Request.GetBoolParam("Ativo");
        }

        private async Task<Models.Grid.Grid> ObterGridPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var filtrosPesquisa = new Dominio.ObjetosDeValor.FiltroPesquisaMotivoRejeicao()
                {
                    Descricao = Request.GetNullableStringParam("Descricao"),
                    Ativo = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo"),
                    TipoMotivo = Request.GetEnumParam<TipoMotivo>("TipoMotivo")
                };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 50, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 50, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Configuracoes.Motivo repositorioMotivo = new Repositorio.Embarcador.Configuracoes.Motivo(unitOfWork, cancellationToken);

                int totalRegistros = await repositorioMotivo.ContarConsultaAsync(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Configuracoes.Motivo> listaMotivo = totalRegistros > 0 ? await repositorioMotivo.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Configuracoes.Motivo>();

                var listaMotivoRetornar = (
                    from motivo in listaMotivo
                    select new
                    {
                        motivo.Codigo,
                        motivo.Descricao,
                        motivo.DescricaoTipo,
                        motivo.DescricaoAtivo,
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion
    }
}
