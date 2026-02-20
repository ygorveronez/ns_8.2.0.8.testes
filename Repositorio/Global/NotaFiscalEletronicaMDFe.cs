using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class NotaFiscalEletronicaMDFe : RepositorioBase<Dominio.Entidades.NotaFiscalEletronicaMDFe>, Dominio.Interfaces.Repositorios.NotaFiscalEletronicaMDFe
    {
        public NotaFiscalEletronicaMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public NotaFiscalEletronicaMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<string> ObterNumerosMDFesPorChaveNFe(int codigoEmpresa, string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalEletronicaMDFe>();

            var result = from obj in query where obj.Chave.Equals(chaveNFe) && obj.MunicipioDescarregamento.MDFe.Empresa.Codigo == codigoEmpresa && obj.MunicipioDescarregamento.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado select obj.MunicipioDescarregamento.MDFe.Numero + " - " + obj.MunicipioDescarregamento.MDFe.Serie.Numero;

            return result.ToList();
        }

        public List<string> ObterNumerosMDFesPorChaveNFe(int codigoEmpresa, string chaveNFe, int codigoMunicipioDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalEletronicaMDFe>();

            var result = from obj in query
                         where obj.Chave.Equals(chaveNFe) &&
                                                 obj.MunicipioDescarregamento.MDFe.Empresa.Codigo == codigoEmpresa &&
                                                 obj.MunicipioDescarregamento.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                                                 obj.MunicipioDescarregamento.Codigo != codigoMunicipioDescarregamento
                         select obj.MunicipioDescarregamento.MDFe.Numero + " - " + obj.MunicipioDescarregamento.MDFe.Serie.Numero;

            return result.ToList();
        }

        public List<Dominio.Entidades.NotaFiscalEletronicaMDFe> BuscarPorMunicipio(int codigoMunicipio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalEletronicaMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.Codigo == codigoMunicipio select obj;

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.NotaFiscalEletronicaMDFe>> BuscarPorMunicipioAsync(int codigoMunicipio, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalEletronicaMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.Codigo == codigoMunicipio select obj;

            return result.ToListAsync(cancellationToken);
        }

        public void DeletarPorMunicipio(int codigoMunicipioDescarregamento)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE NotaFiscalEletronicaMDFe obj WHERE obj.MunicipioDescarregamento.Codigo = :codigoMunicipioDescarregamento")
                                     .SetInt32("codigoMunicipioDescarregamento", codigoMunicipioDescarregamento)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE NotaFiscalEletronicaMDFe obj WHERE obj.MunicipioDescarregamento.Codigo = :codigoMunicipioDescarregamento")
                                    .SetInt32("codigoMunicipioDescarregamento", codigoMunicipioDescarregamento)
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

        public Dominio.Entidades.NotaFiscalEletronicaMDFe Buscar(int codigoNotaFiscal, int codigoMunicipio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalEletronicaMDFe>();

            var result = from obj in query where obj.Codigo == codigoNotaFiscal && obj.MunicipioDescarregamento.Codigo == codigoMunicipio select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosDocumentoMDFe> BuscarDocumentosParaRelatorio(int[] codigosMDFes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalEletronicaMDFe>();

            var result = from obj in query
                         where codigosMDFes.Contains(obj.MunicipioDescarregamento.MDFe.Codigo)
                         select new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosDocumentoMDFe()
                         {
                             CodigoMDFe = obj.MunicipioDescarregamento.MDFe.Codigo,
                             Chave = obj.Chave,
                             UFDescarregamento = obj.MunicipioDescarregamento.Municipio.Estado.Sigla,
                             MunicipioDescarregamento = obj.MunicipioDescarregamento.Municipio.Descricao
                         };

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.DocumentoMDFe> BuscarDocumentosParaDAMDFE(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalEletronicaMDFe>();

            var result = from obj in query
                         where obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe
                         select new Dominio.ObjetosDeValor.Relatorios.DocumentoMDFe()
                         {
                             Chave = obj.Chave,
                             Tipo = "NF-e",
                             CNPJEmitente = obj.MunicipioDescarregamento.MDFe.Empresa.CNPJ
                         };

            return result.ToList();
        }
    }
}
