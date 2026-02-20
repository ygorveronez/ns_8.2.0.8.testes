using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Acertos
{
    [CustomAuthorize("Acertos/TabelaDiaria")]
    public class TabelaDiariaController : BaseController
    {
		#region Construtores

		public TabelaDiariaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Acerto.TabelaDiaria repTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(unitOfWork);

                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
                Enum.TryParse(Request.Params("Ativo"), out ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> listaTabelaDiaria = repTabelaDiaria.Consultar(descricao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTabelaDiaria.ContarConsulta(descricao, ativo));
                var lista = from p in listaTabelaDiaria
                            select new
                            {
                                p.Codigo,
                                p.Descricao,
                                p.DescricaoAtivo
                            };
                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Acerto.TabelaDiaria repTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.TabelaDiaria tabelaDiaria = new Dominio.Entidades.Embarcador.Acerto.TabelaDiaria();

                PreencherTabelaDiaria(tabelaDiaria, unitOfWork);

                ValidarFilial(tabelaDiaria, unitOfWork);

                repTabelaDiaria.Inserir(tabelaDiaria, Auditado);

                SalvarTabelaPeriodo(tabelaDiaria, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                unitOfWork.Start();

                Repositorio.Embarcador.Acerto.TabelaDiaria repTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Acerto.TabelaDiaria tabelaDiaria = repTabelaDiaria.BuscarPorCodigo(codigo, true);

                if (tabelaDiaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherTabelaDiaria(tabelaDiaria, unitOfWork);

                ValidarFilial(tabelaDiaria, unitOfWork);

                repTabelaDiaria.Atualizar(tabelaDiaria, Auditado);

                SalvarTabelaPeriodo(tabelaDiaria, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Acerto.TabelaDiaria repTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(unitOfWork);
                Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo repTabelaDiariaPeriodo = new Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.TabelaDiaria tabelaDiaria = repTabelaDiaria.BuscarPorCodigo(codigo);
                if (tabelaDiaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo> periodos = repTabelaDiariaPeriodo.BuscarPorTabela(codigo);

                var dynTabelaDiaria = new
                {
                    tabelaDiaria.Codigo,
                    tabelaDiaria.Descricao,
                    tabelaDiaria.Ativo,
                    tabelaDiaria.GerarMovimentoSaidaFixaMotorista,
                    DataVigenciaInicial = tabelaDiaria.DataVigenciaInicial?.ToString("dd/MM/yyyy") ?? "",
                    DataVigenciaFinal = tabelaDiaria.DataVigenciaFinal?.ToString("dd/MM/yyyy") ?? "",
                    Filial = tabelaDiaria.Filial != null ? new { tabelaDiaria.Filial.Codigo, tabelaDiaria.Filial.Descricao } : null,
                    CentroResultado = tabelaDiaria.CentroResultado != null ? new { Codigo = tabelaDiaria.CentroResultado.Codigo, Descricao = tabelaDiaria.CentroResultado.Descricao } : null,
                    SegmentoVeiculo = tabelaDiaria.SegmentoVeiculo != null ? new { Codigo = tabelaDiaria.SegmentoVeiculo.Codigo, Descricao = tabelaDiaria.SegmentoVeiculo.Descricao } : null,
                    ModeloVeicularCarga = tabelaDiaria.ModeloVeicularCarga != null ? new { Codigo = tabelaDiaria.ModeloVeicularCarga.Codigo, Descricao = tabelaDiaria.ModeloVeicularCarga.Descricao } : null,
                    Periodos = periodos != null && periodos.Count > 0 ? (from o in periodos
                                                                         select new
                                                                         {
                                                                             o.Codigo,
                                                                             Justificativa = new { o.Justificativa.Codigo, o.Justificativa.Descricao },
                                                                             o.Descricao,
                                                                             HoraInicial = o.HoraInicial.HasValue ? o.HoraInicial.Value.ToString(@"hh\:mm") : string.Empty,
                                                                             HoraFinal = o.HoraFinal.HasValue ? o.HoraFinal.Value.ToString(@"hh\:mm") : string.Empty,
                                                                             Valor = o.Valor.ToString("n2")
                                                                         }).ToList() : null
                };

                return new JsonpResult(dynTabelaDiaria);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Acerto.TabelaDiaria reTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.TabelaDiaria tabelaDiaria = reTabelaDiaria.BuscarPorCodigo(codigo);

                if (tabelaDiaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                reTabelaDiaria.Deletar(tabelaDiaria, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherTabelaDiaria(Dominio.Entidades.Embarcador.Acerto.TabelaDiaria tabelaDiaria, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            int.TryParse(Request.Params("SegmentoVeiculo"), out int codigoSegmento);
            int.TryParse(Request.Params("CentroResultado"), out int codigoCentroResultado);
            int.TryParse(Request.Params("ModeloVeicularCarga"), out int codigoModelo);
            int.TryParse(Request.Params("Filial"), out int codigoFilial);

            tabelaDiaria.DataVigenciaInicial = Request.GetNullableDateTimeParam("DataVigenciaInicial");
            tabelaDiaria.DataVigenciaFinal = Request.GetNullableDateTimeParam("DataVigenciaFinal");
            tabelaDiaria.Descricao = Request.GetStringParam("Descricao");
            tabelaDiaria.Ativo = Request.GetBoolParam("Ativo");
            tabelaDiaria.GerarMovimentoSaidaFixaMotorista = Request.GetBoolParam("GerarMovimentoSaidaFixaMotorista");

            tabelaDiaria.ModeloVeicularCarga = codigoModelo > 0 ? repModeloVeicularCarga.BuscarPorCodigo(codigoModelo) : null;
            tabelaDiaria.SegmentoVeiculo = codigoSegmento > 0 ? repSegmentoVeiculo.BuscarPorCodigo(codigoSegmento) : null;
            tabelaDiaria.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
            tabelaDiaria.Filial = codigoFilial > 0 ? repFilial.BuscarPorCodigo(codigoFilial) : null;
        }

        private void SalvarTabelaPeriodo(Dominio.Entidades.Embarcador.Acerto.TabelaDiaria tabelaDiaria, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo repTabelaDiariaPeriodo = new Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

            List<dynamic> periodos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Periodos"));
            if (periodos == null) return;

            List<int> codigosRegistros = new List<int>();
            foreach (dynamic codigo in periodos)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosRegistros.Add(intcodigo);
            }
            codigosRegistros = codigosRegistros.Where(o => o > 0).Distinct().ToList();

            List<int> registrosParaExcluir = repTabelaDiariaPeriodo.BuscarItensNaoPesentesNaLista(tabelaDiaria.Codigo, codigosRegistros);

            foreach (dynamic dynVeiculo in periodos)
            {
                int.TryParse((string)dynVeiculo.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo tabelaDiariaPeriodo = repTabelaDiariaPeriodo.BuscarPorParametroHoraExtraECodigo(tabelaDiaria.Codigo, codigo);

                if (tabelaDiariaPeriodo == null)
                    tabelaDiariaPeriodo = new Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo();
                else
                    tabelaDiariaPeriodo.Initialize();

                decimal valor = Utilidades.Decimal.Converter((string)dynVeiculo.Valor);
                TimeSpan.TryParse((string)dynVeiculo.HoraInicial, out TimeSpan horaInicial);
                TimeSpan.TryParse((string)dynVeiculo.HoraFinal, out TimeSpan horaFinal);

                tabelaDiariaPeriodo.TabelaDiaria = tabelaDiaria;
                tabelaDiariaPeriodo.Justificativa = repJustificativa.BuscarPorCodigo((int)dynVeiculo.Justificativa.Codigo);
                tabelaDiariaPeriodo.Descricao = (string)dynVeiculo.Descricao;
                tabelaDiariaPeriodo.HoraInicial = horaInicial;
                tabelaDiariaPeriodo.HoraFinal = horaFinal;
                tabelaDiariaPeriodo.Valor = valor;

                if (tabelaDiariaPeriodo.Justificativa != null)
                {
                    if (tabelaDiariaPeriodo.Codigo == 0)
                        repTabelaDiariaPeriodo.Inserir(tabelaDiariaPeriodo);
                    else
                        repTabelaDiariaPeriodo.Atualizar(tabelaDiariaPeriodo, Auditado);
                }
            }

            foreach (int codigoRegistro in registrosParaExcluir)
            {
                Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo registro = repTabelaDiariaPeriodo.BuscarPorParametroHoraExtraECodigo(tabelaDiaria.Codigo, codigoRegistro);
                if (registro != null) repTabelaDiariaPeriodo.Deletar(registro, Auditado);
            }
        }

        private void ValidarFilial(Dominio.Entidades.Embarcador.Acerto.TabelaDiaria tabelaDiaria, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Acerto.TabelaDiaria repositorioTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(unitOfWork);

            bool existeRegistro = repositorioTabelaDiaria.BuscarExistenciaPorFilialModeloVeicular(tabelaDiaria.Codigo, (tabelaDiaria.Filial?.Codigo ?? 0), (tabelaDiaria.ModeloVeicularCarga?.Codigo ?? 0));

            if (existeRegistro)
                throw new ControllerException("Já existe um registro cadastrado com o mesmo conjunto de filial e modelo veicular.");
        }

        #endregion
    }
}


