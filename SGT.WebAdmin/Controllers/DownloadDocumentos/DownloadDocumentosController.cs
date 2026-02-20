using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.DownloadDocumentos
{
    [CustomAuthorize("DownloadDocumentos/DownloadDocumentos")]
    public class DownloadDocumentosController : BaseController
    {
		#region Construtores

		public DownloadDocumentosController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAnonymous]
        public async Task<IActionResult> Validacao(string token)
        {
            string caminhoBaseViews = "~/Views/DownloadDocumentos/";

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    MontaLayoutBase(unitOfWork);
                    DefineParametrosView(token, unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.DownloadDocumentos.DownloadDocumentosView dataView = ObtemDadosRenderizacao(token, unitOfWork);

                    if (dataView == null)
                        return View(caminhoBaseViews + "DownloadDocumentosErro.cshtml");

                    return View(caminhoBaseViews + "DownloadDocumentosDetalhes.cshtml", dataView);
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return View(caminhoBaseViews + "DownloadDocumentosErro.cshtml");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> DownloadLoteXMLMDFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> codigosMdfes = repCargaMDFe.BuscarCodigosMDFePorAutorizadosCarga(codigoCarga);

                if (codigosMdfes.Count == 0)
                    return new JsonpResult(false, true, "Não há MDF-es disponíveis para esta carga.");
                if (codigosMdfes.Count > 1000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 1000 arquivos.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcMDFe.ObterLoteDeXML(codigosMdfes, carga.Empresa?.Codigo ?? 0, unidadeTrabalho);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_XML_MDFE.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de XML.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> DownloadLoteDocumentosMDFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeTrabalho);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga, false, false);

                List<int> mdfes = repCargaMDFe.BuscarCodigosMDFePorAutorizadosCarga(codigoCarga);

                List<int> valePedagios = repCargaValePedagio.BuscarCodigosValePedagiosSemPararPorCarga(codigoCarga);

                if (ctes.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados documentos autorizados para esta carga.");

                if (ctes.Count > 2000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 2000 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcCTe.ObterLotePDFsTMS(codigoCarga, ctes, mdfes, valePedagios, unidadeTrabalho, TipoServicoMultisoftware, Cliente.NomeFantasia);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_Documentos_MDFe.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de documentos.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> DownloadLoteDAMDFE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<string> chavesMdfes = repCargaMDFe.BuscarChavesMDFeAutorizadosPorCarga(codigoCarga);

                if (chavesMdfes.Count == 0)
                    return new JsonpResult(false, true, "Não há MDF-es disponíveis para esta carga.");
                if (chavesMdfes.Count > 1000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 1000 arquivos.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcMDFe.ObterLoteDeDAMDFE(chavesMdfes, carga.Empresa?.Codigo ?? 0, unidadeTrabalho);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_DAMDFE.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote do DAMDFE.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> DownloadLoteDACTE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga, false, false, false);
                List<int> nfses = repCargaCTe.BuscarCodigosNFSePorCarga(codigoCarga);

                if (ctes.Count <= 0 && nfses.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados documentos autorizados para esta carga.");

                if (ctes.Count + nfses.Count > 2000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 2000 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcCTe.ObterLoteDeDACTE(ctes, nfses, unidadeTrabalho);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_DACTE.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de DACTE.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> DownloadLoteDocumentosCTe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga, false, false);

                if (ctes.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados CT-es autorizados para esta carga.");

                int codigoUsuario = this.Usuario.Codigo;
                string stringConexao = _conexao.StringConexao;
                string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos;
                string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                string caminhoArquivosAnexos = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido" });

                Task.Run(() => Zeus.Embarcador.ZeusNFe.Zeus.GerarPDFTodosDocumentos(0, ctes, stringConexao, codigoUsuario, caminhoRelatorios, caminhoArquivos, diretorioDocumentosFiscais, "Cargas/Carga", caminhoArquivosAnexos));

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote dos documentos.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> DownloadLoteXMLCTe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> codigosCTes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga, false, false, false);
                List<int> codigosNFSes = repCargaCTe.BuscarCodigosNFSePorCarga(codigoCarga);
                for (int i = 0; i < codigosNFSes.Count(); i++)
                    codigosCTes.Add(codigosNFSes[i]);

                if (codigosCTes.Count <= 0 && codigosNFSes.Count <= 0)
                    return new JsonpResult(false, true, "Não há CT-es/NFS-es disponíveis para esta carga.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcCTe.ObterLoteDeXML(codigosCTes, 0, unidadeTrabalho);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_XML.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de XML.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private void DefineParametrosView(string token, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();
            string protocolo = (Request.IsHttps ? "https" : "http");
            if (configuracaoAmbiente?.TipoProtocolo != null && configuracaoAmbiente?.TipoProtocolo.ObterProtocolo() != "")
                protocolo = configuracaoAmbiente?.TipoProtocolo.ObterProtocolo();
            ViewBag.HTTPConnection = protocolo;
            ViewBag.Token = token;
        }

        private void MontaLayoutBase(Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.UnitOfWork adminMultisoftwareUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminMultisoftwareUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                adminMultisoftwareUnitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.DownloadDocumentos.DownloadDocumentosView ObtemDadosRenderizacao(string token, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (string.IsNullOrEmpty(token))
                return null;

            string key = Environment.MachineName + "!Mu!tis*ftw@r3#";
            string tokenDescriptografado = Servicos.Criptografia.Descriptografar(token, key, true);
            string[] tokenDescriptografadoDividido = tokenDescriptografado.Split('-');

            if (!((tokenDescriptografadoDividido?.Length ?? 0) > 1))
                return null;

            DateTime dataEnvioLink = tokenDescriptografadoDividido[1].ToDateTime();

            if (dataEnvioLink.DayOfYear != DateTime.Now.DayOfYear)
                return null;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(tokenDescriptografadoDividido[0].ToInt());

            if (carga == null)
                return null;

            ObterDadosRenderizacaoTela(unitOfWork);


            Dominio.ObjetosDeValor.Embarcador.DownloadDocumentos.DownloadDocumentosView dataView = new Dominio.ObjetosDeValor.Embarcador.DownloadDocumentos.DownloadDocumentosView
            {
                CodigoCarga = carga.Codigo,
                Ativo = (dataEnvioLink.Hour - DateTime.Now.Hour) > 3 ? true : false
            };

            return dataView;
        }

        private void ObterDadosRenderizacaoTela(Repositorio.UnitOfWork unitOfWork)
        {
            var retorno = new
            {
                TipoServicoMultisoftware,
                Culture = System.Globalization.CultureInfo.CurrentCulture.Name,
            };

            ViewBag.ConfiguracaoPadrao = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);
        }

        #endregion
    }
}
