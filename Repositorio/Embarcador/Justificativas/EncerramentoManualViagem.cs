using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Justificativas
{
    public class EncerramentoManualViagem : RepositorioBase<Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem>
    {
        public EncerramentoManualViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
       
        public List<Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem> Consultar(Dominio.ObjetosDeValor.Embarcador.Justificativas.FiltroPesquisaEncerramentoManualViagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Justificativas.FiltroPesquisaEncerramentoManualViagem filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.FirstOrDefault();
        }


        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem> Consultar(Dominio.ObjetosDeValor.Embarcador.Justificativas.FiltroPesquisaEncerramentoManualViagem filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem>();
            query = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao)
                query = query.Where(obj => obj.Situacao == filtrosPesquisa.Situacao);

            return query;
        }

        #endregion
    }
}
