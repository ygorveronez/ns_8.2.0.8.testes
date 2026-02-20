using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;


namespace AdminMultisoftware.Repositorio.Pessoas
{
    public class ClienteURLAcesso : RepositorioBase<Dominio.Entidades.Pessoas.ClienteURLAcesso>
    {
        public ClienteURLAcesso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Pessoas.ClienteURLAcesso BuscarPorURL(string url)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.ClienteURLAcesso>();
            var result = from obj in query where obj.URLAcesso == url select obj;
            return result
                    .Fetch(obj => obj.Cliente).ThenFetch(obj => obj.ClienteConfiguracao)
                    .Fetch(obj => obj.Cliente).ThenFetch(obj => obj.ClienteConfiguracaoHomologacao)
                    .FirstOrDefault();
        }

        public Dominio.Entidades.Pessoas.ClienteURLAcesso BuscarPorClienteETipo(int codigoCliente, Dominio.Enumeradores.TipoServicoMultisoftware tipoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.ClienteURLAcesso>();
            var result = from obj in query where obj.Cliente.Codigo == codigoCliente && obj.TipoServicoMultisoftware == tipoServico && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Pessoas.ClienteURLAcesso BuscarPorClienteETipoProducao(int codigoCliente, Dominio.Enumeradores.TipoServicoMultisoftware tipoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.ClienteURLAcesso>();
            var result = from obj in query where obj.Cliente.Codigo == codigoCliente && obj.TipoServicoMultisoftware == tipoServico && obj.Ativo && !obj.URLHomologacao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Pessoas.ClienteURLAcesso> BuscarPorClientes(List<int> clientes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.ClienteURLAcesso>();
            var result = from obj in query where clientes.Contains(obj.Cliente.Codigo) && obj.Ativo select obj;
            return result
                    .Fetch(obj => obj.Cliente).ThenFetch(obj => obj.ClienteConfiguracao)
                    .Fetch(obj => obj.Cliente).ThenFetch(obj => obj.ClienteConfiguracaoHomologacao)
                    .ToList();
        }

        public List<Dominio.Entidades.Pessoas.ClienteURLAcesso> Consultar(int codigoCliente, string urlAcesso, Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, AdminMultisoftware.Dominio.Enumeradores.SituacaoAtivoPesquisa situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.ClienteURLAcesso>();

            var result = from obj in query select obj;

            if (codigoCliente > 0)
                result = result.Where(obj => obj.Cliente.Codigo.Equals(codigoCliente));

            if (!string.IsNullOrWhiteSpace(urlAcesso))
                result = result.Where(obj => obj.URLAcesso.Contains(urlAcesso));

            if (tipoServico != 0)
                result = result.Where(obj => obj.TipoServicoMultisoftware == tipoServico);

            if (situacao == Dominio.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoCliente, string urlAcesso, Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, AdminMultisoftware.Dominio.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.ClienteURLAcesso>();

            var result = from obj in query select obj;

            if (codigoCliente > 0)
                result = result.Where(obj => obj.Cliente.Codigo.Equals(codigoCliente));

            if (!string.IsNullOrWhiteSpace(urlAcesso))
                result = result.Where(obj => obj.URLAcesso.Contains(urlAcesso));

            if (tipoServico != 0)
                result = result.Where(obj => obj.TipoServicoMultisoftware == tipoServico);

            if (situacao == Dominio.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);


            return result.Count();
        }

        public List<Dominio.Entidades.Pessoas.ClienteURLAcesso> BuscarPorClientesPorTipoExecucao(Dominio.Enumeradores.TipoExecucao tipoExecucao, bool homologacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.ClienteURLAcesso>();
            var result = from obj in query
                         where obj.TipoExecucao == tipoExecucao
                         && obj.PossuiFila
                         && obj.Ativo && obj.Cliente.Ativo 
                         && obj.URLHomologacao == homologacao
                         select obj;
            return result
                    .Fetch(obj => obj.Cliente).ThenFetch(obj => obj.ClienteConfiguracao)
                    .Fetch(obj => obj.Cliente).ThenFetch(obj => obj.ClienteConfiguracaoHomologacao)
                    .ToList();
        }
    }
}
