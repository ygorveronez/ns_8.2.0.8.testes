using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class VencimentoCertificadoController : ApiController
    {

        #region Variáveis Globais
        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("controlecertificados.aspx") select obj).FirstOrDefault();
        }
        #endregion

        [AcceptVerbs("POST")]
        public ActionResult Consulta()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.VencimentoCertificado repVencimentoCertificado = new Repositorio.VencimentoCertificado(unidadeDeTrabalho);

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["DiasVencimento"], out int diasVencimento);

                string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);
                string nome = Request.Params["Nome"];
                string ambiente = Request.Params["Ambiente"];
                string status = Request.Params["Status"];
                string cidade = Request.Params["Cidade"];

                DateTime.TryParse(Request.Params["DataInicio"], out DateTime dataInicio);
                DateTime.TryParse(Request.Params["DataFim"], out DateTime dataFim);

                bool comContato = false;
                Dominio.Enumeradores.StatusVendaCertificado? statusVenda = null;
                if (Enum.TryParse(Request.Params["StatusVenda"], out Dominio.Enumeradores.StatusVendaCertificado statusVendaAux))
                    statusVenda = statusVendaAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? satisfacao = null;
                if (Enum.TryParse(Request.Params["Satisfacao"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao satisfacaoAux))
                    satisfacao = satisfacaoAux;

                if (Request.Params["StatusVenda"] == "!" + Dominio.Enumeradores.StatusVendaCertificado.SemContato.ToString("d"))
                {
                    comContato = true;
                    statusVenda = null;
                }

                IList<Dominio.ObjetosDeValor.ConsultaVencimentoCertificado> lista = repVencimentoCertificado.Consultar(cnpj, nome, ambiente, status, cidade, statusVenda, comContato, diasVencimento, dataInicio, dataFim, satisfacao, inicioRegistros, 50);
                int countRegistros = repVencimentoCertificado.ContarConsulta(cnpj, nome, ambiente, status, cidade, statusVenda, comContato, diasVencimento, dataInicio, dataFim, satisfacao);

                var retorno = (from certificado in lista
                               select new
                               {
                                   Satisfacao = certificado._Satisfacao,
                                   certificado.Cnpj,
                                   certificado.Nome,
                                   Localidade = certificado.Localidade ?? string.Empty,
                                   Vencimento = certificado.Vencimento.ToString("dd/MM/yyyy"),
                                   EmpresaAdmin = certificado.EmpresaAdmin ?? string.Empty,
                                   certificado.Email,
                                   certificado.Telefone,
                                   certificado.DescricaoStatusVenda,
                                   certificado.DescricaoSatisfacao,
                                   certificado.Ambiente
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Satisfacao", "CNPJ|5", "Nome|12", "Cidade/UF|9", "Vencimento|5", "Admin|10", "Email|15", "Telefone|7", "Status Vendas|7", "Satisfação|7", "Ambiente|25" }, countRegistros);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as vencimentos. Tente novamente!");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        [AcceptVerbs("GET")]
        public ActionResult ExportarConsulta()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.VencimentoCertificado repVencimentoCertificado = new Repositorio.VencimentoCertificado(unidadeDeTrabalho);

                int.TryParse(Request.Params["DiasVencimento"], out int diasVencimento);

                string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);
                string nome = Request.Params["Nome"];
                string ambiente = Request.Params["Ambiente"];
                string status = Request.Params["Status"];
                string cidade = Request.Params["Cidade"];

                DateTime.TryParse(Request.Params["DataInicio"], out DateTime dataInicio);
                DateTime.TryParse(Request.Params["DataFim"], out DateTime dataFim);

                bool comContato = false;
                Dominio.Enumeradores.StatusVendaCertificado? statusVenda = null;
                if (Enum.TryParse(Request.Params["StatusVenda"], out Dominio.Enumeradores.StatusVendaCertificado statusVendaAux))
                    statusVenda = statusVendaAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? satisfacao = null;
                if (Enum.TryParse(Request.Params["Satisfacao"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao satisfacaoAux))
                    satisfacao = satisfacaoAux;

                if (Request.Params["StatusVenda"] == "!" + Dominio.Enumeradores.StatusVendaCertificado.SemContato.ToString("d"))
                {
                    comContato = true;
                    statusVenda = null;
                }


                IList<Dominio.ObjetosDeValor.ConsultaVencimentoCertificado> lista = repVencimentoCertificado.Consultar(cnpj, nome, ambiente, status, cidade, statusVenda, comContato, diasVencimento, dataInicio, dataFim, satisfacao, 0, 0);

                List<string> dados = (from certificado in lista select
                                            String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(certificado.Cnpj)) + ";" +
                                            certificado.Nome.Replace("&amp;","&").Replace(";"," ") + ";" +
                                            (certificado.Localidade ?? string.Empty) + ";" +
                                            certificado.Vencimento.ToString("dd/MM/yyyy") + ";" +
                                            (certificado.EmpresaAdmin ?? string.Empty) + ";" +
                                            certificado.Email.Replace(";", " ") + ";" +
                                            certificado.Telefone + ";" +
                                            certificado.DescricaoStatusVenda + ";" +
                                            certificado.Ambiente
                                       ).ToList();

                List<string> cabecalho = new List<string>(){
                    "CNPJ", "Nome", "Cidade/UF", "Vencimento", "Admin", "Email", "Telefone", "Status Vendas", "Ambiente"
                };

                var retorno = MontarCSV(cabecalho, dados);

                return Arquivo(retorno, "text/csv", "Controle Certificados.csv");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as vencimentos. Tente novamente!");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultaDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                DateTime.TryParseExact(Request.Params["Vencimento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime vencimento);
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);

                Repositorio.VencimentoCertificadoHistorico repVencimentoCertificadoHistorico = new Repositorio.VencimentoCertificadoHistorico(unidadeDeTrabalho);

                IList<Dominio.ObjetosDeValor.ConsultaVencimentoCertificadoDetalhes> lista = repVencimentoCertificadoHistorico.Consultar(cnpj, vencimento, inicioRegistros, 50);
                int countRegistros = repVencimentoCertificadoHistorico.ContarConsulta(cnpj, vencimento);

                var retorno = (from certificado in lista
                               select new
                               {
                                   certificado.Detalhes,
                                   Satisfacao = certificado.DescricaoSatisfacao,
                                   DataLancamento = certificado.DataLancamento.ToString("dd/MM/yyyy"),
                                   Tipo = certificado.DescricaoTipo,
                                   StatusVenda = certificado.DescricaoStatusVenda,
                                   certificado.Usuario,
                                   Previa = MontarPreviaDetalhes(certificado.Detalhes ?? string.Empty),
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Detalhes", "Satisfacao", "Data|10", "Tipo|13", "Status|20", "Usuário|15", "Prévia|30" }, countRegistros);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as vencimentos. Tente novamente!");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult LancamentoHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.VencimentoCertificado repVencimentoCertificado = new Repositorio.VencimentoCertificado(unitOfWork);

                string detalhes = Request.Params["Detalhes"] ?? string.Empty;

                Dominio.Enumeradores.StatusVendaCertificado? statusVenda = null;
                if (Enum.TryParse(Request.Params["StatusVenda"], out Dominio.Enumeradores.StatusVendaCertificado statusVendaAux))
                    statusVenda = statusVendaAux;

                Enum.TryParse(Request.Params["Satisfacao"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao satisfacao);

                List<Dominio.Entidades.VencimentoCertificado> certificados = BuscarVencimentoCertificado(unitOfWork);
                
                if (certificados.Count() == 0)
                    return Json<bool>(false, false, "Não foi encontrado os dados");

                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para lançamento negada!");

                ReplicarHistorico(certificados, detalhes, statusVenda, Dominio.Enumeradores.TipoHistorico.Informacao, satisfacao, null, unitOfWork);
                
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult AtualizarHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.VencimentoCertificado repVencimentoCertificado = new Repositorio.VencimentoCertificado(unitOfWork);

                string detalhes = Request.Params["Detalhes"] ?? string.Empty;
                List<Dominio.Entidades.VencimentoCertificado> certificados = BuscarVencimentoCertificado(unitOfWork);

                if (certificados.Count() == 0)
                    return Json<bool>(false, false, "Não foi encontrado os dados");

                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para lançamento negada!");

                string status = "A";
                ReplicarHistorico(certificados, detalhes, Dominio.Enumeradores.StatusVendaCertificado.Atualizado, Dominio.Enumeradores.TipoHistorico.Atualizacao, null, status, unitOfWork);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult InativarCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.VencimentoCertificado repVencimentoCertificado = new Repositorio.VencimentoCertificado(unitOfWork);

                string detalhes = Request.Params["Detalhes"] ?? string.Empty;
                List<Dominio.Entidades.VencimentoCertificado> certificados = BuscarVencimentoCertificado(unitOfWork);

                if (certificados.Count() == 0)
                    return Json<bool>(false, false, "Não foi encontrado os dados");

                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para lançamento negada!");

                string status = "I";
                ReplicarHistorico(certificados, detalhes, Dominio.Enumeradores.StatusVendaCertificado.Inativado, Dominio.Enumeradores.TipoHistorico.Inativacao, null, status, unitOfWork);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void ReplicarHistorico(List<Dominio.Entidades.VencimentoCertificado> certificados, string detalhes, Dominio.Enumeradores.StatusVendaCertificado? statusVenda, Dominio.Enumeradores.TipoHistorico tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? satisfacao, string status, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.VencimentoCertificado repVencimentoCertificado = new Repositorio.VencimentoCertificado(unitOfWork);
            Repositorio.VencimentoCertificadoHistorico repVencimentoCertificadoHistorico = new Repositorio.VencimentoCertificadoHistorico(unitOfWork);

            DateTime timeNow = DateTime.Now;

            foreach (Dominio.Entidades.VencimentoCertificado certificado in certificados)
            {
                if(!string.IsNullOrWhiteSpace(status))
                    certificado.Status = status;

                if (satisfacao.HasValue && satisfacao.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao.NaoAvaliado)
                    certificado.NivelSatisfacao = satisfacao;

                certificado.StatusVenda = statusVenda;
                repVencimentoCertificado.Atualizar(certificado);

                Dominio.Entidades.VencimentoCertificadoHistorico historico = new Dominio.Entidades.VencimentoCertificadoHistorico()
                {
                    VencimentoCertificado = certificado,
                    DataHora = timeNow,
                    Usuario = this.Usuario,
                    Detalhes = detalhes,
                    Tipo = tipo,
                    StatusVenda = statusVenda,
                    NivelSatisfacao = satisfacao
                };
                repVencimentoCertificadoHistorico.Inserir(historico);
            }
        }

        private List<Dominio.Entidades.VencimentoCertificado> BuscarVencimentoCertificado(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.VencimentoCertificado repVencimentoCertificado = new Repositorio.VencimentoCertificado(unitOfWork);

            string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);
            DateTime.TryParseExact(Request.Params["Vencimento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime vencimento);

            return repVencimentoCertificado.BuscarPorCNPJeVencimento(cnpj, vencimento);
        }

        private string MontarPreviaDetalhes(string detalhes)
        {
            if (detalhes.Length <= 50)
                return detalhes;

            return detalhes.Substring(0, 50) + "...";
        }

        private System.IO.Stream MontarCSV(List<string> cabecalho, List<string> dados)
        {
            string csv = String.Join(";", cabecalho) + Environment.NewLine;
            csv += String.Join(Environment.NewLine, dados);
            
            return Utilidades.String.ToStream(csv);
        }
    }
}
