using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ApuracaoICMSController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("apuracaoicms.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

        [AcceptVerbs("POST")]
        public ActionResult ObterValoresParaApuracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão para acesso negada!");

                DateTime dataInicial, dataFinal, dataInicialPeriodoAnterior, dataFinalPeriodoAnterior;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                if (dataInicial.Month != dataFinal.Month || dataInicial.Year != dataFinal.Year)
                    return Json<bool>(false, false, "O mês/ano inicial e final devem ser iguais!");

                dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, 1);
                dataFinal = new DateTime(dataInicial.Year, dataInicial.Month, DateTime.DaysInMonth(dataInicial.Year, dataInicial.Month));
                dataInicialPeriodoAnterior = dataInicial.AddMonths(-1);
                dataFinalPeriodoAnterior = new DateTime(dataFinal.AddMonths(-1).Year, dataFinal.AddMonths(-1).Month, DateTime.DaysInMonth(dataFinal.AddMonths(-1).Year, dataFinal.AddMonths(-1).Month));

                Repositorio.DocumentoEntrada repDocumentoEntrada = new Repositorio.DocumentoEntrada(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.ApuracaoICMS repApuracaoICMS = new Repositorio.ApuracaoICMS(unitOfWork);

                Dominio.Entidades.ApuracaoICMS apuracaoPeriodoAnterior = repApuracaoICMS.BuscarPorPeriodo(this.EmpresaUsuario.Codigo, dataInicialPeriodoAnterior, dataFinalPeriodoAnterior);

                decimal valorCreditos = repDocumentoEntrada.BuscarTotalICMS(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado, new string[] { "01", "1B", "04", "55", "65", "06", "29", "28", "21", "22" });

                Dominio.Enumeradores.TipoAmbiente ambiente = Dominio.Enumeradores.TipoAmbiente.Producao;

#if DEBUG
                ambiente = Dominio.Enumeradores.TipoAmbiente.Homologacao;
#endif

                decimal valorDebitos = repCTe.BuscarTotalICMS(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, new string[] { "A" }, ambiente);

                decimal valorSaldoCredorPeriodoAnterior = apuracaoPeriodoAnterior != null ? apuracaoPeriodoAnterior.ValorSaldoCredorTransportar : 0m;

                decimal valorSaldoCredorTransportar = (valorSaldoCredorPeriodoAnterior + valorCreditos) > valorDebitos ? valorSaldoCredorPeriodoAnterior + valorCreditos - valorDebitos : 0m;

                decimal valorICMSRecolher = valorDebitos > (valorSaldoCredorPeriodoAnterior + valorCreditos) ? valorDebitos - (valorSaldoCredorPeriodoAnterior + valorCreditos) : 0m;

                return Json(new
                {
                    PossuiApuracaoPeriodoAnterior = apuracaoPeriodoAnterior == null ? false : true,
                    ValorCreditos = valorCreditos.ToString("n2"),
                    ValorDebitos = valorDebitos.ToString("n2"),
                    ValorSaldoCredorPeriodoAnterior = valorSaldoCredorPeriodoAnterior.ToString("n2"),
                    ValorSaldoCredorTransportar = valorSaldoCredorTransportar.ToString("n2"),
                    ValorICMSRecolher = valorICMSRecolher.ToString("n2"),
                    DataInicial = dataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = dataFinal.ToString("dd/MM/yyyy")
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os valores para a apuração do ICMS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                decimal saldoCredorPeriodoAnterior, valorCreditos, valorDebitos, valorICMSRecolher, saldoCredorTransportar;
                decimal.TryParse(Request.Params["SaldoCredorPeriodoAnterior"], out saldoCredorPeriodoAnterior);
                decimal.TryParse(Request.Params["ValorCreditos"], out valorCreditos);
                decimal.TryParse(Request.Params["ValorDebitos"], out valorDebitos);
                decimal.TryParse(Request.Params["ValorICMSRecolher"], out valorICMSRecolher);
                decimal.TryParse(Request.Params["SaldoCredorTransportar"], out saldoCredorTransportar);

                if (dataInicial.Month != dataFinal.Month || dataInicial.Year != dataFinal.Year || dataInicial == DateTime.MinValue || dataFinal == DateTime.MinValue)
                    return Json<bool>(false, false, "Datas inválidas! O mês/ano da data inicial e final devem ser os mesmos.");

                dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, 1);
                dataFinal = new DateTime(dataInicial.Year, dataInicial.Month, DateTime.DaysInMonth(dataInicial.Year, dataInicial.Month));

                Repositorio.ApuracaoICMS repApuracaoICMS = new Repositorio.ApuracaoICMS(unitOfWork);

                Dominio.Entidades.ApuracaoICMS apuracaoICMS = repApuracaoICMS.BuscarPorPeriodo(this.EmpresaUsuario.Codigo, dataInicial, dataFinal);

                if (apuracaoICMS == null)
                    apuracaoICMS = new Dominio.Entidades.ApuracaoICMS();

                apuracaoICMS.DataFinal = dataFinal;
                apuracaoICMS.DataInicial = dataInicial;
                apuracaoICMS.Empresa = this.EmpresaUsuario;
                apuracaoICMS.ValorCreditos = valorCreditos;
                apuracaoICMS.ValorCreditosPeriodoAnterior = saldoCredorPeriodoAnterior;
                apuracaoICMS.ValorDebitos = valorDebitos;
                apuracaoICMS.ValorICMSRecolher = valorICMSRecolher;
                apuracaoICMS.ValorSaldoCredorTransportar = saldoCredorTransportar;

                if (apuracaoICMS.Codigo > 0)
                    repApuracaoICMS.Atualizar(apuracaoICMS);
                else
                    repApuracaoICMS.Inserir(apuracaoICMS);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a apuração do ICMS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados



        #endregion
    }
}
