using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ReservaCargaGrupoPessoa")]
    public class ReservaCargaGrupoPessoaController : BaseController
    {
		#region Construtores

		public ReservaCargaGrupoPessoaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data Reserva", "DataReserva", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Centro de Carregamento", "CentroCarregamento", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo Pessoas", "GrupoPessoas", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Previsão Carregamento", "PrevisaoCarregamento", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Quantidade Reservada", "QuantidadeReservada", 20, Models.Grid.Align.right, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                int centroCarregamento = 0;
                int.TryParse(Request.Params("CentroCarregamento"), out centroCarregamento);

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, DateTimeStyles.None, out dataInicio);

                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, DateTimeStyles.None, out dataFim);

                // Consulta
                List<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa> listaGrid = repReservaCargaGrupoPessoa.Consultar(dataInicio, dataFim, centroCarregamento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repReservaCargaGrupoPessoa.ContarConsulta(dataInicio, dataFim, centroCarregamento);

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                DataReserva = obj.DataReserva.ToString("dd/MM/yyyy"),
                                CentroCarregamento = obj.CentroCarregamento.Descricao,
                                GrupoPessoas = obj.GrupoPessoas.Descricao,
                                PrevisaoCarregamento = obj.PrevisaoCarregamento.Descricao,
                                obj.QuantidadeReservada,
                            };

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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
        public async Task<IActionResult> PrevisoesDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Rota", "Rota", 33, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descrição", "DescricaoPrevisao", 33, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Quantidade de Cargas", "QuantidadeCargas", 33, Models.Grid.Align.right, false);

                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                DateTime dataReserva = Request.GetDateTimeParam("DataReserva");

                if (codigoCentroCarregamento == 0)
                    return new JsonpResult(false, "Nenhum centro de carregamento selecionado.");

                if (dataReserva == DateTime.MinValue)
                    return new JsonpResult(false, "Nenhuma data selecionada.");

                string descricao = Request.GetStringParam("Descricao");
                string descricaoRota = Request.GetStringParam("Rota");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana dia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaHelper.ObterDiaSemana(dataReserva);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = servicoCargaJanelaCarregamentoDisponibilidade.ObterExcecaoCentroCarregamentoPorData(codigoCentroCarregamento, dataReserva);

                Repositorio.Embarcador.Logistica.PrevisaoCarregamento repositorioPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento> previsoes = repositorioPrevisaoCarregamento.ConsultaPrevisoes(codigoCentroCarregamento, excecao?.Codigo ?? 0, dia, descricaoRota, descricao);

                var previsoesRetornar = (
                    from previsao in previsoes
                    select new
                    {
                        previsao.Codigo,
                        Descricao = String.Join(" - ", new string[] { previsao.Rota.Descricao, previsao.Descricao }),
                        Rota = previsao.Rota.Descricao,
                        DescricaoPrevisao = previsao.Descricao,
                        previsao.QuantidadeCargas
                    }
                ).ToList();

                grid.setarQuantidadeTotal(previsoes.Count);
                grid.AdicionaRows(previsoesRetornar);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa reserva = repReservaCargaGrupoPessoa.BuscarPorCodigo(codigo);

                // Valida
                if (reserva == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    reserva.Codigo,
                    CentroCarregamento = new { reserva.CentroCarregamento.Codigo, reserva.CentroCarregamento.Descricao },
                    GrupoPessoa = new { reserva.GrupoPessoas.Codigo, reserva.GrupoPessoas.Descricao },
                    PrevisaoCarregamento = new { reserva.PrevisaoCarregamento.Codigo, Descricao = String.Join(" - ", new string[] { reserva.PrevisaoCarregamento.Rota.Descricao, reserva.PrevisaoCarregamento.Descricao }) },
                    DataReserva = reserva.DataReserva.ToString("dd/MM/yyyy"),
                    reserva.QuantidadeReservada,
                    QuantidadeReservadaCount = reserva.PrevisaoCarregamento.QuantidadeCargas
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa reserva = new Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa();

                // Preenche entidade com dados
                PreencheEntidade(ref reserva, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(reserva, out erro))
                    return new JsonpResult(false, true, erro);

                // Verifica disponibilidade
                if (!VerificaDisponibilidade(reserva, unitOfWork))
                    return new JsonpResult(false, true, "Não há disponibilidade para reserva nesse dia para essa rota.");

                // Persiste dados
                repReservaCargaGrupoPessoa.Inserir(reserva, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa reserva = repReservaCargaGrupoPessoa.BuscarPorCodigo(codigo, true);

                // Valida
                if (reserva == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref reserva, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(reserva, out erro))
                    return new JsonpResult(false, true, erro);

                // Verifica disponibilidade
                if (!VerificaDisponibilidade(reserva, unitOfWork))
                    return new JsonpResult(false, true, "Não há disponibilidade para reserva nesse dia para essa rota.");

                // Persiste dados
                repReservaCargaGrupoPessoa.Atualizar(reserva, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa reserva = repReservaCargaGrupoPessoa.BuscarPorCodigo(codigo);

                // Valida
                if (reserva == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repReservaCargaGrupoPessoa.Deletar(reserva, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa reserva, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
            * Recebe uma instancia da entidade
            * Converte parametros recebido por request
            * Atribui a entidade
            */

            // Instancia Repositorios
            Repositorio.Embarcador.Logistica.PrevisaoCarregamento repPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            // Converte valores
            int codigoCentroCarregamento = 0;
            int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento);

            int codigoGrupoPessoa = 0;
            int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa);

            int codigoPrevisaoCarregamento = 0;
            int.TryParse(Request.Params("PrevisaoCarregamento"), out codigoPrevisaoCarregamento);
            Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsaoCarregamento = repPrevisaoCarregamento.BuscarPorCodigo(codigoPrevisaoCarregamento);

            DateTime dataReserva;
            DateTime.TryParseExact(Request.Params("DataReserva"), "dd/MM/yyyy", null, DateTimeStyles.None, out dataReserva);

            int quantidadeReservada = 0;
            int.TryParse(Request.Params("QuantidadeReservada"), out quantidadeReservada);

            // Vincula dados
            reserva.CentroCarregamento = centroCarregamento;
            reserva.DataReserva = dataReserva;
            reserva.GrupoPessoas = grupoPessoa;
            reserva.PrevisaoCarregamento = previsaoCarregamento;
            reserva.QuantidadeReservada = quantidadeReservada;

            // Dados Criacao 
            if (reserva.Codigo == 0)
            {
            }
        }

        private bool VerificaDisponibilidade(Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa reserva, Repositorio.UnitOfWork unitOfWork)
        {
            // Busca todas reservas para essa previsão sumarizando o número de cargas reservada
            Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unitOfWork);
            int totalReservado = repReservaCargaGrupoPessoa.TotalReservadoPrevisao(reserva.PrevisaoCarregamento.Codigo, reserva.DataReserva, reserva.Codigo);

            // Compara valores
            if (reserva.QuantidadeReservada > (reserva.PrevisaoCarregamento.QuantidadeCargas - totalReservado))
                return false;
            else
                return true;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa reserva, out string msgErro)
        {
            /* ValidaEntidade
            * Recebe uma instancia da entidade
            * Valida informacoes
            * Retorna de entidade e valida ou nao e retorna erro (se tiver)
            */
            msgErro = "";

            if (reserva.GrupoPessoas == null)
            {
                msgErro = "Grupo de Pessoas é obrigatório.";
                return false;
            }

            if (reserva.PrevisaoCarregamento == null)
            {
                msgErro = "Previsão de Carregamento é obrigatório.";
                return false;
            }

            if (reserva.DataReserva == DateTime.MinValue)
            {
                msgErro = "Data da Reserva é obrigatória.";
                return false;
            }

            if (reserva.PrevisaoCarregamento.ExcecaoCapacidadeCarregamento == null)
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana dia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaHelper.ObterDiaSemana(reserva.DataReserva);

                if (reserva.PrevisaoCarregamento.Dia != dia)
                {
                    msgErro = "A data da reserva não é compatível com a previsão selecionada.";
                    return false;
                }
            }
            else if (reserva.PrevisaoCarregamento.ExcecaoCapacidadeCarregamento.Data.Date != reserva.DataReserva.Date)
            {
                msgErro = "A data da reserva não é compatível com a previsão selecionada.";
                return false;
            }
            

            if (reserva.QuantidadeReservada <= 0)
            {
                msgErro = "Quantidade Reservada deve ser maior que zero.";
                return false;
            }

            if (reserva.QuantidadeReservada > reserva.PrevisaoCarregamento.QuantidadeCargas)
            {
                msgErro = "Quantidade Reservada não pode ultrapassar " + reserva.PrevisaoCarregamento.QuantidadeCargas.ToString() + ".";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
            * Recebe o campo ordenado na grid
            * Retorna o elemento especifico da entidade para ordenacao
            */
        }

        #endregion
    }
}
