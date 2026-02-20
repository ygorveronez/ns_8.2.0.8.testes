using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoCodigoExterno : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno>
    {
        public IntegracaoCodigoExterno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno BuscarPorCPFCNPJETipo(string cnfcnpj, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCodigoExternoIntegracao tipoCodigoExterno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno>();

            query = query.Where(o => o.CPF_CNPJ == cnfcnpj && o.TipoCodigoExternoIntegracao == tipoCodigoExterno && o.TipoIntegracao == tipoIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno BuscarPorVeiculoETipo(int codigoveiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCodigoExternoIntegracao tipoCodigoExterno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno>();

            query = query.Where(o => o.Veiculo.Codigo == codigoveiculo && o.TipoCodigoExternoIntegracao == tipoCodigoExterno && o.TipoIntegracao == tipoIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno BuscarPorMarcaETipo(int codigoMarca, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCodigoExternoIntegracao tipoCodigoExterno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno>();

            query = query.Where(o => o.Marca.Codigo == codigoMarca && o.TipoCodigoExternoIntegracao == tipoCodigoExterno && o.TipoIntegracao == tipoIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno BuscarPorModeloETipo(int codigoModelo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCodigoExternoIntegracao tipoCodigoExterno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno>();

            query = query.Where(o => o.Modelo.Codigo == codigoModelo && o.TipoCodigoExternoIntegracao == tipoCodigoExterno && o.TipoIntegracao == tipoIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno BuscarPorLocalidadeETipo(int codigoLocalidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCodigoExternoIntegracao tipoCodigoExterno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno>();

            query = query.Where(o => o.Localidade.Codigo == codigoLocalidade && o.TipoCodigoExternoIntegracao == tipoCodigoExterno && o.TipoIntegracao == tipoIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno BuscarPorModeloVeicularETipo(int codigoModeloVeicular, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCodigoExternoIntegracao tipoCodigoExterno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno>();

            query = query.Where(o => o.ModeloVeicular.Codigo == codigoModeloVeicular && o.TipoCodigoExternoIntegracao == tipoCodigoExterno && o.TipoIntegracao == tipoIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno BuscarPorContratoFreteETipo(int codigoContratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCodigoExternoIntegracao tipoCodigoExterno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno>();

            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete && o.TipoCodigoExternoIntegracao == tipoCodigoExterno && o.TipoIntegracao == tipoIntegracao);

            return query.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno BuscarPorPagamentoMotoristaTMSETipo(int codigoPagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCodigoExternoIntegracao tipoCodigoExterno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno>();

            query = query.Where(o => o.PagamentoMotoristaTMS.Codigo == codigoPagamentoMotorista && o.TipoCodigoExternoIntegracao == tipoCodigoExterno && o.TipoIntegracao == tipoIntegracao);

            return query.FirstOrDefault();
        }

    }
}
