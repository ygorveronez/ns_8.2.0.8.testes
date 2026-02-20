using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class HistoricoVeiculoVinculoReboque : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque>
    {
        public HistoricoVeiculoVinculoReboque(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque> BuscarPorVinculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque>();
            var result = from obj in query where obj.HistoricoVeiculoVinculo.Codigo == codigo select obj;
            return result.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculoReboques> ConsultarReboquesVinculo(int codigoVinculo) {

            string query = @"SELECT 
                                Veiculo.VEI_PLACA                   Veiculo,
                                Veiculo.VEI_NUMERO_FROTA            NumeroFrota,
                                MarcaVeiculo.VMA_DESCRICAO          Marca,
                                Veiculo.VEI_ANO                     Ano,
                                ModeloVeicularCarga.MVC_DESCRICAO   ModeloVeicularCarga

                                FROM T_HISTORICO_VEICULO_VINCULO_REBOQUE VeiculoReboque
                                LEFT OUTER JOIN T_VEICULO Veiculo                           on Veiculo.VEI_CODIGO = VeiculoReboque.VEI_CODIGO
                                LEFT OUTER JOIN T_VEICULO_MARCA MarcaVeiculo                on MarcaVeiculo.VMA_CODIGO = Veiculo.VMA_CODIGO
                                LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Veiculo.MVC_CODIGO";

            if (codigoVinculo > 0)
                query += " where HVV_CODIGO = " + codigoVinculo.ToString();
            else
                query += " where 1 = 0";

            var consultaReboques = this.SessionNHiBernate.CreateSQLQuery(query);

            consultaReboques.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculoReboques)));

            return consultaReboques.List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculoReboques>();
        }

    }
}
