using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/ValorParametroOcorrencia")]
    public class ValorParametroOcorrenciaController : BaseController
    {
		#region Construtores

		public ValorParametroOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia repValorParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia valorParametroOcorrencia = repValorParametroOcorrencia.BuscarPorCodigo(codigo);

                // Valida
                if (valorParametroOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia horaExtra = valorParametroOcorrencia.ValorParametroHoraExtraOcorrencia;
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia estadia = valorParametroOcorrencia.ValorParametroEstadiaOcorrencia;
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia pernoite = valorParametroOcorrencia.ValorParametroPernoiteOcorrencia;
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia escolta = valorParametroOcorrencia.ValorParametroEscoltaOcorrencia;

                // Formata retorno
                var retorno = new
                {
                    valorParametroOcorrencia.Codigo,
                    valorParametroOcorrencia.TipoPessoa,
                    Pessoa = valorParametroOcorrencia.Pessoa != null ? new { valorParametroOcorrencia.Pessoa.Codigo, valorParametroOcorrencia.Pessoa.Descricao } : null,
                    GrupoPessoa = valorParametroOcorrencia.GrupoPessoas != null ? new { valorParametroOcorrencia.GrupoPessoas.Codigo, valorParametroOcorrencia.GrupoPessoas.Descricao } : null,
                    VigenciaInicial = valorParametroOcorrencia.VigenciaInicial.ToString("dd/MM/yyyy"),
                    VigenciaFinal = valorParametroOcorrencia.VigenciaFinal.ToString("dd/MM/yyyy"),
                    valorParametroOcorrencia.Observacao,

                    HoraExtraComponenteFreteAjudante = horaExtra.ComponenteFreteAjudante != null ? new { horaExtra.ComponenteFreteAjudante.Codigo, horaExtra.ComponenteFreteAjudante.Descricao } : null,
                    HoraExtraComponenteFreteVeiculo = horaExtra.ComponenteFreteVeiculo != null ? new { horaExtra.ComponenteFreteVeiculo.Codigo, horaExtra.ComponenteFreteVeiculo.Descricao } : null,
                    HoraExtraTipoOcorrencia = horaExtra.TipoOcorrencia != null ? new { horaExtra.TipoOcorrencia.Codigo, horaExtra.TipoOcorrencia.Descricao } : null,

                    EstadiaComponenteFreteAjudante = estadia.ComponenteFreteAjudante != null ? new { estadia.ComponenteFreteAjudante.Codigo, estadia.ComponenteFreteAjudante.Descricao } : null,
                    EstadiaComponenteFreteVeiculo = estadia.ComponenteFreteVeiculo != null ? new { estadia.ComponenteFreteVeiculo.Codigo, estadia.ComponenteFreteVeiculo.Descricao } : null,
                    EstadiaTipoOcorrencia = estadia.TipoOcorrencia != null ? new { estadia.TipoOcorrencia.Codigo, estadia.TipoOcorrencia.Descricao } : null,

                    PernoiteTipoOcorrencia = pernoite.TipoOcorrencia != null ? new { pernoite.TipoOcorrencia.Codigo, pernoite.TipoOcorrencia.Descricao } : null,
                    PernoiteComponenteFrete = pernoite.ComponenteFrete != null ? new { pernoite.ComponenteFrete.Codigo, pernoite.ComponenteFrete.Descricao } : null,

                    EscoltaTipoOcorrencia = escolta.TipoOcorrencia != null ? new { escolta.TipoOcorrencia.Codigo, escolta.TipoOcorrencia.Descricao } : null,
                    EscoltaComponenteFrete = escolta.ComponenteFrete != null ? new { escolta.ComponenteFrete.Codigo, escolta.ComponenteFrete.Descricao } : null,

                    EscoltaHoraContratada = escolta.HoraContratado.HasValue ? escolta.HoraContratado.Value.ToString(@"hh\:mm") : string.Empty,
                    EscoltaValorHoraExcedente = escolta.ValorHoraExcedente.ToString("n2"),
                    EscoltaKmContratada = escolta.KmContratado,
                    EscoltaValorKmExcedente = escolta.ValorKmExcedente.ToString("n2"),

                    TiposOperacao = (from obj in valorParametroOcorrencia.TiposOperacao
                                     select new
                                     {
                                         TIPOOPERACAO = new
                                         {
                                             obj.TipoOperacao.Codigo,
                                             obj.TipoOperacao.Descricao
                                         }
                                     }).ToList(),

                    HoraExtraAjudantes = (from o in horaExtra.Ajudantes
                                          select new
                                          {
                                              o.Codigo,
                                              HoraInicial = o.HoraInicial.ToString(@"hh\:mm"),
                                              HoraFinal = o.HoraFinal.ToString(@"hh\:mm"),
                                              Valor = o.Valor.ToString("n2"),
                                          }).ToList(),
                    HoraExtraVeiculos = (from o in horaExtra.Veiculos
                                         select new
                                         {
                                             o.Codigo,
                                             ModeloVeicular = new { o.ModeloVeicular.Codigo, o.ModeloVeicular.Descricao },
                                             HoraInicial = o.HoraInicial.ToString(@"hh\:mm"),
                                             HoraFinal = o.HoraFinal.ToString(@"hh\:mm"),
                                             Valor = o.Valor.ToString("n2"),
                                         }).ToList(),
                    EstadiaAjudantes = (from o in estadia.Ajudantes
                                        select new
                                        {
                                            o.Codigo,
                                            HoraInicial = o.HoraInicial.ToString(@"hh\:mm"),
                                            HoraFinal = o.HoraFinal.ToString(@"hh\:mm"),
                                            Valor = o.Valor.ToString("n2"),
                                        }).ToList(),
                    EstadiaVeiculos = (from o in estadia.Veiculos
                                       select new
                                       {
                                           o.Codigo,
                                           ModeloVeicular = new { o.ModeloVeicular.Codigo, o.ModeloVeicular.Descricao },
                                           HoraInicial = o.HoraInicial.ToString(@"hh\:mm"),
                                           HoraFinal = o.HoraFinal.ToString(@"hh\:mm"),
                                           Valor = o.Valor.ToString("n2"),
                                       }).ToList(),
                    PernoiteValores = (from o in pernoite.Veiculos
                                       select new
                                       {
                                           o.Codigo,
                                           ModeloVeicular = new { o.ModeloVeicular.Codigo, o.ModeloVeicular.Descricao },
                                           Valor = o.Valor.ToString("n2"),
                                       }).ToList()

                    //EscoltaValores = (from o in escolta.Valores
                    //                  select new
                    //                  {
                    //                      o.Codigo,
                    //                      HoraInicial = o.HoraInicial.ToString(@"hh\:mm"),
                    //                      HoraFinal = o.HoraFinal.ToString(@"hh\:mm"),
                    //                      ValorPeriodo = o.ValorPeriodo.ToString("n2"),
                    //                      ValorHoraExcedente = o.ValorHoraExcedente.ToString("n2"),
                    //                      KmRodado = o.KmRodado,
                    //                      ValorFaixaKm = o.ValorFaixaKm.ToString("n2"),
                    //                      ValorFaixaKmExcedente = o.ValorFaixaKmExcedente.ToString("n2"),
                    //                  }).ToList()
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
                Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia repValorParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia repValorParametroHoraExtraOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia repValorParametroEstadiaOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia repValorParametroPernoiteOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia repValorParametroEscoltaOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia valorParametroOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia();
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia valorParametroHoraExtraOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia()
                {
                    ValorParametroOcorrencia = valorParametroOcorrencia
                };
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia valorParametroEstadiaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia()
                {
                    ValorParametroOcorrencia = valorParametroOcorrencia
                };
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia valorParametroPernoiteOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia()
                {
                    ValorParametroOcorrencia = valorParametroOcorrencia
                };
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia valorParametroEscoltaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia()
                {
                    ValorParametroOcorrencia = valorParametroOcorrencia
                };

                // Preenche entidade com dados
                PreencheEntidade(ref valorParametroOcorrencia, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(valorParametroOcorrencia, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repValorParametroOcorrencia.Inserir(valorParametroOcorrencia, Auditado);

                PreencheEntidadeHoraExtra(ref valorParametroHoraExtraOcorrencia, unitOfWork);
                PreencheEntidadeEstadia(ref valorParametroEstadiaOcorrencia, unitOfWork);
                PreencheEntidadePernoite(ref valorParametroPernoiteOcorrencia, unitOfWork);
                PreencheEntidadeEscolta(ref valorParametroEscoltaOcorrencia, unitOfWork);

                repValorParametroHoraExtraOcorrencia.Inserir(valorParametroHoraExtraOcorrencia);
                repValorParametroEstadiaOcorrencia.Inserir(valorParametroEstadiaOcorrencia);
                repValorParametroPernoiteOcorrencia.Inserir(valorParametroPernoiteOcorrencia);
                repValorParametroEscoltaOcorrencia.Inserir(valorParametroEscoltaOcorrencia);

                valorParametroOcorrencia.ValorParametroHoraExtraOcorrencia = valorParametroHoraExtraOcorrencia;
                valorParametroOcorrencia.ValorParametroEstadiaOcorrencia = valorParametroEstadiaOcorrencia;
                valorParametroOcorrencia.ValorParametroPernoiteOcorrencia = valorParametroPernoiteOcorrencia;
                valorParametroOcorrencia.ValorParametroEscoltaOcorrencia = valorParametroEscoltaOcorrencia;

                repValorParametroOcorrencia.Atualizar(valorParametroOcorrencia);

                SalvarVeiculosHoraExtra(ref valorParametroHoraExtraOcorrencia, unitOfWork);
                SalvarAjudantesHoraExtra(ref valorParametroHoraExtraOcorrencia, unitOfWork);

                SalvarVeiculosEstadia(ref valorParametroEstadiaOcorrencia, unitOfWork);
                SalvarAjudantesEstadia(ref valorParametroEstadiaOcorrencia, unitOfWork);

                SalvarVeiculosPernoite(ref valorParametroPernoiteOcorrencia, unitOfWork);

                if (!ValidaAbas(valorParametroOcorrencia, out string erroMsg, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erroMsg);
                }

                SalvarTiposOperacao(valorParametroOcorrencia, unitOfWork);

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
                Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia repValorParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia repValorParametroHoraExtraOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia repValorParametroEstadiaOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia repValorParametroPernoiteOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia repValorParametroEscoltaOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia valorParametroOcorrencia = repValorParametroOcorrencia.BuscarPorCodigo(codigo, true);

                // Valida
                if (valorParametroOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref valorParametroOcorrencia, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(valorParametroOcorrencia, out string erro))
                    return new JsonpResult(false, true, erro);

                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia valorParametroHoraExtraOcorrencia = valorParametroOcorrencia.ValorParametroHoraExtraOcorrencia;
                valorParametroHoraExtraOcorrencia.Initialize();
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia valorParametroEstadiaOcorrencia = valorParametroOcorrencia.ValorParametroEstadiaOcorrencia;
                valorParametroEstadiaOcorrencia.Initialize();
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia valorParametroPernoiteOcorrencia = valorParametroOcorrencia.ValorParametroPernoiteOcorrencia;
                valorParametroPernoiteOcorrencia.Initialize();
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia valorParametroEscoltaOcorrencia = valorParametroOcorrencia.ValorParametroEscoltaOcorrencia;
                valorParametroEscoltaOcorrencia.Initialize();

                // Persiste dados
                PreencheEntidadeHoraExtra(ref valorParametroHoraExtraOcorrencia, unitOfWork);
                SalvarVeiculosHoraExtra(ref valorParametroHoraExtraOcorrencia, unitOfWork);
                SalvarAjudantesHoraExtra(ref valorParametroHoraExtraOcorrencia, unitOfWork);

                PreencheEntidadeEstadia(ref valorParametroEstadiaOcorrencia, unitOfWork);
                SalvarVeiculosEstadia(ref valorParametroEstadiaOcorrencia, unitOfWork);
                SalvarAjudantesEstadia(ref valorParametroEstadiaOcorrencia, unitOfWork);

                PreencheEntidadePernoite(ref valorParametroPernoiteOcorrencia, unitOfWork);
                SalvarVeiculosPernoite(ref valorParametroPernoiteOcorrencia, unitOfWork);

                PreencheEntidadeEscolta(ref valorParametroEscoltaOcorrencia, unitOfWork);

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repValorParametroOcorrencia.Atualizar(valorParametroOcorrencia, Auditado);

                repValorParametroHoraExtraOcorrencia.Atualizar(valorParametroHoraExtraOcorrencia, Auditado, historico);
                repValorParametroEstadiaOcorrencia.Atualizar(valorParametroEstadiaOcorrencia, Auditado, historico);
                repValorParametroPernoiteOcorrencia.Atualizar(valorParametroPernoiteOcorrencia, Auditado, historico);
                repValorParametroEscoltaOcorrencia.Atualizar(valorParametroEscoltaOcorrencia, Auditado, historico);

                if (!ValidaAbas(valorParametroOcorrencia, out string erroMsg, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erroMsg);
                }

                SalvarTiposOperacao(valorParametroOcorrencia, unitOfWork);

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
                Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia repValorParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia valorParametroOcorrencia = repValorParametroOcorrencia.BuscarPorCodigo(codigo);

                // Valida
                if (valorParametroOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repValorParametroOcorrencia.Deletar(valorParametroOcorrencia);
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


        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("PessoaGrupoPessoa").Nome("Pessoa / Grupo Pessoa").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("TipoOperacao").Nome("Tipos de Operação").Tamanho(20).Align(Models.Grid.Align.left).Ord(false);
            grid.Prop("Vigencia").Nome("Vigência").Tamanho(20).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia repValorParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia(unitOfWork);

            // Dados do filtro
            double.TryParse(Request.Params("Pessoa"), out double pessoa);
            int.TryParse(Request.Params("GrupoPessoa"), out int grupoPessoa);
            int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);

            // Consulta
            List<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia> listaGrid = repValorParametroOcorrencia.Consultar(tipoOperacao, pessoa, grupoPessoa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repValorParametroOcorrencia.ContarConsulta(tipoOperacao, pessoa, grupoPessoa);

            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             PessoaGrupoPessoa = obj.DescricaoPessoaGrupoPessoa,
                             TipoOperacao = obj.TiposOperacao != null && obj.TiposOperacao.Count > 0 ? string.Join(", ", obj.TiposOperacao.Select(o => o.TipoOperacao.Descricao).ToList()) : string.Empty,
                             Vigencia = obj.VigenciaInicial.ToString("dd/MM/yyyy") + " - " + obj.VigenciaFinal.ToString("dd/MM/yyyy")
                         }).ToList();

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia valorParametroOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            // Converte valores
            int.TryParse(Request.Params("GrupoPessoa"), out int grupoPessoas);

            double.TryParse(Request.Params("Pessoa"), out double pessoa);

            string observacao = Request.Params("Observacao") ?? string.Empty;

            Enum.TryParse(Request.Params("TipoPessoa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa);

            DateTime.TryParse(Request.Params("VigenciaInicial"), out DateTime vigenciaInicial);
            DateTime.TryParse(Request.Params("VigenciaFinal"), out DateTime vigenciaFinal);

            // Vincula dados
            valorParametroOcorrencia.Observacao = observacao;
            valorParametroOcorrencia.TipoPessoa = tipoPessoa;
            valorParametroOcorrencia.GrupoPessoas = tipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa ? repGrupoPessoas.BuscarPorCodigo(grupoPessoas) : null;
            valorParametroOcorrencia.Pessoa = tipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa && pessoa > 0 ? repCliente.BuscarPorCPFCNPJ(pessoa) : null;
            valorParametroOcorrencia.VigenciaFinal = vigenciaFinal;
            valorParametroOcorrencia.VigenciaInicial = vigenciaInicial;
        }

        private void SalvarTiposOperacao(Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia valorParametroOcorrencia, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Ocorrencias.ValorParametroTipoOperacao repValorParametroTipoOperacao = new Repositorio.Embarcador.Ocorrencias.ValorParametroTipoOperacao(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeTrabalho);

            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));
            if (valorParametroOcorrencia.TiposOperacao != null && valorParametroOcorrencia.TiposOperacao.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var tipoOperacao in tiposOperacao)
                    if (tipoOperacao.TIPOOPERACAO.Codigo != null)
                        codigos.Add((int)tipoOperacao.TIPOOPERACAO.Codigo);

                List<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroTipoOperacao> valorParametroTipoOperacaoDeletar = (from obj in valorParametroOcorrencia.TiposOperacao where !codigos.Contains(obj.TipoOperacao.Codigo) select obj).ToList();

                for (var i = 0; i < valorParametroTipoOperacaoDeletar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroTipoOperacao valorParametroTipoOperacao = valorParametroTipoOperacaoDeletar[i];

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, valorParametroTipoOperacao.ValorParametroOcorrencia, null, "Excluiu o tipo de operação: " + valorParametroTipoOperacao.TipoOperacao.Descricao, unidadeTrabalho);
                    repValorParametroTipoOperacao.Deletar(valorParametroTipoOperacao);
                }
            }
            else
                valorParametroOcorrencia.TiposOperacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroTipoOperacao>();

            foreach (var tipoOperacao in tiposOperacao)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroTipoOperacao valorParametroTipoOperacao = tipoOperacao.TIPOOPERACAO.Codigo != null ? repValorParametroTipoOperacao.BuscarPorTipoOperacaoEValorParametro((int)tipoOperacao.TIPOOPERACAO.Codigo, valorParametroOcorrencia.Codigo) : null;
                if (valorParametroTipoOperacao == null)
                {
                    valorParametroTipoOperacao = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroTipoOperacao();

                    int.TryParse((string)tipoOperacao.TIPOOPERACAO.Codigo, out int codigoTipoOperacao);
                    valorParametroTipoOperacao.TipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);
                    valorParametroTipoOperacao.ValorParametroOcorrencia = valorParametroOcorrencia;

                    repValorParametroTipoOperacao.Inserir(valorParametroTipoOperacao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, valorParametroTipoOperacao.ValorParametroOcorrencia, null, "Adicionou o tipo de operação: " + valorParametroTipoOperacao.TipoOperacao.Descricao, unidadeTrabalho);
                }
            }
        }

        private void PreencheEntidadeEscolta(ref Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia valorParametroEscoltaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            // Converte valores
            int.TryParse(Request.Params("EscoltaTipoOcorrencia"), out int tipoOcorrencia);
            int.TryParse(Request.Params("EscoltaComponenteFrete"), out int componenteFrete);

            //int.TryParse(Request.Params("EscoltaHoraContratada"), out int horaContratada);
            TimeSpan.TryParse(Request.Params("EscoltaHoraContratada"), out TimeSpan horaContratada);
            int.TryParse(Request.Params("EscoltaKmContratada"), out int kmContratada);

            //decimal.TryParse(Request.Params("EscoltaValorHoraExcedente"), out decimal valorHoraExcedente);
            //decimal.TryParse(Request.Params("EscoltaValorKmExcedente"), out decimal valorKmExcedente);

            decimal valorHoraExcedente = Utilidades.Decimal.Converter((string)Request.Params("EscoltaValorHoraExcedente"));
            decimal valorKmExcedente = Utilidades.Decimal.Converter((string)Request.Params("EscoltaValorKmExcedente"));

            // Vincula dados
            valorParametroEscoltaOcorrencia.TipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(tipoOcorrencia);
            valorParametroEscoltaOcorrencia.ComponenteFrete = repComponenteFrete.BuscarPorCodigo(componenteFrete);
            valorParametroEscoltaOcorrencia.HoraContratado = horaContratada;
            valorParametroEscoltaOcorrencia.ValorHoraExcedente = valorHoraExcedente;
            valorParametroEscoltaOcorrencia.ValorKmExcedente = valorKmExcedente;
            valorParametroEscoltaOcorrencia.KmContratado = kmContratada;
        }




        private void PreencheEntidadePernoite(ref Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia valorParametroPernoiteOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            // Converte valores
            int.TryParse(Request.Params("PernoiteTipoOcorrencia"), out int tipoOcorrencia);
            int.TryParse(Request.Params("PernoiteComponenteFrete"), out int componenteFrete);

            // Vincula dados
            valorParametroPernoiteOcorrencia.TipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(tipoOcorrencia);
            valorParametroPernoiteOcorrencia.ComponenteFrete = repComponenteFrete.BuscarPorCodigo(componenteFrete);
        }
        private void SalvarVeiculosPernoite(ref Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia valorParametroPernoiteOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo repValorParametroPernoiteVeiculo = new Repositorio.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            List<dynamic> veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("PernoiteValores"));
            if (veiculos == null) return;

            List<int> codigosRegistros = new List<int>();
            foreach (dynamic codigo in veiculos)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosRegistros.Add(intcodigo);
            }
            codigosRegistros = codigosRegistros.Where(o => o > 0).Distinct().ToList();

            List<int> registrosParaExcluir = repValorParametroPernoiteVeiculo.BuscarItensNaoPesentesNaLista(valorParametroPernoiteOcorrencia.Codigo, codigosRegistros);

            foreach (dynamic dynVeiculo in veiculos)
            {
                int.TryParse((string)dynVeiculo.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo veiculo = repValorParametroPernoiteVeiculo.BuscarPorParametroPernoiteECodigo(valorParametroPernoiteOcorrencia.Codigo, codigo);

                if (veiculo == null)
                    veiculo = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo();
                else
                    veiculo.Initialize();

                //decimal.TryParse((string)dynVeiculo.Valor, out decimal valor);
                decimal valor = Utilidades.Decimal.Converter((string)dynVeiculo.Valor);

                veiculo.ValorParametroPernoiteOcorrencia = valorParametroPernoiteOcorrencia;
                veiculo.ModeloVeicular = repModeloVeicularCarga.BuscarPorCodigo((int)dynVeiculo.ModeloVeicular.Codigo);
                veiculo.Valor = valor;

                if (veiculo.ModeloVeicular != null)
                {
                    if (veiculo.Codigo == 0)
                        repValorParametroPernoiteVeiculo.Inserir(veiculo);
                    else
                        repValorParametroPernoiteVeiculo.Atualizar(veiculo, Auditado);
                }
            }

            foreach (int codigoRegistro in registrosParaExcluir)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo registro = repValorParametroPernoiteVeiculo.BuscarPorParametroPernoiteECodigo(valorParametroPernoiteOcorrencia.Codigo, codigoRegistro);
                if (registro != null) repValorParametroPernoiteVeiculo.Deletar(registro, Auditado);
            }
        }





        private void PreencheEntidadeEstadia(ref Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia valorParametroEstadiaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            // Converte valores
            int.TryParse(Request.Params("EstadiaTipoOcorrencia"), out int tipoOcorrencia);
            int.TryParse(Request.Params("EstadiaComponenteFreteAjudante"), out int componenteFreteAjudante);
            int.TryParse(Request.Params("EstadiaComponenteFreteVeiculo"), out int componenteFreteVeiculo);

            // Vincula dados
            valorParametroEstadiaOcorrencia.TipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(tipoOcorrencia);
            valorParametroEstadiaOcorrencia.ComponenteFreteAjudante = repComponenteFrete.BuscarPorCodigo(componenteFreteAjudante);
            valorParametroEstadiaOcorrencia.ComponenteFreteVeiculo = repComponenteFrete.BuscarPorCodigo(componenteFreteVeiculo);
        }
        private void SalvarVeiculosEstadia(ref Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia valorParametroEstadiaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaVeiculo repValorParametroEstadiaVeiculo = new Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaVeiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            List<dynamic> veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("EstadiaVeiculos"));
            if (veiculos == null) return;

            List<int> codigosRegistros = new List<int>();
            foreach (dynamic codigo in veiculos)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosRegistros.Add(intcodigo);
            }
            codigosRegistros = codigosRegistros.Where(o => o > 0).Distinct().ToList();

            List<int> registrosParaExcluir = repValorParametroEstadiaVeiculo.BuscarItensNaoPesentesNaLista(valorParametroEstadiaOcorrencia.Codigo, codigosRegistros);

            foreach (dynamic dynVeiculo in veiculos)
            {
                int.TryParse((string)dynVeiculo.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaVeiculo veiculo = repValorParametroEstadiaVeiculo.BuscarPorParametroEstadiaECodigo(valorParametroEstadiaOcorrencia.Codigo, codigo);

                if (veiculo == null)
                    veiculo = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaVeiculo();
                else
                    veiculo.Initialize();

                //decimal.TryParse((string)dynVeiculo.Valor, out decimal valor);
                decimal valor = Utilidades.Decimal.Converter((string)dynVeiculo.Valor);
                TimeSpan.TryParse((string)dynVeiculo.HoraInicial, out TimeSpan horaInicial);
                TimeSpan.TryParse((string)dynVeiculo.HoraFinal, out TimeSpan horaFinal);

                veiculo.ValorParametroEstadiaOcorrencia = valorParametroEstadiaOcorrencia;
                veiculo.ModeloVeicular = repModeloVeicularCarga.BuscarPorCodigo((int)dynVeiculo.ModeloVeicular.Codigo);
                veiculo.HoraInicial = horaInicial;
                veiculo.HoraFinal = horaFinal;
                veiculo.Valor = valor;

                if (veiculo.ModeloVeicular != null)
                {
                    if (veiculo.Codigo == 0)
                        repValorParametroEstadiaVeiculo.Inserir(veiculo);
                    else
                        repValorParametroEstadiaVeiculo.Atualizar(veiculo, Auditado);
                }
            }

            foreach (int codigoRegistro in registrosParaExcluir)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaVeiculo registro = repValorParametroEstadiaVeiculo.BuscarPorParametroEstadiaECodigo(valorParametroEstadiaOcorrencia.Codigo, codigoRegistro);
                if (registro != null) repValorParametroEstadiaVeiculo.Deletar(registro, Auditado);
            }
        }
        private void SalvarAjudantesEstadia(ref Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia valorParametroEstadiaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante repValorParametroEstadiaAjudante = new Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante(unitOfWork);

            List<dynamic> ajudantes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("EstadiaAjudantes"));
            if (ajudantes == null) return;

            List<int> codigosRegistros = new List<int>();
            foreach (dynamic codigo in ajudantes)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosRegistros.Add(intcodigo);
            }
            codigosRegistros = codigosRegistros.Where(o => o > 0).Distinct().ToList();

            List<int> registrosParaExcluir = repValorParametroEstadiaAjudante.BuscarItensNaoPesentesNaLista(valorParametroEstadiaOcorrencia.Codigo, codigosRegistros);

            foreach (dynamic dynAjudante in ajudantes)
            {
                int.TryParse((string)dynAjudante.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante ajudante = repValorParametroEstadiaAjudante.BuscarPorParametroEstadiaECodigo(valorParametroEstadiaOcorrencia.Codigo, codigo);

                if (ajudante == null)
                    ajudante = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante();
                else
                    ajudante.Initialize();

                //decimal.TryParse((string)dynAjudante.Valor, out decimal valor);
                decimal valor = Utilidades.Decimal.Converter((string)dynAjudante.Valor);
                TimeSpan.TryParse((string)dynAjudante.HoraInicial, out TimeSpan horaInicial);
                TimeSpan.TryParse((string)dynAjudante.HoraFinal, out TimeSpan horaFinal);

                ajudante.ValorParametroEstadiaOcorrencia = valorParametroEstadiaOcorrencia;
                ajudante.HoraInicial = horaInicial;
                ajudante.HoraFinal = horaFinal;
                ajudante.Valor = valor;

                if (ajudante.Codigo == 0)
                    repValorParametroEstadiaAjudante.Inserir(ajudante);
                else
                    repValorParametroEstadiaAjudante.Atualizar(ajudante, Auditado);
            }

            foreach (int codigoRegistro in registrosParaExcluir)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante registro = repValorParametroEstadiaAjudante.BuscarPorParametroEstadiaECodigo(valorParametroEstadiaOcorrencia.Codigo, codigoRegistro);
                if (registro != null) repValorParametroEstadiaAjudante.Deletar(registro, Auditado);
            }
        }





        private void PreencheEntidadeHoraExtra(ref Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia valorParametroHoraExtraOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            // Converte valores
            int.TryParse(Request.Params("HoraExtraTipoOcorrencia"), out int tipoOcorrencia);
            int.TryParse(Request.Params("HoraExtraComponenteFreteAjudante"), out int componenteFreteAjudante);
            int.TryParse(Request.Params("HoraExtraComponenteFreteVeiculo"), out int componenteFreteVeiculo);

            // Vincula dados
            valorParametroHoraExtraOcorrencia.TipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(tipoOcorrencia);
            valorParametroHoraExtraOcorrencia.ComponenteFreteAjudante = repComponenteFrete.BuscarPorCodigo(componenteFreteAjudante);
            valorParametroHoraExtraOcorrencia.ComponenteFreteVeiculo = repComponenteFrete.BuscarPorCodigo(componenteFreteVeiculo);
        }
        private void SalvarVeiculosHoraExtra(ref Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia valorParametroHoraExtraOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo repValorParametroHoraExtraVeiculo = new Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            List<dynamic> veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("HoraExtraVeiculos"));
            if (veiculos == null) return;

            List<int> codigosRegistros = new List<int>();
            foreach (dynamic codigo in veiculos)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosRegistros.Add(intcodigo);
            }
            codigosRegistros = codigosRegistros.Where(o => o > 0).Distinct().ToList();

            List<int> registrosParaExcluir = repValorParametroHoraExtraVeiculo.BuscarItensNaoPesentesNaLista(valorParametroHoraExtraOcorrencia.Codigo, codigosRegistros);

            foreach (dynamic dynVeiculo in veiculos)
            {
                int.TryParse((string)dynVeiculo.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo veiculo = repValorParametroHoraExtraVeiculo.BuscarPorParametroHoraExtraECodigo(valorParametroHoraExtraOcorrencia.Codigo, codigo);

                if (veiculo == null)
                    veiculo = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo();
                else
                    veiculo.Initialize();

                decimal valor = Utilidades.Decimal.Converter((string)dynVeiculo.Valor);
                //decimal.TryParse(((string)dynVeiculo.Valor).Replace(",", "."), out decimal valor);

                TimeSpan.TryParse((string)dynVeiculo.HoraInicial, out TimeSpan horaInicial);
                TimeSpan.TryParse((string)dynVeiculo.HoraFinal, out TimeSpan horaFinal);

                veiculo.ValorParametroHoraExtraOcorrencia = valorParametroHoraExtraOcorrencia;
                veiculo.ModeloVeicular = repModeloVeicularCarga.BuscarPorCodigo((int)dynVeiculo.ModeloVeicular.Codigo);
                veiculo.HoraInicial = horaInicial;
                veiculo.HoraFinal = horaFinal;
                veiculo.Valor = valor;

                if (veiculo.ModeloVeicular != null)
                {
                    if (veiculo.Codigo == 0)
                        repValorParametroHoraExtraVeiculo.Inserir(veiculo);
                    else
                        repValorParametroHoraExtraVeiculo.Atualizar(veiculo, Auditado);
                }
            }

            foreach (int codigoRegistro in registrosParaExcluir)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo registro = repValorParametroHoraExtraVeiculo.BuscarPorParametroHoraExtraECodigo(valorParametroHoraExtraOcorrencia.Codigo, codigoRegistro);
                if (registro != null) repValorParametroHoraExtraVeiculo.Deletar(registro, Auditado);
            }
        }
        private void SalvarAjudantesHoraExtra(ref Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia valorParametroHoraExtraOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraAjudante repValorParametroHoraExtraAjudante = new Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraAjudante(unitOfWork);

            List<dynamic> ajudantes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("HoraExtraAjudantes"));
            if (ajudantes == null) return;

            List<int> codigosRegistros = new List<int>();
            foreach (dynamic codigo in ajudantes)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosRegistros.Add(intcodigo);
            }
            codigosRegistros = codigosRegistros.Where(o => o > 0).Distinct().ToList();

            List<int> registrosParaExcluir = repValorParametroHoraExtraAjudante.BuscarItensNaoPesentesNaLista(valorParametroHoraExtraOcorrencia.Codigo, codigosRegistros);

            foreach (dynamic dynAjudante in ajudantes)
            {
                int.TryParse((string)dynAjudante.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraAjudante ajudante = repValorParametroHoraExtraAjudante.BuscarPorParametroHoraExtraECodigo(valorParametroHoraExtraOcorrencia.Codigo, codigo);

                if (ajudante == null)
                    ajudante = new Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraAjudante();
                else
                    ajudante.Initialize();

                //decimal.TryParse((string)dynAjudante.Valor, out decimal valor);
                decimal valor = Utilidades.Decimal.Converter((string)dynAjudante.Valor);
                TimeSpan.TryParse((string)dynAjudante.HoraInicial, out TimeSpan horaInicial);
                TimeSpan.TryParse((string)dynAjudante.HoraFinal, out TimeSpan horaFinal);

                ajudante.ValorParametroHoraExtraOcorrencia = valorParametroHoraExtraOcorrencia;
                ajudante.HoraInicial = horaInicial;
                ajudante.HoraFinal = horaFinal;
                ajudante.Valor = valor;

                if (ajudante.Codigo == 0)
                    repValorParametroHoraExtraAjudante.Inserir(ajudante);
                else
                    repValorParametroHoraExtraAjudante.Atualizar(ajudante, Auditado);
            }

            foreach (int codigoRegistro in registrosParaExcluir)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraAjudante registro = repValorParametroHoraExtraAjudante.BuscarPorParametroHoraExtraECodigo(valorParametroHoraExtraOcorrencia.Codigo, codigoRegistro);
                if (registro != null) repValorParametroHoraExtraAjudante.Deletar(registro, Auditado);
            }
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia valorParametroOcorrencia, out string msgErro)
        {
            msgErro = "";

            if (valorParametroOcorrencia.VigenciaInicial == DateTime.MinValue || valorParametroOcorrencia.VigenciaFinal == DateTime.MinValue)
            {
                msgErro = "Vigência é obrigatório.";
                return false;
            }

            return true;
        }
        private bool ValidaAbas(Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia valorParametroOcorrencia, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            msgErro = "";

            Repositorio.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo repValorParametroPernoiteVeiculo = new Repositorio.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaVeiculo repValorParametroEstadiaVeiculo = new Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaVeiculo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante repValorParametroEstadiaAjudante = new Repositorio.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo repValorParametroHoraExtraVeiculo = new Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraAjudante repValorParametroHoraExtraAjudante = new Repositorio.Embarcador.Ocorrencias.ValorParametroHoraExtraAjudante(unitOfWork);

            int totalVeiculosHoraExtra = repValorParametroHoraExtraVeiculo.ContarPorHoraExtra(valorParametroOcorrencia.ValorParametroHoraExtraOcorrencia.Codigo);
            int totalAjudantesHoraExtra = repValorParametroHoraExtraAjudante.ContarPorHoraExtra(valorParametroOcorrencia.ValorParametroHoraExtraOcorrencia.Codigo);
            if ((totalVeiculosHoraExtra > 0 || totalAjudantesHoraExtra > 0) && valorParametroOcorrencia.ValorParametroHoraExtraOcorrencia.TipoOcorrencia == null)
            {
                msgErro = "Tipo Ocorrência de Hora Extra é obrigatório.";
                return false;
            }
            if (totalVeiculosHoraExtra > 0 && valorParametroOcorrencia.ValorParametroHoraExtraOcorrencia.ComponenteFreteVeiculo == null)
            {
                msgErro = "Componente de Frete de Veículo em Hora Extra é obrigatório.";
                return false;
            }
            if (totalAjudantesHoraExtra > 0 && valorParametroOcorrencia.ValorParametroHoraExtraOcorrencia.ComponenteFreteAjudante == null)
            {
                msgErro = "Componente de Frete de Ajudante em Hora Extra é obrigatório.";
                return false;
            }

            int totalVeiculosEstadia = repValorParametroEstadiaVeiculo.ContarPorEstadia(valorParametroOcorrencia.ValorParametroEstadiaOcorrencia.Codigo);
            int totalAjudantesEstadia = repValorParametroEstadiaAjudante.ContarPorEstadia(valorParametroOcorrencia.ValorParametroEstadiaOcorrencia.Codigo);
            if ((totalVeiculosEstadia > 0 || totalAjudantesEstadia > 0) && valorParametroOcorrencia.ValorParametroEstadiaOcorrencia.TipoOcorrencia == null)
            {
                msgErro = "Tipo Ocorrência de Estadia é obrigatório.";
                return false;
            }
            if (totalVeiculosEstadia > 0 && valorParametroOcorrencia.ValorParametroEstadiaOcorrencia.ComponenteFreteVeiculo == null)
            {
                msgErro = "Componente de Frete de Veículo em Estadia é obrigatório.";
                return false;
            }
            if (totalAjudantesEstadia > 0 && valorParametroOcorrencia.ValorParametroEstadiaOcorrencia.ComponenteFreteAjudante == null)
            {
                msgErro = "Componente de Frete de Ajudante em Estadia é obrigatório.";
                return false;
            }

            int totalVeiculosPernoite = repValorParametroPernoiteVeiculo.ContarPorPernoite(valorParametroOcorrencia.ValorParametroPernoiteOcorrencia.Codigo);
            if (totalVeiculosPernoite > 0 && valorParametroOcorrencia.ValorParametroPernoiteOcorrencia.TipoOcorrencia == null)
            {
                msgErro = "Tipo Ocorrência de Pernoite é obrigatório.";
                return false;
            }
            if (totalVeiculosPernoite > 0 && valorParametroOcorrencia.ValorParametroPernoiteOcorrencia.ComponenteFrete == null)
            {
                msgErro = "Componente de Frete em Pernoite é obrigatório.";
                return false;
            }

            if (valorParametroOcorrencia.ValorParametroEscoltaOcorrencia.ComponenteFrete != null && valorParametroOcorrencia.ValorParametroEscoltaOcorrencia.TipoOcorrencia == null)
            {
                msgErro = "Tipo Ocorrência de Escolta é obrigatório.";
                return false;
            }
            if (valorParametroOcorrencia.ValorParametroEscoltaOcorrencia.TipoOcorrencia != null && valorParametroOcorrencia.ValorParametroEscoltaOcorrencia.ComponenteFrete == null)
            {
                msgErro = "Componente de Frete em Escolta é obrigatório.";
                return false;
            }

            return true;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "PessoaGrupoPessoa")
                propOrdenar = "GrupoPessoas, Pessoa ";
            else if (propOrdenar == "Vigencia")
                propOrdenar = "VigenciaInicial";
        }
        #endregion
    }
}
