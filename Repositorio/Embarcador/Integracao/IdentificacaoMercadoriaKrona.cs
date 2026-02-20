using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Integracao
{
    public sealed class IdentificacaoMercadoriaKrona : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona>
    {
        #region Construtores

        public IdentificacaoMercadoriaKrona(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona> Consultar(int identificador, string identificadorDescricao, SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaIdentificacaoMercadoriaKrona = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona>();

            if (identificador > 0)
                consultaIdentificacaoMercadoriaKrona = consultaIdentificacaoMercadoriaKrona.Where(o => o.Identificador == identificador);

            if (!string.IsNullOrWhiteSpace(identificadorDescricao))
                consultaIdentificacaoMercadoriaKrona = consultaIdentificacaoMercadoriaKrona.Where(o => o.IdentificadorDescricao.Contains(identificadorDescricao));

            if (situacaoAtivo == SituacaoAtivoPesquisa.Ativo)
                consultaIdentificacaoMercadoriaKrona = consultaIdentificacaoMercadoriaKrona.Where(o => o.Ativo);
            else if (situacaoAtivo == SituacaoAtivoPesquisa.Inativo)
                consultaIdentificacaoMercadoriaKrona = consultaIdentificacaoMercadoriaKrona.Where(o => !o.Ativo);

            return consultaIdentificacaoMercadoriaKrona;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona> Consultar(int identificador, string identificadorDescricao, SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIdentificacaoMercadoriaKrona = Consultar(identificador, identificadorDescricao, situacaoAtivo);

            return ObterLista(consultaIdentificacaoMercadoriaKrona, parametrosConsulta);
        }

        public int ContarConsulta(int identificador, string identificadorDescricao, SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaIdentificacaoMercadoriaKrona = Consultar(identificador, identificadorDescricao, situacaoAtivo);

            return consultaIdentificacaoMercadoriaKrona.Count();
        }

        #endregion
    }
}
