using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace EmissaoCTe.API.Controllers
{
    public class CTesReferenciaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("ctesreferencia.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["fimRegistros"], out int fimRegistros);

                if (fimRegistros == 0)
                    fimRegistros = 20;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.BuscarCTesReferencia(this.EmpresaUsuario.Codigo, inicioRegistros, fimRegistros);
                int countCTes = repCTe.ContarCTesReferencia(this.EmpresaUsuario.Codigo);

                if (System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"] == "Sintravir")
                {
                    var retorno = (from cte in listaCTes
                                   select new
                                   {
                                       cte.Codigo,
                                       cte.Numero,
                                       Serie = cte.Serie.Numero,
                                       cte.DescricaoTipoServico,
                                       Remetente = cte.Remetente != null ? cte.Remetente.Nome + " (" + cte.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                       Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome + " (" + cte.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                                       Expedidor = cte.Expedidor != null ? cte.Expedidor.Nome + " (" + cte.Expedidor.CPF_CNPJ_Formatado + ")" : string.Empty,
                                       Recebedor = cte.Recebedor != null ? cte.Recebedor.Nome + " (" + cte.Recebedor.CPF_CNPJ_Formatado + ")" : string.Empty,
                                       Origem = cte.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                                       Destino = cte.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                                       Tomador = cte.DescricaoTipoTomador,
                                       CST = cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? "SN" : cte.CST,
                                       AliquotaICMS = cte.AliquotaICMS.ToString("n2"),
                                       IncluirICMS = cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? "Sim" : "Não"
                                   }).ToList();


                    return Json(retorno, true, null, new string[] { "Codigo", "Núm.|6", "Série|4", "Tipo|6", "Remetente|10", "Destinatário|10", "Expedidor|6", "Recebedor|6", "Origem|4", "Destino|4", "Tomador|10", "CST|6", "Aliq. ICMS|8", "Incluir ICMS|10" }, countCTes);

                }
                else
                {
                    var retorno = (from cte in listaCTes
                                   select new
                                   {
                                       cte.Codigo,
                                       cte.Numero,
                                       Serie = cte.Serie.Numero,
                                       cte.DescricaoTipoServico,
                                       Remetente = cte.Remetente != null ? cte.Remetente.Nome + " (" + cte.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                       UFOrigem = cte.LocalidadeInicioPrestacao?.Estado.Sigla ?? string.Empty,
                                       UFDestino = cte.LocalidadeTerminoPrestacao?.Estado.Sigla ?? string.Empty,
                                       Tomador = cte.DescricaoTipoTomador,
                                       CST = cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? "SN" : cte.CST,
                                       AliquotaICMS = cte.AliquotaICMS.ToString("n2"),
                                       IncluirICMS = cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? "Sim" : "Não"
                                   }).ToList();

                    return Json(retorno, true, null, new string[] { "Codigo", "Núm.|6", "Série|4", "Tipo|6", "Remetente|20", "UF Origem|9", "UF Destino|9", "Tomador|10", "CST|6", "Aliq. ICMS|10", "Incluir ICMS|10" }, countCTes);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult AdicionarCTeReferencia()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao!= "A")
                    return Json<bool>(false, false, "Permissão para adicionar negada!");

                int.TryParse(Request.Params["CodigoCTe"], out int codigoCTe);

                if (codigoCTe == 0)
                    return Json<bool>(false, false, "CT-e não selecionado.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                
                if (cte == null)
                    return Json<bool>(false, false, "CT-e não localizado");

                //Validar se já existe outro CTe com mesmo Cliente, Origem e destino
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteValidacao = null;

                if (System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"] == "Sintravir")
                    cteValidacao = repCTe.BuscarReferenciaSintravir(cte.Empresa.Codigo, cte.Remetente.CPF_CNPJ, cte.Destinatario?.CPF_CNPJ, cte.Expedidor?.CPF_CNPJ, cte.Recebedor?.CPF_CNPJ, cte.LocalidadeInicioPrestacao.Codigo, cte.LocalidadeTerminoPrestacao.Codigo, cte.TipoServico);
                else
                    cteValidacao = repCTe.BuscarReferenciaPorClienteOrigemDestino(cte.Empresa.Codigo, cte.Remetente?.CPF_CNPJ ?? string.Empty, cte.LocalidadeInicioPrestacao.Estado.Sigla, cte.LocalidadeTerminoPrestacao.Estado.Sigla, cte.TipoServico);

                if (cteValidacao != null)
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"] == "Sintravir")
                        return Json<bool>(false, false, "CT-e " + cteValidacao.Numero + "-" + cte.Serie.Numero + " já configurado como referência.");
                    else
                        return Json<bool>(false, false, "CT-e " + cteValidacao.Numero + "-" + cte.Serie.Numero + " já configurado como referência com mesmo Remetente, UF Origem, UF Destino e Tipo.");
                }
                    

                cte.ReferenciaEmissao = true;
                repCTe.Atualizar(cte);

                return Json<bool>(true, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult RemoverCTeReferencia()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeDelecao != "A")
                    return Json<bool>(false, false, "Permissão para remover negada!");

                int.TryParse(Request.Params["CodigoCTe"], out int codigoCTe);

                if (codigoCTe == 0)
                    return Json<bool>(false, false, "CT-e não selecionado.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return Json<bool>(false, false, "CT-e não localizado");

                cte.ReferenciaEmissao = false;
                repCTe.Atualizar(cte);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao remover.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarPorTipo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                int numero = 0;
                int.TryParse(Request.Params["Numero"], out numero);

                DateTime dataEmissao;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                Dominio.Enumeradores.TipoCTE? tipoCTe = null;
                Dominio.Enumeradores.TipoCTE tipoCTeAux;
                if (Enum.TryParse(Request.Params["TipoCTe"], out tipoCTeAux))
                    tipoCTe = tipoCTeAux;

                string numeroDocumento = Request.Params["NumeroDocumento"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarPorTipo(this.EmpresaUsuario.Codigo, dataEmissao, numero, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Select(o => o.Codigo).ToArray(), numeroDocumento, inicioRegistros, 50, "");
                int countCTes = repCTe.ContarConsultaPorTipo(this.EmpresaUsuario.Codigo, dataEmissao, numero, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Select(o => o.Codigo).ToArray(), numeroDocumento);

                unidadeDeTrabalho.CommitChanges();

                var retorno = (from obj in listaCTes
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   Serie = obj.Serie.Numero,
                                   obj.DescricaoTipoServico,
                                   Remetente = obj.Remetente != null ? obj.Remetente.Nome + " (" + obj.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                   UFOrigem = obj.LocalidadeInicioPrestacao?.Estado.Sigla ?? string.Empty,
                                   UFDestino = obj.LocalidadeTerminoPrestacao?.Estado.Sigla ?? string.Empty,
                                   Tomador = obj.DescricaoTipoTomador,
                                   CST = obj.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? "SN" : obj.CST
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Núm.|10", "Série|6", "Tipo|10", "Remetente|20", "UF Origem|10", "UF Destino|10", "Tomador|12", "CST|6" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeDeTrabalho.Rollback();

                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion
    }
}