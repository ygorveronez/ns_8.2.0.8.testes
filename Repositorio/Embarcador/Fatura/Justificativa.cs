using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class Justificativa : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.Justificativa>
    {
        public Justificativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fatura.Justificativa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Justificativa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Fatura.Justificativa BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Justificativa>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Fatura.Justificativa BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Justificativa>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Fatura.Justificativa BuscarPrimeiraJustificativa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipoJustificativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Justificativa>();
            var result = from obj in query where obj.TipoJustificativa == tipoJustificativa && obj.Ativo == true select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Fatura.Justificativa BuscarPorTipoMovimento(int codigoTipoMovimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipoJustificativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Justificativa>();

            var result = from obj in query where obj.TipoMovimentoUsoJustificativa.Codigo == codigoTipoMovimento && obj.Ativo == true select obj;
            result = result.Where(o => o.TipoJustificativa == tipoJustificativa);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Justificativa> BuscarTodasAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Justificativa>();

            var result = from obj in query where obj.Ativo == true select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Justificativa> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Justificativa>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Justificativa> Consultar(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa? tipoJustificativa, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa> finalidadesJustificativa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Justificativa>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(obj => obj.Ativo == true);
                else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(obj => obj.Ativo == false);
            }

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (tipoJustificativa.HasValue)
                query = query.Where(o => o.TipoJustificativa == tipoJustificativa.Value);

            if (finalidadesJustificativa != null && finalidadesJustificativa.Count > 0)
                query = query.Where(o => finalidadesJustificativa.Contains(o.FinalidadeJustificativa));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa? tipoJustificativa, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa> finalidadesJustificativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Justificativa>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(obj => obj.Ativo == true);
                else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(obj => obj.Ativo == false);
            }

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (tipoJustificativa.HasValue)
                query = query.Where(o => o.TipoJustificativa == tipoJustificativa.Value);

            if (finalidadesJustificativa != null && finalidadesJustificativa.Count > 0)
                query = query.Where(o => finalidadesJustificativa.Contains(o.FinalidadeJustificativa));

            return query.Count();
        }
    }
}
