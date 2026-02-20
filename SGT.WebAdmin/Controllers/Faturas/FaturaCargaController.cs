using OfficeOpenXml;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize(new string[] { "PesquisaCargas", "ObterResumoCargas", "PesquisaDocumentosCarga", "PesquisaCargasSemFatura" }, "Faturas/Fatura", "SAC/AtendimentoCliente")]
    public class FaturaCargaController : BaseController
    {
		#region Construtores

		public FaturaCargaController(Conexao conexao) : base(conexao) { }

		#endregion


        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Fatura.FaturaCarga repCargaCTe = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                int inicio = int.Parse(Request.Params("inicio"));
                int limite = int.Parse(Request.Params("limite"));

                int numeroCTe, numeroCarga, codigoFatura, numeroCTeFinal = 0;
                int.TryParse(Request.Params("NumeroCTeFinal"), out numeroCTeFinal);
                int.TryParse(Request.Params("NumeroCTe"), out numeroCTe);
                int.TryParse(Request.Params("NumeroCarga"), out numeroCarga);
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);

                string numeroPedido = Request.Params("NumeroPedido");
                string numeroOcorrencia = Request.Params("NumeroOcorrencia");

                decimal aliquotaICMS = 0;
                decimal.TryParse(Request.Params("AliquotaICMS"), out aliquotaICMS);

                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;
                Enum.TryParse(Request.Params("TipoCTe"), out tipoCTe);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = null;
                int quantidade = 0;
                if (numeroCTe > 0 || numeroCTeFinal > 0 || !string.IsNullOrWhiteSpace(numeroPedido) || aliquotaICMS > 0 || tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                {
                    listaCarga = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                    List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> documentosFatura = repCargaCTe.BuscarPorNumeroCTe(numeroCTe, codigoFatura, numeroCTeFinal, numeroPedido, numeroOcorrencia, aliquotaICMS, tipoCTe);
                    quantidade = documentosFatura.Count();
                    foreach (Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento documento in documentosFatura)
                    {
                        listaCarga.Add(documento.Carga);
                    }
                }
                if (listaCarga == null)
                {
                    listaCarga = repCargaCTe.Consultar(numeroCarga.ToString(), numeroOcorrencia, codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Todas, "Codigo", "desc", inicio, limite);
                    quantidade = repCargaCTe.ContarConsulta(numeroCarga.ToString(), numeroOcorrencia, codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Todas);
                }

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                List<dynamic> lista = (from carga in listaCarga select serCarga.ObterDetalhesDaCargaParaFatura(codigoFatura, carga, TipoServicoMultisoftware, unitOfWork)).ToList();

                return new JsonpResult(lista, quantidade);

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
        public async Task<IActionResult> ObterResumoCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                if (codigo > 0)
                {
                    var dynRetorno = servFatura.RetornaObjetoResumoCargaFatura(codigo, unitOfWork);
                    return new JsonpResult(dynRetorno);
                }
                else
                {
                    return new JsonpResult(true, "Favor selecione/inicie uma fatura.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o resumo das cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasSemFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                string codigoCargaEmbarcador = Request.Params("CodigoCargaEmbarcador");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga;
                Enum.TryParse(Request.Params("Situacao"), out situacaoCarga);
                int codigoFatura, numeroDocumento;
                int.TryParse(Request.Params("Fatura"), out codigoFatura);
                int.TryParse(Request.Params("NumeroDocumento"), out numeroDocumento);
                DateTime dataCarga;
                DateTime.TryParse(Request.Params("DataCarga"), out dataCarga);
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura;
                if (codigoFatura > 0)
                    fatura = repFatura.BuscarPorCodigo(codigoFatura);
                else
                    return new JsonpResult(false, "Favor selecione/inicie uma fatura.");


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emitente", "Emitente", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motoristas", "Motoristas", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Data")
                    propOrdenacao = "DataCriacaoCarga";

                dynamic listaCarga = repCarga.BuscarCargasSemFatura(fatura, codigoCargaEmbarcador, numeroDocumento, dataCarga, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int quantidade = repCarga.ContarBuscarCargasSemFatura(fatura, codigoCargaEmbarcador, numeroDocumento, dataCarga);

                grid.setarQuantidadeTotal(quantidade);

                //var dynListaCarga = (from obj in listaCarga
                //                     select new
                //                     {
                //                         obj.Codigo,
                //                         Data = obj.DataCriacaoCarga.ToString("dd/MM/yyyy"),
                //                         obj.CodigoCargaEmbarcador,
                //                         Emitente =  serCargaDadosSumarizados.ObterOrigemTMS(obj, unitOfWork),
                //                         Motoristas = obj.NomeMotoristas,
                //                         Veiculo = obj.PlacasVeiculos
                //                     }).ToList();

                grid.AdicionaRows(listaCarga);
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

        public async Task<IActionResult> IniserirNovaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteAdicionarNovasCargas)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para adicionar novas cargas a fatura.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int codigoCarga, codigoFatura;
                int.TryParse(Request.Params("CodigoCarga"), out codigoCarga);
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura;
                if (codigoFatura > 0)
                    fatura = repFatura.BuscarPorCodigo(codigoFatura);
                else
                    return new JsonpResult(false, "Por favor inicie uma fatura antes.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga;
                if (codigoCarga > 0)
                    carga = repCarga.BuscarPorCodigo(codigoCarga);
                else
                    return new JsonpResult(false, "Por favor selecione uma carga antes.");

                //unitOfWork.Start(); //removido por causa da Avon, vai travar o sistema

                servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.AdicionouNovaCarga, this.Usuario);

                if (!repFaturaCarga.ContemCargaoFatura(codigoFatura, codigoCarga))
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaCarga faturaCarga = new Dominio.Entidades.Embarcador.Fatura.FaturaCarga();
                    faturaCarga.Carga = carga;
                    faturaCarga.Fatura = fatura;
                    faturaCarga.StatusFaturaCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.Faturada;

                    repFaturaCarga.Inserir(faturaCarga, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Inseriu a Carga " + carga.Descricao +" na Fatura.", unitOfWork);
                }

                int count = 0;
                List<int> codigosCTesSemFatura = repCarga.BuscarCargasCTeSemFatura(fatura, carga.Codigo);

                foreach (int codigoCTe in codigosCTesSemFatura)
                {
                    count++;

                    //if (!repFaturaCargaDocumento.ContemDocumentoFatura(fatura.Codigo, codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento))
                    //{
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento faturaDocumento = new Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento();
                    faturaDocumento.Carga = carga;
                    faturaDocumento.ConhecimentoDeTransporteEletronico = repCTe.BuscarPorCodigo(codigoCTe);
                    faturaDocumento.Fatura = fatura;
                    faturaDocumento.NFSe = null;
                    faturaDocumento.NumeroFatura = fatura.Numero;
                    faturaDocumento.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal;
                    faturaDocumento.TipoDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento;
                    repFaturaCargaDocumento.Inserir(faturaDocumento, Auditado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Adicionou o documento " + faturaDocumento.ConhecimentoDeTransporteEletronico.Descricao + " a fatura.", unitOfWork);

                    faturaDocumento.ConhecimentoDeTransporteEletronico.Fatura = fatura;
                    repCTe.Atualizar(faturaDocumento.ConhecimentoDeTransporteEletronico);

                    unitOfWork.CommitChanges();
                    //}

                    if (count % 20 == 0)
                    {
                        count = 0;
                        unitOfWork.FlushAndClear();
                    }
                }

                //unitOfWork.CommitChanges(); //removido por causa da Avon, vai travar o sistema

                servFatura.AtualizaStatusFaturaCarga(codigoCarga, codigoFatura, unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao lançar nova carga a fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumentosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumentoExcluido repFaturaCargaDocumentoExcluido = new Repositorio.Embarcador.Fatura.FaturaCargaDocumentoExcluido(unitOfWork);


                int codigoFatura, codigoCarga, numeroDocumento, numeroDocumentoFinal, codigoModeloDocumento;
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);
                int.TryParse(Request.Params("Codigo"), out codigoCarga);
                int.TryParse(Request.Params("NumeroDocumento"), out numeroDocumento);
                int.TryParse(Request.Params("NumeroDocumentoFinal"), out numeroDocumentoFinal);
                int.TryParse(Request.Params("ModeloDocumento"), out codigoModeloDocumento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento;
                Enum.TryParse(Request.Params("TipoDocumento"), out tipoDocumento);

                decimal valorDocumento;
                decimal.TryParse(Request.Params("ValorDocumento"), out valorDocumento);

                string numeroPedido = Request.Params("NumeroPedido");
                string numeroOcorrencia = Request.Params("NumeroOcorrencia");

                decimal aliquotaICMS = 0;
                decimal.TryParse(Request.Params("AliquotaICMS"), out aliquotaICMS);

                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;
                Enum.TryParse(Request.Params("TipoCTe"), out tipoCTe);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Série", "Serie", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo Doc", "DescricaoTipoDocumento", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("% ICMS", "AliquotaICMS", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Tipo CT-e", "TipoCTE", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Fatura", "Fatura", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("CodigoFatura", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Numero")
                    propOrdenar = "ConhecimentoDeTransporteEletronico.Numero";
                else if (propOrdenar == "Serie")
                    propOrdenar = "ConhecimentoDeTransporteEletronico.Serie.Numero";
                else if (propOrdenar == "NumeroCarga")
                    propOrdenar = "Carga.CodigoCargaEmbarcador";
                else if (propOrdenar == "DataEmissao")
                    propOrdenar = "ConhecimentoDeTransporteEletronico.DataEmissao";
                else if (propOrdenar == "AliquotaICMS")
                    propOrdenar = "ConhecimentoDeTransporteEletronico.AliquotaICMS";

                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> listaCTe = repFaturaCargaDocumento.ConsultaDocumentosCargaFatura(codigoModeloDocumento, "", numeroPedido, numeroOcorrencia, aliquotaICMS, tipoCTe, tipoDocumento, valorDocumento, numeroDocumento, codigoCarga, fatura, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, numeroDocumentoFinal);
                grid.setarQuantidadeTotal(repFaturaCargaDocumento.ContarConsultaDocumentosCargaFatura(codigoModeloDocumento, "", numeroPedido, numeroOcorrencia, aliquotaICMS, tipoCTe, tipoDocumento, valorDocumento, numeroDocumento, codigoCarga, fatura, numeroDocumentoFinal));

                var lista = (from p in listaCTe
                             select new
                             {
                                 p.ConhecimentoDeTransporteEletronico.Codigo,
                                 p.ConhecimentoDeTransporteEletronico.Numero,
                                 Serie = p.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString("n0"),
                                 DescricaoTipoDocumento = p.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal != null ? p.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.Abreviacao : p.DescricaoTipoDocumento,
                                 DataEmissao = p.ConhecimentoDeTransporteEletronico.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 Valor = p.ConhecimentoDeTransporteEletronico.ValorAReceber.ToString("n2"),
                                 AliquotaICMS = p.ConhecimentoDeTransporteEletronico.AliquotaICMS.ToString("n2"),
                                 TipoCTE = p.ConhecimentoDeTransporteEletronico.DescricaoTipoCTE,
                                 Fatura = p.Fatura != null && p.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal ? p.Fatura.Numero.ToString("n0") : string.Empty,
                                 DT_RowColor = repFaturaCarga.BuscarPorCargaFatura(codigoCarga, codigoFatura).StatusFaturaCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.NaoFaturada || repFaturaCargaDocumentoExcluido.CTeExcluidoEmFatura(p.ConhecimentoDeTransporteEletronico.Codigo, codigoFatura) ? "#FF8C69" : p.ConhecimentoDeTransporteEletronico.Fatura != null && p.ConhecimentoDeTransporteEletronico.Fatura.Codigo != fatura.Codigo ? "#ADD8E6" : "#FFFFFF",
                                 CodigoCarga = p.Carga.Codigo,
                                 CodigoFatura = p.Fatura.Codigo
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaConhecimentosCargaFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumentoExcluido repFaturaCargaDocumentoExcluido = new Repositorio.Embarcador.Fatura.FaturaCargaDocumentoExcluido(unitOfWork);

                int codigoFatura, numeroDocumento, numeroDocumentoFinal, codigoModeloDocumento;
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);
                int.TryParse(Request.Params("NumeroDocumento"), out numeroDocumento);
                int.TryParse(Request.Params("NumeroDocumentoFinal"), out numeroDocumentoFinal);
                int.TryParse(Request.Params("ModeloDocumento"), out codigoModeloDocumento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento;
                Enum.TryParse(Request.Params("TipoDocumento"), out tipoDocumento);

                decimal valorDocumento;
                decimal.TryParse(Request.Params("ValorDocumento"), out valorDocumento);

                string numeroPedido = Request.Params("NumeroPedido");
                string numeroOcorrencia = Request.Params("NumeroOcorrencia");

                decimal aliquotaICMS = 0;
                decimal.TryParse(Request.Params("AliquotaICMS"), out aliquotaICMS);

                string numeroCarga = Request.Params("NumeroCarga");

                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;
                Enum.TryParse(Request.Params("TipoCTe"), out tipoCTe);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Série", "Serie", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo Doc", "DescricaoTipoDocumento", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("% ICMS", "AliquotaICMS", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Tipo CT-e", "TipoCTE", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("CodigoFatura", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Numero")
                    propOrdenar = "ConhecimentoDeTransporteEletronico.Numero";
                else if (propOrdenar == "Serie")
                    propOrdenar = "ConhecimentoDeTransporteEletronico.Serie.Numero";
                else if (propOrdenar == "NumeroCarga")
                    propOrdenar = "Carga.CodigoCargaEmbarcador";
                else if (propOrdenar == "DataEmissao")
                    propOrdenar = "ConhecimentoDeTransporteEletronico.DataEmissao";
                else if (propOrdenar == "AliquotaICMS")
                    propOrdenar = "ConhecimentoDeTransporteEletronico.AliquotaICMS";

                if (fatura == null)
                {
                    List<dynamic> dynlista = new List<dynamic>();

                    var lista = new
                    {
                        Codigo = "",
                        Numero = "",
                        Serie = "",
                        DescricaoTipoDocumento = "",
                        DataEmissao = "",
                        Valor = "",
                        AliquotaICMS = "",
                        TipoCTE = "",
                        NumeroCarga = "",
                        DT_RowColor = "#FFFFFF",
                        CodigoCarga = "",
                        CodigoFatura = ""
                    };

                    dynlista.Add(lista);
                    grid.AdicionaRows(dynlista);
                    return new JsonpResult(grid);
                }

                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> listaCTe = repFaturaCargaDocumento.ConsultaDocumentosCargaFatura(codigoModeloDocumento, numeroCarga, numeroPedido, numeroOcorrencia, aliquotaICMS, tipoCTe, tipoDocumento, valorDocumento, numeroDocumento, 0, fatura, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, numeroDocumentoFinal);
                grid.setarQuantidadeTotal(repFaturaCargaDocumento.ContarConsultaDocumentosCargaFatura(codigoModeloDocumento, numeroCarga, numeroPedido, numeroOcorrencia, aliquotaICMS, tipoCTe, tipoDocumento, valorDocumento, numeroDocumento, 0, fatura, numeroDocumentoFinal));

                if (listaCTe.Count() > 0)
                {
                    var lista = (from p in listaCTe
                                 select new
                                 {
                                     Codigo = p.ConhecimentoDeTransporteEletronico != null ? p.ConhecimentoDeTransporteEletronico?.Codigo : 0,
                                     Numero = p.ConhecimentoDeTransporteEletronico != null ? p.ConhecimentoDeTransporteEletronico?.Numero : 0,
                                     Serie = p.ConhecimentoDeTransporteEletronico != null && p.ConhecimentoDeTransporteEletronico.Serie != null ? p.ConhecimentoDeTransporteEletronico?.Serie?.Numero.ToString("n0") : string.Empty,
                                     DescricaoTipoDocumento = p.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal != null ? p.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.Abreviacao : p.DescricaoTipoDocumento,
                                     DataEmissao = p.ConhecimentoDeTransporteEletronico != null && p.ConhecimentoDeTransporteEletronico.DataEmissao.HasValue ? p.ConhecimentoDeTransporteEletronico?.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                     Valor = p.ConhecimentoDeTransporteEletronico != null ? p.ConhecimentoDeTransporteEletronico?.ValorAReceber.ToString("n2") : string.Empty,
                                     AliquotaICMS = p.ConhecimentoDeTransporteEletronico.AliquotaICMS.ToString("n2"),
                                     TipoCTE = p.ConhecimentoDeTransporteEletronico.DescricaoTipoCTE,
                                     NumeroCarga = p.Carga != null ? p.Carga?.CodigoCargaEmbarcador : string.Empty,
                                     DT_RowColor = repFaturaCarga.BuscarPorCargaFatura(p.Carga != null ? p.Carga.Codigo : 0, codigoFatura) == null ? "#FFFFFF" : repFaturaCarga.BuscarPorCargaFatura(p.Carga != null ? p.Carga.Codigo : 0, codigoFatura).StatusFaturaCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.NaoFaturada || repFaturaCargaDocumentoExcluido.CTeExcluidoEmFatura(p.ConhecimentoDeTransporteEletronico != null ? p.ConhecimentoDeTransporteEletronico.Codigo : 0, codigoFatura) ? "#FF8C69" : p.ConhecimentoDeTransporteEletronico != null && p.ConhecimentoDeTransporteEletronico?.Fatura != null && p.ConhecimentoDeTransporteEletronico?.Fatura.Codigo != fatura.Codigo ? "#ADD8E6" : "#FFFFFF",
                                     CodigoCarga = p.Carga != null ? p.Carga?.Codigo : 0,
                                     CodigoFatura = p.Fatura != null ? p.Fatura.Codigo : 0
                                 }).ToList();

                    grid.AdicionaRows(lista);
                    return new JsonpResult(grid);
                }
                else
                {
                    List<dynamic> dynlista = new List<dynamic>();

                    var lista = new
                    {
                        Codigo = "",
                        Numero = "",
                        Serie = "",
                        DescricaoTipoDocumento = "",
                        DataEmissao = "",
                        Valor = "",
                        AliquotaICMS = "",
                        TipoCTE = "",
                        NumeroCarga = "",
                        DT_RowColor = "#FFFFFF",
                        CodigoCarga = "",
                        CodigoFatura = ""
                    };

                    dynlista.Add(lista);
                    grid.AdicionaRows(dynlista);
                    return new JsonpResult(grid);
                }

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

        public async Task<IActionResult> RemoverConhecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteRemoverConhecimento)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para remover um conhecimento da fatura.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumentoExcluido repFaturaCargaDocumentoExcluido = new Repositorio.Embarcador.Fatura.FaturaCargaDocumentoExcluido(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int codigoFatura, codigoCarga, codigoConhecimento;
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);
                int.TryParse(Request.Params("CodigoCarga"), out codigoCarga);
                int.TryParse(Request.Params("CodigoConhecimento"), out codigoConhecimento);

                if (codigoCarga > 0 && codigoConhecimento > 0 && codigoFatura > 0)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento documentoFatura = repFaturaCargaDocumento.BuscarPorFatura(codigoFatura, codigoConhecimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento);
                    if (documentoFatura != null)
                    {
                        documentoFatura.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido;
                        repFaturaCargaDocumento.Atualizar(documentoFatura);
                    }
                    else
                    {
                        documentoFatura.Carga = repCarga.BuscarPorCodigo(codigoCarga);
                        documentoFatura.ConhecimentoDeTransporteEletronico = repCTE.BuscarPorCodigo(codigoConhecimento);
                        documentoFatura.Fatura = repFatura.BuscarPorCodigo(codigoFatura);
                        documentoFatura.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido;
                        documentoFatura.TipoDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento;

                        repFaturaCargaDocumento.Inserir(documentoFatura);
                    }

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTE.BuscarPorCodigo(codigoConhecimento);
                    cte.Fatura = null;
                    repCTE.Atualizar(cte);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoFatura.Fatura, null, "Removeu o documento " + documentoFatura.ConhecimentoDeTransporteEletronico.Descricao + " da fatura.", unitOfWork);

                    unitOfWork.CommitChanges();

                    servFatura.AtualizaStatusFaturaCarga(codigoCarga, codigoFatura, unitOfWork);

                    return new JsonpResult(true, "Sucesso");
                }
                else
                    return new JsonpResult(false, "Por favor selecione uma carga, um conhecimento e uma fatura!");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> RealocarConhecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteRemoverConhecimento)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para realocar um conhecimento da fatura.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumentoExcluido repFaturaCargaDocumentoExcluido = new Repositorio.Embarcador.Fatura.FaturaCargaDocumentoExcluido(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int codigoFatura, codigoCarga, codigoConhecimento;
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);
                int.TryParse(Request.Params("CodigoCarga"), out codigoCarga);
                int.TryParse(Request.Params("CodigoConhecimento"), out codigoConhecimento);

                if (codigoCarga > 0 && codigoConhecimento > 0 && codigoFatura > 0)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento documentoFatura = repFaturaCargaDocumento.BuscarPorFatura(codigoFatura, codigoConhecimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento);

                    if (documentoFatura != null)
                    {
                        if (documentoFatura.ConhecimentoDeTransporteEletronico != null && documentoFatura.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento)
                        {
                            if (!repFaturaCargaDocumento.ContemDocumentoEmOutraFatura(codigoFatura, documentoFatura.ConhecimentoDeTransporteEletronico.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento))
                            {
                                documentoFatura.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal;
                                repFaturaCargaDocumento.Atualizar(documentoFatura);
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, "O Conhecimento " + documentoFatura.ConhecimentoDeTransporteEletronico.Numero + " está lançado em uma outra fatura!");
                            }
                        }
                    }
                    else
                    {
                        documentoFatura.Carga = repCarga.BuscarPorCodigo(codigoCarga);
                        documentoFatura.ConhecimentoDeTransporteEletronico = repCTE.BuscarPorCodigo(codigoConhecimento);
                        documentoFatura.Fatura = repFatura.BuscarPorCodigo(codigoFatura);
                        documentoFatura.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal;
                        documentoFatura.TipoDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento;

                        repFaturaCargaDocumento.Inserir(documentoFatura);
                    }
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoFatura.Fatura, null, "Realocou o documento " + documentoFatura.ConhecimentoDeTransporteEletronico.Descricao + " da fatura.", unitOfWork);

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTE.BuscarPorCodigo(codigoConhecimento);
                    cte.Fatura = repFatura.BuscarPorCodigo(codigoFatura);
                    repCTE.Atualizar(cte);

                    unitOfWork.CommitChanges();

                    servFatura.AtualizaStatusFaturaCarga(codigoCarga, codigoFatura, unitOfWork);

                    return new JsonpResult(true, "Sucesso");
                }
                else
                    return new JsonpResult(false, "Por favor selecione uma carga, um conhecimento e uma fatura!");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realocar o conhecimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> RemoverCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteCancelarCarga)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para cancelar uma carga.");

                //unitOfWork.Start();

                Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int codigoFatura, codigoCarga;
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);
                int.TryParse(Request.Params("CodigoCarga"), out codigoCarga);

                if (codigoCarga > 0 && codigoFatura > 0)
                {
                    int count = 0;

                    List<int> documentosFatura = repFaturaCargaDocumento.BuscarCodigosPorCarga(codigoFatura, codigoCarga);

                    foreach (int codigoDocumento in documentosFatura)
                    {
                        Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento documento = repFaturaCargaDocumento.BuscarPorCodigo(codigoDocumento);

                        count++;

                        documento.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido;
                        repFaturaCargaDocumento.Atualizar(documento);

                        documento.ConhecimentoDeTransporteEletronico.Fatura = null;
                        repCTE.Atualizar(documento.ConhecimentoDeTransporteEletronico);

                        if (count % 20 == 0)
                        {
                            unitOfWork.FlushAndClear();
                            count = 0;
                        }
                    }

                    Dominio.Entidades.Embarcador.Fatura.FaturaCarga faturaCarga = repFaturaCarga.BuscarPorCargaFatura(codigoCarga, codigoFatura);
                    faturaCarga.StatusFaturaCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.NaoFaturada;

                    repFaturaCarga.Atualizar(faturaCarga);

                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);
                    fatura.Total = repFaturaCarga.ValorConhecimentos(codigoFatura);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Removeu a Carga " + faturaCarga.Carga.Descricao + " da Fatura.", unitOfWork);

                    repFatura.Atualizar(fatura);

                    //unitOfWork.CommitChanges();

                    return new JsonpResult(true, "Sucesso");
                }
                else
                    return new JsonpResult(false, "Por favor selecione uma carga e uma fatura!");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> RealocarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteCancelarCarga)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para realocar uma carga.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int codigoFatura, codigoCarga;
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);
                int.TryParse(Request.Params("CodigoCarga"), out codigoCarga);

                if (codigoCarga > 0 && codigoFatura > 0)
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);
                    List<int> documentosFatura = repFaturaCargaDocumento.BuscarCodigosPorCarga(codigoFatura, codigoCarga);
                    int count = 0;

                    foreach (int codigoDocumento in documentosFatura)
                    {
                        Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento documento = repFaturaCargaDocumento.BuscarPorCodigo(codigoDocumento);

                        if (documento.ConhecimentoDeTransporteEletronico != null && documento.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento)
                        {
                            count++;

                            if (!repFaturaCargaDocumento.ContemDocumentoEmOutraFatura(codigoFatura, documento.ConhecimentoDeTransporteEletronico.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento))
                            {
                                documento.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal;
                                repFaturaCargaDocumento.Atualizar(documento);

                                documento.ConhecimentoDeTransporteEletronico.Fatura = fatura;
                                repCTE.Atualizar(documento.ConhecimentoDeTransporteEletronico);
                            }
                            //else
                            //    return new JsonpResult(false, "O Conhecimento " + documento.ConhecimentoDeTransporteEletronico.Numero + " está lançado em uma outra fatura!");

                            if (count % 20 == 0)
                            {
                                count = 0;
                                unitOfWork.FlushAndClear();
                            }
                        }
                    }

                    Dominio.Entidades.Embarcador.Fatura.FaturaCarga faturaCarga = repFaturaCarga.BuscarPorCargaFatura(codigoCarga, codigoFatura);
                    faturaCarga.StatusFaturaCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.Faturada;
                    repFaturaCarga.Atualizar(faturaCarga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Realocou a Carga " + faturaCarga.Carga.Descricao + " da Fatura.", unitOfWork);

                    return new JsonpResult(true, "Sucesso");
                }
                else
                    return new JsonpResult(false, "Por favor selecione uma carga e uma fatura!");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realocar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverRealocarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteCancelarCarga)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para realocar uma carga.");

                //unitOfWork.Start();

                Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int codigoFatura, numeroDocumento, numeroDocumentoFinal;
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);
                int.TryParse(Request.Params("NumeroDocumento"), out numeroDocumento);
                int.TryParse(Request.Params("NumeroDocumentoFinal"), out numeroDocumentoFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento;
                Enum.TryParse(Request.Params("TipoDocumento"), out tipoDocumento);

                decimal valorDocumento;
                decimal.TryParse(Request.Params("ValorDocumento"), out valorDocumento);

                long numeroPedido = 0;
                long.TryParse(Request.Params("NumeroPedido"), out numeroPedido);

                decimal aliquotaICMS = 0;
                decimal.TryParse(Request.Params("AliquotaICMS"), out aliquotaICMS);

                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;
                Enum.TryParse(Request.Params("TipoCTe"), out tipoCTe);

                string numeroCarga = Request.Params("NumeroCarga");

                if (codigoFatura > 0)
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Removeu Realocação de Carga da Fatura.", unitOfWork);
                    List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> documentosFatura = repFaturaCargaDocumento.ConsultaDocumentosCargaFatura(numeroCarga, numeroPedido, aliquotaICMS, tipoCTe, tipoDocumento, valorDocumento, numeroDocumento, 0, fatura, numeroDocumentoFinal);

                    foreach (Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento documento in documentosFatura)
                    {
                        if (documento.ConhecimentoDeTransporteEletronico != null && documento.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento)
                        {
                            if (!repFaturaCargaDocumento.ContemDocumentoEmOutraFatura(codigoFatura, documento.ConhecimentoDeTransporteEletronico.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento))
                            {
                                if (documento.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido)
                                    documento.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal;
                                else
                                    documento.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido;

                                repFaturaCargaDocumento.Atualizar(documento);

                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTE.BuscarPorCodigo(documento.ConhecimentoDeTransporteEletronico.Codigo);
                                if (documento.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal)
                                    cte.Fatura = fatura;
                                else
                                    cte.Fatura = null;
                                repCTE.Atualizar(cte);
                            }
                            //else
                            //    return new JsonpResult(false, "O Conhecimento " + documento.ConhecimentoDeTransporteEletronico.Numero + " está lançado em uma outra fatura, impossibilitando a sua remoção/realocação a esta!");
                        }
                    }

                    List<int> codigosCargas = (from obj in documentosFatura select obj.Carga.Codigo).Distinct().ToList();

                    foreach (int codigoCarga in codigosCargas)
                        servFatura.AtualizaStatusFaturaCarga(codigoCarga, codigoFatura, unitOfWork);

                    //unitOfWork.CommitChanges();

                    return new JsonpResult(true, "Sucesso");
                }
                else
                    return new JsonpResult(false, "Por favor selecione uma fatura!");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover/realocar os documentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> SalvarConhecimentoCargasFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteAdicionarNovasCargas)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para adicionar novas cargas a fatura.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int numeroDocumento, codigoFatura;
                int.TryParse(Request.Params("Numero").Replace(".", ""), out numeroDocumento);
                int.TryParse(Request.Params("Codigo"), out codigoFatura);

                if (numeroDocumento == 0)
                    return new JsonpResult(false, "Por favor informe o número do CT-e.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura;
                if (codigoFatura > 0)
                    fatura = repFatura.BuscarPorCodigo(codigoFatura);
                else
                    return new JsonpResult(false, "Por favor inicie uma fatura antes.");

                servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.AdicionouNovoCTe, this.Usuario);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repFaturaCarga.BuscarCargaCTeSemFatura(fatura, numeroDocumento);
                List<int> codigoCargas = new List<int>();
                if (listaCargaCTe.Count() > 0)
                {
                    for (int i = 0; i < listaCargaCTe.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento faturaDocumento = new Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento();
                        faturaDocumento.Carga = listaCargaCTe[i].Carga;
                        faturaDocumento.ConhecimentoDeTransporteEletronico = listaCargaCTe[i].CTe;
                        faturaDocumento.Fatura = fatura;
                        faturaDocumento.NFSe = null;
                        faturaDocumento.NumeroFatura = fatura.Numero;
                        faturaDocumento.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal;
                        faturaDocumento.TipoDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento;
                        repFaturaCargaDocumento.Inserir(faturaDocumento, Auditado);

                        faturaDocumento.ConhecimentoDeTransporteEletronico.Fatura = fatura;
                        repCTe.Atualizar(faturaDocumento.ConhecimentoDeTransporteEletronico);

                        if (!codigoCargas.Contains(listaCargaCTe[i].Carga.Codigo))
                            codigoCargas.Add(listaCargaCTe[i].Carga.Codigo);
                    }
                } else
                    return new JsonpResult(false, "Conhecimento não encontrado ou já lançado em fatura.");

                for (int i = 0; i < codigoCargas.Count; i++)
                {
                    if (!repFaturaCarga.ContemCargaoFatura(codigoFatura, codigoCargas[i]))
                    {
                        Dominio.Entidades.Embarcador.Fatura.FaturaCarga faturaCarga = new Dominio.Entidades.Embarcador.Fatura.FaturaCarga();
                        faturaCarga.Carga = repCarga.BuscarPorCodigo(codigoCargas[i]);
                        faturaCarga.Fatura = fatura;
                        faturaCarga.StatusFaturaCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.Faturada;

                        repFaturaCarga.Inserir(faturaCarga, Auditado);
                    }
                    servFatura.AtualizaStatusFaturaCarga(codigoCargas[i], codigoFatura, unitOfWork);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Salvou os conhecimentos das cargas da fatura.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao lançar novo conhecimento a fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarCargasFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteSalvarCarga)))
                    return new JsonpResult(false, "Seu usuário não possui permissão salvar as cargas da fatura.");

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura etapa;
                Enum.TryParse(Request.Params("Etapa"), out etapa);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura;
                if (codigo > 0)
                    fatura = repFatura.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, "Nenhuma fatura encontrada");

                int quantidadeDocumentosFaturados = repFaturaCarga.QuantidadeDocumentosFaturadosCarga(codigo, fatura, 0);
                decimal valorConhecimentos = repFaturaCarga.ValorConhecimentos(codigo);

                if (quantidadeDocumentosFaturados == 0 || valorConhecimentos <= 0)
                    return new JsonpResult(false, "Esta fatura não possui nenhum documento vinculado. Favor verificar.");

                if (quantidadeDocumentosFaturados > 30000)
                    return new JsonpResult(false, "Esta fatura possui mais de 30.000 documentos vinculados. Favor verificar.");

                unitOfWork.Start();

                fatura.Etapa = etapa;
                fatura.ImprimeObservacaoFatura = false;
                fatura.Total = valorConhecimentos;

                List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> listaCargaCTe = repFaturaCarga.ConsultarConhecimento(0, fatura, null, null, 0, 10);
                if (fatura.Empresa == null && listaCargaCTe != null && listaCargaCTe.Count > 0)
                    fatura.Empresa = listaCargaCTe[0].ConhecimentoDeTransporteEletronico.Empresa;

                if (ConfiguracaoEmbarcador.UtilizarDadosBancariosDaEmpresa && fatura.Empresa != null && fatura.Empresa.Banco != null)
                {
                    fatura.Banco = fatura.Empresa.Banco;
                    fatura.Agencia = fatura.Empresa.Agencia;
                    fatura.DigitoAgencia = fatura.Empresa.DigitoAgencia;
                    fatura.NumeroConta = fatura.Empresa.NumeroConta;
                    fatura.TipoContaBanco = fatura.Empresa.TipoContaBanco;
                }

                if (fatura.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
                {
                    if (fatura.Banco == null)
                    {
                        fatura.Banco = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.Banco;
                        fatura.Agencia = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.Agencia;
                        fatura.DigitoAgencia = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DigitoAgencia;
                        fatura.NumeroConta = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.NumeroConta;
                        fatura.TipoContaBanco = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.TipoContaBanco;
                    }
                    fatura.ClienteTomadorFatura = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.ClienteTomadorFatura;
                    fatura.ObservacaoFatura = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.ObservacaoFatura;
                }
                else if (fatura.Cliente != null && !fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPrimeiroGrupoCliente(fatura.Cliente);
                    if (grupo != null)
                    {
                        if (fatura.Banco == null)
                        {
                            fatura.Banco = grupo.Banco;
                            fatura.Agencia = grupo.Agencia;
                            fatura.DigitoAgencia = grupo.DigitoAgencia;
                            fatura.NumeroConta = grupo.NumeroConta;
                            fatura.TipoContaBanco = grupo.TipoContaBanco;
                        }
                        fatura.ClienteTomadorFatura = grupo.ClienteTomadorFatura;
                        fatura.ObservacaoFatura = grupo.ObservacaoFatura;
                    }
                    else
                    {
                        if (fatura.Banco == null)
                        {
                            fatura.Banco = fatura.Cliente.Banco;
                            fatura.Agencia = fatura.Cliente.Agencia;
                            fatura.DigitoAgencia = fatura.Cliente.DigitoAgencia;
                            fatura.NumeroConta = fatura.Cliente.NumeroConta;
                            fatura.TipoContaBanco = fatura.Cliente.TipoContaBanco;
                        }
                        fatura.ClienteTomadorFatura = fatura.Cliente.ClienteTomadorFatura;
                        fatura.ObservacaoFatura = fatura.Cliente.ObservacaoFatura;
                    }
                }
                else if (fatura.Cliente != null)
                {
                    if (fatura.Banco == null)
                    {
                        fatura.Banco = fatura.Cliente.Banco;
                        fatura.Agencia = fatura.Cliente.Agencia;
                        fatura.DigitoAgencia = fatura.Cliente.DigitoAgencia;
                        fatura.NumeroConta = fatura.Cliente.NumeroConta;
                        fatura.TipoContaBanco = fatura.Cliente.TipoContaBanco;
                    }
                    fatura.ClienteTomadorFatura = fatura.Cliente.ClienteTomadorFatura;
                    fatura.ObservacaoFatura = fatura.Cliente.ObservacaoFatura;
                }
                else if (fatura.GrupoPessoas != null)
                {
                    if (fatura.Banco == null)
                    {
                        fatura.Banco = fatura.GrupoPessoas.Banco;
                        fatura.Agencia = fatura.GrupoPessoas.Agencia;
                        fatura.DigitoAgencia = fatura.GrupoPessoas.DigitoAgencia;
                        fatura.NumeroConta = fatura.GrupoPessoas.NumeroConta;
                        fatura.TipoContaBanco = fatura.GrupoPessoas.TipoContaBanco;
                    }
                    fatura.ClienteTomadorFatura = fatura.GrupoPessoas.ClienteTomadorFatura;
                    fatura.ObservacaoFatura = fatura.GrupoPessoas.ObservacaoFatura;
                }

                if (!string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                    fatura.ImprimeObservacaoFatura = true;

                if (fatura.ClienteTomadorFatura == null && listaCargaCTe != null && listaCargaCTe.Count > 0)
                {
                    string cnpjTomador = "";
                    if (listaCargaCTe[0].ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        cnpjTomador = listaCargaCTe[0].ConhecimentoDeTransporteEletronico.Destinatario.CPF_CNPJ;
                    else if (listaCargaCTe[0].ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                        cnpjTomador = listaCargaCTe[0].ConhecimentoDeTransporteEletronico.Expedidor.CPF_CNPJ;
                    else if (listaCargaCTe[0].ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                        cnpjTomador = listaCargaCTe[0].ConhecimentoDeTransporteEletronico.OutrosTomador.CPF_CNPJ;
                    else if (listaCargaCTe[0].ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                        cnpjTomador = listaCargaCTe[0].ConhecimentoDeTransporteEletronico.Recebedor.CPF_CNPJ;
                    else if (listaCargaCTe[0].ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        cnpjTomador = listaCargaCTe[0].ConhecimentoDeTransporteEletronico.Remetente.CPF_CNPJ;
                    double cnpjPessoa = 0;
                    double.TryParse(cnpjTomador, out cnpjPessoa);
                    if (cnpjPessoa > 0)
                        fatura.ClienteTomadorFatura = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                }

                repFatura.Atualizar(fatura, Auditado);

                servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.SalvouCargas, this.Usuario);
                //if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                //    servFatura.LancarMoedaEstrangeira(fatura, unitOfWork);
                servFatura.LancarParcelaFatura(fatura, unitOfWork, ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);

                unitOfWork.CommitChanges();

                var dynRetorno = servFatura.RetornaObjetoCompletoFatura(fatura.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaConhecimentosFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int numeroCTe, codigoFatura = 0;
                int.TryParse(Request.Params("Descricao"), out numeroCTe);
                int.TryParse(Request.Params("Fatura"), out codigoFatura);

                if (codigoFatura == 0)
                    int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);

                if (codigoFatura == 0)
                    return new JsonpResult(false, "Este título não pertente a nenhuma fatura.");

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Série", "Serie", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Numero")
                    propOrdenar = "CTe.Numero";

                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> listaConhecimentos = repFaturaCarga.ConsultarConhecimento(numeroCTe, fatura, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturaCarga.ContarConsultarConhecimento(numeroCTe, fatura));

                var lista = (from p in listaConhecimentos
                             select new
                             {
                                 p.ConhecimentoDeTransporteEletronico.Codigo,
                                 Numero = p.ConhecimentoDeTransporteEletronico.Numero.ToString("n0"),
                                 Serie = p.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString("n0"),
                                 DataEmissao = p.ConhecimentoDeTransporteEletronico.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 Valor = p.ConhecimentoDeTransporteEletronico.ValorAReceber.ToString("n2")
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

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarPreFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumentoExcluido repFaturaCargaDocumentoExcluido = new Repositorio.Embarcador.Fatura.FaturaCargaDocumentoExcluido(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int codigoFatura = 0;
                int.TryParse(Request.Params("Codigo"), out codigoFatura);

                if (codigoFatura == 0)
                    return new JsonpResult(false, "Favor selecione uma Fatura.");

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                decimal totalFatura = repFaturaCarga.ValorConhecimentos(codigoFatura);

                if (totalFatura == 0)
                    return new JsonpResult(false, "Para importar a pré-fatura é necessário que a mesma contenha os conhecimentos lançados.");

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                DataSet ds = new DataSet();
                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
                    string msgRetorno = "";
                    List<dynamic> dadosRetorno = new List<dynamic>();

                    List<Dominio.ObjetosDeValor.Embarcador.Fatura.CTePreFatura> listaCTePreFatura = null;

                    if (fileExtension.ToLower() == ".xls" || fileExtension.ToLower() == ".xlsx")
                    {
                        ExcelPackage package = new ExcelPackage(file.InputStream);
                        Servicos.Embarcador.Fatura.PreFatura svcPreFatura = new Servicos.Embarcador.Fatura.PreFatura(unitOfWork);
                        Dominio.ObjetosDeValor.Embarcador.Fatura.PreFatura retornoPreFatura = svcPreFatura.ProcessarArquivoPreFatura(package, unitOfWork);
                        listaCTePreFatura = retornoPreFatura.CTePreFatura;
                        msgRetorno = retornoPreFatura.MsgRetorno;
                    }

                    List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> listaConhecimentos = repFaturaCarga.ConsultarConhecimento(0, fatura, null, null, 1, 5000);
                    List<int> codigosCargas = new List<int>();
                    bool cteLocalizado = false;
                    if (listaCTePreFatura != null && listaCTePreFatura.Count() > 0)
                    {
                        unitOfWork.Start();
                        int totalConhecimentos = listaConhecimentos.Count;

                        for (int i = 0; i < listaConhecimentos.Count; i++)
                        {
                            //if (!repFaturaCargaDocumentoExcluido.CTeExcluidoEmFatura(listaConhecimentos[i].ConhecimentoDeTransporteEletronico.Codigo, codigoFatura))
                            if (listaConhecimentos[i].StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal)
                            {
                                cteLocalizado = false;
                                for (int j = 0; j < listaCTePreFatura.Count; j++)
                                {
                                    if (listaCTePreFatura[j].NumeroCTe == listaConhecimentos[i].ConhecimentoDeTransporteEletronico.Numero && listaCTePreFatura[j].ValorCTe == listaConhecimentos[i].ConhecimentoDeTransporteEletronico.ValorAReceber)
                                    {
                                        cteLocalizado = true;
                                        listaCTePreFatura.Remove(listaCTePreFatura[j]);
                                        break;
                                    }
                                }
                                if (!cteLocalizado)
                                {
                                    Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento documentoFatura = repFaturaCargaDocumento.BuscarPorFatura(codigoFatura, listaConhecimentos[i].ConhecimentoDeTransporteEletronico.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento);
                                    if (documentoFatura != null)
                                    {
                                        documentoFatura.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido;
                                        repFaturaCargaDocumento.Atualizar(documentoFatura);
                                    }
                                    else
                                    {
                                        documentoFatura.Carga = repCarga.BuscarPorCodigo(listaConhecimentos[i].Carga.Codigo);
                                        documentoFatura.ConhecimentoDeTransporteEletronico = repCTE.BuscarPorCodigo(listaConhecimentos[i].ConhecimentoDeTransporteEletronico.Codigo);
                                        documentoFatura.Fatura = repFatura.BuscarPorCodigo(codigoFatura);
                                        documentoFatura.StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido;
                                        documentoFatura.TipoDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento;

                                        repFaturaCargaDocumento.Inserir(documentoFatura);
                                    }

                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTE.BuscarPorCodigo(listaConhecimentos[i].ConhecimentoDeTransporteEletronico.Codigo);
                                    cte.Fatura = null;
                                    repCTE.Atualizar(cte);

                                    //servFatura.AtualizaStatusFaturaCarga(listaConhecimentos[i].Carga.Codigo, codigoFatura, unitOfWork);
                                    if (!codigosCargas.Contains(listaConhecimentos[i].Carga.Codigo))
                                        codigosCargas.Add(listaConhecimentos[i].Carga.Codigo);
                                }
                            }

                            int processados = (int)(100 * i) / totalConhecimentos;
                            serNotificacao.InfomarPercentualProcessamento(this.Usuario, fatura.Codigo, "Faturas/Fatura", (decimal)processados, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.fatura, TipoServicoMultisoftware, unitOfWork);
                        }
                        if (codigosCargas.Count > 0)
                        {
                            for (int a = 0; a < codigosCargas.Count; a++)
                            {
                                servFatura.AtualizaStatusFaturaCarga(codigosCargas[a], codigoFatura, unitOfWork);
                            }
                        }
                        if (listaCTePreFatura.Count > 0)
                        {
                            for (int i = 0; i < listaCTePreFatura.Count; i++)
                            {
                                msgRetorno = msgRetorno + "CTe Nº " + listaCTePreFatura[i].NumeroCTe.ToString() + " de valor R$ " + listaCTePreFatura[i].ValorCTe.ToString("n2") + " está no arquivo de importação mas não foi localizado nas cargas consultadas.<br/>";
                                var retornoCTe = new
                                {
                                    NumeroCTe = listaCTePreFatura[i].NumeroCTe,
                                    ValorCTe = listaCTePreFatura[i].ValorCTe,
                                    Motivo = "Está no arquivo de importação mas não foi localizado nas cargas consultadas"
                                };
                                dadosRetorno.Add(retornoCTe);
                            }
                        }

                        unitOfWork.CommitChanges();
                        Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                        Dominio.Entidades.Usuario user = repUsuario.BuscarPorCodigo(this.Usuario.Codigo);
                        serNotificacao.GerarNotificacao(user, fatura.Codigo, "Faturas/Fatura", Localization.Resources.Configuracao.Fatura.ImportacaoPreFaturaConcluida, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.fatura, TipoServicoMultisoftware, unitOfWork);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(msgRetorno))
                            return new JsonpResult(dadosRetorno, false, "Arquivo selecionado não está de acordo com o layout selecionado!<br/>" + msgRetorno);
                        else
                            return new JsonpResult(dadosRetorno, false, "Arquivo selecionado não está de acordo com o layout selecionado!");
                    }

                    if (!string.IsNullOrWhiteSpace(msgRetorno))
                        return new JsonpResult(dadosRetorno, true, "Importação da pré-fatura foi realizada com sucesso.<br/>Contudo existem alguns conhecimentos não localizados:<br/>" + msgRetorno);
                    else
                        return new JsonpResult(dadosRetorno, true, "Importação da pré-fatura foi realizada com sucesso.");

                }
                else
                {
                    return new JsonpResult(null, false, "Arquivo não encontrado, por favor verifique!");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar a pré-fatura. Erro: " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteXML()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeTrabalho);

                int codigoFatura = 0;
                int.TryParse(Request.Params("Codigo"), out codigoFatura);

                List<int> listaCodigosCTes = repFaturaDocumento.BuscarListaCodigosCTes(codigoFatura);

                //if (listaCodigosCTes.Count > 500)
                //    return new JsonpResult(false, true, "Quantidade de CT-es para geração de lote inválida (" + listaCodigosCTes.Count + "). É permitido o download de um lote com o máximo de 500 CT-es.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                return Arquivo(svcCTe.ObterLoteDeXML(listaCodigosCTes, 0, unidadeTrabalho), "application/zip", "LoteXML.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BaixarRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R035_DivergenciaPreFatura, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R035_DivergenciaPreFatura, TipoServicoMultisoftware, "Relatorio Diverências da Pré-Fatura", "Fatura", "DivergenciaPreFatura.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                int codigo = int.Parse(Request.Params("Codigo"));

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.XLS, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;
                List<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioDivergenciaPreFatura> dadosPreFatura = DadosPreFatura(unitOfWork);
                if (dadosPreFatura.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioDivergenciasPreFatura(codigo, nomeCliente, stringConexao, relatorioControleGeracao, dadosPreFatura));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de divergencias para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioDivergenciaPreFatura> DadosPreFatura(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioDivergenciaPreFatura> retorno = new List<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioDivergenciaPreFatura>();
            dynamic listaDados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Dados"));
            if (listaDados != null)
            {
                foreach (var dadosCTe in listaDados)
                {
                    Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioDivergenciaPreFatura dado = new Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioDivergenciaPreFatura();
                    dado.NumeroCTe = (int)dadosCTe.NumeroCTe;
                    dado.ValorCTe = (decimal)dadosCTe.ValorCTe;
                    dado.Motivo = (string)dadosCTe.Motivo;
                    retorno.Add(dado);
                }
            }
            return retorno;
        }

        private void GerarRelatorioDivergenciasPreFatura(int codigoFatura, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, List<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioDivergenciaPreFatura> dadosPreFatura)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                var result = ReportRequest.WithType(ReportType.DivergenciaPreFatura)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("CodigoFatura", codigoFatura)
                    .AddExtraData("NomeEmpresa", nomeEmpresa)
                    .AddExtraData("DadosPreFatura", dadosPreFatura.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
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
    }
}
