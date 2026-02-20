using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoAnalisisArvore : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore>
    {
        public ChamadoAnalisisArvore(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }


        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore> BuscarPorMotivoChamado(int codigoMotivo, int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore>();
            query = query.Where(a => a.MotivoChamado.Codigo == codigoMotivo && codigoChamado == a.Chamado.Codigo);
            return query.ToList();

        }

        public Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore BuscarChamadoSelecionado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore>();
            query = query.Where(a => a.Chamado.Codigo == codigoChamado);

            return query.FirstOrDefault();

        }
        #endregion
    }
}
