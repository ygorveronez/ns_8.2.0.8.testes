using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PrazoSituacaoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga>
    {
        #region Construtores

        public PrazoSituacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga BuscarPorCodigo(int codigo)
        {
            var consultaPrazoSituacaoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga>()
                .Where(o => o.Codigo == codigo);

            return consultaPrazoSituacaoCarga.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga BuscarPorSituacao(SituacaoCargaJanelaCarregamento situacaoCarga)
        {
            var consultaPrazoSituacaoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga>()
                .Where(o => o.SituacaoCarga == situacaoCarga);

            return consultaPrazoSituacaoCarga.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga> Consultar(SituacaoCargaJanelaCarregamento? situacaoCarga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPrazoSituacaoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga>();

            if (situacaoCarga != null)
                consultaPrazoSituacaoCarga = consultaPrazoSituacaoCarga.Where(o => o.SituacaoCarga == situacaoCarga);

            return ObterLista(consultaPrazoSituacaoCarga, parametrosConsulta);
        }

        public int ContarConsulta(SituacaoCargaJanelaCarregamento? situacaoCarga)
        {
            var consultaPrazoSituacaoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga>();

            if (situacaoCarga != null)
                consultaPrazoSituacaoCarga = consultaPrazoSituacaoCarga.Where(o => o.SituacaoCarga == situacaoCarga);

            return consultaPrazoSituacaoCarga.Count();
        }

        #endregion
    }
}
