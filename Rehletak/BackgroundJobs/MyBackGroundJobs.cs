using Rehletak.Presistense.Contexts;

namespace Rehletak.Web.BackgroundJobs
{
    public class MyBackGroundJobs(RehletakDbContext context)
    {

       public async Task CleanNotActiveTokens()
        {
            var notActiveTokens = context.refresh_tokens.Where(t => t.expires_at < DateTime.UtcNow || t.revoked_at != null).ToList();
            if (notActiveTokens.Any())
            {
                context.refresh_tokens.RemoveRange(notActiveTokens);
                await context.SaveChangesAsync();
            }
        }

    }
}
