using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public sealed class ValeAvulso : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ValeAvulso>
    {
        public ValeAvulso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.ValeAvulso BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ValeAvulso>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var consultaValeAvulso = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ValeAvulso>();

            int? ultimo = consultaValeAvulso.Max(o => (int?)o.NumeroVale);

            return ultimo.HasValue ? ultimo.Value + 1 : 2;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.ValeAvulso> Consultar(string numero, double codigoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValeAvulso? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoValeAvulso? tipoDocumento, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ValeAvulso>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(o => o.Numero.Equals(numero));

            if (codigoPessoa > 0)
                result = result.Where(o => o.Pessoa.CPF_CNPJ.Equals(codigoPessoa));

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValeAvulso.Todos)
                result = result.Where(o => o.Situacao == situacao.Value);

            if (tipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoValeAvulso.Todos)
                result = result.Where(o => o.TipoDocumento == tipoDocumento.Value);

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string numero, double codigoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValeAvulso? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoValeAvulso? tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ValeAvulso>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(o => o.Numero.Equals(numero));

            if (codigoPessoa > 0)
                result = result.Where(o => o.Pessoa.CPF_CNPJ.Equals(codigoPessoa));

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValeAvulso.Todos)
                result = result.Where(o => o.Situacao == situacao.Value);

            if (tipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoValeAvulso.Todos)
                result = result.Where(o => o.TipoDocumento == tipoDocumento.Value);

            return result.Count();
        }
    }
}
