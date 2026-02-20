using System.Linq;


namespace Dominio.Entidades
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CTE_VEICULO", EntityName = "VeiculoPreCTE", Name = "Dominio.Entidades.VeiculoPreCTE", NameType = typeof(VeiculoPreCTE))]
    public class VeiculoPreCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreConhecimentoDeTransporteEletronico PreCTE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Veiculo", Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeKG", Column = "PVE_CAPACIDADE_KG", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeKG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeM3", Column = "PVE_CAPACIDADE_M3", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeM3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "PVE_PLACA", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RENAVAM", Column = "PVE_RENAVAM", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string RENAVAM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tara", Column = "PVE_TARA", TypeType = typeof(int), NotNull = false)]
        public virtual int Tara { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCarroceria", Column = "PVE_TIPO_CARROCERIA", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string TipoCarroceria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropriedade", Column = "PVE_TIPO_PROPRIEDADE", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string TipoPropriedade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRodado", Column = "PVE_TIPO_RODADO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string TipoRodado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoVeiculo", Column = "PVE_TIPO_VEICULO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string TipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Estado", Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Proprietario", Class = "ProprietarioVeiculoCTe", Column = "PVC_CODIGO", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProprietarioVeiculoCTe Proprietario { get; set; }

        public virtual void SetarDadosVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo != null)
            {
                this.CapacidadeKG = veiculo.CapacidadeKG;
                this.CapacidadeM3 = veiculo.CapacidadeM3;
                this.Estado = veiculo.Estado;
                this.Placa = veiculo.Placa;
                this.RENAVAM = veiculo.Renavam;
                this.Tara = veiculo.Tara;
                this.TipoCarroceria = veiculo.TipoCarroceria;
                this.TipoPropriedade = veiculo.Tipo;
                this.TipoRodado = veiculo.TipoRodado;
                this.TipoVeiculo = veiculo.TipoVeiculo;

                if (veiculo.Tipo == "T" && veiculo.Proprietario != null)
                {
                    if (this.Proprietario == null)
                        this.Proprietario = new ProprietarioVeiculoCTe();

                    this.Proprietario.CPF_CNPJ = veiculo.Proprietario.CPF_CNPJ_SemFormato;
                    this.Proprietario.Estado = veiculo.Proprietario.Localidade.Estado;
                    this.Proprietario.IE = veiculo.Proprietario.IE_RG;
                    this.Proprietario.Nome = Utilidades.String.Left(veiculo.Proprietario.Nome, 60);
                    this.Proprietario.RNTRC = string.Format("{0:00000000}", veiculo.RNTRC);
                    this.Proprietario.Tipo = veiculo.TipoProprietario;
                }
            }
        }
         
        public virtual string Motorista
        {
            get
            {
                Usuario motorista = (from m in this.Veiculo.Motoristas where m.Principal select m.Motorista).FirstOrDefault(); 
                return this.Veiculo != null && motorista != null ? string.Concat(motorista.CPF, " - ", motorista.Nome) : string.Empty;
            }
        }

        public virtual string SiglaUF
        {
            get
            {
                return this.Estado != null ? this.Estado.Sigla : string.Empty;
            }
        }

        public virtual string RNTRC
        {
            get
            {
                return this.Proprietario != null ? this.Proprietario.RNTRC : string.Empty;
            }
        }
    }
}
