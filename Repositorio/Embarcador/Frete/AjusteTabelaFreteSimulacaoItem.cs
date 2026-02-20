using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    public class AjusteTabelaFreteSimulacaoItem : RepositorioBase<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem>
    {
        public AjusteTabelaFreteSimulacaoItem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem> BuscarPorSimulacao(int codigoSimulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem>();

            query = query.Where(o => o.Simulacao.Codigo == codigoSimulacao);

            return query.ToList();
        }

        public List<int> BuscarCodigosPorSimulacao(int codigoSimulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem>();

            query = query.Where(o => o.Simulacao.Codigo == codigoSimulacao);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Fretes.SimulacaoFrete> ConsultarRelatorioSimulacaoFrete(int codigoSimulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem>();

            query = query.Where(o => o.Simulacao.Codigo == codigoSimulacao);

            return query.OrderBy(o => o.TabelaCarga.Carga.DataCriacaoCarga).Select(o => new Dominio.Relatorios.Embarcador.DataSource.Fretes.SimulacaoFrete()
            {
                CodigoCarga = o.TabelaCarga.Carga.Codigo,
                CodigoItemSimulacao = o.Codigo,
                DataCriacaoCarga = o.TabelaCarga.Carga.DataCriacaoCarga,
                Destinatario = o.TabelaCarga.Carga.DadosSumarizados.Destinatarios,
                Destino = o.TabelaCarga.Carga.DadosSumarizados.Destinos,
                NumeroCarga = o.TabelaCarga.Carga.CodigoCargaEmbarcador,
                Origem = o.TabelaCarga.Carga.DadosSumarizados.Origens,
                Remetente = o.TabelaCarga.Carga.DadosSumarizados.Remetentes,
                Transportador = o.TabelaCarga.Carga.Empresa.RazaoSocial,
                ValorFreteOriginal = o.TabelaCarga.Carga.ValorFrete,
                ValorFreteTotalOriginal = o.TabelaCarga.Carga.ValorFrete + o.TabelaCarga.Carga.ValorICMS,
                ValorICMSOriginal = o.TabelaCarga.Carga.ValorICMS,
                ValorFreteAjuste = o.ValorFrete,
                ValorFreteTotalAjuste = o.ValorFreteTotal,
                ValorICMSAjuste = o.ValorICMS
            }).ToList();
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Fretes.SimulacaoFreteComponente> ConsultarComponenteRelatorioSimulacaoFreteItem(int codigoSimulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente>();

            query = query.Where(o => o.ItemSimulacao.Simulacao.Codigo == codigoSimulacao);

            return query.OrderBy(o => o.ComponenteFrete.Descricao).Select(o => new Dominio.Relatorios.Embarcador.DataSource.Fretes.SimulacaoFreteComponente()
            {
                CodigoItemSimulacao = o.ItemSimulacao.Codigo,
                Descricao = o.ComponenteFrete.Descricao,
                Valor = o.ValorComponente
            }).ToList();
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Fretes.SimulacaoFreteComponente> ConsultarComponenteRelatorioSimulacaoFreteCarga(int codigoSimulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            var subqueryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem>().Where(o => o.Simulacao.Codigo == codigoSimulacao).Select(o => o.TabelaCarga.Carga.Codigo);
                        
            query = query.Where(o => subqueryCargas.Contains(o.Carga.Codigo));
            
            return query.OrderBy(o => o.ComponenteFrete.Descricao).Select(o => new Dominio.Relatorios.Embarcador.DataSource.Fretes.SimulacaoFreteComponente()
            {
                CodigoCarga = o.Carga.Codigo,
                Descricao = o.ComponenteFrete.Descricao,
                Valor = o.ValorComponente
            }).ToList();
        }
    }
}
