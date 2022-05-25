using Microsoft.EntityFrameworkCore;
using WorkTimeMonitor.DTO.Commands;
using WorkTimeMonitor.Web;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Db");
builder.Services.AddDbContext<Database>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/cards", async (Database db) =>
{
    var cards = await db.Cards.Select(s => new
    {
        s.UserName,
        s.CardId
    }).ToListAsync();
    return Results.Ok(cards);
});

app.MapGet("/api/cards/{cardId}/history", async (string cardId, Database db) =>
{
    var cardEntity = await db.Cards
        .Include(i => i.History)
        .FirstOrDefaultAsync(w => w.CardId == cardId);

    if (cardEntity == null)
        return Results.NotFound();

    var results = new
    {
        cardEntity.CardId,
        cardEntity.UserName,
        History = cardEntity.History.Select(s => new
        {
            Status = s.Status.ToString(),
            s.TimeStamp
        })
    };

    return Results.Ok(results);
});

app.MapPost("/api/cards/history", async (CreateCardHistoryCommand command, Database db) =>
{
    var cardEntity = await db.Cards.FirstOrDefaultAsync(w => w.CardId == command.CardId);

    if (cardEntity == null)
    {
        var userName = $"User {Guid.NewGuid()}";
        cardEntity = new CardEntity(command.CardId, userName);
        db.Cards.Add(cardEntity);
    }

    var lastHistoryEntity = await db.CardHistory.Where(w => w.CardId == cardEntity.CardId)
        .OrderByDescending(by => by.TimeStamp).FirstOrDefaultAsync();

    var cardStatus = lastHistoryEntity == null
        ? CardStatus.In
        : lastHistoryEntity.Status == CardStatus.In
            ? CardStatus.Out
            : CardStatus.In;

    cardEntity.AddHistory(cardStatus);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();
