using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/OcorrenciaLote")]
    public class OcorrenciaLoteController : BaseController
    {
		#region Construtores

		public OcorrenciaLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLote filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Operador", "Operador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Frete Líquido", "ValorFreteLiquido", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", 20, Models.Grid.Align.left, false);
                if (filtrosPesquisa.Situacao == SituacaoOcorrenciaLote.Todos)
                    grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Ocorrencias.OcorrenciaLote repOcorrenciaLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLote(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote> listaContratoFreteAcrescimoDesconto = repOcorrenciaLote.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repOcorrenciaLote.ContarConsulta(filtrosPesquisa));

                grid.AdicionaRows((from obj in listaContratoFreteAcrescimoDesconto
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Numero,
                                       Data = obj.Data.ToDateTimeString(),
                                       Operador = obj.Usuario.Nome,
                                       ValorFreteLiquido = obj.ValorFreteLiquido.ToString("n2"),
                                       TipoOcorrencia = obj.TipoOcorrencia.Descricao,
                                       Situacao = obj.Situacao.ObterDescricao()
                                   }).ToList());

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
        public async Task<IActionResult> PesquisaCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLoteCarga filtrosPesquisa = ObterFiltrosPesquisaCarga();

                bool clicouNaPesquisa = Request.GetBoolParam("ClicouNaPesquisa");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", "Destino", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", 15, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Ocorrencias.OcorrenciaLote repOcorrenciaLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLote(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaRegistros = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                int quantidadeRegistros = !clicouNaPesquisa ? 0 : repOcorrenciaLote.ContarConsultaCarga(filtrosPesquisa);
                if (quantidadeRegistros > 0)
                    listaRegistros = repOcorrenciaLote.ConsultarCarga(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(quantidadeRegistros);

                var lista = (from p in listaRegistros
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoCargaEmbarcador,
                                 Origem = p.DadosSumarizados.Origens,
                                 Destino = p.DadosSumarizados.Destinos,
                                 ValorFrete = p.ValorFrete.ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaOcorrencias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrenciaLote = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Ocorrência", "NumeroOcorrencia", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CT-es Gerados", "NumerosCTes", 35, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor da Ocorrência", "ValorOcorrencia", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Ocorrencias.OcorrenciaLote repOcorrenciaLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLote(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaRegistros = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

                int quantidadeRegistros = codigoOcorrenciaLote > 0 ? repOcorrenciaLote.ContarConsultaOcorrencia(codigoOcorrenciaLote) : 0;
                if (quantidadeRegistros > 0)
                    listaRegistros = repOcorrenciaLote.ConsultarOcorrencia(codigoOcorrenciaLote, parametrosConsulta);
                grid.setarQuantidadeTotal(quantidadeRegistros);

                var lista = (from p in listaRegistros
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroOcorrencia,
                                 p.NumerosCTes,
                                 ValorOcorrencia = p.ValorOcorrencia.ToString("n2"),
                                 p.DescricaoSituacao
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as ocorrências.");
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
                Repositorio.Embarcador.Ocorrencias.OcorrenciaLote repOcorrenciaLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLote(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas = ObterCargas(unitOfWork);

                unitOfWork.Start();

                int codigoTipoOcorrencia = Request.GetIntParam("TipoOcorrencia");

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote ocorrenciaLote = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote()
                {
                    Data = DateTime.Now,
                    Situacao = SituacaoOcorrenciaLote.EmGeracao,
                    Numero = repOcorrenciaLote.BuscarProximoNumero(),
                    Usuario = Usuario,
                    TipoRateio = Request.GetEnumParam<TipoRateioOcorrenciaLote>("TipoRateio"),
                    ValorFreteLiquido = Request.GetDecimalParam("ValorFreteLiquido"),
                    TipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoTipoOcorrencia)
                };

                repOcorrenciaLote.Inserir(ocorrenciaLote, Auditado);

                SalvarCargas(ocorrenciaLote, listaCargas, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(ObterDetalhesOcorrenciaLote(ocorrenciaLote));
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o lote de ocorrências.");
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

                Repositorio.Embarcador.Ocorrencias.OcorrenciaLote repOcorrenciaLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLote(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote ocorrenciaLote = repOcorrenciaLote.BuscarPorCodigo(codigo);

                if (ocorrenciaLote == null)
                    return new JsonpResult(false, true, "Lote não encontrado.");

                return new JsonpResult(ObterDetalhesOcorrenciaLote(ocorrenciaLote));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarOcorrenciasNaoGeradas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Ocorrencias.OcorrenciaLote repOcorrenciaLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLote(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote ocorrenciaLote = repOcorrenciaLote.BuscarPorCodigo(codigo);

                if (ocorrenciaLote == null)
                    return new JsonpResult(false, true, "Lote não encontrado.");

                if (ocorrenciaLote.Situacao != SituacaoOcorrenciaLote.FalhaNaGeracao)
                    return new JsonpResult(false, true, "Só é possível reprocessar na situação de Falha na Geração");

                unitOfWork.Start();

                ocorrenciaLote.Situacao = SituacaoOcorrenciaLote.EmGeracao;
                repOcorrenciaLote.Atualizar(ocorrenciaLote);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrenciaLote, null, "Reprocessou as ocorrências não geradas", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(ObterDetalhesOcorrenciaLote(ocorrenciaLote));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar ocorrências não geradas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterDetalhesOcorrenciaLote(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote ocorrenciaLote)
        {
            if (ocorrenciaLote == null)
                return null;

            var retorno = new
            {
                ocorrenciaLote.Codigo,
                ocorrenciaLote.Numero,
                ocorrenciaLote.Situacao,
                ocorrenciaLote.TipoRateio,
                ValorFreteLiquido = ocorrenciaLote.ValorFreteLiquido.ToString("n2"),
                TipoOcorrencia = new { ocorrenciaLote.TipoOcorrencia.Codigo, ocorrenciaLote.TipoOcorrencia.Descricao },
                ocorrenciaLote.MotivoRejeicao
            };

            return retorno;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.Carga> ObterCargas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaLote repOcorrenciaLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLote(unitOfWork);

            dynamic dynFiltrosCargas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("FiltrosCargas"));
            if (dynFiltrosCargas == null)
                throw new ControllerException("Filtros não foram encontrados");

            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLoteCarga filtrosPesquisa = ObterFiltrosPesquisaCarga(true, dynFiltrosCargas);

            bool selecionarTodos = ((string)dynFiltrosCargas.SelecionarTodos).ToBool();
            List<int> codigosCargas = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaCargas"));

            if (codigosCargas == null)
                throw new ControllerException("É necessário selecionar ao menos uma carga.");

            List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas = repOcorrenciaLote.ObterCargas(selecionarTodos, codigosCargas, filtrosPesquisa);

            if (listaCargas.Count == 0)
                throw new ControllerException("Nenhuma carga selecionada.");

            return listaCargas;
        }

        private void SalvarCargas(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote ocorrenciaLote, List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia repOcorrenciaLoteCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            TipoRateioOcorrenciaLote tipoRateio = ocorrenciaLote.TipoRateio;
            decimal valorFreteLiquido = ocorrenciaLote.ValorFreteLiquido;
            List<int> codigosCargas = listaCargas.Select(o => o.Codigo).ToList();

            decimal valorTotalParametroRateio = 0;
            if (tipoRateio == TipoRateioOcorrenciaLote.Peso)
                valorTotalParametroRateio = listaCargas.Sum(o => o.DadosSumarizados.PesoTotal);
            else if (tipoRateio == TipoRateioOcorrenciaLote.ValorMercadoria)
                valorTotalParametroRateio = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(codigosCargas);
            else if (tipoRateio == TipoRateioOcorrenciaLote.QuantidadeCTe)
                valorTotalParametroRateio = repCargaCTe.ContarCtesPorCarga(codigosCargas);

            decimal valorTotalRateado = 0;
            int count = 0;
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in listaCargas)
            {
                decimal valorRateado = 0;
                decimal percentualRateio = 0;
                count++;

                if (tipoRateio == TipoRateioOcorrenciaLote.Peso)
                {
                    decimal peso = carga.DadosSumarizados.PesoTotal;
                    if (peso == 0)
                        throw new ControllerException($"A carga {carga.CodigoCargaEmbarcador} não possui peso.");

                    percentualRateio = Math.Round((peso * 100) / valorTotalParametroRateio, 2);
                }
                else if (tipoRateio == TipoRateioOcorrenciaLote.ValorMercadoria)
                {
                    decimal valorMercadoria = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(carga.Codigo);
                    if (valorMercadoria == 0)
                        throw new ControllerException($"A carga {carga.CodigoCargaEmbarcador} não possui valor de mercadoria.");

                    percentualRateio = Math.Round((valorMercadoria * 100) / valorTotalParametroRateio, 2);
                }
                else if (tipoRateio == TipoRateioOcorrenciaLote.QuantidadeCTe)
                {
                    int quantidadeCTes = repCargaCTe.ContarCtesPorCarga(carga.Codigo);
                    if (quantidadeCTes == 0)
                        throw new ControllerException($"A carga {carga.CodigoCargaEmbarcador} não possui ctes.");

                    percentualRateio = Math.Round((quantidadeCTes * 100) / valorTotalParametroRateio, 2);
                }

                valorRateado = Math.Round(valorFreteLiquido * (percentualRateio / 100), 2);
                if (count == listaCargas.Count && valorTotalRateado + valorRateado != valorFreteLiquido)
                {
                    decimal diferenca = valorFreteLiquido - (valorTotalRateado + valorRateado);
                    valorRateado += diferenca;
                    if (valorRateado <= 0)
                        throw new ControllerException("Não foi possível aplicar a diferença para a carga " + carga.CodigoCargaEmbarcador);
                }

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia ocorrenciaLoteCargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia()
                {
                    OcorrenciaLote = ocorrenciaLote,
                    Carga = carga,
                    ValorOcorrenciaRateado = valorRateado
                };

                repOcorrenciaLoteCargaOcorrencia.Inserir(ocorrenciaLoteCargaOcorrencia);

                valorTotalRateado += valorRateado;
            }

            if (valorFreteLiquido != valorTotalRateado)
                throw new ControllerException("Não foi possível aplicar a difereça de valores no rateio.");
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLote ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLote()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigoTipoOcorrencia = Request.GetIntParam("TipoOcorrencia"),
                Situacao = Request.GetEnumParam<SituacaoOcorrenciaLote>("Situacao")
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLoteCarga ObterFiltrosPesquisaCarga(bool porDynamic = false, dynamic dynFiltrosCargas = null)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLoteCarga()
            {
                CodigoOcorrenciaLote = porDynamic ? 0 : Request.GetIntParam("Codigo"),
                CodigoGrupoPessoas = porDynamic ? ((string)dynFiltrosCargas.GrupoPessoas).ToInt() : Request.GetIntParam("GrupoPessoas"),
                CodigoTipoOperacao = porDynamic ? ((string)dynFiltrosCargas.TipoOperacao).ToInt() : Request.GetIntParam("TipoOperacao"),
                CnpjCpfRemetente = porDynamic ? ((string)dynFiltrosCargas.Remetente).ToDouble() : Request.GetDoubleParam("Remetente"),
                DataCriacaoInicial = porDynamic ? ((string)dynFiltrosCargas.DataCriacaoInicial).ToDateTime() : Request.GetDateTimeParam("DataCriacaoInicial"),
                DataCriacaoFinal = porDynamic ? ((string)dynFiltrosCargas.DataCriacaoFinal).ToDateTime() : Request.GetDateTimeParam("DataCriacaoFinal")
            };
        }

        #endregion
    }
}
