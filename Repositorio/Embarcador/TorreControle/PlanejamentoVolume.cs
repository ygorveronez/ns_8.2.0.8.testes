using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.TorreControle
{
    public class PlanejamentoVolume : RepositorioBase<Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume>
    {
        public PlanejamentoVolume(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PlanejamentoVolume(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume BuscarPorCodigo(int codigo)
        {
            var consultaPlanejamentoVolume = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume>();

            consultaPlanejamentoVolume = consultaPlanejamentoVolume.Where(planejamento => planejamento.Codigo == codigo);

            return consultaPlanejamentoVolume.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeRemetente> BuscarRemetentesPorCodigosPlanejamento(List<int> codigosPlanejamento)
        {
            string sql = @"SELECT 
                            Cliente.CLI_NOME AS NomeRemetente,
							Cliente.CLI_CGCCPF AS CPFCNPJRemetente,
                            PlanejamentoRemetente.PLV_CODIGO AS CodigoPlanejamento
                            FROM T_PLANEJAMENTO_VOLUME_REMETENTE PlanejamentoRemetente 
                            join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = PlanejamentoRemetente.CLI_CGCCPF
                            WHERE PLV_CODIGO IN (:codigoPlanejamento)";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameterList("codigoPlanejamento", codigosPlanejamento);

            return consulta
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeRemetente)))
                .List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeRemetente>()
                .ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeRemetente> BuscarPlanejamentosPorCodigosRemetentes(List<double> codigosRemetentes)
        {
            string sql = @"SELECT 
                            PlanejamentoRemetente.PLV_CODIGO AS CodigoPlanejamento
                            FROM T_PLANEJAMENTO_VOLUME_REMETENTE PlanejamentoRemetente 
                            join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = PlanejamentoRemetente.CLI_CGCCPF
                            WHERE Cliente.CLI_CGCCPF IN (:codigosRemetentes)";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameterList("codigosRemetentes", codigosRemetentes);

            return consulta
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeRemetente)))
                .List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeRemetente>()
                .ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestinatario> BuscarPlanejamentosPorCodigosDestinatarios(List<double> codigosDestinatarios)
        {
            string sql = @"SELECT 
                            PlanejamentoDestinatario.PLV_CODIGO AS CodigoPlanejamento
                            FROM T_PLANEJAMENTO_VOLUME_DESTINATARIO PlanejamentoDestinatario 
                            join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = PlanejamentoDestinatario.CLI_CGCCPF
                            WHERE Cliente.CLI_CGCCPF IN (:codigosDestinatarios)";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameterList("codigosDestinatarios", codigosDestinatarios);

            return consulta
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestinatario)))
                .List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestinatario>()
                .ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestinatario> BuscarDestinatariosPorCodigosPlanejamento(List<int> codigosPlanejamento)
        {
            string sql = @"SELECT 
                            Cliente.CLI_NOME AS NomeDestinatario,
							Cliente.CLI_CGCCPF AS CPFCNPJDestinatario,
                            PlanejamentoDestinatario.PLV_CODIGO AS CodigoPlanejamento
                            FROM T_PLANEJAMENTO_VOLUME_DESTINATARIO PlanejamentoDestinatario
                            join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = PlanejamentoDestinatario.CLI_CGCCPF
                            WHERE PLV_CODIGO IN (:codigoPlanejamento)";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameterList("codigoPlanejamento", codigosPlanejamento);

            return consulta
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestinatario)))
                .List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestinatario>()
                .ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeOrigem> BuscarOrigensPorCodigosPlanejamento(List<int> codigosPlanejamento)
        {
            string sql = @"SELECT 
                            Localidade.LOC_CODIGO AS CodigoLocalidade,
							Localidade.LOC_DESCRICAO AS DescricaoLocalidade,
							PlanejamentoOrigem.PLV_CODIGO AS CodigoPlanejamento
                            FROM T_PLANEJAMENTO_VOLUME_ORIGENS PlanejamentoOrigem
                            join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = PlanejamentoOrigem.LOC_CODIGO
                            WHERE PLV_CODIGO IN (:codigoPlanejamento)";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameterList("codigoPlanejamento", codigosPlanejamento);

            return consulta
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeOrigem)))
                .List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeOrigem>()
                .ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestino> BuscarDestinosPorCodigosPlanejamento(List<int> codigosPlanejamento)
        {
            string sql = @"SELECT 
                            Localidade.LOC_CODIGO AS CodigoLocalidade,
							Localidade.LOC_DESCRICAO AS DescricaoLocalidade,
							PlanejamentoDestino.PLV_CODIGO AS CodigoPlanejamento
                            FROM T_PLANEJAMENTO_VOLUME_DESTINOS PlanejamentoDestino
                            join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = PlanejamentoDestino.LOC_CODIGO
                            WHERE PLV_CODIGO IN (:codigoPlanejamento)";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameterList("codigoPlanejamento", codigosPlanejamento);

            return consulta
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestino)))
                .List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestino>()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume> Consultar(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaPlanejamentoVolume filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPlanejamentoVolume = Consultar(filtrosPesquisa);

            return ObterLista(consultaPlanejamentoVolume, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaPlanejamentoVolume filtrosPesquisa)
        {
            var consultaPlanejamentoVolume = Consultar(filtrosPesquisa);

            return consultaPlanejamentoVolume.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume> Consultar(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaPlanejamentoVolume filtrosPesquisa)
        {
            var consultaPlanejamentoVolume = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume>();
            var result = from obj in consultaPlanejamentoVolume select obj;

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                result = result.Where(x => x.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoTipoCarga > 0)
                result = result.Where(x => x.TipoDeCarga.Codigo == filtrosPesquisa.CodigoTipoCarga);

            if (filtrosPesquisa.DataProgramacaoCargaInicial != DateTime.MinValue)
                result = result.Where(x => x.DataProgramacaoCargaInicial == filtrosPesquisa.DataProgramacaoCargaInicial);

            if (filtrosPesquisa.DataProgramacaoCargaFinal != DateTime.MinValue)
                result = result.Where(x => x.DataProgramacaoCargaFinal == filtrosPesquisa.DataProgramacaoCargaFinal);

            if (filtrosPesquisa.Remetentes?.Count > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeRemetente> planejamentos = this.BuscarPlanejamentosPorCodigosRemetentes(filtrosPesquisa.Remetentes);
                List<int> codigosPlanejamentos = planejamentos.Select(x => x.CodigoPlanejamento).ToList();

                result = result.Where(x => codigosPlanejamentos.Contains(x.Codigo));
            }

            if (filtrosPesquisa.Destinatarios?.Count > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestinatario> planejamentos = this.BuscarPlanejamentosPorCodigosDestinatarios(filtrosPesquisa.Destinatarios);
                List<int> codigosPlanejamentos = planejamentos.Select(x => x.CodigoPlanejamento).ToList();

                result = result.Where(x => codigosPlanejamentos.Contains(x.Codigo));
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                result = result.Where(x => x.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            }

            return result;
        }
    }
}
