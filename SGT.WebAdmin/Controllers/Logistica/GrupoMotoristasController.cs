using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/GrupoMotoristas")]
    public class GrupoMotoristasController(Conexao conexao) : BaseController(conexao)
    {
        #region Métodos Globais

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Servicos.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas servico = new(unitOfWork, cancellationToken, Auditado);

                Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.CriacaoGrupoMotoristas criacao = MontarCriacaoGrupoMotoristas();

                await servico.CriarGrupoMotoristas(criacao);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o Grupo de Motoristas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Servicos.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas servico = new(unitOfWork, cancellationToken, Auditado);

                Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.AtualizacaoGrupoMotoristas atualizacao = await MontarAtualizacaoGrupoMotoristas(unitOfWork, cancellationToken);

                await servico.AtualizarGrupoMotoristas(atualizacao);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o Grupo de Motoristas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

        }

        public async Task<IActionResult> Excluir(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Servicos.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas servico = new(unitOfWork, cancellationToken, Auditado);

                Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.RetornoGrupoMotoristas grupoMotoristasAExcluir = await MontarExclusaoGrupoMotoristas(unitOfWork, cancellationToken);

                await servico.ExcluirGrupoMotoristas(grupoMotoristasAExcluir, cancellationToken);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await unitOfWork.RollbackAsync(cancellationToken);
                if (ex is ServicoException)
                    return new JsonpResult(true, false, ex.Message);

                return new JsonpResult(false, "Ocorreu uma falha ao excluir o Grupo de Motoristas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

        }

        public async Task<IActionResult> Pesquisar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas repositorio = new(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaGrupoMotoristas filtrosPesquisa = MontarFiltroPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Ativo", "Ativo", 20, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas> listaGrupoMotoristas = await repositorio.BuscarAsync(
                    filtrosPesquisa,
                    parametrosConsulta
                );

                grid.setarQuantidadeTotal(await repositorio.ContarConsultaAsync(filtrosPesquisa));

                var lista = (
                    from grupoMotoristas in listaGrupoMotoristas
                    select new
                    {
                        grupoMotoristas.Codigo,
                        grupoMotoristas.Descricao,
                        grupoMotoristas.CodigoIntegracao,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoGrupoMotoristasHelper.ObterDescricao(grupoMotoristas.Situacao),
                        Ativo = grupoMotoristas.Ativo ? "Ativo" : "Inativo",
                    }
                ).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar os Grupo de Motorista.");
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

                if (codigo < 1)
                    throw new ControllerException($"Código de Grupo de Motoristas inválido. Recebido: {codigo}");

                return new JsonpResult(await ConsultarGrupoMotoristas(codigo, unitOfWork, cancellationToken));
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os Grupo de Motoristas com seus relacionamentos.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        private Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaGrupoMotoristas MontarFiltroPesquisa()
        {
            return new()
            {
                Descricao = Request.GetNullableStringParam("Descricao"),
                CodigoIntegracao = Request.GetNullableStringParam("CodigoIntegracao"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoGrupoMotoristas>("Situacao"),
                Ativo = Request.GetNullableBoolParam("Ativo"),
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.CriacaoGrupoMotoristas MontarCriacaoGrupoMotoristas()
        {
            return new()
            {
                GrupoMotoristas = new()
                {
                    Descricao = Request.GetStringParam("Descricao"),
                    Observacoes = Request.GetStringParam("Observacao"),
                    CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                    Ativo = true,
                },
                Motoristas = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FuturoRelacionamentoGrupoMotoristas>("Funcionarios"),
                TiposIntegracao = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("TiposIntegracao"),
            };
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.AtualizacaoGrupoMotoristas> MontarAtualizacaoGrupoMotoristas(
            Repositorio.UnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.RetornoGrupoMotoristas resultadoConsulta = await ConsultarGrupoMotoristas(
                Request.GetIntParam("Codigo"),
                unitOfWork,
                cancellationToken);

            resultadoConsulta.GrupoMotoristas.Descricao = Request.GetStringParam("Descricao");
            resultadoConsulta.GrupoMotoristas.Observacoes = Request.GetStringParam("Observacao");
            resultadoConsulta.GrupoMotoristas.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            resultadoConsulta.GrupoMotoristas.Ativo = Request.GetBoolParam("Ativo");

            return new(
                resultadoConsulta,
                Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.RelacionamentoGrupoMotoristas>("Funcionarios"),
                Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("TiposIntegracao"));
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.RetornoGrupoMotoristas> MontarExclusaoGrupoMotoristas(
            Repositorio.UnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.RetornoGrupoMotoristas resultadoConsulta = await ConsultarGrupoMotoristas(
                Request.GetIntParam("Codigo"),
                unitOfWork,
                cancellationToken);

            return resultadoConsulta;
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.RetornoGrupoMotoristas> ConsultarGrupoMotoristas(int codigo, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas repositorioGrupoMotoristas = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionario repositorioGMFuncionario = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasTipoIntegracao repositorioGMTipoIntegracao = new(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas grupoMotorista = await repositorioGrupoMotoristas.BuscarPorCodigoAsync(codigo, Auditado != null);

            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao> listaGMTipoIntegracao = await repositorioGMTipoIntegracao.BuscarAsync(codigo);

            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasFuncionario> listaGMFuncionario = await repositorioGMFuncionario.BuscarAsync(codigo);

            return new(grupoMotorista, listaGMFuncionario, listaGMTipoIntegracao);
        }
    }
}
