using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ConsultaDisponibilidadeCarregamento")]
    public class ConsultaDisponibilidadeCarregamentoController : BaseController
    {
		#region Construtores

		public ConsultaDisponibilidadeCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterInformacoesCentroCarregamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);

                // Converte valors
                int codigoCentroCarregamento;
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);

                int codigoRota;
                int.TryParse(Request.Params("Rota"), out codigoRota);

                int codigoFilial;
                int.TryParse(Request.Params("Filial"), out codigoFilial);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento);

                // Rota selecionada
                var rotas = codigoRota == 0 ? (from obj in centroCarregamento.PrevisoesCarregamento select obj.Rota).Distinct() : (from obj in centroCarregamento.PrevisoesCarregamento where obj.Rota.Codigo == codigoRota select obj.Rota).Distinct();

                // Retorna objeto
                var informacoesCentroCarregamento = new
                {
                    centroCarregamento.Codigo,
                    centroCarregamento.Descricao,
                    PeriodosCarregamento = (
                        from periodo in centroCarregamento.PeriodosCarregamento
                        where periodo.ExcecaoCapacidadeCarregamento == null && periodo.ExclusividadeCarregamento == null
                        select new
                        {
                            periodo.Codigo,
                            periodo.Dia,
                            periodo.HoraInicio,
                            periodo.HoraTermino,
                            periodo.ToleranciaExcessoTempo,
                            periodo.CapacidadeCarregamentoSimultaneo,
                            periodo.CapacidadeCarregamentoVolume
                        }
                    ).ToList(),
                    ExcecoesPeriodosCarregamento = (
                        from excecao in centroCarregamento.ExcecoesCapacidadeCarregamento
                        select (
                            from periodo in excecao.PeriodosCarregamento
                            select new
                            {
                                periodo.Codigo,
                                Data = excecao.Data.ToString("dd/MM/yyyy"),
                                periodo.HoraInicio,
                                periodo.HoraTermino,
                                periodo.ToleranciaExcessoTempo,
                                periodo.CapacidadeCarregamentoSimultaneo,
                                periodo.CapacidadeCarregamentoVolume
                            }
                        )
                    ).SelectMany(o => o).ToList(),
                    Rotas = (
                        from obj in rotas
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            Estados = (from estado in obj.Estados select estado.Sigla).ToList(),
                            PrevisoesCarregamento = (
                                from previsao in centroCarregamento.PrevisoesCarregamento
                                where previsao.Rota.Codigo == obj.Codigo
                                select new
                                {
                                    previsao.Dia,
                                    ModelosVeiculos = (
                                        from modelo in previsao.ModelosVeiculos
                                        select new
                                        {
                                            Codigo = modelo.Codigo,
                                            Descricao = modelo.Descricao
                                        }
                                    ).ToList(),
                                    previsao.QuantidadeCargas,
                                    previsao.QuantidadeCargasExcedentes
                                }
                            ).ToList(),
                            ExcecoesPrevisoesCarregamento = (
                                from excecao in centroCarregamento.ExcecoesCapacidadeCarregamento
                                select (
                                    from previsao in excecao.PrevisoesCarregamento
                                    where previsao.Rota.Codigo == obj.Codigo
                                    select new
                                    {
                                        Data = excecao.Data.ToString("dd/MM/yyyy"),
                                        ModelosVeiculos = (from modelo in previsao.ModelosVeiculos
                                                            select new
                                                            {
                                                                Codigo = modelo.Codigo,
                                                                Descricao = modelo.Descricao
                                                            }).ToList(),
                                        previsao.QuantidadeCargas,
                                        previsao.QuantidadeCargasExcedentes
                                    }
                                )
                            ).SelectMany(o => o).ToList()
                        }
                    ).ToList()
                };

                return new JsonpResult(informacoesCentroCarregamento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter as informações do centro de carregamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterDisponibilidadeCarregamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Reposiórios
                Repositorio.Embarcador.Logistica.PrevisaoCarregamento repPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unidadeDeTrabalho);

                // Converte dados
                int codigoRota;
                int.TryParse(Request.Params("Rota"), out codigoRota);

                int codigoCentroCarregamento;
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                // Instancia objeto de retorno
                List<object> retorno = new List<object>();

                // Itera o intervalo de dias
                for (var dia = dataInicial; dia <= dataFinal; dia = dia.AddDays(1))
                {
                    // Busca o dia da semana
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaHelper.ObterDiaSemana(dia);

                    List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento> previsoesCarregamentoDia = null;

                    // Verifica se o centro carrega nesse dia
                    Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = servicoCargaJanelaCarregamentoDisponibilidade.ObterExcecaoCentroCarregamentoPorData(codigoCentroCarregamento, dia);

                    // Se ele carrega
                    if (excecao != null)
                        previsoesCarregamentoDia = repPrevisaoCarregamento.BuscarPorExcecao(excecao.Codigo, codigoRota);
                    else
                        previsoesCarregamentoDia = repPrevisaoCarregamento.BuscarPorCentroCarregamento(codigoCentroCarregamento, codigoRota, diaSemana);

                    retorno.Add(new
                    {
                        Dia = dia.ToString("dd/MM/yyyy"),
                        Previsoes = (from obj in previsoesCarregamentoDia select PrevisoesDia(obj, codigoCentroCarregamento, codigoRota, dia, repCargaJanelaCarregamento, repReservaCargaGrupoPessoa)).ToList()
                    });
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter as disponibilidades de carregamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesCargasReserva()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "Carga", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 30, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destino", "Destino", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Pedidos", "Pedidos", 25, Models.Grid.Align.left, false);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Transportador")
                    propOrdenar = "Carga.Empresa.RazaoSocial";
                if (propOrdenar == "Carga")
                    propOrdenar = "Carga.Codigo";

                // Dados do filtro
                int.TryParse(Request.Params("CentroCarregamento"), out int centroCarregamento);
                int.TryParse(Request.Params("PrevisaoCarregamento"), out int previsaoCarregamento);

                DateTime.TryParse(Request.Params("Data"), out DateTime data);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repReservaCargaGrupoPessoa.GruposReservadosDoDia(centroCarregamento, previsaoCarregamento, data);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargaJanelaCarregamentos = repCargaJanelaCarregamento.BuscarGrupoPessoas(centroCarregamento, data, grupoPessoas, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                int totalRegistros = repCargaJanelaCarregamento.ContarGrupoPessoas(centroCarregamento, data, grupoPessoas);

                var lista = from obj in cargaJanelaCarregamentos
                            select new
                            {
                                Codigo = obj.Codigo,
                                Carga = obj.Carga?.CodigoCargaEmbarcador ?? "",
                                Transportador = obj.Carga?.Empresa?.Descricao ?? "",
                                Destino = obj.Carga?.DadosSumarizados?.Destinos ?? "",
                                Destinatario = obj.Carga?.DadosSumarizados?.Destinatarios ?? "",
                                Pedidos = obj.Carga != null ? string.Join(",", (from ped in obj.Carga.Pedidos select ped.Pedido.NumeroPedidoEmbarcador)) : ""
                            };

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
        public async Task<IActionResult> DetalhesEmReserva()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PrevisaoCarregamento repPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(unitOfWork);
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Reserva", "Reserva", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "Carga", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cliente", "Cliente", 50, Models.Grid.Align.left, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Dados do filtro
                int.TryParse(Request.Params("CentroCarregamento"), out int centroCarregamento);
                int.TryParse(Request.Params("Rota"), out int rota);

                DateTime.TryParse(Request.Params("Data"), out DateTime data);


                int.TryParse(Request.Params("PrevisaoCarregamento"), out int codigoPrevisaoCarregamento);

                Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsaoCarregamento = repPrevisaoCarregamento.BuscarPorCodigo(codigoPrevisaoCarregamento);

                // Consulta
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaGrid = repCargaJanelaCarregamento.BuscarReservadasDoDia(centroCarregamento, rota, data, previsaoCarregamento.ModelosVeiculos.Select(o => o.Codigo).ToArray(), propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCargaJanelaCarregamento.ContarReservadasDoDia(centroCarregamento, rota, data, previsaoCarregamento.ModelosVeiculos.Select(o => o.Codigo).ToArray());

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                Reserva = obj.PreCarga.NumeroPreCarga,
                                Carga = obj.Carga?.CodigoCargaEmbarcador ?? "",
                                Cliente = string.Join(",", (from cli in obj.PreCarga.Destinatarios select cli.Descricao).ToList())
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
        public async Task<IActionResult> DetalhesReserva()
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

                // Dados do filtro
                int.TryParse(Request.Params("CentroCarregamento"), out int centroCarregamento);
                int.TryParse(Request.Params("PrevisaoCarregamento"), out int previsaoCarregamento);

                DateTime.TryParse(Request.Params("Data"), out DateTime data);

                // Consulta
                List<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa> listaGrid = repReservaCargaGrupoPessoa.ReservasDoDia(centroCarregamento, previsaoCarregamento, data, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repReservaCargaGrupoPessoa.ContarReservasDoDia(centroCarregamento, previsaoCarregamento, data);

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

        #endregion

        #region Métodos Privados

        /* Lucas Mahle:
         * Sim, to passando os repositorios por parametro. Eu sei que isso é feio, mas não estou afim de instanciar os repositorios toda 
         * linha dos resultados,
         * 
         * Se tiver uma ideia melhor, sinta-se a vontade
         */
        private dynamic PrevisoesDia(Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento obj, int codigoCentroCarregamento, int codigoRota, DateTime dia, Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento, Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repReservaCargaGrupoPessoa)
        {
            int ocupadas = repCargaJanelaCarregamento.ContarCargasPorRotaOcupadas(codigoCentroCarregamento, codigoRota, dia, obj.ModelosVeiculos.Select(o => o.Codigo).ToArray());
            int emReservadas = repCargaJanelaCarregamento.ContarEmReservadasCargasPorRota(codigoCentroCarregamento, codigoRota, dia, obj.ModelosVeiculos.Select(o => o.Codigo).ToArray());
            int reservasDeGrupo = repReservaCargaGrupoPessoa.ContarReservasPorDia(codigoCentroCarregamento, obj.Codigo, dia);
            int livres = obj.QuantidadeCargas - emReservadas - reservasDeGrupo;

            return new
            {
                Data = dia.ToString("dd/MM/yyyy"),
                PrevisaoCarregamento = obj.Codigo,
                obj.Descricao,
                obj.QuantidadeCargas,
                Rota = obj.Rota.Codigo,
                //Excedidas = excedidas,
                //Reservadas = reservadas,
                Ocupadas = ocupadas,
                EmReservas = emReservadas,
                ReservasDeGrupo = reservasDeGrupo,
                Livres = livres > 0 ? livres : 0
            };
        }
        
        #endregion
    }
}
