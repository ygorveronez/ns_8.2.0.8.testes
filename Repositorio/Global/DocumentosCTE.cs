using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class DocumentosCTE : RepositorioBase<Dominio.Entidades.DocumentosCTE>, Dominio.Interfaces.Repositorios.DocumentosCTE
    {
        public DocumentosCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public bool ExistePorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.DocumentosCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            query = query.Where(o => o.CTE.Codigo == codigoCTe);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.DocumentosCTE BuscarPorChaveNFe(string chaveNFe, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.ChaveNFE == chaveNFe && obj.CTE.Empresa.Codigo == empresa && obj.CTE.Status == "A" select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.DocumentosCTE BuscarPrimeiroPorCTe(int cte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.CTE.Codigo == cte select obj;
            return result.FirstOrDefault();
        }

        public string BuscarPrimeiroNumeroPorCTe(int cte)
        {
            IQueryable<Dominio.Entidades.DocumentosCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            query = query.Where(obj => obj.CTE.Codigo == cte);

            return query.Select(o => o.Numero).FirstOrDefault();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorChaveNFe(string chaveNFe, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.ChaveNFE == chaveNFe && obj.CTE.Empresa.Codigo == empresa select obj.CTE;
            return result.OrderByDescending(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.DocumentosCTE> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.DocumentosCTE> BuscarPorCTe(IEnumerable<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.DocumentosCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosCTes.Count() / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.DocumentosCTE> documentosCTeRetornar = new List<Dominio.Entidades.DocumentosCTE>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                documentosCTeRetornar.AddRange(query.Where(o => codigosCTes.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CTE.Codigo)).Fetch(o => o.CTE).Fetch(o => o.ModeloDocumentoFiscal).ToList());

            return documentosCTeRetornar;
        }

        public List<string> BuscarChavesPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj.ChaveNFE;
            return result.ToList();
        }

        public List<Dominio.Entidades.DocumentosCTE> ConsultarNotasVinculadas(int protocoloCarga, int protocoloPedido, string chaveCTe)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var queryXMl = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (protocoloCarga > 0 && protocoloPedido > 0)
                query = query.Where(obj => !queryXMl.Any(o => o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.Chave == obj.ChaveNFE && o.CargaPedido.Carga.Protocolo == protocoloCarga && o.CargaPedido.Pedido.Protocolo == protocoloPedido));
            else if (protocoloCarga > 0)
                query = query.Where(obj => !queryXMl.Any(o => o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.Chave == obj.ChaveNFE && o.CargaPedido.Carga.Protocolo == protocoloCarga));
            else if (protocoloPedido > 0)
                query = query.Where(obj => !queryXMl.Any(o => o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.Chave == obj.ChaveNFE && o.CargaPedido.Pedido.Protocolo == protocoloPedido));

            if (protocoloCarga > 0)
                queryCargaCTe = queryCargaCTe.Where(obj => obj.CargaCTe.Carga.Protocolo == protocoloCarga);

            if (protocoloPedido > 0)
                queryCargaCTe = queryCargaCTe.Where(obj => obj.CargaCTe.Carga.Pedidos.Any(p => p.Pedido.Protocolo == protocoloPedido));

            if (!string.IsNullOrWhiteSpace(chaveCTe))
            {
                queryCargaCTe = queryCargaCTe.Where(obj => obj.CargaCTe.CTe.Chave == chaveCTe);
            }

            query = query.Where(obj => queryCargaCTe.Any(o => o.CargaCTe.CTe == obj.CTE));

            return query.ToList();
        }

        public int ContarConsultarNotasVinculadas(int protocoloCarga, int protocoloPedido, string chaveCTe)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var queryXMl = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (protocoloCarga > 0 && protocoloPedido > 0)
                query = query.Where(obj => !queryXMl.Any(o => o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.Chave == obj.ChaveNFE && o.CargaPedido.Carga.Protocolo == protocoloCarga && o.CargaPedido.Pedido.Protocolo == protocoloPedido));
            else if (protocoloCarga > 0)
                query = query.Where(obj => !queryXMl.Any(o => o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.Chave == obj.ChaveNFE && o.CargaPedido.Carga.Protocolo == protocoloCarga));
            else if (protocoloPedido > 0)
                query = query.Where(obj => !queryXMl.Any(o => o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.Chave == obj.ChaveNFE && o.CargaPedido.Pedido.Protocolo == protocoloPedido));

            if (protocoloCarga > 0)
                queryCargaCTe = queryCargaCTe.Where(obj => obj.Carga.Protocolo == protocoloCarga);

            if (protocoloPedido > 0)
                queryCargaCTe = queryCargaCTe.Where(obj => obj.Carga.Pedidos.Any(p => p.Pedido.Protocolo == protocoloPedido));

            if (!string.IsNullOrWhiteSpace(chaveCTe))
                queryCargaCTe = queryCargaCTe.Where(obj => obj.CTe.Chave == chaveCTe);

            query = query.Where(obj => queryCargaCTe.Any(o => o.CTe == obj.CTE));

            return query.Count();
        }

        public List<Dominio.Entidades.DocumentosCTE> BuscarPorCTe(int codigoCTe, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.DocumentosCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            query = query.Where(obj => obj.CTE.Codigo == codigoCTe);

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query.ToList();
        }


        public List<Dominio.Entidades.DocumentosCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            return query.Where(x => x.CTE.Codigo == codigoCTe && x.CTE.Empresa.Codigo == codigoEmpresa)
            .Fetch(x => x.ModeloDocumentoFiscal)
            .ToList();
        }

        public Dominio.Entidades.DocumentosCTE BuscarPorCTeENFe(int codigoCTe, string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.ChaveNFE == chaveNFe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DocumentosCTE> BuscarPorChaveCTe(int codigoEmpresa, string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.CTE.Chave == chaveCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.DocumentosCTE> BuscarPorChaveCTe(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.DocumentosCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            query = query.Where(o => o.CTE.Chave == chaveCTe);

            return query.ToList();
        }

        public Dominio.Entidades.DocumentosCTE BuscarPrimeiroPorChaveCTe(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.DocumentosCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            query = query.Where(o => o.CTE.Chave == chaveCTe);

            return query.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.DocumentoDACTE> BuscarParaDACTE(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query
                         where obj.CTE.Codigo == codigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa
                         select new Dominio.ObjetosDeValor.Relatorios.DocumentoDACTE()
                         {
                             ChaveNFE = obj.ChaveNFE,
                             CNPJEmitente = obj.CTE.Remetente.CPF_CNPJ,
                             Codigo = obj.Codigo,
                             Descricao = obj.DescricaoCTe,
                             Numero = obj.Numero,
                             NumeroModelo = obj.ModeloDocumentoFiscal.Numero,
                             Serie = obj.Serie
                         };

            return result.ToList();
        }

        public List<Dominio.Entidades.DocumentosCTE> BuscarPorCTe(int codigoEmpresa, int[] codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where codigoCTe.Contains(obj.CTE.Codigo) && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return result.Timeout(240).ToList();
        }

        public string BuscarNumeroNotaFiscal(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj.Numero;
            return result.FirstOrDefault();
        }

        public List<string> BuscarNumeroNotasFiscais(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            query = query.Where(o => o.CTE.Codigo == codigoCTe);

            return query.Select(o => o.Numero).ToList();
        }

        public List<string> BuscarNumeroReferenciaEDI(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            query = query.Where(o => o.CTE.Codigo == codigoCTe);

            return query.Select(o => o.NumeroReferenciaEDI).ToList();
        }

        public int ContarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj.Codigo;
            return result.Count();
        }

        public int ContarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj.Codigo;
            return result.Count();
        }

        public int ContarPorChaveEStatus(string chave, string[] statusCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            var result = from obj in query where obj.ChaveNFE.Equals(chave) && statusCTes.Contains(obj.CTE.Status) select obj.Codigo;

            return result.Count();
        }

        public Dominio.Entidades.DocumentosCTE BuscarPorCTeENumero(int codigoCTe, string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.Numero.Equals(numero) select obj;
            return result.FirstOrDefault();
        }

        public IList<int> BuscarNumeroDoCTePorChaveEEmpresa(int codigoEmpresa, string chaveNFe, int codigoCTe = 0)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.DocumentosCTE>();
            criteria.CreateAlias("CTE", "CTe");
            if (codigoCTe > 0)
                criteria.Add(Restrictions.Not(Restrictions.Eq("CTe.Codigo", codigoCTe)));
            criteria.Add(Restrictions.Eq("CTe.Empresa.Codigo", codigoEmpresa));
            criteria.Add(Restrictions.Eq("ChaveNFE", chaveNFe));
            criteria.SetProjection(Projections.Property("CTe.Numero"));
            return criteria.List<int>();
        }

        public IList<int> BuscarNumeroDoCTePorOutrosDocumentosEEmpresa(int codigoEmpresa, string numeroDocumento, int codigoCTe = 0)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.DocumentosCTE>();
            criteria.CreateAlias("CTE", "CTe");
            if (codigoCTe > 0)
                criteria.Add(Restrictions.Not(Restrictions.Eq("CTe.Codigo", codigoCTe)));
            criteria.Add(Restrictions.Eq("CTe.Empresa.Codigo", codigoEmpresa));
            criteria.Add(Restrictions.Eq("Numero", numeroDocumento));
            criteria.SetProjection(Projections.Property("CTe.Numero"));
            return criteria.List<int>();
        }


        public List<string> BuscarNumeroStatusDoCTePorChaveEEmpresa(int codigoEmpresa, string chaveNFe, int codigoCTe = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            var result = from obj in query where obj.CTE.Empresa.Codigo == codigoEmpresa && obj.ChaveNFE.Equals(chaveNFe) select obj.CTE;

            if (codigoCTe > 0)
                result = result.Where(obj => obj.Codigo != codigoCTe);

            return result.Select(o => o.Numero.ToString() + " (" + o.Status + ")").ToList();
        }

        public IList<string> BuscarNumeroStatusDoCTePorChaveEEmpresaQuery(int codigoEmpresa, string chaveNFe, int codigoCTe = 0)
        {
            string sqlQuery = $@"
                Select 
                    cast(cte.CON_NUM as nvarchar(50)) + ' (' + cte.CON_STATUS + ')'
                from 
                    T_CTE_DOCS DocumentosCTE
                    left join T_CTE cte on DocumentosCTE.CON_CODIGO = cte.CON_CODIGO
                where
                    cte.EMP_CODIGO = {codigoEmpresa} 
                    and DocumentosCTE.NFC_CHAVENFE = '{chaveNFe}'";

            if (codigoCTe > 0)
                sqlQuery += $" and cte.CON_CODIGO = {codigoCTe}";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return consulta.List<string>();
        }

        public List<int> BuscarNumeroDosCTes(int codigoEmpresa, string chaveNFe, string[] statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            var result = from obj in query where obj.CTE.Empresa.Codigo == codigoEmpresa && obj.ChaveNFE.Equals(chaveNFe) && statusCTe.Contains(obj.CTE.Status) select obj.CTE.Numero;

            return result.ToList();
        }

        public Dominio.Entidades.DocumentosCTE BuscarPorCodigoECTe(int codigo, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.Codigo == codigo && obj.CTE.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DocumentosCTE> BuscarPorCTeEModelos(int codigoCTe, List<string> numerosModelos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && numerosModelos.Contains(obj.ModeloDocumentoFiscal.Numero) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.DocumentosCTE> BuscarPorCTes(int[] codigoCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            var result = from obj in query where codigoCTes.Contains(obj.CTE.Codigo) select obj;
            return result.Fetch(o => o.CTE).ToList();
        }
        public Dominio.Entidades.DocumentosCTE BuscarPorNumeroSerieNFe(string Numero, string serie, string embarcador, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            var result = from obj in query
                         where obj.Numero == Numero
                            && obj.CTE.Status == "A" // autorizado
                         select obj;

            if (!string.IsNullOrEmpty(serie))
                result = result.Where(obj => obj.Serie == serie || obj.Serie == null);

            if (!string.IsNullOrEmpty(embarcador))
                result = result.Where(obj => obj.CTE.Remetente.CPF_CNPJ == embarcador);

            if (empresa > 0)
                result = result.Where(obj => obj.CTE.Empresa.Codigo == empresa);

            return result.FirstOrDefault();
        }


        public decimal BuscarPesoPorCTe(int codigoCTe, int codigoEmpresa)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.DocumentosCTE>();
            criteria.CreateAlias("CTE", "cte");
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("cte.Empresa.Codigo", codigoEmpresa));
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("cte.Codigo", codigoCTe));
            criteria.SetProjection(NHibernate.Criterion.Projections.Sum("Peso"));
            return criteria.UniqueResult<decimal>();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE DocumentosCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE DocumentosCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                    .SetInt32("codigoCTe", codigoCTe)
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

        public List<Dominio.Entidades.DocumentosCTE> BuscarPorCTes(IEnumerable<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.DocumentosCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            query = query.Where(o => codigosCTes.Contains(o.CTE.Codigo));

            return query.Fetch(o => o.ModeloDocumentoFiscal).ToList();
        }
    }
}
