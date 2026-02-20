using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CTe
{
    public class ControleGeracaoCTeAnulacao : RepositorioBase<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao>
    {
        public ControleGeracaoCTeAnulacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao> BuscarPorSituacaoEStatusCTeAnulacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao situacao, string statusCTeAnulacao, int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao>();

            query = query.Where(o => o.Situacao == situacao && (o.CargaCTeAnulacao.CTe.Status == statusCTeAnulacao || o.CargaCTeAnulacao == null));

            return query.Take(quantidadeRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao> BuscarPorSituacaoEStatusCTeSubstituicao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao situacao, string statusCTeSubstituicao, int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao>();

            query = query.Where(o => o.Situacao == situacao && (o.NaoGerarCTeSubstituicao || o.CargaCTeSubstituicao.CTe.Status == statusCTeSubstituicao));

            return query.Take(quantidadeRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao> BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao situacao, int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao>();

            query = query.Where(o => o.Situacao == situacao);

            return query.Take(quantidadeRegistros).ToList();
        }

        public bool ExistePendenteGeracaoPorCarga(int codigoControleAnulacaoAtual, int codigoCargaAtual)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao>();

            query = query.Where(o => o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao.Finalizado
                                    && o.Codigo != codigoControleAnulacaoAtual
                                    && o.CargaCTeOriginal.Carga.Codigo == codigoCargaAtual);

            return query.Any();
        }
    }
}
