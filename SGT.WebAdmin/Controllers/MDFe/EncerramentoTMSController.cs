using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.MDFe
{
    [CustomAuthorize("MDFe/EncerramentoTMS")]
    public class EncerramentoTMSController : BaseController
    {
		#region Construtores

		public EncerramentoTMSController(Conexao conexao) : base(conexao) { }

		#endregion

        private Models.Grid.Grid ObterGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Série", "Serie", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Origem", "LocalidadeOrigem", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Destino", "LocalidadeDestino", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaEncerramentoMDFeTMS ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaEncerramentoMDFeTMS()
            {
                Placa = Request.GetStringParam("Placa")
            };
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaEncerramentoMDFeTMS filtrosPesquisa = ObterFiltrosPesquisa();
                Models.Grid.Grid grid = ObterGrid();

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                int rowCount = repMDFe.ContarConsultaMDFesEncerramentoTMS(filtrosPesquisa);

                IList<Dominio.ObjetosDeValor.Embarcador.MDFe.CamposPesquisaEncerramentoMDFeTMS> mdfes = new List<Dominio.ObjetosDeValor.Embarcador.MDFe.CamposPesquisaEncerramentoMDFeTMS>();

                if (rowCount > 0)
                    mdfes = repMDFe.ConsultarMDFesEncerramentoTMS(filtrosPesquisa, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(rowCount);

                grid.AdicionaRows(mdfes);

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

        public async Task<IActionResult> Encerrar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, true, "MDF-e não encontrado.");

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                {
                    Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);

                    if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, DateTime.Now, unidadeTrabalho))
                    {
                        mdfe.Log = "Encerrado manualmente pelo usuário " + this.Usuario.Nome + " às " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                        repMDFe.Atualizar(mdfe);

                        svcMDFe.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, DateTime.Now, mdfe.Empresa, mdfe.Empresa.Localidade, mdfe.Log, unidadeTrabalho);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Encerramento manual solicitado.", unidadeTrabalho);
                    }

                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Não é possível encerrar um MDF-e na situação " + mdfe.DescricaoStatus + ".");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDAMDFE()
        {
            try
            {
                Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado/cancelado/encerrado para o download do DAMDFE.");

                Servicos.DAMDFE svcDAMDFE = new Servicos.DAMDFE(unidadeDeTrabalho);

                byte[] arquivo = null;

                if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                {
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";
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
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                Dominio.Enumeradores.TipoXMLMDFe tipo;
                Enum.TryParse(Request.Params("Tipo"), out tipo);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (tipo == Dominio.Enumeradores.TipoXMLMDFe.Autorizacao &&
                    (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado))
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado/encerrado/cancelado para o download do XML.");

                if (tipo == Dominio.Enumeradores.TipoXMLMDFe.Cancelamento && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado)
                    return new JsonpResult(false, false, "O MDF-e deve estar cancelado para o download do XML.");

                if (tipo == Dominio.Enumeradores.TipoXMLMDFe.Encerramento && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar encerrado para o download do XML.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXML(mdfe, tipo, unitOfWork);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, ".xml"));
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
                unitOfWork.Dispose();
            }
        }
    }
}
