using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ProcessoMovimento : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento>
    {
        public ProcessoMovimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento tipoProcesso, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if ((int)tipoProcesso > 0)
                result = result.Where(obj => obj.TipoProcessoMovimento == tipoProcesso);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento tipoProcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if ((int)tipoProcesso > 0)
                result = result.Where(obj => obj.TipoProcessoMovimento == tipoProcesso);

            return result.Count();
        }

    }
}
