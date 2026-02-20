using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ConfiguracaoRotaFrete")]
    public class ConfiguracaoRotaFreteController : BaseController
    {
        #region Construtores

        public ConfiguracaoRotaFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete repositorio = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete = new Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete();

                PreencherConfiguracaoRotaFrete(configuracaoRotaFrete, unitOfWork);
                repositorio.Inserir(configuracaoRotaFrete, Auditado);

                await AtualizarGrupoTransportadoresHUBOfertas(configuracaoRotaFrete, unitOfWork);
                AtualizarEmpresas(configuracaoRotaFrete, unitOfWork);
                AtualizarEstados(configuracaoRotaFrete, unitOfWork);
                AtualizarModelosVeicularesCarga(configuracaoRotaFrete, unitOfWork);
                AtualizarLocalidadesDestino(configuracaoRotaFrete, unitOfWork);
                AtualizarLocalidadesOrigem(configuracaoRotaFrete, unitOfWork);
                AtualizarTiposCarga(configuracaoRotaFrete, unitOfWork);
                await AtualizarTiposOperacao(configuracaoRotaFrete, unitOfWork, cancellationToken);
                repositorio.Atualizar(configuracaoRotaFrete);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete repositorio = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (configuracaoRotaFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherConfiguracaoRotaFrete(configuracaoRotaFrete, unitOfWork);
                await AtualizarGrupoTransportadoresHUBOfertas(configuracaoRotaFrete, unitOfWork);
                AtualizarEmpresas(configuracaoRotaFrete, unitOfWork);
                AtualizarEstados(configuracaoRotaFrete, unitOfWork);
                AtualizarModelosVeicularesCarga(configuracaoRotaFrete, unitOfWork);
                AtualizarLocalidadesDestino(configuracaoRotaFrete, unitOfWork);
                AtualizarLocalidadesOrigem(configuracaoRotaFrete, unitOfWork);
                AtualizarTiposCarga(configuracaoRotaFrete, unitOfWork);
                await AtualizarTiposOperacao(configuracaoRotaFrete, unitOfWork, cancellationToken);

                repositorio.Atualizar(configuracaoRotaFrete, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete repositorio = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (configuracaoRotaFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa repositorioConfiguracaoRotaFreteEmpresa = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa(unitOfWork);
                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga repositorioConfiguracaoRotaFreteModeloVeicularCarga = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga repositorioConfiguracaoRotaFreteTipoCarga = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.TipoOperacaoConfiguracaoRotaFrete repositorioTipoOperacaoConfiguracaoRotaFrete = new Repositorio.Embarcador.Logistica.TipoOperacaoConfiguracaoRotaFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas repositorioGrupoTransportadoresHUBOfertas = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas(unitOfWork);
                Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas repositorioTransportadorGrupoTransportadoresHUB = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> empresas = repositorioConfiguracaoRotaFreteEmpresa.BuscarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga> modelosVeicularesCarga = repositorioConfiguracaoRotaFreteModeloVeicularCarga.BuscarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga> tiposCarga = repositorioConfiguracaoRotaFreteTipoCarga.BuscarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas> grupoTransportadoresHUB = await repositorioGrupoTransportadoresHUBOfertas.BuscarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas> transportadorGrupoTransportadoresHUBO = grupoTransportadoresHUB == null || !grupoTransportadoresHUB.Any() ? new List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>() : await repositorioTransportadorGrupoTransportadoresHUB.BuscarPorGruposTransportadores(grupoTransportadoresHUB);
                List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TipoOperacaoConfiguracaoRotaFrete> tipoOperacoesConfiguracaoRotaFrete = await repositorioTipoOperacaoConfiguracaoRotaFrete.BuscarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);
                List<Dominio.Entidades.Localidade> localidadesOrigem = configuracaoRotaFrete.LocalidadesOrigem?.ToList() ?? new List<Dominio.Entidades.Localidade>();
                List<Dominio.Entidades.Localidade> localidadesDestino = configuracaoRotaFrete.LocalidadesDestino?.ToList() ?? new List<Dominio.Entidades.Localidade>();

                return new JsonpResult(new
                {
                    Detalhes = new
                    {
                        configuracaoRotaFrete.Codigo,
                        configuracaoRotaFrete.Descricao,
                        configuracaoRotaFrete.Ativo,
                        Filial = new { Codigo = configuracaoRotaFrete.Filial?.Codigo ?? 0, Descricao = configuracaoRotaFrete.Filial?.Descricao ?? "" },
                        HoraEnvioTransportadorRota = configuracaoRotaFrete.HoraEnvioTransportadorRota.HasValue ? configuracaoRotaFrete.HoraEnvioTransportadorRota.Value.ToString(@"hh\:mm") : "",
                        configuracaoRotaFrete.DiasAntecedenciaEnvioTransportadorRota,
                        configuracaoRotaFrete.EnviarTransportadorRotaSegunda,
                        configuracaoRotaFrete.EnviarTransportadorRotaTerca,
                        configuracaoRotaFrete.EnviarTransportadorRotaQuarta,
                        configuracaoRotaFrete.EnviarTransportadorRotaQuinta,
                        configuracaoRotaFrete.EnviarTransportadorRotaSexta,
                        configuracaoRotaFrete.EnviarTransportadorRotaSabado,
                        configuracaoRotaFrete.EnviarTransportadorRotaDomingo,
                        configuracaoRotaFrete.EnviarTransportadorRotaSegundaHUB,
                        configuracaoRotaFrete.EnviarTransportadorRotaTercaHUB,
                        configuracaoRotaFrete.EnviarTransportadorRotaQuartaHUB,
                        configuracaoRotaFrete.EnviarTransportadorRotaQuintaHUB,
                        configuracaoRotaFrete.EnviarTransportadorRotaSextaHUB,
                        configuracaoRotaFrete.EnviarTransportadorRotaSabadoHUB,
                        configuracaoRotaFrete.EnviarTransportadorRotaDomingoHUB,
                        configuracaoRotaFrete.TipoOferta,
                        configuracaoRotaFrete.LiberarSpotAbertoAposTempoLimiteGrupos,
                        HoraEnvioOfertaHUBOfertas = configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas.HasValue ? configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas.Value.ToString(@"hh\:mm") : "",
                        configuracaoRotaFrete.DiasAntecedenciaHUBOfertas,
                    },
                    Empresas = (
                        from o in empresas
                        select new
                        {
                            o.Codigo,
                            CodigoEmpresa = o.Empresa.Codigo,
                            CodigoModeloVeicularCarga = o.ModeloVeicularCarga?.Codigo ?? 0,
                            DescricaoEmpresa = o.Empresa.Descricao,
                            DescricaoModeloVeicularCarga = o.ModeloVeicularCarga?.Descricao ?? "",
                            PercentualCargasDaRota = o.PercentualCargasDaRota.ToString("n2"),
                            Prioridade = o.Prioridade.ToString("n0")
                        }
                    ).ToList(),
                    LocalidadesOrigem = (
                        from o in localidadesOrigem
                        select new
                        {
                            o.Codigo,
                            o.Descricao
                        }
                    ).ToList(),
                    Estados = (
                        from o in configuracaoRotaFrete.Estados
                        select new
                        {
                            Codigo = o.Sigla,
                            Descricao = o.Nome
                        }
                    ).ToList(),
                    TiposOperacao = (
                        from o in tipoOperacoesConfiguracaoRotaFrete
                        select new
                        {
                            Codigo = o.TipoOperacao.Codigo,
                            Descricao = o.TipoOperacao.Descricao
                        }
                    ).ToList(),
                    LocalidadesDestino = (
                        from o in localidadesDestino
                        select new
                        {
                            o.Codigo,
                            o.Descricao
                        }
                    ).ToList(),
                    ModelosVeicularesCarga = (
                        from o in modelosVeicularesCarga
                        select new
                        {
                            o.ModeloVeicularCarga.Codigo,
                            o.ModeloVeicularCarga.Descricao
                        }
                    ).ToList(),
                    TiposCarga = (
                        from o in tiposCarga
                        select new
                        {
                            o.TipoCarga.Codigo,
                            o.TipoCarga.Descricao
                        }
                    ).ToList(),
                    GrupoTransportadoresHUBOfertas = (
                        from o in grupoTransportadoresHUB
                        select new
                        {
                            o.Codigo,
                            DescricaoEmpresa = string.Join(",", transportadorGrupoTransportadoresHUBO.Where(x => x.GrupoTransportador.Codigo == o.Codigo).Select(x => x.Empresa.Descricao)),
                            Transportadores = transportadorGrupoTransportadoresHUBO.Where(x => x.GrupoTransportador.Codigo == o.Codigo).Select(x => new { x.Empresa.Codigo, DescricaoEmpresa = x.Empresa.Descricao }).ToList(),
                            o.Descricao,
                            o.SequenciaOferta,
                            o.TempoOfertarExclusivamenteParaGrupo,
                            o.TempoDeixarDeOfertarAntesDoCarregamento,
                        }
                    ).ToList(),

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

        public async Task<IActionResult> ExcluirPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete repositorio = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (configuracaoRotaFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa repositorioConfiguracaoRotaFreteEmpresa = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa(unitOfWork);
                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga repositorioConfiguracaoRotaFreteModeloVeicularCarga = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga repositorioConfiguracaoRotaFreteTipoCarga = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas repositorioGrupoTransportadoresHUBOfertas = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas(unitOfWork);
                Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas repositorioTransportadorGrupoTransportadoresHUBOfertas = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas(unitOfWork);
                Repositorio.Embarcador.Logistica.TipoOperacaoConfiguracaoRotaFrete repositorioTipoOperacaoConfiguracaoRotaFrete = new Repositorio.Embarcador.Logistica.TipoOperacaoConfiguracaoRotaFrete(unitOfWork, cancellationToken);

                configuracaoRotaFrete.Estados?.Clear();
                configuracaoRotaFrete.LocalidadesDestino?.Clear();
                configuracaoRotaFrete.LocalidadesOrigem?.Clear();
                repositorioConfiguracaoRotaFreteEmpresa.DeletarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);
                repositorioConfiguracaoRotaFreteModeloVeicularCarga.DeletarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);
                repositorioConfiguracaoRotaFreteTipoCarga.DeletarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);
                repositorioGrupoTransportadoresHUBOfertas.DeletarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);
                repositorioTipoOperacaoConfiguracaoRotaFrete.DeletarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);

                repositorio.Deletar(configuracaoRotaFrete, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
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

        #endregion

        #region Métodos Privados

        private void PreencherConfiguracaoRotaFrete(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoFilial = Request.GetIntParam("Filial");
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            configuracaoRotaFrete.Ativo = Request.GetBoolParam("Ativo");
            configuracaoRotaFrete.Descricao = Request.GetStringParam("Descricao");
            configuracaoRotaFrete.Filial = (codigoFilial > 0) ? repositorioFilial.BuscarPorCodigo(codigoFilial) : null;
            configuracaoRotaFrete.HoraEnvioTransportadorRota = Request.GetNullableTimeParam("HoraEnvioTransportadorRota");
            configuracaoRotaFrete.DiasAntecedenciaEnvioTransportadorRota = Request.GetIntParam("DiasAntecedenciaEnvioTransportadorRota");
            configuracaoRotaFrete.EnviarTransportadorRotaSegunda = Request.GetBoolParam("EnviarTransportadorRotaSegunda");
            configuracaoRotaFrete.EnviarTransportadorRotaTerca = Request.GetBoolParam("EnviarTransportadorRotaTerca");
            configuracaoRotaFrete.EnviarTransportadorRotaQuarta = Request.GetBoolParam("EnviarTransportadorRotaQuarta");
            configuracaoRotaFrete.EnviarTransportadorRotaQuinta = Request.GetBoolParam("EnviarTransportadorRotaQuinta");
            configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas = Request.GetNullableTimeParam("HoraEnvioOfertaHUBOfertas");
            configuracaoRotaFrete.EnviarTransportadorRotaSexta = Request.GetBoolParam("EnviarTransportadorRotaSexta");
            configuracaoRotaFrete.EnviarTransportadorRotaSabado = Request.GetBoolParam("EnviarTransportadorRotaSabado");
            configuracaoRotaFrete.EnviarTransportadorRotaDomingo = Request.GetBoolParam("EnviarTransportadorRotaDomingo");
            configuracaoRotaFrete.TipoOferta = Request.GetIntParam("TipoOferta");
            configuracaoRotaFrete.LiberarSpotAbertoAposTempoLimiteGrupos = Request.GetBoolParam("LiberarSpotAbertoAposTempoLimiteGrupos");
            configuracaoRotaFrete.DiasAntecedenciaHUBOfertas = Request.GetIntParam("DiasAntecedenciaHUBOfertas");
            configuracaoRotaFrete.EnviarTransportadorRotaSegundaHUB = Request.GetBoolParam("EnviarTransportadorRotaSegundaHUB");
            configuracaoRotaFrete.EnviarTransportadorRotaTercaHUB = Request.GetBoolParam("EnviarTransportadorRotaTercaHUB");
            configuracaoRotaFrete.EnviarTransportadorRotaQuartaHUB = Request.GetBoolParam("EnviarTransportadorRotaQuartaHUB");
            configuracaoRotaFrete.EnviarTransportadorRotaQuintaHUB = Request.GetBoolParam("EnviarTransportadorRotaQuintaHUB");
            configuracaoRotaFrete.EnviarTransportadorRotaSextaHUB = Request.GetBoolParam("EnviarTransportadorRotaSextaHUB");
            configuracaoRotaFrete.EnviarTransportadorRotaSabadoHUB = Request.GetBoolParam("EnviarTransportadorRotaSabadoHUB");
            configuracaoRotaFrete.EnviarTransportadorRotaDomingoHUB = Request.GetBoolParam("EnviarTransportadorRotaDomingoHUB");

            if (configuracaoRotaFrete.HoraEnvioTransportadorRota.HasValue && !configuracaoRotaFrete.EnviarTransportadorRotaConfigurado())
                throw new ControllerException("Informe um ou mais dias da semana para envio aos transportadores");
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaConfiguracaoRotaFrete ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaConfiguracaoRotaFrete()
            {
                Descricao = Request.GetStringParam("Descricao"),
                SituacaoAtivo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo)
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaConfiguracaoRotaFrete filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 30, Models.Grid.Align.left, false);

                if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Ativo", "Ativo", 10, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete repositorio = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete> listaConfiguracaoRotaFrete = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete>();

                var listaConfiguracaoRotaFreteRetornar = (
                    from configuracaoRotaFrete in listaConfiguracaoRotaFrete
                    select new
                    {
                        configuracaoRotaFrete.Codigo,
                        configuracaoRotaFrete.Descricao,
                        Filial = configuracaoRotaFrete.Filial?.Descricao ?? "",
                        Ativo = configuracaoRotaFrete.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaConfiguracaoRotaFreteRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        #endregion

        #region Métodos Privados - Localidades de Origem

        private void AtualizarLocalidadesOrigem(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            dynamic localidadesOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("LocalidadesOrigem"));

            if (configuracaoRotaFrete.LocalidadesOrigem == null)
                configuracaoRotaFrete.LocalidadesOrigem = new List<Dominio.Entidades.Localidade>();
            else
                configuracaoRotaFrete.LocalidadesOrigem.Clear();

            foreach (var localidadeOrigem in localidadesOrigem)
            {
                Dominio.Entidades.Localidade localidadeOrigemAdicionar = repositorioLocalidade.BuscarPorCodigo(((string)localidadeOrigem.Codigo).ToInt()) ?? throw new ControllerException("Origem não encontrada");
                configuracaoRotaFrete.LocalidadesOrigem.Add(localidadeOrigemAdicionar);
            }
        }

        #endregion

        #region Métodos Privados - Localidades de Destino

        private void AtualizarLocalidadesDestino(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            dynamic localidadesDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("LocalidadesDestino"));

            if (configuracaoRotaFrete.LocalidadesDestino == null)
                configuracaoRotaFrete.LocalidadesDestino = new List<Dominio.Entidades.Localidade>();
            else
                configuracaoRotaFrete.LocalidadesDestino.Clear();

            configuracaoRotaFrete.PossuiLocalidadesDestino = false;

            foreach (var localidadeDestino in localidadesDestino)
            {
                Dominio.Entidades.Localidade localidadeDestinoAdicionar = repositorioLocalidade.BuscarPorCodigo(((string)localidadeDestino.Codigo).ToInt()) ?? throw new ControllerException("Destino não encontrado");
                configuracaoRotaFrete.LocalidadesDestino.Add(localidadeDestinoAdicionar);

                configuracaoRotaFrete.PossuiLocalidadesDestino = true;
            }
        }

        #endregion

        #region Métodos Privados - Empresa

        private void AtualizarEmpresas(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic empresas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Empresas"));

            ExcluirEmpresasRemovidas(configuracaoRotaFrete, empresas, unitOfWork);
            SalvarEmpresasAdicionadasOuAtualizadas(configuracaoRotaFrete, empresas, unitOfWork);
        }

        private void ExcluirEmpresasRemovidas(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, dynamic empresas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa repositorioConfiguracaoRotaFreteEmpresa = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> listaConfiguracaoRotaFreteEmpresa = repositorioConfiguracaoRotaFreteEmpresa.BuscarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);

            if (listaConfiguracaoRotaFreteEmpresa.Count == 0)
                return;

            List<int> listaCodigosAtualizados = new List<int>();

            foreach (var empresa in empresas)
            {
                int? codigo = ((string)empresa.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    listaCodigosAtualizados.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> listaConfiguracaoRotaFreteEmpresaRemover = (from o in listaConfiguracaoRotaFreteEmpresa where !listaCodigosAtualizados.Contains(o.Codigo) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa configuracaoRotaFreteEmpresa in listaConfiguracaoRotaFreteEmpresaRemover)
                repositorioConfiguracaoRotaFreteEmpresa.Deletar(configuracaoRotaFreteEmpresa);
        }

        private void SalvarEmpresasAdicionadasOuAtualizadas(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, dynamic empresas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa repositorioConfiguracaoRotaFreteEmpresa = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            foreach (var empresa in empresas)
            {
                Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa configuracaoRotaFreteEmpresa;
                int codigoModeloVeicularCarga = ((string)empresa.CodigoModeloVeicularCarga).ToInt();
                int? codigo = ((string)empresa.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    configuracaoRotaFreteEmpresa = repositorioConfiguracaoRotaFreteEmpresa.BuscarPorCodigo(codigo.Value, auditavel: false) ?? throw new ControllerException("Transportador da configuração de rota de frete não encontrado");
                else
                    configuracaoRotaFreteEmpresa = new Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa() { ConfiguracaoRotaFrete = configuracaoRotaFrete };

                configuracaoRotaFreteEmpresa.Empresa = repositorioEmpresa.BuscarPorCodigo(((string)empresa.CodigoEmpresa).ToInt()) ?? throw new ControllerException("Transportador não encontrado");
                configuracaoRotaFreteEmpresa.ModeloVeicularCarga = (codigoModeloVeicularCarga > 0) ? repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga) : null;
                configuracaoRotaFreteEmpresa.PercentualCargasDaRota = ((string)empresa.PercentualCargasDaRota).ToDecimal();
                configuracaoRotaFreteEmpresa.Prioridade = ((string)empresa.Prioridade).ToInt();

                if (codigo.HasValue)
                    repositorioConfiguracaoRotaFreteEmpresa.Atualizar(configuracaoRotaFreteEmpresa);
                else
                    repositorioConfiguracaoRotaFreteEmpresa.Inserir(configuracaoRotaFreteEmpresa);
            }
        }

        #endregion

        #region Métodos Privados - Estados

        private void AtualizarEstados(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);
            dynamic estados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Estados"));

            if (configuracaoRotaFrete.Estados == null)
                configuracaoRotaFrete.Estados = new List<Dominio.Entidades.Estado>();
            else
                configuracaoRotaFrete.Estados.Clear();

            foreach (var estado in estados)
            {
                Dominio.Entidades.Estado estadoAdicionar = repositorioEstado.BuscarPorSigla((string)estado.Codigo) ?? throw new ControllerException("Estado não encontrado");
                configuracaoRotaFrete.Estados.Add(estadoAdicionar);
            }
        }

        #endregion

        #region Métodos Privados - Modelo Veicular de Carga

        private void AtualizarModelosVeicularesCarga(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic modelosVeicularesCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosVeicularesCarga"));
            Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga repositorioConfiguracaoRotaFreteModeloVeicularCarga = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            repositorioConfiguracaoRotaFreteModeloVeicularCarga.DeletarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);

            configuracaoRotaFrete.PossuiModelosVeicularesCarga = false;

            foreach (var modeloVeicularCarga in modelosVeicularesCarga)
            {
                Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga configuracaoRotaFreteModeloVeicularCarga = new Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga()
                {
                    ConfiguracaoRotaFrete = configuracaoRotaFrete,
                    ModeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(((string)modeloVeicularCarga.Codigo).ToInt()) ?? throw new ControllerException("Modelo veicular de carga não encontrado")
                };

                repositorioConfiguracaoRotaFreteModeloVeicularCarga.Inserir(configuracaoRotaFreteModeloVeicularCarga);

                configuracaoRotaFrete.PossuiModelosVeicularesCarga = true;
            }
        }

        #endregion

        #region Métodos Privados - Tipo de Carga

        private void AtualizarTiposCarga(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic tiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposCarga"));
            Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga repositorioConfiguracaoRotaFreteTipoCarga = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioETipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            repositorioConfiguracaoRotaFreteTipoCarga.DeletarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);

            configuracaoRotaFrete.PossuiTiposCarga = false;

            foreach (var tipoCarga in tiposCarga)
            {
                Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga configuracaoRotaFreteTipoCarga = new Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga()
                {
                    ConfiguracaoRotaFrete = configuracaoRotaFrete,
                    TipoCarga = repositorioETipoCarga.BuscarPorCodigo(((string)tipoCarga.Codigo).ToInt()) ?? throw new ControllerException("Tipo de carga não encontrado")
                };

                repositorioConfiguracaoRotaFreteTipoCarga.Inserir(configuracaoRotaFreteTipoCarga);

                configuracaoRotaFrete.PossuiTiposCarga = true;
            }
        }

        private async Task AtualizarTiposOperacao(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));
            Repositorio.Embarcador.Logistica.TipoOperacaoConfiguracaoRotaFrete repositorioTipoOperacaoConfiguracaoRotaFrete = new Repositorio.Embarcador.Logistica.TipoOperacaoConfiguracaoRotaFrete(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            repositorioTipoOperacaoConfiguracaoRotaFrete.DeletarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);

            foreach (var tipoOperacao in tiposOperacao)
            {
                Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TipoOperacaoConfiguracaoRotaFrete configuracaoTipoOperacao = new Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TipoOperacaoConfiguracaoRotaFrete()
                {
                    ConfiguracaoRotaFrete = configuracaoRotaFrete,
                    TipoOperacao = await repositorioTipoOperacao.BuscarPorCodigoAsync(((string)tipoOperacao.Codigo).ToInt()) ?? throw new ControllerException("Tipo de operação não encontrada.")
                };

                await repositorioTipoOperacaoConfiguracaoRotaFrete.InserirAsync(configuracaoTipoOperacao);
            }
        }

        #endregion

        #region Métodos Privados - Grupo Transportadores HUB de Ofertas

        private async Task AtualizarGrupoTransportadoresHUBOfertas(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic gruposTransportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GrupoTransportadoresHUBOfertas"));

            await ExcluirGruposTransportadoresHUBRemovidos(configuracaoRotaFrete, gruposTransportadores, unitOfWork);
            await SalvarGruposTransportadoresHUBAdicionadosOuAtualizados(configuracaoRotaFrete, gruposTransportadores, unitOfWork);
        }

        private async Task ExcluirGruposTransportadoresHUBRemovidos(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, dynamic gruposTransportadores, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas repositorioGrupoTransportadoresHUB = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas(unitOfWork);
            Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas repositorioTransportadorGrupoHUB = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas> listaGruposExistentes = await repositorioGrupoTransportadoresHUB.BuscarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo);

            if (listaGruposExistentes.Count == 0)
                return;

            List<int> listaCodigosAtualizados = new List<int>();

            foreach (var grupo in gruposTransportadores)
            {
                int? codigo = ((string)grupo.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    listaCodigosAtualizados.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas> listaGruposRemover = listaGruposExistentes.Where(g => !listaCodigosAtualizados.Contains(g.Codigo)).ToList();

            foreach (var grupoRemover in listaGruposRemover)
            {
                var associacoesGrupo = await repositorioTransportadorGrupoHUB.BuscarPorGrupoTransportador(grupoRemover.Codigo);
                foreach (var associacao in associacoesGrupo)
                {
                    repositorioTransportadorGrupoHUB.Deletar(associacao);
                }

                repositorioGrupoTransportadoresHUB.Deletar(grupoRemover);
            }

            return;
        }

        private async Task SalvarGruposTransportadoresHUBAdicionadosOuAtualizados(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, dynamic gruposTransportadores, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas repositorioGrupoTransportadoresHUB = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas(unitOfWork);
            Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas repositorioTransportadorGrupoHUB = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            configuracaoRotaFrete.PossuiGrupoTransportadoresHUBOfertas = false;

            foreach (var grupo in gruposTransportadores)
            {
                Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas grupoTransportadoresHUB;
                int? codigo = ((string)grupo.Codigo).ToNullableInt();

                if (codigo.HasValue && codigo.Value > 0)
                    grupoTransportadoresHUB = await repositorioGrupoTransportadoresHUB.BuscarPorCodigoAsync(codigo.Value, false) ?? throw new ControllerException("Grupo de transportadores não encontrado");
                else
                    grupoTransportadoresHUB = new Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas()
                    {
                        ConfiguracaoRotaFrete = configuracaoRotaFrete
                    };

                grupoTransportadoresHUB.Descricao = (string)grupo.Descricao;
                grupoTransportadoresHUB.SequenciaOferta = ((string)grupo.SequenciaOferta).ToInt();
                grupoTransportadoresHUB.TempoOfertarExclusivamenteParaGrupo = ((string)grupo.TempoOfertarExclusivamenteParaGrupo).ToInt();

                if (codigo.HasValue && codigo.Value > 0)
                    await repositorioGrupoTransportadoresHUB.AtualizarAsync(grupoTransportadoresHUB);
                else
                    await repositorioGrupoTransportadoresHUB.InserirAsync(grupoTransportadoresHUB);

                configuracaoRotaFrete.PossuiGrupoTransportadoresHUBOfertas = true;

                if (grupo.Transportadores != null)
                {
                    var associacoesExistentes = await repositorioTransportadorGrupoHUB.BuscarPorGrupoTransportador(grupoTransportadoresHUB.Codigo);

                    foreach (var associacao in associacoesExistentes)
                    {
                        repositorioTransportadorGrupoHUB.Deletar(associacao);
                    }

                    foreach (var transportador in grupo.Transportadores)
                    {
                        int codigoEmpresa = ((string)transportador.Codigo).ToInt();

                        if (codigoEmpresa <= 0)
                            continue;

                        var novaAssociacao = new Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas
                        {
                            GrupoTransportador = grupoTransportadoresHUB,
                            Empresa = new Dominio.Entidades.Empresa { Codigo = codigoEmpresa }
                        };

                        repositorioTransportadorGrupoHUB.Inserir(novaAssociacao);
                    }


                }
            }
        }

        public async Task<IActionResult> ContarGruposPorConfiguracaoRotaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas repositorioGrupoTransportadoresHUBOfertas = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas(unitOfWork);

                int codigoConfiguracaoRotaFrete = Request.GetIntParam("CodigoConfiguracaoRotaFrete");

                if (codigoConfiguracaoRotaFrete <= 0)
                    return new JsonpResult(0);

                int totalGrupos = repositorioGrupoTransportadoresHUBOfertas.ContarGruposPorConfiguracaoRotaFrete(codigoConfiguracaoRotaFrete);

                return new JsonpResult(totalGrupos);
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
        #endregion
    }
}
