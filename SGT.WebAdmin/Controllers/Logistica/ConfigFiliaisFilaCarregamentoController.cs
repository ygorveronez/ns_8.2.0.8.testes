using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Logistica/ConfigFiliaisFilaCarregamento")]
    public class ConfigFiliaisFilaCarregamentoController : BaseController
    {
		#region Construtores

		public ConfigFiliaisFilaCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarFiliaisLiberadasPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisAutorizadas = repFilial.BuscarFiliaisComFilaCarregamentoLiberada();

                if (filiaisAutorizadas == null)
                    filiaisAutorizadas = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();

                return new JsonpResult(new
                {
                    Filiais = filiaisAutorizadas.Select(o => new
                    {
                        o.Codigo,
                        o.Descricao,
                    }).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                dynamic filiaisAtualizar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

                if (filiaisAtualizar == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar registros.");

                List<int> codigos = new List<int>();

                foreach (dynamic filial in filiaisAtualizar)
                    codigos.Add((int)filial.Codigo);

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisAutorizar = repFilial.BuscarPorCodigos(codigos);

                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisAutorizadas = repFilial.BuscarFiliaisComFilaCarregamentoLiberada();
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisSemPermissao = filiaisAutorizadas.Where(o => !codigos.Contains(o.Codigo)).ToList();

                unitOfWork.Start();

                AtualizarPermissaoFilaCarregamento(filiaisAutorizar, true, repFilial);
                AtualizarPermissaoFilaCarregamento(filiaisSemPermissao, false, repFilial);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private void AtualizarPermissaoFilaCarregamento(List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisAtualizar, bool liberar, Repositorio.Embarcador.Filiais.Filial repFilial)
        {
            if (filiaisAtualizar == null)
                filiaisAtualizar = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();

            foreach (var filial in filiaisAtualizar)
            {
                filial.LiberarParaFilaCarregamento = liberar;
                repFilial.Atualizar(filial);
            }
        }

        #endregion
    }
}
