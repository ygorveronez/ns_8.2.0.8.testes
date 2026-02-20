//using SGTAdmin.Controllers;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//
//using Microsoft.AspNetCore.Mvc;


//namespace SGT.WebAdmin.Controllers.Escrituracao
//{

//    [CustomAuthorize("Escrituracao/LoteEscrituracao")]
//    public class DocumentoEscrituracaoController : BaseController
//    {
//        [AllowAuthenticate]
//        public async Task<IActionResult> PesquisaDocumento()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                // Manipula grids
//                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
//                {
//                    header = new List<Models.Grid.Head>()
//                };

//                // Cabecalhos grid
//                grid.AdicionarCabecalho("Codigo", false);
//                grid.AdicionarCabecalho("Filial", false);
//                grid.AdicionarCabecalho("CodigoFilial", false);
//                grid.AdicionarCabecalho("CodigoTomador", false);
//                grid.AdicionarCabecalho("Documento", "Documento", 8, Models.Grid.Align.center, true);
//                grid.AdicionarCabecalho("Tipo", "Tipo", 8, Models.Grid.Align.center, true);
//                grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 8, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Emissão NF-e", "DataEmissao", 8, Models.Grid.Align.center, true);
//                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Transportador", "Empresa", 20, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 15, Models.Grid.Align.right, true);

//                // Ordenacao da grid
//                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
//                if (propOrdenar == "Carga") propOrdenar = "Carga.CodigoCargaEmbarcador";
//                else if (propOrdenar == "Ocorrencia") propOrdenar = "CargaOcorrencia.NumeroOcorrencia";
//                else if (propOrdenar == "Destinatario") propOrdenar = "Destinatario.Nome";
//                else if (propOrdenar == "Tomador") propOrdenar = "Tomador.Nome";

//                // Busca Dados
//                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> listaGrid = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();
//                int totalRegistros = 0;
//                var lista = ExecutaPesquisaDocumento(ref listaGrid, ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

//                // Seta valores na grid
//                grid.AdicionaRows(lista);
//                grid.setarQuantidadeTotal(totalRegistros);

//                return new JsonpResult(grid);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        private dynamic ExecutaPesquisaDocumento(ref List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> listaGrid, ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
//        {

//            // Instancia repositorios
//            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);

//            // Dados do filtro
//            // Quando há um codigo, tras apenas os documentos daquela nfsmanual
//            int.TryParse(Request.Params("LoteEscrituracao"), out int loteEscrituracao);

//            int transportador = 0;

//            int.TryParse(Request.Params("Empresa"), out transportador);

//            int.TryParse(Request.Params("Filial"), out int filial);
//            double.TryParse(Request.Params("Tomador"), out double tomador);


//            DateTime.TryParse(Request.Params("DataInicio"), out DateTime dataInicio);
//            DateTime.TryParse(Request.Params("DataFim"), out DateTime dataFim);

//            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento situacaoEscrituracaoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.Todos;
//            bool somenteSemLote = true;
//            if (loteEscrituracao > 0)
//            {
//                somenteSemLote = false;
//                situacaoEscrituracaoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.AgEscrituracao;
//            }

//            listaGrid = repDocumentoEscrituracao.Consultar(loteEscrituracao, somenteSemLote, dataInicio, dataFim, transportador, filial, tomador, situacaoEscrituracaoDocumento, propOrdenar, dirOrdena, inicio, limite);
//            totalRegistros = repDocumentoEscrituracao.ContarConsulta(loteEscrituracao, somenteSemLote, dataInicio, dataFim, transportador, filial, tomador, situacaoEscrituracaoDocumento);

//            var lista = from obj in listaGrid
//                        select new
//                        {
//                            obj.Codigo,
//                            CodigoFilial = obj.Filial?.Codigo ?? 0,
//                            CodigoTomador = obj.CTe.TomadorPagador.Cliente.Codigo,
//                            Empresa = obj.CTe.Empresa?.RazaoSocial,
//                            Documento = obj.CTe.Numero.ToString() + " - " + obj.CTe.Serie.Numero.ToString(),
//                            Tipo = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
//                            Carga = obj.Carga?.CodigoCargaEmbarcador ?? "",
//                            Ocorrencia = obj.CargaOcorrencia?.NumeroOcorrencia.ToString() ?? "",
//                            DataEmissao = obj.CTe.DataEmissao?.ToString("dd/MM/yyyy") ?? "",
//                            Tomador = obj.CTe.TomadorPagador.Cliente.Descricao,
//                            Filial = obj.Filial?.Descricao ?? "",
//                            ValorFrete = obj.CTe.ValorFrete.ToString("n2")
//                        };

//            return lista.ToList();
//        }

//    }
//}
