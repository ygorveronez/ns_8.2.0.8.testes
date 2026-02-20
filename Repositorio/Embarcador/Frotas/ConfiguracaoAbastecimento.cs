using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frotas
{
    public class ConfiguracaoAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento>
    {

        public ConfiguracaoAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento tipoImportacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (tipoImportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.EDI || tipoImportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.PostoAmigao || tipoImportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.PostoReforco)
                result = result.Where(obj => obj.TipoImportacaoAbastecimento == tipoImportacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento tipoImportacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (tipoImportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.EDI || tipoImportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.PostoAmigao || tipoImportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.PostoReforco)
                result = result.Where(obj => obj.TipoImportacaoAbastecimento == tipoImportacao);

            return result.Count();
        }
    }
}
