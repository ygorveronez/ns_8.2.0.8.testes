using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Cargas.Retornos
{
    public sealed class RetornoCargaColetaBackhaul : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul>
    {
        #region Construtores

        public RetornoCargaColetaBackhaul(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCargaColetaBackhaul filtrosPesquisa)
        {
            var consultaRetornoCargaColetaBackhaul = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul>()
                .Where(o =>
                    o.RetornoCarga.Carga.SituacaoCarga == SituacaoCarga.Encerrada ||
                    o.RetornoCarga.Carga.SituacaoCarga == SituacaoCarga.EmTransporte ||
                    o.RetornoCarga.Carga.SituacaoCarga == SituacaoCarga.LiberadoPagamento
                );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.RetornoCarga.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.RetornoCarga.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.RetornoCarga.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || obj.RetornoCarga.Carga.VeiculosVinculados.Any(reb => reb.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.RetornoCarga.Carga.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.RetornoCarga.Carga.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoOrigem > 0)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.RetornoCarga.Carga.Pedidos.Any(ped => ped.Origem.Codigo == filtrosPesquisa.CodigoOrigem));

            if (filtrosPesquisa.CodigoDestino > 0)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.RetornoCarga.Carga.Pedidos.Any(ped => ped.Destino.Codigo == filtrosPesquisa.CodigoDestino));

            if (filtrosPesquisa.CodigoMotorista > 0)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.RetornoCarga.Carga.Motoristas.Any(mot => mot.Codigo == filtrosPesquisa.CodigoMotorista));

            if (filtrosPesquisa.CpfCnpjRemetente > 0d)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.RetornoCarga.Carga.Pedidos.Any(ped => ped.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente) || obj.RetornoCarga.Carga.Pedidos.Any(ped => ped.Expedidor.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente));

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.RetornoCarga.Carga.Pedidos.Any(ped => ped.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario) || obj.RetornoCarga.Carga.Pedidos.Any(ped => ped.Recebedor.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Where(obj => obj.Situacao== filtrosPesquisa.Situacao);

            return consultaRetornoCargaColetaBackhaul;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul BuscarPorCodigo(int codigo)
        {
            var consultaRetornoCargaColetaBackhaul = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul>()
                .Where(o => o.Codigo == codigo);

            return consultaRetornoCargaColetaBackhaul.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul BuscarPorCargaColetaBackhaul(int codigoCarga)
        {
            var consultaRetornoCargaColetaBackhaul = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul>()
                .Where(o => o.RetornoCarga.CargaColeta.Codigo == codigoCarga);

            return consultaRetornoCargaColetaBackhaul.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul BuscarPorRetornoCarga(int codigoRetornoCarga)
        {
            var consultaRetornoCargaColetaBackhaul = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul>()
                .Where(o => o.RetornoCarga.Codigo == codigoRetornoCarga);

            return consultaRetornoCargaColetaBackhaul.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul> BuscarPorSituacao(SituacaoRetornoCargaColetaBackhaul situacao, int limite)
        {
            var consultaRetornoCargaColetaBackhaul = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul>()
                .Where(o => o.Situacao == situacao);

            if (limite > 0)
                consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul.Take(limite);

            return consultaRetornoCargaColetaBackhaul.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCargaColetaBackhaul filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRetornoCargaColetaBackhaul = Consultar(filtrosPesquisa);

            consultaRetornoCargaColetaBackhaul = consultaRetornoCargaColetaBackhaul
                .Fetch(obj => obj.RetornoCarga)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.RetornoCarga)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial);

            return ObterLista(consultaRetornoCargaColetaBackhaul, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCargaColetaBackhaul filtrosPesquisa)
        {
            var consultaRetornoCargaColetaBackhaul = Consultar(filtrosPesquisa);

            return consultaRetornoCargaColetaBackhaul.Count();
        }

        #endregion
    }
}
