using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class SPEDContribuicoesController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("spedcontribuicoes.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public ActionResult GerarSPED()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para a geração do SPED Contribuições.");

                if (this.EmpresaUsuario.Configuracao == null)
                    return Json<bool>(false, false, "As configurações da empresa são inválidas para a geração do SPED Contribuições.");

                string numeroReciboAnterior = Request.Params["NumeroReciboAnterior"];

                DateTime dataInicial = DateTime.MinValue;
                DateTime dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.Enumeradores.TipoEscrituracao tipoEscrituracao;
                Enum.TryParse<Dominio.Enumeradores.TipoEscrituracao>(Request.Params["TipoEscrituracao"], out tipoEscrituracao);

                Dominio.Enumeradores.IndicadorDeSituacaoEspecial indicadorSituacaoEspecial;
                Enum.TryParse<Dominio.Enumeradores.IndicadorDeSituacaoEspecial>(Request.Params["IndicadorSituacaoEspecial"], out indicadorSituacaoEspecial);

                Dominio.Enumeradores.IndicadorDeMetodoDeApropriacaoDeCreditosComuns indicadorApropriacaoCreditos;
                Enum.TryParse<Dominio.Enumeradores.IndicadorDeMetodoDeApropriacaoDeCreditosComuns>(Request.Params["IndicadorApropriacaoCreditos"], out indicadorApropriacaoCreditos);

                Dominio.Enumeradores.IndicadorDoTipoDeContribuicaoApuradaNoPeriodo indicadorTipoContribuicao;
                Enum.TryParse<Dominio.Enumeradores.IndicadorDoTipoDeContribuicaoApuradaNoPeriodo>(Request.Params["IndicadorTipoContribuicao"], out indicadorTipoContribuicao);

                if (dataInicial.Month != dataFinal.Month || dataInicial.Year != dataFinal.Year || dataInicial == DateTime.MinValue || dataFinal == DateTime.MinValue)
                    return Json<bool>(false, false, "Datas inválidas! O mês/ano da data inicial e final devem ser os mesmos.");

                dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, 1);
                dataFinal = new DateTime(dataInicial.Year, dataInicial.Month, DateTime.DaysInMonth(dataInicial.Year, dataInicial.Month));

                string atoCOTEPE = Request.Params["AtoCOTEPE"];

                MultiSoftware.EFD.SPEDContribuicoes sped = new MultiSoftware.EFD.SPEDContribuicoes(this.EmpresaUsuario, dataInicial, dataFinal, atoCOTEPE, indicadorSituacaoEspecial, Dominio.Enumeradores.TipoDeAtividadeSPEDContribuicoes.PrestadorDeServicos, tipoEscrituracao, indicadorApropriacaoCreditos, indicadorTipoContribuicao, unitOfWork);

                return Arquivo(sped.GerarSPED(), "text/plain", string.Concat("SPEDContribuicoes_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o SPED Contribuições.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
