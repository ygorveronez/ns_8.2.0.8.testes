using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiroNFe : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>
    {
        public CTeTerceiroNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> BuscarPorCTeTerceiro(int codigoCTeTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>();

            query = query.Where(o => o.CTeTerceiro.Codigo == codigoCTeTerceiro);

            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroNFe> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<int> queryPedidoCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido)
                .Select(a => a.CTeTerceiro.Codigo);

            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>()
                .Where(o => queryPedidoCTeParaSubContratacao.Contains(o.CTeTerceiro.Codigo))
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroNFe()
                {
                    Chave = o.Chave,
                    DataEmissao = o.DataEmissao,
                    NCM = o.NCM,
                    Numero = o.Numero,
                    NumeroPedido = o.NumeroPedido,
                    NumeroReferenciaEDI = o.NumeroReferenciaEDI,
                    NumeroRomaneio = o.NumeroRomaneio,
                    Peso = o.Peso,
                    PesoCubado = o.PesoCubado,
                    Protocolo = o.Protocolo,
                    ProtocoloCliente = o.ProtocoloCliente,
                    ValorTotal = o.ValorTotal,
                    Volumes = o.Volumes,
                    Serie = o.Serie,
                    NumeroControleCliente = o.NumeroControleCliente,
                    PINSuframa = o.PINSuframa,
                    CTeTerceiro = new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiro()
                    {
                        Codigo = o.CTeTerceiro.Codigo
                    }
                }).ToList();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe BuscarPrimeiroPorCTeTerceiro(int codigoCTeTerceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>();

            query = query.Where(o => o.CTeTerceiro.Codigo == codigoCTeTerceiro);

            return query.FirstOrDefault();
        }

        public bool ExistePorCTeTerceiro(int codigoCTeTerceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>();

            query = query.Where(o => o.CTeTerceiro.Codigo == codigoCTeTerceiro);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> BuscarPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>();

            query = query.Where(o => o.CTeTerceiro.ChaveAcesso == chave);

            return query.ToList();
        }

        public List<string> BuscarChavePorCTeTerceiro(int codigoCTeTerceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>();

            query = query.Where(o => o.CTeTerceiro.Codigo == codigoCTeTerceiro);

            return query.Select(o => o.Chave).ToList();
        }

        public void DeletarPorCTeTerceiro(int codigoCTeTerceiro, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente)
        {
            try
            {
                if (codigoCTeTerceiro <= 0)
                    return;
                if (objetoValorPersistente != null)
                {
                    objetoValorPersistente.lstDelete.Add($" DELETE FROM T_CTE_TERCEIRO_NFE WHERE  CPS_CODIGO = {codigoCTeTerceiro}"); // SQL-INJECTION-SAFE
                    return;
                }
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroNFe obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
                                     .SetInt32("codigoCTeTerceiro", codigoCTeTerceiro)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroNFe obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
                                    .SetInt32("codigoCTeTerceiro", codigoCTeTerceiro)
                                    .ExecuteUpdate();

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
