using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RegraICMSController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            { 
                Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);

                int.TryParse(Request.Params["InicioRegistros"], out int inicioRegistros);

                string ufEmitete = Request.Params["UFEmitete"] ?? string.Empty;
                string ufOrigem = Request.Params["UFOrigem"] ?? string.Empty;
                string ufDestino = Request.Params["UFDestino"] ?? string.Empty;
                string ufTomador = Request.Params["UFTomador"] ?? string.Empty;
                
                List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regras = repRegraICMS.ConsultarMultiCTe(this.EmpresaUsuario.Codigo, ufEmitete, ufOrigem, ufDestino, ufTomador, inicioRegistros, 50);
                int countRegras = repRegraICMS.ContarConsultaMultiCTe(this.EmpresaUsuario.Codigo, ufEmitete, ufOrigem, ufDestino, ufTomador);

                var retorno = from obj in regras
                              select new
                                {
                                    obj.Codigo,
                                    UFEmitente = obj.UFEmitente?.Sigla ?? string.Empty,
                                    Origem = obj.UFOrigem?.Sigla ?? string.Empty,
                                    Destino = obj.UFDestino?.Sigla ?? string.Empty,
                                    Regra = obj.DescricaoRegra ?? string.Empty
                              };
                string[] campos = new string[] { "Codigo", "Emitente|10", "Origem|10", "Destino|10", "Regra|60" }; ;

                return Json(retorno, true, null, campos, countRegras);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as Regra de ICMS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                // Repositorio
                Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unidadeDeTrabalho);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.Embarcador.ICMS.RegraICMS regra = repRegraICMS.BuscarPorCodigoEmpresa(codigo, this.EmpresaUsuario.Codigo);

                if (regra == null)
                    regra = new Dominio.Entidades.Embarcador.ICMS.RegraICMS();

                PreencherEntidade(ref regra, unidadeDeTrabalho);
                
                // Valida dados
                if((!string.IsNullOrWhiteSpace(regra.CST) && !(new string[] { "40", "41", "51","SN" }).Contains(regra.CST)) && ((!regra.Aliquota.HasValue || regra.Aliquota.Value == 0) && !regra.ZerarValorICMS))
                    return Json<bool>(false, false, "Quando uma CST estiver selecionada, é obrigatório selecionar uma Aliquota.");

                if (regra.ZerarValorICMS && (!regra.Aliquota.HasValue || regra.Aliquota.Value != 0))
                    return Json<bool>(false, false, "Quando a opção de zerar BC estiver marcada, é obrigatório ter Aliquota zerada.");

                // Adiciona log
                if (regra.Codigo == 0)
                {
                    if (this.UsuarioAdministrativo != null)
                        regra.Log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Inserido  por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome);
                    else
                        regra.Log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Inserido  por ", this.Usuario.CPF, " - ", this.Usuario.Nome);
                } 
                else
                {
                    if (this.UsuarioAdministrativo != null)
                        regra.Log += string.Concat("\n", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Alterado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome);
                    else
                        regra.Log += string.Concat("\n", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Alterado por ", this.Usuario.CPF, " - ", this.Usuario.Nome);
                }

                unidadeDeTrabalho.Start();

                if (regra.Codigo > 0)
                    repRegraICMS.Atualizar(regra);
                else
                    repRegraICMS.Inserir(regra);

                unidadeDeTrabalho.CommitChanges();

                return Json(new
                {
                    Codigo = regra.Codigo
                }, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a regra.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult InserirArquivo()
        {
            Repositorio.UnitOfWork unidadeDeTarabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unidadeDeTarabalho);

                string[] extensoesValidas = { ".jpg", ".png", ".pdf", ".xls", ".xlsx", ".doc", ".docx", ".txt" };

                int.TryParse(Request.Params["Codigo"], out int codigo);
                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = repRegraICMS.BuscarPorCodigo(codigo);

                if (Request.Files.Count == 0)
                    return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");

                if (regraICMS == null)
                    return Json<bool>(false, false, "Não foi possível buscar os dados.");

                // Converte arquivo upado
                System.Web.HttpPostedFileBase file = Request.Files[0];

                // Valida
                string extensao = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!extensoesValidas.Contains(extensao))
                    return Json<bool>(false, false, "Extensão " + extensao.Substring(1) + " inválida.");

                // Cria Entidade e insere
                regraICMS.NomeAnexo = System.IO.Path.GetFileName(file.FileName);

                // Salva na pasta configurada
                string caminho = this.CaminhoArquivos();
                string arquivoFisico = regraICMS.Codigo.ToString() + extensao;
                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                if (Utilidades.IO.FileStorageService.Storage.Exists(arquivoFisico))
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivoFisico);

                Utilidades.IO.FileStorageService.Storage.SaveStream(arquivoFisico, file.InputStream);

                regraICMS.CaminhoAnexo = arquivoFisico;
                
                repRegraICMS.Atualizar(regraICMS);

                // Retorna informacoes
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeDeTarabalho.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");
            }
            finally
            {
                unidadeDeTarabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = repRegraICMS.BuscarPorCodigo(codigo);

                if (regraICMS == null)
                    return Json<bool>(false, false, "Não foi possível encontrar regra de icms.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(regraICMS.CaminhoAnexo))
                    return Json<bool>(false, false, "O arquivo não existe.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(regraICMS.CaminhoAnexo), MimeMapping.GetMimeMapping(regraICMS.CaminhoAnexo), regraICMS.NomeAnexo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["Codigo"], out int codigo);

                Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.ICMS.RegraICMS regra = repRegraICMS.BuscarPorCodigo(codigo);

                if (regra == null)
                    return Json<bool>(false, false, "Regra não encontrado");

                var retorno = new
                {
                    regra.Codigo,
                    Status = regra.Ativo,
                    EstadoEmitente = regra.UFEmitente?.Sigla ?? string.Empty,
                    EstadoOrigem = regra.UFOrigem?.Sigla ?? string.Empty,
                    UFOrigemDiferente = regra.EstadoOrigemDiferente,
                    UFDestinoDiferente = regra.EstadoDestinoDiferente,
                    EstadoDestino = regra.UFDestino?.Sigla ?? string.Empty,
                    EstadoTomador = regra.UFTomador?.Sigla ?? string.Empty,
                    EstadoEmitenteDiferente = regra.UFEmitenteDiferente?.Sigla ?? string.Empty,
                    Remetente = regra.Remetente != null ? new { Codigo = regra.Remetente.CPF_CNPJ, Descricao = regra.Remetente.Nome } : null,
                    AtividadeRemetente = regra.AtividadeRemetente != null ? new { Codigo = regra.AtividadeRemetente.Codigo, Descricao = regra.AtividadeRemetente.Descricao } : null,
                    Destinatario = regra.Destinatario != null ? new { Codigo = regra.Destinatario.CPF_CNPJ, Descricao = regra.Destinatario.Nome } : null,
                    AtividadeDestinatario = regra.AtividadeDestinatario != null ? new { Codigo = regra.AtividadeDestinatario.Codigo, Descricao = regra.AtividadeDestinatario.Descricao } : null,
                    Tomador = regra.Tomador != null ? new { Codigo = regra.Tomador.CPF_CNPJ, Descricao = regra.Tomador.Nome } : null,
                    AtividadeTomador = regra.AtividadeTomador != null ? new { Codigo = regra.AtividadeTomador.Codigo, Descricao = regra.AtividadeTomador.Descricao } : null,
                    regra.CST,
                    CFOP = regra.CFOP != null ? new { Codigo = regra.CFOP.Codigo, Descricao = regra.CFOP.CodigoCFOP.ToString() } : null,
                    Aliquota = regra.Aliquota != null ? new { Codigo = regra.Aliquota.ToString(), Descricao = regra.Aliquota.ToString() } : null,
                    PercentualReducaoBC = regra.PercentualReducaoBC.HasValue ? regra.PercentualReducaoBC.Value.ToString("n2") : "0,00",
                    regra.DescricaoRegra,
                    regra.ImprimeLeiNoCTe,
                    regra.ZerarValorICMS,
                    regra.Log,
                    AliquotaSimples = regra.AliquotaSimples.HasValue ? regra.AliquotaSimples.Value.ToString("n2") : "0,00",
                    PossuiArquivo = !string.IsNullOrWhiteSpace(regra.NomeAnexo)
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da regra.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.ICMS.RegraICMS regra, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            // Repositório
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);
            Repositorio.AliquotaDeICMS repAliquota = new Repositorio.AliquotaDeICMS(unidadeDeTrabalho);

            // Converte dados
            bool.TryParse(Request.Params["Status"], out bool status);

            //string ufEstadoEmitente = Request.Params["EstadoEmitente"] ?? string.Empty;
            string ufEstadoOrigem = Request.Params["EstadoOrigem"] ?? string.Empty;
            bool.TryParse(Request.Params["UFOrigemDiferente"], out bool ufOrigemDiferente);
            string ufEstadoDestino = Request.Params["EstadoDestino"] ?? string.Empty;
            bool.TryParse(Request.Params["UFDestinoDiferente"], out bool ufDestinoDiferente);
            string ufEstadoTomador = Request.Params["EstadoTomador"] ?? string.Empty;
            //string ufEstadoEmitenteDiferente = Request.Params["EstadoEmitenteDiferente"] ?? string.Empty;

            double.TryParse(Request.Params["Remetente"], out double codigoRemetente);
            int.TryParse(Request.Params["AtividadeRemetente"], out int codigoAtividadeRemetente);
            double.TryParse(Request.Params["Destinatario"], out double codigoDestinatario);
            int.TryParse(Request.Params["AtividadeDestinatario"], out int codigoAtividadeDestinatario);
            double.TryParse(Request.Params["Tomador"], out double codigoTomador);
            int.TryParse(Request.Params["AtividadeTomador"], out int codigoAtividadeTomador);

            string CST = Request.Params["CST"] ?? string.Empty;
            int.TryParse(Request.Params["CFOP"], out int codigoCFOP);
            string aliquotaParametro = Request.Params["Aliquota"] ?? string.Empty;
            decimal.TryParse(Request.Params["PercentualReducaoBC"], out decimal percentualReducaoBC);
            decimal.TryParse(Request.Params["AliquotaSimples"], out decimal aliquotaSimples);

            int? aliquota = null;
            if (!string.IsNullOrWhiteSpace(aliquotaParametro))
            {
                decimal.TryParse(aliquotaParametro, out decimal valorAliquota);
                Dominio.Entidades.AliquotaDeICMS aliquotaICMS = repAliquota.BuscarPorAliquota(this.EmpresaUsuario.Codigo, valorAliquota);

                if (aliquotaICMS != null)
                    aliquota = decimal.ToInt32(aliquotaICMS.Aliquota);
            }

            string descricaoRegra = Request.Params["DescricaoRegra"] ?? string.Empty;
            bool.TryParse(Request.Params["ImprimeLeiNoCTe"], out bool imprimeLeiNoCTe);
            bool.TryParse(Request.Params["ZerarValorICMS"], out bool zerarValorICMS);

            regra.Empresa = this.EmpresaUsuario;
            regra.Ativo = status;
            //regra.UFEmitente = repEstado.BuscarPorSigla(ufEstadoEmitente);
            regra.UFOrigem = repEstado.BuscarPorSigla(ufEstadoOrigem);
            regra.EstadoOrigemDiferente = ufOrigemDiferente;
            regra.UFDestino = repEstado.BuscarPorSigla(ufEstadoDestino);
            regra.EstadoDestinoDiferente = ufDestinoDiferente;
            regra.UFTomador = repEstado.BuscarPorSigla(ufEstadoTomador);
            //regra.UFEmitenteDiferente = repEstado.BuscarPorSigla(ufEstadoEmitenteDiferente);
            regra.Remetente = codigoRemetente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoRemetente) : null;
            regra.AtividadeRemetente = repAtividade.BuscarPorCodigo(codigoAtividadeRemetente);
            regra.Destinatario = codigoDestinatario > 0 ? repCliente.BuscarPorCPFCNPJ(codigoDestinatario) : null;
            regra.AtividadeDestinatario = repAtividade.BuscarPorCodigo(codigoAtividadeDestinatario);
            regra.Tomador = codigoTomador > 0 ? repCliente.BuscarPorCPFCNPJ(codigoTomador) : null;
            regra.AtividadeTomador = repAtividade.BuscarPorCodigo(codigoAtividadeTomador);
            regra.CST = CST;
            regra.CFOP = repCFOP.BuscarPorCodigo(codigoCFOP);
            regra.Aliquota = aliquota;
            regra.PercentualReducaoBC = percentualReducaoBC;
            regra.DescricaoRegra = descricaoRegra;
            regra.ImprimeLeiNoCTe = imprimeLeiNoCTe;
            regra.ZerarValorICMS = zerarValorICMS;
            regra.AliquotaSimples = aliquotaSimples;
        }

        private string CaminhoArquivos()
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivos"], "AnexosRegras");

            return caminho;
        }
    }
}