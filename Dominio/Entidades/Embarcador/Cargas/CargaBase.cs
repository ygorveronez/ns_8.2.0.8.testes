using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Cargas
{
    public abstract class CargaBase : EntidadeBase
    {
        #region Propriedades

        public virtual int Codigo { get; set; }

        public virtual CargaDadosSumarizados DadosSumarizados { get; set; }

        public virtual Empresa Empresa { get; set; }

        public virtual Filiais.Filial Filial { get; set; }

        public virtual Filiais.Filial FilialOrigem { get; set; }

        public virtual ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        public virtual RotaFrete Rota { get; set; }

        public virtual TipoDeCarga TipoDeCarga { get; set; }

        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        public virtual Veiculo Veiculo { get; set; }

        public virtual ICollection<Usuario> ListaMotorista { get; set; }

        public virtual ICollection<Veiculo> VeiculosVinculados { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public virtual string DescricaoEntidade
        {
            get
            {
                return ObterDescricaoEntidade();
            }
        }

        public virtual ModeloVeicularCarga ModeloVeicularCargaVeiculo
        {
            get
            {
                ModeloVeicularCarga modeloVeicularCargaReboque = null;

                if (VeiculosVinculados?.Count > 0)
                    modeloVeicularCargaReboque = (from veiculo in VeiculosVinculados where veiculo.ModeloVeicularCarga != null select veiculo.ModeloVeicularCarga).FirstOrDefault();

                return modeloVeicularCargaReboque ?? Veiculo?.ModeloVeicularCarga;
            }
        }

        public virtual string Numero
        {
            get
            {
                return ObterNumero();
            }
        }

        public virtual string RetornarDescricaoMotoristas
        {
            get
            {
                if (ListaMotorista?.Count > 0)
                    return string.Join(", ", from motorista in ListaMotorista select motorista.Descricao);

                return "";
            }
        }

        public virtual string RetornarDescricaoTelefoneMotoristas
        {
            get
            {
                if (ListaMotorista?.Count > 0)
                    return string.Join(", ", from motorista in ListaMotorista select motorista.DescricaoTelefone);

                return "";
            }
        }

        public virtual string RetornarMotoristas
        {
            get
            {
                if (ListaMotorista?.Count > 0)
                    return string.Join(", ", from motorista in ListaMotorista select motorista.Nome);

                return "";
            }
        }

        public virtual string RetornarTelefoneMotoristas
        {
            get
            {
                if (ListaMotorista?.Count > 0)
                    return string.Join(", ", from motorista in ListaMotorista select motorista.Telefone_Formatado);

                return "";
            }
        }

        public virtual string RetornarPlacas
        {
            get
            {
                List<string> placas = new List<string>();

                if (Veiculo != null)
                    placas.Add(Veiculo.Placa);

                if (VeiculosVinculados?.Count > 0)
                    placas.AddRange(VeiculosVinculados.Select(reboque => reboque.Placa));

                return string.Join(", ", placas);
            }
        }

        public virtual string RetornarVeiculoLicencas
        {
            get
            {
                List<string> licencas = new List<string>();

                if (Veiculo != null)
                    licencas.AddRange(Veiculo.LicencasVeiculo.Where(licenca => licenca.Licenca != null).Select(licenca => licenca.Licenca.Descricao));

                if (VeiculosVinculados?.Count > 0)
                    licencas.AddRange(VeiculosVinculados.SelectMany(veiculo => veiculo.LicencasVeiculo.Select(licenca => licenca.Licenca.Descricao)));

                return string.Join(", ", licencas.Distinct());
            }
        }

        public virtual List<int> RetornarCodigos
        {
            get
            {
                List<int> codigos = new List<int>();

                if (Veiculo != null)
                    codigos.Add(Veiculo.Codigo);

                if (VeiculosVinculados?.Count > 0)
                    codigos.AddRange(VeiculosVinculados.Select(reboque => reboque.Codigo));

                return codigos;
            }
        }

        #endregion Propriedades com Regras

        #region Métodos Públicos

        public virtual bool IsPossuiRestricaoFilaCarregamentoPorDestinatario()
        {
            if (DadosSumarizados?.ClientesDestinatarios == null)
                return false;
            return (from destinatario in DadosSumarizados?.ClientesDestinatarios where destinatario.IsPossuiRestricaoFilaCarregamento(TipoDeCarga, Enumeradores.TipoTomador.Destinatario) select destinatario).Count() > 0;
        }

        public virtual bool IsPossuiRestricaoFilaCarregamentoPorRemetente()
        {
            if (DadosSumarizados?.ClientesRemetentes == null)
                return false;
            return (from destinatario in DadosSumarizados?.ClientesRemetentes where destinatario.IsPossuiRestricaoFilaCarregamento(TipoDeCarga, Enumeradores.TipoTomador.Remetente) select destinatario).Count() > 0;
        }

        public virtual string ObterPlacasComDescricao()
        {
            string placas = string.Empty;

            if (Veiculo != null)
                placas += $"{Veiculo.Placa} ({Veiculo.ModeloVeicularCarga?.Descricao}) ";

            if (VeiculosVinculados?.Count > 0)
                placas += $", {string.Join(", ", VeiculosVinculados.Select(x => (x.Placa + " (" + x.ModeloVeicularCarga?.Descricao + ")")))}";

            return placas;
        }

        public virtual bool IsDadosTransporteInformados()
        {
            if (Empresa == null)
                return false;

            if (Veiculo == null)
                return false;

            if (!ListaMotorista.Any())
                return false;

            return true;
        }

        #endregion Métodos Públicos

        #region Métodos Públicos Abstratos

        public abstract bool IsCarga();

        protected abstract string ObterDescricaoEntidade();

        protected abstract string ObterNumero();

        #endregion Métodos Públicos Abstratos
    }
}
