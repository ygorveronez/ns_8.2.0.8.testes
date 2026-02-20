using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloBaixaAgrupadoDocumentoAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto>
    {
        public TituloBaixaAgrupadoDocumentoAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto> BuscarPorTituloBaixaAgrupadoDocumento(int codigoTituloBaixaAgrupadoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.TituloBaixaAgrupadoDocumento.Codigo == codigoTituloBaixaAgrupadoDocumento);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto> BuscarPorTituloBaixa(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.TituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.Fetch(o => o.TituloBaixaAgrupadoDocumento)
                        .ThenFetch(o => o.TituloBaixaAgrupado)
                        .Fetch(o => o.TipoMovimentoUso)
                        .ThenFetch(o => o.PlanoDeContaDebito)
                        .Fetch(o => o.TipoMovimentoUso)
                        .ThenFetch(o => o.PlanoDeContaCredito)
                        .Fetch(o => o.TipoMovimentoReversao)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto> Consultar(int codigoTituloBaixaAgrupadoDocumento, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.TituloBaixaAgrupadoDocumento.Codigo == codigoTituloBaixaAgrupadoDocumento);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoTituloBaixaAgrupadoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.TituloBaixaAgrupadoDocumento.Codigo == codigoTituloBaixaAgrupadoDocumento);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto BuscarPorTituloBaixaAgrupadoDocumentoComVariacaoCambial(int codigoTituloBaixaAgrupadoDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.TituloBaixaAgrupadoDocumento.Codigo == codigoTituloBaixaAgrupadoDocumento && o.VariacaoCambial);

            return query.FirstOrDefault();
        }
    }
}
