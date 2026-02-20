using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class SPEDController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("sped.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST", "GET")]
        public ActionResult GerarSPED()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para a geração do SPED Fiscal.");

                DateTime dataInicial = DateTime.MinValue;
                DateTime dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                if (dataInicial.Month != dataFinal.Month || dataInicial.Year != dataFinal.Year || dataInicial == DateTime.MinValue || dataFinal == DateTime.MinValue)
                    return Json<bool>(false, false, "Datas inválidas! O mês/ano da data inicial e final devem ser os mesmos.");

                dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, 1);
                dataFinal = new DateTime(dataInicial.Year, dataInicial.Month, DateTime.DaysInMonth(dataInicial.Year, dataInicial.Month));

                string atoCOTEPE = Request.Params["AtoCOTEPE"];

                bool gerarD160 = false;
                bool.TryParse(Request.Params["GerarD160"], out gerarD160);

                List<string> naoGerar = new List<string>();

                if (!gerarD160)
                    naoGerar.Add("D160");

                MultiSoftware.EFD.SPED sped = new MultiSoftware.EFD.SPED(this.EmpresaUsuario, dataInicial, dataFinal, atoCOTEPE, naoGerar, Conexao.StringConexao, unitOfWork);

                return Arquivo(sped.GerarSPED(), "text/plain", string.Concat("SPEDFiscal_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"), ".txt"));

                //Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                //Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(Conexao.StringConexao);
                //List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.BuscarTodosPorStatus(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, new string[] { "A", "C", "D", "I" }, this.EmpresaUsuario.TipoAmbiente, string.Empty, 0);
                //return Arquivo(svcCTe.GerarEDIPipes(listaCTes), "text/plain", string.Concat("EDI_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"), ".txt"));

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o SPED Fiscal.");
            }
        }
    }
}
