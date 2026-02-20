using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/GeracaoTituloManual")]
    public class GeracaoTituloManualController : BaseController
    {
		#region Construtores

		public GeracaoTituloManualController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> PesquisaDocumentosPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime.TryParseExact(Request.Params("DataEmissaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params("DataEmissaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out double cpfCnpjPessoa);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Remetente")), out double cpfCnpjRemetente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out double cpfCnpjDestinatario);

                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
                int.TryParse(Request.Params("NumeroInicial"), out int numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out int numeroFinal);
                int.TryParse(Request.Params("Serie"), out int serie);
                int.TryParse(Request.Params("Origem"), out int codigoOrigem);
                int.TryParse(Request.Params("Destino"), out int codigoDestino);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);
                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
                int.TryParse(Request.Params("Motorista"), out int codigoMotorista);

                string numeroCarga = Request.Params("NumeroCarga");
                string numeroPedido = Request.Params("NumeroPedido");
                string numeroOcorrencia = Request.Params("NumeroOcorrencia");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Série", "Serie", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "NumeroCarga", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 17, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 17, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Documento", "ValorDocumento", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Faturar", "ValorAFaturar", 8, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Serie")
                    propOrdenar = "EmpresaSerie.Numero";
                else if (propOrdenar == "Tomador")
                    propOrdenar = "Tomador.Nome";
                else if (propOrdenar == "Origem")
                    propOrdenar = "Origem.Descricao";
                else if (propOrdenar == "Destino")
                    propOrdenar = "Destino.Descricao";

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentos = repDocumentoFaturamento.ConsultarDocumentosParaFaturar(codigoEmpresa, codigoVeiculo, codigoMotorista, serie, dataEmissaoInicial, dataEmissaoFinal, numeroCarga, numeroInicial, numeroFinal, codigoGrupoPessoas, cpfCnpjPessoa, cpfCnpjRemetente, cpfCnpjDestinatario, codigoOrigem, codigoDestino, numeroPedido, numeroOcorrencia, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repDocumentoFaturamento.ContarConsultaDocumentosParaFaturar(codigoEmpresa, codigoVeiculo, codigoMotorista, serie, dataEmissaoInicial, dataEmissaoFinal, numeroCarga, numeroInicial, numeroFinal, codigoGrupoPessoas, cpfCnpjPessoa, cpfCnpjRemetente, cpfCnpjDestinatario, codigoOrigem, codigoDestino, numeroPedido, numeroOcorrencia));

                var lista = (from obj in documentos
                             select new
                             {
                                 obj.Codigo,
                                 obj.Numero,
                                 Serie = obj.EmpresaSerie?.Numero.ToString() ?? string.Empty,
                                 obj.NumeroCarga,
                                 DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                 Tomador = obj.Tomador != null ? obj.Tomador.CPF_CNPJ_Formatado + " - " + obj.Tomador.Nome : string.Empty,
                                 Origem = obj.Origem?.DescricaoCidadeEstado ?? string.Empty,
                                 Destino = obj.Destino.DescricaoCidadeEstado ?? string.Empty,
                                 obj.ValorDocumento,
                                 obj.ValorAFaturar
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

        public async Task<IActionResult> GerarTitulos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime.TryParseExact(Request.Params("DataEmissaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params("DataEmissaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out double cpfCnpjPessoa);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Remetente")), out double cpfCnpjRemetente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out double cpfCnpjDestinatario);

                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
                int.TryParse(Request.Params("NumeroInicial"), out int numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out int numeroFinal);
                int.TryParse(Request.Params("Serie"), out int serie);
                int.TryParse(Request.Params("Origem"), out int codigoOrigem);
                int.TryParse(Request.Params("Destino"), out int codigoDestino);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);
                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
                int.TryParse(Request.Params("Motorista"), out int codigoMotorista);

                string numeroCarga = Request.Params("NumeroCarga");
                string numeroPedido = Request.Params("NumeroPedido");
                string numeroOcorrencia = Request.Params("NumeroOcorrencia");

                bool.TryParse(Request.Params("SelecionarTodos"), out bool selecionarTodos);

                List<int> codigosDocumentosFiltrar = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaDocumentos"));

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFinanceiraFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(unitOfWork);

                Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);

                List<int> codigosDocumentos = repDocumentoFaturamento.ObterCodigosDocumentosParaFaturar(codigoEmpresa, codigoVeiculo, codigoMotorista, serie, dataEmissaoInicial, dataEmissaoFinal, numeroCarga, numeroInicial, numeroFinal, codigoGrupoPessoas, cpfCnpjPessoa, cpfCnpjRemetente, cpfCnpjDestinatario, codigoOrigem, codigoDestino, numeroPedido, numeroOcorrencia, selecionarTodos, codigosDocumentosFiltrar);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura = repConfiguracaoFinanceiraFatura.BuscarPrimeiroRegistro();

                foreach (int codigoDocumento in codigosDocumentos)
                {
                    unitOfWork.Start();

                    Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                    if (!servicoTitulo.GerarTituloPorDocumentoFaturamento(out string erro, codigoDocumento, configuracaoFinanceiraFatura, Auditado, TipoServicoMultisoftware, tipoAmbiente, this.Usuario))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar os títulos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
