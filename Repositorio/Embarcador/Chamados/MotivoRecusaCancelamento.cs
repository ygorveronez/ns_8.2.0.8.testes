using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class MotivoRecusaCancelamento : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento>
    {
        public MotivoRecusaCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoRecusaCancelamento tipoMotivoRecusaCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status, tipoMotivoRecusaCancelamento, tipoServicoMultisoftware);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoRecusaCancelamento tipoMotivoRecusaCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var result = _Consultar(descricao, status, tipoMotivoRecusaCancelamento, tipoServicoMultisoftware);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoRecusaCancelamento tipoMotivoRecusaCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                bool statusBool = status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                result = result.Where(o => o.Status == statusBool);
            }

            if (tipoMotivoRecusaCancelamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoRecusaCancelamento.Todos)
                result = result.Where(o => o.TipoMotivoRecusaCancelamento == tipoMotivoRecusaCancelamento);

            return result;
        }

        #endregion
    }
}
