using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoLiberacaoGR : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoLiberacaoGR>
    {
        public VeiculoLiberacaoGR(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoLiberacaoGR BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoLiberacaoGR>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoLiberacaoGR> BuscarPorCodigoVeiculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoLiberacaoGR>();
            var result = from obj in query where obj.Veiculo.Codigo == codigo select obj;
            return result.ToList();
        }

        public bool Excluir(int codigo)
        {
            try
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoLiberacaoGR>();
                var itemExcluir = (from obj in query where obj.Codigo == codigo select obj);

                if (itemExcluir != null && itemExcluir.Count() > 0)
                    foreach (var item in itemExcluir)
                        this.SessionNHiBernate.Delete(item);

                return true;

            }
            catch (Exception ex)
            {
                var erro = ex.Message;
                return false;
            }
        }

        public bool ExcluirTodosPorVeiculo(int codigo)
        {
            try
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoLiberacaoGR>();
                var veiculosParaExcluir = (from obj in query where obj.Veiculo.Codigo == codigo select obj);

                if (veiculosParaExcluir != null && veiculosParaExcluir.Count() > 0)
                    foreach (var item in veiculosParaExcluir)
                        this.SessionNHiBernate.Delete(item);

                return true;

            }
            catch (Exception ex)
            {
                var erro = ex.Message;
                return false;
            }
        }
    }
}
