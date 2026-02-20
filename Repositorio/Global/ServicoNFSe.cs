using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio
{
    public class ServicoNFSe : RepositorioBase<Dominio.Entidades.ServicoNFSe>, Dominio.Interfaces.Repositorios.ServicoNFSe
    {
        public ServicoNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ServicoNFSe BuscarPorNumero(int codigoEmpresa, string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFSe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ServicoNFSe BuscarPorNumero(string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFSe>();

            var result = from obj in query where obj.Numero == numero select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ServicoNFSe BuscarPorNumeroELocalidadeECodigoTributacao(string numero, int localidade, string codigoTributacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFSe>();

            var result = from obj in query where obj.Numero == numero && obj.Localidade.Codigo == localidade select obj;

            if (!string.IsNullOrWhiteSpace(codigoTributacao))
                result = result.Where(o => o.CodigoTributacao.Equals(codigoTributacao));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ServicoNFSe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFSe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ServicoNFSe BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFSe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ServicoNFSe> Consultar(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaServicoNFSe filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFSe>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(o => o.Status.Contains("A"));
                else
                    result = result.Where(o => o.Status.Contains("I"));
            }

            if (filtrosPesquisa.Localidade > 0)
                result = result.Where(obj => obj.Localidade.Codigo == filtrosPesquisa.Localidade);

            if (filtrosPesquisa.Empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.Empresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaServicoNFSe filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFSe>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(o => o.Status.Contains("A"));
                else
                    result = result.Where(o => o.Status.Contains("I"));
            }

            if (filtrosPesquisa.Localidade > 0)
                result = result.Where(obj => obj.Localidade.Codigo == filtrosPesquisa.Localidade);

            if (filtrosPesquisa.Empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.Empresa);

            return result.Count();
        }

        public List<Dominio.Entidades.ServicoNFSe> Consultar(int codigoEmpresa, int codigoEmpresaPai, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFSe>();

            var result = from obj in query where (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai) select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Contains(status));

            return result.OrderBy(o => o.Descricao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoEmpresaPai, string descricao, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFSe>();

            var result = from obj in query where (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai) select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Contains(status));

            return result.Count();
        }
    }
}
