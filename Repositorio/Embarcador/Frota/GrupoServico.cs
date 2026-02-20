using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class GrupoServico : RepositorioBase<Dominio.Entidades.Embarcador.Frota.GrupoServico>
    {
        public GrupoServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.GrupoServico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServico>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.GrupoServico> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServico>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.GrupoServico> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGrupoServico filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.GrupoServico> query = Consultar(filtrosPesquisa);

            query = query.Fetch(o => o.TipoOrdemServico);

            return ObterLista(query, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGrupoServico filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.GrupoServico> query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Frota.GrupoServico BuscarGrupoServicoSugerido(int km, int horimetro, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento)
        {
            //Primeiro por veículo
            if (veiculo != null)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServico>();

                if (km > 0)
                    query = query.Where(o => (km >= o.KmInicial && km <= o.KmFinal) || (o.KmInicial == 0 && o.KmFinal == 0));

                query = query.Where(o => o.Ativo && (o.TipoVeiculoEquipamento == VeiculoEquipamento.Todos || o.TipoVeiculoEquipamento == VeiculoEquipamento.Veiculo));

                int codigoModelo = veiculo.Modelo?.Codigo ?? 0;
                int codigoMarca = veiculo.Marca?.Codigo ?? 0;

                if (codigoModelo > 0 && codigoMarca > 0)
                    query = query.Where(o => (o.ModelosVeiculo.Any(m => m.Codigo == codigoModelo) || !o.PossuiModelosVeiculo) && (o.MarcasVeiculo.Any(m => m.Codigo == codigoMarca) || !o.PossuiMarcasVeiculo));
                else if (codigoModelo > 0)
                    query = query.Where(o => o.ModelosVeiculo.Any(m => m.Codigo == codigoModelo) || !o.PossuiModelosVeiculo);
                else if (codigoMarca > 0)
                    query = query.Where(o => o.MarcasVeiculo.Any(m => m.Codigo == codigoMarca) || !o.PossuiMarcasVeiculo);
                else
                    query = query.Where(o => !o.PossuiModelosVeiculo && !o.PossuiMarcasVeiculo);

                double qtdDias = 0;
                if (veiculo.DataCompra.HasValue)
                {
                    qtdDias = (DateTime.Now.Date - veiculo.DataCompra.Value.Date).TotalDays;
                    query = query.Where(o => (qtdDias >= o.DiaInicial && qtdDias <= o.DiaFinal) || (o.DiaInicial == 0 && o.DiaFinal == 0));
                }

                bool encontrou = query.Count() > 0;
                if (encontrou)
                    return query
                            .OrderByDescending(o => o.PossuiModelosVeiculo)
                            .OrderByDescending(o => o.PossuiMarcasVeiculo)
                            .FirstOrDefault();
            }

            //Segundo por equipamento
            if (equipamento != null)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServico>();

                if (horimetro > 0)
                    query = query.Where(o => (horimetro >= o.KmInicial && horimetro <= o.KmFinal) || (o.KmInicial == 0 && o.KmFinal == 0));

                query = query.Where(o => o.Ativo && (o.TipoVeiculoEquipamento == VeiculoEquipamento.Todos || o.TipoVeiculoEquipamento == VeiculoEquipamento.Equipamento));

                int codigoModelo = equipamento.ModeloEquipamento?.Codigo ?? 0;
                int codigoMarca = equipamento.MarcaEquipamento?.Codigo ?? 0;

                if (codigoModelo > 0 && codigoMarca > 0)
                    query = query.Where(o => (o.ModelosEquipamento.Any(m => m.Codigo == codigoModelo) || !o.PossuiModelosEquipamento) && (o.MarcasEquipamento.Any(m => m.Codigo == codigoMarca) || !o.PossuiMarcasEquipamento));
                else if (codigoModelo > 0)
                    query = query.Where(o => o.ModelosEquipamento.Any(m => m.Codigo == codigoModelo) || !o.PossuiModelosEquipamento);
                else if (codigoMarca > 0)
                    query = query.Where(o => o.MarcasEquipamento.Any(m => m.Codigo == codigoMarca) || !o.PossuiMarcasEquipamento);
                else
                    query = query.Where(o => !o.PossuiModelosEquipamento && !o.PossuiMarcasEquipamento);

                double qtdDias = 0;
                if (equipamento.DataAquisicao.HasValue)
                {
                    qtdDias = (DateTime.Now.Date - equipamento.DataAquisicao.Value.Date).TotalDays;
                    query = query.Where(o => (qtdDias >= o.DiaInicial && qtdDias <= o.DiaFinal) || (o.DiaInicial == 0 && o.DiaFinal == 0));
                }

                bool encontrou = query.Count() > 0;
                if (encontrou)
                    return query
                            .OrderByDescending(o => o.PossuiModelosEquipamento)
                            .OrderByDescending(o => o.PossuiMarcasEquipamento)
                            .FirstOrDefault();
            }

            return null;
        }

        public List<Dominio.Entidades.Embarcador.Frota.GrupoServico> BuscarPorServicoVeiculo(int codigoServicoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServico>();
            var queryServicoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServicoServicoVeiculo>();

            var resultQueryServicoVeiculo = from obj in queryServicoVeiculo select obj;
            query = query.Where(obj => resultQueryServicoVeiculo.Where(s => s.ServicoVeiculoFrota.Codigo == codigoServicoVeiculo && s.GrupoServico.Codigo == obj.Codigo).Any());

            return query.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.GrupoServico> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGrupoServico filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServico>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Status != SituacaoAtivoPesquisa.Todos)
            {
                if (filtrosPesquisa.Status == SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(o => o.Ativo);
                else if (filtrosPesquisa.Status == SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(o => !o.Ativo);
            }

            if (filtrosPesquisa.CodigoTipoOrdemServico > 0)
                query = query.Where(o => o.TipoOrdemServico.Codigo == filtrosPesquisa.CodigoTipoOrdemServico || o.TipoOrdemServico == null);

            if (filtrosPesquisa.CpfCnpjLocalManutencao > 0d || filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                var queryLocalManutencao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao>();
                query = query.Where(o => queryLocalManutencao.Any(s => s.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjLocalManutencao && s.GrupoServico.Codigo == o.Codigo));
            }

            return query;
        }

        #endregion
    }
}
