using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class MontagemCarregamentoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido>
    {
        public MontagemCarregamentoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MontagemCarregamentoPedido(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public void Inserir(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador, List<int> codigosPedidos)
        {
            string sqlQuery = @"
INSERT INTO T_MONTAGEM_CARREGAMENTO_PEDIDO ( PED_CODIGO, SRO_CODIGO ) 
SELECT PED_CODIGO, :SRO_CODIGO
  FROM T_PEDIDO
 WHERE PED_CODIGO IN ( :codigos );";

            int take = 1000;
            int start = 0;
            object codigo = DBNull.Value;
            if (sessaoRoteirizador != null)
                codigo = sessaoRoteirizador.Codigo;

            while (start < codigosPedidos?.Count)
            {

                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameter("SRO_CODIGO", codigo);
                query.SetParameterList("codigos", tmp);
                query.ExecuteUpdate();
                start += take;
            }
        }

        public async Task InserirAsync(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador, List<int> codigosPedidos)
        {
            string sqlQuery = @"
INSERT INTO T_MONTAGEM_CARREGAMENTO_PEDIDO ( PED_CODIGO, SRO_CODIGO ) 
SELECT PED_CODIGO, :SRO_CODIGO
  FROM T_PEDIDO
 WHERE PED_CODIGO IN ( :codigos );";

            int take = 1000;
            int start = 0;
            object codigo = DBNull.Value;
            if (sessaoRoteirizador != null)
                codigo = sessaoRoteirizador.Codigo;

            while (start < codigosPedidos?.Count)
            {

                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameter("SRO_CODIGO", codigo);
                query.SetParameterList("codigos", tmp);
                await query.ExecuteUpdateAsync(CancellationToken);
                start += take;
            }
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido BuscarPrimerioCarregamentoPedido(int cod_sessao_roteirizador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido>();
            if (cod_sessao_roteirizador > 0)
                query = query.Where(x => x.SessaoRoteirizador.Codigo == cod_sessao_roteirizador);
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido> BuscarPrimerioCarregamentoPedidoAsync(int codigoSessaoRoteirizador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido>();

            if (codigoSessaoRoteirizador > 0)
                query = query.Where(x => x.SessaoRoteirizador.Codigo == codigoSessaoRoteirizador);

            var result = from obj in query select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public List<int> SessoesRoteirizador()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido>();
            return query.Where(o => o.SessaoRoteirizador != null &&
                                    o.SessaoRoteirizador.SituacaoSessaoRoteirizador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Cancelada)
                        .Select(o => o.SessaoRoteirizador.Codigo).Distinct().ToList();
        }

        public List<int> BusarCodigosPedido(int cod_sessao_roteirizador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido>();
            if (cod_sessao_roteirizador > 0)
                query = query.Where(x => x.SessaoRoteirizador.Codigo == cod_sessao_roteirizador);
            var result = from obj in query select obj.Pedido.Codigo;
            return result.ToList();
        }

        public void DeletarTodos(int cod_sessao_roteirizador)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    if (cod_sessao_roteirizador == 0)
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM MontagemCarregamentoPedido").ExecuteUpdate();
                    else
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM MontagemCarregamentoPedido WHERE Codigo in (SELECT c.Codigo FROM MontagemCarregamentoPedido c WHERE c.SessaoRoteirizador.Codigo = :codigoSessaoRoteirizador)").SetInt32("codigoSessaoRoteirizador", cod_sessao_roteirizador).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        if (cod_sessao_roteirizador == 0)
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM MontagemCarregamentoPedido").ExecuteUpdate();
                        else
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM MontagemCarregamentoPedido WHERE Codigo IN (SELECT c.Codigo FROM MontagemCarregamentoPedido c WHERE c.SessaoRoteirizador.Codigo = :codigoSessaoRoteirizador)").SetInt32("codigoSessaoRoteirizador", cod_sessao_roteirizador).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

    }
}
