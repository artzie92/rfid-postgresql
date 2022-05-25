using Microsoft.EntityFrameworkCore;

namespace WorkTimeMonitor.Web
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options) : base(options)
        {
        }

        public DbSet<CardEntity> Cards { get; protected set; }
        public DbSet<CardHistoryEntity> CardHistory { get; protected set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var cardEntity = modelBuilder.Entity<CardEntity>();
            cardEntity.ToTable("card");
            cardEntity.HasKey(pk => pk.CardId);
            cardEntity.Property(p => p.CardId)
                .ValueGeneratedNever()
                .HasMaxLength(50)
                .IsRequired();
            cardEntity.Property(p => p.UserName).IsRequired().HasMaxLength(250);
            cardEntity.Metadata
                .FindNavigation(nameof(CardEntity.History))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            var cardHistoryEntity = modelBuilder.Entity<CardHistoryEntity>();
            cardHistoryEntity.ToTable("cardHistory");
            cardHistoryEntity.HasOne(o => o.Card)
            .WithMany(m => m.History)
            .HasForeignKey(fk => fk.CardId);
        }
    }

    public class CardEntity
    {
        public string CardId { get; protected set; }
        public string UserName { get; protected set; }

        private List<CardHistoryEntity> history = new List<CardHistoryEntity>();
        public IReadOnlyCollection<CardHistoryEntity> History => history;

        public CardEntity(string cardId, string userName)
        {
            CardId = cardId;
            UserName = userName;
        }

        public CardHistoryEntity AddHistory(CardStatus status)
        {
            var entity = new CardHistoryEntity(status);
            history.Add(entity);
            return entity;
        }
    }

    public class CardHistoryEntity
    {
        public CardHistoryEntity(CardStatus status)
        {
            TimeStamp = DateTime.UtcNow;
            Status = status;
        }

        public int Id { get; protected set; }
        public DateTime TimeStamp { get; protected set; }
        public CardStatus Status { get; protected set; }

        public string CardId { get; protected set; }
        public CardEntity Card { get; protected set; }
    }

    public enum CardStatus
    {
        In = 1,
        Out = 2
    }
}