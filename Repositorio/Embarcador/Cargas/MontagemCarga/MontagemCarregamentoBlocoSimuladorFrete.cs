using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class MontagemCarregamentoBlocoSimuladorFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete>
    {
        public MontagemCarregamentoBlocoSimuladorFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> BuscarPorCodigos(List<int> codigosMontagemCarregamentoBloco)
        {
            const int loteMaximo = 2000;
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> listaTotal =
                new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete>();

            for (int i = 0; i < codigosMontagemCarregamentoBloco.Count; i += loteMaximo)
            {
                List<int> lote = codigosMontagemCarregamentoBloco.Skip(i).Take(loteMaximo).ToList();

                IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> query =
                    this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete>();

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> resultadoParcial =
                    (from obj in query
                     where lote.Contains(obj.Codigo)
                     select obj)
                    .Fetch(obj => obj.Transportador)
                    .Fetch(obj => obj.TipoOperacao)
                    .ToList();

                listaTotal.AddRange(resultadoParcial);
            }

            return listaTotal;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> BuscarPorBloco(int codigoMontagemCarregamentoBloco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete>();

            var result = from obj in query
                         where obj.Bloco.Codigo == codigoMontagemCarregamentoBloco
                         select obj;

            return result
                    .Fetch(obj => obj.Transportador)
                    .Fetch(obj => obj.TipoOperacao)
                    .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> BuscarPorBlocos(List<int> codigosMontagemCarregamentoBloco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete>();

            var result = from obj in query
                         where codigosMontagemCarregamentoBloco.Contains(obj.Bloco.Codigo)
                         select obj;

            return result
                    .Fetch(obj => obj.Bloco)
                    .ThenFetch(obj => obj.Cliente)
                    .Fetch(obj => obj.Transportador)
                    .Fetch(obj => obj.TipoOperacao)
                    .Fetch(obj => obj.ModeloVeicularCarga)
                    .Fetch(obj => obj.TipoDeCarga)
                    .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete BuscarVencedorBloco(int codigoMontagemCarregamentoBloco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete>();

            var result = from obj in query
                         where obj.Bloco.Codigo == codigoMontagemCarregamentoBloco && obj.Vencedor
                         select obj;

            return result
                    .Fetch(obj => obj.Transportador)
                    .Fetch(obj => obj.TipoOperacao)
                    .FirstOrDefault();
        }

        private void DeletarPorTrasportadorDisputaBloco(int codigoEmpresaTransportador, int codigoTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.CentroCarregamentoTipoOperacaoTipo tipo, int codigoBloco)
        {
            // Limpando as tabelas T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE_PEDIDO, T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE, T_MONTAGEM_CARREGAMENTO_BLOCO_PEDIDO e T_MONTAGEM_CARREGAMENTO_BLOCO
            UnitOfWork.Sessao.CreateSQLQuery(@"
DELETE 
  FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE_PEDIDO 
 WHERE MSF_CODIGO IN (SELECT MSF.MSF_CODIGO 
                        FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE MSF 
                       WHERE MSF.EMP_CODIGO = :codigoEmpresa 
                         AND MSF.TOP_CODIGO = :codigoTipoOperacao 
                         AND MSF.MSF_TIPO_SIMULACAO = :tipo 
                         AND MSF.MCB_CODIGO = :codigoBloco )")
                .SetInt32("codigoEmpresa", codigoEmpresaTransportador)
                .SetInt32("codigoTipoOperacao", codigoTipoOperacao)
                .SetEnum("tipo", tipo)
                .SetInt32("codigoBloco", codigoBloco)
                .ExecuteUpdate();

            UnitOfWork.Sessao.CreateSQLQuery(@"
DELETE 
  FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE 
 WHERE EMP_CODIGO = :codigoEmpresa 
   AND TOP_CODIGO = :codigoTipoOperacao 
   AND MSF_TIPO_SIMULACAO = :tipo 
   AND MCB_CODIGO = :codigoBloco ").SetInt32("codigoEmpresa", codigoEmpresaTransportador)
                                        .SetInt32("codigoTipoOperacao", codigoTipoOperacao)
                                        .SetEnum("tipo", tipo)
                                        .SetInt32("codigoBloco", codigoBloco)
                                        .ExecuteUpdate();

        }

        public void RemoverTrasportadorDisputaBloco(int codigoEmpresaTransportador, int codigoTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.CentroCarregamentoTipoOperacaoTipo tipo, int codigoBloco)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                    DeletarPorTrasportadorDisputaBloco(codigoEmpresaTransportador, codigoTipoOperacao, tipo, codigoBloco);
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        DeletarPorTrasportadorDisputaBloco(codigoEmpresaTransportador, codigoTipoOperacao, tipo, codigoBloco);

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
                        throw new System.Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }
    }
}
