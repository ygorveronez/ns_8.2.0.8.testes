using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Logistica
{
    public class VeiculoDisponivelCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>
    {
        public VeiculoDisponivelCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        //todo: rever regra, melhor com metricas de peso cubagem, por enquanto usa apenas pallet, ou seja, deixar dinâmico.
        public int ContarNumeroVeiculosDisponiveisPodemFazerCarga(int numeroPallet, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> possiveisModelos, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            var result = from obj in query where obj.Disponivel && obj.Veiculo.Empresa.Codigo == empresa select obj;

            result = result.Where(obj => (possiveisModelos.Contains(obj.Veiculo.ModeloVeicularCarga) || obj.Veiculo.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao));

            result = result.Where(obj => (obj.Veiculo.ModeloVeicularCarga.NumeroPaletes >= numeroPallet || obj.Veiculo.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao));

            return result.Count();
        }

        public int ContarNumeroVeiculosDisponiveisPodemFazerCargaTransportadorTerceiro(int numeroPallet, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> possiveisModelos, double codigoTransportadorTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            var result = from obj in query where obj.Disponivel && obj.Veiculo.Proprietario.CPF_CNPJ == codigoTransportadorTerceiro select obj;

            result = result.Where(obj => (possiveisModelos.Contains(obj.Veiculo.ModeloVeicularCarga) || obj.Veiculo.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao));

            result = result.Where(obj => (obj.Veiculo.ModeloVeicularCarga.NumeroPaletes >= numeroPallet || obj.Veiculo.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao));

            return result.Count();
        }

        public int ContarNumeroVeiculosDisponiveisSemModelo(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            var result = from obj in query where obj.Disponivel && obj.Veiculo.Empresa.Codigo == empresa select obj;

            result = result.Where(obj => obj.Veiculo.ModeloVeicularCarga == null);

            return result.Count();
        }

        public int ContarNumeroVeiculosDisponiveisSemModeloTransportadorTerceiro(double codigoTransportadorTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            var result = from obj in query where obj.Disponivel && obj.Veiculo.Proprietario.CPF_CNPJ == codigoTransportadorTerceiro select obj;

            result = result.Where(obj => obj.Veiculo.ModeloVeicularCarga == null);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento BuscarPorVeiculoEmpresa(int veiculo, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            var result = from obj in query where obj.Veiculo.Codigo == veiculo && obj.Empresa.Codigo == empresa && obj.Disponivel select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento BuscarPorVeiculoProprietario(int veiculo, double codigoProprietario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            var result = from obj in query where obj.Veiculo.Codigo == veiculo && obj.Veiculo.Proprietario.CPF_CNPJ == codigoProprietario && obj.Disponivel select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento> BuscarVeiculosDisponiveisPorEmpresa(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            query = from obj in query where obj.Empresa.Codigo == empresa && obj.Disponivel select obj;

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento> Consultar(int empresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            var result = from obj in query where obj.Empresa.Codigo == empresa && obj.Disponivel select obj;

            return result.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public bool BuscarSePossuiVeiculoDisponivel(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            query = from obj in query where obj.Empresa.Codigo == empresa && obj.Disponivel select obj;

            return query.Count() > 0;
        }

        public bool BuscarSePossuiVeiculoDisponivelTransportadorTerceiro(double codigoTransportadorTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            query = from obj in query where obj.Veiculo.Proprietario.CPF_CNPJ == codigoTransportadorTerceiro && obj.Disponivel select obj;

            return query.Count() > 0;
        }

        public int ContarConsulta(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            var result = from obj in query where obj.Empresa.Codigo == empresa && obj.Disponivel select obj;

            return result.Count();
        }

    }
}
