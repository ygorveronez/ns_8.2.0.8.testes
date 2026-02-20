using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class HistoricoVeiculoVinculoEquipamento : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento>
    {
        public HistoricoVeiculoVinculoEquipamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento> BuscarPorVinculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento>();
            var result = from obj in query where obj.HistoricoVeiculoVinculo.Codigo == codigo select obj;
            return result.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculoEquipamentos> ConsultarEquipamentosVinculo(int codigoVinculo)
        {

            string query = @"SELECT 
                                Equipamento.EQP_DESCRICAO       Descricao,
                                Equipamento.EQP_NUMERO          Numero,
                                ModeloEquipamento.EMO_DESCRICAO Modelo,
                                MarcaEquipamento.EQM_DESCRICAO  Marca

                                FROM T_HISTORICO_VEICULO_VINCULO_EQUIPAMENTO VeiculoEquipamento
                                LEFT OUTER JOIN T_VEICULO_EQUIPAMENTO   VecEquipamento    on VecEquipamento.EQP_CODIGO = VeiculoEquipamento.EQP_CODIGO
                                LEFT OUTER JOIN T_EQUIPAMENTO           Equipamento       on Equipamento.EQP_CODIGO = VecEquipamento.EQP_CODIGO
                                LEFT OUTER JOIN T_EQUIPAMENTO_MARCA     MarcaEquipamento  on MarcaEquipamento.EQM_CODIGO = Equipamento.EQM_CODIGO
                                LEFT OUTER JOIN T_EQUIPAMENTO_MODELO    ModeloEquipamento on ModeloEquipamento.EMO_CODIGO = Equipamento.EMO_CODIGO";

            if (codigoVinculo > 0)
                query += " where HVV_CODIGO = " + codigoVinculo.ToString();
            else
                query += " where 1 = 0";

            var consultaEquipamentos = this.SessionNHiBernate.CreateSQLQuery(query);

            consultaEquipamentos.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculoEquipamentos)));

            return consultaEquipamentos.List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculoEquipamentos>();
        }

    }
}
