using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoAvonFaturasController : ApiController
    {
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int numeroInicial, numeroFinal, inicioRegistros;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.FaturaAvon repFatura = new Repositorio.FaturaAvon(unitOfWork);

                List<Dominio.Entidades.FaturaAvon> faturas = repFatura.Consultar(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataInicial, dataFinal, inicioRegistros, 50);
                int countFaturas = repFatura.ContarConsulta(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataInicial, dataFinal);

                var retorno = from obj in faturas
                              select new
                              {
                                  obj.Codigo,
                                  Numero = obj.Numero + " - " + obj.Serie,
                                  DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                  DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                  ValorTotal = obj.ValorTotal.ToString("n2"),
                                  Status = obj.Status.ToString("F"),
                                  obj.Mensagem
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Número|11", "Emissão|11", "Vencimento|11", "Valor Total|15", "Status|12", "Mensagem|30" }, countFaturas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as faturas.");
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
                DateTime dataVencimento;
                DateTime.TryParseExact(Request.Params["DataVencimento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

                List<int> codigosManifestos = JsonConvert.DeserializeObject<List<int>>(Request.Params["Manifestos"]);

                if (dataVencimento == DateTime.MinValue)
                    return Json<bool>(false, false, "Data de vencimento inválida.");

                if (codigosManifestos == null || codigosManifestos.Count <= 0)
                    return Json<bool>(false, false, "Quantidade de manifestos selecionados inválida.");

                Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);

                if (repDocumento.ContarPorManifesto(codigosManifestos.ToArray()) > 30000)
                    return Json<bool>(false, false, "A quantidade de CT-es para a fatura é superior à permitida. Selecione manifestos para que a quantidade fique menor ou igual a 30 mil CT-es.");

                if (repDocumento.ContarPorManifestoEStatusDiff(codigosManifestos.ToArray(), Dominio.Enumeradores.StatusDocumentoManifestoAvon.Finalizado) > 0)
                    return Json<bool>(false, false, "Há CT-es que não foram finalizados nos manifestos selecionados.");

                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unidadeDeTrabalho);

                List<Dominio.Entidades.ManifestoAvon> manifestos = repManifesto.BuscarPorCodigo(codigosManifestos.ToArray(), this.EmpresaUsuario.Codigo);

                Repositorio.FaturaAvon repFatura = new Repositorio.FaturaAvon(unidadeDeTrabalho);

                foreach (Dominio.Entidades.ManifestoAvon manifesto in manifestos)
                    if (repFatura.ContarFaturasPorManifesto(manifesto.Codigo) > 0)
                        return Json<bool>(false, false, "O manifesto " + manifesto.Numero + " já está vinculado à uma fatura.");

                Dominio.Entidades.FaturaAvon fatura = new Dominio.Entidades.FaturaAvon();

                fatura.DataEmissao = DateTime.Now;
                fatura.DataVencimento = dataVencimento;
                fatura.Empresa = this.EmpresaUsuario;
                fatura.Manifestos = manifestos;
                fatura.ValorTotal = repDocumento.ObterValorDosCTesPorManifesto(codigosManifestos.ToArray());
                fatura.Serie = 1;
                fatura.Numero = repFatura.ObterUltimoNumero(this.EmpresaUsuario.Codigo) + 1;

                repFatura.Inserir(fatura);

                this.EnviarFatura(fatura.Codigo, unidadeDeTrabalho);

                return Json<bool>(true, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a fatura.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarFatura()
        {
            try
            {
                Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

                int codigoFatura;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                Repositorio.FaturaAvon repFatura = new Repositorio.FaturaAvon(unidadeDeTrabalho);

                Dominio.Entidades.FaturaAvon fatura = repFatura.BuscarPorCodigo(codigoFatura, this.EmpresaUsuario.Codigo);

                if (fatura == null)
                    return Json<bool>(false, false, "Fatura não encontrada. Atualize a página e tente novamente.");

                this.EnviarFatura(fatura.Codigo, unidadeDeTrabalho);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao enviar a fatura.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultaFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {

                int codigoFatura = 0;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                Repositorio.FaturaAvon repFatura = new Repositorio.FaturaAvon(unidadeDeTrabalho);

                Dominio.Entidades.FaturaAvon fatura = repFatura.BuscarPorCodigo(codigoFatura);

                if (fatura == null)
                    return Json<bool>(false, false, "Fatura não encontrada.");

                Servicos.Avon svcAvon = new Servicos.Avon(unidadeDeTrabalho);

                Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = svcAvon.ConsultarFatura(this.EmpresaUsuario.CNPJ, this.EmpresaUsuario.Configuracao.TokenIntegracaoAvon, fatura);

                if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "200")
                {
                    if (retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Document != null && retorno.CrossTalk_Body.Document[0].Response != null)
                    {
                        fatura.Mensagem = retorno.CrossTalk_Body.Document[0].Response.InnerCode + " - " + retorno.CrossTalk_Body.Document[0].Response.Description;

                        if (!string.IsNullOrWhiteSpace(retorno.CrossTalk_Body.Document[0].Response.Comment))
                            fatura.Mensagem += " - " + retorno.CrossTalk_Body.Document[0].Response.Comment;

                        repFatura.Atualizar(fatura);

                        return Json<bool>(true, true);
                    }
                }

                fatura.Mensagem = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage;

                repFatura.Atualizar(fatura);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar a fatura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadLoteCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoFatura;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                Repositorio.FaturaAvon repFaturaAvon = new Repositorio.FaturaAvon(unitOfWork);

                Dominio.Entidades.FaturaAvon fatura = repFaturaAvon.BuscarPorCodigo(codigoFatura, this.EmpresaUsuario.Codigo);

                if (fatura == null)
                    return Json<bool>(false, false, "Fatura não encontrada. Atualize a página e tente novamente.");

                Repositorio.DocumentoManifestoAvon repDocumentoManifestoAvon = new Repositorio.DocumentoManifestoAvon(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repDocumentoManifestoAvon.ObterCTes(fatura.Codigo);

                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                return Arquivo(svcCTe.ObterLoteDeXML(ctes, unitOfWork), "application/zip", "Lote_Fatura_" + fatura.Numero + ".zip");
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

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadArquivosFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoFatura;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                Repositorio.FaturaAvon repFaturaAvon = new Repositorio.FaturaAvon(unitOfWork);
                Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(unitOfWork);

                Dominio.Entidades.FaturaAvon fatura = repFaturaAvon.BuscarPorCodigo(codigoFatura, this.EmpresaUsuario.Codigo);

                if (fatura == null)
                    return Json<bool>(false, false, "Fatura não encontrada. Atualize a página e tente novamente.");

                Servicos.Avon svcAvon = new Servicos.Avon(unitOfWork);

                var arquivo = svcAvon.ObterArquivosRequisicaoFatura(fatura, this.EmpresaUsuario.CNPJ, this.EmpresaUsuario.Configuracao.TokenIntegracaoAvon);

                return Arquivo(arquivo, "application/zip", "Fatura_" + fatura.Numero + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar os arquivos da fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult QuitarFatura()
        {
            try
            {
                int codigoFatura;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                string stringConexao = Conexao.StringConexao;

                Task.Factory.StartNew(() => QuitarFatura(codigoFatura, stringConexao));

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao quitar a fatura.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult CancelarFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoFatura;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                Repositorio.FaturaAvon repFatura = new Repositorio.FaturaAvon(unidadeDeTrabalho);

                Dominio.Entidades.FaturaAvon fatura = repFatura.BuscarPorCodigo(codigoFatura);

                if(fatura == null)
                    return Json<bool>(false, false, "Fatura não encontrada.");

                if (fatura.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaAvon.Pendente)
                    return Json<bool>(false, false, "O status da fatura não permite o cancelamento da mesma.");

                fatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusFaturaAvon.Cancelada;

                repFatura.Atualizar(fatura);
                
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao cancelar a fatura.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadDetalhesFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoFatura;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                Repositorio.FaturaAvon repFatura = new Repositorio.FaturaAvon(unidadeDeTrabalho);

                Dominio.Entidades.FaturaAvon fatura = repFatura.BuscarPorCodigo(codigoFatura, this.EmpresaUsuario.Codigo);

                if (fatura == null)
                    return Json<bool>(false, false, "Fatura não encontrada.");

                Dominio.Entidades.ParticipanteCTe tomador = fatura.Manifestos.First().Documentos.First().CTe.Tomador;

                List<Dominio.ObjetosDeValor.Relatorios.DetalheFaturaAvon> detalhes = new List<Dominio.ObjetosDeValor.Relatorios.DetalheFaturaAvon>()
                {
                    new Dominio.ObjetosDeValor.Relatorios.DetalheFaturaAvon()
                    {
                         BairroEmpresa = fatura.Empresa.Bairro,
                         CEPEmpresa = fatura.Empresa.CEP,
                         CidadeEmpresa = fatura.Empresa.Localidade.DescricaoCidadeEstado,
                         CNPJEmpresa = fatura.Empresa.CNPJ_Formatado,
                         DataVencimento = fatura.DataVencimento,
                         EnderecoEmpresa = fatura.Empresa.Endereco,
                         IEEmpresa = fatura.Empresa.InscricaoEstadual,
                         Logo = Servicos.Imagem.GetFromPath(fatura.Empresa.CaminhoLogoDacte, System.Drawing.Imaging.ImageFormat.Bmp),
                         NomeEmpresa = fatura.Empresa.RazaoSocial,
                         Numero = fatura.Numero,
                         NumeroEmpresa = fatura.Empresa.Numero,
                         TelefoneEmpresa = fatura.Empresa.Telefone,
                         ValorTotal = fatura.ValorTotal,
                         ValorTotalPorExtenso = Utilidades.Conversor.DecimalToWords(fatura.ValorTotal),
                         BairroTomador = tomador.Bairro,
                         CEPTomador = tomador.CEP,
                         CidadeTomador = tomador.Localidade.DescricaoCidadeEstado,
                         CNPJTomador = tomador.CPF_CNPJ_Formatado,
                         EnderecoTomador = tomador.Endereco,
                         IETomador = tomador.IE_RG,
                         NomeTomador = tomador.Nome,
                         NumeroTomador = tomador.Numero,
                         TelefoneTomador = tomador.Telefone1
                    }
                };

                List<Dominio.ObjetosDeValor.Relatorios.DetalheFaturaAvonDocumento> documentos = (from obj in fatura.Manifestos
                                                                                                 select new Dominio.ObjetosDeValor.Relatorios.DetalheFaturaAvonDocumento()
                                                                                                 {
                                                                                                     CTes = obj.Documentos.Where(o => o.CTe != null).Min(o => o.CTe.Numero) + " - " + obj.Documentos.Where(o => o.CTe != null).Max(o => o.CTe.Numero),
                                                                                                     Destino = obj.TabelaFrete?.CidadeDestino?.DescricaoCidadeEstado ?? string.Empty,
                                                                                                     NumeroManifesto = int.Parse(obj.Numero) + 1,
                                                                                                     NumeroMinuta = int.Parse(obj.Numero),
                                                                                                     ValorICMS = obj.Documentos.Sum(o => o.CTe.ValorICMS),
                                                                                                     ValorTotal = obj.Documentos.Sum(o => o.CTe.ValorAReceber),
                                                                                                     ValorFrete = obj.Documentos.Sum(o => o.CTe.ValorFrete)
                                                                                                 }).ToList();

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeDeTrabalho);

                List<Microsoft.Reporting.WebForms.ReportDataSource> dataSources = new List<Microsoft.Reporting.WebForms.ReportDataSource>();

                dataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Fatura", detalhes));
                dataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Documentos", documentos));

                Dominio.ObjetosDeValor.Relatorios.Relatorio relatorio = svcRelatorio.GerarWeb("Relatorios/DetalheFaturaAvon.rdlc", "PDF", null, dataSources, null);

                return Arquivo(relatorio.Arquivo, relatorio.MimeType, "Fatura_" + fatura.Numero.ToString() + "." + relatorio.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar os detalhes da fatura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #region Métodos Privados

        private void QuitarFatura(int codigoFatura, string stringConexao)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao);

            try
            {
                Repositorio.FaturaAvon repFatura = new Repositorio.FaturaAvon(unidadeDeTrabalho);
                Repositorio.DocumentoManifestoAvon repDocumentoManifesto = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);
                Repositorio.MovimentoDoFinanceiro repMovimentoFinanceiro = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                Repositorio.ParcelaCobrancaCTe repParcelaCTe = new Repositorio.ParcelaCobrancaCTe(unidadeDeTrabalho);

                Dominio.Entidades.FaturaAvon fatura = repFatura.BuscarPorCodigo(codigoFatura);

                if (fatura.Status == Dominio.ObjetosDeValor.Enumerador.StatusFaturaAvon.Paga)
                    return;

                fatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusFaturaAvon.Paga;

                repFatura.Atualizar(fatura);

                int i = 0;

                foreach (Dominio.Entidades.ManifestoAvon manifesto in fatura.Manifestos)
                {
                    List<Dominio.Entidades.DocumentoManifestoAvon> documentosManifesto = repDocumentoManifesto.BuscarPorManifesto(manifesto.Codigo);

                    foreach (Dominio.Entidades.DocumentoManifestoAvon documentoManifesto in documentosManifesto)
                    {
                        if (i == 50)
                        {
                            i = 0;
                            unidadeDeTrabalho.Dispose();
                            repDocumentoManifesto = null;
                            repFatura = null;
                            repMovimentoFinanceiro = null;
                            repParcelaCTe = null;

                            unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao);
                            repDocumentoManifesto = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);
                            repFatura = new Repositorio.FaturaAvon(unidadeDeTrabalho);
                            repMovimentoFinanceiro = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                            repParcelaCTe = new Repositorio.ParcelaCobrancaCTe(unidadeDeTrabalho);
                        }

                        List<Dominio.Entidades.ParcelaCobrancaCTe> parcelas = repParcelaCTe.BuscarPorCTe(documentoManifesto.CTe.Empresa.Codigo, documentoManifesto.CTe.Codigo);

                        foreach (Dominio.Entidades.ParcelaCobrancaCTe parcela in parcelas)
                        {
                            parcela.DataPagamento = DateTime.Now;

                            repParcelaCTe.Atualizar(parcela);

                            Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimentoFinanceiro.BuscarPorParcelaCobrancaCTe(documentoManifesto.CTe.Empresa.Codigo, parcela.Codigo);

                            if (movimento != null)
                            {
                                movimento.DataBaixa = DateTime.Now;
                                movimento.DataPagamento = DateTime.Now;

                                repMovimentoFinanceiro.Atualizar(movimento);
                            }
                        }

                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        private void EnviarFatura(int codigoFatura, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.FaturaAvon repFatura = new Repositorio.FaturaAvon(unidadeDeTrabalho);

            Dominio.Entidades.FaturaAvon fatura = repFatura.BuscarPorCodigo(codigoFatura);

            Servicos.Avon svcAvon = new Servicos.Avon(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = svcAvon.EnviarFatura(this.EmpresaUsuario.CNPJ, this.EmpresaUsuario.Configuracao.TokenIntegracaoAvon, fatura);

            string mensagem = string.Empty;

            if (retorno != null && retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "200")
            {
                if (retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Root != null && retorno.CrossTalk_Body.Root.Document != null && retorno.CrossTalk_Body.Root.Document.Response != null)
                {
                    mensagem = retorno.CrossTalk_Body.Root.Document.Response.InnerCode + " - " + retorno.CrossTalk_Body.Root.Document.Response.Description;

                    if (retorno.CrossTalk_Body.Root.Document.Response.ErrorMessages != null)
                        foreach (string error in retorno.CrossTalk_Body.Root.Document.Response.ErrorMessages)
                            mensagem += " - " + error;

                }
                else if (retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Document != null)
                {
                    mensagem = retorno.CrossTalk_Body.Document[0].Response.InnerCode + " - " + retorno.CrossTalk_Body.Document[0].Response.Description;
                }
                else
                {
                    mensagem = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage;
                }
            }
            else
            {
                mensagem = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage;
            }

            fatura.Mensagem = mensagem;

            repFatura.Atualizar(fatura);
        }

        #endregion
    }
}
