using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class TrechoBalsa : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa>
    {
        public TrechoBalsa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.TrechoBalsa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public bool ValidarDuplicidadeCliente(Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa>();
            var consulta = from obj in query
                       where
                            (obj.PortoOrigem.CPF_CNPJ == trechoBalsa.PortoOrigem.CPF_CNPJ &&
                            obj.PortoDestino.CPF_CNPJ == trechoBalsa.PortoDestino.CPF_CNPJ) &&
                            obj.Codigo != trechoBalsa.Codigo
                       select obj;

            return consulta.Count() == 0;
        }

         
        public List<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa> BuscarTrechosBalsa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa>();
            consulta = from obj in consulta select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                consulta = consulta.Where(o => o.PortoOrigem.Codigo.Equals(filtrosPesquisa.CodigoPortoOrigem));

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                consulta = consulta.Where(o => o.PortoDestino.Codigo.Equals(filtrosPesquisa.CodigoPortoDestino));

            if (filtrosPesquisa.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                consulta = consulta.Where(o => o.Ativo == (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false));

            return consulta.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa>();
            consulta = from obj in consulta select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if(filtrosPesquisa.CodigoPortoOrigem > 0)
                consulta = consulta.Where(o => o.PortoOrigem.Codigo.Equals(filtrosPesquisa.CodigoPortoOrigem));

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                consulta = consulta.Where(o => o.PortoDestino.Codigo.Equals(filtrosPesquisa.CodigoPortoDestino));

            if(filtrosPesquisa.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                consulta = consulta.Where(o => o.Ativo == (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false));

            return consulta;
        }

    }


}
