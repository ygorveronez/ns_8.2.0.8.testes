using System.Linq;

namespace Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo
{
    public class CadastroVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo>
    {
        public CadastroVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo BuscarUltimoCadastroVeiculo(int codigoVeiculo)
        {
            var consultaAlcada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo>();

            consultaAlcada = consultaAlcada.Where(o => o.Veiculo.Codigo == codigoVeiculo);
            consultaAlcada = consultaAlcada.Where(o => !o.Finalizado);

            return consultaAlcada
                .OrderByDescending(o => o.DataCadastro)
                .FirstOrDefault();
        }
    }
}
