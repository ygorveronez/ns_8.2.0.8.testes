using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class AverbacaoNFSe : RepositorioBase<Dominio.Entidades.AverbacaoNFSe>
    {
        public AverbacaoNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.AverbacaoNFSe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.AverbacaoNFSe BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }


        public int ContarPorNFSe(int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe select obj;

            return result.Count();
        }

        public int ContarPorNFSeEStatus(int codigoNFSe, Dominio.Enumeradores.StatusAverbacaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.Status == status select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.AverbacaoNFSe> BuscarPorNFSe(int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.AverbacaoNFSe> BuscarPraReenviar(int[] nfses, Dominio.Enumeradores.StatusAverbacaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query where nfses.Contains(obj.NFSe.Codigo) && obj.Status == status select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }


        public List<Dominio.Entidades.AverbacaoNFSe> BuscarPorNFSe(int codigoEmpresa, int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.NFSe.Empresa.Codigo == codigoEmpresa select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }


        public Dominio.Entidades.AverbacaoNFSe BuscarPorNFSesTipoESituacao(int codigoNFSe, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Enumeradores.StatusAverbacaoCTe situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query
                         where
                            obj.NFSe.Codigo == codigoNFSe &&
                            obj.Tipo == tipo &&
                            obj.Status == situacao
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.AverbacaoNFSe> BuscarPorNFSeCanceladosESituacao(int codigoEmpresa, int codigoNFSe, Dominio.Enumeradores.StatusAverbacaoCTe situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query
                         where
                            obj.NFSe.Codigo == codigoNFSe &&
                            obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Cancelado && 
                            obj.Status == situacao
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.AverbacaoNFSe> BuscarPorNFSeESituacao(int codigoNFSe, Dominio.Enumeradores.StatusAverbacaoCTe situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query
                         where
                            obj.NFSe.Codigo == codigoNFSe &&
                            obj.Status == situacao
                         select obj;

            return result.ToList();
        }

        public int ContarPorNFSeTipoEStatus(int codigoNFSe, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Enumeradores.StatusAverbacaoCTe[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.Tipo == tipo && status.Contains(obj.Status) select obj;

            return result.Count();
        }

        
        public int ContarPorCTeEStatus(int codigoNFSe, Dominio.Enumeradores.StatusAverbacaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.Status == status select obj;

            return result.Count();
        }


        public List<int> BuscarAverbacoes(Dominio.Enumeradores.StatusAverbacaoCTe situacaoAverbacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoNFSe>();

            query = query.Where(o => o.Status == situacaoAverbacao);

            return query.Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }
    }
}
