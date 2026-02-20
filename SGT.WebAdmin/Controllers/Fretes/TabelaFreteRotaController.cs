using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/TabelaFreteRota")]
    public class TabelaFreteRotaController : BaseController
    {
		#region Construtores

		public TabelaFreteRotaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int? codigoOrigem = null;
                int? codigoDestino = null;
                if (!String.IsNullOrEmpty(Request.Params("Origem")))
                {
                    codigoOrigem = int.Parse(Request.Params("Origem"));
                    if (codigoOrigem.Value == 0)
                        codigoOrigem = null;
                }

                if (!String.IsNullOrEmpty(Request.Params("Destino")))
                {
                    codigoDestino = int.Parse(Request.Params("Destino"));
                    if (codigoDestino.Value == 0)
                        codigoDestino = null;
                }

                string codigoEmbarcador = Request.Params("CodigoEmbarcador");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                int codigoTabelaFrete = int.Parse(Request.Params("TabelaFrete"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Codigo", "CodigoEmbarcador", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Origem", "Origem", 31, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 36, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Origem" || propOrdenar == "Destino")
                {
                    propOrdenar += ".Descricao";
                }


                Repositorio.Embarcador.Frete.TabelaFreteRota repTabelaFreteRota = new Repositorio.Embarcador.Frete.TabelaFreteRota(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota> listaTabelaFreteRota = repTabelaFreteRota.Consultar(codigoTabelaFrete, ativo, codigoOrigem, codigoDestino, null, codigoEmbarcador, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTabelaFreteRota.ContarConsulta(codigoTabelaFrete, ativo, codigoOrigem, codigoDestino, null, codigoEmbarcador));

                dynamic lista = (from p in listaTabelaFreteRota
                                 select new
                                 {
                                     p.Codigo,
                                     CodigoEmbarcador = !string.IsNullOrWhiteSpace(p.CodigoEmbarcador) ? p.CodigoEmbarcador : "",
                                     p.DescricaoAtivo,
                                     Origem = p.Origem.DescricaoCidadeEstado,
                                     Destino = p.Destino.DescricaoCidadeEstado + (!string.IsNullOrEmpty(p.DescricaoDestinos) ? " (" + p.DescricaoDestinos + ")" : "")
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


        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                string codigoEmbarcador = Request.Params("CodigoEmbarcador");
                Dominio.Entidades.Embarcador.Frete.TabelaFreteRota tabelaFreteRota = new Dominio.Entidades.Embarcador.Frete.TabelaFreteRota();
                Repositorio.Embarcador.Frete.TabelaFreteRota repTabelaFreteRota = new Repositorio.Embarcador.Frete.TabelaFreteRota(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.TabelaFreteRota tabelaExiste = repTabelaFreteRota.BuscarPorCodigoEmbarcador(codigoEmbarcador);
                if (tabelaExiste == null)
                {
                    tabelaFreteRota.Ativo = bool.Parse(Request.Params("Ativo"));
                    tabelaFreteRota.TabelaFrete = new Dominio.Entidades.Embarcador.Frete.TabelaFrete() { Codigo = int.Parse(Request.Params("TabelaFrete")) };
                    tabelaFreteRota.Origem = new Dominio.Entidades.Localidade() { Codigo = int.Parse(Request.Params("Origem")) };
                    tabelaFreteRota.Destino = new Dominio.Entidades.Localidade() { Codigo = int.Parse(Request.Params("Destino")) };
                    tabelaFreteRota.DescricaoDestinos = Request.Params("DescricaoDestinos");
                    tabelaFreteRota.CodigoEmbarcador = codigoEmbarcador;

                    repTabelaFreteRota.Inserir(tabelaFreteRota);

                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                    Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCarga repTabelaFreteRotaTipoCarga = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCarga(unitOfWork);
                    Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga repTabelaFreteRotaTipoCargaModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga(unitOfWork);

                    dynamic listaTiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("TiposCarga"));
                    foreach (var tipoCarga in listaTiposCarga)
                    {
                        Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga tabelaFreteTipoCarga = new Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga();
                        tabelaFreteTipoCarga.TabelaFreteRota = tabelaFreteRota;
                        tabelaFreteTipoCarga.TipoDeCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = (int)tipoCarga.Codigo };
                        tabelaFreteTipoCarga.Ativo = (bool)tipoCarga.Ativo;
                        repTabelaFreteRotaTipoCarga.Inserir(tabelaFreteTipoCarga);
                        foreach (var modeloVeicularCarga in tipoCarga.ModeloVeicularCarga)
                        {
                            Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga tabelaFreteRotaTipoCargaModeloVeicularCarga = new Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga();
                            tabelaFreteRotaTipoCargaModeloVeicularCarga.ModeloVeicularCarga = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = (int)modeloVeicularCarga.Codigo };
                            tabelaFreteRotaTipoCargaModeloVeicularCarga.TabelaFreteRotaTipoCarga = tabelaFreteTipoCarga;
                            tabelaFreteRotaTipoCargaModeloVeicularCarga.ValorFrete = (decimal)modeloVeicularCarga.ValorFrete;
                            tabelaFreteRotaTipoCargaModeloVeicularCarga.ValorPedagio = (decimal)modeloVeicularCarga.ValorPedagio;
                            repTabelaFreteRotaTipoCargaModeloVeicularCarga.Inserir(tabelaFreteRotaTipoCargaModeloVeicularCarga);
                        }
                    }

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe uma tabela de frete para esse código (" + codigoEmbarcador + ").");
                }

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
                string codigoEmbarcador = Request.Params("CodigoEmbarcador");
                Repositorio.Embarcador.Frete.TabelaFreteRota repTabelaFreteRota = new Repositorio.Embarcador.Frete.TabelaFreteRota(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteRota tabelaFreteRota = repTabelaFreteRota.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));

                Dominio.Entidades.Embarcador.Frete.TabelaFreteRota tabelaExiste = repTabelaFreteRota.BuscarPorCodigoEmbarcador(codigoEmbarcador);
                if (tabelaExiste == null || tabelaExiste.Codigo == tabelaFreteRota.Codigo)
                {

                    tabelaFreteRota.Ativo = bool.Parse(Request.Params("Ativo"));
                    tabelaFreteRota.TabelaFrete = new Dominio.Entidades.Embarcador.Frete.TabelaFrete() { Codigo = int.Parse(Request.Params("TabelaFrete")) };
                    tabelaFreteRota.Origem = new Dominio.Entidades.Localidade() { Codigo = int.Parse(Request.Params("Origem")) };
                    tabelaFreteRota.Destino = new Dominio.Entidades.Localidade() { Codigo = int.Parse(Request.Params("Destino")) };
                    tabelaFreteRota.DescricaoDestinos = Request.Params("DescricaoDestinos");
                    tabelaFreteRota.CodigoEmbarcador = codigoEmbarcador;
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                    Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCarga repTabelaFreteRotaTipoCarga = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCarga(unitOfWork);
                    Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga repTabelaFreteRotaTipoCargaModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga(unitOfWork);

                    dynamic listaTiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("TiposCarga"));
                    foreach (var tipoCarga in listaTiposCarga)
                    {
                        Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga tabelaFreteTipoCarga = repTabelaFreteRotaTipoCarga.BuscarPorTabelaTipoCarga(tabelaFreteRota.Codigo, (int)tipoCarga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos);
                        if (tabelaFreteTipoCarga != null)
                        {
                            tabelaFreteTipoCarga.Ativo = (bool)tipoCarga.Ativo;
                            repTabelaFreteRotaTipoCarga.Atualizar(tabelaFreteTipoCarga);
                        }
                        else
                        {
                            tabelaFreteTipoCarga = new Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga();
                            tabelaFreteTipoCarga.TabelaFreteRota = tabelaFreteRota;
                            tabelaFreteTipoCarga.TipoDeCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = (int)tipoCarga.Codigo };
                            tabelaFreteTipoCarga.Ativo = (bool)tipoCarga.Ativo;
                            repTabelaFreteRotaTipoCarga.Inserir(tabelaFreteTipoCarga);

                        }
                        List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga> listaModelosVeicularesFretesAtivos = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga>();
                        foreach (var modeloVeicularCarga in tipoCarga.ModeloVeicularCarga)
                        {

                            Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga tabelaFreteRotaTipoCargaModeloVeicularCarga = repTabelaFreteRotaTipoCargaModeloVeicularCarga.BuscarPorTipoCargaModeloVeicular(tabelaFreteTipoCarga.Codigo, (int)modeloVeicularCarga.Codigo);
                            if (tabelaFreteRotaTipoCargaModeloVeicularCarga != null)
                            {
                                tabelaFreteRotaTipoCargaModeloVeicularCarga.ModeloVeicularCarga = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = (int)modeloVeicularCarga.Codigo };
                                tabelaFreteRotaTipoCargaModeloVeicularCarga.ValorFrete = (decimal)modeloVeicularCarga.ValorFrete;
                                tabelaFreteRotaTipoCargaModeloVeicularCarga.ValorPedagio = (decimal)modeloVeicularCarga.ValorPedagio;
                                repTabelaFreteRotaTipoCargaModeloVeicularCarga.Atualizar(tabelaFreteRotaTipoCargaModeloVeicularCarga);
                            }
                            else
                            {
                                tabelaFreteRotaTipoCargaModeloVeicularCarga = new Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga();
                                tabelaFreteRotaTipoCargaModeloVeicularCarga.ModeloVeicularCarga = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = (int)modeloVeicularCarga.Codigo };
                                tabelaFreteRotaTipoCargaModeloVeicularCarga.TabelaFreteRotaTipoCarga = tabelaFreteTipoCarga;
                                tabelaFreteRotaTipoCargaModeloVeicularCarga.ValorFrete = (decimal)modeloVeicularCarga.ValorFrete;
                                tabelaFreteRotaTipoCargaModeloVeicularCarga.ValorPedagio = (decimal)modeloVeicularCarga.ValorPedagio;
                                repTabelaFreteRotaTipoCargaModeloVeicularCarga.Inserir(tabelaFreteRotaTipoCargaModeloVeicularCarga);
                            }
                            listaModelosVeicularesFretesAtivos.Add(tabelaFreteRotaTipoCargaModeloVeicularCarga);
                        }

                        List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga> listaModelosVeicularesFretesSalvosNoBanco = repTabelaFreteRotaTipoCargaModeloVeicularCarga.Consultar(tabelaFreteTipoCarga.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga modeloCargaSalvoNoBanco in listaModelosVeicularesFretesSalvosNoBanco)
                        {
                            if (!listaModelosVeicularesFretesAtivos.Exists(obj => obj.Codigo == modeloCargaSalvoNoBanco.Codigo))
                            {
                                repTabelaFreteRotaTipoCargaModeloVeicularCarga.Deletar(modeloCargaSalvoNoBanco);
                            }
                        }
                    }
                    repTabelaFreteRota.Atualizar(tabelaFreteRota);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe uma tabela de frete para esse código (" + codigoEmbarcador + ").");
                }

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
                int codigo;
                int.TryParse(Request.Params("codigo"), out codigo);

                Repositorio.Embarcador.Frete.TabelaFreteRota repTabelaFreteRota = new Repositorio.Embarcador.Frete.TabelaFreteRota(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCarga repTabelaFreteRotaTipoCarga = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCarga(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga repTabelaFreteRotaTipoCargaModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.TabelaFreteRota tabelaFreteRota = repTabelaFreteRota.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga> listaTabelaFreteTipoCarga = repTabelaFreteRotaTipoCarga.Consultar(tabelaFreteRota.Codigo);

                List<dynamic> dynListaTabelaTipoCarga = new List<dynamic>();
                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga tabelaTipoCarga in listaTabelaFreteTipoCarga)
                {
                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga> listaTabelaFreteRotaTipoCargaModeloVeicularCarga = repTabelaFreteRotaTipoCargaModeloVeicularCarga.Consultar(tabelaTipoCarga.Codigo);

                    var dynTabelaFreteTipoCarga = new
                    {
                        Codigo = tabelaTipoCarga.TipoDeCarga.Codigo,
                        Descricao = tabelaTipoCarga.TipoDeCarga.Descricao,
                        Ativo = tabelaTipoCarga.Ativo,
                        ModeloVeicularCarga = (from p in listaTabelaFreteRotaTipoCargaModeloVeicularCarga
                                               select new
                                               {
                                                   p.ModeloVeicularCarga.Codigo,
                                                   p.ModeloVeicularCarga.Descricao,
                                                   ValorFrete = new { val = p.ValorFrete, tipo = "decimal" },
                                                   ValorPedagio = new { val = p.ValorPedagio, tipo = "decimal" }
                                               }).ToList()
                    };
                    dynListaTabelaTipoCarga.Add(dynTabelaFreteTipoCarga);
                }

                var retorno = new
                {
                    tabelaFreteRota.Codigo,
                    tabelaFreteRota.Ativo,
                    tabelaFreteRota.DescricaoDestinos,
                    tabelaFreteRota.CodigoEmbarcador,
                    Destino = new { tabelaFreteRota.Destino.Codigo, Descricao = tabelaFreteRota.Destino.DescricaoCidadeEstado },
                    Origem = new { tabelaFreteRota.Origem.Codigo, Descricao = tabelaFreteRota.Origem.DescricaoCidadeEstado },
                    TabelaFrete = new { tabelaFreteRota.TabelaFrete.Codigo, tabelaFreteRota.TabelaFrete.Descricao },
                    TiposCarga = dynListaTabelaTipoCarga
                };

                return new JsonpResult(retorno);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Frete.TabelaFreteRota repTabelaFreteRota = new Repositorio.Embarcador.Frete.TabelaFreteRota(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteRota tabelaFreteRota = repTabelaFreteRota.BuscarPorCodigo(codigo);
                repTabelaFreteRota.Deletar(tabelaFreteRota);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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
    }
}
