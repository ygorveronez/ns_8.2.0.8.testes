using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ArquivoImportacaoNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal>
    {
        public ArquivoImportacaoNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal> Consultar(string descricao, SituacaoAtivoPesquisa ativo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao == descricao);

            if (ativo == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (ativo == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao == descricao);

            if (ativo == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (ativo == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }
    }
}
