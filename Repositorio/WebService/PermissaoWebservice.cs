using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.WebService
{

    public class PermissaoWebservice : RepositorioBase<Dominio.Entidades.WebService.PermissaoWebservice>
    {
        #region Construtores

        public PermissaoWebservice(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public PermissaoWebservice(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.WebService.PermissaoWebservice BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.PermissaoWebservice>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.WebService.PermissaoWebservice> BuscarPorIntegradoraNomeMetodoAsync(int codigoIntegradora, string nomeMetodo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.PermissaoWebservice>();
            var result = from obj in query where obj.Integradora.Codigo == codigoIntegradora && obj.NomeMetodo == nomeMetodo select obj;
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.WebService.PermissaoWebservice> BuscarPorIntegradora(int codigoIntegradora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.PermissaoWebservice>();
            var result = from obj in query where obj.Integradora.Codigo == codigoIntegradora select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.WebService.PermissaoWebservice>> BuscarPorIntegradoraAsync(int codigoIntegradora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.PermissaoWebservice>();
            var result = from obj in query where obj.Integradora.Codigo == codigoIntegradora select obj;

            return result.ToListAsync(CancellationToken);
        }

        public void CadastrarPadrao(UnitOfWork unitOfWork, Dominio.Entidades.WebService.Integradora integradora, string nomeMetodo)
        {
            this.Inserir(new Dominio.Entidades.WebService.PermissaoWebservice()
            {
                Integradora = integradora,
                NomeMetodo = nomeMetodo,
                QtdRequisicoes = 1,
                RequisicoesMinuto = 5,
                UltimoReset = DateTime.Now
            });
        }

        public List<Dominio.Entidades.WebService.PermissaoWebservice> Consultar(Dominio.ObjetosDeValor.WebService.PermissaoWebService.FiltroPesquisaPermissaoWebService filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPermissoes = Consultar(filtrosPesquisa);

            return ObterLista(consultaPermissoes, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.WebService.PermissaoWebService.FiltroPesquisaPermissaoWebService filtrosPesquisa)
        {
            var consultaPermissoes = Consultar(filtrosPesquisa);

            return consultaPermissoes.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.WebService.PermissaoWebservice> Consultar(Dominio.ObjetosDeValor.WebService.PermissaoWebService.FiltroPesquisaPermissaoWebService filtro)
        {
            var consultaPermissao = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.PermissaoWebservice>();

            if (!string.IsNullOrWhiteSpace("NomeMetodo"))
                consultaPermissao = consultaPermissao.Where(o => o.NomeMetodo.Contains(filtro.NomeMetodo));

            if (filtro.DistinguirPorNomeMetodo)
                consultaPermissao = consultaPermissao.DistinctBy(x => x.NomeMetodo).AsQueryable();

            return consultaPermissao;
        }

        #endregion Métodos Privados
    }
}
