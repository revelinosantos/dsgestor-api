using DsGestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Data;

public class DsGestorDbContext : DbContext
{
    public DsGestorDbContext(DbContextOptions<DsGestorDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Vendedor> Vendedores => Set<Vendedor>();
    public DbSet<Supervisor> Supervisores => Set<Supervisor>();
    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();
    public DbSet<LancamentoDet> LancamentosDet => Set<LancamentoDet>();
    public DbSet<Promocao> Promocoes =>  Set<Promocao>();
    public DbSet<PromocaoItem> PromocaoItens => Set<PromocaoItem>();
    public DbSet<PromocaoCliente> PromocaoClientes => Set<PromocaoCliente>();
    public DbSet<PromocaoOrigemPedido> PromocaoOrigemPedidos => Set<PromocaoOrigemPedido>();
    public DbSet<Conferencia> Conferencias => Set<Conferencia>();
    public DbSet<ConferenciaPedidoView> ConferenciaPedidoView => Set<ConferenciaPedidoView>();
    public DbSet<CoteFacilPedido> CoteFacilPedidos => Set<CoteFacilPedido>();
    public DbSet<CoteFacilPedidoItem> CoteFacilPedidoItens => Set<CoteFacilPedidoItem>();
    public DbSet<CoteFacilFilialDistribuidor> CoteFacilFiliaisDistribuidor => Set<CoteFacilFilialDistribuidor>();
    public DbSet<CoteFacilCondicaoPagamento> CoteFacilCondicoesPagamento => Set<CoteFacilCondicaoPagamento>();
    public DbSet<CoteFacilProdutoView> CoteFacilProdutos => Set<CoteFacilProdutoView>();
    public DbSet<PcPedc> PcPedcs => Set<PcPedc>();
    public DbSet<PcPedi> PcPedis => Set<PcPedi>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ECOACRE");

        ConfigureCliente(modelBuilder);
        ConfigureProduto(modelBuilder);
        ConfigureDepartamento(modelBuilder);
        ConfigureUsuario(modelBuilder);
        ConfigureVendedor(modelBuilder);
        ConfigureSupervisor(modelBuilder);
        ConfigureLancamento(modelBuilder);
        ConfigureLancamentoDet(modelBuilder);
        ConfigurePromocao(modelBuilder);
        ConfigurePromcaoItem(modelBuilder);
        ConfigurePromocaoCliente(modelBuilder);
        ConfigurePromocaoOrigemPedido(modelBuilder);
        ConfigureConferencia(modelBuilder);
        ConfigureConferenciaPedidoView(modelBuilder);
        ConfigureCoteFacilPedido(modelBuilder);
        ConfigureCoteFacilPedidoItem(modelBuilder);
        ConfigureCoteFacilFilialDistribuidor(modelBuilder);
        ConfigureCoteFacilCondicaoPagamento(modelBuilder);
        ConfigureCoteFacilProdutoView(modelBuilder);
        ConfigurePcPedc(modelBuilder);
        ConfigurePcPedi(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureConferenciaPedidoView(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ConferenciaPedidoView>(entity =>
      {
            entity.ToView("VIEW_DSGESTOR_CONF_PED");

            entity.HasNoKey();

            entity.Property(x => x.Numped).HasColumnName("NUMPED").HasPrecision(10, 0);
            entity.Property(x => x.Dataped).HasColumnName("DATAPED");
            entity.Property(x => x.Statusped).HasColumnName("STATUSPED").HasMaxLength(2).IsUnicode(false);
            entity.Property(x => x.Codfilial).HasColumnName("CODFILIAL");
            entity.Property(x => x.Codusur).HasColumnName("CODUSUR");
            entity.Property(x => x.Condvenda).HasColumnName("CONDVENDA");
            entity.Property(x => x.Codemitenteped).HasColumnName("CODEMITENTEPED");
            entity.Property(x => x.Codcli).HasColumnName("CODCLI");
            entity.Property(x => x.Cliente).HasColumnName("CLIENTE").HasMaxLength(100).IsUnicode(false);
            entity.Property(x => x.Vltotal).HasColumnName("VLTOTAL").HasPrecision(18, 2);

            entity.Property(x => x.Codprod).HasColumnName("CODPROD");
            entity.Property(x => x.Numseq).HasColumnName("NUMSEQ");
            entity.Property(x => x.Descricao).HasColumnName("DESCRICAO").HasMaxLength(200).IsUnicode(false);

            entity.Property(x => x.Qtoriginal).HasColumnName("QTORIGINAL");
            entity.Property(x => x.Qt).HasColumnName("QT");
            entity.Property(x => x.Qtfalta).HasColumnName("QTFALTA");
            entity.Property(x => x.Qtunit).HasColumnName("QTUNIT");
            entity.Property(x => x.Qtunitcx).HasColumnName("QTUNITCX");

            entity.Property(x => x.Pvenda).HasColumnName("PVENDA").HasPrecision(18, 2);
            entity.Property(x => x.Codauxiliar).HasColumnName("CODAUXILIAR").HasMaxLength(20).IsUnicode(false);

            entity.Property(x => x.Pesobrutopro).HasColumnName("PESOBRUTOPRO").HasPrecision(18, 4);
            entity.Property(x => x.Pesoliqpro).HasColumnName("PESOLIQPRO").HasPrecision(18, 4);

            entity.Property(x => x.Codconf).HasColumnName("CODCONF");
            entity.Property(x => x.Volume).HasColumnName("VOLUME");

            entity.Property(x => x.Pesobrutoped).HasColumnName("PESOBRUTOPED").HasPrecision(18, 4);
            entity.Property(x => x.Pesoliqped).HasColumnName("PESOLIQPED").HasPrecision(18, 4);

            entity.Property(x => x.Codsep).HasColumnName("CODSEP");
            entity.Property(x => x.Nomeconf).HasColumnName("NOMECONF").HasMaxLength(100).IsUnicode(false);

            entity.Property(x => x.Codauxiliar2).HasColumnName("CODAUXILIAR2").HasMaxLength(20).IsUnicode(false);

            entity.Property(x => x.Numnota).HasColumnName("NUMNOTA");
            entity.Property(x => x.Numtransvenda).HasColumnName("NUMTRANSVENDA");

            entity.Property(x => x.Lpvid).HasColumnName("LPVID");
            entity.Property(x => x.Lpvcod).HasColumnName("LPVCOD").HasMaxLength(20).IsUnicode(false);
            entity.Property(x => x.Lpvcodfind).HasColumnName("LPVCODFIND").HasMaxLength(20).IsUnicode(false);
     });
    }
    private void ConfigureConferencia(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Conferencia>(entity =>
      {
            entity.ToTable("DSGESTOR_CONF");

            entity.HasKey(x => x.Confid);

            entity.Property(x => x.Confid).HasColumnName("CONFID")
                  .HasPrecision(10, 0)
                  .ValueGeneratedOnAdd();
         
            entity.Property(x => x.Numped).HasColumnName("NUMPED").HasPrecision(10, 0).IsRequired();
            entity.Property(x => x.Dataped).HasColumnName("DATAPED");
            entity.Property(x => x.Statusped).HasColumnName("STATUSPED").HasMaxLength(2);
            entity.Property(x => x.Codfilial).HasColumnName("CODFILIAL");
            entity.Property(x => x.Codusur).HasColumnName("CODUSUR");
            entity.Property(x => x.Condvenda).HasColumnName("CONDVENDA");
            entity.Property(x => x.Codemitenteped).HasColumnName("CODEMITENTEPED");
            entity.Property(x => x.Codcli).HasColumnName("CODCLI");
            entity.Property(x => x.Cliente).HasColumnName("CLIENTE").HasMaxLength(100);
            entity.Property(x => x.Vltotal).HasColumnName("VLTOTAL").HasPrecision(18, 2);

            entity.Property(x => x.Codprod).HasColumnName("CODPROD").IsRequired();
            entity.Property(x => x.Numseq).HasColumnName("NUMSEQ").IsRequired();
            entity.Property(x => x.Descricao).HasColumnName("DESCRICAO").HasMaxLength(200);

            entity.Property(x => x.Qtoriginal).HasColumnName("QTORIGINAL");
            entity.Property(x => x.Qt).HasColumnName("QT");
            entity.Property(x => x.Qtfalta).HasColumnName("QTFALTA");
            entity.Property(x => x.Qtunit).HasColumnName("QTUNIT");
            entity.Property(x => x.Qtunitcx).HasColumnName("QTUNITCX");

            entity.Property(x => x.Pvenda).HasColumnName("PVENDA").HasPrecision(18, 2);
            entity.Property(x => x.Codauxiliar).HasColumnName("CODAUXILIAR").HasMaxLength(20);

            entity.Property(x => x.Pesobrutopro).HasColumnName("PESOBRUTOPRO").HasPrecision(18, 4);
            entity.Property(x => x.Pesoliqpro).HasColumnName("PESOLIQPRO").HasPrecision(18, 4);

            entity.Property(x => x.Codconf).HasColumnName("CODCONF");
            entity.Property(x => x.Datainiconf).HasColumnName("DATAINICONF");
            entity.Property(x => x.Datafimconf).HasColumnName("DATAFIMCONF");

            entity.Property(x => x.Qtconfunid).HasColumnName("QTCONFUNID").HasDefaultValue(0);
            entity.Property(x => x.Qtconf).HasColumnName("QTCONF").HasDefaultValue(0);

            entity.Property(x => x.Conferido).HasColumnName("CONFERIDO").HasMaxLength(1).HasDefaultValue("N");
            entity.Property(x => x.Conferidoitem).HasColumnName("CONFERIDOITEM").HasMaxLength(1).HasDefaultValue("N");

            entity.Property(x => x.Volume).HasColumnName("VOLUME");

            entity.Property(x => x.Pesobrutoped).HasColumnName("PESOBRUTOPED").HasPrecision(18, 4);
            entity.Property(x => x.Pesoliqped).HasColumnName("PESOLIQPED").HasPrecision(18, 4);

            entity.Property(x => x.Fechadodiverg).HasColumnName("FECHADODIVERG").HasMaxLength(1).HasDefaultValue("N");
            entity.Property(x => x.Datafechdiverg).HasColumnName("DATAFECHDIVERG");

            entity.Property(x => x.Codsep).HasColumnName("CODSEP");
            entity.Property(x => x.Dataconfwinthor).HasColumnName("DATACONFWINTHOR");
            entity.Property(x => x.Nomeconf).HasColumnName("NOMECONF").HasMaxLength(100);

            entity.Property(x => x.Codfundiverg).HasColumnName("CODFUNDIVERG").HasDefaultValue(0);

            entity.Property(x => x.Codauxiliar2).HasColumnName("CODAUXILIAR2").HasMaxLength(20);

            entity.Property(x => x.Android).HasColumnName("ANDROID").HasMaxLength(1).HasDefaultValue("N");

            entity.Property(x => x.Numnota).HasColumnName("NUMNOTA").HasDefaultValue(0).IsRequired();
            entity.Property(x => x.Numtransvenda).HasColumnName("NUMTRANSVENDA").HasDefaultValue(0);

            entity.Property(x => x.Lpvid).HasColumnName("LPVID").HasDefaultValue(0);
            entity.Property(x => x.Lpvcod).HasColumnName("LPVCOD").HasMaxLength(20).HasDefaultValue("N");
            entity.Property(x => x.Lpvcodfind).HasColumnName("LPVCODFIND").HasMaxLength(20).HasDefaultValue("N");

            entity.Property(x => x.Statusconf).HasColumnName("STATUSCONF").HasMaxLength(20).HasDefaultValue("ABERTA");
            entity.Property(x => x.Usuarioinicio).HasColumnName("USUARIOINICIO");
            entity.Property(x => x.Usuariofinalizou).HasColumnName("USUARIOFINALIZOU");
            entity.Property(x => x.Dataultleitura).HasColumnName("DATAULTLEITURA");
            entity.Property(x => x.Observacao).HasColumnName("OBSERVACAO").HasMaxLength(500);
            entity.Property(x => x.Finalizadowinthor).HasColumnName("FINALIZADOWINTHOR").HasMaxLength(1).HasDefaultValue("N");
            entity.Property(x => x.Errowinthor).HasColumnName("ERROWINTHOR").HasMaxLength(1000);
                  
      });
        
    }

    private void ConfigurePromocaoCliente(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<PromocaoCliente>(entity =>
      {
            entity.ToTable("PCPROMOCAOCLIMED");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("CODPROMOCAOMED")
                  .HasPrecision(10, 0)
                  .ValueGeneratedNever();

             entity.Property(e => e.CodigoCliente)
                  .HasColumnName("CODCLI")
                  .HasPrecision(10, 0);
      });
    }

    private void ConfigurePromocaoOrigemPedido(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<PromocaoOrigemPedido>(entity =>
      {
            entity.ToTable("PCPROMOCAOORIGEMMED");

            entity.HasKey(e => new { e.Id, e.OrigemPed });
            
            entity.Property(e => e.Id)
                  .HasColumnName("CODPROMOCAOMED")
                  .HasPrecision(10, 0)
                  .ValueGeneratedNever();

             entity.Property(e => e.OrigemPed)
                  .HasColumnName("ORIGEMPED")
                  .HasMaxLength(1)
                  .IsUnicode(false);
      });
    }

    private void ConfigurePromocao(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Promocao>(entity =>
      {
            entity.ToTable("PCPROMOCAOMED");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("CODPROMOCAOMED")
                  .HasPrecision(10, 0)
                  .ValueGeneratedNever();
                
            entity.Property(e => e.IdentificadorPromocao)
                  .HasColumnName("IDENTIFICADORPROMOCAO")
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.Property(e => e.TipoPolitica)
                  .HasColumnName("TIPOPOLITICA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.DescricaoResumida)
                  .HasColumnName("DESCRICAORESUMIDA")
                  .HasMaxLength(40)
                  .IsUnicode(false);

            entity.Property(e => e.DescricaoDetalhada)
                  .HasColumnName("DESCRICAODETALHADA")
                  .HasMaxLength(200)
                  .IsUnicode(false);

            entity.Property(e => e.DataInicial)
                  .HasColumnName("DATAINICIAL");

            entity.Property(e => e.DataFinal)
                  .HasColumnName("DATAFINAL");

            entity.Property(e => e.TipoPromocao)
                  .HasColumnName("TIPOPROMOCAO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.TipoRestricao)
                  .HasColumnName("TIPORESTRICAO")
                  .HasPrecision(3, 0);

            entity.Property(e => e.CodFuncLanc)
                  .HasColumnName("CODFUNCLANC")
                  .HasPrecision(10, 0);

            entity.Property(e => e.DataLanc)
                  .HasColumnName("DATALANC");

            entity.Property(e => e.AlteraDesconto)
                  .HasColumnName("ALTERADESCONTO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.SyncFv)
                  .HasColumnName("SYNCFV")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.CodReferencia)
                  .HasColumnName("CODREFERENCIA")
                  .HasMaxLength(1000)
                  .IsUnicode(false);

            entity.Property(e => e.BaseCredDebBrca)
                  .HasColumnName("BASECREDDEBRCA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.CreditaSobrePolitica)
                  .HasColumnName("CREDITASOBREPOLITICA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.ConsideraCalcGiroMedic)
                  .HasColumnName("CONSIDERACALCGIROMEDIC")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.UtilizaDescRede)
                  .HasColumnName("UTILIZADESCREDE")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.TipoPlanoPag)
                  .HasColumnName("TIPOPLANOPAG")
                  .HasMaxLength(1);

            entity.Property(e => e.AplicaFamiliaProdutos)
                  .HasColumnName("APLICAFAMILIAPRODUTOS")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.TipoSimplesNacional)
                  .HasColumnName("TIPOSIMPLESNACIONAL")
                  .HasMaxLength(1);

            entity.Property(e => e.PontuarAcordoParceria)
                  .HasColumnName("PONTUARACORDOPARCERIA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.PrazoPlPagMandatorio)
                  .HasColumnName("PRAZOPLPAGMANDATORIO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Bloqueio)
                  .HasColumnName("BLOQUEIO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.UtilizaPercDescBaseRca)
                  .HasColumnName("UTILIZAPERCDESCBASERCA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.OrigemPreco)
                  .HasColumnName("ORIGEMPRECO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.VisualizarPromoEsp)
                  .HasColumnName("VISUALIZARPROMOESP")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.UsaNormalizacaoDesc)
                  .HasColumnName("USANORMALIZACAODESC")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.ParticipaPremiacaoCli)
                  .HasColumnName("PARTICIPAPREMIACAOCLI")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.MultiplosBrindes)
                  .HasColumnName("MULTIPLOSBRINDES")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.AplicaDesconto)
                  .HasColumnName("APLICADESCONTO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.TipoSolicitante)
                  .HasColumnName("TIPOSOLICITANTE")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.AceitaDescPromCampanha)
                  .HasColumnName("ACEITADESCPROMCAMPANHA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.ValidarMultEmbMaster)
                  .HasColumnName("VALIDARMULTEMBMASTER")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.ParticipaPremiacaoInd)
                  .HasColumnName("PARTICIPAPREMIACAOIND")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.IncluirFiltroTelevendas)
                  .HasColumnName("INCLUIRFILTROTELEVENDAS")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.TipoCotaGeral)
                  .HasColumnName("TIPOCOTAGERAL")
                  .HasPrecision(5, 0);

            entity.Property(e => e.TipoCotaCliente)
                  .HasColumnName("TIPOCOTACLIENTE")
                  .HasPrecision(5, 0);

            entity.Property(e => e.IonSync)
                  .HasColumnName("IONSYNC")
                  .HasMaxLength(1)
                  .IsUnicode(false);
      });
    }

      private void ConfigurePromcaoItem(ModelBuilder modelBuilder)
      {
        modelBuilder.Entity<PromocaoItem>(entity =>
        {
            entity.ToTable("PCDESCONTO");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("CODDESCONTO")
                  .HasPrecision(18, 0)
                  .ValueGeneratedOnAdd();
             
            entity.Property(e => e.CodigoCliente)
                  .HasColumnName("CODCLI")
                  .HasPrecision(10, 0);

            entity.Property(e => e.CodigoProduto)
                  .HasColumnName("CODPROD")
                  .HasPrecision(10, 0);

            entity.Property(e => e.PercDesc)
                  .HasColumnName("PERCDESC")
                  .HasPrecision(18, 6);

            entity.Property(e => e.DtInicio)
                  .HasColumnName("DTINICIO");

            entity.Property(e => e.DtFim)
                  .HasColumnName("DTFIM");

            entity.Property(e => e.CodFuncLanc)
                  .HasColumnName("CODFUNCLANC")
                  .HasPrecision(10, 0);

            entity.Property(e => e.DataLanc)
                  .HasColumnName("DATALANC");

            entity.Property(e => e.CodFuncUltAlter)
                  .HasColumnName("CODFUNCULTALTER")
                  .HasPrecision(10, 0);

            entity.Property(e => e.DataUltAlter)
                  .HasColumnName("DATAULTALTER");

            entity.Property(e => e.AplicaDesconto)
                  .HasColumnName("APLICADESCONTO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.BaseCredDebBrca)
                  .HasColumnName("BASECREDDEBRCA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.UtilizaDescRede)
                  .HasColumnName("UTILIZADESCREDE")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.CreditaSobrePolitica)
                  .HasColumnName("CREDITASOBREPOLITICA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Tipo)
                  .HasColumnName("TIPO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.OrigemPed)
                  .HasColumnName("ORIGEMPED")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.CodigoPromocao)
                  .HasColumnName("CODPROMOCAOMED")
                  .HasPrecision(10, 0);

            entity.Property(e => e.TipoPoliticaPromocaoMed)
                  .HasColumnName("TIPOPOLITICAPROMOCAOMED")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.SyncFv)
                  .HasColumnName("SYNCFV")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.IonSync)
                  .HasColumnName("IONSYNC")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Descricao)
                  .HasColumnName("DESCRICAO")
                  .HasMaxLength(200)
                  .IsUnicode(false);

            entity.Property(e => e.GrupoValidacao)
                  .HasColumnName("GRUPO_VALIDACAO")
                  .HasMaxLength(10)
                  .IsUnicode(false);

            // relacionamento lógico (sem navegação obrigatória)
            entity.HasIndex(e => e.CodigoPromocao);
      });
    }
    private void ConfigureProduto(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Produto>(entity =>
      {
   
      entity.ToView("VIEW_DSGESTOR_PRO_MARGEM"); 
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).HasColumnName("CODPROD").HasPrecision(6, 0);
   
      entity.Property(e => e.CodigoFilial).HasColumnName("CODFILIAL");
      entity.Property(e => e.CodigoRegiao).HasColumnName("NUMREGIAO").HasPrecision(4, 0);
      entity.Property(e => e.NomeRegiao).HasColumnName("REGIAO");
      entity.Property(e => e.Estado).HasColumnName("UF");
      entity.Property(e => e.Descricao).HasColumnName("DESCRICAO");
      entity.Property(e => e.CodigoFornecedor).HasColumnName("CODFORNEC").HasPrecision(6, 0);
      entity.Property(e => e.Fornecedor).HasColumnName("FORNECEDOR");

      entity.Property(e => e.CodigoDepto).HasColumnName("CODEPTO").HasPrecision(6, 0);
      entity.Property(e => e.DescricaoDepto).HasColumnName("DESCRICAODEPTO");

      entity.Property(e => e.Unidade).HasColumnName("UNIDADE");
      entity.Property(e => e.DataUltimaEntrada).HasColumnName("DTULTENT");

      entity.Property(e => e.CustoFinanceiro).HasColumnName("CUSTO_FIN");
      entity.Property(e => e.CodigoIcmTab).HasColumnName("CODICMTAB");
      entity.Property(e => e.Estoque).HasColumnName("QTESTGER");
      entity.Property(e => e.PrecoTabela).HasColumnName("PTABELA");
      entity.Property(e => e.PrecoVenda).HasColumnName("PVENDA");
      entity.Property(e => e.MargemIdeal).HasColumnName("MARGEM_IDEAL");
      entity.Property(e => e.PrecoMinimo).HasColumnName("PRECOMINIMO");
      });

    }
    private static void ConfigureCliente(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Cliente>();

        entity.ToTable("VIEW_DSGESTOR_CLIENTE");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("CODCLI");

        entity.Property(e => e.RazaoSocial)
              .HasColumnName("CLIENTE")
              .HasMaxLength(200)
              .IsRequired();

        entity.Property(e => e.NomeFantasia)
              .HasColumnName("FANTASIA")
              .HasMaxLength(200);

        entity.Property(e => e.CnpjCpf)
              .HasColumnName("CGCENT")
              .HasMaxLength(20)
              .IsRequired();

        entity.Property(e => e.Endereco)
              .HasColumnName("ENDERENT")
              .HasMaxLength(250);

        entity.Property(e => e.Bairro)
              .HasColumnName("BAIRROENT")
              .HasMaxLength(100);

        entity.Property(e => e.Cidade)
              .HasColumnName("NOMECIDADE")
              .HasMaxLength(100);

        entity.Property(e => e.Estado)
              .HasColumnName("UF")
              .HasMaxLength(2);

        entity.Property(e => e.CodigoFilial)
              .HasColumnName("CODFILIALNF")
              .HasMaxLength(2);
       
        entity.Property(e => e.CodigoVendedor)
              .HasColumnName("CODUSUR");
       
        entity.Property(e => e.NomeVendedor)
              .HasColumnName("NOME");
       
       entity.Property(e => e.CodigoSupervisor)
              .HasColumnName("CODSUPERVISOR");     
  
       entity.Property(e => e.NomeSupervisor)
              .HasColumnName("NOMESUPERVISOR");    
     
       entity.Property(e => e.DataCadastro)
              .HasColumnName("DTCADASTRO");     
       
       entity.Property(e => e.DataUltimaCompra)
              .HasColumnName("DTULTCOMP");                                        

       entity.Property(e => e.Fone)
              .HasColumnName("TELENT");  
   
       entity.Property(e => e.LimiteCredito)
              .HasColumnName("LIMCRED");       
      
       entity.Property(e => e.NumRegiao)
              .HasColumnName("NUMREGIAO");

       entity.Property(e => e.NomeRegiao)
              .HasColumnName("REGIAO");
                  
    }
    private static void ConfigureDepartamento(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Departamento>();

        entity.ToTable("PCDEPTO");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("CODEPTO");

        entity.Property(e => e.Descricao)
              .HasColumnName("DESCRICAO")
              .HasMaxLength(150)
              .IsRequired();
    }
    private static void ConfigureUsuario(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Usuario>();

        entity.ToTable("DSGESTOR_USUARIO");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("ID");

        entity.Property(e => e.Nome)
              .HasColumnName("NOME")
              .HasMaxLength(150)
              .IsRequired();

        entity.Property(e => e.Email)
              .HasColumnName("EMAIL")
              .HasMaxLength(200)
              .IsRequired();

        entity.Property(e => e.Pwd)
              .HasColumnName("PWD")
              .HasMaxLength(200)
              .IsRequired();

        entity.Property(e => e.Perfil)
              .HasColumnName("PERFIL")
              .HasConversion<int>(); // enum -> int

        entity.Property(e => e.Codigo)
              .HasColumnName("MATRICULA")
              .HasMaxLength(50);

        entity.Property(e => e.CodigoVendedor)
              .HasColumnName("CODUSUR");

        entity.Property(e => e.CodigoSupervisor)
              .HasColumnName("CODSUPERVISOR");
        entity.HasOne(e => e.Vendedor)
              .WithMany()
              .HasForeignKey(e => e.CodigoVendedor)
              .HasPrincipalKey(v => v.Id);
        entity.HasOne(e => e.Supervisor)
              .WithMany()
              .HasForeignKey(e => e.CodigoSupervisor)
              .HasPrincipalKey(v => v.Id);
 
    }
    private static void ConfigureVendedor(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Vendedor>();

        entity.ToTable("PCUSUARI");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("CODUSUR");

        entity.Property(e => e.Nome)
              .HasColumnName("NOME")
              .HasMaxLength(150)
              .IsRequired();

        entity.Property(e => e.DataTermino )
                  .HasColumnName("DTTERMINO");

        entity.Property(e => e.CodigoSupervisor)
              .HasColumnName("CODSUPERVISOR");  
              
        entity.HasOne(e => e.Supervisor)
              .WithMany()
              .HasForeignKey(e => e.CodigoSupervisor)
              .HasPrincipalKey(v => v.Id);
    }
   
