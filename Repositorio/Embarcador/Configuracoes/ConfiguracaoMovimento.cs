using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoMovimento : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento>
    {
        public ConfiguracaoMovimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento BuscarPorTipoMovimento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento tipoMovimento, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento>();
            var result = from obj in query where obj.Ativo == true select obj;
            if (codigoFilial > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoFilial);                

            var queryTipo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento>();
            var resultTipo = from obj in queryTipo where obj.Ativo == true && obj.TipoProcessoMovimento == tipoMovimento select obj;

            result = result.Where(obj => resultTipo.Contains(obj.ProcessoMovimento));

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int processoMovimento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (processoMovimento > 0)
                result = result.Where(obj => obj.ProcessoMovimento.Codigo == processoMovimento);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int processoMovimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (processoMovimento > 0)
                result = result.Where(obj => obj.ProcessoMovimento.Codigo == processoMovimento);

            return result.Count();
        }

    }
}
