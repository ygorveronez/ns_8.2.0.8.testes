using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo>
    {
        public VeiculoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo> BuscarPorCodigoVeiculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo>();
            var result = from obj in query where obj.Veiculo.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo BuscarCRLVPorCodigoVeiculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo>();
            var result = from obj in query where obj.Veiculo.Codigo == codigo && obj.Descricao == "CRLV" select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo> BuscarCRLVPorCodigosVeiculo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo>();
            var result = from obj in query where obj.Descricao == "CRLV" && codigos.Contains(obj.Veiculo.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo> BuscarPorCodigoVeiculoETipo(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoVeiculo tipoAnexo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo>();
            var result = from obj in query where (obj.Veiculo.Codigo == codigo) && (obj.TipoAnexoVeiculo == tipoAnexo) select obj;
            return result.ToList();
        }

    }
}
