using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using System.IO;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.PagamentoAgregado
{
    [CustomAuthorize(new string[] { "PesquisaAutorizacoes", "DetalhesAutorizacao" }, "PagamentosAgregados/PagamentoAgregado")]
    public class PagamentoAgregadoController : BaseController
    {
		#region Construtores

		public PagamentoAgregadoController(Conexao conexao) : base(conexao) { }

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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarDescontoAcrescimo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPagamento = 0;
                int.TryParse(Request.Params("Codigo"), out codigoPagamento);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoJustificativa", false);
                grid.AdicionarCabecalho("CodigoPagamento", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(unitOfWork);

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> listaPagamentoAgregadoAcrescimoDesconto = repPagamentoAgregadoAcrescimoDesconto.BuscarPorPagamento(codigoPagamento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPagamentoAgregadoAcrescimoDesconto.ContarBuscarPorPagamento(codigoPagamento));
                var dynRetorno = (from obj in listaPagamentoAgregadoAcrescimoDesconto
                                  select new
                                  {
                                      obj.Codigo,
                                      obj.Justificativa.TipoJustificativa,
                                      CodigoPagamento = obj.PagamentoAgregado.Codigo,
                                      Descricao = obj.Justificativa.Descricao,
                                      Valor = obj.Valor.ToString("n2")
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o desconto e acréscimo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarOcorrenciasDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPagamento = 0;
                int.TryParse(Request.Params("Codigo"), out codigoPagamento);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> listaCargaOcorrenciaDocumento = repCargaOcorrenciaDocumento.BuscarOcorrenciaPorCTe(codigoPagamento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaOcorrenciaDocumento.ContarBuscarOcorrenciaPorCTe(codigoPagamento));
                var dynRetorno = (from obj in listaCargaOcorrenciaDocumento
                                  select new
                                  {
                                      obj.Codigo,
                                      Descricao = obj.CargaOcorrencia.TipoOcorrencia?.Descricao ?? "",
                                      Data = obj.CargaOcorrencia.DataOcorrencia.ToString("dd/MM/yyyy")
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as ocorrências do documento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> listaAutorizacao = repAprovacaoAlcadaPagamentoAgregado.ConsultarAutorizacoesPorPagamento(codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAprovacaoAlcadaPagamentoAgregado.ContarConsultaAutorizacoesPorPagamento(codigo));

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = obj.DescricaoSituacao,
                                 Usuario = obj.Usuario?.Nome,
                                 Regra = TituloRegra(obj),
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 DT_RowColor = CoresRegras(obj)
                             }).ToList();
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

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia
                Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);

                // Converte dados
                int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

                // Busca a autorizacao
                Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado autorizacao = repAprovacaoAlcadaPagamentoAgregado.BuscarPorCodigo(codigoAutorizacao);

                var retorno = new
                {
                    autorizacao.Codigo,
                    Regra = TituloRegra(autorizacao),
                    Situacao = autorizacao.DescricaoSituacao,
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,

                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = autorizacao.Motivo ?? string.Empty,
                };

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

        public async Task<IActionResult> ObterResumo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento repPagamentoAgregadoAdiantamento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento repPagamentoAgregadoAbastecimento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento> documentos = repPagamentoAgregadoDocumento.BuscarPorPagamento(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento> adiantamentos = repPagamentoAgregadoAdiantamento.BuscarPorPagamento(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> acrescimosDescontos = repPagamentoAgregadoAcrescimoDesconto.BuscarPorPagamento(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento> abastecimentos = repPagamentoAgregadoAbastecimento.BuscarPorPagamento(codigo);

                // Valida
                if (pagamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                decimal totalAbastecimentos = abastecimentos != null && abastecimentos.Count > 0 ? abastecimentos.Select(o => (o.Abastecimento.ValorTotal)).Sum() : 0m;
                decimal totalAdiantamentos = documentos != null && documentos.Count > 0 ? documentos.Select(o => (o.ValorAdiantamento)).Sum() : 0m;
                decimal totalSaldo = documentos != null && documentos.Count > 0 ? documentos.Select(o => (o.ValorSaldo)).Sum() : 0m;
                decimal totalPagamento = documentos != null && documentos.Count > 0 ? documentos.Select(o => (o.Valor)).Sum() : 0m;
                decimal valorAdiantado = adiantamentos != null && adiantamentos.Count > 0 ? adiantamentos.Select(o => (o.PagamentoMotoristaTMS.Valor - o.PagamentoMotoristaTMS.SaldoDescontado)).Sum() : 0m;
                decimal valorAcrescimo = acrescimosDescontos != null && acrescimosDescontos.Count > 0 ? acrescimosDescontos.Where(o => o.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Select(o => o.Valor).Sum() : 0m;
                decimal valorDesconto = acrescimosDescontos != null && acrescimosDescontos.Count > 0 ? acrescimosDescontos.Where(o => o.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Select(o => o.Valor).Sum() : 0m;

                // Formata retorno
                var retorno = new
                {
                    pagamento.Codigo,
                    TotalAbastecimentos = totalAbastecimentos.ToString("n2"),
                    TotalAdiantamentos = totalAdiantamentos.ToString("n2"),
                    TotalPagamento = totalPagamento.ToString("n2"),
                    ValorAdiantado = valorAdiantado.ToString("n3"),
                    ValorAcrescimo = valorAcrescimo.ToString("n3"),
                    ValorDesconto = valorDesconto.ToString("n3"),
                    ValorSaldo = totalSaldo.ToString("n3"),
                    Saldo = (totalPagamento - totalAdiantamentos - totalAbastecimentos + valorAcrescimo - valorDesconto).ToString("n2")
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar resumo.");
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
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento repPagamentoAgregadoAdiantamento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento repPagamentoAgregadoAbastecimento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento> documentos = repPagamentoAgregadoDocumento.BuscarPorPagamento(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento> adiantamentos = repPagamentoAgregadoAdiantamento.BuscarPorPagamento(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> acrescimosDescontos = repPagamentoAgregadoAcrescimoDesconto.BuscarPorPagamento(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento> abastecimentos = repPagamentoAgregadoAbastecimento.BuscarPorPagamento(codigo);

                // Valida
                if (pagamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                decimal totalAbastecimentos = abastecimentos != null && abastecimentos.Count > 0 ? abastecimentos.Select(o => (o.Abastecimento.ValorTotal)).Sum() : 0m;
                decimal totalAdiantamentos = documentos != null && documentos.Count > 0 ? documentos.Select(o => (o.ValorAdiantamento)).Sum() : 0m;
                decimal totalSaldo = documentos != null && documentos.Count > 0 ? documentos.Select(o => (o.ValorSaldo)).Sum() : 0m;
                decimal totalPagamento = documentos != null && documentos.Count > 0 ? documentos.Select(o => (o.Valor)).Sum() : 0m;
                decimal valorAdiantado = adiantamentos != null && adiantamentos.Count > 0 ? adiantamentos.Select(o => (o.PagamentoMotoristaTMS.Valor - o.PagamentoMotoristaTMS.SaldoDescontado)).Sum() : 0m;
                decimal valorAcrescimo = acrescimosDescontos != null && acrescimosDescontos.Count > 0 ? acrescimosDescontos.Where(o => o.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Select(o => o.Valor).Sum() : 0m;
                decimal valorDesconto = acrescimosDescontos != null && acrescimosDescontos.Count > 0 ? acrescimosDescontos.Where(o => o.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Select(o => o.Valor).Sum() : 0m;

                // Formata retorno
                var retorno = new
                {
                    pagamento.Codigo,
                    pagamento.Situacao,
                    DadosPagamento = new
                    {
                        Codigo = pagamento.Codigo,
                        Numero = pagamento.Numero,
                        TotalAbastecimentos = totalAbastecimentos.ToString("n2"),
                        TotalAdiantamentos = totalAdiantamentos.ToString("n2"),
                        TotalSaldo = totalSaldo.ToString("n2"),
                        TotalPagamento = totalPagamento.ToString("n2"),
                        ValorAdiantado = valorAdiantado.ToString("n3"),
                        ValorAcrescimo = valorAcrescimo.ToString("n3"),
                        ValorDesconto = valorDesconto.ToString("n3"),
                        Saldo = (totalPagamento - totalAdiantamentos - totalAbastecimentos + valorAcrescimo - valorDesconto).ToString("n2"),
                        Cliente = pagamento.Cliente != null ? new { Codigo = pagamento.Cliente.CPF_CNPJ, Descricao = pagamento.Cliente.Descricao } : null,
                        Veiculo = pagamento.Veiculo != null ? new { Codigo = pagamento.Veiculo.Codigo, Descricao = pagamento.Veiculo.Descricao } : null,
                        DataInicial = pagamento.DataInicial.HasValue ? pagamento.DataInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataFinal = pagamento.DataFinal.HasValue ? pagamento.DataFinal.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataInicialOcorrencia = pagamento.DataInicialOcorrencia.HasValue ? pagamento.DataInicialOcorrencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataFinalOcorrencia = pagamento.DataFinalOcorrencia.HasValue ? pagamento.DataFinalOcorrencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataPagamento = pagamento.DataPagamento.ToString("dd/MM/yyyy") ?? string.Empty,
                        pagamento.Observacao,
                        TomadorFatura = new { Codigo = pagamento.TomadorFatura?.CPF_CNPJ ?? 0d, Descricao = pagamento.TomadorFatura?.Descricao ?? string.Empty },
                        NumeroFatura = pagamento.NumeroFatura.ObterSomenteNumeros(),
                        CompetenciaMes = pagamento.CompetenciaMes,
                        CompetenciaQuinzena = pagamento.CompetenciaQuinzena,
                        DescricaoCompetencia = pagamento.DescricaoCompetencia,
                        Empresa = new { Codigo = pagamento.Empresa?.Codigo ?? 0, Descricao = pagamento.Empresa?.Descricao ?? string.Empty },
                        ListaDocumentos = (from obj in documentos
                                           select new
                                           {
                                               Codigo = obj.ConhecimentoDeTransporteEletronico.Codigo,
                                               CodigoDocumentoPagamentoAgregado = obj.Codigo,
                                               CodigoDocumento = obj.ConhecimentoDeTransporteEletronico.Codigo,
                                               CodigoPagamento = codigo,
                                               Numero = obj.ConhecimentoDeTransporteEletronico.Numero.ToString("n0"),
                                               Serie = obj.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString("n0"),
                                               ValorCTe = obj.ConhecimentoDeTransporteEletronico.ValorAReceber.ToString("n2"),
                                               ValorPagamento = obj.Valor.ToString("n2"),
                                               ValorAdiantamento = obj.ValorAdiantamento.ToString("n2"),
                                               ValorSaldo = obj.ValorSaldo.ToString("n2"),
                                               Remetente = obj.ConhecimentoDeTransporteEletronico.Remetente.Nome,
                                               Destinatario = obj.ConhecimentoDeTransporteEletronico.Destinatario.Nome,
                                               UltimaOcorrencia = string.Empty, //UltimaOcorrenciaDocumento(obj.ConhecimentoDeTransporteEletronico.Codigo, unitOfWork),
                                               Status = obj.ConhecimentoDeTransporteEletronico.DescricaoStatus,
                                               Motorista = obj.ConhecimentoDeTransporteEletronico.Motoristas != null && obj.ConhecimentoDeTransporteEletronico.Motoristas.Count > 0 ? String.Join(",", obj.ConhecimentoDeTransporteEletronico.Motoristas.Select(o => o.NomeMotorista).ToList()) : string.Empty,
                                               CIOT = obj.CIOT?.Numero ?? "",
                                               Carga = obj.Carga?.CodigoCargaEmbarcador ?? ""
                                           }).ToList(),
                        ListaAdiantamentos = (from obj in adiantamentos
                                              select new
                                              {
                                                  CodigoAdiantamentoPagamentoAgregado = obj.Codigo,
                                                  Codigo = obj.PagamentoMotoristaTMS.Codigo,
                                                  CodigoAdiantamento = obj.PagamentoMotoristaTMS.Codigo,
                                                  CodigoPagamento = codigo,
                                                  Numero = obj.PagamentoMotoristaTMS.Numero.ToString("n0"),
                                                  Valor = obj.PagamentoMotoristaTMS.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2"),
                                                  Descricao = obj.PagamentoMotoristaTMS.PagamentoMotoristaTipo.Descricao,
                                                  Data = obj.PagamentoMotoristaTMS.DataPagamento.ToString("dd/MM/yyyy"),
                                                  Motorista = obj.PagamentoMotoristaTMS.Motorista.Nome
                                              }).ToList(),
                        ListaAbastecimentos = (from obj in abastecimentos
                                               select new
                                               {
                                                   CodigoAbastecimentoPagamentoAgregado = obj.Codigo,
                                                   Codigo = obj.Abastecimento.Codigo,
                                                   CodigoAbastecimento = obj.PagamentoAgregado.Codigo,
                                                   CodigoPagamento = codigo,
                                                   Data = obj.Abastecimento.Data.HasValue ? obj.Abastecimento.Data.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                                   Fornecedor = obj.Abastecimento?.Posto?.Descricao ?? "",
                                                   Documento = obj.Abastecimento?.Documento ?? "",
                                                   KM = obj.Abastecimento?.Kilometragem,
                                                   Litros = obj.Abastecimento?.Litros.ToString("n2"),
                                                   ValorUnitario = obj.Abastecimento?.ValorUnitario.ToString("n4"),
                                                   ValorTotal = obj.Abastecimento?.ValorTotal.ToString("n2")
                                               }).ToList(),
                        ListaAnexos = (from obj in pagamento.Anexos
                                       select new
                                       {
                                           obj.Codigo,
                                           DescricaoAnexo = obj.Descricao,
                                           Arquivo = obj.NomeArquivo
                                       }).ToList()
                    },
                    DadosDocumento = new
                    {
                        Codigo = pagamento.Codigo
                    },
                    Resumo = new
                    {
                        ResumoSolicitacao = new
                        {
                            Situacao = pagamento.DescricaoSituacao,
                            Data = pagamento.Data.ToString("dd/MM/yyyy"),
                            Usuario = pagamento.Usuario.Nome,
                            Cliente = pagamento.Cliente.Nome,
                            Valor = pagamento.Valor.ToString("n3"),
                            TotalAbastecimentos = totalAbastecimentos.ToString("n2"),
                            TotalAdiantamentos = totalAdiantamentos.ToString("n2"),
                            TotalPagamento = totalPagamento.ToString("n2"),
                            ValorAdiantado = valorAdiantado.ToString("n3"),
                            ValorAcrescimo = valorAcrescimo.ToString("n3"),
                            ValorDesconto = valorDesconto.ToString("n3"),
                            DataPagamento = pagamento.DataPagamento.ToString("dd/MM/yyyy"),
                            Saldo = (totalPagamento - totalAdiantamentos - totalPagamento + valorAcrescimo - valorDesconto).ToString("n2")
                        },
                        ResumoRetorno = new
                        {
                            DataRetorno = pagamento.DataAprovacao?.ToString("dd/MM/yyyy hh:mm") ?? " - ",
                            Aprovador = pagamento.UsuarioAprovador?.Nome ?? " - ",
                        }
                    }
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
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado();

                // Preenche entidade com dados
                PreencheEntidade(ref pagamento, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(pagamento, out string erro))
                    return new JsonpResult(false, true, erro);

                repPagamentoAgregado.Inserir(pagamento, Auditado);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    pagamento.Codigo,
                    pagamento.Numero
                });
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
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);

                // Busca informacoes
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigo, true);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> descontosAcrescimos = repPagamentoAgregadoAcrescimoDesconto.BuscarPorPagamento(codigo);

                // Preenche entidade com dados
                PreencheEntidade(ref pagamento, unitOfWork);

                bool validarOcorrenciaFinalizadora = false;
                decimal valorDocumentos = 0, valorAdiantamento = 0, valorAbastecimento = 0;
                bool contemDocumentos = false;
                bool contemDocumentosSemOcorrenciaFinalizadora = false;
                AtualizarDocumentosPagamento(ref contemDocumentosSemOcorrenciaFinalizadora, ref contemDocumentos, ref valorDocumentos, pagamento, unitOfWork, Request.Params("ListaDocumentos"), validarOcorrenciaFinalizadora);
                AtualizarAdiantamentoPagamento(ref valorAdiantamento, pagamento, unitOfWork, Request.Params("ListaAdiantamentos"));
                AtualizarAbastecimentoPagamento(ref valorAbastecimento, pagamento, unitOfWork, Request.Params("ListaAbastecimentos"));
                ExcluirAnexos(unitOfWork);

                if (descontosAcrescimos.Count > 0)
                {
                    pagamento.Valor += descontosAcrescimos.Where(o => o.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Select(o => o.Valor).Sum();
                    pagamento.Valor -= descontosAcrescimos.Where(o => o.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Select(o => o.Valor).Sum();
                }
                pagamento.Valor += valorDocumentos;
                pagamento.Valor -= valorAdiantamento;

                repPagamentoAgregado.Atualizar(pagamento, Auditado);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    pagamento.Codigo,
                    pagamento.Numero
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);

                // Busca informacoes
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigo, true);

                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Cancelado;

                repPagamentoAgregado.Atualizar(pagamento, Auditado);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    pagamento.Codigo,
                    pagamento.Numero
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Estornar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                Servicos.Embarcador.Financeiro.TituloAPagar serTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                // Instancia repositorios
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                // Busca informacoes
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorPagamentoAgregado(codigo);

                if (pagamento == null || pagamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Finalizado)
                    return new JsonpResult(false, "A situação do pagamento não permite realizar o estorno.");

                if (contratoFrete != null)
                {
                    if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorContratoFrete(contratoFrete.Codigo);
                        if (titulo != null && titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                            return new JsonpResult(false, "O título gerado já se encontra quitado, favor reverta o mesmo antes de estornar o pagamento.");

                        contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto;
                        repContratoFrete.Atualizar(contratoFrete);

                        Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                            tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                        string erro = string.Empty;

                        Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTerceiro.BuscarPorPessoa(contratoFrete.TransportadorTerceiro.CPF_CNPJ);
                        if (!modalidadeTransportadoraPessoas.GerarPagamentoTerceiro)
                        {
                            if (!serTituloAPagar.AtualizarTitulos(contratoFrete, unitOfWork, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, erro);
                            }

                            erro = string.Empty;
                            if (!serContratoFrete.GerarMovimentacaoFinanceiraReversaoJustificativas(contratoFrete, unitOfWork, TipoServicoMultisoftware, out erro))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, erro);
                            }
                        }
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFrete, null, "Reabriu o Contrato.", unitOfWork);
                    }
                    else
                        return new JsonpResult(false, "A situação do contrato de frete não permite estornar o pagamento ao agregado.");
                }


                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Iniciada;

                repPagamentoAgregado.Atualizar(pagamento, Auditado);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    pagamento.Codigo,
                    pagamento.Numero
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento repPagamentoAgregadoAdiantamento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento repPagamentoAgregadoAbastecimento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFrete = repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();

                // Busca informacoes
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigo, true);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> descontosAcrescimos = repPagamentoAgregadoAcrescimoDesconto.BuscarPorPagamento(codigo);

                // Preenche entidade com dados
                PreencheEntidade(ref pagamento, unitOfWork);

                bool validarOcorrenciaFinalizadora = false;
                if (pagamento.Cliente.NaoUsarConfiguracaoFaturaGrupo)
                    validarOcorrenciaFinalizadora = pagamento.Cliente.SomenteOcorrenciasFinalizadoras != null ? pagamento.Cliente.SomenteOcorrenciasFinalizadoras.Value : false;
                else if (pagamento.Cliente.GrupoPessoas != null)
                    validarOcorrenciaFinalizadora = pagamento.Cliente.GrupoPessoas.SomenteOcorrenciasFinalizadoras != null ? pagamento.Cliente.GrupoPessoas.SomenteOcorrenciasFinalizadoras.Value : false;

                decimal valorDocumentos = 0, valorAdiantamento = 0, valorAbastecimento = 0;
                bool contemDocumentos = false;
                bool contemDocumentosSemOcorrenciaFinalizadora = false;
                AtualizarDocumentosPagamento(ref contemDocumentosSemOcorrenciaFinalizadora, ref contemDocumentos, ref valorDocumentos, pagamento, unitOfWork, Request.Params("ListaDocumentos"), validarOcorrenciaFinalizadora);
                AtualizarAdiantamentoPagamento(ref valorAdiantamento, pagamento, unitOfWork, Request.Params("ListaAdiantamentos"));
                AtualizarAbastecimentoPagamento(ref valorAbastecimento, pagamento, unitOfWork, Request.Params("ListaAbastecimentos"));

                if (descontosAcrescimos.Count > 0)
                {
                    pagamento.Valor += descontosAcrescimos.Where(o => o.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Select(o => o.Valor).Sum();
                    pagamento.Valor -= descontosAcrescimos.Where(o => o.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Select(o => o.Valor).Sum();
                }
                pagamento.Valor += valorDocumentos;
                pagamento.Valor -= valorAdiantamento;

                // Valida entidade
                if (!ValidaEntidade(pagamento, out string erro))
                    return new JsonpResult(false, true, erro);
                if (!contemDocumentos)
                    return new JsonpResult(false, true, "Favor informe ao menos um documento para realizar o pagamento ao agregado.");
                if (contemDocumentosSemOcorrenciaFinalizadora)
                    return new JsonpResult(false, true, "A configuração do Grupo/Pessoa valida para gerar o pagamento ao agregado somente com ocorrências finalizadoras.");

                if (configuracaoContratoFrete.UtilizarNovoLayoutPagamentoAgregado)
                {
                    List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento> documentos = repPagamentoAgregadoDocumento.BuscarPorPagamento(codigo);
                    List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento> adiantamentos = repPagamentoAgregadoAdiantamento.BuscarPorPagamento(codigo);
                    List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> acrescimosDescontos = repPagamentoAgregadoAcrescimoDesconto.BuscarPorPagamento(codigo);
                    List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento> abastecimentos = repPagamentoAgregadoAbastecimento.BuscarPorPagamento(codigo);

                    decimal totalAbastecimentos = abastecimentos != null && abastecimentos.Count > 0 ? abastecimentos.Select(o => (o.Abastecimento.ValorTotal)).Sum() : 0m;
                    decimal totalAdiantamentos = documentos != null && documentos.Count > 0 ? documentos.Select(o => (o.ValorAdiantamento)).Sum() : 0m;
                    decimal totalPagamento = documentos != null && documentos.Count > 0 ? documentos.Select(o => (o.Valor)).Sum() : 0m;
                    decimal valorAdiantado = adiantamentos != null && adiantamentos.Count > 0 ? adiantamentos.Select(o => (o.PagamentoMotoristaTMS.Valor - o.PagamentoMotoristaTMS.SaldoDescontado)).Sum() : 0m;
                    decimal valorAcrescimo = acrescimosDescontos != null && acrescimosDescontos.Count > 0 ? acrescimosDescontos.Where(o => o.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Select(o => o.Valor).Sum() : 0m;
                    decimal valorDesconto = acrescimosDescontos != null && acrescimosDescontos.Count > 0 ? acrescimosDescontos.Where(o => o.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Select(o => o.Valor).Sum() : 0m;

                    // Formata retorno
                    if ((totalPagamento - totalAdiantamentos - totalAbastecimentos + valorAcrescimo - valorDesconto) < 0)
                        return new JsonpResult(false, true, "Não é possível finalizar o pagamento com saldo negativo.");
                }

                // Busca as regras
                string retorno = string.Empty;
                retorno = EtapaAprovacao(ref pagamento, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Ocorreu um erro ao aprovar as solicitações. " + retorno);
                }

                repPagamentoAgregado.Atualizar(pagamento, Auditado);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    pagamento.Codigo,
                    pagamento.Numero
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigo);

                // Valida
                if (pagamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (pagamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.SemRegra)
                    return new JsonpResult(false, true, "A situação não permite essa operação.");

                // Inicia transacao
                unitOfWork.Start();

                // Busca as regras
                string retorno = string.Empty;
                retorno = EtapaAprovacao(ref pagamento, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Ocorreu um erro ao aprovar as solicitações. " + retorno);
                }

                // Persiste dados
                repPagamentoAgregado.Atualizar(pagamento);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    pagamento.Codigo,
                    PossuiRegra = pagamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.AgAprovacao,
                    Finalizado = pagamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Finalizado,
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoPagamento = 0, codigoDocumento = 0;
                int.TryParse(Request.Params("Codigo"), out codigoPagamento);
                int.TryParse(Request.Params("CodigoDocumento"), out codigoDocumento);

                if (codigoPagamento == 0 || codigoDocumento == 0)
                    return new JsonpResult(false, "Favor informe um documento e inicie um pagamento.");

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unitOfWork);

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento documento = repPagamentoAgregadoDocumento.BuscarPorCodigo(codigoDocumento);
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigoPagamento);

                pagamento.Valor -= documento.Valor;
                repPagamentoAgregado.Atualizar(pagamento);

                repPagamentoAgregadoDocumento.Deletar(documento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarStatusPagamentoAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoPagamento = 0;
                int.TryParse(Request.Params("Codigo"), out codigoPagamento);

                if (codigoPagamento == 0)
                    return new JsonpResult(false, "Favor informe um documento e inicie um pagamento.");

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigoPagamento);

                pagamento.Usuario = this.Usuario;
                pagamento.StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.AgInicioDocumentos;
                repPagamentoAgregado.Atualizar(pagamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir novo documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RecalcularPagamentoAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoPagamento = 0;
                int.TryParse(Request.Params("Codigo"), out codigoPagamento);

                if (codigoPagamento == 0)
                    return new JsonpResult(false, "Favor informe um documento e inicie um pagamento.");

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unitOfWork);

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigoPagamento);

                pagamento.Usuario = this.Usuario;
                pagamento.StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.AgInicioDocumentos;
                repPagamentoAgregado.Atualizar(pagamento);

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento> documentos = repPagamentoAgregadoDocumento.BuscarPorPagamento(codigoPagamento);
                for (int i = 0; i < documentos.Count; i++)
                {
                    documentos[i].StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.AgInicio;
                    repPagamentoAgregadoDocumento.Atualizar(documentos[i]);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao recalcular valores.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarDocumentoManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoPagamento, codigoDocumento = 0;
                int.TryParse(Request.Params("Documento"), out codigoDocumento);
                int.TryParse(Request.Params("Codigo"), out codigoPagamento);

                if (codigoPagamento == 0 || codigoDocumento == 0)
                    return new JsonpResult(false, "Favor informe um documento e inicie um pagamento.");

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Documentos.CIOTCTe repCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigoPagamento);
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento documento = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento();
                documento.PagamentoAgregado = pagamento;
                documento.Valor = 0;
                documento.ConhecimentoDeTransporteEletronico = repConhecimentoDeTransporteEletronico.BuscarPorCodigo(codigoDocumento);
                documento.CIOT = repCIOTCTe.BuscarPorCTe(codigoDocumento);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(documento.ConhecimentoDeTransporteEletronico.Codigo);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(cargaCTe?.Carga?.Codigo ?? 0);
                documento.Carga = cargaCTe?.Carga;
                documento.ContratoFrete = contratoFrete;

                pagamento.Usuario = this.Usuario;
                documento.StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.AgInicio;

                repPagamentoAgregadoDocumento.Inserir(documento);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    documento.Codigo,
                    CIOT = documento.CIOT?.Numero ?? "",
                    Carga = documento.Carga?.CodigoCargaEmbarcador ?? ""
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir novo documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> IniciarCalculoDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFrete = repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                int codigoPagamento, codigoTipoOcorrencia = 0;
                int.TryParse(Request.Params("TipoOcorrencia"), out codigoTipoOcorrencia);
                int.TryParse(Request.Params("Codigo"), out codigoPagamento);

                if ((codigoPagamento == 0 || codigoTipoOcorrencia == 0) && !configuracaoContratoFrete.UtilizarNovoLayoutPagamentoAgregado)
                    return new JsonpResult(false, "Favor informe um tipo de ocorrencia e inicie um pagamento.");

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamentoAgregado = repPagamentoAgregado.BuscarPorCodigo(codigoPagamento, true);
                pagamentoAgregado.Usuario = this.Usuario;
                pagamentoAgregado.StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.AgInicio;
                pagamentoAgregado.TipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoTipoOcorrencia);

                bool validarOcorrenciaFinalizadora = false;
                if (pagamentoAgregado.Cliente.NaoUsarConfiguracaoFaturaGrupo)
                    validarOcorrenciaFinalizadora = pagamentoAgregado.Cliente.SomenteOcorrenciasFinalizadoras != null ? pagamentoAgregado.Cliente.SomenteOcorrenciasFinalizadoras.Value : false;
                else if (pagamentoAgregado.Cliente.GrupoPessoas != null)
                    validarOcorrenciaFinalizadora = pagamentoAgregado.Cliente.GrupoPessoas.SomenteOcorrenciasFinalizadoras != null ? pagamentoAgregado.Cliente.GrupoPessoas.SomenteOcorrenciasFinalizadoras.Value : false;

                if (validarOcorrenciaFinalizadora && pagamentoAgregado.TipoOcorrencia?.Tipo != "F")
                    return new JsonpResult(false, false, "A configuração do Grupo/Pessoa valida para gerar o pagamento ao agregado somente com ocorrências finalizadoras.");

                repPagamentoAgregado.Atualizar(pagamentoAgregado, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir novo desconto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InserirDescontoAcrescimo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPagamento, codigoJustificativa = 0;
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);
                int.TryParse(Request.Params("Codigo"), out codigoPagamento);

                decimal valor = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);

                if (codigoPagamento == 0 || codigoJustificativa == 0)
                    return new JsonpResult(false, "Favor informe uma justificativa e inicie um pagamento.");

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto descontoAcrescimo = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto();
                descontoAcrescimo.PagamentoAgregado = repPagamentoAgregado.BuscarPorCodigo(codigoPagamento);
                descontoAcrescimo.Valor = valor;
                descontoAcrescimo.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                repPagamentoAgregadoAcrescimoDesconto.Inserir(descontoAcrescimo, Auditado);

                // Retorna sucesso
                return new JsonpResult(new
                {
                    descontoAcrescimo.Justificativa.TipoJustificativa
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir novo desconto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverDescontoAcrescimo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(unitOfWork);
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto descontoAcrescimo = repPagamentoAgregadoAcrescimoDesconto.BuscarPorCodigo(codigo);

                repPagamentoAgregadoAcrescimoDesconto.Deletar(descontoAcrescimo);
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o desconto/acréscimo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarExtratoPagamentoAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento repPagamentoAgregadoAbastecimento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento repPagamentoAgregadoAdiantamento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));
                if (codigo == 0)
                    return new JsonpResult(false, false, "Favor selecione um registro para gerar o seu relatório.");

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R277_ExtratoPagamentoAgregado, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                {
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R277_ExtratoPagamentoAgregado, TipoServicoMultisoftware, "Relatorio de Extrato ao Agregado", "PagamentosAgregados", "ExtratoPagamentoAgregado.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);
                }

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamentoAgregado = repPagamentoAgregado.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento> abastecimentos = repPagamentoAgregadoAbastecimento.BuscarPorPagamento(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> acrescimos = repPagamentoAgregadoAcrescimoDesconto.BuscarPorPagamentoETipo(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> descontos = repPagamentoAgregadoAcrescimoDesconto.BuscarPorPagamentoETipo(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento> adiantamentos = repPagamentoAgregadoAdiantamento.BuscarPorPagamento(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento> documentos = repPagamentoAgregadoDocumento.BuscarPorPagamento(codigo);

                List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregado> extratoPagamentoAgregado = new List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregado>();
                List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoFretes> extratoPagamentoAgregadoFretes = new List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoFretes>();
                List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAcrescimos> extratoPagamentoAgregadoAcrescimos = new List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAcrescimos>();
                List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAdiantamentos> extratoPagamentoAgregadoAdiantamentos = new List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAdiantamentos>();
                List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoDescontos> extratoPagamentoAgregadoDescontos = new List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoDescontos>();
                List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoTributos> extratoPagamentoAgregadoTributos = new List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoTributos>();
                List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAbastecimentos> extratoPagamentoAgregadoAbastecimentos = new List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAbastecimentos>();

                int sequencia = 1;
                decimal creditos = 0m;
                decimal debitos = 0m;
                decimal saldo = 0m;
                decimal valorSESTSENAT = 0m;
                decimal valorINSS = 0m;
                decimal valorIRRF = 0m;
                foreach (var acrescimo in acrescimos)
                {
                    saldo += acrescimo.Valor;
                    creditos += acrescimo.Valor;
                    extratoPagamentoAgregadoAcrescimos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAcrescimos()
                    {
                        CodigoPagamento = pagamentoAgregado.Codigo,
                        Credito = acrescimo.Valor,
                        Data = pagamentoAgregado.DataPagamento.ToString("dd/MM/yyyy"),
                        Debito = 0m,
                        Descricao = acrescimo.Justificativa.Descricao,
                        Saldo = saldo,
                        Sequencia = sequencia
                    });
                    sequencia++;
                }
                foreach (var documento in documentos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarTodosPorCTe(documento.ConhecimentoDeTransporteEletronico.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargasCTe)
                        pedidos.AddRange(repCargaPedido.BuscarPedidosPorCarga(cargaCte.Carga.Codigo));

                    saldo += documento.Valor;
                    creditos += documento.Valor;
                    extratoPagamentoAgregadoFretes.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoFretes()
                    {
                        CodigoPagamento = pagamentoAgregado.Codigo,
                        Credito = documento.Valor,
                        Debito = 0m,
                        Data = documento.ConhecimentoDeTransporteEletronico.DataEmissao.Value.ToString("dd/MM/yyyy"),
                        Saldo = saldo,
                        Descricao = "DOC: " + documento.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString("D") + "/" + documento.ConhecimentoDeTransporteEletronico.Numero.ToString("D") + " - " + documento.ConhecimentoDeTransporteEletronico.LocalidadeInicioPrestacao.DescricaoCidadeEstado + " X " + documento.ConhecimentoDeTransporteEletronico.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                        Sequencia = sequencia,
                        DataColeta = string.Join(", ", pedidos.Select(obj => obj.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? ""))
                    });

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(documento.ConhecimentoDeTransporteEletronico.Codigo);
                    if (cargaCTe != null)
                    {
                        Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(cargaCTe.Carga.Codigo);
                        valorSESTSENAT += (contratoFrete?.ValorSEST ?? 0m) + (contratoFrete?.ValorSENAT ?? 0m);
                        valorINSS += contratoFrete?.ValorINSS ?? 0m;
                        valorIRRF += contratoFrete?.ValorIRRF ?? 0m;

                        if (valorSESTSENAT < 0)
                            valorSESTSENAT = valorSESTSENAT * -1;
                        if (valorINSS < 0)
                            valorINSS = valorINSS * -1;
                        if (valorIRRF < 0)
                            valorIRRF = valorIRRF * -1;
                    }

                    if (documento.ConhecimentoDeTransporteEletronico.Motoristas != null && documento.ConhecimentoDeTransporteEletronico.Motoristas.Count > 0)
                    {
                        foreach (var motorista in documento.ConhecimentoDeTransporteEletronico.Motoristas)
                        {
                            if (!extratoPagamentoAgregado.Any(c => c.Motorista == motorista.NomeMotorista))
                            {
                                extratoPagamentoAgregado.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregado()
                                {
                                    Agencia = pagamentoAgregado.Cliente.Agencia + " " + pagamentoAgregado.Cliente.DigitoAgencia,
                                    Banco = pagamentoAgregado.Cliente.Banco?.Descricao,
                                    Cavalo = documento.ConhecimentoDeTransporteEletronico.Veiculos != null ? documento.ConhecimentoDeTransporteEletronico.Veiculos.Where(c => c.TipoVeiculo == "0")?.Select(c => c.Placa)?.FirstOrDefault() : "",
                                    CodigoIntegracao = pagamentoAgregado.Cliente.CodigoIntegracao,
                                    CodigoPagamento = pagamentoAgregado.Codigo,
                                    ContaCorrente = pagamentoAgregado.Cliente.NumeroConta,
                                    CPFCNPJ = pagamentoAgregado.Cliente.CPF_CNPJ_Formatado,
                                    Motorista = motorista.NomeMotorista,
                                    Nome = pagamentoAgregado.Cliente.Nome,
                                    Numero = pagamentoAgregado.Numero,
                                    PIX = pagamentoAgregado.Cliente.ChavePix + " (" + (pagamentoAgregado.Cliente.TipoChavePix.HasValue ? pagamentoAgregado.Cliente.TipoChavePix.Value.ObterDescricao() : "") + ")",
                                    Saldo = 0m,
                                    VeiculosVinculados = documento.ConhecimentoDeTransporteEletronico.Veiculos != null ? string.Join(", ", documento.ConhecimentoDeTransporteEletronico.Veiculos.Where(c => c.TipoVeiculo == "1")?.Select(c => c.Placa)?.ToList()) : ""
                                });
                            }
                        }
                    }

                    sequencia++;
                }
                foreach (var abastecimento in abastecimentos)
                {
                    saldo -= abastecimento.Abastecimento.ValorTotal;
                    debitos += abastecimento.Abastecimento.ValorTotal;
                    extratoPagamentoAgregadoAbastecimentos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAbastecimentos()
                    {
                        CodigoPagamento = pagamentoAgregado.Codigo,
                        Credito = 0m,
                        Data = abastecimento.Abastecimento.Data.Value.ToString("dd/MM/yyyy"),
                        Debito = abastecimento.Abastecimento.ValorTotal,
                        Descricao = "Doc.:" + abastecimento.Abastecimento.Documento + " Placa: " + abastecimento.Abastecimento.Veiculo.Placa + " KM: " + abastecimento.Abastecimento.Kilometragem.ToString("n0") + "Qtd: " + abastecimento.Abastecimento.Litros.ToString("n4"),
                        Saldo = saldo,
                        Sequencia = sequencia
                    });
                    sequencia++;
                }
                foreach (var desconto in descontos)
                {
                    saldo -= desconto.Valor;
                    debitos += desconto.Valor;
                    extratoPagamentoAgregadoDescontos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoDescontos()
                    {
                        CodigoPagamento = pagamentoAgregado.Codigo,
                        Credito = 0m,
                        Data = pagamentoAgregado.DataPagamento.ToString("dd/MM/yyyy"),
                        Debito = desconto.Valor,
                        Descricao = desconto.Justificativa.Descricao,
                        Saldo = saldo,
                        Sequencia = sequencia
                    });
                    sequencia++;
                }
                foreach (var adiantamento in adiantamentos)
                {
                    saldo -= adiantamento.PagamentoMotoristaTMS.Valor;
                    debitos += adiantamento.PagamentoMotoristaTMS.Valor;
                    extratoPagamentoAgregadoAdiantamentos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAdiantamentos()
                    {
                        CodigoPagamento = pagamentoAgregado.Codigo,
                        Credito = 0m,
                        Data = adiantamento.PagamentoMotoristaTMS.DataPagamento.ToString("dd/MM/yyyy"),
                        Debito = adiantamento.PagamentoMotoristaTMS.Valor,
                        Descricao = "ADIANTAMENTO " + adiantamento.PagamentoMotoristaTMS.Descricao + " " + adiantamento.PagamentoMotoristaTMS.Observacao,
                        Saldo = saldo,
                        Sequencia = sequencia
                    });
                    sequencia++;
                }
                extratoPagamentoAgregadoTributos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoTributos()
                {
                    CodigoPagamento = pagamentoAgregado.Codigo,
                    Credito = 0m,
                    Data = pagamentoAgregado.DataPagamento.ToString("dd/MM/yyyy"),
                    Debito = valorSESTSENAT,
                    Descricao = "DESCONTO SEST/SENAT",
                    Saldo = 0m,
                    Sequencia = -1
                });
                extratoPagamentoAgregadoTributos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoTributos()
                {
                    CodigoPagamento = pagamentoAgregado.Codigo,
                    Credito = 0m,
                    Data = pagamentoAgregado.DataPagamento.ToString("dd/MM/yyyy"),
                    Debito = valorINSS,
                    Descricao = "DESCONTO INSS",
                    Saldo = 0m,
                    Sequencia = -1
                });
                extratoPagamentoAgregadoTributos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoTributos()
                {
                    CodigoPagamento = pagamentoAgregado.Codigo,
                    Credito = 0m,
                    Data = pagamentoAgregado.DataPagamento.ToString("dd/MM/yyyy"),
                    Debito = valorIRRF,
                    Descricao = "DESCONTO IRRF",
                    Saldo = 0m,
                    Sequencia = -1
                });

                if (extratoPagamentoAgregadoAbastecimentos == null || extratoPagamentoAgregadoAbastecimentos.Count == 0)
                {
                    extratoPagamentoAgregadoAbastecimentos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAbastecimentos()
                    {
                        CodigoPagamento = -1
                    });
                }
                if (extratoPagamentoAgregadoAdiantamentos == null || extratoPagamentoAgregadoAdiantamentos.Count == 0)
                {
                    extratoPagamentoAgregadoAdiantamentos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAdiantamentos()
                    {
                        CodigoPagamento = -1
                    });
                }
                if (extratoPagamentoAgregadoFretes == null || extratoPagamentoAgregadoFretes.Count == 0)
                {
                    extratoPagamentoAgregadoFretes.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoFretes()
                    {
                        CodigoPagamento = -1
                    });
                }
                if (extratoPagamentoAgregadoDescontos == null || extratoPagamentoAgregadoDescontos.Count == 0)
                {
                    extratoPagamentoAgregadoDescontos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoDescontos()
                    {
                        CodigoPagamento = -1
                    });
                }
                if (extratoPagamentoAgregadoAcrescimos == null || extratoPagamentoAgregadoAcrescimos.Count == 0)
                {
                    extratoPagamentoAgregadoAcrescimos.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAcrescimos()
                    {
                        CodigoPagamento = -1
                    });
                }

                foreach (var extrato in extratoPagamentoAgregado)
                    extrato.Saldo = saldo;

                if (extratoPagamentoAgregado.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioExtratoPagamentoAgregado(codigo, nomeCliente, stringConexao, relatorioControleGeracao, extratoPagamentoAgregado,
                    extratoPagamentoAgregadoFretes,
                    extratoPagamentoAgregadoAcrescimos,
                    extratoPagamentoAgregadoAdiantamentos,
                    extratoPagamentoAgregadoDescontos,
                    extratoPagamentoAgregadoTributos,
                    extratoPagamentoAgregadoAbastecimentos));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        public async Task<IActionResult> GerarReciboPagamentoAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));
                if (codigo == 0)
                    return new JsonpResult(false, false, "Favor selecione um registro para gerar o seu relatório.");

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R108_ReciboPagamentoAgregado, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                {
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R108_ReciboPagamentoAgregado, TipoServicoMultisoftware, "Relatorio de Recibo ao Agregado", "PagamentosAgregados", "ReciboPagamentoAgregado.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);
                }

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;

                IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregado> dadosPagamentoAgregado = repPagamentoAgregado.RelatorioReciboPagamentoAgregado(codigo, this.Usuario.Empresa.Codigo);
                IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoAdiantamento> dadosPagamentoAgregadoAdiantamento = repPagamentoAgregado.RelatorioReciboPagamentoAgregadoAdiantamento(codigo);
                IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDescontoAcrescimo> dadosPagamentoAgregadoDescontoAcrescimo = repPagamentoAgregado.RelatorioReciboPagamentoAgregadoDescontoAcrescimo(codigo);
                if (dadosPagamentoAgregadoDescontoAcrescimo == null || dadosPagamentoAgregadoDescontoAcrescimo.Count == 0)
                {
                    if (dadosPagamentoAgregadoDescontoAcrescimo == null)
                        dadosPagamentoAgregadoDescontoAcrescimo = new List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDescontoAcrescimo>();

                    dadosPagamentoAgregadoDescontoAcrescimo.Add(new Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDescontoAcrescimo
                    {
                        Descricao = "",
                        Tipo = 1,
                        Valor = 0
                    });

                }
                IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDocumento> dadosPagamentoAgregadoDocumento = repPagamentoAgregado.RelatorioReciboPagamentoAgregadoDocumento(codigo);

                if (dadosPagamentoAgregado.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioReciboPagamentoAgregado(codigo, nomeCliente, stringConexao, relatorioControleGeracao, dadosPagamentoAgregado, dadosPagamentoAgregadoAdiantamento, dadosPagamentoAgregadoDescontoAcrescimo, dadosPagamentoAgregadoDocumento));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> GerarFaturaPagamentoAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("PagamentoAgregado");

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repositorioPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamentoAgregado = repositorioPagamentoAgregado.BuscarPorCodigo(codigo);

                if (pagamentoAgregado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var pdf = ReportRequest.WithType(ReportType.FaturaPagamentoAgregado)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigo", codigo)
                    .CallReport()
                    .GetContentFile();
                return Arquivo(pdf, "application/pdf", $"Fatura do Pagamento Agregado {pagamentoAgregado.Numero}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar a fatura do pagamento agregado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPagamentoAgregado = 0;
                int.TryParse(Request.Params("CodigoPagamentoAgregado"), out codigoPagamentoAgregado);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                unitOfWork.Start();

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAnexo repPagamentoAgregadoAnexo = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAnexo(unitOfWork);

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "PagamentoAgregado");
                
                for (var i = 0; i < arquivos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAnexo pagamentoAgregadoAnexo = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAnexo();

                    Servicos.DTO.CustomFile file = arquivos[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = Path.GetExtension(nomeArquivo).ToLower();
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);
                    
                    file.SaveAs(caminho);

                    pagamentoAgregadoAnexo.CaminhoArquivo = caminho;
                    pagamentoAgregadoAnexo.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(Path.GetFileName(nomeArquivo)));
                    pagamentoAgregadoAnexo.Descricao = descricoes[i];
                    pagamentoAgregadoAnexo.PagamentoAgregado = repPagamentoAgregado.BuscarPorCodigo(codigoPagamentoAgregado, false);

                    repPagamentoAgregadoAnexo.Inserir(pagamentoAgregadoAnexo, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoAgregadoAnexo.PagamentoAgregado, null, "Adicionou o anexo " + pagamentoAgregadoAnexo.NomeArquivo + ".", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAnexo = 0;
                int.TryParse(Request.Params("CodigoAnexo"), out codigoAnexo);

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAnexo repPagamentoAgregadoAnexo = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAnexo pagamentoAgregadoAnexo = repPagamentoAgregadoAnexo.BuscarPorCodigo(codigoAnexo, false);

                if (pagamentoAgregadoAnexo == null)
                    return new JsonpResult(false, "Anexo não encontrado no Banco de Dados.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(pagamentoAgregadoAnexo.CaminhoArquivo))
                    return new JsonpResult(false, "Anexo não encontrado no Servidor.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(pagamentoAgregadoAnexo.CaminhoArquivo), "image/jpeg", pagamentoAgregadoAnexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter o Anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void GerarRelatorioExtratoPagamentoAgregado(int codigo, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregado> extratoPagamentoAgregado,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoFretes> extratoPagamentoAgregadoFretes,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAcrescimos> extratoPagamentoAgregadoAcrescimos,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAdiantamentos> extratoPagamentoAgregadoAdiantamentos,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoDescontos> extratoPagamentoAgregadoDescontos,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoTributos> extratoPagamentoAgregadoTributos,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAbastecimentos> extratoPagamentoAgregadoAbastecimentos)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            Servicos.Embarcador.PagamentoAgregado.PagamentoAgregado serPagamentoAgregado = new Servicos.Embarcador.PagamentoAgregado.PagamentoAgregado();
            try
            {
                var result = ReportRequest.WithType(ReportType.ExtratoPagamentoAgregado)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("nomeEmpresa", nomeEmpresa)
                    .AddExtraData("extratoPagamentoAgregado", extratoPagamentoAgregado.ToJson())
                    .AddExtraData("extratoPagamentoAgregadoFretes", extratoPagamentoAgregadoFretes.ToJson())
                    .AddExtraData("extratoPagamentoAgregadoAcrescimos", extratoPagamentoAgregadoAcrescimos.ToJson())
                    .AddExtraData("extratoPagamentoAgregadoAdiantamentos", extratoPagamentoAgregadoAdiantamentos.ToJson())
                    .AddExtraData("extratoPagamentoAgregadoDescontos", extratoPagamentoAgregadoDescontos.ToJson())
                    .AddExtraData("extratoPagamentoAgregadoTributos", extratoPagamentoAgregadoTributos.ToJson())
                    .AddExtraData("extratoPagamentoAgregadoAbastecimentos", extratoPagamentoAgregadoAbastecimentos.ToJson())
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void GerarRelatorioReciboPagamentoAgregado(int codigo, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregado> dadosPagamentoAgregado,
            IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoAdiantamento> dadosPagamentoAgregadoAdiantamento,
            IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDescontoAcrescimo> dadosPagamentoAgregadoDescontoAcrescimo,
            IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDocumento> dadosPagamentoAgregadoDocumento)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            Servicos.Embarcador.PagamentoAgregado.PagamentoAgregado serPagamentoAgregado = new Servicos.Embarcador.PagamentoAgregado.PagamentoAgregado();
            try
            {
                var result = ReportRequest.WithType(ReportType.ReciboPagamentoAgregado)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("nomeEmpresa", nomeEmpresa)
                    .AddExtraData("dadosPagamentoAgregado", dadosPagamentoAgregado.ToJson())
                    .AddExtraData("dadosPagamentoAgregadoAdiantamento", dadosPagamentoAgregadoAdiantamento.ToJson())
                    .AddExtraData("dadosPagamentoAgregadoDescontoAcrescimo", dadosPagamentoAgregadoDescontoAcrescimo.ToJson())
                    .AddExtraData("dadosPagamentoAgregadoDocumento", dadosPagamentoAgregadoDocumento.ToJson())
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(5).Align(Models.Grid.Align.left);
            grid.Prop("NumerosContratos").Nome("Contratos").Tamanho(9).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("Data").Nome("Data Do Pagamento").Tamanho(20).Align(Models.Grid.Align.center);
            grid.Prop("Valor").Nome("Valor").Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("Cliente").Nome("Agregado").Tamanho(20);
            grid.Prop("Situacao").Nome("Situação").Tamanho(10);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);

            // Dados do filtro
            DateTime.TryParse(Request.Params("DataInicio"), out DateTime dataInicio);
            DateTime.TryParse(Request.Params("DataFim"), out DateTime dataFim);

            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("NumeroContrato"), out int numeroContrato);

            double.TryParse(Request.Params("Cliente"), out double cliente);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado situacaoAux))
                situacao = situacaoAux;

            List<int> codigosEmpresa = new List<int>();
            if ((this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null && this.Usuario?.Empresas?.Count > 0)
            {
                codigosEmpresa = Request.GetListParam<int>("Empresa");
                if (codigosEmpresa == null || codigosEmpresa.Count == 0)
                    codigosEmpresa = this.Usuario.Empresas.Select(c => c.Codigo).ToList();
            }

            // Consulta
            List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> listaGrid = repPagamentoAgregado.Consultar(numero, numeroContrato, dataInicio, dataFim, cliente, situacao, codigosEmpresa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repPagamentoAgregado.ContarConsulta(numero, numeroContrato, dataInicio, dataFim, cliente, situacao, codigosEmpresa);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            obj.Numero,
                            obj.NumerosContratos,
                            Data = obj.DataPagamento.ToString("dd/MM/yyyy"),
                            Valor = obj.Valor.ToString("n3"),
                            Cliente = obj.Cliente.Nome,
                            Situacao = obj.DescricaoSituacao
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            double.TryParse(Request.Params("Cliente"), out double cliente);
            decimal.TryParse(Request.Params("Valor"), out decimal valor);
            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);
            DateTime.TryParseExact(Request.Params("DataInicialOcorrencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicialOcorrencia);
            DateTime.TryParseExact(Request.Params("DataFinalOcorrencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinalOcorrencia);

            DateTime.TryParseExact(Request.Params("DataPagamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataPagamento);
            string observacao = Request.Params("Observacao");

            int codigoVeiculo = Request.GetIntParam("Veiculo");
            double codigoTomadorFatura = Request.GetDoubleParam("TomadorFatura");
            int codigoEmpresa = Request.GetIntParam("Empresa");

            // Vincula dados            
            pagamento.Cliente = repCliente.BuscarPorCPFCNPJ(cliente);
            pagamento.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

            if (dataFinal > DateTime.MinValue)
                pagamento.DataFinal = dataFinal;
            else
                pagamento.DataFinal = null;
            if (dataInicial > DateTime.MinValue)
                pagamento.DataInicial = dataInicial;
            else
                pagamento.DataInicial = null;
            if (dataFinalOcorrencia > DateTime.MinValue)
                pagamento.DataFinalOcorrencia = dataFinalOcorrencia;
            else
                pagamento.DataFinalOcorrencia = null;
            if (dataInicialOcorrencia > DateTime.MinValue)
                pagamento.DataInicialOcorrencia = dataInicialOcorrencia;
            else
                pagamento.DataInicialOcorrencia = null;

            pagamento.DataPagamento = dataPagamento;
            pagamento.Observacao = observacao;
            pagamento.Valor = valor;

            pagamento.TomadorFatura = repCliente.BuscarPorCPFCNPJ(codigoTomadorFatura);
            pagamento.NumeroFatura = Request.GetStringParam("NumeroFatura");
            pagamento.CompetenciaMes = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes>("CompetenciaMes");
            pagamento.CompetenciaQuinzena = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.Quinzena>("CompetenciaQuinzena");
            pagamento.DescricaoCompetencia = Request.GetStringParam("DescricaoCompetencia");
            pagamento.Empresa = repositorioEmpresa.BuscarPorCodigo(codigoEmpresa);

            // Dados Criacao
            if (pagamento.Codigo == 0)
            {
                pagamento.Data = DateTime.Now;
                pagamento.Usuario = this.Usuario;
                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Iniciada;
                pagamento.Numero = repPagamentoAgregado.BuscarProximoNumero();
            }
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, out string msgErro)
        {
            msgErro = "";

            if (pagamento.Cliente == null)
            {
                msgErro = "Agregado é obrigatório.";
                return false;
            }

            if (pagamento.Codigo > 0 && pagamento.Valor <= 0)
            {
                msgErro = "O valor para o pagamento não pode estar zerado.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Cliente") propOrdenar = "Cliente.Nome";
        }

        private string EtapaAprovacao(ref Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = string.Empty;
            // Instancia Repositorios
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);

            List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado> regras = Servicos.Embarcador.PagamentoAgregado.PagamentoAgregado.VerificarRegrasAutorizacaoPagamento(pagamento, unitOfWork);

            bool possuiRegra = regras.Count() > 0;
            bool agAprovacao = true;

            if (possuiRegra)
            {
                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.AgAprovacao;

                agAprovacao = Servicos.Embarcador.PagamentoAgregado.PagamentoAgregado.CriarRegrasAutorizacao(regras, pagamento, pagamento.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);

                if (!agAprovacao)
                    pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Finalizado;

                Servicos.Embarcador.PagamentoAgregado.PagamentoAgregado.PagamentoAgregadoAprovado(out retorno, pagamento, unitOfWork, Auditado, TipoServicoMultisoftware);
            }
            else
            {
                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.SemRegra;
            }
            return retorno;
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado regra)
        {
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Info;
            else
                return "";
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado regra)
        {
            return regra.RegraPagamentoAgregado?.Descricao;
        }

        private void AtualizarDocumentosPagamento(ref bool contemDocumentosSemOcorrenciaFinalizadora, ref bool contemDocumentos, ref decimal valorDocumentos, Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, Repositorio.UnitOfWork unidadeDeTrabalho, string listaDocumentos, bool validarOcorrenciaFinalizadora)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            List<int> listaDocumento = repPagamentoAgregadoDocumento.BuscarCodigosPorPagamento(pagamento.Codigo);
            List<int> codigoDocumentos = new List<int>();
            valorDocumentos = 0;
            contemDocumentos = false;
            contemDocumentosSemOcorrenciaFinalizadora = false;
            dynamic listaDocs = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(listaDocumentos);
            foreach (var doc in listaDocs)
            {
                int codioInterno = 0;
                int.TryParse((string)doc.Documento.CodigoDocumentoPagamentoAgregado, out codioInterno);
                codigoDocumentos.Add(codioInterno);
                if (!listaDocumento.Contains(int.Parse((string)doc.Documento.CodigoDocumentoPagamentoAgregado)))
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento documento = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento();

                    documento.PagamentoAgregado = pagamento;
                    documento.ConhecimentoDeTransporteEletronico = repCTe.BuscarPorCodigo(int.Parse((string)doc.Documento.Codigo));
                    documento.Valor = Utilidades.Decimal.Converter((string)doc.Documento.ValorPagamento);
                    valorDocumentos += documento.Valor;
                    repPagamentoAgregadoDocumento.Inserir(documento);
                    contemDocumentos = true;
                    if (validarOcorrenciaFinalizadora && !contemDocumentosSemOcorrenciaFinalizadora)
                    {
                        contemDocumentosSemOcorrenciaFinalizadora = !repCargaOcorrenciaDocumento.ContemOcorrenciaFinalizadora(int.Parse((string)doc.Documento.Codigo));
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento documento = repPagamentoAgregadoDocumento.BuscarPorCodigo(int.Parse((string)doc.Documento.CodigoDocumentoPagamentoAgregado));
                    if (documento != null)
                    {
                        documento.PagamentoAgregado = pagamento;
                        documento.ConhecimentoDeTransporteEletronico = repCTe.BuscarPorCodigo(int.Parse((string)doc.Documento.Codigo));
                        documento.Valor = Utilidades.Decimal.Converter((string)doc.Documento.ValorPagamento);
                        valorDocumentos += documento.Valor;
                        repPagamentoAgregadoDocumento.Atualizar(documento);
                        contemDocumentos = true;
                        if (validarOcorrenciaFinalizadora && !contemDocumentosSemOcorrenciaFinalizadora)
                        {
                            contemDocumentosSemOcorrenciaFinalizadora = !repCargaOcorrenciaDocumento.ContemOcorrenciaFinalizadora(int.Parse((string)doc.Documento.Codigo));
                        }
                    }
                }
            }
            for (int i = 0; i < listaDocumento.Count; i++)
            {
                if (!codigoDocumentos.Contains(listaDocumento[i]))
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento documentoExcluir = repPagamentoAgregadoDocumento.BuscarPorCodigo(listaDocumento[i]);
                    repPagamentoAgregadoDocumento.Deletar(documentoExcluir);
                }
            }
        }

        private void AtualizarAdiantamentoPagamento(ref decimal valorAdiantamentos, Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, Repositorio.UnitOfWork unidadeDeTrabalho, string listaAdiantamentos)
        {
            valorAdiantamentos = 0;
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento repPagamentoAgregadoAdiantamento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);

            List<int> listaAdiantamento = repPagamentoAgregadoAdiantamento.BuscarCodigosPorPagamento(pagamento.Codigo);
            List<int> codigoAdiantamentos = new List<int>();

            dynamic listaAdiants = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(listaAdiantamentos);
            foreach (var adiant in listaAdiants)
            {
                int codioInterno = 0;
                int.TryParse((string)adiant.Adiantamento.CodigoAdiantamentoPagamentoAgregado, out codioInterno);
                codigoAdiantamentos.Add(codioInterno);
                if (!listaAdiantamento.Contains(((string)adiant.Adiantamento.CodigoAdiantamentoPagamentoAgregado).ToInt()))
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento adiantamento = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento();

                    adiantamento.PagamentoAgregado = pagamento;
                    adiantamento.PagamentoMotoristaTMS = repPagamentoMotoristaTMS.BuscarPorCodigo(((string)adiant.Adiantamento.Codigo).ToInt());
                    valorAdiantamentos += adiantamento.PagamentoMotoristaTMS.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista);
                    repPagamentoAgregadoAdiantamento.Inserir(adiantamento);
                }
                else
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento adiantamento = repPagamentoAgregadoAdiantamento.BuscarPorCodigo(((string)adiant.Adiantamento.CodigoAdiantamentoPagamentoAgregado).ToInt());
                    if (adiantamento != null)
                    {
                        adiantamento.PagamentoAgregado = pagamento;
                        adiantamento.PagamentoMotoristaTMS = repPagamentoMotoristaTMS.BuscarPorCodigo(((string)adiant.Adiantamento.Codigo).ToInt());
                        valorAdiantamentos += adiantamento.PagamentoMotoristaTMS.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista);
                        repPagamentoAgregadoAdiantamento.Atualizar(adiantamento);
                    }
                }
            }
            for (int i = 0; i < listaAdiantamento.Count; i++)
            {
                if (!codigoAdiantamentos.Contains(listaAdiantamento[i]))
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento adiantamentoExcluir = repPagamentoAgregadoAdiantamento.BuscarPorCodigo(listaAdiantamento[i]);
                    repPagamentoAgregadoAdiantamento.Deletar(adiantamentoExcluir);
                }
            }
        }

        private void AtualizarAbastecimentoPagamento(ref decimal valorAbastecimentos, Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, Repositorio.UnitOfWork unidadeDeTrabalho, string listaAbastecimentos)
        {
            valorAbastecimentos = 0;
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento repPagamentoAgregadoAbastecimento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento(unidadeDeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);

            List<int> listaAbastecimento = repPagamentoAgregadoAbastecimento.BuscarCodigosPorPagamento(pagamento.Codigo);
            List<int> codigoAbastecimentos = new List<int>();

            dynamic listaAbasts = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(listaAbastecimentos);
            foreach (var abast in listaAbasts)
            {
                int codioInterno = 0;
                int.TryParse((string)abast.Abastecimento.CodigoAbastecimentoPagamentoAgregado, out codioInterno);
                codigoAbastecimentos.Add(codioInterno);
                if (!listaAbastecimento.Contains(((string)abast.Abastecimento.CodigoAbastecimentoPagamentoAgregado).ToInt()))
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento abastecimento = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento();

                    abastecimento.PagamentoAgregado = pagamento;
                    abastecimento.Abastecimento = repAbastecimento.BuscarPorCodigo(((string)abast.Abastecimento.Codigo).ToInt());
                    valorAbastecimentos += abastecimento.Abastecimento.ValorTotal;
                    //valorAbastecimentos += abastecimento.Abastecimento.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista);
                    repPagamentoAgregadoAbastecimento.Inserir(abastecimento);
                }
                else
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento abastecimento = repPagamentoAgregadoAbastecimento.BuscarPorCodigo(((string)abast.Abastecimento.CodigoAbastecimentoPagamentoAgregado).ToInt());
                    if (abastecimento != null)
                    {
                        abastecimento.PagamentoAgregado = pagamento;
                        abastecimento.Abastecimento = repAbastecimento.BuscarPorCodigo(((string)abast.Abastecimento.Codigo).ToInt());
                        valorAbastecimentos += abastecimento.Abastecimento.ValorTotal;
                        //valorAbastecimentos += abastecimento.Abastecimento.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista);
                        repPagamentoAgregadoAbastecimento.Atualizar(abastecimento);
                    }
                }
            }
            for (int i = 0; i < listaAbastecimento.Count; i++)
            {
                if (!codigoAbastecimentos.Contains(listaAbastecimento[i]))
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento abastecimentoExcluir = repPagamentoAgregadoAbastecimento.BuscarPorCodigo(listaAbastecimento[i]);
                    repPagamentoAgregadoAbastecimento.Deletar(abastecimentoExcluir);
                }
            }
        }

        private string UltimaOcorrenciaDocumento(int codigoDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento ocorrencia = repCargaOcorrenciaDocumento.BuscarUltimaOcorrenciaPorDocumento(codigoDocumento);
            if (ocorrencia != null)
                return ocorrencia.CargaOcorrencia.TipoOcorrencia?.Descricao;
            else
                return "";
        }

        private void ExcluirAnexos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAnexo repPagamentoAgregadoAnexo = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAnexo(unitOfWork);

            dynamic listaAnexos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAnexosExcluidos"));
            if (listaAnexos.Count > 0)
            {
                foreach (var anexo in listaAnexos)
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAnexo pagamentoAgregadoAnexo = repPagamentoAgregadoAnexo.BuscarPorCodigo((int)anexo.Codigo, true);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(pagamentoAgregadoAnexo.CaminhoArquivo))
                        Utilidades.IO.FileStorageService.Storage.Delete(pagamentoAgregadoAnexo.CaminhoArquivo);

                    repPagamentoAgregadoAnexo.Deletar(pagamentoAgregadoAnexo, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoAgregadoAnexo.PagamentoAgregado, null, "Removeu o anexo " + pagamentoAgregadoAnexo.NomeArquivo + ".", unitOfWork);
                }
            }
        }

        #endregion
    }
}