    private static void ConfigureSupervisor(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Supervisor>();

        entity.ToTable("PCSUPERV");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("CODSUPERVISOR");

        entity.Property(e => e.Nome)
              .HasColumnName("NOME")
              .HasMaxLength(150)
              .IsRequired();
    }

    private static void ConfigureLancamento(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Lancamento>();

        entity.ToTable("DSGESTOR_LANC");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.Data).HasColumnName("DATA");

        entity.Property(e => e.CodigoUsuario).HasColumnName("ID_USUARIO").IsRequired();
        entity.Property(e => e.CodigoPromocao).HasColumnName("CODPROMOCAOMED");
    
        entity.Property(e => e.CodigoVendedor).HasColumnName("CODUSUR");
        entity.Property(e => e.CodigoCliente).HasColumnName("CODCLI");
        entity.Property(e => e.CodigoFilial).HasColumnName("CODFILIALNF");
        entity.Property(e => e.NumRegiao).HasColumnName("NUMREGIAO");
        entity.Property( e => e.Observacao).HasColumnName("OBS").HasMaxLength(4000);

        entity.Property(e => e.CodigoUsuarioAut).HasColumnName("ID_USUARIO_AUT");
        entity.Property(e => e.DataAutorizacao).HasColumnName("DATA_AUT");

