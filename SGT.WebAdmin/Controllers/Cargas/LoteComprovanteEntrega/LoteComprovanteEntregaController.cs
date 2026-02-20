
using System;
using System.Linq;
using SGTAdmin.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Cargas
{
    [CustomAuthorize("Cargas/LoteComprovanteEntrega")]
    public class LoteComprovanteEntregaController : BaseController
    {
		#region Construtores

		public LoteComprovanteEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesCargaEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                if (codigo <= 0)
                {
                    return new JsonpResult(false, "Ocorreu uma falha ao obter detalhes de um canhoto.");
                }

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigo, false);

                if (cargaEntrega == null)
                {
                    return new JsonpResult(false, "Ocorreu uma falha ao obter detalhes de uma nota fiscal.");
                }

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaCarga = repCanhoto.BuscarPorCarga(cargaEntrega.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                var canhotos = obterCanhotosCargaEntrega(codigo, canhotosDaCarga, cargaEntregaNotasFiscais);

                var dadosRecebedor = obterDadosRecebedorCargaEntrega(cargaEntrega);

                // Formata retorno
                var retorno = new
                {
                    Codigo = cargaEntrega.Codigo,
                    Destinatario = cargaEntrega.Cliente.Nome,
                    DadosRecebedor = new
                    {
                        Nome = dadosRecebedor?.Nome ?? "",
                        CPF = dadosRecebedor?.CPF ?? "",
                        DataEntrega = dadosRecebedor?.DataEntrega.ToString("dd/MM/yyyy") ?? ""
                    },
                    GeoLocalizacao = obterGeolocalizacaoCargaEntrega(cargaEntrega),
                    Canhotos = from obj in canhotos
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Tipo = obj.TipoCanhoto,
                                   Imagem = obj.NomeArquivo,
                               }
                };

                // Retorna informacoes
                return new JsonpResult(retorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter detalhes de uma nota fiscal.");
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
                Repositorio.Embarcador.Cargas.LoteComprovanteEntrega repLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.LoteComprovanteEntrega(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega loteComprovanteEntrega = repLoteComprovanteEntrega.BuscarPorCodigo(codigo, false);

                // Valida
                if (loteComprovanteEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    loteComprovanteEntrega.Codigo,
                    Carga = loteComprovanteEntrega.Carga.CodigoCargaEmbarcador,
                    ListaCargaEntrega = ObterListaCargaEntrega(loteComprovanteEntrega.Codigo, unitOfWork),
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

        private List<dynamic> ObterListaCargaEntrega(int codigoLoteComprovanteEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            var repCargaEntregaLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

            var listaCargaEntrega = repCargaEntregaLoteComprovanteEntrega.BuscarPorLoteComprovanteEntrega(codigoLoteComprovanteEntrega);

            List<dynamic> retorno = new List<dynamic>();


            foreach (var cargaEntrega in listaCargaEntrega)
            {
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaCarga = repCanhoto.BuscarPorCarga(cargaEntrega.CargaEntrega.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCarga(cargaEntrega.CargaEntrega.Carga.Codigo);
                var canhotos = obterCanhotosCargaEntrega(cargaEntrega.CargaEntrega.Codigo, canhotosDaCarga, cargaEntregaNotasFiscais);

                retorno.Add(new
                {
                    Codigo = cargaEntrega.CargaEntrega.Codigo,
                    Destinatario = cargaEntrega.CargaEntrega.Cliente.Nome,
                    DadosRecebedor = new
                    {
                        Nome = cargaEntrega.DadosRecebedor?.Nome ?? "",
                        CPF = cargaEntrega.DadosRecebedor?.CPF ?? "",
                        DataEntrega = cargaEntrega.DadosRecebedor?.DataEntrega.ToString("dd/MM/yyyy") ?? ""
                    },
                    GeoLocalizacao = new
                    {
                        Latitude = cargaEntrega.Latitude,
                        Longitude = cargaEntrega.Longitude
                    },
                    Canhotos = from obj in canhotos
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Tipo = obj.TipoCanhoto,
                                   Imagem = obj.NomeArquivo,
                               }
                });
            }

            return retorno;
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Cargas.LoteComprovanteEntrega repLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.LoteComprovanteEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao repLoteComprovanteEntregaIntegracao = new Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega loteComprovanteEntrega = new Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega();
                Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao loteComprovanteEntregaIntegracao = new Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao();

                loteComprovanteEntrega.DataCriacao = DateTime.Now;
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega> listaCargaEntregaComprovanteEntrega = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega>();

                // Preenche entidade com dados
                PreencheEntidade(ref loteComprovanteEntrega, ref listaCargaEntregaComprovanteEntrega, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(loteComprovanteEntrega, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repLoteComprovanteEntrega.Inserir(loteComprovanteEntrega, Auditado);

                PreencheEntidadeIntegracao(ref loteComprovanteEntrega, ref listaCargaEntregaComprovanteEntrega, unitOfWork);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    Codigo = loteComprovanteEntrega.Codigo,
                    Carga = loteComprovanteEntrega.Carga.Codigo
                };

                // Retorna sucesso
                return new JsonpResult(retorno);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                string message = ex is Dominio.Excecoes.Embarcador.ServicoException ? ex.Message : "Ocorreu uma falha ao adicionar dados.";
                return new JsonpResult(false, message);
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
                Repositorio.Embarcador.Cargas.LoteComprovanteEntrega repLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.LoteComprovanteEntrega(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega loteComprovanteEntrega = repLoteComprovanteEntrega.BuscarPorCodigo(codigo, false);

                // Valida
                if (loteComprovanteEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega> listaCargaEntregaComprovanteEntrega = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega>();
                // Preenche entidade com dados
                PreencheEntidade(ref loteComprovanteEntrega, ref listaCargaEntregaComprovanteEntrega, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(loteComprovanteEntrega, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repLoteComprovanteEntrega.Atualizar(loteComprovanteEntrega, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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
                Repositorio.Embarcador.Cargas.LoteComprovanteEntrega repLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.LoteComprovanteEntrega(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega loteComprovanteEntrega = repLoteComprovanteEntrega.BuscarPorCodigo(codigo, false);

                // Valida
                if (loteComprovanteEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repLoteComprovanteEntrega.Deletar(loteComprovanteEntrega, Auditado);
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

        public async Task<IActionResult> ObterImagemCanhoto()
        {
            int? codigoCanhoto = Request.GetNullableIntParam("CodigoCanhoto");

            dynamic miniatura = null;

            if (miniatura == null && codigoCanhoto.HasValue)
            {
                miniatura = obterImagemOriginalDoCanhoto(codigoCanhoto.Value);
            }

            var retorno = new
            {
                Miniatura = miniatura,
            };

            return new JsonpResult(retorno);
        }

        #endregion

        #region Métodos Privados
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega loteComprovanteEntrega, ref List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega> listaCargaControleEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
            */

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega repCargaEntregaComprovanteEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CanhotoLoteComprovanteEntrega repCanhotoLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CanhotoLoteComprovanteEntrega(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor repDadosRecebedor = new Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

            dynamic listaCargaEntrega = Newtonsoft.Json.JsonConvert.DeserializeObject(Request.Params("ListaCargaEntrega"));

            int codigoCarga = Request.GetIntParam("Carga");
            var carga = repCarga.BuscarPorCodigo(codigoCarga);

            if (carga == null)
            {
                throw new Exception("A carga selecionada não existe");
            }

            loteComprovanteEntrega.Carga = carga;

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaCarga = repCanhoto.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCarga(carga.Codigo);

            // Deleta a ligação com os Canhotos e com as CargaEntrega para permitir edição
            repCanhotoLoteComprovanteEntrega.DeletarPorLoteComprovanteEntrega(loteComprovanteEntrega.Codigo);
            repCargaEntregaComprovanteEntrega.DeletarPorLoteComprovanteEntrega(loteComprovanteEntrega.Codigo);

            foreach (var dadosCargaEntrega in listaCargaEntrega)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo((int)dadosCargaEntrega.Codigo);

                if (cargaEntrega == null)
                {
                    throw new Exception("Uma das paradas escolhidas não existem");
                }

                // Dados padrão que serão usados caso não tenha sido alterado no front
                var dadosRecebedorPadrao = obterDadosRecebedorCargaEntrega(cargaEntrega);
                var geoLocalizacaoPadrao = obterGeolocalizacaoCargaEntrega(cargaEntrega);

                // Montar os DadosRecebedor
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor dadosRecebedor = null;
                if (dadosCargaEntrega.DadosRecebedor != null)
                {
                    dadosRecebedor = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor();
                    dadosRecebedor.Nome = dadosCargaEntrega.DadosRecebedor.Nome;
                    dadosRecebedor.CPF = dadosCargaEntrega.DadosRecebedor?.CPF ?? "";
                    dadosRecebedor.CPF = dadosRecebedor.CPF.Replace(".", "").Replace("-", "");
                    dadosRecebedor.DataEntrega = DateTime.ParseExact((string)dadosCargaEntrega.DadosRecebedor.DataEntrega, "dd/MM/yyyy", null);
                    repDadosRecebedor.Inserir(dadosRecebedor);
                }

                // Montar a CargaEntregaLoteComprovanteEntrega
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega cargaEntregaLoteComprovanteEntrega = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega();
                cargaEntregaLoteComprovanteEntrega.CargaEntrega = cargaEntrega;
                cargaEntregaLoteComprovanteEntrega.LoteComprovanteEntrega = loteComprovanteEntrega;
                cargaEntregaLoteComprovanteEntrega.Latitude = dadosCargaEntrega?.GeoLocalizacao?.Latitude ?? geoLocalizacaoPadrao?.Latitude ?? "0";
                cargaEntregaLoteComprovanteEntrega.Longitude = dadosCargaEntrega?.GeoLocalizacao?.Longitude ?? geoLocalizacaoPadrao?.Longitude ?? "0";
                cargaEntregaLoteComprovanteEntrega.DadosRecebedor = dadosRecebedor ?? dadosRecebedorPadrao;

                listaCargaControleEntrega.Add(cargaEntregaLoteComprovanteEntrega);
                try
                {
                    repCargaEntregaComprovanteEntrega.Inserir(cargaEntregaLoteComprovanteEntrega);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null && object.ReferenceEquals(e.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                        throw new ControllerException($"A parada de código {cargaEntrega.Codigo} já está em outro lote");

                    throw;
                }

                var canhotosCargaEntrega = obterCanhotosCargaEntrega(cargaEntrega.Codigo, canhotosDaCarga, cargaEntregaNotasFiscais);

                foreach (var canhoto in canhotosCargaEntrega)
                {
                    var canhotoComprovante = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CanhotoLoteComprovanteEntrega();
                    canhotoComprovante.Canhoto = canhoto;
                    canhotoComprovante.CargaEntregaLoteComprovanteEntrega = cargaEntregaLoteComprovanteEntrega;
                    canhotoComprovante.LoteComprovanteEntrega = loteComprovanteEntrega;

                    repCanhotoLoteComprovanteEntrega.Inserir(canhotoComprovante);
                }

            }

        }

        private List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> obterCanhotosCargaEntrega(int codigoCargaEntrega, List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaCarga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais)
        {
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosEntrega = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            //List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscaisParada = (from obj in cargaEntregaNotasFiscais where obj.CargaEntrega.Codigo == codigoCargaEntrega select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscais)
            {

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = (from obj in canhotosDaCarga
                                                                         where
                                                 (obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe && obj.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)
                                                 || (obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso
                                                 && obj.CanhotoAvulso != null && obj.CanhotoAvulso.PedidosXMLNotasFiscais.Any(nf => nf.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.Codigo)
                                                 )
                                                                         select obj).FirstOrDefault();

                if (canhoto != null && !canhotosEntrega.Contains(canhoto))
                    canhotosEntrega.Add(canhoto);
            }

            return canhotosEntrega;
        }

        private void PreencheEntidadeIntegracao(ref Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega loteComprovanteEntrega, ref List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega> listaCargaControleEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade da Integracao do comprovante de entrega
             * Recebe uma instancia da entidade
             * Atribui a entidade
            */

            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto svcCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.XMLNotaFiscalComprovanteEntrega repCargaEntregaComprovanteEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.XMLNotaFiscalComprovanteEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao repLoteComprovanteIntegracao = new Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracaoDefaultSefaz;
            TipoIntegracaoDefaultSefaz = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Sefaz);

            if (TipoIntegracaoDefaultSefaz == null)
                TipoIntegracaoDefaultSefaz = CriarTipoIntegracaoSefazPadrao(unitOfWork);

            foreach (var dadosComprovanteEntrega in listaCargaControleEntrega)
            {

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaCarga = repCanhoto.BuscarPorCarga(loteComprovanteEntrega.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCarga(loteComprovanteEntrega.Carga.Codigo);
                var canhotos = obterCanhotosCargaEntrega(dadosComprovanteEntrega.CargaEntrega.Codigo, canhotosDaCarga, cargaEntregaNotasFiscais);

                //string ImagemBase64 = "";
                if (canhotos == null || canhotos.Count == 0)
                    throw new Dominio.Excecoes.Embarcador.ControllerException("As notas da parada número " + dadosComprovanteEntrega.CargaEntrega.Codigo + " não possuem nenhuma imagem de canhoto vinculada");
                //{
                //    ImagemBase64 = svcCanhoto.ObterMiniatura(canhotos[0]);
                //}

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> listaEntregaNotas = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(dadosComprovanteEntrega.CargaEntrega.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> listaCtesEntrega = listaEntregaNotas.Select(x => x.PedidoXMLNotaFiscal.CTes).SelectMany(x => x).Distinct().ToList();

                if (listaCtesEntrega == null || listaCtesEntrega.Count <= 0)
                    throw new Dominio.Excecoes.Embarcador.ControllerException("A parada número " + dadosComprovanteEntrega.CargaEntrega.Codigo + " não possui nenhum CT-e vinculado");

                foreach (var cte in listaCtesEntrega)
                {

                    Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega notaFiscalComprovanteEntrega = new Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega();
                    notaFiscalComprovanteEntrega.PedidoXMLNotaFiscal = cte.PedidoXMLNotaFiscal;
                    notaFiscalComprovanteEntrega.Cte = cte;
                    notaFiscalComprovanteEntrega.CargaEntrega = dadosComprovanteEntrega.CargaEntrega;
                    notaFiscalComprovanteEntrega.LoteComprovanteEntrega = loteComprovanteEntrega;
                    notaFiscalComprovanteEntrega.NomeArquivoImagem = svcCanhoto.ObterDiretorioArquivo(canhotos[0], unitOfWork);
                    notaFiscalComprovanteEntrega.GuidNomeArquivoImagem = canhotos[0].GuidNomeArquivo;
                    notaFiscalComprovanteEntrega.Latitude = dadosComprovanteEntrega.Latitude;
                    notaFiscalComprovanteEntrega.Longitude = dadosComprovanteEntrega.Longitude;
                    notaFiscalComprovanteEntrega.DadosRecebedor = dadosComprovanteEntrega.DadosRecebedor;

                    repCargaEntregaComprovanteEntrega.Inserir(notaFiscalComprovanteEntrega);


                    Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao LoteComprovanteEntregaIntegracao = new Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao();

                    LoteComprovanteEntregaIntegracao.Carga = loteComprovanteEntrega.Carga;
                    LoteComprovanteEntregaIntegracao.XMLNotaFiscalComprovanteEntrega = notaFiscalComprovanteEntrega;
                    LoteComprovanteEntregaIntegracao.NumeroTentativas = 0;
                    LoteComprovanteEntregaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    LoteComprovanteEntregaIntegracao.ProblemaIntegracao = "";
                    LoteComprovanteEntregaIntegracao.Protocolo = "";
                    LoteComprovanteEntregaIntegracao.PreProtocolo = "";
                    LoteComprovanteEntregaIntegracao.IntegracaoColeta = false;
                    LoteComprovanteEntregaIntegracao.TipoIntegracao = TipoIntegracaoDefaultSefaz;
                    LoteComprovanteEntregaIntegracao.DataIntegracao = DateTime.Now;

                    repLoteComprovanteIntegracao.Inserir(LoteComprovanteEntregaIntegracao);
                }

            }
        }

        private Dominio.Entidades.Embarcador.Cargas.TipoIntegracao CriarTipoIntegracaoSefazPadrao(Repositorio.UnitOfWork unit)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unit);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracaoDefaultSefaz = new Dominio.Entidades.Embarcador.Cargas.TipoIntegracao();
            TipoIntegracaoDefaultSefaz.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Sefaz;
            TipoIntegracaoDefaultSefaz.Descricao = "Integração Sefaz";
            TipoIntegracaoDefaultSefaz.TipoEnvio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao.Individual;
            TipoIntegracaoDefaultSefaz.IntegrarCargaTransbordo = false;
            TipoIntegracaoDefaultSefaz.Grupo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoTipoIntegracao.GerenciadoraDeRisco;
            TipoIntegracaoDefaultSefaz.IntegracaoTransportador = false;
            TipoIntegracaoDefaultSefaz.QuantidadeMaximaEnvioLote = 10;
            TipoIntegracaoDefaultSefaz.Ativo = true;

            repTipoIntegracao.Inserir(TipoIntegracaoDefaultSefaz);
            return TipoIntegracaoDefaultSefaz;

        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega loteComprovanteEntrega, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Cargas.LoteComprovanteEntrega repLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.LoteComprovanteEntrega(unitOfWork);

            // Dados do filtro

            int codigo = Request.GetIntParam("Codigo");
            int codigoCarga = Request.GetIntParam("Carga");

            // Consulta
            List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega> listaGrid = repLoteComprovanteEntrega.Consultar(codigo, codigoCarga, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repLoteComprovanteEntrega.ContarConsulta(codigo, codigoCarga);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Carga = obj.Carga.CodigoCargaEmbarcador
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "Codigo") propOrdenar = "Codigo";
            else if (propOrdenar == "Carga") propOrdenar = "Carga";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Lote", "Codigo", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.left, true);

            return grid;
        }

        private string obterImagemCanhoto(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal)
        {
            return notaFiscal.Canhoto?.NomeArquivo;
        }

        private Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor obterDadosRecebedorCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            // Pegar aqui os dados que "serão os dados informados no controle de entrega" (issue #21309)

            return cargaEntrega.DadosRecebedor;
        }

        private dynamic obterGeolocalizacaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {

            string latitudeDescarga = cargaEntrega.Cliente?.Latitude?.ToString() ?? "";
            string longitudeDescarga = cargaEntrega.Cliente?.Longitude?.ToString() ?? "";
            bool cargaEntregaTemLatitude = latitudeDescarga != null && latitudeDescarga != "";
            bool cargaEntregaTemLongitude = longitudeDescarga != null && longitudeDescarga != "";
            if (cargaEntregaTemLatitude && cargaEntregaTemLongitude)
            {
                return new
                {
                    Latitude = latitudeDescarga,
                    Longitude = longitudeDescarga
                };
            }

            return new
            {
                Latitude = "",
                Longitude = ""
            };
        }

        private void excluirArquivoNotaFiscal(Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega notaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            if (notaFiscal.NomeArquivoImagem == "")
            {
                return;
            }

            string extensao = System.IO.Path.GetExtension(notaFiscal.NomeArquivoImagem).ToLower();

            string caminho = Servicos.Embarcador.Carga.ControleEntrega.LoteComprovanteEntrega.ObterCaminhoImagemNotaFiscal(notaFiscal, unitOfWork);
            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, notaFiscal.GuidNomeArquivoImagem + extensao);
            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
            {
                Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
            }
        }

        private dynamic obterImagemOriginalDoCanhoto(int codigoCanhoto)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Canhotos.Canhoto repCargaEntrega = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto srvCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaEntrega.BuscarPorCodigo(codigoCanhoto);

            if (canhoto == null)
            {
                return null;
            }

            return !canhoto.IsPDF() ? srvCanhoto.ObterMiniatura(canhoto, unitOfWork) : null;
        }

        #endregion
    }
}
