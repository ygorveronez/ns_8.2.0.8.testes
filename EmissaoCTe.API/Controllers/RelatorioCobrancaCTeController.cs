using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioCobrancaCTeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult ConsultarCTes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params["InicioRegistros"], out int inicioRegistros);
                int limiteRegistros = 50;

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                int countCTes = 0;

                ExecutaPesquisa(ref listaCTes, ref countCTes, inicioRegistros, limiteRegistros, unidadeDeTrabalho);

                var retorno = (from cte in listaCTes
                               select new
                               {
                                   cte.Codigo,
                                   Numero = cte.Numero.ToString(),
                                   Serie = cte.Serie.Numero.ToString(),
                                   DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   Remetente = cte.Remetente != null ? cte.Remetente.Nome : string.Empty,
                                   LocalidadeRemetente = cte.Remetente != null ? cte.Remetente.Exterior ? string.Concat(cte.Remetente.Cidade, " / ", cte.Remetente.Pais.Nome) : string.Concat(cte.Remetente.Localidade.Estado.Sigla, " / ", cte.Remetente.Localidade.Descricao) : string.Empty,
                                   Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome : string.Empty,
                                   LocalidadeDestinatario = cte.Destinatario != null ? cte.Destinatario.Exterior ? string.Concat(cte.Destinatario.Cidade, " / ", cte.Destinatario.Pais.Nome) : string.Concat(cte.Destinatario.Localidade.Estado.Sigla, " / ", cte.Destinatario.Localidade.Descricao) : string.Empty,
                                   Valor = string.Format("{0:n2}", cte.ValorAReceber),
                                   cte.DescricaoStatus
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Núm.|8", "Série|8", "Emissão|8", "Remetente|13", "Loc. Remet.|13", "Destinatário|13", "Loc. Destin.|13", "Valor|8", "Status|8" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string nomeSessao = Request.Params["Sessao"];
                List<int> codigosCTes = JsonConvert.DeserializeObject<List<int>>(Session[nomeSessao].ToString());

                DateTime dataEmissao, dataVencimento;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                DateTime.TryParseExact(Request.Params["DataVencimento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

                double cpfCnpjTomador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Tomador"]), out cpfCnpjTomador);

                string numero = Request.Params["Numero"];
                string observacao = Request.Params["Observacao"];
                string tipoArquivo = Request.Params["TipoArquivo"];

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigosCTes.ToArray());

                decimal valor = (from obj in ctes select obj.ValorAReceber).Sum();

                Dominio.ObjetosDeValor.Relatorios.CobrancaCTe cobranca = new Dominio.ObjetosDeValor.Relatorios.CobrancaCTe()
                {
                    BairroEmpresa = this.EmpresaUsuario.Bairro,
                    BairroTomador = cliente.Bairro,
                    CEPEmpresa = this.EmpresaUsuario.CEP,
                    CidadeEmpresa = this.EmpresaUsuario.Localidade.Descricao,
                    CidadeTomador = cliente.Localidade.Descricao,
                    CNPJEmpresa = this.EmpresaUsuario.CNPJ,
                    CNPJTomador = cliente.CPF_CNPJ_Formatado,
                    DataEmissao = dataEmissao != DateTime.MinValue ? dataEmissao.ToString("dd/MM/yyyy") : string.Empty,
                    DataVencimento = dataVencimento != DateTime.MinValue ? dataVencimento.ToString("dd/MM/yyyy") : string.Empty,
                    EnderecoTomador = cliente.Endereco,
                    EstadoTomador = cliente.Localidade.Estado.Sigla,
                    EstadoEmpresa = this.EmpresaUsuario.Localidade.Estado.Sigla,
                    IEEmpresa = this.EmpresaUsuario.InscricaoEstadual,
                    IETomador = cliente.IE_RG,
                    Logo = Servicos.Imagem.GetFromPath(this.EmpresaUsuario.CaminhoLogoDacte, System.Drawing.Imaging.ImageFormat.Bmp),
                    LogradouroEmpresa = this.EmpresaUsuario.Endereco,
                    NomeEmpresa = this.EmpresaUsuario.RazaoSocial,
                    NomeTomador = cliente.Nome,
                    Numero = numero,
                    NumeroEmpresa = this.EmpresaUsuario.Numero,
                    NumeroTomador = cliente.Numero,
                    Observacao = observacao,
                    TelefoneEmpresa = this.EmpresaUsuario.Telefone,
                    Valor = valor,
                    ValorExtenso = "***" + Utilidades.Conversor.DecimalToWords(valor) + "***"
                };

                //List<ReportParameter> parametros = new List<ReportParameter>();
                //parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                //parametros.Add(new ReportParameter("InscricaoEstadual", this.EmpresaUsuario.InscricaoEstadual));
                //parametros.Add(new ReportParameter("CNPJ", this.EmpresaUsuario.CNPJ_Formatado));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Cobranca", new List<Dominio.ObjetosDeValor.Relatorios.CobrancaCTe>() { cobranca }));
                dataSources.Add(new ReportDataSource("Documentos", (from obj in ctes
                                                                    select new Dominio.ObjetosDeValor.Relatorios.CobrancaCTeDocumento()
                                                                    {
                                                                        Numero = obj.Numero,
                                                                        Serie = obj.Serie.Numero,
                                                                        Valor = obj.ValorAReceber,
                                                                        DataEmissao = obj.DataEmissao.Value,
                                                                        NotaFiscal = obj.NumeroNotas,
                                                                        Origem = obj.LocalidadeInicioPrestacao.Descricao + " - " + obj.LocalidadeInicioPrestacao.Estado.Sigla,
                                                                        Destino = obj.LocalidadeTerminoPrestacao.Descricao + " - " + obj.LocalidadeTerminoPrestacao.Estado.Sigla,
                                                                        Destinatario = obj.Recebedor?.Descricao ?? obj.Destinatario?.Descricao ?? string.Empty,
                                                                        ValorICMS = obj.ValorICMS
                                                                    }).ToList()));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCobrancaCTe.rdlc", tipoArquivo, null, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioCobrancaCTe." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }
        [AcceptVerbs("POST")]

        public ActionResult SelecionarTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params["InicioRegistros"], out int inicioRegistros);
                int limiteRegistros = 0;

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                int countCTes = 0;

                ExecutaPesquisa(ref listaCTes, ref countCTes, inicioRegistros, limiteRegistros, unidadeDeTrabalho);

                var retorno = (from cte in listaCTes
                               select new
                               {
                                   cte.Codigo,
                                   Numero = cte.Numero.ToString(),
                                   Serie = cte.Serie.Numero.ToString(),
                                   Valor = string.Format("{0:n2}", cte.ValorAReceber),
                               }).ToList();

                return Json(new
                {
                    CTes = retorno
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult PreparaGerarao()
        {
            /* Devido a um problema na selecao de muitos CTes, o tamanho do GET excedia o limite
             * 
             * Para solução do problema:
             * - A requisição do download é enviada via POST
             * - Informações são salvas numa sessão
             * - Retorna o nome da sessão
             * - Método do download envia essa sessão
             */
            string idsRequisicao = Request.Params["CTes"];
            string nomeSessao = "CobrancaCTe" + DateTime.Now.ToString("HHmmss");

            Session[nomeSessao] = idsRequisicao;

            return Json(nomeSessao, true);
        }

        private void ExecutaPesquisa(ref List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes, ref int countCTes, int inicioRegistros, int limiteRegistros, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            int.TryParse(Request.Params["NumeroInicial"], out int numeroInicial);
            int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);

            string cpfCnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Tomador"]);

            DateTime dataEmissaoInicial, dataEmissaoFinal;
            DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
            DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            countCTes = repCTe.ContarConsulta(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, cpfCnpjTomador, "A");
            listaCTes = repCTe.Consultar(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, cpfCnpjTomador, "A", inicioRegistros, limiteRegistros);
        }
    }
}