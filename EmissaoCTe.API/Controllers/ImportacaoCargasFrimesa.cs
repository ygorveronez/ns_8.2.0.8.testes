using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.IO;
using OfficeOpenXml;

namespace EmissaoCTe.API.Controllers
{
    public class ImportacaoCargasFrimesaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("importacaocargasfrimesa.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, inicioRegistros;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string placa = Request.Params["Placa"];
                //if (!string.IsNullOrEmpty(placa) && placa.Length >= 7)
                //    placa = placa.Substring(0, 3) + Utilidades.String.OnlyNumbers(placa);
                if (!string.IsNullOrEmpty(placa))
                    placa = Utilidades.String.OnlyNumbersAndChars(placa);


                string embarcador = Utilidades.String.OnlyNumbers(Request.Params["Embarcador"]);

                DateTime data;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.AssumeLocal, out data);

                Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unitOfWork);

                List<Dominio.Entidades.CargaFrimesa> listaCargasFrimesa = repCargaFrimesa.Consultar(data, placa, embarcador, inicioRegistros, 50);

                int countDocumentos = repCargaFrimesa.ContarConsulta(data, placa, embarcador);

                var retorno = (from obj in listaCargasFrimesa
                               select new
                               {
                                   obj.Codigo,
                                   CodigoDocumento = !string.IsNullOrWhiteSpace(obj.NumerosCTes) ? 1 : !string.IsNullOrWhiteSpace(obj.NumerosNFSes) ? 1 : 0,  // obj.CTe != null ? "Número: " + obj.CTe.Numero + " Série: " + obj.CTe.Serie.Numero : obj.NFSe != null ? "Número: " + obj.NFSe.Numero + " Série: " + obj.NFSe.Serie.Numero : string.Empty,, // obj.CTe != null ? obj.CTe.Codigo : obj.NFSe != null ? obj.NFSe.Codigo : 0,
                                   CodigoEmpresa = obj.Empresa != null ? obj.Empresa.Codigo : 0,
                                   StatusEmpresa = obj.Empresa != null ? obj.Empresa.Status : "",
                                   DataCarga = obj.DataCarga.ToString("dd/MM/yyyy"),
                                   Transportadora = obj.Empresa != null ? obj.Empresa.RazaoSocial : "?" + obj.DescricaoTransportadora,
                                   Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : "?" + obj.DescricaoVeiculo,
                                   Tipo = obj.TipoVeiculo != null ? obj.TipoVeiculo.Descricao : "?" + obj.DescricaoTipo,
                                   Rota = obj.Rota != null ? obj.Rota.CodigoIntegracao : "?" + obj.DescricaoRota,
                                   ValorFrete = obj.ValorFrete.ToString("n2"),
                                   ValorAdicional = obj.ValorAdicionalPeso.ToString("n2"),
                                   TipoDocumento = obj.TipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe ? "NFS-e" : "CT-e",
                                   Documento = !string.IsNullOrWhiteSpace(obj.NumerosCTes) ? obj.NumerosCTes : !string.IsNullOrWhiteSpace(obj.NumerosNFSes) ? obj.NumerosNFSes : string.Empty,  // obj.CTe != null ? "Número: " + obj.CTe.Numero + " Série: " + obj.CTe.Serie.Numero : obj.NFSe != null ? "Número: " + obj.NFSe.Numero + " Série: " + obj.NFSe.Serie.Numero : string.Empty,
                                   Status = obj.Documentos == null || obj.Documentos.Count == 0 ? "NF-es Pendentes" :
                                            obj.Documentos.Where(o => o.CTe != null && o.CTe.Status == "E").Count() > 0 ? "CTe(s) em emissão" :
                                            obj.Documentos.Where(o => o.CTe != null && o.CTe.Status == "S").Count() > 0 ? "CTe(s) aguardando emissão" :
                                            obj.Documentos.Where(o => o.CTe != null && o.CTe.Status == "R").Count() > 0 ? "CTe(s) rejeitados(s)" :
                                            obj.Documentos.Where(o => o.CTe != null && o.CTe.Status == "A").Count() > 0 ? "CTe(s) autorizados(s)" :
                                            obj.Documentos.Where(o => o.NFSe != null && o.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Enviado).Count() > 0 ? "NFSe(s) em emissão." :
                                            obj.Documentos.Where(o => o.NFSe != null && o.NFSe.Status == Dominio.Enumeradores.StatusNFSe.EmDigitacao).Count() > 0 ? "NFSe(s) aguardando emissão." :
                                            obj.Documentos.Where(o => o.NFSe != null && o.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao).Count() > 0 ? "NFSe(s) rejeitada(s)." :
                                            obj.Documentos.Where(o => o.NFSe != null && o.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado).Count() > 0 ? "NFSe(s) autorizada(s)." :
                                            "Aguardando emissão"//obj.CTe != null ? "CT-e " + obj.CTe.DescricaoStatus : obj.NFSe != null ? "NFS-e " + obj.NFSe.DescricaoStatus : "NF-e's Pendente",
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "CodigoDocumento", "CodigoEmpresa", "StatusEmpresa", "Data Carga|8", "Transportadora|10", "Veiculo|8", "Tipo|5", "Rota|10", "Valor Frete|8", "Valor Adc. Peso|5", "Tipo Doc.|5", "Doc.|8", "Status|10" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as Cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            DateTime data;
            DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.AssumeLocal, out data);

            if (data == DateTime.MinValue)
                return Json<bool>(false, false, "Data invalida, favor selecionar a data da carga!");

            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (fileExtension.ToLower() == ".xlsx")
                    {

                        if (!ImportarExcelCargas(data, file.InputStream, unitOfWork))
                            return Json<bool>(false, false, "Ocorreu uma falha ao importar arquivo.");
                    }
                    else
                        return Json<bool>(false, false, "Extensão do arquivo deve ser xlsx .");

                }
                else
                {
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao importar arquivos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult DeletarCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unitOfWork);

            DateTime data;
            DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.AssumeLocal, out data);

            string embarcador = Utilidades.String.OnlyNumbers(Request.Params["Embarcador"]);

            if (data == DateTime.MinValue)
                return Json<bool>(false, false, "Data invalida, favor selecionar a data da carga!");

            try
            {
                List<Dominio.Entidades.CargaFrimesa> listaCargaFrimesa = repCargaFrimesa.BuscarPorData(data, embarcador);

                if (listaCargaFrimesa.Count == 0)
                    return Json<bool>(false, false, "Não existem cargas para a data informada!");

                foreach (Dominio.Entidades.CargaFrimesa cargaFrimesa in listaCargaFrimesa)
                {
                    //if (cargaFrimesa.CTe != null || cargaFrimesa.NFSe != null)
                    //    return Json<bool>(false, false, "Carga já possúi documentos emitidos, impossível excluir!");
                    if (cargaFrimesa.Documentos != null && cargaFrimesa.Documentos.Count > 0)
                        return Json<bool>(false, false, "Carga já possúi documentos emitidos, impossível excluir!");
                }

                foreach (Dominio.Entidades.CargaFrimesa cargaFrimesa in listaCargaFrimesa)
                    repCargaFrimesa.Deletar(cargaFrimesa);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao deletar cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EmitirDocumentos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);
                bool ajustarGlobalizado = !string.IsNullOrWhiteSpace(Request.Params["AjustarGlobalizado"]) && Request.Params["AjustarGlobalizado"] == "SIM";

                Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unidadeDeTrabalho);
                Repositorio.CargaFrimesaDocumentos repCargaFrimesaDocumentos = new Repositorio.CargaFrimesaDocumentos(unidadeDeTrabalho);

                Dominio.Entidades.CargaFrimesa cargaFrimesa = repCargaFrimesa.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.CargaFrimesaDocumentos> listaDocumentos = repCargaFrimesaDocumentos.BuscarPorCargaFrimesa(codigo);

                if (cargaFrimesa.ValorFrete > 0 && cargaFrimesa.TipoDocumento == Dominio.Enumeradores.TipoDocumento.CTe && cargaFrimesa.Rota.PermiteAgruparCargas == false)
                {
                    Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = listaDocumentos.Where(o => o.CTe != null && (o.CTe.Status == "R" || o.CTe.Status == "S")).Select(o => o.CTe).ToList();
                    if (listaCTes.Count > 0)
                    {
                        try
                        {
                            unidadeDeTrabalho.Start();
                            if (svcCTe.RatearValorFreteEntreCTes(listaCTes, cargaFrimesa.ValorFrete + cargaFrimesa.ValorAdicionalPeso, Dominio.Enumeradores.TipoRateioTabelaFreteValor.Peso, unidadeDeTrabalho))
                                unidadeDeTrabalho.CommitChanges();
                            else
                            {
                                unidadeDeTrabalho.Rollback();
                                return Json<bool>(true, false, "Não foi possível ratear valor do frete entre os CTes da carga.");
                            }
                        }
                        catch (Exception ex)
                        {
                            unidadeDeTrabalho.Rollback();

                            Servicos.Log.TratarErro("Ratear valor CTes carga Frimesa: " + ex);

                            return Json<bool>(false, false, "Ocorreu uma falha ao ratear valor do frete entre os CTes da carga.");
                        }
                    }
                    else
                        return Json<bool>(true, false, "Alguns CTes estão com status que não permitem a emissão.");
                }


                foreach (Dominio.Entidades.CargaFrimesaDocumentos documento in listaDocumentos)
                {
                    if (documento.CTe != null && (documento.CTe.Status == "S" || documento.CTe.Status == "R"))
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                        Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(documento.CTe.Codigo);

                        if (cte.Empresa.Status != "A")
                            return Json<bool>(true, false, "Empresa não está ativa para emissão de CT-e.");

                        if (cte.Empresa.StatusFinanceiro == "B")
                            return Json<bool>(true, false, "Empresa está com pendências, contate o setor de cadastros para maiores informações.");

                        if (this.UsuarioAdministrativo != null)
                            cte.Usuario = this.UsuarioAdministrativo;
                        else
                            cte.Usuario = this.Usuario;

                        cte.ObservacoesAvancadas = string.Empty; //Limpa as observações avançadas que são informadas toda vez ao emitir

                        if (cte.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim && ajustarGlobalizado && cte.Versao == "3.00")
                        {
                            if (documento.CargaFrimesa.Rota.PermiteAgruparCargas) //Quando emite um CTe por NFe não muda para diversos
                            {
                                if (cte.Documentos == null || cte.Documentos.Count < 5)
                                {
                                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);
                                    cte.IndicadorGlobalizado = Dominio.Enumeradores.OpcaoSimNao.Nao;
                                    cte.SetarDestinatarioDiversos(cte.Empresa, repAtividade.BuscarPorCodigo(4), "DIVERSO");
                                }
                            }
                        }

                        svcCTe.SetarObservacoesAvancadas(ref cte, new Repositorio.UnitOfWork(Conexao.StringConexao));
                        svcCTe.SetarObservacaoAvancadaPorRegraICMS(ref cte, new Repositorio.UnitOfWork(Conexao.StringConexao));

                        repCTe.Atualizar(cte);

                        if (svcCTe.Emitir(cte.Codigo, cte.Empresa.Codigo))
                        {
                            svcCTe.AdicionarCTeNaFilaDeConsulta(cte, unidadeDeTrabalho);
                            //FilaConsultaCTe.GetInstance().QueueItem(cte.Empresa.Codigo, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                        }
                        else
                        {
                            cte.Status = "R";
                            repCTe.Atualizar(cte);
                        }
                    }
                    else if (documento.NFSe != null && (documento.NFSe.Status == Dominio.Enumeradores.StatusNFSe.EmDigitacao || documento.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao))
                    {
                        Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                        Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(documento.NFSe.Codigo);

                        Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                        if (nfse.Empresa.Status != "A")
                            return Json<bool>(true, false, "Empresa não está ativa para emissão de NFS-e.");

                        if (nfse.Empresa.StatusFinanceiro == "B")
                            return Json<bool>(true, false, "Empresa está com pendências, contate o setor de cadastros para maiores informações.");

                        if (svcNFSe.Emitir(nfse, unidadeDeTrabalho))
                        {
                            nfse.Status = Dominio.Enumeradores.StatusNFSe.Enviado;

                            repNFSe.Atualizar(nfse);

                            svcNFSe.AdicionarNFSeNaFilaDeConsulta(nfse, unidadeDeTrabalho);
                        }
                        else
                        {
                            nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;

                            repNFSe.Atualizar(nfse);
                        }
                    }
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir os documentos.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadPDF()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unidadeDeTrabalho);
                Repositorio.CargaFrimesaDocumentos repCargaFrimesaDocumentos = new Repositorio.CargaFrimesaDocumentos(unidadeDeTrabalho);

                Dominio.Entidades.CargaFrimesa cargaFrimesa = repCargaFrimesa.BuscarPorCodigo(codigo);

                if (cargaFrimesa == null)
                    return Json<bool>(false, false, "Carga não localizada.");

                List<Dominio.Entidades.CargaFrimesaDocumentos> listaDocumentos = repCargaFrimesaDocumentos.BuscarPorCargaFrimesa(codigo);

                if (listaDocumentos == null || listaDocumentos.Count == 0)
                    return Json<bool>(false, false, "Nenhum documento encontrado para download do PDF.");

                if (listaDocumentos.Count == 1)
                {
                    if (listaDocumentos[0].CTe != null)
                    {
                        if (listaDocumentos[0].CTe.Status != "A" && listaDocumentos[0].CTe.Status != "C" && listaDocumentos[0].CTe.Status != "K" && listaDocumentos[0].CTe.Status != "F" && listaDocumentos[0].CTe.Status != "Q")
                            return Json<bool>(false, false, "O status do CT-e não permite a geração do DACTE.");

                        if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoRelatorios"]))
                            return Json<bool>(false, false, "O caminho para os download da DACTE não está disponível. Contate o suporte técnico.");

                        string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoRelatorios"], listaDocumentos[0].CTe.Empresa.CNPJ, listaDocumentos[0].CTe.Chave);

                        if (listaDocumentos[0].CTe.Status == "A" || listaDocumentos[0].CTe.Status == "C" || listaDocumentos[0].CTe.Status == "K")
                            caminhoPDF = caminhoPDF + ".pdf";
                        else if (!string.IsNullOrWhiteSpace(listaDocumentos[0].CTe.ChaveContingencia) && listaDocumentos[0].CTe.TipoEmissao == "5") //FSDA
                            caminhoPDF = caminhoPDF + "_FSDA.pdf";
                        else if (!string.IsNullOrWhiteSpace(listaDocumentos[0].CTe.ChaveContingencia) && listaDocumentos[0].CTe.TipoEmissao == "4") //EPEC
                            caminhoPDF = caminhoPDF + "_EPEC.pdf";

                        byte[] dacte = null;

                        long tamanhoPDF = 0;
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        {
                            FileInfo file = new FileInfo(caminhoPDF);
                            tamanhoPDF = file.Length;
                        }

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF) || tamanhoPDF <= 0)
                        {
                            ////Buscar DACTE do Oracle
                            if (ConfigurationManager.AppSettings["RegerarDACTEOracle"] != "NAO")
                            {
                                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);
                                servicoCTe.ObterESalvarDACTE(listaDocumentos[0].CTe, listaDocumentos[0].CTe.Empresa.Codigo, null, unidadeDeTrabalho);

                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                {
                                    FileInfo file = new FileInfo(caminhoPDF);
                                    tamanhoPDF = file.Length;
                                }
                            }

                            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF) || tamanhoPDF <= 0)
                            {
                                if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoGeradorRelatorios"]))
                                    return Json<bool>(false, false, "O gerador da DACTE não está disponível. Contate o suporte técnico.");

                                Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeDeTrabalho);

                                dacte = svcDACTE.GerarPorProcesso(listaDocumentos[0].CTe.Codigo);
                            }
                            else
                                dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                        }
                        else
                            dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);

                        var nomePDF = System.IO.Path.GetFileName(caminhoPDF);

                        if (dacte != null)
                        {
                            try
                            {
                                return Arquivo(dacte, "application/pdf", nomePDF);
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                                nomePDF = System.IO.Path.GetFileName(caminhoPDF);
                                return Arquivo(dacte, "application/pdf", nomePDF);
                            }
                        }
                        else
                            return Json<bool>(false, false, "Não foi possível gerar o DACTE, atualize a página e tente novamente.");

                    }
                    else if (listaDocumentos[0].NFSe != null)
                    {
                        if (listaDocumentos[0].NFSe.Status != Dominio.Enumeradores.StatusNFSe.Autorizado && listaDocumentos[0].NFSe.Status != Dominio.Enumeradores.StatusNFSe.Cancelado && listaDocumentos[0].NFSe.Status != Dominio.Enumeradores.StatusNFSe.EmCancelamento)
                            return Json<bool>(false, false, "O status da NFS-e não permite a geração do DANFSE.");

                        Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                        byte[] danfse = svcNFSe.ObterDANFSE(listaDocumentos[0].NFSe.Codigo);

                        if (danfse != null)
                            return Arquivo(danfse, "application/pdf", "NFSe_" + listaDocumentos[0].NFSe.Numero.ToString() + ".pdf");
                        else
                            return Json<bool>(false, false, "Não foi possível gerar o DANFSE, atualize a página e tente novamente.");
                    }
                    else
                        return Json<bool>(false, false, "Documento não encontrado, atualize a página e tente novamente.");

                }
                {
                    List<string> listaChavesCTes = new List<string>();
                    List<string> listaCodigosNFSe = new List<string>();

                    foreach (Dominio.Entidades.CargaFrimesaDocumentos documento in listaDocumentos)
                    {
                        if (documento.CTe != null)
                        {
                            if (documento.CTe != null && documento.CTe.Status == "A")
                                listaChavesCTes.Add(documento.CTe.Chave);
                        }
                        else if (documento.NFSe != null)
                        {
                            if (documento.NFSe != null && documento.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                                listaCodigosNFSe.Add(documento.NFSe.Codigo.ToString());
                        }
                    }

                    if (listaChavesCTes.Count > 0)
                    {
                        string nomeArquivo = cargaFrimesa.Veiculo != null ? cargaFrimesa.Veiculo.Placa : cargaFrimesa.Codigo.ToString();
                        nomeArquivo = "LoteDACTE_" + nomeArquivo + ".zip";

                        Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                        return Arquivo(svcCTe.ObterLoteDeDACTE(listaChavesCTes, 0, unidadeDeTrabalho), "application/zip", nomeArquivo);
                    }
                    else if (listaCodigosNFSe.Count > 0)
                    {
                        string nomeArquivo = cargaFrimesa.Veiculo != null ? cargaFrimesa.Veiculo.Placa : cargaFrimesa.Codigo.ToString();
                        nomeArquivo = "LoteDANFSE_" + nomeArquivo + ".zip";

                        Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                        return Arquivo(svcNFSe.ObterLoteDeDANFSE(listaCodigosNFSe, unidadeDeTrabalho), "application/zip", nomeArquivo);
                    }
                    return Json<bool>(false, false, "Nenhum PDF encontrado.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do DANFSE.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXML()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unidadeDeTrabalho);
                Repositorio.CargaFrimesaDocumentos repCargaFrimesaDocumentos = new Repositorio.CargaFrimesaDocumentos(unidadeDeTrabalho);

                Dominio.Entidades.CargaFrimesa cargaFrimesa = repCargaFrimesa.BuscarPorCodigo(codigo);

                if (cargaFrimesa == null)
                    return Json<bool>(false, false, "Carga não localizada.");

                List<Dominio.Entidades.CargaFrimesaDocumentos> listaDocumentos = repCargaFrimesaDocumentos.BuscarPorCargaFrimesa(codigo);

                if (listaDocumentos == null || listaDocumentos.Count == 0)
                    return Json<bool>(false, false, "Nenhum documento encontrado para download do PDF.");

                if (listaDocumentos.Count == 1)
                {
                    if (listaDocumentos.FirstOrDefault().CTe != null)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                        byte[] data = svcCTe.ObterXMLAutorizacao(listaDocumentos.FirstOrDefault().CTe);

                        if (data != null)
                            return Arquivo(data, "text/xml", string.Concat(listaDocumentos.FirstOrDefault().CTe.Chave, ".xml"));
                        return Json<bool>(false, false, "XML não encontrado.");
                    }
                    else if (listaDocumentos.FirstOrDefault().NFSe != null)
                    {
                        if (listaDocumentos.FirstOrDefault().NFSe.Status != Dominio.Enumeradores.StatusNFSe.Autorizado && listaDocumentos.FirstOrDefault().NFSe.Status != Dominio.Enumeradores.StatusNFSe.Cancelado && listaDocumentos.FirstOrDefault().NFSe.Status != Dominio.Enumeradores.StatusNFSe.EmCancelamento)
                            return Json<bool>(false, false, "O status da NFS-e não permite a geração do XML.");

                        Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                        byte[] xml = svcNFSe.ObterXML(listaDocumentos.FirstOrDefault().NFSe.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao);

                        if (xml != null)
                            return Arquivo(xml, "text/xml", string.Concat("NFSe_", listaDocumentos.FirstOrDefault().NFSe.Numero, ".xml"));

                        return Json<bool>(false, false, "XML não encontrado.");
                    }
                    else
                        return Json<bool>(false, false, "Documento não encontrado.");

                }
                {
                    List<int> listaCodigosCtes = new List<int>();
                    List<int> listaCodigosNFSe = new List<int>();

                    foreach (Dominio.Entidades.CargaFrimesaDocumentos documento in listaDocumentos)
                    {
                        if (documento.CTe != null)
                        {
                            if (documento.CTe != null && documento.CTe.Status == "A")
                                listaCodigosCtes.Add(documento.CTe.Codigo);
                        }
                        else if (documento.NFSe != null)
                        {
                            if (documento.NFSe != null && documento.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                                listaCodigosNFSe.Add(documento.NFSe.Codigo);
                        }
                    }

                    if (listaCodigosCtes.Count > 0)
                    {
                        string nomeArquivo = cargaFrimesa.Veiculo != null ? cargaFrimesa.Veiculo.Placa : cargaFrimesa.Codigo.ToString();
                        nomeArquivo = "LoteXML_" + nomeArquivo + ".zip";

                        Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                        return Arquivo(svcCTe.ObterLoteDeXML(listaCodigosCtes, 0), "application/zip", nomeArquivo);
                    }
                    else if (listaCodigosNFSe.Count > 0)
                    {
                        string nomeArquivo = cargaFrimesa.Veiculo != null ? cargaFrimesa.Veiculo.Placa : cargaFrimesa.Codigo.ToString();
                        nomeArquivo = "LoteXML_" + nomeArquivo + ".zip";

                        Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                        return Arquivo(svcNFSe.ObterLoteDeXML(listaCodigosNFSe, 0, unidadeDeTrabalho), "application/zip", nomeArquivo);
                    }

                    return Json<bool>(false, false, "Nenhum XML encontrado.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download XML.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        public ActionResult DownloadLoteDACTE()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                DateTime dataCarga;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCarga);

                string placa = Request.Params["Placa"];

                string embarcador = Utilidades.String.OnlyNumbers(Request.Params["Embarcador"]);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unidadeDeTrabalho);

                List<Dominio.Entidades.CargaFrimesa> listaCargasFrimesa = repCargaFrimesa.Consultar(dataCarga, placa, embarcador, 0, 500);
                List<string> listaChavesCTes = new List<string>();

                foreach (Dominio.Entidades.CargaFrimesa carga in listaCargasFrimesa)
                {
                    if (carga.Documentos != null && carga.Documentos.Count > 0)
                    {
                        foreach (Dominio.Entidades.CargaFrimesaDocumentos documentos in carga.Documentos)
                        {
                            if (documentos.CTe != null && documentos.CTe.Status == "A")
                                listaChavesCTes.Add(documentos.CTe.Chave);
                        }
                    }
                }

                if (listaChavesCTes.Count <= 0)
                    return Json<bool>(false, false, "Nenhuma DACTE encontrada para o período selecionado.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                return Arquivo(svcCTe.ObterLoteDeDACTE(listaChavesCTes, 0, unidadeDeTrabalho), "application/zip", "LoteDACTE.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o lote de CT-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public ActionResult DownloadLoteXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataCarga;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCarga);

                string placa = Request.Params["Placa"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unitOfWork);

                string embarcador = Utilidades.String.OnlyNumbers(Request.Params["Embarcador"]);

                List<Dominio.Entidades.CargaFrimesa> listaCargasFrimesa = repCargaFrimesa.Consultar(dataCarga, placa, embarcador, 0, 500);
                List<int> listaCodigosCtes = new List<int>();

                foreach (Dominio.Entidades.CargaFrimesa carga in listaCargasFrimesa)
                {
                    if (carga.Documentos != null && carga.Documentos.Count > 0)
                    {
                        foreach (Dominio.Entidades.CargaFrimesaDocumentos documentos in carga.Documentos)
                        {
                            if (documentos.CTe != null && documentos.CTe.Status == "A")
                                listaCodigosCtes.Add(documentos.CTe.Codigo);
                        }
                    }
                }

                if (listaCodigosCtes.Count <= 0)
                    return Json<bool>(false, false, "Nenhum CT-e encontrado para o período selecionado.");

                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                return Arquivo(svcCTe.ObterLoteDeXML(listaCodigosCtes, 0), "application/zip", "LoteXMLCTe.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o lote de CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult DownloadLoteDANFSE()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataCarga;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCarga);

                string placa = Request.Params["Placa"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unidadeDeTrabalho);


                string embarcador = Utilidades.String.OnlyNumbers(Request.Params["Embarcador"]);

                List<Dominio.Entidades.CargaFrimesa> listaCargasFrimesa = repCargaFrimesa.Consultar(dataCarga, placa, embarcador, 0, 500);
                List<string> listaCodigosNFSe = new List<string>();

                foreach (Dominio.Entidades.CargaFrimesa carga in listaCargasFrimesa)
                {
                    //if (carga.NFSe != null && carga.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                    //listaCodigosNFSe.Add(carga.NFSe.Codigo.ToString());
                    if (carga.Documentos != null && carga.Documentos.Count > 0)
                    {
                        foreach (Dominio.Entidades.CargaFrimesaDocumentos documentos in carga.Documentos)
                        {
                            if (documentos.NFSe != null && documentos.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                                listaCodigosNFSe.Add(documentos.NFSe.Codigo.ToString());
                        }
                    }

                }

                if (listaCodigosNFSe.Count <= 0)
                    return Json<bool>(false, false, "Nenhuma DANFSE encontrada para o período selecionado.");

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                return Arquivo(svcNFSe.ObterLoteDeDANFSE(listaCodigosNFSe, unidadeDeTrabalho), "application/zip", "LoteDANFSE.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o lote de NFS-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public ActionResult DownloadLoteXMLNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataCarga;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCarga);

                string placa = Request.Params["Placa"];

                string embarcador = Utilidades.String.OnlyNumbers(Request.Params["Embarcador"]);

                Repositorio.XMLNFSe repCTe = new Repositorio.XMLNFSe(unitOfWork);
                Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unitOfWork);

                List<Dominio.Entidades.CargaFrimesa> listaCargasFrimesa = repCargaFrimesa.Consultar(dataCarga, placa, embarcador, 0, 500);
                List<int> listaCodigosNFSes = new List<int>();

                foreach (Dominio.Entidades.CargaFrimesa carga in listaCargasFrimesa)
                {
                    //if (carga.NFSe != null && carga.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                    //listaCodigosNFSes.Add(carga.NFSe.Codigo);
                    if (carga.Documentos != null && carga.Documentos.Count > 0)
                    {
                        foreach (Dominio.Entidades.CargaFrimesaDocumentos documentos in carga.Documentos)
                        {
                            if (documentos.NFSe != null && documentos.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                                listaCodigosNFSes.Add(documentos.NFSe.Codigo);
                        }
                    }
                }

                if (listaCodigosNFSes.Count <= 0)
                    return Json<bool>(false, false, "Nenhum CT-e encontrada para o período selecionado.");

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                return Arquivo(svcNFSe.ObterLoteDeXML(listaCodigosNFSes, 0, unitOfWork), "application/zip", "LoteXMLNFSe.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o lote de CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool ImportarExcelCargas(DateTime data, Stream stream, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                ExcelPackage package = new ExcelPackage(stream);

                Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unitOfWork);
                Repositorio.FreteFrimesa repFreteFrimesa = new Repositorio.FreteFrimesa(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.TipoVeiculo repTipoVeiculo = new Repositorio.TipoVeiculo(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

                var cellValue = "";

                for (var i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (i > 1)
                    {
                        cellValue = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, 1].Text);

                        //if (cellValue.ToUpper() != "ID DO ITINERARIO")
                        //{
                        try
                        {
                            Dominio.Entidades.CargaFrimesa cargaFrimesa = new Dominio.Entidades.CargaFrimesa();
                            cargaFrimesa.TipoDocumento = Dominio.Enumeradores.TipoDocumento.Todos;
                            cargaFrimesa.DataCarga = data;
                            cargaFrimesa.DataImportacao = DateTime.Now;
                            cargaFrimesa.ValorFretePlanilha = false;
                            cellValue = "";
                            string cnpjTransportadora = "";

                            Dominio.Entidades.Empresa empresa = null;

                            for (var a = 1; a <= worksheet.Dimension.End.Column; a++)
                            {
                                try
                                {
                                    cellValue = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, a].Text);
                                }
                                catch (Exception)
                                {
                                    cellValue = "";
                                }
                                if (a > worksheet.Dimension.End.Column)
                                {
                                    break;
                                }

                                if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue))
                                {
                                    if (a == 1)
                                        cargaFrimesa.DescricaoRota = cellValue;
                                    else if (a == 2)
                                        cargaFrimesa.DescricaoVeiculo = cellValue;
                                    else if (a == 3)
                                        cargaFrimesa.DescricaoTransportadora = cellValue;
                                    else if (a == 4)
                                        cargaFrimesa.DescricaoTipo = cellValue;
                                    else if (a == 5) //Campo utilizado para o valor do frete enviado na planilha da Frimesa de Itajai
                                    {
                                        decimal valorFrete = 0;
                                        decimal.TryParse(cellValue.Replace(".", ","), out valorFrete);
                                        if (valorFrete > 0)
                                        {
                                            cargaFrimesa.ValorFrete = valorFrete;
                                            cargaFrimesa.ValorFretePlanilha = true;
                                        }
                                    }
                                    else if (a == 6) //Campo utilizado para o tipo do documento enviado na planilha da Frimesa de Itajai
                                    {
                                        if (cellValue.ToUpper() == "CTE")
                                            cargaFrimesa.TipoDocumento = Dominio.Enumeradores.TipoDocumento.CTe;
                                        if (cellValue.ToUpper() == "NFSE")
                                            cargaFrimesa.TipoDocumento = Dominio.Enumeradores.TipoDocumento.NFSe;
                                    }
                                    else if (a == 7) //Campo utilizado para o CNPJ do transportador enviado na planilha da Frimesa de Itajai
                                        cnpjTransportadora = Utilidades.String.OnlyNumbers(cellValue);
                                    else if (a == 8) //Campo utilizado para a descrição da carga, enviado na planilha da Frimesa de Itajai
                                    {
                                        if (!string.IsNullOrWhiteSpace(cellValue))
                                            cargaFrimesa.DescricaoCarga = cellValue;
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(cnpjTransportadora))
                                empresa = repEmpresa.BuscarPorCNPJ(cnpjTransportadora);
                            else
                            {
                                if (empresa == null)
                                    empresa = repEmpresa.BuscarPrimeiraPorRazaoSocial(cargaFrimesa.DescricaoTransportadora.Length > 80 ? cargaFrimesa.DescricaoTransportadora.Substring(0, 80) : cargaFrimesa.DescricaoTransportadora);
                                if (empresa == null)
                                    empresa = repEmpresa.BuscarPrimeiraPorRazaoSocial(cargaFrimesa.DescricaoTransportadora.Length > 50 ? cargaFrimesa.DescricaoTransportadora.Substring(0, 50) : cargaFrimesa.DescricaoTransportadora);
                                if (empresa == null)
                                    empresa = repEmpresa.BuscarPrimeiraPorRazaoSocial(cargaFrimesa.DescricaoTransportadora.Length > 20 ? cargaFrimesa.DescricaoTransportadora.Substring(0, 20) : cargaFrimesa.DescricaoTransportadora);
                                if (empresa == null)
                                    empresa = repEmpresa.BuscarPrimeiraPorRazaoSocial(cargaFrimesa.DescricaoTransportadora.Length > 15 ? cargaFrimesa.DescricaoTransportadora.Substring(0, 15) : cargaFrimesa.DescricaoTransportadora);
                            }

                            if (empresa != null)
                            {
                                cargaFrimesa.Empresa = empresa;
                                cargaFrimesa.Veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, cargaFrimesa.DescricaoVeiculo.Substring(0, 8).Replace("-", ""));
                                cargaFrimesa.Rota = repRotaFrete.BuscarPorCodigoIntegracao(empresa.EmpresaPai.Codigo, cargaFrimesa.DescricaoRota);

                                if (cargaFrimesa.TipoDocumento != Dominio.Enumeradores.TipoDocumento.CTe && cargaFrimesa.TipoDocumento != Dominio.Enumeradores.TipoDocumento.NFSe)
                                { //Se não foi enviado um tipo de documento na planilha (enviado na de ITAJAI) seleciona o documento conforme Rota
                                    if (cargaFrimesa.Rota != null)
                                    {
                                        if (cargaFrimesa.Rota.TipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe && empresa.Configuracao != null && !string.IsNullOrEmpty(empresa.Configuracao.SerieRPSNFSe))
                                            cargaFrimesa.TipoDocumento = Dominio.Enumeradores.TipoDocumento.NFSe;
                                        else
                                            cargaFrimesa.TipoDocumento = Dominio.Enumeradores.TipoDocumento.CTe;
                                    }
                                    else
                                        cargaFrimesa.TipoDocumento = Dominio.Enumeradores.TipoDocumento.CTe;
                                }
                                else
                                { //Se veio o tipo documento valida quando for NFSe e a transportadora estiver configurada para emissão de NFSe
                                    if (cargaFrimesa.TipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe && empresa.Configuracao != null && !string.IsNullOrEmpty(empresa.Configuracao.SerieRPSNFSe))
                                        cargaFrimesa.TipoDocumento = Dominio.Enumeradores.TipoDocumento.NFSe;
                                    else
                                        cargaFrimesa.TipoDocumento = Dominio.Enumeradores.TipoDocumento.CTe;
                                }

                                cargaFrimesa.TipoVeiculo = repTipoVeiculo.BuscarPorDescricao(empresa.Codigo, cargaFrimesa.DescricaoTipo);
                                if (cargaFrimesa.Veiculo == null)
                                {
                                    Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo();
                                    veiculo.Empresa = empresa;
                                    veiculo.Placa = cargaFrimesa.DescricaoVeiculo.Substring(0, 8).Replace("-", "");
                                    veiculo.Renavam = "123456789";
                                    veiculo.Tara = 100;
                                    veiculo.CapacidadeKG = 100;
                                    veiculo.CapacidadeM3 = 100;
                                    veiculo.Tipo = "P";
                                    veiculo.TipoVeiculo = "0";
                                    veiculo.TipoRodado = "00";
                                    veiculo.TipoCarroceria = "00";
                                    veiculo.Estado = repEstado.BuscarPorSigla("RJ");
                                    veiculo.Ativo = true;
                                    if (cargaFrimesa.TipoVeiculo != null)
                                        veiculo.TipoDoVeiculo = cargaFrimesa.TipoVeiculo;
                                    repVeiculo.Inserir(veiculo);
                                    cargaFrimesa.Veiculo = veiculo;
                                }
                                if (cargaFrimesa.TipoVeiculo == null && cargaFrimesa.Veiculo != null && cargaFrimesa.Veiculo.TipoDoVeiculo != null)
                                    cargaFrimesa.TipoVeiculo = cargaFrimesa.Veiculo.TipoDoVeiculo;
                                if (!cargaFrimesa.ValorFretePlanilha && cargaFrimesa.Rota != null && cargaFrimesa.TipoVeiculo != null)
                                    cargaFrimesa.ValorFrete = repFreteFrimesa.BuscarValorPorRotaTipoVeiculo(empresa.Codigo, cargaFrimesa.Rota.Codigo, cargaFrimesa.TipoVeiculo.Codigo);
                            }
                            repCargaFrimesa.Inserir(cargaFrimesa);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                        //}
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return false;
            }


        }

        #endregion

    }
}
