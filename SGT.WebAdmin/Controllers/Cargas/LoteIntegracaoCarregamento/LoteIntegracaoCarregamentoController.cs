using System;
using System.Linq;
using SGTAdmin.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Cargas
{
    [CustomAuthorize("Cargas/LoteIntegracaoCarregamento")]
    public class LoteIntegracaoCarregamentoController : BaseController
    {
		#region Construtores

		public LoteIntegracaoCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais


        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaIntegracoes(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaIntegracoes(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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
        public async Task<IActionResult> ExportarPesquisaIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaIntegracoes(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;


                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaIntegracoes(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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
        public async Task<IActionResult> PesquisaCarregamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork, false);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

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
        public async Task<IActionResult> ExportarPesquisaCarregamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

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
        public async Task<IActionResult> CriarRegistroIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento repLoteIntegracaoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = BuscarCarregamentosSelecionados(unitOfWork);

                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.Isis);

                Servicos.Embarcador.Integracao.IntegracaoLoteCarregamento servicoIntegracaoLote = new Servicos.Embarcador.Integracao.IntegracaoLoteCarregamento(unitOfWork);
                var loteIntegracaoCarregamento = servicoIntegracaoLote.AdicionarIntegracaoCarregamento(carregamentos, TipoIntegracao.Isis, SituacaoIntegracao.AgIntegracao, Auditado);

                if (loteIntegracaoCarregamento == null)
                    throw new ControllerException("Ocorreu uma falha ao adicionar dados.");

                // Retorna sucesso
                return new JsonpResult(new
                {
                    Codigo = loteIntegracaoCarregamento.Codigo
                });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaPorIntegracaoLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoIntegracao = 0;
                int.TryParse(Request.Params("Codigo"), out codigoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 11, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "TipoIntegracao")
                    propOrdenar = "TipoIntegracao.Descricao";

                Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento repLoteCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento> ListIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento>();

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento Integracao = repLoteCarregamento.BuscarPorCodigoESituacao(codigoIntegracao, situacao);
                if (Integracao != null)
                    ListIntegracao.Add(Integracao);

                grid.setarQuantidadeTotal(1);
                var dynRetorno = (from obj in ListIntegracao
                                  select new
                                  {
                                      obj.Codigo,
                                      TipoIntegracao = obj.TipoIntegracao?.Descricao ?? "",
                                      SituacaoIntegracao = obj.DescricaoSituacaoIntegracao,
                                      Tentativas = obj.NumeroTentativas.ToString(),
                                      DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                                      DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ? "#ADD8E6" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? "#DFF0D8" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? "#C16565" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? "#F7F7BA" :
                                                    "#FFFFFF",
                                      DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? "#FFFFFF" : "#666666"
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento reploteCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento LoteCarregamentoIntegracao = reploteCarregamentoIntegracao.BuscarPorCodigo(codigo);

                LoteCarregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, LoteCarregamentoIntegracao, null, "Reenviou integração", unidadeTrabalho);

                reploteCarregamentoIntegracao.Atualizar(LoteCarregamentoIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar reenviar o arquivo para integração.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento reploteIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento loteIntegracao = reploteIntegracao.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(loteIntegracao.ArquivosTransacao.Count());

                var retorno = (from obj in loteIntegracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento repLoteIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento integracao = repLoteIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Lote " + integracao.Codigo + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ExecutaPesquisaIntegracoes(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento repLoteCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            // Dados do filtro
            int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Usuario.Empresa.Codigo : 0;

            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");
            string numeroCarregamento = Request.GetStringParam("NumeroCarregamento");
            string carga = Request.GetStringParam("Carga");

            // Consulta
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento> listaGrid = repLoteCarregamentoIntegracao.Consultarlotes(dataInicio, dataFim, situacao, numeroCarregamento, codigoEmpresa, carga, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repLoteCarregamentoIntegracao.ContarConsulta(dataInicio, dataFim, situacao, numeroCarregamento, codigoEmpresa, carga);

            List<int> codigosCarregamentos = listaGrid.SelectMany(obj => obj.Carregamentos).ToList()?.Select(x => x.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = codigosCarregamentos.Count > 0 ? repositorioCarga.BuscarCargasPorCarregamentos(codigosCarregamentos) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            var lista = from obj in listaGrid select FormatarObjetoPesquisaIntegracoes(obj, cargas, unitOfWork);

            return lista.ToList();
        }

        private dynamic FormatarObjetoPesquisaIntegracoes(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento integracao, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo primeiraIntegracao = integracao.ArquivosTransacao.OrderBy(obj => obj.Data).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo ultimaIntegracao = integracao.ArquivosTransacao.OrderByDescending(obj => obj.Data).FirstOrDefault();
            int numeroEnvios = integracao.ArquivosTransacao.Count;

            List<int> carregamentos = integracao.Carregamentos.Select(x => x.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasCarregamento = cargas.Where(x => carregamentos.Contains(x.Carregamento.Codigo)).ToList();

            return new
            {
                integracao.Codigo,
                NumeroCarregamento = string.Join(",", integracao.Carregamentos.Select(x => x.NumeroCarregamento)),
                Carga = string.Join(",", cargasCarregamento.Select(x => x.CodigoCargaEmbarcador)),
                Integradora = integracao.TipoIntegracao.Descricao,
                Data = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                Situacao = integracao.DescricaoSituacaoIntegracao,
                MensagemRetorno = integracao.ProblemaIntegracao,
                NumeroEnvios = numeroEnvios,
                PrimeiroEnvio = primeiraIntegracao?.Data.ToString("dd/MM/yyyy HH:mm"),
                UltimoEnvio = ultimaIntegracao?.Data.ToString("dd/MM/yyyy HH:mm")
            };
        }


        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> ListaCarregamentos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();

            int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Usuario.Empresa.Codigo : 0;
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento()
            {
                DataFim = Request.GetDateTimeParam("DataFim"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                CpfCnpjRecebedor = Request.GetDoubleParam("Emitente"),
                CodigoCargaEmbarcador = Request.GetStringParam("Carga"),
                NumeroCarregamento = Request.GetStringParam("NumeroCarregamento"),
                CodigosEmpresa = codigoEmpresa > 0 ? new List<int>() { codigoEmpresa } : null
            };

            totalRegistros = repositorioCarregamento.ContarConsultaCarregamentoLoteIntegracao(filtroPesquisa);

            if (totalRegistros > 0)
            {
                ListaCarregamentos = repositorioCarregamento.ConsultarCarregamentosLoteintegracao(filtroPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    InicioRegistros = inicio,
                    LimiteRegistros = limite
                });
            }

            List<int> codigosCarregamentos = ListaCarregamentos.Select(obj => obj.Codigo).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = codigosCarregamentos.Count > 0 ? repositorioCarga.BuscarCargasPorCarregamentos(codigosCarregamentos) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            var lista = from obj in ListaCarregamentos select FormatarObjetoPesquisa(obj, (from carga in cargas where carga.Carregamento.Codigo == obj.Codigo select carga).ToList(), unitOfWork);

            return lista.ToList();
        }

        private dynamic FormatarObjetoPesquisa(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas.Where(x => x.Carregamento?.Codigo == carregamento.Codigo).FirstOrDefault();

            return new
            {
                carregamento?.Codigo,
                carregamento?.NumeroCarregamento,
                Carga = (carga?.CargaFechada ?? false) ? carga?.CodigoCargaEmbarcador : string.Empty,
                Protocolo = carga?.Protocolo ?? 0,
                Emitente = carregamento?.Filiais,
                Destinatario = carregamento?.Destinatarios,
                TipoCarga = carregamento?.TipoDeCarga?.Descricao,
                NotaMercadoLivre = string.Join(", ", carregamento.Pedidos.Where(o => o.Pedido.NotaMercadoLivre != null).Select(o => o.Pedido.NotaMercadoLivre)),
                SiglaFaturamentoMercadoLivre = string.Join(", ", carregamento.Pedidos.Where(o => o.Pedido.SiglaFaturamentoMercadoLivre != null).Select(o => o.Pedido.SiglaFaturamentoMercadoLivre)),
                PFMercadoLivre = string.Join(", ", carregamento.Pedidos.Where(o => o.Pedido.PFMercadoLivre != null).Select(o => o.Pedido.PFMercadoLivre)),
                ItemFaturadoMercadoLivre = string.Join(", ", carregamento.Pedidos.Where(o => o.Pedido.ItemFaturadoMercadoLivre != null).Select(o => o.Pedido.ItemFaturadoMercadoLivre))
            };
        } 

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork, bool isGridExportacao)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("NumeroCarregamento").Nome("Número Carregamento").Tamanho(8).Align(Models.Grid.Align.left);
            grid.Prop("Carga").Nome("Carga").Tamanho(8).Align(Models.Grid.Align.left).Ord(false);
            grid.Prop("Protocolo").Nome("Protocolo").Tamanho(5).Align(Models.Grid.Align.left).Ord(false);
            grid.Prop("Emitente").Nome("Emitente").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Destinatario").Nome("Destinatário").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("TipoCarga").Nome("Tipo Carga").Tamanho(10).Align(Models.Grid.Align.left).Ord(false);

            if (isGridExportacao)
            {
                grid.Prop("NotaMercadoLivre").Nome("Nota Fiscal").Tamanho(10).Align(Models.Grid.Align.left);
                grid.Prop("SiglaFaturamentoMercadoLivre").Nome("Sigla Identificação pedido faturado").Tamanho(10).Align(Models.Grid.Align.left);
                grid.Prop("PFMercadoLivre").Nome("PF do pedido").Tamanho(10).Align(Models.Grid.Align.left);
                grid.Prop("ItemFaturadoMercadoLivre").Nome("Item faturado").Tamanho(10).Align(Models.Grid.Align.left);
            }

            return grid;
        }

        private Models.Grid.Grid GridPesquisaIntegracoes(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.Prop("Codigo");
            grid.Prop("NumeroCarregamento").Nome("Carregamentos").Tamanho(10).Align(Models.Grid.Align.left).Ord(false);
            grid.Prop("Carga").Nome("Cargas").Tamanho(10).Align(Models.Grid.Align.left).Ord(false);
            grid.Prop("Integradora").Nome("Integradora").Tamanho(8).Align(Models.Grid.Align.left);
            grid.Prop("PrimeiroEnvio").Nome("Primeiro Envio").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("UltimoEnvio").Nome("Último Envio").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("NumeroEnvios").Nome("N° Envios").Tamanho(5).Align(Models.Grid.Align.right);
            grid.Prop("Data").Nome("Data").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("MensagemRetorno").Nome("Retorno").Tamanho(15).Align(Models.Grid.Align.left).Ord(false);

            return grid;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> BuscarCarregamentosSelecionados(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> lista = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();

            int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Usuario.Empresa.Codigo : 0;
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento()
            {
                DataFim = Request.GetDateTimeParam("DataFim"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                CpfCnpjRecebedor = Request.GetDoubleParam("Emitente"),
                CodigoCargaEmbarcador = Request.GetStringParam("Carga"),
                NumeroCarregamento = Request.GetStringParam("NumeroCarregamento"),
                CodigosEmpresa = codigoEmpresa > 0 ? new List<int>() { codigoEmpresa } : null
            };

            if (Request.GetBoolParam("SelecionarTodos"))
            {
                try
                {
                    lista = repositorioCarregamento.ConsultarCarregamentosLoteintegracao(filtroPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        LimiteRegistros = 1000
                    });
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    throw new ControllerException("Ocorreu uma falha ao obter os carregamentos selecionados.");
                }

                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CarregamentosNaoSelecionados"));

                foreach (var dynNaoSelecionada in listaNaoSelecionadas)
                    lista.Remove(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento() { Codigo = (int)dynNaoSelecionada.Codigo });
            }
            else
            {
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CarregamentosSelecionados"));

                foreach (var dynSelecionada in listaSelecionadas)
                    lista.Add(repositorioCarregamento.BuscarPorCodigo((int)dynSelecionada.Codigo));
            }

            return lista;
        }

        #endregion
    }
}
