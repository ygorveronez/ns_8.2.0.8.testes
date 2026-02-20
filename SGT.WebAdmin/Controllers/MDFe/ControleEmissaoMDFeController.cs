using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.MDFe
{
    [CustomAuthorize("MDFe/ControleEmissaoMDFe")]
    public class ControleEmissaoMDFeController : BaseController
    {
		#region Construtores

		public ControleEmissaoMDFeController(Conexao conexao) : base(conexao) { }

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

        public async Task<IActionResult> Reemitir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
                List<string> listaErros = new List<string>();

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigo, true);

                // Valida
                if (mdfe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Inicia transacao
                if (!EmitirMDFe(mdfe, unitOfWork, out string msg))
                    return new JsonpResult(false, true, msg);

                // Retorna sucesso
                return new JsonpResult(true);
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDAMDFE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("MDFe"), out int codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado para o download do DAMDFE.");

                Servicos.DAMDFE svcDAMDFE = new Servicos.DAMDFE(unidadeTrabalho);

                byte[] arquivo = null;

                if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                {
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }

                if (arquivo == null)
                    arquivo = svcDAMDFE.Gerar(mdfe.Codigo);

                if (arquivo != null)
                    return Arquivo(arquivo, "application/pdf", string.Concat(mdfe.Chave, ".pdf"));
                else
                    return new JsonpResult(false, false, "Não foi possível gerar o DAMDFE, atualize a página e tente novamente.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do DAMDFE.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("MDFe"), out int codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado para realizar o download do XML de autorização.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXMLAutorizacao(mdfe, unidadeTrabalho);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, ".xml"));
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML de autorização.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unitOfWork);

            // Reutilizado o mesmo metodo do consultar, so foi mantido as variaveis para não ficar confuso
            // Mesmo que elas estejam apenas inicializadas
            int serie = 0, numeroInicial = 0, numeroFinal = 0;
            int[] series = new int[] { };
            int numeroCTe = 0;

            string ufCarregamento = string.Empty;
            string ufDescarregamento = string.Empty;
            string placaVeiculo = string.Empty;
            string nomeMotorista = string.Empty;
            string cpfMotorista = string.Empty;
            string nomeUsuario = string.Empty;

            // Filtros
            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);

            Dominio.Enumeradores.StatusMDFe? status = null;
            if (Enum.TryParse<Dominio.Enumeradores.StatusMDFe>(Request.Params("Status"), out Dominio.Enumeradores.StatusMDFe statusAux))
                status = statusAux;

            int.TryParse(Request.Params("Empresa"), out int empresa);
            string numeroCarga = Request.Params("NumeroCarga");

            List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaGrid = repMDFe.Consultar(empresa, 0, this.Empresa.TipoAmbiente, serie, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, string.Empty, cpfMotorista, nomeMotorista, series, numeroCTe, inicio, 50, nomeUsuario, numeroCarga, "", /*this.Empresa.Codigo*/0);
            totalRegistros = repMDFe.ContarConsulta(empresa, 0, this.Empresa.TipoAmbiente, serie, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, string.Empty, cpfMotorista, nomeMotorista, series, numeroCTe, nomeUsuario, numeroCarga, "", /*this.Empresa.Codigo*/0);
            List<Dominio.Entidades.VeiculoMDFe> veiculos = repVeiculoMDFe.BuscarPorMDFes((from obj in listaGrid select obj.Codigo).ToArray());

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Status,
                            obj.Numero,
                            Serie = obj.Serie.Numero,
                            Empresa = obj.Empresa.RazaoSocial,
                            DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                            Placa = (from veic in veiculos where veic.MDFe.Codigo == obj.Codigo select veic.Placa).FirstOrDefault(),
                            EstadoCarregamento = string.Concat(obj.EstadoCarregamento.Sigla, " - ", obj.EstadoCarregamento.Nome),
                            EstadoDescarregamento = string.Concat(obj.EstadoDescarregamento.Sigla, " - ", obj.EstadoDescarregamento.Nome),
                            obj.DescricaoStatus,
                            MensagemSefaz = (obj.MensagemStatus == null ? (obj.MensagemRetornoSefaz != null ? System.Web.HttpUtility.HtmlEncode(obj.MensagemRetornoSefaz) : string.Empty) : obj.MensagemStatus.MensagemDoErro)
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "Empresa") propOrdenar = "Empresa.RazaoSocial";
            else if (propOrdenar == "EstadoCarregamento") propOrdenar = "EstadoCarregamento.Nome";
            else if (propOrdenar == "EstadoDescarregamento") propOrdenar = "EstadoDescarregamento.Nome";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Status", false);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Série", "Serie", 6, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Empresa", "Empresa", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Placa", "Placa", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("UF Carga", "EstadoCarregamento", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("UF Descarga", "EstadoDescarregamento", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno Sefaz", "MensagemSefaz", 18, Models.Grid.Align.left, true);

            return grid;

        }

        private bool EmitirMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork, out string msg)
        {
            msg = "";

            unitOfWork.Start();

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

            if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)
            {
                if (mdfe.DataEmissao < DateTime.Now.AddDays(-1))
                {
                    TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                    mdfe.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
                }

                mdfe.Status = Dominio.Enumeradores.StatusMDFe.Pendente;

                repMDFe.Atualizar(mdfe);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Solicitou Emissão do MDF-e", unitOfWork);

                unitOfWork.CommitChanges();

                svcMDFe.AtualizarIntegracaoRetornoMDFe(mdfe, unitOfWork);

                if (svcMDFe.Emitir(mdfe, unitOfWork))
                {
                    svcMDFe.AdicionarMDFeNaFilaDeConsulta(mdfe, unitOfWork);
                    svcMDFe.RemoverPendenciaMDFeCarga(mdfe, Auditado, unitOfWork);
                }
                else
                {
                    unitOfWork.Rollback();
                    msg = "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + mdfe.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                    return false;
                }
            }
            else
            {
                unitOfWork.Rollback();
                msg = "A atual situação do MDF-e (" + mdfe.DescricaoStatus + ") não permite sua emissão.";
                return false;
            }

            return true;

        }
        #endregion
    }
}
