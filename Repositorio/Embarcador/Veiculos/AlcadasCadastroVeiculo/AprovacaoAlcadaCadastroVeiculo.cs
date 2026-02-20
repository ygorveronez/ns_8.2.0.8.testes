using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo
{
    public class AprovacaoAlcadaCadastroVeiculo : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo,
        Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo,
        Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo
    >
    {
        #region Construtores

        public AprovacaoAlcadaCadastroVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaCadastroVeiculoAprovacao filtrosPesquisa)
        {
            var consultaEntidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo>();

            var consultaAlcada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroVeiculo.Aprovado)
                consultaEntidade = consultaEntidade.Where(o => !o.Finalizado);

            if (filtrosPesquisa.Codigo > 0)
                consultaEntidade = consultaEntidade.Where(o => o.Codigo == filtrosPesquisa.Codigo);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaEntidade = consultaEntidade.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaEntidade = consultaEntidade.Where(o => o.Veiculo.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoModeloVeicular > 0)
                consultaEntidade = consultaEntidade.Where(o => o.Veiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicular);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroVeiculo.Todos)
                consultaEntidade = consultaEntidade.Where(o => o.Veiculo.SituacaoCadastro == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaEntidade = consultaEntidade.Where(o => o.DataCadastro >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaEntidade = consultaEntidade.Where(o => o.DataCadastro <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroVeiculo.SemRegraAprovacao)
                return consultaEntidade.Where(o => consultaAlcada.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaEntidade;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaCadastroVeiculoAprovacao filtrosPesquisa, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var cadastros = Consultar(filtrosPesquisa);

            return ObterLista(cadastros, propriedadeOrdenacao, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaCadastroVeiculoAprovacao filtrosPesquisa)
        {
            var cadastros = Consultar(filtrosPesquisa);

            return cadastros.Count();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo> ObterAutorizacoesRejeitadas(int codigoOrigem)
        {
            var consultaAlcada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo>()
                .Where(o => !o.Bloqueada)
                .Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                .Where(o => o.Motivo != null && o.Motivo != "")
                .Where(o => o.OrigemAprovacao.Codigo == codigoOrigem);

            return consultaAlcada.ToList();
        }

        public bool BuscarSeExisteRegraPorFilial()
        {
            var regraAutorizacaoCadastroVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo>();
            var result = from obj in regraAutorizacaoCadastroVeiculo where obj.AlcadasFilial.Count > 0 select obj;

            return result.Any();
        }

        #endregion Métodos Públicos
    }
}
