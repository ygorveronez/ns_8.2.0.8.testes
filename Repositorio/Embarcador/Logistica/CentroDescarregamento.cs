using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>
    {
        #region Construtores

        public CentroDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CentroDescarregamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> Consultar(string descricao, double cpfCnpjDestinatario, int codigoTipoCarga, SituacaoAtivoPesquisa ativo, bool somenteCentrosOperadorLogistica, int codigoOperadorLogistica)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaCentroDescarregamento = consultaCentroDescarregamento.Where(o => o.Descricao.Contains(descricao));

            if (cpfCnpjDestinatario > 0)
                consultaCentroDescarregamento = consultaCentroDescarregamento.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            if (codigoTipoCarga > 0)
                consultaCentroDescarregamento = consultaCentroDescarregamento.Where(o => o.TiposCarga.Any(tipoCarga => tipoCarga.Codigo == codigoTipoCarga));

            if (ativo == SituacaoAtivoPesquisa.Ativo)
                consultaCentroDescarregamento = consultaCentroDescarregamento.Where(obj => obj.Ativo == true);
            else if (ativo == SituacaoAtivoPesquisa.Inativo)
                consultaCentroDescarregamento = consultaCentroDescarregamento.Where(obj => obj.Ativo == false);

            if (somenteCentrosOperadorLogistica && codigoOperadorLogistica > 0)
                consultaCentroDescarregamento = consultaCentroDescarregamento.Where(o => o.OperadoresLogistica.Any(opl => opl.Codigo == codigoOperadorLogistica));

            return consultaCentroDescarregamento;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Cliente BuscarDestinatarioPorCentroDeDescarregamento(int codigoCentro)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => o.Codigo == codigoCentro);

            return consultaCentroDescarregamento
                .Select(obj => obj.Destinatario)
                .First();
        }
        public async Task<Dominio.Entidades.Cliente> BuscarDestinatarioPorCentroDeDescarregamentoAsync(int codigoCentro)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => o.Codigo == codigoCentro);

            return await consultaCentroDescarregamento
                .Select(obj => obj.Destinatario)
                .FirstAsync();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento BuscarPorCodigo(int codigo)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaCentroDescarregamento.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> BuscarPorCodigoAsync(int codigo)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaCentroDescarregamento.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento BuscarPorDestinatario(long codigoDestinatario)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => o.Destinatario.CPF_CNPJ == codigoDestinatario && o.Ativo == true);

            return consultaCentroDescarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento BuscarPorDestinatario(double codigoDestinatario)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => o.Destinatario.CPF_CNPJ == codigoDestinatario && o.Ativo == true);

            return consultaCentroDescarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento BuscarPorDestinatarioEFilial(double codigoDestinatario, int codigoFilialCarga)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => o.Destinatario.CPF_CNPJ == codigoDestinatario && o.Ativo == true && (o.Filial == null || o.Filial.Codigo == codigoFilialCarga));

            return consultaCentroDescarregamento.OrderByDescending(o => o.Filial != null).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento BuscarPorFilial(int codigoFilialCarga)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => o.Ativo && (o.Filial == null || o.Filial.Codigo == codigoFilialCarga));

            return consultaCentroDescarregamento.OrderByDescending(o => o.Filial != null).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> BuscarPorDestinatarios(List<double> listaCpfCnpjDestinatario)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => listaCpfCnpjDestinatario.Contains(o.Destinatario.CPF_CNPJ) && o.Ativo == true);

            return consultaCentroDescarregamento.ToList();
        }

        public async Task<bool> PossuiRestricaoHorarioCarregamentoPorDestinatariosAsync(List<double> listaCpfCnpjDestinatario)
        {
            if (listaCpfCnpjDestinatario == null || listaCpfCnpjDestinatario.Count == 0)
                return false;

            int take = 2000;
            int start = 0;

            while (start < listaCpfCnpjDestinatario.Count)
            {
                List<double> tmp = listaCpfCnpjDestinatario.Skip(start).Take(take).ToList();

                bool existe = await SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                    .Where(o => tmp.Contains(o.Destinatario.CPF_CNPJ) && o.Ativo)
                    .Select(x => x.PeriodosDescarregamento)
                    .AnyAsync();

                if (existe)
                    return true;

                start += take;
            }

            return false;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>> BuscarPorDestinatariosAsync(List<double> listaCpfCnpjDestinatario)
        {
            if (listaCpfCnpjDestinatario == null || listaCpfCnpjDestinatario.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();

            int take = 2000;
            int start = 0;
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> resultado = new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();

            while (start < listaCpfCnpjDestinatario.Count)
            {
                List<double> tmp = listaCpfCnpjDestinatario.Skip(start).Take(take).ToList();

                List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> parcial = await SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                    .Where(o => tmp.Contains(o.Destinatario.CPF_CNPJ) && o.Ativo)
                    .ToListAsync();

                resultado.AddRange(parcial);

                start += take;
            }

            return resultado;
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento BuscarPorDescricao(string descricao)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => o.Descricao == descricao && o.Ativo == true);

            return consultaCentroDescarregamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> BuscarPorDestinatariosDaCarga(int codigoCarga)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = (from d in this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                                                                                  where d.TipoCarregamentoPedido != TipoCarregamentoPedido.TrocaNota &&
                                                                                        d.Carga.Codigo == codigoCarga &&
                                                                                        d.Pedido.TipoPedido != TipoPedido.Coleta &&
                                                                                        d.PedidoPallet == false
                                                                                  select d).ToList();

            List<double> cpfsCnpjsDestinatarios = cargasPedido.Select(d => (d.Recebedor ?? d.Pedido.Destinatario).CPF_CNPJ).ToList();

            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(centroDescarregamento =>
                    centroDescarregamento.Ativo == true &&
                    cpfsCnpjsDestinatarios.Contains(centroDescarregamento.Destinatario.CPF_CNPJ)
                );

            return consultaCentroDescarregamento
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> BuscarPorDestinatariosComRestricaoVeicular(List<double> listaCpfCnpjDestinatario)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => listaCpfCnpjDestinatario.Contains(o.Destinatario.CPF_CNPJ) && o.Ativo == true && o.VeiculosPermitidos.Count > 0);

            return consultaCentroDescarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> BuscarPorCanaisEntrega(List<int> codigosCanaisEntrega)
        {
            var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>()
                .Where(o => codigosCanaisEntrega.Contains(o.CanalEntrega.Codigo) && o.Ativo == true);
            return consultaCentroDescarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> Consultar(string descricao, double cpfCnpjDestinatario, int codigoTipoCarga, SituacaoAtivoPesquisa ativo, bool somenteCentrosOperadorLogistica, int codigoOperadorLogistica, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaCentroDescarregamento = Consultar(descricao, cpfCnpjDestinatario, codigoTipoCarga, ativo, somenteCentrosOperadorLogistica, codigoOperadorLogistica);

            return ObterLista(consultaCentroDescarregamento, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(string descricao, double cpfCnpjDestinatario, int codigoTipoCarga, SituacaoAtivoPesquisa ativo, bool somenteCentrosOperadorLogistica, int codigoOperadorLogistica)
        {
            var consultaCentroDescarregamento = Consultar(descricao, cpfCnpjDestinatario, codigoTipoCarga, ativo, somenteCentrosOperadorLogistica, codigoOperadorLogistica);

            return consultaCentroDescarregamento.Count();
        }

        #endregion Métodos Públicos
    }
}
