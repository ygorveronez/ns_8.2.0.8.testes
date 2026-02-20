using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class PosicaoAtual : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>
    {
        #region Construtores públicos

        public PosicaoAtual(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos públicos

        public DateTime BuscarUltimaDataPorPlaca(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();

            var result = from obj in query
                         where obj.Veiculo.Placa.Contains(placa)
                         select obj.DataVeiculo;

            return result.FirstOrDefault();

        }

        public Dominio.Entidades.Embarcador.Logistica.PosicaoAtual BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result
                .Fetch(o => o.Veiculo)
                .FirstOrDefault();

        }

        public Dominio.Entidades.Embarcador.Logistica.PosicaoAtual BuscarPorVeiculo(int veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Veiculo.Codigo == veiculo);

            return result.Fetch(obj => obj.ClienteAlvo).Fetch(obj => obj.Veiculo).OrderByDescending(x => x.Data).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> BuscarPorVeiculos(List<int> veiculos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();
            var result = from obj in query select obj;
            result = result.Where(ent => veiculos.Contains(ent.Veiculo.Codigo));
            result = result.Fetch(obj => obj.Veiculo);
            return result.OrderBy(obj => obj.DataVeiculo).ToList();
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao BuscarPorVeiculoOV(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();

            var result = from obj in query
                         where obj.Veiculo.Codigo == codigoVeiculo
                         select new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                         {
                             CodigoVeiculo = obj.Veiculo.Codigo,
                             DataVeiculo = obj.DataVeiculo
                         };

            return result.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscarPorVeiculosOV(List<int> veiculos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();

            var result = from obj in query
                         where veiculos.Contains(obj.Veiculo.Codigo)
                         select new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                         {
                             CodigoVeiculo = obj.Veiculo.Codigo,
                             DataVeiculo = obj.DataVeiculo
                         };

            return result.ToList();
        }

        public List<(int CodigoVeiculo, DateTime DataPosicaoAtual, bool Rastreador)> BuscarDadosPosicaoPorVeiculos(List<int> codigosVeiculo)
        {
            DateTime dataBaseRastreador = DateTime.Now.AddMinutes(-30);

            var consultaPosicaoAtual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>()
                .Where(o => codigosVeiculo.Contains(o.Veiculo.Codigo));

            return consultaPosicaoAtual
                .Select(o => ValueTuple.Create(o.Veiculo.Codigo, o.DataVeiculo, o.DataVeiculo > dataBaseRastreador))
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PosicaoAtual BuscarPorIDEquipamento(string IDEquipamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.IDEquipamento == IDEquipamento);

            return result.FirstOrDefault();
        }

        public new List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();
            var result = from obj in query select obj;
            result = result.Fetch(obj => obj.Veiculo);
            return result.OrderBy(obj => obj.DataVeiculo).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoDaFrotaMapa> BuscarTodosPosicaoDaFrota()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();

            var result = from obj in query
                         where obj.Latitude != 0 && obj.Longitude != 0
                         select new Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoDaFrotaMapa
                         {

                             Descricao = obj.Descricao,
                             Latitude = obj.Latitude,
                             Longitude = obj.Longitude,
                             Placa = obj.Veiculo.Placa,
                             Status = obj.Status,
                             DataPosicao = obj.DataVeiculo,
                             IDEquipamento = obj.IDEquipamento
                         };

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoDaFrotaMapa> BuscarPosicaoDaFrotaporPlaca(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();

            var result = from obj in query
                         where obj.Veiculo.Placa.Contains(placa)
                         select new Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoDaFrotaMapa
                         {

                             Descricao = obj.Descricao,
                             Latitude = obj.Latitude,
                             Longitude = obj.Longitude,
                             Placa = obj.Veiculo.Placa,
                             DataPosicao = obj.DataVeiculo,
                             Status = obj.Status
                         };

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> BuscarPlaca(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();

            var result = from obj in query
                         where obj.Veiculo.Placa.Contains(placa)
                         select obj;

            return result.Fetch(obj => obj.Veiculo).ToList();

        }
        public List<(int CodigoVeiculo, DateTime DataPosicaoAtual, bool Rastreador, string UltimaPosicaoDescricao)> BuscarPosicaoPorVeiculos(List<int> codigosVeiculo)
        {
            DateTime dataBaseRastreador = DateTime.Now.AddMinutes(-30);

            var consultaPosicaoAtual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>()
                .Where(o => codigosVeiculo.Contains(o.Veiculo.Codigo));

            return consultaPosicaoAtual
                .Select(o => ValueTuple.Create(o.Veiculo.Codigo, o.DataVeiculo, o.DataVeiculo > dataBaseRastreador, o.Descricao))
                .ToList();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> BuscarProcessarEventos()
        {
            string sql = $@"
                select
					0 CodigoProcessarEvento,
					PosicaoAtual.POA_DATA_VEICULO DataProcessarEvento, 
					PosicaoAtual.POS_CODIGO CodigoPosicao,
					PosicaoAtual.VEI_CODIGO CodigoVeiculo,
					PosicaoAtual.POA_DATA_VEICULO DataVeiculoPosicao,
					PosicaoAtual.POA_LATITUDE LatitudePosicao,
					PosicaoAtual.POA_LONGITUDE LongitudePosicao,
                    PosicaoAtual.POA_IGNICAO IgnicaoPosicao,
					PosicaoAtual.POA_VELOCIDADE VelocidadePosicao,
                    PosicaoAtual.POA_TEMPERATURA TemperaturaPosicao,
                    PosicaoAtual.POA_SENSOR_TEMPERATURA SensorTemperaturaPosicao,
					PosicaoAtual.POA_EM_ALVO EmAlvoPosicao,
					case
						when PosicaoAtual.POA_EM_ALVO = 1 then 
							SUBSTRING((
								SELECT',' + convert(varchar, convert(bigint, PosicaoAlvo.CLI_CGCCPF)) AS [text()]                                                        
								FROM T_POSICAO_ALVO PosicaoAlvo
								WHERE PosicaoAlvo.POS_CODIGO = PosicaoAtual.POS_CODIGO     
								FOR XML PATH ('')
							), 2, 2000) 
						else null
					end CodigosClientesAlvoPosicao,
					null CodigoMonitoramento,
					null DataCriacaoMonitoramento,
					null DataInicioMonitoramento,
					null DataFimMonitoramento,
					null CodigoCarga,
					null DataInicioViagem,
					null DataCarregamentoCarga
				from
					t_posicao_atual PosicaoAtual
                inner join t_monitoramento_veiculo vei on vei.vei_codigo = PosicaoAtual.vei_codigo
                inner join t_monitoramento mon on mon.mon_codigo = vei.mon_codigo
                where mon.mon_status = 1
				order by
					PosicaoAtual.POA_CODIGO";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento)));
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> monitoramentoProcessarEventos = query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento>();
            return monitoramentoProcessarEventos;
        }

        public Dominio.Entidades.Embarcador.Logistica.PosicaoAtual BuscarPorPosicao(long codigoPosicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>()
                .Where(o => o.Posicao.Codigo == codigoPosicao);

            return query.FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>> BuscarPorVeiculosAsync(List<int> veiculos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> query =
                this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();

            var result = from obj in query select obj;

            result = result.Where(ent => veiculos.Contains(ent.Veiculo.Codigo));
            result = result.Fetch(obj => obj.Veiculo);

            return result.OrderBy(obj => obj.DataVeiculo).ToListAsync();
        }

        #endregion

    }

}
