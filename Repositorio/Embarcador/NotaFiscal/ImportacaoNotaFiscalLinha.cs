using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ImportacaoNotaFiscalLinha : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha>
    {
        public ImportacaoNotaFiscalLinha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha> BuscarLinhasPendentesImportacao(int codigoImportacao, int quantidade)
        {
            IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha>();

            query = query.Where(o => o.ImportacaoNotaFiscal.Codigo == codigoImportacao && o.Situacao == SituacaoImportacaoNotaFiscal.Pendente);
            
            return query.Take(quantidade).ToList();
        }

        public int ContarLinhas(int codigoImportacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha>();

            query = query.Where(o => o.ImportacaoNotaFiscal.Codigo == codigoImportacao);

            return query.Count();
        }

        public int ContarLinhasPendentes(int codigoImportacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha>();

            query = query.Where(o => o.ImportacaoNotaFiscal.Codigo == codigoImportacao && o.Situacao == SituacaoImportacaoNotaFiscal.Pendente);

            return query.Count();
        }

        public int ContarPedidosProcessados(int codigoImportacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha>();

            query = query.Where(o => o.ImportacaoNotaFiscal.Codigo == codigoImportacao && o.Situacao == SituacaoImportacaoNotaFiscal.Sucesso && o.Pedido != null);

            return query.Count();
        }

        public int ContarNotasProcessadas(int codigoImportacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha>();

            query = query.Where(o => o.ImportacaoNotaFiscal.Codigo == codigoImportacao && o.Situacao == SituacaoImportacaoNotaFiscal.Sucesso && o.XMLNotaFiscal != null);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha> BuscarPorImportacao(int codigoImportacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha>();

            query = query.Where(o => o.ImportacaoNotaFiscal.Codigo == codigoImportacao);
            
            return query.Fetch(o => o.Pedido).Fetch(o => o.XMLNotaFiscal).OrderBy(o => o.Numero).ToList();
        }
    }
}
