using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Operacional
{
    [CustomAuthorize("Operacional/RestricaoVisualizacaoCanhotos")]
    public class RestricaoVisualizacaoCanhotosController : BaseController
    {
		#region Construtores

		public RestricaoVisualizacaoCanhotosController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorOperador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoUsuario = Request.GetIntParam("Codigo");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhoto repOperadorCanhoto = new Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhoto(unitOfWork);
                Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga repOperadorTipoCarga = new Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga(unitOfWork);
                Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular repOperadorTipoCargaModeloVeicular = new Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular(unitOfWork);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);

                Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto operadorCanhoto = repOperadorCanhoto.BuscarPorUsuario(usuario.Codigo);
                if (operadorCanhoto == null)
                {
                    Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                    Dominio.Entidades.Embarcador.Operacional.OperadorLogistica logistica = repOperadorLogistica.BuscarPorUsuario(usuario.Codigo);

                    if (logistica == null)
                        return new JsonpResult(false, true, Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.UsuarioInformadoNaoOperadorLogistica);

                    Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto novoOperador = new Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto();
                    novoOperador.Usuario = usuario;

                    repOperadorCanhoto.Inserir(novoOperador);
                    operadorCanhoto = repOperadorCanhoto.BuscarPorUsuario(usuario.Codigo);

                    unitOfWork.CommitChanges();

                    if (operadorCanhoto == null)
                        return new JsonpResult(false, true, Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.UsuarioInformadoNaoOperadorLogistica);
                }

                List<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga> operadorTiposCarga = repOperadorTipoCarga.BuscarPorOperador(operadorCanhoto.Codigo);
                List<dynamic> dynOperadorTiposCarga = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga operadorTipoCarga in operadorTiposCarga)
                {
                    List<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular> operadorTipoCargaModelosVeicular = repOperadorTipoCargaModeloVeicular.BuscarPorOperadorTipoCarga(operadorTipoCarga.Codigo);

                    var dynOperadorTipoCarga = new
                    {
                        operadorTipoCarga.Codigo,
                        TipoCarga = new { operadorTipoCarga.TipoDeCarga.Codigo, operadorTipoCarga.TipoDeCarga.Descricao },
                        OperadorTipoCargaModelosVeicular = (from obj in operadorTipoCargaModelosVeicular
                                                            select new
                                                            {
                                                                obj.Codigo,
                                                                ModeloVeicularCarga = new { obj.ModeloVeicularCarga.Codigo, obj.ModeloVeicularCarga.Descricao }
                                                            }).ToList()
                    };
                    dynOperadorTiposCarga.Add(dynOperadorTipoCarga);
                }

                var dynOperadorCanhoto = new
                {
                    CodigoOperadorCanhoto = operadorCanhoto.Codigo,
                    operadorCanhoto.Usuario.Codigo,
                    operadorCanhoto.PossuiFiltroTipoOperacao,
                    operadorCanhoto.VisualizaCargasSemTipoOperacao,
                    RestricaoAtiva = operadorCanhoto.Ativo,
                    OperadorCanhoto = new { operadorCanhoto.Usuario.Codigo, Descricao = operadorCanhoto.Usuario.Nome },
                    Filiais = (from obj in operadorCanhoto.Filiais ?? new List<Dominio.Entidades.Embarcador.Filiais.Filial>()
                               select new
                               {
                                   obj.Codigo,
                                   Filial = new { obj.Codigo, obj.Descricao }
                               }).ToList(),
                    TiposOperacao = (from obj in operadorCanhoto.TiposOperacao ?? new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>()
                                     select new
                                     {
                                         Operacao = new { obj.Codigo, obj.Descricao }
                                     }).ToList(),
                    TiposCarga = dynOperadorTiposCarga,
                };

                return new JsonpResult(dynOperadorCanhoto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.OcorreuFalhaBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhoto repOperadorCanhoto = new Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto operadorCanhoto = repOperadorCanhoto.BuscarPorUsuario(Request.GetIntParam("Codigo"));

                if (operadorCanhoto == null)
                    throw new ControllerException(Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.UsuarioInformadoNaoOperadorLogistica);

                operadorCanhoto.Initialize();

                operadorCanhoto.Ativo = Request.GetBoolParam("RestricaoAtiva");
                operadorCanhoto.VisualizaCargasSemTipoOperacao = Request.GetBoolParam("VisualizaCargasSemTipoOperacao");
                operadorCanhoto.PossuiFiltroTipoOperacao = Request.GetBoolParam("PossuiFiltroTipoOperacao");

                SalvarTiposOperacao(operadorCanhoto, unitOfWork);
                SalvarFilial(operadorCanhoto, unitOfWork);
                SalvarTiposCarga(operadorCanhoto, unitOfWork);

                repOperadorCanhoto.Atualizar(operadorCanhoto);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, operadorCanhoto, operadorCanhoto.GetChanges(), Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.AtualizouOperador, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.OcorreuFalhaAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarFilial(Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto operadorCanhoto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);

            if (operadorCanhoto.Filiais == null)
                operadorCanhoto.Filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            else
                operadorCanhoto.Filiais.Clear();

            dynamic dynFiliais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

            foreach (dynamic dynFilial in dynFiliais)
                operadorCanhoto.Filiais.Add(repFilial.BuscarPorCodigo((int)dynFilial.Codigo));
        }

        private void SalvarTiposOperacao(Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto operadorCanhoto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

            if (operadorCanhoto.TiposOperacao == null)
                operadorCanhoto.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            else
                operadorCanhoto.TiposOperacao.Clear();

            dynamic dynTiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));

            foreach (dynamic dynTipoOperacao in dynTiposOperacao)
                operadorCanhoto.TiposOperacao.Add(repTipoOperacao.BuscarPorCodigo((int)dynTipoOperacao.Codigo));
        }

        private void SalvarTiposCarga(Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto operadorCanhoto, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga repOperadorTipoCarga = new Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeTrabalho);

            dynamic dynTiposDeCargaVenda = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposCarga"));

            List<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga> operadoresTipoDeCargas = repOperadorTipoCarga.BuscarPorOperador(operadorCanhoto.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();
            if (operadoresTipoDeCargas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dynTipoDeCarga in dynTiposDeCargaVenda)
                    if (dynTipoDeCarga.Codigo != null)
                        codigos.Add((int)dynTipoDeCarga.Codigo);

                List<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga> operadoresTiposDeCargasDeletar = (from obj in operadoresTipoDeCargas where !codigos.Contains(obj.OperadorCanhoto.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga operadorTipoDeCargaDeletar in operadoresTiposDeCargasDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "TipoCarga",
                        De = operadorTipoDeCargaDeletar.TipoDeCarga.Descricao,
                        Para = ""
                    });

                    repOperadorTipoCarga.Deletar(operadorTipoDeCargaDeletar);
                }
            }

            foreach (var dynTipoDeCarga in dynTiposDeCargaVenda)
            {
                int codigoTipoDeCarga = ((string)dynTipoDeCarga.TipoCarga.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga operadorTipoCarga = codigoTipoDeCarga > 0 ? repOperadorTipoCarga.BuscarPorOperadorETipoDeCarga(operadorCanhoto.Codigo, codigoTipoDeCarga) : null;

                if (operadorTipoCarga == null)
                {
                    operadorTipoCarga = new Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga()
                    {
                        TipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoDeCarga),
                        OperadorCanhoto = operadorCanhoto
                    };
                    repOperadorTipoCarga.Inserir(operadorTipoCarga);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "TipoCarga",
                        De = "",
                        Para = operadorTipoCarga.TipoDeCarga.Descricao
                    });
                }
            }

            operadorCanhoto.SetExternalChanges(alteracoes);
        }

    }

    #endregion

}
