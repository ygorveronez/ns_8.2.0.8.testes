using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/GrupoMotoristas")]
    public class GrupoMotoristasIntegracaoController : BaseController
    {
        public GrupoMotoristasIntegracaoController(Conexao conexao) : base(conexao) { }

        public async Task<IActionResult> Pesquisar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaRelacionamentosGrupoMotoristas filtro = MontarFiltroPesquisa();

                int totalRegistros = await ContarResultadosPesquisa(filtro, unitOfWork, cancellationToken);

                if (totalRegistros == 0)
                {
                    grid.AdicionaRows(new List<dynamic>());
                    grid.setarQuantidadeTotal(totalRegistros);
                    return new JsonpResult(grid);
                }

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                var lista = await ExecutaPesquisa(filtro, parametrosConsulta, unitOfWork, cancellationToken);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> PesquisarHistorico(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repositorioGrupoMotoristasIntegracao = new(unitOfWork, cancellationToken);

                int.TryParse(Request.Params("Integracao"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao integracao = await repositorioGrupoMotoristasIntegracao.BuscarPorCodigoAsync(codigo, false);
                List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracaoArquivos> arquivos = integracao.ArquivosTransacao.ToList();


                grid.setarQuantidadeTotal(arquivos.Count);

                var retorno = (from obj in arquivos.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   DescricaoTipo = integracao.TipoIntegracao.Descricao,
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
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Reenviar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repGrupoMotoristasIntegracao = new(unitOfWork, cancellationToken);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao integracao = await repGrupoMotoristasIntegracao.BuscarPorCodigoAsync(codigo, Auditado != null);

                if (integracao == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                await unitOfWork.StartAsync(cancellationToken);

                Servicos.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao servico = new(unitOfWork, cancellationToken, Auditado);

                await servico.AplicarReenvio(integracao);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ReenviarTodos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repGrupoMotoristasIntegracao = new(unitOfWork, cancellationToken);

                if (!int.TryParse(Request.Params("GrupoMotoristas"), out int codigoGrupoMotoristas))
                {
                    return new JsonpResult(false, true, "Grupo de Motoristas não informado.");
                }

                Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaRelacionamentosGrupoMotoristas filtro = new()
                {
                    CodigoGrupoMotoristas = codigoGrupoMotoristas,
                };

                List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao> integracoes = await repGrupoMotoristasIntegracao.BuscarGrupoMotoristaIntegracao(filtro, null);

                if (integracoes.Count < 1)
                {
                    return new JsonpResult(false, true, "Nenhuma integração encontrada para o grupo de motoristas informado.");
                }

                await unitOfWork.StartAsync(cancellationToken);

                Servicos.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao servico = new(unitOfWork, cancellationToken, Auditado);

                foreach (Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao integracao in integracoes)
                {

                    if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                    {
                        continue;
                    }

                    await servico.AplicarReenvio(integracao);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracaoArquivos repositorioGrupoMotoristasIntegracaoArquivos = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracaoArquivos(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracaoArquivos arquivoIntegracao = await repositorioGrupoMotoristasIntegracaoArquivos.BuscarPorCodigoAsync(codigo, false);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo", "Tipo", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 25, Models.Grid.Align.left, false);

            return grid;
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "TipoIntegracao")
                propOrdena = "TipoIntegracao.Tipo";
            else if (propOrdena == "Tentativas")
                propOrdena = "NumeroTentativas";
            else if (propOrdena == "DataEnvio")
                propOrdena = "DataIntegracao";
            else if (propOrdena == "Situacao")
                propOrdena = "SituacaoIntegracao";
        }

        private async Task<dynamic> ExecutaPesquisa(Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaRelacionamentosGrupoMotoristas filtroPesquisaRelacionamentos, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repositorio = new(unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao> listaGrid = await repositorio.BuscarGrupoMotoristaIntegracao(filtroPesquisaRelacionamentos, parametroConsulta);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Situacao = obj.DescricaoSituacaoIntegracao,
                            Tipo = obj.Tipo.ObterDescricao(),
                            TipoIntegracao = obj.TipoIntegracao.DescricaoTipo,
                            Retorno = obj.ProblemaIntegracao,
                            Tentativas = obj.NumeroTentativas,
                            DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                            DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                   obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                   Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                            DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
                        };

            return lista.ToList();
        }

        private async Task<int> ContarResultadosPesquisa(
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaRelacionamentosGrupoMotoristas filtro,
            Repositorio.UnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repositorio = new(unitOfWork, cancellationToken);


            return await repositorio.ContarBuscaGrupoMotoristaIntegracao(filtro);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaRelacionamentosGrupoMotoristas MontarFiltroPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                situacao = situacaoAux;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("TipoConsultaGrupoMotoristas");

            int.TryParse(Request.Params("CodigoGrupoMotoristas"), out int codigoGrupoMotoristas);

            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaRelacionamentosGrupoMotoristas filtroPesquisaRelacionamentos = new()
            {
                CodigoGrupoMotoristas = codigoGrupoMotoristas,
                SituacaoIntegracao = situacao,
                TipoIntegracao = tipoIntegracao,
            };

            return filtroPesquisaRelacionamentos;
        }
        #endregion

    }
}
