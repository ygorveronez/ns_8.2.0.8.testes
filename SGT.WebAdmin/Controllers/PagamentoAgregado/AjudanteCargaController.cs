using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.PagamentoAgregado
{
    [CustomAuthorize("PagamentosAgregados/AjudanteCarga")]
    public class AjudanteCargaController : BaseController
    {
		#region Construtores

		public AjudanteCargaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);

                // Valida
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    carga.Codigo,
                    Carga = carga.CodigoCargaEmbarcador,
                    ListaAjudantes = carga.Ajudantes != null ? (from obj in carga.Ajudantes
                                                                orderby obj.Nome
                                                                select new
                                                                {
                                                                    obj.Codigo,
                                                                    CPF = obj.CPF_Formatado,
                                                                    obj.Nome
                                                                }).ToList() : null
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);                

                // Valida
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                SalvarListaAjudante(ref carga, unitOfWork);

                // Persiste dados
                repCarga.Atualizar(carga);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private void SalvarListaAjudante(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Usuario repAjudante = new Repositorio.Usuario(unidadeDeTrabalho);
            if (carga.Ajudantes == null)
                carga.Ajudantes = new List<Dominio.Entidades.Usuario>();
            carga.Ajudantes.Clear();

            dynamic listaAjudante = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAjudantes"));
            if (listaAjudante != null)
            {
                foreach (var ajudante in listaAjudante)
                {
                    int codigo = 0;
                    int.TryParse((string)ajudante.Ajudante.Codigo, out codigo);
                    if (codigo > 0)
                    {
                        Dominio.Entidades.Usuario mot = repAjudante.BuscarPorCodigo(codigo);
                        carga.Ajudantes.Add(mot);
                    }
                }
            }
        }

        #endregion


    }
}
