using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize(new string[] { "PesquisaCTeSemCarga", "Pesquisa" }, "CTe/CancelamentoCTeSemCarga")]
    public class CancelamentoCTeSemCargaController : BaseController
    {
        #region Construtores

        public CancelamentoCTeSemCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos
        public async Task<IActionResult> PesquisaCTeSemCarga(CancellationToken cancellationToke)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emitente", "Emitente", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Retorno SEFAZ", "RetornoSEFAZ", 12, Models.Grid.Align.left, false,false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                else if (propOrdenacao == "Origem")
                    propOrdenacao = "LocalidadeInicioPrestacao.Descricao";
                else if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
               
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.ConsultarCTesSemCarga(propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int countCTes = ctes!= null ? ctes.Count : 0;

                grid.setarQuantidadeTotal(countCTes);

                var lista = (from obj in ctes
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = obj.Status,
                                 obj.Numero,
                                 Serie = obj.Serie.Numero,
                                 DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                 Emitente = obj.Empresa.RazaoSocial + "(" + obj.Empresa.CNPJ_Formatado + ")",
                                 Remetente = obj.Remetente.Nome + "(" + obj.Remetente.CPF_CNPJ_Formatado + ")",
                                 Origem = obj.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                 Destinatario = obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")",
                                 Destino = obj.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                 ValorFrete = obj.ValorAReceber.ToString("n2"),
                                 Status = obj.DescricaoStatus,
                                 RetornoSEFAZ = !string.IsNullOrWhiteSpace(obj.MensagemRetornoSefaz) ? obj.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.MensagemRetornoSefaz) : "" : "",
                                 DT_RowColor = obj.Status != null ? obj.Status == "A" ? "#dff0d8" : obj.Status == "R" ? "rgba(193, 101, 101, 1)" : (obj.Status == "C" || obj.Status == "I" || obj.Status == "D" || obj.Status == "Z") ? "#777" : "" : "",
                                 DT_FontColor = "#000000",
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToke)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Cancelamento", "DataInclusao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Solicitante", "UsuarioInclusao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motivo Cancelamento", "MotivoCancelamento", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motivo Rejeição", "MotivoRejeicao", 12, Models.Grid.Align.left, false);

                Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga repCancelamentoSemCarga = new Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCancelamentoCTeSemCarga filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
                List<Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga> cancelamentoCteSemCarga = repCancelamentoSemCarga.Consultar(filtrosPesquisa, parametroConsulta);
                int countCTes = cancelamentoCteSemCarga != null ? cancelamentoCteSemCarga.Count : 0;

                grid.setarQuantidadeTotal(countCTes);

                var lista = (from obj in cancelamentoCteSemCarga
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = obj.Status.Descricao(),
                                 obj.DataInclusao,
                                 UsuarioInclusao = obj.Usuario.Nome,
                                 obj.MotivoCancelamento,
                                 obj.MotivoRejeicao,
                                 DT_RowColor = obj != null ? obj.Status == SituacaoCancelamentoCTeSemCarga.Cancelado ? "#777" : obj.Status == SituacaoCancelamentoCTeSemCarga.RejeicaoCancelamento ? "rgba(193, 101, 101, 1)" : (obj.Status == SituacaoCancelamentoCTeSemCarga.AgCancelamentoCTe || obj.Status == SituacaoCancelamentoCTeSemCarga.AgCancelamentoIntegracao) ? " " : "" : "",
                                 DT_FontColor = obj != null ? obj.Status == SituacaoCancelamentoCTeSemCarga.Cancelado ? "#F9F8F9" : "#000000": "#000000",
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);


            }
            catch(Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os CT-es.");
            }

        }

        public async Task<IActionResult> BuscarCTePorCodigoCancelamento(CancellationToken cancellationToke)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigoCancelamento = Request.GetIntParam("CodigoCancelamento");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emitente", "Emitente", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Retorno SEFAZ", "RetornoSEFAZ", 12, Models.Grid.Align.left, false, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                else if (propOrdenacao == "Origem")
                    propOrdenacao = "LocalidadeInicioPrestacao.Descricao";
                else if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga repCancelamentoSemCarga = new Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga(unitOfWork);
                Repositorio.Embarcador.CTe.CancelamentoCTe repCancelamentoCTe = new Repositorio.Embarcador.CTe.CancelamentoCTe(unitOfWork);
                var cancelamento = repCancelamentoSemCarga.BuscarPorCodigo(codigoCancelamento);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesCancelamento = repCancelamentoCTe.BuscarPorCodigoCancelamentoCTe(codigoCancelamento);
                int countCTes = ctesCancelamento != null ? ctesCancelamento.Count : 0;

                var retorno = (from obj in ctesCancelamento
                               select new
                               {
                                   NumeroCancelamento = cancelamento.Codigo,
                                   DataCancelamento = cancelamento.DataInclusao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   MensagemRejeicaoCancelamento = cancelamento.MotivoRejeicao,
                                   obj.Codigo,
                                   Situacao = obj.Status,
                                   obj.Numero,
                                   Serie = obj.Serie.Numero,
                                   DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   Emitente = obj.Empresa.RazaoSocial + "(" + obj.Empresa.CNPJ_Formatado + ")",
                                   Remetente = obj.Remetente.Nome + "(" + obj.Remetente.CPF_CNPJ_Formatado + ")",
                                   Origem = obj.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                   Destinatario = obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")",
                                   Destino = obj.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                   ValorFrete = obj.ValorAReceber.ToString("n2"),
                                   Status = obj.DescricaoStatus,
                                   RetornoSEFAZ = !string.IsNullOrWhiteSpace(obj.MensagemRetornoSefaz) ? obj.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.MensagemRetornoSefaz) : "" : "",
                                   DT_RowColor = obj.Status == "A" ? "#dff0d8" :
                                                 obj.Status == "R" ? "rgba(193, 101, 101, 1)" :
                                                 (obj.Status == "C" || obj.Status == "I" || obj.Status == "D") ? "#777" :
                                                 "",
                                   DT_FontColor = (obj.Status == "R" || obj.Status == "C" || obj.Status == "I" || obj.Status == "D") ? "#FFFFFF" : "",
                               }).ToList();

                grid.setarQuantidadeTotal(countCTes);
                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToke)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCancelamento = Request.GetIntParam("Codigo");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga repCancelamentoSemCarga = new Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga(unitOfWork);
                Repositorio.Embarcador.CTe.CancelamentoCTe repCancelamentoCTe = new Repositorio.Embarcador.CTe.CancelamentoCTe(unitOfWork);
                var cancelamento = repCancelamentoSemCarga.BuscarPorCodigo(codigoCancelamento);

                var retorno = (new
                               {
                                   cancelamento.Codigo,
                                   NumeroCancelamento = cancelamento.Codigo,
                                   DataCancelamento = cancelamento.DataInclusao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   MensagemRejeicaoCancelamento = cancelamento.MotivoRejeicao,
                                   Situacao = cancelamento.Status.Descricao(),
                                   cancelamento.Status,
                               });

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar(CancellationToken cancellationToke)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho, cancellationToke);
                Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga cancelamentoCTeSemCarga = new Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga();
                var listaCondigosCtes = RetornaCodigosCTes(unidadeTrabalho);

                string erro = string.Empty;

                unidadeTrabalho.Start();

                if (listaCondigosCtes.Count > 0 )
                {
                    Servicos.Embarcador.Carga.Cancelamento servCancelamento = new Servicos.Embarcador.Carga.Cancelamento();
                    servCancelamento.AdicionarCancelamentoCTeSemCarga(listaCondigosCtes, unidadeTrabalho,this.Usuario);
                }
                else{
                    return new JsonpResult(true, "Selecione 1 ou mais CT-es para cancelamento !");
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cancelamentoCTeSemCarga, null, "Solicitou Cancelamento CT-e sem Carga", unidadeTrabalho);
                unidadeTrabalho.CommitChanges();


                return new JsonpResult(true,"Cancelamento de CT-es sem carga enviado com sucesso !");
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao cancelar o documento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCancelamentoCTeSemCarga ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCancelamentoCTeSemCarga filtrosPesquisaConsultaCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCancelamentoCTeSemCarga()
            {
                DataInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                StatusCancelamento = Request.GetEnumParam<SituacaoCancelamentoCTeSemCarga>("Status")
            };

            return filtrosPesquisaConsultaCTe;
        }

        private List<int> RetornaCodigosCTes(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaCTes")))
            {
                dynamic listaCtes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCTes"));
                if (listaCtes != null)
                {
                    foreach (var cte in listaCtes)
                    {
                        listaCodigos.Add(int.Parse((string)cte.CodigoCTe));
                    }
                }
            }
            return listaCodigos;
        }


        #endregion
    }
}
