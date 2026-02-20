using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using NHibernate.Linq;
using System.Threading;

namespace Repositorio
{
    public class LayoutEDI : RepositorioBase<Dominio.Entidades.LayoutEDI>, Dominio.Interfaces.Repositorios.LayoutEDI
    {
        public LayoutEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public LayoutEDI(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.LayoutEDI BuscarPorCodigo(int codigoLayoutEDI)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();
            var result = from obj in query where obj.Codigo == codigoLayoutEDI select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.LayoutEDI BuscarPorEmailRemetente(string remetente, Dominio.Enumeradores.TipoLayoutEDI tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();
            var result = from obj in query where obj.EmailLeitura.Contains(remetente) && obj.Tipo == tipo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.LayoutEDI BuscarPorCodigoETipo(int codigoLayoutEDI, Dominio.Enumeradores.TipoLayoutEDI tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();
            var result = from obj in query where obj.Codigo == codigoLayoutEDI && obj.Tipo == tipo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.LayoutEDI> BuscarPorTipo(Dominio.Enumeradores.TipoLayoutEDI tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query where obj.Tipo == tipo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.LayoutEDI BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI tipo, bool? ativo)
        {
            IQueryable<Dominio.Entidades.LayoutEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            query = query.Where(obj => obj.Tipo == tipo);

            if (ativo.HasValue)
            {
                if (ativo.Value)
                    query = query.Where(o => o.Status == "A");
                else
                    query = query.Where(o => o.Status == "I");
            }

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.LayoutEDI BuscarPorDescricaoETipo(string descricao, Dominio.Enumeradores.TipoLayoutEDI tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query where obj.Descricao.Contains(descricao) && obj.Tipo == tipo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.LayoutEDI BuscarDescricao(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query where obj.Descricao == nome select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.LayoutEDI> BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI tipo, string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query where obj.Tipo == tipo && obj.Descricao == nome select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.LayoutEDI> BuscarParaRelatorios()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query where obj.VincularNomeArquivo == true select obj;

            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.LayoutEDI>> BuscarParaRelatoriosAsync()
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>().Where(x => x.VincularNomeArquivo == true).ToListAsync();
        }

        public Dominio.Entidades.LayoutEDI BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query where obj.Tipo == tipo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.LayoutEDI> ConsultaPorEmpresaETipo(int codigoEmpresa, Dominio.Enumeradores.TipoLayoutEDI tipo, string descricao, int inicioRegistros, int maximoRegistros)
        {
            var queryEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            var resultEmpresa = (from obj in queryEmpresa where obj.Codigo == codigoEmpresa select obj).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query where obj.Tipo == tipo && resultEmpresa.LayoutsEDI.Contains(obj) select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorEmpresaETipo(int codigoEmpresa, Dominio.Enumeradores.TipoLayoutEDI tipo, string descricao)
        {
            var queryEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            var resultEmpresa = (from obj in queryEmpresa where obj.Codigo == codigoEmpresa select obj).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query where obj.Tipo == tipo && resultEmpresa.LayoutsEDI.Contains(obj) select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.Count();
        }

        public Dominio.Entidades.LayoutEDI Buscar(Dominio.Enumeradores.TipoLayoutEDI tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query where obj.Tipo == tipo && obj.Status == "A" select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.LayoutEDI> Buscar(string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.ToList();
        }

        public Dominio.Entidades.LayoutEDI Buscar(int codigoLayout)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query where obj.Codigo == codigoLayout select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.LayoutEDI> Consultar(string descricao, Dominio.Enumeradores.TipoLayoutEDI tipoLayoutEDI, List<Dominio.Enumeradores.TipoLayoutEDI> tiposLayoutsEDI, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, int codigoGrupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (tipoLayoutEDI != Dominio.Enumeradores.TipoLayoutEDI.Todos)
                result = result.Where(obj => obj.Tipo == tipoLayoutEDI);

            if (tiposLayoutsEDI != null && tiposLayoutsEDI.Count > 0)
                result = result.Where(o => tiposLayoutsEDI.Contains(o.Tipo));

            if (codigoGrupoPessoa > 0)
            {
                var queryGrupoPessoaLayout = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();
                var resultGrupoPessoaLayout = from obj in queryGrupoPessoaLayout where obj.GrupoPessoas.Codigo == codigoGrupoPessoa select obj;
                result = result.Where(obj => resultGrupoPessoaLayout.Select(a => a.LayoutEDI).Contains(obj));
            }

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.Enumeradores.TipoLayoutEDI tipoLayoutEDI, List<Dominio.Enumeradores.TipoLayoutEDI> tiposLayoutsEDI, int codigoGrupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LayoutEDI>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (tipoLayoutEDI != Dominio.Enumeradores.TipoLayoutEDI.Todos)
                result = result.Where(obj => obj.Tipo == tipoLayoutEDI);

            if (tiposLayoutsEDI != null && tiposLayoutsEDI.Count > 0)
                result = result.Where(o => tiposLayoutsEDI.Contains(o.Tipo));

            if (codigoGrupoPessoa > 0)
            {
                var queryGrupoPessoaLayout = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();
                var resultGrupoPessoaLayout = from obj in queryGrupoPessoaLayout where obj.GrupoPessoas.Codigo == codigoGrupoPessoa select obj;
                result = result.Where(obj => resultGrupoPessoaLayout.Select(a => a.LayoutEDI).Contains(obj));
            }


            return result.Count();
        }
    }
}
