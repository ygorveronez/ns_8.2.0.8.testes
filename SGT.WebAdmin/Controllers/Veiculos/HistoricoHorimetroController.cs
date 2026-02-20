using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/HistoricoHorimetro")]
    public class HistoricoHorimetroController : BaseController
    {
        #region Construtores

        public HistoricoHorimetroController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Veiculos.HistoricoHorimetro repHistoricoHorimetro = new Repositorio.Embarcador.Veiculos.HistoricoHorimetro(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro historicoHorimetro = new Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro();

                repHistoricoHorimetro.Inserir(historicoHorimetro, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion
    }
}
