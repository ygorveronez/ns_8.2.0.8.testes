using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ControleCTe
{
    [CustomAuthorize("ControleCTe/ControleEmissaoCTe")]
    public class ControleEmissaoCTeController : BaseController
    {
		#region Construtores

		public ControleEmissaoCTeController(Conexao conexao) : base(conexao) { }

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
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo, true);

                // Valida
                if (cte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                if (cte.Empresa.Status != "A")
                    return new JsonpResult(false, true, "Empresa não está ativa para emissão de CT-e.");

                if (cte.Empresa.StatusFinanceiro == "B")
                    return new JsonpResult(false, true, "Empresa está com pendências, contate o setor de cadastros para maiores informações.");

                // Inicia transacao
                unitOfWork.Start();
                string retorno = EmitirCTe(cte, unitOfWork);
                unitOfWork.CommitChanges();

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

        public async Task<IActionResult> RemoverConsulta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.RetornoXMLCTe repRetornoXMLCTe = new Repositorio.RetornoXMLCTe(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo, true);

                // Valida
                if (cte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cte.Status != "A" && cte.Status != "C")
                    return new JsonpResult(false, true, "Status do CTe não permite remover consulta.");

                List<Dominio.Entidades.RetornoXMLCTe> retornosCTe = repRetornoXMLCTe.BuscarPorCodigoCTe(cte.Codigo);

                if (retornosCTe == null || retornosCTe.Count() == 0)
                    return new JsonpResult(true);

                foreach (Dominio.Entidades.RetornoXMLCTe retornoCTe in retornosCTe)
                {
                    repRetornoXMLCTe.Deletar(retornoCTe);
                }

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
        public async Task<IActionResult> DownloadDACTE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("CTe"), out int codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(true, false, "CT-e não encontrado, atualize a página e tente novamente.");

                if (cte.Status != "A" && cte.Status != "C" && cte.Status != "K")
                    return new JsonpResult(true, false, "O status do CT-e não permite a geração do DACTE.");

                if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download do DACTE não está disponível. Contate o suporte técnico.");

                byte[] pdf = null;
                string nomeArquivo = "";

                if (cte.ModeloDocumentoFiscal.Numero == "39")
                {
                    nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".pdf";

                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeTrabalho);

                    pdf = svcNFSe.ObterDANFSECTe(cte.Codigo);
                }
                else
                {
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, cte.Chave) + ".pdf";
                    nomeArquivo = System.IO.Path.GetFileName(caminhoPDF);

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    {
                        if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoGeradorRelatorios))
                            return new JsonpResult(true, false, "O gerador do PDF não está disponível. Contate o suporte técnico.");

                        Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);

                        pdf = svcDACTE.GerarPorProcesso(cte.Codigo, unidadeTrabalho);
                    }
                    else
                    {
                        pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                    }
                }

                if (pdf != null)
                    return Arquivo(pdf, "application/pdf", nomeArquivo);
                else
                    return new JsonpResult(false, false, "Não foi possível gerar o PDF, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do PDF.");
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
                int.TryParse(Request.Params("CTe"), out int codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(false, false, "CT-e não encontrado, atualize a página e tente novamente.");

                string nomeArquivo = "";
                byte[] data = null;

                if (cte.ModeloDocumentoFiscal.Numero == "39")
                {
                    nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".xml";

                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeTrabalho);

                    data = svcNFSe.ObterXMLAutorizacaoCTe(cte.Codigo);
                }
                else
                {
                    nomeArquivo = string.Concat(cte.Chave, ".xml");
                    Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                    data = svcCTe.ObterXMLAutorizacao(cte);
                }


                if (data != null)
                    return Arquivo(data, "text/xml", nomeArquivo);
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
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
        #endregion

        #region Métodos Privados
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.RetornoXMLCTe repRetornoXMLCTe = new Repositorio.RetornoXMLCTe(unitOfWork);

            // Reutilizado o mesmo metodo do consultar, so foi mantido as variaveis para não ficar confuso
            // Mesmo que elas estejam apenas inicializadas
            int serie = 0, numeroInicial = 0, numeroFinal = 0;
            int[] series = new int[] { };

            bool contem = false;
            string cpfCnpjRemetente = string.Empty;
            string cpfCnpjDestinatario = string.Empty;
            string placa = string.Empty;
            string motorista = string.Empty;
            string tipoOcorrencia = string.Empty;
            string numeroNF = string.Empty;

            Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;

            // Filtros
            int.TryParse(Request.Params("Empresa"), out int empresa);
            int.TryParse(Request.Params("NumeroCarga"), out int numeroCarga);

            string status = Request.Params("Status") ?? string.Empty;

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);

            numeroInicial = Request.GetIntParam("NumeroInicial");
            numeroFinal = Request.GetIntParam("NumeroFinal");
            serie = Request.GetIntParam("Serie");

            Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe = null;

            unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

            List<Dominio.ObjetosDeValor.ConsultaCTe> listaGrid = repCTe.ConsultarEmissaoCTe(empresa, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placa, motorista, cpfCnpjRemetente, cpfCnpjDestinatario, status, tipoCTe, this.Empresa.TipoAmbiente, series, serie, tipoOcorrencia, numeroNF, contem, averbacaoCTe, inicio, limite, /*this.Empresa.Codigo*/0, numeroCarga);
            totalRegistros = repCTe.ContarConsultaEmissaoCTe(empresa, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placa, motorista, cpfCnpjRemetente, cpfCnpjDestinatario, status, tipoCTe, this.Empresa.TipoAmbiente, series, serie, tipoOcorrencia, numeroNF, contem, averbacaoCTe, /*this.Empresa.Codigo*/0, numeroCarga);

            unitOfWork.CommitChanges();

            bool exibirSituacaoIntegracaoXML = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().ExibirSituacaoIntegracaoXMLGPA.Value;

            var lista = from cte in listaGrid
                        select new
                        {
                            //CodigoCriptografado = Servicos.Criptografia.Criptografar(cte.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                            cte.Codigo,
                            cte.Numero,
                            cte.Serie,
                            Empresa = cte.NomeEmpresa,
                            DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                            TipoServico = cte.DescricaoTipoServico,
                            Placa = cte.Placa ?? string.Empty,
                            Remetente = cte.Remetente != null ? cte.Remetente.Nome : string.Empty,
                            LocalidadeRemetente = cte.Remetente != null ? cte.Remetente.Exterior ? string.Concat(cte.Remetente.Cidade, " / ", cte.Remetente.Pais != null ? cte.Remetente.Pais.Nome : "EXPORTACAO") : string.Concat(cte.Remetente.Localidade.Estado.Sigla, " / ", cte.Remetente.Localidade.Descricao) : string.Empty,
                            Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome : string.Empty,
                            LocalidadeDestinatario = cte.Destinatario != null ? cte.Destinatario.Exterior ? string.Concat(cte.Destinatario.Cidade, " / ", cte.Destinatario.Pais != null ? cte.Destinatario.Pais.Nome : "EXPORTACAO") : string.Concat(cte.Destinatario.Localidade.Estado.Sigla, " / ", cte.Destinatario.Localidade.Descricao) : cte.TerminoPrestacao,
                            Valor = string.Format("{0:n2}", cte.Valor),
                            cte.Status,
                            cte.DescricaoStatus,
                            MensagemRetornoSefaz = cte.Status == "A" && exibirSituacaoIntegracaoXML ? (repRetornoXMLCTe.ExisteRegistroCTePorStatus(cte.Codigo, "A") ? "XML consutado por integração" : "XML pendente consulta por integração") : cte.Status == "E" ? "CT-e em processamento." : cte.MensagemStatus == null ? string.IsNullOrEmpty(cte.MensagemRetornoSefaz) ? string.Empty : System.Web.HttpUtility.HtmlEncode(cte.MensagemRetornoSefaz) : cte.MensagemStatus.MensagemDoErro
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
            else if (propOrdenar == "Remetente") propOrdenar = "Remetente.Nome";
            else if (propOrdenar == "LocalidadeRemetente") propOrdenar = "Remetente.Nome.Cidade";
            else if (propOrdenar == "Destinatario") propOrdenar = "Destinatario.Nome";
            else if (propOrdenar == "LocalidadeDestinatario") propOrdenar = "Destinatario.Nome.Cidade";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Status", false);
            grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Empresa", "Empresa", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo", "TipoServico", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Placa", "Placa", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", 11, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Loc. Remet", "LocalidadeRemetente", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 11, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Loc. Destin", "LocalidadeDestinatario", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor", "Valor", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Mensagem Situação", "MensagemRetornoSefaz", 11, Models.Grid.Align.left, true);
            return grid;

        }

        private string EmitirCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            if (cte != null)
            {
                if (cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao)
                {

                    cte.Status = "P";
                    repCTe.Atualizar(cte);

                    if (svcCTe.Emitir(cte.Codigo, cte.Empresa.Codigo))
                        svcCTe.AdicionarCTeNaFilaDeConsulta(cte, unitOfWork);
                    else
                        mensagem = "O CT-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                }
                else
                {
                    mensagem = "A atual situação do CT-e (" + cte.DescricaoStatus + ") não permite sua emissão.";
                }
            }
            else
            {
                mensagem = "O CT-e informado não foi localizado";
            }

            return mensagem;
        }
        #endregion
    }
}
