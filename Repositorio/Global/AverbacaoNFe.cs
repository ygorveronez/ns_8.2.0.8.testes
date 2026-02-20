using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class AverbacaoNFe : RepositorioBase<Dominio.Entidades.AverbacaoNFe>
    {
        public AverbacaoNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.AverbacaoNFe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.AverbacaoNFe BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.AverbacaoNFe BuscarPorChaveNFe(string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query where obj.XMLNotaFiscal.Chave == chaveNFe select obj;

            return result.FirstOrDefault();
        }


        public int ContarPorNFe(int codigoNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoNFe select obj;

            return result.Count();
        }

        public int ContarPorNFeEStatus(int codigoNFe, Dominio.Enumeradores.StatusAverbacaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoNFe && obj.Status == status select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.AverbacaoNFe> BuscarPorNFe(int codigoNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoNFe select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.AverbacaoNFe> BuscarPraReenviar(int[] nfes, Dominio.Enumeradores.StatusAverbacaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query where nfes.Contains(obj.XMLNotaFiscal.Codigo) && obj.Status == status select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }


        public List<Dominio.Entidades.AverbacaoNFe> BuscarPorNFe(int codigoEmpresa, int codigoNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoNFe && obj.XMLNotaFiscal.Empresa.Codigo == codigoEmpresa select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }


        public Dominio.Entidades.AverbacaoNFe BuscarPorNFesTipoESituacao(int codigoNFe, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Enumeradores.StatusAverbacaoCTe situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.Codigo == codigoNFe &&
                            obj.Tipo == tipo &&
                            obj.Status == situacao
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.AverbacaoNFe> BuscarPorNFeCanceladosESituacao(int codigoEmpresa, int codigoNFe, Dominio.Enumeradores.StatusAverbacaoCTe situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.Codigo == codigoNFe &&
                            obj.Status == situacao
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.AverbacaoNFe> BuscarPorNFeESituacao(int codigoNFe, Dominio.Enumeradores.StatusAverbacaoCTe situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.Codigo == codigoNFe &&
                            obj.Status == situacao
                         select obj;

            return result.ToList();
        }

        public int ContarPorNFeTipoEStatus(int codigoNFe, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Enumeradores.StatusAverbacaoCTe[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoNFe && obj.Tipo == tipo && status.Contains(obj.Status) select obj;

            return result.Count();
        }


        public int ContarPorCTeEStatus(int codigoNFe, Dominio.Enumeradores.StatusAverbacaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoNFe && obj.Status == status select obj;

            return result.Count();
        }


        public List<int> BuscarAverbacoes(Dominio.Enumeradores.StatusAverbacaoCTe situacaoAverbacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFe>();

            query = query.Where(o => o.Status == situacaoAverbacao);

            return query.Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }
    }
}
