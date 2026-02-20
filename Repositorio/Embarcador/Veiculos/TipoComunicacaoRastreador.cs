using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class TipoComunicacaoRastreador : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador>
    {
        public TipoComunicacaoRastreador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador> ConsultarAtivos(string descricao)
        {
            return Consultar(descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, "Descricao", "asc", 0, 1000);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador ConsultarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador>();
            query = query.Where(obj => obj.Descricao.Contains(descricao));
            return query.FirstOrDefault();
        }

    }
}
