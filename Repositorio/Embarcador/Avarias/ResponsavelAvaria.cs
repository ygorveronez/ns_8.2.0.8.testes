using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class ResponsavelAvaria : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria>
    {
        public ResponsavelAvaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria> BuscarPorLoteUsario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria>();
            var result = from obj in query where obj.SolicitacaoAvaria.Lote.Codigo == codigo && obj.Usuario.Codigo == usuario select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> ResponsavelSolicitacao(int avaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria>();

            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == avaria select obj.Usuario;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria BuscaPorUsuarioAvaria(int avaria, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria>();

            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == avaria && obj.Usuario.Codigo == usuario select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria> ResponsavelLotes(List<int> lotes)
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria>();

            result = result.Where(obj => lotes.Contains(obj.SolicitacaoAvaria.Lote.Codigo));

            return result
                .Fetch(obj => obj.Usuario)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ToList();
        }

        public List<Dominio.Entidades.Usuario> ResponsavelLote(int lote)
        {
            var queryUsuarios = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var resultUsuarios = from obj in queryUsuarios where obj.SolicitacaoAvaria.Lote.Codigo == lote select obj.Usuario;
            var result = from obj in query where resultUsuarios.Contains(obj) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria> AprovacoesLote(List<int> lotes, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria>();

            var result = from obj in query where lotes.Contains(obj.Codigo) && obj.Usuario.Codigo == usuario select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscaPorSolicitacao(int codSolicitacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria>();

            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == codSolicitacao select obj.Usuario;

            return result.ToList();
        }
    }
}