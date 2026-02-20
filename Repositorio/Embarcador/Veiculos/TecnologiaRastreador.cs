using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class TecnologiaRastreador : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador>
    {
        public TecnologiaRastreador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador> ConsultarAtivos(string descricao)
        {
            return Consultar(descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, "Descricao", "asc", 0, 1000);
        }

        public Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador BuscarAtivaPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador>();
            var result = from obj in query where obj.TipoIntegracao == tipoIntegracao && obj.Ativo == true select obj;
            return result.FirstOrDefault();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador BuscarAtivoPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Ativo == true select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador ConsultarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador>();
            query = query.Where(obj => obj.Descricao.Contains(descricao));
            return query.FirstOrDefault();
        }



        private NHibernate.ISQLQuery ConsultaUltimaPosicaoRastreadores(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoTecnologia filtrosPesquisa)
        {

            string sql = @"
                with ultimaPosicaoTecnologia AS (select ";

            sql += @" 
                    '' Tecnologia, 
                    POA_DATA DataUltimaPosicaoProcessada, 
                    POA_CODIGO CodigoPosicao,
                    POA_DESCRICAO Descricao, 
                    T_POSICAO.POS_RASTREADOR Rastreador, 
                    T_POSICAO.POS_GERENCIADORA Gerenciadora,
                    ROW_NUMBER() OVER (PARTITION BY  T_POSICAO.POS_GERENCIADORA, POA_DESCRICAO ORDER BY POA_DATA DESC) AS RN,
                    T_VEICULO.VEI_CODIGO Veiculo";


            sql += $@"
                from T_POSICAO_ATUAL 
                inner join T_VEICULO on T_VEICULO.VEI_CODIGO = T_POSICAO_ATUAL.VEI_CODIGO
                inner join T_POSICAO on T_POSICAO.POS_CODIGO = T_POSICAO_ATUAL.POS_CODIGO";

            sql += $@"
                WHERE 1=1 AND POS_PROCESSAR=2";


            if (filtrosPesquisa.Tecnologia > 0)
                sql += $@"
                AND T_RASTREADOR_TECNOLOGIA.TRA_CODIGO = {filtrosPesquisa.Tecnologia} ";

            ////if (filtrosPesquisa.SomenteRastreadoresAtivos)
            ////    sql += $@"
            ////    AND T_VEICULO.VEI_ATIVO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo} ";
            sql += ")";

            sql += @", ultimaPosicaoRecebida AS (
                        SELECT T_VEICULO.VEI_CODIGO Veiculo,
		                       MAX(POS_DATA) AS DataUltimaPosicaoRecebida
                        FROM T_POSICAO
	                    INNER JOIN T_VEICULO on T_VEICULO.VEI_CODIGO = T_POSICAO.VEI_CODIGO
                        WHERE POS_PROCESSAR IN(0, 1)
                        GROUP BY POS_RASTREADOR, T_VEICULO.VEI_CODIGO
                    )"; 


            sql += @"  SELECT Tecnologia,
		                      DataUltimaPosicaoProcessada,
                              DataUltimaPosicaoRecebida,
		                      CodigoPosicao,
		                      Descricao,
		                      Rastreador,
		                      Gerenciadora
	                     FROM ultimaPosicaoTecnologia
                         LEFT JOIN ultimaPosicaoRecebida ON ultimaPosicaoRecebida.Veiculo=ultimaPosicaoTecnologia.Veiculo
                        WHERE RN=1";
            sql += @"
                order by DataUltimaPosicaoProcessada desc";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            return consulta;

        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoRastreador> consultaRastreadoresTecnologia(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoTecnologia filtrosPesquisa)
        {
            var consulta = ConsultaUltimaPosicaoRastreadores(filtrosPesquisa);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoRastreador)));
            var rastreadores = consulta.List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoRastreador>();
            return rastreadores;
        }

    }
}
