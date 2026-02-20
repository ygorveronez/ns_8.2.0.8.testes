using System;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ClienteExportacaoCTeController : ApiController
    {
        public ActionResult BuscarPorNome()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string nome = Request.Params["Nome"];
                if (!string.IsNullOrWhiteSpace(nome))
                {
                    Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
                    Dominio.Entidades.ParticipanteCTe participante = repParticipanteCTe.BuscarDoExteriorPorNome(nome);

                    if (participante != null)
                    {
                        var retorno = new { Bairro = participante.Bairro, Cidade = participante.Cidade, Complemento = participante.Complemento, Email = participante.Email, Endereco = participante.Endereco, Exterior = true, Nome = participante.Nome, Numero = participante.Numero, SiglaPais = participante.Pais.Sigla };
                        return Json(retorno, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, "Nenhum cliente para exportação encontrado com este nome.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Nome inválido para consulta.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados do cliente para exportação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
