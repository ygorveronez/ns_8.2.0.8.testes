using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class PagamentoMotoristaIntegracaoEnvio : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>
    {
        public PagamentoMotoristaIntegracaoEnvio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> BuscarIntegracoesPendenteDeEnvio(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista[] tipos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();
            var result = from obj in query
                         where tipos.Contains(obj.TipoIntegracaoPagamentoMotorista)
                               && obj.PagamentoMotoristaTMS.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgIntegracao
                               && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                               && obj.PagamentoMotoristaTMS.PagamentoLiberado
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> BuscarIntegracoesPendenteDeEnvioAdicionais(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista[] tipos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();
            var result = from obj in query
                         where tipos.Contains(obj.TipoIntegracaoPagamentoMotorista)
                               && (obj.PagamentoMotoristaTMS.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgIntegracao || 
                                    obj.PagamentoMotoristaTMS.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FalhaIntegracao)
                               && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                               && obj.PagamentoMotoristaTMS.PagamentoLiberado
                         select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio BuscarPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();
            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigo && obj.TipoIntegracaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista.KMM select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio BuscarPorPagamentoETipoIntegracao(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();
            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigo && obj.TipoIntegracaoPagamentoMotorista == tipoIntegracaoPagamentoMotorista select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> BuscarPorPagamentoETipoIntegracao(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista[] tipos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();
            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigo && tipos.Contains(obj.TipoIntegracaoPagamentoMotorista) && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado select obj;

            return result.ToList();
        }

        public int ContarPorPagamento(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();

            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigoPagamento && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public int ContarPorPagamentoDiffSituacao(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();

            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigoPagamento && 
                         obj.SituacaoIntegracao != situacao &&
                         obj.TipoIntegracaoPagamentoMotorista == tipo
                         select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> BuscarPorPagamento(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();

            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigoPagamento select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracaoPagamentoMotorista == tipo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> Consultar(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();

            var result = from obj in query select obj;

            if (codigoPagamento > 0)
                result = result.Where(o => o.PagamentoMotoristaTMS.Codigo == codigoPagamento);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracaoPagamentoMotorista == tipo);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .ToList();
        }

        public int ContarConsulta(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();

            var result = from obj in query select obj;

            if (codigoPagamento > 0)
                result = result.Where(o => o.PagamentoMotoristaTMS.Codigo == codigoPagamento);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracaoPagamentoMotorista == tipo);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> BuscarIntegracoesPendenteDeEnvio(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista[] tipos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();
            var result = from obj in query
                         where obj.PagamentoMotoristaTMS.PagamentoLiberado
                               && obj.PagamentoMotoristaTMS.Codigo == codigoPagamento
                               && (obj.PagamentoMotoristaTMS.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgIntegracao || obj.PagamentoMotoristaTMS.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FalhaIntegracao)
                               && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                         select obj;

            if (tipos != null)
                result = result.Where(o => tipos.Contains(o.TipoIntegracaoPagamentoMotorista));

            return result.ToList();
        }
    }
}
