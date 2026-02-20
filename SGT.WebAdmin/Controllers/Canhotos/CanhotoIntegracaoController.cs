using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Canhotos
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Canhotos/CanhotoIntegracao")]
    public class CanhotoIntegracaoController : BaseController
    {
        #region Construtores

        public CanhotoIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                int totalRegistros = 0;
                Dominio.ObjetosDeValor.Embarcador.CanhotoIntegracao.FiltroPesquisaCanhotoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, filtrosPesquisa, unitOfWork);

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
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                int totalRegistros = 0;
                Dominio.ObjetosDeValor.Embarcador.CanhotoIntegracao.FiltroPesquisaCanhotoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, filtrosPesquisa, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

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
        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao = repCanhotoIntegracao.BuscarPorCodigo(codigo);

                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                canhotoIntegracao.IniciouConexaoExterna = false;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, canhotoIntegracao, null, "Reenviou integração", unidadeTrabalho);

                repCanhotoIntegracao.Atualizar(canhotoIntegracao);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ReenviarIntegracoes()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigos = Request.GetListParam<int>("Codigos");

                Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao> canhotoIntegracaoPendentes = repCanhotoIntegracao.BuscarPorCodigoSituacaoFalhaIntegracao(codigos);

                unidadeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao in canhotoIntegracaoPendentes)
                {
                    canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    repCanhotoIntegracao.Atualizar(canhotoIntegracao);
                }

                if (canhotoIntegracaoPendentes.Count == 0)
                {
                    return new JsonpResult(false, "Não existe arquivo com situação 'Falha ao Integrar' para reenvio.");
                }

                unidadeTrabalho.CommitChanges();

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

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao integracao = repCanhotoIntegracao.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
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
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número Documento", "Numero", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Emitente", "Emitente", 20, Models.Grid.Align.left, false);

            if (repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei))
            {
                grid.AdicionarCabecalho("Gatilho", "Gatilho", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Canhoto Validado?", "CanhotoValidado", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Número Canhoto Validado?", "ValidacaoNumero", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data do Canhoto Encontrada?", "ValidacaoEncontrouData", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Validou assinatura?", "ValidacaoAssinatura", 30, Models.Grid.Align.left, false);
            }

            grid.AdicionarCabecalho("Integradora", "Integradora", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Carga", "Carga", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("CT-e", "NumeroCTe", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Digitalização", "DataDigitalizacao", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Integração", "DataIntegracao", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situacao", "Situacao", 15, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Retorno", "MensagemRetorno", 30, Models.Grid.Align.left, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Dominio.ObjetosDeValor.Embarcador.CanhotoIntegracao.FiltroPesquisaCanhotoIntegracao filtroPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(unitOfWork);

            // Consulta
            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao> listaGrid = repCanhotoIntegracao.Consultar(filtroPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCanhotoIntegracao.ContarConsulta(filtroPesquisa);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Numero = obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars ? string.Empty : obj.Canhoto.Numero.ToString(),
                            Gatilho = obj.Canhoto.OrigemDigitalizacao.HasValue ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CanhotoOrigemDigitalizacaoHelper.ObterDescricao(obj.Canhoto.OrigemDigitalizacao.Value) : string.Empty,
                            CanhotoValidado = obj.ValidacaoCanhoto ? "Sim" : "Não",
                            ValidacaoNumero = obj.ValidacaoNumero ? "Sim" : "Não",
                            ValidacaoEncontrouData = obj.ValidacaoEncontrouData ? "Sim" : "Não",
                            ValidacaoAssinatura = obj.ValidacaoAssinatura ? "Sim" : "Não",
                            Emitente = obj.Canhoto?.Emitente?.Descricao,
                            Integradora = obj.TipoIntegracao.Descricao,
                            Carga = obj.Canhoto.Carga?.Descricao ?? string.Empty,
                            NumeroCTe = obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars ? string.Empty : obj.Canhoto.NumeroCTe,
                            DataIntegracao = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                            Situacao = obj.DescricaoSituacaoIntegracao,
                            Tentativas = obj.NumeroTentativas,
                            MensagemRetorno = obj.ProblemaIntegracao,
                            DataDigitalizacao = obj.Canhoto.DataDigitalizacao?.ToString("dd/MM/yyyy HH:mm"),
                            DT_RowColor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoHelper.ObterCorLinha(obj.SituacaoIntegracao),
                            DT_FontColor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoHelper.ObterCorFonte(obj.SituacaoIntegracao),
                        };

            return lista.ToList();
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Canhotos.CanhotoIntegracao repIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta Canhoto " + integracao.Canhoto.Numero + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do arquivo de integração.");
            }
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Situacao") propOrdenar = "SituacaoIntegracao";
        }

        #endregion Métodos Privados
        private Dominio.ObjetosDeValor.Embarcador.CanhotoIntegracao.FiltroPesquisaCanhotoIntegracao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.CanhotoIntegracao.FiltroPesquisaCanhotoIntegracao filtroPesquisaCanhoto = new Dominio.ObjetosDeValor.Embarcador.CanhotoIntegracao.FiltroPesquisaCanhotoIntegracao
            {
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao"),
                NumeroDocumento = Request.GetIntParam("NumeroDocumento"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Usuario.Empresa.Codigo : 0,
                Emitente = Request.GetDoubleParam("Emitente"),
                CodigoCanhoto = Request.GetIntParam("CodigoCanhoto"),
                Carga = Request.GetIntParam("Carga"),
                Filial = Request.GetIntParam("Filial"),
                Transportador = Request.GetIntParam("Transportador"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                CodigoTipoIntegracao = Request.GetIntParam("TipoIntegracao"),
            };

            return filtroPesquisaCanhoto;
        }
    }
}
