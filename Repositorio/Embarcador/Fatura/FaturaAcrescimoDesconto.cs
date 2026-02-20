using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto>
    {
        public FaturaAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto> BuscarDescontosFatura(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto>();
            var result = from obj in query where obj.Fatura.Codigo == codigo && obj.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto> BuscarAcrescimosFatura(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto>();
            var result = from obj in query where obj.Fatura.Codigo == codigo && obj.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto> ConsultarAcrescimoDesconto(int codigoFatura, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarAcrescimoDesconto(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura select obj;

            return result.Count();
        }
    }
}
