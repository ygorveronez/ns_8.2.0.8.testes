using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioManifestosEmitidosAvonController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("relatoriomanifestosemitidosavon.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão de acesso negada!");

                int codigoMotorista, codigoVeiculo;
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                DateTime dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                Dominio.Enumeradores.StatusManifestoAvon? status;
                Dominio.Enumeradores.StatusManifestoAvon statusAux;
                if (!Enum.TryParse<Dominio.Enumeradores.StatusManifestoAvon>(Request.Params["Status"], out statusAux))
                    status = null;
                else
                    status = statusAux;

                string numeroManifesto = Request.Params["NumeroManifesto"];
                string tipoArquivo = Request.Params["TipoArquivo"];

                Repositorio.ManifestoAvon repManifestoAvon = new Repositorio.ManifestoAvon(unidadeDeTrabalho);
                
                List<Dominio.ObjetosDeValor.Relatorios.RelatorioManifestosEmitidosAvon> listaManifestos = repManifestoAvon.Relatorio(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, codigoMotorista, codigoVeiculo, numeroManifesto, status);
                
                for (var i = listaManifestos.Count - 1; i >= 0; i--)
                {
                    listaManifestos[i].ValorPedagio = 0;//Tombini não utiliza valor do pedágio nas minutas da Avon  repManifestoAvon.BuscarValorTotalPedagioCTes(listaManifestos[i].CodigoManifesto);
                    listaManifestos[i].NumeroFatura = repManifestoAvon.BuscarNumeroFatura(listaManifestos[i].CodigoManifesto);
                    listaManifestos[i].PesoCargaCTe = repManifestoAvon.BuscarPesoTotalCTes(listaManifestos[i].CodigoManifesto);

                    if (string.IsNullOrEmpty(listaManifestos[i].CidadeDestino) && string.IsNullOrEmpty(listaManifestos[i].UFDestino))
                    {
                        KeyValuePair<string, string> destino = repManifestoAvon.BuscarDestinoManifesto(listaManifestos[i].CodigoManifesto);

                        listaManifestos[i].CidadeDestino = (string)destino.Key;
                        listaManifestos[i].UFDestino = (string)destino.Value;
                    }
                }
                
                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataEmissaoInicial != DateTime.MinValue ? dataEmissaoInicial.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataEmissaoFinal != DateTime.MinValue ? dataEmissaoFinal.ToString("dd/MM/yyyy") : "99/99/9999")));
                
                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("ManifestosEmitidos", listaManifestos));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeDeTrabalho);
                
                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioManifestosEmitidosAvon.rdlc", tipoArquivo, parametros, dataSources);
                
                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioManifestosEmitidosAvon." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório de manifestos emitidos.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
