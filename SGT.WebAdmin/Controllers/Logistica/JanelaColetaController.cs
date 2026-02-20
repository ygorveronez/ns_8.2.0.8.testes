using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/JanelaColeta")]
    public class JanelaColetaController : BaseController
    {
		#region Construtores

		public JanelaColetaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
                Enum.TryParse(Request.Params("Ativo"), out ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                

                Repositorio.Embarcador.Logistica.JanelaColeta repJanelaColeta = new Repositorio.Embarcador.Logistica.JanelaColeta(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Logistica.JanelaColeta> listaJanelaColeta = repJanelaColeta.Consultar(descricao, null);
                grid.setarQuantidadeTotal(repJanelaColeta.ContarConsulta(descricao));

                var lista = (
                    from JanelaColeta in listaJanelaColeta
                    select new
                    {
                        JanelaColeta.Codigo,
                        JanelaColeta.Descricao,
                    }
                ).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

    
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool ativo = Request.GetBoolParam("Ativo");
                string descricao = Request.GetStringParam("Descricao");
                string observacao = Request.GetStringParam("Observacao");
                dynamic clientes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Clientes"));
                dynamic UFs = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("UFs"));
                dynamic localidades = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Localidades"));
                dynamic periodosColeta = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("PeriodosColeta"));


                Repositorio.Embarcador.Logistica.JanelaColeta repJanelaColeta = new Repositorio.Embarcador.Logistica.JanelaColeta(unidadeDeTrabalho);
                

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Logistica.JanelaColeta janelaColeta = new Dominio.Entidades.Embarcador.Logistica.JanelaColeta
                {
                    Descricao = descricao,
                    Observacao = observacao,
                    Ativo = ativo,
                    
                };


                repJanelaColeta.Inserir(janelaColeta, Auditado);
                
                SalvarPeriodosColeta(janelaColeta, null, periodosColeta, unidadeDeTrabalho);
                SalvarUFs(janelaColeta, null, UFs, unidadeDeTrabalho);
                SalvarLocalidades(janelaColeta, null, localidades, unidadeDeTrabalho);
                SalvarClientes(janelaColeta, null, clientes, unidadeDeTrabalho);

                repJanelaColeta.Atualizar(janelaColeta);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unidadeDeTrabalho.Rollback();

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool ativo = Request.GetBoolParam("Ativo");
                string descricao = Request.GetStringParam("Descricao");
                string observacao = Request.GetStringParam("Observacao");
                dynamic clientes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Clientes"));
                dynamic UFs = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("UFs"));
                dynamic localidades = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Localidades"));
                dynamic periodosColeta = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("PeriodosColeta"));


                Repositorio.Embarcador.Logistica.JanelaColeta repJanelaColeta = new Repositorio.Embarcador.Logistica.JanelaColeta(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Logistica.JanelaColeta janelaColeta = repJanelaColeta.BuscarPorCodigo(codigo, true);

                if (janelaColeta == null)
                    return new JsonpResult(true, false, "Janela de Coleta não encontrado.");

                unidadeDeTrabalho.Start();

               
                janelaColeta.Descricao = descricao;
                janelaColeta.Observacao = observacao;
                janelaColeta.Ativo = ativo;


                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repJanelaColeta.Atualizar(janelaColeta, Auditado);

                SalvarPeriodosColeta(janelaColeta, historico, periodosColeta, unidadeDeTrabalho);
                SalvarUFs(janelaColeta, historico, UFs, unidadeDeTrabalho);
                SalvarLocalidades(janelaColeta, historico, localidades, unidadeDeTrabalho);
                SalvarClientes(janelaColeta, historico, clientes, unidadeDeTrabalho);

                repJanelaColeta.Atualizar(janelaColeta);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unidadeDeTrabalho.Rollback();

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        private dynamic ObterJanela(Dominio.Entidades.Embarcador.Logistica.JanelaColeta JanelaColeta)
        {
            return new
            {
                JanelaColeta.Codigo,
                JanelaColeta.Descricao,
                JanelaColeta.Observacao,
                JanelaColeta.Ativo,
                Localidades = (from obj in JanelaColeta.Localidades
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Descricao = obj.Descricao
                               }).ToList(),

                UFs = (from obj in JanelaColeta.Estados
                               select new
                               {
                                   Codigo = obj.Sigla,
                                   Descricao = obj.Nome
                               }).ToList(),

                Clientes = (from obj in JanelaColeta.Clientes
                       select new
                       {
                           Codigo = obj.Codigo,
                           Descricao = obj.Descricao
                       }).ToList(),

                PeriodosColeta = (from obj in JanelaColeta.PeriodosColeta
                            select new
                            {
                                Codigo = obj.Codigo,
                                Descricao = obj.Descricao,
                                HoraInicio = string.Format("{0:00}:{1:00}", obj.HoraInicio.Hours, obj.HoraInicio.Minutes),
                                HoraTermino = string.Format("{0:00}:{1:00}", obj.HoraTermino.Hours, obj.HoraTermino.Minutes),
                                Ativo = obj.Ativo,
                                DiaSemana = obj.Dia
                            }).ToList(),

            };
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Logistica.JanelaColeta repJanelaColeta = new Repositorio.Embarcador.Logistica.JanelaColeta(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Logistica.JanelaColeta JanelaColeta = repJanelaColeta.BuscarPorCodigo(codigo);

                var retorno = ObterJanela(JanelaColeta);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Logistica.JanelaColeta repJanelaColeta = new Repositorio.Embarcador.Logistica.JanelaColeta(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.TempoCarregamento repTempoCarregamento = new Repositorio.Embarcador.Logistica.TempoCarregamento(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Logistica.JanelaColeta JanelaColeta = repJanelaColeta.BuscarPorCodigo(codigo);

                JanelaColeta.Clientes = null;
                

                repJanelaColeta.Deletar(JanelaColeta, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");

            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarPeriodosColeta(Dominio.Entidades.Embarcador.Logistica.JanelaColeta JanelaColeta, Dominio.Entidades.Auditoria.HistoricoObjeto historico, dynamic dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoColeta repPeriodoColeta = new Repositorio.Embarcador.Logistica.PeriodoColeta(unidadeDeTrabalho);

            repPeriodoColeta.DeletarPorEntidade(JanelaColeta);

            foreach (var peridodoColeta in dados)
            {
                if (!(bool)peridodoColeta.FazColeta)
                    continue;

                Dominio.Entidades.Embarcador.Logistica.PeriodoColeta per = new Dominio.Entidades.Embarcador.Logistica.PeriodoColeta()
                {
                    Ativo = true,
                    Dia = peridodoColeta.DiaSemana,
                    HoraInicio = peridodoColeta.HoraInicio,
                    HoraTermino = !string.IsNullOrEmpty((string)peridodoColeta.HoraTermino) ? peridodoColeta.HoraTermino : new TimeSpan (0,0,0),
                    JanelaColeta = JanelaColeta
                };

                repPeriodoColeta.Inserir(per);
            }
        }

        private void SalvarUFs(Dominio.Entidades.Embarcador.Logistica.JanelaColeta JanelaColeta, Dominio.Entidades.Auditoria.HistoricoObjeto historico, dynamic dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

            JanelaColeta.Estados = new List<Dominio.Entidades.Estado>();

            foreach (var estado in dados)
            {
                Dominio.Entidades.Estado uf = repEstado.BuscarPorSigla((string)estado.Codigo);
                JanelaColeta.Estados.Add(uf);
            }

        }
        private void SalvarClientes(Dominio.Entidades.Embarcador.Logistica.JanelaColeta JanelaColeta, Dominio.Entidades.Auditoria.HistoricoObjeto historico, dynamic dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            JanelaColeta.Clientes = new List<Dominio.Entidades.Cliente>();

            foreach (var cliente in dados)
            {
                Dominio.Entidades.Cliente cli =  repCliente.BuscarPorCPFCNPJ((double)cliente.Codigo);
                JanelaColeta.Clientes.Add(cli);
            }

        }

        private void SalvarLocalidades(Dominio.Entidades.Embarcador.Logistica.JanelaColeta JanelaColeta, Dominio.Entidades.Auditoria.HistoricoObjeto historico, dynamic dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            JanelaColeta.Localidades = new List<Dominio.Entidades.Localidade>();

            foreach (var localidade in dados)
            {
                Dominio.Entidades.Localidade loc = repLocalidade.BuscarPorCodigo((int)localidade.Codigo);
                JanelaColeta.Localidades.Add(loc);
            }

        }



        #endregion
    }
}
