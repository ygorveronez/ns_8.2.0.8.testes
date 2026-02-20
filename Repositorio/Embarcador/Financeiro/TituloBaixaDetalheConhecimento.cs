using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloBaixaDetalheConhecimento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento>
    {
        public TituloBaixaDetalheConhecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento> BuscarPorBaixa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento BuscarPorCTeBaixa(int codigoCTe, int codigoBaixaTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.TituloBaixa.Codigo == codigoBaixaTitulo select obj;
            return result.FirstOrDefault();
        }

        public decimal TotalDescontoPorTituloBaixa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            if (result.Count() > 0)
                return (from obj in result select obj.ValorDesconto).Sum();
            else
                return 0;
        }

        public decimal TotalAcrescimoPorTituloBaixa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            if (result.Count() > 0)
                return (from obj in result select obj.ValorAcrescimo).Sum();
            else
                return 0;
        }

        public decimal TotalPagoPorTituloBaixa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            if (result.Count() > 0)
                return (from obj in result select obj.ValorPago).Sum();
            else
                return 0;
        }
    }
}
