using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio
{
    public class NaturezaNFSe : RepositorioBase<Dominio.Entidades.NaturezaNFSe>, Dominio.Interfaces.Repositorios.NaturezaNFSe
    {
        public NaturezaNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.NaturezaNFSe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaNFSe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NaturezaNFSe BuscarPorNumero(int codigoEmpresa, int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaNFSe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NaturezaNFSe BuscarPorNumeroELocalidade(int numero, int localidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaNFSe>();

            var result = from obj in query where obj.Numero == numero && obj.Localidade.Codigo == localidade select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.NaturezaNFSe> BuscarPorEmpresa(int codigoEmpresa, int codigoEmpresaPai, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaNFSe>();

            var result = from obj in query where (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai) && obj.Status.Equals(status) select obj;

            return result.ToList();
        }
        public Dominio.Entidades.NaturezaNFSe BuscarPorDescricaoELocalidade(string descricao, int localidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaNFSe>();

            var result = from obj in query select obj;
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao) && o.Localidade.Codigo == localidade);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.NaturezaNFSe> Consultar(string descricao, int localidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida, bool? dentroEstado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaNFSe>();
            var queryNaturezaDaOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(o => o.Status.Contains("A"));
                else
                    result = result.Where(o => o.Status.Contains("I"));
            }

            if (localidade > 0)
                result = result.Where(obj => obj.Localidade.Codigo == localidade);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            if (dentroEstado != null)
            {
                var resultNaturezaDaOperacao = from obj in queryNaturezaDaOperacao where obj.Status == "A" && obj.DentroEstado == dentroEstado select obj;
                result = result.Where(obj => resultNaturezaDaOperacao.Any(o => o.NaturezaNFSe.Codigo == obj.Codigo));
            }

            if (tipoEntradaSaida > 0)
            {
                var resultNaturezaDaOperacao = from obj in queryNaturezaDaOperacao where obj.Status == "A" && (obj.Tipo == tipoEntradaSaida || obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Todos) select obj;
                result = result.Where(obj => resultNaturezaDaOperacao.Any(o => o.NaturezaNFSe.Codigo == obj.Codigo));
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, int localidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida, bool? dentroEstado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaNFSe>();
            var queryNaturezaDaOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(o => o.Status.Contains("A"));
                else
                    result = result.Where(o => o.Status.Contains("I"));
            }

            if (localidade > 0)
                result = result.Where(obj => obj.Localidade.Codigo == localidade);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            if (dentroEstado != null)
            {
                var resultNaturezaDaOperacao = from obj in queryNaturezaDaOperacao where obj.Status == "A" && obj.DentroEstado == dentroEstado select obj;
                result = result.Where(obj => resultNaturezaDaOperacao.Any(o => o.NaturezaNFSe.Codigo == obj.Codigo));
            }

            if (tipoEntradaSaida > 0)
            {
                var resultNaturezaDaOperacao = from obj in queryNaturezaDaOperacao where obj.Status == "A" && (obj.Tipo == tipoEntradaSaida || obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Todos) select obj;
                result = result.Where(obj => resultNaturezaDaOperacao.Any(o => o.NaturezaNFSe.Codigo == obj.Codigo));
            }

            return result.Count();
        }

        public List<Dominio.Entidades.NaturezaNFSe> Consultar(int codigoEmpresa, int codigoEmpresaPai, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaNFSe>();

            var result = from obj in query where (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai) select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Contains(status));

            return result.OrderBy(o => o.Descricao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoEmpresaPai, string descricao, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaNFSe>();

            var result = from obj in query where (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai) select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Contains(status));

            return result.Count();
        }
    }
}
