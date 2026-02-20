using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Roteirizador
{
    [CustomAuthorize(new string[] { "BuscarDadosRoteirizacao" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class RoteirizadorController : BaseController
    {
		#region Construtores

		public RoteirizadorController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> BlocosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(Request.GetIntParam("Carga"));

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("S. Carregamento", "OrdemCarregamento", 10, Models.Grid.Align.center, true).Ord(false);
                grid.AdicionarCabecalho("S. Entrega", "OrdemEntrega", 10, Models.Grid.Align.center, false).Ord(false);
                grid.AdicionarCabecalho("Cliente", "Cliente", 25, Models.Grid.Align.left, true).Ord(false);
                grid.AdicionarCabecalho("Pedido", "Pedido", 20, Models.Grid.Align.center, false).Ord(false);
                grid.AdicionarCabecalho("Cubagem", "Cubagem", 20, Models.Grid.Align.right, false).Ord(false);
                grid.AdicionarCabecalho("Peso", "Peso", 15, Models.Grid.Align.right, false).Ord(false);
                grid.AdicionarCabecalho("Peso Bloco", "PesoBloco", 15, Models.Grid.Align.right, false).Ord(false);
                grid.AdicionarCabecalho("Bloco", "Bloco", 15, Models.Grid.Align.center, false).Ord(false).Ord(false);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "OrdemCarregamento") propOrdenar = "OrdemCarrgamento";
                else if (propOrdenar == "Cliente") propOrdenar = "Pedido.Destinatario.Nome";
                else if (propOrdenar == "Pedido") propOrdenar = "Pedido.NumeroPedidoEmbarcador";
                else if (propOrdenar == "Peso") propOrdenar = "Pedido.PesoTotal";
                else if (propOrdenar == "Cubagem") propOrdenar = "Pedido.CubagemTotal";

                // Busca Dados
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocos = repBlocoCarregamento.BuscarPorCarregamento(carga.Carregamento?.Codigo ?? 0);
                int totalRegistros = repBlocoCarregamento.ContarPorCarregamento(carga.Carregamento?.Codigo ?? 0);

                Dictionary<string, decimal> pesoBloco = new Dictionary<string, decimal>();
                List<dynamic> lista = new List<dynamic>();

                foreach (var p in blocos)
                {
                    pesoBloco.TryGetValue(p.Bloco, out decimal peso);
                    peso += p.Pedido.PesoTotal;
                    pesoBloco[p.Bloco] = peso;

                    lista.Add(new
                    {
                        p.Codigo,
                        OrdemCarregamento = p.OrdemCarregamento,
                        p.OrdemEntrega,
                        Cliente = p.Pedido.Destinatario.Descricao,
                        Pedido = p.Pedido.NumeroPedidoEmbarcador,
                        Peso = p.Pedido.PesoTotal.ToString("n2"),
                        Cubagem = p.Pedido.CubagemTotal.ToString("n2"),
                        p.Bloco,
                        PesoBloco = peso.ToString("n2"),
                    });
                }

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

        public async Task<IActionResult> BuscarSimulacaoFrete()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete repSimulacaoFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete(unidadeTrabalho);

                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacaoFrete = repSimulacaoFrete.BuscarPorCarregamento(carga?.Carregamento?.Codigo ?? 0);

                if (simulacaoFrete == null)
                    return new JsonpResult(false, true, "Não foi encontrado nenhuma simulação para a carga.");

                var retorno = new
                {
                    PesoFrete = simulacaoFrete.PesoFrete.ToString("n4"),
                    ValorMercadoria = simulacaoFrete.ValorMercadoria.ToString("n4"),
                    ValorFrete = simulacaoFrete.ValorFrete.ToString("n4"),
                    Distancia = simulacaoFrete.Distancia.ToString(),
                    ValorPorPeso = simulacaoFrete.ValorPorPeso.ToString("n4"),
                    PercentualSobValorMercadoria = simulacaoFrete.PercentualSobValorMercadoria.ToString("n4") + "%"
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar simulação de frete.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarRotaCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeTrabalho.Start();
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaRoteirizacao repCargaRoteirizacao = new Repositorio.Embarcador.Cargas.CargaRoteirizacao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaRoteirizacaoClientesRota repCargaRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.CargaRoteirizacaoClientesRota(unidadeTrabalho);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
                int codigoCarga;
                decimal distancia;
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                decimal.TryParse(Request.Params("Distancia").Replace("km", ""), out distancia);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao)int.Parse(Request.Params("TipoUltimoPontoRoteirizacao"));
                string tipoRota = Request.Params("TipoRota");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao cargaRoteirizacao = repCargaRoteirizacao.BuscarPorCarga(carga.Codigo);
                bool inserir = false;
                if (cargaRoteirizacao == null)
                {
                    inserir = true;
                    cargaRoteirizacao = new Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao();
                }

                cargaRoteirizacao.Carga = carga;
                cargaRoteirizacao.DistanciaKM = distancia;
                cargaRoteirizacao.TipoRota = tipoRota;
                cargaRoteirizacao.TipoUltimoPontoRoteirizacao = tipoUltimoPontoRoteirizacao;


                if (inserir)
                    repCargaRoteirizacao.Inserir(cargaRoteirizacao);
                else
                {
                    repCargaRoteirizacao.Atualizar(cargaRoteirizacao);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota> rotasClientesExiste = repCargaRoteirizacaoClientesRota.BuscarPorCargaRoteirizacao(cargaRoteirizacao.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota rotasClienteExiste in rotasClientesExiste)
                        repCargaRoteirizacaoClientesRota.Deletar(rotasClienteExiste);

                }

                dynamic pessoas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Pessoas"));
                int ordem = 1;
                foreach (var pessoa in pessoas)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota rotasCliente = new Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota();
                    double codigoCliente = double.Parse(Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ.ToString()));
                    rotasCliente.Cliente = repCliente.BuscarPorCPFCNPJ(codigoCliente);
                    rotasCliente.CargaRoteirizacao = cargaRoteirizacao;
                    rotasCliente.Ordem = ordem;
                    ordem++;
                    repCargaRoteirizacaoClientesRota.Inserir(rotasCliente);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Salvou a rota.", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter ao savar rota.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosRoteirizacao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaRoteirizacao repCargaRoteirizacao = new Repositorio.Embarcador.Cargas.CargaRoteirizacao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaRoteirizacaoClientesRota repCargaRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.CargaRoteirizacaoClientesRota(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unidadeTrabalho);
                Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unidadeTrabalho);

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                bool roteirizado = false;
                decimal distanciaKM = 0;
                string tipoRota = null;
                string polilinhaRota = null;
                string pontosDaRota = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? tipoUltimoPontoRoteirizacao = null;
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotasInformacaoPessoa = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa>();

                Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao cargaRoteirizacao = repCargaRoteirizacao.BuscarPorCarga(codigoCarga);
                if (cargaRoteirizacao != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

                    roteirizado = true;
                    distanciaKM = cargaRoteirizacao.DistanciaKM;
                    tipoRota = cargaRoteirizacao.TipoRota;
                    tipoUltimoPontoRoteirizacao = cargaRoteirizacao.TipoUltimoPontoRoteirizacao;
                    polilinhaRota = cargaRotaFrete?.PolilinhaRota ?? "";
                    pontosDaRota = cargaRotaFrete != null ? Servicos.Embarcador.Carga.RotaFrete.ObterPontosPassagemCargaRotaFreteSerializada(cargaRotaFrete, unidadeTrabalho) : "";

                    List<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota> cargaRoteirizacaoClientesRota = repCargaRoteirizacaoClientesRota.BuscarPorCargaRoteirizacao(cargaRoteirizacao.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota clientesRota in cargaRoteirizacaoClientesRota)
                    {
                        if (clientesRota.Cliente != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa rotaInformacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa
                            {
                                pessoa = serPessoa.ConverterObjetoPessoa(clientesRota.Cliente),
                                coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas
                                {
                                    tipoLocalizacao = clientesRota.Cliente.TipoLocalizacao,
                                    latitude = clientesRota.Cliente.Latitude,
                                    longitude = clientesRota.Cliente.Longitude,
                                    RestricoesEntregas = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega>()
                                }
                            };

                            List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega> restricaoEntrega = clientesRota.Cliente?.ClienteDescargas?.FirstOrDefault()?.RestricoesDescarga.ToList();
                            if (restricaoEntrega != null)
                            {
                                rotaInformacao.coordenadas.RestricoesEntregas = (from restricao in restricaoEntrega
                                                                                 select new Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega()
                                                                                 {
                                                                                     Codigo = restricao.Codigo,
                                                                                     Descricao = restricao.Descricao,
                                                                                     Observacao = restricao.Observacao,
                                                                                     PrimeiraEntrega = restricao.PrimeiraEntrega,
                                                                                     CorVisualizacao = restricao.CorVisualizacao
                                                                                 }).ToList();
                            }

                            rotasInformacaoPessoa.Add(rotaInformacao);
                        }
                        else
                        {
                            roteirizado = false;
                        }
                    }
                }

                var retorno = new
                {
                    roteirizado,
                    rotasInformacaoPessoa,
                    DistanciaKM = distanciaKM,
                    TipoRota = tipoRota,
                    TipoUltimoPontoRoteirizacao = tipoUltimoPontoRoteirizacao,
                    PontosDaRota = pontosDaRota,
                    PolilinhaRota = polilinhaRota,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados da Roteirização.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
