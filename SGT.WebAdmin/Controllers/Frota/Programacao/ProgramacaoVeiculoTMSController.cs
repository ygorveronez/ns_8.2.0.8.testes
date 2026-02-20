using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Frota.Programacao
{
    [CustomAuthorize("Frota/ProgramacaoVeiculoTMS")]
    public class ProgramacaoVeiculoTMSController : BaseController
    {
		#region Construtores

		public ProgramacaoVeiculoTMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                int totalRegistros = 0, novoLimite = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork, ref novoLimite);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);
                if (novoLimite > 0)
                    grid.limite = novoLimite;

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                int totalRegistros = 0, novoLimite = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork, ref novoLimite);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);
                if (novoLimite > 0)
                    grid.limite = novoLimite;

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS repProgramacaoVeiculoTMS = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS programacaoVeiculoTMS = repProgramacaoVeiculoTMS.BuscarPorCodigo(codigo);

                // Valida
                if (programacaoVeiculoTMS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    programacaoVeiculoTMS.Codigo,
                    Veiculo = programacaoVeiculoTMS.Veiculo != null ? new { Codigo = programacaoVeiculoTMS.Veiculo.Codigo, Descricao = programacaoVeiculoTMS.Veiculo.Placa } : null,
                    Motorista = programacaoVeiculoTMS.Motorista != null ? new { Codigo = programacaoVeiculoTMS.Motorista.Codigo, Descricao = programacaoVeiculoTMS.Motorista.Nome } : null,
                    ProgramacaoSituacao = programacaoVeiculoTMS.ProgramacaoSituacao != null ? new { Codigo = programacaoVeiculoTMS.ProgramacaoSituacao.Codigo, Descricao = programacaoVeiculoTMS.ProgramacaoSituacao.Descricao } : null,
                    CidadeEstado = programacaoVeiculoTMS.CidadeEstado != null ? new { Codigo = programacaoVeiculoTMS.CidadeEstado.Codigo, Descricao = programacaoVeiculoTMS.CidadeEstado.Descricao } : null,
                    DataDisponivelInicio = programacaoVeiculoTMS.DataDisponivelInicio != null && programacaoVeiculoTMS.DataDisponivelInicio.HasValue ? programacaoVeiculoTMS.DataDisponivelInicio.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataDisponivelFim = programacaoVeiculoTMS.DataDisponivel != null && programacaoVeiculoTMS.DataDisponivel.HasValue ? programacaoVeiculoTMS.DataDisponivel.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    programacaoVeiculoTMS.Observacao,
                    programacaoVeiculoTMS.Folga
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarSumarizadores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS repProgramacaoVeiculo = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacaoTMS repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacaoTMS(unitOfWork);

                // Parametros
                

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao> finalidades = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao>();
                finalidades.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao.Veiculo);
                finalidades.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao.Todos);

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoVeiculoTMS filtrosPesquisa = ObterFiltrosPesquisa();

                // Busca informacoes
                int totalRegistros = repProgramacaoVeiculo.ContarConsultarProgramacaoVeiculo(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacaoTMS> situacoesMotorista = repProgramacaoSituacao.BuscarSituacoes(finalidades);

                List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS> listaProgVeiculo = repProgramacaoVeiculo.ConsultarProgramacaoVeiculo(filtrosPesquisa, null, null, 0, 0);

                // Formata retorno
                var retorno = new
                {
                    Status = situacoesMotorista != null ? (from obj in situacoesMotorista
                                                           select new
                                                           {
                                                               Descricao = obj.Descricao,
                                                               Quantidade = listaProgVeiculo.Where(o => o.ProgramacaoSituacao.Codigo == obj.Codigo).Count().ToString("n0")
                                                           }).ToList() : null

                };
                var todos = new
                {
                    Descricao = "Todos",
                    Quantidade = totalRegistros.ToString("n0")
                };
                retorno.Status.Add(todos);

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados sumarizados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS repProgramacaoVeiculo = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacaoTMS repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacaoTMS(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS programacao = null;
                if (codigo > 0)
                    programacao = repProgramacaoVeiculo.BuscarPorCodigo(codigo, true);
                else
                    programacao = new Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS();

                // Valida

                int  codigoMotorista = 0, codigoProgramacaoSituacao = 0, codigoVeiculo = 0, codigoCidadeEstado = 0;
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("ProgramacaoSituacao"), out codigoProgramacaoSituacao);
                int.TryParse(Request.Params("CidadeEstado"), out codigoCidadeEstado);

                DateTime? dataDisponivelInicio = Request.GetNullableDateTimeParam("DataDisponivelInicio");
                DateTime? dataDisponvielFim = Request.GetNullableDateTimeParam("DataDisponivelFim");
                int folga = Request.GetIntParam("Folga");
                string observacao = Request.GetStringParam("Observacao");

                unitOfWork.Start();

                programacao.Observacao = observacao;
                programacao.Folga = folga;
                programacao.DataDisponivelInicio = dataDisponivelInicio;
                programacao.DataDisponivel = dataDisponvielFim;
                programacao.CidadeEstado = codigoCidadeEstado > 0 ? repLocalidade.BuscarPorCodigo(codigoCidadeEstado) : null;
                programacao.Motorista = codigoMotorista > 0 ? repUsuario.BuscarPorCodigo(codigoMotorista) : null;
                programacao.ProgramacaoSituacao = codigoProgramacaoSituacao > 0 ? repProgramacaoSituacao.BuscarPorCodigo(codigoProgramacaoSituacao) : null;
                programacao.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;

                if (codigo > 0)
                    repProgramacaoVeiculo.Atualizar(programacao, Auditado);
                else
                {
                    DateTime dataCriacao = DateTime.Now;
                    programacao.DataCriacaoPlanejamento = dataCriacao;
                    repProgramacaoVeiculo.Inserir(programacao, Auditado);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicioGrid, int limiteGrid, Repositorio.UnitOfWork unitOfWork, ref int novoLimite)
        {
            Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS repProgramacaoVeiculo = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoVeiculoTMS filtrosPesquisa = ObterFiltrosPesquisa();

            PropOrdena(ref propOrdenar);

            List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS> listaGrid = repProgramacaoVeiculo.ConsultarProgramacaoVeiculo(filtrosPesquisa, propOrdenar, dirOrdena, inicioGrid, limiteGrid);
            totalRegistros = repProgramacaoVeiculo.ContarConsultarProgramacaoVeiculo(filtrosPesquisa);

            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Veiculo = obj.Veiculo.Placa,
                             obj.Veiculo.NumeroFrota,
                             Reboques = obj.Veiculo.VeiculosVinculados != null && obj.Veiculo.VeiculosVinculados.Count > 0 ? string.Join(", ", obj.Veiculo.VeiculosVinculados.Select(o => o.Placa)) : string.Empty,
                             Motorista = obj.Motorista?.Nome ?? string.Empty,
                             ModeloCarroceria = obj.Veiculo?.ModeloCarroceria?.Descricao ?? string.Empty,
                             FuncionarioResponsavel = obj.Veiculo?.FuncionarioResponsavel?.Descricao ?? string.Empty,
                             TipoPlotagem = obj.Veiculo?.TipoPlotagem?.Descricao ?? string.Empty,
                             ModeloVeiculo = obj.Veiculo?.Modelo?.Descricao ?? string.Empty,
                             Cidade = obj.CidadeEstado?.Descricao ?? string.Empty,
                             EstadoSigla = obj.CidadeEstado?.Estado?.Descricao ?? string.Empty,
                             EstadoNome = obj.CidadeEstado?.Estado?.Nome ?? string.Empty,
                             obj.Folga,
                             obj.Observacao,
                             Situacao = obj.ProgramacaoSituacao?.Descricao,
                             DataDisponivelInicio = obj.DataDisponivelInicio != null && obj.DataDisponivelInicio.HasValue ? obj.DataDisponivelInicio.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                             DataDisponivel = obj.DataDisponivel != null && obj.DataDisponivel.HasValue ? obj.DataDisponivel.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                             DataCriacaoPlanejamento = obj.DataCriacaoPlanejamento != null && obj.DataCriacaoPlanejamento.HasValue ? obj.DataCriacaoPlanejamento.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                             DT_RowColor = obj.ProgramacaoSituacao != null ? obj.ProgramacaoSituacao.DescricaoCor : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco,
                             DT_FontColor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Black
                         }).ToList();

            return lista.ToList();
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.Prop("Codigo");
            grid.Prop("EstadoNome");
            grid.Prop("Motorista").Nome("Motorista").Align(Models.Grid.Align.left).Tamanho(15);
            grid.Prop("NumeroFrota").Nome("N° Frota").Align(Models.Grid.Align.left).Tamanho(10);
            grid.Prop("Veiculo").Nome("Placa").Align(Models.Grid.Align.left).Tamanho(10);
            grid.Prop("ModeloCarroceria").Nome("Modelo da Carroceria").Align(Models.Grid.Align.left).Tamanho(15);
            grid.Prop("FuncionarioResponsavel").Nome("Funcionário Responsável").Align(Models.Grid.Align.left).Tamanho(15);
            grid.Prop("TipoPlotagem").Nome("Plotagem do Veículo").Align(Models.Grid.Align.left).Tamanho(15);
            grid.Prop("ModeloVeiculo").Nome("Modelo").Align(Models.Grid.Align.left).Tamanho(10);
            grid.Prop("Reboques").Nome("Reboques").Align(Models.Grid.Align.left).Tamanho(10).Ord(false);
            grid.Prop("DataDisponivelInicio").Nome("Data Disponível Inicio").Align(Models.Grid.Align.center).Tamanho(10);
            grid.Prop("DataDisponivel").Nome("Data Disponível Fim").Align(Models.Grid.Align.center).Tamanho(10);
            grid.Prop("DataCriacaoPlanejamento").Nome("Data Criação Planejamento").Align(Models.Grid.Align.center).Tamanho(10);
            grid.Prop("EstadoSigla").Nome("Estado").Align(Models.Grid.Align.center).Tamanho(10).Ord(false);
            grid.Prop("Cidade").Nome("Cidade").Align(Models.Grid.Align.center).Tamanho(10);
            grid.Prop("Folga").Nome("Folga").Align(Models.Grid.Align.center).Tamanho(5);
            grid.Prop("Situacao").Nome("Situação").Align(Models.Grid.Align.center).Tamanho(10);
            grid.Prop("Observacao").Nome("Observação").Align(Models.Grid.Align.center).Tamanho(10);

            return grid;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "NumeroFrota") propOrdenar = "Veiculo.NumeroFrota";
            else if (propOrdenar == "Veiculo") propOrdenar = "Veiculo.Placa";
            else if (propOrdenar == "Motorista") propOrdenar = "Motorista.Nome";
            else if (propOrdenar == "Situacao") propOrdenar = "ProgramacaoSituacao.Descricao";
            else if (propOrdenar == "Cidade") propOrdenar = "CidadeEstado.Descricao";
            else if (propOrdenar == "TipoPlotagem") propOrdenar = "Veiculo.TipoPlotagem.Descricao";
            else if (propOrdenar == "ModeloVeiculo") propOrdenar = "Veiculo.Modelo.Descricao";
            else if (propOrdenar == "ModeloCarroceria") propOrdenar = "Veiculo.ModeloCarroceria.Descricao";
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoVeiculoTMS ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoVeiculoTMS
            {
                NumeroFrota = Request.GetStringParam("NumeroFrota"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoReboque = Request.GetIntParam("Reboque"),
                ModeloVeicular = Request.GetIntParam("ModeloVeicular"),
                Motorista = Request.GetIntParam("Motorista"),
                Situacoes = Request.GetListParam<int>("ProgramacaoSituacao"),
                Estados = Request.GetListParam<string>("Estado"),
                CodigoFuncionarioResponsavelCavalo = Request.GetIntParam("FuncionarioResponsavelCavalo"),
                CodigoMarcaCavalo = Request.GetIntParam("MarcaCavalo"),
                DataCadastroPlanejamentoInicial = Request.GetNullableDateTimeParam("DataCadastroPlanejamentoInicial"),
                DataCadastroPlanejamentoFinal = Request.GetNullableDateTimeParam("DataCadastroPlanejamentoFinal"),
                DataDisponibilidadeInicial = Request.GetNullableDateTimeParam("DataDisponibilidadeInicial"),
                DataDisponibilidadeFinal = Request.GetNullableDateTimeParam("DataDisponibilidadeFinal"),
            };
        }

        #endregion
    }
}
