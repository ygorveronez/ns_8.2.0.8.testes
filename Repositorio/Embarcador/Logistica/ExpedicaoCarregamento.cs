using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class ExpedicaoCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento>
    {
        #region Construtores

        public ExpedicaoCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento> BuscarPorProduto(int codigoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana dia)
        {
            var consultaExpedicaoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento>()
                .Where(o => o.Dia == dia && o.ProdutoEmbarcador.Codigo == codigoProduto && o.ClienteDestino != null && o.Quantidade > 0 && o.CentroCarregamento.Ativo);

            return consultaExpedicaoCarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento> BuscarPorDiaSemana(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana dia)
        {
            var consultaExpedicaoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento>()
                .Where(o => o.Dia == dia && o.ClienteDestino != null && o.Quantidade > 0 && o.CentroCarregamento.Ativo);

            return consultaExpedicaoCarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga> BuscarPorProdutoECentro(int codigoProduto, int codigoCentroCarregamento, double cpfCnpjClienteDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana dia)
        {
            var consultaExpedicaoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento>()
                .Where(o => 
                    o.Dia == dia &&
                    o.ProdutoEmbarcador.Codigo == codigoProduto &&
                    o.CentroCarregamento.Ativo &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.ClienteDestino.CPF_CNPJ == cpfCnpjClienteDestino
                );

            return consultaExpedicaoCarregamento
                .SelectMany(o => o.ModelosVeicularesCargaExclusivo)
                .ToList();
        }

        #endregion
    }
}
