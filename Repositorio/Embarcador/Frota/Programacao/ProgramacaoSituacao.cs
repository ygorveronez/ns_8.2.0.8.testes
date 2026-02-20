using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Frota.Programacao
{
    public class ProgramacaoSituacao : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao>
    {

        public ProgramacaoSituacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao> BuscarSituacoes(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao> finalidades)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao>();
            var result = from obj in query select obj;

            if (finalidades != null && finalidades.Count > 0)
                result = result.Where(o => finalidades.Contains(o.TipoEntidadeProgramacao));

            return result.ToList();
        }
        private IQueryable<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao> _Consultar(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao> finalidades, int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo == true);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => o.Ativo == false);

            if (finalidades != null && finalidades.Count > 0)
                result = result.Where(o => finalidades.Contains(o.TipoEntidadeProgramacao));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao> Consultar(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao> finalidades, int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(finalidades, codigoEmpresa, descricao, status);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao> finalidades, int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(finalidades, codigoEmpresa, descricao, status);

            return result.Count();
        }
    }
}
