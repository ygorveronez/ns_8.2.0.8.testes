using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoVariacaoCambial : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial>
    {
        public AcertoVariacaoCambial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarValorMoedaestrangeira(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial>();
            query = query.Where(obj => obj.AcertoViagem.Codigo == codigo);

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && (obj.MoedaCotacaoBancoCentral == null || moedas.Contains(obj.MoedaCotacaoBancoCentral.Value)));
            else
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && obj.MoedaCotacaoBancoCentral != null && moedas.Contains(obj.MoedaCotacaoBancoCentral.Value));

            return query.Sum(o => (decimal?)o.ValorVariacao) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial> BuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial> BuscarPorAcerto(int codigoAcerto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.Count();
        }
    }
}
