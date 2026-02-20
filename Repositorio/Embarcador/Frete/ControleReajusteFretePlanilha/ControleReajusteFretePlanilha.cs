using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ControleReajusteFretePlanilha : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha>
    {

        public ControleReajusteFretePlanilha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha>();
            var result = from obj in query select obj.Numero;

            int maiorNumero = 0;
            if (result.Count() > 0)
                maiorNumero = result.Max();

            return maiorNumero + 1;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha> _Consultar(int numero, int tipoOperacao, int filial, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha situacaoControleReajusteFretePlanilha)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha>();

            var result = from obj in query select obj;

            // Filtros
            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (tipoOperacao > 0)
                result = result.Where(o => o.TipoOperacao.Codigo == tipoOperacao);

            if (filial > 0)
                result = result.Where(o => o.Filial.Codigo == filial);

            if (empresa > 0)
                result = result.Where(o => o.Empresa.Codigo == empresa);

            if (situacaoControleReajusteFretePlanilha != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.Todas)
                result = result.Where(o => o.Situacao == situacaoControleReajusteFretePlanilha);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha> Consultar(int numero, int tipoOperacao, int filial, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha situacaoControleReajusteFretePlanilha, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(numero, tipoOperacao, filial, empresa, situacaoControleReajusteFretePlanilha);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int numero, int tipoOperacao, int filial, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha situacaoControleReajusteFretePlanilha)
        {
            var result = _Consultar(numero, tipoOperacao, filial, empresa, situacaoControleReajusteFretePlanilha);

            return result.Count();
        }

        public int ContarAprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>();
            var resut = from obj in query
                        where
                            obj.ControleReajusteFretePlanilha.Codigo == codigo
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                        select obj;

            return resut.Count();
        }

        public int ContarReprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>();
            var resut = from obj in query
                        where
                            obj.ControleReajusteFretePlanilha.Codigo == codigo
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                        select obj;

            return resut.Count();
        }
    }
}
