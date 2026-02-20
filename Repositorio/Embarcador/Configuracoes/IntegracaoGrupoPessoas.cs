using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoGrupoPessoas : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa>
    {
        public IntegracaoGrupoPessoas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> BuscarPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa>();

            query = query.Where(o => o.TipoIntegracao.Tipo == tipoIntegracao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa>();

            query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> BuscarPorGruposPessoas(List<int> codigosGruposPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa>();

            query = query.Where(o => codigosGruposPessoas.Contains(o.GrupoPessoas.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa BuscarPorGrupoPessoasETipoIntegracao(int codigoGrupoPessoas, int codigoTipoIntegracoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa>();

            query = query.Where(o => o.TipoIntegracao.Codigo == codigoTipoIntegracoes && o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.FirstOrDefault();
        }

        public bool ContemPorGrupoPessoasETipoIntegracao(int codigoGrupoPessoas, int codigoTipoIntegracoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa>();

            query = query.Where(o => o.TipoIntegracao.Codigo == codigoTipoIntegracoes && o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.Any();
        }
    }
}
