using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class MontagemCarregamentoBloco : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco>
    {
        public MontagemCarregamentoBloco(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco>();

            var result = from obj in query
                         where codigos.Contains(obj.Codigo)
                         select obj;

            return result
                   .Fetch(obj => obj.Cliente)
                   .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> BuscarPorSessaoRoteirizador(int codigoSessaoRoteirizador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco>();

            var result = from obj in query
                         where obj.SessaoRoteirizador.Codigo == codigoSessaoRoteirizador
                         select obj;

            return result
                    .Fetch(obj => obj.Cliente)
                    .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco BuscarPorSessaoRoteirizadorECliente(int codigoSessaoRoteirizador, int codigoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco>();

            var result = from obj in query
                         where obj.SessaoRoteirizador.Codigo == codigoSessaoRoteirizador &&
                               obj.Cliente.Codigo == codigoCliente
                         select obj;

            return result
                    .Fetch(obj => obj.Cliente)
                    .FirstOrDefault();
        }

        public void DeletarTodosPorSessaoRoteirizador(int codigoSessao)
        {
            try
            {
                if (codigoSessao == 0) return;

                if (UnitOfWork.IsActiveTransaction())
                    DeletarPorSessao(codigoSessao);
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        DeletarPorSessao(codigoSessao);

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

        private void DeletarPorSessao(int codigoSessao)
        {
            // Limpando as tabelas T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE_PEDIDO, T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE, T_MONTAGEM_CARREGAMENTO_BLOCO_PEDIDO e T_MONTAGEM_CARREGAMENTO_BLOCO
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE_PEDIDO WHERE MSF_CODIGO IN (SELECT MSF_CODIGO FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE MSF INNER JOIN T_MONTAGEM_CARREGAMENTO_BLOCO MCB ON MSF.MCB_CODIGO = MCB.MCB_CODIGO AND MCB.SRO_CODIGO = :codigoSessao AND NOT EXISTS (SELECT 1 FROM T_CARREGAMENTO CAR WHERE CAR.MCB_CODIGO = MCB.MCB_CODIGO))").SetInt32("codigoSessao", codigoSessao).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE MSF FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE MSF INNER JOIN T_MONTAGEM_CARREGAMENTO_BLOCO MCB ON MSF.MCB_CODIGO = MCB.MCB_CODIGO AND MCB.SRO_CODIGO = :codigoSessao AND NOT EXISTS (SELECT 1 FROM T_CARREGAMENTO CAR WHERE CAR.MCB_CODIGO = MCB.MCB_CODIGO)").SetInt32("codigoSessao", codigoSessao).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE MBP FROM T_MONTAGEM_CARREGAMENTO_BLOCO_PEDIDO MBP INNER JOIN T_MONTAGEM_CARREGAMENTO_BLOCO MCB ON MBP.MCB_CODIGO = MCB.MCB_CODIGO AND MCB.SRO_CODIGO = :codigoSessao AND NOT EXISTS (SELECT 1 FROM T_CARREGAMENTO CAR WHERE CAR.MCB_CODIGO = MCB.MCB_CODIGO)").SetInt32("codigoSessao", codigoSessao).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE MCB FROM T_MONTAGEM_CARREGAMENTO_BLOCO MCB WHERE SRO_CODIGO = :codigoSessao AND NOT EXISTS (SELECT 1 FROM T_CARREGAMENTO CAR WHERE CAR.MCB_CODIGO = MCB.MCB_CODIGO)").SetInt32("codigoSessao", codigoSessao).ExecuteUpdate();
        }

        public void DeletarTodosPorCarregamento(int codigoCarregamento)
        {
            try
            {
                if (codigoCarregamento == 0) return;

                if (UnitOfWork.IsActiveTransaction())
                    DeletarPorCarregamento(codigoCarregamento);
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        DeletarPorCarregamento(codigoCarregamento);

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

        private void DeletarPorCarregamento(int codigoCarregamento)
        {
            // Limpando as tabelas T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE_PEDIDO, T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE, T_MONTAGEM_CARREGAMENTO_BLOCO_PEDIDO e T_MONTAGEM_CARREGAMENTO_BLOCO
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE_PEDIDO WHERE MSF_CODIGO IN (SELECT MSF_CODIGO FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE MSF INNER JOIN T_MONTAGEM_CARREGAMENTO_BLOCO MCB ON MSF.MCB_CODIGO = MCB.MCB_CODIGO AND MCB.CRG_CODIGO = :codigoSessao AND NOT EXISTS (SELECT 1 FROM T_CARREGAMENTO CAR WHERE CAR.MCB_CODIGO = MCB.MCB_CODIGO))").SetInt32("codigoSessao", codigoCarregamento).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE MSF FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE MSF INNER JOIN T_MONTAGEM_CARREGAMENTO_BLOCO MCB ON MSF.MCB_CODIGO = MCB.MCB_CODIGO AND MCB.CRG_CODIGO = :codigoSessao AND NOT EXISTS (SELECT 1 FROM T_CARREGAMENTO CAR WHERE CAR.MCB_CODIGO = MCB.MCB_CODIGO)").SetInt32("codigoSessao", codigoCarregamento).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE MBP FROM T_MONTAGEM_CARREGAMENTO_BLOCO_PEDIDO MBP INNER JOIN T_MONTAGEM_CARREGAMENTO_BLOCO MCB ON MBP.MCB_CODIGO = MCB.MCB_CODIGO AND MCB.CRG_CODIGO = :codigoSessao AND NOT EXISTS (SELECT 1 FROM T_CARREGAMENTO CAR WHERE CAR.MCB_CODIGO = MCB.MCB_CODIGO)").SetInt32("codigoSessao", codigoCarregamento).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE MCB FROM T_MONTAGEM_CARREGAMENTO_BLOCO MCB WHERE CRG_CODIGO = :codigoSessao AND NOT EXISTS (SELECT 1 FROM T_CARREGAMENTO CAR WHERE CAR.MCB_CODIGO = MCB.MCB_CODIGO)").SetInt32("codigoSessao", codigoCarregamento).ExecuteUpdate();
        }

        public void AtualizarSituacao(int montagemCarregamentoBlocoCodigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco situacao)
        {
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_MONTAGEM_CARREGAMENTO_BLOCO SET CRG_SITUACAO = :situacao WHERE MCB_CODIGO = :codigo").SetEnum("situacao", situacao).SetInt32("codigo", montagemCarregamentoBlocoCodigo).ExecuteUpdate();
        }
    }
}
