using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Veiculos
{
    public class HistoricoHorimetro : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro>
    {
        public HistoricoHorimetro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro> BuscarPorEquipamento(int codigoEquipamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro>();
            var result = from obj in query where obj.Equipamento.Codigo == codigoEquipamento select obj;
            return result.ToList();
        }


        #endregion

        #region Métodos Privados



        #endregion
    }
}
