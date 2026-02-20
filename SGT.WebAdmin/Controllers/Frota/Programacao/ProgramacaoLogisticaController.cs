using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Frota.Programacao
{
    [CustomAuthorize("Frota/ProgramacaoLogistica")]
    public class ProgramacaoLogisticaController : BaseController
    {
		#region Construtores

		public ProgramacaoLogisticaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(bool exportacao = false)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoLogistica filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.Prop("Codigo");
                grid.Prop("Motorista").Nome("Motorista").Align(Models.Grid.Align.left).Tamanho(15);
                grid.Prop("SituacaoColaborador").Nome("Situação").Align(Models.Grid.Align.left).Tamanho(10);
                grid.Prop("PeriodoSituacaoColaborador").Nome("Período").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("Ociosidade").Nome("Ociosidade").Align(Models.Grid.Align.right).Tamanho(10);
                grid.Prop("Veiculos").Nome("Veículos").Align(Models.Grid.Align.left).Tamanho(10);
                grid.Prop("Manutencao").Nome("Manutenção").Align(Models.Grid.Align.left).Tamanho(10);
                grid.Prop("DescricaoSituacaoCarga").Nome("Situação da Carga").Align(Models.Grid.Align.left).Tamanho(10);
                grid.Prop("TipoCarroceria").Nome("Tipo da Carroceria").Align(Models.Grid.Align.left).Tamanho(10);
                grid.Prop("TipoVeiculo").Nome("Tipo do Veículo").Align(Models.Grid.Align.left).Tamanho(15);
                grid.Prop("TipoPlotagem").Nome("Plotagem").Align(Models.Grid.Align.left).Tamanho(8);
                grid.Prop("DescricaDataUltimaEntragaGuarita").Nome("Última Entrada Guarita").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("ObservacaoUltimaEntragaGuarita").Nome("OBS Última Entrada Guarita").Align(Models.Grid.Align.left).Tamanho(15);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                IList<Dominio.ObjetosDeValor.Embarcador.Frota.ProgramacaoLogistica> listaGrid = repUsuario.ConsultarProgramacaoLogistica(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repUsuario.ContarConsultarProgramacaoLogistica(filtrosPesquisa);

                var lista = (from obj in listaGrid
                             select new
                             {
                                 obj.Codigo,
                                 obj.Motorista,
                                 obj.SituacaoColaborador,
                                 obj.PeriodoSituacaoColaborador,
                                 obj.Ociosidade,
                                 obj.Veiculos,
                                 obj.Manutencao,
                                 obj.DescricaoSituacaoCarga,
                                 obj.TipoCarroceria,
                                 obj.TipoVeiculo,
                                 obj.TipoPlotagem,
                                 obj.DescricaDataUltimaEntragaGuarita,
                                 obj.ObservacaoUltimaEntragaGuarita,
                                 DT_RowColor = obj.CorSituacaoColaborador.Descricao(),
                                 DT_FontColor = obj.CorSituacaoColaborador == Cores.VerdeEscuro || obj.CorSituacaoColaborador == Cores.Cinza || obj.CorSituacaoColaborador == Cores.Laranja ? CorGrid.Branco : CorGrid.Black
                             }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());

                if (exportacao)
                {
                    byte[] bArquivo = grid.GerarExcel();

                    if (bArquivo != null)
                        return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                    else
                        return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoGerarArquivo);
                }
                else
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
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigo);

                // Valida
                if (usuario == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    usuario.Codigo,
                    Motorista = new { Codigo = usuario.Codigo, Descricao = usuario.Nome },
                    ProgramacaoAlocacao = usuario.ProgramacaoMotorista != null && usuario.ProgramacaoMotorista.ProgramacaoAlocacao != null ? new { Codigo = usuario.ProgramacaoMotorista.ProgramacaoAlocacao.Codigo, Descricao = usuario.ProgramacaoMotorista.ProgramacaoAlocacao.Descricao } : null,
                    CategoriaCNH = usuario.Categoria,
                    VencimentoCNH = usuario.DataVencimentoHabilitacao != null && usuario.DataVencimentoHabilitacao.HasValue ? usuario.DataVencimentoHabilitacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    VencimentoMoop = usuario.DataVencimentoMoop != null && usuario.DataVencimentoMoop.HasValue ? usuario.DataVencimentoMoop.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataAdmissao = usuario.DataAdmissao != null && usuario.DataAdmissao.HasValue ? usuario.DataAdmissao.Value.ToString("dd/MM/yyyy") : string.Empty
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
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(unitOfWork);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoLogistica filtrosPesquisa = ObterFiltrosPesquisa();

                int totalRegistros = repUsuario.ContarConsultarProgramacaoLogistica(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao> situacoes = repColaboradorSituacao.BuscarSituacoesAtivas();
                List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> segmentos = repSegmentoVeiculo.BuscarTodosAtivos();

                var retorno = new
                {
                    Status = (from obj in situacoes
                              select ObterDetalhesColaboradorSituacao(obj, filtrosPesquisa, repUsuario)).ToList(),
                    StatusManutencao = (from obj in segmentos
                                        select new
                                        {
                                            Descricao = obj.Descricao,
                                            Quantidade = repOrdemServicoFrota.ContarPorSegmentoVeiculoESituacao(obj.Codigo, SituacaoOrdemServicoFrota.EmManutencao).ToString("n0")
                                        }).ToList()
                };
                var todos = new
                {
                    Descricao = "Todos",
                    Quantidade = totalRegistros.ToString("n0")
                };
                var todosManutencao = new
                {
                    Descricao = "Todos",
                    Quantidade = repOrdemServicoFrota.ContarPorSegmentoVeiculoESituacao(0, SituacaoOrdemServicoFrota.EmManutencao).ToString("n0")
                };
                retorno.Status.Add(todos);
                retorno.StatusManutencao.Add(todosManutencao);

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
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoMotorista repProgramacaoMotorista = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoMotorista(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoAlocacao repProgramacaoAlocacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoAlocacao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigo, true);

                // Valida
                if (usuario == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                int codigoProgramacaoAlocacao = 0, codigoMotorista = 0;
                int.TryParse(Request.Params("ProgramacaoAlocacao"), out codigoProgramacaoAlocacao);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);

                bool inserir = true;
                Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista programacao = null;
                if (usuario.ProgramacaoMotorista == null)
                    programacao = new Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista();
                else
                {
                    inserir = false;
                    programacao = repProgramacaoMotorista.BuscarPorCodigo(usuario.ProgramacaoMotorista.Codigo, true);
                }
                unitOfWork.Start();

                programacao.Cliente = null;
                programacao.DataInicioFerias = null;
                programacao.DataFimFerias = null;
                programacao.Empresa = this.Usuario.Empresa;
                programacao.ProgramacaoAlocacao = codigoProgramacaoAlocacao > 0 ? repProgramacaoAlocacao.BuscarPorCodigo(codigoProgramacaoAlocacao) : null;
                programacao.ProgramacaoEspecialidade = null;
                programacao.ProgramacaoSituacao = null;

                if (inserir)
                    repProgramacaoMotorista.Inserir(programacao, Auditado);
                else
                    repProgramacaoMotorista.Atualizar(programacao, Auditado);

                usuario.ProgramacaoMotorista = programacao;
                repUsuario.Atualizar(usuario, Auditado);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                return Pesquisa(true).Result;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterDetalhesColaboradorSituacao(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao situacao, Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoLogistica filtrosPesquisa, Repositorio.Usuario repUsuario)
        {
            filtrosPesquisa.CodigosSituacaoColaborador = new List<int>() { situacao.Codigo };

            return new
            {
                Descricao = situacao.Descricao,
                Quantidade = repUsuario.ContarConsultarProgramacaoLogistica(filtrosPesquisa).ToString("n0")
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoLogistica ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoLogistica()
            {
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoProgramacaoAlocacao = Request.GetIntParam("ProgramacaoAlocacao"),
                CodigosTipoVeiculo = Request.GetListParam<int>("TipoVeiculo"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigosPlotagemVeiculo = Request.GetListParam<int>("PlotagemVeiculo"),
                CodigosTipoCarroceria = Request.GetListParam<int>("TipoCarroceria"),
                CodigosSituacaoColaborador = Request.GetListParam<int>("SituacaoColaborador"),
                SituacaoProgramacaoLogistica = Request.GetEnumParam<SituacaoProgramacaoLogistica>("SituacaoVeiculo"),
                TipoMotorista = Request.GetEnumParam<TipoMotorista>("TipoMotorista"),
                EmViagem = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("EmViagem"),
                EmManutencao = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("EmManutencao"),
                TipoGuarita = Request.GetEnumParam<TipoEntradaSaida>("TipoGuarita"),
                SomenteMotoristaComOciosidade = Request.GetBoolParam("SomenteMotoristaComOciosidade")
            };
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */
            if (propOrdenar == "DescricaoSituacaoCarga") propOrdenar = "SituacaoCarga";
            else if (propOrdenar == "DescricaDataUltimaEntragaGuarita") propOrdenar = "DataUltimaEntragaGuarita";
        }

        #endregion
    }
}
