using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo>
    {
        public ChamadoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo> BuscarPorChamado(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo>();
            var result = from obj in query where obj.Chamado.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo> Consultar(int codigoChamado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo>();
            var result = from obj in query where obj.Chamado.Codigo == codigoChamado select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo>();
            var result = from obj in query where obj.Chamado.Codigo == codigoChamado select obj;

            return result.Count();
        }

        public bool PossuiAnexo(int codigoChamado)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo>();
            var result = query.Where(obj => obj.Chamado.Codigo == codigoChamado);
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo>();

            var result = from obj in query where obj.Chamado.CargaEntrega.Carga.Codigo == codigoCarga select obj;

            return result.ToList();
        }

        #endregion
    }
}
