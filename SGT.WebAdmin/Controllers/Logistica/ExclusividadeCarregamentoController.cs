using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ExclusividadeCarregamento")]
    public class ExclusividadeCarregamentoController : BaseController
    {
		#region Construtores

		public ExclusividadeCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repositorio = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividade = new Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento();

                PreencherExclusividadeCarregamento(exclusividade, unitOfWork);

                repositorio.Inserir(exclusividade, Auditado);

                AtualizarPeriodosCarregamento(exclusividade, null, unitOfWork);
                ValidarExclusividadeCarregamento(exclusividade, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException exclusividade)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exclusividade.Message);
            }
            catch (Exception exclusividade)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exclusividade);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repositorio = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividade = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (exclusividade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                PreencherExclusividadeCarregamento(exclusividade, unitOfWork);

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repositorio.Atualizar(exclusividade, Auditado);

                AtualizarPeriodosCarregamento(exclusividade, historico, unitOfWork);
                ValidarExclusividadeCarregamento(exclusividade, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException exclusividade)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exclusividade.Message);
            }
            catch (Exception exclusividade)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exclusividade);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repositorio = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividade = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (exclusividade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    exclusividade.Codigo,
                    CentroCarregamento = new
                    {
                        exclusividade.CentroCarregamento.Codigo,
                        exclusividade.CentroCarregamento.Descricao,
                    },
                    Transportador = new
                    {
                        Codigo = exclusividade.Transportador?.Codigo ?? 0,
                        Descricao = exclusividade.Transportador?.Descricao ?? string.Empty,
                    },
                    Cliente = new
                    {
                        Codigo = exclusividade.Cliente?.Codigo ?? 0,
                        Descricao = exclusividade.Cliente?.Descricao ?? string.Empty,
                    },
                    ModeloVeicularCarga = new
                    {
                        Codigo = exclusividade.ModeloVeicularCarga?.Codigo ?? 0,
                        Descricao = exclusividade.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    },
                    exclusividade.Descricao,
                    DataInicial = exclusividade.DataInicial.ToDateString(),
                    DataFinal = exclusividade.DataFinal.ToDateString(),
                    Segunda = exclusividade.DisponivelSegunda,
                    Terca = exclusividade.DisponivelTerca,
                    Quarta = exclusividade.DisponivelQuarta,
                    Quinta = exclusividade.DisponivelQuinta,
                    Sexta = exclusividade.DisponivelSexta,
                    Sabado = exclusividade.DisponivelSabado,
                    Domingo = exclusividade.DisponivelDomingo,
                    PeriodosCarregamento = (
                        from periodo in exclusividade.PeriodosCarregamento
                        select new
                        {
                            periodo.Codigo,
                            DiaSemana = 0,
                            HoraInicio = periodo.HoraInicio.ToString(@"hh\:mm"),
                            HoraTermino = periodo.HoraTermino.ToString(@"hh\:mm"),
                            ContainerTipoOperacao = (from obj in periodo.TipoOperacaoSimultaneo
                                                     select new
                                                     {
                                                         TipoOperacao = new
                                                         {
                                                             obj.TipoOperacao.Codigo,
                                                             obj.TipoOperacao.Descricao,
                                                         },
                                                         CapacidadeTipoOperacao = obj.CapacidadeCarregamentoSimultaneo
                                                     }).ToList()
                        }
                    ).ToList(),
                });
            }
            catch (Exception exclusividade)
            {
                Servicos.Log.TratarErro(exclusividade);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repositorio = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoCarregamento repPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividade = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (exclusividade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo in exclusividade.PeriodosCarregamento)
                    repPeriodoCarregamento.Deletar(periodo);

                repositorio.Deletar(exclusividade, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception exclusividade)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exclusividade);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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
                var grid = ObterGridPesquisa();
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

        private void AdicionarOuAtualizarPeriodosCarregamento(Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividade, dynamic periodosCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo repPeriodoCarregamentoTipoOperacaoSimultaneo = new Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo(unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            if (exclusividade.PeriodosCarregamento == null)
                exclusividade.PeriodosCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();

            foreach (var periodoCarregamento in periodosCarregamento)
            {
                int? codigo = ((string)periodoCarregamento.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo;

                if (codigo.HasValue)
                    periodo = repositorioPeriodoCarregamento.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException("Não foi possível encontrar o período de carregamento.");
                else
                    periodo = new Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento();

                periodo.CentroCarregamento = exclusividade.CentroCarregamento;
                periodo.ExclusividadeCarregamento = exclusividade;
                periodo.Dia = 0;
                periodo.HoraInicio = ((string)periodoCarregamento.HoraInicio).ToTime();
                periodo.HoraTermino = ((string)periodoCarregamento.HoraTermino).ToTime();

                if (periodo.TipoOperacaoSimultaneo == null)
                    periodo.TipoOperacaoSimultaneo = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo>();

                Dominio.Entidades.Auditoria.HistoricoObjeto historicoPeriodo = null;
                if (periodo.Codigo > 0)
                    historicoPeriodo = repositorioPeriodoCarregamento.Atualizar(periodo, historico != null ? Auditado : null, historico);
                else
                    repositorioPeriodoCarregamento.Inserir(periodo, historico != null ? Auditado : null, historico);

                List<int> codigosTipoOperacaoAdicionadoOuAtualizado = new List<int>();
                foreach (var periodoCarregamentoTipoOperacao in periodoCarregamento.ContainerTipoOperacao)
                {
                    int codigoTipoOperacao = ((string)periodoCarregamentoTipoOperacao.TipoOperacao.Codigo).ToInt();
                    if (codigoTipoOperacao == 0)
                        throw new ControllerException("Não foi possível encontrar o período de carregamento tipo operação simultâneo.");

                    Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo tipoOperacao = repPeriodoCarregamentoTipoOperacaoSimultaneo.BuscarPorCodigoPeriodoETipoOperacao(periodo.Codigo, codigoTipoOperacao);
                    if (tipoOperacao == null)
                        tipoOperacao = new Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo();
                    else
                        tipoOperacao.Initialize();

                    tipoOperacao.PeriodoCarregamento = periodo;
                    tipoOperacao.TipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);
                    tipoOperacao.CapacidadeCarregamentoSimultaneo = ((string)periodoCarregamentoTipoOperacao.CapacidadeTipoOperacao).ToInt();

                    if (tipoOperacao.Codigo > 0)
                        repPeriodoCarregamentoTipoOperacaoSimultaneo.Atualizar(tipoOperacao, historicoPeriodo != null ? Auditado : null, historicoPeriodo);
                    else
                        repPeriodoCarregamentoTipoOperacaoSimultaneo.Inserir(tipoOperacao, historicoPeriodo != null ? Auditado : null, historicoPeriodo);

                    codigosTipoOperacaoAdicionadoOuAtualizado.Add(tipoOperacao.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> registrosParaDeletar = (from o in periodo.TipoOperacaoSimultaneo
                                                                                                                               where !codigosTipoOperacaoAdicionadoOuAtualizado.Contains(o.Codigo)
                                                                                                                               select o).ToList();

                foreach (var registro in registrosParaDeletar)
                    repPeriodoCarregamentoTipoOperacaoSimultaneo.Deletar(registro, historicoPeriodo != null ? Auditado : null, historicoPeriodo);
            }
        }

        private void ValidarExclusividadeCarregamento(Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repExclusividadeCarregamento = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(unitOfWork);

            if (exclusividade.Transportador == null && exclusividade.Cliente == null && exclusividade.ModeloVeicularCarga == null)
                throw new ControllerException($"É obrigatório selecionar um dos parâmetros: transportador, cliente ou modelo veicular");

            Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividadeInvalida = repExclusividadeCarregamento.BuscarExclusividadeIncompativel(exclusividade);
            if (exclusividadeInvalida != null)
                throw new ControllerException($"Já existe uma exceção ({exclusividadeInvalida.Descricao}) para o mesmo período.");
        }

        private void AtualizarPeriodosCarregamento(Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividade, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic periodosCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PeriodosCarregamento"));

            ExcluirPeriodosCarregamentoRemovidos(exclusividade, periodosCarregamento, historico, unitOfWork);
            AdicionarOuAtualizarPeriodosCarregamento(exclusividade, periodosCarregamento, historico, unitOfWork);
        }

        private void ExcluirPeriodosCarregamentoRemovidos(Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividade, dynamic periodosCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);

            if (exclusividade.PeriodosCarregamento?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var periodoCarregamento in periodosCarregamento)
                {
                    int? codigo = ((string)periodoCarregamento.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        codigos.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamentoDeletar = (from o in exclusividade.PeriodosCarregamento where !codigos.Contains(o.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento in periodosCarregamentoDeletar)
                    repositorioPeriodoCarregamento.Deletar(periodoCarregamento, historico != null ? Auditado : null, historico);
            }
        }

        private void PreencherExclusividadeCarregamento(Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            exclusividade.CentroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(Request.GetIntParam("CentroCarregamento")) ?? throw new ControllerException("O centro de carregamento deve ser informado.");
            exclusividade.Transportador = repEmpresa.BuscarPorCodigo(Request.GetIntParam("Transportador"));
            exclusividade.Cliente = repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Cliente"));
            exclusividade.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(Request.GetIntParam("ModeloVeicularCarga"));
            exclusividade.DataInicial = Request.GetNullableDateTimeParam("DataInicial") ?? throw new ControllerException("Data Inicial Obrigatória.");
            exclusividade.DataFinal = Request.GetNullableDateTimeParam("DataFinal") ?? throw new ControllerException("Data Final Obrigatória.");
            exclusividade.Descricao = Request.GetStringParam("Descricao");
            exclusividade.DisponivelSegunda = Request.GetBoolParam("Segunda");
            exclusividade.DisponivelTerca = Request.GetBoolParam("Terca");
            exclusividade.DisponivelQuarta = Request.GetBoolParam("Quarta");
            exclusividade.DisponivelQuinta = Request.GetBoolParam("Quinta");
            exclusividade.DisponivelSexta = Request.GetBoolParam("Sexta");
            exclusividade.DisponivelSabado = Request.GetBoolParam("Sabado");
            exclusividade.DisponivelDomingo = Request.GetBoolParam("Domingo");

            if (exclusividade.Codigo == 0)
                exclusividade.Usuario = Usuario;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExclusividadeCarregamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExclusividadeCarregamento()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                Transportador = Request.GetIntParam("Transportador"),
                Cliente = Request.GetIntParam("Cliente"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Dia Semana", "DiaSemana", 20, Models.Grid.Align.left, true).Ord(false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Centro de Carregamento", "CentroCarregamento", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cliente", "Cliente", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicularCarga", 20, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExclusividadeCarregamento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repositorio = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> listaExclusividadeCarregamento = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento>();

                var listaExclusividadeCarregamentoRetornar = (
                    from exclusividade in listaExclusividadeCarregamento
                    select new
                    {
                        exclusividade.Codigo,
                        Data = exclusividade.DescricaoData,
                        exclusividade.Descricao,
                        DiaSemana = exclusividade.DescricaoDiaSemana,
                        CentroCarregamento = exclusividade.CentroCarregamento.Descricao,
                        Transportador = exclusividade.Transportador?.Descricao ?? string.Empty,
                        Cliente = exclusividade.Cliente?.Descricao ?? string.Empty,
                        ModeloVeicularCarga = exclusividade.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    }
                ).ToList();

                grid.AdicionaRows(listaExclusividadeCarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CentroCarregamento")
                return "CentroCarregamento.Descricao";

            if (propriedadeOrdenar == "Transportador")
                return "Transportador.RazaoSocial";

            if (propriedadeOrdenar == "Cliente")
                return "Cliente.Nome";

            if (propriedadeOrdenar == "ModeloVeicularCarga")
                return "ModeloVeicularCarga.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
