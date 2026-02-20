using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EntregaMotoristaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("entregamotorista.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                bool finalizados = false;
                bool.TryParse(Request.Params["Status"], out finalizados);

                Repositorio.CTeEntrega repCTeEntrega = new Repositorio.CTeEntrega(unidadeDeTrabalho);

                dynamic ctes = repCTeEntrega.ConsultarPorMotorista(this.EmpresaUsuario.Codigo, this.Usuario.Codigo, finalizados, inicioRegistros, 50);

                int countCTes = repCTeEntrega.ContarConsultaPorMotorista(this.EmpresaUsuario.Codigo, this.Usuario.Codigo, finalizados);

                return Json(ctes, true, null, new string[] { "Codigo", "Entrega|15", "CT-e|20", "Destino|45" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult RegistrarOcorrencia()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para inclusão negada.");

                int codigoCTe, codigoOcorrencia;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["CodigoOcorrencia"], out codigoOcorrencia);

                DateTime dataOcorrencia;
                DateTime.TryParseExact(Request.Params["DataOcorrencia"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataOcorrencia);

                string observacao = Request.Params["Observacao"];

                Repositorio.CTeEntrega repCTeEntrega = new Repositorio.CTeEntrega(unidadeDeTrabalho);
                Repositorio.Entrega repEntrega = new Repositorio.Entrega(unidadeDeTrabalho);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);
                Repositorio.OcorrenciaDeCTe repOcorrencia = new Repositorio.OcorrenciaDeCTe(unidadeDeTrabalho);

                Dominio.Entidades.CTeEntrega cteEntrega = repCTeEntrega.BuscarPorCTe(codigoCTe);

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(codigoOcorrencia);

                if (cteEntrega == null)
                    return Json<bool>(false, false, "Entrega não encontrada. Atualize a página e tente novamente.");

                if (tipoOcorrencia == null)
                    return Json<bool>(false, false, "Ocorrência não encontrada. Atualize a página e tente novamente.");

                if (dataOcorrencia == DateTime.MinValue)
                    return Json<bool>(false, false, "Data da ocorrência inválida.");

                unidadeDeTrabalho.Start();

                if (tipoOcorrencia.Tipo == "F")
                    cteEntrega.Finalizado = true;
                else
                    cteEntrega.Finalizado = false;

                repCTeEntrega.Atualizar(cteEntrega);

                Dominio.Entidades.OcorrenciaDeCTe ocorrencia = new Dominio.Entidades.OcorrenciaDeCTe();

                ocorrencia.CTe = cteEntrega.CTe;
                ocorrencia.DataDaOcorrencia = dataOcorrencia;
                ocorrencia.DataDeCadastro = DateTime.Now;
                ocorrencia.Observacao = observacao;
                ocorrencia.Ocorrencia = tipoOcorrencia;

                repOcorrencia.Inserir(ocorrencia);

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao registrar a ocorrência para a entrega.");
            }
        }

        #endregion
    }
}
