using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Painel
{
    [CustomAuthorize("Painel/Indicador")]
    public class IndicadorController : BaseController
    {
		#region Construtores

		public IndicadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterQuantidadesGerais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRota = 0, codigoCentroCarregamento = 0, codigoTransportador = 0, codigoVeiculo = 0, codigoFilial = 0;
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                double cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);
                DateTime dataInicialCarregamento = DateTime.Now;
                DateTime dataFinalCarregamento = DateTime.Now;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuantidadeCarga? tipoPai = null;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoQuantidadeCarga> quantidades = repCarga.BuscarQuantidades(codigoTransportador, codigoVeiculo, codigoFilial, codigoCentroCarregamento, codigoRota, dataInicialCarregamento, dataFinalCarregamento, cpfCnpjDestinatario, tipoPai);
                // TODO: ToList addRange
                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProdutoExpedido> produtoExpedido = repCargaPedidoProduto.ConsultarGraficoUnidadeMedida(dataInicialCarregamento, dataFinalCarregamento, codigoCentroCarregamento).ToList();
                produtoExpedido.AddRange(repCargaPedidoProduto.ConsultarGraficoProdutosExpedidos(dataInicialCarregamento, dataFinalCarregamento, codigoCentroCarregamento));

                quantidades = quantidades.Where(o => o.Quantidade > 0).ToList();
                produtoExpedido = produtoExpedido.Where(o => o.QuantidadeTotal > 0).ToList();
                var retorno = new
                {
                    Quantidades = quantidades,
                    ProdutosExpedidos = produtoExpedido
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as quantidades de cargas.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterCargasAtrazadas()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasAtrazadas = repCargaJanelaCarregamento.BuscarCargasEmAtrazo();

                var result = (from obj in cargasAtrazadas
                              select new
                              {
                                  NumeroCarga = obj.Carga.CodigoCargaEmbarcador,
                                  Destinos = obj.Carga.DadosSumarizados != null ? obj.Carga.DadosSumarizados.Destinos : "",
                                  Destinatarios = obj.Carga.DadosSumarizados != null ? obj.Carga.DadosSumarizados.Destinatarios : "",
                                  Peso = 0.ToString("n2"), //obj.Carga.PesoTotalNotasFiscais.ToString("n2"),
                                  Empresa = obj.Carga.Empresa != null ? (obj.Carga.Empresa.RazaoSocial + " - " + obj.Carga.Empresa.Localidade.DescricaoCidadeEstado) : "",
                                  Veiculo = obj.Carga.RetornarPlacas,
                                  Motorista = obj.Carga.RetornarMotoristas,
                                  obj.DiasAtraso,
                                  SituacaoJanela = obj.DescricaoSituacao,
                                  SituacaoCarga = obj.Carga.DescricaoSituacaoCarga,
                                  Filial = obj.Carga.Filial != null ? obj.Carga.Filial.Descricao : ""
                              }).ToList();
                return new JsonpResult(result);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar as cargas em atrazo.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterVeiculosAtrazados()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> cargaJanelaCarregamentoGuarita = repCargaJanelaCarregamentoGuarita.ConsultarVeiculosAtrazados(0, 10);

                var retorno = (from obj in cargaJanelaCarregamentoGuarita
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   DataCarregamento = obj.CargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:00"),
                                   Centro = obj.CargaJanelaCarregamento.CentroCarregamento.Descricao,
                                   NumeroCarga = obj.Carga != null ? obj.Carga.CodigoCargaEmbarcador : obj.PreCarga.NumeroPreCarga,
                                   Transportador = obj.CargaBase.Empresa.RazaoSocial,
                                   Motorista = (obj.CargaBase.ListaMotorista.FirstOrDefault()?.Nome ?? string.Empty),
                                   Veiculo = ObterPlacas(obj.CargaBase.Veiculo, obj.CargaBase.Veiculo.VeiculosVinculados),
                                   ModeloVeiculo = obj.CargaBase.ModeloVeicularCarga.Descricao,
                                   DescricaoSituacao = obj.Situacao.ObterDescricao(),
                                   Situacao = obj.Situacao,
                                   TempoAtraso = (DateTime.Now - obj.CargaJanelaCarregamento.InicioCarregamento).ToString(@"hh\:mm"),
                                   DescricaoTipoChegadaGuarita = obj.TipoChegadaGuarita.ObterDescricao()
                               }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar os veículos em atraso.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterVeiculosEmCarregamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> cargaJanelaCarregamentoGuarita = repCargaJanelaCarregamentoGuarita.ConsultarVeiculosEmCarregamento(0, 10);

                var retorno = (from obj in cargaJanelaCarregamentoGuarita
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   DataEntrada = obj.DataEntregaGuarita.Value.ToString("dd/MM/yyyy HH:00"),
                                   Centro = obj.CargaJanelaCarregamento.CentroCarregamento.Descricao,
                                   NumeroCarga = obj.Carga != null ? obj.Carga.CodigoCargaEmbarcador : obj.PreCarga.NumeroPreCarga,
                                   Transportador = obj.CargaBase.Empresa.RazaoSocial,
                                   Motorista = obj.CargaBase.ListaMotorista.FirstOrDefault()?.Nome ?? string.Empty,
                                   Veiculo = ObterPlacas(obj.CargaBase.Veiculo, obj.CargaBase.VeiculosVinculados),
                                   ModeloVeiculo = obj.CargaBase.ModeloVeicularCarga.Descricao,
                                   DescricaoSituacao = obj.Situacao.ObterDescricao(),
                                   Situacao = obj.Situacao,
                                   TempoEmCarregamento = (DateTime.Now - obj.DataEntregaGuarita.Value).ToString(@"hh\:mm"),
                                   DescricaoTipoChegadaGuarita = obj.TipoChegadaGuarita.ObterDescricao()
                               }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar os veículos em atraso.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterVeiculosAgFaturamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> cargaJanelaCarregamentoGuarita = repCargaJanelaCarregamentoGuarita.ConsultarVeiculosEmFaturamento(0, 10);

                var retorno = (from obj in cargaJanelaCarregamentoGuarita
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   DataCarregamento = obj.DataEntregaGuarita.Value.ToString("dd/MM/yyyy HH:00"),
                                   Centro = obj.CargaJanelaCarregamento.CentroCarregamento.Descricao,
                                   NumeroCarga = obj.Carga != null ? obj.Carga.CodigoCargaEmbarcador : obj.PreCarga.NumeroPreCarga,
                                   Transportador = obj.CargaBase.Empresa.RazaoSocial,
                                   Motorista = (obj.CargaBase.ListaMotorista.FirstOrDefault()?.Nome ?? string.Empty),
                                   Veiculo = ObterPlacas(obj.CargaBase.Veiculo, obj.CargaBase.Veiculo.VeiculosVinculados),
                                   ModeloVeiculo = obj.CargaBase.ModeloVeicularCarga.Descricao,
                                   DescricaoSituacao = obj.Situacao.ObterDescricao(),
                                   Situacao = obj.Situacao,
                                   TempoEmFaturamento = (DateTime.Now - obj.DataFinalCarregamento.Value).ToString(@"hh\:mm"),
                                   DescricaoTipoChegadaGuarita = obj.TipoChegadaGuarita.ObterDescricao()
                               }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar os veículos em atraso.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosChamados()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unidadeTrabalho);

                int tempoRetroativo = 4;
                DateTime dataRetroativa = DateTime.Now.AddHours(-tempoRetroativo);

                DateTime dataInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime dataFim = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> ultimosChamados = repChamado.BuscarUltimosChamados(dataRetroativa);
                IList<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadosPorMotivo> valoresPorMotivo = repChamado.RelacaoChamadosPorMotivo(dataInicio, dataFim);
                IList<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadosPorUsuarios> relacaoPorUsuarios = repChamado.RelacaoChamadosPorUsuario(dataInicio, dataFim);

                IList<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.RelacaoOcorrencia> relacaoPorOcorrencia = repChamado.RelacaoPorValorOcorrencia(dataInicio, dataFim);

                List<string> coresMotivo = new List<string>()
                {
                    "#e29e23", "#dec27b", "#292929", "#85de7b", "#de7b7b",
                    "#b94747", "#1667c6", "#66cdaa", "#3d613c", "#6655a0",
                    "#739e73", "#c8e8ff", "#967bde", "#5591a0", "#9c1010"
                };

                List<string> coresRelacaoOcorrencia = new List<string>()
                {
                    "#e29e23", "#dec27b", "#292929", "#85de7b", "#de7b7b",
                    "#b94747", "#1667c6", "#66cdaa", "#3d613c", "#6655a0",
                    "#739e73", "#c8e8ff", "#967bde", "#5591a0", "#9c1010"
                };

                // Adiciona a cor
                valoresPorMotivo = (from o in valoresPorMotivo
                                    select new Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadosPorMotivo
                                    {
                                        Motivo = o.Motivo + " (" + o.Valor.ToString("n2") + ")",
                                        Valor = o.Valor,
                                        Color = CorGrafico(ref coresMotivo)
                                    }).ToList();

                // Adiciona Totalizador
                if (valoresPorMotivo.Count > 0)
                {
                    var total = (from o in valoresPorMotivo select o.Valor).Sum();
                    valoresPorMotivo.Add(new Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadosPorMotivo()
                    {
                        Motivo = "Total (" + total.ToString("n2") + ")",
                        Valor = total,
                        Color = CorGrafico(ref coresMotivo)
                    });

                }

                var _relacaoPorOcorrencia = (from o in relacaoPorOcorrencia
                                             where o.Total > 0
                                             select new
                                             {
                                                 Motivo = o.Motivo + " (" + o.Total.ToString("n2") + ")",
                                                 o.Total,
                                                 Color = CorGrafico(ref coresRelacaoOcorrencia)
                                             }).ToList();

                if (_relacaoPorOcorrencia.Count > 0)
                {
                    var total = (from o in relacaoPorOcorrencia select o.Total).Sum();
                    _relacaoPorOcorrencia.Add(new
                    {
                        Motivo = "Total (" + total.ToString("n2") + ")",
                        Total = total,
                        Color = CorGrafico(ref coresRelacaoOcorrencia)
                    });
                }

                return new JsonpResult(new
                {
                    TempoRetroativo = tempoRetroativo,


                    TotalAbertos = repChamado.TotalChamadosAberto(),
                    TotalEncerrados = repChamado.TotalEncerradosNoPeriodo(dataInicio, dataFim),


                    UltimosChamados = (from o in ultimosChamados
                                       select new
                                       {
                                           o.Codigo,
                                           o.Numero,
                                           Empresa = o.Empresa.Descricao,
                                           Carga = o.Carga.CodigoCargaEmbarcador,
                                           Cliente = o.Cliente?.Descricao ?? string.Empty,
                                           //NotasFiscais = string.Join(", ", notas),
                                           MotivoChamado = o.MotivoChamado.Descricao,
                                           Responsavel = o.Responsavel?.Nome ?? string.Empty,
                                           TempoAtraso = TempoChamado(o)
                                       }).ToList(),


                    RelacaoPorMotivo = valoresPorMotivo,


                    RelacaoPorUsuario = relacaoPorUsuarios,


                    RelacaoPorOcorrencia = _relacaoPorOcorrencia
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao obter os valores.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string CorGrafico(ref List<string> cores)
        {
            string cor = "";
            if (cores.Count > 0)
            {
                cor = cores[0];
                cores.Remove(cor);
            }

            return cor;
        }

        private string ObterPlacas(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            if (veiculo != null)
            {
                List<string> placas = new List<string>() { veiculo.Placa };
                placas.AddRange(veiculosVinculados.Select(o => o.Placa));

                return string.Join(", ", placas);
            }

            return "";
        }

        private string TempoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            DateTime refTime = DateTime.Now;

            TimeSpan tempoTotal = (refTime - chamado.DataCriacao);

            int dias = tempoTotal.Days;
            string tempo = "";

            if (dias > 0)
                tempo += dias + " dia" + (dias > 1 ? "s" : "") + " ";

            tempo += tempoTotal.ToString(@"hh\:mm");

            return tempo.Trim();
        }

        #endregion
    }
}
