using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Sistema
{
    [CustomAuthorize("Sistema/DadosPadrao")]
    public class DadosPadraoController : BaseController
    {
		#region Construtores

		public DadosPadraoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterCentroCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

                if (operadorLogistica?.CentrosCarregamento?.Count == 1)
                {
                    return new JsonpResult(new
                    {
                        operadorLogistica.CentrosCarregamento.First().Codigo,
                        operadorLogistica.CentrosCarregamento.First().Descricao
                    });
                }

                return new JsonpResult(null);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o centro de carregamento padrão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterFilial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

                if (operadorLogistica?.Filiais?.Count == 1)
                {
                    return new JsonpResult(new
                    {
                        operadorLogistica.Filiais.First().Filial.Codigo,
                        operadorLogistica.Filiais.First().Filial.Descricao
                    });
                }

                return new JsonpResult(null);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a Filial padrão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterFiliais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

                if (operadorLogistica?.Filiais?.Count > 0)
                {
                    var listaFilialRetornar = (
                        from operadorFilial in operadorLogistica.Filiais
                        select new
                        {
                            operadorFilial.Filial.Codigo,
                            operadorFilial.Filial.Descricao
                        }
                    ).ToList();

                    return new JsonpResult(listaFilialRetornar);
                }

                return new JsonpResult(null);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a Filial padrão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
