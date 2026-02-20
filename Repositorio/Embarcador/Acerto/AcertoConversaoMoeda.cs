using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Acerto
{
    public class AcertoConversaoMoeda : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda>
    {
        public AcertoConversaoMoeda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarValorMoedaestrangeira(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda>();
            query = query.Where(obj => obj.AcertoViagem.Codigo == codigo);

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && (obj.MoedaCotacaoBancoCentralDestino == null || moedas.Contains(obj.MoedaCotacaoBancoCentralDestino)));
            else
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && obj.MoedaCotacaoBancoCentralDestino != null && moedas.Contains(obj.MoedaCotacaoBancoCentralDestino));
            return query.Sum(o => (decimal?)o.ValorFinal) ?? 0m;
        }

        public decimal BuscarValorMoedaEstrangeiraOrigem(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda>();
            query = query.Where(obj => obj.AcertoViagem.Codigo == codigo);

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && (obj.MoedaCotacaoBancoCentralOrigem == null || moedas.Contains(obj.MoedaCotacaoBancoCentralOrigem)));
            else
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && obj.MoedaCotacaoBancoCentralOrigem != null && moedas.Contains(obj.MoedaCotacaoBancoCentralOrigem));

            return query.Sum(o => (decimal?)o.ValorOrigem) ?? 0m;
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda BuscarPorAcertoeMoeda(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaOrigem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.MoedaCotacaoBancoCentralDestino == moeda && obj.MoedaCotacaoBancoCentralOrigem == moedaOrigem select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda> BuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda> BuscarPorAcerto(int codigoAcerto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.Count();
        }
    }
}
