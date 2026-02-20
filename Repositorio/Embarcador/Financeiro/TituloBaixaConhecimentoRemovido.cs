using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Financeiro
{
    public class TituloBaixaConhecimentoRemovido : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido>
    {
        public TituloBaixaConhecimentoRemovido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido BuscarCTePorBaixa(int codigoBaixa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigoBaixa && obj.CTe.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemConhecimentoRemovido(int codigo, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo && obj.CTe.Codigo == codigoCTe select obj;
            return result.Count() > 0;
        }

        public bool ContemConhecimentoRemovido(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido> BuscarPorBaixaTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido> BuscarPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigoTituloBaixa select obj;
            return result.ToList();
        }

        public int ContarBuscarPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigoTituloBaixa select obj;
            return result.Count();
        }


    }

}
