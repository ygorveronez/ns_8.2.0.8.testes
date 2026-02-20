using System;
using System.Linq;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Email
{
    public class EmailGlobalizadoFornecedor : RepositorioBase<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor>
    {
        public EmailGlobalizadoFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor> Consultar(Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailGlobalizadoFornecedor filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailGlobalizadoFornecedor filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor> BuscarEmailsPendentesEnvio()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor>();

            var result = from obj in query 
                            where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioEmail.AguardandoEnvio 
                            && obj.DataEnvio <= DateTime.Now 
                         select obj;

            return result.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor> Consultar(Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailGlobalizadoFornecedor filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor>();
            consulta = from obj in consulta select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                consulta = consulta.Where(o => o.DataEnvio.Date >= filtrosPesquisa.DataInicial);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                consulta = consulta.Where(o => o.DataEnvio.Date <= filtrosPesquisa.DataFinal);

            if (filtrosPesquisa.Situacao.HasValue)
                consulta = consulta.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return consulta;
        }

        #endregion
    }
}