        entity.Property(e => e.Status)
              .HasColumnName("STATUS")
              .HasConversion<int>();

         entity.HasMany(e => e.Itens)
              .WithOne(d => d.Lancamento)
              .HasForeignKey(d => d.CodigoLancamento)
              .HasConstraintName("FK_DSGESTOR_LANC_DET_LANC");
       
         entity.HasOne(e => e.Cliente)
              .WithMany()
              .HasForeignKey(e => new { e.CodigoCliente, e.CodigoVendedor, e.CodigoFilial })
              .HasPrincipalKey(c => new { c.Id, c.CodigoVendedor, c.CodigoFilial });
             
         entity.HasOne(e => e.Vendedor)
              .WithMany()
              .HasForeignKey(e => e.CodigoVendedor)
              .HasPrincipalKey(v => v.Id);
                   
         entity.HasOne(e => e.Usuario)
              .WithMany()
              .HasForeignKey(e => e.CodigoUsuario)
               .HasPrincipalKey(c => c.Id);

         entity.HasOne(e => e.UsuarioAut)
              .WithMany()
              .HasForeignKey(e => e.CodigoUsuarioAut)
              .HasPrincipalKey(c => c.Id);
      }

      private static void ConfigureLancamentoDet(ModelBuilder modelBuilder)
      {
          var entity = modelBuilder.Entity<LancamentoDet>();

          entity.ToTable("DSGESTOR_LANC_DET");

          entity.HasKey(e => e.Id);
          entity.Property(e => e.Id).HasColumnName("ID");

          entity.Property(e => e.CodigoLancamento).HasColumnName("ID_LANC").IsRequired();
          entity.Property(e => e.CodigoProduto).HasColumnName("CODPROD").IsRequired();
      
          entity.Property(e => e.PrecoCustoFin).HasColumnName("PCUSTO_FIN").HasPrecision(18, 6);
          entity.Property(e => e.CodigoIcmTab).HasColumnName("CODICMTAB").HasPrecision(6, 4);
          entity.Property(e => e.PrecoVenda).HasColumnName("PVENDA").HasPrecision(18, 6);

          entity.Property(e => e.MargemIdeal).HasColumnName("MARGEM_IDEAL").HasPrecision(6, 4);
          entity.Property(e => e.Quantidade).HasColumnName("QTD").HasPrecision(12, 4);

          entity.Property(e => e.PrecoUnitario).HasColumnName("PUNIT").HasPrecision(18, 6);
          entity.Property(e => e.PercDesconto).HasColumnName("PERC_DESC").HasPrecision(6, 4);
          entity.Property(e => e.Margem).HasColumnName("MARGEM").HasPrecision(6, 4);

          entity.Property(e => e.QuantidadeAut).HasColumnName("QTD_AUT").HasPrecision(12, 4);
          entity.Property(e => e.PrecoUnitarioAut).HasColumnName("PUNIT_AUT").HasPrecision(18, 6);
          entity.Property(e => e.PercDescontoAut).HasColumnName("PERC_DESC_AUT").HasPrecision(6, 4);
          entity.Property(e => e.MargemAut).HasColumnName("MARGEM_AUT").HasPrecision(6, 4);
          entity.Property(e => e.PrecoMinimo).HasColumnName("PRECOMINIMO").HasPrecision(18, 6);
          entity.HasOne(e => e.Produto)
                  .WithMany()
                  .HasForeignKey(e => e.CodigoProduto)
                  .HasPrincipalKey(p => p.Id);
      } 

      private static void ConfigureCoteFacilPedido(ModelBuilder modelBuilder)
      {
            var entity = modelBuilder.Entity<CoteFacilPedido>();

            entity.ToTable("DSGESTOR_COTEFACIL_PEDIDO");

            entity.HasKey(e => e.IdPedido);

            entity.Property(e => e.IdPedido)
                  .HasColumnName("ID_PEDIDO")
                  .HasPrecision(18, 0)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.CnpjDistribuidor)
                  .HasColumnName("CNPJ_DISTRIBUIDOR")
                  .HasMaxLength(14)
                  .IsRequired()
                  .IsUnicode(false);

            entity.Property(e => e.CodigoDistribuidor)
                  .HasColumnName("CODIGO_DISTRIBUIDOR")
                  .HasPrecision(10, 0);

            entity.Property(e => e.CodfilialWinthor)
                  .HasColumnName("CODFILIAL_WINTHOR")
                  .HasMaxLength(4)
                  .IsUnicode(false);

            entity.Property(e => e.CnpjCliente)
                  .HasColumnName("CNPJ_CLIENTE")
                  .HasMaxLength(14)
                  .IsRequired()
                  .IsUnicode(false);

            entity.Property(e => e.CodcliWinthor)
                  .HasColumnName("CODCLI_WINTHOR")
                  .HasPrecision(10, 0);

            entity.Property(e => e.CotacaoCoteFacil)
                  .HasColumnName("COTACAO_COTEFACIL")
                  .HasPrecision(18, 0);

            entity.Property(e => e.PedidoCoteFacil)
                  .HasColumnName("PEDIDO_COTEFACIL")
                  .HasPrecision(18, 0);

            entity.Property(e => e.PedidoCliente)
                  .HasColumnName("PEDIDO_CLIENTE")
                  .HasMaxLength(60)
                  .IsUnicode(false);

            entity.Property(e => e.CodigoPromocao)
                  .HasColumnName("CODIGO_PROMOCAO")
                  .HasPrecision(18, 0);

            entity.Property(e => e.CodigoPrazoPagamento)
                  .HasColumnName("CODIGO_PRAZO_PAGAMENTO")
                  .HasPrecision(10, 0);

            entity.Property(e => e.CodigoCondicaoComercial)
                  .HasColumnName("CODIGO_CONDICAO_COMERCIAL")
                  .HasPrecision(10, 0);

            entity.Property(e => e.CodplpagWinthor)
                  .HasColumnName("CODPLPAG_WINTHOR")
                  .HasPrecision(10, 0);

            entity.Property(e => e.CodcobWinthor)
                  .HasColumnName("CODCOB_WINTHOR")
                  .HasMaxLength(10)
                  .IsUnicode(false);

            entity.Property(e => e.CodusurWinthor)
                  .HasColumnName("CODUSUR_WINTHOR")
                  .HasPrecision(10, 0);

            entity.Property(e => e.NumpedWinthor)
                  .HasColumnName("NUMPED_WINTHOR")
                  .HasPrecision(18, 0);

            entity.Property(e => e.DataImportacaoWinthor)
                  .HasColumnName("DATA_IMPORTACAO_WINTHOR");

            entity.Property(e => e.Status)
                  .HasColumnName("STATUS")
                  .HasMaxLength(30)
                  .HasDefaultValue("RECEBIDO")
                  .IsRequired()
                  .IsUnicode(false);

            entity.Property(e => e.TentativasProcessamento)
                  .HasColumnName("TENTATIVAS_PROCESSAMENTO")
                  .HasPrecision(5, 0)
                  .HasDefaultValue(0);

            entity.Property(e => e.HashRequisicao)
                  .HasColumnName("HASH_REQUISICAO")
                  .HasMaxLength(64)
                  .IsUnicode(false);

            entity.Property(e => e.JsonRequest)
                  .HasColumnName("JSON_REQUEST")
                  .HasColumnType("CLOB");

            entity.Property(e => e.JsonResponse)
                  .HasColumnName("JSON_RESPONSE")
                  .HasColumnType("CLOB");

            entity.Property(e => e.MensagemErro)
                  .HasColumnName("MENSAGEM_ERRO")
                  .HasMaxLength(4000);

            entity.Property(e => e.DataCriacao)
                  .HasColumnName("DATA_CRIACAO")
                  .HasDefaultValueSql("SYSDATE");

            entity.Property(e => e.DataAtualizacao)
                  .HasColumnName("DATA_ATUALIZACAO");

            entity.Property(e => e.DataProcessamento)
                  .HasColumnName("DATA_PROCESSAMENTO");

            entity.Property(e => e.UsuarioCriacao)
                  .HasColumnName("USUARIO_CRIACAO")
                  .HasMaxLength(30)
                  .IsUnicode(false);

            entity.HasMany(e => e.Itens)
                  .WithOne(i => i.Pedido)
                  .HasForeignKey(i => i.IdPedido)
                  .HasConstraintName("FK_DSGESTOR_CF_ITEM_PEDIDO");
      }

      private static void ConfigureCoteFacilPedidoItem(ModelBuilder modelBuilder)
      {
            var entity = modelBuilder.Entity<CoteFacilPedidoItem>();

            entity.ToTable("DSGESTOR_COTEFACIL_PEDIDO_ITEM");

            entity.HasKey(e => e.IdItem);

            entity.Property(e => e.IdItem)
                  .HasColumnName("ID_ITEM")
                  .HasPrecision(18, 0)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.IdPedido)
                  .HasColumnName("ID_PEDIDO")
                  .HasPrecision(18, 0)
                  .IsRequired();

            entity.Property(e => e.Sequencia)
                  .HasColumnName("SEQUENCIA")
                  .HasPrecision(6, 0)
                  .IsRequired();

            entity.Property(e => e.Ean)
                  .HasColumnName("EAN")
                  .HasMaxLength(30)
                  .IsUnicode(false);

            entity.Property(e => e.Dun)
                  .HasColumnName("DUN")
                  .HasMaxLength(30)
                  .IsUnicode(false);

            entity.Property(e => e.CodigoProdutoCoteFacil)
                  .HasColumnName("CODIGO_PRODUTO_COTEFACIL")
                  .HasMaxLength(30)
                  .IsUnicode(false);

            entity.Property(e => e.CodprodWinthor)
                  .HasColumnName("CODPROD_WINTHOR")
                  .HasPrecision(10, 0);

            entity.Property(e => e.DescricaoProduto)
                  .HasColumnName("DESCRICAO_PRODUTO")
                  .HasMaxLength(255);

            entity.Property(e => e.Embalagem)
                  .HasColumnName("EMBALAGEM")
                  .HasMaxLength(60);

            entity.Property(e => e.Unidade)
                  .HasColumnName("UNIDADE")
                  .HasMaxLength(10)
                  .IsUnicode(false);

            entity.Property(e => e.QuantidadeSolicitada)
                  .HasColumnName("QUANTIDADE_SOLICITADA")
                  .HasPrecision(18, 6)
                  .HasDefaultValue(0);

            entity.Property(e => e.QuantidadeAtendida)
                  .HasColumnName("QUANTIDADE_ATENDIDA")
                  .HasPrecision(18, 6);

            entity.Property(e => e.ValorFabrica)
                  .HasColumnName("VALOR_FABRICA")
                  .HasPrecision(18, 6);

            entity.Property(e => e.ValorUnitario)
                  .HasColumnName("VALOR_UNITARIO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.ValorUnitarioNf)
                  .HasColumnName("VALOR_UNITARIO_NF")
                  .HasPrecision(18, 6);

            entity.Property(e => e.ValorUnitarioBoleto)
                  .HasColumnName("VALOR_UNITARIO_BOLETO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.ValorTotalItem)
                  .HasColumnName("VALOR_TOTAL_ITEM")
                  .HasPrecision(18, 6);

            entity.Property(e => e.DescontoAdicional)
                  .HasColumnName("DESCONTO_ADICIONAL")
                  .HasPrecision(18, 6);

            entity.Property(e => e.ValorDescAdicional)
                  .HasColumnName("VALOR_DESC_ADICIONAL")
                  .HasPrecision(18, 6);

            entity.Property(e => e.DescontoBonificacao)
                  .HasColumnName("DESCONTO_BONIFICACAO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.ValorDescBonificacao)
                  .HasColumnName("VALOR_DESC_BONIFICACAO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.DescontoComercial)
                  .HasColumnName("DESCONTO_COMERCIAL")
                  .HasPrecision(18, 6);

            entity.Property(e => e.ValorDescComercial)
                  .HasColumnName("VALOR_DESC_COMERCIAL")
                  .HasPrecision(18, 6);

            entity.Property(e => e.DescontoFinanceiro)
                  .HasColumnName("DESCONTO_FINANCEIRO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.ValorDescFinanceiro)
                  .HasColumnName("VALOR_DESC_FINANCEIRO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.CodigoPromocao)
                  .HasColumnName("CODIGO_PROMOCAO")
                  .HasPrecision(18, 0);

            entity.Property(e => e.ValorUnitarioCalculado)
                  .HasColumnName("VALOR_UNITARIO_CALCULADO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.EstoqueDisponivel)
                  .HasColumnName("ESTOQUE_DISPONIVEL")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Status)
                  .HasColumnName("STATUS")
                  .HasMaxLength(30)
                  .HasDefaultValue("RECEBIDO")
                  .IsRequired()
                  .IsUnicode(false);

            entity.Property(e => e.MensagemErro)
                  .HasColumnName("MENSAGEM_ERRO")
                  .HasMaxLength(4000);

            entity.Property(e => e.DataCriacao)
                  .HasColumnName("DATA_CRIACAO")
                  .HasDefaultValueSql("SYSDATE");

            entity.Property(e => e.DataAtualizacao)
                  .HasColumnName("DATA_ATUALIZACAO");
      }

      private static void ConfigureCoteFacilFilialDistribuidor(ModelBuilder modelBuilder)
      {
            var entity = modelBuilder.Entity<CoteFacilFilialDistribuidor>();

            entity.ToTable("PCFILIAL");

            entity.HasKey(e => e.CodigoFilial);

            entity.Property(e => e.CodigoFilial)
                  .HasColumnName("CODIGO")
                  .HasMaxLength(4)
                  .IsUnicode(false);

            entity.Property(e => e.RazaoSocial)
                  .HasColumnName("RAZAOSOCIAL")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.NomeFantasia)
                  .HasColumnName("FANTASIA")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.Cnpj)
                  .HasColumnName("CGC")
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.Property(e => e.Cidade)
                  .HasColumnName("CIDADE")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.Uf)
                  .HasColumnName("UF")
                  .HasMaxLength(2)
                  .IsUnicode(false);
      }

      private static void ConfigureCoteFacilCondicaoPagamento(ModelBuilder modelBuilder)
      {
            var entity = modelBuilder.Entity<CoteFacilCondicaoPagamento>();

            entity.ToTable("PCPLPAG");

            entity.HasKey(e => e.CodigoCondicaoPagamento);

            entity.Property(e => e.CodigoCondicaoPagamento)
                  .HasColumnName("CODPLPAG")
                  .HasPrecision(10, 0)
                  .ValueGeneratedNever();

            entity.Property(e => e.Descricao)
                  .HasColumnName("DESCRICAO")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.NumDias)
                  .HasColumnName("NUMDIAS")
                  .HasPrecision(10, 0);
      }
      private static void ConfigureCoteFacilProdutoView(ModelBuilder modelBuilder)
      {
            var entity = modelBuilder.Entity<CoteFacilProdutoView>();

            entity.ToView("VIEW_DSGESTOR_COTEFACIL_PRODUTO");

            entity.HasNoKey();

            entity.Property(e => e.Codfilial)
                  .HasColumnName("CODFILIAL")
                  .HasMaxLength(4)
                  .IsUnicode(false);

            entity.Property(e => e.CnpjDistribuidor)
                  .HasColumnName("CNPJ_DISTRIBUIDOR")
                  .HasMaxLength(14)
                  .IsUnicode(false);

            entity.Property(e => e.RazaoSocialDistribuidor)
                  .HasColumnName("RAZAO_SOCIAL_DISTRIBUIDOR")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.FantasiaDistribuidor)
                  .HasColumnName("FANTASIA_DISTRIBUIDOR")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.CodigoProduto)
                  .HasColumnName("CODIGO_PRODUTO")
                  .HasPrecision(10, 0);

            entity.Property(e => e.Ean)
                  .HasColumnName("EAN")
                  .HasMaxLength(30)
                  .IsUnicode(false);

            entity.Property(e => e.Dun)
                  .HasColumnName("DUN")
                  .HasMaxLength(30)
                  .IsUnicode(false);

            entity.Property(e => e.Descricao)
                  .HasColumnName("DESCRICAO")
                  .HasMaxLength(200)
                  .IsUnicode(false);

            entity.Property(e => e.Unidade)
                  .HasColumnName("UNIDADE")
                  .HasMaxLength(10)
                  .IsUnicode(false);

            entity.Property(e => e.EmbalagemEan)
                  .HasColumnName("EMBALAGEM_EAN")
                  .HasMaxLength(10)
                  .IsUnicode(false);

            entity.Property(e => e.QtdeEan)
                  .HasColumnName("QTDE_EAN")
                  .HasPrecision(18, 6);

            entity.Property(e => e.EmbalagemDun)
                  .HasColumnName("EMBALAGEM_DUN")
                  .HasMaxLength(10)
                  .IsUnicode(false);

            entity.Property(e => e.QtdeDun)
                  .HasColumnName("QTDE_DUN")
                  .HasPrecision(18, 6);

            entity.Property(e => e.TipoProduto)
                  .HasColumnName("TIPO_PRODUTO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Ncm)
                  .HasColumnName("NCM")
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.Property(e => e.CnpjFornecedor)
                  .HasColumnName("CNPJ_FORNECEDOR")
                  .HasMaxLength(14)
                  .IsUnicode(false);

            entity.Property(e => e.NomeFornecedor)
                  .HasColumnName("NOME_FORNECEDOR")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.CnpjFabricante)
                  .HasColumnName("CNPJ_FABRICANTE")
                  .HasMaxLength(14)
                  .IsUnicode(false);

            entity.Property(e => e.Fabricante)
                  .HasColumnName("FABRICANTE")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.Departamento)
                  .HasColumnName("DEPARTAMENTO")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.Categoria1)
                  .HasColumnName("CATEGORIA1")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.Categoria2)
                  .HasColumnName("CATEGORIA2")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.Categoria3)
                  .HasColumnName("CATEGORIA3")
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.NumRegiao)
                  .HasColumnName("NUMREGIAO")
                  .HasPrecision(10, 0);

            entity.Property(e => e.PrecoFabrica)
                  .HasColumnName("PRECO_FABRICA")
                  .HasPrecision(18, 6);

            entity.Property(e => e.DescontoMaximo)
                  .HasColumnName("DESCONTO_MAXIMO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.PrecoVenda)
                  .HasColumnName("PRECO_VENDA")
                  .HasPrecision(18, 6);

            entity.Property(e => e.EstoqueGeral)
                  .HasColumnName("ESTOQUE_GERAL")
                  .HasPrecision(18, 6);

            entity.Property(e => e.EstoqueReservado)
                  .HasColumnName("ESTOQUE_RESERVADO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.EstoqueBloqueado)
                  .HasColumnName("ESTOQUE_BLOQUEADO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.EstoqueDisponivel)
                  .HasColumnName("ESTOQUE_DISPONIVEL")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Imagem)
                  .HasColumnName("IMAGEM")
                  .HasMaxLength(500)
                  .IsUnicode(false);

            entity.Property(e => e.Ativo)
                  .HasColumnName("ATIVO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Observacao)
                  .HasColumnName("OBSERVACAO")
                  .HasMaxLength(1000)
                  .IsUnicode(false);
      }
      private static void ConfigurePcPedc(ModelBuilder modelBuilder)
      {
            var entity = modelBuilder.Entity<PcPedc>();

            entity.ToTable("PCPEDC");

            entity.HasKey(e => e.Numped);

            entity.Property(e => e.Numped)
                  .HasColumnName("NUMPED")
                  .HasPrecision(10, 0)
                  .ValueGeneratedNever();

            entity.Property(e => e.Data)
                  .HasColumnName("DATA")
                  .IsRequired();

            entity.Property(e => e.Vltotal)
                  .HasColumnName("VLTOTAL")
                  .HasPrecision(16, 3);

            entity.Property(e => e.Codcli)
                  .HasColumnName("CODCLI")
                  .HasPrecision(9, 0)
                  .IsRequired();

            entity.Property(e => e.Codusur)
                  .HasColumnName("CODUSUR")
                  .HasPrecision(4, 0)
                  .IsRequired();

            entity.Property(e => e.Dtentrega)
                  .HasColumnName("DTENTREGA");

            entity.Property(e => e.Vltabela)
                  .HasColumnName("VLTABELA")
                  .HasPrecision(16, 3);

            entity.Property(e => e.Codfilial)
                  .HasColumnName("CODFILIAL")
                  .HasMaxLength(2)
                  .IsUnicode(false)
                  .IsRequired();

            entity.Property(e => e.Vldesconto)
                  .HasColumnName("VLDESCONTO")
                  .HasPrecision(16, 3);

            entity.Property(e => e.Tipovenda)
                  .HasColumnName("TIPOVENDA")
                  .HasMaxLength(2)
                  .IsUnicode(false);

            entity.Property(e => e.Obs)
                  .HasColumnName("OBS")
                  .HasMaxLength(25)
                  .IsUnicode(false);

            entity.Property(e => e.Vlcustoreal)
                  .HasColumnName("VLCUSTOREAL")
                  .HasPrecision(14, 2);

            entity.Property(e => e.Vlcustofin)
                  .HasColumnName("VLCUSTOFIN")
                  .HasPrecision(14, 2);

            entity.Property(e => e.Vlfrete)
                  .HasColumnName("VLFRETE")
                  .HasPrecision(16, 3);

            entity.Property(e => e.Vloutrasdesp)
                  .HasColumnName("VLOUTRASDESP")
                  .HasPrecision(16, 3);

            entity.Property(e => e.Totpeso)
                  .HasColumnName("TOTPESO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Totvolume)
                  .HasColumnName("TOTVOLUME")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Codpraca)
                  .HasColumnName("CODPRACA")
                  .HasPrecision(4, 0)
                  .IsRequired();

            entity.Property(e => e.Numitens)
                  .HasColumnName("NUMITENS")
                  .HasPrecision(4, 0);

            entity.Property(e => e.Codemitente)
                  .HasColumnName("CODEMITENTE")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Dtcancel)
                  .HasColumnName("DTCANCEL");

            entity.Property(e => e.Posicao)
                  .HasColumnName("POSICAO")
                  .HasMaxLength(2)
                  .IsUnicode(false)
                  .IsRequired();

            entity.Property(e => e.Vlatend)
                  .HasColumnName("VLATEND")
                  .HasPrecision(16, 3);

            entity.Property(e => e.Operacao)
                  .HasColumnName("OPERACAO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Numcar)
                  .HasColumnName("NUMCAR")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Codcob)
                  .HasColumnName("CODCOB")
                  .HasMaxLength(4)
                  .IsUnicode(false);

            entity.Property(e => e.Hora)
                  .HasColumnName("HORA")
                  .HasPrecision(2, 0);

            entity.Property(e => e.Minuto)
                  .HasColumnName("MINUTO")
                  .HasPrecision(2, 0);

            entity.Property(e => e.Numseqentrega)
                  .HasColumnName("NUMSEQENTREGA")
                  .HasPrecision(20, 0);

            entity.Property(e => e.Custoentrega)
                  .HasColumnName("CUSTOENTREGA")
                  .HasPrecision(14, 2);

            entity.Property(e => e.Codsupervisor)
                  .HasColumnName("CODSUPERVISOR")
                  .HasPrecision(4, 0)
                  .IsRequired();

            entity.Property(e => e.Campanha)
                  .HasColumnName("CAMPANHA")
                  .HasMaxLength(2)
                  .IsUnicode(false);

            entity.Property(e => e.Numpedcli)
                  .HasColumnName("NUMPEDCLI")
                  .HasMaxLength(15)
                  .IsUnicode(false);

            entity.Property(e => e.Condvenda)
                  .HasColumnName("CONDVENDA")
                  .HasPrecision(5, 0);

            entity.Property(e => e.Percvenda)
                  .HasColumnName("PERCVENDA")
                  .HasPrecision(5, 2);

            entity.Property(e => e.Obs1)
                  .HasColumnName("OBS1")
                  .HasMaxLength(50)
                  .IsUnicode(false);

            entity.Property(e => e.Obs2)
                  .HasColumnName("OBS2")
                  .HasMaxLength(50)
                  .IsUnicode(false);

            entity.Property(e => e.Perdesc)
                  .HasColumnName("PERDESC")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Negociado)
                  .HasColumnName("NEGOCIADO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Codplpag)
                  .HasColumnName("CODPLPAG")
                  .HasPrecision(4, 0)
                  .IsRequired();

            entity.Property(e => e.Codfunccancel)
                  .HasColumnName("CODFUNCCANCEL")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Numtransvenda)
                  .HasColumnName("NUMTRANSVENDA")
                  .HasPrecision(10, 0);

            entity.Property(e => e.Montando)
                  .HasColumnName("MONTANDO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Numpedrca)
                  .HasColumnName("NUMPEDRCA")
                  .HasPrecision(10, 0);

            entity.Property(e => e.Fretdespacho)
                  .HasColumnName("FRETEDESPACHO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Freteredespacho)
                  .HasColumnName("FRETEREDESPACHO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Codfornecfrete)
                  .HasColumnName("CODFORNECFRETE")
                  .HasPrecision(6, 0);

            entity.Property(e => e.Tipocarga)
                  .HasColumnName("TIPOCARGA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Prazo1).HasColumnName("PRAZO1").HasPrecision(4, 0);
            entity.Property(e => e.Prazo2).HasColumnName("PRAZO2").HasPrecision(4, 0);
            entity.Property(e => e.Prazo3).HasColumnName("PRAZO3").HasPrecision(4, 0);
            entity.Property(e => e.Prazo4).HasColumnName("PRAZO4").HasPrecision(4, 0);
            entity.Property(e => e.Prazo5).HasColumnName("PRAZO5").HasPrecision(4, 0);
            entity.Property(e => e.Prazo6).HasColumnName("PRAZO6").HasPrecision(4, 0);
            entity.Property(e => e.Prazo7).HasColumnName("PRAZO7").HasPrecision(4, 0);
            entity.Property(e => e.Prazo8).HasColumnName("PRAZO8").HasPrecision(4, 0);
            entity.Property(e => e.Prazo9).HasColumnName("PRAZO9").HasPrecision(4, 0);
            entity.Property(e => e.Prazo10).HasColumnName("PRAZO10").HasPrecision(4, 0);
            entity.Property(e => e.Prazo11).HasColumnName("PRAZO11").HasPrecision(4, 0);
            entity.Property(e => e.Prazo12).HasColumnName("PRAZO12").HasPrecision(4, 0);

            entity.Property(e => e.Prazomedio)
                  .HasColumnName("PRAZOMEDIO")
                  .HasPrecision(4, 0);

            entity.Property(e => e.Obsentrega1)
                  .HasColumnName("OBSENTREGA1")
                  .HasMaxLength(75)
                  .IsUnicode(false);

            entity.Property(e => e.Obsentrega2)
                  .HasColumnName("OBSENTREGA2")
                  .HasMaxLength(75)
                  .IsUnicode(false);

            entity.Property(e => e.Obsentrega3)
                  .HasColumnName("OBSENTREGA3")
                  .HasMaxLength(75)
                  .IsUnicode(false);

            entity.Property(e => e.Codepto)
                  .HasColumnName("CODEPTO")
                  .HasPrecision(6, 0);

            entity.Property(e => e.Tipoembalagem)
                  .HasColumnName("TIPOEMBALAGEM")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Dtlibera)
                  .HasColumnName("DTLIBERA");

            entity.Property(e => e.Codfilialnf)
                  .HasColumnName("CODFILIALNF")
                  .HasMaxLength(2)
                  .IsUnicode(false);

            entity.Property(e => e.Origemped)
                  .HasColumnName("ORIGEMPED")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Exportado)
                  .HasColumnName("EXPORTADO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Importado)
                  .HasColumnName("IMPORTADO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Dtimportado)
                  .HasColumnName("DTIMPORTADO");

            entity.Property(e => e.Numregiao)
                  .HasColumnName("NUMREGIAO")
                  .HasPrecision(4, 0);

            entity.Property(e => e.Numpedweb)
                  .HasColumnName("NUMPEDWEB")
                  .HasPrecision(15, 0);

            entity.Property(e => e.Numtabella)
                  .HasColumnName("NUMTABELA")
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.Property(e => e.SistemaLegado)
                  .HasColumnName("SISTEMALEGADO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.NumPedMktPlace)
                  .HasColumnName("NUMPEDMKTPLACE")
                  .HasMaxLength(150)
                  .IsUnicode(false);

            entity.Property(e => e.RotinaLanc)
                  .HasColumnName("ROTINALANC")
                  .HasMaxLength(48)
                  .IsUnicode(false);
      }

      private static void ConfigurePcPedi(ModelBuilder modelBuilder)
      {
            var entity = modelBuilder.Entity<PcPedi>();

            entity.ToTable("PCPEDI");

            entity.HasKey(e => new { e.Numped, e.Numseq });

            entity.Property(e => e.Numped)
                  .HasColumnName("NUMPED")
                  .HasPrecision(10, 0)
                  .ValueGeneratedNever();

            entity.Property(e => e.Data)
                  .HasColumnName("DATA")
                  .IsRequired();

            entity.Property(e => e.Codcli)
                  .HasColumnName("CODCLI")
                  .HasPrecision(9, 0)
                  .IsRequired();

            entity.Property(e => e.Codprod)
                  .HasColumnName("CODPROD")
                  .HasPrecision(6, 0)
                  .IsRequired();

            entity.Property(e => e.Codusur)
                  .HasColumnName("CODUSUR")
                  .HasPrecision(4, 0)
                  .IsRequired();

            entity.Property(e => e.Qt)
                  .HasColumnName("QT")
                  .HasPrecision(20, 6)
                  .IsRequired();

            entity.Property(e => e.Pvenda)
                  .HasColumnName("PVENDA")
                  .HasPrecision(18, 6)
                  .IsRequired();

            entity.Property(e => e.Ptabela)
                  .HasColumnName("PTABELA")
                  .HasPrecision(18, 6)
                  .IsRequired();

            entity.Property(e => e.Numcar)
                  .HasColumnName("NUMCAR")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Posicao)
                  .HasColumnName("POSICAO")
                  .HasMaxLength(2)
                  .IsUnicode(false)
                  .IsRequired();

            entity.Property(e => e.St)
                  .HasColumnName("ST")
                  .HasPrecision(18, 6)
                  .IsRequired();

            entity.Property(e => e.Vlcustofin)
                  .HasColumnName("VLCUSTOFIN")
                  .HasPrecision(18, 6)
                  .IsRequired();

            entity.Property(e => e.Vlcustoreal)
                  .HasColumnName("VLCUSTOREAL")
                  .HasPrecision(18, 6)
                  .IsRequired();

            entity.Property(e => e.Percom)
                  .HasColumnName("PERCOM")
                  .HasPrecision(8, 4)
                  .IsRequired();

            entity.Property(e => e.Perdesc)
                  .HasColumnName("PERDESC")
                  .HasPrecision(18, 6)
                  .IsRequired();

            entity.Property(e => e.Qtfalta)
                  .HasColumnName("QTFALTA")
                  .HasPrecision(20, 6);

            entity.Property(e => e.Numseq)
                  .HasColumnName("NUMSEQ")
                  .HasPrecision(20, 0)
                  .ValueGeneratedNever();

            entity.Property(e => e.Tipopeso)
                  .HasColumnName("TIPOPESO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Percomtab)
                  .HasColumnName("PERCOMTAB")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Perdesctab)
                  .HasColumnName("PERDESCTAB")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Codmotnaocompra)
                  .HasColumnName("CODMOTNAOCOMPRA")
                  .HasPrecision(6, 0);

            entity.Property(e => e.Vldesccustocmv)
                  .HasColumnName("VLDESCCUSTOCMV")
                  .HasPrecision(12, 4);

            entity.Property(e => e.Qtseparada)
                  .HasColumnName("QTSEPARADA")
                  .HasPrecision(20, 6);

            entity.Property(e => e.Qtvendaemb)
                  .HasColumnName("QTVENDAEMB")
                  .HasPrecision(12, 3);

            entity.Property(e => e.Pvendaemb)
                  .HasColumnName("PVENDAEMB")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Vloutros)
                  .HasColumnName("VLOUTROS")
                  .HasPrecision(16, 3);

            entity.Property(e => e.Qtembalagem)
                  .HasColumnName("QTEMBALAGEM")
                  .HasPrecision(12, 3);

            entity.Property(e => e.Pvendaembalagem)
                  .HasColumnName("PVENDAEMBALAGEM")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Codauxiliar)
                  .HasColumnName("CODAUXILIAR")
                  .HasPrecision(20, 0);

            entity.Property(e => e.Vlcustorep)
                  .HasColumnName("VLCUSTOREP")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Vlcustocont)
                  .HasColumnName("VLCUSTOCONT")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Codcertific)
                  .HasColumnName("CODCERTIFIC")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Pvendabase)
                  .HasColumnName("PVENDABASE")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Nomeconcorrente)
                  .HasColumnName("NOMECONCORRENTE")
                  .HasMaxLength(60)
                  .IsUnicode(false);

            entity.Property(e => e.Preco)
                  .HasColumnName("PRECO")
                  .HasPrecision(10, 2);

            entity.Property(e => e.Prazo)
                  .HasColumnName("PRAZO")
                  .HasMaxLength(40)
                  .IsUnicode(false);

            entity.Property(e => e.Qtnaocompra)
                  .HasColumnName("QTNAOCOMPRA")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Codfilialretira)
                  .HasColumnName("CODFILIALRETIRA")
                  .HasMaxLength(2)
                  .IsUnicode(false);

            entity.Property(e => e.Numtira)
                  .HasColumnName("NUMTIRA")
                  .HasPrecision(10, 0);

            entity.Property(e => e.Codfuncsep)
                  .HasColumnName("CODFUNCSEP")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Vldescsuframa)
                  .HasColumnName("VLDESCSUFRAMA")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Numlote)
                  .HasColumnName("NUMLOTE")
                  .HasMaxLength(15)
                  .IsUnicode(false);

            entity.Property(e => e.Vldescrepasse)
                  .HasColumnName("VLDESCREPASSE")
                  .HasPrecision(12, 12);

            entity.Property(e => e.Refcor)
                  .HasColumnName("REFCOR")
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.Property(e => e.Codfuncconf)
                  .HasColumnName("CODFUNCCONF")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Dataconf)
                  .HasColumnName("DATACONF");

            entity.Property(e => e.Vldescicmisencao)
                  .HasColumnName("VLDESCICMISENCAO")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Qtoriginal)
                  .HasColumnName("QTORIGINAL")
                  .HasPrecision(20, 6);

            entity.Property(e => e.Vldescfornec)
                  .HasColumnName("VLDESCFORNEC")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Vlfrete)
                  .HasColumnName("VLFRETE")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Vlipi)
                  .HasColumnName("VLIPI")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Qtorig)
                  .HasColumnName("QTORIG")
                  .HasPrecision(20, 6);

            entity.Property(e => e.Qtsepararun)
                  .HasColumnName("QTSEPARARUN")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Qtsepararcx)
                  .HasColumnName("QTSEPARARCX")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Codst)
                  .HasColumnName("CODST")
                  .HasPrecision(4, 0);

            entity.Property(e => e.Vldescfin)
                  .HasColumnName("VLDESCFIN")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Percipi)
                  .HasColumnName("PERCIPI")
                  .HasPrecision(12, 4);

            entity.Property(e => e.Iva)
                  .HasColumnName("IVA")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Aliqicms1)
                  .HasColumnName("ALIQICMS1")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Aliqicms2)
                  .HasColumnName("ALIQICMS2")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Pauta)
                  .HasColumnName("PAUTA")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Percbasered)
                  .HasColumnName("PERCBASERED")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Vldesccom)
                  .HasColumnName("VLDESCCOM")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Perdesccom)
                  .HasColumnName("PERDESCCOM")
                  .HasPrecision(12, 4);

            entity.Property(e => e.Perdescfin)
                  .HasColumnName("PERDESCFIN")
                  .HasPrecision(12, 4);

            entity.Property(e => e.Vlbonific)
                  .HasColumnName("VLBONIFIC")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Perbonific)
                  .HasColumnName("PERBONIFIC")
                  .HasPrecision(12, 4);

            entity.Property(e => e.Poriginal)
                  .HasColumnName("PORIGINAL")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Vlrebaixacmv)
                  .HasColumnName("VLREBAIXACMV")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Numaplic)
                  .HasColumnName("NUMAPLIC")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Perfretecmv)
                  .HasColumnName("PERFRETECMV")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Vldescrodape)
                  .HasColumnName("VLDESCRODAPE")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Stclientegnre)
                  .HasColumnName("STCLIENTEGNRE")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Imprime)
                  .HasColumnName("IMPRIME")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Complemento)
                  .HasColumnName("COMPLEMENTO")
                  .HasMaxLength(40)
                  .IsUnicode(false);

            entity.Property(e => e.Custofinest)
                  .HasColumnName("CUSTOFINEST")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Percbaseredstfonte)
                  .HasColumnName("PERCBASEREDSTFONTE")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Percbaseredst)
                  .HasColumnName("PERCBASEREDST")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Perdesccusto)
                  .HasColumnName("PERDESCCUSTO")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Codicmtab)
                  .HasColumnName("CODICMTAB")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Txvenda)
                  .HasColumnName("TXVENDA")
                  .HasPrecision(8, 6);

            entity.Property(e => e.Percom2)
                  .HasColumnName("PERCOM2")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Percom3)
                  .HasColumnName("PERCOM3")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Perciss)
                  .HasColumnName("PERCISS")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Vliss)
                  .HasColumnName("VLISS")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Numtranswms)
                  .HasColumnName("NUMTRANSWMS")
                  .HasPrecision(10, 0);

            entity.Property(e => e.Codpromocao)
                  .HasColumnName("CODPROMOCAO")
                  .HasMaxLength(10)
                  .IsUnicode(false);

            entity.Property(e => e.Prazomedio)
                  .HasColumnName("PRAZOMEDIO")
                  .HasPrecision(4, 0);

            entity.Property(e => e.Localizacao)
                  .HasColumnName("LOCALIZACAO")
                  .HasMaxLength(40)
                  .IsUnicode(false);

            entity.Property(e => e.Vlrepasse)
                  .HasColumnName("VLREPASSE")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Pbonific)
                  .HasColumnName("PBONIFIC")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Percvenda)
                  .HasColumnName("PERCVENDA")
                  .HasPrecision(5, 2);

            entity.Property(e => e.Vldescpissuframa)
                  .HasColumnName("VLDESCPISSUFRAMA")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Coddegustacao)
                  .HasColumnName("CODDEGUSTACAO")
                  .HasPrecision(10, 0);

            entity.Property(e => e.Qtlocalizada)
                  .HasColumnName("QTLOCALIZADA")
                  .HasPrecision(8, 2);

            entity.Property(e => e.Perdescflex)
                  .HasColumnName("PERDESCFLEX")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Vldescflex)
                  .HasColumnName("VLDESCFLEX")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Perredcomiss)
                  .HasColumnName("PERREDCOMISS")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Vlredcomiss)
                  .HasColumnName("VLREDCOMISS")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Tipodescaplicado)
                  .HasColumnName("TIPODESCAPLICADO")
                  .HasMaxLength(2)
                  .IsUnicode(false);

            entity.Property(e => e.Pbaserca)
                  .HasColumnName("PBASERCA")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Pesobruto)
                  .HasColumnName("PESOBRUTO")
                  .HasPrecision(7, 3);

            entity.Property(e => e.Numverbarebcmv)
                  .HasColumnName("NUMVERBAREBCMV")
                  .HasPrecision(6, 0);

            entity.Property(e => e.Condvenda)
                  .HasColumnName("CONDVENDA")
                  .HasPrecision(5, 0);

            entity.Property(e => e.Codplpag)
                  .HasColumnName("CODPLPAG")
                  .HasPrecision(4, 0);

            entity.Property(e => e.Eancodprod)
                  .HasColumnName("EANCODPROD")
                  .HasPrecision(14, 0);

            entity.Property(e => e.Brinde)
                  .HasColumnName("BRINDE")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Percomsup)
                  .HasColumnName("PERCOMSUP")
                  .HasPrecision(8, 4);

            entity.Property(e => e.Perredcomisssup)
                  .HasColumnName("PERREDCOMISSSUP")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Vlredcomisssup)
                  .HasColumnName("VLREDCOMISSSUP")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Baseicst)
                  .HasColumnName("BASEICST")
                  .HasPrecision(18, 6);

            entity.Property(e => e.Numop)
                  .HasColumnName("NUMOP")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Qtcx)
                  .HasColumnName("QTCX")
                  .HasPrecision(14, 6);

            entity.Property(e => e.Qtpecas)
                  .HasColumnName("QTPECAS")
                  .HasPrecision(14, 6);

            entity.Property(e => e.Codfunclanc)
                  .HasColumnName("CODFUNCLANC")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Rotinalanc)
                  .HasColumnName("ROTINALANC")
                  .HasPrecision(6, 0);

            entity.Property(e => e.Dtlanc)
                  .HasColumnName("DTLANC");

            entity.Property(e => e.Codfuncultalter)
                  .HasColumnName("CODFUNCULTALTER")
                  .HasPrecision(8, 0);

            entity.Property(e => e.Rotinaultlalter)
                  .HasColumnName("ROTINAULTLALTER")
                  .HasPrecision(6, 0);

            entity.Property(e => e.Dtultlalter)
                  .HasColumnName("DTULTLALTER");

            entity.Property(e => e.Codsupervisor)
                  .HasColumnName("CODSUPERVISOR")
                  .HasPrecision(4, 0);

            entity.Property(e => e.Numpedcli)
                  .HasColumnName("NUMPEDCLI")
                  .HasMaxLength(15)
                  .IsUnicode(false);
      }
}
