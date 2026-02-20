using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Cargas.Retornos
{
    public class RetornoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga>
    {
        #region Construtores

        public RetornoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCarga filtrosPesquisa)
        {
            var consultaRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga>()
                .Where(o =>
                    o.Carga.SituacaoCarga == SituacaoCarga.Encerrada ||
                    o.Carga.SituacaoCarga == SituacaoCarga.EmTransporte ||
                    o.Carga.SituacaoCarga == SituacaoCarga.LiberadoPagamento
                );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || obj.Carga.VeiculosVinculados.Any(reb => reb.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.Carga.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.Carga.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoOrigem > 0)
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.Carga.Pedidos.Any(ped => ped.Origem.Codigo == filtrosPesquisa.CodigoOrigem));

            if (filtrosPesquisa.CodigoDestino > 0)
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.Carga.Pedidos.Any(ped => ped.Destino.Codigo == filtrosPesquisa.CodigoDestino));

            if (filtrosPesquisa.CodigoMotorista > 0)
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.Carga.Motoristas.Any(mot => mot.Codigo == filtrosPesquisa.CodigoMotorista));

            if (filtrosPesquisa.CpfCnpjRemetente > 0d)
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.Carga.Pedidos.Any(ped => ped.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente) || obj.Carga.Pedidos.Any(ped => ped.Expedidor.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente));

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.Carga.Pedidos.Any(ped => ped.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario) || obj.Carga.Pedidos.Any(ped => ped.Recebedor.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario));

            if (filtrosPesquisa.Situacao != SituacaoRetornoCarga.Todas)
                consultaRetornoCarga = consultaRetornoCarga.Where(obj => obj.SituacaoRetornoCarga == filtrosPesquisa.Situacao);

            return consultaRetornoCarga;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga BuscarPorCodigo(int codigo)
        {
            var consultaRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga>()
                .Where(o => o.Codigo == codigo);

            return consultaRetornoCarga.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga BuscarPorCarga(int codigoCarga)
        {
            var consultaRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaRetornoCarga.FirstOrDefault();
        }
        
        public bool BuscarExistenciaPorNumeroCarga(string numeroCarga, int codigoCarga)
        {
            var consultaRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga>()
                .Where(o => o.Carga.CodigoCargaEmbarcador == numeroCarga && o.Carga.Codigo != codigoCarga);
            
            return consultaRetornoCarga
                .Count() > 0;
        }


        public List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga> BuscarPorCargas(List<int> codigoCarga)
        {
            var consultaRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga>()
                .Where(o => codigoCarga.Contains(o.Carga.Codigo));

            return consultaRetornoCarga.ToList();
        }


        public Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga BuscarPorCargaRetorno(int codigoCargaRetorno)
        {
            var consultaRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga>()
                .Where(o => o.CargaRetorno.Codigo == codigoCargaRetorno);

            return consultaRetornoCarga.Fetch(obj => obj.Carga).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga> BuscarPorSituacao(SituacaoRetornoCarga situacaoRetornoCarga, int limite)
        {
            var consultaRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga>()
                .Where(o => o.SituacaoRetornoCarga == situacaoRetornoCarga && o.Carga.CargaFechada);

            if (limite > 0)
                consultaRetornoCarga = consultaRetornoCarga.Take(limite);

            return consultaRetornoCarga.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRetornoCarga = Consultar(filtrosPesquisa);

            consultaRetornoCarga = consultaRetornoCarga
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial);

            return ObterLista(consultaRetornoCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCarga filtrosPesquisa)
        {
            var consultaRetornoCarga = Consultar(filtrosPesquisa);

            return consultaRetornoCarga.Count();
        }

        #endregion
    }
}
