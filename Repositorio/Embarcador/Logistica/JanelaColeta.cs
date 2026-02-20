using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class JanelaColeta : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.JanelaColeta>
    {
        public JanelaColeta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.JanelaColeta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JanelaColeta>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }
        public Dominio.Entidades.Embarcador.Logistica.JanelaColeta BuscarPorCliente(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JanelaColeta>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Clientes.Any(o => o.CPF_CNPJ == cpfCnpj) && ent.Ativo);

            return result.Fetch(obj => obj.PeriodosColeta).FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Logistica.JanelaColeta BuscarPorLocalidade(int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JanelaColeta>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Localidades.Any(o => o.Codigo == codigoLocalidade) && ent.Ativo);
            

            return result.Fetch(obj=> obj.PeriodosColeta).FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Logistica.JanelaColeta BuscarPorEstado(string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JanelaColeta>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Estados.Any(o => o.Sigla == sigla) && ent.Ativo);

            return result.Fetch(obj => obj.PeriodosColeta).FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.JanelaColeta> Consultar(string descricao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JanelaColeta>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));


            return consulta;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.JanelaColeta> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(descricao);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(string descricao)
        {
            var consulta = Consultar(descricao);

            return consulta.Count();
        }

    }


}
